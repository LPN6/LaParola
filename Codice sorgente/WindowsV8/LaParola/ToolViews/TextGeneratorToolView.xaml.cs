using LaParola.Utilities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace LaParola.ToolViews;

public partial class TextGeneratorToolView : UserControl
{
    // TODO2 aggiungere "Con definizioni": load and save state from settings
    //    then genitore.MostraDefinizioniInEditor(branoDaMostrare, clbVersioni.CheckedItems[0].ToString());
    //    and in SelectedItem change:             cbDefinizioni.Enabled = (clbVersioni.CheckedItems.Count > 0 && !string.IsNullOrEmpty(Funzioni.DizionarioDiVersione(clbVersioni.CheckedItems[0].ToString())));
    // TODO2 progress bar; there are better ways to do the threading
    // TODO2 testo può andare in una finestra editor attuale?
    // TODO2 alternare - riga addizionale da togliere

    private VersioneItem? _draggedItem;
    private Point _dragStartPoint;
    private bool _isMouseDown;
    private bool _isDragging;

    public TextGeneratorToolView()
    {
        InitializeComponent();
        DataContext = this;
        VersioneItem.SelectionChanged = SaveSelectedVersions;

        // Initial load on startup
        AggiornaVersioniDisponibili();

        cbAlternare.IsChecked = MainWindow.settings.MostraAlternare;
        MostraPulsanteStato();
    }

    // Public method accessible by the removal coordinator
    public void AggiornaVersioniDisponibili()
    {
        List<string> available = [.. MainWindow.Testi.NomiVersioni(TestoTipi.Bibbia | TestoTipi.Commentario)];
        HashSet<string> availableSet = [.. available];
        List<string> savedAll = MainWindow.settings.MostraVersioniTutte ?? [];
        HashSet<string> savedSet = [.. savedAll];
        List<string> savedSelected = MainWindow.settings.MostraVersioniSelezionate ?? [];

        // Keep only saved items that still exist
        List<string> validSaved = [.. savedAll.Where(v => availableSet.Contains(v))];

        // Add new items not yet in settings
        List<string> newItems = [.. available.Where(v => !savedSet.Contains(v))];

        // Final ordered list
        List<string> finalList = [.. validSaved, .. newItems];

        // Build UI items cleanly
        VersioneItems.Clear();

        foreach (string v in finalList)
        {
            VersioneItems.Add(new VersioneItem
            {
                Name = v,
                IsSelected = savedSelected.Contains(v)
            });
        }

        SaveAllVersions();
        SaveSelectedVersions();
    }

    public IReadOnlyList<string> GetSelectedVersionNames()
    {
        return [.. VersioneItems
                .Where(x => x.IsSelected)
                .Select(x => x.Name)];
    }

    public sealed class VersioneItem : INotifyPropertyChanged
    {
        private bool _isSelected;

        public string Name { get; init; } = "";

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value) return;
                _isSelected = value;
                OnPropertyChanged();

