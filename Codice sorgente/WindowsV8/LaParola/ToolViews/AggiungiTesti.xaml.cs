using LaParola.Services;
using LaParola.Utilities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Linq;

namespace LaParola.ToolViews
{
    // TODO2 come aggiornare testi? programma?

    /// <summary>
    /// Interaction logic for AggiungiTesti.xaml
    /// </summary>
    public partial class AggiungiTesti : UserControl
    {
        private const string UrlXmlAggiornamenti = "https://www.laparola.net/programma/aggiorna_it.xml";
        private static readonly HttpClient HttpClient = CreateConfiguredHttpClient();
        IEnumerable<XElement> nodiFile = [];

        private readonly ObservableCollection<ItemTestoViewModel> _listaTesti = [];
        private readonly ICollectionView _vistaTesti;
        private bool checkboxModificato = false;
        private readonly Version? versioneApp = Assembly.GetExecutingAssembly().GetName().Version;
        private bool _datiCaricati = false; // flag per tracciare il primo caricamento
        private readonly string directoryDownload;

        private Point _toolTipOpenPosition;
        private bool _isToolTipOpen;

        public AggiungiTesti()
        {
            InitializeComponent();
            _vistaTesti = CollectionViewSource.GetDefaultView(_listaTesti);
            _vistaTesti.Filter = FiltraElemento;
            GridTesti.ItemsSource = _vistaTesti;

            directoryDownload = Path.GetDirectoryName(SettingsService.ResolveSettingsPath()) + Path.DirectorySeparatorChar;
        }

        private void HelpFlyout_OnHelpClicked(object sender, RoutedEventArgs e)
        {
            // TODO2: Open correct help section
            MessageBox.Show("Open Help Centre");
        }

        private static HttpClient CreateConfiguredHttpClient()
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Add("User-Agent", "LaParolaDesktop/7.0 (Windows)");
            client.Timeout = TimeSpan.FromSeconds(30);
            return client;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_datiCaricati)
                return;

