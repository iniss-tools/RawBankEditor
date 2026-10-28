using RawBankEditor.Speech;
using ToolsCore.Iniss.Entities;
using ToolsCore.Iniss.Tools;

namespace RawBankEditor.Tools;

/// <summary>
/// Zmena nahravky zvuku - povodna a nova verzia (kopie WAV v <see cref="SoundVersions" />), dlzky suborov a dlzky zvuku
/// v zozname zvukov pred zmenou a po nej. Bez povodnej verzie (<see cref="CreatesFile" />) nova nahravka subor vytvara
/// (zvuk nahravku nemal) a Spat ho odstrani.
/// </summary>
/// <param name="File">subor nahravky v banke</param>
/// <param name="OldWav">kopia povodnej nahravky; <see langword="null" />, ak subor neexistoval</param>
/// <param name="NewWav">kopia novej nahravky</param>
/// <param name="OldDuration">dlzka povodnej nahravky v ms</param>
/// <param name="NewDuration">dlzka novej nahravky v ms</param>
/// <param name="OldSoundDuration">dlzka zvuku v zozname zvukov pred zmenou</param>
/// <param name="NewSoundDuration">dlzka zvuku v zozname zvukov po zmene (bez prepoctu dlzok ostava povodna)</param>
public sealed record RecordingChange(SoundFileElement File, string? OldWav, string NewWav, int OldDuration, int NewDuration, int OldSoundDuration,
    int NewSoundDuration)
{
    /// <summary>Zvuk nahravky.</summary>
    public FyzSound? Sound { get; init; }

    /// <summary>Nazov suboru zvuku pred zmenou (zvuk bez nahravky ho mohol mat prazdny).</summary>
    public string? OldFileName { get; init; }

    /// <summary>Povod povodnej nahravky (<c>.rbe-ai</c>); <see langword="null" /> = povodna nahravka banky.</summary>
    public SoundOrigin? OldOrigin { get; init; }

    /// <summary>Povod novej nahravky.</summary>
    public SoundOrigin? NewOrigin { get; init; }

    /// <summary>Text pre syntezu pred zmenou.</summary>
    public string? OldText { get; init; }

    /// <summary>Text pre syntezu po zmene.</summary>
    public string? NewText { get; init; }

    /// <summary>Zmena meni aj text pre syntezu.</summary>
    public bool ChangesText => OldText != NewText;

    /// <summary>
    /// Nova nahravka vytvara subor, ktory predtym neexistoval.
    /// </summary>
    public bool CreatesFile => OldWav is null;

    /// <summary>
    /// Zmena existujucej nahravky - povodna verzia sa skopiruje do <paramref name="versions" />.
    /// </summary>
    /// <param name="file">subor nahravky v banke</param>
    /// <param name="newWav">nova nahravka (kopia v <paramref name="versions" />)</param>
    /// <param name="newDuration">dlzka novej nahravky v ms</param>
    /// <param name="versions">kopie verzii</param>
    /// <param name="recalculateDuration">prepocitat dlzku zvuku v zozname zvukov</param>
    public static RecordingChange Replace(SoundFileElement file, string newWav, int newDuration, SoundVersions versions, bool recalculateDuration)
    {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(versions);

        var oldWav = versions.AddCopyOf(file.FileInfo.FullName);
        var oldDuration = file.Duration >= 0 ? file.Duration : SoundProcessing.ReadWav(System.IO.File.ReadAllBytes(oldWav)).DurationMs;
        var oldSoundDuration = file.Sound?.Duration ?? oldDuration;
        return new RecordingChange(file, oldWav, newWav, oldDuration, newDuration, oldSoundDuration, recalculateDuration ? newDuration : oldSoundDuration)
        {
            Sound = file.Sound
        };
    }

    /// <summary>
    /// Zmena existujucej nahravky na novy zvuk.
    /// </summary>
    public static RecordingChange Create(SoundFileElement file, PcmSound newSound, SoundVersions versions, bool recalculateDuration)
    {
        ArgumentNullException.ThrowIfNull(newSound);
        ArgumentNullException.ThrowIfNull(versions);
        return Replace(file, versions.Add(SoundProcessing.ToWav(newSound)), newSound.DurationMs, versions, recalculateDuration);
    }

    /// <summary>
    /// Nova nahravka zvuku, ktory nahravku nema. Subor lezi tam, kde ho INISS hlada: v priecinku skupiny pod nazvom
    /// suboru zvuku (prazdny nazov = predvoleny podla kluca), pri pridavnej ceste na nej.
    /// </summary>
    /// <param name="sound">zvuk bez nahravky</param>
    /// <param name="pathToBank">priecinok RAWBANK</param>
    /// <param name="newWav">nova nahravka (kopia v <see cref="SoundVersions" />)</param>
    /// <param name="newDuration">dlzka novej nahravky v ms</param>
    /// <param name="recalculateDuration">prepocitat dlzku zvuku v zozname zvukov</param>
    /// <exception cref="DirectoryNotFoundException">priecinok nahravky neexistuje</exception>
    public static RecordingChange CreateMissing(FyzSound sound, string pathToBank, string newWav, int newDuration, bool recalculateDuration)
    {
        ArgumentNullException.ThrowIfNull(sound);

        var fileName = string.IsNullOrWhiteSpace(sound.FileName) ? SoundUtils.GetDefaultFileName(sound.Group, sound.Key) : sound.FileName;
        var additional = !RawBankParser.AdditionalPathIsEmpty(sound.AdditionalRelativePath);
        var directory = sound.Group.GetAbsPath(pathToBank) + (additional ? sound.AdditionalRelativePath : "");
        var path = Path.GetFullPath(Path.Combine(directory, fileName));
        if (!Directory.Exists(Path.GetDirectoryName(path)))
            throw new DirectoryNotFoundException(Path.GetDirectoryName(path));

        // subor v priecinku skupiny patri do stromu prieskumnika, subor na pridavnej ceste nie (ako pri nacitani)
        var file = new SoundFileElement(path) { Parent = additional ? null : sound.Group.Directory };
        return new RecordingChange(file, null, newWav, -1, newDuration, sound.Duration, recalculateDuration ? newDuration : sound.Duration)
        {
            Sound = sound,
            OldFileName = sound.FileName
        };
    }
}

