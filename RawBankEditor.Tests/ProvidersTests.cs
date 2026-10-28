using System.Net;
using System.Security.Cryptography;
using System.Xml.Linq;
using RawBankEditor.Speech;
using RawBankEditor.Tools;

namespace RawBankEditor.Tests;

/// <summary>
/// Dalsie sluzby syntezy (Azure Speech, Piper), stahovanie hlasov Piper, vyber sluzby a odhad strihu nosnej vety
/// pri sluzbe bez casov znakov.
/// </summary>
[TestClass]
public sealed class ProvidersTests
{
    private const int RATE = SoundProcessing.BankSampleRate;
    private string _root = null!;

    [TestInitialize]
    public void Init() => _root = Directory.CreateTempSubdirectory("rbeproviders").FullName;

    [TestCleanup]
    public void Cleanup() => Directory.Delete(_root, true);

    private static byte[] Wav(int rate, int ms, float value = 0.2f) => SoundProcessing.ToWav(new PcmSound(Enumerable.Repeat(value, rate * ms / 1000).ToArray(), rate));

    [TestMethod]
    [DataRow("westeurope", "https://westeurope.tts.speech.microsoft.com/cognitiveservices/v1",
        "https://westeurope.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1")]
    [DataRow(" WestEurope ", "https://westeurope.tts.speech.microsoft.com/cognitiveservices/v1",
        "https://westeurope.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1")]
    [DataRow("https://moja-rec.cognitiveservices.azure.com/", "https://moja-rec.cognitiveservices.azure.com/tts/cognitiveservices/v1",
        "https://moja-rec.cognitiveservices.azure.com/stt/speech/recognition/conversation/cognitiveservices/v1")]
    [DataRow("moja-rec.cognitiveservices.azure.com", "https://moja-rec.cognitiveservices.azure.com/tts/cognitiveservices/v1",
        "https://moja-rec.cognitiveservices.azure.com/stt/speech/recognition/conversation/cognitiveservices/v1")]
    public void Azure_AdresyPodlaOblastiAleboProstriedku(string location, string tts, string stt)
    {
        var (ttsUri, _, sttUri) = AzureSpeechClient.Endpoints(location);

        Assert.AreEqual(tts, ttsUri.ToString());
        Assert.AreEqual(stt, sttUri.ToString());
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("west europe")]
    [DataRow("west/europe")]
    public void Azure_NeplatnaOblast(string location)
    {
        Assert.ThrowsExactly<ArgumentException>(() => AzureSpeechClient.Endpoints(location));
        Assert.IsFalse(SpeechProviders.IsValidAzureRegion(location));
    }

    [TestMethod]
    public void Azure_Ssml_HlasRychlostAZnaky()
    {
        var ssml = XElement.Parse(AzureSpeechClient.SsmlOf(new SpeechRequest("Vlak <R 601> & Ex", "sk-SK-ViktoriaNeural", AzureSpeechClient.NeuralModel)
        {
            LanguageCode = "sk",
            Speed = 1.1
        }));
        XNamespace ns = "http://www.w3.org/2001/10/synthesis";

        Assert.AreEqual("sk-SK", (string?)ssml.Attribute(XNamespace.Xml + "lang"));
        var voice = ssml.Element(ns + "voice")!;
        Assert.AreEqual("sk-SK-ViktoriaNeural", (string?)voice.Attribute("name"));
        var prosody = voice.Element(ns + "prosody")!;
        Assert.AreEqual("1.10", (string?)prosody.Attribute("rate"));
        Assert.AreEqual("Vlak <R 601> & Ex", prosody.Value);
        Assert.IsNull(voice.Element(ns + "lang"), "hlas jazyka banky netreba prepinat");
    }

