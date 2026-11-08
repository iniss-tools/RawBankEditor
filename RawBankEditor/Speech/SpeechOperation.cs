namespace RawBankEditor.Speech;

/// <summary>
/// Co sa s nahravkou robi.
/// </summary>
public enum SpeechOperation
{
    /// <summary>Nova nahravka z textu.</summary>
    Tts,

    /// <summary>Zmena hlasu povodnej nahravky - tempo a intonacia ostanu.</summary>
    ChangeVoice,

    /// <summary>Vycistenie povodnej nahravky od sumu.</summary>
    Isolate
}
