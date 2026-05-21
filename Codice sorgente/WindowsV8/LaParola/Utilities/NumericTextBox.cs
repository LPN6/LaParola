using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LaParola.Utilities
{
    public static class NumericTextBox
    {
        public static readonly DependencyProperty IsEnabledProperty =
             DependencyProperty.RegisterAttached(
                 "IsEnabled",
                 typeof(bool),
                 typeof(NumericTextBox),
                 new PropertyMetadata(false, OnIsEnabledChanged));

        public static void SetIsEnabled(DependencyObject element, bool value) =>
            element.SetValue(IsEnabledProperty, value);

        public static bool GetIsEnabled(DependencyObject element) =>
            (bool)element.GetValue(IsEnabledProperty);

        // Opzionale: se vuoi formattare/normalizzare alla perdita focus
        public static readonly DependencyProperty NormalizeOnLostFocusProperty =
            DependencyProperty.RegisterAttached(
                "NormalizeOnLostFocus",
                typeof(bool),
                typeof(NumericTextBox),
                new PropertyMetadata(true));

        public static void SetNormalizeOnLostFocus(DependencyObject element, bool value) =>
            element.SetValue(NormalizeOnLostFocusProperty, value);

        public static bool GetNormalizeOnLostFocus(DependencyObject element) =>
            (bool)element.GetValue(NormalizeOnLostFocusProperty);

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not TextBox tb) return;

            if ((bool)e.NewValue)
            {
                tb.PreviewTextInput += Tb_PreviewTextInput;
                DataObject.AddPastingHandler(tb, Tb_OnPaste);
                tb.PreviewKeyDown += Tb_PreviewKeyDown; // per Decimal del tastierino
                tb.LostFocus += Tb_LostFocus;
            }
            else
            {
                tb.PreviewTextInput -= Tb_PreviewTextInput;
                DataObject.RemovePastingHandler(tb, Tb_OnPaste);
                tb.PreviewKeyDown -= Tb_PreviewKeyDown;
                tb.LostFocus -= Tb_LostFocus;
            }
        }

        private static CultureInfo Culture => CultureInfo.CurrentCulture;

        private static string DecimalSep =>
            Culture.NumberFormat.NumberDecimalSeparator;

        private static string GroupSep =>
            Culture.NumberFormat.NumberGroupSeparator;

        // Intercetta il tasto "Decimal" del tastierino numerico e lo traduce
        private static void Tb_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is not TextBox tb) return;

            if (e.Key == Key.Decimal)
            {
                e.Handled = true;
                InsertText(tb, DecimalSep);
            }
        }

        private static void Tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is not TextBox tb) return;

            // Converte '.' e ',' nel separatore di cultura
            string incoming = e.Text == "." || e.Text == "," ? DecimalSep : e.Text;

            // Se non valido, blocca.
            if (!IsProposedTextValid(tb, incoming))
            {
                e.Handled = true;
                return;
            }

            // Se stiamo convertendo '.' o ',' -> inseriamo noi e blocchiamo l’input originale
            if (incoming != e.Text)
            {
                e.Handled = true;
                InsertText(tb, incoming);
            }
        }

        private static void Tb_OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (sender is not TextBox tb)
                return;

            if (!e.DataObject.GetDataPresent(typeof(string)))
            {
                e.CancelCommand();
                return;
            }

            string raw = (string)e.DataObject.GetData(typeof(string));

            // Normalizza i separatori nel testo incollato
            string sanitized = Sanitize(raw);

            if (!IsProposedTextValid(tb, sanitized))
            {
                e.CancelCommand();
                return;
            }

            // Per assicurare la conversione, annulliamo il paste standard e inseriamo noi
            e.CancelCommand();
            InsertText(tb, sanitized);
        }

        private static void Tb_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is not TextBox tb) return;
            if (!GetNormalizeOnLostFocus(tb)) return;

            string txt = tb.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(txt)) return;

            txt = Sanitize(txt);

            // Se parseabile, riscrive in formato cultura corrente (senza cambiare il valore)
            if (decimal.TryParse(txt,
                                NumberStyles.AllowDecimalPoint,
                                Culture,
                                out decimal value))
            {
                tb.Text = value.ToString(Culture);
            }
        }

        private static bool IsProposedTextValid(TextBox tb, string incoming)
        {
            // incoming può essere più caratteri (paste)
            if (incoming is null) return false;

            string proposed = GetProposedText(tb, incoming);
            proposed = Sanitize(proposed);

            // Permetti stringa vuota (utente può cancellare)
            if (string.IsNullOrEmpty(proposed))
                return true;

            // Permetti iniziare con separatore: "." o "," -> "0<sep>"
            if (proposed == DecimalSep)
                return true;

            // Un solo separatore decimale
            if (proposed.Count(c => c.ToString() == DecimalSep) > 1)
                return false;

            // Solo cifre e separatore
            if (proposed.Any(c => !char.IsDigit(c) && c.ToString() != DecimalSep))
                return false;

            // Deve essere parseabile (accetta anche "12," durante digitazione? Decimal.TryParse lo accetta spesso,
            // ma per sicurezza non forziamo: consideriamo valido se:
            // - finisce con separatore (digitazione in corso)
            // - oppure parseabile come numero
            if (proposed.EndsWith(DecimalSep, StringComparison.Ordinal))
                return true;

            return decimal.TryParse(proposed,
                                   NumberStyles.AllowDecimalPoint,
                                   Culture,
                                   out _);
        }

        private static string GetProposedText(TextBox tb, string incoming)
        {
            string text = tb.Text ?? string.Empty;
            int start = tb.SelectionStart;
            int length = tb.SelectionLength;

            // Simula l’inserimento al caret (rispetta selezione)
            return text.Remove(start, length).Insert(start, incoming);
        }

        private static string Sanitize(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            // Rimuove separatori di gruppo e spazi
            string s = input.Replace(GroupSep, string.Empty)
                         .Replace(" ", string.Empty)
                         .Trim();

            // Converte '.' e ',' nel separatore corrente
            s = s.Replace(".", DecimalSep).Replace(",", DecimalSep);

            return s;
        }

        private static void InsertText(TextBox tb, string text)
        {
            int start = tb.SelectionStart;
            tb.SelectedText = text;
            tb.SelectionStart = start + (text?.Length ?? 0);
        }
    }
}