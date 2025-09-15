namespace LaParola
{
    partial class InformazioniSu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InformazioniSu));
            this.immLogo = new System.Windows.Forms.PictureBox();
            this.okButton = new System.Windows.Forms.Button();
            this.labelDescription = new System.Windows.Forms.LinkLabel();
            this.labelCopyright = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.labelProductName = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.immLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // immLogo
            // 
            this.immLogo.AccessibleDescription = null;
            this.immLogo.AccessibleName = null;
            resources.ApplyResources(this.immLogo, "immLogo");
            this.immLogo.BackgroundImage = null;
            this.immLogo.Font = null;
            this.immLogo.Image = global::LaParola.Properties.Resources.bibbia;
            this.immLogo.ImageLocation = null;
            this.immLogo.Name = "immLogo";
            this.immLogo.TabStop = false;
            // 
            // okButton
            // 
            this.okButton.AccessibleDescription = null;
            this.okButton.AccessibleName = null;
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.BackgroundImage = null;
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okButton.Font = null;
            this.okButton.Name = "okButton";
            // 
            // labelDescription
            // 
            this.labelDescription.AccessibleDescription = null;
            this.labelDescription.AccessibleName = null;
            resources.ApplyResources(this.labelDescription, "labelDescription");
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.TabStop = true;
            this.labelDescription.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.labelDescription_LinkClicked);
            // 
            // labelCopyright
            // 
            this.labelCopyright.AccessibleDescription = null;
            this.labelCopyright.AccessibleName = null;
            resources.ApplyResources(this.labelCopyright, "labelCopyright");
            this.labelCopyright.Name = "labelCopyright";
            // 
            // labelVersion
            // 
            this.labelVersion.AccessibleDescription = null;
            this.labelVersion.AccessibleName = null;
            resources.ApplyResources(this.labelVersion, "labelVersion");
            this.labelVersion.Name = "labelVersion";
            // 
            // labelProductName
            // 
            this.labelProductName.AccessibleDescription = null;
            this.labelProductName.AccessibleName = null;
            resources.ApplyResources(this.labelProductName, "labelProductName");
            this.labelProductName.Name = "labelProductName";
            // 
            // InformazioniSu
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.labelProductName);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.labelCopyright);
            this.Controls.Add(this.labelDescription);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.immLogo);
            this.Font = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InformazioniSu";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)(this.immLogo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox immLogo;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.LinkLabel labelDescription;
        private System.Windows.Forms.Label labelCopyright;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Label labelProductName;
    }
}
