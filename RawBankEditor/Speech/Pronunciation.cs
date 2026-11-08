using System.Text.RegularExpressions;

namespace RawBankEditor.Speech;

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
