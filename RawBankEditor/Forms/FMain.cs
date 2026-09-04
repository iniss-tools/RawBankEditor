using System.Drawing.Imaging;
using System.Threading.Tasks;
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
    private readonly ShellIcon _error, _warning, _info;

    private BackButtonElement _explorerBack;

    private string _cellOldValue;
    private string _actualStatusTxt;
    private bool _langAlreadySet;
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

    internal bool DoNotChangeSoundsSelection { get; set; }
    internal FyzLanguage CurrentLanguage { get; private set; }
    internal FyzGroup CurrentGroup { get; private set; }

    internal FileSystemElement SelectElement { get; set; }

    internal DirectoryElement Root { get; set; }
    internal DirectoryElement CurrentDirectory { get; set; }

    internal ExBindingList<FyzSound> MenuSounds { get; private set; }
    internal ExBindingList<FyzGroup> MenuGroups { get; private set; }

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

        var dirs = AppRegistry.GetOpenedProjects();

        foreach (var recentDir in dirs)
        {
            ToolStripItem item1 = new ToolStripMenuItem(recentDir.Path);
            ToolStripItem item2 = new ToolStripMenuItem(recentDir.Path);

            item1.Click += RecentDirs_Click;
            item2.Click += RecentDirs_Click;
            tsmimRecent.DropDownItems.Add(item1);
            tsbRecent.DropDownItems.Add(item2);
        }

        if (dirs.Length == 0 || dirs[0].Path == "")
        {
            tsmimRecent.Enabled = false;
            tsbRecent.Enabled = false;
        }
        else
        {
            tsmimRecent.Enabled = true;
            tsbRecent.Enabled = true;
        }

        _error = new ShellIcon(ShellIconType.Error, ShellIconSize.Small);
        _warning = new ShellIcon(ShellIconType.Warning, ShellIconSize.Small);
        _info = new ShellIcon(ShellIconType.Info, ShellIconSize.Small);
        
        tsbErrors.Image = _error.ToBitmap();
        tsbWarnings.Image = _warning.ToBitmap();
        tsbInfos.Image = _info.ToBitmap();
        tsmimShowErrors.Image = _error.ToBitmap();

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

        var configsDir = Utils.CombinePath(Application.StartupPath, ToolsCore.FileConsts.CONFIG_PATH);
        
        tsmimShowErrors.Checked = GlobData.Config.ShowErrorsWindow;
        splitSoundsErrors.Panel2.VisibleChanged += (_, _) =>
        {
            GlobData.Config.ShowErrorsWindow = !splitSoundsErrors.Panel2Collapsed;
            XmlSerialization.WriteData(Utils.CombinePath(configsDir, ToolsCore.FileConsts.FILE_CONFIG), GlobData.Config);
        };

        if (GlobData.Config.LeftPanelWidth != -1) splitContainer1.SplitterDistance = GlobData.Config.LeftPanelWidth;
        splitContainer1.SplitterMoved += (_, _) =>
        {
            GlobData.Config.LeftPanelWidth = splitContainer1.SplitterDistance;
            XmlSerialization.WriteData(Utils.CombinePath(configsDir, ToolsCore.FileConsts.FILE_CONFIG), GlobData.Config);
        };

        if (GlobData.Config.GroupPanelWidth != -1) splitContainer2.SplitterDistance = GlobData.Config.GroupPanelWidth;
        splitContainer2.SplitterMoved += (_, _) =>
        {
            GlobData.Config.GroupPanelWidth = splitContainer2.SplitterDistance;
            XmlSerialization.WriteData(Utils.CombinePath(configsDir, ToolsCore.FileConsts.FILE_CONFIG), GlobData.Config);
        };

        if (GlobData.Config.ErrorPanelWidth != -1) splitSoundsErrors.SplitterDistance = splitSoundsErrors.Width - GlobData.Config.ErrorPanelWidth;
        splitSoundsErrors.SplitterMoved += (_, _) =>
        {
            GlobData.Config.ErrorPanelWidth = splitSoundsErrors.Width - splitSoundsErrors.SplitterDistance;
            XmlSerialization.WriteData(Utils.CombinePath(configsDir, ToolsCore.FileConsts.FILE_CONFIG), GlobData.Config);
        };

        AppRegistry.RegisterJumpList();

        //cesta k projektu zadana ako argument ma prednost pred poslednym otvorenym projektom
        var path = Utils.GetProjectPathFromArgs();
        if (path is null && GlobData.Config.Startup == StartupType.LastProject)
            path = AppRegistry.GetLastProject();

        if (Directory.Exists(path))
        {
            PrepareGlobalData(path);
            AppRegistry.SetUsageOfProject(path);
            AppRegistry.SetLastProject(path);
        }
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

    private void PrepareGlobalData(string dirpath)
    {
        if (GlobData.OpenedProject is not null && !Saved)
        {
            var result = Utils.ShowQuestion(Resources.FMain_Save_Changes, MessageBoxButtons.YesNoCancel);
            switch (result)
            {
                case DialogResult.Yes:
                    DoSave(this, EventArgs.Empty);
                    break;
                case DialogResult.No:
                    break;
                default:
                    return;
            }
        }
        
        dgvErrors.DataSource = null;

        if (GlobData.Config.DebugModeGUI != DebugMode.AppCrash)
            try
            {
                GlobData.PrepareGlobalData(dirpath);
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
            GlobData.PrepareGlobalData(dirpath);

        if (GlobData.OpenedProject is null)
            return;

        FyzLanguage lang = null;
        
        switch (GlobData.OpenedProject!.Languages.Count)
        {
            case 0:
                Utils.ShowWarning("V banke sa nenachádza žiadnen jazyk.\r\nMôžete pridať nový jazyk.");
                break;
            case 1:
                lang = GlobData.OpenedProject.Languages[0];
                break;
            default:
                var flang = new FLangChoose();
                var result = flang.ShowDialog();
                if (result == DialogResult.OK)
                    lang = flang.Selected;
                break;
        }

        if (lang == null)
            return;

        Text = @$"{Application.ProductName} - {GlobData.OpenedProject.AbsPathToINISS}";

        CurrentLanguage = lang;

        _langAlreadySet = true;
        tscboxLanguages.ComboBox.DataSource = GlobData.OpenedProject.Languages;
        tscboxLanguages.SelectedItem = lang;
        _langAlreadySet = false;

        tspbProgress.Visible = true;
        ChangeStatus("Načítanie súborov banky");
        bWorkerReadDat.RunWorkerAsync(lang);
        
        moveManager.Clear();
        changeManager.Clear();
    }

    private void RecentDirs_Click(object sender, EventArgs e)
    {
        var menuItem = (ToolStripMenuItem)sender;
        var menuText = menuItem.Text;
        
        PrepareGlobalData(menuText);
        AppRegistry.SetUsageOfProject(menuText);
        AppRegistry.SetLastProject(menuText);
    }

    private void DoOpenDir(object sender, EventArgs e)
    {
        var dialog = new ExFolderBrowserDialog { Description = "Vyberte priečinok s INISS.exe" };
        if (dialog.ShowDialog(this) == DialogResult.Cancel)
            return;

        var selectedPath = dialog.SelectedPath;

        PrepareGlobalData(selectedPath);
        AppRegistry.SetUsageOfProject(selectedPath);
        AppRegistry.SetLastProject(selectedPath);
    }

    private void bWorkerReadDat_DoWork(object sender, DoWorkEventArgs e)
    {
        //part 1 - read and analyze all files in rawbank
        var progressFiles = new ProgressStatus("Načítavanie súborového systému", 0);
        bWorkerReadDat.ReportProgress(-1, progressFiles);
        Root = RawBankExplorer.ExploreFileSystem();

        //part 2 - analyze FYZZVUK.dat file
        var lang = (FyzLanguage)e.Argument;
        RawBankParser.ReadFyzZvukFile(GlobData.OpenedProject.AbsPathToBank, lang, bWorkerReadDat);

        //part 3 - merge physical files and logical data
        var progressMerge = new ProgressStatus("Spájam načítané dáta so súborovým systémom", 0);
        bWorkerReadDat.ReportProgress(-1, progressMerge);
        RawBankExplorer.MergeFilesAndData(Root, lang, GlobData.OpenedProject.Messages);

        Invoke(() => _messages.Clear());

        foreach (var msg in GlobData.OpenedProject.Messages[lang])
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

        if (GlobData.Config.DebugModeGUI != DebugMode.AppCrash && e.Error != null)
        {
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

            return;
        }

        _programChange = true;
        dgvSounds.DataSource = null;
        MenuGroups = new ExBindingList<FyzGroup>(CurrentLanguage.Groups);
        dgvGroups.DataSource = MenuGroups;
        tscboxLanguages.Enabled = true;
        _programChange = false;

        //_lastMovePosition = new MovePosition(GetSelectedSoundRows(), CurrentLanguage, CurrentGroup);
        moveManager.AddCommand(new SelectedCellSoundMoveAction(this,
            new MovePosition(GetSelectedSoundRows(), CurrentLanguage, CurrentGroup)));

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
        fileSystemWatcher.Path = GlobData.OpenedProject.AbsPathToBank;
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

    private void DoSave(object sender, EventArgs e)
    {
        RawBankParser.WriteFyzBankFile(GlobData.OpenedProject.AbsPathToBank, GlobData.OpenedProject.Languages.ToList());
        RawBankParser.WriteFyzZvukFile(GlobData.OpenedProject.AbsPathToBank, CurrentLanguage);
        _unUndoableUnsavedChanges = false;
        changeManager.SetSavedState();
        Saved = true;
    }

    private void DoSaveAll(object sender, EventArgs e)
    {
        RawBankParser.WriteFyzBankFile(GlobData.OpenedProject.AbsPathToBank, GlobData.OpenedProject.Languages.ToList());
        foreach (var lang in GlobData.OpenedProject.Languages)
            RawBankParser.WriteFyzZvukFile(GlobData.OpenedProject.AbsPathToBank, lang);

        _unUndoableUnsavedChanges = false;
        changeManager.SetSavedState();
        Saved = true;
    }

    private void DoUndo(object sender, EventArgs e)
    {
        changeManager.Undo();
        EnableUndoRedo();
    }

    private void DoRedo(object sender, EventArgs e)
    {
        changeManager.Redo();
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

    private void BackButtonItemOnClick(object sender, EventArgs e)
    {
        var tsmi = (ToolStripMenuItem)sender;
        if (tsmi.Tag is IBackwardForwardCommand action)
        {
            moveManager.Backward(action);
            EnableGoBackForward();
            RefreshBackButtonDropDown();
        }
    }

    private void ForwardButtonItemOnClick(object sender, EventArgs e)
    {
        var tsmi = (ToolStripMenuItem)sender;
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
        var form = new FAddSound();
        if (form.ShowDialog() == DialogResult.OK)
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

        var sounds = new List<FyzSound>();
        var first = true;
        var firstDisplayedRow = -1;

        _deletingRows = true;
        foreach (DataGridViewRow row in dgvSounds.SelectedRows)
        {
            var sound = row.DataBoundItem as FyzSound;
            sounds.Add(sound);
            if (first)
            {
                first = false;
                firstDisplayedRow = row.Index - 1;
            }
            CurrentLanguage.Groups[dgvGroups.SelectedRows[0].Index].Sounds.RemoveAt(row.Index);
        }
        MenuSounds.ResetBindings();
        _deletingRows = false;

        if (firstDisplayedRow >= 0 && firstDisplayedRow < dgvSounds.Rows.Count)
        {
            EnsureVisibleRow(dgvSounds, firstDisplayedRow);
        }

        RegisterNewAction(new RemovedSoundsAction(this, sounds));
        MenuGroups.ResetBindings();
        CheckProjectState();
    }

    private void DoMoveSounds(object sender, EventArgs e)
    {
        if (dgvSounds.IsSelectionEmpty() || dgvGroups.IsSelectionEmpty())
            return;

        var form = new FSoundsMove(CurrentLanguage.Groups, CurrentGroup);
        if (form.ShowDialog(this) != DialogResult.OK) 
            return;

        var sounds = dgvSounds.SelectedRows.Cast<FyzSound>().ToList();
        var pathTobank = GlobData.OpenedProject.AbsPathToBank;
        foreach (var sound in sounds)
        {
            CurrentGroup.Sounds.Remove(sound);
            sound.Group = form.NewGroup;
            form.NewGroup.Sounds.Add(sound);
            var oldPath = CurrentGroup.GetAbsPath(pathTobank);
            var newPath = form.NewGroup.GetAbsPath(pathTobank);

            RawBankExplorer.MovingSoundIsHandled = true;
            if (File.Exists(oldPath)) 
                File.Move(oldPath, newPath);
            RawBankExplorer.MovingSoundIsHandled = false;
        }

        RegisterNewAction(new MoveSoundsAction(this, sounds, CurrentGroup, form.NewGroup));
        CheckProjectState();
        dgvSounds.ResetBindings();
        SelectGroup(form.NewGroup);
        MenuGroups.ResetBindings();
    }

    private void DoConvertSoundsToEwa(object sender, EventArgs e)
    {
        var result = Utils.ShowQuestion(string.Format(Resources.FMain_DoConvertSounds, SoundUtils.EWA_EXT));
        if (result != DialogResult.Yes) 
            return;

        ChangeStatus("Konvertujem vybrané zvuky");
        tspbProgress.Visible = true;
        var sounds = dgvSounds.SelectedRows.Cast<FyzSound>().ToList();
        RegisterNewAction(new ConvertSoundsEwaWawAction(this, sounds, true));
        ConvertSounds(sounds, true);
        CheckProjectState();
    }

    private void DoConvertSoundsToWav(object sender, EventArgs e)
    {
        var result = Utils.ShowQuestion(string.Format(Resources.FMain_DoConvertSounds, SoundUtils.WAV_EXT));
        if (result != DialogResult.Yes) 
            return;

        ChangeStatus("Konvertujem vybrané zvuky");
        tspbProgress.Visible = true;
        var sounds = dgvSounds.SelectedRows.Cast<FyzSound>().ToList();
        RegisterNewAction(new ConvertSoundsEwaWawAction(this, sounds, false));
        ConvertSounds(sounds, false);
        CheckProjectState();
    }

    private async void ConvertSounds(ICollection<FyzSound> sounds, bool toEwa)
    {
        tspbProgress.Minimum = 0;
        tspbProgress.Maximum = sounds.Count;
        tspbProgress.Style = ProgressBarStyle.Blocks;
        tspbProgress.Value = 0;
        await Task.Run(() =>
        {
            RawBankExplorer.ConvertSoundIsHandled = true;
            SoundUtils.ConvertSounds(sounds, toEwa, tspbProgress);
            RawBankExplorer.ConvertSoundIsHandled = false;

            Invoke(ResetStatusAfterTask);
        });
    }

    private void DoAddLanguage(object sender, EventArgs e)
    {
        var form = new FAddEditLanguage();
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            GlobData.OpenedProject.Languages.Add(form.Language);
            tscboxLanguages.SelectedItem = form.Language;
            CheckProjectState();
        }
    }

    private void DoEditLanguage(object sender, EventArgs e)
    {
        var form = new FAddEditLanguage(CurrentLanguage);
        if (form.ShowDialog(this) == DialogResult.OK) 
            CheckProjectState();
    }

    private void DoDeleteLanguage(object sender, EventArgs e)
    {
        var result = Utils.ShowWarning("Práve vybraný jazyk sa odstráni.\n\nSte si istý?", MessageBoxButtons.YesNo);
        if (result == DialogResult.Yes)
        {
            result = Utils.ShowWarning("Vymazať aj priečinok so zvukmi jazyka?\n\nPriečinok sa premiestni do koša.", MessageBoxButtons.YesNoCancel);
            if (result == DialogResult.Cancel)
                return;

            RegisterNewAction(new RemoveLanguageAction(this, CurrentLanguage, result == DialogResult.Yes));

            GlobData.OpenedProject.Languages.Remove(CurrentLanguage);
            if (result == DialogResult.Yes)
                Utils.DeleteDirectoryToRecycleBin(CurrentLanguage.Directory.DirInfo.FullName, true);

            CheckProjectState();
        }
    }

    private void DoConvertLangToEwa(object sender, EventArgs e)
    {
        var result = Utils.ShowQuestion(string.Format(Resources.FMain_DoConvertLang, SoundUtils.EWA_EXT));
        if (result != DialogResult.Yes) 
            return;

        ChangeStatus("Konvertujem zvuky jazyka");
        tspbProgress.Visible = true;
        RegisterNewAction(new ConvertLanguageEwaWawAction(this, CurrentLanguage, true));
        ConvertSoundsInLanguage(CurrentLanguage, true);
        CheckProjectState();
    }

    private void DoConvertLangToWav(object sender, EventArgs e)
    {
        var result = Utils.ShowQuestion(string.Format(Resources.FMain_DoConvertLang, SoundUtils.WAV_EXT));
        if (result != DialogResult.Yes) 
            return;

        ChangeStatus("Konvertujem zvuky jazyka");
        tspbProgress.Visible = true;
        RegisterNewAction(new ConvertLanguageEwaWawAction(this, CurrentLanguage, false));
        ConvertSoundsInLanguage(CurrentLanguage, false);
        CheckProjectState();
    }

    private async void ConvertSoundsInLanguage(FyzLanguage language, bool toEwa)
    {
        var sndCount = language.Groups.Sum(g => g.Sounds.Count);

        tspbProgress.Minimum = 0;
        tspbProgress.Maximum = sndCount;
        tspbProgress.Style = ProgressBarStyle.Blocks;
        await Task.Run(() =>
        {
            RawBankExplorer.ConvertSoundIsHandled = true;
            SoundUtils.ConvertSoundsLanguage(language, toEwa, tspbProgress);
            RawBankExplorer.ConvertSoundIsHandled = false;

            Invoke(ResetStatusAfterTask);
        });
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

    private void ShowUpdates(object sender, EventArgs e) => Process.Start(LinkConsts.UPDATE);

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
        var configsDir = Utils.CombinePath(Application.StartupPath, ToolsCore.FileConsts.CONFIG_PATH);
        XmlSerialization.WriteData(Utils.CombinePath(configsDir, ToolsCore.FileConsts.FILE_CONFIG), GlobData.Config);
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

        var problem = (IRawBankMessage)dgvErrors.SelectedRows[0].DataBoundItem;
        problem.Show();
    }

    private void DoSolveProblem(object sender, EventArgs e)
    {
        if (dgvErrors.IsSelectionEmpty())
            return;

        var problem = (IRawBankMessage)dgvErrors.SelectedRows[0].DataBoundItem;
        problem.Resolve();
        CheckProjectState();
    }

    #endregion

    #region GroupsPanel

    private void DoAddGroup(object sender, EventArgs e)
    {
        var form = new FAddEditGroup();
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            MenuGroups.Add(form.Group);
            CheckProjectState();
        }
    }

    private void DoEditGroup(object sender, EventArgs e)
    {
        if (dgvGroups.IsSelectionEmpty())
            return;

        var form = new FAddEditGroup(dgvGroups.SelectedRows[0].DataBoundItem as FyzGroup);
        if (form.ShowDialog(this) == DialogResult.OK)
        {
            CheckProjectState();
            MenuGroups.ResetBindings();
        }
    }

    private void DoDeleteGroup(object sender, EventArgs e)
    {
        if (dgvGroups.IsSelectionEmpty())
            return;

        var delete = false;
        var result = Utils.ShowWarning("Chystáte sa vymazať skupinu/y zvukov.\n\nMám vymazať aj priečinok/ky so zvukmi? (Ak áno, táto operácia je nenávrátná).", MessageBoxButtons.YesNoCancel);
        switch (result)
        {
            case DialogResult.Yes:
                delete = true;
                break;
            case DialogResult.No:
                break;
            default:
                return;
        }

        var grps = new LinkedList<FyzGroup>();
        foreach (DataGridViewRow row in dgvGroups.SelectedRows)
        {
            grps.AddLast(row.DataBoundItem as FyzGroup);
        }

        RegisterNewAction(new RemovedGroupsAction(this, grps));

        foreach (var fyzGroup in grps)
        {
            MenuGroups.Remove(fyzGroup);
            if (delete && fyzGroup.Directory is not null) 
                Utils.DeleteDirectoryToRecycleBin(fyzGroup.Directory.DirInfo.FullName);
        }

        MenuGroups.ResetBindings();
        CheckProjectState();
    }

    private void DoConvertGroupToEwa(object sender, EventArgs e)
    {
        if (dgvGroups.IsSelectionEmpty())
            return;

        var result = Utils.ShowQuestion(string.Format(Resources.FMain_DoConvertGroup, SoundUtils.EWA_EXT));
        if (result != DialogResult.Yes) 
            return;

        ChangeStatus("Konvertujem skupinu zvukov");
        tspbProgress.Visible = true;
        var group = dgvGroups.SelectedRows[0].DataBoundItem as FyzGroup;
        RegisterNewAction(new ConvertGroupEwaWawAction(this, group, true));
        ConvertSoundsInGroup(group, true);
        CheckProjectState();
    }

    private void DoConvertGroupToWav(object sender, EventArgs e)
    {
        if (dgvGroups.IsSelectionEmpty())
            return;

        var result = Utils.ShowQuestion(string.Format(Resources.FMain_DoConvertGroup, SoundUtils.WAV_EXT));
        if (result != DialogResult.Yes) 
            return;

        ChangeStatus("Konvertujem skupinu zvukov");
        tspbProgress.Visible = true;
        var group = dgvGroups.SelectedRows[0].DataBoundItem as FyzGroup;
        RegisterNewAction(new ConvertGroupEwaWawAction(this, group, false));
        ConvertSoundsInGroup(group, false);
        CheckProjectState();
    }

    private async void ConvertSoundsInGroup(FyzGroup group, bool toEwa)
    {
        var sndCount = group.Sounds.Count;

        tspbProgress.Minimum = 0;
        tspbProgress.Maximum = sndCount;
        tspbProgress.Style = ProgressBarStyle.Blocks;
        await Task.Run(() =>
        {
            RawBankExplorer.ConvertSoundIsHandled = true;
            SoundUtils.ConvertSoundsInGroup(group, toEwa, tspbProgress);
            RawBankExplorer.ConvertSoundIsHandled = false;

            Invoke(ResetStatusAfterTask);
        });
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

    private void DoRenameFile(object sender, EventArgs e)
    {
        if (dgvExplorer.IsSelectionEmpty())
            return;

        dgvExplorer.CurrentCell = dgvExplorer.SelectedRows[0].Cells[nameof(cFileName)];
        dgvExplorer.BeginEdit(true);
        _editingFileName = true;
    }

    private void DgvExplorer_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
    {
        if (!_editingFileName)
            return;

        var row = dgvExplorer.Rows[e.RowIndex].DataBoundItem as FileSystemElement;
        var newName = e.FormattedValue as string;
        e.Cancel = row switch
        {
            FileElement fe => !(fe.FileInfo.Directory != null && Utils.IsFileNameCorrect(fe.FileInfo.Directory.FullName, newName)),
            DirectoryElement de => !(de.DirInfo.Parent != null && Utils.IsFileNameCorrect(de.DirInfo.Parent.FullName, newName)),
            _ => true
        };
    }

    private void DgvExplorer_CellEndEdit(object sender, DataGridViewCellEventArgs e)
    {
        _editingFileName = false;
        dgvExplorer.EndEdit();

        var row = dgvExplorer.Rows[e.RowIndex].DataBoundItem as FileSystemElement;
        var newName = (string) dgvExplorer[e.ColumnIndex, e.RowIndex].Value;

        try
        {
            switch (row)
            {
                case FileElement fe:
                    if (string.Equals(newName, fe.FileInfo.Name, StringComparison.CurrentCultureIgnoreCase))
                        return;
                    FileSystem.RenameFile(fe.FileInfo.FullName, newName);
                    break;
                case DirectoryElement de:
                    if (string.Equals(newName, de.DirInfo.Name, StringComparison.CurrentCultureIgnoreCase))
                        return;
                    FileSystem.RenameDirectory(de.DirInfo.FullName, newName);
                    break;
            }
        }
        catch (Exception ex)
        {
            Utils.ShowError(ex.Message);
        }
    }

    private void DoDeleteFile(object sender, EventArgs e)
    {
        if (dgvExplorer.IsSelectionEmpty())
            return;

        var result =
            Utils.ShowWarning("Naozaj sa majú odstrániť všetky vybrané položky ?\n\nTáto operácia je nevratná.", MessageBoxButtons.YesNo);
        if (result == DialogResult.Yes)
        {
            foreach (DataGridViewRow row in dgvExplorer.SelectedRows)
            {
                var item = row.DataBoundItem as FileSystemElement;
                switch (item)
                {
                    case BackButtonElement:
                        break;
                    case DirectoryElement de:
                        if (de.Group != null)
                        {
                            de.Group.Language.Groups.Remove(de.Group);
                            RegisterNewAction(new RemovedGroupsAction(this, de.Group));
                        }
                        Utils.DeleteDirectoryToRecycleBin(de.DirInfo.FullName);
                        break;
                    case SoundFileElement sfe:
                        if (sfe.Sound != null)
                        {
                            SelectSound(sfe.Sound);
                            MenuSounds.Remove(sfe.Sound);
                            //sfe.Sound.Group.Sounds.Remove(sfe.Sound);
                            RegisterNewAction(new RemovedSoundsAction(this, sfe.Sound));
                            MenuGroups.ResetBindings();
                        }
                        Utils.DeleteFileToRecycleBin(sfe.FileInfo.FullName);
                        break;
                    case OtherFileElement ofe:
                        Utils.DeleteFileToRecycleBin(ofe.FileInfo.FullName);
                        break;
                }
            }
        }
    }

    private void DoConvertFilesToEwa(object sender, EventArgs e)
    {
        var result = Utils.ShowQuestion(string.Format(Resources.FMain_DoConvertFiles, SoundUtils.EWA_EXT));
        if (result != DialogResult.Yes) 
            return;

        ChangeStatus("Konvertujem vybrané súbory/priečinky");
        tspbProgress.Visible = true;
        var files = dgvExplorer.SelectedRows.Cast<FileSystemElement>().ToList();
        RegisterNewAction(new ConvertFilesEwaWawAction(this, files, true));
        ConvertFiles(files, true);
        CheckProjectState();
    }

    private void DoConvertFilesToWav(object sender, EventArgs e)
    {
        var result = Utils.ShowQuestion(string.Format(Resources.FMain_DoConvertFiles, SoundUtils.WAV_EXT));
        if (result != DialogResult.Yes) 
            return;

        ChangeStatus("Konvertujem vybrané súbory/priečinky");
        tspbProgress.Visible = true;
        var files = dgvExplorer.SelectedRows.Cast<FileSystemElement>().ToList();
        RegisterNewAction(new ConvertFilesEwaWawAction(this, files, false));
        ConvertFiles(files, false);
        CheckProjectState();
    }

    private async void ConvertFiles(IEnumerable<FileSystemElement> files, bool toEwa)
    {
        tspbProgress.Style = ProgressBarStyle.Marquee;
        await Task.Run(() =>
        {
            RawBankExplorer.ConvertSoundIsHandled = true;
            SoundUtils.ConvertFiles(files, toEwa);
            RawBankExplorer.ConvertSoundIsHandled = false;

            Invoke(ResetStatusAfterTask);
        });
    }

    #endregion

    #region SelectionManagement

    private void tscboxLanguages_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (tscboxLanguages.SelectedIndex == -1)
        {
            CurrentLanguage = null;
            return;
        }

        if (!_langAlreadySet)
        {
            
            CurrentLanguage = tscboxLanguages.SelectedItem as FyzLanguage;
            if (!bWorkerReadDat.IsBusy)
            {
                tspbProgress.Visible = true;
                tspbProgress.Style = ProgressBarStyle.Marquee;
                ChangeStatus("Načítanie súborov banky");
                bWorkerReadDat.RunWorkerAsync(tscboxLanguages.SelectedItem);
            }
        }
    }

    private void dgvGroups_SelectionChanged(object sender, EventArgs e)
    {
        if (_reorderingGroups)
            return;
        
        _actualStatusTxt = tsslStatus.Text;
        tsslStatus.Text = "Otváram skupinu zvukov";
        tsslStatus.Invalidate();
        tspbProgress.Visible = true;
        tspbProgress.Style = ProgressBarStyle.Marquee;
        
        if (!dgvGroups.IsSelectionEmpty())
        {
            CurrentGroup = (FyzGroup)dgvGroups.SelectedRows[0].DataBoundItem;

            FillExplorerList(CurrentGroup.Directory);

            MenuSounds = new ExBindingList<FyzSound>(CurrentGroup.Sounds);

            _doNotChangeExplorerSelection = true;
            dgvSounds.DataSource = MenuSounds;
            _doNotChangeExplorerSelection = false;

            foreach (DataGridViewRow row in dgvSounds.Rows) 
                ValidateRow(row.Index);

            if (!_programChange && !dgvSounds.IsSelectionEmpty())
            {
                var newPosition = new MovePosition(GetSelectedSoundRows(), CurrentLanguage, CurrentGroup);
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
            var newPosition = new MovePosition(rows, CurrentLanguage, CurrentGroup);
            var action = new SelectedCellSoundMoveAction(this, newPosition);
            moveManager.AddCommand(action);
            EnableGoBackForward();
            //_lastMovePosition = newPosition;
        }

        var item = (FyzSound)dgvSounds.Rows[dgvSounds.SelectedRows[0].Index].DataBoundItem;
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
            FillExplorerList(item.File.Parent);
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
            rows[i] = (FyzSound)row.DataBoundItem;
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
        tsmimConvertLangToEwa.Enabled = enabled && CurrentLanguage != null;
        tsmimConvertLangToWav.Enabled = enabled && CurrentLanguage != null;
        tsmiConvertLangToEwa.Enabled = enabled && CurrentLanguage != null;
        tsmiConvertLangToWav.Enabled = enabled && CurrentLanguage != null;
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
                DoSave(this, EventArgs.Empty);
                e.Cancel = false;
                break;
            case DialogResult.No:
                e.Cancel = false;
                break;
            default:
                e.Cancel = true;
                break;
        }
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
            var selectedObj = dgvGroups.SelectedRows[0].DataBoundItem as FyzGroup;

            if (sel - 1 >= 0)
            {
                _reorderingGroups = true;
                MenuGroups.RemoveAt(sel);
                MenuGroups.Insert(sel - 1, selectedObj);
                dgvGroups.ClearSelection();
                dgvGroups.Rows[sel - 1].Selected = true;
                _reorderingGroups = false;
            }
        }
        else if (e.KeyCode == Keys.PageDown)
        {
            var sel = dgvGroups.SelectedRows[0].Index;
            var selectedObj = dgvGroups.SelectedRows[0].DataBoundItem as FyzGroup;

            if (sel + 1 < CurrentLanguage.Groups.Count)
            {
                _reorderingGroups = true;
                MenuGroups.RemoveAt(sel);
                MenuGroups.Insert(sel + 1, selectedObj);
                dgvGroups.ClearSelection();
                dgvGroups.Rows[sel + 1].Selected = true;
                _reorderingGroups = false;
            }
        }

        e.Handled = true;
    }

    private void DgvGroups_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
    {
        if (e.RowIndex != -1 && e.Button.HasFlag(MouseButtons.Right)) 
            dgvGroups.Rows[e.RowIndex].Selected = true;
    }

    private async void fileSystemWatcher_Created(object sender, FileSystemEventArgs e)
    {
        if (RawBankExplorer.ConvertSoundIsHandled || RawBankExplorer.MovingSoundIsHandled)
            return;

        var newElement = RawBankExplorer.GetElement(e.FullPath, CurrentLanguage.Directory, RawBankExplorer.SearchOperation.Create);

        if (newElement is SoundFileElement sfe && GlobData.Config.AutoInsertSoundData && sfe.Parent.Group is not null)
        {
            var nameWoExt = Path.GetFileNameWithoutExtension(sfe.Name);
            var alreadyDefinedSound = sfe.Parent.Group.Sounds.FirstOrDefault(s => s.Key == nameWoExt && s.File == null);
            if (alreadyDefinedSound is not null)
            {
                alreadyDefinedSound.File = sfe;
            }
            else
            {
                var sound = new FyzSound(sfe.Parent.Group, nameWoExt, nameWoExt, sfe.Name, "", "", await SoundUtils.GetSoundDuration(sfe));
                if (GlobData.Config.ShowAfterInsertSoundDialog)
                {
                    FAfterInsertSounds.CreateOrUseExistingForm(sound);
                }
                else
                {
                    Program.MainForm.RegisterNewAction(new AddSoundAction(Program.MainForm, sound));
                    sound.Group.Sounds.Add(sound);
                    Program.MainForm.dgvSounds.ResetBindings();
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
        if (RawBankExplorer.ConvertSoundIsHandled || RawBankExplorer.MovingSoundIsHandled)
            return;

        RawBankExplorer.GetElement(e.FullPath, CurrentLanguage.Directory, RawBankExplorer.SearchOperation.Delete);

        if (e.FullPath.StartsWith(CurrentDirectory.DirInfo.FullName, StringComparison.OrdinalIgnoreCase))
            FillExplorerList(CurrentDirectory);

        CheckProjectState();
        dgvGroups.ResetBindings();
        dgvSounds.ResetBindings();
    }

    private async void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
    {
        var fileElement = RawBankExplorer.GetElement(e.FullPath, CurrentLanguage.Directory);
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
        if (RawBankExplorer.ConvertSoundIsHandled)
            return;

        var fileElement = RawBankExplorer.GetElement(e.OldFullPath, CurrentLanguage.Directory);
        
        switch (fileElement)
        {
            case null:
                break;
            case DirectoryElement de:
                de.DirInfo = new DirectoryInfo(e.FullPath);
                de.Name = Path.GetDirectoryName(e.FullPath);
                break;
            case FileElement fe:
                fe.FileInfo = new FileInfo(e.FullPath);
                fe.Name = Path.GetFileName(e.FullPath);
                break;
        }

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
            snd.File = sfe;
            snd.FileName = sfe.Name;
            sfe.Sound = snd;
            if (CurrentGroup.Directory != sfe.Parent)
            {
                snd.AdditionalRelativePath = 
                    Path.GetDirectoryName(Utils.GetRelativePath(sfe.FileInfo.FullName, CurrentLanguage.GetAbsPath(GlobData.OpenedProject.AbsPathToBank))) + Path.DirectorySeparatorChar;
            }
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
        var newAdditionalPath = (string)dgvSounds.Rows[e.RowIndex].Cells[nameof(cSoundAdditionalRelativePath)].Value;
        if (!string.IsNullOrEmpty(newAdditionalPath) && (!newAdditionalPath.EndsWith("\\") || string.IsNullOrWhiteSpace(newAdditionalPath)))
        {
            Utils.ShowError("Prídavná relatívna cesta musí končiť '\\' a nesmie obsahovať iba prázdne znaky.");
            e.Cancel = true;
            dgvSounds.Rows[e.RowIndex].Cells[nameof(cSoundAdditionalRelativePath)].Value = "";
            return;
        }
        ValidateRow(e.RowIndex, true);
    }

    private void dgvSounds_DataError(object sender, DataGridViewDataErrorEventArgs e) => Utils.ShowError("Tabuľka obsahuje nesprávny údaj: " + e.Exception.Message);

    private void dgvSounds_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
    {
        _cellUserEditing = true;
        var val = dgvSounds.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
        _cellOldValue = val == null ? "" : val.ToString();
    }

    private void dgvSounds_RowValidated(object sender, DataGridViewCellEventArgs e)
    {
        if (!_cellUserEditing)
            return;

        _cellUserEditing = false;
        var newval = dgvSounds.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
        if (newval is not string s || s == _cellOldValue)
            return;

        var action = new EditSoundAction(this, dgvSounds.Rows[e.RowIndex].DataBoundItem as FyzSound);
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
        var newFileName = (string) dgvSounds.Rows[row].Cells[nameof(cSoundFileName)].Value;
        var sound = (FyzSound) dgvSounds.Rows[row].DataBoundItem;

        if (string.IsNullOrWhiteSpace(newFileName) || !Utils.IsFileNameCorrect(sound.GetAbsPath(GlobData.OpenedProject.AbsPathToBank), newFileName, false))
        {
            if (sound.File is not null)
                sound.File.Sound = null;
            sound.File = null;
            dgvSounds.Rows[row].Cells[nameof(cSoundFileName)].ErrorText = "Neplatný názov súboru";
            return;
        }

        if (userEditing)
        {
            if(sound.File is not null)
                sound.File.Name = newFileName;
            var newPath = Utils.CombinePath(sound.Group.GetAbsPath(GlobData.OpenedProject.AbsPathToBank), newFileName);
            if (!File.Exists(newPath))
            {
                if (sound.File is not null)
                    sound.File.Sound = null;
                sound.File = null;
                dgvSounds.Rows[row].Cells[nameof(cSoundFileName)].ErrorText = "Súbor zvuku v priečinku neexistuje";
            }
            else
            {
                var fe = RawBankExplorer.GetElement(newPath, CurrentLanguage.Directory, RawBankExplorer.SearchOperation.Create);
                if (fe is SoundFileElement sfe)
                {
                    sound.File = sfe;

                    if (sound.File.Duration < 0 )
                        await SoundUtils.GetSoundDuration(sound.File);
                    if (GlobData.Config.AutoRecalculateSoundDuration) 
                        sound.Duration = sound.File.Duration;
                }
                else
                {
                    dgvSounds.Rows[row].Cells[nameof(cSoundFileName)].ErrorText = "Neplatný typ súboru";
                }
            }
            return;
        }

        if (sound.File == null || !sound.File.FileInfo.Exists)
        {
            dgvSounds.Rows[row].Cells[nameof(cSoundFileName)].ErrorText = "Súbor zvuku v priečinku neexistuje";
        }
    }

    internal async void FillExplorerList(DirectoryElement dir)
    {
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
                _explorerBack = new BackButtonElement(dir.Parent);
                ExplorerContent.Add(_explorerBack);
            }

            foreach (var element in dir.Children)
            {
                ExplorerContent.Add(element);

                if (element is SoundFileElement { Duration: -1 } sound) 
                    sound.Duration = await SoundUtils.GetSoundDuration(sound);
            }
            dgvExplorer.Refresh();
        }

        if (SelectElement != null)
        {
            var index = ExplorerContent.IndexOf(SelectElement);
            if (index == -1) 
                return;

            dgvExplorer.ClearSelection();
            dgvExplorer.Rows[index].Selected = true;
            SelectElement = null;
        }
    }

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
        var currencyManager = (CurrencyManager)BindingContext[dgvErrors.DataSource];
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
                        e.Value = _info.ToBitmap();
                        dgvErrors.Rows[e.RowIndex].Cells[nameof(cMsgType)].ToolTipText = "Informácia";
                        break;
                    case MessageType.Warning:
                        e.Value = _warning.ToBitmap();
                        dgvErrors.Rows[e.RowIndex].Cells[nameof(cMsgType)].ToolTipText = "Upozornenie";
                        break;
                    case MessageType.Error:
                        e.Value = _error.ToBitmap();
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

        ((IRawBankMessage) dgvErrors.Rows[e.RowIndex].DataBoundItem).Show();
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
            Path.GetFullPath(GlobData.OpenedProject.AbsPathToBank + CurrentLanguage.RelativePath).TrimEnd('\\'),
            StringComparison.InvariantCultureIgnoreCase) == 0;
    }

    private void Messages_ListChanged(object sender, ListChangedEventArgs e)
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
            tsbErrors.Text = $"{errorCount} chýb";
            tsbWarnings.Text = $"{warningCount} upozornení";
            tsbInfos.Text = $"{infoCount} správ";

            if (errorCount != 0)
            {
                tssbErrors.Image = _error.ToBitmap();
                tssbErrors.Text = $"{errorCount} chýb";
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
        RawBankExplorer.MergeFilesAndData(Root, CurrentLanguage, GlobData.OpenedProject.Messages, true);
        timerToCheck.Stop();
        timerToCheck.Enabled = false;

        _messages.Clear();
        try
        {
            foreach (var msg in GlobData.OpenedProject.Messages[CurrentLanguage])
            {
                _messages.Add(msg);
            }
        }
        catch (InvalidOperationException)
        {
            //ignored
        }
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