using LaParola.ToolViews;
using Microsoft.Win32;
using System.Buffers.Binary;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;

namespace LaParola.Services
{
    public enum TipoEsportazione
    {
        Nessuno,
        EsportaOSIS,
        EsportaZefania,
        EsportaAndroid,
        EsportaFileMultipli,
        EsportaFileSingolo
    }

    internal class EsportaService
    {
        internal async static void EsportaTesto(VersioneInformazioni infoVersione, TipoEsportazione tipo)
        {
            if (tipo == TipoEsportazione.Nessuno)
            {
                return;
            }

            string nomeFile = "";
            if (tipo == TipoEsportazione.EsportaOSIS || tipo == TipoEsportazione.EsportaZefania || tipo == TipoEsportazione.EsportaAndroid)
            {
                string filtro = "";
                if (tipo == TipoEsportazione.EsportaOSIS || tipo == TipoEsportazione.EsportaZefania)
                {
                    filtro = (string)(Application.Current.TryFindResource("BibliotecaAggiungiLibriApriFileXMLFiltro") ?? "XML files (*.xml)|*.xml|All files (*.*)|*.*");
                }
                else if (tipo == TipoEsportazione.EsportaAndroid)
                {
                    filtro = (string)(Application.Current.TryFindResource("BibliotecaEsportaAndroidFiltro") ?? "LaParola Android files (*.lpj)|*.lpj|All files (*.*)|*.*");
                }
                string defaultDirectory = MainWindow.settings.UltimaCartellaEsportareFile;
                if (string.IsNullOrEmpty(defaultDirectory))
                {
                    defaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar;
                }
                if (!Path.Exists(defaultDirectory))
                {
                    try
                    {
                        Directory.CreateDirectory(defaultDirectory);
                    }
                    catch
                    {
                        defaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Path.DirectorySeparatorChar;
                    }
                }

                SaveFileDialog dlg = new()
                {
                    Filter = filtro,
                    FileName = infoVersione.Nome + (tipo == TipoEsportazione.EsportaOSIS || tipo == TipoEsportazione.EsportaZefania ? ".xml" : ".lpj"),
                    InitialDirectory = defaultDirectory
                };

                if (dlg.ShowDialog(Application.Current.MainWindow) == true)
                {
                    nomeFile = dlg.FileName;
                }
                if (string.IsNullOrEmpty(nomeFile))
                {
                    return;
                }
                MainWindow.settings.UltimaCartellaEsportareFile = Path.GetDirectoryName(nomeFile) ?? defaultDirectory;

                if (tipo != TipoEsportazione.EsportaAndroid)
                {
                    AvviaScriviAltriFile(infoVersione, nomeFile, tipo);
                }

                AvviaEsportazioneFile(nomeFile, infoVersione, tipo);
            }

            else if (tipo == TipoEsportazione.EsportaFileMultipli)
            {
                string defaultDirectory = MainWindow.settings.UltimaCartellaEsportareCartella;
                if (string.IsNullOrEmpty(defaultDirectory))
                    defaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar;
                if (!Path.Exists(defaultDirectory))
                {
                    try
                    {
                        Directory.CreateDirectory(defaultDirectory);
                    }
                    catch
                    {
                        defaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Path.DirectorySeparatorChar;
                    }
                }

                OpenFolderDialog dialogCartella = new()
                {
                    Title = (string)(Application.Current.TryFindResource("BibliotecaAggiungiLibriCartellaDialogoTitolo") ?? "Select the directory that contains the files to import"),
                    InitialDirectory = defaultDirectory,
                };

                string selectedDirectory = "";
                if (dialogCartella.ShowDialog(Application.Current.MainWindow) == true)
                {
                    selectedDirectory = dialogCartella.FolderName;
                }
                if (string.IsNullOrEmpty(selectedDirectory) || !Directory.Exists(selectedDirectory))
                {
                    return;
                }
                MainWindow.settings.UltimaCartellaEsportareCartella = selectedDirectory;
                string provaDirectory = Path.Combine(selectedDirectory, infoVersione.Nome);
                try
                {
                    Directory.CreateDirectory(provaDirectory);
                }
                catch (Exception)
                {
                    // On error (Access Denied, Invalid Characters, Disk Read-Only, etc.), fallback to 'a'
                    provaDirectory = selectedDirectory;
                }
                selectedDirectory = provaDirectory;

                AvviaScriviAltriFile(infoVersione, Path.Combine(selectedDirectory, infoVersione.Nome), tipo);

                await AvviaEsportazioneCartella(selectedDirectory, infoVersione, tipo);
            }
            else if (tipo == TipoEsportazione.EsportaFileSingolo)
            {
                if ((infoVersione.Tipo & TestoTipi.Bibbia) == TestoTipi.Bibbia)
                {
                    TextGeneratorToolView.MostraBranoInEditor(MainWindow.Testi.LibriAbbreviazioniRiconosciute.Abbreviazione(1)+"-"+ MainWindow.Testi.LibriAbbreviazioniRiconosciute.Abbreviazione(73), infoVersione.Nome);
                }
                else
                {
                    TextGeneratorToolView.MostraBranoInEditor(MainWindow.Testi.NotePrimaOrdinate(infoVersione.Nome, true), infoVersione.Nome);
                }
            }
        }

        private static void AvviaScriviAltriFile(VersioneInformazioni infoVersione, string nomeFile, TipoEsportazione tipo)
        {
            _ = Task.Run(() =>
            {
                try
                {
                    ScriviAltriFile(infoVersione, nomeFile, tipo);
                }
                catch (Exception ex)
                {
                    // Always catch exceptions in un-awaited tasks to prevent silent failures
                    string messaggio = (string)(Application.Current.TryFindResource("BibliotecaEsportaErrore") ?? "Error during the exporting: {0}");
                    messaggio = string.Format(messaggio, ex.Message).Replace("\\n", "\n");
                    MessageBoxLPN.Show(Application.Current.MainWindow, messaggio, (string)(Application.Current.TryFindResource("Errore") ?? "Error"));
                }
            });
        }

