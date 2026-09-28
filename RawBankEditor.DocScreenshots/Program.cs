using System.Globalization;
using System.Reflection;
using Microsoft.Win32;
using RawBankEditor.Forms;
using RawBankEditor.XML;
using ToolsCore;
using ToolsCore.Tools;

namespace RawBankEditor.DocScreenshots;

/// <summary>
/// Generátor snímok okien RawBankEditora do dokumentácie.
/// </summary>
/// <remarks>
/// Použitie: <c>RawBankEditor.DocScreenshots [--out=priečinok] [--work=priečinok] [--only=text] [--theme=light|dark|both]</c>.
/// <list type="bullet">
/// <item><c>--out</c> – kam uložiť PNG; predvolene <c>iniss-tools-docs\static\img\rawbankeditor</c> vedľa repozitára.</item>
/// <item><c>--work</c> – kde zostaviť ukážkovú inštaláciu INISS so zvukovou bankou; predvolene <c>C:\INISS</c>
/// (cesta je vidno v titulku). Existujúci priečinok bez značky <c>.docshots</c> sa nezmaže.</item>
/// <item><c>--timeout</c> – po koľkých minútach sa harness ukončí, ak ho zablokuje modálne okno (predvolene 5).</item>
/// <item><c>--only</c> – len snímky, ktorých cesta obsahuje daný text (napr. <c>hlavne-okno</c>).</item>
/// </list>
/// Hodnoty sa zadávajú len v tvare <c>--názov=hodnota</c>: FMain.OnLoad otvára posledný argument, ktorý
/// nezačína pomlčkou, ako banku – samostatná cesta (<c>--out D:\…</c>) by sa otvorila ako inštalácia INISS.
/// Program beží pod vlastným menom, takže konfiguráciu (<c>%LocalAppData%\RawBankEditor.DocScreenshots</c>)
/// aj register má oddelené od RawBankEditora – pri každom spustení začína s predvolenými nastaveniami.
/// </remarks>
internal static class Program
{
    private const BindingFlags Any = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    [STAThread]
    private static int Main(string[] args)
    {
        Options options;
        try
        {
            options = Options.Parse(args);
        }
        catch (ArgumentException e)
        {
            Console.Error.WriteLine(e.Message);
            return 1;
        }

        var log = new List<string>();

        // poistka: modálne okno (chyba, otázka) by harness zablokovalo navždy
        using var watchdog = new System.Threading.Timer(_ =>
        {
            Console.Error.WriteLine("Časový limit vypršal – pravdepodobne visí modálne okno. Posledné kroky:");
            foreach (var line in log.TakeLast(5)) Console.Error.WriteLine("  " + line);
            Console.Error.WriteLine("Otvorené okná: " + string.Join(" | ", WindowCapture.ProcessWindowTitles()));
            Environment.Exit(2);
        }, null, TimeSpan.FromMinutes(options.TimeoutMinutes), Timeout.InfiniteTimeSpan);

        try
        {
            InitApp();
            DemoBank.Build(options.WorkDir, log);
            log.Add($"inštalácia: {options.WorkDir}");
            using (var screen = Graphics.FromHwnd(IntPtr.Zero))
                log.Add($"DPI: {screen.DpiX} (snímky majú rozmery podľa škálovania obrazovky, pre docs 96 = 100 %)");

            var shots = 0;
            foreach (var theme in options.Themes)
            {
                SetTheme(theme);
                shots += new Shots(options, theme, log).Run(options.WorkDir);
            }

            log.Add($"hotovo: {shots} snímok do {options.OutDir}");
            Flush(log);
            return 0;
        }
        catch (Exception e)
        {
            log.Add("CHYBA: " + (e is TargetInvocationException { InnerException: { } inner } ? inner : e));
            Flush(log);
            return 1;
        }
    }

    private static void Flush(List<string> log)
    {
        foreach (var line in log) Console.WriteLine(line);
    }

    /// <summary>
    /// Rovnaká inicializácia ako RawBankEditor.Program.Main, s čistou konfiguráciou, registrom a slovenčinou.
    /// </summary>
    private static void InitApp()
    {
        if (Directory.Exists(AppPaths.DataDir))
            Directory.Delete(AppPaths.DataDir, true);

        // zoznam nedávnych bánk a posledná banka – kľúč patrí harnessu (názov zostavy), nie RawBankEditoru
        var name = Assembly.GetEntryAssembly()!.GetName().Name;
        Registry.CurrentUser.DeleteSubKeyTree($@"SOFTWARE\{name}", throwOnMissingSubKey: false);

        GlobData.Session = AppInit.Initialization<RawBankEditorConfig, RawBankEditorStyle>();

        // harness nebezi v Application.Run: modalne okno (ShowDialog) by pri skonceni svojej slucky odinstalovalo
        // synchronizacny kontext WinForms a BackgroundWorker spusteny potom by volal ProgressChanged/RunWorkerCompleted
        // na vlakne z thread poolu - prvky okna by sa menili z cudzieho vlakna a UI by na niekolko sekund zamrzlo
        WindowsFormsSynchronizationContext.AutoInstall = false;
        SynchronizationContext.SetSynchronizationContext(new WindowsFormsSynchronizationContext());
        // pristup k prvkom z cudzieho vlakna ma skoncit vynimkou, nie zamrznutim
        Control.CheckForIllegalCrossThreadCalls = true;

        var culture = CultureInfo.CreateSpecificCulture("sk");
        Thread.CurrentThread.CurrentUICulture = culture;
        Thread.CurrentThread.CurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }

