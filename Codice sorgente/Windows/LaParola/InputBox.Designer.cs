namespace LaParola
{
    partial class InputBox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InputBox));
            this.tbRisposta = new System.Windows.Forms.TextBox();
            this.etiDomanda = new System.Windows.Forms.Label();
            this.pulOK = new System.Windows.Forms.Button();
            this.pulCanc = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbRisposta
            // 
            this.tbRisposta.AccessibleDescription = null;
            this.tbRisposta.AccessibleName = null;
            resources.ApplyResources(this.tbRisposta, "tbRisposta");
            this.tbRisposta.BackgroundImage = null;
            this.tbRisposta.Font = null;
            this.tbRisposta.Name = "tbRisposta";
            // 
            // etiDomanda
            // 
            this.etiDomanda.AccessibleDescription = null;
            this.etiDomanda.AccessibleName = null;
            resources.ApplyResources(this.etiDomanda, "etiDomanda");
            this.etiDomanda.Font = null;
            this.etiDomanda.Name = "etiDomanda";
            // 
            // pulOK
            // 
            this.pulOK.AccessibleDescription = null;
            this.pulOK.AccessibleName = null;
            resources.ApplyResources(this.pulOK, "pulOK");
            this.pulOK.BackgroundImage = null;
            this.pulOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.pulOK.Font = null;
            this.pulOK.Name = "pulOK";
            this.pulOK.UseVisualStyleBackColor = true;
            this.pulOK.Click += new System.EventHandler(this.pulOK_Click);
            // 
            // pulCanc
            // 
            this.pulCanc.AccessibleDescription = null;
            this.pulCanc.AccessibleName = null;
            resources.ApplyResources(this.pulCanc, "pulCanc");
            this.pulCanc.BackgroundImage = null;
            this.pulCanc.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.pulCanc.Font = null;
            this.pulCanc.Name = "pulCanc";
            this.pulCanc.UseVisualStyleBackColor = true;
            // 
            // InputBox
            // 
            this.AcceptButton = this.pulOK;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.CancelButton = this.pulCanc;
            this.Controls.Add(this.pulCanc);
            this.Controls.Add(this.pulOK);
            this.Controls.Add(this.etiDomanda);
            this.Controls.Add(this.tbRisposta);
            this.Font = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InputBox";
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbRisposta;
        private System.Windows.Forms.Label etiDomanda;
        private System.Windows.Forms.Button pulOK;
        private System.Windows.Forms.Button pulCanc;
    }
}