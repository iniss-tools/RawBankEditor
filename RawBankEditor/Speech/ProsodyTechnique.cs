using System.Text.Json.Serialization;

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
