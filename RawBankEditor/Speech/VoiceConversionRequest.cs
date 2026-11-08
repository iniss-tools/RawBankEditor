using System.Globalization;
using System.Security.Cryptography;

namespace RawBankEditor.Speech;

/// <summary>
/// Zmena hlasu nahravky (speech-to-speech): tempo a intonacia ostanu, zmeni sa hlas.
/// </summary>
/// <param name="SourceWav">povodna nahravka (WAV)</param>
/// <param name="VoiceId">cielovy hlas</param>
/// <param name="ModelId">model zmeny hlasu</param>
public sealed record VoiceConversionRequest(byte[] SourceWav, string VoiceId, string ModelId)
{
    /// <summary>Stabilita hlasu 0-1.</summary>
    public double Stability { get; init; } = 0.5;

    /// <summary>Podobnost s hlasom 0-1.</summary>
    public double SimilarityBoost { get; init; } = 0.75;

    /// <summary>Zvyraznenie stylu 0-1.</summary>
    public double Style { get; init; }

    /// <summary>Zvyraznenie podobnosti s hovoriacim.</summary>
    public bool SpeakerBoost { get; init; } = true;

    /// <summary>Seed; <see langword="null" /> = nahodny.</summary>
    public uint? Seed { get; init; }

    /// <summary>Odstranit sum z povodnej nahravky.</summary>
    public bool RemoveBackgroundNoise { get; init; }

    /// <summary>
    /// Kluc cache - nastavenia a obsah povodnej nahravky.
    /// </summary>
    public string CacheKey(string providerId)
    {
        var settings = string.Join('|', providerId, "sts", ModelId, VoiceId, Stability.ToString("R", CultureInfo.InvariantCulture),
            SimilarityBoost.ToString("R", CultureInfo.InvariantCulture), Style.ToString("R", CultureInfo.InvariantCulture), SpeakerBoost ? "1" : "0",
            Seed?.ToString(CultureInfo.InvariantCulture) ?? "-", RemoveBackgroundNoise ? "1" : "0", Convert.ToHexStringLower(SHA256.HashData(SourceWav)));
        return Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(settings)));
    }
}
