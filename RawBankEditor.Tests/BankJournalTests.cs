using RawBankEditor.Tools;

namespace RawBankEditor.Tests;

/// <summary>
/// Zurnal zmien na disku: zahodenie zmien vrati disk do stavu posledneho ulozenia, ulozenie posle odstranene
/// polozky do Kosa.
/// </summary>
[TestClass]
public class BankJournalTests
{
    private string _root = null!;
    private string _bin = null!;
    private BankJournal _journal = null!;
    private readonly List<string> _recycled = [];

    [TestInitialize]
    public void Init()
    {
        _root = Directory.CreateTempSubdirectory("rbejournal").FullName;
        _bin = Path.Combine(_root, "_kos");
        Directory.CreateDirectory(_bin);
        Directory.CreateDirectory(Path.Combine(_root, "SK", "A"));
        Directory.CreateDirectory(Path.Combine(_root, "SK", "B"));
        File.WriteAllText(P("SK", "A", "x.wav"), "x");
        File.WriteAllText(P("SK", "A", "y.wav"), "y");
        _journal = new BankJournal(Path.Combine(_root, "_zurnal"), Recycle, RestoreFromBin);
    }

    [TestCleanup]
    public void Cleanup() => Directory.Delete(_root, true);

    private string P(params string[] parts) => Path.Combine([_root, .. parts]);

    // Kos: presun do _kos pod plnym nazvom, obnova podla povodnej cesty
    private void Recycle(string path)
    {
        _recycled.Add(path);
        var target = Path.Combine(_bin, Path.GetFileName(path));
        if (Directory.Exists(path)) Directory.Move(path, target);
        else File.Move(path, target);
    }

    private bool RestoreFromBin(string path)
    {
        var inBin = Path.Combine(_bin, Path.GetFileName(path));
        if (!File.Exists(inBin)) return false;
        File.Move(inBin, path);
        return true;
    }

    [TestMethod]
    public void Zahodenie_PresunASubor_VratiNaPovodneMiesto()
    {
        _journal.Move(P("SK", "A", "x.wav"), P("SK", "B", "x.wav"), "SK");
        _journal.Move(P("SK", "A", "y.wav"), P("SK", "A", "z.wav"), "SK");

        var errors = _journal.Rollback();

        Assert.IsEmpty(errors);
        Assert.IsTrue(File.Exists(P("SK", "A", "x.wav")));
        Assert.IsTrue(File.Exists(P("SK", "A", "y.wav")));
        Assert.IsFalse(File.Exists(P("SK", "B", "x.wav")));
        Assert.IsFalse(_journal.HasChanges);
    }

    [TestMethod]
    public void Odstranenie_NejdeDoKosaAzPriUlozeni()
    {
        _journal.Delete(P("SK", "A", "x.wav"), "SK");

        Assert.IsFalse(File.Exists(P("SK", "A", "x.wav")));
        Assert.IsEmpty(_recycled);

        var errors = _journal.Commit();

        Assert.IsEmpty(errors);
        Assert.HasCount(1, _recycled);
        Assert.IsTrue(File.Exists(Path.Combine(_bin, "x.wav")));
        Assert.IsFalse(Directory.Exists(_journal.BackupRoot));
        Assert.IsFalse(_journal.HasChanges);
    }

    [TestMethod]
    public void Odstranenie_ZahodenieVratiSuborAjPriecinok()
    {
        _journal.Delete(P("SK", "A", "x.wav"), "SK");
        _journal.Delete(P("SK", "B"), null);

        _journal.Rollback();

        Assert.AreEqual("x", File.ReadAllText(P("SK", "A", "x.wav")));
        Assert.IsTrue(Directory.Exists(P("SK", "B")));
        Assert.IsFalse(Directory.Exists(_journal.BackupRoot));
    }

