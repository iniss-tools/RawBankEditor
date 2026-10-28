using System.Net;
using System.Text.Json.Nodes;
using RawBankEditor.Speech;
using RawBankEditor.Tools;
using ToolsCore.Iniss.Entities;

namespace RawBankEditor.Tests;

/// <summary>
/// Intonacia uryvkov (predvolene podla typu skupiny, kontext, nosna veta s vystrihnutim), slovnik vyslovnosti
/// a prehratie za sebou.
/// </summary>
[TestClass]
public sealed class ProsodyTests : IDisposable
{
    private const int RATE = SoundProcessing.BankSampleRate;
    private string _root = null!;
    private SpeechCache _cache = null!;

    [TestInitialize]
    public void Init()
    {
        _root = Directory.CreateTempSubdirectory("rbeprosody").FullName;
        _cache = new SpeechCache(_root);
    }

    [TestCleanup]
    public void Cleanup() => Directory.Delete(_root, true);

    public void Dispose()
    {
    }

    /// <summary>
    /// Falosna sluzba - nosna veta su tri tony (slova) oddelene tichom, kazdy znak textu dostane cas podla slova.
    /// </summary>
    private sealed class FakeProvider(bool context) : ISpeechProvider
    {
        public List<SpeechRequest> Requests { get; } = [];

        public string Id => "fake";

        public Task<IReadOnlyList<SpeechVoice>> GetVoicesAsync(CancellationToken cancel) => Task.FromResult<IReadOnlyList<SpeechVoice>>([]);

        public Task<IReadOnlyList<SpeechModel>> GetModelsAsync(CancellationToken cancel) => Task.FromResult<IReadOnlyList<SpeechModel>>([]);

        public Task<SpeechQuota> GetQuotaAsync(CancellationToken cancel) => Task.FromResult(new SpeechQuota("", 0, 0));

        public Task<PcmSound> SynthesizeAsync(SpeechRequest request, CancellationToken cancel)
        {
            Requests.Add(request);
            return Task.FromResult(Words(1).Sound);
        }

        public Task<TimedSpeech> SynthesizeTimedAsync(SpeechRequest request, CancellationToken cancel)
        {
            Requests.Add(request);
            return Task.FromResult(Words(request.Text.Split(' ').Length, request.Text));
        }

        public Task<string> TranscribeAsync(byte[] sourceWav, string languageCode, CancellationToken cancel) => Task.FromResult("");

        public bool SupportsContext(string modelId) => context;

        public Task<PcmSound> ConvertVoiceAsync(VoiceConversionRequest request, CancellationToken cancel) => throw new NotSupportedException();

        public Task<PcmSound> IsolateAsync(byte[] sourceWav, CancellationToken cancel) => throw new NotSupportedException();
    }

    /// <summary>
    /// Slova po 300 ms tonu a 200 ms ticha; casy znakov rovnomerne v ramci slova.
    /// </summary>
    private static TimedSpeech Words(int count, string text = "x")
    {
        var samples = new List<float>();
        var words = text.Split(' ');
        var timings = new List<CharTiming>();
        for (var w = 0; w < count; w++)
        {
            var start = samples.Count / (double)RATE;
            for (var i = 0; i < RATE * 3 / 10; i++)
                samples.Add(0.4f * MathF.Sin(i / 6f) * (w + 1) / count);
            var end = samples.Count / (double)RATE;
            samples.AddRange(new float[RATE / 5]);
            var word = words[Math.Min(w, words.Length - 1)];
            for (var c = 0; c < word.Length; c++)
                timings.Add(new CharTiming(word[c], start + (end - start) * c / word.Length, start + (end - start) * (c + 1) / word.Length));
            if (w < count - 1)
                timings.Add(new CharTiming(' ', end, end + 0.2));
        }

        return new TimedSpeech(new PcmSound(samples.ToArray(), RATE), timings);
    }

    [TestMethod]
    public void Defaults_PodlaTypuSkupiny()
    {
        var comma = ProsodyDefaults.For(FyzGroupType.R1, "sk");
        Assert.AreEqual(ProsodyTechnique.Context, comma.Technique);
        Assert.AreEqual(ProsodyPunctuation.Comma, comma.Punctuation);
        Assert.IsTrue(comma.Before.Length > 0 && comma.After.Length > 0);

        var end = ProsodyDefaults.For(FyzGroupType.R4, "cs");
        Assert.AreEqual(ProsodyPunctuation.Period, end.Punctuation);
        Assert.AreEqual("", end.After);

        Assert.AreEqual("", ProsodyDefaults.For(FyzGroupType.V4, "sk").Before);
        Assert.AreEqual(ProsodyTechnique.None, ProsodyDefaults.For(FyzGroupType.Vety, "sk").Technique);
        Assert.AreEqual("", ProsodyDefaults.For(FyzGroupType.R1, "hu").Before);
    }

