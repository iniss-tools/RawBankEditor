using System.Globalization;
using System.Security.Cryptography;
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

/// <summary>
/// Filter kniznice hlasov.
/// </summary>
/// <param name="Language">kod jazyka (ISO 639-1); prazdny = vsetky</param>
/// <param name="Gender">pohlavie (male, female, neutral); prazdne = vsetky</param>
/// <param name="Search">hladany text</param>
/// <param name="Page">strana od 0</param>
public sealed record SharedVoiceQuery(string Language, string Gender, string Search, int Page);

/// <summary>
/// Strana vysledkov kniznice.
/// </summary>
/// <param name="Voices">hlasy</param>
/// <param name="HasMore">su dalsie strany</param>
public sealed record SharedVoicePage(IReadOnlyList<SharedVoice> Voices, bool HasMore);

/// <summary>
/// Hlas kniznice.
/// </summary>
/// <param name="OwnerId">verejny identifikator autora</param>
/// <param name="VoiceId">identifikator hlasu</param>
/// <param name="Name">nazov</param>
/// <param name="Gender">pohlavie</param>
/// <param name="Age">vek</param>
/// <param name="Accent">prizvuk</param>
/// <param name="Language">jazyk</param>
/// <param name="Description">popis autora</param>
/// <param name="PreviewUrl">adresa ukazky</param>
public sealed record SharedVoice(string OwnerId, string VoiceId, string Name, string Gender, string Age, string Accent, string Language,
    string Description, string PreviewUrl);

/// <summary>
/// Ukazka navrhnuteho hlasu.
/// </summary>
/// <param name="GeneratedVoiceId">identifikator ukazky pre vytvorenie hlasu</param>
/// <param name="Sound">ukazka</param>
public sealed record VoicePreview(string GeneratedVoiceId, PcmSound Sound);

/// <summary>
/// Klonovanie hlasu z nahravok.
/// </summary>
/// <param name="Name">nazov hlasu</param>
/// <param name="Description">popis</param>
/// <param name="Samples">nahravky (WAV)</param>
/// <param name="RemoveBackgroundNoise">odstranit sum zo vzoriek</param>
/// <param name="LanguageCode">jazyk hlasu (stitok); prazdny = bez stitku</param>
public sealed record VoiceCloneRequest(string Name, string Description, IReadOnlyList<byte[]> Samples, bool RemoveBackgroundNoise, string LanguageCode);

/// <summary>
/// Zmena hlasu nahravky (speech-to-speech): tempo a intonacia ostanu, zmeni sa hlas.
/// </summary>
/// <param name="SourceWav">povodna nahravka (WAV)</param>
/// <param name="VoiceId">cielovy hlas</param>
/// <param name="ModelId">model zmeny hlasu</param>
public sealed record VoiceConversionRequest(byte[] SourceWav, string VoiceId, string ModelId)
{
    /// <summary>Stabilita hlasu 0-1.</summary>
    public double Stability { get; init; } = 0.5;

    /// <summary>Podobnost s hlasom 0-1.</summary>
    public double SimilarityBoost { get; init; } = 0.75;

    /// <summary>Zvyraznenie stylu 0-1.</summary>
    public double Style { get; init; }

    /// <summary>Zvyraznenie podobnosti s hovoriacim.</summary>
    public bool SpeakerBoost { get; init; } = true;

    /// <summary>Seed; <see langword="null" /> = nahodny.</summary>
    public uint? Seed { get; init; }

    /// <summary>Odstranit sum z povodnej nahravky.</summary>
    public bool RemoveBackgroundNoise { get; init; }

    /// <summary>
    /// Kluc cache - nastavenia a obsah povodnej nahravky.
    /// </summary>
    public string CacheKey(string providerId)
    {
        var settings = string.Join('|', providerId, "sts", ModelId, VoiceId, Stability.ToString("R", CultureInfo.InvariantCulture),
            SimilarityBoost.ToString("R", CultureInfo.InvariantCulture), Style.ToString("R", CultureInfo.InvariantCulture), SpeakerBoost ? "1" : "0",
            Seed?.ToString(CultureInfo.InvariantCulture) ?? "-", RemoveBackgroundNoise ? "1" : "0", Convert.ToHexStringLower(SHA256.HashData(SourceWav)));
        return Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(settings)));
    }
}
