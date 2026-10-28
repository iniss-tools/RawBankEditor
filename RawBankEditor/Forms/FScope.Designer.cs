namespace RawBankEditor.Forms
{
    partial class FScope
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FScope));
            this.lHint = new System.Windows.Forms.Label();
            this.rbSelected = new ExControls.ExRadioButton();
            this.rbGroup = new ExControls.ExRadioButton();
            this.rbLanguage = new ExControls.ExRadioButton();
            this.bOK = new ExControls.ExButton();
            this.bCancel = new ExControls.ExButton();
            this.SuspendLayout();
            // 
            // lHint
            // 
            resources.ApplyResources(this.lHint, "lHint");
            this.lHint.Name = "lHint";
            // 
            // rbSelected
            // 
            resources.ApplyResources(this.rbSelected, "rbSelected");
            this.rbSelected.TabStop = true;
            this.rbSelected.Name = "rbSelected";
            // 
            // rbGroup
            // 
            resources.ApplyResources(this.rbGroup, "rbGroup");
            this.rbGroup.TabStop = true;
            this.rbGroup.Name = "rbGroup";
            // 
            // rbLanguage
            // 
            resources.ApplyResources(this.rbLanguage, "rbLanguage");
            this.rbLanguage.TabStop = true;
            this.rbLanguage.Name = "rbLanguage";
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
            // FScope
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.lHint);
            this.Controls.Add(this.rbSelected);
            this.Controls.Add(this.rbGroup);
            this.Controls.Add(this.rbLanguage);
            this.Controls.Add(this.bOK);
            this.Controls.Add(this.bCancel);
            this.AcceptButton = this.bOK;
            this.CancelButton = this.bCancel;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Name = "FScope";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lHint;
        private ExControls.ExRadioButton rbSelected;
        private ExControls.ExRadioButton rbGroup;
        private ExControls.ExRadioButton rbLanguage;
        private ExControls.ExButton bOK;
        private ExControls.ExButton bCancel;
    }
}
