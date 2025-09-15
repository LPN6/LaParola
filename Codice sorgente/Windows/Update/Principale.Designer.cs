namespace Update
{
    partial class Principale
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Principale));
            this.etiMessaggio = new System.Windows.Forms.Label();
            this.pulChiudi = new System.Windows.Forms.Button();
            this.pulOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // etiMessaggio
            // 
            this.etiMessaggio.AccessibleDescription = null;
            this.etiMessaggio.AccessibleName = null;
            resources.ApplyResources(this.etiMessaggio, "etiMessaggio");
            this.etiMessaggio.Name = "etiMessaggio";
            // 
            // pulChiudi
            // 
            this.pulChiudi.AccessibleDescription = null;
            this.pulChiudi.AccessibleName = null;
            resources.ApplyResources(this.pulChiudi, "pulChiudi");
            this.pulChiudi.BackgroundImage = null;
            this.pulChiudi.Font = null;
            this.pulChiudi.Name = "pulChiudi";
            this.pulChiudi.UseVisualStyleBackColor = true;
            this.pulChiudi.Click += new System.EventHandler(this.pulChiudi_Click);
            // 
            // pulOK
            // 
            this.pulOK.AccessibleDescription = null;
            this.pulOK.AccessibleName = null;
            resources.ApplyResources(this.pulOK, "pulOK");
            this.pulOK.BackgroundImage = null;
            this.pulOK.Font = null;
            this.pulOK.Name = "pulOK";
            this.pulOK.UseVisualStyleBackColor = true;
            this.pulOK.Click += new System.EventHandler(this.pulOK_Click);
            // 
            // Principale
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.pulOK);
            this.Controls.Add(this.pulChiudi);
            this.Controls.Add(this.etiMessaggio);
            this.Font = null;
            this.Name = "Principale";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Shown += new System.EventHandler(this.Principale_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label etiMessaggio;
        private System.Windows.Forms.Button pulChiudi;
        private System.Windows.Forms.Button pulOK;
    }
}

