using System;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace LaParola
{
    public partial class CreaRiferimento : Template
    {
        internal string riferimento = "";

        public CreaRiferimento(Principale formGenitore)
        {
            if (formGenitore == null)
                throw new ArgumentNullException("formGenitore");

            InitializeComponent();

            guidaFile.HelpNamespace = formGenitore.NomeFileGuida();
            string libro = "";

            for (int i = 1; i <= 73; ++i)
            {
                libro = Principale.testi.GetLibroNome(i);
                cbLibro1.Items.Add(libro);
                cbLibro2.Items.Add(libro);
            }
            cbLibro1.SelectedIndex = 0;
            cbLibro2.SelectedIndex = 0;
        }

        private void cbBrano_CheckedChanged(object sender, EventArgs e)
        {
            etiLibro.Visible = !cbBrano.Checked;
            etiCapitolo.Visible = !cbBrano.Checked;
            etiVersetto.Visible = !cbBrano.Checked;
            etiParola.Visible = !cbBrano.Checked;

            etiLibro1.Visible = cbBrano.Checked;
            etiCapitolo1.Visible = cbBrano.Checked;
            etiVersetto1.Visible = cbBrano.Checked;
            etiParola1.Visible = cbBrano.Checked;

            etiLibro2.Visible = cbBrano.Checked;
            etiCapitolo2.Visible = cbBrano.Checked;
            etiVersetto2.Visible = cbBrano.Checked;
            etiParola2.Visible = cbBrano.Checked;
            cbLibro2.Visible = cbBrano.Checked;
            txtCapitolo2.Visible = cbBrano.Checked;
            txtVersetto2.Visible = cbBrano.Checked;
            txtParola2.Visible = cbBrano.Checked;
        }

        private void txt_TextChanged(object sender, EventArgs e)
        {
            // quando il testo è cambiato, controlla che ci siano solo dei numeri
            TextBox senderAsTB = (TextBox)sender;
            string s = senderAsTB.Text;
            int selInizio = senderAsTB.SelectionStart;
            for (int i = s.Length - 1; i >= 0; --i)
                if (!char.IsDigit(s[i]))
                {
                    s = s.Remove(i, 1);
                    if (i < selInizio)
                        --selInizio;
                }
            if (s != senderAsTB.Text)
            {
                senderAsTB.Text = s;
                senderAsTB.SelectionStart = selInizio;
            }
        }

        private void btnCanc_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            StringBuilder titoloNota = new StringBuilder("#");
            titoloNota.Append(Funzioni.AggiungiZero((cbLibro1.SelectedIndex + 1).ToString(CultureInfo.CurrentCulture), 2));
            if (String.IsNullOrEmpty(txtCapitolo1.Text))
            {
                txtCapitolo1.Text = "0";
                txtVersetto1.Text = "0";
                txtParola1.Text = "0";
            }
            if (String.IsNullOrEmpty(txtVersetto1.Text))
            {
                txtVersetto1.Text = "0";
                txtParola1.Text = "0";
            }
            if (String.IsNullOrEmpty(txtParola1.Text))
                txtParola1.Text = "0";
            titoloNota.Append(Funzioni.AggiungiZero(txtCapitolo1.Text, 3));
            titoloNota.Append(Funzioni.AggiungiZero(txtVersetto1.Text, 3));
            titoloNota.Append(Funzioni.AggiungiZero(txtParola1.Text, 4));
            if (cbBrano.Checked)
            {
                if (cbLibro2.SelectedIndex < cbLibro1.SelectedIndex)
                    cbLibro2.SelectedIndex = cbLibro1.SelectedIndex;
                titoloNota.Append("-").Append(Funzioni.AggiungiZero((cbLibro2.SelectedIndex + 1).ToString(CultureInfo.InvariantCulture), 2));
                if (String.IsNullOrEmpty(txtCapitolo2.Text))
                {
                    txtCapitolo2.Text = "0";
                    txtVersetto2.Text = "0";
                    txtParola2.Text = "0";
                }
                if (String.IsNullOrEmpty(txtVersetto2.Text))
                {
                    txtVersetto2.Text = "0";
                    txtParola2.Text = "0";
                }
                if (String.IsNullOrEmpty(txtParola2.Text))
                    txtParola2.Text = "0";
                if (cbLibro2.SelectedIndex == cbLibro1.SelectedIndex)
                {
                    if (Convert.ToInt32(txtCapitolo2.Text, CultureInfo.CurrentCulture) < Convert.ToInt32(txtCapitolo1.Text, CultureInfo.CurrentCulture))
                        txtCapitolo2.Text = txtCapitolo1.Text;
                    if (txtCapitolo2.Text == txtCapitolo1.Text)
                    {
                        if (Convert.ToInt32(txtVersetto2.Text, CultureInfo.CurrentCulture) < Convert.ToInt32(txtVersetto1.Text, CultureInfo.CurrentCulture))
                            txtVersetto2.Text = txtVersetto1.Text;
                        if (txtVersetto2.Text == txtVersetto1.Text)
                        {
                            if (Convert.ToInt32(txtParola2.Text, CultureInfo.CurrentCulture) < Convert.ToInt32(txtParola1.Text, CultureInfo.CurrentCulture))
                                txtParola2.Text = txtParola1.Text;
                        }
                    }
                }
                titoloNota.Append(Funzioni.AggiungiZero(txtCapitolo2.Text, 3));
                titoloNota.Append(Funzioni.AggiungiZero(txtVersetto2.Text, 3));
                titoloNota.Append(Funzioni.AggiungiZero(txtParola2.Text, 4));
            }
            else
            {
                string titoloStringa = titoloNota.ToString();
                titoloNota.Append("-").Append(titoloStringa.Substring(1));
            }

            riferimento = titoloNota.ToString();

            Close();
        }

    }
}