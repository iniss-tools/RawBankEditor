using System.Globalization;
using RawBankEditor.Entities;
using RawBankEditor.Properties;
using RawBankEditor.Speech;
using RawBankEditor.Tools;
using ToolsCore.Iniss.Entities;
using ToolsCore.Iniss.Tools;
using ToolsCore.Tools;
using ToolsCore.XML;

namespace RawBankEditor.Forms;

partial class FMain
{
    // prehravanie nahravok, kopie verzii nahradenych nahravok pre Spat a vysledky hlasovej syntezy
    private readonly AudioPlayer _player = new();
    private readonly SoundVersions _versions = SoundVersions.ForProcess();
    private readonly SpeechCache _speechCache = SpeechCache.Default();

    /// <summary>
    /// Nahradenie nahravok novymi verziami (uprava zvuku, hlas AI) - Spat zapise povodne verzie, Znova nove.
    /// </summary>
    public class ReplaceRecordingsAction(FMain form, IReadOnlyList<RecordingChange> changes, string commandName) : Action(form)
    {
        /// <inheritdoc />
        public override string CommandName => commandName;

        /// <inheritdoc />
        public override void Undo() => _ = Form.ApplyRecordingsAsync(changes, false, Resources.Recordings_UndoStatus);

        /// <inheritdoc />
        public override void Redo() => _ = Form.ApplyRecordingsAsync(changes, true, Resources.Recordings_RedoStatus);
    }

