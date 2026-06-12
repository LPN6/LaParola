using AvalonDock.Layout;
using LaParola.DocumentViews;
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
    // TODO2 - results in same editor window or new
    // TODO2 - References: context in searches

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

        TestoVersetti.IsChecked = MainWindow.settings.Formato.TestoVisualizzato == TestoVisualizzato.Versetti;
        TestoParagrafi.IsChecked = MainWindow.settings.Formato.TestoVisualizzato == TestoVisualizzato.Paragrafi;
        TestoNessuno.IsChecked = MainWindow.settings.Formato.TestoVisualizzato == TestoVisualizzato.Nessuno;

        TestoTitoli.IsChecked = MainWindow.settings.Formato.TitoliVisualizzati;

        switch (MainWindow.settings.Formato.RiferimentoTipo)
        {
            case RiferimentoTipo.Virgola:
                RiferimentiTipoVirgola.IsChecked = true;
                break;
            case RiferimentoTipo.Citazione:
                RiferimentiTipoCitazione.IsChecked = true;
                break;
            default:
                RiferimentiTipoDuePunti.IsChecked = true;
                break;
        }

        switch (MainWindow.settings.Formato.RiferimentoFormato)
        {
            case RiferimentoFormato.Intero:
                RiferimentiFormatoIntero.IsChecked = true;
                break;
            case RiferimentoFormato.Nessuno:
                RiferimentiFormatoNessuno.IsChecked = true;
                break;
            default:
                RiferimentiFormatoAbbreviazione.IsChecked = true;
                break;
        }

        switch (MainWindow.settings.Formato.RiferimentoPosto)
        {
            case RiferimentoPosto.PrimaRigaDiversa:
                RiferimentiPosizionePrimaDiversa.IsChecked = true;
                break;
            case RiferimentoPosto.Dopo:
                RiferimentiPosizioneDopo.IsChecked = true;
                break;
            default:
                RiferimentiPosizionePrimaStessa.IsChecked = true;
                break;
        }

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

        switch (MainWindow.settings?.ControlloMessaggi)
        {
            case 0:
                AggiornamentiMessaggiMai.IsChecked = true;
                break;
            case 1:
                AggiornamentiMessaggiGiorno.IsChecked = true;
                break;
            case 7:
                AggiornamentiMessaggiSettimana.IsChecked = true;
                break;
            case 30:
                AggiornamentiMessaggiMese.IsChecked = true;
                break;
            default:
                AggiornamentiMessaggiGiorno.IsChecked = true;
                break;
        }

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
                tbEsempio.FontSize = dim != null ? (double)dim * 4.0 / 3.0 : 16;
            }
            catch
            {
                tbEsempio.FontSize = 12;
            }
            tbEsempio.FontWeight = grassetto != null ? (bool)grassetto ? FontWeights.Bold : FontWeights.Normal : FontWeights.Normal;
            tbEsempio.FontStyle = corsivo != null ? (bool)corsivo ? FontStyles.Italic : FontStyles.Normal : FontStyles.Normal;
            tbEsempio.TextDecorations = sottolineato != null ? (bool)sottolineato ? TextDecorations.Underline : null : null;

            UpdateFontColorTextBlock(tbEsempio, colore, Services.ThemeManager.IsDark(MainWindow.settings.ThemeMode));
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

    private static void UpdateFontColorTextBlock(TextBlock tbEsempio, Color? colore, bool isDarkTheme)
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

    private void OptionsTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (e.NewValue is TreeViewItem selectedItem)
        {
            FrameworkElement? targetElement = null;

            // Map the TreeViewItem names to their structural counterparts on the right side
            switch (selectedItem.Name)
            {
                case "TreeAspetto":
                    targetElement = SectionAspettoHeader;
                    break;
                case "NodeTheme":
                    targetElement = SectionTheme;
                    break;
                case "NodeLanguage":
                    targetElement = SectionLanguage;
                    break;
                case "TreeFormato":
                    targetElement = SectionFormatoHeader;
                    break;
                case "NodeText":
                    targetElement = SectionText;
                    break;
                case "NodeReferences":
                    targetElement = SectionReferences;
                    break;
                case "NodeFonts":
                    targetElement = SectionFonts;
                    break;
                case "TreeAggiornamenti":
                    targetElement = SectionAggiornamentiHeader;
                    break;
                case "NodeMessages":
                    targetElement = SectionMessages;
                    break;
            }

            if (targetElement != null)
            {
                // Calculate the exact target offset relative to the inner canvas container
                GeneralTransform transform = targetElement.TransformToVisual(RightContentStackPanel);
                Point relativeOffset = transform.Transform(new Point(0, 0));

                // Scroll the right viewport straight to the calculated Y position
                RightScrollViewer.ScrollToVerticalOffset(relativeOffset.Y);
            }
        }
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
                    AggiornaDocumentiVisualizzazione();
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
                    AggiornaDocumentiVisualizzazione();
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
                    AggiornaDocumentiVisualizzazione();
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
                    AggiornaDocumentiVisualizzazione();
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
                    AggiornaDocumentiVisualizzazione();
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
        bool cambiaTema = false;
        bool cambiaLingua = false;
        bool cambiaFormato = false;

        TestoVisualizzato testoVisualizzatoPrecedente = _settings.Formato.TestoVisualizzato;
        TestoVisualizzato testoVisualizzatoAttuale = TestoVersetti.IsChecked == true ? TestoVisualizzato.Versetti :
                                                  TestoParagrafi.IsChecked == true ? TestoVisualizzato.Paragrafi :
                                                  TestoNessuno.IsChecked == true ? TestoVisualizzato.Nessuno :
                                                  testoVisualizzatoPrecedente; // fallback al precedente se nessuno è selezionato
        if (testoVisualizzatoAttuale != testoVisualizzatoPrecedente)
        {
            _settings.Formato.TestoVisualizzato = testoVisualizzatoAttuale;
            cambiaFormato = true;
        }

        bool titoliVisualizzatiAttuali = TestoTitoli.IsChecked == true;
        if (titoliVisualizzatiAttuali != _settings.Formato.TitoliVisualizzati)
        {
            _settings.Formato.TitoliVisualizzati = titoliVisualizzatiAttuali;
            cambiaFormato = true;
        }

        RiferimentoTipo riferimentoTipoPrecedente = _settings.Formato.RiferimentoTipo;
        RiferimentoTipo riferimentoTipoAttuale = RiferimentiTipoDuePunti.IsChecked == true ? RiferimentoTipo.DuePunti :
            RiferimentiTipoVirgola.IsChecked == true ? RiferimentoTipo.Virgola :
            RiferimentiTipoCitazione.IsChecked == true ? RiferimentoTipo.Citazione :
            riferimentoTipoPrecedente;
        if (riferimentoTipoPrecedente != riferimentoTipoAttuale)
        {
            _settings.Formato.RiferimentoTipo = riferimentoTipoAttuale;
            cambiaFormato = true;
        }

        RiferimentoFormato riferimentoFormatoPrecedente = _settings.Formato.RiferimentoFormato;
        RiferimentoFormato riferimentoFormatoAttuale = RiferimentiFormatoIntero.IsChecked == true ? RiferimentoFormato.Intero :
            RiferimentiFormatoAbbreviazione.IsChecked == true ? RiferimentoFormato.Abbreviazione :
            RiferimentiFormatoNessuno.IsChecked == true ? RiferimentoFormato.Nessuno :
            riferimentoFormatoPrecedente;
        if (riferimentoFormatoPrecedente != riferimentoFormatoAttuale)
        {
            _settings.Formato.RiferimentoFormato = riferimentoFormatoAttuale;
            cambiaFormato = true;
        }

        RiferimentoPosto riferimentoPostoPrecedente = _settings.Formato.RiferimentoPosto;
        RiferimentoPosto riferimentoPostoAttuale = RiferimentiPosizionePrimaStessa.IsChecked == true ? RiferimentoPosto.PrimaStessaRiga :
            RiferimentiPosizionePrimaDiversa.IsChecked == true ? RiferimentoPosto.PrimaRigaDiversa :
            RiferimentiPosizioneDopo.IsChecked == true ? RiferimentoPosto.Dopo :
            riferimentoPostoPrecedente;
        if (riferimentoPostoPrecedente != riferimentoPostoAttuale)
        {
            _settings.Formato.RiferimentoPosto = riferimentoPostoAttuale;
            cambiaFormato = true;
        }

        ThemeState newTheme = ThemeDark.IsChecked == true ? ThemeState.Dark :
                                  ThemeLight.IsChecked == true ? ThemeState.Light : ThemeState.System;
        if (newTheme != _settings.ThemeMode)
        {
            _settings.ThemeMode = newTheme;
            Services.ThemeManager.ApplyTheme(_settings.ThemeMode);
            cambiaTema = true;
        }

        if (LanguageCombo.SelectedItem is FrameworkElement fe && fe.Tag is string lang)
        {
            if (lang != _settings.Language)
            {
                _settings.Language = lang;
                Services.LocalizationManager.ApplyLanguage(_settings.Language);
                cambiaLingua = true;

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

        int agg = AggiornamentiMessaggiGiorno.IsChecked == true ? 1 :
                  AggiornamentiMessaggiSettimana.IsChecked == true ? 7 :
                  AggiornamentiMessaggiMese.IsChecked == true ? 30 :
                  0;
        if (agg != _settings.ControlloMessaggi)
        {
            _settings.ControlloMessaggi = agg;
        }

        if (Application.Current.MainWindow is MainWindow mw)
        {
            if (mw.FindName("Dock") is AvalonDock.DockingManager dock)
            {
                if (cambiaTema)
                {
                    Services.ThemeManager.ApplyDockTheme(dock, _settings.ThemeMode);
                    App.ThemeManager.HookSystemThemeChanges(dock, _settings.ThemeMode);
                }
                if (cambiaLingua)
                {
                    Services.LocalizationManager.RefreshToolTitles(dock.Layout);
                }
            }
            if (cambiaLingua)
            {
                mw.UpdateShortcutBindings(_settings.Language);
            }
        }

        if (cambiaFormato | cambiaTema)
        {
            AggiornaDocumentiVisualizzazione();
        }

        App.Settings.Save(_settings);
    }

    private static void AggiornaDocumentiVisualizzazione()
    {
        List<LayoutDocument>? viewers = Funzioni.ListViewerDocuments();

        if (viewers != null)
        {
            foreach (LayoutDocument d in viewers)
                (d.Content as ViewerDocumentView)?.CambiaFormato();
        }
    }
}
