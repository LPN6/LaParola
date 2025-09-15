namespace LaParola
{
    partial class Template
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Template));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCanc = new System.Windows.Forms.Button();
            this.guidaFile = new System.Windows.Forms.HelpProvider();
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
            this.btnOK.Name = "btnOK";
            this.guidaFile.SetShowHelp(this.btnOK, ((bool)(resources.GetObject("btnOK.ShowHelp"))));
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCanc
            // 
            this.btnCanc.AccessibleDescription = null;
            this.btnCanc.AccessibleName = null;
            resources.ApplyResources(this.btnCanc, "btnCanc");
            this.btnCanc.BackgroundImage = null;
            this.btnCanc.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCanc.Font = null;
            this.guidaFile.SetHelpKeyword(this.btnCanc, null);
            this.guidaFile.SetHelpNavigator(this.btnCanc, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("btnCanc.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.btnCanc, null);
            this.btnCanc.Name = "btnCanc";
            this.guidaFile.SetShowHelp(this.btnCanc, ((bool)(resources.GetObject("btnCanc.ShowHelp"))));
            this.btnCanc.UseVisualStyleBackColor = true;
            // 
            // guidaFile
            // 
            this.guidaFile.HelpNamespace = null;
            // 
            // Template
            // 
            this.AcceptButton = this.btnOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.CancelButton = this.btnCanc;
            this.Controls.Add(this.btnCanc);
            this.Controls.Add(this.btnOK);
            this.Font = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.guidaFile.SetHelpKeyword(this, null);
            this.guidaFile.SetHelpNavigator(this, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("$this.HelpNavigator"))));
            this.guidaFile.SetHelpString(this, null);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Template";
            this.guidaFile.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Button btnOK;
        protected System.Windows.Forms.Button btnCanc;
        protected System.Windows.Forms.HelpProvider guidaFile;

    }
}