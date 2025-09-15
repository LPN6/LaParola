namespace LaParola
{
    partial class Mostra
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Mostra));
            this.labBrano = new System.Windows.Forms.Label();
            this.clbVersioni = new System.Windows.Forms.CheckedListBox();
            this.labVersioni = new System.Windows.Forms.Label();
            this.cbBrano = new System.Windows.Forms.ComboBox();
            this.btnSelezionaTutte = new System.Windows.Forms.Button();
            this.btnDeselezionaTutte = new System.Windows.Forms.Button();
            this.btnGiu = new System.Windows.Forms.Button();
            this.btnSu = new System.Windows.Forms.Button();
            this.cbAlternare = new System.Windows.Forms.CheckBox();
            this.cbDefinizioni = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.guidaFile.SetShowHelp(this.btnOK, ((bool)(resources.GetObject("btnOK.ShowHelp"))));
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCanc
            // 
            resources.ApplyResources(this.btnCanc, "btnCanc");
            this.guidaFile.SetShowHelp(this.btnCanc, ((bool)(resources.GetObject("btnCanc.ShowHelp"))));
            this.btnCanc.Click += new System.EventHandler(this.btnCanc_Click);
            // 
            // labBrano
            // 
            resources.ApplyResources(this.labBrano, "labBrano");
            this.guidaFile.SetHelpKeyword(this.labBrano, resources.GetString("labBrano.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.labBrano, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("labBrano.HelpNavigator"))));
            this.labBrano.Name = "labBrano";
            this.guidaFile.SetShowHelp(this.labBrano, ((bool)(resources.GetObject("labBrano.ShowHelp"))));
            // 
            // clbVersioni
            // 
            this.clbVersioni.CheckOnClick = true;
            this.clbVersioni.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.clbVersioni, resources.GetString("clbVersioni.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.clbVersioni, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("clbVersioni.HelpNavigator"))));
            resources.ApplyResources(this.clbVersioni, "clbVersioni");
            this.clbVersioni.Name = "clbVersioni";
            this.guidaFile.SetShowHelp(this.clbVersioni, ((bool)(resources.GetObject("clbVersioni.ShowHelp"))));
            this.clbVersioni.SelectedIndexChanged += new System.EventHandler(this.clbVersioni_SelectedIndexChanged);
            this.clbVersioni.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbVersioni_ItemCheck);
            // 
            // labVersioni
            // 
            resources.ApplyResources(this.labVersioni, "labVersioni");
            this.labVersioni.Name = "labVersioni";
            this.guidaFile.SetShowHelp(this.labVersioni, ((bool)(resources.GetObject("labVersioni.ShowHelp"))));
            // 
            // cbBrano
            // 
            this.cbBrano.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbBrano.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbBrano.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.cbBrano, resources.GetString("cbBrano.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbBrano, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbBrano.HelpNavigator"))));
            resources.ApplyResources(this.cbBrano, "cbBrano");
            this.cbBrano.Name = "cbBrano";
            this.guidaFile.SetShowHelp(this.cbBrano, ((bool)(resources.GetObject("cbBrano.ShowHelp"))));
            this.cbBrano.SelectedIndexChanged += new System.EventHandler(this.cbBrano_TextChanged);
            this.cbBrano.TextChanged += new System.EventHandler(this.cbBrano_TextChanged);
            // 
            // btnSelezionaTutte
            // 
            resources.ApplyResources(this.btnSelezionaTutte, "btnSelezionaTutte");
            this.guidaFile.SetHelpKeyword(this.btnSelezionaTutte, resources.GetString("btnSelezionaTutte.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.btnSelezionaTutte, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("btnSelezionaTutte.HelpNavigator"))));
            this.btnSelezionaTutte.Name = "btnSelezionaTutte";
            this.guidaFile.SetShowHelp(this.btnSelezionaTutte, ((bool)(resources.GetObject("btnSelezionaTutte.ShowHelp"))));
            this.btnSelezionaTutte.UseVisualStyleBackColor = true;
            this.btnSelezionaTutte.Click += new System.EventHandler(this.btnSelezionaTutte_Click);
            // 
            // btnDeselezionaTutte
            // 
            resources.ApplyResources(this.btnDeselezionaTutte, "btnDeselezionaTutte");
            this.guidaFile.SetHelpKeyword(this.btnDeselezionaTutte, resources.GetString("btnDeselezionaTutte.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.btnDeselezionaTutte, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("btnDeselezionaTutte.HelpNavigator"))));
            this.btnDeselezionaTutte.Name = "btnDeselezionaTutte";
            this.guidaFile.SetShowHelp(this.btnDeselezionaTutte, ((bool)(resources.GetObject("btnDeselezionaTutte.ShowHelp"))));
            this.btnDeselezionaTutte.UseVisualStyleBackColor = true;
            this.btnDeselezionaTutte.Click += new System.EventHandler(this.btnDeselezionaTutte_Click);
            // 
            // btnGiu
            // 
            resources.ApplyResources(this.btnGiu, "btnGiu");
            this.guidaFile.SetHelpKeyword(this.btnGiu, resources.GetString("btnGiu.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.btnGiu, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("btnGiu.HelpNavigator"))));
            this.btnGiu.Image = global::LaParola.Properties.Resources.arrow_d;
            this.btnGiu.Name = "btnGiu";
            this.guidaFile.SetShowHelp(this.btnGiu, ((bool)(resources.GetObject("btnGiu.ShowHelp"))));
            this.btnGiu.UseVisualStyleBackColor = true;
            this.btnGiu.Click += new System.EventHandler(this.btnGiu_Click);
            // 
            // btnSu
            // 
            resources.ApplyResources(this.btnSu, "btnSu");
            this.guidaFile.SetHelpKeyword(this.btnSu, resources.GetString("btnSu.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.btnSu, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("btnSu.HelpNavigator"))));
            this.btnSu.Image = global::LaParola.Properties.Resources.arrow_u;
            this.btnSu.Name = "btnSu";
            this.guidaFile.SetShowHelp(this.btnSu, ((bool)(resources.GetObject("btnSu.ShowHelp"))));
            this.btnSu.UseVisualStyleBackColor = true;
            this.btnSu.Click += new System.EventHandler(this.btnSu_Click);
            // 
            // cbAlternare
            // 
            resources.ApplyResources(this.cbAlternare, "cbAlternare");
            this.guidaFile.SetHelpKeyword(this.cbAlternare, resources.GetString("cbAlternare.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbAlternare, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbAlternare.HelpNavigator"))));
            this.cbAlternare.Name = "cbAlternare";
            this.guidaFile.SetShowHelp(this.cbAlternare, ((bool)(resources.GetObject("cbAlternare.ShowHelp"))));
            this.cbAlternare.UseVisualStyleBackColor = true;
            // 
            // cbDefinizioni
            // 
            resources.ApplyResources(this.cbDefinizioni, "cbDefinizioni");
            this.guidaFile.SetHelpKeyword(this.cbDefinizioni, resources.GetString("cbDefinizioni.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbDefinizioni, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbDefinizioni.HelpNavigator"))));
            this.cbDefinizioni.Name = "cbDefinizioni";
            this.guidaFile.SetShowHelp(this.cbDefinizioni, ((bool)(resources.GetObject("cbDefinizioni.ShowHelp"))));
            this.cbDefinizioni.UseVisualStyleBackColor = true;
            // 
            // Mostra
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbDefinizioni);
            this.Controls.Add(this.cbAlternare);
            this.Controls.Add(this.btnDeselezionaTutte);
            this.Controls.Add(this.btnSelezionaTutte);
            this.Controls.Add(this.cbBrano);
            this.Controls.Add(this.btnGiu);
            this.Controls.Add(this.labVersioni);
            this.Controls.Add(this.clbVersioni);
            this.Controls.Add(this.btnSu);
            this.Controls.Add(this.labBrano);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "Mostra";
            this.guidaFile.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.Tag = "Mostra";
            this.Load += new System.EventHandler(this.Mostra_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Mostra_FormClosing);
            this.Resize += new System.EventHandler(this.Mostra_Resize);
            this.Controls.SetChildIndex(this.labBrano, 0);
            this.Controls.SetChildIndex(this.btnSu, 0);
            this.Controls.SetChildIndex(this.clbVersioni, 0);
            this.Controls.SetChildIndex(this.labVersioni, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnCanc, 0);
            this.Controls.SetChildIndex(this.btnGiu, 0);
            this.Controls.SetChildIndex(this.cbBrano, 0);
            this.Controls.SetChildIndex(this.btnSelezionaTutte, 0);
            this.Controls.SetChildIndex(this.btnDeselezionaTutte, 0);
            this.Controls.SetChildIndex(this.cbAlternare, 0);
            this.Controls.SetChildIndex(this.cbDefinizioni, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labBrano;
        private System.Windows.Forms.CheckedListBox clbVersioni;
        private System.Windows.Forms.Label labVersioni;
        private System.Windows.Forms.Button btnSu;
        private System.Windows.Forms.Button btnGiu;
        private System.Windows.Forms.ComboBox cbBrano;
        private System.Windows.Forms.Button btnSelezionaTutte;
        private System.Windows.Forms.Button btnDeselezionaTutte;
        private System.Windows.Forms.CheckBox cbAlternare;
        private System.Windows.Forms.CheckBox cbDefinizioni;
    }
}