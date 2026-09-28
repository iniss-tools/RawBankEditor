using ExControls;
using ExControls.Providers;
using Microsoft.VisualBasic.FileIO;
using RawBankEditor.Entities;
using RawBankEditor.Properties;
using RawBankEditor.Tools;
using ToolsCore.Entities;
using ToolsCore;
using ToolsCore.Forms;
using ToolsCore.Tools;
using ToolsCore.XML;

namespace RawBankEditor.Forms;

public partial class FMain : Form
{
    private readonly ExBindingList<IRawBankMessage> _messages = new();

    //ikony
    private readonly Bitmap _error, _warning, _info;

    private BackButtonElement? _explorerBack;

    private string _cellOldValue = null!;
    private string _actualStatusTxt = null!;
    private bool _langAlreadySet;
    // jazyky pridane v otvorenej banke, ktorym sa este nezapisal FYZZVUK.DAT - otvaraju sa ako prazdne
    private readonly HashSet<FyzLanguage> _newLanguages = [];
    // jazyk, ktory nacitava bWorkerReadDat
    private FyzLanguage? _loadingLanguage;
    // ci sa CurrentLanguage nacital bez chyby
    private bool _languageLoaded;
    // prave bezi spat/znovu - prepnutie jazyka z akcie sa nepyta na ulozenie a nemaze historiu
    private bool _inUndoRedo;
    // prepnutie pri spat/znovu pocas nacitania ineho jazyka - plati aj pre odlozene nacitanie
    private bool _deferredFromUndoRedo;
    private bool _saved = true;
    private bool _unUndoableUnsavedChanges;

    // presun, premenovanie a mazanie sa na disku prejavia hned, FYZZVUK.DAT az pri ulozeni
    private bool _diskChangedSinceSave;
    private bool _programChange;
    private bool _editingFileName;
    private bool _doNotChangeExplorerSelection;
    private bool _rewriteMode;
    private bool _deletingRows;
    private bool _selectingMoreRows;
    private bool _reorderingGroups;
    private bool _cellUserEditing;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal bool DoNotChangeSoundsSelection { get; set; }

    internal FyzLanguage? CurrentLanguage { get; private set; }
    internal FyzGroup? CurrentGroup { get; private set; }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal FileSystemElement? SelectElement { get; set; }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal DirectoryElement Root { get; set; } = null!;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal DirectoryElement CurrentDirectory { get; set; } = null!;

    internal ExBindingList<FyzSound> MenuSounds { get; private set; } = null!;
    internal ExBindingList<FyzGroup> MenuGroups { get; private set; } = null!;

    internal ExBindingList<FileSystemElement> ExplorerContent { get; } = new();

    internal FMain()
    {
        InitializeComponent();
        tsslStatus.Font = GlobData.Config.Fonts.StateRow;

        switch (GlobData.Config.DesktopMenuMode)
        {
            case DesktopMenu.MsTs:
                menuStripMain.Visible = true;
                toolStripMain.Visible = true;
                menuStripMain.Items.Remove(tscboxLanguages);
                break;
            case DesktopMenu.MsOnly:
                menuStripMain.Visible = true;
                toolStripMain.Visible = false;
                menuStripMain.Items.Add(tscboxLanguages);
                break;
            case DesktopMenu.TsOnly:
                menuStripMain.Visible = false;
                toolStripMain.Visible = true;
                menuStripMain.Items.Remove(tscboxLanguages);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        SetRecentDirs();

        _error = StockIcon(ShellIconType.Error);
        _warning = StockIcon(ShellIconType.Warning);
        _info = StockIcon(ShellIconType.Info);
        
        tsbErrors.Image = _error;
        tsbWarnings.Image = _warning;
        tsbInfos.Image = _info;
        tsmimShowErrors.Image = _error;

        dgvExplorer.DataSource = ExplorerContent;

        menuStripMain.Renderer = new ToolStripProfessionalRenderer(new FormUtils.LightColorTable());
        this.ApplyThemeAndFonts();
        
        _messages.ListChanged += Messages_ListChanged;

        InitShortcuts();
        InitColumns();
        SetColumnsAutoWidth();
        
        dgvSounds.RowHeadersVisible = GlobData.Config.ShowRowsHeader;

        //neviem preco niekedy umiestni stlpec s typom do stredu
        if (cFileType.DisplayIndex != 0) 
            cFileType.DisplayIndex = 0;

        if (GlobData.UsingStyle.HighlightStatusBar)
        {
            statusStripMain.BackColor = GlobData.UsingStyle.ControlsColorScheme.Highlight.BackColor;
            tsslStatus.ForeColor = GlobData.UsingStyle.ControlsColorScheme.Highlight.ForeColor;
            tssbErrors.BackColor = GlobData.UsingStyle.ControlsColorScheme.Highlight.BackColor;
        }
    }

    private void InitShortcuts()
    {
        var shortcuts = GlobData.Config.Shortcuts;

        tsmimOpen.ShortcutKeys = shortcuts.Open;
        tsmimSave.ShortcutKeys = shortcuts.Save;
        tsmimSaveAll.ShortcutKeys = shortcuts.SaveAll;
        tsmimUndo.ShortcutKeys = shortcuts.Undo;
        tsmimRedo.ShortcutKeys = shortcuts.Redo;
        tsmimAddSound.ShortcutKeys = shortcuts.AddSound;
        cmiAddSound.ShortcutKeys = shortcuts.AddSound;
        tsmimDeleteSound.ShortcutKeys = shortcuts.DeleteSounds;
        cmiDeleteSound.ShortcutKeys = shortcuts.DeleteSounds;
        tsmimMoveSounds.ShortcutKeys = shortcuts.MoveSounds;
        cmiMoveSounds.ShortcutKeys = shortcuts.MoveSounds;
        tsmimRewriteMode.ShortcutKeys = shortcuts.RewriteMode;
        tsmimWrapTextSoundCol.ShortcutKeys = shortcuts.WrapTextSoundCol;
        tsmimGoBack.ShortcutKeys = shortcuts.GoBack;
        tsmimGoForward.ShortcutKeys = shortcuts.GoForward;
        tsmimSearch.ShortcutKeys = shortcuts.Search;
        tsmimAddLanguage.ShortcutKeys = shortcuts.AddLanguage;
        tsmimEditLanguage.ShortcutKeys = shortcuts.EditLanguage;
        tsmimDeleteLanguage.ShortcutKeys = shortcuts.DeleteLanguage;
        tsmimAppSettings.ShortcutKeys = shortcuts.AppSettings;
        cmiHighlightProblem.ShortcutKeys = shortcuts.HighlightProblem;
        cmiResolveProblem.ShortcutKeys = shortcuts.ResolveProblem;
    }

    private void InitColumns()
    {
        static void SetCol(DataGridViewColumn column, DesktopColumn format)
        {
            column.Visible = format.Visible;
            column.MinimumWidth = format.MinWidth;
            column.DisplayIndex = format.Order;
        }
        
        var columns = GlobData.Config.DesktopCols;

        SetCol(cSoundKey, columns.Key);
        SetCol(cSoundName, columns.Name);
        SetCol(cSoundAdditionalRelativePath, columns.RelativePath);
        SetCol(cSoundFileName, columns.FileName);
        SetCol(cSoundDuration, columns.Duration);
        SetCol(cSoundText, columns.Text);

        dgvSounds.Columns[dgvSounds.Columns.Count - 1].AutoSizeMode = GlobData.Config.FitLastColumn
            ? DataGridViewAutoSizeColumnMode.Fill
            : DataGridViewAutoSizeColumnMode.None;

        tsmimWrapTextSoundCol.Checked = GlobData.Config.WrapSoundText;
    }

    private void SetSoundTextColumn()
    {
        dgvSounds.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        cSoundText.DefaultCellStyle.WrapMode = GlobData.Config.WrapSoundText ? DataGridViewTriState.True : DataGridViewTriState.NotSet;
    }

    private void UpdateMainUI()
    {
        var menu = GlobData.Config.DesktopMenuMode;
        tsslStatus.Font = GlobData.Config.Fonts.StateRow;
        menuStripMain.Visible = menu is DesktopMenu.MsTs or DesktopMenu.MsOnly;
        toolStripMain.Visible = menu is DesktopMenu.MsTs or DesktopMenu.TsOnly;
        dgvSounds.RowHeadersVisible = GlobData.Config.ShowRowsHeader;

        this.ApplyThemeAndFonts();
        InitColumns();
        InitShortcuts();
        SetColumnsAutoWidth();
        Invalidate(true);
        AppInit.MsgBoxStyleInit(GlobData.UsingStyle, GlobData.Config);
        if (GlobData.UsingStyle.HighlightStatusBar)
        {
            statusStripMain.BackColor = GlobData.UsingStyle.ControlsColorScheme.Highlight.BackColor;
            tsslStatus.ForeColor = GlobData.UsingStyle.ControlsColorScheme.Highlight.ForeColor;
            tssbErrors.BackColor = GlobData.UsingStyle.ControlsColorScheme.Highlight.BackColor;
        }
    }

    private void SetColumnsAutoWidth()
    {
        for (var i = 0; i < dgvSounds.Columns.Count - 1; i++)
        {
            dgvSounds.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            var widthCol = dgvSounds.Columns[i].Width;
            dgvSounds.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dgvSounds.Columns[i].Width = widthCol;
        }
    }

    /// <inheritdoc />
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        var configsDir = ToolsCore.AppPaths.ConfigDir;
        
        tsmimShowErrors.Checked = GlobData.Config.ShowErrorsWindow;
        splitSoundsErrors.Panel2.VisibleChanged += (_, _) =>
        {
            GlobData.Config.ShowErrorsWindow = !splitSoundsErrors.Panel2Collapsed;
            XmlSerialization.WriteData(Utils.CombinePath(configsDir, ToolsCore.FileConsts.FILE_CONFIG)!, GlobData.Config);
        };

        if (GlobData.Config.LeftPanelWidth != -1) splitContainer1.SplitterDistance = GlobData.Config.LeftPanelWidth;
        splitContainer1.SplitterMoved += (_, _) =>
        {
            GlobData.Config.LeftPanelWidth = splitContainer1.SplitterDistance;
            XmlSerialization.WriteData(Utils.CombinePath(configsDir, ToolsCore.FileConsts.FILE_CONFIG)!, GlobData.Config);
        };

        if (GlobData.Config.GroupPanelWidth != -1) splitContainer2.SplitterDistance = GlobData.Config.GroupPanelWidth;
        splitContainer2.SplitterMoved += (_, _) =>
        {
            GlobData.Config.GroupPanelWidth = splitContainer2.SplitterDistance;
            XmlSerialization.WriteData(Utils.CombinePath(configsDir, ToolsCore.FileConsts.FILE_CONFIG)!, GlobData.Config);
        };

        if (GlobData.Config.ErrorPanelWidth != -1) splitSoundsErrors.SplitterDistance = splitSoundsErrors.Width - GlobData.Config.ErrorPanelWidth;
        splitSoundsErrors.SplitterMoved += (_, _) =>
        {
            GlobData.Config.ErrorPanelWidth = splitSoundsErrors.Width - splitSoundsErrors.SplitterDistance;
            XmlSerialization.WriteData(Utils.CombinePath(configsDir, ToolsCore.FileConsts.FILE_CONFIG)!, GlobData.Config);
        };

        AppRegistry.RegisterJumpList();

        //cesta k projektu zadana ako argument ma prednost pred poslednym otvorenym projektom
        var path = Utils.GetProjectPathFromArgs();
        if (path is null && GlobData.Config.Startup == StartupType.LastProject)
            path = AppRegistry.GetLastProject();

        if (Directory.Exists(path))
            OpenProject(path!);
    }

