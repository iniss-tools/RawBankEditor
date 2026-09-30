namespace RawBankEditor.Tools;

/// <summary>
/// Zurnal zmien na disku od posledneho ulozenia banky. Operacie sa vykonaju hned (prieskumnik a sledovanie suborov
/// vidia skutocny stav), no daju sa vratit: <see cref="Commit" /> ich pri ulozeni potvrdi, <see cref="Rollback" />
/// pri zahodeni zmien vrati disk do stavu posledneho ulozenia - banka (FYZBANK/FYZZVUK) a disk si tak vzdy zodpovedaju.
/// </summary>
/// <remarks>
/// Odstranene subory a priecinky sa neposielaju hned do Kosa, ale do zaloznoho priecinka zurnalu; do Kosa idu az pri
/// ulozeni. Kazda operacia ma rozsah (napr. jazyk) - zahodenie zmien jedneho jazyka vrati len jeho operacie a cesty
/// prepocita cez neskorsie ponechane presuny (napr. premenovanie priecinka jazyka).
/// </remarks>
/// <param name="backupRoot">zalozny priecinok zurnalu - na rovnakom disku ako banka (presuny su okamzite)</param>
/// <param name="recycle">presun do Kosa pri potvrdeni (subor alebo priecinok)</param>
/// <param name="restoreFromRecycleBin">obnova z Kosa - pri vrateni odstranenia, ktore uz bolo potvrdene</param>
public sealed class BankJournal(string backupRoot, Action<string> recycle, Func<string, bool> restoreFromRecycleBin)
{
    private enum Kind
    {
        /// <summary>Vytvoreny priecinok.</summary>
        CreatedDirectory,

        /// <summary>Vytvoreny subor (napr. konverziou).</summary>
        CreatedFile,

        /// <summary>Presun alebo premenovanie suboru ci priecinka (aj obnova zo zalohy).</summary>
        Moved,

        /// <summary>Odstraneny prazdny priecinok.</summary>
        RemovedEmptyDirectory,

        /// <summary>Subor alebo priecinok obnoveny z Kosa (odstranenie uz bolo potvrdene ulozenim).</summary>
        RestoredFromRecycleBin
    }

    private sealed record Entry(Kind Kind, string Path, string? Target, object? Scope);

    private readonly List<Entry> _entries = [];
    private readonly object _locker = new();
    private int _backupCounter;

    /// <summary>
    /// Zalozny priecinok zurnalu.
    /// </summary>
    public string BackupRoot { get; } = backupRoot;

    /// <summary>
    /// Od posledneho ulozenia sa disk zmenil.
    /// </summary>
    public bool HasChanges
    {
        get
        {
            lock (_locker)
                return _entries.Count > 0;
        }
    }

    /// <summary>
    /// Vytvori priecinok (ak neexistuje).
    /// </summary>
    /// <returns><see langword="true" />, ak priecinok vznikol.</returns>
    public bool CreateDirectory(string path, object? scope)
    {
        if (Directory.Exists(path))
            return false;

        Directory.CreateDirectory(path);
        Add(new Entry(Kind.CreatedDirectory, path, null, scope));
        return true;
    }

    /// <summary>
    /// Zaznamena subor vytvoreny mimo zurnalu (napr. konverziou) - vratenie ho zmaze.
    /// </summary>
    public void FileCreated(string path, object? scope) => Add(new Entry(Kind.CreatedFile, path, null, scope));

    /// <summary>
    /// Presunie alebo premenuje subor ci priecinok.
    /// </summary>
    public void Move(string from, string to, object? scope)
    {
        MoveOnDisk(from, to);
        Add(new Entry(Kind.Moved, from, to, scope));
    }

    /// <summary>
    /// Odstrani subor alebo priecinok - presunie ho do zaloznoho priecinka (do Kosa pojde az pri ulozeni).
    /// </summary>
    public void Delete(string path, object? scope)
    {
        var backup = NewBackupPath(path);
        if (!Directory.Exists(BackupRoot))
            Directory.CreateDirectory(BackupRoot).Attributes |= FileAttributes.Hidden;
        Directory.CreateDirectory(Path.GetDirectoryName(backup)!);
        MoveOnDisk(path, backup);
        Add(new Entry(Kind.Moved, path, backup, scope));
    }

    /// <summary>
    /// Odstrani prazdny priecinok (napr. spat pri pridani skupiny) - do Kosa nejde nic.
    /// </summary>
    public void RemoveEmptyDirectory(string path, object? scope)
    {
        Directory.Delete(path, false);
        Add(new Entry(Kind.RemovedEmptyDirectory, path, null, scope));
    }

    /// <summary>
    /// Vrati odstraneny subor alebo priecinok na povodne miesto - zo zalohy, a ak uz bol potvrdeny, z Kosa.
    /// </summary>
    /// <returns><see langword="false" />, ak sa ho nepodarilo obnovit.</returns>
    public bool Restore(string path, object? scope)
    {
        if (File.Exists(path) || Directory.Exists(path))
            return true;

        string? backup;
        lock (_locker)
            backup = _entries.LastOrDefault(e => e.Kind == Kind.Moved && e.Target is not null && IsBackup(e.Target) && SamePath(e.Path, path))?.Target;

        if (backup is not null && (File.Exists(backup) || Directory.Exists(backup)))
        {
            Move(backup, path, scope);
            return true;
        }

        if (!restoreFromRecycleBin(path))
            return false;

        Add(new Entry(Kind.RestoredFromRecycleBin, path, null, scope));
        return true;
    }

