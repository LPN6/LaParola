using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Windows.Forms;
using LaParola.Properties;
using TestiBiblici;

namespace LaParola
{
    public partial class Segnalibri : Template
    {
        private Principale genitore;
        private Visualizza formVisualizza = null;

        private TreeNode nodoDaTrascinare = null;
        private Rectangle rettangoloPerDrag = Rectangle.Empty;

        public Segnalibri(Principale formGenitore)
        {
            if (formGenitore == null)
                throw new ArgumentNullException("formGenitore");

            InitializeComponent();
            genitore = formGenitore;
            guidaFile.HelpNamespace = formGenitore.NomeFileGuida();

            AggiornaListaSegnalibri();
        }

        public void AggiornaListaSegnalibri()
        {
            tvSegnalibri.BeginUpdate();
            tvSegnalibri.Nodes.Clear();
            int numeroFileSegnalibri = genitore.bookmarksToolStripMenuItem.DropDownItems.Count;
            for (int i = 4; i < numeroFileSegnalibri - 2; ++i) // per non mettere "Segnalibri veloci", "Capitoli" e "Modifica"
            {
                TreeNode nodo = tvSegnalibri.Nodes.Add(genitore.bookmarksToolStripMenuItem.DropDownItems[i].Text);
                nodo.Tag = genitore.bookmarksToolStripMenuItem.DropDownItems[i].Tag;
                nodo.ToolTipText = genitore.bookmarksToolStripMenuItem.DropDownItems[i].ToolTipText;
                AggiungiSottoNodi(nodo, genitore.bookmarksToolStripMenuItem.DropDownItems[i]);
            }
            tvSegnalibri.EndUpdate();
        }

        private void AggiungiSottoNodi(TreeNode nodo, ToolStripItem menuVoce)
        {
            foreach (ToolStripItem sottoVoce in ((ToolStripMenuItem)(menuVoce)).DropDownItems)
            {
                TreeNode sottoNodo = nodo.Nodes.Add(sottoVoce.Text);
                sottoNodo.Tag = sottoVoce.Tag;
                AggiungiSottoNodi(sottoNodo, sottoVoce);
            }
        }

        private void Segnalibri_Resize(object sender, EventArgs e)
        {
            tvSegnalibri.Size = new Size(Width - 131, Height - 84);
        }

        private void tvSegnalibri_DoubleClick(object sender, EventArgs e)
        {
            if (formVisualizza != null && formVisualizza.Parent == null)
                formVisualizza = null;
            if (tvSegnalibri.SelectedNode.Level > 0 && !string.IsNullOrEmpty(tvSegnalibri.SelectedNode.Tag.ToString()))
                formVisualizza = genitore.MostraSegnalibro(tvSegnalibri.SelectedNode.Tag.ToString(), formVisualizza);
        }

        private void tvSegnalibri_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (tvSegnalibri.SelectedNode.Level > 0)
            {
                etiDescrizione.Visible = false;
                etiRiferimento.Visible = true;
                if (tvSegnalibri.SelectedNode.Tag != null)
                    tbRiferimento.Text = Principale.testi.NormalizzaRiferimentoSegnalibro(tvSegnalibri.SelectedNode.Tag.ToString());
            }
            else
            {
                etiDescrizione.Visible = true;
                etiRiferimento.Visible = false;
                tbRiferimento.Text = tvSegnalibri.SelectedNode.ToolTipText;
            }

            bool nodoSelezionato = (tvSegnalibri.SelectedNode != null);
            pulAggiungi.Enabled = nodoSelezionato;
            pulApri.Enabled = nodoSelezionato && (tvSegnalibri.SelectedNode.Level >= 1);
            pulMostra.Enabled = nodoSelezionato;
            pulModifica.Enabled = nodoSelezionato;
            pulCancella.Enabled = nodoSelezionato;
            pulSalva.Enabled = nodoSelezionato;
        }

        private void tbRiferimento_TextChanged(object sender, EventArgs e)
        {
            if (tvSegnalibri.SelectedNode != null)
            {
                if (tvSegnalibri.SelectedNode.Level == 0)
                {
                    tvSegnalibri.SelectedNode.ToolTipText = tbRiferimento.Text;
                }
                else
                {
                    Riferimento riferimento = Principale.testi.ConvertiRiferimento(tbRiferimento.Text);
                    string riferimentoComeTesto = "";
                    foreach (byte[] brano in riferimento.Brani)
                        riferimentoComeTesto += brano[0] + " " + brano[1] + " " + brano[2] + " " + brano[3] + " " + brano[4] + " " + brano[5] + ";";
                    if (riferimentoComeTesto.EndsWith(";", StringComparison.Ordinal))
                        riferimentoComeTesto = riferimentoComeTesto.Remove(riferimentoComeTesto.Length - 1);
                    tvSegnalibri.SelectedNode.Tag = riferimentoComeTesto;
                }
            }
        }

