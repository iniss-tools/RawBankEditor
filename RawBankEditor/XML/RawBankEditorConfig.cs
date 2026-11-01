using System.Xml.Serialization;
using RawBankEditor.Speech;
using RawBankEditor.Tools;
using ToolsCore.XML;

namespace RawBankEditor.XML;

/// <summary>
/// Konfiguracny subor
/// </summary>
[XmlRoot("CONFIG")]
public record RawBankEditorConfig() : ConfigBase
{


    [XmlElement("ShowErrorsWindow")]
    [DefaultValue(true)]
    public bool ShowErrorsWindow { get; set; } = true;

    [XmlElement("LeftPanelWidth")]
    [DefaultValue(-1)]
    public int LeftPanelWidth { get; set; } = -1;

    [XmlElement("GroupPanelWidth")]
    [DefaultValue(-1)]
    public int GroupPanelWidth { get; set; } = -1;

    [XmlElement("ErrorPanelWidth")]
    [DefaultValue(-1)]
    public int ErrorPanelWidth { get; set; } = -1;

    [XmlElement("AutoRecalculateSoundDuration")]
    [DefaultValue(true)]
    public bool AutoRecalculateSoundDuration { get; set; } = true;

    [XmlElement("AutoInsertSoundData")]
    [DefaultValue(true)]
    public bool AutoInsertSoundData { get; set; } = true;

    [XmlElement("ShowAfterInsertSoundDlg")]
    [DefaultValue(true)]
    public bool ShowAfterInsertSoundDialog { get; set; } = true;

    [XmlElement("WrapSoundText")]
    [DefaultValue(true)]
    public bool WrapSoundText { get; set; } = true;

    /// <summary>
    /// Uprava zvuku nahravok (okno Upravit zvuk a uprava vygenerovanych nahravok).
    /// </summary>
    [XmlElement("SoundAdjust")]
    public SoundAdjustOptions SoundAdjust { get; set; } = new();

    /// <summary>
    /// Hlasova synteza (ElevenLabs).
    /// </summary>
    [XmlElement("Speech")]
    public SpeechSettings Speech { get; set; } = new();

    /// <inheritdoc />
    public override string LinkAppSettingsGuide => LinkConsts.AppSettings;

    protected RawBankEditorConfig(RawBankEditorConfig original) : base(original)
    {
        ShowErrorsWindow = original.ShowErrorsWindow;
        LeftPanelWidth = original.LeftPanelWidth;
        GroupPanelWidth = original.GroupPanelWidth;
        ErrorPanelWidth = original.ErrorPanelWidth;
        AutoRecalculateSoundDuration = original.AutoRecalculateSoundDuration;
        AutoInsertSoundData = original.AutoInsertSoundData;
        ShowAfterInsertSoundDialog = original.ShowAfterInsertSoundDialog;
        WrapSoundText = original.WrapSoundText;
        SoundAdjust = original.SoundAdjust.Clone();
        Speech = original.Speech.Clone();
    }
}