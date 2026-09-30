using System.Globalization;
using RawBankEditor.Properties;
using RawBankEditor.Tools;
using ToolsCore.Iniss.Entities;
using ToolsCore.Iniss.Tools;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

public partial class FAddSound : Form
{
    private bool _autoChangeNameAndFile = true;
    private readonly string _pathToBank;

    public FyzSound Sound { get; private set; } = null!;
    private FyzGroup Group { get; }
    private SoundFileElement? File { get; }

    /// <param name="group">skupina, do ktorej sa zvuk prida (validacia duplicit, pripona a vyhladanie suboru).</param>
    /// <param name="file">
    /// existujuci subor bez udajov o zvuku - nazov suboru sa z neho prevezme a nazov suboru
    /// ani pridavna cesta sa nedaju menit.
    /// </param>
    /// <param name="group">skupina noveho zvuku</param>
    /// <param name="pathToBank">priecinok RAWBANK - nahravka noveho zvuku sa hlada v nom</param>
    /// <param name="file">subor bez udajov o zvuku, ku ktoremu sa zvuk pridava</param>
    public FAddSound(FyzGroup group, string pathToBank, SoundFileElement? file = null)
    {
        _pathToBank = pathToBank;
        InitializeComponent();
        this.ApplyThemeAndFonts();

        Group = group ?? throw new ArgumentNullException(nameof(group));
        File = file;

        if (file is not null)
        {
            tbFileName.ReadOnly = true;
            tbRelativePath.ReadOnly = true;
            tbFileName.Text = file.Name;
            tbKey.Text = Path.GetFileNameWithoutExtension(file.Name);
        }
    }

    private async void BSave_Click(object sender, EventArgs e)
    {
        var key = tbKey.Text;
        var name = tbName.Text;
        var fileName = tbFileName.Text.Trim();
        var relative = tbRelativePath.Text;

        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(name) || string.IsNullOrEmpty(fileName))
        {
            Utils.ShowError(Resources.FAddSound_FieldsRequired);
            DialogResult = DialogResult.None;
            return;
        }

        var ext = Path.GetExtension(fileName);
        if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || !(ext.EqualsIgnoreCase(SoundUtils.WAVExt) || ext.EqualsIgnoreCase(SoundUtils.EWAExt)))
        {
            Utils.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.FAddSound_InvalidFileName, SoundUtils.WAVExt, SoundUtils.EWAExt));
            DialogResult = DialogResult.None;
            return;
        }

        if (!string.IsNullOrEmpty(relative) && (!relative.EndsWith('\\') || string.IsNullOrWhiteSpace(relative)))
        {
            Utils.ShowError(Resources.FMain_InvalidRelativePath);
            DialogResult = DialogResult.None;
            return;
        }

        foreach (var snd in Group.Sounds)
        {
            if (SoundRules.SameText(snd.Key, key))
            {
                Utils.ShowError(Resources.FAddSound_KeyExists);
                DialogResult = DialogResult.None;
                return;
            }

            if (SoundRules.SameText(snd.Name, name))
            {
                Utils.ShowError(Resources.FAddSound_NameExists);
                DialogResult = DialogResult.None;
                return;
            }
        }

        var sound = new FyzSound(Group, key, name, fileName, relative, rtbText.Text, 0);

        // prepojenie s existujucim suborom a dlzka zvuku (rovnako ako pri automatickom vkladani)
        var sfe = File ?? SoundUtils.FindSoundFile(sound, _pathToBank);
        if (sfe is not null)
        {
            if (sfe.Duration < 0)
            {
                bSave.Enabled = false;
                sfe.Duration = await SoundUtils.GetSoundDuration(sfe);
                bSave.Enabled = true;

                // okno medzitym zavrete cez Zrusit
                if (DialogResult == DialogResult.Cancel || IsDisposed)
                    return;
            }

            sound.Duration = Math.Max(sfe.Duration, 0);
            sound.File = sfe;
            sfe.Sound = sound;
        }

        Sound = sound;
        DialogResult = DialogResult.OK;
    }

    private void BStorno_Click(object sender, EventArgs e) => DialogResult = DialogResult.Cancel;

    private void TbKey_TextChanged(object sender, EventArgs e)
    {
        if (!_autoChangeNameAndFile)
            return;

        tbName.Text = tbKey.Text;

        // subor zadany zvonku (SoundDataMissing) sa nemeni
        if (File is null)
            tbFileName.Text = string.IsNullOrEmpty(tbKey.Text) ? "" : SoundUtils.GetDefaultFileName(Group, tbKey.Text);
    }

    private void CboxNameAndFileAutoChange_CheckedChanged(object sender, EventArgs e)
    {
        _autoChangeNameAndFile = cboxNameAndFileAutoChange.Checked;
    }
}
