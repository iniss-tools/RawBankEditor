using System.Text;
using RawBankEditor.Entities;
using RawBankEditor.Services;
using RawBankEditor.Tools;
using RawBankEditor.XML;
using ToolsCore;
using ToolsCore.Iniss.Entities;
using ToolsCore.Iniss.Tools;
using ToolsCore.Iniss2.RawBank;
using ToolsCore.XML;

namespace RawBankEditor.Tests;

/// <summary>
/// Banka INISS2: otvorenie, ukladanie bez zbytocneho prepisu, MD5 nahravok a predvoleny jazyk.
/// </summary>
[TestClass]
public sealed class BankXmlStoreTests
{
    // md5 suboru s obsahom "abc"
    private const string MD5_ABC = "900150983cd24fb0d6963f7d28e17f72";

    private string _root = null!;
    private string _bank = null!;
    private BankXmlStore _store = null!;

    [TestInitialize]
    public void Init()
    {
        _root = Path.Combine(Path.GetTempPath(), "BankXmlStoreTests_" + Guid.NewGuid().ToString("N"));
        _bank = Path.Combine(_root, "Server", "RawBank") + Path.DirectorySeparatorChar;
        Directory.CreateDirectory(_bank);
        _store = new BankXmlStore();
    }

    [TestCleanup]
    public void Cleanup() => Directory.Delete(_root, true);

    private static string Xml(string key, string defaultAttribute, string md5) => $"""
        <?xml version="1.0" encoding="UTF-8"?>
        <FYZBANK xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
          <lang {defaultAttribute}k="{key}" n="Jazyk {key}" d="{key}" date="2018-11-26T14:34:12+01:00" location="Dolné Mesto">
            <g k="R1" d="R1\">
              <z k="A" t="Dolné Mesto" f="A.EWA" l="1000" md5="{md5}" />
            </g>
          </lang>
        </FYZBANK>
        """;

    private string CreateLanguage(string key, bool isDefault = false, string md5 = "00000000000000000000000000000000", string? recording = "abc")
    {
        var dir = Path.Combine(_bank, key);
        Directory.CreateDirectory(Path.Combine(dir, "R1"));
        if (recording is not null)
            File.WriteAllText(Path.Combine(dir, "R1", "A.EWA"), recording);

        var file = Path.Combine(dir, FyzBankXmlWriter.FileName);
        File.WriteAllText(file, Xml(key, isDefault ? "default=\"true\" " : "", md5), new UTF8Encoding(true));
        return file;
    }

    private FyzLanguage Load(string key)
    {
        var language = _store.ReadLanguages(_bank).Single(l => l.Key == key);
        _store.ReadSounds(_bank, language);
        _store.RememberRecordings(_bank, language);
        return language;
    }

    [TestMethod]
    public void Load_InstalaciaIniss2PouzijeBankuXml()
    {
        CreateLanguage("SK");

        var project = RawBankProject.Load(_root);

        Assert.IsTrue(project.Store.IsIniss2);
        Assert.AreEqual(Path.Combine(_root, "Server"), project.AbsPathToINISS);
        Assert.AreEqual(_bank, project.AbsPathToBank);
        Assert.AreEqual("SK", project.Languages.Single().Key);
    }

    [TestMethod]
    public void Load_BankaSoSuboromDatOstavaInissTriNula()
    {
        var bank = Path.Combine(_root, "RAWBANK") + Path.DirectorySeparatorChar;
        Directory.CreateDirectory(bank);
        RawBankParser.WriteFyzBankFile(bank, [new FyzLanguage("SK", "Slovenčina")]);

        var project = RawBankProject.Load(_root);

        Assert.IsFalse(project.Store.IsIniss2);
        Assert.AreEqual(bank, project.AbsPathToBank);
    }

    [TestMethod]
    public void WriteSounds_BezZmenySuborNeprepise()
    {
        var file = CreateLanguage("SK");
        var original = File.ReadAllBytes(file);
        var language = Load("SK");

        _store.WriteSounds(_bank, language);

        CollectionAssert.AreEqual(original, File.ReadAllBytes(file));
    }

    [TestMethod]
    public void WriteSounds_NesuladZMinulostiOstavaAjPriZmeneInehoUdaja()
    {
        var file = CreateLanguage("SK");
        var language = Load("SK");

        language.Groups[0].Sounds[0].Text = "Horná Ves";
        _store.WriteSounds(_bank, language);

        var content = File.ReadAllText(file);
        StringAssert.Contains(content, "Horná Ves");
        StringAssert.Contains(content, "md5=\"00000000000000000000000000000000\"");
        StringAssert.Contains(content, "location=\"Dolné Mesto\"");
    }

    [TestMethod]
    public void WriteSounds_ZmenenaNahravkaDostaneNoveMd5()
    {
        var file = CreateLanguage("SK", recording: "xyz");
        var language = Load("SK");

        File.WriteAllText(Path.Combine(_bank, "SK", "R1", "A.EWA"), "abc");
        _store.WriteSounds(_bank, language);

        Assert.AreEqual(MD5_ABC, language.Groups[0].Sounds[0].Md5);
        StringAssert.Contains(File.ReadAllText(file), MD5_ABC);
    }

