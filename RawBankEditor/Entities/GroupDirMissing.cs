using System.Globalization;
using RawBankEditor.Forms;
using RawBankEditor.Properties;
using ToolsCore.Iniss.Entities;
// ReSharper disable MemberCanBePrivate.Global

namespace RawBankEditor.Entities;

public class GroupDirMissing : IRawBankMessage
{
    /// <inheritdoc />
    public string Code => "GroupDirMissing";

    /// <inheritdoc />
    public MessageType Type => MessageType.Error;

    /// <inheritdoc />
    public string Message => string.Format(CultureInfo.CurrentCulture, Resources.Msg_GroupMissing, Group.Name);

    /// <inheritdoc />
    public string ResolveMessage => string.Format(CultureInfo.CurrentCulture, Resources.Msg_CreateDir, Group.GetAbsPath(""));

    /// <inheritdoc />
    public string Path => Group.GetAbsPath("");

    public FyzGroup Group { get; }

    public GroupDirMissing(FyzGroup group)
    {
        Group = group;
    }

    public void Resolve(FMain form)
    {
        // priecinok sa prida do prieskumnika a zvuky skupiny sa prepoja s nahravkami, ktore v nom uz su
        form.CreateGroupDirectory(Group);
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
