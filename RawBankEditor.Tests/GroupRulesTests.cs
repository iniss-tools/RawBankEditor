using RawBankEditor.Tools;
using ToolsCore.Entities;

namespace RawBankEditor.Tests;

[TestClass]
public class GroupRulesTests
{
    private static List<FyzGroup> Groups()
    {
        var lang = new FyzLanguage("SK", "Slovenčina", "FYZZVUK.DAT", "SK\\");
        return
        [
            new FyzGroup(lang, "R1", "Stanice", "R1\\"),
            new FyzGroup(lang, "Slova", "Slová", "Slova\\")
        ];
    }

    [TestMethod]
    public void Validate_NovaSkupinaJeVPoriadku()
    {
        Assert.IsNull(GroupRules.Validate(Groups(), null, "Znelky", "Znelky", "Znelky\\"));
    }

    [TestMethod]
    public void Validate_UpravaBezZmenyJeVPoriadku()
    {
        // upravovana skupina sa neporovnava sama so sebou
        var groups = Groups();

        Assert.IsNull(GroupRules.Validate(groups, groups[0], "R1", "Stanice", "R1\\"));
    }

    [TestMethod]
    [DataRow("r1", "Iny", "Iny\\", "kľúčom")]
    [DataRow("Iny", "slová", "Iny\\", "názvom")]
    [DataRow("Iny", "Iny", "slova\\", "cestou")]
    public void Validate_DuplicitaBezOhladuNaVelkostPismenJeChyba(string key, string name, string path, string expected)
    {
        StringAssert.Contains(GroupRules.Validate(Groups(), null, key, name, path), expected);
    }

    [TestMethod]
    [DataRow("", "Znelky", "Znelky\\", "Nie sú vyplnené")]
    [DataRow("Znelky", "Znelky", "Znelky", "končiť")]
    [DataRow("Znelky", "Znelky", "Hudba\\Znelky\\", "jedného priečinka")]
    [DataRow("Znelky", "Znelky", "..\\", "jedného priečinka")]
    [DataRow("Znelky", "Znelky", "Zne:lky\\", "jedného priečinka")]
    public void Validate_NespravneHodnotyJeChyba(string key, string name, string path, string expected)
    {
        StringAssert.Contains(GroupRules.Validate(Groups(), null, key, name, path), expected);
    }

    [TestMethod]
    public void DefaultRelativePath_KlucSLomkou()
    {
        Assert.AreEqual("Znelky\\", GroupRules.DefaultRelativePath(" Znelky "));
        Assert.AreEqual("", GroupRules.DefaultRelativePath(" "));
    }

    [TestMethod]
    public void FyzGroup_TypSaMeniSKlucom()
    {
        var group = Groups()[1];

        group.Key = "R1";

        Assert.AreEqual(FyzGroupType.R1, group.Type);
    }
}
