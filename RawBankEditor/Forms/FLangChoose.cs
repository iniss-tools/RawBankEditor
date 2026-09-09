using ToolsCore.Entities;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

public partial class FLangChoose : Form
{
    public FLangChoose()
    {
        InitializeComponent();
        this.ApplyThemeAndFonts();

        cboxLanguages.DataSource = GlobData.OpenedProject!.Languages;
    }

    public FyzLanguage Selected { get; private set; } = null!;

    private void bOK_Click(object sender, EventArgs e)
    {
        Selected = (FyzLanguage)cboxLanguages.SelectedItem!;
        DialogResult = DialogResult.OK;
    }
}