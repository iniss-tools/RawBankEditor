using RawBankEditor.Tools;

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
