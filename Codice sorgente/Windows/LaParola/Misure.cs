using System;
using System.Globalization;
using System.Windows.Forms;

namespace LaParola
{
    public partial class Misure : Template
    {
        private double[] conversionePesi = new double[8] { 1, 0.82, 8.2, 10.9333333333333, 16.4, 820, 49200, 327 };
        private double[] conversioneLunghezze = new double[10] { 1, 0.018520833333333, 0.0748033333333, 0.22225, 0.4445, 2.667, 889, 1.778, 1422.4, 177.8 };
        private double[] conversioneCapacita = new double[9] { 1, 0.486111111111111, 5.83333333333333, 35, 350, 2.33333333333333, 3.5, 11.6666666666667, 35 };
        private double[] conversioneMonete = new double[11] { 1, 0.3125, 0.625, 2.5, 40, 40, 80, 160, 1000, 4000, 240000 };

        char separatoreNumerico = '.';

        public Misure(Principale formGenitore)
        {
            if (formGenitore == null)
                throw new ArgumentNullException("formGenitore");

            InitializeComponent();

            guidaFile.HelpNamespace = formGenitore.NomeFileGuida();

            string separatoreNumericoStringa = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            if (!String.IsNullOrEmpty(separatoreNumericoStringa))
                separatoreNumerico = separatoreNumericoStringa[0];

            for (int i = 0; i < cbPesi1.Items.Count; ++i)
                cbPesi2.Items.Add(cbPesi1.Items[i]);
            cbPesi1.SelectedIndex = 0;
            cbPesi2.SelectedIndex = 0;

            for (int i = 0; i < cbLunghezze1.Items.Count; ++i)
                cbLunghezze2.Items.Add(cbLunghezze1.Items[i]);
            cbLunghezze1.SelectedIndex = 0;
            cbLunghezze2.SelectedIndex = 0;

            for (int i = 0; i < cbCapacita1.Items.Count; ++i)
                cbCapacita2.Items.Add(cbCapacita1.Items[i]);
            cbCapacita1.SelectedIndex = 0;
            cbCapacita2.SelectedIndex = 0;

            for (int i = 0; i < cbMonete1.Items.Count; ++i)
                cbMonete2.Items.Add(cbMonete1.Items[i]);
            cbMonete1.SelectedIndex = 0;
            cbMonete2.SelectedIndex = 0;
        }

        private void btnCanc_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tb1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ',' || e.KeyChar == '.' || e.KeyChar == separatoreNumerico)
                e.KeyChar = separatoreNumerico;
            else if ((e.KeyChar < '0' || e.KeyChar > '9') && e.KeyChar != (char)Keys.Back)
                e.Handled = true;
        }

        private void ConversionChanged(object sender, EventArgs e)
        {
            Label eti2 = null;
            ComboBox cb1 = null, cb2 = null;
            TextBox tb1 = null;
            double[] conversione = new double[11];
            switch (((Control)sender).Tag.ToString())
            {
                case "Pesi":
                    tb1 = tbPesi1;
                    eti2 = etiPesi2;
                    cb1 = cbPesi1;
                    cb2 = cbPesi2;
                    conversionePesi.CopyTo(conversione, 0);
                    break;
                case "Lunghezze":
                    tb1 = tbLunghezze1;
                    eti2 = etiLunghezze2;
                    cb1 = cbLunghezze1;
                    cb2 = cbLunghezze2;
                    conversioneLunghezze.CopyTo(conversione, 0);
                    break;
                case "Capacita":
                    tb1 = tbCapacita1;
                    eti2 = etiCapacita2;
                    cb1 = cbCapacita1;
                    cb2 = cbCapacita2;
                    conversioneCapacita.CopyTo(conversione, 0);
                    break;
                case "Monete":
                    tb1 = tbMonete1;
                    eti2 = etiMonete2;
                    cb1 = cbMonete1;
                    cb2 = cbMonete2;
                    conversioneMonete.CopyTo(conversione, 0);
                    break;
                default:
                    return;
            }
            if (String.IsNullOrEmpty(tb1.Text))
                eti2.Text = "";
            else if (cb1.SelectedIndex >= 0 && cb2.SelectedIndex >= 0) // all'apertura della form, non è così
            {
                double nuovoValore = Convert.ToDouble(tb1.Text, CultureInfo.CurrentCulture) * conversione[cb1.SelectedIndex] / conversione[cb2.SelectedIndex];
                if (nuovoValore == 0)
                    eti2.Text = "0";
                else
                {
                    int cifreDecimali = 3 - Convert.ToInt32(Math.Log10(nuovoValore));
                    if (cifreDecimali < 0)
                        cifreDecimali = 0;
                    String nuovoValoreStringa = nuovoValore.ToString("N" + cifreDecimali.ToString(CultureInfo.CurrentCulture), CultureInfo.CurrentCulture);
                    while (nuovoValoreStringa.EndsWith("0", StringComparison.Ordinal) && nuovoValoreStringa.IndexOf(separatoreNumerico) >= 0)
                        nuovoValoreStringa = nuovoValoreStringa.Remove(nuovoValoreStringa.Length - 1);
                    while (nuovoValoreStringa.EndsWith(separatoreNumerico.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal))
                        nuovoValoreStringa = nuovoValoreStringa.Remove(nuovoValoreStringa.Length - 1);
                    eti2.Text = nuovoValoreStringa;
                }
            }
        }
    }
}
