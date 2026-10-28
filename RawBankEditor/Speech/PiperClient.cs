using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using RawBankEditor.Tools;

namespace RawBankEditor.Speech;

/// <summary>
/// Piper - hlasova synteza v pocitaci bez pripojenia (program <c>piper.exe</c> a hlasy <c>.onnx</c> v priecinku).
/// Kazda poziadavka spusti program, text mu posle na standardny vstup a precita WAV. Casy znakov ani kontext
/// nevie - nosna veta sa striha podla odhadu (<see cref="SpeechGenerator" />).
/// </summary>
public sealed class PiperClient : ISpeechProvider
{
    /// <summary>Kluc sluzby v profile hlasu.</summary>
    public const string ProviderId = "piper";

    /// <summary>Jediny model - hlas je subor modelu.</summary>
    public const string Model = "piper";

    /// <summary>Stranka na stiahnutie programu Piper pre Windows.</summary>
    public const string DownloadUrl = "https://github.com/rhasspy/piper/releases/latest";

    // oddelenie hovoriaceho vo viachlasovom modeli: model#cislo
    private const char SPEAKER_SEPARATOR = '#';
    private static readonly TimeSpan Timeout = TimeSpan.FromMinutes(2);

    private readonly string _executable;
    private readonly string _voicesFolder;

    /// <param name="executable">program <c>piper.exe</c> (vydanie pre Windows aj <c>piper</c> z balika piper-tts)</param>
    /// <param name="voicesFolder">priecinok hlasov (<c>nazov.onnx</c> a <c>nazov.onnx.json</c>)</param>
    public PiperClient(string executable, string voicesFolder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(executable);
        ArgumentException.ThrowIfNullOrWhiteSpace(voicesFolder);
        _executable = executable;
        _voicesFolder = voicesFolder;
    }

    /// <summary>
    /// Spustenie programu (testy ho nahradia).
    /// </summary>
    internal Func<PiperRun, CancellationToken, Task<PiperResult>> Run { get; set; } = RunProcessAsync;

    /// <inheritdoc />
    public string Id => ProviderId;

    /// <inheritdoc />
    public SpeechFeatures Features => SpeechFeatures.Offline;

    /// <inheritdoc />
    public string TranscriptionModel => "";

    /// <inheritdoc />
    public Task<IReadOnlyList<SpeechVoice>> GetVoicesAsync(CancellationToken cancel) =>
        Task.Run<IReadOnlyList<SpeechVoice>>(() => PiperVoices.Installed(_voicesFolder).SelectMany(VoicesOf).ToList(), cancel);

    /// <inheritdoc />
    public async Task<IReadOnlyList<SpeechModel>> GetModelsAsync(CancellationToken cancel)
    {
        var voices = await GetVoicesAsync(cancel).ConfigureAwait(false);
        return [new SpeechModel(Model, "Piper", voices.SelectMany(v => v.Languages).Distinct().ToList())];
    }

    /// <inheritdoc />
    public Task<SpeechQuota> GetQuotaAsync(CancellationToken cancel) => throw new NotSupportedException();

    /// <inheritdoc />
    public async Task<PcmSound> SynthesizeAsync(SpeechRequest request, CancellationToken cancel)
    {
        ArgumentNullException.ThrowIfNull(request);

        var (key, speaker) = SplitVoice(request.VoiceId);
        var model = PiperVoices.ModelPath(_voicesFolder, key);
        if (!File.Exists(model))
            throw new SpeechException($"Voice {key} is not installed.");

        var output = Path.Combine(Path.GetTempPath(), $"rbe-piper-{Guid.NewGuid():N}.wav");
        var arguments = new List<string>
        {
            "--model", model,
            "--output_file", output,
            // dlzka hlasok je opak rychlosti
            "--length_scale", (1 / Math.Clamp(request.Speed, 0.5, 2)).ToString("0.###", CultureInfo.InvariantCulture)
        };
        if (speaker is { } id)
            arguments.AddRange(["--speaker", id.ToString(CultureInfo.InvariantCulture)]);

        try
        {
            // text na jeden riadok - kazdy riadok je pre Piper samostatna veta
            var result = await Run(new PiperRun(_executable, arguments, request.Text.ReplaceLineEndings(" ")), cancel).ConfigureAwait(false);
            if (result.ExitCode != 0 || !File.Exists(output))
                throw new SpeechException(result.Error.Length > 0 ? result.Error : $"Piper exited with code {result.ExitCode}.");

            return SoundProcessing.ReadWav(await File.ReadAllBytesAsync(output, cancel).ConfigureAwait(false));
        }
        catch (InvalidDataException e)
        {
            throw new SpeechException(e.Message, e);
        }
        catch (IOException e)
        {
            throw new SpeechException(e.Message, e);
        }
        finally
        {
            try
            {
                File.Delete(output);
            }
            catch (IOException)
            {
                // docasny subor zmaze system
            }
        }
    }

    /// <inheritdoc />
    public Task<TimedSpeech> SynthesizeTimedAsync(SpeechRequest request, CancellationToken cancel) => throw new NotSupportedException();

    /// <inheritdoc />
    public Task<PcmSound> ConvertVoiceAsync(VoiceConversionRequest request, CancellationToken cancel) => throw new NotSupportedException();

    /// <inheritdoc />
    public Task<PcmSound> IsolateAsync(byte[] sourceWav, CancellationToken cancel) => throw new NotSupportedException();

