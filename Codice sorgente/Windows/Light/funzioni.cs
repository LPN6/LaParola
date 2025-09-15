using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using TestiBiblici;

namespace LaParola
{
    #region altre classi

    public struct FileDaAggiornare
    {
        internal string tipo;
        internal string nome;
        internal string componente;
        internal string versioneNuova;
        internal string versioneAttuale;
        internal Collection<string> url;
        internal string dimensione;
        internal string nomeFile;
    }

    class InfoBranoParallelo
    {
        internal string titolo;
        internal Collection<string> brani = new Collection<string>();
    }

    public class InfoBraniParalleli
    {
        internal string nome;
        internal string versione;
        internal string nomeFile;
        internal Collection<string> nomiColonne = new Collection<string>();
        internal Collection<InfoBranoParallelo> braniParalleli = new Collection<InfoBranoParallelo>();
    }

    struct InfoLettura
    {
        internal string nome;
        internal string versione;
        internal string nomeFile;
    }

    public struct CollegamentoMappaVoce
    {
        internal string inizio;
        internal string fine;
        internal string pagina;
    }

    public enum CollegamentoTipo
    {
        Riferimento,
        Parola
    }

    struct InfoCollegamento
    {
        internal string nomeFile;
        internal string descrizione;
        internal string categoria;
        internal string versione;
        internal string url;
        internal Collection<CollegamentoMappaVoce> mappa;
        internal string parametri;
        internal CollegamentoTipo tipo;
        internal string lingua;
        internal string immagine;
        internal string scorciatoia;
    }

    struct InfoDisposizione
    {
        internal string nome;
        internal string nomeFile;
    }

    public class BarraConEtichetta
    {
        private ToolStripProgressBar barra;
        private ToolStripStatusLabel etichetta;
        private ToolStripStatusLabel etichettaMessaggio;

        public int Valore
        {
            get { return barra.Value; }
            set
            {
                barra.Value = value;
            }
        }

        public int Massimo
        {
            get { return barra.Maximum; }
            set { barra.Maximum = value; }
        }

        public void Aumenta(int value)
        {
            barra.Increment(value);
        }

        public BarraConEtichetta(string messaggio, int minimo, int massimo, ToolStripStatusLabel etichettaPerMessaggioCompleto)
        {
            ToolStripProgressBar pb = new ToolStripProgressBar();
            ToolStripStatusLabel eti = new ToolStripStatusLabel(messaggio);
            pb.Minimum = minimo;
            pb.Maximum = massimo;

            barra = pb;
            etichetta = eti;
            etichettaMessaggio = etichettaPerMessaggioCompleto;
        }

        public void MettiInStatusStrip(ToolStrip statusStrip)
        {
            etichetta.Owner = statusStrip;
            barra.Owner = statusStrip;
        }

        public void MessaggioCompleto(string messaggio)
        {
            etichettaMessaggio.Text = messaggio;
        }

        public void Chiudi()
        {
            barra.Visible = false;
            etichetta.Visible = false;
        }

        public void Ridisegna()
        {
            barra.ProgressBar.Invalidate();
            barra.ProgressBar.Update();
        }
    }

    public enum EsportoTestoTipo
    {
        BibbiaFile,
        BibbiaOsis,
        CollezioneFile,
        CollezioneUnico
    }

    struct ThreadEsportaArgomenti
    {
        public EsportoTestoTipo tipo;
        public string directoryBase;
        public string nomeVersione;
        public BarraConEtichetta barra;
    }

    #endregion

    static class funzioni
    {
        #region Generali

        /// <summary>
        /// Aggiunge degli zero all'inizio di una stringa, fino a quando ha una certa lunghezza.
        /// </summary>
        /// <param name="stringa">La stringa a cui aggiungere gli zero.</param>
        /// <param name="lunghezza">La lunghezza desiderata.</param>
        /// <returns>La stringa con gli zero addizionali.</returns>
        internal static string AggiungiZero(string stringa, int lunghezza)
        {
            string s1 = new String('0', lunghezza) + stringa;
            return s1.Substring(s1.Length - lunghezza);
        }

