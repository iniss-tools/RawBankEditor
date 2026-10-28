using System.Text.Json.Serialization;
using ToolsCore.Iniss.Entities;

namespace RawBankEditor.Speech;

/// <summary>
/// Odkial je nahravka zvuku - zaznam v <c>.rbe-ai</c> pri nahravke vygenerovanej hlasom AI.
/// </summary>
public sealed class SoundOrigin
{
    /// <summary>Druh - prevod textu na rec.</summary>
    public const string KindTts = "tts";

    /// <summary>Druh - zmena hlasu povodnej nahravky.</summary>
    public const string KindSts = "sts";

    /// <summary>Druh vzniku nahravky.</summary>
    [JsonPropertyName("kind")]
    public string Kind { get; set; } = KindTts;

    /// <summary>Sluzba syntezy.</summary>
    [JsonPropertyName("provider")]
    public string Provider { get; set; } = "";

    /// <summary>Model.</summary>
    [JsonPropertyName("modelId")]
    public string ModelId { get; set; } = "";

    /// <summary>Hlas.</summary>
    [JsonPropertyName("voiceId")]
    public string VoiceId { get; set; } = "";

    /// <summary>Nazov hlasu.</summary>
    [JsonPropertyName("voiceName")]
    public string VoiceName { get; set; } = "";

    /// <summary>Odtlacok hlasu, intonacie a vysloveneho textu (<see cref="PreparedSpeech.Fingerprint" />).</summary>
    [JsonPropertyName("fingerprint")]
    public string Fingerprint { get; set; } = "";

    /// <summary>Text na vyslovenie (pred slovnikom vyslovnosti a interpunkciou skupiny).</summary>
    [JsonPropertyName("text")]
    public string Text { get; set; } = "";

    /// <summary>Seed generovania; <see langword="null" /> = nahodny.</summary>
    [JsonPropertyName("seed")]
    public uint? Seed { get; set; }

    /// <summary>Kedy nahravka vznikla.</summary>
    [JsonPropertyName("created")]
    public DateTimeOffset Created { get; set; }

    /// <summary>
    /// Zaznam nahravky vygenerovanej hlasom jazyka.
    /// </summary>
    /// <param name="profile">hlas</param>
    /// <param name="text">text na vyslovenie</param>
    /// <param name="seed">seed generovania</param>
    /// <param name="fingerprint">odtlacok pripravenej poziadavky</param>
    /// <param name="created">cas vzniku</param>
    public static SoundOrigin Generated(VoiceProfile profile, string text, uint? seed, string fingerprint, DateTimeOffset created) => new()
    {
        Provider = profile.Provider,
        ModelId = profile.ModelId,
        VoiceId = profile.VoiceId,
        VoiceName = profile.VoiceName,
        Fingerprint = fingerprint,
        Text = text.Trim(),
        Seed = seed ?? profile.Seed,
        Created = created
    };

    /// <summary>
    /// Zaznam nahravky, ktorej hlas sa zmenil na hlas jazyka.
    /// </summary>
    public static SoundOrigin Converted(VoiceProfile profile, uint? seed, string fingerprint, DateTimeOffset created) => new()
    {
        Kind = KindSts,
        Provider = profile.Provider,
        ModelId = ElevenLabsClient.StsModel,
        VoiceId = profile.VoiceId,
        VoiceName = profile.VoiceName,
        Fingerprint = fingerprint,
        Seed = seed ?? profile.Seed,
        Created = created
    };

    /// <summary>
    /// Nahravka vznikla rovnako (hlas, intonacia a text, pri zmene hlasu rovnaky hlas) - netreba ju robit znova.
    /// </summary>
    public bool SameAs(string fingerprint) => Fingerprint.Length > 0 && Fingerprint == fingerprint;
}

/// <summary>
/// Zaznamy zvukov jedneho jazyka v <c>.rbe-ai</c> podla kluca skupiny a zvuku (bez rozlisenia velkosti pismen ako
/// v INISSe): povod nahravok a text pre syntezu. Menia sa so zvukmi (aj Spat a Znova) a zapisuju sa pri ulozeni banky.
/// </summary>
public sealed class SoundRecords
{
    private readonly Dictionary<string, SoundOrigin> _origins;
    private readonly Dictionary<string, string> _texts;

