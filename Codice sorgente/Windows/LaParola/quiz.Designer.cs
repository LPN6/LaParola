namespace LaParola
{
    partial class Quiz
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Quiz));
            this.rbRis1 = new System.Windows.Forms.RadioButton();
            this.rbRis2 = new System.Windows.Forms.RadioButton();
            this.rbRis3 = new System.Windows.Forms.RadioButton();
            this.rbRis4 = new System.Windows.Forms.RadioButton();
            this.tbDomanda = new System.Windows.Forms.TextBox();
            this.etiGiustoSbagliato = new System.Windows.Forms.Label();
            this.tbRisposta = new TestiBiblici.RichTextBoxEx();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.guidaFile.SetShowHelp(this.btnOK, ((bool)(resources.GetObject("btnOK.ShowHelp"))));
            // 
            // btnCanc
            // 
            resources.ApplyResources(this.btnCanc, "btnCanc");
            this.guidaFile.SetShowHelp(this.btnCanc, ((bool)(resources.GetObject("btnCanc.ShowHelp"))));
            this.btnCanc.Click += new System.EventHandler(this.btnCanc_Click);
            // 
            // rbRis1
            // 
            resources.ApplyResources(this.rbRis1, "rbRis1");
            this.guidaFile.SetHelpKeyword(this.rbRis1, resources.GetString("rbRis1.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.rbRis1, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("rbRis1.HelpNavigator"))));
            this.rbRis1.Name = "rbRis1";
            this.guidaFile.SetShowHelp(this.rbRis1, ((bool)(resources.GetObject("rbRis1.ShowHelp"))));
            this.rbRis1.TabStop = true;
            this.rbRis1.Tag = "1";
            this.rbRis1.UseVisualStyleBackColor = true;
            this.rbRis1.CheckedChanged += new System.EventHandler(this.rbRis_CheckedChanged);
            // 
            // rbRis2
            // 
            resources.ApplyResources(this.rbRis2, "rbRis2");
            this.guidaFile.SetHelpKeyword(this.rbRis2, resources.GetString("rbRis2.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.rbRis2, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("rbRis2.HelpNavigator"))));
            this.rbRis2.Name = "rbRis2";
            this.guidaFile.SetShowHelp(this.rbRis2, ((bool)(resources.GetObject("rbRis2.ShowHelp"))));
            this.rbRis2.TabStop = true;
            this.rbRis2.Tag = "2";
            this.rbRis2.UseVisualStyleBackColor = true;
            this.rbRis2.CheckedChanged += new System.EventHandler(this.rbRis_CheckedChanged);
            // 
            // rbRis3
            // 
            resources.ApplyResources(this.rbRis3, "rbRis3");
            this.guidaFile.SetHelpKeyword(this.rbRis3, resources.GetString("rbRis3.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.rbRis3, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("rbRis3.HelpNavigator"))));
            this.rbRis3.Name = "rbRis3";
            this.guidaFile.SetShowHelp(this.rbRis3, ((bool)(resources.GetObject("rbRis3.ShowHelp"))));
            this.rbRis3.TabStop = true;
            this.rbRis3.Tag = "3";
            this.rbRis3.UseVisualStyleBackColor = true;
            this.rbRis3.CheckedChanged += new System.EventHandler(this.rbRis_CheckedChanged);
            // 
            // rbRis4
            // 
            resources.ApplyResources(this.rbRis4, "rbRis4");
            this.guidaFile.SetHelpKeyword(this.rbRis4, resources.GetString("rbRis4.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.rbRis4, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("rbRis4.HelpNavigator"))));
            this.rbRis4.Name = "rbRis4";
            this.guidaFile.SetShowHelp(this.rbRis4, ((bool)(resources.GetObject("rbRis4.ShowHelp"))));
            this.rbRis4.TabStop = true;
            this.rbRis4.Tag = "4";
            this.rbRis4.UseVisualStyleBackColor = true;
            this.rbRis4.CheckedChanged += new System.EventHandler(this.rbRis_CheckedChanged);
            // 
            // tbDomanda
            // 
            this.tbDomanda.BackColor = System.Drawing.SystemColors.Control;
            this.tbDomanda.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.tbDomanda, "tbDomanda");
            this.guidaFile.SetHelpKeyword(this.tbDomanda, resources.GetString("tbDomanda.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.tbDomanda, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("tbDomanda.HelpNavigator"))));
            this.tbDomanda.Name = "tbDomanda";
            this.tbDomanda.ReadOnly = true;
            this.guidaFile.SetShowHelp(this.tbDomanda, ((bool)(resources.GetObject("tbDomanda.ShowHelp"))));
            this.tbDomanda.TabStop = false;
            // 
            // etiGiustoSbagliato
            // 
            resources.ApplyResources(this.etiGiustoSbagliato, "etiGiustoSbagliato");
            this.etiGiustoSbagliato.Name = "etiGiustoSbagliato";
            this.guidaFile.SetShowHelp(this.etiGiustoSbagliato, ((bool)(resources.GetObject("etiGiustoSbagliato.ShowHelp"))));
            // 
            // tbRisposta
            // 
            this.tbRisposta.BackColor = System.Drawing.SystemColors.Control;
            this.guidaFile.SetHelpKeyword(this.tbRisposta, resources.GetString("tbRisposta.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.tbRisposta, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("tbRisposta.HelpNavigator"))));
            this.tbRisposta.Lingua = null;
            resources.ApplyResources(this.tbRisposta, "tbRisposta");
            this.tbRisposta.Name = "tbRisposta";
            this.tbRisposta.ReadOnly = true;
            this.tbRisposta.SelectionAlignment = TestiBiblici.RichTextBoxEx.TextAlign.Left;
            this.guidaFile.SetShowHelp(this.tbRisposta, ((bool)(resources.GetObject("tbRisposta.ShowHelp"))));
            this.tbRisposta.TabStop = false;
            this.tbRisposta.Versione = null;
            // 
            // Quiz
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.etiGiustoSbagliato);
            this.Controls.Add(this.tbDomanda);
            this.Controls.Add(this.tbRisposta);
            this.Controls.Add(this.rbRis4);
            this.Controls.Add(this.rbRis3);
            this.Controls.Add(this.rbRis2);
            this.Controls.Add(this.rbRis1);
            this.Name = "Quiz";
            this.guidaFile.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.Tag = "Quiz";
            this.Load += new System.EventHandler(this.Quiz_Load);
            this.Controls.SetChildIndex(this.rbRis1, 0);
            this.Controls.SetChildIndex(this.rbRis2, 0);
            this.Controls.SetChildIndex(this.rbRis3, 0);
            this.Controls.SetChildIndex(this.rbRis4, 0);
            this.Controls.SetChildIndex(this.tbRisposta, 0);
            this.Controls.SetChildIndex(this.tbDomanda, 0);
            this.Controls.SetChildIndex(this.etiGiustoSbagliato, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnCanc, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rbRis1;
        private System.Windows.Forms.RadioButton rbRis2;
        private System.Windows.Forms.RadioButton rbRis3;
        private System.Windows.Forms.RadioButton rbRis4;
        private System.Windows.Forms.TextBox tbDomanda;
        private System.Windows.Forms.Label etiGiustoSbagliato;
        private TestiBiblici.RichTextBoxEx tbRisposta;
    }
}