using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using LaParola.Properties;
using TestiBiblici;

namespace LaParola
{
    public enum TipoImportazione
    {
        ImportaBibbia,
        ImportaZefania,
        ImportaBibleworks,
        ImportaNote,
        ImportaThml,
        ImportaEsword,
        NuovaNote
    }

    public partial class ImportaBibbia : Template
    {
        private enum TipoThML
        {
            Bibbia,
            Commentario,
            Altro
        }

        private enum TipoEsword
        {
            Bibbia,
            Commentario,
            Dizionario,
            Tema
        }

        private Principale genitore;
        private TipoImportazione tipo;
        private bool inThread;
        private TipoThML thmlTipo = TipoThML.Altro;
        private TipoEsword eswordTipo;
        private DataSet dataSetEsword = null;
        internal string NomeVersione = "";
        private const string nessunoTrovato = "nessunoTrovato";

        #region Costruttori

        public ImportaBibbia(Principale formGenitore, TipoImportazione tipoImportazione)
        {
            Costruttore(formGenitore, tipoImportazione, true);
        }

        public ImportaBibbia(Principale formGenitore, TipoImportazione tipoImportazione, bool eseguireInThread)
        {
            Costruttore(formGenitore, tipoImportazione, eseguireInThread);
        }

        private void Costruttore(Principale formGenitore, TipoImportazione tipoImportazione, bool eseguireInThread)
        {
            genitore = formGenitore;
            tipo = tipoImportazione;
            inThread = eseguireInThread && !Principale.isRunningOnMono;
            InitializeComponent();
        }

