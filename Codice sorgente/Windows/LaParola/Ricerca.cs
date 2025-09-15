using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using LaParola.Properties;
using TestiBiblici;

namespace LaParola
{
    public partial class Ricerca : Template
    {
        private Principale genitore;
        private float cbFontSize;
        private string cbFontName;
        private Font font = null;

        public string EspressioneDaRicercare
        {
            get { return cbEspressione.Text; }
            set { cbEspressione.Text = value; }
        }

        public Ricerca(Principale formGenitore)
        {
            InitializeComponent();
            genitore = formGenitore;

            cbFontSize = cbVersione.Font.Size;
            cbFontName = cbVersione.Font.Name;
        }

        private void Ricerca_Load(object sender, EventArgs e)
        {
            this.ActiveControl = cbEspressione;
            string versionePrecedente = Settings.Default.RicercaVersione;

            guidaFile.HelpNamespace = genitore.NomeFileGuida();

            cbSalvaListaVersetti.Left = cbEspressione.Right - cbSalvaListaVersetti.Width;

            cbVersione.BeginUpdate();
            foreach (string s in Principale.testi.NomiVersioni())
            {
                cbVersione.Items.Add(s);
                if (s == versionePrecedente)
                    cbVersione.SelectedIndex = cbVersione.Items.Count - 1;
            }

            if (cbVersione.Items.Count > 0)
            {
                if (cbVersione.SelectedIndex < 0)
                    cbVersione.SelectedIndex = 0;
            }
            else
            {
                btnOK.Visible = false;
            }
            cbVersione.EndUpdate();

            cbEspressione.Items.AddRange(Settings.Default.RicercaRicerchePrecedenti.Split(new char[] { '§' }, StringSplitOptions.RemoveEmptyEntries));

            cbParte.SelectedIndex = 0;
            CreaListaListaVersetti();
            cbBrano.Items.AddRange(Settings.Default.RicercaBraniPrecedenti.Split(new char[] { '§' }, StringSplitOptions.RemoveEmptyEntries));
        }

        private void RinnovaTutteListeDiSegnalibri()
        {
            Cursor cursoreAttuale = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                genitore.CreaMenuSegnalibri();
                CreaListaListaVersetti();
                foreach (Form f in genitore.MdiChildren)
                {
                    if (f.Tag != null && f.Tag.ToString() == "Segnalibri")
                        ((Segnalibri)f).AggiornaListaSegnalibri();
                }
            }
            finally
            {
                Cursor.Current = cursoreAttuale;
                if (cursoreAttuale != null)
                    cursoreAttuale.Dispose();
            }
        }

        public void CreaListaListaVersetti()
        {
            string listaSelezionata = (cbListaVersetti.SelectedItem != null ? cbListaVersetti.SelectedItem.ToString() : "");
            int indiceDaSelezionare = 0;
            cbListaVersetti.BeginUpdate();
            cbListaVersetti.Items.Clear();
            for (int i = 4; i < genitore.bookmarksToolStripMenuItem.DropDown.Items.Count - 2; ++i)
            { // non includiamo i segnalibri veloci, dei capitoli, né il separatore né la voce della finestra dei segnalibri
                cbListaVersetti.Items.Add(genitore.bookmarksToolStripMenuItem.DropDown.Items[i].Text);
                if (genitore.bookmarksToolStripMenuItem.DropDown.Items[i].Text == listaSelezionata)
                    indiceDaSelezionare = cbListaVersetti.Items.Count - 1;
            }
            if (cbListaVersetti.Items.Count == 0)
                rbListaVersetti.Enabled = false;
            else
                cbListaVersetti.SelectedIndex = indiceDaSelezionare;
            cbListaVersetti.EndUpdate();
        }

