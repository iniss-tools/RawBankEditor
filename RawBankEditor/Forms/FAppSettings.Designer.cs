namespace RawBankEditor.Forms
{
    partial class FAppSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FAppSettings));
            this.exGroupBox2 = new ExControls.ExGroupBox();
            this.cboxAutoRecalculateSoundDurations = new ExControls.ExCheckBox();
            this.cboxAutoInsertSoundData = new ExControls.ExCheckBox();
            this.cboxShowAfterInsertSoundDlg = new ExControls.ExCheckBox();
            this.pGeneral.SuspendLayout();
            this.pConcreteGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.optionsView)).BeginInit();
            this.pDesktopComponents.SuspendLayout();
            this.pLocalization.SuspendLayout();
            this.exGroupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pConcreteGeneral
            // 
            this.pConcreteGeneral.Controls.Add(this.exGroupBox2);
            resources.ApplyResources(this.pConcreteGeneral, "pConcreteGeneral");
            // 
            // optionsView
            // 
            this.optionsView.HeaderNodeNameVisible = true;
            this.optionsView.SearchBoxVisible = true;
            resources.ApplyResources(this.optionsView, "optionsView");
            // 
            // 
            // 
            this.optionsView.TreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.optionsView.TreeView.FullRowSelect = true;
            this.optionsView.TreeView.HideSelection = false;
            this.optionsView.TreeView.ImageIndex = 0;
            this.optionsView.TreeView.ItemHeight = 20;
            this.optionsView.TreeView.Name = "treeView";
            this.optionsView.TreeView.PathSeparator = " / ";
            this.optionsView.TreeView.SelectedImageIndex = 0;
            this.optionsView.TreeView.ShowLines = false;
            this.optionsView.TreeView.ShowNodeToolTips = true;
            this.optionsView.TreeView.Style = ExControls.ExTreeViewStyle.Light;
            this.optionsView.TreeView.TabIndex = 0;
            // 
            // exGroupBox2
            // 
            resources.ApplyResources(this.exGroupBox2, "exGroupBox2");
            this.exGroupBox2.Controls.Add(this.cboxShowAfterInsertSoundDlg);
            this.exGroupBox2.Controls.Add(this.cboxAutoInsertSoundData);
            this.exGroupBox2.Controls.Add(this.cboxAutoRecalculateSoundDurations);
            this.exGroupBox2.Name = "exGroupBox2";
            this.exGroupBox2.TabStop = false;
            // 
            // cboxAutoRecalculateSoundDurations
            // 
            resources.ApplyResources(this.cboxAutoRecalculateSoundDurations, "cboxAutoRecalculateSoundDurations");
            this.cboxAutoRecalculateSoundDurations.BoxBackColor = System.Drawing.Color.White;
            this.cboxAutoRecalculateSoundDurations.HighlightColor = System.Drawing.SystemColors.Highlight;
            this.cboxAutoRecalculateSoundDurations.Name = "cboxAutoRecalculateSoundDurations";
            // 
            // cboxAutoInsertSoundData
            // 
            resources.ApplyResources(this.cboxAutoInsertSoundData, "cboxAutoInsertSoundData");
            this.cboxAutoInsertSoundData.BoxBackColor = System.Drawing.Color.White;
            this.cboxAutoInsertSoundData.HighlightColor = System.Drawing.SystemColors.Highlight;
            this.cboxAutoInsertSoundData.Name = "cboxAutoInsertSoundData";
            this.cboxAutoInsertSoundData.CheckedChanged += new System.EventHandler(this.CboxAutoInsertSoundData_CheckedChanged);
            // 
            // cboxShowAfterInsertSoundDlg
            // 
            resources.ApplyResources(this.cboxShowAfterInsertSoundDlg, "cboxShowAfterInsertSoundDlg");
            this.cboxShowAfterInsertSoundDlg.BoxBackColor = System.Drawing.Color.White;
            this.cboxShowAfterInsertSoundDlg.HighlightColor = System.Drawing.SystemColors.Highlight;
            this.cboxShowAfterInsertSoundDlg.Name = "cboxShowAfterInsertSoundDlg";
            // 
            // FAppSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            resources.ApplyResources(this, "$this");
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FAppSettings";
            this.pGeneral.ResumeLayout(false);
            this.pGeneral.PerformLayout();
            this.pConcreteGeneral.ResumeLayout(false);
            this.pConcreteGeneral.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.optionsView)).EndInit();
            this.pDesktopComponents.ResumeLayout(false);
            this.pDesktopComponents.PerformLayout();
            this.pLocalization.ResumeLayout(false);
            this.pLocalization.PerformLayout();
            this.exGroupBox2.ResumeLayout(false);
            this.exGroupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ExControls.ExGroupBox exGroupBox2;
        private ExControls.ExCheckBox cboxAutoRecalculateSoundDurations;
        private ExControls.ExCheckBox cboxAutoInsertSoundData;
        private ExControls.ExCheckBox cboxShowAfterInsertSoundDlg;
    }
}