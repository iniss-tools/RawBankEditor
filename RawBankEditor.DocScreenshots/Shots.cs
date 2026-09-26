using System.Globalization;
using System.Reflection;
using RawBankEditor.Forms;
using ToolsCore.Entities;

namespace RawBankEditor.DocScreenshots;

/// <summary>
///     Zoznam snímok. Každá snímka je okno uložené ako <c>&lt;priečinok&gt;/&lt;názov&gt;-light.png</c> a <c>-dark.png</c>;
///     priečinok je názov článku v <c>docs/rawbankeditor</c>.
/// </summary>
internal sealed class Shots(Program.Options options, string theme, List<string> log)
{
    private const BindingFlags Any = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    private int _count;

    public int Run(string installDir)
    {
        log.Add($"krok: otvorenie hlavného okna ({theme})");

        // banka má dva jazyky - pri otvorení sa pýta, ktorý načítať
        var main = Program.OpenMain(installDir, choose =>
        {
            Save("otvorenie-banky/vyber-jazyka", choose);
            Field<ComboBox>(choose, "cboxLanguages").SelectedIndex = 0;
            Invoke(choose, "bOK_Click");
        });

        try
        {
            var language = main.CurrentLanguage!;
            var stations = language.Groups.First(g => g.Key == "R1");
            LogMessages(main);

            // hlavné okno so skupinou staníc a vybranou stanicou Dolné Mesto
            Shot("hlavne-okno/hlavne-okno", main, _ =>
            {
                SelectGroup(main, stations);
                SelectSound(main, stations.Sounds.First(s => s.Key == "9900100"));
            }, dispose: false);

            // zoznam slov so zvukom bez súboru - bunka Názov súboru má značku chyby
            var words = language.Groups.First(g => g.Key == "Slova");
            Shot("uprava-zvukov/zoznam-zvukov", main, _ =>
            {
                SelectGroup(main, words);
                SelectSound(main, words.Sounds.First(s => s.Key == DemoBank.MissingFileKey));
            }, dispose: false);

            Shot("pridanie-zvukov/pridat-zvuk", () => new FAddSound(stations), form =>
            {
                Field<TextBox>(form, "tbKey").Text = "9900160";
                Field<RichTextBox>(form, "rtbText").Text = "Horná Ves";
                Unselect(form);
            });

            // súbor nakopírovaný do priečinka banky - okno sa otvorí samo, ak je zapnuté automatické vkladanie
            Shot("pridanie-zvukov/novopridane-subory", () =>
            {
                var sound = new FyzSound(stations, "9900150", "9900150", DemoBank.UndefinedFile, "", "Horná Ves", 900);
                var form = (Form)Activator.CreateInstance(typeof(FAfterInsertSounds), Any, null, [sound], CultureInfo.InvariantCulture)!;
                // druhý súbor skopírovaný naraz s prvým - pribudne do toho istého okna
                var sounds = (System.Collections.IList)typeof(FAfterInsertSounds).GetProperty("NewSounds", Any)!.GetValue(form)!;
                sounds.Add(new FyzSound(stations, "9900160", "9900160", "9900160.WAV", "", "", 1100));
                return form;
            });

            Shot("presun-zvukov/vyber-skupiny", () => new FSoundsMove(language.Groups, stations));

            Shot("hladanie/hladanie", () => new FSearch(), form =>
            {
                Field<TextBox>(form, "tbText").Text = "Dolné";
                Field<RadioButton>(form, "rbText").Checked = true;
                Unselect(form);
            });

            Shot("skupiny-zvukov/pridanie-skupiny", () => new FAddEditGroup());
            Shot("skupiny-zvukov/uprava-skupiny", () => new FAddEditGroup(stations), Unselect);

            Shot("jazyky/pridanie-jazyka", () => new FAddEditLanguage(GlobData.OpenedProject!.Languages));
            Shot("jazyky/uprava-jazyka", () => new FAddEditLanguage(GlobData.OpenedProject!.Languages, language), Unselect);

            // stránky nastavení programu - rovnaké ako v GVDEditore, stránka Všeobecné má navyše skupinu Program
            foreach (var (slug, panel) in new[]
                     {
                         ("nastavenia-programu", ""),
                         ("komponenty", "pDesktopComponents"),
                         ("stlpce", "pDesktopColumns"),
                         ("lokalizacia", "pLocalization"),
                         ("klavesove-skratky", "pShortcuts"),
                         ("styly", "pStyles"),
                         ("pisma", "pFonts"),
                         ("logovanie", "pLogging"),
                     })
            {
                Shot($"nastavenia-programu/{slug}", () =>
                {
                    var form = new FAppSettings(GlobData.Config, GlobData.Styles);
                    if (panel.Length > 0)
                        form.PreselectMenuItem(panel);
                    return form;
                });
            }
        }
        finally
        {
            // Dispose namiesto Close - Close by sa pri neuloženej banke pýtal na uloženie
            main.Dispose();
            GlobData.OpenedProject = null;
        }

        return _count;
    }

