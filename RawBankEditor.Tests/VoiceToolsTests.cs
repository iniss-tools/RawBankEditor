using System.Net;
using System.Text.Json.Nodes;
using RawBankEditor.Forms;
using RawBankEditor.Speech;
using RawBankEditor.Tools;

namespace RawBankEditor.Tests;

/// <summary>
/// Zmena hlasu, vycistenie nahravok a sprava hlasov (kniznica, navrh, klon, odstranenie) cez ElevenLabs.
/// </summary>
[TestClass]
public sealed class VoiceToolsTests
{
    private const int RATE = SoundProcessing.BankSampleRate;
    private string _root = null!;

    [TestInitialize]
    public void Init() => _root = Directory.CreateTempSubdirectory("rbevoices").FullName;

    [TestCleanup]
    public void Cleanup() => Directory.Delete(_root, true);

    /// <summary>
    /// Zachytena poziadavka - cesta, hlavicka kluca a obsah (JSON alebo casti multipart).
    /// </summary>
    private sealed record Captured(HttpMethod Method, Uri Uri, bool HasKey, string Json, Dictionary<string, List<string>> Parts);

    private sealed class Handler(Func<HttpRequestMessage, HttpResponseMessage> respond) : HttpMessageHandler
    {
        public List<Captured> Requests { get; } = [];

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var parts = new Dictionary<string, List<string>>();
            var json = "";
            if (request.Content is MultipartFormDataContent form)
            {
                foreach (var part in form)
                {
                    var name = part.Headers.ContentDisposition!.Name!.Trim('"');
                    var value = part is ByteArrayContent and not StringContent
                        ? $"bytes:{(await part.ReadAsByteArrayAsync(cancellationToken)).Length}:{part.Headers.ContentType}"
                        : await part.ReadAsStringAsync(cancellationToken);
                    if (!parts.TryGetValue(name, out var list))
                        parts[name] = list = [];
                    list.Add(value);
                }
            }
            else if (request.Content is not null)
            {
                json = await request.Content.ReadAsStringAsync(cancellationToken);
            }

