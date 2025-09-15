using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LaParola
{
    public partial class TrovaSostituisci : Form
    {

        #region proprietà

        private int pulsanteCliccato;
        public int PulsanteCliccato
        {
            get { return pulsanteCliccato; }
        }

        public string TestoTrova
        {
            get { return txtTrova.Text; }
            set { txtTrova.Text = value; }
        }

        public string TestoSostituisci
        {
            get { return txtSostituisci.Text; }
            set { txtSostituisci.Text = value; }
        }

        public RichTextBoxFinds Opzioni
        {
            get
            {
                RichTextBoxFinds opzioniDaRestituire = RichTextBoxFinds.None;
                if (cbParoleIntere.Checked)
                    opzioniDaRestituire |= RichTextBoxFinds.WholeWord;
                if (cbMaiuscolo.Checked)
                    opzioniDaRestituire |= RichTextBoxFinds.MatchCase;
                return opzioniDaRestituire;
            }
            set
            {
                cbParoleIntere.Checked = ((value & RichTextBoxFinds.WholeWord) == RichTextBoxFinds.WholeWord);
                cbMaiuscolo.Checked = ((value & RichTextBoxFinds.MatchCase) == RichTextBoxFinds.MatchCase);
            }
        }

        #endregion

        public TrovaSostituisci(Principale genitore, bool trova)
        {
            if (genitore == null)
                throw new ArgumentNullException("genitore");

            InitializeComponent();
            guidaFile.HelpNamespace = genitore.NomeFileGuida();

            String[] titolo = Text.Split(new char[] {'|'});
            if (trova)
            {
                Text = titolo[0];
                etiSostituisci.Visible = false;
                txtSostituisci.Visible = false;
                pulSostituisci.Visible = false;
                pulSostituisciTutto.Visible = false;
            }
            else
            {
                Text = titolo[1];
                AcceptButton = pulSostituisci;
            }
        }

        private void txtTrova_TextChanged(object sender, EventArgs e)
        {
            pulTrova.Enabled = !String.IsNullOrEmpty(txtTrova.Text);
            pulSostituisci.Enabled = !String.IsNullOrEmpty(txtTrova.Text) && !String.IsNullOrEmpty(txtSostituisci.Text);
            pulSostituisciTutto.Enabled = !String.IsNullOrEmpty(txtTrova.Text) && !String.IsNullOrEmpty(txtSostituisci.Text);
        }

        private void txtSostituisci_TextChanged(object sender, EventArgs e)
        {
            pulSostituisci.Enabled = !String.IsNullOrEmpty(txtTrova.Text) && !String.IsNullOrEmpty(txtSostituisci.Text);
            pulSostituisciTutto.Enabled = !String.IsNullOrEmpty(txtTrova.Text) && !String.IsNullOrEmpty(txtSostituisci.Text);
        }

        private void pulTrova_Click(object sender, EventArgs e)
        {
            pulsanteCliccato = 1;
            Close();
        }

        private void pulSostituisci_Click(object sender, EventArgs e)
        {
            pulsanteCliccato = 2;
            Close();
        }

        private void pulSostituisciTutto_Click(object sender, EventArgs e)
        {
            pulsanteCliccato = 3;
            Close();
        }
    }
}