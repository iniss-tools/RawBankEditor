using System.Globalization;
using RawBankEditor.Forms;
using RawBankEditor.Properties;
using ToolsCore.Iniss.Entities;
using ToolsCore.Tools;
// ReSharper disable MemberCanBePrivate.Global

namespace RawBankEditor.Entities;

public class SoundDataMissing : IRawBankMessage
{
    /// <inheritdoc />
    public string Code => "SoundDataMissing";

    /// <inheritdoc />
    public MessageType Type => MessageType.Warning;

    /// <inheritdoc />
    public string Message => string.Format(CultureInfo.CurrentCulture, Resources.Msg_SoundUndefined, File.Name);

    /// <inheritdoc />
    public string ResolveMessage => string.Format(CultureInfo.CurrentCulture, Resources.Msg_AddSoundData, File.Name);

    /// <inheritdoc />
    public string Path => System.IO.Path.GetRelativePath(PathToBank, File.FileInfo.FullName);

    public SoundFileElement File { get; }

    /// <summary>
    /// Priecinok RAWBANK - cesta suboru sa ukazuje vzhladom na neho, rovnako ako pri chybajucich priecinkoch a suboroch.
    /// </summary>
    private string PathToBank { get; }

    public SoundDataMissing(SoundFileElement file, string pathToBank)
    {
        File = file;
        PathToBank = pathToBank;
    }
    
    public void Resolve(FMain form)
    {
        var group = File.Parent?.Group;
        if (group is null)
        {
            Utils.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.Msg_FileNotInGroup, File.Name));
            return;
        }

        var dialog = new FAddSound(group, form.PathToBank, File);
        if (dialog.ShowDialog(form) == DialogResult.OK)
        {
            form.RegisterNewAction(new FMain.AddSoundAction(form, dialog.Sound));
            group.Sounds.Add(dialog.Sound);
            form.RefreshSoundViews();
        }
    }

    /// <inheritdoc />
    public void Show(FMain form)
    {
        var grp = File.Parent!.Group;
        var index = form.CurrentLanguage!.Groups.IndexOf(grp);
        if (index != -1)
        {
            form.dgvGroups.ClearSelection();
            form.dgvGroups.Rows[index].Selected = true;
            form.SelectElement = File;
        }
        else
        {
            form.SelectElement = File;
            form.FillExplorerList(File.Parent);
        }
    }
}
