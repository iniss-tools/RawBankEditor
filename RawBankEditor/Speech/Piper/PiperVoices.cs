using System.Text.Json;
using Serilog;

namespace RawBankEditor.Speech.Piper;

/// <summary>
/// Hlasy Piper v priecinku hlasov.
/// </summary>
public static class PiperVoices
{
    /// <summary>
    /// Predvoleny priecinok hlasov (<c>%LocalAppData%\RawBankEditor\piper</c>).
    /// </summary>
    public static string DefaultFolder => Path.Combine(ToolsCore.AppPaths.DataDir, "piper");

    /// <summary>
    /// Nazov hlasu je platny nazov suboru (pismena, cislice, <c>_</c>, <c>-</c>, <c>.</c>) - nemoze ukazovat mimo priecinka.
    /// </summary>
    public static bool IsValidKey(string key) =>
        key.Length > 0 && !key.StartsWith('.') && key.All(c => char.IsAsciiLetterOrDigit(c) || c is '_' or '-' or '.') && !key.Contains("..", StringComparison.Ordinal);

    /// <summary>
    /// Subor modelu hlasu.
    /// </summary>
    /// <exception cref="SpeechException">nazov hlasu nie je platny</exception>
    public static string ModelPath(string folder, string key) =>
        IsValidKey(key) ? Path.Combine(folder, key + ".onnx") : throw new SpeechException($"Invalid voice name {key}.");

    /// <summary>
    /// Nainstalovane hlasy - modely, ktore maju nastavenie; poskodene nastavenie hlas vynecha.
    /// </summary>
    public static IReadOnlyList<InstalledPiperVoice> Installed(string folder)
    {
        if (!Directory.Exists(folder))
            return [];

        var voices = new List<InstalledPiperVoice>();
        foreach (var model in Directory.EnumerateFiles(folder, "*.onnx").Order(StringComparer.OrdinalIgnoreCase))
        {
            var key = Path.GetFileNameWithoutExtension(model);
            if (!IsValidKey(key) || !File.Exists(model + ".json"))
                continue;

            try
            {
                voices.Add(ReadConfig(key, File.ReadAllText(model + ".json")));
            }
            catch (Exception e) when (e is IOException or JsonException or UnauthorizedAccessException)
            {
                Log.Warning(e, "Nastavenie hlasu Piper {File} je poskodene - hlas sa vynecha", model + ".json");
            }
        }

        return voices;
    }

    /// <summary>
    /// Hlas z nastavenia modelu: <c>language</c> (<c>family</c>, <c>name_native</c>), <c>audio.quality</c> a
    /// <c>speaker_id_map</c>.
    /// </summary>
    /// <exception cref="JsonException">nastavenie nie je platny JSON</exception>
    internal static InstalledPiperVoice ReadConfig(string key, string json)
    {
        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        var language = root.TryGetProperty("language", out var l) && l.ValueKind == JsonValueKind.Object ? l : default;
        var family = StringOf(language, "family");
        if (family.Length == 0 && StringOf(language, "code") is { Length: > 0 } code)
            family = code.Split('_', '-')[0];
        // starsie modely maju len hlas eSpeak (napr. "sk")
        if (family.Length == 0 && root.TryGetProperty("espeak", out var espeak))
            family = StringOf(espeak, "voice").Split('-')[0];

        var quality = root.TryGetProperty("audio", out var audio) ? StringOf(audio, "quality") : "";
        var speakers = new Dictionary<string, int>(StringComparer.Ordinal);
        if (root.TryGetProperty("speaker_id_map", out var map) && map.ValueKind == JsonValueKind.Object)
            foreach (var speaker in map.EnumerateObject().Where(s => s.Value.ValueKind == JsonValueKind.Number && s.Value.TryGetInt32(out _)))
                speakers[speaker.Name] = speaker.Value.GetInt32();

        return new InstalledPiperVoice(key, family.ToLowerInvariant(), StringOf(language, "name_native"), quality, speakers);
    }

    /// <summary>
    /// Zmaze hlas (model a nastavenie).
    /// </summary>
    /// <exception cref="IOException">subor sa neda zmazat</exception>
    /// <exception cref="UnauthorizedAccessException">bez prava na zapis</exception>
    public static void Delete(string folder, string key)
    {
        var model = ModelPath(folder, key);
        File.Delete(model + ".json");
        File.Delete(model);
    }

    private static string StringOf(JsonElement element, string property) =>
        element.ValueKind == JsonValueKind.Object && element.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString() ?? ""
            : "";
}
