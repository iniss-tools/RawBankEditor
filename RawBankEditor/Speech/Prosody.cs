using System.Text.Json.Serialization;
using ToolsCore.Iniss.Entities;

namespace RawBankEditor.Speech;

/// <summary>
/// Sposob, akym sa pri generovani zachova intonacia uryvku podla jeho miesta vo vete.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<ProsodyTechnique>))]
public enum ProsodyTechnique
{
    /// <summary>Uryvok sa vyslovi samostatne (cele vety, hlasenia).</summary>
    [JsonStringEnumMemberName("none")]
    None,

    /// <summary>Sluzba dostane text pred a za uryvkom, ktory ovplyvni intonaciu, ale nevyslovi ho.</summary>
    [JsonStringEnumMemberName("context")]
    Context,

    /// <summary>Vyslovi sa cela nosna veta a uryvok sa z nej vystrihne podla casov znakov.</summary>
    [JsonStringEnumMemberName("carrier")]
    Carrier
}

/// <summary>
/// Interpunkcia za uryvkom - urcuje, ci intonacia pokracuje (ciarka) alebo klesa (koniec vety).
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<ProsodyPunctuation>))]
public enum ProsodyPunctuation
{
    /// <summary>Bez interpunkcie.</summary>
    [JsonStringEnumMemberName("none")]
    None,

    /// <summary>Ciarka - veta pokracuje.</summary>
    [JsonStringEnumMemberName("comma")]
    Comma,

    /// <summary>Bodka - koniec vety.</summary>
    [JsonStringEnumMemberName("period")]
    Period
}

/// <summary>
/// Intonacia skupiny zvukov - sposob, text pred a za uryvkom (kontext alebo nosna veta) a interpunkcia.
/// </summary>
public sealed class GroupProsody
{
    /// <summary>Sposob.</summary>
    [JsonPropertyName("technique")]
    public ProsodyTechnique Technique { get; set; }

    /// <summary>Text pred uryvkom.</summary>
    [JsonPropertyName("before")]
    public string Before { get; set; } = "";

    /// <summary>Text za uryvkom.</summary>
    [JsonPropertyName("after")]
    public string After { get; set; } = "";

    /// <summary>Interpunkcia za uryvkom.</summary>
    [JsonPropertyName("punctuation")]
    public ProsodyPunctuation Punctuation { get; set; }

    /// <summary>Kopia.</summary>
    public GroupProsody Clone() => (GroupProsody)MemberwiseClone();

    /// <summary>
    /// Rovnake nastavenie.
    /// </summary>
    public bool SameAs(GroupProsody other) =>
        Technique == other.Technique && Before == other.Before && After == other.After && Punctuation == other.Punctuation;

    /// <summary>
    /// Text uryvku s interpunkciou - ak uz interpunkciu ma, neprida sa.
    /// </summary>
    public string Spoken(string text)
    {
        text = text.Trim();
        if (text.Length == 0 || Punctuation == ProsodyPunctuation.None || char.IsPunctuation(text[^1]))
            return text;
        return text + (Punctuation == ProsodyPunctuation.Comma ? "," : ".");
    }
}

/// <summary>
/// Predvolena intonacia skupiny podla typu skupiny INISS (popisy typov: "s ciarkou", "na konci vety", "na zaciatku
/// vety") a nosne texty v jazyku banky.
/// </summary>
public static class ProsodyDefaults
{
    private enum Position
    {
        // cela veta alebo samostatne hlasenie
        Whole,
        // uprostred vety
        Middle,
        // pred ciarkou - veta pokracuje
        Comma,
        // na konci vety
        End,
        // na zaciatku vety
        Start
    }

    private static readonly FyzGroupType[] CommaTypes =
    [
        FyzGroupType.R1, FyzGroupType.R2, FyzGroupType.C3, FyzGroupType.Cislo, FyzGroupType.Cislo5, FyzGroupType.Cislo8, FyzGroupType.K2,
        FyzGroupType.N1, FyzGroupType.N2, FyzGroupType.N3, FyzGroupType.N5, FyzGroupType.N11, FyzGroupType.Poz2
    ];

