using System.Globalization;
using System.Security.Cryptography;
using RawBankEditor.Tools;

namespace RawBankEditor.Speech;

/// <summary>
/// Prepis nahravok na text (ElevenLabs Scribe, Azure Speech) s cache podla obsahu nahravky - doplnenie textov zvukov a kontrola, ci
/// vygenerovana nahravka hovori to, co mala.
/// </summary>
/// <param name="provider">sluzba prepisu</param>
/// <param name="cache">cache vysledkov</param>
public sealed class SpeechTranscriber(ISpeechProvider provider, SpeechCache cache)
{
    /// <summary>
    /// Prepise nahravku.
    /// </summary>
    /// <param name="sound">nahravka</param>
    /// <param name="languageCode">jazyk reci (ISO 639-1); prazdny = zisti sluzba</param>
    /// <returns>prepis a priznak, ci bol v cache.</returns>
    /// <exception cref="SpeechException">sluzba poziadavku odmietla alebo nie je dostupna</exception>
    public async Task<(string Text, bool FromCache)> TranscribeAsync(PcmSound sound, string languageCode, CancellationToken cancel)
    {
        ArgumentNullException.ThrowIfNull(sound);

        var wav = SoundProcessing.ToWav(SoundProcessing.Resample(sound, SoundProcessing.BankSampleRate));
        var key = Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(
            string.Join('|', provider.Id, "stt", provider.TranscriptionModel, languageCode, Convert.ToHexStringLower(SHA256.HashData(wav))))));
        if (cache.TryGetText(key) is { } cached)
            return (cached, true);

        var text = await provider.TranscribeAsync(wav, languageCode, cancel).ConfigureAwait(false);
        cache.PutText(key, text);
        return (text, false);
    }

    /// <summary>
    /// Prepise subor nahravky (.WAV/.EWA).
    /// </summary>
    /// <exception cref="SpeechException">sluzba poziadavku odmietla alebo nie je dostupna</exception>
    /// <exception cref="InvalidDataException">nahravka sa neda precitat</exception>
    public Task<(string Text, bool FromCache)> TranscribeFileAsync(string path, string languageCode, CancellationToken cancel) =>
        TranscribeAsync(SoundProcessing.ReadFile(path), languageCode, cancel);
}

/// <summary>
/// Porovnanie textu s prepisom nahravky - zhoda bez ohladu na velkost pismen, diakritiku a interpunkciu.
/// </summary>
public static class TextMatch
{
    /// <summary>Pod touto zhodou je nahravka podozriva.</summary>
    public const double Threshold = 0.8;

    /// <summary>
    /// Text na porovnanie: male pismena bez diakritiky, len pismena a cislice, medzery zlucene.
    /// </summary>
    public static string Normalize(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var decomposed = text.ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var result = new StringBuilder(decomposed.Length);
        var space = false;
        foreach (var c in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
                continue;
            if (char.IsLetterOrDigit(c))
            {
                if (space && result.Length > 0)
                    result.Append(' ');
                result.Append(c);
                space = false;
            }
            else
            {
                space = true;
            }
        }

        return result.ToString();
    }

    /// <summary>
    /// Zhoda 0-1 (1 = rovnake) podla poctu upravenych znakov (Levenshtein) po <see cref="Normalize" />.
    /// </summary>
    public static double Similarity(string expected, string transcript)
    {
        var a = Normalize(expected);
        var b = Normalize(transcript);
        if (a.Length == 0 && b.Length == 0)
            return 1;
        return 1 - (double)Distance(a, b) / Math.Max(a.Length, b.Length);
    }

    /// <summary>
    /// Najlepsia zhoda prepisu s niektorym z ocakavanych textov (napr. text zvuku a text po slovniku vyslovnosti).
    /// </summary>
    public static double Best(IEnumerable<string> expected, string transcript) =>
        expected.Where(e => !string.IsNullOrWhiteSpace(e)).Select(e => Similarity(e, transcript)).DefaultIfEmpty(0).Max();

    /// <summary>
    /// Prepis upraveny na text zvuku - bez interpunkcie na konci (texty v banke ju nemaju).
    /// </summary>
    public static string AsSoundText(string transcript) => transcript.Trim().TrimEnd('.', ',', ';', ':', '!', '?', '…').Trim();

    private static int Distance(string a, string b)
    {
        var previous = new int[b.Length + 1];
        var current = new int[b.Length + 1];
        for (var j = 0; j <= b.Length; j++)
            previous[j] = j;

        for (var i = 1; i <= a.Length; i++)
        {
            current[0] = i;
            for (var j = 1; j <= b.Length; j++)
                current[j] = Math.Min(Math.Min(current[j - 1] + 1, previous[j] + 1), previous[j - 1] + (a[i - 1] == b[j - 1] ? 0 : 1));
            (previous, current) = (current, previous);
        }

        return previous[b.Length];
    }
}
