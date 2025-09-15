using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using TestiBiblici;

namespace LaParola
{
    public partial class BraniParalleli : Template
    {
        #region Pane

        internal class Pane : IDisposable
        {
            #region Proprietà

            private Panel panComponenti;
            private Label etichetta;
            private RichTextBoxHighlight rtfTesto;
            private Button pulsanteSu;
            private Button pulsanteGiu;

            private BraniParalleli genitore;

            public RichTextBoxHighlight ControlloRichText
            {
                get { return rtfTesto; }
            }

            public int Width
            {
                get { return panComponenti.Width; }
                set { panComponenti.Width = value; }
            }

            #endregion

            #region Costruttori

            public Pane(BraniParalleli sender, int sinistra, int larghezza, string nome)
            {
                genitore = sender;

                panComponenti = new Panel();
                rtfTesto = new RichTextBoxHighlight();
                etichetta = new Label();
                pulsanteSu = new Button();
                pulsanteGiu = new Button();

                panComponenti.Location = new Point(sinistra, 0);
                panComponenti.Size = new Size(larghezza, genitore.PanelHeight);
                panComponenti.Tag = this;
                panComponenti.Resize += new EventHandler(panComponenti_Resize);

                etichetta.Location = new Point(0, 0);
                etichetta.Text = nome;
                etichetta.AutoSize = true;

                pulsanteSu.Anchor = (AnchorStyles.Right | AnchorStyles.Top);
                pulsanteSu.Location = new Point(larghezza - 34, 0);
                pulsanteSu.Size = new Size(17, 17);
                pulsanteSu.Image = LaParola.Properties.Resources.arrow_u_small;
                pulsanteSu.Tag = 0;
                pulsanteSu.Click += new EventHandler(pulsanteSu_Click);
                pulsanteSu.Enter += new EventHandler(controllo_Enter);

                pulsanteGiu.Anchor = (AnchorStyles.Right | AnchorStyles.Top);
                pulsanteGiu.Location = new Point(larghezza - 17, 0);
                pulsanteGiu.Size = new Size(17, 17);
                pulsanteGiu.Image = LaParola.Properties.Resources.arrow_d_small;
                pulsanteGiu.Tag = 1;
                pulsanteGiu.Click += new EventHandler(pulsanteGiu_Click);
                pulsanteGiu.Enter += new EventHandler(controllo_Enter);

                rtfTesto.Location = new Point(0, 17);
                rtfTesto.ReadOnly = true;
                rtfTesto.TabIndex = 2;
                rtfTesto.SelectionChanged += new EventHandler(rtfTesto_SelectionChanged);
                rtfTesto.HighlightChangedEvent += new EventHandler<RichTextBoxHighlight.HighlightChangedEventArgs>(rtfTesto_HighlightChangedEvent);
                rtfTesto.Enter += new EventHandler(controllo_Enter);

                panComponenti.Controls.Add(rtfTesto);
                panComponenti.Controls.Add(etichetta);
                panComponenti.Controls.Add(pulsanteSu);
                panComponenti.Controls.Add(pulsanteGiu);
                etichetta.SendToBack();

                RidimensionaComponenti();

                sender.panPanes.Controls.Add(panComponenti);
                panComponenti.Dock = (sinistra == 0 ? DockStyle.Fill : DockStyle.Right);
            }

            #endregion

            #region eventi

            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (etichetta != null)
                        etichetta.Dispose();
                    if (rtfTesto != null)
                        rtfTesto.Dispose();
                    if (pulsanteSu != null)
                        pulsanteSu.Dispose();
                    if (pulsanteGiu != null)
                        pulsanteGiu.Dispose();
                    if (panComponenti != null)
                        panComponenti.Dispose();
                }
                // free native resources
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            void pulsanteSu_Click(object sender, EventArgs e)
            {
                genitore.pulsanteSuGiu_Click(this, 1);
            }

            void pulsanteGiu_Click(object sender, EventArgs e)
            {
                genitore.pulsanteSuGiu_Click(this, -1);
            }

            private void panComponenti_Resize(object sender, EventArgs e)
            {
                rtfTesto.Size = new Size(panComponenti.Width, panComponenti.Height - 17);
            }

            private void rtfTesto_SelectionChanged(object sender, EventArgs e)
            {
                genitore.paneAttivo = this;
                genitore.genitore.AggiornaPulsanti(this.ControlloRichText);
            }

