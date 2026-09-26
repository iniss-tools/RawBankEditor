namespace RawBankEditor.Forms
{
    partial class FAfterInsertSounds
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.bStorno = new ExControls.ExButton();
            this.bOK = new ExControls.ExButton();
            this.dgvFilesSounds = new System.Windows.Forms.DataGridView();
            this.cFilePath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cSoundKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cSoundName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cSoundFileName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cSoundDuration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cSoundText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fyzSoundBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.exLabel1 = new ExControls.ExLabel();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFilesSounds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fyzSoundBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.bStorno);
            this.panel1.Controls.Add(this.bOK);
            this.panel1.Controls.Add(this.dgvFilesSounds);
            this.panel1.Controls.Add(this.exLabel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(5);
            this.panel1.Size = new System.Drawing.Size(804, 442);
            this.panel1.TabIndex = 0;
            // 
            // bStorno
            // 
            this.bStorno.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bStorno.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bStorno.Location = new System.Drawing.Point(714, 411);
            this.bStorno.Name = "bStorno";
            this.bStorno.Size = new System.Drawing.Size(75, 23);
            this.bStorno.TabIndex = 3;
            this.bStorno.Text = "Zrušiť";
            this.bStorno.UseVisualStyleBackColor = true;
            // 
            // bOK
            // 
            this.bOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bOK.Location = new System.Drawing.Point(633, 411);
            this.bOK.Name = "bOK";
            this.bOK.Size = new System.Drawing.Size(75, 23);
            this.bOK.TabIndex = 2;
            this.bOK.Text = "OK";
            this.bOK.UseVisualStyleBackColor = true;
            this.bOK.Click += new System.EventHandler(this.BOK_Click);
            // 
            // dgvFilesSounds
            // 
            this.dgvFilesSounds.AllowUserToAddRows = false;
            this.dgvFilesSounds.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvFilesSounds.AutoGenerateColumns = false;
            this.dgvFilesSounds.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFilesSounds.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cFilePath,
            this.cSoundKey,
            this.cSoundName,
            this.cSoundFileName,
            this.cSoundDuration,
            this.cSoundText});
            this.dgvFilesSounds.DataSource = this.fyzSoundBindingSource;
            this.dgvFilesSounds.Location = new System.Drawing.Point(5, 52);
            this.dgvFilesSounds.Name = "dgvFilesSounds";
            this.dgvFilesSounds.RowHeadersVisible = false;
            this.dgvFilesSounds.Size = new System.Drawing.Size(791, 353);
            this.dgvFilesSounds.TabIndex = 1;
            this.dgvFilesSounds.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.DgvFilesSounds_CellFormatting);
            this.dgvFilesSounds.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.DgvFilesSounds_UserDeletingRow);
            // 
            // cFilePath
            // 
            this.cFilePath.HeaderText = "Cesta";
            this.cFilePath.Name = "cFilePath";
            this.cFilePath.ReadOnly = true;
            // 
            // cSoundKey
            // 
            this.cSoundKey.DataPropertyName = "Key";
            this.cSoundKey.HeaderText = "Kľúč";
            this.cSoundKey.Name = "cSoundKey";
            // 
            // cSoundName
            // 
            this.cSoundName.DataPropertyName = "Name";
            this.cSoundName.HeaderText = "Názov";
            this.cSoundName.Name = "cSoundName";
            // 
            // cSoundFileName
            // 
            this.cSoundFileName.DataPropertyName = "FileName";
            this.cSoundFileName.HeaderText = "Názov súboru";
            this.cSoundFileName.Name = "cSoundFileName";
            this.cSoundFileName.ReadOnly = true;
            // 
            // cSoundDuration
            // 
            this.cSoundDuration.DataPropertyName = "DurationText";
            this.cSoundDuration.HeaderText = "Trvanie";
            this.cSoundDuration.Name = "cSoundDuration";
            this.cSoundDuration.ReadOnly = true;
            // 
            // cSoundText
            // 
            this.cSoundText.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cSoundText.DataPropertyName = "Text";
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.cSoundText.DefaultCellStyle = dataGridViewCellStyle4;
            this.cSoundText.HeaderText = "Text hlásenia";
            this.cSoundText.Name = "cSoundText";
            // 
            // fyzSoundBindingSource
            // 
            this.fyzSoundBindingSource.DataSource = typeof(ToolsCore.Entities.FyzSound);
            // 
            // exLabel1
            // 
            this.exLabel1.AutoSize = true;
            this.exLabel1.Location = new System.Drawing.Point(2, 10);
            this.exLabel1.Name = "exLabel1";
            this.exLabel1.Size = new System.Drawing.Size(306, 39);
            this.exLabel1.TabIndex = 0;
            this.exLabel1.Text = "Do priečinka zvukovej banky boli práve pridané tieto súbory. \r\n\r\nMôžete upraviť i" +
    "ch vlastnosti pred pridaním do zvukovej banky:";
            // 
            // FAfterInsertSounds
            // 
            this.AcceptButton = this.bOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bStorno;
            this.ClientSize = new System.Drawing.Size(804, 442);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FAfterInsertSounds";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Novopridané súbory";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FAfterInsertSounds_FormClosed);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFilesSounds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fyzSoundBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel1;
        private ExControls.ExLabel exLabel1;
        private DataGridView dgvFilesSounds;
        private ExControls.ExButton bStorno;
        private ExControls.ExButton bOK;
        private BindingSource fyzSoundBindingSource;
        private DataGridViewTextBoxColumn cFilePath;
        private DataGridViewTextBoxColumn cSoundKey;
        private DataGridViewTextBoxColumn cSoundName;
        private DataGridViewTextBoxColumn cSoundFileName;
        private DataGridViewTextBoxColumn cSoundDuration;
        private DataGridViewTextBoxColumn cSoundText;
    }
}