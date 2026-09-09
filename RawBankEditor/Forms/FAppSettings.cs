using ExControls;
using RawBankEditor.XML;
using ToolsCore;
using ToolsCore.Forms;
using ToolsCore.Tools;
using ToolsCore.XML;

namespace RawBankEditor.Forms;

public partial class FAppSettings : FAppSettingsBase
{
    public new RawBankEditorConfig Config => (RawBankEditorConfig)base.Config;
    public new Styles<RawBankEditorStyle> Styles => (Styles<RawBankEditorStyle>)base.Styles;

    protected override IList<CmdShortcut> DefaultShortcuts => new AppShortcuts().GetValues();
    
    protected override IList<DesktopColumn> DefaultColumns => new DesktopColumns().GetValues();

    public FAppSettings(RawBankEditorConfig config, Styles<RawBankEditorStyle> styles) 
        : base(config, new Styles<RawBankEditorStyle>(styles), GlobData.UsingStyle, typeof(RawBankEditorStyle))
    {
        InitializeComponent();
        
        Shortcuts = new ExBindingList<CmdShortcut>(Config.Shortcuts.GetValues());
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
        base.OnSaving();
        Config.Shortcuts.SetValues(Shortcuts);
        Config.DesktopCols.SetValues(Columns);
        Config.AutoRecalculateSoundDuration = cboxAutoRecalculateSoundDurations.Checked;
        Config.AutoInsertSoundData = cboxAutoInsertSoundData.Checked;
        Config.ShowAfterInsertSoundDialog = cboxShowAfterInsertSoundDlg.Checked;
        return true;
    }

    /// <inheritdoc />
    protected override void SaveData()
    {
        GlobData.Config = Config;
        GlobData.UsingStyle = (RawBankEditorStyle)UsingStyle;
        GlobSettings.UsingStyle = UsingStyle;
        GlobData.Styles = Styles;
        
        var configsDir = Utils.CombinePath(Application.StartupPath, ToolsCore.FileConsts.CONFIG_PATH)!;
        if (!Directory.Exists(configsDir))
            Directory.CreateDirectory(configsDir);

        Styles<RawBankEditorStyle>.WriteData(Utils.CombinePath(configsDir, ToolsCore.FileConsts.FILE_STYLES)!, GlobData.Styles);
        XmlSerialization.WriteData(Utils.CombinePath(configsDir, ToolsCore.FileConsts.FILE_CONFIG)!, GlobData.Config);
    }

    /// <inheritdoc />
    protected override Style CreateStyleInstance(string name) => new RawBankEditorStyle {Name = name};

    /// <inheritdoc />
    protected override Style OnResetStyle(bool darkMode) => Styles<RawBankEditorStyle>.GetDefaultStyle(darkMode);

    private void CboxAutoInsertSoundData_CheckedChanged(object sender, EventArgs e)
    {
        //AutoInsertSoundData musi byt true, aby toto nastavenie fungovalo
        cboxShowAfterInsertSoundDlg.Checked = cboxAutoInsertSoundData.Checked;
        cboxShowAfterInsertSoundDlg.Enabled = cboxAutoInsertSoundData.Checked;
    }
}