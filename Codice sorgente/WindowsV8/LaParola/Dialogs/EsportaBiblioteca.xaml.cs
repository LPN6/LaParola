using LaParola.Services;
using System.Windows;
using System.Windows.Controls;

namespace LaParola.Dialogs
{
    /// <summary>
    /// Interaction logic for EsportaBiblioteca.xaml
    /// </summary>
    public partial class EsportaBiblioteca : Window
    {
        public string Informazioni
        {
            get => string.Join("|", ItemsListBox.Items
                .OfType<CheckBox>()
                .Select((cb, index) => (cb, index))
                .Where(x => x.cb.IsChecked == true)
                .Select(x => x.index));
            set => ImpostaCheckBox(value);
        }

        private void ImpostaCheckBox(string input)
        {
            if (input == "-") input = "0|1|2|3|4|5|6|7|8|9|10|11";
            HashSet<int> selectedIndices = input?
                .Split(['|'], StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s, out int idx) ? (int?)idx : null)
                .OfType<int>()
                .ToHashSet() ?? [];

            int index = 0;
            foreach (CheckBox cb in ItemsListBox.Items.OfType<CheckBox>())
            {
                cb.IsChecked = selectedIndices.Contains(index);
                index++;
            }
        }

        public int Ordine
        {
            get => CbOrdine.SelectedIndex;
            set => CbOrdine.SelectedIndex = value;
        }

        public int Formato
        {
            get => CbFormato.SelectedIndex;
            set => CbFormato.SelectedIndex = value;
        }

        public EsportaBiblioteca()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            ThemeManager.SetDarkTitleBar(this, ThemeManager.IsDark(MainWindow.settings.ThemeMode));
        }

        private void HelpFlyout_OnHelpClicked(object sender, RoutedEventArgs e)
        {
            MainWindow.MostraGuida((string)(Application.Current.TryFindResource("ImportaDialogoAiutoPagine") ?? "Bibliographical Information"));
        }

        // Select All handler
        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            SetAllCheckState(true);
        }

        // Deselect All handler
        private void DeselectAllButton_Click(object sender, RoutedEventArgs e)
        {
            SetAllCheckState(false);
        }

        private void SetAllCheckState(bool isChecked)
        {
            foreach (object? item in ItemsListBox.Items)
            {
                if (item is CheckBox checkBox)
                {
                    checkBox.IsChecked = isChecked;
                }
            }
        }

        // OK Button click handler
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
