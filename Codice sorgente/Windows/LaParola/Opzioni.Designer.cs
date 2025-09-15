using TestiBiblici;
namespace LaParola
{
    partial class Opzioni
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
                font.Dispose();
                fontRicerca.Dispose();
                fontRiferimento.Dispose();
                fontGreco.Dispose();
                fontEbraico.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Opzioni));
            this.tvCategorie = new System.Windows.Forms.TreeView();
            this.panInterfaccia = new System.Windows.Forms.Panel();
            this.cbIpertestoTooltipInTooltip = new System.Windows.Forms.CheckBox();
            this.udVociMassimeMenu = new System.Windows.Forms.NumericUpDown();
            this.labVociMassimeMenu = new System.Windows.Forms.Label();
            this.labBarraDiStrumenti = new System.Windows.Forms.Label();
            this.cbBSRigaComando = new System.Windows.Forms.CheckBox();
            this.cbBSOrdine = new System.Windows.Forms.CheckBox();
            this.cbBSFormato = new System.Windows.Forms.CheckBox();
            this.cbBSPrincipale = new System.Windows.Forms.CheckBox();
            this.cbIpertestoTooltip = new System.Windows.Forms.CheckBox();
            this.cbStessaFinestraPerRisultati = new System.Windows.Forms.CheckBox();
            this.cbBarraDiStato = new System.Windows.Forms.CheckBox();
            this.labRichiesteMemorizzate = new System.Windows.Forms.Label();
            this.udRichiesteMemorizzate = new System.Windows.Forms.NumericUpDown();
            this.labLingua = new System.Windows.Forms.Label();
            this.cbLingua = new System.Windows.Forms.ComboBox();
            this.panCaratteri = new System.Windows.Forms.Panel();
            this.btnFontGreco = new System.Windows.Forms.Button();
            this.labFontEbraicoTesto = new System.Windows.Forms.Label();
            this.labFontGrecoTesto = new System.Windows.Forms.Label();
            this.labFontEbraico = new System.Windows.Forms.Label();
            this.labFontGreco = new System.Windows.Forms.Label();
            this.labFontRicerca = new System.Windows.Forms.Label();
            this.btnFontRicerca = new System.Windows.Forms.Button();
            this.labFontRicercaTesto = new System.Windows.Forms.Label();
            this.cbRifApice = new System.Windows.Forms.CheckBox();
            this.btnFontRif = new System.Windows.Forms.Button();
            this.labFontRiferimento = new System.Windows.Forms.Label();
            this.btnFont = new System.Windows.Forms.Button();
            this.labFontPredefTesto = new System.Windows.Forms.Label();
            this.labFontPredef = new System.Windows.Forms.Label();
            this.labFontRifTesto = new System.Windows.Forms.Label();
            this.btnFontEbraico = new System.Windows.Forms.Button();
            this.panRisultati = new System.Windows.Forms.Panel();
            this.rtEsempio = new TestiBiblici.RichTextBoxEx();
            this.labEsempio = new System.Windows.Forms.Label();
            this.panTesto = new System.Windows.Forms.Panel();
            this.cbTitoli = new System.Windows.Forms.CheckBox();
            this.gbTesto = new System.Windows.Forms.GroupBox();
            this.rbTesto2 = new System.Windows.Forms.RadioButton();
            this.rbTesto1 = new System.Windows.Forms.RadioButton();
            this.rbTesto0 = new System.Windows.Forms.RadioButton();
            this.panRiferimenti = new System.Windows.Forms.Panel();
            this.cbRifContestoRicerche = new System.Windows.Forms.CheckBox();
            this.gbPosto = new System.Windows.Forms.GroupBox();
            this.rbRifPosto2 = new System.Windows.Forms.RadioButton();
            this.rbRifPosto1 = new System.Windows.Forms.RadioButton();
            this.rbRifPosto0 = new System.Windows.Forms.RadioButton();
            this.gbFormato = new System.Windows.Forms.GroupBox();
            this.rbRifFormato2 = new System.Windows.Forms.RadioButton();
            this.rbRifFormato1 = new System.Windows.Forms.RadioButton();
            this.rbRifFormato0 = new System.Windows.Forms.RadioButton();
            this.gbRifTipo = new System.Windows.Forms.GroupBox();
            this.rbRifTipo2 = new System.Windows.Forms.RadioButton();
            this.rbRifTipo1 = new System.Windows.Forms.RadioButton();
            this.rbRifTipo0 = new System.Windows.Forms.RadioButton();
            this.panLibri = new System.Windows.Forms.Panel();
            this.btnLibriSpagnolo = new System.Windows.Forms.Button();
            this.gridLibri = new System.Windows.Forms.DataGridView();
            this.btnLibriItaliano = new System.Windows.Forms.Button();
            this.btnLibriInglese = new System.Windows.Forms.Button();
            this.panTesti = new System.Windows.Forms.Panel();
            this.pulCancellaCartella = new System.Windows.Forms.Button();
            this.pulAggiungiCartella = new System.Windows.Forms.Button();
            this.lbCartelle = new System.Windows.Forms.ListBox();
            this.labCartelle = new System.Windows.Forms.Label();
            this.clbCommentari = new System.Windows.Forms.CheckedListBox();
            this.labCommentari = new System.Windows.Forms.Label();
            this.panAggiornamenti = new System.Windows.Forms.Panel();
            this.tbProxyDominio = new System.Windows.Forms.TextBox();
            this.etiProxyDominio = new System.Windows.Forms.Label();
            this.tbProxyPassword = new System.Windows.Forms.TextBox();
            this.etiProxyPassword = new System.Windows.Forms.Label();
            this.tbProxyNomeUtente = new System.Windows.Forms.TextBox();
            this.etiProxyNomeUtente = new System.Windows.Forms.Label();
            this.tbProxyPorta = new System.Windows.Forms.TextBox();
            this.etiProxyPort = new System.Windows.Forms.Label();
            this.tbProxy = new System.Windows.Forms.TextBox();
            this.etiProxy = new System.Windows.Forms.Label();
            this.cbAggiornaGiorni = new System.Windows.Forms.ComboBox();
            this.rbAggiornaAutomatica = new System.Windows.Forms.RadioButton();
            this.rbAggiornaManuale = new System.Windows.Forms.RadioButton();
            this.panDizionari = new System.Windows.Forms.Panel();
            this.labDizionarioLatino = new System.Windows.Forms.Label();
            this.cbDizionariLatini = new System.Windows.Forms.ComboBox();
            this.cbDizionariEbraici = new System.Windows.Forms.ComboBox();
            this.labDizionarioEbraico = new System.Windows.Forms.Label();
            this.labDizionarioGreco = new System.Windows.Forms.Label();
            this.labDizionarioItaliano = new System.Windows.Forms.Label();
            this.labDizionarioInglese = new System.Windows.Forms.Label();
            this.cbDizionarioTooltip = new System.Windows.Forms.CheckBox();
            this.cbDizionariGreci = new System.Windows.Forms.ComboBox();
            this.cbDizionariItaliani = new System.Windows.Forms.ComboBox();
            this.cbDizionariInglesi = new System.Windows.Forms.ComboBox();
            this.panAltre = new System.Windows.Forms.Panel();
            this.pulReload = new System.Windows.Forms.Button();
            this.pulReset = new System.Windows.Forms.Button();
            this.cbDisposizioni = new System.Windows.Forms.ComboBox();
            this.etiDisposizione = new System.Windows.Forms.Label();
            this.cbLetture = new System.Windows.Forms.CheckBox();
            this.panClipboard = new System.Windows.Forms.Panel();
            this.labClipboardLunghezza2 = new System.Windows.Forms.Label();
            this.tbClipboardLunghezza = new System.Windows.Forms.TextBox();
            this.labClipboardLunghezza1 = new System.Windows.Forms.Label();
            this.labClipboardTempo2 = new System.Windows.Forms.Label();
            this.tbClipboardTempo = new System.Windows.Forms.TextBox();
            this.labClipboardTempo1 = new System.Windows.Forms.Label();
            this.cbClipboardAttivo = new System.Windows.Forms.CheckBox();
            this.Nome = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AbbUsate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AbbRicono = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panInterfaccia.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udVociMassimeMenu)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udRichiesteMemorizzate)).BeginInit();
            this.panCaratteri.SuspendLayout();
            this.panRisultati.SuspendLayout();
            this.panTesto.SuspendLayout();
            this.gbTesto.SuspendLayout();
            this.panRiferimenti.SuspendLayout();
            this.gbPosto.SuspendLayout();
            this.gbFormato.SuspendLayout();
            this.gbRifTipo.SuspendLayout();
            this.panLibri.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridLibri)).BeginInit();
            this.panTesti.SuspendLayout();
            this.panAggiornamenti.SuspendLayout();
            this.panDizionari.SuspendLayout();
            this.panAltre.SuspendLayout();
            this.panClipboard.SuspendLayout();
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
            // guidaFile
            // 
            this.guidaFile.HelpNamespace = null;
            // 
            // tvCategorie
            // 
            this.tvCategorie.AccessibleDescription = null;
            this.tvCategorie.AccessibleName = null;
            resources.ApplyResources(this.tvCategorie, "tvCategorie");
            this.tvCategorie.BackgroundImage = null;
            this.tvCategorie.Font = null;
            this.guidaFile.SetHelpKeyword(this.tvCategorie, resources.GetString("tvCategorie.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.tvCategorie, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("tvCategorie.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.tvCategorie, null);
            this.tvCategorie.HotTracking = true;
            this.tvCategorie.Name = "tvCategorie";
            this.tvCategorie.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            ((System.Windows.Forms.TreeNode)(resources.GetObject("tvCategorie.Nodes"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("tvCategorie.Nodes1"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("tvCategorie.Nodes2"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("tvCategorie.Nodes3"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("tvCategorie.Nodes4"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("tvCategorie.Nodes5"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("tvCategorie.Nodes6")))});
            this.guidaFile.SetShowHelp(this.tvCategorie, ((bool)(resources.GetObject("tvCategorie.ShowHelp"))));
            this.tvCategorie.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvCategorie_AfterSelect);
            // 
            // panInterfaccia
            // 
            this.panInterfaccia.AccessibleDescription = null;
            this.panInterfaccia.AccessibleName = null;
            resources.ApplyResources(this.panInterfaccia, "panInterfaccia");
            this.panInterfaccia.BackgroundImage = null;
            this.panInterfaccia.Controls.Add(this.cbIpertestoTooltipInTooltip);
            this.panInterfaccia.Controls.Add(this.udVociMassimeMenu);
            this.panInterfaccia.Controls.Add(this.labVociMassimeMenu);
            this.panInterfaccia.Controls.Add(this.labBarraDiStrumenti);
            this.panInterfaccia.Controls.Add(this.cbBSRigaComando);
            this.panInterfaccia.Controls.Add(this.cbBSOrdine);
            this.panInterfaccia.Controls.Add(this.cbBSFormato);
            this.panInterfaccia.Controls.Add(this.cbBSPrincipale);
            this.panInterfaccia.Controls.Add(this.cbIpertestoTooltip);
            this.panInterfaccia.Controls.Add(this.cbStessaFinestraPerRisultati);
            this.panInterfaccia.Controls.Add(this.cbBarraDiStato);
            this.panInterfaccia.Controls.Add(this.labRichiesteMemorizzate);
            this.panInterfaccia.Controls.Add(this.udRichiesteMemorizzate);
            this.panInterfaccia.Controls.Add(this.labLingua);
            this.panInterfaccia.Controls.Add(this.cbLingua);
            this.panInterfaccia.Font = null;
            this.guidaFile.SetHelpKeyword(this.panInterfaccia, null);
            this.guidaFile.SetHelpNavigator(this.panInterfaccia, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("panInterfaccia.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.panInterfaccia, null);
            this.panInterfaccia.Name = "panInterfaccia";
            this.guidaFile.SetShowHelp(this.panInterfaccia, ((bool)(resources.GetObject("panInterfaccia.ShowHelp"))));
            this.panInterfaccia.Tag = "panInterfaccia";
            // 
            // cbIpertestoTooltipInTooltip
            // 
            this.cbIpertestoTooltipInTooltip.AccessibleDescription = null;
            this.cbIpertestoTooltipInTooltip.AccessibleName = null;
            resources.ApplyResources(this.cbIpertestoTooltipInTooltip, "cbIpertestoTooltipInTooltip");
            this.cbIpertestoTooltipInTooltip.BackgroundImage = null;
            this.cbIpertestoTooltipInTooltip.Font = null;
            this.guidaFile.SetHelpKeyword(this.cbIpertestoTooltipInTooltip, resources.GetString("cbIpertestoTooltipInTooltip.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbIpertestoTooltipInTooltip, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbIpertestoTooltipInTooltip.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbIpertestoTooltipInTooltip, null);
            this.cbIpertestoTooltipInTooltip.Name = "cbIpertestoTooltipInTooltip";
            this.guidaFile.SetShowHelp(this.cbIpertestoTooltipInTooltip, ((bool)(resources.GetObject("cbIpertestoTooltipInTooltip.ShowHelp"))));
            this.cbIpertestoTooltipInTooltip.UseVisualStyleBackColor = true;
            // 
            // udVociMassimeMenu
            // 
            this.udVociMassimeMenu.AccessibleDescription = null;
            this.udVociMassimeMenu.AccessibleName = null;
            resources.ApplyResources(this.udVociMassimeMenu, "udVociMassimeMenu");
            this.udVociMassimeMenu.Font = null;
            this.guidaFile.SetHelpKeyword(this.udVociMassimeMenu, resources.GetString("udVociMassimeMenu.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.udVociMassimeMenu, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("udVociMassimeMenu.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.udVociMassimeMenu, null);
            this.udVociMassimeMenu.Name = "udVociMassimeMenu";
            this.guidaFile.SetShowHelp(this.udVociMassimeMenu, ((bool)(resources.GetObject("udVociMassimeMenu.ShowHelp"))));
            // 
            // labVociMassimeMenu
            // 
            this.labVociMassimeMenu.AccessibleDescription = null;
            this.labVociMassimeMenu.AccessibleName = null;
            resources.ApplyResources(this.labVociMassimeMenu, "labVociMassimeMenu");
            this.labVociMassimeMenu.Font = null;
            this.guidaFile.SetHelpKeyword(this.labVociMassimeMenu, resources.GetString("labVociMassimeMenu.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.labVociMassimeMenu, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("labVociMassimeMenu.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.labVociMassimeMenu, null);
            this.labVociMassimeMenu.Name = "labVociMassimeMenu";
            this.guidaFile.SetShowHelp(this.labVociMassimeMenu, ((bool)(resources.GetObject("labVociMassimeMenu.ShowHelp"))));
            // 
            // labBarraDiStrumenti
            // 
            this.labBarraDiStrumenti.AccessibleDescription = null;
            this.labBarraDiStrumenti.AccessibleName = null;
            resources.ApplyResources(this.labBarraDiStrumenti, "labBarraDiStrumenti");
            this.labBarraDiStrumenti.Font = null;
            this.guidaFile.SetHelpKeyword(this.labBarraDiStrumenti, resources.GetString("labBarraDiStrumenti.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.labBarraDiStrumenti, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("labBarraDiStrumenti.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.labBarraDiStrumenti, null);
            this.labBarraDiStrumenti.Name = "labBarraDiStrumenti";
            this.guidaFile.SetShowHelp(this.labBarraDiStrumenti, ((bool)(resources.GetObject("labBarraDiStrumenti.ShowHelp"))));
            // 
            // cbBSRigaComando
            // 
            this.cbBSRigaComando.AccessibleDescription = null;
            this.cbBSRigaComando.AccessibleName = null;
            resources.ApplyResources(this.cbBSRigaComando, "cbBSRigaComando");
            this.cbBSRigaComando.BackgroundImage = null;
            this.cbBSRigaComando.Font = null;
            this.guidaFile.SetHelpKeyword(this.cbBSRigaComando, resources.GetString("cbBSRigaComando.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbBSRigaComando, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbBSRigaComando.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbBSRigaComando, null);
            this.cbBSRigaComando.Name = "cbBSRigaComando";
            this.guidaFile.SetShowHelp(this.cbBSRigaComando, ((bool)(resources.GetObject("cbBSRigaComando.ShowHelp"))));
            this.cbBSRigaComando.UseVisualStyleBackColor = true;
            // 
            // cbBSOrdine
            // 
            this.cbBSOrdine.AccessibleDescription = null;
            this.cbBSOrdine.AccessibleName = null;
            resources.ApplyResources(this.cbBSOrdine, "cbBSOrdine");
            this.cbBSOrdine.BackgroundImage = null;
            this.cbBSOrdine.Font = null;
            this.guidaFile.SetHelpKeyword(this.cbBSOrdine, resources.GetString("cbBSOrdine.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbBSOrdine, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbBSOrdine.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbBSOrdine, null);
            this.cbBSOrdine.Name = "cbBSOrdine";
            this.guidaFile.SetShowHelp(this.cbBSOrdine, ((bool)(resources.GetObject("cbBSOrdine.ShowHelp"))));
            this.cbBSOrdine.UseVisualStyleBackColor = true;
            // 
            // cbBSFormato
            // 
            this.cbBSFormato.AccessibleDescription = null;
            this.cbBSFormato.AccessibleName = null;
            resources.ApplyResources(this.cbBSFormato, "cbBSFormato");
            this.cbBSFormato.BackgroundImage = null;
            this.cbBSFormato.Font = null;
            this.guidaFile.SetHelpKeyword(this.cbBSFormato, resources.GetString("cbBSFormato.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbBSFormato, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbBSFormato.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbBSFormato, null);
            this.cbBSFormato.Name = "cbBSFormato";
            this.guidaFile.SetShowHelp(this.cbBSFormato, ((bool)(resources.GetObject("cbBSFormato.ShowHelp"))));
            this.cbBSFormato.UseVisualStyleBackColor = true;
            // 
            // cbBSPrincipale
            // 
            this.cbBSPrincipale.AccessibleDescription = null;
            this.cbBSPrincipale.AccessibleName = null;
            resources.ApplyResources(this.cbBSPrincipale, "cbBSPrincipale");
            this.cbBSPrincipale.BackgroundImage = null;
            this.cbBSPrincipale.Font = null;
            this.guidaFile.SetHelpKeyword(this.cbBSPrincipale, resources.GetString("cbBSPrincipale.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbBSPrincipale, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbBSPrincipale.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbBSPrincipale, null);
            this.cbBSPrincipale.Name = "cbBSPrincipale";
            this.guidaFile.SetShowHelp(this.cbBSPrincipale, ((bool)(resources.GetObject("cbBSPrincipale.ShowHelp"))));
            this.cbBSPrincipale.UseVisualStyleBackColor = true;
            // 
            // cbIpertestoTooltip
            // 
            this.cbIpertestoTooltip.AccessibleDescription = null;
            this.cbIpertestoTooltip.AccessibleName = null;
            resources.ApplyResources(this.cbIpertestoTooltip, "cbIpertestoTooltip");
            this.cbIpertestoTooltip.BackgroundImage = null;
            this.cbIpertestoTooltip.Font = null;
            this.guidaFile.SetHelpKeyword(this.cbIpertestoTooltip, resources.GetString("cbIpertestoTooltip.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbIpertestoTooltip, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbIpertestoTooltip.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbIpertestoTooltip, null);
            this.cbIpertestoTooltip.Name = "cbIpertestoTooltip";
            this.guidaFile.SetShowHelp(this.cbIpertestoTooltip, ((bool)(resources.GetObject("cbIpertestoTooltip.ShowHelp"))));
            this.cbIpertestoTooltip.UseVisualStyleBackColor = true;
            // 
            // cbStessaFinestraPerRisultati
            // 
            this.cbStessaFinestraPerRisultati.AccessibleDescription = null;
            this.cbStessaFinestraPerRisultati.AccessibleName = null;
            resources.ApplyResources(this.cbStessaFinestraPerRisultati, "cbStessaFinestraPerRisultati");
            this.cbStessaFinestraPerRisultati.BackgroundImage = null;
            this.cbStessaFinestraPerRisultati.Font = null;
            this.guidaFile.SetHelpKeyword(this.cbStessaFinestraPerRisultati, resources.GetString("cbStessaFinestraPerRisultati.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbStessaFinestraPerRisultati, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbStessaFinestraPerRisultati.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbStessaFinestraPerRisultati, null);
            this.cbStessaFinestraPerRisultati.Name = "cbStessaFinestraPerRisultati";
            this.guidaFile.SetShowHelp(this.cbStessaFinestraPerRisultati, ((bool)(resources.GetObject("cbStessaFinestraPerRisultati.ShowHelp"))));
            this.cbStessaFinestraPerRisultati.UseVisualStyleBackColor = true;
            // 
            // cbBarraDiStato
            // 
            this.cbBarraDiStato.AccessibleDescription = null;
            this.cbBarraDiStato.AccessibleName = null;
            resources.ApplyResources(this.cbBarraDiStato, "cbBarraDiStato");
            this.cbBarraDiStato.BackgroundImage = null;
            this.cbBarraDiStato.Font = null;
            this.guidaFile.SetHelpKeyword(this.cbBarraDiStato, resources.GetString("cbBarraDiStato.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbBarraDiStato, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbBarraDiStato.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbBarraDiStato, null);
            this.cbBarraDiStato.Name = "cbBarraDiStato";
            this.guidaFile.SetShowHelp(this.cbBarraDiStato, ((bool)(resources.GetObject("cbBarraDiStato.ShowHelp"))));
            this.cbBarraDiStato.UseVisualStyleBackColor = true;
            // 
            // labRichiesteMemorizzate
            // 
            this.labRichiesteMemorizzate.AccessibleDescription = null;
            this.labRichiesteMemorizzate.AccessibleName = null;
            resources.ApplyResources(this.labRichiesteMemorizzate, "labRichiesteMemorizzate");
            this.labRichiesteMemorizzate.Font = null;
            this.guidaFile.SetHelpKeyword(this.labRichiesteMemorizzate, resources.GetString("labRichiesteMemorizzate.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.labRichiesteMemorizzate, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("labRichiesteMemorizzate.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.labRichiesteMemorizzate, null);
            this.labRichiesteMemorizzate.Name = "labRichiesteMemorizzate";
            this.guidaFile.SetShowHelp(this.labRichiesteMemorizzate, ((bool)(resources.GetObject("labRichiesteMemorizzate.ShowHelp"))));
            // 
            // udRichiesteMemorizzate
            // 
            this.udRichiesteMemorizzate.AccessibleDescription = null;
            this.udRichiesteMemorizzate.AccessibleName = null;
            resources.ApplyResources(this.udRichiesteMemorizzate, "udRichiesteMemorizzate");
            this.udRichiesteMemorizzate.Font = null;
            this.guidaFile.SetHelpKeyword(this.udRichiesteMemorizzate, resources.GetString("udRichiesteMemorizzate.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.udRichiesteMemorizzate, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("udRichiesteMemorizzate.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.udRichiesteMemorizzate, null);
            this.udRichiesteMemorizzate.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            0});
            this.udRichiesteMemorizzate.Name = "udRichiesteMemorizzate";
            this.guidaFile.SetShowHelp(this.udRichiesteMemorizzate, ((bool)(resources.GetObject("udRichiesteMemorizzate.ShowHelp"))));
            // 
            // labLingua
            // 
            this.labLingua.AccessibleDescription = null;
            this.labLingua.AccessibleName = null;
            resources.ApplyResources(this.labLingua, "labLingua");
            this.labLingua.Font = null;
            this.guidaFile.SetHelpKeyword(this.labLingua, resources.GetString("labLingua.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.labLingua, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("labLingua.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.labLingua, null);
            this.labLingua.Name = "labLingua";
            this.guidaFile.SetShowHelp(this.labLingua, ((bool)(resources.GetObject("labLingua.ShowHelp"))));
            // 
            // cbLingua
            // 
            this.cbLingua.AccessibleDescription = null;
            this.cbLingua.AccessibleName = null;
            resources.ApplyResources(this.cbLingua, "cbLingua");
            this.cbLingua.BackgroundImage = null;
            this.cbLingua.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLingua.Font = null;
            this.cbLingua.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.cbLingua, resources.GetString("cbLingua.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbLingua, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbLingua.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbLingua, null);
            this.cbLingua.Items.AddRange(new object[] {
            resources.GetString("cbLingua.Items"),
            resources.GetString("cbLingua.Items1"),
            resources.GetString("cbLingua.Items2"),
            resources.GetString("cbLingua.Items3")});
            this.cbLingua.Name = "cbLingua";
            this.guidaFile.SetShowHelp(this.cbLingua, ((bool)(resources.GetObject("cbLingua.ShowHelp"))));
            // 
            // panCaratteri
            // 
            this.panCaratteri.AccessibleDescription = null;
            this.panCaratteri.AccessibleName = null;
            resources.ApplyResources(this.panCaratteri, "panCaratteri");
            this.panCaratteri.BackgroundImage = null;
            this.panCaratteri.Controls.Add(this.btnFontGreco);
            this.panCaratteri.Controls.Add(this.labFontEbraicoTesto);
            this.panCaratteri.Controls.Add(this.labFontGrecoTesto);
            this.panCaratteri.Controls.Add(this.labFontEbraico);
            this.panCaratteri.Controls.Add(this.labFontGreco);
            this.panCaratteri.Controls.Add(this.labFontRicerca);
            this.panCaratteri.Controls.Add(this.btnFontRicerca);
            this.panCaratteri.Controls.Add(this.labFontRicercaTesto);
            this.panCaratteri.Controls.Add(this.cbRifApice);
            this.panCaratteri.Controls.Add(this.btnFontRif);
            this.panCaratteri.Controls.Add(this.labFontRiferimento);
            this.panCaratteri.Controls.Add(this.btnFont);
            this.panCaratteri.Controls.Add(this.labFontPredefTesto);
            this.panCaratteri.Controls.Add(this.labFontPredef);
            this.panCaratteri.Controls.Add(this.labFontRifTesto);
            this.panCaratteri.Controls.Add(this.btnFontEbraico);
            this.panCaratteri.Font = null;
            this.guidaFile.SetHelpKeyword(this.panCaratteri, null);
            this.guidaFile.SetHelpNavigator(this.panCaratteri, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("panCaratteri.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.panCaratteri, null);
            this.panCaratteri.Name = "panCaratteri";
            this.guidaFile.SetShowHelp(this.panCaratteri, ((bool)(resources.GetObject("panCaratteri.ShowHelp"))));
            this.panCaratteri.Tag = "panCaratteri";
            // 
            // btnFontGreco
            // 
            this.btnFontGreco.AccessibleDescription = null;
            this.btnFontGreco.AccessibleName = null;
            resources.ApplyResources(this.btnFontGreco, "btnFontGreco");
            this.btnFontGreco.BackgroundImage = null;
            this.btnFontGreco.Font = null;
            this.guidaFile.SetHelpKeyword(this.btnFontGreco, resources.GetString("btnFontGreco.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.btnFontGreco, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("btnFontGreco.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.btnFontGreco, null);
            this.btnFontGreco.Image = global::LaParola.Properties.Resources.fontstile;
            this.btnFontGreco.Name = "btnFontGreco";
            this.guidaFile.SetShowHelp(this.btnFontGreco, ((bool)(resources.GetObject("btnFontGreco.ShowHelp"))));
            this.btnFontGreco.UseVisualStyleBackColor = true;
            this.btnFontGreco.Click += new System.EventHandler(this.btnFontGreco_Click);
            // 
            // labFontEbraicoTesto
            // 
            this.labFontEbraicoTesto.AccessibleDescription = null;
            this.labFontEbraicoTesto.AccessibleName = null;
            resources.ApplyResources(this.labFontEbraicoTesto, "labFontEbraicoTesto");
            this.labFontEbraicoTesto.Font = null;
            this.guidaFile.SetHelpKeyword(this.labFontEbraicoTesto, resources.GetString("labFontEbraicoTesto.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.labFontEbraicoTesto, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("labFontEbraicoTesto.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.labFontEbraicoTesto, null);
            this.labFontEbraicoTesto.Name = "labFontEbraicoTesto";
            this.guidaFile.SetShowHelp(this.labFontEbraicoTesto, ((bool)(resources.GetObject("labFontEbraicoTesto.ShowHelp"))));
            // 
            // labFontGrecoTesto
            // 
            this.labFontGrecoTesto.AccessibleDescription = null;
            this.labFontGrecoTesto.AccessibleName = null;
            resources.ApplyResources(this.labFontGrecoTesto, "labFontGrecoTesto");
            this.labFontGrecoTesto.Font = null;
            this.guidaFile.SetHelpKeyword(this.labFontGrecoTesto, resources.GetString("labFontGrecoTesto.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.labFontGrecoTesto, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("labFontGrecoTesto.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.labFontGrecoTesto, null);
            this.labFontGrecoTesto.Name = "labFontGrecoTesto";
            this.guidaFile.SetShowHelp(this.labFontGrecoTesto, ((bool)(resources.GetObject("labFontGrecoTesto.ShowHelp"))));
            // 
            // labFontEbraico
            // 
            this.labFontEbraico.AccessibleDescription = null;
            this.labFontEbraico.AccessibleName = null;
            resources.ApplyResources(this.labFontEbraico, "labFontEbraico");
            this.labFontEbraico.Font = null;
            this.guidaFile.SetHelpKeyword(this.labFontEbraico, resources.GetString("labFontEbraico.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.labFontEbraico, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("labFontEbraico.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.labFontEbraico, null);
            this.labFontEbraico.Name = "labFontEbraico";
            this.guidaFile.SetShowHelp(this.labFontEbraico, ((bool)(resources.GetObject("labFontEbraico.ShowHelp"))));
            // 
            // labFontGreco
            // 
            this.labFontGreco.AccessibleDescription = null;
            this.labFontGreco.AccessibleName = null;
            resources.ApplyResources(this.labFontGreco, "labFontGreco");
            this.labFontGreco.Font = null;
            this.guidaFile.SetHelpKeyword(this.labFontGreco, resources.GetString("labFontGreco.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.labFontGreco, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("labFontGreco.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.labFontGreco, null);
            this.labFontGreco.Name = "labFontGreco";
            this.guidaFile.SetShowHelp(this.labFontGreco, ((bool)(resources.GetObject("labFontGreco.ShowHelp"))));
            // 
            // labFontRicerca
            // 
            this.labFontRicerca.AccessibleDescription = null;
            this.labFontRicerca.AccessibleName = null;
            resources.ApplyResources(this.labFontRicerca, "labFontRicerca");
            this.labFontRicerca.Font = null;
            this.guidaFile.SetHelpKeyword(this.labFontRicerca, resources.GetString("labFontRicerca.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.labFontRicerca, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("labFontRicerca.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.labFontRicerca, null);
            this.labFontRicerca.Name = "labFontRicerca";
            this.guidaFile.SetShowHelp(this.labFontRicerca, ((bool)(resources.GetObject("labFontRicerca.ShowHelp"))));
            // 
            // btnFontRicerca
            // 
            this.btnFontRicerca.AccessibleDescription = null;
            this.btnFontRicerca.AccessibleName = null;
            resources.ApplyResources(this.btnFontRicerca, "btnFontRicerca");
            this.btnFontRicerca.BackgroundImage = null;
            this.btnFontRicerca.Font = null;
            this.guidaFile.SetHelpKeyword(this.btnFontRicerca, resources.GetString("btnFontRicerca.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.btnFontRicerca, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("btnFontRicerca.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.btnFontRicerca, null);
            this.btnFontRicerca.Image = global::LaParola.Properties.Resources.fontstile;
            this.btnFontRicerca.Name = "btnFontRicerca";
            this.guidaFile.SetShowHelp(this.btnFontRicerca, ((bool)(resources.GetObject("btnFontRicerca.ShowHelp"))));
            this.btnFontRicerca.UseVisualStyleBackColor = true;
            this.btnFontRicerca.Click += new System.EventHandler(this.btnFontRicerca_Click);
            // 
            // labFontRicercaTesto
            // 
            this.labFontRicercaTesto.AccessibleDescription = null;
            this.labFontRicercaTesto.AccessibleName = null;
            resources.ApplyResources(this.labFontRicercaTesto, "labFontRicercaTesto");
            this.labFontRicercaTesto.Font = null;
            this.guidaFile.SetHelpKeyword(this.labFontRicercaTesto, resources.GetString("labFontRicercaTesto.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.labFontRicercaTesto, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("labFontRicercaTesto.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.labFontRicercaTesto, null);
            this.labFontRicercaTesto.Name = "labFontRicercaTesto";
            this.guidaFile.SetShowHelp(this.labFontRicercaTesto, ((bool)(resources.GetObject("labFontRicercaTesto.ShowHelp"))));
            // 
            // cbRifApice
            // 
            this.cbRifApice.AccessibleDescription = null;
            this.cbRifApice.AccessibleName = null;
            resources.ApplyResources(this.cbRifApice, "cbRifApice");
            this.cbRifApice.BackgroundImage = null;
            this.cbRifApice.Font = null;
            this.guidaFile.SetHelpKeyword(this.cbRifApice, resources.GetString("cbRifApice.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbRifApice, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbRifApice.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbRifApice, null);
            this.cbRifApice.Name = "cbRifApice";
            this.guidaFile.SetShowHelp(this.cbRifApice, ((bool)(resources.GetObject("cbRifApice.ShowHelp"))));
            this.cbRifApice.UseVisualStyleBackColor = true;
            // 
            // btnFontRif
            // 
            this.btnFontRif.AccessibleDescription = null;
            this.btnFontRif.AccessibleName = null;
            resources.ApplyResources(this.btnFontRif, "btnFontRif");
            this.btnFontRif.BackgroundImage = null;
            this.btnFontRif.Font = null;
            this.guidaFile.SetHelpKeyword(this.btnFontRif, resources.GetString("btnFontRif.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.btnFontRif, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("btnFontRif.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.btnFontRif, null);
            this.btnFontRif.Image = global::LaParola.Properties.Resources.fontstile;
            this.btnFontRif.Name = "btnFontRif";
            this.guidaFile.SetShowHelp(this.btnFontRif, ((bool)(resources.GetObject("btnFontRif.ShowHelp"))));
            this.btnFontRif.UseVisualStyleBackColor = true;
            this.btnFontRif.Click += new System.EventHandler(this.btnFontRif_Click);
            // 
            // labFontRiferimento
            // 
            this.labFontRiferimento.AccessibleDescription = null;
            this.labFontRiferimento.AccessibleName = null;
            resources.ApplyResources(this.labFontRiferimento, "labFontRiferimento");
            this.labFontRiferimento.Font = null;
            this.guidaFile.SetHelpKeyword(this.labFontRiferimento, resources.GetString("labFontRiferimento.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.labFontRiferimento, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("labFontRiferimento.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.labFontRiferimento, null);
            this.labFontRiferimento.Name = "labFontRiferimento";
            this.guidaFile.SetShowHelp(this.labFontRiferimento, ((bool)(resources.GetObject("labFontRiferimento.ShowHelp"))));
            // 
            // btnFont
            // 
            this.btnFont.AccessibleDescription = null;
            this.btnFont.AccessibleName = null;
            resources.ApplyResources(this.btnFont, "btnFont");
            this.btnFont.BackgroundImage = null;
            this.btnFont.Font = null;
            this.guidaFile.SetHelpKeyword(this.btnFont, resources.GetString("btnFont.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.btnFont, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("btnFont.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.btnFont, null);
            this.btnFont.Image = global::LaParola.Properties.Resources.fontstile;
            this.btnFont.Name = "btnFont";
            this.guidaFile.SetShowHelp(this.btnFont, ((bool)(resources.GetObject("btnFont.ShowHelp"))));
            this.btnFont.UseVisualStyleBackColor = true;
            this.btnFont.Click += new System.EventHandler(this.btnFont_Click);
            // 
            // labFontPredefTesto
            // 
            this.labFontPredefTesto.AccessibleDescription = null;
            this.labFontPredefTesto.AccessibleName = null;
            resources.ApplyResources(this.labFontPredefTesto, "labFontPredefTesto");
            this.labFontPredefTesto.Font = null;
            this.guidaFile.SetHelpKeyword(this.labFontPredefTesto, resources.GetString("labFontPredefTesto.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.labFontPredefTesto, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("labFontPredefTesto.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.labFontPredefTesto, null);
            this.labFontPredefTesto.Name = "labFontPredefTesto";
            this.guidaFile.SetShowHelp(this.labFontPredefTesto, ((bool)(resources.GetObject("labFontPredefTesto.ShowHelp"))));
            // 
            // labFontPredef
            // 
            this.labFontPredef.AccessibleDescription = null;
            this.labFontPredef.AccessibleName = null;
            resources.ApplyResources(this.labFontPredef, "labFontPredef");
            this.labFontPredef.Font = null;
            this.guidaFile.SetHelpKeyword(this.labFontPredef, resources.GetString("labFontPredef.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.labFontPredef, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("labFontPredef.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.labFontPredef, null);
            this.labFontPredef.Name = "labFontPredef";
            this.guidaFile.SetShowHelp(this.labFontPredef, ((bool)(resources.GetObject("labFontPredef.ShowHelp"))));
            // 
            // labFontRifTesto
            // 
            this.labFontRifTesto.AccessibleDescription = null;
            this.labFontRifTesto.AccessibleName = null;
            resources.ApplyResources(this.labFontRifTesto, "labFontRifTesto");
            this.labFontRifTesto.Font = null;
            this.guidaFile.SetHelpKeyword(this.labFontRifTesto, resources.GetString("labFontRifTesto.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.labFontRifTesto, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("labFontRifTesto.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.labFontRifTesto, null);
            this.labFontRifTesto.Name = "labFontRifTesto";
            this.guidaFile.SetShowHelp(this.labFontRifTesto, ((bool)(resources.GetObject("labFontRifTesto.ShowHelp"))));
            // 
            // btnFontEbraico
            // 
            this.btnFontEbraico.AccessibleDescription = null;
            this.btnFontEbraico.AccessibleName = null;
            resources.ApplyResources(this.btnFontEbraico, "btnFontEbraico");
            this.btnFontEbraico.BackgroundImage = null;
            this.btnFontEbraico.Font = null;
            this.guidaFile.SetHelpKeyword(this.btnFontEbraico, resources.GetString("btnFontEbraico.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.btnFontEbraico, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("btnFontEbraico.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.btnFontEbraico, null);
            this.btnFontEbraico.Image = global::LaParola.Properties.Resources.fontstile;
            this.btnFontEbraico.Name = "btnFontEbraico";
            this.guidaFile.SetShowHelp(this.btnFontEbraico, ((bool)(resources.GetObject("btnFontEbraico.ShowHelp"))));
            this.btnFontEbraico.UseVisualStyleBackColor = true;
            this.btnFontEbraico.Click += new System.EventHandler(this.btnFontEbraico_Click);
            // 
            // panRisultati
            // 
            this.panRisultati.AccessibleDescription = null;
            this.panRisultati.AccessibleName = null;
            resources.ApplyResources(this.panRisultati, "panRisultati");
            this.panRisultati.BackgroundImage = null;
            this.panRisultati.Controls.Add(this.rtEsempio);
            this.panRisultati.Controls.Add(this.labEsempio);
            this.panRisultati.Font = null;
            this.guidaFile.SetHelpKeyword(this.panRisultati, null);
            this.guidaFile.SetHelpNavigator(this.panRisultati, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("panRisultati.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.panRisultati, null);
            this.panRisultati.Name = "panRisultati";
            this.guidaFile.SetShowHelp(this.panRisultati, ((bool)(resources.GetObject("panRisultati.ShowHelp"))));
            this.panRisultati.Tag = "panRisultati";
            // 
            // rtEsempio
            // 
            this.rtEsempio.AccessibleDescription = null;
            this.rtEsempio.AccessibleName = null;
            resources.ApplyResources(this.rtEsempio, "rtEsempio");
            this.rtEsempio.BackgroundImage = null;
            this.rtEsempio.Font = null;
            this.guidaFile.SetHelpKeyword(this.rtEsempio, resources.GetString("rtEsempio.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.rtEsempio, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("rtEsempio.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.rtEsempio, null);
            this.rtEsempio.Lingua = null;
            this.rtEsempio.Name = "rtEsempio";
            this.rtEsempio.ReadOnly = true;
            this.rtEsempio.SelectionAlignment = TestiBiblici.RichTextBoxEx.TextAlign.Left;
            this.guidaFile.SetShowHelp(this.rtEsempio, ((bool)(resources.GetObject("rtEsempio.ShowHelp"))));
            this.rtEsempio.Versione = null;
            // 
            // labEsempio
            // 
            this.labEsempio.AccessibleDescription = null;
            this.labEsempio.AccessibleName = null;
            resources.ApplyResources(this.labEsempio, "labEsempio");
            this.labEsempio.Font = null;
            this.guidaFile.SetHelpKeyword(this.labEsempio, null);
            this.guidaFile.SetHelpNavigator(this.labEsempio, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("labEsempio.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.labEsempio, null);
            this.labEsempio.Name = "labEsempio";
            this.guidaFile.SetShowHelp(this.labEsempio, ((bool)(resources.GetObject("labEsempio.ShowHelp"))));
            // 
            // panTesto
            // 
            this.panTesto.AccessibleDescription = null;
            this.panTesto.AccessibleName = null;
            resources.ApplyResources(this.panTesto, "panTesto");
            this.panTesto.BackgroundImage = null;
            this.panTesto.Controls.Add(this.cbTitoli);
            this.panTesto.Controls.Add(this.gbTesto);
            this.panTesto.Font = null;
            this.guidaFile.SetHelpKeyword(this.panTesto, null);
            this.guidaFile.SetHelpNavigator(this.panTesto, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("panTesto.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.panTesto, null);
            this.panTesto.Name = "panTesto";
            this.guidaFile.SetShowHelp(this.panTesto, ((bool)(resources.GetObject("panTesto.ShowHelp"))));
            this.panTesto.Tag = "panTesto";
            // 
            // cbTitoli
            // 
            this.cbTitoli.AccessibleDescription = null;
            this.cbTitoli.AccessibleName = null;
            resources.ApplyResources(this.cbTitoli, "cbTitoli");
            this.cbTitoli.BackgroundImage = null;
            this.cbTitoli.Font = null;
            this.guidaFile.SetHelpKeyword(this.cbTitoli, resources.GetString("cbTitoli.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbTitoli, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbTitoli.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbTitoli, null);
            this.cbTitoli.Name = "cbTitoli";
            this.guidaFile.SetShowHelp(this.cbTitoli, ((bool)(resources.GetObject("cbTitoli.ShowHelp"))));
            this.cbTitoli.UseVisualStyleBackColor = true;
            // 
            // gbTesto
            // 
            this.gbTesto.AccessibleDescription = null;
            this.gbTesto.AccessibleName = null;
            resources.ApplyResources(this.gbTesto, "gbTesto");
            this.gbTesto.BackgroundImage = null;
            this.gbTesto.Controls.Add(this.rbTesto2);
            this.gbTesto.Controls.Add(this.rbTesto1);
            this.gbTesto.Controls.Add(this.rbTesto0);
            this.gbTesto.Font = null;
            this.guidaFile.SetHelpKeyword(this.gbTesto, null);
            this.guidaFile.SetHelpNavigator(this.gbTesto, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("gbTesto.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.gbTesto, null);
            this.gbTesto.Name = "gbTesto";
            this.guidaFile.SetShowHelp(this.gbTesto, ((bool)(resources.GetObject("gbTesto.ShowHelp"))));
            this.gbTesto.TabStop = false;
            // 
            // rbTesto2
            // 
            this.rbTesto2.AccessibleDescription = null;
            this.rbTesto2.AccessibleName = null;
            resources.ApplyResources(this.rbTesto2, "rbTesto2");
            this.rbTesto2.BackgroundImage = null;
            this.rbTesto2.Font = null;
            this.guidaFile.SetHelpKeyword(this.rbTesto2, resources.GetString("rbTesto2.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.rbTesto2, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("rbTesto2.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.rbTesto2, null);
            this.rbTesto2.Name = "rbTesto2";
            this.guidaFile.SetShowHelp(this.rbTesto2, ((bool)(resources.GetObject("rbTesto2.ShowHelp"))));
            this.rbTesto2.TabStop = true;
            this.rbTesto2.UseVisualStyleBackColor = true;
            // 
            // rbTesto1
            // 
            this.rbTesto1.AccessibleDescription = null;
            this.rbTesto1.AccessibleName = null;
            resources.ApplyResources(this.rbTesto1, "rbTesto1");
            this.rbTesto1.BackgroundImage = null;
            this.rbTesto1.Font = null;
            this.guidaFile.SetHelpKeyword(this.rbTesto1, resources.GetString("rbTesto1.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.rbTesto1, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("rbTesto1.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.rbTesto1, null);
            this.rbTesto1.Name = "rbTesto1";
            this.guidaFile.SetShowHelp(this.rbTesto1, ((bool)(resources.GetObject("rbTesto1.ShowHelp"))));
            this.rbTesto1.TabStop = true;
            this.rbTesto1.UseVisualStyleBackColor = true;
            // 
            // rbTesto0
            // 
            this.rbTesto0.AccessibleDescription = null;
            this.rbTesto0.AccessibleName = null;
            resources.ApplyResources(this.rbTesto0, "rbTesto0");
            this.rbTesto0.BackgroundImage = null;
            this.rbTesto0.Font = null;
            this.guidaFile.SetHelpKeyword(this.rbTesto0, resources.GetString("rbTesto0.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.rbTesto0, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("rbTesto0.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.rbTesto0, null);
            this.rbTesto0.Name = "rbTesto0";
            this.guidaFile.SetShowHelp(this.rbTesto0, ((bool)(resources.GetObject("rbTesto0.ShowHelp"))));
            this.rbTesto0.TabStop = true;
            this.rbTesto0.UseVisualStyleBackColor = true;
            // 
            // panRiferimenti
            // 
            this.panRiferimenti.AccessibleDescription = null;
            this.panRiferimenti.AccessibleName = null;
            resources.ApplyResources(this.panRiferimenti, "panRiferimenti");
            this.panRiferimenti.BackgroundImage = null;
            this.panRiferimenti.Controls.Add(this.cbRifContestoRicerche);
            this.panRiferimenti.Controls.Add(this.gbPosto);
            this.panRiferimenti.Controls.Add(this.gbFormato);
            this.panRiferimenti.Controls.Add(this.gbRifTipo);
            this.panRiferimenti.Font = null;
            this.guidaFile.SetHelpKeyword(this.panRiferimenti, null);
            this.guidaFile.SetHelpNavigator(this.panRiferimenti, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("panRiferimenti.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.panRiferimenti, null);
            this.panRiferimenti.Name = "panRiferimenti";
            this.guidaFile.SetShowHelp(this.panRiferimenti, ((bool)(resources.GetObject("panRiferimenti.ShowHelp"))));
            this.panRiferimenti.Tag = "panRiferimenti";
            // 
            // cbRifContestoRicerche
            // 
            this.cbRifContestoRicerche.AccessibleDescription = null;
            this.cbRifContestoRicerche.AccessibleName = null;
            resources.ApplyResources(this.cbRifContestoRicerche, "cbRifContestoRicerche");
            this.cbRifContestoRicerche.BackgroundImage = null;
            this.cbRifContestoRicerche.Font = null;
            this.guidaFile.SetHelpKeyword(this.cbRifContestoRicerche, resources.GetString("cbRifContestoRicerche.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbRifContestoRicerche, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbRifContestoRicerche.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbRifContestoRicerche, null);
            this.cbRifContestoRicerche.Name = "cbRifContestoRicerche";
            this.guidaFile.SetShowHelp(this.cbRifContestoRicerche, ((bool)(resources.GetObject("cbRifContestoRicerche.ShowHelp"))));
            this.cbRifContestoRicerche.UseVisualStyleBackColor = true;
            // 
            // gbPosto
            // 
            this.gbPosto.AccessibleDescription = null;
            this.gbPosto.AccessibleName = null;
            resources.ApplyResources(this.gbPosto, "gbPosto");
            this.gbPosto.BackgroundImage = null;
            this.gbPosto.Controls.Add(this.rbRifPosto2);
            this.gbPosto.Controls.Add(this.rbRifPosto1);
            this.gbPosto.Controls.Add(this.rbRifPosto0);
            this.gbPosto.Font = null;
            this.guidaFile.SetHelpKeyword(this.gbPosto, null);
            this.guidaFile.SetHelpNavigator(this.gbPosto, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("gbPosto.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.gbPosto, null);
            this.gbPosto.Name = "gbPosto";
            this.guidaFile.SetShowHelp(this.gbPosto, ((bool)(resources.GetObject("gbPosto.ShowHelp"))));
            this.gbPosto.TabStop = false;
            // 
            // rbRifPosto2
            // 
            this.rbRifPosto2.AccessibleDescription = null;
            this.rbRifPosto2.AccessibleName = null;
            resources.ApplyResources(this.rbRifPosto2, "rbRifPosto2");
            this.rbRifPosto2.BackgroundImage = null;
            this.rbRifPosto2.Font = null;
            this.guidaFile.SetHelpKeyword(this.rbRifPosto2, resources.GetString("rbRifPosto2.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.rbRifPosto2, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("rbRifPosto2.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.rbRifPosto2, null);
            this.rbRifPosto2.Name = "rbRifPosto2";
            this.guidaFile.SetShowHelp(this.rbRifPosto2, ((bool)(resources.GetObject("rbRifPosto2.ShowHelp"))));
            this.rbRifPosto2.TabStop = true;
            this.rbRifPosto2.UseVisualStyleBackColor = true;
            // 
            // rbRifPosto1
            // 
            this.rbRifPosto1.AccessibleDescription = null;
            this.rbRifPosto1.AccessibleName = null;
            resources.ApplyResources(this.rbRifPosto1, "rbRifPosto1");
            this.rbRifPosto1.BackgroundImage = null;
            this.rbRifPosto1.Font = null;
            this.guidaFile.SetHelpKeyword(this.rbRifPosto1, resources.GetString("rbRifPosto1.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.rbRifPosto1, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("rbRifPosto1.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.rbRifPosto1, null);
            this.rbRifPosto1.Name = "rbRifPosto1";
            this.guidaFile.SetShowHelp(this.rbRifPosto1, ((bool)(resources.GetObject("rbRifPosto1.ShowHelp"))));
            this.rbRifPosto1.TabStop = true;
            this.rbRifPosto1.UseVisualStyleBackColor = true;
            // 
            // rbRifPosto0
            // 
            this.rbRifPosto0.AccessibleDescription = null;
            this.rbRifPosto0.AccessibleName = null;
            resources.ApplyResources(this.rbRifPosto0, "rbRifPosto0");
            this.rbRifPosto0.BackgroundImage = null;
            this.rbRifPosto0.Font = null;
            this.guidaFile.SetHelpKeyword(this.rbRifPosto0, resources.GetString("rbRifPosto0.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.rbRifPosto0, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("rbRifPosto0.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.rbRifPosto0, null);
            this.rbRifPosto0.Name = "rbRifPosto0";
            this.guidaFile.SetShowHelp(this.rbRifPosto0, ((bool)(resources.GetObject("rbRifPosto0.ShowHelp"))));
            this.rbRifPosto0.TabStop = true;
            this.rbRifPosto0.UseVisualStyleBackColor = true;
            // 
            // gbFormato
            // 
            this.gbFormato.AccessibleDescription = null;
            this.gbFormato.AccessibleName = null;
            resources.ApplyResources(this.gbFormato, "gbFormato");
            this.gbFormato.BackgroundImage = null;
            this.gbFormato.Controls.Add(this.rbRifFormato2);
            this.gbFormato.Controls.Add(this.rbRifFormato1);
            this.gbFormato.Controls.Add(this.rbRifFormato0);
            this.gbFormato.Font = null;
            this.guidaFile.SetHelpKeyword(this.gbFormato, null);
            this.guidaFile.SetHelpNavigator(this.gbFormato, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("gbFormato.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.gbFormato, null);
            this.gbFormato.Name = "gbFormato";
            this.guidaFile.SetShowHelp(this.gbFormato, ((bool)(resources.GetObject("gbFormato.ShowHelp"))));
            this.gbFormato.TabStop = false;
            // 
            // rbRifFormato2
            // 
            this.rbRifFormato2.AccessibleDescription = null;
            this.rbRifFormato2.AccessibleName = null;
            resources.ApplyResources(this.rbRifFormato2, "rbRifFormato2");
            this.rbRifFormato2.BackgroundImage = null;
            this.rbRifFormato2.Font = null;
            this.guidaFile.SetHelpKeyword(this.rbRifFormato2, resources.GetString("rbRifFormato2.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.rbRifFormato2, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("rbRifFormato2.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.rbRifFormato2, null);
            this.rbRifFormato2.Name = "rbRifFormato2";
            this.guidaFile.SetShowHelp(this.rbRifFormato2, ((bool)(resources.GetObject("rbRifFormato2.ShowHelp"))));
            this.rbRifFormato2.TabStop = true;
            this.rbRifFormato2.UseVisualStyleBackColor = true;
            // 
            // rbRifFormato1
            // 
            this.rbRifFormato1.AccessibleDescription = null;
            this.rbRifFormato1.AccessibleName = null;
            resources.ApplyResources(this.rbRifFormato1, "rbRifFormato1");
            this.rbRifFormato1.BackgroundImage = null;
            this.rbRifFormato1.Font = null;
            this.guidaFile.SetHelpKeyword(this.rbRifFormato1, resources.GetString("rbRifFormato1.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.rbRifFormato1, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("rbRifFormato1.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.rbRifFormato1, null);
            this.rbRifFormato1.Name = "rbRifFormato1";
            this.guidaFile.SetShowHelp(this.rbRifFormato1, ((bool)(resources.GetObject("rbRifFormato1.ShowHelp"))));
            this.rbRifFormato1.TabStop = true;
            this.rbRifFormato1.UseVisualStyleBackColor = true;
            // 
            // rbRifFormato0
            // 
            this.rbRifFormato0.AccessibleDescription = null;
            this.rbRifFormato0.AccessibleName = null;
            resources.ApplyResources(this.rbRifFormato0, "rbRifFormato0");
            this.rbRifFormato0.BackgroundImage = null;
            this.rbRifFormato0.Font = null;
            this.guidaFile.SetHelpKeyword(this.rbRifFormato0, resources.GetString("rbRifFormato0.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.rbRifFormato0, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("rbRifFormato0.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.rbRifFormato0, null);
            this.rbRifFormato0.Name = "rbRifFormato0";
            this.guidaFile.SetShowHelp(this.rbRifFormato0, ((bool)(resources.GetObject("rbRifFormato0.ShowHelp"))));
            this.rbRifFormato0.TabStop = true;
            this.rbRifFormato0.UseVisualStyleBackColor = true;
            // 
            // gbRifTipo
            // 
            this.gbRifTipo.AccessibleDescription = null;
            this.gbRifTipo.AccessibleName = null;
            resources.ApplyResources(this.gbRifTipo, "gbRifTipo");
            this.gbRifTipo.BackgroundImage = null;
            this.gbRifTipo.Controls.Add(this.rbRifTipo2);
            this.gbRifTipo.Controls.Add(this.rbRifTipo1);
            this.gbRifTipo.Controls.Add(this.rbRifTipo0);
            this.gbRifTipo.Font = null;
            this.guidaFile.SetHelpKeyword(this.gbRifTipo, null);
            this.guidaFile.SetHelpNavigator(this.gbRifTipo, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("gbRifTipo.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.gbRifTipo, null);
            this.gbRifTipo.Name = "gbRifTipo";
            this.guidaFile.SetShowHelp(this.gbRifTipo, ((bool)(resources.GetObject("gbRifTipo.ShowHelp"))));
            this.gbRifTipo.TabStop = false;
            // 
            // rbRifTipo2
            // 
            this.rbRifTipo2.AccessibleDescription = null;
            this.rbRifTipo2.AccessibleName = null;
            resources.ApplyResources(this.rbRifTipo2, "rbRifTipo2");
            this.rbRifTipo2.BackgroundImage = null;
            this.rbRifTipo2.Font = null;
            this.guidaFile.SetHelpKeyword(this.rbRifTipo2, resources.GetString("rbRifTipo2.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.rbRifTipo2, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("rbRifTipo2.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.rbRifTipo2, null);
            this.rbRifTipo2.Name = "rbRifTipo2";
            this.guidaFile.SetShowHelp(this.rbRifTipo2, ((bool)(resources.GetObject("rbRifTipo2.ShowHelp"))));
            this.rbRifTipo2.TabStop = true;
            this.rbRifTipo2.UseVisualStyleBackColor = true;
            // 
            // rbRifTipo1
            // 
            this.rbRifTipo1.AccessibleDescription = null;
            this.rbRifTipo1.AccessibleName = null;
            resources.ApplyResources(this.rbRifTipo1, "rbRifTipo1");
            this.rbRifTipo1.BackgroundImage = null;
            this.rbRifTipo1.Font = null;
            this.guidaFile.SetHelpKeyword(this.rbRifTipo1, resources.GetString("rbRifTipo1.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.rbRifTipo1, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("rbRifTipo1.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.rbRifTipo1, null);
            this.rbRifTipo1.Name = "rbRifTipo1";
            this.guidaFile.SetShowHelp(this.rbRifTipo1, ((bool)(resources.GetObject("rbRifTipo1.ShowHelp"))));
            this.rbRifTipo1.TabStop = true;
            this.rbRifTipo1.UseVisualStyleBackColor = true;
            // 
            // rbRifTipo0
            // 
            this.rbRifTipo0.AccessibleDescription = null;
            this.rbRifTipo0.AccessibleName = null;
            resources.ApplyResources(this.rbRifTipo0, "rbRifTipo0");
            this.rbRifTipo0.BackgroundImage = null;
            this.rbRifTipo0.Font = null;
            this.guidaFile.SetHelpKeyword(this.rbRifTipo0, resources.GetString("rbRifTipo0.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.rbRifTipo0, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("rbRifTipo0.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.rbRifTipo0, null);
            this.rbRifTipo0.Name = "rbRifTipo0";
            this.guidaFile.SetShowHelp(this.rbRifTipo0, ((bool)(resources.GetObject("rbRifTipo0.ShowHelp"))));
            this.rbRifTipo0.TabStop = true;
            this.rbRifTipo0.UseVisualStyleBackColor = true;
            // 
            // panLibri
            // 
            this.panLibri.AccessibleDescription = null;
            this.panLibri.AccessibleName = null;
            resources.ApplyResources(this.panLibri, "panLibri");
            this.panLibri.BackgroundImage = null;
            this.panLibri.Controls.Add(this.btnLibriSpagnolo);
            this.panLibri.Controls.Add(this.gridLibri);
            this.panLibri.Controls.Add(this.btnLibriItaliano);
            this.panLibri.Controls.Add(this.btnLibriInglese);
            this.panLibri.Font = null;
            this.guidaFile.SetHelpKeyword(this.panLibri, null);
            this.guidaFile.SetHelpNavigator(this.panLibri, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("panLibri.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.panLibri, null);
            this.panLibri.Name = "panLibri";
            this.guidaFile.SetShowHelp(this.panLibri, ((bool)(resources.GetObject("panLibri.ShowHelp"))));
            this.panLibri.Tag = "panLibri";
            // 
            // btnLibriSpagnolo
            // 
            this.btnLibriSpagnolo.AccessibleDescription = null;
            this.btnLibriSpagnolo.AccessibleName = null;
            resources.ApplyResources(this.btnLibriSpagnolo, "btnLibriSpagnolo");
            this.btnLibriSpagnolo.BackgroundImage = null;
            this.btnLibriSpagnolo.Font = null;
            this.guidaFile.SetHelpKeyword(this.btnLibriSpagnolo, resources.GetString("btnLibriSpagnolo.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.btnLibriSpagnolo, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("btnLibriSpagnolo.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.btnLibriSpagnolo, null);
            this.btnLibriSpagnolo.Name = "btnLibriSpagnolo";
            this.guidaFile.SetShowHelp(this.btnLibriSpagnolo, ((bool)(resources.GetObject("btnLibriSpagnolo.ShowHelp"))));
            this.btnLibriSpagnolo.UseVisualStyleBackColor = true;
            this.btnLibriSpagnolo.Click += new System.EventHandler(this.btnLibri_Click);
            // 
            // gridLibri
            // 
            this.gridLibri.AccessibleDescription = null;
            this.gridLibri.AccessibleName = null;
            this.gridLibri.AllowUserToAddRows = false;
            this.gridLibri.AllowUserToDeleteRows = false;
            resources.ApplyResources(this.gridLibri, "gridLibri");
            this.gridLibri.BackgroundImage = null;
            this.gridLibri.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.gridLibri.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridLibri.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Nome,
            this.AbbUsate,
            this.AbbRicono});
            this.gridLibri.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.gridLibri.Font = null;
            this.guidaFile.SetHelpKeyword(this.gridLibri, resources.GetString("gridLibri.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.gridLibri, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("gridLibri.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.gridLibri, null);
            this.gridLibri.MultiSelect = false;
            this.gridLibri.Name = "gridLibri";
            this.gridLibri.RowHeadersVisible = false;
            this.guidaFile.SetShowHelp(this.gridLibri, ((bool)(resources.GetObject("gridLibri.ShowHelp"))));
            // 
            // btnLibriItaliano
            // 
            this.btnLibriItaliano.AccessibleDescription = null;
            this.btnLibriItaliano.AccessibleName = null;
            resources.ApplyResources(this.btnLibriItaliano, "btnLibriItaliano");
            this.btnLibriItaliano.BackgroundImage = null;
            this.btnLibriItaliano.Font = null;
            this.guidaFile.SetHelpKeyword(this.btnLibriItaliano, resources.GetString("btnLibriItaliano.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.btnLibriItaliano, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("btnLibriItaliano.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.btnLibriItaliano, null);
            this.btnLibriItaliano.Name = "btnLibriItaliano";
            this.guidaFile.SetShowHelp(this.btnLibriItaliano, ((bool)(resources.GetObject("btnLibriItaliano.ShowHelp"))));
            this.btnLibriItaliano.UseVisualStyleBackColor = true;
            this.btnLibriItaliano.Click += new System.EventHandler(this.btnLibri_Click);
            // 
            // btnLibriInglese
            // 
            this.btnLibriInglese.AccessibleDescription = null;
            this.btnLibriInglese.AccessibleName = null;
            resources.ApplyResources(this.btnLibriInglese, "btnLibriInglese");
            this.btnLibriInglese.BackgroundImage = null;
            this.btnLibriInglese.Font = null;
            this.guidaFile.SetHelpKeyword(this.btnLibriInglese, resources.GetString("btnLibriInglese.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.btnLibriInglese, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("btnLibriInglese.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.btnLibriInglese, null);
            this.btnLibriInglese.Name = "btnLibriInglese";
            this.guidaFile.SetShowHelp(this.btnLibriInglese, ((bool)(resources.GetObject("btnLibriInglese.ShowHelp"))));
            this.btnLibriInglese.UseVisualStyleBackColor = true;
            this.btnLibriInglese.Click += new System.EventHandler(this.btnLibri_Click);
            // 
            // panTesti
            // 
            this.panTesti.AccessibleDescription = null;
            this.panTesti.AccessibleName = null;
            this.panTesti.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            resources.ApplyResources(this.panTesti, "panTesti");
            this.panTesti.BackgroundImage = null;
            this.panTesti.Controls.Add(this.pulCancellaCartella);
            this.panTesti.Controls.Add(this.pulAggiungiCartella);
            this.panTesti.Controls.Add(this.lbCartelle);
            this.panTesti.Controls.Add(this.labCartelle);
            this.panTesti.Controls.Add(this.clbCommentari);
            this.panTesti.Controls.Add(this.labCommentari);
            this.panTesti.Font = null;
            this.guidaFile.SetHelpKeyword(this.panTesti, null);
            this.guidaFile.SetHelpNavigator(this.panTesti, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("panTesti.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.panTesti, null);
            this.panTesti.Name = "panTesti";
            this.guidaFile.SetShowHelp(this.panTesti, ((bool)(resources.GetObject("panTesti.ShowHelp"))));
            this.panTesti.Tag = "panTesti";
            // 
            // pulCancellaCartella
            // 
            this.pulCancellaCartella.AccessibleDescription = null;
            this.pulCancellaCartella.AccessibleName = null;
            resources.ApplyResources(this.pulCancellaCartella, "pulCancellaCartella");
            this.pulCancellaCartella.BackgroundImage = null;
            this.pulCancellaCartella.Font = null;
            this.guidaFile.SetHelpKeyword(this.pulCancellaCartella, resources.GetString("pulCancellaCartella.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.pulCancellaCartella, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulCancellaCartella.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.pulCancellaCartella, null);
            this.pulCancellaCartella.Name = "pulCancellaCartella";
            this.guidaFile.SetShowHelp(this.pulCancellaCartella, ((bool)(resources.GetObject("pulCancellaCartella.ShowHelp"))));
            this.pulCancellaCartella.UseVisualStyleBackColor = true;
            this.pulCancellaCartella.Click += new System.EventHandler(this.pulCancellaCartella_Click);
            // 
            // pulAggiungiCartella
            // 
            this.pulAggiungiCartella.AccessibleDescription = null;
            this.pulAggiungiCartella.AccessibleName = null;
            resources.ApplyResources(this.pulAggiungiCartella, "pulAggiungiCartella");
            this.pulAggiungiCartella.BackgroundImage = null;
            this.pulAggiungiCartella.Font = null;
            this.guidaFile.SetHelpKeyword(this.pulAggiungiCartella, resources.GetString("pulAggiungiCartella.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.pulAggiungiCartella, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulAggiungiCartella.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.pulAggiungiCartella, null);
            this.pulAggiungiCartella.Name = "pulAggiungiCartella";
            this.guidaFile.SetShowHelp(this.pulAggiungiCartella, ((bool)(resources.GetObject("pulAggiungiCartella.ShowHelp"))));
            this.pulAggiungiCartella.UseVisualStyleBackColor = true;
            this.pulAggiungiCartella.Click += new System.EventHandler(this.pulAggiungiCartella_Click);
            // 
            // lbCartelle
            // 
            this.lbCartelle.AccessibleDescription = null;
            this.lbCartelle.AccessibleName = null;
            resources.ApplyResources(this.lbCartelle, "lbCartelle");
            this.lbCartelle.BackgroundImage = null;
            this.lbCartelle.Font = null;
            this.lbCartelle.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.lbCartelle, resources.GetString("lbCartelle.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.lbCartelle, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("lbCartelle.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.lbCartelle, null);
            this.lbCartelle.Name = "lbCartelle";
            this.guidaFile.SetShowHelp(this.lbCartelle, ((bool)(resources.GetObject("lbCartelle.ShowHelp"))));
            this.lbCartelle.SelectedIndexChanged += new System.EventHandler(this.lbCartelle_SelectedIndexChanged);
            // 
            // labCartelle
            // 
            this.labCartelle.AccessibleDescription = null;
            this.labCartelle.AccessibleName = null;
            resources.ApplyResources(this.labCartelle, "labCartelle");
            this.labCartelle.Font = null;
            this.guidaFile.SetHelpKeyword(this.labCartelle, resources.GetString("labCartelle.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.labCartelle, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("labCartelle.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.labCartelle, null);
            this.labCartelle.Name = "labCartelle";
            this.guidaFile.SetShowHelp(this.labCartelle, ((bool)(resources.GetObject("labCartelle.ShowHelp"))));
            // 
            // clbCommentari
            // 
            this.clbCommentari.AccessibleDescription = null;
            this.clbCommentari.AccessibleName = null;
            resources.ApplyResources(this.clbCommentari, "clbCommentari");
            this.clbCommentari.BackgroundImage = null;
            this.clbCommentari.CheckOnClick = true;
            this.clbCommentari.Font = null;
            this.clbCommentari.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.clbCommentari, resources.GetString("clbCommentari.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.clbCommentari, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("clbCommentari.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.clbCommentari, null);
            this.clbCommentari.Name = "clbCommentari";
            this.guidaFile.SetShowHelp(this.clbCommentari, ((bool)(resources.GetObject("clbCommentari.ShowHelp"))));
            // 
            // labCommentari
            // 
            this.labCommentari.AccessibleDescription = null;
            this.labCommentari.AccessibleName = null;
            resources.ApplyResources(this.labCommentari, "labCommentari");
            this.labCommentari.Font = null;
            this.guidaFile.SetHelpKeyword(this.labCommentari, resources.GetString("labCommentari.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.labCommentari, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("labCommentari.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.labCommentari, null);
            this.labCommentari.Name = "labCommentari";
            this.guidaFile.SetShowHelp(this.labCommentari, ((bool)(resources.GetObject("labCommentari.ShowHelp"))));
            // 
            // panAggiornamenti
            // 
            this.panAggiornamenti.AccessibleDescription = null;
            this.panAggiornamenti.AccessibleName = null;
            resources.ApplyResources(this.panAggiornamenti, "panAggiornamenti");
            this.panAggiornamenti.BackgroundImage = null;
            this.panAggiornamenti.Controls.Add(this.tbProxyDominio);
            this.panAggiornamenti.Controls.Add(this.etiProxyDominio);
            this.panAggiornamenti.Controls.Add(this.tbProxyPassword);
            this.panAggiornamenti.Controls.Add(this.etiProxyPassword);
            this.panAggiornamenti.Controls.Add(this.tbProxyNomeUtente);
            this.panAggiornamenti.Controls.Add(this.etiProxyNomeUtente);
            this.panAggiornamenti.Controls.Add(this.tbProxyPorta);
            this.panAggiornamenti.Controls.Add(this.etiProxyPort);
            this.panAggiornamenti.Controls.Add(this.tbProxy);
            this.panAggiornamenti.Controls.Add(this.etiProxy);
            this.panAggiornamenti.Controls.Add(this.cbAggiornaGiorni);
            this.panAggiornamenti.Controls.Add(this.rbAggiornaAutomatica);
            this.panAggiornamenti.Controls.Add(this.rbAggiornaManuale);
            this.panAggiornamenti.Font = null;
            this.guidaFile.SetHelpKeyword(this.panAggiornamenti, null);
            this.guidaFile.SetHelpNavigator(this.panAggiornamenti, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("panAggiornamenti.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.panAggiornamenti, null);
            this.panAggiornamenti.Name = "panAggiornamenti";
            this.guidaFile.SetShowHelp(this.panAggiornamenti, ((bool)(resources.GetObject("panAggiornamenti.ShowHelp"))));
            this.panAggiornamenti.Tag = "panAggiornamenti";
            // 
            // tbProxyDominio
            // 
            this.tbProxyDominio.AccessibleDescription = null;
            this.tbProxyDominio.AccessibleName = null;
            resources.ApplyResources(this.tbProxyDominio, "tbProxyDominio");
            this.tbProxyDominio.BackgroundImage = null;
            this.tbProxyDominio.Font = null;
            this.guidaFile.SetHelpKeyword(this.tbProxyDominio, resources.GetString("tbProxyDominio.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.tbProxyDominio, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("tbProxyDominio.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.tbProxyDominio, null);
            this.tbProxyDominio.Name = "tbProxyDominio";
            this.guidaFile.SetShowHelp(this.tbProxyDominio, ((bool)(resources.GetObject("tbProxyDominio.ShowHelp"))));
            // 
            // etiProxyDominio
            // 
            this.etiProxyDominio.AccessibleDescription = null;
            this.etiProxyDominio.AccessibleName = null;
            resources.ApplyResources(this.etiProxyDominio, "etiProxyDominio");
            this.etiProxyDominio.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiProxyDominio, resources.GetString("etiProxyDominio.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.etiProxyDominio, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiProxyDominio.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiProxyDominio, null);
            this.etiProxyDominio.Name = "etiProxyDominio";
            this.guidaFile.SetShowHelp(this.etiProxyDominio, ((bool)(resources.GetObject("etiProxyDominio.ShowHelp"))));
            // 
            // tbProxyPassword
            // 
            this.tbProxyPassword.AccessibleDescription = null;
            this.tbProxyPassword.AccessibleName = null;
            resources.ApplyResources(this.tbProxyPassword, "tbProxyPassword");
            this.tbProxyPassword.BackgroundImage = null;
            this.tbProxyPassword.Font = null;
            this.guidaFile.SetHelpKeyword(this.tbProxyPassword, resources.GetString("tbProxyPassword.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.tbProxyPassword, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("tbProxyPassword.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.tbProxyPassword, null);
            this.tbProxyPassword.Name = "tbProxyPassword";
            this.guidaFile.SetShowHelp(this.tbProxyPassword, ((bool)(resources.GetObject("tbProxyPassword.ShowHelp"))));
            // 
            // etiProxyPassword
            // 
            this.etiProxyPassword.AccessibleDescription = null;
            this.etiProxyPassword.AccessibleName = null;
            resources.ApplyResources(this.etiProxyPassword, "etiProxyPassword");
            this.etiProxyPassword.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiProxyPassword, resources.GetString("etiProxyPassword.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.etiProxyPassword, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiProxyPassword.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiProxyPassword, null);
            this.etiProxyPassword.Name = "etiProxyPassword";
            this.guidaFile.SetShowHelp(this.etiProxyPassword, ((bool)(resources.GetObject("etiProxyPassword.ShowHelp"))));
            // 
            // tbProxyNomeUtente
            // 
            this.tbProxyNomeUtente.AccessibleDescription = null;
            this.tbProxyNomeUtente.AccessibleName = null;
            resources.ApplyResources(this.tbProxyNomeUtente, "tbProxyNomeUtente");
            this.tbProxyNomeUtente.BackgroundImage = null;
            this.tbProxyNomeUtente.Font = null;
            this.guidaFile.SetHelpKeyword(this.tbProxyNomeUtente, resources.GetString("tbProxyNomeUtente.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.tbProxyNomeUtente, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("tbProxyNomeUtente.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.tbProxyNomeUtente, null);
            this.tbProxyNomeUtente.Name = "tbProxyNomeUtente";
            this.guidaFile.SetShowHelp(this.tbProxyNomeUtente, ((bool)(resources.GetObject("tbProxyNomeUtente.ShowHelp"))));
            // 
            // etiProxyNomeUtente
            // 
            this.etiProxyNomeUtente.AccessibleDescription = null;
            this.etiProxyNomeUtente.AccessibleName = null;
            resources.ApplyResources(this.etiProxyNomeUtente, "etiProxyNomeUtente");
            this.etiProxyNomeUtente.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiProxyNomeUtente, resources.GetString("etiProxyNomeUtente.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.etiProxyNomeUtente, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiProxyNomeUtente.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiProxyNomeUtente, null);
            this.etiProxyNomeUtente.Name = "etiProxyNomeUtente";
            this.guidaFile.SetShowHelp(this.etiProxyNomeUtente, ((bool)(resources.GetObject("etiProxyNomeUtente.ShowHelp"))));
            // 
            // tbProxyPorta
            // 
            this.tbProxyPorta.AccessibleDescription = null;
            this.tbProxyPorta.AccessibleName = null;
            resources.ApplyResources(this.tbProxyPorta, "tbProxyPorta");
            this.tbProxyPorta.BackgroundImage = null;
            this.tbProxyPorta.Font = null;
            this.guidaFile.SetHelpKeyword(this.tbProxyPorta, resources.GetString("tbProxyPorta.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.tbProxyPorta, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("tbProxyPorta.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.tbProxyPorta, null);
            this.tbProxyPorta.Name = "tbProxyPorta";
            this.guidaFile.SetShowHelp(this.tbProxyPorta, ((bool)(resources.GetObject("tbProxyPorta.ShowHelp"))));
            this.tbProxyPorta.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbPorta_KeyDown);
            this.tbProxyPorta.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbPorta_KeyPress);
            // 
            // etiProxyPort
            // 
            this.etiProxyPort.AccessibleDescription = null;
            this.etiProxyPort.AccessibleName = null;
            resources.ApplyResources(this.etiProxyPort, "etiProxyPort");
            this.etiProxyPort.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiProxyPort, resources.GetString("etiProxyPort.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.etiProxyPort, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiProxyPort.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiProxyPort, null);
            this.etiProxyPort.Name = "etiProxyPort";
            this.guidaFile.SetShowHelp(this.etiProxyPort, ((bool)(resources.GetObject("etiProxyPort.ShowHelp"))));
            // 
            // tbProxy
            // 
            this.tbProxy.AccessibleDescription = null;
            this.tbProxy.AccessibleName = null;
            resources.ApplyResources(this.tbProxy, "tbProxy");
            this.tbProxy.BackgroundImage = null;
            this.tbProxy.Font = null;
            this.guidaFile.SetHelpKeyword(this.tbProxy, resources.GetString("tbProxy.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.tbProxy, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("tbProxy.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.tbProxy, null);
            this.tbProxy.Name = "tbProxy";
            this.guidaFile.SetShowHelp(this.tbProxy, ((bool)(resources.GetObject("tbProxy.ShowHelp"))));
            // 
            // etiProxy
            // 
            this.etiProxy.AccessibleDescription = null;
            this.etiProxy.AccessibleName = null;
            resources.ApplyResources(this.etiProxy, "etiProxy");
            this.etiProxy.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiProxy, resources.GetString("etiProxy.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.etiProxy, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiProxy.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiProxy, null);
            this.etiProxy.Name = "etiProxy";
            this.guidaFile.SetShowHelp(this.etiProxy, ((bool)(resources.GetObject("etiProxy.ShowHelp"))));
            // 
            // cbAggiornaGiorni
            // 
            this.cbAggiornaGiorni.AccessibleDescription = null;
            this.cbAggiornaGiorni.AccessibleName = null;
            resources.ApplyResources(this.cbAggiornaGiorni, "cbAggiornaGiorni");
            this.cbAggiornaGiorni.BackgroundImage = null;
            this.cbAggiornaGiorni.Font = null;
            this.cbAggiornaGiorni.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.cbAggiornaGiorni, resources.GetString("cbAggiornaGiorni.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbAggiornaGiorni, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbAggiornaGiorni.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbAggiornaGiorni, null);
            this.cbAggiornaGiorni.Items.AddRange(new object[] {
            resources.GetString("cbAggiornaGiorni.Items"),
            resources.GetString("cbAggiornaGiorni.Items1"),
            resources.GetString("cbAggiornaGiorni.Items2"),
            resources.GetString("cbAggiornaGiorni.Items3"),
            resources.GetString("cbAggiornaGiorni.Items4")});
            this.cbAggiornaGiorni.Name = "cbAggiornaGiorni";
            this.guidaFile.SetShowHelp(this.cbAggiornaGiorni, ((bool)(resources.GetObject("cbAggiornaGiorni.ShowHelp"))));
            // 
            // rbAggiornaAutomatica
            // 
            this.rbAggiornaAutomatica.AccessibleDescription = null;
            this.rbAggiornaAutomatica.AccessibleName = null;
            resources.ApplyResources(this.rbAggiornaAutomatica, "rbAggiornaAutomatica");
            this.rbAggiornaAutomatica.BackgroundImage = null;
            this.rbAggiornaAutomatica.Checked = true;
            this.rbAggiornaAutomatica.Font = null;
            this.guidaFile.SetHelpKeyword(this.rbAggiornaAutomatica, resources.GetString("rbAggiornaAutomatica.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.rbAggiornaAutomatica, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("rbAggiornaAutomatica.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.rbAggiornaAutomatica, null);
            this.rbAggiornaAutomatica.Name = "rbAggiornaAutomatica";
            this.guidaFile.SetShowHelp(this.rbAggiornaAutomatica, ((bool)(resources.GetObject("rbAggiornaAutomatica.ShowHelp"))));
            this.rbAggiornaAutomatica.TabStop = true;
            this.rbAggiornaAutomatica.UseVisualStyleBackColor = true;
            this.rbAggiornaAutomatica.CheckedChanged += new System.EventHandler(this.rbAggiornaAutomatica_CheckedChanged);
            // 
            // rbAggiornaManuale
            // 
            this.rbAggiornaManuale.AccessibleDescription = null;
            this.rbAggiornaManuale.AccessibleName = null;
            resources.ApplyResources(this.rbAggiornaManuale, "rbAggiornaManuale");
            this.rbAggiornaManuale.BackgroundImage = null;
            this.rbAggiornaManuale.Font = null;
            this.guidaFile.SetHelpKeyword(this.rbAggiornaManuale, resources.GetString("rbAggiornaManuale.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.rbAggiornaManuale, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("rbAggiornaManuale.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.rbAggiornaManuale, null);
            this.rbAggiornaManuale.Name = "rbAggiornaManuale";
            this.guidaFile.SetShowHelp(this.rbAggiornaManuale, ((bool)(resources.GetObject("rbAggiornaManuale.ShowHelp"))));
            this.rbAggiornaManuale.UseVisualStyleBackColor = true;
            // 
            // panDizionari
            // 
            this.panDizionari.AccessibleDescription = null;
            this.panDizionari.AccessibleName = null;
            resources.ApplyResources(this.panDizionari, "panDizionari");
            this.panDizionari.BackgroundImage = null;
            this.panDizionari.Controls.Add(this.labDizionarioLatino);
            this.panDizionari.Controls.Add(this.cbDizionariLatini);
            this.panDizionari.Controls.Add(this.cbDizionariEbraici);
            this.panDizionari.Controls.Add(this.labDizionarioEbraico);
            this.panDizionari.Controls.Add(this.labDizionarioGreco);
            this.panDizionari.Controls.Add(this.labDizionarioItaliano);
            this.panDizionari.Controls.Add(this.labDizionarioInglese);
            this.panDizionari.Controls.Add(this.cbDizionarioTooltip);
            this.panDizionari.Controls.Add(this.cbDizionariGreci);
            this.panDizionari.Controls.Add(this.cbDizionariItaliani);
            this.panDizionari.Controls.Add(this.cbDizionariInglesi);
            this.panDizionari.Font = null;
            this.guidaFile.SetHelpKeyword(this.panDizionari, null);
            this.guidaFile.SetHelpNavigator(this.panDizionari, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("panDizionari.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.panDizionari, null);
            this.panDizionari.Name = "panDizionari";
            this.guidaFile.SetShowHelp(this.panDizionari, ((bool)(resources.GetObject("panDizionari.ShowHelp"))));
            this.panDizionari.Tag = "panDizionari";
            // 
            // labDizionarioLatino
            // 
            this.labDizionarioLatino.AccessibleDescription = null;
            this.labDizionarioLatino.AccessibleName = null;
            resources.ApplyResources(this.labDizionarioLatino, "labDizionarioLatino");
            this.labDizionarioLatino.Font = null;
            this.guidaFile.SetHelpKeyword(this.labDizionarioLatino, resources.GetString("labDizionarioLatino.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.labDizionarioLatino, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("labDizionarioLatino.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.labDizionarioLatino, null);
            this.labDizionarioLatino.Name = "labDizionarioLatino";
            this.guidaFile.SetShowHelp(this.labDizionarioLatino, ((bool)(resources.GetObject("labDizionarioLatino.ShowHelp"))));
            // 
            // cbDizionariLatini
            // 
            this.cbDizionariLatini.AccessibleDescription = null;
            this.cbDizionariLatini.AccessibleName = null;
            resources.ApplyResources(this.cbDizionariLatini, "cbDizionariLatini");
            this.cbDizionariLatini.BackgroundImage = null;
            this.cbDizionariLatini.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDizionariLatini.Font = null;
            this.cbDizionariLatini.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.cbDizionariLatini, resources.GetString("cbDizionariLatini.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbDizionariLatini, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbDizionariLatini.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbDizionariLatini, null);
            this.cbDizionariLatini.Name = "cbDizionariLatini";
            this.guidaFile.SetShowHelp(this.cbDizionariLatini, ((bool)(resources.GetObject("cbDizionariLatini.ShowHelp"))));
            // 
            // cbDizionariEbraici
            // 
            this.cbDizionariEbraici.AccessibleDescription = null;
            this.cbDizionariEbraici.AccessibleName = null;
            resources.ApplyResources(this.cbDizionariEbraici, "cbDizionariEbraici");
            this.cbDizionariEbraici.BackgroundImage = null;
            this.cbDizionariEbraici.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDizionariEbraici.Font = null;
            this.cbDizionariEbraici.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.cbDizionariEbraici, resources.GetString("cbDizionariEbraici.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbDizionariEbraici, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbDizionariEbraici.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbDizionariEbraici, null);
            this.cbDizionariEbraici.Name = "cbDizionariEbraici";
            this.guidaFile.SetShowHelp(this.cbDizionariEbraici, ((bool)(resources.GetObject("cbDizionariEbraici.ShowHelp"))));
            // 
            // labDizionarioEbraico
            // 
            this.labDizionarioEbraico.AccessibleDescription = null;
            this.labDizionarioEbraico.AccessibleName = null;
            resources.ApplyResources(this.labDizionarioEbraico, "labDizionarioEbraico");
            this.labDizionarioEbraico.Font = null;
            this.guidaFile.SetHelpKeyword(this.labDizionarioEbraico, resources.GetString("labDizionarioEbraico.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.labDizionarioEbraico, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("labDizionarioEbraico.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.labDizionarioEbraico, null);
            this.labDizionarioEbraico.Name = "labDizionarioEbraico";
            this.guidaFile.SetShowHelp(this.labDizionarioEbraico, ((bool)(resources.GetObject("labDizionarioEbraico.ShowHelp"))));
            // 
            // labDizionarioGreco
            // 
            this.labDizionarioGreco.AccessibleDescription = null;
            this.labDizionarioGreco.AccessibleName = null;
            resources.ApplyResources(this.labDizionarioGreco, "labDizionarioGreco");
            this.labDizionarioGreco.Font = null;
            this.guidaFile.SetHelpKeyword(this.labDizionarioGreco, resources.GetString("labDizionarioGreco.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.labDizionarioGreco, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("labDizionarioGreco.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.labDizionarioGreco, null);
            this.labDizionarioGreco.Name = "labDizionarioGreco";
            this.guidaFile.SetShowHelp(this.labDizionarioGreco, ((bool)(resources.GetObject("labDizionarioGreco.ShowHelp"))));
            // 
            // labDizionarioItaliano
            // 
            this.labDizionarioItaliano.AccessibleDescription = null;
            this.labDizionarioItaliano.AccessibleName = null;
            resources.ApplyResources(this.labDizionarioItaliano, "labDizionarioItaliano");
            this.labDizionarioItaliano.Font = null;
            this.guidaFile.SetHelpKeyword(this.labDizionarioItaliano, resources.GetString("labDizionarioItaliano.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.labDizionarioItaliano, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("labDizionarioItaliano.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.labDizionarioItaliano, null);
            this.labDizionarioItaliano.Name = "labDizionarioItaliano";
            this.guidaFile.SetShowHelp(this.labDizionarioItaliano, ((bool)(resources.GetObject("labDizionarioItaliano.ShowHelp"))));
            // 
            // labDizionarioInglese
            // 
            this.labDizionarioInglese.AccessibleDescription = null;
            this.labDizionarioInglese.AccessibleName = null;
            resources.ApplyResources(this.labDizionarioInglese, "labDizionarioInglese");
            this.labDizionarioInglese.Font = null;
            this.guidaFile.SetHelpKeyword(this.labDizionarioInglese, resources.GetString("labDizionarioInglese.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.labDizionarioInglese, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("labDizionarioInglese.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.labDizionarioInglese, null);
            this.labDizionarioInglese.Name = "labDizionarioInglese";
            this.guidaFile.SetShowHelp(this.labDizionarioInglese, ((bool)(resources.GetObject("labDizionarioInglese.ShowHelp"))));
            // 
            // cbDizionarioTooltip
            // 
            this.cbDizionarioTooltip.AccessibleDescription = null;
            this.cbDizionarioTooltip.AccessibleName = null;
            resources.ApplyResources(this.cbDizionarioTooltip, "cbDizionarioTooltip");
            this.cbDizionarioTooltip.BackgroundImage = null;
            this.cbDizionarioTooltip.Font = null;
            this.guidaFile.SetHelpKeyword(this.cbDizionarioTooltip, resources.GetString("cbDizionarioTooltip.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbDizionarioTooltip, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbDizionarioTooltip.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbDizionarioTooltip, null);
            this.cbDizionarioTooltip.Name = "cbDizionarioTooltip";
            this.guidaFile.SetShowHelp(this.cbDizionarioTooltip, ((bool)(resources.GetObject("cbDizionarioTooltip.ShowHelp"))));
            this.cbDizionarioTooltip.UseVisualStyleBackColor = true;
            // 
            // cbDizionariGreci
            // 
            this.cbDizionariGreci.AccessibleDescription = null;
            this.cbDizionariGreci.AccessibleName = null;
            resources.ApplyResources(this.cbDizionariGreci, "cbDizionariGreci");
            this.cbDizionariGreci.BackgroundImage = null;
            this.cbDizionariGreci.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDizionariGreci.Font = null;
            this.cbDizionariGreci.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.cbDizionariGreci, resources.GetString("cbDizionariGreci.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbDizionariGreci, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbDizionariGreci.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbDizionariGreci, null);
            this.cbDizionariGreci.Name = "cbDizionariGreci";
            this.guidaFile.SetShowHelp(this.cbDizionariGreci, ((bool)(resources.GetObject("cbDizionariGreci.ShowHelp"))));
            // 
            // cbDizionariItaliani
            // 
            this.cbDizionariItaliani.AccessibleDescription = null;
            this.cbDizionariItaliani.AccessibleName = null;
            resources.ApplyResources(this.cbDizionariItaliani, "cbDizionariItaliani");
            this.cbDizionariItaliani.BackgroundImage = null;
            this.cbDizionariItaliani.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDizionariItaliani.Font = null;
            this.cbDizionariItaliani.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.cbDizionariItaliani, resources.GetString("cbDizionariItaliani.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbDizionariItaliani, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbDizionariItaliani.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbDizionariItaliani, null);
            this.cbDizionariItaliani.Name = "cbDizionariItaliani";
            this.guidaFile.SetShowHelp(this.cbDizionariItaliani, ((bool)(resources.GetObject("cbDizionariItaliani.ShowHelp"))));
            // 
            // cbDizionariInglesi
            // 
            this.cbDizionariInglesi.AccessibleDescription = null;
            this.cbDizionariInglesi.AccessibleName = null;
            resources.ApplyResources(this.cbDizionariInglesi, "cbDizionariInglesi");
            this.cbDizionariInglesi.BackgroundImage = null;
            this.cbDizionariInglesi.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDizionariInglesi.Font = null;
            this.cbDizionariInglesi.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.cbDizionariInglesi, resources.GetString("cbDizionariInglesi.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbDizionariInglesi, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbDizionariInglesi.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbDizionariInglesi, null);
            this.cbDizionariInglesi.Name = "cbDizionariInglesi";
            this.guidaFile.SetShowHelp(this.cbDizionariInglesi, ((bool)(resources.GetObject("cbDizionariInglesi.ShowHelp"))));
            // 
            // panAltre
            // 
            this.panAltre.AccessibleDescription = null;
            this.panAltre.AccessibleName = null;
            resources.ApplyResources(this.panAltre, "panAltre");
            this.panAltre.BackgroundImage = null;
            this.panAltre.Controls.Add(this.pulReload);
            this.panAltre.Controls.Add(this.pulReset);
            this.panAltre.Controls.Add(this.cbDisposizioni);
            this.panAltre.Controls.Add(this.etiDisposizione);
            this.panAltre.Controls.Add(this.cbLetture);
            this.panAltre.Font = null;
            this.guidaFile.SetHelpKeyword(this.panAltre, null);
            this.guidaFile.SetHelpNavigator(this.panAltre, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("panAltre.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.panAltre, null);
            this.panAltre.Name = "panAltre";
            this.guidaFile.SetShowHelp(this.panAltre, ((bool)(resources.GetObject("panAltre.ShowHelp"))));
            this.panAltre.Tag = "panAltre";
            // 
            // pulReload
            // 
            this.pulReload.AccessibleDescription = null;
            this.pulReload.AccessibleName = null;
            resources.ApplyResources(this.pulReload, "pulReload");
            this.pulReload.BackgroundImage = null;
            this.pulReload.Font = null;
            this.guidaFile.SetHelpKeyword(this.pulReload, resources.GetString("pulReload.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.pulReload, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulReload.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.pulReload, null);
            this.pulReload.Name = "pulReload";
            this.guidaFile.SetShowHelp(this.pulReload, ((bool)(resources.GetObject("pulReload.ShowHelp"))));
            this.pulReload.UseVisualStyleBackColor = true;
            this.pulReload.Click += new System.EventHandler(this.pulReload_Click);
            // 
            // pulReset
            // 
            this.pulReset.AccessibleDescription = null;
            this.pulReset.AccessibleName = null;
            resources.ApplyResources(this.pulReset, "pulReset");
            this.pulReset.BackgroundImage = null;
            this.pulReset.Font = null;
            this.guidaFile.SetHelpKeyword(this.pulReset, resources.GetString("pulReset.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.pulReset, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulReset.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.pulReset, null);
            this.pulReset.Name = "pulReset";
            this.guidaFile.SetShowHelp(this.pulReset, ((bool)(resources.GetObject("pulReset.ShowHelp"))));
            this.pulReset.UseVisualStyleBackColor = true;
            this.pulReset.Click += new System.EventHandler(this.pulReset_Click);
            // 
            // cbDisposizioni
            // 
            this.cbDisposizioni.AccessibleDescription = null;
            this.cbDisposizioni.AccessibleName = null;
            resources.ApplyResources(this.cbDisposizioni, "cbDisposizioni");
            this.cbDisposizioni.BackgroundImage = null;
            this.cbDisposizioni.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDisposizioni.Font = null;
            this.cbDisposizioni.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.cbDisposizioni, resources.GetString("cbDisposizioni.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbDisposizioni, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbDisposizioni.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbDisposizioni, null);
            this.cbDisposizioni.Items.AddRange(new object[] {
            resources.GetString("cbDisposizioni.Items")});
            this.cbDisposizioni.Name = "cbDisposizioni";
            this.guidaFile.SetShowHelp(this.cbDisposizioni, ((bool)(resources.GetObject("cbDisposizioni.ShowHelp"))));
            // 
            // etiDisposizione
            // 
            this.etiDisposizione.AccessibleDescription = null;
            this.etiDisposizione.AccessibleName = null;
            resources.ApplyResources(this.etiDisposizione, "etiDisposizione");
            this.etiDisposizione.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiDisposizione, resources.GetString("etiDisposizione.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.etiDisposizione, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiDisposizione.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiDisposizione, null);
            this.etiDisposizione.Name = "etiDisposizione";
            this.guidaFile.SetShowHelp(this.etiDisposizione, ((bool)(resources.GetObject("etiDisposizione.ShowHelp"))));
            // 
            // cbLetture
            // 
            this.cbLetture.AccessibleDescription = null;
            this.cbLetture.AccessibleName = null;
            resources.ApplyResources(this.cbLetture, "cbLetture");
            this.cbLetture.BackgroundImage = null;
            this.cbLetture.Font = null;
            this.guidaFile.SetHelpKeyword(this.cbLetture, resources.GetString("cbLetture.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbLetture, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbLetture.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbLetture, null);
            this.cbLetture.Name = "cbLetture";
            this.guidaFile.SetShowHelp(this.cbLetture, ((bool)(resources.GetObject("cbLetture.ShowHelp"))));
            this.cbLetture.UseVisualStyleBackColor = true;
            // 
            // panClipboard
            // 
            this.panClipboard.AccessibleDescription = null;
            this.panClipboard.AccessibleName = null;
            resources.ApplyResources(this.panClipboard, "panClipboard");
            this.panClipboard.BackgroundImage = null;
            this.panClipboard.Controls.Add(this.labClipboardLunghezza2);
            this.panClipboard.Controls.Add(this.tbClipboardLunghezza);
            this.panClipboard.Controls.Add(this.labClipboardLunghezza1);
            this.panClipboard.Controls.Add(this.labClipboardTempo2);
            this.panClipboard.Controls.Add(this.tbClipboardTempo);
            this.panClipboard.Controls.Add(this.labClipboardTempo1);
            this.panClipboard.Controls.Add(this.cbClipboardAttivo);
            this.panClipboard.Font = null;
            this.guidaFile.SetHelpKeyword(this.panClipboard, null);
            this.guidaFile.SetHelpNavigator(this.panClipboard, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("panClipboard.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.panClipboard, null);
            this.panClipboard.Name = "panClipboard";
            this.guidaFile.SetShowHelp(this.panClipboard, ((bool)(resources.GetObject("panClipboard.ShowHelp"))));
            this.panClipboard.Tag = "panClipboard";
            // 
            // labClipboardLunghezza2
            // 
            this.labClipboardLunghezza2.AccessibleDescription = null;
            this.labClipboardLunghezza2.AccessibleName = null;
            resources.ApplyResources(this.labClipboardLunghezza2, "labClipboardLunghezza2");
            this.labClipboardLunghezza2.Font = null;
            this.guidaFile.SetHelpKeyword(this.labClipboardLunghezza2, resources.GetString("labClipboardLunghezza2.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.labClipboardLunghezza2, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("labClipboardLunghezza2.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.labClipboardLunghezza2, null);
            this.labClipboardLunghezza2.Name = "labClipboardLunghezza2";
            this.guidaFile.SetShowHelp(this.labClipboardLunghezza2, ((bool)(resources.GetObject("labClipboardLunghezza2.ShowHelp"))));
            // 
            // tbClipboardLunghezza
            // 
            this.tbClipboardLunghezza.AccessibleDescription = null;
            this.tbClipboardLunghezza.AccessibleName = null;
            resources.ApplyResources(this.tbClipboardLunghezza, "tbClipboardLunghezza");
            this.tbClipboardLunghezza.BackgroundImage = null;
            this.tbClipboardLunghezza.Font = null;
            this.guidaFile.SetHelpKeyword(this.tbClipboardLunghezza, resources.GetString("tbClipboardLunghezza.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.tbClipboardLunghezza, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("tbClipboardLunghezza.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.tbClipboardLunghezza, null);
            this.tbClipboardLunghezza.Name = "tbClipboardLunghezza";
            this.guidaFile.SetShowHelp(this.tbClipboardLunghezza, ((bool)(resources.GetObject("tbClipboardLunghezza.ShowHelp"))));
            this.tbClipboardLunghezza.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbClipboardLunghezza_KeyDown);
            this.tbClipboardLunghezza.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbClipboardLunghezza_KeyPress);
            // 
            // labClipboardLunghezza1
            // 
            this.labClipboardLunghezza1.AccessibleDescription = null;
            this.labClipboardLunghezza1.AccessibleName = null;
            resources.ApplyResources(this.labClipboardLunghezza1, "labClipboardLunghezza1");
            this.labClipboardLunghezza1.Font = null;
            this.guidaFile.SetHelpKeyword(this.labClipboardLunghezza1, resources.GetString("labClipboardLunghezza1.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.labClipboardLunghezza1, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("labClipboardLunghezza1.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.labClipboardLunghezza1, null);
            this.labClipboardLunghezza1.Name = "labClipboardLunghezza1";
            this.guidaFile.SetShowHelp(this.labClipboardLunghezza1, ((bool)(resources.GetObject("labClipboardLunghezza1.ShowHelp"))));
            // 
            // labClipboardTempo2
            // 
            this.labClipboardTempo2.AccessibleDescription = null;
            this.labClipboardTempo2.AccessibleName = null;
            resources.ApplyResources(this.labClipboardTempo2, "labClipboardTempo2");
            this.labClipboardTempo2.Font = null;
            this.guidaFile.SetHelpKeyword(this.labClipboardTempo2, resources.GetString("labClipboardTempo2.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.labClipboardTempo2, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("labClipboardTempo2.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.labClipboardTempo2, null);
            this.labClipboardTempo2.Name = "labClipboardTempo2";
            this.guidaFile.SetShowHelp(this.labClipboardTempo2, ((bool)(resources.GetObject("labClipboardTempo2.ShowHelp"))));
            // 
            // tbClipboardTempo
            // 
            this.tbClipboardTempo.AccessibleDescription = null;
            this.tbClipboardTempo.AccessibleName = null;
            resources.ApplyResources(this.tbClipboardTempo, "tbClipboardTempo");
            this.tbClipboardTempo.BackgroundImage = null;
            this.tbClipboardTempo.Font = null;
            this.guidaFile.SetHelpKeyword(this.tbClipboardTempo, resources.GetString("tbClipboardTempo.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.tbClipboardTempo, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("tbClipboardTempo.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.tbClipboardTempo, null);
            this.tbClipboardTempo.Name = "tbClipboardTempo";
            this.guidaFile.SetShowHelp(this.tbClipboardTempo, ((bool)(resources.GetObject("tbClipboardTempo.ShowHelp"))));
            this.tbClipboardTempo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbClipboardTempo_KeyDown);
            this.tbClipboardTempo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbClipboardTempo_KeyPress);
            // 
            // labClipboardTempo1
            // 
            this.labClipboardTempo1.AccessibleDescription = null;
            this.labClipboardTempo1.AccessibleName = null;
            resources.ApplyResources(this.labClipboardTempo1, "labClipboardTempo1");
            this.labClipboardTempo1.Font = null;
            this.guidaFile.SetHelpKeyword(this.labClipboardTempo1, resources.GetString("labClipboardTempo1.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.labClipboardTempo1, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("labClipboardTempo1.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.labClipboardTempo1, null);
            this.labClipboardTempo1.Name = "labClipboardTempo1";
            this.guidaFile.SetShowHelp(this.labClipboardTempo1, ((bool)(resources.GetObject("labClipboardTempo1.ShowHelp"))));
            // 
            // cbClipboardAttivo
            // 
            this.cbClipboardAttivo.AccessibleDescription = null;
            this.cbClipboardAttivo.AccessibleName = null;
            resources.ApplyResources(this.cbClipboardAttivo, "cbClipboardAttivo");
            this.cbClipboardAttivo.BackgroundImage = null;
            this.cbClipboardAttivo.Font = null;
            this.guidaFile.SetHelpKeyword(this.cbClipboardAttivo, resources.GetString("cbClipboardAttivo.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbClipboardAttivo, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbClipboardAttivo.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbClipboardAttivo, null);
            this.cbClipboardAttivo.Name = "cbClipboardAttivo";
            this.guidaFile.SetShowHelp(this.cbClipboardAttivo, ((bool)(resources.GetObject("cbClipboardAttivo.ShowHelp"))));
            this.cbClipboardAttivo.UseVisualStyleBackColor = true;
            // 
            // Nome
            // 
            resources.ApplyResources(this.Nome, "Nome");
            this.Nome.Name = "Nome";
            // 
            // AbbUsate
            // 
            resources.ApplyResources(this.AbbUsate, "AbbUsate");
            this.AbbUsate.Name = "AbbUsate";
            // 
            // AbbRicono
            // 
            resources.ApplyResources(this.AbbRicono, "AbbRicono");
            this.AbbRicono.Name = "AbbRicono";
            // 
            // Opzioni
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.tvCategorie);
            this.Controls.Add(this.panClipboard);
            this.Controls.Add(this.panRiferimenti);
            this.Controls.Add(this.panTesto);
            this.Controls.Add(this.panAltre);
            this.Controls.Add(this.panAggiornamenti);
            this.Controls.Add(this.panCaratteri);
            this.Controls.Add(this.panLibri);
            this.Controls.Add(this.panRisultati);
            this.Controls.Add(this.panDizionari);
            this.Controls.Add(this.panTesti);
            this.Controls.Add(this.panInterfaccia);
            this.Font = null;
            this.guidaFile.SetHelpKeyword(this, null);
            this.guidaFile.SetHelpNavigator(this, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("$this.HelpNavigator"))));
            this.guidaFile.SetHelpString(this, null);
            this.Name = "Opzioni";
            this.guidaFile.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.Load += new System.EventHandler(this.Opzioni_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Opzioni_FormClosing);
            this.Controls.SetChildIndex(this.panInterfaccia, 0);
            this.Controls.SetChildIndex(this.panTesti, 0);
            this.Controls.SetChildIndex(this.panDizionari, 0);
            this.Controls.SetChildIndex(this.panRisultati, 0);
            this.Controls.SetChildIndex(this.panLibri, 0);
            this.Controls.SetChildIndex(this.panCaratteri, 0);
            this.Controls.SetChildIndex(this.panAggiornamenti, 0);
            this.Controls.SetChildIndex(this.panAltre, 0);
            this.Controls.SetChildIndex(this.panTesto, 0);
            this.Controls.SetChildIndex(this.panRiferimenti, 0);
            this.Controls.SetChildIndex(this.panClipboard, 0);
            this.Controls.SetChildIndex(this.btnCanc, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.tvCategorie, 0);
            this.panInterfaccia.ResumeLayout(false);
            this.panInterfaccia.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udVociMassimeMenu)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udRichiesteMemorizzate)).EndInit();
            this.panCaratteri.ResumeLayout(false);
            this.panCaratteri.PerformLayout();
            this.panRisultati.ResumeLayout(false);
            this.panRisultati.PerformLayout();
            this.panTesto.ResumeLayout(false);
            this.panTesto.PerformLayout();
            this.gbTesto.ResumeLayout(false);
            this.gbTesto.PerformLayout();
            this.panRiferimenti.ResumeLayout(false);
            this.panRiferimenti.PerformLayout();
            this.gbPosto.ResumeLayout(false);
            this.gbPosto.PerformLayout();
            this.gbFormato.ResumeLayout(false);
            this.gbFormato.PerformLayout();
            this.gbRifTipo.ResumeLayout(false);
            this.gbRifTipo.PerformLayout();
            this.panLibri.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridLibri)).EndInit();
            this.panTesti.ResumeLayout(false);
            this.panTesti.PerformLayout();
            this.panAggiornamenti.ResumeLayout(false);
            this.panAggiornamenti.PerformLayout();
            this.panDizionari.ResumeLayout(false);
            this.panDizionari.PerformLayout();
            this.panAltre.ResumeLayout(false);
            this.panAltre.PerformLayout();
            this.panClipboard.ResumeLayout(false);
            this.panClipboard.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvCategorie;
        private System.Windows.Forms.Panel panInterfaccia;
        private System.Windows.Forms.Label labLingua;
        private System.Windows.Forms.ComboBox cbLingua;
        private System.Windows.Forms.Panel panCaratteri;
        private System.Windows.Forms.CheckBox cbRifApice;
        private System.Windows.Forms.Button btnFontRif;
        private System.Windows.Forms.Label labFontRiferimento;
        private System.Windows.Forms.Button btnFont;
        private System.Windows.Forms.Label labFontPredefTesto;
        private System.Windows.Forms.Label labFontPredef;
        private System.Windows.Forms.Label labFontRifTesto;
        private System.Windows.Forms.Panel panRisultati;
        private System.Windows.Forms.Label labEsempio;
        private System.Windows.Forms.Panel panTesto;
        private System.Windows.Forms.Panel panRiferimenti;
        private System.Windows.Forms.GroupBox gbRifTipo;
        private System.Windows.Forms.RadioButton rbRifTipo2;
        private System.Windows.Forms.RadioButton rbRifTipo1;
        private System.Windows.Forms.RadioButton rbRifTipo0;
        private System.Windows.Forms.GroupBox gbPosto;
        private System.Windows.Forms.GroupBox gbFormato;
        private System.Windows.Forms.RadioButton rbRifPosto2;
        private System.Windows.Forms.RadioButton rbRifPosto1;
        private System.Windows.Forms.RadioButton rbRifPosto0;
        private System.Windows.Forms.RadioButton rbRifFormato2;
        private System.Windows.Forms.RadioButton rbRifFormato1;
        private System.Windows.Forms.RadioButton rbRifFormato0;
        private System.Windows.Forms.GroupBox gbTesto;
        private System.Windows.Forms.RadioButton rbTesto2;
        private System.Windows.Forms.RadioButton rbTesto1;
        private System.Windows.Forms.RadioButton rbTesto0;
        private System.Windows.Forms.Panel panLibri;
        private System.Windows.Forms.Button btnLibriInglese;
        private System.Windows.Forms.Button btnLibriItaliano;
        private System.Windows.Forms.DataGridView gridLibri;
        private System.Windows.Forms.Label labRichiesteMemorizzate;
        private System.Windows.Forms.NumericUpDown udRichiesteMemorizzate;
        private System.Windows.Forms.Button btnFontRicerca;
        private System.Windows.Forms.Label labFontRicercaTesto;
        private System.Windows.Forms.Label labFontRicerca;
        private System.Windows.Forms.CheckBox cbBarraDiStato;
        private System.Windows.Forms.CheckBox cbStessaFinestraPerRisultati;
        private System.Windows.Forms.Panel panTesti;
        private System.Windows.Forms.CheckedListBox clbCommentari;
        private System.Windows.Forms.Label labCommentari;
        private System.Windows.Forms.CheckBox cbIpertestoTooltip;
        private System.Windows.Forms.Label labBarraDiStrumenti;
        private System.Windows.Forms.CheckBox cbBSRigaComando;
        private System.Windows.Forms.CheckBox cbBSOrdine;
        private System.Windows.Forms.CheckBox cbBSFormato;
        private System.Windows.Forms.CheckBox cbBSPrincipale;
        private System.Windows.Forms.ListBox lbCartelle;
        private System.Windows.Forms.Label labCartelle;
        private System.Windows.Forms.Button pulCancellaCartella;
        private System.Windows.Forms.Button pulAggiungiCartella;
        private System.Windows.Forms.Label labFontEbraico;
        private System.Windows.Forms.Label labFontGreco;
        private System.Windows.Forms.Label labFontEbraicoTesto;
        private System.Windows.Forms.Label labFontGrecoTesto;
        private System.Windows.Forms.Button btnFontEbraico;
        private System.Windows.Forms.Button btnFontGreco;
        private System.Windows.Forms.Panel panAggiornamenti;
        private System.Windows.Forms.ComboBox cbAggiornaGiorni;
        private System.Windows.Forms.RadioButton rbAggiornaAutomatica;
        private System.Windows.Forms.RadioButton rbAggiornaManuale;
        private RichTextBoxEx rtEsempio;
        private System.Windows.Forms.Panel panDizionari;
        private System.Windows.Forms.ComboBox cbDizionariEbraici;
        private System.Windows.Forms.Label labDizionarioEbraico;
        private System.Windows.Forms.Label labDizionarioGreco;
        private System.Windows.Forms.Label labDizionarioItaliano;
        private System.Windows.Forms.Label labDizionarioInglese;
        private System.Windows.Forms.CheckBox cbDizionarioTooltip;
        private System.Windows.Forms.ComboBox cbDizionariGreci;
        private System.Windows.Forms.ComboBox cbDizionariItaliani;
        private System.Windows.Forms.ComboBox cbDizionariInglesi;
        private System.Windows.Forms.TextBox tbProxyPorta;
        private System.Windows.Forms.Label etiProxyPort;
        private System.Windows.Forms.TextBox tbProxy;
        private System.Windows.Forms.Label etiProxy;
        private System.Windows.Forms.Label etiProxyPassword;
        private System.Windows.Forms.TextBox tbProxyNomeUtente;
        private System.Windows.Forms.Label etiProxyNomeUtente;
        private System.Windows.Forms.TextBox tbProxyDominio;
        private System.Windows.Forms.Label etiProxyDominio;
        private System.Windows.Forms.TextBox tbProxyPassword;
        private System.Windows.Forms.Panel panAltre;
        private System.Windows.Forms.CheckBox cbLetture;
        private System.Windows.Forms.ComboBox cbDisposizioni;
        private System.Windows.Forms.Label etiDisposizione;
        private System.Windows.Forms.Label labDizionarioLatino;
        private System.Windows.Forms.ComboBox cbDizionariLatini;
        private System.Windows.Forms.Button pulReset;
        private System.Windows.Forms.Button pulReload;
        private System.Windows.Forms.Panel panClipboard;
        private System.Windows.Forms.CheckBox cbClipboardAttivo;
        private System.Windows.Forms.TextBox tbClipboardTempo;
        private System.Windows.Forms.Label labClipboardTempo1;
        private System.Windows.Forms.Label labClipboardLunghezza2;
        private System.Windows.Forms.TextBox tbClipboardLunghezza;
        private System.Windows.Forms.Label labClipboardLunghezza1;
        private System.Windows.Forms.Label labClipboardTempo2;
        private System.Windows.Forms.NumericUpDown udVociMassimeMenu;
        private System.Windows.Forms.Label labVociMassimeMenu;
        private System.Windows.Forms.CheckBox cbIpertestoTooltipInTooltip;
        private System.Windows.Forms.CheckBox cbTitoli;
        private System.Windows.Forms.CheckBox cbRifContestoRicerche;
        private System.Windows.Forms.Button btnLibriSpagnolo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Nome;
        private System.Windows.Forms.DataGridViewTextBoxColumn AbbUsate;
        private System.Windows.Forms.DataGridViewTextBoxColumn AbbRicono;
    }
}