        private async static void ScriviAltriFile(VersioneInformazioni versione, string nomeFile, TipoEsportazione tipo)
        {
            if (tipo != TipoEsportazione.EsportaOSIS && tipo != TipoEsportazione.EsportaZefania && tipo != TipoEsportazione.EsportaAndroid)
            {
                List<string> righeDati = new(10);
                if (!string.IsNullOrEmpty(versione.Titolo))
                    righeDati.Add("Titolo=" + versione.Titolo);
                if (!string.IsNullOrEmpty(versione.Abbreviazione))
                    righeDati.Add("Abbreviazione=" + versione.Abbreviazione);
                if (!string.IsNullOrEmpty(versione.Autore))
                    righeDati.Add("Autore=" + versione.Autore);
                if (!string.IsNullOrEmpty(versione.CasaEditrice))
                    righeDati.Add("CasaEditrice=" + versione.CasaEditrice);
                if (!string.IsNullOrEmpty(versione.Copyright))
                    righeDati.Add("Copyright=" + versione.Copyright);
                if (!string.IsNullOrEmpty(versione.Data))
                    righeDati.Add("Data=" + versione.Data);
                if (!string.IsNullOrEmpty(versione.Isbn))
                    righeDati.Add("ISBN=" + versione.Isbn);
                if (!string.IsNullOrEmpty(versione.Lingua))
                    righeDati.Add("Lingua=" + versione.Lingua);
                if (!string.IsNullOrEmpty(versione.VersioneDelleNote))
                     righeDati.Add("VersioneDelleNote=" + versione.VersioneDelleNote);
                if (!string.IsNullOrEmpty(versione.Descrizione))
                    righeDati.Add("Descrizione=" + versione.Descrizione);
                await File.WriteAllLinesAsync(Path.ChangeExtension(nomeFile, ".laparolainfo"), [.. righeDati], Encoding.UTF8);
            }
            List<string> paroleRadici = [.. MainWindow.Testi.GetParoleRadici(versione.Nome)];
            if (paroleRadici.Count > 0)
                await File.WriteAllLinesAsync(Path.ChangeExtension(nomeFile, ".parole_radici"), [.. paroleRadici], Encoding.UTF8);
            List<string> radiciDiverse = [.. MainWindow.Testi.GetRadiciDiverse(versione.Nome)];
            if (radiciDiverse.Count > 0)
                await File.WriteAllLinesAsync(Path.ChangeExtension(nomeFile, ".radici_diverse"), [.. radiciDiverse], Encoding.UTF8);
            List<string> riferimentiDiversi = [.. MainWindow.Testi.GetRiferimentiDiversi(versione.Nome)];
            if (riferimentiDiversi.Count > 0)
                await File.WriteAllLinesAsync(Path.ChangeExtension(nomeFile, ".riferimenti"), [.. riferimentiDiversi], Encoding.UTF8);
        }

        private async static void AvviaEsportazioneFile(string nomeFile, VersioneInformazioni info, TipoEsportazione tipo)
        {
            string messaggioBase = (string)(Application.Current.TryFindResource("BibliotecaEsportaMessaggio") ?? "Exporting file");
            using StatusTask? statusTask = StatusService.AvviaTask(messaggioBase, isIndeterminate: false);
            Progress<double>? progress = new(percent =>
            {
                statusTask.Update(messaggioBase, percent);
            });
            try
            {
                if (tipo == TipoEsportazione.EsportaOSIS)
                {
                    await Task.Run(() => EsportaOSIS(nomeFile, info, progress));
                }
                else if (tipo == TipoEsportazione.EsportaZefania)
                {
                    await Task.Run(() => EsportaZefania(nomeFile, info, progress));
                }
                else if (tipo == TipoEsportazione.EsportaAndroid)
                {
                    await Task.Run(() => EsportaAndroid(nomeFile, info, progress));
                }
                statusTask.Update((string)(Application.Current.TryFindResource("BibliotecaEsportaFinito") ?? "Exporting file completed"), 100.0);
                await Task.Delay(5000); // lasciare il messaggio, poi scompare dopo 5 secondi
            }
            catch (Exception ex)
            {
                string messaggio = (string)(Application.Current.TryFindResource("BibliotecaEsportaErrore") ?? "Error during the exporting: {0}");
                messaggio = string.Format(messaggio, ex.Message).Replace("\\n", "\n");
                MessageBoxLPN.Show(Application.Current.MainWindow, messaggio, (string)(Application.Current.TryFindResource("Errore") ?? "Error"));
            }
        }

        private async static Task AvviaEsportazioneCartella(string cartella, VersioneInformazioni info, TipoEsportazione tipo)
        {
            string messaggioBase = (string)(Application.Current.TryFindResource("BibliotecaEsportaFilesMessaggio") ?? "Exporting files");
            using StatusTask? statusTask = StatusService.AvviaTask(messaggioBase, isIndeterminate: false);
            Progress<double>? progress = new(percent =>
            {
                statusTask.Update(messaggioBase, percent);
            });
            try
            {
                if (tipo == TipoEsportazione.EsportaFileMultipli)
                {
                    await EsportaFileMultipli(cartella, info, progress);
                }
                statusTask.Update((string)(Application.Current.TryFindResource("BibliotecaEsportaFilesFinito") ?? "Exporting files completed"), 100.0);
                await Task.Delay(5000); // lasciare il messaggio, poi scompare dopo 5 secondi
            }
            catch (Exception ex)
            {
                string messaggio = (string)(Application.Current.TryFindResource("BibliotecaEsportaErrore") ?? "Error during the exporting: {0}");
                messaggio = string.Format(messaggio, ex.Message).Replace("\\n", "\n");
                MessageBoxLPN.Show(Application.Current.MainWindow, messaggio, (string)(Application.Current.TryFindResource("Errore") ?? "Error"));
            }
        }

