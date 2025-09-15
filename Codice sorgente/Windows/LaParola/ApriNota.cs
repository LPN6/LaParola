using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using TestiBiblici;

namespace LaParola
{
    public partial class ApriNota : Template
    {
        private string versione;
        public string Versione
        {
            get { return versione; }
        }

        private bool ordineCambiato = false;
        private Rectangle dragBoxFromMouseDown;
        private List<string> notaDeiRiferimenti = new List<string>();
        private Principale genitore;
        private Boolean solaLettura;

        public ApriNota(Principale formGenitore, string nomeVersione)
        {
            Costruttore(formGenitore, nomeVersione, -1);
        }

        public ApriNota(Principale formGenitore, string nomeVersione, int scheda)
        {
            Costruttore(formGenitore, nomeVersione, scheda);
        }

        private void Costruttore(Principale formGenitore, string nomeVersione, int scheda)
        {
            InitializeComponent();

            genitore = formGenitore;
            guidaFile.HelpNamespace = genitore.NomeFileGuida();
            versione = nomeVersione;

            Text += " (" + nomeVersione + ")";
            btnCanc.Text = Principale.LocRM.GetString("MiscClose");

            if (!Principale.testi.EsistonoCitazioni(nomeVersione))
                tabControl.TabPages.Remove(tpRiferimenti);

            solaLettura = (Principale.testi.Info(nomeVersione).Bloccato != BloccatoTipi.Sbloccato);
            if (solaLettura)
            {
                TestoTipi tipi = Principale.testi.Info(nomeVersione).Tipo;
                if ((tipi & TestoTipi.Libro) != TestoTipi.Libro)
                    tabControl.TabPages.Remove(tpLibro);
                if ((tipi & TestoTipi.Dizionario) != TestoTipi.Dizionario)
                    tabControl.TabPages.Remove(tpDizionario);
                if ((tipi & TestoTipi.Commentario) != TestoTipi.Commentario)
                    tabControl.TabPages.Remove(tpCommentario);

                tvNoteOrdinate.LabelEdit = false;
                tvNoteOrdinate.AllowDrop = false;
                tvNoteOrdinate.ContextMenuStrip = null;
            }

            AggiornaElenchi(TestoTipi.Commentario | TestoTipi.Dizionario | TestoTipi.Libro);

            if (scheda >= 0 && scheda < tabControl.TabPages.Count)
                tabControl.SelectedIndex = scheda;
            else
            {
                if (tvNoteOrdinate.Nodes.Count > 0)
                    tabControl.SelectedIndex = 2;
                else
                {
                    if (tvNoteCommentario.Nodes.Count > lbNoteDizionario.Items.Count)
                        tabControl.SelectedIndex = 0;
                    else
                        tabControl.SelectedIndex = 1;
                }
            }
        }

        private void ApriNota_Load(object sender, EventArgs e)
        {
            tabControl_SelectedIndexChanged(this, null); // non può essere fatto in costruttore

            ordineCambiato = false;

            Application.DoEvents();
        }

