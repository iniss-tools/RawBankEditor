using System.Globalization;
using RawBankEditor.Properties;
using RawBankEditor.Speech;
using RawBankEditor.Tools;
using ToolsCore.Iniss.Entities;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

/// <summary>
/// Generovanie nahravok hlasom jazyka (vybrane zvuky, skupina, cely jazyk alebo chybajuce nahravky) a ich kontrola:
/// povodna a nova nahravka sa daju prehrat, text upravit a nahravku vygenerovat znova. Prevzate riadky
/// (<see cref="Accepted" />) zapise do banky hlavne okno ako jeden krok Spat.
/// </summary>
public partial class FGenerateSounds : Form
{
    // nad tolko znakov sa generovanie potvrdzuje
    private const long CONFIRM_CHARACTERS = 5000;
    // pri zmene hlasu a vycisteni nad tolko sekund zvuku
    private const long CONFIRM_SECONDS = 300;

    private readonly ISpeechProvider _provider;
    private readonly SpeechCache _cache;
    private readonly Func<VoiceProfile?> _editVoice;
    private readonly SpeechSettings _settings;
    private readonly SoundAdjustOptions _adjust;
    private readonly AudioPlayer _player;
    private readonly SoundVersions _versions;
    private readonly Func<FyzGroup, GroupProsody?> _prosodyOf;
    private readonly IReadOnlyList<PronunciationRule> _pronunciation;
    private readonly SpeechOperation _operation;
    private readonly string _languageCode;
    // pri zmene hlasu a vycisteni sa dlzka nahravky nema velmi zmenit
    private const double MIN_DURATION_RATIO = 0.6;
    private const double MAX_DURATION_RATIO = 1.6;
    private readonly List<GenerationRow> _allRows;
    private readonly BindingList<GenerationRow> _rows = [];
    private readonly CancellationTokenSource _cancel = new();
    private readonly System.Windows.Forms.Timer _stateTimer = new() { Interval = 200 };
    private VoiceProfile _profile;
    private SpeechQuota? _quota;
    private int _running;
    private long _characters;
    // stav a odhad sa pocas generovania obnovuju najviac raz za 200 ms - pri tisickach riadkov by inak okno stalo
    private bool _stateDirty;
    private bool _estimateDirty = true;
    private (int Count, long Characters) _estimate;