    [TestMethod]
    public void WriteSounds_NovyZvukDostaneMd5ZvukBezNahravkyNie()
    {
        CreateLanguage("SK");
        var language = Load("SK");
        var group = language.Groups[0];
        File.WriteAllText(Path.Combine(_bank, "SK", "R1", "B.EWA"), "abc");
        group.Sounds.Add(new FyzSound(group, "B", "B", "B.EWA", "", "", 0));
        group.Sounds.Add(new FyzSound(group, "C", "C", "C.EWA", "", "", 0));

        _store.WriteSounds(_bank, language);

        Assert.AreEqual(MD5_ABC, group.Sounds[1].Md5);
        Assert.IsNull(group.Sounds[2].Md5);
    }

    [TestMethod]
    public void WriteSounds_NazvySaZjednotiaSKlucmi()
    {
        CreateLanguage("SK");
        var language = Load("SK");
        var group = language.Groups[0];
        group.Name = "Stanice";
        group.Sounds[0].Name = "Dolné Mesto";

        _store.WriteSounds(_bank, language);

        Assert.AreEqual("R1", group.Name);
        Assert.AreEqual("A", group.Sounds[0].Name);
        Assert.IsFalse(LanguageRules.SoundsDiffer(_bank, language, _store));
    }

    [TestMethod]
    public void WriteLanguages_ZmenaPredvolenehoJazykaNezapiseNeulozeneZvuky()
    {
        CreateLanguage("CZ", true);
        var file = CreateLanguage("SK");
        var cz = Load("CZ");
        var sk = Load("SK");
        cz.IsBasic = false;
        sk.IsBasic = true;
        sk.Groups[0].Sounds[0].Text = "neulozena zmena";

        _store.WriteLanguages(_bank, [cz, sk]);

        var saved = _store.ReadLanguages(_bank);
        Assert.IsFalse(saved.Single(l => l.Key == "CZ").IsBasic);
        Assert.IsTrue(saved.Single(l => l.Key == "SK").IsBasic);
        var content = File.ReadAllText(file);
        StringAssert.Contains(content, "Dolné Mesto");
        Assert.IsFalse(content.Contains("neulozena zmena", StringComparison.Ordinal));
    }

    [TestMethod]
    public void WriteLanguages_NenacitanyJazykSoZmenenymNazvomZachovaZvuky()
    {
        var file = CreateLanguage("SK");
        var language = _store.ReadLanguages(_bank).Single();
        language.Name = "Slovenčina";

        _store.WriteLanguages(_bank, [language]);

        var content = File.ReadAllText(file);
        StringAssert.Contains(content, "n=\"Slovenčina\"");
        StringAssert.Contains(content, "t=\"Dolné Mesto\"");
        StringAssert.Contains(content, "date=\"2018-11-26T14:34:12+01:00\"");
    }

    [TestMethod]
    public void RemovedLanguageFiles_JazykOdstranenyZoZoznamu()
    {
        CreateLanguage("CZ");
        var file = CreateLanguage("SK");
        var languages = _store.ReadLanguages(_bank).Where(l => l.Key == "CZ").ToList();

        var removed = _store.RemovedLanguageFiles(_bank, languages).ToList();

        Assert.AreEqual(file, removed.Single());
    }

    [TestMethod]
    public void BankDiffers_PoradieJazykovNevadiPredvolenyJazykAno()
    {
        CreateLanguage("SK", true);
        CreateLanguage("CZ");
        var languages = _store.ReadLanguages(_bank);
        languages.Reverse();

        Assert.IsFalse(LanguageRules.BankDiffers(_bank, languages, _store));

        languages.ForEach(l => l.IsBasic = l.Key == "CZ");
        Assert.IsTrue(LanguageRules.BankDiffers(_bank, languages, _store));
    }

    [TestMethod]
    public void BankDiffers_PriecinkySPodobnymNazvomSaPorovnajuSpravne()
    {
        CreateLanguage("CZ");
        CreateLanguage("CZ2");

        Assert.IsFalse(LanguageRules.BankDiffers(_bank, _store.ReadLanguages(_bank), _store));
    }

    [TestMethod]
    [DataRow(1, 0, false)]
    [DataRow(2, 1, false)]
    [DataRow(2, 0, true)]
    [DataRow(2, 2, true)]
    public void DefaultLanguageProblem_PriViacerychJazykochPraveJeden(int count, int defaults, bool problem)
    {
        var languages = Enumerable.Range(0, count).Select(i => new FyzLanguage("L" + i, "L" + i) { IsBasic = i < defaults }).ToList();

        Assert.AreEqual(problem, LanguageRules.DefaultLanguageProblem(languages) is not null);
    }

