using System.Globalization;
using RawBankEditor.Forms;
using RawBankEditor.Properties;
using ToolsCore.Iniss.Entities;
// ReSharper disable MemberCanBePrivate.Global

namespace RawBankEditor.Entities;

public class InvalidSoundFile : IRawBankMessage
{
    /// <inheritdoc />
    public string Code => "InvalidSoundFile";

    /// <inheritdoc />
    public MessageType Type => MessageType.Error;

    /// <inheritdoc />
    public string Message => string.Format(CultureInfo.CurrentCulture, Resources.Msg_InvalidWav, File.Name);

    /// <inheritdoc />
    public string ResolveMessage => string.Format(CultureInfo.CurrentCulture, Resources.Msg_RecycleFile, File.Name);

    /// <inheritdoc />
    public string Path => System.IO.Path.GetRelativePath(PathToBank, File.FileInfo.FullName);

    public SoundFileElement File { get; }

    /// <summary>
    /// Priecinok RAWBANK - cesta suboru sa ukazuje vzhladom na neho, rovnako ako pri chybajucich priecinkoch a suboroch.
    /// </summary>
    private string PathToBank { get; }

    public InvalidSoundFile(SoundFileElement file, string pathToBank)
    {
        File = file;
        PathToBank = pathToBank;
    }    

    /// <inheritdoc />
    public void Resolve(FMain form)
    {
        // zvuk, ktoremu subor patril, ostane bez suboru
        if (File.Sound is { } sound && ReferenceEquals(sound.File, File))
        {
            sound.File = null!;
            File.Sound = null!;
        }

        form.RecycleElement(File);
        form.FillExplorerList(form.CurrentDirectory);
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
        }
        else
        {
            form.FillExplorerList(File.Parent);
        }

        index = form.ExplorerContent.IndexOf(File);
        if (index == -1)
            return;

        form.dgvExplorer.ClearSelection();
        form.dgvExplorer.Rows[index].Selected = true;
    }
}
