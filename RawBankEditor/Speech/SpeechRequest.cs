using System.Globalization;
using System.Security.Cryptography;

namespace RawBankEditor.Speech;

/// <summary>
/// Poziadavka na prevod textu na rec. Vsetko, co meni vysledok, je sucastou kluca cache.
/// </summary>
/// <param name="Text">text</param>
/// <param name="VoiceId">hlas</param>
/// <param name="ModelId">model</param>
public sealed record SpeechRequest(string Text, string VoiceId, string ModelId)
{
    /// <summary>Kod jazyka (ISO 639-1); prazdny = model ho urci sam.</summary>
    public string LanguageCode { get; init; } = "";

    /// <summary>Stabilita hlasu 0-1 - vyssia = jednotvarnejsi a stalejsi prejav.</summary>
    public double Stability { get; init; } = 0.5;

    /// <summary>Podobnost s povodnym hlasom 0-1.</summary>
    public double SimilarityBoost { get; init; } = 0.75;

    /// <summary>Zvyraznenie stylu hlasu 0-1.</summary>
    public double Style { get; init; }

    /// <summary>Rychlost reci (1 = normalna).</summary>
    public double Speed { get; init; } = 1;

    /// <summary>Zvyraznenie podobnosti s hovoriacim.</summary>
    public bool SpeakerBoost { get; init; } = true;

    /// <summary>Seed pre opakovatelny vysledok; <see langword="null" /> = nahodny.</summary>
    public uint? Seed { get; init; }

    /// <summary>Text pred uryvkom - ovplyvni intonaciu, nevyslovi sa.</summary>
    public string PreviousText { get; init; } = "";

    /// <summary>Text za uryvkom - ovplyvni intonaciu, nevyslovi sa.</summary>
    public string NextText { get; init; } = "";

    /// <summary>
    /// Kluc cache - SHA-256 zo sluzby a vsetkych hodnot poziadavky.
    /// </summary>
    public string CacheKey(string providerId)
    {
        var text = string.Join('\n', providerId, ModelId, VoiceId, LanguageCode,
            Stability.ToString("R", CultureInfo.InvariantCulture), SimilarityBoost.ToString("R", CultureInfo.InvariantCulture),
            Style.ToString("R", CultureInfo.InvariantCulture), Speed.ToString("R", CultureInfo.InvariantCulture),
            SpeakerBoost ? "1" : "0", Seed?.ToString(CultureInfo.InvariantCulture) ?? "-", PreviousText, NextText, Text);
        return Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(text)));
    }
}
