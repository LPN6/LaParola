using LaParola.Models;
using System.Windows;
using System.Windows.Controls;

namespace LaParola.ToolViews;

public partial class OptionsToolView : UserControl
{
    public OptionsToolView()
    {
        InitializeComponent();

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
    }

    private void HelpFlyout_OnHelpClicked(object sender, RoutedEventArgs e)
    {
        // TODO2: Open correct help section
        MessageBox.Show("Open Help Centre");
    }

    private void SettingChanged(object sender, RoutedEventArgs e)
    {
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
                    Services.ThemeManager.ApplyDockTheme(dock, _settings.ThemeMode);
                if (cambiaLanguage)
                    App.ThemeManager.HookSystemThemeChanges(_settings.ThemeMode, dock);
            }
        }

        App.Settings.Save(_settings);
    }
}