    /// <summary>
    /// Potvrdi zmeny pri ulozeni banky: odstranene polozky zo zalohy presunie do Kosa a zurnal vyprazdni.
    /// </summary>
    /// <returns>chyby pri presune do Kosa (polozky ostanu v zaloznom priecinku).</returns>
    public List<string> Commit()
    {
        var errors = new List<string>();
        lock (_locker)
        {
            if (Directory.Exists(BackupRoot))
            {
                foreach (var item in Directory.EnumerateFileSystemEntries(BackupRoot).SelectMany(Directory.EnumerateFileSystemEntries).ToList())
                    try
                    {
                        recycle(item);
                    }
                    catch (Exception e) when (e is IOException or UnauthorizedAccessException or OperationCanceledException)
                    {
                        errors.Add($"{item}: {e.Message}");
                    }

                if (errors.Count == 0)
                    TryDeleteDirectory(BackupRoot, true);
            }

            _entries.Clear();
        }

        return errors;
    }

    /// <summary>
    /// Vrati zmeny na disku - vsetky, alebo len operacie s rozsahom <paramref name="scope" />, od poslednej po prvu.
    /// </summary>
    /// <param name="scope">rozsah (napr. jazyk); <see langword="null" /> = vsetky operacie</param>
    /// <returns>operacie, ktore sa vratit nepodarilo.</returns>
    public List<string> Rollback(object? scope = null)
    {
        var errors = new List<string>();
        lock (_locker)
        {
            for (var i = _entries.Count - 1; i >= 0; i--)
            {
                var entry = _entries[i];
                if (scope is not null && !Equals(entry.Scope, scope))
                    continue;

                // cesty sa prepocitaju cez neskorsie ponechane presuny (napr. premenovany priecinok jazyka)
                var later = _entries.Skip(i + 1).Where(e => e.Kind == Kind.Moved).ToList();
                var path = Remap(entry.Path, later);
                var target = entry.Target is null ? null : Remap(entry.Target, later);
                try
                {
                    Undo(entry.Kind, path, target);
                }
                catch (Exception e) when (e is IOException or UnauthorizedAccessException or OperationCanceledException)
                {
                    errors.Add($"{path}: {e.Message}");
                }

                _entries.RemoveAt(i);
            }

            if (_entries.Count == 0 && Directory.Exists(BackupRoot) && !Directory.EnumerateFileSystemEntries(BackupRoot).SelectMany(Directory.EnumerateFileSystemEntries).Any())
                TryDeleteDirectory(BackupRoot, true);
        }

        return errors;
    }

    private void Undo(Kind kind, string path, string? target)
    {
        switch (kind)
        {
            case Kind.CreatedDirectory:
                // priecinok sa maze, len ak je prazdny - obsah mohol pribudnut mimo programu
                if (Directory.Exists(path) && !Directory.EnumerateFileSystemEntries(path).Any())
                    Directory.Delete(path);
                break;
            case Kind.CreatedFile:
                if (File.Exists(path))
                    File.Delete(path);
                break;
            case Kind.Moved:
                MoveOnDisk(target!, path);
                break;
            case Kind.RemovedEmptyDirectory:
                Directory.CreateDirectory(path);
                break;
            case Kind.RestoredFromRecycleBin:
                // stav ulozenia ho nema - ide spat do Kosa
                if (File.Exists(path) || Directory.Exists(path))
                    recycle(path);
                break;
        }
    }

    private static string Remap(string path, IEnumerable<Entry> laterMoves)
    {
        foreach (var move in laterMoves)
        {
            if (SamePath(path, move.Path))
                path = move.Target!;
            else if (path.StartsWith(Path.TrimEndingDirectorySeparator(move.Path) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                path = Path.TrimEndingDirectorySeparator(move.Target!) + path[Path.TrimEndingDirectorySeparator(move.Path).Length..];
        }

        return path;
    }

    private static void MoveOnDisk(string from, string to)
    {
        if (Directory.Exists(from))
            Directory.Move(from, to);
        else
            File.Move(from, to);
    }

    private string NewBackupPath(string path)
    {
        // zalozny priecinok moze obsahovat polozky, ktore sa pri ulozeni nepodarilo presunut do Kosa
        string slot;
        do
            slot = Path.Combine(BackupRoot, Interlocked.Increment(ref _backupCounter).ToString(System.Globalization.CultureInfo.InvariantCulture));
        while (Directory.Exists(slot));

        return Path.Combine(slot, Path.GetFileName(Path.TrimEndingDirectorySeparator(path)));
    }

    private bool IsBackup(string path) =>
        path.StartsWith(Path.TrimEndingDirectorySeparator(BackupRoot) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);

    private static bool SamePath(string a, string b) =>
        string.Equals(Path.TrimEndingDirectorySeparator(a), Path.TrimEndingDirectorySeparator(b), StringComparison.OrdinalIgnoreCase);

    private static void TryDeleteDirectory(string path, bool recursive)
    {
        try
        {
            Directory.Delete(path, recursive);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
        }
    }

    private void Add(Entry entry)
    {
        lock (_locker)
            _entries.Add(entry);
    }
}
