namespace RawBankEditor.Speech.Piper;

/// <summary>
/// Nainstalovany hlas Piper - model a jeho nastavenie (<c>.onnx.json</c>).
/// </summary>
/// <param name="Key">nazov hlasu (subor bez pripony)</param>
/// <param name="Family">jazyk (ISO 639-1)</param>
/// <param name="LanguageName">nazov jazyka</param>
/// <param name="Quality">kvalita (x_low, low, medium, high)</param>
/// <param name="Speakers">hovoriaci viachlasoveho modelu (nazov - cislo); prazdne = jeden hlas</param>
public sealed record InstalledPiperVoice(string Key, string Family, string LanguageName, string Quality, IReadOnlyDictionary<string, int> Speakers);
