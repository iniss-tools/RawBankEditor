using ToolsCore.Entities;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

public partial class FSoundsMove : Form
{
    public FyzGroup NewGroup { get; private set; } = null!;
    private IList<FyzGroup> GroupsWithoutCurrent { get; }

    public FSoundsMove(IList<FyzGroup> allGroups, FyzGroup currentGroup)
    {
        InitializeComponent();
        this.ApplyThemeAndFonts();

        GroupsWithoutCurrent = new List<FyzGroup>(allGroups);
        GroupsWithoutCurrent.Remove(currentGroup);
        cbGroups.DataSource = GroupsWithoutCurrent;
    }

    private void BOK_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.None;
        if (cbGroups.SelectedIndex == -1)
        {
            Utils.ShowError("Nebola vybratá žiadna skupina.");
            return;
        }

        NewGroup = GroupsWithoutCurrent[cbGroups.SelectedIndex];
    }
}