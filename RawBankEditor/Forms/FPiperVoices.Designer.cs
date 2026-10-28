namespace RawBankEditor.Forms
{
    partial class FPiperVoices
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FPiperVoices));
            this.lHint = new System.Windows.Forms.Label();
            this.lLanguage = new System.Windows.Forms.Label();
            this.cbLanguage = new ExControls.ExComboBox();
            this.llSamples = new System.Windows.Forms.LinkLabel();
            this.dgvVoices = new System.Windows.Forms.DataGridView();
            this.lStatus = new System.Windows.Forms.Label();
            this.bDownload = new ExControls.ExButton();
            this.bDelete = new ExControls.ExButton();
            this.bClose = new ExControls.ExButton();
            this.cKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cLanguage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cQuality = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cState = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVoices)).BeginInit();
            this.SuspendLayout();
            // 
            // lHint
            // 
            resources.ApplyResources(this.lHint, "lHint");
            this.lHint.Name = "lHint";
            // 
            // lLanguage
            // 
            resources.ApplyResources(this.lLanguage, "lLanguage");
            this.lLanguage.Name = "lLanguage";
            // 
            // cbLanguage
            // 
            resources.ApplyResources(this.cbLanguage, "cbLanguage");
            this.cbLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLanguage.FormattingEnabled = true;
            this.cbLanguage.Name = "cbLanguage";
            this.cbLanguage.SelectedIndexChanged += new System.EventHandler(this.CbLanguage_SelectedIndexChanged);
            // 
            // llSamples
            // 
            resources.ApplyResources(this.llSamples, "llSamples");
            this.llSamples.TabStop = true;
            this.llSamples.Name = "llSamples";
            this.llSamples.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LlSamples_LinkClicked);
            // 
            // dgvVoices
            // 
            this.dgvVoices.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cKey,
            this.cLanguage,
            this.cQuality,
            this.cSize,
            this.cState});
            resources.ApplyResources(this.dgvVoices, "dgvVoices");
            this.dgvVoices.AllowUserToAddRows = false;
            this.dgvVoices.AllowUserToDeleteRows = false;
            this.dgvVoices.AllowUserToResizeRows = false;
            this.dgvVoices.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvVoices.ReadOnly = true;
            this.dgvVoices.RowHeadersVisible = false;
            this.dgvVoices.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvVoices.Name = "dgvVoices";
            this.dgvVoices.SelectionChanged += new System.EventHandler(this.DgvVoices_SelectionChanged);
            // 
            // lStatus
            // 
            resources.ApplyResources(this.lStatus, "lStatus");
            this.lStatus.Name = "lStatus";
            // 
            // bDownload
            // 
            resources.ApplyResources(this.bDownload, "bDownload");
            this.bDownload.UseVisualStyleBackColor = true;
            this.bDownload.Name = "bDownload";
            this.bDownload.Click += new System.EventHandler(this.BDownload_Click);
            // 
            // bDelete
            // 
            resources.ApplyResources(this.bDelete, "bDelete");
            this.bDelete.UseVisualStyleBackColor = true;
            this.bDelete.Name = "bDelete";
            this.bDelete.Click += new System.EventHandler(this.BDelete_Click);
            // 
            // bClose
            // 
            resources.ApplyResources(this.bClose, "bClose");
            this.bClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bClose.UseVisualStyleBackColor = true;
            this.bClose.Name = "bClose";
            // 
            // FPiperVoices
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.lHint);
            this.Controls.Add(this.lLanguage);
            this.Controls.Add(this.cbLanguage);
            this.Controls.Add(this.llSamples);
            this.Controls.Add(this.dgvVoices);
            this.Controls.Add(this.lStatus);
            this.Controls.Add(this.bDownload);
            this.Controls.Add(this.bDelete);
            this.Controls.Add(this.bClose);
            this.CancelButton = this.bClose;
            this.MinimizeBox = false;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Name = "FPiperVoices";
            this.Shown += new System.EventHandler(this.FPiperVoices_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FPiperVoices_FormClosing);
            // 
            // cKey
            // 
            this.cKey.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cKey.ReadOnly = true;
            resources.ApplyResources(this.cKey, "cKey");
            this.cKey.Name = "cKey";
            // 
            // cLanguage
            // 
            this.cLanguage.ReadOnly = true;
            resources.ApplyResources(this.cLanguage, "cLanguage");
            this.cLanguage.Name = "cLanguage";
            // 
            // cQuality
            // 
            this.cQuality.ReadOnly = true;
            resources.ApplyResources(this.cQuality, "cQuality");
            this.cQuality.Name = "cQuality";
            // 
            // cSize
            // 
            this.cSize.ReadOnly = true;
            resources.ApplyResources(this.cSize, "cSize");
            this.cSize.Name = "cSize";
            // 
            // cState
            // 
            this.cState.ReadOnly = true;
            resources.ApplyResources(this.cState, "cState");
            this.cState.Name = "cState";
            ((System.ComponentModel.ISupportInitialize)(this.dgvVoices)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lHint;
        private System.Windows.Forms.Label lLanguage;
        private ExControls.ExComboBox cbLanguage;
        private System.Windows.Forms.LinkLabel llSamples;
        private System.Windows.Forms.DataGridView dgvVoices;
        private System.Windows.Forms.Label lStatus;
        private ExControls.ExButton bDownload;
        private ExControls.ExButton bDelete;
        private ExControls.ExButton bClose;
        private System.Windows.Forms.DataGridViewTextBoxColumn cKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn cLanguage;
        private System.Windows.Forms.DataGridViewTextBoxColumn cQuality;
        private System.Windows.Forms.DataGridViewTextBoxColumn cSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn cState;
    }
}