        /// <summary>
        /// Aggiunge degli zero all'inizio di un numero, fino a quando ha una certa lunghezza.
        /// </summary>
        /// <param name="numero">Il numero a cui aggiungere gli zero.</param>
        /// <param name="lunghezza">La lunghezza desiderata.</param>
        /// <returns>La stringa con gli zero addizionali.</returns>
        internal static string AggiungiZero(int numero, int lunghezza)
        {
            return AggiungiZero(numero.ToString(CultureInfo.InvariantCulture), lunghezza);
        }

        internal static string VersioneMinore2Cifre(string v)
        {
            int p = v.IndexOf('.');
            if (p > 0 && p < v.Length - 3 && v[p + 2] == '.')
                v = v.Insert(p + 1, "0");
            return v;
        }

        internal static bool IsLettera(char c)
        { // anche in testi.cs
            return (Char.IsLetter(c) || Char.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark || (c >= '\u02d2' && c <= '\u02d3')); // gli ultimi caratteri sono usati nella traslitterazione dell'ebraico
        }

        internal static bool IsLetteraGreca(char c)
        { // anche in testi.cs
            return ((c >= '\u0370' && c <= '\u03ff') || (c >= '\u1f00' && c <= '\u1fff'));
        }

        /// <summary>
        /// Data una stringa con diverse lingue separate da una riga verticale |, restituisce la prima
        /// </summary>
        /// <param name="lingua">Un elenco di lingue separate da una riga verticale.</param>
        /// <returns>La lingua principale.</returns>
        internal static string LinguaPrincipale(string lingua)
        { // anche in testi.cs, Light
            if (!string.IsNullOrEmpty(lingua))
                return lingua.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)[0].ToLowerInvariant();
            else
                return "";
        }

        internal static bool RightToLeft(string lingua)
        { // anche in testi.cs
            string linguaPrincipale = LinguaPrincipale(lingua);
            return (linguaPrincipale == "he" || linguaPrincipale == "ar");
        }

        #endregion

        #region Radici

        /// <summary>
        /// Aggiungi le radici delle parole ad un testo.
        /// </summary>
        /// <param name="cartellaDeiFile">La cartella in cui si trova i file *.parole_radici con contengono le radici.</param>
        /// <param name="linguePreferite">La lingua normale del testo (radici in questa lingua hanno preferenza sulle altre).</param>
        /// <param name="elencoParoleInVersione">Tutte le parole nel testo.</param>
        /// <param name="listaRadici">Una lista di tutte le radici usate.</param>
        /// <returns>La radice di ogni parola nel testo.</returns>
        public static string[] AggiungiRadiciDaFile(string cartellaDeiFile, string linguePreferite, string[] elencoParoleInVersione, List<string> listaRadici)
        {
            ConfrontoCI confrontoParole = new ConfrontoCI();
            string[] fileParoleRadici = Directory.GetFiles(cartellaDeiFile, "*.parole_radici");
            // i file devono essere UTF-8
            foreach (string linguaPreferita in linguePreferite.ToUpperInvariant().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (linguaPreferita.Length >= 2)
                {
                    // far sì che la lingua del testo abbia precedenza sulle altre lingue presenti nel testo
                    bool trovato = false;
                    for (int i = 0; i < fileParoleRadici.Length; ++i)
                    {
                        if (fileParoleRadici[i].EndsWith(Path.DirectorySeparatorChar + linguaPreferita + ".parole_radici", StringComparison.InvariantCultureIgnoreCase))
                        {
                            string temp = fileParoleRadici[i];
                            fileParoleRadici[i] = fileParoleRadici[0];
                            fileParoleRadici[0] = temp;
                            trovato = true;
                            break;
                        }
                    }
                    if (trovato)
                    {
                        trovato = false;
                        for (int i = 1; i < fileParoleRadici.Length; ++i)
                        {
                            if (fileParoleRadici[i].EndsWith(Path.DirectorySeparatorChar + linguaPreferita + "1.parole_radici", StringComparison.InvariantCultureIgnoreCase))
                            {
                                string temp = fileParoleRadici[i];
                                fileParoleRadici[i] = fileParoleRadici[1];
                                fileParoleRadici[1] = temp;
                                trovato = true;
                                break;
                            }
                        }
                    }
                    if (trovato)
                    {
                        for (int i = 2; i < fileParoleRadici.Length; ++i)
                        {
                            if (fileParoleRadici[i].EndsWith(Path.DirectorySeparatorChar + linguaPreferita + "2.parole_radici", StringComparison.InvariantCultureIgnoreCase))
                            {
                                string temp = fileParoleRadici[i];
                                fileParoleRadici[i] = fileParoleRadici[2];
                                fileParoleRadici[2] = temp;
                                break;
                            }
                        }
                    }
                }
            }
            string[] paroleRadici = new string[] { };
            foreach (string fileConParoleRadici in fileParoleRadici)
            {
                string[] paroleRadiciInFile = File.ReadAllLines(fileConParoleRadici);
                Array.Resize(ref paroleRadici, paroleRadici.Length + paroleRadiciInFile.Length);
                Array.Copy(paroleRadiciInFile, 0, paroleRadici, paroleRadici.Length - paroleRadiciInFile.Length, paroleRadiciInFile.Length);
            }

            string[] radiceDiParola = new string[elencoParoleInVersione.Length];

            int indiceRadice, numeroParola;
            string[] parolaRadice = { "", "" };
            foreach (string s in paroleRadici)
            {
                try
                {
                    parolaRadice = s.Split('=');
                    numeroParola = Array.BinarySearch(elencoParoleInVersione, parolaRadice[0], confrontoParole);
                    // se parola è usata nel testo, e non già data una radice (perché multipli file possono dare radici diverse alla stessa parola)
                    //      dà la radice alla parola
                    if (numeroParola >= 0 && string.IsNullOrEmpty(radiceDiParola[numeroParola]))
                        radiceDiParola[numeroParola] = parolaRadice[1];
                }
                catch
                {
                    // problema con una riga in formato sbagliato (cioè senza =) - basta saltare la riga
                }
            }

            // creare la lista di tutte le radici utilizzate
            listaRadici.Add("*"); // quando una parola non ha radice
            bool esisteUnaParolaSenzaRadice = false;
            int numeroParole = radiceDiParola.Length;
            for (int i = 0; i < numeroParole; ++i)
            {
                if (String.IsNullOrEmpty(radiceDiParola[i]))
                    radiceDiParola[i] = "*";
                if (radiceDiParola[i] == "*")
                    esisteUnaParolaSenzaRadice = true;

                indiceRadice = listaRadici.BinarySearch(radiceDiParola[i], confrontoParole);
                if (indiceRadice < 0)
                    listaRadici.Insert(~indiceRadice, radiceDiParola[i]);
            }
            if (!esisteUnaParolaSenzaRadice)
                listaRadici.RemoveAt(0);

            return radiceDiParola;
        }

