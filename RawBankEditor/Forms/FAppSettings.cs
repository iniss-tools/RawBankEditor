using ExControls;
using RawBankEditor.XML;
using ToolsCore;
using ToolsCore.Forms;
using ToolsCore.Iniss.Tools;
using ToolsCore.XML;

namespace RawBankEditor.Forms;

public partial class FAppSettings : FAppSettingsBase
{
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
    }

    /// <inheritdoc />
    protected override bool OnSaving()
    {
        Config.Shortcuts.SetFromRows(Shortcuts);
        Config.DesktopCols.SetValues(Columns);
        Config.AutoRecalculateSoundDuration = cboxAutoRecalculateSoundDurations.Checked;
        Config.AutoInsertSoundData = cboxAutoInsertSoundData.Checked;
        Config.ShowAfterInsertSoundDialog = cboxShowAfterInsertSoundDlg.Checked;
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

        Styles<RawBankEditorStyle>.WriteData(PathUtils.CombinePath(configsDir, ToolsCore.FileConsts.FILE_STYLES)!, _session.Styles);
        XmlSerialization.WriteData(PathUtils.CombinePath(configsDir, ToolsCore.FileConsts.FILE_CONFIG)!, _session.Config);
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
}