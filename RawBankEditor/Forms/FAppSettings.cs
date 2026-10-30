using System.Globalization;
using ExControls;
using RawBankEditor.Properties;
using RawBankEditor.Speech;
using RawBankEditor.XML;
using ToolsCore;
using ToolsCore.Commands;
using ToolsCore.Forms;
using ToolsCore.Iniss.Tools;
using ToolsCore.Tools;
using ToolsCore.XML;

namespace RawBankEditor.Forms;

public partial class FAppSettings : FAppSettingsBase
{
    /// <summary>
    /// Stranka hlasovej syntezy (pre <see cref="FAppSettingsBase.PreselectMenuItem" />).
    /// </summary>
    public const string SpeechPage = "pSpeech";

    public new RawBankEditorConfig Config => (RawBankEditorConfig)base.Config;
    public new Styles<RawBankEditorStyle> Styles => (Styles<RawBankEditorStyle>)base.Styles;


    protected override IList<DesktopColumn> DefaultColumns => new DesktopColumns().GetValues();

    /// <inheritdoc />
    protected override string ReopenLastText => Resources.Settings_ReopenLast;

    /// <inheritdoc />
    protected override string ColumnsPageText => Resources.Settings_ColumnsPage;

    // nastavenia programu - po ulozeni sa v nich nahradi konfiguracia a styly
    private readonly AppSession<RawBankEditorConfig, RawBankEditorStyle> _session;

    /// <param name="session">nastavenia programu</param>
    /// <param name="commands">prikazy hlavneho okna - strom skratiek podla jeho ponuky; null = podla katalogu</param>
    internal FAppSettings(AppSession<RawBankEditorConfig, RawBankEditorStyle> session, IEnumerable<AppCommand>? commands = null)
        : base(session.Config, new Styles<RawBankEditorStyle>(session.Styles), session.UsingStyle, typeof(RawBankEditorStyle))
    {
        _session = session;
        InitializeComponent();

        SetCommands(RbeCommands.All, commands);
        Columns = new ExBindingList<DesktopColumn>(Config.DesktopCols.GetValues());
        dgvColumns.DataSource = Columns;
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        cboxAutoRecalculateSoundDurations.Checked = Config.AutoRecalculateSoundDuration;
        cboxAutoInsertSoundData.Checked = Config.AutoInsertSoundData;
        cboxShowAfterInsertSoundDlg.Checked = Config.ShowAfterInsertSoundDialog;
        tbSpeechKey.Text = Config.Speech.ElevenLabsKey;
        tbAzureKey.Text = Config.Speech.AzureKey;
        tbAzureRegion.Text = Config.Speech.AzureRegion;
        tbPiperPath.Text = Config.Speech.PiperPath;
        tbPiperVoices.Text = Config.Speech.PiperVoicesFolder;
        // prazdny priecinok = predvoleny
        tbPiperVoices.HintText = PiperVoices.DefaultFolder;
        nudSpeechConcurrency.Value = Math.Clamp(Config.Speech.Concurrency, (int)nudSpeechConcurrency.Minimum, (int)nudSpeechConcurrency.Maximum);
        ShowSpeechCacheSize();
    }

    /// <inheritdoc />
    protected override bool OnSaving()
    {
        Config.DesktopCols.SetValues(Columns);
        Config.AutoRecalculateSoundDuration = cboxAutoRecalculateSoundDurations.Checked;
        Config.AutoInsertSoundData = cboxAutoInsertSoundData.Checked;
        Config.ShowAfterInsertSoundDialog = cboxShowAfterInsertSoundDlg.Checked;
        Config.Speech.ElevenLabsKey = tbSpeechKey.Text;
        Config.Speech.AzureKey = tbAzureKey.Text.Trim();
        Config.Speech.AzureRegion = tbAzureRegion.Text.Trim();
        Config.Speech.PiperPath = tbPiperPath.Text.Trim();
        Config.Speech.PiperVoicesFolder = tbPiperVoices.Text.Trim();
        Config.Speech.Concurrency = decimal.ToInt32(nudSpeechConcurrency.Value);
        return base.OnSaving();
    }

    /// <inheritdoc />
    protected override void SaveData()
    {
        _session.Config = Config;
        _session.UsingStyle = (RawBankEditorStyle)UsingStyle;
        _session.Styles = Styles;

        var configsDir = ToolsCore.AppPaths.ConfigDir;
        if (!Directory.Exists(configsDir))
            Directory.CreateDirectory(configsDir);

        Styles<RawBankEditorStyle>.WriteData(PathUtils.CombinePath(configsDir, ToolsCore.FileConsts.FileStyles)!, _session.Styles);
        XmlSerialization.WriteData(PathUtils.CombinePath(configsDir, ToolsCore.FileConsts.FileConfig)!, _session.Config);
    }

    /// <inheritdoc />
    protected override Style CreateStyleInstance(string name) => new RawBankEditorStyle {Name = name};

    /// <inheritdoc />
    protected override Style OnResetStyle(bool darkMode) => Styles<RawBankEditorStyle>.GetDefaultStyle(darkMode);

    private void CboxAutoInsertSoundData_CheckedChanged(object sender, EventArgs e)
    {
        // okno sa otvara len pri automatickom vkladani - bez neho volba nic nerobi, ale jej hodnota ostava
        cboxShowAfterInsertSoundDlg.Enabled = cboxAutoInsertSoundData.Checked;
    }

    private void ShowSpeechCacheSize() =>
        lSpeechCache.Text = string.Format(CultureInfo.CurrentCulture, Resources.FAppSettings_SpeechCacheSize, SpeechCache.Default().SizeBytes() / 1048576.0);

