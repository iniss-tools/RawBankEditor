namespace RawBankEditor.Speech.Piper;

/// <summary>
/// Vysledok programu Piper.
/// </summary>
/// <param name="ExitCode">navratovy kod</param>
/// <param name="Error">posledne riadky chyboveho vystupu</param>
internal sealed record PiperResult(int ExitCode, string Error);