    /// <param name="provider">sluzba syntezy</param>
    /// <param name="cache">cache vysledkov</param>
    /// <param name="profile">hlas jazyka</param>
    /// <param name="editVoice">otvori okno hlasu jazyka; vrati novy hlas alebo <see langword="null" /> pri zruseni</param>
    /// <param name="title">rozsah v nazve okna (napr. skupina)</param>
    /// <param name="rows">zvuky na generovanie</param>
    /// <param name="skipDone">vynechat zvuky, ktorych nahravka uz vznikla tymto hlasom z rovnakeho textu</param>
    /// <param name="settings">nastavenia syntezy (pocet sucasnych poziadaviek, uprava zvuku)</param>
    /// <param name="adjust">uprava zvuku</param>
    /// <param name="player">prehravac</param>
    /// <param name="versions">kopie verzii nahravok - ukladaju sa sem vygenerovane nahravky</param>
    /// <param name="prosodyOf">intonacia skupiny</param>
    /// <param name="pronunciation">slovnik vyslovnosti jazyka</param>
    /// <param name="operation">nova nahravka z textu, zmena hlasu alebo vycistenie povodnej nahravky</param>
    /// <param name="languageCode">jazyk reci pre kontrolu prepisom (ISO 639-1); prazdny = zisti sluzba</param>
    public FGenerateSounds(ISpeechProvider provider, SpeechCache cache, VoiceProfile profile, Func<VoiceProfile?> editVoice, string title,
        IEnumerable<GenerationRow> rows, bool skipDone, SpeechSettings settings, SoundAdjustOptions adjust, AudioPlayer player, SoundVersions versions,
        Func<FyzGroup, GroupProsody?> prosodyOf, IReadOnlyList<PronunciationRule> pronunciation, SpeechOperation operation = SpeechOperation.Tts,
        string languageCode = "")
    {
        InitializeComponent();
        this.ApplyThemeAndFonts();

        _provider = provider;
        _cache = cache;
        _profile = profile;
        _editVoice = editVoice;
        _settings = settings;
        _adjust = adjust;
        _player = player;
        _versions = versions;
        _prosodyOf = prosodyOf;
        _pronunciation = pronunciation;
        _operation = operation;
        _languageCode = languageCode;
        _allRows = rows.ToList();

        Text = string.Format(CultureInfo.CurrentCulture, Resources.FGenerateSounds_Title, operation switch
        {
            SpeechOperation.ChangeVoice => Resources.FGenerateSounds_TitleChangeVoice,
            SpeechOperation.Isolate => Resources.FGenerateSounds_TitleIsolate,
            _ => Text
        }, title);
        if (operation != SpeechOperation.Tts)
        {
            // nahravka vznika z povodnej, nie z textu
            cText.Visible = false;
            cStatus.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            bGenerate.Text = Resources.FGenerateSounds_Process;
        }

        // vycistenie povod nemeni - neda sa poznat, co uz vycistene je
        cboxSkipDone.Visible = operation != SpeechOperation.Isolate;
        skipDone &= operation != SpeechOperation.Isolate;
        dgvItems.AutoGenerateColumns = false;
        dgvItems.DataSource = _rows;
        cboxAdjust.Checked = settings.AdjustSound;
        cboxVerify.Checked = settings.Verify;
        dgvItems.CellToolTipTextNeeded += (_, e) =>
        {
            // prepis pri zhode - co nahravka naozaj hovori
            if (e.RowIndex >= 0 && e.ColumnIndex == cMatch.Index && dgvItems.Rows[e.RowIndex].DataBoundItem is GenerationRow { Transcript.Length: > 0 } row)
                e.ToolTipText = string.Format(CultureInfo.CurrentCulture, Resources.Match_Transcript, row.Transcript);
        };
        cboxSkipDone.Checked = skipDone;
        cboxSkipDone.Enabled = _allRows.Any(r => r.Origin is not null);
        ShowVoice();
        Filter();

        _stateTimer.Tick += (_, _) =>
        {
            if (_stateDirty)
                UpdateState();
        };
        _stateTimer.Start();
    }

    /// <summary>
    /// Prevzate riadky s vygenerovanou nahravkou (po OK).
    /// </summary>
    public IReadOnlyList<GenerationRow> Accepted { get; private set; } = [];

    /// <summary>
    /// Hlas, ktorym sa nahravky vygenerovali (mohol sa v okne zmenit).
    /// </summary>
    public VoiceProfile Profile => _profile;

    private IEnumerable<GenerationRow> SelectedRows =>
        dgvItems.SelectedRows.Cast<DataGridViewRow>().Select(r => r.DataBoundItem).OfType<GenerationRow>().OrderBy(_rows.IndexOf);

    private GenerationRow? CurrentRow => dgvItems.CurrentRow?.DataBoundItem as GenerationRow;

    private SpeechGenerator Generator() => new(_provider, _cache, _profile, cboxAdjust.Checked ? _adjust : null, _pronunciation);

    private SpeechInput Input(GenerationRow row) => new(row.Text, _prosodyOf(row.Sound.Group));

    private void ShowVoice() =>
        lVoice.Text = string.Format(CultureInfo.CurrentCulture, Resources.FGenerateSounds_Voice, _profile.VoiceName, _profile.ModelId);

    /// <summary>
    /// Zobrazi riadky - pri <see cref="cboxSkipDone" /> bez zvukov, ktorych nahravka uz vznikla tymto hlasom.
    /// </summary>
    private void Filter()
    {
        var skip = cboxSkipDone.Checked;
        var generator = Generator();
        _rows.RaiseListChangedEvents = false;
        _rows.Clear();
        var suspicious = cboxSuspicious.Checked;
        foreach (var row in _allRows.Where(r => (!skip || r.ResultWav is not null || !Done(r, generator)) && (!suspicious || r.Suspicious)))
            _rows.Add(row);
        _rows.RaiseListChangedEvents = true;
        _rows.ResetBindings();
        _estimateDirty = true;
        UpdateState();
    }

