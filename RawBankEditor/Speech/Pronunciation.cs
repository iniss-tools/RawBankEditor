using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

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

/// <summary>
/// Uplatnenie slovnika vyslovnosti na text pred odoslanim sluzbe syntezy.
/// </summary>
public static class Pronunciation
{
    private static readonly TimeSpan Timeout = TimeSpan.FromMilliseconds(250);

    /// <summary>
    /// Text po uplatneni pravidiel. Pravidlo s prazdnym alebo neplatnym vyrazom sa preskoci.
    /// </summary>
    public static string Apply(string text, IEnumerable<PronunciationRule>? rules)
    {
        ArgumentNullException.ThrowIfNull(text);
        if (rules is null)
            return text;

        foreach (var rule in rules)
        {
            if (PatternOf(rule) is not { } pattern)
                continue;

            try
            {
                text = pattern.Replace(text, rule.Regex ? rule.Replace : rule.Replace.Replace("$", "$$", StringComparison.Ordinal));
            }
            catch (RegexMatchTimeoutException)
            {
                // patologicky vyraz - text ostava
            }
        }

        return text;
    }

    /// <summary>
    /// Chyba pravidla (neplatny regularny vyraz); <see langword="null" />, ak je pravidlo v poriadku.
    /// </summary>
    public static string? Validate(PronunciationRule rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        if (!rule.Regex || rule.Find.Length == 0)
            return null;

        try
        {
            _ = new Regex(rule.Find, RegexOptions.None, Timeout);
            return null;
        }
        catch (ArgumentException e)
        {
            return e.Message;
        }
    }

    private static Regex? PatternOf(PronunciationRule rule)
    {
        if (rule.Find.Length == 0)
            return null;

        var pattern = rule.Regex ? rule.Find : System.Text.RegularExpressions.Regex.Escape(rule.Find);
        if (rule.WholeWord)
            pattern = $@"(?<![\p{{L}}\p{{N}}])(?:{pattern})(?![\p{{L}}\p{{N}}])";
        var options = RegexOptions.CultureInvariant | (rule.MatchCase ? RegexOptions.None : RegexOptions.IgnoreCase);
        try
        {
            return new Regex(pattern, options, Timeout);
        }
        catch (ArgumentException)
        {
            return null;
        }
    }
}
