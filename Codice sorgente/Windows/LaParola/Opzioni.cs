using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using LaParola.Properties;
using TestiBiblici;

namespace LaParola
{
    public partial class Opzioni : Template
    {

        #region proprietà

        private Principale genitore;

        Font font;
        Font fontGreco;
        Font fontEbraico;
        Font fontRiferimento;
        Font fontRicerca;
        Color fontColore;
        Color fontGrecoColore;
        Color fontEbraicoColore;
        Color fontRiferimentoColore;
        Color fontRicercaColore;
        int linguaVecchia;

        private bool carattereNonNumerico = false;

        #endregion

        public Opzioni(Principale formGenitore)
        {
            if (formGenitore == null)
                throw new ArgumentNullException("formGenitore");

            InitializeComponent();
            guidaFile.HelpNamespace = formGenitore.NomeFileGuida();
            genitore = formGenitore;
        }

        private void Opzioni_Load(object sender, EventArgs e)
        {
            Cursor cursoreAttuale = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                #region Formato

                switch (Principale.testi.Formato.TestoVisualizzato)
                {
                    case TestoVisualizzato.Versetti:
                        rbTesto0.Checked = true;
                        break;
                    case TestoVisualizzato.Paragrafi:
                        rbTesto1.Checked = true;
                        break;
                    case TestoVisualizzato.Nessuno:
                        rbTesto2.Checked = true;
                        break;
                    default:
                        rbTesto1.Checked = true;
                        break;
                }

                cbTitoli.Checked = Principale.testi.Formato.TitoliVisualizzati;

                switch (Principale.testi.Formato.RiferimentoTipo)
                {
                    case RiferimentoTipo.DuePunti:
                        rbRifTipo0.Checked = true;
                        break;
                    case RiferimentoTipo.Virgola:
                        rbRifTipo1.Checked = true;
                        break;
                    case RiferimentoTipo.Citazione:
                        rbRifTipo2.Checked = true;
                        break;
                    default:
                        rbRifTipo0.Checked = true;
                        break;
                }
                switch (Principale.testi.Formato.RiferimentoFormato)
                {
                    case RiferimentoFormato.Intero:
                        rbRifFormato0.Checked = true;
                        break;
                    case RiferimentoFormato.Abbreviazione:
                        rbRifFormato1.Checked = true;
                        break;
                    case RiferimentoFormato.Nessuno:
                        rbRifFormato2.Checked = true;
                        break;
                    default:
                        rbRifFormato1.Checked = true;
                        break;
                }
                switch (Principale.testi.Formato.RiferimentoPosto)
                {
                    case RiferimentoPosto.PrimaStessaRiga:
                        rbRifPosto0.Checked = true;
                        break;
                    case RiferimentoPosto.PrimaRigaDiversa:
                        rbRifPosto1.Checked = true;
                        break;
                    case RiferimentoPosto.Dopo:
                        rbRifPosto2.Checked = true;
                        break;
                    default:
                        rbRifPosto0.Checked = true;
                        break;
                }

                FontStyle fs = FontStyle.Regular;
                if (Principale.testi.Formato.FontGrassetto)
                    fs |= FontStyle.Bold;
                if (Principale.testi.Formato.FontCorsivo)
                    fs |= FontStyle.Italic;
                if (Principale.testi.Formato.FontSottolineato)
                    fs |= FontStyle.Underline;
                try
                {
                    font = new Font(Principale.testi.Formato.FontNome, Principale.testi.Formato.FontDimensione, fs);
                }
                catch (ArgumentException)
                { // con un computer, la riga precedente ha dato un errore che Palatino Linotype non ha lo stile normale. Proviamo in altri modi...
                    try
                    {
                        font = new Font(Principale.testi.Formato.FontNome, Principale.testi.Formato.FontDimensione);
                    }
                    catch (ArgumentException)
                    {
                    }
                }

                fontColore = Principale.testi.Formato.FontColore;
                if (font != null)
                    SetEtichettaFont(labFontPredef, font, fontColore);
                else
                    labFontPredef.Text = Principale.testi.Formato.FontNome + " " + Principale.testi.Formato.FontDimensione.ToString(CultureInfo.CurrentCulture);

                fs = FontStyle.Regular;
                if (Principale.testi.Formato.FontGrecoGrassetto)
                    fs |= FontStyle.Bold;
                if (Principale.testi.Formato.FontGrecoCorsivo)
                    fs |= FontStyle.Italic;
                if (Principale.testi.Formato.FontGrecoSottolineato)
                    fs |= FontStyle.Underline;
                try
                {
                    fontGreco = new Font(Principale.testi.Formato.FontGrecoNome, Principale.testi.Formato.FontGrecoDimensione, fs);
                }
                catch (ArgumentException)
                { // con un computer, la riga precedente ha dato un errore che Palatino Linotype non ha lo stile normale. Proviamo in altri modi...
                    try
                    {
                        fontGreco = new Font(Principale.testi.Formato.FontGrecoNome, Principale.testi.Formato.FontGrecoDimensione);
                    }
                    catch (ArgumentException)
                    {
                    }
                }

                fontGrecoColore = Principale.testi.Formato.FontGrecoColore;
                if (fontGreco != null)
                    SetEtichettaFont(labFontGreco, fontGreco, fontGrecoColore);
                else
                    labFontGreco.Text = Principale.testi.Formato.FontGrecoNome + " " + Principale.testi.Formato.FontGrecoDimensione.ToString(CultureInfo.CurrentCulture);

                fs = FontStyle.Regular;
                if (Principale.testi.Formato.FontEbraicoGrassetto)
                    fs |= FontStyle.Bold;
                if (Principale.testi.Formato.FontEbraicoCorsivo)
                    fs |= FontStyle.Italic;
                if (Principale.testi.Formato.FontEbraicoSottolineato)
                    fs |= FontStyle.Underline;
                try
                {
                    fontEbraico = new Font(Principale.testi.Formato.FontEbraicoNome, Principale.testi.Formato.FontEbraicoDimensione, fs);
                }
                catch (ArgumentException)
                { // con un computer, la riga precedente ha dato un errore che Palatino Linotype non ha lo stile normale. Proviamo in altri modi...
                    try
                    {
                        fontEbraico = new Font(Principale.testi.Formato.FontEbraicoNome, Principale.testi.Formato.FontEbraicoDimensione);
                    }
                    catch (ArgumentException)
                    {
                    }
                }

                fontEbraicoColore = Principale.testi.Formato.FontEbraicoColore;
                if (fontEbraico != null)
                    SetEtichettaFont(labFontEbraico, fontEbraico, fontEbraicoColore);
                else
                    labFontEbraico.Text = Principale.testi.Formato.FontEbraicoNome + " " + Principale.testi.Formato.FontEbraicoDimensione.ToString(CultureInfo.CurrentCulture);

                fs = FontStyle.Regular;
                if (Principale.testi.Formato.FontRiferimentoGrassetto)
                    fs |= FontStyle.Bold;
                if (Principale.testi.Formato.FontRiferimentoCorsivo)
                    fs |= FontStyle.Italic;
                if (Principale.testi.Formato.FontRiferimentoSottolineato)
                    fs |= FontStyle.Underline;
                try
                {
                    fontRiferimento = new Font(Principale.testi.Formato.FontRiferimentoNome, Principale.testi.Formato.FontRiferimentoDimensione, fs);
                }
                catch (ArgumentException)
                { // con un computer, la riga precedente ha dato un errore che Palatino Linotype non ha lo stile normale. Proviamo in altri modi...
                    try
                    {
                        fontRiferimento = new Font(Principale.testi.Formato.FontRiferimentoNome, Principale.testi.Formato.FontRiferimentoDimensione);
                    }
                    catch (ArgumentException)
                    {
                    }
                }

                fontRiferimentoColore = Principale.testi.Formato.FontRiferimentoColore;
                if (fontRiferimento != null)
                    SetEtichettaFont(labFontRiferimento, fontRiferimento, fontRiferimentoColore);
                else
                    labFontRiferimento.Text = Principale.testi.Formato.FontRiferimentoNome + " " + Principale.testi.Formato.FontRiferimentoDimensione.ToString(CultureInfo.CurrentCulture);
                cbRifApice.Checked = Principale.testi.Formato.RiferimentoApice;
                cbRifContestoRicerche.Checked = Principale.testi.Formato.RiferimentoContestoRicerche;

                fs = FontStyle.Regular;
                if (Principale.testi.Formato.FontRicercaGrassetto)
                    fs |= FontStyle.Bold;
                if (Principale.testi.Formato.FontRicercaCorsivo)
                    fs |= FontStyle.Italic;
                if (Principale.testi.Formato.FontRicercaSottolineato)
                    fs |= FontStyle.Underline;
                try
                {
                    fontRicerca = new Font(Principale.testi.Formato.FontRicercaNome, Principale.testi.Formato.FontRicercaDimensione, fs);
                }
                catch (ArgumentException)
                { // con un computer, la riga precedente ha dato un errore che Palatino Linotype non ha lo stile normale. Proviamo in altri modi...
                    try
                    {
                        fontRicerca = new Font(Principale.testi.Formato.FontRicercaNome, Principale.testi.Formato.FontRicercaDimensione);
                    }
                    catch (ArgumentException)
                    {
                    }
                }

                fontRicercaColore = Principale.testi.Formato.FontRicercaColore;
                if (fontRicerca != null)
                    SetEtichettaFont(labFontRicerca, fontRicerca, fontRicercaColore);
                else
                    labFontRicerca.Text = Principale.testi.Formato.FontRicercaNome + " " + Principale.testi.Formato.FontRicercaDimensione.ToString(CultureInfo.CurrentCulture);

                #endregion

                #region Testi

                lbCartelle.Items.AddRange(Settings.Default.CartelleDaCercare.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));

