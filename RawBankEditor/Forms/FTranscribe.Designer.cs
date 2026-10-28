namespace RawBankEditor.Forms
{
    partial class FTranscribe
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FTranscribe));
            this.lHint = new System.Windows.Forms.Label();
            this.cboxOnlyEmpty = new ExControls.ExCheckBox();
            this.cboxSuspicious = new ExControls.ExCheckBox();
            this.dgvItems = new System.Windows.Forms.DataGridView();
            this.bTranscribe = new ExControls.ExButton();
            this.bPlay = new ExControls.ExButton();
            this.bStop = new ExControls.ExButton();
            this.lInfo = new System.Windows.Forms.Label();
            this.bOK = new ExControls.ExButton();
            this.bCancel = new ExControls.ExButton();
            this.cAccept = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.cGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cTranscript = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cMatch = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
            this.SuspendLayout();
            // 
            // lHint
            // 
            resources.ApplyResources(this.lHint, "lHint");
            this.lHint.Name = "lHint";
            // 
            // cboxOnlyEmpty
            // 
            resources.ApplyResources(this.cboxOnlyEmpty, "cboxOnlyEmpty");
            this.cboxOnlyEmpty.Name = "cboxOnlyEmpty";
            this.cboxOnlyEmpty.CheckedChanged += new System.EventHandler(this.CboxFilter_CheckedChanged);
            // 
            // cboxSuspicious
            // 
            resources.ApplyResources(this.cboxSuspicious, "cboxSuspicious");
            this.cboxSuspicious.Name = "cboxSuspicious";
            this.cboxSuspicious.CheckedChanged += new System.EventHandler(this.CboxFilter_CheckedChanged);
            // 
            // dgvItems
            // 
            this.dgvItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cAccept,
            this.cGroup,
            this.cKey,
            this.cText,
            this.cTranscript,
            this.cMatch,
            this.cStatus});
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
            // 
            // bTranscribe
            // 
            resources.ApplyResources(this.bTranscribe, "bTranscribe");
            this.bTranscribe.UseVisualStyleBackColor = true;
            this.bTranscribe.Name = "bTranscribe";
            this.bTranscribe.Click += new System.EventHandler(this.BTranscribe_Click);
            // 
            // bPlay
            // 
            resources.ApplyResources(this.bPlay, "bPlay");
            this.bPlay.UseVisualStyleBackColor = true;
            this.bPlay.Name = "bPlay";
            this.bPlay.Click += new System.EventHandler(this.BPlay_Click);
            // 
            // bStop
            // 
            resources.ApplyResources(this.bStop, "bStop");
            this.bStop.UseVisualStyleBackColor = true;
            this.bStop.Name = "bStop";
            this.bStop.Click += new System.EventHandler(this.BStop_Click);
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
            // FTranscribe
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.lHint);
            this.Controls.Add(this.cboxOnlyEmpty);
            this.Controls.Add(this.cboxSuspicious);
            this.Controls.Add(this.dgvItems);
            this.Controls.Add(this.bTranscribe);
            this.Controls.Add(this.bPlay);
            this.Controls.Add(this.bStop);
            this.Controls.Add(this.lInfo);
            this.Controls.Add(this.bOK);
            this.Controls.Add(this.bCancel);
            this.AcceptButton = this.bOK;
            this.CancelButton = this.bCancel;
            this.MinimizeBox = false;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Name = "FTranscribe";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FTranscribe_FormClosing);
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
            resources.ApplyResources(this.cGroup, "cGroup");
            this.cGroup.Name = "cGroup";
            // 
            // cKey
            // 
            this.cKey.DataPropertyName = "Key";
            resources.ApplyResources(this.cKey, "cKey");
            this.cKey.Name = "cKey";
            // 
            // cText
            // 
            this.cText.DataPropertyName = "CurrentText";
            resources.ApplyResources(this.cText, "cText");
            this.cText.Name = "cText";
            // 
            // cTranscript
            // 
            this.cTranscript.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cTranscript.DataPropertyName = "Transcript";
            resources.ApplyResources(this.cTranscript, "cTranscript");
            this.cTranscript.Name = "cTranscript";
            // 
            // cMatch
            // 
            this.cMatch.DataPropertyName = "MatchText";
            resources.ApplyResources(this.cMatch, "cMatch");
            this.cMatch.Name = "cMatch";
            // 
            // cStatus
            // 
            this.cStatus.DataPropertyName = "Status";
            resources.ApplyResources(this.cStatus, "cStatus");
            this.cStatus.Name = "cStatus";
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lHint;
        private ExControls.ExCheckBox cboxOnlyEmpty;
        private ExControls.ExCheckBox cboxSuspicious;
        private System.Windows.Forms.DataGridView dgvItems;
        private ExControls.ExButton bTranscribe;
        private ExControls.ExButton bPlay;
        private ExControls.ExButton bStop;
        private System.Windows.Forms.Label lInfo;
        private ExControls.ExButton bOK;
        private ExControls.ExButton bCancel;
        private System.Windows.Forms.DataGridViewCheckBoxColumn cAccept;
        private System.Windows.Forms.DataGridViewTextBoxColumn cGroup;
        private System.Windows.Forms.DataGridViewTextBoxColumn cKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn cText;
        private System.Windows.Forms.DataGridViewTextBoxColumn cTranscript;
        private System.Windows.Forms.DataGridViewTextBoxColumn cMatch;
        private System.Windows.Forms.DataGridViewTextBoxColumn cStatus;
    }
}
