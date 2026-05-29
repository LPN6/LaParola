using AvalonDock;
using AvalonDock.Layout;
using LaParola.DocumentViews;
using LaParola.ToolViews;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;

namespace LaParola.Services;

public class DockingHostService
{
    private DockingManager? _dock;
    private LayoutDocumentPane? _docPane;
    private EditorDocumentView? _lastActiveEditor;
    private bool _isShuttingDown;
    public bool HasActiveEditor =>
    GetActiveEditor() != null;
    public bool HasClosableContent =>
    GetActiveLayoutContent() != null;

    public void Initialize(DockingManager dock, LayoutDocumentPane docPane)
    {
        _dock = dock;
        _docPane = docPane;
        _dock.ActiveContentChanged += Dock_ActiveContentChanged;
        _dock.DocumentClosing += Dock_DocumentClosing;
    }

    public event EventHandler? ActiveEditorChanged;

    public EditorDocumentView? GetActiveEditor()
    {
        return _lastActiveEditor;
    }

    private void Dock_ActiveContentChanged(object? sender, EventArgs e)
    {
        EditorDocumentView? previous = _lastActiveEditor;

        if (_dock?.Layout?.ActiveContent?.Content
            is EditorDocumentView editor)
        {
            _lastActiveEditor = editor;
        }

        // Fire event ONLY if remembered editor changed
        if (!ReferenceEquals(previous, _lastActiveEditor))
        {
            ActiveEditorChanged?.Invoke(this, EventArgs.Empty);
        }
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

        LayoutAnchorable? anchorable = _dock.Layout.Descendents().OfType<LayoutAnchorable>().FirstOrDefault(a => a.ContentId == contentId);
        if (anchorable == null)
        {
            LayoutAnchorablePane? pane = _dock.Layout.Descendents()
                            .OfType<LayoutAnchorablePane>()
                            .FirstOrDefault();

            if (pane == null) return;

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
            else if (contentId == "tool.options")
            {
                v = new OptionsToolView();
                titolo = (string)(System.Windows.Application.Current.TryFindResource("OpzioniTitolo") ?? "Options");
            }
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
            else if (contentId == "tool.options")
                anchorable.Title = (string)(System.Windows.Application.Current.TryFindResource("OpzioniTitolo") ?? "Options");
            else
                anchorable.Title = contentId; // non dovrebbe succedere
            anchorable.IsVisible = true;
            anchorable.IsSelected = true;
            anchorable.IsActive = true;
        }
    }

    public EditorDocumentView? OpenEditorDocument(FlowDocument? doc = null, string titolo = "")
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

    public void OpenViewerDocument(FlowDocument? doc = null)
    {
        LayoutDocumentPane? docPane = _dock?.Layout?.Descendents()
    .OfType<LayoutDocumentPane>()
    .FirstOrDefault();

        if (docPane == null)
            return;

        ViewerDocumentView view = new();
        if (doc != null)
        {
            view.SetDocument(doc);
        }

        LayoutDocument layoutDoc = new()
        {
            Title = (string)(System.Windows.Application.Current.TryFindResource("ViewerTitle") ?? "Viewer Document"),
            ContentId = $"doc.viewer.{Guid.NewGuid():N}",
            Content = view
        };
        docPane.Children.Add(layoutDoc);
        layoutDoc.IsSelected = true;
        layoutDoc.IsActive = true;
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

    public void SendFlowDocumentToActive(FlowDocument doc, bool preferEditor, string titolo)
    {
        if (_dock?.ActiveContent is IFlowDocumentHost host)
        {
            host.SetDocument(doc);
            return;
        }
        if (preferEditor)
        {
            OpenEditorDocument(doc, titolo);
        }
        else
        {
            OpenViewerDocument(doc);
        }
    }
}