    private bool Saved
    {
        get => _saved;
        set
        {
            if (_saved == value)
                return;
            _saved = value;
            if (!_saved)
                Text += @"*";
            else
                Text = Text.Replace("*", "");
            EnableUndoRedo();
        }
    }

    private void ChangeStatus(string status) => tsslStatus.Text = status;

    private void ChangeStatusReady() => tsslStatus.Text = "Pripravený";

    /// <summary>
    ///     Nacita banku a zacne nacitavat vybrany jazyk. Otvorena banka sa nahradi az ked je nova nacitana
    ///     a jazyk vybrany - pri chybe alebo zruseni vyberu jazyka ostane otvorena povodna.
    /// </summary>
    /// <returns><c>true</c>, ak sa banka otvorila.</returns>
    private bool PrepareGlobalData(string dirpath)
    {
        if (GlobData.OpenedProject is not null && !Saved)
        {
            var result = Utils.ShowQuestion(Resources.FMain_Save_Changes, MessageBoxButtons.YesNoCancel);
            switch (result)
            {
                case DialogResult.Yes:
                    if (!SaveBank(false))
                        return false;
                    break;
                case DialogResult.No when ConfirmDiscardDiskChanges():
                    break;
                default:
                    return false;
            }
        }

        RawBankProject? project = null;
        if (GlobData.Config.DebugModeGUI != DebugMode.AppCrash)
            try
            {
                project = GlobData.LoadProject(dirpath);
            }
            catch (Exception exception)
            {
                Log.Exception(exception);

                switch (GlobData.Config.DebugModeGUI)
                {
                    case DebugMode.OnlyMessage:
                        FError.ShowError(exception.Message);
                        break;
                    case DebugMode.DetailInfo:
                        FError.ShowError(exception.ToString());
                        break;
                }
            }
        else
            project = GlobData.LoadProject(dirpath);

        if (project is null)
            return false;

        // banka bez jazyka sa otvori prazdna - prvy jazyk sa prida cez Nastavenia jazykov
        FyzLanguage? lang = null;
        if (project.Languages.Count == 1)
            lang = project.Languages[0];
        else if (project.Languages.Count > 1)
        {
            var flang = new FLangChoose(project.Languages);
            if (flang.ShowDialog(this) != DialogResult.OK)
                return false;
            lang = flang.Selected;
        }

        GlobData.OpenedProject = project;
        _newLanguages.Clear();
        _diskChangedSinceSave = false;
        dgvErrors.DataSource = null;
        Text = @$"{Application.ProductName} - {GlobData.OpenedProject!.AbsPathToINISS}";

        _langAlreadySet = true;
        tscboxLanguages.ComboBox.DataSource = GlobData.OpenedProject!.Languages;
        _langAlreadySet = false;
        SetComboLanguage(lang);

        moveManager.Clear();
        ResetHistory(false);
        LoadLanguage(lang);
        return true;
    }

    /// <summary>
    ///     Otvori jazyk: zoznam zvukov a subory banky sa nacitaju na pozadi. Bez jazyka (banka nema ziadny) sa okno vycisti.
    /// </summary>
    private void LoadLanguage(FyzLanguage? lang)
    {
        CurrentLanguage = lang;
        _languageLoaded = false;
        // prieskumnik sa nacitava znova - sleduje sa az po nacitani (bWorkerReadDat_RunWorkerCompleted)
        fileSystemWatcher.EnableRaisingEvents = false;
        SwitchLangSettingsButtons(true);

        if (lang is null)
        {
            ShowNoLanguage();
            return;
        }

        // nacitava sa iny jazyk - po dokonceni sa nacita tento (bWorkerReadDat_RunWorkerCompleted)
        if (bWorkerReadDat.IsBusy)
        {
            _deferredFromUndoRedo = _inUndoRedo;
            return;
        }

        // zvuky sa citaju z disku okrem noveho jazyka (FYZZVUK.DAT este nema) a prepnutia pri spat/znovu,
        // kde plati stav v pamati, na ktory sa akcie odkazuju
        var keepMemory = _inUndoRedo || _deferredFromUndoRedo;
        _deferredFromUndoRedo = false;
        var readFile = !_newLanguages.Contains(lang) && !(keepMemory && lang.Groups is not null);
        if (!readFile)
            lang.Groups ??= new List<FyzGroup>();

        _loadingLanguage = lang;
        tscboxLanguages.Enabled = false;
        tspbProgress.Visible = true;
        tspbProgress.Style = ProgressBarStyle.Marquee;
        ChangeStatus("Načítanie súborov banky");
        bWorkerReadDat.RunWorkerAsync((lang, readFile));
    }

    /// <summary>
    ///     Vymaze historiu zmien (akcie sa odkazuju na data jazyka, ktore sa znova nacitaju).
    /// </summary>
    /// <param name="unsavedChanges">Ci ostali neulozene zmeny, ktore sa uz nedaju vratit (napr. zoznam jazykov).</param>
    private void ResetHistory(bool unsavedChanges)
    {
        changeManager.Clear();
        changeManager.SetSavedState();
        _unUndoableUnsavedChanges = unsavedChanges;
        Saved = !unsavedChanges;
        EnableUndoRedo();
    }

    /// <summary>
    ///     Zisti, ci sa zoznam jazykov lisi od ulozeneho FYZBANK.DAT.
    /// </summary>
    private static bool LanguageListChanged()
        => LanguageRules.BankDiffers(GlobData.OpenedProject!.AbsPathToBank, GlobData.OpenedProject.Languages);

    /// <summary>
    ///     Pred odchodom z otvoreneho jazyka sa spyta na neulozene zmeny jeho zoznamu zvukov:
    ///     Ano ich ulozi, Nie zahodi (jazyk sa pri dalsom otvoreni nacita z disku).
    /// </summary>
    /// <returns><c>false</c>, ak pouzivatel odchod zrusil.</returns>
    private bool ConfirmLeaveLanguage()
    {
        var lang = CurrentLanguage;
        if (lang is null || !_languageLoaded || Saved || !LanguageRules.SoundsDiffer(GlobData.OpenedProject!.AbsPathToBank, lang))
            return true;

        var result = Utils.ShowQuestion($"Jazyk {lang.Name} má neuložené zmeny.\n\nUložiť ich pred prepnutím jazyka?", MessageBoxButtons.YesNoCancel);
        switch (result)
        {
            case DialogResult.Yes:
                return SaveBank(false);
            case DialogResult.No when ConfirmDiscardDiskChanges():
                DiscardLanguage(lang);
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    ///     Zahodi zoznam zvukov jazyka v pamati - pri dalsom otvoreni sa nacita z disku, novy jazyk bude prazdny.
    /// </summary>
    private void DiscardLanguage(FyzLanguage lang)
    {
        lang.Groups = _newLanguages.Contains(lang) ? new List<FyzGroup>() : null!;
    }

    /// <summary>
    ///     Pred zahodenim zmien upozorni, ze cast z nich uz je na disku (presunute, premenovane alebo odstranene
    ///     subory a priecinky) - bez ulozenia by im banka nezodpovedala a INISS by nenasiel nahravky.
    /// </summary>
    /// <returns><c>true</c>, ak sa zmeny mozu zahodit.</returns>
    private bool ConfirmDiscardDiskChanges()
        => !_diskChangedSinceSave || Utils.ShowWarning(Resources.FMain_Disk_Changed_Discard, MessageBoxButtons.YesNo) == DialogResult.Yes;

    private void SetComboLanguage(FyzLanguage? lang)
    {
        _langAlreadySet = true;
        tscboxLanguages.SelectedItem = lang;
        _langAlreadySet = false;
    }

    /// <summary>
    ///     Naplní menu naposledy otvorenými projektmi zoradenými od naposledy otvoreného.
    /// </summary>
    private void SetRecentDirs()
    {
        tsmimRecent.DropDownItems.Clear();
        tsbRecent.DropDownItems.Clear();

        var dirs = AppRegistry.GetOpenedProjects();

        foreach (var recentDir in dirs)
        {
            ToolStripItem item1 = new ToolStripMenuItem(recentDir.Path);
            ToolStripItem item2 = new ToolStripMenuItem(recentDir.Path);

            item1.Click += RecentDirs_Click;
            item2.Click += RecentDirs_Click;
            item1.ApplyThemeAndFont();
            item2.ApplyThemeAndFont();
            tsmimRecent.DropDownItems.Add(item1);
            tsbRecent.DropDownItems.Add(item2);
        }

        var enabled = dirs.Length != 0 && dirs[0].Path != "";
        tsmimRecent.Enabled = enabled;
        tsbRecent.Enabled = enabled;
    }
    
    /// <inheritdoc />
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        // pri pisani do bunky patria Delete, Insert a Backspace textu - skratky Odstranit zvuky a Pridat zvuk
        // z ponuky by inak zmazali alebo pridali zvuk
        if (IsTextEditingKey(keyData) && IsEditingText())
            return false;

        // v prieskumniku suborov patria Delete, F2 a F5 suborom (DgvExplorer_KeyDown), nie zvukom
        if (dgvExplorer.ContainsFocus && keyData is Keys.Delete or Keys.F2 or Keys.F5)
            return false;

        // Odstranit zvuky (bez potvrdenia) len v zozname zvukov - v zozname skupin alebo chyb by Del zmazal
        // zvuky, ktore pouzivatel prave nevidi vybrane
        if (keyData == (Keys)GlobData.Config.Shortcuts.DeleteSounds && keyData != Keys.None && !dgvSounds.ContainsFocus)
            return false;

        return base.ProcessCmdKey(ref msg, keyData);
    }

    private static bool IsTextEditingKey(Keys keyData)
        => (keyData & Keys.KeyCode) is Keys.Delete or Keys.Insert or Keys.Back && (keyData & Keys.Alt) == 0;

    private bool IsEditingText()
    {
        if (dgvSounds.EditingControl is TextBoxBase || dgvGroups.EditingControl is TextBoxBase || dgvExplorer.EditingControl is TextBoxBase)
            return true;

        Control? focused = ActiveControl;
        while (focused is ContainerControl { ActiveControl: { } inner })
            focused = inner;
        return focused is TextBoxBase or ComboBox { DropDownStyle: not ComboBoxStyle.DropDownList };
    }

    /// <summary>
    ///     Otvorí projekt a zapíše ho do zoznamu naposledy otvorených projektov.
    /// </summary>
    /// <param name="dirpath">Cesta k priečinku s projektom.</param>
    private void OpenProject(string dirpath)
    {
        // do zoznamu nedavnych len banka, ktora sa naozaj otvorila
        if (!PrepareGlobalData(dirpath))
            return;

        AppRegistry.SetUsageOfProject(dirpath);
        AppRegistry.SetLastProject(dirpath);
        SetRecentDirs();
    }

    private void RecentDirs_Click(object? sender, EventArgs e)
    {
        var menuItem = (ToolStripMenuItem)sender!;
        OpenProject(menuItem.Text!);
    }

    private void DoOpenDir(object sender, EventArgs e)
    {
        var dialog = new FolderBrowserDialog { Description = "Vyberte priečinok s INISS.exe" };
        if (dialog.ShowDialog(this) == DialogResult.Cancel)
            return;

        OpenProject(dialog.SelectedPath);
    }

    private void bWorkerReadDat_DoWork(object sender, DoWorkEventArgs e)
    {
        //part 1 - read and analyze all files in rawbank
        var progressFiles = new ProgressStatus("Načítavanie súborového systému", 0);
        bWorkerReadDat.ReportProgress(-1, progressFiles);
        Root = RawBankExplorer.ExploreFileSystem();

        //part 2 - analyze FYZZVUK.dat file
        var (lang, readFile) = ((FyzLanguage, bool))e.Argument!;
        if (readFile)
            RawBankParser.ReadFyzZvukFile(GlobData.OpenedProject!.AbsPathToBank, lang, bWorkerReadDat);

        //part 3 - merge physical files and logical data
        var progressMerge = new ProgressStatus("Spájam načítané dáta so súborovým systémom", 0);
        bWorkerReadDat.ReportProgress(-1, progressMerge);
        RawBankExplorer.MergeFilesAndData(Root, lang, GlobData.OpenedProject!.Messages);

        Invoke(() => _messages.Clear());

        foreach (var msg in GlobData.OpenedProject!.Messages[lang])
        {
            Invoke(() => _messages.Add(msg));
        }
    }

    private void bWorkerReadDat_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
        if (e.UserState != null)
        {
            var status = (ProgressStatus)e.UserState;
            if (status.TotalProgress < 1)
            {
                tsslStatus.Text = status.ProgressPartName;
                tspbProgress.Style = ProgressBarStyle.Marquee;
            }
            else
            {
                tspbProgress.Style = ProgressBarStyle.Continuous;
                tspbProgress.Maximum = status.TotalProgress;
                tspbProgress.Value = e.ProgressPercentage;
                tsslStatus.Text = $@"{status.ProgressPartName} ({(int)(e.ProgressPercentage / (float)status.TotalProgress * 100)}%)";
            }
        }
        else
        {
            tspbProgress.Style = ProgressBarStyle.Marquee;
            if (e.ProgressPercentage < 0) 
                tspbProgress.Value = e.ProgressPercentage;
        }
    }

