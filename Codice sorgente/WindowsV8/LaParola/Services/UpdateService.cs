using LaParola.Properties;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Xml.Linq;

namespace LaParola.Services
{
    public class TextUpdateInfo
    {
        public string Componente { get; set; } = "";
        public string Nome { get; set; } = "";
        public string Tipo { get; set; } = "";
        public string VersioneDisponibile { get; set; } = "";
        public string VersioneInstallata { get; set; } = "";
        public List<string> Urls { get; set; } = [];
        public string Dimensione { get; set; } = "";
        public string Lingua { get; set; } = "";
    }

    public static class UpdateService
    {
        public const string UrlXmlAggiornamenti = "https://www.laparola.net/programma/aggiorna8.xml";

        private static readonly HttpClient HttpClient = CreateConfiguredHttpClient();
        private static readonly Version? VersioneApp = Assembly.GetExecutingAssembly().GetName().Version;
        private static bool _promptShownThisSession = false;

        private static HttpClient CreateConfiguredHttpClient()
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Add("User-Agent", "LaParolaDesktop/7.0 (Windows)");
            client.Timeout = TimeSpan.FromSeconds(30);
            return client;
        }

        /// <summary>
        /// Downloads the XML feed and parses all valid text nodes.
        /// </summary>
        public static async Task<(List<XElement> Nodes, string RawXml)> ScaricaFeedXmlAsync()
        {
            string xmlContent = await HttpClient.GetStringAsync(UrlXmlAggiornamenti);
            XDocument doc = XDocument.Parse(xmlContent);
            List<XElement> nodes = doc.Root?.Elements("file").ToList() ?? [];
            return (nodes, xmlContent);
        }

        /// <summary>
        /// Checks if any currently installed texts have newer versions available in the XML feed.
        /// </summary>
        public static List<TextUpdateInfo> ControllaAggiornamentiTesti(IEnumerable<XElement> nodiFile)
        {
            List<TextUpdateInfo> aggiornamentiDisponibili = [];
            bool isItaliano = MainWindow.settings?.Lingua?.StartsWith("it", StringComparison.OrdinalIgnoreCase) ?? true;

            foreach (XElement f in nodiFile)
            {
                string componente = f.Element("componente")?.Value?.Trim() ?? "";
                if (string.IsNullOrEmpty(componente))
                    continue;

                // Check if text is installed and obtain installed version string
                if (MainWindow.Testi == null || !MainWindow.Testi.VersioneEsiste(componente))
                    continue;

                string versioneRemotaStr = f.Element("versione")?.Value?.Trim() ?? "";
                string versioneInstallataStr = MainWindow.Testi.Info(componente)?.Versione ?? "0.0";

                // Check app version compatibility first
                if (!IsVersioneCompatibile(versioneRemotaStr))
                    continue;

                // Compare versions
                if (IsNuovaVersioneDisponibile(versioneInstallataStr, versioneRemotaStr))
                {
                    string? nomeIt = f.Elements("nome").FirstOrDefault(x => (string?)x.Attribute("language") == "it")?.Value;
                    string? nomeDef = f.Elements("nome").FirstOrDefault(x => x.Attribute("language") == null)?.Value ?? f.Element("nome")?.Value;
                    string nome = (isItaliano && !string.IsNullOrEmpty(nomeIt)) ? nomeIt : (nomeDef ?? componente);

                    string tipo = f.Element("tipo")?.Value?.Trim() ?? "";
                    if (tipo == "note") tipo = "commentario";

                    List<string> urls = [.. f.Elements("url")
                        .Select(x => x.Value.Trim())
                        .Where(x => !string.IsNullOrEmpty(x))];

                    aggiornamentiDisponibili.Add(new TextUpdateInfo
                    {
                        Componente = componente,
                        Nome = nome,
                        Tipo = tipo,
                        VersioneDisponibile = versioneRemotaStr,
                        VersioneInstallata = versioneInstallataStr,
                        Urls = urls,
                        Dimensione = f.Element("dimensione")?.Value ?? "",
                        Lingua = f.Element("lingua")?.Value ?? ""
                    });
                }
            }

            return aggiornamentiDisponibili;
        }

