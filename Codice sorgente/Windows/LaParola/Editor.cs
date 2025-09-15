using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using LaParola.Properties;
using TestiBiblici;

namespace LaParola
{
    public partial class Editor : Form
    {
        #region Proprietà

        private Principale genitore;

        private bool inizializzazione = true;
        private int posizioneUltimoClic = -1;
        private Rectangle dragBoxFromMouseDown;

        private string versionePerIpertesto;
        /// <summary>
        /// La nomeVersione da usare per salti ipertestuali da questa finestra.
        /// </summary>
        public string VersionePerIpertesto
        {
            get { return versionePerIpertesto; }
            set { versionePerIpertesto = value; }
        }

        private string nomeFile = "";
        public string NomeFile
        {
            get { return nomeFile; }
        }

        private bool mostraOrdine;
        public bool MostraOrdine
        {
            get { return mostraOrdine; }
            set { mostraOrdine = value; }
        }
        private string notaPrecedente;
        public string NotaPrecedente
        {
            get { return notaPrecedente; }
            set { notaPrecedente = value; }
        }
        private string notaProssima;
        public string NotaProssima
        {
            get { return notaProssima; }
            set { notaProssima = value; }
        }
        private string notaIndice;
        public string NotaIndice
        {
            get { return notaIndice; }
            set { notaIndice = value; }
        }

        #endregion

        #region Costruttore

        public Editor(Principale formGenitore)
        {
            CostruttoreComune(formGenitore);
        }

        public Editor(Principale formGenitore, string nomeFile)
        {
            CostruttoreComune(formGenitore);
            inizializzazione = true;
            ApriFile(nomeFile);
            //            inizializzazione = false;
        }

        public Editor(Principale formGenitore, string nomeNota, string nomeVersione)
        {
            CostruttoreComune(formGenitore);
            rtEditor.Versione = nomeVersione;
            nomeFile = nomeNota;
        }

        private void CostruttoreComune(Principale formGenitore)
        {
            genitore = formGenitore;

            InitializeComponent();
            rtEditor.Modified = false;
            FontStyle fs = FontStyle.Regular;
            if (Principale.testi.Formato.FontGrassetto)
                fs &= FontStyle.Bold;
            if (Principale.testi.Formato.FontCorsivo)
                fs &= FontStyle.Italic;
            if (Principale.testi.Formato.FontSottolineato)
                fs &= FontStyle.Underline;
            try
            {
                rtEditor.Font = new Font(Principale.testi.Formato.FontNome, Principale.testi.Formato.FontDimensione, fs);
            }
            catch (ArgumentException)
            { // con un computer, la riga precedente ha dato un errore che Palatino Linotype non ha lo stile normale. Proviamo in altri modi...
                try
                {
                    rtEditor.Font = new Font(Principale.testi.Formato.FontNome, Principale.testi.Formato.FontDimensione);
                }
                catch (ArgumentException)
                {
                }
            }

            rtEditor.LinkHoverEvento += new EventHandler<LinkHoverEventArgs>(LinkHover);
            rtEditor.MouseWheel += new MouseEventHandler(Rtb_MouseWheel);

            rtEditor.AllowDrop = true;
            rtEditor.DragEnter += new DragEventHandler(RtEditor_DragEnter);
            rtEditor.DragDrop += new DragEventHandler(RtEditor_DragDrop);

            mostraOrdine = false;
            notaPrecedente = "";
            notaProssima = "";
            notaIndice = "";

            AggiornaMenuCollezioni();

            // la localizzazione delle Tag non funziona bene, quindi settiamo qui
            informationOnVerseToolStripMenuItem.Tag = informationOnVerseToolStripMenuItem.Text;
            browseToolStripMenuItem.Tag = browseToolStripMenuItem.Text;
            noteOnVerseToolStripMenuItem.Tag = noteOnVerseToolStripMenuItem.Text;
            informationOnWordToolStripMenuItem.Tag = informationOnWordToolStripMenuItem.Text;
            noteOnVerseToolStripMenuItem.Tag = noteOnVerseToolStripMenuItem.Text;

            inizializzazione = false;
        }