                SelectionChanged?.Invoke();
            }
        }

        public static Action? SelectionChanged;

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public ObservableCollection<VersioneItem> VersioneItems { get; } = [];

    /* private void VersionCheckBox_Click(object sender, RoutedEventArgs e)
     {
         // Qui la property IsSelected è già aggiornata (perché UpdateSourceTrigger=PropertyChanged)
         SaveSelectedVersions();
         MostraPulsanteStato();
     }*/

    private void ListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        // Se clicchi sul CheckBox (o dentro), NON iniziare un potenziale drag
        //if (IsClickOnCheckBox(e.OriginalSource as DependencyObject))
        //  return;

        ListBox listBox = (ListBox)sender;
        _dragStartPoint = e.GetPosition(listBox);
        _isMouseDown = true;
        _isDragging = false;

        if (ItemsControl.ContainerFromElement(listBox, e.OriginalSource as DependencyObject) is ListBoxItem item)
            _draggedItem = (VersioneItem)item.DataContext;
        else
            _draggedItem = null;
    }

    private void ListBox_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (!_isMouseDown || _isDragging || _draggedItem == null)
            return;

        if (e.LeftButton != MouseButtonState.Pressed)
            return;

        ListBox listBox = (ListBox)sender;
        Point currentPos = e.GetPosition(listBox);

        // soglia di drag di Windows (SM_CXDRAG/SM_CYDRAG)
        if (Math.Abs(currentPos.X - _dragStartPoint.X) < SystemParameters.MinimumHorizontalDragDistance &&
            Math.Abs(currentPos.Y - _dragStartPoint.Y) < SystemParameters.MinimumVerticalDragDistance)
            return;

        _isDragging = true;

        // Ora sì: avvia il drag
        DataObject data = new(typeof(VersioneItem), _draggedItem);
        DragDrop.DoDragDrop(listBox, data, DragDropEffects.Move);

        // cleanup
        _isDragging = false;
        _isMouseDown = false;
    }

    private void ListBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _isMouseDown = false;
        _isDragging = false;
        _draggedItem = null;
    }

    private void ListBox_DragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(typeof(VersioneItem)))
            e.Effects = DragDropEffects.Move;
        else
            e.Effects = DragDropEffects.None;

        e.Handled = true;
    }

    private void ListBox_Drop(object sender, DragEventArgs e)
    {
        ListBox listBox = (ListBox)sender;

        if (!e.Data.GetDataPresent(typeof(VersioneItem)))
            return;

        VersioneItem draggedItem = (VersioneItem)e.Data.GetData(typeof(VersioneItem))!;

        VersioneItem? target = GetItemAt(listBox, e.GetPosition(listBox));
        if (target == null) return;

        if (listBox.ItemsSource is not ObservableCollection<VersioneItem> collection)
            return;

        int oldIndex = collection.IndexOf(draggedItem);
        int newIndex = collection.IndexOf(target);

        if (oldIndex >= 0 && newIndex >= 0 && oldIndex != newIndex)
        {
            collection.Move(oldIndex, newIndex);
            SaveAllVersions();
        }

        e.Handled = true;
    }

    private static VersioneItem? GetItemAt(ListBox? listBox, Point position)
    {
        if (listBox == null) return null;

        DependencyObject? element = listBox.InputHitTest(position) as DependencyObject;

        while (element != null)
        {
            if (element is ListBoxItem item)
                return item.DataContext as VersioneItem;

            element = VisualTreeHelper.GetParent(element);
        }

        return null;
    }

    private void Brano_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
    {
        MostraPulsanteStato();
    }

    private void MostraPulsanteStato()
    {
        btnMostra.IsEnabled = !string.IsNullOrWhiteSpace(tbBrano.Text) && GetSelectedVersionNames().Count > 0;
        //rbAlternare.IsEnabled = GetSelectedVersionNames().Count > 1;
    }

    private void SaveAllVersions()
    {
        MainWindow.settings.MostraVersioniTutte =
        [
            .. VersioneItems
            .Select(x => x.Name)
        ];
        App.Settings.Save(MainWindow.settings);
    }

    private void SaveSelectedVersions()
    {
        MostraPulsanteStato();

        MainWindow.settings.MostraVersioniSelezionate =
        [
            .. VersioneItems
            .Where(x => x.IsSelected)
            .Select(x => x.Name)
        ];

        App.Settings.Save(MainWindow.settings);
    }

    private void HelpFlyout_OnHelpClicked(object sender, RoutedEventArgs e)
    {
        // TODO2: Open correct help section
        MessageBox.Show("Open Help Centre");
    }

    private void Seleziona_Click(object sender, RoutedEventArgs e)
    {
        ImpostaTutte(true);
    }

    private void Deseleziona_Click(object sender, RoutedEventArgs e)
    {
        ImpostaTutte(false);
    }

    private void ImpostaTutte(bool value)
    {
        foreach (VersioneItem item in VersioneItems)
            item.IsSelected = value;
        SaveSelectedVersions();
        MostraPulsanteStato();
    }

    private void Alternare_Click(object sender, RoutedEventArgs e)
    {
        MainWindow.settings.MostraAlternare = cbAlternare.IsChecked == true;
        App.Settings.Save(MainWindow.settings);
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        base.OnPreviewKeyDown(e);
        if (e.Key == Key.F1)
        {
            HelpFlyoutControl.Open();
            e.Handled = true;
        }
    }

    private async void Generate_Click(object sender, RoutedEventArgs e)
    {
        Collection<string> versioni = new([.. GetSelectedVersionNames()]);
        if (versioni.Count == 0)
        {
            return;
        }

        string abbVersioni = "";
        foreach (string versione in versioni)
        {
            VersioneInformazioni info = MainWindow.Testi.Info(versione);
            abbVersioni += info.Abbreviazione + ", ";
            //if ((info.Tipo & TestoTipi.Commentario) == TestoTipi.Commentario)
            //    almenoUnCommentario = true;
            //if ((info.Tipo & TestoTipi.Bibbia) == TestoTipi.Bibbia)
            //    almenoUnaBibbia = true;
        }
        if (!String.IsNullOrEmpty(abbVersioni))
            abbVersioni = " (" + abbVersioni[..^2] + ")";

        string title = string.IsNullOrWhiteSpace(tbBrano.Text) ? (string)(System.Windows.Application.Current.TryFindResource("EditorTitle") ?? "Editor Document") : MainWindow.Testi.NormalizzaRiferimento(tbBrano.Text) + abbVersioni;

        Riferimento rif = MainWindow.Testi.ConvertiRiferimento(tbBrano.Text);
        bool alternare = cbAlternare.IsChecked == true;
        FlowDocument doc = await MainWindow.Testi.FlowDocumentBranoAsync(rif, versioni, alternare:alternare);
        doc.Tag = versioni[0];
        Brush fg = (Brush)Application.Current.FindResource("AppForegroundBrush");
        RtfColorTransformer.ApplyThemeToDocument(doc, true, fg, true);

        App.DockingHost.SendFlowDocumentToActiveEditor(doc, title, versioni[0]);
    }
}