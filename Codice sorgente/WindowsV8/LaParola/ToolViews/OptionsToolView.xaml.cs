using LaParola.Models;
using LaParola.Utilities;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LaParola.ToolViews;

public partial class OptionsToolView : UserControl
{
    readonly bool nonSalvare = true;

    // TODO2 - reimposta default settings, help on export/import settings, setting to turn off tool tips

    public OptionsToolView()
    {
        InitializeComponent();

        ApplicaFontAdEsempio(FontPredefinitoEsempio, "Font");
        ApplicaFontAdEsempio(FontGrecoEsempio, "FontGreco");
        ApplicaFontAdEsempio(FontEbraicoEsempio, "FontEbraico");
        ApplicaFontAdEsempio(FontRicercaEsempio, "FontRicerca");
        ApplicaFontAdEsempio(FontRiferimentoEsempio, "FontRiferimento");

        ThemeSystem.IsChecked = MainWindow.settings.ThemeMode == ThemeState.System;
        ThemeLight.IsChecked = MainWindow.settings.ThemeMode == ThemeState.Light;
        ThemeDark.IsChecked = MainWindow.settings.ThemeMode == ThemeState.Dark;

        foreach (FrameworkElement item in LanguageCombo.Items.OfType<FrameworkElement>())
        {
            if (item.Tag?.ToString() == MainWindow.settings?.Language)
            {
                LanguageCombo.SelectedItem = item;
                break;
            }
        }
        if (LanguageCombo.SelectedIndex < 0)
        {
            LanguageCombo.SelectedIndex = 0;
        }

        List<string> fonts = [.. Fonts.SystemFontFamilies
            .Select(f => f.Source)
            .OrderBy(f => f)];

        nonSalvare = false;
    }

    private static void ApplicaFontAdEsempio(TextBlock tbEsempio, string categoria)
    {
        PropertyInfo? propNome = MainWindow.settings.Formato.GetType().GetProperty(categoria + "Nome");
        PropertyInfo? propDim = MainWindow.settings.Formato.GetType().GetProperty(categoria + "Dimensione");
        PropertyInfo? propGrassetto = MainWindow.settings.Formato.GetType().GetProperty(categoria + "Grassetto");
        PropertyInfo? propCorsivo = MainWindow.settings.Formato.GetType().GetProperty(categoria + "Corsivo");
        PropertyInfo? propSottolineato = MainWindow.settings.Formato.GetType().GetProperty(categoria + "Sottolineato");
        PropertyInfo? propColore = MainWindow.settings.Formato.GetType().GetProperty(categoria + "Colore");
        if (propNome != null && propDim != null && propGrassetto != null && propCorsivo != null && propSottolineato != null && propColore != null)
        {
            string? nome = propNome.GetValue(MainWindow.settings.Formato) as string;
            float? dim = (float?)propDim.GetValue(MainWindow.settings.Formato);
            bool? grassetto = (bool?)propGrassetto.GetValue(MainWindow.settings.Formato);
            bool? corsivo = (bool?)propCorsivo.GetValue(MainWindow.settings.Formato);
            bool? sottolineato = (bool?)propSottolineato.GetValue(MainWindow.settings.Formato);
            System.Windows.Media.Color? colore = (System.Windows.Media.Color?)propColore.GetValue(MainWindow.settings.Formato);
            tbEsempio.Text = nome + " " + dim;
            tbEsempio.FontFamily = new FontFamily(nome);
            try
            {
                tbEsempio.FontSize = dim != null ? (double)dim : 12;
            }
            catch
            {
                tbEsempio.FontSize = 12;
            }
            tbEsempio.FontWeight = grassetto != null ? (bool)grassetto ? FontWeights.Bold : FontWeights.Normal : FontWeights.Normal;
            tbEsempio.FontStyle = corsivo != null ? (bool)corsivo ? FontStyles.Italic : FontStyles.Normal : FontStyles.Normal;
            tbEsempio.TextDecorations = sottolineato != null ? (bool)sottolineato ? TextDecorations.Underline : null : null;

            UpdateFontColorTextBlock(tbEsempio,colore, Services.ThemeManager.IsDark(MainWindow.settings.ThemeMode));
        }
    }

