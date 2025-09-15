using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using LaParola.Properties;
using TestiBiblici;

namespace LaParola
{
    public partial class Lettura : Template
    {
        #region proprietà

        private Principale genitore;
        private LaParola.InfoLettura schemaAttuale;
        private List<string> letture = new List<string>(366);
        private string letturaAttuale = "";
        private List<RichTextBoxHighlight> listaRtb = new List<RichTextBoxHighlight>(4);
        private bool inizializzazione = true;

        private const int LARGHEZZA_MINIMA_RTF = 100;
        private const int LARGHEZZA_SPLITTER = 1;

        private RichTextBoxHighlight ultimaRtb = null;
        public RichTextBoxHighlight UltimaRtb
        {
            get { return ultimaRtb; }
        }

        public string Piano
        {
            get
            {
                try
                {
                    return cbSchemi.SelectedItem.ToString();
                }
                catch
                { // può succedere se non ci sono piani installati
                    return "";
                }
            }
            set
            {
                foreach (object piano in cbSchemi.Items)
                    if (piano.ToString() == value)
                    {
                        cbSchemi.SelectedItem = piano;
                        break;
                    }
            }
        }

        #endregion

        #region costruttori

        public Lettura(Principale formGenitore, string piano)
        {
            Costruttore(formGenitore, piano);
        }

        public Lettura(Principale formGenitore)
        {
            Costruttore(formGenitore, "");
        }

        private void Costruttore(Principale formGenitore, string piano)
        {
            InitializeComponent();
            genitore = formGenitore;
            guidaFile.HelpNamespace = genitore.NomeFileGuida();

            Collection<string> versioni = Principale.testi.NomiVersioni(TestoTipi.Bibbia);
            string versionePredefinita = (string.IsNullOrEmpty(Settings.Default.LetturaVersione) ? Principale.testi.UltimaBibbiaCompleta : Settings.Default.LetturaVersione);
            foreach (string versione in versioni)
            {
                cbVersioni.Items.Add(versione);
                if (versione == versionePredefinita)
                    cbVersioni.SelectedIndex = cbVersioni.Items.Count - 1;
            }
            if (cbVersioni.SelectedIndex == -1 && cbVersioni.Items.Count > 0)
                cbVersioni.SelectedIndex = 0;

            //            DateTime dt = DateTime.Today;

            string pianoDaProvare = piano;
            if (string.IsNullOrEmpty(piano))
                pianoDaProvare = Settings.Default.LetturaUltima;
            if (string.IsNullOrEmpty(pianoDaProvare))
            { // imposta lo schema predefinito
                foreach (LaParola.InfoLettura schema in genitore.schemiLettura)
                {
                    if (schema.nomeFile.EndsWith("All the Bible in a Year, 4 Readings per Day.xml", StringComparison.OrdinalIgnoreCase))
                    {
                        pianoDaProvare = schema.nome;
                        break;
                    }
                }
            }

            cbSchemi.BeginUpdate();
            foreach (LaParola.InfoLettura schema in genitore.schemiLettura)
            {
                cbSchemi.Items.Add(schema.nome);
                if (schema.nome == pianoDaProvare)
                {
                    schemaAttuale = schema;
                    cbSchemi.SelectedIndex = cbSchemi.Items.Count - 1;
                }
            }
            if (cbSchemi.SelectedIndex == -1 && cbSchemi.Items.Count > 0)
            {
                schemaAttuale = genitore.schemiLettura[0];
                cbSchemi.SelectedIndex = 0;
            }
            cbSchemi.EndUpdate();

            if (cbSchemi.SelectedIndex >= 0)
                Settings.Default.LetturaUltima = cbSchemi.SelectedItem.ToString();

            if (cbSchemi.Items.Count == 0)
                MessageBox.Show(Principale.LocRM.GetString("ReadingsErrorNoPlan"), Principale.LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions);

            inizializzazione = false;
        }

