namespace RawBankEditor.Speech;

/// <summary>
/// Text uryvku na generovanie s intonaciou jeho skupiny.
/// </summary>
/// <param name="Text">text na vyslovenie (bez interpunkcie skupiny)</param>
/// <param name="Prosody">intonacia; <see langword="null" /> = uryvok samostatne</param>
public sealed record SpeechInput(string Text, GroupProsody? Prosody = null);