    internal void UpdateFontColor(bool isDarkTheme)
    {
        UpdateFontColorTextBlock(FontPredefinitoEsempio, MainWindow.settings.Formato.FontColore, isDarkTheme);
        UpdateFontColorTextBlock(FontGrecoEsempio, MainWindow.settings.Formato.FontGrecoColore, isDarkTheme);
        UpdateFontColorTextBlock(FontEbraicoEsempio, MainWindow.settings.Formato.FontEbraicoColore, isDarkTheme);
        UpdateFontColorTextBlock(FontRicercaEsempio, MainWindow.settings.Formato.FontRicercaColore, isDarkTheme);
        UpdateFontColorTextBlock(FontRiferimentoEsempio, MainWindow.settings.Formato.FontRiferimentoColore, isDarkTheme);
    }

    private static void UpdateFontColorTextBlock(TextBlock tbEsempio, Color? colore,bool isDarkTheme)
    {
        if (isDarkTheme)
        {
            Brush fg = (Brush)Application.Current.FindResource("AppForegroundBrush");
            tbEsempio.Foreground = RtfColorTransformer.GetDarkThemedBrush(colore != null ? new SolidColorBrush((System.Windows.Media.Color)colore) : Brushes.Black, fg);
        }
        else
            tbEsempio.Foreground = colore != null ? new SolidColorBrush((System.Windows.Media.Color)colore) : Brushes.Black;
    }

    private void HelpFlyout_OnHelpClicked(object sender, RoutedEventArgs e)
    {
        // TODO2: Open correct help section
        MessageBox.Show("Open Help Centre");
    }

