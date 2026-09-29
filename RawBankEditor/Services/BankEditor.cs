using System.Globalization;
using RawBankEditor.Entities;
using RawBankEditor.Properties;
using RawBankEditor.Tools;
using RawBankEditor.XML;
using ToolsCore;
using ToolsCore.Iniss.Entities;
using ToolsCore.Iniss.Tools;
using ToolsCore.Tools;

namespace RawBankEditor.Services;

/// <summary>
/// Otvorena banka zvukov bez UI: nastavenia programu, banka, zurnal zmien na disku, nove jazyky, ulozenie a zahodenie
/// zmien. Banka (FYZBANK/FYZZVUK) a disk si vdaka zurnalu zodpovedaju - ulozenie potvrdi zmeny na disku, zahodenie
/// ich vrati.
/// </summary>
/// <param name="session">nastavenia programu</param>
internal sealed class BankEditor(AppSession<RawBankEditorConfig, RawBankEditorStyle> session)
{
    // zalozny priecinok zurnalu v priecinku INISS (vedla RAWBANK - rovnaky disk, prieskumnik banky ho nevidi)
    private const string JOURNAL_PREFIX = ".rbe-zurnal-";

    // jazyky pridane v otvorenej banke, ktorym sa este nezapisal FYZZVUK.DAT - otvaraju sa ako prazdne
    private readonly HashSet<FyzLanguage> _newLanguages = [];
    private BankJournal? _journal;

    /// <summary>
    /// Nastavenia programu.
    /// </summary>
    public AppSession<RawBankEditorConfig, RawBankEditorStyle> Session { get; } = session;

    /// <summary>
    /// Konfiguracia programu.
    /// </summary>
    public RawBankEditorConfig Config => Session.Config;

    /// <summary>
    /// Pouzivany styl.
    /// </summary>
    public RawBankEditorStyle UsingStyle => Session.UsingStyle;

    /// <summary>
    /// Otvorena banka; <see langword="null" />, kym pouzivatel ziadnu neotvori.
    /// </summary>
    public RawBankProject? Project { get; private set; }

    /// <summary>
    /// Priecinok RAWBANK otvorenej banky.
    /// </summary>
    public string PathToBank => Project?.AbsPathToBank ?? throw new InvalidOperationException("Banka nie je otvorena.");

    /// <summary>
    /// Zurnal zmien na disku otvorenej banky - kazda zmena suborov a priecinkov banky ide cez neho.
    /// </summary>
    public BankJournal Journal => _journal ?? throw new InvalidOperationException("Banka nie je otvorena.");

    /// <summary>
    /// Otvori nacitanu banku. Predchadzajuca banka musi byt zatvorena cez <see cref="Close" />.
    /// </summary>
    /// <returns>polozky, ktore po neukoncenom behu programu ostali v zaloznom priecinku a presunuli sa do Kosa.</returns>
    public List<string> Open(RawBankProject project)
    {
        ArgumentNullException.ThrowIfNull(project);

        var leftovers = RecycleLeftoverJournals(project.AbsPathToINISS);
        Project = project;
        _newLanguages.Clear();
        _journal = new BankJournal(Path.Combine(project.AbsPathToINISS, JOURNAL_PREFIX + Environment.ProcessId.ToString(CultureInfo.InvariantCulture)),
            Recycle, Utils.TryRecoverFileOrDirFromBin);
        return leftovers;
    }

    /// <summary>
    /// Zatvori banku: zmeny na disku potvrdi (banka je ulozena) alebo vrati (zmeny sa zahadzuju).
    /// </summary>
    /// <returns>chyby pri potvrdeni alebo vrateni zmien na disku.</returns>
    public List<string> Close(bool discardChanges)
    {
        if (_journal is null)
            return [];

        var errors = discardChanges ? _journal.Rollback() : _journal.Commit();
        _journal = null;
        Project = null;
        _newLanguages.Clear();
        return errors;
    }

    /// <summary>
    /// Jazyk je novy - jeho FYZZVUK.DAT este nie je zapisany.
    /// </summary>
    public bool IsNew(FyzLanguage language) => _newLanguages.Contains(language);

    /// <summary>
    /// Oznaci jazyk ako novy (FYZZVUK.DAT sa zapise pri ulozeni).
    /// </summary>
    public void MarkNew(FyzLanguage language) => _newLanguages.Add(language);

    /// <summary>
    /// Zoznam jazykov sa lisi od ulozeneho FYZBANK.DAT.
    /// </summary>
    public bool LanguageListChanged() => LanguageRules.BankDiffers(PathToBank, Project!.Languages);

    /// <summary>
    /// Zoznam zvukov jazyka sa lisi od jeho ulozeneho FYZZVUK.DAT.
    /// </summary>
    public bool SoundsChanged(FyzLanguage language) => LanguageRules.SoundsDiffer(PathToBank, language);