        #endregion

        #region ApriBrowser

        /// <summary>
        /// Apre un indirizzo Internet nel browser predefinito.
        /// </summary>
        /// <param name="url">L'indirizzo da aprire.</param>
        /// <param name="throwException">Se un eventuale errore è passato a chi ha chiamato questa routine, oppure è ignorato.</param>
        internal static void ApriBrowser(Uri url, bool throwException)
        {
            if (url == null)
                throw new ArgumentNullException("url");
            else
                ApriBrowser(url.ToString(), "", throwException);
        }

        /// <summary>
        /// Apre un indirizzo Internet nel browser predefinito.
        /// </summary>
        /// <param name="url">L'indirizzo da aprire.</param>
        /// <param name="throwException">Se un eventuale errore è passato a chi ha chiamato questa routine, oppure è ignorato.</param>
        internal static void ApriBrowser(string url, bool throwException)
        {
            ApriBrowser(url, "", throwException);
        }

        /// <summary>
        /// Apre un indirizzo Internet nel browser predefinito.
        /// </summary>
        /// <param name="url">L'indirizzo da aprire.</param>
        /// <param name="parametri">Gli eventuali parametri dell'indirizzo.</param>
        /// <param name="throwException">Se un eventuale errore è passato a chi ha chiamato questa routine, oppure è ignorato.</param>
        internal static void ApriBrowser(Uri url, string parametri, bool throwException)
        {
            if (url == null)
                throw new ArgumentNullException("url");
            else
                ApriBrowser(url.ToString(), parametri, throwException);
        }

