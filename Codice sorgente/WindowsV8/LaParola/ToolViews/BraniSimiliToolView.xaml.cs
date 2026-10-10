using LaParola.Services;
using LaParola.Utilities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace LaParola.ToolViews
{
    public enum TipoBrani
    {
        Versetti,
        Capitoli
    }

    /// <summary>
    /// Interaction logic for BraniSimili.xaml
    /// </summary>
    public partial class BraniSimiliToolView : UserControl, INotifyPropertyChanged
    {
        private string? _selectedVersione;
        private int _numero = MainWindow.settings.BraniSimiliNumero;
        private TipoBrani _tipo = MainWindow.settings.BraniSimiliVersetti;

        // Available items for the ComboBox
        public ObservableCollection<string> VersioneItems { get; } = [];

        public BraniSimiliToolView()
        {
            DataContext = this;

            InitializeComponent();

            // Initial load on startup
            AggiornaVersioniDisponibili();

            MostraPulsanteStato();
        }

        private void HelpFlyout_OnHelpClicked(object sender, RoutedEventArgs e)
        {
            MainWindow.MostraGuida((string)(Application.Current.TryFindResource("BraniSimiliTitolo") ?? "Similar Passages"));
        }

        private void Brano_KeyUp(object sender, KeyEventArgs e)
        {
            MostraPulsanteStato();
        }

        // Public method accessible by the removal coordinator
        public void AggiornaVersioniDisponibili()
        {
            List<string> available = [.. MainWindow.Testi.NomiVersioni(TestoTipi.Bibbia)];
            HashSet<string> availableSet = [.. available];
            List<string> savedSelected = [];
            if (!string.IsNullOrEmpty(MainWindow.settings.BraniSimiliVersioneSelezionata))
            {
                savedSelected.Add(MainWindow.settings.BraniSimiliVersioneSelezionata);
            }

            // Build UI items cleanly
            VersioneItems.Clear();
            foreach (string v in available)
            {
                if (MainWindow.Testi.EsistonoRadici(v))
                    VersioneItems.Add(v);
            }

            // Pick the first saved selection that still exists, or default to the first available item
            SelectedVersione = savedSelected.FirstOrDefault(v => availableSet.Contains(v))
                               ?? available.FirstOrDefault();

            SaveSelectedVersion();
        }

        // Currently selected single item
        public string? SelectedVersione
        {
            get => _selectedVersione;
            set
            {
                if (_selectedVersione == value) return;
                _selectedVersione = value;
                OnPropertyChanged();

                SaveSelectedVersion();
            }
        }

        public int Numero
        {
            get => _numero;
            set
            {
                // Clamp value between 1 and 100
                int clamped = Math.Clamp(value, 1, 100);
                if (_numero == clamped) return;

                _numero = clamped;
                OnPropertyChanged();
                MainWindow.settings.BraniSimiliNumero = _numero;
            }
        }

        public TipoBrani Tipo
        {
            get => _tipo;
            set
            {
                if (_tipo == value) return;
                _tipo = value;

                // Notify UI of main state change
                OnPropertyChanged();

                // Notify both radio buttons to refresh their checked states
                OnPropertyChanged(nameof(IsVersetti));
                OnPropertyChanged(nameof(IsCapitoli));

                // Save selection to settings if needed
                MainWindow.settings.BraniSimiliVersetti = _tipo;
            }
        }

        public bool IsVersetti
        {
            get => Tipo == TipoBrani.Versetti;
            set { if (value) Tipo = TipoBrani.Versetti; }
        }

        // Boolean wrapper for "Roots" option
        public bool IsCapitoli
        {
            get => Tipo == TipoBrani.Capitoli;
            set { if (value) Tipo = TipoBrani.Capitoli; }
        }

        private void SaveSelectedVersion()
        {
            MostraPulsanteStato();
            MainWindow.settings.BraniSimiliVersioneSelezionata = SelectedVersione ?? "";
        }

        private void MostraPulsanteStato()
        {
            if (!string.IsNullOrWhiteSpace(_selectedVersione) && !string.IsNullOrEmpty(tbBrano.Text))
            {
                PulAvvia.IsEnabled = true;
            }
            else
            {
                PulAvvia.IsEnabled = false;
            }
        }

        private async void BraniSimili_Click(object sender, RoutedEventArgs e)
        {
            string brano = tbBrano.Text;
            if (string.IsNullOrWhiteSpace(_selectedVersione))
                return;
            string versione = _selectedVersione;

            await BraniSimiliInEditorAsync(brano, versione, _numero, IsVersetti);
        }

        public static async Task BraniSimiliInEditorAsync(string brano, string versione, int nBrani, bool versetti)
        {
            int limiteProgresso = MainWindow.Testi.Radici(versione).Length;

            string statusMsg = (string)(Application.Current.TryFindResource("BraniSimiliCreazione") ?? "Calculating the similar passages...");

            // 1. Start Status Task (auto-removed from UI via using/Dispose when method completes)
            using StatusTask status = StatusService.AvviaTask(statusMsg, isIndeterminate: false);

            // 2. Run computation off the UI thread
            string testoRtf = await Task.Run(async () => await CreaBraniSimili(
                brano, versione,
                nBrani, versetti,
                limiteProgresso, statusMsg, status
            ));

            // 3. Back on the UI thread: Render and load document in active editor
            string titoloFinestra = (string)(Application.Current.TryFindResource("BraniSimiliRisultatoTitolo") ?? "Similar passages to") + " " + brano + " (" + MainWindow.Testi.Info(versione).Abbreviazione + ") ";

            FlowDocument doc = Texts.LoadRtfToFlowDocumentOnUiThread(testoRtf);
            Brush fg = (Brush)Application.Current.FindResource("AppForegroundBrush");
            RtfColorTransformer.ApplyThemeToDocument(doc, true, fg, true);

            App.DockingHost.SendFlowDocumentToActiveEditor(doc, titoloFinestra, versione);
        }

        private static async Task<string> CreaBraniSimili(
    string brano, string versione,
    int nBrani, bool versetti,
    int limiteProgresso, string statusMsg, StatusTask status)
        {
            string[] radici = MainWindow.Testi.Radici(versione);
            int nRadici = radici.Length;
            Riferimento braniSimili = new();

            const int MaxCapitoli = 151;
            const int MaxVersetti = 177;
            double[] punteggio = new double[74 * MaxCapitoli * MaxVersetti];
            Riferimento apparenze;
            Riferimento apparenzeInBrano;
            Riferimento riferimentoBrano = MainWindow.Testi.ConvertiRiferimento(brano);
            int nVolteInBrano;
            double fattore, logNumeroVersetti = Math.Log(MainWindow.Testi.VersettiFinoACapitolo(73, 22, versione) + MainWindow.Testi.VersettiInCapitolo(73, 22, versione));

            // Throttle progress updates to ~1% increments to prevent UI message flooding
            int updateInterval = Math.Max(1, nRadici / 100);

            // prima di tutto, controlliamo che il riferimento esiste in questa versione, cioè almeno uno dei libri
            bool riferimentoEsiste = false;
            foreach (byte[] branoDaControllare in riferimentoBrano.Brani)
            {
                for (byte i = branoDaControllare[0]; i <= branoDaControllare[3]; ++i)
                {
                    if (MainWindow.Testi.CapitoliInLibro(i, versione) > 0)
                    {
                        riferimentoEsiste = true;
                        break;
                    }
                }
                if (riferimentoEsiste)
                    break;
            }

            if (riferimentoEsiste)
            {
                // usiamo l'algoritmo tf-idf
                // http://en.wikipedia.org/wiki/Tf%E2%80%93idf
                // con qualche modifica
                // dovrebbe essere
                // somma (numero volte in brano base)/(numero totale parole in quel brano)*(numero volte in quel brano) * log(numero versetti/numero versetti con parola)
                for (int i = 0; i < nRadici; ++i)
                {
                    if (radici[i] != "*")
                    {
                        apparenzeInBrano = MainWindow.Testi.RicercaRadiceInBrano(radici[i], riferimentoBrano, versione);
                        nVolteInBrano = apparenzeInBrano.Count;
                        if (nVolteInBrano > 0)
                        {
                            apparenze = MainWindow.Testi.RicercaRadiceInBrano(radici[i], versione);
                            apparenze.RimuoviVersetti(apparenzeInBrano);
                            if (apparenze.Count > 0)
                            {
                                //fattore = (float)nVolteInBrano / apparenze.Count; - un modo alternativo
                                fattore = nVolteInBrano * (logNumeroVersetti - Math.Log(apparenze.Count));
                                foreach (byte[] b in apparenze.Brani)
                                {
                                    // Calculate flat index: (book * 151 + chapter) * 177 + verse
                                    int index = (b[0] * MaxCapitoli + b[1]) * MaxVersetti + (versetti ? b[2] : (byte)0);
                                    punteggio[index] += fattore;
                                }
                            }
                        }
                    }
                    if (i % updateInterval == 0 || i == nRadici - 1)
                    {
                        status.Update(statusMsg, (double)i / limiteProgresso * 100.0);
                    }
                }

                // Keep only the Top-N highest scoring passages using a Min-Heap PriorityQueue
                PriorityQueue<uint, double> topBrani = new();
                double score, scoreNormalized;
                uint capitoloId;

                if (versetti)
                {
                    for (byte i = 1; i <= 73; ++i)
                    {
                        for (byte j = 0; j <= 149; ++j) // necessario fare in questo modo, perché da 1 a 150, byte-1 non funziona
                        {
                            for (byte k = 1; k <= 176; ++k)
                            {
                                score = punteggio[(i * MaxCapitoli + (j + 1)) * MaxVersetti + k];

                                if (score > 0)
                                {
                                    uint versettoId = (uint)(MainWindow.Testi.VersettiFinoACapitolo(i, j, versione) + k);
                                    EnqueueTopN(topBrani, versettoId, score, nBrani);
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (byte i = 1; i <= 73; ++i) // necessario fare in questo modo, perché da 1 a 73, byte-1 non funziona
                    {
                        for (byte j = 1; j <= 150; ++j)
                        {
                            score = punteggio[(i * MaxCapitoli + j) * MaxVersetti];

                            if (score > 0)
                            {
                                scoreNormalized = score / MainWindow.Testi.VersettiInCapitolo(i, j, versione);
                                // dovrebbe essere "/ (parole in capitoli)", ma il programma non sa questo numero
                                capitoloId = (uint)(MainWindow.Testi.CapitoliFinoALibro((byte)(i - 1), versione) + j);
                                EnqueueTopN(topBrani, capitoloId, scoreNormalized, nBrani);
                            }
                        }
                    }
                }

                // Extract IDs in descending order
                List<uint> resultIds = new(topBrani.Count);
                while (topBrani.Count > 0)
                {
                    resultIds.Add(topBrani.Dequeue());
                }
                resultIds.Reverse(); // Place highest score first

                foreach (uint id in resultIds)
                {
                    if (versetti)
                        braniSimili.AggiungiBraniDaRiferimento(MainWindow.Testi.RiferimentoDiVersetto((int)id, versione));
                    else
                        braniSimili.AggiungiBraniDaRiferimento(MainWindow.Testi.RiferimentoDiCapitolo((int)id, versione));
                }

            }
            else
            {
                status.Update(statusMsg, 100.0);
            }

            return await MainWindow.Testi.TestoBranoAsync(braniSimili, versione);// MainWindow.Testi.TestoBrano(braniSimili, versione);
        }

        /// <summary>
        /// Maintains a bounded Min-Heap storing only the top N highest-scoring items.
        /// </summary>
        private static void EnqueueTopN(PriorityQueue<uint, double> queue, uint item, double score, int maxCapacity)
        {
            if (queue.Count < maxCapacity)
            {
                queue.Enqueue(item, score);
            }
            else if (queue.TryPeek(out _, out double minScore) && score > minScore)
            {
                queue.Dequeue();
                queue.Enqueue(item, score);
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        #endregion

    }
}
