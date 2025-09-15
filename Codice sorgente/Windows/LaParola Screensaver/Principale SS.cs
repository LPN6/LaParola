using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using LaParola_Screensaver.Properties;
using TestiBiblici;

// invece di "gacutil /i /silent testi.dll", testi.dll si registra nel GAC con System.EnterpriseServices

// rtDummy serve per evitare con il punto di inserizione sia visibile

[assembly: CLSCompliant(true)]
namespace LaParola_Screensaver
{
    public struct Rect
    {
        public int width { get { return right - left; } }
        public int height { get { return bottom - top; } }
        public int left;
        public int top;
        public int right;
        public int bottom;
        public Rect(int l, int t, int r, int b)
        {
            left = l;
            top = t;
            right = r;
            bottom = b;
        }
    }

    public partial class Principale : ScreensaverBase
    {
        internal static Texts testi;
        private readonly string cartellaDati = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar;

        private ScreensaverBase[] formsSuScreens = null;
        private readonly IntPtr genitore;
        int ticksInizio;

        byte versettoAttuale, capitoloAttuale, libroAttuale;
        int branoAttuale;
        string nomeVersione;
        Riferimento branoDaUsare;
        long versettiOCapitoliInBrano;
        string testoDaMostrare;

        // per l'anteprima
        Rect rect = new Rect();
        Graphics sfondoPerScrivere;
        Font font;
        Brush brush;
        int coordinamentoFisso;
        SizeF grandezzaTesto;

        public Principale(IntPtr parentHwd)
        {
            InitializeComponent();
            //System.Windows.Forms.MessageBox.Show(cartellaDati, cartellaDati);

            genitore = parentHwd;

            testi = new Texts(cartellaDati);

            testi.Formato.FontColore = Settings.Default.FontColore;
            testi.Formato.FontCorsivo = Settings.Default.FontCorsivo;
            testi.Formato.FontGrassetto = Settings.Default.FontGrassetto;
            testi.Formato.FontSottolineato = Settings.Default.FontSottolineato;
            testi.Formato.FontDimensione = Settings.Default.FontDimensione;
            testi.Formato.FontNome = Settings.Default.FontNome;

            testi.Formato.FontRiferimentoColore = Settings.Default.FontColore;
            testi.Formato.FontRiferimentoCorsivo = Settings.Default.FontCorsivo;
            testi.Formato.FontRiferimentoGrassetto = Settings.Default.FontGrassetto;
            testi.Formato.FontRiferimentoSottolineato = Settings.Default.FontSottolineato;
            testi.Formato.FontRiferimentoDimensione = Settings.Default.FontDimensione;
            testi.Formato.FontRiferimentoNome = Settings.Default.FontNome;

            testi.Formato.FontGrecoColore = Settings.Default.FontColore;
            testi.Formato.FontGrecoCorsivo = Settings.Default.FontCorsivo;
            testi.Formato.FontGrecoGrassetto = Settings.Default.FontGrassetto;
            testi.Formato.FontGrecoSottolineato = Settings.Default.FontSottolineato;
            testi.Formato.FontGrecoDimensione = Settings.Default.FontDimensione;
            testi.Formato.FontGrecoNome = Settings.Default.FontNome;

            testi.Formato.FontEbraicoColore = Settings.Default.FontColore;
            testi.Formato.FontEbraicoCorsivo = Settings.Default.FontCorsivo;
            testi.Formato.FontEbraicoGrassetto = Settings.Default.FontGrassetto;
            testi.Formato.FontEbraicoSottolineato = Settings.Default.FontSottolineato;
            testi.Formato.FontEbraicoDimensione = Settings.Default.FontDimensione;
            testi.Formato.FontEbraicoNome = Settings.Default.FontNome;
        }

