using RawBankEditor.Properties;
using ToolsCore.Iniss.Tools;

namespace RawBankEditor.Tools;

/// <summary>
/// Typ stlpca tabulky, z ktorej sa importuju zvuky.
/// </summary>
public sealed class SoundImportColumn : Enumeration<SoundImportColumn>
{
    private SoundImportColumn(int id, string name, params string[] aliases) : base(id, name) => Aliases = aliases;

    // nazvy hlaviciek stlpcov rozpoznane automaticky - bez diakritiky, malymi pismenami (aj stare nazvy z XLS2FyzZvuk)
    private string[] Aliases { get; }

    public static readonly SoundImportColumn None = new(0, Resources.ImportColumn_None);

    public static readonly SoundImportColumn GroupKey = new(1, Resources.ImportColumn_GroupKey,
        "kluc skupiny", "klic skupiny", "skupina", "group", "group key", "priecinok", "nazov priecinka", "slozka", "nazev slozky", "directory", "dir");

    public static readonly SoundImportColumn GroupName = new(2, Resources.ImportColumn_GroupName,
        "nazov skupiny", "nazev skupiny", "group name", "groupname", "skupina zvukov");

    public static readonly SoundImportColumn GroupPath = new(3, Resources.ImportColumn_GroupPath,
        "cesta", "cesta skupiny", "relativna cesta", "relativni cesta", "relative path", "path");

    public static readonly SoundImportColumn SoundKey = new(4, Resources.ImportColumn_SoundKey,
        "kluc", "klic", "kluc zvuku", "klic zvuku", "key", "sound key", "zvuk", "sound", "soundname", "nazov zvuku pre", "nazev zvuku pro");

    public static readonly SoundImportColumn SoundName = new(5, Resources.ImportColumn_SoundName,
        "nazov", "nazev", "nazov zvuku", "nazev zvuku", "name", "sound name");

    public static readonly SoundImportColumn FileName = new(6, Resources.ImportColumn_FileName,
        "subor", "soubor", "nazov suboru", "nazev souboru", "file", "file name", "filename");

    public static readonly SoundImportColumn Text = new(7, Resources.ImportColumn_Text,
        "text", "text hlasenia", "text hlaseni", "announcement text");

    /// <summary>
    /// Typy stlpcov, ktore sa daju vybrat (bez <see cref="None" />), v poradi ponuky.
    /// </summary>
    public static IReadOnlyList<SoundImportColumn> Values => [GroupKey, GroupName, GroupPath, SoundKey, SoundName, FileName, Text];

    /// <summary>
    /// Povinne stlpce - ostatne hodnoty sa doplnia podla klucov.
    /// </summary>
    public static IReadOnlyList<SoundImportColumn> Required => [GroupKey, SoundKey];

    /// <summary>
    /// Typ stlpca podla textu hlavicky; <see cref="None" />, ak sa nerozpozna.
    /// </summary>
    public static SoundImportColumn ParseHeader(string? header)
    {
        var text = Normalize(header);
        if (text.Length == 0)
            return None;

        return Values.FirstOrDefault(c => Normalize(c.Name) == text || c.Aliases.Contains(text)) ?? None;
    }

    private static string Normalize(string? text)
        => StringUtils.RemoveDiacritics(text ?? "").Replace('_', ' ').Replace("-", "", StringComparison.Ordinal).Trim().ToLowerInvariant();
}
