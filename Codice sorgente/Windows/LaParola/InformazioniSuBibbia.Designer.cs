namespace LaParola
{
    partial class InformazioniSuBibbia
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
            this.labTitolo = new System.Windows.Forms.Label();
            this.rtAltreInfo = new System.Windows.Forms.RichTextBox();
            this.labCopyright = new System.Windows.Forms.Label();
            this.labData = new System.Windows.Forms.Label();
            this.labCasaEditrice = new System.Windows.Forms.Label();
            this.labISBN = new System.Windows.Forms.Label();
            this.labAutore = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(180, 229);
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCanc
            // 
            this.btnCanc.Location = new System.Drawing.Point(271, 229);
            this.btnCanc.Visible = false;
            // 
            // labTitolo
            // 
            this.labTitolo.AutoSize = true;
            this.labTitolo.Location = new System.Drawing.Point(9, 9);
            this.labTitolo.Name = "labTitolo";
            this.labTitolo.Size = new System.Drawing.Size(0, 13);
            this.labTitolo.TabIndex = 2;
            this.labTitolo.UseMnemonic = false;
            // 
            // rtAltreInfo
            // 
            this.rtAltreInfo.Location = new System.Drawing.Point(12, 117);
            this.rtAltreInfo.Name = "rtAltreInfo";
            this.rtAltreInfo.ReadOnly = true;
            this.rtAltreInfo.Size = new System.Drawing.Size(336, 108);
            this.rtAltreInfo.TabIndex = 8;
            this.rtAltreInfo.Text = "";
            this.rtAltreInfo.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtAltreInfo_LinkClicked);
            // 
            // labCopyright
            // 
            this.labCopyright.AutoSize = true;
            this.labCopyright.Location = new System.Drawing.Point(9, 99);
            this.labCopyright.Name = "labCopyright";
            this.labCopyright.Size = new System.Drawing.Size(0, 13);
            this.labCopyright.TabIndex = 7;
            this.labCopyright.UseMnemonic = false;
            // 
            // labData
            // 
            this.labData.AutoSize = true;
            this.labData.Location = new System.Drawing.Point(9, 63);
            this.labData.Name = "labData";
            this.labData.Size = new System.Drawing.Size(0, 13);
            this.labData.TabIndex = 5;
            this.labData.UseMnemonic = false;
            // 
            // labCasaEditrice
            // 
            this.labCasaEditrice.AutoSize = true;
            this.labCasaEditrice.Location = new System.Drawing.Point(9, 45);
            this.labCasaEditrice.Name = "labCasaEditrice";
            this.labCasaEditrice.Size = new System.Drawing.Size(0, 13);
            this.labCasaEditrice.TabIndex = 4;
            this.labCasaEditrice.UseMnemonic = false;
            // 
            // labISBN
            // 
            this.labISBN.AutoSize = true;
            this.labISBN.Location = new System.Drawing.Point(9, 81);
            this.labISBN.Name = "labISBN";
            this.labISBN.Size = new System.Drawing.Size(0, 13);
            this.labISBN.TabIndex = 6;
            this.labISBN.UseMnemonic = false;
            // 
            // labAutore
            // 
            this.labAutore.AutoSize = true;
            this.labAutore.Location = new System.Drawing.Point(9, 27);
            this.labAutore.Name = "labAutore";
            this.labAutore.Size = new System.Drawing.Size(0, 13);
            this.labAutore.TabIndex = 3;
            this.labAutore.UseMnemonic = false;
            // 
            // InformazioniSuBibbia
            // 
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(358, 264);
            this.Controls.Add(this.labAutore);
            this.Controls.Add(this.labISBN);
            this.Controls.Add(this.labCasaEditrice);
            this.Controls.Add(this.rtAltreInfo);
            this.Controls.Add(this.labTitolo);
            this.Controls.Add(this.labData);
            this.Controls.Add(this.labCopyright);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MinimumSize = new System.Drawing.Size(366, 291);
            this.Name = "InformazioniSuBibbia";
            this.Resize += new System.EventHandler(this.InformazioniSuBibbia_Resize);
            this.Controls.SetChildIndex(this.labCopyright, 0);
            this.Controls.SetChildIndex(this.labData, 0);
            this.Controls.SetChildIndex(this.labTitolo, 0);
            this.Controls.SetChildIndex(this.rtAltreInfo, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnCanc, 0);
            this.Controls.SetChildIndex(this.labCasaEditrice, 0);
            this.Controls.SetChildIndex(this.labISBN, 0);
            this.Controls.SetChildIndex(this.labAutore, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labTitolo;
        private System.Windows.Forms.RichTextBox rtAltreInfo;
        private System.Windows.Forms.Label labCopyright;
        private System.Windows.Forms.Label labData;
        private System.Windows.Forms.Label labCasaEditrice;
        private System.Windows.Forms.Label labISBN;
        private System.Windows.Forms.Label labAutore;
    }
}