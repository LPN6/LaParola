using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Text;
using System.Windows.Forms;
using Light.Properties;
using TestiBiblici;

[assembly: CLSCompliant(true)]
namespace Light
{
    public partial class Principale : Form
    {
        internal static Texts testi;
        private const int ComandiMemorizzati = 5;
        private float cbFontSize;
        private string cbFontName;
        internal static ResourceManager LocRM = new ResourceManager("Light.LightRisorse", typeof(Principale).Assembly);
        enum PostoOutput
        {
            finestra, clipboard, rtf
        }
        private PostoOutput output = PostoOutput.finestra;
        private string cartellaDati = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar;
        private PageSettings storedPageSettings = null;
        private bool riferimentoContestoRicercheVecchio;
        internal static bool isRunningOnMono;

        public Principale()
        {
            if (Settings.Default.LightNuovaVersione)
            {
                Settings.Default.Upgrade();
                Settings.Default.LightNuovaVersione = false;
            }

            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");

            InitializeComponent();

            etiEspressione.Text = etiEspressione.Text.Replace("%", "\n    ");

            cbFontSize = cbVersione.Font.Size;
            cbFontName = cbVersione.Font.Name;

            // una volta, c'è un errore in FormClosing quando cercava i file RTF da cancellare,
            // che cartellaDati non esisteva. Non so perché, non doveva essere possibile,
            // ma questa riga crea la cartella, caso mai
            Directory.CreateDirectory(cartellaDati);

            testi = new Texts(cartellaDati);
            testi.AggiungiDirectory(Application.StartupPath);

            cbEspressione.Items.AddRange(Settings.Default.LightComandiPrecedenti.Split(new char[] { '§' }, StringSplitOptions.RemoveEmptyEntries));

            riferimentoContestoRicercheVecchio = testi.Formato.RiferimentoContestoRicerche;
            testi.Formato.RiferimentoContestoRicerche = false;

            string versionePrecedente = Settings.Default.LightVersione;
            if (string.IsNullOrEmpty(versionePrecedente))
                versionePrecedente = testi.UltimaBibbia;
            if (string.IsNullOrEmpty(versionePrecedente))
                versionePrecedente = testi.UltimaBibbiaCompleta;

            cbVersione.BeginUpdate();
            foreach (string s in testi.NomiVersioni(TestoTipi.Bibbia))
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
                pulEsegui.Visible = false;
            }
            cbVersione.EndUpdate();

            isRunningOnMono = (Type.GetType("Mono.Runtime") != null);

            string[] argomenti = Environment.GetCommandLineArgs();
            bool eseguiSubito = false;
            for (int i = 1; i < argomenti.Length; ++i) // da 1, perché argomenti[0] contiene il nome del file eseguibile
            {
                if (argomenti[i].StartsWith("-v", StringComparison.OrdinalIgnoreCase) || argomenti[i].StartsWith("-t", StringComparison.OrdinalIgnoreCase))
                {
                    string versione = argomenti[i].Remove(0, 2);
                    if (cbVersione.Items.IndexOf(versione) >= 0)
                        cbVersione.SelectedIndex = cbVersione.Items.IndexOf(versione);
                }
                else if (argomenti[i].StartsWith("-c", StringComparison.OrdinalIgnoreCase))
                {
                    output = PostoOutput.clipboard;
                    WindowState = FormWindowState.Minimized;
                    ShowInTaskbar = false;
                    Visible = false;
                }
                else if (argomenti[i].StartsWith("-r", StringComparison.OrdinalIgnoreCase) || argomenti[i].StartsWith("-w", StringComparison.OrdinalIgnoreCase))
                {
                    output = PostoOutput.rtf;
                    WindowState = FormWindowState.Minimized;
                    ShowInTaskbar = false;
                    Visible = false;
                }
                else
                {
                    cbEspressione.Text = argomenti[i];
                    eseguiSubito = true;
                }
            }

            if (eseguiSubito)
                pulEsegui_Click(null, null);
            else
            { // possono diventare falsi se programma è chiamato con argomento -c o -r, ma senza l'espressione da usare
                ShowInTaskbar = true;
                Visible = true;
                output = PostoOutput.finestra;
            }
        }

