using System.Xml;
using RawBankEditor.Properties;
using RawBankEditor.Services;
using ToolsCore.Iniss.Entities;
using ToolsCore.Iniss.Tools;
using ToolsCore.Iniss2.RawBank;

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
            return Resources.Rules_AllFieldsRequired;

        if (!relativePath.EndsWith('\\'))
            return Resources.Rules_PathMustEndWithBackslash;

        if (!IsValidRelativePath(relativePath))
            return Resources.LanguageRules_PathSingleDir;

        foreach (var lang in languages)
        {
            if (ReferenceEquals(lang, edited))
                continue;

            if (string.Equals(lang.Key, key, StringComparison.OrdinalIgnoreCase))
                return Resources.LanguageRules_KeyExists;

            if (string.Equals(lang.Name, name, StringComparison.OrdinalIgnoreCase))
                return Resources.LanguageRules_NameExists;

            if (string.Equals(lang.RelativePath, relativePath, StringComparison.OrdinalIgnoreCase))
                return Resources.LanguageRules_PathExists;
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
    /// Jazyky lezia v tom istom priecinku banky.
    /// </summary>
    public static bool SameFolder(FyzLanguage a, FyzLanguage b) =>
        string.Equals(FolderName(a.RelativePath), FolderName(b.RelativePath), StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Jazyk banky INISS2 - nazvy skupin a zvukov sa neukladaju, nazov je vzdy kluc.
    /// </summary>
    public static bool NameIsKey(FyzLanguage language) => string.Equals(language.FileDefName, FyzBankXmlWriter.FileName, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Server INISS2 banku s viacerymi jazykmi bez predvoleneho jazyka nenacita.
    /// </summary>
    /// <returns>Text chyby alebo <see langword="null" />, ak je predvoleny jazyk v poriadku.</returns>
    public static string? DefaultLanguageProblem(IReadOnlyCollection<FyzLanguage> languages) =>
        languages.Count > 1 && languages.Count(l => l.IsBasic) != 1 ? Resources.LanguageRules_DefaultRequired : null;

    /// <summary>
    /// Cesta k suboru so zvukmi jazyka (FYZZVUK.DAT, v INISS2 FyzBank.xml).
    /// </summary>
    public static string SoundsFile(string pathToBank, FyzLanguage language)
        => PathUtils.CombinePath(pathToBank, language.RelativePath, language.FileDefName)!;

    /// <summary>
    /// Zisti, ci sa skupiny a zvuky jazyka v pamati lisia od jeho suboru so zvukmi. Nenacitany jazyk
    /// (<see cref="FyzLanguage.Groups" /> je <see langword="null" />) sa nelisi; jazyk bez suboru sa lisi, len ak ma skupiny.
    /// </summary>
    /// <param name="pathToBank">priecinok banky</param>
    /// <param name="language">jazyk</param>
    /// <param name="store">sposob ulozenia banky; predvolene INISS 3.39</param>
    public static bool SoundsDiffer(string pathToBank, FyzLanguage language, IBankStore? store = null)
    {
        if (language.Groups is null)
            return false;

        if (!File.Exists(SoundsFile(pathToBank, language)))
            return language.Groups.Count != 0;

        var saved = new FyzLanguage(language.Key, language.Name, language.FileDefName, language.RelativePath);
        try
        {
            (store ?? BankDatStore.Instance).ReadSounds(pathToBank, saved);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException or XmlException)
        {
            return true;
        }

        return !SameGroups(language.Groups, saved.Groups);
    }

    /// <summary>
    /// Zisti, ci sa zoznam jazykov v pamati lisi od banky na disku.
    /// </summary>
    /// <param name="pathToBank">priecinok banky</param>
    /// <param name="languages">jazyky v pamati</param>
    /// <param name="store">sposob ulozenia banky; predvolene INISS 3.39</param>
    public static bool BankDiffers(string pathToBank, IReadOnlyList<FyzLanguage> languages, IBankStore? store = null)
    {
        store ??= BankDatStore.Instance;

        List<FyzLanguage> saved;
        try
        {
            saved = store.ReadLanguages(pathToBank);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException or XmlException)
        {
            return true;
        }

        if (saved.Count != languages.Count)
            return true;

        // INISS2 zoznam jazykov nema - z disku pridu zoradene podla priecinkov
        var current = store.IsIniss2 ? languages.OrderBy(l => FolderName(l.RelativePath), StringComparer.OrdinalIgnoreCase).ToList() : languages;
        for (var i = 0; i < saved.Count; i++)
        {
            var a = saved[i];
            var b = current[i];
            if (a.Key != b.Key || a.Name != b.Name || a.RelativePath != b.RelativePath || a.FileDefName != b.FileDefName || a.IsBasic != b.IsBasic)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Skupiny a zvuky jazykov su rovnake (nazvy, cesty, dlzky a MD5 nahravok).
    /// </summary>
    public static bool SameGroups(IList<FyzGroup> a, IList<FyzGroup> b)
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
                if (sa.Key != sb.Key || sa.Name != sb.Name || sa.Text != sb.Text || sa.Duration != sb.Duration || sa.Md5 != sb.Md5
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