                string[] commentariPredefiniti = Settings.Default.Commentari.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                int nCommentariPredefiniti = commentariPredefiniti.Length;
                Collection<string> commentariTutti = Principale.testi.NomiVersioni(TestoTipi.Commentario);
                bool commentarioChecked;
                foreach (string commentario in commentariTutti)
                {
                    commentarioChecked = false;
                    for (int i = 0; i < nCommentariPredefiniti; ++i)
                        if (commentariPredefiniti[i] == commentario)
                            commentarioChecked = true;
                    clbCommentari.Items.Add(commentario, commentarioChecked);
                }

                Collection<string> dizionariTutti = Principale.testi.NomiVersioni(TestoTipi.Dizionario);
                foreach (string dizionario in dizionariTutti)
                {
                    switch (Funzioni.LinguaPrincipale(Principale.testi.Info(dizionario).Lingua))
                    {
                        case "en":
                            cbDizionariInglesi.Items.Add(dizionario);
                            if (dizionario == Settings.Default.DizionarioInglese)
                                cbDizionariInglesi.SelectedIndex = cbDizionariInglesi.Items.Count - 1;
                            break;
                        case "it":
                            cbDizionariItaliani.Items.Add(dizionario);
                            if (dizionario == Settings.Default.DizionarioItaliano)
                                cbDizionariItaliani.SelectedIndex = cbDizionariItaliani.Items.Count - 1;
                            break;
                        case "el":
                            cbDizionariGreci.Items.Add(dizionario);
                            if (dizionario == Settings.Default.DizionarioGreco)
                                cbDizionariGreci.SelectedIndex = cbDizionariGreci.Items.Count - 1;
                            break;
                        case "he":
                            cbDizionariEbraici.Items.Add(dizionario);
                            if (dizionario == Settings.Default.DizionarioEbraico)
                                cbDizionariEbraici.SelectedIndex = cbDizionariEbraici.Items.Count - 1;
                            break;
                        case "la":
                            cbDizionariLatini.Items.Add(dizionario);
                            if (dizionario == Settings.Default.DizionarioLatino)
                                cbDizionariLatini.SelectedIndex = cbDizionariLatini.Items.Count - 1;
                            break;
                    }
                }

