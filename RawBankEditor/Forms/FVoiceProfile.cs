using System.Globalization;
using RawBankEditor.Properties;
using RawBankEditor.Speech;
using RawBankEditor.Tools;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

/// <summary>
/// Okno hlasu jazyka - model, hlas, jazyk reci a nastavenia hlasu, s ukazkou. Zoznamy modelov a hlasov sa nacitaju zo
/// sluzby po otvoreni okna. Profil ulozi hlavne okno.
/// </summary>
public partial class FVoiceProfile : Form
{
    // kody jazykov reci v ponuke - prazdny = model ho urci sam
    private static readonly string[] LanguageCodes = ["", "sk", "cs", "en", "de", "pl", "hu", "uk", "fr", "it", "es"];

    private readonly ISpeechProvider _provider;
    private readonly SpeechCache _cache;
    private readonly AudioPlayer _player;
    private readonly SoundAdjustOptions? _adjust;
    private readonly CancellationTokenSource _cancel = new();
    private readonly VoiceProfile _original;

    /// <param name="provider">sluzba syntezy</param>
    /// <param name="cache">cache vysledkov (ukazka)</param>
    /// <param name="profile">doterajsi hlas jazyka</param>
    /// <param name="languageName">nazov jazyka banky</param>
    /// <param name="sampleText">text ukazky</param>
    /// <param name="player">prehravac ukazky</param>
    /// <param name="adjust">uprava zvuku ukazky; <see langword="null" /> = bez upravy</param>
    public FVoiceProfile(ISpeechProvider provider, SpeechCache cache, VoiceProfile profile, string languageName, string sampleText,
        AudioPlayer player, SoundAdjustOptions? adjust)
    {
        ArgumentNullException.ThrowIfNull(profile);

        InitializeComponent();
        this.ApplyThemeAndFonts();

        _provider = provider;
        _cache = cache;
        _player = player;
        _adjust = adjust;
        _original = profile.Clone();
        Profile = profile.Clone();

        lHint.Text = string.Format(CultureInfo.CurrentCulture, Resources.FVoiceProfile_Hint, languageName, BankAiFolder.FolderName);
        tbSample.Text = sampleText;

        // kym sa zoznamy nenacitaju, ponuka obsahuje len doterajsi model a hlas
        cbModel.Items.Add(new SpeechModel(profile.ModelId, "", []));
        cbModel.SelectedIndex = 0;
        if (profile.VoiceId.Length > 0)
        {
            cbVoice.Items.Add(new SpeechVoice(profile.VoiceId, profile.VoiceName.Length > 0 ? profile.VoiceName : profile.VoiceId, ""));
            cbVoice.SelectedIndex = 0;
        }

        foreach (var code in LanguageCodes)
            cbLanguage.Items.Add(code.Length == 0 ? Resources.FVoiceProfile_LanguageAuto : LanguageName(code));
        var languageIndex = Array.IndexOf(LanguageCodes, profile.LanguageCode);
        if (languageIndex < 0 && profile.LanguageCode.Length > 0)
        {
            // kod mimo ponuky (napr. z ineho pocitaca) ostava
            cbLanguage.Items.Add(profile.LanguageCode);
            languageIndex = cbLanguage.Items.Count - 1;
        }

        cbLanguage.SelectedIndex = Math.Max(languageIndex, 0);

        SetValue(nudStability, profile.Stability);
        SetValue(nudSimilarity, profile.SimilarityBoost);
        SetValue(nudStyle, profile.Style);
        SetValue(nudSpeed, profile.Speed);
        cboxSpeakerBoost.Checked = profile.SpeakerBoost;
        cboxSeed.Checked = profile.Seed is not null;
        SetValue(nudSeed, profile.Seed ?? SpeechGenerator.NewSeed());
        nudSeed.Enabled = cboxSeed.Checked;

        Shown += async (_, _) => await LoadListsAsync();
        FormClosed += (_, _) =>
        {
            _cancel.Cancel();
            _cancel.Dispose();
        };
    }

    /// <summary>
    /// Hlas jazyka po potvrdeni okna.
    /// </summary>
    public VoiceProfile Profile { get; private set; }

    private static void SetValue(NumericUpDown nud, double value) => nud.Value = Math.Clamp((decimal)value, nud.Minimum, nud.Maximum);

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

    private async Task LoadListsAsync()
    {
        lStatus.Text = Resources.FVoiceProfile_Loading;
        bReload.Enabled = false;
        try
        {
            var models = await _provider.GetModelsAsync(_cancel.Token);
            var voices = await _provider.GetVoicesAsync(_cancel.Token);
            if (IsDisposed)
                return;

            FillModels(models);
            FillVoices(voices);
            lStatus.Text = voices.Count == 0 ? Resources.FVoiceProfile_NoVoices : "";
        }
        catch (OperationCanceledException)
        {
            return;
        }
        catch (SpeechException e)
        {
            if (!IsDisposed)
                lStatus.Text = string.Format(CultureInfo.CurrentCulture, Resources.Speech_Error, e.Message);
        }

        if (!IsDisposed)
            bReload.Enabled = true;
    }

