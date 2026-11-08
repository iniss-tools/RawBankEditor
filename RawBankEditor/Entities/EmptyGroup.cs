using System.Globalization;
using RawBankEditor.Forms;
using RawBankEditor.Properties;
using ToolsCore.Iniss.Entities;
// ReSharper disable MemberCanBePrivate.Global

namespace RawBankEditor.Entities;

public class EmptyGroup : IRawBankMessage
{
    /// <inheritdoc />
    public string Code => "EmptyGroup";

    /// <inheritdoc />
    public MessageType Type => MessageType.Info;

    /// <inheritdoc />
    public string Message => string.Format(CultureInfo.CurrentCulture, Resources.Msg_GroupEmpty, Group.Name);

    /// <inheritdoc />
    public string ResolveMessage => string.Format(CultureInfo.CurrentCulture, Resources.Msg_RemoveGroup, Group.Name);

    /// <inheritdoc />
    public string Path => Group.GetAbsPath("");

    public FyzGroup Group { get; }

    public EmptyGroup(FyzGroup group)
    {
        Group = group;
    }

    /// <inheritdoc />
    public void Resolve(FMain form)
    {
        // priecinok ide do kosa len prazdny - nahravky bez udajov o zvuku by sa inak stratili nepozorovane
        var directory = form.GroupDirectoryPath(Group);
        var emptyDirectory = Directory.Exists(directory) && !Directory.EnumerateFileSystemEntries(directory).Any();
        form.RemoveGroup(Group, emptyDirectory);
    }

    /// <inheritdoc />
    public void Show(FMain form)
    {
        var index = form.CurrentLanguage!.Groups.IndexOf(Group);
        if (index != -1)
        {
            form.dgvGroups.ClearSelection();
            form.dgvGroups.Rows[index].Selected = true;
        }
    }
}
