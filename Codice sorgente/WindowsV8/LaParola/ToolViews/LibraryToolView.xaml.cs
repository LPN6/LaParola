using AvalonDock.Layout;
using LaParola.Dialogs;
using LaParola.DocumentViews;
using LaParola.Utilities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

namespace LaParola.ToolViews
{
    // TODO2 for each text, buttons to: copiare, unire 2, cambia sola lettura, esportare, creare file unico, aggiungere radici
    // TODO2 in general, button to: create nuova collezione

    /// <summary>
    /// Logica di interazione per LibraryToolView.xaml
    /// </summary>
    public partial class LibraryToolView : UserControl
    {
        private ICollectionView _booksView;

        public LibraryToolView()
        {
            InitializeComponent();

            // 1. Build list collection frameworks & setup _booksView proxy
            _booksView = CollectionViewSource.GetDefaultView(Array.Empty<VersioneInformazioni>());
            LoadLibraryData();

            // 2. Map stored settings variables directly onto the form controls
            RestoreLibraryState();

            // 3. Bind runtime interactions to your tracking method
            AttachStateTrackers();
        }

        private void LoadLibraryData()
        {
            try
            {
                // 1. Get the list of book keys/identifiers (e.g., ["LND", "Rive", "KJV"])
                Collection<string> nomiVersioni = MainWindow.Testi.NomiVersioni();

                // 2. Use LINQ to instantly fetch the info object for every book name
                List<VersioneInformazioni> libri = [.. nomiVersioni.Select(nome => MainWindow.Testi.Info(nome))];

                // 3. Bind the resulting collection straight to your UI DataGrid
                BooksDataGrid.ItemsSource = libri;

                // Build your Language collection dynamically based on actual book values
                PopulateLanguageOptions(libri);

                // Get the collection view wrapping the DataGrid's items source
                _booksView = CollectionViewSource.GetDefaultView(BooksDataGrid.ItemsSource);

                // Define the filter matching logic
                _booksView.Filter = FilterBooks;
            }
            catch (Exception ex)
            {
                // Soft fallback / log mapping errors if any book fails to parse
                System.Diagnostics.Debug.WriteLine($"Error loading library: {ex.Message}");
            }
        }

        private void PopulateLanguageOptions(List<VersioneInformazioni> libri)
        {
            // Extract unique root language tags across all books
            var rootLanguages = libri
                .Where(b => !string.IsNullOrWhiteSpace(b.Lingua))
                .SelectMany(b => GetBaseLanguageCodes(b.Lingua))
                .Distinct()
                .OrderBy(lang => lang);

            foreach (var lang in rootLanguages)
            {
                LinguaFilterComboBox.Items.Add(new ComboBoxItem { Content = lang.ToUpperInvariant(), Tag = lang });
            }
        }

        private static IEnumerable<string> GetBaseLanguageCodes(string rawLanguage)
        {
            if (string.IsNullOrWhiteSpace(rawLanguage)) return [];

            return rawLanguage.Split('|', StringSplitOptions.RemoveEmptyEntries)
                              .Select(part => part.Split('-')[0].Trim().ToLowerInvariant());
        }

        private void HelpFlyout_OnHelpClicked(object sender, RoutedEventArgs e)
        {
            // TODO2: Open correct help section
            MessageBox.Show("Open Help Centre");
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Tell the proxy view to re-run the FilterBooks check on all items
            _booksView?.Refresh();
        }

