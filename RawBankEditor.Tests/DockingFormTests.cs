using System.Reflection;
using System.Windows.Forms;
using ExControls;
using RawBankEditor.Forms;
using RawBankEditor.Services;
using RawBankEditor.XML;
using ToolsCore;
using ToolsCore.Commands;
using ToolsCore.Tools;
using ToolsCore.XML;

namespace RawBankEditor.Tests;

/// <summary>
/// Ukotvitelne panely hlavneho okna bez zobrazenia: skupiny a prieskumnik vlavo, zvuky v dokumente, zoznam chyb dole,
/// obnovenie rozlozenia z nastaveni a navrat k predvolenemu.
/// </summary>
[TestClass]
public sealed class DockingFormTests
{
    private const BindingFlags Any = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private static void Sta(Action test)
    {
        Exception? failure = null;
        var thread = new Thread(() =>
        {
            try
            {
                test();
            }
            catch (Exception e)
            {
                failure = e;
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
        if (failure is not null)
            throw new AssertFailedException(failure.ToString());
    }

    private static FMain MainWindow(string? layout = null)
    {
        var styles = Styles<RawBankEditorStyle>.CreateDefault();
        var session = new AppSession<RawBankEditorConfig, RawBankEditorStyle>(new RawBankEditorConfig { DockLayout = layout, WrapSoundText = false }, styles,
            styles.StyleList[0]);
        // zalamovanie textu ako v navrhu okna - jeho zmena by zapisala config.xml programu
        // inicializator vlastnosti obide setter - tema okien cita styl z GlobSettings
        session.UsingStyle = styles.StyleList[0];
        var form = new FMain(new BankEditor(session), new DialogService());
        form.CreateControl();
        return form;
    }

    private static T Field<T>(Control control, string name) => (T)control.GetType().GetField(name, Any)!.GetValue(control)!;

    private static ExDockManager Dock(FMain form) => Field<ExDockManager>(form, "_dock");

    private static ExDockPanel Panel(FMain form, string key) => Dock(form).Panels.Single(panel => panel.Key == key);

    [TestMethod]
    public void HlavneOkno_PanelyAZvukyVDokumente() => Sta(() =>
    {
        using var form = MainWindow();

        CollectionAssert.AreEqual(new[] { "Groups", "Explorer", "Errors" }, Dock(form).Panels.Select(panel => panel.Key).ToArray());
        Assert.AreSame(Panel(form, "Groups"), Field<DataGridView>(form, "dgvGroups").Parent);
        Assert.AreSame(Panel(form, "Explorer"), Field<ToolStrip>(form, "toolStripExplorer").Parent);
        Assert.AreSame(Panel(form, "Errors"), Field<DataGridView>(form, "dgvErrors").Parent);
        Assert.AreSame(Dock(form).Document, Field<DataGridView>(form, "dgvSounds").Parent);
        Assert.AreEqual((ExDockSide.Left, ExDockSide.Left, ExDockSide.Bottom),
            (Panel(form, "Groups").Side, Panel(form, "Explorer").Side, Panel(form, "Errors").Side));
        Assert.IsTrue(Field<SplitContainer>(form, "splitContainer1").IsDisposed, "deliace listy navrhu sa zahodia");
    });

    [TestMethod]
    public void Rozlozenie_SkrytyZoznamChybSaObnoviAPrikazVratiPredvolene() => Sta(() =>
    {
        string layout;
        using (var first = MainWindow())
        {
            Dock(first).HidePanel(Panel(first, "Errors"));
            layout = Dock(first).SaveLayout();
        }

        using var form = MainWindow(layout);
        Assert.IsFalse(Panel(form, "Errors").IsShown);

        Field<CommandSet>(form, "_commands")["ShowErrors"].Execute();
        Assert.IsTrue(Panel(form, "Errors").IsShown);

        Dock(form).HidePanel(Panel(form, "Groups"));
        Field<CommandSet>(form, "_commands")["ResetLayout"].Execute();
        Assert.IsTrue(Dock(form).Panels.All(panel => panel.IsShown));
    });
}
