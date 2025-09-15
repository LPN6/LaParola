namespace LaParola
{
    partial class GestisciTesti
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GestisciTesti));
            this.etiAzione = new System.Windows.Forms.Label();
            this.cbAzioni = new System.Windows.Forms.ComboBox();
            this.cbCollezioni = new System.Windows.Forms.ComboBox();
            this.etiCancellaCollezione = new System.Windows.Forms.Label();
            this.etiCancellaCollezioneNessuna = new System.Windows.Forms.Label();
            this.cbCollezioni2 = new System.Windows.Forms.ComboBox();
            this.etiCollezione2 = new System.Windows.Forms.Label();
            this.cbLasciaAperta = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // guidaFile
            // 
            this.guidaFile.HelpNamespace = null;
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
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
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
            // 
            // etiAzione
            // 
            this.etiAzione.AccessibleDescription = null;
            this.etiAzione.AccessibleName = null;
            resources.ApplyResources(this.etiAzione, "etiAzione");
            this.etiAzione.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiAzione, resources.GetString("etiAzione.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.etiAzione, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiAzione.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiAzione, null);
            this.etiAzione.Name = "etiAzione";
            this.guidaFile.SetShowHelp(this.etiAzione, ((bool)(resources.GetObject("etiAzione.ShowHelp"))));
            // 
            // cbAzioni
            // 
            this.cbAzioni.AccessibleDescription = null;
            this.cbAzioni.AccessibleName = null;
            resources.ApplyResources(this.cbAzioni, "cbAzioni");
            this.cbAzioni.BackgroundImage = null;
            this.cbAzioni.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAzioni.Font = null;
            this.cbAzioni.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.cbAzioni, resources.GetString("cbAzioni.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbAzioni, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbAzioni.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbAzioni, null);
            this.cbAzioni.Items.AddRange(new object[] {
            resources.GetString("cbAzioni.Items"),
            resources.GetString("cbAzioni.Items1"),
            resources.GetString("cbAzioni.Items2"),
            resources.GetString("cbAzioni.Items3"),
            resources.GetString("cbAzioni.Items4"),
            resources.GetString("cbAzioni.Items5"),
            resources.GetString("cbAzioni.Items6"),
            resources.GetString("cbAzioni.Items7"),
            resources.GetString("cbAzioni.Items8")});
            this.cbAzioni.Name = "cbAzioni";
            this.guidaFile.SetShowHelp(this.cbAzioni, ((bool)(resources.GetObject("cbAzioni.ShowHelp"))));
            this.cbAzioni.SelectedIndexChanged += new System.EventHandler(this.cbAzioni_SelectedIndexChanged);
            // 
            // cbCollezioni
            // 
            this.cbCollezioni.AccessibleDescription = null;
            this.cbCollezioni.AccessibleName = null;
            resources.ApplyResources(this.cbCollezioni, "cbCollezioni");
            this.cbCollezioni.BackgroundImage = null;
            this.cbCollezioni.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCollezioni.Font = null;
            this.cbCollezioni.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.cbCollezioni, resources.GetString("cbCollezioni.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbCollezioni, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbCollezioni.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbCollezioni, null);
            this.cbCollezioni.Name = "cbCollezioni";
            this.guidaFile.SetShowHelp(this.cbCollezioni, ((bool)(resources.GetObject("cbCollezioni.ShowHelp"))));
            // 
            // etiCancellaCollezione
            // 
            this.etiCancellaCollezione.AccessibleDescription = null;
            this.etiCancellaCollezione.AccessibleName = null;
            resources.ApplyResources(this.etiCancellaCollezione, "etiCancellaCollezione");
            this.etiCancellaCollezione.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiCancellaCollezione, resources.GetString("etiCancellaCollezione.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.etiCancellaCollezione, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiCancellaCollezione.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiCancellaCollezione, null);
            this.etiCancellaCollezione.Name = "etiCancellaCollezione";
            this.guidaFile.SetShowHelp(this.etiCancellaCollezione, ((bool)(resources.GetObject("etiCancellaCollezione.ShowHelp"))));
            // 
            // etiCancellaCollezioneNessuna
            // 
            this.etiCancellaCollezioneNessuna.AccessibleDescription = null;
            this.etiCancellaCollezioneNessuna.AccessibleName = null;
            resources.ApplyResources(this.etiCancellaCollezioneNessuna, "etiCancellaCollezioneNessuna");
            this.etiCancellaCollezioneNessuna.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiCancellaCollezioneNessuna, null);
            this.guidaFile.SetHelpNavigator(this.etiCancellaCollezioneNessuna, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiCancellaCollezioneNessuna.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiCancellaCollezioneNessuna, null);
            this.etiCancellaCollezioneNessuna.Name = "etiCancellaCollezioneNessuna";
            this.guidaFile.SetShowHelp(this.etiCancellaCollezioneNessuna, ((bool)(resources.GetObject("etiCancellaCollezioneNessuna.ShowHelp"))));
            // 
            // cbCollezioni2
            // 
            this.cbCollezioni2.AccessibleDescription = null;
            this.cbCollezioni2.AccessibleName = null;
            resources.ApplyResources(this.cbCollezioni2, "cbCollezioni2");
            this.cbCollezioni2.BackgroundImage = null;
            this.cbCollezioni2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCollezioni2.Font = null;
            this.cbCollezioni2.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.cbCollezioni2, resources.GetString("cbCollezioni2.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbCollezioni2, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbCollezioni2.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbCollezioni2, null);
            this.cbCollezioni2.Name = "cbCollezioni2";
            this.guidaFile.SetShowHelp(this.cbCollezioni2, ((bool)(resources.GetObject("cbCollezioni2.ShowHelp"))));
            // 
            // etiCollezione2
            // 
            this.etiCollezione2.AccessibleDescription = null;
            this.etiCollezione2.AccessibleName = null;
            resources.ApplyResources(this.etiCollezione2, "etiCollezione2");
            this.etiCollezione2.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiCollezione2, resources.GetString("etiCollezione2.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.etiCollezione2, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiCollezione2.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiCollezione2, null);
            this.etiCollezione2.Name = "etiCollezione2";
            this.guidaFile.SetShowHelp(this.etiCollezione2, ((bool)(resources.GetObject("etiCollezione2.ShowHelp"))));
            // 
            // cbLasciaAperta
            // 
            this.cbLasciaAperta.AccessibleDescription = null;
            this.cbLasciaAperta.AccessibleName = null;
            resources.ApplyResources(this.cbLasciaAperta, "cbLasciaAperta");
            this.cbLasciaAperta.BackgroundImage = null;
            this.cbLasciaAperta.Font = null;
            this.guidaFile.SetHelpKeyword(this.cbLasciaAperta, null);
            this.guidaFile.SetHelpNavigator(this.cbLasciaAperta, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbLasciaAperta.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbLasciaAperta, null);
            this.cbLasciaAperta.Name = "cbLasciaAperta";
            this.guidaFile.SetShowHelp(this.cbLasciaAperta, ((bool)(resources.GetObject("cbLasciaAperta.ShowHelp"))));
            this.cbLasciaAperta.UseVisualStyleBackColor = true;
            // 
            // GestisciTesti
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.etiCollezione2);
            this.Controls.Add(this.cbLasciaAperta);
            this.Controls.Add(this.cbAzioni);
            this.Controls.Add(this.cbCollezioni2);
            this.Controls.Add(this.etiAzione);
            this.Controls.Add(this.etiCancellaCollezione);
            this.Controls.Add(this.etiCancellaCollezioneNessuna);
            this.Controls.Add(this.cbCollezioni);
            this.Font = null;
            this.guidaFile.SetHelpKeyword(this, null);
            this.guidaFile.SetHelpNavigator(this, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("$this.HelpNavigator"))));
            this.guidaFile.SetHelpString(this, null);
            this.Name = "GestisciTesti";
            this.guidaFile.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.Controls.SetChildIndex(this.cbCollezioni, 0);
            this.Controls.SetChildIndex(this.etiCancellaCollezioneNessuna, 0);
            this.Controls.SetChildIndex(this.etiCancellaCollezione, 0);
            this.Controls.SetChildIndex(this.etiAzione, 0);
            this.Controls.SetChildIndex(this.cbCollezioni2, 0);
            this.Controls.SetChildIndex(this.cbAzioni, 0);
            this.Controls.SetChildIndex(this.cbLasciaAperta, 0);
            this.Controls.SetChildIndex(this.etiCollezione2, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnCanc, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label etiAzione;
        private System.Windows.Forms.ComboBox cbAzioni;
        private System.Windows.Forms.ComboBox cbCollezioni;
        private System.Windows.Forms.Label etiCancellaCollezione;
        private System.Windows.Forms.Label etiCancellaCollezioneNessuna;
        private System.Windows.Forms.ComboBox cbCollezioni2;
        private System.Windows.Forms.Label etiCollezione2;
        private System.Windows.Forms.CheckBox cbLasciaAperta;
    }
}