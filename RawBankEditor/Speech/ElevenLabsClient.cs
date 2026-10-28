using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using RawBankEditor.Tools;

namespace RawBankEditor.Speech;

/// <summary>
/// Hlasova synteza ElevenLabs (REST API). Rec sa vyziada rovno vo formate banky (<c>pcm_22050</c> - PCM 16 bit mono
/// 22 050 Hz), takze sa neprevzorkuje. Pri preplneni (429) a docasnej nedostupnosti sluzby poziadavku zopakuje.
/// </summary>
public sealed class ElevenLabsClient : ISpeechProvider, IVoiceLibrary, IDisposable
{
    /// <summary>Kluc sluzby v profile hlasu a v cache.</summary>
    public const string ProviderId = "elevenlabs";

    /// <summary>Predvoleny model - stabilny pri kratkych vetach a hovori slovensky aj cesky.</summary>
    public const string DefaultModel = "eleven_multilingual_v2";

    /// <summary>Model zmeny hlasu (speech-to-speech) - hovori slovensky aj cesky.</summary>
    public const string StsModel = "eleven_multilingual_sts_v2";

    /// <summary>Model prepisu reci na text.</summary>
    public const string SttModel = "scribe_v2";

    /// <summary>Modely navrhu hlasu z popisu.</summary>
    public static readonly IReadOnlyList<string> DesignModels = ["eleven_multilingual_ttv_v2", "eleven_ttv_v3"];

    /// <summary>Stranka s klucmi API v ucte ElevenLabs.</summary>
    public const string ApiKeysUrl = "https://elevenlabs.io/app/settings/api-keys";

    private const string OUTPUT_FORMAT = "pcm_22050";
    private const int MAX_VOICE_PAGES = 20;

    private static readonly Uri BaseAddress = new("https://api.elevenlabs.io/");

    // stitky hlasu v ponuke - popis a pouzitie su dlhe a vyber nezjednodusia
    private static readonly string[] VoiceLabels = ["gender", "age", "accent"];

    private readonly HttpClient _http;
    // ukazky hlasov su na verejnych adresach mimo API - bez kluca, ten nesmie odist inam
    private readonly HttpClient _previews;

    /// <param name="apiKey">kluc API (hlavicka <c>xi-api-key</c>)</param>
    public ElevenLabsClient(string apiKey) : this(new HttpClientHandler(), apiKey)
    {
    }

    /// <param name="handler">spracovanie poziadaviek (testy ho nahradia)</param>
    /// <param name="apiKey">kluc API (hlavicka <c>xi-api-key</c>)</param>
    public ElevenLabsClient(HttpMessageHandler handler, string apiKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);

