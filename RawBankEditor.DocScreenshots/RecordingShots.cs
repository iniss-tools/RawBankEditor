using System.Collections.Concurrent;
using System.Net;
using RawBankEditor.Forms;
using RawBankEditor.Speech;
using RawBankEditor.Tools;
using ToolsCore.Iniss.Entities;

namespace RawBankEditor.DocScreenshots;

/// <summary>
/// Snímky ponuky Nahrávky - úprava zvuku a okná hlasu AI. Služby hlasu nahrádza <see cref="DemoSpeech" />, takže
/// harness nepotrebuje kľúč ani pripojenie.
/// </summary>
internal sealed partial class Shots
{
    // neexistujúci priečinok - okno hlasov Piper z neho len číta
    private const string PiperFolder = @"C:\Programy\piper\hlasy";

    private void RecordingShots(FMain main, FyzLanguage language)
    {
        var stations = language.Groups.First(g => g.Key == "R1");
        var work = Directory.CreateTempSubdirectory("rbe-docshots").FullName;
        using var player = new AudioPlayer();
        var speech = new DemoSpeech();
        using var providers = new SpeechProviders([speech]);
        var cache = new SpeechCache(Path.Combine(work, "cache"));
        var voice = new VoiceProfile
        {
            Provider = ElevenLabsClient.ProviderId, ModelId = ElevenLabsClient.DefaultModel, VoiceId = DemoSpeech.BankVoiceId,
            VoiceName = DemoSpeech.BankVoiceName, LanguageCode = "sk", Speed = 0.95
        };
        var sample = stations.Sounds.First(s => s.Key == "9900100").Text;

        try
        {
            // stĺpec Pôvod - niekoľko staníc vygenerovaných hlasom jazyka
            var records = Program.Bank.Records(language);
            foreach (var sound in stations.Sounds.Where(s => s.Key is "9900100" or "9900110" or "9900120"))
                records.SetOrigin(sound, SoundOrigin.Generated(voice, sound.Text, null, "", new DateTimeOffset(2026, 10, 27, 14, 5, 0, TimeSpan.FromHours(1))));
            Shot("generovanie-nahravok/povod", main, _ =>
            {
                SelectGroup(main, stations);
                SelectSound(main, stations.Sounds.First(s => s.Key == "9900110"));
            }, dispose: false);

            var files = stations.Sounds.Select(s => s.File).OfType<SoundFileElement>().ToList();
            Shot("uprava-zvuku/uprava-zvuku", () => new FAdjustSounds(new SoundAdjustOptions(), files.Take(3).ToList(), stations.Name, files,
                language.Name, language.Groups.SelectMany(g => g.Sounds).Select(s => s.File).OfType<SoundFileElement>().ToList(), player));

            // nastavenia služieb - vyplnené ako po zadaní kľúčov
            var speechConfig = Program.Session.Config.Speech;
            speechConfig.ElevenLabsKey = "sk_0123456789abcdef0123456789abcdef0123456789abcdef";
            speechConfig.AzureKey = "0123456789abcdef0123456789abcdef";
            speechConfig.AzureRegion = "westeurope";
            speechConfig.PiperPath = @"C:\Programy\piper\piper.exe";
            // priečinok hlasov mimo profilu používateľa - na snímke nemá byť meno počítača ani používateľa
            speechConfig.PiperVoicesFolder = PiperFolder;
            foreach (var (slug, page) in new[] { ("elevenlabs", FAppSettings.SpeechPage), ("azure", "pAzure"), ("piper", "pPiper") })
            {
                Shot($"sluzby-hlasu/nastavenia-{slug}", () =>
                {
                    var form = new FAppSettings(Program.Session);
                    form.PreselectMenuItem(page);
                    return form;
                });
            }

            Shot("hlas-jazyka/hlas-jazyka", () => new FVoiceProfile(providers, cache, voice, language.Name, sample, player, null, work, "sk"), form =>
            {
                Pump.Until(() => Field<ComboBox>(form, "cbVoice").Items.Count > 0);
                Unselect(form);
            });

            Shot("hlas-jazyka/hlasy-piper", () =>
            {
                var form = new FPiperVoices(PiperFolder, "sk");
                form.GetType().GetField("_catalog", Any)!.SetValue(form, new PiperVoiceCatalog(new DemoPiperCatalog()));
                return form;
            }, form => Pump.Until(() => Field<DataGridView>(form, "dgvVoices").Rows.Count > 0));

            GenerationShots(language, stations, providers, cache, voice, player, work);

            var groups = language.Groups.Where(g => g.Key != DemoBank.EmptyGroupKey).ToList();
            Shot("intonacia-a-vyslovnost/intonacia", () => new FProsody(groups, new Dictionary<string, GroupProsody>(),
                g => ProsodyDefaults.For(g.Type, "sk"), (prosody, _, cancel) => Generator(speech, cache, voice).GenerateAsync(new SpeechInput(sample, prosody), null, cancel),
                player));
            Shot("intonacia-a-vyslovnost/vyslovnost", () => new FPronunciation(
                [
                    new PronunciationRule { Find = "Os", Replace = "osobný vlak", WholeWord = true, MatchCase = true },
                    new PronunciationRule { Find = "IC", Replace = "intercity", WholeWord = true, MatchCase = true },
                    new PronunciationRule { Find = "ŽSR", Replace = "železnice slovenskej republiky", WholeWord = true }
                ], true, "sk", "Os 3501 z Dolného Mesta príde 11. apríla na koľaj 2.",
                (text, rules, numbers, cancel) => Generator(speech, cache, voice, rules, numbers).GenerateAsync(text, null, cancel), player), Unselect);

            Shot("zmena-hlasu-a-vycistenie/rozsah", () => new FScope(RawBankEditor.Properties.Resources.FGenerateSounds_TitleChangeVoice, 3, stations.Name,
                stations.Sounds.Count, language.Name, language.Groups.Sum(g => g.Sounds.Count(s => s.Key != DemoBank.MissingFileKey))));

            // prepis - zvuky bez textu dostanú prepis, pri ostatných sa porovná
            var transcribed = stations.Sounds.Where(s => s.Key is "9900010" or "9900020" or "9900030" or "9900100" or "9900110" or "9900120").ToList();
            var emptied = transcribed[4];
            var emptiedText = emptied.Text;
            emptied.Text = "";
            Shot("prepis-na-text/prepis", () => new FTranscribe(new SpeechTranscriber(speech, cache), transcribed,
                string.Format(System.Globalization.CultureInfo.CurrentCulture, RawBankEditor.Properties.Resources.Generate_ScopeGroup, stations.Name), "sk",
                new SpeechSettings(), player), form =>
            {
                Field<CheckBox>(form, "cboxOnlyEmpty").Checked = false;
                Invoke(form, "BTranscribe_Click");
                Pump.Until(() => Field<Button>(form, "bTranscribe").Enabled);
            });
            emptied.Text = emptiedText;

            VoiceShots(speech, player, files);
        }
        finally
        {
            foreach (var sound in stations.Sounds)
                Program.Bank.Records(language).SetOrigin(sound, null);
            Directory.Delete(work, true);
        }
    }

