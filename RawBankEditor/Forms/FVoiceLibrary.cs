using System.Globalization;
using RawBankEditor.Properties;
using RawBankEditor.Speech;
using RawBankEditor.Tools;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

/// <summary>
/// Kniznica zdielanych hlasov ElevenLabs - hladanie podla jazyka, pohlavia a textu, ukazka a pridanie do hlasov uctu.
/// </summary>
public partial class FVoiceLibrary : Form
{
    // kody jazykov v ponuke - prazdny = vsetky
    private static readonly string[] Languages = ["", "sk", "cs", "en", "de", "pl", "hu", "uk"];
    private static readonly string[] Genders = ["", "female", "male", "neutral"];

    private readonly IVoiceLibrary _library;
    private readonly AudioPlayer _player;
    private readonly BindingList<SharedVoice> _voices = [];
    private readonly CancellationTokenSource _cancel = new();
    private SharedVoiceQuery? _query;
    private bool _hasMore;
    private bool _busy;

    /// <param name="library">sprava hlasov</param>
    /// <param name="player">prehravac ukazok</param>
    /// <param name="languageCode">predvoleny jazyk filtra</param>
    public FVoiceLibrary(IVoiceLibrary library, AudioPlayer player, string languageCode)
    {
        InitializeComponent();
        this.ApplyThemeAndFonts();

        _library = library;
        _player = player;
        foreach (var code in Languages)
            cbLanguage.Items.Add(code.Length == 0 ? Resources.FVoiceLibrary_All : LanguageName(code));
        cbLanguage.SelectedIndex = Math.Max(Array.IndexOf(Languages, languageCode), 0);
        cbGender.Items.AddRange([Resources.FVoiceLibrary_All, Resources.FVoiceLibrary_Female, Resources.FVoiceLibrary_Male, Resources.FVoiceLibrary_Neutral]);
        cbGender.SelectedIndex = 0;
        dgvVoices.AutoGenerateColumns = false;
        dgvVoices.DataSource = _voices;
        UpdateButtons();
        FormClosed += (_, _) =>
        {
            _cancel.Cancel();
            _cancel.Dispose();
        };
    }

    /// <summary>
    /// Do hlasov uctu pribudol aspon jeden hlas.
    /// </summary>
    public bool Added { get; private set; }

    private SharedVoice? Current => dgvVoices.CurrentRow?.DataBoundItem as SharedVoice;

    private static string LanguageName(string code)
    {
        try
        {
            return $"{CultureInfo.GetCultureInfo(code).DisplayName} ({code})";
        }
        catch (CultureNotFoundException)
        {
            return code;
        }
    }

    private void UpdateButtons()
    {
        bSearch.Enabled = !_busy;
        bMore.Enabled = !_busy && _hasMore;
        bPreview.Enabled = !_busy && Current is { PreviewUrl.Length: > 0 };
        bAdd.Enabled = !_busy && Current is not null;
    }

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

    private async Task SearchAsync(SharedVoiceQuery query)
    {
        await RunAsync(Resources.FVoiceLibrary_Searching, async cancel =>
        {
            var page = await _library.SearchSharedAsync(query, cancel);
            if (query.Page == 0)
                _voices.Clear();
            foreach (var voice in page.Voices)
                _voices.Add(voice);
            _query = query;
            _hasMore = page.HasMore;
            return _voices.Count == 0 ? Resources.FVoiceLibrary_NotFound : string.Format(CultureInfo.CurrentCulture, Resources.FVoiceLibrary_Found, _voices.Count);
        });
    }

    private async void FVoiceLibrary_Shown(object? sender, EventArgs e) => await SearchAsync(NewQuery());

    private SharedVoiceQuery NewQuery() =>
        new(Languages[Math.Max(cbLanguage.SelectedIndex, 0)], Genders[Math.Max(cbGender.SelectedIndex, 0)], tbSearch.Text, 0);

    private async void BSearch_Click(object? sender, EventArgs e) => await SearchAsync(NewQuery());

    private async void BMore_Click(object? sender, EventArgs e)
    {
        if (_query is not null)
            await SearchAsync(_query with { Page = _query.Page + 1 });
    }

    private void DgvVoices_SelectionChanged(object? sender, EventArgs e) => UpdateButtons();

    private async void BPreview_Click(object? sender, EventArgs e)
    {
        if (Current is not { PreviewUrl.Length: > 0 } voice)
            return;

        await RunAsync(Resources.FVoices_Downloading, async cancel =>
        {
            _player.Play(SoundProcessing.ToWav(await _library.DownloadPreviewAsync(voice.PreviewUrl, cancel)));
            return "";
        });
    }

    private async void BAdd_Click(object? sender, EventArgs e)
    {
        if (Current is not { } voice)
            return;

        await RunAsync(Resources.FVoiceLibrary_Adding, async cancel =>
        {
            await _library.AddSharedAsync(voice, voice.Name, cancel);
            Added = true;
            return string.Format(CultureInfo.CurrentCulture, Resources.FVoiceLibrary_AddedVoice, voice.Name);
        });
    }
}
