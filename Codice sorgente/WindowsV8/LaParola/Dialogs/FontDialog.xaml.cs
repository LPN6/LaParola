using LaParola.Utilities;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LaParola
{
    /// <summary>
    /// Interaction logic for FontDialog.xaml
    /// </summary>
    public partial class FontDialog : Window
    {
        public string SelectedFontFamily { get; private set; }
        public float SelectedFontSize { get; private set; }
        public bool SelectedBold { get; private set; }
        public bool SelectedItalic { get; private set; }
        public bool SelectedUnderline { get; private set; }
        public bool SelectedSuperscript { get; private set; }
        public string SelectedBrush { get; private set; }

        public static readonly Dictionary<string, string> Map = new()
        {
            ["#FF000000"] = "Black",
            ["#FFFFFFFF"] = "White",
            ["#FFFF0000"] = "Red",
            ["#FF0000FF"] = "Blue",
            ["#FF008000"] = "Green",
            ["#FFFFA500"] = "Orange",
            ["#FFFFD700"] = "Yellow",
            ["#FF800080"] = "Purple"
        };

        public FontDialog(bool allowSuperscript = false,
            string initialFont = "",
            float initialSize = 12 * 4 / 3,
            bool bold = false,
            bool italic = false,
            bool underline = false,
            bool superscript = false,
            string initialColor = "Black")
        {
            InitializeComponent();

            SelectedFontFamily = "";
            SelectedBrush = "";

            if (initialColor.Length == 9 && initialColor.StartsWith('#'))
            {
                initialColor = Map.TryGetValue(initialColor, out string? colorName) ? colorName : "Black";
            }

            LoadFonts();
            LoadSizes();
            LoadColours(MainWindow.settings.Language == "it");

            chkSuperscript.Visibility =
                allowSuperscript
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            // Initial values
            cmbFonts.SelectedItem = !string.IsNullOrEmpty(initialFont) && cmbFonts.Items.Contains(initialFont) ? initialFont : "Georgia";

            cmbSize.Text = initialSize.ToString(CultureInfo.InvariantCulture);

            chkBold.IsChecked = bold;
            chkItalic.IsChecked = italic;
            chkUnderline.IsChecked = underline;
            chkSuperscript.IsChecked = superscript;

            foreach (var fci in cmbColour.Items)
            {
                if (((FontColourItem)fci).NameEnglish == initialColor)
                {
                    cmbColour.SelectedItem = fci;
                    break;
                }
            }


            // Events
            cmbFonts.SelectionChanged += (_, __) => UpdatePreview();
            cmbSize.SelectionChanged += (_, __) => UpdatePreview();
            cmbSize.AddHandler(TextBox.TextChangedEvent, new TextChangedEventHandler((s, e) => UpdatePreview()));

            chkBold.Checked += (_, __) => UpdatePreview();
            chkBold.Unchecked += (_, __) => UpdatePreview();

            chkItalic.Checked += (_, __) => UpdatePreview();
            chkItalic.Unchecked += (_, __) => UpdatePreview();

            chkUnderline.Checked += (_, __) => UpdatePreview();
            chkUnderline.Unchecked += (_, __) => UpdatePreview();

            chkSuperscript.Checked += (_, __) => UpdatePreview();
            chkSuperscript.Unchecked += (_, __) => UpdatePreview();

            cmbColour.SelectionChanged += (_, __) => UpdatePreview();

            UpdatePreview();

        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            Services.ThemeManager.SetDarkTitleBar(this, Services.ThemeManager.IsDark(MainWindow.settings.ThemeMode));
        }

        private void LoadFonts()
        {
            var fonts = Fonts.SystemFontFamilies
                .Select(f => f.Source)
                .OrderBy(f => f);

            cmbFonts.ItemsSource = fonts;
        }

        private void LoadSizes()
        {
            cmbSize.ItemsSource = new double[]
            {
                8, 9, 10, 11, 12,
                14, 16, 18, 20,
                22, 24, 26, 28,
                36, 48, 72
            };
        }

        private void LoadColours(bool italian)
        {
            cmbColour.ItemsSource = new[]
            {
        new FontColourItem
        {
            Name = italian ? "Nero" : "Black",
            NameEnglish = "Black",
            Brush = Brushes.Black
        },
        new FontColourItem
        {
            Name = italian ? "Bianco" : "White",
            NameEnglish = "White",
            Brush = Brushes.White
        },
        new FontColourItem
        {
            Name = italian ? "Rosso" : "Red",
            NameEnglish = "Red",
            Brush = Brushes.Red
        },
        new FontColourItem
        {
            Name = italian ? "Blu" : "Blue",
            NameEnglish = "Blue",
            Brush = Brushes.Blue
        },
        new FontColourItem
        {
            Name = italian ? "Verde" : "Green",
            NameEnglish = "Green",
            Brush = Brushes.Green
        },
        new FontColourItem
        {
            Name = italian ? "Giallo" : "Yellow",
            NameEnglish = "Yellow",
            Brush = Brushes.Goldenrod
        },
        new FontColourItem
        {
            Name = italian ? "Arancione" : "Orange",
            NameEnglish = "Orange",
            Brush = Brushes.Orange
        },
        new FontColourItem
        {
            Name = italian ? "Viola" : "Purple",
            NameEnglish = "Purple",
            Brush = Brushes.Purple
        }
    };

            cmbColour.SelectedIndex = 0;
        }

        private void HelpFlyout_OnHelpClicked(object sender, RoutedEventArgs e)
        {
            // TODO2: Open correct help section
            MessageBox.Show("Open Help Centre");
        }

        private void UpdatePreview()
        {
            try
            {
                txtPreview.FontFamily = new FontFamily(cmbFonts.SelectedItem?.ToString() ?? "Georgia");

                if (double.TryParse(cmbSize.Text, out double size))
                {
                    txtPreview.FontSize = size * 4.0 / 3.0;
                }

                txtPreview.FontWeight =
                    chkBold.IsChecked == true
                        ? FontWeights.Bold
                        : FontWeights.Normal;

                txtPreview.FontStyle =
                    chkItalic.IsChecked == true
                        ? FontStyles.Italic
                        : FontStyles.Normal;

                txtPreview.TextDecorations =
                    chkUnderline.IsChecked == true
                        ? TextDecorations.Underline
                        : null;

                if (cmbColour.SelectedItem is FontColourItem colour)
                {
                    if (Services.ThemeManager.IsDark(MainWindow.settings.ThemeMode))
                    {
                        Brush fg = (Brush)Application.Current.FindResource("AppForegroundBrush");
                        txtPreview.Foreground = RtfColorTransformer.GetDarkThemedBrush(colour.Brush, fg);
                    }
                    else
                        txtPreview.Foreground = colour.Brush;
                }

                if (chkSuperscript.IsChecked == true)
                {
                    txtPreview.BaselineOffset = txtPreview.FontSize * 0.4;
                    txtPreview.FontSize *= 0.8;
                }
                else
                {
                    txtPreview.BaselineOffset = 0;
                }
            }
            catch
            {
                // Ignore invalid values
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            SelectedFontFamily = cmbFonts.SelectedItem?.ToString() ?? "Georgia";

            if (!float.TryParse(cmbSize.Text, out float size))
            {
                size = 12;
            }

            SelectedFontSize = size;
            SelectedBold = chkBold.IsChecked == true;
            SelectedItalic = chkItalic.IsChecked == true;
            SelectedUnderline = chkUnderline.IsChecked == true;
            SelectedSuperscript = chkSuperscript.IsChecked == true;
            SelectedBrush = (cmbColour.SelectedItem as FontColourItem)?.NameEnglish ?? "Black";

            DialogResult = true;
        }
    }

    public class FontColourItem
    {
        public string Name { get; set; }
        public string NameEnglish { get; set; }
        public Brush Brush { get; set; }

        public FontColourItem()
        {
            Name = string.Empty;
            NameEnglish = string.Empty;
            Brush = Brushes.Black;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
