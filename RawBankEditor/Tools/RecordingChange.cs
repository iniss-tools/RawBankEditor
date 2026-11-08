using RawBankEditor.Speech;
using ToolsCore.Iniss.Entities;
using ToolsCore.Iniss.Tools;

namespace RawBankEditor.Tools;

/// <summary>
/// Zmena nahravky zvuku - povodna a nova verzia (kopie WAV v <see cref="SoundVersions" />), dlzky suborov a dlzky zvuku
/// v zozname zvukov pred zmenou a po nej. Bez povodnej verzie (<see cref="CreatesFile" />) nova nahravka subor vytvara
/// (zvuk nahravku nemal) a Spat ho odstrani.
/// </summary>
/// <param name="File">subor nahravky v banke</param>
/// <param name="OldWav">kopia povodnej nahravky; <see langword="null" />, ak subor neexistoval</param>
/// <param name="NewWav">kopia novej nahravky</param>
/// <param name="OldDuration">dlzka povodnej nahravky v ms</param>
/// <param name="NewDuration">dlzka novej nahravky v ms</param>
/// <param name="OldSoundDuration">dlzka zvuku v zozname zvukov pred zmenou</param>
/// <param name="NewSoundDuration">dlzka zvuku v zozname zvukov po zmene (bez prepoctu dlzok ostava povodna)</param>
public sealed record RecordingChange(SoundFileElement File, string? OldWav, string NewWav, int OldDuration, int NewDuration, int OldSoundDuration,
    int NewSoundDuration)
{
    /// <summary>Zvuk nahravky.</summary>
    public FyzSound? Sound { get; init; }

    /// <summary>Nazov suboru zvuku pred zmenou (zvuk bez nahravky ho mohol mat prazdny).</summary>
    public string? OldFileName { get; init; }

    /// <summary>Povod povodnej nahravky (<c>.rbe-ai</c>); <see langword="null" /> = povodna nahravka banky.</summary>
    public SoundOrigin? OldOrigin { get; init; }

    /// <summary>Povod novej nahravky.</summary>
    public SoundOrigin? NewOrigin { get; init; }

    /// <summary>Text pre syntezu pred zmenou.</summary>
    public string? OldText { get; init; }

    /// <summary>Text pre syntezu po zmene.</summary>
    public string? NewText { get; init; }

    /// <summary>Zmena meni aj text pre syntezu.</summary>
    public bool ChangesText => OldText != NewText;

    /// <summary>
    /// Nova nahravka vytvara subor, ktory predtym neexistoval.
    /// </summary>
    public bool CreatesFile => OldWav is null;

    /// <summary>
    /// Zmena existujucej nahravky - povodna verzia sa skopiruje do <paramref name="versions" />.
    /// </summary>
    /// <param name="file">subor nahravky v banke</param>
    /// <param name="newWav">nova nahravka (kopia v <paramref name="versions" />)</param>
    /// <param name="newDuration">dlzka novej nahravky v ms</param>
    /// <param name="versions">kopie verzii</param>
    /// <param name="recalculateDuration">prepocitat dlzku zvuku v zozname zvukov</param>
    public static RecordingChange Replace(SoundFileElement file, string newWav, int newDuration, SoundVersions versions, bool recalculateDuration)
    {
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(versions);

        var oldWav = versions.AddCopyOf(file.FileInfo.FullName);
        var oldDuration = file.Duration >= 0 ? file.Duration : SoundProcessing.ReadWav(System.IO.File.ReadAllBytes(oldWav)).DurationMs;
        var oldSoundDuration = file.Sound?.Duration ?? oldDuration;
        return new RecordingChange(file, oldWav, newWav, oldDuration, newDuration, oldSoundDuration, recalculateDuration ? newDuration : oldSoundDuration)
        {
            Sound = file.Sound
        };
    }

    /// <summary>
    /// Zmena existujucej nahravky na novy zvuk.
    /// </summary>
    public static RecordingChange Create(SoundFileElement file, PcmSound newSound, SoundVersions versions, bool recalculateDuration)
    {
        ArgumentNullException.ThrowIfNull(newSound);
        ArgumentNullException.ThrowIfNull(versions);
        return Replace(file, versions.Add(SoundProcessing.ToWav(newSound)), newSound.DurationMs, versions, recalculateDuration);
    }

    /// <summary>
    /// Nova nahravka zvuku, ktory nahravku nema. Subor lezi tam, kde ho INISS hlada: v priecinku skupiny pod nazvom
    /// suboru zvuku (prazdny nazov = predvoleny podla kluca), pri pridavnej ceste na nej.
    /// </summary>
    /// <param name="sound">zvuk bez nahravky</param>
    /// <param name="pathToBank">priecinok RAWBANK</param>
    /// <param name="newWav">nova nahravka (kopia v <see cref="SoundVersions" />)</param>
    /// <param name="newDuration">dlzka novej nahravky v ms</param>
    /// <param name="recalculateDuration">prepocitat dlzku zvuku v zozname zvukov</param>
    /// <exception cref="DirectoryNotFoundException">priecinok nahravky neexistuje</exception>
    public static RecordingChange CreateMissing(FyzSound sound, string pathToBank, string newWav, int newDuration, bool recalculateDuration)
    {
        ArgumentNullException.ThrowIfNull(sound);

        var fileName = string.IsNullOrWhiteSpace(sound.FileName) ? SoundUtils.GetDefaultFileName(sound.Group, sound.Key) : sound.FileName;
        var additional = !RawBankParser.AdditionalPathIsEmpty(sound.AdditionalRelativePath);
        var directory = sound.Group.GetAbsPath(pathToBank) + (additional ? sound.AdditionalRelativePath : "");
        var path = Path.GetFullPath(Path.Combine(directory, fileName));
        if (!Directory.Exists(Path.GetDirectoryName(path)))
            throw new DirectoryNotFoundException(Path.GetDirectoryName(path));

        // subor v priecinku skupiny patri do stromu prieskumnika, subor na pridavnej ceste nie (ako pri nacitani)
        var file = new SoundFileElement(path) { Parent = additional ? null : sound.Group.Directory };
        return new RecordingChange(file, null, newWav, -1, newDuration, sound.Duration, recalculateDuration ? newDuration : sound.Duration)
        {
            Sound = sound,
            OldFileName = sound.FileName
        };
    }
}