    /// <summary>
    /// Zapise FYZBANK.DAT a FYZZVUK.DAT otvoreneho jazyka (alebo vsetkych nacitanych jazykov) a novych jazykov, aby
    /// FYZBANK.DAT neodkazoval na chybajuci subor. Po zapise potvrdi zmeny na disku (odstranene polozky idu do Kosa).
    /// </summary>
    /// <param name="allLanguages">zapisat vsetky nacitane jazyky</param>
    /// <param name="loadedLanguage">otvoreny jazyk, ak sa nacital bez chyby</param>
    public BankSaveResult Save(bool allLanguages, FyzLanguage? loadedLanguage)
    {
        var project = Project ?? throw new InvalidOperationException("Banka nie je otvorena.");

        // nenacitany jazyk (aj po chybe nacitania) sa nezapisuje - na disku ostava jeho subor
        var languages = project.Languages
            .Where(lang => lang.Groups is not null && (allLanguages || IsNew(lang) || ReferenceEquals(lang, loadedLanguage)))
            .ToList();

        // FYZBANK.DAT a FYZZVUK.DAT jazykov patria k sebe - pri chybe v polovici sa vratia vsetky,
        // inak by FYZBANK.DAT mohol odkazovat na subor, ktory nevznikol, a INISS by pri starte skoncil
        FileTransaction transaction;
        try
        {
            transaction = new FileTransaction(
                languages.Select(lang => RawBankParser.FyzZvukFile(project.AbsPathToBank, lang))
                    .Prepend(RawBankParser.FyzBankFile(project.AbsPathToBank)));
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            Log.Exception(exception);
            return BankSaveResult.Failed(string.Format(CultureInfo.CurrentCulture, Resources.FMain_Save_Failed, exception.Message));
        }

        try
        {
            RawBankParser.WriteFyzBankFile(project.AbsPathToBank, project.Languages.ToList());

            foreach (var lang in languages)
            {
                Directory.CreateDirectory(lang.GetAbsPath(project.AbsPathToBank));
                RawBankParser.WriteFyzZvukFile(project.AbsPathToBank, lang);
            }
        }
        catch (Exception exception)
        {
            // napr. subor len na citanie, bez opravneni na zapis, otvoreny inym programom alebo pridlhy text
            Log.Exception(exception);
            return BankSaveResult.Failed(transaction.TryRollback()
                ? string.Format(CultureInfo.CurrentCulture, Resources.FMain_Save_Failed, exception.Message)
                : string.Format(CultureInfo.CurrentCulture, Resources.FMain_Save_Failed_Rollback, exception.Message, transaction.BackupPath));
        }

        transaction.Commit();
        foreach (var lang in languages)
            _newLanguages.Remove(lang);

        return new BankSaveResult(null, Journal.Commit());
    }

    /// <summary>
    /// Zahodi zmeny zvukov jazyka: vrati jeho zmeny na disku a zoznam zvukov v pamati - pri dalsom otvoreni sa
    /// nacita z disku, novy jazyk bude prazdny. Zmeny zoznamu jazykov ostavaju.
    /// </summary>
    /// <returns>zmeny na disku, ktore sa nepodarilo vratit.</returns>
    public List<string> DiscardLanguage(FyzLanguage language)
    {
        var errors = _journal?.Rollback(language) ?? [];
        ForgetLanguage(language);
        return errors;
    }

    /// <summary>
    /// Zabudne zoznam zvukov jazyka v pamati (napr. po chybe nacitania) - disk sa nemeni.
    /// </summary>
    public void ForgetLanguage(FyzLanguage language)
        => language.Groups = IsNew(language) ? new List<FyzGroup>() : null!;

    /// <summary>
    /// Presun do Kosa pri potvrdeni zurnalu.
    /// </summary>
    private static void Recycle(string path)
    {
        if (Directory.Exists(path))
            Utils.DeleteDirectoryToRecycleBin(path);
        else
            Utils.DeleteFileToRecycleBin(path);
    }

    /// <summary>
    /// Zalozne priecinky zurnalu, ktore ostali po neukoncenom behu programu (padol pred ulozenim alebo zahodenim
    /// zmien), presunie do Kosa - odtial sa daju obnovit. Priecinky beziacich instancii programu ostanu.
    /// </summary>
    private static List<string> RecycleLeftoverJournals(string pathToIniss)
    {
        var recycled = new List<string>();
        if (!Directory.Exists(pathToIniss))
            return recycled;

        foreach (var dir in Directory.EnumerateDirectories(pathToIniss, JOURNAL_PREFIX + "*"))
        {
            if (!int.TryParse(Path.GetFileName(dir)[JOURNAL_PREFIX.Length..], NumberStyles.None, CultureInfo.InvariantCulture, out var pid)
                || IsRunningInstance(pid))
                continue;

            foreach (var item in Directory.EnumerateDirectories(dir).SelectMany(Directory.EnumerateFileSystemEntries).ToList())
            {
                try
                {
                    Recycle(item);
                    recycled.Add(item);
                    Log.Warning(string.Format(CultureInfo.InvariantCulture, "Zurnal: {0} z neukonceneho behu presunuty do Kosa", item));
                }
                catch (Exception e) when (e is IOException or UnauthorizedAccessException or OperationCanceledException)
                {
                    Log.Exception(e);
                }
            }

            try
            {
                if (!Directory.EnumerateDirectories(dir).SelectMany(Directory.EnumerateFileSystemEntries).Any())
                    Directory.Delete(dir, true);
            }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException)
            {
                Log.Exception(e);
            }
        }

        return recycled;
    }

    private static bool IsRunningInstance(int pid)
    {
        if (pid == Environment.ProcessId)
            return true;

        try
        {
            using var process = Process.GetProcessById(pid);
            using var current = Process.GetCurrentProcess();
            return string.Equals(process.ProcessName, current.ProcessName, StringComparison.OrdinalIgnoreCase);
        }
        catch (ArgumentException)
        {
            // proces uz nebezi
            return false;
        }
    }
}

/// <summary>
/// Vysledok ulozenia banky.
/// </summary>
/// <param name="Error">chyba zapisu (zmeny ostali neulozene); <see langword="null" />, ak sa banka ulozila</param>
/// <param name="RecycleErrors">polozky, ktore sa po ulozeni nepodarilo presunut do Kosa</param>
internal sealed record BankSaveResult(string? Error, IReadOnlyList<string> RecycleErrors)
{
    /// <summary>
    /// Banka sa ulozila.
    /// </summary>
    public bool Saved => Error is null;

    /// <summary>
    /// Neuspesne ulozenie.
    /// </summary>
    public static BankSaveResult Failed(string error) => new(error, []);
}
