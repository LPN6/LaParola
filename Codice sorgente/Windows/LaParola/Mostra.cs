using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Forms;
using LaParola.Properties;
using TestiBiblici;

namespace LaParola
{
    public partial class Mostra : Template
    {
        private Principale genitore;

        public Mostra(Principale formGenitore)
        {
            InitializeComponent();
            genitore = formGenitore;
        }

        private void Mostra_Load(object sender, EventArgs e)
        {
            guidaFile.HelpNamespace = genitore.NomeFileGuida();

            this.ActiveControl = cbBrano;
            cbAlternare.Checked = Settings.Default.MostraAlterna;
            cbDefinizioni.Checked = Settings.Default.MostraDefinizioni;
            string listaVersioni = Settings.Default.MostraVersioni;
            if (Settings.Default.MostraAltezza>0)
                Height = Settings.Default.MostraAltezza;

            string[] arrayVersioni = listaVersioni.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries);
            string listaVersioniScelte = Settings.Default.MostraVersioniScelte;
            string[] arrayVersioniScelte = listaVersioniScelte.Split(new char[] { '|' }, System.StringSplitOptions.RemoveEmptyEntries);
            clbVersioni.BeginUpdate();
            foreach (string s in arrayVersioni)
            {
                if (((Principale.testi.Info(s).Tipo & TestoTipi.Bibbia) == TestoTipi.Bibbia) || ((Principale.testi.Info(s).Tipo & TestoTipi.Commentario) == TestoTipi.Commentario))
                    clbVersioni.Items.Add(s, (Array.IndexOf(arrayVersioniScelte, s) >= 0));
            }
            foreach (string s in Principale.testi.NomiVersioni(TestoTipi.Bibbia))
            {
                if (Array.IndexOf(arrayVersioni, s) < 0)
                    clbVersioni.Items.Add(s);
            }
            foreach (string s in Principale.testi.NomiVersioni(TestoTipi.Commentario))
            {
                if (Array.IndexOf(arrayVersioni, s) < 0)
                    clbVersioni.Items.Add(s);
            }

            if (clbVersioni.Items.Count > 0)
            {
                if (clbVersioni.CheckedIndices.Count == 0)
                {
                    clbVersioni.SetItemChecked(0, true);
                }
            }
            else
            {
                btnOK.Visible = false;
            }
            clbVersioni.EndUpdate();

            cbBrano.Items.AddRange(Settings.Default.MostraBraniPrecedenti.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Collection<string> versioni = new Collection<string>();
            foreach (object itemChecked in clbVersioni.CheckedItems)
                versioni.Add(itemChecked.ToString());

            string branoDaMostrare = cbBrano.Text;

            genitore.MostraBranoInEditor(branoDaMostrare, versioni, cbAlternare.Enabled & cbAlternare.Checked);

            Application.DoEvents();
            if (cbDefinizioni.Enabled & cbDefinizioni.Checked)
                genitore.MostraDefinizioniInEditor(branoDaMostrare, clbVersioni.CheckedItems[0].ToString());
            Application.DoEvents();

            if (cbBrano.Items.IndexOf(branoDaMostrare) > -1)
                cbBrano.Items.Remove(branoDaMostrare);
            cbBrano.Items.Insert(0, branoDaMostrare);
            //            cbBrano.Text = "";
            //            btnOK.Enabled = false;
            cbBrano.AutoCompleteMode = AutoCompleteMode.None;
            // per costringere il combo box a rileggere i valori che possono essere usati per l'autocomplete
            cbBrano.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cbBrano.Text = branoDaMostrare;
            cbBrano.SelectAll();
        }

        private void btnCanc_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Mostra_Resize(object sender, EventArgs e)
        {
            clbVersioni.Height = Height - 206;
        }

