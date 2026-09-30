using System.Globalization;
using ExControls;
using RawBankEditor.Properties;
using RawBankEditor.Tools;
using ToolsCore.Iniss.Entities;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

partial class FMain
{
    private void DoAddGroup()
    {
        var language = CurrentLanguage!;
        var form = new FAddEditGroup(language.Groups);
        if (form.ShowDialog(this) != DialogResult.OK)
            return;

        var group = new FyzGroup(language, form.GroupKey, form.GroupName, form.GroupRelativePath);
        var directory = GroupDirectoryPath(group);
        var created = false;
        try
        {
            WithoutFileWatcher(() => created = _bank.Journal.CreateDirectory(directory, language));
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
        {
            _dialogs.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.Groups_DirFailed, directory, ex.Message));
            return;
        }

        var index = language.Groups.Count;
        InsertGroup(group, index);
        RegisterNewAction(new AddGroupAction(this, group, index, created ? directory : null));
    }

    private void DoEditGroup()
    {
        if (dgvGroups.IsSelectionEmpty())
            return;

        var group = (FyzGroup)dgvGroups.SelectedRows[0].DataBoundItem!;
        var form = new FAddEditGroup(group.Language.Groups, group);
        if (form.ShowDialog(this) != DialogResult.OK)
            return;

        if (form.GroupKey == group.Key && form.GroupName == group.Name && form.GroupRelativePath == group.RelativePath)
            return;

        var action = new EditGroupAction(this, group,
            (group.Key, form.GroupKey), (group.Name, form.GroupName), (group.RelativePath, form.GroupRelativePath));
        if (ChangeGroup(group, form.GroupKey, form.GroupName, form.GroupRelativePath))
            RegisterNewAction(action);
    }

    private void DoDeleteGroup()
    {
        if (dgvGroups.IsSelectionEmpty())
            return;

        var group = (FyzGroup)dgvGroups.SelectedRows[0].DataBoundItem!;
        var result = _dialogs.ShowWarning(
            string.Format(CultureInfo.CurrentCulture, Resources.Groups_DeleteConfirm, group.Name, group.Sounds.Count),
            MessageBoxButtons.YesNoCancel);
        if (result == DialogResult.Cancel)
            return;

        RemoveGroup(group, result == DialogResult.Yes);
    }

    /// <summary>
    /// Odstrani skupinu zo zoznamu a zaregistruje akciu spat.
    /// </summary>
    /// <param name="group">Odstranovana skupina.</param>
    /// <param name="withDirectory">Ci sa ma odstranit aj priecinok skupiny (do kosa pojde pri ulozeni).</param>
    internal void RemoveGroup(FyzGroup group, bool withDirectory)
    {
        var action = new RemovedGroupsAction(this, group, withDirectory && Directory.Exists(GroupDirectoryPath(group)));
        if (action.Apply())
            RegisterNewAction(action);
    }

    /// <summary>
    /// Absolutna cesta k priecinku skupiny bez koncovej lomky.
    /// </summary>
    internal string GroupDirectoryPath(FyzGroup group)
        => Path.TrimEndingDirectorySeparator(group.GetAbsPath(_bank.PathToBank));

    /// <summary>
    /// Vytvori chybajuci priecinok skupiny a prepoji skupinu s nim aj so zvukmi, ktorych nahravky v nom uz su
    /// (zoznam chyb - Vyriesit).
    /// </summary>
    internal void CreateGroupDirectory(FyzGroup group)
    {
        var path = GroupDirectoryPath(group);
        try
        {
            WithoutFileWatcher(() => _bank.Journal.CreateDirectory(path, group.Language));
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
        {
            _dialogs.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.Groups_DirFailed, path, ex.Message));
            return;
        }

        LinkGroupDirectory(group);
        foreach (var sound in group.Sounds)
            RelinkSoundFile(sound);
        FillExplorerList(group.Directory);
    }

    /// <summary>
    /// Vlozi skupinu do zoznamu skupin jazyka, prepoji ju s priecinkom a vyberie ju.
    /// </summary>
    internal void InsertGroup(FyzGroup group, int index)
    {
        var groups = group.Language.Groups;
        groups.Insert(Math.Clamp(index, 0, groups.Count), group);
        LinkGroupDirectory(group);
        // subory priecinka zase patria zvukom skupiny (po odstraneni skupiny boli bez udajov o zvuku)
        foreach (var sound in group.Sounds)
            RelinkSoundFile(sound);

        RefreshGroupViews(group);
    }

    /// <summary>
    /// Vyberie skupinu zo zoznamu skupin jazyka; jej priecinok a subory ostanu v prieskumniku bez udajov o skupine a zvukoch.
    /// </summary>
    internal void TakeOutGroup(FyzGroup group)
    {
        if (group.Directory?.Group == group)
            group.Directory.Group = null!;
        foreach (var sound in group.Sounds)
            if (sound.File?.Sound == sound)
                sound.File.Sound = null!;

        group.Language.Groups.Remove(group);
        RefreshGroupViews(null);
    }

    /// <summary>
    /// Zmeni kluc, nazov a relativnu cestu skupiny. Pri zmene cesty premenuje priecinok skupiny, ak existuje.
    /// </summary>
    /// <returns><see langword="false" />, ak sa priecinok nepodarilo premenovat - skupina ostala bez zmeny.</returns>
    internal bool ChangeGroup(FyzGroup group, string key, string name, string relativePath)
    {
        if (relativePath != group.RelativePath && !MoveGroupDirectory(group, relativePath))
            return false;

        group.Key = key;
        group.Name = name;
        RefreshGroupViews(group);
        return true;
    }

    private bool MoveGroupDirectory(FyzGroup group, string relativePath)
    {
        var oldRelativePath = group.RelativePath;
        var oldPath = GroupDirectoryPath(group);
        group.RelativePath = relativePath;
        var newPath = GroupDirectoryPath(group);
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
                    WithoutFileWatcher(() => _bank.Journal.Move(oldPath, newPath, group.Language));
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                {
                    error = ex.Message;
                }
            }

            if (error is not null)
            {
                group.RelativePath = oldRelativePath;
                _dialogs.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.Groups_RenameFailed, oldPath, error));
                return false;
            }

            if (group.Directory is not null)
                SetElementPath(group.Directory, newPath);
        }

        // skupina bez priecinka sa prepoji s priecinkom novej cesty, ak existuje
        LinkGroupDirectory(group);
        // zvuky s pridavnou cestou (relativnou k priecinku skupiny) mozu teraz ukazovat inam
        foreach (var sound in group.Sounds)
            RelinkSoundFile(sound);
        return true;
    }

    /// <summary>
    /// Prepoji skupinu s priecinkom jej relativnej cesty v strome prieskumnika (ak priecinok existuje).
    /// </summary>
    internal void LinkGroupDirectory(FyzGroup group)
    {
        var languageDir = group.Language.Directory;
        if (languageDir is null)
            return;

        var path = GroupDirectoryPath(group);
        var folder = GroupRules.FolderName(group.RelativePath);
        var element = languageDir.Children.OfType<DirectoryElement>().FirstOrDefault(d => RawBankExplorer.EqualsPathNames(d.Name, folder));
        if (element is null && Directory.Exists(path))
        {
            element = new DirectoryElement(path) { Parent = languageDir };
            languageDir.Children.Add(element);
        }

        if (group.Directory is not null && group.Directory != element && group.Directory.Group == group)
            group.Directory.Group = null!;

        group.Directory = element!;
        if (element is not null)
            element.Group = group;
    }

    /// <summary>
    /// Po premenovani priecinka na disku upravi cesty prvku a vsetkych prvkov v nom.
    /// </summary>
    internal static void SetElementPath(DirectoryElement directory, string newPath)
    {
        directory.DirInfo = new DirectoryInfo(newPath);
        directory.Name = directory.DirInfo.Name;
        foreach (var child in directory.Children)
        {
            var childPath = Path.Combine(newPath, child.Name);
            switch (child)
            {
                case DirectoryElement d:
                    SetElementPath(d, childPath);
                    break;
                case FileElement f:
                    f.FileInfo = new FileInfo(childPath);
                    break;
            }
        }
    }

    /// <summary>
    /// Vykona zmenu na disku (cez zurnal banky) tak, aby udalosti sledovania suborov nevytvorili ani nezmazali prvky
    /// prieskumnika - tie upravi volajuci sam.
    /// </summary>
    internal void WithoutFileWatcher(System.Action action)
    {
        var watching = fileSystemWatcher.EnableRaisingEvents;
        fileSystemWatcher.EnableRaisingEvents = false;
        RawBankExplorer.ConvertSoundIsHandled = true;
        try
        {
            action();
        }
        finally
        {
            fileSystemWatcher.EnableRaisingEvents = watching;
            // udalosti, ktore watcher zaradil do fronty okna este pred vypnutim, sa spracuju az po tomto
            BeginInvoke(() => RawBankExplorer.ConvertSoundIsHandled = false);
        }
    }

    /// <summary>
    /// Obnovi zoznam skupin, zvukov a prieskumnik po zmene skupin jazyka a vyberie skupinu.
    /// </summary>
    private void RefreshGroupViews(FyzGroup? select)
    {
        if (CurrentLanguage is null || MenuGroups is null)
            return;

        MenuGroups.ResetBindings();
        if (select is not null && ReferenceEquals(select.Language, CurrentLanguage))
        {
            dgvGroups.ClearSelection();
            SelectGroup(select);
        }

        // vybrana skupina sa mohla zmenit alebo zmiznut - zoznam zvukov a prieskumnik musia ukazovat vybranu
        dgvGroups_SelectionChanged(dgvGroups, EventArgs.Empty);
        if (dgvGroups.IsSelectionEmpty())
        {
            CurrentGroup = null;
            MenuSounds = new ExBindingList<FyzSound>([]);
            dgvSounds.DataSource = MenuSounds;
            FillExplorerList(CurrentLanguage.Directory);
        }

        CheckProjectState();
    }
}
