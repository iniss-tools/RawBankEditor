using RawBankEditor.Speech;
using RawBankEditor.Tools;

namespace RawBankEditor.Tests;

/// <summary>
/// Tempo reci vygenerovanych nahravok a uprava rychlosti hlasu pre uryvok.
/// </summary>
[TestClass]
public sealed class TempoTests
{
    private const int RATE = SoundProcessing.BankSampleRate;

    [TestMethod]
    [DataRow("Stanica Devínska Nová Ves.", 9)]
    [DataRow("linky S", 3)]
    [DataRow("Vlak do Brna stojí na koľaji.", 10)]
    [DataRow("vlk a prst", 3)]
    [DataRow("ZSSK", 4)]
    [DataRow("11. apríl", 6)]
    [DataRow("Deň, keď sú slová zbytočné.", 8)]
    public void Slabiky(string text, int syllables) => Assert.AreEqual(syllables, SpeechTempo.Syllables(text));

    /// <summary>
    /// Rec v slabikach po 200 ms tonu, medzi nimi ticho - tempo nezavisi od prestavok.
    /// </summary>
    private static PcmSound Speech(int syllables, int pauseMs)
    {
        var samples = new List<float>();
        for (var i = 0; i < syllables; i++)
        {
            samples.AddRange(Enumerable.Range(0, RATE / 5).Select(n => 0.3f * MathF.Sin(n / 5f)));
            samples.AddRange(new float[RATE * pauseMs / 1000]);
        }

        return new PcmSound(samples.ToArray(), RATE);
    }

    [TestMethod]
    public void Tempo_SlabikyZaSekunduReciBezPrestavok()
    {
        var text = "Stanica Devínska Nová Ves.";

        Assert.AreEqual(5, SpeechTempo.Rate(Speech(9, 50), text)!.Value, 0.3);
        Assert.AreEqual(5, SpeechTempo.Rate(Speech(9, 300), text)!.Value, 0.3, "prestavky tempo nemenia");
        Assert.IsNull(SpeechTempo.Rate(Speech(3, 50), "linky S"), "kratky uryvok sa nemeria");
    }

    [TestMethod]
    public void Median_AzOdTrochNahravok()
    {
        Assert.IsNull(SpeechTempo.Median([5, 6]));
        Assert.AreEqual(5.0, SpeechTempo.Median([4, 5, 7])!.Value);
        Assert.AreEqual(5.5, SpeechTempo.Median([4, 5, 6, 7])!.Value);
    }

    [TestMethod]
    public void Uprava_RychlyUryvokPomalsieANajviacOTretinu()
    {
        Assert.AreEqual(1 / 1.3, SpeechTempo.Corrected(1, 1.3), 0.001);
        Assert.AreEqual(1 / 1.33, SpeechTempo.Corrected(1, 2), 0.001);
        Assert.AreEqual(1.16, SpeechTempo.Corrected(1, 0.86), 0.01);
        Assert.IsTrue(SpeechTempo.NeedsCorrection(1.15));
        Assert.IsFalse(SpeechTempo.NeedsCorrection(0.95));
    }

    [TestMethod]
    [DataRow("elevenlabs", 0.9, 0.7, 0.7)]
    [DataRow("elevenlabs", 0.9, 1.1, 0.99)]
    [DataRow("piper", 0.9, 0.7, 0.63)]
    public void Prepare_UpravaRychlostiVRozsahuSluzbyBezZmenyOdtlacku(string providerId, double speed, double factor, double expected)
    {
        var root = Directory.CreateTempSubdirectory("rbetempo").FullName;
        try
        {
            var generator = new SpeechGenerator(new IdProvider(providerId), new SpeechCache(root), new VoiceProfile { Provider = providerId, VoiceId = "v", Speed = speed },
                null);
            var input = new SpeechInput("Stanica Devínska Nová Ves.");

            var plain = generator.Prepare(input, null);
            var corrected = generator.Prepare(input, null, factor);

            Assert.AreEqual(expected, corrected.Request.Speed, 0.001);
            Assert.AreEqual(plain.Fingerprint, corrected.Fingerprint, "zjednotenie tempa nie je iny hlas");
            Assert.AreNotEqual(plain.CacheKey, corrected.CacheKey);
            Assert.AreEqual("Stanica Devínska Nová Ves.", corrected.SpokenText);
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    private sealed class IdProvider(string id) : ISpeechProvider
    {
        public string Id => id;

        public SpeechFeatures Features => SpeechFeatures.Timings;

        public string TranscriptionModel => "";

        public Task<IReadOnlyList<SpeechVoice>> GetVoicesAsync(CancellationToken cancel) => throw new NotSupportedException();

        public Task<IReadOnlyList<SpeechModel>> GetModelsAsync(CancellationToken cancel) => throw new NotSupportedException();

        public Task<SpeechQuota> GetQuotaAsync(CancellationToken cancel) => throw new NotSupportedException();

        public Task<PcmSound> SynthesizeAsync(SpeechRequest request, CancellationToken cancel) => throw new NotSupportedException();

        public Task<TimedSpeech> SynthesizeTimedAsync(SpeechRequest request, CancellationToken cancel) => throw new NotSupportedException();

        public Task<PcmSound> ConvertVoiceAsync(VoiceConversionRequest request, CancellationToken cancel) => throw new NotSupportedException();

        public Task<PcmSound> IsolateAsync(byte[] sourceWav, CancellationToken cancel) => throw new NotSupportedException();

        public Task<string> TranscribeAsync(byte[] sourceWav, string languageCode, CancellationToken cancel) => throw new NotSupportedException();

        public bool SupportsContext(string modelId) => true;
    }
}
