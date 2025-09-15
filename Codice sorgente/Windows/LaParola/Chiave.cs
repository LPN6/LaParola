using System;
using System.Text;
using System.Windows.Forms;
using LaParola.Properties;
using TestiBiblici;

namespace LaParola
{
    // "Con Ipertesto" rende la chiave di tutta la Bibbia troppo lenta, quando disabilitato

    struct ParolaApparenze
    {
        /// <summary>
        /// La parola.
        /// </summary>
        public string Parola;
        /// <summary>
        /// Le apparenze della parola.
        /// </summary>
        public Riferimento Apparenze;
    }

    public partial class Chiave : Template
    {
        private Principale genitore;
        string dizionario = "";

        public Chiave(Principale formGenitore)
        {
            InitializeComponent();
            genitore = formGenitore;
        }

        private void Chiave_Load(object sender, EventArgs e)
        {
            this.ActiveControl = cbBrano;
            string versionePrecedente = Settings.Default.ChiaveVersione;

            guidaFile.HelpNamespace = genitore.NomeFileGuida();

            cbVersione.BeginUpdate();
            foreach (string s in Principale.testi.NomiVersioni())
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

            cbIpertesto.Checked = Settings.Default.ChiaveIpertesto;
            cbDefinizioni.Checked = Settings.Default.ChiaveDefinizioni;
            cbNonRadiciComuni.Checked = Settings.Default.ChiaveNonRadiciComuni;
            string[] radiciComuni = Settings.Default.ChiaveRadiciComuni.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            string radiciPerTB = "";
            foreach (string radiceComune in radiciComuni)
                radiciPerTB += radiceComune + "\r\n";
            tbNonRadiciComuni.Text = radiciPerTB;
            switch (Settings.Default.ChiaveParoleRadici)
            {
                case 1:
                    rbRadici.Checked = true;
                    break;
                default: // incluso case 0: che è normale
                    rbParole.Checked = true;
                    break;
            }
            switch (Settings.Default.ChiaveOrdine)
            {
                case 1:
                    rbApparenze.Checked = true;
                    break;
                case 2:
                    rbPrimaApparenza.Checked = true;
                    break;
                default: // incluso case 0: che è normale
                    rbAlfabetico.Checked = true;
                    break;
            }
            udNumeroMinimo.Value = Settings.Default.ChiaveNumeroMinimo;
            cbRiferimenti.Checked = Settings.Default.ChiaveConRiferimenti;

            cbBrano.Items.AddRange(Settings.Default.ChiaveBraniPrecedenti.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
        }

        private void Chiave_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ((e.CloseReason != CloseReason.WindowsShutDown) && (e.CloseReason != CloseReason.TaskManagerClosing))
            {
                Settings.Default.ChiaveVersione = cbVersione.SelectedItem.ToString();
                Settings.Default.ChiaveIpertesto = cbIpertesto.Checked;
                Settings.Default.ChiaveDefinizioni = cbDefinizioni.Checked;
                Settings.Default.ChiaveNonRadiciComuni = cbNonRadiciComuni.Checked;
                string[] radiciComuni = tbNonRadiciComuni.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                StringBuilder listaRadici = new StringBuilder("");
                foreach (string radiceComune in radiciComuni)
                    listaRadici.Append(radiceComune).Append("|");
                Settings.Default.ChiaveRadiciComuni = listaRadici.ToString();
                Settings.Default.ChiaveNumeroMinimo = (int)(udNumeroMinimo.Value);
                Settings.Default.ChiaveConRiferimenti = cbRiferimenti.Checked;
                if (rbRadici.Checked)
                    Settings.Default.ChiaveParoleRadici = 1;
                else
                    Settings.Default.ChiaveParoleRadici = 0;
                if (rbPrimaApparenza.Checked)
                    Settings.Default.ChiaveOrdine = 2;
                else if (rbApparenze.Checked)
                    Settings.Default.ChiaveOrdine = 1;
                else
                    Settings.Default.ChiaveOrdine = 0;

                int nBraniDaSalvare = Settings.Default.MiscRichiesteMemorizzate;
                if (cbBrano.Items.Count < nBraniDaSalvare)
                    nBraniDaSalvare = cbBrano.Items.Count;
                StringBuilder braniDaSalvare = new StringBuilder("");
                for (int i = 0; i < nBraniDaSalvare; ++i)
                    braniDaSalvare.Append("|").Append(cbBrano.Items[i]);
                Settings.Default.ChiaveBraniPrecedenti = braniDaSalvare.ToString();
            }

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string brano = cbBrano.Text;
            string versione = cbVersione.SelectedItem.ToString();

            if (gbParoleRadici.Visible == false)
            {
                rbParole.Checked = true;
                cbNonRadiciComuni.Checked = false;
            }

            int ordine = 0;
            if (rbApparenze.Checked)
                ordine = 1;
            if (rbPrimaApparenza.Checked)
                ordine = 2;

            genitore.ChiaveInEditor(brano, versione, rbParole.Checked, cbNonRadiciComuni.Checked, tbNonRadiciComuni.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries), ordine, (int)(udNumeroMinimo.Value), cbRiferimenti.Checked, (cbDefinizioni.Checked & cbDefinizioni.Enabled) ? dizionario : "");

            if (!string.IsNullOrEmpty(brano))
            {
                if (cbBrano.Items.IndexOf(brano) > -1)
                    cbBrano.Items.Remove(brano);
                cbBrano.Items.Insert(0, brano);
                //                cbBrano.Text = "";
                cbBrano.AutoCompleteMode = AutoCompleteMode.None;
                // per costringere il combo box a rileggere i valori che possono essere usati per l'autocomplete
                cbBrano.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cbBrano.Text = brano;
                cbBrano.SelectAll();
            }
        }

        private void btnCanc_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cbNonRadiciComuni_CheckedChanged(object sender, EventArgs e)
        {
            tbNonRadiciComuni.Enabled = cbNonRadiciComuni.Checked;
        }

        private void cbVersione_SelectedIndexChanged(object sender, EventArgs e)
        {
            gbParoleRadici.Visible = Principale.testi.EsistonoRadici(cbVersione.Text);
            cbNonRadiciComuni.Visible = gbParoleRadici.Visible;
            tbNonRadiciComuni.Visible = gbParoleRadici.Visible;

            dizionario = Funzioni.DizionarioDiVersione(cbVersione.Text);
            string stringaBase = cbDefinizioni.Text;
            if (stringaBase.IndexOf(" (") > -1)
                stringaBase = stringaBase.Remove(stringaBase.IndexOf(" ("));
            if (!string.IsNullOrEmpty(dizionario))
                cbDefinizioni.Text = stringaBase + " (" + dizionario + ")";
            else
                cbDefinizioni.Text = stringaBase;
            cbDefinizioni.Enabled = !string.IsNullOrEmpty(dizionario);
        }
    }
}