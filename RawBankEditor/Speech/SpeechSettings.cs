using System.Xml.Serialization;
using ToolsCore.Tools;

namespace RawBankEditor.Speech;

/// <summary>
/// Nastavenia hlasovej syntezy v konfiguracii programu - sluzby (ElevenLabs, Azure Speech, Piper) a sposob generovania.
/// </summary>
public sealed class SpeechSettings
{
    /// <summary>Najvyssi pocet sucasnych poziadaviek (plan Free dovoli 2, platene viac).</summary>
    public const int MaxConcurrency = 10;

    /// <summary>Kluc ElevenLabs sifrovany pre aktualneho pouzivatela Windows (DPAPI) - v configu nie je citatelny.</summary>
    [XmlAttribute("ElevenLabsKey")]
    public string ProtectedElevenLabsKey { get; set; } = "";

    /// <summary>
    /// Kluc API ElevenLabs; prazdny = synteza nie je nastavena. Kluc ulozeny inym pouzivatelom alebo na inom pocitaci sa
    /// neda precitat - je prazdny.
    /// </summary>
    [XmlIgnore]
    public string ElevenLabsKey
    {
        get => UserSecret.Unprotect(ProtectedElevenLabsKey, Entropy);
        set => ProtectedElevenLabsKey = UserSecret.Protect(value, Entropy);
    }

    /// <summary>Kluc Azure Speech sifrovany pre aktualneho pouzivatela Windows (DPAPI).</summary>
    [XmlAttribute("AzureKey")]
    public string ProtectedAzureKey { get; set; } = "";

    /// <summary>
    /// Kluc prostriedku Azure Speech; prazdny = Azure nie je nastavene.
    /// </summary>
    [XmlIgnore]
    public string AzureKey
    {
        get => UserSecret.Unprotect(ProtectedAzureKey, AzureEntropy);
        set => ProtectedAzureKey = UserSecret.Protect(value, AzureEntropy);
    }

    /// <summary>Oblast prostriedku Azure Speech (napr. <c>westeurope</c>) alebo jeho adresa.</summary>
    [XmlAttribute("AzureRegion")]
    [DefaultValue("")]
    public string AzureRegion { get; set; } = "";

    /// <summary>Program Piper (<c>piper.exe</c>); prazdny = Piper nie je nastaveny.</summary>
    [XmlAttribute("PiperPath")]
    [DefaultValue("")]
    public string PiperPath { get; set; } = "";

    /// <summary>Priecinok hlasov Piper; prazdny = <see cref="PiperVoices.DefaultFolder" />.</summary>
    [XmlAttribute("PiperVoices")]
    [DefaultValue("")]
    public string PiperVoicesFolder { get; set; } = "";

    /// <summary>
    /// Priecinok hlasov Piper - nastaveny alebo predvoleny.
    /// </summary>
    [XmlIgnore]
    public string PiperVoicesPath => PiperVoicesFolder.Length > 0 ? PiperVoicesFolder : PiperVoices.DefaultFolder;

    /// <summary>Pocet sucasnych poziadaviek pri generovani viacerych nahravok.</summary>
    [XmlAttribute("Concurrency")]
    [DefaultValue(2)]
    public int Concurrency { get; set; } = 2;

    /// <summary>Vygenerovane nahravky upravit ako v okne Upravit zvuk (ticho na okrajoch, hlasitost).</summary>
    [XmlAttribute("Adjust")]
    [DefaultValue(true)]
    public bool AdjustSound { get; set; } = true;

    /// <summary>Vygenerovane nahravky kontrolovat prepisom na text.</summary>
    [XmlAttribute("Verify")]
    [DefaultValue(true)]
    public bool Verify { get; set; } = true;

    /// <summary>Kopia.</summary>
    public SpeechSettings Clone() => (SpeechSettings)MemberwiseClone();

    private static readonly byte[] Entropy = "RawBankEditor.ElevenLabs.ApiKey"u8.ToArray();
    private static readonly byte[] AzureEntropy = "RawBankEditor.Azure.ApiKey"u8.ToArray();
}
