using System;
using System.Reflection;
using System.Windows.Forms;
using LaParola_Screensaver.Properties;

namespace LaParola_Screensaver
{
    static class Program
    {
        private static IntPtr parentHwnd = IntPtr.Zero;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (Settings.Default.ScreensaverNuovaVersione)
            {
                Settings.Default.Upgrade();
                Settings.Default.ScreensaverNuovaVersione = false;
            }

            string[] cmdList = Environment.GetCommandLineArgs();
            if (cmdList.Length >= 2)
            {
                if (cmdList[1].IndexOf("/c", StringComparison.OrdinalIgnoreCase) >= 0)
                {        // Configuration mode
                    Application.Run(new Opzioni());
                    Settings.Default.Save();
                    return;
                }
                else if (cmdList[1].IndexOf("/p", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    // Handle alla finestra di dialogo dello SS è il prossimo parametro
                    if (cmdList.Length >= 3) // altrimenti sarà eseguito a tutto schermo
                        parentHwnd = (IntPtr)uint.Parse(cmdList[2], System.Globalization.CultureInfo.InvariantCulture);
                }
            }

            try
            {
                Application.Run(new Principale(parentHwnd));
            }
            catch (System.IO.FileNotFoundException)
            {
                string messaggio, titolo;
                string versione = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                if (Settings.Default.InterfacciaLingua.Length >= 2 && Settings.Default.InterfacciaLingua.ToUpperInvariant().Substring(0, 2) == "IT")
                {
                    titolo = "Errore";
                    messaggio = "La versione più recente del programma LaParola deve essere installata e eseguita almeno una volta affinché questo salvaschermo funzioni. Il programma può essere scaricato da http://www.laparola.net/programma/windows.php";
                }
                else
                {
                    titolo = "Error";
                    messaggio = "The most recent version of the LaParola program must be installed and run at least once before this screensaver will work. The program can be downloaded from http://www.laparola.net/program/";
                }
                MessageBox.Show(messaggio, titolo);
                return;
            }
        }
    }
}
