using AvalonDock.Layout;
using LaParola.Dialogs;
using LaParola.DocumentViews;
using LaParola.Services;
using LaParola.Utilities;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

namespace LaParola.ToolViews
{
    // TODO2 for each text, buttons to: esportare, unire 2, cambia sola lettura, creare file unico, aggiungere radici

    /// <summary>
    /// Logica di interazione per LibraryToolView.xaml
    /// </summary>
    public partial class LibraryToolView : UserControl
    {
        private ICollectionView _booksView;

        private Point _toolTipOpenPosition;
        private bool _isToolTipOpen;

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

        internal void LoadLibraryData()
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
            IOrderedEnumerable<string> rootLanguages = libri
                .Where(b => !string.IsNullOrWhiteSpace(b.Lingua))
                .SelectMany(b => GetBaseLanguageCodes(b.Lingua))
                .Distinct()
                .OrderBy(lang => lang);

            foreach (string lang in rootLanguages)
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

        private void DataGridToolTip_Opened(object sender, RoutedEventArgs e)
        {
            // Capture the exact location where the tooltip spawned relative to the DataGrid
            _toolTipOpenPosition = Mouse.GetPosition(BooksDataGrid);
            _isToolTipOpen = true;
        }

        private void DataGridToolTip_Closed(object sender, RoutedEventArgs e)
        {
            // Reset flag if the tooltip closes naturally via timeout
            _isToolTipOpen = false;
        }

        private void BooksDataGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isToolTipOpen)
            {
                Point currentPosition = e.GetPosition(BooksDataGrid);

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
                if (!matchText)
                    return false;
            }

            // 2. TIPO FILTER
            if (TipoFilterComboBox?.SelectedItem is ComboBoxItem selectedTipoItem)
            {
                string? tipoTag = selectedTipoItem.Tag.ToString();
                if (tipoTag != null && tipoTag != "All" && Enum.TryParse(typeof(TestoTipi), tipoTag, out object? match) && match is TestoTipi selectedEnumFlag)
                {
                    if (!info.Tipo.HasFlag(selectedEnumFlag))
                        return false;
                }
            }

            // 3. LINGUA FILTER
            if (LinguaFilterComboBox?.SelectedItem is ComboBoxItem selectedLangItem)
            {
                string? langTag = selectedLangItem.Tag.ToString();
                if (langTag != null && langTag != "All" && !GetBaseLanguageCodes(info.Lingua).Contains(langTag))
                    return false;
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
            Match match = RegExAnno().Match(dataStr);
            if (match.Success && int.TryParse(match.Value, out int year))
            {
                return year;
            }
            return null;
        }

