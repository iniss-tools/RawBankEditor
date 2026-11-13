using ExControls;
using RawBankEditor.Properties;
using ToolsCore.Tools;

namespace RawBankEditor.Forms;

// ukotvitelne panely (ExDockManager): zvuky skupiny v dokumente, skupiny a prieskumnik suborov vlavo, zoznam chyb dole
public partial class FMain
{
    // kluce panelov v ulozenom rozlozeni (config.xml) - nemenit
    private const string GroupsPanelKey = "Groups";
    private const string ExplorerPanelKey = "Explorer";
    private const string ErrorsPanelKey = "Errors";

    private readonly ExDockManager _dock = AppDock.Create();
    private readonly ExDockPanel _groupsPanel = new(GroupsPanelKey, Resources.Panel_Groups);
    private readonly ExDockPanel _explorerPanel = new(ExplorerPanelKey, Resources.Panel_Explorer);
    private readonly ExDockPanel _errorsPanel = new(ErrorsPanelKey, Resources.Panel_Errors);
    private string _defaultLayout = "";

    /// <summary>
    /// Navrh okna ma casti v deliacich listach s nadpismi. Za behu idu panely nastrojov a tabulky do ukotvitelnych panelov
    /// (nadpis je ich zahlavie, zbalenie zoznamu chyb nahradi tlacidlo panela) a deliace listy s nadpismi sa zahodia;
    /// rozlozenie z nastaveni sa obnovi az po teme a pismach.
    /// </summary>
    private void InitDocking()
    {
        SuspendLayout();
        MoveToPanel(_groupsPanel, toolStripGroups, dgvGroups);
        MoveToPanel(_explorerPanel, toolStripExplorer, dgvExplorer);
        MoveToPanel(_errorsPanel, toolStripErrors, dgvErrors);
        dgvSounds.Parent?.Controls.Remove(dgvSounds);
        dgvSounds.Dock = DockStyle.Fill;
        _dock.Document.Controls.Add(dgvSounds);

        // povodne usporiadanie (deliace listy, nadpisy, tlacidlo zbalenia) uz nic potrebne neobsahuje
        var designed = splitContainer1;
        _dock.Margin = designed.Margin;
        tableLayoutPanel1.Controls.Remove(designed);
        designed.Dispose();
        tableLayoutPanel1.Controls.Add(_dock, 0, 0);

        _dock.AddPanel(_groupsPanel, ExDockSide.Left);
        _dock.AddPanel(_explorerPanel, _groupsPanel, ExDockSide.Bottom);
        _dock.AddPanel(_errorsPanel, ExDockSide.Bottom);
        _dock.SetZoneSize(ExDockSide.Left, 380);
        _dock.SetZoneSize(ExDockSide.Bottom, 200);
        ResumeLayout(true);

        _defaultLayout = _dock.SaveLayout();
        // rozlozenie sa uklada pri zatvoreni okna spolu s ostatnymi nastaveniami (az ked zatvorenie nikto nezrusil)
        FormClosed += (_, _) =>
        {
            _bank.Config.DockLayout = _dock.SaveLayout();
            SaveConfig();
        };
    }

    // panel nastrojov hore, tabulka vyplni zvysok (ukotvuje sa posledna, preto je prva v zozname prvkov)
    private static void MoveToPanel(ExDockPanel panel, ToolStrip toolStrip, DataGridView grid)
    {
        toolStrip.Parent?.Controls.Remove(toolStrip);
        grid.Parent?.Controls.Remove(grid);
        toolStrip.Dock = DockStyle.Top;
        grid.Dock = DockStyle.Fill;
        panel.Controls.AddRange([grid, toolStrip]);
    }

    private void CreateViewCommands()
    {
        _commands.Add(RbeCommands.ShowGroups, () => _dock.ShowPanel(_groupsPanel)).Bind(tsmimShowGroups);
        _commands.Add(RbeCommands.ShowExplorer, () => _dock.ShowPanel(_explorerPanel)).Bind(tsmimShowExplorer);
        _commands.Add(RbeCommands.ShowErrors, () => _dock.ShowPanel(_errorsPanel)).Bind(tsmimShowErrors);
        _commands.Add(RbeCommands.ResetLayout, () => _dock.LoadLayout(_defaultLayout)).Bind(tsmimResetLayout);
    }
}
