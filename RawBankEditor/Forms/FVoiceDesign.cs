using System.Globalization;
using RawBankEditor.Properties;
using RawBankEditor.Speech;
using RawBankEditor.Tools;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

/// <summary>
/// Navrh hlasu z popisu - sluzba vrati niekolko ukazok, z vybranej sa vytvori hlas uctu.
/// </summary>
public partial class FVoiceDesign : Form
{
    // dlzka textu ukazky podla sluzby
    private const int MIN_TEXT = 100;
    private const int MAX_TEXT = 1000;

    private readonly IVoiceLibrary _library;
    private readonly AudioPlayer _player;
    private readonly CancellationTokenSource _cancel = new();
    private IReadOnlyList<VoicePreview> _previews = [];

    /// <param name="library">sprava hlasov</param>
    /// <param name="player">prehravac ukazok</param>
    public FVoiceDesign(IVoiceLibrary library, AudioPlayer player)
    {
        InitializeComponent();
        this.ApplyThemeAndFonts();

        _library = library;
        _player = player;
        foreach (var model in ElevenLabsClient.DesignModels)
            cbModel.Items.Add(model);
        cbModel.SelectedIndex = 0;
        UpdateButtons();
        FormClosed += (_, _) =>
        {
            _cancel.Cancel();
            _cancel.Dispose();
        };
    }

    /// <summary>
    /// Hlas sa vytvoril.
    /// </summary>
    public bool VoiceCreated { get; private set; }

    private void UpdateButtons(bool busy = false)
    {
        bDesign.Enabled = !busy;
        bPlay.Enabled = !busy && lbPreviews.SelectedIndex >= 0;
        bSave.Enabled = !busy && lbPreviews.SelectedIndex >= 0;
    }

    private async void BDesign_Click(object? sender, EventArgs e)
    {
        var description = tbDescription.Text.Trim();
        var text = tbText.Text.Trim();
        if (description.Length == 0)
        {
            lStatus.Text = Resources.FVoiceDesign_NoDescription;
            return;
        }

        if (text.Length > 0 && text.Length is < MIN_TEXT or > MAX_TEXT)
        {
            lStatus.Text = string.Format(CultureInfo.CurrentCulture, Resources.FVoiceDesign_TextLength, MIN_TEXT, MAX_TEXT, text.Length);
            return;
        }

        UpdateButtons(true);
        lStatus.Text = Resources.FVoiceDesign_Designing;
        try
        {
            _previews = await _library.DesignAsync(description, (string)cbModel.SelectedItem!, text, _cancel.Token);
            if (IsDisposed)
                return;

            lbPreviews.Items.Clear();
            for (var i = 0; i < _previews.Count; i++)
                lbPreviews.Items.Add(string.Format(CultureInfo.CurrentCulture, Resources.FVoiceDesign_Preview, i + 1, _previews[i].Sound.DurationMs / 1000.0));
            lStatus.Text = _previews.Count == 0 ? Resources.FVoiceDesign_NoPreviews : Resources.FVoiceDesign_Done;
            if (_previews.Count > 0)
            {
                lbPreviews.SelectedIndex = 0;
                _player.Play(SoundProcessing.ToWav(_previews[0].Sound));
            }
        }
        catch (OperationCanceledException)
        {
            return;
        }
        catch (SpeechException ex)
        {
            if (!IsDisposed)
                lStatus.Text = string.Format(CultureInfo.CurrentCulture, Resources.Speech_Error, ex.Message);
        }

        if (!IsDisposed)
            UpdateButtons();
    }

    private void BPlay_Click(object? sender, EventArgs e)
    {
        UpdateButtons();
        if (lbPreviews.SelectedIndex >= 0 && lbPreviews.SelectedIndex < _previews.Count)
            _player.Play(SoundProcessing.ToWav(_previews[lbPreviews.SelectedIndex].Sound));
    }

    private async void BSave_Click(object? sender, EventArgs e)
    {
        if (lbPreviews.SelectedIndex < 0 || lbPreviews.SelectedIndex >= _previews.Count)
            return;

        var name = tbName.Text.Trim();
        if (name.Length == 0)
        {
            lStatus.Text = Resources.FVoiceDesign_NoName;
            return;
        }

        UpdateButtons(true);
        lStatus.Text = Resources.FVoiceDesign_Saving;
        try
        {
            await _library.CreateDesignedAsync(name, tbDescription.Text.Trim(), _previews[lbPreviews.SelectedIndex].GeneratedVoiceId, _cancel.Token);
            VoiceCreated = true;
            if (!IsDisposed)
                lStatus.Text = string.Format(CultureInfo.CurrentCulture, Resources.FVoiceDesign_Saved, name);
        }
        catch (OperationCanceledException)
        {
            return;
        }
        catch (SpeechException ex)
        {
            if (!IsDisposed)
                lStatus.Text = string.Format(CultureInfo.CurrentCulture, Resources.Speech_Error, ex.Message);
        }

        if (!IsDisposed)
            UpdateButtons();
    }
}
