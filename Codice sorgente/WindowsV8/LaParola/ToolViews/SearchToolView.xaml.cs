using LaParola.Utilities;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace LaParola.ToolViews
{
    // TODO2: scegliere parola
    // salva in lista versetti, cercare in lista versetti
    // anche commentari dizionari, libri

    public partial class SearchToolView : UserControl
    {
        public SearchToolView()
        {
            InitializeComponent();

            string s = MainWindow.settings.RicercaTestoSelezionato;
            foreach (string v in MainWindow.Testi.NomiVersioni(TestoTipi.Bibbia))
            {
                cbVersione.Items.Add(v);
                if (v == s)
                {
                    cbVersione.SelectedIndex = cbVersione.Items.Count - 1;
                }
            }

            if (cbVersione.Items.Count > 0)
            {
                if (cbVersione.SelectedIndex == -1)
                    cbVersione.SelectedIndex = 0;
            }

            RicercaPulsanteStato();
        }

        private void HelpFlyout_OnHelpClicked(object sender, RoutedEventArgs e)
        {
            // TODO2: Open correct help section
            MessageBox.Show("Open Help Centre");
        }

        private void AiutoEspressione_Click(object sender, RoutedEventArgs e)
        {
            FlowDocument doc = new()
            {
                FontFamily = new FontFamily("Georgia"),
                FontSize = 14
            };
            string testo = (string)(Application.Current.TryFindResource("RicercaEspressioneAiutoLungo") ?? "Help");

            TextRange range = new(doc.ContentStart, doc.ContentEnd);
            using MemoryStream ms = new(Encoding.UTF8.GetBytes(testo));
            range.Load(ms, DataFormats.Rtf);

            App.DockingHost.OpenEditorDocument(doc, (string)(Application.Current.TryFindResource("RicercaEspressioneAiutoTitolo") ?? "Help for the Search Expression"));
        }

        private void Versione_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainWindow.settings.RicercaTestoSelezionato = cbVersione.SelectedItem as string ?? "";
        }

        private void Espressione_KeyUp(object sender, KeyEventArgs e)
        {
            RicercaPulsanteStato();
        }

        private void RicercaPulsanteStato()
        {
            btnRicerca.IsEnabled = !string.IsNullOrWhiteSpace(tbEspressione.Text) && !string.IsNullOrEmpty(cbVersione.SelectedItem as string);
        }

        private async void Generate_Click(object sender, RoutedEventArgs e)
        {
            string espressione = tbEspressione.Text;
            string versioneSelezionata = cbVersione.SelectedItem as string ?? "";
            if (string.IsNullOrEmpty(versioneSelezionata) || string.IsNullOrEmpty(espressione))
            {
                return;
            }

            string abbVersioni = MainWindow.Testi.Info(versioneSelezionata)?.Abbreviazione ?? "";

            if (!String.IsNullOrEmpty(abbVersioni))
                abbVersioni = " (" + abbVersioni + ")";

            string title = espressione + abbVersioni;

            string branoDaRicercare = "";
            if (rbBrano.IsChecked == true)
            {
                branoDaRicercare = tbBrano.Text;
            }
            else
            {
                int sezione = cbSezione.SelectedIndex;
                switch (sezione)
                {
                    case 0:
                        branoDaRicercare = "";
                        break;
                    case 1: // AT
                        branoDaRicercare = MainWindow.Testi.GetLibroNome(1) + "-" + MainWindow.Testi.GetLibroNome(46);
                        break;
                    case 2: // NT
                        branoDaRicercare = MainWindow.Testi.GetLibroNome(47) + "-" + MainWindow.Testi.GetLibroNome(73);
                        break;
                    case 3: // Pentateuco
                        branoDaRicercare = MainWindow.Testi.GetLibroNome(1) + "-" + MainWindow.Testi.GetLibroNome(5);
                        break;
                    case 4: // storici
                        branoDaRicercare = MainWindow.Testi.GetLibroNome(1) + "-" + MainWindow.Testi.GetLibroNome(21);
                        break;
                    case 5: // scritti
                        branoDaRicercare = MainWindow.Testi.GetLibroNome(22) + "-" + MainWindow.Testi.GetLibroNome(28);
                        break;
                    case 6: // profeti
                        branoDaRicercare = MainWindow.Testi.GetLibroNome(29) + "-" + MainWindow.Testi.GetLibroNome(46);
                        break;
                    case 7: // Vangeli
                        branoDaRicercare = MainWindow.Testi.GetLibroNome(47) + "-" + MainWindow.Testi.GetLibroNome(50);
                        break;
                    case 8: // Vangeli e Atti
                        branoDaRicercare = MainWindow.Testi.GetLibroNome(47) + "-" + MainWindow.Testi.GetLibroNome(51);
                        break;
                    case 9: // lettere
                        branoDaRicercare = MainWindow.Testi.GetLibroNome(52) + "-" + MainWindow.Testi.GetLibroNome(73);
                        break;
                    case 10: // lettere di Paolo
                        branoDaRicercare = MainWindow.Testi.GetLibroNome(52) + "-" + MainWindow.Testi.GetLibroNome(64);
                        break;
                    case 11: // lettere di altri
                        branoDaRicercare = MainWindow.Testi.GetLibroNome(65) + "-" + MainWindow.Testi.GetLibroNome(73);
                        break;
                    default: // cerca in tutto il testo
                        break;
                }
            }

            Riferimento? versettiConFrase = null;
            try
            {
                versettiConFrase = MainWindow.Testi.Ricerca(espressione, branoDaRicercare, versioneSelezionata);
            }
            catch (SearchParenthesesException)
            {
                MessageBoxLPN.Show(Window.GetWindow(this), (string)(Application.Current.TryFindResource("RicercaErroreParentesi") ?? "The parentheses in the search expression are not balanced."), (string)(Application.Current.TryFindResource("Errore") ?? "Error"));
            }
            catch (SearchBracketsException)
            {
                MessageBoxLPN.Show(Window.GetWindow(this), (string)(Application.Current.TryFindResource("RicercaErroreParentesiQuadrate") ?? "The square brackets in the search expression are not balanced."), (string)(Application.Current.TryFindResource("Errore") ?? "Error"));
            }
            catch (SearchSyntaxErrorException ex)
            {
                MessageBoxLPN.Show(Window.GetWindow(this), String.Format(CultureInfo.InvariantCulture, (string)(Application.Current.TryFindResource("RicercaErroreSintasi") ?? "The syntax of the search expression is incorrect at about character number {0}."), ex.Message), (string)(Application.Current.TryFindResource("Errore") ?? "Error"));
            }

            if (versettiConFrase != null)
            {
                FlowDocument doc = await MainWindow.Testi.FlowDocumentBranoAsync(versettiConFrase, versioneSelezionata);

                //if (Services.ThemeManager.IsDark(MainWindow.settings.ThemeMode))
                //{
                Brush fg = (Brush)Application.Current.FindResource("AppForegroundBrush");
                RtfColorTransformer.ApplyThemeToDocument(doc, true, fg, true);
                //}

                App.DockingHost.SendFlowDocumentToActive(doc, title);
            }
        }
    }
}
