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
        CollectionAssert.AreEquivalent(legacy, ids);
        var shortcuts = RbeCommands.All.Where(c => c.DefaultShortcut != Shortcut.None).Select(c => c.DefaultShortcut).ToList();
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
        var config = (RawBankEditorConfig)new XmlSerializer(typeof(RawBankEditorConfig)).Deserialize(reader)!;

        Assert.AreEqual(Shortcut.CtrlShiftL, config.Shortcuts.Get(RbeCommands.AddLanguage));
        Assert.AreEqual(Shortcut.None, config.Shortcuts.Get(RbeCommands.DeleteSounds));
        Assert.AreEqual(Shortcut.CtrlS, config.Shortcuts.Get(RbeCommands.Save));
    }
}
