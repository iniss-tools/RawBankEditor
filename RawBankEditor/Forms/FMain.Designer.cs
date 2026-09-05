
using ExControls;
using RawBankEditor.Entities;
using RawBankEditor.Tools;
using ToolsCore.Entities;

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
            this.bWorkerReadDat = new System.ComponentModel.BackgroundWorker();
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
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStripMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menuStripMain.Size = new System.Drawing.Size(1095, 24);
            this.menuStripMain.TabIndex = 0;
            this.menuStripMain.Text = "menuStrip1";
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
            this.tsmiFile.Size = new System.Drawing.Size(50, 20);
            this.tsmiFile.Text = "&Súbor";
            // 
            // tsmimOpen
            // 
            this.tsmimOpen.Image = global::ToolsCore.GlobalResources.open;
            this.tsmimOpen.Name = "tsmimOpen";
            this.tsmimOpen.Size = new System.Drawing.Size(142, 22);
            this.tsmimOpen.Text = "&Otvoriť...";
            this.tsmimOpen.Click += new System.EventHandler(this.DoOpenDir);
            // 
            // tsmimRecent
            // 
            this.tsmimRecent.Image = global::ToolsCore.GlobalResources.recent_gvds;
            this.tsmimRecent.Name = "tsmimRecent";
            this.tsmimRecent.Size = new System.Drawing.Size(142, 22);
            this.tsmimRecent.Text = "&Nedávne";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(139, 6);
            // 
            // tsmimSave
            // 
            this.tsmimSave.Enabled = false;
            this.tsmimSave.Image = global::ToolsCore.GlobalResources.save;
            this.tsmimSave.Name = "tsmimSave";
            this.tsmimSave.Size = new System.Drawing.Size(142, 22);
            this.tsmimSave.Text = "&Uložiť";
            this.tsmimSave.Click += new System.EventHandler(this.DoSave);
            // 
            // tsmimSaveAll
            // 
            this.tsmimSaveAll.Enabled = false;
            this.tsmimSaveAll.Image = global::ToolsCore.GlobalResources.save_all;
            this.tsmimSaveAll.Name = "tsmimSaveAll";
            this.tsmimSaveAll.Size = new System.Drawing.Size(142, 22);
            this.tsmimSaveAll.Text = "Uložiť &všetko";
            this.tsmimSaveAll.Click += new System.EventHandler(this.DoSaveAll);
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
            this.tsmiEdit.Size = new System.Drawing.Size(58, 20);
            this.tsmiEdit.Text = "&Upraviť";
            // 
            // tsmimUndo
            // 
            this.tsmimUndo.Enabled = false;
            this.tsmimUndo.Image = global::ToolsCore.GlobalResources.undo;
            this.tsmimUndo.Name = "tsmimUndo";
            this.tsmimUndo.Size = new System.Drawing.Size(264, 22);
            this.tsmimUndo.Text = "&Späť";
            this.tsmimUndo.Click += new System.EventHandler(this.DoUndo);
            // 
            // tsmimRedo
            // 
            this.tsmimRedo.Enabled = false;
            this.tsmimRedo.Image = global::ToolsCore.GlobalResources.redo;
            this.tsmimRedo.Name = "tsmimRedo";
            this.tsmimRedo.Size = new System.Drawing.Size(264, 22);
            this.tsmimRedo.Text = "&Znovu";
            this.tsmimRedo.Click += new System.EventHandler(this.DoRedo);
            // 
            // toolStripSeparator11
            // 
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            this.toolStripSeparator11.Size = new System.Drawing.Size(261, 6);
            // 
            // tsmimAddSound
            // 
            this.tsmimAddSound.Enabled = false;
            this.tsmimAddSound.Image = global::ToolsCore.GlobalResources.add;
            this.tsmimAddSound.Name = "tsmimAddSound";
            this.tsmimAddSound.Size = new System.Drawing.Size(264, 22);
            this.tsmimAddSound.Text = "P&ridať zvuk...";
            this.tsmimAddSound.Click += new System.EventHandler(this.DoAddSound);
            // 
            // tsmimDeleteSound
            // 
            this.tsmimDeleteSound.Enabled = false;
            this.tsmimDeleteSound.Image = global::ToolsCore.GlobalResources.delete;
            this.tsmimDeleteSound.Name = "tsmimDeleteSound";
            this.tsmimDeleteSound.Size = new System.Drawing.Size(264, 22);
            this.tsmimDeleteSound.Text = "O&dstrániť zvuky";
            this.tsmimDeleteSound.Click += new System.EventHandler(this.DoDeleteSounds);
            // 
            // tsmimMoveSounds
            // 
            this.tsmimMoveSounds.Enabled = false;
            this.tsmimMoveSounds.Image = global::ToolsCore.GlobalResources.move;
            this.tsmimMoveSounds.Name = "tsmimMoveSounds";
            this.tsmimMoveSounds.Size = new System.Drawing.Size(264, 22);
            this.tsmimMoveSounds.Text = "&Premiestniť zvuky...";
            this.tsmimMoveSounds.Click += new System.EventHandler(this.DoMoveSounds);
            // 
            // tsmimConvertSoundsToEwa
            // 
            this.tsmimConvertSoundsToEwa.Enabled = false;
            this.tsmimConvertSoundsToEwa.Image = global::RawBankEditor.Properties.Resources.ewa;
            this.tsmimConvertSoundsToEwa.Name = "tsmimConvertSoundsToEwa";
            this.tsmimConvertSoundsToEwa.Size = new System.Drawing.Size(264, 22);
            this.tsmimConvertSoundsToEwa.Text = "Konvertovať vybrané zvuky na .&EWA";
            this.tsmimConvertSoundsToEwa.Click += new System.EventHandler(this.DoConvertSoundsToEwa);
            // 
            // tsmimConvertSoundsToWav
            // 
            this.tsmimConvertSoundsToWav.Enabled = false;
            this.tsmimConvertSoundsToWav.Image = global::RawBankEditor.Properties.Resources.wav;
            this.tsmimConvertSoundsToWav.Name = "tsmimConvertSoundsToWav";
            this.tsmimConvertSoundsToWav.Size = new System.Drawing.Size(264, 22);
            this.tsmimConvertSoundsToWav.Text = "Konvertovať vybrané zvuky na .&WAV";
            this.tsmimConvertSoundsToWav.Click += new System.EventHandler(this.DoConvertSoundsToWav);
            // 
            // toolStripSeparator26
            // 
            this.toolStripSeparator26.Name = "toolStripSeparator26";
            this.toolStripSeparator26.Size = new System.Drawing.Size(261, 6);
            // 
            // tsmimRewriteMode
            // 
            this.tsmimRewriteMode.CheckOnClick = true;
            this.tsmimRewriteMode.Enabled = false;
            this.tsmimRewriteMode.Image = global::ToolsCore.GlobalResources.convert;
            this.tsmimRewriteMode.Name = "tsmimRewriteMode";
            this.tsmimRewriteMode.Size = new System.Drawing.Size(264, 22);
            this.tsmimRewriteMode.Text = "Prepisovací &mód";
            this.tsmimRewriteMode.CheckedChanged += new System.EventHandler(this.RewriteModeChanged);
            // 
            // tsmimWrapTextSoundCol
            // 
            this.tsmimWrapTextSoundCol.CheckOnClick = true;
            this.tsmimWrapTextSoundCol.Image = global::ToolsCore.GlobalResources.textbox;
            this.tsmimWrapTextSoundCol.Name = "tsmimWrapTextSoundCol";
            this.tsmimWrapTextSoundCol.Size = new System.Drawing.Size(264, 22);
            this.tsmimWrapTextSoundCol.Text = "Prispôsobiť &text hlásenia v bunkách";
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
            this.tsmiShow.Size = new System.Drawing.Size(63, 20);
            this.tsmiShow.Text = "Z&obraziť";
            // 
            // tsmimGoBack
            // 
            this.tsmimGoBack.Enabled = false;
            this.tsmimGoBack.Image = global::ToolsCore.GlobalResources.back;
            this.tsmimGoBack.Name = "tsmimGoBack";
            this.tsmimGoBack.Size = new System.Drawing.Size(152, 22);
            this.tsmimGoBack.Text = "Prejsť &späť";
            this.tsmimGoBack.Click += new System.EventHandler(this.DoGoBack);
            // 
            // tsmimGoForward
            // 
            this.tsmimGoForward.Enabled = false;
            this.tsmimGoForward.Image = global::ToolsCore.GlobalResources.forward;
            this.tsmimGoForward.Name = "tsmimGoForward";
            this.tsmimGoForward.Size = new System.Drawing.Size(152, 22);
            this.tsmimGoForward.Text = "Prejsť &dopredu";
            this.tsmimGoForward.Click += new System.EventHandler(this.DoGoForward);
            // 
            // toolStripSeparator18
            // 
            this.toolStripSeparator18.Name = "toolStripSeparator18";
            this.toolStripSeparator18.Size = new System.Drawing.Size(149, 6);
            // 
            // tsmimSearch
            // 
            this.tsmimSearch.Enabled = false;
            this.tsmimSearch.Image = global::ToolsCore.GlobalResources.search;
            this.tsmimSearch.Name = "tsmimSearch";
            this.tsmimSearch.Size = new System.Drawing.Size(152, 22);
            this.tsmimSearch.Text = "&Hľadať";
            this.tsmimSearch.Click += new System.EventHandler(this.DoSearch);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(149, 6);
            // 
            // tsmimShowErrors
            // 
            this.tsmimShowErrors.Checked = true;
            this.tsmimShowErrors.CheckOnClick = true;
            this.tsmimShowErrors.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmimShowErrors.Name = "tsmimShowErrors";
            this.tsmimShowErrors.Size = new System.Drawing.Size(152, 22);
            this.tsmimShowErrors.Text = "Zoznam &chýb";
            this.tsmimShowErrors.CheckedChanged += new System.EventHandler(this.TsmimShowErrors_CheckedChanged);
            // 
            // tsmiTools
            // 
            this.tsmiTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmimLangsSettings,
            this.toolStripSeparator8,
            this.tsmimAppSettings});
            this.tsmiTools.Name = "tsmiTools";
            this.tsmiTools.Size = new System.Drawing.Size(63, 20);
            this.tsmiTools.Text = "&Nástroje";
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
            this.tsmimLangsSettings.Size = new System.Drawing.Size(188, 22);
            this.tsmimLangsSettings.Text = "Nastavenia &jazykov";
            // 
            // tsmimAddLanguage
            // 
            this.tsmimAddLanguage.Enabled = false;
            this.tsmimAddLanguage.Image = global::ToolsCore.GlobalResources.add;
            this.tsmimAddLanguage.Name = "tsmimAddLanguage";
            this.tsmimAddLanguage.Size = new System.Drawing.Size(264, 22);
            this.tsmimAddLanguage.Text = "P&ridať jazyk...";
            this.tsmimAddLanguage.Click += new System.EventHandler(this.DoAddLanguage);
            // 
            // tsmimEditLanguage
            // 
            this.tsmimEditLanguage.Enabled = false;
            this.tsmimEditLanguage.Image = global::ToolsCore.GlobalResources.edit;
            this.tsmimEditLanguage.Name = "tsmimEditLanguage";
            this.tsmimEditLanguage.Size = new System.Drawing.Size(264, 22);
            this.tsmimEditLanguage.Text = "&Upraviť jazyk...";
            this.tsmimEditLanguage.Click += new System.EventHandler(this.DoEditLanguage);
            // 
            // tsmimDeleteLanguage
            // 
            this.tsmimDeleteLanguage.Enabled = false;
            this.tsmimDeleteLanguage.Image = global::ToolsCore.GlobalResources.delete;
            this.tsmimDeleteLanguage.Name = "tsmimDeleteLanguage";
            this.tsmimDeleteLanguage.Size = new System.Drawing.Size(264, 22);
            this.tsmimDeleteLanguage.Text = "O&dstrániť jazyk";
            this.tsmimDeleteLanguage.Click += new System.EventHandler(this.DoDeleteLanguage);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(261, 6);
            // 
            // tsmimConvertLangToEwa
            // 
            this.tsmimConvertLangToEwa.Enabled = false;
            this.tsmimConvertLangToEwa.Image = global::RawBankEditor.Properties.Resources.ewa;
            this.tsmimConvertLangToEwa.Name = "tsmimConvertLangToEwa";
            this.tsmimConvertLangToEwa.Size = new System.Drawing.Size(264, 22);
            this.tsmimConvertLangToEwa.Text = "Konvertovať zvuky v jazyku na .&EWA";
            this.tsmimConvertLangToEwa.Click += new System.EventHandler(this.DoConvertLangToEwa);
            // 
            // tsmimConvertLangToWav
            // 
            this.tsmimConvertLangToWav.Enabled = false;
            this.tsmimConvertLangToWav.Image = global::RawBankEditor.Properties.Resources.wav;
            this.tsmimConvertLangToWav.Name = "tsmimConvertLangToWav";
            this.tsmimConvertLangToWav.Size = new System.Drawing.Size(264, 22);
            this.tsmimConvertLangToWav.Text = "Konvertovať zvuky v jazyku na .&WAV";
            this.tsmimConvertLangToWav.Click += new System.EventHandler(this.DoConvertLangToWav);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(185, 6);
            // 
            // tsmimAppSettings
            // 
            this.tsmimAppSettings.Image = global::ToolsCore.GlobalResources.app_settings;
            this.tsmimAppSettings.Name = "tsmimAppSettings";
            this.tsmimAppSettings.Size = new System.Drawing.Size(188, 22);
            this.tsmimAppSettings.Text = "Nastavenia &programu";
            this.tsmimAppSettings.Click += new System.EventHandler(this.ShowAppSettings);
            // 
            // tsmiHelp
            // 
            this.tsmiHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmimInfoApp,
            this.tsmimUpdates});
            this.tsmiHelp.Name = "tsmiHelp";
            this.tsmiHelp.Size = new System.Drawing.Size(73, 20);
            this.tsmiHelp.Text = "&Pomocník";
            // 
            // tsmimInfoApp
            // 
            this.tsmimInfoApp.Image = global::ToolsCore.GlobalResources.info_app;
            this.tsmimInfoApp.Name = "tsmimInfoApp";
            this.tsmimInfoApp.Size = new System.Drawing.Size(222, 22);
            this.tsmimInfoApp.Text = "&Informácie o programe";
            this.tsmimInfoApp.Click += new System.EventHandler(this.ShowInfoApp);
            // 
            // tsmimUpdates
            // 
            this.tsmimUpdates.Name = "tsmimUpdates";
            this.tsmimUpdates.Size = new System.Drawing.Size(222, 22);
            this.tsmimUpdates.Text = "&Poznámky k aktualizáciám...";
            this.tsmimUpdates.Click += new System.EventHandler(this.ShowUpdates);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(2, 2);
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
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(4, 0);
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
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(292, 186);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.label1.Size = new System.Drawing.Size(286, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "Skupiny zvukov";
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
            this.toolStripGroups.Location = new System.Drawing.Point(3, 27);
            this.toolStripGroups.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            this.toolStripGroups.Name = "toolStripGroups";
            this.toolStripGroups.Size = new System.Drawing.Size(286, 27);
            this.toolStripGroups.TabIndex = 1;
            this.toolStripGroups.Text = "toolStrip2";
            // 
            // tsbAddGroup
            // 
            this.tsbAddGroup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAddGroup.Enabled = false;
            this.tsbAddGroup.Image = global::ToolsCore.GlobalResources.add;
            this.tsbAddGroup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAddGroup.Name = "tsbAddGroup";
            this.tsbAddGroup.Size = new System.Drawing.Size(24, 24);
            this.tsbAddGroup.Text = "Pridať skupinu";
            this.tsbAddGroup.Click += new System.EventHandler(this.DoAddGroup);
            // 
            // tsbEditGroup
            // 
            this.tsbEditGroup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbEditGroup.Enabled = false;
            this.tsbEditGroup.Image = global::ToolsCore.GlobalResources.edit;
            this.tsbEditGroup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbEditGroup.Name = "tsbEditGroup";
            this.tsbEditGroup.Size = new System.Drawing.Size(24, 24);
            this.tsbEditGroup.Text = "Upraviť skupinu";
            this.tsbEditGroup.Click += new System.EventHandler(this.DoEditGroup);
            // 
            // tsbDeleteGroup
            // 
            this.tsbDeleteGroup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDeleteGroup.Enabled = false;
            this.tsbDeleteGroup.Image = global::ToolsCore.GlobalResources.delete;
            this.tsbDeleteGroup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDeleteGroup.Name = "tsbDeleteGroup";
            this.tsbDeleteGroup.Size = new System.Drawing.Size(24, 24);
            this.tsbDeleteGroup.Text = "Odstrániť skupinu";
            this.tsbDeleteGroup.Click += new System.EventHandler(this.DoDeleteGroup);
            // 
            // toolStripSeparator21
            // 
            this.toolStripSeparator21.Name = "toolStripSeparator21";
            this.toolStripSeparator21.Size = new System.Drawing.Size(6, 27);
            // 
            // tsbConvertGroupToEwa
            // 
            this.tsbConvertGroupToEwa.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbConvertGroupToEwa.Enabled = false;
            this.tsbConvertGroupToEwa.Image = global::RawBankEditor.Properties.Resources.ewa;
            this.tsbConvertGroupToEwa.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbConvertGroupToEwa.Name = "tsbConvertGroupToEwa";
            this.tsbConvertGroupToEwa.Size = new System.Drawing.Size(24, 24);
            this.tsbConvertGroupToEwa.Text = "Konvertovať skupinu na .EWA";
            this.tsbConvertGroupToEwa.Click += new System.EventHandler(this.DoConvertGroupToEwa);
            // 
            // tsbConvertGroupToWav
            // 
            this.tsbConvertGroupToWav.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbConvertGroupToWav.Enabled = false;
            this.tsbConvertGroupToWav.Image = global::RawBankEditor.Properties.Resources.wav;
            this.tsbConvertGroupToWav.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbConvertGroupToWav.Name = "tsbConvertGroupToWav";
            this.tsbConvertGroupToWav.Size = new System.Drawing.Size(24, 24);
            this.tsbConvertGroupToWav.Text = "Konvertovať skupinu na .WAV";
            this.tsbConvertGroupToWav.Click += new System.EventHandler(this.DoConvertGroupToWav);
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
            this.dgvGroups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvGroups.Location = new System.Drawing.Point(3, 57);
            this.dgvGroups.Margin = new System.Windows.Forms.Padding(2);
            this.dgvGroups.MultiSelect = false;
            this.dgvGroups.Name = "dgvGroups";
            this.dgvGroups.ReadOnly = true;
            this.dgvGroups.RowHeadersVisible = false;
            this.dgvGroups.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvGroups.Size = new System.Drawing.Size(286, 126);
            this.dgvGroups.TabIndex = 2;
            this.dgvGroups.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DgvGroups_CellMouseDown);
            this.dgvGroups.SelectionChanged += new System.EventHandler(this.dgvGroups_SelectionChanged);
            this.dgvGroups.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DgvGroups_KeyDown);
            // 
            // cGroupName
            // 
            this.cGroupName.DataPropertyName = "Name";
            this.cGroupName.HeaderText = "Názov";
            this.cGroupName.Name = "cGroupName";
            this.cGroupName.ReadOnly = true;
            // 
            // cGroupRelativePath
            // 
            this.cGroupRelativePath.DataPropertyName = "RelativePath";
            this.cGroupRelativePath.HeaderText = "Relatívna cesta";
            this.cGroupRelativePath.Name = "cGroupRelativePath";
            this.cGroupRelativePath.ReadOnly = true;
            // 
            // cGroupCountSounds
            // 
            this.cGroupCountSounds.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cGroupCountSounds.DataPropertyName = "CountSounds";
            this.cGroupCountSounds.HeaderText = "Počet zvukov";
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
            this.contextMenuGroups.Size = new System.Drawing.Size(236, 140);
            // 
            // cmiAddGroup
            // 
            this.cmiAddGroup.Enabled = false;
            this.cmiAddGroup.Image = global::ToolsCore.GlobalResources.add;
            this.cmiAddGroup.Name = "cmiAddGroup";
            this.cmiAddGroup.Size = new System.Drawing.Size(235, 26);
            this.cmiAddGroup.Text = "Pridať skupinu...";
            this.cmiAddGroup.Click += new System.EventHandler(this.DoAddGroup);
            // 
            // cmiEditGroup
            // 
            this.cmiEditGroup.Enabled = false;
            this.cmiEditGroup.Image = global::ToolsCore.GlobalResources.edit;
            this.cmiEditGroup.Name = "cmiEditGroup";
            this.cmiEditGroup.Size = new System.Drawing.Size(235, 26);
            this.cmiEditGroup.Text = "Upraviť skupinu...";
            this.cmiEditGroup.Click += new System.EventHandler(this.DoEditGroup);
            // 
            // cmiDeleteGroup
            // 
            this.cmiDeleteGroup.Enabled = false;
            this.cmiDeleteGroup.Image = global::ToolsCore.GlobalResources.delete;
            this.cmiDeleteGroup.Name = "cmiDeleteGroup";
            this.cmiDeleteGroup.Size = new System.Drawing.Size(235, 26);
            this.cmiDeleteGroup.Text = "Odstrániť skupinu";
            this.cmiDeleteGroup.Click += new System.EventHandler(this.DoDeleteGroup);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(232, 6);
            // 
            // cmiConvertGroupToEwa
            // 
            this.cmiConvertGroupToEwa.Enabled = false;
            this.cmiConvertGroupToEwa.Image = global::RawBankEditor.Properties.Resources.ewa;
            this.cmiConvertGroupToEwa.Name = "cmiConvertGroupToEwa";
            this.cmiConvertGroupToEwa.Size = new System.Drawing.Size(235, 26);
            this.cmiConvertGroupToEwa.Text = "Konvertovať skupinu na .EWA";
            // 
            // cmiConvertGroupToWav
            // 
            this.cmiConvertGroupToWav.Enabled = false;
            this.cmiConvertGroupToWav.Image = global::RawBankEditor.Properties.Resources.wav;
            this.cmiConvertGroupToWav.Name = "cmiConvertGroupToWav";
            this.cmiConvertGroupToWav.Size = new System.Drawing.Size(235, 26);
            this.cmiConvertGroupToWav.Text = "Konvertovať skupinu na .WAV";
            // 
            // fyzGroupBindingSource
            // 
            this.fyzGroupBindingSource.DataSource = typeof(ToolsCore.Entities.FyzGroup);
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
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(292, 310);
            this.tableLayoutPanel3.TabIndex = 0;
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
            this.dgvExplorer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvExplorer.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvExplorer.Location = new System.Drawing.Point(3, 59);
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
            this.dgvExplorer.Size = new System.Drawing.Size(286, 248);
            this.dgvExplorer.TabIndex = 2;
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
            this.cFileName.HeaderText = "Názov";
            this.cFileName.MinimumWidth = 100;
            this.cFileName.Name = "cFileName";
            this.cFileName.Width = 160;
            // 
            // cFileType
            // 
            this.cFileType.HeaderText = "Typ";
            this.cFileType.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
            this.cFileType.MinimumWidth = 50;
            this.cFileType.Name = "cFileType";
            this.cFileType.ReadOnly = true;
            this.cFileType.Width = 50;
            // 
            // cFileDuration
            // 
            this.cFileDuration.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cFileDuration.HeaderText = "Dĺžka";
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
            this.contextMenuExplorer.Size = new System.Drawing.Size(229, 148);
            // 
            // cmiOpenInExplorer
            // 
            this.cmiOpenInExplorer.Enabled = false;
            this.cmiOpenInExplorer.Image = global::ToolsCore.GlobalResources.folder;
            this.cmiOpenInExplorer.Name = "cmiOpenInExplorer";
            this.cmiOpenInExplorer.Size = new System.Drawing.Size(228, 22);
            this.cmiOpenInExplorer.Text = "Otvoriť v prieskumníkovi";
            this.cmiOpenInExplorer.Click += new System.EventHandler(this.DoOpenFileInExplorer);
            // 
            // cmiPlay
            // 
            this.cmiPlay.Enabled = false;
            this.cmiPlay.Image = global::ToolsCore.GlobalResources.sound;
            this.cmiPlay.Name = "cmiPlay";
            this.cmiPlay.ShortcutKeyDisplayString = "F5";
            this.cmiPlay.Size = new System.Drawing.Size(228, 22);
            this.cmiPlay.Text = "Prehrať zvuk";
            this.cmiPlay.Click += new System.EventHandler(this.DoPlayFile);
            // 
            // toolStripSeparator20
            // 
            this.toolStripSeparator20.Name = "toolStripSeparator20";
            this.toolStripSeparator20.Size = new System.Drawing.Size(225, 6);
            // 
            // cmiRenameFileDir
            // 
            this.cmiRenameFileDir.Enabled = false;
            this.cmiRenameFileDir.Image = global::ToolsCore.GlobalResources.rename;
            this.cmiRenameFileDir.Name = "cmiRenameFileDir";
            this.cmiRenameFileDir.ShortcutKeyDisplayString = "F2";
            this.cmiRenameFileDir.Size = new System.Drawing.Size(228, 22);
            this.cmiRenameFileDir.Text = "Premenovať súbor";
            this.cmiRenameFileDir.Click += new System.EventHandler(this.DoRenameFile);
            // 
            // cmiDeleteFileDir
            // 
            this.cmiDeleteFileDir.Enabled = false;
            this.cmiDeleteFileDir.Image = global::ToolsCore.GlobalResources.delete;
            this.cmiDeleteFileDir.Name = "cmiDeleteFileDir";
            this.cmiDeleteFileDir.ShortcutKeyDisplayString = "Del";
            this.cmiDeleteFileDir.Size = new System.Drawing.Size(228, 22);
            this.cmiDeleteFileDir.Text = "Odstrániť súbor";
            this.cmiDeleteFileDir.Click += new System.EventHandler(this.DoDeleteFile);
            // 
            // toolStripSeparator23
            // 
            this.toolStripSeparator23.Name = "toolStripSeparator23";
            this.toolStripSeparator23.Size = new System.Drawing.Size(225, 6);
            // 
            // cmiConvertToEwaFile
            // 
            this.cmiConvertToEwaFile.Enabled = false;
            this.cmiConvertToEwaFile.Image = global::RawBankEditor.Properties.Resources.ewa;
            this.cmiConvertToEwaFile.Name = "cmiConvertToEwaFile";
            this.cmiConvertToEwaFile.Size = new System.Drawing.Size(228, 22);
            this.cmiConvertToEwaFile.Text = "Konvertovať  súbory na .EWA";
            // 
            // cmiConvertToWavFile
            // 
            this.cmiConvertToWavFile.Enabled = false;
            this.cmiConvertToWavFile.Image = global::RawBankEditor.Properties.Resources.wav;
            this.cmiConvertToWavFile.Name = "cmiConvertToWavFile";
            this.cmiConvertToWavFile.Size = new System.Drawing.Size(228, 22);
            this.cmiConvertToWavFile.Text = "Konvertovať  súbory na .WAV";
            // 
            // fileSystemElementBindingSource
            // 
            this.fileSystemElementBindingSource.DataSource = typeof(ToolsCore.Entities.FileSystemElement);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 3);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.label2.Size = new System.Drawing.Size(286, 21);
            this.label2.TabIndex = 0;
            this.label2.Text = "Prieskumník súborov";
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
            this.toolStripExplorer.Location = new System.Drawing.Point(3, 27);
            this.toolStripExplorer.Margin = new System.Windows.Forms.Padding(2);
            this.toolStripExplorer.Name = "toolStripExplorer";
            this.toolStripExplorer.Size = new System.Drawing.Size(286, 27);
            this.toolStripExplorer.TabIndex = 1;
            this.toolStripExplorer.Text = "toolStrip2";
            // 
            // tsbOpenInExplorer
            // 
            this.tsbOpenInExplorer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbOpenInExplorer.Enabled = false;
            this.tsbOpenInExplorer.Image = global::ToolsCore.GlobalResources.folder;
            this.tsbOpenInExplorer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbOpenInExplorer.Name = "tsbOpenInExplorer";
            this.tsbOpenInExplorer.Size = new System.Drawing.Size(24, 24);
            this.tsbOpenInExplorer.Text = "Otvoriť v prieskumníkovi";
            this.tsbOpenInExplorer.Click += new System.EventHandler(this.DoOpenFileInExplorer);
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            this.toolStripSeparator16.Size = new System.Drawing.Size(6, 27);
            // 
            // tsbPlay
            // 
            this.tsbPlay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbPlay.Enabled = false;
            this.tsbPlay.Image = global::ToolsCore.GlobalResources.sound;
            this.tsbPlay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbPlay.Name = "tsbPlay";
            this.tsbPlay.Size = new System.Drawing.Size(24, 24);
            this.tsbPlay.Text = "Prehrať zvuk (F5)";
            this.tsbPlay.Click += new System.EventHandler(this.DoPlayFile);
            // 
            // toolStripSeparator17
            // 
            this.toolStripSeparator17.Name = "toolStripSeparator17";
            this.toolStripSeparator17.Size = new System.Drawing.Size(6, 27);
            // 
            // tsbRenameFileDir
            // 
            this.tsbRenameFileDir.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRenameFileDir.Enabled = false;
            this.tsbRenameFileDir.Image = global::ToolsCore.GlobalResources.rename;
            this.tsbRenameFileDir.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRenameFileDir.Name = "tsbRenameFileDir";
            this.tsbRenameFileDir.Size = new System.Drawing.Size(24, 24);
            this.tsbRenameFileDir.Text = "Premenovať (F2)";
            this.tsbRenameFileDir.Click += new System.EventHandler(this.DoRenameFile);
            // 
            // tsbDeleteFileDir
            // 
            this.tsbDeleteFileDir.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDeleteFileDir.Enabled = false;
            this.tsbDeleteFileDir.Image = global::ToolsCore.GlobalResources.delete;
            this.tsbDeleteFileDir.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDeleteFileDir.Name = "tsbDeleteFileDir";
            this.tsbDeleteFileDir.Size = new System.Drawing.Size(24, 24);
            this.tsbDeleteFileDir.Text = "Odstrániť (Del)";
            this.tsbDeleteFileDir.Click += new System.EventHandler(this.DoDeleteFile);
            // 
            // toolStripSeparator22
            // 
            this.toolStripSeparator22.Name = "toolStripSeparator22";
            this.toolStripSeparator22.Size = new System.Drawing.Size(6, 27);
            // 
            // tsbConvertFilesToEwa
            // 
            this.tsbConvertFilesToEwa.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbConvertFilesToEwa.Enabled = false;
            this.tsbConvertFilesToEwa.Image = global::RawBankEditor.Properties.Resources.ewa;
            this.tsbConvertFilesToEwa.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbConvertFilesToEwa.Name = "tsbConvertFilesToEwa";
            this.tsbConvertFilesToEwa.Size = new System.Drawing.Size(24, 24);
            this.tsbConvertFilesToEwa.Text = "Konvertovať súbory na .EWA";
            this.tsbConvertFilesToEwa.Click += new System.EventHandler(this.DoConvertFilesToEwa);
            // 
            // tsbConvertFilesToWav
            // 
            this.tsbConvertFilesToWav.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbConvertFilesToWav.Enabled = false;
            this.tsbConvertFilesToWav.Image = global::RawBankEditor.Properties.Resources.wav;
            this.tsbConvertFilesToWav.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbConvertFilesToWav.Name = "tsbConvertFilesToWav";
            this.tsbConvertFilesToWav.Size = new System.Drawing.Size(24, 24);
            this.tsbConvertFilesToWav.Text = "Konvertovať súbory na .WAV";
            this.tsbConvertFilesToWav.Click += new System.EventHandler(this.DoConvertFilesToWav);
            // 
            // splitSoundsErrors
            // 
            this.splitSoundsErrors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitSoundsErrors.Location = new System.Drawing.Point(0, 0);
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
            this.dgvSounds.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSounds.Location = new System.Drawing.Point(0, 0);
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
            this.dgvSounds.Size = new System.Drawing.Size(785, 339);
            this.dgvSounds.TabIndex = 0;
            this.dgvSounds.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvSounds_CellBeginEdit);
            this.dgvSounds.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DgvSounds_CellMouseDown);
            this.dgvSounds.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvSounds_DataError);
            this.dgvSounds.RowValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSounds_RowValidated);
            this.dgvSounds.RowValidating += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvSounds_RowValidating);
            this.dgvSounds.SelectionChanged += new System.EventHandler(this.dgvSounds_SelectionChanged);
            this.dgvSounds.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DgvSounds_MouseDown);
            this.dgvSounds.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DgvSounds_MouseUp);
            // 
            // cSoundKey
            // 
            this.cSoundKey.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.cSoundKey.DataPropertyName = "Key";
            this.cSoundKey.HeaderText = "Kľúč zvuku";
            this.cSoundKey.MinimumWidth = 6;
            this.cSoundKey.Name = "cSoundKey";
            this.cSoundKey.Width = 80;
            // 
            // cSoundName
            // 
            this.cSoundName.DataPropertyName = "Name";
            this.cSoundName.HeaderText = "Názov zvuku";
            this.cSoundName.Name = "cSoundName";
            // 
            // cSoundAdditionalRelativePath
            // 
            this.cSoundAdditionalRelativePath.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.cSoundAdditionalRelativePath.DataPropertyName = "AdditionalRelativePath";
            this.cSoundAdditionalRelativePath.HeaderText = "Prídavná relatívna cesta";
            this.cSoundAdditionalRelativePath.MinimumWidth = 6;
            this.cSoundAdditionalRelativePath.Name = "cSoundAdditionalRelativePath";
            this.cSoundAdditionalRelativePath.Width = 95;
            // 
            // cSoundFileName
            // 
            this.cSoundFileName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.cSoundFileName.DataPropertyName = "FileName";
            this.cSoundFileName.HeaderText = "Názov súboru";
            this.cSoundFileName.MinimumWidth = 6;
            this.cSoundFileName.Name = "cSoundFileName";
            this.cSoundFileName.Width = 90;
            // 
            // cSoundDuration
            // 
            this.cSoundDuration.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.cSoundDuration.DataPropertyName = "DurationText";
            this.cSoundDuration.HeaderText = "Trvanie";
            this.cSoundDuration.MinimumWidth = 6;
            this.cSoundDuration.Name = "cSoundDuration";
            this.cSoundDuration.ReadOnly = true;
            this.cSoundDuration.Width = 68;
            // 
            // cSoundText
            // 
            this.cSoundText.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cSoundText.DataPropertyName = "Text";
            this.cSoundText.HeaderText = "Text hlásenia";
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
            this.contextMenuSounds.Size = new System.Drawing.Size(224, 140);
            // 
            // cmiAddSound
            // 
            this.cmiAddSound.Enabled = false;
            this.cmiAddSound.Image = global::ToolsCore.GlobalResources.add;
            this.cmiAddSound.Name = "cmiAddSound";
            this.cmiAddSound.Size = new System.Drawing.Size(223, 26);
            this.cmiAddSound.Text = "Pridať zvuk";
            this.cmiAddSound.Click += new System.EventHandler(this.DoAddSound);
            // 
            // cmiDeleteSound
            // 
            this.cmiDeleteSound.Enabled = false;
            this.cmiDeleteSound.Image = global::ToolsCore.GlobalResources.delete;
            this.cmiDeleteSound.Name = "cmiDeleteSound";
            this.cmiDeleteSound.Size = new System.Drawing.Size(223, 26);
            this.cmiDeleteSound.Text = "Odstrániť vybrané zvuky";
            this.cmiDeleteSound.Click += new System.EventHandler(this.DoDeleteSounds);
            // 
            // cmiMoveSounds
            // 
            this.cmiMoveSounds.Enabled = false;
            this.cmiMoveSounds.Image = global::ToolsCore.GlobalResources.move;
            this.cmiMoveSounds.Name = "cmiMoveSounds";
            this.cmiMoveSounds.Size = new System.Drawing.Size(223, 26);
            this.cmiMoveSounds.Text = "Presunúť vybrané zvuky...";
            this.cmiMoveSounds.Click += new System.EventHandler(this.DoMoveSounds);
            // 
            // toolStripSeparator19
            // 
            this.toolStripSeparator19.Name = "toolStripSeparator19";
            this.toolStripSeparator19.Size = new System.Drawing.Size(220, 6);
            // 
            // cmiConvertSoundsToEwa
            // 
            this.cmiConvertSoundsToEwa.Enabled = false;
            this.cmiConvertSoundsToEwa.Image = global::RawBankEditor.Properties.Resources.ewa;
            this.cmiConvertSoundsToEwa.Name = "cmiConvertSoundsToEwa";
            this.cmiConvertSoundsToEwa.Size = new System.Drawing.Size(223, 26);
            this.cmiConvertSoundsToEwa.Text = "Konvertovať zvuky na .EWA";
            this.cmiConvertSoundsToEwa.Click += new System.EventHandler(this.DoConvertSoundsToEwa);
            // 
            // cmiConvertSoundsToWav
            // 
            this.cmiConvertSoundsToWav.Enabled = false;
            this.cmiConvertSoundsToWav.Image = global::RawBankEditor.Properties.Resources.wav;
            this.cmiConvertSoundsToWav.Name = "cmiConvertSoundsToWav";
            this.cmiConvertSoundsToWav.Size = new System.Drawing.Size(223, 26);
            this.cmiConvertSoundsToWav.Text = "Konvertovať zvuky na .WAV";
            this.cmiConvertSoundsToWav.Click += new System.EventHandler(this.DoConvertSoundsToWav);
            // 
            // fyzSoundBindingSource
            // 
            this.fyzSoundBindingSource.DataSource = typeof(ToolsCore.Entities.FyzSound);
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
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 3;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(785, 157);
            this.tableLayoutPanel4.TabIndex = 0;
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
            this.toolStripErrors.Location = new System.Drawing.Point(3, 33);
            this.toolStripErrors.Margin = new System.Windows.Forms.Padding(2, 2, 2, 0);
            this.toolStripErrors.Name = "toolStripErrors";
            this.toolStripErrors.Size = new System.Drawing.Size(779, 27);
            this.toolStripErrors.TabIndex = 1;
            this.toolStripErrors.Text = "toolStrip2";
            // 
            // tsbErrors
            // 
            this.tsbErrors.Checked = true;
            this.tsbErrors.CheckOnClick = true;
            this.tsbErrors.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsbErrors.Image = ((System.Drawing.Image)(resources.GetObject("tsbErrors.Image")));
            this.tsbErrors.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbErrors.Name = "tsbErrors";
            this.tsbErrors.Size = new System.Drawing.Size(66, 24);
            this.tsbErrors.Text = "0 chýb";
            this.tsbErrors.CheckedChanged += new System.EventHandler(this.tsbErrors_CheckedChanged);
            // 
            // toolStripSeparator13
            // 
            this.toolStripSeparator13.Name = "toolStripSeparator13";
            this.toolStripSeparator13.Size = new System.Drawing.Size(6, 27);
            // 
            // tsbWarnings
            // 
            this.tsbWarnings.Checked = true;
            this.tsbWarnings.CheckOnClick = true;
            this.tsbWarnings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsbWarnings.Image = ((System.Drawing.Image)(resources.GetObject("tsbWarnings.Image")));
            this.tsbWarnings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbWarnings.Name = "tsbWarnings";
            this.tsbWarnings.Size = new System.Drawing.Size(100, 24);
            this.tsbWarnings.Text = "0 upozornení";
            this.tsbWarnings.CheckedChanged += new System.EventHandler(this.tsbWarnings_CheckedChanged);
            // 
            // toolStripSeparator14
            // 
            this.toolStripSeparator14.Name = "toolStripSeparator14";
            this.toolStripSeparator14.Size = new System.Drawing.Size(6, 27);
            // 
            // tsbInfos
            // 
            this.tsbInfos.Checked = true;
            this.tsbInfos.CheckOnClick = true;
            this.tsbInfos.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsbInfos.Image = ((System.Drawing.Image)(resources.GetObject("tsbInfos.Image")));
            this.tsbInfos.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbInfos.Name = "tsbInfos";
            this.tsbInfos.Size = new System.Drawing.Size(68, 24);
            this.tsbInfos.Text = "0 správ";
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
            this.tsbResolveProblem.Size = new System.Drawing.Size(24, 24);
            this.tsbResolveProblem.Text = "Vyriešiť";
            this.tsbResolveProblem.Click += new System.EventHandler(this.DoSolveProblem);
            // 
            // tsbHighlightProblem
            // 
            this.tsbHighlightProblem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsbHighlightProblem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbHighlightProblem.Enabled = false;
            this.tsbHighlightProblem.Image = global::ToolsCore.GlobalResources.analyze;
            this.tsbHighlightProblem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHighlightProblem.Name = "tsbHighlightProblem";
            this.tsbHighlightProblem.Size = new System.Drawing.Size(24, 24);
            this.tsbHighlightProblem.Text = "Zvýrazniť problém";
            this.tsbHighlightProblem.Click += new System.EventHandler(this.DoFindProblem);
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
            this.dgvErrors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvErrors.Location = new System.Drawing.Point(3, 63);
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
            this.dgvErrors.Size = new System.Drawing.Size(779, 91);
            this.dgvErrors.TabIndex = 2;
            this.dgvErrors.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvErrors_CellDoubleClick);
            this.dgvErrors.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvErrors_CellFormatting);
            this.dgvErrors.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DgvErrors_CellMouseDown);
            // 
            // cMsgType
            // 
            this.cMsgType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.cMsgType.HeaderText = "Typ";
            this.cMsgType.MinimumWidth = 6;
            this.cMsgType.Name = "cMsgType";
            this.cMsgType.ReadOnly = true;
            this.cMsgType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.cMsgType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.cMsgType.Width = 50;
            // 
            // cMsgCode
            // 
            this.cMsgCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.cMsgCode.DataPropertyName = "Code";
            this.cMsgCode.HeaderText = "Kód";
            this.cMsgCode.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.cMsgCode.Name = "cMsgCode";
            this.cMsgCode.ReadOnly = true;
            this.cMsgCode.Width = 32;
            // 
            // cMessage
            // 
            this.cMessage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.cMessage.DataPropertyName = "Message";
            this.cMessage.HeaderText = "Správa";
            this.cMessage.MinimumWidth = 450;
            this.cMessage.Name = "cMessage";
            this.cMessage.ReadOnly = true;
            this.cMessage.Width = 450;
            // 
            // cMsgResolve
            // 
            this.cMsgResolve.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.cMsgResolve.DataPropertyName = "ResolveMessage";
            this.cMsgResolve.HeaderText = "Riešenie";
            this.cMsgResolve.MinimumWidth = 150;
            this.cMsgResolve.Name = "cMsgResolve";
            this.cMsgResolve.ReadOnly = true;
            this.cMsgResolve.Width = 150;
            // 
            // cMsgPath
            // 
            this.cMsgPath.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cMsgPath.DataPropertyName = "Path";
            this.cMsgPath.HeaderText = "Cesta";
            this.cMsgPath.Name = "cMsgPath";
            this.cMsgPath.ReadOnly = true;
            // 
            // contextMenuErrors
            // 
            this.contextMenuErrors.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmiHighlightProblem,
            this.cmiResolveProblem});
            this.contextMenuErrors.Name = "contextMenuErrors";
            this.contextMenuErrors.Size = new System.Drawing.Size(160, 48);
            // 
            // cmiHighlightProblem
            // 
            this.cmiHighlightProblem.Enabled = false;
            this.cmiHighlightProblem.Image = global::ToolsCore.GlobalResources.analyze;
            this.cmiHighlightProblem.Name = "cmiHighlightProblem";
            this.cmiHighlightProblem.Size = new System.Drawing.Size(159, 22);
            this.cmiHighlightProblem.Text = "Zvýrazniť chybu";
            this.cmiHighlightProblem.Click += new System.EventHandler(this.DoFindProblem);
            // 
            // cmiResolveProblem
            // 
            this.cmiResolveProblem.Enabled = false;
            this.cmiResolveProblem.Image = global::ToolsCore.GlobalResources.correct;
            this.cmiResolveProblem.Name = "cmiResolveProblem";
            this.cmiResolveProblem.Size = new System.Drawing.Size(159, 22);
            this.cmiResolveProblem.Text = "Vyriešiť";
            this.cmiResolveProblem.Click += new System.EventHandler(this.DoSolveProblem);
            // 
            // rawBankMessageBindingSource
            // 
            this.rawBankMessageBindingSource.DataSource = typeof(RawBankEditor.Entities.IRawBankMessage);
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.AutoSize = true;
            this.tableLayoutPanel5.BorderColor = System.Drawing.Color.Empty;
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Controls.Add(this.mmErrors, 1, 0);
            this.tableLayoutPanel5.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(779, 25);
            this.tableLayoutPanel5.TabIndex = 3;
            // 
            // mmErrors
            // 
            this.mmErrors.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mmErrors.HoverColor = System.Drawing.SystemColors.Highlight;
            this.mmErrors.IconColor = System.Drawing.SystemColors.ControlDark;
            this.mmErrors.IconHoverColor = System.Drawing.Color.White;
            this.mmErrors.Location = new System.Drawing.Point(765, 2);
            this.mmErrors.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.mmErrors.Name = "mmErrors";
            this.mmErrors.Size = new System.Drawing.Size(12, 13);
            this.mmErrors.TabIndex = 1;
            this.mmErrors.ToolTipText = "Zavrieť";
            this.mmErrors.Click += new System.EventHandler(this.mmErrors_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(2, 2);
            this.label3.Margin = new System.Windows.Forms.Padding(2);
            this.label3.Name = "label3";
            this.label3.Padding = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.label3.Size = new System.Drawing.Size(385, 21);
            this.label3.TabIndex = 1;
            this.label3.Text = "Zoznam chýb";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BorderColor = System.Drawing.Color.Empty;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.splitContainer1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.statusStripMain, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 51);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1095, 532);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // statusStripMain
            // 
            this.statusStripMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslStatus,
            this.tspbProgress,
            this.toolStripStatusLabel1,
            this.tssbErrors});
            this.statusStripMain.Location = new System.Drawing.Point(0, 506);
            this.statusStripMain.Name = "statusStripMain";
            this.statusStripMain.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
            this.statusStripMain.Size = new System.Drawing.Size(1095, 26);
            this.statusStripMain.SizingGrip = false;
            this.statusStripMain.TabIndex = 2;
            this.statusStripMain.Text = "statusStrip1";
            // 
            // tsslStatus
            // 
            this.tsslStatus.Name = "tsslStatus";
            this.tsslStatus.Size = new System.Drawing.Size(141, 21);
            this.tsslStatus.Text = "Neotvorená žiadna banka";
            // 
            // tspbProgress
            // 
            this.tspbProgress.Margin = new System.Windows.Forms.Padding(5, 4, 1, 4);
            this.tspbProgress.Name = "tspbProgress";
            this.tspbProgress.Size = new System.Drawing.Size(75, 18);
            this.tspbProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.tspbProgress.Visible = false;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(807, 21);
            this.toolStripStatusLabel1.Spring = true;
            // 
            // tssbErrors
            // 
            this.tssbErrors.Image = global::ToolsCore.GlobalResources.correct;
            this.tssbErrors.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tssbErrors.Name = "tssbErrors";
            this.tssbErrors.ShowDropDownArrow = false;
            this.tssbErrors.Size = new System.Drawing.Size(24, 24);
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
            this.toolStripMain.Location = new System.Drawing.Point(0, 24);
            this.toolStripMain.Name = "toolStripMain";
            this.toolStripMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolStripMain.Size = new System.Drawing.Size(1095, 27);
            this.toolStripMain.TabIndex = 1;
            this.toolStripMain.Text = "toolStrip1";
            // 
            // tsbOpen
            // 
            this.tsbOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbOpen.Image = global::ToolsCore.GlobalResources.open;
            this.tsbOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbOpen.Name = "tsbOpen";
            this.tsbOpen.Size = new System.Drawing.Size(24, 24);
            this.tsbOpen.Text = "Otvoriť";
            this.tsbOpen.Click += new System.EventHandler(this.DoOpenDir);
            // 
            // tsbRecent
            // 
            this.tsbRecent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRecent.Image = global::ToolsCore.GlobalResources.recent_gvds;
            this.tsbRecent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRecent.Name = "tsbRecent";
            this.tsbRecent.Size = new System.Drawing.Size(33, 24);
            this.tsbRecent.Text = "Nedávne";
            // 
            // toolStripSeparator25
            // 
            this.toolStripSeparator25.Name = "toolStripSeparator25";
            this.toolStripSeparator25.Size = new System.Drawing.Size(6, 27);
            // 
            // tsbSave
            // 
            this.tsbSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSave.Enabled = false;
            this.tsbSave.Image = global::ToolsCore.GlobalResources.save;
            this.tsbSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSave.Name = "tsbSave";
            this.tsbSave.Size = new System.Drawing.Size(24, 24);
            this.tsbSave.Text = "Uložiť";
            this.tsbSave.Click += new System.EventHandler(this.DoSave);
            // 
            // tsbSaveAll
            // 
            this.tsbSaveAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSaveAll.Enabled = false;
            this.tsbSaveAll.Image = global::ToolsCore.GlobalResources.save_all;
            this.tsbSaveAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSaveAll.Name = "tsbSaveAll";
            this.tsbSaveAll.Size = new System.Drawing.Size(24, 24);
            this.tsbSaveAll.Text = "Uložiť všetko";
            this.tsbSaveAll.Click += new System.EventHandler(this.DoSaveAll);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // tsbGoBack
            // 
            this.tsbGoBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbGoBack.DropDownButtonWidth = 15;
            this.tsbGoBack.Enabled = false;
            this.tsbGoBack.Image = global::ToolsCore.GlobalResources.back;
            this.tsbGoBack.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbGoBack.Name = "tsbGoBack";
            this.tsbGoBack.Size = new System.Drawing.Size(40, 24);
            this.tsbGoBack.Text = "Prejsť späť";
            this.tsbGoBack.ButtonClick += new System.EventHandler(this.DoGoBack);
            // 
            // tsbGoForward
            // 
            this.tsbGoForward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbGoForward.Enabled = false;
            this.tsbGoForward.Image = global::ToolsCore.GlobalResources.forward;
            this.tsbGoForward.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbGoForward.Name = "tsbGoForward";
            this.tsbGoForward.Size = new System.Drawing.Size(24, 24);
            this.tsbGoForward.Text = "Prejsť dopredu";
            this.tsbGoForward.Click += new System.EventHandler(this.DoGoForward);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(6, 27);
            // 
            // tsbUndo
            // 
            this.tsbUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbUndo.Enabled = false;
            this.tsbUndo.Image = global::ToolsCore.GlobalResources.undo;
            this.tsbUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbUndo.Name = "tsbUndo";
            this.tsbUndo.Size = new System.Drawing.Size(24, 24);
            this.tsbUndo.Text = "Späť";
            this.tsbUndo.Click += new System.EventHandler(this.DoUndo);
            // 
            // tsbRedo
            // 
            this.tsbRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbRedo.Enabled = false;
            this.tsbRedo.Image = global::ToolsCore.GlobalResources.redo;
            this.tsbRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRedo.Name = "tsbRedo";
            this.tsbRedo.Size = new System.Drawing.Size(24, 24);
            this.tsbRedo.Text = "Znovu";
            this.tsbRedo.Click += new System.EventHandler(this.DoRedo);
            // 
            // toolStripSeparator10
            // 
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            this.toolStripSeparator10.Size = new System.Drawing.Size(6, 27);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(37, 24);
            this.toolStripLabel1.Text = "Jazyk:";
            // 
            // tscboxLanguages
            // 
            this.tscboxLanguages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscboxLanguages.DropDownWidth = 150;
            this.tscboxLanguages.Enabled = false;
            this.tscboxLanguages.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.tscboxLanguages.Name = "tscboxLanguages";
            this.tscboxLanguages.Size = new System.Drawing.Size(130, 24);
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
            this.tsbLangsSettings.Size = new System.Drawing.Size(33, 24);
            this.tsbLangsSettings.Text = "Nastavenie jazykov";
            // 
            // tsmiAddLanguage
            // 
            this.tsmiAddLanguage.Enabled = false;
            this.tsmiAddLanguage.Image = global::ToolsCore.GlobalResources.add;
            this.tsmiAddLanguage.Name = "tsmiAddLanguage";
            this.tsmiAddLanguage.Size = new System.Drawing.Size(264, 22);
            this.tsmiAddLanguage.Text = "Pridať jazyk";
            this.tsmiAddLanguage.Click += new System.EventHandler(this.DoAddLanguage);
            // 
            // tsmiEditLanguage
            // 
            this.tsmiEditLanguage.Enabled = false;
            this.tsmiEditLanguage.Image = global::ToolsCore.GlobalResources.edit;
            this.tsmiEditLanguage.Name = "tsmiEditLanguage";
            this.tsmiEditLanguage.Size = new System.Drawing.Size(264, 22);
            this.tsmiEditLanguage.Text = "Upraviť jazyk";
            this.tsmiEditLanguage.Click += new System.EventHandler(this.DoEditLanguage);
            // 
            // tsmiDeleteLanguage
            // 
            this.tsmiDeleteLanguage.Enabled = false;
            this.tsmiDeleteLanguage.Image = global::ToolsCore.GlobalResources.delete;
            this.tsmiDeleteLanguage.Name = "tsmiDeleteLanguage";
            this.tsmiDeleteLanguage.Size = new System.Drawing.Size(264, 22);
            this.tsmiDeleteLanguage.Text = "Odstrániť jazyk";
            this.tsmiDeleteLanguage.Click += new System.EventHandler(this.DoDeleteLanguage);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(261, 6);
            // 
            // tsmiConvertLangToEwa
            // 
            this.tsmiConvertLangToEwa.Enabled = false;
            this.tsmiConvertLangToEwa.Image = global::RawBankEditor.Properties.Resources.ewa;
            this.tsmiConvertLangToEwa.Name = "tsmiConvertLangToEwa";
            this.tsmiConvertLangToEwa.Size = new System.Drawing.Size(264, 22);
            this.tsmiConvertLangToEwa.Text = "Konvertovať zvuky v jazyku na .EWA";
            this.tsmiConvertLangToEwa.Click += new System.EventHandler(this.DoConvertLangToEwa);
            // 
            // tsmiConvertLangToWav
            // 
            this.tsmiConvertLangToWav.Enabled = false;
            this.tsmiConvertLangToWav.Image = global::RawBankEditor.Properties.Resources.wav;
            this.tsmiConvertLangToWav.Name = "tsmiConvertLangToWav";
            this.tsmiConvertLangToWav.Size = new System.Drawing.Size(264, 22);
            this.tsmiConvertLangToWav.Text = "Konvertovať zvuky v jazyku na .WAV";
            this.tsmiConvertLangToWav.Click += new System.EventHandler(this.DoConvertLangToWav);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 27);
            // 
            // tsbAppSettings
            // 
            this.tsbAppSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAppSettings.Image = global::ToolsCore.GlobalResources.app_settings;
            this.tsbAppSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAppSettings.Name = "tsbAppSettings";
            this.tsbAppSettings.Size = new System.Drawing.Size(24, 24);
            this.tsbAppSettings.Text = "Nastavenia programu";
            this.tsbAppSettings.Click += new System.EventHandler(this.ShowAppSettings);
            // 
            // tsbInfoApp
            // 
            this.tsbInfoApp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbInfoApp.Image = global::ToolsCore.GlobalResources.info_app;
            this.tsbInfoApp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbInfoApp.Name = "tsbInfoApp";
            this.tsbInfoApp.Size = new System.Drawing.Size(24, 24);
            this.tsbInfoApp.Text = "Informácie o programe";
            this.tsbInfoApp.Click += new System.EventHandler(this.ShowInfoApp);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(6, 27);
            // 
            // tsbAddSound
            // 
            this.tsbAddSound.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbAddSound.Enabled = false;
            this.tsbAddSound.Image = global::ToolsCore.GlobalResources.add;
            this.tsbAddSound.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbAddSound.Name = "tsbAddSound";
            this.tsbAddSound.Size = new System.Drawing.Size(24, 24);
            this.tsbAddSound.Text = "Pridať zvuk";
            this.tsbAddSound.Click += new System.EventHandler(this.DoAddSound);
            // 
            // tsbMoveSounds
            // 
            this.tsbMoveSounds.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbMoveSounds.Enabled = false;
            this.tsbMoveSounds.Image = global::ToolsCore.GlobalResources.move;
            this.tsbMoveSounds.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbMoveSounds.Name = "tsbMoveSounds";
            this.tsbMoveSounds.Size = new System.Drawing.Size(24, 24);
            this.tsbMoveSounds.Text = "Premiestniť zvuky";
            this.tsbMoveSounds.Click += new System.EventHandler(this.DoMoveSounds);
            // 
            // tsbDeleteSound
            // 
            this.tsbDeleteSound.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbDeleteSound.Enabled = false;
            this.tsbDeleteSound.Image = global::ToolsCore.GlobalResources.delete;
            this.tsbDeleteSound.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbDeleteSound.Name = "tsbDeleteSound";
            this.tsbDeleteSound.Size = new System.Drawing.Size(24, 24);
            this.tsbDeleteSound.Text = "Odstrániť zvuk";
            this.tsbDeleteSound.Click += new System.EventHandler(this.DoDeleteSounds);
            // 
            // toolStripSeparator24
            // 
            this.toolStripSeparator24.Name = "toolStripSeparator24";
            this.toolStripSeparator24.Size = new System.Drawing.Size(6, 27);
            // 
            // tsbConvertSoundsToEwa
            // 
            this.tsbConvertSoundsToEwa.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbConvertSoundsToEwa.Enabled = false;
            this.tsbConvertSoundsToEwa.Image = global::RawBankEditor.Properties.Resources.ewa;
            this.tsbConvertSoundsToEwa.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbConvertSoundsToEwa.Name = "tsbConvertSoundsToEwa";
            this.tsbConvertSoundsToEwa.Size = new System.Drawing.Size(24, 24);
            this.tsbConvertSoundsToEwa.Text = "Konvertovať vybrané zvuky na .EWA";
            this.tsbConvertSoundsToEwa.Click += new System.EventHandler(this.DoConvertSoundsToEwa);
            // 
            // tsbConvertSoundsToWav
            // 
            this.tsbConvertSoundsToWav.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbConvertSoundsToWav.Enabled = false;
            this.tsbConvertSoundsToWav.Image = global::RawBankEditor.Properties.Resources.wav;
            this.tsbConvertSoundsToWav.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbConvertSoundsToWav.Name = "tsbConvertSoundsToWav";
            this.tsbConvertSoundsToWav.Size = new System.Drawing.Size(24, 24);
            this.tsbConvertSoundsToWav.Text = "Konvertovať vybrané zvuky na .WAV";
            this.tsbConvertSoundsToWav.Click += new System.EventHandler(this.DoConvertSoundsToWav);
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            this.toolStripSeparator15.Size = new System.Drawing.Size(6, 27);
            // 
            // tsbSearch
            // 
            this.tsbSearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbSearch.Enabled = false;
            this.tsbSearch.Image = global::ToolsCore.GlobalResources.search;
            this.tsbSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbSearch.Name = "tsbSearch";
            this.tsbSearch.Size = new System.Drawing.Size(24, 24);
            this.tsbSearch.Text = "Hľadať";
            this.tsbSearch.Click += new System.EventHandler(this.DoSearch);
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
            this.tsbWrapTextSoundCol.Size = new System.Drawing.Size(24, 24);
            this.tsbWrapTextSoundCol.Text = "Prispôsobiť text hlásenia v bunkách";
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
            this.tsbRewriteMode.Size = new System.Drawing.Size(24, 24);
            this.tsbRewriteMode.Text = "Prepisovací mód";
            this.tsbRewriteMode.CheckedChanged += new System.EventHandler(this.RewriteModeChanged);
            // 
            // undoActionChooser
            // 
            this.undoActionChooser.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.undoActionChooser.Name = "undoActionChooser";
            this.undoActionChooser.Size = new System.Drawing.Size(2, 31);
            // 
            // bWorkerReadDat
            // 
            this.bWorkerReadDat.WorkerReportsProgress = true;
            this.bWorkerReadDat.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bWorkerReadDat_DoWork);
            this.bWorkerReadDat.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bWorkerReadDat_ProgressChanged);
            this.bWorkerReadDat.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bWorkerReadDat_RunWorkerCompleted);
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
            this.ClientSize = new System.Drawing.Size(1095, 583);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.toolStripMain);
            this.Controls.Add(this.menuStripMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStripMain;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FMain";
            this.Text = "RawBankEditor";
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
        private System.ComponentModel.BackgroundWorker bWorkerReadDat;
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