    private void SelectFont_Click(object sender, RoutedEventArgs e)
    {
        FormatoTesto f = MainWindow.Testi.Formato;
        FontDialog dlg;
        object conv;
        System.Windows.Media.Color chosenColor;
        switch (((Button)sender).Name)
        {
            case "Modifica":
                dlg = new FontDialog(allowSuperscript: false, f.FontNome, f.FontDimensione, f.FontGrassetto, f.FontCorsivo, f.FontSottolineato, false, f.FontColore.ToString());

                if (dlg.ShowDialog() == true)
                {
                    f.FontNome = MainWindow.settings.Formato.FontNome = dlg.SelectedFontFamily;
                    f.FontDimensione = MainWindow.settings.Formato.FontDimensione = dlg.SelectedFontSize;
                    f.FontGrassetto = MainWindow.settings.Formato.FontGrassetto = dlg.SelectedBold;
                    f.FontCorsivo = MainWindow.settings.Formato.FontCorsivo = dlg.SelectedItalic;
                    f.FontSottolineato = MainWindow.settings.Formato.FontSottolineato = dlg.SelectedUnderline;

                    // Safely convert brush string to Color to avoid CS8629
                    conv = ColorConverter.ConvertFromString(dlg.SelectedBrush);
                    chosenColor = conv is Color c1 ? c1 : f.FontColore; // fallback to current color if parse fails
                    f.FontColore = MainWindow.settings.Formato.FontColore = chosenColor;

                    ApplicaFontAdEsempio(FontPredefinitoEsempio, "Font");
                }
                break;
            case "ModificaGreco":
                dlg = new FontDialog(allowSuperscript: false, f.FontGrecoNome, f.FontGrecoDimensione, f.FontGrecoGrassetto, f.FontGrecoCorsivo, f.FontGrecoSottolineato, false, f.FontGrecoColore.ToString());

                if (dlg.ShowDialog() == true)
                {
                    f.FontGrecoNome = MainWindow.settings.Formato.FontGrecoNome = dlg.SelectedFontFamily;
                    f.FontGrecoDimensione = MainWindow.settings.Formato.FontGrecoDimensione = dlg.SelectedFontSize;
                    f.FontGrecoGrassetto = MainWindow.settings.Formato.FontGrecoGrassetto = dlg.SelectedBold;
                    f.FontGrecoCorsivo = MainWindow.settings.Formato.FontGrecoCorsivo = dlg.SelectedItalic;
                    f.FontGrecoSottolineato = MainWindow.settings.Formato.FontGrecoSottolineato = dlg.SelectedUnderline;

                    // Safely convert brush string to Color to avoid CS8629
                    conv = ColorConverter.ConvertFromString(dlg.SelectedBrush);
                    chosenColor = conv is Color c2 ? c2 : f.FontGrecoColore; // fallback to current color if parse fails
                    f.FontGrecoColore = MainWindow.settings.Formato.FontGrecoColore = chosenColor;

                    ApplicaFontAdEsempio(FontGrecoEsempio, "FontGreco");
                }
                break;
            case "ModificaEbraico":
                dlg = new FontDialog(allowSuperscript: false, f.FontEbraicoNome, f.FontEbraicoDimensione, f.FontEbraicoGrassetto, f.FontEbraicoCorsivo, f.FontEbraicoSottolineato, false, f.FontEbraicoColore.ToString());

                if (dlg.ShowDialog() == true)
                {
                    f.FontEbraicoNome = MainWindow.settings.Formato.FontEbraicoNome = dlg.SelectedFontFamily;
                    f.FontEbraicoDimensione = MainWindow.settings.Formato.FontEbraicoDimensione = dlg.SelectedFontSize;
                    f.FontEbraicoGrassetto = MainWindow.settings.Formato.FontEbraicoGrassetto = dlg.SelectedBold;
                    f.FontEbraicoCorsivo = MainWindow.settings.Formato.FontEbraicoCorsivo = dlg.SelectedItalic;
                    f.FontEbraicoSottolineato = MainWindow.settings.Formato.FontEbraicoSottolineato = dlg.SelectedUnderline;

                    // Safely convert brush string to Color to avoid CS8629
                    conv = ColorConverter.ConvertFromString(dlg.SelectedBrush);
                    chosenColor = conv is Color c3 ? c3 : f.FontEbraicoColore; // fallback to current color if parse fails
                    f.FontEbraicoColore = MainWindow.settings.Formato.FontEbraicoColore = chosenColor;

                    ApplicaFontAdEsempio(FontEbraicoEsempio, "FontEbraico");
                }
                break;
            case "ModificaRicerca":
                dlg = new FontDialog(allowSuperscript: false, f.FontRicercaNome, f.FontRicercaDimensione, f.FontRicercaGrassetto, f.FontRicercaCorsivo, f.FontRicercaSottolineato, false, f.FontRicercaColore.ToString());

                if (dlg.ShowDialog() == true)
                {
                    f.FontRicercaNome = MainWindow.settings.Formato.FontRicercaNome = dlg.SelectedFontFamily;
                    f.FontRicercaDimensione = MainWindow.settings.Formato.FontRicercaDimensione = dlg.SelectedFontSize;
                    f.FontRicercaGrassetto = MainWindow.settings.Formato.FontRicercaGrassetto = dlg.SelectedBold;
                    f.FontRicercaCorsivo = MainWindow.settings.Formato.FontRicercaCorsivo = dlg.SelectedItalic;
                    f.FontRicercaSottolineato = MainWindow.settings.Formato.FontRicercaSottolineato = dlg.SelectedUnderline;

                    // Safely convert brush string to Color to avoid CS8629
                    conv = ColorConverter.ConvertFromString(dlg.SelectedBrush);
                    chosenColor = conv is Color c3 ? c3 : f.FontRicercaColore; // fallback to current color if parse fails
                    f.FontRicercaColore = MainWindow.settings.Formato.FontRicercaColore = chosenColor;

                    ApplicaFontAdEsempio(FontRicercaEsempio, "FontRicerca");
                }
                break;
            case "ModificaRiferimenti":
                dlg = new FontDialog(allowSuperscript: true, f.FontRiferimentoNome, f.FontRiferimentoDimensione, f.FontRiferimentoGrassetto, f.FontRiferimentoCorsivo, f.FontRiferimentoSottolineato, f.RiferimentoApice, f.FontRiferimentoColore.ToString());

                if (dlg.ShowDialog() == true)
                {
                    f.FontRiferimentoNome = MainWindow.settings.Formato.FontRiferimentoNome = dlg.SelectedFontFamily;
                    f.FontRiferimentoDimensione = MainWindow.settings.Formato.FontRiferimentoDimensione = dlg.SelectedFontSize;
                    f.FontRiferimentoGrassetto = MainWindow.settings.Formato.FontRiferimentoGrassetto = dlg.SelectedBold;
                    f.FontRiferimentoCorsivo = MainWindow.settings.Formato.FontRiferimentoCorsivo = dlg.SelectedItalic;
                    f.FontRiferimentoSottolineato = MainWindow.settings.Formato.FontRiferimentoSottolineato = dlg.SelectedUnderline;
                    f.RiferimentoApice = MainWindow.settings.Formato.RiferimentoApice = dlg.SelectedSuperscript;

                    // Safely convert brush string to Color to avoid CS8629
                    conv = ColorConverter.ConvertFromString(dlg.SelectedBrush);
                    chosenColor = conv is Color c3 ? c3 : f.FontRiferimentoColore; // fallback to current color if parse fails
                    f.FontRiferimentoColore = MainWindow.settings.Formato.FontRiferimentoColore = chosenColor;

                    ApplicaFontAdEsempio(FontRiferimentoEsempio, "FontRiferimento");
                }
                break;
            default:
                break;
        }
        App.Settings.Save(MainWindow.settings);
    }

