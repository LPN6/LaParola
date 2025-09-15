using System;
using System.Collections.Generic;
using System.Globalization;
using TestiBiblici;

namespace LaParola
{
    public partial class Paragrafo : Template
    {
        private RichTextBoxEx richTextControllo;

        public Paragrafo(Principale genitore, RichTextBoxEx rtfBox)
        {
            if (genitore == null)
                throw new ArgumentNullException("genitore");

            InitializeComponent();
            guidaFile.HelpNamespace = genitore.NomeFileGuida();

            if (Principale.isRunningOnMono)
            {
                rbAllineamentoGiusto.Visible = false;
                cbPunti.Visible = false;
                gbRientro.Visible = false;
                gbTab.Visible = false;
            }

            if (rtfBox != null)
            {
                richTextControllo = rtfBox;

                switch (richTextControllo.SelectionAlignment)
                {
                    case RichTextBoxEx.TextAlign.Left:
                        rbAllineamentoSinistra.Checked = true;
                        break;
                    case RichTextBoxEx.TextAlign.Center:
                        rbAllineamentoCentro.Checked = true;
                        break;
                    case RichTextBoxEx.TextAlign.Right:
                        rbAllineamentoDestra.Checked = true;
                        break;
                    case RichTextBoxEx.TextAlign.Justify:
                        rbAllineamentoGiusto.Checked = true;
                        break;
                }

                if (!Principale.isRunningOnMono)
                {
                    tbRientroSinistra.Text = Math.Round((richTextControllo.SelectionIndent / Principale.pixelPerCm), 1).ToString(CultureInfo.CurrentCulture);
                    tbRientroSporgente.Text = Math.Round((richTextControllo.SelectionHangingIndent / Principale.pixelPerCm), 1).ToString(CultureInfo.CurrentCulture);
                    tbRientroDestra.Text = Math.Round((richTextControllo.SelectionRightIndent / Principale.pixelPerCm), 1).ToString(CultureInfo.CurrentCulture);

                    cbPunti.Checked = richTextControllo.SelectionBullet;

                    foreach (int tab in richTextControllo.SelectionTabs)
                        lbTab.Items.Add(Convert.ToInt32(tab * 10 / Principale.pixelPerCm, CultureInfo.InvariantCulture) / 10.0);
                }
            }
            else
                btnOK.Enabled = false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (rbAllineamentoSinistra.Checked)
                richTextControllo.SelectionAlignment = RichTextBoxEx.TextAlign.Left;
            if (rbAllineamentoCentro.Checked)
                richTextControllo.SelectionAlignment = RichTextBoxEx.TextAlign.Center;
            if (rbAllineamentoDestra.Checked)
                richTextControllo.SelectionAlignment = RichTextBoxEx.TextAlign.Right;
            if (rbAllineamentoGiusto.Checked)
                richTextControllo.SelectionAlignment = RichTextBoxEx.TextAlign.Justify;

            if (!Principale.isRunningOnMono)
                PuntiNonMono();

            Close();
        }

        private void PuntiNonMono()
        {
            richTextControllo.SelectionBullet = cbPunti.Checked;

            richTextControllo.SelectionIndent = Convert.ToInt32(Convert.ToDouble(tbRientroSinistra.Text, CultureInfo.CurrentCulture) * Principale.pixelPerCm);
            richTextControllo.SelectionHangingIndent = Convert.ToInt32(Convert.ToDouble(tbRientroSporgente.Text, CultureInfo.CurrentCulture) * Principale.pixelPerCm);
            richTextControllo.SelectionRightIndent = Convert.ToInt32(Convert.ToDouble(tbRientroDestra.Text, CultureInfo.CurrentCulture) * Principale.pixelPerCm);

            List<int> tabulazioni = new List<int>(32);
            foreach (object tab in lbTab.Items)
            {
                if (tabulazioni.Count < 32) // perché Windows ha un limite di 24 tabulazioni
                    tabulazioni.Add(Convert.ToInt32(Convert.ToSingle(tab, CultureInfo.InvariantCulture) * Principale.pixelPerCm, CultureInfo.InvariantCulture));
            }
            richTextControllo.SelectionTabs = tabulazioni.ToArray();
        }

        private void pulNuovoTab_Click(object sender, EventArgs e)
        {
            float tab = -1;
            using (InputBox inputBox = new InputBox(Principale.LocRM.GetString("ParagraphTabs"), Principale.LocRM.GetString("ParagraphTabsNew"), ""))
            {
                inputBox.ShowDialog();
                try
                {
                    tab = Convert.ToSingle(inputBox.Risposta, CultureInfo.InvariantCulture);
                }
                catch (OverflowException) { } // tab rimane -1, e quindi non la tabulazione non viene aggiunta
                catch (FormatException) { }
            }
            if (tab > 0)
                lbTab.Items.Add(tab);
        }

        private void pulCancellaTab_Click(object sender, EventArgs e)
        {
            if (lbTab.SelectedIndex >= 0)
                lbTab.Items.RemoveAt(lbTab.SelectedIndex);
        }

        private void lbTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            pulCancellaTab.Enabled = (lbTab.SelectedIndex >= 0);
        }

    }
}