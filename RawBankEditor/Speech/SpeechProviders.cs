namespace RawBankEditor.Speech;

/// <summary>
/// Nastavene sluzby syntezy - klienta vytvori az pri prvom pouziti a uvolni ich spolu. Hlas jazyka urcuje sluzbu
/// generovania; zmena hlasu a vycistenie su len v ElevenLabs, prepis v ElevenLabs alebo Azure.
/// </summary>
public sealed class SpeechProviders : IDisposable
{
    // poradie v ponuke a pri vybere sluzby pre funkciu (prepis, vycistenie)
    private static readonly string[] Order = [ElevenLabsClient.ProviderId, AzureSpeechClient.ProviderId, PiperClient.ProviderId];

    private readonly Dictionary<string, Func<ISpeechProvider>> _factories = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, ISpeechProvider> _created = new(StringComparer.OrdinalIgnoreCase);
    private readonly bool _owns;
    private readonly Lock _lock = new();

    /// <summary>
    /// Sluzby uz vytvorene volajucim (testy, harness) - neuvolnuju sa.
    /// </summary>
    public SpeechProviders(IEnumerable<ISpeechProvider> providers)
    {
        ArgumentNullException.ThrowIfNull(providers);
        foreach (var provider in providers)
        {
            _created[provider.Id] = provider;
            _factories[provider.Id] = () => provider;
        }
    }

    private SpeechProviders(Dictionary<string, Func<ISpeechProvider>> factories)
    {
        _factories = factories;
        _owns = true;
    }

    /// <summary>
    /// Sluzby z nastaveni programu: ElevenLabs a Azure s klucom, Piper s existujucim programom.
    /// </summary>
    public static SpeechProviders FromSettings(SpeechSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        var factories = new Dictionary<string, Func<ISpeechProvider>>(StringComparer.OrdinalIgnoreCase);
        if (settings.ElevenLabsKey is { Length: > 0 } elevenLabs)
            factories[ElevenLabsClient.ProviderId] = () => new ElevenLabsClient(elevenLabs);
        if (settings.AzureKey is { Length: > 0 } azure && IsValidAzureRegion(settings.AzureRegion))
            factories[AzureSpeechClient.ProviderId] = () => new AzureSpeechClient(azure, settings.AzureRegion);
        if (settings.PiperPath.Length > 0 && File.Exists(settings.PiperPath))
        {
            var (path, voices) = (settings.PiperPath, settings.PiperVoicesPath);
            factories[PiperClient.ProviderId] = () => new PiperClient(path, voices);
        }

        return new SpeechProviders(factories);
    }

    /// <summary>
    /// Oblast alebo adresa prostriedku Azure je platna.
    /// </summary>
    public static bool IsValidAzureRegion(string region)
    {
        try
        {
            AzureSpeechClient.Endpoints(region);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    /// <summary>
    /// Rychlost hlasu, ktoru sluzba prijme - ElevenLabs 0,7 az 1,2, ostatne 0,5 az 2.
    /// </summary>
    public static (double Min, double Max) SpeedRange(string providerId) =>
        string.Equals(providerId, ElevenLabsClient.ProviderId, StringComparison.OrdinalIgnoreCase) ? (0.7, 1.2) : (0.5, 2);

    /// <summary>
    /// Nazov sluzby na zobrazenie.
    /// </summary>
    public static string NameOf(string providerId) => providerId.ToLowerInvariant() switch
    {
        ElevenLabsClient.ProviderId => "ElevenLabs",
        AzureSpeechClient.ProviderId => "Azure Speech",
        PiperClient.ProviderId => "Piper",
        _ => providerId
    };

    /// <summary>
    /// Nastavene sluzby v poradi ponuky.
    /// </summary>
    public IReadOnlyList<string> Ids =>
        _factories.Keys.OrderBy(id => Array.IndexOf(Order, id.ToLowerInvariant()) is var i and >= 0 ? i : int.MaxValue).ThenBy(id => id, StringComparer.Ordinal)
            .ToList();

    /// <summary>
    /// Aspon jedna sluzba je nastavena.
    /// </summary>
    public bool Any => _factories.Count > 0;

    /// <summary>
    /// Sluzba; <see langword="null" />, ak nie je nastavena.
    /// </summary>
    public ISpeechProvider? Get(string providerId)
    {
        lock (_lock)
        {
            if (_created.TryGetValue(providerId, out var provider))
                return provider;
            if (!_factories.TryGetValue(providerId, out var factory))
                return null;

            provider = factory();
            _created[providerId] = provider;
            return provider;
        }
    }

    /// <summary>
    /// Sluzba, ktora vie funkciu - predost <paramref name="preferredId" /> (napr. sluzba hlasu jazyka), inak prva
    /// nastavena v poradi ponuky.
    /// </summary>
    /// <returns>sluzba alebo <see langword="null" />, ak funkciu ziadna nastavena sluzba nevie.</returns>
    public ISpeechProvider? With(SpeechFeatures feature, string preferredId = "")
    {
        foreach (var id in Ids.OrderBy(id => string.Equals(id, preferredId, StringComparison.OrdinalIgnoreCase) ? 0 : 1))
            if (Get(id) is { } provider && provider.Features.HasFlag(feature))
                return provider;
        return null;
    }

    /// <summary>
    /// Sluzba prepisu na text - sluzba hlasu jazyka, ak prepis vie.
    /// </summary>
    public ISpeechProvider? Transcriber(string preferredId) => With(SpeechFeatures.Transcribe, preferredId);

    /// <summary>
    /// Sluzba pre operaciu s hlasom jazyka: nova nahravka a zmena hlasu sluzbou hlasu (zmena hlasu len ak ju vie),
    /// vycistenie lubovolnou sluzbou, ktora ho vie.
    /// </summary>
    /// <returns>sluzba alebo <see langword="null" />, ak operaciu nema kto urobit.</returns>
    public ISpeechProvider? For(SpeechOperation operation, VoiceProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);
        return operation switch
        {
            SpeechOperation.Isolate => With(SpeechFeatures.Isolate, profile.Provider),
            SpeechOperation.ChangeVoice => Get(profile.Provider) is { } p && p.Features.HasFlag(SpeechFeatures.ConvertVoice) ? p : null,
            _ => Get(profile.Provider)
        };
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_owns)
            return;

        lock (_lock)
        {
            foreach (var provider in _created.Values.OfType<IDisposable>())
                provider.Dispose();
            _created.Clear();
        }
    }
}
