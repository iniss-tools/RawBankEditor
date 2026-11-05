using ToolsCore.Iniss.Entities;
using ToolsCore.Iniss.Tools;

namespace RawBankEditor.Services;

/// <summary>
/// Banka INISS 3.39: zoznam jazykov vo FYZBANK.DAT, skupiny a zvuky vo FYZZVUK.DAT jazyka.
/// </summary>
public sealed class BankDatStore : IBankStore
{
    /// <summary>
    /// Jedina instancia - sposob ulozenia nema stav.
    /// </summary>
    public static BankDatStore Instance { get; } = new();

    private BankDatStore()
    {
    }

    /// <inheritdoc />
    public bool IsIniss2 => false;

    /// <inheritdoc />
    public string LanguageFileName => RawBankParser.FileFyzzvuk;

    /// <inheritdoc />
    public List<FyzLanguage> ReadLanguages(string pathToBank) => RawBankParser.ReadFyzBankFile(pathToBank, out _);

    /// <inheritdoc />
    public void ReadSounds(string pathToBank, FyzLanguage language, IProgress<ProgressStatus>? progress = null) =>
        RawBankParser.ReadFyzZvukFile(pathToBank, language, progress);

    /// <inheritdoc />
    public void RememberRecordings(string pathToBank, FyzLanguage language)
    {
        // subory 3.39 o nahravkach nic neukladaju
    }

    /// <inheritdoc />
    public IEnumerable<string> FilesToSave(string pathToBank, IReadOnlyList<FyzLanguage> languages, IReadOnlyList<FyzLanguage> saved) =>
        saved.Select(language => RawBankParser.FyzZvukFile(pathToBank, language)).Prepend(RawBankParser.FyzBankFile(pathToBank));

    /// <inheritdoc />
    public void WriteLanguages(string pathToBank, IReadOnlyList<FyzLanguage> languages) => RawBankParser.WriteFyzBankFile(pathToBank, languages.ToList());

    /// <inheritdoc />
    public void WriteSounds(string pathToBank, FyzLanguage language) => RawBankParser.WriteFyzZvukFile(pathToBank, language);

    /// <inheritdoc />
    public IEnumerable<string> RemovedLanguageFiles(string pathToBank, IReadOnlyList<FyzLanguage> languages) => [];
}