        private void Ricerca_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ((e.CloseReason != CloseReason.WindowsShutDown) && (e.CloseReason != CloseReason.TaskManagerClosing))
            {
                Settings.Default.RicercaVersione = cbVersione.SelectedItem.ToString();

                int nRicercheDaSalvare = Settings.Default.MiscRichiesteMemorizzate;
                if (cbEspressione.Items.Count < nRicercheDaSalvare)
                    nRicercheDaSalvare = cbEspressione.Items.Count;
                StringBuilder ricercheDaSalvare = new StringBuilder("");
                for (int i = 0; i < nRicercheDaSalvare; ++i)
                    ricercheDaSalvare.Append("§").Append(cbEspressione.Items[i]);
                Settings.Default.RicercaRicerchePrecedenti = ricercheDaSalvare.ToString();

                int nBraniDaSalvare = Settings.Default.MiscRichiesteMemorizzate;
                if (cbBrano.Items.Count < nBraniDaSalvare)
                    nBraniDaSalvare = cbBrano.Items.Count;
                StringBuilder braniDaSalvare = new StringBuilder("");
                for (int i = 0; i < nBraniDaSalvare; ++i)
                    braniDaSalvare.Append("§").Append(cbBrano.Items[i]);
                Settings.Default.RicercaBraniPrecedenti = braniDaSalvare.ToString();
            }

