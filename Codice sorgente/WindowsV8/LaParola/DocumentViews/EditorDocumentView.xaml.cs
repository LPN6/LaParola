using AvalonDock.Layout;
using LaParola.Services;
using LaParola.Utilities;
using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace LaParola.DocumentViews;

public partial class EditorDocumentView : UserControl, IFlowDocumentHost, INotifyPropertyChanged
{
    private string? _currentFile;
    public LayoutDocument? ParentDocument { get; set; }
    private string ultimoTitolo = "";
    internal bool IsRiferimentoBiblico;
    private bool _suppressTextChanged;

    private bool _isDirty;
    public bool IsDirty
    {
        get => _isDirty;
        set
        {
            if (_isDirty == value)
                return;

            _isDirty = value;

            UpdateTitle();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    public EditorDocumentView()
    {
        InitializeComponent();

        this.PreviewKeyDown += (s, e) =>
        {
            bool italiano = MainWindow.settings.Lingua.StartsWith("it", StringComparison.CurrentCultureIgnoreCase);
            if (e.Key == System.Windows.Input.Key.T && System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Control)
            {
                if (italiano)
                {
                    ShowFindDialog();
                    e.Handled = true;
                }
            }
            else if (e.Key == System.Windows.Input.Key.U && System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Control)
            {
                if (italiano)
                {
                    ShowReplaceDialog();
                    e.Handled = true;
                }
            }
            else if (e.Key == System.Windows.Input.Key.F && System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Control)
            {
                if (italiano)
                {
                    e.Handled = true;
                }
            }
            else if (e.Key == System.Windows.Input.Key.H && System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Control)
            {
                if (italiano)
                {
                    e.Handled = true;
                }
            }
            else if (e.Key == System.Windows.Input.Key.Escape && FindReplacePanel.Visibility == Visibility.Visible)
            {
                CloseFindDialog();
                e.Handled = true;
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (e.Key == Key.OemPlus || e.Key == Key.Add)
                {
                    ZoomIn(Editor, true);
                    e.Handled = true;

                }
                else if (e.Key == Key.OemMinus || e.Key == Key.Subtract)
                {
                    ZoomIn(Editor, false);
                    e.Handled = true;
                }
            }
        };

        DataContext = this;
        Editor.Document = new FlowDocument
        {
            FontFamily = new System.Windows.Media.FontFamily(MainWindow.Testi.Formato.FontNome),
            FontSize = MainWindow.Testi.Formato.FontDimensione * 4.0 / 3.0, // perché WPF usa unità di misura in 1/96 di pollice, mentre i font sono in punti (1/72 di pollice)
            PageWidth = double.NaN,
            ColumnWidth = double.PositiveInfinity,
            PagePadding = new Thickness(20)
        };

        Editor.TextChanged += Editor_TextChanged;

        PreviewMouseLeftButtonDown += Editor_PreviewMouseLeftButtonDown;
        PreviewMouseWheel += (s, e) =>
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                e.Handled = true;
                // Ignore micro-events arriving within the cooldown window (stops momentum chatter)
                if ((DateTime.Now - _lastZoomTime).TotalMilliseconds < ZoomCooldownMs)
                    return;

                _lastZoomTime = DateTime.Now;
                ZoomIn(Editor, e.Delta > 0);
            }
        };
    }

    private DateTime _lastZoomTime = DateTime.MinValue;
    private const int ZoomCooldownMs = 100; // Minimum milliseconds between zoom steps

    private static void ZoomIn(RichTextBox rtb, bool zoomIn)
    {
        int zoom = (int)Math.Round(rtb.LayoutTransform.Value.M11 * 100) + (zoomIn ? 10 : -10);
        setZoom(rtb, zoom);
    }

    private static void setZoom(RichTextBox rtb, int zoom)
    {
        if (zoom < 20) zoom = 20;
        if (zoom > 500) zoom = 500;
        rtb.LayoutTransform = new ScaleTransform(zoom / 100.0, zoom / 100.0);
    }