    private async void BSpeechVerify_Click(object? sender, EventArgs e)
    {
        var key = tbSpeechKey.Text.Trim();
        if (key.Length == 0)
        {
            lSpeechStatus.Text = Resources.FAppSettings_SpeechNoKey;
            return;
        }

        bSpeechVerify.Enabled = false;
        lSpeechStatus.Text = Resources.FAppSettings_SpeechVerifying;
        try
        {
            using var client = new ElevenLabsClient(key);
            var quota = await client.GetQuotaAsync(CancellationToken.None);
            if (!IsDisposed)
                lSpeechStatus.Text = string.Format(CultureInfo.CurrentCulture, Resources.FAppSettings_SpeechVerified, quota.Tier, quota.Used, quota.Limit);
        }
        catch (SpeechException ex)
        {
            if (!IsDisposed)
                lSpeechStatus.Text = string.Format(CultureInfo.CurrentCulture, Resources.Speech_Error, ex.Message);
        }

        if (!IsDisposed)
            bSpeechVerify.Enabled = true;
    }

    private void LlSpeechKey_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e) => Utils.OpenShell(ElevenLabsClient.ApiKeysUrl);

    private void LlAzure_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e) => Utils.OpenShell(AzureSpeechClient.PortalUrl);

    private void LlPiper_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e) => Utils.OpenShell(PiperClient.DownloadUrl);

    private void LlPiperSamples_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e) => Utils.OpenShell(PiperVoiceCatalog.SamplesUrl);

    private string PiperVoicesPath => tbPiperVoices.Text.Trim() is { Length: > 0 } folder ? folder : PiperVoices.DefaultFolder;

    /// <summary>
    /// Overi kluc a oblast Azure nacitanim zoznamu hlasov.
    /// </summary>
    private async void BAzureVerify_Click(object? sender, EventArgs e)
    {
        var key = tbAzureKey.Text.Trim();
        var region = tbAzureRegion.Text.Trim();
        if (key.Length == 0)
        {
            lAzureStatus.Text = Resources.FAppSettings_SpeechNoKey;
            return;
        }

        if (!SpeechProviders.IsValidAzureRegion(region))
        {
            lAzureStatus.Text = Resources.FAppSettings_AzureNoRegion;
            return;
        }

        bAzureVerify.Enabled = false;
        lAzureStatus.Text = Resources.FAppSettings_SpeechVerifying;
        try
        {
            using var client = new AzureSpeechClient(key, region);
            var voices = await client.GetVoicesAsync(CancellationToken.None);
            if (!IsDisposed)
                lAzureStatus.Text = string.Format(CultureInfo.CurrentCulture, Resources.FAppSettings_AzureVerified, voices.Count,
                    voices.Count(v => v.Speaks("sk")), voices.Count(v => v.Speaks("cs")));
        }
        catch (SpeechException ex)
        {
            if (!IsDisposed)
                lAzureStatus.Text = string.Format(CultureInfo.CurrentCulture, Resources.Speech_Error, ex.Message);
        }

        if (!IsDisposed)
            bAzureVerify.Enabled = true;
    }

    private void BPiperBrowse_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Filter = Resources.FAppSettings_PiperFilter,
            FileName = tbPiperPath.Text.Trim(),
            CheckFileExists = true
        };
        if (dialog.ShowDialog(this) == DialogResult.OK)
            tbPiperPath.Text = dialog.FileName;
    }

    private void BPiperVoicesBrowse_Click(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog { SelectedPath = PiperVoicesPath, ShowNewFolderButton = true };
        if (dialog.ShowDialog(this) == DialogResult.OK)
            tbPiperVoices.Text = string.Equals(dialog.SelectedPath, PiperVoices.DefaultFolder, StringComparison.OrdinalIgnoreCase) ? "" : dialog.SelectedPath;
    }

    private void BPiperDownload_Click(object? sender, EventArgs e)
    {
        using var form = new FPiperVoices(PiperVoicesPath, "");
        form.ShowDialog(this);
    }

    /// <summary>
    /// Overi Piper ukazkou prvym nainstalovanym hlasom.
    /// </summary>
    private async void BPiperVerify_Click(object? sender, EventArgs e)
    {
        var path = tbPiperPath.Text.Trim();
        if (!File.Exists(path))
        {
            lPiperStatus.Text = Resources.FAppSettings_PiperNoProgram;
            return;
        }

        bPiperVerify.Enabled = false;
        lPiperStatus.Text = Resources.FAppSettings_SpeechVerifying;
        try
        {
            var client = new PiperClient(path, PiperVoicesPath);
            var voices = await client.GetVoicesAsync(CancellationToken.None);
            if (voices.Count == 0)
            {
                lPiperStatus.Text = Resources.FAppSettings_PiperNoVoices;
            }
            else
            {
                var sound = await client.SynthesizeAsync(new SpeechRequest("Test.", voices[0].Id, PiperClient.Model), CancellationToken.None);
                if (!IsDisposed)
                    lPiperStatus.Text = string.Format(CultureInfo.CurrentCulture, Resources.FAppSettings_PiperVerified, voices.Count, sound.DurationMs);
            }
        }
        catch (SpeechException ex)
        {
            if (!IsDisposed)
                lPiperStatus.Text = string.Format(CultureInfo.CurrentCulture, Resources.Speech_Error, ex.Message);
        }

        if (!IsDisposed)
            bPiperVerify.Enabled = true;
    }

    private void BSpeechClearCache_Click(object? sender, EventArgs e)
    {
        if (SpeechCache.Default().Clear() > 0)
            Utils.ShowWarning(Resources.FAppSettings_SpeechCacheNotCleared);
        ShowSpeechCacheSize();
    }
}