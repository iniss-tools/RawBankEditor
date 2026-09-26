using ExControls;
using RawBankEditor.Entities;
using RawBankEditor.XML;
using ToolsCore.Entities;
using ToolsCore.Tools;
using ToolsCore.XML;

namespace RawBankEditor;

internal static class GlobData
{
    public static RawBankProject? OpenedProject;
    public static RawBankEditorConfig Config = null!;
    public static Styles<RawBankEditorStyle> Styles = null!;
    public static RawBankEditorStyle UsingStyle = null!;

    /// <summary>
    ///     Nacita zoznam jazykov banky v instalacii INISS. Otvorenu banku nemeni - prevezme sa az po vybere jazyka.
    /// </summary>
    public static RawBankProject LoadProject(string pathToINISS)
    {
        if (string.IsNullOrEmpty(pathToINISS))
            throw new ArgumentNullException(nameof(pathToINISS));

        var pathToBank = pathToINISS + FileConsts.DIR_RAWBANK;

        return new RawBankProject
        {
            AbsPathToINISS = pathToINISS,
            AbsPathToBank = pathToBank,
            Languages = new ExBindingList<FyzLanguage>(RawBankParser.ReadFyzBankFile(pathToBank, out _)),
            Messages = new Dictionary<FyzLanguage, List<IRawBankMessage>>()
        };
    }
}