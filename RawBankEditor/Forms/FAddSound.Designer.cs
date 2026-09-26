
namespace RawBankEditor.Forms
{
    partial class FAddSound
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
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbFileName = new ExControls.ExTextBox();
            this.bSave = new ExControls.ExButton();
            this.bStorno = new ExControls.ExButton();
            this.tbRelativePath = new ExControls.ExTextBox();
            this.tbName = new ExControls.ExTextBox();
            this.tbKey = new ExControls.ExTextBox();
            this.rtbText = new ExControls.ExRichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cboxNameAndFileAutoChange = new ExControls.ExCheckBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 10);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Kľúč zvuku:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(224, 10);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Názov zvuku:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 91);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Text hlásenia:";
            //
            // label4
            //
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 62);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(128, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Prídavná relatívna cesta:";
            //
            // label5
            //
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 38);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(71, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Názov súboru:";
            //
            // tbFileName
            //
            this.tbFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbFileName.BorderColor = System.Drawing.Color.DimGray;
            this.tbFileName.DisabledBackColor = System.Drawing.SystemColors.Control;
            this.tbFileName.DisabledBorderColor = System.Drawing.SystemColors.InactiveBorder;
            this.tbFileName.DisabledForeColor = System.Drawing.SystemColors.GrayText;
            this.tbFileName.HighlightColor = System.Drawing.SystemColors.Highlight;
            this.tbFileName.HintForeColor = System.Drawing.SystemColors.GrayText;
            this.tbFileName.HintText = "(Povinné) Napr. 9900160.WAV";
            this.tbFileName.Location = new System.Drawing.Point(151, 31);
            this.tbFileName.Margin = new System.Windows.Forms.Padding(2);
            this.tbFileName.Name = "tbFileName";
            this.tbFileName.Size = new System.Drawing.Size(319, 20);
            this.tbFileName.TabIndex = 5;
            //
            // bSave
            //
            this.bSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bSave.AutoSize = true;
            this.bSave.Location = new System.Drawing.Point(316, 213);
            this.bSave.Margin = new System.Windows.Forms.Padding(2);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(70, 23);
            this.bSave.TabIndex = 11;
            this.bSave.Text = "Uložiť";
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.BSave_Click);
            // 
            // bStorno
            // 
            this.bStorno.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bStorno.AutoSize = true;
            this.bStorno.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bStorno.Location = new System.Drawing.Point(400, 213);
            this.bStorno.Margin = new System.Windows.Forms.Padding(2);
            this.bStorno.Name = "bStorno";
            this.bStorno.Size = new System.Drawing.Size(70, 23);
            this.bStorno.TabIndex = 12;
            this.bStorno.Text = "Zrušiť";
            this.bStorno.UseVisualStyleBackColor = true;
            this.bStorno.Click += new System.EventHandler(this.BStorno_Click);
            // 
            // tbRelativePath
            // 
            this.tbRelativePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbRelativePath.BorderColor = System.Drawing.Color.DimGray;
            this.tbRelativePath.DisabledBackColor = System.Drawing.SystemColors.Control;
            this.tbRelativePath.DisabledBorderColor = System.Drawing.SystemColors.InactiveBorder;
            this.tbRelativePath.DisabledForeColor = System.Drawing.SystemColors.GrayText;
            this.tbRelativePath.HighlightColor = System.Drawing.SystemColors.Highlight;
            this.tbRelativePath.HintForeColor = System.Drawing.SystemColors.GrayText;
            this.tbRelativePath.HintText = "(Nepovinné) Namiesto priečinka skupiny, napr. ..\\CISLO\\";
            this.tbRelativePath.Location = new System.Drawing.Point(151, 55);
            this.tbRelativePath.Margin = new System.Windows.Forms.Padding(2);
            this.tbRelativePath.Name = "tbRelativePath";
            this.tbRelativePath.Size = new System.Drawing.Size(319, 20);
            this.tbRelativePath.TabIndex = 7;
            // 
            // tbName
            // 
            this.tbName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbName.BorderColor = System.Drawing.Color.DimGray;
            this.tbName.DisabledBackColor = System.Drawing.SystemColors.Control;
            this.tbName.DisabledBorderColor = System.Drawing.SystemColors.InactiveBorder;
            this.tbName.DisabledForeColor = System.Drawing.SystemColors.GrayText;
            this.tbName.HighlightColor = System.Drawing.SystemColors.Highlight;
            this.tbName.HintForeColor = System.Drawing.SystemColors.GrayText;
            this.tbName.HintText = "(Povinné)";
            this.tbName.Location = new System.Drawing.Point(301, 7);
            this.tbName.Margin = new System.Windows.Forms.Padding(2);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(169, 20);
            this.tbName.TabIndex = 3;
            // 
            // tbKey
            // 
            this.tbKey.BorderColor = System.Drawing.Color.DimGray;
            this.tbKey.DisabledBackColor = System.Drawing.SystemColors.Control;
            this.tbKey.DisabledBorderColor = System.Drawing.SystemColors.InactiveBorder;
            this.tbKey.DisabledForeColor = System.Drawing.SystemColors.GrayText;
            this.tbKey.HighlightColor = System.Drawing.SystemColors.Highlight;
            this.tbKey.HintForeColor = System.Drawing.SystemColors.GrayText;
            this.tbKey.HintText = "(Povinné)";
            this.tbKey.Location = new System.Drawing.Point(76, 7);
            this.tbKey.Margin = new System.Windows.Forms.Padding(2);
            this.tbKey.Name = "tbKey";
            this.tbKey.Size = new System.Drawing.Size(130, 20);
            this.tbKey.TabIndex = 1;
            this.tbKey.TextChanged += new System.EventHandler(this.TbKey_TextChanged);
            // 
            // rtbText
            // 
            this.rtbText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbText.BorderColor = System.Drawing.Color.DimGray;
            this.rtbText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbText.DefaultStyle = false;
            this.rtbText.DisabledBackColor = System.Drawing.SystemColors.Control;
            this.rtbText.DisabledBorderColor = System.Drawing.SystemColors.InactiveBorder;
            this.rtbText.DisabledForeColor = System.Drawing.SystemColors.GrayText;
            this.rtbText.HighlightColor = System.Drawing.SystemColors.Highlight;
            this.rtbText.Location = new System.Drawing.Point(11, 106);
            this.rtbText.Margin = new System.Windows.Forms.Padding(2);
            this.rtbText.Name = "rtbText";
            this.rtbText.Size = new System.Drawing.Size(459, 97);
            this.rtbText.TabIndex = 9;
            this.rtbText.Text = "";
            //
            // panel1
            //
            this.panel1.Controls.Add(this.cboxNameAndFileAutoChange);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.bStorno);
            this.panel1.Controls.Add(this.rtbText);
            this.panel1.Controls.Add(this.bSave);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.tbKey);
            this.panel1.Controls.Add(this.tbRelativePath);
            this.panel1.Controls.Add(this.tbName);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.tbFileName);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(5);
            this.panel1.Size = new System.Drawing.Size(481, 247);
            this.panel1.TabIndex = 13;
            //
            // cboxNameAndFileAutoChange
            //
            this.cboxNameAndFileAutoChange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cboxNameAndFileAutoChange.AutoSize = true;
            this.cboxNameAndFileAutoChange.BoxBackColor = System.Drawing.Color.White;
            this.cboxNameAndFileAutoChange.Checked = true;
            this.cboxNameAndFileAutoChange.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cboxNameAndFileAutoChange.HighlightColor = System.Drawing.SystemColors.Highlight;
            this.cboxNameAndFileAutoChange.Location = new System.Drawing.Point(13, 218);
            this.cboxNameAndFileAutoChange.Name = "cboxNameAndFileAutoChange";
            this.cboxNameAndFileAutoChange.Size = new System.Drawing.Size(245, 17);
            this.cboxNameAndFileAutoChange.TabIndex = 10;
            this.cboxNameAndFileAutoChange.Text = "Automaticky meniť názov a súbor podľa kľúča";
            this.cboxNameAndFileAutoChange.CheckedChanged += new System.EventHandler(this.CboxNameAndFileAutoChange_CheckedChanged);
            // 
            // FAddSound
            // 
            this.AcceptButton = this.bSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bStorno;
            this.ClientSize = new System.Drawing.Size(481, 247);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(432, 274);
            this.Name = "FAddSound";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Pridať zvuk";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private ExControls.ExTextBox tbKey;
        private ExControls.ExTextBox tbName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private ExControls.ExTextBox tbFileName;
        private ExControls.ExTextBox tbRelativePath;
        private ExControls.ExButton bSave;
        private ExControls.ExButton bStorno;
        private ExControls.ExRichTextBox rtbText;
        private Panel panel1;
        private ExControls.ExCheckBox cboxNameAndFileAutoChange;
    }
}