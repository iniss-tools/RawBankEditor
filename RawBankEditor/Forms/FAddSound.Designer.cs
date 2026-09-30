
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FAddSound));
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
            resources.ApplyResources(this.label1, "label1");
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            //
            // label4
            //
            resources.ApplyResources(this.label4, "label4");
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            //
            // label5
            //
            resources.ApplyResources(this.label5, "label5");
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            //
            // tbFileName
            //
            resources.ApplyResources(this.tbFileName, "tbFileName");
            this.tbFileName.BorderColor = System.Drawing.Color.DimGray;
            this.tbFileName.DisabledBackColor = System.Drawing.SystemColors.Control;
            this.tbFileName.DisabledBorderColor = System.Drawing.SystemColors.InactiveBorder;
            this.tbFileName.DisabledForeColor = System.Drawing.SystemColors.GrayText;
            this.tbFileName.HighlightColor = System.Drawing.SystemColors.Highlight;
            this.tbFileName.HintForeColor = System.Drawing.SystemColors.GrayText;
            this.tbFileName.HintText = "(Povinné) Napr. 9900160.WAV";
            this.tbFileName.Margin = new System.Windows.Forms.Padding(2);
            this.tbFileName.Name = "tbFileName";
            //
            // bSave
            //
            resources.ApplyResources(this.bSave, "bSave");
            this.bSave.Margin = new System.Windows.Forms.Padding(2);
            this.bSave.Name = "bSave";
            this.bSave.UseVisualStyleBackColor = true;
            this.bSave.Click += new System.EventHandler(this.BSave_Click);
            // 
            // bStorno
            // 
            resources.ApplyResources(this.bStorno, "bStorno");
            this.bStorno.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bStorno.Margin = new System.Windows.Forms.Padding(2);
            this.bStorno.Name = "bStorno";
            this.bStorno.UseVisualStyleBackColor = true;
            this.bStorno.Click += new System.EventHandler(this.BStorno_Click);
            // 
            // tbRelativePath
            // 
            resources.ApplyResources(this.tbRelativePath, "tbRelativePath");
            this.tbRelativePath.BorderColor = System.Drawing.Color.DimGray;
            this.tbRelativePath.DisabledBackColor = System.Drawing.SystemColors.Control;
            this.tbRelativePath.DisabledBorderColor = System.Drawing.SystemColors.InactiveBorder;
            this.tbRelativePath.DisabledForeColor = System.Drawing.SystemColors.GrayText;
            this.tbRelativePath.HighlightColor = System.Drawing.SystemColors.Highlight;
            this.tbRelativePath.HintForeColor = System.Drawing.SystemColors.GrayText;
            this.tbRelativePath.HintText = "(Nepovinné) Vzhľadom na priečinok skupiny, napr. ..\\CISLO1\\";
            this.tbRelativePath.Margin = new System.Windows.Forms.Padding(2);
            this.tbRelativePath.Name = "tbRelativePath";
            // 
            // tbName
            // 
            resources.ApplyResources(this.tbName, "tbName");
            this.tbName.BorderColor = System.Drawing.Color.DimGray;
            this.tbName.DisabledBackColor = System.Drawing.SystemColors.Control;
            this.tbName.DisabledBorderColor = System.Drawing.SystemColors.InactiveBorder;
            this.tbName.DisabledForeColor = System.Drawing.SystemColors.GrayText;
            this.tbName.HighlightColor = System.Drawing.SystemColors.Highlight;
            this.tbName.HintForeColor = System.Drawing.SystemColors.GrayText;
            this.tbName.HintText = "(Povinné)";
            this.tbName.Margin = new System.Windows.Forms.Padding(2);
            this.tbName.Name = "tbName";
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
            resources.ApplyResources(this.tbKey, "tbKey");
            this.tbKey.Margin = new System.Windows.Forms.Padding(2);
            this.tbKey.Name = "tbKey";
            this.tbKey.TextChanged += new System.EventHandler(this.TbKey_TextChanged);
            // 
            // rtbText
            // 
            resources.ApplyResources(this.rtbText, "rtbText");
            this.rtbText.BorderColor = System.Drawing.Color.DimGray;
            this.rtbText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbText.DefaultStyle = false;
            this.rtbText.DisabledBackColor = System.Drawing.SystemColors.Control;
            this.rtbText.DisabledBorderColor = System.Drawing.SystemColors.InactiveBorder;
            this.rtbText.DisabledForeColor = System.Drawing.SystemColors.GrayText;
            this.rtbText.HighlightColor = System.Drawing.SystemColors.Highlight;
            this.rtbText.Margin = new System.Windows.Forms.Padding(2);
            this.rtbText.Name = "rtbText";
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
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(5);
            //
            // cboxNameAndFileAutoChange
            //
            resources.ApplyResources(this.cboxNameAndFileAutoChange, "cboxNameAndFileAutoChange");
            this.cboxNameAndFileAutoChange.BoxBackColor = System.Drawing.Color.White;
            this.cboxNameAndFileAutoChange.Checked = true;
            this.cboxNameAndFileAutoChange.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cboxNameAndFileAutoChange.HighlightColor = System.Drawing.SystemColors.Highlight;
            this.cboxNameAndFileAutoChange.Name = "cboxNameAndFileAutoChange";
            this.cboxNameAndFileAutoChange.CheckedChanged += new System.EventHandler(this.CboxNameAndFileAutoChange_CheckedChanged);
            // 
            // FAddSound
            // 
            this.AcceptButton = this.bSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bStorno;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(432, 274);
            this.Name = "FAddSound";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
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