    /// <summary>
    /// Povoli tlacidla a ukaze odhad znakov, priebeh a zostatok predplatneho.
    /// </summary>
    private void UpdateState()
    {
        _stateDirty = false;
        var busy = _running > 0;
        bGenerate.Enabled = !busy && _rows.Any(r => r.ResultWav is null);
        // vycistenie dava vzdy rovnaky vysledok - znova nema zmysel
        bRegenerate.Enabled = !busy && dgvItems.SelectedRows.Count > 0 && _operation != SpeechOperation.Isolate;
        bVoice.Enabled = cboxAdjust.Enabled = !busy;
        cboxSkipDone.Enabled = !busy && _allRows.Any(r => r.Origin is not null);
        bPlayOld.Enabled = CurrentRow?.File is not null;
        bPlayNew.Enabled = CurrentRow?.ResultWav is not null;
        bPlaySequence.Enabled = dgvItems.SelectedRows.Count > 0;
        bOK.Enabled = !busy && _rows.Any(r => r.Accept && r.ResultWav is not null);

        string info;
        if (busy)
        {
            info = string.Format(CultureInfo.CurrentCulture, Resources.FGenerateSounds_Progress, _rows.Count(r => r.Busy), _characters);
        }
        else if (_rows.Count == 0)
        {
            info = Resources.FGenerateSounds_AllDone;
        }
        else
        {
            if (_estimateDirty)
            {
                _estimate = Estimate(_rows.Where(r => r.ResultWav is null));
                _estimateDirty = false;
            }

            info = string.Format(CultureInfo.CurrentCulture,
                _operation == SpeechOperation.Tts ? Resources.FGenerateSounds_Estimate : Resources.FGenerateSounds_EstimateAudio,
                _estimate.Count, _estimate.Characters, _characters);
        }

        if (_quota is not null)
            info += " " + string.Format(CultureInfo.CurrentCulture, Resources.FGenerateSounds_Quota, Remaining, _quota.Tier);
        lInfo.Text = info;
    }

    private long Remaining => Math.Max(0, (_quota?.Remaining ?? 0) - _characters);

    /// <summary>
    /// Nahravka uz vznikla tak, ako by vznikla teraz (rovnaky hlas, intonacia a text, pri zmene hlasu rovnaky hlas).
    /// </summary>
    private bool Done(GenerationRow row, SpeechGenerator generator) => _operation switch
    {
        SpeechOperation.Tts => row.Origin?.SameAs(generator.Prepare(Input(row), null).Fingerprint) == true,
        SpeechOperation.ChangeVoice => row.Origin?.SameAs(generator.ConversionFingerprint()) == true,
        _ => false
    };

    private bool CanRun(GenerationRow row) => _operation == SpeechOperation.Tts ? !string.IsNullOrWhiteSpace(row.Text) : row.File is not null;

    /// <summary>
    /// Pocet nahravok a znaky, ktore sa poslu sluzbe (vysledky v cache nic nestoja); pri operacii nad nahravkou
    /// namiesto znakov sekundy zvuku.
    /// </summary>
    private (int Count, long Characters) Estimate(IEnumerable<GenerationRow> rows)
    {
        if (_operation != SpeechOperation.Tts)
        {
            var files = rows.Where(CanRun).ToList();
            // sekundy nahor - aj kratky uryvok sa zauctuje
            return (files.Count, (files.Sum(r => (long)Math.Max(r.File!.Duration >= 0 ? r.File.Duration : r.Sound.Duration, 0)) + 999) / 1000);
        }

        var generator = Generator();
        var prepared = rows.Where(r => !string.IsNullOrWhiteSpace(r.Text)).Select(r => generator.Prepare(Input(r), r.Seed)).ToList();
        return (prepared.Count, prepared.Where(p => !generator.IsCached(p)).Sum(p => (long)p.Characters));
    }

    private void Refresh(GenerationRow row)
    {
        if (IsDisposed)
            return;

        var index = _rows.IndexOf(row);
        if (index >= 0)
            _rows.ResetItem(index);
        _stateDirty = true;
    }

    private async void FGenerateSounds_Shown(object? sender, EventArgs e)
    {
        try
        {
            _quota = await _provider.GetQuotaAsync(_cancel.Token);
        }
        catch (OperationCanceledException)
        {
            return;
        }
        catch (SpeechException ex)
        {
            if (!IsDisposed)
                lInfo.Text = string.Format(CultureInfo.CurrentCulture, Resources.Speech_Error, ex.Message);
            return;
        }

        if (!IsDisposed)
            UpdateState();
    }

