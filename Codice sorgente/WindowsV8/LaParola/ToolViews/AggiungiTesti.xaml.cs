using LaParola.Services;
using LaParola.Utilities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml.Linq;

namespace LaParola.ToolViews
{
    public partial class AggiungiTesti : UserControl
    {
        private IEnumerable<XElement> _nodiFile = [];

        private readonly ObservableCollection<ItemTestoViewModel> _listaTesti = [];
        private readonly ICollectionView _vistaTesti;
        private bool _checkboxModificato = false;
        private bool _datiCaricati = false;
        private readonly string _directoryDownload;

        private Point _toolTipOpenPosition;
        private bool _isToolTipOpen;

        public AggiungiTesti()
        {
            InitializeComponent();
            _vistaTesti = CollectionViewSource.GetDefaultView(_listaTesti);
            _vistaTesti.Filter = FiltraElemento;
            GridTesti.ItemsSource = _vistaTesti;

            _directoryDownload = Path.GetDirectoryName(SettingsService.SettingsPath) + Path.DirectorySeparatorChar;
        }

        /// <summary>
        /// Pre-selects specific texts marked for update when opened from the update prompt.
        /// </summary>
        public void SelezionaAggiornamenti(List<TextUpdateInfo> aggiornamenti)
        {
            if (aggiornamenti == null || aggiornamenti.Count == 0)
                return;

            HashSet<string> componentiDaAggiornare = new(aggiornamenti.Select(a => a.Componente), StringComparer.OrdinalIgnoreCase);

            foreach (ItemTestoViewModel item in _listaTesti)
            {
                if (componentiDaAggiornare.Contains(item.Componente))
                {
                    item.DaScaricare = true;
                }
            }

            _vistaTesti?.Refresh();
            AggiornaConteggioStato();
        }

        private void HelpFlyout_OnHelpClicked(object sender, RoutedEventArgs e)
        {
            MainWindow.MostraGuida((string)(Application.Current.TryFindResource("AggiungiTestiTitolo") ?? "Add Texts"));
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
                // Download feed via UpdateService and trigger update check automatically
                (List<XElement> nodes, string _) = await UpdateService.ScaricaFeedXmlAsync();
                _nodiFile = nodes;

                PopolaGriglia();

                // Non-modal update dialog trigger after feed download
                _ = UpdateService.VerificaEMostraNotificaAggiornamentiAsync();
            }
            catch (Exception ex)
            {
                MessageBoxLPN.Show(Window.GetWindow(this),
                    string.Format(CultureInfo.InvariantCulture, (string)(Application.Current.TryFindResource("AggiungiTestiErroreCaricamento") ?? "Error loading text list: {0}"), ex.Message),
                    (string)(Application.Current.TryFindResource("Errore") ?? "Error"));
            }
            finally
            {
                AggiornaConteggioStato();
                if (_listaTesti.Any(FiltraElemento))
                {
                    OverlayCaricamento.Visibility = Visibility.Collapsed;
                }
                else
                {
                    OverlayProgress.Visibility = Visibility.Collapsed;
                    string messaggio = (string)(Application.Current.TryFindResource("AggiungiTestiNienteDaInstallare") ?? "No new texts found.");
                    if (MainWindow.settings.TestiNascosti.Count > 0)
                        messaggio += " " + (string)(Application.Current.TryFindResource("AggiungiTestiNienteDaInstallareTestiNascosti") ?? "You have hidden texts that can be installed.");
                    OverlayTesto.Text = messaggio;
                }
            }
        }

