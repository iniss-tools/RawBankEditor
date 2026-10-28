using System.Globalization;
using RawBankEditor.Properties;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

/// <summary>
/// Vyber rozsahu nahravok (vybrane zvuky, skupina, cely jazyk) pred zmenou hlasu alebo vycistenim.
/// </summary>
public partial class FScope : Form
{
    /// <param name="title">nazov operacie</param>
    /// <param name="selected">pocet vybranych zvukov s nahravkou</param>
    /// <param name="groupName">nazov otvorenej skupiny</param>
    /// <param name="group">pocet zvukov skupiny s nahravkou</param>
    /// <param name="languageName">nazov jazyka</param>
    /// <param name="language">pocet zvukov jazyka s nahravkou</param>
    public FScope(string title, int selected, string groupName, int group, string languageName, int language)
    {
        InitializeComponent();
        this.ApplyThemeAndFonts();

        Text = title;
        rbSelected.Text = string.Format(CultureInfo.CurrentCulture, Resources.FAdjustSounds_Selected, selected);
        rbGroup.Text = string.Format(CultureInfo.CurrentCulture, Resources.FAdjustSounds_Group, groupName, group);
        rbLanguage.Text = string.Format(CultureInfo.CurrentCulture, Resources.FAdjustSounds_Language, languageName, language);
        rbSelected.Enabled = selected > 0;
        rbGroup.Enabled = group > 0;
        rbLanguage.Enabled = language > 0;
        (selected > 0 ? rbSelected : group > 0 ? rbGroup : rbLanguage).Checked = true;
        bOK.Enabled = language > 0;
    }

    /// <summary>
    /// Vybrany rozsah.
    /// </summary>
    public SoundScope Scope => rbSelected.Checked ? SoundScope.Selected : rbGroup.Checked ? SoundScope.Group : SoundScope.Language;

    private void BOK_Click(object? sender, EventArgs e) => DialogResult = DialogResult.OK;
}

/// <summary>
/// Ktore nahravky spracovat.
/// </summary>
public enum SoundScope
{
    /// <summary>Vybrane zvuky.</summary>
    Selected,

    /// <summary>Otvorena skupina.</summary>
    Group,

    /// <summary>Cely jazyk.</summary>
    Language
}