    /// <param name="origins">nacitane zaznamy povodu</param>
    /// <param name="texts">nacitane texty pre syntezu</param>
    /// <param name="loadedKey">kluc jazyka, pod ktorym sa zaznamy nacitali (subor v <c>.rbe-ai</c>)</param>
    public SoundRecords(IDictionary<string, SoundOrigin>? origins, IDictionary<string, string>? texts, string loadedKey)
    {
        _origins = new Dictionary<string, SoundOrigin>(origins ?? new Dictionary<string, SoundOrigin>(), StringComparer.OrdinalIgnoreCase);
        _texts = new Dictionary<string, string>(texts ?? new Dictionary<string, string>(), StringComparer.OrdinalIgnoreCase);
        LoadedKey = loadedKey;
    }

    /// <summary>
    /// Kluc jazyka, pod ktorym su zaznamy na disku.
    /// </summary>
    public string LoadedKey { get; set; }

    /// <summary>
    /// Zaznamy sa zmenili od nacitania alebo ulozenia.
    /// </summary>
    public bool Changed { get; set; }

    /// <summary>
    /// Kluc zaznamu zvuku.
    /// </summary>
    public static string KeyOf(FyzSound sound) => (sound.Group?.Key ?? "") + "/" + sound.Key;

    /// <summary>
    /// Text zvuku, ak text pre syntezu nie je zadany - text hlasenia, bez neho nazov.
    /// </summary>
    public static string DefaultText(FyzSound sound) => string.IsNullOrWhiteSpace(sound.Text) ? sound.Name : sound.Text;

    /// <summary>
    /// Povod nahravky zvuku; <see langword="null" /> = povodna nahravka banky.
    /// </summary>
    public SoundOrigin? GetOrigin(FyzSound sound) => _origins.GetValueOrDefault(KeyOf(sound));

    /// <summary>
    /// Nastavi alebo zmaze (<see langword="null" />) povod nahravky zvuku.
    /// </summary>
    public void SetOrigin(FyzSound sound, SoundOrigin? origin)
    {
        var key = KeyOf(sound);
        if (origin is null ? _origins.Remove(key) : !ReferenceEquals(_origins.GetValueOrDefault(key), origin))
            Changed = true;
        if (origin is not null)
            _origins[key] = origin;
    }

    /// <summary>
    /// Text pre syntezu zadany pri zvuku; <see langword="null" /> = vyslovi sa <see cref="DefaultText" />.
    /// </summary>
    public string? GetText(FyzSound sound) => _texts.GetValueOrDefault(KeyOf(sound));

    /// <summary>
    /// Text, ktory sa pri generovani vyslovi.
    /// </summary>
    public string SpokenText(FyzSound sound) => GetText(sound) ?? DefaultText(sound);

    /// <summary>
    /// Nastavi text pre syntezu; prazdny alebo rovnaky ako <see cref="DefaultText" /> ho zmaze.
    /// </summary>
    public void SetText(FyzSound sound, string? text)
    {
        var key = KeyOf(sound);
        text = text?.Trim();
        if (string.IsNullOrEmpty(text) || text == DefaultText(sound).Trim())
        {
            if (_texts.Remove(key))
                Changed = true;
            return;
        }

        if (_texts.GetValueOrDefault(key) == text)
            return;
        _texts[key] = text;
        Changed = true;
    }

    /// <summary>
    /// Zaznamy povodu na ulozenie - len zvuky, ktore jazyk este ma (zaznamy odstranenych a premenovanych zvukov sa zahodia).
    /// </summary>
    public Dictionary<string, SoundOrigin> OriginsForSave(FyzLanguage language) => ForSave(_origins, language);

    /// <summary>
    /// Texty pre syntezu na ulozenie - len zvuky, ktore jazyk este ma.
    /// </summary>
    public Dictionary<string, string> TextsForSave(FyzLanguage language) => ForSave(_texts, language);

    private static Dictionary<string, T> ForSave<T>(Dictionary<string, T> items, FyzLanguage language)
    {
        var keys = language.Groups?.SelectMany(g => g.Sounds).Select(KeyOf).ToHashSet(StringComparer.OrdinalIgnoreCase);
        return items.Where(i => keys is null || keys.Contains(i.Key))
            .OrderBy(i => i.Key, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(i => i.Key, i => i.Value, StringComparer.OrdinalIgnoreCase);
    }
}
