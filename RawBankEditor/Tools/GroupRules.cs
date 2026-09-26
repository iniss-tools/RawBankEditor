using ToolsCore.Entities;

namespace RawBankEditor.Tools;

/// <summary>
///     Pravidla pre skupiny zvukov: kontrola okna Pridat/Upravit skupinu.
/// </summary>
public static class GroupRules
{
    /// <summary>
    ///     Relativna cesta, ktoru okno skupiny navrhne podla kluca (napr. <c>Slova</c> -> <c>Slova\</c>).
    /// </summary>
    public static string DefaultRelativePath(string key)
    {
        key = key.Trim();
        return key.Length == 0 ? "" : key + '\\';
    }

    /// <summary>
    ///     Nazov priecinka skupiny v priecinku jazyka (relativna cesta bez koncovej lomky).
    /// </summary>
    public static string FolderName(string relativePath) => relativePath.TrimEnd('\\');

    /// <summary>
    ///     Skontroluje kluc, nazov a relativnu cestu skupiny voci ostatnym skupinam jazyka. Kluce a nazvy sa porovnavaju
    ///     bez ohladu na velkost pismen - INISS hlada skupinu podla kluca rovnako.
    /// </summary>
    /// <param name="groups">Skupiny jazyka.</param>
    /// <param name="edited">Upravovana skupina (s nou sa neporovnava), pri pridani <see langword="null" />.</param>
    /// <returns>Text chyby alebo <see langword="null" />, ak su hodnoty v poriadku.</returns>
    public static string? Validate(IEnumerable<FyzGroup> groups, FyzGroup? edited, string key, string name, string relativePath)
    {
        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(relativePath))
            return "Nie sú vyplnené všetky polia.";

        if (!relativePath.EndsWith('\\'))
            return "Relatívna cesta musí končiť '\\'.";

        if (!IsValidRelativePath(relativePath))
            return "Relatívna cesta musí byť názov jedného priečinka v priečinku jazyka, napr. Slova\\.";

        foreach (var grp in groups)
        {
            if (ReferenceEquals(grp, edited))
                continue;

            if (SoundRules.SameText(grp.Key, key))
                return "Skupina s rovnakým kľúčom už existuje.";

            if (SoundRules.SameText(grp.Name, name))
                return "Skupina s rovnakým názvom už existuje.";

            if (SoundRules.SameText(grp.RelativePath, relativePath))
                return "Skupina s rovnakou relatívnou cestou už existuje.";
        }

        return null;
    }

    // prieskumnik aj nacitanie banky paruju skupinu s priecinkom jazyka podla nazvu - vnorene cesty by sa nenasli
    private static bool IsValidRelativePath(string relativePath)
    {
        var folder = relativePath[..^1];
        return folder.Trim().Length != 0
               && folder is not ("." or "..")
               && folder.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
    }
}