    [TestMethod]
    [DataRow("Lipová", ProsodyPunctuation.Comma, "Lipová,")]
    [DataRow("Lipová", ProsodyPunctuation.Period, "Lipová.")]
    [DataRow("Lipová.", ProsodyPunctuation.Comma, "Lipová.")]
    [DataRow(" Lipová ", ProsodyPunctuation.None, "Lipová")]
    public void Spoken_PridaInterpunkciuAkJuTextNema(string text, ProsodyPunctuation punctuation, string expected) =>
        Assert.AreEqual(expected, new GroupProsody { Punctuation = punctuation }.Spoken(text));

    [TestMethod]
    public void Prepare_Kontext_TextPredAZaSaNevyslovi()
    {
        var generator = new SpeechGenerator(new FakeProvider(true), _cache, new VoiceProfile { VoiceId = "v" }, null);
        var prosody = new GroupProsody { Technique = ProsodyTechnique.Context, Before = "Vlak ide", After = "a ďalej.", Punctuation = ProsodyPunctuation.Comma };

        var prepared = generator.Prepare(new SpeechInput("Lipová", prosody), null);

        Assert.AreEqual(ProsodyTechnique.Context, prepared.Technique);
        Assert.AreEqual("Lipová,", prepared.Request.Text);
        Assert.AreEqual("Vlak ide", prepared.Request.PreviousText);
        Assert.AreEqual("a ďalej.", prepared.Request.NextText);
        Assert.AreEqual(7, prepared.Characters);
    }

    [TestMethod]
    public void Prepare_ModelBezKontextu_NosnaVeta()
    {
        var generator = new SpeechGenerator(new FakeProvider(false), _cache, new VoiceProfile { VoiceId = "v", ModelId = "eleven_v3" }, null);
        var prosody = new GroupProsody { Technique = ProsodyTechnique.Context, Before = "Vlak ide", After = "a ďalej." };

        var prepared = generator.Prepare(new SpeechInput("Lipová", prosody), null);

        Assert.AreEqual(ProsodyTechnique.Carrier, prepared.Technique);
        Assert.AreEqual("Vlak ide Lipová a ďalej.", prepared.Request.Text);
        Assert.AreEqual("Lipová", prepared.Request.Text.Substring(prepared.CutStart, prepared.CutLength));
    }

    [TestMethod]
    public void Prepare_OdtlacokBezSeeduAleSIntonaciou()
    {
        var generator = new SpeechGenerator(new FakeProvider(true), _cache, new VoiceProfile { VoiceId = "v" }, null);
        var comma = new GroupProsody { Punctuation = ProsodyPunctuation.Comma };

        var a = generator.Prepare(new SpeechInput("Lipová", comma), 1);

        Assert.AreEqual(a.Fingerprint, generator.Prepare(new SpeechInput("Lipová", comma), 2).Fingerprint);
        Assert.AreNotEqual(a.CacheKey, generator.Prepare(new SpeechInput("Lipová", comma), 2).CacheKey);
        Assert.AreNotEqual(a.Fingerprint, generator.Prepare(new SpeechInput("Lipová", new GroupProsody { Punctuation = ProsodyPunctuation.Period }), 1).Fingerprint);
    }

    [TestMethod]
    public async Task Generate_NosnaVeta_VystrihneStredneSlovoVTichu()
    {
        var provider = new FakeProvider(false);
        var generator = new SpeechGenerator(provider, _cache, new VoiceProfile { VoiceId = "v" }, null);
        var prosody = new GroupProsody { Technique = ProsodyTechnique.Carrier, Before = "aaa", After = "ccc" };

        var result = await generator.GenerateAsync(new SpeechInput("bbb", prosody), null, CancellationToken.None);

        // slovo 300 ms + casti ticha pred a za (rez v tichu do 100 a 150 ms)
        Assert.IsTrue(result.Sound.DurationMs is >= 300 and <= 560, $"{result.Sound.DurationMs} ms");
        Assert.AreEqual(11, result.Characters);
        var level = SoundProcessing.SpeechLevelDb(result.Sound, -45)!.Value;
        Assert.AreEqual(SoundProcessing.SpeechLevelDb(new PcmSound(Words(3).Sound.Samples[(RATE / 2)..(RATE / 2 + RATE * 3 / 10)], RATE), -45)!.Value,
            level, 1.0, "vystrihnute je stredne slovo");

        var again = await generator.GenerateAsync(new SpeechInput("bbb", prosody), null, CancellationToken.None);
        Assert.IsTrue(again.FromCache);
        Assert.HasCount(1, provider.Requests);
    }

    [TestMethod]
    public void Extract_CasyNezodpovedajuDlzke_HladaUryvokVZnakoch()
    {
        var timed = Words(2, "aa bb");
        // sluzba vratila casy bez medzery
        var shifted = timed with { Timings = timed.Timings.Where(t => t.Character != ' ').ToList() };

        var cut = SpeechGenerator.Extract(shifted, "aa bb", 3, 2);

        Assert.IsTrue(cut.DurationMs >= 300);
    }