    private void SettingChanged(object sender, RoutedEventArgs e)
    {
        if (nonSalvare) return;

        AppSettings _settings = MainWindow.settings;
        bool cambiaTheme = false;
        bool cambiaLanguage = false;

        ThemeState newTheme = ThemeDark.IsChecked == true ? ThemeState.Dark :
                                  ThemeLight.IsChecked == true ? ThemeState.Light : ThemeState.System;
        if (newTheme != _settings.ThemeMode)
        {
            _settings.ThemeMode = newTheme;
            Services.ThemeManager.ApplyTheme(_settings.ThemeMode);
            cambiaTheme = true;
        }

        if (LanguageCombo.SelectedItem is FrameworkElement fe && fe.Tag is string lang)
        {
            if (lang != _settings.Language)
            {
                _settings.Language = lang;
                Services.LocalizationManager.ApplyLanguage(_settings.Language);
                cambiaLanguage = true;

                string libriNomi, libriAbbUsate, libriAbbRic;
                if (_settings.Language == "it")
                {
                    libriNomi = Texts.LibriNomiItaliano;
                    libriAbbUsate = Texts.LibriAbbreviazioniUsateItaliano;
                    libriAbbRic = Texts.LibriAbbreviazioniRiconosciuteItaliano;
                }
                else
                {
                    // default inglese, o in caso di lingua non riconosciuta
                    libriNomi = Texts.LibriNomiInglese;
                    libriAbbUsate = Texts.LibriAbbreviazioniUsateInglese;
                    libriAbbRic = Texts.LibriAbbreviazioniRiconosciuteInglese;
                }
                MainWindow.Testi.libriNomi = libriNomi.Split('|');
                MainWindow.Testi.libriAbbreviazioniUsate = libriAbbUsate.Split('|');
                string[] libriAbbRicArray = libriAbbRic.Split('|', StringSplitOptions.RemoveEmptyEntries);
                string[] abbreviazioniDiLibro;
                MainWindow.Testi.LibriAbbreviazioniRiconosciute.Clear();
                for (byte i = 1; i <= 73; ++i)
                {
                    abbreviazioniDiLibro = libriAbbRicArray[i - 1].Split(',');
                    foreach (string abbreviazioneDiLibro in abbreviazioniDiLibro)
                        MainWindow.Testi.LibriAbbreviazioniRiconosciute[abbreviazioneDiLibro] = i;
                }
            }
        }

        if (Application.Current.MainWindow is MainWindow mw)
        {
            if (mw.FindName("Dock") is AvalonDock.DockingManager dock)
            {
                if (cambiaTheme)
                {
                    Services.ThemeManager.ApplyDockTheme(dock, _settings.ThemeMode);
                    App.ThemeManager.HookSystemThemeChanges(dock, _settings.ThemeMode);
                }
            }
            if (cambiaLanguage)
            {
                mw.UpdateShortcutBindings(_settings.Language);
            }
        }

        App.Settings.Save(_settings);
    }
}