    private static void SetTheme(string theme)
    {
        var style = theme == "dark" ? RawBankEditorStyle.DefaultDarkStyle : RawBankEditorStyle.DefaultLightStyle;
        GlobData.UsingStyle = style;
        AppInit.MsgBoxStyleInit(style, GlobData.Config);
    }

    /// <summary>
    /// Otvorí hlavné okno s ukážkovou bankou rovnako ako Súbor → Nedávne. Výber jazyka, ktorý sa pri banke
    /// s viacerými jazykmi otvorí ako modálne okno, obslúži <paramref name="chooseLanguage" />.
    /// </summary>
    public static FMain OpenMain(string installDir, Action<FLangChoose> chooseLanguage)
    {
        // FMain si šírky panelov pri posune deliča ukladá do konfigurácie - každá téma začína s predvolenými
        GlobData.Config.LeftPanelWidth = -1;
        GlobData.Config.GroupPanelWidth = -1;
        GlobData.Config.ErrorPanelWidth = -1;
        GlobData.Config.ShowErrorsWindow = true;

        var main = new FMain();
        typeof(global::RawBankEditor.Program).GetProperty(nameof(global::RawBankEditor.Program.MainForm), Any)!.SetValue(null, main);

        main.StartPosition = FormStartPosition.Manual;
        main.Location = new Point(40, 40);
        main.Show();
        Pump.Events();

        // okno sa otvára maximalizované - na snímke by malo šírku celej obrazovky
        main.WindowState = FormWindowState.Normal;
        main.Location = new Point(40, 40);
        main.Size = new Size(1400, 800);
        Pump.Events();

        // deliče napevno: všetky skupiny bez posuvníka, rovnako vysoký zoznam chýb v oboch témach
        SplitContainer Split(string name) => (SplitContainer)main.GetType().GetField(name, Any)!.GetValue(main)!;
        Split("splitContainer1").SplitterDistance = 380;
        Split("splitContainer2").SplitterDistance = 290;
        var errors = Split("splitSoundsErrors");
        errors.SplitterDistance = errors.Height - 200;
        Pump.Events();

        using (new ModalWatcher(form =>
               {
                   if (form is not FLangChoose choose) return false;
                   chooseLanguage(choose);
                   return true;
               }))
        {
            typeof(FMain).GetMethod("OpenProject", Any)!.Invoke(main, [installDir]);
        }

        var worker = (System.ComponentModel.BackgroundWorker)main.GetType().GetField("bWorkerReadDat", Any)!.GetValue(main)!;
        var groups = (DataGridView)main.GetType().GetField("dgvGroups", Any)!.GetValue(main)!;
        if (!Pump.Until(() => !worker.IsBusy && groups.DataSource is not null && groups.Rows.Count > 0))
            throw new TimeoutException("Banka sa nenačítala.");

        return main;
    }

    internal sealed record Options(string OutDir, string WorkDir, string? Only, string[] Themes, int TimeoutMinutes)
    {
        public static Options Parse(string[] args)
        {
            var values = new Dictionary<string, string>();
            foreach (var arg in args)
            {
                var eq = arg.IndexOf('=');
                if (!arg.StartsWith("--", StringComparison.Ordinal) || eq < 0)
                    throw new ArgumentException($"Neznámy argument '{arg}' – hodnoty sa zadávajú ako --názov=hodnota (napr. --only=hlavne-okno).");
                values[arg[..eq]] = arg[(eq + 1)..];
            }

            string? Arg(string name) => values.GetValueOrDefault(name);

            var theme = Arg("--theme") ?? "both";
            return new Options(
                Arg("--out") ?? DefaultOutDir(),
                Arg("--work") ?? @"C:\INISS",
                Arg("--only"),
                theme == "both" ? ["light", "dark"] : [theme],
                int.Parse(Arg("--timeout") ?? "5", CultureInfo.InvariantCulture));
        }

        private static string DefaultOutDir()
        {
            // hľadá iniss-tools-docs v niektorom nadradenom priečinku (D:\INISSTools\iniss-tools-docs)
            for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
            {
                var docs = Path.Combine(dir.FullName, "iniss-tools-docs");
                if (Directory.Exists(docs))
                    return Path.Combine(docs, "static", "img", "rawbankeditor");
            }

            throw new DirectoryNotFoundException("Nenašiel sa priečinok iniss-tools-docs – zadaj --out=….");
        }
    }
}

/// <summary>
/// Obslúži modálne okná, ktoré program otvára cez ShowDialog (výber jazyka, otázky) – bežia vo vlastnej
/// slučke správ, preto ich zachytí časovač. Obsluha vráti <c>true</c>, ak okno spracovala (zavrela).
/// </summary>
internal sealed class ModalWatcher : IDisposable
{
    private readonly System.Windows.Forms.Timer _timer = new() { Interval = 100 };
    private readonly HashSet<Form> _handled = [];

    public ModalWatcher(Func<Form, bool> handler)
    {
        _timer.Tick += (_, _) =>
        {
            var modal = Application.OpenForms.Cast<Form>().FirstOrDefault(f => f.Visible && f.Modal && !_handled.Contains(f));
            if (modal is null)
                return;

            _timer.Stop();
            _handled.Add(modal);
            Pump.Events();
            if (!handler(modal))
                _handled.Remove(modal);
            _timer.Start();
        };
        _timer.Start();
    }

    public void Dispose()
    {
        _timer.Stop();
        _timer.Dispose();
    }
}
