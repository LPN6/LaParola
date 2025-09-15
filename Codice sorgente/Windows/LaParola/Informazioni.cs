using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using LaParola.Properties;
using TestiBiblici;

namespace LaParola
{
    public partial class Informazioni : Template
    {
        private Principale genitore;

        public Informazioni(Principale formGenitore)
        {
            if (formGenitore == null)
                throw new ArgumentNullException("formGenitore");

            InitializeComponent();
            genitore = formGenitore;

            cbInfo.Items.AddRange(Settings.Default.InfoPrecedenti.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));

            guidaFile.HelpNamespace = genitore.NomeFileGuida();
        }

        private void Informazioni_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ((e.CloseReason != CloseReason.WindowsShutDown) && (e.CloseReason != CloseReason.TaskManagerClosing))
            {
                int nBraniDaSalvare = Settings.Default.MiscRichiesteMemorizzate;
                if (cbInfo.Items.Count < nBraniDaSalvare)
                    nBraniDaSalvare = cbInfo.Items.Count;
                StringBuilder testiDaSalvare = new StringBuilder("");
                for (int i = 0; i < nBraniDaSalvare; ++i)
                    testiDaSalvare.Append("|").Append(cbInfo.Items[i]);
                Settings.Default.InfoPrecedenti = testiDaSalvare.ToString();
            }

        }

        private void Informazioni_Resize(object sender, EventArgs e)
        {
            tvRisultati.Width = this.Width - 192;
            tvRisultati.Height = this.Height - 96;
        }

        public void ImpostaRichiesta(string richiesta)
        {
            bool temaSicuro = false;
            if (richiesta.StartsWith("<", StringComparison.Ordinal) && richiesta.EndsWith(">", StringComparison.Ordinal))
            {
                richiesta = richiesta.Substring(1, richiesta.Length - 2);
                temaSicuro = true;
            }
            cbInfo.Text = richiesta;
            if (!string.IsNullOrEmpty(richiesta))
            {
                if (!temaSicuro && Char.IsDigit(richiesta[richiesta.Length - 1]))
                    pulsante_Click(pulRiferimento, null);
                else
                    pulsante_Click(pulTema, null);
            }
        }

        private void btnCanc_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cbInfo_TextChanged(object sender, EventArgs e)
        {
            pulRiferimento.Enabled = (!String.IsNullOrEmpty(cbInfo.Text));
            pulTema.Enabled = pulRiferimento.Enabled;
        }

        private void pulsante_Click(object sender, EventArgs e)
        {
            Cursor cursoreAttuale = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                string testo = cbInfo.Text.ToLowerInvariant().Trim();

                tvRisultati.BeginUpdate();
                tvRisultati.Nodes.Clear();
                TreeNode nodo = null, nodo1 = null, nodo2, nodo3;
                bool primoNodo;
                Riferimento testoComeRiferimento;
                string tipoLink = ((Control)(sender)).Tag.ToString();

                if (tipoLink == "Riferimento")
                {
                    #region Riferimento

                    testoComeRiferimento = Principale.testi.ConvertiRiferimento(testo);

                    primoNodo = true;
                    foreach (string versione in Principale.testi.NomiVersioni(TestoTipi.Bibbia))
                    {
                        if (Principale.testi.EsisteBrano(testoComeRiferimento, versione))
                        {
                            if (primoNodo)
                            {
                                nodo = tvRisultati.Nodes.Add(Principale.LocRM.GetString("InformationBible"));
                                nodo1 = tvRisultati.Nodes.Add(Principale.LocRM.GetString("InformationConcordance"));
                                primoNodo = false;
                            }
                            nodo2 = nodo.Nodes.Add(versione);
                            nodo2.Tag = "mostra \"" + testo + "\" da \"" + versione + "\"";
                            nodo2 = nodo1.Nodes.Add(versione);
                            nodo2.Tag = "chiave \"" + testo + "\" da \"" + versione + "\"";
                        }
                    }
                    if (nodo != null && nodo.Nodes.Count > 1)
                    {
                        nodo2 = nodo.Nodes.Insert(0, Principale.LocRM.GetString("InformationBiblesAll"));
                        nodo2.Tag = "mostra \"" + testo + "\" da bibbie";
                    }

                    primoNodo = true;
                    nodo = null;
                    foreach (string versione in Principale.testi.NomiVersioni(TestoTipi.Commentario))
                    {
                        if (Principale.testi.EsisteBrano(testoComeRiferimento, versione))
                        {
                            if (primoNodo)
                            {
                                if (tvRisultati.Nodes.Count > 0)
                                    nodo = tvRisultati.Nodes.Insert(1, Principale.LocRM.GetString("InformationCommentary"));
                                else
                                    nodo = tvRisultati.Nodes.Add(Principale.LocRM.GetString("InformationCommentary"));
                                primoNodo = false;
                            }
                            nodo2 = nodo.Nodes.Add(versione);
                            nodo2.Tag = "mostra \"" + testo + "\" da \"" + versione + "\"";
                        }
                    }
                    if (nodo != null && nodo.Nodes.Count > 1)
                    {
                        nodo2 = nodo.Nodes.Insert(0, Principale.LocRM.GetString("InformationCommentariesAll"));
                        nodo2.Tag = "mostra \"" + testo + "\" da commentari";
                    }

                    // brani simili
                    primoNodo = true;
                    foreach (string versione in Principale.testi.NomiVersioni(TestoTipi.Bibbia))
                    {
                        if (primoNodo)
                        {
                            nodo = tvRisultati.Nodes.Add(Principale.LocRM.GetString("InformationSimilar"));
                            primoNodo = false;
                        }
                        nodo2 = nodo.Nodes.Add(versione);
                        nodo2.Tag = "simili \"" + testo + "\" da \"" + versione + "\"";
                    }

                    // definizioni di parole nel brano
                    primoNodo = true;
                    foreach (string versione in Principale.testi.NomiVersioni(TestoTipi.Bibbia))
                    {
                        if (!string.IsNullOrEmpty(Funzioni.DizionarioDiVersione(versione)))
                        {
                            if (primoNodo)
                            {
                                nodo = tvRisultati.Nodes.Add(Principale.LocRM.GetString("InformationDefinitions"));
                                primoNodo = false;
                            }
                            nodo2 = nodo.Nodes.Add(versione);
                            nodo2.Tag = "definizioni \"" + testo + "\" da \"" + versione + "\"";
                        }
                    }

                    primoNodo = true;
                    int numeroVolteAppare;
                    Riferimento noteInCuiAppare = new Riferimento(false);
                    foreach (string versione in Principale.testi.NomiVersioni(TestoTipi.Commentario | TestoTipi.Dizionario))
                    {
                        noteInCuiAppare = Principale.testi.Citazioni(testoComeRiferimento, versione);
                        numeroVolteAppare = noteInCuiAppare.Count;
                        if (numeroVolteAppare > 0)
                        {
                            if (primoNodo)
                            {
                                nodo = tvRisultati.Nodes.Add(Principale.LocRM.GetString("InformationReferences"));
                                primoNodo = false;
                            }
                            nodo2 = nodo.Nodes.Add(versione + " (" + numeroVolteAppare.ToString(CultureInfo.InvariantCulture) + ")");
                            nodo2.Tag = "riferimenti \"" + testo + "\" da \"" + versione + "\"";
                            foreach (string nota in noteInCuiAppare.Note)
                            {
                                if (nota.StartsWith("#", StringComparison.Ordinal))
                                    nodo3 = nodo2.Nodes.Add(Principale.testi.ConvertiTitoloNotaARiferimento(nota));
                                else
                                    nodo3 = nodo2.Nodes.Add(nota);
                                nodo3.Tag = "nota \"" + nota + "\" da \"" + versione + "\"";
                            }
                        }
                    }

                    // brani paralleli
                    primoNodo = true;
                    nodo = null;
                    string riferimento;
                    int numeroBraniParalleli = genitore.parallelsToolStripMenuItem.DropDownItems.Count;
                    for (int i = 0; i < numeroBraniParalleli; ++i)
                    {
                        foreach (LaParola.InfoBranoParallelo branoParallelo in ((LaParola.InfoBraniParalleli)genitore.parallelsToolStripMenuItem.DropDownItems[i].Tag).braniParalleli)
                        {
                            for (int j = 0; j < branoParallelo.brani.Count; ++j)
                            {
                                riferimento = branoParallelo.brani[j];
                                if (!string.IsNullOrEmpty(riferimento))
                                {
                                    if (Principale.testi.ConvertiRiferimento(riferimento).ContieneBrano(testoComeRiferimento))
                                    { // il riferimento è stato trovato in uno dei brani paralleli
                                        string braniDaVisualizzare = "";
                                        for (int k = 0; k < branoParallelo.brani.Count; ++k)
                                        {
                                            if (k != j && !string.IsNullOrEmpty(branoParallelo.brani[k]))
                                                braniDaVisualizzare += branoParallelo.brani[k] + ";";
                                        }
                                        if (!string.IsNullOrEmpty(braniDaVisualizzare)) // altrimenti è elencato nei brani paralleli, senza avere dei paralleli
                                        {
                                            if (primoNodo)
                                            {
                                                nodo = tvRisultati.Nodes.Add(Principale.LocRM.GetString("InformationParallels"));
                                                primoNodo = false;
                                            }
                                            nodo2 = nodo.Nodes.Add(branoParallelo.titolo + " (" + ((LaParola.InfoBraniParalleli)genitore.parallelsToolStripMenuItem.DropDownItems[i].Tag).nome + ")");
                                            nodo2.Tag = "mostra \"" + braniDaVisualizzare + "\"";
                                        }
                                    }
                                }
                            }
                        }
                    }

                    #endregion
                }
                else // informazioni su un tema
                {
                    #region Tema

                    testoComeRiferimento = new Riferimento(false);
                    testoComeRiferimento.AggiungiNotaEParole(testo, new Collection<ushort>());

                    primoNodo = true;
                    int numeroVolteAppare;
                    Riferimento doveAppare = new Riferimento();
                    string riferimentoComeStringa;
                    string radice;
                    foreach (string versione in Principale.testi.NomiVersioni(TestoTipi.Bibbia))
                    {
                        radice = testo;
                        try
                        {
                            radice = Principale.testi.RadiceDiParola(testo, versione);
                            if (string.IsNullOrEmpty(radice) || radice == "*")
                            { // la parola non ha radice; ma forse è una radice che non appare nella nomeVersione
                                if (Principale.testi.ParoleDiRadice(testo, versione).Count > 0)
                                    radice = "/" + testo;
                                else  // se non, cerchiamo solo la parola
                                {
                                    radice = testo;
                                    if (radice.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }) >= 0 && radice.IndexOfAny(new char[] { '<', '>' }) == 0)
                                        radice = "<" + radice + ">";
                                }
                            }
                            else
                                radice = "/" + radice;
                            doveAppare = Principale.testi.Ricerca(radice, versione);
                        }
                        catch // errore per esempio di sintassi
                        {
                            doveAppare.Clear();
                        }
                        numeroVolteAppare = doveAppare.Count;
                        if (numeroVolteAppare > 0)
                        {
                            if (primoNodo)
                            {
                                nodo = tvRisultati.Nodes.Add(Principale.LocRM.GetString("InformationBible"));
                                primoNodo = false;
                            }
                            nodo2 = nodo.Nodes.Add(versione + " (" + numeroVolteAppare.ToString(CultureInfo.CurrentCulture) + ")");
                            nodo2.Tag = "ricerca \"" + radice + "\" da \"" + versione + "\"";
                            foreach (byte[] brano in doveAppare.Brani)
                            {
                                riferimentoComeStringa = Principale.testi.NormalizzaRiferimento(new Riferimento(brano));
                                nodo3 = nodo2.Nodes.Add(riferimentoComeStringa);
                                nodo3.Tag = "mostra \"" + riferimentoComeStringa + "\" da \"" + versione + "\"";
                            }
                        }
                    }

                    primoNodo = true;
                    foreach (string versione in Principale.testi.NomiVersioni(TestoTipi.Dizionario))
                    {
                        string parolaDaMostrare = "";
                        if (Principale.testi.EsisteBrano(testoComeRiferimento, versione))
                            parolaDaMostrare = testo;
                        else
                        {
                            radice = Principale.testi.RadiceDiParola(testo, versione);
                            if (!string.IsNullOrEmpty(radice) && radice != "*")
                            {
                                Riferimento radiceComeRiferimento = new Riferimento(false);
                                radiceComeRiferimento.AggiungiNotaEParole(radice, new Collection<ushort>());
                                if (Principale.testi.EsisteBrano(radiceComeRiferimento, versione))
                                    parolaDaMostrare = radice;
                            }
                        }
                        if (!string.IsNullOrEmpty(parolaDaMostrare))
                        {
                            if (primoNodo)
                            {
                                nodo = tvRisultati.Nodes.Add(Principale.LocRM.GetString("InformationNote"));
                                primoNodo = false;
                            }
                            nodo2 = nodo.Nodes.Add(versione);
                            nodo2.Tag = "nota \"" + parolaDaMostrare + "\" da \"" + versione + "\"";
                        }
                    }

                    primoNodo = true;
                    doveAppare.Clear();
                    doveAppare.Versetti = false;
                    foreach (string versione in Principale.testi.NomiVersioni(TestoTipi.Commentario | TestoTipi.Dizionario))
                    {
                        radice = testo;
                        try
                        {
                            // se la parola da ricerca ha una radice, mostriamo tutte le parole della radice
                            // se non, se la parola è una radice mostriamo tutte le parole della parola come radice
                            // se non, mostriamo la parola come parola
                            radice = Principale.testi.RadiceDiParola(testo, versione);
                            if (string.IsNullOrEmpty(radice) || radice == "*")
                            {
                                if (Principale.testi.ParoleDiRadice(testo, versione).Count > 0)
                                    radice = "/" + testo;
                                else
                                    radice = testo;
                            }
                            else
                                radice = "/" + radice;
                            doveAppare = Principale.testi.Ricerca(radice, versione);
                        }
                        catch // errore per esempio di sintassi
                        {
                            doveAppare.Clear();
                        }
                        numeroVolteAppare = doveAppare.Count;
                        if (numeroVolteAppare > 0)
                        {
                            if (primoNodo)
                            {
                                nodo = tvRisultati.Nodes.Add(Principale.LocRM.GetString("InformationCollection"));
                                primoNodo = false;
                            }
                            nodo2 = nodo.Nodes.Add(versione + " (" + numeroVolteAppare.ToString(CultureInfo.CurrentCulture) + ")");
                            nodo2.Tag = "ricerca \"" + radice + "\" da \"" + versione + "\"";
                            foreach (string nota in doveAppare.Note)
                            {
                                if (nota.StartsWith("#", StringComparison.Ordinal))
                                    nodo3 = nodo2.Nodes.Add(Principale.testi.ConvertiTitoloNotaARiferimento(nota));
                                else
                                    nodo3 = nodo2.Nodes.Add(nota);
                                nodo3.Tag = "nota \"" + nota + "\" da \"" + versione + "\"";
                            }
                        }
                    }

                    // tema in un collegamento ipertestuale da un'immagine
                    Collection<string> immagini = Principale.testi.Immagini(testo);
                    if (immagini.Count > 0)
                    {
                        nodo = tvRisultati.Nodes.Add(Principale.LocRM.GetString("InformationImage"));
                        string immaginePrecedente = "-";
                        foreach (string immagine in immagini)
                        {
                            if (immagine != immaginePrecedente)
                            {
                                // per non ripetere un'immagine, quando un tema ci appare 2 volte
                                nodo2 = nodo.Nodes.Add(Path.GetFileNameWithoutExtension(immagine));
                                nodo2.Tag = "apri \"" + immagine + "\"";
                            }
                            immaginePrecedente = immagine;
                        }
                    }

                    // tema nel testo di un segnalibro
                    int numeroFileSegnalibri = genitore.bookmarksToolStripMenuItem.DropDownItems.Count;
                    string testoMinuscolo = testo.ToLowerInvariant();
                    nodo = tvRisultati.Nodes.Add(Principale.LocRM.GetString("InformationBookmarks"));
                    for (int i = 4; i < numeroFileSegnalibri - 2; ++i) // per non mettere veloci, "Capitoli" e "Modifica"
                    {
                        ControllaSegnalibri(nodo, genitore.bookmarksToolStripMenuItem.DropDownItems[i], testoMinuscolo, false);
                    }
                    if (nodo.Nodes.Count == 0) // non ci sono segnalibri con il testo
                        nodo.Remove();

                    #endregion
                }

                primoNodo = true;
                bool primoNodoInCategoria;
                int numeroCollegamenti = genitore.externalLinkStripMenuItem.DropDownItems.Count - 2;
                ToolStripMenuItem voceMenu;
                nodo2 = null;
                string url, parametri;
                LaParola.InfoCollegamento collegamento;
                string testoPerLink = testo;
                if (tipoLink == "Riferimento")
                { // converti il riferimento al formato delle note, che è usato da CostruisciCollegamento
                    string testoComeNota = Principale.testi.ConvertiRiferimento(testo).ComeNotaPrimoRiferimento();
                    if (string.IsNullOrEmpty(testoComeNota))
                        testoPerLink = "";
                    else
                        testoPerLink = testoComeNota.Substring(1, 8);
                }
                if (!string.IsNullOrEmpty(testoPerLink))
                {
                    for (int i = 0; i < numeroCollegamenti; ++i)
                    {
                        voceMenu = (ToolStripMenuItem)(genitore.externalLinkStripMenuItem.DropDownItems[i]);
                        if (voceMenu.DropDownItems.Count == 0)
                        { // è una voce normale, non una categoria
                            if (primoNodo)
                            {
                                nodo = tvRisultati.Nodes.Add(Principale.LocRM.GetString("InformationLinks"));
                                primoNodo = false;
                            }
                            collegamento = ((LaParola.InfoCollegamento)(voceMenu.Tag));
                            if ((collegamento.tipo == LaParola.CollegamentoTipo.Riferimento && tipoLink == "Riferimento")
                                || (collegamento.tipo == LaParola.CollegamentoTipo.Parola && tipoLink == "Tema"))
                            {
                                nodo2 = nodo.Nodes.Add(voceMenu.Text);
                                string[] urlConParametri = Principale.CostruisciCollegamento(collegamento.url, collegamento.parametri, testoPerLink, collegamento.mappa, collegamento.tipo);
                                url = urlConParametri[0];
                                parametri = urlConParametri[1];
                                nodo2.Tag = "link \"" + url + "\" \"" + parametri + "\"";
                            }
                        }
                        else
                        {
                            primoNodoInCategoria = true;
                            foreach (ToolStripItem voceInCategoria in voceMenu.DropDownItems)
                            {
                                collegamento = ((LaParola.InfoCollegamento)(voceInCategoria.Tag));
                                if ((collegamento.tipo == LaParola.CollegamentoTipo.Riferimento && tipoLink == "Riferimento")
                                    || (collegamento.tipo == LaParola.CollegamentoTipo.Parola && tipoLink == "Tema"))
                                {
                                    if (primoNodoInCategoria)
                                    {
                                        if (primoNodo)
                                        {
                                            nodo = tvRisultati.Nodes.Add(Principale.LocRM.GetString("InformationLinks"));
                                            primoNodo = false;
                                        }
                                        nodo2 = nodo.Nodes.Add(voceMenu.Text);
                                        primoNodoInCategoria = false;
                                    }
                                    nodo3 = nodo2.Nodes.Add(voceInCategoria.Text);
                                    string[] urlConParametri = Principale.CostruisciCollegamento(collegamento.url, collegamento.parametri, testoPerLink, collegamento.mappa, collegamento.tipo);
                                    url = urlConParametri[0];
                                    parametri = urlConParametri[1];
                                    nodo3.Tag = "link \"" + url + "\" \"" + parametri + "\"";
                                }
                            }
                        }
                    }
                }

                tvRisultati.EndUpdate();
                tvRisultati.Visible = true;

                if (cbInfo.Items.IndexOf(testo) > -1)
                    cbInfo.Items.Remove(testo);
                cbInfo.Items.Insert(0, testo);
                //                cbInfo.Text = "";
                //                pulRiferimento.Enabled = false;
                //                pulTema.Enabled = false;
                cbInfo.AutoCompleteMode = AutoCompleteMode.None;
                // per costringere il combo box a rileggere i valori che possono essere usati per l'autocomplete
                cbInfo.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cbInfo.Text = testo;
                cbInfo.SelectAll();
            }
            finally
            {
                Cursor.Current = cursoreAttuale;
                if (cursoreAttuale != null)
                    cursoreAttuale.Dispose();
            }
        }

        private void ControllaSegnalibri(TreeNode nodo, ToolStripItem menuVoce, string testoDaRicercare, bool aggiungiNodo)
        {
            TreeNode sottoNodo = null;
            bool aggiungiSottoNodi = aggiungiNodo;
            if (aggiungiNodo || menuVoce.Text.ToLowerInvariant().Contains(testoDaRicercare))
            {
                aggiungiSottoNodi = true;
                sottoNodo = nodo.Nodes.Add(menuVoce.Text);
                string riferimento = menuVoce.Tag.ToString();
                if (!string.IsNullOrEmpty(riferimento))
                {
                    try
                    {
                        sottoNodo.Tag = "visualizza \"" + Principale.testi.NormalizzaRiferimentoSegnalibro(riferimento) + "\"";
                    }
                    catch { }
                }
            }

            ToolStripItemCollection sottoVoci = ((ToolStripMenuItem)(menuVoce)).DropDownItems;
            if (sottoVoci.Count > 0)
            {
                if (sottoNodo == null)
                    sottoNodo = nodo.Nodes.Add(menuVoce.Text);
                foreach (ToolStripItem sottoVoce in sottoVoci)
                    ControllaSegnalibri(sottoNodo, sottoVoce, testoDaRicercare, aggiungiSottoNodi);
            }
            if (sottoNodo != null && sottoNodo.Nodes.Count == 0 && !aggiungiSottoNodi)
                sottoNodo.Remove();
        }

        private void tvRisultati_DoubleClick(object sender, EventArgs e)
        {
            if (tvRisultati.SelectedNode.Level > 0 && tvRisultati.SelectedNode.Tag != null)
                genitore.EseguiComando(tvRisultati.SelectedNode.Tag.ToString(), true);
        }

        private void Informazioni_Load(object sender, EventArgs e)
        {

        }
    }
}