        private void ImportaBibbia_Load(object sender, EventArgs e)
        {
            guidaFile.HelpNamespace = genitore.NomeFileGuida();

            this.ActiveControl = cbNomeFileXmlODirectory;
            switch (tipo)
            {
                case TipoImportazione.ImportaBibbia:
                case TipoImportazione.ImportaZefania:
                case TipoImportazione.ImportaBibleworks:
                case TipoImportazione.ImportaThml:
                    labNomeCartella.Visible = false;
                    labNomeNuovaCollezione.Visible = false;
                    cbNomeFileXmlODirectory.Items.AddRange(Settings.Default.ImportaFilePrecedenti.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
                    tbNuovaCollezione.Visible = false;
                    labVersioneDelleNote.Visible = false;
                    tbVersioneDelleNote.Visible = false;
                    break;
                case TipoImportazione.ImportaNote:
                    labNomeFileXml.Visible = false;
                    labNomeNuovaCollezione.Visible = false;
                    cbNomeFileXmlODirectory.Items.AddRange(Settings.Default.ImportaDirectoryPrecedenti.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
                    tbNuovaCollezione.Visible = false;
                    break;
                case TipoImportazione.ImportaEsword:
                    labNomeCartella.Visible = false;
                    labNomeNuovaCollezione.Visible = false;
                    cbNomeFileXmlODirectory.Items.AddRange(Settings.Default.ImportaEswordPrecedenti.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
                    tbNuovaCollezione.Visible = false;
                    labVersioneDelleNote.Visible = false;
                    tbVersioneDelleNote.Visible = false;
                    break;
                case TipoImportazione.NuovaNote:
                    labNomeCartella.Visible = false;
                    labNomeFileXml.Visible = false;
                    cbNomeFileXmlODirectory.Visible = false;
                    btnSfogliaFileXmlODirectory.Visible = false;
                    this.ActiveControl = tbNuovaCollezione;

                    tbAbbreviazione.Enabled = true;
                    tbNomeFileLP.Enabled = true;
                    tbTitolo.Enabled = true;
                    tbAutore.Enabled = true;
                    tbCasaEd.Enabled = true;
                    tbCopyright.Enabled = true;
                    tbDescrizione.Enabled = true;
                    tbData.Enabled = true;
                    tbISBN.Enabled = true;
                    tbLingua.Enabled = true;
                    tbVersioneDelleNote.Enabled = true;

                    break;
                default:
                    break;
            }
        }

        #endregion

        struct ThreadArgomenti
        {
            public string fileDaAnalizzare;
            public string nomeFileLP;
            public string abbreviazione;
            public string titolo;
            public string autore;
            public string casaEditrice;
            public string data;
            public string copyright;
            public string isbn;
            public string descrizione;
            public string lingua;
            public string versioneDelleNote;
            public TipoImportazione tipo;
            public BarraConEtichetta barra;
            public CultureInfo culturaInterfaccia;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            ThreadArgomenti argomenti = new ThreadArgomenti();
            if (inThread)
                argomenti.barra = genitore.CreaBarraDiStato(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("ImportCurrent"), tbNomeFileLP.Text), 0, 128); // 73 per i libri, 55 per le altre cose
            string fileDaAnalizzare = (tipo == TipoImportazione.NuovaNote ? tbNuovaCollezione.Text : cbNomeFileXmlODirectory.Text);
            argomenti.fileDaAnalizzare = fileDaAnalizzare;
            argomenti.nomeFileLP = tbNomeFileLP.Text;
            argomenti.abbreviazione = tbAbbreviazione.Text;
            argomenti.titolo = tbTitolo.Text;
            argomenti.autore = tbAutore.Text;
            argomenti.casaEditrice = tbCasaEd.Text;
            argomenti.data = tbData.Text;
            argomenti.copyright = tbCopyright.Text;
            argomenti.isbn = tbISBN.Text;
            argomenti.descrizione = tbDescrizione.Text;
            argomenti.lingua = tbLingua.Text;
            argomenti.versioneDelleNote = tbVersioneDelleNote.Text;
            argomenti.tipo = tipo;
            argomenti.culturaInterfaccia = Thread.CurrentThread.CurrentUICulture;

            BackgroundWorker backgroundWorker = new BackgroundWorker();
            if (!inThread)
            {
                DoWorkEventArgs e1 = new DoWorkEventArgs(argomenti);
                AnalizzaFileXmlODirectory(backgroundWorker, e1);
                RunWorkerCompletedEventArgs e2 = new RunWorkerCompletedEventArgs(argomenti, null, false);
                AnalizzatoFileXmlODirectory(backgroundWorker, e2);
            }
            else
            {
                backgroundWorker.WorkerReportsProgress = true;
                backgroundWorker.WorkerSupportsCancellation = true;
                backgroundWorker.DoWork += new DoWorkEventHandler(AnalizzaFileXmlODirectory);
                backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(AnalizzaFileXmlODirectoryProgresso);
                backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(AnalizzatoFileXmlODirectory);
                backgroundWorker.RunWorkerAsync(argomenti);
            }

            if (tipo != TipoImportazione.NuovaNote)
            {
                if (cbNomeFileXmlODirectory.Items.IndexOf(fileDaAnalizzare) > -1)
                    cbNomeFileXmlODirectory.Items.Remove(fileDaAnalizzare);
                cbNomeFileXmlODirectory.Items.Insert(0, fileDaAnalizzare);
                int nFileDaSalvare = Settings.Default.MiscRichiesteMemorizzate;
                if (cbNomeFileXmlODirectory.Items.Count < nFileDaSalvare)
                    nFileDaSalvare = cbNomeFileXmlODirectory.Items.Count;
                StringBuilder fileDaSalvare = new StringBuilder("");
                for (int i = 0; i < nFileDaSalvare; ++i)
                    fileDaSalvare.Append("|").Append(cbNomeFileXmlODirectory.Items[i]);
                switch (tipo)
                {
                    case TipoImportazione.ImportaBibbia:
                    case TipoImportazione.ImportaZefania:
                    case TipoImportazione.ImportaBibleworks:
                    case TipoImportazione.ImportaThml:
                        Settings.Default.ImportaFilePrecedenti = fileDaSalvare.ToString();
                        break;
                    case TipoImportazione.ImportaEsword:
                        Settings.Default.ImportaEswordPrecedenti = fileDaSalvare.ToString();
                        break;
                    case TipoImportazione.ImportaNote:
                        Settings.Default.ImportaDirectoryPrecedenti = fileDaSalvare.ToString();
                        break;
                }
            }
            this.Close();
        }

        private void AnalizzaFileXmlODirectory(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = null;
            if (inThread)
                worker = sender as BackgroundWorker;
            ThreadArgomenti argomenti = (ThreadArgomenti)e.Argument;
            // fa sì che i messaggi di errore in questo thread siano nella lingua dell'interfaccia del programma
            Thread.CurrentThread.CurrentUICulture = argomenti.culturaInterfaccia;

            FileStream fs = null;
            BinaryWriter bw = null;
            ConfrontoCI confrontoParole = new ConfrontoCI();

            SortedDictionary<string, List<OccorrenzaParola>> chiave = new SortedDictionary<string, List<OccorrenzaParola>>(confrontoParole);

            string[] fileNote = new string[0];

            try
            {
                if (inThread)
                    worker.ReportProgress(1, argomenti.barra);

                fs = new FileStream(argomenti.nomeFileLP, FileMode.Create, FileAccess.Write);
                bw = new BinaryWriter(fs);
                char[] inizioFile = { 'L', 'P', 'N',
                    Convert.ToChar(Assembly.GetExecutingAssembly().GetName().Version.Major), Convert.ToChar(Assembly.GetExecutingAssembly().GetName().Version.Minor), Convert.ToChar(Assembly.GetExecutingAssembly().GetName().Version.Build), (char)1};
                bw.Write(inizioFile);
                bw.Write((UInt32)11);
                bw.Write((UInt32)0);

                bw.Write(Path.GetFileNameWithoutExtension(argomenti.nomeFileLP)); // nomeTesto
                bw.Write(argomenti.abbreviazione);
                bw.Write(argomenti.titolo);
                bw.Write(argomenti.autore);
                bw.Write(argomenti.casaEditrice);
                bw.Write(argomenti.data);
                bw.Write(argomenti.copyright);
                bw.Write(argomenti.isbn);
                bw.Write(argomenti.descrizione);
                bw.Write(argomenti.lingua);
                bw.Write(argomenti.versioneDelleNote);
                bw.Write((byte)BloccatoTipi.Sbloccato);
                byte tipoTestoDaScrivere = (argomenti.tipo == TipoImportazione.ImportaNote || argomenti.tipo == TipoImportazione.NuovaNote || (argomenti.tipo == TipoImportazione.ImportaThml && thmlTipo != TipoThML.Bibbia) || (argomenti.tipo == TipoImportazione.ImportaEsword && eswordTipo != TipoEsword.Bibbia) ? (byte)1 : (byte)0);
                bw.Write(tipoTestoDaScrivere);
                UInt32 pInizioDati = (UInt32)(bw.Seek(0, SeekOrigin.Current));
                bw.Seek(11, SeekOrigin.Begin);
                bw.Write(pInizioDati);
                bw.Seek(0, SeekOrigin.End);

                if (inThread)
                    worker.ReportProgress(2, argomenti.barra);

                chiave.Clear();
                UInt32 inizioTestoIndiceLC = 0, inizioTestoIndice = 0;
                UInt32 inizioTesto = pInizioDati + 44; // '44' va cambiato qui, nella riga successiva, e 2 volte in testi.cs::Versione.Chiudi
                bw.Write((UInt32)44); // inizio del testo
                bw.Write((UInt32)0); // inizio indici libri e capitoli/inizio titoli note
                bw.Write((UInt32)0); // inizio indice versetti/note
                bw.Write((UInt32)0); // inizio elenco parole
                bw.Write((UInt32)0); // inizio indice dell'indice delle parole
                bw.Write((UInt32)0); // inizio indice delle parole
                bw.Write((UInt32)0); // inizio elenco radici
                bw.Write((UInt32)0); // inizio elenco radici diverse
                bw.Write((UInt32)0); // inizio elenco differenze nei riferimenti
                bw.Write((UInt32)0); // inizio indice dei riferimenti citati
                bw.Write((UInt32)0); // inizio elenco note in ordine

                string[] noteTesto = null;
                string[] noteInOrdine = null;
                UInt32[] indici = new UInt32[2];
                RichTextBoxEx rtb = new RichTextBoxEx();
                UInt32 numeroVersetto = 0;
                string rif, libro, libroPrecedente = nessunoTrovato;
                List<byte> capitoliInLibri = new List<byte>();
                List<byte> versettiInCapitoli = new List<byte>();
                List<int> indice = new List<int>();
                byte capitolo = 0, versetto = 0, capitoloPrecedente = 0;
                int numeroLibro = 0, numeroLibroPrecedente = 0, versettoPrecedente = 0;

                switch (argomenti.tipo)
                {
                    #region Importa Bibbia (OSIS)
                    case TipoImportazione.ImportaBibbia:
                        XmlDocument xmlDocumento = new XmlDocument();
                        xmlDocumento.Load(argomenti.fileDaAnalizzare);
                        XmlNamespaceManager nspmgr = new XmlNamespaceManager(xmlDocumento.NameTable);
                        nspmgr.AddNamespace("nsp", xmlDocumento.ChildNodes[1].NamespaceURI);

                        XmlNodeList nl = xmlDocumento.DocumentElement.SelectNodes("nsp:osisText/nsp:div/nsp:div/nsp:chapter/nsp:verse", nspmgr);
                        if (nl.Count == 0) // alcuni documenti OSIS non usano una sezione div per i Testamenti
                            nl = xmlDocumento.DocumentElement.SelectNodes("nsp:osisText/nsp:div/nsp:chapter/nsp:verse", nspmgr);
                        if (nl.Count == 0) // alcuni documenti OSIS usano una div per le sezioni del testo
                            nl = xmlDocumento.DocumentElement.SelectNodes("nsp:osisText/nsp:div/nsp:chapter/nsp:div/nsp:p/nsp:verse", nspmgr);
                        if (nl.Count == 0) // alcuni documenti OSIS usano una sezione div per un capitolo, invece di una sezione chapter
                            nl = xmlDocumento.DocumentElement.SelectNodes("nsp:osisText/nsp:div/nsp:div/nsp:div/nsp:verse", nspmgr);
                        string[] testoAnalizzato; // primo elemento è RTF, secondo è testo normale
                        int punto1, punto2;

                        Dictionary<string, int> libriOSIS = new Dictionary<string, int>();
                        string[] libriOSISArray = {"", "Gen", "Exod", "Lev", "Num", "Deut",
                     "Josh", "Judg", "Ruth", "1Sam", "2Sam", "1Kgs", "2Kgs", "1Chr", "2Chr",
                     "Ezra", "Neh", "Tob", "Jdt", "Esth", "1Macc", "2Macc",
                     "Job", "Ps", "Prov", "Eccl", "Song", "Wis", "Sir",
                     "Isa", "Jer", "Lam", "Bar", "Ezek", "Dan",
                     "Hos", "Joel", "Amos", "Obad", "Jonah", "Mic", "Nah", "Hab", "Zeph", "Hag", "Zech", "Mal",
                     "Matt", "Mark", "Luke", "John", "Acts",
                     "Rom", "1Cor", "2Cor", "Gal", "Eph", "Phil", "Col", "1Thess", "2Thess", "1Tim", "2Tim", "Titus", "Phlm",
                     "Heb", "Jas", "1Pet", "2Pet", "1John", "2John", "3John", "Jude", "Rev"};
                        for (int i = 1; i <= 73; ++i)
                            libriOSIS.Add(libriOSISArray[i], i);

                        foreach (XmlNode xn in nl)
                        {
                            if (xn.Attributes["eID"] == null) // questo file ha <verse sID=...> e <verse eID=...> per ogni versetto
                            {
                                rif = xn.Attributes["osisID"].Value;
                                punto1 = rif.IndexOf(".", StringComparison.Ordinal);
                                punto2 = rif.LastIndexOf(".", StringComparison.Ordinal);
                                libro = rif.Substring(0, punto1);
                                if (libro != libroPrecedente)
                                {
                                    numeroLibro = libriOSIS[libro]; // può dare exception se libro non è un libro riconosciuto; c'è il catch e il messaggio in questo caso
                                    if (libroPrecedente != nessunoTrovato)
                                        capitoliInLibri.Add(capitolo); // il numero di capitoli nel libro precedente
                                    for (int i = 0; i < numeroLibro - numeroLibroPrecedente - 1; i++)
                                        capitoliInLibri.Add(0);
                                    if (inThread)
                                        worker.ReportProgress(numeroLibro + 1, argomenti.barra);
                                    libroPrecedente = libro;
                                    numeroLibroPrecedente = numeroLibro;
                                    if (capitoloPrecedente == 1)
                                    {
                                        // per i libri con uno solo capitolo - quando si va al libro successivo non si cambia il numero del capitolo
                                        versettiInCapitoli.Add(versetto);
                                        versettoPrecedente = 0;
                                    }
                                }
                                capitolo = Convert.ToByte(rif.Substring(punto1 + 1, punto2 - punto1 - 1), CultureInfo.InvariantCulture);
                                if (capitolo != capitoloPrecedente)
                                {
                                    if (capitoloPrecedente != 0)
                                        versettiInCapitoli.Add(versetto);
                                    capitoloPrecedente = capitolo;
                                    versettoPrecedente = 0;
                                }
                                versetto = Convert.ToByte(rif.Substring(punto2 + 1), CultureInfo.InvariantCulture);
                                for (int i = versettoPrecedente + 1; i < versetto; ++i)
                                { // versetti mancanti
                                    ++numeroVersetto;
                                    indice.Add((int)(bw.Seek(0, SeekOrigin.Current) - inizioTesto)); // -8 perché inizioBibbiaTesto è 8 dopo inizioBibbia
                                    bw.Write("");
                                }
                                versettoPrecedente = versetto;
                                if (xn.Attributes["sID"] != null)
                                    testoAnalizzato = ConvertiOsisARtfETesto(xn.NextSibling);
                                else
                                    testoAnalizzato = ConvertiOsisARtfETesto(xn);
                                if (versetto == 1 && testoAnalizzato[0].StartsWith(@"\par", StringComparison.OrdinalIgnoreCase))
                                    testoAnalizzato[0] = testoAnalizzato[0].Substring(4).Trim();
                                indice.Add((int)(bw.Seek(0, SeekOrigin.Current) - inizioTesto)); // -8 perché inizioBibbiaTesto è 8 dopo inizioBibbia
                                bw.Write(testoAnalizzato[0]);
                                ++numeroVersetto;

                                chiave = Texts.TrovaParoleInVoce(testoAnalizzato[1], numeroVersetto, chiave, argomenti.lingua);
                            }
                        }

                        versettiInCapitoli.Add(versetto);
                        capitoliInLibri.Add(capitolo);
                        for (int i = 0; i < 73 - numeroLibro; i++)
                            capitoliInLibri.Add(0);
                        inizioTestoIndiceLC = (UInt32)(bw.Seek(0, SeekOrigin.Current)) - pInizioDati;
                        bw.Write(capitoliInLibri.ToArray());
                        bw.Write(versettiInCapitoli.ToArray());
                        inizioTestoIndice = (UInt32)(bw.Seek(0, SeekOrigin.Current)) - pInizioDati;
                        foreach (int i in indice)
                            bw.Write(i);

                        break;
                    #endregion
                    #region Importa Zefania
                    case TipoImportazione.ImportaZefania:
                        XmlDocument xmlDocumentoZefania = new XmlDocument
                        {
                            PreserveWhitespace = true
                        };
                        xmlDocumentoZefania.Load(argomenti.fileDaAnalizzare);
                        XmlNodeList nlZef = null;
                        nlZef = xmlDocumentoZefania.DocumentElement.SelectNodes("BIBLEBOOK/CHAPTER/VERS");
                        foreach (XmlNode xn in nlZef)
                        {
                            // nota: quando c'è l'apocrifa, bisogna prima modificare il file XML affinché i libri siano nell'ordine di questo programma, non nell'ordine del bnumber di Zefania
                            numeroLibro = ConvertiLibro66A73Zefania(xn.ParentNode.ParentNode.Attributes["bnumber"].Value);
                            if (numeroLibro != numeroLibroPrecedente)
                            {
                                if (numeroLibroPrecedente > 0)
                                    capitoliInLibri.Add(capitolo); // il numero di capitoli nel libro precedente
                                for (int i = 0; i < numeroLibro - numeroLibroPrecedente - 1; i++)
                                    capitoliInLibri.Add(0);
                                if (inThread)
                                    worker.ReportProgress(numeroLibro + 1, argomenti.barra);
                                numeroLibroPrecedente = numeroLibro;
                                if (capitoloPrecedente == 1)
                                {
                                    // per i libri con uno solo capitolo - quando si va al libro successivo non si cambia il numero del capitolo
                                    versettiInCapitoli.Add(versetto);
                                    versettoPrecedente = 0;
                                }
                            }
                            capitolo = Convert.ToByte(xn.ParentNode.Attributes["cnumber"].Value, CultureInfo.InvariantCulture);
                            if (capitolo != capitoloPrecedente)
                            {
                                if (capitoloPrecedente != 0)
                                    versettiInCapitoli.Add(versetto);
                                capitoloPrecedente = capitolo;
                                versettoPrecedente = 0;
                            }
                            versetto = Convert.ToByte(xn.Attributes["vnumber"].Value, CultureInfo.InvariantCulture);
                            for (int i = versettoPrecedente + 1; i < versetto; ++i)
                            { // versetti mancanti
                                ++numeroVersetto;
                                indice.Add((int)(bw.Seek(0, SeekOrigin.Current) - inizioTesto)); // -8 perché inizioBibbiaTesto è 8 dopo inizioBibbia
                                bw.Write("");
                            }
                            versettoPrecedente = versetto;
                            testoAnalizzato = ConvertiZefaniaARtfETesto(xn);
                            if (versetto == 1 && testoAnalizzato[0].StartsWith(@"\par", StringComparison.OrdinalIgnoreCase))
                                testoAnalizzato[0] = testoAnalizzato[0].Substring(4).Trim();
                            indice.Add((int)(bw.Seek(0, SeekOrigin.Current) - inizioTesto)); // -8 perché inizioBibbiaTesto è 8 dopo inizioBibbia
                            bw.Write(testoAnalizzato[0]);
                            ++numeroVersetto;

                            chiave = Texts.TrovaParoleInVoce(testoAnalizzato[1], numeroVersetto, chiave, argomenti.lingua);
                        }

                        versettiInCapitoli.Add(versetto);
                        capitoliInLibri.Add(capitolo);
                        for (int i = 0; i < 73 - numeroLibro; i++)
                            capitoliInLibri.Add(0);
                        inizioTestoIndiceLC = (UInt32)(bw.Seek(0, SeekOrigin.Current)) - pInizioDati;
                        bw.Write(capitoliInLibri.ToArray());
                        bw.Write(versettiInCapitoli.ToArray());
                        inizioTestoIndice = (UInt32)(bw.Seek(0, SeekOrigin.Current)) - pInizioDati;
                        foreach (int i in indice)
                            bw.Write(i);

                        break;
                    #endregion
                    #region Importa BibleWorks
                    case TipoImportazione.ImportaBibleworks:
                        string[] libriBibleworksArray = {"", "Gen", "Exo", "Lev", "Num", "Deu",
                     "Jos", "Jdg", "Rut", "1Sa", "2Sa", "1Ki", "2Ki", "1Ch", "2Ch",
                     "Ezr", "Neh", "Tob", "Jdt", "Est", "1Ma", "2Ma",
                     "Job", "Psa", "Pro", "Ecc", "Sol", "Wis", "Sir",
                     "Isa", "Jer", "Lam", "Bar", "Eze", "Dan",
                     "Hos", "Joe", "Amo", "Oba", "Jon", "Mic", "Nah", "Hab", "Zep", "Hag", "Zec", "Mal",
                     "Mat", "Mar", "Luk", "Joh", "Act",
                     "Rom", "1Co", "2Co", "Gal", "Eph", "Phi", "Col", "1Th", "2Th", "1Ti", "2Ti", "Tit", "Phm",
                     "Heb", "Jam", "1Pe", "2Pe", "1Jo", "2Jo", "3Jo", "Jud", "Rev"};
                        Dictionary<string, int> libriBibleworks = new Dictionary<string, int>();
                        for (int i = 1; i <= 73; ++i)
                            libriBibleworks.Add(libriBibleworksArray[i], i);

                        string testoBibleworks;
                        int spazio;
                        string[] righe = File.ReadAllLines(argomenti.fileDaAnalizzare);

                        // prima di tutto bisognare riordinare le righe, nel caso che non siano nel ordine giusto
                        // (che è probabilmente vero, perché BibleWorks mette 1Mac e 2Mac dopo Malachia invece di dopo Ester).
                        List<string> listaRighe = new List<string>(righe.Length);
                        string numeroLibro2 = "00";
                        foreach (string riga in righe)
                        {
                            if (riga.Trim().Length > 0)
                            {
                                spazio = riga.IndexOf(" ");
                                punto1 = riga.IndexOf(":");
                                punto2 = riga.IndexOf(" ", spazio + 1);
                                libro = riga.Substring(0, spazio);
                                if (libro != libroPrecedente)
                                {
                                    numeroLibro = 0;
                                    if (!libriBibleworks.TryGetValue(libro, out numeroLibro))
                                        numeroLibro = Principale.testi.GetLibroNumeroDaAbbreviazione(libro);
                                    if (numeroLibro == 0)
                                        throw new KeyNotFoundException();
                                    libroPrecedente = libro;
                                    numeroLibro2 = Funzioni.AggiungiZero(numeroLibro, 2);
                                }
                                listaRighe.Add(numeroLibro2 + Funzioni.AggiungiZero(riga.Substring(spazio + 1, punto1 - spazio - 1), 3) + Funzioni.AggiungiZero(riga.Substring(punto1 + 1, punto2 - punto1 - 1), 3) + riga.Substring(punto2 + 1));
                            }
                        }
                        listaRighe.Sort();
                        righe = listaRighe.ToArray();
                        libroPrecedente = nessunoTrovato;

                        foreach (string riga in righe)
                        {
                            libro = riga.Substring(0, 2);
                            if (libro != libroPrecedente)
                            {
                                numeroLibro = Convert.ToInt32(libro, CultureInfo.InvariantCulture);
                                if (libroPrecedente != nessunoTrovato)
                                    capitoliInLibri.Add(capitolo); // il numero di capitoli nel libro precedente
                                for (int i = 0; i < numeroLibro - numeroLibroPrecedente - 1; i++)
                                    capitoliInLibri.Add(0);
                                if (inThread)
                                    worker.ReportProgress(numeroLibro + 1, argomenti.barra);
                                libroPrecedente = libro;
                                numeroLibroPrecedente = numeroLibro;
                                if (capitoloPrecedente == 1)
                                {
                                    // per i libri con uno solo capitolo - quando si va al libro successivo non si cambia il numero del capitolo
                                    versettiInCapitoli.Add(versetto);
                                    versettoPrecedente = 0;
                                }
                            }
                            capitolo = Convert.ToByte(riga.Substring(2, 3), CultureInfo.InvariantCulture);
                            if (capitolo != capitoloPrecedente)
                            {
                                if (capitoloPrecedente != 0)
                                    versettiInCapitoli.Add(versetto);
                                capitoloPrecedente = capitolo;
                                versettoPrecedente = 0;
                            }
                            versetto = Convert.ToByte(riga.Substring(5, 3), CultureInfo.InvariantCulture);
                            for (int i = versettoPrecedente + 1; i < versetto; ++i)
                            { // versetti mancanti
                                ++numeroVersetto;
                                indice.Add((int)(bw.Seek(0, SeekOrigin.Current) - inizioTesto)); // -8 perché inizioBibbiaTesto è 8 dopo inizioBibbia
                                bw.Write("");
                            }
                            versettoPrecedente = versetto;
                            testoBibleworks = riga.Substring(8).Trim().Replace(@"\", @"\\").Replace("{", @"\{").Replace("}", @"\}");
                            if (testoBibleworks == ".") // versetto mancante
                                testoBibleworks = "";
                            indice.Add((int)(bw.Seek(0, SeekOrigin.Current) - inizioTesto)); // -8 perché inizioBibbiaTesto è 8 dopo inizioBibbia
                            bw.Write(testoBibleworks);
                            ++numeroVersetto;

                            chiave = Texts.TrovaParoleInVoce(testoBibleworks, numeroVersetto, chiave, argomenti.lingua);

                        }

                        versettiInCapitoli.Add(versetto);
                        capitoliInLibri.Add(capitolo);
                        for (int i = 0; i < 73 - numeroLibro; i++)
                            capitoliInLibri.Add(0);
                        inizioTestoIndiceLC = (UInt32)(bw.Seek(0, SeekOrigin.Current)) - pInizioDati;
                        bw.Write(capitoliInLibri.ToArray());
                        bw.Write(versettiInCapitoli.ToArray());
                        inizioTestoIndice = (UInt32)(bw.Seek(0, SeekOrigin.Current)) - pInizioDati;
                        foreach (int i in indice)
                            bw.Write(i);
                        break;
                    #endregion
                    #region Importa note
                    case TipoImportazione.ImportaNote:
                        fileNote = Directory.GetFiles(argomenti.fileDaAnalizzare);
                        int numeroNote = fileNote.Length;
                        noteTesto = new string[numeroNote];
                        string estensione;
                        for (int i = numeroNote - 1; i >= 0; --i)
                        {
                            estensione = Path.GetExtension(fileNote[i]);
                            if (estensione == ".parole_radici" || estensione == ".radici_diverse" || estensione == ".riferimenti" || estensione == ".ordine" || estensione == ".laparolainfo")
                            {
                                for (int j = i + 1; j < numeroNote; ++j)
                                    fileNote[j - 1] = fileNote[j];
                                --numeroNote;
                            }
                        }
                        Array.Resize(ref fileNote, numeroNote);
                        Array.Resize(ref noteTesto, numeroNote);
                        string[] noteTitoli = new string[numeroNote];
                        for (UInt32 i = 0; i < numeroNote; ++i)
                        {
                            noteTitoli[i] = Path.GetFileNameWithoutExtension(fileNote[i]);
                            if (noteTitoli[i].StartsWith("#", StringComparison.Ordinal))
                            {
                                if (noteTitoli[i].Length == 1) noteTitoli[i] += "0"; // non è un formato riconosciuto
                                if (noteTitoli[i].Length == 2) noteTitoli[i] += "0"; // non è un formato riconosciuto
                                if (noteTitoli[i].Length <= 3) noteTitoli[i] += "000"; // nessun capitolo
                                if (noteTitoli[i].Length <= 6) noteTitoli[i] += "000"; // nessun versetto
                                if (noteTitoli[i].Length <= 9) noteTitoli[i] += "0000"; // nessuna parola
                                if (noteTitoli[i].Length <= 13) noteTitoli[i] += "-" + noteTitoli[i].Remove(0, 1); // singolo versetto invece di brano
                                if (noteTitoli[i].Length == 18 && noteTitoli[i][9] == '-') noteTitoli[i] = noteTitoli[i].Insert(9, "0000") + "0000"; // formato #01001001-01001002 cioè senza il numero della parola
                            }
                            // eventuali segni di più alla fine di una nota sono tolti
                            // il segno è usato per distinguere due note che sono diverse, ma i titoli differiscono solo nelle lettere minuscole/maiuscole
                            // necessario perché Windows non può distinguere due file che hanno nomi che differiscono solo così (è case insensitive)
                            else
                            {
                                while (noteTitoli[i].EndsWith("+", StringComparison.Ordinal))
                                    noteTitoli[i] = noteTitoli[i].Remove(noteTitoli[i].Length - 1);
                            }
                        }
                        Array.Sort(noteTitoli, fileNote, new ConfrontoCI());

                        for (UInt32 i = 0; i < numeroNote; ++i)
                        {
                            try
                            {
                                noteTesto[i] = File.ReadAllText(fileNote[i], Encoding.GetEncoding(1252));
                            }
                            catch (ArgumentException) // se code page europeo occidentale non è installato, proviamo quello predefinito
                            {
                                noteTesto[i] = File.ReadAllText(fileNote[i], Encoding.GetEncoding(0));
                            }
                            catch (NotSupportedException) // se code page europeo occidentale non è installato, proviamo quello predefinito
                            {
                                noteTesto[i] = File.ReadAllText(fileNote[i], Encoding.GetEncoding(0));
                            }
                            noteTesto[i] = ConvertiLink(ConvertiApostrofeTrattino(noteTesto[i]));
                            try
                            {
                                rtb.Rtf = noteTesto[i];
                                chiave = Texts.TrovaParoleInVoce(rtb.Text, i, chiave, argomenti.lingua);
                            }
                            catch
                            { // il file non è RTF, lo consideriamo testo normale
                                chiave = Texts.TrovaParoleInVoce(noteTesto[i], i, chiave, argomenti.lingua);
                            }
                            if (inThread)
                                worker.ReportProgress((int)(i * 73 / numeroNote) + 1, argomenti.barra);
                        }
                        indici = Texts.ScriviNote(bw, pInizioDati, noteTitoli, noteTesto);
                        inizioTestoIndiceLC = indici[0];
                        inizioTestoIndice = indici[1];
                        break;
                    #endregion
                    #region Nuove note
                    case TipoImportazione.NuovaNote:
                        string[] titoliNuoveNote = { };
                        noteTesto = new string[] { };
                        for (UInt32 i = 0; i < noteTesto.Length; ++i)
                            chiave = Texts.TrovaParoleInVoce(noteTesto[i], i, chiave, argomenti.lingua);
                        indici = Texts.ScriviNote(bw, pInizioDati, titoliNuoveNote, noteTesto);
                        inizioTestoIndiceLC = indici[0];
                        inizioTestoIndice = indici[1];
                        break;
                    #endregion
                    #region ThML
                    case TipoImportazione.ImportaThml:
                        XmlDocument xmlDocumentoThML = new XmlDocument
                        {
                            PreserveWhitespace = true
                        };
                        xmlDocumentoThML.Load(argomenti.fileDaAnalizzare);
                        XmlNodeList nlThML = null;
                        #region ThML Bibbia
                        if (thmlTipo == TipoThML.Bibbia)
                        {
                            nlThML = xmlDocumentoThML.DocumentElement.SelectNodes("ThML.body/div1/div2");
                            int numeroNodiDiLibri = nlThML.Count;
                            int[] numeroLibri = new int[numeroNodiDiLibri];
                            string id = "";
                            for (int i = 0; i < numeroNodiDiLibri; ++i)
                            {
                                id = (nlThML[i].Attributes["id"] != null ? nlThML[i].Attributes["id"].Value : "");
                                if (id == "PrAzar")
                                    numeroLibri[i] = -34;
                                else if (id == "AddEsth")
                                    numeroLibri[i] = -19;
                                else
                                    numeroLibri[i] = Principale.testi.GetLibroNumeroDaAbbreviazione(id);
                            }
                            int libroMinimo;
                            bool nodoSecondarioTrovato;
                            do
                            {
                                int numeroLibroMinimo = 999;
                                libroMinimo = 999;
                                for (int i = 0; i < numeroNodiDiLibri; ++i)
                                {
                                    if (numeroLibri[i] > 0 && numeroLibri[i] < numeroLibroMinimo)
                                    {
                                        libroMinimo = i;
                                        numeroLibroMinimo = numeroLibri[i];
                                    }
                                }
                                if (libroMinimo < 999)
                                {
                                    nodoSecondarioTrovato = false;
                                    for (int i = 0; i < numeroNodiDiLibri; ++i)
                                    {
                                        if (numeroLibri[i] == -numeroLibroMinimo)
                                        { // quando ci sono due nodi per lo stesso libro, per esempio PrAzar o AddEst
                                            libroPrecedente = ImportaLibroThML(worker, argomenti, bw, ref chiave, inizioTesto, ref numeroVersetto, libroPrecedente, ref numeroLibroPrecedente, ref capitoliInLibri, ref versettiInCapitoli, ref indice, nlThML[libroMinimo], nlThML[i]);
                                            nodoSecondarioTrovato = true;
                                            break;
                                        }
                                    }
                                    if (!nodoSecondarioTrovato)
                                        libroPrecedente = ImportaLibroThML(worker, argomenti, bw, ref chiave, inizioTesto, ref numeroVersetto, libroPrecedente, ref numeroLibroPrecedente, ref capitoliInLibri, ref versettiInCapitoli, ref indice, nlThML[libroMinimo]);
                                    numeroLibri[libroMinimo] = 0;
                                }
                            } while (libroMinimo < 999);

                            int numeroUltimoLibroFatto = Principale.testi.GetLibroNumeroDaAbbreviazione(libroPrecedente);
                            for (int i = 0; i < 73 - numeroUltimoLibroFatto; i++)
                                capitoliInLibri.Add(0);

                            inizioTestoIndiceLC = (UInt32)(bw.Seek(0, SeekOrigin.Current)) - pInizioDati;
                            bw.Write(capitoliInLibri.ToArray());
                            bw.Write(versettiInCapitoli.ToArray());
                            inizioTestoIndice = (UInt32)(bw.Seek(0, SeekOrigin.Current)) - pInizioDati;
                            foreach (int i in indice)
                                bw.Write(i);
                        }
                        #endregion
                        else
                        #region ThML altro
                        {
                            nlThML = xmlDocumentoThML.DocumentElement.SelectNodes("ThML.body/div1");

                            List<string> noteTitoliThML = new List<string>();
                            List<string> noteTestoThML = new List<string>();

                            foreach (XmlNode nodo1 in nlThML)
                                AggiungiNoteDaThMLDiv(nodo1, 1, noteTitoliThML, noteTestoThML, worker, argomenti.barra);

                            for (int i = noteTitoliThML.Count - 1; i >= 0; --i)
                            {
                                if (string.IsNullOrEmpty(noteTestoThML[i]))
                                {
                                    noteTestoThML.RemoveAt(i);
                                    noteTitoliThML.RemoveAt(i);
                                }
                            }

                            fileNote = noteTitoliThML.ToArray();
                            noteTesto = noteTestoThML.ToArray();
                            for (int i = 0; i < fileNote.Length; ++i)
                                while (fileNote[i].StartsWith("\t", StringComparison.Ordinal))
                                    fileNote[i] = fileNote[i].Remove(0, 1);
                            Array.Sort(fileNote, noteTesto, new ConfrontoCI());
                            for (int i = noteTitoliThML.Count - 1; i >= 0; --i)
                                if (noteTitoliThML[i].StartsWith("#", StringComparison.Ordinal))
                                    noteTitoliThML.RemoveAt(i);
                            noteTitoliThML.Insert(0, ""); // l'indice è vuoto
                            noteInOrdine = noteTitoliThML.ToArray();

                            for (UInt32 i = 0; i < fileNote.Length; ++i)
                            {
                                rtb.Rtf = noteTesto[i];
                                chiave = Texts.TrovaParoleInVoce(rtb.Text, i, chiave, argomenti.lingua);
                            }

                            indici = Texts.ScriviNote(bw, pInizioDati, fileNote, noteTesto);
                            inizioTestoIndiceLC = indici[0];
                            inizioTestoIndice = indici[1];
                        }
                        break;
                        #endregion
                    #endregion
                    #region e-Sword
                    case TipoImportazione.ImportaEsword:
                        List<string> noteTitoliEsword = new List<string>(); // serve solo se non bibbia
                        List<string> noteTestoEsword = new List<string>(); // serve solo se non bibbia

                        int numeroRiga = 0;
                        int numeroRighe, righePerProgress;
                        switch (eswordTipo)
                        {
                            case TipoEsword.Bibbia:
                                DataRowCollection righeBibbia = dataSetEsword.Tables["Bible"].Rows;
                                numeroRighe = righeBibbia.Count;
                                righePerProgress = (numeroRighe < 73 ? 1 : numeroRighe / 73);
                                int numeroLibroESword = 0;
                                byte numeroCapitoloESword = 0, numeroVersettoESword = 0;
                                string testoVersetto;
                                RichTextBoxEx rtbESword = new RichTextBoxEx();
                                int[] arrayRiferimenti = new int[numeroRighe];
                                string[] arrayTesto = new string[numeroRighe];
                                int iRiga = 0;
                                // è necessario prima ordinare i versetti, perché quando c'è l'apocrifa e-sword mette i libri addizionali dopo Apocalisse
                                foreach (DataRow riga in righeBibbia)
                                {
                                    arrayRiferimenti[iRiga] = 1000000 * ConvertiLibro66A73ESword(riga["Book ID"]) + 1000 * Convert.ToInt32(riga["Chapter"], CultureInfo.InvariantCulture) + Convert.ToInt32(riga["Verse"], CultureInfo.InvariantCulture);
                                    arrayTesto[iRiga] = riga["Scripture"].ToString().Trim();
                                    ++iRiga;
                                }
                                Array.Sort(arrayRiferimenti, arrayTesto);
                                //                                foreach (DataRow riga in righeBibbia)
                                for (int i = 0; i < numeroRighe; ++i)
                                {
                                    testoVersetto = arrayTesto[i];
                                    //                                    testoVersetto = riga["Scripture"].ToString().Trim();
                                    if (!string.IsNullOrEmpty(testoVersetto))
                                    {
                                        numeroLibroESword = arrayRiferimenti[i] / 1000000;
                                        //                                        numeroLibroESword = ConvertiLibro66A73(riga["Book ID"]);
                                        if (numeroLibroESword != numeroLibroPrecedente)
                                        {
                                            if (numeroLibroPrecedente >= 1)
                                                capitoliInLibri.Add(numeroCapitoloESword); // il numero di capitoli nel libro precedente
                                            for (int j = 0; j < numeroLibroESword - numeroLibroPrecedente - 1; j++)
                                                capitoliInLibri.Add(0);
                                            numeroLibroPrecedente = numeroLibroESword;
                                            if (capitoloPrecedente == 1)
                                            {
                                                // per i libri con uno solo capitolo - quando si va al libro successivo non si cambia il numero del capitolo
                                                versettiInCapitoli.Add(numeroVersettoESword);
                                                versettoPrecedente = 0;
                                            }
                                        }
                                        numeroCapitoloESword = (byte)((arrayRiferimenti[i] / 1000) % 1000);
                                        //                                        numeroCapitoloESword = Convert.ToByte(riga["Chapter"]);
                                        if (numeroCapitoloESword != capitoloPrecedente)
                                        {
                                            if (capitoloPrecedente != 0)
                                                versettiInCapitoli.Add(numeroVersettoESword);
                                            capitoloPrecedente = numeroCapitoloESword;
                                            versettoPrecedente = 0;
                                        }
                                        numeroVersettoESword = (byte)(arrayRiferimenti[i] % 1000);
                                        //                                        numeroVersettoESword = Convert.ToByte(riga["Verse"]);
                                        for (int j = versettoPrecedente + 1; j < numeroVersettoESword; ++j)
                                        { // versetti mancanti
                                            ++numeroVersetto;
                                            indice.Add((int)(bw.Seek(0, SeekOrigin.Current) - inizioTesto)); // -8 perché inizioBibbiaTesto è 8 dopo inizioBibbia
                                            bw.Write("");
                                        }
                                        versettoPrecedente = numeroVersettoESword;

                                        indice.Add((int)(bw.Seek(0, SeekOrigin.Current) - inizioTesto)); // -8 perché inizioBibbiaTesto è 8 dopo inizioBibbia
                                        testoVersetto = testoVersetto.Replace(@"\cf6", @"\cf7"); // red letter Bibles: rosso è colore 6 in e-Sword ma colore 7 qui
                                        bw.Write(testoVersetto);

                                        ++numeroVersetto;
                                        if (testoVersetto.Contains(@"\"))
                                        {
                                            if (!testoVersetto.StartsWith(@"{\rtf", StringComparison.Ordinal))
                                                testoVersetto = @"{\rtf" + testoVersetto + "}";
                                            rtbESword.Rtf = testoVersetto;
                                            testoVersetto = rtbESword.Text;
                                        }
                                        chiave = Texts.TrovaParoleInVoce(testoVersetto, numeroVersetto, chiave, argomenti.lingua);
                                        ++numeroRiga;
                                    }
                                    if (inThread && (numeroRiga % righePerProgress == 0))
                                        worker.ReportProgress((int)(numeroRiga / righePerProgress) + 2, argomenti.barra);
                                }

                                versettiInCapitoli.Add(numeroVersettoESword);
                                capitoliInLibri.Add(numeroCapitoloESword);
                                for (int i = 0; i < 73 - numeroLibroESword; i++)
                                    capitoliInLibri.Add(0);
                                inizioTestoIndiceLC = (UInt32)(bw.Seek(0, SeekOrigin.Current)) - pInizioDati;
                                bw.Write(capitoliInLibri.ToArray());
                                bw.Write(versettiInCapitoli.ToArray());
                                inizioTestoIndice = (UInt32)(bw.Seek(0, SeekOrigin.Current)) - pInizioDati;
                                foreach (int i in indice)
                                    bw.Write(i);
                                break;
                            case TipoEsword.Commentario:
                                DataRowCollection righeCommentario = dataSetEsword.Tables["Commentary"].Rows;
                                DataRowCollection righeCapitoli = dataSetEsword.Tables["Chapter Notes"].Rows;
                                DataRowCollection righeLibri = dataSetEsword.Tables["Book Notes"].Rows;
                                numeroRighe = righeCommentario.Count + righeCapitoli.Count + righeLibri.Count;
                                righePerProgress = (numeroRighe < 73 ? 1 : numeroRighe / 73);
                                foreach (DataRow riga in righeCommentario)
                                {
                                    string libroEsword = Funzioni.AggiungiZero(ConvertiLibro66A73ESword(riga["Book ID"]).ToString(CultureInfo.InvariantCulture), 2);
                                    string capitoloEsword = Funzioni.AggiungiZero(riga["Chapter"].ToString(), 3);
                                    string commento = riga["Comments"].ToString();
                                    if (!string.IsNullOrEmpty(commento))
                                    {
                                        noteTitoliEsword.Add("#" + libroEsword + capitoloEsword + Funzioni.AggiungiZero(riga["Start Verse"].ToString(), 3) + "0000-" + libroEsword + capitoloEsword + Funzioni.AggiungiZero(riga["End Verse"].ToString(), 3) + "0000");
                                        noteTestoEsword.Add(ConvertiESwordARtf(commento));
                                    }
                                    ++numeroRiga;
                                    if (inThread && numeroRiga % righePerProgress == 0)
                                        worker.ReportProgress((int)(numeroRiga / righePerProgress) + 2, argomenti.barra);
                                }
                                foreach (DataRow riga in righeCapitoli)
                                {
                                    string commento = riga["Comments"].ToString();
                                    if (!string.IsNullOrEmpty(commento))
                                    {
                                        noteTitoliEsword.Add("#" + Funzioni.AggiungiZero(ConvertiLibro66A73ESword(riga["Book ID"]).ToString(CultureInfo.InvariantCulture), 2) + Funzioni.AggiungiZero(riga["Chapter"].ToString(), 3) + "0000000");
                                        noteTestoEsword.Add(ConvertiESwordARtf(commento));
                                    }
                                    ++numeroRiga;
                                    if (inThread && numeroRiga % righePerProgress == 0)
                                        worker.ReportProgress((int)(numeroRiga / righePerProgress) + 2, argomenti.barra);
                                }
                                foreach (DataRow riga in righeLibri)
                                {
                                    string commento = riga["Comments"].ToString();
                                    if (!string.IsNullOrEmpty(commento))
                                    {
                                        noteTitoliEsword.Add("#" + Funzioni.AggiungiZero(ConvertiLibro66A73ESword(riga["Book ID"]).ToString(CultureInfo.InvariantCulture), 2) + "0000000000");
                                        noteTestoEsword.Add(ConvertiESwordARtf(commento));
                                    }
                                    ++numeroRiga;
                                    if (inThread && numeroRiga % righePerProgress == 0)
                                        worker.ReportProgress((int)(numeroRiga / righePerProgress) + 2, argomenti.barra);
                                }
                                break;
                            case TipoEsword.Dizionario:
                                DataRowCollection righeVoce = dataSetEsword.Tables["Dictionary"].Rows;
                                numeroRighe = righeVoce.Count;
                                righePerProgress = (numeroRighe < 73 ? 1 : numeroRighe / 73);
                                foreach (DataRow riga in righeVoce)
                                {
                                    string definizione = riga["Definition"].ToString();
                                    if (!string.IsNullOrEmpty(definizione))
                                    {
                                        noteTitoliEsword.Add(riga["Topic"].ToString());
                                        noteTestoEsword.Add(ConvertiESwordARtf(definizione));
                                    }
                                    ++numeroRiga;
                                    if (inThread && numeroRiga % righePerProgress == 0)
                                        worker.ReportProgress((int)(numeroRiga / righePerProgress) + 2, argomenti.barra);
                                }
                                break;
                            case TipoEsword.Tema:
                                DataRowCollection righeTemi = dataSetEsword.Tables["Topic Notes"].Rows;
                                numeroRighe = righeTemi.Count;
                                righePerProgress = (numeroRighe < 73 ? 1 : numeroRighe / 73);
                                foreach (DataRow riga in righeTemi)
                                {
                                    string commento = riga["Comments"].ToString();
                                    if (!string.IsNullOrEmpty(commento))
                                    {
                                        noteTitoliEsword.Add(riga["Title"].ToString());
                                        noteTestoEsword.Add(ConvertiESwordARtf(commento));
                                    }
                                    ++numeroRiga;
                                    if (inThread && numeroRiga % righePerProgress == 0)
                                        worker.ReportProgress((int)(numeroRiga / righePerProgress) + 2, argomenti.barra);
                                }
                                break;
                        }

                        if (eswordTipo != TipoEsword.Bibbia)
                        {
                            fileNote = noteTitoliEsword.ToArray();
                            noteTesto = noteTestoEsword.ToArray();
                            Array.Sort(fileNote, noteTesto, new ConfrontoCI());

                            for (UInt32 i = 0; i < fileNote.Length; ++i)
                            {
                                try
                                {
                                    rtb.Rtf = noteTesto[i];
                                    chiave = Texts.TrovaParoleInVoce(rtb.Text, i, chiave, argomenti.lingua);
                                }
                                catch // il testo della nota non è RTF; lo consideriamo testo normale
                                {
                                    chiave = Texts.TrovaParoleInVoce(noteTesto[i], i, chiave, argomenti.lingua);
                                }
                            }

                            indici = Texts.ScriviNote(bw, pInizioDati, fileNote, noteTesto);
                            inizioTestoIndiceLC = indici[0];
                            inizioTestoIndice = indici[1];
                        }
                        if (eswordTipo == TipoEsword.Dizionario || eswordTipo == TipoEsword.Tema)
                        {
                            noteTitoliEsword.Insert(0, ""); // l'indice è vuoto
                            noteInOrdine = noteTitoliEsword.ToArray();
                        }
                        break;
                    #endregion
                }

                if (inThread)
                    worker.ReportProgress(75, argomenti.barra);
                UInt32 inizioParole = (UInt32)(bw.Seek(0, SeekOrigin.Current)) - pInizioDati;
                StringBuilder parole = new StringBuilder("");
                foreach (string s in chiave.Keys)
                    parole.Append(s).Append("|");
                bw.Write(parole.ToString());

                UInt32 inizioParoleIndiceIndice = (UInt32)(bw.Seek(0, SeekOrigin.Current)) - pInizioDati;
                ScriviNumeroApparenzeParole(bw, chiave);

                UInt32 inizioParoleIndice = (UInt32)(bw.Seek(0, SeekOrigin.Current)) - pInizioDati;
                foreach (List<OccorrenzaParola> lista in chiave.Values)
                    ScriviChiaveAFile(bw, lista);
                if (inThread)
                    worker.ReportProgress(83, argomenti.barra);

                UInt32 inizioRadici = (UInt32)(bw.Seek(0, SeekOrigin.Current)) - pInizioDati;
                List<string> listaRadici = new List<string>(8192);
                string nomeFileBase = Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(argomenti.fileDaAnalizzare);
                if (argomenti.fileDaAnalizzare.IndexOf(".", StringComparison.Ordinal) > -1)
                    nomeFileBase = Path.GetDirectoryName(argomenti.fileDaAnalizzare) + nomeFileBase;
                else // infoVersione[0] è il nome di una directory cioè è "importa note"
                    nomeFileBase = argomenti.fileDaAnalizzare + nomeFileBase;
                try
                {
                    string[] radiceDiParola = Funzioni.AggiungiRadiciDaFile(Path.GetDirectoryName(nomeFileBase), argomenti.lingua, parole.ToString().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries), listaRadici);

                    if (listaRadici.Count > 1)
                    {
                        // scrivere l'elenco delle radici
                        StringBuilder radici = new StringBuilder("");
                        foreach (string s in listaRadici)
                            radici.Append(s).Append("|");
                        bw.Write(radici.ToString());

                        // scrivere il numero della radice di ogni parola
                        int numeroParole = radiceDiParola.Length;
                        for (int i = 0; i < numeroParole; ++i)
                            bw.Write((UInt32)(listaRadici.BinarySearch(radiceDiParola[i], confrontoParole)));
                    }
                    else
                    {
                        // sola una radice (probabilmente *), quindi come se non ci fossero
                        inizioRadici = 0;
                    }
                }
                catch // file non esiste, o qualche problema nella lettura
                {
                    inizioRadici = 0;
                }
                if (inThread)
                    worker.ReportProgress(93, argomenti.barra);

                UInt32 inizioRadiciDiverse = 0;
                if (inizioRadici > 0)
                {
                    inizioRadiciDiverse = (UInt32)(bw.Seek(0, SeekOrigin.Current)) - pInizioDati;
                    string[] riga = new string[5];
                    bool fileAperto = false;
                    try
                    {
                        string[] radiciDiverse = File.ReadAllLines(nomeFileBase + ".radici_diverse");
                        fileAperto = true;
                        bw.Write((UInt32)(radiciDiverse.Length));
                        if (argomenti.tipo == TipoImportazione.ImportaBibbia || argomenti.tipo == TipoImportazione.ImportaZefania || argomenti.tipo == TipoImportazione.ImportaBibleworks || (argomenti.tipo == TipoImportazione.ImportaThml && thmlTipo == TipoThML.Bibbia))
                        {
                            foreach (string radiceDiversa in radiciDiverse)
                            {
                                riga = radiceDiversa.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                                bw.Write(Convert.ToByte(riga[0], CultureInfo.InvariantCulture));
                                bw.Write(Convert.ToByte(riga[1], CultureInfo.InvariantCulture));
                                bw.Write(Convert.ToByte(riga[2], CultureInfo.InvariantCulture));
                                bw.Write(Convert.ToUInt16(riga[3], CultureInfo.InvariantCulture));
                                bw.Write(riga[4]);
                            }
                        }
                        else if (argomenti.tipo == TipoImportazione.ImportaNote || (argomenti.tipo == TipoImportazione.ImportaThml && thmlTipo != TipoThML.Bibbia))
                        { // TipoImportazione.NuovaNota non può avere radiciDiverse (perché non ci sono ancora parole)
                            foreach (string radiceDiversa in radiciDiverse)
                            {
                                riga = radiceDiversa.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                                int numeroNota = Array.BinarySearch(fileNote, riga[0]);
                                if (numeroNota >= 0)
                                {
                                    bw.Write(Convert.ToUInt32(numeroNota, CultureInfo.InvariantCulture));
                                    bw.Write(Convert.ToUInt16(riga[1], CultureInfo.InvariantCulture));
                                    bw.Write(riga[2]);
                                }
                                else
                                {
                                    bw.Write(UInt32.MaxValue);
                                    bw.Write((UInt16)(1));
                                    bw.Write("*");
                                }
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        inizioRadiciDiverse = 0;
                        if (fileAperto) // altrimenti il file non esiste (o impossibile aprirlo), e non mostriamo un messaggio di errore
                            MessageBox.Show(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("ImportErrorDifferentRoots"), exc.Message, riga[0] + "|" + riga[1] + "|" + riga[2] + "|" + riga[3] + "|" + riga[4] + "|"), Principale.LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions);
                    }
                }
                if (inThread)
                    worker.ReportProgress(98, argomenti.barra);

                UInt32 inizioRiferimentiDiversi = (UInt32)(bw.Seek(0, SeekOrigin.Current)) - pInizioDati;
                try
                {
                    ScriviRiferimentiDiversi(bw, nomeFileBase);
                }
                catch // file non esiste, o qualche problema nella lettura
                {
                    inizioRiferimentiDiversi = 0;
                }

                if (inThread)
                    worker.ReportProgress(112, argomenti.barra);

                UInt32 inizioRiferimentiCitati = (UInt32)(bw.Seek(0, SeekOrigin.Current)) - pInizioDati;
                {
                    if (noteTesto != null)
                    {
                        if (!Principale.testi.ScriviRiferimentiCitati(bw, noteTesto))
                            inizioRiferimentiCitati = 0;
                    }
                    else
                        inizioRiferimentiCitati = 0;
                }

                if (inThread)
                    worker.ReportProgress(122, argomenti.barra);

                UInt32 inizioOrdine = (UInt32)(bw.Seek(0, SeekOrigin.Current)) - pInizioDati;
                try
                {
                    ScriviOrdine(bw, noteInOrdine, nomeFileBase);
                }
                catch // file non esiste, o qualche problema nella lettura
                {
                    inizioOrdine = 0;
                }

                if (inThread)
                    worker.ReportProgress(127, argomenti.barra);

                bw.Seek((int)pInizioDati + 4, SeekOrigin.Begin);
                bw.Write(inizioTestoIndiceLC);
                bw.Write(inizioTestoIndice);
                bw.Write(inizioParole);
                bw.Write(inizioParoleIndiceIndice);
                bw.Write(inizioParoleIndice);
                bw.Write(inizioRadici);
                bw.Write(inizioRadiciDiverse);
                bw.Write(inizioRiferimentiDiversi);
                bw.Write(inizioRiferimentiCitati);
                bw.Write(inizioOrdine);
                bw.Seek(0, SeekOrigin.End);
            }
            catch (Exception exc)
            {
                MessageBox.Show(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("ImportError"), exc.Message), Principale.LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions);
            }
            finally
            {
                try
                {
                    bw.Close();
                    fs.Close();
                }
                catch { }
            }

            e.Result = argomenti;

            if (inThread)
            {
                if (worker.CancellationPending)
                    e.Cancel = true;
            }
        }

        private static void ScriviChiaveAFile(BinaryWriter bw, List<OccorrenzaParola> lista)
        {
            byte[] datiDaScrivere = new byte[lista.Count * 6];
            MemoryStream ms = new MemoryStream(datiDaScrivere, true);
            BinaryWriter bwMemoria = new BinaryWriter(ms);
            foreach (OccorrenzaParola op in lista)
            {
                bwMemoria.Write(op.Voce);
                bwMemoria.Write(op.Parola);
            }
            bwMemoria.Seek(0, SeekOrigin.Begin);
            bw.Write(datiDaScrivere);
        }

        private static int ConvertiLibro66A73ESword(object libro)
        {
            // 67 Tobia -> 17
            // 68 Giuditta -> 18
            // 69 Sapienza -> 27
            // 70 Sirach -> 28
            // 71 Baruc -> 32
            // 72 1M -> 20
            // 73 2M -> 21
            int n = ConvertiLibro66A73(Convert.ToInt32(libro, CultureInfo.InvariantCulture));
            switch (n)
            {
                case 20:
                case 21:
                    return n + 7;
                case 27:
                    return 32;
                case 28:
                    return 20;
                case 32:
                    return 21;
                default:
                    return n;
            }
        }

        private static int ConvertiLibro66A73Zefania(string libro)
        {
            // 67 Giuditta -> 18
            // 68 Sapienza -> 27
            // 69 Tobia -> 17
            // 70 Sirach -> 28
            // 71 Baruc -> 32
            // 72 1M -> 20
            // 73 2M -> 21
            // 74 Song 3 Children
            // 75 Prayer Manasses
            // 77 3M
            // 78 4M
            // 80 1Esdras
            // 87 Susanna
            // 89 Psalm 151
            // 91 Bel and the Dragon
            int n = ConvertiLibro66A73(Convert.ToInt32(libro, CultureInfo.InvariantCulture));
            switch (n)
            {
                case 17:
                    return 18;
                case 18:
                    return 27;
                case 20:
                    return 17;
                case 21:
                    return 28;
                case 27:
                    return 32;
                case 28:
                    return 20;
                case 32:
                    return 21;
                default:
                    return n;
            }
        }

        private static int ConvertiLibro66A73(int libro)
        {
            // 67 Tobia -> 17
            // 68 Giuditta -> 18
            // 69 1M -> 20
            // 70 2M -> 21
            // 71 Sapienza -> 27
            // 72 Sirach -> 28
            // 73 Baruc -> 32
            if (libro <= 16)
                return libro;
            if (libro == 17)
                return 19;
            if (libro <= 22)
                return libro + 4;
            if (libro <= 25)
                return libro + 6;
            if (libro <= 66)
                return libro + 7;
            switch (libro)
            {
                case 67: // Tobia
                case 68: // Giuditta
                    return libro - 50;
                case 69: // 1Macc
                case 70: // 2Macc
                    return libro - 49;
                case 71: // Sapienza
                case 72: // Sirach
                    return libro - 44;
                case 73: // Baruc
                    return 32;
            }
            return libro; // non dovrebbe succedere mai, ma è necessario per non dare un errore nella compilazione del programma
        }

        private static string ConvertiESwordARtf(string commento)
        {
            if (!commento.Contains(@"\"))
                return commento;
            if (!commento.StartsWith(@"{\rtf", StringComparison.Ordinal))
            {
                while (commento.StartsWith(@"\par", StringComparison.Ordinal))
                    commento = commento.Substring(4);
                // commento = @"{\rtf1\ansi\ansicpg1252\deff0{\fonttbl{\f0 " + Principale.testi.Formato.FontNome + @";}{\f1 " + Principale.testi.Formato.FontGrecoNome + @";}{\f2 " + Principale.testi.Formato.FontEbraicoNome + @";}{\f3 " + Principale.testi.Formato.FontNome + @";}}{\colortbl ;\red0\green0\blue0;\red0\green0\blue255;\red0\green255\blue255;\red0\green255\blue0;\red255\green0\blue255;\red255\green0\blue0;\red255\green255\blue0;\red255\green255\blue255;\red0\green0\blue128;\red0\green128\blue128;\red0\green128\blue0;\red128\green0\blue128;\red128\green0\blue0;\red128\green128\blue0;\red128\green128\blue128;\red192\green192\blue192;}\viewkind4\uc1\pard\cf1\f0\fs" + Convert.ToInt32(Principale.testi.Formato.FontDimensione * 2).ToString(CultureInfo.InvariantCulture) + " " + commento.Trim() + "}";
                commento = @"{\rtf1\ansi\deff0{\fonttbl{\f0 " + Principale.testi.Formato.FontNome + @";}{\f1 " + Principale.testi.Formato.FontGrecoNome + @";}{\f2 " + Principale.testi.Formato.FontEbraicoNome + @";}{\f3 " + Principale.testi.Formato.FontNome + @";}}{\colortbl ;\red0\green0\blue0;\red0\green0\blue255;\red0\green255\blue255;\red0\green255\blue0;\red255\green0\blue255;\red255\green0\blue0;\red255\green255\blue0;\red255\green255\blue255;\red0\green0\blue128;\red0\green128\blue128;\red0\green128\blue0;\red128\green0\blue128;\red128\green0\blue0;\red128\green128\blue0;\red128\green128\blue128;\red192\green192\blue192;}\viewkind4\uc1\pard\cf1\f0\fs" + Convert.ToInt32(Principale.testi.Formato.FontDimensione * 2).ToString(CultureInfo.InvariantCulture) + " " + commento.Trim() + "}";
            }
            while (commento.IndexOf(@"{\cf11\ul ", StringComparison.Ordinal) >= 0)
                commento = CreareLinkESword(commento, commento.IndexOf(@"{\cf11\ul ", StringComparison.Ordinal));
            while (commento.IndexOf(@"{\ul\cf11 ", StringComparison.Ordinal) >= 0)
                commento = CreareLinkESword(commento, commento.IndexOf(@"{\ul\cf11 ", StringComparison.Ordinal));
            while (commento.IndexOf(@"{\field{\*\fldinst{HYPERLINK ", StringComparison.Ordinal) >= 0)
            {
                int p1 = commento.IndexOf(@"{\field{\*\fldinst{HYPERLINK ", StringComparison.Ordinal);
                int p2 = commento.IndexOf("}}", p1, StringComparison.Ordinal);
                // string riferimento = commento.Substring(p1 + 29, p2 - p1 - 29);
                commento = commento.Substring(0, p1 + 1) + commento.Substring(p2 + 2);
            }
            while (commento.IndexOf(@"{\fldrslt{\ul\cf2 ", StringComparison.Ordinal) >= 0)
            {
                int p1 = commento.IndexOf(@"{\fldrslt{\ul\cf2 ", StringComparison.Ordinal);
                int p2 = commento.IndexOf("}}}", p1, StringComparison.Ordinal);
                string riferimento = commento.Substring(p1 + 18, p2 - p1 - 18);
                commento = commento.Substring(0, p1) + @"\v " + RichTextBoxEx.InizioLink + @"\v0 " + riferimento + @"\v " + RichTextBoxEx.FineLink1 + RichTextBoxEx.FineLinkFile + riferimento + RichTextBoxEx.FineLink2 + @"\v0 " + commento.Substring(p2 + 3);
            }
            while (commento.IndexOf(@"<http://", StringComparison.Ordinal) >= 0)
            {
                int p1 = commento.IndexOf(@"<http://", StringComparison.Ordinal);
                if (p1 > 0 && commento[p1 - 1] == ' ')
                    --p1;
                int p2 = commento.IndexOf(">", p1, StringComparison.Ordinal);
                commento = commento.Substring(0, p1) + commento.Substring(p2 + 1);
            }
            return commento;
        }

        private static string CreareLinkESword(string commento, int p1)
        {
            int p2 = commento.IndexOf("}", p1, StringComparison.Ordinal);
            string riferimento = commento.Substring(p1 + 10, p2 - p1 - 10).Replace("_", " ");
            if (riferimento.Length >= 2 && (riferimento.StartsWith("H", StringComparison.Ordinal) || riferimento.StartsWith("G", StringComparison.Ordinal)) && char.IsDigit(riferimento[1]) && !riferimento.Contains(" "))
                // link ad un numero Strong
                commento = commento.Substring(0, p1) + @"\v " + RichTextBoxEx.InizioLink + @"\v0 " + riferimento + @"\v " + RichTextBoxEx.FineLink1 + RichTextBoxEx.FineLinkNota + riferimento + RichTextBoxEx.FineLink2 + @"\v0 " + commento.Substring(p2 + 1);
            else
                // link ad un riferimento
                commento = commento.Substring(0, p1) + @"\v " + RichTextBoxEx.InizioLink + @"\v0 " + riferimento + @"\v " + RichTextBoxEx.FineLink1 + RichTextBoxEx.FineLinkBrano + Principale.testi.ConvertiRiferimento(riferimento).ComeNotaTuttoRiferimento() + RichTextBoxEx.FineLink2 + @"\v0 " + commento.Substring(p2 + 1);
            return commento;
        }

        private static string ConvertiApostrofeTrattino(string stringa)
        {
            return stringa.Replace(@"\rquote ", @"'").Replace(@"’", @"'").Replace(@"\rquote\", @"'\").Replace("‘", @"'").Replace("&#8217;", @"'").Replace("–", "-"); // prima – è ASCII 150
        }

        private string ImportaLibroThML(BackgroundWorker worker, ThreadArgomenti argomenti, BinaryWriter bw, ref SortedDictionary<string, List<OccorrenzaParola>> chiave, uint inizioTesto, ref UInt32 numeroVersetto, string libroPrecedente, ref int numeroLibroPrecedente, ref List<byte> capitoliInLibri, ref List<byte> versettiInCapitoli, ref List<int> indice, XmlNode nodoPrincipale)
        {
            return ImportaLibroThML(worker, argomenti, bw, ref chiave, inizioTesto, ref numeroVersetto, libroPrecedente, ref numeroLibroPrecedente, ref capitoliInLibri, ref versettiInCapitoli, ref indice, nodoPrincipale, null);
        }

        private string ImportaLibroThML(BackgroundWorker worker, ThreadArgomenti argomenti, BinaryWriter bw, ref SortedDictionary<string, List<OccorrenzaParola>> chiave, uint inizioTesto, ref UInt32 numeroVersetto, string libroPrecedente, ref int numeroLibroPrecedente, ref List<byte> capitoliInLibri, ref List<byte> versettiInCapitoli, ref List<int> indice, XmlNode nodoPrincipale, XmlNode nodoSecondario)
        {
            string testoVersetto = nessunoTrovato, libro = "", nuovoTesto = "";
            int versettoPrecedente = 0, numeroLibro = 0;
            byte capitolo = 0, versetto = 0, capitoloPrecedente = 0;
            XmlNodeList nodiPrincipali = nodoPrincipale.SelectNodes("div3/p");
            XmlNodeList nodiSecondari = null;
            int numeroNodi = nodiPrincipali.Count;
            int numeroNodiPrincipali = numeroNodi;
            if (nodoSecondario != null)
            {
                nodiSecondari = nodoSecondario.SelectNodes("div3/p");
                numeroNodi += nodiSecondari.Count;
            }
            XmlNode nodo1 = null;
            for (int i = 0; i < numeroNodi; ++i)
            {
                nodo1 = (i < numeroNodiPrincipali ? nodiPrincipali[i] : nodiSecondari[i - numeroNodiPrincipali]);
                foreach (XmlNode nodo2 in nodo1.ChildNodes)
                {
                    if (nodo2.Name == "scripture")
                    {
                        if (!testoVersetto.Equals(nessunoTrovato) && capitolo != 0)
                        {
                            // funziona anche se "scripture" due volte di seguito, cioè un versetto mancante, quando testoVersetto=""
                            // con Sirach in alcune versioni, il prologo è il capitolo 0. Noi aggiungiamo tutto il testo al primo versetto del capitolo 1
                            indice.Add((int)(bw.Seek(0, SeekOrigin.Current) - inizioTesto)); // -8 perché inizioBibbiaTesto è 8 dopo inizioBibbia
                            testoVersetto = testoVersetto.Replace('\n', ' ').Trim();
                            while (testoVersetto.IndexOf("  ", StringComparison.Ordinal) >= 0)
                                testoVersetto = testoVersetto.Replace("  ", " ");
                            XmlNode sibling = nodo2.NextSibling;
                            while (sibling != null && sibling.Name != "#text")
                                sibling = sibling.NextSibling;
                            if (sibling != null && (sibling.InnerText.StartsWith("¶", StringComparison.Ordinal) || sibling.InnerText.StartsWith("Â ", StringComparison.Ordinal)) && !nodo2.OuterXml.Contains("|1|0|0\"")) // cioè non il primo versetto di un capitolo
                                testoVersetto += @"\par ";
                            bw.Write(testoVersetto);
                            ++numeroVersetto;
                            chiave = Texts.TrovaParoleInVoce(testoVersetto, numeroVersetto, chiave, argomenti.lingua);
                        }
                        if (!testoVersetto.Equals(nessunoTrovato) || capitolo != 0) // vedi commento qui sopra su Sirach
                            testoVersetto = "";

                        string brano = (nodo2.Attributes["parsed"] != null ? nodo2.Attributes["parsed"].Value : "");
                        string[] branoParti = brano.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        int lunghezza = branoParti.Length;
                        if (lunghezza >= 5)
                        {
                            libro = branoParti[lunghezza - 5];
                            if (libro != libroPrecedente)
                            {
                                if (libro == "PrAzar")
                                    numeroLibro = 34;
                                else if (libro == "AddEsth")
                                    numeroLibro = 19;
                                else
                                    numeroLibro = Principale.testi.GetLibroNumeroDaAbbreviazione(libro);
                                //                                if (!String.IsNullOrEmpty(libroPrecedente))
                                //                                    capitoliInLibri.Add(capitolo); // il numero di capitoli nel libro precedente
                                for (int j = 0; j < numeroLibro - numeroLibroPrecedente - 1; j++)
                                    capitoliInLibri.Add(0);
                                if (inThread)
                                    worker.ReportProgress(numeroLibro + 1, argomenti.barra);
                                libroPrecedente = libro;
                                numeroLibroPrecedente = numeroLibro;
                                if (capitoloPrecedente == 1)
                                {
                                    // per i libri con uno solo capitolo - quando si va al libro successivo non si cambia il numero del capitolo
                                    versettiInCapitoli.Add(versetto);
                                    versettoPrecedente = 0;
                                }
                            }
                            capitolo = Convert.ToByte(branoParti[lunghezza - 4], CultureInfo.InvariantCulture);
                            if (libro == "PrAzar")
                                capitolo = 15; // aggiungiamo PrAzar alla fine del libro di Daniele come il capitolo 15
                            if (capitolo != capitoloPrecedente)
                            {
                                if (capitoloPrecedente != 0)
                                    versettiInCapitoli.Add(versetto);
                                capitoloPrecedente = capitolo;
                                versettoPrecedente = 0;
                            }
                            versetto = Convert.ToByte(branoParti[lunghezza - 3], CultureInfo.InvariantCulture);
                            for (int j = versettoPrecedente + 1; j < versetto; ++j)
                            { // versetti mancanti
                                ++numeroVersetto;
                                indice.Add((int)(bw.Seek(0, SeekOrigin.Current) - inizioTesto)); // -8 perché inizioBibbiaTesto è 8 dopo inizioBibbia
                                bw.Write("");
                            }
                            versettoPrecedente = versetto;
                        }
                    }
                    else if (nodo2.Name == "#text" || nodo2.Name == "span")
                    {
                        nuovoTesto = nodo2.InnerText.Replace("`", "'");
                        while (nuovoTesto.Contains("¶")) // nuovo paragrafo
                        {
                            if (!nuovoTesto.StartsWith("¶", StringComparison.Ordinal)) // quando è all'inizio del versetto, è già stato aggiunto alla fine del versetto precedente
                                nuovoTesto = nuovoTesto.Insert(nuovoTesto.IndexOf("¶", StringComparison.Ordinal) + 1, @"\par "); // questa riga è da controllare ancora
                            nuovoTesto = nuovoTesto.Remove(nuovoTesto.IndexOf("¶", StringComparison.Ordinal), 1).TrimStart();
                        }
                        if (nuovoTesto.StartsWith("Â ", StringComparison.Ordinal)) // nuovo paragrafo
                        { // quando è all'inizio del versetto, è già stato aggiunto alla fine del versetto precedente
                            nuovoTesto = nuovoTesto.Remove(0, 2).TrimStart();
                        }
                        if (nuovoTesto.StartsWith(" ,", StringComparison.Ordinal))
                        {
                            nuovoTesto = nuovoTesto.Remove(0, 1);
                        }
                        if (nodo2.Name == "span" && nodo2.Attributes["class"] != null)
                        {
                            if (nodo2.Attributes["class"].Value == "smallcap")
                                nuovoTesto = nuovoTesto.ToUpperInvariant();
                        }
                        if (testoVersetto.EndsWith(".", StringComparison.Ordinal) && !nuovoTesto.StartsWith(" ", StringComparison.Ordinal))
                            testoVersetto += " ";
                        testoVersetto += nuovoTesto;
                    }
                    else if (nodo2.Name == "sup" || nodo2.Name == "#whitespace" || nodo2.Name == "note")
                    {
                    }
                    else if (nodo2.Name == "scripRef")
                    {
                        testoVersetto += nodo2.InnerText.Trim();
                    }
                    else if (nodo2.Name == "i")
                    {
                        testoVersetto += @"{\i1 " + nodo2.InnerText.Replace("`", "'") + @"}";
                    }
                    else
                        throw new FormatException(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("ImportUnknownCode"), nodo2.Name));
                }
            }
            // scrivere il testo rimasto non ancora scritto
            indice.Add((int)(bw.Seek(0, SeekOrigin.Current) - inizioTesto)); // -8 perché inizioBibbiaTesto è 8 dopo inizioBibbia
            testoVersetto = testoVersetto.Replace('\n', ' ').Trim();
            while (testoVersetto.IndexOf("  ", StringComparison.Ordinal) >= 0)
                testoVersetto = testoVersetto.Replace("  ", " ");
            bw.Write(testoVersetto);
            ++numeroVersetto;
            chiave = Texts.TrovaParoleInVoce(testoVersetto, numeroVersetto, chiave, argomenti.lingua);

            versettiInCapitoli.Add(versetto);
            capitoliInLibri.Add(capitolo);

            if (libro == "PrAzar")
                libro = "Dan";
            else if (libro == "AddEsth")
                libro = "Esth";

            return libro;
        }

        private static void ScriviNumeroApparenzeParole(BinaryWriter bw, SortedDictionary<string, List<OccorrenzaParola>> chiave)
        {
            UInt32 numeroApparenze = 0;
            byte[] datiDaScrivere = new byte[4 * chiave.Count + 4];
            MemoryStream ms = new MemoryStream(datiDaScrivere, true);
            BinaryWriter bwMemoria = new BinaryWriter(ms);
            bwMemoria.Write((UInt32)0);
            foreach (List<OccorrenzaParola> lista in chiave.Values)
            {
                numeroApparenze += (UInt32)(lista.Count);
                bwMemoria.Write(6 * numeroApparenze);
            }
            bwMemoria.Seek(0, SeekOrigin.Begin);
            bw.Write(datiDaScrivere);
        }

        private static void ScriviRiferimentiDiversi(BinaryWriter bw, string nomeFileBase)
        {
            string nomeFile = Path.GetFileName(nomeFileBase);
            if (nomeFile.Contains(@"("))
                nomeFile = nomeFile.Remove(nomeFile.IndexOf(@"(")).Trim();
            string[] fileRiferimenti = Directory.GetFiles(Path.GetDirectoryName(nomeFileBase), nomeFile + "*.riferimenti");
            if (fileRiferimenti.Length == 0)
                throw new Exception();
            string[] riferimentiDiversi = File.ReadAllLines(fileRiferimenti[0]);
            bw.Write(riferimentiDiversi.Length);
            string[] riferimento6Cifre = { "0", "0", "0", "0", "0", "0" };
            foreach (string riferimentoDiverso in riferimentiDiversi)
            {
                riferimento6Cifre = riferimentoDiverso.Split('|');
                for (int i = 0; i < 6; ++i)
                    bw.Write(Convert.ToInt16(riferimento6Cifre[i], CultureInfo.InvariantCulture));
            }
        }

        private static void ScriviOrdine(BinaryWriter bw, string[] noteInOrdine, string nomeFileBase)
        {
            if (noteInOrdine == null) // altrimenti l'ordine già preso dal file ThML
                noteInOrdine = File.ReadAllLines(nomeFileBase + ".ordine"); // deve essere UTF-8
            bw.Write(noteInOrdine.Length);
            foreach (string notaInOrdine in noteInOrdine)
                bw.Write(notaInOrdine);
        }

        private void AggiungiNoteDaThMLDiv(XmlNode nodo, int livello, List<string> noteTitoliThML, List<string> noteTestoThML, BackgroundWorker worker, BarraConEtichetta barra)
        {
            StringBuilder testoNota = new StringBuilder("");
            string titolo = (nodo.Attributes["title"] != null ? nodo.Attributes["title"].Value.Trim() : "");
            if (inThread)
            {
                string progresso = (nodo.Attributes["progress"] != null ? nodo.Attributes["progress"].Value : "");
                if (!string.IsNullOrEmpty(progresso))
                {
                    if (progresso.EndsWith("%", StringComparison.Ordinal))
                        progresso = progresso.Substring(0, progresso.Length - 1);
                    worker.ReportProgress(Convert.ToInt32(Convert.ToSingle(progresso, CultureInfo.InvariantCulture) * 0.73 + 2.0), barra);
                }
            }

            if (titolo != "Indexes" && titolo != "Indexes.")
            {
                bool trovataNota;
                int numeroConTitolo = 0;
                string titoloDaProvare = titolo;
                do
                {
                    trovataNota = false;
                    for (int i = 0; i < noteTitoliThML.Count; ++i)
                        if (noteTitoliThML[i].Trim() == titoloDaProvare)
                            trovataNota = true;
                    if (trovataNota)
                    {
                        ++numeroConTitolo;
                        titoloDaProvare = titolo + " (" + numeroConTitolo.ToString(CultureInfo.InvariantCulture) + ")";
                    }
                } while (trovataNota);
                titolo = titoloDaProvare;

                titolo = new string('\t', livello - 1) + titolo;
                string testoParagrafo = "";
                noteTitoliThML.Add(titolo);
                noteTestoThML.Add("");
                int posizioneNota = noteTestoThML.Count - 1;

                foreach (XmlNode sottoNodo in nodo.ChildNodes)
                {
                    if (!string.IsNullOrEmpty(sottoNodo.InnerXml) || sottoNodo.Name == "scripCom")
                    {
                        switch (sottoNodo.Name)
                        {
                            case "scripCom":
                                testoParagrafo = sottoNodo.OuterXml;
                                break;
                            case "h1":
                                testoParagrafo = @"\fs36\b " + sottoNodo.InnerXml + @"\plain\par ";
                                break;
                            case "h2":
                                testoParagrafo = @"\fs32\i " + sottoNodo.InnerXml + @"\plain\par ";
                                break;
                            case "h3":
                                testoParagrafo = @"\fs30\b " + sottoNodo.InnerXml + @"\plain\par ";
                                break;
                            case "h4":
                                testoParagrafo = @"\fs28\i " + sottoNodo.InnerXml + @"\plain\par ";
                                break;
                            case "h5":
                                testoParagrafo = @"\fs26\b " + sottoNodo.InnerXml + @"\plain\par ";
                                break;
                            case "h6":
                                testoParagrafo = @"\fs26\i " + sottoNodo.InnerXml + @"\plain\par ";
                                break;
                            case "p":
                                testoParagrafo = sottoNodo.InnerXml + @"\par ";
                                break;
                            case "attr":
                                testoParagrafo = "    " + sottoNodo.InnerXml + @"\par ";
                                break;
                            case "verse":
                                testoParagrafo = @"\par " + sottoNodo.InnerXml + @"\par ";
                                break;
                            case "div2":
                                AggiungiNoteDaThMLDiv(sottoNodo, 2, noteTitoliThML, noteTestoThML, worker, barra);
                                testoParagrafo = "";
                                break;
                            case "div3":
                                AggiungiNoteDaThMLDiv(sottoNodo, 3, noteTitoliThML, noteTestoThML, worker, barra);
                                testoParagrafo = "";
                                break;
                            case "div4":
                                AggiungiNoteDaThMLDiv(sottoNodo, 4, noteTitoliThML, noteTestoThML, worker, barra);
                                testoParagrafo = "";
                                break;
                            case "div":
                                testoParagrafo = sottoNodo.InnerXml.Trim();
                                if (testoParagrafo.StartsWith("<p ", StringComparison.Ordinal) && testoParagrafo.EndsWith("</p>", StringComparison.Ordinal))
                                    testoParagrafo = testoParagrafo.Remove(testoParagrafo.Length - 4, 4).Remove(0, testoParagrafo.IndexOf(">", StringComparison.Ordinal) + 1) + @"\par ";
                                if (testoParagrafo.StartsWith("<table ", StringComparison.Ordinal) && testoParagrafo.EndsWith("</table>", StringComparison.Ordinal))
                                    testoParagrafo = testoParagrafo.Remove(testoParagrafo.Length - 8, 8).Remove(0, testoParagrafo.IndexOf(">", StringComparison.Ordinal) + 1) + @"\par ";
                                break;
                            case "table":
                            case "blockquote":
                            case "argument":
                                testoParagrafo = sottoNodo.InnerXml + @"\par ";
                                break;
                            case "ul":
                            case "ol":
                                testoParagrafo = @"\par " + sottoNodo.InnerXml;
                                break;
                            case "pre":
                                testoParagrafo = sottoNodo.InnerXml.Replace("\n", @"\par");
                                break;
                            case "glossary":
                                AggiungiNoteDaThMLDiv(sottoNodo, livello + 1, noteTitoliThML, noteTestoThML, worker, barra);
                                testoParagrafo = "";
                                break;
                            case "term":
                                titolo = new string('\t', livello - 1) + sottoNodo.InnerXml;
                                posizioneNota = AggiungiNota(noteTitoliThML, noteTestoThML, ConvertiThMLARtf(testoParagrafo), titolo, posizioneNota);
                                testoNota.Remove(0, testoNota.Length);
                                testoParagrafo = "";
                                break;
                            case "def":
                                testoParagrafo = sottoNodo.InnerXml;
                                break;
                            default:
                                throw new FormatException(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("ImportUnknownCode"), sottoNodo.Name));
                        }

                        while (testoParagrafo.IndexOf("<scripCom", StringComparison.Ordinal) > -1 && thmlTipo == TipoThML.Commentario)
                        {
                            int inizioParsed = testoParagrafo.IndexOf("parsed=", testoParagrafo.IndexOf("<scripCom", StringComparison.Ordinal), StringComparison.Ordinal);
                            if (inizioParsed > -1)
                            {
                                // quello che è prima del <scripCom> appartiene alla nota precedente; più tardi il testo sarà cancellato da testoParagrafo
                                testoNota.Append(ConvertiThMLARtf(testoParagrafo.Substring(0, testoParagrafo.IndexOf("<scripCom", StringComparison.Ordinal))));

                                string riferimento = testoParagrafo.Substring(inizioParsed + 8, testoParagrafo.IndexOf("\"", inizioParsed + 8, StringComparison.Ordinal) - inizioParsed - 8);
                                string[] riferimentoBrani = riferimento.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                                Riferimento titoloRiferimento = new Riferimento();
                                for (int i = 0; i < riferimentoBrani.Length; ++i)
                                {
                                    string[] branoParti = riferimentoBrani[i].Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                                    int lunghezza = branoParti.Length;
                                    byte numeroLibro = Principale.testi.GetLibroNumeroDaAbbreviazione(branoParti[lunghezza - 5]);
                                    byte numeroCapitoloDa = Convert.ToByte(branoParti[lunghezza - 4], CultureInfo.InvariantCulture);
                                    byte numeroVersettoDa = Convert.ToByte(branoParti[lunghezza - 3], CultureInfo.InvariantCulture);
                                    byte numeroCapitoloA = Convert.ToByte(branoParti[lunghezza - 2], CultureInfo.InvariantCulture);
                                    byte numeroVersettoA = Convert.ToByte(branoParti[lunghezza - 1], CultureInfo.InvariantCulture);
                                    if (numeroCapitoloDa == 0)
                                    {
                                        numeroCapitoloDa = 1;
                                        numeroVersettoDa = 1;
                                        numeroCapitoloA = 255;
                                        numeroVersettoA = 255;
                                    }
                                    else if (numeroVersettoDa == 0)
                                    {
                                        numeroVersettoDa = 1;
                                        if (numeroCapitoloA == 0)
                                            numeroCapitoloA = numeroCapitoloDa;
                                        numeroVersettoA = 255;
                                    }
                                    else if (numeroCapitoloA == 0)
                                    {
                                        numeroCapitoloA = numeroCapitoloDa;
                                        numeroVersettoA = numeroVersettoDa;
                                    }
                                    else if (numeroVersettoA == 0)
                                        numeroVersettoA = 255;
                                    titoloRiferimento.AggiungiBrano(new byte[] { numeroLibro, numeroCapitoloDa, numeroVersettoDa, numeroLibro, numeroCapitoloA, numeroVersettoA });
                                }
                                titolo = titoloRiferimento.ComeNotaTuttoRiferimento();

                                if (!string.IsNullOrEmpty(titolo) && titolo != noteTitoliThML[posizioneNota])
                                { // altrimenti c'è una seconda nota sullo stesso versetto, e possiamo continuare
                                    posizioneNota = AggiungiNota(noteTitoliThML, noteTestoThML, testoNota.ToString(), titolo, posizioneNota);
                                    testoNota.Remove(0, testoNota.Length);
                                }
                            }
                            testoParagrafo = testoParagrafo.Remove(0, testoParagrafo.IndexOf("<scripCom", StringComparison.Ordinal));
                            testoParagrafo = testoParagrafo.Remove(0, testoParagrafo.IndexOf(">", StringComparison.Ordinal) + 1);
                        }
                        testoNota.Append(ConvertiThMLARtf(testoParagrafo));
                    }
                }

                noteTestoThML[posizioneNota] = (testoNota.Length == 0 ? "" : TestoNotaAggiustato(testoNota.ToString()));
            }
        }

        private static int AggiungiNota(List<string> noteTitoliThML, List<string> noteTestoThML, String testoNota, string titolo, int posizioneNota)
        {
            // concludere la nota attuale (non salvandola se non c'è testo) e cominciare una nota sul versetto
            if (testoNota.Length == 0)
            {
                noteTitoliThML.RemoveAt(posizioneNota);
                noteTestoThML.RemoveAt(posizioneNota);
            }
            else
            {
                noteTestoThML[posizioneNota] = TestoNotaAggiustato(testoNota);
            }
            noteTitoliThML.Add(titolo);
            noteTestoThML.Add("");
            return noteTestoThML.Count - 1;
        }

        private static string TestoNotaAggiustato(string testoNota)
        {
            // succede in un commentario, quando un paragrafo contiene solo il tag per indicare il versetto del commento
            while (testoNota.StartsWith(@"\par ", StringComparison.Ordinal))
                testoNota = testoNota.Remove(0, 5);

            return Principale.testi.RtfIntestazione() + ConvertiApostrofeTrattino(testoNota).Trim() + "}";
        }

        private static string ConvertiThMLARtf(string s)
        {
            if (string.IsNullOrEmpty(s))
                return "";

            s = s.Replace('\n', ' ').Replace('\t', ' ').Replace("’", "'");
            while (s.IndexOf("  ", StringComparison.Ordinal) > -1) // "Ordinal" altrimenti trova anche spazio+lettera greca, ma non viene rimosso e c'è un ciclo infinito
                s = s.Replace("  ", " ");
            s = CancellaHtmlTag(s, "table");
            s = CancellaHtmlTag(s, "tr");
            s = CancellaHtmlTag(s, "td");
            s = CancellaHtmlTag(s, "thead");
            s = CancellaHtmlTag(s, "th");
            s = CancellaHtmlTag(s, "tbody");
            s = CancellaHtmlTag(s, "colgroup");
            s = CancellaHtmlTag(s, "col");
            s = CancellaHtmlTag(s, "scripCom");
            while (s.IndexOf("<span class=\"MsoEndnoteReference\"", StringComparison.Ordinal) >= 0)
                s = s.Remove(s.IndexOf("<span class=\"MsoEndnoteReference\"", StringComparison.Ordinal), s.IndexOf("</span>", s.IndexOf("<span class=\"MsoEndnoteReference\"", StringComparison.Ordinal), StringComparison.Ordinal) - s.IndexOf("<span class=\"MsoEndnoteReference\"", StringComparison.Ordinal) + 7);
            s = CancellaHtmlTag(s, "date");
            s = CancellaHtmlTag(s, "index");
            s = CancellaHtmlTag(s, "ul");
            s = CancellaHtmlTag(s, "del");
            s = CancellaHtmlTag(s, "span");
            s = CancellaHtmlTag(s, "cite");
            s = CancellaHtmlTag(s, "blockquote");
            s = CancellaHtmlTag(s, "verse");
            s = CancellaHtmlTag(s, "name");
            s = CancellaHtmlTag(s, "div");
            s = CancellaHtmlTag(s, "unclear");
            s = CancellaHtmlTag(s, "img");
            s = s.Replace("</l>", @"\par "); // così righe del verso sono messe su una nuova riga, prima di cancellare </l> nella prossima riga
            s = CancellaHtmlTag(s, "l");
            if (s == @"<br /> \par ")
                s = @"\par ";
            s = s.Replace("<br />", @"\par ");
            while (s.IndexOf("<note", StringComparison.Ordinal) > -1)
            {
                int inizioTag = s.IndexOf("<note", StringComparison.Ordinal);
                int fineTag = s.IndexOf(">", inizioTag, StringComparison.Ordinal);
                if (s[fineTag - 1] == '/')
                {  // una nota vuota, che chiude se stessa <note... />
                    s = s.Substring(0, inizioTag) + s.Substring(fineTag + 1);
                }
                else
                {
                    int tagFine = s.IndexOf("</note>", fineTag, StringComparison.Ordinal);
                    //File.WriteAllText(@"c:\test.txt", s);
                    string nota = s.Substring(fineTag + 1, tagFine - fineTag - 1);
                    nota = CancellaHtmlTag(nota, "p").Trim();
                    s = s.Substring(0, inizioTag) + @"\{" + nota + @"\}" + s.Substring(tagFine + 7);
                }
            }

            s = SostituisciHtmlTag(s, "p", "", @"\par "); // deve essere dopo "note"
            s = s.Replace("&amp;", "&");
            while (s.IndexOf("  ", StringComparison.Ordinal) > -1) // "Ordinal" altrimenti trova anche spazio+lettera greca, ma non viene rimosso e c'è un ciclo infinito
                s = s.Replace("  ", " ");

            // le prossime 9 righe più sezione '<a>' devono essere dopo la rimovione di "  "
            s = s.Replace("<i>", @"{\i ").Replace("</i>", "}");
            s = SostituisciHtmlTag(s, "em", @"{\i ", "}");
            s = s.Replace("<b>", @"{\b ").Replace("</b>", "}");
            s = SostituisciHtmlTag(s, "sup", @"{\super ", "}");
            s = SostituisciHtmlTag(s, "sub", @"{\sub ", "}");
            s = SostituisciHtmlTag(s, "small", @"{\fs18 ", @"}");
            s = SostituisciHtmlTag(s, "strong", @"{\b ", @"}");
            s = SostituisciHtmlTag(s, "li", @"\u8226?\tab ", @"\par ");
            s = SostituisciHtmlTag(s, "h1", @"\fs36\b ", @"\plain\par ");
            s = SostituisciHtmlTag(s, "h2", @"\fs32\i ", @"\plain\par ");
            s = SostituisciHtmlTag(s, "h3", @"\fs30\b ", @"\plain\par ");
            s = SostituisciHtmlTag(s, "h4", @"\fs28\i ", @"\plain\par ");
            s = SostituisciHtmlTagChiusa(s, "hr", @"\par -----------\par ");
            s = SostituisciHtmlTagChiusa(s, "scripture", "");
            while (s.Contains("<a "))
            {
                int inizioA = s.IndexOf("<a ", StringComparison.Ordinal);
                // questo funziona con Naves'; bisogna controllare con altri testi con questa tag
                int p1 = s.IndexOf(">", inizioA, StringComparison.Ordinal);
                if (s[p1 - 1] == '/')
                    s = s.Substring(0, inizioA) + s.Substring(p1 + 1);
                else
                {
                    int p2 = s.IndexOf("</a>", p1, StringComparison.Ordinal);
                    if (s.Substring(inizioA, 7) == "<a href")
                    { // se c'è "href" nella tag, usiamo quello per il collegamento, per visualizziamo comunque quello che è visualizzato nel file XML
                        int p3 = s.IndexOf("\"", inizioA, StringComparison.Ordinal);
                        int p4 = s.IndexOf("\"", p3 + 1, StringComparison.Ordinal);
                        if (s.IndexOf("=", p3, StringComparison.Ordinal) > p3 && s.IndexOf("=", p3, StringComparison.Ordinal) < p4)
                            p3 = s.IndexOf("=", p3, StringComparison.Ordinal);
                        s = s.Substring(0, inizioA) + @"\v " + RichTextBoxEx.InizioLink + @"\v0 " + s.Substring(p1 + 1, p2 - p1 - 1) + @"\v " + RichTextBoxEx.FineLink1 + RichTextBoxEx.FineLinkNota + s.Substring(p3 + 1, p4 - p3 - 1) + RichTextBoxEx.FineLink2 + @"\v0 " + s.Substring(p2 + 4);
                    }
                    else
                    {
                        if (s.Substring(p1 + 1, 9) == "<scripRef")
                            s = s.Substring(0, inizioA) + s.Substring(p1 + 1, p2 - p1 - 1) + s.Substring(p2 + 4);
                        else
                            s = s.Substring(0, inizioA) + @"\v " + RichTextBoxEx.InizioLink + @"\v0 " + s.Substring(p1 + 1, p2 - p1 - 1) + @"\v " + RichTextBoxEx.FineLink1 + RichTextBoxEx.FineLinkNota + s.Substring(p1 + 1, p2 - p1 - 1) + RichTextBoxEx.FineLink2 + @"\v0 " + s.Substring(p2 + 4);
                    }
                }
            }
            //            s = CancellaHtmlTag(s, "a");

            int inizioRiferimento = s.IndexOf("<scripRef", StringComparison.Ordinal);
            while (inizioRiferimento > -1)
            {
                int inizioParsed = s.IndexOf("parsed=", inizioRiferimento, StringComparison.Ordinal);
                string libroStringa, riferimento, riferimentoStringa;
                int lunghezza, numeroCapitoloDa, numeroVersettoDa, numeroCapitoloA, numeroVersettoA, inizioTesto;
                if (inizioParsed > -1)
                {
                    riferimento = s.Substring(inizioParsed + 8, s.IndexOf("\"", inizioParsed + 8, StringComparison.Ordinal) - inizioParsed - 8);
                    string[] riferimentoBrani = riferimento.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    riferimentoStringa = "";
                    foreach (string brano in riferimentoBrani)
                    {
                        string[] branoParti = brano.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        lunghezza = branoParti.Length;
                        libroStringa = Funzioni.AggiungiZero(Principale.testi.GetLibroNumeroDaAbbreviazione(branoParti[lunghezza - 5]), 2);
                        numeroCapitoloDa = Convert.ToInt32(branoParti[lunghezza - 4], CultureInfo.InvariantCulture);
                        numeroVersettoDa = Convert.ToInt32(branoParti[lunghezza - 3], CultureInfo.InvariantCulture);
                        numeroCapitoloA = Convert.ToInt32(branoParti[lunghezza - 2], CultureInfo.InvariantCulture);
                        numeroVersettoA = Convert.ToInt32(branoParti[lunghezza - 1], CultureInfo.InvariantCulture);
                        if (numeroCapitoloDa == 0)
                            riferimentoStringa += "#" + libroStringa + "0000000000-" + libroStringa + "0000000000";
                        else if (numeroVersettoDa == 0)
                        {
                            if (numeroCapitoloA == 0)
                                numeroCapitoloA = numeroCapitoloDa; // per casi come 7|0|0|0; per altri casi come 7|0|8|0 funziona senza questo aggiustamento
                            riferimentoStringa += "#" + libroStringa + Funzioni.AggiungiZero(numeroCapitoloDa, 3) + "0000000-" + libroStringa + Funzioni.AggiungiZero(numeroCapitoloA, 3) + "0000000";
                        }
                        else if (numeroCapitoloA == 0)
                            riferimentoStringa += "#" + libroStringa + Funzioni.AggiungiZero(numeroCapitoloDa, 3) + Funzioni.AggiungiZero(numeroVersettoDa, 3) + "0000-" + libroStringa + Funzioni.AggiungiZero(numeroCapitoloDa, 3) + Funzioni.AggiungiZero(numeroVersettoDa, 3) + "0000";
                        else
                            riferimentoStringa += "#" + libroStringa + Funzioni.AggiungiZero(numeroCapitoloDa, 3) + Funzioni.AggiungiZero(numeroVersettoDa, 3) + "0000-" + libroStringa + Funzioni.AggiungiZero(numeroCapitoloA, 3) + Funzioni.AggiungiZero(numeroVersettoA, 3) + "0000";
                    }
                    inizioTesto = s.IndexOf(">", inizioRiferimento, StringComparison.Ordinal) + 1;
                    s = s.Substring(0, inizioRiferimento) + @"\v " + RichTextBoxEx.InizioLink + @"\v0 " + s.Substring(inizioTesto, s.IndexOf("<", inizioTesto, StringComparison.Ordinal) - inizioTesto) + @"\v " + RichTextBoxEx.FineLink1 + RichTextBoxEx.FineLinkBrano + riferimentoStringa + RichTextBoxEx.FineLink2 + @"\v0 " + s.Substring(s.IndexOf("</scripRef>", inizioRiferimento, StringComparison.Ordinal) + 11);
                }
                else
                {
                    inizioTesto = s.IndexOf(">", inizioRiferimento, StringComparison.Ordinal) + 1;
                    s = s.Substring(0, inizioRiferimento) + s.Substring(inizioTesto, s.IndexOf("<", inizioTesto, StringComparison.Ordinal) - inizioTesto) + s.Substring(s.IndexOf("</scripRef>", inizioRiferimento, StringComparison.Ordinal) + 11);
                }
                inizioRiferimento = s.IndexOf("<scripRef", StringComparison.Ordinal);
            }
            s = s.Replace("</scripRef>", ""); // necessario perché a volte ci sono due scripRef dello stesso versetto uno dentro l'altro, e in quel caso uno finale rimane

            if (s.IndexOf("<", StringComparison.Ordinal) > -1)
                throw new FormatException(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("ImportUnknownCode"), s));
            return s;
        }

        private static string SostituisciHtmlTagChiusa(string s, string htmlTag, string codiceRtf)
        {
            // sostituisce una tag come <hr .../> (in realtà anche <hr ... >) con codiceRtf; SostituisciHtmlTag elimina sempre una simile tag
            while (s.IndexOf("<" + htmlTag, StringComparison.Ordinal) > -1)
                s = s.Substring(0, s.IndexOf("<" + htmlTag, StringComparison.Ordinal)) + codiceRtf + s.Substring(s.IndexOf(">", s.IndexOf("<" + htmlTag, StringComparison.Ordinal), StringComparison.Ordinal) + 1);
            return s;
        }

        private static string SostituisciHtmlTag(string s, string htmlTag, string codiceRtfInizio, string codiceRtfFine)
        {
            while (s.IndexOf("<" + htmlTag, StringComparison.Ordinal) > -1)
            {
                int posizioneFine = s.IndexOf(">", s.IndexOf("<" + htmlTag, StringComparison.Ordinal), StringComparison.Ordinal);
                if (s[posizioneFine - 1] == '/') // tipo <sub ... />
                    s = s.Substring(0, s.IndexOf("<" + htmlTag, StringComparison.Ordinal)) + s.Substring(posizioneFine + 1);
                else
                    s = s.Substring(0, s.IndexOf("<" + htmlTag, StringComparison.Ordinal)) + codiceRtfInizio + s.Substring(posizioneFine + 1);
            }
            if (htmlTag.Contains(" "))
                htmlTag = htmlTag.Remove(htmlTag.IndexOf(" ", StringComparison.Ordinal));
            return s.Replace("</" + htmlTag + ">", codiceRtfFine);
        }

        private static string CancellaHtmlTag(string s, string htmlTag)
        {
            string htmlTagPiuSpazio = htmlTag + " ";
            while (s.IndexOf("<" + htmlTagPiuSpazio, StringComparison.Ordinal) > -1)
                s = s.Remove(s.IndexOf("<" + htmlTagPiuSpazio, StringComparison.Ordinal), s.IndexOf(">", s.IndexOf("<" + htmlTagPiuSpazio, StringComparison.Ordinal), StringComparison.Ordinal) - s.IndexOf("<" + htmlTagPiuSpazio, StringComparison.Ordinal) + 1);
            return s.Replace("</" + htmlTag + ">", "");
        }

        private void AnalizzaFileXmlODirectoryProgresso(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                ((BarraConEtichetta)(e.UserState)).Valore = e.ProgressPercentage;
            }
            catch (NullReferenceException) // succede quando programma è chiuso mentre thread in esecuzione
            {
                ((BackgroundWorker)sender).CancelAsync();
            }
        }

        private void AnalizzatoFileXmlODirectory(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                return;

            string nomeFileDaAggiungere = ((ThreadArgomenti)e.Result).nomeFileLP;
            try
            {
                NomeVersione = Principale.testi.AggiungiTesto(nomeFileDaAggiungere, 0);
                genitore.GeneraMenuConTesti();
            }
            catch (FileNonValidoException)
            {
                NomeVersione = "";
            }

            if (inThread)
            {
                (((ThreadArgomenti)e.Result).barra).MessaggioCompleto(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("ImportCompleted"), nomeFileDaAggiungere));
                (((ThreadArgomenti)e.Result).barra).Chiudi();
                //                genitore.SetBarraDiStatoTesto(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("ImportCompleted"), nomeFileDaAggiungere));
            }
        }

        private string[] ConvertiOsisARtfETesto(XmlNode xn)
        {
            StringBuilder rtf = new StringBuilder("");
            StringBuilder testo = new StringBuilder("");
            string[] testoDelSottoNodo = { "", "" };
            int p, i;
            string lemma, morph;
            bool parolaFatta;
            if (xn.PreviousSibling != null && xn.PreviousSibling.Name == "title")
            {
                XmlNode nodoForseConTitolo = xn.PreviousSibling;
                if (nodoForseConTitolo.Attributes["type"] != null && nodoForseConTitolo.Attributes["type"].Value == "psalm")
                {
                    nodoForseConTitolo = nodoForseConTitolo.PreviousSibling;
                    // quando c'è un titolo inglese prima del titolo canonico, viene aggiunto in ConvertiOsisARtfETesto
                    rtf.Append(@"\lptit1 ").Append(ConvertiOsisARtfETesto(xn.PreviousSibling)[0]).Append(@"\lptit0 \par ");
                }
                else if (nodoForseConTitolo.Attributes["type"] == null || nodoForseConTitolo.Attributes["type"].Value != "chapter")
                {
                    rtf.Append(@"\par\lptit1 ").Append(nodoForseConTitolo.InnerText).Append(@"\lptit0 \par ");
                }
            }
            if (xn.Name == "#text")
            {
                rtf.Append(xn.InnerText);
                testo.Append(xn.InnerText);
            }
            foreach (XmlNode nodo in xn.ChildNodes)
            {
                if (nodo.HasChildNodes)
                {
                    switch (nodo.Name)
                    {
                        case "head":
                            rtf.Append(@"\lptit1 ").Append(ConvertiOsisARtfETesto(nodo)[0]).Append(@"\lptit0 ");
                            break;
                        case "q":
                            if (nodo.Attributes["who"] != null && nodo.Attributes["who"].Value == "Jesus")
                            {
                                testoDelSottoNodo = ConvertiOsisARtfETesto(nodo);
                                rtf.Append(testoDelSottoNodo[0]);
                                testo.Append(testoDelSottoNodo[1]);
                            }
                            else
                            {
                                testoDelSottoNodo = ConvertiOsisARtfETesto(nodo);
                                if (rtf.ToString().EndsWith("}", StringComparison.Ordinal))
                                {
                                    // quando ci sono due parole, con uno spazio non corsivo in mezzo,
                                    // lo spazio è saltato perché InnerText non può essere vuoto
                                    rtf.Append(" ");
                                    testo.Append(" ");
                                }
                                if (testoDelSottoNodo[0].EndsWith(@"\par ", StringComparison.Ordinal))
                                    testoDelSottoNodo[0] = testoDelSottoNodo[0].Remove(testoDelSottoNodo[0].Length - 1);
                                rtf.Append(@"{\i1 ").Append(testoDelSottoNodo[0]).Append("}");
                                testo.Append(testoDelSottoNodo[1]);
                            }
                            break;
                        case "hi":
                            if (nodo.Attributes["type"] != null)
                            {
                                if (rtf.ToString().EndsWith("}", StringComparison.Ordinal))
                                {
                                    // quando ci sono due parole, con uno spazio non corsivo in mezzo,
                                    // lo spazio è saltato perché InnerText non può essere vuoto
                                    rtf.Append(" ");
                                    testo.Append(" ");
                                }
                                if (nodo.Attributes["type"] != null && nodo.Attributes["type"].Value == "italic")
                                {
                                    testoDelSottoNodo = ConvertiOsisARtfETesto(nodo);
                                    rtf.Append(@"{\i1 ").Append(testoDelSottoNodo[0]).Append("}");
                                    testo.Append(testoDelSottoNodo[1]);
                                }
                                if (nodo.Attributes["type"] != null && nodo.Attributes["type"].Value == "bold")
                                {
                                    testoDelSottoNodo = ConvertiOsisARtfETesto(nodo);
                                    rtf.Append(@"{\b1 ").Append(testoDelSottoNodo[0]).Append("}");
                                    testo.Append(testoDelSottoNodo[1]);
                                }
                                else if (nodo.Attributes["type"] != null && nodo.Attributes["type"].Value == "small-caps")
                                {
                                    testoDelSottoNodo = ConvertiOsisARtfETesto(nodo);
                                    rtf.Append(@"{\caps ").Append(testoDelSottoNodo[0]).Append("}");
                                    testo.Append(testoDelSottoNodo[1]);
                                }
                                else if (nodo.Attributes["type"] != null && nodo.Attributes["type"].Value == "super")
                                {
                                    testoDelSottoNodo = ConvertiOsisARtfETesto(nodo);
                                    rtf.Append(@"{\super ").Append(testoDelSottoNodo[0]).Append("}");
                                    testo.Append(testoDelSottoNodo[1]);
                                }
                                else
                                    throw new FormatException(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("ImportUnknownCode"), nodo.Name));
                            }
                            else
                                throw new FormatException(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("ImportUnknownCode"), nodo.Name));
                            break;
                        case "title":
                            if (nodo.Attributes["canonical"] != null && nodo.Attributes["canonical"].Value == "true")
                            {
                                testoDelSottoNodo = ConvertiOsisARtfETesto(nodo);
                                rtf.Append(@"{\b1 ").Append(testoDelSottoNodo[0]).Append("} ");
                                testo.Append(testoDelSottoNodo[1]).Append(" ");
                            }
                            else if (nodo.Attributes["type"] != null && nodo.Attributes["type"].Value == "psalm")
                            {
                                rtf.Append(@"\lptit1 ").Append(ConvertiOsisARtfETesto(nodo)[0]).Append(@"\lptit0 \par ");
                            }
                            else
                                throw new FormatException(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("ImportUnknownCode"), nodo.Name));
                            break;
                        case "transChange":
                            if (nodo.Attributes["type"] != null && nodo.Attributes["type"].Value == "added")
                            {
                                if (testo.Length > 0 && !testo.ToString().EndsWith(" ", StringComparison.Ordinal) && !testo.ToString().EndsWith("(", StringComparison.Ordinal))
                                {
                                    rtf.Append(" ");
                                    testo.Append(" ");
                                }
                                testoDelSottoNodo = ConvertiOsisARtfETesto(nodo);
                                rtf.Append(@"{\i1 ").Append(testoDelSottoNodo[0]).Append("} ");
                                testo.Append(testoDelSottoNodo[1]).Append(" ");
                            }
                            else
                                throw new FormatException(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("ImportUnknownCode"), nodo.Name));
                            break;
                        case "inscription":
                        case "seg":
                        case "foreign":
                            if (nodo.Name == "foreign" || nodo.Attributes.Count == 0)
                            {
                                testoDelSottoNodo = ConvertiOsisARtfETesto(nodo);
                                if (rtf.ToString().EndsWith("}", StringComparison.Ordinal) && testoDelSottoNodo[0].StartsWith("{", StringComparison.Ordinal))
                                {
                                    rtf.Append(" ");
                                    testo.Append(" ");
                                }
                                rtf.Append(testoDelSottoNodo[0]);
                                testo.Append(testoDelSottoNodo[1]);
                            }
                            else if (nodo.Name == "seg" && nodo.Attributes["subType"] != null && nodo.Attributes["subType"].Value == "x-added")
                            {
                                testoDelSottoNodo = ConvertiOsisARtfETesto(nodo);
                                rtf.Append(@"{\i1 ").Append(testoDelSottoNodo[0]).Append("}");
                                testo.Append(testoDelSottoNodo[1]);
                            }
                            else
                                throw new FormatException(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("ImportUnknownCode"), nodo.Name));
                            break;
                        case "divineName":
                            testoDelSottoNodo = ConvertiOsisARtfETesto(nodo);
                            rtf.Append(@"{\caps ").Append(testoDelSottoNodo[0]).Append("}");
                            testo.Append(testoDelSottoNodo[1]);
                            break;
                        case "w":
                            testoDelSottoNodo = ConvertiOsisARtfETesto(nodo);
                            // nella Apostolic Bible, le parole hanno spesso dei numeri attaccati prima per indicare l'ordine
                            for (i = testoDelSottoNodo[1].Length - 2; i >= 0; --i)
                            {
                                if (i == 0 || (i > 0 && testoDelSottoNodo[1][i - 1] == ' '))
                                {
                                    if (char.IsDigit(testoDelSottoNodo[1][i]) && char.IsLetter(testoDelSottoNodo[1][i + 1]))
                                        testoDelSottoNodo[1] = testoDelSottoNodo[1].Insert(i + 1, " ");
                                    else if (i < testoDelSottoNodo[1].Length - 2)
                                    {
                                        if (testoDelSottoNodo[1][i] == '[' && char.IsDigit(testoDelSottoNodo[1][i + 1]) && char.IsLetter(testoDelSottoNodo[1][i + 2]))
                                            testoDelSottoNodo[1] = testoDelSottoNodo[1].Insert(i + 2, " ");
                                        else if (char.IsDigit(testoDelSottoNodo[1][i]) && char.IsDigit(testoDelSottoNodo[1][i + 1]) && char.IsLetter(testoDelSottoNodo[1][i + 2]))
                                            testoDelSottoNodo[1] = testoDelSottoNodo[1].Insert(i + 2, " ");
                                        else if (i < testoDelSottoNodo[1].Length - 3)
                                        {
                                            if (testoDelSottoNodo[1][i] == '[' && char.IsDigit(testoDelSottoNodo[1][i + 1]) && char.IsDigit(testoDelSottoNodo[1][i + 2]) && char.IsLetter(testoDelSottoNodo[1][i + 3]))
                                                testoDelSottoNodo[1] = testoDelSottoNodo[1].Insert(i + 3, " ");
                                        }
                                    }
                                }
                            }
                            rtf.Append(testoDelSottoNodo[0]);
                            testo.Append(testoDelSottoNodo[1]);
                            lemma = (nodo.Attributes["lemma"] != null ? nodo.Attributes["lemma"].Value : "");
                            morph = (nodo.Attributes["morph"] != null ? nodo.Attributes["morph"].Value : "");
                            parolaFatta = false;
                            if (lemma.StartsWith("strong:", StringComparison.OrdinalIgnoreCase) || lemma.StartsWith("strongab:", StringComparison.OrdinalIgnoreCase))
                            {
                                while (lemma.IndexOf("strongab:", StringComparison.OrdinalIgnoreCase) > -1 || lemma.IndexOf("strong:", StringComparison.OrdinalIgnoreCase) > -1)
                                {
                                    if (lemma.IndexOf("strongab:", StringComparison.OrdinalIgnoreCase) > -1 && (lemma.IndexOf("strong:", StringComparison.OrdinalIgnoreCase) < 0 || lemma.IndexOf("strongab:", StringComparison.OrdinalIgnoreCase) < lemma.IndexOf("strong:", StringComparison.OrdinalIgnoreCase)))
                                        lemma = lemma.Substring(0, lemma.IndexOf("strongab:", StringComparison.OrdinalIgnoreCase)) + lemma.Substring(lemma.IndexOf("strongab:", StringComparison.OrdinalIgnoreCase) + 9);
                                    else
                                        lemma = lemma.Substring(0, lemma.IndexOf("strong:", StringComparison.OrdinalIgnoreCase)) + lemma.Substring(lemma.IndexOf("strong:", StringComparison.OrdinalIgnoreCase) + 7);
                                }
                                rtf.Append(@" {\super ").Append(lemma).Append("}");
                                testo.Append(" ").Append(lemma);
                                if (!morph.StartsWith("strongMorph:", StringComparison.OrdinalIgnoreCase))
                                {
                                    rtf.Append(" ");
                                    testo.Append(" ");
                                }
                                parolaFatta = true;
                            }
                            if (morph.StartsWith("strongMorph:", StringComparison.OrdinalIgnoreCase))
                            {
                                while (morph.IndexOf("strongMorph:", StringComparison.OrdinalIgnoreCase) > -1)
                                    morph = morph.Substring(0, morph.IndexOf("strongMorph:", StringComparison.OrdinalIgnoreCase)) + morph.Substring(morph.IndexOf("strongMorph:", StringComparison.OrdinalIgnoreCase) + 12);
                                rtf.Append(@" {\super ").Append(morph).Append("} ");
                                testo.Append(" ").Append(morph).Append(" ");
                                parolaFatta = true;
                            }
                            if (!parolaFatta) // cioè non c'è nessuno dei casi precedenti
                            {
                                rtf.Append(" ");
                                testo.Append(" ");
                            }
                            break;
                        case "div":
                            if (nodo.Attributes["type"] != null && nodo.Attributes["type"].Value == "colophon")
                            {
                                testoDelSottoNodo = ConvertiOsisARtfETesto(nodo);
                                rtf.Append(@" {\i1 ").Append(testoDelSottoNodo[0]).Append("}");
                                testo.Append(" ").Append(testoDelSottoNodo[1]);
                            }
                            break;
                        case "note": // le note non sono importate
                        case "reference":
                            break;
                        default:
                            throw new FormatException(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("ImportUnknownCode"), nodo.Name));
                    }
                }
                else
                {
                    if ((nodo.Name == "p" && nodo.Attributes[0].Name == "sID") || (nodo.Name == "milestone" && nodo.Attributes["type"] != null && nodo.Attributes["type"].Value.EndsWith("-p", StringComparison.OrdinalIgnoreCase)))
                    {
                        rtf.Append(@"\par ");
                        testo.Append(" ");
                    }
                    string testoDaAggiungere = ConvertiApostrofeTrattino(nodo.InnerText);
                    if (nodo.Name == "#text" && nodo.ParentNode != null && (nodo.ParentNode.Name == "verse" || nodo.ParentNode.Name == "q" || nodo.ParentNode.Name == "title" || nodo.ParentNode.Name == "div" || nodo.ParentNode.Name == "w"))
                    {
                        if (nodo.PreviousSibling == null)
                            testoDaAggiungere = testoDaAggiungere.TrimStart();
                        else
                        {
                            if (nodo.PreviousSibling.Name == "w" || nodo.PreviousSibling.Name == "transChange")
                            {
                                if (rtf.Length > 0)
                                    rtf = rtf.Remove(rtf.Length - 1, 1);
                                if (testo.Length > 0)
                                    testo = testo.Remove(testo.Length - 1, 1);
                            }
                        }
                        if (nodo.NextSibling == null)
                            testoDaAggiungere = testoDaAggiungere.TrimEnd();
                    }
                    rtf.Append(testoDaAggiungere);
                    testo.Append(testoDaAggiungere);
                }
            }
            string rtfStringa = rtf.ToString();
            while (rtfStringa.IndexOf(@"\lptit0 \par ", StringComparison.Ordinal) > -1)
            {
                p = rtfStringa.IndexOf(@"\lptit0 \par ", StringComparison.Ordinal);
                rtfStringa = rtfStringa.Substring(0, p) + @"\par\lptit0 " + rtfStringa.Substring(p + 13);
            }
            return new string[] { rtfStringa, testo.ToString() };
        }

        private static string[] ConvertiZefaniaARtfETesto(XmlNode xn)
        {
            string s = xn.InnerXml.Replace("<DIV>", "").Replace("</DIV>", "");

            while (s.IndexOf("<NOTE", StringComparison.Ordinal) > -1)
            {
                int inizioTag = s.IndexOf("<NOTE", StringComparison.Ordinal);
                int fineTag = s.IndexOf(">", inizioTag, StringComparison.Ordinal);
                if (s[fineTag - 1] == '/')
                {  // una nota vuota, che chiude se stessa <note... />
                    s = s.Substring(0, inizioTag) + s.Substring(fineTag + 1);
                }
                else
                {
                    int tagFine = s.IndexOf("</NOTE>", fineTag, StringComparison.Ordinal);
                    string nota = s.Substring(fineTag + 1, tagFine - fineTag - 1).Trim();
                    s = s.Substring(0, inizioTag) + @"\{" + nota + @"\}" + s.Substring(tagFine + 7);
                }
            }

            string sTesto = s;
            s = SostituisciHtmlTag(s, "STYLE css=\"font-style:italic\"", @"{\i ", @"}");
            s = SostituisciHtmlTag(s, "STYLE css=\"text-decoration:underline\"", @"{\ul ", @"}");
            s = SostituisciHtmlTag(s, "STYLE id=\"cl:divineName\"", @"{\caps ", @"}");
            sTesto = SostituisciHtmlTag(sTesto, "STYLE css=\"font-style:italic\"", "", "");
            sTesto = SostituisciHtmlTag(sTesto, "STYLE css=\"text-decoration:underline\"", "", "");
            sTesto = SostituisciHtmlTag(sTesto, "STYLE id=\"cl:divineName\"", "", "");

            if (s.Contains("<"))
                throw new FormatException(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("ImportUnknownCode"), xn.Name));
            return new string[] { s, sTesto };
        }

        private void BtnSfogliaXML_Click(object sender, EventArgs e)
        {
            string ultimaDirectory;
            switch (tipo)
            {
                case TipoImportazione.ImportaBibbia:
                case TipoImportazione.ImportaZefania:
                case TipoImportazione.ImportaBibleworks:
                case TipoImportazione.ImportaThml:
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    ultimaDirectory = Settings.Default.ImportaXMLDirectory;
                    if (String.IsNullOrEmpty(ultimaDirectory))
                        ultimaDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    openFileDialog.InitialDirectory = ultimaDirectory;
                    openFileDialog.Filter = (tipo == TipoImportazione.ImportaBibleworks ? Principale.LocRM.GetString("ImportTextFilter") : Principale.LocRM.GetString("ImportXmlFilter"));
                    openFileDialog.CheckFileExists = true;
                    openFileDialog.CheckPathExists = true;
                    if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                    {
                        Settings.Default.ImportaXMLDirectory = Path.GetDirectoryName(openFileDialog.FileName);
                        cbNomeFileXmlODirectory.Text = openFileDialog.FileName;
                    }
                    break;
                case TipoImportazione.ImportaNote:
                    ultimaDirectory = Settings.Default.ImportaDirectory;
                    if (String.IsNullOrEmpty(ultimaDirectory))
                        ultimaDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

                    using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
                    {
                        folderBrowserDialog.SelectedPath = ultimaDirectory;
                        folderBrowserDialog.ShowNewFolderButton = false;
                        folderBrowserDialog.Description = Principale.LocRM.GetString("ImportDirectoryDescription");
                        if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
                        {
                            Settings.Default.ImportaDirectory = folderBrowserDialog.SelectedPath;
                            cbNomeFileXmlODirectory.Text = folderBrowserDialog.SelectedPath;
                        }
                    }
                    break;
                case TipoImportazione.ImportaEsword:
                    OpenFileDialog openFileDialogEsword = new OpenFileDialog();
                    ultimaDirectory = Settings.Default.ImportaEswordDirectory;
                    if (String.IsNullOrEmpty(ultimaDirectory))
                        ultimaDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    openFileDialogEsword.InitialDirectory = ultimaDirectory;
                    openFileDialogEsword.Filter = Principale.LocRM.GetString("ImportEswordFilter");
                    openFileDialogEsword.CheckFileExists = true;
                    openFileDialogEsword.CheckPathExists = true;
                    if (openFileDialogEsword.ShowDialog(this) == DialogResult.OK)
                    {
                        Settings.Default.ImportaEswordDirectory = Path.GetDirectoryName(openFileDialogEsword.FileName);
                        cbNomeFileXmlODirectory.Text = openFileDialogEsword.FileName;
                    }
                    break;
                case TipoImportazione.NuovaNote:
                    break;
                default:
                    break;
            }
        }

        private void CbNomeFileXML_TextChanged(object sender, EventArgs e)
        {
            bool fileEsiste = false;
            switch (tipo)
            {
                case TipoImportazione.ImportaBibbia:
                case TipoImportazione.ImportaZefania:
                case TipoImportazione.ImportaBibleworks:
                case TipoImportazione.ImportaThml:
                case TipoImportazione.ImportaEsword:
                    fileEsiste = File.Exists(cbNomeFileXmlODirectory.Text);
                    break;
                case TipoImportazione.ImportaNote:
                    fileEsiste = Directory.Exists(cbNomeFileXmlODirectory.Text);
                    break;
                case TipoImportazione.NuovaNote:
                    break;
                default:
                    break;
            }

            tbAbbreviazione.Enabled = fileEsiste;
            tbNomeFileLP.Enabled = fileEsiste;
            tbTitolo.Enabled = fileEsiste;
            tbAutore.Enabled = fileEsiste;
            tbCasaEd.Enabled = fileEsiste;
            tbCopyright.Enabled = fileEsiste;
            tbDescrizione.Enabled = fileEsiste;
            tbData.Enabled = fileEsiste;
            tbISBN.Enabled = fileEsiste;
            tbLingua.Enabled = fileEsiste;
            tbVersioneDelleNote.Enabled = fileEsiste;
            btnOK.Enabled = fileEsiste;

            if (fileEsiste)
            {
                string nomeVersioneUtilizzato;
                Cursor cursoreAttuale = Cursor.Current;
                Cursor.Current = Cursors.WaitCursor;

                switch (tipo)
                {
                    #region ImportaBibbia
                    case TipoImportazione.ImportaBibbia:
                        try
                        {
                            XmlDocument xd = new XmlDocument();
                            xd.Load(cbNomeFileXmlODirectory.Text);
                            XmlNamespaceManager nspmgr = new XmlNamespaceManager(xd.NameTable);
                            nspmgr.AddNamespace("nsp", xd.ChildNodes[1].NamespaceURI);
                            Application.DoEvents();

                            XmlNode xsn = xd.DocumentElement.SelectSingleNode("nsp:osisText/nsp:header/nsp:work/nsp:title", nspmgr);
                            if (xsn != null)
                                tbTitolo.Text = xsn.InnerText;
                            else
                                tbTitolo.Text = "";

                            xsn = xd.DocumentElement.SelectSingleNode("nsp:osisText", nspmgr);
                            if (xsn != null)
                                tbAbbreviazione.Text = xsn.Attributes["osisIDWork"].Value;
                            else
                                tbAbbreviazione.Text = CreaAbbreviazione(tbTitolo.Text);

                            xsn = xd.DocumentElement.SelectSingleNode("nsp:osisText/nsp:header/nsp:work/nsp:publisher", nspmgr);
                            if (xsn != null)
                                tbCasaEd.Text = xsn.InnerText.Replace("\r\n", " ");
                            else
                                tbCasaEd.Text = "";

                            xsn = xd.DocumentElement.SelectSingleNode("nsp:osisText/nsp:header/nsp:work/nsp:description", nspmgr);
                            if (xsn != null)
                                tbDescrizione.Text = xsn.InnerText;
                            else
                                tbDescrizione.Text = "";

                            xsn = xd.DocumentElement.SelectSingleNode("nsp:osisText/nsp:header/nsp:work/nsp:language", nspmgr);
                            if (xsn != null)
                                tbLingua.Text = xsn.InnerText.ToLowerInvariant();
                            else
                                tbLingua.Text = "";
                            if (tbLingua.Text.Length >= 4)
                                tbLingua.Text = tbLingua.Text.Substring(0, 3);

                            tbCopyright.Text = "";
                            XmlNodeList nlHead = xd.DocumentElement.SelectNodes("nsp:osisText/nsp:header/nsp:work/nsp:rights", nspmgr);
                            foreach (XmlNode xn in nlHead)
                            {
                                if (string.IsNullOrEmpty(tbCopyright.Text))
                                {
                                    if (xn.Attributes.Count == 0 || xn.Attributes[0].Value == "copyright")
                                        tbCopyright.Text = xn.InnerText.Replace("\r\n", " ");
                                }
                            }

                            tbData.Text = "";
                            nlHead = xd.DocumentElement.SelectNodes("nsp:osisText/nsp:header/nsp:work/nsp:date", nspmgr);
                            bool dataOriginaleTrovata = false;
                            foreach (XmlNode xn in nlHead)
                            {
                                if (!dataOriginaleTrovata)
                                {
                                    tbData.Text = xn.InnerText;
                                    if (xn.Attributes.Count > 0 && xn.Attributes[0].Value == "original")
                                        dataOriginaleTrovata = true;
                                }
                            }

                            tbISBN.Text = "";
                            nlHead = xd.DocumentElement.SelectNodes("nsp:osisText/nsp:header/nsp:work/nsp:identifier", nspmgr);
                            foreach (XmlNode xn in nlHead)
                                if (xn.Attributes[0].Value == "ISBN")
                                    tbISBN.Text = xn.InnerText;

                            tbVersioneDelleNote.Text = "";

                            nomeVersioneUtilizzato = ImpostaNomeFileLaParolaDaFileOrigine();
                        }
                        catch (Exception exc)
                        {
                            tbAbbreviazione.Enabled = false;
                            tbNomeFileLP.Enabled = false;
                            tbTitolo.Enabled = false;
                            tbAutore.Enabled = false;
                            tbCasaEd.Enabled = false;
                            tbCopyright.Enabled = false;
                            tbDescrizione.Enabled = false;
                            tbData.Enabled = false;
                            tbISBN.Enabled = false;
                            tbLingua.Enabled = false;
                            tbVersioneDelleNote.Enabled = false;
                            btnOK.Enabled = false;
                            MessageBox.Show(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("ImportError"), exc.Message), Principale.LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions);
                        }
                        finally
                        {
                            Cursor.Current = cursoreAttuale;
                            cursoreAttuale?.Dispose();
                        }
                        break;
                    #endregion
                    #region ImportaZefania
                    case TipoImportazione.ImportaZefania:
                        try
                        {
                            XmlDocument xd = new XmlDocument();
                            xd.Load(cbNomeFileXmlODirectory.Text);
                            Application.DoEvents();

                            XmlNode xsn = xd.DocumentElement.SelectSingleNode("INFORMATION/description");
                            tbDescrizione.Text = (xsn != null ? xsn.InnerText : "");

                            xsn = xd.DocumentElement.SelectSingleNode("INFORMATION/title");
                            tbTitolo.Text = (xsn != null ? xsn.InnerText : "");
                            if (string.IsNullOrEmpty(tbTitolo.Text))
                            {
                                if (xd.DocumentElement.Attributes["biblename"] != null)
                                    tbTitolo.Text = xd.DocumentElement.Attributes["biblename"].Value;
                            }

                            xsn = xd.DocumentElement.SelectSingleNode("INFORMATION/identifier");
                            tbAbbreviazione.Text = (xsn != null ? xsn.InnerText : "");
                            if (tbAbbreviazione.Text.ToUpperInvariant().StartsWith("BIBLE.", StringComparison.Ordinal))
                                tbAbbreviazione.Text = tbAbbreviazione.Text.Substring(6);
                            if (string.IsNullOrEmpty(tbAbbreviazione.Text))
                                tbAbbreviazione.Text = CreaAbbreviazione(tbTitolo.Text);

                            xsn = xd.DocumentElement.SelectSingleNode("INFORMATION/publisher");
                            tbCasaEd.Text = (xsn != null ? xsn.InnerText : "");

                            xsn = xd.DocumentElement.SelectSingleNode("INFORMATION/language");
                            if (xsn != null)
                                tbLingua.Text = xsn.InnerText.ToLowerInvariant();
                            else
                            {
                                if (xd.DocumentElement.Attributes["lgid"] != null)
                                    tbLingua.Text = xd.DocumentElement.Attributes["lgid"].Value.ToLowerInvariant();
                                else
                                    tbLingua.Text = "";
                            }
                            if (tbLingua.Text.Length >= 3)
                                tbLingua.Text = tbLingua.Text.Substring(0, 2);

                            xsn = xd.DocumentElement.SelectSingleNode("INFORMATION/rights");
                            tbCopyright.Text = (xsn != null ? xsn.InnerText : "");

                            xsn = xd.DocumentElement.SelectSingleNode("INFORMATION/date");
                            tbData.Text = (xsn != null ? xsn.InnerText : "");

                            tbISBN.Text = "";

                            tbVersioneDelleNote.Text = "";

                            nomeVersioneUtilizzato = ImpostaNomeFileLaParolaDaFileOrigine();
                        }
                        catch (Exception exc)
                        {
                            tbAbbreviazione.Enabled = false;
                            tbNomeFileLP.Enabled = false;
                            tbTitolo.Enabled = false;
                            tbAutore.Enabled = false;
                            tbCasaEd.Enabled = false;
                            tbCopyright.Enabled = false;
                            tbDescrizione.Enabled = false;
                            tbData.Enabled = false;
                            tbISBN.Enabled = false;
                            tbLingua.Enabled = false;
                            tbVersioneDelleNote.Enabled = false;
                            btnOK.Enabled = false;
                            MessageBox.Show(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("ImportError"), exc.Message), Principale.LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions);
                        }
                        finally
                        {
                            Cursor.Current = cursoreAttuale;
                            cursoreAttuale?.Dispose();
                        }
                        break;
                    #endregion
                    #region ImportaBibleworks
                    case TipoImportazione.ImportaBibleworks:
                        try
                        {
                            string nomeFileTesto = Path.GetFileNameWithoutExtension(cbNomeFileXmlODirectory.Text);
                            tbAbbreviazione.Text = CreaAbbreviazione(nomeFileTesto);
                            int suffisso = 0;
                            nomeFileTesto = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + nomeFileTesto;
                            nomeVersioneUtilizzato = nomeFileTesto;
                            while (File.Exists(nomeVersioneUtilizzato + ".laparola"))
                            {
                                suffisso += 1;
                                nomeVersioneUtilizzato = nomeFileTesto + suffisso.ToString(CultureInfo.InvariantCulture);
                            }
                            tbNomeFileLP.Text = nomeVersioneUtilizzato + ".laparola";
                        }
                        finally
                        {
                            Cursor.Current = cursoreAttuale;
                            cursoreAttuale?.Dispose();
                        }
                        break;
                    #endregion
                    #region ImportaNote
                    case TipoImportazione.ImportaNote:
                        try
                        {
                            string nomeDirectory = cbNomeFileXmlODirectory.Text;
                            if (nomeDirectory.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                                nomeDirectory = nomeDirectory.Remove(nomeDirectory.Length - 1, 1);
                            if (nomeDirectory.IndexOf(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal) > 0)
                                nomeDirectory = nomeDirectory.Substring(nomeDirectory.LastIndexOf(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal) + 1);

                            tbAbbreviazione.Text = CreaAbbreviazione(nomeDirectory);

                            int suffisso = 0;
                            nomeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + nomeDirectory;
                            nomeVersioneUtilizzato = nomeDirectory;
                            while (File.Exists(nomeVersioneUtilizzato + ".laparola"))
                            {
                                suffisso += 1;
                                nomeVersioneUtilizzato = nomeDirectory + suffisso.ToString(CultureInfo.InvariantCulture);
                            }
                            tbNomeFileLP.Text = nomeVersioneUtilizzato + ".laparola";
                        }
                        finally
                        {
                            Cursor.Current = cursoreAttuale;
                            cursoreAttuale?.Dispose();
                        }
                        break;
                    #endregion
                    #region ImportaThml
                    case TipoImportazione.ImportaThml:
                        try
                        {
                            XmlDocument xd = new XmlDocument();
                            xd.Load(cbNomeFileXmlODirectory.Text);
                            Application.DoEvents();

                            XmlNode xsn = xd.DocumentElement.SelectSingleNode("ThML.head/generalInfo/description");
                            tbDescrizione.Text = (xsn != null ? xsn.InnerText : "");

                            xsn = xd.DocumentElement.SelectSingleNode("ThML.head/generalInfo/firstPublished");
                            tbData.Text = (xsn != null ? xsn.InnerText : "");

                            xsn = xd.DocumentElement.SelectSingleNode("ThML.head/printSourceInfo/published");
                            tbCasaEd.Text = (xsn != null ? xsn.InnerText : "");
                            if (string.IsNullOrEmpty(tbCasaEd.Text))
                            {
                                XmlNodeList nl = xd.DocumentElement.SelectNodes("ThML.head/electronicEdInfo/DC/DC.Source");
                                foreach (XmlNode nodo in nl)
                                {
                                    if (nodo.Attributes["sub"] != null && nodo.Attributes["sub"].Value == "PrintEdition")
                                        tbCasaEd.Text = nodo.InnerText;
                                }
                            }

                            xsn = xd.DocumentElement.SelectSingleNode("ThML.head/electronicEdInfo/bookID");
                            tbAbbreviazione.Text = (xsn != null ? xsn.InnerText : "");
                            if (!string.IsNullOrEmpty(tbAbbreviazione.Text))
                                tbAbbreviazione.Text = tbAbbreviazione.Text.Substring(0, 1).ToUpperInvariant() + tbAbbreviazione.Text.Substring(1);

                            xsn = xd.DocumentElement.SelectSingleNode("ThML.head/electronicEdInfo/DC/DC.Title");
                            tbTitolo.Text = (xsn != null ? xsn.InnerText : "");

                            xsn = xd.DocumentElement.SelectSingleNode("ThML.head/electronicEdInfo/DC/DC.Creator");
                            tbAutore.Text = (xsn != null ? xsn.InnerText : "");

                            xsn = xd.DocumentElement.SelectSingleNode("ThML.head/electronicEdInfo/DC/DC.Description");
                            string descrizione2 = (xsn != null ? xsn.InnerText : "");
                            if (!string.IsNullOrEmpty(tbDescrizione.Text) && !string.IsNullOrEmpty(descrizione2))
                                tbDescrizione.Text += " ";
                            tbDescrizione.Text += descrizione2;
                            tbDescrizione.Text = tbDescrizione.Text.Replace("\n", " ").Replace("\t", " ").Trim();
                            while (tbDescrizione.Text.IndexOf("  ", StringComparison.Ordinal) > 0)
                                tbDescrizione.Text = tbDescrizione.Text.Replace("  ", " ");

                            xsn = xd.DocumentElement.SelectSingleNode("ThML.head/electronicEdInfo/DC/DC.Language");
                            tbLingua.Text = (xsn != null ? xsn.InnerText.ToLowerInvariant() : "");
                            if (tbLingua.Text.Length > 2)
                                tbLingua.Text = tbLingua.Text.Substring(0, 2);

                            xsn = xd.DocumentElement.SelectSingleNode("ThML.head/electronicEdInfo/DC/DC.Rights");
                            tbCopyright.Text = (xsn != null ? xsn.InnerText : "");

                            XmlNodeList nl2 = xd.DocumentElement.SelectNodes("ThML.head/electronicEdInfo/DC/DC.Subject");
                            foreach (XmlNode nodo in nl2)
                            {
                                if (nodo.Attributes["scheme"] != null && nodo.Attributes["scheme"].Value == "LCCN")
                                    tbISBN.Text = nodo.InnerText;
                            }

                            xsn = xd.DocumentElement.SelectSingleNode("ThML.head/electronicEdInfo/DC/DC.Type");
                            switch (xsn.InnerText)
                            {
                                case "Text.Bible":
                                    thmlTipo = TipoThML.Bibbia;
                                    break;
                                case "Text.Commentary":
                                    thmlTipo = TipoThML.Commentario;
                                    break;
                            }

                            tbVersioneDelleNote.Text = "";

                            nomeVersioneUtilizzato = ImpostaNomeFileLaParolaDaFileOrigine();
                        }
                        catch (Exception exc)
                        {
                            tbAbbreviazione.Enabled = false;
                            tbNomeFileLP.Enabled = false;
                            tbTitolo.Enabled = false;
                            tbAutore.Enabled = false;
                            tbCasaEd.Enabled = false;
                            tbCopyright.Enabled = false;
                            tbDescrizione.Enabled = false;
                            tbData.Enabled = false;
                            tbISBN.Enabled = false;
                            tbLingua.Enabled = false;
                            tbVersioneDelleNote.Enabled = false;
                            btnOK.Enabled = false;
                            MessageBox.Show(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("ImportError"), exc.Message), Principale.LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions);
                        }
                        finally
                        {
                            Cursor.Current = cursoreAttuale;
                            cursoreAttuale?.Dispose();
                        }
                        break;
                    #endregion
                    #region ImportaEsword
                    case TipoImportazione.ImportaEsword:
                        tbCasaEd.Text = "";
                        tbCopyright.Text = "";
                        tbData.Text = "";
                        tbISBN.Text = "";
                        tbLingua.Text = "";

                        try
                        {
                            switch (Path.GetExtension(cbNomeFileXmlODirectory.Text.ToUpperInvariant()))
                            {
                                case ".CMT":
                                    eswordTipo = TipoEsword.Commentario;
                                    break;
                                case ".DCT":
                                    eswordTipo = TipoEsword.Dizionario;
                                    break;
                                case ".TOP":
                                    eswordTipo = TipoEsword.Tema;
                                    break;
                                default: // incluso .bbt
                                    eswordTipo = TipoEsword.Bibbia;
                                    break;
                            }

                            dataSetEsword = ConnectToData("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + cbNomeFileXmlODirectory.Text, eswordTipo);
                            try
                            {
                                tbTitolo.Text = dataSetEsword.Tables["Details"].Rows[0]["Description"].ToString();
                            }
                            catch // in qualche testo manca la tabella Details
                            {
                                tbTitolo.Text = Path.GetFileNameWithoutExtension(cbNomeFileXmlODirectory.Text);
                            }
                            try
                            {
                                tbAbbreviazione.Text = dataSetEsword.Tables["Details"].Rows[0]["Abbreviation"].ToString();
                            }
                            catch
                            {
                                tbAbbreviazione.Text = CreaAbbreviazione(tbTitolo.Text);
                            }
                            try
                            {
                                tbDescrizione.Text = ConvertiESwordARtf(dataSetEsword.Tables["Details"].Rows[0]["Comments"].ToString());
                            }
                            catch
                            {
                                tbDescrizione.Text = "";
                            }

                            nomeVersioneUtilizzato = ImpostaNomeFileLaParolaDaFileOrigine();
                        }
                        catch (Exception exc)
                        {
                            tbAbbreviazione.Enabled = false;
                            tbNomeFileLP.Enabled = false;
                            tbTitolo.Enabled = false;
                            tbAutore.Enabled = false;
                            tbCasaEd.Enabled = false;
                            tbCopyright.Enabled = false;
                            tbDescrizione.Enabled = false;
                            tbData.Enabled = false;
                            tbISBN.Enabled = false;
                            tbLingua.Enabled = false;
                            tbVersioneDelleNote.Enabled = false;
                            btnOK.Enabled = false;
                            if (exc.GetType().FullName == "System.Data.OleDb.OleDbException")
                                MessageBox.Show(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("ImportErrorPassword"), exc.Message), Principale.LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions);
                            else
                                MessageBox.Show(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("ImportError"), exc.Message), Principale.LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions);
                        }
                        break;
                    #endregion
                    case TipoImportazione.NuovaNote:
                        Cursor.Current = cursoreAttuale;
                        cursoreAttuale?.Dispose();
                        break;
                    default:
                        break;
                }
            }
        }

        private static string CreaAbbreviazione(string nomeDirectory)
        {
            string[] directoryParole = nomeDirectory.Trim().Split(' ');
            if (directoryParole.Length > 1)
            {
                StringBuilder abbreviazione = new StringBuilder("");
                foreach (string s in directoryParole)
                    abbreviazione.Append(s[0]);
                return abbreviazione.ToString();
            }
            else
            {
                if (nomeDirectory.Length >= 4)
                    return nomeDirectory.Substring(0, 3);
                else
                    return nomeDirectory;
            }
        }

        private string ImpostaNomeFileLaParolaDaFileOrigine()
        {
            int suffisso = 0;
            string nomeVersioneUtilizzato;
            string nomeVersione = Path.GetFileNameWithoutExtension(cbNomeFileXmlODirectory.Text);
            nomeVersioneUtilizzato = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + nomeVersione;
            while (File.Exists(nomeVersioneUtilizzato + ".laparola"))
            {
                suffisso += 1;
                nomeVersioneUtilizzato = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + nomeVersione + suffisso.ToString(CultureInfo.InvariantCulture);
            }
            tbNomeFileLP.Text = nomeVersioneUtilizzato + ".laparola";
            return nomeVersioneUtilizzato;
        }

        private void TbNuovaCollezione_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbNuovaCollezione.Text))
            {
                tbNomeFileLP.Text = "";
                btnOK.Enabled = false;
            }
            else
            {
                int suffisso = 0;
                string nomeNuovoFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + tbNuovaCollezione.Text;
                string nomeFileUtilizzato = nomeNuovoFile;
                while (File.Exists(nomeFileUtilizzato + ".laparola"))
                {
                    suffisso += 1;
                    nomeFileUtilizzato = nomeNuovoFile + suffisso.ToString(CultureInfo.InvariantCulture);
                }
                tbNomeFileLP.Text = nomeFileUtilizzato + ".laparola";
                btnOK.Enabled = !(Principale.testi.VersioneEsiste(tbNuovaCollezione.Text));
            }
        }

        private void TbNomeFileLP_TextChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = !File.Exists(tbNomeFileLP.Text);
        }

        private static string ConvertiLink(string s)
        {
            // converte i link ipertestuali dal formato della versione 6 a quella della versione 7

            // a volte, testo è sottolineato e colorato senza che sia un collegamento (per esempi, Fathers of the Church).
            // In quel caso, usa la prossima riga.
            //return s;

            if (!s.StartsWith(@"{\rtf1", StringComparison.Ordinal))
                return s;
            int inizioColori = s.IndexOf(@"{\colortbl", StringComparison.Ordinal);
            if (inizioColori < 0)
                return s;
            int fineColori = s.IndexOf("}", inizioColori, StringComparison.Ordinal);
            if (fineColori < 0)
                return s;
            int puntoVirgola = s.IndexOf(";", inizioColori, StringComparison.Ordinal);
            int puntoVirgolaPrecedente = inizioColori + 9;
            string coloreStringa = "";
            int coloreNumero = 0;
            int blu = -1, verde = -1, rosso = -1;
            while (puntoVirgola > 0 && puntoVirgola < fineColori)
            {
                coloreStringa = s.Substring(puntoVirgolaPrecedente + 1, puntoVirgola - puntoVirgolaPrecedente - 1).Trim();
                switch (coloreStringa)
                {
                    case @"\red0\green128\blue0":
                        verde = coloreNumero;
                        break;
                    case @"\red0\green0\blue255":
                        blu = coloreNumero;
                        break;
                    case @"\red255\green0\blue0":
                        rosso = coloreNumero;
                        break;
                }
                puntoVirgolaPrecedente = puntoVirgola;
                puntoVirgola = s.IndexOf(";", puntoVirgola + 1, StringComparison.Ordinal);
                ++coloreNumero;
            }
            s = ConvertiColore(s, verde, RichTextBoxEx.FineLinkBrano);
            s = ConvertiColore(s, blu, RichTextBoxEx.FineLinkNota);
            s = ConvertiColore(s, rosso, RichTextBoxEx.FineLinkFile);
            return s;
        }

        private static string ConvertiColore(string s, int colore, char tipo)
        {
            string codiceLink = @"\cf" + colore + @"\ul";
            int inizioLink = s.IndexOf(codiceLink, StringComparison.Ordinal);
            string codiceFine = "", testoLink = "";
            int fineLink, fineLink2;
            while (inizioLink > 0)
            {
                codiceFine = @"\cf0\ulnone";
                fineLink = s.IndexOf(codiceFine, inizioLink, StringComparison.Ordinal);
                fineLink2 = s.IndexOf(@"\cf1\ulnone", inizioLink, StringComparison.Ordinal);
                if ((fineLink2 > 0 && fineLink2 < fineLink) || fineLink == -1)
                {
                    fineLink = fineLink2;
                    codiceFine = @"\cf1\ulnone";
                }
                fineLink2 = s.IndexOf(@"\cf2\ulnone", inizioLink, StringComparison.Ordinal);
                if ((fineLink2 > 0 && fineLink2 < fineLink) || fineLink == -1)
                {
                    fineLink = fineLink2;
                    codiceFine = @"\cf2\ulnone";
                }
                fineLink2 = s.IndexOf(@"\cf3\ulnone", inizioLink, StringComparison.Ordinal);
                if ((fineLink2 > 0 && fineLink2 < fineLink) || fineLink == -1)
                {
                    fineLink = fineLink2;
                    codiceFine = @"\cf3\ulnone";
                }
                fineLink2 = s.IndexOf(@"\cf4\ulnone", inizioLink, StringComparison.Ordinal);
                if ((fineLink2 > 0 && fineLink2 < fineLink) || fineLink == -1)
                {
                    fineLink = fineLink2;
                    codiceFine = @"\cf4\ulnone";
                }
                fineLink2 = s.IndexOf(@"\plain", inizioLink, StringComparison.Ordinal);
                if ((fineLink2 > 0 && fineLink2 < fineLink) || fineLink == -1)
                {
                    fineLink = fineLink2;
                    codiceFine = @"\plain";
                }
                if (fineLink == -1 /*&& s[inizioLink - 1] == '}'*/)
                {
                    fineLink2 = s.IndexOf("}", inizioLink, StringComparison.Ordinal);
                    if (fineLink2 > 0)
                    {
                        fineLink = fineLink2;
                        codiceFine = "}";
                    }
                }
                testoLink = s.Substring(inizioLink + codiceLink.Length, fineLink - inizioLink - codiceLink.Length);
                if (testoLink[0] == ' ')
                    testoLink = testoLink.Substring(1);
                string testoLinkComeNota = "";
                switch (tipo)
                {
                    case RichTextBoxEx.FineLinkBrano:
                        testoLinkComeNota = Principale.testi.ConvertiRiferimento(testoLink).ComeNotaTuttoRiferimento();
                        break;
                    default:
                        testoLinkComeNota = testoLink;
                        break;
                }
                if (codiceFine == "}")
                {
                    codiceFine = ""; // non dobbiamo cancellare questo carattere, perché c'è { all'inizio del link
                    if (s.Substring(fineLink) == "}}") // altrimenti .NET non riesce a selezionare tutto il testo del link per crearne un link
                        s = s.Insert(fineLink + 1, @"\par");
                }
                s = s.Substring(0, inizioLink) + @"\v " + RichTextBoxEx.InizioLink + @"\v0 " + testoLink + @"\v " + RichTextBoxEx.FineLink1 + tipo + testoLinkComeNota + RichTextBoxEx.FineLink2 + @"\v0" + s.Substring(fineLink + codiceFine.Length);
                inizioLink = s.IndexOf(codiceLink, StringComparison.Ordinal);
            }
            return s;
        }

        private static DataSet ConnectToData(string connectionString, TipoEsword tipo)
        {
            DataSet dataSet = new DataSet("e-Sword")
            {
                Locale = CultureInfo.InvariantCulture
            };

            //Create a connection to the database
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                // Open the connection.
                connection.Open();

                try
                {
                    AggiungiTabella(dataSet, connection, "Details");
                }
                catch
                {
                    // se c'è un problema con la tabella Details,
                    // non importa, metteremo valori alternativi per i suoi campi più avanti
                }

                switch (tipo)
                {
                    case TipoEsword.Bibbia:
                        AggiungiTabella(dataSet, connection, "Bible");
                        break;
                    case TipoEsword.Commentario:
                        AggiungiTabella(dataSet, connection, "Commentary");
                        AggiungiTabella(dataSet, connection, "Chapter Notes");
                        AggiungiTabella(dataSet, connection, "Book Notes");
                        break;
                    case TipoEsword.Dizionario:
                        AggiungiTabella(dataSet, connection, "Dictionary");
                        break;
                    case TipoEsword.Tema:
                        AggiungiTabella(dataSet, connection, "Topic Notes");
                        break;
                }

                // Close the connection
                connection.Close();
            }
            return dataSet;
        }

        private static void AggiungiTabella(DataSet dataSet, OleDbConnection connection, string tabella)
        {
            //Create a DataAdapter for the table.
            OleDbDataAdapter adapter = new OleDbDataAdapter();

            // A table mapping names the DataTable.
            adapter.TableMappings.Add("Table", tabella);

            // Create a Command to retrieve data.
            string ordine = (tabella == "Bible" ? " ORDER BY ID,Chapter,Verse" : "");
            OleDbCommand commando = new OleDbCommand("SELECT * FROM [" + tabella + "]" + ordine + ";", connection)
            {
                CommandType = CommandType.Text
            };

            // Set the DataAdapter's SelectCommand.
            adapter.SelectCommand = commando;

            // Fill the DataSet.
            adapter.Fill(dataSet);
        }

    }
}