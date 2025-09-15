using System;
using System.Text;
using System.Windows.Forms;
using LaParola.Properties;
using TestiBiblici;

namespace LaParola
{
    public partial class BraniSimili : Template
    {
        private readonly Principale genitore;

        public BraniSimili(Principale formGenitore)
        {
            InitializeComponent();
            genitore = formGenitore;
        }

        private void BraniSimili_Load(object sender, EventArgs e)
        {
            this.ActiveControl = cbBrano;
            string versionePrecedente = Settings.Default.SimiliVersione;

            guidaFile.HelpNamespace = genitore.NomeFileGuida();

            cbVersione.BeginUpdate();
            foreach (string s in Principale.testi.NomiVersioni(TestoTipi.Bibbia))
            {
                cbVersione.Items.Add(s);
                if (s == versionePrecedente)
                    cbVersione.SelectedIndex = cbVersione.Items.Count - 1;
            }

            if (cbVersione.Items.Count > 0)
            {
                if (cbVersione.SelectedIndex < 0)
                    cbVersione.SelectedIndex = 0;
            }
            else
            {
                btnOK.Visible = false;
            }
            cbVersione.EndUpdate();

            if (Settings.Default.SimiliVersetti)
                cbVersettiCapitoli.SelectedIndex = 0;
            else
                cbVersettiCapitoli.SelectedIndex = 1;
            udNumeroMassimo.Value = Settings.Default.SimiliNumeroMassimo;

            cbBrano.Items.AddRange(Settings.Default.SimiliBraniPrecedenti.Split(new char[] { '§' }, StringSplitOptions.RemoveEmptyEntries));
        }

        private void BraniSimili_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ((e.CloseReason != CloseReason.WindowsShutDown) && (e.CloseReason != CloseReason.TaskManagerClosing))
            {
                Settings.Default.SimiliVersione = cbVersione.SelectedItem.ToString();

                int nBraniDaSalvare = Settings.Default.MiscRichiesteMemorizzate;
                if (cbBrano.Items.Count < nBraniDaSalvare)
                    nBraniDaSalvare = cbBrano.Items.Count;
                StringBuilder braniDaSalvare = new StringBuilder("");
                for (int i = 0; i < nBraniDaSalvare; ++i)
                    braniDaSalvare.Append("§").Append(cbBrano.Items[i]);
                Settings.Default.SimiliBraniPrecedenti = braniDaSalvare.ToString();

                Settings.Default.SimiliVersetti = (cbVersettiCapitoli.SelectedIndex == 0);
                Settings.Default.SimiliNumeroMassimo = (int)(udNumeroMassimo.Value);
            }
        }

        private void cbBrano_TextChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = (!String.IsNullOrEmpty(cbBrano.Text));
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string brano = cbBrano.Text;

            genitore.TrovaBraniSimili(brano, cbVersione.Text, cbVersettiCapitoli.SelectedIndex == 0, (int)(udNumeroMassimo.Value));

            if (cbBrano.Items.IndexOf(brano) > -1)
                cbBrano.Items.Remove(brano);
            cbBrano.Items.Insert(0, brano);
            cbBrano.AutoCompleteMode = AutoCompleteMode.None;
            // per costringere il combo box a rileggere i valori che possono essere usati per l'autocomplete
            cbBrano.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cbBrano.Text = brano;
            cbBrano.SelectAll();
        }

        private void btnCanc_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
