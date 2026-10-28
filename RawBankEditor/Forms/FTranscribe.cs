using System.Globalization;
using RawBankEditor.Properties;
using RawBankEditor.Speech;
using RawBankEditor.Tools;
using ToolsCore.Iniss.Entities;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

/// <summary>
/// Prepis nahravok na text (Scribe): doplnenie chybajucich textov zvukov a kontrola, ci text zvuku zodpoveda nahravke.
/// Prevzate prepisy (<see cref="Changes" />) zapise hlavne okno ako jeden krok Spat.
/// </summary>
public partial class FTranscribe : Form
{
    private readonly SpeechTranscriber _transcriber;
    private readonly AudioPlayer _player;
    private readonly SpeechSettings _settings;
    private readonly string _languageCode;
    private readonly List<TranscriptionRow> _allRows;
    private readonly BindingList<TranscriptionRow> _rows = [];
    private readonly CancellationTokenSource _cancel = new();
    private int _running;

    /// <param name="transcriber">prepis s cache</param>
    /// <param name="sounds">zvuky s nahravkou</param>
    /// <param name="title">rozsah v nazve okna</param>
    /// <param name="languageCode">jazyk reci (ISO 639-1); prazdny = zisti sluzba</param>
    /// <param name="settings">nastavenia syntezy (pocet sucasnych poziadaviek)</param>
    /// <param name="player">prehravac</param>
    public FTranscribe(SpeechTranscriber transcriber, IEnumerable<FyzSound> sounds, string title, string languageCode, SpeechSettings settings, AudioPlayer player)
    {
        InitializeComponent();
        this.ApplyThemeAndFonts();

        _transcriber = transcriber;
        _player = player;
        _settings = settings;
        _languageCode = languageCode;
        _allRows = sounds.Where(s => s.File is not null).Select(s => new TranscriptionRow(s)).ToList();

        Text = string.Format(CultureInfo.CurrentCulture, Resources.FGenerateSounds_Title, Text, title);
        lHint.Text = Resources.FTranscribe_Hint;
        dgvItems.AutoGenerateColumns = false;
        dgvItems.DataSource = _rows;
        dgvItems.CellToolTipTextNeeded += (_, e) =>
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == cTranscript.Index && dgvItems.Rows[e.RowIndex].DataBoundItem is TranscriptionRow { Heard.Length: > 0 } row)
                e.ToolTipText = string.Format(CultureInfo.CurrentCulture, Resources.Match_Transcript, row.Heard);
        };
        // bez textov je cielom ich doplnenie - zobrazia sa len zvuky bez textu
        cboxOnlyEmpty.Checked = _allRows.Any(r => r.CurrentText.Length == 0);
        Filter();
    }

    /// <summary>
    /// Prevzate texty zvukov (po OK).
    /// </summary>
    public IReadOnlyList<(FyzSound Sound, string OldText, string NewText)> Changes { get; private set; } = [];

    private IEnumerable<TranscriptionRow> SelectedRows =>
        dgvItems.SelectedRows.Cast<DataGridViewRow>().Select(r => r.DataBoundItem).OfType<TranscriptionRow>().OrderBy(_rows.IndexOf);

    private TranscriptionRow? CurrentRow => dgvItems.CurrentRow?.DataBoundItem as TranscriptionRow;

    private void Filter()
    {
        _rows.RaiseListChangedEvents = false;
        _rows.Clear();
        foreach (var row in _allRows.Where(r => (!cboxOnlyEmpty.Checked || r.CurrentText.Length == 0) && (!cboxSuspicious.Checked || r.Mismatch)))
            _rows.Add(row);
        _rows.RaiseListChangedEvents = true;
        _rows.ResetBindings();
        UpdateState();
    }

    private void UpdateState()
    {
        var busy = _running > 0;
        bTranscribe.Enabled = !busy && _rows.Count > 0;
        bPlay.Enabled = CurrentRow is not null;
        bOK.Enabled = !busy && _allRows.Any(r => r.Accept && r.Transcript.Length > 0);
        var done = _rows.Count(r => r.Heard.Length > 0);
        lInfo.Text = busy
            ? string.Format(CultureInfo.CurrentCulture, Resources.FTranscribe_Progress, _rows.Count(r => r.Busy))
            : string.Format(CultureInfo.CurrentCulture, Resources.FTranscribe_Summary, _rows.Count, done, _rows.Count(r => r.Mismatch));
    }

    private void Refresh(TranscriptionRow row)
    {
        if (IsDisposed)
            return;

        var index = _rows.IndexOf(row);
        if (index >= 0)
            _rows.ResetItem(index);
        UpdateState();
    }

    private async void BTranscribe_Click(object? sender, EventArgs e)
    {
        // vybrane riadky, bez vyberu vsetky zobrazene
        var rows = (dgvItems.SelectedRows.Count > 1 ? SelectedRows : _rows).Where(r => !r.Busy).ToList();
        if (rows.Count == 0)
            return;

        _running++;
        var cancel = _cancel.Token;
        using var gate = new SemaphoreSlim(Math.Clamp(_settings.Concurrency, 1, SpeechSettings.MaxConcurrency));
        foreach (var row in rows)
        {
            row.Busy = true;
            row.Status = Resources.FGenerateSounds_StatusQueued;
            Refresh(row);
        }

        await Task.WhenAll(rows.Select(row => TranscribeRowAsync(row, gate, cancel)));
        _running--;
        if (!IsDisposed)
            UpdateState();
    }

    private async Task TranscribeRowAsync(TranscriptionRow row, SemaphoreSlim gate, CancellationToken cancel)
    {
        try
        {
            await gate.WaitAsync(cancel);
        }
        catch (OperationCanceledException)
        {
            row.Busy = false;
            return;
        }

        try
        {
            row.Status = Resources.FTranscribe_StatusRunning;
            Refresh(row);
            var path = row.Sound.File.FileInfo.FullName;
            var language = _languageCode;
            var (heard, _) = await Task.Run(() => _transcriber.TranscribeFileAsync(path, language, cancel), cancel);
            row.SetHeard(heard);
        }
        catch (OperationCanceledException)
        {
            row.Status = Resources.FGenerateSounds_StatusCanceled;
        }
        catch (Exception ex) when (ex is SpeechException or InvalidDataException or IOException)
        {
            row.Status = string.Format(CultureInfo.CurrentCulture, Resources.Speech_Error, ex.Message);
        }
        finally
        {
            gate.Release();
            row.Busy = false;
            Refresh(row);
        }
    }

    private void CboxFilter_CheckedChanged(object? sender, EventArgs e) => Filter();

    private void Play()
    {
        if (CurrentRow is not { } row)
            return;

        try
        {
            _player.PlayFile(row.Sound.File.FileInfo.FullName);
        }
        catch (Exception ex) when (ex is IOException or InvalidDataException or UnauthorizedAccessException)
        {
            Utils.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.FGenerateSounds_PlayFailed, row.Key, ex.Message));
        }
    }

    private void BPlay_Click(object? sender, EventArgs e) => Play();

    private void BStop_Click(object? sender, EventArgs e) => _player.Stop();

    private void DgvItems_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0 && e.ColumnIndex != cTranscript.Index && e.ColumnIndex != cAccept.Index)
            Play();
    }

    private void DgvItems_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
    {
        // upraveny prepis sa prevezme
        if (e.ColumnIndex == cTranscript.Index && dgvItems.Rows[e.RowIndex].DataBoundItem is TranscriptionRow row)
        {
            row.Accept = row.Transcript.Trim().Length > 0 && row.Transcript.Trim() != row.CurrentText;
            Refresh(row);
        }
    }

    private void DgvItems_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
    {
        if (dgvItems.CurrentCell is DataGridViewCheckBoxCell)
            dgvItems.CommitEdit(DataGridViewDataErrorContexts.Commit);
        UpdateState();
    }

    private void DgvItems_SelectionChanged(object? sender, EventArgs e) => UpdateState();

    private void BOK_Click(object? sender, EventArgs e)
    {
        dgvItems.EndEdit();
        Changes = _allRows.Where(r => r.Accept && r.Transcript.Trim().Length > 0 && r.Transcript.Trim() != r.CurrentText)
            .Select(r => (r.Sound, r.CurrentText, r.Transcript.Trim()))
            .ToList();
        DialogResult = DialogResult.OK;
    }

    private void FTranscribe_FormClosing(object? sender, FormClosingEventArgs e)
    {
        _player.Stop();
        _cancel.Cancel();
    }

    /// <inheritdoc />
    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _cancel.Dispose();
        base.OnFormClosed(e);
    }
}