    [TestMethod]
    public void NovyPriecinok_ZahodenieHoZmazeAzPoVrateniObsahu()
    {
        Assert.IsTrue(_journal.CreateDirectory(P("SK", "C"), "SK"));
        Assert.IsFalse(_journal.CreateDirectory(P("SK", "C"), "SK"));
        _journal.Move(P("SK", "A", "x.wav"), P("SK", "C", "x.wav"), "SK");

        _journal.Rollback();

        Assert.IsFalse(Directory.Exists(P("SK", "C")));
        Assert.IsTrue(File.Exists(P("SK", "A", "x.wav")));
    }

    [TestMethod]
    public void ZahodenieJazyka_PrepocitaCestyCezPremenovanyPriecinokJazyka()
    {
        // zmena v jazyku SK, potom premenovanie priecinka jazyka (zmena zoznamu jazykov ostava)
        _journal.Move(P("SK", "A", "x.wav"), P("SK", "B", "x.wav"), "SK");
        _journal.Move(P("SK"), P("SK2"), null);

        _journal.Rollback("SK");

        Assert.IsTrue(Directory.Exists(P("SK2")));
        Assert.IsTrue(File.Exists(P("SK2", "A", "x.wav")));
        Assert.IsFalse(File.Exists(P("SK2", "B", "x.wav")));
        Assert.IsTrue(_journal.HasChanges);

        _journal.Rollback();
        Assert.IsTrue(File.Exists(P("SK", "A", "x.wav")));
    }

    [TestMethod]
    public void Obnova_PoOdstraneniAZahodeni_SuborOstane()
    {
        // odstranenie a spat, potom zahodenie - subor bol pred zmenami, ma ostat
        _journal.Delete(P("SK", "A", "x.wav"), "SK");
        Assert.IsTrue(_journal.Restore(P("SK", "A", "x.wav"), "SK"));
        Assert.IsTrue(File.Exists(P("SK", "A", "x.wav")));

        _journal.Rollback();

        Assert.IsTrue(File.Exists(P("SK", "A", "x.wav")));
    }

    [TestMethod]
    public void Obnova_PoUlozeni_ZKosa()
    {
        _journal.Delete(P("SK", "A", "x.wav"), "SK");
        _journal.Commit();

        Assert.IsTrue(_journal.Restore(P("SK", "A", "x.wav"), "SK"));
        Assert.IsTrue(File.Exists(P("SK", "A", "x.wav")));

        // ulozeny stav subor nema - zahodenie ho vrati do Kosa
        _journal.Rollback();
        Assert.IsFalse(File.Exists(P("SK", "A", "x.wav")));
        Assert.HasCount(2, _recycled);
    }

    [TestMethod]
    public void Odstranenie_NeuspesnyKos_DalsieOdstranenieNeprepiseZalohu()
    {
        _journal.Delete(P("SK", "A", "x.wav"), "SK");
        // polozka ostala v zalohe (napr. Kos zlyhal) a novy zurnal zacina cislovat od zaciatku
        var journal = new BankJournal(_journal.BackupRoot, Recycle, RestoreFromBin);
        journal.Delete(P("SK", "A", "y.wav"), "SK");

        journal.Commit();

        Assert.IsTrue(File.Exists(Path.Combine(_bin, "x.wav")));
        Assert.IsTrue(File.Exists(Path.Combine(_bin, "y.wav")));
        Assert.IsFalse(Directory.Exists(_journal.BackupRoot));
    }

    [TestMethod]
    public void Konverzia_ZahodenieVratiPovodnySuborAZmazeNovy()
    {
        File.WriteAllText(P("SK", "A", "x.ewa"), "e");
        _journal.FileCreated(P("SK", "A", "x.ewa"), "SK");
        _journal.Delete(P("SK", "A", "x.wav"), "SK");

        _journal.Rollback();

        Assert.IsTrue(File.Exists(P("SK", "A", "x.wav")));
        Assert.IsFalse(File.Exists(P("SK", "A", "x.ewa")));
    }

    [TestMethod]
    public void PrazdnyPriecinok_OdstranenieAZahodenie()
    {
        _journal.RemoveEmptyDirectory(P("SK", "B"), "SK");
        Assert.IsFalse(Directory.Exists(P("SK", "B")));

        _journal.Rollback();

        Assert.IsTrue(Directory.Exists(P("SK", "B")));
    }
}