        public void AggiornaElenchi(TestoTipi schedeDaAggiornare)
        {
            Cursor cursoreAttuale = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                Collection<string> note = Principale.testi.Note(versione);

                if ((schedeDaAggiornare & TestoTipi.Commentario) == TestoTipi.Commentario)
                {
                    tvNoteCommentario.BeginUpdate();
                    tvNoteCommentario.Nodes.Clear();
                    int libroPrecedente = -1, capitoloPrecedente = -1, libro, capitolo;
                    TreeNode ultimoCapitoloNodo = new TreeNode();
                    TreeNode ultimoLibroNodo = new TreeNode();
                    TreeNode ultimoNotaNodo = new TreeNode();
                    string libroNome = "";
                    foreach (string titolo in note)
                    {
                        if (titolo.StartsWith("#", StringComparison.Ordinal))
                        {
                            libro = Convert.ToInt32(titolo.Substring(1, 2), CultureInfo.InvariantCulture);
                            if (libro != libroPrecedente)
                            {
                                libroNome = Principale.testi.GetLibroNome(libro);
                                ultimoLibroNodo = tvNoteCommentario.Nodes.Add(libroNome);
                                libroPrecedente = libro;
                                capitoloPrecedente = -1;
                            }
                            capitolo = Convert.ToInt32(titolo.Substring(3, 3), CultureInfo.InvariantCulture);
                            if (capitolo != capitoloPrecedente)
                            {
                                // se capitolo==0, la nota è su tutto il libro
                                ultimoCapitoloNodo = ultimoLibroNodo.Nodes.Add(libroNome + (capitolo > 0 ? (" " + capitolo.ToString(CultureInfo.InvariantCulture)) : ""));
                                capitoloPrecedente = capitolo;
                            }
                            ultimoNotaNodo = ultimoCapitoloNodo.Nodes.Add(Principale.testi.ConvertiTitoloNotaARiferimento(titolo));
                            ultimoNotaNodo.Tag = titolo; // il riferimento come titolo di una nota, usato con i pulsanti OK e Cancella
                        }
                    }
                    tvNoteCommentario.EndUpdate();
                }

                if ((schedeDaAggiornare & TestoTipi.Dizionario) == TestoTipi.Dizionario)
                {
                    lbNoteDizionario.BeginUpdate();
                    lbNoteDizionario.Items.Clear();
                    foreach (string titolo in note)
                        if (!titolo.StartsWith("#", StringComparison.Ordinal))
                            lbNoteDizionario.Items.Add(titolo);
                    lbNoteDizionario.EndUpdate();
                }

                if ((schedeDaAggiornare & TestoTipi.Libro) == TestoTipi.Libro)
                {
                    Collection<string> noteInOrdine = Principale.testi.GetNoteInOrdine(versione);

                    cbIndice.Items.Clear();
                    if (solaLettura)
                    {
                        if (noteInOrdine.Count > 0)
                            cbIndice.Items.Add(noteInOrdine[0]);
                    }
                    else
                        cbIndice.Items.AddRange(new List<string>(Principale.testi.NoteConTitolo(versione)).ToArray());

                    if (noteInOrdine.Count > 0)
                    {
                        cbIndice.SelectedIndex = cbIndice.Items.IndexOf(noteInOrdine[0]);
                        tvNoteOrdinate.BeginUpdate();
                        tvNoteOrdinate.Nodes.Clear();
                        List<TreeNode> ultimoALivello = new List<TreeNode>();
                        string titolo;
                        int livello;
                        for (int i = 1; i < noteInOrdine.Count; ++i)
                        {
                            titolo = noteInOrdine[i];
                            while (!string.IsNullOrEmpty(titolo) && titolo[0] == '\t')
                                titolo = titolo.Remove(0, 1);
                            livello = noteInOrdine[i].Length - titolo.Length;
                            if (livello >= ultimoALivello.Count)
                                ultimoALivello.Add(new TreeNode());
                            if (livello == 0)
                                ultimoALivello[livello] = tvNoteOrdinate.Nodes.Add(titolo);
                            else
                                ultimoALivello[livello] = ultimoALivello[livello - 1].Nodes.Add(titolo);
                        }
                        tvNoteOrdinate.EndUpdate();
                    }
                }
            }
            finally
            {
                Cursor.Current = cursoreAttuale;
                if (cursoreAttuale != null)
                    cursoreAttuale.Dispose();
            }
        }

