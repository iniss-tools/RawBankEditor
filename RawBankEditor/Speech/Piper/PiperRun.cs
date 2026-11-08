namespace RawBankEditor.Speech.Piper;

/// <summary>
/// Spustenie programu Piper.
/// </summary>
/// <param name="Executable">program</param>
/// <param name="Arguments">parametre</param>
/// <param name="Input">text na standardny vstup</param>
internal sealed record PiperRun(string Executable, IReadOnlyList<string> Arguments, string Input);
