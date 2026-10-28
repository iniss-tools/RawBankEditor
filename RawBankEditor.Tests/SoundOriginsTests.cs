using System.Text.Json.Nodes;
using RawBankEditor.Entities;
using RawBankEditor.Services;
using RawBankEditor.Speech;
using RawBankEditor.Tools;
using RawBankEditor.XML;
using ToolsCore;
using ToolsCore.Iniss.Entities;
using ToolsCore.Iniss.Tools;
using ToolsCore.XML;

namespace RawBankEditor.Tests;

/// <summary>
/// Zaznamy povodu nahravok (.rbe-ai) a doplnenie chybajucich nahravok: novy subor, Spat, zahodenie zmien a ulozenie
/// zaznamov s bankou.
/// </summary>
[TestClass]
public sealed class SoundOriginsTests : IDisposable
{
    private string _root = null!;
    private string _bank = null!;
    private BankJournal _journal = null!;
    private SoundVersions _versions = null!;
    private FyzLanguage _language = null!;
    private FyzGroup _group = null!;

    [TestInitialize]
    public void Init()
    {
        _root = Directory.CreateTempSubdirectory("rbeorigins").FullName;
        _bank = Path.Combine(_root, "RAWBANK") + Path.DirectorySeparatorChar;
        Directory.CreateDirectory(Path.Combine(_bank, "SK", "R1"));
        _journal = new BankJournal(Path.Combine(_root, "_zurnal"), path => File.Delete(path), _ => false);
        _versions = new SoundVersions(Path.Combine(_root, "_verzie"));

        _language = new FyzLanguage("SK", "Slovenčina") { Groups = new List<FyzGroup>() };
        _group = new FyzGroup(_language, "R1", "R1", "R1\\") { Directory = new DirectoryElement(Path.Combine(_bank, "SK", "R1")) };
        _language.Groups.Add(_group);
    }

    [TestCleanup]
    public void Cleanup() => Directory.Delete(_root, true);

    public void Dispose() => _versions.Dispose();

    private FyzSound AddSound(string key, string fileName)
    {
        var sound = new FyzSound(_group, key, key, fileName, "", key, 0);
        _group.Sounds.Add(sound);
        return sound;
    }

    private string Wav(float amplitude, int samples = 2205) =>
        _versions.Add(SoundProcessing.ToWav(new PcmSound(Enumerable.Repeat(amplitude, samples).ToArray(), SoundProcessing.BankSampleRate)));

    [TestMethod]
    public void CreateMissing_VytvoriSuborPrepojiZvukASpatHoOdstrani()
    {
        var sound = AddSound("9900100", "9900100.EWA");
        var change = RecordingChange.CreateMissing(sound, _bank, Wav(0.2f), 100, true);

        RecordingReplace.Apply([change], true, _journal, "SK");
        RecordingReplace.Link([change], true, null);

        var path = Path.Combine(_bank, "SK", "R1", "9900100.EWA");
        Assert.IsTrue(File.Exists(path));
        Assert.AreNotEqual((byte)'R', File.ReadAllBytes(path)[0]);
        Assert.AreSame(change.File, sound.File);
        Assert.Contains(change.File, _group.Directory.Children);
        Assert.AreEqual(100, sound.Duration);

        RecordingReplace.Apply([change], false, _journal, "SK");
        RecordingReplace.Link([change], false, null);

        Assert.IsFalse(File.Exists(path));
        Assert.IsNull(sound.File);
        Assert.DoesNotContain(change.File, _group.Directory.Children);
        Assert.AreEqual(0, sound.Duration);
    }

    [TestMethod]
    public void CreateMissing_PrazdnyNazovSuboru_PredvolenyAVratiSa()
    {
        var sound = AddSound("ABC", "");
        var change = RecordingChange.CreateMissing(sound, _bank, Wav(0.2f), 100, false);

        RecordingReplace.Apply([change], true, _journal, "SK");
        RecordingReplace.Link([change], true, null);
        Assert.AreEqual("ABC.WAV", sound.FileName);
        Assert.AreEqual(0, sound.Duration);

        RecordingReplace.Apply([change], false, _journal, "SK");
        RecordingReplace.Link([change], false, null);
        Assert.AreEqual("", sound.FileName);
    }

