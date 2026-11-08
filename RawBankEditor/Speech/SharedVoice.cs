namespace RawBankEditor.Speech;

/// <summary>
/// Hlas kniznice.
/// </summary>
/// <param name="OwnerId">verejny identifikator autora</param>
/// <param name="VoiceId">identifikator hlasu</param>
/// <param name="Name">nazov</param>
/// <param name="Gender">pohlavie</param>
/// <param name="Age">vek</param>
/// <param name="Accent">prizvuk</param>
/// <param name="Language">jazyk</param>
/// <param name="Description">popis autora</param>
/// <param name="PreviewUrl">adresa ukazky</param>
public sealed record SharedVoice(string OwnerId, string VoiceId, string Name, string Gender, string Age, string Accent, string Language,
    string Description, string PreviewUrl);
