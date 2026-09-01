using LaParola.Services;
using LaParola.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace LaParola.ToolViews
{
    struct ParolaApparenze
    {
        /// <summary>
        /// La parola.
        /// </summary>
        public string Parola;
        /// <summary>
        /// Le apparenze della parola.
        /// </summary>
        public Riferimento Apparenze;
    }

    public enum TipoChiave
    {
        Parole,
        Radici
    }

    public enum OrdineChiave
    {
        Alfabetico,
        Numerico,
        Prima
    }

    /// <summary>
    /// Interaction logic for CreaChiaveToolView.xaml
    /// </summary>
    public partial class CreaChiaveToolView : UserControl, INotifyPropertyChanged
    {
        private string? _selectedVersione;
        private int _numeroMinimo = MainWindow.settings.CreaChiaveNumeroMinimo;
        private bool _conRiferimenti = MainWindow.settings.CreaChiaveConRiferimenti;
        private TipoChiave _tipo = MainWindow.settings.CreaChiaveTipo;
        private OrdineChiave _ordine = MainWindow.settings.CreaChiaveOrdine;
        private bool _escludiParoleAbilitato = MainWindow.settings.CreaChiaveEscludiParole;
        private string _paroleDaEscludereText = MainWindow.settings.CreaChiaveParoleDaEscludere;

        public CreaChiaveToolView()
        {
            DataContext = this;

            InitializeComponent();

            // Initial load on startup
            AggiornaVersioniDisponibili();

            MostraPulsanteStato();

            // Auto-save when view closes/unloads
            Unloaded += (s, e) => App.Settings.Save(MainWindow.settings);
        }

        // Available items for the ComboBox
        public ObservableCollection<string> VersioneItems { get; } = [];

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

        public TipoChiave Tipo
        {
            get => _tipo;
            set
            {
                if (_tipo == value) return;
                _tipo = value;

                // Notify UI of main state change
                OnPropertyChanged();

                // Notify both radio buttons to refresh their checked states
                OnPropertyChanged(nameof(IsTipoParole));
                OnPropertyChanged(nameof(IsTipoRadici));

                // Save selection to settings if needed
                MainWindow.settings.CreaChiaveTipo = _tipo;
                App.Settings.Save(MainWindow.settings);
            }
        }

        // Boolean wrapper for "Words" option
        public bool IsTipoParole
        {
            get => Tipo == TipoChiave.Parole;
            set { if (value) Tipo = TipoChiave.Parole; }
        }

        // Boolean wrapper for "Roots" option
        public bool IsTipoRadici
        {
            get => Tipo == TipoChiave.Radici;
            set { if (value) Tipo = TipoChiave.Radici; }
        }

        public OrdineChiave Ordine
        {
            get => _ordine;
            set
            {
                if (_ordine == value) return;
                _ordine = value;

                // Notify UI of main state change
                OnPropertyChanged();

                // Notify both radio buttons to refresh their checked states
                OnPropertyChanged(nameof(IsOrdineAlfabetico));
                OnPropertyChanged(nameof(IsOrdineNumerico));
                OnPropertyChanged(nameof(IsOrdinePrima));

                // Save selection to settings if needed
                MainWindow.settings.CreaChiaveOrdine = _ordine;
                App.Settings.Save(MainWindow.settings);
            }
        }

        public bool IsOrdineAlfabetico
        {
            get => Ordine == OrdineChiave.Alfabetico;
            set { if (value) Ordine = OrdineChiave.Alfabetico; }
        }

        public bool IsOrdineNumerico
        {
            get => Ordine == OrdineChiave.Numerico;
            set { if (value) Ordine = OrdineChiave.Numerico; }
        }

        public bool IsOrdinePrima
        {
            get => Ordine == OrdineChiave.Prima;
            set { if (value) Ordine = OrdineChiave.Prima; }
        }


        public int NumeroMinimo
        {
            get => _numeroMinimo;
            set
            {
                // Clamp value between 1 and 100
                int clamped = Math.Clamp(value, 1, 100);
                if (_numeroMinimo == clamped) return;

                _numeroMinimo = clamped;
                OnPropertyChanged();
                MainWindow.settings.CreaChiaveNumeroMinimo = _numeroMinimo;
                //App.Settings.Save(MainWindow.settings); non salviamo, perché con slider, troppi salvataggi sul disco, salviamo solo quando si chiude la finestra
            }
        }

        public bool ConRiferimenti
        {
            get => _conRiferimenti;
            set
            {
                if (_conRiferimenti == value) return;
                _conRiferimenti = value;
                OnPropertyChanged();
                MainWindow.settings.CreaChiaveConRiferimenti = _conRiferimenti;
                App.Settings.Save(MainWindow.settings);
            }
        }

        public bool EscludiParoleAbilitato
        {
            get => _escludiParoleAbilitato;
            set
            {
                if (_escludiParoleAbilitato == value) return;
                _escludiParoleAbilitato = value;
                OnPropertyChanged();
                MainWindow.settings.CreaChiaveEscludiParole = _escludiParoleAbilitato;
                App.Settings.Save(MainWindow.settings);
            }
        }

        public string ParoleDaEscludereText
        {
            get => _paroleDaEscludereText;
            set
            {
                if (_paroleDaEscludereText == value) return;
                _paroleDaEscludereText = value;
                OnPropertyChanged();
                MainWindow.settings.CreaChiaveParoleDaEscludere = _paroleDaEscludereText;
                App.Settings.Save(MainWindow.settings);
            }
        }

        // Helper method to get clean list of individual words in C#
        public List<string> GetParoleDaEscludereList()
        {
            if (!EscludiParoleAbilitato || string.IsNullOrWhiteSpace(ParoleDaEscludereText))
                return [];

            return [.. ParoleDaEscludereText
                .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
                .Select(w => w.Trim())
                .Where(w => !string.IsNullOrEmpty(w))
                .Distinct()];
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        #endregion

        // Public method accessible by the removal coordinator
        public void AggiornaVersioniDisponibili()
        {
            List<string> available = [.. MainWindow.Testi.NomiVersioni()];
            HashSet<string> availableSet = [.. available];
            List<string> savedSelected = [];
            if (!string.IsNullOrEmpty(MainWindow.settings.CreaChiaveVersioneSelezionata))
            {
                savedSelected.Add(MainWindow.settings.CreaChiaveVersioneSelezionata);
            }

            // Build UI items cleanly
            VersioneItems.Clear();
            foreach (string v in available)
            {
                VersioneItems.Add(v);
            }

            // Pick the first saved selection that still exists, or default to the first available item
            SelectedVersione = savedSelected.FirstOrDefault(v => availableSet.Contains(v))
                               ?? available.FirstOrDefault();

            SaveSelectedVersion();
        }

        private void SaveSelectedVersion()
        {
            MostraPulsanteStato();
            MainWindow.settings.CreaChiaveVersioneSelezionata = SelectedVersione ?? "";
            if (!string.IsNullOrEmpty(SelectedVersione))
            {
                TestoTipi tipo = MainWindow.Testi.Info(SelectedVersione).Tipo;
                bool bibbiaComm = ((tipo & TestoTipi.Bibbia) == TestoTipi.Bibbia || (tipo & TestoTipi.Commentario) == TestoTipi.Commentario);
                labBrano.Visibility = bibbiaComm ? Visibility.Visible : Visibility.Collapsed;
                tbBrano.Visibility = bibbiaComm ? Visibility.Visible : Visibility.Collapsed;
            }
            App.Settings.Save(MainWindow.settings);

        }

        private void HelpFlyout_OnHelpClicked(object sender, RoutedEventArgs e)
        {
            MainWindow.MostraGuida((string)(Application.Current.TryFindResource("CreaChiaveTitolo") ?? "Create Concordance"));
        }

        private void MostraPulsanteStato()
        {
            if (!string.IsNullOrWhiteSpace(_selectedVersione))
            {
                PulCreaChiave.IsEnabled = true;

                GbTipo.Visibility = MainWindow.Testi.EsistonoRadici(_selectedVersione) ? Visibility.Visible : Visibility.Collapsed;
                CbEscludiParole.Visibility = GbTipo.Visibility;
                TbEscludiParole.Visibility = GbTipo.Visibility;
            }
            else
            {
                PulCreaChiave.IsEnabled = false;
            }

        }

        private async void CreaChiave_Click(object sender, RoutedEventArgs e)
        {
            string brano = tbBrano.Text;
            if (string.IsNullOrWhiteSpace(_selectedVersione))
                return;
            string versione = _selectedVersione;

            if (GbTipo.Visibility == Visibility.Collapsed)
            {
                IsTipoParole = true;
                _escludiParoleAbilitato = true;
            }

            int ordine = 0;
            if (IsOrdineNumerico)
                ordine = 1;
            else if (IsOrdinePrima)
                ordine = 2;

            await ChiaveInEditorAsync(brano, versione, IsTipoParole, _escludiParoleAbilitato, _paroleDaEscludereText, ordine, _numeroMinimo, _conRiferimenti);
        }

        public static async Task ChiaveInEditorAsync(
                    string brano,
                    string versione,
                    bool diParole,
                    bool nonRadiciComuni,
                    string radiciComuni,
                    int ordine,
                    int numeroMinimo,
                    bool conRiferimenti)
        {
            int itemsCount = diParole ? MainWindow.Testi.Parole(versione).Length : MainWindow.Testi.Radici(versione).Length;
            int limiteProgresso = (ordine != 0) ? itemsCount * 2 : itemsCount;

            // Parse common roots/words to exclude into a HashSet for fast lookup
            HashSet<string> radiciComuniSet = [];
            if (nonRadiciComuni && !string.IsNullOrWhiteSpace(radiciComuni))
            {
                radiciComuniSet = new HashSet<string>(
                    radiciComuni.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()),
                    StringComparer.OrdinalIgnoreCase
                );
            }

            string statusMsg = (string)(Application.Current.TryFindResource("CreaChiaveCreazione") ?? "Creating the concordance...");

            // 1. Start Status Task (auto-removed from UI via using/Dispose when method completes)
            using StatusTask status = StatusService.AvviaTask(statusMsg, isIndeterminate: false);

            // 2. Run computation off the UI thread
            string testoRtf = await Task.Run(() => CreaTestoDiChiave(
                brano,
                versione,
                diParole,
                nonRadiciComuni,
                radiciComuniSet,
                ordine,
                numeroMinimo,
                conRiferimenti,
                limiteProgresso,
                statusMsg,
                status
            ));

            // 3. Back on the UI thread: Render and load document in active editor
            string titoloFinestra = (string)(Application.Current.TryFindResource("CreaChiaveChiaveTitolo") ?? "Concordance of") + " ";
            if (string.IsNullOrEmpty(brano))
                titoloFinestra += MainWindow.Testi.Info(versione).Abbreviazione;
            else
                titoloFinestra += brano + " (" + MainWindow.Testi.Info(versione).Abbreviazione + ") ";

            FlowDocument doc = Texts.LoadRtfToFlowDocumentOnUiThread(testoRtf);
            Brush fg = (Brush)Application.Current.FindResource("AppForegroundBrush");
            RtfColorTransformer.ApplyThemeToDocument(doc, true, fg, true);

            App.DockingHost.SendFlowDocumentToActiveEditor(doc, titoloFinestra, versione);
        }

        private static string CreaTestoDiChiave(
            string brano,
            string versione,
            bool diParole,
            bool nonRadiciComuni,
            HashSet<string> radiciComuniSet,
            int ordine,
            int numeroMinimo,
            bool conRiferimenti,
            int limiteProgresso,
            string statusMsg,
            StatusTask status)
        {
            Riferimento branoDaMostrare = MainWindow.Testi.ConvertiRiferimento(brano);
            StringBuilder testo = new(MainWindow.Testi.RtfIntestazione());

            if (diParole)
            {
                int nParole = MainWindow.Testi.Parole(versione).Length;
                string[] parole = new string[nParole];
                Array.Copy(MainWindow.Testi.Parole(versione), parole, nParole);

                if (nonRadiciComuni)
                {
                    foreach (string radiceComune in radiciComuniSet)
                    {
                        Collection<string> paroleDiRadice = MainWindow.Testi.ParoleDiRadice(radiceComune, versione);
                        foreach (string parolaDiRadice in paroleDiRadice)
                        {
                            int idx = Array.IndexOf(parole, parolaDiRadice);
                            if (idx >= 0)
                                parole[idx] = "";
                        }
                    }
                }

                if (ordine == 1)
                {
                    ParolaApparenze[] paroleEApparenze = new ParolaApparenze[nParole];
                    string[] numeroApparenze = new string[nParole];
                    for (int i = 0; i < nParole; ++i)
                    {
                        paroleEApparenze[i].Parola = parole[i];
                        paroleEApparenze[i].Apparenze = MainWindow.Testi.RicercaParolaInBrano(parole[i], branoDaMostrare, versione);
                        numeroApparenze[i] = "0000000" + (9999999 - paroleEApparenze[i].Apparenze.Count).ToString(CultureInfo.InvariantCulture);
                        numeroApparenze[i] = numeroApparenze[i][^7..] + parole[i];

                        status.Update(statusMsg, (double)i / limiteProgresso * 100.0);
                    }
                    Array.Sort(numeroApparenze, paroleEApparenze);
                    for (int i = 0; i < nParole; ++i)
                    {
                        testo.Append(RigaDiChiave(paroleEApparenze[i].Parola, paroleEApparenze[i].Apparenze, numeroMinimo, conRiferimenti));
                        status.Update(statusMsg, (double)(nParole + i) / limiteProgresso * 100.0);
                    }
                }
                else if (ordine == 2)
                {
                    Riferimento[] apparenze = new Riferimento[nParole];
                    for (int i = 0; i < nParole; ++i)
                    {
                        apparenze[i] = MainWindow.Testi.RicercaParolaInBrano(parole[i], branoDaMostrare, versione);
                        status.Update(statusMsg, (double)i / limiteProgresso * 100.0);
                    }
                    Array.Sort(apparenze, parole, new Riferimento());
                    for (int i = 0; i < nParole; ++i)
                    {
                        testo.Append(RigaDiChiave(parole[i], apparenze[i], numeroMinimo, conRiferimenti));
                        status.Update(statusMsg, (double)(nParole + i) / limiteProgresso * 100.0);
                    }
                }
                else // Alfabetico
                {
                    for (int i = 0; i < nParole; ++i)
                    {
                        Riferimento apparenze = MainWindow.Testi.RicercaParolaInBrano(parole[i], branoDaMostrare, versione);
                        testo.Append(RigaDiChiave(parole[i], apparenze, numeroMinimo, conRiferimenti));
                        status.Update(statusMsg, (double)i / limiteProgresso * 100.0);
                    }
                }
                testo.Append('}');
            }
            else // Radici
            {
                int nRadici = MainWindow.Testi.Radici(versione).Length;
                string[] radici = new string[nRadici];
                Array.Copy(MainWindow.Testi.Radici(versione), radici, nRadici);

                if (nonRadiciComuni)
                {
                    foreach (string radiceComune in radiciComuniSet)
                    {
                        int idx = Array.IndexOf(radici, radiceComune);
                        if (idx >= 0)
                            radici[idx] = "";
                    }
                }

                int primaRadiceDaRicercare = (radici[0] == "*" ? 1 : 0);

                if (ordine == 1)
                {
                    ParolaApparenze[] radiciEApparenze = new ParolaApparenze[nRadici];
                    string[] numeroApparenze = new string[nRadici];
                    if (primaRadiceDaRicercare == 1)
                    {
                        radiciEApparenze[0].Parola = "*";
                        radiciEApparenze[0].Apparenze = new Riferimento();
                        numeroApparenze[0] = "9999999*";
                    }
                    for (int i = primaRadiceDaRicercare; i < nRadici; ++i)
                    {
                        radiciEApparenze[i].Parola = radici[i];
                        radiciEApparenze[i].Apparenze = MainWindow.Testi.RicercaRadiceInBrano(radici[i], branoDaMostrare, versione);
                        numeroApparenze[i] = "0000000" + (9999999 - radiciEApparenze[i].Apparenze.Count).ToString(CultureInfo.InvariantCulture);
                        numeroApparenze[i] = numeroApparenze[i][^7..] + radici[i];

                        status.Update(statusMsg, (double)i / limiteProgresso * 100.0);
                    }
                    Array.Sort(numeroApparenze, radiciEApparenze);
                    for (int i = 0; i < nRadici; ++i)
                    {
                        testo.Append(RigaDiChiave(radiciEApparenze[i].Parola, radiciEApparenze[i].Apparenze, numeroMinimo, conRiferimenti));
                        status.Update(statusMsg, (double)(nRadici + i) / limiteProgresso * 100.0);
                    }
                }
                else if (ordine == 2)
                {
                    Riferimento[] apparenze = new Riferimento[nRadici];
                    if (primaRadiceDaRicercare == 1)
                        apparenze[0] = new Riferimento();

                    for (int i = primaRadiceDaRicercare; i < nRadici; ++i)
                    {
                        apparenze[i] = MainWindow.Testi.RicercaRadiceInBrano(radici[i], branoDaMostrare, versione);
                        status.Update(statusMsg, (double)i / limiteProgresso * 100.0);
                    }
                    Array.Sort(apparenze, radici, new Riferimento());
                    for (int i = 0; i < nRadici; ++i)
                    {
                        testo.Append(RigaDiChiave(radici[i], apparenze[i], numeroMinimo, conRiferimenti));
                        status.Update(statusMsg, (double)(nRadici + i) / limiteProgresso * 100.0);
                    }
                }
                else // Alfabetico
                {
                    for (int i = primaRadiceDaRicercare; i < nRadici; ++i)
                    {
                        Riferimento apparenze = MainWindow.Testi.RicercaRadiceInBrano(radici[i], branoDaMostrare, versione);
                        testo.Append(RigaDiChiave(radici[i], apparenze, numeroMinimo, conRiferimenti));
                        status.Update(statusMsg, (double)i / limiteProgresso * 100.0);
                    }
                }
                testo.Append('}');
            }

            return testo.ToString();
        }

        private static string RigaDiChiave(string parola, Riferimento apparenze, int numeroMinimo, bool conRiferimenti)
        {
            if (apparenze.Count < numeroMinimo)
                return "";

            StringBuilder riga = new(parola);
            if (conRiferimenti)
            {
                riga.Append(" (").Append(apparenze.Count.ToString(CultureInfo.CurrentCulture)).Append("): ");
                if (apparenze.Versetti)
                {
                    string[] separatori = MainWindow.Testi.SeparatoriNeiRiferimenti();
                    foreach (byte[] brano in apparenze.Brani)
                        riga.Append(MainWindow.Testi.GetLibroAbbreviazioneUsata(brano[0])).Append(separatori[0]).Append(brano[1].ToString(CultureInfo.InvariantCulture)).Append(separatori[1]).Append(brano[2].ToString(CultureInfo.InvariantCulture)).Append(", ");
                }
                else
                {
                    foreach (string nota in apparenze.Note)
                    {
                        if (nota[0] == '#') // Nota su un brano
                            riga.Append(MainWindow.Testi.ConvertiTitoloNotaARiferimento(nota)).Append(", ");
                        else // Nota con un titolo
                            riga.Append(nota).Append(", ");
                    }
                }
            }
            else
            {
                riga.Append(": ").Append(apparenze.Count.ToString(CultureInfo.CurrentCulture));
            }

            string rigaStringa = riga.ToString();
            if (rigaStringa.EndsWith(", "))
                rigaStringa = rigaStringa[..^2];

            return rigaStringa + @"\par ";
        }
    }
}