    private void bWorkerReadDat_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        tspbProgress.Visible = false;

        // pocas nacitania sa otvoril iny jazyk (spat/znovu, odstranenie jazyka) - vysledok neplati, nacita sa ten
        if (!ReferenceEquals(_loadingLanguage, CurrentLanguage))
        {
            if (e.Error != null && _loadingLanguage != null)
                DiscardLanguage(_loadingLanguage);
            _loadingLanguage = null;
            LoadLanguage(CurrentLanguage);
            return;
        }

        if (GlobData.Config.DebugModeGUI != DebugMode.AppCrash && e.Error != null)
        {
            // ciastocne nacitane skupiny sa nesmu zapisat cez Ulozit vsetko
            DiscardLanguage(CurrentLanguage!);
            Log.Exception(e.Error);

            ChangeStatus("Vznikla chyba pri načítaní banky");

            switch (GlobData.Config.DebugModeGUI)
            {
                case DebugMode.OnlyMessage:
                    FError.ShowError(e.Error.Message);
                    break;
                case DebugMode.DetailInfo:
                    FError.ShowError(e.Error.ToString());
                    break;
            }

            // jazyk nema nacitane skupiny - v okne nesmu ostat skupiny a zvuky predchadzajucej banky ani jazyka
            // a nic sa nesmie ulozit; vyber jazyka ostava, aby sa dalo prepnut na iny
            ClearAfterLoadError();
            return;
        }

        _languageLoaded = true;
        _programChange = true;
        dgvSounds.DataSource = null;
        MenuGroups = new ExBindingList<FyzGroup>(CurrentLanguage!.Groups);
        dgvGroups.DataSource = MenuGroups;
        tscboxLanguages.Enabled = true;
        _programChange = false;

        //_lastMovePosition = new MovePosition(GetSelectedSoundRows(), CurrentLanguage!, CurrentGroup!);
        moveManager.AddCommand(new SelectedCellSoundMoveAction(this,
            new MovePosition(GetSelectedSoundRows(), CurrentLanguage!, CurrentGroup!)));

        ChangeStatusReady();

        SwitchAddSoundButtons(true);
        SwitchRewriteModeButtons(true);
        SwitchDeleteSoundsButtons(!dgvSounds.IsSelectionEmpty());
        SwitchMoveSoundsButtons(!dgvSounds.IsSelectionEmpty());

        SwitchAddGroupButtons(true);
        SwitchEditGroupButtons(true);        
        SwitchDeleteGroupButtons(true);
        
        SwitchLangSettingsButtons(true);
        SwitchConvertSoundsLangButtons(true);
        SwitchSearchButtons(true);

        SwitchHighlightProblemButtons(true);
        SwitchSolveProblemButtons(true);

        SwitchSaveButtons(true);

