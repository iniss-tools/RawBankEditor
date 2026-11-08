namespace RawBankEditor.Services;

/// <summary>
/// Vysledok ulozenia banky.
/// </summary>
/// <param name="Error">chyba zapisu (zmeny ostali neulozene); <see langword="null" />, ak sa banka ulozila</param>
/// <param name="RecycleErrors">polozky, ktore sa po ulozeni nepodarilo presunut do Kosa</param>
internal sealed record BankSaveResult(string? Error, IReadOnlyList<string> RecycleErrors)
{
    /// <summary>
    /// Banka sa ulozila.
    /// </summary>
    public bool Saved => Error is null;

    /// <summary>
    /// Neuspesne ulozenie.
    /// </summary>
    public static BankSaveResult Failed(string error) => new(error, []);
}
