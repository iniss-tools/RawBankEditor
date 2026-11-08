using System.Globalization;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace RawBankEditor.Speech;

/// <summary>
/// Hlas jazyka banky - sluzba, model, hlas a jeho nastavenia, ktorymi sa generuju nahravky jazyka.
/// </summary>
public sealed class VoiceProfile
{
    /// <summary>Sluzba syntezy (<see cref="ISpeechProvider.Id" />).</summary>
    [JsonPropertyName("provider")]
    public string Provider { get; set; } = ElevenLabsClient.ProviderId;

    /// <summary>Model.</summary>
    [JsonPropertyName("modelId")]
    public string ModelId { get; set; } = ElevenLabsClient.DefaultModel;

    /// <summary>Hlas.</summary>
    [JsonPropertyName("voiceId")]
    public string VoiceId { get; set; } = "";

    /// <summary>Nazov hlasu - len na zobrazenie.</summary>
    [JsonPropertyName("voiceName")]
    public string VoiceName { get; set; } = "";

    /// <summary>Kod jazyka (ISO 639-1); prazdny = model ho urci sam.</summary>
    [JsonPropertyName("languageCode")]
    public string LanguageCode { get; set; } = "";

    /// <summary>Stabilita 0-1.</summary>
    [JsonPropertyName("stability")]
    public double Stability { get; set; } = 0.5;

    /// <summary>Podobnost s hlasom 0-1.</summary>
    [JsonPropertyName("similarityBoost")]
    public double SimilarityBoost { get; set; } = 0.75;

    /// <summary>Zvyraznenie stylu 0-1.</summary>
    [JsonPropertyName("style")]
    public double Style { get; set; }

    /// <summary>Rychlost reci.</summary>
    [JsonPropertyName("speed")]
    public double Speed { get; set; } = 1;

    /// <summary>Zvyraznenie podobnosti s hovoriacim.</summary>
    [JsonPropertyName("speakerBoost")]
    public bool SpeakerBoost { get; set; } = true;

    /// <summary>Pevny seed - rovnaky text da (takmer) rovnaku nahravku; <see langword="null" /> = nahodny.</summary>
    [JsonPropertyName("seed")]
    public uint? Seed { get; set; }

    /// <summary>
    /// Hlas je vybrany - da sa generovat.
    /// </summary>
    [JsonIgnore]
    public bool IsComplete => VoiceId.Length > 0 && ModelId.Length > 0;

    /// <summary>
    /// Poziadavka na prevod textu.
    /// </summary>
    /// <param name="text">text</param>
    /// <param name="seed">seed namiesto <see cref="Seed" /> (generovanie znova)</param>
    public SpeechRequest ToRequest(string text, uint? seed = null) => new(text, VoiceId, ModelId)
    {
        LanguageCode = LanguageCode,
        Stability = Stability,
        SimilarityBoost = SimilarityBoost,
        Style = Style,
        Speed = Speed,
        SpeakerBoost = SpeakerBoost,
        Seed = seed ?? Seed
    };

    /// <summary>Kopia.</summary>
    public VoiceProfile Clone() => (VoiceProfile)MemberwiseClone();

    /// <summary>
    /// Odtlacok hlasu a nastaveni, ktore menia nahravku (bez seedu) - rovnaky odtlacok a text = rovnaky prejav.
    /// </summary>
    public string Fingerprint()
    {
        var text = string.Join('|', Provider, ModelId, VoiceId, LanguageCode, Stability.ToString("R", CultureInfo.InvariantCulture),
            SimilarityBoost.ToString("R", CultureInfo.InvariantCulture), Style.ToString("R", CultureInfo.InvariantCulture),
            Speed.ToString("R", CultureInfo.InvariantCulture), SpeakerBoost ? "1" : "0");
        return Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(text)))[..16];
    }

    /// <summary>
    /// Kod jazyka (ISO 639-1) pre kluc jazyka banky - SK, CZ, GB, D; neznamy kluc = prazdny (model ho urci sam).
    /// </summary>
    public static string LanguageCodeOf(string bankLanguageKey) => bankLanguageKey.ToUpperInvariant() switch
    {
        "SK" => "sk",
        "CZ" => "cs",
        "GB" or "EN" => "en",
        "D" or "DE" => "de",
        "PL" => "pl",
        "H" or "HU" => "hu",
        _ => ""
    };
}
