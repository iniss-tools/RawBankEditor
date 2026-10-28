namespace RawBankEditor.Forms
{
    partial class FGenerateSounds
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FGenerateSounds));
            this.lVoice = new System.Windows.Forms.Label();
            this.bVoice = new ExControls.ExButton();
            this.cboxAdjust = new ExControls.ExCheckBox();
            this.cboxSkipDone = new ExControls.ExCheckBox();
            this.cboxVerify = new ExControls.ExCheckBox();
            this.cboxSuspicious = new ExControls.ExCheckBox();
            this.dgvItems = new System.Windows.Forms.DataGridView();
            this.bGenerate = new ExControls.ExButton();
            this.bRegenerate = new ExControls.ExButton();
            this.bEvenTempo = new ExControls.ExButton();
            this.bPlayOld = new ExControls.ExButton();
            this.bPlayNew = new ExControls.ExButton();
            this.bStop = new ExControls.ExButton();
            this.bPlaySequence = new ExControls.ExButton();
            this.lInfo = new System.Windows.Forms.Label();
            this.bOK = new ExControls.ExButton();
            this.bCancel = new ExControls.ExButton();
            this.cAccept = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.cGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cMatch = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cTempo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cOldDuration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cNewDuration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
            this.SuspendLayout();
            // 
            // lVoice
            // 
            resources.ApplyResources(this.lVoice, "lVoice");
            this.lVoice.Name = "lVoice";
            // 
            // bVoice
            // 
            resources.ApplyResources(this.bVoice, "bVoice");
            this.bVoice.UseVisualStyleBackColor = true;
            this.bVoice.Name = "bVoice";
            this.bVoice.Click += new System.EventHandler(this.BVoice_Click);
            // 
            // cboxAdjust
            // 
            resources.ApplyResources(this.cboxAdjust, "cboxAdjust");
            this.cboxAdjust.Checked = true;
            this.cboxAdjust.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cboxAdjust.Name = "cboxAdjust";
            this.cboxAdjust.CheckedChanged += new System.EventHandler(this.CboxAdjust_CheckedChanged);
            // 
            // cboxSkipDone
            // 
            resources.ApplyResources(this.cboxSkipDone, "cboxSkipDone");
            this.cboxSkipDone.Name = "cboxSkipDone";
            this.cboxSkipDone.CheckedChanged += new System.EventHandler(this.CboxSkipDone_CheckedChanged);
            // 
            // cboxVerify
            // 
            resources.ApplyResources(this.cboxVerify, "cboxVerify");
            this.cboxVerify.Checked = true;
            this.cboxVerify.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cboxVerify.Name = "cboxVerify";
            this.cboxVerify.CheckedChanged += new System.EventHandler(this.CboxVerify_CheckedChanged);
            // 
            // cboxSuspicious
            // 
            resources.ApplyResources(this.cboxSuspicious, "cboxSuspicious");
            this.cboxSuspicious.Name = "cboxSuspicious";
            this.cboxSuspicious.CheckedChanged += new System.EventHandler(this.CboxSuspicious_CheckedChanged);
            // 
            // dgvItems
            // 
            this.dgvItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cAccept,
            this.cGroup,
            this.cKey,
            this.cText,
            this.cStatus,
            this.cMatch,
            this.cTempo,
            this.cOldDuration,
            this.cNewDuration});
            resources.ApplyResources(this.dgvItems, "dgvItems");
            this.dgvItems.AllowUserToAddRows = false;
            this.dgvItems.AllowUserToDeleteRows = false;
            this.dgvItems.AllowUserToResizeRows = false;
            this.dgvItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvItems.RowHeadersVisible = false;
            this.dgvItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvItems.Name = "dgvItems";
            this.dgvItems.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvItems_CellDoubleClick);
            this.dgvItems.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvItems_CellEndEdit);
            this.dgvItems.CurrentCellDirtyStateChanged += new System.EventHandler(this.DgvItems_CurrentCellDirtyStateChanged);
            this.dgvItems.SelectionChanged += new System.EventHandler(this.DgvItems_SelectionChanged);
            this.dgvItems.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DgvItems_KeyDown);
            this.dgvItems.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DgvItems_ColumnHeaderMouseClick);
            // 
            // bGenerate
            // 
            resources.ApplyResources(this.bGenerate, "bGenerate");
            this.bGenerate.UseVisualStyleBackColor = true;
            this.bGenerate.Name = "bGenerate";
            this.bGenerate.Click += new System.EventHandler(this.BGenerate_Click);
            // 
            // bRegenerate
            // 
            resources.ApplyResources(this.bRegenerate, "bRegenerate");
            this.bRegenerate.UseVisualStyleBackColor = true;
            this.bRegenerate.Name = "bRegenerate";
            this.bRegenerate.Click += new System.EventHandler(this.BRegenerate_Click);
            // 
            // bEvenTempo
            // 
            resources.ApplyResources(this.bEvenTempo, "bEvenTempo");
            this.bEvenTempo.UseVisualStyleBackColor = true;
            this.bEvenTempo.Name = "bEvenTempo";
            this.bEvenTempo.Click += new System.EventHandler(this.BEvenTempo_Click);
            // 
            // bPlayOld
            // 
            resources.ApplyResources(this.bPlayOld, "bPlayOld");
            this.bPlayOld.UseVisualStyleBackColor = true;
            this.bPlayOld.Name = "bPlayOld";
            this.bPlayOld.Click += new System.EventHandler(this.BPlayOld_Click);
            // 
            // bPlayNew
            // 
            resources.ApplyResources(this.bPlayNew, "bPlayNew");
            this.bPlayNew.UseVisualStyleBackColor = true;
            this.bPlayNew.Name = "bPlayNew";
            this.bPlayNew.Click += new System.EventHandler(this.BPlayNew_Click);
            // 
            // bStop
            // 
            resources.ApplyResources(this.bStop, "bStop");
            this.bStop.UseVisualStyleBackColor = true;
            this.bStop.Name = "bStop";
            this.bStop.Click += new System.EventHandler(this.BStop_Click);
            // 
            // bPlaySequence
            // 
            resources.ApplyResources(this.bPlaySequence, "bPlaySequence");
            this.bPlaySequence.UseVisualStyleBackColor = true;
            this.bPlaySequence.Name = "bPlaySequence";
            this.bPlaySequence.Click += new System.EventHandler(this.BPlaySequence_Click);
            // 
            // lInfo
            // 
            resources.ApplyResources(this.lInfo, "lInfo");
            this.lInfo.Name = "lInfo";
            // 
            // bOK
            // 
            resources.ApplyResources(this.bOK, "bOK");
            this.bOK.UseVisualStyleBackColor = true;
            this.bOK.Name = "bOK";
            this.bOK.Click += new System.EventHandler(this.BOK_Click);
            // 
            // bCancel
            // 
            resources.ApplyResources(this.bCancel, "bCancel");
            this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancel.UseVisualStyleBackColor = true;
            this.bCancel.Name = "bCancel";
            // 
            // FGenerateSounds
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.lVoice);
            this.Controls.Add(this.bVoice);
            this.Controls.Add(this.cboxAdjust);
            this.Controls.Add(this.cboxSkipDone);
            this.Controls.Add(this.cboxVerify);
            this.Controls.Add(this.cboxSuspicious);
            this.Controls.Add(this.dgvItems);
            this.Controls.Add(this.bGenerate);
            this.Controls.Add(this.bRegenerate);
            this.Controls.Add(this.bEvenTempo);
            this.Controls.Add(this.bPlayOld);
            this.Controls.Add(this.bPlayNew);
            this.Controls.Add(this.bStop);
            this.Controls.Add(this.bPlaySequence);
            this.Controls.Add(this.lInfo);
            this.Controls.Add(this.bOK);
            this.Controls.Add(this.bCancel);
            this.AcceptButton = this.bOK;
            this.CancelButton = this.bCancel;
            this.MinimizeBox = false;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Name = "FGenerateSounds";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FGenerateSounds_FormClosing);
            this.Shown += new System.EventHandler(this.FGenerateSounds_Shown);
            // 
            // cAccept
            // 
            this.cAccept.DataPropertyName = "Accept";
            resources.ApplyResources(this.cAccept, "cAccept");
            this.cAccept.Name = "cAccept";
            // 
            // cGroup
            // 
            this.cGroup.DataPropertyName = "Group";
            this.cGroup.ReadOnly = true;
            resources.ApplyResources(this.cGroup, "cGroup");
            this.cGroup.Name = "cGroup";
            // 
            // cKey
            // 
            this.cKey.DataPropertyName = "Key";
            this.cKey.ReadOnly = true;
            resources.ApplyResources(this.cKey, "cKey");
            this.cKey.Name = "cKey";
            // 
            // cText
            // 
            this.cText.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cText.DataPropertyName = "Text";
            resources.ApplyResources(this.cText, "cText");
            this.cText.Name = "cText";
            // 
            // cStatus
            // 
            this.cStatus.DataPropertyName = "Status";
            this.cStatus.ReadOnly = true;
            resources.ApplyResources(this.cStatus, "cStatus");
            this.cStatus.Name = "cStatus";
            // 
            // cMatch
            // 
            this.cMatch.DataPropertyName = "MatchText";
            this.cMatch.ReadOnly = true;
            resources.ApplyResources(this.cMatch, "cMatch");
            this.cMatch.Name = "cMatch";
            // 
            // cTempo
            // 
            this.cTempo.DataPropertyName = "TempoText";
            this.cTempo.ReadOnly = true;
            resources.ApplyResources(this.cTempo, "cTempo");
            this.cTempo.Name = "cTempo";
            // 
            // cOldDuration
            // 
            this.cOldDuration.DataPropertyName = "OldDurationText";
            this.cOldDuration.ReadOnly = true;
            resources.ApplyResources(this.cOldDuration, "cOldDuration");
            this.cOldDuration.Name = "cOldDuration";
            // 
            // cNewDuration
            // 
            this.cNewDuration.DataPropertyName = "NewDurationText";
            this.cNewDuration.ReadOnly = true;
            resources.ApplyResources(this.cNewDuration, "cNewDuration");
            this.cNewDuration.Name = "cNewDuration";
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lVoice;
        private ExControls.ExButton bVoice;
        private ExControls.ExCheckBox cboxAdjust;
        private ExControls.ExCheckBox cboxSkipDone;
        private ExControls.ExCheckBox cboxVerify;
        private ExControls.ExCheckBox cboxSuspicious;
        private System.Windows.Forms.DataGridView dgvItems;
        private ExControls.ExButton bGenerate;
        private ExControls.ExButton bRegenerate;
        private ExControls.ExButton bEvenTempo;
        private ExControls.ExButton bPlayOld;
        private ExControls.ExButton bPlayNew;
        private ExControls.ExButton bStop;
        private ExControls.ExButton bPlaySequence;
        private System.Windows.Forms.Label lInfo;
        private ExControls.ExButton bOK;
        private ExControls.ExButton bCancel;
        private System.Windows.Forms.DataGridViewCheckBoxColumn cAccept;
        private System.Windows.Forms.DataGridViewTextBoxColumn cGroup;
        private System.Windows.Forms.DataGridViewTextBoxColumn cKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn cText;
        private System.Windows.Forms.DataGridViewTextBoxColumn cStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn cMatch;
        private System.Windows.Forms.DataGridViewTextBoxColumn cTempo;
        private System.Windows.Forms.DataGridViewTextBoxColumn cOldDuration;
        private System.Windows.Forms.DataGridViewTextBoxColumn cNewDuration;
    }
}