    /// <summary>
    /// Pri vela znakoch alebo nedostatku kreditu sa spyta, ci generovat.
    /// </summary>
    private bool ConfirmCost(IEnumerable<GenerationRow> rows)
    {
        var (count, characters) = Estimate(rows);
        if (_operation != SpeechOperation.Tts)
            return characters <= CONFIRM_SECONDS
                   || Utils.ShowQuestion(string.Format(CultureInfo.CurrentCulture, Resources.FGenerateSounds_ConfirmAudio, count, characters)) == DialogResult.Yes;
        if (_quota is not null && characters > Remaining)
            return Utils.ShowWarning(string.Format(CultureInfo.CurrentCulture, Resources.FGenerateSounds_OverQuota, characters, Remaining),
                MessageBoxButtons.YesNo) == DialogResult.Yes;
        if (characters > CONFIRM_CHARACTERS)
            return Utils.ShowQuestion(string.Format(CultureInfo.CurrentCulture, Resources.FGenerateSounds_ConfirmCost, count, characters)) == DialogResult.Yes;
        return true;
    }

    /// <summary>
    /// Vygeneruje nahravky riadkov - najviac <see cref="SpeechSettings.Concurrency" /> naraz.
    /// </summary>
    /// <param name="newSeed">kazdy riadok dostane novy seed (generovanie znova dava inu nahravku)</param>
    private async Task RunAsync(IEnumerable<GenerationRow> candidates, bool newSeed)
    {
        var rows = candidates.Where(r => !r.Busy && CanRun(r)).ToList();
        if (rows.Count == 0)
            return;

        _running++;
        var cancel = _cancel.Token;
        var generator = Generator();
        using var gate = new SemaphoreSlim(Math.Clamp(_settings.Concurrency, 1, SpeechSettings.MaxConcurrency));
        foreach (var row in rows)
        {
            row.Busy = true;
            row.Status = Resources.FGenerateSounds_StatusQueued;
            Refresh(row);
        }

        UpdateState();
        await Task.WhenAll(rows.Select(row => GenerateRowAsync(row, generator, gate, newSeed, cancel)));

        _running--;
        _estimateDirty = true;
        if (!IsDisposed)
            UpdateState();
    }

    private async Task GenerateRowAsync(GenerationRow row, SpeechGenerator generator, SemaphoreSlim gate, bool newSeed, CancellationToken cancel)
    {
        try
        {
            await gate.WaitAsync(cancel);
        }
        catch (OperationCanceledException)
        {
            row.Busy = false;
            row.Status = Resources.FGenerateSounds_StatusCanceled;
            Refresh(row);
            return;
        }

        try
        {
            if (newSeed)
                row.Seed = SpeechGenerator.NewSeed();
            row.Status = Resources.FGenerateSounds_StatusGenerating;
            Refresh(row);

            var input = Input(row);
            var seed = row.Seed;
            var versions = _versions;
            var operation = _operation;
            var source = row.File?.FileInfo.FullName;
            var transcriber = cboxVerify.Checked ? new SpeechTranscriber(_provider, _cache) : null;
            var languageCode = _languageCode;
            var (wav, duration, fromCache, characters, transcript) = await Task.Run(async () =>
            {
                var result = operation == SpeechOperation.Tts
                    ? await generator.GenerateAsync(input, seed, cancel)
                    : await generator.ProcessAudioAsync(operation, source!, seed, cancel);
                string? heard = null;
                if (transcriber is not null)
                {
                    try
                    {
                        heard = (await transcriber.TranscribeAsync(result.Sound, languageCode, cancel)).Text;
                    }
                    catch (SpeechException e)
                    {
                        // nahravka je v poriadku, len sa neda skontrolovat
                        ToolsCore.Iniss.Tools.Log.Exception(e);
                    }
                }

                return (versions.Add(SoundProcessing.ToWav(result.Sound)), result.Sound.DurationMs, result.FromCache, result.Characters, heard);
            }, cancel);
            row.Fingerprint = operation switch
            {
                SpeechOperation.Tts => generator.Prepare(input, seed).Fingerprint,
                SpeechOperation.ChangeVoice => generator.ConversionFingerprint(),
                _ => ""
            };
            row.ResultWav = wav;
            row.ResultDuration = duration;
            row.Accept = true;
            row.Status = fromCache ? Resources.FGenerateSounds_StatusCached : Resources.FGenerateSounds_StatusDone;
            Check(row, transcript, generator, input);
            _characters += characters;
        }
        catch (OperationCanceledException)
        {
            row.Status = Resources.FGenerateSounds_StatusCanceled;
        }
        catch (Exception ex) when (ex is SpeechException or InvalidDataException or IOException)
        {
            row.ClearResult();
            row.Status = string.Format(CultureInfo.CurrentCulture, Resources.Speech_Error, ex.Message);
        }
        finally
        {
            gate.Release();
            row.Busy = false;
            Refresh(row);
        }
    }

