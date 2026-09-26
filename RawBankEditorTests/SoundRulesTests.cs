using RawBankEditor.Tools;
using ToolsCore.Entities;

namespace RawBankEditorTests;

[TestClass]
public class SoundRulesTests
{
    private static FyzGroup Group(string key = "R1")
    {
        var lang = new FyzLanguage("SK", "Slovenčina", "FYZZVUK.DAT", "SK\\");
        var group = new FyzGroup(lang, key, key, key + "\\");
        group.Sounds.Add(new FyzSound(group, "9900100", "9900100", "9900100.EWA", "", "Dolné Mesto", 900));
        return group;
    }

    private static FyzSound Sound(FyzGroup group, string key, string? name = null) =>
        new(group, key, name ?? key, key + ".WAV", "", "", 900);

    [TestMethod]
    public void ValidateNew_NovyKlucJeVPoriadku()
    {
        var group = Group();

        var problems = SoundRules.ValidateNew([Sound(group, "9900160")]);

        Assert.AreEqual(0, problems.Count);
    }

    [TestMethod]
    public void ValidateNew_KlucExistujuciVSkupineJeChyba()
    {
        // 9900100.WAV nakopirovany do skupiny, kde uz je zvuk 9900100 so suborom .EWA
        var group = Group();
        var sound = Sound(group, "9900100", "iny nazov");

        var problems = SoundRules.ValidateNew([sound]);

        StringAssert.Contains(problems[sound], "Kľúč 9900100");
    }

    [TestMethod]
    public void ValidateNew_RovnakyNazovJeChyba()
    {
        var group = Group();
        var sound = Sound(group, "9900160", "9900100");

        var problems = SoundRules.ValidateNew([sound]);

        StringAssert.Contains(problems[sound], "Názov 9900100");
    }

    [TestMethod]
    public void ValidateNew_DuplicitaMedziNovymiZvukmi()
    {
        var group = Group();
        var first = Sound(group, "9900160");
        var second = Sound(group, "9900160", "druhy");

        var problems = SoundRules.ValidateNew([first, second]);

        Assert.IsTrue(problems.ContainsKey(first));
        Assert.IsTrue(problems.ContainsKey(second));
    }

    [TestMethod]
    public void ValidateNew_RovnakyKlucVInejSkupineNevadi()
    {
        var r1 = Group();
        var r2 = Group("R2");
        r2.Sounds.Clear();

        var problems = SoundRules.ValidateNew([Sound(r1, "9900160"), Sound(r2, "9900160"), Sound(r2, "9900100")]);

        Assert.AreEqual(0, problems.Count);
    }

    [TestMethod]
    [DataRow("", "nazov")]
    [DataRow("kluc", " ")]
    public void ValidateNew_PrazdnyKlucAleboNazovJeChyba(string key, string name)
    {
        var group = Group();
        var sound = new FyzSound(group, key, name, "x.WAV", "", "", 0);

        var problems = SoundRules.ValidateNew([sound]);

        StringAssert.Contains(problems[sound], "povinné");
    }
}