                if (cbDizionariInglesi.Items.Count > 0)
                    cbDizionariInglesi.SelectedIndex = Math.Max(0, cbDizionariInglesi.SelectedIndex);
                else
                {
                    labDizionarioInglese.Enabled = false;
                    cbDizionariInglesi.Enabled = false;
                }
                if (cbDizionariItaliani.Items.Count > 0)
                    cbDizionariItaliani.SelectedIndex = Math.Max(0, cbDizionariItaliani.SelectedIndex);
                else
                {
                    labDizionarioItaliano.Enabled = false;
                    cbDizionariItaliani.Enabled = false;
                }
                if (cbDizionariGreci.Items.Count > 0)
                    cbDizionariGreci.SelectedIndex = Math.Max(0, cbDizionariGreci.SelectedIndex);
                else
                {
                    labDizionarioGreco.Enabled = false;
                    cbDizionariGreci.Enabled = false;
                }
                if (cbDizionariEbraici.Items.Count > 0)
                    cbDizionariEbraici.SelectedIndex = Math.Max(0, cbDizionariEbraici.SelectedIndex);
                else
                {
                    labDizionarioEbraico.Enabled = false;
                    cbDizionariEbraici.Enabled = false;
                }
                if (cbDizionariLatini.Items.Count > 0)
                    cbDizionariLatini.SelectedIndex = Math.Max(0, cbDizionariLatini.SelectedIndex);
                else
                {
                    labDizionarioLatino.Enabled = false;
                    cbDizionariLatini.Enabled = false;
                }

                cbDizionarioTooltip.Checked = Settings.Default.DizionarioTooltip;

                #endregion

                #region Libri

                string[] abbreviazioniRiconosciute = Principale.testi.LibriAbbreviazioniRiconosciute.AbbreviazioniPerLibro();
                for (int i = 0; i < 73; ++i)
                {
                    gridLibri.Rows.Add(new string[] {Principale.testi.GetLibroNome(i + 1),
                            Principale.testi.GetLibroAbbreviazioneUsata(i + 1),
                            abbreviazioniRiconosciute[i].Remove(abbreviazioniRiconosciute[i].Length - 1)});
                }

                #endregion

                #region Interfaccia

                switch (Settings.Default.InterfacciaLingua)
                {
                    case "it-IT":
                        cbLingua.SelectedIndex = 1;
                        break;
                    case "en-GB":
                        cbLingua.SelectedIndex = 2;
                        break;
                    case "es-ES":
                        cbLingua.SelectedIndex = 3;
                        break;
                    default:
                        cbLingua.SelectedIndex = 0;
                        break;
                }
                linguaVecchia = cbLingua.SelectedIndex;

                udRichiesteMemorizzate.Value = Settings.Default.MiscRichiesteMemorizzate;
                udVociMassimeMenu.Value = Settings.Default.PrincipaleMassimoVociMenu;

                cbBSPrincipale.Checked = Settings.Default.PrincipaleBSPrincipale;
                cbBSFormato.Checked = Settings.Default.PrincipaleBSFormato;
                cbBSOrdine.Checked = Settings.Default.PrincipaleBSOrdine;
                cbBSRigaComando.Checked = Settings.Default.PrincipaleBSComando;
                cbBarraDiStato.Checked = Settings.Default.PrincipaleBarraDiStato;
                cbStessaFinestraPerRisultati.Checked = Settings.Default.OpzioniStessaFinestra;
                cbIpertestoTooltip.Checked = Settings.Default.OpzioniIpertestoTooltip;
                cbIpertestoTooltipInTooltip.Checked = Settings.Default.OpzioniIpertestoTooltipInTooltip;

                #endregion

                #region Clipboard

                cbClipboardAttivo.Checked = Settings.Default.PrincipaleClipboardAttivo;
                tbClipboardTempo.Text = Settings.Default.PrincipaleClipboardTempo.ToString(CultureInfo.CurrentCulture);
                tbClipboardLunghezza.Text = Settings.Default.PrincipaleClipboardLunghezzaMassima.ToString(CultureInfo.CurrentCulture);

                #endregion

                #region Aggiornamenti

                rbAggiornaManuale.Checked = Settings.Default.AggiornamentoManuale;

                switch (Settings.Default.AggiornamentoGiorni)
                {
                    case 1:
                        cbAggiornaGiorni.SelectedIndex = 0;
                        break;
                    case 3:
                        cbAggiornaGiorni.SelectedIndex = 1;
                        break;
                    case 7:
                        cbAggiornaGiorni.SelectedIndex = 2;
                        break;
                    case 14:
                        cbAggiornaGiorni.SelectedIndex = 3;
                        break;
                    case 30:
                        cbAggiornaGiorni.SelectedIndex = 4;
                        break;
                    default:
                        cbAggiornaGiorni.SelectedIndex = 3;
                        break;
                }

                tbProxy.Text = Settings.Default.AggiornamentoProxyHost;
                if (Settings.Default.AggiornamentoProxyPorta != 0)
                    tbProxyPorta.Text = Settings.Default.AggiornamentoProxyPorta.ToString(CultureInfo.CurrentCulture);
                tbProxyNomeUtente.Text = Settings.Default.AggiornamentoProxyNomeUtente;
                tbProxyPassword.Text = Settings.Default.AggiornamentoProxyPassword;
                tbProxyDominio.Text = Settings.Default.AggiornamentoProxyDominio;

                #endregion

                #region Altre

                // non possiamo mettere l'icona in Esecuzione automatica se non siamo in Windows
                cbLetture.Visible = !Principale.isRunningOnMono;
                cbLetture.Checked = Settings.Default.LetturaAvvio;
                if (genitore.schemiLettura.Count == 0)
                { // se non ci sono schemi installati, non è possibile scegliere questa opzione
                    cbLetture.Checked = false;
                    cbLetture.Enabled = false;
                }

