using ToolsCore.Entities;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

public partial class FLangChoose : Form
{
    public FLangChoose(IList<FyzLanguage> languages)
    {
        InitializeComponent();
        this.ApplyThemeAndFonts();

        cboxLanguages.DataSource = languages;
    }

    public FyzLanguage Selected { get; private set; } = null!;

    private void bOK_Click(object sender, EventArgs e)
    {
        Selected = (FyzLanguage)cboxLanguages.SelectedItem!;
        DialogResult = DialogResult.OK;
    }
}