using System.Globalization;
using RawBankEditor.Properties;
using RawBankEditor.Tools;
using ToolsCore.Iniss.Entities;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

/// <summary>
/// Okno upravy zvuku nahravok bez AI: rozsah (vybrane zvuky, skupina, cely jazyk) a nastavenia - ticho na okrajoch,
/// hlasitost a format. Nahravky upravi hlavne okno podla <see cref="Files" /> a <see cref="Options" />.
/// </summary>
public partial class FAdjustSounds : Form
{
    private readonly IReadOnlyList<SoundFileElement> _selected;
    private readonly IReadOnlyList<SoundFileElement> _group;
    private readonly IReadOnlyList<SoundFileElement> _language;
    private readonly AudioPlayer _player;
    private readonly CancellationTokenSource _cancel = new();

    /// <param name="options">posledne pouzite nastavenia</param>
    /// <param name="selected">nahravky vybranych zvukov</param>
    /// <param name="groupName">nazov otvorenej skupiny</param>
    /// <param name="group">nahravky otvorenej skupiny</param>
    /// <param name="languageName">nazov jazyka</param>
    /// <param name="language">nahravky celeho jazyka</param>
    /// <param name="player">prehravac ukazky</param>
    public FAdjustSounds(SoundAdjustOptions options, IReadOnlyList<SoundFileElement> selected, string groupName, IReadOnlyList<SoundFileElement> group,
        string languageName, IReadOnlyList<SoundFileElement> language, AudioPlayer player)
    {
        ArgumentNullException.ThrowIfNull(options);

        InitializeComponent();
        this.ApplyThemeAndFonts();

        _selected = selected;
        _group = group;
        _language = language;
        _player = player;

        rbSelected.Text = string.Format(CultureInfo.CurrentCulture, Resources.FAdjustSounds_Selected, selected.Count);
        rbGroup.Text = string.Format(CultureInfo.CurrentCulture, Resources.FAdjustSounds_Group, groupName, group.Count);
        rbLanguage.Text = string.Format(CultureInfo.CurrentCulture, Resources.FAdjustSounds_Language, languageName, language.Count);
        rbSelected.Enabled = selected.Count > 0;
        rbGroup.Enabled = group.Count > 0;
        rbLanguage.Enabled = language.Count > 0;
        (selected.Count > 0 ? rbSelected : group.Count > 0 ? rbGroup : rbLanguage).Checked = true;

        cboxTrim.Checked = options.TrimSilence;
        SetValue(nudLead, options.LeadMs);
        SetValue(nudTail, options.TailMs);
        SetValue(nudThreshold, options.SilenceThresholdDb);
        cboxNormalize.Checked = options.Normalize;
        SetValue(nudLevel, options.TargetLevelDb);
        cboxConvert.Checked = options.ConvertFormat;
        SetValue(nudFade, options.FadeMs);
        bMeasure.Enabled = language.Count > 0;
        OptionChanged(this, EventArgs.Empty);

        FormClosed += (_, _) =>
        {
            _cancel.Cancel();
            _cancel.Dispose();
        };
    }

    /// <summary>
    /// Nastavenia upravy.
    /// </summary>
    public SoundAdjustOptions Options => new()
    {
        TrimSilence = cboxTrim.Checked,
        LeadMs = decimal.ToInt32(nudLead.Value),
        TailMs = decimal.ToInt32(nudTail.Value),
        SilenceThresholdDb = (double)nudThreshold.Value,
        Normalize = cboxNormalize.Checked,
        TargetLevelDb = (double)nudLevel.Value,
        ConvertFormat = cboxConvert.Checked,
        FadeMs = decimal.ToInt32(nudFade.Value)
    };

    /// <summary>
    /// Nahravky na upravu podla vybraneho rozsahu.
    /// </summary>
    public IReadOnlyList<SoundFileElement> Files => rbSelected.Checked ? _selected : rbGroup.Checked ? _group : _language;

    private static void SetValue(NumericUpDown nud, double value) => nud.Value = Math.Clamp((decimal)value, nud.Minimum, nud.Maximum);

    private void OptionChanged(object? sender, EventArgs e)
    {
        nudLead.Enabled = nudTail.Enabled = cboxTrim.Checked;
        // prah ticha urcuje aj, co sa pri hlasitosti povazuje za rec
        nudThreshold.Enabled = cboxTrim.Checked || cboxNormalize.Checked;
        nudLevel.Enabled = cboxNormalize.Checked;
    }

    private async void BMeasure_Click(object? sender, EventArgs e)
    {
        var paths = _language.Select(f => f.FileInfo.FullName).ToList();
        var threshold = (double)nudThreshold.Value;
        var cancel = _cancel.Token;

        bMeasure.Enabled = false;
        lMeasure.Text = Resources.FAdjustSounds_Measuring;
        double? level;
        try
        {
            level = await Task.Run(() => SoundProcessing.MedianSpeechLevelDb(paths, threshold, cancel: cancel), cancel);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        if (IsDisposed)
            return;

        bMeasure.Enabled = true;
        if (level is null)
        {
            lMeasure.Text = Resources.FAdjustSounds_MeasureFailed;
            return;
        }

        SetValue(nudLevel, Math.Round(level.Value * 2) / 2);
        lMeasure.Text = string.Format(CultureInfo.CurrentCulture, Resources.FAdjustSounds_Measured, level.Value);
    }

    private void BPreview_Click(object? sender, EventArgs e)
    {
        if (Files.Count == 0)
            return;

        var file = Files[0];

        try
        {
            var sound = SoundProcessing.Process(SoundProcessing.ReadFile(file.FileInfo.FullName), Options);
            _player.Play(SoundProcessing.ToWav(sound));
        }
        catch (Exception ex) when (ex is IOException or InvalidDataException or UnauthorizedAccessException)
        {
            Utils.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.FAdjustSounds_PreviewFailed, file.Name, ex.Message));
        }
    }

    private void BOK_Click(object? sender, EventArgs e)
    {
        var options = Options;
        if (!options.TrimSilence && !options.Normalize && !options.ConvertFormat && options.FadeMs == 0)
        {
            Utils.ShowWarning(Resources.FAdjustSounds_Nothing);
            DialogResult = DialogResult.None;
            return;
        }

        DialogResult = DialogResult.OK;
    }
}
