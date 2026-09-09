using RawBankEditor.Tools;
using ToolsCore.Entities;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

public partial class FAddEditGroup : Form
{
    private bool _autoChangeNameAndPath = true;

    public FAddEditGroup(FyzGroup? group = null)
    {
        InitializeComponent();
        this.ApplyThemeAndFonts();

        if (group == null)
        {
            base.Text = "Pridanie skupiny zvukov";
            bOK.Text = "Pridať";
        }
        else
        {
            Group = group;
            tbKey.Text = group.Key;
            tbName.Text = group.Name;
            tbRelativePath.Text = group.RelativePath;
        }
    }

    public FyzGroup? Group { get; private set; }

    private void bOK_Click(object sender, EventArgs e)
    {
        var key = tbKey.Text;
        var name = tbName.Text;
        var relative = tbRelativePath.Text;

        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(relative))
        {
            Utils.ShowError("Nie sú vyplnené všetky polia.");
            DialogResult = DialogResult.None;
            return;
        }

        if (!relative.EndsWith("\\"))
        {
            Utils.ShowError("Relatívna cesta musí končiť '\\'.");
            DialogResult = DialogResult.None;
            return;
        }

        foreach (var grp in Program.MainForm.CurrentLanguage!.Groups)
        {
            if (grp.Key == key)
            {
                Utils.ShowError("Položka s rovnakým kľučom už existuje.");
                DialogResult = DialogResult.None;
                return;
            }

            if (grp.Name == name)
            {
                Utils.ShowError("Položka s rovnakým názvom už existuje.");
                DialogResult = DialogResult.None;
                return;
            }

            if (grp.RelativePath == relative)
            {
                Utils.ShowError("Položka s rovnakou relatívnou cestou už existuje.");
                DialogResult = DialogResult.None;
                return;
            }
        }

        if (Group != null)
        {
            Program.MainForm.RegisterNewAction(
                new FMain.EditGroupAction(Program.MainForm, Group, (Group.Key, key), (Group.Name,name), (Group.RelativePath, relative)));
            var oldPath = Group.GetAbsPath(GlobData.OpenedProject!.AbsPathToBank);
            Group.Key = key;
            Group.Name = name;
            Group.RelativePath = relative;
            var newPath = Group.GetAbsPath(GlobData.OpenedProject!.AbsPathToBank);
            Directory.Move(oldPath, newPath);
        }
        else
        {
            Group = new FyzGroup(Program.MainForm.CurrentLanguage!, key, name, relative);
            Program.MainForm.RegisterNewAction(new FMain.AddGroupAction(Program.MainForm, Group));
            var path = Group.GetAbsPath(GlobData.OpenedProject!.AbsPathToBank);
            Directory.CreateDirectory(path);
            Group.Directory = new DirectoryElement(path) { Group = Group };
            RawBankExplorer.AddDirHandled = Group.Directory;
        }

        DialogResult = DialogResult.OK;
    }

    private void BStorno_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
    }

    private void TbKey_TextChanged(object sender, EventArgs e)
    {
        if (!_autoChangeNameAndPath)
            return;

        tbName.Text = tbKey.Text;
        tbRelativePath.Text = tbKey.Text + '\\';
    }

    private void CboxNameAndPathAutoChange_CheckedChanged(object sender, EventArgs e)
    {
        _autoChangeNameAndPath = cboxNameAndPathAutoChange.Checked;
    }
}