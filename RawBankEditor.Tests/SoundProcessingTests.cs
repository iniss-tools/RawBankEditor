using RawBankEditor.Tools;
using ToolsCore.Iniss.Entities;

namespace RawBankEditor.Tests;

/// <summary>
/// Uprava zvuku nahravok: orezanie ticha, hlasitost, prevzorkovanie, zapis .WAV/.EWA a nahradenie nahravok cez zurnal.
/// </summary>
[TestClass]
public class SoundProcessingTests
{
    private const int RATE = 22050;

    // ton 440 Hz s amplitudou, s tichom pred a za nim
    private static PcmSound Tone(int silenceBeforeMs, int toneMs, int silenceAfterMs, float amplitude = 0.5f, int rate = RATE)
    {
        var before = rate * silenceBeforeMs / 1000;
        var tone = rate * toneMs / 1000;
        var samples = new float[before + tone + rate * silenceAfterMs / 1000];
        for (var i = 0; i < tone; i++)
            samples[before + i] = amplitude * MathF.Sin(2 * MathF.PI * 440 * i / rate);
        return new PcmSound(samples, rate);
    }

    [TestMethod]
    public void TrimSilence_NechaPresneTichoNaOkrajoch()
    {
        var sound = Tone(500, 300, 700);

        var trimmed = SoundProcessing.TrimSilence(sound, -45, 30, 80);

        // rec 300 ms + 30 ms pred + 80 ms za (okna 10 ms - tolerancia jedneho okna)
        Assert.AreEqual(410, trimmed.DurationMs, 10);
        Assert.AreEqual(0f, trimmed.Samples[0]);
        Assert.AreEqual(0f, trimmed.Samples[^1]);
    }

    [TestMethod]
    public void TrimSilence_KratkeTicho_DoplniTichom()
    {
        var sound = Tone(0, 300, 0);

        var trimmed = SoundProcessing.TrimSilence(sound, -45, 50, 100);

        Assert.AreEqual(450, trimmed.DurationMs, 10);
        Assert.IsTrue(trimmed.Samples.Take(RATE * 50 / 1000).All(s => s == 0));
    }

    [TestMethod]
    public void TrimSilence_CeleTicho_VratiNezmenene()
    {
        var sound = new PcmSound(new float[RATE], RATE);

        Assert.AreSame(sound, SoundProcessing.TrimSilence(sound, -45, 30, 80));
    }

    [TestMethod]
    public void Normalize_DosiahneCielovuHlasitostReci()
    {
        var sound = Tone(200, 500, 200, 0.05f);

        var normalized = SoundProcessing.Normalize(sound, -20, -45);

        Assert.AreEqual(-20, SoundProcessing.SpeechLevelDb(normalized, -45)!.Value, 0.1);
    }

    [TestMethod]
    public void Normalize_SpickaNeprekrociMinusJednaDb()
    {
        var sound = Tone(0, 500, 0, 0.5f);

        var normalized = SoundProcessing.Normalize(sound, 0, -45);

        Assert.AreEqual(-1, SoundProcessing.PeakDb(normalized), 0.05);
    }

    [TestMethod]
    public void Resample_ZachovaDlzku()
    {
        var sound = Tone(0, 1000, 0, 0.5f, 44100);

        var resampled = SoundProcessing.Resample(sound, RATE);

        Assert.AreEqual(RATE, resampled.SampleRate);
        Assert.AreEqual(1000, resampled.DurationMs, 5);
    }

    [TestMethod]
    public void Fade_KrajeSuNulove()
    {
        var sound = new PcmSound(Enumerable.Repeat(0.5f, RATE).ToArray(), RATE);

        var faded = SoundProcessing.Fade(sound, 5);

        Assert.AreEqual(0f, faded.Samples[0]);
        Assert.AreEqual(0f, faded.Samples[^1]);
        Assert.AreEqual(0.5f, faded.Samples[RATE / 2]);
    }

    [TestMethod]
    public void ToWav_ReadWav_ZachovaVzorky()
    {
        var sound = Tone(10, 100, 10);

        var read = SoundProcessing.ReadWav(SoundProcessing.ToWav(sound));

        Assert.AreEqual(sound.SampleRate, read.SampleRate);
        Assert.HasCount(sound.Samples.Length, read.Samples);
        for (var i = 0; i < sound.Samples.Length; i += 97)
            Assert.AreEqual(sound.Samples[i], read.Samples[i], 1.0 / 16000);
    }

    [TestMethod]
    public void FromPcm16_PrevediePodlaLittleEndian()
    {
        var sound = SoundProcessing.FromPcm16([0x00, 0x40, 0x00, 0xC0], RATE);

        CollectionAssert.AreEqual(new[] { 0.5f, -0.5f }, sound.Samples);
    }

    [TestMethod]
    public void WriteSoundFile_Ewa_ZakodujeAReadFileDekoduje()
    {
        var dir = Directory.CreateTempSubdirectory("rbesound").FullName;
        try
        {
            var wav = SoundProcessing.ToWav(Tone(0, 100, 0));
            var path = Path.Combine(dir, "A.EWA");

            SoundProcessing.WriteSoundFile(path, wav);

            Assert.AreNotEqual((byte)'R', File.ReadAllBytes(path)[0]);
            CollectionAssert.AreEqual(wav, SoundProcessing.ReadWavBytes(path));
        }
        finally
        {
            Directory.Delete(dir, true);
        }
    }

