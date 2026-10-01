namespace RawBankEditor.Forms
{
    partial class FImportSounds
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FImportSounds));
            this.tlpMain = new System.Windows.Forms.TableLayoutPanel();
            this.flpSource = new System.Windows.Forms.FlowLayoutPanel();
            this.bClipboard = new ExControls.ExButton();
            this.bFile = new ExControls.ExButton();
            this.lblEncoding = new ExControls.ExLabel();
            this.cbEncoding = new ExControls.ExComboBox();
            this.cboxFirstHeader = new ExControls.ExCheckBox();
            this.cboxSkipExisting = new ExControls.ExCheckBox();
            this.lblHint = new ExControls.ExLabel();
            this.dgvData = new System.Windows.Forms.DataGridView();
            this.flpButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.bStorno = new ExControls.ExButton();
            this.bImport = new ExControls.ExButton();
            this.cmsColumns = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ofdTable = new System.Windows.Forms.OpenFileDialog();
            this.tlpMain.SuspendLayout();
            this.flpSource.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
            this.flpButtons.SuspendLayout();
            this.SuspendLayout();
            //
            // tlpMain
            //
            resources.ApplyResources(this.tlpMain, "tlpMain");
            this.tlpMain.Controls.Add(this.flpSource, 0, 0);
            this.tlpMain.Controls.Add(this.lblHint, 0, 1);
            this.tlpMain.Controls.Add(this.dgvData, 0, 2);
            this.tlpMain.Controls.Add(this.flpButtons, 0, 3);
            this.tlpMain.Name = "tlpMain";
            //
            // flpSource
            //
            resources.ApplyResources(this.flpSource, "flpSource");
            this.flpSource.Controls.Add(this.bClipboard);
            this.flpSource.Controls.Add(this.bFile);
            this.flpSource.Controls.Add(this.lblEncoding);
            this.flpSource.Controls.Add(this.cbEncoding);
            this.flpSource.Controls.Add(this.cboxFirstHeader);
            this.flpSource.Controls.Add(this.cboxSkipExisting);
            this.flpSource.Name = "flpSource";
            //
            // bClipboard
            //
            resources.ApplyResources(this.bClipboard, "bClipboard");
            this.bClipboard.Name = "bClipboard";
            this.bClipboard.UseVisualStyleBackColor = true;
            this.bClipboard.Click += new System.EventHandler(this.BClipboard_Click);
            //
            // bFile
            //
            resources.ApplyResources(this.bFile, "bFile");
            this.bFile.Name = "bFile";
            this.bFile.UseVisualStyleBackColor = true;
            this.bFile.Click += new System.EventHandler(this.BFile_Click);
            //
            // lblEncoding
            //
            resources.ApplyResources(this.lblEncoding, "lblEncoding");
            this.lblEncoding.Name = "lblEncoding";
            //
            // cbEncoding
            //
            this.cbEncoding.DropDownSelectedRowBackColor = System.Drawing.SystemColors.Highlight;
            this.cbEncoding.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbEncoding.FormattingEnabled = true;
            this.cbEncoding.Items.AddRange(new object[] {
            resources.GetString("cbEncoding.Items"),
            resources.GetString("cbEncoding.Items1")});
            resources.ApplyResources(this.cbEncoding, "cbEncoding");
            this.cbEncoding.Name = "cbEncoding";
            this.cbEncoding.UseDarkScrollBar = false;
            this.cbEncoding.SelectedIndexChanged += new System.EventHandler(this.CbEncoding_SelectedIndexChanged);
            //
            // cboxFirstHeader
            //
            resources.ApplyResources(this.cboxFirstHeader, "cboxFirstHeader");
            this.cboxFirstHeader.BoxBackColor = System.Drawing.Color.White;
            this.cboxFirstHeader.Checked = true;
            this.cboxFirstHeader.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cboxFirstHeader.HighlightColor = System.Drawing.SystemColors.Highlight;
            this.cboxFirstHeader.Name = "cboxFirstHeader";
            this.cboxFirstHeader.UseVisualStyleBackColor = true;
            this.cboxFirstHeader.CheckedChanged += new System.EventHandler(this.CboxFirstHeader_CheckedChanged);
            //
            // cboxSkipExisting
            //
            resources.ApplyResources(this.cboxSkipExisting, "cboxSkipExisting");
            this.cboxSkipExisting.BoxBackColor = System.Drawing.Color.White;
            this.cboxSkipExisting.Checked = true;
            this.cboxSkipExisting.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cboxSkipExisting.HighlightColor = System.Drawing.SystemColors.Highlight;
            this.cboxSkipExisting.Name = "cboxSkipExisting";
            this.cboxSkipExisting.UseVisualStyleBackColor = true;
            //
            // lblHint
            //
            resources.ApplyResources(this.lblHint, "lblHint");
            this.lblHint.Name = "lblHint";
            //
            // dgvData
            //
            this.dgvData.AllowDrop = true;
            this.dgvData.AllowUserToAddRows = false;
            this.dgvData.AllowUserToDeleteRows = false;
            this.dgvData.AllowUserToResizeRows = false;
            this.dgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            resources.ApplyResources(this.dgvData, "dgvData");
            this.dgvData.Name = "dgvData";
            this.dgvData.ReadOnly = true;
            this.dgvData.RowHeadersVisible = false;
            this.dgvData.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DgvData_ColumnHeaderMouseClick);
            this.dgvData.DragDrop += new System.Windows.Forms.DragEventHandler(this.FImportSounds_DragDrop);
            this.dgvData.DragEnter += new System.Windows.Forms.DragEventHandler(this.FImportSounds_DragEnter);
            //
            // flpButtons
            //
            resources.ApplyResources(this.flpButtons, "flpButtons");
            this.flpButtons.Controls.Add(this.bStorno);
            this.flpButtons.Controls.Add(this.bImport);
            this.flpButtons.Name = "flpButtons";
            //
            // bStorno
            //
            resources.ApplyResources(this.bStorno, "bStorno");
            this.bStorno.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bStorno.Name = "bStorno";
            this.bStorno.UseVisualStyleBackColor = true;
            //
            // bImport
            //
            resources.ApplyResources(this.bImport, "bImport");
            this.bImport.Name = "bImport";
            this.bImport.UseVisualStyleBackColor = true;
            this.bImport.Click += new System.EventHandler(this.BImport_Click);
            //
            // cmsColumns
            //
            this.cmsColumns.Name = "cmsColumns";
            resources.ApplyResources(this.cmsColumns, "cmsColumns");
            //
            // ofdTable
            //
            resources.ApplyResources(this.ofdTable, "ofdTable");
            //
            // FImportSounds
            //
            this.AcceptButton = this.bImport;
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bStorno;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.tlpMain);
            this.MinimizeBox = false;
            this.Name = "FImportSounds";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.FImportSounds_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.FImportSounds_DragEnter);
            this.tlpMain.ResumeLayout(false);
            this.tlpMain.PerformLayout();
            this.flpSource.ResumeLayout(false);
            this.flpSource.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
            this.flpButtons.ResumeLayout(false);
            this.flpButtons.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpMain;
        private System.Windows.Forms.FlowLayoutPanel flpSource;
        private ExControls.ExButton bClipboard;
        private ExControls.ExButton bFile;
        private ExControls.ExLabel lblEncoding;
        private ExControls.ExComboBox cbEncoding;
        private ExControls.ExCheckBox cboxFirstHeader;
        private ExControls.ExCheckBox cboxSkipExisting;
        private ExControls.ExLabel lblHint;
        private System.Windows.Forms.DataGridView dgvData;
        private System.Windows.Forms.FlowLayoutPanel flpButtons;
        private ExControls.ExButton bStorno;
        private ExControls.ExButton bImport;
        private System.Windows.Forms.ContextMenuStrip cmsColumns;
        private System.Windows.Forms.OpenFileDialog ofdTable;
    }
}
