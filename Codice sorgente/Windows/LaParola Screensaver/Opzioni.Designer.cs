namespace LaParola_Screensaver
{
    partial class Opzioni
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
            if (disposing)
            {
                font.Dispose();
                if (components != null)
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Opzioni));
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.etiSfondoColore = new System.Windows.Forms.Label();
            this.pulSfondoColore = new System.Windows.Forms.Button();
            this.pulOK = new System.Windows.Forms.Button();
            this.pulAnnulla = new System.Windows.Forms.Button();
            this.etiFont = new System.Windows.Forms.Label();
            this.etiFontEsempio = new System.Windows.Forms.Label();
            this.pulFont = new System.Windows.Forms.Button();
            this.cbDirezione = new System.Windows.Forms.ComboBox();
            this.etiDirezione = new System.Windows.Forms.Label();
            this.tbVelocita = new System.Windows.Forms.TrackBar();
            this.etiVelocita = new System.Windows.Forms.Label();
            this.etiPosizione = new System.Windows.Forms.Label();
            this.cbPosizione = new System.Windows.Forms.ComboBox();
            this.etiBrano = new System.Windows.Forms.Label();
            this.etiRaggruppa = new System.Windows.Forms.Label();
            this.cbRaggruppa = new System.Windows.Forms.ComboBox();
            this.etiOrdine = new System.Windows.Forms.Label();
            this.tbBrano = new System.Windows.Forms.TextBox();
            this.cbOrdine = new System.Windows.Forms.ComboBox();
            this.etiVersione = new System.Windows.Forms.Label();
            this.cbVersione = new System.Windows.Forms.ComboBox();
            this.etiVelocita1 = new System.Windows.Forms.Label();
            this.etiVelocita2 = new System.Windows.Forms.Label();
            this.etiLingua = new System.Windows.Forms.Label();
            this.cbLingua = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.tbVelocita)).BeginInit();
            this.SuspendLayout();
            // 
            // etiSfondoColore
            // 
            this.etiSfondoColore.AutoSize = true;
            this.etiSfondoColore.Location = new System.Drawing.Point(14, 8);
            this.etiSfondoColore.Name = "etiSfondoColore";
            this.etiSfondoColore.Size = new System.Drawing.Size(100, 13);
            this.etiSfondoColore.TabIndex = 2;
            this.etiSfondoColore.Text = "Bac&kground colour:";
            // 
            // pulSfondoColore
            // 
            this.pulSfondoColore.Location = new System.Drawing.Point(162, 3);
            this.pulSfondoColore.Name = "pulSfondoColore";
            this.pulSfondoColore.Size = new System.Drawing.Size(29, 23);
            this.pulSfondoColore.TabIndex = 3;
            this.pulSfondoColore.Text = "...";
            this.pulSfondoColore.UseVisualStyleBackColor = true;
            this.pulSfondoColore.Click += new System.EventHandler(this.pulSfondoColore_Click);
            // 
            // pulOK
            // 
            this.pulOK.Location = new System.Drawing.Point(162, 404);
            this.pulOK.Name = "pulOK";
            this.pulOK.Size = new System.Drawing.Size(75, 23);
            this.pulOK.TabIndex = 0;
            this.pulOK.Text = "OK";
            this.pulOK.UseVisualStyleBackColor = true;
            this.pulOK.Click += new System.EventHandler(this.pulOK_Click);
            // 
            // pulAnnulla
            // 
            this.pulAnnulla.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.pulAnnulla.Location = new System.Drawing.Point(259, 404);
            this.pulAnnulla.Name = "pulAnnulla";
            this.pulAnnulla.Size = new System.Drawing.Size(75, 23);
            this.pulAnnulla.TabIndex = 1;
            this.pulAnnulla.Text = "Cancel";
            this.pulAnnulla.UseVisualStyleBackColor = true;
            this.pulAnnulla.Click += new System.EventHandler(this.pulAnnulla_Click);
            // 
            // etiFont
            // 
            this.etiFont.AutoSize = true;
            this.etiFont.Location = new System.Drawing.Point(14, 43);
            this.etiFont.Name = "etiFont";
            this.etiFont.Size = new System.Drawing.Size(31, 13);
            this.etiFont.TabIndex = 4;
            this.etiFont.Text = "Font:";
            // 
            // etiFontEsempio
            // 
            this.etiFontEsempio.AutoSize = true;
            this.etiFontEsempio.Location = new System.Drawing.Point(14, 56);
            this.etiFontEsempio.Name = "etiFontEsempio";
            this.etiFontEsempio.Size = new System.Drawing.Size(35, 13);
            this.etiFontEsempio.TabIndex = 6;
            this.etiFontEsempio.Text = "label1";
            // 
            // pulFont
            // 
            this.pulFont.Image = global::LaParola_Screensaver.Properties.Resources.fontstile;
            this.pulFont.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.pulFont.Location = new System.Drawing.Point(162, 38);
            this.pulFont.Name = "pulFont";
            this.pulFont.Size = new System.Drawing.Size(75, 23);
            this.pulFont.TabIndex = 5;
            this.pulFont.Text = "&Change";
            this.pulFont.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.pulFont.UseVisualStyleBackColor = true;
            this.pulFont.Click += new System.EventHandler(this.pulFont_Click);
            // 
            // cbDirezione
            // 
            this.cbDirezione.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDirezione.FormattingEnabled = true;
            this.cbDirezione.Items.AddRange(new object[] {
            "Horizontal",
            "Vertical"});
            this.cbDirezione.Location = new System.Drawing.Point(162, 221);
            this.cbDirezione.Name = "cbDirezione";
            this.cbDirezione.Size = new System.Drawing.Size(172, 21);
            this.cbDirezione.TabIndex = 14;
            this.cbDirezione.SelectedIndexChanged += new System.EventHandler(this.cbDirezione_SelectedIndexChanged);
            // 
            // etiDirezione
            // 
            this.etiDirezione.AutoSize = true;
            this.etiDirezione.Location = new System.Drawing.Point(14, 224);
            this.etiDirezione.Name = "etiDirezione";
            this.etiDirezione.Size = new System.Drawing.Size(52, 13);
            this.etiDirezione.TabIndex = 13;
            this.etiDirezione.Text = "&Direction:";
            // 
            // tbVelocita
            // 
            this.tbVelocita.LargeChange = 10;
            this.tbVelocita.Location = new System.Drawing.Point(154, 287);
            this.tbVelocita.Maximum = 100;
            this.tbVelocita.Name = "tbVelocita";
            this.tbVelocita.Size = new System.Drawing.Size(180, 45);
            this.tbVelocita.TabIndex = 18;
            this.tbVelocita.TickFrequency = 10;
            // 
            // etiVelocita
            // 
            this.etiVelocita.AutoSize = true;
            this.etiVelocita.Location = new System.Drawing.Point(14, 299);
            this.etiVelocita.Name = "etiVelocita";
            this.etiVelocita.Size = new System.Drawing.Size(41, 13);
            this.etiVelocita.TabIndex = 17;
            this.etiVelocita.Text = "&Speed:";
            // 
            // etiPosizione
            // 
            this.etiPosizione.AutoSize = true;
            this.etiPosizione.Location = new System.Drawing.Point(14, 259);
            this.etiPosizione.Name = "etiPosizione";
            this.etiPosizione.Size = new System.Drawing.Size(47, 13);
            this.etiPosizione.TabIndex = 15;
            this.etiPosizione.Text = "&Position:";
            // 
            // cbPosizione
            // 
            this.cbPosizione.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPosizione.FormattingEnabled = true;
            this.cbPosizione.Items.AddRange(new object[] {
            "Random",
            "Center"});
            this.cbPosizione.Location = new System.Drawing.Point(162, 256);
            this.cbPosizione.Name = "cbPosizione";
            this.cbPosizione.Size = new System.Drawing.Size(172, 21);
            this.cbPosizione.TabIndex = 16;
            // 
            // etiBrano
            // 
            this.etiBrano.AutoSize = true;
            this.etiBrano.Location = new System.Drawing.Point(14, 99);
            this.etiBrano.Name = "etiBrano";
            this.etiBrano.Size = new System.Drawing.Size(235, 13);
            this.etiBrano.TabIndex = 7;
            this.etiBrano.Text = "Part of the &Bible: (leave empty for all of the Bible)";
            // 
            // etiRaggruppa
            // 
            this.etiRaggruppa.AutoSize = true;
            this.etiRaggruppa.Location = new System.Drawing.Point(14, 154);
            this.etiRaggruppa.Name = "etiRaggruppa";
            this.etiRaggruppa.Size = new System.Drawing.Size(53, 13);
            this.etiRaggruppa.TabIndex = 9;
            this.etiRaggruppa.Text = "&Group by:";
            // 
            // cbRaggruppa
            // 
            this.cbRaggruppa.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRaggruppa.FormattingEnabled = true;
            this.cbRaggruppa.Items.AddRange(new object[] {
            "Verses",
            "Chapters"});
            this.cbRaggruppa.Location = new System.Drawing.Point(162, 151);
            this.cbRaggruppa.Name = "cbRaggruppa";
            this.cbRaggruppa.Size = new System.Drawing.Size(172, 21);
            this.cbRaggruppa.TabIndex = 10;
            // 
            // etiOrdine
            // 
            this.etiOrdine.AutoSize = true;
            this.etiOrdine.Location = new System.Drawing.Point(14, 189);
            this.etiOrdine.Name = "etiOrdine";
            this.etiOrdine.Size = new System.Drawing.Size(36, 13);
            this.etiOrdine.TabIndex = 11;
            this.etiOrdine.Text = "&Order:";
            // 
            // tbBrano
            // 
            this.tbBrano.Location = new System.Drawing.Point(162, 115);
            this.tbBrano.Name = "tbBrano";
            this.tbBrano.Size = new System.Drawing.Size(172, 20);
            this.tbBrano.TabIndex = 8;
            // 
            // cbOrdine
            // 
            this.cbOrdine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOrdine.FormattingEnabled = true;
            this.cbOrdine.Items.AddRange(new object[] {
            "Biblical order",
            "Random"});
            this.cbOrdine.Location = new System.Drawing.Point(162, 186);
            this.cbOrdine.Name = "cbOrdine";
            this.cbOrdine.Size = new System.Drawing.Size(172, 21);
            this.cbOrdine.TabIndex = 12;
            // 
            // etiVersione
            // 
            this.etiVersione.AutoSize = true;
            this.etiVersione.Location = new System.Drawing.Point(14, 341);
            this.etiVersione.Name = "etiVersione";
            this.etiVersione.Size = new System.Drawing.Size(62, 13);
            this.etiVersione.TabIndex = 21;
            this.etiVersione.Text = "&Translation:";
            // 
            // cbVersione
            // 
            this.cbVersione.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbVersione.FormattingEnabled = true;
            this.cbVersione.Location = new System.Drawing.Point(162, 338);
            this.cbVersione.Name = "cbVersione";
            this.cbVersione.Size = new System.Drawing.Size(172, 21);
            this.cbVersione.TabIndex = 22;
            // 
            // etiVelocita1
            // 
            this.etiVelocita1.AutoSize = true;
            this.etiVelocita1.Location = new System.Drawing.Point(135, 319);
            this.etiVelocita1.Name = "etiVelocita1";
            this.etiVelocita1.Size = new System.Drawing.Size(73, 13);
            this.etiVelocita1.TabIndex = 19;
            this.etiVelocita1.Text = "Stopped Slow";
            // 
            // etiVelocita2
            // 
            this.etiVelocita2.AutoSize = true;
            this.etiVelocita2.Location = new System.Drawing.Point(307, 319);
            this.etiVelocita2.Name = "etiVelocita2";
            this.etiVelocita2.Size = new System.Drawing.Size(27, 13);
            this.etiVelocita2.TabIndex = 20;
            this.etiVelocita2.Text = "Fast";
            // 
            // etiLingua
            // 
            this.etiLingua.AutoSize = true;
            this.etiLingua.Location = new System.Drawing.Point(14, 376);
            this.etiLingua.Name = "etiLingua";
            this.etiLingua.Size = new System.Drawing.Size(58, 13);
            this.etiLingua.TabIndex = 23;
            this.etiLingua.Text = "&Language:";
            // 
            // cbLingua
            // 
            this.cbLingua.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLingua.FormattingEnabled = true;
            this.cbLingua.Items.AddRange(new object[] {
            "English",
            "Italiano"});
            this.cbLingua.Location = new System.Drawing.Point(162, 373);
            this.cbLingua.Name = "cbLingua";
            this.cbLingua.Size = new System.Drawing.Size(172, 21);
            this.cbLingua.TabIndex = 24;
            this.cbLingua.SelectedIndexChanged += new System.EventHandler(this.cbLingua_SelectedIndexChanged);
            // 
            // Opzioni
            // 
            this.AcceptButton = this.pulOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.pulAnnulla;
            this.ClientSize = new System.Drawing.Size(346, 439);
            this.Controls.Add(this.cbLingua);
            this.Controls.Add(this.etiLingua);
            this.Controls.Add(this.etiVelocita2);
            this.Controls.Add(this.etiVelocita1);
            this.Controls.Add(this.cbVersione);
            this.Controls.Add(this.etiVersione);
            this.Controls.Add(this.cbOrdine);
            this.Controls.Add(this.tbBrano);
            this.Controls.Add(this.etiOrdine);
            this.Controls.Add(this.cbRaggruppa);
            this.Controls.Add(this.etiRaggruppa);
            this.Controls.Add(this.etiBrano);
            this.Controls.Add(this.cbPosizione);
            this.Controls.Add(this.etiPosizione);
            this.Controls.Add(this.etiVelocita);
            this.Controls.Add(this.tbVelocita);
            this.Controls.Add(this.etiDirezione);
            this.Controls.Add(this.cbDirezione);
            this.Controls.Add(this.pulFont);
            this.Controls.Add(this.etiFontEsempio);
            this.Controls.Add(this.etiFont);
            this.Controls.Add(this.pulAnnulla);
            this.Controls.Add(this.pulOK);
            this.Controls.Add(this.pulSfondoColore);
            this.Controls.Add(this.etiSfondoColore);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Opzioni";
            this.Text = "LaParola";
            this.Load += new System.EventHandler(this.Opzioni_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Opzioni_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.tbVelocita)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.Label etiSfondoColore;
        private System.Windows.Forms.Button pulSfondoColore;
        private System.Windows.Forms.Button pulOK;
        private System.Windows.Forms.Button pulAnnulla;
        private System.Windows.Forms.Label etiFont;
        private System.Windows.Forms.Label etiFontEsempio;
        private System.Windows.Forms.Button pulFont;
        private System.Windows.Forms.ComboBox cbDirezione;
        private System.Windows.Forms.Label etiDirezione;
        private System.Windows.Forms.TrackBar tbVelocita;
        private System.Windows.Forms.Label etiVelocita;
        private System.Windows.Forms.Label etiPosizione;
        private System.Windows.Forms.ComboBox cbPosizione;
        private System.Windows.Forms.Label etiBrano;
        private System.Windows.Forms.Label etiRaggruppa;
        private System.Windows.Forms.ComboBox cbRaggruppa;
        private System.Windows.Forms.Label etiOrdine;
        private System.Windows.Forms.TextBox tbBrano;
        private System.Windows.Forms.ComboBox cbOrdine;
        private System.Windows.Forms.Label etiVersione;
        private System.Windows.Forms.ComboBox cbVersione;
        private System.Windows.Forms.Label etiVelocita1;
        private System.Windows.Forms.Label etiVelocita2;
        private System.Windows.Forms.Label etiLingua;
        private System.Windows.Forms.ComboBox cbLingua;
    }
}