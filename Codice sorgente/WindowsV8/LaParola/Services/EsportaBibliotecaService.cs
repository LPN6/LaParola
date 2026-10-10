using LaParola.Utilities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security;
using System.Text;
using System.Windows;

namespace LaParola.Services
{
    internal class EsportaBibliotecaService
    {
        internal static async Task<bool> EsportaBiblioteca(string nomeFile, string informazioni, int ordine, int formato)
        {
            List<int> listaInformazioni = informazioni?
                .Split(['|'], StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s, out int idx) ? (int?)idx : null)
                .OfType<int>()
                .ToList() ?? [];

            string[] titoliInformazioni = [
                GetResource("BibliotecaNome", "Name"),
            GetResource("BibliotecaAbbreviazione", "Abbreviation"),
            GetResource("BibliotecaTipo", "Type"),
            GetResource("BibliotecaAutore", "Author"),
            GetResource("BibliotecaData", "Date"),
            GetResource("BibliotecaLingua", "Language"),
            GetResource("BibliotecaCasaEditrice", "Publisher"),
            GetResource("BibliotecaISBN", "ISBN"),
            GetResource("BibliotecaCopyright", "Copyright"),
            GetResource("BibliotecaDescrizione", "Description"),
            GetResource("BibliotecaNomeDelFile", "File Name"),
            GetResource("BibliotecaVersione", "Version")
            ];

            string bibbia = GetResource("BibliotecaFiltroTipoBibbia", "Bible");
            string commentario = GetResource("BibliotecaFiltroTipoCommentario", "Commentary");
            string dizionario = GetResource("BibliotecaFiltroTipoDizionario", "Dictionary");
            string libro = GetResource("BibliotecaFiltroTipoLibro", "Book");

            string inizioRiga, fineRiga, inizioColonna, fineColonna;
            StringBuilder testoFile = new();

            switch (formato)
            {
                case 1: // RTF
                    inizioRiga = "";
                    fineRiga = "\\par\r\n";
                    inizioColonna = "{\\b $$$:} ";
                    fineColonna = "\\par\r\n";
                    testoFile.Append(MainWindow.Testi.RtfIntestazione());
                    break;
                case 2: // CSV
                    inizioRiga = "";
                    fineRiga = "\r\n";
                    inizioColonna = "";
                    fineColonna = ";";
                    break;
                case 3: // HTML
                    inizioRiga = "<tr>\r\n";
                    fineRiga = "</tr>\r\n";
                    inizioColonna = "  <td>";
                    fineColonna = "</td>\r\n";
                    testoFile.Append("<html>\r\n<head>\r\n<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />\r\n</head>\r\n<body>\r\n<table>\r\n");
                    testoFile.Append("<tr>\r\n  <th></th>\r\n");
                    foreach (int i in listaInformazioni)
                        testoFile.Append("  <th>").Append(WebUtility.HtmlEncode(titoliInformazioni[i])).Append("</th>\r\n");
                    testoFile.Append("</tr>\r\n");
                    break;
                case 4: // XML
                    inizioRiga = "<text>\r\n";
                    fineRiga = "</text>\r\n";
                    inizioColonna = "  <$$$>";
                    fineColonna = "</$$$>\r\n";
                    testoFile.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n<texts>\r\n");
                    break;
                default: // Plain text (0)
                    inizioRiga = "";
                    fineRiga = "----------\r\n";
                    inizioColonna = "$$$: ";
                    fineColonna = "\r\n";
                    break;
            }

            // Order versions using LINQ (prevents key collision crashes)
            Collection<string> tuttiTesti = MainWindow.Testi.NomiVersioni();
            IEnumerable<string> versioniInOrdine = tuttiTesti
                .Select(v => new { Versione = v, Chiave = GetChiaveOrdine(v, ordine) })
                .OrderBy(x => x.Chiave, StringComparer.Ordinal)
                .ThenBy(x => x.Versione, StringComparer.Ordinal)
                .Select(x => x.Versione);