        /// <summary>
        /// Apre un indirizzo Internet nel browser predefinito.
        /// </summary>
        /// <param name="url">L'indirizzo da aprire.</param>
        /// <param name="parametri">Gli eventuali parametri dell'indirizzo.</param>
        /// <param name="throwException">Se un eventuale errore è passato a chi ha chiamato questa routine, oppure è ignorato.</param>
        internal static void ApriBrowser(string url, string parametri, bool throwException)
        {
            Cursor cursoreAttuale = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.AppStarting;
                if (string.IsNullOrEmpty(parametri))
                    System.Diagnostics.Process.Start(url);
                else
                    System.Diagnostics.Process.Start(url, parametri);
            }
            catch (Exception)
            {
                if (throwException)
                    throw;
            }
            finally
            {
                Cursor.Current = cursoreAttuale;
            }
        }

        #endregion

        #region Esporta testi

        static internal void EsportaTesto(Principale genitore, EsportoTestoTipo tipo, string nomeVersione)
        {
            EsportaTesto(genitore, tipo, nomeVersione, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar);
        }

        static internal void EsportaTesto(Principale genitore, EsportoTestoTipo tipo, string nomeVersione, string directoryBase)
        {
            if (!string.IsNullOrEmpty(directoryBase))
            {
                if (directoryBase[directoryBase.Length - 1] != Path.DirectorySeparatorChar)
                    directoryBase += Path.DirectorySeparatorChar;
            }
            else
            {
                EsportaTesto(genitore, tipo, nomeVersione);
                return;
            }

            ThreadEsportaArgomenti argomenti = new ThreadEsportaArgomenti();

            if (tipo != EsportoTestoTipo.CollezioneUnico)
            {
                if (tipo == EsportoTestoTipo.BibbiaFile || tipo == EsportoTestoTipo.BibbiaOsis)
                    argomenti.barra = genitore.CreaBarraDiStato(Principale.LocRM.GetString("ExportCurrent"), 0, 76);
                else
                    argomenti.barra = genitore.CreaBarraDiStato(Principale.LocRM.GetString("ExportCurrent"), 0, Principale.testi.Note(nomeVersione).Count + 3);
                argomenti.nomeVersione = nomeVersione;
                argomenti.directoryBase = directoryBase;
                argomenti.tipo = tipo;

                BackgroundWorker backgroundWorker = new BackgroundWorker();
                backgroundWorker.WorkerReportsProgress = true;
                backgroundWorker.WorkerSupportsCancellation = true;
                backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(funzioni.EsportaTestoProgress);

                if (Principale.isRunningOnMono)
                {
                    funzioni.EsportaTestoInThread(backgroundWorker, new DoWorkEventArgs(argomenti));
                    funzioni.TestoEsportato(backgroundWorker, new RunWorkerCompletedEventArgs(argomenti, null, false));
                }
                else
                {
                    backgroundWorker.DoWork += new DoWorkEventHandler(funzioni.EsportaTestoInThread);
                    backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(funzioni.TestoEsportato);
                    backgroundWorker.RunWorkerAsync(argomenti);
                }
            }
            else
            // per collezione in unico file, il thread è creatato dalla routine per mostrare un brano
            {
                genitore.MostraBranoInEditor(funzioni.FileUnicoCollezione(nomeVersione), nomeVersione);
            }
        }

        static public Riferimento FileUnicoCollezione(string nomeVersione)
        {
            Riferimento titoli = new Riferimento(false);
            if (!String.IsNullOrEmpty(nomeVersione))
            {
                Collection<string> noteInOrdine = Principale.testi.GetNoteInOrdine(nomeVersione);
                List<string> note = new List<string>(Principale.testi.Note(nomeVersione));
                ConfrontoCI confronto = new ConfrontoCI();
                note.Sort(confronto);

                // aggiungere prima le note in ordine, poi le altre note in ordine alfabetico
                int indiceNota;
                string notaSenzaTab;
                char[] trimTab = { '\t' };

                foreach (string nota in noteInOrdine)
                {
                    if (!string.IsNullOrEmpty(nota))
                    {
                        notaSenzaTab = nota.TrimStart(trimTab); // possono essere note dalle note in ordine, ma senza l'indentazione (indicata da una tabulazione) rimossa
                        titoli.AggiungiNotaEParole(notaSenzaTab, new Collection<UInt16>());
                        indiceNota = note.BinarySearch(notaSenzaTab, confronto);
                        if (indiceNota > -1)
                            note.RemoveAt(indiceNota);
                    }
                }

                foreach (string nota in note)
                    titoli.AggiungiNotaEParole(nota, new Collection<UInt16>());
            }
            return titoli;
        }

        static private void EsportaTestoInThread(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            ThreadEsportaArgomenti argomenti = (ThreadEsportaArgomenti)e.Argument;
            string directoryBase = argomenti.directoryBase;
            string nomeVersione = argomenti.nomeVersione;

            int capitoliInLibro, versettiInCapitolo, inizioVersetto;
            string testo;
            Collection<string> listaVersione = new Collection<string>();
            listaVersione.Add(nomeVersione);
            Collection<string> listaCollezioni = new Collection<string>();

            try
            {
                if (argomenti.tipo == EsportoTestoTipo.BibbiaFile || argomenti.tipo == EsportoTestoTipo.CollezioneFile)
                {
                    directoryBase += nomeVersione + Path.DirectorySeparatorChar;
                    Directory.CreateDirectory(directoryBase);
                    string[] fileInDirectory = Directory.GetFiles(directoryBase, "*.*");
                    foreach (string fileDaCancellare in fileInDirectory)
                        File.Delete(fileDaCancellare);
                }

                switch (argomenti.tipo)
                {
                    #region Bibbia file di testo
                    case EsportoTestoTipo.BibbiaFile:
                        List<string> righe = new List<string>(3000);
                        RichTextBoxEx rtb = new RichTextBoxEx();

                        FormatoTesto formatoVecchio = Principale.testi.Formato;
                        if (argomenti.tipo == EsportoTestoTipo.BibbiaFile || argomenti.tipo == EsportoTestoTipo.BibbiaOsis)
                        {
                            FormatoTesto formatoPerEsporto = new FormatoTesto();
                            formatoVecchio.CopiaA(formatoPerEsporto);
                            formatoPerEsporto.RiferimentoFormato = RiferimentoFormato.Nessuno;
                            formatoPerEsporto.TestoVisualizzato = TestoVisualizzato.Versetti;
                            Principale.testi.Formato = formatoPerEsporto;
                        }

                        try
                        {
                            for (byte iLibro = 1; iLibro <= 73; ++iLibro)
                            {
                                righe.Clear();
                                capitoliInLibro = Principale.testi.CapitoliInLibro(iLibro, nomeVersione);
                                for (byte iCapitolo = 1; iCapitolo <= capitoliInLibro; ++iCapitolo)
                                {
                                    rtb.Rtf = Principale.testi.TestoBrano(new Riferimento(new byte[] { iLibro, iCapitolo, 1, iLibro, iCapitolo, 255 }), listaVersione, listaCollezioni, null, null);
                                    testo = rtb.Text;
                                    testo = testo.Replace("\n", "|");
                                    while (testo.Contains("\x01"))
                                    {
                                        inizioVersetto = testo.IndexOf("\x01");
                                        testo = testo.Remove(inizioVersetto, 9);
                                        if (inizioVersetto > 0 && testo[inizioVersetto - 1] == '|')
                                            testo = testo.Substring(0, inizioVersetto - 1) + "\r\n" + testo.Substring(inizioVersetto);
                                    }
                                    righe.Add(testo.Replace("\r\n\r\n", "\r\n.\r\n")); // versetto mancante
                                    righe.Add(""); // riga vuota fra capitoli
                                }
                                if (righe.Count > 0)
                                {
                                    righe.RemoveAt(righe.Count - 1); // la riga addizionale dopo l'ultimo capitolo non serve
                                    File.WriteAllLines(directoryBase + Principale.testi.GetLibroNome(iLibro) + ".txt", righe.ToArray(), Encoding.UTF8);
                                }
                                worker.ReportProgress(iLibro, e);
                            }
                        }
                        finally
                        {
                            Principale.testi.Formato = formatoVecchio;
                        }
                        break;
                    #endregion
                    #region Bibbia OSIS
                    case EsportoTestoTipo.BibbiaOsis:
                        VersioneInformazioni info = Principale.testi.Info(nomeVersione);
                        string[] libriNomiOSIS = new string[]{"Gen", "Exod", "Lev", "Num", "Deut",
         "Josh", "Judg", "Ruth", "1Sam", "2Sam", "1Kgs", "2Kgs", "1Chr", "2Chr", "Ezra", "Neh", "Tob", "Jdt", "Esth", "1Macc", "2Macc",
         "Job", "Ps", "Prov", "Eccl", "Song", "Wis", "Sir",
         "Isa", "Jer", "Lam", "Bar", "Ezek", "Dan",
         "Hos", "Joel", "Amos", "Obad", "Jonah", "Mic", "Nah", "Hab", "Zeph", "Hag", "Zech", "Mal",
         "Matt", "Mark", "Luke", "John", "Acts",
         "Rom", "1Cor", "2Cor", "Gal", "Eph", "Phil", "Col", "1Thess", "2Thess", "1Tim", "2Tim", "Titus", "Phlm",
         "Heb", "Jas", "1Pet", "2Pet", "1John", "2John", "3John", "Jude", "Rev"};

                        List<string> righeOsis = new List<string>(32768);
                        righeOsis.Add("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
                        righeOsis.Add("<osis xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"http://www.bibletechnologies.net/osisCore.2.0.1.xsd\">");
                        righeOsis.Add("<osisText osisIDWork=\"" + info.Abbreviazione + "\" osisRefWork=\"Bible\">");
                        righeOsis.Add("<header>");
                        righeOsis.Add("  <work osisWork=\"" + info.Abbreviazione + "\">");
                        if (!string.IsNullOrEmpty(info.Titolo))
                            righeOsis.Add("    <title>" + info.Titolo + "</title>");
                        righeOsis.Add("    <identifier type=\"OSIS\">Bible." + info.Abbreviazione + "</identifier>");
                        if (!string.IsNullOrEmpty(info.Lingua))
                            righeOsis.Add("    <language type=\"ISO-639\">" + info.Lingua + "</language>");
                        righeOsis.Add("    <refSystem>Bible</refSystem>");
                        righeOsis.Add("    <creator>LaParola.Net</creator>");
                        if (!string.IsNullOrEmpty(info.CasaEditrice))
                            righeOsis.Add("    <publisher>" + info.CasaEditrice + "</publisher>");
                        if (!string.IsNullOrEmpty(info.Data))
                            righeOsis.Add("    <date type=\"original\">" + info.Data + "</date>");
                        righeOsis.Add("    <date type=\"eversion\">" + DateTime.Now.Year + "</date>");
                        if (!string.IsNullOrEmpty(info.Isbn))
                            righeOsis.Add("    <identifier type=\"ISBN\">" + info.Isbn + "</identifier>");
                        if (!string.IsNullOrEmpty(info.Copyright))
                            righeOsis.Add("    <rights type=\"copyright\">" + info.Copyright + "</rights>");
                        if (!string.IsNullOrEmpty(info.Descrizione))
                            righeOsis.Add("    <description>" + info.Descrizione + "</description>");
                        righeOsis.Add("  </work>");
                        righeOsis.Add("  <work osisWork=\"Bible\">");
                        righeOsis.Add("    <refSystem>Bible</refSystem>");
                        righeOsis.Add("  </work>");
                        righeOsis.Add("</header>");
                        righeOsis.Add("<p sID=\"1\" /><div type=\"testament\">");
                        StringBuilder rigaOsis = new StringBuilder(512);
                        StringBuilder paragrafo = new StringBuilder(32);
                        int numeroParagrafo = 1, fineTag, inizioTag;
                        //int indiceCarattereHex;

                        for (byte iLibro = 1; iLibro <= 73; ++iLibro)
                        {
                            capitoliInLibro = Principale.testi.CapitoliInLibro(iLibro, nomeVersione);
                            if (capitoliInLibro > 0)
                                righeOsis.Add("<div type=\"book\" osisID=\"" + libriNomiOSIS[iLibro - 1] + "\">");
                            for (byte iCapitolo = 1; iCapitolo <= capitoliInLibro; ++iCapitolo)
                            {
                                versettiInCapitolo = Principale.testi.VersettiInCapitolo(iLibro, iCapitolo, nomeVersione);
                                rigaOsis.Length = 0;
                                righeOsis.Add(rigaOsis.Append("  <chapter osisID=\"").Append(libriNomiOSIS[iLibro - 1]).Append(".").Append(iCapitolo).Append("\">").ToString());
                                for (byte iVersetto = 1; iVersetto <= versettiInCapitolo; ++iVersetto)
                                {
                                    testo = Principale.testi.TestoVersettoRaw(iLibro, iCapitolo, iVersetto, nomeVersione);
                                    /*
                                     * non necessario
                                    while (testo.IndexOf(@"\'") >= 0)
                                    {
                                        indiceCarattereHex = testo.IndexOf(@"\'");
                                        testo = testo.Substring(0, indiceCarattereHex) + "&#" + (Uri.FromHex(testo[indiceCarattereHex + 2]) * 16 + Uri.FromHex(testo[indiceCarattereHex + 3])) + ";" + testo.Substring(indiceCarattereHex + 4);
                                    }*/
                                    while (testo.IndexOf(@"\par") >= 0)
                                    {
                                        paragrafo.Length = 0;
                                        testo = paragrafo.Append(testo.Substring(0, testo.IndexOf(@"\par"))).Append("<p eID=\"").Append(numeroParagrafo).Append("\" /><p sID=\"").Append(numeroParagrafo + 1).Append("\" />").Append(testo.Substring(testo.IndexOf(@"\par") + 4)).ToString();
                                        ++numeroParagrafo;
                                    }

                                    while (testo.IndexOf(@" {\super ") >= 0)
                                    { // i numeri Strong non sono esportati
                                        inizioTag = testo.IndexOf(@" {\super ");
                                        fineTag = testo.IndexOf('}', inizioTag);
                                        testo = testo.Substring(0, inizioTag) + testo.Substring(fineTag + 1);
                                    }

                                    while (testo.IndexOf(@"\lptit1 ") >= 0)
                                        testo = testo.Substring(0, testo.IndexOf(@"\lptit1 ")) + "<head>" + testo.Substring(testo.IndexOf(@"\lptit1 ") + 8);
                                    while (testo.IndexOf(@"\lptit0 ") >= 0)
                                        testo = testo.Substring(0, testo.IndexOf(@"\lptit0 ")) + "</head>" + testo.Substring(testo.IndexOf(@"\lptit0 ") + 8);
                                    // nota: durante l'importazione, il nuovo paragrafo è messo dentro le tag \lptit, anche se nel file OSIS originale era dopo la chiusura
                                    // quindi il file esportato sarà leggermente diverso
                                    while (testo.IndexOf(@"{\i1 ") >= 0)
                                    {
                                        inizioTag = testo.IndexOf(@"{\i1 ");
                                        fineTag = testo.IndexOf('}', inizioTag);
                                        testo = testo.Substring(0, inizioTag) + "<q>" + testo.Substring(inizioTag + 5, fineTag - inizioTag - 5) + "</q>" + testo.Substring(fineTag + 1);
                                    }
                                    while (testo.IndexOf(@"{\b1 ") >= 0) // deve essere dopo "super", nel caso di parole con super dentro il titolo
                                    {
                                        inizioTag = testo.IndexOf(@"{\b1 ");
                                        fineTag = testo.IndexOf('}', inizioTag);
                                        testo = testo.Substring(0, inizioTag) + "<title>" + testo.Substring(inizioTag + 5, fineTag - inizioTag - 5) + "</title>" + testo.Substring(fineTag + 1);
                                    }
                                    while (testo.IndexOf(@"{\caps ") >= 0)
                                    {
                                        inizioTag = testo.IndexOf(@"{\caps ");
                                        fineTag = testo.IndexOf('}', inizioTag);
                                        testo = testo.Substring(0, inizioTag) + "<divineName>" + testo.Substring(inizioTag + 7, fineTag - inizioTag - 7) + "</divineName>" + testo.Substring(fineTag + 1);
                                    }
                                    /*                                    while (testo.IndexOf(@"{\qr ") >= 0)
                                                                        {
                                                                            inizioTag = testo.IndexOf(@"{\qr ");
                                                                            fineTag = testo.IndexOf('}', inizioTag);
                                                                            testo = testo.Substring(0, inizioTag) + testo.Substring(inizioTag + 5, fineTag - inizioTag - 5) + testo.Substring(fineTag + 1);
                                                                        }*/
                                    if (testo.IndexOf('{') >= 0 || testo.IndexOf('}') >= 0 || testo.IndexOf('\\') >= 0)
                                        inizioTag = 0;
                                    rigaOsis.Length = 0;
                                    if (!string.IsNullOrEmpty(testo))
                                        righeOsis.Add(rigaOsis.Append("    <verse osisID=\"").Append(libriNomiOSIS[iLibro - 1]).Append(".").Append(iCapitolo).Append(".").Append(iVersetto).Append("\">").Append(testo.Trim()).Append("</verse>").ToString());
                                }
                                righeOsis.Add("  </chapter>");
                            }
                            if (capitoliInLibro > 0)
                                righeOsis.Add("</div>");
                            if (iLibro == 46)
                            {
                                righeOsis.Add("</div>");
                                righeOsis.Add("<div type=\"testament\">");
                            }
                            worker.ReportProgress(iLibro, e);
                        }

                        righeOsis.Add("</div>");
                        righeOsis.Add("<p eID=\"" + numeroParagrafo + "\" />");
                        righeOsis.Add("</osisText>");
                        righeOsis.Add("</osis>");
                        File.WriteAllLines(directoryBase + nomeVersione + ".xml", righeOsis.ToArray(), Encoding.UTF8);
                        break;
                    #endregion
                    #region Collezione file diversi
                    case EsportoTestoTipo.CollezioneFile:
                        Collection<string> note = Principale.testi.Note(nomeVersione);
                        foreach (string nota in note)
                        {
                            File.WriteAllText(directoryBase + nota.Replace("?", "").Replace(":", "-") + ".rtf", Principale.testi.GetNotaTesto(nota, nomeVersione));
                            worker.ReportProgress(-1, e);
                        }
                        List<string> noteInOrdine = new List<string>(Principale.testi.GetNoteInOrdine(nomeVersione));
                        for (int i = 0; i < noteInOrdine.Count; ++i)
                            noteInOrdine[i] = noteInOrdine[i].Replace("?", "").Replace(":", "-");
                        if (noteInOrdine.Count > 0)
                            File.WriteAllLines(directoryBase + nomeVersione + ".ordine", noteInOrdine.ToArray(), Encoding.UTF8);
                        break;
                    #endregion
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("ManageNotesErrorNotExported"), exc.Message, nomeVersione), Principale.LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions);
            }

            List<string> paroleRadici = new List<string>(Principale.testi.GetParoleRadici(nomeVersione));
            if (paroleRadici.Count > 0)
                File.WriteAllLines(directoryBase + nomeVersione + ".parole_radici", paroleRadici.ToArray(), Encoding.UTF8);
            worker.ReportProgress(-1, e);
            List<string> radiciDiverse = new List<string>(Principale.testi.GetRadiciDiverse(nomeVersione));
            if (radiciDiverse.Count > 0)
                File.WriteAllLines(directoryBase + nomeVersione + ".radici_diverse", radiciDiverse.ToArray(), Encoding.UTF8);
            worker.ReportProgress(-1, e);
            List<string> riferimentiDiversi = new List<string>(Principale.testi.GetRiferimentiDiversi(nomeVersione));
            if (riferimentiDiversi.Count > 0)
                File.WriteAllLines(directoryBase + nomeVersione + ".riferimenti", riferimentiDiversi.ToArray(), Encoding.UTF8);
            worker.ReportProgress(-1, e);

            e.Result = argomenti;
            if (worker.CancellationPending)
                e.Cancel = true;
        }

        static private void EsportaTestoProgress(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                int valore = e.ProgressPercentage;
                if (valore >= 0) // è il valore da impostare
                    ((ThreadEsportaArgomenti)(((DoWorkEventArgs)(e.UserState)).Argument)).barra.Valore = e.ProgressPercentage;
                else // è il negativo dell'aumento
                    ((ThreadEsportaArgomenti)(((DoWorkEventArgs)(e.UserState)).Argument)).barra.Aumenta(-valore);
            }
            catch (NullReferenceException) // succede quando programma è chiuso mentre thread in esecuzione
            {
                ((BackgroundWorker)sender).CancelAsync();
            }
            ((ThreadEsportaArgomenti)(((DoWorkEventArgs)(e.UserState)).Argument)).barra.Ridisegna();
        }

        static private void TestoEsportato(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                return;

            (((ThreadEsportaArgomenti)e.Result).barra).MessaggioCompleto(Principale.LocRM.GetString("ExportCompleted"));
            (((ThreadEsportaArgomenti)e.Result).barra).Chiudi();
            //            genitore.SetBarraDiStatoTesto(Principale.LocRM.GetString("ExportCompleted"));
        }

        #endregion
    }

    internal static class SafeNativeMethods
    {
        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        internal static extern IntPtr SendMessage(IntPtr hWnd, Int32 msg, IntPtr wParam, IntPtr lParam);
    }
}