        private async static void EsportaOSIS(string nomeFile, VersioneInformazioni info, IProgress<double>? progress = null)
        {
            List<string> righeDaScrivere = new(32768);
            int capitoliInLibro, versettiInCapitolo;
            string testo;
            int fineTag, inizioTag;
            string[] libriNomiOSIS = ["Gen", "Exod", "Lev", "Num", "Deut",
         "Josh", "Judg", "Ruth", "1Sam", "2Sam", "1Kgs", "2Kgs", "1Chr", "2Chr", "Ezra", "Neh", "Tob", "Jdt", "Esth", "1Macc", "2Macc",
         "Job", "Ps", "Prov", "Eccl", "Song", "Wis", "Sir",
         "Isa", "Jer", "Lam", "Bar", "Ezek", "Dan",
         "Hos", "Joel", "Amos", "Obad", "Jonah", "Mic", "Nah", "Hab", "Zeph", "Hag", "Zech", "Mal",
         "Matt", "Mark", "Luke", "John", "Acts",
         "Rom", "1Cor", "2Cor", "Gal", "Eph", "Phil", "Col", "1Thess", "2Thess", "1Tim", "2Tim", "Titus", "Phlm",
         "Heb", "Jas", "1Pet", "2Pet", "1John", "2John", "3John", "Jude", "Rev"];

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
            StringBuilder rigaOsis = new(512);
            StringBuilder paragrafo = new(32);
            int numeroParagrafo = 1;

            for (byte iLibro = 1; iLibro <= 73; ++iLibro)
            {
                capitoliInLibro = MainWindow.Testi.CapitoliInLibro(iLibro, info.Nome);
                if (capitoliInLibro > 0)
                    righeDaScrivere.Add("<div type=\"book\" osisID=\"" + libriNomiOSIS[iLibro - 1] + "\">");
                for (byte iCapitolo = 1; iCapitolo <= capitoliInLibro; ++iCapitolo)
                {
                    versettiInCapitolo = MainWindow.Testi.VersettiInCapitolo(iLibro, iCapitolo, info.Nome);
                    rigaOsis.Length = 0;
                    righeDaScrivere.Add(rigaOsis.Append("  <chapter osisID=\"").Append(libriNomiOSIS[iLibro - 1]).Append('.').Append(iCapitolo).Append("\">").ToString());
                    for (byte iVersetto = 1; iVersetto <= versettiInCapitolo; ++iVersetto)
                    {
                        testo = MainWindow.Testi.TestoVersettoRaw(iLibro, iCapitolo, iVersetto, info.Nome);
                        testo = testo.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;"); // bisogna fare prima di aggiungere tag <...> qui sotto
                        /*
                         * non necessario
                        while (testo.IndexOf(@"\'") >= 0)
                        {
                            indiceCarattereHex = testo.IndexOf(@"\'");
                            testo = testo.Substring(0, indiceCarattereHex) + "&#" + (Uri.FromHex(testo[indiceCarattereHex + 2]) * 16 + Uri.FromHex(testo[indiceCarattereHex + 3])) + ";" + testo.Substring(indiceCarattereHex + 4);
                        }*/
                        while (testo.Contains(@"\par", StringComparison.OrdinalIgnoreCase))
                        {
                            paragrafo.Length = 0;
                            testo = paragrafo.Append(testo.AsSpan(0, testo.IndexOf(@"\par", StringComparison.OrdinalIgnoreCase))).Append("<p eID=\"").Append(numeroParagrafo).Append("\" /><p sID=\"").Append(numeroParagrafo + 1).Append("\" />").Append(testo.AsSpan(testo.IndexOf(@"\par", StringComparison.OrdinalIgnoreCase) + 4)).ToString();
                            ++numeroParagrafo;
                        }

                        while (testo.Contains(@" {\super ", StringComparison.OrdinalIgnoreCase))
                        { // i numeri Strong non sono esportati
                            inizioTag = testo.IndexOf(@" {\super ", StringComparison.OrdinalIgnoreCase);
                            fineTag = testo.IndexOf("}", inizioTag, StringComparison.OrdinalIgnoreCase);
                            testo = string.Concat(testo.AsSpan(0, inizioTag), testo.AsSpan(fineTag + 1));
                        }

                        while (testo.Contains(@"\lptit1 ", StringComparison.OrdinalIgnoreCase))
                            testo = string.Concat(testo.AsSpan(0, testo.IndexOf(@"\lptit1 ", StringComparison.OrdinalIgnoreCase)), "<head>", testo.AsSpan(testo.IndexOf(@"\lptit1 ", StringComparison.OrdinalIgnoreCase) + 8));
                        while (testo.Contains(@"\lptit0 ", StringComparison.OrdinalIgnoreCase))
                            testo = string.Concat(testo.AsSpan(0, testo.IndexOf(@"\lptit0 ", StringComparison.OrdinalIgnoreCase)), "</head>", testo.AsSpan(testo.IndexOf(@"\lptit0 ", StringComparison.OrdinalIgnoreCase) + 8));
                        // nota: durante l'importazione, il nuovo paragrafo è messo dentro le tag \lptit, anche se nel file OSIS originale era dopo la chiusura
                        // quindi il file esportato sarà leggermente diverso
                        while (testo.Contains(@"{\i1 ", StringComparison.OrdinalIgnoreCase))
                        {
                            inizioTag = testo.IndexOf(@"{\i1 ", StringComparison.OrdinalIgnoreCase);
                            fineTag = testo.IndexOf("}", inizioTag, StringComparison.OrdinalIgnoreCase);
                            testo = testo[..inizioTag] + "<q>" + testo.Substring(inizioTag + 5, fineTag - inizioTag - 5) + "</q>" + testo[(fineTag + 1)..];
                        }
                        while (testo.Contains(@"{\b1 ", StringComparison.OrdinalIgnoreCase)) // deve essere dopo "super", nel caso di parole con super dentro il titolo
                        {
                            inizioTag = testo.IndexOf(@"{\b1 ", StringComparison.OrdinalIgnoreCase);
                            fineTag = testo.IndexOf("}", inizioTag, StringComparison.OrdinalIgnoreCase);
                            testo = testo[..inizioTag] + "<title>" + testo.Substring(inizioTag + 5, fineTag - inizioTag - 5) + "</title>" + testo[(fineTag + 1)..];
                        }
                        while (testo.Contains(@"{\caps ", StringComparison.OrdinalIgnoreCase))
                        {
                            inizioTag = testo.IndexOf(@"{\caps ", StringComparison.OrdinalIgnoreCase);
                            fineTag = testo.IndexOf("}", inizioTag, StringComparison.OrdinalIgnoreCase);
                            testo = testo[..inizioTag] + "<divineName>" + testo.Substring(inizioTag + 7, fineTag - inizioTag - 7) + "</divineName>" + testo[(fineTag + 1)..];
                        }
                        /*                                    while (testo.IndexOf(@"{\qr ") >= 0)
                                                            {
                                                                inizioTag = testo.IndexOf(@"{\qr ");
                                                                fineTag = testo.IndexOf('}', inizioTag);
                                                                testo = testo.Substring(0, inizioTag) + testo.Substring(inizioTag + 5, fineTag - inizioTag - 5) + testo.Substring(fineTag + 1);
                                                            }*/
                        //if (testo.Contains('{') || testo.Contains('}') || testo.Contains('\\'))
                        //    inizioTag = 0;
                        rigaOsis.Length = 0;
                        if (!string.IsNullOrEmpty(testo))
                            righeDaScrivere.Add(rigaOsis.Append("    <verse osisID=\"").Append(libriNomiOSIS[iLibro - 1]).Append('.').Append(iCapitolo).Append('.').Append(iVersetto).Append("\">").Append(testo.Trim()).Append("</verse>").ToString());
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
                progress?.Report((iLibro / 73.0) * 100.0);
            }

            righeDaScrivere.Add("</div>");
            righeDaScrivere.Add("<p eID=\"" + numeroParagrafo + "\" />");
            righeDaScrivere.Add("</osisText>");
            righeDaScrivere.Add("</osis>");
            await File.WriteAllLinesAsync(nomeFile, [.. righeDaScrivere], Encoding.UTF8);
        }

        private async static void EsportaZefania(string nomeFile, VersioneInformazioni info, IProgress<double>? progress = null)
        {
            List<string> righeDaScrivere = new(32768);
            int capitoliInLibro, versettiInCapitolo;
            string testo;
            int fineTag, inizioTag;

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

            StringBuilder rigaZefania = new(512);
            for (byte iLibro = 1; iLibro <= 73; ++iLibro)
            {
                capitoliInLibro = MainWindow.Testi.CapitoliInLibro(iLibro, info.Nome);
                if (capitoliInLibro > 0)
                    righeDaScrivere.Add("<BIBLEBOOK bnumber=\"" + ConvertiLibro73A66Zefania(iLibro).ToString(CultureInfo.InvariantCulture) + "\">");
                for (byte iCapitolo = 1; iCapitolo <= capitoliInLibro; ++iCapitolo)
                {
                    versettiInCapitolo = MainWindow.Testi.VersettiInCapitolo(iLibro, iCapitolo, info.Nome);
                    rigaZefania.Length = 0;
                    righeDaScrivere.Add(rigaZefania.Append("  <CHAPTER cnumber=\"").Append(iCapitolo).Append("\">").ToString());
                    for (byte iVersetto = 1; iVersetto <= versettiInCapitolo; ++iVersetto)
                    {
                        testo = MainWindow.Testi.TestoVersettoRaw(iLibro, iCapitolo, iVersetto, info.Nome);
                        testo = testo.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;"); // bisogna fare prima di aggiungere tag <...> qui sotto
                        while (testo.Contains(@"\par", StringComparison.OrdinalIgnoreCase))
                        {
                            inizioTag = testo.IndexOf(@"\par", StringComparison.OrdinalIgnoreCase);
                            testo = string.Concat(testo.AsSpan(0, inizioTag), testo.AsSpan(inizioTag + 4));
                        }

                        while (testo.Contains(@" {\super ", StringComparison.OrdinalIgnoreCase))
                        { // i numeri Strong non sono esportati
                            inizioTag = testo.IndexOf(@" {\super ", StringComparison.OrdinalIgnoreCase);
                            fineTag = testo.IndexOf("}", inizioTag, StringComparison.OrdinalIgnoreCase);
                            testo = string.Concat(testo.AsSpan(0, inizioTag), testo.AsSpan(fineTag + 1));
                        }

                        /*
                        // lasciavo i titoli, ma non so perché. Meglio senza
                        while (testo.IndexOf(@"\lptit1 ", StringComparison.OrdinalIgnoreCase) >= 0)
                            testo = testo.Substring(0, testo.IndexOf(@"\lptit1 ", StringComparison.OrdinalIgnoreCase)) + testo.Substring(testo.IndexOf(@"\lptit1 ", StringComparison.OrdinalIgnoreCase) + 8);
                        while (testo.IndexOf(@"\lptit0 ", StringComparison.OrdinalIgnoreCase) >= 0)
                            testo = testo.Substring(0, testo.IndexOf(@"\lptit0 ", StringComparison.OrdinalIgnoreCase)) + " " + testo.Substring(testo.IndexOf(@"\lptit0 ", StringComparison.OrdinalIgnoreCase) + 8);
                        */
                        while (testo.Contains(@"\lptit1 ", StringComparison.OrdinalIgnoreCase))
                        {
                            inizioTag = testo.IndexOf(@"\lptit1 ", StringComparison.OrdinalIgnoreCase);
                            fineTag = testo.IndexOf(@"\lptit0 ", inizioTag, StringComparison.OrdinalIgnoreCase);
                            testo = string.Concat(testo.AsSpan(0, inizioTag), testo.AsSpan(fineTag + 8));
                        }
                        while (testo.Contains(@"{\i1 ", StringComparison.OrdinalIgnoreCase))
                        {
                            inizioTag = testo.IndexOf(@"{\i1 ", StringComparison.OrdinalIgnoreCase);
                            fineTag = testo.IndexOf("}", inizioTag, StringComparison.OrdinalIgnoreCase);
                            testo = string.Concat(testo.AsSpan(0, inizioTag), testo.AsSpan(inizioTag + 5, fineTag - inizioTag - 5), testo.AsSpan(fineTag + 1));
                        }
                        while (testo.Contains(@"{\b1 ", StringComparison.OrdinalIgnoreCase)) // deve essere dopo "super", nel caso di parole con super dentro il titolo
                        {
                            inizioTag = testo.IndexOf(@"{\b1 ", StringComparison.OrdinalIgnoreCase);
                            fineTag = testo.IndexOf("}", inizioTag, StringComparison.OrdinalIgnoreCase);
                            testo = string.Concat(testo.AsSpan(0, inizioTag), testo.AsSpan(inizioTag + 5, fineTag - inizioTag - 5), testo.AsSpan(fineTag + 1));
                        }
                        while (testo.Contains(@"{\caps ", StringComparison.OrdinalIgnoreCase))
                        {
                            inizioTag = testo.IndexOf(@"{\caps ", StringComparison.OrdinalIgnoreCase);
                            fineTag = testo.IndexOf("}", inizioTag, StringComparison.OrdinalIgnoreCase);
                            testo = string.Concat(testo.AsSpan(0, inizioTag), testo.AsSpan(inizioTag + 7, fineTag - inizioTag - 7), testo.AsSpan(fineTag + 1));
                        }
                        //if (testo.Contains('{', StringComparison.OrdinalIgnoreCase) || testo.Contains('}', StringComparison.OrdinalIgnoreCase) || testo.Contains('\\', StringComparison.OrdinalIgnoreCase))
                        //    inizioTag = 0;
                        rigaZefania.Length = 0;
                        if (!string.IsNullOrEmpty(testo))
                            righeDaScrivere.Add(rigaZefania.Append("    <VERS vnumber=\"").Append(iVersetto).Append("\">").Append(testo.Trim()).Append("</VERS>").ToString());
                    }
                    righeDaScrivere.Add("  </CHAPTER>");
                }
                if (capitoliInLibro > 0)
                    righeDaScrivere.Add("</BIBLEBOOK>");
                progress?.Report((iLibro / 73.0) * 100.0);
            }

            righeDaScrivere.Add("</XMLBIBLE>");
            await File.WriteAllLinesAsync(nomeFile, [.. righeDaScrivere], Encoding.UTF8);
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

        private static void EsportaAndroid(string nomeFile, VersioneInformazioni info, IProgress<double>? progress = null)
        {
            if ((info.Tipo & TestoTipi.Bibbia) == TestoTipi.Bibbia)
            {
                EsportaAndroidBibbia(nomeFile, info, progress);
            }
            else
            {
                EsportaAndroidCollezione(nomeFile, info, progress);
            }
        }

        private static void EsportaAndroidCollezione(string nomeFile, VersioneInformazioni info, IProgress<double>? progress = null)
        {
            if (MainWindow.Testi.CollezioneModificata(info.Nome))
            {
                string messaggio = (string)(Application.Current.TryFindResource("BibliotecaEsportaNoteModificate") ?? "The text '{0}' has been modified. Please close and reopen the program to save the changes before exporting.");
                messaggio = string.Format(messaggio, info.Nome);
                MessageBoxLPN.Show(Application.Current.MainWindow, messaggio, (string)(Application.Current.TryFindResource("Errore") ?? "Error"));
                return;
            }
            string testo;
            string percorsoFile = nomeFile.ToLowerInvariant().Replace(' ', '_');
            using FileStream fs = new(percorsoFile, FileMode.Create, FileAccess.Write, FileShare.None);
            using BinaryWriter wc = new(fs);

            long pInizioDati = ScriviDatiJava(info, wc, false);
            wc.Write(Intabyte4(0)); // inizio riferimenti citati
            wc.Write(Intabyte4(0)); // inizio note in ordine

            Collection<string> notec = MainWindow.Testi.Note(info.Nome);
            StringBuilder titoli = new();
            int numeroNote = notec.Count;
            long[] posizioniNote = new long[numeroNote];
            int nNota = 0;

            foreach (string nota in notec)
            {
                posizioniNote[nNota] = wc.Seek(0, SeekOrigin.Current);
                ++nNota;
                testo = MainWindow.Testi.GetNotaTesto(nota, info.Nome);
                //                            if (testo.Contains("location"))
                //                              nNota = nNota + 0;
                testo = testo.Replace("\r\n", " ").Trim();
                if (testo.Contains("deflang", StringComparison.CurrentCulture))
                    testo = testo[testo.IndexOf('\\', testo.IndexOf("deflang"))..];
                //                            if (testo.IndexOf("lang1040") >= 0)
                //                                testo = testo.Substring(testo.IndexOf("\\", testo.IndexOf("lang1040")));
                //                            if (testo.IndexOf("lang3081") >= 0)
                //                                testo = testo.Substring(testo.IndexOf("\\", testo.IndexOf("lang3081")));
                if (testo.Contains("\\fonttbl{", StringComparison.CurrentCulture))
                    testo = testo[(testo.IndexOf("}}", testo.IndexOf("\\fonttbl{")) + 2)..];
                if (testo.Contains("\\colortbl", StringComparison.CurrentCulture))
                    testo = testo[(testo.IndexOf('}', testo.IndexOf("\\colortbl")) + 1)..];
                if (testo.Contains("viewkind4", StringComparison.CurrentCulture))
                    testo = testo[testo.IndexOf('\\', testo.IndexOf("viewkind4"))..];
                if (testo.StartsWith("{\\rtf1"))
                    testo = testo[6..];
                if (testo.StartsWith("\\uc1"))
                    testo = testo[4..];
                if (testo.StartsWith("\\pard"))
                    testo = testo[5..];
                if (testo.EndsWith('\0'))
                    testo = testo[..^1];
                if (testo.EndsWith('}'))
                    testo = testo[..^1];
                if (testo.EndsWith("\\par "))
                    testo = testo[..^5];
                if (testo.EndsWith("\\par"))
                    testo = testo[..^4];
                testo = ConvAHTMLJava(testo, info.Nome);
                wc.Write(testo.ToCharArray());
                wc.Write((char)0);
                titoli.Append(nota).Append('|');
                progress?.Report((nNota / (double)numeroNote) * 100.0);
            }

            long inizioTestoIndiceLCc = wc.Seek(0, SeekOrigin.Current);
            wc.Write(Encoding.Convert(Encoding.UTF8, Encoding.BigEndianUnicode, Encoding.UTF8.GetBytes(titoli.ToString().Replace("_", "-"))));
            wc.Write((char)0);
            long inizioTestoIndicec = wc.Seek(0, SeekOrigin.Current);
            for (int i = 0; i < numeroNote; ++i)
                wc.Write(Intabyte4(posizioniNote[i] - posizioniNote[0]));

            long inizioParolec = wc.Seek(0, SeekOrigin.Current);
            string[] parolec = MainWindow.Testi.Parole(info.Nome);
            wc.Write(Encoding.Convert(Encoding.UTF8, Encoding.BigEndianUnicode, Encoding.UTF8.GetBytes(string.Join("|", parolec))));

            long inizioParoleIndiceIndicec = wc.Seek(0, SeekOrigin.Current);
            int nParolec = parolec.Length;
            int numeroApparenzec = 0;
            byte[] datiDaScriverec = new byte[4 * nParolec + 4];
            MemoryStream msc = new(datiDaScriverec, true);
            BinaryWriter bwMemoriac = new(msc);
            bwMemoriac.Write(Intabyte4(0));
            for (int i = 0; i < nParolec; ++i)
            {
                numeroApparenzec += MainWindow.Testi.NumeroVolteParola(parolec[i], info.Nome);
                bwMemoriac.Write(Intabyte4(6 * numeroApparenzec));
            }
            bwMemoriac.Seek(0, SeekOrigin.Begin);
            wc.Write(datiDaScriverec);

            long inizioParoleIndicec = wc.Seek(0, SeekOrigin.Current);
            wc.Write(MainWindow.Testi.GetApparenzeParole(info.Nome));

            long inizioRadicic = wc.Seek(0, SeekOrigin.Current);
            string[] radicic = MainWindow.Testi.Radici(info.Nome);
            wc.Write(Encoding.Convert(Encoding.UTF8, Encoding.BigEndianUnicode, Encoding.UTF8.GetBytes(string.Join("|", radicic))));
            long inizioRadiciDiParolec = wc.Seek(0, SeekOrigin.Current);
            for (int i = 0; i < nParolec; ++i)
                wc.Write(Intabyte4(Array.BinarySearch(radicic, MainWindow.Testi.RadiceDiParola(parolec[i], info.Nome), new ConfrontoCI())));

            long inizioRadiciDiversec = wc.Seek(0, SeekOrigin.Current);
            List<string> radDiversec = [.. MainWindow.Testi.GetRadiciDiverse(info.Nome)];
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
            List<string> rifDiversec = [.. MainWindow.Testi.GetRiferimentiDiversi(info.Nome)];
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
            List<string> rifCitatic = [.. MainWindow.Testi.GetRiferimentiCitati(info.Nome)];
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
            List<string> noteInOrdinec = [.. MainWindow.Testi.GetNoteInOrdine(info.Nome)];
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
        }

        private static void EsportaAndroidBibbia(string nomeFile, VersioneInformazioni info, IProgress<double>? progress = null)
        {
            string percorsoFile = nomeFile.ToLowerInvariant().Replace(' ', '_');
            using FileStream fs = new(percorsoFile, FileMode.Create, FileAccess.Write, FileShare.None);
            using BinaryWriter w = new(fs);

            int capitoliInLibro, versettiInCapitolo;
            string testo;
            long pInizioDati = ScriviDatiJava(info, w, true);

            int nVersetto = 0;
            List<long> indiceVersetti = [];

            for (byte iLibro = 1; iLibro <= 73; ++iLibro)
            {
                capitoliInLibro = MainWindow.Testi.CapitoliInLibro(iLibro, info.Nome);
                for (byte iCapitolo = 1; iCapitolo <= capitoliInLibro; ++iCapitolo)
                {
                    versettiInCapitolo = MainWindow.Testi.VersettiInCapitolo(iLibro, iCapitolo, info.Nome);
                    for (byte iVersetto = 1; iVersetto <= versettiInCapitolo; ++iVersetto)
                    {
                        testo = MainWindow.Testi.TestoVersettoRaw(iLibro, iCapitolo, iVersetto, info.Nome);
                        testo = ConvAHTMLJava(testo, info.Nome);
                        //if (!string.IsNullOrEmpty(testo))
                        //{
                        indiceVersetti.Add(w.Seek(0, SeekOrigin.Current));
                        nVersetto++;
                        w.Write(testo.ToCharArray());
                        w.Write((char)0);
                        //}
                    }
                }
                progress?.Report((iLibro / 73.0) * 100.0);
            }

            UInt32 inizioTestoIndiceLC = (UInt32)(w.Seek(0, SeekOrigin.Current));
            for (byte iLibro = 1; iLibro <= 73; ++iLibro)
                w.Write(MainWindow.Testi.CapitoliInLibro(iLibro, info.Nome));
            byte v;
            for (byte iLibro = 1; iLibro <= 73; ++iLibro)
            {
                for (byte iCapitolo = 1; iCapitolo <= MainWindow.Testi.CapitoliInLibro(iLibro, info.Nome); ++iCapitolo)
                {
                    v = MainWindow.Testi.VersettiInCapitolo(iLibro, iCapitolo, info.Nome);
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
            string[] parole = MainWindow.Testi.Parole(info.Nome);
            w.Write(Encoding.BigEndianUnicode.GetBytes(string.Join("|", parole)));

            long inizioParoleIndiceIndice = w.Seek(0, SeekOrigin.Current);
            int nParole = parole.Length;
            int numeroApparenze = 0;
            byte[] datiDaScrivere = new byte[4 * nParole + 4];
            MemoryStream ms = new(datiDaScrivere, true);
            BinaryWriter bwMemoria = new(ms);
            bwMemoria.Write((UInt32)0);
            for (int i = 0; i < nParole; ++i)
            {
                numeroApparenze += MainWindow.Testi.NumeroVolteParola(parole[i], info.Nome);
                bwMemoria.Write(Intabyte4(6 * numeroApparenze));
            }
            bwMemoria.Seek(0, SeekOrigin.Begin);
            w.Write(datiDaScrivere);

            long inizioParoleIndice = w.Seek(0, SeekOrigin.Current);
            w.Write(MainWindow.Testi.GetApparenzeParole(info.Nome));

            long inizioRadici = w.Seek(0, SeekOrigin.Current);
            string[] radici = MainWindow.Testi.Radici(info.Nome);
            w.Write(Encoding.BigEndianUnicode.GetBytes(string.Join("|", radici)));
            long inizioRadiciDiParole = w.Seek(0, SeekOrigin.Current);
            for (int i = 0; i < nParole; ++i)
                w.Write(Intabyte4(Array.BinarySearch(radici, MainWindow.Testi.RadiceDiParola(parole[i], info.Nome), new ConfrontoCI())));

            long inizioRadiciDiverse = w.Seek(0, SeekOrigin.Current);
            List<string> radDiverse = [.. MainWindow.Testi.GetRadiciDiverse(info.Nome)];
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
            List<string> rifDiverse = [.. MainWindow.Testi.GetRiferimentiDiversi(info.Nome)];
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
        }

        private static long ScriviDatiJava(VersioneInformazioni info, BinaryWriter w, bool bibbia)
        {
            w.Write(['L', 'P', 'N', (char)1, (char)1, (char)22]);
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
                RichTextBoxEx rtDesc = new()
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

            long pInizioDati = w.BaseStream.Position;
            w.BaseStream.Position = pInizioVersione;
            w.Write(Intabyte4(pInizioDati));
            w.BaseStream.Position = w.BaseStream.Length;

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

        private static byte[] Intabyte4(long p)
        {
            byte[] b = new byte[4];
            BinaryPrimitives.WriteUInt32BigEndian(b, (uint)p);
            return b;
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
                    mezzoTag = testo.IndexOf(' ', inizioTag);
                    testo = testo.Insert(testo.IndexOf('>', inizioTag), "</span").Insert(mezzoTag + 1, "<span class=\"m\">").Insert(mezzoTag, "</span>").Insert(inizioTag + 5, "<span class=\"r\">").Remove(inizioTag, 5);
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

            StringBuilder link = new();
            Riferimento rif;
            testo = testo.Replace(@"\v\'03\'05#260000000000-260000000000\'04\v0 ", "").Replace(@"\b\v\'03\'05#590010030000-590010030000\'04\b0\v0 ", "").Replace(@"\v\f1\'02\'03\'05#560010220000-560010220000\'04\v0\f2", "");
            testo = testo.Replace(@"\v\f0\", @"\v\").Replace(@"\v\f1\", @"\v\").Replace(@"\v\f2\", @"\v\").Replace(@"\v\f3\", @"\v\").Replace(@"\v\f4\", @"\v\");
            testo = testo.Replace(@"\v\fs24", @"\v").Replace(@"\v\fs27", @"\v").Replace(@"\v\fs32", @"\v").Replace(@"\v\fs36", @"\v");
            testo = testo.Replace(@"\v\'02\i0", @"\i0\v\'02").Replace(@"\v\'02\i", @"\i\v\'02").Replace(@"\v\'02\cf0", @"\cf0\v\'02");
            testo = testo.Replace(@"\v\'02\b\i0", @"\b\i0\v\'02").Replace(@"\v\'02\b", @"\b\v\'02").Replace(@"\b0\v0", @"\v0\b0").Replace(@"\v0\f0", @"\v0").Replace(@"\v0\f1", @"\v0").Replace(@"\v0\f2", @"\v0").Replace(@"\v0\f3", @"\v0").Replace(@"\v0\fs24", @"\v0");
            testo = testo.Replace(@"\'02", RichTextBoxEx.InizioLink.ToString()).Replace(@"\'03", " " + RichTextBoxEx.FineLink1).Replace(@"\'04", RichTextBoxEx.FineLink1.ToString()).Replace(@"\'05", RichTextBoxEx.FineLinkBrano.ToString()).Replace(@"\'06", RichTextBoxEx.FineLinkNota.ToString()).Replace(@"\'07", RichTextBoxEx.FineLinkFile.ToString());
            testo = testo.Replace(@"\v" + RichTextBoxEx.InizioLink, @"\v " + RichTextBoxEx.InizioLink);
            String linkDaCercare = @"\v " + RichTextBoxEx.InizioLink + " " + RichTextBoxEx.FineLink1;
            while (testo.Contains(linkDaCercare, StringComparison.CurrentCulture))
            {
                testo = string.Concat(testo.AsSpan(0, testo.IndexOf(linkDaCercare) - 1), testo.AsSpan(testo.IndexOf(@"\v0", testo.IndexOf(linkDaCercare)) + 4));
            }

            linkDaCercare = @"\v " + RichTextBoxEx.InizioLink + @"\v0 ";
            while (testo.Contains(linkDaCercare, StringComparison.OrdinalIgnoreCase))
            {
                inizioTag = testo.IndexOf(linkDaCercare, StringComparison.OrdinalIgnoreCase);
                mezzoTag = testo.IndexOf("\\v", inizioTag + 5, StringComparison.OrdinalIgnoreCase);
                fineTag = testo.IndexOf("\\v0", mezzoTag, StringComparison.OrdinalIgnoreCase);
                switch (testo[mezzoTag + 4])
                {
                    case RichTextBoxEx.FineLinkBrano:
                        // laparola:1 1 1 1 1 2@*bibbia oppure laparola:1 1 1 1 1 2@Nuova Riveduta
                        rif = MainWindow.Testi.ConvertiRiferimento(MainWindow.Testi.ConvertiTitoloNotaARiferimento(testo.Substring(mezzoTag + 5, fineTag - mezzoTag - 6)));
                        link.Length = 0;
                        foreach (byte[] brano in rif.Brani)
                            link.Append(brano[0]).Append(' ').Append(brano[1]).Append(' ').Append(brano[2]).Append(' ').Append(brano[3]).Append(' ').Append(brano[4]).Append(' ').Append(brano[5]).Append(';');
                        if (link.Length > 0)
                            link.Remove(link.Length - 1, 1);
                        testo = testo[..inizioTag] + "<a href=\"laparola:" + link.ToString() + "@*bibbia\">" + testo.Substring(inizioTag + 8, mezzoTag - inizioTag - 8) + "</a>" + testo[(fineTag + 3)..];
                        break;
                    case RichTextBoxEx.FineLinkNota:
                        if (testo[mezzoTag + 5] == '#')
                        {
                            // nota: usato solo in Aiuto Biblico in Apoc 3:14-22
                            testo = testo[..inizioTag] + "<a href=\"laparola:" + MainWindow.Testi.ConvertiTitoloNotaARiferimento(testo.Substring(mezzoTag + 5, fineTag - mezzoTag - 6)) + "\">" + testo.Substring(inizioTag + 8, mezzoTag - inizioTag - 8) + "</a>" + testo[(fineTag + 3)..];
                            // forse il seguente modo è più preciso, ma forse richiede un cambio nel programma, e di aggiungere @nomeVersione alla fine
                            // laparola:1 1 1 1 1 2
                            /*
                            rif = MainWindow.Testi.ConvertiRiferimento(MainWindow.Testi.ConvertiTitoloNotaARiferimento(testo.Substring(mezzoTag + 5, fineTag - mezzoTag - 6)));
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
                            String dollaro = (MainWindow.Testi.GetNumeroNotaTitolo(titoloNota, nomeVersione) < 0 ? "" : "$");
                            testo = testo[..inizioTag] + "<a href=\"laparola:" + dollaro + titoloNota.Replace("_", "-") + "\">" + testo.Substring(inizioTag + 8, mezzoTag - inizioTag - 8) + "</a>" + testo[(fineTag + 3)..];
                        }
                        break;
                    case RichTextBoxEx.FineLinkFile:
                        testo = string.Concat(testo.AsSpan(0, inizioTag), testo.AsSpan(mezzoTag + 5, fineTag - mezzoTag - 6), testo.AsSpan(fineTag + 3));
                        break;
                    default:
                        break;
                }
            }

            linkDaCercare = @"\v " + RichTextBoxEx.FineLink1 + RichTextBoxEx.FineLinkBrano;
            while (testo.Contains(linkDaCercare, StringComparison.CurrentCulture))
            {
                testo = string.Concat(testo.AsSpan(0, testo.IndexOf(linkDaCercare)), testo.AsSpan(testo.IndexOf(@"\v0", testo.IndexOf(linkDaCercare)) + 3));
            }

            testo = SostituisciTagSingolo(testo, "pard", "");
            testo = SostituisciTagSingolo(testo, "par", "<br />", false);

            while (testo.Contains(@" {\super ", StringComparison.OrdinalIgnoreCase))
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
            while (testo.Contains(@"\ul ", StringComparison.OrdinalIgnoreCase))
            {
                iUL = testo.IndexOf(@"\ul ", StringComparison.OrdinalIgnoreCase);
                //                if (testo.Substring(iUL, 10) == "\\ul Quelli")
                //                    iUL = iUL + 1 - 1;
                iPlain = testo.IndexOf(@"\plain", iUL);
                iUL0 = testo.IndexOf(@"\ul0", iUL);
                iULnone = testo.IndexOf(@"\ulnone", iUL);
                if (iPlain > 0 && (iPlain < iUL0 || iUL0 < 0) && (iPlain < iULnone || iULnone < 0))
                    testo = testo[..iUL] + "<span style=\"text-decoration:underline;\">" + testo.Substring(iUL + 4, iPlain - iUL - 4) + "</span>" + testo[iPlain..];
                else if (iUL0 > 0 && (iUL0 < iULnone || iULnone < 0))
                    testo = testo[..iUL] + "<span style=\"text-decoration:underline;\">" + testo.Substring(iUL + 4, iUL0 - iUL - 4) + "</span>" + testo[(iUL0 + 5)..];
                else if (iULnone > 0)
                    testo = testo[..iUL] + "<span style=\"text-decoration:underline;\">" + testo.Substring(iUL + 4, iULnone - iUL - 4) + "</span>" + testo[(iULnone + 7)..];
                else
                    testo = string.Concat(testo.AsSpan(0, iUL), testo.AsSpan(iUL + 4));
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
                        testo = testo[..iUL] + "<span style=\"text-decoration:underline;\">" + testo.Substring(iUL + 3, iPlain - iUL - 3) + "</span>" + testo[iPlain..];
                    else if (iUL0 > 0 && (iUL0 < iULnone || iULnone < 0))
                        testo = testo[..iUL] + "<span style=\"text-decoration:underline;\">" + testo.Substring(iUL + 3, iUL0 - iUL - 3) + "</span>" + testo[(iUL0 + 5)..];
                    else if (iULnone > 0)
                        testo = testo[..iUL] + "<span style=\"text-decoration:underline;\">" + testo.Substring(iUL + 3, iULnone - iUL - 3) + "</span>" + testo[(iULnone + 7)..];
                    else
                        testo = string.Concat(testo.AsSpan(0, iUL), testo.AsSpan(iUL + 3));
                }
            }
            bool inserisciI, inserisciB;
            int iI, iI1, iB, iB1;
            while (testo.IndexOf(@"\plain") > -1)
            {
                inserisciB = false;
                inserisciI = false;
                iPlain = testo.IndexOf(@"\plain");
                iI = testo[..iPlain].LastIndexOf(@"\i ");
                iI1 = testo[..iPlain].LastIndexOf(@"\i<");
                if (iI1 > iI) iI = iI1;
                if (iI > -1 && testo[..iPlain].LastIndexOf(@"\i0") < iI)
                    inserisciI = true;
                iB = testo[..iPlain].LastIndexOf(@"\b ");
                iB1 = testo[..iPlain].LastIndexOf(@"\b<");
                if (iB1 > iB) iB = iB1;
                if (iB > -1 && testo[..iPlain].LastIndexOf(@"\b0") < iB)
                    inserisciB = true;
                testo = string.Concat(testo.AsSpan(0, iPlain), inserisciB ? "\\b0" : "", inserisciI ? "\\i0" : "", testo.AsSpan(iPlain + 6)); // \i è rimosso più avanti
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
                testo = testo[..^6];
            if (testo.EndsWith("\\f6"))
                testo = testo[..^3];
            if (testo.EndsWith("\\i"))
                testo = testo[..^2];
            if (testo.Contains("</td>"))
                testo = string.Concat(testo.AsSpan(0, testo.IndexOf("</td>")), "<table><tr>", testo.AsSpan(testo.IndexOf("</td>") + 5));
            if (testo.Contains("<tr>"))
                testo = string.Concat(testo.AsSpan(0, testo.LastIndexOf("<tr>")), "</table>", testo.AsSpan(testo.LastIndexOf("<tr>") + 4));
            testo = testo.Replace("<td></tr>", "</tr>");
            testo = testo.Replace("</tr><tr>", "</tr><tr><td>");
            testo = testo.Trim();
            if (testo.LastIndexOf("<i>") > testo.LastIndexOf("</i>"))
                testo += "</i>";
            //if (testo.Contains('\\', StringComparison.OrdinalIgnoreCase))
            //    inizioTag = 0;
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
            while (testo.Contains(tag2, StringComparison.CurrentCulture))
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
            while (testo.Contains(@"{\" + tag + " ", StringComparison.OrdinalIgnoreCase))
            {
                inizioTag = testo.IndexOf(@"{\" + tag + " ", StringComparison.OrdinalIgnoreCase);
                fineTag = testo.IndexOf("}", inizioTag, StringComparison.OrdinalIgnoreCase);
                testo = testo[..inizioTag] + tagNuovo1 + testo.Substring(inizioTag + tag.Length + 3, fineTag - inizioTag - tag.Length - 3) + tagNuovo2 + testo[(fineTag + 1)..];
            }
            return testo;
        }

        private static string ConvAHTMLCharEntity(string testo)
        {
            for (int iLettera = testo.Length - 1; iLettera >= 0; --iLettera)
            {
                if (testo[iLettera] > (char)127)
                    testo = testo[..iLettera] + "&#" + Convert.ToUInt32(testo[iLettera]) + ";" + testo[(iLettera + 1)..];
            }
            int instr, instr2;
            while (testo.IndexOf(@"\'") > -1)
            {
                instr = testo.IndexOf(@"\'");
                testo = testo[..instr] + "&#" + (Uri.FromHex(testo[instr + 2]) * 16 + Uri.FromHex(testo[instr + 3])) + ";" + testo[(instr + 4)..];
            }
            while (testo.IndexOf(@"\u") > -1)
            {
                instr = testo.IndexOf(@"\u");
                instr2 = testo.IndexOf('?', instr);
                if (instr2 > 0)
                    testo = testo[..instr] + "&#" + testo.Substring(instr + 2, instr2 - instr - 2) + ";" + testo[(instr2 + 1)..];
                else
                    testo = testo[..instr] + "&#" + testo.Substring(instr + 2, 4) + ";" + testo[(instr + 7)..];
            }

            // per ebraico traslitterato, dove il font per Android manca dei caratteri
            testo = testo.Replace("&#702;", "&#1158;").Replace("&#703;", "&#1157;").Replace("&#7829;", "&#382;").Replace("&#7830;", "&#295;");

            return testo;
        }

        private static async Task EsportaFileMultipli(string cartella, VersioneInformazioni info, IProgress<double>? progress = null)
        {
            if ((info.Tipo & TestoTipi.Bibbia) == TestoTipi.Bibbia)
            {
                await EsportaFileMultipliBibbia(cartella, info, progress);
            }
            else
            {
                await EsportaFileMultipliCollezione(cartella, info, progress);
            }
        }

        private static async Task EsportaFileMultipliBibbia(string cartella, VersioneInformazioni info, IProgress<double>? progress = null)
        {
            List<string> righe = new(3000);
            RichTextBoxEx rtb = new();
            string testo;
            int capitoliInLibro, inizioVersetto;
            Collection<string> listaCollezioni = [];

            FormatoTesto formatoVecchio = MainWindow.Testi.Formato;
            FormatoTesto formatoPerEsporto = new();
            formatoVecchio.CopiaA(formatoPerEsporto);
            formatoPerEsporto.RiferimentoFormato = RiferimentoFormato.Nessuno;
            formatoPerEsporto.TestoVisualizzato = TestoVisualizzato.Versetti;
            MainWindow.Testi.Formato = formatoPerEsporto;

            for (byte iLibro = 1; iLibro <= 73; ++iLibro)
            {
                righe.Clear();
                capitoliInLibro = MainWindow.Testi.CapitoliInLibro(iLibro, info.Nome);
                for (byte iCapitolo = 1; iCapitolo <= capitoliInLibro; ++iCapitolo)
                {
                    string rtfText = await MainWindow.Testi.TestoBranoAsync(new Riferimento([iLibro, iCapitolo, 1, iLibro, iCapitolo, 255]), info.Nome, listaCollezioni);
                    rtb.Rtf = rtfText;
                    testo = rtb.Text.Trim();
                    if (testo.StartsWith("LPN_ANCORA_"))
                        testo = testo[19..];
                    while (testo.Contains("\r\nLPN_ANCORA_"))
                    {
                        inizioVersetto = testo.IndexOf("\r\nLPN_ANCORA_");
                        testo = string.Concat(testo.AsSpan(0, inizioVersetto), "LPNq$/d3", testo.AsSpan(inizioVersetto + 21));
                    }
                    testo = testo.Replace("\r\n", "|");
                    testo = testo.Replace("LPNq$/d3", "\r\n");
                    righe.Add(testo.Replace("\r\n\r\n", "\r\n.\r\n")); // versetto mancante
                    righe.Add(""); // riga vuota fra capitoli
                }
                if (righe.Count > 0)
                {
                    righe.RemoveAt(righe.Count - 1); // la riga addizionale dopo l'ultimo capitolo non serve
                    await File.WriteAllLinesAsync(Path.Combine(cartella, MainWindow.Testi.GetLibroNome(iLibro) + ".txt"), [.. righe], Encoding.UTF8);
                }
                progress?.Report((iLibro / 73.0) * 100.0);
            }

            MainWindow.Testi.Formato = formatoVecchio;
        }

        private static async Task EsportaFileMultipliCollezione(string cartella, VersioneInformazioni info, IProgress<double>? progress = null)
        {
            Collection<string> note = MainWindow.Testi.Note(info.Nome);
            double numeroNote = (double)note.Count;
            int nNota = 0;
            foreach (string nota in note)
            {
                await File.WriteAllTextAsync(Path.Combine(cartella , nota.Replace("?", "").Replace(":", "-").Replace("\"", "'") + ".rtf"), MainWindow.Testi.GetNotaTesto(nota, info.Nome));
                ++nNota;
                progress?.Report((nNota / numeroNote) * 100.0);
            }
            List<string> noteInOrdine = [.. MainWindow.Testi.GetNoteInOrdine(info.Nome)];
            for (int i = 0; i < noteInOrdine.Count; ++i)
                noteInOrdine[i] = noteInOrdine[i].Replace("?", "").Replace(":", "-");
            if (noteInOrdine.Count > 0)
                await File.WriteAllLinesAsync(Path.Combine(cartella, info.Nome + ".ordine"), [.. noteInOrdine], Encoding.UTF8);
        }
    }
}