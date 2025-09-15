namespace LaParola
{
    partial class ApriNota
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ApriNota));
            this.btnCancellaNota = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tpCommentario = new System.Windows.Forms.TabPage();
            this.pulAutoComp = new System.Windows.Forms.Button();
            this.tvNoteCommentario = new System.Windows.Forms.TreeView();
            this.txtNotaCommentario = new System.Windows.Forms.TextBox();
            this.etiNotaCommentario = new System.Windows.Forms.Label();
            this.tpDizionario = new System.Windows.Forms.TabPage();
            this.etiNotaDizionario = new System.Windows.Forms.Label();
            this.lbNoteDizionario = new System.Windows.Forms.ListBox();
            this.txtNotaDizionario = new System.Windows.Forms.TextBox();
            this.tpLibro = new System.Windows.Forms.TabPage();
            this.cbIndice = new System.Windows.Forms.ComboBox();
            this.etiIndice = new System.Windows.Forms.Label();
            this.tvNoteOrdinate = new System.Windows.Forms.TreeView();
            this.pmNoteInOrdine = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addbeforeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addafterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addunderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tpRiferimenti = new System.Windows.Forms.TabPage();
            this.txtRiferimenti = new System.Windows.Forms.TextBox();
            this.lbRiferimenti = new System.Windows.Forms.ListBox();
            this.pulRiferimenti = new System.Windows.Forms.Button();
            this.etiRiferimenti = new System.Windows.Forms.Label();
            this.tabControl.SuspendLayout();
            this.tpCommentario.SuspendLayout();
            this.tpDizionario.SuspendLayout();
            this.tpLibro.SuspendLayout();
            this.pmNoteInOrdine.SuspendLayout();
            this.tpRiferimenti.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.AccessibleDescription = null;
            this.btnOK.AccessibleName = null;
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.BackgroundImage = null;
            this.btnOK.Font = null;
            this.guidaFile.SetHelpKeyword(this.btnOK, resources.GetString("btnOK.HelpKeyword"));
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
            this.btnCanc.Click += new System.EventHandler(this.btnCanc_Click);
            // 
            // guidaFile
            // 
            this.guidaFile.HelpNamespace = null;
            // 
            // btnCancellaNota
            // 
            this.btnCancellaNota.AccessibleDescription = null;
            this.btnCancellaNota.AccessibleName = null;
            resources.ApplyResources(this.btnCancellaNota, "btnCancellaNota");
            this.btnCancellaNota.BackgroundImage = null;
            this.btnCancellaNota.Font = null;
            this.guidaFile.SetHelpKeyword(this.btnCancellaNota, resources.GetString("btnCancellaNota.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.btnCancellaNota, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("btnCancellaNota.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.btnCancellaNota, null);
            this.btnCancellaNota.Image = global::LaParola.Properties.Resources.cancellaoggetto;
            this.btnCancellaNota.Name = "btnCancellaNota";
            this.guidaFile.SetShowHelp(this.btnCancellaNota, ((bool)(resources.GetObject("btnCancellaNota.ShowHelp"))));
            this.btnCancellaNota.UseVisualStyleBackColor = true;
            this.btnCancellaNota.Click += new System.EventHandler(this.btnCancellaNota_Click);
            // 
            // tabControl
            // 
            this.tabControl.AccessibleDescription = null;
            this.tabControl.AccessibleName = null;
            resources.ApplyResources(this.tabControl, "tabControl");
            this.tabControl.BackgroundImage = null;
            this.tabControl.Controls.Add(this.tpCommentario);
            this.tabControl.Controls.Add(this.tpDizionario);
            this.tabControl.Controls.Add(this.tpLibro);
            this.tabControl.Controls.Add(this.tpRiferimenti);
            this.tabControl.Font = null;
            this.guidaFile.SetHelpKeyword(this.tabControl, resources.GetString("tabControl.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.tabControl, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("tabControl.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.tabControl, null);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.guidaFile.SetShowHelp(this.tabControl, ((bool)(resources.GetObject("tabControl.ShowHelp"))));
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // tpCommentario
            // 
            this.tpCommentario.AccessibleDescription = null;
            this.tpCommentario.AccessibleName = null;
            resources.ApplyResources(this.tpCommentario, "tpCommentario");
            this.tpCommentario.BackgroundImage = null;
            this.tpCommentario.Controls.Add(this.pulAutoComp);
            this.tpCommentario.Controls.Add(this.tvNoteCommentario);
            this.tpCommentario.Controls.Add(this.txtNotaCommentario);
            this.tpCommentario.Controls.Add(this.etiNotaCommentario);
            this.tpCommentario.Font = null;
            this.guidaFile.SetHelpKeyword(this.tpCommentario, null);
            this.guidaFile.SetHelpNavigator(this.tpCommentario, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("tpCommentario.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.tpCommentario, null);
            this.tpCommentario.Name = "tpCommentario";
            this.guidaFile.SetShowHelp(this.tpCommentario, ((bool)(resources.GetObject("tpCommentario.ShowHelp"))));
            this.tpCommentario.Tag = "";
            this.tpCommentario.UseVisualStyleBackColor = true;
            // 
            // pulAutoComp
            // 
            this.pulAutoComp.AccessibleDescription = null;
            this.pulAutoComp.AccessibleName = null;
            resources.ApplyResources(this.pulAutoComp, "pulAutoComp");
            this.pulAutoComp.BackgroundImage = null;
            this.pulAutoComp.Font = null;
            this.guidaFile.SetHelpKeyword(this.pulAutoComp, resources.GetString("pulAutoComp.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.pulAutoComp, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulAutoComp.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.pulAutoComp, null);
            this.pulAutoComp.Image = global::LaParola.Properties.Resources.autocomp;
            this.pulAutoComp.Name = "pulAutoComp";
            this.guidaFile.SetShowHelp(this.pulAutoComp, ((bool)(resources.GetObject("pulAutoComp.ShowHelp"))));
            this.pulAutoComp.UseVisualStyleBackColor = true;
            this.pulAutoComp.Click += new System.EventHandler(this.pulAutoComp_Click);
            // 
            // tvNoteCommentario
            // 
            this.tvNoteCommentario.AccessibleDescription = null;
            this.tvNoteCommentario.AccessibleName = null;
            resources.ApplyResources(this.tvNoteCommentario, "tvNoteCommentario");
            this.tvNoteCommentario.BackgroundImage = null;
            this.tvNoteCommentario.Font = null;
            this.guidaFile.SetHelpKeyword(this.tvNoteCommentario, resources.GetString("tvNoteCommentario.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.tvNoteCommentario, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("tvNoteCommentario.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.tvNoteCommentario, null);
            this.tvNoteCommentario.HotTracking = true;
            this.tvNoteCommentario.Name = "tvNoteCommentario";
            this.guidaFile.SetShowHelp(this.tvNoteCommentario, ((bool)(resources.GetObject("tvNoteCommentario.ShowHelp"))));
            this.tvNoteCommentario.DoubleClick += new System.EventHandler(this.tvNoteCommentario_DoubleClick);
            this.tvNoteCommentario.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvNoteCommentario_AfterSelect);
            // 
            // txtNotaCommentario
            // 
            this.txtNotaCommentario.AccessibleDescription = null;
            this.txtNotaCommentario.AccessibleName = null;
            resources.ApplyResources(this.txtNotaCommentario, "txtNotaCommentario");
            this.txtNotaCommentario.BackgroundImage = null;
            this.txtNotaCommentario.Font = null;
            this.guidaFile.SetHelpKeyword(this.txtNotaCommentario, resources.GetString("txtNotaCommentario.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.txtNotaCommentario, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("txtNotaCommentario.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.txtNotaCommentario, null);
            this.txtNotaCommentario.Name = "txtNotaCommentario";
            this.guidaFile.SetShowHelp(this.txtNotaCommentario, ((bool)(resources.GetObject("txtNotaCommentario.ShowHelp"))));
            this.txtNotaCommentario.TextChanged += new System.EventHandler(this.txtNotaCommentario_TextChanged);
            // 
            // etiNotaCommentario
            // 
            this.etiNotaCommentario.AccessibleDescription = null;
            this.etiNotaCommentario.AccessibleName = null;
            resources.ApplyResources(this.etiNotaCommentario, "etiNotaCommentario");
            this.etiNotaCommentario.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiNotaCommentario, resources.GetString("etiNotaCommentario.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.etiNotaCommentario, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiNotaCommentario.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiNotaCommentario, null);
            this.etiNotaCommentario.Name = "etiNotaCommentario";
            this.guidaFile.SetShowHelp(this.etiNotaCommentario, ((bool)(resources.GetObject("etiNotaCommentario.ShowHelp"))));
            // 
            // tpDizionario
            // 
            this.tpDizionario.AccessibleDescription = null;
            this.tpDizionario.AccessibleName = null;
            resources.ApplyResources(this.tpDizionario, "tpDizionario");
            this.tpDizionario.BackgroundImage = null;
            this.tpDizionario.Controls.Add(this.etiNotaDizionario);
            this.tpDizionario.Controls.Add(this.lbNoteDizionario);
            this.tpDizionario.Controls.Add(this.txtNotaDizionario);
            this.tpDizionario.Font = null;
            this.guidaFile.SetHelpKeyword(this.tpDizionario, null);
            this.guidaFile.SetHelpNavigator(this.tpDizionario, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("tpDizionario.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.tpDizionario, null);
            this.tpDizionario.Name = "tpDizionario";
            this.guidaFile.SetShowHelp(this.tpDizionario, ((bool)(resources.GetObject("tpDizionario.ShowHelp"))));
            this.tpDizionario.Tag = "";
            this.tpDizionario.UseVisualStyleBackColor = true;
            // 
            // etiNotaDizionario
            // 
            this.etiNotaDizionario.AccessibleDescription = null;
            this.etiNotaDizionario.AccessibleName = null;
            resources.ApplyResources(this.etiNotaDizionario, "etiNotaDizionario");
            this.etiNotaDizionario.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiNotaDizionario, null);
            this.guidaFile.SetHelpNavigator(this.etiNotaDizionario, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiNotaDizionario.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiNotaDizionario, null);
            this.etiNotaDizionario.Name = "etiNotaDizionario";
            this.guidaFile.SetShowHelp(this.etiNotaDizionario, ((bool)(resources.GetObject("etiNotaDizionario.ShowHelp"))));
            // 
            // lbNoteDizionario
            // 
            this.lbNoteDizionario.AccessibleDescription = null;
            this.lbNoteDizionario.AccessibleName = null;
            resources.ApplyResources(this.lbNoteDizionario, "lbNoteDizionario");
            this.lbNoteDizionario.BackgroundImage = null;
            this.lbNoteDizionario.Font = null;
            this.lbNoteDizionario.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.lbNoteDizionario, resources.GetString("lbNoteDizionario.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.lbNoteDizionario, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("lbNoteDizionario.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.lbNoteDizionario, null);
            this.lbNoteDizionario.Name = "lbNoteDizionario";
            this.guidaFile.SetShowHelp(this.lbNoteDizionario, ((bool)(resources.GetObject("lbNoteDizionario.ShowHelp"))));
            this.lbNoteDizionario.DoubleClick += new System.EventHandler(this.lbNoteDizionario_DoubleClick);
            this.lbNoteDizionario.Click += new System.EventHandler(this.lbNoteDizionario_Click);
            // 
            // txtNotaDizionario
            // 
            this.txtNotaDizionario.AccessibleDescription = null;
            this.txtNotaDizionario.AccessibleName = null;
            resources.ApplyResources(this.txtNotaDizionario, "txtNotaDizionario");
            this.txtNotaDizionario.BackgroundImage = null;
            this.txtNotaDizionario.Font = null;
            this.guidaFile.SetHelpKeyword(this.txtNotaDizionario, resources.GetString("txtNotaDizionario.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.txtNotaDizionario, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("txtNotaDizionario.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.txtNotaDizionario, null);
            this.txtNotaDizionario.Name = "txtNotaDizionario";
            this.guidaFile.SetShowHelp(this.txtNotaDizionario, ((bool)(resources.GetObject("txtNotaDizionario.ShowHelp"))));
            this.txtNotaDizionario.TextChanged += new System.EventHandler(this.txtNotaDizionario_TextChanged);
            // 
            // tpLibro
            // 
            this.tpLibro.AccessibleDescription = null;
            this.tpLibro.AccessibleName = null;
            resources.ApplyResources(this.tpLibro, "tpLibro");
            this.tpLibro.BackgroundImage = null;
            this.tpLibro.Controls.Add(this.cbIndice);
            this.tpLibro.Controls.Add(this.etiIndice);
            this.tpLibro.Controls.Add(this.tvNoteOrdinate);
            this.tpLibro.Font = null;
            this.guidaFile.SetHelpKeyword(this.tpLibro, null);
            this.guidaFile.SetHelpNavigator(this.tpLibro, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("tpLibro.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.tpLibro, null);
            this.tpLibro.Name = "tpLibro";
            this.guidaFile.SetShowHelp(this.tpLibro, ((bool)(resources.GetObject("tpLibro.ShowHelp"))));
            this.tpLibro.Tag = "";
            this.tpLibro.UseVisualStyleBackColor = true;
            // 
            // cbIndice
            // 
            this.cbIndice.AccessibleDescription = null;
            this.cbIndice.AccessibleName = null;
            resources.ApplyResources(this.cbIndice, "cbIndice");
            this.cbIndice.BackgroundImage = null;
            this.cbIndice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbIndice.Font = null;
            this.cbIndice.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.cbIndice, resources.GetString("cbIndice.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.cbIndice, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("cbIndice.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.cbIndice, null);
            this.cbIndice.Name = "cbIndice";
            this.guidaFile.SetShowHelp(this.cbIndice, ((bool)(resources.GetObject("cbIndice.ShowHelp"))));
            this.cbIndice.SelectedIndexChanged += new System.EventHandler(this.cbIndice_SelectedIndexChanged);
            // 
            // etiIndice
            // 
            this.etiIndice.AccessibleDescription = null;
            this.etiIndice.AccessibleName = null;
            resources.ApplyResources(this.etiIndice, "etiIndice");
            this.etiIndice.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiIndice, resources.GetString("etiIndice.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.etiIndice, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiIndice.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiIndice, null);
            this.etiIndice.Name = "etiIndice";
            this.guidaFile.SetShowHelp(this.etiIndice, ((bool)(resources.GetObject("etiIndice.ShowHelp"))));
            // 
            // tvNoteOrdinate
            // 
            this.tvNoteOrdinate.AccessibleDescription = null;
            this.tvNoteOrdinate.AccessibleName = null;
            this.tvNoteOrdinate.AllowDrop = true;
            resources.ApplyResources(this.tvNoteOrdinate, "tvNoteOrdinate");
            this.tvNoteOrdinate.BackgroundImage = null;
            this.tvNoteOrdinate.ContextMenuStrip = this.pmNoteInOrdine;
            this.tvNoteOrdinate.Font = null;
            this.guidaFile.SetHelpKeyword(this.tvNoteOrdinate, resources.GetString("tvNoteOrdinate.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.tvNoteOrdinate, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("tvNoteOrdinate.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.tvNoteOrdinate, null);
            this.tvNoteOrdinate.HotTracking = true;
            this.tvNoteOrdinate.LabelEdit = true;
            this.tvNoteOrdinate.Name = "tvNoteOrdinate";
            this.guidaFile.SetShowHelp(this.tvNoteOrdinate, ((bool)(resources.GetObject("tvNoteOrdinate.ShowHelp"))));
            this.tvNoteOrdinate.Tag = "Titolo";
            this.tvNoteOrdinate.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.tvNoteOrdinate_AfterLabelEdit);
            this.tvNoteOrdinate.DoubleClick += new System.EventHandler(this.tvNoteOrdinate_DoubleClick);
            this.tvNoteOrdinate.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tvNoteOrdinate_MouseUp);
            this.tvNoteOrdinate.DragDrop += new System.Windows.Forms.DragEventHandler(this.tvNoteOrdinate_DragDrop);
            this.tvNoteOrdinate.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvNoteOrdinate_AfterSelect);
            this.tvNoteOrdinate.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tvNoteOrdinate_MouseMove);
            this.tvNoteOrdinate.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tvNoteOrdinate_MouseDown);
            this.tvNoteOrdinate.DragOver += new System.Windows.Forms.DragEventHandler(this.tvNoteOrdinate_DragOver);
            // 
            // pmNoteInOrdine
            // 
            this.pmNoteInOrdine.AccessibleDescription = null;
            this.pmNoteInOrdine.AccessibleName = null;
            resources.ApplyResources(this.pmNoteInOrdine, "pmNoteInOrdine");
            this.pmNoteInOrdine.BackgroundImage = null;
            this.pmNoteInOrdine.Font = null;
            this.guidaFile.SetHelpKeyword(this.pmNoteInOrdine, null);
            this.guidaFile.SetHelpNavigator(this.pmNoteInOrdine, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pmNoteInOrdine.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.pmNoteInOrdine, null);
            this.pmNoteInOrdine.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addbeforeToolStripMenuItem,
            this.addafterToolStripMenuItem,
            this.addunderToolStripMenuItem,
            this.toolStripSeparator1,
            this.removeToolStripMenuItem});
            this.pmNoteInOrdine.Name = "pmNoteInOrdine";
            this.guidaFile.SetShowHelp(this.pmNoteInOrdine, ((bool)(resources.GetObject("pmNoteInOrdine.ShowHelp"))));
            this.pmNoteInOrdine.Opening += new System.ComponentModel.CancelEventHandler(this.pmNoteInOrdine_Opening);
            // 
            // addbeforeToolStripMenuItem
            // 
            this.addbeforeToolStripMenuItem.AccessibleDescription = null;
            this.addbeforeToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.addbeforeToolStripMenuItem, "addbeforeToolStripMenuItem");
            this.addbeforeToolStripMenuItem.BackgroundImage = null;
            this.addbeforeToolStripMenuItem.Name = "addbeforeToolStripMenuItem";
            this.addbeforeToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.addbeforeToolStripMenuItem.Tag = "0";
            this.addbeforeToolStripMenuItem.Click += new System.EventHandler(this.noteInOrdineMenuItem_Click);
            // 
            // addafterToolStripMenuItem
            // 
            this.addafterToolStripMenuItem.AccessibleDescription = null;
            this.addafterToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.addafterToolStripMenuItem, "addafterToolStripMenuItem");
            this.addafterToolStripMenuItem.BackgroundImage = null;
            this.addafterToolStripMenuItem.Name = "addafterToolStripMenuItem";
            this.addafterToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.addafterToolStripMenuItem.Tag = "1";
            this.addafterToolStripMenuItem.Click += new System.EventHandler(this.noteInOrdineMenuItem_Click);
            // 
            // addunderToolStripMenuItem
            // 
            this.addunderToolStripMenuItem.AccessibleDescription = null;
            this.addunderToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.addunderToolStripMenuItem, "addunderToolStripMenuItem");
            this.addunderToolStripMenuItem.BackgroundImage = null;
            this.addunderToolStripMenuItem.Name = "addunderToolStripMenuItem";
            this.addunderToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.addunderToolStripMenuItem.Tag = "2";
            this.addunderToolStripMenuItem.Click += new System.EventHandler(this.noteInOrdineMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.AccessibleDescription = null;
            this.toolStripSeparator1.AccessibleName = null;
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.AccessibleDescription = null;
            this.removeToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.removeToolStripMenuItem, "removeToolStripMenuItem");
            this.removeToolStripMenuItem.BackgroundImage = null;
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.removeToolStripMenuItem.Tag = "3";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.noteInOrdineMenuItem_Click);
            // 
            // tpRiferimenti
            // 
            this.tpRiferimenti.AccessibleDescription = null;
            this.tpRiferimenti.AccessibleName = null;
            resources.ApplyResources(this.tpRiferimenti, "tpRiferimenti");
            this.tpRiferimenti.BackgroundImage = null;
            this.tpRiferimenti.Controls.Add(this.txtRiferimenti);
            this.tpRiferimenti.Controls.Add(this.lbRiferimenti);
            this.tpRiferimenti.Controls.Add(this.pulRiferimenti);
            this.tpRiferimenti.Controls.Add(this.etiRiferimenti);
            this.tpRiferimenti.Font = null;
            this.guidaFile.SetHelpKeyword(this.tpRiferimenti, null);
            this.guidaFile.SetHelpNavigator(this.tpRiferimenti, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("tpRiferimenti.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.tpRiferimenti, null);
            this.tpRiferimenti.Name = "tpRiferimenti";
            this.guidaFile.SetShowHelp(this.tpRiferimenti, ((bool)(resources.GetObject("tpRiferimenti.ShowHelp"))));
            this.tpRiferimenti.Tag = "";
            this.tpRiferimenti.UseVisualStyleBackColor = true;
            // 
            // txtRiferimenti
            // 
            this.txtRiferimenti.AccessibleDescription = null;
            this.txtRiferimenti.AccessibleName = null;
            resources.ApplyResources(this.txtRiferimenti, "txtRiferimenti");
            this.txtRiferimenti.BackgroundImage = null;
            this.txtRiferimenti.Font = null;
            this.guidaFile.SetHelpKeyword(this.txtRiferimenti, resources.GetString("txtRiferimenti.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.txtRiferimenti, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("txtRiferimenti.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.txtRiferimenti, null);
            this.txtRiferimenti.Name = "txtRiferimenti";
            this.guidaFile.SetShowHelp(this.txtRiferimenti, ((bool)(resources.GetObject("txtRiferimenti.ShowHelp"))));
            this.txtRiferimenti.TextChanged += new System.EventHandler(this.txtRiferimenti_TextChanged);
            // 
            // lbRiferimenti
            // 
            this.lbRiferimenti.AccessibleDescription = null;
            this.lbRiferimenti.AccessibleName = null;
            resources.ApplyResources(this.lbRiferimenti, "lbRiferimenti");
            this.lbRiferimenti.BackgroundImage = null;
            this.lbRiferimenti.Font = null;
            this.lbRiferimenti.FormattingEnabled = true;
            this.guidaFile.SetHelpKeyword(this.lbRiferimenti, resources.GetString("lbRiferimenti.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.lbRiferimenti, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("lbRiferimenti.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.lbRiferimenti, null);
            this.lbRiferimenti.Name = "lbRiferimenti";
            this.guidaFile.SetShowHelp(this.lbRiferimenti, ((bool)(resources.GetObject("lbRiferimenti.ShowHelp"))));
            this.lbRiferimenti.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lbRiferimenti_MouseDoubleClick);
            this.lbRiferimenti.SelectedIndexChanged += new System.EventHandler(this.lbRiferimenti_SelectedIndexChanged);
            // 
            // pulRiferimenti
            // 
            this.pulRiferimenti.AccessibleDescription = null;
            this.pulRiferimenti.AccessibleName = null;
            resources.ApplyResources(this.pulRiferimenti, "pulRiferimenti");
            this.pulRiferimenti.BackgroundImage = null;
            this.pulRiferimenti.Font = null;
            this.guidaFile.SetHelpKeyword(this.pulRiferimenti, resources.GetString("pulRiferimenti.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.pulRiferimenti, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("pulRiferimenti.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.pulRiferimenti, null);
            this.pulRiferimenti.Name = "pulRiferimenti";
            this.guidaFile.SetShowHelp(this.pulRiferimenti, ((bool)(resources.GetObject("pulRiferimenti.ShowHelp"))));
            this.pulRiferimenti.UseVisualStyleBackColor = true;
            this.pulRiferimenti.Click += new System.EventHandler(this.pulRiferimenti_Click);
            // 
            // etiRiferimenti
            // 
            this.etiRiferimenti.AccessibleDescription = null;
            this.etiRiferimenti.AccessibleName = null;
            resources.ApplyResources(this.etiRiferimenti, "etiRiferimenti");
            this.etiRiferimenti.Font = null;
            this.guidaFile.SetHelpKeyword(this.etiRiferimenti, resources.GetString("etiRiferimenti.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this.etiRiferimenti, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("etiRiferimenti.HelpNavigator"))));
            this.guidaFile.SetHelpString(this.etiRiferimenti, null);
            this.etiRiferimenti.Name = "etiRiferimenti";
            this.guidaFile.SetShowHelp(this.etiRiferimenti, ((bool)(resources.GetObject("etiRiferimenti.ShowHelp"))));
            // 
            // ApriNota
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.btnCancellaNota);
            this.Font = null;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.guidaFile.SetHelpKeyword(this, resources.GetString("$this.HelpKeyword"));
            this.guidaFile.SetHelpNavigator(this, ((System.Windows.Forms.HelpNavigator)(resources.GetObject("$this.HelpNavigator"))));
            this.guidaFile.SetHelpString(this, null);
            this.Name = "ApriNota";
            this.guidaFile.SetShowHelp(this, ((bool)(resources.GetObject("$this.ShowHelp"))));
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Tag = "ApriNota";
            this.Load += new System.EventHandler(this.ApriNota_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ApriNota_FormClosing);
            this.Resize += new System.EventHandler(this.ApriNota_Resize);
            this.Controls.SetChildIndex(this.btnCancellaNota, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.btnCanc, 0);
            this.Controls.SetChildIndex(this.tabControl, 0);
            this.tabControl.ResumeLayout(false);
            this.tpCommentario.ResumeLayout(false);
            this.tpCommentario.PerformLayout();
            this.tpDizionario.ResumeLayout(false);
            this.tpDizionario.PerformLayout();
            this.tpLibro.ResumeLayout(false);
            this.tpLibro.PerformLayout();
            this.pmNoteInOrdine.ResumeLayout(false);
            this.tpRiferimenti.ResumeLayout(false);
            this.tpRiferimenti.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancellaNota;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tpCommentario;
        private System.Windows.Forms.TabPage tpDizionario;
        private System.Windows.Forms.ListBox lbNoteDizionario;
        private System.Windows.Forms.TextBox txtNotaDizionario;
        private System.Windows.Forms.TabPage tpLibro;
        private System.Windows.Forms.Label etiNotaDizionario;
        private System.Windows.Forms.ComboBox cbIndice;
        private System.Windows.Forms.Label etiIndice;
        private System.Windows.Forms.TreeView tvNoteOrdinate;
        private System.Windows.Forms.TextBox txtNotaCommentario;
        private System.Windows.Forms.Label etiNotaCommentario;
        private System.Windows.Forms.ContextMenuStrip pmNoteInOrdine;
        private System.Windows.Forms.ToolStripMenuItem addbeforeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addafterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addunderToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.TreeView tvNoteCommentario;
        private System.Windows.Forms.Button pulAutoComp;
        private System.Windows.Forms.TabPage tpRiferimenti;
        private System.Windows.Forms.ListBox lbRiferimenti;
        private System.Windows.Forms.Button pulRiferimenti;
        private System.Windows.Forms.Label etiRiferimenti;
        private System.Windows.Forms.TextBox txtRiferimenti;
    }
}