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

    [TestMethod]
    public void ValidateNew_KlucSaPorovnavaBezOhladuNaVelkostPismen()
    {
        var group = Group("Slova");
        group.Sounds.Add(new FyzSound(group, "prich", "prich", "prich.WAV", "", "", 0));
        var sound = Sound(group, "PRICH", "iny");

        var problems = SoundRules.ValidateNew([sound]);

        Assert.IsTrue(problems.ContainsKey(sound));
    }

    [TestMethod]
    public void ValidateMove_KolizieKlucaNazvuASuboru()
    {
        var bank = Path.Combine(Path.GetTempPath(), "RawBankEditorTests_" + Guid.NewGuid().ToString("N")) + "\\";
        try
        {
            var r1 = Group("R1");
            var r2 = Group("R2");
            r2.Sounds.Clear();
            Directory.CreateDirectory(r1.GetAbsPath(bank));
            Directory.CreateDirectory(r2.GetAbsPath(bank));

            var free = new FyzSound(r1, "9900160", "9900160", "9900160.WAV", "", "", 0);
            File.WriteAllText(free.GetAbsPath(bank), "x");
            Assert.AreEqual(0, SoundRules.ValidateMove([free], r2, bank).Count);

            // v cielovom priecinku uz je subor s rovnakym menom
            File.WriteAllText(Path.Combine(r2.GetAbsPath(bank), "9900160.wav"), "x");
            StringAssert.Contains(SoundRules.ValidateMove([free], r2, bank)[0], "súbor 9900160.WAV");

            // kluc uz v cielovej skupine je
            r2.Sounds.Add(new FyzSound(r2, "9900100", "iny", "iny.WAV", "", "", 0));
            var clash = new FyzSound(r1, "9900100", "9900100", "chyba.WAV", "", "", 0);
            StringAssert.Contains(SoundRules.ValidateMove([clash], r2, bank)[0], "Kľúč 9900100");
        }
        finally
        {
            if (Directory.Exists(bank))
                Directory.Delete(bank, true);
        }
    }

    [TestMethod]
    public void ValidateMove_ChybajuciPriecinokCielovejSkupiny()
    {
        var bank = Path.Combine(Path.GetTempPath(), "RawBankEditorTests_" + Guid.NewGuid().ToString("N")) + "\\";
        try
        {
            var r1 = Group("R1");
            var r2 = Group("R2");
            r2.Sounds.Clear();
            Directory.CreateDirectory(r1.GetAbsPath(bank));
            var sound = new FyzSound(r1, "9900160", "9900160", "9900160.WAV", "", "", 0);
            File.WriteAllText(sound.GetAbsPath(bank), "x");

            StringAssert.Contains(SoundRules.ValidateMove([sound], r2, bank)[0], "neexistuje");

            // nahravka s pridavnou cestou sa nepresuva - priecinok cielovej skupiny netreba
            var shared = new FyzSound(r1, "9900170", "9900170", "9900160.WAV", ".\\", "", 0) { AdditionalRelativePath = "..\\R1\\" };
            Assert.AreEqual(0, SoundRules.ValidateMove([shared], r2, bank).Count);
        }
        finally
        {
            if (Directory.Exists(bank))
                Directory.Delete(bank, true);
        }
    }

    [TestMethod]
    [DataRow("R1", @"C:\INISS\RAWBANK\SK\R1", "")]
    [DataRow("R2", @"C:\INISS\RAWBANK\SK\R1", "..\\R1\\")]
    [DataRow("R2", @"C:\INISS\RAWBANK\SK\Poz1", "..\\Poz1\\")]
    [DataRow("R2", @"C:\INISS\RAWBANK\SK\R2\stare", "stare\\")]
    public void AdditionalPathFor_RelativnaKPriecinkuSkupiny(string group, string fileDir, string expected)
    {
        Assert.AreEqual(expected, SoundRules.AdditionalPathFor(Group(group), fileDir, @"C:\INISS\RAWBANK\"));
    }
}