        private void Principale_Load(object sender, EventArgs e)
        {
            if (genitore != IntPtr.Zero)
            {
                this.Visible = false;

                if (!SafeNativeMethods.IsWindowVisible(genitore))
                { // altrimenti dà errore quando si esce dalla finestra delle impostazioni prima che si arrivi a questo punto nel codice
                    ChiudiProgramma();
                    return;
                }

                sfondoPerScrivere = Graphics.FromHwnd(genitore);
                brush = new SolidBrush(Settings.Default.FontColore);

                FontStyle fs = FontStyle.Regular;
                if (Settings.Default.FontGrassetto)
                    fs |= FontStyle.Bold;
                if (Settings.Default.FontCorsivo)
                    fs |= FontStyle.Italic;
                if (Settings.Default.FontSottolineato)
                    fs |= FontStyle.Underline;
                try
                {
                    font = new Font(Settings.Default.FontNome, Settings.Default.FontDimensione, fs);
                }
                catch (ArgumentException)
                { // con un computer, la riga precedente ha dato un errore che Palatino Linotype non ha lo stile normale. Proviamo in altri modi...
                    font = new Font(Settings.Default.FontNome, Settings.Default.FontDimensione);
                }

                SafeNativeMethods.GetClientRect(genitore, ref rect);
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
                base.CaricaForm(this);
                int screenCount = Screen.AllScreens.Length;
                formsSuScreens = new ScreensaverBase[screenCount];
                for (int i = 0; i < screenCount; i++)
                {
                    if (!Screen.FromControl(this).Equals(Screen.AllScreens[i]))
                    {
                        ScreensaverBase formSuAltroSchermo = new ScreensaverBase();
                        formSuAltroSchermo.Show();
                        formSuAltroSchermo.CaricaForm(this, Screen.AllScreens[i].Bounds);
                        formsSuScreens[i] = formSuAltroSchermo;
                    }
                    else
                    {
                        formsSuScreens[i] = this;
                    }
                }
            }

            if (Settings.Default.DirezioneVerticale)
            {
                //                rtTesto.Width = 3 * Width / 5;
            }
            else
            {
                //                rtTesto.Multiline = false;
                testi.Formato.TitoliVisualizzati = false;
            }

            nomeVersione = Settings.Default.Versione;
            if (!testi.VersioneEsiste(nomeVersione))
                nomeVersione = "";
            if (string.IsNullOrEmpty(nomeVersione))
                nomeVersione = testi.UltimaBibbia;
            if (!testi.VersioneEsiste(nomeVersione))
                nomeVersione = "";
            if (string.IsNullOrEmpty(nomeVersione) && testi.NomiVersioni(TestoTipi.Bibbia).Count > 0)
                nomeVersione = testi.NomiVersioni(TestoTipi.Bibbia)[0];
            if (string.IsNullOrEmpty(nomeVersione))
                Application.Exit();

            branoDaUsare = testi.ConvertiRiferimento(Settings.Default.Brano);
            if (branoDaUsare.Count == 0)
                branoDaUsare.AggiungiBrano(new byte[] { 1, 1, 1, 73, 22, 21 });

            if (Settings.Default.OrdineBiblico)
            {
                libroAttuale = branoDaUsare.Brani[0][0];
                capitoloAttuale = branoDaUsare.Brani[0][1];
                versettoAttuale = branoDaUsare.Brani[0][2];
                if (Settings.Default.PerCapitoli)
                    --capitoloAttuale;
                else
                    --versettoAttuale;
                branoAttuale = 0;
            }
            else
            {
                versettiOCapitoliInBrano = 0;
                foreach (byte[] brano in branoDaUsare.Brani)
                {
                    if (Settings.Default.PerCapitoli)
                    {
                        versettiOCapitoliInBrano += testi.CapitoliFinoALibro(brano[3], nomeVersione) - (brano[4] == 255 ? 0 : testi.CapitoliInLibro(brano[3], nomeVersione) - brano[4]) - (testi.CapitoliFinoALibro(brano[0], nomeVersione) - (testi.CapitoliInLibro(brano[0], nomeVersione) - brano[1] + 1));
                    }
                    else
                    {
                        if (brano[4] == 255)
                            versettiOCapitoliInBrano += testi.VersettiFinoACapitolo(brano[3], testi.CapitoliInLibro(brano[3], nomeVersione), nomeVersione) - (testi.VersettiFinoACapitolo(brano[0], brano[1], nomeVersione) - (testi.VersettiInCapitolo(brano[0], brano[1], nomeVersione) - brano[2] + 1));
                        else
                            versettiOCapitoliInBrano += testi.VersettiFinoACapitolo(brano[3], brano[4], nomeVersione) - (brano[5] == 255 ? 0 : testi.VersettiInCapitolo(brano[3], brano[4], nomeVersione) - brano[5]) - (testi.VersettiFinoACapitolo(brano[0], brano[1], nomeVersione) - (testi.VersettiInCapitolo(brano[0], brano[1], nomeVersione) - brano[2] + 1));
                    }
                }
            }
            //System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.TextWriterTraceListener(@"c:\Documents and Settings\richard\Desktop\trace.txt"));

            NuovoVersetto();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            timer.Enabled = false;

            Application.DoEvents();
            if (genitore != IntPtr.Zero) // anteprima
            {
                // Continue to run until dialog is dismissed
                //System.Diagnostics.Trace.WriteLine("genitore " + genitore.ToString());
                //System.Diagnostics.Trace.Flush();

                if (!SafeNativeMethods.IsWindowVisible(genitore))
                {
                    //System.Diagnostics.Trace.WriteLine("genitore2 " + IsWindowVisible(genitore).ToString());
                    //System.Diagnostics.Trace.Flush();
                    //Close();
                    ChiudiProgramma();
                    return;
                }
            }

            long numeroTick = Environment.TickCount - ticksInizio;
            while (numeroTick < 0)
            {
                numeroTick += Int32.MaxValue;
                numeroTick -= Int32.MinValue;
            }

            if (Settings.Default.Velocita == 0)
            {
                if (numeroTick > 60000) // un minuto
                    NuovoVersetto();
                return;
            }

            if (genitore != IntPtr.Zero) // anteprima
            {
                sfondoPerScrivere.Clear(this.BackColor);
                //System.Diagnostics.Trace.WriteLine("genitore3 " + IsWindowVisible(genitore).ToString());
                //System.Diagnostics.Trace.Flush();
                if (Settings.Default.DirezioneVerticale)
                {
                    int posizione = rect.bottom - Convert.ToInt32(Settings.Default.Velocita * numeroTick * (rect.height) / 1000);
                    if (posizione != posizioneVecchia)
                    {
                        posizioneVecchia = posizione;
                        sfondoPerScrivere.DrawString(testoDaMostrare, font, brush, new RectangleF(0, posizione, rect.width, rect.height - posizione));
                        if (posizione + grandezzaTesto.Height < rect.top)
                            NuovoVersetto();
                    }
                }
                else // orizzontale
                {
                    int posizione = rect.right - Convert.ToInt32(Settings.Default.Velocita * numeroTick * (rect.width) / 1000);
                    if (posizione != posizioneVecchia)
                    {
                        posizioneVecchia = posizione;
                        sfondoPerScrivere.DrawString(testoDaMostrare, font, brush, new PointF(posizione, coordinamentoFisso));
                        if (posizione + grandezzaTesto.Width < rect.left)
                            NuovoVersetto();
                    }
                }
            }
            else // salvaschermo normale
            {
                for (int i = 0; i < formsSuScreens.Length; i++)
                    formsSuScreens[i].SpostaTesto(numeroTick);

                if (Settings.Default.DirezioneVerticale)
                {
                    if (rtTesto.Bottom < 50)
                        NuovoVersetto();
                }
                else // orizzontale
                {
                    if (rtTesto.Right < 100)
                        NuovoVersetto();
                }
            }

            Application.DoEvents();
            timer.Enabled = true;
        }

