namespace LaParola
{
    partial class Misure
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Misure));
            this.etiPesi = new System.Windows.Forms.Label();
            this.tbPesi1 = new System.Windows.Forms.TextBox();
            this.cbPesi1 = new System.Windows.Forms.ComboBox();
            this.etiPesi2 = new System.Windows.Forms.Label();
            this.cbPesi2 = new System.Windows.Forms.ComboBox();
            this.etiPesiUguale = new System.Windows.Forms.Label();
            this.etiLunghezze = new System.Windows.Forms.Label();
            this.tbLunghezze1 = new System.Windows.Forms.TextBox();
            this.cbLunghezze1 = new System.Windows.Forms.ComboBox();
            this.cbLunghezze2 = new System.Windows.Forms.ComboBox();
            this.etiLunghezze2 = new System.Windows.Forms.Label();
            this.etiLunghezzeUguali = new System.Windows.Forms.Label();
            this.etiCapacita = new System.Windows.Forms.Label();
            this.tbCapacita1 = new System.Windows.Forms.TextBox();
            this.cbCapacita1 = new System.Windows.Forms.ComboBox();
            this.cbCapacita2 = new System.Windows.Forms.ComboBox();
            this.etiCapacita2 = new System.Windows.Forms.Label();
            this.etiCapacitaUguali = new System.Windows.Forms.Label();
            this.etiMonete = new System.Windows.Forms.Label();
            this.tbMonete1 = new System.Windows.Forms.TextBox();
            this.cbMonete1 = new System.Windows.Forms.ComboBox();
            this.cbMonete2 = new System.Windows.Forms.ComboBox();
            this.etiMonete2 = new System.Windows.Forms.Label();
            this.etiMoneteUguali = new System.Windows.Forms.Label();
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
            this.btnCanc.Click += new System.EventHandler(this.btnCanc_Click);
            // 
            // guidaFile
            // 
            this.guidaFile.HelpNamespace = null;
            // 
            // etiPesi
            // 
            this.etiPesi.AccessibleDescription = null;
            this.etiPesi.AccessibleName = null;
            resources.ApplyResources(this.etiPesi, "etiPesi");
            this.etiPesi.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiPesi, null);
            this.guidaFile.SetHelpNavigator(this.etiPesi, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiPesi.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiPesi, null);
            this.etiPesi.Name = "etiPesi";
            this.guidaFile.SetShowHelp(this.etiPesi, ((bool)(resources.GetObject("etiPesi.ShowHelp"))));
            // 
            // tbPesi1
            // 
            this.tbPesi1.AccessibleDescription = null;
            this.tbPesi1.AccessibleName = null;
            resources.ApplyResources(this.tbPesi1, "tbPesi1");
            this.tbPesi1.BackgroundImage = null;
            this.tbPesi1.Font = null;
            this.guidaFile.SetHelpKeyword(this.tbPesi1, resources.GetString("tbPesi1.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.tbPesi1, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("tbPesi1.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.tbPesi1, null);
            this.tbPesi1.Name = "tbPesi1";
            this.guidaFile.SetShowHelp(this.tbPesi1, ((bool)(resources.GetObject("tbPesi1.ShowHelp"))));
            this.tbPesi1.Tag = "Pesi";
            this.tbPesi1.TextChanged += new System.EventHandler(this.ConversionChanged);
            this.tbPesi1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tb1_KeyPress);
            // 
            // cbPesi1
            // 
            this.cbPesi1.AccessibleDescription = null;
            this.cbPesi1.AccessibleName = null;
            resources.ApplyResources(this.cbPesi1, "cbPesi1");
            this.cbPesi1.BackgroundImage = null;
            this.cbPesi1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPesi1.Font = null;
            this.cbPesi1.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.cbPesi1, resources.GetString("cbPesi1.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbPesi1, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbPesi1.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbPesi1, null);
            this.cbPesi1.Items.AddRange(new object[] {
            resources.GetString("cbPesi1.Items"),
            resources.GetString("cbPesi1.Items1"),
            resources.GetString("cbPesi1.Items2"),
            resources.GetString("cbPesi1.Items3"),
            resources.GetString("cbPesi1.Items4"),
            resources.GetString("cbPesi1.Items5"),
            resources.GetString("cbPesi1.Items6"),
            resources.GetString("cbPesi1.Items7")});
            this.cbPesi1.Name = "cbPesi1";
            this.guidaFile.SetShowHelp(this.cbPesi1, ((bool)(resources.GetObject("cbPesi1.ShowHelp"))));
            this.cbPesi1.Tag = "Pesi";
            this.cbPesi1.SelectedIndexChanged += new System.EventHandler(this.ConversionChanged);
            // 
            // etiPesi2
            // 
            this.etiPesi2.AccessibleDescription = null;
            this.etiPesi2.AccessibleName = null;
            resources.ApplyResources(this.etiPesi2, "etiPesi2");
            this.etiPesi2.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiPesi2, null);
            this.guidaFile.SetHelpNavigator(this.etiPesi2, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiPesi2.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiPesi2, null);
            this.etiPesi2.Name = "etiPesi2";
            this.guidaFile.SetShowHelp(this.etiPesi2, ((bool)(resources.GetObject("etiPesi2.ShowHelp"))));
            // 
            // cbPesi2
            // 
            this.cbPesi2.AccessibleDescription = null;
            this.cbPesi2.AccessibleName = null;
            resources.ApplyResources(this.cbPesi2, "cbPesi2");
            this.cbPesi2.BackgroundImage = null;
            this.cbPesi2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPesi2.Font = null;
            this.cbPesi2.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.cbPesi2, resources.GetString("cbPesi2.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbPesi2, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbPesi2.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbPesi2, null);
            this.cbPesi2.Name = "cbPesi2";
            this.guidaFile.SetShowHelp(this.cbPesi2, ((bool)(resources.GetObject("cbPesi2.ShowHelp"))));
            this.cbPesi2.Tag = "Pesi";
            this.cbPesi2.SelectedIndexChanged += new System.EventHandler(this.ConversionChanged);
            // 
            // etiPesiUguale
            // 
            this.etiPesiUguale.AccessibleDescription = null;
            this.etiPesiUguale.AccessibleName = null;
            resources.ApplyResources(this.etiPesiUguale, "etiPesiUguale");
            this.guidaFile.SetHelpKeyword(this.etiPesiUguale, null);
            this.guidaFile.SetHelpNavigator(this.etiPesiUguale, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiPesiUguale.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiPesiUguale, null);
            this.etiPesiUguale.Name = "etiPesiUguale";
            this.guidaFile.SetShowHelp(this.etiPesiUguale, ((bool)(resources.GetObject("etiPesiUguale.ShowHelp"))));
            // 
            // etiLunghezze
            // 
            this.etiLunghezze.AccessibleDescription = null;
            this.etiLunghezze.AccessibleName = null;
            resources.ApplyResources(this.etiLunghezze, "etiLunghezze");
            this.etiLunghezze.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiLunghezze, null);
            this.guidaFile.SetHelpNavigator(this.etiLunghezze, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiLunghezze.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiLunghezze, null);
            this.etiLunghezze.Name = "etiLunghezze";
            this.guidaFile.SetShowHelp(this.etiLunghezze, ((bool)(resources.GetObject("etiLunghezze.ShowHelp"))));
            // 
            // tbLunghezze1
            // 
            this.tbLunghezze1.AccessibleDescription = null;
            this.tbLunghezze1.AccessibleName = null;
            resources.ApplyResources(this.tbLunghezze1, "tbLunghezze1");
            this.tbLunghezze1.BackgroundImage = null;
            this.tbLunghezze1.Font = null;
            this.guidaFile.SetHelpKeyword(this.tbLunghezze1, resources.GetString("tbLunghezze1.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.tbLunghezze1, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("tbLunghezze1.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.tbLunghezze1, null);
            this.tbLunghezze1.Name = "tbLunghezze1";
            this.guidaFile.SetShowHelp(this.tbLunghezze1, ((bool)(resources.GetObject("tbLunghezze1.ShowHelp"))));
            this.tbLunghezze1.Tag = "Lunghezze";
            this.tbLunghezze1.TextChanged += new System.EventHandler(this.ConversionChanged);
            this.tbLunghezze1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tb1_KeyPress);
            // 
            // cbLunghezze1
            // 
            this.cbLunghezze1.AccessibleDescription = null;
            this.cbLunghezze1.AccessibleName = null;
            resources.ApplyResources(this.cbLunghezze1, "cbLunghezze1");
            this.cbLunghezze1.BackgroundImage = null;
            this.cbLunghezze1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLunghezze1.Font = null;
            this.cbLunghezze1.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.cbLunghezze1, resources.GetString("cbLunghezze1.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbLunghezze1, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbLunghezze1.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbLunghezze1, null);
            this.cbLunghezze1.Items.AddRange(new object[] {
            resources.GetString("cbLunghezze1.Items"),
            resources.GetString("cbLunghezze1.Items1"),
            resources.GetString("cbLunghezze1.Items2"),
            resources.GetString("cbLunghezze1.Items3"),
            resources.GetString("cbLunghezze1.Items4"),
            resources.GetString("cbLunghezze1.Items5"),
            resources.GetString("cbLunghezze1.Items6"),
            resources.GetString("cbLunghezze1.Items7"),
            resources.GetString("cbLunghezze1.Items8"),
            resources.GetString("cbLunghezze1.Items9"),
            resources.GetString("cbLunghezze1.Items10")});
            this.cbLunghezze1.Name = "cbLunghezze1";
            this.guidaFile.SetShowHelp(this.cbLunghezze1, ((bool)(resources.GetObject("cbLunghezze1.ShowHelp"))));
            this.cbLunghezze1.Tag = "Lunghezze";
            this.cbLunghezze1.SelectedIndexChanged += new System.EventHandler(this.ConversionChanged);
            // 
            // cbLunghezze2
            // 
            this.cbLunghezze2.AccessibleDescription = null;
            this.cbLunghezze2.AccessibleName = null;
            resources.ApplyResources(this.cbLunghezze2, "cbLunghezze2");
            this.cbLunghezze2.BackgroundImage = null;
            this.cbLunghezze2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLunghezze2.Font = null;
            this.cbLunghezze2.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.cbLunghezze2, resources.GetString("cbLunghezze2.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbLunghezze2, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbLunghezze2.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbLunghezze2, null);
            this.cbLunghezze2.Name = "cbLunghezze2";
            this.guidaFile.SetShowHelp(this.cbLunghezze2, ((bool)(resources.GetObject("cbLunghezze2.ShowHelp"))));
            this.cbLunghezze2.Tag = "Lunghezze";
            this.cbLunghezze2.SelectedIndexChanged += new System.EventHandler(this.ConversionChanged);
            // 
            // etiLunghezze2
            // 
            this.etiLunghezze2.AccessibleDescription = null;
            this.etiLunghezze2.AccessibleName = null;
            resources.ApplyResources(this.etiLunghezze2, "etiLunghezze2");
            this.etiLunghezze2.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiLunghezze2, null);
            this.guidaFile.SetHelpNavigator(this.etiLunghezze2, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiLunghezze2.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiLunghezze2, null);
            this.etiLunghezze2.Name = "etiLunghezze2";
            this.guidaFile.SetShowHelp(this.etiLunghezze2, ((bool)(resources.GetObject("etiLunghezze2.ShowHelp"))));
            // 
            // etiLunghezzeUguali
            // 
            this.etiLunghezzeUguali.AccessibleDescription = null;
            this.etiLunghezzeUguali.AccessibleName = null;
            resources.ApplyResources(this.etiLunghezzeUguali, "etiLunghezzeUguali");
            this.guidaFile.SetHelpKeyword(this.etiLunghezzeUguali, null);
            this.guidaFile.SetHelpNavigator(this.etiLunghezzeUguali, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiLunghezzeUguali.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiLunghezzeUguali, null);
            this.etiLunghezzeUguali.Name = "etiLunghezzeUguali";
            this.guidaFile.SetShowHelp(this.etiLunghezzeUguali, ((bool)(resources.GetObject("etiLunghezzeUguali.ShowHelp"))));
            // 
            // etiCapacita
            // 
            this.etiCapacita.AccessibleDescription = null;
            this.etiCapacita.AccessibleName = null;
            resources.ApplyResources(this.etiCapacita, "etiCapacita");
            this.etiCapacita.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiCapacita, null);
            this.guidaFile.SetHelpNavigator(this.etiCapacita, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiCapacita.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiCapacita, null);
            this.etiCapacita.Name = "etiCapacita";
            this.guidaFile.SetShowHelp(this.etiCapacita, ((bool)(resources.GetObject("etiCapacita.ShowHelp"))));
            // 
            // tbCapacita1
            // 
            this.tbCapacita1.AccessibleDescription = null;
            this.tbCapacita1.AccessibleName = null;
            resources.ApplyResources(this.tbCapacita1, "tbCapacita1");
            this.tbCapacita1.BackgroundImage = null;
            this.tbCapacita1.Font = null;
            this.guidaFile.SetHelpKeyword(this.tbCapacita1, resources.GetString("tbCapacita1.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.tbCapacita1, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("tbCapacita1.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.tbCapacita1, null);
            this.tbCapacita1.Name = "tbCapacita1";
            this.guidaFile.SetShowHelp(this.tbCapacita1, ((bool)(resources.GetObject("tbCapacita1.ShowHelp"))));
            this.tbCapacita1.Tag = "Capacita";
            this.tbCapacita1.TextChanged += new System.EventHandler(this.ConversionChanged);
            this.tbCapacita1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tb1_KeyPress);
            // 
            // cbCapacita1
            // 
            this.cbCapacita1.AccessibleDescription = null;
            this.cbCapacita1.AccessibleName = null;
            resources.ApplyResources(this.cbCapacita1, "cbCapacita1");
            this.cbCapacita1.BackgroundImage = null;
            this.cbCapacita1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCapacita1.Font = null;
            this.cbCapacita1.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.cbCapacita1, resources.GetString("cbCapacita1.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbCapacita1, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbCapacita1.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbCapacita1, null);
            this.cbCapacita1.Items.AddRange(new object[] {
            resources.GetString("cbCapacita1.Items"),
            resources.GetString("cbCapacita1.Items1"),
            resources.GetString("cbCapacita1.Items2"),
            resources.GetString("cbCapacita1.Items3"),
            resources.GetString("cbCapacita1.Items4"),
            resources.GetString("cbCapacita1.Items5"),
            resources.GetString("cbCapacita1.Items6"),
            resources.GetString("cbCapacita1.Items7"),
            resources.GetString("cbCapacita1.Items8")});
            this.cbCapacita1.Name = "cbCapacita1";
            this.guidaFile.SetShowHelp(this.cbCapacita1, ((bool)(resources.GetObject("cbCapacita1.ShowHelp"))));
            this.cbCapacita1.Tag = "Capacita";
            this.cbCapacita1.SelectedIndexChanged += new System.EventHandler(this.ConversionChanged);
            // 
            // cbCapacita2
            // 
            this.cbCapacita2.AccessibleDescription = null;
            this.cbCapacita2.AccessibleName = null;
            resources.ApplyResources(this.cbCapacita2, "cbCapacita2");
            this.cbCapacita2.BackgroundImage = null;
            this.cbCapacita2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCapacita2.Font = null;
            this.cbCapacita2.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.cbCapacita2, resources.GetString("cbCapacita2.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbCapacita2, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbCapacita2.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbCapacita2, null);
            this.cbCapacita2.Name = "cbCapacita2";
            this.guidaFile.SetShowHelp(this.cbCapacita2, ((bool)(resources.GetObject("cbCapacita2.ShowHelp"))));
            this.cbCapacita2.Tag = "Capacita";
            this.cbCapacita2.SelectedIndexChanged += new System.EventHandler(this.ConversionChanged);
            // 
            // etiCapacita2
            // 
            this.etiCapacita2.AccessibleDescription = null;
            this.etiCapacita2.AccessibleName = null;
            resources.ApplyResources(this.etiCapacita2, "etiCapacita2");
            this.etiCapacita2.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiCapacita2, null);
            this.guidaFile.SetHelpNavigator(this.etiCapacita2, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiCapacita2.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiCapacita2, null);
            this.etiCapacita2.Name = "etiCapacita2";
            this.guidaFile.SetShowHelp(this.etiCapacita2, ((bool)(resources.GetObject("etiCapacita2.ShowHelp"))));
            // 
            // etiCapacitaUguali
            // 
            this.etiCapacitaUguali.AccessibleDescription = null;
            this.etiCapacitaUguali.AccessibleName = null;
            resources.ApplyResources(this.etiCapacitaUguali, "etiCapacitaUguali");
            this.guidaFile.SetHelpKeyword(this.etiCapacitaUguali, null);
            this.guidaFile.SetHelpNavigator(this.etiCapacitaUguali, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiCapacitaUguali.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiCapacitaUguali, null);
            this.etiCapacitaUguali.Name = "etiCapacitaUguali";
            this.guidaFile.SetShowHelp(this.etiCapacitaUguali, ((bool)(resources.GetObject("etiCapacitaUguali.ShowHelp"))));
            // 
            // etiMonete
            // 
            this.etiMonete.AccessibleDescription = null;
            this.etiMonete.AccessibleName = null;
            resources.ApplyResources(this.etiMonete, "etiMonete");
            this.etiMonete.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiMonete, null);
            this.guidaFile.SetHelpNavigator(this.etiMonete, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiMonete.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiMonete, null);
            this.etiMonete.Name = "etiMonete";
            this.guidaFile.SetShowHelp(this.etiMonete, ((bool)(resources.GetObject("etiMonete.ShowHelp"))));
            // 
            // tbMonete1
            // 
            this.tbMonete1.AccessibleDescription = null;
            this.tbMonete1.AccessibleName = null;
            resources.ApplyResources(this.tbMonete1, "tbMonete1");
            this.tbMonete1.BackgroundImage = null;
            this.tbMonete1.Font = null;
            this.guidaFile.SetHelpKeyword(this.tbMonete1, resources.GetString("tbMonete1.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.tbMonete1, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("tbMonete1.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.tbMonete1, null);
            this.tbMonete1.Name = "tbMonete1";
            this.guidaFile.SetShowHelp(this.tbMonete1, ((bool)(resources.GetObject("tbMonete1.ShowHelp"))));
            this.tbMonete1.Tag = "Monete";
            this.tbMonete1.TextChanged += new System.EventHandler(this.ConversionChanged);
            this.tbMonete1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tb1_KeyPress);
            // 
            // cbMonete1
            // 
            this.cbMonete1.AccessibleDescription = null;
            this.cbMonete1.AccessibleName = null;
            resources.ApplyResources(this.cbMonete1, "cbMonete1");
            this.cbMonete1.BackgroundImage = null;
            this.cbMonete1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMonete1.Font = null;
            this.cbMonete1.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.cbMonete1, resources.GetString("cbMonete1.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbMonete1, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbMonete1.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbMonete1, null);
            this.cbMonete1.Items.AddRange(new object[] {
            resources.GetString("cbMonete1.Items"),
            resources.GetString("cbMonete1.Items1"),
            resources.GetString("cbMonete1.Items2"),
            resources.GetString("cbMonete1.Items3"),
            resources.GetString("cbMonete1.Items4"),
            resources.GetString("cbMonete1.Items5"),
            resources.GetString("cbMonete1.Items6"),
            resources.GetString("cbMonete1.Items7"),
            resources.GetString("cbMonete1.Items8"),
            resources.GetString("cbMonete1.Items9"),
            resources.GetString("cbMonete1.Items10")});
            this.cbMonete1.Name = "cbMonete1";
            this.guidaFile.SetShowHelp(this.cbMonete1, ((bool)(resources.GetObject("cbMonete1.ShowHelp"))));
            this.cbMonete1.Tag = "Monete";
            this.cbMonete1.SelectedIndexChanged += new System.EventHandler(this.ConversionChanged);
            // 
            // cbMonete2
            // 
            this.cbMonete2.AccessibleDescription = null;
            this.cbMonete2.AccessibleName = null;
            resources.ApplyResources(this.cbMonete2, "cbMonete2");
            this.cbMonete2.BackgroundImage = null;
            this.cbMonete2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMonete2.Font = null;
            this.cbMonete2.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.cbMonete2, resources.GetString("cbMonete2.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbMonete2, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbMonete2.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbMonete2, null);
            this.cbMonete2.Name = "cbMonete2";
            this.guidaFile.SetShowHelp(this.cbMonete2, ((bool)(resources.GetObject("cbMonete2.ShowHelp"))));
            this.cbMonete2.Tag = "Monete";
            this.cbMonete2.SelectedIndexChanged += new System.EventHandler(this.ConversionChanged);
            // 
            // etiMonete2
            // 
            this.etiMonete2.AccessibleDescription = null;
            this.etiMonete2.AccessibleName = null;
            resources.ApplyResources(this.etiMonete2, "etiMonete2");
            this.etiMonete2.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiMonete2, null);
            this.guidaFile.SetHelpNavigator(this.etiMonete2, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiMonete2.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiMonete2, null);
            this.etiMonete2.Name = "etiMonete2";
            this.guidaFile.SetShowHelp(this.etiMonete2, ((bool)(resources.GetObject("etiMonete2.ShowHelp"))));
            // 
            // etiMoneteUguali
            // 
            this.etiMoneteUguali.AccessibleDescription = null;
            this.etiMoneteUguali.AccessibleName = null;
            resources.ApplyResources(this.etiMoneteUguali, "etiMoneteUguali");
            this.guidaFile.SetHelpKeyword(this.etiMoneteUguali, null);
            this.guidaFile.SetHelpNavigator(this.etiMoneteUguali, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiMoneteUguali.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiMoneteUguali, null);
            this.etiMoneteUguali.Name = "etiMoneteUguali";
            this.guidaFile.SetShowHelp(this.etiMoneteUguali, ((bool)(resources.GetObject("etiMoneteUguali.ShowHelp"))));
            // 
            // Misure
            // 
            this.AcceptButton = this.btnCanc;
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.etiMoneteUguali);
            this.Controls.Add(this.etiMonete2);
            this.Controls.Add(this.etiMonete);
            this.Controls.Add(this.cbMonete2);
            this.Controls.Add(this.cbMonete1);
            this.Controls.Add(this.etiCapacitaUguali);
            this.Controls.Add(this.tbMonete1);
            this.Controls.Add(this.etiLunghezze2);
            this.Controls.Add(this.etiCapacita2);
            this.Controls.Add(this.cbCapacita1);
            this.Controls.Add(this.etiCapacita);
            this.Controls.Add(this.cbCapacita2);
            this.Controls.Add(this.etiLunghezzeUguali);
            this.Controls.Add(this.tbCapacita1);
            this.Controls.Add(this.cbPesi2);
            this.Controls.Add(this.etiPesi2);
            this.Controls.Add(this.etiLunghezze);
            this.Controls.Add(this.cbLunghezze2);
            this.Controls.Add(this.tbLunghezze1);
            this.Controls.Add(this.etiPesiUguale);
            this.Controls.Add(this.cbLunghezze1);
            this.Controls.Add(this.cbPesi1);
            this.Controls.Add(this.etiPesi);
            this.Controls.Add(this.tbPesi1);
            this.Font = null;
            this.guidaFile.SetHelpKeyword(this, null);
            this.guidaFile.SetHelpNavigator(this, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("$this.HelpNavigator"))));
            this.guidaFile.SetHelpString(this, null);
            this.Name = "Misure";
            this.guidaFile.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.Tag = "Misure";
            this.Controls.SetChildIndex(this.tbPesi1, 0);
            this.Controls.SetChildIndex(this.etiPesi, 0);
            this.Controls.SetChildIndex(this.cbPesi1, 0);
            this.Controls.SetChildIndex(this.cbLunghezze1, 0);
            this.Controls.SetChildIndex(this.etiPesiUguale, 0);
            this.Controls.SetChildIndex(this.tbLunghezze1, 0);
            this.Controls.SetChildIndex(this.cbLunghezze2, 0);
            this.Controls.SetChildIndex(this.etiLunghezze, 0);
            this.Controls.SetChildIndex(this.etiPesi2, 0);
            this.Controls.SetChildIndex(this.cbPesi2, 0);
            this.Controls.SetChildIndex(this.tbCapacita1, 0);
            this.Controls.SetChildIndex(this.etiLunghezzeUguali, 0);
            this.Controls.SetChildIndex(this.cbCapacita2, 0);
            this.Controls.SetChildIndex(this.etiCapacita, 0);
            this.Controls.SetChildIndex(this.cbCapacita1, 0);
            this.Controls.SetChildIndex(this.etiCapacita2, 0);
            this.Controls.SetChildIndex(this.etiLunghezze2, 0);
            this.Controls.SetChildIndex(this.tbMonete1, 0);
            this.Controls.SetChildIndex(this.etiCapacitaUguali, 0);
            this.Controls.SetChildIndex(this.cbMonete1, 0);
            this.Controls.SetChildIndex(this.cbMonete2, 0);
            this.Controls.SetChildIndex(this.etiMonete, 0);
            this.Controls.SetChildIndex(this.etiMonete2, 0);
            this.Controls.SetChildIndex(this.etiMoneteUguali, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnCanc, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label etiPesi;
        private System.Windows.Forms.TextBox tbPesi1;
        private System.Windows.Forms.ComboBox cbPesi1;
        private System.Windows.Forms.Label etiPesi2;
        private System.Windows.Forms.ComboBox cbPesi2;
        private System.Windows.Forms.Label etiPesiUguale;
        private System.Windows.Forms.Label etiLunghezze;
        private System.Windows.Forms.TextBox tbLunghezze1;
        private System.Windows.Forms.ComboBox cbLunghezze1;
        private System.Windows.Forms.ComboBox cbLunghezze2;
        private System.Windows.Forms.Label etiLunghezze2;
        private System.Windows.Forms.Label etiLunghezzeUguali;
        private System.Windows.Forms.Label etiCapacita;
        private System.Windows.Forms.TextBox tbCapacita1;
        private System.Windows.Forms.ComboBox cbCapacita1;
        private System.Windows.Forms.ComboBox cbCapacita2;
        private System.Windows.Forms.Label etiCapacita2;
        private System.Windows.Forms.Label etiCapacitaUguali;
        private System.Windows.Forms.Label etiMonete;
        private System.Windows.Forms.TextBox tbMonete1;
        private System.Windows.Forms.ComboBox cbMonete1;
        private System.Windows.Forms.ComboBox cbMonete2;
        private System.Windows.Forms.Label etiMonete2;
        private System.Windows.Forms.Label etiMoneteUguali;
    }
}