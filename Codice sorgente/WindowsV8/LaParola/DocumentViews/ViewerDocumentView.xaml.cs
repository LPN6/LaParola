using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace LaParola.DocumentViews;

public partial class ViewerDocumentView : UserControl, IFlowDocumentHost, INotifyPropertyChanged
{
    private string? _currentFile;

    public string CurrentFileDisplay => string.IsNullOrWhiteSpace(_currentFile)
        ? (string)(Application.Current.TryFindResource("FileNone") ?? "(none)")
        : _currentFile!;

    public ViewerDocumentView()
    {
        InitializeComponent();
        DataContext = this;
        Viewer.Document = new FlowDocument
        {
            FontFamily = new System.Windows.Media.FontFamily("Segoe UI"),
            FontSize = 14,
            PageWidth = double.NaN,
            ColumnWidth = double.PositiveInfinity,
            PagePadding = new Thickness(20)
        };
    }

    public void SetDocument(FlowDocument doc)
    {
        Viewer.Document = doc;
        _currentFile = null;
        OnPropertyChanged(nameof(CurrentFileDisplay));
    }

    private void Open_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dlg = new() {
            Filter = (string)(Application.Current.TryFindResource("FileDialogoFiltroTutti") ?? "Rich Text (*.rtf)|*.rtf|Plain Text (*.txt)|*.txt|All files (*.*)|*.*")
        };
        if (dlg.ShowDialog(Window.GetWindow(this)) == true)
        {
            LoadFromFile(dlg.FileName);
        }
    }

    private void LoadFromFile(string path)
    {
        try
        {
            TextRange range = new(Viewer.Document.ContentStart, Viewer.Document.ContentEnd);
            using FileStream fs = new(path, FileMode.Open, FileAccess.Read);
            if (Path.GetExtension(path).Equals(".xaml", System.StringComparison.OrdinalIgnoreCase))
            {
                range.Load(fs, DataFormats.XamlPackage);
            }
            else if (Path.GetExtension(path).Equals(".rtf", System.StringComparison.OrdinalIgnoreCase))
            {
                range.Load(fs, DataFormats.Rtf);
            }
            else
            {
                range.Load(fs, DataFormats.Text);
                foreach (Block block in Viewer.Document.Blocks)
                {
                    if (block is Paragraph p)
                    {
                        p.Margin = new Thickness(0);
                    }
                }
            }

            _currentFile = path;
            OnPropertyChanged(nameof(CurrentFileDisplay));
        }
        catch (System.Exception ex)
        {
            MessageBoxLPN.Show(Window.GetWindow(this), ex.Message, (string)(Application.Current.TryFindResource("EditorApriFallito") ?? "Open failed"));
        }
    }

    public void LoadPlaceholder(string displayName, string verseRef)
    {
        // TODO2: sostituisci con logica reale quando avrai identificatori e fonte contenuto
        Viewer.Document.Blocks.Clear();
        Viewer.Document.Blocks.Add(new Paragraph(new Run($"[Viewer restored]\n{displayName}\n{verseRef}")));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
