using System.Xml.Serialization;

namespace RawBankEditor.Tools;

/// <summary>
/// Nastavenia upravy zvuku nahravok (ticho na okrajoch, hlasitost, format) - pouziva ich okno Upravit zvuk aj uprava
/// nahravok vygenerovanych hlasom AI. Ukladaju sa v konfiguracii programu.
/// </summary>
public sealed class SoundAdjustOptions
{
    /// <summary>Orezat ticho na zaciatku a konci a nechat presne <see cref="LeadMs" /> a <see cref="TailMs" />.</summary>
    [XmlAttribute("TrimSilence")]
    [DefaultValue(true)]
    public bool TrimSilence { get; set; } = true;

    /// <summary>Uroven v dBFS, pod ktorou sa usek povazuje za ticho.</summary>
    [XmlAttribute("SilenceThreshold")]
    [DefaultValue(-45d)]
    public double SilenceThresholdDb { get; set; } = -45;

    /// <summary>Ticho pred zaciatkom reci v milisekundach.</summary>
    [XmlAttribute("LeadMs")]
    [DefaultValue(30)]
    public int LeadMs { get; set; } = 30;

    /// <summary>Ticho za koncom reci v milisekundach.</summary>
    [XmlAttribute("TailMs")]
    [DefaultValue(80)]
    public int TailMs { get; set; } = 80;

    /// <summary>Zjednotit hlasitost reci na <see cref="TargetLevelDb" />.</summary>
    [XmlAttribute("Normalize")]
    [DefaultValue(true)]
    public bool Normalize { get; set; } = true;

    /// <summary>Cielova hlasitost reci (RMS useku bez ticha) v dBFS.</summary>
    [XmlAttribute("TargetLevel")]
    [DefaultValue(-20d)]
    public double TargetLevelDb { get; set; } = -20;

    /// <summary>Previest na format banky - PCM 16 bit, mono, 22 050 Hz. Inak ostava vzorkovacia frekvencia nahravky.</summary>
    [XmlAttribute("ConvertFormat")]
    [DefaultValue(true)]
    public bool ConvertFormat { get; set; } = true;

    /// <summary>Dlzka postupneho nabehu a dobehu na okrajoch v milisekundach (proti luskaniu); 0 = bez.</summary>
    [XmlAttribute("FadeMs")]
    [DefaultValue(5)]
    public int FadeMs { get; set; } = 5;

    /// <summary>Kopia.</summary>
    public SoundAdjustOptions Clone() => (SoundAdjustOptions)MemberwiseClone();
}
