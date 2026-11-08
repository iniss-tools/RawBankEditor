using RawBankEditor.Speech;
using RawBankEditor.Tools;

namespace RawBankEditor.DocScreenshots;

/// <summary>
/// Služba hlasu pre snímky - hlasy, modely a knižnica ako v ElevenLabs, „reč“ je tón s dĺžkou podľa počtu slabík
/// a prepis vráti text, z ktorého nahrávka vznikla (jedna stanica zámerne nesedí, jedna je pomalá).
/// </summary>
internal sealed class DemoSpeech : ISpeechProvider, IVoiceLibrary
{
    public const string BankVoiceId = "demo-milena";
    public const string BankVoiceName = "Milena";
    // pomalá veta a stanica, ktorej prepis nesedí
    public const string SlowKey = "D1001";
    private const string SlowText = "náhradnou autobusovou";
    private const string MisheardText = "Podhradie";
    private const string MisheardAs = "Pod hradom.";
    private const int RATE = 22050;


    public string Id => ElevenLabsClient.ProviderId;

    public SpeechFeatures Features => SpeechFeatures.Timings | SpeechFeatures.VoiceSettings | SpeechFeatures.Seed | SpeechFeatures.ConvertVoice
                                      | SpeechFeatures.Isolate | SpeechFeatures.Transcribe | SpeechFeatures.Quota;

    public string TranscriptionModel => "demo";

    public Task<IReadOnlyList<SpeechVoice>> GetVoicesAsync(CancellationToken cancel) =>
        Task.FromResult<IReadOnlyList<SpeechVoice>>(
        [
            new(BankVoiceId, BankVoiceName, "female, middle aged, slovak") { Own = true, Category = "cloned", Languages = ["sk"] },
            new("demo-zuzana", "Zuzana – pokojná", "female, young, slovak") { Own = true, Category = "professional", Languages = ["sk", "cs"] },
            new("demo-adam", "Adam – Dominant, Firm", "male, middle aged, american") { Category = "premade", Languages = ["en", "sk", "cs", "de"] },
            new("demo-brian", "Brian – Deep, Resonant", "male, middle aged, american") { Category = "premade", Languages = ["en", "sk", "cs", "de"] }
        ]);

    public Task<IReadOnlyList<SpeechModel>> GetModelsAsync(CancellationToken cancel) =>
        Task.FromResult<IReadOnlyList<SpeechModel>>(
        [
            new(ElevenLabsClient.DefaultModel, "Eleven Multilingual v2", ["sk", "cs", "en", "de"]),
            new("eleven_v3", "Eleven v3", ["sk", "cs", "en", "de"]),
            new("eleven_flash_v2_5", "Eleven Flash v2.5", ["sk", "cs", "en", "de"])
        ]);

    public Task<SpeechQuota> GetQuotaAsync(CancellationToken cancel) => Task.FromResult(new SpeechQuota("creator", 41_850, 100_000));

    public Task<PcmSound> SynthesizeAsync(SpeechRequest request, CancellationToken cancel)
    {
        var text = request.Text;
        // pomalá veta trvá o tretinu dlhšie
        var ms = SpeechTempo.Syllables(text) * 190 * (text.Contains(SlowText, StringComparison.Ordinal) ? 1.35 : 1) + 40;
        return Task.FromResult(Tone(text, (int)(ms * RATE / 1000)));
    }

    private static PcmSound Tone(string text, int count)
    {
        var frequency = DemoTones.Frequency(text);
        var samples = new float[count];
        for (var i = 0; i < count; i++)
        {
            var envelope = Math.Min(1f, Math.Min(i, count - i) / (RATE * 0.02f));
            samples[i] = 0.25f * envelope * (float)Math.Sin(2 * Math.PI * frequency * i / RATE);
        }

        return new PcmSound(samples, RATE);
    }

