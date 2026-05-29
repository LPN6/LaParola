using AvalonDock.Layout;
using AvalonDock.Layout.Serialization;
using LaParola.DocumentViews;
using LaParola.Models;
using LaParola.ToolViews;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace LaParola;

// TODO2 toolbar: new, open, save (all), print, undo, redo, find, cut, copy, paste, vis bibbia, commentario, apri note, segnalibri, navigare, ricerca, mostra, (chiave), (racc info), paralleli, LQ, Misure, Gesti testi, aggiorna, opzioni,aiuto
// available icons are listed here: https://pictogrammers.com/library/mdi/
// TODO2 help centre: Search, FAQs, tutorials, contact, release notes, keyboard shortcuts, about, documentation, how to use, getting started
// TODO2 option to not ask confirm when closing documents

// TODO2 ApplicationCommands:
/*| `CancelPrint` | Cancels a print job. |
| `Close` | Closes a file or document. |
| `ContextMenu` | Opens the context menu. |
| `CorrectionList` | Opens a correction list (typically for speech or handwriting). |
| `Find` | Opens a search or "Find" dialog. |
| `Help` | Opens the help documentation. |
| `New` | Creates a new file or document. |
| `Open` | Opens an existing file or document. |
| `Print` | Prints the current document. |
| `PrintPreview` | Opens a print preview window. |
| `Properties` | Opens the properties for the current selection. |
| `Replace` | Opens a "Replace" dialog. |
| `Save` | Saves the current document. |
| `SaveAs` | Saves the current document with a new name or location. |
| `Stop` | Stops the current operation. |
 */

// TODO2 NavigationCommands:
/*
 * * `BrowseBack` (Navigates to the previous page in history)
* `BrowseForward` (Navigates to the next page in history)
* `BrowseHome` (Navigates to the home page)
* `DecreaseZoom` (Decreases the zoom percentage)
* `Favorites` | `FirstPage` | `GoToPage`
* `IncreaseZoom` (Increases the zoom percentage)
* `LastPage` | `NextPage` | `PreviousPage`
* `Refresh` (Refreshes the current page/content)
* `Search` (Navigates to a search interface)
* `StopLoading` (Stops loading the current page)
* `Zoom` (Sets a specific zoom level)
 */

// TODO2 EditingCommands:
/*
 * ### Caret Movement and Selection

* `MoveDownByLine` | `MoveDownByPage` | `MoveDownByParagraph`
* `MoveUpByLine` | `MoveUpByPage` | `MoveUpByParagraph`
* `MoveLeftByCharacter` | `MoveLeftByWord`
* `MoveRightByCharacter` | `MoveRightByWord`
* `MoveToDocumentEnd` | `MoveToDocumentStart`
* `MoveToLineEnd` | `MoveToLineStart`
* `SelectDownByLine` | `SelectDownByPage` | `SelectDownByParagraph`
* `SelectUpByLine` | `SelectUpByPage` | `SelectUpByParagraph`
* `SelectLeftByCharacter` | `SelectLeftByWord`
* `SelectRightByCharacter` | `SelectRightByWord`
* `SelectToDocumentEnd` | `SelectToDocumentStart`
* `SelectToLineEnd` | `SelectToLineStart`

### Text Deletion and Modification

* `Backspace` (Deletes character to the left)
* `Delete` (Deletes character to the right)
* `DeleteNextWord` | `DeletePreviousWord`
* `EnterParagraphBreak` (Inserts a paragraph break / Enter key behavior)
* `EnterLineBreak` (Inserts a line break / Shift+Enter behavior)
* `TabForward` | `TabBackward`

### Formatting (Rich Text)

* `ToggleSubscript` | `ToggleSuperscript`
* `ApplyFontSize` | `ApplyFontFamily`
* `ApplyForeground` | `ApplyBackground`
* `IncreaseIndentation` | `DecreaseIndentation`

### Lists and Structures

* `ToggleNumbering` (Toggles numbered list format)
* `InsertTable` | `InsertRows` | `InsertColumns`
* `DeleteRows` | `DeleteColumns` | `MergeCells` | `SplitCell`
 */

public partial class MainWindow : Window
{
    private SearchToolView? _searchView;
    private TextGeneratorToolView? _textGenView;
    private ConverterToolView? _converterView;
    private OptionsToolView? _optionsView;

    private object SearchToolViewInstance()
        => _searchView ??= new SearchToolView();

    private object TextGenToolViewInstance()
        => _textGenView ??= new TextGeneratorToolView();