            void rtfTesto_HighlightChangedEvent(object sender, RichTextBoxHighlight.HighlightChangedEventArgs e)
            {
                genitore.genitore.HighlightChangedEvent(e);
            }

            private void controllo_Enter(object sender, EventArgs e)
            {
                rtfTesto_SelectionChanged(sender, e);
            }

            #endregion

            #region metodi

            internal void RidimensionaComponenti()
            {
                panComponenti_Resize(null, null);
            }

            internal void BloccaRtf(bool blocca)
            {
                rtfTesto.BloccaRtf(blocca);
            }

            internal void SetTesto(string testo)
            {
                BloccaRtf(true);
                try
                {
                    rtfTesto.Rtf = testo;
                }
                catch
                {
                    rtfTesto.Text = testo;
                }

                // non funziona, perché non sempre dall'inizio del capitolo
                /*
                int posizioneCapitolo;
                foreach (Highlight highlight in rtfTesto.highlightAttuale)
                {
                    posizioneCapitolo = rtfTesto.Text.IndexOf(RichTextBoxEx.InizioRiferimento + funzioni.AggiungiZero(highlight.libro, 2) + funzioni.AggiungiZero(highlight.capitolo, 3) + "001", StringComparison.Ordinal);
                    if (posizioneCapitolo >= 0)
                    {
                        // se l'inizio non è stato trovato, non mostriamo l'evidenziatore
                        // può essere un errore, ma più probabilmente il testo da evidenziare non è nei 5 capitoli attualmente visualizzati
                        rtfTesto.MettiHighlight(highlight, posizioneCapitolo);
                    }
                }
                 * */
                rtfTesto.SelectionStart = 0;
                rtfTesto.SelectionLength = 0;
                rtfTesto.SelectionStart = 0;
                rtfTesto.ScrollToCaret();

                BloccaRtf(false);
            }

            internal void SetPulsanti(bool testoEsiste)
            {
                pulsanteGiu.Enabled = testoEsiste;
                pulsanteSu.Enabled = testoEsiste;
            }

            internal void AggiornaHighlight()
            {
                // non funziona
//                rtfTesto.AggiungiHighlightDaFile();
            }

            #endregion
        }

        #endregion

        #region proprietà

        private const int LARGHEZZA_MINIMA_PANE = 35;
        private const int LARGHEZZA_SPLITTER = 1;

        internal Collection<Pane> panes = new Collection<Pane>();
        private Pane paneAttivo;
        internal Pane PaneAttivo
        {
            get { return paneAttivo; }
        }

        int branoAttuale;
        LaParola.InfoBraniParalleli info = new LaParola.InfoBraniParalleli();
        Principale genitore;

        public int NumeroBranoMostrato
        {
            get { return cbBrani.SelectedIndex; }
            set { if (value < cbBrani.Items.Count && value >= 0) cbBrani.SelectedIndex = value; }
        }

        public string GruppoParalleli
        {
            get { return info.nome; }
        }

        internal int PanelHeight
        {
            get { return panPanes.Height; }
        }

        internal RichTextBoxHighlight UltimaRtb
        {
            get { return PaneAttivo == null ? null : PaneAttivo.ControlloRichText; }
        }

        #endregion

        #region costruttori

        public BraniParalleli(Principale formGenitore, LaParola.InfoBraniParalleli informazioniSuiBraniParalleli)
        {
            InitializeComponent();
            genitore = formGenitore;
            guidaFile.HelpNamespace = formGenitore.NomeFileGuida();
            info = informazioniSuiBraniParalleli;
        }

