using RawBankEditor.Tools;
using ToolsCore.Entities;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

/// <summary>
/// Okno na pridanie alebo upravu jazyka banky. Samo nic nemeni - zadane hodnoty spracuje FMain.
/// </summary>
public partial class FAddEditLanguage : Form
{
    private readonly IEnumerable<FyzLanguage> _languages;
    private readonly FyzLanguage? _edited;
    private bool _autoChangeNameAndPath = true;

    public string LanguageKey { get; private set; } = "";
    public string LanguageName { get; private set; } = "";
    public string LanguageRelativePath { get; private set; } = "";

    /// <param name="languages">Jazyky banky, s ktorymi sa porovnava kluc, nazov a cesta.</param>
    /// <param name="language">Upravovany jazyk, pri pridani <see langword="null" />.</param>
    public FAddEditLanguage(IEnumerable<FyzLanguage> languages, FyzLanguage? language = null)
    {
        InitializeComponent();
        this.ApplyThemeAndFonts();

        _languages = languages;
        _edited = language;

        if (language != null)
        {
            // pri uprave by zmena kluca prepisala nazov aj cestu existujuceho jazyka
            cboxNameAndPathAutoChange.Checked = false;
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
        var key = tbKey.Text.Trim();
        var name = tbName.Text.Trim();
        var relative = tbRelativePath.Text.Trim();

        var error = LanguageRules.Validate(_languages, _edited, key, name, relative);
        if (error != null)
        {
            Utils.ShowError(error);
            DialogResult = DialogResult.None;
            return;
        }

        LanguageKey = key;
        LanguageName = name;
        LanguageRelativePath = relative;
        DialogResult = DialogResult.OK;
    }

    private void TbKey_TextChanged(object sender, EventArgs e)
    {
        if (!_autoChangeNameAndPath)
            return;

        tbName.Text = tbKey.Text.Trim();
        tbRelativePath.Text = LanguageRules.DefaultRelativePath(tbKey.Text);
    }

    private void CboxNameAndPathAutoChange_CheckedChanged(object sender, EventArgs e)
    {
        _autoChangeNameAndPath = cboxNameAndPathAutoChange.Checked;
    }
}
