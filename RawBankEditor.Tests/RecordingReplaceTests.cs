using RawBankEditor.Tools;
using ToolsCore.Iniss.Entities;

namespace RawBankEditor.Tests;

/// <summary>
/// Nahradenie nahravok v banke: zahodenie zmien vrati povodnu nahravku, Spat a Znova prepinaju verzie aj po ulozeni.
/// </summary>
[TestClass]
public sealed class RecordingReplaceTests : IDisposable
{
    private string _root = null!;
    private BankJournal _journal = null!;
    private SoundVersions _versions = null!;

    [TestInitialize]
    public void Init()
    {
        _root = Directory.CreateTempSubdirectory("rbereplace").FullName;
        Directory.CreateDirectory(Path.Combine(_root, "SK", "R1"));
        _journal = new BankJournal(Path.Combine(_root, "_zurnal"), path => File.Delete(path), _ => false);
        _versions = new SoundVersions(Path.Combine(_root, "_verzie"));
    }

    [TestCleanup]
    public void Cleanup() => Directory.Delete(_root, true);

    public void Dispose() => _versions.Dispose();

    private static byte[] Wav(float amplitude) =>
        SoundProcessing.ToWav(new PcmSound(Enumerable.Repeat(amplitude, 2205).ToArray(), SoundProcessing.BankSampleRate));

    private (RecordingChange Change, byte[] Old, byte[] New) Prepare(string name)
    {
        var path = Path.Combine(_root, "SK", "R1", name);
        var old = Wav(0.1f);
        SoundProcessing.WriteSoundFile(path, old);
        var file = new SoundFileElement(path);
        var sound = new FyzSound(null!, "A", "A", name, "", "", 100) { File = file };
        file.Sound = sound;
        var neu = Wav(0.3f);
        return (new RecordingChange(file, _versions.AddCopyOf(path), _versions.Add(neu), 100, 200, 100, 200), old, neu);
    }

    [TestMethod]
    public void Apply_ZapiseNovuVerziuANastaviDlzku()
    {
        var (change, _, neu) = Prepare("A.EWA");

        var errors = RecordingReplace.Apply([change], true, _journal, "SK");

        Assert.IsEmpty(errors);
        CollectionAssert.AreEqual(neu, SoundProcessing.ReadWavBytes(change.File.FileInfo.FullName));
        Assert.AreEqual(200, change.File.Duration);
        Assert.AreEqual(200, change.File.Sound.Duration);
    }

    [TestMethod]
    public void Zahodenie_VratiPovodnuNahravku()
    {
        var (change, old, _) = Prepare("A.WAV");
        RecordingReplace.Apply([change], true, _journal, "SK");

        var errors = _journal.Rollback();

        Assert.IsEmpty(errors);
        CollectionAssert.AreEqual(old, File.ReadAllBytes(change.File.FileInfo.FullName));
    }

    [TestMethod]
    public void SpatPoUlozeni_VratiPovodnuVerziu()
    {
        var (change, old, neu) = Prepare("A.EWA");
        RecordingReplace.Apply([change], true, _journal, "SK");
        _journal.Commit();

        RecordingReplace.Apply([change], false, _journal, "SK");
        CollectionAssert.AreEqual(old, SoundProcessing.ReadWavBytes(change.File.FileInfo.FullName));
        Assert.AreEqual(100, change.File.Sound.Duration);

        RecordingReplace.Apply([change], true, _journal, "SK");
        CollectionAssert.AreEqual(neu, SoundProcessing.ReadWavBytes(change.File.FileInfo.FullName));
    }

    [TestMethod]
    public void Create_BezPrepoctuDlzky_NemeniDlzkuZvuku()
    {
        var path = Path.Combine(_root, "SK", "R1", "B.WAV");
        SoundProcessing.WriteSoundFile(path, Wav(0.1f));
        var file = new SoundFileElement(path) { Duration = 100 };
        file.Sound = new FyzSound(null!, "B", "B", "B.WAV", "", "", 95) { File = file };
        var neu = new PcmSound(new float[SoundProcessing.BankSampleRate / 2], SoundProcessing.BankSampleRate);

        var change = RecordingChange.Create(file, neu, _versions, false);
        RecordingReplace.Apply([change], true, _journal, "SK");

        Assert.AreEqual(500, file.Duration);
        Assert.AreEqual(95, file.Sound.Duration);
        Assert.AreEqual(100, change.OldDuration);
    }
}
