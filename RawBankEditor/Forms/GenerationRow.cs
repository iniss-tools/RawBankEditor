using System.Globalization;
using RawBankEditor.Properties;
using RawBankEditor.Speech;
using ToolsCore.Iniss.Entities;

namespace RawBankEditor.Forms;

/// <summary>
/// Riadok okna generovania - zvuk, text na vyslovenie a vygenerovana nahravka. Nahravka je ako WAV na disku
/// (<see cref="RawBankEditor.Tools.SoundVersions" />), v pamati ostava len jej dlzka - dava celeho jazyka by inak
/// zabrala gigabajty.
/// </summary>
public sealed class GenerationRow
{
    /// <param name="sound">zvuk</param>
    /// <param name="file">jeho nahravka v banke; <see langword="null" />, ak nahravku nema</param>
    /// <param name="origin">povod sucasnej nahravky</param>
    /// <param name="savedText">text pre syntezu zadany pri zvuku; <see langword="null" /> = text zvuku</param>
    public GenerationRow(FyzSound sound, SoundFileElement? file, SoundOrigin? origin = null, string? savedText = null)
    {
        Sound = sound;
        File = file;
        Origin = origin;
        SavedText = savedText;
        Text = savedText ?? SoundRecords.DefaultText(sound);
    }

    /// <summary>Text pre syntezu zadany pri zvuku pred otvorenim okna.</summary>
    [Browsable(false)]
    public string? SavedText { get; }

    /// <summary>
    /// Text pre syntezu na ulozenie pri zvuku - <see langword="null" />, ak sa vyslovuje text zvuku.
    /// </summary>
    [Browsable(false)]
    public string? TextToSave => Text.Trim() == SoundRecords.DefaultText(Sound).Trim() || Text.Trim().Length == 0 ? null : Text.Trim();

    /// <summary>Odtlacok poziadavky vygenerovanej nahravky (<see cref="PreparedSpeech.Fingerprint" />).</summary>
    [Browsable(false)]
    public string Fingerprint { get; set; } = "";

    /// <summary>Zvuk.</summary>
    public FyzSound Sound { get; }

    /// <summary>Nahravka zvuku v banke; <see langword="null" /> = zvuk nahravku nema, vygenerovana ju vytvori.</summary>
    [Browsable(false)]
    public SoundFileElement? File { get; }

    /// <summary>Povod sucasnej nahravky (<c>.rbe-ai</c>).</summary>
    [Browsable(false)]
    public SoundOrigin? Origin { get; }

    /// <summary>Prevziat novu nahravku do banky.</summary>
    public bool Accept { get; set; }

    /// <summary>Kluc skupiny.</summary>
    public string Group => Sound.Group?.Key ?? "";

    /// <summary>Kluc zvuku.</summary>
    public string Key => Sound.Key;

    /// <summary>Text, ktory sa vyslovi (predvolene text zvuku).</summary>
    public string Text { get; set; }

    /// <summary>Stav generovania.</summary>
    public string Status { get; set; } = Resources.FGenerateSounds_StatusWaiting;

    /// <summary>Dlzka povodnej nahravky.</summary>
    public string OldDurationText => File is null
        ? Resources.FGenerateSounds_NoRecording
        : DurationText(File.Duration >= 0 ? File.Duration : Sound.Duration);

    /// <summary>Dlzka novej nahravky.</summary>
    public string NewDurationText => ResultWav is null ? "" : DurationText(ResultDuration);

    /// <summary>Vygenerovana nahravka (WAV na disku); <see langword="null" />, kym nie je.</summary>
    [Browsable(false)]
    public string? ResultWav { get; set; }

    /// <summary>Dlzka vygenerovanej nahravky v ms.</summary>
    [Browsable(false)]
    public int ResultDuration { get; set; }

    /// <summary>Seed posledneho generovania (generovanie znova ho zmeni).</summary>
    [Browsable(false)]
    public uint? Seed { get; set; }

    /// <summary>Zhoda prepisu novej nahravky s ocakavanym textom 0-1; <see langword="null" />, kym sa nekontrolovala.</summary>
    [Browsable(false)]
    public double? Match { get; set; }

    /// <summary>Zhoda v percentach.</summary>
    public string MatchText => Match is { } match ? string.Format(CultureInfo.CurrentCulture, Resources.Match_Percent, match * 100) : "";

    /// <summary>Prepis novej nahravky.</summary>
    [Browsable(false)]
    public string Transcript { get; set; } = "";

    /// <summary>Nova nahravka je podozriva - prepis sa nezhoduje s textom alebo sa dlzka velmi lisi.</summary>
    [Browsable(false)]
    public bool Suspicious { get; set; }

    /// <summary>Prave sa generuje.</summary>
    [Browsable(false)]
    public bool Busy { get; set; }

    /// <summary>
    /// Zabudne vygenerovanu nahravku (iny text alebo hlas).
    /// </summary>
    public void ClearResult()
    {
        ResultWav = null;
        ResultDuration = 0;
        Accept = false;
        Match = null;
        Transcript = "";
        Suspicious = false;
        Status = Resources.FGenerateSounds_StatusWaiting;
    }

    private static string DurationText(int ms) => string.Format(CultureInfo.CurrentCulture, Resources.Duration_Ms, ms);
}
