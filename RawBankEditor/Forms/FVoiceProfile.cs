using System.Globalization;
using RawBankEditor.Properties;
using RawBankEditor.Speech;
using RawBankEditor.Tools;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

/// <summary>
/// Okno hlasu jazyka - sluzba, model, hlas, jazyk reci a nastavenia hlasu, s ukazkou. Zoznamy modelov a hlasov sa
/// nacitaju zo sluzby po otvoreni okna a po zmene sluzby. Profil ulozi hlavne okno.
/// </summary>
public partial class FVoiceProfile : Form
{
    // kody jazykov reci v ponuke - prazdny = model ho urci sam
    private static readonly string[] LanguageCodes = ["", "sk", "cs", "en", "de", "pl", "hu", "uk", "fr", "it", "es"];

    // rychlost, ktoru ElevenLabs prijme; ostatne sluzby zvladnu sirsi rozsah
    private const decimal ELEVENLABS_MIN_SPEED = 0.7m;
    private const decimal ELEVENLABS_MAX_SPEED = 1.2m;
    private const decimal MIN_SPEED = 0.5m;
    private const decimal MAX_SPEED = 2m;

    private readonly SpeechProviders _providers;
    private readonly SpeechCache _cache;
    private readonly AudioPlayer _player;
    private readonly SoundAdjustOptions? _adjust;
    private readonly string _piperVoices;
    private readonly string _numbersLanguage;
    private readonly CancellationTokenSource _cancel = new();
    private readonly VoiceProfile _original;
    // vsetky hlasy sluzby - ponuka ukazuje hlasy jazyka reci
    private IReadOnlyList<SpeechVoice> _voices = [];
    // kazde nacitanie zoznamov ma cislo - neskoro dokoncene nacitanie inej sluzby sa zahodi
    private int _loading;