/// <summary>
/// Riadok okna prepisu - zvuk, jeho text a prepis nahravky.
/// </summary>
public sealed class TranscriptionRow(FyzSound sound)
{
    /// <summary>Zvuk.</summary>
    [Browsable(false)]
    public FyzSound Sound { get; } = sound;

    /// <summary>Prevziat prepis ako text zvuku.</summary>
    public bool Accept { get; set; }

    /// <summary>Kluc skupiny.</summary>
    public string Group => Sound.Group?.Key ?? "";

    /// <summary>Kluc zvuku.</summary>
    public string Key => Sound.Key;

    /// <summary>Sucasny text zvuku.</summary>
    public string CurrentText { get; } = sound.Text?.Trim() ?? "";

    /// <summary>Prepis (upravitelny) - novy text zvuku.</summary>
    public string Transcript { get; set; } = "";

    /// <summary>Prepis od sluzby.</summary>
    [Browsable(false)]
    public string Heard { get; private set; } = "";

    /// <summary>Zhoda prepisu so sucasnym textom; <see langword="null" />, ak zvuk text nema alebo este nie je prepis.</summary>
    [Browsable(false)]
    public double? Match { get; private set; }

    /// <summary>Zhoda v percentach.</summary>
    public string MatchText => Match is { } match ? string.Format(CultureInfo.CurrentCulture, Resources.Match_Percent, match * 100) : "";

    /// <summary>Text zvuku sa nezhoduje s nahravkou.</summary>
    [Browsable(false)]
    public bool Mismatch => Match < TextMatch.Threshold;

    /// <summary>Stav.</summary>
    public string Status { get; set; } = "";

    /// <summary>Prave sa prepisuje.</summary>
    [Browsable(false)]
    public bool Busy { get; set; }

    /// <summary>
    /// Prevezme prepis: zvuk bez textu ho dostane (prevzatie zapnute), pri texte sa porovna - nezhodu treba posudit.
    /// </summary>
    public void SetHeard(string heard)
    {
        Heard = heard;
        Transcript = TextMatch.AsSoundText(heard);
        if (CurrentText.Length == 0)
        {
            Match = null;
            Accept = Transcript.Length > 0;
            Status = Transcript.Length > 0 ? Resources.FTranscribe_StatusNew : Resources.FTranscribe_StatusEmpty;
            return;
        }

        Match = TextMatch.Similarity(CurrentText, heard);
        Accept = false;
        Status = Mismatch ? Resources.FTranscribe_StatusMismatch : Resources.FTranscribe_StatusMatch;
    }
}
