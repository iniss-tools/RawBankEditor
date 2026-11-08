using RawBankEditor.Tools;

namespace RawBankEditor.Speech;

/// <summary>
/// Sluzba hlasovej syntezy (ElevenLabs, Azure Speech, Piper). Vysledok je surovy zvuk - upravu na format banky robi
/// <see cref="SpeechGenerator" />. Co sluzba nevie (<see cref="Features" />), hodi <see cref="NotSupportedException" />.
/// </summary>
public interface ISpeechProvider
{
    /// <summary>
    /// Kluc sluzby v profile hlasu a v cache (nemenit).
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Co sluzba vie okrem prevodu textu na rec.
    /// </summary>
    SpeechFeatures Features { get; }

    /// <summary>
    /// Model prepisu na text - sucast kluca cache prepisov; prazdny, ak sluzba prepis nevie.
    /// </summary>
    string TranscriptionModel { get; }

    /// <summary>
    /// Hlasy dostupne v ucte.
    /// </summary>
    Task<IReadOnlyList<SpeechVoice>> GetVoicesAsync(CancellationToken cancel);

    /// <summary>
    /// Modely, ktore vedia prevest text na rec.
    /// </summary>
    Task<IReadOnlyList<SpeechModel>> GetModelsAsync(CancellationToken cancel);

    /// <summary>
    /// Predplatne a minute znaky.
    /// </summary>
    Task<SpeechQuota> GetQuotaAsync(CancellationToken cancel);

    /// <summary>
    /// Prevedie text na rec.
    /// </summary>
    /// <exception cref="SpeechException">sluzba poziadavku odmietla alebo nie je dostupna</exception>
    Task<PcmSound> SynthesizeAsync(SpeechRequest request, CancellationToken cancel);

    /// <summary>
    /// Prevedie text na rec a vrati aj casy jednotlivych znakov textu (vystrihnutie uryvku z nosnej vety).
    /// </summary>
    /// <exception cref="SpeechException">sluzba poziadavku odmietla alebo nie je dostupna</exception>
    Task<TimedSpeech> SynthesizeTimedAsync(SpeechRequest request, CancellationToken cancel);

    /// <summary>
    /// Zmeni hlas nahravky - tempo a intonacia ostanu.
    /// </summary>
    /// <exception cref="SpeechException">sluzba poziadavku odmietla alebo nie je dostupna</exception>
    Task<PcmSound> ConvertVoiceAsync(VoiceConversionRequest request, CancellationToken cancel);

    /// <summary>
    /// Odstrani z nahravky sum a ponecha rec.
    /// </summary>
    /// <param name="sourceWav">nahravka (WAV)</param>
    /// <exception cref="SpeechException">sluzba poziadavku odmietla alebo nie je dostupna</exception>
    Task<PcmSound> IsolateAsync(byte[] sourceWav, CancellationToken cancel);

    /// <summary>
    /// Prepise nahravku na text.
    /// </summary>
    /// <param name="sourceWav">nahravka (WAV, aspon 100 ms)</param>
    /// <param name="languageCode">jazyk reci (ISO 639-1); prazdny = zisti sluzba</param>
    /// <exception cref="SpeechException">sluzba poziadavku odmietla alebo nie je dostupna</exception>
    Task<string> TranscribeAsync(byte[] sourceWav, string languageCode, CancellationToken cancel);

    /// <summary>
    /// Model prijima text pred a za uryvkom (<see cref="SpeechRequest.PreviousText" />), ktory nevyslovi.
    /// </summary>
    bool SupportsContext(string modelId);
}
