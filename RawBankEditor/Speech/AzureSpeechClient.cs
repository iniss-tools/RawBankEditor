using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;
using RawBankEditor.Tools;

namespace RawBankEditor.Speech;

/// <summary>
/// Klient Azure Speech (REST): prevod textu na rec cez SSML, zoznam hlasov oblasti a prepis kratkych nahravok.
/// Casy slov REST nevracia - nosna veta sa striha podla odhadu (<see cref="SpeechGenerator" />).
/// </summary>
public sealed class AzureSpeechClient : ISpeechProvider, IDisposable
{
    /// <summary>Kluc sluzby v profile hlasu.</summary>
    public const string ProviderId = "azure";

    /// <summary>Jediny model - neuronove hlasy.</summary>
    public const string NeuralModel = "neural";

    /// <summary>Model prepisu - REST pre kratke nahravky.</summary>
    public const string SttModel = "short-audio";

    /// <summary>Stranka prostriedkov Speech na portali Azure (kluc a oblast).</summary>
    public const string PortalUrl = "https://portal.azure.com/#view/Microsoft_Azure_ProjectOxford/CognitiveServicesHub/~/SpeechServices";

    // 24 kHz je vlastna vzorkovacia frekvencia hlasov - na 22 050 Hz banky sa prevedie u nas
    private const string OUTPUT_FORMAT = "riff-24khz-16bit-mono-pcm";
    private const int STT_RATE = 16000;
    private const string USER_AGENT = "RawBankEditor";

    private static readonly XNamespace Ssml = "http://www.w3.org/2001/10/synthesis";

    private readonly HttpClient _http;
    private readonly Uri _tts;
    private readonly Uri _voices;
    private readonly Uri _stt;

    /// <param name="key">kluc prostriedku Speech</param>
    /// <param name="location">oblast (napr. <c>westeurope</c>) alebo adresa prostriedku</param>
    /// <exception cref="ArgumentException">kluc je prazdny alebo oblast nie je platna</exception>
    public AzureSpeechClient(string key, string location) : this(new HttpClientHandler(), key, location)
    {
    }

    /// <param name="handler">spracovanie poziadaviek (testy ho nahradia)</param>
    /// <param name="key">kluc prostriedku Speech</param>
    /// <param name="location">oblast (napr. <c>westeurope</c>) alebo adresa prostriedku</param>
    /// <exception cref="ArgumentException">kluc je prazdny alebo oblast nie je platna</exception>
    public AzureSpeechClient(HttpMessageHandler handler, string key, string location)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        (_tts, _voices, _stt) = Endpoints(location);