        private async void AddBookButton_Click(object sender, RoutedEventArgs e)
        {
            // CASO 1: L'utente ha fatto clic sul pulsante principale
            if (sender is Button bottone)
            {
                if (bottone.ContextMenu != null)
                {
                    // Allinea il menu esattamente sotto il pulsante
                    bottone.ContextMenu.PlacementTarget = bottone;
                    // Forza l'apertura del menu a comparsa
                    bottone.ContextMenu.IsOpen = true;
                }
            }

            // CASO 2: L'utente ha cliccato su una delle opzioni del menu
            else if (sender is MenuItem voceMenu)
            {
                // Recuperiamo il Tag identificativo
                string tagSelezionato = voceMenu.Tag?.ToString() ?? "";
                TipoImportazione tipo = TipoImportazione.Nessuno;

                switch (tagSelezionato)
                {
                    case "Scarica":
                        App.DockingHost.ShowTool("tool.aggiungitesti");
                        break;

                    case "ImportaOSIS":
                    case "ImportaZefania":
                    case "ImportaThML":
                    case "ImportaBibleWorks":
                        string titoloSelezionaXMLDialogo = (string)(Application.Current.TryFindResource("BibliotecaAggiungiLibriApriFileXML") ?? "Select an XML File");
                        string filtro = (string)(Application.Current.TryFindResource("BibliotecaAggiungiLibriApriFileXMLFiltro") ?? "XML files (*.xml)|*.xml|All files (*.*)|*.*");
                        if (tagSelezionato == "ImportaOSIS")
                            tipo = TipoImportazione.ImportaOSIS;
                        else if (tagSelezionato == "ImportaZefania")
                            tipo = TipoImportazione.ImportaZefania;
                        else if (tagSelezionato == "ImportaThML")
                            tipo = TipoImportazione.ImportaThML;
                        else if (tagSelezionato == "ImportaBibleWorks")
                        {
                            tipo = TipoImportazione.ImportaBibleWorks;
                            titoloSelezionaXMLDialogo = (string)(Application.Current.TryFindResource("BibliotecaAggiungiLibriApriFileTXT") ?? "Select a Text File (BibleWorks format)");
                            filtro = (string)(Application.Current.TryFindResource("BibliotecaAggiungiLibriApriFileTXTFiltro") ?? "Text files (*.txt)|*.txt|All files (*.*)|*.*");
                        }

                        string ultimaCartellaImportare = MainWindow.settings.UltimaCartellaImportare;
                        if (string.IsNullOrEmpty(ultimaCartellaImportare))
                            ultimaCartellaImportare = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                        OpenFileDialog openFileDialog = new()
                        {
                            Title = titoloSelezionaXMLDialogo,
                            Filter = filtro,
                            InitialDirectory = ultimaCartellaImportare,
                            Multiselect = false
                        };

                        // Show the dialog box. ShowDialog() returns a nullable boolean (bool?).
                        bool? result = openFileDialog.ShowDialog();

                        // If the user clicked OK, import the selected file path
                        if (result == true)
                        {
                            string? percorso = Path.GetDirectoryName(openFileDialog.FileName);
                            if (!string.IsNullOrEmpty(percorso))
                            {
                                MainWindow.settings.UltimaCartellaImportare = openFileDialog.FileName;
                            }

                            using StatusTask status = StatusService.AvviaTask((string)(Application.Current.TryFindResource("BibliotecaAggiungiLibriInCorso") ?? "Importing the file"));

                            MetaData? data = await ImportaService.ImportaDaFileAsync(openFileDialog.FileName, tipo);
                            if (data != null)
                            {
                                string messaggio = string.Format(CultureInfo.InvariantCulture, (string)(Application.Current.TryFindResource("BibliotecaAggiungiLibriCompletato") ?? "{0} added to the program"), Path.GetFileName(data.NomeVersioneUtilizzato));
                                status.Update(messaggio, 100);
                                MainWindow.Testi.AggiungiTesto(data.NomeVersioneUtilizzato + ".laparola", 0);

                                Funzioni.AggiornaTestiNellInterfaccia();
                            }
                            else
                            {
                                status.Update((string)(Application.Current.TryFindResource("BibliotecaAggiungiLibriErrore") ?? "Error while importing the file"), 100);
                            }

                            // Remove status bar text after 5 seconds
                            await Task.Delay(5000);
                        }
                        break;

                    case "ImportaRtf":
                        //tipo = TipoImportazione.ImportaRtf;
                        string ultimaCartellaImportareRtf = MainWindow.settings.UltimaCartellaImportareRtf;
                        if (string.IsNullOrEmpty(ultimaCartellaImportareRtf))
                            ultimaCartellaImportareRtf = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                        OpenFolderDialog dialogCartella = new()
                        {
                            Title = (string)(Application.Current.TryFindResource("BibliotecaAggiungiLibriCartellaDialogoTitolo") ?? "Importing the file"),
                            InitialDirectory = ultimaCartellaImportareRtf,
                        };

                        if (dialogCartella.ShowDialog() == true)
                        {
                            string selectedDirectory = dialogCartella.FolderName;
                            MainWindow.settings.UltimaCartellaImportareRtf = selectedDirectory;
                            MetaData dati = new();
                            if (Directory.Exists(selectedDirectory))
                            {
                                // Search for any file ending with .laparolainfo
                                string? infoFilePath = Directory.GetFiles(selectedDirectory, "*.laparolainfo").FirstOrDefault();

                                if (infoFilePath != null)
                                {
                                    // Execute your function passing the path as an argument
                                    dati = ImportaService.CaricaMetadatiDaFile(infoFilePath);
                                }
                            }

                            ImportDialog dialogRtf = new(tipo)
                            {
                                Owner = Window.GetWindow(this), // Sets parent window for proper styling/centering
                                File = selectedDirectory,
                                Titolo = dati.Titolo,
                                Abbreviazione = dati.Abbreviazione,
                                Autore = dati.Autore,
                                CasaEditrice = dati.CasaEditrice,
                                Copyright = dati.Copyright,
                                Descrizione = dati.Descrizione,
                                Data = dati.Data,
                                Isbn = dati.ISBN,
                                Lingua = dati.Lingua,
                                VersioneDiNote = dati.VersioneDelleNote,
                            };

                            if (dialogRtf.ShowDialog() == true)
                            {
                                MetaData datiNuova = new()
                                {
                                    Titolo = dialogRtf.Titolo,
                                    Abbreviazione = dialogRtf.Abbreviazione,
                                    Autore = dialogRtf.Autore,
                                    CasaEditrice = dialogRtf.CasaEditrice,
                                    Copyright = dialogRtf.Copyright,
                                    Descrizione = dialogRtf.Descrizione,
                                    Data = dialogRtf.Data,
                                    ISBN = dialogRtf.Isbn,
                                    Lingua = dialogRtf.Lingua,
                                    VersioneDelleNote = dialogRtf.VersioneDiNote,
                                    FileDaAnalizzare = selectedDirectory,
                                    NomeVersioneUtilizzato = ImportaService.ImpostaNomeFileLaParolaDaFileOrigine(dialogRtf.Titolo),
                                    Tipo = TipoImportazione.ImportaRtf
                                };

                                using StatusTask status = StatusService.AvviaTask((string)(Application.Current.TryFindResource("BibliotecaAggiungiLibriInCorso") ?? "Importing the file"));
                                bool successo = await ImportaService.CreaFileAsync(datiNuova);
                                if (!successo)
                                {
                                    string messaggio = (string)(Application.Current.TryFindResource("BibliotecaAggiungiLibriErrore") ?? "Error while importing the file");
                                    status.Update(messaggio, 100);
                                }
                                else
                                {
                                    string messaggio = string.Format(CultureInfo.InvariantCulture, (string)(Application.Current.TryFindResource("BibliotecaAggiungiLibriCreaCompletato") ?? "{0} added to the program"), Path.GetFileName(datiNuova.Titolo));
                                    status.Update(messaggio, 100);
                                    MainWindow.Testi.AggiungiTesto(datiNuova.NomeVersioneUtilizzato + ".laparola", 0);
                                    Funzioni.AggiornaTestiNellInterfaccia();
                                }

                                // Remove status bar text after 5 seconds
                                await Task.Delay(5000);
                            }
                        }
                        break;
                    case "ImportaPDFRTF":
                        tipo = TipoImportazione.ImportaPDF;
                        titoloSelezionaXMLDialogo = (string)(Application.Current.TryFindResource("BibliotecaAggiungiLibriApriFilePDF") ?? "Select a PDF or RTF File");
                        filtro = (string)(Application.Current.TryFindResource("BibliotecaAggiungiLibriApriFilePDFFiltro") ?? "PDF files (*.pdf)|*.pdf|RTF files (*.rtf)|*.rtf|All files (*.*)|*.*");

                        ultimaCartellaImportare = MainWindow.settings.UltimaCartellaImportarePDF;
                        if (string.IsNullOrEmpty(ultimaCartellaImportare))
                            ultimaCartellaImportare = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                        OpenFileDialog openFileDialogPDF = new()
                        {
                            Title = titoloSelezionaXMLDialogo,
                            Filter = filtro,
                            InitialDirectory = ultimaCartellaImportare,
                            Multiselect = false
                        };

                        // Show the dialog box. ShowDialog() returns a nullable boolean (bool?).
                        bool? resultPDF = openFileDialogPDF.ShowDialog();

                        // If the user clicked OK, import the selected file path
                        if (resultPDF == true)
                        {
                            string? percorso = Path.GetDirectoryName(openFileDialogPDF.FileName);
                            if (!string.IsNullOrEmpty(percorso))
                            {
                                MainWindow.settings.UltimaCartellaImportarePDF = openFileDialogPDF.FileName;
                            }

                            MetaData datiPDF = new();
                            string infoFilePathPDF = Path.ChangeExtension(openFileDialogPDF.FileName, ".laparolainfo");
                            if (File.Exists(infoFilePathPDF))
                            {
                                datiPDF = ImportaService.CaricaMetadatiDaFile(infoFilePathPDF);
                            }

                            ImportDialog dialogPDF = new(tipo)
                            {
                                Owner = Window.GetWindow(this), // Sets parent window for proper styling/centering
                                File = openFileDialogPDF.FileName,
                                Titolo = datiPDF.Titolo,
                                Abbreviazione = datiPDF.Abbreviazione,
                                Autore = datiPDF.Autore,
                                CasaEditrice = datiPDF.CasaEditrice,
                                Copyright = datiPDF.Copyright,
                                Descrizione = datiPDF.Descrizione,
                                Data = datiPDF.Data,
                                Isbn = datiPDF.ISBN,
                                Lingua = datiPDF.Lingua,
                                VersioneDiNote = datiPDF.VersioneDelleNote,
                            };

                            if (dialogPDF.ShowDialog() == true)
                            {
                                MetaData datiNuova = new()
                                {
                                    Titolo = dialogPDF.Titolo,
                                    Abbreviazione = dialogPDF.Abbreviazione,
                                    Autore = dialogPDF.Autore,
                                    CasaEditrice = dialogPDF.CasaEditrice,
                                    Copyright = dialogPDF.Copyright,
                                    Descrizione = dialogPDF.Descrizione,
                                    Data = dialogPDF.Data,
                                    ISBN = dialogPDF.Isbn,
                                    Lingua = dialogPDF.Lingua,
                                    VersioneDelleNote = dialogPDF.VersioneDiNote,
                                    PDFComeLibro = dialogPDF.ComeLibro.IsChecked==true,
                                    FileDaAnalizzare = openFileDialogPDF.FileName,
                                    NomeVersioneUtilizzato = ImportaService.ImpostaNomeFileLaParolaDaFileOrigine(dialogPDF.Titolo),
                                    Tipo = TipoImportazione.ImportaPDF
                                };

                                using StatusTask statusPDF = StatusService.AvviaTask((string)(Application.Current.TryFindResource("BibliotecaAggiungiLibriInCorso") ?? "Importing the file"));
                                bool successo = await ImportaService.CreaFileAsync(datiNuova);
                                if (!successo)
                                {
                                    string messaggio = (string)(Application.Current.TryFindResource("BibliotecaAggiungiLibriErrore") ?? "Error while importing the file");
                                    statusPDF.Update(messaggio, 100);
                                }
                                else
                                {
                                    string messaggio = string.Format(CultureInfo.InvariantCulture, (string)(Application.Current.TryFindResource("BibliotecaAggiungiLibriCreaCompletato") ?? "{0} added to the program"), Path.GetFileName(datiNuova.Titolo));
                                    statusPDF.Update(messaggio, 100);
                                    MainWindow.Testi.AggiungiTesto(datiNuova.NomeVersioneUtilizzato + ".laparola", 0);
                                    Funzioni.AggiornaTestiNellInterfaccia();
                                }

                                // Remove status bar text after 5 seconds
                                await Task.Delay(5000);
                            }
                        }
                        break;

                    case "Crea":
                        tipo = TipoImportazione.Crea;
                        ImportDialog dialog = new(tipo)
                        {
                            Owner = Window.GetWindow(this), // Sets parent window for proper styling/centering
                            //Lingua = "it", // Pre-populate default value
                        };

                        if (dialog.ShowDialog() == true)
                        {
                            MetaData datiNuova = new()
                            {
                                Abbreviazione = dialog.Abbreviazione,
                                Titolo = dialog.Titolo,
                                ISBN = dialog.Isbn,
                                Autore = dialog.Autore,
                                CasaEditrice = dialog.CasaEditrice,
                                Data = dialog.Data,
                                Copyright = dialog.Copyright,
                                Lingua = dialog.Lingua,
                                VersioneDelleNote = dialog.VersioneDiNote,
                                Descrizione = dialog.Descrizione,
                                NomeVersioneUtilizzato = ImportaService.ImpostaNomeFileLaParolaDaFileOrigine(dialog.Titolo),
                                Tipo = TipoImportazione.Crea
                            };

                            using StatusTask status = StatusService.AvviaTask((string)(Application.Current.TryFindResource("BibliotecaAggiungiLibriCreaInCorso") ?? "Creating the file"));
                            bool successo = await ImportaService.CreaFileAsync(datiNuova);
                            if (!successo)
                            {
                                string messaggio = (string)(Application.Current.TryFindResource("BibliotecaAggiungiLibriCreaErrore") ?? "Error while creating and saving the file");
                                status.Update(messaggio, 100);
                            }
                            else
                            {
                                string messaggio = string.Format(CultureInfo.InvariantCulture, (string)(Application.Current.TryFindResource("BibliotecaAggiungiLibriCreaCompletato") ?? "{0} added to the program"), Path.GetFileName(datiNuova.Titolo));
                                status.Update(messaggio, 100);
                                MainWindow.Testi.AggiungiTesto(datiNuova.NomeVersioneUtilizzato + ".laparola", 0);
                                Funzioni.AggiornaTestiNellInterfaccia();
                            }

                            // Remove status bar text after 5 seconds
                            await Task.Delay(5000);
                        }
                        break;

                    default:
                        // Gestione di sicurezza se un Tag dovesse mancare o essere errato
                        break;
                }
            }
        }

