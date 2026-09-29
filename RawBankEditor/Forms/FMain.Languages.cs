using System.Globalization;
using RawBankEditor.Properties;
using RawBankEditor.Tools;
using ToolsCore.Iniss.Entities;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

partial class FMain
{
    /// <summary>
    /// Zmeni kluc, nazov a relativnu cestu jazyka. Pri zmene cesty premenuje priecinok jazyka, ak existuje - inak by
    /// sa FYZZVUK.DAT pri ulozeni zapisal do noveho prazdneho priecinka a skupiny s nahravkami by ostali v starom.
    /// </summary>
    /// <returns><see langword="false" />, ak sa priecinok nepodarilo premenovat - jazyk ostal bez zmeny.</returns>
    internal bool ChangeLanguage(FyzLanguage language, string key, string name, string relativePath)
    {
        if (relativePath != language.RelativePath && !MoveLanguageDirectory(language, relativePath))
            return false;

        language.Key = key;
        language.Name = name;
        RefreshLanguage(language);
        CheckProjectState();
        return true;
    }

    /// <summary>
    /// Vytvori chybajuci priecinok jazyka a prepoji ho s jazykom v prieskumniku (zoznam chyb - Vyriesit).
    /// </summary>
    internal void CreateLanguageDirectory(FyzLanguage language)
    {
        var path = Path.TrimEndingDirectorySeparator(language.GetAbsPath(GlobData.OpenedProject!.AbsPathToBank));
        try
        {
            WithoutFileWatcher(() => Directory.CreateDirectory(path));
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
        {
            Utils.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.FMain_LanguageDirFailed, path, ex.Message));
            return;
        }

        var folder = LanguageRules.FolderName(language.RelativePath);
        var element = Root.Children.OfType<DirectoryElement>().FirstOrDefault(d => RawBankExplorer.EqualsPathNames(d.Name, folder));
        if (element is null)
        {
            element = new DirectoryElement(path) { Parent = Root };
            Root.Children.Add(element);
        }

        language.Directory = element;
        if (language.Groups is not null)
            foreach (var group in language.Groups)
                LinkGroupDirectory(group);

        if (ReferenceEquals(language, CurrentLanguage))
            FillExplorerList(CurrentGroup?.Directory ?? element);
    }

    private bool MoveLanguageDirectory(FyzLanguage language, string relativePath)
    {
        var pathToBank = GlobData.OpenedProject!.AbsPathToBank;
        var oldRelativePath = language.RelativePath;
        var oldPath = Path.TrimEndingDirectorySeparator(language.GetAbsPath(pathToBank));
        language.RelativePath = relativePath;
        var newPath = Path.TrimEndingDirectorySeparator(language.GetAbsPath(pathToBank));
        // len zmena velkosti pismen - na disku je to ten isty priecinok
        var sameFolder = string.Equals(oldPath, newPath, StringComparison.OrdinalIgnoreCase);

        if (Directory.Exists(oldPath))
        {
            string? error = null;
            if (!sameFolder && Directory.Exists(newPath))
            {
                error = string.Format(CultureInfo.CurrentCulture, Resources.Groups_DirExists, newPath);
            }
            else
            {
                try
                {
                    WithoutFileWatcher(() => Directory.Move(oldPath, newPath));
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                {
                    error = ex.Message;
                }
            }

            if (error is not null)
            {
                language.RelativePath = oldRelativePath;
                Utils.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.Languages_RenameFailed, oldPath, error));
                return false;
            }

            if (language.Directory is not null)
                SetElementPath(language.Directory, newPath);
        }
        else
        {
            // jazyk bez priecinka sa prepoji s priecinkom novej cesty, ak existuje
            var folder = LanguageRules.FolderName(relativePath);
            language.Directory = Root?.Children.OfType<DirectoryElement>()
                .FirstOrDefault(d => RawBankExplorer.EqualsPathNames(d.Name, folder))!;
        }

        if (language.Groups is not null)
        {
            foreach (var group in language.Groups)
            {
                LinkGroupDirectory(group);
                foreach (var sound in group.Sounds)
                    RelinkSoundFile(sound);
            }
        }

        if (ReferenceEquals(language, CurrentLanguage))
            FillExplorerList(CurrentGroup?.Directory ?? language.Directory);
        return true;
    }
}