    private void HelpFlyout_OnHelpClicked(object sender, RoutedEventArgs e)
    {
        MainWindow.MostraGuida((string)(Application.Current.TryFindResource("OpzioniEditorTitolo") ?? "Editor"));
    }

    /// <summary>
    /// Interrompe immediatamente la sintesi vocale in corso.
    /// Invocato dal DockingManager quando il pannello viene chiuso definitivamente.
    /// </summary>
    public void StoppaSintesiVocale()
    {
        try
        {
            LettoreVoce.FermaSeAttivo(BtnVoce);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Errore stop TTS: {ex.Message}");
        }
    }


    private void Editor_SelectionChanged(object sender, RoutedEventArgs e)
    {
        if (Editor == null) return;

        if (BoldBtn != null)
        {
            // Recuperiamo il peso del font del testo attualmente selezionato
            object lFontWeight = Editor.Selection.GetPropertyValue(TextElement.FontWeightProperty);

            // Se la selezione è mista (un po' in grassetto e un po' no), WPF restituisce DependencyProperty.UnsetValue
            BoldBtn.IsChecked = (lFontWeight != DependencyProperty.UnsetValue && lFontWeight.Equals(FontWeights.Bold));
        }

        if (ItalicBtn != null)
        {
            object lFontStyle = Editor.Selection.GetPropertyValue(TextElement.FontStyleProperty);
            ItalicBtn.IsChecked = (lFontStyle != DependencyProperty.UnsetValue && lFontStyle.Equals(FontStyles.Italic));
        }

        if (UnderlineBtn != null)
        {
            object lTextDecorations = Editor.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            UnderlineBtn.IsChecked = (lTextDecorations != DependencyProperty.UnsetValue && lTextDecorations.Equals(TextDecorations.Underline));
        }

    }

    private void UpdateTitle()
    {
        if (ParentDocument == null)
            return;

        string title;

        if (string.IsNullOrWhiteSpace(_currentFile))
        {
            title = (string)(Application.Current.TryFindResource("FileUnsaved") ?? "Untitled");
        }
        else
        {
            title = Path.GetFileName(_currentFile);
        }

        ultimoTitolo = title;
        if (IsDirty)
        {
            title += "*";
        }
        ParentDocument.Title = title;
    }

    public string CurrentFileDisplay => string.IsNullOrWhiteSpace(_currentFile)
        ? (string)(Application.Current.TryFindResource("FileUnsaved") ?? "(unsaved)")
        : _currentFile!;

    public FlowDocument FlowDocument => Editor.Document;

