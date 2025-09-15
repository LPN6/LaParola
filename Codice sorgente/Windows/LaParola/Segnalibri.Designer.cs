namespace LaParola
{
    partial class Segnalibri
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Segnalibri));
            this.tvSegnalibri = new System.Windows.Forms.TreeView();
            this.etiRiferimento = new System.Windows.Forms.Label();
            this.tbRiferimento = new System.Windows.Forms.TextBox();
            this.etiDescrizione = new System.Windows.Forms.Label();
            this.pulApri = new System.Windows.Forms.Button();
            this.pulAggiungi = new System.Windows.Forms.Button();
            this.pulModifica = new System.Windows.Forms.Button();
            this.pulCancella = new System.Windows.Forms.Button();
            this.pulSalva = new System.Windows.Forms.Button();
            this.pulMostra = new System.Windows.Forms.Button();
            this.pulImporta = new System.Windows.Forms.Button();
            this.menuImporta = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuImportaFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuImportaInternet = new System.Windows.Forms.ToolStripMenuItem();
            this.menuImportaClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.menuImporta.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.guidaFile.SetShowHelp(this.btnOK, ((bool)(resources.GetObject("btnOK.ShowHelp"))));
            // 
            // btnCanc
            // 
            resources.ApplyResources(this.btnCanc, "btnCanc");
            this.guidaFile.SetShowHelp(this.btnCanc, ((bool)(resources.GetObject("btnCanc.ShowHelp"))));
            this.btnCanc.Click += new System.EventHandler(this.btnCanc_Click);
            // 
            // tvSegnalibri
            // 
            this.tvSegnalibri.AllowDrop = true;
            this.guidaFile.SetHelpKeyword(this.tvSegnalibri, resources.GetString("tvSegnalibri.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.tvSegnalibri, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("tvSegnalibri.HelpNavigator"))));
            this.tvSegnalibri.HotTracking = true;
            this.tvSegnalibri.LabelEdit = true;
            resources.ApplyResources(this.tvSegnalibri, "tvSegnalibri");
            this.tvSegnalibri.Name = "tvSegnalibri";
            this.guidaFile.SetShowHelp(this.tvSegnalibri, ((bool)(resources.GetObject("tvSegnalibri.ShowHelp"))));
            this.tvSegnalibri.ShowNodeToolTips = true;
            this.tvSegnalibri.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvSegnalibri_AfterSelect);
            this.tvSegnalibri.DragDrop += new System.Windows.Forms.DragEventHandler(this.tvSegnalibri_DragDrop);
            this.tvSegnalibri.DragOver += new System.Windows.Forms.DragEventHandler(this.tvSegnalibri_DragOver);
            this.tvSegnalibri.DoubleClick += new System.EventHandler(this.tvSegnalibri_DoubleClick);
            this.tvSegnalibri.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tvSegnalibri_MouseDown);
            this.tvSegnalibri.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tvSegnalibri_MouseMove);
            this.tvSegnalibri.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tvSegnalibri_MouseUp);
            // 
            // etiRiferimento
            // 
            resources.ApplyResources(this.etiRiferimento, "etiRiferimento");
            this.guidaFile.SetHelpKeyword(this.etiRiferimento, resources.GetString("etiRiferimento.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.etiRiferimento, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiRiferimento.HelpNavigator"))));
            this.etiRiferimento.Name = "etiRiferimento";
            this.guidaFile.SetShowHelp(this.etiRiferimento, ((bool)(resources.GetObject("etiRiferimento.ShowHelp"))));
            // 
            // tbRiferimento
            // 
            resources.ApplyResources(this.tbRiferimento, "tbRiferimento");
            this.guidaFile.SetHelpKeyword(this.tbRiferimento, resources.GetString("tbRiferimento.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.tbRiferimento, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("tbRiferimento.HelpNavigator"))));
            this.tbRiferimento.Name = "tbRiferimento";
            this.guidaFile.SetShowHelp(this.tbRiferimento, ((bool)(resources.GetObject("tbRiferimento.ShowHelp"))));
            this.tbRiferimento.TextChanged += new System.EventHandler(this.tbRiferimento_TextChanged);
            // 
            // etiDescrizione
            // 
            resources.ApplyResources(this.etiDescrizione, "etiDescrizione");
            this.guidaFile.SetHelpKeyword(this.etiDescrizione, resources.GetString("etiDescrizione.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.etiDescrizione, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiDescrizione.HelpNavigator"))));
            this.etiDescrizione.Name = "etiDescrizione";
            this.guidaFile.SetShowHelp(this.etiDescrizione, ((bool)(resources.GetObject("etiDescrizione.ShowHelp"))));
            // 
            // pulApri
            // 
            resources.ApplyResources(this.pulApri, "pulApri");
            this.guidaFile.SetHelpKeyword(this.pulApri, resources.GetString("pulApri.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.pulApri, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulApri.HelpNavigator"))));
            this.pulApri.Name = "pulApri";
            this.guidaFile.SetShowHelp(this.pulApri, ((bool)(resources.GetObject("pulApri.ShowHelp"))));
            this.pulApri.UseVisualStyleBackColor = true;
            this.pulApri.Click += new System.EventHandler(this.pulApri_Click);
            // 
            // pulAggiungi
            // 
            resources.ApplyResources(this.pulAggiungi, "pulAggiungi");
            this.guidaFile.SetHelpKeyword(this.pulAggiungi, resources.GetString("pulAggiungi.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.pulAggiungi, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulAggiungi.HelpNavigator"))));
            this.pulAggiungi.Name = "pulAggiungi";
            this.guidaFile.SetShowHelp(this.pulAggiungi, ((bool)(resources.GetObject("pulAggiungi.ShowHelp"))));
            this.pulAggiungi.UseVisualStyleBackColor = true;
            this.pulAggiungi.Click += new System.EventHandler(this.pulAggiungi_Click);
            // 
            // pulModifica
            // 
            resources.ApplyResources(this.pulModifica, "pulModifica");
            this.guidaFile.SetHelpKeyword(this.pulModifica, resources.GetString("pulModifica.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.pulModifica, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulModifica.HelpNavigator"))));
            this.pulModifica.Name = "pulModifica";
            this.guidaFile.SetShowHelp(this.pulModifica, ((bool)(resources.GetObject("pulModifica.ShowHelp"))));
            this.pulModifica.UseVisualStyleBackColor = true;
            this.pulModifica.Click += new System.EventHandler(this.pulModifica_Click);
            // 
            // pulCancella
            // 
            resources.ApplyResources(this.pulCancella, "pulCancella");
            this.guidaFile.SetHelpKeyword(this.pulCancella, resources.GetString("pulCancella.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.pulCancella, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulCancella.HelpNavigator"))));
            this.pulCancella.Name = "pulCancella";
            this.guidaFile.SetShowHelp(this.pulCancella, ((bool)(resources.GetObject("pulCancella.ShowHelp"))));
            this.pulCancella.UseVisualStyleBackColor = true;
            this.pulCancella.Click += new System.EventHandler(this.pulCancella_Click);
            // 
            // pulSalva
            // 
            resources.ApplyResources(this.pulSalva, "pulSalva");
            this.guidaFile.SetHelpKeyword(this.pulSalva, resources.GetString("pulSalva.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.pulSalva, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulSalva.HelpNavigator"))));
            this.pulSalva.Name = "pulSalva";
            this.guidaFile.SetShowHelp(this.pulSalva, ((bool)(resources.GetObject("pulSalva.ShowHelp"))));
            this.pulSalva.UseVisualStyleBackColor = true;
            this.pulSalva.Click += new System.EventHandler(this.pulSalva_Click);
            // 
            // pulMostra
            // 
            resources.ApplyResources(this.pulMostra, "pulMostra");
            this.guidaFile.SetHelpKeyword(this.pulMostra, resources.GetString("pulMostra.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.pulMostra, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulMostra.HelpNavigator"))));
            this.pulMostra.Name = "pulMostra";
            this.guidaFile.SetShowHelp(this.pulMostra, ((bool)(resources.GetObject("pulMostra.ShowHelp"))));
            this.pulMostra.UseVisualStyleBackColor = true;
            this.pulMostra.Click += new System.EventHandler(this.pulMostra_Click);
            // 
            // pulImporta
            // 
            resources.ApplyResources(this.pulImporta, "pulImporta");
            this.pulImporta.ContextMenuStrip = this.menuImporta;
            this.guidaFile.SetHelpKeyword(this.pulImporta, resources.GetString("pulImporta.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.pulImporta, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulImporta.HelpNavigator"))));
            this.pulImporta.Name = "pulImporta";
            this.guidaFile.SetShowHelp(this.pulImporta, ((bool)(resources.GetObject("pulImporta.ShowHelp"))));
            this.pulImporta.UseVisualStyleBackColor = true;
            this.pulImporta.Click += new System.EventHandler(this.pulImporta_Click);
            // 
            // menuImporta
            // 
            this.menuImporta.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuImportaFile,
            this.menuImportaInternet,
            this.menuImportaClipboard});
            this.menuImporta.Name = "menuImporta";
            this.guidaFile.SetShowHelp(this.menuImporta, ((bool)(resources.GetObject("menuImporta.ShowHelp"))));
            resources.ApplyResources(this.menuImporta, "menuImporta");
            // 
            // menuImportaFile
            // 
            this.menuImportaFile.Name = "menuImportaFile";
            resources.ApplyResources(this.menuImportaFile, "menuImportaFile");
            this.menuImportaFile.Click += new System.EventHandler(this.menuImportaFile_Click);
            // 
            // menuImportaInternet
            // 
            this.menuImportaInternet.Name = "menuImportaInternet";
            resources.ApplyResources(this.menuImportaInternet, "menuImportaInternet");
            this.menuImportaInternet.Click += new System.EventHandler(this.menuImportaInternet_Click);
            // 
            // menuImportaClipboard
            // 
            this.menuImportaClipboard.Name = "menuImportaClipboard";
            resources.ApplyResources(this.menuImportaClipboard, "menuImportaClipboard");
            this.menuImportaClipboard.Click += new System.EventHandler(this.menuImportaClipboard_Click);
            // 
            // Segnalibri
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pulImporta);
            this.Controls.Add(this.pulMostra);
            this.Controls.Add(this.tvSegnalibri);
            this.Controls.Add(this.tbRiferimento);
            this.Controls.Add(this.pulSalva);
            this.Controls.Add(this.pulCancella);
            this.Controls.Add(this.pulModifica);
            this.Controls.Add(this.pulApri);
            this.Controls.Add(this.etiRiferimento);
            this.Controls.Add(this.pulAggiungi);
            this.Controls.Add(this.etiDescrizione);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "Segnalibri";
            this.guidaFile.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Tag = "Segnalibri";
            this.Resize += new System.EventHandler(this.Segnalibri_Resize);
            this.Controls.SetChildIndex(this.etiDescrizione, 0);
            this.Controls.SetChildIndex(this.pulAggiungi, 0);
            this.Controls.SetChildIndex(this.etiRiferimento, 0);
            this.Controls.SetChildIndex(this.pulApri, 0);
            this.Controls.SetChildIndex(this.pulModifica, 0);
            this.Controls.SetChildIndex(this.pulCancella, 0);
            this.Controls.SetChildIndex(this.pulSalva, 0);
            this.Controls.SetChildIndex(this.tbRiferimento, 0);
            this.Controls.SetChildIndex(this.tvSegnalibri, 0);
            this.Controls.SetChildIndex(this.pulMostra, 0);
            this.Controls.SetChildIndex(this.pulImporta, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnCanc, 0);
            this.menuImporta.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView tvSegnalibri;
        private System.Windows.Forms.Label etiRiferimento;
        private System.Windows.Forms.TextBox tbRiferimento;
        private System.Windows.Forms.Label etiDescrizione;
        private System.Windows.Forms.Button pulApri;
        private System.Windows.Forms.Button pulAggiungi;
        private System.Windows.Forms.Button pulModifica;
        private System.Windows.Forms.Button pulCancella;
        private System.Windows.Forms.Button pulSalva;
        private System.Windows.Forms.Button pulMostra;
        private System.Windows.Forms.Button pulImporta;
        private System.Windows.Forms.ContextMenuStrip menuImporta;
        private System.Windows.Forms.ToolStripMenuItem menuImportaFile;
        private System.Windows.Forms.ToolStripMenuItem menuImportaInternet;
        private System.Windows.Forms.ToolStripMenuItem menuImportaClipboard;
    }
}