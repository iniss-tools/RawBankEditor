using System.Text.Json.Serialization;

namespace RawBankEditor.Speech;

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
