using ToolsCore.Entities;

namespace RawBankEditor.Tools;

/// <summary>
///     Pravidla pre zvuky skupiny: kluc a nazov su povinne a v skupine jedinecne (rovnako ako v okne Pridat zvuk).
/// </summary>
public static class SoundRules
{
    /// <summary>
    ///     Skontroluje nove zvuky pred pridanim do ich skupin - voci zvukom, ktore uz v skupine su, aj navzajom.
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
            if (others.Any(s => s.Key == sound.Key))
                problems[sound] = $"Kľúč {sound.Key} už v skupine {sound.Group.Name} existuje.";
            else if (others.Any(s => s.Name == sound.Name))
                problems[sound] = $"Názov {sound.Name} už v skupine {sound.Group.Name} existuje.";
        }

        return problems;
    }
}