        private void Principale_Shown(object sender, EventArgs e)
        {
            if (output != PostoOutput.finestra)
                pulChiudi_Click(sender, e);
            else
            {
                WindowState = Settings.Default.LightWindowState;
                if (WindowState != FormWindowState.Maximized)
                {
                    // necessario mettere i valori predefiniti qui invece del file settings.settings
                    // perché Mono 1.2.4 non può convertire il valore 300, 300 ad un Size
                    try
                    {
                        if (Settings.Default.LightWindowSize == null)
                            Size = new Size(300, 300);
                        else
                            Size = Settings.Default.LightWindowSize;
                    }
                    catch (NullReferenceException)
                    {
                        Size = new Size(300, 300);
                    }

                    try
                    {
                        if (Settings.Default.LightWindowLocation == null)
                            Location = new Point(300, 300);
                        else
                            Location = Settings.Default.LightWindowLocation;
                    }
                    catch (NullReferenceException)
                    {
                        Location = new Point(300, 300);
                    }
                }
            }
        }

        private void Principale_Resize(object sender, EventArgs e)
        {
            cbEspressione.Width = Width - 33;
            cbVersione.Width = Width - 33;
        }

        private void Principale_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.LightVersione = cbVersione.SelectedItem.ToString();

            testi.Formato.RiferimentoContestoRicerche = riferimentoContestoRicercheVecchio;

            int nRicercheDaSalvare = ComandiMemorizzati;
            if (cbEspressione.Items.Count < nRicercheDaSalvare)
                nRicercheDaSalvare = cbEspressione.Items.Count;
            StringBuilder ricercheDaSalvare = new StringBuilder("");
            for (int i = 0; i < nRicercheDaSalvare; ++i)
                ricercheDaSalvare.Append("§").Append(cbEspressione.Items[i]);
            Settings.Default.LightComandiPrecedenti = ricercheDaSalvare.ToString();

            if (output == PostoOutput.finestra)
            {
                Settings.Default.LightWindowState = WindowState;
                if (WindowState != FormWindowState.Maximized)
                {
                    Settings.Default.LightWindowSize = Size;
                    Settings.Default.LightWindowLocation = Location;
                }
            }

            Settings.Default.Save();