        private void Lettura_Load(object sender, EventArgs e)
        {
            // bisogna avere 2 impostazioni per la Location, invece di essere un Point, perché altrimenti Mono ha problemi
            // anche per Size
            if (Settings.Default.ReadingsLocationY > -9990)
            {
                Location = new Point(Settings.Default.ReadingsLocationX, Settings.Default.ReadingsLocationY);
                if (Settings.Default.ReadingsSizeY > 0)
                {
                    Size = new Size(Settings.Default.ReadingsSizeX, Settings.Default.ReadingsSizeY);
                    Lettura_Resize(sender, e);
                    //                    MostraLetture();
                }
            }
        }

        #endregion

        private void Lettura_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.ReadingsSizeX = Size.Width;
            Settings.Default.ReadingsSizeY = Size.Height;
            Settings.Default.ReadingsLocationX = Location.X;
            Settings.Default.ReadingsLocationY = Location.Y;
        }

        private void Lettura_Resize(object sender, EventArgs e)
        {
            panLetture.Size = new Size(Width - 16, Height - 106);
            int nRtf = listaRtb.Count;
            for (int i = 1; i < nRtf; ++i)
            {
                // non aggiustiamo listaRtb[0], perché ha DockStyle.Fill, e prende il resto dello spazio
                listaRtb[i].BloccaRtf(true);
                listaRtb[i].Width = (panLetture.Width - (nRtf - 1) * LARGHEZZA_SPLITTER) / nRtf;
                listaRtb[i].BloccaRtf(false);
            }
        }

        private void MostraLetture()
        {
            if (letture.Count > 0)
            {
                int numeroLettura = (dtCalendario.Value.DayOfYear - 1 - Settings.Default.LetturaGiorniOffset) % letture.Count;
                if (numeroLettura < 0)
                    numeroLettura += letture.Count;
                letturaAttuale = letture[numeroLettura];
                panLetture.Controls.Clear();
                foreach (RichTextBoxHighlight rtb in listaRtb)
                {
                    rtb.Visible = false;
                    rtb.Dispose();
                }
                listaRtb.Clear();
                string[] letturaAttualeArray = letturaAttuale.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                int minLarghezza = letturaAttualeArray.Length * LARGHEZZA_MINIMA_RTF + (letturaAttualeArray.Length - 1) + LARGHEZZA_SPLITTER + 6;
                if (minLarghezza < 493)
                    minLarghezza = 493; // la dimensione minima per mettere tutti i controlli
                MinimumSize = new Size(minLarghezza, MinimumSize.Height);
                string nomeVersione = cbVersioni.SelectedItem.ToString();

                for (int i = 0; i < letturaAttualeArray.Length; ++i)
                {
                    if (i != 0)
                    {
                        Splitter splitter = new Splitter
                        {
                            MinExtra = LARGHEZZA_MINIMA_RTF,
                            MinSize = LARGHEZZA_MINIMA_RTF,
                            Width = LARGHEZZA_SPLITTER,
                            Parent = panLetture,
                            Dock = DockStyle.Right
                        };
                    }
                    RichTextBoxHighlight rtb = new RichTextBoxHighlight
                    {
                        /*                    rtb.Size = new Size(panLetture.Width / letturaAttualeArray.Length, panLetture.Height);
                                            rtb.Location = new Point(i * panLetture.Width / letturaAttualeArray.Length, 0);*/
                        Width = (panLetture.Width - (letturaAttualeArray.Length - 1) * LARGHEZZA_SPLITTER) / letturaAttualeArray.Length,
                        Parent = panLetture,
                        Dock = (i == 0 ? DockStyle.Fill : DockStyle.Right),
                        ReadOnly = true
                    };
                    rtb.SelectionChanged += new EventHandler(rtb_SelectionChanged);
                    rtb.HighlightChangedEvent += new EventHandler<RichTextBoxHighlight.HighlightChangedEventArgs>(rtb_HighlightChangedEvent);
                    rtb.Versione = nomeVersione;
//                    rtb.AggiungiHighlightDaFile(); non funziona
                    listaRtb.Add(rtb);
                }
                Aggiorna();
            }
        }

