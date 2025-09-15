namespace LaParola
{
    partial class Collegamenti
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Collegamenti));
            this.lbCollegamenti = new System.Windows.Forms.ListBox();
            this.etiNome = new System.Windows.Forms.Label();
            this.txtNome = new System.Windows.Forms.TextBox();
            this.etiDescrizione = new System.Windows.Forms.Label();
            this.txtDescrizione = new System.Windows.Forms.TextBox();
            this.etiIndirizzo = new System.Windows.Forms.Label();
            this.txtIndirizzo = new System.Windows.Forms.TextBox();
            this.etiImmagine = new System.Windows.Forms.Label();
            this.etiCategoria = new System.Windows.Forms.Label();
            this.txtImmagine = new System.Windows.Forms.TextBox();
            this.txtCategoria = new System.Windows.Forms.TextBox();
            this.gbTipo = new System.Windows.Forms.GroupBox();
            this.etiLingua = new System.Windows.Forms.Label();
            this.txtLingua = new System.Windows.Forms.TextBox();
            this.rbTipoParola = new System.Windows.Forms.RadioButton();
            this.rbTipoRiferimento = new System.Windows.Forms.RadioButton();
            this.pulNuovo = new System.Windows.Forms.Button();
            this.pulCancella = new System.Windows.Forms.Button();
            this.pulSalva = new System.Windows.Forms.Button();
            this.etiParametri = new System.Windows.Forms.Label();
            this.txtParametri = new System.Windows.Forms.TextBox();
            this.etiScorciatoia = new System.Windows.Forms.Label();
            this.txtScorciatoia = new System.Windows.Forms.TextBox();
            this.gbTipo.SuspendLayout();
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
            // lbCollegamenti
            // 
            this.lbCollegamenti.AccessibleDescription = null;
            this.lbCollegamenti.AccessibleName = null;
            resources.ApplyResources(this.lbCollegamenti, "lbCollegamenti");
            this.lbCollegamenti.BackgroundImage = null;
            this.lbCollegamenti.Font = null;
            this.lbCollegamenti.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.lbCollegamenti, resources.GetString("lbCollegamenti.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.lbCollegamenti, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("lbCollegamenti.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.lbCollegamenti, null);
            this.lbCollegamenti.Name = "lbCollegamenti";
            this.guidaFile.SetShowHelp(this.lbCollegamenti, ((bool)(resources.GetObject("lbCollegamenti.ShowHelp"))));
            this.lbCollegamenti.SelectedIndexChanged += new System.EventHandler(this.lbCollegamenti_SelectedIndexChanged);
            // 
            // etiNome
            // 
            this.etiNome.AccessibleDescription = null;
            this.etiNome.AccessibleName = null;
            resources.ApplyResources(this.etiNome, "etiNome");
            this.etiNome.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiNome, resources.GetString("etiNome.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.etiNome, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiNome.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiNome, null);
            this.etiNome.Name = "etiNome";
            this.guidaFile.SetShowHelp(this.etiNome, ((bool)(resources.GetObject("etiNome.ShowHelp"))));
            // 
            // txtNome
            // 
            this.txtNome.AccessibleDescription = null;
            this.txtNome.AccessibleName = null;
            resources.ApplyResources(this.txtNome, "txtNome");
            this.txtNome.BackgroundImage = null;
            this.txtNome.Font = null;
            this.guidaFile.SetHelpKeyword(this.txtNome, resources.GetString("txtNome.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.txtNome, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("txtNome.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.txtNome, null);
            this.txtNome.Name = "txtNome";
            this.guidaFile.SetShowHelp(this.txtNome, ((bool)(resources.GetObject("txtNome.ShowHelp"))));
            this.txtNome.Tag = "Nuovo collegamento";
            this.txtNome.TextChanged += new System.EventHandler(this.txt_TextChanged);
            // 
            // etiDescrizione
            // 
            this.etiDescrizione.AccessibleDescription = null;
            this.etiDescrizione.AccessibleName = null;
            resources.ApplyResources(this.etiDescrizione, "etiDescrizione");
            this.etiDescrizione.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiDescrizione, resources.GetString("etiDescrizione.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.etiDescrizione, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiDescrizione.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiDescrizione, null);
            this.etiDescrizione.Name = "etiDescrizione";
            this.guidaFile.SetShowHelp(this.etiDescrizione, ((bool)(resources.GetObject("etiDescrizione.ShowHelp"))));
            // 
            // txtDescrizione
            // 
            this.txtDescrizione.AccessibleDescription = null;
            this.txtDescrizione.AccessibleName = null;
            resources.ApplyResources(this.txtDescrizione, "txtDescrizione");
            this.txtDescrizione.BackgroundImage = null;
            this.txtDescrizione.Font = null;
            this.guidaFile.SetHelpKeyword(this.txtDescrizione, resources.GetString("txtDescrizione.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.txtDescrizione, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("txtDescrizione.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.txtDescrizione, null);
            this.txtDescrizione.Name = "txtDescrizione";
            this.guidaFile.SetShowHelp(this.txtDescrizione, ((bool)(resources.GetObject("txtDescrizione.ShowHelp"))));
            this.txtDescrizione.TextChanged += new System.EventHandler(this.txt_TextChanged);
            // 
            // etiIndirizzo
            // 
            this.etiIndirizzo.AccessibleDescription = null;
            this.etiIndirizzo.AccessibleName = null;
            resources.ApplyResources(this.etiIndirizzo, "etiIndirizzo");
            this.etiIndirizzo.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiIndirizzo, resources.GetString("etiIndirizzo.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.etiIndirizzo, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiIndirizzo.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiIndirizzo, null);
            this.etiIndirizzo.Name = "etiIndirizzo";
            this.guidaFile.SetShowHelp(this.etiIndirizzo, ((bool)(resources.GetObject("etiIndirizzo.ShowHelp"))));
            // 
            // txtIndirizzo
            // 
            this.txtIndirizzo.AccessibleDescription = null;
            this.txtIndirizzo.AccessibleName = null;
            resources.ApplyResources(this.txtIndirizzo, "txtIndirizzo");
            this.txtIndirizzo.BackgroundImage = null;
            this.txtIndirizzo.Font = null;
            this.guidaFile.SetHelpKeyword(this.txtIndirizzo, resources.GetString("txtIndirizzo.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.txtIndirizzo, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("txtIndirizzo.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.txtIndirizzo, null);
            this.txtIndirizzo.Name = "txtIndirizzo";
            this.guidaFile.SetShowHelp(this.txtIndirizzo, ((bool)(resources.GetObject("txtIndirizzo.ShowHelp"))));
            this.txtIndirizzo.TextChanged += new System.EventHandler(this.txt_TextChanged);
            // 
            // etiImmagine
            // 
            this.etiImmagine.AccessibleDescription = null;
            this.etiImmagine.AccessibleName = null;
            resources.ApplyResources(this.etiImmagine, "etiImmagine");
            this.etiImmagine.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiImmagine, resources.GetString("etiImmagine.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.etiImmagine, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiImmagine.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiImmagine, null);
            this.etiImmagine.Name = "etiImmagine";
            this.guidaFile.SetShowHelp(this.etiImmagine, ((bool)(resources.GetObject("etiImmagine.ShowHelp"))));
            // 
            // etiCategoria
            // 
            this.etiCategoria.AccessibleDescription = null;
            this.etiCategoria.AccessibleName = null;
            resources.ApplyResources(this.etiCategoria, "etiCategoria");
            this.etiCategoria.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiCategoria, resources.GetString("etiCategoria.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.etiCategoria, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiCategoria.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiCategoria, null);
            this.etiCategoria.Name = "etiCategoria";
            this.guidaFile.SetShowHelp(this.etiCategoria, ((bool)(resources.GetObject("etiCategoria.ShowHelp"))));
            // 
            // txtImmagine
            // 
            this.txtImmagine.AccessibleDescription = null;
            this.txtImmagine.AccessibleName = null;
            resources.ApplyResources(this.txtImmagine, "txtImmagine");
            this.txtImmagine.BackgroundImage = null;
            this.txtImmagine.Font = null;
            this.guidaFile.SetHelpKeyword(this.txtImmagine, resources.GetString("txtImmagine.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.txtImmagine, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("txtImmagine.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.txtImmagine, null);
            this.txtImmagine.Name = "txtImmagine";
            this.guidaFile.SetShowHelp(this.txtImmagine, ((bool)(resources.GetObject("txtImmagine.ShowHelp"))));
            this.txtImmagine.TextChanged += new System.EventHandler(this.txt_TextChanged);
            // 
            // txtCategoria
            // 
            this.txtCategoria.AccessibleDescription = null;
            this.txtCategoria.AccessibleName = null;
            resources.ApplyResources(this.txtCategoria, "txtCategoria");
            this.txtCategoria.BackgroundImage = null;
            this.txtCategoria.Font = null;
            this.guidaFile.SetHelpKeyword(this.txtCategoria, resources.GetString("txtCategoria.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.txtCategoria, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("txtCategoria.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.txtCategoria, null);
            this.txtCategoria.Name = "txtCategoria";
            this.guidaFile.SetShowHelp(this.txtCategoria, ((bool)(resources.GetObject("txtCategoria.ShowHelp"))));
            this.txtCategoria.TextChanged += new System.EventHandler(this.txt_TextChanged);
            // 
            // gbTipo
            // 
            this.gbTipo.AccessibleDescription = null;
            this.gbTipo.AccessibleName = null;
            resources.ApplyResources(this.gbTipo, "gbTipo");
            this.gbTipo.BackgroundImage = null;
            this.gbTipo.Controls.Add(this.etiLingua);
            this.gbTipo.Controls.Add(this.txtLingua);
            this.gbTipo.Controls.Add(this.rbTipoParola);
            this.gbTipo.Controls.Add(this.rbTipoRiferimento);
            this.gbTipo.Font = null;
            this.guidaFile.SetHelpKeyword(this.gbTipo, resources.GetString("gbTipo.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.gbTipo, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("gbTipo.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.gbTipo, null);
            this.gbTipo.Name = "gbTipo";
            this.guidaFile.SetShowHelp(this.gbTipo, ((bool)(resources.GetObject("gbTipo.ShowHelp"))));
            this.gbTipo.TabStop = false;
            // 
            // etiLingua
            // 
            this.etiLingua.AccessibleDescription = null;
            this.etiLingua.AccessibleName = null;
            resources.ApplyResources(this.etiLingua, "etiLingua");
            this.etiLingua.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiLingua, resources.GetString("etiLingua.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.etiLingua, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiLingua.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiLingua, null);
            this.etiLingua.Name = "etiLingua";
            this.guidaFile.SetShowHelp(this.etiLingua, ((bool)(resources.GetObject("etiLingua.ShowHelp"))));
            // 
            // txtLingua
            // 
            this.txtLingua.AccessibleDescription = null;
            this.txtLingua.AccessibleName = null;
            resources.ApplyResources(this.txtLingua, "txtLingua");
            this.txtLingua.BackgroundImage = null;
            this.txtLingua.Font = null;
            this.guidaFile.SetHelpKeyword(this.txtLingua, resources.GetString("txtLingua.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.txtLingua, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("txtLingua.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.txtLingua, null);
            this.txtLingua.Name = "txtLingua";
            this.guidaFile.SetShowHelp(this.txtLingua, ((bool)(resources.GetObject("txtLingua.ShowHelp"))));
            this.txtLingua.TextChanged += new System.EventHandler(this.txt_TextChanged);
            // 
            // rbTipoParola
            // 
            this.rbTipoParola.AccessibleDescription = null;
            this.rbTipoParola.AccessibleName = null;
            resources.ApplyResources(this.rbTipoParola, "rbTipoParola");
            this.rbTipoParola.BackgroundImage = null;
            this.rbTipoParola.Font = null;
            this.guidaFile.SetHelpKeyword(this.rbTipoParola, resources.GetString("rbTipoParola.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.rbTipoParola, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("rbTipoParola.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.rbTipoParola, null);
            this.rbTipoParola.Name = "rbTipoParola";
            this.guidaFile.SetShowHelp(this.rbTipoParola, ((bool)(resources.GetObject("rbTipoParola.ShowHelp"))));
            this.rbTipoParola.Tag = "parola";
            this.rbTipoParola.UseVisualStyleBackColor = true;
            this.rbTipoParola.CheckedChanged += new System.EventHandler(this.rbTipo_CheckedChanged);
            // 
            // rbTipoRiferimento
            // 
            this.rbTipoRiferimento.AccessibleDescription = null;
            this.rbTipoRiferimento.AccessibleName = null;
            resources.ApplyResources(this.rbTipoRiferimento, "rbTipoRiferimento");
            this.rbTipoRiferimento.BackgroundImage = null;
            this.rbTipoRiferimento.Checked = true;
            this.rbTipoRiferimento.Font = null;
            this.guidaFile.SetHelpKeyword(this.rbTipoRiferimento, resources.GetString("rbTipoRiferimento.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.rbTipoRiferimento, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("rbTipoRiferimento.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.rbTipoRiferimento, null);
            this.rbTipoRiferimento.Name = "rbTipoRiferimento";
            this.guidaFile.SetShowHelp(this.rbTipoRiferimento, ((bool)(resources.GetObject("rbTipoRiferimento.ShowHelp"))));
            this.rbTipoRiferimento.TabStop = true;
            this.rbTipoRiferimento.Tag = "riferimento";
            this.rbTipoRiferimento.UseVisualStyleBackColor = true;
            this.rbTipoRiferimento.CheckedChanged += new System.EventHandler(this.rbTipo_CheckedChanged);
            // 
            // pulNuovo
            // 
            this.pulNuovo.AccessibleDescription = null;
            this.pulNuovo.AccessibleName = null;
            resources.ApplyResources(this.pulNuovo, "pulNuovo");
            this.pulNuovo.BackgroundImage = null;
            this.pulNuovo.Font = null;
            this.guidaFile.SetHelpKeyword(this.pulNuovo, resources.GetString("pulNuovo.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.pulNuovo, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulNuovo.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.pulNuovo, null);
            this.pulNuovo.Name = "pulNuovo";
            this.guidaFile.SetShowHelp(this.pulNuovo, ((bool)(resources.GetObject("pulNuovo.ShowHelp"))));
            this.pulNuovo.UseVisualStyleBackColor = true;
            this.pulNuovo.Click += new System.EventHandler(this.pulNuovo_Click);
            // 
            // pulCancella
            // 
            this.pulCancella.AccessibleDescription = null;
            this.pulCancella.AccessibleName = null;
            resources.ApplyResources(this.pulCancella, "pulCancella");
            this.pulCancella.BackgroundImage = null;
            this.pulCancella.Font = null;
            this.guidaFile.SetHelpKeyword(this.pulCancella, resources.GetString("pulCancella.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.pulCancella, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulCancella.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.pulCancella, null);
            this.pulCancella.Name = "pulCancella";
            this.guidaFile.SetShowHelp(this.pulCancella, ((bool)(resources.GetObject("pulCancella.ShowHelp"))));
            this.pulCancella.UseVisualStyleBackColor = true;
            this.pulCancella.Click += new System.EventHandler(this.pulCancella_Click);
            // 
            // pulSalva
            // 
            this.pulSalva.AccessibleDescription = null;
            this.pulSalva.AccessibleName = null;
            resources.ApplyResources(this.pulSalva, "pulSalva");
            this.pulSalva.BackgroundImage = null;
            this.pulSalva.Font = null;
            this.guidaFile.SetHelpKeyword(this.pulSalva, resources.GetString("pulSalva.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.pulSalva, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulSalva.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.pulSalva, null);
            this.pulSalva.Name = "pulSalva";
            this.guidaFile.SetShowHelp(this.pulSalva, ((bool)(resources.GetObject("pulSalva.ShowHelp"))));
            this.pulSalva.UseVisualStyleBackColor = true;
            this.pulSalva.Click += new System.EventHandler(this.pulSalva_Click);
            // 
            // etiParametri
            // 
            this.etiParametri.AccessibleDescription = null;
            this.etiParametri.AccessibleName = null;
            resources.ApplyResources(this.etiParametri, "etiParametri");
            this.etiParametri.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiParametri, resources.GetString("etiParametri.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.etiParametri, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiParametri.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiParametri, null);
            this.etiParametri.Name = "etiParametri";
            this.guidaFile.SetShowHelp(this.etiParametri, ((bool)(resources.GetObject("etiParametri.ShowHelp"))));
            // 
            // txtParametri
            // 
            this.txtParametri.AccessibleDescription = null;
            this.txtParametri.AccessibleName = null;
            resources.ApplyResources(this.txtParametri, "txtParametri");
            this.txtParametri.BackgroundImage = null;
            this.txtParametri.Font = null;
            this.guidaFile.SetHelpKeyword(this.txtParametri, resources.GetString("txtParametri.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.txtParametri, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("txtParametri.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.txtParametri, null);
            this.txtParametri.Name = "txtParametri";
            this.guidaFile.SetShowHelp(this.txtParametri, ((bool)(resources.GetObject("txtParametri.ShowHelp"))));
            this.txtParametri.TextChanged += new System.EventHandler(this.txt_TextChanged);
            // 
            // etiScorciatoia
            // 
            this.etiScorciatoia.AccessibleDescription = null;
            this.etiScorciatoia.AccessibleName = null;
            resources.ApplyResources(this.etiScorciatoia, "etiScorciatoia");
            this.etiScorciatoia.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiScorciatoia, resources.GetString("etiScorciatoia.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.etiScorciatoia, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiScorciatoia.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiScorciatoia, null);
            this.etiScorciatoia.Name = "etiScorciatoia";
            this.guidaFile.SetShowHelp(this.etiScorciatoia, ((bool)(resources.GetObject("etiScorciatoia.ShowHelp"))));
            // 
            // txtScorciatoia
            // 
            this.txtScorciatoia.AccessibleDescription = null;
            this.txtScorciatoia.AccessibleName = null;
            resources.ApplyResources(this.txtScorciatoia, "txtScorciatoia");
            this.txtScorciatoia.BackgroundImage = null;
            this.txtScorciatoia.Font = null;
            this.guidaFile.SetHelpKeyword(this.txtScorciatoia, resources.GetString("txtScorciatoia.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.txtScorciatoia, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("txtScorciatoia.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.txtScorciatoia, null);
            this.txtScorciatoia.Name = "txtScorciatoia";
            this.guidaFile.SetShowHelp(this.txtScorciatoia, ((bool)(resources.GetObject("txtScorciatoia.ShowHelp"))));
            this.txtScorciatoia.TextChanged += new System.EventHandler(this.txt_TextChanged);
            // 
            // Collegamenti
            // 
            this.AcceptButton = this.btnCanc;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.etiScorciatoia);
            this.Controls.Add(this.txtParametri);
            this.Controls.Add(this.txtScorciatoia);
            this.Controls.Add(this.etiParametri);
            this.Controls.Add(this.pulCancella);
            this.Controls.Add(this.gbTipo);
            this.Controls.Add(this.pulSalva);
            this.Controls.Add(this.pulNuovo);
            this.Controls.Add(this.txtImmagine);
            this.Controls.Add(this.txtCategoria);
            this.Controls.Add(this.etiCategoria);
            this.Controls.Add(this.txtDescrizione);
            this.Controls.Add(this.etiImmagine);
            this.Controls.Add(this.lbCollegamenti);
            this.Controls.Add(this.etiIndirizzo);
            this.Controls.Add(this.etiNome);
            this.Controls.Add(this.etiDescrizione);
            this.Controls.Add(this.txtIndirizzo);
            this.Controls.Add(this.txtNome);
            this.Font = null;
            this.guidaFile.SetHelpKeyword(this, null);
            this.guidaFile.SetHelpNavigator(this, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("$this.HelpNavigator"))));
            this.guidaFile.SetHelpString(this, null);
            this.Name = "Collegamenti";
            this.guidaFile.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.Controls.SetChildIndex(this.txtNome, 0);
            this.Controls.SetChildIndex(this.txtIndirizzo, 0);
            this.Controls.SetChildIndex(this.etiDescrizione, 0);
            this.Controls.SetChildIndex(this.etiNome, 0);
            this.Controls.SetChildIndex(this.etiIndirizzo, 0);
            this.Controls.SetChildIndex(this.lbCollegamenti, 0);
            this.Controls.SetChildIndex(this.etiImmagine, 0);
            this.Controls.SetChildIndex(this.txtDescrizione, 0);
            this.Controls.SetChildIndex(this.etiCategoria, 0);
            this.Controls.SetChildIndex(this.txtCategoria, 0);
            this.Controls.SetChildIndex(this.txtImmagine, 0);
            this.Controls.SetChildIndex(this.pulNuovo, 0);
            this.Controls.SetChildIndex(this.pulSalva, 0);
            this.Controls.SetChildIndex(this.gbTipo, 0);
            this.Controls.SetChildIndex(this.pulCancella, 0);
            this.Controls.SetChildIndex(this.etiParametri, 0);
            this.Controls.SetChildIndex(this.txtScorciatoia, 0);
            this.Controls.SetChildIndex(this.txtParametri, 0);
            this.Controls.SetChildIndex(this.etiScorciatoia, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnCanc, 0);
            this.gbTipo.ResumeLayout(false);
            this.gbTipo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbCollegamenti;
        private System.Windows.Forms.Label etiNome;
        private System.Windows.Forms.TextBox txtNome;
        private System.Windows.Forms.Label etiDescrizione;
        private System.Windows.Forms.TextBox txtDescrizione;
        private System.Windows.Forms.Label etiIndirizzo;
        private System.Windows.Forms.TextBox txtIndirizzo;
        private System.Windows.Forms.Label etiImmagine;
        private System.Windows.Forms.Label etiCategoria;
        private System.Windows.Forms.TextBox txtImmagine;
        private System.Windows.Forms.TextBox txtCategoria;
        private System.Windows.Forms.GroupBox gbTipo;
        private System.Windows.Forms.RadioButton rbTipoParola;
        private System.Windows.Forms.RadioButton rbTipoRiferimento;
        private System.Windows.Forms.Label etiLingua;
        private System.Windows.Forms.TextBox txtLingua;
        private System.Windows.Forms.Button pulNuovo;
        private System.Windows.Forms.Button pulCancella;
        private System.Windows.Forms.Button pulSalva;
        private System.Windows.Forms.Label etiParametri;
        private System.Windows.Forms.TextBox txtParametri;
        private System.Windows.Forms.Label etiScorciatoia;
        private System.Windows.Forms.TextBox txtScorciatoia;
    }
}