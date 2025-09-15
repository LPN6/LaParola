using System.Reflection;
using System.Windows.Forms;

namespace LaParola
{
    public partial class SplashScreen : Form
    {
        public SplashScreen()
        {
            try
            {
                InitializeComponent();
            }
            catch
            {
                // con un computer caricare le risorse ha dato un errore in .NET e il programma non partiva
                // non so perché, ma se si salta la riga il programma funziona comunque (forse senza SplashScreen)
            }
            string versione = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            labVersione.Text = Funzioni.VersioneMinore2Cifre(versione.Remove(versione.LastIndexOf('.')));
        }
    }
}