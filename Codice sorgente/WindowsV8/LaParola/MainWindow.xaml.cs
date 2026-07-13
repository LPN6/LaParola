using AvalonDock;
using AvalonDock.Layout;
using AvalonDock.Layout.Serialization;
using LaParola.DocumentViews;
using LaParola.Models;
using LaParola.ToolViews;
using LaParola.Utilities;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace LaParola;

// per passare SmartScreen, usa https://www.microsoft.com/en-us/wdsi/filesubmission 
// per controllare il contenuto di un FlowDocument, System.Windows.Markup.XamlWriter.Save(finalDoc) in Immediate Window

// TODO2 versione 8.0.5

// TODO2 in TextGen, multiple versioni, hover non cambia colore; anche solo uno commentario ma con alternare
// TODO2 rimuovere righe vuote addizionali, soprattutto in TextGen

// TODO2 toolbar: new, open, save (all), print, undo, redo, find, cut, copy, paste, vis bibbia, commentario, apri note, segnalibri, navigare, ricerca, mostra, (chiave), (racc info), paralleli, LQ, Misure, Gesti testi, aggiorna, opzioni,aiuto
// available icons are listed here: https://pictogrammers.com/library/mdi/ and https://pictogrammers.github.io/@mdi/font/5.4.55/ 
// TODO2 oppure invece di un toolbar, creare un tool "Quick Access", come in Logos
// TODO2 help centre: Search, FAQs, tutorials, contact, release notes, keyboard shortcuts, about, documentation, how to use, getting started
// TODO2 right click menu for Editor and Visualizza

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
    private LibraryToolView? _libraryView;

    private object SearchToolViewInstance()
        => _searchView ??= new SearchToolView();

    private object TextGenToolViewInstance()
        => _textGenView ??= new TextGeneratorToolView();

    private object ConverterToolViewInstance()
        => _converterView ??= new ConverterToolView();

    private object OptionsToolViewInstance()
        => _optionsView ??= new OptionsToolView();

    private object LibraryToolViewInstance()
        => _libraryView ??= new LibraryToolView();

    internal static Texts Testi = null!;
    internal static AppSettings settings = new();

    private IInputElement? _previousFocus;

    public static readonly RoutedUICommand NuovoCommand = new("NewEditor", "NuovoCommand", typeof(MainWindow));
    public static readonly RoutedUICommand ApriCommand = new("OpenEditor", "ApriCommand", typeof(MainWindow));
    public static readonly RoutedUICommand ChiudiCommand = new("Close", "ChiudiCommand", typeof(MainWindow));
    public static readonly RoutedUICommand SalvaCommand = new("Save", "SalvaCommand", typeof(MainWindow));
    public static readonly RoutedUICommand SalvaComeCommand = new("SaveAs", "SalvaComeCommand", typeof(MainWindow));
    public static readonly RoutedUICommand EsciCommand = new("Exit", "EsciCommand", typeof(MainWindow));
    public static readonly RoutedUICommand FindNextCommand = new("FindNext", "FindNextCommand", typeof(MainWindow));
    public static readonly RoutedUICommand ReplaceNextCommand = new("ReplaceNext", "ReplaceNextCommand", typeof(MainWindow));
    public static readonly RoutedUICommand ImageCommand = new("Image", "ImageCommand", typeof(MainWindow));
    public static readonly RoutedUICommand LibraryCommand = new("Library", "LibraryCommand", typeof(MainWindow));
    public static readonly RoutedUICommand SearchCommand = new("Search", "SearchCommand", typeof(MainWindow));
    public static readonly RoutedUICommand FontCommand = new("Font", "FontCommand", typeof(MainWindow));
    public static readonly RoutedUICommand MostraCommand = new("Mostra", "MostraCommand", typeof(MainWindow));
    public static readonly RoutedUICommand ConverterCommand = new("Converter", "ConverterCommand", typeof(MainWindow));
    public static readonly RoutedUICommand OptionsCommand = new("Options", "OptionsCommand", typeof(MainWindow));

    internal static readonly string LPN_ANCORA = "LPN_ANCORA_";
    internal static Regex AncoraRegEx = new(LPN_ANCORA + @".*?(\d{8})");

    public MainWindow(AppSettings settingsLoaded)
    {
        settings = settingsLoaded;

        InitializeComponent();

        RestoreWindowPlacement();
        App.DockingHost.Initialize(Dock, DocumentPane, this);

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
            Testi.Formato = settings.Formato;

            if (settings.UltimaBibbia != "")
                Testi.UltimaBibbia = settings.UltimaBibbia;
            if (settings.UltimaBibbiaCompleta != "")
                Testi.UltimaBibbiaCompleta = settings.UltimaBibbiaCompleta;

            if (settings.Lingua == "it")
            {
                // TODO2 salvati in settings, da cambiare in Opzioni
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

            // Ora ripristina layout
            RestoreDockLayout();

            ShowLoadingOverlay(false);

            App.DockingHost.ActiveEditorChanged += (_, _) =>
            {
                UpdateEditorMenuState();
            };

            AggiornaMenuVisualizza();

            if (Testi.NomiVersioni().Count == 0)
            {
                MessageBoxLPN.Show(this, // TODO2 cambiare link per nuovi testi (in risorse x 2)
                    (string)(Application.Current.TryFindResource("MainNessunaVersione") ?? "No text was found. Go to https://www.laparola.net/programma/windowsbeta.php to install texts to read."),
                    (string)(Application.Current.TryFindResource("Errore") ?? "Error"));
            }

            // imposta dizionari
            if (Funzioni.LinguaPrincipale(Testi.Info(settings.DizionarioInglese).Lingua) != "en")
                settings.DizionarioInglese = "";
            if (Funzioni.LinguaPrincipale(Testi.Info(settings.DizionarioItaliano).Lingua) != "it")
                settings.DizionarioItaliano = "";
            if (Funzioni.LinguaPrincipale(Testi.Info(settings.DizionarioGreco).Lingua) != "el")
                settings.DizionarioGreco = "";
            if (Funzioni.LinguaPrincipale(Testi.Info(settings.DizionarioEbraico).Lingua) != "he")
                settings.DizionarioEbraico = "";
            if (Funzioni.LinguaPrincipale(Testi.Info(settings.DizionarioLatino).Lingua) != "la")
                settings.DizionarioLatino = "";

            if (string.IsNullOrEmpty(settings.DizionarioInglese) && Testi.Info("International Standard Bible Encyclopedia").Lingua == "en")
                settings.DizionarioInglese = "International Standard Bible Encyclopedia";
            if (string.IsNullOrEmpty(settings.DizionarioInglese) && Testi.Info("Easton's Bible Dictionary").Lingua == "en")
                settings.DizionarioInglese = "Easton's Bible Dictionary";
            if (string.IsNullOrEmpty(settings.DizionarioInglese) && Testi.Info("Torrey's New Topical Textbook").Lingua == "en")
                settings.DizionarioInglese = "Torrey's New Topical Textbook";
            if (string.IsNullOrEmpty(settings.DizionarioInglese) && Testi.Info("Smith's Bible Dictionary").Lingua == "en")
                settings.DizionarioInglese = "Smith's Bible Dictionary";
            if (string.IsNullOrEmpty(settings.DizionarioInglese) && Testi.Info("Nave's Topical Bible").Lingua == "en")
                settings.DizionarioInglese = "Nave's Topical Bible";
            if (string.IsNullOrEmpty(settings.DizionarioInglese) && Testi.Info("Hitchcock's Bible Names Dictionary").Lingua == "en")
                settings.DizionarioInglese = "Hitchcock's Bible Names Dictionary";

            if (string.IsNullOrEmpty(settings.DizionarioItaliano) && Testi.Info("Enciclopedia biblica").Lingua == "it")
                settings.DizionarioItaliano = "Enciclopedia biblica";
            if (string.IsNullOrEmpty(settings.DizionarioItaliano) && Testi.Info("Note della Nuova Riveduta").Lingua == "it")
                settings.DizionarioItaliano = "Note della Nuova Riveduta";

            if (settings.Lingua.Length >= 2 && settings.Lingua[..2].Equals("IT", StringComparison.InvariantCultureIgnoreCase))
            {
                if (string.IsNullOrEmpty(settings.DizionarioGreco) && Funzioni.LinguaPrincipale(Testi.Info("Vocabolario del Nuovo Testamento").Lingua) == "el")
                    settings.DizionarioGreco = "Vocabolario del Nuovo Testamento";
            }
            if (string.IsNullOrEmpty(settings.DizionarioGreco) && Funzioni.LinguaPrincipale(Testi.Info("Strong's Greek Dictionary").Lingua) == "el")
                settings.DizionarioGreco = "Strong's Greek Dictionary";
            if (string.IsNullOrEmpty(settings.DizionarioEbraico) && Funzioni.LinguaPrincipale(Testi.Info("Strong's Hebrew Dictionary").Lingua) == "he")
                settings.DizionarioEbraico = "Strong's Hebrew Dictionary";
            if (string.IsNullOrEmpty(settings.DizionarioLatino) && Funzioni.LinguaPrincipale(Testi.Info("Words Latin Dictionary").Lingua) == "la")
                settings.DizionarioLatino = "Words Latin Dictionary";

            Collection<string> dizionariTutti = Testi.NomiVersioni(TestoTipi.Dizionario);
            foreach (string dizionario in dizionariTutti)
            {
                switch (Funzioni.LinguaPrincipale(Testi.Info(dizionario).Lingua))
                {
                    case "en":
                        if (string.IsNullOrEmpty(settings.DizionarioInglese))
                            settings.DizionarioInglese = dizionario;
                        break;
                    case "it":
                        if (string.IsNullOrEmpty(settings.DizionarioItaliano))
                            settings.DizionarioItaliano = dizionario;
                        break;
                    case "el":
                        if (string.IsNullOrEmpty(settings.DizionarioGreco))
                            settings.DizionarioGreco = dizionario;
                        break;
                    case "he":
                        if (string.IsNullOrEmpty(settings.DizionarioEbraico))
                            settings.DizionarioEbraico = dizionario;
                        break;
                    case "la":
                        if (string.IsNullOrEmpty(settings.DizionarioLatino))
                            settings.DizionarioLatino = dizionario;
                        break;
                }
            }
        }
    }

    public void AggiornaMenuVisualizza()
    {
        Collection<string> versioni = Testi.NomiVersioni(TestoTipi.Bibbia);

        if (versioni.Count == 1)
        {
            // CASO 1: Una sola Bibbia. Trasformiamo il menu in un pulsante diretto.
            MenuVisualizzaBibbia.Header = versioni[0]; // Sostituisce il testo generico "Bibbia" con es. "Nuova Riveduta"
            MenuVisualizzaBibbia.ItemsSource = null;     // Rimuove il sottomenu a tendina
            MenuVisualizzaBibbia.Visibility = Visibility.Visible; // Ensure it's visible if it was hidden before
        }
        else if (versioni.Count == 0)
        {
            // CASO 2: Nessuna Bibbia disponibile. Nascondiamo il menu.
            MenuVisualizzaBibbia.Visibility = Visibility.Collapsed;
        }
        else
        {
            // CASO 3: Più Bibbie disponibili. Manteniamo il comportamento standard con sottomenu.
            MenuVisualizzaBibbia.Header = ((string)(Application.Current.TryFindResource("MenuVisualizzaBibbia") ?? "_Bible")); // Reset to default generic string resource
            MenuVisualizzaBibbia.ItemsSource = versioni;
            MenuVisualizzaBibbia.Visibility = Visibility.Visible;
        }

        Collection<string> commentari = Testi.NomiVersioni(TestoTipi.Commentario);
        if (commentari.Count == 1)
        {
            MenuVisualizzaCommentario.Header = commentari[0]; // Sostituisce il testo generico "Bibbia" con es. "Nuova Riveduta"
            MenuVisualizzaCommentario.ItemsSource = null;     // Rimuove il sottomenu a tendina
            MenuVisualizzaCommentario.Visibility = Visibility.Visible; // Ensure it's visible if it was hidden before
        }
        else if (commentari.Count == 0)
        {
            MenuVisualizzaCommentario.Visibility = Visibility.Collapsed;
        }
        else
        {
            MenuVisualizzaCommentario.Header = ((string)(Application.Current.TryFindResource("MenuVisualizzaCommentario") ?? "_Commentary")); // Reset to default generic string resource
            MenuVisualizzaCommentario.ItemsSource = commentari;
            MenuVisualizzaCommentario.Visibility = Visibility.Visible;
        }

        Collection<string> dizionari = Testi.NomiVersioni(TestoTipi.Dizionario);
        if (dizionari.Count == 1)
        {
            MenuVisualizzaDizionario.Header = dizionari[0];
            MenuVisualizzaDizionario.ItemsSource = null;
            MenuVisualizzaDizionario.Visibility = Visibility.Visible;
        }
        else if (dizionari.Count == 0)
        {
            MenuVisualizzaDizionario.Visibility = Visibility.Collapsed;
        }
        else
        {
            MenuVisualizzaDizionario.Header = ((string)(Application.Current.TryFindResource("MenuVisualizzaDizionario") ?? "_Dictionary")); // Reset to default generic string resource
            MenuVisualizzaDizionario.ItemsSource = dizionari;
            MenuVisualizzaDizionario.Visibility = Visibility.Visible;
        }

        Collection<string> libri = Testi.NomiVersioni(TestoTipi.Libro);
        if (libri.Count == 1)
        {
            MenuVisualizzaLibro.Header = libri[0];
            MenuVisualizzaLibro.ItemsSource = null;
            MenuVisualizzaLibro.Visibility = Visibility.Visible;
        }
        else if (libri.Count == 0)
        {
            MenuVisualizzaLibro.Visibility = Visibility.Collapsed;
        }
        else
        {
            MenuVisualizzaLibro.Header = ((string)(Application.Current.TryFindResource("MenuVisualizzaLibro") ?? "_Book")); // Reset to default generic string resource
            MenuVisualizzaLibro.ItemsSource = libri;
            MenuVisualizzaLibro.Visibility = Visibility.Visible;
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
            //HideDefaultToolWindows();

            string testo = "Questa è la versione beta (di StringheBranoCommentario) di LaParola 8.\n\n" +
                "Usa il menu 'Visualizza' per leggere la Bibbia. Usa il menu 'Strumenti' per altre possibili azioni da eseguire. Altre funzionalità saranno aggiunte prossimamente.\n\n" +
                "La disposizione delle finestre è molto flessibile: trascinando il titolo di una finestra, potete spostare, ancorare, affiancare e sovrapporre le finestre come preferite. Potete aprire più finestre e organizzarle come volete.\n\n" +
                "Se trovi dei problemi e hai dei suggerimenti, scrivimi a info@laparola.net.\n\n" +
                "-----------------------------------------\n\n" +
                "This is the beta version of LaParola 8.\n\n" +
                "Use the 'View' menu to read the Bible. Use the 'Tools' menu for other possible actions. Further features will be added soon.\n\n" +
"The window layout is very flexible: by dragging a window's title bar, you can move, dock, tile or overlap windows as you wish. You can open multiple windows and arrange them in any way.\n\n" +
"If you encounter any issues or have suggestions, please email me at info@laparola.net.";
            CreaEditorDocument(testo, ((string)(Application.Current.TryFindResource("MenuAbout") ?? "About LaParola")).Replace("_", ""));

            bool versioneVisualizzata = false;
            if (Testi.VersioneEsiste("Nuova Riveduta"))
            {
                VisualizzaBibbia("Nuova Riveduta");
                versioneVisualizzata = true;
            }
            if (Testi.VersioneEsiste("C.E.I."))
            {
                VisualizzaBibbia("C.E.I.");
                versioneVisualizzata = true;
            }
            if (!versioneVisualizzata)
            {
                string nome = "";
                if (Testi.NomiVersioni(TestoTipi.Bibbia).Count > 0)
                    nome = Testi.NomiVersioni(TestoTipi.Bibbia)[0];
                else if (Testi.NomiVersioni(TestoTipi.Commentario).Count > 0)
                    nome = Testi.NomiVersioni(TestoTipi.Commentario)[0];
                if (!string.IsNullOrEmpty(nome))
                    VisualizzaBibbia(nome);
                // else non visualizziamo nessuna versione, ci sarà un messaggio che nessuna versione è installata
            }

            return;
        }

        XmlLayoutSerializer serializer = new(Dock);

        // Prepara una lookup rapida degli stati viewer
        Dictionary<string, ViewerWindowState> viewerById = settings.ViewerWindows.ToDictionary(v => v.ContentId);

        serializer.LayoutSerializationCallback += (_, args) =>
        {
            string id = args.Model.ContentId ?? "";

            // Tool panes: hanno ContentId fissi. Qui puoi restituire
            // le istanze già presenti (se le hai nominate con x:Name) oppure crearle.
            if (id == "tool.search") { args.Content = SearchToolViewInstance(); return; }
            if (id == "tool.textgen") { args.Content = TextGenToolViewInstance(); return; }
            if (id == "tool.converter") { args.Content = ConverterToolViewInstance(); return; }
            if (id == "tool.options") { args.Content = OptionsToolViewInstance(); return; }
            if (id == "tool.library") { args.Content = LibraryToolViewInstance(); return; }

            // 2) Viewer docs: ricrea e applica placeholder state
            if (id.StartsWith("doc.viewer."))
            {
                if (viewerById.TryGetValue(id, out ViewerWindowState? state))
                {
                    if (state != null && Testi.VersioneEsiste(state.Versione))
                    {
                        ViewerDocumentView view = new(state.Versione);
                        if (state.VersettoMostrato)
                            _ = view.SpostaTesto(state.Libro, state.Capitolo, state.Versetto, true, false);
                        else
                            _ = view.SpostaTesto(state.Titolo, true, false);
                        view.IsTocVisible = state.IsSommarioVisibile;
                        view.SincGruppo = state.SincGruppo;
                        args.Content = view;
                    }
                    else
                    {
                        args.Cancel = true;
                    }
                }
                else
                {
                    args.Cancel = true;
                }
                return;
            }

            // 3) Editor docs: NON ricreare contenuti -> così non vengono ripristinati
            if (id.StartsWith("doc.editor.") || id.StartsWith("doc.immagine."))
            {
                args.Cancel = true;
                return;
            }
        };

        using StringReader reader = new(settings.DockLayoutXml);
        serializer.Deserialize(reader); // supportato https://www.microsoft365.com/34ef1840-738c-4d0c-b09d-2cdf559636bd)[1](https://blog.csdn.net/qq_41375318/article/details/149233645)
    }

    internal static void CreaEditorDocument(string testo, string titolo)
    {
        FlowDocument doc = new()
        {
            FontFamily = new FontFamily("Georgia"),
            FontSize = 15,
            Tag = titolo
        };
        doc.Blocks.Clear();
        doc.Blocks.Add(new Paragraph(new Run(testo)));
        App.DockingHost.OpenEditorDocument(doc, titolo);
    }

    //private void HideDefaultToolWindows()
    //{
    //TextGenToolAnchorable?.Hide();
    //ConverterToolAnchorable?.Hide();
    //OptionsToolAnchorable?.Hide();
    //}

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
        //MenuChiudi.IsEnabled = App.DockingHost.HasClosableContent;
    }

    internal void UpdateShortcutBindings(string lingua)
    {
        // per comandi che operano sul documento attivo (come Trova, Sostituisci),
        // bisogna anche aggiungere code nel costruttore dell'Editor

        bool italiano = lingua.StartsWith("it", StringComparison.CurrentCultureIgnoreCase);

        // 1. Update the visual text displayed in the menu
        MenuApri.InputGestureText = italiano ? "Ctrl+F12" : "Ctrl+O";

        // 2. Remove the old key binding if it exists (to avoid duplicate triggers)
        KeyBinding? existingBinding = this.InputBindings
            .OfType<KeyBinding>()
            .FirstOrDefault(b => b.Command == ApriCommand);

        if (existingBinding != null)
        {
            this.InputBindings.Remove(existingBinding);
        }

        // 3. Add the brand new physical key listener to the window
        KeyBinding newShortcut = new(ApriCommand, italiano ? Key.F12 : Key.O, ModifierKeys.Control);
        this.InputBindings.Add(newShortcut);

        // MenuTrova
        MenuTrova.InputGestureText = italiano ? "Ctrl+T" : "Ctrl+F";
        existingBinding = this.InputBindings
    .OfType<KeyBinding>()
    .FirstOrDefault(b => b.Command == ApplicationCommands.Find);
        if (existingBinding != null)
        {
            this.InputBindings.Remove(existingBinding);
        }
        newShortcut = new(ApplicationCommands.Find, italiano ? Key.T : Key.F, ModifierKeys.Control);
        this.InputBindings.Add(newShortcut);

        // MenuSostituisci
        MenuSostituisci.InputGestureText = italiano ? "Ctrl+U" : "Ctrl+H";
        existingBinding = this.InputBindings
    .OfType<KeyBinding>()
    .FirstOrDefault(b => b.Command == ApplicationCommands.Replace);
        if (existingBinding != null)
        {
            this.InputBindings.Remove(existingBinding);
        }
        newShortcut = new(ApplicationCommands.Replace, italiano ? Key.U : Key.H, ModifierKeys.Control);
        this.InputBindings.Add(newShortcut);

        // MenuBiblioteca
        MenuBiblioteca.InputGestureText = italiano ? "Ctrl+B" : "Ctrl+L";
        existingBinding = this.InputBindings
    .OfType<KeyBinding>()
    .FirstOrDefault(b => b.Command == LibraryCommand);
        if (existingBinding != null)
        {
            this.InputBindings.Remove(existingBinding);
        }
        newShortcut = new(LibraryCommand, italiano ? Key.B : Key.L, ModifierKeys.Control);
        this.InputBindings.Add(newShortcut);
    }

    private void NuovoEditor_Executed(object sender, ExecutedRoutedEventArgs e) => App.DockingHost.OpenEditorDocument();

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

    private void Font_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        EditorDocumentView? edv = App.DockingHost.GetActiveEditor();

        if (edv == null)
            return;

        // Grab the active text selection (represents highlighted text OR the empty caret typing position)
        TextSelection selection = edv.Editor.Selection;

        // 1. Safely extract current formatting properties, using type-patterns to gracefully fall back if mixed selections return UnsetValue
        var fontFamVal = selection.GetPropertyValue(TextElement.FontFamilyProperty);
        FontFamily currentFontFamily = fontFamVal is FontFamily ff ? ff : edv.Editor.FontFamily;

        var fontSizeVal = selection.GetPropertyValue(TextElement.FontSizeProperty);
        double currentFontSize = (fontSizeVal is double fs) ? fs * 3.0 / 4.0 : edv.Editor.FontSize * 3.0 / 4.0;

        var fontWeightVal = selection.GetPropertyValue(TextElement.FontWeightProperty);
        bool isBold = fontWeightVal is FontWeight fw && fw == FontWeights.Bold;

        var fontStyleVal = selection.GetPropertyValue(TextElement.FontStyleProperty);
        bool isItalic = fontStyleVal is FontStyle fst && fst == FontStyles.Italic;

        var textDecVal = selection.GetPropertyValue(Inline.TextDecorationsProperty);
        bool isUnderline = textDecVal is TextDecorationCollection tdc && tdc.Count > 0;

        var foregroundVal = selection.GetPropertyValue(TextElement.ForegroundProperty);
        Brush currentBrush = foregroundVal is Brush b ? b : edv.Editor.Foreground;
        string currentColorStr = currentBrush?.ToString() ?? "Black";

        // 2. Initialize your dark-mode aware custom dialog with the extracted properties
        FontDialog dlg = new(
            allowSuperscript: false,
            initialFont: currentFontFamily.ToString(),
            initialSize: (float)currentFontSize,
            bold: isBold,
            italic: isItalic,
            underline: isUnderline,
            superscript: false,
            initialColor: currentColorStr
        );

        if (dlg.ShowDialog() == true)
        {
            // 3. Apply changes via ApplyPropertyValue.
            // WPF NATIVE ADVANTAGE: If text is highlighted, this modifies the selection. 
            // If no text is highlighted, this automatically updates the typing formatting at the caret.
            selection.ApplyPropertyValue(TextElement.FontFamilyProperty, dlg.SelectedFontFamily);
            selection.ApplyPropertyValue(TextElement.FontSizeProperty, (double)dlg.SelectedFontSize * 4.0 / 3.0);
            selection.ApplyPropertyValue(TextElement.FontWeightProperty, dlg.SelectedBold ? FontWeights.Bold : FontWeights.Normal);
            selection.ApplyPropertyValue(TextElement.FontStyleProperty, dlg.SelectedItalic ? FontStyles.Italic : FontStyles.Normal);
            selection.ApplyPropertyValue(Inline.TextDecorationsProperty, dlg.SelectedUnderline ? TextDecorations.Underline : null);

            // 4. Safely parse and build a SolidColorBrush for the text foreground
            try
            {
                if (ColorConverter.ConvertFromString(dlg.SelectedBrush) is Color chosenColor)
                {
                    selection.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(chosenColor));
                }
            }
            catch
            {
                // Fallback: If parsing fails, leave the current foreground color completely intact
            }
        }
    }

    private void Allineamento_SubmenuOpened(object sender, RoutedEventArgs e)
    {
        if (!ReferenceEquals(sender, e.Source))
            return;

        MenuAlignLeft.IsChecked = false;
        MenuAlignCenter.IsChecked = false;
        MenuAlignRight.IsChecked = false;
        MenuAlignJustify.IsChecked = false;

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
            MenuAlignLeft.IsChecked = alignment == TextAlignment.Left;
            MenuAlignCenter.IsChecked = alignment == TextAlignment.Center;
            MenuAlignRight.IsChecked = alignment == TextAlignment.Right;
            MenuAlignJustify.IsChecked = alignment == TextAlignment.Justify;
        }
    }

    private void Find_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        // Verifica se esiste un editor aperto e attivo che contiene la RichTextBox
        if (FocusManager.GetFocusedElement(this) is RichTextBox rtb)
        {
            e.CanExecute = true;
            e.Handled = true;
            // a bit if a hack to do it here
            MenuSelezionaTutto.IsEnabled = !rtb.IsReadOnly;
            MenuCarattere.IsEnabled = !rtb.IsReadOnly;
            MenuAlign.IsEnabled = !rtb.IsReadOnly;
            MenuTrova.IsEnabled = !rtb.IsReadOnly;
            MenuTrovaProssima.IsEnabled = !rtb.IsReadOnly;
            MenuSostituisci.IsEnabled = !rtb.IsReadOnly;
            MenuSostituisciProssima.IsEnabled = !rtb.IsReadOnly;
        }
        else
        {
            MenuCarattere.IsEnabled = false;
            MenuAlign.IsEnabled = false;
            MenuTrovaProssima.IsEnabled = false;
            MenuSostituisciProssima.IsEnabled = false;
        }
    }

    private void Find_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        EditorDocumentView? edv = App.DockingHost.GetActiveEditor();

        if (edv == null)
            return;

        edv.ShowFindDialog();
    }

    private void FindNext_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        EditorDocumentView? edv = App.DockingHost.GetActiveEditor();

        if (edv == null)
            return;

        edv.FindNext();
    }

    private void Replace_CanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        // Verifica se esiste un editor aperto e attivo che contiene la RichTextBox
        if (FocusManager.GetFocusedElement(this) is RichTextBox)
        {
            e.CanExecute = true;
            e.Handled = true;
        }
    }

    private void Replace_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        EditorDocumentView? edv = App.DockingHost.GetActiveEditor();

        if (edv == null)
            return;

        edv.ShowReplaceDialog();
    }

    private void ReplaceNext_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        EditorDocumentView? edv = App.DockingHost.GetActiveEditor();

        if (edv == null)
            return;

        edv.ReplaceNext();
    }

    // Questo si attiva solo se l'utente clicca sul menu principale (quando non ha sottomenu)
    private void MenuVisualizzaBibbia_Click(object sender, RoutedEventArgs e)
    {
        // Verifichiamo che il clic sia avvenuto proprio sul menu principale e che l'ItemsSource sia vuoto
        if (e.Source == sender && MenuVisualizzaBibbia.ItemsSource == null && MenuVisualizzaBibbia.Header is string versionName)
        {
            VisualizzaBibbia(versionName);
        }
    }

    private void MenuVisualizzaCommentario_Click(object sender, RoutedEventArgs e)
    {
        // Verifichiamo che il clic sia avvenuto proprio sul menu principale e che l'ItemsSource sia vuoto
        if (e.Source == sender && MenuVisualizzaCommentario.ItemsSource == null && MenuVisualizzaCommentario.Header is string commentarioNome)
        {
            VisualizzaCommentario(commentarioNome);
        }
    }

    private void MenuVisualizzaDizionario_Click(object sender, RoutedEventArgs e)
    {
        // Verifichiamo che il clic sia avvenuto proprio sul menu principale e che l'ItemsSource sia vuoto
        if (e.Source == sender && MenuVisualizzaDizionario.ItemsSource == null && MenuVisualizzaDizionario.Header is string dizionarioNome)
        {
            VisualizzaDizionario(dizionarioNome);
        }
    }

    private void MenuVisualizzaLibro_Click(object sender, RoutedEventArgs e)
    {
        // Verifichiamo che il clic sia avvenuto proprio sul menu principale e che l'ItemsSource sia vuoto
        if (e.Source == sender && MenuVisualizzaLibro.ItemsSource == null && MenuVisualizzaLibro.Header is string libroNome)
        {
            VisualizzaLibro(libroNome);
        }
    }

    private void SubMenuBibbia_Click(object sender, RoutedEventArgs e)
    {
        // Cast the sender to access the exact MenuItem that was clicked
        if (sender is MenuItem clickedItem && clickedItem.Header is string versionName)
        {
            // Pass the version name text straight to your processing function
            VisualizzaBibbia(versionName);
        }
    }

    private void SubMenuCommentario_Click(object sender, RoutedEventArgs e)
    {
        // Cast the sender to access the exact MenuItem that was clicked
        if (sender is MenuItem clickedItem && clickedItem.Header is string commentarioName)
        {
            // Pass the version name text straight to your processing function
            VisualizzaCommentario(commentarioName);
        }
    }

    private void SubMenuDizionario_Click(object sender, RoutedEventArgs e)
    {
        // Cast the sender to access the exact MenuItem that was clicked
        if (sender is MenuItem clickedItem && clickedItem.Header is string dizionarioName)
        {
            // Pass the version name text straight to your processing function
            VisualizzaDizionario(dizionarioName);
        }
    }

    private void SubMenuLibro_Click(object sender, RoutedEventArgs e)
    {
        // Cast the sender to access the exact MenuItem that was clicked
        if (sender is MenuItem clickedItem && clickedItem.Header is string libroName)
        {
            // Pass the version name text straight to your processing function
            VisualizzaLibro(libroName);
        }
    }

    public static void VisualizzaBibbia(string testoNome)
    {
        byte libro = 1;
        for (byte i = 1; i <= 73; ++i)
        {
            if (Testi.CapitoliInLibro(i, testoNome) > 0)
            {
                libro = i;
                break;
            }
        }
        VisualizzaBibbia(testoNome, libro, 1, 1);
    }

    public static void VisualizzaBibbia(string testoNome, byte libro, byte capitolo, byte versetto)
    {
        /*ViewerDocumentView? view =*/
        App.DockingHost.OpenViewerDocument(testoNome, libro, capitolo, versetto);
    }

    public static void VisualizzaCommentario(string testoNome)
    {
        try
        {
            string primoCommento = Testi.Note(testoNome)[0]; //#LLCCCVVV
            byte libro = byte.Parse(primoCommento.Substring(1, 2));
            byte capitolo = byte.Parse(primoCommento.Substring(3, 3));
            byte versetto = byte.Parse(primoCommento.Substring(6, 3));
            App.DockingHost.OpenViewerDocument(testoNome, libro, capitolo, versetto);
        }
        catch
        {
            App.DockingHost.OpenViewerDocument(testoNome, 1, 1, 1);
        }
    }

    public static void VisualizzaDizionario(string testoNome)
    {
        try
        {
            string notaDaAprire = Testi.NoteConTitolo(testoNome)[0]; // può dare exception
            App.DockingHost.OpenViewerDocument(testoNome, notaDaAprire);
        }
        catch
        { // per esempio nessuna nota su un tema, o testoNome inesistente
            App.DockingHost.OpenViewerDocument(testoNome, "");
        }
    }

    public static void VisualizzaLibro(string testoNome)
    {
        ViewerDocumentView? view;
        try
        {
            string notaDaAprire = "";
            Collection<string> noteOrdinate = Testi.GetNoteInOrdine(testoNome);
            if (noteOrdinate.Count > 0)
                notaDaAprire = noteOrdinate[0]; // l'indice (se esiste)
            if (string.IsNullOrEmpty(notaDaAprire) && noteOrdinate.Count > 1)
                notaDaAprire = noteOrdinate[1]; // la prima nota in ordine
            if (string.IsNullOrEmpty(notaDaAprire))
                notaDaAprire = Testi.NoteConTitolo(testoNome)[0]; // può dare exception
            view = App.DockingHost.OpenViewerDocument(testoNome, notaDaAprire);
        }
        catch
        { // per esempio nessuna nota su un tema, o testoNome inesistente
            view = App.DockingHost.OpenViewerDocument(testoNome, "");
        }
        view?.MostraIndice(true);
    }

    private void Image_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        {
            string[] selectedImagePaths = SelectImageFile();

            if (selectedImagePaths != null && selectedImagePaths.Length > 0)
            {
                ApriNomeImmagini(selectedImagePaths);
            }
        }
    }

    private static string[] SelectImageFile()
    {
        string ultimaCartellaImmagini = settings.UltimaCartellaImmagini;
        if (String.IsNullOrEmpty(ultimaCartellaImmagini))
        {
            string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LaParola");
            string baseDir = AppContext.BaseDirectory;

            // 2. Count the images in both folders
            int appDataImageCount = CountImagesInFolder(appDataPath);
            int baseDirImageCount = CountImagesInFolder(baseDir);

            // 3. Compare and return the winner. 
            // If they are equal (e.g., both have 0 images), default to AppContext.BaseDirectory
            ultimaCartellaImmagini = appDataImageCount > baseDirImageCount ? appDataPath : baseDir;
        }

        // Create an instance of the OpenFileDialog
        OpenFileDialog openFileDialog = new()
        {
            Title = (string)(Application.Current.TryFindResource("ImmaginiApriTitolo") ?? "Select One or More Images"),
            Filter = (string)(Application.Current.TryFindResource("ImmaginiFileFiltro") ?? "Image files (*.png;*.jpeg;*.jpg;*.gif;*.bmp)|*.png;*.jpeg;*.jpg;*.gif;*.bmp|All files (*.*)|*.*"),

            InitialDirectory = ultimaCartellaImmagini,
            Multiselect = true
        };

        // 4. Show the dialog box. ShowDialog() returns a nullable boolean (bool?).
        bool? result = openFileDialog.ShowDialog();

        // 5. If the user clicked OK, return the selected file path
        if (result == true)
        {
            return openFileDialog.FileNames;
        }

        // Return null or empty string if the user cancelled
        return [];
    }

    private static int CountImagesInFolder(string folderPath)
    {
        // Guard clause: If the folder doesn't exist, it has 0 images
        if (!Directory.Exists(folderPath)) return 0;

        // Define the image extensions we care about (with dots)
        string[] imageExtensions = [".jpg", ".jpeg", ".png", ".gif", ".bmp"];

        try
        {
            // Enumerate top-level files and count how many match our extension list
            return Directory.EnumerateFiles(folderPath, "*.*", SearchOption.TopDirectoryOnly)
                .Count(file => imageExtensions.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase));
        }
        catch (Exception)
        {
            // Fail safely if the folder is locked, corrupted, or inaccessible
            return 0;
        }
    }

    private static void ApriNomeImmagini(string[] fileDaAprire)
    {
        if (fileDaAprire == null || fileDaAprire.Length == 0)
            return;

        string? percorso = Path.GetDirectoryName(fileDaAprire[0]);
        if (percorso != null && percorso.Length > 0)
            settings.UltimaCartellaImmagini = percorso;
        foreach (string nomeFile in fileDaAprire)
        {
            App.DockingHost.OpenImmagineDocument(nomeFile);
        }
    }

    private void Library_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        // Assicurati che Testi sia pronto prima di creare la view
        if (Testi == null)
            return;

        App.DockingHost.ShowTool("tool.library");
    }

    private void Search_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        //App.DockingHost.ShowTool("tool.search"); 

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

    private void Options_Executed(object sender, ExecutedRoutedEventArgs e)
    {
        if (Testi == null)
            return; // oppure mostra un messaggio / lascia l’overlay attivo
        App.DockingHost.ShowTool("tool.options");
    }

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
        CloseDocumentsByPrefix("doc.immagine.");

        // 2) Snapshot viewer docs -> placeholder state
        settings.ViewerWindows = CaptureViewerStates();

        // 3) Salva layout XML
        settings.DockLayoutXml = SerializeDockLayoutToString();

        // salvare UltimaBibbia, che non viene aggiornato automaticamente durante l'utilizzo,
        // diversamente dalle altre impostazioni
        settings.UltimaBibbia = Testi.UltimaBibbia;
        settings.UltimaBibbiaCompleta = Testi.UltimaBibbiaCompleta;

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

        List<ViewerWindowState> states = [];
        foreach (LayoutDocument d in viewers)
        {
            // 1. Cast d.Content to your actual UserControl class name
            if (d.Content is ViewerDocumentView viewer)
            {
                // 2. Safely read the values straight out of the control
                states.Add(new ViewerWindowState
                {
                    ContentId = d.ContentId ?? "",
                    Versione = viewer.Versione,
                    VersettoMostrato = viewer.VersettoMostrato,
                    Libro = viewer.Libro,
                    Capitolo = viewer.Capitolo,
                    Versetto = viewer.Versetto,
                    Titolo = viewer.Titolo,
                    IsSommarioVisibile = viewer.IsTocVisible,
                    SincGruppo = viewer.SincGruppo
                });
            }
        }

        return states;
    }

    private string SerializeDockLayoutToString()
    {
        XmlLayoutSerializer serializer = new(Dock);

        StringBuilder sb = new();
        using (StringWriter writer = new(sb))
        {
            serializer.Serialize(writer);  // supportato (https://www.microsoft365.com/34ef1840-738c-4d0c-b09d-2cdf559636bd) (https://blog.csdn.net/qq_41375318/article/details/149233645)
        }
        return sb.ToString();
    }

    private void Esci_Executed(object sender, ExecutedRoutedEventArgs e) => Close();

    internal async static void LinkCliccato(int tipo, string nomeLink)
    {
        nomeLink = Uri.UnescapeDataString(nomeLink);
        switch (tipo)
        {
            case 1:
                string versioneDaUtilizzare;
                Riferimento riferimento;
                (versioneDaUtilizzare, riferimento) = VersionePerLinkBibbia(nomeLink);

                if (versioneDaUtilizzare == "")
                    break;

                if (riferimento.Count == 0)
                {
                    // non fare niente
                }
                // un + invece di un - nel riferimento indica sempre nella finestra Visualizza (usato per mostrare i risultati di una ricerca in contesto)
                else if ((riferimento.Count == 1 && riferimento.SoloUnoVersetto(0)) || nomeLink.Contains('+'))
                {
                    VisualizzaBibbia(versioneDaUtilizzare, riferimento.Brani[0][0], riferimento.Brani[0][1], riferimento.Brani[0][2]);
                }
                else
                {
                    FlowDocument doc = await MainWindow.Testi.FlowDocumentBranoAsync(riferimento, versioneDaUtilizzare);
                    doc.Tag = versioneDaUtilizzare;
                    Brush fg = (Brush)Application.Current.FindResource("AppForegroundBrush");
                    RtfColorTransformer.ApplyThemeToDocument(doc, true, fg, true);

                    string abbVersione = Testi.Info(versioneDaUtilizzare).Abbreviazione;
                    App.DockingHost.SendFlowDocumentToActiveEditor(doc, Testi.NormalizzaRiferimento(riferimento) + " (" + abbVersione + ")", versioneDaUtilizzare);
                }
                break;
            case 2:
                string collezioneNuovaNota = "";
                if (nomeLink.IndexOf('\\') > 0) // in questo modo, è possibile creare un link "Nuova Riveduta\#010010010000-01001002000"
                { // in versione.cs, il nome del commentario è stato nel link affinché possa aprire nello stesso commentario
                    collezioneNuovaNota = nomeLink[..nomeLink.IndexOf('\\')];
                    nomeLink = nomeLink[(nomeLink.IndexOf('\\') + 1)..];

                    // se la collezione richiesta non esiste, non fare niente
                    if (!Testi.VersioneEsiste(collezioneNuovaNota))
                        return;
                }


                if (!string.IsNullOrEmpty(collezioneNuovaNota))
                {
                    // old way was from View window to the same View window, now always in Editor window. Also because getting the View window is not easy
                    /*
                    if (finestra != null && finestra.Tag != null && finestra.Tag.ToString() == "Visualizza" && (((Visualizza)(finestra)).paneAttivo.TuttiTesti != TestoTipi.None || finestra.Text.StartsWith(collezioneNuovaNota, StringComparison.Ordinal)))
                    {
                        if (titoloNota.StartsWith("#", StringComparison.Ordinal))
                            finestra.SpostaTesto(Testi.ConvertiRiferimento(titoloNota), true);
                        else
                            finestra.SpostaTesto(titoloNota, true);
                    }
                    else
                        ApriNotaInEditor(titoloNota, collezioneNuovaNota);
                    */
                    if (!nomeLink.StartsWith('#') && Char.IsDigit(nomeLink[^1]) && Testi.GetNumeroNotaTitolo(collezioneNuovaNota, nomeLink) < 0)
                    { // riferimenti ai brani non con #, come Mt 1:21 -> Mt 1:1 in Note NR
                        Riferimento noteInBrano = Testi.ElencaNoteInBrano(Testi.ConvertiRiferimento(nomeLink), collezioneNuovaNota);
                        if (noteInBrano.Count > 0)
                        {
                            nomeLink = string.Join("", noteInBrano.Note);
                        }
                    }

                    await ApriNotaInEditor(nomeLink, collezioneNuovaNota);
                }
                break;
            case 3:
                string collezioneNota = "";
                if (nomeLink.IndexOf('\\') > 0) // in questo modo, è possibile creare un link "Nuova Riveduta\#010010010000-01001002000"
                { // in versione.cs, il nome del commentario è stato nel link affinché possa aprire nello stesso commentario
                    collezioneNota = nomeLink[..nomeLink.IndexOf('\\')];
                    nomeLink = nomeLink[(nomeLink.IndexOf('\\') + 1)..];

                    // se la collezione richiesta non esiste, non fare niente
                    if (!Testi.VersioneEsiste(collezioneNota))
                        return;
                }

                if (!File.Exists(nomeLink))
                {
                    string nomeDelFileDellaCollezione = Testi.Info(collezioneNota).NomeDelFile;
                    try
                    {
                        if (!string.IsNullOrEmpty(nomeDelFileDellaCollezione))
                        { // solo se la nota fa parte di una collezione
                            string? percorso = Path.GetDirectoryName(nomeDelFileDellaCollezione);
                            if (percorso != null)
                            {
                                string[] fileTrovati = Directory.GetFiles(percorso, nomeLink + ".*");
                                if (fileTrovati.Length > 0)
                                    nomeLink = fileTrovati[0];
                                else
                                { // proviamo anche nella sottocartella con lo stesso nome della collezione
                                    string[] fileTrovatiSotto = Directory.GetFiles(percorso + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(nomeDelFileDellaCollezione), nomeLink + ".*");
                                    if (fileTrovatiSotto.Length > 0)
                                        nomeLink = fileTrovatiSotto[0];
                                }
                            }
                        }
                    }
                    catch
                    {
                        // un Internet link, o altro testo che non è lecito per un percorso, dà un errore qui; basta saltare
                    }
                }
                if (!File.Exists(nomeLink))
                {
                    try
                    {
                        string[] fileTrovati = Directory.GetFiles(AppContext.BaseDirectory, nomeLink + ".*");
                        if (fileTrovati.Length > 0)
                            nomeLink = fileTrovati[0];
                    }
                    catch
                    {
                        // un Internet link, o altro testo che non è lecito per un percorso, dà un errore qui; basta saltare
                    }
                }
                if (!File.Exists(nomeLink))
                {
                    try
                    {
                        string[] fileTrovati = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola", nomeLink + ".*");
                        if (fileTrovati.Length > 0)
                            nomeLink = fileTrovati[0];
                    }
                    catch
                    {
                        // un Internet link, o altro testo che non è lecito per un percorso, dà un errore qui; basta saltare
                    }
                }
                // TODO2 cartelle extra per file
                /*
                    if (!File.Exists(titoloNota))
                    {
                        string[] cartelle = Settings.Default.CartelleDaCercare.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                        try
                        {
                            foreach (string cartella in cartelle)
                            {
                                string[] fileTrovati = Directory.GetFiles(cartella, titoloNota + ".*");
                                if (fileTrovati.Length > 0)
                                {
                                    titoloNota = fileTrovati[0];
                                    break;
                                }
                            }
                        }
                        catch
                        {
                            // un Internet link, o altro testo che non è lecito per un percorso, dà un errore qui; basta saltare
                        }
                    }
                */

                string estensione = Path.GetExtension(nomeLink);
                if (File.Exists(nomeLink) && (string.Compare(estensione, ".gif", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(estensione, ".jpg", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(estensione, ".jpeg", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(estensione, ".bmp", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(estensione, ".png", StringComparison.OrdinalIgnoreCase) == 0))
                    ApriNomeImmagini([nomeLink]);
                else
                {
                    try
                    {
                        Funzioni.AprilFileOUrl(nomeLink, true);
                    }
                    catch (Exception exc)
                    {
                        Application.Current?.Dispatcher.Invoke(() =>
                        {
                            Window owner = Application.Current.MainWindow;
                            MessageBoxLPN.Show(owner,
                                (string)(Application.Current.TryFindResource("LinkFileErrore") ?? "I could not open the file" + " " + nomeLink + ": " + exc.Message),
                                (string)(Application.Current.TryFindResource("Errore") ?? "Error"));
                        });
                    }
                }
                break;
            default:
                break;
        }
        return;
    }

    internal static void ApriDefinizioneDizionario(string voce, string versione)
    {
        if (string.IsNullOrEmpty(voce))
            return;

        string dizionario = "";

        bool cercaRadiceInDizionario = true;
        // se la parola è greca/ebraica in un testo in un'altra lingua, usa il dizionario appropriato
        // anche la radice della parola deve essere cercata nel dizionario greco/ebraico, non nella nomeVersione del testo
        char primaLettera = voce[0];
        if (Funzioni.IsLetteraGreca(primaLettera))
            dizionario = settings.DizionarioGreco;
        else if ((primaLettera >= '\u0591' && primaLettera <= '\u05ff') || (primaLettera >= '\ufb1e' && primaLettera <= '\u5b4f'))
            dizionario = settings.DizionarioEbraico;
        else
        {
            cercaRadiceInDizionario = string.IsNullOrEmpty(versione); // se non c'è nomeVersione (per es un file non in una Bibbia o collezione) controlliamo invece del dizionario
            foreach (string linguaDelTesto in Testi.Info(versione).Lingua.Split(['|'], StringSplitOptions.RemoveEmptyEntries))
            {
                if (string.IsNullOrEmpty(dizionario))
                {
                    switch (linguaDelTesto)
                    {
                        case "it":
                            dizionario = settings.DizionarioItaliano;
                            break;
                        case "en":
                            dizionario = settings.DizionarioInglese;
                            break;
                        case "el":
                            dizionario = settings.DizionarioGreco;
                            break;
                        case "he":
                            dizionario = settings.DizionarioEbraico;
                            break;
                        case "la":
                            dizionario = settings.DizionarioLatino;
                            break;
                    }
                }
            }
            if (string.IsNullOrEmpty(dizionario))
            {
                dizionario = settings.Lingua.ToUpperInvariant() switch
                {
                    "IT" => settings.DizionarioItaliano,
                    "EN" => settings.DizionarioInglese,
                    _ => settings.DizionarioInglese,
                };
            }
        }

        if (!string.IsNullOrEmpty(dizionario))
        {
            string testo = Testi.GetNotaTesto(voce, dizionario);
            if (string.IsNullOrEmpty(testo))
            {
                try
                {
                    string parolaAttualeTrovata = Testi.RadiceDiParola(voce, cercaRadiceInDizionario ? dizionario : versione);
                    voce = ((string.IsNullOrEmpty(parolaAttualeTrovata) && cercaRadiceInDizionario) ? Testi.RadiceDiParola(voce, versione) : parolaAttualeTrovata);
                }
                catch (TextNotExistException)
                { // succede quando la versione è stata cancellata
                    voce = "";
                }

                primaLettera = (!string.IsNullOrEmpty(voce) ? voce[0] : (char)0);
                if (Funzioni.IsLetteraGreca(primaLettera))
                    dizionario = settings.DizionarioGreco;
                else if ((primaLettera >= '\u0591' && primaLettera <= '\u05ff') || (primaLettera >= '\ufb1e' && primaLettera <= '\u5b4f'))
                    dizionario = settings.DizionarioEbraico;
                else if (primaLettera == 'H' && voce.Length >= 2 && voce[1] == '8')
                    dizionario = settings.DizionarioEbraico; // nel dizionario Strongs Hebrew, le voci di tempo voce modo hanno radice H8xxx

                // dizionario può essere "" se non c'è un dizionario ebraico installato
                testo = (string.IsNullOrEmpty(dizionario) ? "" : Testi.GetNotaTesto(voce, dizionario));
                if (string.IsNullOrEmpty(testo))
                    voce = "";
            }
        }

        if (string.IsNullOrEmpty(dizionario) || string.IsNullOrEmpty(voce))
            return;

        _ = ApriNotaInEditor(voce, dizionario);
    }

    internal static async Task ApriNotaInEditor(string titoloNota, string collezioneNuovaNota)
    {
        Riferimento riferimentoNota;
        string titoloToolbar;
        if (titoloNota.StartsWith('#'))
        {
            riferimentoNota = Testi.ConvertiRiferimento(Testi.ConvertiTitoloNotaARiferimento(titoloNota));
            titoloToolbar = Testi.NormalizzaRiferimento(riferimentoNota);
        }
        else
        {
            // apri nota su tema
            riferimentoNota = new(false);
            riferimentoNota.AggiungiNotaEParole(titoloNota, []);
            titoloToolbar = titoloNota;
        }
        FlowDocument doc = await MainWindow.Testi.FlowDocumentBranoAsync(riferimentoNota, collezioneNuovaNota);
        doc.Tag = collezioneNuovaNota;
        Brush fg = (Brush)Application.Current.FindResource("AppForegroundBrush");
        RtfColorTransformer.ApplyThemeToDocument(doc, true, fg, true);

        string abbVersione = Testi.Info(collezioneNuovaNota).Abbreviazione;
        App.DockingHost.SendFlowDocumentToActiveEditor(doc, titoloToolbar + " (" + abbVersione + ")", collezioneNuovaNota);
    }

    internal static (string versioneDaUtilizzare, Riferimento riferimento) VersionePerLinkBibbia(string nomeLink)
    {
        string versioneDaUtilizzare = "";
        if (nomeLink.IndexOf('\\') > 0) // in questo modo, è possibile creare un link "Nuova Riveduta\#010010010000-01001002000"
        {
            versioneDaUtilizzare = nomeLink[..nomeLink.IndexOf('\\')];
            nomeLink = nomeLink[(nomeLink.IndexOf('\\') + 1)..];
        }
        string riftestuale = Testi.ConvertiTitoloNotaARiferimento(nomeLink);
        Riferimento riferimento = Testi.ConvertiRiferimento(riftestuale);

        if (riferimento.Count > 0)
        {
            string versioneDaProvare;
            if (versioneDaUtilizzare == "")
            {
                versioneDaProvare = settings.BibbiaPreferita1;
                // controlliamo che il capitolo esista, per casi come Dan 13
                try
                {
                    if (Testi.CapitoliInLibro(riferimento.Brani[0][0], versioneDaProvare) >= riferimento.Brani[0][1])
                        versioneDaUtilizzare = versioneDaProvare;
                }
                catch { } // se testo non esiste più, o è ""
            }
            if (versioneDaUtilizzare == "")
            {
                versioneDaProvare = settings.BibbiaPreferita2;
                try
                {
                    if (Testi.CapitoliInLibro(riferimento.Brani[0][0], versioneDaProvare) >= riferimento.Brani[0][1])
                        versioneDaUtilizzare = versioneDaProvare;
                }
                catch { } // se testo non esiste più, o è ""
            }
            if (versioneDaUtilizzare == "")
            {
                versioneDaProvare = settings.BibbiaPreferita3;
                try
                {
                    if (Testi.CapitoliInLibro(riferimento.Brani[0][0], versioneDaProvare) >= riferimento.Brani[0][1])
                        versioneDaUtilizzare = versioneDaProvare;
                }
                catch { } // se testo non esiste più, o è ""
            }
            if (versioneDaUtilizzare == "")
            {
                versioneDaProvare = Testi.UltimaBibbia;
                try
                {
                    if (Testi.CapitoliInLibro(riferimento.Brani[0][0], versioneDaProvare) >= riferimento.Brani[0][1])
                        versioneDaUtilizzare = versioneDaProvare;
                }
                catch { } // se testo non esiste più, o è ""
            }
            if (versioneDaUtilizzare == "")
            {
                versioneDaProvare = Testi.UltimaBibbiaCompleta;
                try
                {
                    if (Testi.CapitoliInLibro(riferimento.Brani[0][0], versioneDaProvare) >= riferimento.Brani[0][1])
                        versioneDaUtilizzare = versioneDaProvare;
                }
                catch { } // se testo non esiste più, o è ""
            }
        }
        if (versioneDaUtilizzare == "" && Testi.NomiVersioni(TestoTipi.Bibbia).Count > 0)
            versioneDaUtilizzare = Testi.NomiVersioni(TestoTipi.Bibbia)[0];

        if (versioneDaUtilizzare != "")
            riferimento = Testi.ConvertiDaStandard(riferimento, versioneDaUtilizzare);

        return (versioneDaUtilizzare, riferimento);
    }
}