        internal void PopolaGriglia()
        {
            bool isItaliano = MainWindow.settings?.Lingua?.StartsWith("it", StringComparison.OrdinalIgnoreCase) ?? true;
            HashSet<string> tipiAmmessi = new(StringComparer.OrdinalIgnoreCase) { "Bibbia", "commentario", "dizionario", "libro" };

            _listaTesti.Clear();

            foreach (XElement f in _nodiFile)
            {
                string tipo = f.Element("tipo")?.Value?.Trim() ?? "";
                if (tipo == "note") tipo = "commentario";
                if (!tipiAmmessi.Contains(tipo)) continue;

                string componente = f.Element("componente")?.Value?.Trim() ?? "";
                if (string.IsNullOrEmpty(componente)) continue;

                string? nomeIt = f.Elements("nome").FirstOrDefault(x => (string?)x.Attribute("language") == "it")?.Value;
                string? nomeDef = f.Elements("nome").FirstOrDefault(x => x.Attribute("language") == null)?.Value ?? f.Element("nome")?.Value;
                string nome = (isItaliano && !string.IsNullOrEmpty(nomeIt)) ? nomeIt : (nomeDef ?? componente);

                string versioneRemota = f.Element("versione")?.Value ?? "";
                List<string> urls = [.. f.Elements("url").Select(x => x.Value.Trim()).Where(x => !string.IsNullOrEmpty(x))];

                bool giaInstallato = MainWindow.Testi?.VersioneEsiste(componente) ?? false;
                string versioneInstallata = giaInstallato ? (MainWindow.Testi?.Info(componente)?.Versione ?? "0.0") : "";

                // Flag if an update is available for installed text
                bool haAggiornamento = giaInstallato && UpdateService.IsNuovaVersioneDisponibile(versioneInstallata, versioneRemota);
                bool nascosto = MainWindow.settings?.TestiNascosti?.Contains(componente) ?? false;

                ItemTestoViewModel item = new()
                {
                    Componente = componente,
                    Nome = nome,
                    Tipo = tipo,
                    Versione = versioneRemota,
                    VersioneInstallata = versioneInstallata,
                    HaAggiornamentoDisponibile = haAggiornamento,
                    Urls = urls,
                    Dimensione = f.Element("dimensione")?.Value ?? "",
                    Lingua = f.Element("lingua")?.Value ?? "",
                    GiaInstallato = giaInstallato && !haAggiornamento, // Allows downloading if it's an update!
                    Nascosto = nascosto
                };
                item.PropertyChanged += Item_PropertyChanged;
                _listaTesti.Add(item);
            }

            static int GetTipoPriority(string tipo) => tipo.ToLowerInvariant() switch
            {
                "bibbia" => 1,
                "commentario" => 2,
                "dizionario" => 3,
                "libro" => 4,
                _ => 99
            };

            IOrderedEnumerable<ItemTestoViewModel> testiOrdinati = _listaTesti.ToList()
                .OrderByDescending(x => x.HaAggiornamentoDisponibile) // Put available updates at top
                .ThenBy(x => GetTipoPriority(x.Tipo))
                .ThenBy(x => isItaliano && string.Equals(x.Lingua, "it", StringComparison.OrdinalIgnoreCase) ? 0 : 1);

            _listaTesti.Clear();
            foreach (ItemTestoViewModel item in testiOrdinati)
            {
                _listaTesti.Add(item);
            }
        }

        private void DataGridToolTip_Opened(object sender, RoutedEventArgs e)
        {
            _toolTipOpenPosition = Mouse.GetPosition(GridTesti);
            _isToolTipOpen = true;
        }

        private void DataGridToolTip_Closed(object sender, RoutedEventArgs e)
        {
            _isToolTipOpen = false;
        }