    private void Editor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_suppressTextChanged)
            return;

        IsDirty = true;
    }

    private void Editor_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        // Check if the user clicked exactly on a Hyperlink element
        if (e.OriginalSource is DependencyObject source)
        {
            // Walk up the visual/logical tree to find the Hyperlink
            DependencyObject current = source;
            while (current != null)
            {
                if (current is Hyperlink link)
                {
                    // Trigger the click programmatically
                    link.DoClick();

                    // Mark event as handled so the RichTextBox doesn't move the caret
                    e.Handled = true;
                    return;
                }

                // Move up the tree (FrameworkContentElements like Run/Hyperlink use LogicalTreeHelper)
                if (current is FrameworkContentElement fce)
                {
                    current = fce.Parent;
                }
                else
                {
                    break;
                }
            }
        }
    }

    public void EseguiSenzaSporcareDocumento(Action azione)
    {
        bool eraSospeso = _suppressTextChanged;
        _suppressTextChanged = true;
        try
        {
            azione();
        }
        finally
        {
            _suppressTextChanged = eraSospeso;
        }
    }

    public void SetDocument(FlowDocument doc)
    {
        _suppressTextChanged = true;
        Editor.Document = doc;
        _currentFile = null;
        IsDirty = false;
        _suppressTextChanged = false;
        OnPropertyChanged(nameof(CurrentFileDisplay));
        UpdateTitle();
        FocusEditor();
    }

    public void FocusEditor(bool caretAtEnd = false)
    {
        Dispatcher.BeginInvoke(new Action(() =>
        {
            Editor.Focus();
            Keyboard.Focus(Editor);

            Editor.CaretPosition = caretAtEnd
                ? Editor.Document.ContentEnd
                : Editor.Document.ContentStart;

            Editor.Selection.Select(
                Editor.CaretPosition,
                Editor.CaretPosition);

        }), DispatcherPriority.Input);
    }

    private void Open_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dlg = new()
        {
            Filter = (string)(Application.Current.TryFindResource("FileDialogoFiltroTutti") ?? "Rich Text (*.rtf)|*.rtf|Plain Text (*.txt)|*.txt|All files (*.*)|*.*")
        };
        if (dlg.ShowDialog(Window.GetWindow(this)) == true)
        {
            LoadFromFile(dlg.FileName);
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        SaveDocument();
    }

    private void SaveAs_Click(object sender, RoutedEventArgs e)
    {
        SaveDocumentAs();
    }

    public void SaveDocument()
    {
        if (string.IsNullOrWhiteSpace(_currentFile))
        {
            SaveDocumentAs();
            return;
        }
        SaveToFile(_currentFile);
    }

    public void SaveDocumentAs()
    {
        SaveFileDialog dlg = new()
        {
            Filter = (string)(Application.Current.TryFindResource("FileDialogoFiltro") ?? "Rich Text (*.rtf)|*.rtf|Plain text (*.txt)|*.txt"),
            FileName = (string)(Application.Current.TryFindResource("FileDialogoNome") ?? "document"),
        };

        if (dlg.ShowDialog(Window.GetWindow(this)) == true)
        {
            _currentFile = dlg.FileName;
            SaveToFile(_currentFile);
            OnPropertyChanged(nameof(CurrentFileDisplay));
            ParentDocument?.Title = Path.GetFileName(_currentFile);
        }
    }

    private void SaveToFile(string path)
    {
        try
        {
            TextRange range = new(Editor.Document.ContentStart, Editor.Document.ContentEnd);
            using FileStream fs = new(path, FileMode.Create, FileAccess.Write);
            if (Path.GetExtension(path).Equals(".xaml", System.StringComparison.OrdinalIgnoreCase))
            {
                range.Save(fs, DataFormats.XamlPackage);
            }
            else if (Path.GetExtension(path).Equals(".rtf", System.StringComparison.OrdinalIgnoreCase))
            {
                range.Save(fs, DataFormats.Rtf);
            }
            else
            {
                range.Save(fs, DataFormats.Text);
            }
            IsDirty = false;
            UpdateTitle();
        }
        catch (System.Exception ex)
        {
            MessageBoxLPN.Show(Window.GetWindow(this), ex.Message, (string)(Application.Current.TryFindResource("EditorSalvaFallito") ?? "Save failed"));
        }
    }

    public void LoadDocument(string path)
    {
        LoadFromFile(path);
    }

    private void LoadFromFile(string path)
    {
        _suppressTextChanged = true;
        try
        {
            TextRange range = new(Editor.Document.ContentStart, Editor.Document.ContentEnd);
            using FileStream fs = new(path, FileMode.Open, FileAccess.Read);
            if (Path.GetExtension(path).Equals(".xaml", System.StringComparison.OrdinalIgnoreCase))
            {
                range.Load(fs, DataFormats.XamlPackage);
            }
            else if (Path.GetExtension(path).Equals(".rtf", System.StringComparison.OrdinalIgnoreCase))
            {
                range.Load(fs, DataFormats.Rtf);
                Brush fg = (Brush)Application.Current.FindResource("AppForegroundBrush");
                RtfColorTransformer.ApplyThemeToDocument(Editor.Document, true, fg, true);
            }
            else
            {
                range.Load(fs, DataFormats.Text);
                foreach (Block block in Editor.Document.Blocks)
                {
                    if (block is Paragraph p)
                    {
                        p.Margin = new Thickness(0);
                    }
                }
            }

            _currentFile = path;
            ParentDocument?.Title = Path.GetFileName(_currentFile);
            OnPropertyChanged(nameof(CurrentFileDisplay));

            FocusEditor();
        }
        catch (System.Exception ex)
        {
            MessageBoxLPN.Show(Window.GetWindow(this), ex.Message, (string)(Application.Current.TryFindResource("EditorApriFallito") ?? "Open failed"));
        }
        finally
        {
            IsDirty = false;
            _suppressTextChanged = false;
            UpdateTitle();
        }
    }

    public bool ConfirmClose()
    {
        if (!IsDirty || MainWindow.settings.EditorChiudere)
            return true;

        string message =
            (string)(Application.Current.TryFindResource("EditorSalvaModifiche") ?? "Do you want to save changes to") + " " + ultimoTitolo + "?";

        string title =
            (string)(Application.Current.TryFindResource("EditorSalvaModificheTitolo") ?? "Unsaved changes");

        MessageBoxResult result =
            MessageBoxLPN.Show(
                Window.GetWindow(this),
                message,
                title, MessageBoxButton.YesNoCancel);

        switch (result)
        {
            case MessageBoxResult.Yes:
                SaveDocument();
                return !IsDirty;
            case MessageBoxResult.No:
                return true;
            default:
                return false;
        }
    }

    private void BtnZoom_Click(object sender, RoutedEventArgs e)
    {
        if (BtnZoom.ContextMenu != null)
        {
            if (Application.Current.TryFindResource("ControlBackgroundBrush") is Brush currentThemeBrush)
            {
                BtnZoom.ContextMenu.Resources[SystemColors.MenuBrushKey] = currentThemeBrush;
                BtnZoom.ContextMenu.Resources[SystemColors.MenuBarBrushKey] = currentThemeBrush;
            }

            BtnZoom.ContextMenu.PlacementTarget = BtnZoom;
            BtnZoom.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            BtnZoom.ContextMenu.IsOpen = true;

            if (!ReferenceEquals(sender, e.Source))
                return;

            MenuZoom100.IsChecked = false;
            MenuZoom200.IsChecked = false;
            MenuZoom050.IsChecked = false;
            MenuZoom080.IsChecked = false;
            MenuZoom150.IsChecked = false;
            MenuZoom120.IsChecked = false;
            MenuZoom400.IsChecked = false;

            int zoom = (int)Math.Round(Editor.LayoutTransform.Value.M11 * 100);
            switch (zoom)
            {
                case 50: MenuZoom050.IsChecked = true; break;
                case 80: MenuZoom080.IsChecked = true; break;
                case 100: MenuZoom100.IsChecked = true; break;
                case 120: MenuZoom120.IsChecked = true; break;
                case 150: MenuZoom150.IsChecked = true; break;
                case 200: MenuZoom200.IsChecked = true; break;
                case 400: MenuZoom400.IsChecked = true; break;
                default:
                    break;
            }
        }
    }

    private void MenuZoomItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem item && item.Tag is string tagStr && int.TryParse(tagStr, out int zoom))
        {
            setZoom(Editor, zoom);
        }
    }


    private void Voce_Click(object sender, RoutedEventArgs e)
    {
        ToggleLettura();
    }

    public void ToggleLettura()
    {
        LettoreVoce.ToggleLettura(BtnVoce, this, () => Editor.Document, "", IsRiferimentoBiblico);
    }

    public void ShowFindDialog()
    {
        ShowFindReplaceDialog(false);
    }

    public void FindNext()
    {
        if (FindReplacePanel.Visibility != Visibility.Visible)
            ShowFindDialog();
        DoSearch(false);
    }

    public void ReplaceNext()
    {
        if (FindReplacePanel.Visibility != Visibility.Visible)
            ShowReplaceDialog();
        DoReplace();
    }

    public void ShowReplaceDialog()
    {
        ShowFindReplaceDialog(true);
    }

    private void ShowFindReplaceDialog(bool showReplace)
    {
        FindReplacePanel.Visibility = Visibility.Visible;
        BtnToggleReplace.IsChecked = showReplace;
        if (showReplace)
        {
            TxtReplace.Visibility = Visibility.Visible;
            StackPanelReplace.Visibility = Visibility.Visible;
            ToggleIcon.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.ChevronUp;
        }
        else
        {
            TxtReplace.Visibility = Visibility.Collapsed;
            StackPanelReplace.Visibility = Visibility.Collapsed;
            ToggleIcon.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.ChevronDown;
        }

        if (showReplace && !string.IsNullOrEmpty(TxtSearch.Text))
        {
            // If they hit "Replace" and already typed a search term, focus the replace input
            TxtReplace.Focus();
            TxtReplace.SelectAll();
        }
        else
        {
            // Otherwise, focus the search bar first
            TxtSearch.Focus();
            TxtSearch.SelectAll();
        }
    }

    private void CloseFindDialog()
    {
        FindReplacePanel.Visibility = Visibility.Collapsed;
        TxtSearch.ClearValue(TextBox.BackgroundProperty);
        Editor.Focus();
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e) => CloseFindDialog();

    // Gestione dell'espansione del pannello Replace (Animazione della freccetta inclusa)
    private void BtnToggleReplace_Click(object sender, RoutedEventArgs e)
    {
        if (BtnToggleReplace.IsChecked == true)
        {
            TxtReplace.Visibility = Visibility.Visible;
            StackPanelReplace.Visibility = Visibility.Visible;
            ToggleIcon.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.ChevronUp;
            // Sposta il focus sulla sostituzione se l'utente ha già scritto cosa cercare
            if (!string.IsNullOrEmpty(TxtSearch.Text))
            {
                TxtReplace.Focus();
            }
        }
        else
        {
            TxtReplace.Visibility = Visibility.Collapsed;
            StackPanelReplace.Visibility = Visibility.Collapsed;
            ToggleIcon.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.ChevronDown;
            TxtSearch.Focus();
        }
    }

    private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        // Resetta lo sfondo della TextBox se l'utente cambia testo dopo un errore
        TxtSearch.ClearValue(TextBox.BackgroundProperty);
        TxtSearch.ClearValue(TextBox.BorderBrushProperty);
    }

    private void TxtSearch_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        // Check if the user pressed the Enter/Return key
        if (e.Key == System.Windows.Input.Key.Enter || e.Key == System.Windows.Input.Key.Return)
        {
            // Execute "Find Next"
            DoSearch(false);

            e.Handled = true;
        }
    }

    private void BtnNext_Click(object sender, RoutedEventArgs e) => DoSearch(false);
    private void BtnPrev_Click(object sender, RoutedEventArgs e) => DoSearch(true);

    private bool DoSearch(bool backward)
    {
        string searchText = TxtSearch.Text;
        if (string.IsNullOrEmpty(searchText)) return false;

        bool matchCase = BtnMatchCase.IsChecked ?? false;
        bool wholeWord = BtnWholeWord.IsChecked ?? false;

        TextPointer startPointer = backward ? Editor.Selection.Start : Editor.Selection.End;
        TextRange? foundRange = FindTextInDocument(startPointer, searchText, matchCase, wholeWord, backward);

        if (foundRange == null)
        {
            // Cerca di nuovo dall'inizio/fine (Wrap-around)
            TextPointer restartPointer = backward ? Editor.Document.ContentEnd : Editor.Document.ContentStart;
            foundRange = FindTextInDocument(restartPointer, searchText, matchCase, wholeWord, backward);
        }

        if (foundRange != null)
        {
            // Ripristina lo stato grafico corretto del tema
            TxtSearch.ClearValue(TextBox.BackgroundProperty);
            TxtSearch.ClearValue(TextBox.BorderBrushProperty);

            Editor.Selection.Select(foundRange.Start, foundRange.End);
            Editor.Focus();
            return true;
        }

        // testo non trovato. Usa un rosso scuro/trasparente adatto al Dark Mode
        TxtSearch.Background = new SolidColorBrush(Color.FromArgb(50, 255, 0, 0)); // Rosso molto sfumato
        TxtSearch.BorderBrush = Brushes.Red;
        return false;
    }

    private static TextRange? FindTextInDocument(TextPointer startPosition, string textToFind, bool matchCase, bool wholeWord, bool backward)
    {
        LogicalDirection direction = backward ? LogicalDirection.Backward : LogicalDirection.Forward;
        TextPointer position = startPosition;

        while (position != null)
        {
            if (position.GetPointerContext(direction) == TextPointerContext.Text)
            {
                string textRun = position.GetTextInRun(direction);

                // Determina il tipo di comparazione delle stringhe
                StringComparison comparison = matchCase ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;

                int index = backward ? textRun.LastIndexOf(textToFind, comparison) : textRun.IndexOf(textToFind, comparison);

                if (index >= 0)
                {
                    // Verifica l'opzione "Parola Intera" usando Regex sui confini del testo trovato
                    if (wholeWord)
                    {
                        bool isWordStart = index == 0 || !char.IsLetterOrDigit(textRun[index - 1]);
                        bool isWordEnd = (index + textToFind.Length) >= textRun.Length || !char.IsLetterOrDigit(textRun[index + textToFind.Length]);

                        if (!isWordStart || !isWordEnd)
                        {
                            // Salta questa occorrenza perché non è una parola isolata
                            position = position.GetPositionAtOffset(backward ? -1 : 1);
                            continue;
                        }
                    }

                    // Crea i puntatori precisi per selezionare l'istanza trovata nel run di testo
                    TextPointer startMatch = position.GetPositionAtOffset(backward ? index - textRun.Length : index);
                    TextPointer endMatch = startMatch.GetPositionAtOffset(textToFind.Length);

                    return new TextRange(startMatch, endMatch);
                }
            }

            position = position.GetNextContextPosition(direction);
        }

        return null;
    }

    // --- SEZIONE SOSTITUZIONE (REPLACE) ---

    private void BtnReplace_Click(object sender, RoutedEventArgs e)
    {
        DoReplace();
    }

    private void DoReplace()
    {
        // Se la selezione corrente corrisponde già alla ricerca, la sostituisce al volo
        if (!string.IsNullOrEmpty(TxtSearch.Text) && Editor.Selection.Text.Equals(TxtSearch.Text, StringComparison.CurrentCultureIgnoreCase))
        {
            Editor.Selection.Text = TxtReplace.Text;
        }

        // Successivamente si sposta sulla parola successiva
        DoSearch(false);
    }

    private void BtnReplaceAll_Click(object sender, RoutedEventArgs e)
    {
        // Disabilita temporaneamente il rendering grafico per aumentare drasticamente le performance di sostituzione di massa
        Editor.BeginChange();

        // Ricomincia dall'inizio del documento
        Editor.Selection.Select(Editor.Document.ContentStart, Editor.Document.ContentStart);

        int counter = 0;
        while (DoSearch(false))
        {
            Editor.Selection.Text = TxtReplace.Text;
            counter++;
            if (counter > 5000) break; // Protezione da loop infiniti accidentali
        }

        Editor.EndChange();

        string messageTemplate;
        if (counter != 1)
        {
            messageTemplate = (string)(Application.Current.TryFindResource("TrovaSostituzioneCompletata") ?? "{0} replacements were made.");
        }
        else
        {
            messageTemplate = (string)(Application.Current.TryFindResource("TrovaSostituzioneCompletata1") ?? "{0} replacement was made.");
        }
        string message = string.Format(messageTemplate, counter);
        MessageBoxLPN.Show(Window.GetWindow(this), message, (string)(Application.Current.TryFindResource("TrovaSostituzione") ?? "Replace"));
    }
}
