using System.Globalization;
using System.Security.Cryptography;
using RawBankEditor.Tools;

namespace RawBankEditor.Speech;

/// <summary>
/// Sluzba hlasovej syntezy (ElevenLabs, neskor dalsie). Vysledok je surovy zvuk - upravu na format banky robi
/// <see cref="SpeechGenerator" />.
/// </summary>
public interface ISpeechProvider
{
    /// <summary>
    /// Kluc sluzby v profile hlasu a v cache (nemenit).
    /// </summary>
    string Id { get; }

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

/// <summary>
/// Cas znaku textu v nahravke.
/// </summary>
/// <param name="Character">znak</param>
/// <param name="Start">zaciatok v sekundach</param>
/// <param name="End">koniec v sekundach</param>
public sealed record CharTiming(char Character, double Start, double End);

/// <summary>
/// Rec s casmi znakov textu.
/// </summary>
/// <param name="Sound">rec</param>
/// <param name="Timings">casy znakov v poradi textu</param>
public sealed record TimedSpeech(PcmSound Sound, IReadOnlyList<CharTiming> Timings);

/// <summary>
/// Hlas sluzby.
/// </summary>
/// <param name="Id">identifikator hlasu</param>
/// <param name="Name">nazov</param>
/// <param name="Description">popis (pohlavie, vek, prizvuk, kategoria)</param>
public sealed record SpeechVoice(string Id, string Name, string Description)
{
    /// <summary>Vlastny hlas uctu (klonovany, navrhnuty) - nie predvoleny hlas sluzby.</summary>
    public bool Own { get; init; }

    /// <summary>Druh hlasu podla sluzby (premade, cloned, generated, professional).</summary>
    public string Category { get; init; } = "";

    /// <summary>Adresa ukazky; prazdna = bez ukazky.</summary>
    public string PreviewUrl { get; init; } = "";

    /// <inheritdoc />
    public override string ToString() => Description.Length == 0 ? Name : $"{Name} ({Description})";
}

/// <summary>
/// Model syntezy.
/// </summary>
/// <param name="Id">identifikator modelu</param>
/// <param name="Name">nazov</param>
/// <param name="Languages">kody jazykov, ktore model hovori (ISO 639-1)</param>
public sealed record SpeechModel(string Id, string Name, IReadOnlyList<string> Languages)
{
    /// <inheritdoc />
    public override string ToString() => Name.Length == 0 ? Id : $"{Name} ({Id})";
}

/// <summary>
/// Predplatne sluzby.
/// </summary>
/// <param name="Tier">nazov predplatneho</param>
/// <param name="Used">minute znaky v obdobi</param>
/// <param name="Limit">znaky v obdobi</param>
public sealed record SpeechQuota(string Tier, long Used, long Limit)
{
    /// <summary>
    /// Zostavajuce znaky.
    /// </summary>
    public long Remaining => Math.Max(0, Limit - Used);
}

/// <summary>
/// Poziadavka na prevod textu na rec. Vsetko, co meni vysledok, je sucastou kluca cache.
/// </summary>
/// <param name="Text">text</param>
/// <param name="VoiceId">hlas</param>
/// <param name="ModelId">model</param>
public sealed record SpeechRequest(string Text, string VoiceId, string ModelId)
{
    /// <summary>Kod jazyka (ISO 639-1); prazdny = model ho urci sam.</summary>
    public string LanguageCode { get; init; } = "";

    /// <summary>Stabilita hlasu 0-1 - vyssia = jednotvarnejsi a stalejsi prejav.</summary>
    public double Stability { get; init; } = 0.5;

    /// <summary>Podobnost s povodnym hlasom 0-1.</summary>
    public double SimilarityBoost { get; init; } = 0.75;

    /// <summary>Zvyraznenie stylu hlasu 0-1.</summary>
    public double Style { get; init; }

    /// <summary>Rychlost reci (1 = normalna).</summary>
    public double Speed { get; init; } = 1;

    /// <summary>Zvyraznenie podobnosti s hovoriacim.</summary>
    public bool SpeakerBoost { get; init; } = true;

    /// <summary>Seed pre opakovatelny vysledok; <see langword="null" /> = nahodny.</summary>
    public uint? Seed { get; init; }

    /// <summary>Text pred uryvkom - ovplyvni intonaciu, nevyslovi sa.</summary>
    public string PreviousText { get; init; } = "";

    /// <summary>Text za uryvkom - ovplyvni intonaciu, nevyslovi sa.</summary>
    public string NextText { get; init; } = "";

    /// <summary>
    /// Kluc cache - SHA-256 zo sluzby a vsetkych hodnot poziadavky.
    /// </summary>
    public string CacheKey(string providerId)
    {
        var text = string.Join('\n', providerId, ModelId, VoiceId, LanguageCode,
            Stability.ToString("R", CultureInfo.InvariantCulture), SimilarityBoost.ToString("R", CultureInfo.InvariantCulture),
            Style.ToString("R", CultureInfo.InvariantCulture), Speed.ToString("R", CultureInfo.InvariantCulture),
            SpeakerBoost ? "1" : "0", Seed?.ToString(CultureInfo.InvariantCulture) ?? "-", PreviousText, NextText, Text);
        return Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(text)));
    }
}

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
