namespace RawBankEditor.Speech;

/// <summary>
/// Cas znaku textu v nahravke.
/// </summary>
/// <param name="Character">znak</param>
/// <param name="Start">zaciatok v sekundach</param>
/// <param name="End">koniec v sekundach</param>
public sealed record CharTiming(char Character, double Start, double End);