    private void Shot(string name, Func<Form> create, Action<Form>? setup = null) => Shot(name, create(), setup);

    private void Shot(string name, Form form, Action<Form>? setup = null, bool dispose = true)
    {
        log.Add($"krok: {name} ({theme})");
        try
        {
            if (!form.Visible)
            {
                form.StartPosition = FormStartPosition.Manual;
                form.Location = new Point(60, 60);
                form.ShowInTaskbar = false;
                form.Show();
            }

            Pump.Events();
            setup?.Invoke(form);
            Pump.Events();
            form.Refresh();
            Pump.Events();
            Save(name, form);
        }
        catch (Exception e)
        {
            log.Add($"{name} ({theme}): {e.GetType().Name}: {(e is TargetInvocationException { InnerException: { } inner } ? inner.Message : e.Message)}");
        }
        finally
        {
            if (dispose)
                form.Dispose();
        }
    }

    private void Save(string name, Form form)
    {
        if (options.Only is not null && !name.Contains(options.Only, StringComparison.OrdinalIgnoreCase))
            return;

        var file = Path.Combine(options.OutDir, $"{name}-{theme}.png".Replace('/', Path.DirectorySeparatorChar));
        WindowCapture.Save(form, file);
        _count++;
    }

    /// <summary>
    ///     Správy zoznamu chýb po načítaní banky - ukážková banka má mať práve tri zámerné.
    /// </summary>
    private void LogMessages(FMain main)
    {
        if (theme != "light")
            return;

        foreach (var message in GlobData.OpenedProject!.Messages[main.CurrentLanguage!])
            log.Add($"  zoznam chýb: {message.Code}: {message.Message}");
    }

    /// <summary>
    ///     Výber skupiny jedným krokom ako kliknutím - každá zmena výberu skupinu znova otvára.
    /// </summary>
    private static void SelectGroup(FMain main, FyzGroup group)
    {
        var grid = Field<DataGridView>(main, "dgvGroups");
        var row = grid.Rows.Cast<DataGridViewRow>().First(r => r.DataBoundItem == group);
        grid.CurrentCell = row.Cells.Cast<DataGridViewCell>().First(c => c.Visible);
        Pump.Events();
    }

    private static void SelectSound(FMain main, FyzSound sound)
    {
        var grid = main.dgvSounds;
        var index = grid.Rows.Cast<DataGridViewRow>().First(r => r.DataBoundItem == sound).Index;
        grid.ClearSelection();
        grid.CurrentCell = grid.Rows[index].Cells.Cast<DataGridViewCell>().First(c => c.Visible);
        grid.Rows[index].Selected = true;
        Pump.Events();
    }

    /// <summary>
    ///     Bez zvýrazneného textu v poliach a s fokusom na hlavnom tlačidle.
    /// </summary>
    private static void Unselect(Form form)
    {
        foreach (var box in Descendants(form).OfType<TextBoxBase>())
            box.SelectionLength = 0;
        if (form.AcceptButton is Control accept)
            form.ActiveControl = accept;
    }

    private static void Invoke(Form form, string method) =>
        form.GetType().GetMethod(method, Any)!.Invoke(form, [form, EventArgs.Empty]);

    private static T Field<T>(Form form, string name) =>
        (T)form.GetType().GetField(name, Any)!.GetValue(form)!;

    private static IEnumerable<Control> Descendants(Control control)
    {
        foreach (Control child in control.Controls)
        {
            yield return child;
            foreach (var nested in Descendants(child))
                yield return nested;
        }
    }
}
