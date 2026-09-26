using RawBankEditor.Tools;
using ToolsCore.Entities;
using ToolsCore.Tools;

namespace RawBankEditorTests;

[TestClass]
public class LanguageRulesTests
{
    private string _bank = null!;

    [TestInitialize]
    public void Init()
    {
        _bank = Path.Combine(Path.GetTempPath(), "RawBankEditorTests_" + Guid.NewGuid().ToString("N")) + "\\";
        Directory.CreateDirectory(_bank);
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (Directory.Exists(_bank))
            Directory.Delete(_bank, true);
    }

    private static List<FyzLanguage> Languages() =>
    [
        new("SK", "Slovenčina", "FYZZVUK.DAT", "SK\\"),
        new("CZ", "Čeština", "FYZZVUK.DAT", "CZ\\")
    ];

    [TestMethod]
    [DataRow("SK", "SK\\")]
    [DataRow(" GB ", "GB\\")]
    [DataRow("", "")]
    public void DefaultRelativePath_KonciLomkouPodlaKluca(string key, string expected)
    {
        Assert.AreEqual(expected, LanguageRules.DefaultRelativePath(key));
    }

    [TestMethod]
    public void Validate_PorovnavaSJazykmiBanky()
    {
        var langs = Languages();

        Assert.IsNull(LanguageRules.Validate(langs, null, "GB", "Angličtina", "GB\\"));
        Assert.Contains("kľúčom", LanguageRules.Validate(langs, null, "sk", "Iný", "X\\")!);
        Assert.Contains("názvom", LanguageRules.Validate(langs, null, "GB", "čeština", "GB\\")!);
        Assert.Contains("cestou", LanguageRules.Validate(langs, null, "GB", "Angličtina", "cz\\")!);
    }

    [TestMethod]
    public void Validate_PriUpraveIgnorujeUpravovanyJazyk()
    {
        var langs = Languages();

        Assert.IsNull(LanguageRules.Validate(langs, langs[0], "SK", "Slovenčina", "SK\\"));
        Assert.IsNull(LanguageRules.Validate(langs, langs[0], "SK", "Slovensky", "SK2\\"));
        Assert.IsNotNull(LanguageRules.Validate(langs, langs[0], "CZ", "Slovenčina", "SK\\"));
    }

    [TestMethod]
    public void Validate_FungujeAjBezJazykov()
    {
        Assert.IsNull(LanguageRules.Validate([], null, "SK", "Slovenčina", "SK\\"));
    }

    [TestMethod]
    [DataRow("SK", "Slovenčina", "", "vyplnené")]
    [DataRow("", "Slovenčina", "SK\\", "vyplnené")]
    [DataRow("SK", "Slovenčina", "SK", "končiť")]
    [DataRow("SK", "Slovenčina", "\\", "priečinok")]
    [DataRow("SK", "Slovenčina", "C:\\SK\\", "priečinok")]
    [DataRow("SK", "Slovenčina", "..\\SK\\", "priečinok")]
    [DataRow("SK", "Slovenčina", "S*K\\", "priečinok")]
    [DataRow("SK", "Slovenčina", "SK\\\\", "priečinok")]
    public void Validate_OdmietneNeplatneHodnoty(string key, string name, string path, string expected)
    {
        Assert.Contains(expected, LanguageRules.Validate([], null, key, name, path)!);
    }

    [TestMethod]
    public void Validate_PrijmeVnorenuCestu()
    {
        Assert.IsNull(LanguageRules.Validate([], null, "SK", "Slovenčina", "JAZYKY\\SK\\"));
    }

    [TestMethod]
    public void SoundsDiffer_NovyJazykBezSuboruSaLisiLenSoSkupinami()
    {
        var lang = new FyzLanguage("GB", "Angličtina", "FYZZVUK.DAT", "GB\\");
        Assert.IsFalse(LanguageRules.SoundsDiffer(_bank, lang), "nenacitany jazyk");

        lang.Groups = new List<FyzGroup>();
        Assert.IsFalse(LanguageRules.SoundsDiffer(_bank, lang));

        lang.Groups.Add(new FyzGroup(lang, "R1", "Stanice", "R1\\"));
        Assert.IsTrue(LanguageRules.SoundsDiffer(_bank, lang));
    }

    [TestMethod]
    public void SoundsDiffer_PorovnaZvukySUlozenymSuborom()
    {
        var lang = new FyzLanguage("SK", "Slovenčina", "FYZZVUK.DAT", "SK\\") { Groups = new List<FyzGroup>() };
        var group = new FyzGroup(lang, "R1", "Stanice", "R1\\");
        group.Sounds.Add(new FyzSound(group, "9900100", "Dolné Mesto", "9900100.EWA", "", "Dolné Mesto", 900));
        group.Sounds.Add(new FyzSound(group, "9900110", "Horná Ves", "X.WAV", "CISLO\\", "Horná Ves", 800));
        lang.Groups.Add(group);

        Directory.CreateDirectory(lang.GetAbsPath(_bank));
        RawBankParser.WriteFyzZvukFile(_bank, lang);
        Assert.IsFalse(LanguageRules.SoundsDiffer(_bank, lang));

        group.Sounds[0].Text = "Dolné Mesto zastávka";
        Assert.IsTrue(LanguageRules.SoundsDiffer(_bank, lang));
        group.Sounds[0].Text = "Dolné Mesto";

        group.Sounds.RemoveAt(1);
        Assert.IsTrue(LanguageRules.SoundsDiffer(_bank, lang));
    }

    [TestMethod]
    public void BankDiffers_PorovnaZoznamJazykovSFyzbank()
    {
        var langs = Languages();
        Assert.IsTrue(LanguageRules.BankDiffers(_bank, langs), "chyba FYZBANK.DAT");

        RawBankParser.WriteFyzBankFile(_bank, langs);
        Assert.IsFalse(LanguageRules.BankDiffers(_bank, langs));

        langs[1].Name = "Česky";
        Assert.IsTrue(LanguageRules.BankDiffers(_bank, langs));

        RawBankParser.WriteFyzBankFile(_bank, langs);
        langs.Add(new FyzLanguage("GB", "Angličtina"));
        Assert.IsTrue(LanguageRules.BankDiffers(_bank, langs));

        RawBankParser.WriteFyzBankFile(_bank, []);
        Assert.IsFalse(LanguageRules.BankDiffers(_bank, []), "prazdna banka");
    }
}
