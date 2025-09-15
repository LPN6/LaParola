namespace LaParola
{
    partial class BraniParalleli
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BraniParalleli));
            this.cbBrani = new System.Windows.Forms.ComboBox();
            this.pulBranoPrecedente = new System.Windows.Forms.Button();
            this.pulBranoSuccessivo = new System.Windows.Forms.Button();
            this.cbVersioni = new System.Windows.Forms.ComboBox();
            this.etiCercaVersetto = new System.Windows.Forms.Label();
            this.tbCercaVersetto = new System.Windows.Forms.TextBox();
            this.pulCercaVersetto = new System.Windows.Forms.Button();
            this.pulStampa = new System.Windows.Forms.Button();
            this.pulCopia = new System.Windows.Forms.Button();
            this.panPanes = new System.Windows.Forms.Panel();
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
            // cbBrani
            // 
            this.cbBrani.AccessibleDescription = null;
            this.cbBrani.AccessibleName = null;
            resources.ApplyResources(this.cbBrani, "cbBrani");
            this.cbBrani.BackgroundImage = null;
            this.cbBrani.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBrani.Font = null;
            this.cbBrani.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.cbBrani, resources.GetString("cbBrani.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbBrani, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbBrani.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbBrani, null);
            this.cbBrani.Name = "cbBrani";
            this.guidaFile.SetShowHelp(this.cbBrani, ((bool)(resources.GetObject("cbBrani.ShowHelp"))));
            this.cbBrani.SelectedIndexChanged += new System.EventHandler(this.cbBrani_SelectedIndexChanged);
            // 
            // pulBranoPrecedente
            // 
            this.pulBranoPrecedente.AccessibleDescription = null;
            this.pulBranoPrecedente.AccessibleName = null;
            resources.ApplyResources(this.pulBranoPrecedente, "pulBranoPrecedente");
            this.pulBranoPrecedente.BackgroundImage = null;
            this.pulBranoPrecedente.Font = null;
            this.guidaFile.SetHelpKeyword(this.pulBranoPrecedente, resources.GetString("pulBranoPrecedente.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.pulBranoPrecedente, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulBranoPrecedente.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.pulBranoPrecedente, null);
            this.pulBranoPrecedente.Image = global::LaParola.Properties.Resources.ordineprecdente;
            this.pulBranoPrecedente.Name = "pulBranoPrecedente";
            this.guidaFile.SetShowHelp(this.pulBranoPrecedente, ((bool)(resources.GetObject("pulBranoPrecedente.ShowHelp"))));
            this.pulBranoPrecedente.UseVisualStyleBackColor = true;
            this.pulBranoPrecedente.Click += new System.EventHandler(this.pulBranoPrecedente_Click);
            // 
            // pulBranoSuccessivo
            // 
            this.pulBranoSuccessivo.AccessibleDescription = null;
            this.pulBranoSuccessivo.AccessibleName = null;
            resources.ApplyResources(this.pulBranoSuccessivo, "pulBranoSuccessivo");
            this.pulBranoSuccessivo.BackgroundImage = null;
            this.pulBranoSuccessivo.Font = null;
            this.guidaFile.SetHelpKeyword(this.pulBranoSuccessivo, resources.GetString("pulBranoSuccessivo.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.pulBranoSuccessivo, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulBranoSuccessivo.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.pulBranoSuccessivo, null);
            this.pulBranoSuccessivo.Image = global::LaParola.Properties.Resources.ordineprossimo;
            this.pulBranoSuccessivo.Name = "pulBranoSuccessivo";
            this.guidaFile.SetShowHelp(this.pulBranoSuccessivo, ((bool)(resources.GetObject("pulBranoSuccessivo.ShowHelp"))));
            this.pulBranoSuccessivo.UseVisualStyleBackColor = true;
            this.pulBranoSuccessivo.Click += new System.EventHandler(this.pulBranoSuccessivo_Click);
            // 
            // cbVersioni
            // 
            this.cbVersioni.AccessibleDescription = null;
            this.cbVersioni.AccessibleName = null;
            resources.ApplyResources(this.cbVersioni, "cbVersioni");
            this.cbVersioni.BackgroundImage = null;
            this.cbVersioni.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVersioni.Font = null;
            this.cbVersioni.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.cbVersioni, resources.GetString("cbVersioni.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbVersioni, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbVersioni.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbVersioni, null);
            this.cbVersioni.Name = "cbVersioni";
            this.guidaFile.SetShowHelp(this.cbVersioni, ((bool)(resources.GetObject("cbVersioni.ShowHelp"))));
            this.cbVersioni.SelectedIndexChanged += new System.EventHandler(this.cbVersioni_SelectedIndexChanged);
            // 
            // etiCercaVersetto
            // 
            this.etiCercaVersetto.AccessibleDescription = null;
            this.etiCercaVersetto.AccessibleName = null;
            resources.ApplyResources(this.etiCercaVersetto, "etiCercaVersetto");
            this.etiCercaVersetto.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiCercaVersetto, resources.GetString("etiCercaVersetto.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.etiCercaVersetto, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiCercaVersetto.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiCercaVersetto, null);
            this.etiCercaVersetto.Name = "etiCercaVersetto";
            this.guidaFile.SetShowHelp(this.etiCercaVersetto, ((bool)(resources.GetObject("etiCercaVersetto.ShowHelp"))));
            // 
            // tbCercaVersetto
            // 
            this.tbCercaVersetto.AccessibleDescription = null;
            this.tbCercaVersetto.AccessibleName = null;
            resources.ApplyResources(this.tbCercaVersetto, "tbCercaVersetto");
            this.tbCercaVersetto.BackgroundImage = null;
            this.tbCercaVersetto.Font = null;
            this.guidaFile.SetHelpKeyword(this.tbCercaVersetto, resources.GetString("tbCercaVersetto.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.tbCercaVersetto, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("tbCercaVersetto.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.tbCercaVersetto, null);
            this.tbCercaVersetto.Name = "tbCercaVersetto";
            this.guidaFile.SetShowHelp(this.tbCercaVersetto, ((bool)(resources.GetObject("tbCercaVersetto.ShowHelp"))));
            this.tbCercaVersetto.TextChanged += new System.EventHandler(this.tbCercaVersetto_TextChanged);
            this.tbCercaVersetto.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbCercaVersetto_KeyUp);
            // 
            // pulCercaVersetto
            // 
            this.pulCercaVersetto.AccessibleDescription = null;
            this.pulCercaVersetto.AccessibleName = null;
            resources.ApplyResources(this.pulCercaVersetto, "pulCercaVersetto");
            this.pulCercaVersetto.BackgroundImage = null;
            this.pulCercaVersetto.Font = null;
            this.guidaFile.SetHelpKeyword(this.pulCercaVersetto, resources.GetString("pulCercaVersetto.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.pulCercaVersetto, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulCercaVersetto.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.pulCercaVersetto, null);
            this.pulCercaVersetto.Image = global::LaParola.Properties.Resources.infovai;
            this.pulCercaVersetto.Name = "pulCercaVersetto";
            this.guidaFile.SetShowHelp(this.pulCercaVersetto, ((bool)(resources.GetObject("pulCercaVersetto.ShowHelp"))));
            this.pulCercaVersetto.UseVisualStyleBackColor = true;
            this.pulCercaVersetto.Click += new System.EventHandler(this.pulCercaVersetto_Click);
            // 
            // pulStampa
            // 
            this.pulStampa.AccessibleDescription = null;
            this.pulStampa.AccessibleName = null;
            resources.ApplyResources(this.pulStampa, "pulStampa");
            this.pulStampa.BackgroundImage = null;
            this.pulStampa.Font = null;
            this.guidaFile.SetHelpKeyword(this.pulStampa, resources.GetString("pulStampa.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.pulStampa, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulStampa.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.pulStampa, null);
            this.pulStampa.Image = global::LaParola.Properties.Resources.stampa;
            this.pulStampa.Name = "pulStampa";
            this.guidaFile.SetShowHelp(this.pulStampa, ((bool)(resources.GetObject("pulStampa.ShowHelp"))));
            this.pulStampa.UseVisualStyleBackColor = true;
            this.pulStampa.Click += new System.EventHandler(this.pulStampa_Click);
            // 
            // pulCopia
            // 
            this.pulCopia.AccessibleDescription = null;
            this.pulCopia.AccessibleName = null;
            resources.ApplyResources(this.pulCopia, "pulCopia");
            this.pulCopia.BackgroundImage = null;
            this.pulCopia.Font = null;
            this.guidaFile.SetHelpKeyword(this.pulCopia, resources.GetString("pulCopia.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.pulCopia, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulCopia.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.pulCopia, null);
            this.pulCopia.Image = global::LaParola.Properties.Resources.copia;
            this.pulCopia.Name = "pulCopia";
            this.guidaFile.SetShowHelp(this.pulCopia, ((bool)(resources.GetObject("pulCopia.ShowHelp"))));
            this.pulCopia.UseVisualStyleBackColor = true;
            this.pulCopia.Click += new System.EventHandler(this.pulCopia_Click);
            // 
            // panPanes
            // 
            this.panPanes.AccessibleDescription = null;
            this.panPanes.AccessibleName = null;
            resources.ApplyResources(this.panPanes, "panPanes");
            this.panPanes.BackgroundImage = null;
            this.panPanes.Font = null;
            this.guidaFile.SetHelpKeyword(this.panPanes, null);
            this.guidaFile.SetHelpNavigator(this.panPanes, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("panPanes.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.panPanes, null);
            this.panPanes.Name = "panPanes";
            this.guidaFile.SetShowHelp(this.panPanes, ((bool)(resources.GetObject("panPanes.ShowHelp"))));
            // 
            // BraniParalleli
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.pulCercaVersetto);
            this.Controls.Add(this.tbCercaVersetto);
            this.Controls.Add(this.panPanes);
            this.Controls.Add(this.pulCopia);
            this.Controls.Add(this.pulStampa);
            this.Controls.Add(this.etiCercaVersetto);
            this.Controls.Add(this.pulBranoPrecedente);
            this.Controls.Add(this.pulBranoSuccessivo);
            this.Controls.Add(this.cbVersioni);
            this.Controls.Add(this.cbBrani);
            this.Font = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.guidaFile.SetHelpKeyword(this, null);
            this.guidaFile.SetHelpNavigator(this, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("$this.HelpNavigator"))));
            this.guidaFile.SetHelpString(this, null);
            this.Name = "BraniParalleli";
            this.guidaFile.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Tag = "BraniParalleli";
            this.Load += new System.EventHandler(this.BraniParalleli_Load);
            this.Resize += new System.EventHandler(this.BraniParalleli_Resize);
            this.Controls.SetChildIndex(this.cbBrani, 0);
            this.Controls.SetChildIndex(this.cbVersioni, 0);
            this.Controls.SetChildIndex(this.pulBranoSuccessivo, 0);
            this.Controls.SetChildIndex(this.pulBranoPrecedente, 0);
            this.Controls.SetChildIndex(this.etiCercaVersetto, 0);
            this.Controls.SetChildIndex(this.pulStampa, 0);
            this.Controls.SetChildIndex(this.pulCopia, 0);
            this.Controls.SetChildIndex(this.panPanes, 0);
            this.Controls.SetChildIndex(this.tbCercaVersetto, 0);
            this.Controls.SetChildIndex(this.pulCercaVersetto, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnCanc, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbBrani;
        private System.Windows.Forms.Button pulBranoPrecedente;
        private System.Windows.Forms.Button pulBranoSuccessivo;
        private System.Windows.Forms.ComboBox cbVersioni;
        private System.Windows.Forms.Label etiCercaVersetto;
        private System.Windows.Forms.TextBox tbCercaVersetto;
        private System.Windows.Forms.Button pulCercaVersetto;
        private System.Windows.Forms.Button pulStampa;
        private System.Windows.Forms.Button pulCopia;
        private System.Windows.Forms.Panel panPanes;
    }
}