        // evaluates every row in the background
        private bool FilterBooks(object item)
        {
            if (item is not VersioneInformazioni info) return false;

            // 1. TEXT FILTER
            if (SearchTextBox != null && !string.IsNullOrWhiteSpace(SearchTextBox.Text) && SearchTextBox.Text != (string)(Application.Current.TryFindResource("BibliotecaRicercaHint") ?? "Search..."))
            {
                string txt = SearchTextBox.Text.Trim();
                bool matchText = (info.Nome?.Contains(txt, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
                                 (info.Autore?.Contains(txt, StringComparison.CurrentCultureIgnoreCase) ?? false) ||
                                 (info.Abbreviazione?.Contains(txt, StringComparison.CurrentCultureIgnoreCase) ?? false);
                if (!matchText) return false;
            }

            // 2. TIPO FILTER
            if (TipoFilterComboBox?.SelectedItem is ComboBoxItem selectedTipoItem)
            {
                string? tipoTag = selectedTipoItem.Tag.ToString();
                if (tipoTag != null && tipoTag != "All" && Enum.TryParse(typeof(TestoTipi), tipoTag, out var match) && match is TestoTipi selectedEnumFlag)
                {
                    if (!info.Tipo.HasFlag(selectedEnumFlag)) return false;
                }
            }

            // 3. LINGUA FILTER
            if (LinguaFilterComboBox?.SelectedItem is ComboBoxItem selectedLangItem)
            {
                string? langTag = selectedLangItem.Tag.ToString();
                if (langTag != null && langTag != "All" && !GetBaseLanguageCodes(info.Lingua).Contains(langTag)) return false;
            }

            // 4. FIXED DATA (YEAR) FILTER
            if (DataConditionComboBox?.SelectedItem is ComboBoxItem selectedDataCondition)
            {
                string? condition = selectedDataCondition.Tag.ToString();
                if (condition != null && condition != "Any")
                {
                    string typedYearStr = FilterYearTextBox.Text.Trim();

                    // UX Optimization: While the user is still typing (e.g., they only typed "1" or "19"), 
                    // don't turn the grid completely blank. Wait until they type a valid 4-digit year.
                    if (typedYearStr.Length < 4) return true;

                    if (int.TryParse(typedYearStr, out int targetYear))
                    {
                        // If a book doesn't have a date but a filter is active, hide it
                        if (string.IsNullOrWhiteSpace(info.Data)) return false;

                        int? bookYear = ExtractYear(info.Data);
                        if (bookYear == null) return false; // Hide if we can't parse a year out of it

                        // Execute straightforward mathematical integer evaluations
                        if (condition == "Before" && bookYear.Value >= targetYear) return false;
                        if (condition == "After" && bookYear.Value <= targetYear) return false;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void FilterYearTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            // Block the character if it isn't a digit
            e.Handled = !e.Text.All(char.IsDigit);
        }

        private void FilterYearTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            // Safeguard against clipboard pasting bypasses
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!text.All(char.IsDigit))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        // Helper method: Extracts the first 4-digit number found in the data string
        private static int? ExtractYear(string dataStr)
        {
            if (string.IsNullOrWhiteSpace(dataStr)) return null;

            // Matches standalone 3 OR 4 digit numbers.
            // Catches "350" in "A.D. 350", "384" in "384-405", and skips "15" and "03" to grab "2006" in full dates.
            var match = RegExAnno().Match(dataStr);
            if (match.Success && int.TryParse(match.Value, out int year))
            {
                return year;
            }
            return null;
        }

        private void AddBookButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO2 Launch your file picker or installer dialog here - bisogna visibility=visible on the button also
        }

        // Triggers filter recalculation on generic combo changes
        private void FilterControl_SelectionChanged(object sender, SelectionChangedEventArgs e) => _booksView?.Refresh();
        private void FilterControl_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => _booksView?.Refresh();

        private void DataConditionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilterYearTextBox == null) return;

            var selectedTag = (DataConditionComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString();

            // Show the year input only if Before or After is selected
            FilterYearTextBox.Visibility = (selectedTag == "Before" || selectedTag == "After") ? Visibility.Visible : Visibility.Collapsed;

            _booksView?.Refresh();
        }

        // Triggered instantly whenever the user types or deletes a digit in the year box
        private void FilterYearTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _booksView?.Refresh();
        }

        private void BooksDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Ensure the user double-clicked an actual row, not empty space or column headers
            if (BooksDataGrid.SelectedItem is VersioneInformazioni selectedBook && selectedBook != null)
            {
                dynamic book = selectedBook;
                string testo = book.Nome;
                TestoTipi tt = book.Tipo;

                if (!string.IsNullOrEmpty(testo))
                {
                    if ((tt & TestoTipi.Bibbia) == TestoTipi.Bibbia)
                        MainWindow.VisualizzaBibbia(testo);
                    else if ((tt & TestoTipi.Commentario) == TestoTipi.Commentario)
                        MainWindow.VisualizzaCommentario(testo);
                    // TODO2 dizionari e libri - come decidere se aprire comm / diz / libri se tutti e tre?
                }
            }
        }

