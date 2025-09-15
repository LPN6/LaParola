namespace Light
{
    partial class Principale
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Principale));
            this.fileGuida = new System.Windows.Forms.HelpProvider();
            this.cbEspressione = new System.Windows.Forms.ComboBox();
            this.cbVersione = new System.Windows.Forms.ComboBox();
            this.pulEsegui = new System.Windows.Forms.Button();
            this.pulGuida = new System.Windows.Forms.Button();
            this.pulChiudi = new System.Windows.Forms.Button();
            this.panControlli = new System.Windows.Forms.Panel();
            this.etiVersione = new System.Windows.Forms.Label();
            this.etiEspressione = new System.Windows.Forms.Label();
            this.msRtf = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.msiCopiaTutto = new System.Windows.Forms.ToolStripMenuItem();
            this.msiStampaTutto = new System.Windows.Forms.ToolStripMenuItem();
            this.msiSelezionaTutto = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.msiCopiaSelezione = new System.Windows.Forms.ToolStripMenuItem();
            this.msiStampaSeleziona = new System.Windows.Forms.ToolStripMenuItem();
            this.rtTesto = new TestiBiblici.RichTextBoxEx();
            this.printDocument = new System.Drawing.Printing.PrintDocument();
            this.panControlli.SuspendLayout();
            this.msRtf.SuspendLayout();
            this.SuspendLayout();
            // 
            // fileGuida
            // 
            resources.ApplyResources(this.fileGuida, "fileGuida");
            // 
            // cbEspressione
            // 
            this.cbEspressione.AccessibleDescription = null;
            this.cbEspressione.AccessibleName = null;
            resources.ApplyResources(this.cbEspressione, "cbEspressione");
            this.cbEspressione.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbEspressione.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbEspressione.BackgroundImage = null;
            this.cbEspressione.Font = null;
            this.cbEspressione.FormattingEnabled = true;
            this.fileGuida.SetHelpKeyword(this.cbEspressione, null);
            this.fileGuida.SetHelpNavigator(this.cbEspressione, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbEspressione.HelpNavigator"))));
            this.fileGuida.SetHelpString(this.cbEspressione, null);
            this.cbEspressione.Name = "cbEspressione";
            this.fileGuida.SetShowHelp(this.cbEspressione, ((bool)(resources.GetObject("cbEspressione.ShowHelp"))));
            this.cbEspressione.SelectedIndexChanged += new System.EventHandler(this.cbEspressione_SelectedIndexChanged);
            this.cbEspressione.TextUpdate += new System.EventHandler(this.cbEspressione_TextChanged);
            // 
            // cbVersione
            // 
            this.cbVersione.AccessibleDescription = null;
            this.cbVersione.AccessibleName = null;
            resources.ApplyResources(this.cbVersione, "cbVersione");
            this.cbVersione.BackgroundImage = null;
            this.cbVersione.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVersione.Font = null;
            this.cbVersione.FormattingEnabled = true;
            this.fileGuida.SetHelpKeyword(this.cbVersione, null);
            this.fileGuida.SetHelpNavigator(this.cbVersione, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbVersione.HelpNavigator"))));
            this.fileGuida.SetHelpString(this.cbVersione, null);
            this.cbVersione.Name = "cbVersione";
            this.fileGuida.SetShowHelp(this.cbVersione, ((bool)(resources.GetObject("cbVersione.ShowHelp"))));
            this.cbVersione.SelectedIndexChanged += new System.EventHandler(this.cbVersione_SelectedIndexChanged);
            // 
            // pulEsegui
            // 
            this.pulEsegui.AccessibleDescription = null;
            this.pulEsegui.AccessibleName = null;
            resources.ApplyResources(this.pulEsegui, "pulEsegui");
            this.pulEsegui.BackgroundImage = null;
            this.pulEsegui.Font = null;
            this.fileGuida.SetHelpKeyword(this.pulEsegui, null);
            this.fileGuida.SetHelpNavigator(this.pulEsegui, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulEsegui.HelpNavigator"))));
            this.fileGuida.SetHelpString(this.pulEsegui, null);
            this.pulEsegui.Image = global::Light.Properties.Resources.vai;
            this.pulEsegui.Name = "pulEsegui";
            this.fileGuida.SetShowHelp(this.pulEsegui, ((bool)(resources.GetObject("pulEsegui.ShowHelp"))));
            this.pulEsegui.UseVisualStyleBackColor = true;
            this.pulEsegui.Click += new System.EventHandler(this.pulEsegui_Click);
            // 
            // pulGuida
            // 
            this.pulGuida.AccessibleDescription = null;
            this.pulGuida.AccessibleName = null;
            resources.ApplyResources(this.pulGuida, "pulGuida");
            this.pulGuida.BackgroundImage = null;
            this.pulGuida.Font = null;
            this.fileGuida.SetHelpKeyword(this.pulGuida, null);
            this.fileGuida.SetHelpNavigator(this.pulGuida, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulGuida.HelpNavigator"))));
            this.fileGuida.SetHelpString(this.pulGuida, null);
            this.pulGuida.Image = global::Light.Properties.Resources.guida;
            this.pulGuida.Name = "pulGuida";
            this.fileGuida.SetShowHelp(this.pulGuida, ((bool)(resources.GetObject("pulGuida.ShowHelp"))));
            this.pulGuida.UseVisualStyleBackColor = true;
            this.pulGuida.Click += new System.EventHandler(this.pulGuida_Click);
            // 
            // pulChiudi
            // 
            this.pulChiudi.AccessibleDescription = null;
            this.pulChiudi.AccessibleName = null;
            resources.ApplyResources(this.pulChiudi, "pulChiudi");
            this.pulChiudi.BackgroundImage = null;
            this.pulChiudi.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.pulChiudi.Font = null;
            this.fileGuida.SetHelpKeyword(this.pulChiudi, null);
            this.fileGuida.SetHelpNavigator(this.pulChiudi, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulChiudi.HelpNavigator"))));
            this.fileGuida.SetHelpString(this.pulChiudi, null);
            this.pulChiudi.Image = global::Light.Properties.Resources.cancella;
            this.pulChiudi.Name = "pulChiudi";
            this.fileGuida.SetShowHelp(this.pulChiudi, ((bool)(resources.GetObject("pulChiudi.ShowHelp"))));
            this.pulChiudi.UseVisualStyleBackColor = true;
            this.pulChiudi.Click += new System.EventHandler(this.pulChiudi_Click);
            // 
            // panControlli
            // 
            this.panControlli.AccessibleDescription = null;
            this.panControlli.AccessibleName = null;
            resources.ApplyResources(this.panControlli, "panControlli");
            this.panControlli.BackgroundImage = null;
            this.panControlli.Controls.Add(this.etiVersione);
            this.panControlli.Controls.Add(this.etiEspressione);
            this.panControlli.Controls.Add(this.pulChiudi);
            this.panControlli.Controls.Add(this.pulGuida);
            this.panControlli.Controls.Add(this.pulEsegui);
            this.panControlli.Controls.Add(this.cbVersione);
            this.panControlli.Controls.Add(this.cbEspressione);
            this.panControlli.Font = null;
            this.fileGuida.SetHelpKeyword(this.panControlli, null);
            this.fileGuida.SetHelpNavigator(this.panControlli, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("panControlli.HelpNavigator"))));
            this.fileGuida.SetHelpString(this.panControlli, null);
            this.panControlli.Name = "panControlli";
            this.fileGuida.SetShowHelp(this.panControlli, ((bool)(resources.GetObject("panControlli.ShowHelp"))));
            // 
            // etiVersione
            // 
            this.etiVersione.AccessibleDescription = null;
            this.etiVersione.AccessibleName = null;
            resources.ApplyResources(this.etiVersione, "etiVersione");
            this.etiVersione.Font = null;
            this.fileGuida.SetHelpKeyword(this.etiVersione, null);
            this.fileGuida.SetHelpNavigator(this.etiVersione, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiVersione.HelpNavigator"))));
            this.fileGuida.SetHelpString(this.etiVersione, null);
            this.etiVersione.Name = "etiVersione";
            this.fileGuida.SetShowHelp(this.etiVersione, ((bool)(resources.GetObject("etiVersione.ShowHelp"))));
            // 
            // etiEspressione
            // 
            this.etiEspressione.AccessibleDescription = null;
            this.etiEspressione.AccessibleName = null;
            resources.ApplyResources(this.etiEspressione, "etiEspressione");
            this.etiEspressione.Font = null;
            this.fileGuida.SetHelpKeyword(this.etiEspressione, null);
            this.fileGuida.SetHelpNavigator(this.etiEspressione, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiEspressione.HelpNavigator"))));
            this.fileGuida.SetHelpString(this.etiEspressione, null);
            this.etiEspressione.Name = "etiEspressione";
            this.fileGuida.SetShowHelp(this.etiEspressione, ((bool)(resources.GetObject("etiEspressione.ShowHelp"))));
            // 
            // msRtf
            // 
            this.msRtf.AccessibleDescription = null;
            this.msRtf.AccessibleName = null;
            resources.ApplyResources(this.msRtf, "msRtf");
            this.msRtf.BackgroundImage = null;
            this.msRtf.Font = null;
            this.fileGuida.SetHelpKeyword(this.msRtf, null);
            this.fileGuida.SetHelpNavigator(this.msRtf, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("msRtf.HelpNavigator"))));
            this.fileGuida.SetHelpString(this.msRtf, null);
            this.msRtf.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.msiCopiaTutto,
            this.msiStampaTutto,
            this.msiSelezionaTutto,
            this.toolStripMenuItem1,
            this.msiCopiaSelezione,
            this.msiStampaSeleziona});
            this.msRtf.Name = "msRtf";
            this.fileGuida.SetShowHelp(this.msRtf, ((bool)(resources.GetObject("msRtf.ShowHelp"))));
            this.msRtf.Opening += new System.ComponentModel.CancelEventHandler(this.msRtf_Opening);
            // 
            // msiCopiaTutto
            // 
            this.msiCopiaTutto.AccessibleDescription = null;
            this.msiCopiaTutto.AccessibleName = null;
            resources.ApplyResources(this.msiCopiaTutto, "msiCopiaTutto");
            this.msiCopiaTutto.BackgroundImage = null;
            this.msiCopiaTutto.Name = "msiCopiaTutto";
            this.msiCopiaTutto.ShortcutKeyDisplayString = null;
            this.msiCopiaTutto.Click += new System.EventHandler(this.msiCopiaTutto_Click);
            // 
            // msiStampaTutto
            // 
            this.msiStampaTutto.AccessibleDescription = null;
            this.msiStampaTutto.AccessibleName = null;
            resources.ApplyResources(this.msiStampaTutto, "msiStampaTutto");
            this.msiStampaTutto.BackgroundImage = null;
            this.msiStampaTutto.Name = "msiStampaTutto";
            this.msiStampaTutto.ShortcutKeyDisplayString = null;
            this.msiStampaTutto.Click += new System.EventHandler(this.msiStampaTutto_Click);
            // 
            // msiSelezionaTutto
            // 
            this.msiSelezionaTutto.AccessibleDescription = null;
            this.msiSelezionaTutto.AccessibleName = null;
            resources.ApplyResources(this.msiSelezionaTutto, "msiSelezionaTutto");
            this.msiSelezionaTutto.BackgroundImage = null;
            this.msiSelezionaTutto.Name = "msiSelezionaTutto";
            this.msiSelezionaTutto.ShortcutKeyDisplayString = null;
            this.msiSelezionaTutto.Click += new System.EventHandler(this.msiSelezionaTutto_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.AccessibleDescription = null;
            this.toolStripMenuItem1.AccessibleName = null;
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            // 
            // msiCopiaSelezione
            // 
            this.msiCopiaSelezione.AccessibleDescription = null;
            this.msiCopiaSelezione.AccessibleName = null;
            resources.ApplyResources(this.msiCopiaSelezione, "msiCopiaSelezione");
            this.msiCopiaSelezione.BackgroundImage = null;
            this.msiCopiaSelezione.Name = "msiCopiaSelezione";
            this.msiCopiaSelezione.ShortcutKeyDisplayString = null;
            this.msiCopiaSelezione.Click += new System.EventHandler(this.msiCopiaSelezione_Click);
            // 
            // msiStampaSeleziona
            // 
            this.msiStampaSeleziona.AccessibleDescription = null;
            this.msiStampaSeleziona.AccessibleName = null;
            resources.ApplyResources(this.msiStampaSeleziona, "msiStampaSeleziona");
            this.msiStampaSeleziona.BackgroundImage = null;
            this.msiStampaSeleziona.Name = "msiStampaSeleziona";
            this.msiStampaSeleziona.ShortcutKeyDisplayString = null;
            this.msiStampaSeleziona.Click += new System.EventHandler(this.msiStampaSeleziona_Click);
            // 
            // rtTesto
            // 
            this.rtTesto.AccessibleDescription = null;
            this.rtTesto.AccessibleName = null;
            resources.ApplyResources(this.rtTesto, "rtTesto");
            this.rtTesto.BackgroundImage = null;
            this.rtTesto.ContextMenuStrip = this.msRtf;
            this.rtTesto.Font = null;
            this.fileGuida.SetHelpKeyword(this.rtTesto, null);
            this.fileGuida.SetHelpNavigator(this.rtTesto, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("rtTesto.HelpNavigator"))));
            this.fileGuida.SetHelpString(this.rtTesto, null);
            this.rtTesto.Lingua = null;
            this.rtTesto.Name = "rtTesto";
            this.rtTesto.SelectionAlignment = TestiBiblici.RichTextBoxEx.TextAlign.Left;
            this.fileGuida.SetShowHelp(this.rtTesto, ((bool)(resources.GetObject("rtTesto.ShowHelp"))));
            this.rtTesto.Versione = null;
            // 
            // printDocument
            // 
            this.printDocument.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDoc_PrintPage);
            this.printDocument.EndPrint += new System.Drawing.Printing.PrintEventHandler(this.printDoc_EndPrint);
            this.printDocument.BeginPrint += new System.Drawing.Printing.PrintEventHandler(this.printDoc_BeginPrint);
            // 
            // Principale
            // 
            this.AcceptButton = this.pulEsegui;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.CancelButton = this.pulChiudi;
            this.Controls.Add(this.rtTesto);
            this.Controls.Add(this.panControlli);
            this.Font = null;
            this.fileGuida.SetHelpKeyword(this, null);
            this.fileGuida.SetHelpNavigator(this, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("$this.HelpNavigator"))));
            this.fileGuida.SetHelpString(this, null);
            this.Name = "Principale";
            this.fileGuida.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Shown += new System.EventHandler(this.Principale_Shown);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Principale_FormClosing);
            this.Resize += new System.EventHandler(this.Principale_Resize);
            this.panControlli.ResumeLayout(false);
            this.panControlli.PerformLayout();
            this.msRtf.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.HelpProvider fileGuida;
        private System.Windows.Forms.Panel panControlli;
        private System.Windows.Forms.Button pulChiudi;
        private System.Windows.Forms.Button pulGuida;
        private System.Windows.Forms.Button pulEsegui;
        private System.Windows.Forms.ComboBox cbVersione;
        private System.Windows.Forms.ComboBox cbEspressione;
        private TestiBiblici.RichTextBoxEx rtTesto;
        private System.Windows.Forms.Label etiEspressione;
        private System.Windows.Forms.Label etiVersione;
        private System.Windows.Forms.ContextMenuStrip msRtf;
        private System.Windows.Forms.ToolStripMenuItem msiCopiaTutto;
        private System.Windows.Forms.ToolStripMenuItem msiStampaTutto;
        private System.Windows.Forms.ToolStripMenuItem msiSelezionaTutto;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem msiCopiaSelezione;
        private System.Windows.Forms.ToolStripMenuItem msiStampaSeleziona;
        private System.Drawing.Printing.PrintDocument printDocument;
    }
}

