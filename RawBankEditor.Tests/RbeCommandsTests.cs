using System.Xml;
using System.Windows.Forms;
using System.Xml.Serialization;
using RawBankEditor.Forms;
using RawBankEditor.XML;

namespace RawBankEditor.Tests;

/// <summary>
/// Prikazy hlavneho okna a ich skratky v config.xml.
/// </summary>
[TestClass]
public class RbeCommandsTests
{
    [TestMethod]
    public void All_ObsahujePrvkySkratiekZoStarsichVerziiBezDuplicit()
    {
        string[] legacy =
        [
            "Open", "Save", "SaveAll", "Undo", "Redo", "AddSound", "DeleteSounds", "MoveSounds", "RewriteMode", "WrapTextSoundCol",
            "GoBack", "GoForward", "Search", "AddLang", "EditLang", "DeleteLang", "AppSettings", "HighlightProblem", "ResolveProblem"
        ];

        var ids = RbeCommands.All.Select(c => c.Id).ToList();
        CollectionAssert.IsSubsetOf(legacy, ids);
        Assert.HasCount(ids.Count, ids.Distinct().ToList());
        var shortcuts = RbeCommands.All.Where(c => c.DefaultKeys != Keys.None).Select(c => c.DefaultKeys).ToList();
        Assert.HasCount(shortcuts.Count, shortcuts.Distinct().ToList());
    }

    [TestMethod]
    public void Config_StarySuborNastaveni_NacitaZmeneneSkratky()
    {
        const string xml = """
            <CONFIG>
              <Shortcuts>
                <AddLang sc="CtrlShiftL" />
                <DeleteSounds sc="None" />
              </Shortcuts>
            </CONFIG>
            """;

        using var reader = new StringReader(xml);
        var config = (RawBankEditorConfig)new XmlSerializer(typeof(RawBankEditorConfig)).Deserialize(XmlReader.Create(reader))!;

        Assert.AreEqual((Keys)Shortcut.CtrlShiftL, config.Shortcuts.Get(RbeCommands.AddLanguage));
        Assert.AreEqual((Keys)Shortcut.None, config.Shortcuts.Get(RbeCommands.DeleteSounds));
        Assert.AreEqual((Keys)Shortcut.CtrlS, config.Shortcuts.Get(RbeCommands.Save));
    }
}
