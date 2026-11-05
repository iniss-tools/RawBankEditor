using System.Collections.Concurrent;
using RawBankEditor.Tools;
using ToolsCore.Iniss.Entities;
using ToolsCore.Iniss.Tools;
using ToolsCore.Iniss2.RawBank;

namespace RawBankEditor.Services;

/// <summary>
/// Banka INISS2: kazdy jazyk ma vlastny FyzBank.xml, spolocny zoznam jazykov neexistuje.
/// </summary>
/// <remarks>
/// MD5 suboru FyzBank.xml posiela server ustredni AK Signal, preto sa subor bez zmeny neprepisuje. MD5 a dlzka
/// nahravky v banke sa prepocitaju len pri nahravke zmenenej od nacitania jazyka - nesulad z minulosti ostava,
/// kym ho pouzivatel nevyriesi v zozname chyb.
/// </remarks>
public sealed class BankXmlStore : IBankStore
{
    // stav nahravok pri nacitani jazyka; null = subor neexistoval
    private readonly ConcurrentDictionary<string, FileStamp?> _recordings = new(StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    public bool IsIniss2 => true;

    /// <inheritdoc />
    public string LanguageFileName => FyzBankXmlWriter.FileName;

    /// <inheritdoc />
    public List<FyzLanguage> ReadLanguages(string pathToBank) => FyzBankXmlReader.ReadLanguages(pathToBank);

    /// <inheritdoc />
    public void ReadSounds(string pathToBank, FyzLanguage language, IProgress<ProgressStatus>? progress = null) =>
        FyzBankXmlReader.ReadSounds(pathToBank, language);

    /// <inheritdoc />
    public void RememberRecordings(string pathToBank, FyzLanguage language)
    {
        ArgumentNullException.ThrowIfNull(language);

        foreach (var sound in language.Groups.SelectMany(group => group.Sounds))
        {
            var path = sound.GetAbsPath(pathToBank);
            _recordings[path] = FileStamp.Of(path);
        }
    }

    /// <inheritdoc />
    public IEnumerable<string> FilesToSave(string pathToBank, IReadOnlyList<FyzLanguage> languages, IReadOnlyList<FyzLanguage> saved) =>
        languages.Select(language => RawBankParser.FyzZvukFile(pathToBank, language));

    /// <summary>
    /// Udaje jazyka (nazov, kluc, predvoleny jazyk) su v tom istom subore ako zvuky. Zmenia sa aj jazyku, ktoreho zvuky
    /// sa nezapisuju - jeho zvuky sa vezmu zo suboru, neulozene zmeny v pamati ostanu neulozene.
    /// </summary>
    public void WriteLanguages(string pathToBank, IReadOnlyList<FyzLanguage> languages)
    {
        ArgumentNullException.ThrowIfNull(languages);

        var saved = ReadLanguages(pathToBank);
        foreach (var language in languages)
        {
            if (saved.FirstOrDefault(s => LanguageRules.SameFolder(s, language)) is not { } file || SameData(file, language))
                continue;

            var updated = new FyzLanguage(language.Key, language.Name, language.FileDefName, language.RelativePath)
            {
                IsBasic = language.IsBasic,
                Date = file.Date,
                Location = file.Location
            };
            ReadSounds(pathToBank, updated);
            Write(pathToBank, updated);
        }
    }

    /// <inheritdoc />
    public void WriteSounds(string pathToBank, FyzLanguage language)
    {
        ArgumentNullException.ThrowIfNull(language);

        foreach (var group in language.Groups)
        {
            group.Name = group.Key;
            foreach (var sound in group.Sounds)
            {
                sound.Name = sound.Key;
                UpdateRecording(pathToBank, sound);
            }
        }

        if (!IsSaved(pathToBank, language))
            Write(pathToBank, language);
    }

    /// <inheritdoc />
    public IEnumerable<string> RemovedLanguageFiles(string pathToBank, IReadOnlyList<FyzLanguage> languages) =>
        ReadLanguages(pathToBank)
            .Where(saved => !languages.Any(language => LanguageRules.SameFolder(saved, language)))
            .Select(saved => RawBankParser.FyzZvukFile(pathToBank, saved))
            .ToList();

    private static void Write(string pathToBank, FyzLanguage language) =>
        FyzBankXmlWriter.Write(RawBankParser.FyzZvukFile(pathToBank, language), language, language.IsBasic, language.Date ?? DateTimeOffset.Now, sound => sound.Md5);

    private static bool SameData(FyzLanguage a, FyzLanguage b) => a.Key == b.Key && a.Name == b.Name && a.IsBasic == b.IsBasic;

    private bool IsSaved(string pathToBank, FyzLanguage language)
    {
        if (ReadLanguages(pathToBank).FirstOrDefault(saved => LanguageRules.SameFolder(saved, language)) is not { } file || !SameData(file, language))
            return false;

        ReadSounds(pathToBank, file);
        return LanguageRules.SameGroups(language.Groups, file.Groups);
    }

    private void UpdateRecording(string pathToBank, FyzSound sound)
    {
        var path = sound.GetAbsPath(pathToBank);
        var stamp = FileStamp.Of(path);
        var unchanged = _recordings.TryGetValue(path, out var remembered) ? remembered == stamp : stamp is null;
        if (unchanged)
            return;

        sound.Md5 = stamp is null ? null : FyzBankXmlWriter.FileMd5(path);
        _recordings[path] = stamp;
    }

    /// <summary>
    /// Stav suboru nahravky - zmena dlzky alebo casu zapisu znamena, ze sa nahravka zmenila.
    /// </summary>
    private readonly record struct FileStamp(long Length, DateTime LastWriteUtc)
    {
        public static FileStamp? Of(string path)
        {
            var info = new FileInfo(path);
            return info.Exists ? new FileStamp(info.Length, info.LastWriteTimeUtc) : null;
        }
    }
}
