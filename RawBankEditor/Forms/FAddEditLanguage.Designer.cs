namespace RawBankEditor.Forms
{
    partial class FAddEditLanguage
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
            this.bSave = new ExControls.ExButton();
            this.bStorno = new ExControls.ExButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbKey = new ExControls.ExTextBox();
            this.tbName = new ExControls.ExTextBox();
            this.tbRelativePath = new ExControls.ExTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cboxNameAndPathAutoChange = new ExControls.ExCheckBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // bSave
            // 
            this.bSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bSave.Location = new System.Drawing.Point(263, 100);
            this.bSave.Name = "bSave";
            this.bSave.Size = new System.Drawing.Size(75, 23);
            this.bSave.TabIndex = 7;
            this.bSave.Text = "Uložiť";
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.BSave_Click);
            // 
            // bStorno
            // 
            this.bStorno.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bStorno.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bStorno.Location = new System.Drawing.Point(344, 100);
            this.bStorno.Name = "bStorno";
            this.bStorno.Size = new System.Drawing.Size(75, 23);
            this.bStorno.TabIndex = 8;
            this.bStorno.Text = "Zrušiť";
            this.bStorno.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Kľúč jazyka:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Názov jazyka:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Relatívna cesta:";
            // 
            // tbKey
            // 
            this.tbKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbKey.BorderColor = System.Drawing.Color.DimGray;
            this.tbKey.DisabledBackColor = System.Drawing.SystemColors.Control;
            this.tbKey.DisabledBorderColor = System.Drawing.SystemColors.InactiveBorder;
            this.tbKey.DisabledForeColor = System.Drawing.SystemColors.GrayText;
            this.tbKey.HighlightColor = System.Drawing.SystemColors.Highlight;
            this.tbKey.HintForeColor = System.Drawing.SystemColors.GrayText;
            this.tbKey.HintText = "Napr.: SK";
            this.tbKey.Location = new System.Drawing.Point(106, 8);
            this.tbKey.Name = "tbKey";
            this.tbKey.Size = new System.Drawing.Size(313, 20);
            this.tbKey.TabIndex = 1;
            this.tbKey.TextChanged += new System.EventHandler(this.TbKey_TextChanged);
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
            this.tbName.HintText = "Napr.: Slovenčina";
            this.tbName.Location = new System.Drawing.Point(106, 34);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(313, 20);
            this.tbName.TabIndex = 3;
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
            this.tbRelativePath.HintText = "Napr.: SK\\";
            this.tbRelativePath.Location = new System.Drawing.Point(106, 60);
            this.tbRelativePath.Name = "tbRelativePath";
            this.tbRelativePath.Size = new System.Drawing.Size(313, 20);
            this.tbRelativePath.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cboxNameAndPathAutoChange);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.bStorno);
            this.panel1.Controls.Add(this.tbRelativePath);
            this.panel1.Controls.Add(this.bSave);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.tbName);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.tbKey);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(5);
            this.panel1.Size = new System.Drawing.Size(427, 131);
            this.panel1.TabIndex = 8;
            // 
            // cboxNameAndPathAutoChange
            // 
            this.cboxNameAndPathAutoChange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cboxNameAndPathAutoChange.AutoSize = true;
            this.cboxNameAndPathAutoChange.BoxBackColor = System.Drawing.Color.White;
            this.cboxNameAndPathAutoChange.Checked = true;
            this.cboxNameAndPathAutoChange.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cboxNameAndPathAutoChange.HighlightColor = System.Drawing.SystemColors.Highlight;
            this.cboxNameAndPathAutoChange.Location = new System.Drawing.Point(8, 106);
            this.cboxNameAndPathAutoChange.Name = "cboxNameAndPathAutoChange";
            this.cboxNameAndPathAutoChange.Size = new System.Drawing.Size(245, 17);
            this.cboxNameAndPathAutoChange.TabIndex = 6;
            this.cboxNameAndPathAutoChange.Text = "Automaticky meniť názov a cestu podľa kľúča";
            this.cboxNameAndPathAutoChange.CheckedChanged += new System.EventHandler(this.CboxNameAndPathAutoChange_CheckedChanged);
            // 
            // FAddEditLanguage
            // 
            this.AcceptButton = this.bSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bStorno;
            this.ClientSize = new System.Drawing.Size(427, 131);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(625, 170);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(443, 170);
            this.Name = "FAddEditLanguage";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Upraviť jazyk";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ExControls.ExButton bSave;
        private ExControls.ExButton bStorno;
        private Label label1;
        private Label label2;
        private Label label3;
        private ExControls.ExTextBox tbKey;
        private ExControls.ExTextBox tbName;
        private ExControls.ExTextBox tbRelativePath;
        private Panel panel1;
        private ExControls.ExCheckBox cboxNameAndPathAutoChange;
    }
}