    [TestMethod]
    public void NameIsKey_LenJazykBankyIniss2()
    {
        Assert.IsFalse(LanguageRules.NameIsKey(new FyzLanguage("SK", "Slovenčina")));
        Assert.IsTrue(LanguageRules.NameIsKey(new FyzLanguage("SK", "Slovenčina", FyzBankXmlWriter.FileName, "SK\\")));
    }

    [TestMethod]
    public void BankEditor_BezPredvolenehoJazykaBankuNeulozi()
    {
        var file = CreateLanguage("CZ");
        CreateLanguage("SK");
        var original = File.ReadAllBytes(file);
        var editor = Editor(out var project);
        var language = project.Languages[0];
        _store.ReadSounds(_bank, language);
        language.Groups[0].Sounds[0].Text = "zmena";

        var result = editor.Save(false, language);

        Assert.IsFalse(result.Saved);
        Assert.IsFalse(string.IsNullOrEmpty(result.Error));
        CollectionAssert.AreEqual(original, File.ReadAllBytes(file));
        editor.Close(true);
    }

    [TestMethod]
    public void BankEditor_UlozenieZapiseLenZmenenyJazyk()
    {
        var cz = CreateLanguage("CZ", true);
        var sk = CreateLanguage("SK");
        var originalSk = File.ReadAllBytes(sk);
        var editor = Editor(out var project);
        var language = project.Languages.Single(l => l.Key == "CZ");
        project.Store.ReadSounds(_bank, language);
        project.Store.RememberRecordings(_bank, language);
        language.Groups[0].Sounds[0].Text = "Horná Ves";

        var result = editor.Save(false, language);

        Assert.IsTrue(result.Saved, result.Error);
        StringAssert.Contains(File.ReadAllText(cz), "Horná Ves");
        CollectionAssert.AreEqual(originalSk, File.ReadAllBytes(sk));
        Assert.IsFalse(File.Exists(Path.Combine(_bank, RawBankParser.FileFyzbank)), "INISS2 nema spolocny subor jazykov");
        Assert.IsFalse(editor.LanguageListChanged());
        Assert.IsFalse(editor.SoundsChanged(language));
        editor.Close(false);
    }

    [TestMethod]
    public void BankEditor_NovyJazykZapiseSuborJazyka()
    {
        CreateLanguage("CZ", true);
        var editor = Editor(out var project);
        var language = new FyzLanguage("SK", "Slovenčina", project.Store.LanguageFileName, "SK\\") { Groups = [] };
        project.Languages.Add(language);
        editor.MarkNew(language);

        var result = editor.Save(false, null);

        Assert.IsTrue(result.Saved, result.Error);
        Assert.IsTrue(File.Exists(Path.Combine(_bank, "SK", FyzBankXmlWriter.FileName)));
        Assert.IsFalse(editor.LanguageListChanged());
        editor.Close(false);
    }

    [TestMethod]
    public async Task RecordingCheck_NajdeNesuladAVynechaZvukBezNahravky()
    {
        CreateLanguage("SK", md5: MD5_ABC);
        var language = Load("SK");
        var group = language.Groups[0];
        var same = group.Sounds[0];
        same.File = new SoundFileElement(same.GetAbsPath(_bank));

        var changedPath = Path.Combine(_bank, "SK", "R1", "B.WAV");
        File.WriteAllBytes(changedPath, Wav(2205));
        var changed = new FyzSound(group, "B", "B", "B.WAV", "", "", 5) { Md5 = "stare", File = new SoundFileElement(changedPath) };
        var missing = new FyzSound(group, "C", "C", "C.EWA", "", "", 5);

        var mismatches = await RecordingCheck.FindAsync([same, changed, missing]);

        var mismatch = mismatches.Single();
        Assert.AreSame(changed, mismatch.Sound);
        Assert.AreEqual(FyzBankXmlWriter.FileMd5(changedPath), mismatch.Md5);
        Assert.AreEqual(100, mismatch.Duration);
    }

    private BankEditor Editor(out RawBankProject project)
    {
        project = RawBankProject.Load(_root);
        var editor = new BankEditor(new AppSession<RawBankEditorConfig, RawBankEditorStyle>(new RawBankEditorConfig(), new Styles<RawBankEditorStyle>(),
            new RawBankEditorStyle()));
        editor.Open(project);
        return editor;
    }

    // PCM 16 bit mono 22 050 Hz
    private static byte[] Wav(int samples)
    {
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream);
        writer.Write("RIFF"u8);
        writer.Write(36 + samples * 2);
        writer.Write("WAVEfmt "u8);
        writer.Write(16);
        writer.Write((short)1);
        writer.Write((short)1);
        writer.Write(22050);
        writer.Write(44100);
        writer.Write((short)2);
        writer.Write((short)16);
        writer.Write("data"u8);
        writer.Write(samples * 2);
        writer.Write(new byte[samples * 2]);
        writer.Flush();
        return stream.ToArray();
    }
}