        // Triggers filter recalculation on generic combo changes
        private void FilterControl_SelectionChanged(object sender, SelectionChangedEventArgs e) => _booksView?.Refresh();
        private void FilterControl_SelectedDateChanged(object sender, SelectionChangedEventArgs e) => _booksView?.Refresh();

        private void DataConditionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FilterYearTextBox == null) return;

            string? selectedTag = (DataConditionComboBox.SelectedItem as ComboBoxItem)?.Tag?.ToString();

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
                VersioneInformazioni book = selectedBook;
                string testo = book.Nome;

                if (!string.IsNullOrEmpty(testo))
                {
                    if ((book.Tipo & TestoTipi.Bibbia) == TestoTipi.Bibbia)
                        MainWindow.VisualizzaBibbia(testo);
                    else
                    {
                        try
                        {
                            int nNote = MainWindow.Testi.NumeroNote(testo);
                            int nNoteTitolo = MainWindow.Testi.NumeroNoteConTitolo(testo);
                            if (nNoteTitolo < nNote / 2)
                                MainWindow.VisualizzaCommentario(testo);
                            else
                                MainWindow.VisualizzaDizionario(testo);
                        }
                        catch { }
                    }
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

                    ChiudiViewerConTesto(selectedBook.Nome);
                    Funzioni.AggiornaTestiNellInterfaccia();
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
                InputDialog dialog = new(
                    prompt: (string)(Application.Current.TryFindResource("BibliotecaRinominaDomanda") ?? "Enter the new name for this text:"),
                    windowTitle: (string)(Application.Current.TryFindResource("BibliotecRinominaTitolo") ?? "Rename Text"),
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

                    ChiudiViewerConTesto(selectedBook.Nome);
                    Funzioni.AggiornaTestiNellInterfaccia();
                }
            }
        }

        private void ButtonCopia_Click(object sender, RoutedEventArgs e)
        {
            if (BooksDataGrid.SelectedItem is VersioneInformazioni selectedBook)
            {
                try
                {
                    string vecchioNome = MainWindow.Testi.Info(selectedBook.Nome).NomeDelFile;
                    int count = 1;
                    string nuovoNomeTesto = selectedBook.Nome + count.ToString(CultureInfo.InvariantCulture);
                    while (MainWindow.Testi.VersioneEsiste(nuovoNomeTesto))
                    {
                        ++count;
                        nuovoNomeTesto = selectedBook.Nome + count.ToString(CultureInfo.InvariantCulture);
                    }

                    string cartella = Path.GetDirectoryName(SettingsService.ResolveSettingsPath()) + Path.DirectorySeparatorChar;
                    count = 1;
                    string nuovoNomeFile = cartella + Path.GetFileNameWithoutExtension(vecchioNome) + count.ToString(CultureInfo.InvariantCulture) + Path.GetExtension(vecchioNome);
                    while (File.Exists(nuovoNomeFile))
                    {
                        ++count;
                        nuovoNomeFile = cartella + Path.GetFileNameWithoutExtension(vecchioNome) + count.ToString(CultureInfo.InvariantCulture) + Path.GetExtension(vecchioNome);
                    }

                    string nuovoNome = MainWindow.Testi.CopiaTesto(selectedBook.Nome, nuovoNomeTesto, nuovoNomeFile);

                    Funzioni.AggiornaTestiNellInterfaccia();

                    string messaggio = string.Format((string)(Application.Current.TryFindResource("BibliotecaCopiaCopiato") ?? "The text '{0}' was copied to '{1}'."), selectedBook.Nome, nuovoNome);
                    MessageBoxLPN.Show(Window.GetWindow(this), messaggio, (string)(Application.Current.TryFindResource("BibliotecaCopia") ?? "Copy Text"));
                }
                catch (Exception exc)
                {
                    string messaggio = string.Format((string)(Application.Current.TryFindResource("BibliotecaCopiaErrore") ?? $"Error copying text: {{0}}."), exc);
                    MessageBoxLPN.Show(Window.GetWindow(this), messaggio, (string)(Application.Current.TryFindResource("Errore") ?? "Error"));
                }
            }
        }

        private static void ChiudiViewerConTesto(string nome)
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
        }

        private void SaveLibraryState()
        {
            if (MainWindow.settings?.LibraryState == null) return;
            LibraryToolState state = MainWindow.settings.LibraryState;

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
            LibraryToolState? state = MainWindow.settings?.LibraryState;
            if (state == null)
                return;

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
                foreach (ColumnState colState in state.ColumnStates)
                {
                    DataGridColumn? column = BooksDataGrid.Columns.FirstOrDefault(c => c.Header?.ToString() == colState.Header);
                    column?.Width = new DataGridLength(colState.Width);
                }
            }

            // 5. DataGrid Sorting Rules
            if (!string.IsNullOrEmpty(state.SortColumnMemberPath) && _booksView != null)
            {
                DataGridColumn? column = BooksDataGrid.Columns.FirstOrDefault(c => c.SortMemberPath == state.SortColumnMemberPath);
                if (column != null)
                {
                    ListSortDirection dir = state.SortDirection == "Ascending" ? ListSortDirection.Ascending : ListSortDirection.Descending;
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
                    TextRange range = new(rtb.Document.ContentStart, rtb.Document.ContentEnd);
                    using MemoryStream ms = new(Encoding.UTF8.GetBytes(text));
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
                if (Enum.TryParse(typeof(TestoTipi), targetFlagStr, out object? result) && result is TestoTipi flag)
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

                List<string> localizedNames = [];

                // Dynamically loop through all flags inside the enum set
                foreach (TestoTipi flag in Enum.GetValues<TestoTipi>())
                {
                    if (flag == TestoTipi.None) continue;

                    if (tipi.HasFlag(flag))
                    {
                        // Matches your existing keys: "BibliotecaFiltroTipoBibbia", etc.
                        string resourceKey = $"BibliotecaFiltroTipo{flag}";

                        // Look up the string in the active localized ResourceDictionary
                        string? localizedString = Application.Current.TryFindResource(resourceKey)?.ToString();

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
                string? localizedString = Application.Current.TryFindResource(resourceKey)?.ToString();

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
