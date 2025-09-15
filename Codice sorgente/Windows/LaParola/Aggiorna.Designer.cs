namespace LaParola
{
    partial class Aggiorna
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Aggiorna));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.gridFile = new System.Windows.Forms.DataGridView();
            this.colNome = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTipo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVersioneAttuale = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVersioneNuova = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDimensione = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAzione = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.btnVisualizza = new System.Windows.Forms.Button();
            this.btnAggiornaTutti = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridFile)).BeginInit();
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
            // gridFile
            // 
            this.gridFile.AccessibleDescription = null;
            this.gridFile.AccessibleName = null;
            this.gridFile.AllowUserToAddRows = false;
            this.gridFile.AllowUserToDeleteRows = false;
            resources.ApplyResources(this.gridFile, "gridFile");
            this.gridFile.BackgroundImage = null;
            this.gridFile.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.gridFile.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridFile.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colNome,
            this.colTipo,
            this.colVersioneAttuale,
            this.colVersioneNuova,
            this.colDimensione,
            this.colAzione});
            this.gridFile.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.gridFile.Font = null;
            this.guidaFile.SetHelpKeyword(this.gridFile, resources.GetString("gridFile.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.gridFile, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("gridFile.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.gridFile, null);
            this.gridFile.MultiSelect = false;
            this.gridFile.Name = "gridFile";
            this.gridFile.RowHeadersVisible = false;
            this.guidaFile.SetShowHelp(this.gridFile, ((bool)(resources.GetObject("gridFile.ShowHelp"))));
            // 
            // colNome
            // 
            resources.ApplyResources(this.colNome, "colNome");
            this.colNome.Name = "colNome";
            // 
            // colTipo
            // 
            resources.ApplyResources(this.colTipo, "colTipo");
            this.colTipo.Name = "colTipo";
            // 
            // colVersioneAttuale
            // 
            resources.ApplyResources(this.colVersioneAttuale, "colVersioneAttuale");
            this.colVersioneAttuale.Name = "colVersioneAttuale";
            // 
            // colVersioneNuova
            // 
            resources.ApplyResources(this.colVersioneNuova, "colVersioneNuova");
            this.colVersioneNuova.Name = "colVersioneNuova";
            // 
            // colDimensione
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.colDimensione.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.colDimensione, "colDimensione");
            this.colDimensione.Name = "colDimensione";
            // 
            // colAzione
            // 
            dataGridViewCellStyle2.NullValue = "Don\'t update";
            this.colAzione.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.colAzione, "colAzione");
            this.colAzione.Name = "colAzione";
            // 
            // btnVisualizza
            // 
            this.btnVisualizza.AccessibleDescription = null;
            this.btnVisualizza.AccessibleName = null;
            resources.ApplyResources(this.btnVisualizza, "btnVisualizza");
            this.btnVisualizza.BackgroundImage = null;
            this.btnVisualizza.Font = null;
            this.guidaFile.SetHelpKeyword(this.btnVisualizza, resources.GetString("btnVisualizza.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.btnVisualizza, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("btnVisualizza.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.btnVisualizza, null);
            this.btnVisualizza.Name = "btnVisualizza";
            this.guidaFile.SetShowHelp(this.btnVisualizza, ((bool)(resources.GetObject("btnVisualizza.ShowHelp"))));
            this.btnVisualizza.UseVisualStyleBackColor = true;
            this.btnVisualizza.Click += new System.EventHandler(this.btnVisualizza_Click);
            // 
            // btnAggiornaTutti
            // 
            this.btnAggiornaTutti.AccessibleDescription = null;
            this.btnAggiornaTutti.AccessibleName = null;
            resources.ApplyResources(this.btnAggiornaTutti, "btnAggiornaTutti");
            this.btnAggiornaTutti.BackgroundImage = null;
            this.btnAggiornaTutti.Font = null;
            this.guidaFile.SetHelpKeyword(this.btnAggiornaTutti, resources.GetString("btnAggiornaTutti.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.btnAggiornaTutti, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("btnAggiornaTutti.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.btnAggiornaTutti, null);
            this.btnAggiornaTutti.Name = "btnAggiornaTutti";
            this.guidaFile.SetShowHelp(this.btnAggiornaTutti, ((bool)(resources.GetObject("btnAggiornaTutti.ShowHelp"))));
            this.btnAggiornaTutti.UseVisualStyleBackColor = true;
            this.btnAggiornaTutti.Click += new System.EventHandler(this.btnAggiornaTutti_Click);
            // 
            // Aggiorna
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.btnVisualizza);
            this.Controls.Add(this.btnAggiornaTutti);
            this.Controls.Add(this.gridFile);
            this.Font = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.guidaFile.SetHelpKeyword(this, null);
            this.guidaFile.SetHelpNavigator(this, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("$this.HelpNavigator"))));
            this.guidaFile.SetHelpString(this, null);
            this.Name = "Aggiorna";
            this.guidaFile.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Controls.SetChildIndex(this.gridFile, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnCanc, 0);
            this.Controls.SetChildIndex(this.btnAggiornaTutti, 0);
            this.Controls.SetChildIndex(this.btnVisualizza, 0);
            ((System.ComponentModel.ISupportInitialize)(this.gridFile)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridFile;
        private System.Windows.Forms.Button btnAggiornaTutti;
        internal System.Windows.Forms.Button btnVisualizza;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNome;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTipo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVersioneAttuale;
        private System.Windows.Forms.DataGridViewTextBoxColumn colVersioneNuova;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDimensione;
        private System.Windows.Forms.DataGridViewComboBoxColumn colAzione;
    }
}