using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text.Json;

namespace RawBankEditor.Speech;

/// <summary>
/// Hlas Piper v zozname hlasov na stiahnutie.
/// </summary>
/// <param name="Key">nazov hlasu (napr. <c>sk_SK-lili-medium</c>)</param>
/// <param name="Family">jazyk (ISO 639-1)</param>
/// <param name="LanguageName">nazov jazyka</param>
/// <param name="Quality">kvalita (x_low, low, medium, high)</param>
/// <param name="Speakers">pocet hovoriacich</param>
/// <param name="Files">subory modelu a nastavenia</param>
public sealed record PiperCatalogVoice(string Key, string Family, string LanguageName, string Quality, int Speakers, IReadOnlyList<PiperCatalogFile> Files)
{
    /// <summary>Velkost na stiahnutie v bajtoch.</summary>
    public long Size => Files.Sum(f => f.Size);
}

/// <summary>
/// Subor hlasu v zozname.
/// </summary>
/// <param name="Path">cesta v ulozisku hlasov</param>
/// <param name="FileName">nazov suboru v priecinku hlasov</param>
/// <param name="Size">velkost v bajtoch</param>
/// <param name="Md5">kontrolny sucet MD5 (hex)</param>
public sealed record PiperCatalogFile(string Path, string FileName, long Size, string Md5);

/// <summary>
/// Zoznam hlasov Piper na stiahnutie (ulozisko rhasspy/piper-voices na Hugging Face) a stiahnutie hlasu do priecinka
/// hlasov s kontrolou velkosti a suctu.
/// </summary>
public sealed class PiperVoiceCatalog : IDisposable
{
    /// <summary>Ulozisko hlasov.</summary>
    public const string BaseUrl = "https://huggingface.co/rhasspy/piper-voices/resolve/main/";

    /// <summary>Stranka s ukazkami hlasov.</summary>
    public const string SamplesUrl = "https://rhasspy.github.io/piper-samples/";

    private readonly HttpClient _http;

    /// <summary>Initializes a new instance of the <see cref="PiperVoiceCatalog" /> class.</summary>
    public PiperVoiceCatalog() : this(new HttpClientHandler())
    {
    }

    /// <param name="handler">spracovanie poziadaviek (testy ho nahradia)</param>
    public PiperVoiceCatalog(HttpMessageHandler handler) =>
        _http = new HttpClient(handler) { BaseAddress = new Uri(BaseUrl), Timeout = TimeSpan.FromMinutes(15) };

    /// <summary>
    /// Nacita zoznam hlasov.
    /// </summary>
    /// <exception cref="SpeechException">zoznam sa neda stiahnut alebo precitat</exception>
    public async Task<IReadOnlyList<PiperCatalogVoice>> LoadAsync(CancellationToken cancel)
    {
        try
        {
            return Parse(await _http.GetStringAsync(new Uri("voices.json", UriKind.Relative), cancel).ConfigureAwait(false));
        }
        catch (HttpRequestException e)
        {
            throw new SpeechException(e.Message, e);
        }
        catch (TaskCanceledException e) when (!cancel.IsCancellationRequested)
        {
            throw new SpeechException(e.Message, e);
        }
        catch (JsonException e)
        {
            throw new SpeechException(e.Message, e);
        }
    }

    /// <summary>
    /// Hlasy zo zoznamu <c>voices.json</c> - len tie, ktorych model a nastavenie maju platny nazov a cestu.
    /// </summary>
    /// <exception cref="JsonException">zoznam nie je platny JSON</exception>
    public static IReadOnlyList<PiperCatalogVoice> Parse(string json)
    {
        using var document = JsonDocument.Parse(json);
        var voices = new List<PiperCatalogVoice>();
        if (document.RootElement.ValueKind != JsonValueKind.Object)
            return voices;

        foreach (var entry in document.RootElement.EnumerateObject())
        {
            var key = entry.Name;
            var voice = entry.Value;
            if (!PiperVoices.IsValidKey(key) || voice.ValueKind != JsonValueKind.Object || !voice.TryGetProperty("files", out var files)
                || files.ValueKind != JsonValueKind.Object)
                continue;

            var wanted = files.EnumerateObject()
                .Select(f => FileOf(key, f))
                .OfType<PiperCatalogFile>()
                .ToList();
            // hlas sa da pouzit len s modelom aj nastavenim
            if (wanted.Count != 2)
                continue;

            var language = voice.TryGetProperty("language", out var l) ? l : default;
            voices.Add(new PiperCatalogVoice(key, StringOf(language, "family").ToLowerInvariant(),
                StringOf(language, "name_native") is { Length: > 0 } native ? native : StringOf(language, "name_english"),
                StringOf(voice, "quality"), voice.TryGetProperty("num_speakers", out var n) && n.TryGetInt32(out var count) ? count : 1, wanted));
        }

        return voices.OrderBy(v => v.Key, StringComparer.OrdinalIgnoreCase).ToList();
    }

