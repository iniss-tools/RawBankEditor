using ToolsCore.Iniss.Entities;
using ToolsCore.Iniss2.RawBank;

namespace RawBankEditor.Tools;

/// <summary>
/// Nahravka, ktora nezodpoveda udajom zvuku v banke INISS2.
/// </summary>
/// <param name="Sound">zvuk</param>
/// <param name="Md5">MD5 nahravky na disku</param>
/// <param name="Duration">dlzka nahravky na disku v milisekundach</param>
public sealed record RecordingMismatch(FyzSound Sound, string Md5, int Duration);

/// <summary>
/// Porovnanie nahravok s MD5 a dlzkou zvukov v banke INISS2.
/// </summary>
/// <remarks>
/// Server hodnotu MD5 neoveruje, len kontroluje, ci je vyplnena, a podla dlzky zo suboru rata cas hlasenia. Nesulad je
/// preto upozornenie - program ho sam neopravi, rozhoduje pouzivatel.
/// </remarks>
internal static class RecordingCheck
{
    /// <summary>
    /// Najde zvuky, ktorych nahravka na disku nezodpoveda udajom v banke. Zvuky bez nahravky hlasi zoznam chyb inak.
    /// </summary>
    /// <param name="sounds">zvuky jazyka</param>
    public static async Task<List<RecordingMismatch>> FindAsync(IEnumerable<FyzSound> sounds)
    {
        var mismatches = new List<RecordingMismatch>();
        foreach (var sound in sounds)
        {
            if (sound.File?.FileInfo is not { Exists: true } file)
                continue;

            var md5 = await Task.Run(() => FyzBankXmlWriter.FileMd5(file.FullName));
            if (string.Equals(md5, sound.Md5, StringComparison.OrdinalIgnoreCase))
                continue;

            var duration = await SoundUtils.GetSoundDuration(sound.File);
            mismatches.Add(new RecordingMismatch(sound, md5, duration >= 0 ? duration : sound.Duration));
        }

        return mismatches;
    }
}
