using System.Text.Json.Serialization;

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
