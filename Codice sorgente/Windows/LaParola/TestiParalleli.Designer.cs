namespace LaParola
{
    partial class TestiParalleli
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestiParalleli));
            this.lbBibbie = new System.Windows.Forms.ListBox();
            this.lbCommentari = new System.Windows.Forms.ListBox();
            this.lbScelti = new System.Windows.Forms.ListBox();
            this.etiBibbie = new System.Windows.Forms.Label();
            this.etiCommentari = new System.Windows.Forms.Label();
            this.etiScelti = new System.Windows.Forms.Label();
            this.pulSalva = new System.Windows.Forms.Button();
            this.pulApri = new System.Windows.Forms.Button();
            this.pulAggiungiBibbia = new System.Windows.Forms.Button();
            this.pulAggiungiCommentario = new System.Windows.Forms.Button();
            this.pulRimuovi = new System.Windows.Forms.Button();
            this.pulSu = new System.Windows.Forms.Button();
            this.pulGiu = new System.Windows.Forms.Button();
            this.pmApri = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.lbDizionari = new System.Windows.Forms.ListBox();
            this.etiDizionari = new System.Windows.Forms.Label();
            this.pulAggiungiDizionario = new System.Windows.Forms.Button();
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
            // lbBibbie
            // 
            this.lbBibbie.AccessibleDescription = null;
            this.lbBibbie.AccessibleName = null;
            resources.ApplyResources(this.lbBibbie, "lbBibbie");
            this.lbBibbie.BackgroundImage = null;
            this.lbBibbie.Font = null;
            this.lbBibbie.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.lbBibbie, resources.GetString("lbBibbie.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.lbBibbie, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("lbBibbie.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.lbBibbie, null);
            this.lbBibbie.Name = "lbBibbie";
            this.guidaFile.SetShowHelp(this.lbBibbie, ((bool)(resources.GetObject("lbBibbie.ShowHelp"))));
            this.lbBibbie.Sorted = true;
            this.lbBibbie.SelectedIndexChanged += new System.EventHandler(this.lbBibbie_SelectedIndexChanged);
            // 
            // lbCommentari
            // 
            this.lbCommentari.AccessibleDescription = null;
            this.lbCommentari.AccessibleName = null;
            resources.ApplyResources(this.lbCommentari, "lbCommentari");
            this.lbCommentari.BackgroundImage = null;
            this.lbCommentari.Font = null;
            this.lbCommentari.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.lbCommentari, resources.GetString("lbCommentari.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.lbCommentari, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("lbCommentari.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.lbCommentari, null);
            this.lbCommentari.Name = "lbCommentari";
            this.guidaFile.SetShowHelp(this.lbCommentari, ((bool)(resources.GetObject("lbCommentari.ShowHelp"))));
            this.lbCommentari.Sorted = true;
            this.lbCommentari.SelectedIndexChanged += new System.EventHandler(this.lbCommentari_SelectedIndexChanged);
            // 
            // lbScelti
            // 
            this.lbScelti.AccessibleDescription = null;
            this.lbScelti.AccessibleName = null;
            resources.ApplyResources(this.lbScelti, "lbScelti");
            this.lbScelti.BackgroundImage = null;
            this.lbScelti.Font = null;
            this.lbScelti.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.lbScelti, resources.GetString("lbScelti.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.lbScelti, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("lbScelti.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.lbScelti, null);
            this.lbScelti.Name = "lbScelti";
            this.guidaFile.SetShowHelp(this.lbScelti, ((bool)(resources.GetObject("lbScelti.ShowHelp"))));
            this.lbScelti.SelectedIndexChanged += new System.EventHandler(this.lbScelti_SelectedIndexChanged);
            // 
            // etiBibbie
            // 
            this.etiBibbie.AccessibleDescription = null;
            this.etiBibbie.AccessibleName = null;
            resources.ApplyResources(this.etiBibbie, "etiBibbie");
            this.etiBibbie.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiBibbie, resources.GetString("etiBibbie.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.etiBibbie, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiBibbie.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiBibbie, null);
            this.etiBibbie.Name = "etiBibbie";
            this.guidaFile.SetShowHelp(this.etiBibbie, ((bool)(resources.GetObject("etiBibbie.ShowHelp"))));
            // 
            // etiCommentari
            // 
            this.etiCommentari.AccessibleDescription = null;
            this.etiCommentari.AccessibleName = null;
            resources.ApplyResources(this.etiCommentari, "etiCommentari");
            this.etiCommentari.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiCommentari, resources.GetString("etiCommentari.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.etiCommentari, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiCommentari.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiCommentari, null);
            this.etiCommentari.Name = "etiCommentari";
            this.guidaFile.SetShowHelp(this.etiCommentari, ((bool)(resources.GetObject("etiCommentari.ShowHelp"))));
            // 
            // etiScelti
            // 
            this.etiScelti.AccessibleDescription = null;
            this.etiScelti.AccessibleName = null;
            resources.ApplyResources(this.etiScelti, "etiScelti");
            this.etiScelti.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiScelti, resources.GetString("etiScelti.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.etiScelti, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiScelti.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiScelti, null);
            this.etiScelti.Name = "etiScelti";
            this.guidaFile.SetShowHelp(this.etiScelti, ((bool)(resources.GetObject("etiScelti.ShowHelp"))));
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
            this.pulSalva.Image = global::LaParola.Properties.Resources.salva;
            this.pulSalva.Name = "pulSalva";
            this.guidaFile.SetShowHelp(this.pulSalva, ((bool)(resources.GetObject("pulSalva.ShowHelp"))));
            this.pulSalva.UseVisualStyleBackColor = true;
            this.pulSalva.Click += new System.EventHandler(this.pulSalva_Click);
            // 
            // pulApri
            // 
            this.pulApri.AccessibleDescription = null;
            this.pulApri.AccessibleName = null;
            resources.ApplyResources(this.pulApri, "pulApri");
            this.pulApri.BackgroundImage = null;
            this.pulApri.Font = null;
            this.guidaFile.SetHelpKeyword(this.pulApri, resources.GetString("pulApri.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.pulApri, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulApri.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.pulApri, null);
            this.pulApri.Image = global::LaParola.Properties.Resources.apri;
            this.pulApri.Name = "pulApri";
            this.guidaFile.SetShowHelp(this.pulApri, ((bool)(resources.GetObject("pulApri.ShowHelp"))));
            this.pulApri.UseVisualStyleBackColor = true;
            this.pulApri.Click += new System.EventHandler(this.pulApri_Click);
            // 
            // pulAggiungiBibbia
            // 
            this.pulAggiungiBibbia.AccessibleDescription = null;
            this.pulAggiungiBibbia.AccessibleName = null;
            resources.ApplyResources(this.pulAggiungiBibbia, "pulAggiungiBibbia");
            this.pulAggiungiBibbia.BackgroundImage = null;
            this.pulAggiungiBibbia.Font = null;
            this.guidaFile.SetHelpKeyword(this.pulAggiungiBibbia, resources.GetString("pulAggiungiBibbia.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.pulAggiungiBibbia, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulAggiungiBibbia.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.pulAggiungiBibbia, null);
            this.pulAggiungiBibbia.Image = global::LaParola.Properties.Resources.ordineprossimo;
            this.pulAggiungiBibbia.Name = "pulAggiungiBibbia";
            this.guidaFile.SetShowHelp(this.pulAggiungiBibbia, ((bool)(resources.GetObject("pulAggiungiBibbia.ShowHelp"))));
            this.pulAggiungiBibbia.UseVisualStyleBackColor = true;
            this.pulAggiungiBibbia.Click += new System.EventHandler(this.pulAggiungiBibbia_Click);
            // 
            // pulAggiungiCommentario
            // 
            this.pulAggiungiCommentario.AccessibleDescription = null;
            this.pulAggiungiCommentario.AccessibleName = null;
            resources.ApplyResources(this.pulAggiungiCommentario, "pulAggiungiCommentario");
            this.pulAggiungiCommentario.BackgroundImage = null;
            this.pulAggiungiCommentario.Font = null;
            this.guidaFile.SetHelpKeyword(this.pulAggiungiCommentario, resources.GetString("pulAggiungiCommentario.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.pulAggiungiCommentario, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulAggiungiCommentario.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.pulAggiungiCommentario, null);
            this.pulAggiungiCommentario.Image = global::LaParola.Properties.Resources.ordineprossimo;
            this.pulAggiungiCommentario.Name = "pulAggiungiCommentario";
            this.guidaFile.SetShowHelp(this.pulAggiungiCommentario, ((bool)(resources.GetObject("pulAggiungiCommentario.ShowHelp"))));
            this.pulAggiungiCommentario.UseVisualStyleBackColor = true;
            this.pulAggiungiCommentario.Click += new System.EventHandler(this.pulAggiungiCommentario_Click);
            // 
            // pulRimuovi
            // 
            this.pulRimuovi.AccessibleDescription = null;
            this.pulRimuovi.AccessibleName = null;
            resources.ApplyResources(this.pulRimuovi, "pulRimuovi");
            this.pulRimuovi.BackgroundImage = null;
            this.pulRimuovi.Font = null;
            this.guidaFile.SetHelpKeyword(this.pulRimuovi, resources.GetString("pulRimuovi.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.pulRimuovi, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulRimuovi.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.pulRimuovi, null);
            this.pulRimuovi.Image = global::LaParola.Properties.Resources.ordineprecdente;
            this.pulRimuovi.Name = "pulRimuovi";
            this.guidaFile.SetShowHelp(this.pulRimuovi, ((bool)(resources.GetObject("pulRimuovi.ShowHelp"))));
            this.pulRimuovi.UseVisualStyleBackColor = true;
            this.pulRimuovi.Click += new System.EventHandler(this.pulRimuovi_Click);
            // 
            // pulSu
            // 
            this.pulSu.AccessibleDescription = null;
            this.pulSu.AccessibleName = null;
            resources.ApplyResources(this.pulSu, "pulSu");
            this.pulSu.BackgroundImage = null;
            this.pulSu.Font = null;
            this.guidaFile.SetHelpKeyword(this.pulSu, resources.GetString("pulSu.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.pulSu, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulSu.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.pulSu, null);
            this.pulSu.Image = global::LaParola.Properties.Resources.arrow_u;
            this.pulSu.Name = "pulSu";
            this.guidaFile.SetShowHelp(this.pulSu, ((bool)(resources.GetObject("pulSu.ShowHelp"))));
            this.pulSu.UseVisualStyleBackColor = true;
            this.pulSu.Click += new System.EventHandler(this.pulSu_Click);
            // 
            // pulGiu
            // 
            this.pulGiu.AccessibleDescription = null;
            this.pulGiu.AccessibleName = null;
            resources.ApplyResources(this.pulGiu, "pulGiu");
            this.pulGiu.BackgroundImage = null;
            this.pulGiu.Font = null;
            this.guidaFile.SetHelpKeyword(this.pulGiu, resources.GetString("pulGiu.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.pulGiu, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulGiu.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.pulGiu, null);
            this.pulGiu.Image = global::LaParola.Properties.Resources.arrow_d;
            this.pulGiu.Name = "pulGiu";
            this.guidaFile.SetShowHelp(this.pulGiu, ((bool)(resources.GetObject("pulGiu.ShowHelp"))));
            this.pulGiu.UseVisualStyleBackColor = true;
            this.pulGiu.Click += new System.EventHandler(this.pulGiu_Click);
            // 
            // pmApri
            // 
            this.pmApri.AccessibleDescription = null;
            this.pmApri.AccessibleName = null;
            resources.ApplyResources(this.pmApri, "pmApri");
            this.pmApri.BackgroundImage = null;
            this.pmApri.Font = null;
            this.guidaFile.SetHelpKeyword(this.pmApri, null);
            this.guidaFile.SetHelpNavigator(this.pmApri, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pmApri.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.pmApri, null);
            this.pmApri.Name = "pmApri";
            this.guidaFile.SetShowHelp(this.pmApri, ((bool)(resources.GetObject("pmApri.ShowHelp"))));
            this.pmApri.Opening += new System.ComponentModel.CancelEventHandler(this.pmApri_Opening);
            // 
            // lbDizionari
            // 
            this.lbDizionari.AccessibleDescription = null;
            this.lbDizionari.AccessibleName = null;
            resources.ApplyResources(this.lbDizionari, "lbDizionari");
            this.lbDizionari.BackgroundImage = null;
            this.lbDizionari.Font = null;
            this.lbDizionari.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.lbDizionari, resources.GetString("lbDizionari.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.lbDizionari, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("lbDizionari.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.lbDizionari, null);
            this.lbDizionari.Name = "lbDizionari";
            this.guidaFile.SetShowHelp(this.lbDizionari, ((bool)(resources.GetObject("lbDizionari.ShowHelp"))));
            this.lbDizionari.Sorted = true;
            this.lbDizionari.SelectedIndexChanged += new System.EventHandler(this.lbDizionari_SelectedIndexChanged);
            // 
            // etiDizionari
            // 
            this.etiDizionari.AccessibleDescription = null;
            this.etiDizionari.AccessibleName = null;
            resources.ApplyResources(this.etiDizionari, "etiDizionari");
            this.etiDizionari.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiDizionari, resources.GetString("etiDizionari.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.etiDizionari, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiDizionari.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiDizionari, null);
            this.etiDizionari.Name = "etiDizionari";
            this.guidaFile.SetShowHelp(this.etiDizionari, ((bool)(resources.GetObject("etiDizionari.ShowHelp"))));
            // 
            // pulAggiungiDizionario
            // 
            this.pulAggiungiDizionario.AccessibleDescription = null;
            this.pulAggiungiDizionario.AccessibleName = null;
            resources.ApplyResources(this.pulAggiungiDizionario, "pulAggiungiDizionario");
            this.pulAggiungiDizionario.BackgroundImage = null;
            this.pulAggiungiDizionario.Font = null;
            this.guidaFile.SetHelpKeyword(this.pulAggiungiDizionario, resources.GetString("pulAggiungiDizionario.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.pulAggiungiDizionario, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulAggiungiDizionario.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.pulAggiungiDizionario, null);
            this.pulAggiungiDizionario.Image = global::LaParola.Properties.Resources.ordineprossimo;
            this.pulAggiungiDizionario.Name = "pulAggiungiDizionario";
            this.guidaFile.SetShowHelp(this.pulAggiungiDizionario, ((bool)(resources.GetObject("pulAggiungiDizionario.ShowHelp"))));
            this.pulAggiungiDizionario.UseVisualStyleBackColor = true;
            this.pulAggiungiDizionario.Click += new System.EventHandler(this.pulAggiungiDizionario_Click);
            // 
            // TestiParalleli
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.pulAggiungiDizionario);
            this.Controls.Add(this.pulRimuovi);
            this.Controls.Add(this.pulSu);
            this.Controls.Add(this.etiBibbie);
            this.Controls.Add(this.etiDizionari);
            this.Controls.Add(this.pulAggiungiBibbia);
            this.Controls.Add(this.pulApri);
            this.Controls.Add(this.pulAggiungiCommentario);
            this.Controls.Add(this.pulSalva);
            this.Controls.Add(this.lbBibbie);
            this.Controls.Add(this.pulGiu);
            this.Controls.Add(this.lbCommentari);
            this.Controls.Add(this.etiScelti);
            this.Controls.Add(this.lbDizionari);
            this.Controls.Add(this.etiCommentari);
            this.Controls.Add(this.lbScelti);
            this.Font = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.guidaFile.SetHelpKeyword(this, null);
            this.guidaFile.SetHelpNavigator(this, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("$this.HelpNavigator"))));
            this.guidaFile.SetHelpString(this, null);
            this.Name = "TestiParalleli";
            this.guidaFile.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Resize += new System.EventHandler(this.TestiParalleli_Resize);
            this.Controls.SetChildIndex(this.lbScelti, 0);
            this.Controls.SetChildIndex(this.etiCommentari, 0);
            this.Controls.SetChildIndex(this.lbDizionari, 0);
            this.Controls.SetChildIndex(this.etiScelti, 0);
            this.Controls.SetChildIndex(this.lbCommentari, 0);
            this.Controls.SetChildIndex(this.pulGiu, 0);
            this.Controls.SetChildIndex(this.lbBibbie, 0);
            this.Controls.SetChildIndex(this.pulSalva, 0);
            this.Controls.SetChildIndex(this.pulAggiungiCommentario, 0);
            this.Controls.SetChildIndex(this.pulApri, 0);
            this.Controls.SetChildIndex(this.pulAggiungiBibbia, 0);
            this.Controls.SetChildIndex(this.etiDizionari, 0);
            this.Controls.SetChildIndex(this.etiBibbie, 0);
            this.Controls.SetChildIndex(this.pulSu, 0);
            this.Controls.SetChildIndex(this.pulRimuovi, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnCanc, 0);
            this.Controls.SetChildIndex(this.pulAggiungiDizionario, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbBibbie;
        private System.Windows.Forms.ListBox lbCommentari;
        private System.Windows.Forms.ListBox lbScelti;
        private System.Windows.Forms.Label etiBibbie;
        private System.Windows.Forms.Label etiCommentari;
        private System.Windows.Forms.Label etiScelti;
        private System.Windows.Forms.Button pulSalva;
        private System.Windows.Forms.Button pulApri;
        private System.Windows.Forms.Button pulAggiungiBibbia;
        private System.Windows.Forms.Button pulAggiungiCommentario;
        private System.Windows.Forms.Button pulRimuovi;
        private System.Windows.Forms.Button pulSu;
        private System.Windows.Forms.Button pulGiu;
        private System.Windows.Forms.ContextMenuStrip pmApri;
        private System.Windows.Forms.ListBox lbDizionari;
        private System.Windows.Forms.Label etiDizionari;
        private System.Windows.Forms.Button pulAggiungiDizionario;
    }
}