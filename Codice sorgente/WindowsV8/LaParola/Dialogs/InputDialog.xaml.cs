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
using System.Windows.Shapes;

namespace LaParola.Dialogs
{
    /// <summary>
    /// Logica di interazione per InputDialog.xaml
    /// </summary>
    public partial class InputDialog : Window
    {
        public string InputText { get; private set; } = string.Empty;

        // Constructor accepting a custom prompt, an optional suggestion, and an optional window title
        public InputDialog(string prompt, string windowTitle, string suggestion = "")
        {
            InitializeComponent();

            Title = windowTitle;
            PromptTextBlock.Text = prompt;
            InputTextBox.Text = suggestion;

            InputTextBox.Focus();

            if (!string.IsNullOrEmpty(suggestion))
            {
                InputTextBox.SelectAll(); // Highlights text for easy overtyping
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            Services.ThemeManager.SetDarkTitleBar(this, Services.ThemeManager.IsDark(MainWindow.settings.ThemeMode));
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Intentionally bypass validation here to allow caller control
            InputText = InputTextBox.Text;
            DialogResult = true;
        }
    }
}