        private void pulApri_Click(object sender, EventArgs e)
        {
            if (tvSegnalibri.SelectedNode != null)
                tvSegnalibri_DoubleClick(sender, e);
        }

        private void pulMostra_Click(object sender, EventArgs e)
        {
            if (tvSegnalibri.SelectedNode != null)
            {
                Riferimento riferimentoConBrani = new Riferimento();
                riferimentoConBrani.AggiungiBraniDaRiferimento(RiferimentiDeiNodi(tvSegnalibri.SelectedNode));
                genitore.MostraBranoInEditor(Principale.testi.ConvertiDaStandard(riferimentoConBrani, Principale.testi.UltimaBibbia), Principale.testi.UltimaBibbia);
            }
        }

        private Riferimento RiferimentiDeiNodi(TreeNode nodo)
        {
            Riferimento riferimentoConBrani = new Riferimento();
            if (nodo.Tag != null)
            {
                string[] riferimenti = nodo.Tag.ToString().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string riferimento in riferimenti)
                {
                    try
                    {
                        string[] riferimentoDiviso = riferimento.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (riferimentoDiviso.Length >= 6)
                        {
                            riferimentoConBrani.AggiungiBrano(new byte[] { Convert.ToByte(riferimentoDiviso[0], CultureInfo.InvariantCulture), Convert.ToByte(riferimentoDiviso[1], CultureInfo.InvariantCulture), Convert.ToByte(riferimentoDiviso[2], CultureInfo.InvariantCulture), Convert.ToByte(riferimentoDiviso[3], CultureInfo.InvariantCulture), Convert.ToByte(riferimentoDiviso[4], CultureInfo.InvariantCulture), Convert.ToByte(riferimentoDiviso[5], CultureInfo.InvariantCulture) });
                        }
                        else if (riferimentoDiviso.Length >= 3)
                        {
                            riferimentoConBrani.AggiungiBrano(new byte[] { Convert.ToByte(riferimentoDiviso[0], CultureInfo.InvariantCulture), Convert.ToByte(riferimentoDiviso[1], CultureInfo.InvariantCulture), Convert.ToByte(riferimentoDiviso[2], CultureInfo.InvariantCulture), Convert.ToByte(riferimentoDiviso[0], CultureInfo.InvariantCulture), Convert.ToByte(riferimentoDiviso[1], CultureInfo.InvariantCulture), Convert.ToByte(riferimentoDiviso[2], CultureInfo.InvariantCulture) });
                        }
                    }
                    catch
                    { // non nel formato corretto (per esempio radice di un gruppo di segnalibri ha il percorso del file, non un riferimento) quindi saltiamo
                    }
                }
            }
            foreach (TreeNode sottoNodo in nodo.Nodes)
                riferimentoConBrani.AggiungiBraniDaRiferimento(RiferimentiDeiNodi(sottoNodo));
            return riferimentoConBrani;
        }

        private void pulAggiungi_Click(object sender, EventArgs e)
        {
            if (tvSegnalibri.SelectedNode != null)
            {
                TreeNode nodoInserito = (tvSegnalibri.SelectedNode.Level == 0 ? nodoInserito = tvSegnalibri.Nodes.Insert(tvSegnalibri.SelectedNode.Index + 1, Principale.LocRM.GetString("BookmarkNew")) : tvSegnalibri.SelectedNode.Parent.Nodes.Insert(tvSegnalibri.SelectedNode.Index + 1, Principale.LocRM.GetString("BookmarkNew")));
                if (nodoInserito != null)
                {
                    nodoInserito.Tag = "";
                    tvSegnalibri.SelectedNode = nodoInserito;
                    nodoInserito.BeginEdit();
                }
            }
        }

        private void pulModifica_Click(object sender, EventArgs e)
        {
            if (tvSegnalibri.SelectedNode != null)
                tvSegnalibri.SelectedNode.BeginEdit();
        }

        private void pulCancella_Click(object sender, EventArgs e)
        {
            if (tvSegnalibri.SelectedNode != null)
            {
                if (tvSegnalibri.SelectedNode.Level == 0)
                {
                    if (MessageBox.Show(Principale.LocRM.GetString("BookmarkDelete"), Principale.LocRM.GetString("MiscConfirm"), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions) == DialogResult.No)
                        return;
                    string[] tagInfo = tvSegnalibri.SelectedNode.Tag.ToString().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    string nomeFile = ((tagInfo.Length >= 1) ? nomeFile = tagInfo[0] : nomeFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + "Segnalibri" + Path.DirectorySeparatorChar + tvSegnalibri.SelectedNode.Text + ".xml");
                    try
                    {
                        File.Delete(nomeFile);
                    }
                    catch { }
                    RinnovaTutteListeDiSegnalibri();
                }
                TreeNode nuovoNodoSelezionato = (tvSegnalibri.SelectedNode.Level == 0 ? tvSegnalibri.Nodes[0] : tvSegnalibri.SelectedNode.Parent);
                tvSegnalibri.SelectedNode.Remove();
                tvSegnalibri.SelectedNode = nuovoNodoSelezionato;
                ActiveControl = tvSegnalibri;
            }
        }

        private void pulSalva_Click(object sender, EventArgs e)
        {
            SalvaRamo(true);
        }

        private void SalvaRamo(bool chiediConferma)
        {
            if (tvSegnalibri.SelectedNode != null)
            {
                TreeNode nodoGenitore = tvSegnalibri.SelectedNode;
                while (nodoGenitore.Parent != null)
                    nodoGenitore = nodoGenitore.Parent;
                string[] tagInfo = nodoGenitore.Tag.ToString().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                string nomeFile = ((tagInfo.Length >= 1) ? nomeFile = tagInfo[0] : nomeFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + "Segnalibri" + Path.DirectorySeparatorChar + nodoGenitore.Text + ".xml");
                if (File.Exists(nomeFile) && chiediConferma)
                {
                    if (MessageBox.Show(Principale.LocRM.GetString("BookmarkFileExists"), Principale.LocRM.GetString("MiscConfirm"), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions) == DialogResult.No)
                        return;
                }

                string testoFile = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>";
                testoFile += Environment.NewLine + "<bookmarks>";
                testoFile += Environment.NewLine + "<name>" + nodoGenitore.Text + "</name>";
                if (!string.IsNullOrEmpty(nodoGenitore.ToolTipText))
                    testoFile += Environment.NewLine + "<description>" + nodoGenitore.ToolTipText + "</description>";
                foreach (TreeNode nodo in nodoGenitore.Nodes)
                    ScriviNodi(nodo, ref testoFile);
                testoFile += Environment.NewLine + "</bookmarks>";
                File.WriteAllText(nomeFile, testoFile);

                RinnovaTutteListeDiSegnalibri();
            }
        }

        private void RinnovaTutteListeDiSegnalibri()
        {
            Cursor cursoreAttuale = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                genitore.CreaMenuSegnalibri();
                foreach (Form f in genitore.MdiChildren)
                {
                    if (f.Tag != null && f.Tag.ToString() == "Ricerca")
                        ((Ricerca)f).CreaListaListaVersetti();
                }
            }
            finally
            {
                Cursor.Current = cursoreAttuale;
                if (cursoreAttuale != null)
                    cursoreAttuale.Dispose();
            }
        }

        private void ScriviNodi(TreeNode nodo, ref string testoFile)
        {
            testoFile += Environment.NewLine + "<bookmark>";
            testoFile += Environment.NewLine + "<name>" + nodo.Text + "</name>";
            if (!string.IsNullOrEmpty(nodo.Tag.ToString()))
                testoFile += Environment.NewLine + "<reference>" + nodo.Tag.ToString() + "</reference>";

            foreach (TreeNode sottoNodo in nodo.Nodes)
                ScriviNodi(sottoNodo, ref testoFile);
            testoFile += Environment.NewLine + "</bookmark>";
        }

        #region Importa

        private void pulImporta_Click(object sender, EventArgs e)
        {
            menuImporta.Show(pulImporta, 0, pulImporta.Height);
        }

        private void menuImportaFile_Click(object sender, EventArgs e)
        {
            string nomeFile = "";
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                openFileDialog.Multiselect = false;
                openFileDialog.CheckFileExists = true;
                openFileDialog.CheckPathExists = true;
                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                    nomeFile = openFileDialog.FileName;
            }
            if (!string.IsNullOrEmpty(nomeFile))
            {
                string testo = "";
                try
                {
                    testo = File.ReadAllText(nomeFile);
                }
                catch (Exception exc)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("BookmarkImportFileError"), exc.Message), Principale.LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions);
                    return;
                }
                ImportaRiferimento(Principale.testi.ConvertiRiferimenti(testo));
            }
        }

        private void menuImportaInternet_Click(object sender, EventArgs e)
        {
            string indirizzo = "";
            using (InputBox ibForm = new InputBox(Principale.LocRM.GetString("BookmarkImportWebInputTitle"), Principale.LocRM.GetString("BookmarkImportWebInputCaption"), "http://"))
            {
                ibForm.ShowDialog();
                indirizzo = ibForm.Risposta;
            }
            if (!String.IsNullOrEmpty(indirizzo) && indirizzo != "http://")
            {
                string proxyHost = Settings.Default.AggiornamentoProxyHost;
                string credentialUtente = Settings.Default.AggiornamentoProxyNomeUtente;
                WebClient webClient = new WebClient();

                if (!string.IsNullOrEmpty(proxyHost))
                {
                    int proxyPorta = Settings.Default.AggiornamentoProxyPorta;
                    if (proxyPorta == 0)
                        webClient.Proxy = new WebProxy(proxyHost);
                    else
                        webClient.Proxy = new WebProxy(proxyHost, proxyPorta);
                    if (!string.IsNullOrEmpty(credentialUtente))
                    {
                        string credentialPassword = Settings.Default.AggiornamentoProxyPassword;
                        string credentialDominio = Settings.Default.AggiornamentoProxyDominio;
                        if (string.IsNullOrEmpty(credentialDominio))
                            webClient.Proxy.Credentials = new NetworkCredential(credentialUtente, credentialPassword);
                        else
                            webClient.Proxy.Credentials = new NetworkCredential(credentialUtente, credentialPassword, credentialDominio);
                    }
                }
                string testoHtml = "";
                Cursor cursoreAttuale = null;
                try
                {
                    cursoreAttuale = Cursor.Current;
                    Cursor.Current = Cursors.WaitCursor;
                    testoHtml = webClient.DownloadString(indirizzo);
                }
                catch (WebException exc)
                {
                    Cursor.Current = cursoreAttuale;
                    if (cursoreAttuale != null)
                        cursoreAttuale.Dispose();
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("BookmarkImportWebError"), exc.Message), Principale.LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions);
                    return;
                }
                Cursor.Current = cursoreAttuale;
                if (cursoreAttuale != null)
                    cursoreAttuale.Dispose();
                int p1, p2;
                while (testoHtml.IndexOf("<", StringComparison.Ordinal) >= 0)
                {
                    p1 = testoHtml.IndexOf("<", StringComparison.Ordinal);
                    p2 = testoHtml.IndexOf(">", p1, StringComparison.Ordinal);
                    if (p2 > 0)
                        testoHtml = testoHtml.Substring(0, p1) + testoHtml.Substring(p2 + 1);
                    else
                        testoHtml = testoHtml.Remove(p1);
                }
                ImportaRiferimento(Principale.testi.ConvertiRiferimenti(testoHtml));
            }
        }

        private void menuImportaClipboard_Click(object sender, EventArgs e)
        {
            string clipboard = "";
            try
            {
                clipboard = Clipboard.GetText();
            }
            catch { } // a volte la riga precedente dà un External Exception; non so perché
            ImportaRiferimento(Principale.testi.ConvertiRiferimenti(clipboard));

        }

        private void ImportaRiferimento(Riferimento riferimento)
        {
            if (riferimento.Count == 0)
                MessageBox.Show(Principale.LocRM.GetString("BookmarkImportNoReference"), Principale.LocRM.GetString("MiscInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions);
            else
            {
                TreeNode nodo = tvSegnalibri.Nodes.Add(Principale.LocRM.GetString("BookmarkImportName"));
                nodo.Tag = "";
                foreach (byte[] brano in riferimento.Brani)
                {
                    TreeNode sottoNodo = nodo.Nodes.Add(Principale.testi.NormalizzaRiferimento(brano[0], brano[1], brano[2], brano[3], brano[4], brano[5]));
                    sottoNodo.Tag = brano[0].ToString(CultureInfo.InvariantCulture) + " " + brano[1].ToString(CultureInfo.InvariantCulture) + " " + brano[2].ToString(CultureInfo.InvariantCulture) + " " + brano[3].ToString(CultureInfo.InvariantCulture) + " " + brano[4].ToString(CultureInfo.InvariantCulture) + " " + brano[5].ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        #endregion

        private void btnCanc_Click(object sender, EventArgs e)
        {
            Close();
        }

        #region DragDrop

        private void tvSegnalibri_MouseDown(object sender, MouseEventArgs e)
        {
            nodoDaTrascinare = tvSegnalibri.GetNodeAt(e.X, e.Y);
            if (nodoDaTrascinare != null)
                rettangoloPerDrag = new Rectangle(new Point(e.X - (SystemInformation.DragSize.Width / 2), e.Y - (SystemInformation.DragSize.Height / 2)), SystemInformation.DragSize);
            else
                rettangoloPerDrag = Rectangle.Empty;
        }

        private void tvSegnalibri_MouseUp(object sender, MouseEventArgs e)
        {
            rettangoloPerDrag = Rectangle.Empty;
            nodoDaTrascinare = null;
        }

        private void tvSegnalibri_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                if (rettangoloPerDrag != Rectangle.Empty && !rettangoloPerDrag.Contains(e.X, e.Y))
                {
                    if (nodoDaTrascinare != null)
                        tvSegnalibri.DoDragDrop(nodoDaTrascinare, DragDropEffects.Move);
                }
            }
        }

        private void tvSegnalibri_DragDrop(object sender, DragEventArgs e)
        {
            TreeNode nodoDrag = (TreeNode)(e.Data.GetData("System.Windows.Forms.TreeNode"));
            if (nodoDrag != null)
            {
                TreeNode nodoDrop = tvSegnalibri.GetNodeAt(tvSegnalibri.PointToClient(new Point(e.X, e.Y)));
                if (nodoDrop != null)
                {
                    // dopo il remove, l'indice è uno di meno nel seguente caso
                    int sposta = ((nodoDrag.Parent == nodoDrop.Parent && nodoDrag.Index < nodoDrop.Index) ? 1 : 0);
                    nodoDrag.Remove();

                    TreeNodeCollection nodiPerDrop;
                    if (nodoDrop.Parent != null)
                        nodiPerDrop = nodoDrop.Parent.Nodes;
                    else
                        nodiPerDrop = tvSegnalibri.Nodes;

                    TreeViewHitTestLocations luogoDrop = tvSegnalibri.HitTest(tvSegnalibri.PointToClient(new Point(e.X, e.Y))).Location;
                    if ((luogoDrop & TreeViewHitTestLocations.Label) == TreeViewHitTestLocations.Label
                        || (luogoDrop & TreeViewHitTestLocations.RightOfLabel) == TreeViewHitTestLocations.RightOfLabel)
                        nodiPerDrop.Insert(nodoDrop.Index + sposta, nodoDrag);
                    else if ((luogoDrop & TreeViewHitTestLocations.Indent) == TreeViewHitTestLocations.Indent
                        || (luogoDrop & TreeViewHitTestLocations.PlusMinus) == TreeViewHitTestLocations.PlusMinus)
                        nodoDrop.Nodes.Add(nodoDrag);

                    tvSegnalibri.SelectedNode = nodoDrag;
                }
            }
        }

        private void tvSegnalibri_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TreeNode)))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
                e.Effect = DragDropEffects.None;
        }

        #endregion

        internal void AggiungiSegnalibroPersonale(byte libro, byte capitolo, byte versetto)
        {
            TreeNode nodo = tvSegnalibri.Nodes[0];
            while (nodo != null)
            {
                if (nodo.Tag.ToString().IndexOf(Path.DirectorySeparatorChar + "Personal.xml", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    AggiungiSegnalibro(nodo, libro, capitolo, versetto);
                    break;
                }
                nodo = nodo.NextNode;
            }
        }

        private void AggiungiSegnalibro(TreeNode nodo, byte libro, byte capitolo, byte versetto)
        {
            TreeNode nodoInserito = nodo.Nodes.Add(Principale.testi.NormalizzaRiferimento(libro, capitolo, versetto));
            if (nodoInserito != null)
            {
                nodoInserito.Tag = libro + " " + capitolo + " " + versetto;
                tvSegnalibri.SelectedNode = nodoInserito;
                SalvaRamo(false);
                nodoInserito.BeginEdit();
            }
        }

    }
}