                Collection<string> disposizioni = genitore.DisposizioniFinestre();
                foreach (string disposizione in disposizioni)
                {
                    cbDisposizioni.Items.Add(disposizione);
                    if (Settings.Default.MiscArrangementDefaultType == 2 && disposizione == Settings.Default.MiscArrangementDefault)
                        cbDisposizioni.SelectedIndex = cbDisposizioni.Items.Count - 1;
                }
                if (Settings.Default.MiscArrangementDefaultType < 2)
                    cbDisposizioni.SelectedIndex = Settings.Default.MiscArrangementDefaultType;
                else if (cbDisposizioni.SelectedIndex == -1)
                    cbDisposizioni.SelectedIndex = 0;

                #endregion

                bool panTrovato = AggiornaPanelli(Settings.Default.OpzioniPanello);
                if (!panTrovato)
                    AggiornaPanelli("panRisultati");

                tvCategorie.ExpandAll();
            }
            finally
            {
                Cursor.Current = cursoreAttuale;
                if (cursoreAttuale != null)
                    cursoreAttuale.Dispose();
            }
        }

        private void Opzioni_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (font != null)
                font.Dispose();
            if (fontGreco != null)
                fontGreco.Dispose();
            if (fontEbraico != null)
                fontEbraico.Dispose();
            if (fontRicerca != null)
                fontRicerca.Dispose();
            if (fontRiferimento != null)
                fontRiferimento.Dispose();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string nome;
            foreach (Control c in this.Controls)
            {
                nome = c.Name;
                if (nome.Substring(0, 3) == "pan")
                {
                    if (c.Visible)
                    {
                        Settings.Default.OpzioniPanello = nome;
                        break;
                    }
                }
            }

            #region Formato

            FormatoTesto ft = new FormatoTesto();
            GetFormatoOpzioni(ft);

            if (font != null)
            {
                Settings.Default.FormatoFontNome = font.Name;
                // bisogna convertire ad un int, perché se si mette 14 punti, .NET lo cambia in 14.25
                Settings.Default.FormatoFontDimensione = Convert.ToInt32(font.SizeInPoints);
                Settings.Default.FormatoFontStileGrassetto = font.Bold;
                Settings.Default.FormatoFontStileCorsivo = font.Italic;
                Settings.Default.FormatoFontStileSotto = font.Underline;
            }
            Settings.Default.FormatoFontColore = fontColore;

            if (fontGreco != null)
            {
                Settings.Default.FormatoFontGrecoNome = fontGreco.Name;
                Settings.Default.FormatoFontGrecoDimensione = Convert.ToInt32(fontGreco.SizeInPoints);
                Settings.Default.FormatoFontGrecoStileGrassetto = fontGreco.Bold;
                Settings.Default.FormatoFontGrecoStileCorsivo = fontGreco.Italic;
                Settings.Default.FormatoFontGrecoStileSotto = fontGreco.Underline;
            }
            Settings.Default.FormatoFontGrecoColore = fontGrecoColore;

            if (fontEbraico != null)
            {
                Settings.Default.FormatoFontEbraicoNome = fontEbraico.Name;
                Settings.Default.FormatoFontEbraicoDimensione = Convert.ToInt32(fontEbraico.SizeInPoints);
                Settings.Default.FormatoFontEbraicoStileGrassetto = fontEbraico.Bold;
                Settings.Default.FormatoFontEbraicoStileCorsivo = fontEbraico.Italic;
                Settings.Default.FormatoFontEbraicoStileSotto = fontEbraico.Underline;
            }
            Settings.Default.FormatoFontEbraicoColore = fontEbraicoColore;

            if (fontRiferimento != null)
            {
                Settings.Default.FormatoFontRifNome = fontRiferimento.Name;
                Settings.Default.FormatoFontRifDimensione = Convert.ToInt32(fontRiferimento.SizeInPoints);
                Settings.Default.FormatoFontRifStileGrassetto = fontRiferimento.Bold;
                Settings.Default.FormatoFontRifStileCorsivo = fontRiferimento.Italic;
                Settings.Default.FormatoFontRifStileSotto = fontRiferimento.Underline;
                Settings.Default.FormatoFontRifColore = fontRiferimentoColore;
            }
            Settings.Default.FormatoRifApice = cbRifApice.Checked;
            Settings.Default.FormatoRifContestoRicerche = cbRifContestoRicerche.Checked;

            if (fontRicerca != null)
            {
                Settings.Default.FormatoFontRicercaNome = fontRicerca.Name;
                Settings.Default.FormatoFontRicercaDimensione = Convert.ToInt32(fontRicerca.SizeInPoints);
                Settings.Default.FormatoFontRicercaStileGrassetto = fontRicerca.Bold;
                Settings.Default.FormatoFontRicercaStileCorsivo = fontRicerca.Italic;
                Settings.Default.FormatoFontRicercaStileSotto = fontRicerca.Underline;
            }
            Settings.Default.FormatoFontRicercaColore = fontRicercaColore;

            Settings.Default.FormatoRifTipo = (int)ft.RiferimentoTipo;
            Settings.Default.FormatoRifFormato = (int)ft.RiferimentoFormato;
            Settings.Default.FormatoRifPosto = (int)ft.RiferimentoPosto;
            Settings.Default.FormatoTestoBibbia = (int)ft.TestoVisualizzato;
            Settings.Default.FormatoTitoliVisualizzati = cbTitoli.Checked;

            Principale.testi.Formato = ft;

            #endregion

            #region Testi

            StringBuilder cartelle = new StringBuilder();
            foreach (object cartella in lbCartelle.Items)
                cartelle.Append(cartella.ToString()).Append("|");
            string cartelleComeString = cartelle.ToString();
            if (cartelleComeString.Length > 0) // per togliere l'ultimo |
                cartelleComeString = cartelleComeString.Remove(cartelleComeString.Length - 1);
            Settings.Default.CartelleDaCercare = cartelleComeString;

            StringBuilder commentari = new StringBuilder("");
            for (int i = 0; i < clbCommentari.CheckedItems.Count; ++i)
                commentari.Append(clbCommentari.CheckedItems[i].ToString()).Append("|");
            Settings.Default.Commentari = commentari.ToString();

            if (cbDizionariInglesi.SelectedIndex >= 0)
                Settings.Default.DizionarioInglese = cbDizionariInglesi.Text;
            if (cbDizionariItaliani.SelectedIndex >= 0)
                Settings.Default.DizionarioItaliano = cbDizionariItaliani.Text;
            if (cbDizionariGreci.SelectedIndex >= 0)
                Settings.Default.DizionarioGreco = cbDizionariGreci.Text;
            if (cbDizionariEbraici.SelectedIndex >= 0)
                Settings.Default.DizionarioEbraico = cbDizionariEbraici.Text;
            if (cbDizionariLatini.SelectedIndex >= 0)
                Settings.Default.DizionarioLatino = cbDizionariLatini.Text;
            Settings.Default.DizionarioTooltip = cbDizionarioTooltip.Checked;

            #endregion

            #region Interfaccia

            switch (cbLingua.SelectedIndex)
            {
                case 1:
                    Settings.Default.InterfacciaLingua = "it-IT";
                    break;
                case 2:
                    Settings.Default.InterfacciaLingua = "en-GB";
                    break;
                case 3:
                    Settings.Default.InterfacciaLingua = "es-ES";
                    break;
                default:
                    Settings.Default.InterfacciaLingua = "";
                    break;
            }
            if (!String.IsNullOrEmpty(Settings.Default.InterfacciaLingua))
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(Settings.Default.InterfacciaLingua);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Settings.Default.InterfacciaLingua);
            }
            else
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InstalledUICulture;
                System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.InstalledUICulture;
            }
            if (cbLingua.SelectedIndex > 0 && cbLingua.SelectedIndex != linguaVecchia)
            {
                if (MessageBox.Show(Principale.LocRM.GetString("OptionsChangeLanguage"), Principale.LocRM.GetString("MiscConfirm"), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions) == DialogResult.Yes)
                    CambiaLingua(cbLingua.SelectedIndex);
            }

            Settings.Default.MiscRichiesteMemorizzate = (int)(udRichiesteMemorizzate.Value);
            Settings.Default.PrincipaleMassimoVociMenu = (int)(udVociMassimeMenu.Value);

            Settings.Default.PrincipaleBSPrincipale = cbBSPrincipale.Checked;
            Settings.Default.PrincipaleBSFormato = cbBSFormato.Checked;
            Settings.Default.PrincipaleBSOrdine = cbBSOrdine.Checked;
            Settings.Default.PrincipaleBSComando = cbBSRigaComando.Checked;

            Settings.Default.PrincipaleBarraDiStato = cbBarraDiStato.Checked;
            Settings.Default.OpzioniStessaFinestra = cbStessaFinestraPerRisultati.Checked;
            Settings.Default.OpzioniIpertestoTooltip = cbIpertestoTooltip.Checked;
            Settings.Default.OpzioniIpertestoTooltipInTooltip = cbIpertestoTooltipInTooltip.Checked;

            #endregion

            #region Libri
            // deve essere la sezione Interfaccia, perché un cambiamento di lingua dell'interfaccia può
            // creare un cambiamento nei libri (usando i nomi predefiniti nella nuova lingua)

            string[] abbRiconoLibro;
            string[] libriAbbRiconosciute = new string[74];
            Principale.testi.LibriAbbreviazioniRiconosciute.Clear();
            for (byte i = 1; i <= 73; ++i)
            {
                DataGridViewRow riga = gridLibri.Rows[i - 1];
                Principale.testi.SetLibroNome(i, riga.Cells[0].Value.ToString());
                Principale.testi.SetLibroAbbreviazioneUsata(i, riga.Cells[1].Value.ToString());
                abbRiconoLibro = riga.Cells[2].Value.ToString().Split(',');
                libriAbbRiconosciute[i] = riga.Cells[2].Value.ToString();
                foreach (string abbreviazioneRiconosciuta in abbRiconoLibro)
                {
                    try
                    {
                        Principale.testi.LibriAbbreviazioniRiconosciute[abbreviazioneRiconosciuta] = i;
                    }
                    catch (ArgumentException) { } // due abbreviazioni uguali (la seconda non è memorizzata) oppure abbreviazioneRiconosciuta == null
                }
            }
            StringBuilder libriNomi = new StringBuilder("");
            StringBuilder libriAbbreviazioniUsate = new StringBuilder("");
            for (int i = 1; i <= 73; ++i)
            {
                libriNomi.Append("|").Append(Principale.testi.GetLibroNome(i));
                libriAbbreviazioniUsate.Append("|").Append(Principale.testi.GetLibroAbbreviazioneUsata(i));
            }
            Settings.Default.LibriNomi = libriNomi.ToString(); // diventa "|Genesi|Esodo|...|Apocalisse"
            Settings.Default.LibriAbbUsate = libriAbbreviazioniUsate.ToString();
            Settings.Default.LibriAbbRiconosciute = String.Join("|", libriAbbRiconosciute);

            #endregion

            #region Clipboard

            Settings.Default.PrincipaleClipboardAttivo = cbClipboardAttivo.Checked;
            try
            {
                Settings.Default.PrincipaleClipboardTempo = Convert.ToInt32(tbClipboardTempo.Text, CultureInfo.CurrentCulture);
            }
            catch
            {
                Settings.Default.PrincipaleClipboardTempo = 1000;
            }
            try
            {
                Settings.Default.PrincipaleClipboardLunghezzaMassima = Convert.ToInt32(tbClipboardLunghezza.Text, CultureInfo.CurrentCulture);
            }
            catch
            {
                Settings.Default.PrincipaleClipboardLunghezzaMassima = 1000;
            }

            #endregion

            #region Aggiornamenti

            Settings.Default.AggiornamentoManuale = rbAggiornaManuale.Checked;

            switch (cbAggiornaGiorni.SelectedIndex)
            {
                case 0:
                    Settings.Default.AggiornamentoGiorni = 1;
                    break;
                case 1:
                    Settings.Default.AggiornamentoGiorni = 3;
                    break;
                case 2:
                    Settings.Default.AggiornamentoGiorni = 7;
                    break;
                case 3:
                    Settings.Default.AggiornamentoGiorni = 14;
                    break;
                case 4:
                    Settings.Default.AggiornamentoGiorni = 30;
                    break;
                default:
                    Settings.Default.AggiornamentoGiorni = 14;
                    break;
            }

            Settings.Default.AggiornamentoProxyHost = tbProxy.Text;
            if (!string.IsNullOrEmpty(tbProxyPorta.Text))
            {
                try
                {
                    Settings.Default.AggiornamentoProxyPorta = Convert.ToInt32(tbProxyPorta.Text, CultureInfo.CurrentCulture);
                }
                catch
                {
                    Settings.Default.AggiornamentoProxyPorta = 0;
                }
            }
            else
            {
                Settings.Default.AggiornamentoProxyPorta = 0;
            }
            Settings.Default.AggiornamentoProxyNomeUtente = tbProxyNomeUtente.Text;
            Settings.Default.AggiornamentoProxyPassword = tbProxyPassword.Text;
            Settings.Default.AggiornamentoProxyDominio = tbProxyDominio.Text;

            #endregion

            #region Altre

            if (Settings.Default.LetturaAvvio != cbLetture.Checked && !Principale.isRunningOnMono)
            {
                Settings.Default.LetturaAvvio = cbLetture.Checked;
                if (cbLetture.Checked)
                {
                    MSjogren.Samples.ShellLink.ShellShortcut m_Shortcut = new MSjogren.Samples.ShellLink.ShellShortcut(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + Path.DirectorySeparatorChar + Principale.LocRM.GetString("StartupReadingsTitle") + ".lnk")
                    {
                        Path = Application.ExecutablePath,
                        WorkingDirectory = Path.GetDirectoryName(Application.ExecutablePath),
                        Arguments = "readings",
                        Description = Principale.LocRM.GetString("StartupReadingsDescription"),
                        IconPath = Application.ExecutablePath,
                        IconIndex = 0,
                        WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal
                    };
                    m_Shortcut.Save();
                }
                else
                { // nota non funziona se la lingua dell'interfaccia del programma è stata cambiata dopo la creazione del link
                    File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + Path.DirectorySeparatorChar + Principale.LocRM.GetString("StartupReadingsTitle") + ".lnk");
                }
            }

            if (cbDisposizioni.SelectedIndex < 2)
            {
                Settings.Default.MiscArrangementDefaultType = cbDisposizioni.SelectedIndex;
                Settings.Default.MiscArrangementDefault = "";
            }
            else
            {
                Settings.Default.MiscArrangementDefaultType = 2;
                Settings.Default.MiscArrangementDefault = cbDisposizioni.SelectedItem.ToString();
            }

            #endregion

            this.Close();
        }

        private static void SetEtichettaFont(Label etichetta, Font font, Color fontColore)
        {
            // bisogna convertire ad un int, perché se si mette 14 punti, .NET lo cambia in 14.25
            etichetta.Text = font.Name + " " + Convert.ToInt32(font.SizeInPoints).ToString(CultureInfo.CurrentCulture);
            etichetta.Font = font;
            etichetta.ForeColor = fontColore;
            if (etichetta.ForeColor == etichetta.BackColor)
                etichetta.BackColor = Color.White;
            if (etichetta.ForeColor == etichetta.BackColor)
                etichetta.BackColor = Color.Black;
        }

        #region cliccare pulsante cambia font

        private void btnFont_Click(object sender, EventArgs e)
        {
            FontDialog fd = new FontDialog
            {
                AllowScriptChange = false,
                AllowVerticalFonts = false,
                ShowColor = true
            };
            if (font != null)
                fd.Font = font;
            fd.Color = fontColore;
            if (fd.ShowDialog(this) == DialogResult.OK)
            {
                font = fd.Font;
                fontColore = fd.Color;
                SetEtichettaFont(labFontPredef, font, fontColore);
            }
        }

        private void btnFontGreco_Click(object sender, EventArgs e)
        {
            FontDialog fd = new FontDialog
            {
                AllowScriptChange = false,
                AllowVerticalFonts = false,
                ShowColor = true
            };
            if (fontGreco != null)
                fd.Font = fontGreco;
            fd.Color = fontGrecoColore;
            if (fd.ShowDialog(this) == DialogResult.OK)
            {
                fontGreco = fd.Font;
                fontGrecoColore = fd.Color;
                SetEtichettaFont(labFontGreco, fontGreco, fontGrecoColore);
            }

        }

        private void btnFontEbraico_Click(object sender, EventArgs e)
        {
            FontDialog fd = new FontDialog
            {
                AllowScriptChange = false,
                AllowVerticalFonts = false,
                ShowColor = true
            };
            if (fontEbraico != null)
                fd.Font = fontEbraico;
            fd.Color = fontEbraicoColore;
            if (fd.ShowDialog(this) == DialogResult.OK)
            {
                fontEbraico = fd.Font;
                fontEbraicoColore = fd.Color;
                SetEtichettaFont(labFontEbraico, fontEbraico, fontEbraicoColore);
            }

        }

        private void btnFontRif_Click(object sender, EventArgs e)
        {
            FontDialog fd = new FontDialog
            {
                AllowScriptChange = false,
                AllowVerticalFonts = false,
                ShowColor = true
            };
            if (fontRiferimento != null)
                fd.Font = fontRiferimento;
            fd.Color = fontRiferimentoColore;
            if (fd.ShowDialog(this) == DialogResult.OK)
            {
                fontRiferimento = fd.Font;
                fontRiferimentoColore = fd.Color;
                SetEtichettaFont(labFontRiferimento, fontRiferimento, fontRiferimentoColore);
            }
        }

        private void btnFontRicerca_Click(object sender, EventArgs e)
        {
            FontDialog fd = new FontDialog
            {
                AllowScriptChange = false,
                AllowVerticalFonts = false,
                ShowColor = true
            };
            if (fontRicerca != null)
                fd.Font = fontRicerca;
            fd.Color = fontRicercaColore;
            if (fd.ShowDialog(this) == DialogResult.OK)
            {
                fontRicerca = fd.Font;
                fontRicercaColore = fd.Color;
                SetEtichettaFont(labFontRicerca, fontRicerca, fontRicercaColore);
            }
        }

        #endregion

        private void btnLibri_Click(object sender, EventArgs e)
        {
            if (sender == btnLibriItaliano)
                CambiaLingua(1);
            else if (sender == btnLibriInglese)
                CambiaLingua(2);
            else
                CambiaLingua(3);
        }

        private void GetFormatoOpzioni(FormatoTesto ft)
        {
            Principale.testi.Formato.CopiaA(ft);

            ft.FontNome = font.Name;
            // bisogna convertire ad un int, perché se si mette 14 punti, .NET lo cambia in 14.25
            ft.FontDimensione = Convert.ToInt32(font.SizeInPoints);
            ft.FontGrassetto = font.Bold;
            ft.FontCorsivo = font.Italic;
            ft.FontSottolineato = font.Underline;
            ft.FontColore = fontColore;

            ft.FontGrecoNome = fontGreco.Name;
            ft.FontGrecoDimensione = Convert.ToInt32(fontGreco.SizeInPoints);
            ft.FontGrecoGrassetto = fontGreco.Bold;
            ft.FontGrecoCorsivo = fontGreco.Italic;
            ft.FontGrecoSottolineato = fontGreco.Underline;
            ft.FontGrecoColore = fontGrecoColore;

            ft.FontEbraicoNome = fontEbraico.Name;
            ft.FontEbraicoDimensione = Convert.ToInt32(fontEbraico.SizeInPoints);
            ft.FontEbraicoGrassetto = fontEbraico.Bold;
            ft.FontEbraicoCorsivo = fontEbraico.Italic;
            ft.FontEbraicoSottolineato = fontEbraico.Underline;
            ft.FontEbraicoColore = fontEbraicoColore;

            ft.FontRiferimentoNome = fontRiferimento.Name;
            ft.FontRiferimentoDimensione = Convert.ToInt32(fontRiferimento.SizeInPoints);
            ft.FontRiferimentoGrassetto = fontRiferimento.Bold;
            ft.FontRiferimentoCorsivo = fontRiferimento.Italic;
            ft.FontRiferimentoSottolineato = fontRiferimento.Underline;
            ft.FontRiferimentoColore = fontRiferimentoColore;

            ft.FontRicercaNome = fontRicerca.Name;
            ft.FontRicercaDimensione = Convert.ToInt32(fontRicerca.SizeInPoints);
            ft.FontRicercaGrassetto = fontRicerca.Bold;
            ft.FontRicercaCorsivo = fontRicerca.Italic;
            ft.FontRicercaSottolineato = fontRicerca.Underline;
            ft.FontRicercaColore = fontRicercaColore;

            ft.RiferimentoApice = cbRifApice.Checked;
            ft.RiferimentoContestoRicerche = cbRifContestoRicerche.Checked;
            if (rbRifTipo0.Checked)
            {
                ft.RiferimentoTipo = RiferimentoTipo.DuePunti;
            }
            if (rbRifTipo1.Checked)
            {
                ft.RiferimentoTipo = RiferimentoTipo.Virgola;
            }
            if (rbRifTipo2.Checked)
            {
                ft.RiferimentoTipo = RiferimentoTipo.Citazione;
            }
            if (rbRifFormato0.Checked)
            {
                ft.RiferimentoFormato = RiferimentoFormato.Intero;
            }
            if (rbRifFormato1.Checked)
            {
                ft.RiferimentoFormato = RiferimentoFormato.Abbreviazione;
            }
            if (rbRifFormato2.Checked)
            {
                ft.RiferimentoFormato = RiferimentoFormato.Nessuno;
            }
            if (rbRifPosto0.Checked)
            {
                ft.RiferimentoPosto = RiferimentoPosto.PrimaStessaRiga;
            }
            if (rbRifPosto1.Checked)
            {
                ft.RiferimentoPosto = RiferimentoPosto.PrimaRigaDiversa;
            }
            if (rbRifPosto2.Checked)
            {
                ft.RiferimentoPosto = RiferimentoPosto.Dopo;
            }

            if (rbTesto0.Checked)
            {
                ft.TestoVisualizzato = TestoVisualizzato.Versetti;
            }
            if (rbTesto1.Checked)
            {
                ft.TestoVisualizzato = TestoVisualizzato.Paragrafi;
            }
            if (rbTesto2.Checked)
            {
                ft.TestoVisualizzato = TestoVisualizzato.Nessuno;
            }
            ft.TitoliVisualizzati = cbTitoli.Checked;
        }

        private void pulReset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Principale.LocRM.GetString("OptionsReset"), Principale.LocRM.GetString("MiscConfirm"), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions) == DialogResult.Yes)
            {
                Settings.Default.Reset();
                this.Close();
            }
        }

        private void pulReload_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Principale.LocRM.GetString("OptionsReload"), Principale.LocRM.GetString("MiscConfirm"), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions) == DialogResult.Yes)
            {
                Settings.Default.Reload();
                this.Close();
            }
        }

        private void tvCategorie_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                AggiornaPanelli("pan" + ((TreeView)(sender)).SelectedNode.Text);
            }
            catch
            {
                AggiornaPanelli("panRisultati");
            }
        }

        private bool AggiornaPanelli(string nomePanello)
        {
            // se il nome è in inglese, dobbiamo tradurre in italiano (perché i nomi dei panelli sono così)
            // non è molto elegante (era meglio usare le risorse), ma è l'unico modo per farlo funzionare in Mono,
            // a cui manca molti aspetti della classe TreeViewNode
            switch (nomePanello)
            {
                case "panResults":
                    nomePanello = "panRisultati";
                    break;
                case "panText":
                    nomePanello = "panTesto";
                    break;
                case "panReferences":
                    nomePanello = "panRiferimenti";
                    break;
                case "panFonts":
                    nomePanello = "panCaratteri";
                    break;
                case "panTexts":
                    nomePanello = "panTesti";
                    break;
                case "panDictionaries":
                    nomePanello = "panDizionari";
                    break;
                case "panBooks":
                    nomePanello = "panLibri";
                    break;
                case "panInterface":
                    nomePanello = "panInterfaccia";
                    break;
                // non serve cambiare Clipboard
                case "panUpdates":
                    nomePanello = "panAggiornamenti";
                    break;
                case "panMiscellaneous":
                    nomePanello = "panAltre";
                    break;
            }
            if (nomePanello == panRisultati.Name && !string.IsNullOrEmpty(Principale.testi.UltimaBibbia))
            {
                Collection<string> versioni = new Collection<string>
                {
                    Principale.testi.UltimaBibbia
                };
                FormatoTesto ftAttuale = new FormatoTesto();
                ftAttuale = Principale.testi.Formato;
                FormatoTesto ft = new FormatoTesto();
                GetFormatoOpzioni(ft);
                Principale.testi.Formato = ft;
                // mostrare i primi quattro versetti del primo libro nella nomeVersione con testo
                rtEsempio.Rtf = Principale.testi.TestoBrano(Principale.testi.LibriAbbreviazioniRiconosciute.Abbreviazione(Principale.testi.LibroDiCapitolo(1, versioni[0])) + " 1:1-4", versioni);
                Principale.testi.Formato = ftAttuale;
            }

            bool panelloTrovato = false;
            foreach (Control controllo in this.Controls)
            {
                if (controllo.Name.Substring(0, 3) == "pan")
                {
                    controllo.Visible = (controllo.Name == nomePanello);
                    if (controllo.Visible)
                        panelloTrovato = true;
                }
            }
            return panelloTrovato;
        }

        private void CambiaLingua(int lingua)
        {
            string nomi, abbUsate, abbRicono;
            if (lingua == 1)
            {
                nomi = Texts.LibriNomiItaliano;
                abbUsate = Texts.LibriAbbreviazioniUsateItaliano;
                abbRicono = Texts.LibriAbbreviazioniRiconosciuteItaliano;
            }
            else if (lingua==2)
            {
                nomi = Texts.LibriNomiInglese;
                abbUsate = Texts.LibriAbbreviazioniUsateInglese;
                abbRicono = Texts.LibriAbbreviazioniRiconosciuteInglese;
            }
            else
            {
                nomi = Texts.LibriNomiSpagnolo;
                abbUsate = Texts.LibriAbbreviazioniUsateSpagnolo;
                abbRicono = Texts.LibriAbbreviazioniRiconosciuteSpagnolo;
            }

            string[] nomiA = nomi.Split('|');
            string[] abbUsateA = abbUsate.Split('|');
            string[] abbRiconoA = abbRicono.Split('|');

            DataGridViewCellCollection cellule;
            for (int i = 0; i < 73; ++i)
            {
                cellule = gridLibri.Rows[i].Cells;
                cellule[0].Value = nomiA[i + 1];
                cellule[1].Value = abbUsateA[i + 1];
                cellule[2].Value = abbRiconoA[i + 1];
            }
        }

        #region Testi

        private void pulAggiungiCartella_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialogo = new FolderBrowserDialog())
            {
                dialogo.Description = Principale.LocRM.GetString("OptionsAddDirectory");
                if (dialogo.ShowDialog() == DialogResult.OK)
                    lbCartelle.Items.Add(dialogo.SelectedPath);
            }
        }

        private void pulCancellaCartella_Click(object sender, EventArgs e)
        {
            if (lbCartelle.SelectedIndex > -1)
                lbCartelle.Items.RemoveAt(lbCartelle.SelectedIndex);
        }

        private void lbCartelle_SelectedIndexChanged(object sender, EventArgs e)
        {
            pulCancellaCartella.Enabled = (lbCartelle.SelectedIndex > -1);
        }

        #endregion

        #region Clipboard

        private void tbClipboardLunghezza_KeyDown(object sender, KeyEventArgs e)
        {
            ControllaCarattereNumerico(e);
        }

        private void tbClipboardLunghezza_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (carattereNonNumerico == true)
                e.Handled = true;
        }

        private void tbClipboardTempo_KeyDown(object sender, KeyEventArgs e)
        {
            ControllaCarattereNumerico(e);
        }

        private void tbClipboardTempo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (carattereNonNumerico == true)
                e.Handled = true;
        }

        #endregion

        #region Aggiornamento

        private void rbAggiornaAutomatica_CheckedChanged(object sender, EventArgs e)
        {
            cbAggiornaGiorni.Enabled = rbAggiornaAutomatica.Checked;
        }

        private void tbPorta_KeyDown(object sender, KeyEventArgs e)
        {
            ControllaCarattereNumerico(e);
        }

        private void tbPorta_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (carattereNonNumerico == true)
                e.Handled = true;
        }

        #endregion

        private void ControllaCarattereNumerico(KeyEventArgs e)
        {
            carattereNonNumerico = false;
            if (e.KeyCode < Keys.D0 || e.KeyCode > Keys.D9)
            {
                if (e.KeyCode < Keys.NumPad0 || e.KeyCode > Keys.NumPad9)
                {
                    if (e.KeyCode != Keys.Back)
                    {
                        if (e.Control == false || (e.KeyCode != Keys.V && e.KeyCode != Keys.C && e.KeyCode != Keys.X))
                            carattereNonNumerico = true;
                    }
                }
            }
        }

    }
}