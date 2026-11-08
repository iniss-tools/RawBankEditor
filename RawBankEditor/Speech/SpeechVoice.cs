namespace RawBankEditor.Speech;

/// <summary>
/// Hlas sluzby.
/// </summary>
/// <param name="Id">identifikator hlasu</param>
/// <param name="Name">nazov</param>
/// <param name="Description">popis (pohlavie, vek, prizvuk, kategoria)</param>
public sealed record SpeechVoice(string Id, string Name, string Description)
{
    /// <summary>Vlastny hlas uctu (klonovany, navrhnuty) - nie predvoleny hlas sluzby.</summary>
    public bool Own { get; init; }

    /// <summary>Druh hlasu podla sluzby (premade, cloned, generated, professional).</summary>
    public string Category { get; init; } = "";

    /// <summary>Adresa ukazky; prazdna = bez ukazky.</summary>
    public string PreviewUrl { get; init; } = "";

    /// <summary>Jazyky, ktore hlas hovori (ISO 639-1); prazdne = hlas sluzba neurcuje (viacjazycny).</summary>
    public IReadOnlyList<string> Languages { get; init; } = [];

    /// <summary>
    /// Hlas hovori jazyk; hlas bez urcenych jazykov hovori kazdy.
    /// </summary>
    /// <param name="languageCode">kod jazyka (ISO 639-1); prazdny = kazdy hlas</param>
    public bool Speaks(string languageCode) =>
        languageCode.Length == 0 || Languages.Count == 0 || Languages.Contains(languageCode, StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    public override string ToString() => Description.Length == 0 ? Name : $"{Name} ({Description})";
}
