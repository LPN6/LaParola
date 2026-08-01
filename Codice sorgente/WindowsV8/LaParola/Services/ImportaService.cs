using LaParola.Utilities;
using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;
using System.Xml;

namespace LaParola.Services
{
    internal class MetaData
    {
        public string FileDaAnalizzare = string.Empty;
        public string Titolo = string.Empty;
        public string Abbreviazione = string.Empty;
        public string CasaEditrice = string.Empty;
        public string Descrizione = string.Empty;
        public string Autore = string.Empty;
        public string Lingua = string.Empty;
        public string Copyright = string.Empty;
        public string Data = string.Empty;
        public string ISBN = string.Empty;
        public string VersioneDelleNote = string.Empty;
        public string NomeVersioneUtilizzato = string.Empty;
        public bool PDFComeLibro = true;
        public TipoImportazione Tipo = TipoImportazione.Nessuno;
        public TipoThML ThMLTipo = TipoThML.Nessuno;
    }

    internal enum TipoThML
    {
        Nessuno,
        Bibbia,
        Collezione
    }

    public enum TipoImportazione
    {
        Nessuno,
        ImportaOSIS,
        ImportaZefania,
        ImportaThML,
        ImportaBibleWorks,
        ImportaRtf,
        ImportaPDF,
        Crea
    }

    internal partial class ImportaService
    {
        private const string NESSUNO_TROVATO = "nessunoTrovato";

        internal static async Task<MetaData?> ImportaDaFileAsync(string percorsoFile, TipoImportazione tipo)
        {
            bool successo = true;
            MetaData? data = await Task.Run(() =>
            {
                try
                {
                    return TrovaMetaData(percorsoFile, tipo);
                }
                catch (Exception exc)
                {
                    // Marshal UI elements (like MessageBoxes) back to the UI thread
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBoxLPN.Show(Application.Current.MainWindow,
                            string.Format(CultureInfo.InvariantCulture, (string)(Application.Current.TryFindResource("ImportaErroreXML") ?? "Error parsing XML: {0}"), exc.Message),
                            (string)(Application.Current.TryFindResource("Errore") ?? "Error")
                        );
                    });
                    successo = false;
                    return null;
                }
            });

            if (data == null)
            {
                if (successo)  // nessun messaggio di errore visualizzato già
                {
                    MessageBoxLPN.Show(Application.Current.MainWindow,
                        (string)(Application.Current.TryFindResource("ImportaErroreNullo") ?? "Failed to find valid metadata."),
                        (string)(Application.Current.TryFindResource("Errore") ?? "Error")
                    );
                }
                return null;
            }

            successo = await CreaFileAsync(data);