    private void GenerationShots(FyzLanguage language, FyzGroup stations, SpeechProviders providers, SpeechCache cache, VoiceProfile voice, AudioPlayer player,
        string work)
    {
        using var versions = new SoundVersions(Path.Combine(work, "versions"));
        var rows = language.Groups.SelectMany(g => g.Sounds).Select(s => new GenerationRow(s, s.File, null, null)).ToList();
        var settings = new SpeechSettings();
        Shot("generovanie-nahravok/generovanie", () => new FGenerateSounds(providers, cache, voice, () => null,
            string.Format(System.Globalization.CultureInfo.CurrentCulture, RawBankEditor.Properties.Resources.Generate_ScopeLanguage, language.Name), rows, true,
            settings, new SoundAdjustOptions(), player, versions, g => ProsodyDefaults.For(g.Type, "sk"), [], SpeechOperation.Tts, "sk", "sk"), form =>
        {
            form.Size = new Size(form.Width + 60, 640);
            Invoke(form, "BGenerate_Click");
            Pump.Until(() => rows.All(r => !r.Busy && r.Status != RawBankEditor.Properties.Resources.FGenerateSounds_StatusQueued) && Field<Button>(form, "bOK").Enabled);
            var grid = Field<DataGridView>(form, "dgvItems");
            grid.ClearSelection();
            var slow = grid.Rows.Cast<DataGridViewRow>().First(r => ((GenerationRow)r.DataBoundItem!).Key == DemoSpeech.SlowKey);
            grid.CurrentCell = slow.Cells[Field<DataGridViewColumn>(form, "cKey").Index];
            slow.Selected = true;
        });

        using var stsVersions = new SoundVersions(Path.Combine(work, "versions-sts"));
        var stsRows = stations.Sounds.Take(5).Select(s => new GenerationRow(s, s.File, null, null)).ToList();
        Shot("zmena-hlasu-a-vycistenie/zmena-hlasu", () => new FGenerateSounds(providers, cache, voice, () => null,
            string.Format(System.Globalization.CultureInfo.CurrentCulture, RawBankEditor.Properties.Resources.Generate_ScopeGroup, stations.Name), stsRows, false,
            settings, new SoundAdjustOptions(), player, stsVersions, _ => null, [], SpeechOperation.ChangeVoice, "sk"), form =>
        {
            form.Size = new Size(form.Width, 440);
            Invoke(form, "BGenerate_Click");
            Pump.Until(() => stsRows.All(r => !r.Busy && r.ResultWav is not null));
        });
    }