    /// <summary>
    /// Text tónu podľa jeho výšky - prechody nulou od prvej po poslednú periódu (ticho na okrajoch sa nepočíta).
    /// </summary>
    private static string? Heard(float[] samples)
    {
        var peak = samples.Select(Math.Abs).DefaultIfEmpty(0).Max();
        var first = Array.FindIndex(samples, x => Math.Abs(x) > peak * 0.5f);
        var last = Array.FindLastIndex(samples, x => Math.Abs(x) > peak * 0.5f);
        if (first < 0 || last <= first)
            return null;

        var crossings = new List<int>();
        for (var i = first + 1; i <= last; i++)
        {
            if (samples[i - 1] < 0 && samples[i] >= 0)
                crossings.Add(i);
        }

        // celé periódy medzi prvým a posledným prechodom nahor
        return crossings.Count < 2 ? null : DemoTones.TextOf((crossings.Count - 1) / ((double)(crossings[^1] - crossings[0]) / RATE));
    }

    public async Task<TimedSpeech> SynthesizeTimedAsync(SpeechRequest request, CancellationToken cancel)
    {
        var sound = await SynthesizeAsync(request, cancel);
        var step = sound.DurationMs / 1000.0 / request.Text.Length;
        return new TimedSpeech(sound, request.Text.Select((c, i) => new CharTiming(c, i * step, (i + 1) * step)).ToList());
    }

    public bool SupportsContext(string modelId) => true;

    public Task<string> TranscribeAsync(byte[] sourceWav, string languageCode, CancellationToken cancel)
    {
        var samples = SoundProcessing.ReadWav(sourceWav).Samples;
        var text = Heard(samples) ?? "";
        return Task.FromResult(text.Contains(MisheardText, StringComparison.Ordinal) ? MisheardAs : text);
    }

    // nový hlas povie to isté približne rovnako dlho
    public Task<PcmSound> ConvertVoiceAsync(VoiceConversionRequest request, CancellationToken cancel)
    {
        var samples = SoundProcessing.ReadWav(request.SourceWav).Samples;
        return Task.FromResult(Tone(Heard(samples) ?? "", samples.Length * 97 / 100));
    }

    public Task<PcmSound> IsolateAsync(byte[] sourceWav, CancellationToken cancel) => SynthesizeAsync(new SpeechRequest("Lipová", BankVoiceId, ""), cancel);

    public Task<SharedVoicePage> SearchSharedAsync(SharedVoiceQuery query, CancellationToken cancel) =>
        Task.FromResult(new SharedVoicePage(
        [
            new SharedVoice("o1", "s1", "Zuzka – pokojná hlásateľka", "female", "middle_aged", "standard", "sk", "Pokojný a zreteľný hlas pre hlásenia.", ""),
            new SharedVoice("o2", "s2", "Peter – rozhlas", "male", "old", "standard", "sk", "Hlboký rozhlasový hlas.", ""),
            new SharedVoice("o3", "s3", "Katarína", "female", "young", "standard", "sk", "Mladý, priateľský hlas.", ""),
            new SharedVoice("o4", "s4", "Marek – správy", "male", "middle_aged", "standard", "sk", "Spravodajský prednes, rýchlejšie tempo.", "")
        ], true));

    public Task<string> AddSharedAsync(SharedVoice voice, string name, CancellationToken cancel) => Task.FromResult("added");

    public async Task<IReadOnlyList<VoicePreview>> DesignAsync(string description, string modelId, string text, CancellationToken cancel) =>
    [
        new("p1", await SynthesizeAsync(new SpeechRequest("Vážení cestujúci, osobný vlak do Dolného Mesta príde na prvú koľaj.", "", ""), cancel)),
        new("p2", await SynthesizeAsync(new SpeechRequest("Vážení cestujúci, osobný vlak do Dolného Mesta príde na druhú koľaj.", "", ""), cancel)),
        new("p3", await SynthesizeAsync(new SpeechRequest("Vážení cestujúci, rýchlik do Dolného Mesta mešká asi desať minút.", "", ""), cancel))
    ];

    public Task<string> CreateDesignedAsync(string name, string description, string generatedVoiceId, CancellationToken cancel) => Task.FromResult("designed");

    public Task<string> CloneAsync(VoiceCloneRequest request, CancellationToken cancel) => Task.FromResult("cloned");

    public Task DeleteVoiceAsync(string voiceId, CancellationToken cancel) => Task.CompletedTask;

    public Task<PcmSound> DownloadPreviewAsync(string url, CancellationToken cancel) => SynthesizeAsync(new SpeechRequest("Lipová", "", ""), cancel);
}