            _datiCaricati = true;
            await CaricaDatiAsync();
        }

        private async Task CaricaDatiAsync()
        {
            OverlayCaricamento.Visibility = Visibility.Visible;
            _listaTesti.Clear();

            try
            {
                string xmlContent = await HttpClient.GetStringAsync(UrlXmlAggiornamenti);
                XDocument doc = XDocument.Parse(xmlContent);
                // Lettura dei nodi <file>
                nodiFile = doc.Root?.Elements("file") ?? [];

                PopolaGrigia();
            }
            catch (Exception ex)
            {
                MessageBoxLPN.Show(Window.GetWindow(this),
                    string.Format(CultureInfo.InvariantCulture, (string)(Application.Current.TryFindResource("AggiungiTestiErroreCaricamento") ?? "Error during the loading of the list of texts: {0}"), ex.Message),
                    (string)(Application.Current.TryFindResource("Errore") ?? "Error"));
            }
            finally
            {
                AggiornaConteggioStato();
                if (_listaTesti.Any(x => FiltraElemento(x)))
                {
                    OverlayCaricamento.Visibility = Visibility.Collapsed;
                }
                else
                {
                    OverlayProgress.Visibility = Visibility.Collapsed;
                    string messaggio = (string)(Application.Current.TryFindResource("AggiungiTestiNienteDaInstallare") ?? "No texts were found that can be added to the program.");
                    if (MainWindow.settings.TestiNascosti.Count > 0)
                        messaggio += " " + (string)(Application.Current.TryFindResource("AggiungiTestiNienteDaInstallareTestiNascosti") ?? "However, there are texts that you have hidden that can be installed: click the 'Unhide the hidden texts' to reveal them.");
                    OverlayTesto.Text = messaggio;
                }
            }
        }

        internal void PopolaGrigia()
        {
            bool isItaliano = MainWindow.settings?.Lingua?.StartsWith("it", StringComparison.OrdinalIgnoreCase) ?? true;

            // Tipi validi richiesti
            HashSet<string> tipiAmmessi = new(StringComparer.OrdinalIgnoreCase)
                { // TODO2 aggiungere altri tipi
                    "Bibbia", "commentario", "dizionario", "libro"
                };

            _listaTesti.Clear();

            foreach (XElement f in nodiFile)
            {
                string tipo = f.Element("tipo")?.Value?.Trim() ?? "";
                if (tipo == "note")
                    tipo = "commentario";
                if (!tipiAmmessi.Contains(tipo))
                    continue;

                string componente = f.Element("componente")?.Value?.Trim() ?? "";
                if (string.IsNullOrEmpty(componente))
                    continue;

                // Gestione del nome localizzato in italiano vs default
                string? nomeIt = f.Elements("nome").FirstOrDefault(x => (string?)x.Attribute("language") == "it")?.Value;
                string? nomeDef = f.Elements("nome").FirstOrDefault(x => x.Attribute("language") == null)?.Value ?? f.Element("nome")?.Value;
                string nome = (isItaliano && !string.IsNullOrEmpty(nomeIt)) ? nomeIt : (nomeDef ?? componente);

                string versione = f.Element("versione")?.Value ?? "";
                List<string> urls = [.. f.Elements("url")
                                     .Select(x => x.Value.Trim())
                                     .Where(x => !string.IsNullOrEmpty(x))];
                string dimensione = f.Element("dimensione")?.Value ?? "";
                string lingua = f.Element("lingua")?.Value ?? "";

                bool giaInstallato = MainWindow.Testi?.VersioneEsiste(componente) ?? false;
                bool nascosto = MainWindow.settings?.TestiNascosti?.Contains(componente) ?? false;

                ItemTestoViewModel item = new()
                {
                    Componente = componente,
                    Nome = nome,
                    Tipo = tipo,
                    Versione = versione,
                    Urls = urls,
                    Dimensione = dimensione,
                    Lingua = lingua,
                    GiaInstallato = giaInstallato,
                    Nascosto = nascosto,
                    DaNascondere = false,
                    DaScaricare = false
                };
                item.PropertyChanged += Item_PropertyChanged;
                _listaTesti.Add(item);
            }

            // Funzione locale per assegnare una priorità numerica a ciascun tipo
            static int GetTipoPriority(string tipo) => tipo.ToLowerInvariant() switch
            { // TODO2 aggiungere altri tipi
                "bibbia" => 1,
                "commentario" => 2,
                "dizionario" => 3,
                "libro" => 4,
                _ => 99
            };

            // Ordina la lista usando LINQ
            IOrderedEnumerable<ItemTestoViewModel> testiOrdinati = _listaTesti.ToList()
                .OrderBy(x => GetTipoPriority(x.Tipo))
                .ThenBy(x => isItaliano && string.Equals(x.Lingua, "it", StringComparison.OrdinalIgnoreCase) ? 0 : 1);
            //.ThenBy(x => x.Nome); // (Opzionale) Ordine alfabetico secondario per il nome

            // Popola l'ObservableCollection per la UI
            _listaTesti.Clear();
            foreach (ItemTestoViewModel item in testiOrdinati)
            {
                _listaTesti.Add(item);
            }
        }

        private void DataGridToolTip_Opened(object sender, RoutedEventArgs e)
        {
            // Capture the exact location where the tooltip spawned relative to the DataGrid
            _toolTipOpenPosition = Mouse.GetPosition(GridTesti);
            _isToolTipOpen = true;
        }

        private void DataGridToolTip_Closed(object sender, RoutedEventArgs e)
        {
            // Reset flag if the tooltip closes naturally via timeout
            _isToolTipOpen = false;
        }

        private void GridTesti_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isToolTipOpen)
            {
                Point currentPosition = e.GetPosition(GridTesti);

                // Calculate Euclidean distance moved since opening: sqrt((x2-x1)^2 + (y2-y1)^2)
                double distanceMoved = Point.Subtract(currentPosition, _toolTipOpenPosition).Length;

                // Threshold in pixels. If they slide the mouse more than 30px, kill the tooltip
                if (distanceMoved > 30)
                {
                    _isToolTipOpen = false;
                    DataGridToolTip.IsOpen = false;
                }
            }
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ItemTestoViewModel.DaScaricare))
            {
                if (sender != null && !checkboxModificato)
                {
                    checkboxModificato = true;
                    ((ItemTestoViewModel)sender).DaNascondere = false;
                    checkboxModificato = false;
                }
                AggiornaConteggioStato();
            }
            else if (e.PropertyName == nameof(ItemTestoViewModel.DaNascondere))
            {
                if (sender != null && !checkboxModificato)
                {
                    checkboxModificato = true;
                    ((ItemTestoViewModel)sender).DaScaricare = false;
                    checkboxModificato = false;
                }
                AggiornaConteggioStato();
            }
        }

        #region Filtraggio & Visualizzazione

        private bool FiltraElemento(object item)
        {
            if (item is not ItemTestoViewModel testo)
                return false;

            // Filtro versione - il testo non deve essere più recente dell'app
            if (!IsVersioneCompatibile(testo.Versione))
                return false;

            // Filtro Nascosti
            if (testo.Nascosto)
                return false;

            if (testo.GiaInstallato)
                return false;

            // Filtro Testo di ricerca
            string filtro = TxtFiltro.Text.Trim();
            if (string.IsNullOrEmpty(filtro))
                return true;

            return testo.Nome.Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
                   testo.Tipo.Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
                   testo.Componente.Contains(filtro, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsVersioneCompatibile(string versioneStringa)
        {
            if (versioneApp == null)
                return true;
            int versioneMajor = versioneApp.Major;
            int versioneMinor = versioneApp.Minor;

            // Converte la stringa "7.05.8" in un oggetto Version numerico
            if (Version.TryParse(versioneStringa, out Version? itemVersion))
            {
                // 1. Il primo numero (Major) è minore? -> Ok
                if (itemVersion.Major < versioneMajor)
                    return true;

                // 2. Il primo numero è uguale E il secondo (Minor) è minore o uguale? -> Ok
                if (itemVersion.Major == versioneMajor && itemVersion.Minor <= versioneMinor)
                    return true;

                // In tutti gli altri casi (versione superiore) -> Non compatibile
                return false;
            }

            // Se la stringa della versione non è valida o è vuota, decidiamo di mostrarlo
            return true;
        }

        private void TxtFiltro_TextChanged(object sender, TextChangedEventArgs e)
        {
            _vistaTesti?.Refresh();
            AggiornaConteggioStato();
        }

        internal void AggiornaConteggioStato()
        {
            int visibili = _listaTesti.Count(x => FiltraElemento(x));
            int daScaricare = _listaTesti.Count(x => x.DaScaricare);
            int daNascondere = _listaTesti.Count(x => x.DaNascondere);

            BtnApplica.IsEnabled = (daScaricare + daNascondere > 0);
            BtnSelezionaTutti.IsEnabled = visibili > 0;
            BtnDeselezionaTutti.IsEnabled = (daScaricare + daNascondere > 0);
            BtnMostraNascosti.IsEnabled = MainWindow.settings.TestiNascosti.Count > 0;
        }

        #endregion

        #region Azioni Pulsanti

        private async void BtnApplica_Click(object sender, RoutedEventArgs e)
        {
            // 1. Salva lo stato dei testi nascosti nelle impostazioni globali
            MainWindow.settings.TestiNascosti ??= [];

            foreach (ItemTestoViewModel item in _listaTesti)
            {
                if (item.DaNascondere)
                {
                    item.Nascosto = true;
                    item.DaNascondere = false;
                }
                if (item.Nascosto)
                    MainWindow.settings.TestiNascosti.Add(item.Componente);
                else
                    MainWindow.settings.TestiNascosti.Remove(item.Componente);
            }

            // 2. Raccoglie i testi selezionati per il download
            List<ItemTestoViewModel> daScaricare = [.. _listaTesti.Where(x => x.DaScaricare && x.CanBeDownloaded)];
            int nScaricare = 0;
            string stringaDaScaricare = "/" + daScaricare.Count + ")";
            if (daScaricare.Count > 0)
            {
                // Limite di max 3 download simultanei
                using SemaphoreSlim semaforo = new(3);

                // Task complessivo per monitorare tutti i download
                IEnumerable<Task> compitiDownload = daScaricare.Select(async item =>
                {
                    // Attende il suo turno prima di iniziare
                    await semaforo.WaitAsync();

                    // Viene creato un elemento nella StatusBar SOLO quando il download inizia davvero
                    ++nScaricare;
                    string messaggio = string.Format((string)(Application.Current.TryFindResource("AggiungiTestiInstallazioneScaricamentoInCorso") ?? "Downloading the file {0}"), item.Nome);
                    using StatusTask status = StatusService.AvviaTask(messaggio + " (" + nScaricare + stringaDaScaricare);

                    try
                    {
                        string percorso = await EseguiDownloadEInstallazioneAsync(item);

                        if (!string.IsNullOrEmpty(percorso))
                        {
                            item.GiaInstallato = true;
                            item.DaScaricare = false;

                            status.Update(string.Format((string)(Application.Current.TryFindResource("AggiungiTestiInstallazioneInstallazioneInCorso") ?? "Installing the file {0}"), item.Nome));
                            MainWindow.Testi.AggiungiTesto(percorso, 0);
                            Funzioni.AggiornaTestiNellInterfaccia();
                            status.Update(string.Format((string)(Application.Current.TryFindResource("AggiungiTestiInstallazioneInstallazioneCompletata") ?? "Installation of {0} completed"), item.Nome), percent: 100);
                        }
                        else
                        {
                            status.Update(string.Format((string)(Application.Current.TryFindResource("AggiungiTestiInstallazioneErrore") ?? "Error downloading {0}, text not installed"), item.Nome), percent: 100);
                        }
                    }
                    finally
                    {
                        // Libera lo slot per il prossimo file nella coda
                        semaforo.Release();

                        _vistaTesti.Refresh();
                        AggiornaConteggioStato();

                        await Task.Delay(5000); // lasciare il messaggio, poi scompare dopo 5 secondi
                    }
                });

                // Attende che tutti i download (anche quelli in coda) siano terminati
                await Task.WhenAll(compitiDownload);

                if (daScaricare.Count > 1)
                {
                    using StatusTask status = StatusService.AvviaTask((string)(Application.Current.TryFindResource("AggiungiTestiInstallazioneCompletato") ?? "Downloading and installing completed"), Visibility.Collapsed);

                    _vistaTesti.Refresh();
                    AggiornaConteggioStato();

                    await Task.Delay(5000);
                }
            }
            else
            {
                // Ricarica la tabella
                _vistaTesti.Refresh();
                AggiornaConteggioStato();
            }
        }

        private void BtnSelezionaTutti_Click(object sender, RoutedEventArgs e)
        {
            foreach (ItemTestoViewModel item in _vistaTesti)
            {
                if (item.CanBeDownloaded)
                    item.DaScaricare = true;
            }
            AggiornaConteggioStato();
        }

        private void BtnDeselezionaTutti_Click(object sender, RoutedEventArgs e)
        {
            foreach (ItemTestoViewModel item in _listaTesti)
            {
                item.DaScaricare = false;
                item.DaNascondere = false;
            }
            AggiornaConteggioStato();
        }

        private void BtnMostraNascosti_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.settings.TestiNascosti = [];
            foreach (ItemTestoViewModel item in _listaTesti)
            {
                item.Nascosto = false;
            }
            _vistaTesti.Refresh();
            AggiornaConteggioStato();
        }

        private async Task<string> EseguiDownloadEInstallazioneAsync(ItemTestoViewModel item)
        {
            try
            {
                string primoPercorsoFile = "";

                for (int i = 0; i < item.Urls.Count; i++)
                {
                    string url = item.Urls[i];
                    string percorso = await ScaricaEInstallaFileAsync(url);

                    if (string.IsNullOrEmpty(percorso))
                    {
                        System.Diagnostics.Debug.WriteLine($"[DOWNLOAD ERROR] Failed to download sub-file {i + 1} ({url}) for component {item.Nome}. Aborting component.");
                        return "";
                    }

                    // Memorizza il percorso del primo file (quello principale da registrare in MainWindow.Testi)
                    if (i == 0)
                    {
                        primoPercorsoFile = percorso;
                    }

                    // Small delay (150ms) between files to prevent triggering server anti-DoS / throttling
                    await Task.Delay(150);
                }

                return primoPercorsoFile;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Scarica e decompone un file dall'URL specificato all'interno di directoryDownload.
        /// </summary>
        public async Task<string> ScaricaEInstallaFileAsync(string url, int maxTentativi = 3)
        {
            for (int tentativo = 1; tentativo <= maxTentativi; tentativo++)
            {
                try
                {
                    // 1. Uri gestisce automaticamente l'encoding dei caratteri speciali/Unicode
                    Uri uri = new(url);
                    string nomeFileDaUrl = Path.GetFileName(uri.LocalPath);

                    bool isGz = url.EndsWith(".gz", StringComparison.OrdinalIgnoreCase);
                    bool isLptar = url.EndsWith(".lptar", StringComparison.OrdinalIgnoreCase);

                    // 2. Determina il nome file finale (rimuove ".gz" se presente)
                    string nomeFileFinale = isGz && nomeFileDaUrl.EndsWith(".gz", StringComparison.OrdinalIgnoreCase)
                        ? nomeFileDaUrl[..^3] // Slices C#: Rimuove gli ultimi 3 caratteri (.gz)
                        : nomeFileDaUrl;

                    string percorsoCompletoFile = Path.Combine(directoryDownload, nomeFileFinale);

                    // Rimuovi eventuale file esistente prima di sovrascrivere
                    if (File.Exists(percorsoCompletoFile))
                        File.Delete(percorsoCompletoFile);

                    // 3. Download dello stream dalla rete
                    // bisogna codificare l'url per casi come João Ferreira de Almeida Atualizada
                    using HttpResponseMessage response = await HttpClient.GetAsync(new Uri(EncodeUrlLatin1(url)), HttpCompletionOption.ResponseHeadersRead);
                    response.EnsureSuccessStatusCode();

                    using Stream streamRete = await response.Content.ReadAsStreamAsync();
                    using MemoryStream streamMemoria = new();
                    await streamRete.CopyToAsync(streamMemoria);
                    byte[] datiScaricati = streamMemoria.ToArray();

                    // 4. Gestione file .gz
                    if (isGz)
                    {
                        // Controlla il "Magic Number" del formato GZip (31 e 139 / 0x1F e 0x8B)
                        if (datiScaricati.Length >= 2 && datiScaricati[0] == 31 && datiScaricati[1] == 139)
                        {
                            streamMemoria.Position = 0;
                            using GZipStream gzipStream = new(streamMemoria, CompressionMode.Decompress);
                            using FileStream fileStreamOutput = File.Create(percorsoCompletoFile);
                            await gzipStream.CopyToAsync(fileStreamOutput);
                        }
                        else
                        {
                            // Il server ha già decompresso il file durante il trasferimento HTTP
                            await File.WriteAllBytesAsync(percorsoCompletoFile, datiScaricati);
                        }
                    }
                    // 5. Gestione archivi personalizzati .lptar
                    else if (isLptar)
                    {
                        await EstraiLptarAsync(datiScaricati, directoryDownload, nomeFileDaUrl);
                    }
                    // 6. File normale (senza compressione)
                    else
                    {
                        await File.WriteAllBytesAsync(percorsoCompletoFile, datiScaricati);
                    }

                    return percorsoCompletoFile;
                }
                catch (Exception ex)
                {
                    // Gestisci o traccia l'errore se necessario
                    System.Diagnostics.Debug.WriteLine($"Errore durante il download di {url}: {ex.Message}");

                    // If we ran out of retries, fail
                    if (tentativo == maxTentativi)
                        return "";

                    // Wait with backoff before trying again (1s for 1st retry, 2s for 2nd retry)
                    await Task.Delay(1000 * tentativo);
                }
            }
            return "";
        }

        /// <summary>
        /// Converte un URL con caratteri speciali/accentati nella codifica Latin-1 (ISO-8859-1) 
        /// attesa da vecchi server web, preservando le sequenze percent-encoded.
        /// </summary>
        private static string EncodeUrlLatin1(string url)
        {
            if (string.IsNullOrEmpty(url))
                return url;

            StringBuilder sb = new(url.Length * 2);

            foreach (char c in url)
            {
                if (c >= 128)
                {
                    // Ottiene il byte in codifica ISO-8859-1 (Latin-1) e lo formatta come %XX
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

        /// <summary>
        /// Estrae il formato archivio binario custom .lptar
        /// </summary>
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

        #endregion
    }

    #region ViewModel di Supporto

    public class ItemTestoViewModel : INotifyPropertyChanged
    {
        private bool _daScaricare;
        private bool _daNascondere;
        private bool _giaInstallato;
        private bool _nascosto;

        public string Componente { get; set; } = "";
        public string Nome { get; set; } = "";
        public string Tipo { get; set; } = "";
        public string Versione { get; set; } = "";
        public List<string> Urls { get; set; } = [];
        public string Dimensione { get; set; } = "";
        public string Lingua { get; set; } = "";

        public bool GiaInstallato
        {
            get => _giaInstallato;
            set
            {
                _giaInstallato = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanBeDownloaded));
            }
        }

        public bool Nascosto
        {
            get => _nascosto;
            set { _nascosto = value; OnPropertyChanged(); }
        }

        public bool DaNascondere
        {
            get => _daNascondere;
            set { _daNascondere = value; OnPropertyChanged(); }
        }

        public bool DaScaricare
        {
            get => _daScaricare;
            set { _daScaricare = value; OnPropertyChanged(); }
        }

        public bool CanBeDownloaded => !GiaInstallato;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    #endregion
    }
}