    private object ConverterToolViewInstance()
        => _converterView ??= new ConverterToolView();

    private object OptionsToolViewInstance()
        => _optionsView ??= new OptionsToolView();

    internal static Texts Testi;
    internal static AppSettings settings;

    private IInputElement? _previousFocus;

    public static readonly RoutedUICommand NuovoCommand = new("NewEditor", "NuovoCommand", typeof(MainWindow));
    public static readonly RoutedUICommand ApriCommand = new("OpenEditor", "ApriCommand", typeof(MainWindow));
    public static readonly RoutedUICommand ChiudiCommand = new("Close", "ChiudiCommand", typeof(MainWindow));
    public static readonly RoutedUICommand SalvaCommand = new("Save", "SalvaCommand", typeof(MainWindow));
    public static readonly RoutedUICommand SalvaComeCommand = new("SaveAs", "SalvaComeCommand", typeof(MainWindow));
    public static readonly RoutedUICommand EsciCommand = new("Exit", "EsciCommand", typeof(MainWindow));
    public static readonly RoutedUICommand SearchCommand = new("Search", "SearchCommand", typeof(MainWindow));
    public static readonly RoutedUICommand MostraCommand = new("Mostra", "MostraCommand", typeof(MainWindow));
    public static readonly RoutedUICommand ConverterCommand = new("Converter", "ConverterCommand", typeof(MainWindow));
    public static readonly RoutedUICommand OptionsCommand = new("Options", "OptionsCommand", typeof(MainWindow));

    public MainWindow(AppSettings settingsLoaded)
    {
        settings = settingsLoaded;
        // if settings.language is it (but not set the first time...)
        //ApplicationCommands.SelectAll.InputGestures.Clear();
        //ApplicationCommands.SelectAll.InputGestures.Add(new KeyGesture(Key.Q , ModifierKeys.Control));

        InitializeComponent();

        RestoreWindowPlacement();
        App.DockingHost.Initialize(Dock, DocumentPane);

        Services.ThemeManager.ApplyDockTheme(Dock, settings.ThemeMode);
        App.ThemeManager.HookSystemThemeChanges(Dock, settings.ThemeMode);

        Loaded += MainWindow_Loaded;
        Closing += MainWindow_Closing;
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);

        Services.ThemeManager.SetDarkTitleBar(Services.ThemeManager.IsDark(settings.ThemeMode));
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        Loaded -= MainWindow_Loaded;

        ShowLoadingOverlay(true);

        try
        {
            await Task.Run(() =>
            {
                Testi = new Texts(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar);
                Testi.AggiungiDirectory(AppContext.BaseDirectory);
            });
        }
        finally
        {
            // Ora ripristina layout
            RestoreDockLayout();

            ShowLoadingOverlay(false);

            Testi.Formato = settings.Formato;

            if (settings.Language == "it")
            {
                Testi.libriNomi = Texts.LibriNomiItaliano.Split('|');
                Testi.libriAbbreviazioniUsate = Texts.LibriAbbreviazioniUsateItaliano.Split('|');
                string[] libriAbbRic = Texts.LibriAbbreviazioniRiconosciuteItaliano.Split('|', StringSplitOptions.RemoveEmptyEntries);
                string[] abbreviazioniDiLibro;
                Testi.LibriAbbreviazioniRiconosciute.Clear();
                for (byte i = 1; i <= 73; ++i)
                {
                    abbreviazioniDiLibro = libriAbbRic[i - 1].Split(',');
                    foreach (string abbreviazioneDiLibro in abbreviazioniDiLibro)
                        Testi.LibriAbbreviazioniRiconosciute[abbreviazioneDiLibro] = i;
                }

                UpdateShortcutBindings("it");
            }

            App.DockingHost.ActiveEditorChanged += (_, _) =>
            {
                UpdateEditorMenuState();
            };

        }

