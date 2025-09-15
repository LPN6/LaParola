namespace LaParola
{
    partial class ScegliParola
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
            if (disposing)
            {
                if (components != null)
                    components.Dispose();
                if (fontEbraico != null)
                    fontEbraico.Dispose();
                if (fontGreco != null)
                    fontGreco.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScegliParola));
            this.lbParole = new System.Windows.Forms.ListBox();
            this.lbRadici = new System.Windows.Forms.ListBox();
            this.lbParoleDiRadice = new System.Windows.Forms.ListBox();
            this.labParole = new System.Windows.Forms.Label();
            this.labRadici = new System.Windows.Forms.Label();
            this.labParoleDiRadice = new System.Windows.Forms.Label();
            this.labNumeroVolteParola = new System.Windows.Forms.Label();
            this.labNumeroVolteRadice = new System.Windows.Forms.Label();
            this.labNumeroVolteParolaDiRadice = new System.Windows.Forms.Label();
            this.labRadiceDiParola = new System.Windows.Forms.Label();
            this.labNumeroParoleDiRadice = new System.Windows.Forms.Label();
            this.btnParola = new System.Windows.Forms.Button();
            this.btnRadice = new System.Windows.Forms.Button();
            this.btnParolaDiRadice = new System.Windows.Forms.Button();
            this.tbParole = new System.Windows.Forms.TextBox();
            this.tbRadici = new System.Windows.Forms.TextBox();
            this.tbParoleDiRadice = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.AccessibleDescription = null;
            this.btnOK.AccessibleName = null;
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.BackgroundImage = null;
            this.btnOK.Font = null;
            this.guidaFile.SetHelpKeyword(this.btnOK, null);
            this.guidaFile.SetHelpNavigator(this.btnOK, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("btnOK.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.btnOK, null);
            this.guidaFile.SetShowHelp(this.btnOK, ((bool)(resources.GetObject("btnOK.ShowHelp"))));
            // 
            // btnCanc
            // 
            this.btnCanc.AccessibleDescription = null;
            this.btnCanc.AccessibleName = null;
            resources.ApplyResources(this.btnCanc, "btnCanc");
            this.btnCanc.BackgroundImage = null;
            this.btnCanc.Font = null;
            this.guidaFile.SetHelpKeyword(this.btnCanc, null);
            this.guidaFile.SetHelpNavigator(this.btnCanc, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("btnCanc.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.btnCanc, null);
            this.guidaFile.SetShowHelp(this.btnCanc, ((bool)(resources.GetObject("btnCanc.ShowHelp"))));
            this.btnCanc.Click += new System.EventHandler(this.btnCanc_Click);
            // 
            // guidaFile
            // 
            this.guidaFile.HelpNamespace = null;
            // 
            // lbParole
            // 
            this.lbParole.AccessibleDescription = null;
            this.lbParole.AccessibleName = null;
            resources.ApplyResources(this.lbParole, "lbParole");
            this.lbParole.BackgroundImage = null;
            this.lbParole.Font = null;
            this.lbParole.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.lbParole, resources.GetString("lbParole.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.lbParole, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("lbParole.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.lbParole, null);
            this.lbParole.Name = "lbParole";
            this.guidaFile.SetShowHelp(this.lbParole, ((bool)(resources.GetObject("lbParole.ShowHelp"))));
            this.lbParole.SelectedIndexChanged += new System.EventHandler(this.lbParole_SelectedIndexChanged);
            this.lbParole.DoubleClick += new System.EventHandler(this.btnParola_Click);
            // 
            // lbRadici
            // 
            this.lbRadici.AccessibleDescription = null;
            this.lbRadici.AccessibleName = null;
            resources.ApplyResources(this.lbRadici, "lbRadici");
            this.lbRadici.BackgroundImage = null;
            this.lbRadici.Font = null;
            this.lbRadici.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.lbRadici, resources.GetString("lbRadici.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.lbRadici, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("lbRadici.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.lbRadici, null);
            this.lbRadici.Name = "lbRadici";
            this.guidaFile.SetShowHelp(this.lbRadici, ((bool)(resources.GetObject("lbRadici.ShowHelp"))));
            this.lbRadici.SelectedIndexChanged += new System.EventHandler(this.lbRadici_SelectedIndexChanged);
            this.lbRadici.DoubleClick += new System.EventHandler(this.btnRadice_Click);
            // 
            // lbParoleDiRadice
            // 
            this.lbParoleDiRadice.AccessibleDescription = null;
            this.lbParoleDiRadice.AccessibleName = null;
            this.lbParoleDiRadice.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            resources.ApplyResources(this.lbParoleDiRadice, "lbParoleDiRadice");
            this.lbParoleDiRadice.BackgroundImage = null;
            this.lbParoleDiRadice.Font = null;
            this.lbParoleDiRadice.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.lbParoleDiRadice, resources.GetString("lbParoleDiRadice.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.lbParoleDiRadice, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("lbParoleDiRadice.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.lbParoleDiRadice, null);
            this.lbParoleDiRadice.Name = "lbParoleDiRadice";
            this.guidaFile.SetShowHelp(this.lbParoleDiRadice, ((bool)(resources.GetObject("lbParoleDiRadice.ShowHelp"))));
            this.lbParoleDiRadice.SelectedIndexChanged += new System.EventHandler(this.lbParoleRadice_SelectedIndexChanged);
            this.lbParoleDiRadice.DoubleClick += new System.EventHandler(this.btnParolaDiRadice_Click);
            // 
            // labParole
            // 
            this.labParole.AccessibleDescription = null;
            this.labParole.AccessibleName = null;
            resources.ApplyResources(this.labParole, "labParole");
            this.labParole.Font = null;
            this.guidaFile.SetHelpKeyword(this.labParole, null);
            this.guidaFile.SetHelpNavigator(this.labParole, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("labParole.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.labParole, null);
            this.labParole.Name = "labParole";
            this.guidaFile.SetShowHelp(this.labParole, ((bool)(resources.GetObject("labParole.ShowHelp"))));
            // 
            // labRadici
            // 
            this.labRadici.AccessibleDescription = null;
            this.labRadici.AccessibleName = null;
            resources.ApplyResources(this.labRadici, "labRadici");
            this.labRadici.Font = null;
            this.guidaFile.SetHelpKeyword(this.labRadici, null);
            this.guidaFile.SetHelpNavigator(this.labRadici, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("labRadici.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.labRadici, null);
            this.labRadici.Name = "labRadici";
            this.guidaFile.SetShowHelp(this.labRadici, ((bool)(resources.GetObject("labRadici.ShowHelp"))));
            // 
            // labParoleDiRadice
            // 
            this.labParoleDiRadice.AccessibleDescription = null;
            this.labParoleDiRadice.AccessibleName = null;
            resources.ApplyResources(this.labParoleDiRadice, "labParoleDiRadice");
            this.labParoleDiRadice.Font = null;
            this.guidaFile.SetHelpKeyword(this.labParoleDiRadice, null);
            this.guidaFile.SetHelpNavigator(this.labParoleDiRadice, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("labParoleDiRadice.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.labParoleDiRadice, null);
            this.labParoleDiRadice.Name = "labParoleDiRadice";
            this.guidaFile.SetShowHelp(this.labParoleDiRadice, ((bool)(resources.GetObject("labParoleDiRadice.ShowHelp"))));
            // 
            // labNumeroVolteParola
            // 
            this.labNumeroVolteParola.AccessibleDescription = null;
            this.labNumeroVolteParola.AccessibleName = null;
            resources.ApplyResources(this.labNumeroVolteParola, "labNumeroVolteParola");
            this.labNumeroVolteParola.Font = null;
            this.guidaFile.SetHelpKeyword(this.labNumeroVolteParola, null);
            this.guidaFile.SetHelpNavigator(this.labNumeroVolteParola, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("labNumeroVolteParola.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.labNumeroVolteParola, null);
            this.labNumeroVolteParola.Name = "labNumeroVolteParola";
            this.guidaFile.SetShowHelp(this.labNumeroVolteParola, ((bool)(resources.GetObject("labNumeroVolteParola.ShowHelp"))));
            // 
            // labNumeroVolteRadice
            // 
            this.labNumeroVolteRadice.AccessibleDescription = null;
            this.labNumeroVolteRadice.AccessibleName = null;
            resources.ApplyResources(this.labNumeroVolteRadice, "labNumeroVolteRadice");
            this.labNumeroVolteRadice.Font = null;
            this.guidaFile.SetHelpKeyword(this.labNumeroVolteRadice, null);
            this.guidaFile.SetHelpNavigator(this.labNumeroVolteRadice, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("labNumeroVolteRadice.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.labNumeroVolteRadice, null);
            this.labNumeroVolteRadice.Name = "labNumeroVolteRadice";
            this.guidaFile.SetShowHelp(this.labNumeroVolteRadice, ((bool)(resources.GetObject("labNumeroVolteRadice.ShowHelp"))));
            // 
            // labNumeroVolteParolaDiRadice
            // 
            this.labNumeroVolteParolaDiRadice.AccessibleDescription = null;
            this.labNumeroVolteParolaDiRadice.AccessibleName = null;
            resources.ApplyResources(this.labNumeroVolteParolaDiRadice, "labNumeroVolteParolaDiRadice");
            this.labNumeroVolteParolaDiRadice.Font = null;
            this.guidaFile.SetHelpKeyword(this.labNumeroVolteParolaDiRadice, null);
            this.guidaFile.SetHelpNavigator(this.labNumeroVolteParolaDiRadice, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("labNumeroVolteParolaDiRadice.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.labNumeroVolteParolaDiRadice, null);
            this.labNumeroVolteParolaDiRadice.Name = "labNumeroVolteParolaDiRadice";
            this.guidaFile.SetShowHelp(this.labNumeroVolteParolaDiRadice, ((bool)(resources.GetObject("labNumeroVolteParolaDiRadice.ShowHelp"))));
            // 
            // labRadiceDiParola
            // 
            this.labRadiceDiParola.AccessibleDescription = null;
            this.labRadiceDiParola.AccessibleName = null;
            resources.ApplyResources(this.labRadiceDiParola, "labRadiceDiParola");
            this.labRadiceDiParola.Font = null;
            this.guidaFile.SetHelpKeyword(this.labRadiceDiParola, null);
            this.guidaFile.SetHelpNavigator(this.labRadiceDiParola, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("labRadiceDiParola.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.labRadiceDiParola, null);
            this.labRadiceDiParola.Name = "labRadiceDiParola";
            this.guidaFile.SetShowHelp(this.labRadiceDiParola, ((bool)(resources.GetObject("labRadiceDiParola.ShowHelp"))));
            // 
            // labNumeroParoleDiRadice
            // 
            this.labNumeroParoleDiRadice.AccessibleDescription = null;
            this.labNumeroParoleDiRadice.AccessibleName = null;
            resources.ApplyResources(this.labNumeroParoleDiRadice, "labNumeroParoleDiRadice");
            this.labNumeroParoleDiRadice.Font = null;
            this.guidaFile.SetHelpKeyword(this.labNumeroParoleDiRadice, null);
            this.guidaFile.SetHelpNavigator(this.labNumeroParoleDiRadice, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("labNumeroParoleDiRadice.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.labNumeroParoleDiRadice, null);
            this.labNumeroParoleDiRadice.Name = "labNumeroParoleDiRadice";
            this.guidaFile.SetShowHelp(this.labNumeroParoleDiRadice, ((bool)(resources.GetObject("labNumeroParoleDiRadice.ShowHelp"))));
            // 
            // btnParola
            // 
            this.btnParola.AccessibleDescription = null;
            this.btnParola.AccessibleName = null;
            resources.ApplyResources(this.btnParola, "btnParola");
            this.btnParola.BackgroundImage = null;
            this.btnParola.Font = null;
            this.guidaFile.SetHelpKeyword(this.btnParola, resources.GetString("btnParola.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.btnParola, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("btnParola.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.btnParola, null);
            this.btnParola.Name = "btnParola";
            this.guidaFile.SetShowHelp(this.btnParola, ((bool)(resources.GetObject("btnParola.ShowHelp"))));
            this.btnParola.UseVisualStyleBackColor = true;
            this.btnParola.Click += new System.EventHandler(this.btnParola_Click);
            // 
            // btnRadice
            // 
            this.btnRadice.AccessibleDescription = null;
            this.btnRadice.AccessibleName = null;
            resources.ApplyResources(this.btnRadice, "btnRadice");
            this.btnRadice.BackgroundImage = null;
            this.btnRadice.Font = null;
            this.guidaFile.SetHelpKeyword(this.btnRadice, resources.GetString("btnRadice.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.btnRadice, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("btnRadice.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.btnRadice, null);
            this.btnRadice.Name = "btnRadice";
            this.guidaFile.SetShowHelp(this.btnRadice, ((bool)(resources.GetObject("btnRadice.ShowHelp"))));
            this.btnRadice.UseVisualStyleBackColor = true;
            this.btnRadice.Click += new System.EventHandler(this.btnRadice_Click);
            // 
            // btnParolaDiRadice
            // 
            this.btnParolaDiRadice.AccessibleDescription = null;
            this.btnParolaDiRadice.AccessibleName = null;
            resources.ApplyResources(this.btnParolaDiRadice, "btnParolaDiRadice");
            this.btnParolaDiRadice.BackgroundImage = null;
            this.btnParolaDiRadice.Font = null;
            this.guidaFile.SetHelpKeyword(this.btnParolaDiRadice, resources.GetString("btnParolaDiRadice.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.btnParolaDiRadice, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("btnParolaDiRadice.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.btnParolaDiRadice, null);
            this.btnParolaDiRadice.Name = "btnParolaDiRadice";
            this.guidaFile.SetShowHelp(this.btnParolaDiRadice, ((bool)(resources.GetObject("btnParolaDiRadice.ShowHelp"))));
            this.btnParolaDiRadice.UseVisualStyleBackColor = true;
            this.btnParolaDiRadice.Click += new System.EventHandler(this.btnParolaDiRadice_Click);
            // 
            // tbParole
            // 
            this.tbParole.AccessibleDescription = null;
            this.tbParole.AccessibleName = null;
            resources.ApplyResources(this.tbParole, "tbParole");
            this.tbParole.BackgroundImage = null;
            this.tbParole.Font = null;
            this.guidaFile.SetHelpKeyword(this.tbParole, resources.GetString("tbParole.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.tbParole, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("tbParole.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.tbParole, null);
            this.tbParole.Name = "tbParole";
            this.guidaFile.SetShowHelp(this.tbParole, ((bool)(resources.GetObject("tbParole.ShowHelp"))));
            this.tbParole.TextChanged += new System.EventHandler(this.txtParole_TextChanged);
            // 
            // tbRadici
            // 
            this.tbRadici.AccessibleDescription = null;
            this.tbRadici.AccessibleName = null;
            resources.ApplyResources(this.tbRadici, "tbRadici");
            this.tbRadici.BackgroundImage = null;
            this.tbRadici.Font = null;
            this.guidaFile.SetHelpKeyword(this.tbRadici, resources.GetString("tbRadici.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.tbRadici, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("tbRadici.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.tbRadici, null);
            this.tbRadici.Name = "tbRadici";
            this.guidaFile.SetShowHelp(this.tbRadici, ((bool)(resources.GetObject("tbRadici.ShowHelp"))));
            this.tbRadici.TextChanged += new System.EventHandler(this.txtRadici_TextChanged);
            // 
            // tbParoleDiRadice
            // 
            this.tbParoleDiRadice.AccessibleDescription = null;
            this.tbParoleDiRadice.AccessibleName = null;
            resources.ApplyResources(this.tbParoleDiRadice, "tbParoleDiRadice");
            this.tbParoleDiRadice.BackgroundImage = null;
            this.tbParoleDiRadice.Font = null;
            this.guidaFile.SetHelpKeyword(this.tbParoleDiRadice, resources.GetString("tbParoleDiRadice.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.tbParoleDiRadice, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("tbParoleDiRadice.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.tbParoleDiRadice, null);
            this.tbParoleDiRadice.Name = "tbParoleDiRadice";
            this.guidaFile.SetShowHelp(this.tbParoleDiRadice, ((bool)(resources.GetObject("tbParoleDiRadice.ShowHelp"))));
            this.tbParoleDiRadice.TextChanged += new System.EventHandler(this.txtParoleDiRadice_TextChanged);
            // 
            // ScegliParola
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.tbParoleDiRadice);
            this.Controls.Add(this.tbRadici);
            this.Controls.Add(this.tbParole);
            this.Controls.Add(this.lbRadici);
            this.Controls.Add(this.btnParolaDiRadice);
            this.Controls.Add(this.btnRadice);
            this.Controls.Add(this.labParoleDiRadice);
            this.Controls.Add(this.labNumeroParoleDiRadice);
            this.Controls.Add(this.btnParola);
            this.Controls.Add(this.labNumeroVolteParolaDiRadice);
            this.Controls.Add(this.lbParole);
            this.Controls.Add(this.labNumeroVolteRadice);
            this.Controls.Add(this.labRadici);
            this.Controls.Add(this.labRadiceDiParola);
            this.Controls.Add(this.labParole);
            this.Controls.Add(this.lbParoleDiRadice);
            this.Controls.Add(this.labNumeroVolteParola);
            this.Font = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.guidaFile.SetHelpKeyword(this, null);
            this.guidaFile.SetHelpNavigator(this, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("$this.HelpNavigator"))));
            this.guidaFile.SetHelpString(this, null);
            this.Name = "ScegliParola";
            this.guidaFile.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Tag = "ScegliParola";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ScegliParola_FormClosing);
            this.Resize += new System.EventHandler(this.ScegliParola_Resize);
            this.Controls.SetChildIndex(this.labNumeroVolteParola, 0);
            this.Controls.SetChildIndex(this.lbParoleDiRadice, 0);
            this.Controls.SetChildIndex(this.labParole, 0);
            this.Controls.SetChildIndex(this.labRadiceDiParola, 0);
            this.Controls.SetChildIndex(this.labRadici, 0);
            this.Controls.SetChildIndex(this.labNumeroVolteRadice, 0);
            this.Controls.SetChildIndex(this.lbParole, 0);
            this.Controls.SetChildIndex(this.labNumeroVolteParolaDiRadice, 0);
            this.Controls.SetChildIndex(this.btnParola, 0);
            this.Controls.SetChildIndex(this.labNumeroParoleDiRadice, 0);
            this.Controls.SetChildIndex(this.labParoleDiRadice, 0);
            this.Controls.SetChildIndex(this.btnRadice, 0);
            this.Controls.SetChildIndex(this.btnParolaDiRadice, 0);
            this.Controls.SetChildIndex(this.lbRadici, 0);
            this.Controls.SetChildIndex(this.tbParole, 0);
            this.Controls.SetChildIndex(this.tbRadici, 0);
            this.Controls.SetChildIndex(this.tbParoleDiRadice, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnCanc, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbParole;
        private System.Windows.Forms.ListBox lbRadici;
        private System.Windows.Forms.ListBox lbParoleDiRadice;
        private System.Windows.Forms.Label labParole;
        private System.Windows.Forms.Label labRadici;
        private System.Windows.Forms.Label labParoleDiRadice;
        private System.Windows.Forms.Label labNumeroVolteParola;
        private System.Windows.Forms.Label labNumeroVolteRadice;
        private System.Windows.Forms.Label labNumeroVolteParolaDiRadice;
        private System.Windows.Forms.Label labRadiceDiParola;
        private System.Windows.Forms.Label labNumeroParoleDiRadice;
        private System.Windows.Forms.Button btnParola;
        private System.Windows.Forms.Button btnRadice;
        private System.Windows.Forms.Button btnParolaDiRadice;
        private System.Windows.Forms.TextBox tbParole;
        private System.Windows.Forms.TextBox tbRadici;
        private System.Windows.Forms.TextBox tbParoleDiRadice;
    }
}