        /// <summary>
        /// Runs a background check and opens a non-modal update prompt if updates exist.
        /// </summary>
        public static async Task VerificaEMostraNotificaAggiornamentiAsync(bool forzato = true)
        {
            if (_promptShownThisSession)
                return;

            DateTime ultimoControllo = MainWindow.settings?.UltimoControlloAggiornamentiTesti ?? DateTime.MinValue;
            int intervalloGiorni = MainWindow.settings?.IntervalloGiorniControlloTesti ?? 7;

            // se non forzato e meno di 'intervalloGiorni' sono passati, salta il controllo
            if (!forzato && (DateTime.Now - ultimoControllo).TotalDays < intervalloGiorni)
            {
                return;
            }

            try
            {
                (List<XElement> nodes, string _) = await ScaricaFeedXmlAsync();
                // Update timestamp on successful fetch
                MainWindow.settings?.UltimoControlloAggiornamentiTesti = DateTime.Now;

                List<TextUpdateInfo> aggiornamenti = ControllaAggiornamentiTesti(nodes);

                if (aggiornamenti.Count > 0)
                {
                    _promptShownThisSession = true;

                    // Execute on UI thread
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MostraFinestraNonModaleAggiornamenti(aggiornamenti);
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[UpdateService] Background check error: {ex.Message}");
            }
        }

        private static void MostraFinestraNonModaleAggiornamenti(List<TextUpdateInfo> aggiornamenti)
        {
            MainWindow.ApriAggiungiTesti(aggiornamenti);
/* TODO2 da cancellare
            // Simple non-modal dialog window
            Window dialog = new()
            {
                // TODO2 res
                Title = (string)(Application.Current.TryFindResource("TitoloAggiornamentiTesti") ?? "Text Updates Available"),
                Width = 420,
                Height = 220,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = owner,
                ResizeMode = ResizeMode.NoResize,
                ShowInTaskbar = false
            };

            var stack = new System.Windows.Controls.StackPanel { Margin = new Thickness(15) };
            stack.Children.Add(new System.Windows.Controls.TextBlock
            {
                Text = string.Format(
                    // TODO2 res
                    (string)(Application.Current.TryFindResource("MessaggioAggiornamentiDisponibili") ?? "There are {0} updates available for your installed texts. Would you like to view and update them now?"),
                    aggiornamenti.Count),
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 20)
            });

            var btnPanel = new System.Windows.Controls.StackPanel
            {
                Orientation = System.Windows.Controls.Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            var btnSì = new System.Windows.Controls.Button
            {
                // TODO2 res
                Content = (string)(Application.Current.TryFindResource("Si") ?? "Yes"),
                Width = 80,
                Margin = new Thickness(0, 0, 10, 0),
                IsDefault = true
            };
            btnSì.Click += (s, e) =>
            {
                dialog.Close();
                // Open AggiungiTesti view or trigger update process
                MainWindow.ApriAggiungiTesti(aggiornamenti);
            };

            var btnNo = new System.Windows.Controls.Button
            {
                // TODO2 res
                Content = (string)(Application.Current.TryFindResource("No") ?? "No"),
                Width = 80,
                IsCancel = true
            };
            btnNo.Click += (s, e) => dialog.Close();

            btnPanel.Children.Add(btnSì);
            btnPanel.Children.Add(btnNo);
            stack.Children.Add(btnPanel);

            dialog.Content = stack;
            dialog.Show(); // Non-modal!
*/
        }

        public static bool IsNuovaVersioneDisponibile(string versioneCorrente, string versioneNuova)
        {
            if (Version.TryParse(versioneCorrente, out Version? vCorrente) &&
                Version.TryParse(versioneNuova, out Version? vNuova))
            {
                return vNuova > vCorrente;
            }
            return false;
        }

        public static bool IsVersioneCompatibile(string versioneStringa)
        {
            if (VersioneApp == null)
                return true;

            if (Version.TryParse(versioneStringa, out Version? itemVersion))
            {
                if (itemVersion.Major < VersioneApp.Major)
                    return true;
                if (itemVersion.Major == VersioneApp.Major && itemVersion.Minor <= VersioneApp.Minor)
                    return true;

                return false;
            }
            return true;
        }

        public static async Task<string> ScaricaEInstallaFileAsync(string url, string directoryDownload, int maxTentativi = 3)
        {
            for (int tentativo = 1; tentativo <= maxTentativi; tentativo++)
            {
                try
                {
                    Uri uri = new(url);
                    string nomeFileDaUrl = Path.GetFileName(uri.LocalPath);

                    bool isGz = url.EndsWith(".gz", StringComparison.OrdinalIgnoreCase);
                    bool isLptar = url.EndsWith(".lptar", StringComparison.OrdinalIgnoreCase);

                    string nomeFileFinale = isGz && nomeFileDaUrl.EndsWith(".gz", StringComparison.OrdinalIgnoreCase)
                        ? nomeFileDaUrl[..^3]
                        : nomeFileDaUrl;

                    using HttpResponseMessage response = await HttpClient.GetAsync(new Uri(EncodeUrlLatin1(url)), HttpCompletionOption.ResponseHeadersRead);
                    response.EnsureSuccessStatusCode();

                    using Stream streamRete = await response.Content.ReadAsStreamAsync();
                    using MemoryStream streamMemoria = new();
                    await streamRete.CopyToAsync(streamMemoria);
                    byte[] datiScaricati = streamMemoria.ToArray();

                    string percorsoCompletoFile = Path.Combine(directoryDownload, nomeFileFinale);
                    if (File.Exists(percorsoCompletoFile))
                        File.Delete(percorsoCompletoFile);

                    if (isGz)
                    {
                        if (datiScaricati.Length >= 2 && datiScaricati[0] == 31 && datiScaricati[1] == 139)
                        {
                            streamMemoria.Position = 0;
                            using GZipStream gzipStream = new(streamMemoria, CompressionMode.Decompress);
                            using FileStream fileStreamOutput = File.Create(percorsoCompletoFile);
                            await gzipStream.CopyToAsync(fileStreamOutput);
                        }
                        else
                        {
                            await File.WriteAllBytesAsync(percorsoCompletoFile, datiScaricati);
                        }
                    }
                    else if (isLptar)
                    {
                        await EstraiLptarAsync(datiScaricati, directoryDownload, nomeFileDaUrl);
                    }
                    else
                    {
                        await File.WriteAllBytesAsync(percorsoCompletoFile, datiScaricati);
                    }

                    return percorsoCompletoFile;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[UpdateService] Error downloading {url}: {ex.Message}");
                    if (tentativo == maxTentativi) return "";
                    await Task.Delay(1000 * tentativo);
                }
            }
            return "";
        }

        private static string EncodeUrlLatin1(string url)
        {
            if (string.IsNullOrEmpty(url)) return url;
            StringBuilder sb = new(url.Length * 2);
            foreach (char c in url)
            {
                if (c >= 128)
                {
                    byte b = Encoding.Latin1.GetBytes([c])[0];
                    sb.Append($"%{b:X2}");
                }
                else if (c == ' ')
                {
                    sb.Append("%20");
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private static async Task EstraiLptarAsync(byte[] dati, string directoryDownload, string nomeFileLptar)
        {
            string nomeCartellaDestinazione = Path.GetFileNameWithoutExtension(nomeFileLptar);
            string cartellaOutput = Path.Combine(directoryDownload, nomeCartellaDestinazione);
            Directory.CreateDirectory(cartellaOutput);

            using MemoryStream ms = new(dati);
            using BinaryReader br = new(ms);

            int numeroFile = br.ReadInt32();
            for (int j = 0; j < numeroFile; ++j)
            {
                string nomeFileInner = br.ReadString();
                int numeroByte = br.ReadInt32();
                byte[] byteFile = br.ReadBytes(numeroByte);

                string percorsoFileEstratto = Path.Combine(cartellaOutput, nomeFileInner);
                await File.WriteAllBytesAsync(percorsoFileEstratto, byteFile);
            }
        }
    }
}