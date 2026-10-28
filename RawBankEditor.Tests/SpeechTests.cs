using System.Net;
using System.Text.Json.Nodes;
using RawBankEditor.Speech;
using RawBankEditor.Tools;

namespace RawBankEditor.Tests;

/// <summary>
/// Hlasova synteza: poziadavky na ElevenLabs, opakovanie pri preplneni, chyby sluzby, cache a udaje jazyka v .rbe-ai.
/// </summary>
[TestClass]
public class SpeechTests
{
    /// <summary>
    /// Falosna sluzba - zaznamena poziadavky a vrati pripravene odpovede.
    /// </summary>
    private sealed class FakeHandler(params Func<HttpRequestMessage, HttpResponseMessage>[] responses) : HttpMessageHandler
    {
        public List<(HttpRequestMessage Request, string Body)> Requests { get; } = [];

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var body = request.Content is null ? "" : await request.Content.ReadAsStringAsync(cancellationToken);
            Requests.Add((request, body));
            return responses[Math.Min(Requests.Count - 1, responses.Length - 1)](request);
        }
    }

    private static HttpResponseMessage Pcm(params byte[] pcm) => new(HttpStatusCode.OK) { Content = new ByteArrayContent(pcm) };

    private static HttpResponseMessage Json(HttpStatusCode status, string json) =>
        new(status) { Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") };

    private static ElevenLabsClient Client(FakeHandler handler) => new(handler, "kluc") { Delay = (_, _) => Task.CompletedTask };

    [TestMethod]
    public async Task Synthesize_PosliePoziadavkuVoFormateBanky()
    {
        var handler = new FakeHandler(_ => Pcm(0x00, 0x40, 0x00, 0xC0));
        using var client = Client(handler);
        var request = new VoiceProfile { VoiceId = "hlas 1", ModelId = "eleven_flash_v2_5", LanguageCode = "sk", Seed = 7 }.ToRequest("Dolné Mesto");

        var sound = await client.SynthesizeAsync(request, CancellationToken.None);

        var (message, body) = handler.Requests.Single();
        Assert.AreEqual(HttpMethod.Post, message.Method);
        Assert.AreEqual("/v1/text-to-speech/hlas%201", message.RequestUri!.AbsolutePath);
        Assert.AreEqual("?output_format=pcm_22050", message.RequestUri.Query);
        Assert.AreEqual("kluc", message.Headers.GetValues("xi-api-key").Single());
        var json = JsonNode.Parse(body)!;
        Assert.AreEqual("Dolné Mesto", (string)json["text"]!);
        Assert.AreEqual("eleven_flash_v2_5", (string)json["model_id"]!);
        Assert.AreEqual("sk", (string)json["language_code"]!);
        Assert.AreEqual(7u, (uint)json["seed"]!);
        Assert.AreEqual(0.75, (double)json["voice_settings"]!["similarity_boost"]!);
        Assert.AreEqual(SoundProcessing.BankSampleRate, sound.SampleRate);
        CollectionAssert.AreEqual(new[] { 0.5f, -0.5f }, sound.Samples);
    }

    [TestMethod]
    public async Task Synthesize_MultilingualV2_BezKoduJazykaASeedu()
    {
        var handler = new FakeHandler(_ => Pcm(0, 0));
        using var client = Client(handler);
        var request = new VoiceProfile { VoiceId = "v", LanguageCode = "sk" }.ToRequest("text");

        await client.SynthesizeAsync(request, CancellationToken.None);

        var json = JsonNode.Parse(handler.Requests.Single().Body)!.AsObject();
        Assert.IsFalse(json.ContainsKey("language_code"));
        Assert.IsFalse(json.ContainsKey("seed"));
    }

    [TestMethod]
    public async Task Synthesize_PreplnenieZopakuje()
    {
        var handler = new FakeHandler(
            _ => Json(HttpStatusCode.TooManyRequests, """{"detail":{"status":"too_many_concurrent_requests","message":"Too many"}}"""),
            _ => Pcm(0, 0));
        using var client = Client(handler);

        await client.SynthesizeAsync(new SpeechRequest("text", "v", "m"), CancellationToken.None);

        Assert.HasCount(2, handler.Requests);
    }

    [TestMethod]
    public async Task Synthesize_ChybaSluzby_SpravaZDetailu()
    {
        var handler = new FakeHandler(_ => Json(HttpStatusCode.Unauthorized, """{"detail":{"status":"invalid_api_key","message":"Invalid API key"}}"""));
        using var client = Client(handler);

        var error = await Assert.ThrowsExactlyAsync<SpeechException>(() => client.SynthesizeAsync(new SpeechRequest("text", "v", "m"), CancellationToken.None));

        Assert.AreEqual("Invalid API key", error.Message);
        Assert.AreEqual(401, error.StatusCode);
        Assert.HasCount(1, handler.Requests);
    }

    [TestMethod]
    [DataRow("""{"detail":"Not found"}""", "Not found")]
    [DataRow("""{"detail":[{"loc":["body","text"],"msg":"field required"}]}""", "field required")]
    [DataRow("nie json", "nie json")]
    public void ErrorMessage_RozneTvaryOdpovede(string body, string expected) =>
        Assert.AreEqual(expected, ElevenLabsClient.ErrorMessage(body));

    [TestMethod]
    public async Task GetVoices_PrejdeVsetkyStranky()
    {
        var handler = new FakeHandler(
            _ => Json(HttpStatusCode.OK, """{"voices":[{"voice_id":"b","name":"Zuzana","category":"premade","labels":{"gender":"female","age":"middle_aged","descriptive":"warm","use_case":"social_media"}}],"has_more":true,"next_page_token":"t2"}"""),
            _ => Json(HttpStatusCode.OK, """{"voices":[{"voice_id":"a","name":"Adam"},{"voice_id":"c","name":"INISS SK","category":"cloned","labels":{"accent":"slovak"}}],"has_more":false,"next_page_token":null}"""));
        using var client = Client(handler);

        var voices = await client.GetVoicesAsync(CancellationToken.None);

        // vlastne hlasy navrch, v popise len pohlavie, vek, prizvuk a druh vlastneho hlasu
        Assert.AreEqual("INISS SK (slovak, cloned)", voices[0].ToString());
        Assert.IsTrue(voices[0].Own);
        Assert.AreEqual("Adam", voices[1].ToString());
        Assert.AreEqual("Zuzana (female, middle aged)", voices[2].ToString());
        Assert.Contains("next_page_token=t2", handler.Requests[1].Request.RequestUri!.Query);
    }

    [TestMethod]
    public async Task GetModels_LenModelyPreText()
    {
        var handler = new FakeHandler(_ => Json(HttpStatusCode.OK, """
            [{"model_id":"eleven_multilingual_v2","name":"Multilingual v2","can_do_text_to_speech":true,"languages":[{"language_id":"sk","name":"Slovak"}]},
             {"model_id":"eleven_multilingual_sts_v2","name":"STS","can_do_text_to_speech":false}]
            """));
        using var client = Client(handler);

        var models = await client.GetModelsAsync(CancellationToken.None);

        Assert.AreEqual("eleven_multilingual_v2", models.Single().Id);
        CollectionAssert.AreEqual(new[] { "sk" }, models.Single().Languages.ToList());
    }

    [TestMethod]
    public async Task GetQuota_ZostavajuceZnaky()
    {
        var handler = new FakeHandler(_ => Json(HttpStatusCode.OK, """{"tier":"creator","character_count":1200,"character_limit":100000}"""));
        using var client = Client(handler);

        var quota = await client.GetQuotaAsync(CancellationToken.None);

        Assert.AreEqual("creator", quota.Tier);
        Assert.AreEqual(98800, quota.Remaining);
    }

    [TestMethod]
    public void CacheKey_ZavisiOdTextuHlasuASeedu()
    {
        var profile = new VoiceProfile { VoiceId = "v" };
        var key = profile.ToRequest("a").CacheKey("elevenlabs");

        Assert.AreEqual(key, profile.ToRequest("a").CacheKey("elevenlabs"));
        Assert.AreNotEqual(key, profile.ToRequest("b").CacheKey("elevenlabs"));
        Assert.AreNotEqual(key, profile.ToRequest("a", 5).CacheKey("elevenlabs"));
        var faster = profile.Clone();
        faster.Speed = 1.1;
        Assert.AreNotEqual(key, faster.ToRequest("a").CacheKey("elevenlabs"));
    }

    [TestMethod]
    public async Task Generator_DruhyRazZCache()
    {
        var root = Directory.CreateTempSubdirectory("rbecache").FullName;
        try
        {
            var pcm = new byte[2205 * 2];
            for (var i = 0; i < 2205; i++)
                BitConverter.TryWriteBytes(pcm.AsSpan(2 * i), (short)(8000 * Math.Sin(i / 5.0)));
            var handler = new FakeHandler(_ => Pcm(pcm));
            using var client = Client(handler);
            var generator = new SpeechGenerator(client, new SpeechCache(root), new VoiceProfile { VoiceId = "v" }, new SoundAdjustOptions());

            var first = await generator.GenerateAsync("Lipová", null, CancellationToken.None);
            var second = await generator.GenerateAsync("Lipová", null, CancellationToken.None);

            Assert.IsFalse(first.FromCache);
            Assert.AreEqual(6, first.Characters);
            Assert.IsTrue(second.FromCache);
            Assert.AreEqual(0, second.Characters);
            Assert.HasCount(1, handler.Requests);
            CollectionAssert.AreEqual(first.Sound.Samples, second.Sound.Samples);
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [TestMethod]
    public void BankAiFolder_UlozenyHlasSaNacita()
    {
        var root = Directory.CreateTempSubdirectory("rbeai").FullName;
        try
        {
            var data = new LanguageAiData { Voice = new VoiceProfile { VoiceId = "abc", VoiceName = "Adam", LanguageCode = "sk", Seed = 42, Speed = 0.9 } };

            BankAiFolder.Save(root, "SK", data);
            var loaded = BankAiFolder.Load(root, "SK");

            Assert.IsTrue(File.Exists(Path.Combine(root, ".rbe-ai", "SK.json")));
            Assert.AreEqual("abc", loaded.Voice!.VoiceId);
            Assert.AreEqual(42u, loaded.Voice.Seed);
            Assert.AreEqual(0.9, loaded.Voice.Speed);
            Assert.Contains("\"voiceId\"", File.ReadAllText(BankAiFolder.FileOf(root, "SK")));
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [TestMethod]
    public void BankAiFolder_PoskodenySubor_PrazdneUdaje()
    {
        var root = Directory.CreateTempSubdirectory("rbeai").FullName;
        try
        {
            Directory.CreateDirectory(Path.Combine(root, ".rbe-ai"));
            File.WriteAllText(BankAiFolder.FileOf(root, "CZ"), "{ nie je json");

            Assert.IsNull(BankAiFolder.Load(root, "CZ").Voice);
            Assert.IsNull(BankAiFolder.Load(root, "GB").Voice);
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [TestMethod]
    [DataRow("SK", "sk")]
    [DataRow("CZ", "cs")]
    [DataRow("GB", "en")]
    [DataRow("D", "de")]
    [DataRow("XYZ", "")]
    public void LanguageCodeOf_KlucJazykaBanky(string key, string expected) => Assert.AreEqual(expected, VoiceProfile.LanguageCodeOf(key));
}
