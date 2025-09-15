namespace LaParola
{
    partial class Ricerca
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
                if (font != null)
                    font.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Ricerca));
            this.cbEspressione = new System.Windows.Forms.ComboBox();
            this.cbVersione = new System.Windows.Forms.ComboBox();
            this.labEspressione = new System.Windows.Forms.Label();
            this.labVersione = new System.Windows.Forms.Label();
            this.btnScegliParola = new System.Windows.Forms.Button();
            this.gbBrano = new System.Windows.Forms.GroupBox();
            this.pulListaVersettiNuova = new System.Windows.Forms.Button();
            this.cbListaVersetti = new System.Windows.Forms.ComboBox();
            this.rbListaVersetti = new System.Windows.Forms.RadioButton();
            this.cbBrano = new System.Windows.Forms.ComboBox();
            this.cbParte = new System.Windows.Forms.ComboBox();
            this.rbBrano = new System.Windows.Forms.RadioButton();
            this.rbParte = new System.Windows.Forms.RadioButton();
            this.cbSalvaListaVersetti = new System.Windows.Forms.CheckBox();
            this.gbBrano.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.guidaFile.SetShowHelp(this.btnOK, ((bool)(resources.GetObject("btnOK.ShowHelp"))));
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCanc
            // 
            this.guidaFile.SetShowHelp(this.btnCanc, ((bool)(resources.GetObject("btnCanc.ShowHelp"))));
            resources.ApplyResources(this.btnCanc, "btnCanc");
            this.btnCanc.Click += new System.EventHandler(this.btnCanc_Click);
            // 
            // cbEspressione
            // 
            this.cbEspressione.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbEspressione.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbEspressione.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.cbEspressione, resources.GetString("cbEspressione.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbEspressione, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbEspressione.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbEspressione, resources.GetString("cbEspressione.HelpString"));
            resources.ApplyResources(this.cbEspressione, "cbEspressione");
            this.cbEspressione.Name = "cbEspressione";
            this.guidaFile.SetShowHelp(this.cbEspressione, ((bool)(resources.GetObject("cbEspressione.ShowHelp"))));
            this.cbEspressione.SelectedIndexChanged += new System.EventHandler(this.cbEspressione_TextChanged);
            this.cbEspressione.TextChanged += new System.EventHandler(this.cbEspressione_TextChanged);
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
            // labEspressione
            // 
            resources.ApplyResources(this.labEspressione, "labEspressione");
            this.labEspressione.Name = "labEspressione";
            this.guidaFile.SetShowHelp(this.labEspressione, ((bool)(resources.GetObject("labEspressione.ShowHelp"))));
            // 
            // labVersione
            // 
            resources.ApplyResources(this.labVersione, "labVersione");
            this.labVersione.Name = "labVersione";
            this.guidaFile.SetShowHelp(this.labVersione, ((bool)(resources.GetObject("labVersione.ShowHelp"))));
            // 
            // btnScegliParola
            // 
            this.guidaFile.SetHelpKeyword(this.btnScegliParola, resources.GetString("btnScegliParola.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.btnScegliParola, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("btnScegliParola.HelpNavigator"))));
            resources.ApplyResources(this.btnScegliParola, "btnScegliParola");
            this.btnScegliParola.Name = "btnScegliParola";
            this.guidaFile.SetShowHelp(this.btnScegliParola, ((bool)(resources.GetObject("btnScegliParola.ShowHelp"))));
            this.btnScegliParola.TabStop = false;
            this.btnScegliParola.UseVisualStyleBackColor = true;
            this.btnScegliParola.Click += new System.EventHandler(this.btnScegliParola_Click);
            // 
            // gbBrano
            // 
            this.gbBrano.Controls.Add(this.pulListaVersettiNuova);
            this.gbBrano.Controls.Add(this.cbListaVersetti);
            this.gbBrano.Controls.Add(this.rbListaVersetti);
            this.gbBrano.Controls.Add(this.cbBrano);
            this.gbBrano.Controls.Add(this.cbParte);
            this.gbBrano.Controls.Add(this.rbBrano);
            this.gbBrano.Controls.Add(this.rbParte);
            resources.ApplyResources(this.gbBrano, "gbBrano");
            this.gbBrano.Name = "gbBrano";
            this.guidaFile.SetShowHelp(this.gbBrano, ((bool)(resources.GetObject("gbBrano.ShowHelp"))));
            this.gbBrano.TabStop = false;
            // 
            // pulListaVersettiNuova
            // 
            this.guidaFile.SetHelpKeyword(this.pulListaVersettiNuova, resources.GetString("pulListaVersettiNuova.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.pulListaVersettiNuova, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulListaVersettiNuova.HelpNavigator"))));
            resources.ApplyResources(this.pulListaVersettiNuova, "pulListaVersettiNuova");
            this.pulListaVersettiNuova.Name = "pulListaVersettiNuova";
            this.guidaFile.SetShowHelp(this.pulListaVersettiNuova, ((bool)(resources.GetObject("pulListaVersettiNuova.ShowHelp"))));
            this.pulListaVersettiNuova.UseVisualStyleBackColor = true;
            this.pulListaVersettiNuova.Click += new System.EventHandler(this.pulListaVersettiNuova_Click);
            // 
            // cbListaVersetti
            // 
            this.cbListaVersetti.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.cbListaVersetti, "cbListaVersetti");
            this.cbListaVersetti.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.cbListaVersetti, resources.GetString("cbListaVersetti.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbListaVersetti, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbListaVersetti.HelpNavigator"))));
            this.cbListaVersetti.Name = "cbListaVersetti";
            this.guidaFile.SetShowHelp(this.cbListaVersetti, ((bool)(resources.GetObject("cbListaVersetti.ShowHelp"))));
            // 
            // rbListaVersetti
            // 
            resources.ApplyResources(this.rbListaVersetti, "rbListaVersetti");
            this.guidaFile.SetHelpKeyword(this.rbListaVersetti, resources.GetString("rbListaVersetti.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.rbListaVersetti, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("rbListaVersetti.HelpNavigator"))));
            this.rbListaVersetti.Name = "rbListaVersetti";
            this.guidaFile.SetShowHelp(this.rbListaVersetti, ((bool)(resources.GetObject("rbListaVersetti.ShowHelp"))));
            this.rbListaVersetti.UseVisualStyleBackColor = true;
            this.rbListaVersetti.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // cbBrano
            // 
            this.cbBrano.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbBrano.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            resources.ApplyResources(this.cbBrano, "cbBrano");
            this.cbBrano.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.cbBrano, resources.GetString("cbBrano.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbBrano, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbBrano.HelpNavigator"))));
            this.cbBrano.Name = "cbBrano";
            this.guidaFile.SetShowHelp(this.cbBrano, ((bool)(resources.GetObject("cbBrano.ShowHelp"))));
            // 
            // cbParte
            // 
            this.cbParte.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbParte.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.cbParte, resources.GetString("cbParte.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbParte, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbParte.HelpNavigator"))));
            this.cbParte.Items.AddRange(new object[] {
            resources.GetString("cbParte.Items"),
            resources.GetString("cbParte.Items1"),
            resources.GetString("cbParte.Items2"),
            resources.GetString("cbParte.Items3"),
            resources.GetString("cbParte.Items4"),
            resources.GetString("cbParte.Items5"),
            resources.GetString("cbParte.Items6"),
            resources.GetString("cbParte.Items7"),
            resources.GetString("cbParte.Items8"),
            resources.GetString("cbParte.Items9"),
            resources.GetString("cbParte.Items10"),
            resources.GetString("cbParte.Items11")});
            resources.ApplyResources(this.cbParte, "cbParte");
            this.cbParte.Name = "cbParte";
            this.guidaFile.SetShowHelp(this.cbParte, ((bool)(resources.GetObject("cbParte.ShowHelp"))));
            // 
            // rbBrano
            // 
            resources.ApplyResources(this.rbBrano, "rbBrano");
            this.guidaFile.SetHelpKeyword(this.rbBrano, resources.GetString("rbBrano.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.rbBrano, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("rbBrano.HelpNavigator"))));
            this.rbBrano.Name = "rbBrano";
            this.guidaFile.SetShowHelp(this.rbBrano, ((bool)(resources.GetObject("rbBrano.ShowHelp"))));
            this.rbBrano.UseVisualStyleBackColor = true;
            this.rbBrano.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // rbParte
            // 
            resources.ApplyResources(this.rbParte, "rbParte");
            this.rbParte.Checked = true;
            this.guidaFile.SetHelpKeyword(this.rbParte, resources.GetString("rbParte.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.rbParte, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("rbParte.HelpNavigator"))));
            this.rbParte.Name = "rbParte";
            this.guidaFile.SetShowHelp(this.rbParte, ((bool)(resources.GetObject("rbParte.ShowHelp"))));
            this.rbParte.TabStop = true;
            this.rbParte.UseVisualStyleBackColor = true;
            this.rbParte.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
            // 
            // cbSalvaListaVersetti
            // 
            resources.ApplyResources(this.cbSalvaListaVersetti, "cbSalvaListaVersetti");
            this.guidaFile.SetHelpKeyword(this.cbSalvaListaVersetti, resources.GetString("cbSalvaListaVersetti.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbSalvaListaVersetti, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbSalvaListaVersetti.HelpNavigator"))));
            this.cbSalvaListaVersetti.Name = "cbSalvaListaVersetti";
            this.guidaFile.SetShowHelp(this.cbSalvaListaVersetti, ((bool)(resources.GetObject("cbSalvaListaVersetti.ShowHelp"))));
            this.cbSalvaListaVersetti.UseVisualStyleBackColor = true;
            // 
            // Ricerca
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbBrano);
            this.Controls.Add(this.labEspressione);
            this.Controls.Add(this.cbEspressione);
            this.Controls.Add(this.btnScegliParola);
            this.Controls.Add(this.labVersione);
            this.Controls.Add(this.cbVersione);
            this.Controls.Add(this.cbSalvaListaVersetti);
            this.Name = "Ricerca";
            this.guidaFile.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.Tag = "Ricerca";
            this.Load += new System.EventHandler(this.Ricerca_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Ricerca_FormClosing);
            this.Controls.SetChildIndex(this.cbSalvaListaVersetti, 0);
            this.Controls.SetChildIndex(this.cbVersione, 0);
            this.Controls.SetChildIndex(this.labVersione, 0);
            this.Controls.SetChildIndex(this.btnScegliParola, 0);
            this.Controls.SetChildIndex(this.cbEspressione, 0);
            this.Controls.SetChildIndex(this.labEspressione, 0);
            this.Controls.SetChildIndex(this.gbBrano, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnCanc, 0);
            this.gbBrano.ResumeLayout(false);
            this.gbBrano.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbVersione;
        private System.Windows.Forms.Label labEspressione;
        private System.Windows.Forms.Label labVersione;
        private System.Windows.Forms.Button btnScegliParola;
        private System.Windows.Forms.ComboBox cbEspressione;
        private System.Windows.Forms.GroupBox gbBrano;
        private System.Windows.Forms.ComboBox cbBrano;
        private System.Windows.Forms.ComboBox cbParte;
        private System.Windows.Forms.RadioButton rbBrano;
        private System.Windows.Forms.RadioButton rbParte;
        private System.Windows.Forms.ComboBox cbListaVersetti;
        private System.Windows.Forms.RadioButton rbListaVersetti;
        private System.Windows.Forms.CheckBox cbSalvaListaVersetti;
        private System.Windows.Forms.Button pulListaVersettiNuova;
    }
}