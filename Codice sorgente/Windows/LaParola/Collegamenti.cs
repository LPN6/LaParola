using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace LaParola
{
    public partial class Collegamenti : Template
    {
        private Principale genitore;
        private List<InfoCollegamento> informazioniCollegamenti = new List<InfoCollegamento>();
        private bool nuovoLink = false;

        public Collegamenti(Principale formGenitore)
        {
            InitializeComponent();

            genitore = formGenitore;
            guidaFile.HelpNamespace = genitore.NomeFileGuida();

            ToolStripMenuItem vocePrincipale = (ToolStripMenuItem)(genitore.externalLinkStripMenuItem);

            lbCollegamenti.BeginUpdate();
            int numeroCollegamenti = vocePrincipale.DropDownItems.Count - 2;
            for (int i = 0; i < numeroCollegamenti; ++i)
            {
                if (((ToolStripMenuItem)(vocePrincipale.DropDownItems[i])).DropDownItems.Count == 0)
                { // è una voce normale, non una categoria
                    lbCollegamenti.Items.Add(vocePrincipale.DropDownItems[i].Text);
                    informazioniCollegamenti.Add((InfoCollegamento)(((ToolStripMenuItem)(vocePrincipale.DropDownItems[i])).Tag));
                }
                foreach (ToolStripItem voceInCategoria in ((ToolStripMenuItem)(vocePrincipale.DropDownItems[i])).DropDownItems)
                {
                    lbCollegamenti.Items.Add(voceInCategoria.Text);
                    informazioniCollegamenti.Add((InfoCollegamento)(voceInCategoria.Tag));
                }
            }
            lbCollegamenti.EndUpdate();
        }

        private void lbCollegamenti_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool voceSelezionata = (lbCollegamenti.SelectedIndex >= 0);
            AttivaCampi(voceSelezionata);

            if (voceSelezionata)
            {
                int indice = lbCollegamenti.SelectedIndex;
                txtNome.Text = lbCollegamenti.SelectedItem.ToString();
                txtDescrizione.Text = informazioniCollegamenti[indice].descrizione;
                txtIndirizzo.Text = informazioniCollegamenti[indice].url;
                txtParametri.Text = informazioniCollegamenti[indice].parametri;
                txtImmagine.Text = informazioniCollegamenti[indice].immagine;
                txtCategoria.Text = informazioniCollegamenti[indice].categoria;
                txtScorciatoia.Text = informazioniCollegamenti[indice].scorciatoia;
                if (informazioniCollegamenti[indice].tipo == CollegamentoTipo.Riferimento)
                {
                    rbTipoRiferimento.Checked = true;
                    etiLingua.Visible = false;
                    txtLingua.Visible = false;
                }
                else
                {
                    rbTipoParola.Checked = true;
                    etiLingua.Visible = true;
                    txtLingua.Visible = true;
                    txtLingua.Text = informazioniCollegamenti[indice].lingua;
                }
            }

            pulCancella.Enabled = voceSelezionata;
            pulSalva.Enabled = false;

            nuovoLink = false;
        }

        private void AttivaCampi(bool voceSelezionata)
        {
            etiNome.Enabled = voceSelezionata;
            txtNome.Enabled = voceSelezionata;
            etiDescrizione.Enabled = voceSelezionata;
            txtDescrizione.Enabled = voceSelezionata;
            gbTipo.Enabled = voceSelezionata;
            etiIndirizzo.Enabled = voceSelezionata;
            txtIndirizzo.Enabled = voceSelezionata;
            etiParametri.Enabled = voceSelezionata;
            txtParametri.Enabled = voceSelezionata;
            etiImmagine.Enabled = voceSelezionata;
            txtImmagine.Enabled = voceSelezionata;
            etiCategoria.Enabled = voceSelezionata;
            txtCategoria.Enabled = voceSelezionata;
            etiScorciatoia.Enabled = voceSelezionata;
            txtScorciatoia.Enabled = voceSelezionata;
        }

        private void txt_TextChanged(object sender, EventArgs e)
        {
            pulSalva.Enabled = (!string.IsNullOrEmpty(txtNome.Text) && !string.IsNullOrEmpty(txtIndirizzo.Text));
        }

        private void rbTipo_CheckedChanged(object sender, EventArgs e)
        {
            txt_TextChanged(sender, e);
            txtLingua.Visible = (rbTipoParola.Checked);
            etiLingua.Visible = (rbTipoParola.Checked);
        }

        private void pulNuovo_Click(object sender, EventArgs e)
        {
            AttivaCampi(true);
            txtNome.Text = txtNome.Tag.ToString();
            txtDescrizione.Text = "";
            rbTipoRiferimento.Checked = true;
            txtLingua.Text = "";
            txtIndirizzo.Text = "";
            txtParametri.Text = "";
            txtImmagine.Text = "";
            txtCategoria.Text = "";
            txtScorciatoia.Text = "";

            pulCancella.Enabled = false;

            nuovoLink = true;
        }

        private void pulSalva_Click(object sender, EventArgs e)
        {
            if (nuovoLink)
            {
                SalvaCollegamentoAttuale(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + "Collegamenti" + Path.DirectorySeparatorChar + txtNome.Text + ".xml");
                lbCollegamenti.SelectedIndex = -1;
                lbCollegamenti_SelectedIndexChanged(sender, e);
            }
            else // link modificato
            {
                string nomeFile = informazioniCollegamenti[lbCollegamenti.SelectedIndex].nomeFile;
                if (CancellaCollegamento(lbCollegamenti.SelectedIndex))
                    SalvaCollegamentoAttuale(nomeFile);
            }
        }

        private void pulCancella_Click(object sender, EventArgs e)
        {
            int indice = lbCollegamenti.SelectedIndex;
            if (MessageBox.Show(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("LinksConfirmDelete"), txtNome.Text), Principale.LocRM.GetString("MiscConfirm"), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions) == DialogResult.Yes)
            {
                CancellaCollegamento(indice);
            }
        }

        private void SalvaCollegamentoAttuale(string nomeFile)
        {
            // trovare un nome che non esiste già per il file
            string nomeFileOriginale = nomeFile;
            int suffiso = 0;
            while (File.Exists(nomeFile))
            {
                ++suffiso;
                nomeFile = Path.GetDirectoryName(nomeFileOriginale) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(nomeFileOriginale) + suffiso.ToString(CultureInfo.InvariantCulture) + Path.GetExtension(nomeFileOriginale);
            }

            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true
            };
            XmlWriter writer = null;
            try
            {
                writer = XmlWriter.Create(nomeFile, settings);
                InfoCollegamento nuovoCollegamento = new InfoCollegamento();
                writer.WriteStartElement("link");
                writer.WriteAttributeString("name", txtNome.Text);
                nuovoCollegamento.nomeFile = nomeFile;
                if (!string.IsNullOrEmpty(txtDescrizione.Text))
                {
                    writer.WriteElementString("description", txtDescrizione.Text);
                    nuovoCollegamento.descrizione = txtDescrizione.Text;
                }
                if (rbTipoRiferimento.Checked)
                {
                    writer.WriteElementString("type", rbTipoRiferimento.Tag.ToString());
                    nuovoCollegamento.tipo = CollegamentoTipo.Riferimento;
                }
                else
                {
                    writer.WriteElementString("type", rbTipoParola.Tag.ToString());
                    nuovoCollegamento.tipo = CollegamentoTipo.Parola;
                    if (!string.IsNullOrEmpty(txtLingua.Text))
                    {
                        writer.WriteElementString("language", txtLingua.Text);
                        nuovoCollegamento.lingua = txtLingua.Text;
                    }
                }
                if (!string.IsNullOrEmpty(txtIndirizzo.Text))
                {
                    writer.WriteElementString("url", txtIndirizzo.Text);
                    nuovoCollegamento.url = txtIndirizzo.Text;
                }
                if (!string.IsNullOrEmpty(txtParametri.Text))
                {
                    writer.WriteElementString("parameters", txtParametri.Text);
                    nuovoCollegamento.parametri = txtParametri.Text;
                }
                if (!string.IsNullOrEmpty(txtImmagine.Text))
                {
                    writer.WriteElementString("image", txtImmagine.Text);
                    nuovoCollegamento.immagine = txtImmagine.Text;
                }
                if (!string.IsNullOrEmpty(txtScorciatoia.Text))
                {
                    writer.WriteElementString("shortcut", txtScorciatoia.Text);
                    nuovoCollegamento.scorciatoia = txtScorciatoia.Text;
                }
                if (!string.IsNullOrEmpty(txtCategoria.Text))
                {
                    writer.WriteElementString("category", txtCategoria.Text);
                    nuovoCollegamento.categoria = txtCategoria.Text;
                }
                writer.WriteEndElement();
                writer.Flush();
                genitore.SetBarraDiStatoTesto(Path.GetFileName(nomeFile) + " " + Principale.LocRM.GetString("LinksWritten") + ".");
                lbCollegamenti.Items.Add(txtNome.Text);
                informazioniCollegamenti.Add(nuovoCollegamento);
            }
            catch (Exception exc)
            {
                MessageBox.Show(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("LinksErrorWriting"), nomeFile, exc.Message), Principale.LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        private bool CancellaCollegamento(int indice)
        {
            bool successo = true;
            try
            {
                File.Delete(informazioniCollegamenti[indice].nomeFile);
                lbCollegamenti.SelectedIndex = -1;
                lbCollegamenti.Items.RemoveAt(indice);
                informazioniCollegamenti.RemoveAt(indice);
            }
            catch (Exception exc)
            {
                MessageBox.Show(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("LinksErrorDelete"), informazioniCollegamenti[indice].nomeFile, exc.Message), Principale.LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions);
                successo = false;
            }
            return successo;
        }

        private void btnCanc_Click(object sender, EventArgs e)
        {
            genitore.CreaMenuCollegamenti();
            Close();
        }
    }
}
