namespace RawBankEditor.Speech;

/// <summary>
/// Klonovanie hlasu z nahravok.
/// </summary>
/// <param name="Name">nazov hlasu</param>
/// <param name="Description">popis</param>
/// <param name="Samples">nahravky (WAV)</param>
/// <param name="RemoveBackgroundNoise">odstranit sum zo vzoriek</param>
/// <param name="LanguageCode">jazyk hlasu (stitok); prazdny = bez stitku</param>
public sealed record VoiceCloneRequest(string Name, string Description, IReadOnlyList<byte[]> Samples, bool RemoveBackgroundNoise, string LanguageCode);
