using RawBankEditor.Tools;

namespace RawBankEditor.Speech;

/// <summary>
/// Sprava hlasov uctu: kniznica zdielanych hlasov, navrh hlasu z popisu, klon z nahravok a odstranenie.
/// </summary>
public interface IVoiceLibrary
{
    /// <summary>
    /// Hlasy kniznice podla filtra (strankovane).
    /// </summary>
    Task<SharedVoicePage> SearchSharedAsync(SharedVoiceQuery query, CancellationToken cancel);

    /// <summary>
    /// Prida hlas z kniznice do hlasov uctu.
    /// </summary>
    /// <returns>identifikator hlasu v ucte</returns>
    Task<string> AddSharedAsync(SharedVoice voice, string name, CancellationToken cancel);

    /// <summary>
    /// Navrhne hlasy podla popisu - vrati ukazky, z ktorych sa da hlas vytvorit.
    /// </summary>
    /// <param name="description">popis hlasu</param>
    /// <param name="modelId">model navrhu</param>
    /// <param name="text">text ukazky (100-1000 znakov); prazdny = sluzba ho vymysli</param>
    Task<IReadOnlyList<VoicePreview>> DesignAsync(string description, string modelId, string text, CancellationToken cancel);

    /// <summary>
    /// Vytvori hlas z ukazky navrhu.
    /// </summary>
    /// <returns>identifikator hlasu v ucte</returns>
    Task<string> CreateDesignedAsync(string name, string description, string generatedVoiceId, CancellationToken cancel);

    /// <summary>
    /// Vytvori hlas klonovanim z nahravok (WAV).
    /// </summary>
    /// <returns>identifikator hlasu v ucte</returns>
    Task<string> CloneAsync(VoiceCloneRequest request, CancellationToken cancel);

    /// <summary>
    /// Odstrani hlas z uctu.
    /// </summary>
    Task DeleteVoiceAsync(string voiceId, CancellationToken cancel);

    /// <summary>
    /// Stiahne ukazku hlasu (verejna adresa sluzby).
    /// </summary>
    Task<PcmSound> DownloadPreviewAsync(string url, CancellationToken cancel);
}