    private void VoiceShots(DemoSpeech speech, AudioPlayer player, List<SoundFileElement> files)
    {
        Shot("hlasy-elevenlabs/hlasy", () => new FVoices(speech, speech, player, "sk", DemoSpeech.BankVoiceId, () => false),
            form => Pump.Until(() => Field<DataGridView>(form, "dgvVoices").Rows.Count > 0));
        Shot("hlasy-elevenlabs/kniznica", () => new FVoiceLibrary(speech, player, "sk"),
            form => Pump.Until(() => Field<DataGridView>(form, "dgvVoices").Rows.Count > 0));
        Shot("hlasy-elevenlabs/navrh", () => new FVoiceDesign(speech, player), form =>
        {
            Field<TextBox>(form, "tbDescription").Text = "Pokojný ženský hlas strednej generácie, spisovná slovenčina, zreteľná výslovnosť, pomalšie tempo.";
            Invoke(form, "BDesign_Click");
            Pump.Until(() => Field<Button>(form, "bDesign").Enabled);
            Field<TextBox>(form, "tbName").Text = "Hlásateľka SK";
            Unselect(form);
        });
        Shot("hlasy-elevenlabs/klonovanie", () => new FVoiceClone(speech, files.Select(f => f.FileInfo.FullName).ToList(),
            files.Sum(f => Math.Max(f.Duration, 0)), "sk", "INISS SK"), Unselect);
    }

    private static SpeechGenerator Generator(DemoSpeech speech, SpeechCache cache, VoiceProfile voice, IReadOnlyList<PronunciationRule>? rules = null,
        bool numbers = true) =>
        new(speech, cache, voice, null, rules, numbers ? "sk" : "");
}

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

/// <summary>
/// Výška tónu podľa textu - nahrávky ukážkovej banky aj „reč“ <see cref="DemoSpeech" /> sú tóny, z ktorých výšky
/// prepis zistí text.
/// </summary>
internal static class DemoTones
{
    private const double BASE = 160;
    private const double STEP = 20;
    private static readonly List<string> Texts = [];

    public static double Frequency(string text)
    {
        lock (Texts)
        {
            var id = Texts.IndexOf(text);
            if (id < 0)
            {
                id = Texts.Count;
                Texts.Add(text);
            }

            return BASE + STEP * id;
        }
    }

    public static string? TextOf(double frequency)
    {
        var id = (int)Math.Round((frequency - BASE) / STEP);
        lock (Texts)
            return id >= 0 && id < Texts.Count ? Texts[id] : null;
    }
}

/// <summary>
/// Zoznam hlasov Piper (výber z voices.json) bez pripojenia.
/// </summary>
internal sealed class DemoPiperCatalog : HttpMessageHandler
{
    private const string Json = """
        {
          "sk_SK-lili-medium": {"quality": "medium", "num_speakers": 1, "language": {"family": "sk", "name_native": "Slovenčina"},
            "files": {"sk/sk_SK/lili/medium/sk_SK-lili-medium.onnx": {"size_bytes": 63201294, "md5_digest": ""},
                      "sk/sk_SK/lili/medium/sk_SK-lili-medium.onnx.json": {"size_bytes": 4900, "md5_digest": ""}}},
          "cs_CZ-jirka-low": {"quality": "low", "num_speakers": 1, "language": {"family": "cs", "name_native": "Čeština"},
            "files": {"cs/cs_CZ/jirka/low/cs_CZ-jirka-low.onnx": {"size_bytes": 28130791, "md5_digest": ""},
                      "cs/cs_CZ/jirka/low/cs_CZ-jirka-low.onnx.json": {"size_bytes": 5025, "md5_digest": ""}}},
          "cs_CZ-jirka-medium": {"quality": "medium", "num_speakers": 1, "language": {"family": "cs", "name_native": "Čeština"},
            "files": {"cs/cs_CZ/jirka/medium/cs_CZ-jirka-medium.onnx": {"size_bytes": 63201294, "md5_digest": ""},
                      "cs/cs_CZ/jirka/medium/cs_CZ-jirka-medium.onnx.json": {"size_bytes": 5025, "md5_digest": ""}}},
          "en_GB-alba-medium": {"quality": "medium", "num_speakers": 1, "language": {"family": "en", "name_native": "English"},
            "files": {"en/en_GB/alba/medium/en_GB-alba-medium.onnx": {"size_bytes": 63201294, "md5_digest": ""},
                      "en/en_GB/alba/medium/en_GB-alba-medium.onnx.json": {"size_bytes": 4880, "md5_digest": ""}}}
        }
        """;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
        Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(Json) });
}
