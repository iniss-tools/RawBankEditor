namespace RawBankEditor.Forms
{
    partial class FVoiceClone
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FVoiceClone));
            this.lHint = new System.Windows.Forms.Label();
            this.lName = new System.Windows.Forms.Label();
            this.tbName = new ExControls.ExTextBox();
            this.lDescription = new System.Windows.Forms.Label();
            this.tbDescription = new ExControls.ExTextBox();
            this.lSource = new System.Windows.Forms.Label();
            this.lSourceInfo = new System.Windows.Forms.Label();
            this.cboxRemoveNoise = new ExControls.ExCheckBox();
            this.cboxConsent = new ExControls.ExCheckBox();
            this.lStatus = new System.Windows.Forms.Label();
            this.bClone = new ExControls.ExButton();
            this.bCancel = new ExControls.ExButton();
            this.SuspendLayout();
            // 
            // lHint
            // 
            resources.ApplyResources(this.lHint, "lHint");
            this.lHint.Name = "lHint";
            // 
            // lName
            // 
            resources.ApplyResources(this.lName, "lName");
            this.lName.Name = "lName";
            // 
            // tbName
            // 
            resources.ApplyResources(this.tbName, "tbName");
            this.tbName.Name = "tbName";
            // 
            // lDescription
            // 
            resources.ApplyResources(this.lDescription, "lDescription");
            this.lDescription.Name = "lDescription";
            // 
            // tbDescription
            // 
            resources.ApplyResources(this.tbDescription, "tbDescription");
            this.tbDescription.Name = "tbDescription";
            // 
            // lSource
            // 
            resources.ApplyResources(this.lSource, "lSource");
            this.lSource.Name = "lSource";
            // 
            // lSourceInfo
            // 
            resources.ApplyResources(this.lSourceInfo, "lSourceInfo");
            this.lSourceInfo.Name = "lSourceInfo";
            // 
            // cboxRemoveNoise
            // 
            resources.ApplyResources(this.cboxRemoveNoise, "cboxRemoveNoise");
            this.cboxRemoveNoise.Name = "cboxRemoveNoise";
            // 
            // cboxConsent
            // 
            resources.ApplyResources(this.cboxConsent, "cboxConsent");
            this.cboxConsent.Name = "cboxConsent";
            this.cboxConsent.CheckedChanged += new System.EventHandler(this.CboxConsent_CheckedChanged);
            // 
            // lStatus
            // 
            resources.ApplyResources(this.lStatus, "lStatus");
            this.lStatus.Name = "lStatus";
            // 
            // bClone
            // 
            resources.ApplyResources(this.bClone, "bClone");
            this.bClone.UseVisualStyleBackColor = true;
            this.bClone.Name = "bClone";
            this.bClone.Click += new System.EventHandler(this.BClone_Click);
            // 
            // bCancel
            // 
            resources.ApplyResources(this.bCancel, "bCancel");
            this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancel.UseVisualStyleBackColor = true;
            this.bCancel.Name = "bCancel";
            // 
            // FVoiceClone
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.lHint);
            this.Controls.Add(this.lName);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.lDescription);
            this.Controls.Add(this.tbDescription);
            this.Controls.Add(this.lSource);
            this.Controls.Add(this.lSourceInfo);
            this.Controls.Add(this.cboxRemoveNoise);
            this.Controls.Add(this.cboxConsent);
            this.Controls.Add(this.lStatus);
            this.Controls.Add(this.bClone);
            this.Controls.Add(this.bCancel);
            this.AcceptButton = this.bClone;
            this.CancelButton = this.bCancel;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Name = "FVoiceClone";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lHint;
        private System.Windows.Forms.Label lName;
        private ExControls.ExTextBox tbName;
        private System.Windows.Forms.Label lDescription;
        private ExControls.ExTextBox tbDescription;
        private System.Windows.Forms.Label lSource;
        private System.Windows.Forms.Label lSourceInfo;
        private ExControls.ExCheckBox cboxRemoveNoise;
        private ExControls.ExCheckBox cboxConsent;
        private System.Windows.Forms.Label lStatus;
        private ExControls.ExButton bClone;
        private ExControls.ExButton bCancel;
    }
}
