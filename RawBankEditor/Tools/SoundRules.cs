using ToolsCore.Entities;
using ToolsCore.Tools;

namespace RawBankEditor.Tools;

/// <summary>
/// Pravidla pre zvuky skupiny: kluc a nazov su povinne a v skupine jedinecne (rovnako ako v okne Pridat zvuk).
/// INISS porovnava kluce bez ohladu na velkost pismen, preto aj tieto pravidla.
/// </summary>
public static class SoundRules
{
    /// <summary>
    /// Porovnanie klucov a nazvov zvukov - rovnako ako INISS, bez ohladu na velkost pismen.
    /// </summary>
    public static bool SameText(string? a, string? b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Skontroluje nove zvuky pred pridanim do ich skupin - voci zvukom, ktore uz v skupine su, aj navzajom.
    /// </summary>
    /// <param name="newSounds">Nove zvuky, este nepridane do <see cref="FyzGroup.Sounds" />.</param>
    /// <returns>Pre kazdy zvuk s problemom text chyby.</returns>
    public static Dictionary<FyzSound, string> ValidateNew(IEnumerable<FyzSound> newSounds)
    {
        var problems = new Dictionary<FyzSound, string>();
        var sounds = newSounds.ToList();

        foreach (var sound in sounds)
        {
            if (string.IsNullOrWhiteSpace(sound.Key) || string.IsNullOrWhiteSpace(sound.Name))
            {
                problems[sound] = "Kľúč aj názov zvuku sú povinné.";
                continue;
            }

            var others = sound.Group.Sounds.Concat(sounds.Where(s => s != sound && s.Group == sound.Group)).ToList();
            if (others.Any(s => SameText(s.Key, sound.Key)))
                problems[sound] = $"Kľúč {sound.Key} už v skupine {sound.Group.Name} existuje.";
            else if (others.Any(s => SameText(s.Name, sound.Name)))
                problems[sound] = $"Názov {sound.Name} už v skupine {sound.Group.Name} existuje.";
        }

        return problems;
    }

    /// <summary>
    /// Ci sa nahravka zvuku pri presune do inej skupiny presuva s nim - lezi priamo v priecinku skupiny
    /// (bez pridavnej cesty). Nahravka s pridavnou cestou ostava na mieste, prepocita sa len cesta.
    /// </summary>
    public static bool FileMovesWithSound(FyzSound sound) => RawBankParser.AdditionalPathIsEmpty(sound.AdditionalRelativePath);

    /// <summary>
    /// Skontroluje presun zvukov do skupiny <paramref name="target" /> - vsetko alebo nic.
    /// </summary>
    /// <param name="sounds">Presuvane zvuky (este v povodnej skupine).</param>
    /// <param name="target">Cielova skupina.</param>
    /// <param name="pathToBank">Priecinok banky - na kontrolu suborov v priecinku cielovej skupiny.</param>
    /// <returns>Texty problemov; prazdny zoznam = presun je mozny.</returns>
    public static List<string> ValidateMove(IEnumerable<FyzSound> sounds, FyzGroup target, string pathToBank)
    {
        var problems = new List<string>();
        var targetDir = target.GetAbsPath(pathToBank);
        var moving = sounds.ToList();

        foreach (var sound in moving)
        {
            if (target.Sounds.Any(s => SameText(s.Key, sound.Key)))
                problems.Add($"Kľúč {sound.Key} už v skupine {target.Name} existuje.");
            else if (target.Sounds.Any(s => SameText(s.Name, sound.Name)))
                problems.Add($"Názov {sound.Name} už v skupine {target.Name} existuje.");

            if (!FileMovesWithSound(sound) || string.IsNullOrEmpty(sound.FileName))
                continue;

            var source = sound.GetAbsPath(pathToBank);
            if (!File.Exists(source))
                continue;

            if (!Directory.Exists(targetDir))
                problems.Add($"Priečinok skupiny {target.Name} ({targetDir}) neexistuje – nahrávku {sound.FileName} nie je kam presunúť.");
            else if (File.Exists(Path.Combine(targetDir, sound.FileName)))
                problems.Add($"V priečinku skupiny {target.Name} už je súbor {sound.FileName}.");
        }

        return problems.Distinct().ToList();
    }

    /// <summary>
    /// Pridavna cesta, ktorou zvuk zo skupiny <paramref name="group" /> ukazuje na nahravku v priecinku
    /// <paramref name="fileDirectory" /> - prazdna, ak nahravka lezi priamo v priecinku skupiny.
    /// </summary>
    public static string AdditionalPathFor(FyzGroup group, string fileDirectory, string pathToBank)
    {
        var relative = Path.GetRelativePath(group.GetAbsPath(pathToBank), fileDirectory);
        return relative == "." ? "" : relative + Path.DirectorySeparatorChar;
    }
}