        _http = new HttpClient(handler) { Timeout = TimeSpan.FromMinutes(2) };
        _http.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key.Trim());
        _http.DefaultRequestHeaders.UserAgent.ParseAdd(USER_AGENT);
    }

    /// <summary>
    /// Cakanie pred opakovanim poziadavky (testy ho nahradia, aby necakali).
    /// </summary>
    internal Func<TimeSpan, CancellationToken, Task> Delay { get; set; } = Task.Delay;

    /// <inheritdoc />
    public string Id => ProviderId;

    /// <inheritdoc />
    public SpeechFeatures Features => SpeechFeatures.Transcribe;

    /// <inheritdoc />
    public string TranscriptionModel => SttModel;

    /// <summary>
    /// Adresy syntezy, zoznamu hlasov a prepisu: pre oblast regionalne adresy sluzby, pre adresu prostriedku
    /// (<c>https://nazov.cognitiveservices.azure.com</c> alebo len nazov hostitela) jeho cesty.
    /// </summary>
    /// <exception cref="ArgumentException">oblast nie je platna</exception>
    internal static (Uri Tts, Uri Voices, Uri Stt) Endpoints(string location)
    {
        var value = (location ?? "").Trim().TrimEnd('/');
        if (value.Contains('.', StringComparison.Ordinal) || value.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            if (!value.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                value = "https://" + value;
            if (!Uri.TryCreate(value, UriKind.Absolute, out var resource) || resource.Scheme != Uri.UriSchemeHttps)
                throw new ArgumentException("Invalid Azure Speech endpoint.", nameof(location));

            var root = resource.GetLeftPart(UriPartial.Authority);
            return (new Uri(root + "/tts/cognitiveservices/v1"), new Uri(root + "/tts/cognitiveservices/voices/list"),
                new Uri(root + "/stt/speech/recognition/conversation/cognitiveservices/v1"));
        }

        if (value.Length == 0 || !value.All(char.IsAsciiLetterOrDigit))
            throw new ArgumentException("Invalid Azure region.", nameof(location));

        var region = value.ToLowerInvariant();
        return (new Uri($"https://{region}.tts.speech.microsoft.com/cognitiveservices/v1"),
            new Uri($"https://{region}.tts.speech.microsoft.com/cognitiveservices/voices/list"),
            new Uri($"https://{region}.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1"));
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SpeechVoice>> GetVoicesAsync(CancellationToken cancel)
    {
        using var response = await SendAsync(() => new HttpRequestMessage(HttpMethod.Get, _voices), cancel).ConfigureAwait(false);
        var text = await response.Content.ReadAsStringAsync(cancel).ConfigureAwait(false);
        try
        {
            using var json = JsonDocument.Parse(text);
            if (json.RootElement.ValueKind != JsonValueKind.Array)
                throw new SpeechException("Unexpected voice list.");

            return json.RootElement.EnumerateArray().Select(ReadVoice).OfType<SpeechVoice>()
                .OrderBy(v => v.Languages.Count > 0 ? v.Languages[0] : "", StringComparer.OrdinalIgnoreCase).ThenBy(v => v.Name, StringComparer.CurrentCulture)
                .ToList();
        }
        catch (JsonException e)
        {
            throw new SpeechException(e.Message, e);
        }
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<SpeechModel>> GetModelsAsync(CancellationToken cancel) =>
        Task.FromResult<IReadOnlyList<SpeechModel>>([new SpeechModel(NeuralModel, "Neural", [])]);

    /// <inheritdoc />
    public Task<SpeechQuota> GetQuotaAsync(CancellationToken cancel) => throw new NotSupportedException();

    /// <inheritdoc />
    public async Task<PcmSound> SynthesizeAsync(SpeechRequest request, CancellationToken cancel)
    {
        ArgumentNullException.ThrowIfNull(request);

        var ssml = SsmlOf(request);
        using var response = await SendAsync(() =>
        {
            var message = new HttpRequestMessage(HttpMethod.Post, _tts) { Content = new StringContent(ssml, Encoding.UTF8) };
            message.Content.Headers.Remove("Content-Type");
            message.Content.Headers.TryAddWithoutValidation("Content-Type", "application/ssml+xml");
            message.Headers.Add("X-Microsoft-OutputFormat", OUTPUT_FORMAT);
            return message;
        }, cancel).ConfigureAwait(false);

        try
        {
            return SoundProcessing.ReadWav(await response.Content.ReadAsByteArrayAsync(cancel).ConfigureAwait(false));
        }
        catch (InvalidDataException e)
        {
            throw new SpeechException(e.Message, e);
        }
    }

    /// <inheritdoc />
    public Task<TimedSpeech> SynthesizeTimedAsync(SpeechRequest request, CancellationToken cancel) => throw new NotSupportedException();

    /// <inheritdoc />
    public Task<PcmSound> ConvertVoiceAsync(VoiceConversionRequest request, CancellationToken cancel) => throw new NotSupportedException();

    /// <inheritdoc />
    public Task<PcmSound> IsolateAsync(byte[] sourceWav, CancellationToken cancel) => throw new NotSupportedException();

    /// <inheritdoc />
    public async Task<string> TranscribeAsync(byte[] sourceWav, string languageCode, CancellationToken cancel)
    {
        ArgumentNullException.ThrowIfNull(sourceWav);

        // sluzba prijima len PCM 16 kHz mono
        byte[] wav;
        try
        {
            wav = SoundProcessing.ToWav(SoundProcessing.Resample(SoundProcessing.ReadWav(sourceWav), STT_RATE));
        }
        catch (InvalidDataException e)
        {
            throw new SpeechException(e.Message, e);
        }

        var url = new Uri(_stt + "?language=" + Uri.EscapeDataString(LocaleOf(languageCode.Length > 0 ? languageCode : "en")) + "&format=simple&profanity=raw");
        using var response = await SendAsync(() =>
        {
            var content = new ByteArrayContent(wav);
            content.Headers.TryAddWithoutValidation("Content-Type", $"audio/wav; codecs=audio/pcm; samplerate={STT_RATE}");
            var message = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
            message.Headers.Accept.ParseAdd("application/json");
            return message;
        }, cancel).ConfigureAwait(false);

        var text = await response.Content.ReadAsStringAsync(cancel).ConfigureAwait(false);
        try
        {
            using var json = JsonDocument.Parse(text);
            return StringOf(json.RootElement, "RecognitionStatus") switch
            {
                "Success" => StringOf(json.RootElement, "DisplayText").Trim(),
                // ticho, sum alebo rec v inom jazyku - nahravka nic nehovori
                "NoMatch" or "InitialSilenceTimeout" or "BabbleTimeout" => "",
                var status => throw new SpeechException($"Recognition failed: {status}.")
            };
        }
        catch (JsonException e)
        {
            throw new SpeechException(e.Message, e);
        }
    }

    /// <inheritdoc />
    public bool SupportsContext(string modelId) => false;

    /// <summary>
    /// SSML poziadavky: hlas, rychlost a pri viacjazycnom hlase jazyk reci.
    /// </summary>
    internal static string SsmlOf(SpeechRequest request)
    {
        var voiceLocale = VoiceLocale(request.VoiceId);
        object content = request.Text;
        if (Math.Abs(request.Speed - 1) > 0.001)
            content = new XElement(Ssml + "prosody", new XAttribute("rate", request.Speed.ToString("0.00", CultureInfo.InvariantCulture)), content);
        // hlas ineho jazyka (viacjazycny) hovori jazykom banky
        if (request.LanguageCode.Length > 0 && !voiceLocale.StartsWith(request.LanguageCode + "-", StringComparison.OrdinalIgnoreCase))
            content = new XElement(Ssml + "lang", new XAttribute(XNamespace.Xml + "lang", LocaleOf(request.LanguageCode)), content);

        var speak = new XElement(Ssml + "speak", new XAttribute("version", "1.0"),
            new XAttribute(XNamespace.Xml + "lang", voiceLocale.Length > 0 ? voiceLocale : LocaleOf(request.LanguageCode)),
            new XElement(Ssml + "voice", new XAttribute("name", request.VoiceId), content));
        return speak.ToString(SaveOptions.DisableFormatting);
    }

    /// <summary>
    /// Jazyk a krajina hlasu z jeho nazvu (<c>sk-SK-ViktoriaNeural</c> = <c>sk-SK</c>).
    /// </summary>
    internal static string VoiceLocale(string voiceId)
    {
        var parts = voiceId.Split('-');
        return parts.Length >= 3 ? parts[0] + "-" + parts[1] : "";
    }

    /// <summary>
    /// Jazyk a krajina pre kod jazyka (ISO 639-1) - sluzba ich vyzaduje pri prepise a znacke jazyka.
    /// </summary>
    public static string LocaleOf(string languageCode) => languageCode.ToLowerInvariant() switch
    {
        "sk" => "sk-SK",
        "cs" => "cs-CZ",
        "en" => "en-GB",
        "de" => "de-DE",
        "pl" => "pl-PL",
        "hu" => "hu-HU",
        "uk" => "uk-UA",
        "fr" => "fr-FR",
        "it" => "it-IT",
        "es" => "es-ES",
        "" => "en-US",
        var code => code
    };

    /// <inheritdoc />
    public void Dispose() => _http.Dispose();

    private Task<HttpResponseMessage> SendAsync(Func<HttpRequestMessage> create, CancellationToken cancel) =>
        SpeechHttp.SendAsync(_http, create, Delay, ErrorMessage, cancel);

    /// <summary>
    /// Sprava z chybovej odpovede: <c>{"error": {"message": ...}}</c> alebo text; prazdne telo = stav HTTP.
    /// </summary>
    internal static string? ErrorMessage(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
            return null;

        try
        {
            using var json = JsonDocument.Parse(body);
            return json.RootElement.TryGetProperty("error", out var error) && StringOf(error, "message") is { Length: > 0 } message ? message : body.Trim();
        }
        catch (JsonException)
        {
            return body.Trim();
        }
    }

    private static SpeechVoice? ReadVoice(JsonElement voice)
    {
        var id = StringOf(voice, "ShortName");
        if (id.Length == 0 || StringOf(voice, "Status").Equals("Deprecated", StringComparison.OrdinalIgnoreCase))
            return null;

        var locales = new List<string> { StringOf(voice, "Locale") };
        if (voice.TryGetProperty("SecondaryLocaleList", out var secondary) && secondary.ValueKind == JsonValueKind.Array)
            locales.AddRange(secondary.EnumerateArray().Where(l => l.ValueKind == JsonValueKind.String).Select(l => l.GetString() ?? ""));
        var languages = locales.Where(l => l.Length > 0).Select(l => l.Split('-')[0].ToLowerInvariant()).Distinct().ToList();

        var name = StringOf(voice, "DisplayName") is { Length: > 0 } display ? display : id;
        var description = string.Join(", ", new[] { StringOf(voice, "Gender"), StringOf(voice, "LocaleName"), id }.Where(s => s.Length > 0));
        return new SpeechVoice(id, name, description) { Category = StringOf(voice, "VoiceType"), Languages = languages };
    }

    private static string StringOf(JsonElement element, string property) =>
        element.ValueKind == JsonValueKind.Object && element.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString() ?? ""
            : "";
}