    [TestMethod]
    public void QuietestPoint_NajdeTicho()
    {
        var sound = Words(2).Sound;

        var point = SoundProcessing.QuietestPointMs(sound, 200, 500);

        Assert.IsTrue(point is > 300 and < 500, $"{point} ms");
    }

    [TestMethod]
    public void Concat_SpojiVoFormateBanky()
    {
        var a = new PcmSound(new float[RATE / 10], RATE);
        var b = new PcmSound(new float[44100 / 10], 44100);

        var joined = SoundProcessing.Concat([a, b], 50);

        Assert.AreEqual(RATE, joined.SampleRate);
        Assert.AreEqual(250, joined.DurationMs, 2);
    }

    [TestMethod]
    [DataRow("IC 501", "IC", "intercity", true, false, false, "intercity 501")]
    [DataRow("ICE IC", "IC", "intercity", true, false, false, "ICE intercity")]
    [DataRow("pic", "ic", "X", false, false, false, "pX")]
    [DataRow("ic", "IC", "X", true, true, false, "ic")]
    [DataRow("trať 120", @"(\d)(\d)(\d)", "$1 $2 $3", true, false, true, "trať 1 2 0")]
    [DataRow("a$b", "a", "$1", false, true, false, "$1$b")]
    [DataRow("text", "(", "x", false, false, true, "text")]
    public void Pronunciation_Pravidla(string text, string find, string replace, bool wholeWord, bool matchCase, bool regex, string expected) =>
        Assert.AreEqual(expected, Pronunciation.Apply(text, [new PronunciationRule
        {
            Find = find, Replace = replace, WholeWord = wholeWord, MatchCase = matchCase, Regex = regex
        }]));

    [TestMethod]
    public void Pronunciation_NeplatnyVyraz_Chyba()
    {
        Assert.IsNotNull(Pronunciation.Validate(new PronunciationRule { Find = "(", Regex = true }));
        Assert.IsNull(Pronunciation.Validate(new PronunciationRule { Find = "(" }));
    }

    [TestMethod]
    public void Generator_SlovnikSaUplatniPredOdoslanim()
    {
        var generator = new SpeechGenerator(new FakeProvider(true), _cache, new VoiceProfile { VoiceId = "v" }, null,
            [new PronunciationRule { Find = "Os", Replace = "osobný vlak" }]);

        var prepared = generator.Prepare(new SpeechInput("Os 3501", new GroupProsody { Punctuation = ProsodyPunctuation.Period }), null);

        Assert.AreEqual("osobný vlak 3501.", prepared.Request.Text);
    }

    [TestMethod]
    public async Task ElevenLabs_KontextATimestamps()
    {
        var bodies = new List<(string Path, string Body)>();
        var pcm = Convert.ToBase64String(new byte[] { 0, 0, 0, 64 });
        using var client = new ElevenLabsClient(new Handler(async request =>
        {
            bodies.Add((request.RequestUri!.AbsolutePath, await request.Content!.ReadAsStringAsync()));
            return request.RequestUri.AbsolutePath.EndsWith("/with-timestamps", StringComparison.Ordinal)
                ? new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("""{"audio_base64":"PCM","alignment":{"characters":["a","b"],"character_start_times_seconds":[0,0.1],"character_end_times_seconds":[0.1,0.2]}}"""
                        .Replace("PCM", pcm, StringComparison.Ordinal))
                }
                : new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent([0, 0]) };
        }), "k");

        await client.SynthesizeAsync(new SpeechRequest("x", "v", "m") { PreviousText = "pred", NextText = "za" }, CancellationToken.None);
        var timed = await client.SynthesizeTimedAsync(new SpeechRequest("ab", "v", "m"), CancellationToken.None);

        var first = JsonNode.Parse(bodies[0].Body)!;
        Assert.AreEqual("pred", (string)first["previous_text"]!);
        Assert.AreEqual("za", (string)first["next_text"]!);
        Assert.AreEqual("/v1/text-to-speech/v/with-timestamps", bodies[1].Path);
        Assert.HasCount(2, timed.Timings);
        Assert.AreEqual('b', timed.Timings[1].Character);
        Assert.AreEqual(0.2, timed.Timings[1].End);
        Assert.AreEqual(0.5f, timed.Sound.Samples[1]);
        Assert.IsFalse(client.SupportsContext("eleven_v3"));
        Assert.IsTrue(client.SupportsContext("eleven_multilingual_v2"));
    }

    [TestMethod]
    public void Cache_RecSCasmi()
    {
        var timed = Words(2, "ab cd");

        _cache.PutTimed("k", timed);
        var loaded = _cache.TryGetTimed("k");

        Assert.IsTrue(_cache.ContainsTimed("k"));
        Assert.HasCount(timed.Timings.Count, loaded!.Timings);
        Assert.AreEqual(timed.Timings[3], loaded.Timings[3]);
    }

    private sealed class Handler(Func<HttpRequestMessage, Task<HttpResponseMessage>> respond) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => respond(request);
    }
}
