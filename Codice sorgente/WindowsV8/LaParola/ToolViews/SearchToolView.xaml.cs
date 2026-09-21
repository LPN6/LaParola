using LaParola.Utilities;
using System.Globalization;
using System.IO;
using System.Security.Principal;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace LaParola.ToolViews
{
    // TODO2 salva in lista versetti, cercare in lista versetti

    public partial class SearchToolView : UserControl
    {
        public SearchToolView()
        {
            InitializeComponent();

            AggiornaVersioniDisponibili();

            RicercaPulsanteStato();

            expWordPicker.IsExpanded = MainWindow.settings.RicercaScegliParolaAperta;
        }

        public void AggiornaVersioniDisponibili()
        {
            cbVersione.Items.Clear();
            string s = MainWindow.settings.RicercaTestoSelezionato;
            foreach (string v in MainWindow.Testi.NomiVersioni())
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
        }

        private void HelpFlyout_OnHelpClicked(object sender, RoutedEventArgs e)
        {
            MainWindow.MostraGuida((string)(Application.Current.TryFindResource("RicercaTitolo") ?? "Search"));
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

            App.DockingHost.OpenEditorDocument(doc, (string)(Application.Current.TryFindResource("RicercaEspressioneAiutoTitolo") ?? "Help for the Search Expression"), "");
        }

        private void Versione_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string versioneSelezionata = cbVersione.SelectedItem as string ?? "";
            MainWindow.settings.RicercaTestoSelezionato = versioneSelezionata;
            TestoTipi tipoSelezionata = MainWindow.Testi.Info(versioneSelezionata).Tipo;
            if ((tipoSelezionata & TestoTipi.Commentario) == TestoTipi.Commentario || (tipoSelezionata & TestoTipi.Bibbia) == TestoTipi.Bibbia)
            {
                GbBrano.Visibility = Visibility.Visible;
            }
            else
            {
                GbBrano.Visibility = Visibility.Collapsed;
            }
            PopulateWordAndRootLists();
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

            string abbVersione = MainWindow.Testi.Info(versioneSelezionata)?.Abbreviazione ?? "";

            if (!String.IsNullOrEmpty(abbVersione))
                abbVersione = " (" + abbVersione + ")";

            string title = espressione + abbVersione;

            string branoDaRicercare = "";
            if (GbBrano.Visibility == Visibility.Collapsed || rbBrano.IsChecked == true)
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
                doc.Tag = versioneSelezionata;

                Brush fg = (Brush)Application.Current.FindResource("AppForegroundBrush");
                RtfColorTransformer.ApplyThemeToDocument(doc, true, fg, true);

                App.DockingHost.SendFlowDocumentToActiveEditor(doc, title, versioneSelezionata);
            }
        }

        private void ExpWordPicker_StateChanged(object sender, RoutedEventArgs e)
        {
            // Read current state
            bool isOpen = expWordPicker.IsExpanded;

            // Save to app settings
            MainWindow.settings.RicercaScegliParolaAperta = isOpen;
        }

        #region Scegli Parola / Radice
        #region Word & Root Helper Data Loading

        private void PopulateWordAndRootLists()
        {
            if (cbVersione.SelectedValue is string selectedVersion)
            {
                // Load words into Tab 1
                lbParole.ItemsSource = MainWindow.Testi.Parole(selectedVersion);

                // Load roots into Tab 2 (Column 1)
                lbRadici.ItemsSource = MainWindow.Testi.Radici(selectedVersion);

                // Clear word-by-root list
                lbParoleRadice.ItemsSource = null;

                bool radici = (MainWindow.Testi.Radici(selectedVersion).Length > 0);
                tabRadici.Visibility = radici ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        #endregion

        #region Prefix Search Logic

        private static void FilterListBoxByPrefix(TextBox filterBox, ListBox listBox)
        {
            string filterText = filterBox.Text.Trim();
            if (listBox.ItemsSource == null) return;
            if (string.IsNullOrEmpty(filterText))
            {
                listBox.SelectedIndex = -1;
                return;
            }

            foreach (object? item in listBox.ItemsSource)
            {
                string itemText = item?.ToString() ?? "";
                if (itemText.StartsWith(filterText, StringComparison.CurrentCultureIgnoreCase))
                {
                    listBox.SelectedItem = item;
                    listBox.ScrollIntoView(item);
                    break;
                }
            }
        }

        private void TbFiltroParole_TextChanged(object sender, TextChangedEventArgs e)
            => FilterListBoxByPrefix(tbFiltroParole, lbParole);

        private void TbFiltroRadici_TextChanged(object sender, TextChangedEventArgs e)
            => FilterListBoxByPrefix(tbFiltroRadici, lbRadici);

        private void TbFiltroParoleRadice_TextChanged(object sender, TextChangedEventArgs e)
            => FilterListBoxByPrefix(tbFiltroParoleRadice, lbParoleRadice);

        #endregion

        #region Selection Changed Handlers

        private void LbParole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbVersione.SelectedValue is string selectedVersion)
            {
                bool hasSelection = lbParole.SelectedItem != null;
                btnAggiungiParola.IsEnabled = hasSelection;

                if (hasSelection)
                {
                    string selectedWord = lbParole.SelectedItem?.ToString() ?? "";
                    lblOccorrenzeParola.Text = $"{Application.Current.TryFindResource("RicercaOccorrenze") ?? "Appearances:"} {MainWindow.Testi.NumeroVolteParola(selectedWord, selectedVersion)}";
                    lblRadiceParola.Text = $"{Application.Current.TryFindResource("RicercaRadice") ?? "Root:"} {MainWindow.Testi.RadiceDiParola(selectedWord, selectedVersion)}";
                }
                else
                {
                    lblOccorrenzeParola.Text = (string)(Application.Current.TryFindResource("RicercaOccorrenze") ?? "Appearances") + " -";
                    lblRadiceParola.Text = (string)(Application.Current.TryFindResource("RicercaRadice") ?? "Root") + " -";
                }
            }
        }

        private void LbRadici_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbVersione.SelectedValue is string selectedVersion)
            {
                lbParoleRadice.ItemsSource = null;

                bool hasSelection = lbRadici.SelectedItem != null;
                btnAggiungiRadice.IsEnabled = hasSelection;

                if (hasSelection)
                {
                    string selectedRoot = lbRadici.SelectedItem?.ToString() ?? "";
                    lblOccorrenzeRadice.Text = $"{Application.Current.TryFindResource("RicercaOccorrenze") ?? "Appearances:"} {MainWindow.Testi.NumeroVolteRadice(selectedRoot, selectedVersion)}";
                    lblParoleConRadiceHeader.Text = $"{Application.Current.TryFindResource("RicercaParoleConRadice") ?? "Words with Root:"} {MainWindow.Testi.ParoleDiRadice(selectedRoot, selectedVersion).Count}";

                    lbParoleRadice.ItemsSource = MainWindow.Testi.ParoleDiRadice(selectedRoot, selectedVersion);
                    if (lbParoleRadice.Items.Count > 0)
                        lbParoleRadice.SelectedIndex = 0;
                }
                else
                {
                    lblOccorrenzeRadice.Text = (string)(Application.Current.TryFindResource("RicercaOccorrenze") ?? "Appearances") + " -";
                    lbParoleRadice.ItemsSource = null;
                }
            }
        }

        private void LbParoleRadice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbVersione.SelectedValue is string selectedVersion)
            {
                bool hasSelection = lbParoleRadice.SelectedItem != null;
                btnAggiungiParolaRadice.IsEnabled = hasSelection;

                if (hasSelection)
                {
                    string selectedWord = lbParoleRadice.SelectedItem?.ToString() ?? "";
                    lblOccorrenzeParolaRadice.Text = $"{Application.Current.TryFindResource("RicercaOccorrenze") ?? "Appearances:"} {MainWindow.Testi.NumeroVolteParola(selectedWord, selectedVersion)}";
                }
                else
                {
                    lblOccorrenzeParolaRadice.Text = (string)(Application.Current.TryFindResource("RicercaOccorrenze") ?? "Appearances") + " -"; ;
                }
            }
        }

        #endregion

        #region Add to Expression Logic

        private void AppendToExpression(string? parola)
        {
            if (string.IsNullOrWhiteSpace(parola)) return;

            if (parola.IndexOfAny(['0', '1', '2', '3', '4', '5', '6', '7', '8', '9']) >= 0 && parola.IndexOfAny(['<', '>']) == -1)
                parola = "<" + parola + ">";

            if (string.IsNullOrWhiteSpace(tbEspressione.Text))
            {
                tbEspressione.Text = parola;
            }
            else
            {
                char c = tbEspressione.Text[^1];
                if (c != ' ' && c != '(' && c != '[' && c != '/' && c != '\\' && c != '~' && c != '^')
                    tbEspressione.Text += " ";
                tbEspressione.Text += parola;
            }

            RicercaPulsanteStato();
            tbEspressione.Focus();
            tbEspressione.CaretIndex = tbEspressione.Text.Length;
        }

        private void BtnAggiungiParola_Click(object sender, RoutedEventArgs e)
        {
            if (lbParole.SelectedItem != null)
                AppendToExpression(lbParole.SelectedItem.ToString());
        }

        private void BtnAggiungiRadice_Click(object sender, RoutedEventArgs e)
        {
            if (lbRadici.SelectedItem != null)
                AppendToExpression(lbRadici.SelectedItem.ToString());
        }

        private void BtnAggiungiParolaRadice_Click(object sender, RoutedEventArgs e)
        {
            if (lbParoleRadice.SelectedItem != null)
                AppendToExpression(lbParoleRadice.SelectedItem.ToString());
        }

        private void LbParole_MouseDoubleClick(object sender, MouseButtonEventArgs e)
            => BtnAggiungiParola_Click(sender, e);

        private void LbRadici_MouseDoubleClick(object sender, MouseButtonEventArgs e)
            => BtnAggiungiRadice_Click(sender, e);

        private void LbParoleRadice_MouseDoubleClick(object sender, MouseButtonEventArgs e)
            => BtnAggiungiParolaRadice_Click(sender, e);

        #endregion

        #endregion
    }
}