        private void ButtonCancella_Click(object sender, RoutedEventArgs e)
        {
            // Get the currently selected item from the DataGrid
            if (BooksDataGrid.SelectedItem is VersioneInformazioni selectedBook)
            {
                // Safety First: Ask the user to confirm disk deletion
                string messageTemplate = (string)(Application.Current.TryFindResource("BibliotecaCancellaDomanda") ?? "Are you sure you want to permanently delete '{0}' from your disk?\\n(Se è un file di LaParola, potrà essere riscaricato e reinstallato.)");
                string messaggio = string.Format(messageTemplate, selectedBook.Nome).Replace("\\n", "\n");
                MessageBoxResult confirmResult =
        MessageBoxLPN.Show(Window.GetWindow(this),
        messaggio,
        (string)(Application.Current.TryFindResource("BibliotecaCancellaTitolo") ?? "Confirm Deletion"),
        MessageBoxButton.YesNo);

                if (confirmResult == MessageBoxResult.Yes)
                {
                    // Pass the identifier field to your backend method
                    MainWindow.Testi.CancellaTesto(selectedBook.Nome);

                    // 2. Refresh the UI by reloading the data structure
                    LoadLibraryData();

                    RimuoviTestiDaApp(selectedBook.Nome);
                }
            }
            else
            {
                // Optional: Alert the user if they clicked delete without selecting a row
                //MessageBox.Show("Please select a book from the list to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ButtonRinomina_Click(object sender, RoutedEventArgs e)
        {
            if (BooksDataGrid.SelectedItem is VersioneInformazioni selectedBook)
            {
                // Initialize generic dialog with structural text definitions
                var dialog = new InputDialog(
                    prompt: "Please enter the new name for this library text source:",
                    windowTitle: "Rename Book",
                    suggestion: selectedBook.Nome
                )
                {
                    Owner = Window.GetWindow(this)
                };

                // Open the dialog modal
                if (dialog.ShowDialog() == true)
                {
                    string cleanNewName = dialog.InputText.Trim();

                    // Perform specific validation check directly inside the caller workflow
                    if (string.IsNullOrWhiteSpace(cleanNewName) || cleanNewName == selectedBook.Nome)
                    {
                        //MessageBox.Show("The text name cannot be empty. Operational change canceled.", "Validation Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // 4. Execution if validation criteria succeeded
                    MainWindow.Testi.RinominaTesto(selectedBook.Nome, cleanNewName);

                    LoadLibraryData();

                    RimuoviTestiDaApp(selectedBook.Nome);
                    // chiama AggiornaListeTesti per anche aggiungere con il nuovo nome
                }
            }
        }

        private static void RimuoviTestiDaApp(string nome)
        {
            List<LayoutDocument>? viewers = Funzioni.ListViewerDocuments();

            if (viewers != null)
            {
                foreach (LayoutDocument d in viewers)
                {
                    if (d.Content is ViewerDocumentView vd && vd.Versione == nome)
                    {
                        d.Close();
                    }
                }
            }

            AggiornaListeTesti();
        }

        private static void AggiornaListeTesti()
        {
            // Find the TextGenerator and Search tool window via its ContentId and refresh its layout
            if (Application.Current.MainWindow is MainWindow mw)
            {
                if (mw.FindName("Dock") is AvalonDock.DockingManager dock)
                {
                    LayoutAnchorable? textGenAnchorable = dock.Layout.Descendents()
                        .OfType<LayoutAnchorable>()
                        .FirstOrDefault(a => a.ContentId == "tool.textgen");

                    if (textGenAnchorable?.Content is TextGeneratorToolView textGenView)
                    {
                        textGenView.AggiornaVersioniDisponibili();
                    }

                    LayoutAnchorable? searchAnchorable = dock.Layout.Descendents()
                        .OfType<LayoutAnchorable>()
                        .FirstOrDefault(a => a.ContentId == "tool.search");

                    if (searchAnchorable?.Content is SearchToolView searchView)
                    {
                        searchView.AggiornaVersioniDisponibili();
                    }

                    LayoutDocument? opzioniDocument = dock.Layout.Descendents()
                        .OfType<LayoutDocument>()
                        .FirstOrDefault(a => a.ContentId == "tool.options");

                    if (opzioniDocument?.Content is OptionsToolView opzioniView)
                    {
                        opzioniView.InitializeBiblePreferences();
                    }
                }

                mw.AggiornaMenuVisualizza();
            }

        }

        private void SaveLibraryState()
        {
            if (MainWindow.settings?.LibraryState == null) return;
            var state = MainWindow.settings.LibraryState;

            // 1. Text & Filters
            state.SearchText = SearchTextBox.Text;
            state.SelectedTipoFilter = (TipoFilterComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "All";
            state.SelectedLinguaFilter = (LinguaFilterComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "All";
            state.SelectedDataCondition = (DataConditionComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString() ?? "Any";
            state.FilterYearText = FilterYearTextBox.Text;

            // 2. Splitter Position (Assuming your column layout is named 'RightPaneColumn')
            if (RightPaneColumn != null)
            {
                state.SplitterPosition = RightPaneColumn.ActualWidth;
            }

            // 3. DataGrid Column Widths
            state.ColumnStates = [.. BooksDataGrid.Columns.Select(c => new ColumnState
            {
                Header = c.Header?.ToString() ?? "",
                Width = c.ActualWidth
            })];

            // 4. DataGrid Sorting Column
            DataGridColumn? sortedColumn = BooksDataGrid.Columns.FirstOrDefault(c => c.SortDirection != null);
            if (sortedColumn != null)
            {
                state.SortColumnMemberPath = sortedColumn.SortMemberPath;
                state.SortDirection = sortedColumn.SortDirection.ToString() ?? string.Empty;
            }
            else
            {
                state.SortColumnMemberPath = string.Empty;
                state.SortDirection = string.Empty;
            }
        }

        private void RestoreLibraryState()
        {
            var state = MainWindow.settings?.LibraryState;
            if (state == null) return;

            // 1. Text & Year Text
            SearchTextBox.Text = state.SearchText;
            FilterYearTextBox.Text = state.FilterYearText;

            // 2. Dropdown Filter Items (Finds and selects by matching Tag string)
            SetComboBoxByTag(TipoFilterComboBox, state.SelectedTipoFilter);
            SetComboBoxByTag(LinguaFilterComboBox, state.SelectedLinguaFilter);
            SetComboBoxByTag(DataConditionComboBox, state.SelectedDataCondition);

            // 3. Splitter Position
            if (RightPaneColumn != null && state.SplitterPosition > 0)
            {
                RightPaneColumn.Width = new GridLength(state.SplitterPosition, GridUnitType.Pixel);
            }

            // 4. DataGrid Column Widths
            if (state.ColumnStates != null && state.ColumnStates.Count > 0)
            {
                foreach (var colState in state.ColumnStates)
                {
                    var column = BooksDataGrid.Columns.FirstOrDefault(c => c.Header?.ToString() == colState.Header);
                    if (column != null)
                    {
                        column.Width = new DataGridLength(colState.Width);
                    }
                }
            }

            // 5. DataGrid Sorting Rules
            if (!string.IsNullOrEmpty(state.SortColumnMemberPath) && _booksView != null)
            {
                var column = BooksDataGrid.Columns.FirstOrDefault(c => c.SortMemberPath == state.SortColumnMemberPath);
                if (column != null)
                {
                    var dir = state.SortDirection == "Ascending" ? ListSortDirection.Ascending : ListSortDirection.Descending;
                    column.SortDirection = dir;

                    _booksView.SortDescriptions.Clear();
                    _booksView.SortDescriptions.Add(new SortDescription(state.SortColumnMemberPath, dir));
                }
            }

            _booksView?.Refresh();
        }

        // look through combo boxes matching your tag rules
        private static void SetComboBoxByTag(ComboBox comboBox, string tagValue)
        {
            foreach (ComboBoxItem item in comboBox.Items)
            {
                if (item.Tag?.ToString() == tagValue)
                {
                    comboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void AttachStateTrackers()
        {
            // Textboxes & Filters
            SearchTextBox.TextChanged += (s, e) => SaveLibraryState();
            FilterYearTextBox.TextChanged += (s, e) => SaveLibraryState();
            TipoFilterComboBox.SelectionChanged += (s, e) => SaveLibraryState();
            LinguaFilterComboBox.SelectionChanged += (s, e) => SaveLibraryState();
            DataConditionComboBox.SelectionChanged += (s, e) => SaveLibraryState();

            // Splitter Drag tracking (Saves only *after* user stops moving it)
            LibraryGridSplitter.DragCompleted += (s, e) => SaveLibraryState();

            // DataGrid Column Changes (Catches column resizes on header thumb release)
            BooksDataGrid.AddHandler(Thumb.DragCompletedEvent, new DragCompletedEventHandler((s, e) => SaveLibraryState()), true);

            // DataGrid Sort Event 
            BooksDataGrid.Sorting += (s, e) =>
            {
                // Use a minor dispatcher delay to allow WPF to apply the sort direction 
                // to the column object *before* we run our state saving routine
                Dispatcher.BeginInvoke(new Action(() => SaveLibraryState()), System.Windows.Threading.DispatcherPriority.Background);
            };
        }

        [System.Text.RegularExpressions.GeneratedRegex(@"\b\d{3,4}\b")]
        private static partial System.Text.RegularExpressions.Regex RegExAnno();
    }

    public class ValuesAreEqualConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // If we don't have exactly two properties to check, don't collapse anything
            if (values == null || values.Length < 2) return false;

            // Handle potential null values safely
            if (values[0] == null || values[1] == null) return false;

            // Check if Titolo equals Nome
            return values[0].ToString() == values[1].ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            // Not implemented, as we don't need to convert back in this scenario
            return [];
        }
    }

    public static class RichTextBoxHelper
    {
        public static readonly DependencyProperty BindableDescriptionProperty =
            DependencyProperty.RegisterAttached(
                "BindableDescription",
                typeof(string),
                typeof(RichTextBoxHelper),
                new PropertyMetadata(null, OnBindableDescriptionChanged));

        public static string GetBindableDescription(DependencyObject obj) => (string)obj.GetValue(BindableDescriptionProperty);
        public static void SetBindableDescription(DependencyObject obj, string value) => obj.SetValue(BindableDescriptionProperty, value);

        private static void OnBindableDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RichTextBox rtb)
            {
                // Clear out previous content first
                rtb.Document.Blocks.Clear();

                string? text = e.NewValue as string;
                if (string.IsNullOrWhiteSpace(text)) return;

                try
                {
                    var range = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
                    using var ms = new MemoryStream(Encoding.UTF8.GetBytes(text));
                    // Check if the text header declares it's an RTF document
                    if (text.TrimStart().StartsWith("{\\rtf", StringComparison.OrdinalIgnoreCase))
                    {
                        range.Load(ms, DataFormats.Rtf);
                    }
                    else
                    {
                        range.Load(ms, DataFormats.Text);
                    }
                }
                catch
                {
                    // Safe fallback: If the RTF is corrupted, display it as raw text
                    rtb.AppendText(text);
                }
            }
        }
    }

    public class TestoTipoToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TestoTipi tipo && parameter is string targetFlagStr)
            {
                // Converte il parametro stringa del XAML nell'enum corrispondente
                if (Enum.TryParse(typeof(TestoTipi), targetFlagStr, out var result) && result is TestoTipi flag)
                {
                    // Verifica se il bit dell'enum è attivo (es: tipo contiene Bibbia?)
                    return tipo.HasFlag(flag) ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Not implemented, as we don't need to convert back in this scenario
            return DependencyProperty.UnsetValue;
        }
    }

    public class TestoTipiLocalizationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TestoTipi tipi)
            {
                if (tipi == TestoTipi.None)
                    return string.Empty;

                var localizedNames = new List<string>();

                // Dynamically loop through all flags inside the enum set
                foreach (TestoTipi flag in Enum.GetValues(typeof(TestoTipi)))
                {
                    if (flag == TestoTipi.None) continue;

                    if (tipi.HasFlag(flag))
                    {
                        // Matches your existing keys: "BibliotecaFiltroTipoBibbia", etc.
                        string resourceKey = $"BibliotecaFiltroTipo{flag}";

                        // Look up the string in the active localized ResourceDictionary
                        var localizedString = Application.Current.TryFindResource(resourceKey)?.ToString();

                        // Fallback to the raw enum name if the resource isn't found
                        localizedNames.Add(localizedString ?? flag.ToString());
                    }
                }

                // Join combined flags with a comma (e.g., "Bible, Commentary")
                return string.Join(", ", localizedNames);
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Not implemented, as we don't need to convert back in this scenario
            return DependencyProperty.UnsetValue;
        }
    }

    public class BloccatoTipiLocalizationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BloccatoTipi bloccato)
            {
                // Generates: "BibliotecaBloccatoSbloccato", "BibliotecaBloccatoBloccato", etc.
                string resourceKey = $"BibliotecaBloccato{bloccato}";

                // Fetch from active language ResourceDictionary
                var localizedString = Application.Current.TryFindResource(resourceKey)?.ToString();

                return localizedString ?? bloccato.ToString();
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Not implemented, as we don't need to convert back in this scenario
            return DependencyProperty.UnsetValue;
        }
    }

    public class ColumnState
    {
        public string Header { get; set; } = "";
        public double Width { get; set; }
    }

    public class LibraryToolState
    {
        // Textboxes & Dropdowns
        public string SearchText { get; set; } = "";
        public string SelectedTipoFilter { get; set; } = "All";
        public string SelectedLinguaFilter { get; set; } = "All";
        public string SelectedDataCondition { get; set; } = "Any";
        public string FilterYearText { get; set; } = "";

        // Layout Splitter Panel (Width or Height tracking value)
        public double SplitterPosition { get; set; } = 250;

        // DataGrid Column States
        public List<ColumnState> ColumnStates { get; set; } = [];
        public string SortColumnMemberPath { get; set; } = "";
        public string SortDirection { get; set; } = ""; // "Ascending" or "Descending"
    }
}
