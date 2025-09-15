namespace LaParola
{
    partial class Chiave
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Chiave));
            this.etiBrano = new System.Windows.Forms.Label();
            this.etiVersione = new System.Windows.Forms.Label();
            this.cbVersione = new System.Windows.Forms.ComboBox();
            this.gbOrdine = new System.Windows.Forms.GroupBox();
            this.rbPrimaApparenza = new System.Windows.Forms.RadioButton();
            this.rbApparenze = new System.Windows.Forms.RadioButton();
            this.rbAlfabetico = new System.Windows.Forms.RadioButton();
            this.cbIpertesto = new System.Windows.Forms.CheckBox();
            this.cbNonRadiciComuni = new System.Windows.Forms.CheckBox();
            this.gbParoleRadici = new System.Windows.Forms.GroupBox();
            this.rbRadici = new System.Windows.Forms.RadioButton();
            this.rbParole = new System.Windows.Forms.RadioButton();
            this.tbNonRadiciComuni = new System.Windows.Forms.TextBox();
            this.etiNumeroMinimo = new System.Windows.Forms.Label();
            this.udNumeroMinimo = new System.Windows.Forms.NumericUpDown();
            this.cbBrano = new System.Windows.Forms.ComboBox();
            this.cbRiferimenti = new System.Windows.Forms.CheckBox();
            this.cbDefinizioni = new System.Windows.Forms.CheckBox();
            this.gbOrdine.SuspendLayout();
            this.gbParoleRadici.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udNumeroMinimo)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.guidaFile.SetShowHelp(this.btnOK, ((bool)(resources.GetObject("btnOK.ShowHelp"))));
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCanc
            // 
            this.guidaFile.SetShowHelp(this.btnCanc, ((bool)(resources.GetObject("btnCanc.ShowHelp"))));
            resources.ApplyResources(this.btnCanc, "btnCanc");
            this.btnCanc.Click += new System.EventHandler(this.btnCanc_Click);
            // 
            // etiBrano
            // 
            resources.ApplyResources(this.etiBrano, "etiBrano");
            this.guidaFile.SetHelpKeyword(this.etiBrano, resources.GetString("etiBrano.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.etiBrano, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiBrano.HelpNavigator"))));
            this.etiBrano.Name = "etiBrano";
            this.guidaFile.SetShowHelp(this.etiBrano, ((bool)(resources.GetObject("etiBrano.ShowHelp"))));
            // 
            // etiVersione
            // 
            resources.ApplyResources(this.etiVersione, "etiVersione");
            this.guidaFile.SetHelpKeyword(this.etiVersione, resources.GetString("etiVersione.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.etiVersione, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiVersione.HelpNavigator"))));
            this.etiVersione.Name = "etiVersione";
            this.guidaFile.SetShowHelp(this.etiVersione, ((bool)(resources.GetObject("etiVersione.ShowHelp"))));
            // 
            // cbVersione
            // 
            this.cbVersione.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVersione.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.cbVersione, resources.GetString("cbVersione.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbVersione, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbVersione.HelpNavigator"))));
            resources.ApplyResources(this.cbVersione, "cbVersione");
            this.cbVersione.Name = "cbVersione";
            this.guidaFile.SetShowHelp(this.cbVersione, ((bool)(resources.GetObject("cbVersione.ShowHelp"))));
            this.cbVersione.SelectedIndexChanged += new System.EventHandler(this.cbVersione_SelectedIndexChanged);
            // 
            // gbOrdine
            // 
            this.gbOrdine.Controls.Add(this.rbPrimaApparenza);
            this.gbOrdine.Controls.Add(this.rbApparenze);
            this.gbOrdine.Controls.Add(this.rbAlfabetico);
            this.guidaFile.SetHelpKeyword(this.gbOrdine, resources.GetString("gbOrdine.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.gbOrdine, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("gbOrdine.HelpNavigator"))));
            resources.ApplyResources(this.gbOrdine, "gbOrdine");
            this.gbOrdine.Name = "gbOrdine";
            this.guidaFile.SetShowHelp(this.gbOrdine, ((bool)(resources.GetObject("gbOrdine.ShowHelp"))));
            this.gbOrdine.TabStop = false;
            // 
            // rbPrimaApparenza
            // 
            resources.ApplyResources(this.rbPrimaApparenza, "rbPrimaApparenza");
            this.guidaFile.SetHelpKeyword(this.rbPrimaApparenza, resources.GetString("rbPrimaApparenza.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.rbPrimaApparenza, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("rbPrimaApparenza.HelpNavigator"))));
            this.rbPrimaApparenza.Name = "rbPrimaApparenza";
            this.guidaFile.SetShowHelp(this.rbPrimaApparenza, ((bool)(resources.GetObject("rbPrimaApparenza.ShowHelp"))));
            this.rbPrimaApparenza.TabStop = true;
            this.rbPrimaApparenza.UseVisualStyleBackColor = true;
            // 
            // rbApparenze
            // 
            resources.ApplyResources(this.rbApparenze, "rbApparenze");
            this.guidaFile.SetHelpKeyword(this.rbApparenze, resources.GetString("rbApparenze.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.rbApparenze, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("rbApparenze.HelpNavigator"))));
            this.rbApparenze.Name = "rbApparenze";
            this.guidaFile.SetShowHelp(this.rbApparenze, ((bool)(resources.GetObject("rbApparenze.ShowHelp"))));
            this.rbApparenze.TabStop = true;
            this.rbApparenze.UseVisualStyleBackColor = true;
            // 
            // rbAlfabetico
            // 
            resources.ApplyResources(this.rbAlfabetico, "rbAlfabetico");
            this.guidaFile.SetHelpKeyword(this.rbAlfabetico, resources.GetString("rbAlfabetico.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.rbAlfabetico, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("rbAlfabetico.HelpNavigator"))));
            this.rbAlfabetico.Name = "rbAlfabetico";
            this.guidaFile.SetShowHelp(this.rbAlfabetico, ((bool)(resources.GetObject("rbAlfabetico.ShowHelp"))));
            this.rbAlfabetico.TabStop = true;
            this.rbAlfabetico.UseVisualStyleBackColor = true;
            // 
            // cbIpertesto
            // 
            resources.ApplyResources(this.cbIpertesto, "cbIpertesto");
            this.guidaFile.SetHelpKeyword(this.cbIpertesto, resources.GetString("cbIpertesto.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbIpertesto, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbIpertesto.HelpNavigator"))));
            this.cbIpertesto.Name = "cbIpertesto";
            this.guidaFile.SetShowHelp(this.cbIpertesto, ((bool)(resources.GetObject("cbIpertesto.ShowHelp"))));
            this.cbIpertesto.UseVisualStyleBackColor = true;
            // 
            // cbNonRadiciComuni
            // 
            resources.ApplyResources(this.cbNonRadiciComuni, "cbNonRadiciComuni");
            this.guidaFile.SetHelpKeyword(this.cbNonRadiciComuni, resources.GetString("cbNonRadiciComuni.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbNonRadiciComuni, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbNonRadiciComuni.HelpNavigator"))));
            this.cbNonRadiciComuni.Name = "cbNonRadiciComuni";
            this.guidaFile.SetShowHelp(this.cbNonRadiciComuni, ((bool)(resources.GetObject("cbNonRadiciComuni.ShowHelp"))));
            this.cbNonRadiciComuni.UseVisualStyleBackColor = true;
            this.cbNonRadiciComuni.CheckedChanged += new System.EventHandler(this.cbNonRadiciComuni_CheckedChanged);
            // 
            // gbParoleRadici
            // 
            this.gbParoleRadici.Controls.Add(this.rbRadici);
            this.gbParoleRadici.Controls.Add(this.rbParole);
            this.guidaFile.SetHelpKeyword(this.gbParoleRadici, resources.GetString("gbParoleRadici.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.gbParoleRadici, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("gbParoleRadici.HelpNavigator"))));
            resources.ApplyResources(this.gbParoleRadici, "gbParoleRadici");
            this.gbParoleRadici.Name = "gbParoleRadici";
            this.guidaFile.SetShowHelp(this.gbParoleRadici, ((bool)(resources.GetObject("gbParoleRadici.ShowHelp"))));
            this.gbParoleRadici.TabStop = false;
            // 
            // rbRadici
            // 
            resources.ApplyResources(this.rbRadici, "rbRadici");
            this.guidaFile.SetHelpKeyword(this.rbRadici, resources.GetString("rbRadici.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.rbRadici, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("rbRadici.HelpNavigator"))));
            this.rbRadici.Name = "rbRadici";
            this.guidaFile.SetShowHelp(this.rbRadici, ((bool)(resources.GetObject("rbRadici.ShowHelp"))));
            this.rbRadici.TabStop = true;
            this.rbRadici.UseVisualStyleBackColor = true;
            // 
            // rbParole
            // 
            resources.ApplyResources(this.rbParole, "rbParole");
            this.guidaFile.SetHelpKeyword(this.rbParole, resources.GetString("rbParole.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.rbParole, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("rbParole.HelpNavigator"))));
            this.rbParole.Name = "rbParole";
            this.guidaFile.SetShowHelp(this.rbParole, ((bool)(resources.GetObject("rbParole.ShowHelp"))));
            this.rbParole.TabStop = true;
            this.rbParole.UseVisualStyleBackColor = true;
            // 
            // tbNonRadiciComuni
            // 
            this.tbNonRadiciComuni.AcceptsReturn = true;
            resources.ApplyResources(this.tbNonRadiciComuni, "tbNonRadiciComuni");
            this.guidaFile.SetHelpKeyword(this.tbNonRadiciComuni, resources.GetString("tbNonRadiciComuni.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.tbNonRadiciComuni, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("tbNonRadiciComuni.HelpNavigator"))));
            this.tbNonRadiciComuni.Name = "tbNonRadiciComuni";
            this.guidaFile.SetShowHelp(this.tbNonRadiciComuni, ((bool)(resources.GetObject("tbNonRadiciComuni.ShowHelp"))));
            // 
            // etiNumeroMinimo
            // 
            resources.ApplyResources(this.etiNumeroMinimo, "etiNumeroMinimo");
            this.guidaFile.SetHelpKeyword(this.etiNumeroMinimo, resources.GetString("etiNumeroMinimo.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.etiNumeroMinimo, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiNumeroMinimo.HelpNavigator"))));
            this.etiNumeroMinimo.Name = "etiNumeroMinimo";
            this.guidaFile.SetShowHelp(this.etiNumeroMinimo, ((bool)(resources.GetObject("etiNumeroMinimo.ShowHelp"))));
            // 
            // udNumeroMinimo
            // 
            this.guidaFile.SetHelpKeyword(this.udNumeroMinimo, resources.GetString("udNumeroMinimo.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.udNumeroMinimo, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("udNumeroMinimo.HelpNavigator"))));
            resources.ApplyResources(this.udNumeroMinimo, "udNumeroMinimo");
            this.udNumeroMinimo.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.udNumeroMinimo.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udNumeroMinimo.Name = "udNumeroMinimo";
            this.guidaFile.SetShowHelp(this.udNumeroMinimo, ((bool)(resources.GetObject("udNumeroMinimo.ShowHelp"))));
            this.udNumeroMinimo.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // cbBrano
            // 
            this.cbBrano.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbBrano.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbBrano.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.cbBrano, resources.GetString("cbBrano.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbBrano, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbBrano.HelpNavigator"))));
            resources.ApplyResources(this.cbBrano, "cbBrano");
            this.cbBrano.Name = "cbBrano";
            this.guidaFile.SetShowHelp(this.cbBrano, ((bool)(resources.GetObject("cbBrano.ShowHelp"))));
            // 
            // cbRiferimenti
            // 
            resources.ApplyResources(this.cbRiferimenti, "cbRiferimenti");
            this.cbRiferimenti.Checked = true;
            this.cbRiferimenti.CheckState = System.Windows.Forms.CheckState.Checked;
            this.guidaFile.SetHelpKeyword(this.cbRiferimenti, resources.GetString("cbRiferimenti.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbRiferimenti, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbRiferimenti.HelpNavigator"))));
            this.cbRiferimenti.Name = "cbRiferimenti";
            this.guidaFile.SetShowHelp(this.cbRiferimenti, ((bool)(resources.GetObject("cbRiferimenti.ShowHelp"))));
            this.cbRiferimenti.UseVisualStyleBackColor = true;
            // 
            // cbDefinizioni
            // 
            resources.ApplyResources(this.cbDefinizioni, "cbDefinizioni");
            this.guidaFile.SetHelpKeyword(this.cbDefinizioni, resources.GetString("cbDefinizioni.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbDefinizioni, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbDefinizioni.HelpNavigator"))));
            this.cbDefinizioni.Name = "cbDefinizioni";
            this.guidaFile.SetShowHelp(this.cbDefinizioni, ((bool)(resources.GetObject("cbDefinizioni.ShowHelp"))));
            this.cbDefinizioni.Tag = "";
            this.cbDefinizioni.UseVisualStyleBackColor = true;
            // 
            // Chiave
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbDefinizioni);
            this.Controls.Add(this.cbRiferimenti);
            this.Controls.Add(this.cbBrano);
            this.Controls.Add(this.tbNonRadiciComuni);
            this.Controls.Add(this.cbNonRadiciComuni);
            this.Controls.Add(this.udNumeroMinimo);
            this.Controls.Add(this.etiNumeroMinimo);
            this.Controls.Add(this.cbVersione);
            this.Controls.Add(this.etiVersione);
            this.Controls.Add(this.gbParoleRadici);
            this.Controls.Add(this.cbIpertesto);
            this.Controls.Add(this.etiBrano);
            this.Controls.Add(this.gbOrdine);
            this.Name = "Chiave";
            this.guidaFile.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.Tag = "Chiave";
            this.Load += new System.EventHandler(this.Chiave_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Chiave_FormClosing);
            this.Controls.SetChildIndex(this.gbOrdine, 0);
            this.Controls.SetChildIndex(this.etiBrano, 0);
            this.Controls.SetChildIndex(this.cbIpertesto, 0);
            this.Controls.SetChildIndex(this.gbParoleRadici, 0);
            this.Controls.SetChildIndex(this.etiVersione, 0);
            this.Controls.SetChildIndex(this.cbVersione, 0);
            this.Controls.SetChildIndex(this.etiNumeroMinimo, 0);
            this.Controls.SetChildIndex(this.udNumeroMinimo, 0);
            this.Controls.SetChildIndex(this.cbNonRadiciComuni, 0);
            this.Controls.SetChildIndex(this.tbNonRadiciComuni, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnCanc, 0);
            this.Controls.SetChildIndex(this.cbBrano, 0);
            this.Controls.SetChildIndex(this.cbRiferimenti, 0);
            this.Controls.SetChildIndex(this.cbDefinizioni, 0);
            this.gbOrdine.ResumeLayout(false);
            this.gbOrdine.PerformLayout();
            this.gbParoleRadici.ResumeLayout(false);
            this.gbParoleRadici.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udNumeroMinimo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label etiBrano;
        private System.Windows.Forms.Label etiVersione;
        private System.Windows.Forms.ComboBox cbVersione;
        private System.Windows.Forms.GroupBox gbOrdine;
        private System.Windows.Forms.RadioButton rbPrimaApparenza;
        private System.Windows.Forms.RadioButton rbApparenze;
        private System.Windows.Forms.RadioButton rbAlfabetico;
        private System.Windows.Forms.CheckBox cbIpertesto;
        private System.Windows.Forms.CheckBox cbNonRadiciComuni;
        private System.Windows.Forms.GroupBox gbParoleRadici;
        private System.Windows.Forms.RadioButton rbRadici;
        private System.Windows.Forms.RadioButton rbParole;
        private System.Windows.Forms.TextBox tbNonRadiciComuni;
        private System.Windows.Forms.Label etiNumeroMinimo;
        private System.Windows.Forms.NumericUpDown udNumeroMinimo;
        private System.Windows.Forms.ComboBox cbBrano;
        private System.Windows.Forms.CheckBox cbRiferimenti;
        private System.Windows.Forms.CheckBox cbDefinizioni;
    }
}