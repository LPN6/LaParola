using AvalonDock.Layout;
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
    private bool _isDirty;
    private bool _suppressTextChanged;

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

    private void HelpFlyout_OnHelpClicked(object sender, RoutedEventArgs e)
    {
        // TODO2: Open correct help section
        MessageBox.Show("Open Help Centre");
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

    public EditorDocumentView()
    {
        InitializeComponent();
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
    }

    private void Editor_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_suppressTextChanged)
            return;

        IsDirty = true;
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
            if (ParentDocument != null)
            {
                ParentDocument.Title = Path.GetFileName(_currentFile);
            }
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
            if (ParentDocument != null)
            {
                ParentDocument.Title = Path.GetFileName(path);
            }
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
        if (!IsDirty)
            return true;

        string message =
            (string)(Application.Current.TryFindResource("EditorSalvaModifiche") ?? "Do you want to save changes?");

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

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
