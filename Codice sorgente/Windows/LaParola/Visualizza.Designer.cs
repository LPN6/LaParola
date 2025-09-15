using TestiBiblici;
namespace LaParola
{
    partial class Visualizza
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Visualizza));
            this.btnGuida = new System.Windows.Forms.Button();
            this.pmSincronizzato = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.pmSinc1 = new System.Windows.Forms.ToolStripMenuItem();
            this.pmSinc2 = new System.Windows.Forms.ToolStripMenuItem();
            this.pmSinc3 = new System.Windows.Forms.ToolStripMenuItem();
            this.pmSinc4 = new System.Windows.Forms.ToolStripMenuItem();
            this.pmSinc5 = new System.Windows.Forms.ToolStripMenuItem();
            this.pmSinc6 = new System.Windows.Forms.ToolStripMenuItem();
            this.pmSinc7 = new System.Windows.Forms.ToolStripMenuItem();
            this.pmSinc8 = new System.Windows.Forms.ToolStripMenuItem();
            this.pmSinc9 = new System.Windows.Forms.ToolStripMenuItem();
            this.pmSincSeparatore = new System.Windows.Forms.ToolStripSeparator();
            this.pmSincNo = new System.Windows.Forms.ToolStripMenuItem();
            this.pulAggiungi = new System.Windows.Forms.Button();
            this.pmAggiungi = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.pmAggiungiBibbia = new System.Windows.Forms.ToolStripMenuItem();
            this.pmAggiungiCommentario = new System.Windows.Forms.ToolStripMenuItem();
            this.pmAggiungiDizionario = new System.Windows.Forms.ToolStripMenuItem();
            this.pmConfrontaBibbia = new System.Windows.Forms.ToolStripMenuItem();
            this.panPanes = new System.Windows.Forms.Panel();
            this.panPulsanti = new System.Windows.Forms.Panel();
            this.pulTestiParalleli = new System.Windows.Forms.Button();
            this.pmSincronizzato.SuspendLayout();
            this.pmAggiungi.SuspendLayout();
            this.panPulsanti.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.guidaFile.SetShowHelp(this.btnOK, ((bool)(resources.GetObject("btnOK.ShowHelp"))));
            // 
            // btnCanc
            // 
            this.btnCanc.Image = global::LaParola.Properties.Resources.cancella;
            resources.ApplyResources(this.btnCanc, "btnCanc");
            this.guidaFile.SetShowHelp(this.btnCanc, ((bool)(resources.GetObject("btnCanc.ShowHelp"))));
            this.btnCanc.Click += new System.EventHandler(this.BtnCanc_Click);
            // 
            // btnGuida
            // 
            resources.ApplyResources(this.btnGuida, "btnGuida");
            this.btnGuida.Image = global::LaParola.Properties.Resources.guida;
            this.btnGuida.Name = "btnGuida";
            this.guidaFile.SetShowHelp(this.btnGuida, ((bool)(resources.GetObject("btnGuida.ShowHelp"))));
            this.btnGuida.UseVisualStyleBackColor = true;
            this.btnGuida.Click += new System.EventHandler(this.BtnGuida_Click);
            // 
            // pmSincronizzato
            // 
            this.pmSincronizzato.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pmSinc1,
            this.pmSinc2,
            this.pmSinc3,
            this.pmSinc4,
            this.pmSinc5,
            this.pmSinc6,
            this.pmSinc7,
            this.pmSinc8,
            this.pmSinc9,
            this.pmSincSeparatore,
            this.pmSincNo});
            this.pmSincronizzato.Name = "pmSincronizzato";
            this.pmSincronizzato.ShowCheckMargin = true;
            this.guidaFile.SetShowHelp(this.pmSincronizzato, ((bool)(resources.GetObject("pmSincronizzato.ShowHelp"))));
            this.pmSincronizzato.ShowImageMargin = false;
            resources.ApplyResources(this.pmSincronizzato, "pmSincronizzato");
            this.pmSincronizzato.Opening += new System.ComponentModel.CancelEventHandler(this.PmSincronizzato_Opening);
            // 
            // pmSinc1
            // 
            this.pmSinc1.Name = "pmSinc1";
            resources.ApplyResources(this.pmSinc1, "pmSinc1");
            this.pmSinc1.Tag = "&1";
            this.pmSinc1.Click += new System.EventHandler(this.PmSincSottomenu_Click);
            // 
            // pmSinc2
            // 
            this.pmSinc2.Name = "pmSinc2";
            resources.ApplyResources(this.pmSinc2, "pmSinc2");
            this.pmSinc2.Tag = "&2";
            this.pmSinc2.Click += new System.EventHandler(this.PmSincSottomenu_Click);
            // 
            // pmSinc3
            // 
            this.pmSinc3.Name = "pmSinc3";
            resources.ApplyResources(this.pmSinc3, "pmSinc3");
            this.pmSinc3.Tag = "&3";
            this.pmSinc3.Click += new System.EventHandler(this.PmSincSottomenu_Click);
            // 
            // pmSinc4
            // 
            this.pmSinc4.Name = "pmSinc4";
            resources.ApplyResources(this.pmSinc4, "pmSinc4");
            this.pmSinc4.Tag = "&4";
            this.pmSinc4.Click += new System.EventHandler(this.PmSincSottomenu_Click);
            // 
            // pmSinc5
            // 
            this.pmSinc5.Name = "pmSinc5";
            resources.ApplyResources(this.pmSinc5, "pmSinc5");
            this.pmSinc5.Tag = "&5";
            this.pmSinc5.Click += new System.EventHandler(this.PmSincSottomenu_Click);
            // 
            // pmSinc6
            // 
            this.pmSinc6.Name = "pmSinc6";
            resources.ApplyResources(this.pmSinc6, "pmSinc6");
            this.pmSinc6.Tag = "&6";
            this.pmSinc6.Click += new System.EventHandler(this.PmSincSottomenu_Click);
            // 
            // pmSinc7
            // 
            this.pmSinc7.Name = "pmSinc7";
            resources.ApplyResources(this.pmSinc7, "pmSinc7");
            this.pmSinc7.Tag = "&7";
            this.pmSinc7.Click += new System.EventHandler(this.PmSincSottomenu_Click);
            // 
            // pmSinc8
            // 
            this.pmSinc8.Name = "pmSinc8";
            resources.ApplyResources(this.pmSinc8, "pmSinc8");
            this.pmSinc8.Tag = "&8";
            this.pmSinc8.Click += new System.EventHandler(this.PmSincSottomenu_Click);
            // 
            // pmSinc9
            // 
            this.pmSinc9.Name = "pmSinc9";
            resources.ApplyResources(this.pmSinc9, "pmSinc9");
            this.pmSinc9.Tag = "&9";
            this.pmSinc9.Click += new System.EventHandler(this.PmSincSottomenu_Click);
            // 
            // pmSincSeparatore
            // 
            this.pmSincSeparatore.Name = "pmSincSeparatore";
            resources.ApplyResources(this.pmSincSeparatore, "pmSincSeparatore");
            // 
            // pmSincNo
            // 
            this.pmSincNo.Name = "pmSincNo";
            resources.ApplyResources(this.pmSincNo, "pmSincNo");
            this.pmSincNo.Tag = "&X";
            this.pmSincNo.Click += new System.EventHandler(this.PmSincSottomenu_Click);
            // 
            // pulAggiungi
            // 
            resources.ApplyResources(this.pulAggiungi, "pulAggiungi");
            this.pulAggiungi.Image = global::LaParola.Properties.Resources.aggiungi;
            this.pulAggiungi.Name = "pulAggiungi";
            this.guidaFile.SetShowHelp(this.pulAggiungi, ((bool)(resources.GetObject("pulAggiungi.ShowHelp"))));
            this.pulAggiungi.UseVisualStyleBackColor = true;
            this.pulAggiungi.Click += new System.EventHandler(this.PulAggiungi_Click);
            // 
            // pmAggiungi
            // 
            this.pmAggiungi.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pmAggiungiBibbia,
            this.pmAggiungiCommentario,
            this.pmAggiungiDizionario,
            this.pmConfrontaBibbia});
            this.pmAggiungi.Name = "pmAggiungi";
            this.guidaFile.SetShowHelp(this.pmAggiungi, ((bool)(resources.GetObject("pmAggiungi.ShowHelp"))));
            resources.ApplyResources(this.pmAggiungi, "pmAggiungi");
            // 
            // pmAggiungiBibbia
            // 
            this.pmAggiungiBibbia.Name = "pmAggiungiBibbia";
            resources.ApplyResources(this.pmAggiungiBibbia, "pmAggiungiBibbia");
            // 
            // pmAggiungiCommentario
            // 
            this.pmAggiungiCommentario.Name = "pmAggiungiCommentario";
            resources.ApplyResources(this.pmAggiungiCommentario, "pmAggiungiCommentario");
            // 
            // pmAggiungiDizionario
            // 
            this.pmAggiungiDizionario.Name = "pmAggiungiDizionario";
            resources.ApplyResources(this.pmAggiungiDizionario, "pmAggiungiDizionario");
            // 
            // pmConfrontaBibbia
            // 
            this.pmConfrontaBibbia.Name = "pmConfrontaBibbia";
            resources.ApplyResources(this.pmConfrontaBibbia, "pmConfrontaBibbia");
            // 
            // panPanes
            // 
            resources.ApplyResources(this.panPanes, "panPanes");
            this.panPanes.Name = "panPanes";
            this.guidaFile.SetShowHelp(this.panPanes, ((bool)(resources.GetObject("panPanes.ShowHelp"))));
            // 
            // panPulsanti
            // 
            this.panPulsanti.BackColor = System.Drawing.Color.Transparent;
            this.panPulsanti.Controls.Add(this.pulTestiParalleli);
            this.panPulsanti.Controls.Add(this.pulAggiungi);
            this.panPulsanti.Controls.Add(this.btnGuida);
            resources.ApplyResources(this.panPulsanti, "panPulsanti");
            this.panPulsanti.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panPulsanti.Name = "panPulsanti";
            this.guidaFile.SetShowHelp(this.panPulsanti, ((bool)(resources.GetObject("panPulsanti.ShowHelp"))));
            // 
            // pulTestiParalleli
            // 
            resources.ApplyResources(this.pulTestiParalleli, "pulTestiParalleli");
            this.pulTestiParalleli.BackColor = System.Drawing.SystemColors.Control;
            this.pulTestiParalleli.Image = global::LaParola.Properties.Resources.visparalleli;
            this.pulTestiParalleli.Name = "pulTestiParalleli";
            this.guidaFile.SetShowHelp(this.pulTestiParalleli, ((bool)(resources.GetObject("pulTestiParalleli.ShowHelp"))));
            this.pulTestiParalleli.UseVisualStyleBackColor = false;
            this.pulTestiParalleli.Click += new System.EventHandler(this.PulTestiParalleli_Click);
            // 
            // Visualizza
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panPanes);
            this.Controls.Add(this.panPulsanti);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.Name = "Visualizza";
            this.guidaFile.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Tag = "Visualizza";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Visualizza_FormClosing);
            this.Resize += new System.EventHandler(this.Visualizza_Resize);
            this.Controls.SetChildIndex(this.panPulsanti, 0);
            this.Controls.SetChildIndex(this.panPanes, 0);
            this.Controls.SetChildIndex(this.btnCanc, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.pmSincronizzato.ResumeLayout(false);
            this.pmAggiungi.ResumeLayout(false);
            this.panPulsanti.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

//        public System.Windows.Forms.Button btnOK;
        //        protected System.Windows.Forms.Button btnCanc;
        private System.Windows.Forms.Button btnGuida;
        private System.Windows.Forms.ContextMenuStrip pmSincronizzato;
        private System.Windows.Forms.ToolStripMenuItem pmSinc1;
        private System.Windows.Forms.ToolStripMenuItem pmSinc2;
        private System.Windows.Forms.ToolStripMenuItem pmSinc3;
        private System.Windows.Forms.ToolStripMenuItem pmSinc4;
        private System.Windows.Forms.ToolStripMenuItem pmSinc5;
        private System.Windows.Forms.ToolStripMenuItem pmSinc6;
        private System.Windows.Forms.ToolStripMenuItem pmSinc7;
        private System.Windows.Forms.ToolStripMenuItem pmSinc8;
        private System.Windows.Forms.ToolStripMenuItem pmSinc9;
        private System.Windows.Forms.ToolStripSeparator pmSincSeparatore;
        private System.Windows.Forms.ToolStripMenuItem pmSincNo;
        private System.Windows.Forms.Button pulAggiungi;
        private System.Windows.Forms.Panel panPanes;
        private System.Windows.Forms.Panel panPulsanti;
        private System.Windows.Forms.Button pulTestiParalleli;
        private System.Windows.Forms.ToolStripMenuItem pmAggiungiBibbia;
        private System.Windows.Forms.ToolStripMenuItem pmAggiungiCommentario;
        private System.Windows.Forms.ToolStripMenuItem pmAggiungiDizionario;
        private System.Windows.Forms.ContextMenuStrip pmAggiungi;
        private System.Windows.Forms.ToolStripMenuItem pmConfrontaBibbia;

    }
}