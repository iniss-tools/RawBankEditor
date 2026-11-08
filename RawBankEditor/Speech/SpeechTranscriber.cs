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