    /// <param name="providers">nastavene sluzby syntezy</param>
    /// <param name="cache">cache vysledkov (ukazka)</param>
    /// <param name="profile">doterajsi hlas jazyka</param>
    /// <param name="languageName">nazov jazyka banky</param>
    /// <param name="sampleText">text ukazky</param>
    /// <param name="player">prehravac ukazky</param>
    /// <param name="adjust">uprava zvuku ukazky; <see langword="null" /> = bez upravy</param>
    /// <param name="piperVoices">priecinok hlasov Piper</param>
    /// <param name="numbersLanguage">jazyk prevodu cisel na slova v ukazke; prazdny = cisla ostanu</param>
    public FVoiceProfile(SpeechProviders providers, SpeechCache cache, VoiceProfile profile, string languageName, string sampleText,
        AudioPlayer player, SoundAdjustOptions? adjust, string piperVoices, string numbersLanguage = "")
    {
        ArgumentNullException.ThrowIfNull(providers);
        ArgumentNullException.ThrowIfNull(profile);

        InitializeComponent();
        this.ApplyThemeAndFonts();

        _providers = providers;
        _cache = cache;
        _player = player;
        _adjust = adjust;
        _piperVoices = piperVoices;
        _numbersLanguage = numbersLanguage;
        _original = profile.Clone();
        Profile = profile.Clone();

        lHint.Text = string.Format(CultureInfo.CurrentCulture, Resources.FVoiceProfile_Hint, languageName, BankAiFolder.FolderName);
        tbSample.Text = sampleText;

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

        // sluzba hlasu je v ponuke, aj ked uz nie je nastavena - hlas sa da zmenit na inu
        var ids = providers.Ids.ToList();
        if (!ids.Contains(profile.Provider, StringComparer.OrdinalIgnoreCase))
            ids.Insert(0, profile.Provider);
        foreach (var id in ids)
            cbProvider.Items.Add(new ProviderItem(id));
        cbProvider.SelectedIndexChanged -= CbProvider_SelectedIndexChanged;
        cbProvider.SelectedIndex = ids.FindIndex(id => string.Equals(id, profile.Provider, StringComparison.OrdinalIgnoreCase));
        cbProvider.SelectedIndexChanged += CbProvider_SelectedIndexChanged;

        // kym sa zoznamy nenacitaju, ponuka obsahuje len doterajsi model a hlas
        cbModel.Items.Add(new SpeechModel(profile.ModelId, "", []));
        cbModel.SelectedIndex = 0;
        if (profile.VoiceId.Length > 0)
        {
            cbVoice.Items.Add(new SpeechVoice(profile.VoiceId, profile.VoiceName.Length > 0 ? profile.VoiceName : profile.VoiceId, ""));
            cbVoice.SelectedIndex = 0;
        }

        ShowFeatures();
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

    private string ProviderId => (cbProvider.SelectedItem as ProviderItem)?.Id ?? _original.Provider;

    private ISpeechProvider? Provider => _providers.Get(ProviderId);

    private string LanguageCode => cbLanguage.SelectedIndex < LanguageCodes.Length
        ? LanguageCodes[Math.Max(cbLanguage.SelectedIndex, 0)]
        : (string)cbLanguage.SelectedItem!;

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

    /// <summary>
    /// Nastavenia, ktore sluzba pozna: stabilita, podobnost, styl a seed len ElevenLabs; rychlost vsetky.
    /// </summary>
    private void ShowFeatures()
    {
        var features = Provider?.Features ?? SpeechFeatures.None;
        var settings = features.HasFlag(SpeechFeatures.VoiceSettings);
        nudStability.Enabled = nudSimilarity.Enabled = nudStyle.Enabled = cboxSpeakerBoost.Enabled = settings;
        cboxSeed.Enabled = features.HasFlag(SpeechFeatures.Seed);
        nudSeed.Enabled = cboxSeed.Enabled && cboxSeed.Checked;
        lSettingsHint.Text = settings ? Resources.FVoiceProfile_SettingsHint : Resources.FVoiceProfile_SpeedHint;

        var speed = nudSpeed.Value;
        nudSpeed.Minimum = settings ? ELEVENLABS_MIN_SPEED : MIN_SPEED;
        nudSpeed.Maximum = settings ? ELEVENLABS_MAX_SPEED : MAX_SPEED;
        nudSpeed.Value = Math.Clamp(speed, nudSpeed.Minimum, nudSpeed.Maximum);

        bDownloadVoices.Visible = string.Equals(ProviderId, PiperClient.ProviderId, StringComparison.OrdinalIgnoreCase);
        bReload.Enabled = bTry.Enabled = Provider is not null;
    }

    private async Task LoadListsAsync()
    {
        var loading = ++_loading;
        if (Provider is not { } provider)
        {
            lStatus.Text = string.Format(CultureInfo.CurrentCulture, Resources.FVoiceProfile_ProviderMissing, SpeechProviders.NameOf(ProviderId));
            return;
        }

        lStatus.Text = Resources.FVoiceProfile_Loading;
        bReload.Enabled = false;
        try
        {
            var models = await provider.GetModelsAsync(_cancel.Token);
            var voices = await provider.GetVoicesAsync(_cancel.Token);
            if (IsDisposed || loading != _loading)
                return;

            FillModels(models);
            _voices = voices;
            FillVoices();
            lStatus.Text = voices.Count > 0 ? ""
                : provider.Features.HasFlag(SpeechFeatures.Offline) ? Resources.FVoiceProfile_NoPiperVoices
                : Resources.FVoiceProfile_NoVoices;
        }
        catch (OperationCanceledException)
        {
            return;
        }
        catch (SpeechException e)
        {
            if (!IsDisposed && loading == _loading)
                lStatus.Text = string.Format(CultureInfo.CurrentCulture, Resources.Speech_Error, e.Message);
        }

        if (!IsDisposed && loading == _loading)
            bReload.Enabled = true;
    }

    private void FillModels(IReadOnlyList<SpeechModel> models)
    {
        var current = (cbModel.SelectedItem as SpeechModel)?.Id ?? Profile.ModelId;
        cbModel.Items.Clear();
        foreach (var model in models)
            cbModel.Items.Add(model);
        if (models.All(m => m.Id != current))
        {
            // model inej sluzby sa nezachova
            if (models.Count > 0 && (current.Length == 0 || !string.Equals(ProviderId, _original.Provider, StringComparison.OrdinalIgnoreCase)))
                current = models[0].Id;
            else
                cbModel.Items.Insert(0, new SpeechModel(current, "", []));
        }

        cbModel.SelectedItem = cbModel.Items.Cast<SpeechModel>().First(m => m.Id == current);
        FitDropDown(cbModel);
    }

    /// <summary>
    /// Ponuka hlasov - hlasy jazyka reci (ak ziadny nie je, vsetky); vybrany hlas ostava.
    /// </summary>
    private void FillVoices()
    {
        var current = cbVoice.SelectedItem as SpeechVoice;
        var voices = _voices.Where(v => v.Speaks(LanguageCode)).ToList();
        if (voices.Count == 0)
            voices = _voices.ToList();

        cbVoice.Items.Clear();
        foreach (var voice in voices)
            cbVoice.Items.Add(voice);
        if (current is not null && voices.All(v => v.Id != current.Id))
            cbVoice.Items.Insert(0, _voices.FirstOrDefault(v => v.Id == current.Id) ?? current);
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
        profile.Provider = ProviderId;
        profile.ModelId = model.Id;
        profile.VoiceId = voice.Id;
        profile.VoiceName = voice.Name;
        profile.LanguageCode = LanguageCode;
        profile.Stability = (double)nudStability.Value;
        profile.SimilarityBoost = (double)nudSimilarity.Value;
        profile.Style = (double)nudStyle.Value;
        profile.Speed = (double)nudSpeed.Value;
        profile.SpeakerBoost = cboxSpeakerBoost.Checked;
        profile.Seed = cboxSeed.Enabled && cboxSeed.Checked ? decimal.ToUInt32(nudSeed.Value) : null;
        return profile;
    }

    private async void CbProvider_SelectedIndexChanged(object? sender, EventArgs e)
    {
        // hlas a model jednej sluzby v inej neplatia
        cbModel.Items.Clear();
        cbVoice.Items.Clear();
        _voices = [];
        ShowFeatures();
        await LoadListsAsync();
    }

    private void CbLanguage_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (_voices.Count > 0)
            FillVoices();
    }

