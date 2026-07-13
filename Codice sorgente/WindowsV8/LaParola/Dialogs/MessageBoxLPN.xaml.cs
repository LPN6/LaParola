using System.Windows;

namespace LaParola
{
    /// <summary>
    /// Interaction logic for MessageBoxLPN.xaml
    /// </summary>
    public partial class MessageBoxLPN : Window
    {
        public string Message { get; set; }
        public MessageBoxResult Result { get; private set; }

        public MessageBoxLPN(Window owner, string message, MessageBoxButton buttons)
        {
            InitializeComponent();
            Owner = owner;
            Message = message;
            DataContext = this;
            ConfigureButtons(buttons);
            CancelButtonEx.IsCancel = true;
        }

        private void ConfigureButtons(MessageBoxButton buttons)
        {
            switch (buttons)
            {
                case MessageBoxButton.OK:
                    OkButton.Visibility = Visibility.Visible;
                    OkButton.Content =
                        Application.Current.TryFindResource("PulsanteOK") ?? "OK";
                    break;

                case MessageBoxButton.YesNo:
                    YesButton.Visibility = Visibility.Visible;
                    NoButton.Visibility = Visibility.Visible;
                    YesButton.Content =
                        Application.Current.TryFindResource("PulsanteSi") ?? "Yes";
                    NoButton.Content =
                        Application.Current.TryFindResource("PulsanteNo") ?? "No";
                    break;

                case MessageBoxButton.YesNoCancel:
                    YesButton.Visibility = Visibility.Visible;
                    NoButton.Visibility = Visibility.Visible;
                    CancelButtonEx.Visibility = Visibility.Visible;
                    YesButton.Content =
                        Application.Current.TryFindResource("PulsanteSi") ?? "Yes";
                    NoButton.Content =
                        Application.Current.TryFindResource("PulsanteNo") ?? "No";
                    CancelButtonEx.Content =
                        Application.Current.TryFindResource("PulsanteAnnulla") ?? "Cancel";
                    CancelButtonEx.IsCancel = true;
                    break;
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            Services.ThemeManager.SetDarkTitleBar(this, Services.ThemeManager.IsDark(MainWindow.settings.ThemeMode));
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.OK;
            DialogResult = true;
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;
            DialogResult = true;
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;
            DialogResult = false;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Cancel;
            DialogResult = false;
        }

        public static MessageBoxResult Show(Window owner, string message, string title, MessageBoxButton buttons = MessageBoxButton.OK)
        {
            MessageBoxLPN dlg = new(owner, message, buttons)
            {
                Title = title
            };

            dlg.ShowDialog();
            return dlg.Result;
        }
    }
}