    /// <summary>
    /// Kontrola novej nahravky: zhoda prepisu s ocakavanym textom a pri zmene hlasu a vycisteni aj dlzka. Podozrivu
    /// nahravku okno neprevezme samo.
    /// </summary>
    /// <param name="transcript">prepis; <see langword="null" /> = bez kontroly prepisom</param>
    private void Check(GenerationRow row, string? transcript, SpeechGenerator generator, SpeechInput input)
    {
        string? problem = null;
        if (transcript is not null)
        {
            // ocakava sa text zvuku aj text po slovniku vyslovnosti (to sluzba naozaj vyslovila)
            string[] expected = _operation == SpeechOperation.Tts
                ? [row.Text, generator.Prepare(input, row.Seed).Request.Text]
                : [row.SavedText ?? "", SoundRecords.DefaultText(row.Sound)];
            row.Transcript = transcript;
            row.Match = TextMatch.Best(expected, transcript);
            if (row.Match < TextMatch.Threshold)
                problem = string.Format(CultureInfo.CurrentCulture, Resources.Match_Low, row.Match * 100);
        }

        if (problem is null && _operation != SpeechOperation.Tts && row.File is { Duration: > 0 } file)
        {
            var ratio = (double)row.ResultDuration / file.Duration;
            if (ratio is < MIN_DURATION_RATIO or > MAX_DURATION_RATIO)
                problem = string.Format(CultureInfo.CurrentCulture, Resources.Match_Duration, ratio * 100);
        }

        row.Suspicious = problem is not null;
        if (problem is null)
            return;

        row.Accept = false;
        row.Status = problem;
    }

    private void CboxVerify_CheckedChanged(object? sender, EventArgs e) => _settings.Verify = cboxVerify.Checked;

    private void CboxSuspicious_CheckedChanged(object? sender, EventArgs e) => Filter();

    private async void BGenerate_Click(object? sender, EventArgs e)
    {
        var rows = _rows.Where(r => r.ResultWav is null).ToList();
        if (ConfirmCost(rows))
            await RunAsync(rows, false);
    }

    private async void BRegenerate_Click(object? sender, EventArgs e)
    {
        var rows = SelectedRows.ToList();
        // novy seed = vzdy nova poziadavka
        if (ConfirmCost(rows.Select(r => new GenerationRow(r.Sound, r.File, null, r.Text) { Seed = SpeechGenerator.NewSeed() })))
            await RunAsync(rows, true);
    }

    private void BVoice_Click(object? sender, EventArgs e)
    {
        if (_editVoice() is not { } profile)
            return;

        // nahravky ineho hlasu by sa do banky nehodili
        _profile = profile;
        foreach (var row in _allRows)
            row.ClearResult();

        ShowVoice();
        Filter();
    }

    private void CboxSkipDone_CheckedChanged(object? sender, EventArgs e) => Filter();

    private async void CboxAdjust_CheckedChanged(object? sender, EventArgs e)
    {
        _settings.AdjustSound = cboxAdjust.Checked;

        // surove nahravky su v cache - uprava sa len prepocita
        var done = _rows.Where(r => r.ResultWav is not null).ToList();
        foreach (var row in done)
            row.ResultWav = null;
        await RunAsync(done, false);
    }

