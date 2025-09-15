using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using LaParola.Properties;
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
        BibbiaZefania,
        BibbiaJava,
        CollezioneFile,
        CollezioneUnico,
        CollezioneJava
    }

    struct ThreadEsportaArgomenti
    {
        public EsportoTestoTipo tipo;
        public string directoryBase;
        public string nomeVersione;
        public BarraConEtichetta barra;
    }

    #endregion

    static class Funzioni
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
            return (Char.IsLetter(c) || Char.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark || (c >= '\u02be' && c <= '\u02bf')); // gli ultimi caratteri sono usati nella traslitterazione dell'ebraico
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

        internal static string DizionarioDiVersione(string versione)
        {
            string dizionario = "";
            string lingua = Principale.testi.Info(versione).Lingua.ToLowerInvariant();
            if (lingua.Length > 2)
                lingua = lingua.Substring(0, 2);
            switch (lingua)
            {
                case "it":
                    dizionario = Settings.Default.DizionarioItaliano;
                    break;
                case "en":
                    dizionario = Settings.Default.DizionarioInglese;
                    break;
                case "es":
                    dizionario = Settings.Default.DizionarioSpagnolo;
                    break;
                case "el":
                    dizionario = Settings.Default.DizionarioGreco;
                    break;
                case "he":
                case "he-t":
                    dizionario = Settings.Default.DizionarioEbraico;
                    break;
                case "la":
                    dizionario = Settings.Default.DizionarioLatino;
                    break;
            }
            return dizionario;
        }

        /// <summary>
        /// Dato l'inizio di un versetto (del riferimento), trova l'inizio del testo.
        /// </summary>
        /// <param name="testo">Il testo in cui cercare.</param>
        /// <param name="posizioneVersetto">L'indice dell'inizio del versetto in testo.</param>
        /// <returns>L'indice dell'inizio del testo del versetto.</returns>
        internal static int InizioTestoDaInizioRiferimento(string testo, int posizioneVersetto)
        {
            int p = testo.IndexOf(' ', posizioneVersetto);
            if (p > 0)
            {
                if (testo[p - 1] < '0' || testo[p - 1] > '9')
                { // non è solo il numero del versetto, forse c'è anche il libro, quindi cerchiamo il prossimo spazio
                    p = testo.IndexOf(' ', p + 1);
                    if (p > 0)
                    {
                        if (testo[p - 1] < '0' || testo[p - 1] > '9')
                            p = posizioneVersetto - 1; // riferimento non c'è. Forse è testo solo, senza riferimento, quindi torniamo all'inizio.
                    } // altrimenti è tipo Gen 1:1 Nel principio...
                }
            }
            else
                p = posizioneVersetto - 1;
            posizioneVersetto = p + 1; // il primo carattere dopo lo spazio

            if (testo[posizioneVersetto] == RichTextBoxEx.InizioRiferimento)
                posizioneVersetto += 9; // uno per InizioRiferimento, 2 per libro, 3 per capitolo, 3 per versetto
            return posizioneVersetto;
        }

        internal static bool RightToLeft(string lingua)
        { // anche in testi.cs
            string linguaPrincipale = LinguaPrincipale(lingua);
            return (linguaPrincipale == "he" || linguaPrincipale == "ar");
        }

        internal static string RimuoviCaratteriNonValidiInPercorsi(string espressione)
        {
            foreach (char c in Path.GetInvalidPathChars())
                espressione = espressione.Replace(c.ToString(), "");
            return espressione;
        }

        internal static string RimuoviCaratteriNonValidiInXml(string nomeEncoded)
        {
            return nomeEncoded.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
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
                        if (fileParoleRadici[i].EndsWith(Path.DirectorySeparatorChar + linguaPreferita + ".parole_radici", StringComparison.OrdinalIgnoreCase))
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
                            if (fileParoleRadici[i].EndsWith(Path.DirectorySeparatorChar + linguaPreferita + "1.parole_radici", StringComparison.OrdinalIgnoreCase))
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
                            if (fileParoleRadici[i].EndsWith(Path.DirectorySeparatorChar + linguaPreferita + "2.parole_radici", StringComparison.OrdinalIgnoreCase))
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
                if (cursoreAttuale != null)
                    cursoreAttuale.Dispose();
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
                if (tipo == EsportoTestoTipo.BibbiaFile || tipo == EsportoTestoTipo.BibbiaOsis || tipo == EsportoTestoTipo.BibbiaZefania)
                    argomenti.barra = genitore.CreaBarraDiStato(Principale.LocRM.GetString("ExportCurrent"), 0, 76);
                else
                    argomenti.barra = genitore.CreaBarraDiStato(Principale.LocRM.GetString("ExportCurrent"), 0, Principale.testi.Note(nomeVersione).Count + 3);
                argomenti.nomeVersione = nomeVersione;
                argomenti.directoryBase = directoryBase;
                argomenti.tipo = tipo;

                BackgroundWorker backgroundWorker = new BackgroundWorker
                {
                    WorkerReportsProgress = true,
                    WorkerSupportsCancellation = true
                };
                backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(Funzioni.EsportaTestoProgress);

                if (Principale.isRunningOnMono)
                {
                    Funzioni.EsportaTestoInThread(backgroundWorker, new DoWorkEventArgs(argomenti));
                    Funzioni.TestoEsportato(backgroundWorker, new RunWorkerCompletedEventArgs(argomenti, null, false));
                }
                else
                {
                    backgroundWorker.DoWork += new DoWorkEventHandler(Funzioni.EsportaTestoInThread);
                    backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Funzioni.TestoEsportato);
                    backgroundWorker.RunWorkerAsync(argomenti);
                }
            }
            else
            // per collezione in unico file, il thread è creatato dalla routine per mostrare un brano
            {
                genitore.MostraBranoInEditor(Principale.testi.NotePrimaOrdinate(nomeVersione, true), nomeVersione);
            }
        }

        static private void EsportaTestoInThread(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            ThreadEsportaArgomenti argomenti = (ThreadEsportaArgomenti)e.Argument;
            string directoryBase = argomenti.directoryBase;
            string nomeVersione = argomenti.nomeVersione;

            int capitoliInLibro, versettiInCapitolo, inizioVersetto;
            string testo;
            Collection<string> listaVersione = new Collection<string>
            {
                nomeVersione
            };
            Collection<string> listaCollezioni = new Collection<string>();

            FormatoTesto formatoVecchio = Principale.testi.Formato;
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

                if (argomenti.tipo == EsportoTestoTipo.BibbiaFile || argomenti.tipo == EsportoTestoTipo.BibbiaOsis || argomenti.tipo == EsportoTestoTipo.BibbiaZefania)
                {
                    FormatoTesto formatoPerEsporto = new FormatoTesto();
                    formatoVecchio.CopiaA(formatoPerEsporto);
                    formatoPerEsporto.RiferimentoFormato = RiferimentoFormato.Nessuno;
                    formatoPerEsporto.TestoVisualizzato = TestoVisualizzato.Versetti;
                    Principale.testi.Formato = formatoPerEsporto;
                }

                VersioneInformazioni info;
                List<string> righeDaScrivere = new List<string>(32768);
                int fineTag, inizioTag;
                long pInizioDati;
                Encoding utf8 = Encoding.UTF8;
                Encoding unicode = Encoding.BigEndianUnicode;

                switch (argomenti.tipo)
                {
                    #region Bibbia file di testo
                    case EsportoTestoTipo.BibbiaFile:
                        List<string> righe = new List<string>(3000);
                        RichTextBoxEx rtb = new RichTextBoxEx();

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
                                    inizioVersetto = testo.IndexOf("\x01", StringComparison.Ordinal);
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
                                File.WriteAllLines(directoryBase + Principale.testi.GetLibroNome(iLibro) + ".txt", righe.ToArray(), utf8);
                            }
                            worker.ReportProgress(iLibro, e);
                        }
                        break;
                    #endregion
                    #region Bibbia OSIS
                    case EsportoTestoTipo.BibbiaOsis:
                        info = Principale.testi.Info(nomeVersione);
                        string[] libriNomiOSIS = new string[]{"Gen", "Exod", "Lev", "Num", "Deut",
         "Josh", "Judg", "Ruth", "1Sam", "2Sam", "1Kgs", "2Kgs", "1Chr", "2Chr", "Ezra", "Neh", "Tob", "Jdt", "Esth", "1Macc", "2Macc",
         "Job", "Ps", "Prov", "Eccl", "Song", "Wis", "Sir",
         "Isa", "Jer", "Lam", "Bar", "Ezek", "Dan",
         "Hos", "Joel", "Amos", "Obad", "Jonah", "Mic", "Nah", "Hab", "Zeph", "Hag", "Zech", "Mal",
         "Matt", "Mark", "Luke", "John", "Acts",
         "Rom", "1Cor", "2Cor", "Gal", "Eph", "Phil", "Col", "1Thess", "2Thess", "1Tim", "2Tim", "Titus", "Phlm",
         "Heb", "Jas", "1Pet", "2Pet", "1John", "2John", "3John", "Jude", "Rev"};

                        righeDaScrivere.Add("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
                        righeDaScrivere.Add("<osis xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"http://www.bibletechnologies.net/osisCore.2.0.1.xsd\">");
                        righeDaScrivere.Add("<osisText osisIDWork=\"" + info.Abbreviazione + "\" osisRefWork=\"Bible\">");
                        righeDaScrivere.Add("<header>");
                        righeDaScrivere.Add("  <work osisWork=\"" + info.Abbreviazione + "\">");
                        if (!string.IsNullOrEmpty(info.Titolo))
                            righeDaScrivere.Add("    <title>" + info.Titolo + "</title>");
                        righeDaScrivere.Add("    <identifier type=\"OSIS\">Bible." + info.Abbreviazione + "</identifier>");
                        if (!string.IsNullOrEmpty(info.Lingua))
                            righeDaScrivere.Add("    <language type=\"ISO-639\">" + info.Lingua + "</language>");
                        righeDaScrivere.Add("    <refSystem>Bible</refSystem>");
                        righeDaScrivere.Add("    <creator>LaParola.Net</creator>");
                        if (!string.IsNullOrEmpty(info.CasaEditrice))
                            righeDaScrivere.Add("    <publisher>" + info.CasaEditrice + "</publisher>");
                        if (!string.IsNullOrEmpty(info.Data))
                            righeDaScrivere.Add("    <date type=\"original\">" + info.Data + "</date>");
                        righeDaScrivere.Add("    <date type=\"eversion\">" + DateTime.Now.Year + "</date>");
                        if (!string.IsNullOrEmpty(info.Isbn))
                            righeDaScrivere.Add("    <identifier type=\"ISBN\">" + info.Isbn + "</identifier>");
                        if (!string.IsNullOrEmpty(info.Copyright))
                            righeDaScrivere.Add("    <rights type=\"copyright\">" + info.Copyright + "</rights>");
                        if (!string.IsNullOrEmpty(info.Descrizione))
                            righeDaScrivere.Add("    <description>" + info.Descrizione + "</description>");
                        righeDaScrivere.Add("  </work>");
                        righeDaScrivere.Add("  <work osisWork=\"Bible\">");
                        righeDaScrivere.Add("    <refSystem>Bible</refSystem>");
                        righeDaScrivere.Add("  </work>");
                        righeDaScrivere.Add("</header>");
                        righeDaScrivere.Add("<p sID=\"1\" /><div type=\"testament\">");
                        StringBuilder rigaOsis = new StringBuilder(512);
                        StringBuilder paragrafo = new StringBuilder(32);
                        int numeroParagrafo = 1;

                        for (byte iLibro = 1; iLibro <= 73; ++iLibro)
                        {
                            capitoliInLibro = Principale.testi.CapitoliInLibro(iLibro, nomeVersione);
                            if (capitoliInLibro > 0)
                                righeDaScrivere.Add("<div type=\"book\" osisID=\"" + libriNomiOSIS[iLibro - 1] + "\">");
                            for (byte iCapitolo = 1; iCapitolo <= capitoliInLibro; ++iCapitolo)
                            {
                                versettiInCapitolo = Principale.testi.VersettiInCapitolo(iLibro, iCapitolo, nomeVersione);
                                rigaOsis.Length = 0;
                                righeDaScrivere.Add(rigaOsis.Append("  <chapter osisID=\"").Append(libriNomiOSIS[iLibro - 1]).Append(".").Append(iCapitolo).Append("\">").ToString());
                                for (byte iVersetto = 1; iVersetto <= versettiInCapitolo; ++iVersetto)
                                {
                                    testo = Principale.testi.TestoVersettoRaw(iLibro, iCapitolo, iVersetto, nomeVersione);
                                    testo = testo.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;"); // bisogna fare prima di aggiungere tag <...> qui sotto
                                    /*
                                     * non necessario
                                    while (testo.IndexOf(@"\'") >= 0)
                                    {
                                        indiceCarattereHex = testo.IndexOf(@"\'");
                                        testo = testo.Substring(0, indiceCarattereHex) + "&#" + (Uri.FromHex(testo[indiceCarattereHex + 2]) * 16 + Uri.FromHex(testo[indiceCarattereHex + 3])) + ";" + testo.Substring(indiceCarattereHex + 4);
                                    }*/
                                    while (testo.IndexOf(@"\par", StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        paragrafo.Length = 0;
                                        testo = paragrafo.Append(testo.Substring(0, testo.IndexOf(@"\par", StringComparison.OrdinalIgnoreCase))).Append("<p eID=\"").Append(numeroParagrafo).Append("\" /><p sID=\"").Append(numeroParagrafo + 1).Append("\" />").Append(testo.Substring(testo.IndexOf(@"\par", StringComparison.OrdinalIgnoreCase) + 4)).ToString();
                                        ++numeroParagrafo;
                                    }

                                    while (testo.IndexOf(@" {\super ", StringComparison.OrdinalIgnoreCase) >= 0)
                                    { // i numeri Strong non sono esportati
                                        inizioTag = testo.IndexOf(@" {\super ", StringComparison.OrdinalIgnoreCase);
                                        fineTag = testo.IndexOf("}", inizioTag, StringComparison.OrdinalIgnoreCase);
                                        testo = testo.Substring(0, inizioTag) + testo.Substring(fineTag + 1);
                                    }

                                    while (testo.IndexOf(@"\lptit1 ", StringComparison.OrdinalIgnoreCase) >= 0)
                                        testo = testo.Substring(0, testo.IndexOf(@"\lptit1 ", StringComparison.OrdinalIgnoreCase)) + "<head>" + testo.Substring(testo.IndexOf(@"\lptit1 ", StringComparison.OrdinalIgnoreCase) + 8);
                                    while (testo.IndexOf(@"\lptit0 ", StringComparison.OrdinalIgnoreCase) >= 0)
                                        testo = testo.Substring(0, testo.IndexOf(@"\lptit0 ", StringComparison.OrdinalIgnoreCase)) + "</head>" + testo.Substring(testo.IndexOf(@"\lptit0 ", StringComparison.OrdinalIgnoreCase) + 8);
                                    // nota: durante l'importazione, il nuovo paragrafo è messo dentro le tag \lptit, anche se nel file OSIS originale era dopo la chiusura
                                    // quindi il file esportato sarà leggermente diverso
                                    while (testo.IndexOf(@"{\i1 ", StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        inizioTag = testo.IndexOf(@"{\i1 ", StringComparison.OrdinalIgnoreCase);
                                        fineTag = testo.IndexOf("}", inizioTag, StringComparison.OrdinalIgnoreCase);
                                        testo = testo.Substring(0, inizioTag) + "<q>" + testo.Substring(inizioTag + 5, fineTag - inizioTag - 5) + "</q>" + testo.Substring(fineTag + 1);
                                    }
                                    while (testo.IndexOf(@"{\b1 ", StringComparison.OrdinalIgnoreCase) >= 0) // deve essere dopo "super", nel caso di parole con super dentro il titolo
                                    {
                                        inizioTag = testo.IndexOf(@"{\b1 ", StringComparison.OrdinalIgnoreCase);
                                        fineTag = testo.IndexOf("}", inizioTag, StringComparison.OrdinalIgnoreCase);
                                        testo = testo.Substring(0, inizioTag) + "<title>" + testo.Substring(inizioTag + 5, fineTag - inizioTag - 5) + "</title>" + testo.Substring(fineTag + 1);
                                    }
                                    while (testo.IndexOf(@"{\caps ", StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        inizioTag = testo.IndexOf(@"{\caps ", StringComparison.OrdinalIgnoreCase);
                                        fineTag = testo.IndexOf("}", inizioTag, StringComparison.OrdinalIgnoreCase);
                                        testo = testo.Substring(0, inizioTag) + "<divineName>" + testo.Substring(inizioTag + 7, fineTag - inizioTag - 7) + "</divineName>" + testo.Substring(fineTag + 1);
                                    }
                                    /*                                    while (testo.IndexOf(@"{\qr ") >= 0)
                                                                        {
                                                                            inizioTag = testo.IndexOf(@"{\qr ");
                                                                            fineTag = testo.IndexOf('}', inizioTag);
                                                                            testo = testo.Substring(0, inizioTag) + testo.Substring(inizioTag + 5, fineTag - inizioTag - 5) + testo.Substring(fineTag + 1);
                                                                        }*/
                                    if (testo.IndexOf("{", StringComparison.Ordinal) >= 0 || testo.IndexOf("}", StringComparison.Ordinal) >= 0 || testo.IndexOf("\\", StringComparison.Ordinal) >= 0)
                                        inizioTag = 0;
                                    rigaOsis.Length = 0;
                                    if (!string.IsNullOrEmpty(testo))
                                        righeDaScrivere.Add(rigaOsis.Append("    <verse osisID=\"").Append(libriNomiOSIS[iLibro - 1]).Append(".").Append(iCapitolo).Append(".").Append(iVersetto).Append("\">").Append(testo.Trim()).Append("</verse>").ToString());
                                }
                                righeDaScrivere.Add("  </chapter>");
                            }
                            if (capitoliInLibro > 0)
                                righeDaScrivere.Add("</div>");
                            if (iLibro == 46)
                            {
                                righeDaScrivere.Add("</div>");
                                righeDaScrivere.Add("<div type=\"testament\">");
                            }
                            worker.ReportProgress(iLibro, e);
                        }

                        righeDaScrivere.Add("</div>");
                        righeDaScrivere.Add("<p eID=\"" + numeroParagrafo + "\" />");
                        righeDaScrivere.Add("</osisText>");
                        righeDaScrivere.Add("</osis>");
                        File.WriteAllLines(directoryBase + nomeVersione + ".xml", righeDaScrivere.ToArray(), utf8);
                        break;
                    #endregion
                    #region Bibbia Zefania
                    case EsportoTestoTipo.BibbiaZefania:
                        info = Principale.testi.Info(nomeVersione);
                        righeDaScrivere.Add("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
                        righeDaScrivere.Add("<XMLBIBLE xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"xmlbible1007.xsd\" version=\"2.0.1.22\" type=\"x-bible\" status=\"v\" biblename=\"" + info.Nome + "\" lgid=\"" + info.Lingua + "\">");
                        righeDaScrivere.Add("<INFORMATION>");
                        righeDaScrivere.Add("  <creator>LaParola.Net</creator>");
                        righeDaScrivere.Add("  <subject>Bible</subject>");
                        righeDaScrivere.Add("  <format>Zefania XML Bible Markup Language</format>");
                        righeDaScrivere.Add("  <title>" + info.Titolo + "</title>");
                        righeDaScrivere.Add("  <identifier>" + info.Abbreviazione + "</identifier>");
                        righeDaScrivere.Add("  <description>" + info.Descrizione + "</description>");
                        righeDaScrivere.Add("  <publisher>" + info.CasaEditrice + "</publisher>");
                        righeDaScrivere.Add("  <language>" + info.Lingua + "</language>");
                        righeDaScrivere.Add("  <rights>" + info.Copyright + "</rights>");
                        righeDaScrivere.Add("  <date>" + info.Data + "</date>");
                        righeDaScrivere.Add("</INFORMATION>");

                        StringBuilder rigaZefania = new StringBuilder(512);
                        for (byte iLibro = 1; iLibro <= 73; ++iLibro)
                        {
                            capitoliInLibro = Principale.testi.CapitoliInLibro(iLibro, nomeVersione);
                            if (capitoliInLibro > 0)
                                righeDaScrivere.Add("<BIBLEBOOK bnumber=\"" + ConvertiLibro73A66Zefania(iLibro).ToString(CultureInfo.InvariantCulture) + "\">");
                            for (byte iCapitolo = 1; iCapitolo <= capitoliInLibro; ++iCapitolo)
                            {
                                versettiInCapitolo = Principale.testi.VersettiInCapitolo(iLibro, iCapitolo, nomeVersione);
                                rigaZefania.Length = 0;
                                righeDaScrivere.Add(rigaZefania.Append("  <CHAPTER cnumber=\"").Append(iCapitolo).Append("\">").ToString());
                                for (byte iVersetto = 1; iVersetto <= versettiInCapitolo; ++iVersetto)
                                {
                                    testo = Principale.testi.TestoVersettoRaw(iLibro, iCapitolo, iVersetto, nomeVersione);
                                    testo = testo.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;"); // bisogna fare prima di aggiungere tag <...> qui sotto
                                    while (testo.IndexOf(@"\par", StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        inizioTag = testo.IndexOf(@"\par", StringComparison.OrdinalIgnoreCase);
                                        testo = testo.Substring(0, inizioTag) + testo.Substring(inizioTag + 4);
                                    }

                                    while (testo.IndexOf(@" {\super ", StringComparison.OrdinalIgnoreCase) >= 0)
                                    { // i numeri Strong non sono esportati
                                        inizioTag = testo.IndexOf(@" {\super ", StringComparison.OrdinalIgnoreCase);
                                        fineTag = testo.IndexOf("}", inizioTag, StringComparison.OrdinalIgnoreCase);
                                        testo = testo.Substring(0, inizioTag) + testo.Substring(fineTag + 1);
                                    }

                                    /*
                                    // lasciavo i titoli, ma non so perché. Meglio senza
                                    while (testo.IndexOf(@"\lptit1 ", StringComparison.OrdinalIgnoreCase) >= 0)
                                        testo = testo.Substring(0, testo.IndexOf(@"\lptit1 ", StringComparison.OrdinalIgnoreCase)) + testo.Substring(testo.IndexOf(@"\lptit1 ", StringComparison.OrdinalIgnoreCase) + 8);
                                    while (testo.IndexOf(@"\lptit0 ", StringComparison.OrdinalIgnoreCase) >= 0)
                                        testo = testo.Substring(0, testo.IndexOf(@"\lptit0 ", StringComparison.OrdinalIgnoreCase)) + " " + testo.Substring(testo.IndexOf(@"\lptit0 ", StringComparison.OrdinalIgnoreCase) + 8);
                                    */
                                    while (testo.IndexOf(@"\lptit1 ", StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        inizioTag = testo.IndexOf(@"\lptit1 ", StringComparison.OrdinalIgnoreCase);
                                        fineTag = testo.IndexOf(@"\lptit0 ", inizioTag, StringComparison.OrdinalIgnoreCase);
                                        testo = testo.Substring(0, inizioTag) + testo.Substring(fineTag + 8);
                                    }
                                    while (testo.IndexOf(@"{\i1 ", StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        inizioTag = testo.IndexOf(@"{\i1 ", StringComparison.OrdinalIgnoreCase);
                                        fineTag = testo.IndexOf("}", inizioTag, StringComparison.OrdinalIgnoreCase);
                                        testo = testo.Substring(0, inizioTag) + testo.Substring(inizioTag + 5, fineTag - inizioTag - 5) + testo.Substring(fineTag + 1);
                                    }
                                    while (testo.IndexOf(@"{\b1 ", StringComparison.OrdinalIgnoreCase) >= 0) // deve essere dopo "super", nel caso di parole con super dentro il titolo
                                    {
                                        inizioTag = testo.IndexOf(@"{\b1 ", StringComparison.OrdinalIgnoreCase);
                                        fineTag = testo.IndexOf("}", inizioTag, StringComparison.OrdinalIgnoreCase);
                                        testo = testo.Substring(0, inizioTag) + testo.Substring(inizioTag + 5, fineTag - inizioTag - 5) + testo.Substring(fineTag + 1);
                                    }
                                    while (testo.IndexOf(@"{\caps ", StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        inizioTag = testo.IndexOf(@"{\caps ", StringComparison.OrdinalIgnoreCase);
                                        fineTag = testo.IndexOf("}", inizioTag, StringComparison.OrdinalIgnoreCase);
                                        testo = testo.Substring(0, inizioTag) + testo.Substring(inizioTag + 7, fineTag - inizioTag - 7) + testo.Substring(fineTag + 1);
                                    }
                                    if (testo.IndexOf("{", StringComparison.OrdinalIgnoreCase) >= 0 || testo.IndexOf("}", StringComparison.OrdinalIgnoreCase) >= 0 || testo.IndexOf("\\", StringComparison.OrdinalIgnoreCase) >= 0)
                                        inizioTag = 0;
                                    rigaZefania.Length = 0;
                                    if (!string.IsNullOrEmpty(testo))
                                        righeDaScrivere.Add(rigaZefania.Append("    <VERS vnumber=\"").Append(iVersetto).Append("\">").Append(testo.Trim()).Append("</VERS>").ToString());
                                }
                                righeDaScrivere.Add("  </CHAPTER>");
                            }
                            if (capitoliInLibro > 0)
                                righeDaScrivere.Add("</BIBLEBOOK>");
                            worker.ReportProgress(iLibro, e);
                        }

                        righeDaScrivere.Add("</XMLBIBLE>");
                        File.WriteAllLines(directoryBase + nomeVersione + ".xml", righeDaScrivere.ToArray(), utf8);
                        break;
                    #endregion
                    #region Bibbia Java
                    case EsportoTestoTipo.BibbiaJava:
                        FileStream fs = new FileStream(directoryBase + nomeVersione.ToLowerInvariant().Replace(' ', '_') + ".lpj", FileMode.Create);
                        BinaryWriter w = new BinaryWriter(fs);
                        info = Principale.testi.Info(nomeVersione);
                        pInizioDati = ScriviDatiJava(info, w, true);

                        int nVersetto = 0;
                        List<long> indiceVersetti = new List<long>();

                        for (byte iLibro = 1; iLibro <= 73; ++iLibro)
                        {
                            capitoliInLibro = Principale.testi.CapitoliInLibro(iLibro, nomeVersione);
                            for (byte iCapitolo = 1; iCapitolo <= capitoliInLibro; ++iCapitolo)
                            {
                                versettiInCapitolo = Principale.testi.VersettiInCapitolo(iLibro, iCapitolo, nomeVersione);
                                for (byte iVersetto = 1; iVersetto <= versettiInCapitolo; ++iVersetto)
                                {
                                    testo = Principale.testi.TestoVersettoRaw(iLibro, iCapitolo, iVersetto, nomeVersione);
                                    testo = ConvAHTMLJava(testo, nomeVersione);
                                    //if (!string.IsNullOrEmpty(testo))
                                    //{
                                    indiceVersetti.Add(w.Seek(0, SeekOrigin.Current));
                                    nVersetto++;
                                    w.Write(testo.ToCharArray());
                                    w.Write((char)0);
                                    //}
                                }
                            }
                        }

                        UInt32 inizioTestoIndiceLC = (UInt32)(w.Seek(0, SeekOrigin.Current));
                        for (byte iLibro = 1; iLibro <= 73; ++iLibro)
                            w.Write(Principale.testi.CapitoliInLibro(iLibro, nomeVersione));
                        byte v;
                        for (byte iLibro = 1; iLibro <= 73; ++iLibro)
                        {
                            for (byte iCapitolo = 1; iCapitolo <= Principale.testi.CapitoliInLibro(iLibro, nomeVersione); ++iCapitolo)
                            {
                                v = Principale.testi.VersettiInCapitolo(iLibro, iCapitolo, nomeVersione);
                                w.Write(v);
                                // nVersetti += v;
                            }
                        }

                        long inizioTestoIndice = w.Seek(0, SeekOrigin.Current);
                        for (int i = 0; i < indiceVersetti.Count; ++i)
                        {
                            w.Write(Intabyte4(indiceVersetti[i]));
                        }

                        long inizioParole = w.Seek(0, SeekOrigin.Current);
                        string[] parole = Principale.testi.Parole(nomeVersione);
                        w.Write(Encoding.Convert(utf8, unicode, utf8.GetBytes(string.Join("|", parole))));

                        long inizioParoleIndiceIndice = w.Seek(0, SeekOrigin.Current);
                        int nParole = parole.Length;
                        int numeroApparenze = 0;
                        byte[] datiDaScrivere = new byte[4 * nParole + 4];
                        MemoryStream ms = new MemoryStream(datiDaScrivere, true);
                        BinaryWriter bwMemoria = new BinaryWriter(ms);
                        bwMemoria.Write((UInt32)0);
                        for (int i = 0; i < nParole; ++i)
                        {
                            numeroApparenze += Principale.testi.NumeroVolteParola(parole[i], nomeVersione);
                            bwMemoria.Write(Intabyte4(6 * numeroApparenze));
                        }
                        bwMemoria.Seek(0, SeekOrigin.Begin);
                        w.Write(datiDaScrivere);

                        long inizioParoleIndice = w.Seek(0, SeekOrigin.Current);
                        w.Write(Principale.testi.GetApparenzeParole(nomeVersione));

                        long inizioRadici = w.Seek(0, SeekOrigin.Current);
                        string[] radici = Principale.testi.Radici(nomeVersione);
                        w.Write(Encoding.Convert(utf8, unicode, utf8.GetBytes(string.Join("|", radici))));
                        long inizioRadiciDiParole = w.Seek(0, SeekOrigin.Current);
                        for (int i = 0; i < nParole; ++i)
                            w.Write(Intabyte4(Array.BinarySearch(radici, Principale.testi.RadiceDiParola(parole[i], nomeVersione), new ConfrontoCI())));

                        long inizioRadiciDiverse = w.Seek(0, SeekOrigin.Current);
                        List<string> radDiverse = new List<string>(Principale.testi.GetRadiciDiverse(nomeVersione));
                        if (radDiverse.Count == 0)
                            inizioRadiciDiverse = 0;
                        else
                        {
                            w.Write(Intabyte4(radDiverse.Count));
                            string[] radds;
                            foreach (string rd in radDiverse)
                            {
                                radds = rd.Split('|');
                                w.Write(Intabyte4(Convert.ToByte(radds[0], CultureInfo.InvariantCulture)));
                                w.Write(Intabyte4(Convert.ToByte(radds[1], CultureInfo.InvariantCulture)));
                                w.Write(Intabyte4(Convert.ToByte(radds[2], CultureInfo.InvariantCulture)));
                                w.Write(Intabyte4(Convert.ToUInt16(radds[3], CultureInfo.InvariantCulture)));
                                w.Write(radds[4].ToCharArray());
                                w.Write((char)0);
                            }
                        }

                        long inizioRiferimentiDiversi = w.Seek(0, SeekOrigin.Current);
                        List<string> rifDiverse = new List<string>(Principale.testi.GetRiferimentiDiversi(nomeVersione));
                        if (rifDiverse.Count == 0)
                            inizioRiferimentiDiversi = 0;
                        else
                        {
                            w.Write(Intabyte4(rifDiverse.Count));
                            string[] rifds;
                            foreach (string rd in rifDiverse)
                            {
                                rifds = rd.Split('|');
                                for (int j = 0; j <= 5; ++j)
                                    w.Write(Intabyte4(Convert.ToInt16(rifds[j], CultureInfo.InvariantCulture)));
                            }
                        }

                        w.Seek((int)pInizioDati + 4, SeekOrigin.Begin);
                        w.Write(Intabyte4(inizioTestoIndiceLC));
                        w.Write(Intabyte4(inizioTestoIndice));
                        w.Write(Intabyte4(inizioParole));
                        w.Write(Intabyte4(inizioParoleIndiceIndice));
                        w.Write(Intabyte4(inizioParoleIndice));
                        w.Write(Intabyte4(inizioRadici));
                        w.Write(Intabyte4(inizioRadiciDiParole));
                        w.Write(Intabyte4(inizioRadiciDiverse));
                        w.Write(Intabyte4(inizioRiferimentiDiversi));

                        w.Seek(0, SeekOrigin.End);

                        w.Close();
                        fs.Close();
                        break;
                    #endregion
                    #region Collezione file diversi
                    case EsportoTestoTipo.CollezioneFile:
                        Collection<string> note = Principale.testi.Note(nomeVersione);
                        foreach (string nota in note)
                        {
                            File.WriteAllText(directoryBase + nota.Replace("?", "").Replace(":", "-").Replace("\"", "'") + ".rtf", Principale.testi.GetNotaTesto(nota, nomeVersione));
                            worker.ReportProgress(-1, e);
                        }
                        List<string> noteInOrdine = new List<string>(Principale.testi.GetNoteInOrdine(nomeVersione));
                        for (int i = 0; i < noteInOrdine.Count; ++i)
                            noteInOrdine[i] = noteInOrdine[i].Replace("?", "").Replace(":", "-");
                        if (noteInOrdine.Count > 0)
                            File.WriteAllLines(directoryBase + nomeVersione + ".ordine", noteInOrdine.ToArray(), utf8);
                        break;
                    #endregion
                    #region Collezione Java
                    case EsportoTestoTipo.CollezioneJava:
                        if (Principale.testi.CollezioneModificata(nomeVersione))
                        {
                            MessageBox.Show(Principale.LocRM.GetString("ManageNotesErrorModified"), Principale.LocRM.GetString("MiscInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions);
                            break;
                        }
                        FileStream fsc = new FileStream(directoryBase + nomeVersione.ToLowerInvariant().Replace(' ', '_') + ".lpj", FileMode.Create);
                        BinaryWriter wc = new BinaryWriter(fsc);
                        info = Principale.testi.Info(nomeVersione);
                        pInizioDati = ScriviDatiJava(info, wc, false);
                        wc.Write(Intabyte4(0)); // inizio riferimenti citati
                        wc.Write(Intabyte4(0)); // inizio note in ordine

                        Collection<string> notec = Principale.testi.Note(nomeVersione);
                        StringBuilder titoli = new StringBuilder();
                        int numeroNote = notec.Count;
                        long[] posizioniNote = new long[numeroNote];
                        int nNota = 0;
                        foreach (string nota in notec)
                        {
                            posizioniNote[nNota] = wc.Seek(0, SeekOrigin.Current);
                            ++nNota;
                            testo = Principale.testi.GetNotaTesto(nota, nomeVersione);
                            //                            if (testo.Contains("location"))
                            //                              nNota = nNota + 0;
                            testo = testo.Replace("\r\n", " ").Trim();
                            if (testo.IndexOf("deflang") >= 0)
                                testo = testo.Substring(testo.IndexOf("\\", testo.IndexOf("deflang")));
                            //                            if (testo.IndexOf("lang1040") >= 0)
                            //                                testo = testo.Substring(testo.IndexOf("\\", testo.IndexOf("lang1040")));
                            //                            if (testo.IndexOf("lang3081") >= 0)
                            //                                testo = testo.Substring(testo.IndexOf("\\", testo.IndexOf("lang3081")));
                            if (testo.IndexOf("\\fonttbl{") >= 0)
                                testo = testo.Substring(testo.IndexOf("}}", testo.IndexOf("\\fonttbl{")) + 2);
                            if (testo.IndexOf("\\colortbl") >= 0)
                                testo = testo.Substring(testo.IndexOf("}", testo.IndexOf("\\colortbl")) + 1);
                            if (testo.IndexOf("viewkind4") >= 0)
                                testo = testo.Substring(testo.IndexOf("\\", testo.IndexOf("viewkind4")));
                            if (testo.StartsWith("{\\rtf1"))
                                testo = testo.Substring(6);
                            if (testo.StartsWith("\\uc1"))
                                testo = testo.Substring(4);
                            if (testo.StartsWith("\\pard"))
                                testo = testo.Substring(5);
                            if (testo.EndsWith("\0"))
                                testo = testo.Substring(0, testo.Length - 1);
                            if (testo.EndsWith("}"))
                                testo = testo.Substring(0, testo.Length - 1);
                            if (testo.EndsWith("\\par "))
                                testo = testo.Substring(0, testo.Length - 5);
                            if (testo.EndsWith("\\par"))
                                testo = testo.Substring(0, testo.Length - 4);
                            testo = ConvAHTMLJava(testo, nomeVersione);
                            wc.Write(testo.ToCharArray());
                            wc.Write((char)0);
                            titoli.Append(nota).Append("|");
                            worker.ReportProgress(-1, e);
                        }

                        long inizioTestoIndiceLCc = wc.Seek(0, SeekOrigin.Current);
                        wc.Write(Encoding.Convert(utf8, unicode, utf8.GetBytes(titoli.ToString().Replace("_", "-"))));
                        wc.Write((char)0);
                        long inizioTestoIndicec = wc.Seek(0, SeekOrigin.Current);
                        for (int i = 0; i < numeroNote; ++i)
                            wc.Write(Intabyte4(posizioniNote[i] - posizioniNote[0]));

                        long inizioParolec = wc.Seek(0, SeekOrigin.Current);
                        string[] parolec = Principale.testi.Parole(nomeVersione);
                        wc.Write(Encoding.Convert(utf8, unicode, utf8.GetBytes(string.Join("|", parolec))));

                        long inizioParoleIndiceIndicec = wc.Seek(0, SeekOrigin.Current);
                        int nParolec = parolec.Length;
                        int numeroApparenzec = 0;
                        byte[] datiDaScriverec = new byte[4 * nParolec + 4];
                        MemoryStream msc = new MemoryStream(datiDaScriverec, true);
                        BinaryWriter bwMemoriac = new BinaryWriter(msc);
                        bwMemoriac.Write(Intabyte4(0));
                        for (int i = 0; i < nParolec; ++i)
                        {
                            numeroApparenzec += Principale.testi.NumeroVolteParola(parolec[i], nomeVersione);
                            bwMemoriac.Write(Intabyte4(6 * numeroApparenzec));
                        }
                        bwMemoriac.Seek(0, SeekOrigin.Begin);
                        wc.Write(datiDaScriverec);

                        long inizioParoleIndicec = wc.Seek(0, SeekOrigin.Current);
                        wc.Write(Principale.testi.GetApparenzeParole(nomeVersione));

                        long inizioRadicic = wc.Seek(0, SeekOrigin.Current);
                        string[] radicic = Principale.testi.Radici(nomeVersione);
                        wc.Write(Encoding.Convert(utf8, unicode, utf8.GetBytes(string.Join("|", radicic))));
                        long inizioRadiciDiParolec = wc.Seek(0, SeekOrigin.Current);
                        for (int i = 0; i < nParolec; ++i)
                            wc.Write(Intabyte4(Array.BinarySearch(radicic, Principale.testi.RadiceDiParola(parolec[i], nomeVersione), new ConfrontoCI())));

                        long inizioRadiciDiversec = wc.Seek(0, SeekOrigin.Current);
                        List<string> radDiversec = new List<string>(Principale.testi.GetRadiciDiverse(nomeVersione));
                        if (radDiversec.Count == 0)
                            inizioRadiciDiversec = 0;
                        else
                        {
                            wc.Write(Intabyte4(radDiversec.Count));
                            string[] raddsc;
                            foreach (string rd in radDiversec)
                            {
                                raddsc = rd.Split('|');
                                wc.Write(Intabyte4(Convert.ToUInt32(raddsc[0], CultureInfo.InvariantCulture)));
                                wc.Write(Intabyte4(Convert.ToUInt16(raddsc[1], CultureInfo.InvariantCulture)));
                                wc.Write(raddsc[2].ToCharArray());
                                wc.Write((char)0);
                            }
                        }

                        long inizioRiferimentiDiversic = wc.Seek(0, SeekOrigin.Current);
                        List<string> rifDiversec = new List<string>(Principale.testi.GetRiferimentiDiversi(nomeVersione));
                        if (rifDiversec.Count == 0)
                            inizioRiferimentiDiversic = 0;
                        else
                        {
                            wc.Write(Intabyte4(rifDiversec.Count));
                            string[] rifds;
                            foreach (string rd in rifDiversec)
                            {
                                rifds = rd.Split('|');
                                for (int j = 0; j <= 5; ++j)
                                    wc.Write(Intabyte4(Convert.ToInt16(rifds[j], CultureInfo.InvariantCulture)));
                            }
                        }

                        long inizioRiferimentiCitatic = wc.Seek(0, SeekOrigin.Current);
                        List<string> rifCitatic = new List<string>(Principale.testi.GetRiferimentiCitati(nomeVersione));
                        if (rifCitatic.Count == 0)
                            inizioRiferimentiCitatic = 0;
                        else
                        {
                            wc.Write(Intabyte4(rifCitatic.Count));
                            string[] rifcs;
                            foreach (string rc in rifCitatic)
                            {
                                rifcs = rc.Split('|');
                                for (int j = 0; j < 7; ++j)
                                    wc.Write(Intabyte4(Convert.ToInt16(rifcs[j], CultureInfo.InvariantCulture)));
                            }
                        }

                        long inizioNoteInOrdinec = wc.Seek(0, SeekOrigin.Current);
                        List<string> noteInOrdinec = new List<string>(Principale.testi.GetNoteInOrdine(nomeVersione));
                        int numeroNoteInOrdine = noteInOrdinec.Count;
                        if (numeroNoteInOrdine > 0)
                        {
                            wc.Write(Intabyte4(numeroNoteInOrdine));
                            for (int i = 0; i < numeroNoteInOrdine; ++i)
                            {
                                wc.Write(noteInOrdinec[i].ToCharArray());
                                wc.Write((char)0);
                            }
                        }
                        else
                            inizioNoteInOrdinec = 0;

                        wc.Seek((int)pInizioDati + 4, SeekOrigin.Begin);
                        wc.Write(Intabyte4(inizioTestoIndiceLCc));
                        wc.Write(Intabyte4(inizioTestoIndicec));
                        wc.Write(Intabyte4(inizioParolec));
                        wc.Write(Intabyte4(inizioParoleIndiceIndicec));
                        wc.Write(Intabyte4(inizioParoleIndicec));
                        wc.Write(Intabyte4(inizioRadicic));
                        wc.Write(Intabyte4(inizioRadiciDiParolec));
                        wc.Write(Intabyte4(inizioRadiciDiversec));
                        wc.Write(Intabyte4(inizioRiferimentiDiversic));
                        wc.Write(Intabyte4(inizioRiferimentiCitatic));
                        wc.Write(Intabyte4(inizioNoteInOrdinec));
                        wc.Seek(0, SeekOrigin.End);

                        wc.Close();
                        fsc.Close();
                        break;
                        #endregion
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("ManageNotesErrorNotExported"), exc.Message, nomeVersione), Principale.LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions);
            }
            finally
            {
                Principale.testi.Formato = formatoVecchio;
            }

            if (argomenti.tipo != EsportoTestoTipo.CollezioneJava && argomenti.tipo != EsportoTestoTipo.BibbiaJava)
            {
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
            }

            e.Result = argomenti;
            if (worker.CancellationPending)
                e.Cancel = true;
        }

        private static string ConvAHTMLJava(string testo, string nomeVersione)
        {
            while (testo.IndexOf("A Student's Guide to New Testament") > -1)
            {
                int iSG = testo.IndexOf("A Student's Guide to New Testament");
                int iBR = testo.LastIndexOf("\\par", iSG);
                testo = testo.Remove(iBR, testo.IndexOf("\\v0 ", iSG) - iBR + 4);
            }

            int fineTag, inizioTag, mezzoTag;
            testo = testo.Replace("&", "&amp;").Replace(@"\\", @"\").Replace(@" \ ", " "); // in un caso è un errore; supponiamo che sia sempre così
            if (nomeVersione.Contains("morfol"))
            {
                //testo = testo.Replace(">", "£lpn%").Replace("<", "<span class=\"m\">").Replace("£lpn%", "</span>");
                // "ΒΙΒΛΟΣ <βίβλος N-----NSF-> γενέσεως <γένεσις N-----GSF-> Ἰησοῦ <Ἰησοῦς N-----GSM-> Χριστοῦ <Χριστός N-----GSM-> υἱοῦ <υἱός N-----GSM-> Δαυεὶδ <Δαυίδ N---------> υἱοῦ <υἱός N-----GSM-> Ἀβραάμ <Ἀβραάμ N--------->.\\par "
                testo = testo.Replace("<", "£lpn%");
                while (testo.Contains("£lpn%"))
                {
                    inizioTag = testo.IndexOf("£lpn%");
                    mezzoTag = testo.IndexOf(" ", inizioTag);
                    testo = testo.Insert(testo.IndexOf(">", inizioTag), "</span").Insert(mezzoTag + 1, "<span class=\"m\">").Insert(mezzoTag, "</span>").Insert(inizioTag + 5, "<span class=\"r\">").Remove(inizioTag, 5);
                }
                while (testo.Contains("--"))
                    testo = testo.Replace("--", "-");
                testo = testo.Replace("-<", "<");
            }
            else
                testo = testo.Replace("<", "&lt;").Replace(">", "&gt");
            //if (testo.Contains("Introduzione alla lettera ai Filippesi"))
            //fineTag = 1;

            testo = SostituisciTagNumeri(testo, "cf");

            StringBuilder link = new StringBuilder();
            Riferimento rif;
            testo = testo.Replace(@"\v\'03\'05#260000000000-260000000000\'04\v0 ", "").Replace(@"\b\v\'03\'05#590010030000-590010030000\'04\b0\v0 ", "").Replace(@"\v\f1\'02\'03\'05#560010220000-560010220000\'04\v0\f2", "");
            testo = testo.Replace(@"\v\f0\", @"\v\").Replace(@"\v\f1\", @"\v\").Replace(@"\v\f2\", @"\v\").Replace(@"\v\f3\", @"\v\").Replace(@"\v\f4\", @"\v\");
            testo = testo.Replace(@"\v\fs24", @"\v").Replace(@"\v\fs27", @"\v").Replace(@"\v\fs32", @"\v").Replace(@"\v\fs36", @"\v");
            testo = testo.Replace(@"\v\'02\i0", @"\i0\v\'02").Replace(@"\v\'02\i", @"\i\v\'02").Replace(@"\v\'02\cf0", @"\cf0\v\'02");
            testo = testo.Replace(@"\v\'02\b\i0", @"\b\i0\v\'02").Replace(@"\v\'02\b", @"\b\v\'02").Replace(@"\b0\v0", @"\v0\b0").Replace(@"\v0\f0", @"\v0").Replace(@"\v0\f1", @"\v0").Replace(@"\v0\f2", @"\v0").Replace(@"\v0\f3", @"\v0").Replace(@"\v0\fs24", @"\v0");
            testo = testo.Replace(@"\'02", RichTextBoxEx.InizioLink.ToString()).Replace(@"\'03", " " + RichTextBoxEx.FineLink1).Replace(@"\'04", RichTextBoxEx.FineLink1.ToString()).Replace(@"\'05", RichTextBoxEx.FineLinkBrano.ToString()).Replace(@"\'06", RichTextBoxEx.FineLinkNota.ToString()).Replace(@"\'07", RichTextBoxEx.FineLinkFile.ToString());
            testo = testo.Replace(@"\v" + RichTextBoxEx.InizioLink, @"\v " + RichTextBoxEx.InizioLink);
            String linkDaCercare = @"\v " + RichTextBoxEx.InizioLink + " " + RichTextBoxEx.FineLink1;
            while (testo.IndexOf(linkDaCercare) >= 0)
            {
                testo = testo.Substring(0, testo.IndexOf(linkDaCercare) - 1) + testo.Substring(testo.IndexOf(@"\v0", testo.IndexOf(linkDaCercare)) + 4);
            }

            linkDaCercare = @"\v " + RichTextBoxEx.InizioLink + @"\v0 ";
            while (testo.IndexOf(linkDaCercare, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                inizioTag = testo.IndexOf(linkDaCercare, StringComparison.OrdinalIgnoreCase);
                mezzoTag = testo.IndexOf("\\v", inizioTag + 5, StringComparison.OrdinalIgnoreCase);
                fineTag = testo.IndexOf("\\v0", mezzoTag, StringComparison.OrdinalIgnoreCase);
                switch (testo[mezzoTag + 4])
                {
                    case RichTextBoxEx.FineLinkBrano:
                        // laparola:1 1 1 1 1 2@*bibbia oppure laparola:1 1 1 1 1 2@Nuova Riveduta
                        rif = Principale.testi.ConvertiRiferimento(Principale.testi.ConvertiTitoloNotaARiferimento(testo.Substring(mezzoTag + 5, fineTag - mezzoTag - 6)));
                        link.Length = 0;
                        foreach (byte[] brano in rif.Brani)
                            link.Append(brano[0]).Append(" ").Append(brano[1]).Append(" ").Append(brano[2]).Append(" ").Append(brano[3]).Append(" ").Append(brano[4]).Append(" ").Append(brano[5]).Append(";");
                        if (link.Length > 0)
                            link.Remove(link.Length - 1, 1);
                        testo = testo.Substring(0, inizioTag) + "<a href=\"laparola:" + link.ToString() + "@*bibbia\">" + testo.Substring(inizioTag + 8, mezzoTag - inizioTag - 8) + "</a>" + testo.Substring(fineTag + 3);
                        break;
                    case RichTextBoxEx.FineLinkNota:
                        if (testo[mezzoTag + 5] == '#')
                        {
                            // nota: usato solo in Aiuto Biblico in Apoc 3:14-22
                            testo = testo.Substring(0, inizioTag) + "<a href=\"laparola:" + Principale.testi.ConvertiTitoloNotaARiferimento(testo.Substring(mezzoTag + 5, fineTag - mezzoTag - 6)) + "\">" + testo.Substring(inizioTag + 8, mezzoTag - inizioTag - 8) + "</a>" + testo.Substring(fineTag + 3);
                            // forse il seguente metodo è più preciso, ma forse richiede un cambio nel programma, e di aggiungere @nomeVersione alla fine
                            // laparola:1 1 1 1 1 2
                            /*
                            rif = Principale.testi.ConvertiRiferimento(Principale.testi.ConvertiTitoloNotaARiferimento(testo.Substring(mezzoTag + 5, fineTag - mezzoTag - 6)));
                            link.Length = 0;
                            foreach (byte[] brano in rif.Brani)
                                link.Append(brano[0]).Append(" ").Append(brano[1]).Append(" ").Append(brano[2]).Append(" ").Append(brano[3]).Append(" ").Append(brano[4]).Append(" ").Append(brano[5]).Append(";");
                            if (link.Length > 0)
                                link.Remove(link.Length - 1, 1);
                            testo = testo.Substring(0, inizioTag) + "<a href=\"laparola:" + link.ToString() + "\">" + testo.Substring(inizioTag + 8, mezzoTag - inizioTag - 8) + "</a>" + testo.Substring(fineTag + 3);
                             * */
                        }
                        else
                        {
                            // laparola:$titolo oppure laparola:riferimento
                            String titoloNota = testo.Substring(mezzoTag + 5, fineTag - mezzoTag - 6);
                            String dollaro = (Principale.testi.GetNumeroNotaTitolo(titoloNota, nomeVersione) < 0 ? "" : "$");
                            testo = testo.Substring(0, inizioTag) + "<a href=\"laparola:" + dollaro + titoloNota.Replace("_", "-") + "\">" + testo.Substring(inizioTag + 8, mezzoTag - inizioTag - 8) + "</a>" + testo.Substring(fineTag + 3);
                        }
                        break;
                    case RichTextBoxEx.FineLinkFile:
                        testo = testo.Substring(0, inizioTag) + testo.Substring(mezzoTag + 5, fineTag - mezzoTag - 6) + testo.Substring(fineTag + 3);
                        break;
                    default:
                        break;
                }
            }

            linkDaCercare = @"\v " + RichTextBoxEx.FineLink1 + RichTextBoxEx.FineLinkBrano;
            while (testo.IndexOf(linkDaCercare) >= 0)
            {
                testo = testo.Substring(0, testo.IndexOf(linkDaCercare)) + testo.Substring(testo.IndexOf(@"\v0", testo.IndexOf(linkDaCercare)) + 3);
            }

            testo = SostituisciTagSingolo(testo, "pard", "");
            testo = SostituisciTagSingolo(testo, "par", "<br />", false);

            while (testo.IndexOf(@" {\super ", StringComparison.OrdinalIgnoreCase) >= 0)
            { // i numeri Strong non erano esportati, non mi ricordo perché, adesso sì
                testo = testo.Replace(@"{\super ", "{");
                //inizioTag = testo.IndexOf(@" {\super ", StringComparison.OrdinalIgnoreCase);
                //fineTag = testo.IndexOf("}", inizioTag, StringComparison.OrdinalIgnoreCase);
                //testo = testo.Substring(0, inizioTag) + testo.Substring(fineTag + 1);
            }

            testo = testo.Replace(@"\lptit1 ", "<lpt>");
            testo = testo.Replace(@"\lptit0 ", "</lpt>");

            testo = SostituisciTagParentesi(testo, "i1", "i");
            testo = SostituisciTagParentesi(testo, "b1", "b"); // deve essere dopo "super", nel caso di parole con super dentro il titolo
            testo = SostituisciTagParentesi(testo, "b", "b");
            /*                Trace.Listeners.Add(new TextWriterTraceListener(@"d:\trace.txt"));
                            Trace.WriteLine(testo);
                            Trace.Close();*/
            int iPlain, iUL, iUL0, iULnone;
            while (testo.IndexOf(@"\ul ") > -1)
            {
                iUL = testo.IndexOf(@"\ul ");
                //                if (testo.Substring(iUL, 10) == "\\ul Quelli")
                //                    iUL = iUL + 1 - 1;
                iPlain = testo.IndexOf(@"\plain", iUL);
                iUL0 = testo.IndexOf(@"\ul0", iUL);
                iULnone = testo.IndexOf(@"\ulnone", iUL);
                if (iPlain > 0 && (iPlain < iUL0 || iUL0 < 0) && (iPlain < iULnone || iULnone < 0))
                    testo = testo.Substring(0, iUL) + "<span style=\"text-decoration:underline;\">" + testo.Substring(iUL + 4, iPlain - iUL - 4) + "</span>" + testo.Substring(iPlain);
                else if (iUL0 > 0 && (iUL0 < iULnone || iULnone < 0))
                    testo = testo.Substring(0, iUL) + "<span style=\"text-decoration:underline;\">" + testo.Substring(iUL + 4, iUL0 - iUL - 4) + "</span>" + testo.Substring(iUL0 + 5);
                else if (iULnone > 0)
                    testo = testo.Substring(0, iUL) + "<span style=\"text-decoration:underline;\">" + testo.Substring(iUL + 4, iULnone - iUL - 4) + "</span>" + testo.Substring(iULnone + 7);
                else
                    testo = testo.Substring(0, iUL) + testo.Substring(iUL + 4);
            }
            while (testo.IndexOf(@"\ul") > -1)
            {
                iUL = testo.IndexOf(@"\ul");
                if (testo.Substring(iUL, 8).Equals(@"\ulnone "))
                    testo = testo.Remove(iUL, 8);
                else
                {
                    iPlain = testo.IndexOf(@"\plain", iUL);
                    iUL0 = testo.IndexOf(@"\ul0", iUL);
                    iULnone = testo.IndexOf(@"\ulnone", iUL);
                    if (iPlain > 0 && (iPlain < iUL0 || iUL0 < 0) && (iPlain < iULnone || iULnone < 0))
                        testo = testo.Substring(0, iUL) + "<span style=\"text-decoration:underline;\">" + testo.Substring(iUL + 3, iPlain - iUL - 3) + "</span>" + testo.Substring(iPlain);
                    else if (iUL0 > 0 && (iUL0 < iULnone || iULnone < 0))
                        testo = testo.Substring(0, iUL) + "<span style=\"text-decoration:underline;\">" + testo.Substring(iUL + 3, iUL0 - iUL - 3) + "</span>" + testo.Substring(iUL0 + 5);
                    else if (iULnone > 0)
                        testo = testo.Substring(0, iUL) + "<span style=\"text-decoration:underline;\">" + testo.Substring(iUL + 3, iULnone - iUL - 3) + "</span>" + testo.Substring(iULnone + 7);
                    else
                        testo = testo.Substring(0, iUL) + testo.Substring(iUL + 3);
                }
            }
            bool inserisciI, inserisciB;
            int iI, iI1, iB, iB1;
            while (testo.IndexOf(@"\plain") > -1)
            {
                inserisciB = false;
                inserisciI = false;
                iPlain = testo.IndexOf(@"\plain");
                iI = testo.Substring(0, iPlain).LastIndexOf(@"\i ");
                iI1 = testo.Substring(0, iPlain).LastIndexOf(@"\i<");
                if (iI1 > iI) iI = iI1;
                if (iI > -1 && testo.Substring(0, iPlain).LastIndexOf(@"\i0") < iI)
                    inserisciI = true;
                iB = testo.Substring(0, iPlain).LastIndexOf(@"\b ");
                iB1 = testo.Substring(0, iPlain).LastIndexOf(@"\b<");
                if (iB1 > iB) iB = iB1;
                if (iB > -1 && testo.Substring(0, iPlain).LastIndexOf(@"\b0") < iB)
                    inserisciB = true;
                testo = testo.Substring(0, iPlain) + (inserisciB ? "\\b0" : "") + (inserisciI ? "\\i0" : "") + testo.Substring(iPlain + 6); // \i è rimosso più avanti
            }

            //            testo = SostituisciTagSingolo(testo, "plain", "");
            testo = SostituisciTagParentesi(testo, "caps", ""); // possibile fare con <span style="font-variant: small-caps;">...</span> oppure text-transform:uppercase
            testo = SostituisciTagSingolo(testo, "caps1", "");
            testo = SostituisciTagSingolo(testo, "caps0", "");
            testo = SostituisciTagSingolo(testo, "caps", "");
            testo = SostituisciTagNumeri(testo, "cellx");
            testo = SostituisciTagNumeri(testo, "brdrw");
            testo = SostituisciTagNumeri(testo, "brdrcf");
            testo = SostituisciTagNumeri(testo, "clshdng");
            testo = SostituisciTagNumeri(testo, "clcfpat");
            testo = SostituisciTagNumeri(testo, "clcbpat");
            testo = SostituisciTagSingolo(testo, "brdrs", "");
            testo = SostituisciTagSingolo(testo, "brdrdash", "", false);
            testo = SostituisciTagSingolo(testo, "clbrdrb", "");
            testo = SostituisciTagSingolo(testo, "clbrdrl", "");
            testo = SostituisciTagSingolo(testo, "clbrdrr", "");
            testo = SostituisciTagSingolo(testo, "clbrdrt", "", false);
            testo = SostituisciTagSingolo(testo, "trbrdrh", "");
            testo = SostituisciTagSingolo(testo, "trbrdrv", "");
            testo = SostituisciTagSingolo(testo, "trbrdrt", "", false);
            testo = SostituisciTagSingolo(testo, "trbrdrl", "", false);
            testo = SostituisciTagSingolo(testo, "trbrdrr", "", false);
            testo = SostituisciTagSingolo(testo, "trbrdrb", "", false);
            testo = testo.Replace("\\brdrb \\brsp20 ", "");
            testo = SostituisciTagNumeri(testo, "trgaph");
            testo = SostituisciTagNumeri(testo, "trleft");
            testo = SostituisciTagNumeri(testo, "trleft-");
            testo = SostituisciTagSingolo(testo, "trowd", ""); // prima di cell
            testo = SostituisciTagSingolo(testo, "trkeep", "");
            testo = SostituisciTagSingolo(testo, "emdash", "&mdash;");
            testo = SostituisciTagSingolo(testo, "endash", "&ndash;");
            testo = SostituisciTagSingolo(testo, "ldblquote", "&ldquo;", false);
            testo = SostituisciTagSingolo(testo, "rdblquote", "&rdquo;", false);
            testo = SostituisciTagSingolo(testo, "lquote", "&lsquo;");
            testo = SostituisciTagSingolo(testo, "rquote", "&rsquo;", false);
            testo = SostituisciTagSingolo(testo, "b", "<b>");
            testo = SostituisciTagSingolo(testo, "b1", "<b>");
            testo = SostituisciTagSingolo(testo, "b0", "</b>", false);
            testo = SostituisciTagSingolo(testo, "i", "<i>");
            testo = SostituisciTagSingolo(testo, "i1", "<i>");
            testo = SostituisciTagSingolo(testo, "i0", "</i>", false);
            testo = SostituisciTagSingolo(testo, "intbl", "", false); // dopo i
            testo = SostituisciTagParentesi(testo, "super", "sup");
            testo = SostituisciTagSingolo(testo, "super", "<sup>", false);
            testo = SostituisciTagSingolo(testo, "nosupersub", "</sup>", false);
            testo = SostituisciTagSingolo(testo, "up12", "<sup>");
            testo = SostituisciTagSingolo(testo, "dn4", "<sup>");
            testo = SostituisciTagSingolo(testo, "up0", "</sup>");
            //            testo = SostituisciTagNumeri(testo, "cf"); // sposato prima nella routine, perché a volte dentro un link che rovinava la conversione
            /*testo = SostituisciTagSingolo(testo, "cf0", "", false); // prima di f0
            testo = SostituisciTagSingolo(testo, "cf1", "", false);
            testo = SostituisciTagSingolo(testo, "cf2", "", false);
            testo = SostituisciTagSingolo(testo, "cf3", "", false);
            testo = SostituisciTagSingolo(testo, "cf4", "", false);
            testo = SostituisciTagSingolo(testo, "cf8", "", false);*/
            testo = SostituisciTagSingolo(testo, "f10", "");
            testo = SostituisciTagSingolo(testo, "f11", "");
            testo = SostituisciTagSingolo(testo, "f12", "");
            testo = SostituisciTagSingolo(testo, "f0", "", false);
            testo = SostituisciTagSingolo(testo, "f1", "");
            testo = SostituisciTagSingolo(testo, "f2", "", false);
            testo = SostituisciTagSingolo(testo, "f3", "", false);
            testo = SostituisciTagSingolo(testo, "f4", "", false);
            testo = SostituisciTagSingolo(testo, "f5", "", false);
            testo = SostituisciTagSingolo(testo, "f6", "");
            testo = SostituisciTagSingolo(testo, "f7", "");
            testo = SostituisciTagSingolo(testo, "f8", "");
            testo = SostituisciTagSingolo(testo, "f9", "");

            testo = ConvAHTMLCharEntity(testo); // deve essere dopo /up..., /ul e dopo /f1 ecc

            testo = SostituisciTagNumeri(testo, "fs");
            testo = SostituisciTagSingolo(testo, "qj", "", false); // giustificazione: si potrebbe fare (ma la fine della giustificazione è difficile da trovare)
            testo = SostituisciTagSingolo(testo, "qc", "", false);
            testo = SostituisciTagSingolo(testo, "qr", "");
            testo = SostituisciTagSingolo(testo, "pagebb", "");
            testo = SostituisciTagSingolo(testo, "ltrpar", "", false);
            testo = SostituisciTagSingolo(testo, "ltrch", "", false);
            testo = SostituisciTagSingolo(testo, "rtlch", "", false);
            testo = SostituisciTagSingolo(testo, "tqr", "", false);
            testo = SostituisciTagSingolo(testo, "nowidctlpar", "", false);
            testo = SostituisciTagSingolo(testo, "line", "<br />", false);
            testo = SostituisciTagSingolo(testo, "keepn", "", false);
            testo = SostituisciTagNumeri(testo, "slmult");
            testo = SostituisciTagNumeri(testo, "sl");
            testo = SostituisciTagNumeri(testo, "tx");
            testo = SostituisciTagNumeri(testo, "sa");
            testo = SostituisciTagNumeri(testo, "sb");
            testo = SostituisciTagNumeri(testo, "kerning");
            testo = SostituisciTagNumeri(testo, "lang");
            testo = SostituisciTagNumeri(testo, "s");
            testo = SostituisciTagNumeri(testo, "li");
            testo = SostituisciTagNumeri(testo, "ri");
            testo = SostituisciTagNumeri(testo, "fi-");
            testo = SostituisciTagNumeri(testo, "fi");
            testo = testo.Replace("{\\*\\pn\\pnlvlblt\\pnf1\\pnindent200{\\pntxtb&#183;}}", "");
            testo = testo.Replace("{\\*\\pn\\pnlvlblt\\pnf3\\pnindent0{\\pntxtb&#183;}}", "");
            testo = testo.Replace("{\\pntext\\f1&#183;\\tab}", "&#183;&nbsp;");
            testo = testo.Replace("{\\pntext&#183;\\tab}", "&#183;&nbsp;");
            testo = testo.Replace("\\bullet ", "&#183;&nbsp;");
            testo = SostituisciTagSingolo(testo, "tab", "&nbsp;&nbsp;&nbsp;", false);
            testo = SostituisciTagSingolo(testo, "cell", "</td><td>", false);
            testo = SostituisciTagSingolo(testo, "row", "</tr><tr>", false);
            testo = testo.Replace("\\b<", "<b><");
            testo = testo.Replace("\\i<", "<i><");
            testo = testo.Replace("\\i&", "<i>&");
            testo = testo.Replace("{\\f1{&#8237;&#1488;}}", "&#8237;&#1488;");
            testo = SostituisciTagSingolo(testo, "f1", "", false);
            testo = testo.Replace("\\{", "{");
            testo = testo.Replace("\\}", "}");
            testo = testo.Replace("\\-", ""); // un errore in Manoscritti
            if (testo.EndsWith("\\super"))
                testo = testo.Substring(0, testo.Length - 6);
            if (testo.EndsWith("\\f6"))
                testo = testo.Substring(0, testo.Length - 3);
            if (testo.EndsWith("\\i"))
                testo = testo.Substring(0, testo.Length - 2);
            if (testo.Contains("</td>"))
                testo = testo.Substring(0, testo.IndexOf("</td>")) + "<table><tr>" + testo.Substring(testo.IndexOf("</td>") + 5);
            if (testo.Contains("<tr>"))
                testo = testo.Substring(0, testo.LastIndexOf("<tr>")) + "</table>" + testo.Substring(testo.LastIndexOf("<tr>") + 4);
            testo = testo.Replace("<td></tr>", "</tr>");
            testo = testo.Replace("</tr><tr>", "</tr><tr><td>");
            testo = testo.Trim();
            if (testo.LastIndexOf("<i>") > testo.LastIndexOf("</i>"))
                testo += "</i>";
            if (testo.IndexOf("\\", StringComparison.OrdinalIgnoreCase) >= 0)
                inizioTag = 0;
            // link href=laparola per link ad altre note, non solo a brano
            // "\\fs24\\cf0  a" toglie due spazi invece di uno, per esempio in NNR

            return testo.Trim();
        }

        private static string SostituisciTagSingolo(string testo, string tag, string tagNuovo)
        {
            return SostituisciTagSingolo(testo, tag, tagNuovo, true);
        }

        private static string SostituisciTagSingolo(string testo, string tag, string tagNuovo, bool soloConSpazioDopo)
        {
            //            testo.Replace(@"\" + tag + @"\", tagNuovo).Replace(@"\" + tag + (conSpazio ? " " : ""), tagNuovo);
            //            while (testo.IndexOf(@"\" + tag + (conSpazio ? " " : ""), StringComparison.OrdinalIgnoreCase) >= 0)
            //                testo = testo.Substring(0, testo.IndexOf(@"\" + tag + (conSpazio ? " " : ""), StringComparison.OrdinalIgnoreCase)) + tagNuovo + testo.Substring(testo.IndexOf(@"\" + tag + (conSpazio ? " " : ""), StringComparison.OrdinalIgnoreCase) + tag.Length + 2);
            String t = @"\" + tag;
            testo = testo.Replace(t + @"\", tagNuovo + @"\").Replace(t + " ", tagNuovo);
            if (!soloConSpazioDopo)
                testo = testo.Replace(t, tagNuovo);
            return testo;
        }

        private static string SostituisciTagNumeri(string testo, string tag)
        {
            string tag2 = @"\" + tag;
            string casuale = "q$lpn£4";
            int i, j;
            while (testo.IndexOf(tag2) >= 0)
            {
                i = testo.IndexOf(tag2);
                if (!Char.IsDigit(testo[i + tag2.Length]))
                {
                    testo = testo.Insert(i + 1, casuale);
                }
                else
                {
                    j = i + tag2.Length + 1;
                    while (j < testo.Length && Char.IsDigit(testo[j]))
                        j += 1;
                    if (j < testo.Length && testo[j] == ' ')
                        j += 1;
                    testo = testo.Remove(i, j - i);
                }
            }
            return testo.Replace(casuale, "");
        }

        private static string SostituisciTagParentesi(string testo, string tag, string tagNuovo)
        {
            string tagNuovo1 = tagNuovo, tagNuovo2 = tagNuovo;
            if (!string.IsNullOrEmpty(tagNuovo))
            {
                tagNuovo1 = "<" + tagNuovo1 + ">";
                tagNuovo2 = "</" + tagNuovo2 + ">";
            }
            int fineTag, inizioTag;
            while (testo.IndexOf(@"{\" + tag + " ", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                inizioTag = testo.IndexOf(@"{\" + tag + " ", StringComparison.OrdinalIgnoreCase);
                fineTag = testo.IndexOf("}", inizioTag, StringComparison.OrdinalIgnoreCase);
                testo = testo.Substring(0, inizioTag) + tagNuovo1 + testo.Substring(inizioTag + tag.Length + 3, fineTag - inizioTag - tag.Length - 3) + tagNuovo2 + testo.Substring(fineTag + 1);
            }
            return testo;
        }

        private static long ScriviDatiJava(VersioneInformazioni info, BinaryWriter w, bool bibbia)
        {
            w.Write(new char[] { 'L', 'P', 'N', (char)1, (char)1, (char)22 });
            int pInizioVersione = 10;
            w.Write(Intabyte4(pInizioVersione));

            w.Write(Intabyte4(0));
            w.Write(info.Nome.ToCharArray());
            //w.Write(ConvAHTMLCharEntity(info.Nome).ToCharArray());
            w.Write((char)0);
            w.Write(info.Abbreviazione.ToCharArray());
            w.Write((char)0);
            w.Write(info.Titolo.ToCharArray());
            //w.Write(ConvAHTMLCharEntity(info.Titolo).ToCharArray());
            w.Write((char)0);
            w.Write(info.Autore.ToCharArray());
            //w.Write(ConvAHTMLCharEntity(info.Autore).ToCharArray());
            w.Write((char)0);
            w.Write(info.CasaEditrice.ToCharArray());
            //w.Write(ConvAHTMLCharEntity(info.CasaEditrice).ToCharArray());
            w.Write((char)0);
            w.Write(info.Data.ToCharArray());
            w.Write((char)0);
            w.Write(info.Copyright.ToCharArray());
            //w.Write(ConvAHTMLCharEntity(info.Copyright).ToCharArray());
            w.Write((char)0);
            w.Write(info.Isbn.ToCharArray());
            w.Write((char)0);
            string desc = info.Descrizione;
            if (desc.StartsWith(@"{\rtf"))
            {
                RichTextBoxEx rtDesc = new RichTextBoxEx
                {
                    Rtf = desc
                };
                desc = rtDesc.Text;
            }
            //w.Write(ConvAHTMLCharEntity(desc).ToCharArray());
            w.Write(desc.ToCharArray());
            w.Write((char)0);
            w.Write(info.Lingua.ToCharArray());
            w.Write((char)0);
            w.Write((char)(bibbia ? 0 : 1)); // indica tipo Bibbia; bisogna mettere 1 se non è Bibbia

            long pInizioDati = w.Seek(0, SeekOrigin.Current);
            w.Seek(pInizioVersione, SeekOrigin.Begin);
            w.Write(Intabyte4(pInizioDati));
            w.Seek(0, SeekOrigin.End);

            w.Write(Intabyte4(pInizioDati + 40 + (bibbia ? 0 : 8))); // inizio del testo
            w.Write(Intabyte4(0)); // inizio indici libri e capitoli/inizio titoli note
            w.Write(Intabyte4(0)); // inizio indice versetti/note
            w.Write(Intabyte4(0)); // inizio elenco parole
            w.Write(Intabyte4(0)); // inizio indice dell'indice delle parole
            w.Write(Intabyte4(0)); // inizio indice delle parole
            w.Write(Intabyte4(0)); // inizio elenco radici
            w.Write(Intabyte4(0)); // inizio elenco parole delle radici
            w.Write(Intabyte4(0)); // inizio elenco radici diverse
            w.Write(Intabyte4(0)); // inizio elenco differenze nei riferimenti

            return pInizioDati;
        }

        private static string ConvAHTMLCharEntity(string testo)
        {
            for (int iLettera = testo.Length - 1; iLettera >= 0; --iLettera)
            {
                if (testo[iLettera] > (char)127)
                    testo = testo.Substring(0, iLettera) + "&#" + Convert.ToUInt32(testo[iLettera]) + ";" + testo.Substring(iLettera + 1);
            }
            int instr, instr2;
            while (testo.IndexOf(@"\'") > -1)
            {
                instr = testo.IndexOf(@"\'");
                testo = testo.Substring(0, instr) + "&#" + (Uri.FromHex(testo[instr + 2]) * 16 + Uri.FromHex(testo[instr + 3])) + ";" + testo.Substring(instr + 4);
            }
            while (testo.IndexOf(@"\u") > -1)
            {
                instr = testo.IndexOf(@"\u");
                instr2 = testo.IndexOf("?", instr);
                if (instr2 > 0)
                    testo = testo.Substring(0, instr) + "&#" + testo.Substring(instr + 2, instr2 - instr - 2) + ";" + testo.Substring(instr2 + 1);
                else
                    testo = testo.Substring(0, instr) + "&#" + testo.Substring(instr + 2, 4) + ";" + testo.Substring(instr + 7);
            }

            // per ebraico traslitterato, dove il font per Android manca dei caratteri
            testo = testo.Replace("&#702;", "&#1158;").Replace("&#703;", "&#1157;").Replace("&#7829;", "&#382;").Replace("&#7830;", "&#295;");

            return testo;
        }

        private static byte[] Intabyte4(long p)
        {
            byte[] b = new byte[4];
            b[3] = (byte)(p % 256);
            p = (p - b[3]) / 256;
            b[2] = (byte)(p % 256);
            p = (p - b[2]) / 256;
            b[1] = (byte)(p % 256);
            b[0] = (byte)((p - b[1]) / 256);
            return b;
        }

        private static int ConvertiLibro73A66Zefania(int libro)
        {
            // 17 Tobia -> 69
            // 18 Giuditta -> 67
            // 20 1M -> 72
            // 21 2M -> 73
            // 27 Sapienza -> 68
            // 28 Sirach -> 70
            // 32 Baruc -> 71
            if (libro <= 16)
                return libro;
            if (libro == 17)
                return 69;
            if (libro == 18)
                return 67;
            if (libro == 19)
                return 17;
            if (libro <= 21)
                return libro + 52;
            if (libro <= 26)
                return libro - 4;
            if (libro == 27)
                return 68;
            if (libro == 28)
                return 70;
            if (libro <= 31)
                return libro - 6;
            if (libro == 32)
                return 71;
            return libro - 7;
        }

        private static int ConvertiLibro73A66(int libro)
        {
            if (libro <= 16)
                return libro;
            if (libro <= 18)
                return libro + 50;
            if (libro == 19)
                return 17;
            if (libro <= 21)
                return libro + 49;
            if (libro <= 26)
                return libro - 4;
            if (libro <= 28)
                return libro + 44;
            if (libro <= 31)
                return libro - 6;
            if (libro == 32)
                return 73;
            return libro - 7;
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
