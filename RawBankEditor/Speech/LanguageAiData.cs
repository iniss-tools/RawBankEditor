using System.Text.Json.Serialization;

namespace RawBankEditor.Speech;

/// <summary>
/// Udaje AI jedneho jazyka banky v <c>&lt;INISS&gt;\.rbe-ai\&lt;kluc jazyka&gt;.json</c> - mimo RAWBANK, INISS ich
/// necita a prieskumnik banky ich nevidi.
/// </summary>
public sealed class LanguageAiData
{
    /// <summary>Verzia formatu.</summary>
    [JsonPropertyName("version")]
    public int Version { get; set; } = 1;

    /// <summary>Hlas jazyka; <see langword="null" />, kym ho pouzivatel nevyberie.</summary>
    [JsonPropertyName("voice")]
    public VoiceProfile? Voice { get; set; }

    /// <summary>Povod nahravok podla kluca skupiny a zvuku (<see cref="SoundRecords.KeyOf" />) - uklada sa s bankou.</summary>
    [JsonPropertyName("sounds")]
    public Dictionary<string, SoundOrigin> Sounds { get; set; } = [];

    /// <summary>Texty pre syntezu podla kluca skupiny a zvuku - ukladaju sa s bankou.</summary>
    [JsonPropertyName("texts")]
    public Dictionary<string, string> Texts { get; set; } = [];

    /// <summary>Intonacia skupin podla kluca skupiny - len skupiny s nastavenim inym nez predvolenym.</summary>
    [JsonPropertyName("prosody")]
    public Dictionary<string, GroupProsody> Prosody { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Slovnik vyslovnosti jazyka.</summary>
    [JsonPropertyName("pronunciation")]
    public List<PronunciationRule> Pronunciation { get; set; } = [];

    /// <summary>Cisla citat slovami (<see cref="NumberWords" />) - predvolene ano.</summary>
    [JsonPropertyName("numbersAsWords")]
    public bool NumbersAsWords { get; set; } = true;

    /// <summary>
    /// Jazyk prevodu cisel na slova pre generator; prazdny, ak je prevod vypnuty alebo jazyk ho nema.
    /// </summary>
    /// <param name="languageCode">jazyk reci (ISO 639-1)</param>
    public string NumbersLanguage(string languageCode) => NumbersAsWords && NumberWords.Supports(languageCode) ? languageCode : "";
}
