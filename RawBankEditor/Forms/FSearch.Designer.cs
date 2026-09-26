
namespace RawBankEditor.Forms
{
    partial class FSearch
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
            this.label1 = new System.Windows.Forms.Label();
            this.tbText = new ExControls.ExTextBox();
            this.exGroupBox1 = new ExControls.ExGroupBox();
            this.rbText = new ExControls.ExRadioButton();
            this.rbName = new ExControls.ExRadioButton();
            this.rbKey = new ExControls.ExRadioButton();
            this.rbFileName = new ExControls.ExRadioButton();
            this.cboxIgnoreCase = new ExControls.ExCheckBox();
            this.exGroupBox2 = new ExControls.ExGroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.trackBarOpacity = new System.Windows.Forms.TrackBar();
            this.bSearch = new ExControls.ExButton();
            this.bStorno = new ExControls.ExButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.exGroupBox1.SuspendLayout();
            this.exGroupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarOpacity)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 5);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Hľadaný text:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbText
            // 
            this.tbText.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbText.BorderColor = System.Drawing.Color.DimGray;
            this.tbText.DisabledBackColor = System.Drawing.SystemColors.Control;
            this.tbText.DisabledBorderColor = System.Drawing.SystemColors.InactiveBorder;
            this.tbText.DisabledForeColor = System.Drawing.SystemColors.GrayText;
            this.tbText.HighlightColor = System.Drawing.SystemColors.Highlight;
            this.tbText.HintForeColor = System.Drawing.SystemColors.GrayText;
            this.tbText.HintText = null;
            this.tbText.Location = new System.Drawing.Point(77, 2);
            this.tbText.Margin = new System.Windows.Forms.Padding(2);
            this.tbText.Name = "tbText";
            this.tbText.Size = new System.Drawing.Size(339, 20);
            this.tbText.TabIndex = 1;
            // 
            // exGroupBox1
            // 
            this.exGroupBox1.AutoSize = true;
            this.exGroupBox1.Controls.Add(this.rbText);
            this.exGroupBox1.Controls.Add(this.rbName);
            this.exGroupBox1.Controls.Add(this.rbKey);
            this.exGroupBox1.Controls.Add(this.rbFileName);
            this.exGroupBox1.Location = new System.Drawing.Point(5, 26);
            this.exGroupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.exGroupBox1.Name = "exGroupBox1";
            this.exGroupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.exGroupBox1.Size = new System.Drawing.Size(204, 116);
            this.exGroupBox1.TabIndex = 2;
            this.exGroupBox1.TabStop = false;
            this.exGroupBox1.Text = "Parameter hľadania";
            // 
            // rbText
            // 
            this.rbText.AutoSize = true;
            this.rbText.HighlightColor = System.Drawing.SystemColors.Highlight;
            this.rbText.Location = new System.Drawing.Point(4, 82);
            this.rbText.Margin = new System.Windows.Forms.Padding(2);
            this.rbText.Name = "rbText";
            this.rbText.Size = new System.Drawing.Size(88, 17);
            this.rbText.TabIndex = 3;
            this.rbText.TabStop = true;
            this.rbText.Text = "Text hlásenia";
            this.rbText.UseVisualStyleBackColor = true;
            // 
            // rbName
            // 
            this.rbName.AutoSize = true;
            this.rbName.HighlightColor = System.Drawing.SystemColors.Highlight;
            this.rbName.Location = new System.Drawing.Point(4, 39);
            this.rbName.Margin = new System.Windows.Forms.Padding(2);
            this.rbName.Name = "rbName";
            this.rbName.Size = new System.Drawing.Size(88, 17);
            this.rbName.TabIndex = 1;
            this.rbName.TabStop = true;
            this.rbName.Text = "Názov zvuku";
            this.rbName.UseVisualStyleBackColor = true;
            // 
            // rbKey
            // 
            this.rbKey.AutoSize = true;
            this.rbKey.Checked = true;
            this.rbKey.HighlightColor = System.Drawing.SystemColors.Highlight;
            this.rbKey.Location = new System.Drawing.Point(4, 17);
            this.rbKey.Margin = new System.Windows.Forms.Padding(2);
            this.rbKey.Name = "rbKey";
            this.rbKey.Size = new System.Drawing.Size(80, 17);
            this.rbKey.TabIndex = 0;
            this.rbKey.TabStop = true;
            this.rbKey.Text = "Kľúč zvuku";
            this.rbKey.UseVisualStyleBackColor = true;
            // 
            // rbFileName
            // 
            this.rbFileName.AutoSize = true;
            this.rbFileName.HighlightColor = System.Drawing.SystemColors.Highlight;
            this.rbFileName.Location = new System.Drawing.Point(4, 61);
            this.rbFileName.Margin = new System.Windows.Forms.Padding(2);
            this.rbFileName.Name = "rbFileName";
            this.rbFileName.Size = new System.Drawing.Size(151, 17);
            this.rbFileName.TabIndex = 2;
            this.rbFileName.TabStop = true;
            this.rbFileName.Text = "Názov súboru (s príponou)";
            this.rbFileName.UseVisualStyleBackColor = true;
            // 
            // cboxIgnoreCase
            // 
            this.cboxIgnoreCase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cboxIgnoreCase.AutoSize = true;
            this.cboxIgnoreCase.BoxBackColor = System.Drawing.Color.White;
            this.cboxIgnoreCase.HighlightColor = System.Drawing.SystemColors.Highlight;
            this.cboxIgnoreCase.Location = new System.Drawing.Point(214, 125);
            this.cboxIgnoreCase.Margin = new System.Windows.Forms.Padding(2);
            this.cboxIgnoreCase.Name = "cboxIgnoreCase";
            this.cboxIgnoreCase.Size = new System.Drawing.Size(180, 17);
            this.cboxIgnoreCase.TabIndex = 4;
            this.cboxIgnoreCase.Text = "Ignorovať VEĽKÉ/malé písmená";
            // 
            // exGroupBox2
            // 
            this.exGroupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.exGroupBox2.Controls.Add(this.label3);
            this.exGroupBox2.Controls.Add(this.label2);
            this.exGroupBox2.Controls.Add(this.trackBarOpacity);
            this.exGroupBox2.Location = new System.Drawing.Point(213, 26);
            this.exGroupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.exGroupBox2.Name = "exGroupBox2";
            this.exGroupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.exGroupBox2.Size = new System.Drawing.Size(196, 91);
            this.exGroupBox2.TabIndex = 3;
            this.exGroupBox2.TabStop = false;
            this.exGroupBox2.Text = "Nepriehľadnosť okna";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 63);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(27, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "50%";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(148, 63);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "100%";
            // 
            // trackBarOpacity
            // 
            this.trackBarOpacity.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarOpacity.LargeChange = 10;
            this.trackBarOpacity.Location = new System.Drawing.Point(17, 18);
            this.trackBarOpacity.Margin = new System.Windows.Forms.Padding(2);
            this.trackBarOpacity.Maximum = 100;
            this.trackBarOpacity.Minimum = 50;
            this.trackBarOpacity.Name = "trackBarOpacity";
            this.trackBarOpacity.Size = new System.Drawing.Size(164, 45);
            this.trackBarOpacity.SmallChange = 5;
            this.trackBarOpacity.TabIndex = 0;
            this.trackBarOpacity.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackBarOpacity.Value = 100;
            this.trackBarOpacity.Scroll += new System.EventHandler(this.trackBarOpacity_Scroll);
            // 
            // bSearch
            // 
            this.bSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bSearch.AutoSize = true;
            this.bSearch.Location = new System.Drawing.Point(277, 155);
            this.bSearch.Margin = new System.Windows.Forms.Padding(2);
            this.bSearch.Name = "bSearch";
            this.bSearch.Size = new System.Drawing.Size(79, 23);
            this.bSearch.TabIndex = 5;
            this.bSearch.Text = "Hľadať ďalší";
            this.bSearch.UseVisualStyleBackColor = true;
            this.bSearch.Click += new System.EventHandler(this.bSearch_Click);
            // 
            // bStorno
            // 
            this.bStorno.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bStorno.AutoSize = true;
            this.bStorno.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bStorno.Location = new System.Drawing.Point(360, 155);
            this.bStorno.Margin = new System.Windows.Forms.Padding(2);
            this.bStorno.Name = "bStorno";
            this.bStorno.Size = new System.Drawing.Size(56, 23);
            this.bStorno.TabIndex = 6;
            this.bStorno.Text = "Zrušiť";
            this.bStorno.UseVisualStyleBackColor = true;
            this.bStorno.Click += new System.EventHandler(this.bStorno_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cboxIgnoreCase);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.bStorno);
            this.panel1.Controls.Add(this.bSearch);
            this.panel1.Controls.Add(this.tbText);
            this.panel1.Controls.Add(this.exGroupBox1);
            this.panel1.Controls.Add(this.exGroupBox2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(8, 8);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(5);
            this.panel1.Size = new System.Drawing.Size(418, 185);
            this.panel1.TabIndex = 6;
            // 
            // FSearch
            // 
            this.AcceptButton = this.bSearch;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.bStorno;
            this.ClientSize = new System.Drawing.Size(434, 201);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(450, 240);
            this.Name = "FSearch";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Hľadať";
            this.exGroupBox1.ResumeLayout(false);
            this.exGroupBox1.PerformLayout();
            this.exGroupBox2.ResumeLayout(false);
            this.exGroupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarOpacity)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private ExControls.ExTextBox tbText;
        private ExControls.ExGroupBox exGroupBox1;
        private ExControls.ExRadioButton rbText;
        private ExControls.ExRadioButton rbName;
        private ExControls.ExRadioButton rbKey;
        private ExControls.ExRadioButton rbFileName;
        private ExControls.ExGroupBox exGroupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar trackBarOpacity;
        private ExControls.ExCheckBox cboxIgnoreCase;
        private ExControls.ExButton bSearch;
        private ExControls.ExButton bStorno;
        private Panel panel1;
    }
}