        private void ApriNota_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ordineCambiato)
            {
                Collection<string> noteInOrdine = new Collection<string>
                {
                    cbIndice.Text
                };
                AggiungiNodiALista(tvNoteOrdinate.Nodes, noteInOrdine);
                Principale.testi.SetNoteInOrdine(noteInOrdine, versione);
            }
        }

        private void AggiungiNodiALista(TreeNodeCollection nodi, Collection<string> lista)
        {
            foreach (TreeNode nodo in nodi)
            {
                lista.Add(new string('\t', nodo.Level) + nodo.Text);
                AggiungiNodiALista(nodo.Nodes, lista);
            }
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            AggiornaPulsanti();

            if (tabControl.SelectedIndex >= tabControl.TabPages.Count)
                tabControl.SelectedIndex = 0;

            switch (tabControl.SelectedTab.Name)
            {
                case "tpCommentario":
                    ActiveControl = txtNotaCommentario;
                    break;
                case "tpDizionario":
                    ActiveControl = txtNotaDizionario;
                    break;
                case "tpLibro":
                    ActiveControl = tvNoteOrdinate;
                    break;
                case "tpRiferimenti":
                    ActiveControl = txtRiferimenti;
                    break;
            }
        }

        private void AggiornaPulsanti()
        {
            if (solaLettura)
            {
                switch (tabControl.SelectedTab.Name)
                {
                    case "tpCommentario":
                        txtNotaCommentario.Enabled = false;
                        pulRiferimenti.Enabled = false;
                        btnOK.Enabled = (tvNoteCommentario.SelectedNode != null);
                        break;
                    case "tpDizionario":
                        btnOK.Enabled = (lbNoteDizionario.Items.IndexOf(txtNotaDizionario.Text) >= 0);
                        break;
                    case "tpLibro":
                        btnOK.Enabled = (tvNoteOrdinate.SelectedNode != null);
                        break;
                    case "tpRiferimenti":
                        btnOK.Enabled = (!string.IsNullOrEmpty(lbRiferimenti.Text));
                        break;
                }
                btnCancellaNota.Enabled = false;
            }
            else
            {
                switch (tabControl.SelectedTab.Name)
                {
                    case "tpCommentario":
                        btnOK.Enabled = !string.IsNullOrEmpty(txtNotaCommentario.Text);
                        btnCancellaNota.Enabled = btnOK.Enabled;
                        break;
                    case "tpDizionario":
                        btnOK.Enabled = !string.IsNullOrEmpty(txtNotaDizionario.Text);
                        btnCancellaNota.Enabled = btnOK.Enabled;
                        break;
                    case "tpLibro":
                        btnOK.Enabled = (tvNoteOrdinate.SelectedNode != null);
                        btnCancellaNota.Enabled = btnOK.Enabled;
                        break;
                    case "tpRiferimenti":
                        btnOK.Enabled = (!string.IsNullOrEmpty(lbRiferimenti.Text));
                        btnCancellaNota.Enabled = btnOK.Enabled;
                        break;
                }
            }
        }

        private void btnCancellaNota_Click(object sender, EventArgs e)
        {
            switch (tabControl.SelectedTab.Name)
            {
                case "tpCommentario":
                    if (tvNoteCommentario.SelectedNode != null && txtNotaCommentario.Text == tvNoteCommentario.SelectedNode.Text)
                    {
                        // il riferimento è da un clic sul TreeView
                        CancellaNota(tvNoteCommentario.SelectedNode.Tag.ToString());
                    }
                    else
                    {
                        // il riferimento è stato digitato
                        CancellaNota(Principale.testi.ConvertiRiferimento(txtNotaCommentario.Text).ComeNotaTuttoRiferimento());
                    }
                    AggiornaElenchi(TestoTipi.Commentario);
                    break;
                case "tpDizionario":
                    CancellaNota(txtNotaDizionario.Text);
                    AggiornaElenchi(TestoTipi.Dizionario);
                    break;
                case "tpLibro":
                    if (tvNoteOrdinate.SelectedNode != null)
                        CancellaNota(tvNoteOrdinate.SelectedNode.Text);
                    AggiornaElenchi(TestoTipi.Commentario | TestoTipi.Dizionario | TestoTipi.Libro);
                    break;
                case "tpRiferimenti":
                    if (!string.IsNullOrEmpty(lbRiferimenti.Text))
                        CancellaNota(lbRiferimenti.Text);
                    notaDeiRiferimenti.RemoveAt(lbRiferimenti.SelectedIndex);
                    lbRiferimenti.Items.RemoveAt(lbRiferimenti.SelectedIndex);
                    break;
            }
        }

        private void CancellaNota(string titolo)
        {
            if (!string.IsNullOrEmpty(Principale.testi.GetNotaTesto(titolo, versione)))
                Principale.testi.SetNotaTesto("", titolo, versione);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            switch (tabControl.SelectedTab.Name)
            {
                case "tpCommentario":
                    if (tvNoteCommentario.SelectedNode != null && txtNotaCommentario.Text == tvNoteCommentario.SelectedNode.Text)
                    {
                        // il riferimento è da un clic sul TreeView
                        ApriUnaNota(tvNoteCommentario.SelectedNode.Tag.ToString());
                    }
                    else
                    {
                        // il riferimento è stato digitato
                        ApriUnaNota(Principale.testi.ConvertiRiferimento(txtNotaCommentario.Text).ComeNotaTuttoRiferimento());
                    }
                    break;
                case "tpDizionario":
                    ApriUnaNota(txtNotaDizionario.Text);
                    break;
                case "tpLibro":
                    if (tvNoteOrdinate.SelectedNode != null)
                    {
                        String titolo = tvNoteOrdinate.SelectedNode.Text;
                        if (!string.IsNullOrEmpty(Principale.testi.GetNotaTesto(titolo, versione)))
                            ApriUnaNota(titolo);
                    }
                    break;
                case "tpRiferimenti":
                    if (lbRiferimenti.SelectedIndex >= 0)
                        ApriUnaNota(notaDeiRiferimenti[lbRiferimenti.SelectedIndex]);
                    break;
            }
        }

        private void ApriUnaNota(string titolo)
        {
            if (!string.IsNullOrEmpty(titolo))
                ((Principale)(this.MdiParent)).ApriNotaInEditor(titolo, versione);
        }

        private void btnCanc_Click(object sender, EventArgs e)
        {
            Close();
        }

        #region Scheda Commentario

        private void txtNotaCommentario_TextChanged(object sender, EventArgs e)
        {
            AggiornaPulsanti();
        }

        private void pulAutoComp_Click(object sender, EventArgs e)
        {
            using (CreaRiferimento formCreaRiferimento = new CreaRiferimento(genitore))
            {
                if (formCreaRiferimento.ShowDialog() == DialogResult.OK)
                    txtNotaCommentario.Text = Principale.testi.ConvertiTitoloNotaARiferimento(formCreaRiferimento.riferimento);
            }
        }

        private void tvNoteCommentario_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (tvNoteCommentario.SelectedNode != null && tvNoteCommentario.SelectedNode.Level >= 2)
                txtNotaCommentario.Text = tvNoteCommentario.SelectedNode.Text;
        }

        private void tvNoteCommentario_DoubleClick(object sender, EventArgs e)
        {
            if (tvNoteCommentario.SelectedNode != null && tvNoteCommentario.SelectedNode.Level >= 2)
                btnOK_Click(sender, e);
        }

        #endregion

        #region Scheda Dizionario

        private void txtNotaDizionario_TextChanged(object sender, EventArgs e)
        {
            if (txtNotaDizionario.Text.StartsWith("#", StringComparison.Ordinal))
                txtNotaDizionario.Text = txtNotaDizionario.Text.Remove(0, 1); // note che iniziano con # sono per i versetti
            if (!string.IsNullOrEmpty(txtNotaDizionario.Text))
                lbNoteDizionario.SelectedIndex = lbNoteDizionario.FindString(txtNotaDizionario.Text);
            else
                lbNoteDizionario.SelectedIndex = -1;
            AggiornaPulsanti();
        }

        private void lbNoteDizionario_Click(object sender, EventArgs e)
        {
            if (lbNoteDizionario.SelectedItem != null)
                txtNotaDizionario.Text = lbNoteDizionario.SelectedItem.ToString();
        }

        private void lbNoteDizionario_DoubleClick(object sender, EventArgs e)
        {
            ApriUnaNota(txtNotaDizionario.Text);
        }

        #endregion

        #region Scheda Ordine

        private void cbIndice_SelectedIndexChanged(object sender, EventArgs e)
        {
            ordineCambiato = true;
        }

        private void tvNoteOrdinate_DoubleClick(object sender, EventArgs e)
        {
            String titolo = tvNoteOrdinate.SelectedNode.Text;
            if (!string.IsNullOrEmpty(Principale.testi.GetNotaTesto(titolo, versione)))
                ApriUnaNota(titolo);
        }

        private void tvNoteOrdinate_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            ordineCambiato = true;
        }

        private void tvNoteOrdinate_AfterSelect(object sender, TreeViewEventArgs e)
        {
            AggiornaPulsanti();
        }

        private void tvNoteOrdinate_MouseDown(object sender, MouseEventArgs e)
        {
            if (tvNoteOrdinate.GetNodeAt(e.X, e.Y) != null)
            {
                Size dragSize = SystemInformation.DragSize;
                dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2),
                                                               e.Y - (dragSize.Height / 2)), dragSize);
            }
            else
                dragBoxFromMouseDown = Rectangle.Empty;
        }

        private void tvNoteOrdinate_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                // se il mouse si sposta fuori dal rettangolo, inizia il drag-drop
                if (dragBoxFromMouseDown != Rectangle.Empty && !dragBoxFromMouseDown.Contains(e.X, e.Y))
                {
                    TreeNode nodo = tvNoteOrdinate.GetNodeAt(e.X, e.Y);
                    if (nodo != null)
                        tvNoteOrdinate.DoDragDrop(nodo, DragDropEffects.Move);
                }
            }

        }

        private void tvNoteOrdinate_MouseUp(object sender, MouseEventArgs e)
        {
            dragBoxFromMouseDown = Rectangle.Empty;
        }

        private void tvNoteOrdinate_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TreeNode)))
                e.Effect = DragDropEffects.Move;
            else
                e.Effect = DragDropEffects.None;
        }

        private void tvNoteOrdinate_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TreeNode)))
            {
                TreeNode nodo = tvNoteOrdinate.GetNodeAt(tvNoteOrdinate.PointToClient(new Point(e.X, e.Y)));
                if (nodo != null)
                {
                    ordineCambiato = true;
                    TreeNode nodoDaInserire = (TreeNode)((TreeNode)(e.Data.GetData(typeof(TreeNode)))).Clone();
                    if (nodo.Parent != null)
                        nodo.Parent.Nodes.Insert(nodo.Index, nodoDaInserire);
                    else
                        tvNoteOrdinate.Nodes.Insert(nodo.Index, nodoDaInserire);
                    tvNoteOrdinate.Nodes.Remove((TreeNode)(e.Data.GetData(typeof(TreeNode))));
                }
            }
        }

        private void pmNoteInOrdine_Opening(object sender, CancelEventArgs e)
        {
            bool voceSelezionata = (tvNoteOrdinate.SelectedNode != null);
            addbeforeToolStripMenuItem.Enabled = voceSelezionata;
            addafterToolStripMenuItem.Enabled = (voceSelezionata || tvNoteOrdinate.Nodes.Count == 0);
            addunderToolStripMenuItem.Enabled = voceSelezionata;
            removeToolStripMenuItem.Enabled = voceSelezionata;
        }

        private void noteInOrdineMenuItem_Click(object sender, EventArgs e)
        {
            ordineCambiato = true;
            switch (Convert.ToInt32(((ToolStripItem)sender).Tag, CultureInfo.InvariantCulture))
            {
                case 0: // add before
                    if (tvNoteOrdinate.SelectedNode.Parent != null)
                        tvNoteOrdinate.SelectedNode.Parent.Nodes.Insert(tvNoteOrdinate.SelectedNode.Index, tvNoteOrdinate.Tag.ToString());
                    else
                        tvNoteOrdinate.Nodes.Insert(tvNoteOrdinate.SelectedNode.Index, tvNoteOrdinate.Tag.ToString());
                    break;
                case 1: // add after
                    if (tvNoteOrdinate.Nodes.Count == 0)
                    {
                        tvNoteOrdinate.Nodes.Add(tvNoteOrdinate.Tag.ToString());
                    }
                    else
                    {
                        if (tvNoteOrdinate.SelectedNode.NextNode != null)
                        {
                            if (tvNoteOrdinate.SelectedNode.Parent != null)
                                tvNoteOrdinate.SelectedNode.Parent.Nodes.Insert(tvNoteOrdinate.SelectedNode.NextNode.Index, tvNoteOrdinate.Tag.ToString());
                            else
                                tvNoteOrdinate.Nodes.Insert(tvNoteOrdinate.SelectedNode.NextNode.Index, tvNoteOrdinate.Tag.ToString());
                        }
                        else
                        {
                            if (tvNoteOrdinate.SelectedNode.Parent != null)
                                tvNoteOrdinate.SelectedNode.Parent.Nodes.Add(tvNoteOrdinate.Tag.ToString());
                            else
                                tvNoteOrdinate.Nodes.Add(tvNoteOrdinate.Tag.ToString());
                        }
                    }
                    break;
                case 2: // add under
                    tvNoteOrdinate.SelectedNode.Nodes.Add(tvNoteOrdinate.Tag.ToString());
                    break;
                case 3: // remove
                    tvNoteOrdinate.SelectedNode.Remove();
                    break;
            }
        }

        #endregion

        #region Scheda Riferimenti

        private void txtRiferimenti_TextChanged(object sender, EventArgs e)
        {
            pulRiferimenti.Enabled = !string.IsNullOrEmpty(txtRiferimenti.Text);
        }

        private void pulRiferimenti_Click(object sender, EventArgs e)
        {
            Riferimento note = Principale.testi.Citazioni(Principale.testi.ConvertiRiferimento(txtRiferimenti.Text), versione);
            lbRiferimenti.BeginUpdate();
            lbRiferimenti.Items.Clear();
            foreach (string nota in note.Note)
            {
                if (nota.StartsWith("#", StringComparison.Ordinal))
                    lbRiferimenti.Items.Add(Principale.testi.ConvertiTitoloNotaARiferimento(nota));
                else
                    lbRiferimenti.Items.Add(nota);
            }
            lbRiferimenti.EndUpdate();
            notaDeiRiferimenti.Clear();
            notaDeiRiferimenti.AddRange(note.Note);
        }

        private void lbRiferimenti_SelectedIndexChanged(object sender, EventArgs e)
        {
            AggiornaPulsanti();
        }

        private void lbRiferimenti_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!string.IsNullOrEmpty(lbRiferimenti.Text))
                btnOK_Click(sender, e);
        }

        #endregion

        private void ApriNota_Resize(object sender, EventArgs e)
        {
            tabControl.Size = new Size(Width - 27, Height - 77);
            txtNotaCommentario.Width = Width - 208;
            tvNoteCommentario.Size = new Size(Width - 47, Height - 153);
            txtNotaDizionario.Width = Width - 127;
            lbNoteDizionario.Size = new Size(Width - 47, Height - 153);
            cbIndice.Width = Width - 127;
            tvNoteOrdinate.Size = new Size(Width - 47, Height - 153);
            txtRiferimenti.Width = Width - 208;
            lbRiferimenti.Size = new Size(Width - 47, Height - 153);
        }

    }
}