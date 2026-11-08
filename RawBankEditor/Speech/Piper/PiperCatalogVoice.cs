namespace RawBankEditor.Speech.Piper;

/// <summary>
/// Hlas Piper v zozname hlasov na stiahnutie.
/// </summary>
/// <param name="Key">nazov hlasu (napr. <c>sk_SK-lili-medium</c>)</param>
/// <param name="Family">jazyk (ISO 639-1)</param>
/// <param name="LanguageName">nazov jazyka</param>
/// <param name="Quality">kvalita (x_low, low, medium, high)</param>
/// <param name="Speakers">pocet hovoriacich</param>
/// <param name="Files">subory modelu a nastavenia</param>
public sealed record PiperCatalogVoice(string Key, string Family, string LanguageName, string Quality, int Speakers, IReadOnlyList<PiperCatalogFile> Files)
{
    /// <summary>Velkost na stiahnutie v bajtoch.</summary>
    public long Size => Files.Sum(f => f.Size);
}
