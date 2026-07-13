using LaParola.Models;
using LaParola.Services;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace LaParola;

public partial class App : Application
{
    public static SettingsService Settings { get; } = new();
    public static ThemeManager ThemeManager { get; } = new();
    public static LocalizationManager Localization { get; } = new();
    public static DockingHostService DockingHost { get; } = new();

    private AppSettings ImpostazioniApp { get; set; } = default!;

    protected override async void OnStartup(StartupEventArgs e)
    {
        ImpostazioniApp = Settings.Load();

        CultureInfo culture = ImpostazioniApp.Lingua switch
        {
            "" => CultureInfo.CurrentUICulture,
            "en" => new CultureInfo("en-US"),
            _ => new CultureInfo("it-IT"),
        };
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;

        FrameworkElement.LanguageProperty.OverrideMetadata(
                    typeof(FrameworkElement),
                    new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(culture.IetfLanguageTag)));

        if (ImpostazioniApp.Lingua == "")
        {
            ImpostazioniApp.Lingua = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            if (ImpostazioniApp.Lingua != "it" && ImpostazioniApp.Lingua != "en")
            {
                ImpostazioniApp.Lingua = "it";
            }
        }

        LocalizationManager.ApplyLanguage(ImpostazioniApp.Lingua);
        ThemeManager.ApplyTheme(ImpostazioniApp.ThemeMode);

        base.OnStartup(e);

        MainWindow main = new(ImpostazioniApp);
        MainWindow = main;
        main.Show();

        await MessageService.CheckForNewMessagesAsync();
    }
}
