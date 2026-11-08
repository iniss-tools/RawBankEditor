namespace RawBankEditor.Speech;

/// <summary>
/// Strana vysledkov kniznice.
/// </summary>
/// <param name="Voices">hlasy</param>
/// <param name="HasMore">su dalsie strany</param>
public sealed record SharedVoicePage(IReadOnlyList<SharedVoice> Voices, bool HasMore);