            foreach (string versione in versioniInOrdine)
            {
                testoFile.Append(inizioRiga);

                if (formato == 1) // RTF
                    testoFile.Append(@"{\qc");

                string stringaTemp = GetResource("EsportaBibliotecaTesto", "Text");
                if (formato == 4) // XML
                    stringaTemp = stringaTemp.Replace(" ", "");

                testoFile.Append(inizioColonna.Replace("$$$", stringaTemp))
                         .Append(versione)
                         .Append(fineColonna.Replace("$$$", stringaTemp));

                if (formato == 1) // RTF
                    testoFile.Append('}');

                VersioneInformazioni infoData = MainWindow.Testi.Info(versione);

                foreach (int i in listaInformazioni)
                {
                    string info = i switch
                    {
                        0 => infoData.Titolo,
                        1 => infoData.Abbreviazione,
                        2 => FormattaTipi(infoData.Tipo, bibbia, commentario, dizionario, libro),
                        3 => infoData.Autore,
                        4 => infoData.Data,
                        5 => infoData.Lingua,
                        6 => infoData.CasaEditrice,
                        7 => infoData.Isbn,
                        8 => infoData.Copyright,
                        9 => FormattaDescrizione(infoData.Descrizione, formato),
                        10 => formato == 1 ? infoData.NomeDelFile.Replace(@"\", @"\\") : infoData.NomeDelFile,
                        11 => VersioneMinore2Cifre(infoData.Versione),
                        _ => string.Empty
                    };

                    if (string.IsNullOrEmpty(info) && (formato == 0 || formato == 1 || formato == 4))
                        continue;

                    // Format encoding per file type
                    if (formato == 2) // CSV
                    {
                        info = info.Replace("\"", "\"\"").Replace("\r\n", " ").Replace("\n", " ");
                        if (info.Contains(';') || info.Contains("\"\""))
                            info = $"\"{info}\"";
                    }
                    else if (formato == 3) // HTML
                    {
                        info = WebUtility.HtmlEncode(info);
                    }
                    else if (formato == 4) // XML
                    {
                        info = SecurityElement.Escape(info) ?? string.Empty;
                    }

                    string headerColonna = titoliInformazioni[i];
                    if (formato == 4) // XML
                        headerColonna = headerColonna.Replace(" ", "");

                    testoFile.Append(inizioColonna.Replace("$$$", headerColonna))
                             .Append(info)
                             .Append(fineColonna.Replace("$$$", headerColonna));
                }

                testoFile.Append(fineRiga);
            }

            // Close root tags
            switch (formato)
            {
                case 1:
                    testoFile.Append('}');
                    break;
                case 3:
                    testoFile.Append("</table>\r\n</body>\r\n</html>");
                    break;
                case 4:
                    testoFile.Append("</texts>");
                    break;
            }

            string contenutoFinale = testoFile.ToString();

            // Encode RTF special characters safely and quickly
            if (formato == 1) // RTF
            {
                contenutoFinale = EncodeRtfText(contenutoFinale);
            }

            // Async write to disk
            Encoding encoding = (formato == 1) ? Encoding.ASCII : Encoding.UTF8; // ==1 è RTF
            await File.WriteAllTextAsync(nomeFile, contenutoFinale, encoding);

            // Open exported file
            switch (formato)
            {
                case 2:
                case 3:
                case 4:
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = nomeFile,
                            UseShellExecute = true // Fixes Process.Start in modern .NET
                        });
                    }
                    catch (Win32Exception)
                    {
                        // Ignore if no associated application exists or default viewer fails
                    }
                    break;
                case 1:
                default: // include 0
                    App.DockingHost.OpenFileInEditor(nomeFile);
                    break;
            }

            return true;
        }

        private static string GetChiaveOrdine(string versione, int ordine)
        {
            return ordine switch
            {
                1 => TipoPerOrdine(versione),
                2 => LinguaPerOrdine(versione),
                3 => TipoPerOrdine(versione) + LinguaPerOrdine(versione),
                4 => LinguaPerOrdine(versione) + TipoPerOrdine(versione),
                _ => string.Empty
            };
        }

        private static string FormattaTipi(TestoTipi tipi, string bibbia, string commentario, string dizionario, string libro)
        {
            List<string> lista = new(4);
            if (tipi.HasFlag(TestoTipi.Bibbia)) lista.Add(bibbia);
            if (tipi.HasFlag(TestoTipi.Commentario)) lista.Add(commentario);
            if (tipi.HasFlag(TestoTipi.Dizionario)) lista.Add(dizionario);
            if (tipi.HasFlag(TestoTipi.Libro)) lista.Add(libro);

            return string.Join(", ", lista);
        }

        private static string FormattaDescrizione(string descrizione, int formato)
        {
            if (string.IsNullOrEmpty(descrizione)) return string.Empty;

            if (formato == 1) // RTF
            {
                return descrizione
                    .Replace(@"\", @"\\")
                    .Replace("{", @"\{")
                    .Replace("}", @"\}")
                    .Replace("\r\n", @"\par ")
                    .Replace("\n", @"\par ");
            }

            return descrizione;
        }

        // Single-pass fast RTF Unicode/Hex encoder (O(N) vs previous O(N^2))
        private static string EncodeRtfText(string input)
        {
            StringBuilder sb = new(input.Length * 2);
            foreach (char c in input)
            {
                if (c > 256)
                    sb.Append(@"\u").Append((ushort)c).Append('?');
                else if (c > 127)
                    sb.Append(@"\'").Append(((byte)c).ToString("x2", CultureInfo.InvariantCulture));
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }

        private static string GetResource(string key, string fallback)
        {
            return (string)(Application.Current.TryFindResource(key) ?? fallback);
        }

        private static string TipoPerOrdine(string versione)
        {
            int tipo = Convert.ToInt32(MainWindow.Testi.Info(versione).Tipo, CultureInfo.InvariantCulture);
            if (tipo == 4) tipo = 11; // questo mette i tipi di testo nell'ordine che per me è naturale
            if (tipo == 14) tipo = 10;
            return Funzioni.AggiungiZero(tipo, 2);
        }

        private static string LinguaPerOrdine(string versione)
        {
            string lingua = MainWindow.Testi.Info(versione).Lingua ?? string.Empty;
            return lingua.Length <= 8 ? lingua.PadRight(8, '!') : lingua;
        }

        internal static string VersioneMinore2Cifre(string v)
        {
            if (string.IsNullOrEmpty(v)) return string.Empty;

            int p = v.IndexOf('.');
            if (p > 0 && p < v.Length - 3 && v[p + 2] == '.')
                v = v.Insert(p + 1, "0");

            return v;
        }
    }
}