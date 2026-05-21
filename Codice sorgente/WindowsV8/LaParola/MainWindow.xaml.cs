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
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace LaParola;

// TODO2 toolbar: new, open, save (all), print, undo, redo, find, cut, copy, paste, vis bibbia, commentario, apri note, segnalibri, navigare, ricerca, mostra, (chiave), (racc info), paralleli, LQ, Misure, Gesti testi, aggiorna, opzioni,aiuto
// TODO2 menu and toolbar: Application.Commands.Copy/Cut/Paste/Undo/Redo/SelectAll, NavigationCommands.Find, ...
// available icons are listed here: https://pictogrammers.com/library/mdi/
// TODO2 help centre: Search, FAQs, tutorials, contact, release notes, keyboard shortcuts, about, documentation, how to use, getting started
// TODO2 option to not ask confirm when closing documents

public partial class MainWindow : Window
{
    private TextGeneratorToolView? _textGenView;
    private ConverterToolView? _converterView;
    private OptionsToolView? _optionsView;

    private object TextGenToolViewInstance()
        => _textGenView ??= new TextGeneratorToolView();

    private object ConverterToolViewInstance()
        => _converterView ??= new ConverterToolView();

    private object OptionsToolViewInstance()
        => _optionsView ??= new OptionsToolView();

    internal static Texts Testi;
    internal static AppSettings settings;

    private IInputElement? _previousFocus;

    public MainWindow(AppSettings settingsLoaded)
    {
        settings = settingsLoaded;

        InitializeComponent();

        RestoreWindowPlacement();
        App.DockingHost.Initialize(Dock, DocumentPane);

        Services.ThemeManager.ApplyDockTheme(Dock, settings.ThemeMode);
        App.ThemeManager.HookSystemThemeChanges(settings.ThemeMode, Dock);

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
        App.DockingHost.ActiveEditorChanged += (_, _) =>
        {
            UpdateEditorMenuState();
        };

        try
        {
            await Task.Run(() =>
            {
                Testi = new Texts(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar);
                Testi.AggiungiDirectory(AppContext.BaseDirectory);
            });

            // Ora ripristina layout
            RestoreDockLayout();

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
            }
        }
        finally
        {
            ShowLoadingOverlay(false);
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
                FontFamily = new FontFamily("Giorgia"),
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

            App.DockingHost.OpenEditorDocument(doc);
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

    private void NewEditor_Click(object sender, RoutedEventArgs e) => App.DockingHost.OpenEditorDocument();
    private void NewViewer_Click(object sender, RoutedEventArgs e) => App.DockingHost.OpenViewerDocument();

    private void ApriEditor_Click(object sender, RoutedEventArgs e)
    {
        App.DockingHost.OpenEditorDocumentFromFile(this);
    }

    private void Chiudi_Click(object sender, RoutedEventArgs e)
    {   
        App.DockingHost.CloseActiveContent();
    }

    private void Salva_Click(object sender, RoutedEventArgs e)
    {
        App.DockingHost.SaveActiveEditor();
    }

    private void SalvaCome_Click(object sender, RoutedEventArgs e)
    {
        App.DockingHost.SaveActiveEditorAs();
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

    private void ShowTextGenerator_Click(object sender, RoutedEventArgs e)
    {
        //App.DockingHost.ShowTool("tool.textgen"); 

        // Assicurati che Testi sia pronto prima di creare la view
        if (Testi == null)
            return; // oppure mostra un messaggio / lascia l’overlay attivo

        App.DockingHost.ShowTool("tool.textgen");
    }
    private void ShowConverter_Click(object sender, RoutedEventArgs e) => App.DockingHost.ShowTool("tool.converter");
    private void ShowOptions_Click(object sender, RoutedEventArgs e) => App.DockingHost.ShowTool("tool.options");

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

    private void Exit_Click(object sender, RoutedEventArgs e) => Close();

}
