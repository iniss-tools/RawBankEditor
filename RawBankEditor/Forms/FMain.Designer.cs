
using ExControls;
using RawBankEditor.Entities;
using RawBankEditor.Tools;
using ToolsCore.Iniss.Entities;
using ToolsCore.Icons;

namespace RawBankEditor.Forms
{
    public partial class FMain
    {
        /// <summary>
        /// Vyžaduje se proměnná návrháře.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Uvolněte všechny používané prostředky.
        /// </summary>
        /// <param name="disposing">hodnota true, když by se měl spravovaný prostředek odstranit; jinak false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kód generovaný Návrhářem Windows Form

        /// <summary>
        /// Metoda vyžadovaná pro podporu Návrháře - neupravovat
        /// obsah této metody v editoru kódu.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FMain));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            this.titleBar = new ExControls.ExTitleBar();
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.tsmiFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmimOpen = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmimRecent = new ToolsCore.Icons.IconToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmimSave = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmimSaveAll = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmiEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmimUndo = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmimRedo = new ToolsCore.Icons.IconToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmimAddSound = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmimDeleteSound = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmimMoveSounds = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmimConvertSoundsToEwa = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmimConvertSoundsToWav = new ToolsCore.Icons.IconToolStripMenuItem();
            this.toolStripSeparator26 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmimRewriteMode = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmimWrapTextSoundCol = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmiShow = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmimGoBack = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmimGoForward = new ToolsCore.Icons.IconToolStripMenuItem();
            this.toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmimSearch = new ToolsCore.Icons.IconToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmimShowErrors = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmiTools = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmimLangsSettings = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmimAddLanguage = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmimEditLanguage = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmimDeleteLanguage = new ToolsCore.Icons.IconToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmimConvertLangToEwa = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmimConvertLangToWav = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmimSepImportExport = new System.Windows.Forms.ToolStripSeparator();
            this.tsmimImportSounds = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmimExportIniss2 = new ToolsCore.Icons.IconToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmimAppSettings = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmiHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmimInfoApp = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmimUpdates = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel2 = new ExControls.ExTableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.toolStripGroups = new System.Windows.Forms.ToolStrip();
            this.tsbAddGroup = new ToolsCore.Icons.IconToolStripButton();
            this.tsbEditGroup = new ToolsCore.Icons.IconToolStripButton();
            this.tsbDeleteGroup = new ToolsCore.Icons.IconToolStripButton();
            this.toolStripSeparator21 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbConvertGroupToEwa = new ToolsCore.Icons.IconToolStripButton();
            this.tsbConvertGroupToWav = new ToolsCore.Icons.IconToolStripButton();
            this.dgvGroups = new System.Windows.Forms.DataGridView();
            this.cGroupName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cGroupRelativePath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cGroupCountSounds = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuGroups = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmiAddGroup = new ToolsCore.Icons.IconToolStripMenuItem();
            this.cmiEditGroup = new ToolsCore.Icons.IconToolStripMenuItem();
            this.cmiDeleteGroup = new ToolsCore.Icons.IconToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.cmiConvertGroupToEwa = new ToolsCore.Icons.IconToolStripMenuItem();
            this.cmiConvertGroupToWav = new ToolsCore.Icons.IconToolStripMenuItem();
            this.fyzGroupBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tableLayoutPanel3 = new ExControls.ExTableLayoutPanel();
            this.dgvExplorer = new System.Windows.Forms.DataGridView();
            this.cFileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cFileType = new System.Windows.Forms.DataGridViewImageColumn();
            this.cFileDuration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuExplorer = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmiOpenInExplorer = new ToolsCore.Icons.IconToolStripMenuItem();
            this.cmiPlay = new ToolsCore.Icons.IconToolStripMenuItem();
            this.toolStripSeparator20 = new System.Windows.Forms.ToolStripSeparator();
            this.cmiRenameFileDir = new ToolsCore.Icons.IconToolStripMenuItem();
            this.cmiDeleteFileDir = new ToolsCore.Icons.IconToolStripMenuItem();
            this.toolStripSeparator23 = new System.Windows.Forms.ToolStripSeparator();
            this.cmiConvertToEwaFile = new ToolsCore.Icons.IconToolStripMenuItem();
            this.cmiConvertToWavFile = new ToolsCore.Icons.IconToolStripMenuItem();
            this.fileSystemElementBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.toolStripExplorer = new System.Windows.Forms.ToolStrip();
            this.tsbOpenInExplorer = new ToolsCore.Icons.IconToolStripButton();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbPlay = new ToolsCore.Icons.IconToolStripButton();
            this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbRenameFileDir = new ToolsCore.Icons.IconToolStripButton();
            this.tsbDeleteFileDir = new ToolsCore.Icons.IconToolStripButton();
            this.toolStripSeparator22 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbConvertFilesToEwa = new ToolsCore.Icons.IconToolStripButton();
            this.tsbConvertFilesToWav = new ToolsCore.Icons.IconToolStripButton();
            this.splitSoundsErrors = new System.Windows.Forms.SplitContainer();
            this.dgvSounds = new System.Windows.Forms.DataGridView();
            this.cSoundKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cSoundName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cSoundAdditionalRelativePath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cSoundFileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cSoundDuration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cSoundText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuSounds = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmiAddSound = new ToolsCore.Icons.IconToolStripMenuItem();
            this.cmiDeleteSound = new ToolsCore.Icons.IconToolStripMenuItem();
            this.cmiMoveSounds = new ToolsCore.Icons.IconToolStripMenuItem();
            this.toolStripSeparator19 = new System.Windows.Forms.ToolStripSeparator();
            this.cmiConvertSoundsToEwa = new ToolsCore.Icons.IconToolStripMenuItem();
            this.cmiConvertSoundsToWav = new ToolsCore.Icons.IconToolStripMenuItem();
            this.fyzSoundBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tableLayoutPanel4 = new ExControls.ExTableLayoutPanel();
            this.toolStripErrors = new System.Windows.Forms.ToolStrip();
            this.tsbErrors = new ToolsCore.Icons.IconToolStripButton();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbWarnings = new ToolsCore.Icons.IconToolStripButton();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbInfos = new ToolsCore.Icons.IconToolStripButton();
            this.tsbResolveProblem = new ToolsCore.Icons.IconToolStripButton();
            this.tsbHighlightProblem = new ToolsCore.Icons.IconToolStripButton();
            this.dgvErrors = new System.Windows.Forms.DataGridView();
            this.cMsgType = new System.Windows.Forms.DataGridViewImageColumn();
            this.cMsgCode = new System.Windows.Forms.DataGridViewLinkColumn();
            this.cMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cMsgResolve = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cMsgPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuErrors = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmiHighlightProblem = new ToolsCore.Icons.IconToolStripMenuItem();
            this.cmiResolveProblem = new ToolsCore.Icons.IconToolStripMenuItem();
            this.rawBankMessageBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tableLayoutPanel5 = new ExControls.ExTableLayoutPanel();
            this.mmErrors = new RawBankEditor.Controls.MinButton();
            this.label3 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new ExControls.ExTableLayoutPanel();
            this.statusStripMain = new System.Windows.Forms.StatusStrip();
            this.tsslStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tspbProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssbErrors = new ToolsCore.Icons.IconToolStripDropDownButton();
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.tsbOpen = new ToolsCore.Icons.IconToolStripButton();
            this.tsbRecent = new ToolsCore.Icons.IconToolStripDropDownButton();
            this.toolStripSeparator25 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbSave = new ToolsCore.Icons.IconToolStripButton();
            this.tsbSaveAll = new ToolsCore.Icons.IconToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbGoBack = new ToolsCore.Icons.IconToolStripSplitButton();
            this.tsbGoForward = new ToolsCore.Icons.IconToolStripButton();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbUndo = new ToolsCore.Icons.IconToolStripButton();
            this.tsbRedo = new ToolsCore.Icons.IconToolStripButton();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.tscboxLanguages = new ExControls.ExToolStripComboBox();
            this.tsbLangsSettings = new ToolsCore.Icons.IconToolStripDropDownButton();
            this.tsmiAddLanguage = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmiEditLanguage = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmiDeleteLanguage = new ToolsCore.Icons.IconToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiConvertLangToEwa = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmiConvertLangToWav = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmiSepImportExport = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiImportSounds = new ToolsCore.Icons.IconToolStripMenuItem();
            this.tsmiExportIniss2 = new ToolsCore.Icons.IconToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbAppSettings = new ToolsCore.Icons.IconToolStripButton();
            this.tsbInfoApp = new ToolsCore.Icons.IconToolStripButton();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbAddSound = new ToolsCore.Icons.IconToolStripButton();
            this.tsbMoveSounds = new ToolsCore.Icons.IconToolStripButton();
            this.tsbDeleteSound = new ToolsCore.Icons.IconToolStripButton();
            this.toolStripSeparator24 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbConvertSoundsToEwa = new ToolsCore.Icons.IconToolStripButton();
            this.tsbConvertSoundsToWav = new ToolsCore.Icons.IconToolStripButton();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbSearch = new ToolsCore.Icons.IconToolStripButton();
            this.tsbWrapTextSoundCol = new ToolsCore.Icons.IconToolStripButton();
            this.tsbRewriteMode = new ToolsCore.Icons.IconToolStripButton();
            this.undoActionChooser = new ExControls.ToolStripUndoRedoActionChooser();
            this.fileSystemWatcher = new System.IO.FileSystemWatcher();
            this.moveManager = new ExControls.Providers.BackwardForwardProvider();
            this.changeManager = new ExControls.Providers.UndoRedoManager();
            this.timerToCheck = new System.Windows.Forms.Timer(this.components);
            this.titleBar.SuspendLayout();
            this.menuStripMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.toolStripGroups.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGroups)).BeginInit();
            this.contextMenuGroups.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fyzGroupBindingSource)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvExplorer)).BeginInit();
            this.contextMenuExplorer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemElementBindingSource)).BeginInit();
            this.toolStripExplorer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitSoundsErrors)).BeginInit();
            this.splitSoundsErrors.Panel1.SuspendLayout();
            this.splitSoundsErrors.Panel2.SuspendLayout();
            this.splitSoundsErrors.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSounds)).BeginInit();
            this.contextMenuSounds.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fyzSoundBindingSource)).BeginInit();
            this.tableLayoutPanel4.SuspendLayout();
            this.toolStripErrors.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvErrors)).BeginInit();
            this.contextMenuErrors.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rawBankMessageBindingSource)).BeginInit();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.statusStripMain.SuspendLayout();
            this.toolStripMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher)).BeginInit();
            this.SuspendLayout();
            // 
            // titleBar
            // 
            this.titleBar.Controls.Add(this.menuStripMain);
            resources.ApplyResources(this.titleBar, "titleBar");
            this.titleBar.Name = "titleBar";
            // 
            // menuStripMain
            // 
            this.menuStripMain.BackColor = System.Drawing.SystemColors.Control;
            this.menuStripMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiFile,
            this.tsmiEdit,
            this.tsmiShow,
            this.tsmiTools,
            this.tsmiHelp});
            resources.ApplyResources(this.menuStripMain, "menuStripMain");
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStripMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            // 
            // tsmiFile
            // 
            this.tsmiFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmimOpen,
            this.tsmimRecent,
            this.toolStripSeparator2,
            this.tsmimSave,
            this.tsmimSaveAll});
            this.tsmiFile.Name = "tsmiFile";
            resources.ApplyResources(this.tsmiFile, "tsmiFile");
            // 
            // tsmimOpen
            // 
            this.tsmimOpen.Name = "tsmimOpen";
            this.tsmimOpen.IconName = "folder-open";
            resources.ApplyResources(this.tsmimOpen, "tsmimOpen");
            // 
            // tsmimRecent
            // 
            this.tsmimRecent.Name = "tsmimRecent";
            this.tsmimRecent.IconName = "history";
            resources.ApplyResources(this.tsmimRecent, "tsmimRecent");
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // tsmimSave
            // 
            this.tsmimSave.Enabled = false;
            this.tsmimSave.Name = "tsmimSave";
            this.tsmimSave.IconName = "save";
            resources.ApplyResources(this.tsmimSave, "tsmimSave");
            // 
            // tsmimSaveAll
            // 
            this.tsmimSaveAll.Enabled = false;
            this.tsmimSaveAll.Name = "tsmimSaveAll";
            this.tsmimSaveAll.IconName = "save-all";
            resources.ApplyResources(this.tsmimSaveAll, "tsmimSaveAll");
            // 
            // tsmiEdit
            // 
            this.tsmiEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmimUndo,
            this.tsmimRedo,
            this.toolStripSeparator11,
            this.tsmimAddSound,
            this.tsmimDeleteSound,
            this.tsmimMoveSounds,
            this.tsmimConvertSoundsToEwa,
            this.tsmimConvertSoundsToWav,
            this.toolStripSeparator26,
            this.tsmimRewriteMode,
            this.tsmimWrapTextSoundCol});
            this.tsmiEdit.Name = "tsmiEdit";
            resources.ApplyResources(this.tsmiEdit, "tsmiEdit");
            // 
            // tsmimUndo
            // 
            this.tsmimUndo.Enabled = false;
            this.tsmimUndo.Name = "tsmimUndo";
            this.tsmimUndo.IconName = "undo";
            resources.ApplyResources(this.tsmimUndo, "tsmimUndo");
            // 
            // tsmimRedo
            // 
            this.tsmimRedo.Enabled = false;
            this.tsmimRedo.Name = "tsmimRedo";
            this.tsmimRedo.IconName = "redo";
            resources.ApplyResources(this.tsmimRedo, "tsmimRedo");
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            resources.ApplyResources(this.toolStripSeparator11, "toolStripSeparator11");
            // 
            // tsmimAddSound
            // 
            this.tsmimAddSound.Enabled = false;
            this.tsmimAddSound.Name = "tsmimAddSound";
            this.tsmimAddSound.IconName = "add";
            resources.ApplyResources(this.tsmimAddSound, "tsmimAddSound");
            // 
            // tsmimDeleteSound
            // 
            this.tsmimDeleteSound.Enabled = false;
            this.tsmimDeleteSound.Name = "tsmimDeleteSound";
            this.tsmimDeleteSound.IconName = "delete";
            resources.ApplyResources(this.tsmimDeleteSound, "tsmimDeleteSound");
            // 
            // tsmimMoveSounds
            // 
            this.tsmimMoveSounds.Enabled = false;
            this.tsmimMoveSounds.Name = "tsmimMoveSounds";
            this.tsmimMoveSounds.IconName = "move";
            resources.ApplyResources(this.tsmimMoveSounds, "tsmimMoveSounds");
            // 
            // tsmimConvertSoundsToEwa
            // 
            this.tsmimConvertSoundsToEwa.Enabled = false;
            this.tsmimConvertSoundsToEwa.Name = "tsmimConvertSoundsToEwa";
            this.tsmimConvertSoundsToEwa.IconName = "file-ewa";
            resources.ApplyResources(this.tsmimConvertSoundsToEwa, "tsmimConvertSoundsToEwa");
            // 
            // tsmimConvertSoundsToWav
            // 
            this.tsmimConvertSoundsToWav.Enabled = false;
            this.tsmimConvertSoundsToWav.Name = "tsmimConvertSoundsToWav";
            this.tsmimConvertSoundsToWav.IconName = "file-wav";
            resources.ApplyResources(this.tsmimConvertSoundsToWav, "tsmimConvertSoundsToWav");
            // 
            // toolStripSeparator26
            // 
            this.toolStripSeparator26.Name = "toolStripSeparator26";
            resources.ApplyResources(this.toolStripSeparator26, "toolStripSeparator26");
            // 
            // tsmimRewriteMode
            // 
            this.tsmimRewriteMode.CheckOnClick = true;
            this.tsmimRewriteMode.Enabled = false;
            this.tsmimRewriteMode.Name = "tsmimRewriteMode";
            this.tsmimRewriteMode.IconName = "link";
            resources.ApplyResources(this.tsmimRewriteMode, "tsmimRewriteMode");
            this.tsmimRewriteMode.CheckedChanged += new System.EventHandler(this.RewriteModeChanged);
            // 
            // tsmimWrapTextSoundCol
            // 
            this.tsmimWrapTextSoundCol.CheckOnClick = true;
            this.tsmimWrapTextSoundCol.Name = "tsmimWrapTextSoundCol";
            this.tsmimWrapTextSoundCol.IconName = "text-wrap";
            resources.ApplyResources(this.tsmimWrapTextSoundCol, "tsmimWrapTextSoundCol");
            this.tsmimWrapTextSoundCol.CheckedChanged += new System.EventHandler(this.WrapSoundTextChanged);
            // 
            // tsmiShow
            // 
            this.tsmiShow.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmimGoBack,
            this.tsmimGoForward,
            this.toolStripSeparator18,
            this.tsmimSearch,
            this.toolStripSeparator3,
            this.tsmimShowErrors});
            this.tsmiShow.Name = "tsmiShow";
            resources.ApplyResources(this.tsmiShow, "tsmiShow");
            // 
            // tsmimGoBack
            // 
            this.tsmimGoBack.Enabled = false;
            this.tsmimGoBack.Name = "tsmimGoBack";
            this.tsmimGoBack.IconName = "back";
            resources.ApplyResources(this.tsmimGoBack, "tsmimGoBack");
            // 
            // tsmimGoForward
            // 
            this.tsmimGoForward.Enabled = false;
            this.tsmimGoForward.Name = "tsmimGoForward";
            this.tsmimGoForward.IconName = "forward";
            resources.ApplyResources(this.tsmimGoForward, "tsmimGoForward");
            // 
            // toolStripSeparator18
            // 
            this.toolStripSeparator18.Name = "toolStripSeparator18";
            resources.ApplyResources(this.toolStripSeparator18, "toolStripSeparator18");
            // 
            // tsmimSearch
            // 
            this.tsmimSearch.Enabled = false;
            this.tsmimSearch.Name = "tsmimSearch";
            this.tsmimSearch.IconName = "search";
            resources.ApplyResources(this.tsmimSearch, "tsmimSearch");
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // tsmimShowErrors
            // 
            this.tsmimShowErrors.Checked = true;
            this.tsmimShowErrors.CheckOnClick = true;
            this.tsmimShowErrors.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmimShowErrors.Name = "tsmimShowErrors";
            this.tsmimShowErrors.IconName = "status-error";
            resources.ApplyResources(this.tsmimShowErrors, "tsmimShowErrors");
            this.tsmimShowErrors.CheckedChanged += new System.EventHandler(this.TsmimShowErrors_CheckedChanged);
            // 
            // tsmiTools
            // 
            this.tsmiTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmimLangsSettings,
            this.toolStripSeparator8,
            this.tsmimAppSettings});
            this.tsmiTools.Name = "tsmiTools";
            resources.ApplyResources(this.tsmiTools, "tsmiTools");
            // 
            // tsmimLangsSettings
            // 
            this.tsmimLangsSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmimAddLanguage,
            this.tsmimEditLanguage,
            this.tsmimDeleteLanguage,
            this.toolStripSeparator6,
            this.tsmimConvertLangToEwa,
            this.tsmimConvertLangToWav,
            this.tsmimSepImportExport,
            this.tsmimImportSounds,
            this.tsmimExportIniss2});
            this.tsmimLangsSettings.Enabled = false;
            this.tsmimLangsSettings.Name = "tsmimLangsSettings";
            this.tsmimLangsSettings.IconName = "local-language";
            resources.ApplyResources(this.tsmimLangsSettings, "tsmimLangsSettings");
            // 
            // tsmimAddLanguage
            // 
            this.tsmimAddLanguage.Enabled = false;
            this.tsmimAddLanguage.Name = "tsmimAddLanguage";
            this.tsmimAddLanguage.IconName = "add";
            resources.ApplyResources(this.tsmimAddLanguage, "tsmimAddLanguage");
            // 
            // tsmimEditLanguage
            // 
            this.tsmimEditLanguage.Enabled = false;
            this.tsmimEditLanguage.Name = "tsmimEditLanguage";
            this.tsmimEditLanguage.IconName = "edit";
            resources.ApplyResources(this.tsmimEditLanguage, "tsmimEditLanguage");
            // 
            // tsmimDeleteLanguage
            // 
            this.tsmimDeleteLanguage.Enabled = false;
            this.tsmimDeleteLanguage.Name = "tsmimDeleteLanguage";
            this.tsmimDeleteLanguage.IconName = "delete";
            resources.ApplyResources(this.tsmimDeleteLanguage, "tsmimDeleteLanguage");
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
            // 
            // tsmimConvertLangToEwa
            // 
            this.tsmimConvertLangToEwa.Enabled = false;
            this.tsmimConvertLangToEwa.Name = "tsmimConvertLangToEwa";
            this.tsmimConvertLangToEwa.IconName = "file-ewa";
            resources.ApplyResources(this.tsmimConvertLangToEwa, "tsmimConvertLangToEwa");
            // 
            // tsmimConvertLangToWav
            // 
            this.tsmimConvertLangToWav.Enabled = false;
            this.tsmimConvertLangToWav.Name = "tsmimConvertLangToWav";
            this.tsmimConvertLangToWav.IconName = "file-wav";
            resources.ApplyResources(this.tsmimConvertLangToWav, "tsmimConvertLangToWav");
            // 
            // tsmimSepImportExport
            // 
            this.tsmimSepImportExport.Name = "tsmimSepImportExport";
            resources.ApplyResources(this.tsmimSepImportExport, "tsmimSepImportExport");
            // 
            // tsmimImportSounds
            // 
            this.tsmimImportSounds.Enabled = false;
            this.tsmimImportSounds.Name = "tsmimImportSounds";
            this.tsmimImportSounds.IconName = "import";
            resources.ApplyResources(this.tsmimImportSounds, "tsmimImportSounds");
            // 
            // tsmimExportIniss2
            // 
            this.tsmimExportIniss2.Enabled = false;
            this.tsmimExportIniss2.Name = "tsmimExportIniss2";
            this.tsmimExportIniss2.IconName = "export";
            resources.ApplyResources(this.tsmimExportIniss2, "tsmimExportIniss2");
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            resources.ApplyResources(this.toolStripSeparator8, "toolStripSeparator8");
            // 
            // tsmimAppSettings
            // 
            this.tsmimAppSettings.Name = "tsmimAppSettings";
            this.tsmimAppSettings.IconName = "settings";
            resources.ApplyResources(this.tsmimAppSettings, "tsmimAppSettings");
            // 
            // tsmiHelp
            // 
            this.tsmiHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmimInfoApp,
            this.tsmimUpdates});
            this.tsmiHelp.Name = "tsmiHelp";
            resources.ApplyResources(this.tsmiHelp, "tsmiHelp");
            // 
            // tsmimInfoApp
            // 
            this.tsmimInfoApp.Name = "tsmimInfoApp";
            this.tsmimInfoApp.IconName = "info";
            resources.ApplyResources(this.tsmimInfoApp, "tsmimInfoApp");
            // 
            // tsmimUpdates
            // 
            this.tsmimUpdates.Name = "tsmimUpdates";
            resources.ApplyResources(this.tsmimUpdates, "tsmimUpdates");
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitSoundsErrors);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.splitContainer1.Size = new System.Drawing.Size(1091, 502);
            this.splitContainer1.SplitterDistance = 296;
            this.splitContainer1.SplitterWidth = 6;
            this.splitContainer1.TabIndex = 1;
            // 
            // splitContainer2
            // 
            resources.ApplyResources(this.splitContainer2, "splitContainer2");
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(2);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.tableLayoutPanel2);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tableLayoutPanel3);
            this.splitContainer2.Size = new System.Drawing.Size(292, 502);
            this.splitContainer2.SplitterDistance = 186;
            this.splitContainer2.SplitterWidth = 6;
            this.splitContainer2.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.BorderColor = System.Drawing.Color.Empty;
            this.tableLayoutPanel2.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.toolStripGroups, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.dgvGroups, 0, 2);
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripGroups
            // 
            this.toolStripGroups.BackColor = System.Drawing.SystemColors.MenuBar;
            this.toolStripGroups.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripGroups.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStripGroups.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbAddGroup,
            this.tsbEditGroup,
            this.tsbDeleteGroup,
            this.toolStripSeparator21,
            this.tsbConvertGroupToEwa,
            this.tsbConvertGroupToWav});
            resources.ApplyResources(this.toolStripGroups, "toolStripGroups");
            this.toolStripGroups.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            this.toolStripGroups.Name = "toolStripGroups";
            // 
            // tsbAddGroup
            // 
            this.tsbAddGroup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAddGroup.Enabled = false;
            this.tsbAddGroup.Name = "tsbAddGroup";
            this.tsbAddGroup.IconName = "add";
            resources.ApplyResources(this.tsbAddGroup, "tsbAddGroup");
            // 
            // tsbEditGroup
            // 
            this.tsbEditGroup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbEditGroup.Enabled = false;
            this.tsbEditGroup.Name = "tsbEditGroup";
            this.tsbEditGroup.IconName = "edit";
            resources.ApplyResources(this.tsbEditGroup, "tsbEditGroup");
            // 
            // tsbDeleteGroup
            // 
            this.tsbDeleteGroup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDeleteGroup.Enabled = false;
            this.tsbDeleteGroup.Name = "tsbDeleteGroup";
            this.tsbDeleteGroup.IconName = "delete";
            resources.ApplyResources(this.tsbDeleteGroup, "tsbDeleteGroup");
            // 
            // toolStripSeparator21
            // 
            this.toolStripSeparator21.Name = "toolStripSeparator21";
            resources.ApplyResources(this.toolStripSeparator21, "toolStripSeparator21");
            // 
            // tsbConvertGroupToEwa
            // 
            this.tsbConvertGroupToEwa.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbConvertGroupToEwa.Enabled = false;
            this.tsbConvertGroupToEwa.Name = "tsbConvertGroupToEwa";
            this.tsbConvertGroupToEwa.IconName = "file-ewa";
            resources.ApplyResources(this.tsbConvertGroupToEwa, "tsbConvertGroupToEwa");
            // 
            // tsbConvertGroupToWav
            // 
            this.tsbConvertGroupToWav.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbConvertGroupToWav.Enabled = false;
            this.tsbConvertGroupToWav.Name = "tsbConvertGroupToWav";
            this.tsbConvertGroupToWav.IconName = "file-wav";
            resources.ApplyResources(this.tsbConvertGroupToWav, "tsbConvertGroupToWav");
            // 
            // dgvGroups
            // 
            this.dgvGroups.AllowUserToAddRows = false;
            this.dgvGroups.AllowUserToDeleteRows = false;
            this.dgvGroups.AllowUserToResizeRows = false;
            this.dgvGroups.AutoGenerateColumns = false;
            this.dgvGroups.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cGroupName,
            this.cGroupRelativePath,
            this.cGroupCountSounds});
            this.dgvGroups.ContextMenuStrip = this.contextMenuGroups;
            this.dgvGroups.DataSource = this.fyzGroupBindingSource;
            resources.ApplyResources(this.dgvGroups, "dgvGroups");
            this.dgvGroups.Margin = new System.Windows.Forms.Padding(2);
            this.dgvGroups.MultiSelect = false;
            this.dgvGroups.Name = "dgvGroups";
            this.dgvGroups.ReadOnly = true;
            this.dgvGroups.RowHeadersVisible = false;
            this.dgvGroups.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvGroups.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DgvGroups_CellMouseDown);
            this.dgvGroups.SelectionChanged += new System.EventHandler(this.dgvGroups_SelectionChanged);
            this.dgvGroups.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DgvGroups_KeyDown);
            // 
            // cGroupName
            // 
            this.cGroupName.DataPropertyName = "Name";
            resources.ApplyResources(this.cGroupName, "cGroupName");
            this.cGroupName.Name = "cGroupName";
            this.cGroupName.ReadOnly = true;
            // 
            // cGroupRelativePath
            // 
            this.cGroupRelativePath.DataPropertyName = "RelativePath";
            resources.ApplyResources(this.cGroupRelativePath, "cGroupRelativePath");
            this.cGroupRelativePath.Name = "cGroupRelativePath";
            this.cGroupRelativePath.ReadOnly = true;
            // 
            // cGroupCountSounds
            // 
            this.cGroupCountSounds.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cGroupCountSounds.DataPropertyName = "CountSounds";
            resources.ApplyResources(this.cGroupCountSounds, "cGroupCountSounds");
            this.cGroupCountSounds.Name = "cGroupCountSounds";
            this.cGroupCountSounds.ReadOnly = true;
            // 
            // contextMenuGroups
            // 
            this.contextMenuGroups.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuGroups.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmiAddGroup,
            this.cmiEditGroup,
            this.cmiDeleteGroup,
            this.toolStripSeparator4,
            this.cmiConvertGroupToEwa,
            this.cmiConvertGroupToWav});
            this.contextMenuGroups.Name = "contextMenuGroups";
            resources.ApplyResources(this.contextMenuGroups, "contextMenuGroups");
            // 
            // cmiAddGroup
            // 
            this.cmiAddGroup.Enabled = false;
            this.cmiAddGroup.Name = "cmiAddGroup";
            this.cmiAddGroup.IconName = "add";
            resources.ApplyResources(this.cmiAddGroup, "cmiAddGroup");
            // 
            // cmiEditGroup
            // 
            this.cmiEditGroup.Enabled = false;
            this.cmiEditGroup.Name = "cmiEditGroup";
            this.cmiEditGroup.IconName = "edit";
            resources.ApplyResources(this.cmiEditGroup, "cmiEditGroup");
            // 
            // cmiDeleteGroup
            // 
            this.cmiDeleteGroup.Enabled = false;
            this.cmiDeleteGroup.Name = "cmiDeleteGroup";
            this.cmiDeleteGroup.IconName = "delete";
            resources.ApplyResources(this.cmiDeleteGroup, "cmiDeleteGroup");
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // cmiConvertGroupToEwa
            // 
            this.cmiConvertGroupToEwa.Enabled = false;
            this.cmiConvertGroupToEwa.Name = "cmiConvertGroupToEwa";
            this.cmiConvertGroupToEwa.IconName = "file-ewa";
            resources.ApplyResources(this.cmiConvertGroupToEwa, "cmiConvertGroupToEwa");
            // 
            // cmiConvertGroupToWav
            // 
            this.cmiConvertGroupToWav.Enabled = false;
            this.cmiConvertGroupToWav.Name = "cmiConvertGroupToWav";
            this.cmiConvertGroupToWav.IconName = "file-wav";
            resources.ApplyResources(this.cmiConvertGroupToWav, "cmiConvertGroupToWav");
            // 
            // fyzGroupBindingSource
            // 
            this.fyzGroupBindingSource.DataSource = typeof(FyzGroup);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.BorderColor = System.Drawing.Color.Empty;
            this.tableLayoutPanel3.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.dgvExplorer, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.toolStripExplorer, 0, 1);
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            // 
            // dgvExplorer
            // 
            this.dgvExplorer.AllowUserToAddRows = false;
            this.dgvExplorer.AllowUserToDeleteRows = false;
            this.dgvExplorer.AllowUserToOrderColumns = true;
            this.dgvExplorer.AllowUserToResizeRows = false;
            this.dgvExplorer.AutoGenerateColumns = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvExplorer.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvExplorer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvExplorer.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cFileName,
            this.cFileType,
            this.cFileDuration});
            this.dgvExplorer.ContextMenuStrip = this.contextMenuExplorer;
            this.dgvExplorer.DataSource = this.fileSystemElementBindingSource;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvExplorer.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.dgvExplorer, "dgvExplorer");
            this.dgvExplorer.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvExplorer.Margin = new System.Windows.Forms.Padding(2);
            this.dgvExplorer.Name = "dgvExplorer";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvExplorer.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvExplorer.RowHeadersVisible = false;
            this.dgvExplorer.RowHeadersWidth = 51;
            this.dgvExplorer.RowTemplate.Height = 24;
            this.dgvExplorer.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvExplorer.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvExplorer_CellClick);
            this.dgvExplorer.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvExplorer_CellDoubleClick);
            this.dgvExplorer.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvExplorer_CellEndEdit);
            this.dgvExplorer.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvExplorer_CellFormatting);
            this.dgvExplorer.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.DgvExplorer_CellValidating);
            this.dgvExplorer.SelectionChanged += new System.EventHandler(this.dgvExplorer_SelectionChanged);
            this.dgvExplorer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DgvExplorer_KeyDown);
            // 
            // cFileName
            // 
            this.cFileName.DataPropertyName = "Name";
            resources.ApplyResources(this.cFileName, "cFileName");
            this.cFileName.MinimumWidth = 100;
            this.cFileName.Name = "cFileName";
            // 
            // cFileType
            // 
            resources.ApplyResources(this.cFileType, "cFileType");
            this.cFileType.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Normal;
            this.cFileType.MinimumWidth = 50;
            this.cFileType.Name = "cFileType";
            this.cFileType.ReadOnly = true;
            // 
            // cFileDuration
            // 
            this.cFileDuration.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.cFileDuration, "cFileDuration");
            this.cFileDuration.MinimumWidth = 6;
            this.cFileDuration.Name = "cFileDuration";
            this.cFileDuration.ReadOnly = true;
            // 
            // contextMenuExplorer
            // 
            this.contextMenuExplorer.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmiOpenInExplorer,
            this.cmiPlay,
            this.toolStripSeparator20,
            this.cmiRenameFileDir,
            this.cmiDeleteFileDir,
            this.toolStripSeparator23,
            this.cmiConvertToEwaFile,
            this.cmiConvertToWavFile});
            this.contextMenuExplorer.Name = "contextMenuExplorer";
            resources.ApplyResources(this.contextMenuExplorer, "contextMenuExplorer");
            // 
            // cmiOpenInExplorer
            // 
            this.cmiOpenInExplorer.Enabled = false;
            this.cmiOpenInExplorer.Name = "cmiOpenInExplorer";
            this.cmiOpenInExplorer.IconName = "folder-open";
            resources.ApplyResources(this.cmiOpenInExplorer, "cmiOpenInExplorer");
            // 
            // cmiPlay
            // 
            this.cmiPlay.Enabled = false;
            this.cmiPlay.Name = "cmiPlay";
            this.cmiPlay.IconName = "speaker";
            this.cmiPlay.ShortcutKeyDisplayString = "F5";
            resources.ApplyResources(this.cmiPlay, "cmiPlay");
            // 
            // toolStripSeparator20
            // 
            this.toolStripSeparator20.Name = "toolStripSeparator20";
            resources.ApplyResources(this.toolStripSeparator20, "toolStripSeparator20");
            // 
            // cmiRenameFileDir
            // 
            this.cmiRenameFileDir.Enabled = false;
            this.cmiRenameFileDir.Name = "cmiRenameFileDir";
            this.cmiRenameFileDir.IconName = "rename";
            this.cmiRenameFileDir.ShortcutKeyDisplayString = "F2";
            resources.ApplyResources(this.cmiRenameFileDir, "cmiRenameFileDir");
            // 
            // cmiDeleteFileDir
            // 
            this.cmiDeleteFileDir.Enabled = false;
            this.cmiDeleteFileDir.Name = "cmiDeleteFileDir";
            this.cmiDeleteFileDir.IconName = "delete";
            this.cmiDeleteFileDir.ShortcutKeyDisplayString = "Del";
            resources.ApplyResources(this.cmiDeleteFileDir, "cmiDeleteFileDir");
            // 
            // toolStripSeparator23
            // 
            this.toolStripSeparator23.Name = "toolStripSeparator23";
            resources.ApplyResources(this.toolStripSeparator23, "toolStripSeparator23");
            // 
            // cmiConvertToEwaFile
            // 
            this.cmiConvertToEwaFile.Enabled = false;
            this.cmiConvertToEwaFile.Name = "cmiConvertToEwaFile";
            this.cmiConvertToEwaFile.IconName = "file-ewa";
            resources.ApplyResources(this.cmiConvertToEwaFile, "cmiConvertToEwaFile");
            // 
            // cmiConvertToWavFile
            // 
            this.cmiConvertToWavFile.Enabled = false;
            this.cmiConvertToWavFile.Name = "cmiConvertToWavFile";
            this.cmiConvertToWavFile.IconName = "file-wav";
            resources.ApplyResources(this.cmiConvertToWavFile, "cmiConvertToWavFile");
            // 
            // fileSystemElementBindingSource
            // 
            this.fileSystemElementBindingSource.DataSource = typeof(FileSystemElement);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripExplorer
            // 
            this.toolStripExplorer.BackColor = System.Drawing.SystemColors.MenuBar;
            this.toolStripExplorer.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripExplorer.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStripExplorer.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbOpenInExplorer,
            this.toolStripSeparator16,
            this.tsbPlay,
            this.toolStripSeparator17,
            this.tsbRenameFileDir,
            this.tsbDeleteFileDir,
            this.toolStripSeparator22,
            this.tsbConvertFilesToEwa,
            this.tsbConvertFilesToWav});
            resources.ApplyResources(this.toolStripExplorer, "toolStripExplorer");
            this.toolStripExplorer.Margin = new System.Windows.Forms.Padding(2);
            this.toolStripExplorer.Name = "toolStripExplorer";
            // 
            // tsbOpenInExplorer
            // 
            this.tsbOpenInExplorer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbOpenInExplorer.Enabled = false;
            this.tsbOpenInExplorer.Name = "tsbOpenInExplorer";
            this.tsbOpenInExplorer.IconName = "folder-open";
            resources.ApplyResources(this.tsbOpenInExplorer, "tsbOpenInExplorer");
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            resources.ApplyResources(this.toolStripSeparator16, "toolStripSeparator16");
            // 
            // tsbPlay
            // 
            this.tsbPlay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbPlay.Enabled = false;
            this.tsbPlay.Name = "tsbPlay";
            this.tsbPlay.IconName = "speaker";
            resources.ApplyResources(this.tsbPlay, "tsbPlay");
            // 
            // toolStripSeparator17
            // 
            this.toolStripSeparator17.Name = "toolStripSeparator17";
            resources.ApplyResources(this.toolStripSeparator17, "toolStripSeparator17");
            // 
            // tsbRenameFileDir
            // 
            this.tsbRenameFileDir.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRenameFileDir.Enabled = false;
            this.tsbRenameFileDir.Name = "tsbRenameFileDir";
            this.tsbRenameFileDir.IconName = "rename";
            resources.ApplyResources(this.tsbRenameFileDir, "tsbRenameFileDir");
            // 
            // tsbDeleteFileDir
            // 
            this.tsbDeleteFileDir.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDeleteFileDir.Enabled = false;
            this.tsbDeleteFileDir.Name = "tsbDeleteFileDir";
            this.tsbDeleteFileDir.IconName = "delete";
            resources.ApplyResources(this.tsbDeleteFileDir, "tsbDeleteFileDir");
            // 
            // toolStripSeparator22
            // 
            this.toolStripSeparator22.Name = "toolStripSeparator22";
            resources.ApplyResources(this.toolStripSeparator22, "toolStripSeparator22");
            // 
            // tsbConvertFilesToEwa
            // 
            this.tsbConvertFilesToEwa.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbConvertFilesToEwa.Enabled = false;
            this.tsbConvertFilesToEwa.Name = "tsbConvertFilesToEwa";
            this.tsbConvertFilesToEwa.IconName = "file-ewa";
            resources.ApplyResources(this.tsbConvertFilesToEwa, "tsbConvertFilesToEwa");
            // 
            // tsbConvertFilesToWav
            // 
            this.tsbConvertFilesToWav.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbConvertFilesToWav.Enabled = false;
            this.tsbConvertFilesToWav.Name = "tsbConvertFilesToWav";
            this.tsbConvertFilesToWav.IconName = "file-wav";
            resources.ApplyResources(this.tsbConvertFilesToWav, "tsbConvertFilesToWav");
            // 
            // splitSoundsErrors
            // 
            resources.ApplyResources(this.splitSoundsErrors, "splitSoundsErrors");
            this.splitSoundsErrors.Margin = new System.Windows.Forms.Padding(2);
            this.splitSoundsErrors.Name = "splitSoundsErrors";
            this.splitSoundsErrors.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitSoundsErrors.Panel1
            // 
            this.splitSoundsErrors.Panel1.Controls.Add(this.dgvSounds);
            // 
            // splitSoundsErrors.Panel2
            // 
            this.splitSoundsErrors.Panel2.Controls.Add(this.tableLayoutPanel4);
            this.splitSoundsErrors.Size = new System.Drawing.Size(785, 502);
            this.splitSoundsErrors.SplitterDistance = 339;
            this.splitSoundsErrors.SplitterWidth = 6;
            this.splitSoundsErrors.TabIndex = 1;
            // 
            // dgvSounds
            // 
            this.dgvSounds.AllowUserToAddRows = false;
            this.dgvSounds.AllowUserToOrderColumns = true;
            this.dgvSounds.AllowUserToResizeRows = false;
            this.dgvSounds.AutoGenerateColumns = false;
            this.dgvSounds.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSounds.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvSounds.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSounds.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cSoundKey,
            this.cSoundName,
            this.cSoundAdditionalRelativePath,
            this.cSoundFileName,
            this.cSoundDuration,
            this.cSoundText});
            this.dgvSounds.ContextMenuStrip = this.contextMenuSounds;
            this.dgvSounds.DataSource = this.fyzSoundBindingSource;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvSounds.DefaultCellStyle = dataGridViewCellStyle5;
            resources.ApplyResources(this.dgvSounds, "dgvSounds");
            this.dgvSounds.Margin = new System.Windows.Forms.Padding(2);
            this.dgvSounds.Name = "dgvSounds";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvSounds.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvSounds.RowHeadersWidth = 51;
            this.dgvSounds.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvSounds.RowTemplate.Height = 24;
            this.dgvSounds.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSounds.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvSounds_CellBeginEdit);
            this.dgvSounds.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DgvSounds_CellMouseDown);
            this.dgvSounds.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvSounds_DataError);
            this.dgvSounds.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSounds_CellEndEdit);
            this.dgvSounds.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.DgvSounds_CellValidating);
            this.dgvSounds.RowValidating += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvSounds_RowValidating);
            this.dgvSounds.SelectionChanged += new System.EventHandler(this.dgvSounds_SelectionChanged);
            this.dgvSounds.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DgvSounds_MouseDown);
            this.dgvSounds.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DgvSounds_MouseUp);
            // 
            // cSoundKey
            // 
            this.cSoundKey.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.cSoundKey.DataPropertyName = "Key";
            resources.ApplyResources(this.cSoundKey, "cSoundKey");
            this.cSoundKey.MinimumWidth = 6;
            this.cSoundKey.Name = "cSoundKey";
            // 
            // cSoundName
            // 
            this.cSoundName.DataPropertyName = "Name";
            resources.ApplyResources(this.cSoundName, "cSoundName");
            this.cSoundName.Name = "cSoundName";
            // 
            // cSoundAdditionalRelativePath
            // 
            this.cSoundAdditionalRelativePath.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.cSoundAdditionalRelativePath.DataPropertyName = "AdditionalRelativePath";
            resources.ApplyResources(this.cSoundAdditionalRelativePath, "cSoundAdditionalRelativePath");
            this.cSoundAdditionalRelativePath.MinimumWidth = 6;
            this.cSoundAdditionalRelativePath.Name = "cSoundAdditionalRelativePath";
            // 
            // cSoundFileName
            // 
            this.cSoundFileName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.cSoundFileName.DataPropertyName = "FileName";
            resources.ApplyResources(this.cSoundFileName, "cSoundFileName");
            this.cSoundFileName.MinimumWidth = 6;
            this.cSoundFileName.Name = "cSoundFileName";
            // 
            // cSoundDuration
            // 
            this.cSoundDuration.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.cSoundDuration.DataPropertyName = "DurationText";
            resources.ApplyResources(this.cSoundDuration, "cSoundDuration");
            this.cSoundDuration.MinimumWidth = 6;
            this.cSoundDuration.Name = "cSoundDuration";
            this.cSoundDuration.ReadOnly = true;
            // 
            // cSoundText
            // 
            this.cSoundText.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cSoundText.DataPropertyName = "Text";
            resources.ApplyResources(this.cSoundText, "cSoundText");
            this.cSoundText.MinimumWidth = 6;
            this.cSoundText.Name = "cSoundText";
            // 
            // contextMenuSounds
            // 
            this.contextMenuSounds.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuSounds.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmiAddSound,
            this.cmiDeleteSound,
            this.cmiMoveSounds,
            this.toolStripSeparator19,
            this.cmiConvertSoundsToEwa,
            this.cmiConvertSoundsToWav});
            this.contextMenuSounds.Name = "contextMenuSounds";
            resources.ApplyResources(this.contextMenuSounds, "contextMenuSounds");
            // 
            // cmiAddSound
            // 
            this.cmiAddSound.Enabled = false;
            this.cmiAddSound.Name = "cmiAddSound";
            this.cmiAddSound.IconName = "add";
            resources.ApplyResources(this.cmiAddSound, "cmiAddSound");
            // 
            // cmiDeleteSound
            // 
            this.cmiDeleteSound.Enabled = false;
            this.cmiDeleteSound.Name = "cmiDeleteSound";
            this.cmiDeleteSound.IconName = "delete";
            resources.ApplyResources(this.cmiDeleteSound, "cmiDeleteSound");
            // 
            // cmiMoveSounds
            // 
            this.cmiMoveSounds.Enabled = false;
            this.cmiMoveSounds.Name = "cmiMoveSounds";
            this.cmiMoveSounds.IconName = "move";
            resources.ApplyResources(this.cmiMoveSounds, "cmiMoveSounds");
            // 
            // toolStripSeparator19
            // 
            this.toolStripSeparator19.Name = "toolStripSeparator19";
            resources.ApplyResources(this.toolStripSeparator19, "toolStripSeparator19");
            // 
            // cmiConvertSoundsToEwa
            // 
            this.cmiConvertSoundsToEwa.Enabled = false;
            this.cmiConvertSoundsToEwa.Name = "cmiConvertSoundsToEwa";
            this.cmiConvertSoundsToEwa.IconName = "file-ewa";
            resources.ApplyResources(this.cmiConvertSoundsToEwa, "cmiConvertSoundsToEwa");
            // 
            // cmiConvertSoundsToWav
            // 
            this.cmiConvertSoundsToWav.Enabled = false;
            this.cmiConvertSoundsToWav.Name = "cmiConvertSoundsToWav";
            this.cmiConvertSoundsToWav.IconName = "file-wav";
            resources.ApplyResources(this.cmiConvertSoundsToWav, "cmiConvertSoundsToWav");
            // 
            // fyzSoundBindingSource
            // 
            this.fyzSoundBindingSource.DataSource = typeof(FyzSound);
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.BorderColor = System.Drawing.Color.Empty;
            this.tableLayoutPanel4.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.toolStripErrors, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.dgvErrors, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel5, 0, 0);
            resources.ApplyResources(this.tableLayoutPanel4, "tableLayoutPanel4");
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 3;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            // 
            // toolStripErrors
            // 
            this.toolStripErrors.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripErrors.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripErrors.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStripErrors.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbErrors,
            this.toolStripSeparator13,
            this.tsbWarnings,
            this.toolStripSeparator14,
            this.tsbInfos,
            this.tsbResolveProblem,
            this.tsbHighlightProblem});
            resources.ApplyResources(this.toolStripErrors, "toolStripErrors");
            this.toolStripErrors.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            this.toolStripErrors.Name = "toolStripErrors";
            // 
            // tsbErrors
            // 
            this.tsbErrors.Checked = true;
            this.tsbErrors.CheckOnClick = true;
            this.tsbErrors.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsbErrors.Name = "tsbErrors";
            this.tsbErrors.IconName = "status-error";
            resources.ApplyResources(this.tsbErrors, "tsbErrors");
            this.tsbErrors.CheckedChanged += new System.EventHandler(this.tsbErrors_CheckedChanged);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            resources.ApplyResources(this.toolStripSeparator13, "toolStripSeparator13");
            // 
            // tsbWarnings
            // 
            this.tsbWarnings.Checked = true;
            this.tsbWarnings.CheckOnClick = true;
            this.tsbWarnings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsbWarnings.Name = "tsbWarnings";
            this.tsbWarnings.IconName = "status-warning";
            resources.ApplyResources(this.tsbWarnings, "tsbWarnings");
            this.tsbWarnings.CheckedChanged += new System.EventHandler(this.tsbWarnings_CheckedChanged);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            resources.ApplyResources(this.toolStripSeparator14, "toolStripSeparator14");
            // 
            // tsbInfos
            // 
            this.tsbInfos.Checked = true;
            this.tsbInfos.CheckOnClick = true;
            this.tsbInfos.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsbInfos.Name = "tsbInfos";
            this.tsbInfos.IconName = "status-info";
            resources.ApplyResources(this.tsbInfos, "tsbInfos");
            this.tsbInfos.CheckedChanged += new System.EventHandler(this.tsbInfos_CheckedChanged);
            // 
            // tsbResolveProblem
            // 
            this.tsbResolveProblem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsbResolveProblem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbResolveProblem.Enabled = false;
            this.tsbResolveProblem.Name = "tsbResolveProblem";
            this.tsbResolveProblem.IconName = "wrench";
            resources.ApplyResources(this.tsbResolveProblem, "tsbResolveProblem");
            // 
            // tsbHighlightProblem
            // 
            this.tsbHighlightProblem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsbHighlightProblem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbHighlightProblem.Enabled = false;
            this.tsbHighlightProblem.Name = "tsbHighlightProblem";
            this.tsbHighlightProblem.IconName = "search";
            resources.ApplyResources(this.tsbHighlightProblem, "tsbHighlightProblem");
            // 
            // dgvErrors
            // 
            this.dgvErrors.AllowUserToAddRows = false;
            this.dgvErrors.AllowUserToDeleteRows = false;
            this.dgvErrors.AllowUserToResizeRows = false;
            this.dgvErrors.AutoGenerateColumns = false;
            this.dgvErrors.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvErrors.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvErrors.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dgvErrors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvErrors.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cMsgType,
            this.cMsgCode,
            this.cMessage,
            this.cMsgResolve,
            this.cMsgPath});
            this.dgvErrors.ContextMenuStrip = this.contextMenuErrors;
            this.dgvErrors.DataSource = this.rawBankMessageBindingSource;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvErrors.DefaultCellStyle = dataGridViewCellStyle8;
            resources.ApplyResources(this.dgvErrors, "dgvErrors");
            this.dgvErrors.Margin = new System.Windows.Forms.Padding(2);
            this.dgvErrors.MultiSelect = false;
            this.dgvErrors.Name = "dgvErrors";
            this.dgvErrors.ReadOnly = true;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvErrors.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.dgvErrors.RowHeadersVisible = false;
            this.dgvErrors.RowHeadersWidth = 51;
            this.dgvErrors.RowTemplate.Height = 24;
            this.dgvErrors.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvErrors.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvErrors_CellDoubleClick);
            this.dgvErrors.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvErrors_CellFormatting);
            this.dgvErrors.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DgvErrors_CellMouseDown);
            // 
            // cMsgType
            // 
            this.cMsgType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.cMsgType, "cMsgType");
            this.cMsgType.MinimumWidth = 6;
            this.cMsgType.Name = "cMsgType";
            this.cMsgType.ReadOnly = true;
            this.cMsgType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.cMsgType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // cMsgCode
            // 
            this.cMsgCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.cMsgCode.DataPropertyName = "Code";
            resources.ApplyResources(this.cMsgCode, "cMsgCode");
            this.cMsgCode.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.cMsgCode.Name = "cMsgCode";
            this.cMsgCode.ReadOnly = true;
            // 
            // cMessage
            // 
            this.cMessage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.cMessage.DataPropertyName = "Message";
            resources.ApplyResources(this.cMessage, "cMessage");
            this.cMessage.MinimumWidth = 450;
            this.cMessage.Name = "cMessage";
            this.cMessage.ReadOnly = true;
            // 
            // cMsgResolve
            // 
            this.cMsgResolve.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.cMsgResolve.DataPropertyName = "ResolveMessage";
            resources.ApplyResources(this.cMsgResolve, "cMsgResolve");
            this.cMsgResolve.MinimumWidth = 150;
            this.cMsgResolve.Name = "cMsgResolve";
            this.cMsgResolve.ReadOnly = true;
            // 
            // cMsgPath
            // 
            this.cMsgPath.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cMsgPath.DataPropertyName = "Path";
            resources.ApplyResources(this.cMsgPath, "cMsgPath");
            this.cMsgPath.Name = "cMsgPath";
            this.cMsgPath.ReadOnly = true;
            // 
            // contextMenuErrors
            // 
            this.contextMenuErrors.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmiHighlightProblem,
            this.cmiResolveProblem});
            this.contextMenuErrors.Name = "contextMenuErrors";
            resources.ApplyResources(this.contextMenuErrors, "contextMenuErrors");
            // 
            // cmiHighlightProblem
            // 
            this.cmiHighlightProblem.Enabled = false;
            this.cmiHighlightProblem.Name = "cmiHighlightProblem";
            this.cmiHighlightProblem.IconName = "search";
            resources.ApplyResources(this.cmiHighlightProblem, "cmiHighlightProblem");
            // 
            // cmiResolveProblem
            // 
            this.cmiResolveProblem.Enabled = false;
            this.cmiResolveProblem.Name = "cmiResolveProblem";
            this.cmiResolveProblem.IconName = "wrench";
            resources.ApplyResources(this.cmiResolveProblem, "cmiResolveProblem");
            // 
            // rawBankMessageBindingSource
            // 
            this.rawBankMessageBindingSource.DataSource = typeof(RawBankEditor.Entities.IRawBankMessage);
            // 
            // tableLayoutPanel5
            // 
            resources.ApplyResources(this.tableLayoutPanel5, "tableLayoutPanel5");
            this.tableLayoutPanel5.BorderColor = System.Drawing.Color.Empty;
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Controls.Add(this.mmErrors, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            // 
            // mmErrors
            // 
            resources.ApplyResources(this.mmErrors, "mmErrors");
            this.mmErrors.HoverColor = System.Drawing.SystemColors.Highlight;
            this.mmErrors.IconColor = System.Drawing.SystemColors.ControlDark;
            this.mmErrors.IconHoverColor = System.Drawing.Color.White;
            this.mmErrors.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.mmErrors.Name = "mmErrors";
            this.mmErrors.Click += new System.EventHandler(this.mmErrors_Click);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Margin = new System.Windows.Forms.Padding(2);
            this.label3.Name = "label3";
            this.label3.Padding = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BorderColor = System.Drawing.Color.Empty;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.splitContainer1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.statusStripMain, 0, 1);
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            // 
            // statusStripMain
            // 
            this.statusStripMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslStatus,
            this.tspbProgress,
            this.toolStripStatusLabel1,
            this.tssbErrors});
            resources.ApplyResources(this.statusStripMain, "statusStripMain");
            this.statusStripMain.Name = "statusStripMain";
            this.statusStripMain.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
            this.statusStripMain.SizingGrip = false;
            // 
            // tsslStatus
            // 
            this.tsslStatus.Name = "tsslStatus";
            resources.ApplyResources(this.tsslStatus, "tsslStatus");
            // 
            // tspbProgress
            // 
            this.tspbProgress.Margin = new System.Windows.Forms.Padding(5, 4, 1, 4);
            this.tspbProgress.Name = "tspbProgress";
            resources.ApplyResources(this.tspbProgress, "tspbProgress");
            this.tspbProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.tspbProgress.Visible = false;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            resources.ApplyResources(this.toolStripStatusLabel1, "toolStripStatusLabel1");
            this.toolStripStatusLabel1.Spring = true;
            // 
            // tssbErrors
            // 
            this.tssbErrors.Name = "tssbErrors";
            this.tssbErrors.ShowDropDownArrow = false;
            resources.ApplyResources(this.tssbErrors, "tssbErrors");
            this.tssbErrors.Click += new System.EventHandler(this.tssbErrors_Click);
            // 
            // toolStripMain
            // 
            this.toolStripMain.BackColor = System.Drawing.SystemColors.MenuBar;
            this.toolStripMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbOpen,
            this.tsbRecent,
            this.toolStripSeparator25,
            this.tsbSave,
            this.tsbSaveAll,
            this.toolStripSeparator1,
            this.tsbGoBack,
            this.tsbGoForward,
            this.toolStripSeparator9,
            this.tsbUndo,
            this.tsbRedo,
            this.toolStripSeparator10,
            this.toolStripLabel1,
            this.tscboxLanguages,
            this.tsbLangsSettings,
            this.toolStripSeparator7,
            this.tsbAppSettings,
            this.tsbInfoApp,
            this.toolStripSeparator12,
            this.tsbAddSound,
            this.tsbMoveSounds,
            this.tsbDeleteSound,
            this.toolStripSeparator24,
            this.tsbConvertSoundsToEwa,
            this.tsbConvertSoundsToWav,
            this.toolStripSeparator15,
            this.tsbSearch,
            this.tsbWrapTextSoundCol,
            this.tsbRewriteMode});
            resources.ApplyResources(this.toolStripMain, "toolStripMain");
            this.toolStripMain.Name = "toolStripMain";
            this.toolStripMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            // 
            // tsbOpen
            // 
            this.tsbOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbOpen.Name = "tsbOpen";
            this.tsbOpen.IconName = "folder-open";
            resources.ApplyResources(this.tsbOpen, "tsbOpen");
            // 
            // tsbRecent
            // 
            this.tsbRecent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRecent.Name = "tsbRecent";
            this.tsbRecent.IconName = "history";
            resources.ApplyResources(this.tsbRecent, "tsbRecent");
            // 
            // toolStripSeparator25
            // 
            this.toolStripSeparator25.Name = "toolStripSeparator25";
            resources.ApplyResources(this.toolStripSeparator25, "toolStripSeparator25");
            // 
            // tsbSave
            // 
            this.tsbSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSave.Enabled = false;
            this.tsbSave.Name = "tsbSave";
            this.tsbSave.IconName = "save";
            resources.ApplyResources(this.tsbSave, "tsbSave");
            // 
            // tsbSaveAll
            // 
            this.tsbSaveAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSaveAll.Enabled = false;
            this.tsbSaveAll.Name = "tsbSaveAll";
            this.tsbSaveAll.IconName = "save-all";
            resources.ApplyResources(this.tsbSaveAll, "tsbSaveAll");
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // tsbGoBack
            // 
            this.tsbGoBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbGoBack.DropDownButtonWidth = 15;
            this.tsbGoBack.Enabled = false;
            this.tsbGoBack.Name = "tsbGoBack";
            this.tsbGoBack.IconName = "back";
            resources.ApplyResources(this.tsbGoBack, "tsbGoBack");
            // 
            // tsbGoForward
            // 
            this.tsbGoForward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbGoForward.Enabled = false;
            this.tsbGoForward.Name = "tsbGoForward";
            this.tsbGoForward.IconName = "forward";
            resources.ApplyResources(this.tsbGoForward, "tsbGoForward");
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            resources.ApplyResources(this.toolStripSeparator9, "toolStripSeparator9");
            // 
            // tsbUndo
            // 
            this.tsbUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbUndo.Enabled = false;
            this.tsbUndo.Name = "tsbUndo";
            this.tsbUndo.IconName = "undo";
            resources.ApplyResources(this.tsbUndo, "tsbUndo");
            // 
            // tsbRedo
            // 
            this.tsbRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRedo.Enabled = false;
            this.tsbRedo.Name = "tsbRedo";
            this.tsbRedo.IconName = "redo";
            resources.ApplyResources(this.tsbRedo, "tsbRedo");
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            resources.ApplyResources(this.toolStripSeparator10, "toolStripSeparator10");
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            resources.ApplyResources(this.toolStripLabel1, "toolStripLabel1");
            // 
            // tscboxLanguages
            // 
            this.tscboxLanguages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscboxLanguages.DropDownWidth = 150;
            this.tscboxLanguages.Enabled = false;
            this.tscboxLanguages.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.tscboxLanguages.Name = "tscboxLanguages";
            resources.ApplyResources(this.tscboxLanguages, "tscboxLanguages");
            this.tscboxLanguages.SelectedIndexChanged += new System.EventHandler(this.tscboxLanguages_SelectedIndexChanged);
            // 
            // tsbLangsSettings
            // 
            this.tsbLangsSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbLangsSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAddLanguage,
            this.tsmiEditLanguage,
            this.tsmiDeleteLanguage,
            this.toolStripSeparator5,
            this.tsmiConvertLangToEwa,
            this.tsmiConvertLangToWav,
            this.tsmiSepImportExport,
            this.tsmiImportSounds,
            this.tsmiExportIniss2});
            this.tsbLangsSettings.Enabled = false;
            this.tsbLangsSettings.Margin = new System.Windows.Forms.Padding(5, 1, 0, 2);
            this.tsbLangsSettings.Name = "tsbLangsSettings";
            this.tsbLangsSettings.IconName = "local-language";
            resources.ApplyResources(this.tsbLangsSettings, "tsbLangsSettings");
            // 
            // tsmiAddLanguage
            // 
            this.tsmiAddLanguage.Enabled = false;
            this.tsmiAddLanguage.Name = "tsmiAddLanguage";
            this.tsmiAddLanguage.IconName = "add";
            resources.ApplyResources(this.tsmiAddLanguage, "tsmiAddLanguage");
            // 
            // tsmiEditLanguage
            // 
            this.tsmiEditLanguage.Enabled = false;
            this.tsmiEditLanguage.Name = "tsmiEditLanguage";
            this.tsmiEditLanguage.IconName = "edit";
            resources.ApplyResources(this.tsmiEditLanguage, "tsmiEditLanguage");
            // 
            // tsmiDeleteLanguage
            // 
            this.tsmiDeleteLanguage.Enabled = false;
            this.tsmiDeleteLanguage.Name = "tsmiDeleteLanguage";
            this.tsmiDeleteLanguage.IconName = "delete";
            resources.ApplyResources(this.tsmiDeleteLanguage, "tsmiDeleteLanguage");
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            // 
            // tsmiConvertLangToEwa
            // 
            this.tsmiConvertLangToEwa.Enabled = false;
            this.tsmiConvertLangToEwa.Name = "tsmiConvertLangToEwa";
            this.tsmiConvertLangToEwa.IconName = "file-ewa";
            resources.ApplyResources(this.tsmiConvertLangToEwa, "tsmiConvertLangToEwa");
            // 
            // tsmiConvertLangToWav
            // 
            this.tsmiConvertLangToWav.Enabled = false;
            this.tsmiConvertLangToWav.Name = "tsmiConvertLangToWav";
            this.tsmiConvertLangToWav.IconName = "file-wav";
            resources.ApplyResources(this.tsmiConvertLangToWav, "tsmiConvertLangToWav");
            // 
            // tsmiSepImportExport
            // 
            this.tsmiSepImportExport.Name = "tsmiSepImportExport";
            resources.ApplyResources(this.tsmiSepImportExport, "tsmiSepImportExport");
            // 
            // tsmiImportSounds
            // 
            this.tsmiImportSounds.Enabled = false;
            this.tsmiImportSounds.Name = "tsmiImportSounds";
            this.tsmiImportSounds.IconName = "import";
            resources.ApplyResources(this.tsmiImportSounds, "tsmiImportSounds");
            // 
            // tsmiExportIniss2
            // 
            this.tsmiExportIniss2.Enabled = false;
            this.tsmiExportIniss2.Name = "tsmiExportIniss2";
            this.tsmiExportIniss2.IconName = "export";
            resources.ApplyResources(this.tsmiExportIniss2, "tsmiExportIniss2");
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            resources.ApplyResources(this.toolStripSeparator7, "toolStripSeparator7");
            // 
            // tsbAppSettings
            // 
            this.tsbAppSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAppSettings.Name = "tsbAppSettings";
            this.tsbAppSettings.IconName = "settings";
            resources.ApplyResources(this.tsbAppSettings, "tsbAppSettings");
            // 
            // tsbInfoApp
            // 
            this.tsbInfoApp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbInfoApp.Name = "tsbInfoApp";
            this.tsbInfoApp.IconName = "info";
            resources.ApplyResources(this.tsbInfoApp, "tsbInfoApp");
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            resources.ApplyResources(this.toolStripSeparator12, "toolStripSeparator12");
            // 
            // tsbAddSound
            // 
            this.tsbAddSound.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAddSound.Enabled = false;
            this.tsbAddSound.Name = "tsbAddSound";
            this.tsbAddSound.IconName = "add";
            resources.ApplyResources(this.tsbAddSound, "tsbAddSound");
            // 
            // tsbMoveSounds
            // 
            this.tsbMoveSounds.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbMoveSounds.Enabled = false;
            this.tsbMoveSounds.Name = "tsbMoveSounds";
            this.tsbMoveSounds.IconName = "move";
            resources.ApplyResources(this.tsbMoveSounds, "tsbMoveSounds");
            // 
            // tsbDeleteSound
            // 
            this.tsbDeleteSound.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDeleteSound.Enabled = false;
            this.tsbDeleteSound.Name = "tsbDeleteSound";
            this.tsbDeleteSound.IconName = "delete";
            resources.ApplyResources(this.tsbDeleteSound, "tsbDeleteSound");
            // 
            // toolStripSeparator24
            // 
            this.toolStripSeparator24.Name = "toolStripSeparator24";
            resources.ApplyResources(this.toolStripSeparator24, "toolStripSeparator24");
            // 
            // tsbConvertSoundsToEwa
            // 
            this.tsbConvertSoundsToEwa.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbConvertSoundsToEwa.Enabled = false;
            this.tsbConvertSoundsToEwa.Name = "tsbConvertSoundsToEwa";
            this.tsbConvertSoundsToEwa.IconName = "file-ewa";
            resources.ApplyResources(this.tsbConvertSoundsToEwa, "tsbConvertSoundsToEwa");
            // 
            // tsbConvertSoundsToWav
            // 
            this.tsbConvertSoundsToWav.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbConvertSoundsToWav.Enabled = false;
            this.tsbConvertSoundsToWav.Name = "tsbConvertSoundsToWav";
            this.tsbConvertSoundsToWav.IconName = "file-wav";
            resources.ApplyResources(this.tsbConvertSoundsToWav, "tsbConvertSoundsToWav");
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            resources.ApplyResources(this.toolStripSeparator15, "toolStripSeparator15");
            // 
            // tsbSearch
            // 
            this.tsbSearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSearch.Enabled = false;
            this.tsbSearch.Name = "tsbSearch";
            this.tsbSearch.IconName = "search";
            resources.ApplyResources(this.tsbSearch, "tsbSearch");
            // 
            // tsbWrapTextSoundCol
            // 
            this.tsbWrapTextSoundCol.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsbWrapTextSoundCol.CheckOnClick = true;
            this.tsbWrapTextSoundCol.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbWrapTextSoundCol.Margin = new System.Windows.Forms.Padding(0, 1, 4, 2);
            this.tsbWrapTextSoundCol.Name = "tsbWrapTextSoundCol";
            this.tsbWrapTextSoundCol.IconName = "text-wrap";
            resources.ApplyResources(this.tsbWrapTextSoundCol, "tsbWrapTextSoundCol");
            this.tsbWrapTextSoundCol.CheckedChanged += new System.EventHandler(this.WrapSoundTextChanged);
            // 
            // tsbRewriteMode
            // 
            this.tsbRewriteMode.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsbRewriteMode.CheckOnClick = true;
            this.tsbRewriteMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRewriteMode.Enabled = false;
            this.tsbRewriteMode.Name = "tsbRewriteMode";
            this.tsbRewriteMode.IconName = "link";
            resources.ApplyResources(this.tsbRewriteMode, "tsbRewriteMode");
            this.tsbRewriteMode.CheckedChanged += new System.EventHandler(this.RewriteModeChanged);
            // 
            // undoActionChooser
            // 
            this.undoActionChooser.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.undoActionChooser.Name = "undoActionChooser";
            resources.ApplyResources(this.undoActionChooser, "undoActionChooser");
            // 
            // 
            // fileSystemWatcher
            // 
            this.fileSystemWatcher.EnableRaisingEvents = true;
            this.fileSystemWatcher.IncludeSubdirectories = true;
            this.fileSystemWatcher.SynchronizingObject = this;
            this.fileSystemWatcher.Changed += new System.IO.FileSystemEventHandler(this.fileSystemWatcher_Changed);
            this.fileSystemWatcher.Created += new System.IO.FileSystemEventHandler(this.fileSystemWatcher_Created);
            this.fileSystemWatcher.Deleted += new System.IO.FileSystemEventHandler(this.fileSystemWatcher_Deleted);
            this.fileSystemWatcher.Renamed += new System.IO.RenamedEventHandler(this.fileSystemWatcher_Renamed);
            // 
            // moveManager
            // 
            this.moveManager.ManagerEnabled = true;
            this.moveManager.CommandAdded += new System.EventHandler<ExControls.Providers.BackwardForwardAddedCommandEventArgs>(this.MoveManager_CommandAdded);
            // 
            // changeManager
            // 
            this.changeManager.ManagerEnabled = true;
            this.changeManager.UndoRedoStateChanged += new System.EventHandler<ExControls.Providers.UndoRedoStateEventArgs>(this.ChangeManager_UndoRedoStateChanged);
            // 
            // timerToCheck
            // 
            this.timerToCheck.Interval = 1000;
            this.timerToCheck.Tick += new System.EventHandler(this.TimerToCheck_Tick);
            // 
            // FMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.toolStripMain);
            this.Controls.Add(this.titleBar);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStripMain;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FMain";
            this.UseCompositedPainting = false;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FMain_FormClosing);
            this.titleBar.ResumeLayout(false);
            this.titleBar.PerformLayout();
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.toolStripGroups.ResumeLayout(false);
            this.toolStripGroups.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGroups)).EndInit();
            this.contextMenuGroups.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fyzGroupBindingSource)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvExplorer)).EndInit();
            this.contextMenuExplorer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemElementBindingSource)).EndInit();
            this.toolStripExplorer.ResumeLayout(false);
            this.toolStripExplorer.PerformLayout();
            this.splitSoundsErrors.Panel1.ResumeLayout(false);
            this.splitSoundsErrors.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitSoundsErrors)).EndInit();
            this.splitSoundsErrors.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSounds)).EndInit();
            this.contextMenuSounds.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fyzSoundBindingSource)).EndInit();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.toolStripErrors.ResumeLayout(false);
            this.toolStripErrors.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvErrors)).EndInit();
            this.contextMenuErrors.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rawBankMessageBindingSource)).EndInit();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.statusStripMain.ResumeLayout(false);
            this.statusStripMain.PerformLayout();
            this.toolStripMain.ResumeLayout(false);
            this.toolStripMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ExControls.ExTitleBar titleBar;
        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem tsmiFile;
        private System.Windows.Forms.ToolStripMenuItem tsmiEdit;
        private System.Windows.Forms.ToolStripMenuItem tsmiShow;
        private System.Windows.Forms.ToolStripMenuItem tsmiHelp;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private ExTableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStrip toolStripGroups;
        private ExTableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.StatusStrip statusStripMain;
        private System.Windows.Forms.ToolStrip toolStripMain;
        private ToolsCore.Icons.IconToolStripButton tsbOpen;
        private ExTableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label2;
        private ExControls.ExToolStripComboBox tscboxLanguages;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimOpen;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimRecent;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimSave;
        private System.Windows.Forms.ToolStripMenuItem tsmiTools;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private ToolsCore.Icons.IconToolStripButton tsbEditGroup;
        private ToolsCore.Icons.IconToolStripButton tsbDeleteGroup;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimInfoApp;
        private ToolsCore.Icons.IconToolStripButton tsbSave;
        private ToolsCore.Icons.IconToolStripButton tsbInfoApp;
        private System.Windows.Forms.ToolStripStatusLabel tsslStatus;
        private System.Windows.Forms.ToolStripProgressBar tspbProgress;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimLangsSettings;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimAppSettings;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private ToolsCore.Icons.IconToolStripButton tsbAppSettings;
        private System.Windows.Forms.ContextMenuStrip contextMenuGroups;
        private ToolsCore.Icons.IconToolStripMenuItem cmiAddGroup;
        private ToolsCore.Icons.IconToolStripMenuItem cmiEditGroup;
        private ToolsCore.Icons.IconToolStripMenuItem cmiDeleteGroup;
        private System.Windows.Forms.ContextMenuStrip contextMenuSounds;
        private System.Windows.Forms.ToolStrip toolStripExplorer;
        private ToolsCore.Icons.IconToolStripButton tsbGoForward;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private ToolsCore.Icons.IconToolStripButton tsbRedo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimUndo;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimRedo;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimGoBack;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimGoForward;
        private ToolsCore.Icons.IconToolStripDropDownButton tsbRecent;
        private System.Windows.Forms.ToolStripMenuItem tsmimUpdates;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimAddSound;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimDeleteSound;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private ToolsCore.Icons.IconToolStripButton tsbAddSound;
        private ToolsCore.Icons.IconToolStripButton tsbDeleteSound;
        private ToolsCore.Icons.IconToolStripButton tsbAddGroup;
        private System.Windows.Forms.BindingSource fyzSoundBindingSource;
        private ToolsCore.Icons.IconToolStripButton tsbConvertSoundsToEwa;
        private ToolsCore.Icons.IconToolStripMenuItem cmiAddSound;
        private ToolsCore.Icons.IconToolStripMenuItem cmiDeleteSound;
        private System.IO.FileSystemWatcher fileSystemWatcher;
        private System.Windows.Forms.SplitContainer splitSoundsErrors;
        private ExTableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStrip toolStripErrors;
        private ToolsCore.Icons.IconToolStripButton tsbErrors;
        private ToolsCore.Icons.IconToolStripButton tsbWarnings;
        private ToolsCore.Icons.IconToolStripButton tsbInfos;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
        internal System.Windows.Forms.DataGridView dgvErrors;
        private ToolsCore.Icons.IconToolStripButton tsbOpenInExplorer;
        private ToolsCore.Icons.IconToolStripButton tsbPlay;
        private ExTableLayoutPanel tableLayoutPanel5;
        private Controls.MinButton mmErrors;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator16;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator17;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimMoveSounds;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
        private ToolsCore.Icons.IconToolStripButton tsbSearch;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator18;
        private ToolsCore.Icons.IconToolStripMenuItem tsmimSearch;
        internal System.Windows.Forms.DataGridView dgvExplorer;
        private ToolsCore.Icons.IconToolStripButton tsbMoveSounds;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.BindingSource fileSystemElementBindingSource;
        private ToolsCore.Icons.IconToolStripButton tsbRenameFileDir;
        private ToolsCore.Icons.IconToolStripButton tsbDeleteFileDir;
        private System.Windows.Forms.BindingSource rawBankMessageBindingSource;
        private ToolsCore.Icons.IconToolStripDropDownButton tssbErrors;
        private ToolStripSeparator toolStripSeparator19;
        private IconToolStripMenuItem cmiConvertSoundsToWav;
        private IconToolStripMenuItem cmiConvertSoundsToEwa;
        internal DataGridView dgvGroups;
        internal DataGridView dgvSounds;
        private ContextMenuStrip contextMenuExplorer;
        private IconToolStripMenuItem cmiOpenInExplorer;
        private IconToolStripMenuItem cmiPlay;
        private ToolStripSeparator toolStripSeparator20;
        private IconToolStripMenuItem cmiRenameFileDir;
        private IconToolStripMenuItem cmiDeleteFileDir;
        private ContextMenuStrip contextMenuErrors;
        private IconToolStripMenuItem cmiHighlightProblem;
        private IconToolStripMenuItem cmiResolveProblem;
        private IconToolStripButton tsbResolveProblem;
        private IconToolStripButton tsbHighlightProblem;
        private BindingSource fyzGroupBindingSource;
        private ToolStripSeparator toolStripSeparator4;
        private IconToolStripMenuItem cmiConvertGroupToWav;
        private IconToolStripMenuItem cmiConvertGroupToEwa;
        private ToolStripSeparator toolStripSeparator21;
        private IconToolStripButton tsbConvertGroupToEwa;
        private IconToolStripButton tsbConvertGroupToWav;
        private IconToolStripButton tsbConvertSoundsToWav;
        private ToolStripSeparator toolStripSeparator22;
        private IconToolStripButton tsbConvertFilesToEwa;
        private IconToolStripButton tsbConvertFilesToWav;
        private ToolStripSeparator toolStripSeparator23;
        private IconToolStripMenuItem cmiConvertToEwaFile;
        private IconToolStripMenuItem cmiConvertToWavFile;
        private ExControls.Providers.BackwardForwardProvider moveManager;
        internal ExControls.Providers.UndoRedoManager changeManager;
        private IconToolStripMenuItem tsmimSaveAll;
        private IconToolStripButton tsbSaveAll;
        private ToolStripSeparator toolStripSeparator25;
        private System.Windows.Forms.Timer timerToCheck;
        private ToolStripSeparator toolStripSeparator26;
        private IconToolStripMenuItem tsmimRewriteMode;
        private IconToolStripButton tsbRewriteMode;
        private IconToolStripMenuItem tsmimAddLanguage;
        private IconToolStripMenuItem tsmimEditLanguage;
        private IconToolStripMenuItem tsmimDeleteLanguage;
        private IconToolStripDropDownButton tsbLangsSettings;
        private IconToolStripMenuItem tsmiAddLanguage;
        private IconToolStripMenuItem tsmiEditLanguage;
        private IconToolStripMenuItem tsmiDeleteLanguage;
        private ToolStripSeparator toolStripSeparator3;
        private IconToolStripMenuItem tsmimShowErrors;
        private IconToolStripMenuItem tsmimConvertLangToEwa;
        private IconToolStripMenuItem tsmimConvertLangToWav;
        private ToolStripSeparator tsmimSepImportExport;
        private IconToolStripMenuItem tsmimImportSounds;
        private IconToolStripMenuItem tsmimExportIniss2;
        private ToolStripSeparator toolStripSeparator5;
        private IconToolStripMenuItem tsmiConvertLangToEwa;
        private IconToolStripMenuItem tsmiConvertLangToWav;
        private ToolStripSeparator tsmiSepImportExport;
        private IconToolStripMenuItem tsmiImportSounds;
        private IconToolStripMenuItem tsmiExportIniss2;
        private IconToolStripMenuItem tsmimConvertSoundsToEwa;
        private IconToolStripMenuItem tsmimConvertSoundsToWav;
        private IconToolStripMenuItem cmiMoveSounds;
        private DataGridViewImageColumn cFileType;
        private DataGridViewTextBoxColumn cFileName;
        private DataGridViewTextBoxColumn cFileDuration;
        private IconToolStripButton tsbWrapTextSoundCol;
        private IconToolStripMenuItem tsmimWrapTextSoundCol;
        private ToolStripSeparator toolStripSeparator6;
        private IconToolStripSplitButton tsbGoBack;
        private ToolStripUndoRedoActionChooser undoActionChooser;
        private IconToolStripButton tsbUndo;
        private ToolStripSeparator toolStripSeparator24;
        private DataGridViewTextBoxColumn cSoundKey;
        private DataGridViewTextBoxColumn cSoundName;
        private DataGridViewTextBoxColumn cSoundAdditionalRelativePath;
        private DataGridViewTextBoxColumn cSoundFileName;
        private DataGridViewTextBoxColumn cSoundDuration;
        private DataGridViewTextBoxColumn cSoundText;
        private DataGridViewImageColumn cMsgType;
        private DataGridViewLinkColumn cMsgCode;
        private DataGridViewTextBoxColumn cMessage;
        private DataGridViewTextBoxColumn cMsgResolve;
        private DataGridViewTextBoxColumn cMsgPath;
        private DataGridViewTextBoxColumn cGroupName;
        private DataGridViewTextBoxColumn cGroupRelativePath;
        private DataGridViewTextBoxColumn cGroupCountSounds;
    }
}