/// <summary>
/// Nahradenie nahravok v banke cez zurnal: povodny subor ide do zalohy zurnalu (pri ulozeni do Kosa, zahodenie zmien
/// ho vrati), na jeho miesto sa zapise ina verzia s rovnakou priponou (.EWA zakodovana). Prepojenie novych suborov so
/// zvukmi a prieskumnikom a zaznamy povodu robi <see cref="Link" /> na vlakne okna.
/// </summary>
public static class RecordingReplace
{
    /// <summary>
    /// Zapise do banky nove (<paramref name="toNew" />) alebo povodne verzie nahravok a nastavi dlzky suborov aj zvukov.
    /// Subor, ktory zmena vytvorila, Spat odstrani (cez zurnal).
    /// </summary>
    /// <param name="progress">vola sa po kazdej nahravke</param>
    /// <returns>nahravky, ktore sa nahradit nepodarilo, s popisom chyby.</returns>
    public static List<string> Apply(IEnumerable<RecordingChange> changes, bool toNew, BankJournal journal, object? scope,
        System.Action? progress = null)
    {
        ArgumentNullException.ThrowIfNull(changes);
        ArgumentNullException.ThrowIfNull(journal);

        var errors = new List<string>();
        foreach (var change in changes)
        {
            var path = change.File.FileInfo.FullName;
            var removed = false;
            try
            {
                if (File.Exists(path))
                {
                    journal.Delete(path, scope);
                    removed = true;
                }

                if (toNew || !change.CreatesFile)
                {
                    SoundProcessing.WriteSoundFile(path, File.ReadAllBytes(toNew ? change.NewWav : change.OldWav!));
                    journal.FileCreated(path, scope);
                }

                change.File.FileInfo = new FileInfo(path);
                change.File.Duration = toNew ? change.NewDuration : change.OldDuration;
                if ((change.Sound ?? change.File.Sound) is { } sound)
                    sound.Duration = toNew ? change.NewSoundDuration : change.OldSoundDuration;
            }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException)
            {
                errors.Add($"{change.File.Name}: {e.Message}");
                // nedokonceny zapis sa zmaze a vrati sa subor, ktory tam bol
                if (removed)
                    TryRestore(path, journal, scope);
            }
            finally
            {
                progress?.Invoke();
            }
        }

        return errors;
    }

    /// <summary>
    /// Prepoji vytvorene subory so zvukmi a stromom prieskumnika (<paramref name="toNew" />), alebo prepojenie zrusi
    /// (Spat), a nastavi zaznamy povodu a textu. Vola sa po <see cref="Apply" /> na vlakne okna.
    /// </summary>
    /// <param name="records">zaznamy zvukov jazyka zmien; <see langword="null" /> = bez zaznamov</param>
    public static void Link(IEnumerable<RecordingChange> changes, bool toNew, SoundRecords? records)
    {
        ArgumentNullException.ThrowIfNull(changes);

        foreach (var change in changes)
        {
            var sound = change.Sound ?? change.File.Sound;
            if (change.CreatesFile && sound is not null)
            {
                if (toNew && File.Exists(change.File.FileInfo.FullName))
                {
                    sound.File = change.File;
                    sound.FileName = change.File.Name;
                    change.File.Sound = sound;
                    if (change.File.Parent is { } parent && !parent.Children.Contains(change.File))
                        parent.Children.Add(change.File);
                }
                else if (!toNew)
                {
                    sound.File = null!;
                    sound.FileName = change.OldFileName ?? sound.FileName;
                    change.File.Sound = null!;
                    change.File.Parent?.Children.Remove(change.File);
                }
            }

            // uprava zvuku povod nemeni (zmena bez zaznamov)
            if (sound is not null && records is not null && (change.OldOrigin is not null || change.NewOrigin is not null))
                records.SetOrigin(sound, toNew ? change.NewOrigin : change.OldOrigin);
            if (sound is not null && records is not null && change.ChangesText)
                records.SetText(sound, toNew ? change.NewText : change.OldText);
        }
    }

    private static void TryRestore(string path, BankJournal journal, object? scope)
    {
        try
        {
            if (File.Exists(path))
                File.Delete(path);
            journal.Restore(path, scope);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            Log.Exception(e);
        }
    }
}
