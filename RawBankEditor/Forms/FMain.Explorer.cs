using System.Globalization;
using RawBankEditor.Properties;
using RawBankEditor.Tools;
using ToolsCore.Entities;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

partial class FMain
{
    private void DoRenameFile()
    {
        if (dgvExplorer.IsSelectionEmpty() || dgvExplorer.SelectedRows[0].DataBoundItem is BackButtonElement)
            return;

        dgvExplorer.CurrentCell = dgvExplorer.SelectedRows[0].Cells[nameof(cFileName)];
        _editingFileName = true;
        dgvExplorer.BeginEdit(true);
    }

    private void DgvExplorer_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
    {
        if (!_editingFileName || e.RowIndex < 0)
            return;

        var newName = ((string?)e.FormattedValue ?? "").Trim();
        var error = dgvExplorer.Rows[e.RowIndex].DataBoundItem switch
        {
            FileElement fe => RenameError(fe.FileInfo.FullName, newName),
            DirectoryElement de => RenameError(de.DirInfo.FullName, newName),
            _ => null
        };
        if (error is null)
            return;

        Utils.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.Explorer_FixNameOrEsc, error));
        e.Cancel = true;
    }

    private static string? RenameError(string path, string newName)
    {
        if (string.IsNullOrWhiteSpace(newName) || newName is "." or ".." || newName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            return string.Format(CultureInfo.CurrentCulture, Resources.Explorer_InvalidName, newName);

        // len zmena velkosti pismen - ta ista polozka
        if (string.Equals(Path.GetFileName(path), newName, StringComparison.OrdinalIgnoreCase))
            return null;

        var target = Path.Combine(Path.GetDirectoryName(path)!, newName);
        return File.Exists(target) || Directory.Exists(target) ? string.Format(CultureInfo.CurrentCulture, Resources.Explorer_ItemExists, newName) : null;
    }

    private void DgvExplorer_CellEndEdit(object sender, DataGridViewCellEventArgs e)
    {
        if (!_editingFileName)
            return;
        _editingFileName = false;

        var element = dgvExplorer.Rows[e.RowIndex].DataBoundItem as FileSystemElement;
        var newName = ((string?)dgvExplorer[e.ColumnIndex, e.RowIndex].Value ?? "").Trim();

        // tabulka uz zapisala novy nazov do prvku - nazov prvku sa zmeni az po premenovani na disku
        switch (element)
        {
            case DirectoryElement de:
                de.Name = de.DirInfo.Name;
                if (newName != de.Name)
                {
                    if (de.Group is not null && ReferenceEquals(de.Group.Directory, de))
                        RenameGroupFolder(de.Group, newName);
                    else
                        RenameAndRegister(de.DirInfo.FullName, newName);
                }
                break;
            case FileElement fe:
                fe.Name = fe.FileInfo.Name;
                if (newName != fe.Name)
                    RenameAndRegister(fe.FileInfo.FullName, newName);
                break;
        }

        FillExplorerList(CurrentDirectory);
    }

    /// <summary>
    /// Priecinok skupiny sa premenuje upravou skupiny - inak by skupina ukazovala na priecinok, ktory uz neexistuje.
    /// </summary>
    private void RenameGroupFolder(FyzGroup group, string folderName)
    {
        var relativePath = folderName + '\\';
        var error = GroupRules.Validate(group.Language.Groups, group, group.Key, group.Name, relativePath);
        if (error is not null)
        {
            Utils.ShowError(error);
            return;
        }

        var action = new EditGroupAction(this, group, (group.Key, group.Key), (group.Name, group.Name), (group.RelativePath, relativePath));
        if (ChangeGroup(group, group.Key, group.Name, relativePath))
            RegisterNewAction(action);
    }

    private void RenameAndRegister(string oldPath, string newName)
    {
        var newPath = Path.Combine(Path.GetDirectoryName(oldPath)!, newName);
        if (RenameOnDisk(oldPath, newPath))
            RegisterNewAction(new RenameFileAction(this, oldPath, newPath));
    }

    /// <summary>
    /// Premenuje subor alebo priecinok a upravi prvok prieskumnika aj zvuk, ktoremu subor patri.
    /// </summary>
    /// <returns><see langword="false" />, ak premenovanie zlyhalo.</returns>
    internal bool RenameOnDisk(string oldPath, string newPath)
    {
        try
        {
            WithoutFileWatcher(() =>
            {
                if (Directory.Exists(oldPath))
                    Directory.Move(oldPath, newPath);
                else
                    File.Move(oldPath, newPath);
            });
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Utils.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.Explorer_RenameFailed, Path.GetFileName(oldPath), ex.Message));
            return false;
        }

        ApplyRename(oldPath, newPath);
        FillExplorerList(CurrentDirectory);
        return true;
    }

    /// <summary>
    /// Po premenovani na disku (v programe aj mimo neho) upravi prvok prieskumnika. Zvuk, ktoremu subor patri,
    /// dostane novy nazov suboru; skupina, ktorej priecinok sa premenoval, novu relativnu cestu.
    /// </summary>
    private void ApplyRename(string oldPath, string newPath)
    {
        if (CurrentLanguage?.Directory is null)
            return;

        switch (RawBankExplorer.GetElement(oldPath, CurrentLanguage.Directory))
        {
            case DirectoryElement de:
                SetElementPath(de, newPath);
                if (de.Group is not null && ReferenceEquals(de.Group.Directory, de))
                {
                    // premenovanie mimo programu - cesta skupiny sa nedala upravit vopred
                    de.Group.RelativePath = de.Name + '\\';
                    RegisterNewAction();
                    MenuGroups?.ResetBindings();
                }
                break;
            case FileElement fe:
                fe.FileInfo = new FileInfo(newPath);
                fe.Name = fe.FileInfo.Name;
                if (fe is SoundFileElement { Sound: { } sound } sfe && ReferenceEquals(sound.File, sfe))
                {
                    var ext = Path.GetExtension(sfe.Name);
                    if (ext.EqualsIgnoreCase(SoundUtils.WAV_EXT) || ext.EqualsIgnoreCase(SoundUtils.EWA_EXT))
                    {
                        sound.FileName = sfe.Name;
                    }
                    else
                    {
                        // subor uz nie je nahravka - zvuk ostane bez suboru
                        sound.File = null!;
                        sfe.Sound = null!;
                    }
                    MenuSounds?.ResetBindings();
                }
                break;
        }

        CheckProjectState();
    }

    private void DoDeleteFile()
    {
        if (dgvExplorer.IsSelectionEmpty())
            return;

        var result = Utils.ShowWarning(Resources.Explorer_DeleteConfirm, MessageBoxButtons.YesNo);
        if (result != DialogResult.Yes)
            return;

        // kopia - odstranenie meni obsah prieskumnika
        var items = dgvExplorer.SelectedRows.Cast<DataGridViewRow>().Select(r => r.DataBoundItem).OfType<FileSystemElement>().ToList();
        foreach (var item in items)
        {
            switch (item)
            {
                case BackButtonElement:
                    break;
                case DirectoryElement { Group: { } group } de when ReferenceEquals(group.Directory, de):
                    // priecinok skupiny - skupina sa odstrani spolu s nim (Spat vrati oboje)
                    RemoveGroup(group, true);
                    break;
                case SoundFileElement { Sound: { } sound } sfe when ReferenceEquals(sound.File, sfe):
                    // zvuk sa odstrani spolu so suborom (Spat vrati oboje)
                    var action = new RemovedSoundsAction(this, sound) { RecycledFile = sfe };
                    if (RecycleElement(sfe))
                    {
                        action.Apply();
                        RegisterNewAction(action);
                    }
                    break;
                default:
                    RecycleElement(item);
                    break;
            }
        }

        FillExplorerList(CurrentDirectory);
        CheckProjectState();
    }

    /// <summary>
    /// Presunie subor alebo priecinok do kosa a vyberie jeho prvok zo stromu prieskumnika (prvok si pamata rodica,
    /// aby ho Spat mohlo vratit).
    /// </summary>
    internal bool RecycleElement(FileSystemElement element)
    {
        var path = element switch
        {
            DirectoryElement de => de.DirInfo.FullName,
            FileElement fe => fe.FileInfo.FullName,
            _ => null
        };
        if (path is null)
            return false;

        try
        {
            WithoutFileWatcher(() =>
            {
                if (element is DirectoryElement)
                    Utils.DeleteDirectoryToRecycleBin(path);
                else
                    Utils.DeleteFileToRecycleBin(path);
            });
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or OperationCanceledException)
        {
            if (ex is not OperationCanceledException)
                Utils.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.Explorer_RecycleFailed, element.Name, ex.Message));
            return false;
        }

        element.Parent?.Children.Remove(element);
        return true;
    }

    /// <summary>
    /// Obnovi subor z kosa a vrati jeho prvok do stromu prieskumnika.
    /// </summary>
    internal bool RestoreElement(FileElement element)
    {
        var path = element.FileInfo.FullName;
        var restored = File.Exists(path);
        if (!restored)
            WithoutFileWatcher(() => restored = Utils.TryRecoverFileOrDirFromBin(path));
        if (!restored)
        {
            Utils.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.Explorer_RestoreFailed, element.Name));
            return false;
        }

        element.FileInfo = new FileInfo(path);
        if (element.Parent is { } parent && !parent.Children.Contains(element))
            parent.Children.Add(element);
        return true;
    }
}