            return successo ? data : null;
        }

        private static MetaData? TrovaMetaData(string percorsoFile, TipoImportazione tipo)
        {
            MetaData data = new();
            if (string.IsNullOrEmpty(percorsoFile))
                return null;

            data.Tipo = tipo;
            data.FileDaAnalizzare = percorsoFile;
            switch (tipo)
            {
                case TipoImportazione.ImportaOSIS:
                    try
                    {
                        XmlDocument xd = new();
                        xd.Load(percorsoFile);
                        XmlNamespaceManager nspmgr = new(xd.NameTable);
                        nspmgr.AddNamespace("nsp", xd.DocumentElement?.NamespaceURI ?? string.Empty);

                        XmlNode? xsn = xd.DocumentElement?.SelectSingleNode("nsp:osisText/nsp:header/nsp:work/nsp:title", nspmgr);
                        if (xsn != null)
                            data.Titolo = xsn.InnerText;

                        xsn = xd.DocumentElement?.SelectSingleNode("nsp:osisText", nspmgr);
                        if (xsn?.Attributes?["osisIDWork"] is XmlAttribute osisIdAttr)
                        {
                            data.Abbreviazione = osisIdAttr.Value;
                        }

                        xsn = xd.DocumentElement?.SelectSingleNode("nsp:osisText/nsp:header/nsp:work/nsp:publisher", nspmgr);
                        if (xsn != null)
                            data.CasaEditrice = xsn.InnerText.Replace("\r\n", " ");

                        xsn = xd.DocumentElement?.SelectSingleNode("nsp:osisText/nsp:header/nsp:work/nsp:description", nspmgr);
                        if (xsn != null)
                            data.Descrizione = xsn.InnerText;

                        xsn = xd.DocumentElement?.SelectSingleNode("nsp:osisText/nsp:header/nsp:work/nsp:language", nspmgr);
                        if (xsn != null)
                            data.Lingua = xsn.InnerText.ToLowerInvariant();
                        if (data.Lingua.Length >= 4)
                            data.Lingua = data.Lingua[..3];

                        XmlNodeList? nlHead = xd.DocumentElement?.SelectNodes("nsp:osisText/nsp:header/nsp:work/nsp:rights", nspmgr);
                        if (nlHead is not null)
                        {
                            foreach (XmlNode xn in nlHead)
                            {
                                if (xn.Attributes is null || xn.Attributes.Count == 0 || xn.Attributes[0]?.Value == "copyright")
                                {
                                    data.Copyright = xn.InnerText.Replace("\r\n", " ");
                                }
                            }
                        }

                        nlHead = xd.DocumentElement?.SelectNodes("nsp:osisText/nsp:header/nsp:work/nsp:date", nspmgr);
                        bool dataOriginaleTrovata = false;
                        if (nlHead is not null)
                        {
                            foreach (XmlNode xn in nlHead)
                            {
                                if (!dataOriginaleTrovata)
                                {
                                    data.Data = xn.InnerText;
                                    if (xn.Attributes != null && xn.Attributes.Count > 0 && xn.Attributes[0].Value == "original")
                                        dataOriginaleTrovata = true;
                                }
                            }
                        }

                        data.ISBN = "";
                        nlHead = xd.DocumentElement?.SelectNodes("nsp:osisText/nsp:header/nsp:work/nsp:identifier", nspmgr);
                        if (nlHead is not null)
                        {
                            foreach (XmlNode xn in nlHead)
                                if (xn.Attributes != null && xn.Attributes.Count > 0 && xn.Attributes[0].Value == "ISBN")
                                    data.ISBN = xn.InnerText;
                        }

                        data.NomeVersioneUtilizzato = ImpostaNomeFileLaParolaDaFileOrigine(percorsoFile);
                    }
                    catch
                    {
                        throw;
                    }
                    break;
                case TipoImportazione.ImportaZefania:
                    try
                    {
                        XmlDocument xd = new();
                        xd.Load(percorsoFile);

                        XmlNode? xsn = xd.DocumentElement?.SelectSingleNode("INFORMATION/description");
                        data.Descrizione = (xsn != null ? xsn.InnerText : "");

                        string? titoloValido = xd.DocumentElement?.SelectSingleNode("INFORMATION/title")?.InnerText;
                        if (string.IsNullOrEmpty(titoloValido))
                        {
                            titoloValido = xd.DocumentElement?.Attributes?["biblename"]?.Value;
                        }
                        data.Titolo = titoloValido ?? string.Empty;

                        xsn = xd.DocumentElement?.SelectSingleNode("INFORMATION/identifier");
                        data.Abbreviazione = (xsn != null ? xsn.InnerText : "");
                        if (data.Abbreviazione.ToUpperInvariant().StartsWith("BIBLE.", StringComparison.Ordinal))
                            data.Abbreviazione = data.Abbreviazione[6..];
                        if (string.IsNullOrEmpty(data.Abbreviazione))
                            data.Abbreviazione = CreaAbbreviazione(data.Titolo);

                        xsn = xd.DocumentElement?.SelectSingleNode("INFORMATION/publisher");
                        data.CasaEditrice = (xsn != null ? xsn.InnerText : "");

                        xsn = xd.DocumentElement?.SelectSingleNode("INFORMATION/language");
                        string? linguaGrezza = xsn?.InnerText
                            ?? xd.DocumentElement?.Attributes?["lgid"]?.Value;
                        string linguaValida = (linguaGrezza ?? string.Empty).ToLowerInvariant();
                        if (linguaValida.Length >= 3)
                        {
                            linguaValida = linguaValida[..2];
                        }

                        data.Lingua = linguaValida;

                        xsn = xd.DocumentElement?.SelectSingleNode("INFORMATION/rights");
                        data.Copyright = (xsn != null ? xsn.InnerText : "");

                        xsn = xd.DocumentElement?.SelectSingleNode("INFORMATION/date");
                        data.Data = (xsn != null ? xsn.InnerText : "");

                        data.NomeVersioneUtilizzato = ImpostaNomeFileLaParolaDaFileOrigine(percorsoFile);
                    }
                    catch
                    {
                        throw;
                    }
                    break;
                case TipoImportazione.ImportaThML:
                    try
                    {
                        XmlDocument xd = new();
                        xd.Load(percorsoFile);
                        XmlNode? xsn = xd.DocumentElement?.SelectSingleNode("ThML.head/generalInfo/description");
                        data.Descrizione = (xsn != null ? xsn.InnerText : "");

                        xsn = xd.DocumentElement?.SelectSingleNode("ThML.head/generalInfo/firstPublished");
                        data.Data = (xsn != null ? xsn.InnerText : "");

                        xsn = xd.DocumentElement?.SelectSingleNode("ThML.head/printSourceInfo/published");
                        data.CasaEditrice = (xsn != null ? xsn.InnerText : "");
                        if (string.IsNullOrEmpty(data.CasaEditrice))
                        {
                            XmlNodeList? nl = xd.DocumentElement?.SelectNodes("ThML.head/electronicEdInfo/DC/DC.Source");
                            if (nl is not null)
                            {
                                foreach (XmlNode nodo in nl)
                                {
                                    if (nodo.Attributes?["sub"]?.Value == "PrintEdition")
                                    {
                                        data.CasaEditrice = nodo.InnerText;
                                    }
                                }
                            }
                        }

                        xsn = xd.DocumentElement?.SelectSingleNode("ThML.head/electronicEdInfo/bookID");
                        data.Abbreviazione = (xsn != null ? xsn.InnerText : "");
                        if (!string.IsNullOrEmpty(data.Abbreviazione))
                            data.Abbreviazione = data.Abbreviazione[..1].ToUpperInvariant() + data.Abbreviazione[1..];

                        xsn = xd.DocumentElement?.SelectSingleNode("ThML.head/electronicEdInfo/DC/DC.Title");
                        data.Titolo = (xsn != null ? xsn.InnerText : "");

                        xsn = xd.DocumentElement?.SelectSingleNode("ThML.head/electronicEdInfo/DC/DC.Creator");
                        data.Autore = (xsn != null ? xsn.InnerText : "");

                        xsn = xd.DocumentElement?.SelectSingleNode("ThML.head/electronicEdInfo/DC/DC.Description");
                        string descrizione2 = (xsn != null ? xsn.InnerText : "");
                        if (!string.IsNullOrEmpty(data.Descrizione) && !string.IsNullOrEmpty(descrizione2))
                            data.Descrizione += " ";
                        data.Descrizione += descrizione2;
                        data.Descrizione = data.Descrizione.Replace("\n", " ").Replace("\t", " ").Trim();
                        while (data.Descrizione.IndexOf("  ", StringComparison.Ordinal) > 0)
                            data.Descrizione = data.Descrizione.Replace("  ", " ");

                        xsn = xd.DocumentElement?.SelectSingleNode("ThML.head/electronicEdInfo/DC/DC.Language");
                        data.Lingua = (xsn != null ? xsn.InnerText.ToLowerInvariant() : "");
                        if (data.Lingua.Length > 2)
                            data.Lingua = data.Lingua[..2];

                        xsn = xd.DocumentElement?.SelectSingleNode("ThML.head/electronicEdInfo/DC/DC.Rights");
                        data.Copyright = (xsn != null ? xsn.InnerText : "");

                        XmlNodeList? nl2 = xd.DocumentElement?.SelectNodes("ThML.head/electronicEdInfo/DC/DC.Subject");
                        if (nl2 is not null)
                        {
                            foreach (XmlNode nodo in nl2)
                            {
                                if (nodo.Attributes?["scheme"]?.Value == "LCCN")
                                    data.ISBN = nodo.InnerText;
                            }
                        }

                        xsn = xd.DocumentElement?.SelectSingleNode("ThML.head/electronicEdInfo/DC/DC.Type");
                        if (xsn != null && xsn.InnerText == "Text.Commentary")
                            data.ThMLTipo = TipoThML.Collezione;
                        else
                            data.ThMLTipo = TipoThML.Bibbia;

                        data.NomeVersioneUtilizzato = ImpostaNomeFileLaParolaDaFileOrigine(percorsoFile);
                    }
                    catch
                    {
                        throw;
                    }
                    break;
                case TipoImportazione.ImportaBibleWorks:
                    data.NomeVersioneUtilizzato = ImpostaNomeFileLaParolaDaFileOrigine(percorsoFile);
                    data.Abbreviazione = CreaAbbreviazione(data.NomeVersioneUtilizzato);
                    data.Titolo = data.NomeVersioneUtilizzato;
                    break;
                default:
                    return null;
            }

            return data;
        }

        internal static async Task<bool> CreaFileAsync(MetaData data)
        {
            ConfrontoCI confrontoParole = new();
            SortedDictionary<string, List<OccorrenzaParola>> chiave = new(confrontoParole);
            string[] fileNote = [];

            try
            {
                using FileStream fs = new(data.NomeVersioneUtilizzato + ".laparola", FileMode.Create, FileAccess.Write);
                using BinaryWriter bw = new(fs);

                Version? ver = Assembly.GetExecutingAssembly().GetName().Version;

                char[] inizioFile = [
                    'L', 'P', 'N', (char)(ver?.Major ?? 8), (char)(ver?.Minor ?? 0),
                    (char)0, // sempre 0, così nel futuro si può aggiungere altri numeri senza essere incompatibili
                    (char)1];
                bw.Write(inizioFile);
                bw.Write((UInt32)11);
                bw.Write((UInt32)0);

                bw.Write(Path.GetFileNameWithoutExtension(data.NomeVersioneUtilizzato + ".laparola")); // nomeTesto
                bw.Write(data.Abbreviazione);
                bw.Write(data.Titolo);
                bw.Write(data.Autore);
                bw.Write(data.CasaEditrice);
                bw.Write(data.Data);
                bw.Write(data.Copyright);
                bw.Write(data.ISBN);
                bw.Write(data.Descrizione);
                bw.Write(data.Lingua);
                bw.Write(data.VersioneDelleNote);
                bw.Write(data.Tipo == TipoImportazione.Crea ? (byte)BloccatoTipi.Sbloccato : (byte)BloccatoTipi.Bloccato);
                byte tipoTestoDaScrivere = (data.Tipo == TipoImportazione.ImportaRtf || data.Tipo == TipoImportazione.Crea || data.Tipo == TipoImportazione.ImportaPDF || (data.Tipo == TipoImportazione.ImportaThML && data.ThMLTipo != TipoThML.Bibbia) ? (byte)1 : (byte)0);
                bw.Write(tipoTestoDaScrivere);
                UInt32 pInizioDati = (UInt32)(bw.Seek(0, SeekOrigin.Current));
                bw.Seek(11, SeekOrigin.Begin);
                bw.Write(pInizioDati);
                bw.Seek(0, SeekOrigin.End);

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

                string[]? noteTesto = null;
                string[]? noteInOrdine = null;
                UInt32[] indici = new UInt32[2];
                UInt32 numeroVersetto = 0;
                string libro, libroPrecedente = NESSUNO_TROVATO;
                List<byte> capitoliInLibri = [];
                List<byte> versettiInCapitoli = [];
                List<int> indice = [];
                byte capitolo = 0, versetto = 0, capitoloPrecedente = 0;
                int numeroLibro = 0, numeroLibroPrecedente = 0, versettoPrecedente = 0;

                switch (data.Tipo)
                {
                    #region Importa Bibbia (OSIS)
                    case TipoImportazione.ImportaOSIS:
                        XmlDocument xmlDocumento = new();
                        xmlDocumento.Load(data.FileDaAnalizzare);
                        XmlNamespaceManager nspmgr = new(xmlDocumento.NameTable);
                        nspmgr.AddNamespace("nsp", xmlDocumento.ChildNodes[1]?.NamespaceURI ?? "");

                        XmlNodeList? nl = xmlDocumento.DocumentElement?.SelectNodes("nsp:osisText/nsp:div/nsp:div/nsp:chapter/nsp:verse", nspmgr);
                        if (nl == null || nl.Count == 0) // alcuni documenti OSIS non usano una sezione div per i Testamenti
                            nl = xmlDocumento.DocumentElement?.SelectNodes("nsp:osisText/nsp:div/nsp:chapter/nsp:verse", nspmgr);
                        if (nl == null || nl.Count == 0) // alcuni documenti OSIS usano una div per le sezioni del testo
                            nl = xmlDocumento.DocumentElement?.SelectNodes("nsp:osisText/nsp:div/nsp:chapter/nsp:div/nsp:p/nsp:verse", nspmgr);
                        if (nl == null || nl.Count == 0) // alcuni documenti OSIS usano una sezione div per un capitolo, invece di una sezione chapter
                            nl = xmlDocumento.DocumentElement?.SelectNodes("nsp:osisText/nsp:div/nsp:div/nsp:div/nsp:verse", nspmgr);
                        string[] testoAnalizzato; // primo elemento è RTF, secondo è testo normale
                        int punto1, punto2;

                        Dictionary<string, int> libriOSIS = [];
                        string[] libriOSISArray = ["", "Gen", "Exod", "Lev", "Num", "Deut",
                     "Josh", "Judg", "Ruth", "1Sam", "2Sam", "1Kgs", "2Kgs", "1Chr", "2Chr",
                     "Ezra", "Neh", "Tob", "Jdt", "Esth", "1Macc", "2Macc",
                     "Job", "Ps", "Prov", "Eccl", "Song", "Wis", "Sir",
                     "Isa", "Jer", "Lam", "Bar", "Ezek", "Dan",
                     "Hos", "Joel", "Amos", "Obad", "Jonah", "Mic", "Nah", "Hab", "Zeph", "Hag", "Zech", "Mal",
                     "Matt", "Mark", "Luke", "John", "Acts",
                     "Rom", "1Cor", "2Cor", "Gal", "Eph", "Phil", "Col", "1Thess", "2Thess", "1Tim", "2Tim", "Titus", "Phlm",
                     "Heb", "Jas", "1Pet", "2Pet", "1John", "2John", "3John", "Jude", "Rev"];
                        for (int i = 1; i <= 73; ++i)
                            libriOSIS.Add(libriOSISArray[i], i);

                        if (nl != null)
                        {
                            foreach (XmlNode xn in nl)
                            {
                                if (xn.Attributes?["eID"] == null && xn.Attributes?["osisID"]?.Value is string rif) // questo file ha <verse sID=...> e <verse eID=...> per ogni versetto
                                {
                                    punto1 = rif.IndexOf('.');
                                    punto2 = rif.LastIndexOf('.');
                                    libro = rif[..punto1];
                                    if (libro != libroPrecedente)
                                    {
                                        numeroLibro = libriOSIS[libro]; // può dare exception se libro non è un libro riconosciuto; c'è il catch e il messaggio in questo caso
                                        if (libroPrecedente != NESSUNO_TROVATO)
                                            capitoliInLibri.Add(capitolo); // il numero di capitoli nel libro precedente
                                        for (int i = 0; i < numeroLibro - numeroLibroPrecedente - 1; i++)
                                            capitoliInLibri.Add(0);

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
                                    versetto = Convert.ToByte(rif[(punto2 + 1)..], CultureInfo.InvariantCulture);
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
                                        testoAnalizzato[0] = testoAnalizzato[0][4..].Trim();
                                    indice.Add((int)(bw.Seek(0, SeekOrigin.Current) - inizioTesto)); // -8 perché inizioBibbiaTesto è 8 dopo inizioBibbia
                                    bw.Write(testoAnalizzato[0]);
                                    ++numeroVersetto;

                                    chiave = Texts.TrovaParoleInVoce(testoAnalizzato[1], numeroVersetto, chiave, data.Lingua);
                                }
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
                        XmlDocument xmlDocumentoZefania = new()
                        {
                            PreserveWhitespace = true
                        };
                        xmlDocumentoZefania.Load(data.FileDaAnalizzare);
                        XmlNodeList? nlZef = xmlDocumentoZefania.DocumentElement?.SelectNodes("BIBLEBOOK/CHAPTER/VERS");
                        if (nlZef != null)
                        {
                            foreach (XmlNode xn in nlZef)
                            {
                                // nota: quando c'è l'apocrifa, bisogna prima modificare il file XML affinché i libri siano nell'ordine di questo programma, non nell'ordine del bnumber di Zefania
                                if (xn.ParentNode?.ParentNode?.Attributes?["bnumber"]?.Value is string bnumber)
                                    numeroLibro = ConvertiLibro66A73Zefania(bnumber);

                                if (numeroLibro != numeroLibroPrecedente)
                                {
                                    if (numeroLibroPrecedente > 0)
                                        capitoliInLibri.Add(capitolo); // il numero di capitoli nel libro precedente
                                    for (int i = 0; i < numeroLibro - numeroLibroPrecedente - 1; i++)
                                        capitoliInLibri.Add(0);

                                    numeroLibroPrecedente = numeroLibro;
                                    if (capitoloPrecedente == 1)
                                    {
                                        // per i libri con uno solo capitolo - quando si va al libro successivo non si cambia il numero del capitolo
                                        versettiInCapitoli.Add(versetto);
                                        versettoPrecedente = 0;
                                    }
                                }
                                capitolo = Convert.ToByte(xn.ParentNode?.Attributes?["cnumber"]?.Value ?? "0", CultureInfo.InvariantCulture);
                                if (capitolo != capitoloPrecedente)
                                {
                                    if (capitoloPrecedente != 0)
                                        versettiInCapitoli.Add(versetto);
                                    capitoloPrecedente = capitolo;
                                    versettoPrecedente = 0;
                                }
                                versetto = Convert.ToByte(xn.Attributes?["vnumber"]?.Value ?? "0", CultureInfo.InvariantCulture);
                                for (int i = versettoPrecedente + 1; i < versetto; ++i)
                                { // versetti mancanti
                                    ++numeroVersetto;
                                    indice.Add((int)(bw.Seek(0, SeekOrigin.Current) - inizioTesto)); // -8 perché inizioBibbiaTesto è 8 dopo inizioBibbia
                                    bw.Write("");
                                }
                                versettoPrecedente = versetto;
                                testoAnalizzato = ConvertiZefaniaARtfETesto(xn);
                                if (versetto == 1 && testoAnalizzato[0].StartsWith(@"\par", StringComparison.OrdinalIgnoreCase))
                                    testoAnalizzato[0] = testoAnalizzato[0][4..].Trim();
                                indice.Add((int)(bw.Seek(0, SeekOrigin.Current) - inizioTesto)); // -8 perché inizioBibbiaTesto è 8 dopo inizioBibbia
                                bw.Write(testoAnalizzato[0]);
                                ++numeroVersetto;

                                chiave = Texts.TrovaParoleInVoce(testoAnalizzato[1], numeroVersetto, chiave, data.Lingua);
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
                    #region Importa BibleWorks
                    case TipoImportazione.ImportaBibleWorks:
                        string[] libriBibleworksArray = ["", "Gen", "Exo", "Lev", "Num", "Deu",
                     "Jos", "Jdg", "Rut", "1Sa", "2Sa", "1Ki", "2Ki", "1Ch", "2Ch",
                     "Ezr", "Neh", "Tob", "Jdt", "Est", "1Ma", "2Ma",
                     "Job", "Psa", "Pro", "Ecc", "Sol", "Wis", "Sir",
                     "Isa", "Jer", "Lam", "Bar", "Eze", "Dan",
                     "Hos", "Joe", "Amo", "Oba", "Jon", "Mic", "Nah", "Hab", "Zep", "Hag", "Zec", "Mal",
                     "Mat", "Mar", "Luk", "Joh", "Act",
                     "Rom", "1Co", "2Co", "Gal", "Eph", "Phi", "Col", "1Th", "2Th", "1Ti", "2Ti", "Tit", "Phm",
                     "Heb", "Jam", "1Pe", "2Pe", "1Jo", "2Jo", "3Jo", "Jud", "Rev"];
                        Dictionary<string, int> libriBibleworks = [];
                        for (int i = 1; i <= 73; ++i)
                            libriBibleworks.Add(libriBibleworksArray[i], i);

                        string testoBibleworks;
                        int spazio;
                        string[] righe = File.ReadAllLines(data.FileDaAnalizzare);

                        // prima di tutto bisognare riordinare le righe, nel caso che non siano nel ordine giusto
                        // (che è probabilmente vero, perché BibleWorks mette 1Mac e 2Mac dopo Malachia invece di dopo Ester).
                        List<string> listaRighe = new(righe.Length);
                        string numeroLibro2 = "00";
                        foreach (string riga in righe)
                        {
                            if (riga.Trim().Length > 0)
                            {
                                spazio = riga.IndexOf(' ');
                                punto1 = riga.IndexOf(':');
                                punto2 = riga.IndexOf(' ', spazio + 1);
                                libro = riga[..spazio];
                                if (libro != libroPrecedente)
                                {
                                    numeroLibro = 0;
                                    if (!libriBibleworks.TryGetValue(libro, out numeroLibro))
                                        numeroLibro = MainWindow.Testi.GetLibroNumeroDaAbbreviazione(libro);
                                    if (numeroLibro == 0)
                                        throw new KeyNotFoundException();
                                    libroPrecedente = libro;
                                    numeroLibro2 = Funzioni.AggiungiZero(numeroLibro, 2);
                                }
                                listaRighe.Add(string.Concat(numeroLibro2, Funzioni.AggiungiZero(riga.Substring(spazio + 1, punto1 - spazio - 1), 3), Funzioni.AggiungiZero(riga.Substring(punto1 + 1, punto2 - punto1 - 1), 3), riga.AsSpan(punto2 + 1)));
                            }
                        }
                        listaRighe.Sort();
                        righe = [.. listaRighe];
                        libroPrecedente = NESSUNO_TROVATO;

                        foreach (string riga in righe)
                        {
                            libro = riga[..2];
                            if (libro != libroPrecedente)
                            {
                                numeroLibro = Convert.ToInt32(libro, CultureInfo.InvariantCulture);
                                if (libroPrecedente != NESSUNO_TROVATO)
                                    capitoliInLibri.Add(capitolo); // il numero di capitoli nel libro precedente
                                for (int i = 0; i < numeroLibro - numeroLibroPrecedente - 1; i++)
                                    capitoliInLibri.Add(0);

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
                            testoBibleworks = riga[8..].Trim().Replace(@"\", @"\\").Replace("{", @"\{").Replace("}", @"\}");
                            if (testoBibleworks == ".") // versetto mancante
                                testoBibleworks = "";
                            indice.Add((int)(bw.Seek(0, SeekOrigin.Current) - inizioTesto)); // -8 perché inizioBibbiaTesto è 8 dopo inizioBibbia
                            bw.Write(testoBibleworks);
                            ++numeroVersetto;

                            chiave = Texts.TrovaParoleInVoce(testoBibleworks, numeroVersetto, chiave, data.Lingua);

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
                    case TipoImportazione.ImportaRtf:
                        fileNote = Directory.GetFiles(data.FileDaAnalizzare);
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
                            if (noteTitoli[i].StartsWith('#'))
                            {
                                if (noteTitoli[i].Length == 1) noteTitoli[i] += "0"; // non è un formato riconosciuto
                                if (noteTitoli[i].Length == 2) noteTitoli[i] += "0"; // non è un formato riconosciuto
                                if (noteTitoli[i].Length <= 3) noteTitoli[i] += "000"; // nessun capitolo
                                if (noteTitoli[i].Length <= 6) noteTitoli[i] += "000"; // nessun versetto
                                if (noteTitoli[i].Length <= 9) noteTitoli[i] += "0000"; // nessuna parola
                                if (noteTitoli[i].Length <= 13) noteTitoli[i] += "-" + noteTitoli[i][1..]; // singolo versetto invece di brano
                                if (noteTitoli[i].Length == 18 && noteTitoli[i][9] == '-') noteTitoli[i] = noteTitoli[i].Insert(9, "0000") + "0000"; // formato #01001001-01001002 cioè senza il numero della parola
                            }
                            // eventuali segni di più alla fine di una nota sono tolti
                            // il segno è usato per distinguere due note che sono diverse, ma i titoli differiscono solo nelle lettere minuscole/maiuscole
                            // necessario perché Windows non può distinguere due file che hanno nomi che differiscono solo così (è case insensitive)
                            else
                            {
                                while (noteTitoli[i].EndsWith('+'))
                                    noteTitoli[i] = noteTitoli[i][..^1];
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
                                chiave = await RunInSTAThread(() =>
                                {
                                    // This code runs on a custom background STA thread!
                                    RichTextBoxEx rtb = new()
                                    {
                                        Rtf = noteTesto[i]
                                    };
                                    return Texts.TrovaParoleInVoce(rtb.Text, i, chiave, data.Lingua);
                                });
                            }
                            catch
                            { // il file non è RTF, lo consideriamo testo normale
                                chiave = Texts.TrovaParoleInVoce(noteTesto[i], i, chiave, data.Lingua);
                            }
                        }
                        indici = Texts.ScriviNote(bw, pInizioDati, noteTitoli, noteTesto);
                        inizioTestoIndiceLC = indici[0];
                        inizioTestoIndice = indici[1];
                        break;
                    #endregion
                    #region Nuove note
                    case TipoImportazione.Crea:
                        string[] titoliNuoveNote = [];
                        noteTesto = [];
                        for (UInt32 i = 0; i < noteTesto.Length; ++i)
                            chiave = Texts.TrovaParoleInVoce(noteTesto[i], i, chiave, data.Lingua);
                        indici = Texts.ScriviNote(bw, pInizioDati, titoliNuoveNote, noteTesto);
                        inizioTestoIndiceLC = indici[0];
                        inizioTestoIndice = indici[1];
                        break;
                    #endregion
                    #region ThML
                    case TipoImportazione.ImportaThML:
                        XmlDocument xmlDocumentoThML = new()
                        {
                            PreserveWhitespace = true
                        };
                        xmlDocumentoThML.Load(data.FileDaAnalizzare);
                        XmlNodeList? nlThML = null;
                        #region ThML Bibbia
                        if (data.ThMLTipo == TipoThML.Bibbia)
                        {
                            nlThML = xmlDocumentoThML.DocumentElement?.SelectNodes("ThML.body/div1/div2");
                            int numeroNodiDiLibri = nlThML?.Count ?? 0;
                            int[] numeroLibri = new int[numeroNodiDiLibri];
                            string id = "";
                            if (nlThML != null)
                            {
                                for (int i = 0; i < numeroNodiDiLibri; ++i)
                                {
                                    id = nlThML[i]?.Attributes?["id"]?.Value ?? "";
                                    if (id == "PrAzar")
                                        numeroLibri[i] = -34;
                                    else if (id == "AddEsth")
                                        numeroLibri[i] = -19;
                                    else
                                        numeroLibri[i] = MainWindow.Testi.GetLibroNumeroDaAbbreviazione(id);
                                }
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
                                if (libroMinimo < 999 && nlThML != null)
                                {
                                    nodoSecondarioTrovato = false;
                                    for (int i = 0; i < numeroNodiDiLibri; ++i)
                                    {
                                        if (numeroLibri[i] == -numeroLibroMinimo)
                                        { // quando ci sono due nodi per lo stesso libro, per esempio PrAzar o AddEst
                                            libroPrecedente = ImportaLibroThML(data, bw, ref chiave, inizioTesto, ref numeroVersetto, libroPrecedente, ref numeroLibroPrecedente, ref capitoliInLibri, ref versettiInCapitoli, ref indice, nlThML[libroMinimo], nlThML[i]);
                                            nodoSecondarioTrovato = true;
                                            break;
                                        }
                                    }
                                    if (!nodoSecondarioTrovato)
                                        libroPrecedente = ImportaLibroThML(data, bw, ref chiave, inizioTesto, ref numeroVersetto, libroPrecedente, ref numeroLibroPrecedente, ref capitoliInLibri, ref versettiInCapitoli, ref indice, nlThML[libroMinimo]);
                                    numeroLibri[libroMinimo] = 0;
                                }
                            } while (libroMinimo < 999);

                            int numeroUltimoLibroFatto = MainWindow.Testi.GetLibroNumeroDaAbbreviazione(libroPrecedente);
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
                            nlThML = xmlDocumentoThML.DocumentElement?.SelectNodes("ThML.body/div1");

                            List<string> noteTitoliThML = [];
                            List<string> noteTestoThML = [];

                            if (nlThML != null)
                            {
                                foreach (XmlNode nodo1 in nlThML)
                                    AggiungiNoteDaThMLDiv(nodo1, 1, noteTitoliThML, noteTestoThML, data.ThMLTipo);
                            }

                            for (int i = noteTitoliThML.Count - 1; i >= 0; --i)
                            {
                                if (string.IsNullOrEmpty(noteTestoThML[i]))
                                {
                                    noteTestoThML.RemoveAt(i);
                                    noteTitoliThML.RemoveAt(i);
                                }
                            }

                            fileNote = [.. noteTitoliThML];
                            noteTesto = [.. noteTestoThML];
                            for (int i = 0; i < fileNote.Length; ++i)
                                while (fileNote[i].StartsWith('\t'))
                                    fileNote[i] = fileNote[i][1..];
                            Array.Sort(fileNote, noteTesto, new ConfrontoCI());
                            for (int i = noteTitoliThML.Count - 1; i >= 0; --i)
                                if (noteTitoliThML[i].StartsWith('#'))
                                    noteTitoliThML.RemoveAt(i);
                            noteTitoliThML.Insert(0, ""); // l'indice è vuoto
                            noteInOrdine = [.. noteTitoliThML];

                            for (UInt32 i = 0; i < fileNote.Length; ++i)
                            {
                                chiave = await RunInSTAThread(() =>
                                {
                                    // This code runs on a custom background STA thread!
                                    RichTextBoxEx rtb = new()
                                    {
                                        Rtf = noteTesto[i]
                                    };
                                    return Texts.TrovaParoleInVoce(rtb.Text, i, chiave, data.Lingua);
                                });
                            }

                            indici = Texts.ScriviNote(bw, pInizioDati, fileNote, noteTesto);
                            inizioTestoIndiceLC = indici[0];
                            inizioTestoIndice = indici[1];
                        }
                        break;
                        #endregion
                    #endregion
                    #region PDF
                    case TipoImportazione.ImportaPDF:
                        string[]? noteTestoPDF = null;
                        string[]? noteTitoliPDF = null;
                        bool isPdf = Path.GetExtension(data.FileDaAnalizzare).Equals(".pdf", StringComparison.OrdinalIgnoreCase);
                        bool isRtf = Path.GetExtension(data.FileDaAnalizzare).Equals(".rtf", StringComparison.OrdinalIgnoreCase);
                        bool generaLibro = data.PDFComeLibro;
                        bool generaNote = !generaLibro;

                        if (isPdf)
                        {
                            string cartellaTemp = GetAppTempFolder();
                            if (string.IsNullOrEmpty(cartellaTemp))
                            {
                                return false;
                            }
                            else
                            {
                                cartellaTemp += data.Titolo;
                            }
                            //int paginaIniziale = 1;
                            //int.TryParse(TxtPaginaIniziale.Text, out paginaIniziale);
                            int? paginaFinale = null;
                            //if (int.TryParse(TxtPaginaFinale.Text, out int pf)) paginaFinale = pf;
                            CancellationTokenSource tokenSorgente = new();
                            await Task.Run(() =>
                                EstrazionePdf.Estrai(new ParametriEstrazionePdf
                                {
                                    PercorsoPdf = data.FileDaAnalizzare,
                                    Lingua = data.Lingua,
                                    CartellaNote = cartellaTemp,
                                    CartellaLibro = cartellaTemp,
                                    SaltaNote = !generaNote,
                                    SaltaLibro = !generaLibro,
                                    PaginaFine = paginaFinale,
                                }, _ => { }, tokenSorgente.Token,
                                    (fatte, totali) => Application.Current.Dispatcher.Invoke(() =>
                                    paginaFinale = (int?)(totali > 0 ? fatte * 100.0 / totali * 0.8 : 0))));
                            // TODO2 BarraProgresso.Value = totali > 0 ? fatte * 100.0 / totali * 0.8 : 0)));
                            // però la barra di stato è in LibraryToolView
                            List<string> fileDaLeggere;
                            if (generaNote)
                            {
                                fileDaLeggere = [.. Directory.GetFiles(cartellaTemp, "#*.rtf")];
                            }
                            else // (generaLibro)
                            { // match Pagina... or Page...
                                fileDaLeggere = [.. Directory.GetFiles(cartellaTemp, "Pag*.rtf").OrderBy(f => f)];
                            }
                            noteTestoPDF = new string[fileDaLeggere.Count];
                            noteTitoliPDF = new string[fileDaLeggere.Count];
                            for (UInt32 i = 0; i < fileDaLeggere.Count; ++i)
                            {
                                string fileNome = fileDaLeggere[(int)i];
                                noteTitoliPDF[i] = Path.GetFileNameWithoutExtension(fileNome);
                                try
                                {
                                    noteTestoPDF[i] = File.ReadAllText(fileNome, Encoding.GetEncoding(1252));
                                }
                                catch (ArgumentException) // se code page europeo occidentale non è installato, proviamo quello predefinito
                                {
                                    noteTestoPDF[i] = File.ReadAllText(fileNome, Encoding.GetEncoding(0));
                                }
                                catch (NotSupportedException) // se code page europeo occidentale non è installato, proviamo quello predefinito
                                {
                                    noteTestoPDF[i] = File.ReadAllText(fileNome, Encoding.GetEncoding(0));
                                }
                                try
                                {
                                    chiave = await RunInSTAThread(() =>
                                    {
                                        // This code runs on a custom background STA thread!
                                        RichTextBoxEx rtb = new()
                                        {
                                            Rtf = noteTestoPDF[i]
                                        };
                                        return Texts.TrovaParoleInVoce(rtb.Text, i, chiave, data.Lingua);
                                    });
                                }
                                catch
                                { // il file non è RTF, lo consideriamo testo normale
                                    chiave = Texts.TrovaParoleInVoce(noteTestoPDF[i], i, chiave, data.Lingua);
                                }
                            }
                            try { Directory.Delete(cartellaTemp, true); } catch { }
                        }
                        else if (isRtf)
                        {
                            string rtf;
                            try
                            {
                                rtf = File.ReadAllText(data.FileDaAnalizzare, Encoding.GetEncoding(1252));
                            }
                            catch (ArgumentException) // se code page europeo occidentale non è installato, proviamo quello predefinito
                            {
                                rtf = File.ReadAllText(data.FileDaAnalizzare, Encoding.GetEncoding(0));
                            }
                            catch (NotSupportedException) // se code page europeo occidentale non è installato, proviamo quello predefinito
                            {
                                rtf = File.ReadAllText(data.FileDaAnalizzare, Encoding.GetEncoding(0));
                            }

                            if (generaNote)
                            {
                                // Estrae testo, cerca intestazioni "Libro cap:verso", crea note
                                List<string> listaTesti = [];
                                List<string> listaTitoli = [];

                                string plain = RtfToPlainText(rtf);
                                string titoloNota = "";
                                List<string> buffer = [];
                                foreach (string riga in plain.Split('\n'))
                                {
                                    string rp = riga.Trim();
                                    if (string.IsNullOrEmpty(rp))
                                    {
                                        buffer.Add("");
                                        continue;
                                    }
                                    var rif = EstrazionePdf.RilevaIntestazione(rp);
                                    if (rif.HasValue)
                                    {
                                        if (!string.IsNullOrEmpty(titoloNota) && buffer.Count > 0)
                                        {
                                            string corpo = string.Join("\\par\n", buffer.Where(l => l.Trim().Length > 0).Select(l => EstrazionePdf.CollegaCitazioniEEscape(l)));
                                            // TODO2 da cancellare listaTitoli.Add(EstrazionePdf.RiferimentoAStringa(rif.Value.numero, rif.Value.capitolo, rif.Value.v1, rif.Value.v2));
                                            listaTitoli.Add(new Riferimento([(byte)rif.Value.numero, (byte)rif.Value.capitolo, (byte)rif.Value.v1, (byte)rif.Value.numero, (byte)rif.Value.capitolo, (byte)rif.Value.v2]).ComeNotaTuttoRiferimento());
                                            listaTesti.Add("{\\rtf1\\ansi\\ansicpg1252\\deff0\n" + corpo + "\n}");
                                        }
                                        buffer.Clear();
                                        titoloNota = new Riferimento([(byte)rif.Value.numero, (byte)rif.Value.capitolo, (byte)rif.Value.v1, (byte)rif.Value.numero, (byte)rif.Value.capitolo, (byte)rif.Value.v2]).ComeNotaTuttoRiferimento();
                                        // TODO2 cancellare titoloNota = EstrazionePdf.RiferimentoAStringa(rif.Value.numero, rif.Value.capitolo, rif.Value.v1, rif.Value.v2);
                                        continue;
                                    }
                                    buffer.Add(rp);
                                }
                                if (!string.IsNullOrEmpty(titoloNota) && buffer.Count > 0)
                                {
                                    string corpo = string.Join("\\par\n", buffer.Where(l => l.Trim().Length > 0).Select(l => EstrazionePdf.CollegaCitazioniEEscape(l)));
                                    listaTitoli.Add(titoloNota);
                                    listaTesti.Add("{\\rtf1\\ansi\\ansicpg1252\\deff0\n" + corpo + "\n}");
                                }
                                noteTestoPDF = [.. listaTesti];
                                noteTitoliPDF = [.. listaTitoli];
                            }
                            else // (generaLibro)
                            {
                                noteTestoPDF = new string[1];
                                noteTestoPDF[0] = rtf;
                                noteTitoliPDF = new string[1];
                                noteTitoliPDF[0] = data.Titolo;
                            }

                            for (UInt32 i = 0; i < noteTestoPDF.Length; ++i)
                            {
                                try
                                {
                                    chiave = await RunInSTAThread(() =>
                                    {
                                        // This code runs on a custom background STA thread!
                                        RichTextBoxEx rtb = new()
                                        {
                                            Rtf = noteTestoPDF[i]
                                        };
                                        return Texts.TrovaParoleInVoce(rtb.Text, i, chiave, data.Lingua);
                                    });
                                }
                                catch
                                { // il file non è RTF, lo consideriamo testo normale
                                    chiave = Texts.TrovaParoleInVoce(noteTestoPDF[i], i, chiave, data.Lingua);
                                }
                            }
                        } // else if rtf
                        else
                        {
                            return false;
                        }
                        indici = Texts.ScriviNote(bw, pInizioDati, noteTitoliPDF, noteTestoPDF);
                        inizioTestoIndiceLC = indici[0];
                        inizioTestoIndice = indici[1];
                        break;
                    #endregion
                }

                UInt32 inizioParole = (UInt32)(bw.Seek(0, SeekOrigin.Current)) - pInizioDati;
                StringBuilder parole = new("");
                foreach (string s in chiave.Keys)
                    parole.Append(s).Append('|');
                bw.Write(parole.ToString());

                UInt32 inizioParoleIndiceIndice = (UInt32)(bw.Seek(0, SeekOrigin.Current)) - pInizioDati;
                ScriviNumeroApparenzeParole(bw, chiave);

                UInt32 inizioParoleIndice = (UInt32)(bw.Seek(0, SeekOrigin.Current)) - pInizioDati;
                foreach (List<OccorrenzaParola> lista in chiave.Values)
                    ScriviChiaveAFile(bw, lista);

                UInt32 inizioRadici = (UInt32)(bw.Seek(0, SeekOrigin.Current)) - pInizioDati;
                List<string> listaRadici = new(8192);
                string nomeFileBase = Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(data.FileDaAnalizzare);
                if (data.FileDaAnalizzare.IndexOf('.') > -1)
                    nomeFileBase = Path.GetDirectoryName(data.FileDaAnalizzare) + nomeFileBase;
                else // infoVersione[0] è il nome di una directory cioè è "importa note"
                    nomeFileBase = data.FileDaAnalizzare + nomeFileBase;
                try
                {
                    string[] radiceDiParola = Funzioni.AggiungiRadiciDaFile(Path.GetDirectoryName(nomeFileBase), data.Lingua, parole.ToString().Split(['|'], StringSplitOptions.RemoveEmptyEntries), listaRadici);

                    if (listaRadici.Count > 1)
                    {
                        // scrivere l'elenco delle radici
                        StringBuilder radici = new("");
                        foreach (string s in listaRadici)
                            radici.Append(s).Append('|');
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
                        if (data.Tipo == TipoImportazione.ImportaOSIS || data.Tipo == TipoImportazione.ImportaZefania || data.Tipo == TipoImportazione.ImportaBibleWorks || (data.Tipo == TipoImportazione.ImportaThML && data.ThMLTipo == TipoThML.Bibbia))
                        {
                            foreach (string radiceDiversa in radiciDiverse)
                            {
                                riga = radiceDiversa.Split(['|'], StringSplitOptions.RemoveEmptyEntries);
                                bw.Write(Convert.ToByte(riga[0], CultureInfo.InvariantCulture));
                                bw.Write(Convert.ToByte(riga[1], CultureInfo.InvariantCulture));
                                bw.Write(Convert.ToByte(riga[2], CultureInfo.InvariantCulture));
                                bw.Write(Convert.ToUInt16(riga[3], CultureInfo.InvariantCulture));
                                bw.Write(riga[4]);
                            }
                        }
                        else if (data.Tipo == TipoImportazione.ImportaRtf || (data.Tipo == TipoImportazione.ImportaThML && data.ThMLTipo != TipoThML.Bibbia))
                        { // TipoImportazione.NuovaNota non può avere radiciDiverse (perché non ci sono ancora parole)
                            foreach (string radiceDiversa in radiciDiverse)
                            {
                                riga = radiceDiversa.Split(['|'], StringSplitOptions.RemoveEmptyEntries);
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
                        if (fileAperto)
                        { // altrimenti il file non esiste (o impossibile aprirlo)
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                MessageBoxLPN.Show(Application.Current.MainWindow, String.Format(CultureInfo.InvariantCulture, (string)(Application.Current.TryFindResource("ImportaErroreRadiciDiverse") ?? "Error in the line {1} in the file radici_diverse: {0}"), exc.Message, riga[0] + "|" + riga[1] + "|" + riga[2] + "|" + riga[3] + "|" + riga[4] + "|"), (string)(Application.Current.TryFindResource("Errore") ?? "Error"));
                            });
                        }
                    }
                }

                UInt32 inizioRiferimentiDiversi = (UInt32)(bw.Seek(0, SeekOrigin.Current)) - pInizioDati;
                try
                {
                    ScriviRiferimentiDiversi(bw, nomeFileBase);
                }
                catch // file non esiste, o qualche problema nella lettura
                {
                    inizioRiferimentiDiversi = 0;
                }

                UInt32 inizioRiferimentiCitati = (UInt32)(bw.Seek(0, SeekOrigin.Current)) - pInizioDati;
                {
                    if (noteTesto != null)
                    {
                        if (!MainWindow.Testi.ScriviRiferimentiCitati(bw, noteTesto))
                            inizioRiferimentiCitati = 0;
                    }
                    else
                        inizioRiferimentiCitati = 0;
                }

                UInt32 inizioOrdine = (UInt32)(bw.Seek(0, SeekOrigin.Current)) - pInizioDati;
                try
                {
                    ScriviOrdine(bw, noteInOrdine, nomeFileBase);
                }
                catch // file non esiste, o qualche problema nella lettura
                {
                    inizioOrdine = 0;
                }

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
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageBoxLPN.Show(Application.Current.MainWindow, String.Format(CultureInfo.InvariantCulture, (string)(Application.Current.TryFindResource("ImportaErroreXML") ?? "Error importing the file: {0}"), exc.Message), (string)(Application.Current.TryFindResource("Errore") ?? "Error"));
                });
                return false;
            }
            return true;
        }

        public static string GetAppTempFolder()
        {
            string AppTempFolderName = "LaParola";
            // 1. Try System Temp (Preferred)
            try
            {
                string systemTemp = Path.Combine(Path.GetTempPath(), AppTempFolderName);
                Directory.CreateDirectory(systemTemp);
                return systemTemp;
            }
            catch (Exception)
            {
                // System temp was unwritable or restricted
            }

            // 2. User's LocalAppData
            try
            {
                string localAppData = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    AppTempFolderName);

                Directory.CreateDirectory(localAppData);
                return localAppData;
            }
            catch { }

            // 3. Try App Directory (USB Stick folder)
            try
            {
                string appDirTemp = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppTempFolderName);
                Directory.CreateDirectory(appDirTemp);
                return appDirTemp;
            }
            catch (Exception)
            {
                // App directory is read-only (e.g., read-only USB or Program Files)
            }

            return "";
        }

        private static string RtfToPlainText(string rtf)
        {
            string t = RegexRTFPlain1().Replace(rtf, "");
            t = RegexRTFPlain2().Replace(t, "");
            t = RegexRTFPlain3().Replace(t, m => ((char)Convert.ToInt32(m.Groups[1].Value, 16)).ToString());
            t = RegexRTFPlain4().Replace(t, m => ((char)int.Parse(m.Groups[1].Value)).ToString());
            return t.Trim();
        }

        internal static MetaData CaricaMetadatiDaFile(string percorsoInfo)
        {
            MetaData dati = new();
            try
            {
                Dictionary<string, Action<string>> campi = new(StringComparer.OrdinalIgnoreCase)
                {
                    ["Titolo"] = v => dati.Titolo = v,
                    ["Abbreviazione"] = v => dati.Abbreviazione = v,
                    ["Autore"] = v => dati.Autore = v,
                    ["CasaEditrice"] = v => dati.CasaEditrice = v,
                    ["Copyright"] = v => dati.Copyright = v,
                    ["Descrizione"] = v => dati.Descrizione = v,
                    ["Data"] = v => dati.Data = v,
                    ["ISBN"] = v => dati.ISBN = v,
                    ["Lingua"] = v => dati.Lingua = v,
                    ["VersioneDelleNote"] = v => dati.VersioneDelleNote = v,
                };
                foreach (string riga in File.ReadAllLines(percorsoInfo, Encoding.UTF8))
                {
                    int pos = riga.IndexOf('=');
                    if (pos <= 0) continue;
                    string chiave = riga[..pos].Trim();
                    string valore = riga[(pos + 1)..].Trim();
                    if (campi.TryGetValue(chiave, out Action<string>? setter))
                        setter(valore);
                }
            }
            catch { }
            return dati;
        }

        internal static string CreaAbbreviazione(string nomeTesto)
        {
            string[] paroleTesto = nomeTesto.Trim().Split(' ');
            if (paroleTesto.Length > 1)
            {
                StringBuilder abbreviazione = new("");
                foreach (string s in paroleTesto)
                    abbreviazione.Append(s[0]);
                return abbreviazione.ToString();
            }
            else
            {
                if (nomeTesto.Length >= 5)
                    return nomeTesto[..4];
                else
                    return nomeTesto;
            }
        }

        internal static string ImpostaNomeFileLaParolaDaFileOrigine(string percorsoFile)
        {
            int suffisso = 0;
            string nomeVersioneUtilizzato;
            string nomeVersione = Path.GetFileNameWithoutExtension(percorsoFile);
            string percorsoPerSalvare = Path.GetDirectoryName(SettingsService.ResolveSettingsPath()) ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LaParola");
            nomeVersioneUtilizzato = percorsoPerSalvare + Path.DirectorySeparatorChar + nomeVersione;
            while (File.Exists(nomeVersioneUtilizzato + ".laparola"))
            {
                suffisso += 1;
                nomeVersioneUtilizzato = percorsoPerSalvare + Path.DirectorySeparatorChar + nomeVersione + suffisso.ToString(CultureInfo.InvariantCulture);
            }
            return nomeVersioneUtilizzato;
        }

        private static string[] ConvertiOsisARtfETesto(XmlNode? xn)
        {
            if (xn == null)
                return [];

            StringBuilder rtf = new("");
            StringBuilder testo = new("");
            string[] testoDelSottoNodo;
            int p, i;
            string lemma, morph;
            bool parolaFatta;
            if (xn.PreviousSibling != null && xn.PreviousSibling.Name == "title")
            {
                XmlNode? nodoForseConTitolo = xn.PreviousSibling;
                if (nodoForseConTitolo.Attributes?["type"]?.Value == "psalm")
                {
                    //nodoForseConTitolo = nodoForseConTitolo.PreviousSibling;
                    // quando c'è un titolo inglese prima del titolo canonico, viene aggiunto in ConvertiOsisARtfETesto
                    rtf.Append(@"\lptit1 ").Append(ConvertiOsisARtfETesto(xn.PreviousSibling)[0]).Append(@"\lptit0 \par ");
                }
                else if (nodoForseConTitolo.Attributes?["type"] == null || nodoForseConTitolo.Attributes["type"]?.Value != "chapter")
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
                            if (nodo.Attributes?["who"]?.Value == "Jesus")
                            {
                                testoDelSottoNodo = ConvertiOsisARtfETesto(nodo);
                                rtf.Append(testoDelSottoNodo[0]);
                                testo.Append(testoDelSottoNodo[1]);
                            }
                            else
                            {
                                testoDelSottoNodo = ConvertiOsisARtfETesto(nodo);
                                if (rtf.ToString().EndsWith('}'))
                                {
                                    // quando ci sono due parole, con uno spazio non corsivo in mezzo,
                                    // lo spazio è saltato perché InnerText non può essere vuoto
                                    rtf.Append(' ');
                                    testo.Append(' ');
                                }
                                if (testoDelSottoNodo[0].EndsWith(@"\par ", StringComparison.Ordinal))
                                    testoDelSottoNodo[0] = testoDelSottoNodo[0][..^1];
                                rtf.Append(@"{\i1 ").Append(testoDelSottoNodo[0]).Append('}');
                                testo.Append(testoDelSottoNodo[1]);
                            }
                            break;
                        case "hi":
                            if (nodo.Attributes != null && nodo.Attributes["type"] != null)
                            {
                                if (rtf.ToString().EndsWith('}'))
                                {
                                    // quando ci sono due parole, con uno spazio non corsivo in mezzo,
                                    // lo spazio è saltato perché InnerText non può essere vuoto
                                    rtf.Append(' ');
                                    testo.Append(' ');
                                }
                                if (nodo.Attributes?["type"]?.Value == "italic")
                                {
                                    testoDelSottoNodo = ConvertiOsisARtfETesto(nodo);
                                    rtf.Append(@"{\i1 ").Append(testoDelSottoNodo[0]).Append('}');
                                    testo.Append(testoDelSottoNodo[1]);
                                }
                                if (nodo.Attributes?["type"]?.Value == "bold")
                                {
                                    testoDelSottoNodo = ConvertiOsisARtfETesto(nodo);
                                    rtf.Append(@"{\b1 ").Append(testoDelSottoNodo[0]).Append('}');
                                    testo.Append(testoDelSottoNodo[1]);
                                }
                                else if (nodo.Attributes?["type"]?.Value == "small-caps")
                                {
                                    testoDelSottoNodo = ConvertiOsisARtfETesto(nodo);
                                    rtf.Append(@"{\caps ").Append(testoDelSottoNodo[0]).Append('}');
                                    testo.Append(testoDelSottoNodo[1]);
                                }
                                else if (nodo.Attributes?["type"]?.Value == "super")
                                {
                                    testoDelSottoNodo = ConvertiOsisARtfETesto(nodo);
                                    rtf.Append(@"{\super ").Append(testoDelSottoNodo[0]).Append('}');
                                    testo.Append(testoDelSottoNodo[1]);
                                }
                                else
                                    throw new FormatException(String.Format(CultureInfo.InvariantCulture, (string)(Application.Current.TryFindResource("ImportaCodiceSconosciuto") ?? "Unrecognised code: {0}"), nodo.Name));
                            }
                            else
                                throw new FormatException(String.Format(CultureInfo.InvariantCulture, (string)(Application.Current.TryFindResource("ImportaCodiceSconosciuto") ?? "Unrecognised code: {0}"), nodo.Name));
                            break;
                        case "title":
                            if (nodo.Attributes?["canonical"]?.Value == "true")
                            {
                                testoDelSottoNodo = ConvertiOsisARtfETesto(nodo);
                                rtf.Append(@"{\b1 ").Append(testoDelSottoNodo[0]).Append("} ");
                                testo.Append(testoDelSottoNodo[1]).Append(' ');
                            }
                            else if (nodo.Attributes?["type"]?.Value == "psalm")
                            {
                                rtf.Append(@"\lptit1 ").Append(ConvertiOsisARtfETesto(nodo)[0]).Append(@"\lptit0 \par ");
                            }
                            else
                                throw new FormatException(String.Format(CultureInfo.InvariantCulture, (string)(Application.Current.TryFindResource("ImportaCodiceSconosciuto") ?? "Unrecognised code: {0}"), nodo.Name));
                            break;
                        case "transChange":
                            if (nodo.Attributes?["type"]?.Value == "added")
                            {
                                if (testo.Length > 0 && !testo.ToString().EndsWith(' ') && !testo.ToString().EndsWith('('))
                                {
                                    rtf.Append(' ');
                                    testo.Append(' ');
                                }
                                testoDelSottoNodo = ConvertiOsisARtfETesto(nodo);
                                rtf.Append(@"{\i1 ").Append(testoDelSottoNodo[0]).Append("} ");
                                testo.Append(testoDelSottoNodo[1]).Append(' ');
                            }
                            else
                                throw new FormatException(String.Format(CultureInfo.InvariantCulture, (string)(Application.Current.TryFindResource("ImportaCodiceSconosciuto") ?? "Unrecognised code: {0}"), nodo.Name));
                            break;
                        case "inscription":
                        case "seg":
                        case "foreign":
                            if (nodo.Name == "foreign" || (nodo.Attributes != null && nodo.Attributes.Count == 0))
                            {
                                testoDelSottoNodo = ConvertiOsisARtfETesto(nodo);
                                if (rtf.ToString().EndsWith('}') && testoDelSottoNodo[0].StartsWith('{'))
                                {
                                    rtf.Append(' ');
                                    testo.Append(' ');
                                }
                                rtf.Append(testoDelSottoNodo[0]);
                                testo.Append(testoDelSottoNodo[1]);
                            }
                            else if (nodo.Name == "seg" && (nodo.Attributes?["subType"]?.Value == "x-added"))
                            {
                                testoDelSottoNodo = ConvertiOsisARtfETesto(nodo);
                                rtf.Append(@"{\i1 ").Append(testoDelSottoNodo[0]).Append('}');
                                testo.Append(testoDelSottoNodo[1]);
                            }
                            else
                                throw new FormatException(String.Format(CultureInfo.InvariantCulture, (string)(Application.Current.TryFindResource("ImportaCodiceSconosciuto") ?? "Unrecognised code: {0}"), nodo.Name));
                            break;
                        case "divineName":
                            testoDelSottoNodo = ConvertiOsisARtfETesto(nodo);
                            rtf.Append(@"{\caps ").Append(testoDelSottoNodo[0]).Append('}');
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
                            lemma = nodo.Attributes?["lemma"]?.Value ?? "";
                            morph = nodo.Attributes?["morph"]?.Value ?? "";
                            parolaFatta = false;
                            if (lemma.StartsWith("strong:", StringComparison.OrdinalIgnoreCase) || lemma.StartsWith("strongab:", StringComparison.OrdinalIgnoreCase))
                            {
                                while (lemma.IndexOf("strongab:", StringComparison.OrdinalIgnoreCase) > -1 || lemma.IndexOf("strong:", StringComparison.OrdinalIgnoreCase) > -1)
                                {
                                    if (lemma.IndexOf("strongab:", StringComparison.OrdinalIgnoreCase) > -1 && (lemma.IndexOf("strong:", StringComparison.OrdinalIgnoreCase) < 0 || lemma.IndexOf("strongab:", StringComparison.OrdinalIgnoreCase) < lemma.IndexOf("strong:", StringComparison.OrdinalIgnoreCase)))
                                        lemma = string.Concat(lemma.AsSpan(0, lemma.IndexOf("strongab:", StringComparison.OrdinalIgnoreCase)), lemma.AsSpan(lemma.IndexOf("strongab:", StringComparison.OrdinalIgnoreCase) + 9));
                                    else
                                        lemma = string.Concat(lemma.AsSpan(0, lemma.IndexOf("strong:", StringComparison.OrdinalIgnoreCase)), lemma.AsSpan(lemma.IndexOf("strong:", StringComparison.OrdinalIgnoreCase) + 7));
                                }
                                rtf.Append(@" {\super ").Append(lemma).Append('}');
                                testo.Append(' ').Append(lemma);
                                if (!morph.StartsWith("strongMorph:", StringComparison.OrdinalIgnoreCase))
                                {
                                    rtf.Append(' ');
                                    testo.Append(' ');
                                }
                                parolaFatta = true;
                            }
                            if (morph.StartsWith("strongMorph:", StringComparison.OrdinalIgnoreCase))
                            {
                                while (morph.IndexOf("strongMorph:", StringComparison.OrdinalIgnoreCase) > -1)
                                    morph = string.Concat(morph.AsSpan(0, morph.IndexOf("strongMorph:", StringComparison.OrdinalIgnoreCase)), morph.AsSpan(morph.IndexOf("strongMorph:", StringComparison.OrdinalIgnoreCase) + 12));
                                rtf.Append(@" {\super ").Append(morph).Append("} ");
                                testo.Append(' ').Append(morph).Append(' ');
                                parolaFatta = true;
                            }
                            if (!parolaFatta) // cioè non c'è nessuno dei casi precedenti
                            {
                                rtf.Append(' ');
                                testo.Append(' ');
                            }
                            break;
                        case "div":
                            if (nodo.Attributes?["type"]?.Value == "colophon")
                            {
                                testoDelSottoNodo = ConvertiOsisARtfETesto(nodo);
                                rtf.Append(@" {\i1 ").Append(testoDelSottoNodo[0]).Append('}');
                                testo.Append(' ').Append(testoDelSottoNodo[1]);
                            }
                            break;
                        case "note": // le note non sono importate
                        case "reference":
                            break;
                        default:
                            throw new FormatException(String.Format(CultureInfo.InvariantCulture, (string)(Application.Current.TryFindResource("ImportaCodiceSconosciuto") ?? "Unrecognised code: {0}"), nodo.Name));
                    }
                }
                else
                {
                    if (nodo is XmlElement elem &&
                            ((elem.Name == "p" && elem.HasAttribute("sID")) ||
                            (elem.Name == "milestone" && elem.GetAttribute("type").EndsWith("-p", StringComparison.OrdinalIgnoreCase))))
                    {
                        rtf.Append(@"\par ");
                        testo.Append(' ');
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
                rtfStringa = string.Concat(rtfStringa.AsSpan(0, p), @"\par\lptit0 ", rtfStringa.AsSpan(p + 13));
            }
            return [rtfStringa, testo.ToString()];
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
            return n switch
            {
                17 => 18,
                18 => 27,
                20 => 17,
                21 => 28,
                27 => 32,
                28 => 20,
                32 => 21,
                _ => n,
            };
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
                default:
                    break;
            }
            return libro; // non dovrebbe succedere mai, ma è necessario per non dare un errore nella compilazione del programma
        }

        private static string[] ConvertiZefaniaARtfETesto(XmlNode xn)
        {
            string s = xn.InnerXml.Replace("<DIV>", "").Replace("</DIV>", "");

            while (s.IndexOf("<NOTE", StringComparison.Ordinal) > -1)
            {
                int inizioTag = s.IndexOf("<NOTE", StringComparison.Ordinal);
                int fineTag = s.IndexOf('>', inizioTag);
                if (s[fineTag - 1] == '/')
                {  // una nota vuota, che chiude se stessa <note... />
                    s = string.Concat(s.AsSpan(0, inizioTag), s.AsSpan(fineTag + 1));
                }
                else
                {
                    int tagFine = s.IndexOf("</NOTE>", fineTag, StringComparison.Ordinal);
                    string nota = s.Substring(fineTag + 1, tagFine - fineTag - 1).Trim();
                    s = s[..inizioTag] + @"\{" + nota + @"\}" + s[(tagFine + 7)..];
                }
            }

            string sTesto = s;
            s = SostituisciHtmlTag(s, "STYLE css=\"font-style:italic\"", @"{\i ", @"}");
            s = SostituisciHtmlTag(s, "STYLE css=\"text-decoration:underline\"", @"{\ul ", @"}");
            s = SostituisciHtmlTag(s, "STYLE id=\"cl:divineName\"", @"{\caps ", @"}");
            sTesto = SostituisciHtmlTag(sTesto, "STYLE css=\"font-style:italic\"", "", "");
            sTesto = SostituisciHtmlTag(sTesto, "STYLE css=\"text-decoration:underline\"", "", "");
            sTesto = SostituisciHtmlTag(sTesto, "STYLE id=\"cl:divineName\"", "", "");

            if (s.Contains('<'))
                throw new FormatException(String.Format(CultureInfo.InvariantCulture, (string)(Application.Current.TryFindResource("ImportaCodiceSconosciuto") ?? "Unrecognised code: {0}"), xn.Name));
            return [s, sTesto];
        }

        private static string SostituisciHtmlTagChiusa(string s, string htmlTag, string codiceRtf)
        {
            // sostituisce una tag come <hr .../> (in realtà anche <hr ... >) con codiceRtf; SostituisciHtmlTag elimina sempre una simile tag
            while (s.IndexOf("<" + htmlTag, StringComparison.Ordinal) > -1)
                s = string.Concat(s.AsSpan(0, s.IndexOf("<" + htmlTag, StringComparison.Ordinal)), codiceRtf, s.AsSpan(s.IndexOf('>', s.IndexOf("<" + htmlTag, StringComparison.Ordinal)) + 1));
            return s;
        }

        private static string SostituisciHtmlTag(string s, string htmlTag, string codiceRtfInizio, string codiceRtfFine)
        {
            while (s.IndexOf("<" + htmlTag, StringComparison.Ordinal) > -1)
            {
                int posizioneFine = s.IndexOf('>', s.IndexOf("<" + htmlTag, StringComparison.Ordinal));
                if (s[posizioneFine - 1] == '/') // tipo <sub ... />
                    s = string.Concat(s.AsSpan(0, s.IndexOf("<" + htmlTag, StringComparison.Ordinal)), s.AsSpan(posizioneFine + 1));
                else
                    s = string.Concat(s.AsSpan(0, s.IndexOf("<" + htmlTag, StringComparison.Ordinal)), codiceRtfInizio, s.AsSpan(posizioneFine + 1));
            }
            if (htmlTag.Contains(' '))
                htmlTag = htmlTag[..htmlTag.IndexOf(' ')];
            return s.Replace("</" + htmlTag + ">", codiceRtfFine);
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
            int fineColori = s.IndexOf('}', inizioColori);
            if (fineColori < 0)
                return s;
            int puntoVirgola = s.IndexOf(';', inizioColori);
            int puntoVirgolaPrecedente = inizioColori + 9;
            string coloreStringa;
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
                puntoVirgola = s.IndexOf(';', puntoVirgola + 1);
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
            string codiceFine, testoLink;
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
                    fineLink2 = s.IndexOf('}', inizioLink);
                    if (fineLink2 > 0)
                    {
                        fineLink = fineLink2;
                        codiceFine = "}";
                    }
                }
                testoLink = s.Substring(inizioLink + codiceLink.Length, fineLink - inizioLink - codiceLink.Length);
                if (testoLink[0] == ' ')
                    testoLink = testoLink[1..];
                string testoLinkComeNota;
                testoLinkComeNota = tipo switch
                {
                    RichTextBoxEx.FineLinkBrano => MainWindow.Testi.ConvertiRiferimento(testoLink).ComeNotaTuttoRiferimento(),
                    _ => testoLink,
                };
                if (codiceFine == "}")
                {
                    codiceFine = ""; // non dobbiamo cancellare questo carattere, perché c'è { all'inizio del link
                    if (s[fineLink..] == "}}") // altrimenti .NET non riesce a selezionare tutto il testo del link per crearne un link
                        s = s.Insert(fineLink + 1, @"\par");
                }
                s = s[..inizioLink] + @"\v " + RichTextBoxEx.InizioLink + @"\v0 " + testoLink + @"\v " + RichTextBoxEx.FineLink1 + tipo + testoLinkComeNota + RichTextBoxEx.FineLink2 + @"\v0" + s[(fineLink + codiceFine.Length)..];
                inizioLink = s.IndexOf(codiceLink, StringComparison.Ordinal);
            }
            return s;
        }

        private static string ConvertiApostrofeTrattino(string stringa)
        {
            return stringa.Replace(@"\rquote ", @"'").Replace(@"’", @"'").Replace(@"\rquote\", @"'\").Replace("‘", @"'").Replace("&#8217;", @"'").Replace("–", "-"); // prima – è ASCII 150
        }

        private static string ImportaLibroThML(MetaData data, BinaryWriter bw, ref SortedDictionary<string, List<OccorrenzaParola>> chiave, uint inizioTesto, ref UInt32 numeroVersetto, string libroPrecedente, ref int numeroLibroPrecedente, ref List<byte> capitoliInLibri, ref List<byte> versettiInCapitoli, ref List<int> indice, XmlNode? nodoPrincipale)
        {
            return ImportaLibroThML(data, bw, ref chiave, inizioTesto, ref numeroVersetto, libroPrecedente, ref numeroLibroPrecedente, ref capitoliInLibri, ref versettiInCapitoli, ref indice, nodoPrincipale, null);
        }

        private static string ImportaLibroThML(MetaData data, BinaryWriter bw, ref SortedDictionary<string, List<OccorrenzaParola>> chiave, uint inizioTesto, ref UInt32 numeroVersetto, string libroPrecedente, ref int numeroLibroPrecedente, ref List<byte> capitoliInLibri, ref List<byte> versettiInCapitoli, ref List<int> indice, XmlNode? nodoPrincipale, XmlNode? nodoSecondario)
        {
            if (nodoPrincipale == null)
                return "";

            string testoVersetto = NESSUNO_TROVATO, libro = "", nuovoTesto;
            int versettoPrecedente = 0, numeroLibro;
            byte capitolo = 0, versetto = 0, capitoloPrecedente = 0;

            IEnumerable<XmlNode> nodiPrincipali = nodoPrincipale.SelectNodes("div3/p")?.Cast<XmlNode>() ?? [];
            IEnumerable<XmlNode> nodiSecondari = nodoSecondario?.SelectNodes("div3/p")?.Cast<XmlNode>() ?? [];
            foreach (XmlNode nodo1 in nodiPrincipali.Concat(nodiSecondari))
            {
                foreach (XmlNode nodo2 in nodo1.ChildNodes)
                {
                    if (nodo2.Name == "scripture")
                    {
                        if (!testoVersetto.Equals(NESSUNO_TROVATO) && capitolo != 0)
                        {
                            // funziona anche se "scripture" due volte di seguito, cioè un versetto mancante, quando testoVersetto=""
                            // con Sirach in alcune versioni, il prologo è il capitolo 0. Noi aggiungiamo tutto il testo al primo versetto del capitolo 1
                            indice.Add((int)(bw.Seek(0, SeekOrigin.Current) - inizioTesto)); // -8 perché inizioBibbiaTesto è 8 dopo inizioBibbia
                            testoVersetto = testoVersetto.Replace('\n', ' ').Trim();
                            while (testoVersetto.Contains("  "))
                                testoVersetto = testoVersetto.Replace("  ", " ");
                            XmlNode? sibling = nodo2.NextSibling;
                            while (sibling != null && sibling.Name != "#text")
                                sibling = sibling.NextSibling;
                            if (sibling != null && (sibling.InnerText.StartsWith('¶') || sibling.InnerText.StartsWith("Â ", StringComparison.Ordinal)) && !nodo2.OuterXml.Contains("|1|0|0\"")) // cioè non il primo versetto di un capitolo
                                testoVersetto += @"\par ";
                            bw.Write(testoVersetto);
                            ++numeroVersetto;
                            chiave = Texts.TrovaParoleInVoce(testoVersetto, numeroVersetto, chiave, data.Lingua);
                        }
                        if (!testoVersetto.Equals(NESSUNO_TROVATO) || capitolo != 0) // vedi commento qui sopra su Sirach
                            testoVersetto = "";

                        string brano = nodo2.Attributes?["parsed"]?.Value ?? "";
                        string[] branoParti = brano.Split(['|'], StringSplitOptions.RemoveEmptyEntries);
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
                                    numeroLibro = MainWindow.Testi.GetLibroNumeroDaAbbreviazione(libro);
                                //                                if (!String.IsNullOrEmpty(libroPrecedente))
                                //                                    capitoliInLibri.Add(capitolo); // il numero di capitoli nel libro precedente
                                for (int j = 0; j < numeroLibro - numeroLibroPrecedente - 1; j++)
                                    capitoliInLibri.Add(0);

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
                        while (nuovoTesto.Contains('¶')) // nuovo paragrafo
                        {
                            if (!nuovoTesto.StartsWith('¶')) // quando è all'inizio del versetto, è già stato aggiunto alla fine del versetto precedente
                                nuovoTesto = nuovoTesto.Insert(nuovoTesto.IndexOf('¶') + 1, @"\par "); // questa riga è da controllare ancora
                            nuovoTesto = nuovoTesto.Remove(nuovoTesto.IndexOf('¶'), 1).TrimStart();
                        }
                        if (nuovoTesto.StartsWith("Â ", StringComparison.Ordinal)) // nuovo paragrafo
                        { // quando è all'inizio del versetto, è già stato aggiunto alla fine del versetto precedente
                            nuovoTesto = nuovoTesto[2..].TrimStart();
                        }
                        if (nuovoTesto.StartsWith(" ,", StringComparison.Ordinal))
                        {
                            nuovoTesto = nuovoTesto[1..];
                        }
                        if (nodo2.Name == "span" && nodo2.Attributes?["class"]?.Value == "smallcap")
                        {
                            nuovoTesto = nuovoTesto.ToUpperInvariant();
                        }
                        if (testoVersetto.EndsWith('.') && !nuovoTesto.StartsWith(' '))
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
                        throw new FormatException(String.Format(CultureInfo.InvariantCulture, (string)(Application.Current.TryFindResource("ImportaCodiceSconosciuto") ?? "Unrecognised code: {0}"), nodo2.Name));
                }
            }
            // scrivere il testo rimasto non ancora scritto
            indice.Add((int)(bw.Seek(0, SeekOrigin.Current) - inizioTesto)); // -8 perché inizioBibbiaTesto è 8 dopo inizioBibbia
            testoVersetto = testoVersetto.Replace('\n', ' ').Trim();
            while (testoVersetto.Contains("  "))
                testoVersetto = testoVersetto.Replace("  ", " ");
            bw.Write(testoVersetto);
            ++numeroVersetto;
            chiave = Texts.TrovaParoleInVoce(testoVersetto, numeroVersetto, chiave, data.Lingua);

            versettiInCapitoli.Add(versetto);
            capitoliInLibri.Add(capitolo);

            if (libro == "PrAzar")
                libro = "Dan";
            else if (libro == "AddEsth")
                libro = "Esth";

            return libro;
        }

        private static void AggiungiNoteDaThMLDiv(XmlNode nodo, int livello, List<string> noteTitoliThML, List<string> noteTestoThML, TipoThML thmlTipo)
        {
            StringBuilder testoNota = new("");
            string titolo = nodo.Attributes?["title"]?.Value?.Trim() ?? "";

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
                                AggiungiNoteDaThMLDiv(sottoNodo, 2, noteTitoliThML, noteTestoThML, thmlTipo);
                                testoParagrafo = "";
                                break;
                            case "div3":
                                AggiungiNoteDaThMLDiv(sottoNodo, 3, noteTitoliThML, noteTestoThML, thmlTipo);
                                testoParagrafo = "";
                                break;
                            case "div4":
                                AggiungiNoteDaThMLDiv(sottoNodo, 4, noteTitoliThML, noteTestoThML, thmlTipo);
                                testoParagrafo = "";
                                break;
                            case "div":
                                testoParagrafo = sottoNodo.InnerXml.Trim();
                                if (testoParagrafo.StartsWith("<p ", StringComparison.Ordinal) && testoParagrafo.EndsWith("</p>", StringComparison.Ordinal))
                                    testoParagrafo = testoParagrafo[..^4][(testoParagrafo.IndexOf('>') + 1)..] + @"\par ";
                                if (testoParagrafo.StartsWith("<table ", StringComparison.Ordinal) && testoParagrafo.EndsWith("</table>", StringComparison.Ordinal))
                                    testoParagrafo = testoParagrafo[..^8][(testoParagrafo.IndexOf('>') + 1)..] + @"\par ";
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
                                AggiungiNoteDaThMLDiv(sottoNodo, livello + 1, noteTitoliThML, noteTestoThML, thmlTipo);
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
                                throw new FormatException(String.Format(CultureInfo.InvariantCulture, (string)(Application.Current.TryFindResource("ImportaCodiceSconosciuto") ?? "Unrecognised code: {0}"), sottoNodo.Name));
                        }

                        while (testoParagrafo.IndexOf("<scripCom", StringComparison.Ordinal) > -1 && thmlTipo == TipoThML.Collezione)
                        {
                            int inizioParsed = testoParagrafo.IndexOf("parsed=", testoParagrafo.IndexOf("<scripCom", StringComparison.Ordinal), StringComparison.Ordinal);
                            if (inizioParsed > -1)
                            {
                                // quello che è prima del <scripCom> appartiene alla nota precedente; più tardi il testo sarà cancellato da testoParagrafo
                                testoNota.Append(ConvertiThMLARtf(testoParagrafo[..testoParagrafo.IndexOf("<scripCom", StringComparison.Ordinal)]));

                                string riferimento = testoParagrafo.Substring(inizioParsed + 8, testoParagrafo.IndexOf('"', inizioParsed + 8) - inizioParsed - 8);
                                string[] riferimentoBrani = riferimento.Split([';'], StringSplitOptions.RemoveEmptyEntries);
                                Riferimento titoloRiferimento = new();
                                for (int i = 0; i < riferimentoBrani.Length; ++i)
                                {
                                    string[] branoParti = riferimentoBrani[i].Split(['|'], StringSplitOptions.RemoveEmptyEntries);
                                    int lunghezza = branoParti.Length;
                                    byte numeroLibro = MainWindow.Testi.GetLibroNumeroDaAbbreviazione(branoParti[lunghezza - 5]);
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
                                    titoloRiferimento.AggiungiBrano([numeroLibro, numeroCapitoloDa, numeroVersettoDa, numeroLibro, numeroCapitoloA, numeroVersettoA]);
                                }
                                titolo = titoloRiferimento.ComeNotaTuttoRiferimento();

                                if (!string.IsNullOrEmpty(titolo) && titolo != noteTitoliThML[posizioneNota])
                                { // altrimenti c'è una seconda nota sullo stesso versetto, e possiamo continuare
                                    posizioneNota = AggiungiNota(noteTitoliThML, noteTestoThML, testoNota.ToString(), titolo, posizioneNota);
                                    testoNota.Remove(0, testoNota.Length);
                                }
                            }
                            testoParagrafo = testoParagrafo[testoParagrafo.IndexOf("<scripCom", StringComparison.Ordinal)..];
                            testoParagrafo = testoParagrafo[(testoParagrafo.IndexOf('>') + 1)..];
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
                testoNota = testoNota[5..];

            return MainWindow.Testi.RtfIntestazione() + ConvertiApostrofeTrattino(testoNota).Trim() + "}";
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
            while (s.Contains("<span class=\"MsoEndnoteReference\""))
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
                int fineTag = s.IndexOf('>', inizioTag);
                if (s[fineTag - 1] == '/')
                {  // una nota vuota, che chiude se stessa <note... />
                    s = string.Concat(s.AsSpan(0, inizioTag), s.AsSpan(fineTag + 1));
                }
                else
                {
                    int tagFine = s.IndexOf("</note>", fineTag, StringComparison.Ordinal);
                    //File.WriteAllText(@"c:\test.txt", s);
                    string nota = s.Substring(fineTag + 1, tagFine - fineTag - 1);
                    nota = CancellaHtmlTag(nota, "p").Trim();
                    s = s[..inizioTag] + @"\{" + nota + @"\}" + s[(tagFine + 7)..];
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
                int p1 = s.IndexOf('>', inizioA);
                if (s[p1 - 1] == '/')
                    s = string.Concat(s.AsSpan(0, inizioA), s.AsSpan(p1 + 1));
                else
                {
                    int p2 = s.IndexOf("</a>", p1, StringComparison.Ordinal);
                    if (s.Substring(inizioA, 7) == "<a href")
                    { // se c'è "href" nella tag, usiamo quello per il collegamento, per visualizziamo comunque quello che è visualizzato nel file XML
                        int p3 = s.IndexOf('"', inizioA);
                        int p4 = s.IndexOf('"', p3 + 1);
                        if (s.IndexOf('=', p3) > p3 && s.IndexOf('=', p3) < p4)
                            p3 = s.IndexOf('=', p3);
                        s = s[..inizioA] + @"\v " + RichTextBoxEx.InizioLink + @"\v0 " + s.Substring(p1 + 1, p2 - p1 - 1) + @"\v " + RichTextBoxEx.FineLink1 + RichTextBoxEx.FineLinkNota + s.Substring(p3 + 1, p4 - p3 - 1) + RichTextBoxEx.FineLink2 + @"\v0 " + s[(p2 + 4)..];
                    }
                    else
                    {
                        if (s.Substring(p1 + 1, 9) == "<scripRef")
                            s = string.Concat(s.AsSpan(0, inizioA), s.AsSpan(p1 + 1, p2 - p1 - 1), s.AsSpan(p2 + 4));
                        else
                            s = s[..inizioA] + @"\v " + RichTextBoxEx.InizioLink + @"\v0 " + s.Substring(p1 + 1, p2 - p1 - 1) + @"\v " + RichTextBoxEx.FineLink1 + RichTextBoxEx.FineLinkNota + s.Substring(p1 + 1, p2 - p1 - 1) + RichTextBoxEx.FineLink2 + @"\v0 " + s[(p2 + 4)..];
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
                    riferimento = s.Substring(inizioParsed + 8, s.IndexOf('"', inizioParsed + 8) - inizioParsed - 8);
                    string[] riferimentoBrani = riferimento.Split([';'], StringSplitOptions.RemoveEmptyEntries);
                    riferimentoStringa = "";
                    foreach (string brano in riferimentoBrani)
                    {
                        string[] branoParti = brano.Split(['|'], StringSplitOptions.RemoveEmptyEntries);
                        lunghezza = branoParti.Length;
                        libroStringa = Funzioni.AggiungiZero(MainWindow.Testi.GetLibroNumeroDaAbbreviazione(branoParti[lunghezza - 5]), 2);
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
                    inizioTesto = s.IndexOf('>', inizioRiferimento) + 1;
                    s = s[..inizioRiferimento] + @"\v " + RichTextBoxEx.InizioLink + @"\v0 " + s[inizioTesto..s.IndexOf('<', inizioTesto)] + @"\v " + RichTextBoxEx.FineLink1 + RichTextBoxEx.FineLinkBrano + riferimentoStringa + RichTextBoxEx.FineLink2 + @"\v0 " + s[(s.IndexOf("</scripRef>", inizioRiferimento, StringComparison.Ordinal) + 11)..];
                }
                else
                {
                    inizioTesto = s.IndexOf('>', inizioRiferimento) + 1;
                    s = string.Concat(s.AsSpan(0, inizioRiferimento), s.AsSpan(inizioTesto, s.IndexOf('<', inizioTesto) - inizioTesto), s.AsSpan(s.IndexOf("</scripRef>", inizioRiferimento, StringComparison.Ordinal) + 11));
                }
                inizioRiferimento = s.IndexOf("<scripRef", StringComparison.Ordinal);
            }
            s = s.Replace("</scripRef>", ""); // necessario perché a volte ci sono due scripRef dello stesso versetto uno dentro l'altro, e in quel caso uno finale rimane

            if (s.IndexOf('<') > -1)
                throw new FormatException(String.Format(CultureInfo.InvariantCulture, (string)(Application.Current.TryFindResource("ImportaCodiceSconosciuto") ?? "Unrecognised code: {0}"), s));
            return s;
        }

        private static void ScriviChiaveAFile(BinaryWriter bw, List<OccorrenzaParola> lista)
        {
            byte[] datiDaScrivere = new byte[lista.Count * 6];
            MemoryStream ms = new(datiDaScrivere, true);
            BinaryWriter bwMemoria = new(ms);
            foreach (OccorrenzaParola op in lista)
            {
                bwMemoria.Write(op.Voce);
                bwMemoria.Write(op.Parola);
            }
            bwMemoria.Seek(0, SeekOrigin.Begin);
            bw.Write(datiDaScrivere);
        }


        private static void ScriviNumeroApparenzeParole(BinaryWriter bw, SortedDictionary<string, List<OccorrenzaParola>> chiave)
        {
            UInt32 numeroApparenze = 0;
            byte[] datiDaScrivere = new byte[4 * chiave.Count + 4];
            MemoryStream ms = new(datiDaScrivere, true);
            BinaryWriter bwMemoria = new(ms);
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
            if (nomeFile.Contains('('))
                nomeFile = nomeFile[..nomeFile.IndexOf('(')].Trim();
            string directory = Path.GetDirectoryName(nomeFileBase) ?? ".";
            string[] fileRiferimenti = Directory.GetFiles(directory, nomeFile + "*.riferimenti");
            if (fileRiferimenti.Length == 0)
                throw new Exception();
            string[] riferimentiDiversi = File.ReadAllLines(fileRiferimenti[0]);
            bw.Write(riferimentiDiversi.Length);
            string[] riferimento6Cifre;
            foreach (string riferimentoDiverso in riferimentiDiversi)
            {
                riferimento6Cifre = riferimentoDiverso.Split('|');
                for (int i = 0; i < 6; ++i)
                    bw.Write(Convert.ToInt16(riferimento6Cifre[i], CultureInfo.InvariantCulture));
            }
        }

        private static void ScriviOrdine(BinaryWriter bw, string[]? noteInOrdine, string nomeFileBase)
        {
            // se l'ordine==null, già preso dal file ThML
            noteInOrdine ??= File.ReadAllLines(nomeFileBase + ".ordine"); // deve essere UTF-8
            bw.Write(noteInOrdine.Length);
            foreach (string notaInOrdine in noteInOrdine)
                bw.Write(notaInOrdine);
        }

        private static string CancellaHtmlTag(string s, string htmlTag)
        {
            string htmlTagPiuSpazio = htmlTag + " ";
            while (s.IndexOf("<" + htmlTagPiuSpazio, StringComparison.Ordinal) > -1)
                s = s.Remove(s.IndexOf("<" + htmlTagPiuSpazio, StringComparison.Ordinal), s.IndexOf('>', s.IndexOf("<" + htmlTagPiuSpazio, StringComparison.Ordinal)) - s.IndexOf("<" + htmlTagPiuSpazio, StringComparison.Ordinal) + 1);
            return s.Replace("</" + htmlTag + ">", "");
        }

        private static Task<T> RunInSTAThread<T>(Func<T> action)
        {
            TaskCompletionSource<T> tcs = new();
            Thread thread = new(() =>
            {
                try
                {
                    // Run the code and capture the return value
                    tcs.SetResult(action());
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });

            // Crucial step: Configure the thread before starting it
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            return tcs.Task;
        }

        [GeneratedRegex(@"\\([a-z]+)(-?\d+)?[ ]?")]
        private static partial Regex RegexRTFPlain1();
        [GeneratedRegex(@"\{|}")]
        private static partial Regex RegexRTFPlain2();
        [GeneratedRegex(@"\'([0-9a-fA-F]{2})")]
        private static partial Regex RegexRTFPlain3();
        [GeneratedRegex(@"\\u(\d+)")]
        private static partial Regex RegexRTFPlain4();
    }
}
