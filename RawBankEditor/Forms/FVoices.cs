using System.Globalization;
using RawBankEditor.Properties;
using RawBankEditor.Speech;
using RawBankEditor.Tools;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

/// <summary>
/// Hlasy uctu ElevenLabs - ukazka, odstranenie vlastneho hlasu a pridanie hlasu z kniznice, navrhom alebo klonom.
/// </summary>
public partial class FVoices : Form
{
    private readonly ISpeechProvider _provider;
    private readonly IVoiceLibrary _library;
    private readonly AudioPlayer _player;
    private readonly string _languageCode;
    private readonly string _languageVoiceId;
    private readonly Func<bool>? _clone;
    private readonly BindingList<SpeechVoice> _voices = [];
    private readonly CancellationTokenSource _cancel = new();
    private bool _busy;

    /// <param name="provider">sluzba syntezy (zoznam hlasov)</param>
    /// <param name="library">sprava hlasov</param>
    /// <param name="player">prehravac ukazok</param>
    /// <param name="languageCode">jazyk reci banky - predvoleny filter kniznice a jazyk navrhu</param>
    /// <param name="languageVoiceId">hlas jazyka banky (upozornenie pri odstraneni)</param>
    /// <param name="clone">otvori klonovanie z nahravok banky; vrati <see langword="true" />, ak hlas vznikol</param>
    public FVoices(ISpeechProvider provider, IVoiceLibrary library, AudioPlayer player, string languageCode, string languageVoiceId, Func<bool>? clone)
    {
        InitializeComponent();
        this.ApplyThemeAndFonts();

        _provider = provider;
        _library = library;
        _player = player;
        _languageCode = languageCode;
        _languageVoiceId = languageVoiceId;
        _clone = clone;

        lHint.Text = Resources.FVoices_Hint;
        dgvVoices.AutoGenerateColumns = false;
        dgvVoices.DataSource = _voices;
        bClone.Enabled = clone is not null;
        UpdateButtons();
        FormClosed += (_, _) =>
        {
            _cancel.Cancel();
            _cancel.Dispose();
        };
    }

    private SpeechVoice? Current => dgvVoices.CurrentRow?.DataBoundItem as SpeechVoice;

    private void UpdateButtons()
    {
        bPreview.Enabled = !_busy && Current is { PreviewUrl.Length: > 0 };
        bDelete.Enabled = !_busy && Current is { Own: true };
        bLibrary.Enabled = bDesign.Enabled = !_busy;
        bClone.Enabled = !_busy && _clone is not null;
    }

    private async void FVoices_Shown(object? sender, EventArgs e) => await LoadAsync();

    private async Task LoadAsync()
    {
        await RunAsync(Resources.FVoiceProfile_Loading, async cancel =>
        {
            var voices = await _provider.GetVoicesAsync(cancel);
            _voices.RaiseListChangedEvents = false;
            _voices.Clear();
            foreach (var voice in voices)
                _voices.Add(voice);
            _voices.RaiseListChangedEvents = true;
            _voices.ResetBindings();
            return "";
        });
    }

    /// <summary>
    /// Spusti operaciu sluzby; pocas nej su tlacidla vypnute, vysledok alebo chyba ide do stavoveho riadku.
    /// </summary>
    private async Task RunAsync(string status, Func<CancellationToken, Task<string>> action)
    {
        _busy = true;
        UpdateButtons();
        lStatus.Text = status;
        try
        {
            var result = await action(_cancel.Token);
            if (!IsDisposed)
                lStatus.Text = result;
        }
        catch (OperationCanceledException)
        {
            return;
        }
        catch (Exception ex) when (ex is SpeechException or InvalidDataException)
        {
            if (!IsDisposed)
                lStatus.Text = string.Format(CultureInfo.CurrentCulture, Resources.Speech_Error, ex.Message);
        }

        if (IsDisposed)
            return;
        _busy = false;
        UpdateButtons();
    }

    private void DgvVoices_SelectionChanged(object? sender, EventArgs e) => UpdateButtons();

    private async void BPreview_Click(object? sender, EventArgs e)
    {
        if (Current is not { PreviewUrl.Length: > 0 } voice)
            return;

        await RunAsync(Resources.FVoices_Downloading, async cancel =>
        {
            var sound = await _library.DownloadPreviewAsync(voice.PreviewUrl, cancel);
            _player.Play(SoundProcessing.ToWav(sound));
            return "";
        });
    }

    private async void BDelete_Click(object? sender, EventArgs e)
    {
        if (Current is not { Own: true } voice)
            return;

        var question = string.Format(CultureInfo.CurrentCulture, Resources.FVoices_DeleteQuestion, voice.Name)
                       + (voice.Id == _languageVoiceId ? " " + Resources.FVoices_DeleteLanguageVoice : "");
        if (Utils.ShowQuestion(question) != DialogResult.Yes)
            return;

        await RunAsync(Resources.FVoices_Deleting, async cancel =>
        {
            await _library.DeleteVoiceAsync(voice.Id, cancel);
            _voices.Remove(voice);
            return string.Format(CultureInfo.CurrentCulture, Resources.FVoices_Deleted, voice.Name);
        });
    }

    private async void BLibrary_Click(object? sender, EventArgs e)
    {
        using var form = new FVoiceLibrary(_library, _player, _languageCode);
        form.ShowDialog(this);
        if (form.Added)
            await LoadAsync();
    }

    private async void BDesign_Click(object? sender, EventArgs e)
    {
        using var form = new FVoiceDesign(_library, _player);
        form.ShowDialog(this);
        if (form.VoiceCreated)
            await LoadAsync();
    }

    private async void BClone_Click(object? sender, EventArgs e)
    {
        if (_clone?.Invoke() == true)
            await LoadAsync();
    }
}
