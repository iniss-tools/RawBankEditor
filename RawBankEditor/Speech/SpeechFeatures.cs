namespace RawBankEditor.Speech;

/// <summary>
/// Co sluzba vie okrem prevodu textu na rec.
/// </summary>
[Flags]
public enum SpeechFeatures
{
    /// <summary>Len prevod textu na rec.</summary>
    None = 0,

    /// <summary>Casy znakov textu - uryvok sa z nosnej vety vystrihne presne (bez nich sa hranice odhadnu).</summary>
    Timings = 1,

    /// <summary>Stabilita, podobnost, styl a zvyraznenie hovoriaceho.</summary>
    VoiceSettings = 2,

    /// <summary>Seed pre opakovatelny vysledok.</summary>
    Seed = 4,

    /// <summary>Zmena hlasu nahravky.</summary>
    ConvertVoice = 8,

    /// <summary>Vycistenie nahravky od sumu.</summary>
    Isolate = 16,

    /// <summary>Prepis nahravky na text.</summary>
    Transcribe = 32,

    /// <summary>Predplatne a minute znaky.</summary>
    Quota = 64,

    /// <summary>Bezi v pocitaci - nic nestoji a texty neodchadzaju do cloudu.</summary>
    Offline = 128
}
