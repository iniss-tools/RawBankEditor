using RawBankEditor.Tools;
using ToolsCore.Iniss.Entities;

namespace RawBankEditor.Tests;

/// <summary>
/// Import zvukov z tabulky: typy stlpcov, doplnenie hodnot, nove a existujuce skupiny, kontroly.
/// </summary>
[TestClass]
public class SoundImportTests
{
    private static readonly SoundImportColumn[] Columns =
    [
        SoundImportColumn.GroupKey, SoundImportColumn.SoundKey, SoundImportColumn.SoundName, SoundImportColumn.FileName, SoundImportColumn.Text
    ];

    private static FyzLanguage Language()
    {
        var language = new FyzLanguage("SK", "Slovenčina", "FYZZVUK.DAT", "SK\\") { Groups = [] };
        var group = new FyzGroup(language, "DZ", "Dôvody", "DZ\\");
        group.Sounds.Add(new FyzSound(group, "VNP", "Počasie", "VNP.WAV", "", "nepriaznivé počasie", 1000));
        language.Groups.Add(group);
        return language;
    }

    private static SoundImportPlan Build(FyzLanguage language, string[][] rows, bool skipExisting = true, IReadOnlyList<SoundImportColumn>? columns = null)
        => SoundImport.Build(language, rows, columns ?? Columns, skipExisting, 2);

    [TestMethod]
    [DataRow("Kľúč skupiny", "Kľúč skupiny")]
    [DataRow("KLIC ZVUKU", "Kľúč zvuku")]
    [DataRow("Názov priečinka", "Kľúč skupiny")]
    [DataRow("Text hlášení", "Text hlásenia")]
    [DataRow("file-name", "Súbor")]
    [DataRow("Poznámka", "– nepoužiť –")]
    [DataRow("", "– nepoužiť –")]
    public void ParseHeader_RozpoznaTypStlpca(string header, string expected)
    {
        Assert.AreEqual(expected, SoundImportColumn.ParseHeader(header).Name);
    }

    [TestMethod]
    public void Build_NovaSkupinaSDoplnenymiHodnotami()
    {
        var language = Language();

        var plan = Build(language, [["Slova", "a", "", "", "a"], ["Slova", "b", "Bé", "b.EWA", "bé"]]);

        Assert.IsEmpty(plan.Errors);
        var group = plan.NewGroups.Single();
        Assert.AreEqual("Slova", group.Key);
        Assert.AreEqual("Slova", group.Name);
        Assert.AreEqual("Slova\\", group.RelativePath);
        Assert.HasCount(2, group.Sounds);
        Assert.AreEqual("a", group.Sounds[0].Name);
        Assert.AreEqual("a.WAV", group.Sounds[0].FileName);
        Assert.AreEqual("Bé", group.Sounds[1].Name);
        Assert.AreEqual("b.EWA", group.Sounds[1].FileName);
        // jazyk sa nemeni - skupinu prida az hlavne okno
        Assert.HasCount(1, language.Groups);
    }

    [TestMethod]
    public void Build_ExistujucaSkupinaPodlaKlucaBezOhladuNaVelkostPismen()
    {
        var language = Language();

        var plan = Build(language, [["dz", "ZTP", "Technické", "", "technické príčiny"]]);

        Assert.IsEmpty(plan.NewGroups);
        var sound = plan.SoundsForExistingGroups.Single();
        Assert.AreSame(language.Groups[0], sound.Group);
        // zvuk sa do skupiny este nepridal
        Assert.HasCount(1, language.Groups[0].Sounds);
    }

    [TestMethod]
    public void Build_ExistujuciZvukSaPreskociAleboJeChybou()
    {
        string[][] rows = [["DZ", "vnp", "", "", ""]];

        var skipped = Build(Language(), rows);
        Assert.IsEmpty(skipped.Errors);
        Assert.IsEmpty(skipped.AllSounds);
        CollectionAssert.AreEqual(new[] { "DZ/vnp" }, skipped.Skipped);

        var error = Build(Language(), rows, skipExisting: false);
        StringAssert.Contains(error.Errors.Single(), "Riadok 2");
    }

    [TestMethod]
    public void Build_PridavnaCestaSaOddeliOdNazvuSuboru()
    {
        var plan = Build(Language(), [["N5", "01", "", "..\\N4\\01.WAV", ""]]);

        var sound = plan.AllSounds.Single();
        Assert.AreEqual("..\\N4\\", sound.AdditionalRelativePath);
        Assert.AreEqual("01.WAV", sound.FileName);
    }

    [TestMethod]
    public void Build_ChybyZastaviaCelyImportSCislomRiadku()
    {
        var plan = Build(Language(), [["Slova", "a", "", "", ""], ["", "b", "", "", ""], ["Slova", "c", "", "c.mp3", ""], ["Slova", "A", "", "", ""]]);

        // duplicitny kluc v tabulke sa hlasi pri oboch zvukoch; chyby su zoradene podla riadkov
        CollectionAssert.AreEqual(new[] { "Riadok 2:", "Riadok 3:", "Riadok 4:", "Riadok 5:" }, plan.Errors.Select(e => e[..9]).ToList());
        Assert.IsEmpty(plan.NewGroups.SelectMany(g => g.Sounds));
    }

    [TestMethod]
    public void Build_ChybajuciPovinnyStlpec()
    {
        var plan = Build(Language(), [["a", "b"]], columns: [SoundImportColumn.GroupKey, SoundImportColumn.Text]);

        StringAssert.Contains(plan.Errors.Single(), "Kľúč zvuku");
    }

    [TestMethod]
    public void Build_PrazdneRiadkySaPreskocia()
    {
        var plan = Build(Language(), [["", " ", "", "", ""], ["Slova", "a", "", "", ""]]);

        Assert.IsEmpty(plan.Errors);
        Assert.AreEqual(1, plan.AllSounds.Count());
    }

    [TestMethod]
    public void Build_NovaSkupinaSKolidujucimNazvomJeChyba()
    {
        var plan = Build(Language(), [["Nova", "a", "", "", ""]],
            columns: [SoundImportColumn.GroupKey, SoundImportColumn.SoundKey, SoundImportColumn.GroupName], skipExisting: true);
        Assert.IsEmpty(plan.Errors);

        var collision = SoundImport.Build(Language(), [["Nova", "a", "dôvody"]],
            [SoundImportColumn.GroupKey, SoundImportColumn.SoundKey, SoundImportColumn.GroupName], true);
        StringAssert.Contains(collision.Errors.Single(), "názvom");
    }
}
