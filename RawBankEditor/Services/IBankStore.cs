using ToolsCore.Iniss.Entities;
using ToolsCore.Iniss.Tools;

namespace RawBankEditor.Services;

/// <summary>
/// Sposob ulozenia banky zvukov na disku: INISS 3.39 (<see cref="BankDatStore" />) alebo INISS2 (<see cref="BankXmlStore" />).
/// </summary>
public interface IBankStore
{
    /// <summary>
    /// Banka INISS2 - nazvy skupin a zvukov neuklada a ma predvoleny jazyk.
    /// </summary>
    bool IsIniss2 { get; }

    /// <summary>
    /// Nazov suboru so zvukmi noveho jazyka.
    /// </summary>
    string LanguageFileName { get; }

    /// <summary>
    /// Precita zoznam jazykov banky bez skupin a zvukov.
    /// </summary>
    List<FyzLanguage> ReadLanguages(string pathToBank);

    /// <summary>
    /// Precita skupiny a zvuky jazyka.
    /// </summary>
    /// <param name="pathToBank">priecinok banky</param>
    /// <param name="language">jazyk</param>
    /// <param name="progress">priebeh citania (pri citani na pozadi)</param>
    void ReadSounds(string pathToBank, FyzLanguage language, IProgress<ProgressStatus>? progress = null);

    /// <summary>
    /// Zapamata si stav nahravok nacitaneho jazyka - pri ulozeni sa podla neho zisti, ktore sa zmenili.
    /// </summary>
    void RememberRecordings(string pathToBank, FyzLanguage language);

    /// <summary>
    /// Subory, ktore ulozenie moze zmenit - pri chybe sa vratia do povodneho stavu.
    /// </summary>
    /// <param name="pathToBank">priecinok banky</param>
    /// <param name="languages">vsetky jazyky banky</param>
    /// <param name="saved">jazyky, ktorych zvuky sa zapisuju</param>
    IEnumerable<string> FilesToSave(string pathToBank, IReadOnlyList<FyzLanguage> languages, IReadOnlyList<FyzLanguage> saved);

    /// <summary>
    /// Zapise zoznam jazykov banky.
    /// </summary>
    void WriteLanguages(string pathToBank, IReadOnlyList<FyzLanguage> languages);

    /// <summary>
    /// Zapise skupiny a zvuky jazyka.
    /// </summary>
    void WriteSounds(string pathToBank, FyzLanguage language);

    /// <summary>
    /// Subory jazykov, ktore v banke ostali po odstraneni jazyka zo zoznamu a treba ich odstranit.
    /// </summary>
    IEnumerable<string> RemovedLanguageFiles(string pathToBank, IReadOnlyList<FyzLanguage> languages);
}
