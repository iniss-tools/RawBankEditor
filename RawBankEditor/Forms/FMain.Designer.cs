
using ExControls;
using RawBankEditor.Entities;
using RawBankEditor.Tools;
using ToolsCore.Iniss.Entities;

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
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.tsmiFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmimOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmimRecent = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmimSave = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmimSaveAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmimUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmimRedo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmimAddSound = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmimDeleteSound = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmimMoveSounds = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmimConvertSoundsToEwa = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmimConvertSoundsToWav = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator26 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmimRewriteMode = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmimWrapTextSoundCol = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiShow = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmimGoBack = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmimGoForward = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmimSearch = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmimShowErrors = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTools = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmimLangsSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmimAddLanguage = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmimEditLanguage = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmimDeleteLanguage = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmimConvertLangToEwa = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmimConvertLangToWav = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmimAppSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmimInfoApp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmimUpdates = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tableLayoutPanel2 = new ExControls.ExTableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.toolStripGroups = new System.Windows.Forms.ToolStrip();
            this.tsbAddGroup = new System.Windows.Forms.ToolStripButton();
            this.tsbEditGroup = new System.Windows.Forms.ToolStripButton();
            this.tsbDeleteGroup = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator21 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbConvertGroupToEwa = new System.Windows.Forms.ToolStripButton();
            this.tsbConvertGroupToWav = new System.Windows.Forms.ToolStripButton();
            this.dgvGroups = new System.Windows.Forms.DataGridView();
            this.cGroupName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cGroupRelativePath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cGroupCountSounds = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuGroups = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmiAddGroup = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiEditGroup = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiDeleteGroup = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.cmiConvertGroupToEwa = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiConvertGroupToWav = new System.Windows.Forms.ToolStripMenuItem();
            this.fyzGroupBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tableLayoutPanel3 = new ExControls.ExTableLayoutPanel();
            this.dgvExplorer = new System.Windows.Forms.DataGridView();
            this.cFileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cFileType = new System.Windows.Forms.DataGridViewImageColumn();
            this.cFileDuration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuExplorer = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmiOpenInExplorer = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiPlay = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator20 = new System.Windows.Forms.ToolStripSeparator();
            this.cmiRenameFileDir = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiDeleteFileDir = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator23 = new System.Windows.Forms.ToolStripSeparator();
            this.cmiConvertToEwaFile = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiConvertToWavFile = new System.Windows.Forms.ToolStripMenuItem();
            this.fileSystemElementBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.toolStripExplorer = new System.Windows.Forms.ToolStrip();
            this.tsbOpenInExplorer = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbPlay = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbRenameFileDir = new System.Windows.Forms.ToolStripButton();
            this.tsbDeleteFileDir = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator22 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbConvertFilesToEwa = new System.Windows.Forms.ToolStripButton();
            this.tsbConvertFilesToWav = new System.Windows.Forms.ToolStripButton();
            this.splitSoundsErrors = new System.Windows.Forms.SplitContainer();
            this.dgvSounds = new System.Windows.Forms.DataGridView();
            this.cSoundKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cSoundName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cSoundAdditionalRelativePath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cSoundFileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cSoundDuration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cSoundText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuSounds = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmiAddSound = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiDeleteSound = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiMoveSounds = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator19 = new System.Windows.Forms.ToolStripSeparator();
            this.cmiConvertSoundsToEwa = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiConvertSoundsToWav = new System.Windows.Forms.ToolStripMenuItem();
            this.fyzSoundBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tableLayoutPanel4 = new ExControls.ExTableLayoutPanel();
            this.toolStripErrors = new System.Windows.Forms.ToolStrip();
            this.tsbErrors = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator13 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbWarnings = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbInfos = new System.Windows.Forms.ToolStripButton();
            this.tsbResolveProblem = new System.Windows.Forms.ToolStripButton();
            this.tsbHighlightProblem = new System.Windows.Forms.ToolStripButton();
            this.dgvErrors = new System.Windows.Forms.DataGridView();
            this.cMsgType = new System.Windows.Forms.DataGridViewImageColumn();
            this.cMsgCode = new System.Windows.Forms.DataGridViewLinkColumn();
            this.cMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cMsgResolve = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cMsgPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuErrors = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmiHighlightProblem = new System.Windows.Forms.ToolStripMenuItem();
            this.cmiResolveProblem = new System.Windows.Forms.ToolStripMenuItem();
            this.rawBankMessageBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tableLayoutPanel5 = new ExControls.ExTableLayoutPanel();
            this.mmErrors = new RawBankEditor.Controls.MinButton();
            this.label3 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new ExControls.ExTableLayoutPanel();
            this.statusStripMain = new System.Windows.Forms.StatusStrip();
            this.tsslStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tspbProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssbErrors = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.tsbOpen = new System.Windows.Forms.ToolStripButton();
            this.tsbRecent = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripSeparator25 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbSave = new System.Windows.Forms.ToolStripButton();
            this.tsbSaveAll = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbGoBack = new System.Windows.Forms.ToolStripSplitButton();
            this.tsbGoForward = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbUndo = new System.Windows.Forms.ToolStripButton();
            this.tsbRedo = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.tscboxLanguages = new ExControls.ExToolStripComboBox();
            this.tsbLangsSettings = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsmiAddLanguage = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEditLanguage = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDeleteLanguage = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiConvertLangToEwa = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiConvertLangToWav = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbAppSettings = new System.Windows.Forms.ToolStripButton();
            this.tsbInfoApp = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbAddSound = new System.Windows.Forms.ToolStripButton();
            this.tsbMoveSounds = new System.Windows.Forms.ToolStripButton();
            this.tsbDeleteSound = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator24 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbConvertSoundsToEwa = new System.Windows.Forms.ToolStripButton();
            this.tsbConvertSoundsToWav = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbSearch = new System.Windows.Forms.ToolStripButton();
            this.tsbWrapTextSoundCol = new System.Windows.Forms.ToolStripButton();
            this.tsbRewriteMode = new System.Windows.Forms.ToolStripButton();
            this.undoActionChooser = new ExControls.ToolStripUndoRedoActionChooser();
            this.fileSystemWatcher = new System.IO.FileSystemWatcher();
            this.moveManager = new ExControls.Providers.BackwardForwardProvider();
            this.changeManager = new ExControls.Providers.UndoRedoManager();
            this.timerToCheck = new System.Windows.Forms.Timer(this.components);
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
            this.tsmimOpen.Image = global::ToolsCore.GlobalResources.open;
            this.tsmimOpen.Name = "tsmimOpen";
            resources.ApplyResources(this.tsmimOpen, "tsmimOpen");
            // 
            // tsmimRecent
            // 
            this.tsmimRecent.Image = global::ToolsCore.GlobalResources.recent_gvds;
            this.tsmimRecent.Name = "tsmimRecent";
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
            this.tsmimSave.Image = global::ToolsCore.GlobalResources.save;
            this.tsmimSave.Name = "tsmimSave";
            resources.ApplyResources(this.tsmimSave, "tsmimSave");
            // 
            // tsmimSaveAll
            // 
            this.tsmimSaveAll.Enabled = false;
            this.tsmimSaveAll.Image = global::ToolsCore.GlobalResources.save_all;
            this.tsmimSaveAll.Name = "tsmimSaveAll";
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
            this.tsmimUndo.Image = global::ToolsCore.GlobalResources.undo;
            this.tsmimUndo.Name = "tsmimUndo";
            resources.ApplyResources(this.tsmimUndo, "tsmimUndo");
            // 
            // tsmimRedo
            // 
            this.tsmimRedo.Enabled = false;
            this.tsmimRedo.Image = global::ToolsCore.GlobalResources.redo;
            this.tsmimRedo.Name = "tsmimRedo";
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
            this.tsmimAddSound.Image = global::ToolsCore.GlobalResources.add;
            this.tsmimAddSound.Name = "tsmimAddSound";
            resources.ApplyResources(this.tsmimAddSound, "tsmimAddSound");
            // 
            // tsmimDeleteSound
            // 
            this.tsmimDeleteSound.Enabled = false;
            this.tsmimDeleteSound.Image = global::ToolsCore.GlobalResources.delete;
            this.tsmimDeleteSound.Name = "tsmimDeleteSound";
            resources.ApplyResources(this.tsmimDeleteSound, "tsmimDeleteSound");
            // 
            // tsmimMoveSounds
            // 
            this.tsmimMoveSounds.Enabled = false;
            this.tsmimMoveSounds.Image = global::ToolsCore.GlobalResources.move;
            this.tsmimMoveSounds.Name = "tsmimMoveSounds";
            resources.ApplyResources(this.tsmimMoveSounds, "tsmimMoveSounds");
            // 
            // tsmimConvertSoundsToEwa
            // 
            this.tsmimConvertSoundsToEwa.Enabled = false;
            this.tsmimConvertSoundsToEwa.Image = global::RawBankEditor.Properties.Resources.ewa;
            this.tsmimConvertSoundsToEwa.Name = "tsmimConvertSoundsToEwa";
            resources.ApplyResources(this.tsmimConvertSoundsToEwa, "tsmimConvertSoundsToEwa");
            // 
            // tsmimConvertSoundsToWav
            // 
            this.tsmimConvertSoundsToWav.Enabled = false;
            this.tsmimConvertSoundsToWav.Image = global::RawBankEditor.Properties.Resources.wav;
            this.tsmimConvertSoundsToWav.Name = "tsmimConvertSoundsToWav";
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
            this.tsmimRewriteMode.Image = global::ToolsCore.GlobalResources.convert;
            this.tsmimRewriteMode.Name = "tsmimRewriteMode";
            resources.ApplyResources(this.tsmimRewriteMode, "tsmimRewriteMode");
            this.tsmimRewriteMode.CheckedChanged += new System.EventHandler(this.RewriteModeChanged);
            // 
            // tsmimWrapTextSoundCol
            // 
            this.tsmimWrapTextSoundCol.CheckOnClick = true;
            this.tsmimWrapTextSoundCol.Image = global::ToolsCore.GlobalResources.textbox;
            this.tsmimWrapTextSoundCol.Name = "tsmimWrapTextSoundCol";
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
            this.tsmimGoBack.Image = global::ToolsCore.GlobalResources.back;
            this.tsmimGoBack.Name = "tsmimGoBack";
            resources.ApplyResources(this.tsmimGoBack, "tsmimGoBack");
            // 
            // tsmimGoForward
            // 
            this.tsmimGoForward.Enabled = false;
            this.tsmimGoForward.Image = global::ToolsCore.GlobalResources.forward;
            this.tsmimGoForward.Name = "tsmimGoForward";
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
            this.tsmimSearch.Image = global::ToolsCore.GlobalResources.search;
            this.tsmimSearch.Name = "tsmimSearch";
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
            this.tsmimConvertLangToWav});
            this.tsmimLangsSettings.Enabled = false;
            this.tsmimLangsSettings.Image = global::ToolsCore.GlobalResources.global_settings;
            this.tsmimLangsSettings.Name = "tsmimLangsSettings";
            resources.ApplyResources(this.tsmimLangsSettings, "tsmimLangsSettings");
            // 
            // tsmimAddLanguage
            // 
            this.tsmimAddLanguage.Enabled = false;
            this.tsmimAddLanguage.Image = global::ToolsCore.GlobalResources.add;
            this.tsmimAddLanguage.Name = "tsmimAddLanguage";
            resources.ApplyResources(this.tsmimAddLanguage, "tsmimAddLanguage");
            // 
            // tsmimEditLanguage
            // 
            this.tsmimEditLanguage.Enabled = false;
            this.tsmimEditLanguage.Image = global::ToolsCore.GlobalResources.edit;
            this.tsmimEditLanguage.Name = "tsmimEditLanguage";
            resources.ApplyResources(this.tsmimEditLanguage, "tsmimEditLanguage");
            // 
            // tsmimDeleteLanguage
            // 
            this.tsmimDeleteLanguage.Enabled = false;
            this.tsmimDeleteLanguage.Image = global::ToolsCore.GlobalResources.delete;
            this.tsmimDeleteLanguage.Name = "tsmimDeleteLanguage";
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
            this.tsmimConvertLangToEwa.Image = global::RawBankEditor.Properties.Resources.ewa;
            this.tsmimConvertLangToEwa.Name = "tsmimConvertLangToEwa";
            resources.ApplyResources(this.tsmimConvertLangToEwa, "tsmimConvertLangToEwa");
            // 
            // tsmimConvertLangToWav
            // 
            this.tsmimConvertLangToWav.Enabled = false;
            this.tsmimConvertLangToWav.Image = global::RawBankEditor.Properties.Resources.wav;
            this.tsmimConvertLangToWav.Name = "tsmimConvertLangToWav";
            resources.ApplyResources(this.tsmimConvertLangToWav, "tsmimConvertLangToWav");
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            resources.ApplyResources(this.toolStripSeparator8, "toolStripSeparator8");
            // 
            // tsmimAppSettings
            // 
            this.tsmimAppSettings.Image = global::ToolsCore.GlobalResources.app_settings;
            this.tsmimAppSettings.Name = "tsmimAppSettings";
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
            this.tsmimInfoApp.Image = global::ToolsCore.GlobalResources.info_app;
            this.tsmimInfoApp.Name = "tsmimInfoApp";
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
            this.tsbAddGroup.Image = global::ToolsCore.GlobalResources.add;
            this.tsbAddGroup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAddGroup.Name = "tsbAddGroup";
            resources.ApplyResources(this.tsbAddGroup, "tsbAddGroup");
            // 
            // tsbEditGroup
            // 
            this.tsbEditGroup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbEditGroup.Enabled = false;
            this.tsbEditGroup.Image = global::ToolsCore.GlobalResources.edit;
            this.tsbEditGroup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbEditGroup.Name = "tsbEditGroup";
            resources.ApplyResources(this.tsbEditGroup, "tsbEditGroup");
            // 
            // tsbDeleteGroup
            // 
            this.tsbDeleteGroup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDeleteGroup.Enabled = false;
            this.tsbDeleteGroup.Image = global::ToolsCore.GlobalResources.delete;
            this.tsbDeleteGroup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDeleteGroup.Name = "tsbDeleteGroup";
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
            this.tsbConvertGroupToEwa.Image = global::RawBankEditor.Properties.Resources.ewa;
            this.tsbConvertGroupToEwa.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbConvertGroupToEwa.Name = "tsbConvertGroupToEwa";
            resources.ApplyResources(this.tsbConvertGroupToEwa, "tsbConvertGroupToEwa");
            // 
            // tsbConvertGroupToWav
            // 
            this.tsbConvertGroupToWav.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbConvertGroupToWav.Enabled = false;
            this.tsbConvertGroupToWav.Image = global::RawBankEditor.Properties.Resources.wav;
            this.tsbConvertGroupToWav.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbConvertGroupToWav.Name = "tsbConvertGroupToWav";
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
            this.cmiAddGroup.Image = global::ToolsCore.GlobalResources.add;
            this.cmiAddGroup.Name = "cmiAddGroup";
            resources.ApplyResources(this.cmiAddGroup, "cmiAddGroup");
            // 
            // cmiEditGroup
            // 
            this.cmiEditGroup.Enabled = false;
            this.cmiEditGroup.Image = global::ToolsCore.GlobalResources.edit;
            this.cmiEditGroup.Name = "cmiEditGroup";
            resources.ApplyResources(this.cmiEditGroup, "cmiEditGroup");
            // 
            // cmiDeleteGroup
            // 
            this.cmiDeleteGroup.Enabled = false;
            this.cmiDeleteGroup.Image = global::ToolsCore.GlobalResources.delete;
            this.cmiDeleteGroup.Name = "cmiDeleteGroup";
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
            this.cmiConvertGroupToEwa.Image = global::RawBankEditor.Properties.Resources.ewa;
            this.cmiConvertGroupToEwa.Name = "cmiConvertGroupToEwa";
            resources.ApplyResources(this.cmiConvertGroupToEwa, "cmiConvertGroupToEwa");
            // 
            // cmiConvertGroupToWav
            // 
            this.cmiConvertGroupToWav.Enabled = false;
            this.cmiConvertGroupToWav.Image = global::RawBankEditor.Properties.Resources.wav;
            this.cmiConvertGroupToWav.Name = "cmiConvertGroupToWav";
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
            this.cFileType.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
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
            this.cmiOpenInExplorer.Image = global::ToolsCore.GlobalResources.folder;
            this.cmiOpenInExplorer.Name = "cmiOpenInExplorer";
            resources.ApplyResources(this.cmiOpenInExplorer, "cmiOpenInExplorer");
            // 
            // cmiPlay
            // 
            this.cmiPlay.Enabled = false;
            this.cmiPlay.Image = global::ToolsCore.GlobalResources.sound;
            this.cmiPlay.Name = "cmiPlay";
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
            this.cmiRenameFileDir.Image = global::ToolsCore.GlobalResources.rename;
            this.cmiRenameFileDir.Name = "cmiRenameFileDir";
            this.cmiRenameFileDir.ShortcutKeyDisplayString = "F2";
            resources.ApplyResources(this.cmiRenameFileDir, "cmiRenameFileDir");
            // 
            // cmiDeleteFileDir
            // 
            this.cmiDeleteFileDir.Enabled = false;
            this.cmiDeleteFileDir.Image = global::ToolsCore.GlobalResources.delete;
            this.cmiDeleteFileDir.Name = "cmiDeleteFileDir";
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
            this.cmiConvertToEwaFile.Image = global::RawBankEditor.Properties.Resources.ewa;
            this.cmiConvertToEwaFile.Name = "cmiConvertToEwaFile";
            resources.ApplyResources(this.cmiConvertToEwaFile, "cmiConvertToEwaFile");
            // 
            // cmiConvertToWavFile
            // 
            this.cmiConvertToWavFile.Enabled = false;
            this.cmiConvertToWavFile.Image = global::RawBankEditor.Properties.Resources.wav;
            this.cmiConvertToWavFile.Name = "cmiConvertToWavFile";
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
            this.tsbOpenInExplorer.Image = global::ToolsCore.GlobalResources.folder;
            this.tsbOpenInExplorer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbOpenInExplorer.Name = "tsbOpenInExplorer";
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
            this.tsbPlay.Image = global::ToolsCore.GlobalResources.sound;
            this.tsbPlay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbPlay.Name = "tsbPlay";
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
            this.tsbRenameFileDir.Image = global::ToolsCore.GlobalResources.rename;
            this.tsbRenameFileDir.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRenameFileDir.Name = "tsbRenameFileDir";
            resources.ApplyResources(this.tsbRenameFileDir, "tsbRenameFileDir");
            // 
            // tsbDeleteFileDir
            // 
            this.tsbDeleteFileDir.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDeleteFileDir.Enabled = false;
            this.tsbDeleteFileDir.Image = global::ToolsCore.GlobalResources.delete;
            this.tsbDeleteFileDir.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDeleteFileDir.Name = "tsbDeleteFileDir";
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
            this.tsbConvertFilesToEwa.Image = global::RawBankEditor.Properties.Resources.ewa;
            this.tsbConvertFilesToEwa.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbConvertFilesToEwa.Name = "tsbConvertFilesToEwa";
            resources.ApplyResources(this.tsbConvertFilesToEwa, "tsbConvertFilesToEwa");
            // 
            // tsbConvertFilesToWav
            // 
            this.tsbConvertFilesToWav.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbConvertFilesToWav.Enabled = false;
            this.tsbConvertFilesToWav.Image = global::RawBankEditor.Properties.Resources.wav;
            this.tsbConvertFilesToWav.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbConvertFilesToWav.Name = "tsbConvertFilesToWav";
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
            this.cmiAddSound.Image = global::ToolsCore.GlobalResources.add;
            this.cmiAddSound.Name = "cmiAddSound";
            resources.ApplyResources(this.cmiAddSound, "cmiAddSound");
            // 
            // cmiDeleteSound
            // 
            this.cmiDeleteSound.Enabled = false;
            this.cmiDeleteSound.Image = global::ToolsCore.GlobalResources.delete;
            this.cmiDeleteSound.Name = "cmiDeleteSound";
            resources.ApplyResources(this.cmiDeleteSound, "cmiDeleteSound");
            // 
            // cmiMoveSounds
            // 
            this.cmiMoveSounds.Enabled = false;
            this.cmiMoveSounds.Image = global::ToolsCore.GlobalResources.move;
            this.cmiMoveSounds.Name = "cmiMoveSounds";
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
            this.cmiConvertSoundsToEwa.Image = global::RawBankEditor.Properties.Resources.ewa;
            this.cmiConvertSoundsToEwa.Name = "cmiConvertSoundsToEwa";
            resources.ApplyResources(this.cmiConvertSoundsToEwa, "cmiConvertSoundsToEwa");
            // 
            // cmiConvertSoundsToWav
            // 
            this.cmiConvertSoundsToWav.Enabled = false;
            this.cmiConvertSoundsToWav.Image = global::RawBankEditor.Properties.Resources.wav;
            this.cmiConvertSoundsToWav.Name = "cmiConvertSoundsToWav";
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
            this.tsbErrors.Image = ((System.Drawing.Image)(resources.GetObject("tsbErrors.Image")));
            this.tsbErrors.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbErrors.Name = "tsbErrors";
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
            this.tsbWarnings.Image = ((System.Drawing.Image)(resources.GetObject("tsbWarnings.Image")));
            this.tsbWarnings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbWarnings.Name = "tsbWarnings";
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
            this.tsbInfos.Image = ((System.Drawing.Image)(resources.GetObject("tsbInfos.Image")));
            this.tsbInfos.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbInfos.Name = "tsbInfos";
            resources.ApplyResources(this.tsbInfos, "tsbInfos");
            this.tsbInfos.CheckedChanged += new System.EventHandler(this.tsbInfos_CheckedChanged);
            // 
            // tsbResolveProblem
            // 
            this.tsbResolveProblem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsbResolveProblem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbResolveProblem.Enabled = false;
            this.tsbResolveProblem.Image = global::ToolsCore.GlobalResources.correct;
            this.tsbResolveProblem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbResolveProblem.Name = "tsbResolveProblem";
            resources.ApplyResources(this.tsbResolveProblem, "tsbResolveProblem");
            // 
            // tsbHighlightProblem
            // 
            this.tsbHighlightProblem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsbHighlightProblem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbHighlightProblem.Enabled = false;
            this.tsbHighlightProblem.Image = global::ToolsCore.GlobalResources.analyze;
            this.tsbHighlightProblem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHighlightProblem.Name = "tsbHighlightProblem";
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
            this.cmiHighlightProblem.Image = global::ToolsCore.GlobalResources.analyze;
            this.cmiHighlightProblem.Name = "cmiHighlightProblem";
            resources.ApplyResources(this.cmiHighlightProblem, "cmiHighlightProblem");
            // 
            // cmiResolveProblem
            // 
            this.cmiResolveProblem.Enabled = false;
            this.cmiResolveProblem.Image = global::ToolsCore.GlobalResources.correct;
            this.cmiResolveProblem.Name = "cmiResolveProblem";
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
            this.tssbErrors.Image = global::ToolsCore.GlobalResources.correct;
            this.tssbErrors.ImageTransparentColor = System.Drawing.Color.Magenta;
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
            this.tsbOpen.Image = global::ToolsCore.GlobalResources.open;
            this.tsbOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbOpen.Name = "tsbOpen";
            resources.ApplyResources(this.tsbOpen, "tsbOpen");
            // 
            // tsbRecent
            // 
            this.tsbRecent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRecent.Image = global::ToolsCore.GlobalResources.recent_gvds;
            this.tsbRecent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRecent.Name = "tsbRecent";
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
            this.tsbSave.Image = global::ToolsCore.GlobalResources.save;
            this.tsbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSave.Name = "tsbSave";
            resources.ApplyResources(this.tsbSave, "tsbSave");
            // 
            // tsbSaveAll
            // 
            this.tsbSaveAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSaveAll.Enabled = false;
            this.tsbSaveAll.Image = global::ToolsCore.GlobalResources.save_all;
            this.tsbSaveAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSaveAll.Name = "tsbSaveAll";
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
            this.tsbGoBack.Image = global::ToolsCore.GlobalResources.back;
            this.tsbGoBack.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbGoBack.Name = "tsbGoBack";
            resources.ApplyResources(this.tsbGoBack, "tsbGoBack");
            // 
            // tsbGoForward
            // 
            this.tsbGoForward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbGoForward.Enabled = false;
            this.tsbGoForward.Image = global::ToolsCore.GlobalResources.forward;
            this.tsbGoForward.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbGoForward.Name = "tsbGoForward";
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
            this.tsbUndo.Image = global::ToolsCore.GlobalResources.undo;
            this.tsbUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbUndo.Name = "tsbUndo";
            resources.ApplyResources(this.tsbUndo, "tsbUndo");
            // 
            // tsbRedo
            // 
            this.tsbRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRedo.Enabled = false;
            this.tsbRedo.Image = global::ToolsCore.GlobalResources.redo;
            this.tsbRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRedo.Name = "tsbRedo";
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
            this.tsmiConvertLangToWav});
            this.tsbLangsSettings.Enabled = false;
            this.tsbLangsSettings.Image = global::ToolsCore.GlobalResources.global_settings;
            this.tsbLangsSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbLangsSettings.Margin = new System.Windows.Forms.Padding(5, 1, 0, 2);
            this.tsbLangsSettings.Name = "tsbLangsSettings";
            resources.ApplyResources(this.tsbLangsSettings, "tsbLangsSettings");
            // 
            // tsmiAddLanguage
            // 
            this.tsmiAddLanguage.Enabled = false;
            this.tsmiAddLanguage.Image = global::ToolsCore.GlobalResources.add;
            this.tsmiAddLanguage.Name = "tsmiAddLanguage";
            resources.ApplyResources(this.tsmiAddLanguage, "tsmiAddLanguage");
            // 
            // tsmiEditLanguage
            // 
            this.tsmiEditLanguage.Enabled = false;
            this.tsmiEditLanguage.Image = global::ToolsCore.GlobalResources.edit;
            this.tsmiEditLanguage.Name = "tsmiEditLanguage";
            resources.ApplyResources(this.tsmiEditLanguage, "tsmiEditLanguage");
            // 
            // tsmiDeleteLanguage
            // 
            this.tsmiDeleteLanguage.Enabled = false;
            this.tsmiDeleteLanguage.Image = global::ToolsCore.GlobalResources.delete;
            this.tsmiDeleteLanguage.Name = "tsmiDeleteLanguage";
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
            this.tsmiConvertLangToEwa.Image = global::RawBankEditor.Properties.Resources.ewa;
            this.tsmiConvertLangToEwa.Name = "tsmiConvertLangToEwa";
            resources.ApplyResources(this.tsmiConvertLangToEwa, "tsmiConvertLangToEwa");
            // 
            // tsmiConvertLangToWav
            // 
            this.tsmiConvertLangToWav.Enabled = false;
            this.tsmiConvertLangToWav.Image = global::RawBankEditor.Properties.Resources.wav;
            this.tsmiConvertLangToWav.Name = "tsmiConvertLangToWav";
            resources.ApplyResources(this.tsmiConvertLangToWav, "tsmiConvertLangToWav");
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            resources.ApplyResources(this.toolStripSeparator7, "toolStripSeparator7");
            // 
            // tsbAppSettings
            // 
            this.tsbAppSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAppSettings.Image = global::ToolsCore.GlobalResources.app_settings;
            this.tsbAppSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAppSettings.Name = "tsbAppSettings";
            resources.ApplyResources(this.tsbAppSettings, "tsbAppSettings");
            // 
            // tsbInfoApp
            // 
            this.tsbInfoApp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbInfoApp.Image = global::ToolsCore.GlobalResources.info_app;
            this.tsbInfoApp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbInfoApp.Name = "tsbInfoApp";
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
            this.tsbAddSound.Image = global::ToolsCore.GlobalResources.add;
            this.tsbAddSound.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAddSound.Name = "tsbAddSound";
            resources.ApplyResources(this.tsbAddSound, "tsbAddSound");
            // 
            // tsbMoveSounds
            // 
            this.tsbMoveSounds.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbMoveSounds.Enabled = false;
            this.tsbMoveSounds.Image = global::ToolsCore.GlobalResources.move;
            this.tsbMoveSounds.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbMoveSounds.Name = "tsbMoveSounds";
            resources.ApplyResources(this.tsbMoveSounds, "tsbMoveSounds");
            // 
            // tsbDeleteSound
            // 
            this.tsbDeleteSound.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDeleteSound.Enabled = false;
            this.tsbDeleteSound.Image = global::ToolsCore.GlobalResources.delete;
            this.tsbDeleteSound.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDeleteSound.Name = "tsbDeleteSound";
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
            this.tsbConvertSoundsToEwa.Image = global::RawBankEditor.Properties.Resources.ewa;
            this.tsbConvertSoundsToEwa.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbConvertSoundsToEwa.Name = "tsbConvertSoundsToEwa";
            resources.ApplyResources(this.tsbConvertSoundsToEwa, "tsbConvertSoundsToEwa");
            // 
            // tsbConvertSoundsToWav
            // 
            this.tsbConvertSoundsToWav.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbConvertSoundsToWav.Enabled = false;
            this.tsbConvertSoundsToWav.Image = global::RawBankEditor.Properties.Resources.wav;
            this.tsbConvertSoundsToWav.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbConvertSoundsToWav.Name = "tsbConvertSoundsToWav";
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
            this.tsbSearch.Image = global::ToolsCore.GlobalResources.search;
            this.tsbSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSearch.Name = "tsbSearch";
            resources.ApplyResources(this.tsbSearch, "tsbSearch");
            // 
            // tsbWrapTextSoundCol
            // 
            this.tsbWrapTextSoundCol.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsbWrapTextSoundCol.CheckOnClick = true;
            this.tsbWrapTextSoundCol.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbWrapTextSoundCol.Image = global::ToolsCore.GlobalResources.textbox;
            this.tsbWrapTextSoundCol.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbWrapTextSoundCol.Margin = new System.Windows.Forms.Padding(0, 1, 4, 2);
            this.tsbWrapTextSoundCol.Name = "tsbWrapTextSoundCol";
            resources.ApplyResources(this.tsbWrapTextSoundCol, "tsbWrapTextSoundCol");
            this.tsbWrapTextSoundCol.CheckedChanged += new System.EventHandler(this.WrapSoundTextChanged);
            // 
            // tsbRewriteMode
            // 
            this.tsbRewriteMode.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsbRewriteMode.CheckOnClick = true;
            this.tsbRewriteMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRewriteMode.Enabled = false;
            this.tsbRewriteMode.Image = global::ToolsCore.GlobalResources.convert;
            this.tsbRewriteMode.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRewriteMode.Name = "tsbRewriteMode";
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
            this.Controls.Add(this.menuStripMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStripMain;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FMain";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FMain_FormClosed);
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
        private System.Windows.Forms.ToolStripButton tsbOpen;
        private ExTableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label2;
        private ExControls.ExToolStripComboBox tscboxLanguages;
        private System.Windows.Forms.ToolStripMenuItem tsmimOpen;
        private System.Windows.Forms.ToolStripMenuItem tsmimRecent;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem tsmimSave;
        private System.Windows.Forms.ToolStripMenuItem tsmiTools;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripButton tsbEditGroup;
        private System.Windows.Forms.ToolStripButton tsbDeleteGroup;
        private System.Windows.Forms.ToolStripMenuItem tsmimInfoApp;
        private System.Windows.Forms.ToolStripButton tsbSave;
        private System.Windows.Forms.ToolStripButton tsbInfoApp;
        private System.Windows.Forms.ToolStripStatusLabel tsslStatus;
        private System.Windows.Forms.ToolStripProgressBar tspbProgress;
        private System.Windows.Forms.ToolStripMenuItem tsmimLangsSettings;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem tsmimAppSettings;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripButton tsbAppSettings;
        private System.Windows.Forms.ContextMenuStrip contextMenuGroups;
        private System.Windows.Forms.ToolStripMenuItem cmiAddGroup;
        private System.Windows.Forms.ToolStripMenuItem cmiEditGroup;
        private System.Windows.Forms.ToolStripMenuItem cmiDeleteGroup;
        private System.Windows.Forms.ContextMenuStrip contextMenuSounds;
        private System.Windows.Forms.ToolStrip toolStripExplorer;
        private System.Windows.Forms.ToolStripButton tsbGoForward;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripButton tsbRedo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripMenuItem tsmimUndo;
        private System.Windows.Forms.ToolStripMenuItem tsmimRedo;
        private System.Windows.Forms.ToolStripMenuItem tsmimGoBack;
        private System.Windows.Forms.ToolStripMenuItem tsmimGoForward;
        private System.Windows.Forms.ToolStripDropDownButton tsbRecent;
        private System.Windows.Forms.ToolStripMenuItem tsmimUpdates;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripMenuItem tsmimAddSound;
        private System.Windows.Forms.ToolStripMenuItem tsmimDeleteSound;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripButton tsbAddSound;
        private System.Windows.Forms.ToolStripButton tsbDeleteSound;
        private System.Windows.Forms.ToolStripButton tsbAddGroup;
        private System.Windows.Forms.BindingSource fyzSoundBindingSource;
        private System.Windows.Forms.ToolStripButton tsbConvertSoundsToEwa;
        private System.Windows.Forms.ToolStripMenuItem cmiAddSound;
        private System.Windows.Forms.ToolStripMenuItem cmiDeleteSound;
        private System.IO.FileSystemWatcher fileSystemWatcher;
        private System.Windows.Forms.SplitContainer splitSoundsErrors;
        private ExTableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStrip toolStripErrors;
        private System.Windows.Forms.ToolStripButton tsbErrors;
        private System.Windows.Forms.ToolStripButton tsbWarnings;
        private System.Windows.Forms.ToolStripButton tsbInfos;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator13;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
        internal System.Windows.Forms.DataGridView dgvErrors;
        private System.Windows.Forms.ToolStripButton tsbOpenInExplorer;
        private System.Windows.Forms.ToolStripButton tsbPlay;
        private ExTableLayoutPanel tableLayoutPanel5;
        private Controls.MinButton mmErrors;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator16;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator17;
        private System.Windows.Forms.ToolStripMenuItem tsmimMoveSounds;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
        private System.Windows.Forms.ToolStripButton tsbSearch;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator18;
        private System.Windows.Forms.ToolStripMenuItem tsmimSearch;
        internal System.Windows.Forms.DataGridView dgvExplorer;
        private System.Windows.Forms.ToolStripButton tsbMoveSounds;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.BindingSource fileSystemElementBindingSource;
        private System.Windows.Forms.ToolStripButton tsbRenameFileDir;
        private System.Windows.Forms.ToolStripButton tsbDeleteFileDir;
        private System.Windows.Forms.BindingSource rawBankMessageBindingSource;
        private System.Windows.Forms.ToolStripDropDownButton tssbErrors;
        private ToolStripSeparator toolStripSeparator19;
        private ToolStripMenuItem cmiConvertSoundsToWav;
        private ToolStripMenuItem cmiConvertSoundsToEwa;
        internal DataGridView dgvGroups;
        internal DataGridView dgvSounds;
        private ContextMenuStrip contextMenuExplorer;
        private ToolStripMenuItem cmiOpenInExplorer;
        private ToolStripMenuItem cmiPlay;
        private ToolStripSeparator toolStripSeparator20;
        private ToolStripMenuItem cmiRenameFileDir;
        private ToolStripMenuItem cmiDeleteFileDir;
        private ContextMenuStrip contextMenuErrors;
        private ToolStripMenuItem cmiHighlightProblem;
        private ToolStripMenuItem cmiResolveProblem;
        private ToolStripButton tsbResolveProblem;
        private ToolStripButton tsbHighlightProblem;
        private BindingSource fyzGroupBindingSource;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem cmiConvertGroupToWav;
        private ToolStripMenuItem cmiConvertGroupToEwa;
        private ToolStripSeparator toolStripSeparator21;
        private ToolStripButton tsbConvertGroupToEwa;
        private ToolStripButton tsbConvertGroupToWav;
        private ToolStripButton tsbConvertSoundsToWav;
        private ToolStripSeparator toolStripSeparator22;
        private ToolStripButton tsbConvertFilesToEwa;
        private ToolStripButton tsbConvertFilesToWav;
        private ToolStripSeparator toolStripSeparator23;
        private ToolStripMenuItem cmiConvertToEwaFile;
        private ToolStripMenuItem cmiConvertToWavFile;
        private ExControls.Providers.BackwardForwardProvider moveManager;
        internal ExControls.Providers.UndoRedoManager changeManager;
        private ToolStripMenuItem tsmimSaveAll;
        private ToolStripButton tsbSaveAll;
        private ToolStripSeparator toolStripSeparator25;
        private System.Windows.Forms.Timer timerToCheck;
        private ToolStripSeparator toolStripSeparator26;
        private ToolStripMenuItem tsmimRewriteMode;
        private ToolStripButton tsbRewriteMode;
        private ToolStripMenuItem tsmimAddLanguage;
        private ToolStripMenuItem tsmimEditLanguage;
        private ToolStripMenuItem tsmimDeleteLanguage;
        private ToolStripDropDownButton tsbLangsSettings;
        private ToolStripMenuItem tsmiAddLanguage;
        private ToolStripMenuItem tsmiEditLanguage;
        private ToolStripMenuItem tsmiDeleteLanguage;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem tsmimShowErrors;
        private ToolStripMenuItem tsmimConvertLangToEwa;
        private ToolStripMenuItem tsmimConvertLangToWav;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem tsmiConvertLangToEwa;
        private ToolStripMenuItem tsmiConvertLangToWav;
        private ToolStripMenuItem tsmimConvertSoundsToEwa;
        private ToolStripMenuItem tsmimConvertSoundsToWav;
        private ToolStripMenuItem cmiMoveSounds;
        private DataGridViewImageColumn cFileType;
        private DataGridViewTextBoxColumn cFileName;
        private DataGridViewTextBoxColumn cFileDuration;
        private ToolStripButton tsbWrapTextSoundCol;
        private ToolStripMenuItem tsmimWrapTextSoundCol;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripSplitButton tsbGoBack;
        private ToolStripUndoRedoActionChooser undoActionChooser;
        private ToolStripButton tsbUndo;
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