    /// <inheritdoc />
    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _player.Dispose();
        _versions.Dispose();
        base.OnFormClosed(e);
    }

    private IEnumerable<FyzSound> SelectedSounds =>
        dgvSounds.SelectedRows.Cast<DataGridViewRow>().Select(r => r.DataBoundItem).OfType<FyzSound>();

    private void SaveConfig() =>
        XmlSerialization.WriteData(PathUtils.CombinePath(ToolsCore.AppPaths.ConfigDir, ToolsCore.FileConsts.FileConfig)!, _bank.Config);

    /// <summary>
    /// Prehra subor nahravky; predchadzajuca nahravka sa zastavi.
    /// </summary>
    private void PlaySoundFile(string path)
    {
        if (!File.Exists(path))
            return;

        try
        {
            _player.PlayFile(path);
        }
        catch (Exception e) when (e is IOException or InvalidDataException or UnauthorizedAccessException)
        {
            _dialogs.ShowError(e.Message);
        }
    }

    /// <summary>
    /// Zapise nove alebo povodne verzie nahravok na pozadi. Sledovanie suborov je vypnute - prvky prieskumnika aj dlzky
    /// upravi nahradenie samo; povodne subory idu cez zurnal banky.
    /// </summary>
    internal async Task ApplyRecordingsAsync(IReadOnlyList<RecordingChange> changes, bool toNew, string status)
    {
        ChangeStatus(status);
        tspbProgress.Visible = true;
        tspbProgress.Style = ProgressBarStyle.Blocks;
        tspbProgress.Minimum = 0;
        tspbProgress.Maximum = Math.Max(changes.Count, 1);
        tspbProgress.Value = 0;

        // nahravka sa prave moze prehravat zo suboru, ktory sa nahradi
        _player.Stop();
        List<string> errors = [];
        var watching = fileSystemWatcher.EnableRaisingEvents;
        RawBankExplorer.ConvertSoundIsHandled = true;
        fileSystemWatcher.EnableRaisingEvents = false;
        try
        {
            var journal = _bank.Journal;
            var scope = CurrentLanguage;
            errors = await Task.Run(() => RecordingReplace.Apply(changes, toNew, journal, scope, () => BeginInvoke(() => tspbProgress.Increment(1))));
        }
        finally
        {
            fileSystemWatcher.EnableRaisingEvents = watching;
            BeginInvoke(() => RawBankExplorer.ConvertSoundIsHandled = false);
        }

        // nove subory k zvukom a prieskumniku, zaznamy povodu
        RecordingReplace.Link(changes, toNew, CurrentLanguage is { } language ? _bank.Records(language) : null);
        ResetStatusAfterTask();
        RefreshSoundViews();
        if (errors.Count > 0)
            _dialogs.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.Recordings_ReplaceFailed, ListOf(errors)));
    }

    /// <summary>
    /// Uprava zvuku nahravok (ticho, hlasitost, format) - vybrane zvuky, skupina alebo cely jazyk.
    /// </summary>
    private async void DoAdjustSounds()
    {
        if (CurrentLanguage?.Groups is not { } groups)
            return;

        var selected = FilesOf(SelectedSounds).Distinct().ToList();
        var group = CurrentGroup is null ? [] : FilesOf(CurrentGroup.Sounds).Distinct().ToList();
        var language = FilesOf(groups.SelectMany(g => g.Sounds)).Distinct().ToList();

        using var form = new FAdjustSounds(_bank.Config.SoundAdjust, selected, CurrentGroup?.Name ?? "", group, CurrentLanguage.Name, language, _player);
        if (form.ShowDialog(this) != DialogResult.OK)
            return;

        var options = form.Options;
        var files = form.Files;
        _bank.Config.SoundAdjust = options;
        SaveConfig();

        ChangeStatus(Resources.Recordings_AdjustStatus);
        tspbProgress.Visible = true;
        tspbProgress.Style = ProgressBarStyle.Blocks;
        tspbProgress.Minimum = 0;
        tspbProgress.Maximum = Math.Max(files.Count, 1);
        tspbProgress.Value = 0;

        var recalculate = _bank.Config.AutoRecalculateSoundDuration;
        var versions = _versions;
        var (changes, failed) = await Task.Run(() =>
        {
            var changes = new List<RecordingChange>();
            var failed = new List<string>();
            foreach (var file in files)
            {
                try
                {
                    var sound = SoundProcessing.Process(SoundProcessing.ReadFile(file.FileInfo.FullName), options);
                    changes.Add(RecordingChange.Create(file, sound, versions, recalculate));
                }
                catch (Exception e) when (e is IOException or InvalidDataException or UnauthorizedAccessException)
                {
                    failed.Add($"{file.Name}: {e.Message}");
                }

                BeginInvoke(() => tspbProgress.Increment(1));
            }

            return (changes, failed);
        });

        if (changes.Count > 0)
        {
            await ApplyRecordingsAsync(changes, true, Resources.Recordings_AdjustStatus);
            RegisterNewAction(new ReplaceRecordingsAction(this, changes, Resources.Action_AdjustSounds));
        }
        else
        {
            ResetStatusAfterTask();
        }

        if (failed.Count > 0)
            _dialogs.ShowWarning(string.Format(CultureInfo.CurrentCulture, Resources.Recordings_AdjustFailed, ListOf(failed)));
    }

    /// <summary>
    /// Kluc ElevenLabs - bez neho ponukne nastavenia programu.
    /// </summary>
    /// <returns>kluc alebo <see langword="null" />, ak ho pouzivatel nezadal.</returns>
    private string? SpeechKey()
    {
        if (_bank.Config.Speech.ElevenLabsKey is { Length: > 0 } key)
            return key;

        if (_dialogs.ShowQuestion(Resources.Speech_NoKey) != DialogResult.Yes)
            return null;

        ShowAppSettings(FAppSettings.SpeechPage);
        return _bank.Config.Speech.ElevenLabsKey is { Length: > 0 } entered ? entered : null;
    }

    /// <summary>
    /// Hlas jazyka ulozeny v <c>.rbe-ai</c>; <see langword="null" />, ak este nie je vybrany.
    /// </summary>
    private VoiceProfile? LoadVoice(FyzLanguage language) =>
        BankAiFolder.Load(_bank.Project!.AbsPathToINISS, language.Key).Voice is { IsComplete: true } voice ? voice : null;

    /// <summary>
    /// Otvori okno hlasu jazyka a po potvrdeni ulozi hlas do <c>.rbe-ai</c>.
    /// </summary>
    /// <returns>novy hlas alebo <see langword="null" />, ak ho pouzivatel nezmenil alebo sa nedal ulozit.</returns>
    private VoiceProfile? EditVoice(ISpeechProvider provider, FyzLanguage language, string sampleText)
    {
        var pathToIniss = _bank.Project!.AbsPathToINISS;
        var data = BankAiFolder.Load(pathToIniss, language.Key);
        var profile = data.Voice ?? new VoiceProfile { LanguageCode = VoiceProfile.LanguageCodeOf(language.Key) };

        using var form = new FVoiceProfile(provider, _speechCache, profile, language.Name, sampleText, _player,
            _bank.Config.Speech.AdjustSound ? _bank.Config.SoundAdjust : null);
        if (form.ShowDialog(this) != DialogResult.OK)
            return null;

        data.Voice = form.Profile;
        try
        {
            BankAiFolder.Save(pathToIniss, language.Key, data);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            Log.Exception(e);
            _dialogs.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.Speech_VoiceSaveFailed, e.Message));
            return null;
        }

        return form.Profile;
    }

    // ukazkovy text hlasu - text vybraneho zvuku, inak prveho zvuku otvorenej skupiny
    private string SampleText() =>
        SelectedSounds.Concat(CurrentGroup?.Sounds ?? []).Select(s => s.Text).FirstOrDefault(t => !string.IsNullOrWhiteSpace(t)) ?? "";

    private void DoVoiceProfile()
    {
        if (CurrentLanguage is not { } language || SpeechKey() is not { } key)
            return;

        using var provider = new ElevenLabsClient(key);
        EditVoice(provider, language, SampleText());
    }

    /// <summary>
    /// Generovanie nahravok vybranych zvukov hlasom jazyka.
    /// </summary>
    private void DoGenerateSounds()
    {
        var sounds = SelectedSounds.ToList();
        GenerateSounds(sounds, string.Format(CultureInfo.CurrentCulture, Resources.Generate_ScopeSelected, sounds.Count), false);
    }

    /// <summary>
    /// Generovanie nahravok vybranej skupiny - zvuky s nahravkou od tohto hlasu sa predvolene vynechaju.
    /// </summary>
    private void DoGenerateGroup()
    {
        if (dgvGroups.IsSelectionEmpty() || dgvGroups.SelectedRows[0].DataBoundItem is not FyzGroup group)
            return;

        GenerateSounds(group.Sounds.ToList(), string.Format(CultureInfo.CurrentCulture, Resources.Generate_ScopeGroup, group.Name), true);
    }

    /// <summary>
    /// Generovanie nahravok celeho jazyka - zvuky s nahravkou od tohto hlasu sa predvolene vynechaju.
    /// </summary>
    private void DoGenerateLanguage()
    {
        if (CurrentLanguage?.Groups is not { } groups)
            return;

        GenerateSounds(groups.SelectMany(g => g.Sounds).ToList(), string.Format(CultureInfo.CurrentCulture, Resources.Generate_ScopeLanguage, CurrentLanguage.Name),
            true);
    }

    /// <summary>
    /// Doplnenie nahravok zvukom jazyka, ktore ziadnu nemaju.
    /// </summary>
    private void DoGenerateMissing()
    {
        if (CurrentLanguage?.Groups is not { } groups)
            return;

        GenerateMissing(groups.SelectMany(g => g.Sounds).Where(s => s.File is null).ToList());
    }

    /// <summary>
    /// Doplnenie nahravok zvukom vybranym v zozname chyb (chybajuca nahravka); bez vyberu vsetkym.
    /// </summary>
    private void DoGenerateMissingFromErrors()
    {
        var selected = dgvErrors.SelectedRows.Cast<DataGridViewRow>().Select(r => r.DataBoundItem).OfType<SoundFileMissing>()
            .Select(m => m.Sound).Where(s => s.File is null).ToList();
        if (selected.Count == 0)
            DoGenerateMissing();
        else
            GenerateMissing(selected);
    }

    private void GenerateMissing(List<FyzSound> sounds)
    {
        if (sounds.Count == 0)
        {
            _dialogs.ShowInfo(Resources.Generate_NoMissing);
            return;
        }

        GenerateSounds(sounds, string.Format(CultureInfo.CurrentCulture, Resources.Generate_ScopeMissing, sounds.Count), false);
    }

    /// <summary>
    /// Okno generovania pre zvuky - po kontrole sa prevzate nahravky zapisu ako jeden krok Spat (zvukom bez nahravky
    /// subor vznikne) a zaznamenaju sa v <c>.rbe-ai</c>.
    /// </summary>
    /// <param name="sounds">zvuky jazyka</param>
    /// <param name="title">rozsah v nazve okna</param>
    /// <param name="skipDone">predvolene vynechat zvuky, ktorych nahravka uz vznikla tymto hlasom</param>
    /// <param name="operation">nova nahravka z textu, zmena hlasu alebo vycistenie povodnej nahravky</param>
    private async void GenerateSounds(List<FyzSound> sounds, string title, bool skipDone, SpeechOperation operation = SpeechOperation.Tts)
    {
        if (operation != SpeechOperation.Tts)
            sounds = sounds.Where(s => s.File is not null).ToList();
        if (CurrentLanguage is not { } language || sounds.Count == 0)
            return;

        if (SpeechKey() is not { } key)
            return;

        var records = _bank.Records(language);
        var rows = sounds.Select(s => new GenerationRow(s, s.File, records.GetOrigin(s), records.GetText(s))).ToList();
        using var provider = new ElevenLabsClient(key);
        var sample = rows.Select(r => r.Text).FirstOrDefault(t => !string.IsNullOrWhiteSpace(t)) ?? "";
        if ((LoadVoice(language) ?? EditVoice(provider, language, sample)) is not { } voice)
            return;

        var data = BankAiFolder.Load(_bank.Project!.AbsPathToINISS, language.Key);
        var languageCode = voice.LanguageCode.Length > 0 ? voice.LanguageCode : VoiceProfile.LanguageCodeOf(language.Key);

        IReadOnlyList<GenerationRow> accepted;
        VoiceProfile profile;
        using (var form = new FGenerateSounds(provider, _speechCache, voice, () => EditVoice(provider, language, sample), title, rows, skipDone,
                   _bank.Config.Speech, _bank.Config.SoundAdjust, _player, _versions, g => ProsodyOf(data, g, languageCode), data.Pronunciation,
                   operation, languageCode))
        {
            var result = form.ShowDialog(this);
            // volba upravy zvuku sa pamata
            SaveConfig();
            if (result != DialogResult.OK || form.Accepted.Count == 0)
                return;
            accepted = form.Accepted;
            profile = form.Profile;
        }

        ChangeStatus(Resources.Recordings_GenerateStatus);
        UseWaitCursor = true;
        var pathToBank = PathToBank;
        var recalculate = _bank.Config.AutoRecalculateSoundDuration;
        var versions = _versions;
        var created = DateTimeOffset.Now;
        var (changes, failed) = await Task.Run(() => PrepareGenerated(accepted, profile, pathToBank, versions, recalculate, created, operation));
        UseWaitCursor = false;

        if (changes.Count > 0)
        {
            await ApplyRecordingsAsync(changes, true, Resources.Recordings_GenerateStatus);
            RegisterNewAction(new ReplaceRecordingsAction(this, changes, operation switch
            {
                SpeechOperation.ChangeVoice => Resources.Action_ChangeVoice,
                SpeechOperation.Isolate => Resources.Action_IsolateSounds,
                _ => Resources.Action_GenerateSounds
            }));
        }
        else
        {
            ResetStatusAfterTask();
        }

        if (failed.Count > 0)
            _dialogs.ShowWarning(string.Format(CultureInfo.CurrentCulture, Resources.Recordings_GenerateFailed, ListOf(failed)));
    }

    /// <summary>
    /// Zmeny nahravok pre prevzate riadky: existujuca nahravka sa nahradi, chybajuca vznikne. Kazda zmena nesie povod
    /// novej nahravky a povod tej, ktoru nahradza.
    /// </summary>
    private static (List<RecordingChange> Changes, List<string> Failed) PrepareGenerated(IEnumerable<GenerationRow> rows, VoiceProfile profile,
        string pathToBank, SoundVersions versions, bool recalculate, DateTimeOffset created, SpeechOperation operation)
    {
        var changes = new List<RecordingChange>();
        var failed = new List<string>();
        foreach (var row in rows)
        {
            try
            {
                var change = row.File is { } file
                    ? RecordingChange.Replace(file, row.ResultWav!, row.ResultDuration, versions, recalculate)
                    : RecordingChange.CreateMissing(row.Sound, pathToBank, row.ResultWav!, row.ResultDuration, recalculate);
                changes.Add(operation switch
                {
                    SpeechOperation.Tts => change with
                    {
                        OldOrigin = row.Origin,
                        NewOrigin = SoundOrigin.Generated(profile, row.Text, row.Seed, row.Fingerprint, created),
                        OldText = row.SavedText,
                        NewText = row.TextToSave
                    },
                    SpeechOperation.ChangeVoice => change with
                    {
                        OldOrigin = row.Origin,
                        NewOrigin = SoundOrigin.Converted(profile, row.Seed, row.Fingerprint, created)
                    },
                    // vycistenie povod ani text nemeni
                    _ => change
                });
            }
            catch (Exception e) when (e is IOException or InvalidDataException or UnauthorizedAccessException)
            {
                failed.Add($"{row.Group}/{row.Key}: {e.Message}");
            }
        }

        return (changes, failed);
    }

    /// <summary>
    /// Stlpec Povod - vyplna sa zo zaznamov povodu jazyka, nie z udajov zvuku. Doplnenie chybajucich nahravok v zozname
    /// chyb zavisi od jeho obsahu - stav sa obnovi pri otvoreni ponuky.
    /// </summary>
    private void InitRecordings()
    {
        contextMenuErrors.Opening += (_, _) => UpdateCommandStates();
        dgvSounds.CellFormatting += (_, e) =>
        {
            if (e.ColumnIndex == cSoundOrigin.Index && e.RowIndex >= 0)
                e.Value = OriginOf(e.RowIndex) is { } origin ? string.Format(CultureInfo.CurrentCulture, Resources.Origin_Generated, origin.VoiceName) : "";
        };
        dgvSounds.CellFormatting += (_, e) =>
        {
            if (e.ColumnIndex == cSoundSynthText.Index && SoundAt(e.RowIndex) is { } sound)
                e.Value = _bank.Records(CurrentLanguage!).GetText(sound) ?? "";
        };
        dgvSounds.CellToolTipTextNeeded += (_, e) =>
        {
            if (e.ColumnIndex == cSoundOrigin.Index && e.RowIndex >= 0 && OriginOf(e.RowIndex) is { } origin)
                e.ToolTipText = origin.Kind == SoundOrigin.KindSts
                    ? string.Format(CultureInfo.CurrentCulture, Resources.Origin_ConvertedTooltip, origin.VoiceName, origin.Created.LocalDateTime)
                    : string.Format(CultureInfo.CurrentCulture, Resources.Origin_Tooltip, origin.VoiceName, origin.ModelId,
                        origin.Created.LocalDateTime, origin.Text);
        };
    }

    private FyzSound? SoundAt(int row) =>
        _bank.Project is not null && CurrentLanguage is not null && _languageLoaded && row >= 0 && row < dgvSounds.Rows.Count
            ? dgvSounds.Rows[row].DataBoundItem as FyzSound
            : null;

    private SoundOrigin? OriginOf(int row) => SoundAt(row) is { } sound ? _bank.Records(CurrentLanguage!).GetOrigin(sound) : null;

    /// <summary>
    /// Zmena textu pre syntezu v stlpci zoznamu zvukov - samostatny krok Spat.
    /// </summary>
    private void EndSynthTextEdit(int row)
    {
        if (SoundAt(row) is not { } sound)
            return;

        var records = _bank.Records(CurrentLanguage!);
        var cell = dgvSounds.Rows[row].Cells[cSoundSynthText.Index];
        var old = records.GetText(sound);
        records.SetText(sound, cell.Value as string);
        // bunka bez hodnoty - zobrazuje sa zo zaznamov (CellFormatting)
        cell.Value = null;
        var changed = records.GetText(sound);
        if (old != changed)
            RegisterNewAction(new EditSynthTextAction(this, sound, old, changed));
    }

    /// <summary>
    /// Zmena textu pre syntezu zvuku.
    /// </summary>
    public class EditSynthTextAction(FMain form, FyzSound sound, string? oldText, string? newText) : Action(form)
    {
        /// <inheritdoc />
        public override string CommandName => Resources.Action_SynthText;

        /// <inheritdoc />
        public override void Undo() => Set(oldText);

        /// <inheritdoc />
        public override void Redo() => Set(newText);

        private void Set(string? text)
        {
            if (Form.CurrentLanguage is not { } language)
                return;

            Form.SelectGroup(sound.Group);
            Form._bank.Records(language).SetText(sound, text);
            Form.SelectSound(sound);
            Form.dgvSounds.Invalidate();
        }
    }

    /// <summary>
    /// Intonacia skupiny - ulozena, inak predvolena podla typu skupiny a jazyka reci.
    /// </summary>
    private static GroupProsody ProsodyOf(LanguageAiData data, FyzGroup group, string languageCode) =>
        data.Prosody.TryGetValue(group.Key, out var prosody) ? prosody : ProsodyDefaults.For(group.Type, languageCode);

    /// <summary>
    /// Ukazky v oknach intonacie a vyslovnosti - hlas jazyka a kluc, ak su nastavene. Sluzbu uvolni volajuci.
    /// </summary>
    private (ElevenLabsClient Provider, VoiceProfile Voice)? SampleVoice(FyzLanguage language)
    {
        if (_bank.Config.Speech.ElevenLabsKey is not { Length: > 0 } key || LoadVoice(language) is not { } voice)
            return null;
        return (new ElevenLabsClient(key), voice);
    }

    private SoundAdjustOptions? SampleAdjust => _bank.Config.Speech.AdjustSound ? _bank.Config.SoundAdjust : null;

    /// <summary>
    /// Okno intonacie skupin jazyka - nastavenia sa ulozia hned do <c>.rbe-ai</c>.
    /// </summary>
    private void DoProsody()
    {
        if (CurrentLanguage is not { Groups: { } groups } language)
            return;

        var pathToIniss = _bank.Project!.AbsPathToINISS;
        var data = BankAiFolder.Load(pathToIniss, language.Key);
        var sample = SampleVoice(language);
        var languageCode = sample?.Voice.LanguageCode is { Length: > 0 } code ? code : VoiceProfile.LanguageCodeOf(language.Key);
        var records = _bank.Records(language);
        Func<GroupProsody, FyzGroup, CancellationToken, Task<GeneratedSound>>? tryGroup = sample is { } s
            ? (prosody, group, cancel) =>
            {
                var text = group.Sounds.Select(records.SpokenText).FirstOrDefault(t => !string.IsNullOrWhiteSpace(t)) ?? group.Name;
                return new SpeechGenerator(s.Provider, _speechCache, s.Voice, SampleAdjust, data.Pronunciation)
                    .GenerateAsync(new SpeechInput(text, prosody), null, cancel);
            }
            : null;

        try
        {
            using var form = new FProsody(groups, data.Prosody, g => ProsodyDefaults.For(g.Type, languageCode), tryGroup, _player);
            if (form.ShowDialog(this) != DialogResult.OK)
                return;

            BankAiFolder.Update(pathToIniss, language.Key, d => d.Prosody = form.Overrides);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            Log.Exception(e);
            _dialogs.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.Speech_SettingsSaveFailed, e.Message));
        }
        finally
        {
            sample?.Provider.Dispose();
        }
    }

    /// <summary>
    /// Okno slovnika vyslovnosti jazyka - pravidla sa ulozia hned do <c>.rbe-ai</c>.
    /// </summary>
    private void DoPronunciation()
    {
        if (CurrentLanguage is not { } language)
            return;

        var pathToIniss = _bank.Project!.AbsPathToINISS;
        var data = BankAiFolder.Load(pathToIniss, language.Key);
        var sample = SampleVoice(language);
        Func<string, IReadOnlyList<PronunciationRule>, CancellationToken, Task<GeneratedSound>>? trySpeech = sample is { } s
            ? (text, rules, cancel) => new SpeechGenerator(s.Provider, _speechCache, s.Voice, SampleAdjust, rules).GenerateAsync(text, null, cancel)
            : null;

        try
        {
            using var form = new FPronunciation(data.Pronunciation, SampleText(), trySpeech, _player);
            if (form.ShowDialog(this) != DialogResult.OK)
                return;

            BankAiFolder.Update(pathToIniss, language.Key, d => d.Pronunciation = form.Rules);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            Log.Exception(e);
            _dialogs.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.Speech_SettingsSaveFailed, e.Message));
        }
        finally
        {
            sample?.Provider.Dispose();
        }
    }

    /// <summary>
    /// Prehra nahravky vybranych zvukov za sebou v poradi zoznamu - skuska nadvaznosti uryvkov ako v hlaseni.
    /// </summary>
    private async void DoPlaySequence()
    {
        var files = dgvSounds.SelectedRows.Cast<DataGridViewRow>().OrderBy(r => r.Index).Select(r => r.DataBoundItem).OfType<FyzSound>()
            .Select(s => s.File?.FileInfo.FullName).OfType<string>().ToList();
        if (files.Count == 0)
            return;

        try
        {
            var wav = await Task.Run(() => SoundProcessing.ToWav(SoundProcessing.Concat(files.Select(SoundProcessing.ReadFile))));
            _player.Play(wav);
        }
        catch (Exception e) when (e is IOException or InvalidDataException or UnauthorizedAccessException)
        {
            _dialogs.ShowError(e.Message);
        }
    }

    /// <summary>
    /// Zmena hlasu nahravok na hlas jazyka (vybrane zvuky, skupina alebo cely jazyk) - tempo a intonacia ostanu.
    /// </summary>
    private void DoChangeVoice() => ProcessRecordings(SpeechOperation.ChangeVoice, Resources.FGenerateSounds_TitleChangeVoice);

    /// <summary>
    /// Vycistenie nahravok od sumu (vybrane zvuky, skupina alebo cely jazyk).
    /// </summary>
    private void DoIsolateSounds() => ProcessRecordings(SpeechOperation.Isolate, Resources.FGenerateSounds_TitleIsolate);

    /// <summary>
    /// Prepis nahravok na text - doplnenie textov zvukov a kontrola, ci text zodpoveda nahravke.
    /// </summary>
    private void DoTranscribeSounds()
    {
        if (ChooseScope(Resources.Cmd_TranscribeSounds) is not var (sounds, title, _) || SpeechKey() is not { } key)
            return;

        var language = CurrentLanguage!;
        var languageCode = LoadVoice(language)?.LanguageCode is { Length: > 0 } code ? code : VoiceProfile.LanguageCodeOf(language.Key);
        using var client = new ElevenLabsClient(key);
        using var form = new FTranscribe(new SpeechTranscriber(client, _speechCache), sounds, title, languageCode, _bank.Config.Speech, _player);
        if (form.ShowDialog(this) != DialogResult.OK || form.Changes.Count == 0)
            return;

        var action = new EditSoundTextsAction(this, form.Changes);
        action.Redo();
        RegisterNewAction(action);
    }

    /// <summary>
    /// Zmena textov viacerych zvukov (prevzate prepisy) - jeden krok Spat.
    /// </summary>
    public class EditSoundTextsAction(FMain form, IReadOnlyList<(FyzSound Sound, string OldText, string NewText)> changes) : Action(form)
    {
        /// <inheritdoc />
        public override string CommandName => Resources.Action_SoundTexts;

        /// <inheritdoc />
        public override void Undo() => Set(c => c.OldText);

        /// <inheritdoc />
        public override void Redo() => Set(c => c.NewText);

        private void Set(Func<(FyzSound Sound, string OldText, string NewText), string> text)
        {
            foreach (var change in changes)
                change.Sound.Text = text(change);
            Form.RefreshSoundViews();
        }
    }

    /// <summary>
    /// Vyber rozsahu zvukov s nahravkou (vybrane, skupina, cely jazyk).
    /// </summary>
    /// <returns>zvuky, rozsah do nazvu okna a vybrany rozsah; <see langword="null" />, ak pouzivatel zrusil vyber.</returns>
    private (List<FyzSound> Sounds, string Title, SoundScope Scope)? ChooseScope(string title)
    {
        if (CurrentLanguage?.Groups is not { } groups)
            return null;

        static List<FyzSound> WithFile(IEnumerable<FyzSound> sounds) => sounds.Where(s => s.File is not null).ToList();
        var selected = WithFile(SelectedSounds);
        var group = CurrentGroup is null ? [] : WithFile(CurrentGroup.Sounds);
        var language = WithFile(groups.SelectMany(g => g.Sounds));

        using var scope = new FScope(title, selected.Count, CurrentGroup?.Name ?? "", group.Count, CurrentLanguage.Name, language.Count);
        if (scope.ShowDialog(this) != DialogResult.OK)
            return null;

        return scope.Scope switch
        {
            SoundScope.Selected => (selected, string.Format(CultureInfo.CurrentCulture, Resources.Generate_ScopeSelected, selected.Count), scope.Scope),
            SoundScope.Group => (group, string.Format(CultureInfo.CurrentCulture, Resources.Generate_ScopeGroup, CurrentGroup!.Name), scope.Scope),
            _ => (language, string.Format(CultureInfo.CurrentCulture, Resources.Generate_ScopeLanguage, CurrentLanguage.Name), scope.Scope)
        };
    }

    private void ProcessRecordings(SpeechOperation operation, string title)
    {
        if (ChooseScope(title) is var (sounds, scopeTitle, scope))
            GenerateSounds(sounds, scopeTitle, operation == SpeechOperation.ChangeVoice && scope != SoundScope.Selected, operation);
    }

    /// <summary>
    /// Hlasy uctu ElevenLabs - kniznica, navrh, klon z nahravok banky a odstranenie.
    /// </summary>
    private void DoVoices()
    {
        if (CurrentLanguage is not { } language || SpeechKey() is not { } key)
            return;

        using var client = new ElevenLabsClient(key);
        var voice = LoadVoice(language);
        var languageCode = voice?.LanguageCode is { Length: > 0 } code ? code : VoiceProfile.LanguageCodeOf(language.Key);
        using var form = new FVoices(client, client, _player, languageCode, voice?.VoiceId ?? "", () => CloneVoice(client, language, languageCode));
        form.ShowDialog(this);
    }

    /// <summary>
    /// Klon hlasu z nahravok banky - vybrane zvuky, pri jednom alebo ziadnom vybranom zvuku cela otvorena skupina.
    /// </summary>
    /// <returns><see langword="true" />, ak hlas vznikol.</returns>
    private bool CloneVoice(IVoiceLibrary library, FyzLanguage language, string languageCode)
    {
        var sounds = SelectedSounds.Where(s => s.File is not null).ToList();
        if (sounds.Count < 2 && CurrentGroup is not null)
            sounds = CurrentGroup.Sounds.Where(s => s.File is not null).ToList();
        if (sounds.Count == 0)
        {
            _dialogs.ShowInfo(Resources.FVoiceClone_NoSounds);
            return false;
        }

        var files = sounds.Select(s => s.File.FileInfo.FullName).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var totalMs = sounds.Sum(s => Math.Max(s.File.Duration >= 0 ? s.File.Duration : s.Duration, 0));
        using var form = new FVoiceClone(library, files, totalMs, languageCode, $"INISS {language.Key}");
        return form.ShowDialog(this) == DialogResult.OK;
    }
}
