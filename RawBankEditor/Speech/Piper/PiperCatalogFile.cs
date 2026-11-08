namespace RawBankEditor.Speech.Piper;

/// <summary>
/// Subor hlasu v zozname.
/// </summary>
/// <param name="Path">cesta v ulozisku hlasov</param>
/// <param name="FileName">nazov suboru v priecinku hlasov</param>
/// <param name="Size">velkost v bajtoch</param>
/// <param name="Md5">kontrolny sucet MD5 (hex)</param>
public sealed record PiperCatalogFile(string Path, string FileName, long Size, string Md5);
