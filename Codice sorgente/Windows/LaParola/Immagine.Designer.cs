namespace LaParola
{
    partial class Immagine
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
            this.pbImmagine = new System.Windows.Forms.PictureBox();
            this.pmZoom = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pbImmagine)).BeginInit();
            this.pmZoom.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Visible = false;
            // 
            // btnCanc
            // 
            this.btnCanc.Visible = false;
            // 
            // pbImmagine
            // 
            this.pbImmagine.ContextMenuStrip = this.pmZoom;
            this.pbImmagine.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbImmagine.Location = new System.Drawing.Point(0, 0);
            this.pbImmagine.Name = "pbImmagine";
            this.pbImmagine.Size = new System.Drawing.Size(360, 266);
            this.pbImmagine.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbImmagine.TabIndex = 2;
            this.pbImmagine.TabStop = false;
            this.pbImmagine.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbImmagine_MouseMove);
            this.pbImmagine.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbImmagine_MouseUp);
            // 
            // pmZoom
            // 
            this.pmZoom.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4,
            this.toolStripMenuItem5,
            this.toolStripMenuItem6});
            this.pmZoom.Name = "pmZoom";
            this.pmZoom.Size = new System.Drawing.Size(104, 114);
            this.pmZoom.Opening += new System.ComponentModel.CancelEventHandler(this.pmZoom_Opening);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(103, 22);
            this.toolStripMenuItem2.Text = "2&5%";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.pmZoomVoce_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(103, 22);
            this.toolStripMenuItem3.Text = "5&0%";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.pmZoomVoce_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(103, 22);
            this.toolStripMenuItem4.Text = "&75%";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.pmZoomVoce_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(103, 22);
            this.toolStripMenuItem5.Text = "&100%";
            this.toolStripMenuItem5.Click += new System.EventHandler(this.pmZoomVoce_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(103, 22);
            this.toolStripMenuItem6.Text = "&200%";
            this.toolStripMenuItem6.Click += new System.EventHandler(this.pmZoomVoce_Click);
            // 
            // Immagine
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(360, 266);
            this.Controls.Add(this.pbImmagine);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.Name = "Immagine";
            this.Tag = "Immagine";
            this.Text = "Immagine";
            this.Load += new System.EventHandler(this.Immagine_Load);
            this.Controls.SetChildIndex(this.pbImmagine, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnCanc, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pbImmagine)).EndInit();
            this.pmZoom.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbImmagine;
        private System.Windows.Forms.ContextMenuStrip pmZoom;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
    }
}