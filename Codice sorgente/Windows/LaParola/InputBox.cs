using System;
using System.Windows.Forms;

namespace LaParola
{
    public partial class InputBox : Form
    {
        private string risposta;
        public string Risposta
        {
            get { return risposta; }
        }

        /// <summary>
        /// Mostra una finestra per ottiene una stringa.
        /// </summary>
        /// <param name="titolo">Il titolo della finestra.</param>
        /// <param name="domanda">La domanda posta nella finestra.</param>
        /// <param name="rispostaSuggerita">Il valore predefinito.</param>
        public InputBox(string titolo, string domanda, string rispostaSuggerita)
        {
            risposta = "";
            InitializeComponent();
            this.Text = titolo;
            etiDomanda.Text = "&" + domanda;
            tbRisposta.Text = rispostaSuggerita;
        }

        private void pulOK_Click(object sender, EventArgs e)
        {
            risposta = tbRisposta.Text;
        }
    }
}