        private void BraniParalleli_Load(object sender, EventArgs e)
        {
            int numeroColonne = info.nomiColonne.Count;
            int larghezzaColonna = (panPanes.Width - LARGHEZZA_SPLITTER * (numeroColonne - 1)) / numeroColonne;
            for (int i = 0; i < numeroColonne; ++i)
            {
                panes.Add(new Pane(this, i * (larghezzaColonna + LARGHEZZA_SPLITTER), larghezzaColonna, info.nomiColonne[i]));
                if (i < numeroColonne - 1)
                {
                    Splitter splitter = new Splitter
                    {
                        Dock = DockStyle.Right,
                        Width = LARGHEZZA_SPLITTER,
                        MinExtra = LARGHEZZA_MINIMA_PANE,
                        MinSize = LARGHEZZA_MINIMA_PANE
                    };
                    panPanes.Controls.Add(splitter);
                }
            }

            Text = info.nome;

            Collection<string> versioni = Principale.testi.NomiVersioni(TestoTipi.Bibbia);
            foreach (string versione in versioni)
            {
                cbVersioni.Items.Add(versione);
                if (Principale.testi.UltimaBibbia == versione)
                    cbVersioni.SelectedIndex = cbVersioni.Items.Count - 1;
            }
            if (cbVersioni.SelectedIndex == -1 && cbVersioni.Items.Count > 0)
                cbVersioni.SelectedIndex = 0;

            int numeroBrani = info.braniParalleli.Count;
            for (int i = 0; i < numeroBrani; ++i)
                cbBrani.Items.Add(info.braniParalleli[i].titolo);
            if (numeroBrani > 0)
                cbBrani.SelectedIndex = 0;
        }

        #endregion

        #region eventi

        private void BraniParalleli_Resize(object sender, EventArgs e)
        {
            panPanes.Size = new Size(ClientSize.Width, btnCanc.Top - 5 - panPanes.Top);
            int larghezza = pulBranoSuccessivo.Right - pulCercaVersetto.Right - 4;
            if (larghezza > 250)
                larghezza = 250;
            cbVersioni.Width = larghezza;
            cbVersioni.Left = pulBranoSuccessivo.Right - larghezza;
            int nRtf = panes.Count;
            for (int i = 1; i < nRtf; ++i)
            {
                if (i > 0)
                {
                    // non aggiustiamo il primo, perché ha DockStyle.Fill, e prende il resto dello spazio
                    panes[i].BloccaRtf(true);
                    panes[i].Width = (panPanes.Width - (nRtf - 1) * LARGHEZZA_SPLITTER) / nRtf;
                    panes[i].BloccaRtf(false);
                }
            }

            cbBrani.Width = this.Width - 86;
        }

        private void pulBranoPrecedente_Click(object sender, EventArgs e)
        {
            if (cbBrani.SelectedIndex > 0)
                cbBrani.SelectedIndex = cbBrani.SelectedIndex - 1;
        }

        private void pulBranoSuccessivo_Click(object sender, EventArgs e)
        {
            if (cbBrani.SelectedIndex < cbBrani.Items.Count - 1)
                cbBrani.SelectedIndex = cbBrani.SelectedIndex + 1;
        }

        private void cbBrani_SelectedIndexChanged(object sender, EventArgs e)
        {
            branoAttuale = cbBrani.SelectedIndex;
            pulBranoPrecedente.Enabled = (branoAttuale > 0);
            pulBranoSuccessivo.Enabled = (branoAttuale < cbBrani.Items.Count - 1);

            Aggiorna();
        }

        private void tbCercaVersetto_TextChanged(object sender, EventArgs e)
        {
            pulCercaVersetto.Enabled = !string.IsNullOrEmpty(tbCercaVersetto.Text);
        }

