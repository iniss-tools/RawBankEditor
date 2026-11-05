using System.Diagnostics.CodeAnalysis;
using ExControls;
using RawBankEditor.Services;
using ToolsCore.Iniss.Entities;
using ToolsCore.Iniss2;

namespace RawBankEditor.Entities;

/// <summary>
/// Otvorena banka zvukov instalacie INISS - jazyky banky a spravy o problemoch v nich.
/// </summary>
public class RawBankProject
{
    /// <summary>
    /// Nacita zoznam jazykov banky v instalacii INISS. Otvorenu banku nemeni - prevezme sa az po vybere jazyka.
    /// </summary>
    /// <param name="pathToINISS">priecinok INISS 3.39 alebo priecinok INISS2 (koren instalacie alebo priecinok servera)</param>
    public static RawBankProject Load(string pathToINISS)
    {
        ArgumentException.ThrowIfNullOrEmpty(pathToINISS);

        if (Iniss2Installation.Find(pathToINISS) is { } installation)
            return Load(installation.ServerDir, installation.RawBankDir, new BankXmlStore());

        // banka zvukov je v INISS 3.39 vzdy v podpriecinku RAWBANK instalacie
        return Load(pathToINISS, pathToINISS + @"\RAWBANK\", BankDatStore.Instance);
    }

    [SuppressMessage("Performance", "CA1859:Use concrete types when possible for improved performance")]
    private static RawBankProject Load(string pathToINISS, string pathToBank, IBankStore store) => new()
    {
        AbsPathToINISS = pathToINISS,
        AbsPathToBank = pathToBank,
        Store = store,
        Languages = new ExBindingList<FyzLanguage>(store.ReadLanguages(pathToBank)),
        Messages = []
    };

    public string AbsPathToBank { get; init; } = null!;

    /// <summary>
    /// Priecinok INISS, v ktorom je banka - v INISS2 priecinok servera. Vedla banky su priecinky programu (zurnal, .rbe-ai).
    /// </summary>
    public string AbsPathToINISS { get; init; } = null!;

    /// <summary>
    /// Sposob ulozenia banky na disku.
    /// </summary>
    public IBankStore Store { get; init; } = BankDatStore.Instance;

    public ExBindingList<FyzLanguage> Languages { get; init; } = null!;
    public Dictionary<FyzLanguage, List<IRawBankMessage>> Messages { get; init; } = null!;
}