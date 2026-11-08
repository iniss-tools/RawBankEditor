namespace RawBankEditor.Speech;

/// <summary>
/// Chyba sluzby syntezy - sprava je od sluzby (anglicky) alebo popis chyby siete.
/// </summary>
public sealed class SpeechException : Exception
{
    /// <summary>Initializes a new instance of the <see cref="SpeechException" /> class.</summary>
    public SpeechException()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="SpeechException" /> class.</summary>
    public SpeechException(string message) : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="SpeechException" /> class.</summary>
    public SpeechException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    /// Stavovy kod HTTP odpovede; <see langword="null" />, ak sluzba neodpovedala.
    /// </summary>
    public int? StatusCode { get; init; }
}