    private void Play(Func<GenerationRow, string?> path)
    {
        if (CurrentRow is not { } row)
            return;

        try
        {
            if (path(row) is { } file)
                _player.PlayFile(file);
        }
        catch (Exception ex) when (ex is IOException or InvalidDataException or UnauthorizedAccessException)
        {
            Utils.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.FGenerateSounds_PlayFailed, row.Key, ex.Message));
        }
    }

    private void BPlayOld_Click(object? sender, EventArgs e) => Play(row => row.File?.FileInfo.FullName);

    private void BPlayNew_Click(object? sender, EventArgs e) => Play(row => row.ResultWav);

    private void BStop_Click(object? sender, EventArgs e) => _player.Stop();

    private void BPlaySequence_Click(object? sender, EventArgs e)
    {
        // vybrane riadky v poradi zoznamu - nova nahravka, kde je, inak povodna
        var files = SelectedRows.Select(r => r.ResultWav ?? r.File?.FileInfo.FullName).OfType<string>().ToList();
        if (files.Count == 0)
            return;

        try
        {
            _player.Play(SoundProcessing.ToWav(SoundProcessing.Concat(files.Select(SoundProcessing.ReadFile))));
        }
        catch (Exception ex) when (ex is IOException or InvalidDataException or UnauthorizedAccessException)
        {
            Utils.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.FGenerateSounds_PlayFailed, "", ex.Message));
        }
    }

    private void DgvItems_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex == cText.Index || e.ColumnIndex == cAccept.Index)
            return;

        if (CurrentRow?.ResultWav is not null)
            BPlayNew_Click(sender, e);
        else
            BPlayOld_Click(sender, e);
    }

    private void DgvItems_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.ColumnIndex != cText.Index || dgvItems.Rows[e.RowIndex].DataBoundItem is not GenerationRow row || row.Busy)
            return;

        // novy text = nova nahravka
        row.ClearResult();
        _estimateDirty = true;
        Refresh(row);
    }

    private void DgvItems_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
    {
        // zaskrtnutie Prevziat sa prejavi hned, nie az po opusteni bunky
        if (dgvItems.CurrentCell is DataGridViewCheckBoxCell)
            dgvItems.CommitEdit(DataGridViewDataErrorContexts.Commit);
        _stateDirty = true;
    }

    private void DgvItems_SelectionChanged(object? sender, EventArgs e) => _stateDirty = true;

    private void DgvItems_KeyDown(object? sender, KeyEventArgs e)
    {
        // medzernik prepne Prevziat vybranych riadkov s novou nahravkou (mimo upravy textu)
        if (e.KeyCode != Keys.Space || dgvItems.IsCurrentCellInEditMode)
            return;

        var rows = SelectedRows.Where(r => r.ResultWav is not null).ToList();
        if (rows.Count == 0)
            return;

        var accept = !rows.All(r => r.Accept);
        foreach (var row in rows)
        {
            row.Accept = accept;
            Refresh(row);
        }

        e.Handled = true;
    }

    private void DgvItems_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
    {
        // klik na hlavicku Prevziat oznaci vsetky vygenerovane, druhy klik ziadnu
        if (e.ColumnIndex != cAccept.Index)
            return;

        dgvItems.EndEdit();
        var rows = _rows.Where(r => r.ResultWav is not null).ToList();
        var accept = !rows.All(r => r.Accept);
        foreach (var row in rows)
            row.Accept = accept;
        _rows.ResetBindings();
        UpdateState();
    }

    private void BOK_Click(object? sender, EventArgs e)
    {
        dgvItems.EndEdit();
        Accepted = _rows.Where(r => r.Accept && r.ResultWav is not null).ToList();
        DialogResult = DialogResult.OK;
    }

    private void FGenerateSounds_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (DialogResult != DialogResult.OK && _allRows.Any(r => r.ResultWav is not null)
                                             && Utils.ShowQuestion(Resources.FGenerateSounds_DiscardQuestion) != DialogResult.Yes)
        {
            e.Cancel = true;
            return;
        }

        _player.Stop();
        _cancel.Cancel();
    }

    /// <inheritdoc />
    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _stateTimer.Dispose();
        _cancel.Dispose();
        base.OnFormClosed(e);
    }
}