    private static readonly FyzGroupType[] EndTypes =
    [
        FyzGroupType.R4, FyzGroupType.C2, FyzGroupType.Cislo3, FyzGroupType.Cislo6, FyzGroupType.Cislo9, FyzGroupType.K1, FyzGroupType.N10,
        FyzGroupType.N13, FyzGroupType.Poz3
    ];

    private static readonly FyzGroupType[] StartTypes = [FyzGroupType.V4, FyzGroupType.V14, FyzGroupType.K3, FyzGroupType.N4, FyzGroupType.N12];

    private static readonly FyzGroupType[] MiddleTypes =
    [
        FyzGroupType.R3, FyzGroupType.C1, FyzGroupType.C9, FyzGroupType.Cislo1, FyzGroupType.Cislo2, FyzGroupType.Cislo4, FyzGroupType.Cislo7,
        FyzGroupType.V1, FyzGroupType.V2, FyzGroupType.V8, FyzGroupType.Poz1, FyzGroupType.Poz7, FyzGroupType.K4, FyzGroupType.Vlaknum,
        FyzGroupType.Linka, FyzGroupType.Dopravca
    ];

    // nosne texty: pred uryvkom, za uryvkom uprostred vety, za uryvkom s ciarkou
    private static readonly Dictionary<string, (string Before, string After, string AfterComma)> Carriers = new(StringComparer.OrdinalIgnoreCase)
    {
        ["sk"] = ("Osobný vlak pokračuje", "a ďalej podľa cestovného poriadku.", "potom ďalej podľa cestovného poriadku."),
        ["cs"] = ("Osobní vlak pokračuje", "a dále podle jízdního řádu.", "potom dále podle jízdního řádu."),
        ["en"] = ("The passenger train continues", "and then according to the timetable.", "then according to the timetable."),
        ["de"] = ("Der Personenzug fährt weiter", "und dann nach Fahrplan.", "dann weiter nach Fahrplan.")
    };

    /// <summary>
    /// Predvolena intonacia skupiny.
    /// </summary>
    /// <param name="type">typ skupiny</param>
    /// <param name="languageCode">kod jazyka reci (ISO 639-1); jazyk bez nosnych textov ma len interpunkciu</param>
    public static GroupProsody For(FyzGroupType type, string languageCode)
    {
        var position = PositionOf(type);
        if (position == Position.Whole)
            return new GroupProsody { Technique = ProsodyTechnique.None };

        var (before, after, afterComma) = Carriers.GetValueOrDefault(languageCode, ("", "", ""));
        return new GroupProsody
        {
            Technique = ProsodyTechnique.Context,
            Before = position == Position.Start ? "" : before,
            After = position switch
            {
                Position.End => "",
                Position.Comma => afterComma,
                _ => after
            },
            Punctuation = position switch
            {
                Position.Comma => ProsodyPunctuation.Comma,
                Position.End => ProsodyPunctuation.Period,
                _ => ProsodyPunctuation.None
            }
        };
    }

    private static Position PositionOf(FyzGroupType type) =>
        CommaTypes.Contains(type) ? Position.Comma
        : EndTypes.Contains(type) ? Position.End
        : StartTypes.Contains(type) ? Position.Start
        : MiddleTypes.Contains(type) ? Position.Middle
        : Position.Whole;
}

/// <summary>
/// Text uryvku na generovanie s intonaciou jeho skupiny.
/// </summary>
/// <param name="Text">text na vyslovenie (bez interpunkcie skupiny)</param>
/// <param name="Prosody">intonacia; <see langword="null" /> = uryvok samostatne</param>
public sealed record SpeechInput(string Text, GroupProsody? Prosody = null);