    [TestMethod]
    public void CreateMissing_ZahodenieZmien_SuborZmaze()
    {
        var sound = AddSound("X", "X.WAV");
        var change = RecordingChange.CreateMissing(sound, _bank, Wav(0.2f), 100, true);
        RecordingReplace.Apply([change], true, _journal, "SK");

        Assert.IsEmpty(_journal.Rollback());
        Assert.IsFalse(File.Exists(change.File.FileInfo.FullName));
    }

    [TestMethod]
    public void CreateMissing_ChybajuciPriecinok_Vynimka()
    {
        var sound = new FyzSound(new FyzGroup(_language, "N5", "N5", "N5\\"), "01", "01", "01.WAV", "", "", 0);

        Assert.ThrowsExactly<DirectoryNotFoundException>(() => RecordingChange.CreateMissing(sound, _bank, Wav(0.1f), 100, true));
    }

    [TestMethod]
    public void Link_ZaznamyPovodu_SpatVratiPovodnyAUpravaHoNemeni()
    {
        var sound = AddSound("A", "A.WAV");
        var path = Path.Combine(_bank, "SK", "R1", "A.WAV");
        File.WriteAllBytes(path, File.ReadAllBytes(Wav(0.1f)));
        var file = new SoundFileElement(path) { Duration = 100, Sound = sound };
        sound.File = file;
        var origins = new SoundRecords(null, null, "SK");
        var profile = new VoiceProfile { VoiceId = "v", VoiceName = "Zuzana" };
        var generated = SoundOrigin.Generated(profile, "A", 5, "f1", DateTimeOffset.Now);

        var change = RecordingChange.Replace(file, Wav(0.3f), 100, _versions, true) with { NewOrigin = generated };
        RecordingReplace.Apply([change], true, _journal, "SK");
        RecordingReplace.Link([change], true, origins);
        Assert.AreSame(generated, origins.GetOrigin(sound));
        Assert.IsTrue(origins.Changed);

        // uprava zvuku vygenerovanej nahravky povod nemeni
        var adjust = RecordingChange.Replace(file, Wav(0.2f), 100, _versions, true);
        RecordingReplace.Link([adjust], true, origins);
        Assert.AreSame(generated, origins.GetOrigin(sound));

        RecordingReplace.Link([change], false, origins);
        Assert.IsNull(origins.GetOrigin(sound));
    }

    [TestMethod]
    public void SoundOrigins_KlucBezVelkostiPismenAUlozenieBezOdstranenychZvukov()
    {
        var sound = AddSound("Abc", "Abc.WAV");
        var gone = new FyzSound(_group, "Zmazany", "Zmazany", "Z.WAV", "", "", 0);
        var origins = new SoundRecords(new Dictionary<string, SoundOrigin> { ["r1/ABC"] = new() { VoiceName = "Adam" } }, null, "SK");
        origins.SetOrigin(gone, new SoundOrigin());

        Assert.AreEqual("Adam", origins.GetOrigin(sound)!.VoiceName);
        CollectionAssert.AreEqual(new[] { "r1/ABC" }, origins.OriginsForSave(_language).Keys.ToList());
    }

    [TestMethod]
    public void SoundOrigin_SameAs_PodlaOdtlacku()
    {
        var origin = SoundOrigin.Generated(new VoiceProfile(), "Lipová", null, "abc", DateTimeOffset.Now);

        Assert.IsTrue(origin.SameAs("abc"));
        Assert.IsFalse(origin.SameAs("abd"));
        Assert.IsFalse(new SoundOrigin().SameAs(""));
    }

    [TestMethod]
    public void SoundRecords_TextPreSyntezu_RovnakyAkoTextZvukuSaNeuklada()
    {
        var sound = AddSound("A", "A.WAV");
        var records = new SoundRecords(null, null, "SK");

        records.SetText(sound, " A ");
        Assert.IsNull(records.GetText(sound));
        Assert.IsFalse(records.Changed);

        records.SetText(sound, "Á-čko");
        Assert.AreEqual("Á-čko", records.SpokenText(sound));
        Assert.IsTrue(records.Changed);

        records.SetText(sound, "");
        Assert.AreEqual("A", records.SpokenText(sound));
    }

