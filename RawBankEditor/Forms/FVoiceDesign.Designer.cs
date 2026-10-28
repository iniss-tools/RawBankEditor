namespace RawBankEditor.Forms
{
    partial class FVoiceDesign
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FVoiceDesign));
            this.lDescription = new System.Windows.Forms.Label();
            this.tbDescription = new ExControls.ExTextBox();
            this.lModel = new System.Windows.Forms.Label();
            this.cbModel = new ExControls.ExComboBox();
            this.lText = new System.Windows.Forms.Label();
            this.tbText = new ExControls.ExTextBox();
            this.bDesign = new ExControls.ExButton();
            this.lbPreviews = new ExControls.ExListBox();
            this.bPlay = new ExControls.ExButton();
            this.lName = new System.Windows.Forms.Label();
            this.tbName = new ExControls.ExTextBox();
            this.bSave = new ExControls.ExButton();
            this.lStatus = new System.Windows.Forms.Label();
            this.bClose = new ExControls.ExButton();
            this.SuspendLayout();
            // 
            // lDescription
            // 
            resources.ApplyResources(this.lDescription, "lDescription");
            this.lDescription.Name = "lDescription";
            // 
            // tbDescription
            // 
            resources.ApplyResources(this.tbDescription, "tbDescription");
            this.tbDescription.Multiline = true;
            this.tbDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbDescription.Name = "tbDescription";
            // 
            // lModel
            // 
            resources.ApplyResources(this.lModel, "lModel");
            this.lModel.Name = "lModel";
            // 
            // cbModel
            // 
            resources.ApplyResources(this.cbModel, "cbModel");
            this.cbModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbModel.FormattingEnabled = true;
            this.cbModel.Name = "cbModel";
            // 
            // lText
            // 
            resources.ApplyResources(this.lText, "lText");
            this.lText.Name = "lText";
            // 
            // tbText
            // 
            resources.ApplyResources(this.tbText, "tbText");
            this.tbText.Multiline = true;
            this.tbText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbText.Name = "tbText";
            // 
            // bDesign
            // 
            resources.ApplyResources(this.bDesign, "bDesign");
            this.bDesign.UseVisualStyleBackColor = true;
            this.bDesign.Name = "bDesign";
            this.bDesign.Click += new System.EventHandler(this.BDesign_Click);
            // 
            // lbPreviews
            // 
            resources.ApplyResources(this.lbPreviews, "lbPreviews");
            this.lbPreviews.FormattingEnabled = true;
            this.lbPreviews.Name = "lbPreviews";
            this.lbPreviews.DoubleClick += new System.EventHandler(this.BPlay_Click);
            // 
            // bPlay
            // 
            resources.ApplyResources(this.bPlay, "bPlay");
            this.bPlay.UseVisualStyleBackColor = true;
            this.bPlay.Name = "bPlay";
            this.bPlay.Click += new System.EventHandler(this.BPlay_Click);
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
            // bSave
            // 
            resources.ApplyResources(this.bSave, "bSave");
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Name = "bSave";
            this.bSave.Click += new System.EventHandler(this.BSave_Click);
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
            // FVoiceDesign
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.lDescription);
            this.Controls.Add(this.tbDescription);
            this.Controls.Add(this.lModel);
            this.Controls.Add(this.cbModel);
            this.Controls.Add(this.lText);
            this.Controls.Add(this.tbText);
            this.Controls.Add(this.bDesign);
            this.Controls.Add(this.lbPreviews);
            this.Controls.Add(this.bPlay);
            this.Controls.Add(this.lName);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.bSave);
            this.Controls.Add(this.lStatus);
            this.Controls.Add(this.bClose);
            this.CancelButton = this.bClose;
            this.MinimizeBox = false;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Name = "FVoiceDesign";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lDescription;
        private ExControls.ExTextBox tbDescription;
        private System.Windows.Forms.Label lModel;
        private ExControls.ExComboBox cbModel;
        private System.Windows.Forms.Label lText;
        private ExControls.ExTextBox tbText;
        private ExControls.ExButton bDesign;
        private ExControls.ExListBox lbPreviews;
        private ExControls.ExButton bPlay;
        private System.Windows.Forms.Label lName;
        private ExControls.ExTextBox tbName;
        private ExControls.ExButton bSave;
        private System.Windows.Forms.Label lStatus;
        private ExControls.ExButton bClose;
    }
}
