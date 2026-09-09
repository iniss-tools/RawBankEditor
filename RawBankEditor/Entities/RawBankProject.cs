using ExControls;
using ToolsCore.Entities;

namespace RawBankEditor.Entities;

public class RawBankProject
{
    public string AbsPathToBank { get; init; } = null!;
    public string AbsPathToINISS { get; init; } = null!;
    public ExBindingList<FyzLanguage> Languages { get; init; } = null!;
    public Dictionary<FyzLanguage, List<IRawBankMessage>> Messages { get; init; } = null!;
}