using System.Text.Json.Serialization;

namespace RawBankEditor.Speech;

/// <summary>
/// Intonacia skupiny zvukov - sposob, text pred a za uryvkom (kontext alebo nosna veta) a interpunkcia.
/// </summary>
public sealed class GroupProsody
{
    /// <summary>Sposob.</summary>
    [JsonPropertyName("technique")]
    public ProsodyTechnique Technique { get; set; }

    /// <summary>Text pred uryvkom.</summary>
    [JsonPropertyName("before")]
    public string Before { get; set; } = "";

    /// <summary>Text za uryvkom.</summary>
    [JsonPropertyName("after")]
    public string After { get; set; } = "";

    /// <summary>Interpunkcia za uryvkom.</summary>
    [JsonPropertyName("punctuation")]
    public ProsodyPunctuation Punctuation { get; set; }

    /// <summary>Kopia.</summary>
    public GroupProsody Clone() => (GroupProsody)MemberwiseClone();

    /// <summary>
    /// Rovnake nastavenie.
    /// </summary>
    public bool SameAs(GroupProsody other) =>
        Technique == other.Technique && Before == other.Before && After == other.After && Punctuation == other.Punctuation;

    /// <summary>
    /// Text uryvku s interpunkciou - ak uz interpunkciu ma, neprida sa.
    /// </summary>
    public string Spoken(string text)
    {
        text = text.Trim();
        if (text.Length == 0 || Punctuation == ProsodyPunctuation.None || char.IsPunctuation(text[^1]))
            return text;
        return text + (Punctuation == ProsodyPunctuation.Comma ? "," : ".");
    }
}
