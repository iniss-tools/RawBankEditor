using RawBankEditor.Speech;
using Serilog;

namespace RawBankEditor.Tools;

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
                Log.Warning(e, "Nahravku {Path} sa nepodarilo nahradit", path);
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
            Log.Error(e, "Povodnu nahravku {Path} sa po chybe zapisu nepodarilo obnovit", path);
        }
    }
}
