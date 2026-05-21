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

    protected override void OnStartup(StartupEventArgs e)
    {
        ImpostazioniApp = Settings.Load();

        CultureInfo culture = ImpostazioniApp.Language switch
        {
            "" => CultureInfo.CurrentUICulture,
            "it" => new CultureInfo("it-IT"),
            _ => new CultureInfo("en-US"),
        };
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        CultureInfo.DefaultThreadCurrentCulture = culture;

        FrameworkElement.LanguageProperty.OverrideMetadata(
                    typeof(FrameworkElement),
                    new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(culture.IetfLanguageTag)));

        if (ImpostazioniApp.Language=="")
        {
            ImpostazioniApp.Language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            if (ImpostazioniApp.Language!="it" && ImpostazioniApp.Language!="en")
            {
                ImpostazioniApp.Language = "en";
            }
        }

        LocalizationManager.ApplyLanguage(ImpostazioniApp.Language);
        ThemeManager.ApplyTheme(ImpostazioniApp.ThemeMode);

        base.OnStartup(e);

        MainWindow main = new(ImpostazioniApp);
        MainWindow = main;
        main.Show();
    }
}
