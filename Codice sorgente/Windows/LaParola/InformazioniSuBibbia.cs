using System;
using System.Drawing;
using System.Windows.Forms;
using TestiBiblici;

namespace LaParola
{
    public partial class InformazioniSuBibbia : Template
    {
        public InformazioniSuBibbia(string nomeVersione)
        {
            InitializeComponent();
            Text = nomeVersione;
            VersioneInformazioni vi = Principale.testi.Info(nomeVersione);
            if (!String.IsNullOrEmpty(vi.Nome))
            {
                Text = vi.Nome;
                labTitolo.Text = vi.Titolo;
                labAutore.Text = vi.Autore;
                labCasaEditrice.Text = vi.CasaEditrice;
                labData.Text = vi.Data;
                labISBN.Text = (String.IsNullOrEmpty(vi.Isbn)) ? "" : "ISBN " + vi.Isbn;
                labCopyright.Text = vi.Copyright;
                try
                {
                    rtAltreInfo.Rtf = vi.Descrizione;
                }
                catch (ArgumentException)
                {
                    rtAltreInfo.Text = vi.Descrizione;
                }
            }
            else
            {
                Text = "La Parola";
                labTitolo.Text = "";
                labAutore.Text = "";
                labCasaEditrice.Text = "";
                labData.Text = "";
                labISBN.Text = "";
                labCopyright.Text = "";
            }
        }

        private void InformazioniSuBibbia_Resize(object sender, EventArgs e)
        {
            rtAltreInfo.Size = new Size(Width - 38, Height - 192);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void rtAltreInfo_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Funzioni.ApriBrowser(e.LinkText, false);
        }
    }
}