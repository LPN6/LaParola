namespace LaParola
{
    partial class Lettura
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Lettura));
            this.cbSchemi = new System.Windows.Forms.ComboBox();
            this.dtCalendario = new System.Windows.Forms.DateTimePicker();
            this.cbVersioni = new System.Windows.Forms.ComboBox();
            this.pulNuovoInizio = new System.Windows.Forms.Button();
            this.panLetture = new System.Windows.Forms.Panel();
            this.pulStampa = new System.Windows.Forms.Button();
            this.pulCopia = new System.Windows.Forms.Button();
            this.pulContesto = new System.Windows.Forms.Button();
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
            // cbSchemi
            // 
            resources.ApplyResources(this.cbSchemi, "cbSchemi");
            this.cbSchemi.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSchemi.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.cbSchemi, resources.GetString("cbSchemi.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbSchemi, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbSchemi.HelpNavigator"))));
            this.cbSchemi.Name = "cbSchemi";
            this.guidaFile.SetShowHelp(this.cbSchemi, ((bool)(resources.GetObject("cbSchemi.ShowHelp"))));
            this.cbSchemi.SelectedIndexChanged += new System.EventHandler(this.cbSchemi_SelectedIndexChanged);
            // 
            // dtCalendario
            // 
            resources.ApplyResources(this.dtCalendario, "dtCalendario");
            this.guidaFile.SetHelpKeyword(this.dtCalendario, resources.GetString("dtCalendario.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.dtCalendario, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("dtCalendario.HelpNavigator"))));
            this.dtCalendario.Name = "dtCalendario";
            this.guidaFile.SetShowHelp(this.dtCalendario, ((bool)(resources.GetObject("dtCalendario.ShowHelp"))));
            this.dtCalendario.ValueChanged += new System.EventHandler(this.dtCalendario_ValueChanged);
            // 
            // cbVersioni
            // 
            resources.ApplyResources(this.cbVersioni, "cbVersioni");
            this.cbVersioni.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVersioni.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.cbVersioni, resources.GetString("cbVersioni.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbVersioni, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbVersioni.HelpNavigator"))));
            this.cbVersioni.Name = "cbVersioni";
            this.guidaFile.SetShowHelp(this.cbVersioni, ((bool)(resources.GetObject("cbVersioni.ShowHelp"))));
            this.cbVersioni.SelectedIndexChanged += new System.EventHandler(this.cbVersioni_SelectedIndexChanged);
            // 
            // pulNuovoInizio
            // 
            resources.ApplyResources(this.pulNuovoInizio, "pulNuovoInizio");
            this.guidaFile.SetHelpKeyword(this.pulNuovoInizio, resources.GetString("pulNuovoInizio.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.pulNuovoInizio, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulNuovoInizio.HelpNavigator"))));
            this.pulNuovoInizio.Name = "pulNuovoInizio";
            this.guidaFile.SetShowHelp(this.pulNuovoInizio, ((bool)(resources.GetObject("pulNuovoInizio.ShowHelp"))));
            this.pulNuovoInizio.UseVisualStyleBackColor = true;
            this.pulNuovoInizio.Click += new System.EventHandler(this.pulNuovoInizio_Click);
            // 
            // panLetture
            // 
            resources.ApplyResources(this.panLetture, "panLetture");
            this.panLetture.Name = "panLetture";
            this.guidaFile.SetShowHelp(this.panLetture, ((bool)(resources.GetObject("panLetture.ShowHelp"))));
            // 
            // pulStampa
            // 
            resources.ApplyResources(this.pulStampa, "pulStampa");
            this.guidaFile.SetHelpKeyword(this.pulStampa, resources.GetString("pulStampa.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.pulStampa, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulStampa.HelpNavigator"))));
            this.pulStampa.Image = global::LaParola.Properties.Resources.stampa;
            this.pulStampa.Name = "pulStampa";
            this.guidaFile.SetShowHelp(this.pulStampa, ((bool)(resources.GetObject("pulStampa.ShowHelp"))));
            this.pulStampa.UseVisualStyleBackColor = true;
            this.pulStampa.Click += new System.EventHandler(this.pulStampa_Click);
            // 
            // pulCopia
            // 
            resources.ApplyResources(this.pulCopia, "pulCopia");
            this.guidaFile.SetHelpKeyword(this.pulCopia, resources.GetString("pulCopia.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.pulCopia, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulCopia.HelpNavigator"))));
            this.pulCopia.Image = global::LaParola.Properties.Resources.copia;
            this.pulCopia.Name = "pulCopia";
            this.guidaFile.SetShowHelp(this.pulCopia, ((bool)(resources.GetObject("pulCopia.ShowHelp"))));
            this.pulCopia.UseVisualStyleBackColor = true;
            this.pulCopia.Click += new System.EventHandler(this.pulCopia_Click);
            // 
            // pulContesto
            // 
            resources.ApplyResources(this.pulContesto, "pulContesto");
            this.guidaFile.SetHelpKeyword(this.pulContesto, resources.GetString("pulContesto.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.pulContesto, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulContesto.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.pulContesto, resources.GetString("pulContesto.HelpString"));
            this.pulContesto.Image = global::LaParola.Properties.Resources.visbibbia;
            this.pulContesto.Name = "pulContesto";
            this.guidaFile.SetShowHelp(this.pulContesto, ((bool)(resources.GetObject("pulContesto.ShowHelp"))));
            this.pulContesto.UseVisualStyleBackColor = true;
            this.pulContesto.Click += new System.EventHandler(this.pulContesto_Click);
            // 
            // Lettura
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pulContesto);
            this.Controls.Add(this.panLetture);
            this.Controls.Add(this.cbSchemi);
            this.Controls.Add(this.dtCalendario);
            this.Controls.Add(this.pulNuovoInizio);
            this.Controls.Add(this.pulCopia);
            this.Controls.Add(this.cbVersioni);
            this.Controls.Add(this.pulStampa);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "Lettura";
            this.guidaFile.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Tag = "Lettura";
            this.Load += new System.EventHandler(this.Lettura_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Lettura_FormClosing);
            this.Resize += new System.EventHandler(this.Lettura_Resize);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.pulStampa, 0);
            this.Controls.SetChildIndex(this.cbVersioni, 0);
            this.Controls.SetChildIndex(this.pulCopia, 0);
            this.Controls.SetChildIndex(this.pulNuovoInizio, 0);
            this.Controls.SetChildIndex(this.dtCalendario, 0);
            this.Controls.SetChildIndex(this.cbSchemi, 0);
            this.Controls.SetChildIndex(this.panLetture, 0);
            this.Controls.SetChildIndex(this.pulContesto, 0);
            this.Controls.SetChildIndex(this.btnCanc, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cbSchemi;
        private System.Windows.Forms.DateTimePicker dtCalendario;
        private System.Windows.Forms.ComboBox cbVersioni;
        private System.Windows.Forms.Button pulNuovoInizio;
        private System.Windows.Forms.Panel panLetture;
        private System.Windows.Forms.Button pulStampa;
        private System.Windows.Forms.Button pulCopia;
        private System.Windows.Forms.Button pulContesto;
    }
}