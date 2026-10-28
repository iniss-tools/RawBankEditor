namespace RawBankEditor.Forms
{
    partial class FAdjustSounds
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FAdjustSounds));
            this.gbScope = new ExControls.ExGroupBox();
            this.rbSelected = new ExControls.ExRadioButton();
            this.rbGroup = new ExControls.ExRadioButton();
            this.rbLanguage = new ExControls.ExRadioButton();
            this.gbSilence = new ExControls.ExGroupBox();
            this.cboxTrim = new ExControls.ExCheckBox();
            this.lLead = new System.Windows.Forms.Label();
            this.nudLead = new ExControls.ExNumericUpDown();
            this.lTail = new System.Windows.Forms.Label();
            this.nudTail = new ExControls.ExNumericUpDown();
            this.lThreshold = new System.Windows.Forms.Label();
            this.nudThreshold = new ExControls.ExNumericUpDown();
            this.gbLevel = new ExControls.ExGroupBox();
            this.cboxNormalize = new ExControls.ExCheckBox();
            this.nudLevel = new ExControls.ExNumericUpDown();
            this.bMeasure = new ExControls.ExButton();
            this.lMeasure = new System.Windows.Forms.Label();
            this.gbFormat = new ExControls.ExGroupBox();
            this.cboxConvert = new ExControls.ExCheckBox();
            this.lFade = new System.Windows.Forms.Label();
            this.nudFade = new ExControls.ExNumericUpDown();
            this.bPreview = new ExControls.ExButton();
            this.bOK = new ExControls.ExButton();
            this.bCancel = new ExControls.ExButton();
            this.gbScope.SuspendLayout();
            this.gbSilence.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLead)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTail)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudThreshold)).BeginInit();
            this.gbLevel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLevel)).BeginInit();
            this.gbFormat.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFade)).BeginInit();
            this.SuspendLayout();
            // 
            // gbScope
            // 
            this.gbScope.Controls.Add(this.rbSelected);
            this.gbScope.Controls.Add(this.rbGroup);
            this.gbScope.Controls.Add(this.rbLanguage);
            resources.ApplyResources(this.gbScope, "gbScope");
            this.gbScope.TabStop = false;
            this.gbScope.Name = "gbScope";
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
            // gbSilence
            // 
            this.gbSilence.Controls.Add(this.cboxTrim);
            this.gbSilence.Controls.Add(this.lLead);
            this.gbSilence.Controls.Add(this.nudLead);
            this.gbSilence.Controls.Add(this.lTail);
            this.gbSilence.Controls.Add(this.nudTail);
            this.gbSilence.Controls.Add(this.lThreshold);
            this.gbSilence.Controls.Add(this.nudThreshold);
            resources.ApplyResources(this.gbSilence, "gbSilence");
            this.gbSilence.TabStop = false;
            this.gbSilence.Name = "gbSilence";
            // 
            // cboxTrim
            // 
            resources.ApplyResources(this.cboxTrim, "cboxTrim");
            this.cboxTrim.Name = "cboxTrim";
            this.cboxTrim.CheckedChanged += new System.EventHandler(this.OptionChanged);
            // 
            // lLead
            // 
            resources.ApplyResources(this.lLead, "lLead");
            this.lLead.Name = "lLead";
            // 
            // nudLead
            // 
            resources.ApplyResources(this.nudLead, "nudLead");
            this.nudLead.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudLead.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.nudLead.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudLead.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.nudLead.Name = "nudLead";
            // 
            // lTail
            // 
            resources.ApplyResources(this.lTail, "lTail");
            this.lTail.Name = "lTail";
            // 
            // nudTail
            // 
            resources.ApplyResources(this.nudTail, "nudTail");
            this.nudTail.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudTail.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.nudTail.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudTail.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            this.nudTail.Name = "nudTail";
            // 
            // lThreshold
            // 
            resources.ApplyResources(this.lThreshold, "lThreshold");
            this.lThreshold.Name = "lThreshold";
            // 
            // nudThreshold
            // 
            resources.ApplyResources(this.nudThreshold, "nudThreshold");
            this.nudThreshold.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.nudThreshold.Minimum = new decimal(new int[] {
            90,
            0,
            0,
            -2147483648});
            this.nudThreshold.Value = new decimal(new int[] {
            45,
            0,
            0,
            -2147483648});
            this.nudThreshold.Name = "nudThreshold";
            // 
            // gbLevel
            // 
            this.gbLevel.Controls.Add(this.cboxNormalize);
            this.gbLevel.Controls.Add(this.nudLevel);
            this.gbLevel.Controls.Add(this.bMeasure);
            this.gbLevel.Controls.Add(this.lMeasure);
            resources.ApplyResources(this.gbLevel, "gbLevel");
            this.gbLevel.TabStop = false;
            this.gbLevel.Name = "gbLevel";
            // 
            // cboxNormalize
            // 
            resources.ApplyResources(this.cboxNormalize, "cboxNormalize");
            this.cboxNormalize.Name = "cboxNormalize";
            this.cboxNormalize.CheckedChanged += new System.EventHandler(this.OptionChanged);
            // 
            // nudLevel
            // 
            resources.ApplyResources(this.nudLevel, "nudLevel");
            this.nudLevel.DecimalPlaces = 1;
            this.nudLevel.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.nudLevel.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            -2147483648});
            this.nudLevel.Minimum = new decimal(new int[] {
            60,
            0,
            0,
            -2147483648});
            this.nudLevel.Value = new decimal(new int[] {
            20,
            0,
            0,
            -2147483648});
            this.nudLevel.Name = "nudLevel";
            // 
            // bMeasure
            // 
            resources.ApplyResources(this.bMeasure, "bMeasure");
            this.bMeasure.UseVisualStyleBackColor = true;
            this.bMeasure.Name = "bMeasure";
            this.bMeasure.Click += new System.EventHandler(this.BMeasure_Click);
            // 
            // lMeasure
            // 
            resources.ApplyResources(this.lMeasure, "lMeasure");
            this.lMeasure.Name = "lMeasure";
            // 
            // gbFormat
            // 
            this.gbFormat.Controls.Add(this.cboxConvert);
            this.gbFormat.Controls.Add(this.lFade);
            this.gbFormat.Controls.Add(this.nudFade);
            resources.ApplyResources(this.gbFormat, "gbFormat");
            this.gbFormat.TabStop = false;
            this.gbFormat.Name = "gbFormat";
            // 
            // cboxConvert
            // 
            resources.ApplyResources(this.cboxConvert, "cboxConvert");
            this.cboxConvert.Name = "cboxConvert";
            // 
            // lFade
            // 
            resources.ApplyResources(this.lFade, "lFade");
            this.lFade.Name = "lFade";
            // 
            // nudFade
            // 
            resources.ApplyResources(this.nudFade, "nudFade");
            this.nudFade.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nudFade.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.nudFade.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudFade.Name = "nudFade";
            // 
            // bPreview
            // 
            resources.ApplyResources(this.bPreview, "bPreview");
            this.bPreview.UseVisualStyleBackColor = true;
            this.bPreview.Name = "bPreview";
            this.bPreview.Click += new System.EventHandler(this.BPreview_Click);
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
            // FAdjustSounds
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.gbScope);
            this.Controls.Add(this.gbSilence);
            this.Controls.Add(this.gbLevel);
            this.Controls.Add(this.gbFormat);
            this.Controls.Add(this.bPreview);
            this.Controls.Add(this.bOK);
            this.Controls.Add(this.bCancel);
            this.AcceptButton = this.bOK;
            this.CancelButton = this.bCancel;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Name = "FAdjustSounds";
            this.gbScope.ResumeLayout(false);
            this.gbScope.PerformLayout();
            this.gbSilence.ResumeLayout(false);
            this.gbSilence.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLead)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTail)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudThreshold)).EndInit();
            this.gbLevel.ResumeLayout(false);
            this.gbLevel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudLevel)).EndInit();
            this.gbFormat.ResumeLayout(false);
            this.gbFormat.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudFade)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ExControls.ExGroupBox gbScope;
        private ExControls.ExRadioButton rbSelected;
        private ExControls.ExRadioButton rbGroup;
        private ExControls.ExRadioButton rbLanguage;
        private ExControls.ExGroupBox gbSilence;
        private ExControls.ExCheckBox cboxTrim;
        private System.Windows.Forms.Label lLead;
        private ExControls.ExNumericUpDown nudLead;
        private System.Windows.Forms.Label lTail;
        private ExControls.ExNumericUpDown nudTail;
        private System.Windows.Forms.Label lThreshold;
        private ExControls.ExNumericUpDown nudThreshold;
        private ExControls.ExGroupBox gbLevel;
        private ExControls.ExCheckBox cboxNormalize;
        private ExControls.ExNumericUpDown nudLevel;
        private ExControls.ExButton bMeasure;
        private System.Windows.Forms.Label lMeasure;
        private ExControls.ExGroupBox gbFormat;
        private ExControls.ExCheckBox cboxConvert;
        private System.Windows.Forms.Label lFade;
        private ExControls.ExNumericUpDown nudFade;
        private ExControls.ExButton bPreview;
        private ExControls.ExButton bOK;
        private ExControls.ExButton bCancel;
    }
}
