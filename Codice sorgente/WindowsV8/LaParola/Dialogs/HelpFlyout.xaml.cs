using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LaParola
{
    /// <summary>
    /// Interaction logic for HelpFlyout.xaml
    /// </summary>
    public partial class HelpFlyout : UserControl
    {
        public HelpFlyout()
        {
            InitializeComponent();
        }

        // Command-like event (simpler than ICommand for now)
        public event RoutedEventHandler? HelpClicked;

        // Help text
        public static readonly DependencyProperty HelpTextProperty =
            DependencyProperty.Register(nameof(HelpText), typeof(string), typeof(HelpFlyout), new PropertyMetadata(""));

        public string HelpText
        {
            get => (string)GetValue(HelpTextProperty);
            set => SetValue(HelpTextProperty, value);
        }

        // Link text (customizable)
        public static readonly DependencyProperty HelpLinkTextProperty =
            DependencyProperty.Register(nameof(HelpLinkText), typeof(string), typeof(HelpFlyout), new PropertyMetadata((string)(System.Windows.Application.Current.TryFindResource("AiutoApri") ?? "Open Help Centre")));

        public string HelpLinkText
        {
            get => (string)GetValue(HelpLinkTextProperty);
            set => SetValue(HelpLinkTextProperty, value);
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            Popup.IsOpen = !Popup.IsOpen;
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            HelpClicked?.Invoke(this, new RoutedEventArgs());
        }

        public void Open()
        {
            Popup.IsOpen = true;
        }
    }
}