    /// <summary>
    /// Stiahne hlas do priecinka hlasov. Kazdy subor ide najprv do docasneho a presunie sa az po kontrole velkosti a
    /// suctu - nedokoncene stiahnutie hlas neposkodi.
    /// </summary>
    /// <param name="voice">hlas</param>
    /// <param name="folder">priecinok hlasov</param>
    /// <param name="progress">stiahnute bajty</param>
    /// <param name="cancel">zrusenie</param>
    /// <exception cref="SpeechException">subor sa neda stiahnut alebo nesedi</exception>
    /// <exception cref="IOException">subor sa neda zapisat</exception>
    /// <exception cref="UnauthorizedAccessException">bez prava na zapis</exception>
    public async Task DownloadAsync(PiperCatalogVoice voice, string folder, IProgress<long>? progress, CancellationToken cancel)
    {
        ArgumentNullException.ThrowIfNull(voice);

        Directory.CreateDirectory(folder);
        long done = 0;
        // nastavenie az za modelom - hlas bez nastavenia sa medzi nainstalovanymi neukaze
        foreach (var file in voice.Files.OrderBy(f => f.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase)))
        {
            var target = Path.Combine(folder, file.FileName);
            var temp = target + ".part";
            try
            {
                await DownloadFileAsync(file, temp, bytes => progress?.Report(done + bytes), cancel).ConfigureAwait(false);
                File.Move(temp, target, true);
            }
            finally
            {
                if (File.Exists(temp))
                    File.Delete(temp);
            }

            done += file.Size;
        }
    }

    [SuppressMessage("Security", "CA5351:Do Not Use Broken Cryptographic Algorithms",
        Justification = "Ulozisko hlasov uvadza len MD5 - kontrola uplnosti stiahnutia, nie bezpecnost.")]
    private async Task DownloadFileAsync(PiperCatalogFile file, string target, Action<long> progress, CancellationToken cancel)
    {
        try
        {
            using var response = await _http.GetAsync(new Uri(file.Path, UriKind.Relative), HttpCompletionOption.ResponseHeadersRead, cancel).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
                throw new SpeechException($"{(int)response.StatusCode} {response.ReasonPhrase}") { StatusCode = (int)response.StatusCode };

            await using var source = await response.Content.ReadAsStreamAsync(cancel).ConfigureAwait(false);
            await using var output = new FileStream(target, FileMode.Create, FileAccess.Write, FileShare.None, 81920, true);
            using var md5 = IncrementalHash.CreateHash(HashAlgorithmName.MD5);
            var buffer = new byte[81920];
            long total = 0;
            int read;
            while ((read = await source.ReadAsync(buffer, cancel).ConfigureAwait(false)) > 0)
            {
                await output.WriteAsync(buffer.AsMemory(0, read), cancel).ConfigureAwait(false);
                md5.AppendData(buffer, 0, read);
                total += read;
                progress(total);
            }

            if (total != file.Size || (file.Md5.Length > 0 && !Convert.ToHexStringLower(md5.GetHashAndReset()).Equals(file.Md5, StringComparison.OrdinalIgnoreCase)))
                throw new SpeechException($"Download of {file.FileName} is incomplete or damaged.");
        }
        catch (HttpRequestException e)
        {
            throw new SpeechException(e.Message, e);
        }
        catch (TaskCanceledException e) when (!cancel.IsCancellationRequested)
        {
            throw new SpeechException(e.Message, e);
        }
    }

    /// <inheritdoc />
    public void Dispose() => _http.Dispose();

    /// <summary>
    /// Model alebo nastavenie hlasu; ine subory (MODEL_CARD) a cesty mimo ulozisko sa vynechaju.
    /// </summary>
    private static PiperCatalogFile? FileOf(string key, JsonProperty file)
    {
        var path = file.Name;
        var name = path[(path.LastIndexOf('/') + 1)..];
        if (name != key + ".onnx" && name != key + ".onnx.json")
            return null;
        if (path.StartsWith('/') || path.Contains('\\', StringComparison.Ordinal) || path.Split('/').Any(s => s is "" or "." or ".."))
            return null;

        var size = file.Value.TryGetProperty("size_bytes", out var s) && s.TryGetInt64(out var bytes) ? bytes : -1;
        return size < 0 ? null : new PiperCatalogFile(path, name, size, StringOf(file.Value, "md5_digest"));
    }

    private static string StringOf(JsonElement element, string property) =>
        element.ValueKind == JsonValueKind.Object && element.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString() ?? ""
            : "";
}
