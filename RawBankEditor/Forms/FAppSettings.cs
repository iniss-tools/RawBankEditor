using System.Globalization;
using ExControls;
using RawBankEditor.Properties;
using RawBankEditor.Speech;
using RawBankEditor.XML;
using ToolsCore;
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

    protected override IList<CmdShortcut> DefaultShortcuts => ShortcutMap.DefaultRows(RbeCommands.All);

    protected override IList<DesktopColumn> DefaultColumns => new DesktopColumns().GetValues();

    // nastavenia programu - po ulozeni sa v nich nahradi konfiguracia a styly
    private readonly AppSession<RawBankEditorConfig, RawBankEditorStyle> _session;

    internal FAppSettings(AppSession<RawBankEditorConfig, RawBankEditorStyle> session)
        : base(session.Config, new Styles<RawBankEditorStyle>(session.Styles), session.UsingStyle, typeof(RawBankEditorStyle))
    {
        _session = session;
        InitializeComponent();

        Shortcuts = new ExBindingList<CmdShortcut>(Config.Shortcuts.ToRows(RbeCommands.All));
        Columns = new ExBindingList<DesktopColumn>(Config.DesktopCols.GetValues());

        dgvShortcuts.DataSource = Shortcuts;
        dgvColumns.DataSource = Columns;
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        cboxAutoRecalculateSoundDurations.Checked = Config.AutoRecalculateSoundDuration;
        cboxAutoInsertSoundData.Checked = Config.AutoInsertSoundData;
        cboxShowAfterInsertSoundDlg.Checked = Config.ShowAfterInsertSoundDialog;
        tbSpeechKey.Text = Config.Speech.ElevenLabsKey;
        nudSpeechConcurrency.Value = Math.Clamp(Config.Speech.Concurrency, (int)nudSpeechConcurrency.Minimum, (int)nudSpeechConcurrency.Maximum);
        ShowSpeechCacheSize();
    }

    /// <inheritdoc />
    protected override bool OnSaving()
    {
        Config.Shortcuts.SetFromRows(Shortcuts);
        Config.DesktopCols.SetValues(Columns);
        Config.AutoRecalculateSoundDuration = cboxAutoRecalculateSoundDurations.Checked;
        Config.AutoInsertSoundData = cboxAutoInsertSoundData.Checked;
        Config.ShowAfterInsertSoundDialog = cboxShowAfterInsertSoundDlg.Checked;
        Config.Speech.ElevenLabsKey = tbSpeechKey.Text;
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

    private void BSpeechClearCache_Click(object? sender, EventArgs e)
    {
        if (SpeechCache.Default().Clear() > 0)
            Utils.ShowWarning(Resources.FAppSettings_SpeechCacheNotCleared);
        ShowSpeechCacheSize();
    }
}