        private void Mostra_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ((e.CloseReason != CloseReason.WindowsShutDown) && (e.CloseReason != CloseReason.TaskManagerClosing))
            {
                StringBuilder listaVersioni = new StringBuilder("");
                for (int i = 0; i < clbVersioni.Items.Count; i++)
                    listaVersioni.Append(clbVersioni.Items[i]).Append("|");
                Settings.Default.MostraVersioni = listaVersioni.ToString();

                listaVersioni.Remove(0, listaVersioni.Length); ;
                for (int i = 0; i < clbVersioni.CheckedItems.Count; i++)
                    listaVersioni.Append(clbVersioni.CheckedItems[i]).Append("|");
                Settings.Default.MostraVersioniScelte = listaVersioni.ToString();

                int nBraniDaSalvare = Settings.Default.MiscRichiesteMemorizzate;
                if (cbBrano.Items.Count < nBraniDaSalvare)
                    nBraniDaSalvare = cbBrano.Items.Count;
                StringBuilder braniDaSalvare = new StringBuilder("");
                for (int i = 0; i < nBraniDaSalvare; ++i)
                    braniDaSalvare.Append("|").Append(cbBrano.Items[i]);
                Settings.Default.MostraBraniPrecedenti = braniDaSalvare.ToString();

                Settings.Default.MostraAlterna = cbAlternare.Checked;
                Settings.Default.MostraDefinizioni = cbDefinizioni.Checked;
                Settings.Default.MostraAltezza = Height;
            }
        }

        private void btnSu_Click(object sender, EventArgs e)
        {
            int si = clbVersioni.SelectedIndex;
            if (si > 0)
            {
                clbVersioni.BeginUpdate();
                clbVersioni.Items.Insert(si - 1, clbVersioni.SelectedItem);
                clbVersioni.SetItemChecked(si - 1, clbVersioni.GetItemChecked(si + 1));
                clbVersioni.Items.RemoveAt(si + 1);
                clbVersioni.SelectedIndex = si - 1;
                clbVersioni.EndUpdate();
            }
        }

        private void btnGiu_Click(object sender, EventArgs e)
        {
            int si = clbVersioni.SelectedIndex;
            if (si < clbVersioni.Items.Count - 1)
            {
                clbVersioni.BeginUpdate();
                clbVersioni.Items.Insert(si + 2, clbVersioni.SelectedItem);
                clbVersioni.SetItemChecked(si + 2, clbVersioni.GetItemChecked(si));
                clbVersioni.Items.RemoveAt(si);
                clbVersioni.SelectedIndex = si + 1;
                clbVersioni.EndUpdate();
            }
        }

        private void clbVersioni_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSu.Enabled = (clbVersioni.SelectedIndex > 0);
            btnGiu.Enabled = (clbVersioni.SelectedIndex >= 0 && clbVersioni.SelectedIndex < clbVersioni.Items.Count - 1);
            cbDefinizioni.Enabled = (clbVersioni.CheckedItems.Count > 0 && !string.IsNullOrEmpty(Funzioni.DizionarioDiVersione(clbVersioni.CheckedItems[0].ToString())));
        }

        private void clbVersioni_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.CurrentValue == CheckState.Checked && clbVersioni.CheckedItems.Count == 1)
                btnOK.Enabled = false;
            else
                btnOK.Enabled = (!String.IsNullOrEmpty(cbBrano.Text));
            cbAlternare.Enabled = (clbVersioni.CheckedItems.Count + (e.NewValue == CheckState.Checked ? 1 : -1) >= 2);
        }

        private void cbBrano_TextChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = (!String.IsNullOrEmpty(cbBrano.Text) && clbVersioni.CheckedItems.Count > 0);
        }

        private void btnSelezionaTutte_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbVersioni.Items.Count; ++i)
                clbVersioni.SetItemChecked(i, true);
        }

        private void btnDeselezionaTutte_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < clbVersioni.Items.Count; ++i)
                clbVersioni.SetItemChecked(i, false);
        }

    }
}