        _http = new HttpClient(handler) { BaseAddress = BaseAddress, Timeout = TimeSpan.FromMinutes(2) };
        _http.DefaultRequestHeaders.Add("xi-api-key", apiKey.Trim());
        _previews = new HttpClient(handler, false) { Timeout = TimeSpan.FromMinutes(1) };
    }

    /// <summary>
    /// Cakanie pred opakovanim poziadavky (testy ho nahradia, aby necakali).
    /// </summary>
    internal Func<TimeSpan, CancellationToken, Task> Delay { get; set; } = Task.Delay;

    /// <inheritdoc />
    public string Id => ProviderId;

    /// <inheritdoc />
    public SpeechFeatures Features =>
        SpeechFeatures.Timings | SpeechFeatures.VoiceSettings | SpeechFeatures.Seed | SpeechFeatures.ConvertVoice | SpeechFeatures.Isolate
        | SpeechFeatures.Transcribe | SpeechFeatures.Quota;

    /// <inheritdoc />
    public string TranscriptionModel => SttModel;

    /// <inheritdoc />
    public async Task<IReadOnlyList<SpeechVoice>> GetVoicesAsync(CancellationToken cancel)
    {
        var voices = new List<SpeechVoice>();
        string? token = null;
        for (var page = 0; page < MAX_VOICE_PAGES; page++)
        {
            var url = "v2/voices?page_size=100&include_total_count=false"
                      + (token is null ? "" : "&next_page_token=" + Uri.EscapeDataString(token));
            using var json = await GetJsonAsync(url, cancel).ConfigureAwait(false);
            var root = json.RootElement;
            if (root.TryGetProperty("voices", out var items) && items.ValueKind == JsonValueKind.Array)
                voices.AddRange(items.EnumerateArray().Select(ReadVoice).OfType<SpeechVoice>());

            token = root.TryGetProperty("has_more", out var more) && more.ValueKind == JsonValueKind.True
                    && root.TryGetProperty("next_page_token", out var next) && next.ValueKind == JsonValueKind.String
                ? next.GetString()
                : null;
            if (token is null)
                break;
        }

        // vlastne hlasy (klonovane, navrhnute) navrch
        return voices.OrderBy(v => v.Own ? 0 : 1).ThenBy(v => v.Name, StringComparer.CurrentCultureIgnoreCase).ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SpeechModel>> GetModelsAsync(CancellationToken cancel)
    {
        using var json = await GetJsonAsync("v1/models", cancel).ConfigureAwait(false);
        if (json.RootElement.ValueKind != JsonValueKind.Array)
            return [];

        return json.RootElement.EnumerateArray()
            .Where(m => !m.TryGetProperty("can_do_text_to_speech", out var tts) || tts.ValueKind != JsonValueKind.False)
            .Select(m => new SpeechModel(
                StringOf(m, "model_id"),
                StringOf(m, "name"),
                m.TryGetProperty("languages", out var languages) && languages.ValueKind == JsonValueKind.Array
                    ? languages.EnumerateArray().Select(l => StringOf(l, "language_id")).Where(l => l.Length > 0).ToList()
                    : []))
            .Where(m => m.Id.Length > 0)
            .ToList();
    }

    /// <inheritdoc />
    public async Task<SpeechQuota> GetQuotaAsync(CancellationToken cancel)
    {
        using var json = await GetJsonAsync("v1/user/subscription", cancel).ConfigureAwait(false);
        var root = json.RootElement;
        return new SpeechQuota(StringOf(root, "tier"), NumberOf(root, "character_count"), NumberOf(root, "character_limit"));
    }

    /// <inheritdoc />
    public async Task<PcmSound> SynthesizeAsync(SpeechRequest request, CancellationToken cancel)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var response = await PostAsync(request, "", cancel).ConfigureAwait(false);
        var pcm = await response.Content.ReadAsByteArrayAsync(cancel).ConfigureAwait(false);
        if (pcm.Length < 2)
            throw new SpeechException("Empty audio returned.");
        return SoundProcessing.FromPcm16(pcm, SoundProcessing.BankSampleRate);
    }

    /// <inheritdoc />
    public async Task<TimedSpeech> SynthesizeTimedAsync(SpeechRequest request, CancellationToken cancel)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var response = await PostAsync(request, "/with-timestamps", cancel).ConfigureAwait(false);
        var text = await response.Content.ReadAsStringAsync(cancel).ConfigureAwait(false);
        try
        {
            using var json = JsonDocument.Parse(text);
            var root = json.RootElement;
            var pcm = Convert.FromBase64String(StringOf(root, "audio_base64"));
            if (pcm.Length < 2)
                throw new SpeechException("Empty audio returned.");
            return new TimedSpeech(SoundProcessing.FromPcm16(pcm, SoundProcessing.BankSampleRate), Timings(root));
        }
        catch (Exception e) when (e is JsonException or FormatException)
        {
            throw new SpeechException(e.Message, e);
        }
    }

    /// <inheritdoc />
    public async Task<PcmSound> ConvertVoiceAsync(VoiceConversionRequest request, CancellationToken cancel)
    {
        ArgumentNullException.ThrowIfNull(request);

        var settings = new JsonObject
        {
            ["stability"] = request.Stability,
            ["similarity_boost"] = request.SimilarityBoost,
            ["style"] = request.Style,
            ["use_speaker_boost"] = request.SpeakerBoost
        }.ToJsonString();
        var url = "v1/speech-to-speech/" + Uri.EscapeDataString(request.VoiceId) + "?output_format=" + OUTPUT_FORMAT;
        using var response = await SendAsync(() =>
        {
            var form = new MultipartFormDataContent
            {
                { Wav(request.SourceWav), "audio", "sound.wav" },
                { new StringContent(request.ModelId), "model_id" },
                { new StringContent(settings), "voice_settings" },
                { new StringContent(request.RemoveBackgroundNoise ? "true" : "false"), "remove_background_noise" }
            };
            if (request.Seed is { } seed)
                form.Add(new StringContent(seed.ToString(System.Globalization.CultureInfo.InvariantCulture)), "seed");
            return new HttpRequestMessage(HttpMethod.Post, url) { Content = form };
        }, cancel).ConfigureAwait(false);

        var pcm = await response.Content.ReadAsByteArrayAsync(cancel).ConfigureAwait(false);
        if (pcm.Length < 2)
            throw new SpeechException("Empty audio returned.");
        return SoundProcessing.FromPcm16(pcm, SoundProcessing.BankSampleRate);
    }

    /// <inheritdoc />
    public async Task<PcmSound> IsolateAsync(byte[] sourceWav, CancellationToken cancel)
    {
        ArgumentNullException.ThrowIfNull(sourceWav);

        using var response = await SendAsync(() => new HttpRequestMessage(HttpMethod.Post, "v1/audio-isolation")
        {
            Content = new MultipartFormDataContent { { Wav(sourceWav), "audio", "sound.wav" } }
        }, cancel).ConfigureAwait(false);

        // format vystupu sluzba neuvadza - rozpozna sa podla obsahu (WAV alebo MP3)
        return Decode(await response.Content.ReadAsByteArrayAsync(cancel).ConfigureAwait(false));
    }

    /// <inheritdoc />
    public async Task<string> TranscribeAsync(byte[] sourceWav, string languageCode, CancellationToken cancel)
    {
        ArgumentNullException.ThrowIfNull(sourceWav);

        using var response = await SendAsync(() =>
        {
            var form = new MultipartFormDataContent
            {
                { new StringContent(SttModel), "model_id" },
                { Wav(sourceWav), "file", "sound.wav" },
                { new StringContent("false"), "tag_audio_events" },
                { new StringContent("none"), "timestamps_granularity" }
            };
            if (languageCode.Length > 0)
                form.Add(new StringContent(languageCode), "language_code");
            return new HttpRequestMessage(HttpMethod.Post, "v1/speech-to-text") { Content = form };
        }, cancel).ConfigureAwait(false);

        var text = await response.Content.ReadAsStringAsync(cancel).ConfigureAwait(false);
        try
        {
            using var json = JsonDocument.Parse(text);
            return StringOf(json.RootElement, "text").Trim();
        }
        catch (JsonException e)
        {
            throw new SpeechException(e.Message, e);
        }
    }

    /// <inheritdoc />
    public async Task<SharedVoicePage> SearchSharedAsync(SharedVoiceQuery query, CancellationToken cancel)
    {
        ArgumentNullException.ThrowIfNull(query);

        var url = new StringBuilder("v1/shared-voices?page_size=50&page=").Append(query.Page.ToString(System.Globalization.CultureInfo.InvariantCulture));
        if (query.Language.Length > 0)
            url.Append("&language=").Append(Uri.EscapeDataString(query.Language));
        if (query.Gender.Length > 0)
            url.Append("&gender=").Append(Uri.EscapeDataString(query.Gender));
        if (query.Search.Trim().Length > 0)
            url.Append("&search=").Append(Uri.EscapeDataString(query.Search.Trim()));

        using var json = await GetJsonAsync(url.ToString(), cancel).ConfigureAwait(false);
        var root = json.RootElement;
        var voices = root.TryGetProperty("voices", out var items) && items.ValueKind == JsonValueKind.Array
            ? items.EnumerateArray()
                .Select(v => new SharedVoice(StringOf(v, "public_owner_id"), StringOf(v, "voice_id"), StringOf(v, "name"), StringOf(v, "gender"),
                    StringOf(v, "age").Replace('_', ' '), StringOf(v, "accent"), StringOf(v, "language"), StringOf(v, "description"), StringOf(v, "preview_url")))
                .Where(v => v.VoiceId.Length > 0 && v.OwnerId.Length > 0)
                .ToList()
            : [];
        return new SharedVoicePage(voices, root.TryGetProperty("has_more", out var more) && more.ValueKind == JsonValueKind.True);
    }

    /// <inheritdoc />
    public async Task<string> AddSharedAsync(SharedVoice voice, string name, CancellationToken cancel)
    {
        ArgumentNullException.ThrowIfNull(voice);

        var url = "v1/voices/add/" + Uri.EscapeDataString(voice.OwnerId) + "/" + Uri.EscapeDataString(voice.VoiceId);
        return await PostJsonForVoiceIdAsync(url, new JsonObject { ["new_name"] = name }, cancel).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<VoicePreview>> DesignAsync(string description, string modelId, string text, CancellationToken cancel)
    {
        var body = new JsonObject { ["voice_description"] = description, ["model_id"] = modelId };
        if (text.Trim().Length > 0)
            body["text"] = text.Trim();
        else
            body["auto_generate_text"] = true;

        var json = body.ToJsonString();
        using var response = await SendAsync(() => new HttpRequestMessage(HttpMethod.Post, "v1/text-to-voice/design?output_format=" + OUTPUT_FORMAT)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        }, cancel).ConfigureAwait(false);

        var result = await response.Content.ReadAsStringAsync(cancel).ConfigureAwait(false);
        try
        {
            using var document = JsonDocument.Parse(result);
            if (!document.RootElement.TryGetProperty("previews", out var previews) || previews.ValueKind != JsonValueKind.Array)
                return [];

            return previews.EnumerateArray()
                .Select(p => new VoicePreview(StringOf(p, "generated_voice_id"),
                    Preview(Convert.FromBase64String(StringOf(p, "audio_base_64")), StringOf(p, "media_type"))))
                .Where(p => p.GeneratedVoiceId.Length > 0)
                .ToList();
        }
        catch (Exception e) when (e is JsonException or FormatException or InvalidDataException)
        {
            throw new SpeechException(e.Message, e);
        }
    }

    /// <inheritdoc />
    public Task<string> CreateDesignedAsync(string name, string description, string generatedVoiceId, CancellationToken cancel) =>
        PostJsonForVoiceIdAsync("v1/text-to-voice",
            new JsonObject { ["voice_name"] = name, ["voice_description"] = description, ["generated_voice_id"] = generatedVoiceId }, cancel);

    /// <inheritdoc />
    public async Task<string> CloneAsync(VoiceCloneRequest request, CancellationToken cancel)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var response = await SendAsync(() =>
        {
            var form = new MultipartFormDataContent
            {
                { new StringContent(request.Name), "name" },
                { new StringContent(request.Description), "description" },
                { new StringContent(request.RemoveBackgroundNoise ? "true" : "false"), "remove_background_noise" }
            };
            if (request.LanguageCode.Length > 0)
                form.Add(new StringContent(new JsonObject { ["language"] = request.LanguageCode }.ToJsonString()), "labels");
            for (var i = 0; i < request.Samples.Count; i++)
                form.Add(Wav(request.Samples[i]), "files", $"sample{i + 1}.wav");
            return new HttpRequestMessage(HttpMethod.Post, "v1/voices/add") { Content = form };
        }, cancel).ConfigureAwait(false);

        return await VoiceIdOf(response, cancel).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task DeleteVoiceAsync(string voiceId, CancellationToken cancel)
    {
        using var response = await SendAsync(() => new HttpRequestMessage(HttpMethod.Delete, "v1/voices/" + Uri.EscapeDataString(voiceId)), cancel)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<PcmSound> DownloadPreviewAsync(string url, CancellationToken cancel)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
            throw new SpeechException("Invalid preview address.");

        try
        {
            return Decode(await _previews.GetByteArrayAsync(uri, cancel).ConfigureAwait(false));
        }
        catch (HttpRequestException e)
        {
            throw new SpeechException(e.Message, e);
        }
    }

    private static ByteArrayContent Wav(byte[] wav)
    {
        var content = new ByteArrayContent(wav);
        content.Headers.ContentType = new MediaTypeHeaderValue("audio/wav");
        return content;
    }

    private static PcmSound Decode(byte[] audio)
    {
        try
        {
            return SoundProcessing.ReadAudio(audio);
        }
        catch (InvalidDataException e)
        {
            throw new SpeechException(e.Message, e);
        }
    }

    // ukazky navrhu su pri output_format pcm_22050 surove PCM; ak sluzba vrati MP3 alebo WAV, poznat to na type a hlavicke
    private static PcmSound Preview(byte[] audio, string mediaType) =>
        mediaType.Contains("mpeg", StringComparison.OrdinalIgnoreCase) || mediaType.Contains("wav", StringComparison.OrdinalIgnoreCase)
        || (audio.Length >= 4 && ((audio[0] == 'R' && audio[1] == 'I' && audio[2] == 'F' && audio[3] == 'F') || (audio[0] == 'I' && audio[1] == 'D' && audio[2] == '3')))
            ? Decode(audio)
            : SoundProcessing.FromPcm16(audio, SoundProcessing.BankSampleRate);

    private async Task<string> PostJsonForVoiceIdAsync(string url, JsonObject body, CancellationToken cancel)
    {
        var json = body.ToJsonString();
        using var response = await SendAsync(() => new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        }, cancel).ConfigureAwait(false);
        return await VoiceIdOf(response, cancel).ConfigureAwait(false);
    }

    private static async Task<string> VoiceIdOf(HttpResponseMessage response, CancellationToken cancel)
    {
        var text = await response.Content.ReadAsStringAsync(cancel).ConfigureAwait(false);
        try
        {
            using var json = JsonDocument.Parse(text);
            return StringOf(json.RootElement, "voice_id") is { Length: > 0 } id ? id : throw new SpeechException("Missing voice_id.");
        }
        catch (JsonException e)
        {
            throw new SpeechException(e.Message, e);
        }
    }

    /// <inheritdoc />
    public bool SupportsContext(string modelId) =>
        // napajanie poziadaviek (previous_text/next_text) model v3 nepodporuje
        !modelId.StartsWith("eleven_v3", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Casy znakov z odpovede <c>/with-timestamps</c> - polozka <c>alignment</c> pre povodny text.
    /// </summary>
    internal static List<CharTiming> Timings(JsonElement root)
    {
        if (!root.TryGetProperty("alignment", out var alignment) || alignment.ValueKind != JsonValueKind.Object
                                                                  || !alignment.TryGetProperty("characters", out var characters)
                                                                  || !alignment.TryGetProperty("character_start_times_seconds", out var starts)
                                                                  || !alignment.TryGetProperty("character_end_times_seconds", out var ends))
            throw new SpeechException("Missing alignment.");

        var chars = characters.EnumerateArray().Select(c => c.GetString() ?? "").ToList();
        var startList = starts.EnumerateArray().Select(t => t.GetDouble()).ToList();
        var endList = ends.EnumerateArray().Select(t => t.GetDouble()).ToList();
        var count = Math.Min(chars.Count, Math.Min(startList.Count, endList.Count));
        return Enumerable.Range(0, count).Select(i => new CharTiming(chars[i].Length > 0 ? chars[i][0] : ' ', startList[i], endList[i])).ToList();
    }

    private Task<HttpResponseMessage> PostAsync(SpeechRequest request, string endpoint, CancellationToken cancel)
    {
        var body = new JsonObject
        {
            ["text"] = request.Text,
            ["model_id"] = request.ModelId,
            ["voice_settings"] = new JsonObject
            {
                ["stability"] = request.Stability,
                ["similarity_boost"] = request.SimilarityBoost,
                ["style"] = request.Style,
                ["use_speaker_boost"] = request.SpeakerBoost,
                ["speed"] = request.Speed
            }
        };
        if (request.LanguageCode.Length > 0 && SupportsLanguageCode(request.ModelId))
            body["language_code"] = request.LanguageCode;
        if (request.Seed is { } seed)
            body["seed"] = seed;
        if (request.PreviousText.Length > 0)
            body["previous_text"] = request.PreviousText;
        if (request.NextText.Length > 0)
            body["next_text"] = request.NextText;

        var url = "v1/text-to-speech/" + Uri.EscapeDataString(request.VoiceId) + endpoint + "?output_format=" + OUTPUT_FORMAT;
        var json = body.ToJsonString();
        return SendAsync(() => new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        }, cancel);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _previews.Dispose();
        _http.Dispose();
    }

    /// <summary>
    /// Model prijima kod jazyka - Multilingual v2 ho nepodporuje.
    /// </summary>
    public static bool SupportsLanguageCode(string modelId) =>
        !modelId.StartsWith("eleven_multilingual_v2", StringComparison.OrdinalIgnoreCase);

    private async Task<JsonDocument> GetJsonAsync(string url, CancellationToken cancel)
    {
        using var response = await SendAsync(() => new HttpRequestMessage(HttpMethod.Get, url), cancel).ConfigureAwait(false);
        var text = await response.Content.ReadAsStringAsync(cancel).ConfigureAwait(false);
        try
        {
            return JsonDocument.Parse(text);
        }
        catch (JsonException e)
        {
            throw new SpeechException(e.Message, e);
        }
    }

    private Task<HttpResponseMessage> SendAsync(Func<HttpRequestMessage> create, CancellationToken cancel) =>
        SpeechHttp.SendAsync(_http, create, Delay, ErrorMessage, cancel);

    /// <summary>
    /// Sprava z chybovej odpovede: <c>{"detail": {"message": ...}}</c>, <c>{"detail": "..."}</c> alebo zoznam chyb
    /// overenia <c>{"detail": [{"msg": ...}]}</c>.
    /// </summary>
    internal static string? ErrorMessage(string body)
    {
        try
        {
            using var json = JsonDocument.Parse(body);
            if (!json.RootElement.TryGetProperty("detail", out var detail))
                return null;

            return detail.ValueKind switch
            {
                JsonValueKind.String => detail.GetString(),
                JsonValueKind.Object => StringOf(detail, "message") is { Length: > 0 } message ? message : null,
                JsonValueKind.Array => string.Join("; ", detail.EnumerateArray().Select(d => StringOf(d, "msg")).Where(m => m.Length > 0)),
                _ => null
            };
        }
        catch (JsonException)
        {
            return string.IsNullOrWhiteSpace(body) ? null : body.Trim();
        }
    }

    private static SpeechVoice? ReadVoice(JsonElement voice)
    {
        var id = StringOf(voice, "voice_id");
        if (id.Length == 0)
            return null;

        // v ponuke len to, co pomoze vybrat hlas: pohlavie, vek, prizvuk a vlastny hlas (popis a pouzitie su dlhe)
        var labels = voice.TryGetProperty("labels", out var l) && l.ValueKind == JsonValueKind.Object ? l : default;
        var category = StringOf(voice, "category");
        var own = category.Length > 0 && !category.Equals("premade", StringComparison.OrdinalIgnoreCase);
        var description = string.Join(", ", VoiceLabels
            .Select(key => StringOf(labels, key).Replace('_', ' '))
            .Append(own ? category : "")
            .Where(s => s.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase));
        return new SpeechVoice(id, StringOf(voice, "name") is { Length: > 0 } name ? name : id, description)
        {
            Own = own,
            Category = category,
            PreviewUrl = StringOf(voice, "preview_url")
        };
    }

    private static string StringOf(JsonElement element, string property) =>
        element.ValueKind == JsonValueKind.Object && element.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString() ?? ""
            : "";

    private static long NumberOf(JsonElement element, string property) =>
        element.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.Number && value.TryGetInt64(out var number)
            ? number
            : 0;
}