    /// <inheritdoc />
    public Task<string> TranscribeAsync(byte[] sourceWav, string languageCode, CancellationToken cancel) => throw new NotSupportedException();

    /// <inheritdoc />
    public bool SupportsContext(string modelId) => false;

    /// <summary>
    /// Hlas a hovoriaci z identifikatora hlasu (<c>model</c> alebo <c>model#cislo</c>).
    /// </summary>
    internal static (string Key, int? Speaker) SplitVoice(string voiceId)
    {
        var index = voiceId.LastIndexOf(SPEAKER_SEPARATOR);
        return index > 0 && int.TryParse(voiceId.AsSpan(index + 1), NumberStyles.None, CultureInfo.InvariantCulture, out var speaker)
            ? (voiceId[..index], speaker)
            : (voiceId, null);
    }

    /// <summary>
    /// Hlasy nainstalovaneho modelu - pri viacerych hovoriacich kazdy zvlast.
    /// </summary>
    private static IEnumerable<SpeechVoice> VoicesOf(InstalledPiperVoice voice)
    {
        var description = string.Join(", ", new[] { voice.LanguageName, voice.Quality }.Where(s => s.Length > 0));
        IReadOnlyList<string> languages = voice.Family.Length > 0 ? [voice.Family] : [];
        if (voice.Speakers.Count <= 1)
            return [new SpeechVoice(voice.Key, voice.Key, description) { Languages = languages, Category = voice.Quality }];

        return voice.Speakers.OrderBy(s => s.Value).Select(s => new SpeechVoice($"{voice.Key}{SPEAKER_SEPARATOR}{s.Value.ToString(CultureInfo.InvariantCulture)}",
            $"{voice.Key} – {s.Key}", description) { Languages = languages, Category = voice.Quality });
    }

    /// <summary>
    /// Spusti Piper bez okna, posle text v UTF-8 a pocka na koniec; pri zruseni alebo prekroceni casu ho ukonci.
    /// </summary>
    private static async Task<PiperResult> RunProcessAsync(PiperRun run, CancellationToken cancel)
    {
        var info = new ProcessStartInfo(run.Executable)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            StandardInputEncoding = new UTF8Encoding(false),
            StandardErrorEncoding = Encoding.UTF8,
            StandardOutputEncoding = Encoding.UTF8,
            // vydanie pre Windows hlada espeak-ng-data vedla programu
            WorkingDirectory = Path.GetDirectoryName(Path.GetFullPath(run.Executable)) ?? ""
        };
        foreach (var argument in run.Arguments)
            info.ArgumentList.Add(argument);
        // piper z balika piper-tts (Python) cita vstup v kodovani konzoly
        info.Environment["PYTHONIOENCODING"] = "utf-8";
        info.Environment["PYTHONUTF8"] = "1";

        using var process = new Process { StartInfo = info };
        try
        {
            process.Start();
        }
        catch (Exception e) when (e is Win32Exception or InvalidOperationException)
        {
            throw new SpeechException(e.Message, e);
        }

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancel);
        timeout.CancelAfter(Timeout);
        try
        {
            var output = process.StandardOutput.ReadToEndAsync(timeout.Token);
            var error = process.StandardError.ReadToEndAsync(timeout.Token);
            await process.StandardInput.WriteLineAsync(run.Input.AsMemory(), timeout.Token).ConfigureAwait(false);
            process.StandardInput.Close();
            await process.WaitForExitAsync(timeout.Token).ConfigureAwait(false);
            await output.ConfigureAwait(false);
            return new PiperResult(process.ExitCode, LastLines(await error.ConfigureAwait(false)));
        }
        catch (OperationCanceledException) when (!process.HasExited)
        {
            try
            {
                process.Kill(true);
            }
            catch (InvalidOperationException)
            {
                // medzitym skoncil
            }

            if (cancel.IsCancellationRequested)
                throw;
            throw new SpeechException("Piper did not finish in time.");
        }
        catch (IOException e)
        {
            // program skoncil skor, nez precital text
            throw new SpeechException(e.Message, e);
        }
    }

    // z chyboveho vystupu len posledne riadky - Python pri chybe vypise cely zasobnik
    private static string LastLines(string text) =>
        string.Join(Environment.NewLine, text.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).TakeLast(3));
}

/// <summary>
/// Spustenie programu Piper.
/// </summary>
/// <param name="Executable">program</param>
/// <param name="Arguments">parametre</param>
/// <param name="Input">text na standardny vstup</param>
internal sealed record PiperRun(string Executable, IReadOnlyList<string> Arguments, string Input);

/// <summary>
/// Vysledok programu Piper.
/// </summary>
/// <param name="ExitCode">navratovy kod</param>
/// <param name="Error">posledne riadky chyboveho vystupu</param>
internal sealed record PiperResult(int ExitCode, string Error);

/// <summary>
/// Nainstalovany hlas Piper - model a jeho nastavenie (<c>.onnx.json</c>).
/// </summary>
/// <param name="Key">nazov hlasu (subor bez pripony)</param>
/// <param name="Family">jazyk (ISO 639-1)</param>
/// <param name="LanguageName">nazov jazyka</param>
/// <param name="Quality">kvalita (x_low, low, medium, high)</param>
/// <param name="Speakers">hovoriaci viachlasoveho modelu (nazov - cislo); prazdne = jeden hlas</param>
public sealed record InstalledPiperVoice(string Key, string Family, string LanguageName, string Quality, IReadOnlyDictionary<string, int> Speakers);

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
                ToolsCore.Iniss.Tools.Log.Exception(e);
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
