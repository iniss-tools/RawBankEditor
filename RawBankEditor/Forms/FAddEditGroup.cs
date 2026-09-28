using RawBankEditor.Tools;
using ToolsCore.Entities;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

/// <summary>
/// Okno pridania a upravy skupiny zvukov. Len zisti a skontroluje hodnoty - skupinu a jej priecinok meni hlavne okno.
/// </summary>
public partial class FAddEditGroup : Form
{
    private readonly IEnumerable<FyzGroup> _groups;
    private readonly FyzGroup? _group;
    private bool _autoChangeNameAndPath = true;

    /// <param name="groups">Skupiny jazyka - kluc, nazov a cesta musia byt voci nim jedinecne.</param>
    /// <param name="group">Upravovana skupina, pri pridani <see langword="null" />.</param>
    public FAddEditGroup(IEnumerable<FyzGroup> groups, FyzGroup? group = null)
    {
        InitializeComponent();
        this.ApplyThemeAndFonts();

        _groups = groups;
        _group = group;

        if (group == null)
        {
            base.Text = "Pridanie skupiny zvukov";
            bOK.Text = "Pridať";
        }
        else
        {
            // pri uprave by zmena kluca prepisala nazov aj cestu (a tym premenovala priecinok)
            cboxNameAndPathAutoChange.Checked = false;
            tbKey.Text = group.Key;
            tbName.Text = group.Name;
            tbRelativePath.Text = group.RelativePath;
        }
    }

    public string GroupKey => tbKey.Text.Trim();

    public string GroupName => tbName.Text.Trim();

    public string GroupRelativePath => tbRelativePath.Text.Trim();

    private void bOK_Click(object sender, EventArgs e)
    {
        var error = GroupRules.Validate(_groups, _group, GroupKey, GroupName, GroupRelativePath);
        if (error != null)
        {
            Utils.ShowError(error);
            DialogResult = DialogResult.None;
            return;
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

        tbName.Text = tbKey.Text.Trim();
        tbRelativePath.Text = GroupRules.DefaultRelativePath(tbKey.Text);
    }

    private void CboxNameAndPathAutoChange_CheckedChanged(object sender, EventArgs e)
    {
        _autoChangeNameAndPath = cboxNameAndPathAutoChange.Checked;
    }
}
