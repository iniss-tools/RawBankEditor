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

    // rozlozenie panelov hlavneho okna je v ConfigBase.DockLayout (predtym ShowErrorsWindow a sirky panelov - starsi
    // config.xml ich obsahuje, XmlSerializer nezname prvky vynecha)

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
        AutoRecalculateSoundDuration = original.AutoRecalculateSoundDuration;
        AutoInsertSoundData = original.AutoInsertSoundData;
        ShowAfterInsertSoundDialog = original.ShowAfterInsertSoundDialog;
        WrapSoundText = original.WrapSoundText;
        SoundAdjust = original.SoundAdjust.Clone();
        Speech = original.Speech.Clone();
    }
}