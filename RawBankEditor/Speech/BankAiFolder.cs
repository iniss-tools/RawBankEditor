using System.Text.Json;
using Serilog;

namespace RawBankEditor.Speech;

/// <summary>
/// Priecinok <c>.rbe-ai</c> v priecinku INISS s udajmi AI jazykov banky.
/// </summary>
public static class BankAiFolder
{
    /// <summary>Nazov priecinka v priecinku INISS (vedla RAWBANK).</summary>
    public const string FolderName = ".rbe-ai";

    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    /// <summary>
    /// Subor udajov jazyka.
    /// </summary>
    public static string FileOf(string pathToIniss, string languageKey)
    {
        var name = string.Concat(languageKey.Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '_' : c));
        return Path.Combine(pathToIniss, FolderName, name + ".json");
    }

    /// <summary>
    /// Nacita udaje jazyka; chybajuci alebo poskodeny subor = prazdne udaje.
    /// </summary>
    public static LanguageAiData Load(string pathToIniss, string languageKey)
    {
        var path = FileOf(pathToIniss, languageKey);
        if (!File.Exists(path))
            return new LanguageAiData();

        try
        {
            var data = JsonSerializer.Deserialize<LanguageAiData>(File.ReadAllText(path), JsonOptions) ?? new LanguageAiData();
            // kluce skupin bez rozlisenia velkosti pismen ako v INISSe
            data.Prosody = new Dictionary<string, GroupProsody>(data.Prosody, StringComparer.OrdinalIgnoreCase);
            return data;
        }
        catch (Exception e) when (e is IOException or JsonException or UnauthorizedAccessException)
        {
            Log.Error(e, "Udaje AI jazyka {File} sa nepodarilo precitat - pouziju sa prazdne", path);
            return new LanguageAiData();
        }
    }

    /// <summary>
    /// Zmeni nastavenia jazyka (hlas, intonacia, vyslovnost) a hned ich ulozi - zaznamy zvukov na disku ostanu (tie sa
    /// zapisuju s bankou).
    /// </summary>
    /// <exception cref="IOException">subor sa neda zapisat</exception>
    /// <exception cref="UnauthorizedAccessException">bez prava na zapis</exception>
    public static void Update(string pathToIniss, string languageKey, Action<LanguageAiData> change)
    {
        ArgumentNullException.ThrowIfNull(change);
        var data = Load(pathToIniss, languageKey);
        change(data);
        Save(pathToIniss, languageKey, data);
    }

    /// <summary>
    /// Ulozi hlas jazyka.
    /// </summary>
    /// <exception cref="IOException">subor sa neda zapisat</exception>
    /// <exception cref="UnauthorizedAccessException">bez prava na zapis</exception>
    public static void SaveVoice(string pathToIniss, string languageKey, VoiceProfile voice) => Update(pathToIniss, languageKey, d => d.Voice = voice);

    /// <summary>
    /// Ulozi zaznamy zvukov jazyka (povod a texty) - nastavenia na disku ostanu. Pri zmene kluca jazyka sa nastavenia
    /// presunu zo suboru <paramref name="oldKey" /> (ak ich novy subor nema) a stary subor sa zmaze.
    /// </summary>
    /// <exception cref="IOException">subor sa neda zapisat</exception>
    /// <exception cref="UnauthorizedAccessException">bez prava na zapis</exception>
    public static void SaveRecords(string pathToIniss, string languageKey, string oldKey, Dictionary<string, SoundOrigin> sounds,
        Dictionary<string, string> texts)
    {
        var data = Load(pathToIniss, languageKey);
        var renamed = !string.Equals(FileOf(pathToIniss, languageKey), FileOf(pathToIniss, oldKey), StringComparison.OrdinalIgnoreCase);
        if (renamed)
        {
            var old = Load(pathToIniss, oldKey);
            data.Voice ??= old.Voice;
            if (data.Prosody.Count == 0)
                data.Prosody = old.Prosody;
            if (data.Pronunciation.Count == 0)
            {
                data.Pronunciation = old.Pronunciation;
                data.NumbersAsWords = old.NumbersAsWords;
            }
        }

        data.Sounds = sounds;
        data.Texts = texts;
        Save(pathToIniss, languageKey, data);
        if (renamed && File.Exists(FileOf(pathToIniss, oldKey)))
            File.Delete(FileOf(pathToIniss, oldKey));
    }

    /// <summary>
    /// Ulozi udaje jazyka (atomicky - najprv docasny subor).
    /// </summary>
    /// <exception cref="IOException">subor sa neda zapisat</exception>
    /// <exception cref="UnauthorizedAccessException">bez prava na zapis</exception>
    public static void Save(string pathToIniss, string languageKey, LanguageAiData data)
    {
        var path = FileOf(pathToIniss, languageKey);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        var temp = path + ".tmp";
        File.WriteAllText(temp, JsonSerializer.Serialize(data, JsonOptions));
        File.Move(temp, path, true);
    }
}
