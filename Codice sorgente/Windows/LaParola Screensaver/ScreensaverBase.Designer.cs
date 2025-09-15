namespace LaParola_Screensaver
{
    partial class ScreensaverBase
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
            this.rtDummy = new System.Windows.Forms.RichTextBox();
            this.rtTesto = new TestiBiblici.RichTextBoxEx();
            this.SuspendLayout();
            // 
            // rtDummy
            // 
            this.rtDummy.Location = new System.Drawing.Point(172, 144);
            this.rtDummy.Name = "rtDummy";
            this.rtDummy.Size = new System.Drawing.Size(100, 96);
            this.rtDummy.TabIndex = 1;
            this.rtDummy.Text = "";
            this.rtDummy.Visible = false;
            this.rtDummy.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rtTesto_KeyDown);
            this.rtDummy.MouseMove += new System.Windows.Forms.MouseEventHandler(this.rtTesto_MouseMove);
            this.rtDummy.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rtTesto_MouseDown);
            // 
            // rtTesto
            // 
            this.rtTesto.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtTesto.Cursor = System.Windows.Forms.Cursors.Default;
            this.rtTesto.Lingua = null;
            this.rtTesto.Location = new System.Drawing.Point(38, 46);
            this.rtTesto.Name = "rtTesto";
            this.rtTesto.ReadOnly = true;
            this.rtTesto.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.rtTesto.SelectionAlignment = TestiBiblici.RichTextBoxEx.TextAlign.Left;
            this.rtTesto.Size = new System.Drawing.Size(190, 172);
            this.rtTesto.TabIndex = 0;
            this.rtTesto.TabStop = false;
            this.rtTesto.Text = "";
            this.rtTesto.Versione = null;
            this.rtTesto.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rtTesto_KeyDown);
            this.rtTesto.MouseMove += new System.Windows.Forms.MouseEventHandler(this.rtTesto_MouseMove);
            this.rtTesto.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rtTesto_MouseDown);
            // 
            // ScreensaverBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 264);
            this.Controls.Add(this.rtDummy);
            this.Controls.Add(this.rtTesto);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ScreensaverBase";
            this.ShowInTaskbar = false;
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.rtTesto_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.rtTesto_MouseMove);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.rtTesto_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtDummy;
        protected TestiBiblici.RichTextBoxEx rtTesto;

    }
}