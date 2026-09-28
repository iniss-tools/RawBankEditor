using ToolsCore.Entities;
using ToolsCore.Tools;

namespace RawBankEditor.Tools;

/// <summary>
/// Pravidla pre jazyky banky: kontrola okna Pridat/Upravit jazyk a porovnanie jazykov v pamati so subormi na disku.
/// </summary>
public static class LanguageRules
{
    /// <summary>
    /// Relativna cesta, ktoru okno jazyka navrhne podla kluca (napr. <c>SK</c> -> <c>SK\</c>).
    /// </summary>
    public static string DefaultRelativePath(string key)
    {
        key = key.Trim();
        return key.Length == 0 ? "" : key + '\\';
    }

    /// <summary>
    /// Skontroluje kluc, nazov a relativnu cestu jazyka voci ostatnym jazykom banky.
    /// </summary>
    /// <param name="languages">Jazyky banky.</param>
    /// <param name="edited">Upravovany jazyk (s nim sa neporovnava), pri pridani <see langword="null" />.</param>
    /// <returns>Text chyby alebo <see langword="null" />, ak su hodnoty v poriadku.</returns>
    public static string? Validate(IEnumerable<FyzLanguage> languages, FyzLanguage? edited, string key, string name, string relativePath)
    {
        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(relativePath))
            return "Nie sú vyplnené všetky polia.";

        if (!relativePath.EndsWith('\\'))
            return "Relatívna cesta musí končiť '\\'.";

        if (!IsValidRelativePath(relativePath))
            return "Relatívna cesta musí byť názov jedného priečinka v priečinku RAWBANK, napr. SK\\.";

        foreach (var lang in languages)
        {
            if (ReferenceEquals(lang, edited))
                continue;

            if (string.Equals(lang.Key, key, StringComparison.OrdinalIgnoreCase))
                return "Jazyk s rovnakým kľúčom už existuje.";

            if (string.Equals(lang.Name, name, StringComparison.OrdinalIgnoreCase))
                return "Jazyk s rovnakým názvom už existuje.";

            if (string.Equals(lang.RelativePath, relativePath, StringComparison.OrdinalIgnoreCase))
                return "Jazyk s rovnakou relatívnou cestou už existuje.";
        }

        return null;
    }

    // prieskumnik aj nacitanie banky paruju jazyk s priecinkom v RAWBANK podla nazvu - vnorene cesty by sa nenasli
    private static bool IsValidRelativePath(string relativePath)
    {
        var folder = relativePath[..^1];
        return folder.Trim().Length != 0
               && folder is not ("." or "..")
               && folder.IndexOfAny(Path.GetInvalidFileNameChars()) < 0;
    }

    /// <summary>
    /// Nazov priecinka jazyka v RAWBANK (relativna cesta bez koncovej lomky).
    /// </summary>
    public static string FolderName(string relativePath) => relativePath.TrimEnd('\\');

    /// <summary>
    /// Cesta k suboru so zvukmi jazyka (FYZZVUK.DAT).
    /// </summary>
    public static string SoundsFile(string pathToBank, FyzLanguage language)
        => Utils.CombinePath(pathToBank, language.RelativePath, language.FileDefName)!;

    /// <summary>
    /// Zisti, ci sa skupiny a zvuky jazyka v pamati lisia od jeho suboru FYZZVUK.DAT. Nenacitany jazyk
    /// (<see cref="FyzLanguage.Groups" /> je <see langword="null" />) sa nelisi; jazyk bez suboru sa lisi, len ak ma skupiny.
    /// </summary>
    public static bool SoundsDiffer(string pathToBank, FyzLanguage language)
    {
        if (language.Groups is null)
            return false;

        if (!File.Exists(SoundsFile(pathToBank, language)))
            return language.Groups.Count != 0;

        var saved = new FyzLanguage(language.Key, language.Name, language.FileDefName, language.RelativePath);
        try
        {
            RawBankParser.ReadFyzZvukFile(pathToBank, saved);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            return true;
        }

        return !SameGroups(language.Groups, saved.Groups);
    }

    /// <summary>
    /// Zisti, ci sa zoznam jazykov v pamati lisi od suboru FYZBANK.DAT.
    /// </summary>
    public static bool BankDiffers(string pathToBank, IReadOnlyList<FyzLanguage> languages)
    {
        List<FyzLanguage> saved;
        try
        {
            saved = RawBankParser.ReadFyzBankFile(pathToBank, out _);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            return true;
        }

        if (saved.Count != languages.Count)
            return true;

        for (var i = 0; i < saved.Count; i++)
        {
            var a = saved[i];
            var b = languages[i];
            if (a.Key != b.Key || a.Name != b.Name || a.RelativePath != b.RelativePath || a.FileDefName != b.FileDefName)
                return true;
        }

        return false;
    }

    private static bool SameGroups(IList<FyzGroup> a, IList<FyzGroup> b)
    {
        if (a.Count != b.Count)
            return false;

        for (var i = 0; i < a.Count; i++)
        {
            var ga = a[i];
            var gb = b[i];
            if (ga.Key != gb.Key || ga.Name != gb.Name || ga.RelativePath != gb.RelativePath || ga.Sounds.Count != gb.Sounds.Count)
                return false;

            for (var j = 0; j < ga.Sounds.Count; j++)
            {
                var sa = ga.Sounds[j];
                var sb = gb.Sounds[j];
                if (sa.Key != sb.Key || sa.Name != sb.Name || sa.Text != sb.Text || sa.Duration != sb.Duration
                    || StoredPath(sa) != StoredPath(sb))
                    return false;
            }
        }

        return true;
    }

    // cesta k suboru zvuku tak, ako ju zapise RawBankParser.WriteFyzZvukFile
    private static string StoredPath(FyzSound sound)
        => string.IsNullOrEmpty(sound.AdditionalRelativePath) ? sound.FileName : sound.AdditionalRelativePath + sound.FileName;
}