    [TestMethod]
    public void Azure_Ssml_ViacjazycnyHlasHovoriJazykomBanky()
    {
        var ssml = XElement.Parse(AzureSpeechClient.SsmlOf(new SpeechRequest("Dobrý deň", "en-US-AvaMultilingualNeural", AzureSpeechClient.NeuralModel)
        {
            LanguageCode = "sk"
        }));
        XNamespace ns = "http://www.w3.org/2001/10/synthesis";

        var lang = ssml.Element(ns + "voice")!.Element(ns + "lang")!;
        Assert.AreEqual("sk-SK", (string?)lang.Attribute(XNamespace.Xml + "lang"));
        Assert.AreEqual("Dobrý deň", lang.Value);
        Assert.IsNull(ssml.Element(ns + "voice")!.Element(ns + "prosody"), "normalna rychlost bez znacky");
    }

    [TestMethod]
    public async Task Azure_Synteza_HlavickyAVystup()
    {
        HttpRequestMessage? sent = null;
        string? body = null;
        using var client = new AzureSpeechClient(new Handler(async request =>
        {
            sent = request;
            body = await request.Content!.ReadAsStringAsync();
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(Wav(24000, 200)) };
        }), "kluc", "westeurope");

        var sound = await client.SynthesizeAsync(new SpeechRequest("Lipová", "sk-SK-LukasNeural", AzureSpeechClient.NeuralModel), CancellationToken.None);