        public void AggiornaMenuCollezioni()
        {
            Collection<string> commentari = Principale.testi.NomiVersioni(TestoTipi.Commentario);
            foreach (string commentario in commentari)
                noteOnVerseToolStripMenuItem.DropDownItems.Add(commentario, null, NoteOnVerseClick);
        }

        #endregion

        #region Menu File

        public void ApriFile(string nomeFileDaAprire)
        {
            // bisogna sempre chiamare rtEditor.MostraLink() dopo aver chiamato questo
            Stream streamDaFile = null;
            try
            {
                streamDaFile = new FileStream(nomeFileDaAprire, FileMode.Open);
                FileInfo infoDelFile = new FileInfo(nomeFileDaAprire);
                nomeFile = nomeFileDaAprire;
                Text = Path.GetFileName(nomeFileDaAprire);

                if (String.Compare(infoDelFile.Extension, ".rtf", StringComparison.OrdinalIgnoreCase) == 0)
                    rtEditor.LoadFile(streamDaFile, RichTextBoxStreamType.RichText);
                else
                    rtEditor.LoadFile(streamDaFile, RichTextBoxStreamType.PlainText);
            }
            catch (Exception exc)
            {
                MessageBox.Show(string.Format(CultureInfo.CurrentCulture, Principale.LocRM.GetString("EditorErrorCantOpenFile"), nomeFileDaAprire, exc.Message), Principale.LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions);
            }
            finally
            {
                rtEditor.Modified = false;
                if (streamDaFile != null)
                    streamDaFile.Close();
            }
        }

