using System.Text;
using RawBankEditor.Tools;
using ToolsCore.Iniss.Entities;

namespace RawBankEditor.Tests;

[TestClass]
public class SoundUtilsTests
{
    private string _bank = null!;
    private string _groupDir = null!;

    [TestInitialize]
    public void Init()
    {
        _bank = Path.Combine(Path.GetTempPath(), "RawBankEditor.Tests_" + Guid.NewGuid().ToString("N")) + "\\";
        _groupDir = Path.Combine(_bank, "SK", "R1");
        Directory.CreateDirectory(_groupDir);
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (Directory.Exists(_bank))
            Directory.Delete(_bank, true);
    }

    [TestMethod]
    public void Konverzia_WavEwaWavVratiPovodneData()
    {
        var wav = WriteWav("A.WAV");
        var ewa = Path.Combine(_groupDir, "A.EWA");
        var back = Path.Combine(_groupDir, "B.WAV");

        SoundUtils.ConvertWAVtoEWA(wav, ewa);
        SoundUtils.ConvertEWAtoWAV(ewa, back);

        CollectionAssert.AreNotEqual(File.ReadAllBytes(wav), File.ReadAllBytes(ewa));
        CollectionAssert.AreEqual(File.ReadAllBytes(wav), File.ReadAllBytes(back));
    }

    [TestMethod]
    public void Konverzia_SKontrolouPrijmePlatnyWavAVratiPovodneData()
    {
        var wav = WriteWav("A.WAV");
        var ewa = Path.Combine(_groupDir, "A.EWA");
        var back = Path.Combine(_groupDir, "B.WAV");

        SoundUtils.ConvertWAVtoEWA(wav, ewa, check: true);
        SoundUtils.ConvertEWAtoWAV(ewa, back, check: true);

        CollectionAssert.AreEqual(File.ReadAllBytes(wav), File.ReadAllBytes(back));
    }

    [TestMethod]
    public void Konverzia_SKontrolouOdmietneNeplatnyWav()
    {
        var bad = Path.Combine(_groupDir, "X.WAV");
        File.WriteAllBytes(bad, Encoding.ASCII.GetBytes("toto nie je ziadny WAV subor"));
        var ewa = Path.Combine(_groupDir, "X.EWA");

        Assert.ThrowsExactly<FormatException>(() => SoundUtils.ConvertWAVtoEWA(bad, ewa, check: true));

        // EWA vytvoreny bez kontroly sa odhali az pri dekodovani s kontrolou
        SoundUtils.ConvertWAVtoEWA(bad, ewa);
        Assert.ThrowsExactly<FormatException>(() => SoundUtils.ConvertEWAtoWAV(ewa, Path.Combine(_groupDir, "Y.WAV"), check: true));
    }

    [TestMethod]
    public void ConvertWAVtoEWA_NikdyNepouzijeKluc0()
    {
        // INISS pri prvom bajte 'R' (kluc 0) subor nedekoduje - EWA musi mat kluc 1..255
        var wav = Encoding.ASCII.GetBytes("RIFF....WAVEfmt ");
        for (var i = 0; i < 2000; i++)
        {
            using var output = new MemoryStream();
            using (var writer = new BinaryWriter(output, Encoding.ASCII, true))
                SoundUtils.ConvertWAVtoEWA(new BinaryReader(new MemoryStream(wav)), writer);
            Assert.AreNotEqual((byte)'R', output.ToArray()[0]);
        }
    }

    [TestMethod]
    public void ConvertFiles_KonvertujeZmazePovodnyAPremenujePrvokAZvuk()
    {
        var original = File.ReadAllBytes(WriteWav("A.WAV"));
        var (group, dir) = CreateGroup();
        var sfe = dir.Children.OfType<SoundFileElement>().Single();
        var sound = new FyzSound(group, "A", "A", "A.WAV", "", "", 0) { File = sfe };
        sfe.Sound = sound;

        SoundUtils.ConvertFiles([dir], true);

        Assert.IsFalse(File.Exists(Path.Combine(_groupDir, "A.WAV")));
        Assert.IsTrue(File.Exists(Path.Combine(_groupDir, "A.EWA")));
        Assert.AreEqual("A.EWA", sfe.Name);
        Assert.AreEqual(Path.Combine(_groupDir, "A.EWA"), sfe.FileInfo.FullName);
        Assert.AreEqual("A.EWA", sound.FileName);

        // spat (ako undo akcie ConvertFilesAction)
        SoundUtils.ConvertFiles([dir], false);

        Assert.IsFalse(File.Exists(Path.Combine(_groupDir, "A.EWA")));
        Assert.AreEqual("A.WAV", sfe.Name);
        Assert.AreEqual("A.WAV", sound.FileName);
        CollectionAssert.AreEqual(original, File.ReadAllBytes(Path.Combine(_groupDir, "A.WAV")));
    }

