using TestiBiblici;
namespace LaParola
{
    partial class Editor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Editor));
            this.pmEditor = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.popupToolStripSeparatorGeneralWord = new System.Windows.Forms.ToolStripSeparator();
            this.informationOnWordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchWordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchRadiceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noteOnWordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.popupToolStripSeparatorWordVerse = new System.Windows.Forms.ToolStripSeparator();
            this.informationOnVerseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.browseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noteOnVerseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.makeLinkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.versesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rtEditor = new TestiBiblici.RichTextBoxEx();
            this.pmEditor.SuspendLayout();
            this.SuspendLayout();
            // 
            // pmEditor
            // 
            this.pmEditor.AccessibleDescription = null;
            this.pmEditor.AccessibleName = null;
            resources.ApplyResources(this.pmEditor, "pmEditor");
            this.pmEditor.BackgroundImage = null;
            this.pmEditor.Font = null;
            this.pmEditor.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.toolStripSeparator1,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.deleteToolStripMenuItem,
            this.popupToolStripSeparatorGeneralWord,
            this.informationOnWordToolStripMenuItem,
            this.searchToolStripMenuItem,
            this.noteOnWordToolStripMenuItem,
            this.popupToolStripSeparatorWordVerse,
            this.informationOnVerseToolStripMenuItem,
            this.browseToolStripMenuItem,
            this.noteOnVerseToolStripMenuItem,
            this.toolStripSeparator3,
            this.makeLinkToolStripMenuItem});
            this.pmEditor.Name = "pmEditor";
            this.pmEditor.Opening += new System.ComponentModel.CancelEventHandler(this.PmEditor_Opening);
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.AccessibleDescription = null;
            this.undoToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.undoToolStripMenuItem, "undoToolStripMenuItem");
            this.undoToolStripMenuItem.BackgroundImage = null;
            this.undoToolStripMenuItem.Image = global::LaParola.Properties.Resources.annulla;
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.UndoToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.AccessibleDescription = null;
            this.toolStripSeparator1.AccessibleName = null;
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.AccessibleDescription = null;
            this.cutToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.cutToolStripMenuItem, "cutToolStripMenuItem");
            this.cutToolStripMenuItem.BackgroundImage = null;
            this.cutToolStripMenuItem.Image = global::LaParola.Properties.Resources.taglia;
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.cutToolStripMenuItem.Click += new System.EventHandler(this.CutToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.AccessibleDescription = null;
            this.copyToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.copyToolStripMenuItem, "copyToolStripMenuItem");
            this.copyToolStripMenuItem.BackgroundImage = null;
            this.copyToolStripMenuItem.Image = global::LaParola.Properties.Resources.copia;
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.CopyToolStripMenuItem_Click);
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.AccessibleDescription = null;
            this.pasteToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.pasteToolStripMenuItem, "pasteToolStripMenuItem");
            this.pasteToolStripMenuItem.BackgroundImage = null;
            this.pasteToolStripMenuItem.Image = global::LaParola.Properties.Resources.incolla;
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.PasteToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.AccessibleDescription = null;
            this.deleteToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.deleteToolStripMenuItem, "deleteToolStripMenuItem");
            this.deleteToolStripMenuItem.BackgroundImage = null;
            this.deleteToolStripMenuItem.Image = global::LaParola.Properties.Resources.cancella;
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.DeleteToolStripMenuItem_Click);
            // 
            // popupToolStripSeparatorGeneralWord
            // 
            this.popupToolStripSeparatorGeneralWord.AccessibleDescription = null;
            this.popupToolStripSeparatorGeneralWord.AccessibleName = null;
            resources.ApplyResources(this.popupToolStripSeparatorGeneralWord, "popupToolStripSeparatorGeneralWord");
            this.popupToolStripSeparatorGeneralWord.Name = "popupToolStripSeparatorGeneralWord";
            // 
            // informationOnWordToolStripMenuItem
            // 
            this.informationOnWordToolStripMenuItem.AccessibleDescription = null;
            this.informationOnWordToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.informationOnWordToolStripMenuItem, "informationOnWordToolStripMenuItem");
            this.informationOnWordToolStripMenuItem.BackgroundImage = null;
            this.informationOnWordToolStripMenuItem.Image = global::LaParola.Properties.Resources.info;
            this.informationOnWordToolStripMenuItem.Name = "informationOnWordToolStripMenuItem";
            this.informationOnWordToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.informationOnWordToolStripMenuItem.Tag = "Informa&zioni su ";
            this.informationOnWordToolStripMenuItem.Click += new System.EventHandler(this.InformationOnToolStripMenuItem_Click);
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.AccessibleDescription = null;
            this.searchToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.searchToolStripMenuItem, "searchToolStripMenuItem");
            this.searchToolStripMenuItem.BackgroundImage = null;
            this.searchToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.searchWordToolStripMenuItem,
            this.searchRadiceToolStripMenuItem,
            this.searchSelectionToolStripMenuItem});
            this.searchToolStripMenuItem.Image = global::LaParola.Properties.Resources.ricerca;
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.ShortcutKeyDisplayString = null;
            // 
            // searchWordToolStripMenuItem
            // 
            this.searchWordToolStripMenuItem.AccessibleDescription = null;
            this.searchWordToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.searchWordToolStripMenuItem, "searchWordToolStripMenuItem");
            this.searchWordToolStripMenuItem.BackgroundImage = null;
            this.searchWordToolStripMenuItem.Name = "searchWordToolStripMenuItem";
            this.searchWordToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.searchWordToolStripMenuItem.Click += new System.EventHandler(this.SearchToolStripMenuItem_Click);
            // 
            // searchRadiceToolStripMenuItem
            // 
            this.searchRadiceToolStripMenuItem.AccessibleDescription = null;
            this.searchRadiceToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.searchRadiceToolStripMenuItem, "searchRadiceToolStripMenuItem");
            this.searchRadiceToolStripMenuItem.BackgroundImage = null;
            this.searchRadiceToolStripMenuItem.Name = "searchRadiceToolStripMenuItem";
            this.searchRadiceToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.searchRadiceToolStripMenuItem.Click += new System.EventHandler(this.SearchToolStripMenuItem_Click);
            // 
            // searchSelectionToolStripMenuItem
            // 
            this.searchSelectionToolStripMenuItem.AccessibleDescription = null;
            this.searchSelectionToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.searchSelectionToolStripMenuItem, "searchSelectionToolStripMenuItem");
            this.searchSelectionToolStripMenuItem.BackgroundImage = null;
            this.searchSelectionToolStripMenuItem.Name = "searchSelectionToolStripMenuItem";
            this.searchSelectionToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.searchSelectionToolStripMenuItem.Click += new System.EventHandler(this.SearchToolStripMenuItem_Click);
            // 
            // noteOnWordToolStripMenuItem
            // 
            this.noteOnWordToolStripMenuItem.AccessibleDescription = null;
            this.noteOnWordToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.noteOnWordToolStripMenuItem, "noteOnWordToolStripMenuItem");
            this.noteOnWordToolStripMenuItem.BackgroundImage = null;
            this.noteOnWordToolStripMenuItem.Image = global::LaParola.Properties.Resources.aprinota;
            this.noteOnWordToolStripMenuItem.Name = "noteOnWordToolStripMenuItem";
            this.noteOnWordToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.noteOnWordToolStripMenuItem.Tag = "&Nota su ";
            // 
            // popupToolStripSeparatorWordVerse
            // 
            this.popupToolStripSeparatorWordVerse.AccessibleDescription = null;
            this.popupToolStripSeparatorWordVerse.AccessibleName = null;
            resources.ApplyResources(this.popupToolStripSeparatorWordVerse, "popupToolStripSeparatorWordVerse");
            this.popupToolStripSeparatorWordVerse.Name = "popupToolStripSeparatorWordVerse";
            // 
            // informationOnVerseToolStripMenuItem
            // 
            this.informationOnVerseToolStripMenuItem.AccessibleDescription = null;
            this.informationOnVerseToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.informationOnVerseToolStripMenuItem, "informationOnVerseToolStripMenuItem");
            this.informationOnVerseToolStripMenuItem.BackgroundImage = null;
            this.informationOnVerseToolStripMenuItem.Image = global::LaParola.Properties.Resources.info;
            this.informationOnVerseToolStripMenuItem.Name = "informationOnVerseToolStripMenuItem";
            this.informationOnVerseToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.informationOnVerseToolStripMenuItem.Tag = "In&formazioni su ";
            this.informationOnVerseToolStripMenuItem.Click += new System.EventHandler(this.InformationOnToolStripMenuItem_Click);
            // 
            // browseToolStripMenuItem
            // 
            this.browseToolStripMenuItem.AccessibleDescription = null;
            this.browseToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.browseToolStripMenuItem, "browseToolStripMenuItem");
            this.browseToolStripMenuItem.BackgroundImage = null;
            this.browseToolStripMenuItem.Image = global::LaParola.Properties.Resources.visbibbia;
            this.browseToolStripMenuItem.Name = "browseToolStripMenuItem";
            this.browseToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.browseToolStripMenuItem.Tag = "&Sfoglia a ";
            this.browseToolStripMenuItem.Click += new System.EventHandler(this.BrowseToolStripMenuItem_Click);
            // 
            // noteOnVerseToolStripMenuItem
            // 
            this.noteOnVerseToolStripMenuItem.AccessibleDescription = null;
            this.noteOnVerseToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.noteOnVerseToolStripMenuItem, "noteOnVerseToolStripMenuItem");
            this.noteOnVerseToolStripMenuItem.BackgroundImage = null;
            this.noteOnVerseToolStripMenuItem.Image = global::LaParola.Properties.Resources.aprinota;
            this.noteOnVerseToolStripMenuItem.Name = "noteOnVerseToolStripMenuItem";
            this.noteOnVerseToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.noteOnVerseToolStripMenuItem.Tag = "N&ota su ";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.AccessibleDescription = null;
            this.toolStripSeparator3.AccessibleName = null;
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            // 
            // makeLinkToolStripMenuItem
            // 
            this.makeLinkToolStripMenuItem.AccessibleDescription = null;
            this.makeLinkToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.makeLinkToolStripMenuItem, "makeLinkToolStripMenuItem");
            this.makeLinkToolStripMenuItem.BackgroundImage = null;
            this.makeLinkToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.versesToolStripMenuItem,
            this.noteToolStripMenuItem,
            this.fileToolStripMenuItem});
            this.makeLinkToolStripMenuItem.Image = global::LaParola.Properties.Resources.collegamento;
            this.makeLinkToolStripMenuItem.Name = "makeLinkToolStripMenuItem";
            this.makeLinkToolStripMenuItem.ShortcutKeyDisplayString = null;
            // 
            // versesToolStripMenuItem
            // 
            this.versesToolStripMenuItem.AccessibleDescription = null;
            this.versesToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.versesToolStripMenuItem, "versesToolStripMenuItem");
            this.versesToolStripMenuItem.BackgroundImage = null;
            this.versesToolStripMenuItem.Name = "versesToolStripMenuItem";
            this.versesToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.versesToolStripMenuItem.Tag = "V";
            this.versesToolStripMenuItem.Click += new System.EventHandler(this.IpertestoToolStripMenuItem_Click);
            // 
            // noteToolStripMenuItem
            // 
            this.noteToolStripMenuItem.AccessibleDescription = null;
            this.noteToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.noteToolStripMenuItem, "noteToolStripMenuItem");
            this.noteToolStripMenuItem.BackgroundImage = null;
            this.noteToolStripMenuItem.Name = "noteToolStripMenuItem";
            this.noteToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.noteToolStripMenuItem.Tag = "N";
            this.noteToolStripMenuItem.Click += new System.EventHandler(this.IpertestoToolStripMenuItem_Click);
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.AccessibleDescription = null;
            this.fileToolStripMenuItem.AccessibleName = null;
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            this.fileToolStripMenuItem.BackgroundImage = null;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.ShortcutKeyDisplayString = null;
            this.fileToolStripMenuItem.Tag = "F";
            this.fileToolStripMenuItem.Click += new System.EventHandler(this.IpertestoToolStripMenuItem_Click);
            // 
            // rtEditor
            // 
            this.rtEditor.AcceptsTab = true;
            this.rtEditor.AccessibleDescription = null;
            this.rtEditor.AccessibleName = null;
            resources.ApplyResources(this.rtEditor, "rtEditor");
            this.rtEditor.BackgroundImage = null;
            this.rtEditor.ContextMenuStrip = this.pmEditor;
            this.rtEditor.Font = null;
            this.rtEditor.Lingua = null;
            this.rtEditor.Name = "rtEditor";
            this.rtEditor.SelectionAlignment = TestiBiblici.RichTextBoxEx.TextAlign.Left;
            this.rtEditor.Versione = null;
            this.rtEditor.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RtEditor_MouseUp);
            this.rtEditor.SelectionChanged += new System.EventHandler(this.RtEditor_SelectionChanged);
            this.rtEditor.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.RtEditor_LinkClicked);
            this.rtEditor.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.RtEditor_MouseDoubleClick);
            this.rtEditor.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RtEditor_MouseMove);
            this.rtEditor.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RtEditor_MouseDown);
            this.rtEditor.KeyUp += new System.Windows.Forms.KeyEventHandler(this.RtEditor_KeyUp);
            this.rtEditor.MouseHover += new System.EventHandler(this.RtEditor_MouseHover);
            this.rtEditor.TextChanged += new System.EventHandler(this.RtEditor_TextChanged);
            // 
            // Editor
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.rtEditor);
            this.Font = null;
            this.Name = "Editor";
            this.Tag = "Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Editor_FormClosing);
            this.pmEditor.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public RichTextBoxEx rtEditor;
        private System.Windows.Forms.ContextMenuStrip pmEditor;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem informationOnWordToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem makeLinkToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator popupToolStripSeparatorGeneralWord;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem searchWordToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchRadiceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem versesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem browseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noteOnWordToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator popupToolStripSeparatorWordVerse;
        private System.Windows.Forms.ToolStripMenuItem informationOnVerseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noteOnVerseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchSelectionToolStripMenuItem;




    }

}