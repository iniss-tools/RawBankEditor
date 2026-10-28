using System.Globalization;
using RawBankEditor.Properties;
using RawBankEditor.Speech;
using RawBankEditor.Tools;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

/// <summary>
/// Klonovanie hlasu z nahravok banky - napriklad na doplnenie novych stanic hlasom povodneho hlasatela. Klonovat sa
/// smie len so suhlasom hovoriaceho; bez potvrdenia sa hlas nevytvori.
/// </summary>
public partial class FVoiceClone : Form
{
    // nahravky banky su kratke uryvky - spoja sa do vzoriek po najviac minute, sluzba ich prijme najviac 25
    private const int SAMPLE_MS = 60_000;
    private const int MAX_SAMPLES = 25;
    private const int GAP_MS = 300;
    // odporucana dlzka reci pre dobry klon
    private const int RECOMMENDED_MS = 60_000;

    private readonly IVoiceLibrary _library;
    private readonly IReadOnlyList<string> _files;
    private readonly string _languageCode;
    private readonly CancellationTokenSource _cancel = new();

    /// <param name="library">sprava hlasov</param>
    /// <param name="files">nahravky banky (.WAV/.EWA)</param>
    /// <param name="totalMs">celkova dlzka nahravok</param>
    /// <param name="languageCode">jazyk hlasu</param>
    /// <param name="defaultName">predvoleny nazov hlasu</param>
    public FVoiceClone(IVoiceLibrary library, IReadOnlyList<string> files, int totalMs, string languageCode, string defaultName)
    {
        InitializeComponent();
        this.ApplyThemeAndFonts();

        _library = library;
        _files = files;
        _languageCode = languageCode;
        lHint.Text = Resources.FVoiceClone_Hint;
        tbName.Text = defaultName;
        lSourceInfo.Text = string.Format(CultureInfo.CurrentCulture, Resources.FVoiceClone_Source, files.Count, totalMs / 1000.0)
                           + (totalMs < RECOMMENDED_MS ? " " + Resources.FVoiceClone_TooShort : "");
        bClone.Enabled = false;
        FormClosed += (_, _) =>
        {
            _cancel.Cancel();
            _cancel.Dispose();
        };
    }

    /// <summary>
    /// Identifikator vytvoreneho hlasu.
    /// </summary>
    public string VoiceId { get; private set; } = "";

    /// <summary>
    /// Vzorky na klonovanie - nahravky za sebou s pauzou, rozdelene po najviac minute.
    /// </summary>
    public static List<byte[]> Samples(IEnumerable<PcmSound> sounds)
    {
        ArgumentNullException.ThrowIfNull(sounds);

        var samples = new List<byte[]>();
        var chunk = new List<PcmSound>();
        var length = 0;
        foreach (var sound in sounds)
        {
            if (length > 0 && length + sound.DurationMs > SAMPLE_MS)
            {
                samples.Add(SoundProcessing.ToWav(SoundProcessing.Concat(chunk, GAP_MS)));
                chunk.Clear();
                length = 0;
                if (samples.Count == MAX_SAMPLES)
                    return samples;
            }

            chunk.Add(sound);
            length += sound.DurationMs + GAP_MS;
        }

        if (chunk.Count > 0 && samples.Count < MAX_SAMPLES)
            samples.Add(SoundProcessing.ToWav(SoundProcessing.Concat(chunk, GAP_MS)));
        return samples;
    }

    private void CboxConsent_CheckedChanged(object? sender, EventArgs e) => bClone.Enabled = cboxConsent.Checked;

    private async void BClone_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.None;
        var name = tbName.Text.Trim();
        if (!cboxConsent.Checked || name.Length == 0)
        {
            lStatus.Text = name.Length == 0 ? Resources.FVoiceDesign_NoName : Resources.FVoiceClone_NoConsent;
            return;
        }

        bClone.Enabled = false;
        lStatus.Text = Resources.FVoiceClone_Cloning;
        try
        {
            var files = _files;
            var samples = await Task.Run(() => Samples(files.Select(SoundProcessing.ReadFile)), _cancel.Token);
            VoiceId = await _library.CloneAsync(
                new VoiceCloneRequest(name, tbDescription.Text.Trim(), samples, cboxRemoveNoise.Checked, _languageCode), _cancel.Token);
            DialogResult = DialogResult.OK;
            return;
        }
        catch (OperationCanceledException)
        {
            return;
        }
        catch (Exception ex) when (ex is SpeechException or InvalidDataException or IOException or UnauthorizedAccessException)
        {
            if (!IsDisposed)
                lStatus.Text = string.Format(CultureInfo.CurrentCulture, Resources.Speech_Error, ex.Message);
        }

        if (!IsDisposed)
            bClone.Enabled = cboxConsent.Checked;
    }
}
