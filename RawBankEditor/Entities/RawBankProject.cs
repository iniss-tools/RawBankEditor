using ExControls;
using ToolsCore.Entities;
using ToolsCore.Tools;

namespace RawBankEditor.Entities;

/// <summary>
/// Otvorena banka zvukov instalacie INISS - jazyky banky a spravy o problemoch v nich.
/// </summary>
public class RawBankProject
{
    /// <summary>
    /// Nacita zoznam jazykov banky v instalacii INISS. Otvorenu banku nemeni - prevezme sa az po vybere jazyka.
    /// </summary>
    public static RawBankProject Load(string pathToINISS)
    {
        ArgumentException.ThrowIfNullOrEmpty(pathToINISS);

        // banka zvukov je vzdy v podpriecinku RAWBANK instalacie INISS
        var pathToBank = pathToINISS + @"\RAWBANK\";

        return new RawBankProject
        {
            AbsPathToINISS = pathToINISS,
            AbsPathToBank = pathToBank,
            Languages = new ExBindingList<FyzLanguage>(RawBankParser.ReadFyzBankFile(pathToBank, out _)),
            Messages = new Dictionary<FyzLanguage, List<IRawBankMessage>>()
        };
    }

    public string AbsPathToBank { get; init; } = null!;
    public string AbsPathToINISS { get; init; } = null!;
    public ExBindingList<FyzLanguage> Languages { get; init; } = null!;
    public Dictionary<FyzLanguage, List<IRawBankMessage>> Messages { get; init; } = null!;
}