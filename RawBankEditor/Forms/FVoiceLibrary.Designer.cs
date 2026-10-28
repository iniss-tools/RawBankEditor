namespace RawBankEditor.Forms
{
    partial class FVoiceLibrary
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FVoiceLibrary));
            this.lLanguage = new System.Windows.Forms.Label();
            this.cbLanguage = new ExControls.ExComboBox();
            this.lGender = new System.Windows.Forms.Label();
            this.cbGender = new ExControls.ExComboBox();
            this.lSearch = new System.Windows.Forms.Label();
            this.tbSearch = new ExControls.ExTextBox();
            this.bSearch = new ExControls.ExButton();
            this.dgvVoices = new System.Windows.Forms.DataGridView();
            this.bPreview = new ExControls.ExButton();
            this.bAdd = new ExControls.ExButton();
            this.bMore = new ExControls.ExButton();
            this.lStatus = new System.Windows.Forms.Label();
            this.bClose = new ExControls.ExButton();
            this.cName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cGender = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cAge = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cAccent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cLanguage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvVoices)).BeginInit();
            this.SuspendLayout();
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
            // 
            // lGender
            // 
            resources.ApplyResources(this.lGender, "lGender");
            this.lGender.Name = "lGender";
            // 
            // cbGender
            // 
            resources.ApplyResources(this.cbGender, "cbGender");
            this.cbGender.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbGender.FormattingEnabled = true;
            this.cbGender.Name = "cbGender";
            // 
            // lSearch
            // 
            resources.ApplyResources(this.lSearch, "lSearch");
            this.lSearch.Name = "lSearch";
            // 
            // tbSearch
            // 
            resources.ApplyResources(this.tbSearch, "tbSearch");
            this.tbSearch.Name = "tbSearch";
            // 
            // bSearch
            // 
            resources.ApplyResources(this.bSearch, "bSearch");
            this.bSearch.UseVisualStyleBackColor = true;
            this.bSearch.Name = "bSearch";
            this.bSearch.Click += new System.EventHandler(this.BSearch_Click);
            // 
            // dgvVoices
            // 
            this.dgvVoices.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cName,
            this.cGender,
            this.cAge,
            this.cAccent,
            this.cLanguage,
            this.cDescription});
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
            // bAdd
            // 
            resources.ApplyResources(this.bAdd, "bAdd");
            this.bAdd.UseVisualStyleBackColor = true;
            this.bAdd.Name = "bAdd";
            this.bAdd.Click += new System.EventHandler(this.BAdd_Click);
            // 
            // bMore
            // 
            resources.ApplyResources(this.bMore, "bMore");
            this.bMore.UseVisualStyleBackColor = true;
            this.bMore.Name = "bMore";
            this.bMore.Click += new System.EventHandler(this.BMore_Click);
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
            // FVoiceLibrary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.lLanguage);
            this.Controls.Add(this.cbLanguage);
            this.Controls.Add(this.lGender);
            this.Controls.Add(this.cbGender);
            this.Controls.Add(this.lSearch);
            this.Controls.Add(this.tbSearch);
            this.Controls.Add(this.bSearch);
            this.Controls.Add(this.dgvVoices);
            this.Controls.Add(this.bPreview);
            this.Controls.Add(this.bAdd);
            this.Controls.Add(this.bMore);
            this.Controls.Add(this.lStatus);
            this.Controls.Add(this.bClose);
            this.AcceptButton = this.bSearch;
            this.CancelButton = this.bClose;
            this.MinimizeBox = false;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Name = "FVoiceLibrary";
            this.Shown += new System.EventHandler(this.FVoiceLibrary_Shown);
            // 
            // cName
            // 
            this.cName.DataPropertyName = "Name";
            resources.ApplyResources(this.cName, "cName");
            this.cName.Name = "cName";
            // 
            // cGender
            // 
            this.cGender.DataPropertyName = "Gender";
            resources.ApplyResources(this.cGender, "cGender");
            this.cGender.Name = "cGender";
            // 
            // cAge
            // 
            this.cAge.DataPropertyName = "Age";
            resources.ApplyResources(this.cAge, "cAge");
            this.cAge.Name = "cAge";
            // 
            // cAccent
            // 
            this.cAccent.DataPropertyName = "Accent";
            resources.ApplyResources(this.cAccent, "cAccent");
            this.cAccent.Name = "cAccent";
            // 
            // cLanguage
            // 
            this.cLanguage.DataPropertyName = "Language";
            resources.ApplyResources(this.cLanguage, "cLanguage");
            this.cLanguage.Name = "cLanguage";
            // 
            // cDescription
            // 
            this.cDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cDescription.DataPropertyName = "Description";
            resources.ApplyResources(this.cDescription, "cDescription");
            this.cDescription.Name = "cDescription";
            ((System.ComponentModel.ISupportInitialize)(this.dgvVoices)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lLanguage;
        private ExControls.ExComboBox cbLanguage;
        private System.Windows.Forms.Label lGender;
        private ExControls.ExComboBox cbGender;
        private System.Windows.Forms.Label lSearch;
        private ExControls.ExTextBox tbSearch;
        private ExControls.ExButton bSearch;
        private System.Windows.Forms.DataGridView dgvVoices;
        private ExControls.ExButton bPreview;
        private ExControls.ExButton bAdd;
        private ExControls.ExButton bMore;
        private System.Windows.Forms.Label lStatus;
        private ExControls.ExButton bClose;
        private System.Windows.Forms.DataGridViewTextBoxColumn cName;
        private System.Windows.Forms.DataGridViewTextBoxColumn cGender;
        private System.Windows.Forms.DataGridViewTextBoxColumn cAge;
        private System.Windows.Forms.DataGridViewTextBoxColumn cAccent;
        private System.Windows.Forms.DataGridViewTextBoxColumn cLanguage;
        private System.Windows.Forms.DataGridViewTextBoxColumn cDescription;
    }
}