    private void FillModels(IReadOnlyList<SpeechModel> models)
    {
        var current = (cbModel.SelectedItem as SpeechModel)?.Id ?? Profile.ModelId;
        cbModel.Items.Clear();
        foreach (var model in models)
            cbModel.Items.Add(model);
        if (models.All(m => m.Id != current))
            cbModel.Items.Insert(0, new SpeechModel(current, "", []));
        cbModel.SelectedItem = cbModel.Items.Cast<SpeechModel>().First(m => m.Id == current);
        FitDropDown(cbModel);
    }

    private void FillVoices(IReadOnlyList<SpeechVoice> voices)
    {
        var current = cbVoice.SelectedItem as SpeechVoice;
        cbVoice.Items.Clear();
        foreach (var voice in voices)
            cbVoice.Items.Add(voice);
        if (current is not null && voices.All(v => v.Id != current.Id))
            cbVoice.Items.Insert(0, current);
        if (current is not null)
            cbVoice.SelectedItem = cbVoice.Items.Cast<SpeechVoice>().First(v => v.Id == current.Id);
        FitDropDown(cbVoice);
    }

    /// <summary>
    /// Rozbalovaci zoznam siroky ako najdlhsia polozka (najviac po okraj obrazovky), aby sa polozky neorezali.
    /// </summary>
    private static void FitDropDown(ComboBox combo)
    {
        var widest = combo.Items.Cast<object>().Select(item => TextRenderer.MeasureText(combo.GetItemText(item), combo.Font).Width).DefaultIfEmpty(0).Max();
        var available = Screen.FromControl(combo).WorkingArea.Right - combo.PointToScreen(Point.Empty).X;
        combo.DropDownWidth = Math.Clamp(widest + SystemInformation.VerticalScrollBarWidth + 8, combo.Width, Math.Max(combo.Width, available));
    }

    /// <summary>
    /// Hlas podla okna; <see langword="null" />, ak nie je vybrany hlas.
    /// </summary>
    private VoiceProfile? ReadProfile()
    {
        if (cbVoice.SelectedItem is not SpeechVoice voice || cbModel.SelectedItem is not SpeechModel model)
            return null;

        var profile = _original.Clone();
        profile.ModelId = model.Id;
        profile.VoiceId = voice.Id;
        profile.VoiceName = voice.Name;
        profile.LanguageCode = cbLanguage.SelectedIndex < LanguageCodes.Length
            ? LanguageCodes[Math.Max(cbLanguage.SelectedIndex, 0)]
            : (string)cbLanguage.SelectedItem!;
        profile.Stability = (double)nudStability.Value;
        profile.SimilarityBoost = (double)nudSimilarity.Value;
        profile.Style = (double)nudStyle.Value;
        profile.Speed = (double)nudSpeed.Value;
        profile.SpeakerBoost = cboxSpeakerBoost.Checked;
        profile.Seed = cboxSeed.Checked ? decimal.ToUInt32(nudSeed.Value) : null;
        return profile;
    }

    private async void BReload_Click(object? sender, EventArgs e) => await LoadListsAsync();

    private async void BTry_Click(object? sender, EventArgs e)
    {
        var profile = ReadProfile();
        if (profile is null)
        {
            lStatus.Text = Resources.FVoiceProfile_NoVoice;
            return;
        }

        if (string.IsNullOrWhiteSpace(tbSample.Text))
            return;

        bTry.Enabled = false;
        lStatus.Text = Resources.FVoiceProfile_Generating;
        try
        {
            var result = await new SpeechGenerator(_provider, _cache, profile, _adjust).GenerateAsync(tbSample.Text, null, _cancel.Token);
            if (IsDisposed)
                return;

            _player.Play(SoundProcessing.ToWav(result.Sound));
            lStatus.Text = result.FromCache
                ? Resources.FVoiceProfile_SampleCached
                : string.Format(CultureInfo.CurrentCulture, Resources.FVoiceProfile_SampleDone, result.Characters);
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

        if (!IsDisposed)
            bTry.Enabled = true;
    }

    private void CboxSeed_CheckedChanged(object? sender, EventArgs e) => nudSeed.Enabled = cboxSeed.Checked;

    private void BOK_Click(object? sender, EventArgs e)
    {
        var profile = ReadProfile();
        if (profile is null)
        {
            Utils.ShowError(Resources.FVoiceProfile_NoVoice);
            DialogResult = DialogResult.None;
            return;
        }

        Profile = profile;
        DialogResult = DialogResult.OK;
    }
}