    [TestMethod]
    public void Process_PrevedieNaFormatBanky()
    {
        var sound = Tone(300, 400, 300, 0.05f, 44100);

        var processed = SoundProcessing.Process(sound, new SoundAdjustOptions());

        Assert.AreEqual(SoundProcessing.BankSampleRate, processed.SampleRate);
        Assert.AreEqual(510, processed.DurationMs, 15);
        Assert.AreEqual(-20, SoundProcessing.SpeechLevelDb(processed, -45)!.Value, 0.5);
    }

    [TestMethod]
    public void MedianSpeechLevelDb_PreskociNecitatelne()
    {
        var dir = Directory.CreateTempSubdirectory("rbelevel").FullName;
        try
        {
            var files = new List<string>();
            foreach (var (name, amplitude) in new[] { ("a", 0.1f), ("b", 0.2f), ("c", 0.4f) })
            {
                var path = Path.Combine(dir, name + ".WAV");
                File.WriteAllBytes(path, SoundProcessing.ToWav(Tone(0, 300, 0, amplitude)));
                files.Add(path);
            }

            var broken = Path.Combine(dir, "x.WAV");
            File.WriteAllText(broken, "nie je wav");
            files.Add(broken);

            var median = SoundProcessing.MedianSpeechLevelDb(files, -45);

            Assert.AreEqual(SoundProcessing.SpeechLevelDb(Tone(0, 300, 0, 0.2f), -45)!.Value, median!.Value, 0.01);
        }
        finally
        {
            Directory.Delete(dir, true);
        }
    }
}

/// <summary>
/// Nahradenie nahravok v banke: zahodenie zmien vrati povodnu nahravku, Spat a Znova prepinaju verzie aj po ulozeni.
/// </summary>
[TestClass]
public sealed class RecordingReplaceTests : IDisposable
{
    private string _root = null!;
    private BankJournal _journal = null!;
    private SoundVersions _versions = null!;

    [TestInitialize]
    public void Init()
    {
        _root = Directory.CreateTempSubdirectory("rbereplace").FullName;
        Directory.CreateDirectory(Path.Combine(_root, "SK", "R1"));
        _journal = new BankJournal(Path.Combine(_root, "_zurnal"), path => File.Delete(path), _ => false);
        _versions = new SoundVersions(Path.Combine(_root, "_verzie"));
    }

    [TestCleanup]
    public void Cleanup() => Directory.Delete(_root, true);

    public void Dispose() => _versions.Dispose();

    private static byte[] Wav(float amplitude) =>
        SoundProcessing.ToWav(new PcmSound(Enumerable.Repeat(amplitude, 2205).ToArray(), SoundProcessing.BankSampleRate));

    private (RecordingChange Change, byte[] Old, byte[] New) Prepare(string name)
    {
        var path = Path.Combine(_root, "SK", "R1", name);
        var old = Wav(0.1f);
        SoundProcessing.WriteSoundFile(path, old);
        var file = new SoundFileElement(path);
        var sound = new FyzSound(null!, "A", "A", name, "", "", 100) { File = file };
        file.Sound = sound;
        var neu = Wav(0.3f);
        return (new RecordingChange(file, _versions.AddCopyOf(path), _versions.Add(neu), 100, 200, 100, 200), old, neu);
    }

    [TestMethod]
    public void Apply_ZapiseNovuVerziuANastaviDlzku()
    {
        var (change, _, neu) = Prepare("A.EWA");

        var errors = RecordingReplace.Apply([change], true, _journal, "SK");

        Assert.IsEmpty(errors);
        CollectionAssert.AreEqual(neu, SoundProcessing.ReadWavBytes(change.File.FileInfo.FullName));
        Assert.AreEqual(200, change.File.Duration);
        Assert.AreEqual(200, change.File.Sound.Duration);
    }

    [TestMethod]
    public void Zahodenie_VratiPovodnuNahravku()
    {
        var (change, old, _) = Prepare("A.WAV");
        RecordingReplace.Apply([change], true, _journal, "SK");

        var errors = _journal.Rollback();

        Assert.IsEmpty(errors);
        CollectionAssert.AreEqual(old, File.ReadAllBytes(change.File.FileInfo.FullName));
    }

    [TestMethod]
    public void SpatPoUlozeni_VratiPovodnuVerziu()
    {
        var (change, old, neu) = Prepare("A.EWA");
        RecordingReplace.Apply([change], true, _journal, "SK");
        _journal.Commit();

        RecordingReplace.Apply([change], false, _journal, "SK");
        CollectionAssert.AreEqual(old, SoundProcessing.ReadWavBytes(change.File.FileInfo.FullName));
        Assert.AreEqual(100, change.File.Sound.Duration);

        RecordingReplace.Apply([change], true, _journal, "SK");
        CollectionAssert.AreEqual(neu, SoundProcessing.ReadWavBytes(change.File.FileInfo.FullName));
    }

    [TestMethod]
    public void Create_BezPrepoctuDlzky_NemeniDlzkuZvuku()
    {
        var path = Path.Combine(_root, "SK", "R1", "B.WAV");
        SoundProcessing.WriteSoundFile(path, Wav(0.1f));
        var file = new SoundFileElement(path) { Duration = 100 };
        file.Sound = new FyzSound(null!, "B", "B", "B.WAV", "", "", 95) { File = file };
        var neu = new PcmSound(new float[SoundProcessing.BankSampleRate / 2], SoundProcessing.BankSampleRate);

        var change = RecordingChange.Create(file, neu, _versions, false);
        RecordingReplace.Apply([change], true, _journal, "SK");

        Assert.AreEqual(500, file.Duration);
        Assert.AreEqual(95, file.Sound.Duration);
        Assert.AreEqual(100, change.OldDuration);
    }
}