        public void Aggiorna()
        {
            if (cbVersioni.SelectedIndex > -1)
            {
                string[] letturaAttualeArray = letturaAttuale.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                string nomeVersione = cbVersioni.SelectedItem.ToString();
                //int posizioneCapitolo;
                for (int i = 0; i < letturaAttualeArray.Length; ++i)
                {
                    listaRtb[i].BloccaRtf(true);
                    listaRtb[i].Rtf = Principale.testi.TestoBrano(Principale.testi.ConvertiDaStandard(Principale.testi.ConvertiRiferimento(Principale.testi.ConvertiRiferimentoDa3Numeri(letturaAttualeArray[i])), nomeVersione), nomeVersione);
                    // non funziona, perché non è sempre visualizzato l'inizio del capitolo
                    /*
                    foreach (Highlight highlight in listaRtb[i].highlightAttuale)
                    {
                        posizioneCapitolo = listaRtb[i].Text.IndexOf(RichTextBoxEx.InizioRiferimento + funzioni.AggiungiZero(highlight.libro, 2) + funzioni.AggiungiZero(highlight.capitolo, 3) + "001", StringComparison.Ordinal);
                        if (posizioneCapitolo >= 0)
                        {
                            // se l'inizio non è stato trovato, non mostriamo l'evidenziatore
                            // può essere un errore, ma più probabilmente il testo da evidenziare non è nei 5 capitoli attualmente visualizzati
                            listaRtb[i].MettiHighlight(highlight, posizioneCapitolo);
                        }
                    }
                     */
                    listaRtb[i].SelectionStart = 0;
                    listaRtb[i].SelectionLength = 0;
                    listaRtb[i].SelectionStart = 0;
                    listaRtb[i].ScrollToCaret();

                    listaRtb[i].BloccaRtf(false);
                }
            }
        }

        private void cbSchemi_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Default.LetturaUltima = cbSchemi.SelectedItem.ToString();

            letture.Clear();
            try
            {
                schemaAttuale = genitore.schemiLettura[cbSchemi.SelectedIndex];
                XmlDocument xd = new XmlDocument();
                xd.Load(schemaAttuale.nomeFile);
                XmlNode nodoPrincipale = xd.SelectSingleNode("readings");
                XmlNodeList nodiLetture = nodoPrincipale.SelectNodes("reading");
                StringBuilder testoDelGiorno = new StringBuilder("");
                foreach (XmlNode nodo in nodiLetture)
                {
                    try
                    {
                        XmlNodeList testi = nodo.SelectNodes("text");
                        testoDelGiorno.Remove(0, testoDelGiorno.Length);
                        foreach (XmlNode testo in testi)
                        {
                            testoDelGiorno.Append(testo.InnerText).Append("|");
                        }
                        letture.Add(testoDelGiorno.ToString());
                    }
                    catch // se c'è un problema con una lettura, aggiungi niente
                    {
                        letture.Add("");
                    }
                }
            }
            catch
            {
                // errore nell'XML, lasciamo le letture vuote
            }