    [TestMethod]
    public void ConvertFiles_PreskociCielovuPriponuChybajuciSuborAExistujuciCiel()
    {
        WriteWav("A.WAV");
        WriteWav("B.WAV");
        WriteWav("C.WAV");
        var existingEwa = Path.Combine(_groupDir, "C.EWA");
        File.WriteAllBytes(existingEwa, [1, 2, 3]);
        var (_, dir) = CreateGroup();
        File.Delete(Path.Combine(_groupDir, "B.WAV"));

        var result = SoundUtils.ConvertFiles(dir.Children, true);

        // Spat skonvertuje naspat len A - C.EWA uz existoval a mohol patrit inemu zvuku
        CollectionAssert.AreEqual(new[] { "A.EWA" }, result.Converted.Select(f => f.Name).ToList());
        CollectionAssert.AreEqual(new[] { "C.WAV" }, result.Skipped);
        Assert.AreEqual(0, result.Failed.Count);
        Assert.IsTrue(File.Exists(Path.Combine(_groupDir, "A.EWA")));
        Assert.IsFalse(File.Exists(Path.Combine(_groupDir, "B.EWA")));
        Assert.IsTrue(File.Exists(Path.Combine(_groupDir, "C.WAV")));
        CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, File.ReadAllBytes(existingEwa));
        var names = dir.Children.Select(c => c.Name).Order().ToList();
        CollectionAssert.AreEqual(new[] { "A.EWA", "B.WAV", "C.EWA", "C.WAV" }, names);
    }

    [TestMethod]
    public void GetDefaultFileName_PouzijeExistujuciSuborInakPriponuSkupiny()
    {
        WriteWav("9900160.wav");
        var (group, _) = CreateGroup();

        Assert.AreEqual("9900160.wav", SoundUtils.GetDefaultFileName(group, "9900160"));
        Assert.AreEqual("9900170.WAV", SoundUtils.GetDefaultFileName(group, "9900170"));

        group.Sounds.Add(new FyzSound(group, "1", "1", "1.EWA", "", "", 0));
        group.Sounds.Add(new FyzSound(group, "2", "2", "2.ewa", "", "", 0));
        group.Sounds.Add(new FyzSound(group, "3", "3", "3.WAV", "", "", 0));

        Assert.AreEqual("9900170.EWA", SoundUtils.GetDefaultFileName(group, "9900170"));
    }

    [TestMethod]
    public void FindSoundFile_HladaVPriecinkuSkupinyAleboNaPridavnejCeste()
    {
        WriteWav("A.WAV");
        Directory.CreateDirectory(Path.Combine(_bank, "SK", "CISLO"));
        File.Copy(Path.Combine(_groupDir, "A.WAV"), Path.Combine(_bank, "SK", "CISLO", "X.WAV"));
        var (group, dir) = CreateGroup();

        var found = SoundUtils.FindSoundFile(new FyzSound(group, "A", "A", "a.wav", "", "", 0), _bank);
        Assert.AreSame(dir.Children.OfType<SoundFileElement>().Single(), found);

        Assert.IsNull(SoundUtils.FindSoundFile(new FyzSound(group, "B", "B", "B.WAV", "", "", 0), _bank));

        // pridavna cesta je relativna k priecinku skupiny (ako v INISS)
        var additional = SoundUtils.FindSoundFile(new FyzSound(group, "X", "X", "X.WAV", "..\\CISLO\\", "", 0), _bank);
        Assert.IsNotNull(additional);
        Assert.AreEqual(Path.Combine(_bank, "SK", "CISLO", "X.WAV"), additional.FileInfo.FullName);
    }

    private (FyzGroup group, DirectoryElement dir) CreateGroup()
    {
        var lang = new FyzLanguage("SK", "Slovenský", "FYZZVUK.DAT", "SK\\");
        var group = new FyzGroup(lang, "R1", "Stanice", "R1\\");
        var dir = new DirectoryElement(_groupDir) { Group = group };
        group.Directory = dir;
        return (group, dir);
    }

    /// <summary>Zapise kratky 8-bitovy mono PCM WAV.</summary>
    private string WriteWav(string name)
    {
        var data = new byte[800];
        for (var i = 0; i < data.Length; i++)
            data[i] = (byte)(128 + 100 * Math.Sin(i / 5.0));

        using var ms = new MemoryStream();
        using (var w = new BinaryWriter(ms, Encoding.ASCII, true))
        {
            w.Write("RIFF"u8);
            w.Write(36 + data.Length);
            w.Write("WAVE"u8);
            w.Write("fmt "u8);
            w.Write(16);
            w.Write((short)1);    // PCM
            w.Write((short)1);    // mono
            w.Write(8000);        // vzorkovacia frekvencia
            w.Write(8000);        // bajtov za sekundu
            w.Write((short)1);    // block align
            w.Write((short)8);    // bitov na vzorku
            w.Write("data"u8);
            w.Write(data.Length);
            w.Write(data);
        }

        var path = Path.Combine(_groupDir, name);
        File.WriteAllBytes(path, ms.ToArray());
        return path;
    }
}
