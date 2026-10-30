namespace RawBankEditor.Forms
{
    partial class FAppSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FAppSettings));
            ExControls.OptionsNode optionsNodeSpeech = new ExControls.OptionsNode();
            this.pSpeech = new ExControls.ExOptionsPanel(this.optionsView);
            ExControls.OptionsNode optionsNodePiper = new ExControls.OptionsNode();
            this.pPiper = new ExControls.ExOptionsPanel(this.optionsView);
            this.ppPiper = new System.Windows.Forms.Panel();
            this.gbPiper = new ExControls.ExGroupBox();
            this.lPiperPath = new System.Windows.Forms.Label();
            this.tbPiperPath = new ExControls.ExTextBox();
            this.bPiperBrowse = new ExControls.ExButton();
            this.bPiperVerify = new ExControls.ExButton();
            this.lPiperVoices = new System.Windows.Forms.Label();
            this.tbPiperVoices = new ExControls.ExTextBox();
            this.bPiperVoicesBrowse = new ExControls.ExButton();
            this.bPiperDownload = new ExControls.ExButton();
            this.lPiperStatus = new System.Windows.Forms.Label();
            this.llPiper = new System.Windows.Forms.LinkLabel();
            this.llPiperSamples = new System.Windows.Forms.LinkLabel();
            this.lPiperInfo = new System.Windows.Forms.Label();
            ExControls.OptionsNode optionsNodeAzure = new ExControls.OptionsNode();
            this.pAzure = new ExControls.ExOptionsPanel(this.optionsView);
            this.ppAzure = new System.Windows.Forms.Panel();
            this.gbAzure = new ExControls.ExGroupBox();
            this.lAzureKey = new System.Windows.Forms.Label();
            this.tbAzureKey = new ExControls.ExTextBox();
            this.bAzureVerify = new ExControls.ExButton();
            this.lAzureRegion = new System.Windows.Forms.Label();
            this.tbAzureRegion = new ExControls.ExTextBox();
            this.lAzureStatus = new System.Windows.Forms.Label();
            this.llAzure = new System.Windows.Forms.LinkLabel();
            this.lAzureInfo = new System.Windows.Forms.Label();
            this.ppSpeech = new System.Windows.Forms.Panel();
            this.gbSpeechKey = new ExControls.ExGroupBox();
            this.lSpeechKey = new System.Windows.Forms.Label();
            this.tbSpeechKey = new ExControls.ExTextBox();
            this.bSpeechVerify = new ExControls.ExButton();
            this.lSpeechStatus = new System.Windows.Forms.Label();
            this.llSpeechKey = new System.Windows.Forms.LinkLabel();
            this.lSpeechConcurrency = new System.Windows.Forms.Label();
            this.nudSpeechConcurrency = new ExControls.ExNumericUpDown();
            this.gbSpeechCache = new ExControls.ExGroupBox();
            this.lSpeechCache = new System.Windows.Forms.Label();
            this.bSpeechClearCache = new ExControls.ExButton();
            this.lSpeechPrivacy = new System.Windows.Forms.Label();
            this.exGroupBox2 = new ExControls.ExGroupBox();
            this.cboxAutoRecalculateSoundDurations = new ExControls.ExCheckBox();
            this.cboxAutoInsertSoundData = new ExControls.ExCheckBox();
            this.cboxShowAfterInsertSoundDlg = new ExControls.ExCheckBox();
            this.pGeneral.SuspendLayout();
            this.pConcreteGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.optionsView)).BeginInit();
            this.exGroupBox2.SuspendLayout();
            this.pSpeech.SuspendLayout();
            this.pPiper.SuspendLayout();
            this.ppPiper.SuspendLayout();
            this.gbPiper.SuspendLayout();
            this.pAzure.SuspendLayout();
            this.ppAzure.SuspendLayout();
            this.gbAzure.SuspendLayout();
            this.ppSpeech.SuspendLayout();
            this.gbSpeechKey.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSpeechConcurrency)).BeginInit();
            this.gbSpeechCache.SuspendLayout();
            this.SuspendLayout();
            // 
            // pConcreteGeneral
            // 
            this.pConcreteGeneral.Controls.Add(this.exGroupBox2);
            resources.ApplyResources(this.pConcreteGeneral, "pConcreteGeneral");
            // 
            // optionsView
            // 
            this.optionsView.HeaderNodeNameVisible = true;
            this.optionsView.Panels.Add(this.pSpeech);
            this.optionsView.Panels.Add(this.pAzure);
            this.optionsView.Panels.Add(this.pPiper);
            this.optionsView.SearchBoxVisible = true;
            resources.ApplyResources(this.optionsView, "optionsView");
            // 
            // 
            // 
            this.optionsView.TreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.optionsView.TreeView.FullRowSelect = true;
            this.optionsView.TreeView.HideSelection = false;
            this.optionsView.TreeView.ItemHeight = 20;
            this.optionsView.TreeView.Name = "treeView";
            this.optionsView.TreeView.PathSeparator = " / ";
            this.optionsView.TreeView.ShowLines = false;
            this.optionsView.TreeView.ShowNodeToolTips = true;
            this.optionsView.TreeView.Style = ExControls.ExTreeViewStyle.Light;
            this.optionsView.TreeView.TabIndex = 0;
            // 
            // exGroupBox2
            // 
            resources.ApplyResources(this.exGroupBox2, "exGroupBox2");
            this.exGroupBox2.Controls.Add(this.cboxShowAfterInsertSoundDlg);
            this.exGroupBox2.Controls.Add(this.cboxAutoInsertSoundData);
            this.exGroupBox2.Controls.Add(this.cboxAutoRecalculateSoundDurations);
            this.exGroupBox2.Name = "exGroupBox2";
            this.exGroupBox2.TabStop = false;
            // 
            // cboxAutoRecalculateSoundDurations
            // 
            resources.ApplyResources(this.cboxAutoRecalculateSoundDurations, "cboxAutoRecalculateSoundDurations");
            this.cboxAutoRecalculateSoundDurations.BoxBackColor = System.Drawing.Color.White;
            this.cboxAutoRecalculateSoundDurations.HighlightColor = System.Drawing.SystemColors.Highlight;
            this.cboxAutoRecalculateSoundDurations.Name = "cboxAutoRecalculateSoundDurations";
            // 
            // cboxAutoInsertSoundData
            // 
            resources.ApplyResources(this.cboxAutoInsertSoundData, "cboxAutoInsertSoundData");
            this.cboxAutoInsertSoundData.BoxBackColor = System.Drawing.Color.White;
            this.cboxAutoInsertSoundData.HighlightColor = System.Drawing.SystemColors.Highlight;
            this.cboxAutoInsertSoundData.Name = "cboxAutoInsertSoundData";
            this.cboxAutoInsertSoundData.CheckedChanged += new System.EventHandler(this.CboxAutoInsertSoundData_CheckedChanged);
            // 
            // cboxShowAfterInsertSoundDlg
            // 
            resources.ApplyResources(this.cboxShowAfterInsertSoundDlg, "cboxShowAfterInsertSoundDlg");
            this.cboxShowAfterInsertSoundDlg.BoxBackColor = System.Drawing.Color.White;
            this.cboxShowAfterInsertSoundDlg.HighlightColor = System.Drawing.SystemColors.Highlight;
            this.cboxShowAfterInsertSoundDlg.Name = "cboxShowAfterInsertSoundDlg";
            // 
            // pSpeech
            // 
            this.pSpeech.Controls.Add(this.ppSpeech);
            this.pSpeech.Name = "pSpeech";
            optionsNodeSpeech.Name = "";
            optionsNodeSpeech.Text = resources.GetString("optionsNodeSpeech.Text");
            this.pSpeech.Node = optionsNodeSpeech;
            this.pSpeech.NodeText = resources.GetString("pSpeech.NodeText");
            this.pSpeech.ParentNode = null;
            // 
            // ppSpeech
            // 
            this.ppSpeech.Controls.Add(this.gbSpeechKey);
            this.ppSpeech.Controls.Add(this.gbSpeechCache);
            this.ppSpeech.Controls.Add(this.lSpeechPrivacy);
            resources.ApplyResources(this.ppSpeech, "ppSpeech");
            this.ppSpeech.Name = "ppSpeech";
            // 
            // gbSpeechKey
            // 
            this.gbSpeechKey.Controls.Add(this.lSpeechKey);
            this.gbSpeechKey.Controls.Add(this.tbSpeechKey);
            this.gbSpeechKey.Controls.Add(this.bSpeechVerify);
            this.gbSpeechKey.Controls.Add(this.lSpeechStatus);
            this.gbSpeechKey.Controls.Add(this.llSpeechKey);
            this.gbSpeechKey.Controls.Add(this.lSpeechConcurrency);
            this.gbSpeechKey.Controls.Add(this.nudSpeechConcurrency);
            resources.ApplyResources(this.gbSpeechKey, "gbSpeechKey");
            this.gbSpeechKey.TabStop = false;
            this.gbSpeechKey.Name = "gbSpeechKey";
            // 
            // lSpeechKey
            // 
            resources.ApplyResources(this.lSpeechKey, "lSpeechKey");
            this.lSpeechKey.Name = "lSpeechKey";
            // 
            // tbSpeechKey
            // 
            resources.ApplyResources(this.tbSpeechKey, "tbSpeechKey");
            this.tbSpeechKey.UseSystemPasswordChar = true;
            this.tbSpeechKey.Name = "tbSpeechKey";
            // 
            // bSpeechVerify
            // 
            resources.ApplyResources(this.bSpeechVerify, "bSpeechVerify");
            this.bSpeechVerify.UseVisualStyleBackColor = true;
            this.bSpeechVerify.Name = "bSpeechVerify";
            this.bSpeechVerify.Click += new System.EventHandler(this.BSpeechVerify_Click);
            // 
            // lSpeechStatus
            // 
            resources.ApplyResources(this.lSpeechStatus, "lSpeechStatus");
            this.lSpeechStatus.Name = "lSpeechStatus";
            // 
            // llSpeechKey
            // 
            resources.ApplyResources(this.llSpeechKey, "llSpeechKey");
            this.llSpeechKey.TabStop = true;
            this.llSpeechKey.Name = "llSpeechKey";
            this.llSpeechKey.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LlSpeechKey_LinkClicked);
            // 
            // lSpeechConcurrency
            // 
            resources.ApplyResources(this.lSpeechConcurrency, "lSpeechConcurrency");
            this.lSpeechConcurrency.Name = "lSpeechConcurrency";
            // 
            // nudSpeechConcurrency
            // 
            resources.ApplyResources(this.nudSpeechConcurrency, "nudSpeechConcurrency");
            this.nudSpeechConcurrency.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudSpeechConcurrency.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSpeechConcurrency.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudSpeechConcurrency.Name = "nudSpeechConcurrency";
            // 
            // gbSpeechCache
            // 
            this.gbSpeechCache.Controls.Add(this.lSpeechCache);
            this.gbSpeechCache.Controls.Add(this.bSpeechClearCache);
            resources.ApplyResources(this.gbSpeechCache, "gbSpeechCache");
            this.gbSpeechCache.TabStop = false;
            this.gbSpeechCache.Name = "gbSpeechCache";
            // 
            // lSpeechCache
            // 
            resources.ApplyResources(this.lSpeechCache, "lSpeechCache");
            this.lSpeechCache.Name = "lSpeechCache";
            // 
            // bSpeechClearCache
            // 
            resources.ApplyResources(this.bSpeechClearCache, "bSpeechClearCache");
            this.bSpeechClearCache.UseVisualStyleBackColor = true;
            this.bSpeechClearCache.Name = "bSpeechClearCache";
            this.bSpeechClearCache.Click += new System.EventHandler(this.BSpeechClearCache_Click);
            // 
            // lSpeechPrivacy
            // 
            resources.ApplyResources(this.lSpeechPrivacy, "lSpeechPrivacy");
            this.lSpeechPrivacy.Name = "lSpeechPrivacy";
            // 
            // pAzure
            // 
            this.pAzure.Controls.Add(this.ppAzure);
            this.pAzure.Name = "pAzure";
            optionsNodeAzure.Name = "";
            optionsNodeAzure.Text = resources.GetString("optionsNodeAzure.Text");
            this.pAzure.Node = optionsNodeAzure;
            this.pAzure.NodeText = resources.GetString("pAzure.NodeText");
            this.pAzure.ParentNode = optionsNodeSpeech;
            // 
            // ppAzure
            // 
            this.ppAzure.Controls.Add(this.gbAzure);
            this.ppAzure.Controls.Add(this.lAzureInfo);
            resources.ApplyResources(this.ppAzure, "ppAzure");
            this.ppAzure.Name = "ppAzure";
            // 
            // gbAzure
            // 
            this.gbAzure.Controls.Add(this.lAzureKey);
            this.gbAzure.Controls.Add(this.tbAzureKey);
            this.gbAzure.Controls.Add(this.bAzureVerify);
            this.gbAzure.Controls.Add(this.lAzureRegion);
            this.gbAzure.Controls.Add(this.tbAzureRegion);
            this.gbAzure.Controls.Add(this.lAzureStatus);
            this.gbAzure.Controls.Add(this.llAzure);
            resources.ApplyResources(this.gbAzure, "gbAzure");
            this.gbAzure.TabStop = false;
            this.gbAzure.Name = "gbAzure";
            // 
            // lAzureKey
            // 
            resources.ApplyResources(this.lAzureKey, "lAzureKey");
            this.lAzureKey.Name = "lAzureKey";
            // 
            // tbAzureKey
            // 
            resources.ApplyResources(this.tbAzureKey, "tbAzureKey");
            this.tbAzureKey.UseSystemPasswordChar = true;
            this.tbAzureKey.Name = "tbAzureKey";
            // 
            // bAzureVerify
            // 
            resources.ApplyResources(this.bAzureVerify, "bAzureVerify");
            this.bAzureVerify.UseVisualStyleBackColor = true;
            this.bAzureVerify.Name = "bAzureVerify";
            this.bAzureVerify.Click += new System.EventHandler(this.BAzureVerify_Click);
            // 
            // lAzureRegion
            // 
            resources.ApplyResources(this.lAzureRegion, "lAzureRegion");
            this.lAzureRegion.Name = "lAzureRegion";
            // 
            // tbAzureRegion
            // 
            resources.ApplyResources(this.tbAzureRegion, "tbAzureRegion");
            this.tbAzureRegion.Name = "tbAzureRegion";
            // 
            // lAzureStatus
            // 
            resources.ApplyResources(this.lAzureStatus, "lAzureStatus");
            this.lAzureStatus.Name = "lAzureStatus";
            // 
            // llAzure
            // 
            resources.ApplyResources(this.llAzure, "llAzure");
            this.llAzure.TabStop = true;
            this.llAzure.Name = "llAzure";
            this.llAzure.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LlAzure_LinkClicked);
            // 
            // lAzureInfo
            // 
            resources.ApplyResources(this.lAzureInfo, "lAzureInfo");
            this.lAzureInfo.Name = "lAzureInfo";
            // 
            // pPiper
            // 
            this.pPiper.Controls.Add(this.ppPiper);
            this.pPiper.Name = "pPiper";
            optionsNodePiper.Name = "";
            optionsNodePiper.Text = resources.GetString("optionsNodePiper.Text");
            this.pPiper.Node = optionsNodePiper;
            this.pPiper.NodeText = resources.GetString("pPiper.NodeText");
            this.pPiper.ParentNode = optionsNodeSpeech;
            // 
            // ppPiper
            // 
            this.ppPiper.Controls.Add(this.gbPiper);
            this.ppPiper.Controls.Add(this.lPiperInfo);
            resources.ApplyResources(this.ppPiper, "ppPiper");
            this.ppPiper.Name = "ppPiper";
            // 
            // gbPiper
            // 
            this.gbPiper.Controls.Add(this.lPiperPath);
            this.gbPiper.Controls.Add(this.tbPiperPath);
            this.gbPiper.Controls.Add(this.bPiperBrowse);
            this.gbPiper.Controls.Add(this.bPiperVerify);
            this.gbPiper.Controls.Add(this.lPiperVoices);
            this.gbPiper.Controls.Add(this.tbPiperVoices);
            this.gbPiper.Controls.Add(this.bPiperVoicesBrowse);
            this.gbPiper.Controls.Add(this.bPiperDownload);
            this.gbPiper.Controls.Add(this.lPiperStatus);
            this.gbPiper.Controls.Add(this.llPiper);
            this.gbPiper.Controls.Add(this.llPiperSamples);
            resources.ApplyResources(this.gbPiper, "gbPiper");
            this.gbPiper.TabStop = false;
            this.gbPiper.Name = "gbPiper";
            // 
            // lPiperPath
            // 
            resources.ApplyResources(this.lPiperPath, "lPiperPath");
            this.lPiperPath.Name = "lPiperPath";
            // 
            // tbPiperPath
            // 
            resources.ApplyResources(this.tbPiperPath, "tbPiperPath");
            this.tbPiperPath.Name = "tbPiperPath";
            // 
            // bPiperBrowse
            // 
            resources.ApplyResources(this.bPiperBrowse, "bPiperBrowse");
            this.bPiperBrowse.UseVisualStyleBackColor = true;
            this.bPiperBrowse.Name = "bPiperBrowse";
            this.bPiperBrowse.Click += new System.EventHandler(this.BPiperBrowse_Click);
            // 
            // bPiperVerify
            // 
            resources.ApplyResources(this.bPiperVerify, "bPiperVerify");
            this.bPiperVerify.UseVisualStyleBackColor = true;
            this.bPiperVerify.Name = "bPiperVerify";
            this.bPiperVerify.Click += new System.EventHandler(this.BPiperVerify_Click);
            // 
            // lPiperVoices
            // 
            resources.ApplyResources(this.lPiperVoices, "lPiperVoices");
            this.lPiperVoices.Name = "lPiperVoices";
            // 
            // tbPiperVoices
            // 
            resources.ApplyResources(this.tbPiperVoices, "tbPiperVoices");
            this.tbPiperVoices.Name = "tbPiperVoices";
            // 
            // bPiperVoicesBrowse
            // 
            resources.ApplyResources(this.bPiperVoicesBrowse, "bPiperVoicesBrowse");
            this.bPiperVoicesBrowse.UseVisualStyleBackColor = true;
            this.bPiperVoicesBrowse.Name = "bPiperVoicesBrowse";
            this.bPiperVoicesBrowse.Click += new System.EventHandler(this.BPiperVoicesBrowse_Click);
            // 
            // bPiperDownload
            // 
            resources.ApplyResources(this.bPiperDownload, "bPiperDownload");
            this.bPiperDownload.UseVisualStyleBackColor = true;
            this.bPiperDownload.Name = "bPiperDownload";
            this.bPiperDownload.Click += new System.EventHandler(this.BPiperDownload_Click);
            // 
            // lPiperStatus
            // 
            resources.ApplyResources(this.lPiperStatus, "lPiperStatus");
            this.lPiperStatus.Name = "lPiperStatus";
            // 
            // llPiper
            // 
            resources.ApplyResources(this.llPiper, "llPiper");
            this.llPiper.TabStop = true;
            this.llPiper.Name = "llPiper";
            this.llPiper.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LlPiper_LinkClicked);
            // 
            // llPiperSamples
            // 
            resources.ApplyResources(this.llPiperSamples, "llPiperSamples");
            this.llPiperSamples.TabStop = true;
            this.llPiperSamples.Name = "llPiperSamples";
            this.llPiperSamples.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LlPiperSamples_LinkClicked);
            // 
            // lPiperInfo
            // 
            resources.ApplyResources(this.lPiperInfo, "lPiperInfo");
            this.lPiperInfo.Name = "lPiperInfo";
            // 
            // FAppSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            resources.ApplyResources(this, "$this");
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FAppSettings";
            this.pGeneral.ResumeLayout(false);
            this.pGeneral.PerformLayout();
            this.pConcreteGeneral.ResumeLayout(false);
            this.pConcreteGeneral.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.optionsView)).EndInit();
            this.exGroupBox2.ResumeLayout(false);
            this.exGroupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudSpeechConcurrency)).EndInit();
            this.pSpeech.ResumeLayout(false);
            this.pPiper.ResumeLayout(false);
            this.ppPiper.ResumeLayout(false);
            this.ppPiper.PerformLayout();
            this.gbPiper.ResumeLayout(false);
            this.gbPiper.PerformLayout();
            this.pAzure.ResumeLayout(false);
            this.ppAzure.ResumeLayout(false);
            this.ppAzure.PerformLayout();
            this.gbAzure.ResumeLayout(false);
            this.gbAzure.PerformLayout();
            this.ppSpeech.ResumeLayout(false);
            this.ppSpeech.PerformLayout();
            this.gbSpeechKey.ResumeLayout(false);
            this.gbSpeechKey.PerformLayout();
            this.gbSpeechCache.ResumeLayout(false);
            this.gbSpeechCache.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ExControls.ExGroupBox exGroupBox2;
        private ExControls.ExCheckBox cboxAutoRecalculateSoundDurations;
        private ExControls.ExCheckBox cboxAutoInsertSoundData;
        private ExControls.ExCheckBox cboxShowAfterInsertSoundDlg;
        private ExControls.ExOptionsPanel pSpeech;
        private System.Windows.Forms.Panel ppSpeech;
        private ExControls.ExGroupBox gbSpeechKey;
        private System.Windows.Forms.Label lSpeechKey;
        private ExControls.ExTextBox tbSpeechKey;
        private ExControls.ExButton bSpeechVerify;
        private System.Windows.Forms.Label lSpeechStatus;
        private System.Windows.Forms.LinkLabel llSpeechKey;
        private System.Windows.Forms.Label lSpeechConcurrency;
        private ExControls.ExNumericUpDown nudSpeechConcurrency;
        private ExControls.ExGroupBox gbSpeechCache;
        private System.Windows.Forms.Label lSpeechCache;
        private ExControls.ExButton bSpeechClearCache;
        private System.Windows.Forms.Label lSpeechPrivacy;
        private ExControls.ExOptionsPanel pAzure;
        private System.Windows.Forms.Panel ppAzure;
        private ExControls.ExGroupBox gbAzure;
        private System.Windows.Forms.Label lAzureKey;
        private ExControls.ExTextBox tbAzureKey;
        private ExControls.ExButton bAzureVerify;
        private System.Windows.Forms.Label lAzureRegion;
        private ExControls.ExTextBox tbAzureRegion;
        private System.Windows.Forms.Label lAzureStatus;
        private System.Windows.Forms.LinkLabel llAzure;
        private System.Windows.Forms.Label lAzureInfo;
        private ExControls.ExOptionsPanel pPiper;
        private System.Windows.Forms.Panel ppPiper;
        private ExControls.ExGroupBox gbPiper;
        private System.Windows.Forms.Label lPiperPath;
        private ExControls.ExTextBox tbPiperPath;
        private ExControls.ExButton bPiperBrowse;
        private ExControls.ExButton bPiperVerify;
        private System.Windows.Forms.Label lPiperVoices;
        private ExControls.ExTextBox tbPiperVoices;
        private ExControls.ExButton bPiperVoicesBrowse;
        private ExControls.ExButton bPiperDownload;
        private System.Windows.Forms.Label lPiperStatus;
        private System.Windows.Forms.LinkLabel llPiper;
        private System.Windows.Forms.LinkLabel llPiperSamples;
        private System.Windows.Forms.Label lPiperInfo;
    }
}