            font.Dispose();
            font = null;
        }

        private void cbEspressione_TextChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = (!String.IsNullOrEmpty(cbEspressione.Text));
        }

        private void cbVersione_SelectedIndexChanged(object sender, EventArgs e)
        {
            string nomeVersione = cbVersione.SelectedItem.ToString();

            TestoTipi tipo = Principale.testi.Info(nomeVersione).Tipo;
            gbBrano.Visible = (((tipo & TestoTipi.Bibbia) == TestoTipi.Bibbia) || ((tipo & TestoTipi.Commentario) == TestoTipi.Commentario));
            cbSalvaListaVersetti.Visible = gbBrano.Visible;

            switch (Funzioni.LinguaPrincipale(Principale.testi.Info(nomeVersione).Lingua))
            {
                case "he":
                case "he-t":
                    try
                    {
                        font = new Font(Principale.testi.Formato.FontEbraicoNome, Principale.testi.Formato.FontEbraicoDimensione * cbFontSize / Principale.testi.Formato.FontDimensione);
                    }
                    catch (ArgumentException)
                    {
                        //
                    }
                    break;
                case "el":
                    try
                    {
                        font = new Font(Principale.testi.Formato.FontGrecoNome, Principale.testi.Formato.FontGrecoDimensione * cbFontSize / Principale.testi.Formato.FontDimensione);
                    }
                    catch (ArgumentException)
                    {
                        //
                    }
                    break;
                default:
                    try
                    {
                        font = new Font(cbFontName, cbFontSize);
                    }
                    catch (ArgumentException)
                    {
                        //
                    }
                    break;
            }
            try
            {
                cbEspressione.Font = font;
            }
            catch (ArgumentException)
            {
                //
            }
        }

        private void btnScegliParola_Click(object sender, EventArgs e)
        {
            Cursor cursoreAttuale = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                ScegliParola scegliParola = new ScegliParola(genitore, cbVersione.SelectedItem.ToString())
                {
                    MdiParent = this.MdiParent
                };
                scegliParola.Show();
            }
            finally
            {
                Cursor.Current = cursoreAttuale;
                if (cursoreAttuale != null)
                    cursoreAttuale.Dispose();
            }
        }

        private void rb_CheckedChanged(object sender, EventArgs e)
        {
            cbParte.Enabled = rbParte.Checked;
            cbListaVersetti.Enabled = rbListaVersetti.Checked;
            cbBrano.Enabled = rbBrano.Checked;
        }

        private void pulListaVersettiNuova_Click(object sender, EventArgs e)
        {
            string nomeLista = "";
            using (InputBox inputBox = new InputBox(Principale.LocRM.GetString("SearchNewVerseListCaption"), Principale.LocRM.GetString("SearchNewVerseListQuestion1"), nomeLista))
            {
                inputBox.ShowDialog();
                nomeLista = inputBox.Risposta;
            }
            if (string.IsNullOrEmpty(nomeLista))
                return;

            string riferimentoLista = "";
            using (InputBox inputBox = new InputBox(Principale.LocRM.GetString("SearchNewVerseListCaption"), Principale.LocRM.GetString("SearchNewVerseListQuestion2"), riferimentoLista))
            {
                inputBox.ShowDialog();
                riferimentoLista = inputBox.Risposta;
            }

            Riferimento riferimento = Principale.testi.ConvertiRiferimento(riferimentoLista);
            if (riferimento.Count > 0)
            {
                string nomeFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + "Segnalibri" + Path.DirectorySeparatorChar + nomeLista;
                if (File.Exists(nomeFile + ".xml"))
                {
                    int count = 1;
                    while (File.Exists(nomeFile + count.ToString(CultureInfo.InvariantCulture) + ".xml"))
                        ++count;
                    nomeFile += count.ToString(CultureInfo.InvariantCulture);
                }
                nomeFile += ".xml";

                string testoFile = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>";
                testoFile += Environment.NewLine + "<bookmarks>";
                testoFile += Environment.NewLine + "<name>" + nomeLista + "</name>";
                testoFile += Environment.NewLine + "<description></description>";
                foreach (byte[] brano in riferimento.Brani)
                {
                    testoFile += Environment.NewLine + "<bookmark>";
                    testoFile += Environment.NewLine + "<name>" + Principale.testi.NormalizzaRiferimento(new Riferimento(brano)) + "</name>";
                    testoFile += Environment.NewLine + "<reference>" + brano[0] + " " + brano[1] + " " + brano[2] + " " + brano[3] + " " + brano[4] + " " + brano[5] + "</reference>";
                    testoFile += Environment.NewLine + "</bookmark>";
                }
                testoFile += Environment.NewLine + "</bookmarks>";
                File.WriteAllText(nomeFile, testoFile);
                RinnovaTutteListeDiSegnalibri();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string versione = cbVersione.SelectedItem.ToString();
            string espressione = cbEspressione.Text;
            string branoDaRicercare = "";

            if (rbParte.Checked)
            {
                switch (cbParte.SelectedIndex)
                {
                    case 0: // tutta la Bibbia
                        branoDaRicercare = "";
                        break;
                    case 1: // AT
                        branoDaRicercare = Principale.testi.GetLibroNome(1) + "-" + Principale.testi.GetLibroNome(46);
                        break;
                    case 2: // NT
                        branoDaRicercare = Principale.testi.GetLibroNome(47) + "-" + Principale.testi.GetLibroNome(73);
                        break;
                    case 3: // Pentateuco
                        branoDaRicercare = Principale.testi.GetLibroNome(1) + "-" + Principale.testi.GetLibroNome(5);
                        break;
                    case 4: // storici
                        branoDaRicercare = Principale.testi.GetLibroNome(1) + "-" + Principale.testi.GetLibroNome(21);
                        break;
                    case 5: // scritti
                        branoDaRicercare = Principale.testi.GetLibroNome(22) + "-" + Principale.testi.GetLibroNome(28);
                        break;
                    case 6: // profeti
                        branoDaRicercare = Principale.testi.GetLibroNome(29) + "-" + Principale.testi.GetLibroNome(46);
                        break;
                    case 7: // Vangeli
                        branoDaRicercare = Principale.testi.GetLibroNome(47) + "-" + Principale.testi.GetLibroNome(50);
                        break;
                    case 8: // Vangeli e Atti
                        branoDaRicercare = Principale.testi.GetLibroNome(47) + "-" + Principale.testi.GetLibroNome(51);
                        break;
                    case 9: // lettere
                        branoDaRicercare = Principale.testi.GetLibroNome(52) + "-" + Principale.testi.GetLibroNome(73);
                        break;
                    case 10: // lettere di Paolo
                        branoDaRicercare = Principale.testi.GetLibroNome(52) + "-" + Principale.testi.GetLibroNome(64);
                        break;
                    case 11: // lettere di altri
                        branoDaRicercare = Principale.testi.GetLibroNome(65) + "-" + Principale.testi.GetLibroNome(73);
                        break;
                }
            }
            else if (rbListaVersetti.Checked)
            {
                for (int i = 1; i < genitore.bookmarksToolStripMenuItem.DropDown.Items.Count - 2; ++i)
                {
                    if (genitore.bookmarksToolStripMenuItem.DropDown.Items[i].Text == cbListaVersetti.SelectedItem.ToString())
                    {
                        foreach (ToolStripMenuItem voce in ((ToolStripMenuItem)genitore.bookmarksToolStripMenuItem.DropDown.Items[i]).DropDown.Items)
                        {
                            branoDaRicercare += Principale.testi.NormalizzaRiferimentoSegnalibro(voce.Tag.ToString());
                        }
                        break;
                    }
                }
            }
            else
                branoDaRicercare = cbBrano.Text;
            Riferimento versettiConFrase = genitore.RicercaInEditor(espressione, branoDaRicercare, versione);

            if (cbSalvaListaVersetti.Visible && cbSalvaListaVersetti.Checked && versettiConFrase.Count > 0)
            {
                string nomeFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + "Segnalibri" + Path.DirectorySeparatorChar + Funzioni.RimuoviCaratteriNonValidiInPercorsi(espressione);
                if (File.Exists(nomeFile + ".xml"))
                {
                    int count = 1;
                    while (File.Exists(nomeFile + count.ToString(CultureInfo.InvariantCulture) + ".xml"))
                        ++count;
                    nomeFile += count.ToString(CultureInfo.InvariantCulture);
                }
                nomeFile += ".xml";

                string testoFile = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>";
                testoFile += Environment.NewLine + "<bookmarks>";
                testoFile += Environment.NewLine + "<name>" + Funzioni.RimuoviCaratteriNonValidiInXml(espressione) + "</name>";
                testoFile += Environment.NewLine + "<description>" + Principale.LocRM.GetString("SearchBookmarksDescription1") + " " + Funzioni.RimuoviCaratteriNonValidiInXml(espressione) + (string.IsNullOrEmpty(branoDaRicercare) ? "" : " " + Principale.LocRM.GetString("SearchBookmarksDescription2") + " " + branoDaRicercare) + "</description>";
                foreach (byte[] brano in Principale.testi.ConvertiAStandard(versettiConFrase, versione).Brani)
                {
                    testoFile += Environment.NewLine + "<bookmark>";
                    testoFile += Environment.NewLine + "<name>" + Principale.testi.NormalizzaRiferimento(brano[0], brano[1], brano[2]) + "</name>";
                    testoFile += Environment.NewLine + "<reference>" + brano[0] + " " + brano[1] + " " + brano[2] + "</reference>";
                    testoFile += Environment.NewLine + "</bookmark>";
                }
                testoFile += Environment.NewLine + "</bookmarks>";
                File.WriteAllText(nomeFile, testoFile);
                RinnovaTutteListeDiSegnalibri();
            }

            if (cbEspressione.Items.IndexOf(espressione) > -1)
                cbEspressione.Items.Remove(espressione);
            cbEspressione.Items.Insert(0, espressione);
            //            cbEspressione.Text = "";
            //            btnOK.Enabled = false;
            cbEspressione.AutoCompleteMode = AutoCompleteMode.None;
            // per costringere il combo box a rileggere i valori che possono essere usati per l'autocomplete
            cbEspressione.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cbEspressione.Text = espressione;
            cbEspressione.SelectAll();

            if (rbBrano.Checked && !string.IsNullOrEmpty(cbBrano.Text))
            {
                string branoRicercato = cbBrano.Text;
                if (cbBrano.Items.IndexOf(branoRicercato) > -1)
                    cbBrano.Items.Remove(branoRicercato);
                cbBrano.Items.Insert(0, branoRicercato);
                cbBrano.AutoCompleteMode = AutoCompleteMode.None;
                // per costringere il combo box a rileggere i valori che possono essere usati per l'autocomplete
                cbBrano.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cbBrano.Text = branoRicercato;
                cbBrano.SelectAll();
            }
        }

        private void btnCanc_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void SetEspressioneInizioSelezione(int inizio)
        {
            cbEspressione.SelectionStart = inizio;
        }

    }
}