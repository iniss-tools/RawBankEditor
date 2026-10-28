namespace RawBankEditor.Forms
{
    partial class FVoices
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FVoices));
            this.lHint = new System.Windows.Forms.Label();
            this.dgvVoices = new System.Windows.Forms.DataGridView();
            this.bPreview = new ExControls.ExButton();
            this.bLibrary = new ExControls.ExButton();
            this.bDesign = new ExControls.ExButton();
            this.bClone = new ExControls.ExButton();
            this.bDelete = new ExControls.ExButton();
            this.lStatus = new System.Windows.Forms.Label();
            this.bClose = new ExControls.ExButton();
            this.cName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cCategory = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVoices)).BeginInit();
            this.SuspendLayout();
            // 
            // lHint
            // 
            resources.ApplyResources(this.lHint, "lHint");
            this.lHint.Name = "lHint";
            // 
            // dgvVoices
            // 
            this.dgvVoices.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cName,
            this.cDescription,
            this.cCategory});
            resources.ApplyResources(this.dgvVoices, "dgvVoices");
            this.dgvVoices.AllowUserToAddRows = false;
            this.dgvVoices.AllowUserToDeleteRows = false;
            this.dgvVoices.AllowUserToResizeRows = false;
            this.dgvVoices.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvVoices.ReadOnly = true;
            this.dgvVoices.RowHeadersVisible = false;
            this.dgvVoices.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvVoices.MultiSelect = false;
            this.dgvVoices.Name = "dgvVoices";
            this.dgvVoices.SelectionChanged += new System.EventHandler(this.DgvVoices_SelectionChanged);
            // 
            // bPreview
            // 
            resources.ApplyResources(this.bPreview, "bPreview");
            this.bPreview.UseVisualStyleBackColor = true;
            this.bPreview.Name = "bPreview";
            this.bPreview.Click += new System.EventHandler(this.BPreview_Click);
            // 
            // bLibrary
            // 
            resources.ApplyResources(this.bLibrary, "bLibrary");
            this.bLibrary.UseVisualStyleBackColor = true;
            this.bLibrary.Name = "bLibrary";
            this.bLibrary.Click += new System.EventHandler(this.BLibrary_Click);
            // 
            // bDesign
            // 
            resources.ApplyResources(this.bDesign, "bDesign");
            this.bDesign.UseVisualStyleBackColor = true;
            this.bDesign.Name = "bDesign";
            this.bDesign.Click += new System.EventHandler(this.BDesign_Click);
            // 
            // bClone
            // 
            resources.ApplyResources(this.bClone, "bClone");
            this.bClone.UseVisualStyleBackColor = true;
            this.bClone.Name = "bClone";
            this.bClone.Click += new System.EventHandler(this.BClone_Click);
            // 
            // bDelete
            // 
            resources.ApplyResources(this.bDelete, "bDelete");
            this.bDelete.UseVisualStyleBackColor = true;
            this.bDelete.Name = "bDelete";
            this.bDelete.Click += new System.EventHandler(this.BDelete_Click);
            // 
            // lStatus
            // 
            resources.ApplyResources(this.lStatus, "lStatus");
            this.lStatus.Name = "lStatus";
            // 
            // bClose
            // 
            resources.ApplyResources(this.bClose, "bClose");
            this.bClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bClose.UseVisualStyleBackColor = true;
            this.bClose.Name = "bClose";
            // 
            // FVoices
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.lHint);
            this.Controls.Add(this.dgvVoices);
            this.Controls.Add(this.bPreview);
            this.Controls.Add(this.bLibrary);
            this.Controls.Add(this.bDesign);
            this.Controls.Add(this.bClone);
            this.Controls.Add(this.bDelete);
            this.Controls.Add(this.lStatus);
            this.Controls.Add(this.bClose);
            this.CancelButton = this.bClose;
            this.MinimizeBox = false;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Name = "FVoices";
            this.Shown += new System.EventHandler(this.FVoices_Shown);
            // 
            // cName
            // 
            this.cName.DataPropertyName = "Name";
            resources.ApplyResources(this.cName, "cName");
            this.cName.Name = "cName";
            // 
            // cDescription
            // 
            this.cDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cDescription.DataPropertyName = "Description";
            resources.ApplyResources(this.cDescription, "cDescription");
            this.cDescription.Name = "cDescription";
            // 
            // cCategory
            // 
            this.cCategory.DataPropertyName = "Category";
            resources.ApplyResources(this.cCategory, "cCategory");
            this.cCategory.Name = "cCategory";
            ((System.ComponentModel.ISupportInitialize)(this.dgvVoices)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lHint;
        private System.Windows.Forms.DataGridView dgvVoices;
        private ExControls.ExButton bPreview;
        private ExControls.ExButton bLibrary;
        private ExControls.ExButton bDesign;
        private ExControls.ExButton bClone;
        private ExControls.ExButton bDelete;
        private System.Windows.Forms.Label lStatus;
        private ExControls.ExButton bClose;
        private System.Windows.Forms.DataGridViewTextBoxColumn cName;
        private System.Windows.Forms.DataGridViewTextBoxColumn cDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn cCategory;
    }
}