            MostraLetture();
        }

        private void dtCalendario_ValueChanged(object sender, EventArgs e)
        {
            MostraLetture();
        }

        private void pulNuovoInizio_Click(object sender, EventArgs e)
        {
            Settings.Default.LetturaGiorniOffset = dtCalendario.Value.DayOfYear - 1;
            MostraLetture();
        }

        private void cbVersioni_SelectedIndexChanged(object sender, EventArgs e)
        {
            string nomeVersione = cbVersioni.SelectedItem.ToString();
            Settings.Default.LetturaVersione = nomeVersione;
            foreach (RichTextBoxHighlight rtb in listaRtb)
            {
                rtb.Versione = nomeVersione;
                rtb.AggiungiHighlightDaFile();
            }
            if (!inizializzazione)
                Aggiorna();
        }

        private void pulStampa_Click(object sender, EventArgs e)
        {
            genitore.StampaRichText(TuttoIlTesto());
        }

        private void pulCopia_Click(object sender, EventArgs e)
        {
            RichTextBoxEx rtb = TuttoIlTesto();
            rtb.SelectAll();
            rtb.CopiaSenzaTestoNascosto();
        }

        private void pulContesto_Click(object sender, EventArgs e)
        {
            // TODO guida
            // panes, versione, sposta testo
            string nomeVersione = cbVersioni.SelectedItem.ToString();
            string[] letturaAttualeArray = letturaAttuale.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            Visualizza formVisualizza = genitore.VisualizzaTesto(nomeVersione, TestoTipi.Bibbia);
            formVisualizza.SpostaTesto(Principale.testi.ConvertiDaStandard(Principale.testi.ConvertiRiferimento(Principale.testi.ConvertiRiferimentoDa3Numeri(letturaAttualeArray[0])), nomeVersione), false);

            for (int i = 1; i < letturaAttualeArray.Length; ++i)
            {
                LaParola.Visualizza.Pane p = formVisualizza.AggiungiPane(nomeVersione, TestoTipi.Bibbia);
                if (p!=null)
                    p.SpostaTesto(Principale.testi.ConvertiDaStandard(Principale.testi.ConvertiRiferimento(Principale.testi.ConvertiRiferimentoDa3Numeri(letturaAttualeArray[i])), nomeVersione), false);
            }
        }

        private RichTextBoxEx TuttoIlTesto()
        {
            RichTextBoxEx rtb = new RichTextBoxEx
            {
                Visible = false,
                Parent = this
            };
            string nomeVersione = cbVersioni.SelectedItem.ToString();
            rtb.Rtf = Principale.testi.TestoBrano(Principale.testi.ConvertiDaStandard(Principale.testi.ConvertiRiferimento(Principale.testi.ConvertiRiferimentoDa3Numeri(letturaAttuale.Replace('|', ';'))), nomeVersione), nomeVersione);
            return rtb;
        }

        private void btnCanc_Click(object sender, EventArgs e)
        {
            Close();
        }

        void rtb_SelectionChanged(object sender, EventArgs e)
        {
            ultimaRtb = ((RichTextBoxHighlight)sender);
            genitore.AggiornaPulsanti(ultimaRtb);
        }

        void rtb_HighlightChangedEvent(object sender, RichTextBoxHighlight.HighlightChangedEventArgs e)
        {
            genitore.HighlightChangedEvent(e);
        }

        #region highlight

        internal void HighlighterClick(Color colore, TipoHighlight tipo)
        {
            ultimaRtb.HighlighterClick(colore, tipo);
        }

        internal void HighlighterClick(byte tipoSottolineatura)
        {
            ultimaRtb.HighlighterClick(tipoSottolineatura);
        }

        internal void HighlighterNoneClick()
        {
            ultimaRtb.HighlighterNoneClick();
        }

        internal void HighlighterNoneNotMonoClick()
        {
            ultimaRtb.HighlighterNoneNotMonoClick();
        }

        internal void AggiornaHighlight(string versione)
        {
            if (cbVersioni.SelectedItem.ToString() == versione)
            {
                foreach (RichTextBoxHighlight rtb in listaRtb)
                {
                    rtb.AggiungiHighlightDaFile();
                }
                Aggiorna();
            }
        }



        /*
        internal void ImpostaHighlight(TipoHighlight tipoHighlight, Color colore)
        {
            ultimaRtb.ImpostaHighlight(tipoHighlight, colore);
        }

        internal void ImpostaHighlightSottolineatura(byte tipoSottolineatura)
        {
            ultimaRtb.ImpostaHighlightSottolineatura(tipoSottolineatura);
        }

        internal void ImpostaHighlightNone()
        {
            ultimaRtb.ImpostaHighlightNone();
        }
        */

        #endregion

    }
}