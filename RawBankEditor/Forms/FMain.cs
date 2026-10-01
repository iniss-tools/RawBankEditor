using ExControls;
using ExControls.Providers;
using RawBankEditor.Entities;
using RawBankEditor.Properties;
using RawBankEditor.Services;
using RawBankEditor.Tools;
using System.Globalization;
using ToolsCore;
using ToolsCore.Forms;
using ToolsCore.Iniss.Entities;
using ToolsCore.Iniss.Tools;
using ToolsCore.Tools;
using ToolsCore.XML;

namespace RawBankEditor.Forms;

public partial class FMain : Form
{
    private readonly ExBindingList<IRawBankMessage> _messages = [];

    // otvorena banka, nastavenia programu a zurnal zmien na disku
    private readonly BankEditor _bank;
    private readonly IDialogService _dialogs;

    //ikony
    private readonly Bitmap _error, _warning, _info;

    private BackButtonElement? _explorerBack;

    private string _cellOldValue = null!;
    private string _actualStatusTxt = null!;
    private bool _langAlreadySet;
    // jazyk, ktory sa nacitava na pozadi (ReadLanguage)
    private FyzLanguage? _loadingLanguage;
    // prave sa nacitava jazyk
    private bool _readingLanguage;
    // ci sa CurrentLanguage nacital bez chyby
    private bool _languageLoaded;
    // prave bezi spat/znovu - prepnutie jazyka z akcie sa nepyta na ulozenie a nemaze historiu
    private bool _inUndoRedo;
    // prepnutie pri spat/znovu pocas nacitania ineho jazyka - plati aj pre odlozene nacitanie
    private bool _deferredFromUndoRedo;
    private bool _saved = true;
    private bool _unUndoableUnsavedChanges;
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

    internal ExBindingList<FileSystemElement> ExplorerContent { get; } = [];

    /// <summary>
    /// Priecinok RAWBANK otvorenej banky.
    /// </summary>
    internal string PathToBank => _bank.PathToBank;

    /// <param name="bank">otvorena banka a nastavenia programu</param>
    /// <param name="dialogs">dialogy s hlasenim</param>
    internal FMain(BankEditor bank, IDialogService dialogs)
    {
        _bank = bank;
        _dialogs = dialogs;
        InitializeComponent();
        tsslStatus.Font = _bank.Config.Fonts.StateRow;

        switch (_bank.Config.DesktopMenuMode)
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
                throw new UnreachableException();
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

        CreateCommands();
        _commands.ApplyShortcuts(_bank.Config.Shortcuts);
        InitColumns();
        SetColumnsAutoWidth();
        
        dgvSounds.RowHeadersVisible = _bank.Config.ShowRowsHeader;

        //neviem preco niekedy umiestni stlpec s typom do stredu
        if (cFileType.DisplayIndex != 0) 
            cFileType.DisplayIndex = 0;

        if (_bank.UsingStyle.HighlightStatusBar)
        {
            statusStripMain.BackColor = _bank.UsingStyle.ControlsColorScheme.Highlight.BackColor;
            tsslStatus.ForeColor = _bank.UsingStyle.ControlsColorScheme.Highlight.ForeColor;
            tssbErrors.BackColor = _bank.UsingStyle.ControlsColorScheme.Highlight.BackColor;
        }

        UpdateCommandStates();
    }


    private void InitColumns()
    {
        static void SetCol(DataGridViewColumn column, DesktopColumn format)
        {
            column.Visible = format.Visible;
            column.MinimumWidth = format.MinWidth;
            column.DisplayIndex = format.Order;
        }
        
        var columns = _bank.Config.DesktopCols;

        SetCol(cSoundKey, columns.Key);
        SetCol(cSoundName, columns.Name);
        SetCol(cSoundAdditionalRelativePath, columns.RelativePath);
        SetCol(cSoundFileName, columns.FileName);
        SetCol(cSoundDuration, columns.Duration);
        SetCol(cSoundText, columns.Text);

        dgvSounds.Columns[dgvSounds.Columns.Count - 1].AutoSizeMode = _bank.Config.FitLastColumn
            ? DataGridViewAutoSizeColumnMode.Fill
            : DataGridViewAutoSizeColumnMode.None;

        tsmimWrapTextSoundCol.Checked = _bank.Config.WrapSoundText;
    }

    private void SetSoundTextColumn()
    {
        dgvSounds.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        cSoundText.DefaultCellStyle.WrapMode = _bank.Config.WrapSoundText ? DataGridViewTriState.True : DataGridViewTriState.NotSet;
    }

