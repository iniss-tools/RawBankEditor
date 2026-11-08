using System.Text.Json.Serialization;

namespace RawBankEditor.Speech;

/// <summary>
/// Pravidlo vyslovnosti - text, ktory sa pred odoslanim sluzbe nahradi inym (skratky, nazvy staníc s nezvyklou
/// vyslovnostou). Pravidla sa uplatnia v poradi, kazde na vysledok predchadzajuceho.
/// </summary>
public sealed class PronunciationRule
{
    /// <summary>Hladany text alebo regularny vyraz.</summary>
    [JsonPropertyName("find")]
    public string Find { get; set; } = "";

    /// <summary>Nahrada (pri regularnom vyraze moze obsahovat $1 ...).</summary>
    [JsonPropertyName("replace")]
    public string Replace { get; set; } = "";

    /// <summary>Len cele slovo.</summary>
    [JsonPropertyName("wholeWord")]
    public bool WholeWord { get; set; } = true;

    /// <summary>Rozlisovat velke a male pismena.</summary>
    [JsonPropertyName("matchCase")]
    public bool MatchCase { get; set; }

    /// <summary><see cref="Find" /> je regularny vyraz.</summary>
    [JsonPropertyName("regex")]
    public bool Regex { get; set; }

    /// <summary>Kopia.</summary>
    public PronunciationRule Clone() => (PronunciationRule)MemberwiseClone();
}