        private void NuovoVersetto()
        {
            timer.Enabled = false;
            string riferimentoDaMostrare = "";

            if (Settings.Default.OrdineBiblico)
            {
                if (Settings.Default.PerCapitoli)
                {
                    ++capitoloAttuale;
                    if (capitoloAttuale > testi.CapitoliInLibro(libroAttuale, nomeVersione))
                    {
                        capitoloAttuale = 1;
                        ++libroAttuale;
                    }
                    if (libroAttuale > branoDaUsare.Brani[branoAttuale][3] ||
                        (libroAttuale == branoDaUsare.Brani[branoAttuale][3] && capitoloAttuale > branoDaUsare.Brani[branoAttuale][4]))
                    {
                        ++branoAttuale;
                        if (branoAttuale >= branoDaUsare.Brani.Count)
                            branoAttuale = 0;
                        libroAttuale = branoDaUsare.Brani[branoAttuale][0];
                        capitoloAttuale = branoDaUsare.Brani[branoAttuale][1];
                    }
                    versettoAttuale = 1;
                    if (branoDaUsare.Brani[branoAttuale][0] == libroAttuale && branoDaUsare.Brani[branoAttuale][1] == capitoloAttuale)
                        versettoAttuale = branoDaUsare.Brani[branoAttuale][2];
                    byte versettoFine = 255;
                    if (branoDaUsare.Brani[branoAttuale][3] == libroAttuale && branoDaUsare.Brani[branoAttuale][4] == capitoloAttuale)
                        versettoFine = branoDaUsare.Brani[branoAttuale][5];
                    riferimentoDaMostrare = testi.NormalizzaRiferimento(libroAttuale, capitoloAttuale, versettoAttuale, libroAttuale, capitoloAttuale, versettoFine);
                }
                else // per versetti
                {
                    ++versettoAttuale;
                    if (versettoAttuale > testi.VersettiInCapitolo(libroAttuale, capitoloAttuale, nomeVersione))
                    {
                        versettoAttuale = 1;
                        ++capitoloAttuale;
                        if (capitoloAttuale > testi.CapitoliInLibro(libroAttuale, nomeVersione))
                        {
                            capitoloAttuale = 1;
                            ++libroAttuale;
                        }
                    }
                    if (libroAttuale > branoDaUsare.Brani[branoAttuale][3] ||
                        (libroAttuale == branoDaUsare.Brani[branoAttuale][3] && capitoloAttuale > branoDaUsare.Brani[branoAttuale][4]) ||
                        (libroAttuale == branoDaUsare.Brani[branoAttuale][3] && capitoloAttuale == branoDaUsare.Brani[branoAttuale][4] && versettoAttuale > branoDaUsare.Brani[branoAttuale][5]))
                    {
                        ++branoAttuale;
                        if (branoAttuale >= branoDaUsare.Brani.Count)
                            branoAttuale = 0;
                        libroAttuale = branoDaUsare.Brani[branoAttuale][0];
                        capitoloAttuale = branoDaUsare.Brani[branoAttuale][1];
                        versettoAttuale = branoDaUsare.Brani[branoAttuale][2];
                    }
                    riferimentoDaMostrare = testi.NormalizzaRiferimento(libroAttuale, capitoloAttuale, versettoAttuale);
                }
            }
            else // casuale
            {
                if (Settings.Default.PerCapitoli)
                {
                    int numeroCapitolo = numeroCasuale.Next(Convert.ToInt32(versettiOCapitoliInBrano)) + 1;

                    long capitoliInBrano = 0;
                    bool finito = false;
                    long capitoliInBranoVecchio = 0;
                    for (int i = 0; i < branoDaUsare.Brani.Count; ++i)
                    {
                        byte[] brano = branoDaUsare.Brani[i];
                        capitoliInBrano += testi.CapitoliFinoALibro(brano[3], nomeVersione) - (brano[4] == 255 ? 0 : testi.CapitoliInLibro(brano[3], nomeVersione) - brano[4]) - (testi.CapitoliFinoALibro(brano[0], nomeVersione) - (testi.CapitoliInLibro(brano[0], nomeVersione) - brano[1] + 1));

                        if (capitoliInBrano >= numeroCapitolo || i == branoDaUsare.Brani.Count - 1)
                        {
                            capitoliInBrano = capitoliInBranoVecchio;
                            for (byte libro = brano[0]; libro <= brano[3]; ++libro)
                            {
                                if (libro == brano[0])
                                    capitoliInBrano += testi.CapitoliInLibro(libro, nomeVersione) - brano[1] + 1;
                                else if (libro == brano[3])
                                    capitoliInBrano += brano[4] == 255 ? testi.CapitoliInLibro(libro, nomeVersione) : brano[4];
                                else
                                    capitoliInBrano += testi.CapitoliInLibro(libro, nomeVersione);
                                if (capitoliInBrano >= numeroCapitolo)
                                {
                                    byte capitolo = Convert.ToByte(numeroCapitolo - capitoliInBranoVecchio + (libro == brano[0] ? brano[1] - 1 : 0));
                                    riferimentoDaMostrare = testi.NormalizzaRiferimento(libro, capitolo, (libro == brano[0] && capitolo == brano[1]) ? brano[2] : (byte)(1), libro, capitolo, (libro == brano[3] && capitolo == brano[4]) ? brano[5] : (byte)(255));
                                    finito = true;
                                    break;
                                }
                                else
                                    capitoliInBranoVecchio = capitoliInBrano;
                            }
                        }
                        if (finito)
                            break;

                        capitoliInBranoVecchio = capitoliInBrano;
                    }
                }
                else // per versetti
                {
                    int numeroVersetto = numeroCasuale.Next(Convert.ToInt32(versettiOCapitoliInBrano)) + 1;

                    long versettiInBrano = 0;
                    bool finito = false;
                    long versettiInBranoVecchio = 0;
                    for (int i = 0; i < branoDaUsare.Brani.Count; ++i)
                    {
                        byte[] brano = branoDaUsare.Brani[i];
                        if (brano[4] == 255)
                            versettiInBrano += testi.VersettiFinoACapitolo(brano[3], testi.CapitoliInLibro(brano[3], nomeVersione), nomeVersione) - (testi.VersettiFinoACapitolo(brano[0], brano[1], nomeVersione) - (testi.VersettiInCapitolo(brano[0], brano[1], nomeVersione) - brano[2] + 1));
                        else
                            versettiInBrano += testi.VersettiFinoACapitolo(brano[3], brano[4], nomeVersione) - (brano[5] == 255 ? 0 : testi.VersettiInCapitolo(brano[3], brano[4], nomeVersione) - brano[5]) - (testi.VersettiFinoACapitolo(brano[0], brano[1], nomeVersione) - (testi.VersettiInCapitolo(brano[0], brano[1], nomeVersione) - brano[2] + 1));

                        if (versettiInBrano >= numeroVersetto || i == branoDaUsare.Brani.Count - 1)
                        {
                            versettiInBrano = versettiInBranoVecchio;
                            for (byte libro = brano[0]; libro <= brano[3]; ++libro)
                            {
                                byte massimoCapitoli = (libro == brano[3] ? (brano[4] == 255 ? testi.CapitoliInLibro(libro, nomeVersione) : brano[4]) : testi.CapitoliInLibro(libro, nomeVersione));
                                for (byte capitolo = (libro == brano[0] ? brano[1] : (byte)1); capitolo <= massimoCapitoli; ++capitolo)
                                {
                                    versettiInBrano += testi.VersettiInCapitolo(libro, capitolo, nomeVersione) - (capitolo == brano[1] ? brano[2] : 1) + 1;
                                    if (versettiInBrano >= numeroVersetto)
                                    {
                                        riferimentoDaMostrare = testi.NormalizzaRiferimento(libro, capitolo, Convert.ToByte(numeroVersetto - versettiInBranoVecchio + (capitolo == brano[1] ? brano[2] : 1) - 1));
                                        finito = true;
                                        break;
                                    }
                                    else
                                        versettiInBranoVecchio = versettiInBrano;
                                }
                                if (finito)
                                    break;
                            }
                        }
                        if (finito)
                            break;

                        versettiInBranoVecchio = versettiInBrano;
                    }
                }
            }
            // i nomi dei libri devono essere nella lingua dei libri usati nel programma principale
            testoDaMostrare = Texts.RimuoviTestoNascosto(testi.TestoBrano(riferimentoDaMostrare, nomeVersione));
            if (Settings.Default.DirezioneVerticale)
            {
                while (testoDaMostrare.EndsWith("\r\n",StringComparison.Ordinal))
                    testoDaMostrare = testoDaMostrare.Remove(testoDaMostrare.Length - 2, 2);
                testoDaMostrare = testoDaMostrare.Remove(testoDaMostrare.Length - 1, 1) + @"\par}";
                testoDaMostrare = testoDaMostrare.Replace("\\par\r\n ", "\\par\r\n").Replace("  ", " ");
            }
            else
            {
                testoDaMostrare = testoDaMostrare.Replace(@"\pard", "lpnqxd").Replace(@"\par", "  ").Replace("lpnqxd", @"\pard").Replace("  ", " ");
            }

            if (genitore != IntPtr.Zero) // anteprima
            {
                try
                {
                    rtTesto.Rtf = testoDaMostrare;
                }
                catch
                {
                    rtTesto.Text = testoDaMostrare;
                }

                testoDaMostrare = rtTesto.Text;
                if (Settings.Default.DirezioneVerticale)
                {
                    grandezzaTesto = sfondoPerScrivere.MeasureString(testoDaMostrare, font, new SizeF(rect.width, float.MaxValue));
                }
                else
                {
                    grandezzaTesto = sfondoPerScrivere.MeasureString(testoDaMostrare, font);
                    int differenzaAltezza = (int)(rect.height - grandezzaTesto.Height);
                    if (differenzaAltezza < 0)
                        differenzaAltezza = 0;
                    coordinamentoFisso = Settings.Default.PosizioneCasuale ? numeroCasuale.Next(0, differenzaAltezza) : differenzaAltezza / 2;
                }
            }
            else // salvaschermo normale
            {
                for (int i = 0; i < formsSuScreens.Length; i++)
                    formsSuScreens[i].SetTesto(testoDaMostrare);
            }

            ticksInizio = Environment.TickCount;
            timer.Enabled = true;
        }
    }

    internal static class SafeNativeMethods
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetClientRect(IntPtr hWnd, ref Rect rect);

        [DllImport("user32.DLL", EntryPoint = "IsWindowVisible")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool IsWindowVisible(IntPtr hWnd);
    }
}
