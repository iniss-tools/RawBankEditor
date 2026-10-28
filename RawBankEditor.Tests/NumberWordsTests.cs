using System.Text.Json;
using RawBankEditor.Speech;

namespace RawBankEditor.Tests;

/// <summary>
/// Cisla slovami pred syntezou (slovencina, cestina).
/// </summary>
[TestClass]
public sealed class NumberWordsTests
{
    [TestMethod]
    [DataRow("11. apríl - deň narcisov.", "jedenásty apríl - deň narcisov.")]
    [DataRow("od 27. mája do 2. júna", "od dvadsiateho siedmeho mája do druhého júna")]
    [DataRow("dňa 15. marca", "dňa pätnásteho marca")]
    [DataRow("Dňa 15. 3. 2024 o 14:05.", "Dňa pätnásteho tretieho dvetisíc dvadsaťštyri o štrnásť nula päť.")]
    [DataRow("3. koľaj", "tretia koľaj")]
    [DataRow("1. nástupište", "prvé nástupište")]
    [DataRow("2. trieda", "druhá trieda")]
    [DataRow("Os 3501 a IC 501", "Os tritisíc päťsto jeden a IC päťsto jeden")]
    [DataRow("na číslo 877. Ďakujeme.", "na číslo osemsto sedemdesiatsedem. Ďakujeme.")]
    [DataRow("70173501", "sedem nula jedna sedem tri päť nula jedna")]
    [DataRow("0918 123", "nula deväť jedna osem sto dvadsaťtri")]
    [DataRow("2 minúty a 2 vozne", "dve minúty a dva vozne")]
    [DataRow("1 hodina", "jedna hodina")]
    [DataRow("o 9:30", "o deväť tridsať")]
    [DataRow("2,5 km", "2,5 km")]
    [DataRow("bez čísel", "bez čísel")]
    public void Slovencina(string text, string expected) => Assert.AreEqual(expected, NumberWords.Apply(text, "sk"));

    [TestMethod]
    [DataRow("11. dubna", "jedenáctého dubna")]
    [DataRow("1. kolej", "první kolej")]
    [DataRow("2. třída", "druhá třída")]
    [DataRow("Os 3501", "Os tři tisíce pět set jedna")]
    [DataRow("21 minut", "dvacet jedna minut")]
    [DataRow("1 vůz", "jeden vůz")]
    [DataRow("2 minuty", "dvě minuty")]
    [DataRow("v 14:00", "v čtrnáct nula nula")]
    [DataRow("70173501", "sedm nula jedna sedm tři pět nula jedna")]
    public void Cestina(string text, string expected) => Assert.AreEqual(expected, NumberWords.Apply(text, "cs"));

    [TestMethod]
    public void InyJazyk_BezZmeny()
    {
        Assert.AreEqual("Platform 3", NumberWords.Apply("Platform 3", "en"));
        Assert.AreEqual("11. apríl", NumberWords.Apply("11. apríl", ""));
    }

    [TestMethod]
    public void Generator_SlovnikMaPrednostPredCislami()
    {
        var root = Directory.CreateTempSubdirectory("rbenumbers").FullName;
        try
        {
            var rules = new[] { new PronunciationRule { Find = "IC 501", Replace = "intercity päť nula jeden" } };
            var generator = new SpeechGenerator(new NullProvider(), new SpeechCache(root), new VoiceProfile { VoiceId = "v" }, null, rules, "sk");

            Assert.AreEqual("intercity päť nula jeden a osobný vlak tritisíc päťsto jeden", generator.Speakable("IC 501 a osobný vlak 3501"));
            Assert.AreEqual("intercity päť nula jeden a osobný vlak tritisíc päťsto jeden",
                generator.Prepare(new SpeechInput("IC 501 a osobný vlak 3501"), null).Request.Text);
        }
        finally
        {
            Directory.Delete(root, true);
        }
    }

    [TestMethod]
    public void Nastavenie_PredvolenePrevodZapnuty()
    {
        var data = JsonSerializer.Deserialize<LanguageAiData>("""{"version":1}""")!;

        Assert.IsTrue(data.NumbersAsWords);
        Assert.AreEqual("sk", data.NumbersLanguage("sk"));
        Assert.AreEqual("", data.NumbersLanguage("en"));
        data.NumbersAsWords = false;
        Assert.AreEqual("", data.NumbersLanguage("sk"));
    }

    private sealed class NullProvider : ISpeechProvider
    {
        public string Id => "null";

        public SpeechFeatures Features => SpeechFeatures.None;

        public string TranscriptionModel => "";

        public Task<IReadOnlyList<SpeechVoice>> GetVoicesAsync(CancellationToken cancel) => throw new NotSupportedException();

        public Task<IReadOnlyList<SpeechModel>> GetModelsAsync(CancellationToken cancel) => throw new NotSupportedException();

        public Task<SpeechQuota> GetQuotaAsync(CancellationToken cancel) => throw new NotSupportedException();

        public Task<Tools.PcmSound> SynthesizeAsync(SpeechRequest request, CancellationToken cancel) => throw new NotSupportedException();

        public Task<TimedSpeech> SynthesizeTimedAsync(SpeechRequest request, CancellationToken cancel) => throw new NotSupportedException();

        public Task<Tools.PcmSound> ConvertVoiceAsync(VoiceConversionRequest request, CancellationToken cancel) => throw new NotSupportedException();

        public Task<Tools.PcmSound> IsolateAsync(byte[] sourceWav, CancellationToken cancel) => throw new NotSupportedException();

        public Task<string> TranscribeAsync(byte[] sourceWav, string languageCode, CancellationToken cancel) => throw new NotSupportedException();

        public bool SupportsContext(string modelId) => false;
    }
}