    [TestMethod]
    public void Link_TextPreSyntezu_SpatVratiPovodny()
    {
        var sound = AddSound("B", "B.WAV");
        var path = Path.Combine(_bank, "SK", "R1", "B.WAV");
        File.WriteAllBytes(path, File.ReadAllBytes(Wav(0.1f)));
        var file = new SoundFileElement(path) { Duration = 100, Sound = sound };
        sound.File = file;
        var records = new SoundRecords(null, null, "SK");
        records.SetText(sound, "Bé");

        var change = RecordingChange.Replace(file, Wav(0.3f), 100, _versions, true) with { OldText = "Bé", NewText = "Béčko" };
        RecordingReplace.Link([change], true, records);
        Assert.AreEqual("Béčko", records.GetText(sound));

        RecordingReplace.Link([change], false, records);
        Assert.AreEqual("Bé", records.GetText(sound));
    }

    [TestMethod]
    public void BankAiFolder_HlasAZaznamySaUkladajuOddelene()
    {
        BankAiFolder.SaveRecords(_root, "SK", "SK", new Dictionary<string, SoundOrigin> { ["R1/A"] = new() { VoiceName = "Adam" } }, []);
        BankAiFolder.SaveVoice(_root, "SK", new VoiceProfile { VoiceId = "v" });

        var data = BankAiFolder.Load(_root, "SK");
        Assert.AreEqual("v", data.Voice!.VoiceId);
        Assert.AreEqual("Adam", data.Sounds["R1/A"].VoiceName);

        BankAiFolder.Update(_root, "SK", d => d.Pronunciation = [new PronunciationRule { Find = "IC", Replace = "í cé" }]);
        BankAiFolder.SaveRecords(_root, "SK", "SK", [], []);
        var reloaded = BankAiFolder.Load(_root, "SK");
        Assert.AreEqual("v", reloaded.Voice!.VoiceId);
        Assert.AreEqual("í cé", reloaded.Pronunciation.Single().Replace);
        Assert.IsEmpty(reloaded.Sounds);
    }

    [TestMethod]
    public void BankAiFolder_PremenovanyJazyk_PresunieHlas()
    {
        BankAiFolder.SaveVoice(_root, "SK", new VoiceProfile { VoiceId = "v" });

        BankAiFolder.SaveRecords(_root, "SVK", "SK", [], []);

        Assert.IsFalse(File.Exists(BankAiFolder.FileOf(_root, "SK")));
        Assert.AreEqual("v", BankAiFolder.Load(_root, "SVK").Voice!.VoiceId);
    }

    [TestMethod]
    public void BankEditor_UlozenieBanky_ZapiseZaznamyPovodu()
    {
        AddSound("A", "A.WAV");
        RawBankParser.WriteFyzBankFile(_bank, [_language]);
        RawBankParser.WriteFyzZvukFile(_bank, _language);
        var project = RawBankProject.Load(_root);
        var language = project.Languages.Single();
        RawBankParser.ReadFyzZvukFile(_bank, language);
        var editor = new BankEditor(new AppSession<RawBankEditorConfig, RawBankEditorStyle>(new RawBankEditorConfig(), new Styles<RawBankEditorStyle>(),
            new RawBankEditorStyle()));
        editor.Open(project);

        editor.Records(language).SetOrigin(language.Groups[0].Sounds[0], new SoundOrigin { VoiceName = "Zuzana" });
        editor.Records(language).SetText(language.Groups[0].Sounds[0], "Á");
        var result = editor.Save(false, language);

        Assert.IsTrue(result.Saved, result.Error);
        var json = JsonNode.Parse(File.ReadAllText(BankAiFolder.FileOf(_root, "SK")))!;
        Assert.AreEqual("Zuzana", (string)json["sounds"]!["R1/A"]!["voiceName"]!);
        Assert.AreEqual("Á", (string)json["texts"]!["R1/A"]!);
        Assert.IsFalse(editor.Records(language).Changed);
        editor.Close(false);
    }
}
