using ToolsCore.Entities;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

public partial class FAddEditLanguage : Form
{
    private bool _autoChangeNameAndPath = true;
    public FyzLanguage? Language { get; private set; }

    public FAddEditLanguage(FyzLanguage? language = null)
    {
        InitializeComponent();
        this.ApplyThemeAndFonts();

        Language = language;

        if (language != null)
        {
            tbKey.Text = language.Key;
            tbName.Text = language.Name;
            tbRelativePath.Text = language.RelativePath;
        }
        else
        {
            base.Text = "Pridať jazyk";

        }
    }

    private void BSave_Click(object sender, EventArgs e)
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

        if (Language != null)
        {
            Program.MainForm.RegisterNewAction(
                new FMain.EditLanguageAction(Program.MainForm, Language, (Language.Key, key), (Language.Name,name), (Language.RelativePath, relative)));
        }
        else
        {
            Language = new FyzLanguage(key, name, relative);
            Program.MainForm.RegisterNewAction(new FMain.AddLanguageAction(Program.MainForm, Language));
        }

        DialogResult = DialogResult.OK;
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