    private async void BReload_Click(object? sender, EventArgs e) => await LoadListsAsync();

    private async void BDownloadVoices_Click(object? sender, EventArgs e)
    {
        using var form = new FPiperVoices(_piperVoices, LanguageCode);
        form.ShowDialog(this);
        if (form.Changed)
            await LoadListsAsync();
    }

    private async void BTry_Click(object? sender, EventArgs e)
    {
        var profile = ReadProfile();
        if (profile is null)
        {
            lStatus.Text = Resources.FVoiceProfile_NoVoice;
            return;
        }

        if (string.IsNullOrWhiteSpace(tbSample.Text) || Provider is not { } provider)
            return;

        bTry.Enabled = false;
        lStatus.Text = Resources.FVoiceProfile_Generating;
        try
        {
            var result = await new SpeechGenerator(provider, _cache, profile, _adjust, null, _numbersLanguage).GenerateAsync(tbSample.Text, null, _cancel.Token);
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

    private void CboxSeed_CheckedChanged(object? sender, EventArgs e) => nudSeed.Enabled = cboxSeed.Enabled && cboxSeed.Checked;

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

    /// <summary>
    /// Sluzba v ponuke.
    /// </summary>
    private sealed record ProviderItem(string Id)
    {
        /// <inheritdoc />
        public override string ToString() => SpeechProviders.NameOf(Id);
    }
}