    private void UpdateMainUI()
    {
        var menu = _bank.Config.DesktopMenuMode;
        tsslStatus.Font = _bank.Config.Fonts.StateRow;
        menuStripMain.Visible = menu is DesktopMenu.MsTs or DesktopMenu.MsOnly;
        toolStripMain.Visible = menu is DesktopMenu.MsTs or DesktopMenu.TsOnly;
        dgvSounds.RowHeadersVisible = _bank.Config.ShowRowsHeader;

        this.ApplyThemeAndFonts();
        InitColumns();
        _commands.ApplyShortcuts(_bank.Config.Shortcuts);
        SetColumnsAutoWidth();
        Invalidate(true);
        AppInit.MsgBoxStyleInit(_bank.UsingStyle, _bank.Config);
        if (_bank.UsingStyle.HighlightStatusBar)
        {
            statusStripMain.BackColor = _bank.UsingStyle.ControlsColorScheme.Highlight.BackColor;
            tsslStatus.ForeColor = _bank.UsingStyle.ControlsColorScheme.Highlight.ForeColor;
            tssbErrors.BackColor = _bank.UsingStyle.ControlsColorScheme.Highlight.BackColor;
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
        
        tsmimShowErrors.Checked = _bank.Config.ShowErrorsWindow;
        splitSoundsErrors.Panel2.VisibleChanged += (_, _) =>
        {
            _bank.Config.ShowErrorsWindow = !splitSoundsErrors.Panel2Collapsed;
            XmlSerialization.WriteData(PathUtils.CombinePath(configsDir, ToolsCore.FileConsts.FileConfig)!, _bank.Config);
        };

        if (_bank.Config.LeftPanelWidth != -1) splitContainer1.SplitterDistance = _bank.Config.LeftPanelWidth;
        splitContainer1.SplitterMoved += (_, _) =>
        {
            _bank.Config.LeftPanelWidth = splitContainer1.SplitterDistance;
            XmlSerialization.WriteData(PathUtils.CombinePath(configsDir, ToolsCore.FileConsts.FileConfig)!, _bank.Config);
        };

        if (_bank.Config.GroupPanelWidth != -1) splitContainer2.SplitterDistance = _bank.Config.GroupPanelWidth;
        splitContainer2.SplitterMoved += (_, _) =>
        {
            _bank.Config.GroupPanelWidth = splitContainer2.SplitterDistance;
            XmlSerialization.WriteData(PathUtils.CombinePath(configsDir, ToolsCore.FileConsts.FileConfig)!, _bank.Config);
        };

        if (_bank.Config.ErrorPanelWidth != -1) splitSoundsErrors.SplitterDistance = splitSoundsErrors.Width - _bank.Config.ErrorPanelWidth;
        splitSoundsErrors.SplitterMoved += (_, _) =>
        {
            _bank.Config.ErrorPanelWidth = splitSoundsErrors.Width - splitSoundsErrors.SplitterDistance;
            XmlSerialization.WriteData(PathUtils.CombinePath(configsDir, ToolsCore.FileConsts.FileConfig)!, _bank.Config);
        };

        AppRegistry.RegisterJumpList();

        //cesta k projektu zadana ako argument ma prednost pred poslednym otvorenym projektom
        var path = Utils.GetProjectPathFromArgs();
        if (path is null && _bank.Config.Startup == StartupType.LastProject)
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

    private void ChangeStatusReady() => tsslStatus.Text = Resources.FMain_Status_Ready;

    /// <summary>
    /// Nacita banku a zacne nacitavat vybrany jazyk. Otvorena banka sa nahradi az ked je nova nacitana
    /// a jazyk vybrany - pri chybe alebo zruseni vyberu jazyka ostane otvorena povodna.
    /// </summary>
    /// <returns><c>true</c>, ak sa banka otvorila.</returns>
    private bool PrepareGlobalData(string dirpath)
    {
        // zmeny na disku sa zahodia az pri naozajstnom otvoreni novej banky - pri chybe ostava povodna aj so zmenami
        var discard = false;
        if (_bank.Project is not null && !Saved)
        {
            var result = _dialogs.ShowQuestion(Resources.FMain_Save_Changes, MessageBoxButtons.YesNoCancel);
            switch (result)
            {
                case DialogResult.Yes:
                    if (!SaveBank(false))
                        return false;
                    break;
                case DialogResult.No:
                    discard = true;
                    break;
                default:
                    return false;
            }
        }

        RawBankProject? project = null;
        if (_bank.Config.DebugModeGUI != DebugMode.AppCrash)
            try
            {
                project = RawBankProject.Load(dirpath);
            }
            catch (Exception exception)
            {
                Log.Exception(exception);

                switch (_bank.Config.DebugModeGUI)
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
            project = RawBankProject.Load(dirpath);

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

        CloseBank(discard);
        var leftovers = _bank.Open(project);
        if (leftovers.Count > 0)
            _dialogs.ShowWarning(string.Format(CultureInfo.CurrentCulture, Resources.FMain_JournalLeftovers, ListOf(leftovers)));
        dgvErrors.DataSource = null;
        Text = @$"{Application.ProductName} - {_bank.Project!.AbsPathToINISS}";

        _langAlreadySet = true;
        tscboxLanguages.ComboBox.DataSource = _bank.Project!.Languages;
        _langAlreadySet = false;
        SetComboLanguage(lang);

        moveManager.Clear();
        ResetHistory(false);
        LoadLanguage(lang);
        return true;
    }

    /// <summary>
    /// Otvori jazyk: zoznam zvukov a subory banky sa nacitaju na pozadi. Bez jazyka (banka nema ziadny) sa okno vycisti.
    /// </summary>
    private void LoadLanguage(FyzLanguage? lang)
    {
        CurrentLanguage = lang;
        _languageLoaded = false;
        // prieskumnik sa nacitava znova - sleduje sa az po nacitani (OnLanguageRead)
        fileSystemWatcher.EnableRaisingEvents = false;
        UpdateCommandStates();

        if (lang is null)
        {
            ShowNoLanguage();
            return;
        }

        // nacitava sa iny jazyk - po dokonceni sa nacita tento (OnLanguageRead)
        if (_readingLanguage)
        {
            _deferredFromUndoRedo = _inUndoRedo;
            return;
        }

        // zvuky sa citaju z disku okrem noveho jazyka (FYZZVUK.DAT este nema) a prepnutia pri spat/znovu,
        // kde plati stav v pamati, na ktory sa akcie odkazuju
        var keepMemory = _inUndoRedo || _deferredFromUndoRedo;
        _deferredFromUndoRedo = false;
        var readFile = !_bank.IsNew(lang) && !(keepMemory && lang.Groups is not null);
        if (!readFile)
            lang.Groups ??= [];

        _loadingLanguage = lang;
        tscboxLanguages.Enabled = false;
        tspbProgress.Visible = true;
        tspbProgress.Style = ProgressBarStyle.Marquee;
        ChangeStatus(Resources.FMain_Status_LoadingFiles);
        ReadLanguage(lang, readFile);
    }

    /// <summary>
    /// Vymaze historiu zmien (akcie sa odkazuju na data jazyka, ktore sa znova nacitaju).
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
    /// Zisti, ci sa zoznam jazykov lisi od ulozeneho FYZBANK.DAT.
    /// </summary>
    private bool LanguageListChanged() => _bank.LanguageListChanged();

    /// <summary>
    /// Pred odchodom z otvoreneho jazyka sa spyta na neulozene zmeny jeho zoznamu zvukov:
    /// Ano ich ulozi, Nie zahodi (jazyk sa pri dalsom otvoreni nacita z disku).
    /// </summary>
    /// <returns><c>false</c>, ak pouzivatel odchod zrusil.</returns>
    private bool ConfirmLeaveLanguage()
    {
        var lang = CurrentLanguage;
        if (lang is null || !_languageLoaded || Saved || !_bank.SoundsChanged(lang))
            return true;

        var result = _dialogs.ShowQuestion(string.Format(CultureInfo.CurrentCulture, Resources.FMain_Language_Unsaved, lang.Name), MessageBoxButtons.YesNoCancel);
        switch (result)
        {
            case DialogResult.Yes:
                return SaveBank(false);
            case DialogResult.No:
                DiscardLanguage(lang);
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Zahodi zmeny zvukov jazyka: jeho zmeny na disku sa vratia a zoznam zvukov sa pri dalsom otvoreni nacita
    /// z disku, novy jazyk bude prazdny.
    /// </summary>
    private void DiscardLanguage(FyzLanguage lang)
    {
        List<string> errors = [];
        WithoutFileWatcher(() => errors = _bank.DiscardLanguage(lang));
        ReportRollbackErrors(errors);
    }

    /// <summary>
    /// Zatvori otvorenu banku: zmeny na disku potvrdi (ulozena banka), alebo ich pri zahodeni zmien vrati.
    /// </summary>
    private void CloseBank(bool discard)
    {
        if (_bank.Project is null)
            return;

        List<string> errors = [];
        WithoutFileWatcher(() => errors = _bank.Close(discard));
        if (discard)
            ReportRollbackErrors(errors);
        else if (errors.Count > 0)
            _dialogs.ShowWarning(string.Format(CultureInfo.CurrentCulture, Resources.FMain_RecycleFailed, ListOf(errors)));
    }

    private void ReportRollbackErrors(List<string> errors)
    {
        if (errors.Count > 0)
            _dialogs.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.FMain_RollbackFailed, ListOf(errors)));
    }

    /// <summary>
    /// Zoznam poloziek do hlasenia - najviac 10 riadkov.
    /// </summary>
    private static string ListOf(IReadOnlyCollection<string> items)
        => string.Join("\n", items.Take(10)) + (items.Count > 10 ? "\n" + string.Format(CultureInfo.CurrentCulture, Resources.Convert_More, items.Count - 10) : "");

    private void SetComboLanguage(FyzLanguage? lang)
    {
        _langAlreadySet = true;
        tscboxLanguages.SelectedItem = lang;
        _langAlreadySet = false;
    }

    /// <summary>
    /// Naplní menu naposledy otvorenými projektmi zoradenými od naposledy otvoreného.
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
        if (keyData == _commands[RbeCommands.DeleteSounds.Id].ShortcutKeys && keyData != Keys.None && !dgvSounds.ContainsFocus)
            return false;

        // skratky prikazov - funguju aj pri skrytej ponuke a pri polozkach kontextovych ponuk
        if (_commands.ProcessShortcut(keyData))
            return true;

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
    /// Otvorí projekt a zapíše ho do zoznamu naposledy otvorených projektov.
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

    private void DoOpenDir()
    {
        var dialog = new FolderBrowserDialog { Description = Resources.FMain_Vyberte_priecinok_s_INISS };
        if (dialog.ShowDialog(this) == DialogResult.Cancel)
            return;

        OpenProject(dialog.SelectedPath);
    }

    /// <summary>
    /// Nacita jazyk na pozadi: subory banky, FYZZVUK.DAT (ak <paramref name="readFile" />) a ich spojenie.
    /// Priebeh sa ukazuje v stavovom riadku, vysledok spracuje <see cref="OnLanguageRead" />.
    /// </summary>
    private async void ReadLanguage(FyzLanguage lang, bool readFile)
    {
        _readingLanguage = true;
        var project = _bank.Project!;
        var progress = new Progress<ProgressStatus>(ShowLoadProgress);
        Exception? error = null;
        try
        {
            Root = await Task.Run(() => ReadLanguage(project, lang, readFile, progress));
        }
        catch (Exception exception) when (_bank.Config.DebugModeGUI != DebugMode.AppCrash)
        {
            error = exception;
        }
        finally
        {
            _readingLanguage = false;
        }

        OnLanguageRead(error);
    }

    private static DirectoryElement ReadLanguage(RawBankProject project, FyzLanguage lang, bool readFile, IProgress<ProgressStatus> progress)
    {
        // subory banky
        progress.Report(new ProgressStatus(Resources.FMain_Progress_FileSystem, 0));
        var root = RawBankExplorer.ExploreFileSystem(project.AbsPathToBank);

        // zoznam zvukov jazyka z FYZZVUK.DAT
        if (readFile)
            RawBankParser.ReadFyzZvukFile(project.AbsPathToBank, lang, progress);

        // spojenie suborov so zoznamom zvukov
        progress.Report(new ProgressStatus(Resources.FMain_Progress_Merging, 0));
        RawBankExplorer.MergeFilesAndData(root, lang, project.Messages, project.AbsPathToBank);
        return root;
    }

    private void ShowLoadProgress(ProgressStatus status)
    {
        if (status.TotalProgress < 1)
        {
            tsslStatus.Text = status.ProgressPartName;
            tspbProgress.Style = ProgressBarStyle.Marquee;
            return;
        }

        tspbProgress.Style = ProgressBarStyle.Continuous;
        tspbProgress.Maximum = status.TotalProgress;
        tspbProgress.Value = Math.Clamp(status.Value, 0, status.TotalProgress);
        tsslStatus.Text = $@"{status.ProgressPartName} ({(int)(status.Value / (float)status.TotalProgress * 100)}%)";
    }

    private void OnLanguageRead(Exception? error)
    {
        tspbProgress.Visible = false;

        // pocas nacitania sa otvoril iny jazyk (spat/znovu, odstranenie jazyka) - vysledok neplati, nacita sa ten
        if (!ReferenceEquals(_loadingLanguage, CurrentLanguage))
        {
            if (error != null && _loadingLanguage != null)
                _bank.ForgetLanguage(_loadingLanguage);
            _loadingLanguage = null;
            LoadLanguage(CurrentLanguage);
            return;
        }

        if (error != null)
        {
            // ciastocne nacitane skupiny sa nesmu zapisat cez Ulozit vsetko
            _bank.ForgetLanguage(CurrentLanguage!);
            Log.Exception(error);

            ChangeStatus(Resources.FMain_Status_LoadFailed);

            switch (_bank.Config.DebugModeGUI)
            {
                case DebugMode.OnlyMessage:
                    FError.ShowError(error.Message);
                    break;
                case DebugMode.DetailInfo:
                    FError.ShowError(error.ToString());
                    break;
            }

            // jazyk nema nacitane skupiny - v okne nesmu ostat skupiny a zvuky predchadzajucej banky ani jazyka
            // a nic sa nesmie ulozit; vyber jazyka ostava, aby sa dalo prepnut na iny
            ClearAfterLoadError();
            return;
        }

        _messages.Clear();
        foreach (var msg in _bank.Project!.Messages[CurrentLanguage!])
            _messages.Add(msg);

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
        UpdateCommandStates();

        dgvErrors.DataSource = _messages;
        fileSystemWatcher.Path = _bank.PathToBank;
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

        // jazyk sa da odstranit alebo pridat iny; ukladanie a konverzia jazyka su vypnute (_languageLoaded)
        UpdateCommandStates();
    }

    /// <summary>
    /// Banka nema ziadny jazyk: prazdne okno, dostupne je len pridanie jazyka a ulozenie zoznamu jazykov.
    /// </summary>
    private void ShowNoLanguage()
    {
        ClearAfterLoadError();
        tscboxLanguages.Enabled = _bank.Project!.Languages.Count > 0;
        // FYZBANK.DAT sa da ulozit aj bez jazyka, napr. po odstraneni posledneho (CanSave)
        UpdateCommandStates();
        tspbProgress.Visible = false;
        ChangeStatus(Resources.FMain_Status_NoLanguage);
    }

    /// <summary>
    /// Vlozi jazyk do zoznamu jazykov banky bez prepnutia. V prazdnej banke sa jazyk rovno otvori.
    /// </summary>
    internal void InsertLanguage(FyzLanguage language, int index)
    {
        var languages = _bank.Project!.Languages;
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
    /// Odstrani jazyk zo zoznamu jazykov banky. Ak bol otvoreny, jeho neulozene zmeny sa zahodia (novy jazyk si ich
    /// necha pre vratenie) a otvori sa jazyk na jeho mieste.
    /// </summary>
    /// <param name="language">Odstranovany jazyk.</param>
    /// <param name="next">Jazyk, ktory sa ma otvorit namiesto odstraneneho (ak je v banke), inak jazyk na jeho mieste.</param>
    /// <returns>Pozicia, na ktorej jazyk bol, alebo -1.</returns>
    internal int RemoveLanguage(FyzLanguage language, FyzLanguage? next = null)
    {
        var languages = _bank.Project!.Languages;
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

        if (!_bank.IsNew(language))
            DiscardLanguage(language);

        if (next is null || !languages.Contains(next))
            next = languages.Count == 0 ? null : languages[Math.Min(index, languages.Count - 1)];
        SetComboLanguage(next);
        LoadLanguage(next);
        return index;
    }

    /// <summary>
    /// Odstrani priecinok jazyka (do Kosa pojde pri ulozeni). Zmazanie sa v prieskumniku nespracuva - jazyk sa
    /// zaroven zatvara.
    /// </summary>
    /// <returns><see langword="false" />, ak sa priecinok nepodarilo odstranit.</returns>
    internal bool DeleteLanguageDirectory(string directory)
    {
        if (!Directory.Exists(directory))
            return true;

        try
        {
            WithoutFileWatcher(() => _bank.Journal.Delete(directory, null));
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            _dialogs.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.FMain_LanguageDirDeleteFailed, directory, ex.Message));
            return false;
        }
    }

    /// <summary>
    /// Vrati odstraneny priecinok jazyka (zo zalohy zurnalu, po ulozeni z Kosa).
    /// </summary>
    internal void RestoreLanguageDirectory(string directory)
    {
        var restored = false;
        WithoutFileWatcher(() => restored = _bank.Journal.Restore(directory, null));
        if (!restored)
            _dialogs.ShowError(Resources.Action_LanguageRestoreFailed);
    }

    /// <summary>
    /// Obnovi zobrazenie jazyka v poli Jazyk po zmene nazvu.
    /// </summary>
    internal void RefreshLanguage(FyzLanguage language)
    {
        var languages = _bank.Project!.Languages;
        var index = languages.IndexOf(language);
        if (index < 0)
            return;

        _langAlreadySet = true;
        languages.ResetItem(index);
        _langAlreadySet = false;
        SetComboLanguage(CurrentLanguage);
    }

    /// <summary>
    /// Obnovi tabulku zvukov a pocty zvukov v skupinach po zmene zoznamu zvukov priamo vo FyzGroup.Sounds
    /// - BindingList MenuSounds (a dgvSounds.ResetBindings) o takej zmene nevie a riadky by ostali stare.
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

    /// <summary>
    /// Zapise FYZBANK.DAT a FYZZVUK.DAT otvoreneho jazyka (alebo vsetkych nacitanych jazykov) a novych jazykov,
    /// aby FYZBANK.DAT neodkazoval na chybajuci subor, a potvrdi zmeny na disku.
    /// </summary>
    /// <returns><c>false</c>, ak zapis zlyhal - zmeny ostavaju neulozene.</returns>
    private bool SaveBank(bool allLanguages)
    {
        var result = _bank.Save(allLanguages, _languageLoaded ? CurrentLanguage : null);
        if (!result.Saved)
        {
            _dialogs.ShowError(result.Error!);
            return false;
        }

        if (result.RecycleErrors.Count > 0)
            _dialogs.ShowWarning(string.Format(CultureInfo.CurrentCulture, Resources.FMain_RecycleFailed, ListOf(result.RecycleErrors)));

        _unUndoableUnsavedChanges = false;
        changeManager.SetSavedState();
        Saved = true;
        return true;
    }

    private void DoUndo()
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

    private void DoRedo()
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

    private void EnableUndoRedo() => UpdateCommandStates();

    private void EnableGoBackForward() => UpdateCommandStates();

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
            item.ForeColor = _bank.UsingStyle.ControlsColorScheme.Button.ForeColor;
            tsbGoBack.DropDownItems.Add(item);
        }

        if (moveManager.CurrentCommand is not null)
        {
            var currentItem = new ToolStripMenuItem(moveManager.CurrentCommand.CommandName);
            currentItem.Checked = true;
            currentItem.ForeColor = _bank.UsingStyle.ControlsColorScheme.Button.ForeColor;
            tsbGoBack.DropDownItems.Add(currentItem);
        }

        actions = moveManager.GetBackwardHistory().ToArray();

        for (var i = 0; i < Math.Min(actions.Length, 11); i++)
        {
            var action = actions[i];
            var item = new ToolStripMenuItem(action.CommandName);
            item.Tag = action;
            item.Click += BackButtonItemOnClick;
            item.ForeColor = _bank.UsingStyle.ControlsColorScheme.Button.ForeColor;
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

    private void DoGoBack()
    {
        moveManager.Backward();
        EnableGoBackForward();
        RefreshBackButtonDropDown();
    }

    private void DoGoForward()
    {
        moveManager.Forward();
        EnableGoBackForward();
        RefreshBackButtonDropDown();
    }

    private void DoSearch()
    {
        var fsearch = new FSearch(this);
        fsearch.Show(this);
    }

    private void DoAddSound()
    {
        if (CurrentGroup is null)
            return;

        var form = new FAddSound(CurrentGroup, _bank.PathToBank);
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            MenuSounds.Add(form.Sound);
            RegisterNewAction(new AddSoundAction(this, form.Sound));
            CheckProjectState();
        }
    }

    private void DoDeleteSounds()
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

    private void DoMoveSounds()
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
        var problems = SoundRules.ValidateMove(sounds, form.NewGroup, _bank.PathToBank);
        if (problems.Count > 0)
        {
            _dialogs.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.FMain_SoundsNotMoved, string.Join("\n", problems.Take(10))));
            return;
        }

        var moved = MoveSoundsToGroup(sounds, form.NewGroup);
        if (moved.Count > 0)
            RegisterNewAction(new MoveSoundsAction(this, moved, source, form.NewGroup));
    }

    /// <summary>
    /// Presunie zvuky do skupiny <paramref name="target" />. Nahravka, ktora lezi priamo v priecinku skupiny,
    /// sa presunie do priecinka cielovej skupiny; nahravka s pridavnou cestou ostane na mieste a cesta sa
    /// prepocita voci novej skupine (INISS ju berie relativne k priecinku skupiny).
    /// </summary>
    /// <returns>Zvuky, ktore sa presunuli - pri chybe suboru sa presun zastavi a zvysne ostanu na mieste.</returns>
    internal List<FyzSound> MoveSoundsToGroup(IList<FyzSound> sounds, FyzGroup target)
    {
        var pathToBank = _bank.PathToBank;
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
                    _bank.Journal.Move(sourcePath, newPath, target.Language);

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
            _dialogs.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.FMain_MoveStopped, moved.Count, sounds.Count, exception.Message));
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
    /// Presunie prvok suboru v strome prieskumnika do ineho priecinka (subor na disku uz je presunuty).
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
    /// Prida prazdny jazyk (s priecinkom v RAWBANK) a otvori ho. FYZBANK.DAT a jeho FYZZVUK.DAT sa zapisu pri ulozeni.
    /// Ak v priecinku FYZZVUK.DAT uz je, jazyk sa nacita z neho.
    /// </summary>
    private void DoAddLanguage()
    {
        var project = _bank.Project!;
        var form = new FAddEditLanguage(project.Languages);
        if (form.ShowDialog(this) != DialogResult.OK || !ConfirmLeaveLanguage())
            return;

        var lang = new FyzLanguage(form.LanguageKey, form.LanguageName, form.LanguageRelativePath) { Groups = [] };
        var directory = Path.TrimEndingDirectorySeparator(lang.GetAbsPath(project.AbsPathToBank));
        string? createdDirectory = null;
        try
        {
            if (_bank.Journal.CreateDirectory(directory, null))
                createdDirectory = directory;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
        {
            _dialogs.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.FMain_LanguageDirFailed, directory, ex.Message));
            return;
        }

        // zmeny zoznamu jazykov spred pridania sa uz nedaju vratit (historia sa maze pri kazdom prepnuti jazyka)
        var listChanged = LanguageListChanged();
        // priecinok uz ma FYZZVUK.DAT (napr. jazyk odstraneny len zo zoznamu) - nacita sa, inak by ho ulozenie prepisalo prazdnym
        if (File.Exists(LanguageRules.SoundsFile(project.AbsPathToBank, lang)))
            lang.Groups = null!;
        else
            _bank.MarkNew(lang);
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

    private void DoEditLanguage()
    {
        var language = CurrentLanguage!;
        var form = new FAddEditLanguage(_bank.Project!.Languages, language);
        if (form.ShowDialog(this) != DialogResult.OK)
            return;

        if (form.LanguageKey == language.Key && form.LanguageName == language.Name && form.LanguageRelativePath == language.RelativePath)
            return;

        var action = new EditLanguageAction(this, language,
            (language.Key, form.LanguageKey), (language.Name, form.LanguageName), (language.RelativePath, form.LanguageRelativePath));
        if (ChangeLanguage(language, form.LanguageKey, form.LanguageName, form.LanguageRelativePath))
            RegisterNewAction(action);
    }

    private void DoDeleteLanguage()
    {
        var result = _dialogs.ShowWarning(Resources.FMain_DeleteLanguage, MessageBoxButtons.YesNo);
        if (result != DialogResult.Yes)
            return;

        result = _dialogs.ShowWarning(Resources.FMain_DeleteLanguageDir, MessageBoxButtons.YesNoCancel);
        if (result == DialogResult.Cancel)
            return;

        var language = CurrentLanguage!;
        var withData = result == DialogResult.Yes;
        var directory = Path.TrimEndingDirectorySeparator(language.GetAbsPath(_bank.PathToBank));

        // neulozene zmeny zvukov odstraneneho jazyka sa zahodia (aj na disku), zmeny zoznamu jazykov ostavaju neulozene
        var listChanged = LanguageListChanged();
        if (!_bank.IsNew(language))
            DiscardLanguage(language);
        // priecinok sa maze skor, ako sa zacne nacitavat dalsi jazyk (prehliadanie suborov banky na pozadi)
        if (withData && !DeleteLanguageDirectory(directory))
            withData = false;
        var index = RemoveLanguage(language);

        ResetHistory(listChanged);
        RegisterNewAction(new RemoveLanguageAction(this, language, index, directory, withData));
    }

    private void ShowAppSettings()
    {
        var form = new FAppSettings(_bank.Session);
        if (form.ShowDialog() == DialogResult.OK)
        {
            UpdateMainUI();
        }
    }

    private void ShowInfoApp()
    {
        var form = new FAboutApp(Resources.AboutAppDescription, Resources.raw);
        form.ShowDialog();
    }

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

        _bank.Config.WrapSoundText = tsmimWrapTextSoundCol.Checked;
        var configsDir = ToolsCore.AppPaths.ConfigDir;
        XmlSerialization.WriteData(PathUtils.CombinePath(configsDir, ToolsCore.FileConsts.FileConfig)!, _bank.Config);
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

    private void DoFindProblem()
    {
        if (dgvErrors.IsSelectionEmpty())
            return;

        var problem = (IRawBankMessage)dgvErrors.SelectedRows[0].DataBoundItem!;
        problem.Show(this);
    }

    private void DoSolveProblem()
    {
        if (dgvErrors.IsSelectionEmpty())
            return;

        var problem = (IRawBankMessage)dgvErrors.SelectedRows[0].DataBoundItem!;
        problem.Resolve(this);
        CheckProjectState();
    }

    #endregion

    #region ExplorerPanel

    private void DoOpenFileInExplorer()
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

    private void DoPlayFile()
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
        tsslStatus.Text = Resources.FMain_Status_OpeningGroup;
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

        UpdateCommandStates();
    }

    private void dgvSounds_SelectionChanged(object sender, EventArgs e)
    {
        if (_doNotChangeExplorerSelection || _selectingMoreRows || DoNotChangeSoundsSelection)
            return;

        UpdateCommandStates();

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
            return [];

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

    private void mmErrors_Click(object sender, EventArgs e) => splitSoundsErrors.Panel2Collapsed = true;

    private void tssbErrors_Click(object sender, EventArgs e) => splitSoundsErrors.Panel2Collapsed = !splitSoundsErrors.Panel2Collapsed;

    private void TsmimShowErrors_CheckedChanged(object sender, EventArgs e) => splitSoundsErrors.Panel2Collapsed = !tsmimShowErrors.Checked;

    private void FMain_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (_bank.Project is null)
            return;

        var discard = false;
        if (!Saved)
        {
            var result = _dialogs.ShowQuestion(Resources.FMain_Save_Changes, MessageBoxButtons.YesNoCancel);
            switch (result)
            {
                case DialogResult.Yes:
                    // neuspesne ulozenie okno nezatvori - zmeny by sa stratili
                    if (!SaveBank(false))
                    {
                        e.Cancel = true;
                        return;
                    }
                    break;
                case DialogResult.No:
                    discard = true;
                    break;
                default:
                    e.Cancel = true;
                    return;
            }
        }

        // zahodenie vrati disk do stavu posledneho ulozenia, inak sa zmeny na disku potvrdia
        CloseBank(discard);
    }

    /// <summary>
    /// Mala ikona systemu ako obrazok - vytvara sa raz, nie pri kazdom vykresleni bunky.
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
        // jazyk sa nenacital (chyba pri nacitani) - nie je k comu udalost priradit; udalost zaradena do fronty
        // okna este pred zatvorenim banky (napr. zmena priecinka jazyka po pridani skupiny) uz nema banku
        if (CurrentLanguage?.Directory is null || _bank.Project is null)
            return;

        if (RawBankExplorer.ConvertSoundIsHandled || RawBankExplorer.MovingSoundIsHandled)
            return;

        var newElement = RawBankExplorer.GetElement(e.FullPath, CurrentLanguage!.Directory, _bank.PathToBank, RawBankExplorer.SearchOperation.Create);

        // subor sa vratil (napr. obnovenim z kosa) - patri zvuku, ktory ho ma v nazve suboru
        var owner = newElement is SoundFileElement returned && returned.Parent?.Group is { } ownerGroup
            ? ownerGroup.Sounds.FirstOrDefault(s => s.File == null && RawBankParser.AdditionalPathIsEmpty(s.AdditionalRelativePath)
                                                   && RawBankExplorer.EqualsPathNames(s.FileName ?? "", returned.Name))
            : null;
        if (owner is not null)
        {
            RelinkSoundFile(owner);
        }
        else if (newElement is SoundFileElement sfe && _bank.Config.AutoInsertSoundData && sfe.Parent?.Group is not null)
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
                if (_bank.Config.AutoRecalculateSoundDuration && sfe.Duration >= 0)
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
                if (_bank.Config.ShowAfterInsertSoundDialog)
                {
                    FAfterInsertSounds.CreateOrUseExistingForm(this, sound);
                }
                else if (SoundRules.ValidateNew([sound]).Count > 0)
                {
                    // kluc alebo nazov uz v skupine je (napr. 9900100.WAV k zvuku 9900100.EWA) - bez okna sa neda opravit,
                    // subor ostane bez udajov o zvuku a zoznam chyb ho ukaze ako nedefinovany
                    sfe.Sound = null!;
                }
                else
                {
                    RegisterNewAction(new AddSoundAction(this, sound));
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
        // jazyk sa nenacital (chyba pri nacitani) - nie je k comu udalost priradit; udalost zaradena do fronty
        // okna este pred zatvorenim banky (napr. zmena priecinka jazyka po pridani skupiny) uz nema banku
        if (CurrentLanguage?.Directory is null || _bank.Project is null)
            return;

        if (RawBankExplorer.ConvertSoundIsHandled || RawBankExplorer.MovingSoundIsHandled)
            return;

        RawBankExplorer.GetElement(e.FullPath, CurrentLanguage!.Directory, _bank.PathToBank, RawBankExplorer.SearchOperation.Delete);

        if (e.FullPath.StartsWith(CurrentDirectory.DirInfo.FullName, StringComparison.OrdinalIgnoreCase))
            FillExplorerList(CurrentDirectory);

        CheckProjectState();
        dgvGroups.ResetBindings();
        dgvSounds.ResetBindings();
    }

    private async void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
    {
        // jazyk sa nenacital (chyba pri nacitani) - nie je k comu udalost priradit; udalost zaradena do fronty
        // okna este pred zatvorenim banky (napr. zmena priecinka jazyka po pridani skupiny) uz nema banku
        if (CurrentLanguage?.Directory is null || _bank.Project is null)
            return;

        var fileElement = RawBankExplorer.GetElement(e.FullPath, CurrentLanguage!.Directory, _bank.PathToBank);
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
                if (_bank.Config.AutoRecalculateSoundDuration && sfe.Sound is not null)
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
        // jazyk sa nenacital (chyba pri nacitani) - nie je k comu udalost priradit; udalost zaradena do fronty
        // okna este pred zatvorenim banky (napr. zmena priecinka jazyka po pridani skupiny) uz nema banku
        if (CurrentLanguage?.Directory is null || _bank.Project is null)
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
            snd.AdditionalRelativePath = SoundRules.AdditionalPathFor(snd.Group, sfe.FileInfo.DirectoryName!, _bank.PathToBank);

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
        if (!string.IsNullOrEmpty(newAdditionalPath) && (!newAdditionalPath.EndsWith('\\') || string.IsNullOrWhiteSpace(newAdditionalPath)))
        {
            _dialogs.ShowError(Resources.FMain_InvalidRelativePath);
            e.Cancel = true;
            dgvSounds.Rows[e.RowIndex].Cells[nameof(cSoundAdditionalRelativePath)].Value = "";
            return;
        }
        ValidateRow(e.RowIndex, true);
    }

    private void dgvSounds_DataError(object sender, DataGridViewDataErrorEventArgs e) => _dialogs.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.FMain_GridDataError, e.Exception!.Message));

    private void dgvSounds_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
    {
        _cellUserEditing = true;
        var val = dgvSounds.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
        _cellOldValue = val == null ? "" : val.ToString() ?? "";
    }

    /// <summary>
    /// Prepoji zvuk so suborom podla jeho nazvu suboru a pridavnej cesty (po uprave v tabulke, spat a znovu)
    /// a zisti dlzku nahravky. Povodny subor ostane bez udajov o zvuku.
    /// </summary>
    internal async void RelinkSoundFile(FyzSound sound)
    {
        if (sound.File?.Sound == sound)
            sound.File.Sound = null!;
        sound.File = null!;

        var sfe = SoundUtils.FindSoundFile(sound, _bank.PathToBank);
        if (sfe is null)
            return;

        sound.File = sfe;
        sfe.Sound = sound;
        if (sfe.Duration < 0)
            sfe.Duration = await SoundUtils.GetSoundDuration(sfe);
        if (_bank.Config.AutoRecalculateSoundDuration && sfe.Duration >= 0)
            sound.Duration = sfe.Duration;
        CheckProjectState();
    }

    /// <summary>
    /// Kluc a nazov zvuku su povinne a v skupine jedinecne - rovnako ako v okne Pridat zvuk.
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
            error = isKey ? Resources.FMain_SoundKeyRequired : Resources.FMain_SoundNameRequired;
        else if (sound.Group.Sounds.Any(s => s != sound && SoundRules.SameText(isKey ? s.Key : s.Name, value)))
            error = string.Format(CultureInfo.CurrentCulture, isKey ? Resources.SoundRules_KeyExists : Resources.SoundRules_NameExists, value, sound.Group.Name);

        if (error is null)
            return;

        _dialogs.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.FMain_FixValueOrEsc, error));
        e.Cancel = true;
    }

    /// <summary>
    /// Kazda upravena bunka je samostatny krok Spat - so stlpcom a hodnotou prave tejto bunky.
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
                DirectoryElement de => string.Format(CultureInfo.CurrentCulture, Resources.FMain_ItemCount, de.Children.Count),
                _ => null
            };
        }
    }

    private void dgvExplorer_SelectionChanged(object sender, EventArgs e)
    {
        UpdateCommandStates();
    }

    private async void ValidateRow(int row, bool userEditing = false)
    {
        if (_deletingRows || row == dgvSounds.NewRowIndex || CurrentGroup == null || CurrentLanguage is null)
            return;

        dgvSounds.Rows[row].Cells[nameof(cSoundFileName)].ErrorText = null;
        var newFileName = (string)dgvSounds.Rows[row].Cells[nameof(cSoundFileName)].Value!;
        var sound = (FyzSound)dgvSounds.Rows[row].DataBoundItem!;

        if (string.IsNullOrWhiteSpace(newFileName) || !Utils.IsFileNameCorrect(sound.GetAbsPath(_bank.PathToBank), newFileName, false))
        {
            if (sound.File is not null)
                sound.File.Sound = null!;
            sound.File = null!;
            dgvSounds.Rows[row].Cells[nameof(cSoundFileName)].ErrorText = Resources.FMain_InvalidFileName;
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
                ext.EqualsIgnoreCase(SoundUtils.WAVExt) || ext.EqualsIgnoreCase(SoundUtils.EWAExt)
                    ? Resources.FMain_SoundFileMissing
                    : Resources.FMain_InvalidFileType;
            return;
        }

        if (sound.File == null || !sound.File.FileInfo.Exists)
        {
            dgvSounds.Rows[row].Cells[nameof(cSoundFileName)].ErrorText = Resources.FMain_SoundFileMissing;
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
                        dgvErrors.Rows[e.RowIndex].Cells[nameof(cMsgType)].ToolTipText = Resources.RInfo;
                        break;
                    case MessageType.Warning:
                        e.Value = _warning;
                        dgvErrors.Rows[e.RowIndex].Cells[nameof(cMsgType)].ToolTipText = Resources.RWarning;
                        break;
                    case MessageType.Error:
                        e.Value = _error;
                        dgvErrors.Rows[e.RowIndex].Cells[nameof(cMsgType)].ToolTipText = Resources.RError;
                        break;
                    default:
                        throw new UnreachableException();
                }
            }
        }
    }

    private void DgvErrors_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex == -1)
            return;

        ((IRawBankMessage)dgvErrors.Rows[e.RowIndex].DataBoundItem!).Show(this);
    }

    private void ChangeManager_UndoRedoStateChanged(object sender, UndoRedoStateEventArgs e)
    {
        Saved = changeManager.IsInSavedState() && !_unUndoableUnsavedChanges;
        if (e.NewState != UndoRedoState.Clear) 
            CheckProjectState();
    }

    private bool IsRootPath(DirectoryElement dir)
    {
        return string.Equals(
            Path.GetFullPath(dir.DirInfo.FullName).TrimEnd('\\'),
            Path.GetFullPath(_bank.PathToBank + CurrentLanguage!.RelativePath).TrimEnd('\\'),
            StringComparison.OrdinalIgnoreCase);
    }

    // 1 chyba, 2 - 4 chyby, 0 a 5+ chyb
    /// <summary>
    /// Pocet s tvarom slova podla poctu; <paramref name="forms" /> su tvary pre 1, 2-4 a 5+ oddelene '|'.
    /// </summary>
    private static string CountText(int count, string forms)
    {
        var f = forms.Split('|');
        return $"{count} {(count == 1 ? f[0] : count is >= 2 and <= 4 ? f[1] : f[2])}";
    }

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
                    throw new UnreachableException();
            }
        }

        Invoke(() =>
        {
            tsbErrors.Text = CountText(errorCount, Resources.FMain_Count_Errors);
            tsbWarnings.Text = CountText(warningCount, Resources.FMain_Count_Warnings);
            tsbInfos.Text = CountText(infoCount, Resources.FMain_Count_Infos);

            if (errorCount != 0)
            {
                tssbErrors.Image = _error;
                tssbErrors.Text = CountText(errorCount, Resources.FMain_Count_Errors);
                tssbErrors.ForeColor = Color.Red;
            }
            else
            {
                tssbErrors.Image = GlobalResources.correct;
                tssbErrors.Text = "";
                tssbErrors.ForeColor = _bank.UsingStyle.ControlsColorScheme.Panel.ForeColor;
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
        // pevne klavesy prieskumnika - akcie samy preskocia tlacidlo Spat a prvky, na ktore sa nevztahuju
        if (e.KeyData == Keys.F2)
            DoRenameFile();
        if (e.KeyData == Keys.Delete)
            DoDeleteFile();
        if (e.KeyData == Keys.F5)
            DoPlayFile();
    }

    private void TimerToCheck_Tick(object sender, EventArgs e)
    {
        timerToCheck.Stop();
        timerToCheck.Enabled = false;

        // jazyk sa nenacital (chyba pri nacitani) - nie je co porovnavat
        if (CurrentLanguage is null || _bank.Project?.Messages.ContainsKey(CurrentLanguage) != true)
            return;

        RawBankExplorer.MergeFilesAndData(Root, CurrentLanguage, _bank.Project.Messages, _bank.PathToBank, true);

        _messages.Clear();
        try
        {
            foreach (var msg in _bank.Project!.Messages[CurrentLanguage!])
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