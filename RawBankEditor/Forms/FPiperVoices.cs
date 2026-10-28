using System.Globalization;
using RawBankEditor.Properties;
using RawBankEditor.Speech;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

/// <summary>
/// Hlasy Piper na stiahnutie - zoznam z uloziska hlasov podla jazyka, stiahnutie do priecinka hlasov a odstranenie.
/// </summary>
public partial class FPiperVoices : Form
{
    private readonly string _folder;
    private readonly string _languageCode;
    private readonly PiperVoiceCatalog _catalog = new();
    private readonly CancellationTokenSource _cancel = new();
    private IReadOnlyList<PiperCatalogVoice> _voices = [];
    private bool _busy;

    /// <param name="folder">priecinok hlasov</param>
    /// <param name="languageCode">jazyk vybrany na zaciatku (ISO 639-1); prazdny = vsetky jazyky</param>
    public FPiperVoices(string folder, string languageCode)
    {
        InitializeComponent();
        this.ApplyThemeAndFonts();

        _folder = folder;
        _languageCode = languageCode;
        dgvVoices.AutoGenerateColumns = false;
        lHint.Text = string.Format(CultureInfo.CurrentCulture, Resources.FPiperVoices_Hint, folder);
        cbLanguage.Items.Add(Resources.FPiperVoices_AllLanguages);
        cbLanguage.SelectedIndex = 0;
        UpdateState();
    }

    /// <summary>
    /// Hlas pribudol alebo sa odstranil - zoznam hlasov treba nacitat znova.
    /// </summary>
    public bool Changed { get; private set; }

    private IEnumerable<PiperCatalogVoice> SelectedVoices =>
        dgvVoices.SelectedRows.Cast<DataGridViewRow>().OrderBy(r => r.Index).Select(r => r.Tag).OfType<PiperCatalogVoice>();

    private bool IsInstalled(PiperCatalogVoice voice) =>
        File.Exists(PiperVoices.ModelPath(_folder, voice.Key)) && File.Exists(PiperVoices.ModelPath(_folder, voice.Key) + ".json");

    private async void FPiperVoices_Shown(object? sender, EventArgs e)
    {
        lStatus.Text = Resources.FPiperVoices_Loading;
        _busy = true;
        UpdateState();
        try
        {
            _voices = await _catalog.LoadAsync(_cancel.Token);
        }
        catch (OperationCanceledException)
        {
            Release();
            return;
        }
        catch (SpeechException ex)
        {
            if (!IsDisposed)
                lStatus.Text = string.Format(CultureInfo.CurrentCulture, Resources.Speech_Error, ex.Message);
        }

        _busy = false;
        if (IsDisposed)
        {
            Release();
            return;
        }

        // jazyky podla nazvu, kod v zatvorke
        var languages = _voices.Where(v => v.Family.Length > 0).GroupBy(v => v.Family)
            .Select(g => new LanguageItem(g.Key, g.First().LanguageName)).OrderBy(l => l.ToString(), StringComparer.CurrentCulture).ToList();
        cbLanguage.SelectedIndexChanged -= CbLanguage_SelectedIndexChanged;
        foreach (var language in languages)
            cbLanguage.Items.Add(language);
        cbLanguage.SelectedItem = languages.FirstOrDefault(l => l.Code == _languageCode) ?? cbLanguage.Items[0];
        cbLanguage.SelectedIndexChanged += CbLanguage_SelectedIndexChanged;
        if (_voices.Count > 0)
            lStatus.Text = "";
        Fill();
    }

    /// <summary>
    /// Riadky hlasov vybraneho jazyka.
    /// </summary>
    private void Fill()
    {
        var code = (cbLanguage.SelectedItem as LanguageItem)?.Code;
        dgvVoices.Rows.Clear();
        foreach (var voice in _voices.Where(v => code is null || v.Family == code))
        {
            var index = dgvVoices.Rows.Add(voice.Key, voice.LanguageName, voice.Quality,
                string.Format(CultureInfo.CurrentCulture, Resources.FPiperVoices_Size, voice.Size / 1048576.0), StateOf(voice));
            dgvVoices.Rows[index].Tag = voice;
        }

        UpdateState();
    }

    private string StateOf(PiperCatalogVoice voice) => IsInstalled(voice) ? Resources.FPiperVoices_Installed : "";

    private void UpdateState()
    {
        var selected = SelectedVoices.ToList();
        bDownload.Enabled = !_busy && selected.Any(v => !IsInstalled(v));
        bDelete.Enabled = !_busy && selected.Any(IsInstalled);
        cbLanguage.Enabled = !_busy;
    }

    private void RefreshStates()
    {
        foreach (DataGridViewRow row in dgvVoices.Rows)
            if (row.Tag is PiperCatalogVoice voice)
                row.Cells[cState.Index].Value = StateOf(voice);
        UpdateState();
    }

    private void CbLanguage_SelectedIndexChanged(object? sender, EventArgs e) => Fill();

    private void DgvVoices_SelectionChanged(object? sender, EventArgs e) => UpdateState();

    private void LlSamples_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e) => Utils.OpenShell(PiperVoiceCatalog.SamplesUrl);

    private async void BDownload_Click(object? sender, EventArgs e)
    {
        var voices = SelectedVoices.Where(v => !IsInstalled(v)).ToList();
        if (voices.Count == 0)
            return;

        _busy = true;
        UpdateState();
        try
        {
            foreach (var voice in voices)
            {
                var progress = new Progress<long>(bytes =>
                {
                    if (!IsDisposed)
                        lStatus.Text = string.Format(CultureInfo.CurrentCulture, Resources.FPiperVoices_Downloading, voice.Key,
                            voice.Size > 0 ? 100.0 * bytes / voice.Size : 0);
                });
                await _catalog.DownloadAsync(voice, _folder, progress, _cancel.Token);
                Changed = true;
                if (!IsDisposed)
                    RefreshStates();
            }

            if (!IsDisposed)
                lStatus.Text = Resources.FPiperVoices_Downloaded;
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex) when (ex is SpeechException or IOException or UnauthorizedAccessException)
        {
            if (!IsDisposed)
                lStatus.Text = string.Format(CultureInfo.CurrentCulture, Resources.FPiperVoices_DownloadFailed, ex.Message);
        }

        _busy = false;
        if (IsDisposed)
            Release();
        else
            RefreshStates();
    }

    private void BDelete_Click(object? sender, EventArgs e)
    {
        var voices = SelectedVoices.Where(IsInstalled).ToList();
        if (voices.Count == 0
            || Utils.ShowQuestion(string.Format(CultureInfo.CurrentCulture, Resources.FPiperVoices_DeleteQuestion, string.Join(", ", voices.Select(v => v.Key))))
            != DialogResult.Yes)
            return;

        try
        {
            foreach (var voice in voices)
            {
                PiperVoices.Delete(_folder, voice.Key);
                Changed = true;
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            Utils.ShowError(ex.Message);
        }

        RefreshStates();
    }

    private void FPiperVoices_FormClosing(object? sender, FormClosingEventArgs e)
    {
        // nedokoncene stiahnutie sa zahodi; klienta uvolni az koniec stahovania
        _cancel.Cancel();
        if (!_busy)
            Release();
    }

    private void Release()
    {
        _catalog.Dispose();
        _cancel.Dispose();
    }

    /// <summary>
    /// Jazyk v ponuke.
    /// </summary>
    private sealed record LanguageItem(string Code, string Name)
    {
        /// <inheritdoc />
        public override string ToString() => Name.Length > 0 ? $"{Name} ({Code})" : Code;
    }
}
