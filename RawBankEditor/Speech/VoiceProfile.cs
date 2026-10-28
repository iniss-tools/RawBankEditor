using System.Globalization;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using ToolsCore.Iniss.Tools;

namespace RawBankEditor.Speech;

/// <summary>
/// Hlas jazyka banky - sluzba, model, hlas a jeho nastavenia, ktorymi sa generuju nahravky jazyka.
/// </summary>
public sealed class VoiceProfile
{
    /// <summary>Sluzba syntezy (<see cref="ISpeechProvider.Id" />).</summary>
    [JsonPropertyName("provider")]
    public string Provider { get; set; } = ElevenLabsClient.ProviderId;

    /// <summary>Model.</summary>
    [JsonPropertyName("modelId")]
    public string ModelId { get; set; } = ElevenLabsClient.DefaultModel;

    /// <summary>Hlas.</summary>
    [JsonPropertyName("voiceId")]
    public string VoiceId { get; set; } = "";

    /// <summary>Nazov hlasu - len na zobrazenie.</summary>
    [JsonPropertyName("voiceName")]
    public string VoiceName { get; set; } = "";

    /// <summary>Kod jazyka (ISO 639-1); prazdny = model ho urci sam.</summary>
    [JsonPropertyName("languageCode")]
    public string LanguageCode { get; set; } = "";

    /// <summary>Stabilita 0-1.</summary>
    [JsonPropertyName("stability")]
    public double Stability { get; set; } = 0.5;

    /// <summary>Podobnost s hlasom 0-1.</summary>
    [JsonPropertyName("similarityBoost")]
    public double SimilarityBoost { get; set; } = 0.75;

    /// <summary>Zvyraznenie stylu 0-1.</summary>
    [JsonPropertyName("style")]
    public double Style { get; set; }

    /// <summary>Rychlost reci.</summary>
    [JsonPropertyName("speed")]
    public double Speed { get; set; } = 1;

    /// <summary>Zvyraznenie podobnosti s hovoriacim.</summary>
    [JsonPropertyName("speakerBoost")]
    public bool SpeakerBoost { get; set; } = true;

    /// <summary>Pevny seed - rovnaky text da (takmer) rovnaku nahravku; <see langword="null" /> = nahodny.</summary>
    [JsonPropertyName("seed")]
    public uint? Seed { get; set; }

    /// <summary>
    /// Hlas je vybrany - da sa generovat.
    /// </summary>
    [JsonIgnore]
    public bool IsComplete => VoiceId.Length > 0 && ModelId.Length > 0;

    /// <summary>
    /// Poziadavka na prevod textu.
    /// </summary>
    /// <param name="text">text</param>
    /// <param name="seed">seed namiesto <see cref="Seed" /> (generovanie znova)</param>
    public SpeechRequest ToRequest(string text, uint? seed = null) => new(text, VoiceId, ModelId)
    {
        LanguageCode = LanguageCode,
        Stability = Stability,
        SimilarityBoost = SimilarityBoost,
        Style = Style,
        Speed = Speed,
        SpeakerBoost = SpeakerBoost,
        Seed = seed ?? Seed
    };

    /// <summary>Kopia.</summary>
    public VoiceProfile Clone() => (VoiceProfile)MemberwiseClone();

    /// <summary>
    /// Odtlacok hlasu a nastaveni, ktore menia nahravku (bez seedu) - rovnaky odtlacok a text = rovnaky prejav.
    /// </summary>
    public string Fingerprint()
    {
        var text = string.Join('|', Provider, ModelId, VoiceId, LanguageCode, Stability.ToString("R", CultureInfo.InvariantCulture),
            SimilarityBoost.ToString("R", CultureInfo.InvariantCulture), Style.ToString("R", CultureInfo.InvariantCulture),
            Speed.ToString("R", CultureInfo.InvariantCulture), SpeakerBoost ? "1" : "0");
        return Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(text)))[..16];
    }

    /// <summary>
    /// Kod jazyka (ISO 639-1) pre kluc jazyka banky - SK, CZ, GB, D; neznamy kluc = prazdny (model ho urci sam).
    /// </summary>
    public static string LanguageCodeOf(string bankLanguageKey) => bankLanguageKey.ToUpperInvariant() switch
    {
        "SK" => "sk",
        "CZ" => "cs",
        "GB" or "EN" => "en",
        "D" or "DE" => "de",
        "PL" => "pl",
        "H" or "HU" => "hu",
        _ => ""
    };
}

/// <summary>
/// Udaje AI jedneho jazyka banky v <c>&lt;INISS&gt;\.rbe-ai\&lt;kluc jazyka&gt;.json</c> - mimo RAWBANK, INISS ich
/// necita a prieskumnik banky ich nevidi.
/// </summary>
public sealed class LanguageAiData
{
    /// <summary>Verzia formatu.</summary>
    [JsonPropertyName("version")]
    public int Version { get; set; } = 1;

    /// <summary>Hlas jazyka; <see langword="null" />, kym ho pouzivatel nevyberie.</summary>
    [JsonPropertyName("voice")]
    public VoiceProfile? Voice { get; set; }

    /// <summary>Povod nahravok podla kluca skupiny a zvuku (<see cref="SoundRecords.KeyOf" />) - uklada sa s bankou.</summary>
    [JsonPropertyName("sounds")]
    public Dictionary<string, SoundOrigin> Sounds { get; set; } = [];

    /// <summary>Texty pre syntezu podla kluca skupiny a zvuku - ukladaju sa s bankou.</summary>
    [JsonPropertyName("texts")]
    public Dictionary<string, string> Texts { get; set; } = [];

    /// <summary>Intonacia skupin podla kluca skupiny - len skupiny s nastavenim inym nez predvolenym.</summary>
    [JsonPropertyName("prosody")]
    public Dictionary<string, GroupProsody> Prosody { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Slovnik vyslovnosti jazyka.</summary>
    [JsonPropertyName("pronunciation")]
    public List<PronunciationRule> Pronunciation { get; set; } = [];

    /// <summary>Cisla citat slovami (<see cref="NumberWords" />) - predvolene ano.</summary>
    [JsonPropertyName("numbersAsWords")]
    public bool NumbersAsWords { get; set; } = true;

    /// <summary>
    /// Jazyk prevodu cisel na slova pre generator; prazdny, ak je prevod vypnuty alebo jazyk ho nema.
    /// </summary>
    /// <param name="languageCode">jazyk reci (ISO 639-1)</param>
    public string NumbersLanguage(string languageCode) => NumbersAsWords && NumberWords.Supports(languageCode) ? languageCode : "";
}

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
            Log.Exception(e);
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
