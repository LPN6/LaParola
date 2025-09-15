namespace LaParola
{
    partial class Informazioni
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Informazioni));
            this.cbInfo = new System.Windows.Forms.ComboBox();
            this.pulRiferimento = new System.Windows.Forms.Button();
            this.pulTema = new System.Windows.Forms.Button();
            this.tvRisultati = new System.Windows.Forms.TreeView();
            this.etiInfo = new System.Windows.Forms.Label();
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
            this.btnCanc.TabStop = false;
            this.btnCanc.Click += new System.EventHandler(this.btnCanc_Click);
            // 
            // guidaFile
            // 
            this.guidaFile.HelpNamespace = null;
            // 
            // cbInfo
            // 
            this.cbInfo.AccessibleDescription = null;
            this.cbInfo.AccessibleName = null;
            resources.ApplyResources(this.cbInfo, "cbInfo");
            this.cbInfo.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbInfo.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbInfo.BackgroundImage = null;
            this.cbInfo.Font = null;
            this.cbInfo.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.cbInfo, resources.GetString("cbInfo.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbInfo, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbInfo.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbInfo, null);
            this.cbInfo.Name = "cbInfo";
            this.guidaFile.SetShowHelp(this.cbInfo, ((bool)(resources.GetObject("cbInfo.ShowHelp"))));
            this.cbInfo.SelectedIndexChanged += new System.EventHandler(this.cbInfo_TextChanged);
            this.cbInfo.TextChanged += new System.EventHandler(this.cbInfo_TextChanged);
            // 
            // pulRiferimento
            // 
            this.pulRiferimento.AccessibleDescription = null;
            this.pulRiferimento.AccessibleName = null;
            resources.ApplyResources(this.pulRiferimento, "pulRiferimento");
            this.pulRiferimento.BackgroundImage = null;
            this.pulRiferimento.Font = null;
            this.guidaFile.SetHelpKeyword(this.pulRiferimento, resources.GetString("pulRiferimento.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.pulRiferimento, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulRiferimento.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.pulRiferimento, null);
            this.pulRiferimento.Name = "pulRiferimento";
            this.guidaFile.SetShowHelp(this.pulRiferimento, ((bool)(resources.GetObject("pulRiferimento.ShowHelp"))));
            this.pulRiferimento.Tag = "Riferimento";
            this.pulRiferimento.UseVisualStyleBackColor = true;
            this.pulRiferimento.Click += new System.EventHandler(this.pulsante_Click);
            // 
            // pulTema
            // 
            this.pulTema.AccessibleDescription = null;
            this.pulTema.AccessibleName = null;
            resources.ApplyResources(this.pulTema, "pulTema");
            this.pulTema.BackgroundImage = null;
            this.pulTema.Font = null;
            this.guidaFile.SetHelpKeyword(this.pulTema, resources.GetString("pulTema.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.pulTema, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulTema.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.pulTema, null);
            this.pulTema.Name = "pulTema";
            this.guidaFile.SetShowHelp(this.pulTema, ((bool)(resources.GetObject("pulTema.ShowHelp"))));
            this.pulTema.Tag = "Tema";
            this.pulTema.UseVisualStyleBackColor = true;
            this.pulTema.Click += new System.EventHandler(this.pulsante_Click);
            // 
            // tvRisultati
            // 
            this.tvRisultati.AccessibleDescription = null;
            this.tvRisultati.AccessibleName = null;
            resources.ApplyResources(this.tvRisultati, "tvRisultati");
            this.tvRisultati.BackgroundImage = null;
            this.tvRisultati.Font = null;
            this.guidaFile.SetHelpKeyword(this.tvRisultati, resources.GetString("tvRisultati.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.tvRisultati, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("tvRisultati.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.tvRisultati, null);
            this.tvRisultati.HotTracking = true;
            this.tvRisultati.Name = "tvRisultati";
            this.guidaFile.SetShowHelp(this.tvRisultati, ((bool)(resources.GetObject("tvRisultati.ShowHelp"))));
            this.tvRisultati.DoubleClick += new System.EventHandler(this.tvRisultati_DoubleClick);
            // 
            // etiInfo
            // 
            this.etiInfo.AccessibleDescription = null;
            this.etiInfo.AccessibleName = null;
            resources.ApplyResources(this.etiInfo, "etiInfo");
            this.etiInfo.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiInfo, null);
            this.guidaFile.SetHelpNavigator(this.etiInfo, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiInfo.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiInfo, null);
            this.etiInfo.Name = "etiInfo";
            this.guidaFile.SetShowHelp(this.etiInfo, ((bool)(resources.GetObject("etiInfo.ShowHelp"))));
            // 
            // Informazioni
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.tvRisultati);
            this.Controls.Add(this.pulRiferimento);
            this.Controls.Add(this.etiInfo);
            this.Controls.Add(this.pulTema);
            this.Controls.Add(this.cbInfo);
            this.Font = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.guidaFile.SetHelpKeyword(this, null);
            this.guidaFile.SetHelpNavigator(this, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("$this.HelpNavigator"))));
            this.guidaFile.SetHelpString(this, null);
            this.Name = "Informazioni";
            this.guidaFile.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Tag = "Informazioni";
            this.Load += new System.EventHandler(this.Informazioni_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Informazioni_FormClosing);
            this.Resize += new System.EventHandler(this.Informazioni_Resize);
            this.Controls.SetChildIndex(this.cbInfo, 0);
            this.Controls.SetChildIndex(this.pulTema, 0);
            this.Controls.SetChildIndex(this.etiInfo, 0);
            this.Controls.SetChildIndex(this.pulRiferimento, 0);
            this.Controls.SetChildIndex(this.tvRisultati, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnCanc, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cbInfo;
        private System.Windows.Forms.Button pulRiferimento;
        private System.Windows.Forms.Button pulTema;
        private System.Windows.Forms.TreeView tvRisultati;
        private System.Windows.Forms.Label etiInfo;
    }
}