        if (Testi.NomiVersioni().Count == 0)
        {
            MessageBoxLPN.Show(this,
                (string)(Application.Current.TryFindResource("MainNessunaVersione") ?? "No text was found. Go to https://www.laparola.net/programma/windowsbeta.php to install texts to read."),
                (string)(Application.Current.TryFindResource("Errore") ?? "Error"));
        }
    }

    private void RestoreWindowPlacement()
    {
        if (settings.WindowWidth.HasValue)
            Width = settings.WindowWidth.Value;

        if (settings.WindowHeight.HasValue)
            Height = settings.WindowHeight.Value;

        if (settings.WindowLeft.HasValue)
            Left = settings.WindowLeft.Value;

        if (settings.WindowTop.HasValue)
            Top = settings.WindowTop.Value;

        if (settings.WindowState.HasValue)
        { // never restore minimized
            WindowState = settings.WindowState == WindowState.Minimized
                ? WindowState.Normal
                : settings.WindowState.Value;
        }

        // Ensure the window is at least partially visible on the current screen
        Rect screen = SystemParameters.WorkArea;
        if (Left < screen.Left) Left = screen.Left;
        if (Top < screen.Top) Top = screen.Top;
        if (Left + Width > screen.Right) Left = screen.Right - Width;
        if (Top + Height > screen.Bottom) Top = screen.Bottom - Height;
    }

    private void RestoreDockLayout()
    {
        // Se non c'è layout salvato, non fare nulla (layout di default in XAML)
        if (string.IsNullOrWhiteSpace(settings.DockLayoutXml))
        {
            HideDefaultToolWindows();

            FlowDocument doc = new()
            {
                FontFamily = new FontFamily("Georgia"),
                FontSize = 15
            };
            string testo = "Questa è la versione beta (di prova) di LaParola 8.\n\n" +
                "Attualmente il programma può mostrare un brano dalla Bibbia. Usa il menu 'Strumenti' per le possibili azioni da eseguire. Altre funzionalità saranno aggiunte prossimamente.\n\n" +
                "La disposizione delle finestre è molto flessibile: trascinando il titolo di una finestra, potete spostare, ancorare, affiancare e sovrapporre le finestre come preferite. Potete anche aprire più finestre editor e organizzarle come volete.\n\n" +
                "Se trovate dei problemi e avete suggerimenti, scrivetemi a info@laparola.net.\n\n" +
                "-----------------------------------------\n\n" +
                "This is the beta version di LaParola 8.\n\n" +
                "Currently, the program can only display a passage from the Bible. Use the 'Tools' menu for possible actions.Further features will be added soon.\n\n" +
"The window layout is very flexible: by dragging a window's title bar, you can move, dock, tile or overlap windows as you wish. You can also open multiple editor windows and arrange them in any way.\n\n" +
"If you encounter any issues or have suggestions, please email me at info@laparola.net.";
            doc.Blocks.Clear();
            doc.Blocks.Add(new Paragraph(new Run(testo)));
            App.DockingHost.OpenEditorDocument(doc, ((string)(Application.Current.TryFindResource("MenuAbout") ?? "About LaParola")).Replace("_", ""));
            return;
        }

        XmlLayoutSerializer serializer = new(Dock);

        // Prepara una lookup rapida degli stati viewer
        Dictionary<string, ViewerWindowState> viewerById = settings.ViewerWindows.ToDictionary(v => v.ContentId);

        serializer.LayoutSerializationCallback += (_, args) =>
        {
            string id = args.Model.ContentId ?? "";

            // 1) Tool panes: hanno ContentId fissi. Qui puoi restituire
            // le istanze già presenti (se le hai nominate con x:Name) oppure crearle.
            if (id == "tool.search") { args.Content = SearchToolViewInstance(); return; }
            if (id == "tool.textgen") { args.Content = TextGenToolViewInstance(); return; }
            if (id == "tool.converter") { args.Content = ConverterToolViewInstance(); return; }
            if (id == "tool.options") { args.Content = OptionsToolViewInstance(); return; }

            // 2) Viewer docs: ricrea e applica placeholder state
            if (id.StartsWith("doc.viewer."))
            {
                ViewerDocumentView view = new(); // il tuo UserControl viewer
                if (viewerById.TryGetValue(id, out ViewerWindowState state))
                {
                    // Placeholder: applica state fittizio per ora
                    view.LoadPlaceholder(state.DisplayName, state.VerseRef);
                }
                args.Content = view;
                return;
            }

            // 3) Editor docs: NON ricreare contenuti -> così non vengono ripristinati
            if (id.StartsWith("doc.editor."))
            {
                args.Cancel = true;
                return;
            }
        };

        using StringReader reader = new(settings.DockLayoutXml);
        serializer.Deserialize(reader); // supportato https://www.microsoft365.com/34ef1840-738c-4d0c-b09d-2cdf559636bd)[1](https://blog.csdn.net/qq_41375318/article/details/149233645)
    }

    private void HideDefaultToolWindows()
    {
        //TextGenToolAnchorable?.Hide();
        ConverterToolAnchorable?.Hide();
        OptionsToolAnchorable?.Hide();
    }

    private void ShowLoadingOverlay(bool show)
    {
        if (show)
        {
            _previousFocus = Keyboard.FocusedElement;

            LoadingText.Text = (string)(Application.Current.TryFindResource("MainCaricamento") ?? "Loading");

            LoadingOverlay.Visibility = Visibility.Visible;
            LoadingOverlay.IsHitTestVisible = true;

            // blocca input e focus sotto
            LoadingOverlay.Focus();

            Mouse.OverrideCursor = Cursors.Wait;
        }
        else
        {
            LoadingOverlay.Visibility = Visibility.Collapsed;
            LoadingOverlay.IsHitTestVisible = false;

            Mouse.OverrideCursor = null;

            if (_previousFocus != null)
                Keyboard.Focus(_previousFocus);
        }
    }

    private void UpdateEditorMenuState()
    {
        bool enabled = App.DockingHost.HasActiveEditor;

        MenuSalva.IsEnabled = enabled;
        MenuSalvaCome.IsEnabled = enabled;
        MenuChiudi.IsEnabled = App.DockingHost.HasClosableContent;
    }

    internal void UpdateShortcutBindings(string lingua)
    {
        bool italiano = lingua.StartsWith("it", StringComparison.CurrentCultureIgnoreCase);

        // to change default shortcuts, do this. Need also to change in constructor before InitializeComponent, but can't be changed in the menu without restart
        //ApplicationCommands.SelectAll.InputGestures.Clear();
        //ApplicationCommands.SelectAll.InputGestures.Add(new KeyGesture(italiano ? Key.Q : Key.A, ModifierKeys.Control,"Ctrl+Q"));

        // 1. Update the visual text displayed in the menu
        MenuApri.InputGestureText = italiano ? "Ctrl+F12" : "Ctrl+O";

        // 2. Remove the old key binding if it exists (to avoid duplicate triggers)
        var existingBinding = this.InputBindings
            .OfType<KeyBinding>()
            .FirstOrDefault(b => b.Command == ApriCommand);

        if (existingBinding != null)
        {
            this.InputBindings.Remove(existingBinding);
        }

        // 3. Add the brand new physical key listener to the window
        KeyBinding newShortcut = new(ApriCommand, italiano ? Key.F12 : Key.O, ModifierKeys.Control);
        this.InputBindings.Add(newShortcut);
    }

    private void NuovoEditor_Executed(object sender, ExecutedRoutedEventArgs e) => App.DockingHost.OpenEditorDocument();
    private void NewViewer_Click(object sender, RoutedEventArgs e) => App.DockingHost.OpenViewerDocument();

    private void ApriEditor_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        App.DockingHost.OpenEditorDocumentFromFile(this);
    }

    private void ChiudiEditor_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        App.DockingHost.CloseActiveContent();
    }

    private void SalvaEditor_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        App.DockingHost.SaveActiveEditor();
    }

    private void SalvaCome_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        App.DockingHost.SaveActiveEditorAs();
    }

    private void DeleteText_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        // Enable the menu item only if a text box is focused and has selected text
        if (FocusManager.GetFocusedElement(this) is RichTextBox rtb)
        {
            e.CanExecute = !rtb.Selection.IsEmpty && !rtb.IsReadOnly;
            e.Handled = true;
        }
    }

    private void DeleteText_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        // Clear out the selected text
        if (FocusManager.GetFocusedElement(this) is RichTextBox rtb)
        {
            rtb.Selection.Text = string.Empty;
            e.Handled = true;
        }
    }

    private void Allineamento_SubmenuOpened(object sender, RoutedEventArgs e)
    {
        if (!ReferenceEquals(sender, e.Source))
            return;

        MiAlignLeft.IsChecked = false;
        MiAlignCenter.IsChecked = false;
        MiAlignRight.IsChecked = false;
        MiAlignJustify.IsChecked = false;

        EditorDocumentView? edv = App.DockingHost.GetActiveEditor();

        if (edv == null)
            return;

        object value = edv.Editor.Selection.GetPropertyValue(Block.TextAlignmentProperty);

        // If the selection spans paragraphs with different alignment values,
        // WPF returns DependencyProperty.UnsetValue.
        if (value == DependencyProperty.UnsetValue)
            return;

        if (value is TextAlignment alignment)
        {
            MiAlignLeft.IsChecked = alignment == TextAlignment.Left;
            MiAlignCenter.IsChecked = alignment == TextAlignment.Center;
            MiAlignRight.IsChecked = alignment == TextAlignment.Right;
            MiAlignJustify.IsChecked = alignment == TextAlignment.Justify;
        }
    }

    private void Search_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        //App.DockingHost.ShowTool("tool.textgen"); 

        // Assicurati che Testi sia pronto prima di creare la view
        if (Testi == null)
            return; // oppure mostra un messaggio / lascia l’overlay attivo

        App.DockingHost.ShowTool("tool.search");
    }

    private void Mostra_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        //App.DockingHost.ShowTool("tool.textgen"); 

        // Assicurati che Testi sia pronto prima di creare la view
        if (Testi == null)
            return; // oppure mostra un messaggio / lascia l’overlay attivo

        App.DockingHost.ShowTool("tool.textgen");
    }

    private void Converter_Executed(object sender, ExecutedRoutedEventArgs e) => App.DockingHost.ShowTool("tool.converter");
    private void Options_Executed(object sender, ExecutedRoutedEventArgs e) => App.DockingHost.ShowTool("tool.options");

    private void About_Click(object sender, RoutedEventArgs e)
    {
        Version? versione = Assembly.GetExecutingAssembly().GetName().Version;
        string v = versione != null ? versione.ToString() : "";
        MessageBoxLPN.Show(this,
            ((string)(Application.Current.TryFindResource("InformazioniMessaggio") ?? "About LaParola")).Replace("VLPN", v).Replace("\\n", "\n"),
            (string)(Application.Current.TryFindResource("InformazioniTitolo") ?? "About LaParola"));
    }

    private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        settings.WindowState = WindowState;
        if (WindowState == WindowState.Normal)
        {
            settings.WindowTop = Top;
            settings.WindowLeft = Left;
            settings.WindowWidth = Width;
            settings.WindowHeight = Height;
        }
        else
        {
            // If maximized/minimized, save the *restore bounds*
            settings.WindowTop = RestoreBounds.Top;
            settings.WindowLeft = RestoreBounds.Left;
            settings.WindowWidth = RestoreBounds.Width;
            settings.WindowHeight = RestoreBounds.Height;
        }

        // 1) Chiudi editor docs (non devono essere salvati/restaurati)
        CloseDocumentsByPrefix("doc.editor.");

        // 2) Snapshot viewer docs -> placeholder state
        settings.ViewerWindows = CaptureViewerStates();

        // 3) Salva layout XML
        settings.DockLayoutXml = SerializeDockLayoutToString();

        // 4) Salva sempre le impostazioni
        App.Settings.Save(settings);
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        App.DockingHost.BeginShutdown();

        if (!App.DockingHost.ConfirmCloseAll())
        {
            e.Cancel = true;
            App.DockingHost.CancelShutdown();
            return;
        }

        base.OnClosing(e);
    }

    private void CloseDocumentsByPrefix(string prefix)
    {
        LayoutRoot? root = Dock.Layout;
        if (root == null)
        {
            return;
        }

        List<LayoutDocument> docs = [.. root.Descendents()
                       .OfType<LayoutDocument>()
                       .Where(d => (d.ContentId ?? "").StartsWith(prefix))];

        foreach (LayoutDocument d in docs)
        {
            d.Close(); // rimuove il documento dal layout
        }
    }

    private List<ViewerWindowState> CaptureViewerStates()
    {
        LayoutRoot? root = Dock.Layout;
        if (root == null)
        {
            return [];
        }

        List<LayoutDocument> viewers = [.. root.Descendents()
                          .OfType<LayoutDocument>()
                          .Where(d => (d.ContentId ?? "").StartsWith("doc.viewer."))];

        // TODO2 Placeholder: per ora non hai ancora l’identificatore vero.
        // Quindi salva dati fittizi/minimi: ContentId + stringhe placeholder.
        return [.. viewers.Select(d => new ViewerWindowState
        {
            ContentId = d.ContentId ?? "",
            DisplayName = "PLACEHOLDER_TITLE", // TODO2
            VerseRef = "PLACEHOLDER_REF"
        })];
    }

    private string SerializeDockLayoutToString()
    {
        XmlLayoutSerializer serializer = new(Dock);

        StringBuilder sb = new();
        using (StringWriter writer = new(sb))
        {
            serializer.Serialize(writer);  // supportato [2](blob:https://www.microsoft365.com/34ef1840-738c-4d0c-b09d-2cdf559636bd)[1](https://blog.csdn.net/qq_41375318/article/details/149233645)
        }
        return sb.ToString();
    }

    private void Esci_Executed(object sender, ExecutedRoutedEventArgs e) => Close();

}
