using RawBankEditor.Forms;
using RawBankEditor.Speech;
using RawBankEditor.Speech.Piper;
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
            foreach (var (slug, page) in new[] { ("elevenlabs", "pElevenLabs"), ("azure", "pAzure"), ("piper", "pPiper") })
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