        private void GridTesti_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isToolTipOpen)
            {
                Point currentPosition = e.GetPosition(GridTesti);
                if (Point.Subtract(currentPosition, _toolTipOpenPosition).Length > 30)
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
                if (sender != null && !_checkboxModificato)
                {
                    _checkboxModificato = true;
                    ((ItemTestoViewModel)sender).DaNascondere = false;
                    _checkboxModificato = false;
                }
                AggiornaConteggioStato();
            }
            else if (e.PropertyName == nameof(ItemTestoViewModel.DaNascondere))
            {
                if (sender != null && !_checkboxModificato)
                {
                    _checkboxModificato = true;
                    ((ItemTestoViewModel)sender).DaScaricare = false;
                    _checkboxModificato = false;
                }
                AggiornaConteggioStato();
            }
        }

        private bool FiltraElemento(object item)
        {
            if (item is not ItemTestoViewModel testo) return false;

            if (!UpdateService.IsVersioneCompatibile(testo.Versione)) return false;
            if (testo.Nascosto) return false;
            if (testo.GiaInstallato && !testo.HaAggiornamentoDisponibile) return false;

            string filtro = TxtFiltro.Text.Trim();
            if (string.IsNullOrEmpty(filtro)) return true;

            return testo.Nome.Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
                   testo.Tipo.Contains(filtro, StringComparison.OrdinalIgnoreCase) ||
                   testo.Componente.Contains(filtro, StringComparison.OrdinalIgnoreCase);
        }

        private void TxtFiltro_TextChanged(object sender, TextChangedEventArgs e)
        {
            _vistaTesti?.Refresh();
            AggiornaConteggioStato();
        }

        internal void AggiornaConteggioStato()
        {
            int visibili = _listaTesti.Count(FiltraElemento);
            int daScaricare = _listaTesti.Count(x => x.DaScaricare);
            int daNascondere = _listaTesti.Count(x => x.DaNascondere);

            BtnApplica.IsEnabled = (daScaricare + daNascondere > 0);
            BtnSelezionaTutti.IsEnabled = visibili > 0;
            BtnDeselezionaTutti.IsEnabled = (daScaricare + daNascondere > 0);
            BtnMostraNascosti.IsEnabled = MainWindow.settings.TestiNascosti.Count > 0;
        }

        private async void BtnApplica_Click(object sender, RoutedEventArgs e)
        {
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

            List<ItemTestoViewModel> daScaricare = [.. _listaTesti.Where(x => x.DaScaricare && x.CanBeDownloaded)];
            int nScaricare = 0;
            string stringaDaScaricare = "/" + daScaricare.Count + ")";

            if (daScaricare.Count > 0)
            {
                using SemaphoreSlim semaforo = new(3);

                IEnumerable<Task> compitiDownload = daScaricare.Select(async item =>
                {
                    await semaforo.WaitAsync();

                    ++nScaricare;
                    string messaggio = string.Format((string)(Application.Current.TryFindResource("AggiungiTestiInstallazioneScaricamentoInCorso") ?? "Downloading {0}"), item.Nome);
                    using StatusTask status = StatusService.AvviaTask(messaggio + " (" + nScaricare + stringaDaScaricare);

                    try
                    {
                        string percorso = await EseguiDownloadEInstallazioneAsync(item);

                        if (!string.IsNullOrEmpty(percorso))
                        {
                            item.GiaInstallato = true;
                            item.HaAggiornamentoDisponibile = false;
                            item.DaScaricare = false;

                            status.Update(string.Format((string)(Application.Current.TryFindResource("AggiungiTestiInstallazioneInstallazioneInCorso") ?? "Installing {0}"), item.Nome));
                            MainWindow.Testi.AggiungiTesto(percorso, 0);
                            Funzioni.AggiornaTestiNellInterfaccia();
                            status.Update(string.Format((string)(Application.Current.TryFindResource("AggiungiTestiInstallazioneInstallazioneCompletata") ?? "Installation of {0} completed"), item.Nome), percent: 100);
                        }
                        else
                        {
                            status.Update(string.Format((string)(Application.Current.TryFindResource("AggiungiTestiInstallazioneErrore") ?? "Error downloading {0}"), item.Nome), percent: 100);
                        }
                    }
                    finally
                    {
                        semaforo.Release();
                        _vistaTesti.Refresh();
                        AggiornaConteggioStato();
                        await Task.Delay(5000);
                    }
                });

                await Task.WhenAll(compitiDownload);
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
                    if (item.HaAggiornamentoDisponibile)
                    {
                        MainWindow.Testi.CancellaTesto(item.Componente);
                    }

                    string url = item.Urls[i];
                    string percorso = await UpdateService.ScaricaEInstallaFileAsync(url, _directoryDownload);

                    if (string.IsNullOrEmpty(percorso)) return "";
                    if (i == 0) primoPercorsoFile = percorso;

                    await Task.Delay(150);
                }

                return primoPercorsoFile;
            }
            catch
            {
                return "";
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
    }

    public class ItemTestoViewModel : INotifyPropertyChanged
    {
        private bool _daScaricare;
        private bool _daNascondere;
        private bool _giaInstallato;
        private bool _nascosto;
        private bool _haAggiornamentoDisponibile;

        public string Componente { get; set; } = "";
        public string Nome { get; set; } = "";
        public string Tipo { get; set; } = "";
        public string Versione { get; set; } = "";
        public string VersioneInstallata { get; set; } = "";
        public List<string> Urls { get; set; } = [];
        public string Dimensione { get; set; } = "";
        public string Lingua { get; set; } = "";

        public bool HaAggiornamentoDisponibile
        {
            get => _haAggiornamentoDisponibile;
            set { _haAggiornamentoDisponibile = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanBeDownloaded)); }
        }

        public bool GiaInstallato
        {
            get => _giaInstallato;
            set { _giaInstallato = value; OnPropertyChanged(); OnPropertyChanged(nameof(CanBeDownloaded)); }
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

        public bool CanBeDownloaded => !GiaInstallato || HaAggiornamentoDisponibile;

        public string DettaglioVersione => HaAggiornamentoDisponibile
            ? $"{FormatVersion(VersioneInstallata)} → {Versione}"
            : $"{Versione}";

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private static string FormatVersion(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
                return version;

            string[] parts = version.Split('.');

            // Ensure there is at least a second number to format
            if (parts.Length > 1 && int.TryParse(parts[1], out int minor))
            {
                parts[1] = minor.ToString("D2"); // Formats single digits with a leading zero (e.g. 1 -> 01)
                return string.Join(".", parts);
            }

            return version;
        }
    }
}