            string[] fileDaCancellare = Directory.GetFiles(cartellaDati, "light*.rtf");
            for (int i = 0; i < fileDaCancellare.Length; ++i)
            {
                try
                {
                    File.Delete(fileDaCancellare[i]);
                }
                catch
                {
                    // probabilmente il file è ancora aperto; saltiamo e sarà cancellato la prossima volta
                }
            }
        }

        private void cbEspressione_TextChanged(object sender, EventArgs e)
        {
            pulEsegui.Enabled = (!String.IsNullOrEmpty(cbEspressione.Text));
        }

        private void cbEspressione_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbEspressione_TextChanged(sender, e);
        }

        private void cbVersione_SelectedIndexChanged(object sender, EventArgs e)
        {
            string nomeVersione = cbVersione.SelectedItem.ToString();

            TestoTipi tipo = testi.Info(nomeVersione).Tipo;

            switch (LinguaPrincipale(testi.Info(nomeVersione).Lingua))
            {
                case "HE":
                case "HE-T":
                    try
                    {
                        cbEspressione.Font = new Font(testi.Formato.FontEbraicoNome, testi.Formato.FontEbraicoDimensione * cbFontSize / testi.Formato.FontDimensione);
                    }
                    catch (ArgumentException)
                    {
                        //
                    }
                    break;
                case "EL":
                    try
                    {
                        cbEspressione.Font = new Font(testi.Formato.FontGrecoNome, testi.Formato.FontGrecoDimensione * cbFontSize / testi.Formato.FontDimensione);
                    }
                    catch (ArgumentException)
                    {
                        //
                    }
                    break;
                default:
                    {
                        try
                        {
                            cbEspressione.Font = new Font(cbFontName, cbFontSize);
                        }
                        catch (ArgumentException)
                        {
                            //
                        }
                        break;
                    }
            }
        }

        private void pulEsegui_Click(object sender, EventArgs e)
        {
            string espressione = cbEspressione.Text;
            string versione = cbVersione.SelectedItem.ToString();
            string testo = "";

            if (espressione[espressione.Length - 1] >= '0' && espressione[espressione.Length - 1] <= '9')
            {
                // mostra
                // Passage è usato invece di TestoBrano, perché rimuovi i caratteri nascosti, che non servono qui
                testo = testi.Passage(espressione, versione);
            }
            else
            {
                // ricerca
                try
                {
                    Riferimento versetti = testi.Ricerca(espressione, versione);
                    if (versetti.Count > 0)
                        testo = testi.TestoBrano(versetti, versione);
                    else
                        testo = testi.RtfIntestazione() + String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("SearchNoVerses"), espressione, versione) + "}";
                }
                catch (SearchExpressionEmptyException)
                {
                    testo = testi.RtfIntestazione() + LocRM.GetString("SearchExpressionEmpty") + "}";
                }
                catch (SearchSyntaxErrorException ex)
                {
                    testo = testi.RtfIntestazione() + String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("SearchSyntax"), ex.Message) + "}";
                }
                catch (SearchParenthesesException)
                {
                    testo = testi.RtfIntestazione() + LocRM.GetString("SearchBrackets") + "}";
                }
                catch (SearchBracketsException)
                {
                    testo = testi.RtfIntestazione() + LocRM.GetString("SearchSquareBrackets") + "}";
                }
            }

            Application.DoEvents();
            rtTesto.BloccaRtf(true);
            rtTesto.Rtf = testo;
            if (string.IsNullOrEmpty(rtTesto.Text)) // succede in mostra quando il riferimento non è riconosciuto
                rtTesto.Rtf = testi.RtfIntestazione() + String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("ShowNoText"), espressione, versione) + "}";
            Application.DoEvents();
            rtTesto.BloccaRtf(false);

            if (cbEspressione.Items.IndexOf(espressione) > -1)
                cbEspressione.Items.Remove(espressione);
            cbEspressione.Items.Insert(0, espressione);
            cbEspressione.AutoCompleteMode = AutoCompleteMode.None;
            cbEspressione.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cbEspressione.Text = espressione;
            cbEspressione.SelectAll();

            switch (output)
            {
                case PostoOutput.finestra:
                    break;
                case PostoOutput.clipboard:
                    rtTesto.SelectAll();
                    rtTesto.CopiaSenzaTestoNascosto();
                    break;
                case PostoOutput.rtf:
                    string nomeFile = cartellaDati + "light.rtf";
                    if (File.Exists(nomeFile))
                    {
                        int nFile = 1;
                        while (File.Exists(cartellaDati + "light" + nFile.ToString(CultureInfo.InvariantCulture) + ".rtf"))
                            ++nFile;
                        nomeFile = cartellaDati + "light" + nFile.ToString(CultureInfo.InvariantCulture) + ".rtf";
                    }
                    File.WriteAllText(nomeFile, testo, Encoding.ASCII);
                    System.Diagnostics.Process.Start(nomeFile);
                    break;
            }
        }

        private void pulGuida_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, fileGuida.HelpNamespace, HelpNavigator.Topic, "addinslight.html");
        }

        private void pulChiudi_Click(object sender, EventArgs e)
        {
            Close();
        }

        #region popup

        private void msRtf_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            msiCopiaSelezione.Enabled = !string.IsNullOrEmpty(rtTesto.SelectedText);
            msiStampaSeleziona.Enabled = msiCopiaSelezione.Enabled;
        }

        private void msiCopiaTutto_Click(object sender, EventArgs e)
        {
            rtTesto.BloccaRtf(true);
            int ss = rtTesto.SelectionStart;
            int sl = rtTesto.SelectionLength;
            rtTesto.SelectAll();
            rtTesto.CopiaSenzaTestoNascosto();
            rtTesto.Select(ss, sl);
            rtTesto.BloccaRtf(false);
        }

        private void msiStampaTutto_Click(object sender, EventArgs e)
        {
            int len = rtTesto.Text.Length;
            // non so perché sia necessario usare la variable intermedia len, ma senza la variabile fine in StampaRichText è sempre 0
            StampaRichText(rtTesto, 0, len);
        }

        private void msiSelezionaTutto_Click(object sender, EventArgs e)
        {
            rtTesto.SelectAll();
        }

        private void msiCopiaSelezione_Click(object sender, EventArgs e)
        {
            rtTesto.CopiaSenzaTestoNascosto();
        }

        private void msiStampaSeleziona_Click(object sender, EventArgs e)
        {
            int len = rtTesto.SelectionLength + rtTesto.SelectionStart;
            // non so perché sia necessario usare la variable intermedia len, ma senza la variabile fine in StampaRichText è sempre 0
            StampaRichText(rtTesto, rtTesto.SelectionStart, len);
        }

        private void StampaRichText(RichTextBoxEx rtb, int inizio, int fine)
        { // anche in LaParola/principale.cs
            if (rtb == null)
                throw new ArgumentNullException("rtb");
            else
            {
                // aggiustare la lunghezza del testo da stampare quando c'è testo nascosto incluso
                if (fine < rtb.Text.Length)
                {
                    for (int i = inizio; i <= fine; ++i)
                    {
                        if (rtb.Text[i] == RichTextBoxEx.InizioRiferimento)
                            fine += 9; // anche 8 caratteri per le 8 cifre dopo InizioRiferimento
                        if (rtb.Text[i] == RichTextBoxEx.InizioLink)
                            ++fine;
                        if (rtb.Text[i] == RichTextBoxEx.FineLink1)
                        {
                            if (rtb.Text.IndexOf(RichTextBoxEx.FineLink2, i) > 0)
                            {
                                fine += rtb.Text.IndexOf(RichTextBoxEx.FineLink2, i) - i + 1;
                                i = rtb.Text.IndexOf(RichTextBoxEx.FineLink2, i);
                            }
                        }
                    }
                }

                if (storedPageSettings != null)
                    printDocument.DefaultPageSettings = storedPageSettings;

                rtbPerStampa = rtb;
                primoCarattereSullaPagina = inizio;
                ultimoCarattereDaStampare = fine;
                PrintDialog pd = new PrintDialog();
                try
                {
                    pd.Document = printDocument;
                    pd.UseEXDialog = true;
                    if (pd.ShowDialog() == DialogResult.OK)
                        printDocument.Print();
                }
                finally
                {
                    pd.Dispose();
                }
            }
        }

        // variables to trace text to print for pagination
        private int primoCarattereSullaPagina;
        private int ultimoCarattereDaStampare;
        private RichTextBoxEx rtbPerStampa = null;

        private void printDoc_BeginPrint(object sender, PrintEventArgs e)
        {
            // Start at the beginning of the text
            //            m_nFirstCharOnPage = 0;
        }

        private void printDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (!isRunningOnMono)
            {
                // To print the boundaries of the current page margins
                // uncomment the next line:
                // e.Graphics.DrawRectangle(System.Drawing.Pens.Blue, e.MarginBounds);

                // make the RichTextBoxEx calculate and render as much text as will
                // fit on the page and remember the last character printed for the
                // beginning of the next page
                primoCarattereSullaPagina = rtbPerStampa.FormatRangeNotMono(false, e, primoCarattereSullaPagina, ultimoCarattereDaStampare);

                // check if there are more pages to print
                if (primoCarattereSullaPagina < ultimoCarattereDaStampare)
                    e.HasMorePages = true;
                else
                    e.HasMorePages = false;
            }
        }

        private void printDoc_EndPrint(object sender, PrintEventArgs e)
        {
            if (!isRunningOnMono)
            {
                // Clean up cached information
                rtbPerStampa.FormatRangeDoneNotMono();
            }
        }

        #endregion

        private static string LinguaPrincipale(string lingua)
        { // anche in funzioni.cs, testi.cs
            if (!string.IsNullOrEmpty(lingua))
                return lingua.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)[0].ToUpperInvariant();
            else
                return "";
        }
    }
}