        public void SalvaFile(string nome)
        {
            if (String.IsNullOrEmpty(nome))
                SalvaCome();
            else
            {
                if (string.IsNullOrEmpty(rtEditor.Versione)) // è un testo normale e va salvato come file normale
                {
                    FileStream streamDelFile = null;
                    try
                    {
                        if (File.Exists(nome))
                            streamDelFile = new FileStream(nome, FileMode.Open);
                        else
                            streamDelFile = new FileStream(nome, FileMode.Create);

                        if (nome.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                            rtEditor.SaveFile(streamDelFile, RichTextBoxStreamType.PlainText);
                        else
                            rtEditor.SaveFile(streamDelFile, RichTextBoxStreamType.RichText);
                        rtEditor.Modified = false;
                        nomeFile = nome;
                        Text = Path.GetFileName(nome);
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(string.Format(CultureInfo.CurrentCulture, Principale.LocRM.GetString("EditorErrorCantOpenFile"), nome, exc.Message), Principale.LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions);
                    }
                    finally
                    {
                        if (streamDelFile != null)
                        {
                            streamDelFile.Flush();
                            streamDelFile.Close();
                        }
                    }
                }
                else // è una nota e il testo va salvato nel file della nomeVersione
                {
                    genitore.CreaNuovaNota(String.IsNullOrEmpty(rtEditor.Text) ? "" : rtEditor.Rtf, nome, rtEditor.Versione);
                    rtEditor.Modified = false;
                }
            }
        }

        public void SalvaCome()
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                string ultimaCartella = Settings.Default.UltimaDirectory;
                if (String.IsNullOrEmpty(ultimaCartella))
                    ultimaCartella = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                saveFileDialog.InitialDirectory = ultimaCartella;
                saveFileDialog.Filter = Principale.LocRM.GetString("EditorSaveFilter");
                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    Settings.Default.UltimaDirectory = Path.GetDirectoryName(saveFileDialog.FileName);
                    rtEditor.Versione = ""; // diventa un file normale
                    SalvaFile(saveFileDialog.FileName);
                }
            }
        }

        private void Editor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (rtEditor.Modified)
            {
                string nomeFileVisualizzato = nomeFile;
                if (String.IsNullOrEmpty(nomeFileVisualizzato) || nomeFile.StartsWith("#", StringComparison.Ordinal))
                    nomeFileVisualizzato = rtEditor.Parent.Text;
                DialogResult dialogoRisultato = MessageBox.Show(string.Format(CultureInfo.CurrentCulture, Principale.LocRM.GetString("EditorSaveChanges"), nomeFileVisualizzato), Principale.LocRM.GetString("MiscConfirm"), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions);
                if (dialogoRisultato == DialogResult.Yes)
                    SalvaFile(nomeFile);
                if (dialogoRisultato == DialogResult.Cancel)
                    e.Cancel = true;
            }
            if (!e.Cancel && this == genitore.finestraRisultati)
                genitore.finestraRisultati = null;
        }

        #endregion

        #region Mouse Eventi

        void Rtb_MouseWheel(object sender, MouseEventArgs e)
        {
            genitore.VisualizzaZoom();
        }

        private void RtEditor_MouseDown(object sender, MouseEventArgs e)
        {
            posizioneUltimoClic = rtEditor.GetCharIndexFromPosition(e.Location);
            if (rtEditor.SelectionLength > 0)
            {
                // Create a rectangle using the DragSize, with the mouse position being at the center of the rectangle.
                Size dragSize = SystemInformation.DragSize;
                dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
            }
            else
            {
                dragBoxFromMouseDown = Rectangle.Empty;
            }
        }

        private void RtEditor_MouseUp(object sender, MouseEventArgs e)
        {
            dragBoxFromMouseDown = Rectangle.Empty;
        }

        private void RtEditor_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                // If the mouse moves outside the rectangle, start the drag.
                if (dragBoxFromMouseDown != Rectangle.Empty && !dragBoxFromMouseDown.Contains(e.X, e.Y))
                {
                    DataObject dati = new DataObject();
                    dati.SetData(DataFormats.Text, rtEditor.SelectedText);
                    dati.SetData(DataFormats.Rtf, rtEditor.SelectedRtf);
                    rtEditor.DoDragDrop(dati, DragDropEffects.Copy | DragDropEffects.Move);
                }
            }
        }

        private void RtEditor_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string[] parolaTestoDizionario = Principale.TestoDalDizionario(rtEditor, rtEditor.SelectionStart, rtEditor.Versione);
            if (!string.IsNullOrEmpty(parolaTestoDizionario[0]))
            {
                bool modificato = rtEditor.Modified;
                string rtf = rtEditor.Rtf;
                genitore.ApriNotaInEditor(parolaTestoDizionario[0], parolaTestoDizionario[2]);
                // per qualche motivo sconosciuto, la riga precedente cambia la dimensione della parola doppio clicata
                // alla dimensione del testo nel dizionario.
                // Succede durante una chiamata del messaggio EM_STREAMIN durante ImpostaRTF nella finestra che è creata con la nota.
                // Le righe seguenti reimpostano il testo come era.
                if (rtEditor.Rtf != rtf)
                {
                    rtEditor.Rtf = rtf;
                    rtEditor.Modified = modificato;
                }
            }
        }

        private void RtEditor_MouseHover(object sender, EventArgs e)
        {
            if (genitore.ActiveMdiChild == this && Settings.Default.DizionarioTooltip)
            {
                // non so perché, ma sembra funzionare meglio fare così, invece di fare rtEditor.GetCharIndexFromPosition(rtEditor.PointToClient(Cursor.Position))
                Point pointScreen = System.Windows.Forms.Cursor.Position;
                Point pointClient = rtEditor.PointToClient(pointScreen);
                int charPos = rtEditor.GetCharIndexFromPosition(pointClient);
                string[] parolaTestoDizionario = Principale.TestoDalDizionario(rtEditor, charPos, rtEditor.Versione);
                if (!string.IsNullOrEmpty(parolaTestoDizionario[0]))
                {
                    rtEditor.MostraHover(parolaTestoDizionario[1], parolaTestoDizionario[2], pointScreen, Settings.Default.OpzioniIpertestoTooltipInTooltip);
                }
            }
        }

        public void SaltoIpertestuale()
        {
            bool provaConDoppioClic = true;
            int p1 = rtEditor.Text.LastIndexOf(RichTextBoxEx.InizioLink, rtEditor.SelectionStart);
            int p2 = rtEditor.Text.IndexOf(RichTextBoxEx.FineLink2, rtEditor.SelectionStart);
            if (p1 >= 0 && p2 >= 0)
            {
                string ipertesto = rtEditor.Text.Substring(p1 + 1, p2 - p1 - 1);
                if (ipertesto.IndexOf(RichTextBoxEx.InizioLink) < 0) // altrimenti c'è SelectionStart è fra 2 link, ma non ne fa parte di uno
                {
                    string[] link = genitore.LinkCliccato(null, rtEditor.Versione, null, ipertesto, true);
                    if (!string.IsNullOrEmpty(link[0]))
                        provaConDoppioClic = false;
                }
            }
            if (provaConDoppioClic)
                RtEditor_MouseDoubleClick(this, null);
        }

        #region Drag eventi

        void RtEditor_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Rtf))
            {
                rtEditor.SelectionLength = 0;
                rtEditor.SelectedRtf = e.Data.GetData(DataFormats.Rtf).ToString();
            }
            else if (e.Data.GetDataPresent(DataFormats.Text))
            {
                rtEditor.SelectionLength = 0;
                rtEditor.SelectedText = e.Data.GetData(DataFormats.Text).ToString();
            }
        }

        void RtEditor_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text) || e.Data.GetDataPresent(DataFormats.Rtf))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        #endregion

        #endregion

        #region RTF Eventi

        private void RtEditor_KeyUp(object sender, KeyEventArgs e)
        {
            // a volte non funziona, perché i caratteri con ASCII<16 inseriti per inizio riferimento eccetera fanno sì che niente sia selezionato
            if (e.KeyCode == Keys.Home && e.Shift)
            {
                if (rtEditor.SelectionLength == 0 && rtEditor.Text.Length > 0)
                {
                    int ss = rtEditor.SelectionStart;
                    for (int i = 1; i <= 10; ++i)
                    {
                        rtEditor.Select(i, ss-i);
                        if (rtEditor.SelectionLength > 0 && i < ss)
                            break;
                    }
                }
            }
        }

        private void RtEditor_SelectionChanged(object sender, EventArgs e)
        {
            if (!inizializzazione)
                genitore.AggiornaPulsanti(rtEditor);
        }

        private void RtEditor_TextChanged(object sender, EventArgs e)
        {
            if (!inizializzazione)
                genitore.AggiornaPulsanti(rtEditor);
        }

        private void RtEditor_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            genitore.LinkCliccato(rtEditor.Versione, VersionePerIpertesto, e.LinkText);
        }

        private void LinkHover(object sender, LinkHoverEventArgs e)
        {
            genitore.LinkHover(VersionePerIpertesto, e);
        }

        #endregion

        #region Popup menu

        private void PmEditor_Opening(object sender, CancelEventArgs e)
        {
            bool testoSelezionato = (rtEditor.SelectionLength > 0);
            undoToolStripMenuItem.Enabled = rtEditor.CanUndo;
            cutToolStripMenuItem.Enabled = testoSelezionato;
            copyToolStripMenuItem.Enabled = testoSelezionato;
            deleteToolStripMenuItem.Enabled = testoSelezionato;
            makeLinkToolStripMenuItem.Enabled = testoSelezionato;

            // the rest of the code is also in Visualizza for its popup
            string riferimento = rtEditor.VersettoAttuale(posizioneUltimoClic);
            if (string.IsNullOrEmpty(riferimento))
                riferimento = RiferimentoDaNomeNota();
            bool riferimentoScelto = !string.IsNullOrEmpty(riferimento);
            popupToolStripSeparatorWordVerse.Visible = riferimentoScelto;
            informationOnVerseToolStripMenuItem.Visible = riferimentoScelto;
            browseToolStripMenuItem.Visible = riferimentoScelto;
            noteOnVerseToolStripMenuItem.Visible = riferimentoScelto && (noteOnVerseToolStripMenuItem.DropDownItems.Count > 0);
            
            if (riferimentoScelto)
            {
                bool menuVisibile = false, submenuVisibile;
                string commentario, riferimentoComeNota = "#" + riferimento + "0000-" + riferimento + "0000";
                Riferimento riferimentoComeRiferimento = Principale.testi.ConvertiRiferimento(Principale.testi.ConvertiTitoloNotaARiferimento(riferimentoComeNota));
                for (int i = 0; i < noteOnVerseToolStripMenuItem.DropDownItems.Count; ++i)
                {
                    commentario = noteOnVerseToolStripMenuItem.DropDownItems[i].Text;
                    submenuVisibile = (Principale.testi.Info(commentario).Bloccato == BloccatoTipi.Sbloccato || Principale.testi.ElencaNoteInBrano(riferimentoComeRiferimento, commentario).Count > 0);
                    noteOnVerseToolStripMenuItem.DropDownItems[i].Visible = submenuVisibile;
                    // bisogna usare submenuVisibile, perché anche se diciamo menu.Visibile=true, menu.Visibile rimane falso fino a quando il menu è aperto
                    if (submenuVisibile)
                        menuVisibile = true;
                }
                noteOnVerseToolStripMenuItem.Visible = menuVisibile;

                string riferimentoTestuale = Principale.testi.ConvertiTitoloNotaARiferimento(riferimentoComeNota);
                informationOnVerseToolStripMenuItem.Text = informationOnVerseToolStripMenuItem.Tag.ToString() + riferimentoTestuale;
                browseToolStripMenuItem.Text = browseToolStripMenuItem.Tag.ToString() + riferimentoTestuale;
                noteOnVerseToolStripMenuItem.Text = noteOnVerseToolStripMenuItem.Tag.ToString() + riferimentoTestuale;
            }

            string parola = GetParolaAttuale();
            bool parolaScelta = !string.IsNullOrEmpty(parola);
            popupToolStripSeparatorGeneralWord.Visible = parolaScelta;
            searchToolStripMenuItem.Visible = parolaScelta;
            informationOnWordToolStripMenuItem.Visible = parolaScelta;
            noteOnWordToolStripMenuItem.Visible = parolaScelta;
            if (parolaScelta)
            {
                searchWordToolStripMenuItem.Text = parola;
                string versioneDaRicercare = (string.IsNullOrEmpty(rtEditor.Versione) ? Principale.testi.UltimaBibbia : rtEditor.Versione);
                string radice = Principale.testi.RadiceDiParola(parola, versioneDaRicercare);
                if (radice == "*")
                    radice = "";
                searchRadiceToolStripMenuItem.Visible = !string.IsNullOrEmpty(radice);
                searchRadiceToolStripMenuItem.Text = radice;
                informationOnWordToolStripMenuItem.Text = informationOnWordToolStripMenuItem.Tag.ToString() + parola;
                string searchSelection = rtEditor.SelectedText;
                for (int i = searchSelection.Length - 1; i >= 0; --i)
                    if (!Funzioni.IsLettera(searchSelection[i]) && !char.IsWhiteSpace(searchSelection[i]))
                        searchSelection = searchSelection.Remove(i, 1);
                searchSelection = searchSelection.Trim();
                searchSelectionToolStripMenuItem.Text = searchSelection;
                searchSelectionToolStripMenuItem.Visible = !string.IsNullOrEmpty(searchSelection);
                noteOnWordToolStripMenuItem.DropDownItems.Clear();

                Collection<string> dizionari = Principale.testi.NomiVersioni(TestoTipi.Dizionario);
                foreach (string dizionario in dizionari)
                {
                    bool collezioneDaAggiungere = false;
                    if (!string.IsNullOrEmpty(Principale.testi.GetNotaTesto(parola, dizionario)))
                        collezioneDaAggiungere = true;
                    else
                    {
                        if (!string.IsNullOrEmpty(Principale.testi.GetNotaTesto(radice, dizionario)))
                            collezioneDaAggiungere = true;
                    }
                    if (collezioneDaAggiungere)
                        noteOnWordToolStripMenuItem.DropDownItems.Add(dizionario, null, NoteOnWordClick);
                }
                if (noteOnWordToolStripMenuItem.DropDownItems.Count == 0)
                    noteOnWordToolStripMenuItem.Visible = false;
                else
                    noteOnWordToolStripMenuItem.Text = noteOnWordToolStripMenuItem.Tag.ToString() + parola;
            }
        }

        private string GetParolaAttuale()
        {
            string parola = rtEditor.ParolaAttuale(posizioneUltimoClic);
            // a click before a verse reference picks up the hidden text
            if (parola.Length >= 8)
                if (char.IsDigit(parola[0]) && char.IsDigit(parola[1]) && char.IsDigit(parola[2]) && char.IsDigit(parola[3]) && char.IsDigit(parola[4]) && char.IsDigit(parola[5]) && char.IsDigit(parola[6]) && char.IsDigit(parola[7]))
                    parola = "";
            return parola;
        }

        private void NoteOnWordClick(object sender, EventArgs e)
        {
            string parola = GetParolaAttuale();
            string dizionario = ((ToolStripMenuItem)sender).Text;

            if (!string.IsNullOrEmpty(Principale.testi.GetNotaTesto(parola, dizionario)))
                genitore.ApriNotaInEditor(parola, dizionario);
            else
            {
                string versioneDaRicercare = (string.IsNullOrEmpty(rtEditor.Versione) ? Principale.testi.UltimaBibbia : rtEditor.Versione);
                string radice = Principale.testi.RadiceDiParola(parola, versioneDaRicercare);
                if (radice != "*" && !string.IsNullOrEmpty(radice))
                {
                    if (!string.IsNullOrEmpty(Principale.testi.GetNotaTesto(radice, dizionario)))
                        genitore.ApriNotaInEditor(radice, dizionario);
                }
            }
        }

        private void NoteOnVerseClick(object sender, EventArgs e)
        { // questo codice è anche in Visualizza::NoteOnVerseClick
            string riferimento = rtEditor.VersettoAttuale(posizioneUltimoClic) + "0000";
            string riferimentoComeNota = "#" + riferimento + "-" + riferimento;
            string commentario = ((ToolStripMenuItem)sender).Text;

            if (!string.IsNullOrEmpty(Principale.testi.GetNotaTesto(riferimento, commentario)))
                genitore.ApriNotaInEditor(riferimentoComeNota, commentario);
            else
            {
                Riferimento riferimentoComeRiferimento = Principale.testi.ConvertiRiferimento(Principale.testi.ConvertiTitoloNotaARiferimento(riferimentoComeNota));
                Riferimento noteCheContengonoVersetto = Principale.testi.ElencaNoteInBrano(riferimentoComeRiferimento, commentario);
                if (noteCheContengonoVersetto.Count > 0)
                {
                    genitore.ApriNotaInEditor(noteCheContengonoVersetto.Note[0], commentario);
                }
                else
                { // non c'è una nota che contiene questo versetto, quindi aprire una nota vuota
                    genitore.ApriNotaInEditor(riferimentoComeNota, commentario);
                }
            }
        }

        public string RiferimentoDaNomeNota()
        { // i primi 8 caratteri dopo #
            return (!string.IsNullOrEmpty(nomeFile) && nomeFile[0] == '#' && nomeFile.Length >= 9) ? nomeFile.Substring(1, 8) : "";
        }

        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtEditor.Undo();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtEditor.Cut();
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtEditor.CopiaSenzaTestoNascosto();
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtEditor.Paste();
        }

        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtEditor.SelectedText = "";
        }

        private void InformationOnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripItem tsi = (ToolStripItem)(sender);
            string frase = tsi.Text.Substring(tsi.Tag.ToString().Length);
            if (sender == informationOnWordToolStripMenuItem)
                if (frase.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }) >= 0 && frase.IndexOfAny(new char[] { '<', '>' }) == 0)
                    frase = "<" + frase + ">";
            genitore.ApriInformazione(frase);
        }

        private void BrowseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripItem tsi = (ToolStripItem)(sender);
            string versioneDaMostrare = (string.IsNullOrEmpty(rtEditor.Versione) ? Principale.testi.UltimaBibbia : rtEditor.Versione);
            string versetto = tsi.Text.Substring(tsi.Tag.ToString().Length);
            Visualizza formVisualizza = genitore.VisualizzaTesto(versioneDaMostrare, TestoTipi.Bibbia);
            formVisualizza.SpostaTesto(Principale.testi.ConvertiRiferimento(versetto), false);
        }

        private void SearchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string frase = ((ToolStripItem)(sender)).Text;
            if (sender == searchRadiceToolStripMenuItem)
                frase = "/" + frase;
            else if (sender == searchSelectionToolStripMenuItem)
                frase = "[" + frase + "]";
            else
                frase = "<" + frase + ">";
            genitore.RicercaInEditor(frase, "", (string.IsNullOrEmpty(rtEditor.Versione) ? Principale.testi.UltimaBibbia : rtEditor.Versione));
        }

        private void IpertestoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            genitore.CreaLink(rtEditor, ((ToolStripMenuItem)sender).Tag.ToString());
        }

        #endregion
    }
}