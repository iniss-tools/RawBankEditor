namespace RawBankEditor.Forms
{
    partial class FProsody
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FProsody));
            this.lHint = new System.Windows.Forms.Label();
            this.dgvGroups = new System.Windows.Forms.DataGridView();
            this.bDefault = new ExControls.ExButton();
            this.bTry = new ExControls.ExButton();
            this.lStatus = new System.Windows.Forms.Label();
            this.bOK = new ExControls.ExButton();
            this.bCancel = new ExControls.ExButton();
            this.cGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cTechnique = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.cPunctuation = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.cBefore = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cAfter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGroups)).BeginInit();
            this.SuspendLayout();
            // 
            // lHint
            // 
            resources.ApplyResources(this.lHint, "lHint");
            this.lHint.Name = "lHint";
            // 
            // dgvGroups
            // 
            this.dgvGroups.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cGroup,
            this.cType,
            this.cTechnique,
            this.cPunctuation,
            this.cBefore,
            this.cAfter});
            resources.ApplyResources(this.dgvGroups, "dgvGroups");
            this.dgvGroups.AllowUserToAddRows = false;
            this.dgvGroups.AllowUserToDeleteRows = false;
            this.dgvGroups.AllowUserToResizeRows = false;
            this.dgvGroups.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvGroups.RowHeadersVisible = false;
            this.dgvGroups.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvGroups.Name = "dgvGroups";
            this.dgvGroups.CurrentCellDirtyStateChanged += new System.EventHandler(this.DgvGroups_CurrentCellDirtyStateChanged);
            this.dgvGroups.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.DgvGroups_DataError);
            // 
            // bDefault
            // 
            resources.ApplyResources(this.bDefault, "bDefault");
            this.bDefault.UseVisualStyleBackColor = true;
            this.bDefault.Name = "bDefault";
            this.bDefault.Click += new System.EventHandler(this.BDefault_Click);
            // 
            // bTry
            // 
            resources.ApplyResources(this.bTry, "bTry");
            this.bTry.UseVisualStyleBackColor = true;
            this.bTry.Name = "bTry";
            this.bTry.Click += new System.EventHandler(this.BTry_Click);
            // 
            // lStatus
            // 
            resources.ApplyResources(this.lStatus, "lStatus");
            this.lStatus.Name = "lStatus";
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
            // FProsody
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.lHint);
            this.Controls.Add(this.dgvGroups);
            this.Controls.Add(this.bDefault);
            this.Controls.Add(this.bTry);
            this.Controls.Add(this.lStatus);
            this.Controls.Add(this.bOK);
            this.Controls.Add(this.bCancel);
            this.AcceptButton = this.bOK;
            this.CancelButton = this.bCancel;
            this.MinimizeBox = false;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Name = "FProsody";
            // 
            // cGroup
            // 
            this.cGroup.DataPropertyName = "GroupKey";
            this.cGroup.ReadOnly = true;
            resources.ApplyResources(this.cGroup, "cGroup");
            this.cGroup.Name = "cGroup";
            // 
            // cType
            // 
            this.cType.DataPropertyName = "TypeName";
            this.cType.ReadOnly = true;
            resources.ApplyResources(this.cType, "cType");
            this.cType.Name = "cType";
            // 
            // cTechnique
            // 
            this.cTechnique.DataPropertyName = "Technique";
            this.cTechnique.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.cTechnique.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            resources.ApplyResources(this.cTechnique, "cTechnique");
            this.cTechnique.Name = "cTechnique";
            // 
            // cPunctuation
            // 
            this.cPunctuation.DataPropertyName = "Punctuation";
            this.cPunctuation.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.cPunctuation.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            resources.ApplyResources(this.cPunctuation, "cPunctuation");
            this.cPunctuation.Name = "cPunctuation";
            // 
            // cBefore
            // 
            this.cBefore.DataPropertyName = "Before";
            resources.ApplyResources(this.cBefore, "cBefore");
            this.cBefore.Name = "cBefore";
            // 
            // cAfter
            // 
            this.cAfter.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cAfter.DataPropertyName = "After";
            resources.ApplyResources(this.cAfter, "cAfter");
            this.cAfter.Name = "cAfter";
            ((System.ComponentModel.ISupportInitialize)(this.dgvGroups)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lHint;
        private System.Windows.Forms.DataGridView dgvGroups;
        private ExControls.ExButton bDefault;
        private ExControls.ExButton bTry;
        private System.Windows.Forms.Label lStatus;
        private ExControls.ExButton bOK;
        private ExControls.ExButton bCancel;
        private System.Windows.Forms.DataGridViewTextBoxColumn cGroup;
        private System.Windows.Forms.DataGridViewTextBoxColumn cType;
        private System.Windows.Forms.DataGridViewComboBoxColumn cTechnique;
        private System.Windows.Forms.DataGridViewComboBoxColumn cPunctuation;
        private System.Windows.Forms.DataGridViewTextBoxColumn cBefore;
        private System.Windows.Forms.DataGridViewTextBoxColumn cAfter;
    }
}
