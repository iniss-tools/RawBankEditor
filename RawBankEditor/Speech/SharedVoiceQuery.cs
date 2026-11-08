namespace RawBankEditor.Speech;

/// <summary>
/// Filter kniznice hlasov.
/// </summary>
/// <param name="Language">kod jazyka (ISO 639-1); prazdny = vsetky</param>
/// <param name="Gender">pohlavie (male, female, neutral); prazdne = vsetky</param>
/// <param name="Search">hladany text</param>
/// <param name="Page">strana od 0</param>
public sealed record SharedVoiceQuery(string Language, string Gender, string Search, int Page);
