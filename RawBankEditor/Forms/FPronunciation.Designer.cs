namespace RawBankEditor.Forms
{
    partial class FPronunciation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FPronunciation));
            this.lHint = new System.Windows.Forms.Label();
            this.dgvRules = new System.Windows.Forms.DataGridView();
            this.bUp = new ExControls.ExButton();
            this.bDown = new ExControls.ExButton();
            this.lTest = new System.Windows.Forms.Label();
            this.tbTest = new ExControls.ExTextBox();
            this.bTry = new ExControls.ExButton();
            this.lResultCaption = new System.Windows.Forms.Label();
            this.lResult = new System.Windows.Forms.Label();
            this.lStatus = new System.Windows.Forms.Label();
            this.bOK = new ExControls.ExButton();
            this.bCancel = new ExControls.ExButton();
            this.cFind = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cReplace = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cWholeWord = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.cMatchCase = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.cRegex = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRules)).BeginInit();
            this.SuspendLayout();
            // 
            // lHint
            // 
            resources.ApplyResources(this.lHint, "lHint");
            this.lHint.Name = "lHint";
            // 
            // dgvRules
            // 
            this.dgvRules.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cFind,
            this.cReplace,
            this.cWholeWord,
            this.cMatchCase,
            this.cRegex});
            resources.ApplyResources(this.dgvRules, "dgvRules");
            this.dgvRules.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRules.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvRules.Name = "dgvRules";
            this.dgvRules.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.DgvRules_CellEndEdit);
            this.dgvRules.CurrentCellDirtyStateChanged += new System.EventHandler(this.DgvRules_CurrentCellDirtyStateChanged);
            this.dgvRules.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.DgvRules_UserDeletedRow);
            // 
            // bUp
            // 
            resources.ApplyResources(this.bUp, "bUp");
            this.bUp.UseVisualStyleBackColor = true;
            this.bUp.Name = "bUp";
            this.bUp.Click += new System.EventHandler(this.BUp_Click);
            // 
            // bDown
            // 
            resources.ApplyResources(this.bDown, "bDown");
            this.bDown.UseVisualStyleBackColor = true;
            this.bDown.Name = "bDown";
            this.bDown.Click += new System.EventHandler(this.BDown_Click);
            // 
            // lTest
            // 
            resources.ApplyResources(this.lTest, "lTest");
            this.lTest.Name = "lTest";
            // 
            // tbTest
            // 
            resources.ApplyResources(this.tbTest, "tbTest");
            this.tbTest.Name = "tbTest";
            this.tbTest.TextChanged += new System.EventHandler(this.TbTest_TextChanged);
            // 
            // bTry
            // 
            resources.ApplyResources(this.bTry, "bTry");
            this.bTry.UseVisualStyleBackColor = true;
            this.bTry.Name = "bTry";
            this.bTry.Click += new System.EventHandler(this.BTry_Click);
            // 
            // lResultCaption
            // 
            resources.ApplyResources(this.lResultCaption, "lResultCaption");
            this.lResultCaption.Name = "lResultCaption";
            // 
            // lResult
            // 
            resources.ApplyResources(this.lResult, "lResult");
            this.lResult.Name = "lResult";
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
            // FPronunciation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.lHint);
            this.Controls.Add(this.dgvRules);
            this.Controls.Add(this.bUp);
            this.Controls.Add(this.bDown);
            this.Controls.Add(this.lTest);
            this.Controls.Add(this.tbTest);
            this.Controls.Add(this.bTry);
            this.Controls.Add(this.lResultCaption);
            this.Controls.Add(this.lResult);
            this.Controls.Add(this.lStatus);
            this.Controls.Add(this.bOK);
            this.Controls.Add(this.bCancel);
            this.AcceptButton = this.bOK;
            this.CancelButton = this.bCancel;
            this.MinimizeBox = false;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Name = "FPronunciation";
            // 
            // cFind
            // 
            this.cFind.DataPropertyName = "Find";
            resources.ApplyResources(this.cFind, "cFind");
            this.cFind.Name = "cFind";
            // 
            // cReplace
            // 
            this.cReplace.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cReplace.DataPropertyName = "Replace";
            resources.ApplyResources(this.cReplace, "cReplace");
            this.cReplace.Name = "cReplace";
            // 
            // cWholeWord
            // 
            this.cWholeWord.DataPropertyName = "WholeWord";
            resources.ApplyResources(this.cWholeWord, "cWholeWord");
            this.cWholeWord.Name = "cWholeWord";
            // 
            // cMatchCase
            // 
            this.cMatchCase.DataPropertyName = "MatchCase";
            resources.ApplyResources(this.cMatchCase, "cMatchCase");
            this.cMatchCase.Name = "cMatchCase";
            // 
            // cRegex
            // 
            this.cRegex.DataPropertyName = "Regex";
            resources.ApplyResources(this.cRegex, "cRegex");
            this.cRegex.Name = "cRegex";
            ((System.ComponentModel.ISupportInitialize)(this.dgvRules)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lHint;
        private System.Windows.Forms.DataGridView dgvRules;
        private ExControls.ExButton bUp;
        private ExControls.ExButton bDown;
        private System.Windows.Forms.Label lTest;
        private ExControls.ExTextBox tbTest;
        private ExControls.ExButton bTry;
        private System.Windows.Forms.Label lResultCaption;
        private System.Windows.Forms.Label lResult;
        private System.Windows.Forms.Label lStatus;
        private ExControls.ExButton bOK;
        private ExControls.ExButton bCancel;
        private System.Windows.Forms.DataGridViewTextBoxColumn cFind;
        private System.Windows.Forms.DataGridViewTextBoxColumn cReplace;
        private System.Windows.Forms.DataGridViewCheckBoxColumn cWholeWord;
        private System.Windows.Forms.DataGridViewCheckBoxColumn cMatchCase;
        private System.Windows.Forms.DataGridViewCheckBoxColumn cRegex;
    }
}
