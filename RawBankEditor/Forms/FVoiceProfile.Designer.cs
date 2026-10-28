namespace RawBankEditor.Forms
{
    partial class FVoiceProfile
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FVoiceProfile));
            this.lHint = new System.Windows.Forms.Label();
            this.lModel = new System.Windows.Forms.Label();
            this.cbModel = new ExControls.ExComboBox();
            this.lVoice = new System.Windows.Forms.Label();
            this.cbVoice = new ExControls.ExComboBox();
            this.bReload = new ExControls.ExButton();
            this.lLanguage = new System.Windows.Forms.Label();
            this.cbLanguage = new ExControls.ExComboBox();
            this.gbSettings = new ExControls.ExGroupBox();
            this.lStability = new System.Windows.Forms.Label();
            this.nudStability = new ExControls.ExNumericUpDown();
            this.lSimilarity = new System.Windows.Forms.Label();
            this.nudSimilarity = new ExControls.ExNumericUpDown();
            this.lStyle = new System.Windows.Forms.Label();
            this.nudStyle = new ExControls.ExNumericUpDown();
            this.lSpeed = new System.Windows.Forms.Label();
            this.nudSpeed = new ExControls.ExNumericUpDown();
            this.cboxSpeakerBoost = new ExControls.ExCheckBox();
            this.cboxSeed = new ExControls.ExCheckBox();
            this.nudSeed = new ExControls.ExNumericUpDown();
            this.lSettingsHint = new System.Windows.Forms.Label();
            this.lSample = new System.Windows.Forms.Label();
            this.tbSample = new ExControls.ExTextBox();
            this.bTry = new ExControls.ExButton();
            this.lStatus = new System.Windows.Forms.Label();
            this.bOK = new ExControls.ExButton();
            this.bCancel = new ExControls.ExButton();
            this.gbSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudStability)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSimilarity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStyle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSeed)).BeginInit();
            this.SuspendLayout();
            // 
            // lHint
            // 
            resources.ApplyResources(this.lHint, "lHint");
            this.lHint.Name = "lHint";
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
            // lVoice
            // 
            resources.ApplyResources(this.lVoice, "lVoice");
            this.lVoice.Name = "lVoice";
            // 
            // cbVoice
            // 
            resources.ApplyResources(this.cbVoice, "cbVoice");
            this.cbVoice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVoice.FormattingEnabled = true;
            this.cbVoice.Name = "cbVoice";
            // 
            // bReload
            // 
            resources.ApplyResources(this.bReload, "bReload");
            this.bReload.UseVisualStyleBackColor = true;
            this.bReload.Name = "bReload";
            this.bReload.Click += new System.EventHandler(this.BReload_Click);
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
            // gbSettings
            // 
            this.gbSettings.Controls.Add(this.lStability);
            this.gbSettings.Controls.Add(this.nudStability);
            this.gbSettings.Controls.Add(this.lSimilarity);
            this.gbSettings.Controls.Add(this.nudSimilarity);
            this.gbSettings.Controls.Add(this.lStyle);
            this.gbSettings.Controls.Add(this.nudStyle);
            this.gbSettings.Controls.Add(this.lSpeed);
            this.gbSettings.Controls.Add(this.nudSpeed);
            this.gbSettings.Controls.Add(this.cboxSpeakerBoost);
            this.gbSettings.Controls.Add(this.cboxSeed);
            this.gbSettings.Controls.Add(this.nudSeed);
            this.gbSettings.Controls.Add(this.lSettingsHint);
            resources.ApplyResources(this.gbSettings, "gbSettings");
            this.gbSettings.TabStop = false;
            this.gbSettings.Name = "gbSettings";
            // 
            // lStability
            // 
            resources.ApplyResources(this.lStability, "lStability");
            this.lStability.Name = "lStability";
            // 
            // nudStability
            // 
            resources.ApplyResources(this.nudStability, "nudStability");
            this.nudStability.DecimalPlaces = 2;
            this.nudStability.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.nudStability.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudStability.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudStability.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.nudStability.Name = "nudStability";
            // 
            // lSimilarity
            // 
            resources.ApplyResources(this.lSimilarity, "lSimilarity");
            this.lSimilarity.Name = "lSimilarity";
            // 
            // nudSimilarity
            // 
            resources.ApplyResources(this.nudSimilarity, "nudSimilarity");
            this.nudSimilarity.DecimalPlaces = 2;
            this.nudSimilarity.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.nudSimilarity.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSimilarity.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudSimilarity.Value = new decimal(new int[] {
            75,
            0,
            0,
            131072});
            this.nudSimilarity.Name = "nudSimilarity";
            // 
            // lStyle
            // 
            resources.ApplyResources(this.lStyle, "lStyle");
            this.lStyle.Name = "lStyle";
            // 
            // nudStyle
            // 
            resources.ApplyResources(this.nudStyle, "nudStyle");
            this.nudStyle.DecimalPlaces = 2;
            this.nudStyle.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.nudStyle.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudStyle.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudStyle.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudStyle.Name = "nudStyle";
            // 
            // lSpeed
            // 
            resources.ApplyResources(this.lSpeed, "lSpeed");
            this.lSpeed.Name = "lSpeed";
            // 
            // nudSpeed
            // 
            resources.ApplyResources(this.nudSpeed, "nudSpeed");
            this.nudSpeed.DecimalPlaces = 2;
            this.nudSpeed.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.nudSpeed.Maximum = new decimal(new int[] {
            12,
            0,
            0,
            65536});
            this.nudSpeed.Minimum = new decimal(new int[] {
            7,
            0,
            0,
            65536});
            this.nudSpeed.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSpeed.Name = "nudSpeed";
            // 
            // cboxSpeakerBoost
            // 
            resources.ApplyResources(this.cboxSpeakerBoost, "cboxSpeakerBoost");
            this.cboxSpeakerBoost.Checked = true;
            this.cboxSpeakerBoost.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cboxSpeakerBoost.Name = "cboxSpeakerBoost";
            // 
            // cboxSeed
            // 
            resources.ApplyResources(this.cboxSeed, "cboxSeed");
            this.cboxSeed.Name = "cboxSeed";
            this.cboxSeed.CheckedChanged += new System.EventHandler(this.CboxSeed_CheckedChanged);
            // 
            // nudSeed
            // 
            resources.ApplyResources(this.nudSeed, "nudSeed");
            this.nudSeed.Maximum = new decimal(new int[] {
            -1,
            0,
            0,
            0});
            this.nudSeed.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudSeed.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSeed.Name = "nudSeed";
            // 
            // lSettingsHint
            // 
            resources.ApplyResources(this.lSettingsHint, "lSettingsHint");
            this.lSettingsHint.Name = "lSettingsHint";
            // 
            // lSample
            // 
            resources.ApplyResources(this.lSample, "lSample");
            this.lSample.Name = "lSample";
            // 
            // tbSample
            // 
            resources.ApplyResources(this.tbSample, "tbSample");
            this.tbSample.Name = "tbSample";
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
            // FVoiceProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.lHint);
            this.Controls.Add(this.lModel);
            this.Controls.Add(this.cbModel);
            this.Controls.Add(this.lVoice);
            this.Controls.Add(this.cbVoice);
            this.Controls.Add(this.bReload);
            this.Controls.Add(this.lLanguage);
            this.Controls.Add(this.cbLanguage);
            this.Controls.Add(this.gbSettings);
            this.Controls.Add(this.lSample);
            this.Controls.Add(this.tbSample);
            this.Controls.Add(this.bTry);
            this.Controls.Add(this.lStatus);
            this.Controls.Add(this.bOK);
            this.Controls.Add(this.bCancel);
            this.AcceptButton = this.bOK;
            this.CancelButton = this.bCancel;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Name = "FVoiceProfile";
            this.gbSettings.ResumeLayout(false);
            this.gbSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudStability)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSimilarity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudStyle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSeed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lHint;
        private System.Windows.Forms.Label lModel;
        private ExControls.ExComboBox cbModel;
        private System.Windows.Forms.Label lVoice;
        private ExControls.ExComboBox cbVoice;
        private ExControls.ExButton bReload;
        private System.Windows.Forms.Label lLanguage;
        private ExControls.ExComboBox cbLanguage;
        private ExControls.ExGroupBox gbSettings;
        private System.Windows.Forms.Label lStability;
        private ExControls.ExNumericUpDown nudStability;
        private System.Windows.Forms.Label lSimilarity;
        private ExControls.ExNumericUpDown nudSimilarity;
        private System.Windows.Forms.Label lStyle;
        private ExControls.ExNumericUpDown nudStyle;
        private System.Windows.Forms.Label lSpeed;
        private ExControls.ExNumericUpDown nudSpeed;
        private ExControls.ExCheckBox cboxSpeakerBoost;
        private ExControls.ExCheckBox cboxSeed;
        private ExControls.ExNumericUpDown nudSeed;
        private System.Windows.Forms.Label lSettingsHint;
        private System.Windows.Forms.Label lSample;
        private ExControls.ExTextBox tbSample;
        private ExControls.ExButton bTry;
        private System.Windows.Forms.Label lStatus;
        private ExControls.ExButton bOK;
        private ExControls.ExButton bCancel;
    }
}