        Assert.AreEqual(24000, sound.SampleRate);
        Assert.AreEqual(200, sound.DurationMs);
        Assert.AreEqual("https://westeurope.tts.speech.microsoft.com/cognitiveservices/v1", sent!.RequestUri!.ToString());
        Assert.AreEqual("riff-24khz-16bit-mono-pcm", sent.Headers.GetValues("X-Microsoft-OutputFormat").Single());
        Assert.AreEqual("kluc", sent.Headers.GetValues("Ocp-Apim-Subscription-Key").Single());
        Assert.AreEqual("application/ssml+xml", sent.Content!.Headers.GetValues("Content-Type").Single());
        StringAssert.Contains(body, "Lipová");
    }

    [TestMethod]
    public async Task Azure_Prepis_16kHzAJazykSKrajinou()
    {
        string? contentType = null;
        int? rate = null;
        Uri? uri = null;
        var status = "Success";
        using var client = new AzureSpeechClient(new Handler(async request =>
        {
            uri = request.RequestUri;
            contentType = request.Content!.Headers.GetValues("Content-Type").Single();
            rate = SoundProcessing.ReadWav(await request.Content.ReadAsByteArrayAsync()).SampleRate;
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent($$"""{"RecognitionStatus":"{{status}}","DisplayText":" Dolné Mesto. ","Offset":0,"Duration":100}""")
            };
        }), "kluc", "westeurope");

        var text = await client.TranscribeAsync(Wav(RATE, 500), "sk", CancellationToken.None);

        Assert.AreEqual("Dolné Mesto.", text);
        Assert.AreEqual(16000, rate);
        Assert.AreEqual("audio/wav; codecs=audio/pcm; samplerate=16000", contentType);
        StringAssert.Contains(uri!.Query, "language=sk-SK");

        status = "NoMatch";
        Assert.AreEqual("", await client.TranscribeAsync(Wav(RATE, 500), "sk", CancellationToken.None), "nahravka bez reci");
    }

    [TestMethod]
    public async Task Azure_Hlasy_JazykyAVynechanieZastaranych()
    {
        const string json = """
            [
              {"ShortName":"sk-SK-ViktoriaNeural","DisplayName":"Viktoria","Gender":"Female","Locale":"sk-SK","LocaleName":"Slovak (Slovakia)","VoiceType":"Neural","Status":"GA"},
              {"ShortName":"en-US-AvaMultilingualNeural","DisplayName":"Ava Multilingual","Gender":"Female","Locale":"en-US","SecondaryLocaleList":["cs-CZ","de-DE"],"Status":"GA"},
              {"ShortName":"cs-CZ-OldNeural","DisplayName":"Old","Locale":"cs-CZ","Status":"Deprecated"}
            ]
            """;
        using var client = new AzureSpeechClient(new Handler(_ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(json) })),
            "kluc", "westeurope");

        var voices = await client.GetVoicesAsync(CancellationToken.None);

        Assert.HasCount(2, voices);
        var ava = voices.Single(v => v.Id == "en-US-AvaMultilingualNeural");
        Assert.IsTrue(ava.Speaks("cs"));
        Assert.IsFalse(ava.Speaks("sk"));
        Assert.IsTrue(voices.Single(v => v.Name == "Viktoria").Speaks("sk"));
    }

    [TestMethod]
    public async Task Azure_Chyba_SpravaSluzbyAleboStav()
    {
        using var client = new AzureSpeechClient(new Handler(_ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Unauthorized)
        {
            Content = new StringContent("""{"error":{"code":"401","message":"Access denied due to invalid subscription key."}}""")
        })), "kluc", "westeurope");

        var e = await Assert.ThrowsExactlyAsync<SpeechException>(() => client.GetVoicesAsync(CancellationToken.None));

        Assert.AreEqual(401, e.StatusCode);
        StringAssert.Contains(e.Message, "invalid subscription key");
    }

    [TestMethod]
    [DataRow("sk_SK-lili-medium", "sk_SK-lili-medium", null)]
    [DataRow("en_US-libritts-high#12", "en_US-libritts-high", 12)]
    [DataRow("divny#nazov", "divny#nazov", null)]
    public void Piper_HlasAHovoriaci(string voiceId, string key, int? speaker) =>
        Assert.AreEqual((key, speaker), PiperClient.SplitVoice(voiceId));

    [TestMethod]
    [DataRow("sk_SK-lili-medium", true)]
    [DataRow("..", false)]
    [DataRow("..\\evil", false)]
    [DataRow("a/b", false)]
    [DataRow(".skryty", false)]
    [DataRow("", false)]
    public void Piper_PlatnyNazovHlasu(string key, bool valid) => Assert.AreEqual(valid, PiperVoices.IsValidKey(key));

    private void Install(string key, string config)
    {
        File.WriteAllBytes(Path.Combine(_root, key + ".onnx"), [1, 2, 3]);
        File.WriteAllText(Path.Combine(_root, key + ".onnx.json"), config);
    }

    [TestMethod]
    public async Task Piper_Hlasy_ModelySNastavenimAHovoriaci()
    {
        Install("sk_SK-lili-medium", """{"audio":{"quality":"medium","sample_rate":22050},"language":{"code":"sk_SK","family":"sk","name_native":"Slovenčina"},"num_speakers":1,"speaker_id_map":{}}""");
        Install("en_US-pair-low", """{"audio":{"quality":"low"},"language":{"code":"en_US"},"num_speakers":2,"speaker_id_map":{"anna":1,"bob":0}}""");
        // model bez nastavenia sa neda pouzit
        File.WriteAllBytes(Path.Combine(_root, "cs_CZ-bez-json.onnx"), [1]);

        var voices = await new PiperClient("piper.exe", _root).GetVoicesAsync(CancellationToken.None);

        CollectionAssert.AreEqual(new[] { "en_US-pair-low#0", "en_US-pair-low#1", "sk_SK-lili-medium" }, voices.Select(v => v.Id).ToArray());
        Assert.IsTrue(voices[2].Speaks("sk"));
        Assert.IsTrue(voices[0].Speaks("en"), "jazyk z kodu, ked chyba family");
        StringAssert.Contains(voices[1].Name, "anna");
    }

    [TestMethod]
    public async Task Piper_Synteza_ParametreAVystup()
    {
        Install("en_US-pair-low", """{"language":{"family":"en"},"speaker_id_map":{"anna":1}}""");
        PiperRun? run = null;
        var client = new PiperClient(@"C:\piper\piper.exe", _root)
        {
            Run = (r, _) =>
            {
                run = r;
                var output = r.Arguments[r.Arguments.ToList().IndexOf("--output_file") + 1];
                File.WriteAllBytes(output, Wav(16000, 300));
                return Task.FromResult(new PiperResult(0, ""));
            }
        };

        var sound = await client.SynthesizeAsync(new SpeechRequest("Prvý riadok\ndruhý", "en_US-pair-low#1", PiperClient.Model) { Speed = 1.25 },
            CancellationToken.None);

        Assert.AreEqual(16000, sound.SampleRate);
        Assert.AreEqual(300, sound.DurationMs);
        var arguments = run!.Arguments.ToList();
        Assert.AreEqual(Path.Combine(_root, "en_US-pair-low.onnx"), arguments[arguments.IndexOf("--model") + 1]);
        Assert.AreEqual("0.8", arguments[arguments.IndexOf("--length_scale") + 1]);
        Assert.AreEqual("1", arguments[arguments.IndexOf("--speaker") + 1]);
        Assert.AreEqual("Prvý riadok druhý", run.Input, "text na jeden riadok");
        Assert.IsFalse(File.Exists(arguments[arguments.IndexOf("--output_file") + 1]), "docasny subor sa zmaze");
    }

    [TestMethod]
    public async Task Piper_Chyba_ChybovyVystupAleboChybajuciHlas()
    {
        Install("sk_SK-lili-medium", """{"language":{"family":"sk"}}""");
        var client = new PiperClient("piper.exe", _root) { Run = (_, _) => Task.FromResult(new PiperResult(1, "Unable to load model")) };

        var e = await Assert.ThrowsExactlyAsync<SpeechException>(() => client.SynthesizeAsync(new SpeechRequest("A", "sk_SK-lili-medium", PiperClient.Model), CancellationToken.None));
        Assert.AreEqual("Unable to load model", e.Message);

        await Assert.ThrowsExactlyAsync<SpeechException>(() => client.SynthesizeAsync(new SpeechRequest("A", "cs_CZ-jirka-medium", PiperClient.Model), CancellationToken.None));
        await Assert.ThrowsExactlyAsync<SpeechException>(() => client.SynthesizeAsync(new SpeechRequest("A", @"..\sk_SK-lili-medium", PiperClient.Model), CancellationToken.None));
    }

    private const string Catalog = """
        {
          "sk_SK-lili-medium": {
            "key": "sk_SK-lili-medium", "name": "lili", "quality": "medium", "num_speakers": 1,
            "language": {"code": "sk_SK", "family": "sk", "name_native": "Slovenčina", "name_english": "Slovak"},
            "files": {
              "sk/sk_SK/lili/medium/sk_SK-lili-medium.onnx": {"size_bytes": 5, "md5_digest": "MD5ONNX"},
              "sk/sk_SK/lili/medium/sk_SK-lili-medium.onnx.json": {"size_bytes": 2, "md5_digest": "MD5JSON"},
              "sk/sk_SK/lili/medium/MODEL_CARD": {"size_bytes": 10, "md5_digest": "x"}
            }
          },
          "cs_CZ-evil-medium": {
            "language": {"family": "cs"},
            "files": {
              "../../cs_CZ-evil-medium.onnx": {"size_bytes": 5, "md5_digest": "x"},
              "cs/cs_CZ-evil-medium.onnx.json": {"size_bytes": 2, "md5_digest": "x"}
            }
          },
          "a/b": {"files": {"a/b.onnx": {"size_bytes": 1}, "a/b.onnx.json": {"size_bytes": 1}}}
        }
        """;

    private static readonly byte[] Model = [1, 2, 3, 4, 5];
    private static readonly byte[] Config = "{}"u8.ToArray();

    private static string CatalogWith(string onnxMd5) =>
        Catalog.Replace("MD5ONNX", onnxMd5, StringComparison.Ordinal).Replace("MD5JSON", Md5(Config), StringComparison.Ordinal);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5351", Justification = "Test suctu uloziska hlasov.")]
    private static string Md5(byte[] data) => Convert.ToHexStringLower(MD5.HashData(data));

    [TestMethod]
    public void Katalog_LenModelANastavenieSPlatnouCestou()
    {
        var voices = PiperVoiceCatalog.Parse(CatalogWith(Md5(Model)));

        var voice = voices.Single();
        Assert.AreEqual("sk_SK-lili-medium", voice.Key);
        Assert.AreEqual("sk", voice.Family);
        Assert.AreEqual("Slovenčina", voice.LanguageName);
        Assert.AreEqual(7, voice.Size);
        CollectionAssert.AreEquivalent(new[] { "sk_SK-lili-medium.onnx", "sk_SK-lili-medium.onnx.json" }, voice.Files.Select(f => f.FileName).ToArray());
    }

    private static PiperVoiceCatalog Server(List<string> paths) => new(new Handler(request =>
    {
        var path = request.RequestUri!.AbsolutePath;
        paths.Add(path);
        var content = path.EndsWith(".json", StringComparison.Ordinal) ? Config : Model;
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(content) });
    }));

    [TestMethod]
    public async Task Katalog_Stiahnutie_OverenySucet()
    {
        var paths = new List<string>();
        using var catalog = Server(paths);
        var voice = PiperVoiceCatalog.Parse(CatalogWith(Md5(Model))).Single();
        var folder = Path.Combine(_root, "hlasy");
        long last = 0;

        await catalog.DownloadAsync(voice, folder, new SyncProgress(b => last = b), CancellationToken.None);

        CollectionAssert.AreEqual(Model, File.ReadAllBytes(Path.Combine(folder, "sk_SK-lili-medium.onnx")));
        Assert.IsTrue(File.Exists(Path.Combine(folder, "sk_SK-lili-medium.onnx.json")));
        Assert.AreEqual("/rhasspy/piper-voices/resolve/main/sk/sk_SK/lili/medium/sk_SK-lili-medium.onnx", paths[0], "najprv model, nastavenie az za nim");
        Assert.AreEqual(7, last);
        Assert.HasCount(1, PiperVoices.Installed(folder));
    }

    [TestMethod]
    public async Task Katalog_Stiahnutie_ZlySucetNicNenecha()
    {
        using var catalog = Server([]);
        var voice = PiperVoiceCatalog.Parse(CatalogWith("00000000000000000000000000000000")).Single();
        var folder = Path.Combine(_root, "hlasy");

        await Assert.ThrowsExactlyAsync<SpeechException>(() => catalog.DownloadAsync(voice, folder, null, CancellationToken.None));

        Assert.IsEmpty(Directory.GetFiles(folder));
    }

    [TestMethod]
    public void Sluzby_VyberPodlaHlasuAFunkcie()
    {
        var elevenLabs = new FeatureProvider(ElevenLabsClient.ProviderId,
            SpeechFeatures.Timings | SpeechFeatures.ConvertVoice | SpeechFeatures.Isolate | SpeechFeatures.Transcribe);
        var azure = new FeatureProvider(AzureSpeechClient.ProviderId, SpeechFeatures.Transcribe);
        var piper = new FeatureProvider(PiperClient.ProviderId, SpeechFeatures.Offline);
        using var all = new SpeechProviders([piper, azure, elevenLabs]);
        var azureVoice = new VoiceProfile { Provider = AzureSpeechClient.ProviderId };
        var piperVoice = new VoiceProfile { Provider = PiperClient.ProviderId };

        CollectionAssert.AreEqual(new[] { "elevenlabs", "azure", "piper" }, all.Ids.ToArray());
        Assert.AreSame(azure, all.For(SpeechOperation.Tts, azureVoice));
        Assert.IsNull(all.For(SpeechOperation.ChangeVoice, azureVoice), "Azure hlas nemeni");
        Assert.AreSame(elevenLabs, all.For(SpeechOperation.ChangeVoice, new VoiceProfile()));
        Assert.AreSame(elevenLabs, all.For(SpeechOperation.Isolate, piperVoice));
        Assert.AreSame(azure, all.Transcriber(AzureSpeechClient.ProviderId), "predost sluzba hlasu");
        Assert.AreSame(elevenLabs, all.Transcriber(PiperClient.ProviderId));

        using var withoutElevenLabs = new SpeechProviders([piper, azure]);
        Assert.AreSame(azure, withoutElevenLabs.Transcriber(PiperClient.ProviderId));
        Assert.IsNull(withoutElevenLabs.For(SpeechOperation.Isolate, piperVoice));
        Assert.IsNull(withoutElevenLabs.Get(ElevenLabsClient.ProviderId));
    }

    [TestMethod]
    public void Sluzby_ZNastaveni()
    {
        var piper = Path.Combine(_root, "piper.exe");
        File.WriteAllBytes(piper, []);

        using (var none = SpeechProviders.FromSettings(new SpeechSettings()))
            Assert.IsFalse(none.Any);

        var settings = new SpeechSettings { PiperPath = piper, AzureKey = "kluc", AzureRegion = "west europe" };
        using (var configured = SpeechProviders.FromSettings(settings))
            CollectionAssert.AreEqual(new[] { PiperClient.ProviderId }, configured.Ids.ToArray(), "Azure bez platnej oblasti nie je nastavene");

        settings.AzureRegion = "westeurope";
        using (var configured = SpeechProviders.FromSettings(settings))
        {
            CollectionAssert.AreEqual(new[] { AzureSpeechClient.ProviderId, PiperClient.ProviderId }, configured.Ids.ToArray());
            Assert.IsInstanceOfType<AzureSpeechClient>(configured.Get(AzureSpeechClient.ProviderId));
        }
    }

    /// <summary>
    /// Falosna sluzba bez casov znakov - slova su konstantne tony oddelene 60 ms ticha, posledne slovo vypovede je
    /// o 60 ms dlhsie (predlzena posledna slabika); pred a za recou 100 ms ticha.
    /// </summary>
    private sealed class UntimedProvider : ISpeechProvider
    {
        private static readonly Dictionary<string, (int Ms, float Level)> WordsOf = new()
        {
            ["Pred"] = (500, 0.2f),
            ["Stred"] = (400, 0.5f),
            ["Za"] = (300, 0.3f)
        };

        public List<string> Requests { get; } = [];

        public string Id => "untimed";

        public SpeechFeatures Features => SpeechFeatures.None;

        public string TranscriptionModel => "";

        public Task<PcmSound> SynthesizeAsync(SpeechRequest request, CancellationToken cancel)
        {
            Requests.Add(request.Text);
            var samples = new List<float>(new float[RATE / 10]);
            var words = request.Text.Split(' ');
            for (var i = 0; i < words.Length; i++)
            {
                var (ms, level) = WordsOf.GetValueOrDefault(words[i].Trim(',', '.'), (300, 0.5f));
                if (i == words.Length - 1)
                    ms += 60;
                samples.AddRange(Enumerable.Repeat(level, RATE * ms / 1000));
                samples.AddRange(new float[RATE * (i == words.Length - 1 ? 100 : 60) / 1000]);
            }

            return Task.FromResult(new PcmSound(samples.ToArray(), RATE));
        }

        public Task<IReadOnlyList<SpeechVoice>> GetVoicesAsync(CancellationToken cancel) => Task.FromResult<IReadOnlyList<SpeechVoice>>([]);

        public Task<IReadOnlyList<SpeechModel>> GetModelsAsync(CancellationToken cancel) => Task.FromResult<IReadOnlyList<SpeechModel>>([]);

        public Task<SpeechQuota> GetQuotaAsync(CancellationToken cancel) => throw new NotSupportedException();

        public Task<TimedSpeech> SynthesizeTimedAsync(SpeechRequest request, CancellationToken cancel) => throw new NotSupportedException();

        public Task<PcmSound> ConvertVoiceAsync(VoiceConversionRequest request, CancellationToken cancel) => throw new NotSupportedException();

        public Task<PcmSound> IsolateAsync(byte[] sourceWav, CancellationToken cancel) => throw new NotSupportedException();

        public Task<string> TranscribeAsync(byte[] sourceWav, string languageCode, CancellationToken cancel) => throw new NotSupportedException();

        public bool SupportsContext(string modelId) => false;
    }

    [TestMethod]
    public async Task OdhadStrihu_UryvokBezSusednychSlov()
    {
        var provider = new UntimedProvider();
        var generator = new SpeechGenerator(provider, new SpeechCache(Path.Combine(_root, "cache")), new VoiceProfile { Provider = "untimed", VoiceId = "v" }, null);
        var prosody = new GroupProsody { Technique = ProsodyTechnique.Context, Before = "Pred", After = "Za" };

        var prepared = generator.Prepare(new SpeechInput("Stred", prosody), null);
        var result = await generator.GenerateAsync(new SpeechInput("Stred", prosody), null, CancellationToken.None);

        Assert.AreEqual(ProsodyTechnique.Carrier, prepared.Technique, "kontext sluzba nevie - nosna veta");
        Assert.IsTrue(prepared.Estimated);
        var samples = result.Sound.Samples;
        Assert.IsTrue(samples.Count(s => Math.Abs(s - 0.5f) < 0.001f) >= RATE * 2 / 5 - 10, "cele slovo uryvku");
        Assert.IsTrue(samples.Count(s => Math.Abs(s - 0.2f) < 0.001f || Math.Abs(s - 0.3f) < 0.001f) < RATE / 100, "susedne slova nie");
        CollectionAssert.AreEquivalent(new[] { "Pred Stred Za", "Pred", "Za" }, provider.Requests);
        Assert.IsTrue(generator.IsCached(prepared));

        // samostatne texty skupiny su v cache - dalsi uryvok stoji len nosnu vetu
        await generator.GenerateAsync(new SpeechInput("Iny", prosody), null, CancellationToken.None);
        Assert.HasCount(4, provider.Requests);
    }

    [TestMethod]
    public void OdhadStrihu_LenSoSluzbouBezCasov()
    {
        var timed = new FeatureProvider("timed", SpeechFeatures.Timings);
        var generator = new SpeechGenerator(timed, new SpeechCache(Path.Combine(_root, "cache")), new VoiceProfile { VoiceId = "v" }, null);

        var prepared = generator.Prepare(new SpeechInput("Stred", new GroupProsody { Technique = ProsodyTechnique.Carrier, Before = "Pred" }), null);

        Assert.IsFalse(prepared.Estimated);
        Assert.IsNull(prepared.Before);
    }

    private sealed class FeatureProvider(string id, SpeechFeatures features) : ISpeechProvider
    {
        public string Id => id;

        public SpeechFeatures Features => features;

        public string TranscriptionModel => "";

        public Task<IReadOnlyList<SpeechVoice>> GetVoicesAsync(CancellationToken cancel) => Task.FromResult<IReadOnlyList<SpeechVoice>>([]);

        public Task<IReadOnlyList<SpeechModel>> GetModelsAsync(CancellationToken cancel) => Task.FromResult<IReadOnlyList<SpeechModel>>([]);

        public Task<SpeechQuota> GetQuotaAsync(CancellationToken cancel) => throw new NotSupportedException();

        public Task<PcmSound> SynthesizeAsync(SpeechRequest request, CancellationToken cancel) => throw new NotSupportedException();

        public Task<TimedSpeech> SynthesizeTimedAsync(SpeechRequest request, CancellationToken cancel) => throw new NotSupportedException();

        public Task<PcmSound> ConvertVoiceAsync(VoiceConversionRequest request, CancellationToken cancel) => throw new NotSupportedException();

        public Task<PcmSound> IsolateAsync(byte[] sourceWav, CancellationToken cancel) => throw new NotSupportedException();

        public Task<string> TranscribeAsync(byte[] sourceWav, string languageCode, CancellationToken cancel) => throw new NotSupportedException();

        public bool SupportsContext(string modelId) => false;
    }

    private sealed class SyncProgress(Action<long> report) : IProgress<long>
    {
        public void Report(long value) => report(value);
    }

    private sealed class Handler(Func<HttpRequestMessage, Task<HttpResponseMessage>> respond) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => respond(request);
    }
}
