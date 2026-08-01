using AvalonDock;
using AvalonDock.Layout;
using LaParola.Dialogs;
using LaParola.DocumentViews;
using LaParola.ToolViews;
using Microsoft.Win32;
using System.Globalization;
using System.IO;
using System.Security.Principal;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

namespace LaParola.Services;

public class DockingHostService
{
    private DockingManager? _dock;
    private LayoutDocumentPane? _docPane;
    private EditorDocumentView? _lastActiveEditor;
    private bool _isShuttingDown;
    private Window? _ownerWindow;

    public bool HasActiveEditor =>
    GetActiveEditor() != null;
    public bool HasClosableContent =>
    GetActiveLayoutContent() != null;

    public void Initialize(DockingManager dock, LayoutDocumentPane docPane, Window owner)
    {
        _dock = dock;
        _docPane = docPane;
        _dock.ActiveContentChanged += Dock_ActiveContentChanged;
        _dock.DocumentClosing += Dock_DocumentClosing;
        _ownerWindow = owner;
    }

    public event EventHandler? ActiveEditorChanged;
    public event EventHandler? ActiveWindowChanged;

    public EditorDocumentView? GetActiveEditor()
    {
        return _lastActiveEditor;
    }

    private void Dock_ActiveContentChanged(object? sender, EventArgs e)
    {
        EditorDocumentView? previous = _lastActiveEditor;

        if (_dock?.Layout?.ActiveContent?.Content is EditorDocumentView editor)
        {
            _lastActiveEditor = editor;
        }

        // Fire event ONLY if remembered editor changed
        if (!ReferenceEquals(previous, _lastActiveEditor))
        {
            ActiveEditorChanged?.Invoke(this, EventArgs.Empty);
        }

        ActiveWindowChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Dock_DocumentClosing(object? sender, DocumentClosingEventArgs e)
    {
        if (_isShuttingDown)
            return;

        if (e.Document.Content is EditorDocumentView editor)
        {
            if (!editor.ConfirmClose())
            {
                e.Cancel = true;
            }
        }
    }

    public void BeginShutdown()
    {
        _isShuttingDown = true;
    }

    public void CancelShutdown()
    {
        _isShuttingDown = false;
    }

    public bool ConfirmCloseAll()
    {
        if (_dock?.Layout == null)
            return true;

        List<EditorDocumentView> editors = [.. _dock.Layout.Descendents()
            .OfType<LayoutDocument>()
            .Select(d => d.Content)
            .OfType<EditorDocumentView>()];

        foreach (EditorDocumentView editor in editors)
        {
            if (!editor.ConfirmClose())
            {
                return false;
            }
        }

        return true;
    }

    public void ShowTool(string contentId)
    {
        if (_dock?.Layout == null)
        {
            return;
        }

        // INTERCEPT OPTIONS, LIBRARY, RIFERIMENTI, AGGIUNGI_TESTI: Redirect to the wide LayoutDocumentPane (Visual Studio style)
        if (contentId == "tool.options" || contentId == "tool.library" || contentId == "tool.aggiungitesti" || contentId == "tool.riferimenti")
        {
            ShowToolAsDocument(contentId);
            return;
        }

        LayoutAnchorable? anchorable = _dock.Layout.Descendents().OfType<LayoutAnchorable>().FirstOrDefault(a => a.ContentId == contentId);
        if (anchorable == null)
        {
            LayoutAnchorablePane? pane = _dock.Layout.Descendents()
                            .OfType<LayoutAnchorablePane>()
                            .FirstOrDefault();

            if (pane == null)
            {
                LayoutPanel rootPanel = _dock.Layout.RootPanel;
                if (rootPanel == null) return;

                // 1. Look for an existing group. If pruned, recreate it.
                LayoutAnchorablePaneGroup? group = _dock.Layout.Descendents().OfType<LayoutAnchorablePaneGroup>().FirstOrDefault();
                if (group == null)
                {
                    group = new LayoutAnchorablePaneGroup { DockWidth = new System.Windows.GridLength(320) };

                    // Insert at index 0 to force it to dock on the far left of the main layout
                    rootPanel.Children.Insert(0, group);
                }

                // 2. Create the pane and add it to the group
                pane = new LayoutAnchorablePane();
                group.Children.Add(pane);
            }

            object? v = null;
            string titolo;
            if (contentId == "tool.search")
            {
                v = new SearchToolView();
                titolo = (string)(System.Windows.Application.Current.TryFindResource("RicercaTitolo") ?? "Search");
            }
            else if (contentId == "tool.textgen")
            {
                v = new TextGeneratorToolView();
                titolo = (string)(System.Windows.Application.Current.TryFindResource("MostraTitolo") ?? "Show Passage");
            }
            else if (contentId == "tool.converter")
            {
                v = new ConverterToolView();
                titolo = (string)(System.Windows.Application.Current.TryFindResource("MisureTitolo") ?? "Measures Converter"); ;
            }
            /* vecchio stile, nel panello a sinistra
            else if (contentId == "tool.library")
            {
                v = new LibraryToolView();
                titolo = (string)(System.Windows.Application.Current.TryFindResource("LibreriaTitolo") ?? "Library");
            }
            else if (contentId == "tool.options")
            {
                v = new OptionsToolView();
                titolo = (string)(System.Windows.Application.Current.TryFindResource("OpzioniTitolo") ?? "Options");
            } */
            else titolo = "";

            if (!string.IsNullOrEmpty(titolo))
            {
                LayoutAnchorable layoutDoc = new()
                {
                    Title = titolo,
                    ContentId = contentId,
                    CanClose = false,
                    CanHide = true,
                    Content = v
                };

                pane.Children.Add(layoutDoc);
                layoutDoc.IsVisible = true;
                layoutDoc.IsSelected = true;
                layoutDoc.IsActive = true;
            }
        }
        else
        {
            if (contentId == "tool.search")
                anchorable.Title = (string)(System.Windows.Application.Current.TryFindResource("RicercaTitolo") ?? "Search");
            else if (contentId == "tool.textgen")
                anchorable.Title = (string)(System.Windows.Application.Current.TryFindResource("MostraTitolo") ?? "Show Passage");
            else if (contentId == "tool.converter")
                anchorable.Title = (string)(System.Windows.Application.Current.TryFindResource("MisureTitolo") ?? "Measures Converter");
            else
                anchorable.Title = contentId; // non dovrebbe succedere
            anchorable.IsVisible = true;
            anchorable.IsSelected = true;
            anchorable.IsActive = true;
        }
    }

    private void ShowToolAsDocument(string contentId)
    {
        // Check if the wide Options document tab is already open
        LayoutDocument? existingDoc = _dock!.Layout.Descendents()
            .OfType<LayoutDocument>()
            .FirstOrDefault(d => d.ContentId == contentId);

        if (existingDoc != null)
        {
            existingDoc.Content ??= contentId switch
                {
                    "tool.options" => new OptionsToolView(),
                    "tool.library" => new LibraryToolView(),
                    "tool.riferimenti" => new ReferenceSearchToolView(),
                    _ => null
                };
            // Refresh title and focus
            existingDoc.Title = contentId switch
            {
                "tool.options" => (string)(System.Windows.Application.Current.TryFindResource("OpzioniTitolo") ?? "Options"),
                "tool.library" => (string)(System.Windows.Application.Current.TryFindResource("BibliotecaTitolo") ?? "Library"),
                "tool.aggiungitesti" => (string)(System.Windows.Application.Current.TryFindResource("AggiungiTestiTitolo") ?? "Add Texts"),
                "tool.riferimenti" => (string)(System.Windows.Application.Current.TryFindResource("RiferimentiTitolo") ?? "Reference Search"),
                _ => existingDoc.Title
            };
            existingDoc.IsSelected = true;
            existingDoc.IsActive = true;
            return;
        }

        // Locate the wide document pane workspace
        LayoutDocumentPane? docPane = _dock.Layout.Descendents()
            .OfType<LayoutDocumentPane>()
            .FirstOrDefault();

        if (docPane == null) return;

        LayoutDocument layoutDoc;
        if (contentId == "tool.options")
        {
            // Instantiate the options view and title
            OptionsToolView v = new();
            string titolo = (string)(System.Windows.Application.Current.TryFindResource("OpzioniTitolo") ?? "Options");

            // Wrap it inside a LayoutDocument instead of a LayoutAnchorable
            layoutDoc = new()
            {
                Title = titolo,
                ContentId = contentId, // "tool.options"
                Content = v
            };
        }
        else if (contentId == "tool.library")
        {
            LibraryToolView v = new();
            string titolo = (string)(System.Windows.Application.Current.TryFindResource("BibliotecaTitolo") ?? "Library");
            layoutDoc = new()
            {
                Title = titolo,
                ContentId = contentId,
                Content = v
            };
        }
        else if (contentId == "tool.riferimenti")
        {
            ReferenceSearchToolView v = new();
            string titolo = (string)(System.Windows.Application.Current.TryFindResource("RiferimentiTitolo") ?? "Reference Search");
            layoutDoc = new()
            {
                Title = titolo,
                ContentId = contentId,
                Content = v
            };
        }
        else if (contentId == "tool.aggiungitesti")
        {
            // Instantiate the view and title
            AggiungiTesti v = new();
            string titolo = (string)(System.Windows.Application.Current.TryFindResource("AggiungiTestiTitolo") ?? "Add Texts");
            // Wrap it inside a LayoutDocument instead of a LayoutAnchorable
            layoutDoc = new()
            {
                Title = titolo,
                ContentId = contentId, // "tool.aggiungitesti"
                Content = v
            };
        }
        else return; // non dovrebbe succedere

        // Append to the wide document center and activate
        docPane.Children.Add(layoutDoc);
        layoutDoc.IsSelected = true;
        layoutDoc.IsActive = true;
    }

    public EditorDocumentView? OpenEditorDocument(FlowDocument? doc = null, string titolo = "", string versione = "")
    {
        LayoutDocumentPane? docPane = _dock?.Layout?.Descendents()
            .OfType<LayoutDocumentPane>()
            .FirstOrDefault();

        if (docPane == null)
            return null;

        EditorDocumentView view = new()
        {
            Visibility = Visibility.Collapsed
        };

        LayoutDocument layoutDoc = new()
        {
            Title = string.IsNullOrEmpty(titolo) ?
                (string)(System.Windows.Application.Current.TryFindResource("EditorTitle") ?? "Editor Document")
                : titolo,
            ContentId = $"doc.editor.{Guid.NewGuid():N}",
            Content = view
        };

        view.Editor.IsUndoEnabled = false;
        if (doc != null)
        {
            view.SetDocument(doc);
        }

        view.ParentDocument = layoutDoc;
        docPane.Children.Add(layoutDoc);
        view.Visibility = Visibility.Visible;

        view.Editor.Versione = versione;
        view.Editor.IsUndoEnabled = true;
        Application.Current.Dispatcher.BeginInvoke(
          DispatcherPriority.Loaded,
        () =>
        {
            layoutDoc.IsSelected = true;
            layoutDoc.IsActive = true;
            view.FocusEditor();
        });

        return view;
    }

    public void OpenEditorDocumentFromFile(Window owner)
    {
        OpenFileDialog dlg = new()
        {
            Filter = (string)(Application.Current.TryFindResource("FileDialogoFiltroTutti") ?? "Rich Text (*.rtf)|*.rtf|Plain Text (*.txt)|*.txt|All files (*.*)|*.*")
        };

        if (dlg.ShowDialog(owner) != true)
            return;

        string? titolo = Path.GetFileName(dlg.FileName);
        EditorDocumentView? view = OpenEditorDocument(titolo: titolo);
        view?.LoadDocument(dlg.FileName);
    }

    public ViewerDocumentView? OpenViewerDocument(string title, byte libro = 1, byte capitolo = 1, byte versetto = 1)
    {
        ViewerDocumentView? view = OpenViewerDocumentCommon(title);

        if (view != null)
            _ = view.SpostaTesto(libro, capitolo, versetto, true, false);
        return view;
    }

    public ViewerDocumentView? OpenViewerDocument(string title, string notaTitolo = "")
    {
        ViewerDocumentView? view = OpenViewerDocumentCommon(title);

        if (view != null)
            _ = view.SpostaTesto(notaTitolo, true, false);
        return view;
    }

    private ViewerDocumentView? OpenViewerDocumentCommon(string title)
    {
        if (!MainWindow.Testi.VersioneEsiste(title))
            return null;

        LayoutDocumentPane? docPane = _dock?.Layout?.Descendents()
            .OfType<LayoutDocumentPane>()
            .FirstOrDefault();

        if (docPane == null || string.IsNullOrWhiteSpace(title))
            return null;

        ViewerDocumentView view = new(title);

        LayoutDocument layoutDoc = new()
        {
            Title = title,
            ContentId = $"doc.viewer.{Guid.NewGuid():N}",
            Content = view
        };
        docPane.Children.Add(layoutDoc);
        layoutDoc.IsSelected = true;
        layoutDoc.IsActive = true;

        return view;
    }

    public AggiungiTesti? OpenAggiungiTesti(string percorso)
    {
        LayoutDocumentPane? docPane = _dock?.Layout?.Descendents()
    .OfType<LayoutDocumentPane>()
    .FirstOrDefault();

        if (docPane == null)
            return null;

        AggiungiTesti view = new();

        LayoutDocument layoutDoc = new()
        {
            Title = Path.GetFileNameWithoutExtension(percorso),
            ContentId = $"doc.immagine.{Guid.NewGuid():N}",
            Content = view
        };
        docPane.Children.Add(layoutDoc);
        layoutDoc.IsSelected = true;
        layoutDoc.IsActive = true;

        return view;

    }

    public Immagine? OpenImmagineDocument(string percorso)
    {
        if (!File.Exists(percorso) || string.IsNullOrWhiteSpace(percorso))
            return null;

        LayoutDocumentPane? docPane = _dock?.Layout?.Descendents()
            .OfType<LayoutDocumentPane>()
            .FirstOrDefault();

        if (docPane == null)
            return null;

        Immagine view = new(percorso);

        view.LinkClicked += async (linkNome, collezione) =>
        {
            // Tutta la logica di business e i messaggi a schermo rimangono qui nel container
            if (MainWindow.Testi.VersioneEsiste(collezione))
            {
                await MainWindow.ApriNotaInEditor(linkNome, collezione);
            }
            else
            {
                string messaggio = (string)(Application.Current.TryFindResource("ImmaginiCollezioneNonTrovata") ?? "Collection not found");
                messaggio = string.Format(CultureInfo.InvariantCulture, messaggio, collezione);
                string titolo = (string)(Application.Current.TryFindResource("Errore") ?? "Error");
                if (_ownerWindow != null)
                    MessageBoxLPN.Show(_ownerWindow, messaggio, titolo);
            }
        };

        LayoutDocument layoutDoc = new()
        {
            Title = Path.GetFileNameWithoutExtension(percorso),
            ContentId = $"doc.immagine.{Guid.NewGuid():N}",
            Content = view
        };
        docPane.Children.Add(layoutDoc);
        layoutDoc.IsSelected = true;
        layoutDoc.IsActive = true;

        return view;
    }

    public LayoutContent? GetActiveLayoutContent()
    {
        return _dock?.Layout?.ActiveContent;
    }

    public void CloseActiveContent()
    {
        LayoutContent? content = GetActiveLayoutContent();

        if (content == null)
            return;

        // Tool window
        if (content is LayoutAnchorable anchorable)
        {
            if (anchorable.CanClose)
            {
                anchorable.Close();
            }
            else
            {
                anchorable.Hide();
            }
            return;
        }

        // Document window
        if (content is LayoutDocument document)
        {
            if (document.Content is EditorDocumentView editor)
            {
                if (!editor.ConfirmClose())
                    return;
            }
            if (document.Content == _lastActiveEditor)
            {
                _lastActiveEditor = null;
                ActiveEditorChanged?.Invoke(this, EventArgs.Empty);
            }

            document.Close();
            return;
        }
    }

    public void CloseActiveEditor()
    {
        EditorDocumentView? editor = GetActiveEditor();
        if (editor?.ParentDocument != null)
        {
            editor.ParentDocument.Close();
        }
    }

    public void SaveActiveEditor()
    {
        GetActiveEditor()?.SaveDocument();
    }

    public void SaveActiveEditorAs()
    {
        GetActiveEditor()?.SaveDocumentAs();
    }

    public void SendFlowDocumentToActiveEditor(FlowDocument doc, string titolo, string versione = "")
    {
        OpenEditorDocument(doc, titolo, versione);
    }
}
