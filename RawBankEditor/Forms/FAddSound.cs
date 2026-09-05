using ToolsCore.Entities;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

public partial class FAddSound : Form
{
    private bool _autoChangeNameAndPath = true;

    public FyzSound Sound { get; private set; } = null!;
    private SoundFileElement? File { get; }

    public FAddSound(SoundFileElement? file = null)
    {
        InitializeComponent();
        this.ApplyThemeAndFonts();

        File = file;
    }

    private void BSave_Click(object sender, EventArgs e)
    {
        var key = tbKey.Text;
        var name = tbName.Text;
        var relative = tbRelativePath.Text;

        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(name))
        {
            Utils.ShowError("Nie sú vyplnené všetky požadované polia.");
            DialogResult = DialogResult.None;
            return;
        }

        if (!string.IsNullOrEmpty(relative) && (!relative.EndsWith("\\") || string.IsNullOrWhiteSpace(relative)))
        {
            Utils.ShowError("Prídavná relatívna cesta musí končiť '\\' a nesmie obsahovať iba prázdne znaky.");
            DialogResult = DialogResult.None;
            return;
        }

        foreach (var grp in Program.MainForm.CurrentGroup!.Sounds)
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
        }

        Sound = new FyzSound
        {
            Key = key,
            Name = name,
            AdditionalRelativePath = relative,
            Text = rtbText.Text
        };

        if (File is not null) 
            File.Sound = Sound;

        DialogResult = DialogResult.OK;
    }

    private void BStorno_Click(object sender, EventArgs e) => DialogResult = DialogResult.Cancel;

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