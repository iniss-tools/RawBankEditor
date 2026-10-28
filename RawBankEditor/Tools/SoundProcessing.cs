using NAudio.Utils;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using ToolsCore.Iniss.Tools;

namespace RawBankEditor.Tools;

/// <summary>
/// Zvuk nahravky v pamati - vzorky mono v rozsahu -1 az 1.
/// </summary>
/// <param name="Samples">vzorky</param>
/// <param name="SampleRate">vzorkovacia frekvencia v Hz</param>
public sealed record PcmSound(float[] Samples, int SampleRate)
{
    /// <summary>
    /// Dlzka v milisekundach - rovnako ako ju pocita INISS z bloku data.
    /// </summary>
    public int DurationMs => (int)(Samples.LongLength * 1000 / SampleRate);
}

/// <summary>
/// Uprava zvuku nahravok bez AI: citanie .WAV/.EWA, prevzorkovanie, orezanie ticha, zjednotenie hlasitosti, nabeh
/// a dobeh a zapis vo formate banky (PCM 16 bit, mono).
/// </summary>
public static class SoundProcessing
{
    /// <summary>Vzorkovacia frekvencia nahravok banky.</summary>
    public const int BankSampleRate = 22050;

    // okno merania urovne pri hladani ticha a pri merani hlasitosti
    private const int SILENCE_WINDOW_MS = 10;
    private const int LEVEL_WINDOW_MS = 50;

    // relativne hradlo merania hlasitosti - okna slabsie o viac nez 10 dB sa nerataju
    private const double RELATIVE_GATE_DB = -10;

    // najvyssia spicka po zosilneni - rezerva proti skresleniu
    private const double PEAK_CEILING_DB = -1;

    /// <summary>
    /// Obsah nahravky ako WAV - subor .EWA sa dekoduje.
    /// </summary>
    public static byte[] ReadWavBytes(string path)
    {
        if (!Path.GetExtension(path).Equals(SoundUtils.EWAExt, StringComparison.OrdinalIgnoreCase))
            return File.ReadAllBytes(path);

        using var ewa = File.OpenRead(path);
        using var wav = new MemoryStream();
        EwaCodec.Decode(ewa, wav);
        return wav.ToArray();
    }

    /// <summary>
    /// Nacita nahravku (.WAV alebo .EWA) - viac kanalov sa zmiesa do mono.
    /// </summary>
    /// <exception cref="InvalidDataException">subor nie je WAV, ktory NAudio vie citat</exception>
    public static PcmSound ReadFile(string path) => ReadWav(ReadWavBytes(path));

    /// <summary>
    /// Nacita WAV v pamati - viac kanalov sa zmiesa do mono.
    /// </summary>
    /// <exception cref="InvalidDataException">obsah nie je WAV, ktory NAudio vie citat</exception>
    public static PcmSound ReadWav(byte[] wav)
    {
        ArgumentNullException.ThrowIfNull(wav);
        try
        {
            using var reader = new WaveFileReader(new MemoryStream(wav));
            var provider = reader.ToSampleProvider();
            var channels = provider.WaveFormat.Channels;
            var samples = new List<float>((int)Math.Min(reader.SampleCount, int.MaxValue));
            var buffer = new float[channels * 4096];
            int read;
            while ((read = provider.Read(buffer, 0, buffer.Length)) > 0)
            {
                for (var i = 0; i + channels <= read; i += channels)
                {
                    var sum = 0f;
                    for (var c = 0; c < channels; c++)
                        sum += buffer[i + c];
                    samples.Add(sum / channels);
                }
            }

            return new PcmSound(samples.ToArray(), provider.WaveFormat.SampleRate);
        }
        catch (Exception e) when (e is FormatException or ArgumentException or EndOfStreamException)
        {
            throw new InvalidDataException(e.Message, e);
        }
    }

    /// <summary>
    /// Nacita zvuk vo formate WAV alebo MP3 (podla zaciatku obsahu) - ukazky hlasov a vystup sluzieb.
    /// </summary>
    /// <exception cref="InvalidDataException">obsah nie je WAV ani MP3</exception>
    public static PcmSound ReadAudio(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);
        if (data.Length >= 4 && data[0] == 'R' && data[1] == 'I' && data[2] == 'F' && data[3] == 'F')
            return ReadWav(data);