        dgvErrors.DataSource = _messages;
        fileSystemWatcher.Path = GlobData.OpenedProject!.AbsPathToBank;
        fileSystemWatcher.EnableRaisingEvents = true;
    }

    private void ClearAfterLoadError()
    {
        fileSystemWatcher.EnableRaisingEvents = false;
        // bez skupiny ValidateRow neoveruje riadky starej banky voci novemu jazyku, ktory nema priecinok
        CurrentGroup = null;
        _programChange = true;
        dgvSounds.DataSource = null;
        dgvGroups.DataSource = null;
        _programChange = false;
        ExplorerContent.Clear();
        _messages.Clear();
        tscboxLanguages.Enabled = true;

        SwitchAddSoundButtons(false);
        SwitchRewriteModeButtons(false);
        SwitchDeleteSoundsButtons(false);
        SwitchMoveSoundsButtons(false);
        SwitchAddGroupButtons(false);
        SwitchEditGroupButtons(false);
        SwitchDeleteGroupButtons(false);
        SwitchConvertGroupButtons(false);
        SwitchConvertSoundsLangButtons(false);
        SwitchSearchButtons(false);
        SwitchHighlightProblemButtons(false);
        SwitchSolveProblemButtons(false);
        SwitchSaveButtons(false);
        // jazyk sa da odstranit alebo pridat iny; konverzia jazyka je vypnuta (_languageLoaded)
        SwitchLangSettingsButtons(true);
    }

    /// <summary>
    ///     Banka nema ziadny jazyk: prazdne okno, dostupne je len pridanie jazyka a ulozenie zoznamu jazykov.
    /// </summary>
    private void ShowNoLanguage()
    {
        ClearAfterLoadError();
        tscboxLanguages.Enabled = GlobData.OpenedProject!.Languages.Count > 0;
        // FYZBANK.DAT sa da ulozit aj bez jazyka, napr. po odstraneni posledneho
        SwitchSaveButtons(true);
        tspbProgress.Visible = false;
        ChangeStatus("Banka neobsahuje žiadny jazyk – pridajte ho cez Nastavenia jazykov");
    }

    /// <summary>
    ///     Vlozi jazyk do zoznamu jazykov banky bez prepnutia. V prazdnej banke sa jazyk rovno otvori.
    /// </summary>
    internal void InsertLanguage(FyzLanguage language, int index)
    {
        var languages = GlobData.OpenedProject!.Languages;
        _langAlreadySet = true;
        languages.Insert(Math.Clamp(index, 0, languages.Count), language);
        _langAlreadySet = false;

        if (CurrentLanguage is null)
        {
            SetComboLanguage(language);
            LoadLanguage(language);
        }
        else
            SetComboLanguage(CurrentLanguage);
    }

    /// <summary>
    ///     Odstrani jazyk zo zoznamu jazykov banky. Ak bol otvoreny, jeho neulozene zmeny sa zahodia (novy jazyk si ich
    ///     necha pre vratenie) a otvori sa jazyk na jeho mieste.
    /// </summary>
    /// <param name="language">Odstranovany jazyk.</param>
    /// <param name="next">Jazyk, ktory sa ma otvorit namiesto odstraneneho (ak je v banke), inak jazyk na jeho mieste.</param>
    /// <returns>Pozicia, na ktorej jazyk bol, alebo -1.</returns>
    internal int RemoveLanguage(FyzLanguage language, FyzLanguage? next = null)
    {
        var languages = GlobData.OpenedProject!.Languages;
        var index = languages.IndexOf(language);
        if (index < 0)
            return -1;

        _langAlreadySet = true;
        languages.RemoveAt(index);
        _langAlreadySet = false;

        if (!ReferenceEquals(language, CurrentLanguage))
        {
            SetComboLanguage(CurrentLanguage);
            return index;
        }

        if (!_newLanguages.Contains(language))
            DiscardLanguage(language);

        if (next is null || !languages.Contains(next))
            next = languages.Count == 0 ? null : languages[Math.Min(index, languages.Count - 1)];
        SetComboLanguage(next);
        LoadLanguage(next);
        return index;
    }

    /// <summary>
    ///     Presunie priecinok jazyka do kosa. Zmazanie sa v prieskumniku nespracuva - jazyk sa zaroven zatvara.
    /// </summary>
    internal void DeleteLanguageDirectory(string directory)
    {
        if (!Directory.Exists(directory))
            return;

        var watching = fileSystemWatcher.EnableRaisingEvents;
        fileSystemWatcher.EnableRaisingEvents = false;
        RawBankExplorer.ConvertSoundIsHandled = true;
        try
        {
            Utils.DeleteDirectoryToRecycleBin(directory, true);
        }
        finally
        {
            fileSystemWatcher.EnableRaisingEvents = watching;
            // udalosti, ktore watcher zaradil do fronty okna este pred vypnutim, sa spracuju az po tomto
            BeginInvoke(() => RawBankExplorer.ConvertSoundIsHandled = false);
        }
    }

    /// <summary>
    ///     Obnovi zobrazenie jazyka v poli Jazyk po zmene nazvu.
    /// </summary>
    internal void RefreshLanguage(FyzLanguage language)
    {
        var languages = GlobData.OpenedProject!.Languages;
        var index = languages.IndexOf(language);
        if (index < 0)
            return;

        _langAlreadySet = true;
        languages.ResetItem(index);
        _langAlreadySet = false;
        SetComboLanguage(CurrentLanguage);
    }

    /// <summary>
    ///     Obnovi tabulku zvukov a pocty zvukov v skupinach po zmene zoznamu zvukov priamo vo FyzGroup.Sounds
    ///     - BindingList MenuSounds (a dgvSounds.ResetBindings) o takej zmene nevie a riadky by ostali stare.
    /// </summary>
    internal void RefreshSoundViews()
    {
        _programChange = true;
        MenuSounds?.ResetBindings();
        _programChange = false;
        dgvGroups.Invalidate();
    }

    internal void RegisterNewAction()
    {
        _unUndoableUnsavedChanges = true;
        Saved = false;
    }

    internal void RegisterNewAction(IUndoRedoCommand action)
    {
        changeManager.AddCommand(action);
        Saved = false;
    }

    private void ResetStatusAfterTask()
    {
        ChangeStatusReady();
        tspbProgress.Visible = false;
        FillExplorerList(CurrentDirectory);
        CheckProjectState();
        dgvGroups.ResetBindings();
        dgvSounds.ResetBindings();
    }

    #region MainMenu

    private void DoSave(object sender, EventArgs e) => SaveBank(false);

    private void DoSaveAll(object sender, EventArgs e) => SaveBank(true);

    /// <summary>
    ///     Zapise FYZBANK.DAT a FYZZVUK.DAT otvoreneho jazyka (alebo vsetkych nacitanych jazykov) a novych jazykov,
    ///     aby FYZBANK.DAT neodkazoval na chybajuci subor.
    /// </summary>
    /// <returns><c>false</c>, ak zapis zlyhal - zmeny ostavaju neulozene.</returns>
    private bool SaveBank(bool allLanguages)
    {
        var project = GlobData.OpenedProject!;
        try
        {
            RawBankParser.WriteFyzBankFile(project.AbsPathToBank, project.Languages.ToList());

            foreach (var lang in project.Languages)
            {
                // nenacitany jazyk (aj po chybe nacitania) sa nezapisuje - na disku ostava jeho subor
                if (lang.Groups is null)
                    continue;
                if (!allLanguages && !_newLanguages.Contains(lang) && !(ReferenceEquals(lang, CurrentLanguage) && _languageLoaded))
                    continue;

                Directory.CreateDirectory(lang.GetAbsPath(project.AbsPathToBank));
                RawBankParser.WriteFyzZvukFile(project.AbsPathToBank, lang);
                _newLanguages.Remove(lang);
            }
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            // napr. subor len na citanie, bez opravneni na zapis alebo otvoreny inym programom
            Log.Exception(exception);
            Utils.ShowError("Uloženie banky zlyhalo, zmeny ostali neuložené.\n\n" + exception.Message);
            return false;
        }

        _unUndoableUnsavedChanges = false;
        _diskChangedSinceSave = false;
        changeManager.SetSavedState();
        Saved = true;
        return true;
    }

    private void DoUndo(object sender, EventArgs e)
    {
        _inUndoRedo = true;
        try
        {
            changeManager.Undo();
        }
        finally
        {
            _inUndoRedo = false;
        }
        EnableUndoRedo();
    }

    private void DoRedo(object sender, EventArgs e)
    {
        _inUndoRedo = true;
        try
        {
            changeManager.Redo();
        }
        finally
        {
            _inUndoRedo = false;
        }
        EnableUndoRedo();
    }

    private void EnableUndoRedo()
    {
        SwitchUndoButtons(changeManager.CanUndo);
        SwitchRedoButtons(changeManager.CanRedo);
    }

    private void EnableGoBackForward()
    {
        SwitchGoBackButtons(moveManager.CanBackward);
        SwitchGoForwardButtons(moveManager.CanForward);
    }

    private void RefreshBackButtonDropDown()
    {
        tsbGoBack.DropDownItems.Clear();

        var actions = moveManager.GetForwardHistory().ToArray();

        for (var i = Math.Max(0, actions.Length - 11); i < actions.Length; i++)
        {
            var action = actions[i];
            var item = new ToolStripMenuItem(action.CommandName);
            item.Tag = action;
            item.Click += ForwardButtonItemOnClick;
            item.ForeColor = GlobData.UsingStyle.ControlsColorScheme.Button.ForeColor;
            tsbGoBack.DropDownItems.Add(item);
        }

        if (moveManager.CurrentCommand is not null)
        {
            var currentItem = new ToolStripMenuItem(moveManager.CurrentCommand.CommandName);
            currentItem.Checked = true;
            currentItem.ForeColor = GlobData.UsingStyle.ControlsColorScheme.Button.ForeColor;
            tsbGoBack.DropDownItems.Add(currentItem);
        }

        actions = moveManager.GetBackwardHistory().ToArray();

        for (var i = 0; i < Math.Min(actions.Length, 11); i++)
        {
            var action = actions[i];
            var item = new ToolStripMenuItem(action.CommandName);
            item.Tag = action;
            item.Click += BackButtonItemOnClick;
            item.ForeColor = GlobData.UsingStyle.ControlsColorScheme.Button.ForeColor;
            tsbGoBack.DropDownItems.Add(item);
        }
    }

    private void BackButtonItemOnClick(object? sender, EventArgs e)
    {
        var tsmi = (ToolStripMenuItem)sender!;
        if (tsmi.Tag is IBackwardForwardCommand action)
        {
            moveManager.Backward(action);
            EnableGoBackForward();
            RefreshBackButtonDropDown();
        }
    }

    private void ForwardButtonItemOnClick(object? sender, EventArgs e)
    {
        var tsmi = (ToolStripMenuItem)sender!;
        if (tsmi.Tag is IBackwardForwardCommand action)
        {
            moveManager.Forward(action);
            EnableGoBackForward();
            RefreshBackButtonDropDown();
        }
    }

    private void MoveManager_CommandAdded(object sender, BackwardForwardAddedCommandEventArgs e)
    {
        RefreshBackButtonDropDown();
    }

    private void DoGoBack(object sender, EventArgs e)
    {
        moveManager.Backward();
        EnableGoBackForward();
        RefreshBackButtonDropDown();
    }

    private void DoGoForward(object sender, EventArgs e)
    {
        moveManager.Forward();
        EnableGoBackForward();
        RefreshBackButtonDropDown();
    }

    private void DoSearch(object sender, EventArgs e)
    {
        var fsearch = new FSearch();
        fsearch.Show(this);
    }

    private void DoAddSound(object sender, EventArgs e)
    {
        if (CurrentGroup is null)
            return;

        var form = new FAddSound(CurrentGroup);
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            MenuSounds.Add(form.Sound);
            RegisterNewAction(new AddSoundAction(this, form.Sound));
            CheckProjectState();
        }
    }

    private void DoDeleteSounds(object sender, EventArgs e)
    {
        if (dgvSounds.IsSelectionEmpty() || dgvGroups.IsSelectionEmpty())
            return;

        // podla zvukov, nie indexov riadkov - zoznam skupiny sa pri odstranovani zmensuje
        var sounds = dgvSounds.SelectedRows.Cast<DataGridViewRow>().Select(r => r.DataBoundItem).OfType<FyzSound>().ToList();
        if (sounds.Count == 0)
            return;

        var firstDisplayedRow = sounds.Min(s => s.Group.Sounds.IndexOf(s)) - 1;
        var action = new RemovedSoundsAction(this, sounds);

        _deletingRows = true;
        action.Apply();
        _deletingRows = false;

        if (firstDisplayedRow >= 0 && firstDisplayedRow < dgvSounds.Rows.Count)
        {
            EnsureVisibleRow(dgvSounds, firstDisplayedRow);
        }

        RegisterNewAction(action);
        CheckProjectState();
    }

    private void DoMoveSounds(object sender, EventArgs e)
    {
        if (dgvSounds.IsSelectionEmpty() || dgvGroups.IsSelectionEmpty())
            return;

        var sounds = dgvSounds.SelectedRows.Cast<DataGridViewRow>().Select(r => r.DataBoundItem).OfType<FyzSound>()
            .OrderBy(s => CurrentGroup!.Sounds.IndexOf(s)).ToList();
        if (sounds.Count == 0)
            return;

        var form = new FSoundsMove(CurrentLanguage!.Groups, CurrentGroup!);
        if (form.ShowDialog(this) != DialogResult.OK)
            return;

        var source = CurrentGroup!;
        var problems = SoundRules.ValidateMove(sounds, form.NewGroup, GlobData.OpenedProject!.AbsPathToBank);
        if (problems.Count > 0)
        {
            Utils.ShowError("Zvuky sa nepresunuli:\n\n" + string.Join("\n", problems.Take(10)));
            return;
        }

        var moved = MoveSoundsToGroup(sounds, form.NewGroup);
        if (moved.Count > 0)
            RegisterNewAction(new MoveSoundsAction(this, moved, source, form.NewGroup));
    }

    /// <summary>
    ///     Presunie zvuky do skupiny <paramref name="target" />. Nahravka, ktora lezi priamo v priecinku skupiny,
    ///     sa presunie do priecinka cielovej skupiny; nahravka s pridavnou cestou ostane na mieste a cesta sa
    ///     prepocita voci novej skupine (INISS ju berie relativne k priecinku skupiny).
    /// </summary>
    /// <returns>Zvuky, ktore sa presunuli - pri chybe suboru sa presun zastavi a zvysne ostanu na mieste.</returns>
    internal List<FyzSound> MoveSoundsToGroup(IList<FyzSound> sounds, FyzGroup target)
    {
        var pathToBank = GlobData.OpenedProject!.AbsPathToBank;
        var targetDir = target.GetAbsPath(pathToBank);
        var moved = new List<FyzSound>();

        // presunute subory nesmu vyvolat automaticke vlozenie zvukov ani zmazanie prvkov prieskumnika
        var watching = fileSystemWatcher.EnableRaisingEvents;
        fileSystemWatcher.EnableRaisingEvents = false;
        try
        {
            foreach (var sound in sounds)
            {
                var sourcePath = sound.File?.FileInfo.FullName ?? sound.GetAbsPath(pathToBank);
                var moveFile = SoundRules.FileMovesWithSound(sound) && File.Exists(sourcePath);
                var newPath = Path.Combine(targetDir, sound.FileName);

                if (moveFile)
                {
                    File.Move(sourcePath, newPath);
                    _diskChangedSinceSave = true;
                }

                sound.Group.Sounds.Remove(sound);
                sound.Group = target;
                target.Sounds.Add(sound);

                if (moveFile)
                {
                    if (sound.File is not null)
                        MoveFileElement(sound.File, target.Directory, newPath);
                }
                else if (!SoundRules.FileMovesWithSound(sound))
                {
                    // nahravka ostava, kde je - pridavna cesta musi ukazovat na nu aj z novej skupiny
                    sound.AdditionalRelativePath = SoundRules.AdditionalPathFor(target, Path.GetDirectoryName(sourcePath)!, pathToBank);
                }

                moved.Add(sound);
            }
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            Log.Exception(exception);
            Utils.ShowError($"Presun zvukov sa zastavil – presunulo sa {moved.Count} z {sounds.Count}.\n\n{exception.Message}");
        }
        finally
        {
            fileSystemWatcher.EnableRaisingEvents = watching;
        }

        RefreshSoundViews();
        SelectGroup(target);
        FillExplorerList(CurrentDirectory);
        CheckProjectState();
        return moved;
    }

    /// <summary>
    ///     Presunie prvok suboru v strome prieskumnika do ineho priecinka (subor na disku uz je presunuty).
    /// </summary>
    private static void MoveFileElement(SoundFileElement file, DirectoryElement? target, string newPath)
    {
        file.Parent?.Children.Remove(file);
        file.FileInfo = new FileInfo(newPath);
        file.Name = Path.GetFileName(newPath);
        file.Parent = target;
        target?.Children.Add(file);
    }

    /// <summary>
    ///     Prida prazdny jazyk (s priecinkom v RAWBANK) a otvori ho. FYZBANK.DAT a jeho FYZZVUK.DAT sa zapisu pri ulozeni.
    ///     Ak v priecinku FYZZVUK.DAT uz je, jazyk sa nacita z neho.
    /// </summary>
    private void DoAddLanguage(object sender, EventArgs e)
    {
        var project = GlobData.OpenedProject!;
        var form = new FAddEditLanguage(project.Languages);
        if (form.ShowDialog(this) != DialogResult.OK || !ConfirmLeaveLanguage())
            return;

        var lang = new FyzLanguage(form.LanguageKey, form.LanguageName, form.LanguageRelativePath) { Groups = new List<FyzGroup>() };
        var directory = Path.TrimEndingDirectorySeparator(lang.GetAbsPath(project.AbsPathToBank));
        string? createdDirectory = null;
        try
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
                createdDirectory = directory;
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
        {
            Utils.ShowError($"Priečinok jazyka {directory} sa nepodarilo vytvoriť.\n\n{ex.Message}");
            return;
        }

        // zmeny zoznamu jazykov spred pridania sa uz nedaju vratit (historia sa maze pri kazdom prepnuti jazyka)
        var listChanged = LanguageListChanged();
        // priecinok uz ma FYZZVUK.DAT (napr. jazyk odstraneny len zo zoznamu) - nacita sa, inak by ho ulozenie prepisalo prazdnym
        if (File.Exists(LanguageRules.SoundsFile(project.AbsPathToBank, lang)))
            lang.Groups = null!;
        else
            _newLanguages.Add(lang);
        var previous = CurrentLanguage;
        var index = project.Languages.Count;
        InsertLanguage(lang, index);
        if (!ReferenceEquals(CurrentLanguage, lang))
        {
            SetComboLanguage(lang);
            LoadLanguage(lang);
        }

        ResetHistory(listChanged);
        RegisterNewAction(new AddLanguageAction(this, lang, index, previous, createdDirectory));
    }

    private void DoEditLanguage(object sender, EventArgs e)
    {
        var language = CurrentLanguage!;
        var form = new FAddEditLanguage(GlobData.OpenedProject!.Languages, language);
        if (form.ShowDialog(this) != DialogResult.OK)
            return;

        if (form.LanguageKey == language.Key && form.LanguageName == language.Name && form.LanguageRelativePath == language.RelativePath)
            return;

        var action = new EditLanguageAction(this, language,
            (language.Key, form.LanguageKey), (language.Name, form.LanguageName), (language.RelativePath, form.LanguageRelativePath));
        if (ChangeLanguage(language, form.LanguageKey, form.LanguageName, form.LanguageRelativePath))
            RegisterNewAction(action);
    }

    private void DoDeleteLanguage(object sender, EventArgs e)
    {
        var result = Utils.ShowWarning("Práve vybraný jazyk sa odstráni.\n\nSte si istý?", MessageBoxButtons.YesNo);
        if (result != DialogResult.Yes)
            return;

        result = Utils.ShowWarning("Vymazať aj priečinok so zvukmi jazyka?\n\nPriečinok sa premiestni do koša.", MessageBoxButtons.YesNoCancel);
        if (result == DialogResult.Cancel)
            return;

        var language = CurrentLanguage!;
        var withData = result == DialogResult.Yes;
        var directory = Path.TrimEndingDirectorySeparator(language.GetAbsPath(GlobData.OpenedProject!.AbsPathToBank));

        // neulozene zmeny zvukov odstraneneho jazyka sa zahodia, zmeny zoznamu jazykov ostavaju neulozene
        var listChanged = LanguageListChanged();
        // priecinok sa maze skor, ako sa zacne nacitavat dalsi jazyk (prehliadanie suborov banky na pozadi)
        if (withData)
            DeleteLanguageDirectory(directory);
        var index = RemoveLanguage(language);

        ResetHistory(listChanged);
        RegisterNewAction(new RemoveLanguageAction(this, language, index, directory, withData));
    }

    private void ShowAppSettings(object sender, EventArgs e)
    {
        var form = new FAppSettings(GlobData.Config, GlobData.Styles);
        if (form.ShowDialog() == DialogResult.OK)
        {
            UpdateMainUI();
        }
    }

    private void ShowInfoApp(object sender, EventArgs e)
    {
        var form = new FAboutApp(Resources.AboutAppDescription, Resources.raw);
        form.ShowDialog();
    }

    private void ShowUpdates(object sender, EventArgs e) => Utils.OpenShell(LinkConsts.UPDATE);

    private void RewriteModeChanged(object sender, EventArgs e)
    {
        if (sender == tsmimRewriteMode) 
            tsbRewriteMode.Checked = tsmimRewriteMode.Checked;
        else if (sender == tsbRewriteMode) 
            tsmimRewriteMode.Checked = tsbRewriteMode.Checked;

        _rewriteMode = tsmimRewriteMode.Checked;
    }

    private void WrapSoundTextChanged(object sender, EventArgs e)
    {
        if (sender == tsmimWrapTextSoundCol)
            tsbWrapTextSoundCol.Checked = tsmimWrapTextSoundCol.Checked;
        else if (sender == tsbWrapTextSoundCol)
            tsmimWrapTextSoundCol.Checked = tsbWrapTextSoundCol.Checked;

        GlobData.Config.WrapSoundText = tsmimWrapTextSoundCol.Checked;
        var configsDir = ToolsCore.AppPaths.ConfigDir;
        XmlSerialization.WriteData(Utils.CombinePath(configsDir, ToolsCore.FileConsts.FILE_CONFIG)!, GlobData.Config);
        SetSoundTextColumn();
    }

    #endregion

    #region ErrorsPanel

    private void DgvErrors_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
    {
        if (e.Button.HasFlag(MouseButtons.Right) && e.RowIndex != -1)
        {
            dgvErrors.Rows[e.RowIndex].Selected = true;
        }
    }

    private void DoFindProblem(object sender, EventArgs e)
    {
        if (dgvErrors.IsSelectionEmpty())
            return;

        var problem = (IRawBankMessage)dgvErrors.SelectedRows[0].DataBoundItem!;
        problem.Show();
    }

    private void DoSolveProblem(object sender, EventArgs e)
    {
        if (dgvErrors.IsSelectionEmpty())
            return;

        var problem = (IRawBankMessage)dgvErrors.SelectedRows[0].DataBoundItem!;
        problem.Resolve();
        CheckProjectState();
    }

    #endregion

    #region ExplorerPanel

    private void DoOpenFileInExplorer(object sender, EventArgs e)
    {
        if (dgvExplorer.IsSelectionEmpty())
            return;

        var item = dgvExplorer.SelectedRows[0].DataBoundItem as FileSystemElement;

        const string explorerExe = "explorer.exe";
        
        switch (item)
        {
            case DirectoryElement de:
                Process.Start(explorerExe, $"/select, \"{de.DirInfo.FullName}\"");
                break;
            case FileElement fe:
                if (fe.FileInfo.DirectoryName != null)
                    Process.Start(explorerExe, $"/select, \"{fe.FileInfo.FullName}\"");
                break;
            case BackButtonElement:
                Process.Start(explorerExe, $"/select, \"{CurrentDirectory.DirInfo.FullName}\"");
                break;
        }
    }

    private void DoPlayFile(object sender, EventArgs e)
    {
        if (dgvExplorer.IsSelectionEmpty())
            return;

        if (dgvExplorer.SelectedRows[0].DataBoundItem is SoundFileElement se)
            SoundUtils.Play(se.FileInfo.FullName);
    }

    #endregion

    #region SelectionManagement

    private void tscboxLanguages_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (_langAlreadySet)
            return;

        var selected = tscboxLanguages.SelectedItem as FyzLanguage;
        if (ReferenceEquals(selected, CurrentLanguage))
            return;

        // prepnutie z akcie spat/znovu pokracuje v jej historii
        if (_inUndoRedo)
        {
            LoadLanguage(selected);
            return;
        }

        if (!ConfirmLeaveLanguage())
        {
            SetComboLanguage(CurrentLanguage);
            return;
        }

        LoadLanguage(selected);
        // akcie historie sa odkazuju na data opusteneho jazyka; neulozeny moze ostat len zoznam jazykov
        ResetHistory(LanguageListChanged());
    }

    private void dgvGroups_SelectionChanged(object sender, EventArgs e)
    {
        if (_reorderingGroups)
            return;
        
        _actualStatusTxt = tsslStatus.Text!;
        tsslStatus.Text = "Otváram skupinu zvukov";
        tsslStatus.Invalidate();
        tspbProgress.Visible = true;
        tspbProgress.Style = ProgressBarStyle.Marquee;
        
        if (!dgvGroups.IsSelectionEmpty())
        {
            CurrentGroup = (FyzGroup)dgvGroups.SelectedRows[0].DataBoundItem!;

            FillExplorerList(CurrentGroup!.Directory);

            MenuSounds = new ExBindingList<FyzSound>(CurrentGroup!.Sounds);

            _doNotChangeExplorerSelection = true;
            dgvSounds.DataSource = MenuSounds;
            _doNotChangeExplorerSelection = false;

            foreach (DataGridViewRow row in dgvSounds.Rows) 
                ValidateRow(row.Index);

            if (!_programChange && !dgvSounds.IsSelectionEmpty())
            {
                var newPosition = new MovePosition(GetSelectedSoundRows(), CurrentLanguage!, CurrentGroup!);
                var action = new SelectedCellSoundMoveAction(this, newPosition);
                moveManager.AddCommand(action);
                EnableGoBackForward();
            }
        }

        //reset
        tspbProgress.Visible = false;
        tsslStatus.Text = _actualStatusTxt;

        SwitchConvertGroupButtons(!dgvGroups.IsSelectionEmpty());
    }

    private void dgvSounds_SelectionChanged(object sender, EventArgs e)
    {
        if (_doNotChangeExplorerSelection || _selectingMoreRows || DoNotChangeSoundsSelection)
            return;

        SwitchDeleteSoundsButtons(!dgvSounds.IsSelectionEmpty());
        SwitchMoveSoundsButtons(!dgvSounds.IsSelectionEmpty());

        var rows = GetSelectedSoundRows();
        if (rows.Length == 0)
            return;

        if (!_programChange)
        {
            var newPosition = new MovePosition(rows, CurrentLanguage!, CurrentGroup!);
            var action = new SelectedCellSoundMoveAction(this, newPosition);
            moveManager.AddCommand(action);
            EnableGoBackForward();
            //_lastMovePosition = newPosition;
        }

        var item = (FyzSound)dgvSounds.Rows[dgvSounds.SelectedRows[0].Index].DataBoundItem!;
        if (item is null)
            return;

        dgvExplorer.ClearSelection();

        //this folder
        if ((string.IsNullOrWhiteSpace(item.AdditionalRelativePath) || item.AdditionalRelativePath == "\\") && item.Group.Directory == CurrentDirectory)
        {
            for (var j = 0; j < ExplorerContent.Count; j++)
            {
                var element = ExplorerContent[j];
                if (element is SoundFileElement sfe && sfe.Name == item.FileName)
                {
                    dgvExplorer.Rows[j].Selected = true;
                    EnsureVisibleRow(dgvExplorer, dgvExplorer.Rows[j].Index);
                }
            }
        }
        //another folder
        else
        {
            FillExplorerList(item.File.Parent!);
            var index = ExplorerContent.IndexOf(item.File);
            if (index == -1) 
                return;

            dgvExplorer.ClearSelection();
            dgvExplorer.Rows[index].Selected = true;
        }
    }

    private FyzSound[] GetSelectedSoundRows()
    {
        var count = dgvSounds.SelectedRows.Count;
        if (count == 0 || _programChange)
            return Array.Empty<FyzSound>();

        var rows = new FyzSound[count];

        for (var i = 0; i < count; i++)
        {
            var row = dgvSounds.SelectedRows[i];
            rows[i] = (FyzSound)row.DataBoundItem!;
        }

        return rows;
    }

    private void DgvSounds_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
    {
        if (e.RowIndex != -1 && e.Button.HasFlag(MouseButtons.Right)) 
            dgvSounds.Rows[e.RowIndex].Selected = true;
    }

    #endregion //SelectionManagement

    #region SwitchButtons

    private void SwitchGoBackButtons(bool enabled)
    {
        tsbGoBack.Enabled = enabled;
        tsmimGoBack.Enabled = enabled;
    }

    private void SwitchGoForwardButtons(bool enabled)
    {
        tsbGoForward.Enabled = enabled;
        tsmimGoForward.Enabled = enabled;
    }

    private void SwitchUndoButtons(bool enabled)
    {
        tsbUndo.Enabled = enabled;
        tsmimUndo.Enabled = enabled;
    }

    private void SwitchRedoButtons(bool enabled)
    {
        tsbRedo.Enabled = enabled;
        tsmimRedo.Enabled = enabled;
    }

    private void SwitchSaveButtons(bool enabled)
    {
        tsbSave.Enabled = enabled;
        tsmimSave.Enabled = enabled;
        tsbSaveAll.Enabled = enabled;
        tsmimSaveAll.Enabled = enabled;
    }

    private void SwitchLangSettingsButtons(bool enabled)
    {
        tsbLangsSettings.Enabled = enabled;
        tsmimLangsSettings.Enabled = enabled;
        tsmimAddLanguage.Enabled = enabled;
        tsmiAddLanguage.Enabled = enabled;
        tsmimEditLanguage.Enabled = enabled && CurrentLanguage != null;
        tsmiEditLanguage.Enabled = enabled && CurrentLanguage != null;
        tsmimDeleteLanguage.Enabled = enabled && CurrentLanguage != null;
        tsmiDeleteLanguage.Enabled = enabled && CurrentLanguage != null;
        // konverzia prechadza skupiny jazyka - len nacitany jazyk
        var convert = enabled && CurrentLanguage != null && _languageLoaded;
        tsmimConvertLangToEwa.Enabled = convert;
        tsmimConvertLangToWav.Enabled = convert;
        tsmiConvertLangToEwa.Enabled = convert;
        tsmiConvertLangToWav.Enabled = convert;
    }

    private void SwitchConvertSoundsLangButtons(bool enabled)
    {
        tsbConvertSoundsToEwa.Enabled = enabled;
        tsbConvertSoundsToWav.Enabled = enabled;
        cmiConvertSoundsToEwa.Enabled = enabled;
        cmiConvertSoundsToWav.Enabled = enabled;
        tsmimConvertSoundsToEwa.Enabled = enabled;
        tsmimConvertSoundsToWav.Enabled = enabled;
    }

    private void SwitchSearchButtons(bool enabled)
    {
        tsbSearch.Enabled = enabled;
        tsmimSearch.Enabled = enabled;
    }

    private void SwitchAddSoundButtons(bool enabled)
    {
        tsbAddSound.Enabled = enabled;
        tsmimAddSound.Enabled = enabled;
        cmiAddSound.Enabled = enabled;
    }

    private void SwitchDeleteSoundsButtons(bool enabled)
    {
        tsbDeleteSound.Enabled = enabled;
        tsmimDeleteSound.Enabled = enabled;
        cmiDeleteSound.Enabled = enabled;        
    }

    private void SwitchMoveSoundsButtons(bool enabled)
    {
        tsbMoveSounds.Enabled = enabled;
        tsmimMoveSounds.Enabled = enabled;
        cmiMoveSounds.Enabled = enabled;
    }

    private void SwitchRewriteModeButtons(bool enabled)
    {
        tsbRewriteMode.Enabled = enabled;
        tsmimRewriteMode.Enabled = enabled;
    }

    private void SwitchAddGroupButtons(bool enabled)
    {
        tsbAddGroup.Enabled = enabled;
        cmiAddGroup.Enabled = enabled;
    }

    private void SwitchEditGroupButtons(bool enabled)
    {
        tsbEditGroup.Enabled = enabled;
        cmiEditGroup.Enabled = enabled;
    }

    private void SwitchDeleteGroupButtons(bool enabled)
    {
        tsbDeleteGroup.Enabled = enabled;
        cmiDeleteGroup.Enabled = enabled;        
    }

    private void SwitchConvertGroupButtons(bool enabled)
    {
        tsbConvertGroupToEwa.Enabled = enabled;
        cmiConvertGroupToEwa.Enabled = enabled;
        tsbConvertGroupToWav.Enabled = enabled;
        cmiConvertGroupToWav.Enabled = enabled;
    }

    private void SwitchOpenInExplorerButtons(bool enabled)
    {
        tsbOpenInExplorer.Enabled = enabled;
        cmiOpenInExplorer.Enabled = enabled;
    }

    private void SwitchPlayButtons(bool enabled)
    {
        tsbPlay.Enabled = enabled;
        cmiPlay.Enabled = enabled;
    }

    private void SwitchRenameFileDirButtons(bool enabled)
    {
        tsbRenameFileDir.Enabled = enabled;
        cmiRenameFileDir.Enabled = enabled;
    }

    private void SwitchDeleteFileDirButtons(bool enabled)
    {
        tsbDeleteFileDir.Enabled = enabled;
        cmiDeleteFileDir.Enabled = enabled;
    }

    private void SwitchConvertFileButtons(bool enabled)
    {
        tsbConvertFilesToEwa.Enabled = enabled;
        cmiConvertToEwaFile.Enabled = enabled;
        tsbConvertFilesToWav.Enabled = enabled;
        cmiConvertToWavFile.Enabled = enabled;
    }

    private void SwitchHighlightProblemButtons(bool enabled)
    {
        tsbHighlightProblem.Enabled = enabled;
        cmiHighlightProblem.Enabled = enabled;        
    }

    private void SwitchSolveProblemButtons(bool enabled)
    {
        tsbResolveProblem.Enabled = enabled;
        cmiResolveProblem.Enabled = enabled;        
    }

    #endregion

    private void mmErrors_Click(object sender, EventArgs e) => splitSoundsErrors.Panel2Collapsed = true;

    private void tssbErrors_Click(object sender, EventArgs e) => splitSoundsErrors.Panel2Collapsed = !splitSoundsErrors.Panel2Collapsed;

    private void TsmimShowErrors_CheckedChanged(object sender, EventArgs e) => splitSoundsErrors.Panel2Collapsed = !tsmimShowErrors.Checked;

    private void FMain_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (GlobData.OpenedProject is null || Saved) 
            return;

        var result = Utils.ShowQuestion(Resources.FMain_Save_Changes, MessageBoxButtons.YesNoCancel);
        switch (result)
        {
            case DialogResult.Yes:
                // neuspesne ulozenie okno nezatvori - zmeny by sa stratili
                e.Cancel = !SaveBank(false);
                break;
            case DialogResult.No:
                e.Cancel = !ConfirmDiscardDiskChanges();
                break;
            default:
                e.Cancel = true;
                break;
        }
    }

    /// <summary>
    ///     Mala ikona systemu ako obrazok - vytvara sa raz, nie pri kazdom vykresleni bunky.
    /// </summary>
    private static Bitmap StockIcon(ShellIconType type)
    {
        using var icon = new ShellIcon(type, ShellIconSize.Small);
        return icon.ToBitmap();
    }

    private void FMain_FormClosed(object sender, FormClosedEventArgs e)
    {
        _error.Dispose();
        _warning.Dispose();
        _info.Dispose();
    }

    private void DgvGroups_KeyDown(object sender, KeyEventArgs e)
    {
        if (dgvGroups.IsSelectionEmpty())
            return;

        if (e.KeyCode == Keys.PageUp)
        {
            var sel = dgvGroups.SelectedRows[0].Index;
            var selectedObj = (FyzGroup)dgvGroups.SelectedRows[0].DataBoundItem!;

            if (sel - 1 >= 0)
            {
                _reorderingGroups = true;
                MenuGroups.RemoveAt(sel);
                MenuGroups.Insert(sel - 1, selectedObj);
                dgvGroups.ClearSelection();
                dgvGroups.Rows[sel - 1].Selected = true;
                _reorderingGroups = false;
                // poradie skupin sa zapisuje do FYZZVUK.DAT - zmena bez moznosti vratenia spat
                RegisterNewAction();
            }
        }
        else if (e.KeyCode == Keys.PageDown)
        {
            var sel = dgvGroups.SelectedRows[0].Index;
            var selectedObj = (FyzGroup)dgvGroups.SelectedRows[0].DataBoundItem!;

            if (sel + 1 < CurrentLanguage!.Groups.Count)
            {
                _reorderingGroups = true;
                MenuGroups.RemoveAt(sel);
                MenuGroups.Insert(sel + 1, selectedObj);
                dgvGroups.ClearSelection();
                dgvGroups.Rows[sel + 1].Selected = true;
                _reorderingGroups = false;
                // poradie skupin sa zapisuje do FYZZVUK.DAT - zmena bez moznosti vratenia spat
                RegisterNewAction();
            }
        }

        // ostatne klavesy (sipky, Home, End) patria tabulke - pohyb medzi skupinami
        e.Handled = e.KeyCode is Keys.PageUp or Keys.PageDown;
    }

    private void DgvGroups_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
    {
        if (e.RowIndex != -1 && e.Button.HasFlag(MouseButtons.Right)) 
            dgvGroups.Rows[e.RowIndex].Selected = true;
    }

    private async void fileSystemWatcher_Created(object sender, FileSystemEventArgs e)
    {
        // jazyk sa nenacital (chyba pri nacitani) - nie je k comu udalost priradit
        if (CurrentLanguage?.Directory is null)
            return;

        if (RawBankExplorer.ConvertSoundIsHandled || RawBankExplorer.MovingSoundIsHandled)
            return;

        var newElement = RawBankExplorer.GetElement(e.FullPath, CurrentLanguage!.Directory, RawBankExplorer.SearchOperation.Create);

        // subor sa vratil (napr. obnovenim z kosa) - patri zvuku, ktory ho ma v nazve suboru
        var owner = newElement is SoundFileElement returned && returned.Parent?.Group is { } ownerGroup
            ? ownerGroup.Sounds.FirstOrDefault(s => s.File == null && RawBankParser.AdditionalPathIsEmpty(s.AdditionalRelativePath)
                                                   && RawBankExplorer.EqualsPathNames(s.FileName ?? "", returned.Name))
            : null;
        if (owner is not null)
        {
            RelinkSoundFile(owner);
        }
        else if (newElement is SoundFileElement sfe && GlobData.Config.AutoInsertSoundData && sfe.Parent?.Group is not null)
        {
            var nameWoExt = Path.GetFileNameWithoutExtension(sfe.Name);
            var alreadyDefinedSound = sfe.Parent.Group.Sounds.FirstOrDefault(s => SoundRules.SameText(s.Key, nameWoExt) && s.File == null);
            if (alreadyDefinedSound is not null)
            {
                // zvuk bez suboru dostal svoj subor - prepojit oboma smermi, inak zoznam chyb hlasi SoundDataMissing
                alreadyDefinedSound.File = sfe;
                sfe.Sound = alreadyDefinedSound;
                // nahravka lezi priamo v priecinku skupiny a moze mat inu priponu, nez sa cakalo (.WAV/.EWA)
                alreadyDefinedSound.FileName = sfe.Name;
                alreadyDefinedSound.AdditionalRelativePath = "";
                sfe.Duration = await SoundUtils.GetSoundDuration(sfe);
                if (GlobData.Config.AutoRecalculateSoundDuration && sfe.Duration >= 0)
                    alreadyDefinedSound.Duration = sfe.Duration;
            }
            else
            {
                sfe.Duration = await SoundUtils.GetSoundDuration(sfe);
                var sound = new FyzSound(sfe.Parent.Group, nameWoExt, nameWoExt, sfe.Name, "", "", Math.Max(sfe.Duration, 0))
                {
                    File = sfe
                };
                sfe.Sound = sound;
                if (GlobData.Config.ShowAfterInsertSoundDialog)
                {
                    FAfterInsertSounds.CreateOrUseExistingForm(sound);
                }
                else if (SoundRules.ValidateNew([sound]).Count > 0)
                {
                    // kluc alebo nazov uz v skupine je (napr. 9900100.WAV k zvuku 9900100.EWA) - bez okna sa neda opravit,
                    // subor ostane bez udajov o zvuku a zoznam chyb ho ukaze ako nedefinovany
                    sfe.Sound = null!;
                }
                else
                {
                    Program.MainForm.RegisterNewAction(new AddSoundAction(Program.MainForm, sound));
                    sound.Group.Sounds.Add(sound);
                    RefreshSoundViews();
                }
            }
        }

        if (e.FullPath.StartsWith(CurrentDirectory.DirInfo.FullName, StringComparison.OrdinalIgnoreCase))
            FillExplorerList(CurrentDirectory);

        CheckProjectState();
        dgvGroups.ResetBindings();
        dgvSounds.ResetBindings();
    }

    private void fileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
    {
        // jazyk sa nenacital (chyba pri nacitani) - nie je k comu udalost priradit
        if (CurrentLanguage?.Directory is null)
            return;

        if (RawBankExplorer.ConvertSoundIsHandled || RawBankExplorer.MovingSoundIsHandled)
            return;

        RawBankExplorer.GetElement(e.FullPath, CurrentLanguage!.Directory, RawBankExplorer.SearchOperation.Delete);

        if (e.FullPath.StartsWith(CurrentDirectory.DirInfo.FullName, StringComparison.OrdinalIgnoreCase))
            FillExplorerList(CurrentDirectory);

        CheckProjectState();
        dgvGroups.ResetBindings();
        dgvSounds.ResetBindings();
    }

    private async void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
    {
        // jazyk sa nenacital (chyba pri nacitani) - nie je k comu udalost priradit
        if (CurrentLanguage?.Directory is null)
            return;

        var fileElement = RawBankExplorer.GetElement(e.FullPath, CurrentLanguage!.Directory);
        switch (fileElement)
        {
            case null:
                break;
            case DirectoryElement de:
                de.DirInfo = new DirectoryInfo(e.FullPath);
                break;
            case SoundFileElement sfe:
                if (RawBankExplorer.ConvertSoundIsHandled)
                    return;
                sfe.FileInfo = new FileInfo(e.FullPath);
                sfe.Duration = await SoundUtils.GetSoundDuration(sfe);
                if (GlobData.Config.AutoRecalculateSoundDuration && sfe.Sound is not null)
                    sfe.Sound.Duration = sfe.Duration;
                break;
            case FileElement fe:
                fe.FileInfo = new FileInfo(e.FullPath);
                break;
        }

        if (e.FullPath.StartsWith(CurrentDirectory.DirInfo.FullName, StringComparison.OrdinalIgnoreCase))
            FillExplorerList(CurrentDirectory);

        CheckProjectState();
        dgvGroups.ResetBindings();
        dgvSounds.ResetBindings();
    }

    private void fileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
    {
        // jazyk sa nenacital (chyba pri nacitani) - nie je k comu udalost priradit
        if (CurrentLanguage?.Directory is null)
            return;

        if (RawBankExplorer.ConvertSoundIsHandled)
            return;

        // premenovanie mimo programu (napr. v Prieskumnikovi Windows)
        ApplyRename(e.OldFullPath, e.FullPath);

        if (e.FullPath.StartsWith(CurrentDirectory.DirInfo.FullName, StringComparison.OrdinalIgnoreCase))
            FillExplorerList(CurrentDirectory);

        CheckProjectState();
        dgvGroups.ResetBindings();
        dgvSounds.ResetBindings();
    }

    private void dgvExplorer_CellClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex == -1 || !_rewriteMode)
            return;

        if (dgvSounds.SelectedRows.Count == 0 || dgvExplorer.Rows[e.RowIndex].DataBoundItem is not SoundFileElement sfe)
            return;

        if (dgvSounds.SelectedRows[0].DataBoundItem is FyzSound snd)
        {
            // povodny subor ostane bez udajov o zvuku
            if (snd.File?.Sound == snd)
                snd.File.Sound = null!;

            snd.File = sfe;
            snd.FileName = sfe.Name;
            sfe.Sound = snd;

            // pridavna cesta je v INISS relativna k priecinku skupiny (napr. ..\Poz1\), subor priamo v nom ju nema
            snd.AdditionalRelativePath = SoundRules.AdditionalPathFor(snd.Group, sfe.FileInfo.DirectoryName!, GlobData.OpenedProject!.AbsPathToBank);

            // prepisovaci mod nema akciu spat - zmena sa aspon oznaci ako neulozena
            RegisterNewAction();
        }

        dgvSounds.ResetBindings();
        dgvSounds.Refresh();
    }

    private void dgvExplorer_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
        if (dgvExplorer.IsSelectionEmpty())
            return;

        switch (dgvExplorer.SelectedRows[0].DataBoundItem)
        {
            case BackButtonElement b:
                var dirinfo = b.Parent;
                if (dirinfo != null)
                    FillExplorerList(dirinfo);
                break;
            case SoundFileElement sf:
                SoundUtils.Play(sf.FileInfo.FullName);
                break;
            case DirectoryElement de:
                FillExplorerList(de);
                break;
        }
    }

    private void dgvSounds_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
    {
        var newAdditionalPath = (string)dgvSounds.Rows[e.RowIndex].Cells[nameof(cSoundAdditionalRelativePath)].Value!;
        if (!string.IsNullOrEmpty(newAdditionalPath) && (!newAdditionalPath.EndsWith("\\") || string.IsNullOrWhiteSpace(newAdditionalPath)))
        {
            Utils.ShowError("Prídavná relatívna cesta musí končiť '\\' a nesmie obsahovať iba prázdne znaky.");
            e.Cancel = true;
            dgvSounds.Rows[e.RowIndex].Cells[nameof(cSoundAdditionalRelativePath)].Value = "";
            return;
        }
        ValidateRow(e.RowIndex, true);
    }

    private void dgvSounds_DataError(object sender, DataGridViewDataErrorEventArgs e) => Utils.ShowError("Tabuľka obsahuje nesprávny údaj: " + e.Exception!.Message);

    private void dgvSounds_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
    {
        _cellUserEditing = true;
        var val = dgvSounds.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
        _cellOldValue = val == null ? "" : val.ToString() ?? "";
    }

    /// <summary>
    ///     Prepoji zvuk so suborom podla jeho nazvu suboru a pridavnej cesty (po uprave v tabulke, spat a znovu)
    ///     a zisti dlzku nahravky. Povodny subor ostane bez udajov o zvuku.
    /// </summary>
    internal async void RelinkSoundFile(FyzSound sound)
    {
        if (sound.File?.Sound == sound)
            sound.File.Sound = null!;
        sound.File = null!;

        var sfe = SoundUtils.FindSoundFile(sound, GlobData.OpenedProject!.AbsPathToBank);
        if (sfe is null)
            return;

        sound.File = sfe;
        sfe.Sound = sound;
        if (sfe.Duration < 0)
            sfe.Duration = await SoundUtils.GetSoundDuration(sfe);
        if (GlobData.Config.AutoRecalculateSoundDuration && sfe.Duration >= 0)
            sound.Duration = sfe.Duration;
        CheckProjectState();
    }

    /// <summary>
    ///     Kluc a nazov zvuku su povinne a v skupine jedinecne - rovnako ako v okne Pridat zvuk.
    /// </summary>
    private void DgvSounds_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
    {
        if (!dgvSounds.IsCurrentCellInEditMode || e.RowIndex < 0 || dgvSounds.Rows[e.RowIndex].DataBoundItem is not FyzSound sound)
            return;

        var column = dgvSounds.Columns[e.ColumnIndex].Name;
        if (column is not (nameof(cSoundKey) or nameof(cSoundName)))
            return;

        var value = e.FormattedValue as string ?? "";
        var isKey = column == nameof(cSoundKey);
        string? error = null;
        if (string.IsNullOrWhiteSpace(value))
            error = isKey ? "Kľúč zvuku je povinný." : "Názov zvuku je povinný.";
        else if (sound.Group.Sounds.Any(s => s != sound && SoundRules.SameText(isKey ? s.Key : s.Name, value)))
            error = isKey ? $"Kľúč {value} už v skupine {sound.Group.Name} existuje." : $"Názov {value} už v skupine {sound.Group.Name} existuje.";

        if (error is null)
            return;

        Utils.ShowError(error + "\n\nOpravte hodnotu alebo úpravu zrušte klávesom Esc.");
        e.Cancel = true;
    }

    /// <summary>
    ///     Kazda upravena bunka je samostatny krok Spat - so stlpcom a hodnotou prave tejto bunky.
    /// </summary>
    private void dgvSounds_CellEndEdit(object sender, DataGridViewCellEventArgs e)
    {
        if (!_cellUserEditing)
            return;

        _cellUserEditing = false;
        var newval = dgvSounds.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
        var s = newval as string ?? "";
        if (s == _cellOldValue)
            return;

        var action = new EditSoundAction(this, (FyzSound)dgvSounds.Rows[e.RowIndex].DataBoundItem!);
        action.Type = dgvSounds.Columns[e.ColumnIndex].Name switch
        {
            nameof(cSoundKey) => PropertyType.Key,
            nameof(cSoundName) => PropertyType.Name,
            nameof(cSoundAdditionalRelativePath) => PropertyType.RelativePath,
            nameof(cSoundFileName) => PropertyType.FileName,
            nameof(cSoundText) => PropertyType.Text,
            _ => action.Type
        };

        action.OldValue = _cellOldValue;
        action.NewValue = s;

        RegisterNewAction(action);
        CheckProjectState();
    }

    private void dgvExplorer_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex == -1)
            return;

        if (dgvExplorer.Columns[nameof(cFileType)] != null && e.ColumnIndex == dgvExplorer.Columns[nameof(cFileType)]!.Index)
        {
            e.Value = dgvExplorer.Rows[e.RowIndex].DataBoundItem switch
            {
                BackButtonElement => GlobalResources.back,
                DirectoryElement => GlobalResources.open,
                SoundFileElement => GlobalResources.sound,
                _ => GlobalResources.file
            };
        }
        else if (dgvExplorer.Columns[nameof(cFileDuration)] != null && e.ColumnIndex == dgvExplorer.Columns[nameof(cFileDuration)]!.Index)
        {
            var item = dgvExplorer.Rows[e.RowIndex].DataBoundItem as FileSystemElement;
            e.Value = item switch
            {
                SoundFileElement se => se.DurationText,
                DirectoryElement de => de.Children.Count+ " položiek",
                _ => null
            };
        }
    }

    private void dgvExplorer_SelectionChanged(object sender, EventArgs e)
    {
        if (dgvExplorer.SelectedRows.Count == 1)
        {
            var item = dgvExplorer.SelectedRows[0].DataBoundItem;

            SwitchOpenInExplorerButtons(true);
            SwitchPlayButtons(item is SoundFileElement);
            SwitchRenameFileDirButtons(item is not BackButtonElement);
            SwitchDeleteFileDirButtons(item is not BackButtonElement);
            SwitchConvertFileButtons(item is SoundFileElement);
        }
        else if (dgvExplorer.IsSelectionEmpty())
        {
            SwitchOpenInExplorerButtons(false);
            SwitchPlayButtons(false);
            SwitchRenameFileDirButtons(false);
            SwitchDeleteFileDirButtons(false);
            SwitchConvertFileButtons(false);
        }
        else
        {
            var containsBackBtn = dgvExplorer.SelectedRows.Cast<DataGridViewRow>().FirstOrDefault(r => r.DataBoundItem == _explorerBack) != null;
            SwitchOpenInExplorerButtons(true);
            SwitchPlayButtons(false);
            SwitchRenameFileDirButtons(false);
            SwitchDeleteFileDirButtons(!containsBackBtn);
            SwitchConvertFileButtons(true);
        }
    }

    private async void ValidateRow(int row, bool userEditing = false)
    {
        if (_deletingRows || row == dgvSounds.NewRowIndex || CurrentGroup == null || CurrentLanguage is null)
            return;

        dgvSounds.Rows[row].Cells[nameof(cSoundFileName)].ErrorText = null;
        var newFileName = (string)dgvSounds.Rows[row].Cells[nameof(cSoundFileName)].Value!;
        var sound = (FyzSound)dgvSounds.Rows[row].DataBoundItem!;

        if (string.IsNullOrWhiteSpace(newFileName) || !Utils.IsFileNameCorrect(sound.GetAbsPath(GlobData.OpenedProject!.AbsPathToBank), newFileName, false))
        {
            if (sound.File is not null)
                sound.File.Sound = null!;
            sound.File = null!;
            dgvSounds.Rows[row].Cells[nameof(cSoundFileName)].ErrorText = "Neplatný názov súboru";
            return;
        }

        if (userEditing)
        {
            // zmena nazvu suboru alebo pridavnej cesty zvuk len prepoji na iny subor - subor na disku sa nepremenuva
            RelinkSoundFile(sound);
            if (sound.File is not null)
                return;

            var ext = Path.GetExtension(newFileName);
            dgvSounds.Rows[row].Cells[nameof(cSoundFileName)].ErrorText =
                ext.EqualsIgnoreCase(SoundUtils.WAV_EXT) || ext.EqualsIgnoreCase(SoundUtils.EWA_EXT)
                    ? "Súbor zvuku v priečinku neexistuje"
                    : "Neplatný typ súboru";
            return;
        }

        if (sound.File == null || !sound.File.FileInfo.Exists)
        {
            dgvSounds.Rows[row].Cells[nameof(cSoundFileName)].ErrorText = "Súbor zvuku v priečinku neexistuje";
        }
    }

    internal async void FillExplorerList(DirectoryElement? dir)
    {
        var generation = ++_explorerFillGeneration;
        ExplorerContent.Clear();
        _explorerBack = null;

        //skupina neexistuje v suborovom systeme
        if (dir is null)
        {
            _explorerBack = new BackButtonElement(CurrentDirectory);
            ExplorerContent.Add(_explorerBack);
            return;
        }

        CurrentDirectory = dir;

        if (dir.DirInfo.Exists)
        {
            if (!IsRootPath(dir))
            {
                _explorerBack = new BackButtonElement(dir.Parent!);
                ExplorerContent.Add(_explorerBack);
            }

            // kopia - pocas cakania na dlzky moze sledovanie suborov do priecinka pridat alebo z neho odobrat prvok
            var children = dir.Children.ToList();
            foreach (var element in children)
                ExplorerContent.Add(element);
            dgvExplorer.Refresh();
        }

        if (SelectElement != null)
        {
            var index = ExplorerContent.IndexOf(SelectElement);
            if (index != -1)
            {
                dgvExplorer.ClearSelection();
                dgvExplorer.Rows[index].Selected = true;
            }
            SelectElement = null;
        }

        // dlzky nahravok az po naplneni zoznamu; ked sa medzitym zobrazi iny priecinok, dalsie sa uz nepocitaju
        foreach (var sound in dir.Children.OfType<SoundFileElement>().Where(s => s.Duration == -1).ToList())
        {
            sound.Duration = await SoundUtils.GetSoundDuration(sound);
            // okno sa medzitym mohlo zatvorit
            if (generation != _explorerFillGeneration || IsDisposed)
                return;
            dgvExplorer.InvalidateColumn(cFileDuration.Index);
        }
    }

    // pocitadlo volani FillExplorerList - starsie volanie po await zisti, ze prieskumnik uz ukazuje nieco ine
    private int _explorerFillGeneration;

    private void tsbErrors_CheckedChanged(object sender, EventArgs e) => ShowHideMessages();

    private void tsbWarnings_CheckedChanged(object sender, EventArgs e) => ShowHideMessages();

    private void tsbInfos_CheckedChanged(object sender, EventArgs e) => ShowHideMessages();

    private void ShowHideMessages()
    {
        if (dgvErrors.DataSource is null)
            return;

        var showErrors = tsbErrors.Checked;
        var showWarnings = tsbWarnings.Checked;
        var showInfos = tsbInfos.Checked;

        //https://stackoverflow.com/questions/18942017/unable-to-set-row-visible-false-of-a-datagridview
        var currencyManager = (CurrencyManager)BindingContext![dgvErrors.DataSource]!;
        currencyManager.SuspendBinding();
            
        foreach (DataGridViewRow row in dgvErrors.Rows)
        {
            if (row.DataBoundItem is IRawBankMessage message)
            {
                if (showErrors && message.Type == MessageType.Error)
                    row.Visible = true;
                else if (showWarnings && message.Type == MessageType.Warning)
                    row.Visible = true;
                else if (showInfos && message.Type == MessageType.Info)
                    row.Visible = true;
                else
                    row.Visible = false;
            }
        }

        currencyManager.ResumeBinding();
    }

    private void dgvErrors_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
        if (cMsgType.Index == e.ColumnIndex)
        {
            if (dgvErrors.Rows[e.RowIndex].DataBoundItem is IRawBankMessage msg)
            {
                switch (msg.Type)
                {
                    case MessageType.Info:
                        e.Value = _info;
                        dgvErrors.Rows[e.RowIndex].Cells[nameof(cMsgType)].ToolTipText = "Informácia";
                        break;
                    case MessageType.Warning:
                        e.Value = _warning;
                        dgvErrors.Rows[e.RowIndex].Cells[nameof(cMsgType)].ToolTipText = "Upozornenie";
                        break;
                    case MessageType.Error:
                        e.Value = _error;
                        dgvErrors.Rows[e.RowIndex].Cells[nameof(cMsgType)].ToolTipText = "Chyba";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    private void DgvErrors_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex == -1)
            return;

        ((IRawBankMessage)dgvErrors.Rows[e.RowIndex].DataBoundItem!).Show();
    }

    private void ChangeManager_UndoRedoStateChanged(object sender, UndoRedoStateEventArgs e)
    {
        Saved = changeManager.IsInSavedState() && !_unUndoableUnsavedChanges;
        if (e.NewState != UndoRedoState.Clear) 
            CheckProjectState();
    }

    private bool IsRootPath(DirectoryElement dir)
    {
        return string.Compare(
            Path.GetFullPath(dir.DirInfo.FullName).TrimEnd('\\'),
            Path.GetFullPath(GlobData.OpenedProject!.AbsPathToBank + CurrentLanguage!.RelativePath).TrimEnd('\\'),
            StringComparison.InvariantCultureIgnoreCase) == 0;
    }

    // 1 chyba, 2 - 4 chyby, 0 a 5+ chyb
    private static string CountText(int count, string one, string few, string many)
        => $"{count} {(count == 1 ? one : count is >= 2 and <= 4 ? few : many)}";

    private void Messages_ListChanged(object? sender, ListChangedEventArgs e)
    {
        var errorCount = 0;
        var warningCount = 0;
        var infoCount = 0;

        foreach (var message in _messages)
        {
            switch (message.Type)
            {
                case MessageType.Info:
                    infoCount++;
                    break;
                case MessageType.Warning:
                    warningCount++;
                    break;
                case MessageType.Error:
                    errorCount++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        Invoke(() =>
        {
            tsbErrors.Text = CountText(errorCount, "chyba", "chyby", "chýb");
            tsbWarnings.Text = CountText(warningCount, "upozornenie", "upozornenia", "upozornení");
            tsbInfos.Text = CountText(infoCount, "správa", "správy", "správ");

            if (errorCount != 0)
            {
                tssbErrors.Image = _error;
                tssbErrors.Text = CountText(errorCount, "chyba", "chyby", "chýb");
                tssbErrors.ForeColor = Color.Red;
            }
            else
            {
                tssbErrors.Image = GlobalResources.correct;
                tssbErrors.Text = "";
                tssbErrors.ForeColor = GlobData.UsingStyle.ControlsColorScheme.Panel.ForeColor;
            }
        });
    }

    internal void SelectLanguage(FyzLanguage language)
    {
        tscboxLanguages.SelectedItem = language;
    }

    internal int SelectGroup(FyzGroup group)
    {
        SelectLanguage(group.Language);
        var index = MenuGroups.IndexOf(group);
        if (index != -1)
        {
            dgvGroups.Rows[index].Selected = true;
            EnsureVisibleRow(dgvGroups, index);
        }
        
        return index;
    }

    internal int SelectSound(FyzSound sound)
    {
        SelectGroup(sound.Group);
        var index = MenuSounds.IndexOf(sound);
        if (index != -1)
        {
            dgvSounds.Rows[index].Selected = true;
            EnsureVisibleRow(dgvSounds, index);
        }

        return index;
    }

    private void DgvExplorer_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyData == Keys.F2) 
            DoRenameFile(dgvExplorer, EventArgs.Empty);
        if (e.KeyData == Keys.Delete) 
            DoDeleteFile(dgvExplorer, EventArgs.Empty);
        if (e.KeyData == Keys.F5) 
            DoPlayFile(dgvExplorer, EventArgs.Empty);
    }

    private void TimerToCheck_Tick(object sender, EventArgs e)
    {
        timerToCheck.Stop();
        timerToCheck.Enabled = false;

        // jazyk sa nenacital (chyba pri nacitani) - nie je co porovnavat
        if (CurrentLanguage is null || GlobData.OpenedProject?.Messages.ContainsKey(CurrentLanguage) != true)
            return;

        RawBankExplorer.MergeFilesAndData(Root, CurrentLanguage, GlobData.OpenedProject.Messages, true);

        _messages.Clear();
        try
        {
            foreach (var msg in GlobData.OpenedProject!.Messages[CurrentLanguage!])
            {
                _messages.Add(msg);
            }
        }
        catch (InvalidOperationException)
        {
            //ignored
        }

        // nove riadky su viditelne - skryte typy sprav sa musia skryt znova
        ShowHideMessages();
    }

    private void DgvSounds_MouseDown(object sender, MouseEventArgs e)
    {
        _selectingMoreRows = true;
    }

    private void DgvSounds_MouseUp(object sender, MouseEventArgs e)
    {
        _selectingMoreRows = false;
        dgvSounds_SelectionChanged(this, EventArgs.Empty);
    }

    private void CheckProjectState()
    {
        if (timerToCheck.Enabled)
            return;
        timerToCheck.Enabled = true;
        timerToCheck.Start();
    }

    private static void EnsureVisibleRow(DataGridView view, int rowToShow)
    {
        if (rowToShow >= 0 && rowToShow < view.RowCount)
        {
            var countVisible = view.DisplayedRowCount(false);
            var firstVisible = view.FirstDisplayedScrollingRowIndex;
            if (rowToShow < firstVisible)
            {
                view.FirstDisplayedScrollingRowIndex = rowToShow;
            }
            else if (rowToShow >= firstVisible + countVisible)
            {
                view.FirstDisplayedScrollingRowIndex = rowToShow - countVisible + 1;
            }
        }
    }
}