            Requests.Add(new Captured(request.Method, request.RequestUri!, request.Headers.Contains("xi-api-key"), json, parts));
            return respond(request);
        }
    }

    private static HttpResponseMessage Ok(string json) => new(HttpStatusCode.OK) { Content = new StringContent(json) };

    private static HttpResponseMessage Bytes(byte[] data) => new(HttpStatusCode.OK) { Content = new ByteArrayContent(data) };

    private static byte[] Wav(float amplitude, int ms = 100) =>
        SoundProcessing.ToWav(new PcmSound(Enumerable.Repeat(amplitude, RATE * ms / 1000).ToArray(), RATE));

    [TestMethod]
    public async Task ConvertVoice_MultipartSNastaveniami()
    {
        var handler = new Handler(_ => Bytes([0, 64, 0, 64]));
        using var client = new ElevenLabsClient(handler, "k");

        var sound = await client.ConvertVoiceAsync(new VoiceConversionRequest(Wav(0.1f), "hlas", ElevenLabsClient.StsModel) { Seed = 3, Stability = 0.6 },
            CancellationToken.None);

        var request = handler.Requests.Single();
        Assert.AreEqual("/v1/speech-to-speech/hlas", request.Uri.AbsolutePath);
        Assert.AreEqual("?output_format=pcm_22050", request.Uri.Query);
        StringAssert.StartsWith(request.Parts["audio"].Single(), "bytes:");
        StringAssert.EndsWith(request.Parts["audio"].Single(), "audio/wav");
        Assert.AreEqual(ElevenLabsClient.StsModel, request.Parts["model_id"].Single());
        Assert.AreEqual("3", request.Parts["seed"].Single());
        Assert.AreEqual(0.6, (double)JsonNode.Parse(request.Parts["voice_settings"].Single())!["stability"]!);
        Assert.HasCount(2, sound.Samples);
    }

    [TestMethod]
    public async Task Isolate_DekodujeWavZOdpovede()
    {
        var handler = new Handler(_ => Bytes(Wav(0.25f)));
        using var client = new ElevenLabsClient(handler, "k");

        var sound = await client.IsolateAsync(Wav(0.1f), CancellationToken.None);

        Assert.AreEqual("/v1/audio-isolation", handler.Requests.Single().Uri.AbsolutePath);
        Assert.AreEqual(0.25f, sound.Samples[10], 0.001);
    }

    [TestMethod]
    public void ReadAudio_NeznamyFormat_Chyba() =>
        Assert.ThrowsExactly<InvalidDataException>(() => SoundProcessing.ReadAudio([1, 2, 3, 4, 5, 6, 7, 8]));

    [TestMethod]
    public async Task SearchShared_FilterAVysledky()
    {
        var handler = new Handler(_ => Ok("""
            {"voices":[{"public_owner_id":"o1","voice_id":"v1","name":"Zuzka","gender":"female","age":"middle_aged","accent":"slovak","language":"sk","description":"calm","preview_url":"https://x/p.mp3"},
                       {"voice_id":"bez autora"}],"has_more":true}
            """));
        using var client = new ElevenLabsClient(handler, "k");

        var page = await client.SearchSharedAsync(new SharedVoiceQuery("sk", "female", " hlásateľ ", 2), CancellationToken.None);

        var query = handler.Requests.Single().Uri.Query;
        StringAssert.Contains(query, "page=2");
        StringAssert.Contains(query, "language=sk");
        StringAssert.Contains(query, "gender=female");
        StringAssert.Contains(query, "search=hl%C3%A1sate%C4%BE");
        Assert.IsTrue(page.HasMore);
        Assert.AreEqual("middle aged", page.Voices.Single().Age);
    }

    [TestMethod]
    public async Task AddShared_CestaSAutoromANazvom()
    {
        var handler = new Handler(_ => Ok("""{"voice_id":"nove"}"""));
        using var client = new ElevenLabsClient(handler, "k");

        var id = await client.AddSharedAsync(new SharedVoice("o1", "v1", "Zuzka", "", "", "", "", "", ""), "Zuzka", CancellationToken.None);

        Assert.AreEqual("nove", id);
        Assert.AreEqual("/v1/voices/add/o1/v1", handler.Requests.Single().Uri.AbsolutePath);
        Assert.AreEqual("Zuzka", (string)JsonNode.Parse(handler.Requests.Single().Json)!["new_name"]!);
    }

    [TestMethod]
    public async Task Design_BezTextu_VymysliSluzbaAUkazkyVPcm()
    {
        var pcm = Convert.ToBase64String(new byte[] { 0, 64, 0, 64 });
        var handler = new Handler(r => r.RequestUri!.AbsolutePath.EndsWith("/design", StringComparison.Ordinal)
            ? Ok("""{"previews":[{"generated_voice_id":"g1","audio_base_64":"PCM","media_type":"audio/pcm"}]}""".Replace("PCM", pcm, StringComparison.Ordinal))
            : Ok("""{"voice_id":"hlas"}"""));
        using var client = new ElevenLabsClient(handler, "k");

        var previews = await client.DesignAsync("pokojný mužský hlas", "eleven_ttv_v3", "", CancellationToken.None);
        var id = await client.CreateDesignedAsync("Hlásateľ", "pokojný mužský hlas", previews[0].GeneratedVoiceId, CancellationToken.None);

        var design = JsonNode.Parse(handler.Requests[0].Json)!.AsObject();
        Assert.IsTrue((bool)design["auto_generate_text"]!);
        Assert.IsFalse(design.ContainsKey("text"));
        Assert.AreEqual("eleven_ttv_v3", (string)design["model_id"]!);
        Assert.AreEqual(0.5f, previews.Single().Sound.Samples[0]);
        Assert.AreEqual("hlas", id);
        Assert.AreEqual("/v1/text-to-voice", handler.Requests[1].Uri.AbsolutePath);
        Assert.AreEqual("g1", (string)JsonNode.Parse(handler.Requests[1].Json)!["generated_voice_id"]!);
    }

    [TestMethod]
    public async Task Clone_VzorkyNazovAJazyk()
    {
        var handler = new Handler(_ => Ok("""{"voice_id":"klon","requires_verification":false}"""));
        using var client = new ElevenLabsClient(handler, "k");

        var id = await client.CloneAsync(new VoiceCloneRequest("INISS SK", "", [Wav(0.1f), Wav(0.2f)], true, "sk"), CancellationToken.None);

        var parts = handler.Requests.Single().Parts;
        Assert.AreEqual("klon", id);
        Assert.HasCount(2, parts["files"]);
        Assert.AreEqual("INISS SK", parts["name"].Single());
        Assert.AreEqual("true", parts["remove_background_noise"].Single());
        Assert.AreEqual("sk", (string)JsonNode.Parse(parts["labels"].Single())!["language"]!);
    }

    [TestMethod]
    public async Task DeleteVoice_Delete()
    {
        var handler = new Handler(_ => Ok("{}"));
        using var client = new ElevenLabsClient(handler, "k");

        await client.DeleteVoiceAsync("v 1", CancellationToken.None);

        Assert.AreEqual(HttpMethod.Delete, handler.Requests.Single().Method);
        Assert.AreEqual("/v1/voices/v%201", handler.Requests.Single().Uri.AbsolutePath);
    }

    [TestMethod]
    public async Task Ukazka_BezKlucaALenHttps()
    {
        var handler = new Handler(_ => Bytes(Wav(0.1f)));
        using var client = new ElevenLabsClient(handler, "tajny");

        await client.DownloadPreviewAsync("https://storage.example.com/p.wav", CancellationToken.None);
        await Assert.ThrowsExactlyAsync<SpeechException>(() => client.DownloadPreviewAsync("http://storage.example.com/p.wav", CancellationToken.None));

        Assert.IsFalse(handler.Requests.Single().HasKey, "kluc API nesmie odist mimo API");
    }

    /// <summary>
    /// Falosna sluzba pre generator - zmena hlasu vrati ton, vycistenie ticho s tonom.
    /// </summary>
    private sealed class FakeProvider : ISpeechProvider
    {
        public List<string> Calls { get; } = [];

        public string Id => "fake";

        public SpeechFeatures Features => SpeechFeatures.ConvertVoice | SpeechFeatures.Isolate;

        public string TranscriptionModel => "fake-stt";

        public Task<IReadOnlyList<SpeechVoice>> GetVoicesAsync(CancellationToken cancel) => Task.FromResult<IReadOnlyList<SpeechVoice>>([]);

        public Task<IReadOnlyList<SpeechModel>> GetModelsAsync(CancellationToken cancel) => Task.FromResult<IReadOnlyList<SpeechModel>>([]);

        public Task<SpeechQuota> GetQuotaAsync(CancellationToken cancel) => Task.FromResult(new SpeechQuota("", 0, 0));

        public Task<PcmSound> SynthesizeAsync(SpeechRequest request, CancellationToken cancel) => throw new NotSupportedException();

        public Task<TimedSpeech> SynthesizeTimedAsync(SpeechRequest request, CancellationToken cancel) => throw new NotSupportedException();

        public Task<PcmSound> ConvertVoiceAsync(VoiceConversionRequest request, CancellationToken cancel)
        {
            Calls.Add("sts:" + request.VoiceId);
            return Task.FromResult(new PcmSound(Enumerable.Repeat(0.3f, RATE / 2).ToArray(), RATE));
        }

        public Task<PcmSound> IsolateAsync(byte[] sourceWav, CancellationToken cancel)
        {
            Calls.Add("isolate");
            return Task.FromResult(new PcmSound(Enumerable.Repeat(0.2f, RATE / 4).ToArray(), RATE));
        }

        public Task<string> TranscribeAsync(byte[] sourceWav, string languageCode, CancellationToken cancel) => Task.FromResult("");

        public bool SupportsContext(string modelId) => true;
    }

    [TestMethod]
    public async Task ProcessAudio_ZmenaHlasuAVycistenie_ZCachePodlaObsahu()
    {
        var provider = new FakeProvider();
        var cache = new SpeechCache(Path.Combine(_root, "cache"));
        var file = Path.Combine(_root, "A.EWA");
        SoundProcessing.WriteSoundFile(file, Wav(0.1f));
        var generator = new SpeechGenerator(provider, cache, new VoiceProfile { VoiceId = "zuzana" }, null);

        var converted = await generator.ProcessAudioAsync(SpeechOperation.ChangeVoice, file, null, CancellationToken.None);
        var again = await generator.ProcessAudioAsync(SpeechOperation.ChangeVoice, file, null, CancellationToken.None);
        var reseeded = await generator.ProcessAudioAsync(SpeechOperation.ChangeVoice, file, 9, CancellationToken.None);
        var isolated = await generator.ProcessAudioAsync(SpeechOperation.Isolate, file, null, CancellationToken.None);
        SoundProcessing.WriteSoundFile(file, Wav(0.2f));
        await generator.ProcessAudioAsync(SpeechOperation.Isolate, file, null, CancellationToken.None);

        Assert.AreEqual(500, converted.Sound.DurationMs);
        Assert.IsTrue(again.FromCache);
        Assert.IsFalse(reseeded.FromCache);
        Assert.AreEqual(250, isolated.Sound.DurationMs, 1);
        CollectionAssert.AreEqual(new[] { "sts:zuzana", "sts:zuzana", "isolate", "isolate" }, provider.Calls, "iny obsah nahravky = nova poziadavka");
        Assert.AreEqual(generator.ConversionFingerprint(),
            new SpeechGenerator(provider, cache, new VoiceProfile { VoiceId = "zuzana", Seed = 5 }, null).ConversionFingerprint());
    }

    [TestMethod]
    public void CloneSamples_SpojiUryvkyPoMinute()
    {
        var sounds = Enumerable.Range(0, 150).Select(_ => new PcmSound(new float[RATE], RATE));

        var samples = FVoiceClone.Samples(sounds);

        // 1 s reci + 0,3 s pauza = 46 uryvkov na minutu
        Assert.HasCount(4, samples);
        Assert.IsTrue(samples.All(s => SoundProcessing.ReadWav(s).DurationMs <= 60_000));
    }

    [TestMethod]
    public void CloneSamples_NajviacDvadsatPat()
    {
        var sounds = Enumerable.Range(0, 2000).Select(_ => new PcmSound(new float[RATE], RATE));

        Assert.HasCount(25, FVoiceClone.Samples(sounds));
    }

    [TestMethod]
    public void Origin_ZmenaHlasu()
    {
        var origin = SoundOrigin.Converted(new VoiceProfile { VoiceId = "v", VoiceName = "Zuzana" }, 4, "f", DateTimeOffset.Now);

        Assert.AreEqual(SoundOrigin.KindSts, origin.Kind);
        Assert.AreEqual(ElevenLabsClient.StsModel, origin.ModelId);
        Assert.IsTrue(origin.SameAs("f"));
    }
}