        try
        {
            using var reader = new Mp3FileReader(new MemoryStream(data));
            using var wav = new MemoryStream();
            WaveFileWriter.WriteWavFileToStream(wav, reader);
            return ReadWav(wav.ToArray());
        }
        catch (Exception e) when (e is FormatException or InvalidOperationException or EndOfStreamException or ArgumentException or NullReferenceException)
        {
            throw new InvalidDataException(e.Message, e);
        }
    }

    /// <summary>
    /// Zvuk zo surovych vzoriek PCM 16 bit mono little-endian (napr. vystup ElevenLabs <c>pcm_22050</c>).
    /// </summary>
    public static PcmSound FromPcm16(ReadOnlySpan<byte> pcm, int sampleRate)
    {
        var samples = new float[pcm.Length / 2];
        for (var i = 0; i < samples.Length; i++)
            samples[i] = (short)(pcm[2 * i] | (pcm[2 * i + 1] << 8)) / 32768f;
        return new PcmSound(samples, sampleRate);
    }

    /// <summary>
    /// WAV vo formate PCM 16 bit mono.
    /// </summary>
    public static byte[] ToWav(PcmSound sound)
    {
        ArgumentNullException.ThrowIfNull(sound);

        var pcm = new byte[sound.Samples.Length * 2];
        for (var i = 0; i < sound.Samples.Length; i++)
        {
            var value = (short)Math.Clamp(Math.Round(sound.Samples[i] * 32767.0), short.MinValue, short.MaxValue);
            pcm[2 * i] = (byte)value;
            pcm[2 * i + 1] = (byte)(value >> 8);
        }

        using var stream = new MemoryStream();
        using (var writer = new WaveFileWriter(new IgnoreDisposeStream(stream), new WaveFormat(sound.SampleRate, 16, 1)))
            writer.Write(pcm, 0, pcm.Length);
        return stream.ToArray();
    }

    /// <summary>
    /// Zapise WAV do suboru nahravky - do .EWA zakodovany.
    /// </summary>
    public static void WriteSoundFile(string path, byte[] wav)
    {
        ArgumentNullException.ThrowIfNull(wav);

        if (!Path.GetExtension(path).Equals(SoundUtils.EWAExt, StringComparison.OrdinalIgnoreCase))
        {
            File.WriteAllBytes(path, wav);
            return;
        }

        using var input = new MemoryStream(wav);
        using var output = File.Create(path);
        EwaCodec.Encode(input, output);
    }

    /// <summary>
    /// Prevzorkuje zvuk na zadanu frekvenciu.
    /// </summary>
    public static PcmSound Resample(PcmSound sound, int sampleRate)
    {
        ArgumentNullException.ThrowIfNull(sound);
        if (sound.SampleRate == sampleRate)
            return sound;
        if (sound.Samples.Length == 0)
            return sound with { SampleRate = sampleRate };

        var resampler = new WdlResamplingSampleProvider(new ArraySampleProvider(sound), sampleRate);
        var result = new List<float>((int)((long)sound.Samples.Length * sampleRate / sound.SampleRate) + 64);
        var buffer = new float[8192];
        int read;
        while ((read = resampler.Read(buffer, 0, buffer.Length)) > 0)
            result.AddRange(buffer.AsSpan(0, read));
        return new PcmSound(result.ToArray(), sampleRate);
    }

    /// <summary>
    /// Oreze ticho pred recou a za nou a necha presne <paramref name="leadMs" /> a <paramref name="tailMs" /> - co
    /// chyba, doplni tichom. Zvuk bez reci (cele ticho) vrati nezmeneny.
    /// </summary>
    /// <param name="thresholdDb">uroven okna (10 ms), od ktorej zacina rec</param>
    public static PcmSound TrimSilence(PcmSound sound, double thresholdDb, int leadMs, int tailMs)
    {
        ArgumentNullException.ThrowIfNull(sound);

        var window = Math.Max(1, sound.SampleRate * SILENCE_WINDOW_MS / 1000);
        var threshold = FromDb(thresholdDb);
        var first = -1;
        var last = -1;
        for (var start = 0; start < sound.Samples.Length; start += window)
        {
            var length = Math.Min(window, sound.Samples.Length - start);
            if (Rms(sound.Samples, start, length) < threshold)
                continue;
            if (first < 0)
                first = start;
            last = start + length;
        }

        if (first < 0)
            return sound;

        var from = first - MsToSamples(leadMs, sound.SampleRate);
        var to = last + MsToSamples(tailMs, sound.SampleRate);
        var result = new float[to - from];
        var copyFrom = Math.Max(from, 0);
        var copyTo = Math.Min(to, sound.Samples.Length);
        Array.Copy(sound.Samples, copyFrom, result, copyFrom - from, copyTo - copyFrom);
        return sound with { Samples = result };
    }

    /// <summary>
    /// Hlasitost reci v dBFS - RMS okien (50 ms) nad prahom ticha a zaroven najviac 10 dB pod hlasitostou tychto okien
    /// (relativne hradlo ako pri merani hlasitosti podla ITU-R BS.1770 - okraje slov a dych vysledok neznizia a meranie
    /// nezavisi od zosilnenia); <see langword="null" />, ak je cely zvuk ticho.
    /// </summary>
    public static double? SpeechLevelDb(PcmSound sound, double thresholdDb)
    {
        ArgumentNullException.ThrowIfNull(sound);

        var window = Math.Max(1, sound.SampleRate * LEVEL_WINDOW_MS / 1000);
        var windows = new List<(double Rms, int Length)>();
        for (var start = 0; start < sound.Samples.Length; start += window)
        {
            var length = Math.Min(window, sound.Samples.Length - start);
            windows.Add((Rms(sound.Samples, start, length), length));
        }

        var gated = Level(windows, FromDb(thresholdDb));
        return gated is null ? null : ToDb(Level(windows, Math.Max(FromDb(thresholdDb), gated.Value * FromDb(RELATIVE_GATE_DB)))!.Value);

        static double? Level(List<(double Rms, int Length)> windows, double threshold)
        {
            double power = 0;
            long count = 0;
            foreach (var (rms, length) in windows.Where(w => w.Rms >= threshold))
            {
                power += rms * rms * length;
                count += length;
            }

            return count == 0 ? null : Math.Sqrt(power / count);
        }
    }

    /// <summary>
    /// Spicka v dBFS; ticho = zaporne nekonecno.
    /// </summary>
    public static double PeakDb(PcmSound sound)
    {
        ArgumentNullException.ThrowIfNull(sound);

        var peak = 0f;
        foreach (var sample in sound.Samples)
            peak = Math.Max(peak, Math.Abs(sample));
        return ToDb(peak);
    }

    /// <summary>
    /// Zosilni alebo zoslabi rec na cielovu hlasitost - spicka vsak neprekroci -1 dBFS (zosilnenie sa obmedzi).
    /// Zvuk bez reci vrati nezmeneny.
    /// </summary>
    public static PcmSound Normalize(PcmSound sound, double targetDb, double thresholdDb)
    {
        ArgumentNullException.ThrowIfNull(sound);

        if (SpeechLevelDb(sound, thresholdDb) is not { } level)
            return sound;

        var gain = Math.Min(targetDb - level, PEAK_CEILING_DB - PeakDb(sound));
        return Gain(sound, gain);
    }

    /// <summary>
    /// Zmeni hlasitost o <paramref name="gainDb" /> decibelov.
    /// </summary>
    public static PcmSound Gain(PcmSound sound, double gainDb)
    {
        ArgumentNullException.ThrowIfNull(sound);

        var factor = (float)FromDb(gainDb);
        var result = new float[sound.Samples.Length];
        for (var i = 0; i < result.Length; i++)
            result[i] = Math.Clamp(sound.Samples[i] * factor, -1f, 1f);
        return sound with { Samples = result };
    }

    /// <summary>
    /// Linearny nabeh na zaciatku a dobeh na konci (proti luskaniu pri skladani hlasenia).
    /// </summary>
    public static PcmSound Fade(PcmSound sound, int ms)
    {
        ArgumentNullException.ThrowIfNull(sound);

        var length = Math.Min(MsToSamples(ms, sound.SampleRate), sound.Samples.Length / 2);
        if (length <= 0)
            return sound;

        var result = (float[])sound.Samples.Clone();
        for (var i = 0; i < length; i++)
        {
            var factor = (float)i / length;
            result[i] *= factor;
            result[^(i + 1)] *= factor;
        }

        return sound with { Samples = result };
    }

    /// <summary>
    /// Cela uprava podla nastaveni: format banky, orezanie ticha, hlasitost a nabeh a dobeh.
    /// </summary>
    public static PcmSound Process(PcmSound sound, SoundAdjustOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (options.ConvertFormat)
            sound = Resample(sound, BankSampleRate);
        if (options.TrimSilence)
            sound = TrimSilence(sound, options.SilenceThresholdDb, options.LeadMs, options.TailMs);
        if (options.Normalize)
            sound = Normalize(sound, options.TargetLevelDb, options.SilenceThresholdDb);
        if (options.FadeMs > 0)
            sound = Fade(sound, options.FadeMs);
        return sound;
    }

    /// <summary>
    /// Najtichsie miesto (stred 10 ms okna s najnizsou urovnou) v useku - rez tam neprejde stredom slova.
    /// </summary>
    /// <returns>cas v ms; mimo zvuku sa usek oreze.</returns>
    public static int QuietestPointMs(PcmSound sound, int fromMs, int toMs)
    {
        ArgumentNullException.ThrowIfNull(sound);

        var window = Math.Max(1, sound.SampleRate * SILENCE_WINDOW_MS / 1000);
        var from = Math.Clamp(MsToSamples(Math.Max(fromMs, 0), sound.SampleRate), 0, sound.Samples.Length);
        var to = Math.Clamp(MsToSamples(Math.Max(toMs, 0), sound.SampleRate), from, sound.Samples.Length);
        if (to - from <= window)
            return (int)((long)(from + to) / 2 * 1000 / sound.SampleRate);

        var best = from;
        var bestLevel = double.MaxValue;
        // krok pol okna - jemnejsie nez okno, rychlejsie nez po vzorke
        for (var start = from; start + window <= to; start += Math.Max(1, window / 2))
        {
            var level = Rms(sound.Samples, start, window);
            if (level < bestLevel)
            {
                bestLevel = level;
                best = start;
            }
        }

        return (int)((long)(best + window / 2) * 1000 / sound.SampleRate);
    }

    /// <summary>
    /// Usek zvuku od <paramref name="fromMs" /> po <paramref name="toMs" />.
    /// </summary>
    public static PcmSound Cut(PcmSound sound, int fromMs, int toMs)
    {
        ArgumentNullException.ThrowIfNull(sound);

        var from = Math.Clamp(MsToSamples(fromMs, sound.SampleRate), 0, sound.Samples.Length);
        var to = Math.Clamp(MsToSamples(toMs, sound.SampleRate), from, sound.Samples.Length);
        return sound with { Samples = sound.Samples[from..to] };
    }

    /// <summary>
    /// Zvuky za sebou vo formate banky (ako ich sklada INISS), medzi nimi <paramref name="gapMs" /> ticha.
    /// </summary>
    public static PcmSound Concat(IEnumerable<PcmSound> sounds, int gapMs = 0)
    {
        ArgumentNullException.ThrowIfNull(sounds);

        var gap = new float[MsToSamples(gapMs, BankSampleRate)];
        var result = new List<float>();
        foreach (var sound in sounds)
        {
            if (result.Count > 0)
                result.AddRange(gap);
            result.AddRange(Resample(sound, BankSampleRate).Samples);
        }

        return new PcmSound(result.ToArray(), BankSampleRate);
    }

    /// <summary>
    /// Typicka hlasitost reci v nahravkach - median cez najviac <paramref name="maxFiles" /> rovnomerne vybranych
    /// suborov. Nahravky, ktore sa nedaju precitat, a ticho sa preskocia.
    /// </summary>
    /// <returns>hlasitost v dBFS alebo <see langword="null" />, ak sa nedala zmerat ani jedna nahravka.</returns>
    public static double? MedianSpeechLevelDb(IReadOnlyList<string> files, double thresholdDb, int maxFiles = 200, CancellationToken cancel = default)
    {
        ArgumentNullException.ThrowIfNull(files);

        var step = Math.Max(1.0, (double)files.Count / maxFiles);
        var levels = new List<double>();
        for (var i = 0.0; i < files.Count; i += step)
        {
            cancel.ThrowIfCancellationRequested();
            try
            {
                if (SpeechLevelDb(ReadFile(files[(int)i]), thresholdDb) is { } level)
                    levels.Add(level);
            }
            catch (Exception e) when (e is IOException or InvalidDataException or UnauthorizedAccessException)
            {
                // poskodena alebo nedostupna nahravka sa nemeria
            }
        }

        if (levels.Count == 0)
            return null;

        levels.Sort();
        return levels.Count % 2 == 1 ? levels[levels.Count / 2] : (levels[levels.Count / 2 - 1] + levels[levels.Count / 2]) / 2;
    }

    private static double Rms(float[] samples, int start, int length)
    {
        double sum = 0;
        for (var i = start; i < start + length; i++)
            sum += samples[i] * samples[i];
        return length == 0 ? 0 : Math.Sqrt(sum / length);
    }

    private static int MsToSamples(int ms, int sampleRate) => (int)((long)Math.Max(ms, 0) * sampleRate / 1000);

    private static double FromDb(double db) => Math.Pow(10, db / 20);

    private static double ToDb(double value) => value <= 0 ? double.NegativeInfinity : 20 * Math.Log10(value);

    /// <summary>
    /// Vzorky zvuku ako zdroj pre NAudio (prevzorkovanie).
    /// </summary>
    private sealed class ArraySampleProvider(PcmSound sound) : ISampleProvider
    {
        private int _position;

        public WaveFormat WaveFormat { get; } = WaveFormat.CreateIeeeFloatWaveFormat(sound.SampleRate, 1);

        public int Read(float[] buffer, int offset, int count)
        {
            var read = Math.Min(count, sound.Samples.Length - _position);
            Array.Copy(sound.Samples, _position, buffer, offset, read);
            _position += read;
            return read;
        }
    }
}
