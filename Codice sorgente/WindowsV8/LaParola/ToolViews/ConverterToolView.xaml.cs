using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace LaParola.ToolViews;

public partial class ConverterToolView : UserControl
{
    private readonly double[] conversionePesi = [1, 0.82, 8.2, 10.9333333333333, 16.4, 820, 49200, 327];
    private readonly double[] conversioneLunghezze = [1, 0.018520833333333, 0.0748033333333, 0.22225, 0.4445, 2.667, 889, 1.778, 1422.4, 177.8];
    private readonly double[] conversioneCapacita = [1, 0.486111111111111, 5.83333333333333, 35, 350, 2.33333333333333, 3.5, 11.6666666666667, 35];
    private readonly double[] conversioneMonete = [1, 0.390625, 0.78125, 3.125, 50, 50, 100, 200, 1250, 5000, 300000];

    readonly bool iniziato = false;

    public ConverterToolView()
    {
        InitializeComponent();

        Row0ComboBox.ItemsSource = ((string)(Application.Current.TryFindResource("MisurePesiUnita") ?? "")).Split(',');
        Row0ComboBox.SelectedIndex = 0;
        Row1ComboBox.ItemsSource = ((string)(Application.Current.TryFindResource("MisureLunghezzeUnita") ?? "")).Split(',');
        Row1ComboBox.SelectedIndex = 0;
        Row2ComboBox.ItemsSource = ((string)(Application.Current.TryFindResource("MisureCapacitaUnita") ?? "")).Split(',');
        Row2ComboBox.SelectedIndex = 0;
        Row3ComboBox.ItemsSource = ((string)(Application.Current.TryFindResource("MisureMoneteUnita") ?? "")).Split(',');
        Row3ComboBox.SelectedIndex = 0;

        Row0RightComboBox.ItemsSource = ((string)(Application.Current.TryFindResource("MisurePesiUnita") ?? "")).Split(',');
        Row0RightComboBox.SelectedIndex = 0;
        Row1RightComboBox.ItemsSource = ((string)(Application.Current.TryFindResource("MisureLunghezzeUnita") ?? "")).Split(',');
        Row1RightComboBox.SelectedIndex = 0;
        Row2RightComboBox.ItemsSource = ((string)(Application.Current.TryFindResource("MisureCapacitaUnita") ?? "")).Split(',');
        Row2RightComboBox.SelectedIndex = 0;
        Row3RightComboBox.ItemsSource = ((string)(Application.Current.TryFindResource("MisureMoneteUnita") ?? "")).Split(',');
        Row3RightComboBox.SelectedIndex = 0;

        iniziato = true;
    }

    private void HelpFlyout_OnHelpClicked(object sender, RoutedEventArgs e)
    {
        // TODO2: Open correct help section
        MessageBox.Show("Open Help Centre");
    }

    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        char a = (((Control)sender).Name)[3];
        AggiornaConversione(a);
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        char a = (((Control)sender).Name)[3];
        AggiornaConversione(a);
    }

    private void AggiornaConversione(char a)
    {
        if (!iniziato) return;
        switch (a)
        {
            case '0':
                if (Row0ComboBox.SelectedIndex != -1 && Row0RightComboBox.SelectedIndex != -1 && !string.IsNullOrEmpty(Row0TextBox.Text))
                {
                    Row0RightTextBlock.Text = (Convert.ToDouble(Row0TextBox.Text) * conversionePesi[Row0ComboBox.SelectedIndex] / conversionePesi[Row0RightComboBox.SelectedIndex]).ToString("0.###", CultureInfo.CurrentCulture);
                }
                break;
            case '1':
                if (Row1ComboBox.SelectedIndex != -1 && Row1RightComboBox.SelectedIndex != -1 && !string.IsNullOrEmpty(Row1TextBox.Text))
                {
                    Row1RightTextBlock.Text = (Convert.ToDouble(Row1TextBox.Text) * conversioneLunghezze[Row1ComboBox.SelectedIndex] / conversioneLunghezze[Row1RightComboBox.SelectedIndex]).ToString("0.###", CultureInfo.CurrentCulture);
                }
                break;
            case '2':
                if (Row2ComboBox.SelectedIndex != -1 && Row2RightComboBox.SelectedIndex != -1 && !string.IsNullOrEmpty(Row2TextBox.Text))
                {
                    Row2RightTextBlock.Text = (Convert.ToDouble(Row2TextBox.Text) * conversioneCapacita[Row2ComboBox.SelectedIndex] / conversioneCapacita[Row2RightComboBox.SelectedIndex]).ToString("0.###", CultureInfo.CurrentCulture);
                }
                break;
            case '3':
                if (Row3ComboBox.SelectedIndex != -1 && Row3RightComboBox.SelectedIndex != -1 && !string.IsNullOrEmpty(Row3TextBox.Text))
                {
                    Row3RightTextBlock.Text = FormataNumero(Convert.ToDouble(Row3TextBox.Text) * conversioneMonete[Row3ComboBox.SelectedIndex] / conversioneMonete[Row3RightComboBox.SelectedIndex]);
                }
                break;
            default:
                break;
       }
    }

    private static string FormataNumero(double value, CultureInfo? culture = null)
    {

        culture ??= CultureInfo.CurrentCulture;

        if (double.IsNaN(value) || double.IsInfinity(value))
            return value.ToString(culture);

        if (value == 0.0)
            return "0";

        // 1) Arrotonda a 3 cifre significative
        double rounded = RoundToSignificantDigits(value, 3);
        double abs = Math.Abs(rounded);

        // Decidi quanti decimali mostrare (mai scientifico)
        int decimals;

        if (abs < 1.0)
        {
            // Piccoli: 0.000... + 3 cifre significative
            int exp = (int)Math.Floor(Math.Log10(abs));  // negativo
            decimals = -exp + 2;                         // = zeri iniziali + 3
                                                         // opzionale: evita stringhe enormi se arrivano valori estremi
            decimals = Math.Min(decimals, 50);

            // format con '0' per mantenere anche eventuali zeri finali delle 3 cifre significative
            string fmt = "0." + new string('0', decimals);
            string r = rounded.ToString(fmt, culture);
            while (r.EndsWith('0') && r.Length > 3) // non 0.0
            {
                r = r[..^1]; // rimuove eventuali zeri finali inutili (es. 0.000500 -> 0.0005)
            }
            return r;
        }
        else
        {
            // >=1: massimo 3 decimali (senza zeri finali inutili)
            return rounded.ToString("0.###", culture);
        }
    }

    static double RoundToSignificantDigits(double value, int significantDigits)
    {
        if (value == 0.0) return 0.0;
        if (double.IsNaN(value) || double.IsInfinity(value)) return value;

        double abs = Math.Abs(value);
        int exponent = (int)Math.Floor(Math.Log10(abs));
        int scale = significantDigits - 1 - exponent;

        double factor = Math.Pow(10, scale);
        return Math.Round(value * factor, 0, MidpointRounding.AwayFromZero) / factor;
    }
}
