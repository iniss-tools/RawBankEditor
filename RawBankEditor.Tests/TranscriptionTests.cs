using System.Net;
using RawBankEditor.Forms;
using RawBankEditor.Speech;
using RawBankEditor.Tools;
using ToolsCore.Iniss.Entities;

namespace RawBankEditor.Tests;

/// <summary>
/// Prepis nahravok na text a kontrola zhody textu s prepisom.
/// </summary>
[TestClass]
public sealed class TranscriptionTests
{
    private const int RATE = SoundProcessing.BankSampleRate;
    private string _root = null!;

    [TestInitialize]
    public void Init() => _root = Directory.CreateTempSubdirectory("rbestt").FullName;

    [TestCleanup]
    public void Cleanup() => Directory.Delete(_root, true);

    [TestMethod]
    [DataRow("Dolné Mesto", "dolne mesto.", 1.0)]
    [DataRow("Nové  Zámky,", "Nové Zámky", 1.0)]
    [DataRow("", "", 1.0)]
    [DataRow("Lipová", "", 0.0)]
    public void Similarity_BezVelkostiDiakritikyAInterpunkcie(string expected, string transcript, double similarity) =>
        Assert.AreEqual(similarity, TextMatch.Similarity(expected, transcript), 0.001);

    [TestMethod]
    public void Similarity_IneSlovo_PodPrahom()
    {
        Assert.IsTrue(TextMatch.Similarity("Dolné Mesto", "Horná Ves") < TextMatch.Threshold);
        Assert.IsTrue(TextMatch.Similarity("Bratislava hlavná stanica", "Bratislava hlavna stanica") >= TextMatch.Threshold);
        Assert.IsTrue(TextMatch.Similarity("Bratislava hlavná stanica", "Bratislava hlavné stanice") >= TextMatch.Threshold);
    }

    [TestMethod]
    public void Best_NajlepsiZOcakavanych() =>
        Assert.AreEqual(1.0, TextMatch.Best(["Os 3501", "osobný vlak 3501"], "Osobný vlak 3501."), 0.001);

    [TestMethod]
    [DataRow("Dolné Mesto.", "Dolné Mesto")]
    [DataRow(" a šesť, ", "a šesť")]
    [DataRow("Pozor!", "Pozor")]
    public void AsSoundText_BezInterpunkcieNaKonci(string transcript, string expected) =>
        Assert.AreEqual(expected, TextMatch.AsSoundText(transcript));

    [TestMethod]
    public async Task ElevenLabs_Prepis()
    {
        string? model = null, language = null, file = null;
        using var client = new ElevenLabsClient(new Handler(async request =>
        {
            foreach (var part in (MultipartFormDataContent)request.Content!)
            {
                var name = part.Headers.ContentDisposition!.Name!.Trim('"');
                if (name == "model_id") model = await part.ReadAsStringAsync();
                if (name == "language_code") language = await part.ReadAsStringAsync();
                if (name == "file") file = part.Headers.ContentType!.MediaType;
            }

            Assert.AreEqual("/v1/speech-to-text", request.RequestUri!.AbsolutePath);
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("""{"text":" Dolné Mesto. ","language_code":"slk"}""") };
        }), "k");

        var text = await client.TranscribeAsync(SoundProcessing.ToWav(new PcmSound(new float[RATE / 5], RATE)), "sk", CancellationToken.None);

        Assert.AreEqual("Dolné Mesto.", text);
        Assert.AreEqual(ElevenLabsClient.SttModel, model);
        Assert.AreEqual("sk", language);
        Assert.AreEqual("audio/wav", file);
    }

    [TestMethod]
    public async Task Transcriber_DruhyRazZCache()
    {
        var provider = new FakeProvider();
        var transcriber = new SpeechTranscriber(provider, new SpeechCache(_root));
        var sound = new PcmSound(Enumerable.Repeat(0.1f, RATE / 2).ToArray(), RATE);

        var first = await transcriber.TranscribeAsync(sound, "sk", CancellationToken.None);
        var second = await transcriber.TranscribeAsync(sound, "sk", CancellationToken.None);
        await transcriber.TranscribeAsync(sound, "cs", CancellationToken.None);

        Assert.AreEqual("Lipová", first.Text);
        Assert.IsFalse(first.FromCache);
        Assert.IsTrue(second.FromCache);
        Assert.AreEqual(2, provider.Transcriptions, "iny jazyk = nova poziadavka");
    }

    [TestMethod]
    public void TranscriptionRow_BezTextu_PrevezmeSa()
    {
        var row = new TranscriptionRow(new FyzSound(null!, "A", "A", "A.WAV", "", "", 0));

        row.SetHeard("Dolné Mesto.");

        Assert.AreEqual("Dolné Mesto", row.Transcript);
        Assert.IsTrue(row.Accept);
        Assert.IsNull(row.Match);
        Assert.IsFalse(row.Mismatch);
    }

    [TestMethod]
    public void TranscriptionRow_STextom_NezhodaSaNeprevezme()
    {
        var row = new TranscriptionRow(new FyzSound(null!, "A", "A", "A.WAV", "", "Lipová", 0));

        row.SetHeard("Horná Ves.");

        Assert.IsTrue(row.Mismatch);
        Assert.IsFalse(row.Accept);

        row.SetHeard("lipova");
        Assert.IsFalse(row.Mismatch);
    }

    private sealed class Handler(Func<HttpRequestMessage, Task<HttpResponseMessage>> respond) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => respond(request);
    }

    private sealed class FakeProvider : ISpeechProvider
    {
        public int Transcriptions { get; private set; }

        public string Id => "fake";

        public Task<IReadOnlyList<SpeechVoice>> GetVoicesAsync(CancellationToken cancel) => Task.FromResult<IReadOnlyList<SpeechVoice>>([]);

        public Task<IReadOnlyList<SpeechModel>> GetModelsAsync(CancellationToken cancel) => Task.FromResult<IReadOnlyList<SpeechModel>>([]);

        public Task<SpeechQuota> GetQuotaAsync(CancellationToken cancel) => Task.FromResult(new SpeechQuota("", 0, 0));

        public Task<PcmSound> SynthesizeAsync(SpeechRequest request, CancellationToken cancel) => throw new NotSupportedException();

        public Task<TimedSpeech> SynthesizeTimedAsync(SpeechRequest request, CancellationToken cancel) => throw new NotSupportedException();

        public Task<PcmSound> ConvertVoiceAsync(VoiceConversionRequest request, CancellationToken cancel) => throw new NotSupportedException();

        public Task<PcmSound> IsolateAsync(byte[] sourceWav, CancellationToken cancel) => throw new NotSupportedException();

        public Task<string> TranscribeAsync(byte[] sourceWav, string languageCode, CancellationToken cancel)
        {
            Transcriptions++;
            return Task.FromResult("Lipová");
        }

        public bool SupportsContext(string modelId) => true;
    }
}