        private void tbCercaVersetto_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return && !string.IsNullOrEmpty(tbCercaVersetto.Text))
                pulCercaVersetto_Click(sender, e);
        }

        private void pulCercaVersetto_Click(object sender, EventArgs e)
        {
            Cursor cursoreAttuale = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                Riferimento versettoDaRicercare = Principale.testi.ConvertiRiferimento(tbCercaVersetto.Text);
                if (versettoDaRicercare.Count == 0)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("ParallelPassagesVerseNotUnderstood"), tbCercaVersetto.Text), Principale.LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions);
                    return;
                }
                int branoTrovato = -1;
                for (int i = 0; i < info.braniParalleli.Count; ++i)
                {
                    foreach (string riferimento in info.braniParalleli[i].brani)
                    {
                        if (!string.IsNullOrEmpty(riferimento))
                            if (Principale.testi.ConvertiRiferimento(riferimento).ContieneVersetto(versettoDaRicercare))
                                branoTrovato = i;
                    }
                    if (branoTrovato >= 0)
                        break;
                }
                if (branoTrovato >= 0)
                    cbBrani.SelectedIndex = branoTrovato;
                else
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("ParallelPassagesVerseNotFound"), Principale.testi.NormalizzaRiferimento(versettoDaRicercare.Brani[0][0], versettoDaRicercare.Brani[0][1], versettoDaRicercare.Brani[0][2])), Principale.LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions);
            }
            finally
            {
                Cursor.Current = cursoreAttuale;
                if (cursoreAttuale != null)
                    cursoreAttuale.Dispose();
            }
        }

        private void cbVersioni_SelectedIndexChanged(object sender, EventArgs e)
        {
            string nomeVersione = cbVersioni.SelectedItem.ToString();
            foreach (Pane pane in panes)
            {
                pane.ControlloRichText.Versione = nomeVersione;
//                pane.ControlloRichText.AggiungiHighlightDaFile();
            }
            Aggiorna();
        }

        internal void pulsanteSuGiu_Click(object sender, int direzione)
        {
            Cursor cursoreAttuale = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                int colonna = panes.IndexOf((Pane)sender);
                string riferimentoAttuale = info.braniParalleli[branoAttuale].brani[colonna];
                if (string.IsNullOrEmpty(riferimentoAttuale))
                    return;
                Riferimento riferimentoAttualeComeRiferimento = Principale.testi.ConvertiRiferimento(riferimentoAttuale);
                int indiceMiglioreTrovato = -1;
                for (int i = 0; i < info.braniParalleli.Count; ++i)
                {
                    if (!string.IsNullOrEmpty(info.braniParalleli[i].brani[colonna]))
                    {
                        Riferimento riferimentoDaProvare = Principale.testi.ConvertiRiferimento(info.braniParalleli[i].brani[colonna]);
                        if (riferimentoAttualeComeRiferimento.Compare(riferimentoAttualeComeRiferimento, riferimentoDaProvare) == direzione)
                        {
                            if (indiceMiglioreTrovato == -1 || riferimentoDaProvare.Compare(riferimentoDaProvare, Principale.testi.ConvertiRiferimento(info.braniParalleli[indiceMiglioreTrovato].brani[colonna])) == direzione)
                                indiceMiglioreTrovato = i;
                        }
                    }
                }
                if (indiceMiglioreTrovato >= 0)
                    cbBrani.SelectedIndex = indiceMiglioreTrovato;
            }
            finally
            {
                Cursor.Current = cursoreAttuale;
                if (cursoreAttuale != null)
                    cursoreAttuale.Dispose();
            }
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

        private RichTextBoxEx TuttoIlTesto()
        {
            RichTextBoxEx rtb = new RichTextBoxEx
            {
                Visible = false,
                Parent = this
            };
            for (int i = 0; i < panes.Count; ++i)
            {
                rtb.AggiungiRtf((i == 0 ? Principale.testi.RtfIntestazione() + @"\b " : Principale.testi.RtfIntestazione() + @"\par\b ") + info.nomiColonne[i] + @"\b0\par}");
                rtb.AggiungiRtf(panes[i].ControlloRichText.Rtf);
            }
            return rtb;
        }

        private void btnCanc_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        public void Aggiorna()
        {
            string testo;
            string nomeVersione = cbVersioni.SelectedItem.ToString();
            for (int i = 0; i < info.nomiColonne.Count; ++i)
            {
                if (!string.IsNullOrEmpty(info.braniParalleli[branoAttuale].brani[i]) && !string.IsNullOrEmpty(nomeVersione))
                {
                    testo = Principale.testi.TestoBrano(Principale.testi.ConvertiDaStandard(Principale.testi.ConvertiRiferimento(info.braniParalleli[branoAttuale].brani[i]), nomeVersione), nomeVersione);
                    panes[i].SetPulsanti(true);
                }
                else
                {
                    testo = "";
                    panes[i].SetPulsanti(false);
                }
                panes[i].SetTesto(testo);
            }
        }

        #region highlight

        internal void HighlighterClick(Color colore, TipoHighlight tipo)
        {
            UltimaRtb.HighlighterClick(colore, tipo);
        }

        internal void HighlighterClick(byte tipoSottolineatura)
        {
            UltimaRtb.HighlighterClick(tipoSottolineatura);
        }

        internal void HighlighterNoneClick()
        {
            UltimaRtb.HighlighterNoneClick();
        }

        internal void HighlighterNoneNotMonoClick()
        {
            UltimaRtb.HighlighterNoneNotMonoClick();
        }

        internal void AggiornaHighlight(string versione)
        {
            if (cbVersioni.SelectedItem.ToString() == versione)
            {
                foreach (Pane p in panes)
                {
                    p.AggiornaHighlight();
                }
                Aggiorna();
            }
        }

        #endregion
    }
}