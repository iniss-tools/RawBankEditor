using System.Text.Json;
using System.Text.Json.Serialization;
using RawBankEditor.Tools;
using ToolsCore.Iniss.Tools;

namespace RawBankEditor.Speech;

/// <summary>
/// Vysledky syntezy na disku podla kluca poziadavky (<see cref="SpeechRequest.CacheKey" />) - opakovane generovanie
/// toho isteho textu s rovnakym hlasom a nastaveniami nic nestoji. Uklada sa surovy zvuk sluzby (WAV), uprava zvuku
/// sa robi vzdy znova.
/// </summary>
/// <param name="root">priecinok cache</param>
public sealed class SpeechCache(string root)
{
    /// <summary>
    /// Priecinok cache.
    /// </summary>
    public string Root { get; } = root;

    /// <summary>
    /// Cache v priecinku udajov programu (<c>%LocalAppData%\RawBankEditor\ai-cache</c>).
    /// </summary>
    public static SpeechCache Default() => new(Path.Combine(ToolsCore.AppPaths.DataDir, "ai-cache"));

    /// <summary>
    /// Vysledok poziadavky je v cache - generovanie nic nestoji.
    /// </summary>
    public bool Contains(string key) => File.Exists(PathOf(key));

    /// <summary>
    /// Vysledok poziadavky s casmi znakov je v cache.
    /// </summary>
    public bool ContainsTimed(string key) => File.Exists(PathOf(key)) && File.Exists(TimingsOf(key));

    /// <summary>
    /// Ulozena rec s casmi znakov; <see langword="null" />, ak v cache nie je alebo sa neda precitat.
    /// </summary>
    public TimedSpeech? TryGetTimed(string key)
    {
        if (!File.Exists(TimingsOf(key)) || TryGet(key) is not { } sound)
            return null;

        try
        {
            var timings = JsonSerializer.Deserialize<List<StoredTiming>>(File.ReadAllText(TimingsOf(key)));
            return timings is null ? null : new TimedSpeech(sound, timings.Select(t => new CharTiming(t.C.Length > 0 ? t.C[0] : ' ', t.S, t.E)).ToList());
        }
        catch (Exception e) when (e is IOException or JsonException or UnauthorizedAccessException)
        {
            Log.Exception(e);
            return null;
        }
    }

    /// <summary>
    /// Ulozi rec s casmi znakov.
    /// </summary>
    public void PutTimed(string key, TimedSpeech speech)
    {
        ArgumentNullException.ThrowIfNull(speech);
        Put(key, speech.Sound);
        try
        {
            var path = TimingsOf(key);
            File.WriteAllText(path + ".tmp", JsonSerializer.Serialize(speech.Timings.Select(t => new StoredTiming(t.Character.ToString(), t.Start, t.End))));
            File.Move(path + ".tmp", path, true);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            Log.Exception(e);
        }
    }

    /// <summary>
    /// Ulozeny text (prepis); <see langword="null" />, ak v cache nie je.
    /// </summary>
    public string? TryGetText(string key)
    {
        try
        {
            return File.Exists(TextOf(key)) ? File.ReadAllText(TextOf(key)) : null;
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            Log.Exception(e);
            return null;
        }
    }

    /// <summary>
    /// Ulozi text (prepis).
    /// </summary>
    public void PutText(string key, string text)
    {
        try
        {
            Directory.CreateDirectory(Root);
            File.WriteAllText(TextOf(key) + ".tmp", text);
            File.Move(TextOf(key) + ".tmp", TextOf(key), true);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            Log.Exception(e);
        }
    }

    /// <summary>
    /// Ulozeny zvuk; <see langword="null" />, ak v cache nie je alebo sa neda precitat.
    /// </summary>
    public PcmSound? TryGet(string key)
    {
        var path = PathOf(key);
        if (!File.Exists(path))
            return null;

        try
        {
            return SoundProcessing.ReadWav(File.ReadAllBytes(path));
        }
        catch (Exception e) when (e is IOException or InvalidDataException or UnauthorizedAccessException)
        {
            Log.Exception(e);
            return null;
        }
    }

    /// <summary>
    /// Ulozi zvuk - zapis je atomicky (najprv docasny subor), cache sa pri chybe len nevyuzije.
    /// </summary>
    public void Put(string key, PcmSound sound)
    {
        try
        {
            Directory.CreateDirectory(Root);
            var path = PathOf(key);
            var temp = path + ".tmp";
            File.WriteAllBytes(temp, SoundProcessing.ToWav(sound));
            File.Move(temp, path, true);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            Log.Exception(e);
        }
    }

    /// <summary>
    /// Velkost cache v bajtoch.
    /// </summary>
    public long SizeBytes() =>
        Directory.Exists(Root) ? new DirectoryInfo(Root).EnumerateFiles().Sum(f => f.Length) : 0;

    /// <summary>
    /// Zmaze vsetky ulozene zvuky.
    /// </summary>
    /// <returns>pocet suborov, ktore sa zmazat nepodarilo.</returns>
    public int Clear()
    {
        if (!Directory.Exists(Root))
            return 0;

        var failed = 0;
        foreach (var file in Directory.EnumerateFiles(Root))
        {
            try
            {
                File.Delete(file);
            }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException)
            {
                failed++;
            }
        }

        return failed;
    }

    private string PathOf(string key) => Path.Combine(Root, key + SoundUtils.WAVExt);

    private string TimingsOf(string key) => Path.Combine(Root, key + ".timing.json");

    private string TextOf(string key) => Path.Combine(Root, key + ".txt");

    private sealed record StoredTiming([property: JsonPropertyName("c")] string C, [property: JsonPropertyName("s")] double S,
        [property: JsonPropertyName("e")] double E);
}
