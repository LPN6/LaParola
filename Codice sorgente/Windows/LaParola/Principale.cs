using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using LaParola.Properties;
using TestiBiblici;
//using System.Diagnostics;

// Nota: bisogna compilare per il processore x86 (non per "Any") affinché funzioni su Windows 64
// vedi http://oregonstate.edu/~reeset/blog/archives/444

[assembly: CLSCompliant(true)]
namespace LaParola
{
    public partial class Principale : Form
    {
        #region Proprietà

        internal static Texts testi;

        internal static double pixelPerCm;
        internal static MessageBoxOptions messageBoxOptions;
        internal static bool isRunningOnMono;

        private PageSettings storedPageSettings = null;
        private int childFormNumber = 1;
        private bool aggiornaFont = true;
        private readonly int massimoFontPreferiti = 5;
        private List<string> fontPreferiti;
        private bool fontApplied;
        private string redoCaption, undoCaption;
        private int pulsanteUDGiu;
        private bool faBrowseIndiceCambio = true;
        private bool nonAggiornareBrowseBookmarkBookmark = false;
        private string testoInClipboard = "";

        private string trovaTesto = "";
        private string sostituisciTesto = "";
        private RichTextBoxFinds trovaOpzioni = RichTextBoxFinds.None;

        //        private string urlAggiornamenti = @"c:\users\richard\documents\aggiorna2.xml";
        private string urlAggiornamenti = "https://www.laparola.net/programma/aggiorna.xml";
        private int LUNGHEZZA_MASSIMA_PER_MOSTRARE_LINK = 1000000;

        internal Collection<Riferimento> cronologia;
        internal int numeroInCronologia = -1;
        internal bool aggiornaCronologia = true;

        internal Form formProiettato = null;
        private Rectangle formProiettatoBounds = new Rectangle();

        internal static ResourceManager LocRM = new ResourceManager("LaParola.LaParolaRisorse", typeof(Principale).Assembly);
        internal Editor finestraRisultati;
        internal Visualizza ultimaVisualizza = null;
        internal Collection<InfoLettura> schemiLettura = new Collection<InfoLettura>();
        SplashScreen splashScreen;

        #endregion

        #region StartUp

        public Principale()
        {
            //            Trace.Listeners.Add(new TextWriterTraceListener(@"c:\Documents and Settings\richard\Desktop\trace.txt"));
            //            int tick0 = Environment.TickCount;
            //            Trace.WriteLine("Costruttore " + (Environment.TickCount - tick0).ToString());
            if (Settings.Default.MiscNuovaVersione)
            {
                Settings.Default.Upgrade();
                Settings.Default.MiscNuovaVersione = false;
            }

            if (!string.IsNullOrEmpty(Settings.Default.InterfacciaLingua))
            {
                try
                {
                    // Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(Settings.Default.InterfacciaLingua);
                    Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Settings.Default.InterfacciaLingua);
                }
                // se Settings vuoto o non riconosciuto, non facciamo niente e il valore predefinito è usato
                catch (ArgumentNullException) { }
                catch (ArgumentException) { }
            }
            Settings.Default.InterfacciaLingua = Thread.CurrentThread.CurrentUICulture.Name;

            splashScreen = new SplashScreen();
            splashScreen.Show();
            Application.DoEvents();

            InitializeComponent();

            ImpostaLinguaDellaGuida();

            mainToolStrip.Visible = Settings.Default.PrincipaleBSPrincipale;
            formatToolStrip.Visible = Settings.Default.PrincipaleBSFormato;
            browseToolStrip.Visible = false;
            orderToolStrip.Visible = false;
            commandToolStrip.Visible = Settings.Default.PrincipaleBSComando;

            isRunningOnMono = (Type.GetType("Mono.Runtime") != null);

            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + "Collegamenti" + Path.DirectorySeparatorChar);
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + "Paralleli" + Path.DirectorySeparatorChar);
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + "Segnalibri" + Path.DirectorySeparatorChar);
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + "Letture" + Path.DirectorySeparatorChar);
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + "Disposizioni" + Path.DirectorySeparatorChar);
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + "Video" + Path.DirectorySeparatorChar);
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + "TestiParalleli" + Path.DirectorySeparatorChar);

            //            Trace.WriteLine("testi inizio " + (Environment.TickCount - tick0).ToString());
            //testi = new Texts();
            testi = new Texts(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar, false);
            //Trace.WriteLine("testi fine " + (Environment.TickCount - tick0).ToString());
            testi.AggiungiDirectory(Application.StartupPath);
            //Trace.WriteLine("testi add directory " + (Environment.TickCount - tick0).ToString());

            string[] cartelle = Settings.Default.CartelleDaCercare.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string cartella in cartelle)
                testi.AggiungiDirectory(cartella);
            testi.UltimaBibbiaEvento += new EventHandler<UltimaBibbiaEventArgs>(ChangeUltimaBibbia);
            testi.UltimaBibbia = testi.UltimaBibbia; // quando testi è stato creato e UltimaBibbia impostato, UltimaBibbiaEvento era ancora null. Adesso lo impostiamo di nuovo per fare l'evento

            cronologia = new Collection<Riferimento>();

            if (RightToLeft == RightToLeft.Yes)
                messageBoxOptions = MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading;

            if (Thread.CurrentThread.CurrentUICulture.ToString().Length >= 2)
            {
                switch (Thread.CurrentThread.CurrentUICulture.ToString().Substring(0, 2).ToUpperInvariant())
                {
                    case "IT":
                        boldToolStripButton.Image = LaParola.Properties.Resources.boldit;
                        italicToolStripButton.Image = LaParola.Properties.Resources.italicit;
                        underlineToolStripButton.Image = LaParola.Properties.Resources.underit;
                        findToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.T);
                        replaceToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.U);
                        boldToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.G);
                        italicToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.I);
                        underlineToolStripMenuItem1.ShortcutKeys = (Keys.Control | Keys.S);
                        leftToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.Q);
                        centerToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.A);
                        rightToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.R);
                        justifyToolStripMenuItem.ShortcutKeys = (Keys.Control | Keys.F);
                        break;
                    case "ES":
                        // TODO
                        break;
                }
            }

            boldToolStripButton.ToolTipText += " (CTRL+" + (boldToolStripMenuItem.ShortcutKeys ^ Keys.Control).ToString() + ")";
            italicToolStripButton.ToolTipText += " (CTRL+" + (italicToolStripMenuItem.ShortcutKeys ^ Keys.Control).ToString() + ")";
            underlineToolStripButton.ToolTipText += " (CTRL+" + (underlineToolStripMenuItem1.ShortcutKeys ^ Keys.Control).ToString() + ")";
            alignLeftToolStripButton.ToolTipText += " (CTRL+" + (leftToolStripMenuItem.ShortcutKeys ^ Keys.Control).ToString() + ")";
            alignCenterToolStripButton.ToolTipText += " (CTRL+" + (centerToolStripMenuItem.ShortcutKeys ^ Keys.Control).ToString() + ")";
            alignRightToolStripButton.ToolTipText += " (CTRL+" + (rightToolStripMenuItem.ShortcutKeys ^ Keys.Control).ToString() + ")";
            alignJustifyToolStripButton.ToolTipText += " (CTRL+" + (justifyToolStripMenuItem.ShortcutKeys ^ Keys.Control).ToString() + ")";

            Graphics g = CreateGraphics();
            pixelPerCm = g.DpiX / 2.54;
            g.Dispose();

            //            Trace.WriteLine("libri inizio " + (Environment.TickCount - tick0).ToString());

            #region libri

            string lau = Settings.Default.LibriAbbUsate;
            string ln = Settings.Default.LibriNomi;
            string lar = Settings.Default.LibriAbbRiconosciute;
            if (String.IsNullOrEmpty(lau))
            {
                if (Thread.CurrentThread.CurrentUICulture.ToString().Length > 2)
                {
                    switch (Thread.CurrentThread.CurrentUICulture.ToString().Substring(0, 2).ToUpperInvariant())
                    {
                        case "IT":
                            ln = Texts.LibriNomiItaliano;
                            lau = Texts.LibriAbbreviazioniUsateItaliano;
                            lar = Texts.LibriAbbreviazioniRiconosciuteItaliano;
                            break;
                        case "ES":
                            ln = Texts.LibriNomiSpagnolo;
                            lau = Texts.LibriAbbreviazioniUsateSpagnolo;
                            lar = Texts.LibriAbbreviazioniRiconosciuteSpagnolo;
                            break;
                        default: // i libri in inglese
                            ln = Texts.LibriNomiInglese;
                            lau = Texts.LibriAbbreviazioniUsateInglese;
                            lar = Texts.LibriAbbreviazioniRiconosciuteInglese;
                            break;
                    }
                }
                else // i libri in inglese
                {
                    ln = Texts.LibriNomiInglese;
                    lau = Texts.LibriAbbreviazioniUsateInglese;
                    lar = Texts.LibriAbbreviazioniRiconosciuteInglese;
                }
                Settings.Default.LibriAbbUsate = lau;
                Settings.Default.LibriNomi = ln;
                Settings.Default.LibriAbbRiconosciute = lar;
            }

            string[] libriNomi = ln.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < libriNomi.Length; ++i)
            {
                if (i < 73)
                    testi.SetLibroNome(i + 1, libriNomi[i]);
            }

            string[] libriAbbreviazioniUsate = lau.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < libriAbbreviazioniUsate.Length; ++i)
            {
                if (i < 73)
                    testi.SetLibroAbbreviazioneUsata(i + 1, libriAbbreviazioniUsate[i]);
            }

            string[] libriAbbRic = lar.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            string[] abbreviazioniDiLibro;
            testi.LibriAbbreviazioniRiconosciute.Clear();
            for (byte i = 1; i <= 73; ++i)
            {
                abbreviazioniDiLibro = libriAbbRic[i - 1].Split(',');
                foreach (string abbreviazioneDiLibro in abbreviazioniDiLibro)
                    testi.LibriAbbreviazioniRiconosciute[abbreviazioneDiLibro] = i;
            }

            #endregion

            //      Trace.WriteLine("libri/formato testo " + (Environment.TickCount - tick0).ToString());

            #region formato testo

            FormatoTesto formatoTestoSalvato = new FormatoTesto
            {
                FontNome = Settings.Default.FormatoFontNome
            };
            // Palatino Linotype non esiste di solito sui computer Linux
            if (string.IsNullOrEmpty(formatoTestoSalvato.FontNome))
                formatoTestoSalvato.FontNome = isRunningOnMono ? "Times New Roman" : "Palatino Linotype";
            formatoTestoSalvato.FontDimensione = Settings.Default.FormatoFontDimensione;
            formatoTestoSalvato.FontGrassetto = Settings.Default.FormatoFontStileGrassetto;
            formatoTestoSalvato.FontCorsivo = Settings.Default.FormatoFontStileCorsivo;
            formatoTestoSalvato.FontSottolineato = Settings.Default.FormatoFontStileSotto;
            formatoTestoSalvato.FontColore = Settings.Default.FormatoFontColore;

            formatoTestoSalvato.FontGrecoNome = Settings.Default.FormatoFontGrecoNome;
            if (string.IsNullOrEmpty(formatoTestoSalvato.FontGrecoNome))
                formatoTestoSalvato.FontGrecoNome = formatoTestoSalvato.FontNome;
            formatoTestoSalvato.FontGrecoDimensione = Settings.Default.FormatoFontGrecoDimensione;
            formatoTestoSalvato.FontGrecoGrassetto = Settings.Default.FormatoFontGrecoStileGrassetto;
            formatoTestoSalvato.FontGrecoCorsivo = Settings.Default.FormatoFontGrecoStileCorsivo;
            formatoTestoSalvato.FontGrecoSottolineato = Settings.Default.FormatoFontGrecoStileSotto;
            formatoTestoSalvato.FontGrecoColore = Settings.Default.FormatoFontGrecoColore;

            formatoTestoSalvato.FontEbraicoNome = Settings.Default.FormatoFontEbraicoNome;
            formatoTestoSalvato.FontEbraicoDimensione = Settings.Default.FormatoFontEbraicoDimensione;
            formatoTestoSalvato.FontEbraicoGrassetto = Settings.Default.FormatoFontEbraicoStileGrassetto;
            formatoTestoSalvato.FontEbraicoCorsivo = Settings.Default.FormatoFontEbraicoStileCorsivo;
            formatoTestoSalvato.FontEbraicoSottolineato = Settings.Default.FormatoFontEbraicoStileSotto;
            formatoTestoSalvato.FontEbraicoColore = Settings.Default.FormatoFontEbraicoColore;

            formatoTestoSalvato.FontRiferimentoNome = Settings.Default.FormatoFontRifNome;
            if (string.IsNullOrEmpty(formatoTestoSalvato.FontRiferimentoNome))
                formatoTestoSalvato.FontRiferimentoNome = formatoTestoSalvato.FontNome;
            formatoTestoSalvato.FontRiferimentoDimensione = Settings.Default.FormatoFontRifDimensione;
            formatoTestoSalvato.FontRiferimentoGrassetto = Settings.Default.FormatoFontRifStileGrassetto;
            formatoTestoSalvato.FontRiferimentoCorsivo = Settings.Default.FormatoFontRifStileCorsivo;
            formatoTestoSalvato.FontRiferimentoSottolineato = Settings.Default.FormatoFontRifStileSotto;
            formatoTestoSalvato.FontRiferimentoColore = Settings.Default.FormatoFontRifColore;

            formatoTestoSalvato.FontRicercaNome = Settings.Default.FormatoFontRicercaNome;
            if (string.IsNullOrEmpty(formatoTestoSalvato.FontRicercaNome))
                formatoTestoSalvato.FontRicercaNome = formatoTestoSalvato.FontNome;
            formatoTestoSalvato.FontRicercaDimensione = Settings.Default.FormatoFontRicercaDimensione;
            formatoTestoSalvato.FontRicercaGrassetto = Settings.Default.FormatoFontRicercaStileGrassetto;
            formatoTestoSalvato.FontRicercaCorsivo = Settings.Default.FormatoFontRicercaStileCorsivo;
            formatoTestoSalvato.FontRicercaSottolineato = Settings.Default.FormatoFontRicercaStileSotto;
            formatoTestoSalvato.FontRicercaColore = Settings.Default.FormatoFontRicercaColore;

            formatoTestoSalvato.TestoVisualizzato = (TestoVisualizzato)Settings.Default.FormatoTestoBibbia;
            formatoTestoSalvato.TitoliVisualizzati = Settings.Default.FormatoTitoliVisualizzati;
            formatoTestoSalvato.RiferimentoTipo = (RiferimentoTipo)Settings.Default.FormatoRifTipo;
            formatoTestoSalvato.RiferimentoFormato = (RiferimentoFormato)Settings.Default.FormatoRifFormato;
            formatoTestoSalvato.RiferimentoPosto = (RiferimentoPosto)Settings.Default.FormatoRifPosto;
            formatoTestoSalvato.RiferimentoApice = Settings.Default.FormatoRifApice;
            formatoTestoSalvato.RiferimentoContestoRicerche = Settings.Default.FormatoRifContestoRicerche;

            Principale.testi.Formato = formatoTestoSalvato;

            #endregion

            //    Trace.WriteLine("formato testo fine " + (Environment.TickCount - tick0).ToString());

            string ultimaBibbia = Settings.Default.UltimaBibbia;
            foreach (string nomeVersione in testi.NomiVersioni(TestoTipi.Bibbia))
            {
                if (nomeVersione == ultimaBibbia)
                    testi.UltimaBibbia = ultimaBibbia;
            }
            string ultimaBibbiaCompleta = Settings.Default.UltimaBibbiaCompleta;
            foreach (string nomeVersione in testi.NomiVersioni(TestoTipi.Bibbia))
            {
                if (nomeVersione == ultimaBibbiaCompleta)
                    testi.UltimaBibbiaCompleta = ultimaBibbiaCompleta;
            }

            //            Trace.WriteLine("gen menu con testi inizio " + (Environment.TickCount - tick0).ToString());

            GeneraMenuConTesti();

            //          Trace.WriteLine("gen menu con testi fine " + (Environment.TickCount - tick0).ToString());

            redoCaption = redoToolStripMenuItem.Text;
            undoCaption = undoToolStripMenuItem.Text;

            if (isRunningOnMono)
            {
                alignJustifyToolStripButton.Visible = false;
                bulletsToolStripButton.Visible = false;
                indentDecreaseToolStripButton.Visible = false;
                indentIncreaseToolStripButton.Visible = false;
                toolStripSeparatorFormatParagraph.Visible = false;
                arrangementMenu.Alignment = ToolStripItemAlignment.Left;
                statusTranslations.Alignment = ToolStripItemAlignment.Left;
                printToolStripButton.Visible = false;
                printPreviewToolStripButton.Visible = false;
                printToolStripMenuItem.Visible = false;
                printSetupToolStripMenuItem.Visible = false;
                printPreviewToolStripMenuItem.Visible = false;
                printToolStripSeparatorToolbar.Visible = false;
                printToolStripSeparator.Visible = false;
            }
            else
            { // non è fatto nel Designer della form, perché a Mono non piace
                deleteToolStripMenuItem.ShortcutKeys = Keys.Delete;
            }

            // prepara il combobox con i font
            fontPreferiti = new List<string>(Settings.Default.PrincipaleFontPreferiti.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
            if (!DesignMode)
            {
                MettiFontInBarra();
                if (fontPreferiti.Count == 0)
                    fontPreferiti.Add(formatoTestoSalvato.FontNome);
                for (int i = fontPreferiti.Count - 1; i >= 0; --i)
                    fontToolStripComboBox.Items.Insert(0, fontPreferiti[i]);
            }

            browseToolStripComboBox.Items.AddRange(Settings.Default.PrincipaleSfogliaBraniPrecedenti.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
            browseSearchToolStripComboBox.Items.AddRange(Settings.Default.PrincipaleSfogliaRicerchePrecedenti.Split(new char[] { '§' }, StringSplitOptions.RemoveEmptyEntries));
            comandoToolStripComboBox.Items.AddRange(Settings.Default.PrincipaleComandiPrecedenti.Split(new char[] { '§' }, StringSplitOptions.RemoveEmptyEntries));

            //            Trace.WriteLine("crea menu inizio " + (Environment.TickCount - tick0).ToString());

            #region CreaMenu

            // questa regione deve essere prima dell'apertura delle finestre della disposizione,
            // perché la finestra dei Segnalibri legge i segnalibri dal menu principale

            string nomeFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + "Segnalibri" + Path.DirectorySeparatorChar + "Personal.xml";
            if (!File.Exists(nomeFile))
            {
                string testoFile = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>";
                testoFile += Environment.NewLine + "<bookmarks>";
                testoFile += Environment.NewLine + "<name>Personal</name>";
                testoFile += Environment.NewLine + "<name language=\"it\">Personali</name>";
                testoFile += Environment.NewLine + "</bookmarks>";
                File.WriteAllText(nomeFile, testoFile);
            }

            //            Trace.WriteLine("  crea menu coll inizio " + (Environment.TickCount - tick0).ToString());
            CreaMenuCollegamenti();
            //          Trace.WriteLine("  crea menu parall inizio " + (Environment.TickCount - tick0).ToString());
            CreaMenuParalleli();
            //        Trace.WriteLine("  crea menu sl inizio " + (Environment.TickCount - tick0).ToString());
            CreaMenuSegnalibri();
            //      Trace.WriteLine("  crea menu lett inizio " + (Environment.TickCount - tick0).ToString());
            CreaMenuLetture();
            //    Trace.WriteLine("  crea menu disp inizio " + (Environment.TickCount - tick0).ToString());
            CreaMenuDisposizioni();
            //  Trace.WriteLine("  crea menu video inizio " + (Environment.TickCount - tick0).ToString());
            CreaMenuVideo();
            //Trace.WriteLine("  crea menu testipar inizio " + (Environment.TickCount - tick0).ToString());
            CreaMenuTestiParalleli();

            #endregion

            //          Trace.WriteLine("crea menu fine " + (Environment.TickCount - tick0).ToString());

            // TODO (C) visibile
            //similarToolStripMenuItem.Visible = false;

            #region highlight menu

            highlighterToolStripMenuItem.Enabled = !isRunningOnMono;
            underlineToolStripMenuItem.Enabled = !isRunningOnMono;
            for (int i = 0; i < underlineToolStripMenuItem.DropDownItems.Count; ++i)
            {
                ToolStripMenuItem voce = (ToolStripMenuItem)(underlineToolStripMenuItem.DropDownItems[i]);
                byte tag = Convert.ToByte(voce.Tag.ToString(), CultureInfo.InvariantCulture);
                ToolStripMenuItem voceColoreBlack = new ToolStripMenuItem(LocRM.GetString("MiscBlack"), null, HighlightClick)
                {
                    Tag = (byte)(tag),
                    BackColor = Color.Black,
                    ForeColor = Color.White
                };
                voce.DropDownItems.Add(voceColoreBlack);
                ToolStripMenuItem voceColoreBlue = new ToolStripMenuItem(blueHighlighterToolStripMenuItem.Text, null, HighlightClick)
                {
                    Tag = (byte)(tag | 16),
                    BackColor = Color.Blue
                };
                voce.DropDownItems.Add(voceColoreBlue);
                ToolStripMenuItem voceColoreCyan = new ToolStripMenuItem(cyanHighlighterToolStripMenuItem.Text, null, HighlightClick)
                {
                    Tag = (byte)(tag | 32),
                    BackColor = Color.Cyan
                };
                voce.DropDownItems.Add(voceColoreCyan);
                ToolStripMenuItem voceColoreLime = new ToolStripMenuItem(limeHighlighterToolStripMenuItem.Text, null, HighlightClick)
                {
                    Tag = (byte)(tag | 48),
                    BackColor = Color.Lime
                };
                voce.DropDownItems.Add(voceColoreLime);
                ToolStripMenuItem voceColoreMagenta = new ToolStripMenuItem(magentaHighlighterToolStripMenuItem.Text, null, HighlightClick)
                {
                    Tag = (byte)(tag | 64),
                    BackColor = Color.Magenta
                };
                voce.DropDownItems.Add(voceColoreMagenta);
                ToolStripMenuItem voceColoreRed = new ToolStripMenuItem(redHighlighterToolStripMenuItem.Text, null, HighlightClick)
                {
                    Tag = (byte)(tag | 80),
                    BackColor = Color.Red
                };
                voce.DropDownItems.Add(voceColoreRed);
                ToolStripMenuItem voceColoreYellow = new ToolStripMenuItem(yellowHighlighterToolStripMenuItem.Text, null, HighlightClick)
                {
                    Tag = (byte)(tag | 96),
                    BackColor = Color.Yellow
                };
                voce.DropDownItems.Add(voceColoreYellow);
                ToolStripMenuItem voceColoreGreen = new ToolStripMenuItem(greenHighlighterToolStripMenuItem.Text, null, HighlightClick)
                {
                    Tag = (byte)(tag | 160),
                    BackColor = Color.Green
                };
                voce.DropDownItems.Add(voceColoreGreen);
            }

            CopiaMenu(highlightToolStripMenuItem.DropDownItems, highlightFormatToolStripSplitButton.DropDownItems);
            CopiaMenu(highlightToolStripMenuItem.DropDownItems, highlightBrowseToolStripSplitButton.DropDownItems);

            #endregion

            // le finestre caricate all'avvio del programma non sono incluse nella cronologia
            cronologia.Clear();

            // impostare i dizionari da usare se non già fatto
            if (Funzioni.LinguaPrincipale(testi.Info(Settings.Default.DizionarioInglese).Lingua) != "en")
                Settings.Default.DizionarioInglese = "";
            if (Funzioni.LinguaPrincipale(testi.Info(Settings.Default.DizionarioItaliano).Lingua) != "it")
                Settings.Default.DizionarioItaliano = "";
            if (Funzioni.LinguaPrincipale(testi.Info(Settings.Default.DizionarioGreco).Lingua) != "el")
                Settings.Default.DizionarioGreco = "";
            if (Funzioni.LinguaPrincipale(testi.Info(Settings.Default.DizionarioEbraico).Lingua) != "he")
                Settings.Default.DizionarioEbraico = "";
            if (Funzioni.LinguaPrincipale(testi.Info(Settings.Default.DizionarioLatino).Lingua) != "la")
                Settings.Default.DizionarioLatino = "";
            if (string.IsNullOrEmpty(Settings.Default.DizionarioInglese) && testi.Info("International Standard Bible Encyclopedia").Lingua == "en")
                Settings.Default.DizionarioInglese = "International Standard Bible Encyclopedia";
            if (string.IsNullOrEmpty(Settings.Default.DizionarioInglese) && testi.Info("Easton's Bible Dictionary").Lingua == "en")
                Settings.Default.DizionarioInglese = "Easton's Bible Dictionary";
            if (string.IsNullOrEmpty(Settings.Default.DizionarioInglese) && testi.Info("Torrey's New Topical Textbook").Lingua == "en")
                Settings.Default.DizionarioInglese = "Torrey's New Topical Textbook";
            if (string.IsNullOrEmpty(Settings.Default.DizionarioItaliano) && testi.Info("Enciclopedia biblica").Lingua == "it")
                Settings.Default.DizionarioItaliano = "Enciclopedia biblica";
            if (Thread.CurrentThread.CurrentUICulture.Name.Length >= 2 && Thread.CurrentThread.CurrentUICulture.Name.Substring(0, 2).ToUpperInvariant() == "IT")
            {
                if (string.IsNullOrEmpty(Settings.Default.DizionarioGreco) && Funzioni.LinguaPrincipale(testi.Info("Vocabolario del Nuovo Testamento").Lingua) == "el")
                    Settings.Default.DizionarioGreco = "Vocabolario del Nuovo Testamento";
            }
            if (string.IsNullOrEmpty(Settings.Default.DizionarioGreco) && Funzioni.LinguaPrincipale(testi.Info("Strong's Greek Dictionary").Lingua) == "el")
                Settings.Default.DizionarioGreco = "Strong's Greek Dictionary";
            if (string.IsNullOrEmpty(Settings.Default.DizionarioEbraico) && Funzioni.LinguaPrincipale(testi.Info("Strong's Hebrew Dictionary").Lingua) == "he")
                Settings.Default.DizionarioEbraico = "Strong's Hebrew Dictionary";
            if (string.IsNullOrEmpty(Settings.Default.DizionarioLatino) && Funzioni.LinguaPrincipale(testi.Info("Words Latin Dictionary").Lingua) == "la")
                Settings.Default.DizionarioLatino = "Words Latin Dictionary";
            Collection<string> dizionariTutti = testi.NomiVersioni(TestoTipi.Dizionario);
            foreach (string dizionario in dizionariTutti)
            {
                switch (Funzioni.LinguaPrincipale(Principale.testi.Info(dizionario).Lingua))
                {
                    case "en":
                        if (string.IsNullOrEmpty(Settings.Default.DizionarioInglese))
                            Settings.Default.DizionarioInglese = dizionario;
                        break;
                    case "it":
                        if (string.IsNullOrEmpty(Settings.Default.DizionarioItaliano))
                            Settings.Default.DizionarioItaliano = dizionario;
                        break;
                    case "el":
                        if (string.IsNullOrEmpty(Settings.Default.DizionarioGreco))
                            Settings.Default.DizionarioGreco = dizionario;
                        break;
                    case "he":
                        if (string.IsNullOrEmpty(Settings.Default.DizionarioEbraico))
                            Settings.Default.DizionarioEbraico = dizionario;
                        break;
                    case "la":
                        if (string.IsNullOrEmpty(Settings.Default.DizionarioLatino))
                            Settings.Default.DizionarioLatino = dizionario;
                        break;
                }
            }
            Settings.Default.DizionarioSpagnolo = Settings.Default.DizionarioInglese;

            if (!isRunningOnMono)
                HttpWebRequest.DefaultCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);

            if (!isRunningOnMono)
            {
                // registrare i file del programma per essere aperti da Explorer
                // in teoria, si potrebbe fare questo con Mono su Windows, ma non serve usare Mono su Windows quando c'è .NET
                Microsoft.Win32.RegistryKey key, key1;
                try
                {
                    key = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(".laparola");
                    key.SetValue("", "LaParola file");
                    key = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey("LaParola file");
                    key1 = key.CreateSubKey("DefaultIcon");
                    key1.SetValue("", Application.ExecutablePath + ",0");
                    key = key.CreateSubKey("shell").CreateSubKey("open").CreateSubKey("command");
                    key.SetValue("", "\"" + Application.ExecutablePath + "\" \"%1\"");
                }
                catch { } // l'utente non ha l'autorizzazione di modificare il registro

                string startupPath = Application.StartupPath;
                if (!string.IsNullOrEmpty(startupPath) && startupPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                    startupPath = startupPath.Remove(startupPath.Length - Path.DirectorySeparatorChar.ToString().Length);
                // registrare i file del programma per essere aperti da Explorer
                // in teoria, si potrebbe fare questo con Mono su Windows, ma non serve usare Mono su Windows quando c'è .NET
                if (!startupPath.Contains("Debug") || !startupPath.Contains("Visual Studio"))
                {
                    try
                    {
                        key = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(@"TypeLib\{65C6C70A-EF12-4BDC-974E-329244ABF88A}\7.7");
                        key.SetValue("", "LaParola Type Library");
                        key1 = key.CreateSubKey("0").CreateSubKey("win32");
                        key1.SetValue("", startupPath + Path.DirectorySeparatorChar + "testi.tlb");
                        key1 = key.CreateSubKey("Flags");
                        key1.SetValue("", "0");
                        key1 = key.CreateSubKey("HelpDir");
                        key1.SetValue("", startupPath + Path.DirectorySeparatorChar + @"testi.tlb");
                        key = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey("LaParola.Texts");
                        key.SetValue("", "TestiBiblici.Texts");
                        key1 = key.CreateSubKey("CLSID");
                        key.SetValue("", "{E1DC4C02-505D-46C5-A86E-D9620359EC07}");
                        key = Microsoft.Win32.Registry.ClassesRoot.CreateSubKey(@"CLSID\{E1DC4C02-505D-46C5-A86E-D9620359EC07}");
                        key.SetValue("", "TestiBiblici.Texts");
                        key1 = key.CreateSubKey("InprocServer32");
                        key1.SetValue("", "mscoree.dll");
                        key1.SetValue("ThreadingModel", "Both");
                        key1.SetValue("Class", "TestiBiblici.Texts");
                        key1.SetValue("Assembly", "testi, Version=" + Assembly.GetExecutingAssembly().GetName().Version.ToString() + ", Culture=neutral, PublicKeyToken=ce04463d0d40d8e0");
                        key1.SetValue("RuntimeVersion", "v2.0.50727");
                        key1.SetValue("Codebase", @"file:///" + startupPath.Replace(Path.DirectorySeparatorChar, '/') + "/testi.dll");
                        key1 = key1.CreateSubKey(Assembly.GetExecutingAssembly().GetName().Version.ToString());
                        key1.SetValue("Class", "TestiBiblici.Texts");
                        key1.SetValue("Assembly", "testi, Version=" + Assembly.GetExecutingAssembly().GetName().Version.ToString() + ", Culture=neutral, PublicKeyToken=ce04463d0d40d8e0");
                        key1.SetValue("RuntimeVersion", "v2.0.50727");
                        key1.SetValue("Codebase", @"file:///" + startupPath.Replace(Path.DirectorySeparatorChar, '/') + "/testi.dll");
                        key1 = key.CreateSubKey("ProgId");
                        key1.SetValue("", "LaParola.Texts");
                        key1 = key.CreateSubKey(@"Implemented Categories\{62C8FE65-4EBB-45E7-B440-6E39B2CDBF29}");
                    }
                    catch { }
                }

                try // salvare la directory del programma
                {
                    key = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(@"Software\LaParola");
                    key.SetValue("AppPath", startupPath);
                }
                catch { }

                // registrare il programma per essere aperto dal toolbar di LaParola.Net
                try
                {
                    //                    key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(@"Software\Conduit\AppPaths\LaParola");
                    //                    key.SetValue("AppPath", Application.ExecutablePath);
                    key = Microsoft.Win32.Registry.LocalMachine.CreateSubKey(@"Software\Conduit\AppPaths\LaParola");
                    key.SetValue("AppPath", Application.ExecutablePath);
                }
                catch { }
            }

            //            Trace.WriteLine("fine init " + (Environment.TickCount - tick0).ToString());
            //          Trace.WriteLine("");
            //        Trace.Flush();
        }

        private void ImpostaLinguaDellaGuida()
        {
            string lingua = Thread.CurrentThread.CurrentUICulture.Name.ToLowerInvariant();
            string nuovoNamespace = "laparola.chm";
            if (lingua.Length >= 2)
            {
                if (lingua.Substring(0, 2) == "it" && File.Exists(Application.StartupPath + @"\laparola.it.chm"))
                    nuovoNamespace = "laparola.it.chm";
                if (lingua.Substring(0, 2) == "es" && File.Exists(Application.StartupPath + @"\laparola.es.chm"))
                    nuovoNamespace = "laparola.es.chm";
            }
            if (fileGuida.HelpNamespace != nuovoNamespace)
                fileGuida.HelpNamespace = nuovoNamespace;
        }

        #region creare i menu

        public void GeneraMenuConTesti()
        {
            int massimoVoci = Settings.Default.PrincipaleMassimoVociMenu;
            if (massimoVoci <= 0 && bibleStripMenuItem.Height > 0)
            {
                massimoVoci = Screen.GetWorkingArea(this).Height / bibleStripMenuItem.Height - 2;
                if (massimoVoci > 0)
                    Settings.Default.PrincipaleMassimoVociMenu = massimoVoci;
                else
                    massimoVoci = 9999;
            }

            bibleStripMenuItem.DropDownItems.Clear();
            bibleToolStripButton.DropDownItems.Clear();
            notaToolStripButton.DropDownItems.Clear();
            apriNotaToolStripButton.DropDownItems.Clear();
            commentaryStripMenuItem.DropDownItems.Clear();
            dictionaryStripMenuItem.DropDownItems.Clear();
            createNoteToolStripMenuItem.DropDownItems.Clear();
            bookStripMenuItem.DropDownItems.Clear();
            aboutBibleToolStripMenuItem.DropDownItems.Clear();
            statusTranslations.DropDownItems.Clear();

            ToolStripItem[] vociSottoMenuBibbia1 = new ToolStripItem[testi.NomiVersioni(TestoTipi.Bibbia).Count];
            ToolStripItem[] vociSottoMenuBibbia2 = new ToolStripItem[vociSottoMenuBibbia1.Length];
            ToolStripItem[] vociSottoMenuBibbia3 = new ToolStripItem[vociSottoMenuBibbia1.Length];
            int iVociBibbia = 0;
            foreach (string s in testi.NomiVersioni(TestoTipi.Bibbia))
            {
                //                bibleStripMenuItem.DropDownItems.Add(s, null, VisualizzaBibbiaClick);
                //                bibleToolStripButton.DropDownItems.Add(s, null, VisualizzaBibbiaClick);
                //                statusTranslations.DropDownItems.Add(s, null, CambiaBibbiaUtilizzata);
                vociSottoMenuBibbia1[iVociBibbia] = new ToolStripMenuItem(s, null, VisualizzaBibbiaClick);
                vociSottoMenuBibbia2[iVociBibbia] = new ToolStripMenuItem(s, null, VisualizzaBibbiaClick);
                vociSottoMenuBibbia3[iVociBibbia] = new ToolStripMenuItem(s, null, CambiaBibbiaUtilizzata);
                ++iVociBibbia;
            }
            bibleStripMenuItem.DropDownItems.AddRange(vociSottoMenuBibbia1);
            bibleToolStripButton.DropDownItems.AddRange(vociSottoMenuBibbia2);
            statusTranslations.DropDownItems.AddRange(vociSottoMenuBibbia3);

            AggiustaNumeroVociInMenu(bibleStripMenuItem, massimoVoci - 2/*, VisualizzaBibbiaClick*/); // -2 perché un separatore e una voce per tutte le versioni saranno aggiunte
            AggiustaNumeroVociInMenu(bibleToolStripButton, massimoVoci - 2/*, VisualizzaBibbiaClick*/);
            AggiustaNumeroVociInMenu(statusTranslations, massimoVoci/*, CambiaBibbiaUtilizzata*/);

            if (bibleStripMenuItem.DropDownItems.Count == 0)
                bibleStripMenuItem.DropDownItems.Add(LocRM.GetString("MainNoBible"));
            if (bibleToolStripButton.DropDownItems.Count == 0)
                bibleToolStripButton.DropDownItems.Add(LocRM.GetString("MainNoBible"));
            statusTranslations.Visible = (statusTranslations.DropDownItems.Count > 0);
            if (bibleStripMenuItem.DropDownItems.Count >= 2)
            {
                bibleStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
                bibleStripMenuItem.DropDownItems.Add(LocRM.GetString("MainAllBibles"), null, VisualizzaTutteBibbie_Click);
                bibleToolStripButton.DropDownItems.Add(new ToolStripSeparator());
                bibleToolStripButton.DropDownItems.Add(LocRM.GetString("MainAllBibles"), null, VisualizzaTutteBibbie_Click);
            }

            ToolStripItem[] vociSottoMenuComm1 = new ToolStripItem[testi.NomiVersioni(TestoTipi.Commentario).Count];
            ToolStripItem[] vociSottoMenuComm2 = new ToolStripItem[vociSottoMenuComm1.Length];
            int iVociComm = 0;
            foreach (string s in testi.NomiVersioni(TestoTipi.Commentario))
            {
                //                commentaryStripMenuItem.DropDownItems.Add(s, null, VisualizzaCommentarioClick);
                //                notaToolStripButton.DropDownItems.Add(s, null, VisualizzaCommentarioClick);
                vociSottoMenuComm1[iVociComm] = new ToolStripMenuItem(s, null, VisualizzaCommentarioClick);
                vociSottoMenuComm2[iVociComm] = new ToolStripMenuItem(s, null, VisualizzaCommentarioClick);
                ++iVociComm;
            }
            commentaryStripMenuItem.DropDownItems.AddRange(vociSottoMenuComm1);
            notaToolStripButton.DropDownItems.AddRange(vociSottoMenuComm2);

            AggiustaNumeroVociInMenu(commentaryStripMenuItem, massimoVoci - 2/*, VisualizzaCommentarioClick*/);
            AggiustaNumeroVociInMenu(notaToolStripButton, massimoVoci - 2/*, VisualizzaCommentarioClick*/);

            commentaryStripMenuItem.Visible = (commentaryStripMenuItem.DropDownItems.Count > 0);
            notaToolStripButton.Visible = (notaToolStripButton.DropDownItems.Count > 0);
            if (commentaryStripMenuItem.DropDownItems.Count >= 2)
            {
                commentaryStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
                commentaryStripMenuItem.DropDownItems.Add(LocRM.GetString("MainAllCommentaries"), null, VisualizzaTuttiCommentari_Click);
                notaToolStripButton.DropDownItems.Add(new ToolStripSeparator());
                notaToolStripButton.DropDownItems.Add(LocRM.GetString("MainAllCommentaries"), null, VisualizzaTuttiCommentari_Click);
            }

            ToolStripItem[] vociSottoMenuDiz = new ToolStripItem[testi.NomiVersioni(TestoTipi.Dizionario).Count];
            int iVociDiz = 0;
            foreach (string s in testi.NomiVersioni(TestoTipi.Dizionario))
            {
                //                dictionaryStripMenuItem.DropDownItems.Add(s, null, VisualizzaDizionarioClick);
                vociSottoMenuDiz[iVociDiz] = new ToolStripMenuItem(s, null, VisualizzaDizionarioClick);
                ++iVociDiz;
            }
            dictionaryStripMenuItem.DropDownItems.AddRange(vociSottoMenuDiz);
            AggiustaNumeroVociInMenu(dictionaryStripMenuItem, massimoVoci - 2/*, VisualizzaDizionarioClick*/);
            dictionaryStripMenuItem.Visible = (dictionaryStripMenuItem.DropDownItems.Count > 0);
            if (dictionaryStripMenuItem.DropDownItems.Count >= 2)
            {
                dictionaryStripMenuItem.DropDownItems.Add(new ToolStripSeparator());
                dictionaryStripMenuItem.DropDownItems.Add(LocRM.GetString("MainAllCommentaries"), null, VisualizzaTuttiDizionari_Click);
            }

            ToolStripItem[] vociSottoMenuLibro = new ToolStripItem[testi.NomiVersioni(TestoTipi.Libro).Count];
            int iVociLibro = 0;
            foreach (string s in testi.NomiVersioni(TestoTipi.Libro))
            {
                //                bookStripMenuItem.DropDownItems.Add(s, null, VisualizzaLibroClick);
                vociSottoMenuLibro[iVociLibro] = new ToolStripMenuItem(s, null, VisualizzaLibroClick);
                ++iVociLibro;
            }
            bookStripMenuItem.DropDownItems.AddRange(vociSottoMenuLibro);
            AggiustaNumeroVociInMenu(bookStripMenuItem, massimoVoci/*, VisualizzaLibroClick*/);
            bookStripMenuItem.Visible = (bookStripMenuItem.DropDownItems.Count > 0);

            ToolStripItem[] vociSottoMenuCommDiz1 = new ToolStripItem[testi.NomiVersioni(TestoTipi.Commentario | TestoTipi.Dizionario).Count];
            ToolStripItem[] vociSottoMenuCommDiz2 = new ToolStripItem[vociSottoMenuCommDiz1.Length];
            int iVociCommDiz = 0;
            foreach (string s in testi.NomiVersioni(TestoTipi.Commentario | TestoTipi.Dizionario))
            {
                vociSottoMenuCommDiz1[iVociCommDiz] = new ToolStripMenuItem(s, null, createNoteToolStripMenuItem_Click);
                vociSottoMenuCommDiz2[iVociCommDiz] = new ToolStripMenuItem(s, null, createNoteToolStripMenuItem_Click);
                ++iVociCommDiz;
                //                apriNotaToolStripButton.DropDownItems.Add(s, null, createNoteToolStripMenuItem_Click);
                //                createNoteToolStripMenuItem.DropDownItems.Add(s, null, createNoteToolStripMenuItem_Click);
            }
            apriNotaToolStripButton.DropDownItems.AddRange(vociSottoMenuCommDiz1);
            createNoteToolStripMenuItem.DropDownItems.AddRange(vociSottoMenuCommDiz2);

            AggiustaNumeroVociInMenu(apriNotaToolStripButton, massimoVoci/*, createNoteToolStripMenuItem_Click*/);
            AggiustaNumeroVociInMenu(createNoteToolStripMenuItem, massimoVoci/*, createNoteToolStripMenuItem_Click*/);
            apriNotaToolStripButton.Visible = (apriNotaToolStripButton.DropDownItems.Count > 0);
            createNoteToolStripMenuItem.Visible = (createNoteToolStripMenuItem.DropDownItems.Count > 0);

            ToolStripItem[] vociSottoMenuTutti = new ToolStripItem[testi.NomiVersioni().Count];
            int iVociTutti = 0;
            foreach (string s in testi.NomiVersioni())
            {
                vociSottoMenuTutti[iVociTutti] = new ToolStripMenuItem(s, null, AboutBibleToolStripMenuItemClick);
                ++iVociTutti;
                //                aboutBibleToolStripMenuItem.DropDownItems.Add(s, null, AboutBibleToolStripMenuItemClick);
            }
            aboutBibleToolStripMenuItem.DropDownItems.AddRange(vociSottoMenuTutti);
            AggiustaNumeroVociInMenu(aboutBibleToolStripMenuItem, massimoVoci - 2/*, AboutBibleToolStripMenuItemClick*/);
            if (aboutBibleToolStripMenuItem.DropDownItems.Count == 0)
                aboutBibleToolStripMenuItem.DropDownItems.Add(LocRM.GetString("MainNoBible"));
            else
            {
                aboutBibleToolStripMenuItem.DropDownItems.Insert(0, new ToolStripSeparator());
                aboutBibleToolStripMenuItem.DropDownItems.Insert(0, new ToolStripMenuItem(LocRM.GetString("MainListTexts"), null, GeneraListaTesti_Click));
            }

            exportToolStripMenuItem.Enabled = (testi.NomiVersioni().Count > 0);

            foreach (Form formFiglio in MdiChildren)
            {
                if (formFiglio.Tag != null)
                {
                    if (formFiglio.Tag.ToString() == "Visualizza")
                        ((Visualizza)formFiglio).AggiornaMenu();
                    if (formFiglio.Tag.ToString() == "Editor")
                        ((Editor)formFiglio).AggiornaMenuCollezioni();
                }
            }
        }

        static private void AggiustaNumeroVociInMenu(ToolStripDropDownItem menu, int massimoVoci/*, EventHandler evento*/)
        {
            int ultimaVoceInMenu = 0;
            int numeroMenuConSecondaColonna = 0;
            while (ultimaVoceInMenu >= 0 && menu.DropDownItems.Count > massimoVoci)
            {
                char primaLettera = menu.DropDownItems[massimoVoci - 2 - numeroMenuConSecondaColonna].Text[0]; // la prima lettera della prima voce che _deve_ essere nel secondo elenco
                ultimaVoceInMenu = massimoVoci - 3 - numeroMenuConSecondaColonna; // cerchiamo la prima voce precedente a quelli che devono essere nel secondo elenco, per trovare la prima con una lettera iniziale diversa
                while (ultimaVoceInMenu >= 0 && menu.DropDownItems[ultimaVoceInMenu].Text[0] == primaLettera)
                    --ultimaVoceInMenu;
                int numeroVoci = menu.DropDownItems.Count;
                if (numeroVoci - 1 - numeroMenuConSecondaColonna - ultimaVoceInMenu > massimoVoci)
                {
                    ultimaVoceInMenu = numeroVoci - 1 - numeroMenuConSecondaColonna - massimoVoci;
                    primaLettera = menu.DropDownItems[ultimaVoceInMenu].Text[0];
                    while (ultimaVoceInMenu < numeroVoci - numeroMenuConSecondaColonna && menu.DropDownItems[ultimaVoceInMenu].Text[0] == primaLettera)
                        ++ultimaVoceInMenu;
                    --ultimaVoceInMenu; // la riga precedente sposta ultimaVoceInMenu fino a quando la prima lettera è diversa; andiamo indietro una voce affinché sia l'ultima voce con la stessa prima lettera
                    if (ultimaVoceInMenu < numeroVoci - numeroMenuConSecondaColonna - 2)
                        primaLettera = menu.DropDownItems[ultimaVoceInMenu + 1].Text[0];
                }
                if (ultimaVoceInMenu > 0 && ultimaVoceInMenu < numeroVoci - numeroMenuConSecondaColonna - 2)
                {
                    ToolStripMenuItem nuovaVoce = new ToolStripMenuItem(primaLettera + "-" + menu.DropDownItems[numeroVoci - 1 - numeroMenuConSecondaColonna].Text[0]);
                    menu.DropDownItems.Insert(numeroVoci - numeroMenuConSecondaColonna, nuovaVoce); // nota: adesso ci sono numeroVoci+1 voci nel menu
                    ToolStripItem[] vociSottoMenu = new ToolStripItem[numeroVoci - numeroMenuConSecondaColonna - ultimaVoceInMenu - 1];
                    for (int i = ultimaVoceInMenu + 1; i < numeroVoci - numeroMenuConSecondaColonna; ++i)
                    {
                        //                        if (isRunningOnMono)
                        //                        {
                        vociSottoMenu[i - ultimaVoceInMenu - 1] = menu.DropDownItems[ultimaVoceInMenu + 1];
                        //                            nuovaVoce.DropDownItems.Add(menu.DropDownItems[ultimaVoceInMenu + 1].Text, null, evento);
                        //                            menu.DropDownItems.RemoveAt(ultimaVoceInMenu + 1);
                        //                        }
                        //                        else
                        //                        {
                        // questo rimuove anche la voce da "menu" in .NET, ma non in Mono
                        //                            nuovaVoce.DropDownItems.Add(menu.DropDownItems[ultimaVoceInMenu + 1]);
                        vociSottoMenu[i - ultimaVoceInMenu - 1] = menu.DropDownItems[ultimaVoceInMenu + 1];
                        menu.DropDownItems.RemoveAt(ultimaVoceInMenu + 1);
                        //                        }
                    }
                    nuovaVoce.DropDownItems.AddRange(vociSottoMenu);
                }
                ++numeroMenuConSecondaColonna;
            }
        }

        public void CreaMenuCollegamenti()
        {
            while (externalLinkStripMenuItem.DropDownItems.Count >= 3) // per lasciare il separatore e la voce Modifica
                externalLinkStripMenuItem.DropDownItems.RemoveAt(0);

            AggiungiCollegamentiDaDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + "Collegamenti" + Path.DirectorySeparatorChar);
            AggiungiCollegamentiDaDirectory(Application.StartupPath + Path.DirectorySeparatorChar + "Collegamenti" + Path.DirectorySeparatorChar);
            string[] cartelle = Settings.Default.CartelleDaCercare.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string cartella in cartelle)
                AggiungiCollegamentiDaDirectory(cartella + Path.DirectorySeparatorChar + "Collegamenti" + Path.DirectorySeparatorChar);

            externalLinkToolStripDropDownButton.DropDown = externalLinkStripMenuItem.DropDown;
        }

        private void CreaMenuParalleli()
        {
            AggiungiParalleliDaDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + "Paralleli" + Path.DirectorySeparatorChar);
            AggiungiParalleliDaDirectory(Application.StartupPath + Path.DirectorySeparatorChar + "Paralleli" + Path.DirectorySeparatorChar);
            string[] cartelle = Settings.Default.CartelleDaCercare.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string cartella in cartelle)
                AggiungiParalleliDaDirectory(cartella + Path.DirectorySeparatorChar + "Paralleli" + Path.DirectorySeparatorChar);

            if (parallelsToolStripMenuItem.DropDownItems.Count == 0)
            {
                parallelsToolStripMenuItem.Visible = false;
                parallelsToolStripDropDownButton.Visible = false;
            }
            else
                parallelsToolStripDropDownButton.DropDown = parallelsToolStripMenuItem.DropDown;
        }

        public void CreaMenuSegnalibri()
        {
            while (bookmarksToolStripMenuItem.DropDownItems.Count >= 6) // per lasciare il separatore e la voce Modifica
                bookmarksToolStripMenuItem.DropDownItems.RemoveAt(3); // per lasciare i segnalibri veloci
            // rimettere i segnalibri dei capitoli, che erano appena rimossi
            bookmarksToolStripMenuItem.DropDownItems.Insert(3, bookmarksChaptersToolStripMenuItem);

            // trovare una Bibbia con sia l'AT sia il NT
            string bibbiaDaUsare = testi.UltimaBibbiaCompleta;
            if (string.IsNullOrEmpty(bibbiaDaUsare) && testi.NomiVersioni(TestoTipi.Bibbia).Count > 0)
                bibbiaDaUsare = testi.NomiVersioni(TestoTipi.Bibbia)[0];

            if (string.IsNullOrEmpty(bibbiaDaUsare))
                bookmarksChaptersToolStripMenuItem.Enabled = false;
            else
            {
                int capitoliInLibro;
                // bisogna creare una lista e poi convertire in Array, invece di creare un array subito,
                // perché possibilmente mancano dei libri (per esempio l'apocrifica) anche nell'ultima Bibbia completa.
                // In quel caso, aggiungerebbe una voce null al menu, che dà errore
                List<ToolStripItem> vociOT1 = new List<ToolStripItem>(21);
                List<ToolStripItem> vociOT2 = new List<ToolStripItem>(25);
                List<ToolStripItem> vociNT = new List<ToolStripItem>(27);
                for (byte i = 1; i <= 73; ++i)
                {
                    capitoliInLibro = testi.CapitoliInLibro(i, bibbiaDaUsare);
                    if (capitoliInLibro > 0)
                    {
                        ToolStripMenuItem voceLibro = new ToolStripMenuItem(testi.GetLibroNome(i));
                        if (i <= 21)
                            vociOT1.Add(voceLibro);
                        //                            bookmarksOT1ToolStripMenuItem.DropDownItems.Add(voceLibro);
                        else if (i <= 46)
                            vociOT2.Add(voceLibro);
                        //                            bookmarksOT2ToolStripMenuItem.DropDownItems.Add(voceLibro);
                        else
                            vociNT.Add(voceLibro);
                        //                            bookmarksNTToolStripMenuItem.DropDownItems.Add(voceLibro);
                        ToolStripItem[] vociDelSottoMenu = new ToolStripItem[capitoliInLibro];
                        for (int j = 1; j <= capitoliInLibro; ++j)
                        {
                            ToolStripMenuItem voceCapitolo = new ToolStripMenuItem(testi.GetLibroNome(i) + " " + j.ToString(CultureInfo.InvariantCulture), null, bookmark_Click)
                            {
                                Tag = i.ToString(CultureInfo.InvariantCulture) + " " + j.ToString(CultureInfo.InvariantCulture) + " 1"
                            };
                            vociDelSottoMenu[j - 1] = voceCapitolo;
                            //voceLibro.DropDownItems.Add(voceCapitolo);
                        }
                        voceLibro.DropDownItems.AddRange(vociDelSottoMenu);
                    }
                }
                bookmarksOT1ToolStripMenuItem.DropDownItems.AddRange(vociOT1.ToArray());
                bookmarksOT2ToolStripMenuItem.DropDownItems.AddRange(vociOT2.ToArray());
                bookmarksNTToolStripMenuItem.DropDownItems.AddRange(vociNT.ToArray());
            }

            AggiungiSegnalibriDaDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + "Segnalibri" + Path.DirectorySeparatorChar);
            AggiungiSegnalibriDaDirectory(Application.StartupPath + Path.DirectorySeparatorChar + "Segnalibri" + Path.DirectorySeparatorChar);
            string[] cartelle = Settings.Default.CartelleDaCercare.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string cartella in cartelle)
                AggiungiSegnalibriDaDirectory(cartella + Path.DirectorySeparatorChar + "Segnalibri" + Path.DirectorySeparatorChar);

            bookmarksToolStripButton.DropDown = bookmarksToolStripMenuItem.DropDown;

            string listaSelezionata = (browseBookmarkListToolStripComboBox.SelectedIndex >= 0 ? browseBookmarkListToolStripComboBox.SelectedItem.ToString() : "");
            nonAggiornareBrowseBookmarkBookmark = true;
            for (int i = browseBookmarkListToolStripComboBox.Items.Count - 1; i >= 1; --i)
                browseBookmarkListToolStripComboBox.Items.RemoveAt(i);
            nonAggiornareBrowseBookmarkBookmark = false;
            for (int i = 4; i < bookmarksToolStripMenuItem.DropDownItems.Count - 2; ++i)
            {
                browseBookmarkListToolStripComboBox.Items.Add(bookmarksToolStripMenuItem.DropDownItems[i]);
                if (bookmarksToolStripMenuItem.DropDownItems[i].Text == listaSelezionata)
                {
                    nonAggiornareBrowseBookmarkBookmark = true;
                    browseBookmarkListToolStripComboBox.SelectedIndex = browseBookmarkListToolStripComboBox.Items.Count - 1;
                }
            }
            if (browseBookmarkListToolStripComboBox.SelectedIndex == -1)
                browseBookmarkListToolStripComboBox.SelectedIndex = 0;
            nonAggiornareBrowseBookmarkBookmark = false;
        }

        private void CreaMenuLetture()
        {
            AggiungiLettureDaDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + "Letture" + Path.DirectorySeparatorChar);
            AggiungiLettureDaDirectory(Application.StartupPath + Path.DirectorySeparatorChar + "Letture" + Path.DirectorySeparatorChar);
            string[] cartelle = Settings.Default.CartelleDaCercare.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string cartella in cartelle)
                AggiungiLettureDaDirectory(cartella + Path.DirectorySeparatorChar + "Letture" + Path.DirectorySeparatorChar);

            if (schemiLettura.Count == 0)
            {
                readingsToolStripMenuItem.Visible = false;
                readingsToolStripButton.Visible = false;
            }
        }

        private void CreaMenuDisposizioni()
        {
            arrangementDeleteToolStripMenuItem.Enabled = false;
            AggiungiDisposizioniDaDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + "Disposizioni" + Path.DirectorySeparatorChar);
            AggiungiDisposizioniDaDirectory(Application.StartupPath + Path.DirectorySeparatorChar + "Disposizioni" + Path.DirectorySeparatorChar);
            string[] cartelle = Settings.Default.CartelleDaCercare.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string cartella in cartelle)
                AggiungiDisposizioniDaDirectory(cartella + Path.DirectorySeparatorChar + "Disposizioni" + Path.DirectorySeparatorChar);
        }

        private void CreaMenuVideo()
        {
            AggiungiVideoDaDirectory(Application.StartupPath + Path.DirectorySeparatorChar + "Video" + Path.DirectorySeparatorChar);
            AggiungiVideoDaDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + "Video" + Path.DirectorySeparatorChar);
            string[] cartelle = Settings.Default.CartelleDaCercare.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string cartella in cartelle)
                AggiungiVideoDaDirectory(cartella + Path.DirectorySeparatorChar + "Video" + Path.DirectorySeparatorChar);

            if (videoToolStripMenuItem.DropDownItems.Count == 1) // cioè c'è solo la voce "su Internet", nessun video è stato trovato sul computer
                tutorialsOnInternetToolStripMenuItem.Visible = true;

        }

        private void CreaMenuTestiParalleli()
        {
            string nomeCartella = "TestiParalleli";
            AggiungiTestiParalleliDaDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + nomeCartella + Path.DirectorySeparatorChar);
            AggiungiTestiParalleliDaDirectory(Application.StartupPath + Path.DirectorySeparatorChar + nomeCartella + Path.DirectorySeparatorChar);
            string[] cartelle = Settings.Default.CartelleDaCercare.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string cartella in cartelle)
                AggiungiTestiParalleliDaDirectory(cartella + Path.DirectorySeparatorChar + nomeCartella + Path.DirectorySeparatorChar);

            if (parallelTextsStripMenuItem.DropDownItems.Count == 0)
                parallelTextsStripMenuItem.Visible = false;
        }

        private void AggiungiSegnalibriDaDirectory(string directory)
        {
            if (!directory.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                directory += Path.DirectorySeparatorChar;
            if (!Directory.Exists(directory))
                return;

            string[] fileTrovati = Directory.GetFiles(directory, "*.xml");
            Array.Sort(fileTrovati);

            XmlNode nodoPrincipale;
            string linguaInterfaccia = Thread.CurrentThread.CurrentUICulture.ToString().ToLowerInvariant();
            if (linguaInterfaccia.Length >= 2)
                linguaInterfaccia = linguaInterfaccia.Substring(0, 2);

            foreach (string fileTrovato in fileTrovati)
            {
                try
                {
                    XmlDocument xd = new XmlDocument();
                    xd.Load(fileTrovato);
                    nodoPrincipale = xd.SelectSingleNode("bookmarks");
                    ToolStripMenuItem voce = new ToolStripMenuItem(InnerTextInLingua(nodoPrincipale, "name", linguaInterfaccia))
                    {
                        ToolTipText = InnerTextInLingua(nodoPrincipale, "description", linguaInterfaccia),
                        Tag = fileTrovato + "|" + InnerTextInLingua(nodoPrincipale, "version", linguaInterfaccia)
                    };
                    bookmarksToolStripMenuItem.DropDownItems.Insert(bookmarksToolStripMenuItem.DropDownItems.Count - 2, voce);

                    AggiungiSegnalibri(nodoPrincipale, linguaInterfaccia, voce);
                }
                catch
                {
                    // errore nell'XML, saltiamo il file
                }
            }
        }

        private void AggiungiSegnalibri(XmlNode nodo, string linguaInterfaccia, ToolStripMenuItem voceMenu)
        {
            XmlNodeList sottoNodi = nodo.SelectNodes("bookmark");
            ToolStripItem[] vociInSottoMenu = new ToolStripItem[sottoNodi.Count];
            int i = 0;
            foreach (XmlNode sottoNodo in sottoNodi)
            {
                ToolStripMenuItem sottoVoce = new ToolStripMenuItem(InnerTextInLingua(sottoNodo, "name", linguaInterfaccia), null, bookmark_Click)
                {
                    Tag = InnerTextInLingua(sottoNodo, "reference", linguaInterfaccia)
                };
                voceMenu.DropDownItems.Add(sottoVoce);
                AggiungiSegnalibri(sottoNodo, linguaInterfaccia, sottoVoce);
                vociInSottoMenu[i] = sottoVoce;
                ++i;
            }
            voceMenu.DropDownItems.AddRange(vociInSottoMenu);
        }

        private void AggiungiCollegamentiDaDirectory(string directory)
        {
            if (!directory.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                directory += Path.DirectorySeparatorChar;
            if (!Directory.Exists(directory))
                return;

            string[] fileTrovati = Directory.GetFiles(directory, "*.xml");
            Array.Sort(fileTrovati);
            Array.Reverse(fileTrovati); // così quando le voci sono inserite, saranno nell'ordine giusto
            string nome, type, url, parametri, image, category, language, shortcut, descrizione;
            KeysConverter kc = new KeysConverter();
            XmlNode nodePrincipale;
            XmlNode subNode;
            string linguaInterfaccia = Thread.CurrentThread.CurrentUICulture.ToString().ToLowerInvariant();
            if (linguaInterfaccia.Length >= 2)
                linguaInterfaccia = linguaInterfaccia.Substring(0, 2);

            foreach (string fileTrovato in fileTrovati)
            {
                try
                {
                    XmlDocument xd = new XmlDocument();
                    xd.Load(fileTrovato);
                    nodePrincipale = xd.SelectSingleNode("link");
                    InfoCollegamento collegamento = new InfoCollegamento
                    {
                        mappa = new Collection<CollegamentoMappaVoce>()
                    };

                    nome = InnerTextInLingua(nodePrincipale, "name", linguaInterfaccia);
                    descrizione = InnerTextInLingua(nodePrincipale, "description", linguaInterfaccia);
                    type = InnerTextInLingua(nodePrincipale, "type", linguaInterfaccia);
                    url = InnerTextInLingua(nodePrincipale, "url", linguaInterfaccia);
                    parametri = InnerTextInLingua(nodePrincipale, "parameters", linguaInterfaccia);
                    subNode = nodePrincipale.SelectSingleNode("map");
                    if (subNode != null)
                    {
                        XmlNodeList nodeMappa = subNode.SelectNodes("page");
                        foreach (XmlNode nodaMappa in nodeMappa)
                        {
                            CollegamentoMappaVoce voceMappa = new CollegamentoMappaVoce
                            {
                                inizio = nodaMappa.Attributes.GetNamedItem("start").Value,
                                fine = nodaMappa.Attributes.GetNamedItem("end").Value,
                                pagina = nodaMappa.InnerText
                            };
                            collegamento.mappa.Add(voceMappa);
                        }
                    }
                    subNode = nodePrincipale.SelectSingleNode("image");
                    image = (subNode == null ? "" : subNode.InnerText);
                    category = InnerTextInLingua(nodePrincipale, "category", linguaInterfaccia);
                    subNode = nodePrincipale.SelectSingleNode("language");
                    language = (subNode == null ? "" : subNode.InnerText);
                    shortcut = InnerTextInLingua(nodePrincipale, "shortcut", linguaInterfaccia);
                    collegamento.nomeFile = fileTrovato;
                    collegamento.descrizione = descrizione;
                    collegamento.versione = InnerTextInLingua(nodePrincipale, "version", linguaInterfaccia);
                    collegamento.url = url;
                    collegamento.parametri = parametri;
                    if (type.ToLowerInvariant() == "parola" || type.ToLowerInvariant() == "word")
                        collegamento.tipo = CollegamentoTipo.Parola;
                    else
                        collegamento.tipo = CollegamentoTipo.Riferimento;
                    collegamento.categoria = category;
                    collegamento.lingua = language;
                    collegamento.immagine = image;
                    collegamento.scorciatoia = shortcut;
                    Bitmap iconaDaImmagine = null;
                    Bitmap iconaCopiata = null;
                    try
                    {
                        iconaDaImmagine = new Bitmap(directory + image);
                        iconaCopiata = new Bitmap(iconaDaImmagine);
                    }
                    catch { } // file non trovato o formato invalido; andiamo avanti senza icona
                    ToolStripMenuItem voce = new ToolStripMenuItem(nome, iconaCopiata, externalLinkVoceToolStripMenuItem_Click);
                    if (iconaDaImmagine != null)
                        iconaDaImmagine.Dispose(); // facendo in questo modo permette al programma di chiudere il file che contiene l'immagine
                    shortcut = shortcut.ToUpperInvariant().Replace('-', '+');
                    shortcut = shortcut.Replace("CTRL+", "CONTROL+").Replace("MAIUS+", "SHIFT+").Replace("MAIUSC+", "SHIFT+");
                    shortcut = shortcut.Replace("CONTROL+", "Control+").Replace("ALT+", "Alt+").Replace("SHIFT+", "Shift+");
                    if (!string.IsNullOrEmpty(shortcut))
                    {
                        try
                        {
                            voce.ShortcutKeys = (Keys)(kc.ConvertFromInvariantString(shortcut));
                        }
                        catch { } // saltiamo Shortcuts illegali, senza saltare tutto il collegamento
                    }
                    voce.Tag = collegamento;
                    voce.Enabled = false;
                    voce.ToolTipText = descrizione;
                    if (string.IsNullOrEmpty(category))
                        externalLinkStripMenuItem.DropDownItems.Insert(0, voce);
                    else
                    {
                        bool categoriaTrovata = false;
                        foreach (ToolStripItem voceRicercaPerCategoria in externalLinkStripMenuItem.DropDownItems)
                        {
                            if (voceRicercaPerCategoria.Text == category)
                            {
                                ((ToolStripMenuItem)voceRicercaPerCategoria).DropDownItems.Insert(0, voce);
                                categoriaTrovata = true;
                                break;
                            }
                        }
                        if (!categoriaTrovata)
                        {
                            ToolStripMenuItem vocePerCategoria = new ToolStripMenuItem(category)
                            {
                                Enabled = false
                            };
                            vocePerCategoria.DropDownItems.Insert(0, voce);
                            externalLinkStripMenuItem.DropDownItems.Insert(externalLinkStripMenuItem.DropDownItems.Count - 2, vocePerCategoria);
                        }
                    }
                }
                catch
                {
                    // errore nell'XML, saltiamo il file
                }
            }
        }

        private void AggiungiParalleliDaDirectory(string directory)
        {
            if (!directory.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                directory += Path.DirectorySeparatorChar;
            if (!Directory.Exists(directory))
                return;

            string[] fileTrovati = Directory.GetFiles(directory, "*.xml");
            Array.Sort(fileTrovati);

            KeysConverter kc = new KeysConverter();
            XmlNode nodoPrincipale;
            XmlNode sottoNodo;
            string linguaInterfaccia = Thread.CurrentThread.CurrentUICulture.ToString().ToLowerInvariant();
            if (linguaInterfaccia.Length >= 2)
                linguaInterfaccia = linguaInterfaccia.Substring(0, 2);

            ToolStripItem[] vociSottoMenu = new ToolStripItem[fileTrovati.Length];
            int nFile = 0;
            foreach (string fileTrovato in fileTrovati)
            {
                InfoBraniParalleli info = new InfoBraniParalleli();
                try
                {
                    XmlDocument xd = new XmlDocument();
                    xd.Load(fileTrovato);
                    nodoPrincipale = xd.SelectSingleNode("parallels");

                    info.nome = InnerTextInLingua(nodoPrincipale, "name", linguaInterfaccia);
                    info.nomeFile = fileTrovato;
                    string descrizione = InnerTextInLingua(nodoPrincipale, "description", linguaInterfaccia);
                    string scorciatoia = InnerTextInLingua(nodoPrincipale, "shortcut", linguaInterfaccia);
                    info.versione = InnerTextInLingua(nodoPrincipale, "version", linguaInterfaccia);

                    sottoNodo = nodoPrincipale.SelectSingleNode("columns");
                    XmlNodeList colonne = sottoNodo.SelectNodes("column");
                    foreach (XmlNode colonna in colonne)
                        info.nomiColonne.Add(InnerTextInLingua(colonna, "name", linguaInterfaccia));
                    int numeroColonne = info.nomiColonne.Count;

                    sottoNodo = nodoPrincipale.SelectSingleNode("passages");
                    XmlNodeList brani = sottoNodo.SelectNodes("passage");
                    int numeroBrani = brani.Count;
                    XmlNodeList nodiTesti;
                    string[] riferimentiDelBrano;
                    int numeroColonna;
                    for (int i = 0; i < numeroBrani; ++i)
                    {
                        nodiTesti = brani[i].SelectNodes("columntext");
                        riferimentiDelBrano = new string[numeroColonne];
                        foreach (XmlNode nodo in nodiTesti)
                        {
                            numeroColonna = Convert.ToInt32(nodo.Attributes["column"].Value, CultureInfo.InvariantCulture) - 1;
                            if (numeroColonna >= 0 && numeroColonna < numeroColonne)
                            {
                                riferimentiDelBrano[numeroColonna] = testi.ConvertiRiferimentoDa3Numeri(nodo.InnerText);
                            }
                        }
                        InfoBranoParallelo infoBrano = new InfoBranoParallelo
                        {
                            titolo = InnerTextInLingua(brani[i], "title", linguaInterfaccia)
                        };
                        foreach (string riferimentoBrano in riferimentiDelBrano)
                            infoBrano.brani.Add(riferimentoBrano);
                        info.braniParalleli.Add(infoBrano);
                    }

                    ToolStripMenuItem voce = new ToolStripMenuItem(info.nome, null, paralleliVoceToolStripMenuItem_Click);
                    scorciatoia = scorciatoia.ToUpperInvariant().Replace('-', '+');
                    scorciatoia = scorciatoia.Replace("CTRL+", "CONTROL+").Replace("MAIUS+", "SHIFT+").Replace("MAIUSC+", "SHIFT+");
                    scorciatoia = scorciatoia.Replace("CONTROL+", "Control+").Replace("ALT+", "Alt+").Replace("SHIFT+", "Shift+");
                    if (!string.IsNullOrEmpty(scorciatoia))
                    {
                        try
                        {
                            voce.ShortcutKeys = (Keys)(kc.ConvertFromInvariantString(scorciatoia));
                        }
                        catch { } // saltiamo Shortcuts illegali, senza saltare tutto il collegamento
                    }
                    voce.ToolTipText = descrizione;
                    voce.Tag = info;
                    vociSottoMenu[nFile] = voce;
                    nFile++;
                    //                    parallelsToolStripMenuItem.DropDownItems.Add(voce);
                }
                catch
                {
                    // errore nell'XML, saltiamo il file
                }
            }
            ToolStripItem[] vociSottoMenuSenzaErrori = new ToolStripItem[nFile];
            Array.Copy(vociSottoMenu, vociSottoMenuSenzaErrori, nFile);
            parallelsToolStripMenuItem.DropDownItems.AddRange(vociSottoMenuSenzaErrori);
        }

        private void AggiungiLettureDaDirectory(string directory)
        {
            if (!directory.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                directory += Path.DirectorySeparatorChar;
            if (!Directory.Exists(directory))
                return;

            string[] fileTrovati = Directory.GetFiles(directory, "*.xml");
            Array.Sort(fileTrovati);

            XmlNode nodoPrincipale;
            string linguaInterfaccia = Thread.CurrentThread.CurrentUICulture.ToString().ToLowerInvariant();
            if (linguaInterfaccia.Length >= 2)
                linguaInterfaccia = linguaInterfaccia.Substring(0, 2);

            foreach (string fileTrovato in fileTrovati)
            {
                InfoLettura info = new InfoLettura();
                try
                {
                    XmlDocument xd = new XmlDocument();
                    xd.Load(fileTrovato);
                    nodoPrincipale = xd.SelectSingleNode("readings");

                    info.nome = InnerTextInLingua(nodoPrincipale, "name", linguaInterfaccia);
                    info.nomeFile = fileTrovato;
                    info.versione = InnerTextInLingua(nodoPrincipale, "version", linguaInterfaccia);
                    schemiLettura.Add(info);
                }
                catch
                {
                    // errore nell'XML, saltiamo il file
                }
            }
        }

        private void AggiungiDisposizioniDaDirectory(string directory)
        {
            if (!directory.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                directory += Path.DirectorySeparatorChar;
            if (!Directory.Exists(directory))
                return;

            string[] fileTrovati = Directory.GetFiles(directory, "*.xml");
            Array.Sort(fileTrovati);
            Array.Reverse(fileTrovati); // così l'ultimo è inserito prima a posizione 1, e gli altri lo spingono giù

            XmlNode nodoPrincipale;
            string linguaInterfaccia = Thread.CurrentThread.CurrentUICulture.ToString().ToLowerInvariant();
            if (linguaInterfaccia.Length >= 2)
                linguaInterfaccia = linguaInterfaccia.Substring(0, 2);

            foreach (string fileTrovato in fileTrovati)
            {
                InfoDisposizione info = new InfoDisposizione();
                try
                {
                    XmlDocument xd = new XmlDocument();
                    xd.Load(fileTrovato);
                    nodoPrincipale = xd.SelectSingleNode("windows");

                    info.nome = InnerTextInLingua(nodoPrincipale, "name", linguaInterfaccia);
                    info.nomeFile = fileTrovato;

                    ToolStripMenuItem voce = new ToolStripMenuItem(info.nome, null, arrangementToolStripMenuItem_Click)
                    {
                        Tag = info
                    };
                    arrangementMenu.DropDownItems.Insert(1, voce);
                    ToolStripMenuItem voceCancella = new ToolStripMenuItem(info.nome, null, arrangementDeleteToolStripMenuItem_Click)
                    {
                        Tag = info
                    };
                    arrangementDeleteToolStripMenuItem.DropDownItems.Insert(0, voceCancella);
                    arrangementDeleteToolStripMenuItem.Enabled = true;
                }
                catch
                {
                    // errore nell'XML, saltiamo il file
                }
            }
        }

        private void AggiungiVideoDaDirectory(string directory)
        {
            if (!directory.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                directory += Path.DirectorySeparatorChar;
            if (!Directory.Exists(directory))
                return;

            string[] fileTrovati = Directory.GetFiles(directory, "*.swf");
            Array.Sort(fileTrovati);
            ToolStripMenuItem voce;
            foreach (string fileTrovato in fileTrovati)
            {
                voce = new ToolStripMenuItem(Path.GetFileNameWithoutExtension(fileTrovato), null, videoToolStripMenuItem_Click)
                {
                    Tag = fileTrovato
                };
                videoToolStripMenuItem.DropDownItems.Add(voce);
            }
        }

        private void AggiungiTestiParalleliDaDirectory(string directory)
        {
            if (!directory.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                directory += Path.DirectorySeparatorChar;
            if (!Directory.Exists(directory))
                return;

            string[] fileTrovati = Directory.GetFiles(directory, "*.*");
            Array.Sort(fileTrovati);
            foreach (string fileTrovato in fileTrovati)
            {
                AggiungiTestiParalleliAlMenu(fileTrovato);
            }
        }

        internal void AggiungiTestiParalleliAlMenu(string fileTrovato)
        {
            ToolStripMenuItem voce = new ToolStripMenuItem(Path.GetFileNameWithoutExtension(fileTrovato), null, parallelBibbiaCommentarioList_Click)
            {
                Tag = fileTrovato
            };
            parallelTextsStripMenuItem.DropDownItems.Add(voce);
        }

        internal void RimuoviTestiParalleliDalMenu(string nomeGruppoParalleli)
        {
            foreach (ToolStripMenuItem voce in parallelTextsStripMenuItem.DropDownItems)
                if (voce.Text == nomeGruppoParalleli)
                    parallelTextsStripMenuItem.DropDownItems.Remove(voce);
        }

        private static string InnerTextInLingua(XmlNode nodoGenitore, string tag, string linguaInterfaccia)
        {
            string testoInLingua = "";
            XmlNodeList sottoNodi = nodoGenitore.SelectNodes(tag);
            foreach (XmlNode sottoNodo in sottoNodi)
            {
                if (sottoNodo.Attributes["language"] == null)
                {
                    if (string.IsNullOrEmpty(testoInLingua))
                        testoInLingua = sottoNodo.InnerText;
                }
                else if (sottoNodo.Attributes["language"].Value == linguaInterfaccia)
                    testoInLingua = sottoNodo.InnerText;
            }
            return testoInLingua;
        }

        #endregion

        private void MettiFontInBarra()
        {
            System.Drawing.Text.InstalledFontCollection fontInstallati = new System.Drawing.Text.InstalledFontCollection();
            FontFamily[] fonts = fontInstallati.Families;
            string nomeFont = "";
            fontToolStripComboBox.BeginUpdate();
            foreach (FontFamily font in fonts)
            {
                nomeFont = font.Name.Trim();
                if (!fontToolStripComboBox.Items.Contains(nomeFont))
                    fontToolStripComboBox.Items.Add(nomeFont);
            }
            fontToolStripComboBox.EndUpdate();
        }

        private void Principale_Shown(object sender, EventArgs e)
        {
            // questi valori sono usati solo quando non c'è un'ultima disposizione
            // cioè la prima volta che il programma è avviato, e quando ci sono argomenti della riga di comando

            WindowState = Settings.Default.PrincipaleWindowState;
            if (WindowState != FormWindowState.Maximized)
            {
                // necessario mettere i valori predefiniti qui invece del file settings.settings
                // perché Mono 1.2.4 non può convertire il valore 640, 487 ad un Size
                try
                {
                    if (Settings.Default.PrincipaleWindowSize == null)
                        Size = new Size(640, 487);
                    else
                        Size = Settings.Default.PrincipaleWindowSize;
                }
                catch (NullReferenceException)
                {
                    Size = new Size(640, 487);
                }

                try
                {
                    if (Settings.Default.PrincipaleWindowLocation == null)
                        Location = new Point(0, 0);
                    else
                        Location = Settings.Default.PrincipaleWindowLocation;
                }
                catch (NullReferenceException)
                {
                    Location = new Point(0, 0);
                }
            }

            #region CommandLine e disposizione finestre

            string[] argomenti = Environment.GetCommandLineArgs();
            string versione;
            for (int i = 1; i < argomenti.Length; ++i) // da 1, perché argomenti[0] contiene il nome del file eseguibile
            {
                if (argomenti[i].StartsWith("-", StringComparison.Ordinal))
                    argomenti[i] = argomenti[i].Remove(0, 1);
                if (string.Compare(argomenti[i], "readings", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(argomenti[i], "letture", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    readingsToolStripMenuItem_Click(this, null);
                }
                else
                {
                    versione = Path.GetFileName(argomenti[i]);
                    versione = (versione.EndsWith(".laparola", StringComparison.OrdinalIgnoreCase) ? versione.Substring(0, versione.Length - 9) : versione);
                    TestoTipi tipo = testi.Info(versione).Tipo;
                    if (((tipo & TestoTipi.Bibbia) == TestoTipi.Bibbia) || ((tipo & TestoTipi.Commentario) == TestoTipi.Commentario))
                        VisualizzaTesto(versione, TestoTipi.Commentario);
                    else if ((tipo & TestoTipi.Dizionario) == TestoTipi.Dizionario)
                    {
                        ApriApriNota(versione, 1);
                        //VisualizzaTesto(nomeVersione, TestoTipi.Dizionario);
                    }
                }
            }
            if (MdiChildren.Length == 1)
                MdiChildren[0].WindowState = FormWindowState.Maximized;

            // usare la disposizione finestre solo se nessuna finestra era aperta dalla riga di comando
            if (MdiChildren.Length == 0)
            {
                if (Settings.Default.MiscArrangementDefaultType == 0)
                {
                    CaricaDisposizioneFinestre(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + "Disposizioni" + Path.DirectorySeparatorChar + "disposizione all'uscita del programma");
                }
                // ==1 è la disposizione vuota, quindi non serve fare nulla
                else if (Settings.Default.MiscArrangementDefaultType == 2)
                {
                    string disposizione = Settings.Default.MiscArrangementDefault;
                    foreach (ToolStripItem voce in arrangementDeleteToolStripMenuItem.DropDownItems)
                    {
                        if (voce.Text == disposizione)
                        {
                            arrangementToolStripMenuItem_Click(voce, null);
                            break;
                        }
                    }
                }
            }

            #endregion

            string nomeFileAggiornamento = Application.StartupPath + Path.DirectorySeparatorChar + "Update.exe";
            if (!File.Exists(nomeFileAggiornamento))
            {
                updateToolStripButton.Visible = false;
                updateToolStripMenuItem.Visible = false;
                updateToolStripSeparator.Visible = false;
            }

            if (Settings.Default.PrincipaleClipboardAttivo)
            {
                testoInClipboard = Clipboard.GetText();
                timerClipboard.Interval = Settings.Default.PrincipaleClipboardTempo;
                timerClipboard.Enabled = true;
            }

            splashScreen.Close();
            Application.DoEvents();
            splashScreen = null;

            if (!Settings.Default.AggiornamentoManuale && updateToolStripButton.Visible)
            {
                TimeSpan ts = DateTime.Now - Settings.Default.AggiornamentoUltimo;
                if (ts.Days >= Settings.Default.AggiornamentoGiorni)
                {
                    try
                    {
                        CercaAggiornamenti(0);
                    }
                    catch (Exception eccezione)
                    {
                        MessageBox.Show(string.Format(CultureInfo.CurrentCulture, LocRM.GetString("MainUpdateNotConnected"), eccezione.Message), LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, messageBoxOptions);
                    }
                }
            }

            BackgroundWorker backgroundWorker = new BackgroundWorker();
            if (isRunningOnMono)
            {
                CaricaInformazioniAddizionali(null, null);
            }
            else
            {
                backgroundWorker.DoWork += new DoWorkEventHandler(CaricaInformazioniAddizionali);
                backgroundWorker.RunWorkerAsync();
            }

        }

        private void CaricaInformazioniAddizionali(object sender, DoWorkEventArgs e)
        {
            testi.CaricaInformazioniAddizionali();
        }

        private void CopiaMenu(ToolStripItemCollection collectionDa, ToolStripItemCollection collectionA)
        {
            foreach (ToolStripItem voce in collectionDa)
            {
                if (voce.GetType().Name == "ToolStripMenuItem")
                {
                    ToolStripMenuItem nuovaVoce = new ToolStripMenuItem(voce.Text, null, HighlightClick)
                    {
                        Tag = voce.Tag,
                        ForeColor = voce.ForeColor,
                        BackColor = voce.BackColor,
                        Enabled = voce.Enabled
                    };
                    //                    nuovaVoce.Visible = voce.Visible; non funziona, perché voce.Visible è sempre falso se il menu non è aperto
                    CopiaMenu(((ToolStripMenuItem)voce).DropDownItems, nuovaVoce.DropDownItems);
                    collectionA.Add(nuovaVoce);
                }
                else
                {
                    collectionA.Add(new ToolStripSeparator());
                }
            }
        }

        #endregion Startup

        #region Menu

        #region Menu File

        private void ShowNewForm(object sender, EventArgs e)
        {
            Editor formEditor = new Editor(this)
            {
                MdiParent = this,
                Text = LocRM.GetString("MainNewEditorCaption") + " " + childFormNumber++
            };
            formEditor.Show();
        }

        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            string ultimaDirectory = Settings.Default.UltimaDirectory;
            if (String.IsNullOrEmpty(ultimaDirectory))
                ultimaDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            openFileDialog.InitialDirectory = ultimaDirectory;
            openFileDialog.Filter = LocRM.GetString("EditorSaveFilter");
            openFileDialog.Multiselect = true;
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                ApriDiversiFile(openFileDialog.FileNames);
            openFileDialog.Dispose();
        }

        private void ApriDiversiFile(string[] fileDaAprire)
        {
            if (fileDaAprire.Length > 0)
            {
                Settings.Default.UltimaDirectory = Path.GetDirectoryName(fileDaAprire[0]);
                foreach (string nomeFile in fileDaAprire)
                    ApriFile(nomeFile);
            }
        }

        internal void ApriFile(string nomeFile)
        {
            Editor formEditor = new Editor(this, nomeFile)
            {
                MdiParent = this
            };
            formEditor.Show();
            formEditor.rtEditor.MostraLink();
            AggiornaPulsanti(formEditor.rtEditor);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null)
                ActiveMdiChild.Close();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ((Editor)ActiveMdiChild).SalvaFile(((Editor)ActiveMdiChild).NomeFile);
            }
            catch
            {
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ((Editor)ActiveMdiChild).SalvaCome();
            }
            catch
            {
            }
        }

        private void importBibleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ImportaBibbia ib = new ImportaBibbia(this, TipoImportazione.ImportaBibbia))
            {
                ib.ShowDialog();
            }
        }

        private void importZefaniaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ImportaBibbia ib = new ImportaBibbia(this, TipoImportazione.ImportaZefania))
            {
                ib.ShowDialog();
            }
        }

        private void importBibleworksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ImportaBibbia ib = new ImportaBibbia(this, TipoImportazione.ImportaBibleworks))
            {
                ib.ShowDialog();
            }
        }

        private void importNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ImportaBibbia ib = new ImportaBibbia(this, TipoImportazione.ImportaNote))
            {
                ib.ShowDialog();
            }
        }

        private void importThMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ImportaBibbia ib = new ImportaBibbia(this, TipoImportazione.ImportaThml))
            {
                ib.ShowDialog();
            }
        }

        private void importEswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ImportaBibbia ib = new ImportaBibbia(this, TipoImportazione.ImportaEsword))
            {
                ib.ShowDialog();
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!TrovaForm("Esporta"))
            {
                Esporta formEsporta = new Esporta(this)
                {
                    MdiParent = this
                };
                formEsporta.Show();
            }
        }

        private bool TrovaForm(string tag)
        {
            bool trovato = false;
            foreach (Form formFiglio in MdiChildren)
            {
                if (formFiglio.Tag != null && formFiglio.Tag.ToString() == tag)
                {
                    formFiglio.Activate();
                    trovato = true;
                }
            }
            return trovato;
        }

        #region stampa

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null)
            {
                switch (ActiveMdiChild.Tag.ToString())
                {
                    case "Editor":
                        StampaRichText(((Editor)ActiveMdiChild).rtEditor);
                        break;
                    case "Visualizza":
                        ((Visualizza)ActiveMdiChild).StampaSelezione();
                        break;
                    case "BraniParalleli":
                        try
                        {
                            int len = ((BraniParalleli)ActiveMdiChild).UltimaRtb.SelectionStart + ((BraniParalleli)ActiveMdiChild).UltimaRtb.SelectionLength - 1;
                            // in Light è necessario usare una variable intermedia len; usiamo anche qui per essere sicuri
                            StampaRichText(((BraniParalleli)ActiveMdiChild).UltimaRtb, ((BraniParalleli)ActiveMdiChild).UltimaRtb.SelectionStart, len);
                        }
                        catch
                        {
                            // UltimaRtb potrebbe essere null
                        }
                        break;
                    case "Lettura":
                        try
                        {
                            int len = ((Lettura)ActiveMdiChild).UltimaRtb.SelectionStart + ((Lettura)ActiveMdiChild).UltimaRtb.SelectionLength - 1;
                            // in Light è necessario usare una variable intermedia len; usiamo anche qui per essere sicuri
                            StampaRichText(((Lettura)ActiveMdiChild).UltimaRtb, ((Lettura)ActiveMdiChild).UltimaRtb.SelectionStart, len);
                        }
                        catch
                        {
                            // UltimaRtb potrebbe essere null
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Stampa tutto il testo in un RichText box.
        /// </summary>
        /// <param name="rtb">Il controllo che contiene il testo da stampare.</param>
        public void StampaRichText(RichTextBoxEx rtb)
        {
            if (rtb == null)
                throw new ArgumentNullException("rtb");
            else
            {
                int len = rtb.Text.Length;
                // in Light è necessario usare una variable intermedia len; usiamo anche qui per essere sicuri
                StampaRichText(rtb, 0, len);
            }
        }

        /// <summary>
        /// Stampa una parte di un RichText box.
        /// </summary>
        /// <param name="rtb">Il controllo che contiene il testo da stampare.</param>
        /// <param name="inizio">Il numero del primo carattere da stampare.</param>
        /// <param name="fine">Il numero dell'ultimo carattere da stampare.</param>
        public void StampaRichText(RichTextBoxEx rtb, int inizio, int fine)
        { // anche in Light
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

        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null && ActiveMdiChild.Tag.ToString() == "Editor")
            {
                rtbPerStampa = ((Editor)(ActiveMdiChild)).rtEditor;
                primoCarattereSullaPagina = 0;
                ultimoCarattereDaStampare = rtbPerStampa.Text.Length;

                if (storedPageSettings != null)
                    printDocument.DefaultPageSettings = storedPageSettings;

                PrintPreviewDialog ppd = new PrintPreviewDialog();
                try
                {
                    ppd.ShowIcon = false;
                    ppd.Document = printDocument;
                    ppd.ShowDialog();
                }
                finally
                {
                    ppd.Dispose();
                }
            }
        }

        private void printSetupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null && ActiveMdiChild.Tag.ToString() == "Editor")
            {
                PageSetupDialog psd = new PageSetupDialog();
                try
                {
                    if (storedPageSettings == null)
                        storedPageSettings = new PageSettings();
                    psd.PageSettings = storedPageSettings;
                    psd.PrinterSettings = new PrinterSettings();
                    psd.Document = printDocument;
                    //--- If Microsoft fix the bug in VS2005 it should be
                    //psd.ShowDialog()

                    //--- Fix PageSetupDialog bug temporary
                    if (System.Globalization.RegionInfo.CurrentRegion.IsMetric)
                    {
                        psd.PageSettings.Margins.Top = Convert.ToInt32(2.54 * psd.PageSettings.Margins.Top);
                        psd.PageSettings.Margins.Bottom = Convert.ToInt32(2.54 * psd.PageSettings.Margins.Bottom);
                        psd.PageSettings.Margins.Left = Convert.ToInt32(2.54 * psd.PageSettings.Margins.Left);
                        psd.PageSettings.Margins.Right = Convert.ToInt32(2.54 * psd.PageSettings.Margins.Right);
                    }
                    if (psd.ShowDialog() != DialogResult.OK && System.Globalization.RegionInfo.CurrentRegion.IsMetric)
                    {
                        psd.PageSettings.Margins.Top = Convert.ToInt32(psd.PageSettings.Margins.Top / 2.54);
                        psd.PageSettings.Margins.Bottom = Convert.ToInt32(psd.PageSettings.Margins.Bottom / 2.54);
                        psd.PageSettings.Margins.Left = Convert.ToInt32(psd.PageSettings.Margins.Left / 2.54);
                        psd.PageSettings.Margins.Right = Convert.ToInt32(psd.PageSettings.Margins.Right / 2.54);
                    }
                }
                finally
                {
                    psd.Dispose();
                }
            }
        }

        #endregion

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #endregion

        #region Menu Modifica

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ((Editor)ActiveMdiChild).rtEditor.Undo();
            }
            catch
            {
            }
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ((Editor)ActiveMdiChild).rtEditor.Redo();
            }
            catch
            {
            }
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ((Editor)ActiveMdiChild).rtEditor.Cut();
            }
            catch
            {
            }
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null)
            {
                switch (ActiveMdiChild.Tag.ToString())
                {
                    case "Editor":
                        ((Editor)ActiveMdiChild).rtEditor.CopiaSenzaTestoNascosto();
                        break;
                    case "Visualizza":
                        ((Visualizza)ActiveMdiChild).CopiaSelezione();
                        break;
                    case "BraniParalleli":
                        try
                        {
                            ((BraniParalleli)ActiveMdiChild).UltimaRtb.CopiaSenzaTestoNascosto();
                        }
                        catch
                        {
                            // UltimaRtb potrebbe essere null
                        }
                        break;
                    case "Lettura":
                        try
                        {
                            ((Lettura)ActiveMdiChild).UltimaRtb.CopiaSenzaTestoNascosto();
                        }
                        catch
                        {
                            // UltimaRtb potrebbe essere null
                        }
                        break;
                }
            }
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ((Editor)ActiveMdiChild).rtEditor.Paste();
            }
            catch
            {
            }
        }

        private void deleteStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ((Editor)ActiveMdiChild).rtEditor.SelectedText = "";
            }
            catch
            {
            }
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ((Editor)ActiveMdiChild).rtEditor.SelectAll();
            }
            catch
            {
            }
        }

        private void findAgainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(trovaTesto))
                findToolStripMenuItem_Click(sender, e);
            else if (ActiveMdiChild != null && ActiveMdiChild.Tag.ToString() == "Editor")
                TrovaInRichText(((Editor)ActiveMdiChild).rtEditor, trovaTesto, trovaOpzioni);
        }

        private int TrovaInRichText(RichTextBoxEx rtb, string testoDaTrovare, RichTextBoxFinds opzioni)
        {
            int posizioneTestoTrovato;
            bool modificato = rtb.Modified;
            int posizioneDaDoveCominciare = rtb.SelectionStart, selezioneInizio = rtb.SelectionStart, selezioneLunghezza = rtb.SelectionLength;
            bool primaVolta = true;
            do
            {
                if (rtb.SelectionLength > 0 && string.Compare(rtb.SelectedText, testoDaTrovare, StringComparison.OrdinalIgnoreCase) == 0)
                    posizioneTestoTrovato = rtb.Find(testoDaTrovare, posizioneDaDoveCominciare + (primaVolta ? 1 : 0), opzioni);
                else
                    posizioneTestoTrovato = rtb.Find(testoDaTrovare, posizioneDaDoveCominciare, opzioni);
                if (posizioneTestoTrovato > -1)
                {
                    posizioneDaDoveCominciare = posizioneTestoTrovato + 1;
                    posizioneTestoTrovato = AnnullaTestoTrovatoInTestoNascosto(rtb.Text, posizioneTestoTrovato);
                }
                primaVolta = false;
            } while (posizioneTestoTrovato == -2);
            if (posizioneTestoTrovato == -1)
            {
                posizioneDaDoveCominciare = 0;
                do
                {
                    posizioneTestoTrovato = rtb.Find(testoDaTrovare, posizioneDaDoveCominciare, opzioni);
                    if (posizioneTestoTrovato >= 0)
                    {
                        posizioneDaDoveCominciare = posizioneTestoTrovato + 1;
                        posizioneTestoTrovato = AnnullaTestoTrovatoInTestoNascosto(rtb.Text, posizioneTestoTrovato);
                    }
                } while (posizioneTestoTrovato == -2);
            }
            if (posizioneTestoTrovato == -1)
            {
                rtb.Select(selezioneInizio, selezioneLunghezza);
                MessageBox.Show(String.Format(CultureInfo.CurrentCulture, Principale.LocRM.GetString("EditorFindNotFound"), trovaTesto), Principale.LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions);
            }
            rtb.Modified = modificato; // rtb.Find imposta Modified = true, quindi mettiamo il valore come era
            return posizioneTestoTrovato;
        }

        private static int AnnullaTestoTrovatoInTestoNascosto(string testoCercato, int posizioneTestoTrovato)
        {
            // i riferimenti, nomi di note e nomi di file che a cui ci sono collegamenti ipertestuali non devono essere inclusi nella ricerca
            if (testoCercato.IndexOf(RichTextBoxEx.FineLink2, posizioneTestoTrovato) > -1 && testoCercato.LastIndexOf(RichTextBoxEx.FineLink1, posizioneTestoTrovato) > -1)
            {
                if (testoCercato.IndexOf(RichTextBoxEx.FineLink2, posizioneTestoTrovato) < testoCercato.IndexOf(RichTextBoxEx.InizioLink, posizioneTestoTrovato) || testoCercato.IndexOf(RichTextBoxEx.InizioLink, posizioneTestoTrovato) < 0)
                    if (testoCercato.LastIndexOf(RichTextBoxEx.FineLink1, posizioneTestoTrovato) > testoCercato.LastIndexOf(RichTextBoxEx.FineLink2, posizioneTestoTrovato) || testoCercato.LastIndexOf(RichTextBoxEx.FineLink2, posizioneTestoTrovato) < 0)
                        posizioneTestoTrovato = -2;
            }
            return posizioneTestoTrovato;
        }

        private void SostituisciInRichText(RichTextBoxEx rtb, string testoDaTrovare, string testoDaSostituire, RichTextBoxFinds opzioni)
        {
            if (TrovaInRichText(rtb, testoDaTrovare, opzioni) >= 0)
            {
                rtb.SelectedText = testoDaSostituire;
            }
        }

        private void replaceAgainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(trovaTesto) || String.IsNullOrEmpty(sostituisciTesto))
                findToolStripMenuItem_Click(sender, e);
            if (ActiveMdiChild != null && ActiveMdiChild.Tag.ToString() == "Editor")
                SostituisciInRichText(((Editor)ActiveMdiChild).rtEditor, trovaTesto, sostituisciTesto, trovaOpzioni);
        }

        private void hypertextJumpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null)
            {
                switch (ActiveMdiChild.Tag.ToString())
                {
                    case "Editor":
                        ((Editor)ActiveMdiChild).SaltoIpertestuale();
                        break;
                    case "Visualizza":
                        ((Visualizza)ActiveMdiChild).SaltoIpertestuale();
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #region Menu Visualizza

        #region visualizza testo

        private void VisualizzaBibbiaClick(object sender, EventArgs e)
        {
            if (sender == null)
                throw new ArgumentNullException("sender");
            else
                VisualizzaTesto(((ToolStripMenuItem)sender).Text, TestoTipi.Bibbia);
        }

        private void VisualizzaCommentarioClick(object sender, EventArgs e)
        {
            if (sender == null)
                throw new ArgumentNullException("sender");
            else
                VisualizzaTesto(((ToolStripMenuItem)sender).Text, TestoTipi.Commentario);
        }

        private void VisualizzaDizionarioClick(object sender, EventArgs e)
        {
            if (sender == null)
                throw new ArgumentNullException("sender");
            else
                VisualizzaTesto(((ToolStripMenuItem)sender).Text, TestoTipi.Dizionario);
        }

        private void parallelTextsStripMenuItem_Click(object sender, EventArgs e)
        {
            using (TestiParalleli fTestiParalleli = new TestiParalleli(this))
            {
                fTestiParalleli.ShowDialog();
                VisualizzaParalleli(fTestiParalleli.testi, fTestiParalleli.tipiTestiSelezionati, true);
            }
        }

        private void parallelBibbiaCommentarioList_Click(object sender, EventArgs e)
        {
            parallelBibbiaODictionaryList_Click(sender, true);
        }

        private void parallelDictionaryList_Click(object sender, EventArgs e)
        {
            parallelBibbiaODictionaryList_Click(sender, false);
        }

        private void parallelBibbiaODictionaryList_Click(object sender, bool bibbiaCommentario)
        {
            string[] testi;
            string nomeFile = ((ToolStripMenuItem)sender).Tag.ToString();
            try
            {
                testi = File.ReadAllLines(nomeFile, System.Text.Encoding.UTF8);
                VisualizzaParalleli(new List<string>(testi), bibbiaCommentario);
            }
            catch { } // per esempio, il file non esiste più
        }

        internal Visualizza VisualizzaParalleli(List<string> testi, bool bibbiaCommentario)
        {
            List<TestoTipi> tipi = new List<TestoTipi>(testi.Count);
            for (int i = 0; i < testi.Count; ++i)
            {
                int p = testi[i].IndexOf('#');
                if (p > 0)
                {
                    if (testi[i].Substring(p + 1) == "b")
                        tipi.Add(TestoTipi.Bibbia);
                    else if (testi[i].Substring(p + 1) == "c")
                        tipi.Add(TestoTipi.Commentario);
                    else if (testi[i].Substring(p + 1) == "d")
                        tipi.Add(TestoTipi.Dizionario);
                    else
                        tipi.Add(TestoTipi.None);
                    testi[i] = testi[i].Remove(p);
                }
                else
                    tipi.Add(TestoTipi.None);
            }
            return VisualizzaParalleli(testi, tipi, bibbiaCommentario);
        }

        internal Visualizza VisualizzaParalleli(List<string> testi, List<TestoTipi> tipi, bool bibbiaCommentario)
        {
            Visualizza formVisualizza = null;
            TestoTipi tipo;
            bool primoTestoVisualizzato = true;

            for (int i = 0; i < testi.Count; ++i)
            {
                tipo = (i < tipi.Count ? tipi[i] : TestoTipi.None);
                if (tipo == TestoTipi.None)
                {
                    if ((Principale.testi.Info(testi[i]).Tipo & TestoTipi.Bibbia) == TestoTipi.Bibbia)
                        tipo = TestoTipi.Bibbia;
                    else if ((Principale.testi.Info(testi[i]).Tipo & TestoTipi.Commentario) == TestoTipi.Commentario)
                        tipo = TestoTipi.Commentario;
                    else if ((Principale.testi.Info(testi[i]).Tipo & TestoTipi.Dizionario) == TestoTipi.Dizionario)
                        tipo = TestoTipi.Dizionario;
                    if (!bibbiaCommentario) // se non è specificamente Bibbia o commentario, il dizionario ha preferenza
                    {
                        if ((Principale.testi.Info(testi[i]).Tipo & TestoTipi.Dizionario) == TestoTipi.Dizionario)
                            tipo = TestoTipi.Dizionario;
                    }
                }
                // se il testo non esiste, non sarà visualizzato
                if (tipo != TestoTipi.None)
                {
                    if (primoTestoVisualizzato)
                    {
                        formVisualizza = VisualizzaTesto(testi[i], tipo);
                        primoTestoVisualizzato = false;
                    }
                    else
                        formVisualizza.AggiungiPane(testi[i], tipo);
                }
            }
            if (!primoTestoVisualizzato)
                formVisualizza.ImpostaSincronizzato(true);

            return formVisualizza;
        }

        private void VisualizzaLibroClick(object sender, EventArgs e)
        {
            if (sender == null)
                throw new ArgumentNullException("sender");
            else
                VisualizzaTesto(((ToolStripMenuItem)sender).Text, TestoTipi.Libro);
        }

        private void VisualizzaTutteBibbie_Click(object sender, EventArgs e)
        {
            Visualizza formVisualizza = new Visualizza(this, TestoTipi.Bibbia)
            {
                MdiParent = this
            };
            formVisualizza.Show();
        }

        private void VisualizzaTuttiCommentari_Click(object sender, EventArgs e)
        {
            Visualizza formVisualizza = new Visualizza(this, TestoTipi.Commentario)
            {
                MdiParent = this
            };
            formVisualizza.Show();
        }

        private void VisualizzaTuttiDizionari_Click(object sender, EventArgs e)
        {
            Visualizza formVisualizza = new Visualizza(this, TestoTipi.Dizionario)
            {
                MdiParent = this
            };
            formVisualizza.Show();
        }

        public Visualizza VisualizzaTesto(string versione, TestoTipi tipo, Visualizza formVisualizza)
        {
            switch (tipo)
            {
                case TestoTipi.Bibbia:
                    if (formVisualizza == null)
                    {
                        formVisualizza = new Visualizza(this, versione, TestoTipi.Bibbia)
                        {
                            MdiParent = this
                        };
                    }
                    formVisualizza.Show();
                    break;
                case TestoTipi.Commentario:
                    if (formVisualizza == null)
                    {
                        formVisualizza = new Visualizza(this, versione, TestoTipi.Commentario)
                        {
                            MdiParent = this
                        };
                    }
                    formVisualizza.Show();
                    break;
                case TestoTipi.Dizionario:
                    //                    ApriApriNota(nomeVersione, 1);
                    if (formVisualizza == null)
                    {
                        formVisualizza = new Visualizza(this, versione, TestoTipi.Dizionario)
                        {
                            MdiParent = this
                        };
                    }
                    formVisualizza.Show();
                    break;
                case TestoTipi.Libro:
                    ApriApriNota(versione, 2);
                    break;
            }
            return formVisualizza;
        }

        public Visualizza VisualizzaTesto(string versione, TestoTipi tipo)
        {
            return VisualizzaTesto(versione, tipo, null);
        }

        public Visualizza VisualizzaDizionario(string versione)
        {
            Visualizza formVisualizza = new Visualizza(this, versione, TestoTipi.Dizionario)
            {
                MdiParent = this
            };
            formVisualizza.Show();
            return formVisualizza;
        }

        private void bibleToolStripButton_ButtonClick(object sender, EventArgs e)
        {
            bibleToolStripButton.ShowDropDown();
        }

        private void browseZoomToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ActiveMdiChild.Tag != null && ActiveMdiChild.Tag.ToString() == "Visualizza" && ((Visualizza)ActiveMdiChild) != null)
            {
                string zoom = browseZoomToolStripComboBox.Text;
                if (zoom.EndsWith("%", StringComparison.Ordinal))
                    zoom = zoom.Substring(0, zoom.Length - 1);
                Single zoomFactor = Convert.ToSingle(zoom, CultureInfo.InvariantCulture) / 100.0F;
                if (zoomFactor < 64.0 && zoomFactor > 1.0 / 64.0)
                    ((Visualizza)ActiveMdiChild).paneAttivo.Zoom = zoomFactor;
            }
        }

        private void browseZoomToolStripComboBox_Leave(object sender, EventArgs e)
        {
            browseZoomToolStripComboBox_SelectedIndexChanged(sender, e);
        }

        private void browseZoomToolStripComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                browseZoomToolStripComboBox_SelectedIndexChanged(sender, e);
        }

        #endregion

        private void zoomToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem tsm in zoomToolStripMenuItem.DropDownItems)
                tsm.Checked = false;

            float zoom = 0;
            if (ActiveMdiChild != null)
            {
                if (ActiveMdiChild.Tag != null && ActiveMdiChild.Tag.ToString() == "Visualizza")
                    zoom = ((Visualizza)ActiveMdiChild).paneAttivo.Zoom;
                if (ActiveMdiChild.Tag != null && ActiveMdiChild.Tag.ToString() == "Editor")
                    zoom = ((Editor)ActiveMdiChild).rtEditor.ZoomFactor;
            }
            int iZoom = Convert.ToInt32(100 * zoom + 0.1);
            switch (iZoom)
            {
                case 10:
                    zoom010ToolStripMenuItem.Checked = true;
                    break;
                case 25:
                    zoom025ToolStripMenuItem.Checked = true;
                    break;
                case 50:
                    zoom050ToolStripMenuItem.Checked = true;
                    break;
                case 75:
                    zoom075ToolStripMenuItem.Checked = true;
                    break;
                case 100:
                    zoom100ToolStripMenuItem.Checked = true;
                    break;
                case 150:
                    zoom150ToolStripMenuItem.Checked = true;
                    break;
                case 200:
                    zoom200ToolStripMenuItem.Checked = true;
                    break;
                case 500:
                    zoom500ToolStripMenuItem.Checked = true;
                    break;
            }
        }

        #region segnalibri

        private void bookmarksQuickGoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int numero = Convert.ToInt32(((ToolStripMenuItem)sender).Tag.ToString(), CultureInfo.InvariantCulture);
            string[] segnalibri = Settings.Default.SegnalibriVeloci.Split(new char[] { '|' }, StringSplitOptions.None);
            Riferimento riferimento = testi.ConvertiRiferimento(testi.ConvertiRiferimentoDa3Numeri(segnalibri[numero]));
            Visualizza formVisualizza = null;
            if (ActiveMdiChild != null && ActiveMdiChild.Tag != null && ActiveMdiChild.Tag.ToString() == "Visualizza")
                formVisualizza = (Visualizza)ActiveMdiChild;
            else
                formVisualizza = VisualizzaTesto(testi.UltimaBibbia, TestoTipi.Bibbia);
            formVisualizza.SpostaTesto(testi.ConvertiDaStandard(riferimento, formVisualizza.paneAttivo.Versione), true);
        }

        private void bookmarksQuickSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string riferimentoComeTesto = "";
            if (ActiveMdiChild != null && ActiveMdiChild.Tag != null && ActiveMdiChild.Tag.ToString() == "Visualizza")
                riferimentoComeTesto = ((Visualizza)ActiveMdiChild).paneAttivo.PostoAttuale;
            if (string.IsNullOrEmpty(riferimentoComeTesto))
            {
                using (InputBox inputBox = new InputBox(LocRM.GetString("MainQuickBookmarkSetCaption"), LocRM.GetString("MainQuickBookmarkSetQuestion"), riferimentoComeTesto))
                {
                    inputBox.ShowDialog();
                    riferimentoComeTesto = inputBox.Risposta;
                }
            }
            string[] segnalibri = Settings.Default.SegnalibriVeloci.Split(new char[] { '|' }, StringSplitOptions.None);
            int numero = Convert.ToInt32(((ToolStripMenuItem)sender).Tag.ToString(), CultureInfo.InvariantCulture);
            Riferimento riferimento = testi.ConvertiRiferimento(riferimentoComeTesto);
            if (ActiveMdiChild != null && ActiveMdiChild.Tag != null && ActiveMdiChild.Tag.ToString() == "Visualizza")
                riferimento = testi.ConvertiAStandard(riferimento, ((Visualizza)ActiveMdiChild).paneAttivo.Versione);
            if (numero < segnalibri.Length && riferimento.Count > 0)
                segnalibri[numero] = riferimento.Brani[0][0].ToString(CultureInfo.InvariantCulture) + " " + riferimento.Brani[0][1].ToString(CultureInfo.InvariantCulture) + ":" + riferimento.Brani[0][2].ToString(CultureInfo.InvariantCulture);
            Settings.Default.SegnalibriVeloci = string.Join("|", segnalibri);
        }

        private void bookmarksQuickGoToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            string[] segnalibri = Settings.Default.SegnalibriVeloci.Split(new char[] { '|' }, StringSplitOptions.None);
            for (int i = 0; i < bookmarksQuickGoToolStripMenuItem.DropDownItems.Count; ++i)
            {
                if (i >= segnalibri.Length || string.IsNullOrEmpty(testi.ConvertiRiferimentoDa3Numeri(segnalibri[i])))
                    bookmarksQuickGoToolStripMenuItem.DropDownItems[i].Enabled = false;
                else
                {
                    bookmarksQuickGoToolStripMenuItem.DropDownItems[i].Text = bookmarksQuickGoToolStripMenuItem.DropDownItems[i].Text.Substring(0, 2) + " " + testi.ConvertiRiferimentoDa3Numeri(segnalibri[i]);
                    bookmarksQuickGoToolStripMenuItem.DropDownItems[i].Enabled = true;
                }
            }
        }

        private void bookmarksEditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!TrovaForm("Segnalibri"))
            {
                Segnalibri formSegnalibri = new Segnalibri(this)
                {
                    MdiParent = this
                };
                formSegnalibri.Show();
            }
        }

        private void bookmark_Click(object sender, EventArgs e)
        {
            string voceTag = ((ToolStripItem)(sender)).Tag.ToString();
            if (ActiveMdiChild != null && ActiveMdiChild.Tag != null && ActiveMdiChild.Tag.ToString() == "Visualizza")
                MostraSegnalibro(voceTag, (Visualizza)ActiveMdiChild);
            else
                MostraSegnalibro(voceTag);
        }

        public Visualizza MostraSegnalibro(string riferimento, Visualizza formVisualizza)
        {
            if (riferimento == null)
                throw new ArgumentNullException("riferimento");

            formVisualizza = VisualizzaTesto(testi.UltimaBibbia, TestoTipi.Bibbia, formVisualizza);
            string[] riferimentoArray = riferimento.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            try
            {
                formVisualizza.SpostaTesto(testi.ConvertiDaStandard(new Riferimento(Convert.ToByte(riferimentoArray[0], CultureInfo.InvariantCulture), Convert.ToByte(riferimentoArray[1], CultureInfo.InvariantCulture), Convert.ToByte(riferimentoArray[2], CultureInfo.InvariantCulture)), testi.UltimaBibbia), true);
            }
            catch // se il riferimento non era nel formato giusto, non spostare il testo
            {
            }
            return formVisualizza;
        }

        public Visualizza MostraSegnalibro(string riferimento)
        {
            return MostraSegnalibro(riferimento, null);
        }

        #endregion

        private void navigatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!TrovaForm("Navigatore"))
            {
                Navigatore formNavigator = new Navigatore(this)
                {
                    MdiParent = this
                };
                formNavigator.Show();
            }
        }

        #region immagini

        private void imageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                string ultimaDirectoryImmagini = Settings.Default.UltimaDirectoryImmagini;
                if (String.IsNullOrEmpty(ultimaDirectoryImmagini))
                    ultimaDirectoryImmagini = Application.StartupPath;
                //            if (!ultimaDirectoryImmagini.EndsWith(Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase))
                //                ultimaDirectoryImmagini += Path.DirectorySeparatorChar;
                openFileDialog.InitialDirectory = ultimaDirectoryImmagini;
                openFileDialog.Filter = LocRM.GetString("ImagesFilter");
                openFileDialog.Multiselect = true;
                openFileDialog.CheckFileExists = true;
                openFileDialog.CheckPathExists = true;
                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                    ApriNomeImmagini(openFileDialog.FileNames);
            }
        }

        private void ApriNomeImmagini(string[] fileDaAprire)
        {
            Settings.Default.UltimaDirectoryImmagini = Path.GetDirectoryName(fileDaAprire[0]);
            foreach (string nomeFile in fileDaAprire)
            {
                Immagine formImmagine = new Immagine(nomeFile)
                {
                    MdiParent = this
                };
                formImmagine.Show();
            }
        }

        #endregion

        #endregion

        #region Menu Formato

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null && ActiveMdiChild.Tag.ToString() == "Editor")
            {
                fontApplied = false;
                Font fontVecchio = ((Editor)ActiveMdiChild).rtEditor.SelectionFont;
                Font fontPerDialog = null;
                if (fontVecchio == null)
                {
                    FontStyle fs = FontStyle.Regular;
                    if (testi.Formato.FontGrassetto)
                        fs &= FontStyle.Bold;
                    if (testi.Formato.FontCorsivo)
                        fs &= FontStyle.Italic;
                    if (testi.Formato.FontSottolineato)
                        fs &= FontStyle.Underline;
                    try
                    {
                        fontPerDialog = new Font(testi.Formato.FontNome, testi.Formato.FontDimensione, fs);
                    }
                    catch (ArgumentException)
                    { // con un computer, la riga precedente ha dato un errore che Palatino Linotype non ha lo stile normale. Proviamo in altri modi...
                        try
                        {
                            fontPerDialog = new Font(testi.Formato.FontNome, testi.Formato.FontDimensione);
                        }
                        catch (ArgumentException)
                        {
                        }
                    }
                    if (fontPerDialog != null)
                        fontDialog.Font = fontPerDialog;
                }
                else
                    fontDialog.Font = fontVecchio;

                Color coloreVecchio = ((Editor)ActiveMdiChild).rtEditor.SelectionColor;
                fontDialog.Color = coloreVecchio;

                switch (fontDialog.ShowDialog())
                {
                    case DialogResult.Cancel:
                        // forse il font è stato cambiato dal pulsante Apply
                        if (fontApplied)
                        {
                            if (fontVecchio != null) // è null se il testo selezionato aveva due font diversi
                                ((Editor)ActiveMdiChild).rtEditor.SelectionFont = fontVecchio;
                            // però se due font o due dimensioni selezionati, Apply poi Annulla li cambia comunque - non so come risolvere questo problema
                            ((Editor)ActiveMdiChild).rtEditor.SelectionColor = coloreVecchio;
                        }
                        break;
                    case DialogResult.OK:
                        fontDialog_Apply(sender, new System.EventArgs());
                        break;
                }

                if (fontPerDialog != null)
                    fontPerDialog.Dispose();
            }
        }

        private void fontDialog_Apply(object sender, EventArgs e)
        {
            fontApplied = true;
            if (ActiveMdiChild != null && ActiveMdiChild.Tag.ToString() == "Editor")
            {
                ((Editor)ActiveMdiChild).rtEditor.SelectionFont = fontDialog.Font;
                ((Editor)ActiveMdiChild).rtEditor.SelectionColor = fontDialog.Color;
            }
        }

        #region Highlight

        private void highlightToolStripSplitButton_Click(object sender, EventArgs e)
        {
            switch (Settings.Default.EvidenziaTipo)
            {
                case "highlighter":
                    if (!isRunningOnMono)
                        HighlighterClickNotMono(Settings.Default.EvidenziaColore);
                    break;
                case "colour":
                    ColourClick(Settings.Default.EvidenziaColore);
                    break;
                default:
                    UnderlineClick(Convert.ToByte(Settings.Default.EvidenziaTipo, CultureInfo.InvariantCulture));
                    break;
            }
        }

        private void HighlightClick(object sender, EventArgs e)
        {
            ToolStripMenuItem senderComeVoce = (ToolStripMenuItem)sender;
            string evidenziaTipo = senderComeVoce.Tag.ToString();
            switch (evidenziaTipo)
            {
                case "highlighter":
                    if (!isRunningOnMono)
                    {
                        Color colore = senderComeVoce.BackColor;
                        HighlighterClickNotMono(colore);
                        Settings.Default.EvidenziaColore = colore;
                    }
                    break;
                case "colour":
                    Color colore2 = senderComeVoce.ForeColor;
                    ColourClick(colore2);
                    Settings.Default.EvidenziaColore = colore2;
                    break;
                case "none":
                    noneClick();
                    break;
                default:
                    UnderlineClick(Convert.ToByte(senderComeVoce.Tag.ToString(), CultureInfo.InvariantCulture));
                    break;
            }
            if (evidenziaTipo != "none")
                Settings.Default.EvidenziaTipo = evidenziaTipo;
        }

        private void HighlighterClickNotMono(Color colore)
        {
            if (ActiveMdiChild != null && ActiveMdiChild.Tag != null)
            {
                switch (ActiveMdiChild.Tag.ToString())
                {
                    case "Editor":
                        ((Editor)ActiveMdiChild).rtEditor.SelectionBackColor = colore;
                        break;
                    case "Visualizza":
                        ((Visualizza)ActiveMdiChild).HighlighterClick(colore, TipoHighlight.Evidenziatore);
                        break;
                    case "Lettura":
                        ((Lettura)ActiveMdiChild).HighlighterClick(colore, TipoHighlight.Evidenziatore);
                        break;
                    case "BraniParalleli":
                        ((BraniParalleli)ActiveMdiChild).HighlighterClick(colore, TipoHighlight.Evidenziatore);
                        break;
                }
            }
        }

        private void ColourClick(Color colore)
        {
            if (ActiveMdiChild != null && ActiveMdiChild.Tag != null)
            {
                switch (ActiveMdiChild.Tag.ToString())
                {
                    case "Editor":
                        ((Editor)ActiveMdiChild).rtEditor.SelectionColor = colore;
                        break;
                    case "Visualizza":
                        ((Visualizza)ActiveMdiChild).HighlighterClick(colore, TipoHighlight.Colore);
                        break;
                    case "Lettura":
                        ((Lettura)ActiveMdiChild).HighlighterClick(colore, TipoHighlight.Colore);
                        break;
                    case "BraniParalleli":
                        ((BraniParalleli)ActiveMdiChild).HighlighterClick(colore, TipoHighlight.Colore);
                        break;
                }
            }
        }

        private void UnderlineClick(byte tipoSottolineatura)
        {
            if (ActiveMdiChild != null && ActiveMdiChild.Tag != null && !isRunningOnMono)
            {
                switch (ActiveMdiChild.Tag.ToString())
                {
                    case "Editor":
                        ((Editor)ActiveMdiChild).rtEditor.SetSelectionUnderlineTypeNotMono(tipoSottolineatura);
                        break;
                    case "Visualizza":
                        ((Visualizza)ActiveMdiChild).HighlighterClick(tipoSottolineatura);
                        break;
                    case "Lettura":
                        ((Lettura)ActiveMdiChild).HighlighterClick(tipoSottolineatura);
                        break;
                    case "BraniParalleli":
                        ((BraniParalleli)ActiveMdiChild).HighlighterClick(tipoSottolineatura);
                        break;
                }
            }
        }

        private void noneClick()
        {
            if (ActiveMdiChild != null && ActiveMdiChild.Tag != null)
            {
                if (!isRunningOnMono)
                    HighlighterNoneNotMono();
                switch (ActiveMdiChild.Tag.ToString())
                {
                    case "Editor":
                        ((Editor)ActiveMdiChild).rtEditor.SelectionColor = ((Editor)ActiveMdiChild).rtEditor.ForeColor;
                        if (!isRunningOnMono)
                            ((Editor)ActiveMdiChild).rtEditor.SetSelectionUnderlineTypeNotMono(0);
                        break;
                    case "Visualizza":
                        ((Visualizza)ActiveMdiChild).HighlighterNoneClick();
                        break;
                    case "Lettura":
                        ((Lettura)ActiveMdiChild).HighlighterNoneClick();
                        break;
                    case "BraniParalleli":
                        ((BraniParalleli)ActiveMdiChild).HighlighterNoneClick();
                        break;
                }
            }
        }

        private void HighlighterNoneNotMono()
        {
            switch (ActiveMdiChild.Tag.ToString())
            {
                case "Editor":
                    ((Editor)ActiveMdiChild).rtEditor.SelectionBackColor = ((Editor)ActiveMdiChild).rtEditor.BackColor;
                    break;
                case "Visualizza":
                    ((Visualizza)ActiveMdiChild).HighlighterNoneNotMonoClick();
                    break;
                case "Lettura":
                    ((Lettura)ActiveMdiChild).HighlighterNoneNotMonoClick();
                    break;
                case "BraniParalleli":
                    ((BraniParalleli)ActiveMdiChild).HighlighterNoneNotMonoClick();
                    break;
            }
        }

        internal void HighlightChangedEvent(RichTextBoxHighlight.HighlightChangedEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");
            foreach (Form formFiglio in MdiChildren)
            {
                if (formFiglio.Tag != null)
                {
                    if (formFiglio.Tag.ToString() == "Visualizza")
                        ((Visualizza)formFiglio).AggiornaHighlight(e.Versione);
                    else if (formFiglio.Tag.ToString() == "Lettura")
                        ((Lettura)formFiglio).AggiornaHighlight(e.Versione);
                    else if (formFiglio.Tag.ToString() == "BraniParalleli")
                        ((BraniParalleli)formFiglio).AggiornaHighlight(e.Versione);
                }
            }
        }

        #endregion

        private void paragraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null && ActiveMdiChild.Tag.ToString() == "Editor")
            {
                using (Paragrafo fParagrafo = new Paragrafo(this, ((Editor)ActiveMdiChild).rtEditor))
                {
                    fParagrafo.ShowDialog();
                }
            }
        }

        private void fontToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (aggiornaFont)
            {
                if (ActiveMdiChild != null && ActiveMdiChild.Tag.ToString() == "Editor")
                    ((Editor)ActiveMdiChild).rtEditor.SetFont(fontToolStripComboBox.Text);

                string nomeFont = fontToolStripComboBox.Text;
                if (String.IsNullOrEmpty(nomeFont))
                    return;

                int indexOf = fontPreferiti.IndexOf(nomeFont);
                fontToolStripComboBox.BeginUpdate();
                if (indexOf == -1)
                {
                    if (massimoFontPreferiti > fontPreferiti.Count)
                    {
                        // Insert new
                        fontPreferiti.Insert(0, nomeFont);
                        fontToolStripComboBox.Items.Insert(0, nomeFont);
                    }
                    else
                    {
                        // Don't add any new fonts - replace instead
                        fontPreferiti.RemoveAt(massimoFontPreferiti - 1);
                        fontPreferiti.Insert(0, nomeFont);
                        fontToolStripComboBox.Items.RemoveAt(massimoFontPreferiti - 1);
                        fontToolStripComboBox.Items.Insert(0, nomeFont);
                    }
                }
                else
                {
                    // Move existing around
                    if (fontPreferiti.Count > 1)
                    {
                        fontPreferiti.RemoveAt(indexOf);
                        fontPreferiti.Insert(0, nomeFont);
                        fontToolStripComboBox.Items.RemoveAt(indexOf);
                        fontToolStripComboBox.Items.Insert(0, nomeFont);
                    }
                }
                fontToolStripComboBox.EndUpdate();

                if (((Editor)ActiveMdiChild) != null)
                    this.ActiveControl = ActiveMdiChild;
            }
        }

        private void fontSizeToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null && ActiveMdiChild.Tag.ToString() == "Editor")
                ((Editor)ActiveMdiChild).rtEditor.SetSize(Convert.ToSingle(fontSizeToolStripComboBox.Text, CultureInfo.InvariantCulture));
        }

        private void fontSizeToolStripComboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
                if (ActiveMdiChild != null && ActiveMdiChild.Tag.ToString() == "Editor")
                    ((Editor)ActiveMdiChild).rtEditor.SetSize(Convert.ToSingle(fontSizeToolStripComboBox.Text, CultureInfo.InvariantCulture));
        }

        private void styleToolStripButton_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null && ActiveMdiChild.Tag.ToString() == "Editor")
            {
                string tag = "";
                if (sender.GetType().Name == "ToolStripButton")
                    tag = ((ToolStripButton)sender).Tag.ToString();
                else if (sender.GetType().Name == "ToolStripMenuItem")
                    tag = ((ToolStripMenuItem)sender).Tag.ToString();
                switch (tag)
                {
                    case "bold":
                        ((Editor)ActiveMdiChild).rtEditor.SetSelectionBold(!boldToolStripButton.Checked);
                        break;
                    case "italic":
                        ((Editor)ActiveMdiChild).rtEditor.SetSelectionItalic(!italicToolStripButton.Checked);
                        break;
                    case "underline":
                        ((Editor)ActiveMdiChild).rtEditor.SetSelectionUnderline(!underlineToolStripButton.Checked);
                        break;
                }
                AggiornaPulsanti(((Editor)ActiveMdiChild).rtEditor);
            }
        }

        private void alignToolStripButton_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null && ActiveMdiChild.Tag.ToString() == "Editor")
            {
                string tag = "";
                if (sender.GetType().Name == "ToolStripButton")
                    tag = ((ToolStripButton)sender).Tag.ToString();
                else if (sender.GetType().Name == "ToolStripMenuItem")
                    tag = ((ToolStripMenuItem)sender).Tag.ToString();
                switch (tag)
                {
                    case "left":
                        ((Editor)ActiveMdiChild).rtEditor.SelectionAlignment = RichTextBoxEx.TextAlign.Left;
                        break;
                    case "center":
                        ((Editor)ActiveMdiChild).rtEditor.SelectionAlignment = RichTextBoxEx.TextAlign.Center;
                        break;
                    case "right":
                        ((Editor)ActiveMdiChild).rtEditor.SelectionAlignment = RichTextBoxEx.TextAlign.Right;
                        break;
                    case "justify":
                        ((Editor)ActiveMdiChild).rtEditor.SelectionAlignment = RichTextBoxEx.TextAlign.Justify;
                        break;
                }
                AggiornaPulsanti(((Editor)ActiveMdiChild).rtEditor);
            }
        }

        private void bulletsToolStripButton_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null && ActiveMdiChild.Tag.ToString() == "Editor")
            {
                ((Editor)ActiveMdiChild).rtEditor.SelectionBullet = !(bulletsToolStripButton.Checked);
                AggiornaPulsanti(((Editor)ActiveMdiChild).rtEditor);
            }
        }

        private void indentToolStripButton_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null && ActiveMdiChild.Tag.ToString() == "Editor")
            {
                int rientro = Convert.ToInt32(pixelPerCm);
                if (((ToolStripButton)sender).Name == "indentDecreaseToolStripButton")
                    rientro = -rientro;
                ((Editor)ActiveMdiChild).rtEditor.SelectionIndent = ((Editor)ActiveMdiChild).rtEditor.SelectionIndent + rientro;
                AggiornaPulsanti(((Editor)ActiveMdiChild).rtEditor);
            }
        }

        private void ipertestoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null && ActiveMdiChild.Tag.ToString() == "Editor")
            {
                CreaLink(((Editor)ActiveMdiChild).rtEditor, ((ToolStripMenuItem)sender).Tag.ToString());
            }
        }

        public void CreaLink(RichTextBoxEx rtb, string tipo)
        {
            if (rtb == null)
                throw new ArgumentNullException("rtb");

            string linkA = rtb.SelectedText;
            using (InputBox inputBox = new InputBox(LocRM.GetString("MainMakeHypertextCaption"), LocRM.GetString("MainMakeHypertextQuestion"), linkA))
            {
                inputBox.ShowDialog();
                linkA = inputBox.Risposta;
            }
            if (!string.IsNullOrEmpty(linkA))
            {
                if (((Editor)ActiveMdiChild) != null)
                {
                    switch (tipo)
                    {
                        case "V": // versetti
                            rtb.InserisciLink(testi.ConvertiRiferimento(linkA).ComeNotaTuttoRiferimento(), RichTextBoxEx.FineLinkBrano);
                            break;
                        case "N": // nota
                            rtb.InserisciLink(linkA, RichTextBoxEx.FineLinkNota);
                            break;
                        case "F": // file
                            rtb.InserisciLink(linkA, RichTextBoxEx.FineLinkFile);
                            break;
                    }
                }
            }
        }

        #endregion

        #region Menu Strumenti

        private void updateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RichiediAggiornamento(0);
        }

        private void RichiediAggiornamento(int tipoAggiornamento)
        {
            // tipoAggionamento: 0=manuale, 1=automatico di file esistenti, 2=automatico di tutti
            bool esistonoAggiornamenti;
            try
            {
                esistonoAggiornamenti = CercaAggiornamenti(tipoAggiornamento);
            }
            catch (Exception eccezione)
            {
                MessageBox.Show(string.Format(CultureInfo.CurrentCulture, LocRM.GetString("MainUpdateNotConnected"), eccezione.Message), LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, messageBoxOptions);
                return;
            }

            if (!esistonoAggiornamenti)
                MessageBox.Show(LocRM.GetString("MainUpdateNoUpdate"), LocRM.GetString("MiscInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, messageBoxOptions);
        }

        private bool CercaAggiornamenti(int tipoAggiornamento)
        {
            Collection<FileDaAggiornare> listaFileDaAggiornare = new Collection<FileDaAggiornare>();
            Cursor cursoreAttuale = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.AppStarting;

                XmlDocument xmlDocumento = new XmlDocument();
                string proxyHost = Settings.Default.AggiornamentoProxyHost;
                string credentialUtente = Settings.Default.AggiornamentoProxyNomeUtente;
                if (string.IsNullOrEmpty(proxyHost) && string.IsNullOrEmpty(credentialUtente))
                {
                    xmlDocumento.Load(urlAggiornamenti);
                }
                else
                {
                    WebRequest richiestaPagina = WebRequest.Create(urlAggiornamenti);

                    if (!string.IsNullOrEmpty(proxyHost))
                    {
                        int proxyPorta = Settings.Default.AggiornamentoProxyPorta;
                        if (proxyPorta == 0)
                            richiestaPagina.Proxy = new WebProxy(proxyHost);
                        else
                            richiestaPagina.Proxy = new WebProxy(proxyHost, proxyPorta);
                        if (!string.IsNullOrEmpty(credentialUtente))
                        {
                            string credentialPassword = Settings.Default.AggiornamentoProxyPassword;
                            string credentialDominio = Settings.Default.AggiornamentoProxyDominio;
                            if (string.IsNullOrEmpty(credentialDominio))
                                richiestaPagina.Proxy.Credentials = new NetworkCredential(credentialUtente, credentialPassword);
                            else
                                richiestaPagina.Proxy.Credentials = new NetworkCredential(credentialUtente, credentialPassword, credentialDominio);
                        }
                    }

                    WebResponse rispostaPagina = richiestaPagina.GetResponse();
                    StreamReader stream = new StreamReader(rispostaPagina.GetResponseStream());
                    xmlDocumento.Load(stream);
                }

                XmlNodeList nodiDeiFile = xmlDocumento.SelectSingleNode("versioni").SelectNodes("file");
                string nome, versioneDisponibile, versioneAttuale, tipoTesto, nomeFile, componente;
                string linguaInterfaccia = "";
                if (Thread.CurrentThread.CurrentUICulture.ToString().Length >= 2)
                    linguaInterfaccia = Thread.CurrentThread.CurrentUICulture.ToString().Substring(0, 2).ToLower(CultureInfo.InvariantCulture);
                string aggiornaUpdateUrlCartella = "";
                string[] videoInstallati = Settings.Default.VideoInstallati.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (XmlNode nodo in nodiDeiFile)
                {
                    nome = InnerTextInLingua(nodo, "nome", linguaInterfaccia);
                    componente = nodo.SelectSingleNode("componente").InnerText;
                    versioneDisponibile = nodo.SelectSingleNode("versione").InnerText;
                    tipoTesto = nodo.SelectSingleNode("tipo").InnerText;
                    versioneAttuale = "0.0.0";
                    nomeFile = "";
                    switch (tipoTesto)
                    {
                        case "programma":
                            versioneAttuale = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                            versioneAttuale = versioneAttuale.Remove(versioneAttuale.LastIndexOf('.'));
                            nomeFile = Application.ExecutablePath;
                            break;
                        case "aggiornamento":
                            versioneAttuale = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                            versioneAttuale = versioneAttuale.Remove(versioneAttuale.LastIndexOf('.'));
                            break;
                        case "testo":
                        case "Bibbia":
                        case "commentario":
                        case "dizionario":
                        case "note":
                        case "libro":
                            versioneAttuale = testi.Info(componente).Versione;
                            nomeFile = testi.Info(componente).NomeDelFile;
                            if (string.IsNullOrEmpty(nomeFile))
                                nomeFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + componente + ".laparola";
                            break;
                        case "parallelo":
                            foreach (ToolStripItem voce in parallelsToolStripMenuItem.DropDownItems)
                            {
                                if (Path.GetFileNameWithoutExtension(((InfoBraniParalleli)(voce.Tag)).nomeFile) == componente)
                                {
                                    versioneAttuale = ((InfoBraniParalleli)(voce.Tag)).versione;
                                    nomeFile = ((InfoBraniParalleli)(voce.Tag)).nomeFile;
                                    break;
                                }
                            }
                            if (string.IsNullOrEmpty(nomeFile))
                                nomeFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + "Paralleli" + Path.DirectorySeparatorChar + componente + ".xml";
                            break;
                        case "collegamento":
                            int numeroCollegamenti = externalLinkStripMenuItem.DropDownItems.Count - 2;
                            for (int i = 0; i < numeroCollegamenti; ++i)
                            {
                                ToolStripMenuItem voce = (ToolStripMenuItem)(externalLinkStripMenuItem.DropDownItems[i]);
                                if (voce.DropDownItems.Count == 0)
                                { // è una voce normale, non una categoria
                                    if (Path.GetFileNameWithoutExtension(((InfoCollegamento)(voce.Tag)).nomeFile) == componente)
                                    {
                                        versioneAttuale = ((InfoCollegamento)(voce.Tag)).versione;
                                        nomeFile = ((InfoCollegamento)(voce.Tag)).nomeFile;
                                        break;
                                    }
                                }
                                foreach (ToolStripItem voceInCategoria in voce.DropDownItems)
                                {
                                    if (Path.GetFileNameWithoutExtension(((InfoCollegamento)(voceInCategoria.Tag)).nomeFile) == componente)
                                    {
                                        versioneAttuale = ((InfoCollegamento)(voceInCategoria.Tag)).versione;
                                        nomeFile = ((InfoCollegamento)(voceInCategoria.Tag)).nomeFile;
                                        break;
                                    }
                                }
                            }
                            if (string.IsNullOrEmpty(nomeFile))
                                nomeFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + "Collegamenti" + Path.DirectorySeparatorChar + componente + ".xml";
                            break;
                        case "segnalibro":
                            int numeroSegnalibri = bookmarksToolStripMenuItem.DropDownItems.Count - 2;
                            for (int i = 4; i < numeroSegnalibri; ++i)
                            {
                                ToolStripMenuItem voce = (ToolStripMenuItem)(bookmarksToolStripMenuItem.DropDownItems[i]);
                                string tag = voce.Tag.ToString();
                                if (Path.GetFileNameWithoutExtension(tag.Remove(tag.IndexOf('|'))) == componente)
                                {
                                    versioneAttuale = tag.Remove(0, tag.IndexOf('|') + 1);
                                    nomeFile = tag.Remove(tag.IndexOf('|'));
                                    break;
                                }
                            }
                            if (string.IsNullOrEmpty(nomeFile))
                                nomeFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + "Segnalibri" + Path.DirectorySeparatorChar + componente + ".xml";
                            break;
                        case "lettura":
                            foreach (InfoLettura lettura in schemiLettura)
                            {
                                if (Path.GetFileNameWithoutExtension(lettura.nomeFile) == componente)
                                {
                                    versioneAttuale = lettura.versione;
                                    nomeFile = lettura.nomeFile;
                                    break;
                                }
                            }
                            if (string.IsNullOrEmpty(nomeFile))
                                nomeFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + "Letture" + Path.DirectorySeparatorChar + componente + ".xml";
                            break;
                        case "video":
                            string videoFile;
                            foreach (ToolStripMenuItem voce in videoToolStripMenuItem.DropDownItems)
                            {
                                videoFile = voce.Tag.ToString();
                                if (voce.Text == componente)
                                {
                                    versioneAttuale = "7.00.0";
                                    for (int i = 0; i < videoInstallati.Length / 2; ++i)
                                    {
                                        if (Path.GetFileNameWithoutExtension(videoInstallati[i * 2]) == componente)
                                        {
                                            versioneAttuale = videoInstallati[i * 2 + 1];
                                            break;
                                        }
                                    }
                                    nomeFile = videoFile;
                                    break;
                                }
                            }
                            if (string.IsNullOrEmpty(nomeFile))
                                nomeFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + "Video" + Path.DirectorySeparatorChar + componente + ".swf";
                            break;
                        case "testiparalleli":
                            for (int i = 0; i < parallelTextsStripMenuItem.DropDownItems.Count; ++i)
                            {
                                ToolStripMenuItem voce = (ToolStripMenuItem)(parallelTextsStripMenuItem.DropDownItems[i]);
                                string tag = voce.Tag.ToString();
                                if (Path.GetFileNameWithoutExtension(tag) == componente)
                                {
                                    nomeFile = tag;
                                    string[] righe = File.ReadAllLines(tag, Encoding.UTF8);
                                    if (righe.Length > 0 && righe[0].StartsWith("#", StringComparison.Ordinal))
                                        versioneAttuale = righe[0].Substring(1);
                                }
                            }
                            if (string.IsNullOrEmpty(nomeFile))
                                nomeFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + "TestiParalleli" + Path.DirectorySeparatorChar + componente;
                            break;
                        default:
                            versioneAttuale = "9999.0.0";
                            // se il tipo è sconosciuto, non aggiorniamo (perché le altre informazioni per es. dove salvare il file sono sconosciute)
                            // invece dopo l'aggiornamento del programma, il tipo dovrà essere conosciuto e un secondo aggiornamento funzionerà
                            break;
                    }

                    if (new Version(versioneAttuale).CompareTo(new Version(versioneDisponibile)) < 0)
                    {
                        if (tipoTesto != "aggiornamento")
                        {
                            FileDaAggiornare fileDaAggiornare = new FileDaAggiornare
                            {
                                nome = nome,
                                componente = componente,
                                nomeFile = nomeFile,
                                tipo = tipoTesto,
                                versioneNuova = versioneDisponibile,
                                versioneAttuale = versioneAttuale,
                                url = new Collection<string>()
                            };
                            foreach (XmlNode urlNodo in nodo.SelectNodes("url"))
                                fileDaAggiornare.url.Add(urlNodo.InnerText);
                            fileDaAggiornare.dimensione = nodo.SelectSingleNode("dimensione").InnerText;
                            listaFileDaAggiornare.Add(fileDaAggiornare);
                        }
                        else
                        {
                            aggiornaUpdateUrlCartella = nodo.SelectSingleNode("url").InnerText;
                        }
                    }
                }

                Settings.Default.AggiornamentoUltimo = DateTime.Now;
                Cursor.Current = cursoreAttuale;
                if (cursoreAttuale != null)
                    cursoreAttuale.Dispose();

                if (listaFileDaAggiornare.Count > 0)
                {
                    using (Aggiorna fAggiorna = new Aggiorna(this, listaFileDaAggiornare, aggiornaUpdateUrlCartella))
                    {
                        if (tipoAggiornamento == 0)
                            fAggiorna.ShowDialog();
                        else
                            fAggiorna.EseguiAggiornamento(tipoAggiornamento);
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                Cursor.Current = cursoreAttuale;
                if (cursoreAttuale != null)
                    cursoreAttuale.Dispose();
            }
            return (listaFileDaAggiornare.Count > 0);
        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!TrovaForm("Ricerca"))
            {
                Ricerca formRicerca = new Ricerca(this)
                {
                    MdiParent = this
                };
                formRicerca.Show();
            }
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!TrovaForm("Mostra"))
            {
                Mostra formMostra = new Mostra(this)
                {
                    MdiParent = this
                };
                formMostra.Show();
            }
        }

        private void concordanceStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!TrovaForm("Chiave"))
            {
                Chiave formChiave = new Chiave(this)
                {
                    MdiParent = this
                };
                formChiave.Show();
            }
        }

        private void informationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApriInformazione("");
        }

        private void similiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!TrovaForm("Simili"))
            {
                BraniSimili formSimili = new BraniSimili(this)
                {
                    MdiParent = this
                };
                formSimili.Show();
            }
        }

        public void ApriInformazione(string richiesta)
        {
            Informazioni formInformazioni = null;
            foreach (Form formFiglio in MdiChildren)
            {
                if (formFiglio.Tag != null && formFiglio.Tag.ToString() == "Informazioni")
                {
                    formFiglio.Activate();
                    formInformazioni = (Informazioni)formFiglio;
                }
            }
            if (formInformazioni == null)
            {
                formInformazioni = new Informazioni(this)
                {
                    MdiParent = this
                };
                formInformazioni.Show();
            }

            Application.DoEvents();
            formInformazioni.ImpostaRichiesta(richiesta);
        }

        private void paralleliVoceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BraniParalleli formBraniParalleli = new BraniParalleli(this, (InfoBraniParalleli)(((ToolStripItem)sender).Tag))
            {
                MdiParent = this
            };
            formBraniParalleli.Show();
        }

        private void readingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Lettura formLettura = new Lettura(this)
            {
                MdiParent = this
            };
            formLettura.Show();
        }

        private void measuresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Misure formMisure = new Misure(this)
            {
                MdiParent = this
            };
            formMisure.Show();
        }

        private void quizStripMenuItem_Click(object sender, EventArgs e)
        {
            Quiz formQuiz = new Quiz(this)
            {
                MdiParent = this
            };
            formQuiz.Show();
        }

        private void manageNotesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (GestisciTesti formGestisciNote = new GestisciTesti(this))
            {
                formGestisciNote.ShowDialog();
            }
            GeneraMenuConTesti();
        }

        private void createNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApriApriNota(sender.ToString(), -1);
        }

        private void ApriApriNota(string versione, int scheda)
        {
            bool trovato = false;
            foreach (Form formFiglio in MdiChildren)
            {
                if (formFiglio.Tag != null && formFiglio.Tag.ToString() == "ApriNota" && ((ApriNota)formFiglio).Versione == versione)
                {
                    formFiglio.Activate();
                    trovato = true;
                }
            }
            if (!trovato)
            {
                ApriNota formApriNota = new ApriNota(this, versione, scheda)
                {
                    MdiParent = this
                };
                formApriNota.Show();
            }
        }

        private void externalLinkVoceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RichTextBoxEx richText = null;
            if (ActiveMdiChild != null)
            {
                if (ActiveMdiChild.Tag != null && ActiveMdiChild.Tag.ToString() == "Editor")
                    richText = ((Editor)ActiveMdiChild).rtEditor;
                if (ActiveMdiChild.Tag != null && ActiveMdiChild.Tag.ToString() == "Visualizza")
                    richText = ((Visualizza)ActiveMdiChild).RtfAttiva;
                if (ActiveMdiChild.Tag != null && (ActiveMdiChild.Tag.ToString() == "BraniParalleli" || ActiveMdiChild.Tag.ToString() == "Lettura"))
                {
                    try
                    {
                        richText = (RichTextBoxEx)(ActiveMdiChild.ActiveControl);
                    }
                    catch { }
                }
            }
            if (richText != null && !string.IsNullOrEmpty(richText.Text))
            {
                object voceTag = (((ToolStripMenuItem)sender).Tag);
                if (((InfoCollegamento)voceTag).tipo == CollegamentoTipo.Parola)
                {
                    if (richText.SelectionLength > 0)
                        CollegamentoAParolaORiferimento((InfoCollegamento)voceTag, richText.SelectedText);
                    else
                        CollegamentoAParolaORiferimento((InfoCollegamento)voceTag, richText.ParolaAttuale(richText.SelectionStart));
                }
                else
                {
                    // prima proviamo a trovare un riferimento nel testo selezionato, poi se la selezione è nel testo di un versetto
                    string selezione = richText.SelectedText;
                    if (!string.IsNullOrEmpty(selezione) && char.IsDigit(selezione[selezione.Length - 1]) && testi.ConvertiRiferimento(selezione).Count > 0)
                        CollegamentoAParolaORiferimento((InfoCollegamento)voceTag, testi.ConvertiRiferimento(selezione).ComeNotaPrimoRiferimento().Substring(1, 8));
                    else
                    {
                        string riferimento = richText.VersettoAttuale(richText.SelectionStart);
                        if (string.IsNullOrEmpty(riferimento) && ActiveMdiChild.Tag.ToString() == "Editor")
                            riferimento = ((Editor)ActiveMdiChild).RiferimentoDaNomeNota();
                        if (!string.IsNullOrEmpty(riferimento))
                            CollegamentoAParolaORiferimento((InfoCollegamento)voceTag, riferimento);
                    }
                }
            }
        }

        private static void CollegamentoAParolaORiferimento(InfoCollegamento collegamento, string testoDaRicercare)
        {
            string[] urlConParametri = CostruisciCollegamento(collegamento.url, collegamento.parametri, testoDaRicercare, collegamento.mappa, collegamento.tipo);
            string url = urlConParametri[0];
            string parametri = urlConParametri[1];
            try
            {
                if (url.IndexOf("://", StringComparison.Ordinal) > 0) // indirizzo Internet; se non c'è ://, è un file locale e non serve convertire
                    url = Uri.EscapeUriString(url);
                Funzioni.ApriBrowser(url, parametri, true);
            }
            catch (Exception exc)
            {
                MessageBox.Show(string.Format(CultureInfo.CurrentCulture, LocRM.GetString("MainLinksErrorCommandFailed"), (url + " " + parametri).Trim(), exc.Message), LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, messageBoxOptions);
            }
        }

        public static string[] CostruisciCollegamento(String url, String parametri, string testoDaRicercare, Collection<CollegamentoMappaVoce> mappa, CollegamentoTipo tipo)
        {
            foreach (CollegamentoMappaVoce voceMappa in mappa)
            {
                if (String.Compare(testoDaRicercare, voceMappa.inizio, StringComparison.OrdinalIgnoreCase) >= 0 && String.Compare(testoDaRicercare, voceMappa.fine, StringComparison.OrdinalIgnoreCase) <= 0)
                    url = voceMappa.pagina;
            }

            if (tipo == CollegamentoTipo.Parola)
            {
                url = ConvertiCodiciParola(url, testoDaRicercare);
                parametri = ConvertiCodiciParola(parametri, testoDaRicercare);
            }
            else
            {
                int libro = Convert.ToInt16(testoDaRicercare.Substring(0, 2), CultureInfo.InvariantCulture);
                int capitolo = Math.Max((Int16)1, Convert.ToInt16(testoDaRicercare.Substring(2, 3), CultureInfo.InvariantCulture));
                int versetto = Math.Max((Int16)1, Convert.ToInt16(testoDaRicercare.Substring(5, 3), CultureInfo.InvariantCulture));
                url = ConvertiCodiciBrano(url, libro, capitolo, versetto);
                parametri = ConvertiCodiciBrano(parametri, libro, capitolo, versetto);
            }
            string[] urlConParametri = new string[] { url, parametri };
            return urlConParametri;
        }

        private static String ConvertiCodiciParola(String stringa, string testoDaRicercare)
        {
            // se un codice è aggiunto qui, bisogna aggiungerlo anche nel file della Guida
            stringa = stringa.Replace("{primalettera}", testoDaRicercare.Substring(0, 1)).Replace("{firstletter}", testoDaRicercare.Substring(0, 1));
            stringa = stringa.Replace("{parola}", testoDaRicercare).Replace("{word}", testoDaRicercare);
            string testoDaRicercareLC = testoDaRicercare.ToLowerInvariant();
            stringa = stringa.Replace("{primaletteralc}", testoDaRicercareLC.Substring(0, 1)).Replace("{firstletterlc}", testoDaRicercareLC.Substring(0, 1));
            stringa = stringa.Replace("{parolalc}", testoDaRicercareLC).Replace("{wordlc}", testoDaRicercareLC);
            return stringa;
        }

        private static String ConvertiCodiciBrano(String stringa, int libro, int capitolo, int versetto)
        {
            // se un codice è aggiunto qui, bisogna aggiungerlo anche nel file della Guida
            stringa = stringa.Replace("{libro}", Texts.LibriNomiItaliano.Split('|')[libro]).Replace("{book}", Texts.LibriNomiInglese.Split('|')[libro]);
            stringa = stringa.Replace("{librolc}", Texts.LibriNomiItaliano.Split('|')[libro].ToLowerInvariant()).Replace("{booklc}", Texts.LibriNomiInglese.Split('|')[libro].ToLowerInvariant());
            stringa = stringa.Replace("{libronumero}", ConvertiLibro73A66(libro).ToString(CultureInfo.InvariantCulture)).Replace("{booknumber}", ConvertiLibro73A66(libro).ToString(CultureInfo.InvariantCulture));
            stringa = stringa.Replace("{libronumero2}", Funzioni.AggiungiZero(ConvertiLibro73A66(libro).ToString(CultureInfo.InvariantCulture), 2)).Replace("{booknumber2}", Funzioni.AggiungiZero(ConvertiLibro73A66(libro).ToString(CultureInfo.InvariantCulture), 2));
            stringa = stringa.Replace("{libronumero73}", libro.ToString(CultureInfo.InvariantCulture)).Replace("{booknumber73}", libro.ToString(CultureInfo.InvariantCulture));
            stringa = stringa.Replace("{libronumeroNT}", (libro - 46).ToString(CultureInfo.InvariantCulture)).Replace("{booknumberNT}", (libro - 46).ToString(CultureInfo.InvariantCulture));
            stringa = stringa.Replace("{libroabbreviazione}", Texts.LibriAbbreviazioniRiconosciuteItaliano.Split('|')[libro]).Replace("{bookabbreviation}", Texts.LibriAbbreviazioniUsateInglese.Split('|')[libro]);
            stringa = stringa.Replace("{capitolo}", capitolo.ToString(CultureInfo.InvariantCulture)).Replace("{chapter}", capitolo.ToString(CultureInfo.InvariantCulture));
            stringa = stringa.Replace("{capitolo2}", Funzioni.AggiungiZero(capitolo.ToString(CultureInfo.InvariantCulture), 2)).Replace("{chapter2}", Funzioni.AggiungiZero(capitolo.ToString(CultureInfo.InvariantCulture), 2));
            stringa = stringa.Replace("{capitolo3}", Funzioni.AggiungiZero(capitolo.ToString(CultureInfo.InvariantCulture), 3)).Replace("{chapter3}", Funzioni.AggiungiZero(capitolo.ToString(CultureInfo.InvariantCulture), 3));
            stringa = stringa.Replace("{versetto}", versetto.ToString(CultureInfo.InvariantCulture)).Replace("{verse}", versetto.ToString(CultureInfo.InvariantCulture));
            stringa = stringa.Replace("{versetto3}", Funzioni.AggiungiZero(versetto.ToString(CultureInfo.InvariantCulture), 3)).Replace("{verse3}", Funzioni.AggiungiZero(versetto.ToString(CultureInfo.InvariantCulture), 3));
            string riferimentocv = capitolo.ToString(CultureInfo.InvariantCulture) + ":" + versetto.ToString(CultureInfo.InvariantCulture);
            stringa = stringa.Replace("{riferimento}", Texts.LibriNomiItaliano.Split('|')[libro] + riferimentocv).Replace("{reference}", Texts.LibriNomiInglese.Split('|')[libro] + riferimentocv);
            return stringa;
        }

        private static int ConvertiLibro73A66(int libro)
        {
            if (libro == 17 || libro == 18 || libro == 20 || libro == 21 || libro == 27 || libro == 28 || libro == 32)
                return 0;
            else if (libro <= 16)
                return libro;
            else if (libro == 19)
                return 17;
            else if (libro <= 26)
                return libro - 4;
            else if (libro <= 31)
                return libro - 6;
            else
                return libro - 7;
        }

        private void externalLinkEditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (Collegamenti formCollegamenti = new Collegamenti(this))
            {
                formCollegamenti.ShowDialog();
            }
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (Opzioni formOpzioni = new Opzioni(this))
            {
                formOpzioni.ShowDialog();

                MostraNascondiBarreStrumenti();
                statusStrip.Visible = Settings.Default.PrincipaleBarraDiStato;
                ImpostaLinguaDellaGuida();
            }

            foreach (Form formFiglio in MdiChildren)
            {
                if (formFiglio.Tag != null)
                {
                    if (formFiglio.Tag.ToString() == "Visualizza")
                    {
                        ((Visualizza)formFiglio).CambiaFormato();
                        formFiglio.Refresh();
                    }
                    else if (formFiglio.Tag.ToString() == "BraniParalleli")
                    {
                        ((BraniParalleli)formFiglio).Aggiorna();
                        formFiglio.Refresh();
                    }
                    else if (formFiglio.Tag.ToString() == "Lettura")
                    {
                        ((Lettura)formFiglio).Aggiorna();
                        formFiglio.Refresh();
                    }
                }
            }
        }

        #endregion

        #region Menu Finestra

        // aprire una finestra MDI e poi chiuderla lascia un separatore nel menu - errore in .NET non nel programma

        private void windowsMenu_DropDownOpening(object sender, EventArgs e)
        {
            Screen[] sc = Screen.AllScreens;
            if (sc.Length > 1 && ActiveMdiChild != null && ActiveMdiChild.Tag != null && (ActiveMdiChild.Tag.ToString() == "Editor" || ActiveMdiChild.Tag.ToString() == "Visualizza"))
                projectToolStripMenuItem.Enabled = true;
            else
                projectToolStripMenuItem.Enabled = false;
            removeProjectedWindowToolStripMenuItem.Enabled = (formProiettato != null);
        }

        private void windowsMenu_DropDownClosed(object sender, EventArgs e)
        {
            // così F11 e F12 funzionano sempre; disattiviamo le voci solo quando il menu è aperto - non è necessario, ma aiuta l'utente a capire che non si può fare
            projectToolStripMenuItem.Enabled = true;
            removeProjectedWindowToolStripMenuItem.Enabled = true;
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void projectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Screen[] sc = Screen.AllScreens;
            if (sc.Length > 1 && ActiveMdiChild != null && (ActiveMdiChild.Tag.ToString() == "Editor" || ActiveMdiChild.Tag.ToString() == "Visualizza"))
            {
                Form formDaProiettare = ActiveMdiChild; // necessario, perché removeProjected... cambierà ActiveMdiChild
                if (formProiettato != null)
                    removeProjectedWindowToolStripMenuItem_Click(null, null);
                formProiettato = formDaProiettare;
                formProiettatoBounds = formProiettato.Bounds;
                formProiettato.MdiParent = null;
                formProiettato.FormBorderStyle = FormBorderStyle.None;
                formProiettato.Bounds = sc[1].Bounds;
                if (formProiettato.Tag != null && formProiettato.Tag.ToString() == "Visualizza")
                {
                    ((Visualizza)formProiettato).ImpostaPulsantiVisibili(false);
                    ((Visualizza)formProiettato).ZoomPanes(4.0F);
                }
                else
                    ((Editor)formProiettato).rtEditor.ZoomFactor *= 4.0F;
                formProiettato.StartPosition = FormStartPosition.Manual;
                this.Focus();
            }
        }

        private void removeProjectedWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (formProiettato != null)
            {
                formProiettato.FormBorderStyle = FormBorderStyle.Sizable;
                formProiettato.Bounds = formProiettatoBounds;
                if (formProiettato.Tag != null && formProiettato.Tag.ToString() == "Visualizza")
                {
                    ((Visualizza)formProiettato).ImpostaPulsantiVisibili(true);
                    ((Visualizza)formProiettato).ZoomPanes(0.25F);
                }
                else
                    ((Editor)formProiettato).rtEditor.ZoomFactor *= 0.25F;
                formProiettato.MdiParent = this;
                //ActiveMdiChild.StartPosition = FormStartPosition.WindowsDefaultLocation;
                formProiettato = null;
            }
        }

        #endregion

        #region Menu ?

        private void indexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, fileGuida.HelpNamespace);
        }

        private void contentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help.ShowHelpIndex(this, fileGuida.HelpNamespace);
        }

        private void searchHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, fileGuida.HelpNamespace, HelpNavigator.Find, "");
        }

        private void videoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string directoryTemp = Path.GetTempPath();
            if (!directoryTemp.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                directoryTemp += Path.DirectorySeparatorChar;
            int conto = 0;
            while (File.Exists(directoryTemp + "laparola" + conto.ToString(CultureInfo.InvariantCulture) + ".html"))
                ++conto;
            string nomeVideo = ((ToolStripMenuItem)sender).Tag.ToString();
            string testo = "<html><body>" +
                "<center><object classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" width=\"800\" height=\"586\" codebase=\"http://active.macromedia.com/flash5/cabs/swflash.cab#version=7,0,0,0\">" +
                "<param name=\"movie\" value=\"file:///" + nomeVideo + "\"><param name=\"play\" value=\"true\"><param name=\"loop\" value=\"false\"><param name=\"wmode\" value=\"transparent\"><param name=\"quality\" value=\"low\">" +
                "<embed src=\"file:///" + nomeVideo + "\" width=\"800\" height=\"586\" quality=\"low\" loop=\"false\" wmode=\"transparent\" type=\"application/x-shockwave-flash\" pluginspage=\"http://www.macromedia.com/shockwave/download/index.cgi?P1_Prod_Version=ShockwaveFlash\">" +
                "</embed></object></center><script>obj=document.getElementsByTagName(\'object\');for (var i=0; i<obj.length; ++i)obj[i].outerHTML=obj[i].outerHTML;</script></body></html>";
            File.WriteAllText(directoryTemp + "laparola" + conto.ToString(CultureInfo.InvariantCulture) + ".html", testo);
            System.Diagnostics.Process.Start(directoryTemp + "laparola" + conto.ToString(CultureInfo.InvariantCulture) + ".html");
        }

        private void tutorialsOnInternetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(LocRM.GetString("MainVideoAddress"));
        }

        private void GeneraListaTesti_Click(object sender, EventArgs e)
        {
            using (ListaTesti fListaTesti = new ListaTesti(this))
                fListaTesti.ShowDialog();
        }

        private void AboutBibleToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (sender == null)
                throw new ArgumentNullException("sender");
            else
            {
                using (InformazioniSuBibbia fAbout = new InformazioniSuBibbia(((ToolStripMenuItem)(sender)).Text))
                {
                    fAbout.ShowDialog();
                }
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (InformazioniSu fAbout = new InformazioniSu())
            {
                fAbout.ShowDialog();
            }
        }

        #endregion

        #region Disposizione

        private void arrangementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form formFiglio in MdiChildren)
            {
                if (formFiglio.Tag.ToString() != "Editor")
                    formFiglio.Close();
            }

            object voceTag = ((ToolStripItem)(sender)).Tag;
            if (voceTag != null)
                CaricaDisposizioneFinestre(((InfoDisposizione)voceTag).nomeFile);
        }

        private void CaricaDisposizioneFinestre(string nomeFile)
        {
            if (!File.Exists(nomeFile))
                return;

            try
            {
                XmlDocument xd = new XmlDocument();
                xd.Load(nomeFile);
                XmlNode nodoPrincipale = xd.SelectSingleNode("windows");
                XmlNodeList nodiFinestre = nodoPrincipale.SelectNodes("window");
                string tipoFinestra = "";

                foreach (XmlNode nodo in nodiFinestre)
                {
                    tipoFinestra = nodo.SelectSingleNode("type").InnerText;
                    Point posizione = new Point(Convert.ToInt32(nodo.SelectSingleNode("positionX").InnerText, CultureInfo.InvariantCulture), Convert.ToInt32(nodo.SelectSingleNode("positionY").InnerText, CultureInfo.InvariantCulture));
                    Size dimensione = new Size(Convert.ToInt32(nodo.SelectSingleNode("width").InnerText, CultureInfo.InvariantCulture), Convert.ToInt32(nodo.SelectSingleNode("height").InnerText, CultureInfo.InvariantCulture));
                    string statoFinestra = nodo.SelectSingleNode("state").InnerText;
                    switch (tipoFinestra)
                    {
                        case "Principale":
                            if (statoFinestra == FormWindowState.Maximized.ToString())
                                WindowState = FormWindowState.Maximized;
                            else if (statoFinestra == FormWindowState.Minimized.ToString())
                                WindowState = FormWindowState.Minimized;
                            else
                            {
                                WindowState = FormWindowState.Normal;
                                Location = posizione;
                                Size = dimensione;
                            }
                            break;
                        case "Sfoglia":
                            Visualizza formVisualizza = null;
                            byte libro, capitolo, versetto;
                            try
                            {
                                string tuttiTesti = (nodo.SelectSingleNode("all") == null ? TestoTipi.None.ToString() : nodo.SelectSingleNode("all").InnerText);
                                string versione;
                                TestoTipi tipo = TestoTipi.None;
                                if (tuttiTesti == TestoTipi.None.ToString())
                                {
                                    versione = nodo.SelectSingleNode("text").InnerText;
                                    if (nodo.SelectSingleNode("texttype") != null)
                                    {
                                        switch (nodo.SelectSingleNode("texttype").InnerText)
                                        {
                                            case "Bibbia":
                                                tipo = TestoTipi.Bibbia;
                                                break;
                                            case "Commentario":
                                                tipo = TestoTipi.Commentario;
                                                break;
                                            case "Dizionario":
                                                tipo = TestoTipi.Dizionario;
                                                break;
                                        }
                                    }
                                    if (tipo == TestoTipi.None)
                                        tipo = testi.TipoPrincipaleDiTesto(versione);
                                    if (tipo != TestoTipi.None)
                                        formVisualizza = new Visualizza(this, versione, tipo);
                                }
                                else
                                {
                                    switch (tuttiTesti)
                                    {
                                        case "Bibbia":
                                            formVisualizza = new Visualizza(this, TestoTipi.Bibbia);
                                            tipo = TestoTipi.Bibbia;
                                            break;
                                        case "Commentario":
                                            formVisualizza = new Visualizza(this, TestoTipi.Commentario);
                                            tipo = TestoTipi.Commentario;
                                            break;
                                        case "Dizionario":
                                            formVisualizza = new Visualizza(this, TestoTipi.Dizionario);
                                            tipo = TestoTipi.Dizionario;
                                            break;
                                    }
                                }
                                string sincStringa = nodo.SelectSingleNode("syncronize").InnerText;
                                if (string.IsNullOrEmpty(sincStringa))
                                    sincStringa = "0";
                                formVisualizza.paneAttivo.SincNumero = Convert.ToInt32(sincStringa, CultureInfo.InvariantCulture);
                                if (tipo == TestoTipi.Dizionario)
                                {
                                    formVisualizza.SpostaTesto(nodo.SelectSingleNode("item").InnerText, false);
                                }
                                else
                                {
                                    libro = Convert.ToByte(nodo.SelectSingleNode("book").InnerText, CultureInfo.InvariantCulture);
                                    capitolo = Convert.ToByte(nodo.SelectSingleNode("chapter").InnerText, CultureInfo.InvariantCulture);
                                    versetto = Convert.ToByte(nodo.SelectSingleNode("verse").InnerText, CultureInfo.InvariantCulture);
                                    formVisualizza.SpostaTesto(libro, capitolo, versetto, false);
                                }
                                formVisualizza.paneAttivo.Zoom = Convert.ToSingle(nodo.SelectSingleNode("zoom").InnerText, CultureInfo.InvariantCulture);
                                PosizionaFinestra(formVisualizza, posizione, dimensione, statoFinestra);
                                foreach (XmlNode nodoPane in nodo.SelectNodes("pane"))
                                {
                                    tuttiTesti = nodoPane.SelectSingleNode("all").InnerText;
                                    if (tuttiTesti == TestoTipi.None.ToString())
                                    {
                                        versione = nodoPane.SelectSingleNode("text").InnerText;
                                        tipo = TestoTipi.None;
                                        if (nodoPane.SelectSingleNode("texttype") != null)
                                        {
                                            switch (nodoPane.SelectSingleNode("texttype").InnerText)
                                            {
                                                case "Bibbia":
                                                    tipo = TestoTipi.Bibbia;
                                                    break;
                                                case "Commentario":
                                                    tipo = TestoTipi.Commentario;
                                                    break;
                                                case "Dizionario":
                                                    tipo = TestoTipi.Dizionario;
                                                    break;
                                            }
                                        }
                                        if (tipo == TestoTipi.None)
                                            tipo = testi.TipoPrincipaleDiTesto(versione);
                                    }
                                    else
                                    {
                                        // attualmente, questo caso non può esistere, perché solo il primo pane può avere tutti i testi, 
                                        // perché il pulsante "aggiungi testo" non include "tutti i ..."
                                        versione = "";
                                    }
                                    int larghezza = Convert.ToInt32(nodoPane.SelectSingleNode("width").InnerText, CultureInfo.InvariantCulture);
                                    if (tipo != TestoTipi.None)
                                    {
                                        formVisualizza.AggiungiPane(versione, tipo, larghezza);
                                        // la riga precedente imposta questo nuovo pane come quello attivo
                                        formVisualizza.paneAttivo.SincNumero = Convert.ToInt32(nodoPane.SelectSingleNode("syncronize").InnerText, CultureInfo.InvariantCulture);
                                        if (tipo == TestoTipi.Dizionario)
                                        {
                                            formVisualizza.SpostaTesto(nodoPane.SelectSingleNode("item").InnerText, false);
                                        }
                                        else
                                        {
                                            libro = Convert.ToByte(nodoPane.SelectSingleNode("book").InnerText, CultureInfo.InvariantCulture);
                                            capitolo = Convert.ToByte(nodoPane.SelectSingleNode("chapter").InnerText, CultureInfo.InvariantCulture);
                                            versetto = Convert.ToByte(nodoPane.SelectSingleNode("verse").InnerText, CultureInfo.InvariantCulture);
                                            formVisualizza.SpostaTesto(libro, capitolo, versetto, false);
                                        }
                                        formVisualizza.paneAttivo.Zoom = Convert.ToSingle(nodoPane.SelectSingleNode("zoom").InnerText, CultureInfo.InvariantCulture);
                                    }
                                }
                            }
                            catch (TextNotExistException)
                            {
                                // text non esiste più; semplicemente non mostriamo questo (e gli altri) pane della finestra
                            }
                            break;
                        case "Ricerca":
                            Ricerca formRicerca = new Ricerca(this);
                            PosizionaFinestra(formRicerca, posizione, dimensione, statoFinestra);
                            break;
                        case "ScegliParola":
                            string versioneSP = nodo.SelectSingleNode("text").InnerText;
                            if (!string.IsNullOrEmpty(versioneSP))
                            {
                                try
                                {
                                    ScegliParola formScegliParola = new ScegliParola(this, versioneSP);
                                    PosizionaFinestra(formScegliParola, posizione, dimensione, statoFinestra);
                                }
                                catch (TextNotExistException)
                                {
                                    // collezione non esiste più; semplicemente non mostriamo la finestra
                                }
                            }
                            break;
                        case "Navigatore":
                            Navigatore formNavigatore = new Navigatore(this);
                            int splitter1 = Convert.ToInt32(nodo.SelectSingleNode("splitter1").InnerText, CultureInfo.InvariantCulture);
                            int splitter2 = Convert.ToInt32(nodo.SelectSingleNode("splitter2").InnerText, CultureInfo.InvariantCulture);
                            PosizionaFinestra(formNavigatore, posizione, dimensione, statoFinestra);
                            formNavigatore.Splitter1 = splitter1;
                            formNavigatore.Splitter2 = splitter2;
                            break;
                        case "Mostra":
                            Mostra formMostra = new Mostra(this);
                            PosizionaFinestra(formMostra, posizione, dimensione, statoFinestra);
                            break;
                        case "Chiave":
                            Chiave formChiave = new Chiave(this);
                            PosizionaFinestra(formChiave, posizione, dimensione, statoFinestra);
                            break;
                        case "Informazioni":
                            Informazioni formInformazioni = new Informazioni(this);
                            PosizionaFinestra(formInformazioni, posizione, dimensione, statoFinestra);
                            break;
                        case "Simili":
                            BraniSimili formSimili = new BraniSimili(this);
                            PosizionaFinestra(formSimili, posizione, dimensione, statoFinestra);
                            break;
                        case "BraniParalleli":
                            string nomeBraniParalleli = nodo.SelectSingleNode("parallels").InnerText;
                            InfoBraniParalleli infoBraniParalleli = null;
                            foreach (ToolStripMenuItem ts in parallelsToolStripMenuItem.DropDownItems)
                            {
                                if (ts.Text == nomeBraniParalleli)
                                {
                                    infoBraniParalleli = (InfoBraniParalleli)(ts.Tag);
                                    break;
                                }
                            }
                            if (infoBraniParalleli != null)
                            {
                                BraniParalleli formBraniParalleli = new BraniParalleli(this, infoBraniParalleli);
                                PosizionaFinestra(formBraniParalleli, posizione, dimensione, statoFinestra);
                                formBraniParalleli.NumeroBranoMostrato = Convert.ToInt32(nodo.SelectSingleNode("parallel").InnerText, CultureInfo.InvariantCulture);
                            }
                            break;
                        case "Lettura":
                            string piano = nodo.SelectSingleNode("plan").InnerText;
                            Lettura formLettura = new Lettura(this, piano);
                            PosizionaFinestra(formLettura, posizione, dimensione, statoFinestra);
                            break;
                        case "Misure":
                            Misure formMisure = new Misure(this);
                            PosizionaFinestra(formMisure, posizione, dimensione, statoFinestra);
                            break;
                        case "Segnalibri":
                            Segnalibri formSegnalibri = new Segnalibri(this);
                            PosizionaFinestra(formSegnalibri, posizione, dimensione, statoFinestra);
                            break;
                        case "ApriNota":
                            string versioneAN = nodo.SelectSingleNode("text").InnerText;
                            if (!string.IsNullOrEmpty(versioneAN))
                            {
                                try
                                {
                                    ApriNota formApriNota = new ApriNota(this, versioneAN);
                                    PosizionaFinestra(formApriNota, posizione, dimensione, statoFinestra);
                                }
                                catch (TextNotExistException)
                                {
                                    // collezione non esiste più; semplicemente non mostriamo la finestra
                                }
                            }
                            break;
                        case "Immagine":
                            string fileImmagine = nodo.SelectSingleNode("file").InnerText;
                            if (!string.IsNullOrEmpty(fileImmagine))
                            {
                                try
                                {
                                    Immagine formImmagine = new Immagine(fileImmagine);
                                    PosizionaFinestra(formImmagine, posizione, dimensione, statoFinestra);
                                }
                                catch
                                {
                                    // file non esiste più; semplicemente non mostriamo la finestra
                                }
                            }
                            break;
                    }
                    Application.DoEvents();
                }
            }
            catch (Exception)
            {
                // errore nell'XML, saltiamo il file
            }
        }

        private void arrangementSaveCurrentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string nome = ConvertiANomeNonUsato(LocRM.GetString("MainArrangementNewName"));
            using (InputBox inputBox = new InputBox(LocRM.GetString("MainArrangementSaveCaption"), LocRM.GetString("MainArrangementSaveQuestion"), nome))
            {
                inputBox.ShowDialog();
                nome = inputBox.Risposta;
            }
            if (!string.IsNullOrEmpty(nome))
            {
                nome = ConvertiANomeNonUsato(nome);
                try
                {
                    string nomeFile = SalvaDisposizioneAttuale(nome + ".xml");
                    InfoDisposizione info = new InfoDisposizione
                    {
                        nome = nome,
                        nomeFile = nomeFile
                    };

                    ToolStripMenuItem voce = new ToolStripMenuItem(nome, null, arrangementToolStripMenuItem_Click)
                    {
                        Tag = info
                    };
                    arrangementMenu.DropDownItems.Insert(arrangementMenu.DropDownItems.Count - 3, voce);
                    ToolStripMenuItem voceCancella = new ToolStripMenuItem(nome, null, arrangementDeleteToolStripMenuItem_Click)
                    {
                        Tag = info
                    };
                    arrangementDeleteToolStripMenuItem.DropDownItems.Add(voceCancella);
                    arrangementDeleteToolStripMenuItem.Enabled = true;
                }
                catch (Exception eccezione) // se non è stato possibile salvare al file nome.xml
                {
                    MessageBox.Show(string.Format(CultureInfo.CurrentCulture, LocRM.GetString("MainArrangementError"), nome + ".xml", eccezione.Message), LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, messageBoxOptions);
                }
            }
        }

        private string SalvaDisposizioneAttuale(string nome)
        {
            string nomeFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + "Disposizioni" + Path.DirectorySeparatorChar + Funzioni.RimuoviCaratteriNonValidiInPercorsi(nome);
            string testoFile = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>";
            testoFile += Environment.NewLine + "<windows>";
            string nomeEncoded = Funzioni.RimuoviCaratteriNonValidiInXml(nome), tag;
            if (nomeEncoded.ToUpperInvariant().EndsWith(".XML", StringComparison.Ordinal))
                nomeEncoded = nomeEncoded.Substring(0, nomeEncoded.Length - 4);
            testoFile += Environment.NewLine + "<name>" + nomeEncoded + "</name>";
            testoFile += Environment.NewLine + "<window>";
            testoFile += Environment.NewLine + "<type>Principale</type>";
            testoFile += Environment.NewLine + "<state>" + WindowState + "</state>";
            testoFile += Environment.NewLine + "<positionX>" + Location.X + "</positionX>";
            testoFile += Environment.NewLine + "<positionY>" + Location.Y + "</positionY>";
            testoFile += Environment.NewLine + "<width>" + Width + "</width>";
            testoFile += Environment.NewLine + "<height>" + Height + "</height>";
            testoFile += Environment.NewLine + "</window>";
            foreach (Form formFiglio in MdiChildren)
            {
                tag = formFiglio.Tag.ToString();
                if (tag == "Visualizza")
                    tag = "Sfoglia";  // necessario, per compatabilità con versioni precedenti del programma
                if (tag != "Editor")
                {
                    testoFile += Environment.NewLine + "<window>";
                    testoFile += Environment.NewLine + "<type>" + tag + "</type>";
                    testoFile += Environment.NewLine + "<state>" + formFiglio.WindowState + "</state>";
                    testoFile += Environment.NewLine + "<positionX>" + formFiglio.Location.X + "</positionX>";
                    testoFile += Environment.NewLine + "<positionY>" + formFiglio.Location.Y + "</positionY>";
                    testoFile += Environment.NewLine + "<width>" + formFiglio.Width + "</width>";
                    testoFile += Environment.NewLine + "<height>" + formFiglio.Height + "</height>";
                    if (tag == "BraniParalleli")
                    {
                        BraniParalleli formBrani = ((BraniParalleli)formFiglio);
                        testoFile += Environment.NewLine + "<parallels>" + formBrani.GruppoParalleli + "</parallels>";
                        testoFile += Environment.NewLine + "<parallel>" + formBrani.NumeroBranoMostrato + "</parallel>";
                    }
                    else if (tag == "Lettura")
                    {
                        testoFile += Environment.NewLine + "<plan>" + ((Lettura)formFiglio).Piano + "</plan>";
                    }
                    else if (tag == "ApriNota")
                    {
                        testoFile += Environment.NewLine + "<text>" + ((ApriNota)formFiglio).Versione + "</text>";
                    }
                    else if (tag == "ScegliParola")
                    {
                        testoFile += Environment.NewLine + "<text>" + ((ScegliParola)formFiglio).Versione + "</text>";
                    }
                    else if (tag == "Navigatore")
                    {
                        Navigatore formNavigatore = (Navigatore)formFiglio;
                        testoFile += Environment.NewLine + "<splitter1>" + formNavigatore.Splitter1 + "</splitter1>";
                        testoFile += Environment.NewLine + "<splitter2>" + formNavigatore.Splitter2 + "</splitter2>";
                    }
                    else if (tag == "Sfoglia")
                    {
                        Visualizza formComeVisualizza = (Visualizza)formFiglio;
                        for (int i = 0; i < formComeVisualizza.panes.Count; ++i)
                        {
                            if (i != 0) // per compatibilità con versioni prima della 7.08, il primo testo non è contato come pane qui
                                testoFile += Environment.NewLine + "<pane>";
                            testoFile += Environment.NewLine + "<all>" + formComeVisualizza.panes[i].TuttiTesti + "</all>";
                            testoFile += Environment.NewLine + "<text>" + formComeVisualizza.panes[i].Versione + "</text>";
                            testoFile += Environment.NewLine + "<texttype>" + formComeVisualizza.panes[i].TipoTesto.ToString() + "</texttype>";
                            if (formComeVisualizza.panes[i].TipoTesto == TestoTipi.Dizionario)
                            {
                                testoFile += Environment.NewLine + "<item>" + formComeVisualizza.panes[i].Voce + "</item>";
                            }
                            else
                            {
                                string riferimentoAttuale = formComeVisualizza.panes[i].PostoAttuale;
                                Riferimento riferimentoDaSalvare = testi.ConvertiRiferimento(riferimentoAttuale);
                                byte libro, capitolo, versetto;
                                if (riferimentoDaSalvare.Count == 0)
                                {
                                    libro = formComeVisualizza.panes[i].Libro;
                                    capitolo = formComeVisualizza.panes[i].Capitolo;
                                    versetto = formComeVisualizza.panes[i].Versetto;
                                }
                                else
                                {
                                    libro = riferimentoDaSalvare.Brani[0][0];
                                    capitolo = riferimentoDaSalvare.Brani[0][1];
                                    versetto = riferimentoDaSalvare.Brani[0][2];
                                }
                                testoFile += Environment.NewLine + "<book>" + libro + "</book>";
                                testoFile += Environment.NewLine + "<chapter>" + capitolo + "</chapter>";
                                testoFile += Environment.NewLine + "<verse>" + versetto + "</verse>";
                            }
                            testoFile += Environment.NewLine + "<syncronize>" + formComeVisualizza.panes[i].SincNumero.ToString(CultureInfo.InvariantCulture) + "</syncronize>";
                            testoFile += Environment.NewLine + "<zoom>" + formComeVisualizza.panes[i].Zoom.ToString(CultureInfo.InvariantCulture) + "</zoom>";
                            //testoFile += Environment.NewLine + "<positionX>" + formComeVisualizza.panes[i].Location.X.ToString(CultureInfo.InvariantCulture) + "</positionX>";
                            //testoFile += Environment.NewLine + "<positionY>" + formComeVisualizza.panes[i].Location.Y.ToString(CultureInfo.InvariantCulture) + "</positionY>";
                            if (i != 0) // il primo ha DockType Fill, e riempie tutto lo spazio lasciato da eventuali altri pane
                                testoFile += Environment.NewLine + "<width>" + formComeVisualizza.panes[i].Size.Width.ToString(CultureInfo.InvariantCulture) + "</width>";
                            //testoFile += Environment.NewLine + "<height>" + formComeVisualizza.panes[i].Size.Height.ToString(CultureInfo.InvariantCulture) + "</height>";
                            if (i != 0)
                                testoFile += Environment.NewLine + "</pane>";
                        }
                    }
                    else if (tag == "Immagine")
                    {
                        testoFile += Environment.NewLine + "<file>" + ((Immagine)formFiglio).NomeFile + "</file>";
                    }
                    testoFile += Environment.NewLine + "</window>";
                }
            }
            testoFile += Environment.NewLine + "</windows>";
            File.WriteAllText(nomeFile, testoFile);

            return nomeFile;
        }

        private string ConvertiANomeNonUsato(string nome)
        {
            string nomeBase = nome;
            bool nomeEsiste;
            int suffiso = 0;
            do
            {
                nomeEsiste = false;
                foreach (ToolStripItem ts in arrangementMenu.DropDownItems)
                {
                    if (ts.Text == nome)
                    {
                        nomeEsiste = true;
                        ++suffiso;
                        nome = nomeBase + suffiso.ToString(CultureInfo.InvariantCulture);
                        break;
                    }
                }
            } while (nomeEsiste);
            return nome;
        }

        private void arrangementDeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripItem voce = ((ToolStripItem)(sender));
            if (voce.Tag != null)
            {
                InfoDisposizione info = (InfoDisposizione)(voce.Tag);
                File.Delete(info.nomeFile);
                foreach (ToolStripItem voceDisposizione in arrangementMenu.DropDownItems)
                {
                    if (voceDisposizione.Text == voce.Text)
                    {
                        arrangementMenu.DropDownItems.Remove(voceDisposizione);
                        break;
                    }
                }
                arrangementDeleteToolStripMenuItem.DropDownItems.Remove(voce);
                arrangementDeleteToolStripMenuItem.Enabled = (arrangementDeleteToolStripMenuItem.DropDownItems.Count > 0);
            }
        }

        private void PosizionaFinestra(Form nuovoForm, Point posizione, Size dimensione, string statoFinestra)
        {
            nuovoForm.MdiParent = this;
            nuovoForm.Show();
            if (statoFinestra == FormWindowState.Maximized.ToString())
                nuovoForm.WindowState = FormWindowState.Maximized;
            else if (statoFinestra == FormWindowState.Minimized.ToString())
                nuovoForm.WindowState = FormWindowState.Minimized;
            else
            {
                nuovoForm.Location = posizione;
                nuovoForm.Size = dimensione;
            }
        }

        public Collection<string> DisposizioniFinestre()
        {
            Collection<string> disposizioni = new Collection<string>
            {
                arrangementMenu.DropDownItems[0].Text // Empty/Vuoto
            };
            foreach (ToolStripItem voce in arrangementDeleteToolStripMenuItem.DropDownItems)
                disposizioni.Add(voce.Text);
            return disposizioni;
        }

        #endregion

        #endregion

        #region Events

        internal void Principale_MdiChildActivate(object sender, EventArgs e)
        {
            closeToolStripMenuItem.Enabled = (ActiveMdiChild != null);
            closeToolStripButton.Enabled = closeToolStripMenuItem.Enabled;

            bool editorAttivo = (ActiveMdiChild != null && ActiveMdiChild.Tag != null && ActiveMdiChild.Tag.ToString() == "Editor");
            bool visualizzaAttivo = (ActiveMdiChild != null && ActiveMdiChild.Tag != null && ActiveMdiChild.Tag.ToString() == "Visualizza");
            bool letturaAttivo = (ActiveMdiChild != null && ActiveMdiChild.Tag != null && ActiveMdiChild.Tag.ToString() == "Lettura" && ((Lettura)ActiveMdiChild).UltimaRtb != null);
            bool editorOVisualizzaAttivi = editorAttivo | visualizzaAttivo;
            bool editorOVisualizzaOLetturaAttivi = editorOVisualizzaAttivi | letturaAttivo;

            zoomToolStripMenuItem.Enabled = editorOVisualizzaOLetturaAttivi;
            zoomToolStripMenuItem.DropDown.Enabled = editorOVisualizzaOLetturaAttivi;
            hypertextJumpToolStripMenuItem.Enabled = editorOVisualizzaAttivi;
            // qui
            Visualizza.Pane pane = null;
            if (visualizzaAttivo)
            {
                ultimaVisualizza = (Visualizza)ActiveMdiChild;
                pane = ultimaVisualizza.paneAttivo;

                formatToolStrip.Visible = false;
                browseToolStrip.Visible = Settings.Default.PrincipaleBSFormato;

                undoToolStripButton.Enabled = false;
                undoToolStripButton.Text = undoCaption;
                redoToolStripButton.Enabled = false;
                redoToolStripButton.Text = redoCaption;
                bool testoSelezionato = (ultimaVisualizza.SelectionLength > 0);
                copyToolStripButton.Enabled = testoSelezionato;
                copyToolStripMenuItem.Enabled = testoSelezionato;
                printToolStripButton.Enabled = testoSelezionato;
                printToolStripMenuItem.Enabled = testoSelezionato;
                highlightToolStripMenuItem.Enabled = testoSelezionato;
                highlightBrowseToolStripSplitButton.Enabled = testoSelezionato;
                browseZoomToolStripComboBox.Text = Convert.ToInt32((pane.Zoom * 100.0F)).ToString(CultureInfo.InvariantCulture) + "%";

                ImpostaBarraDiStato(pane.PostoAttuale);
                AggiornaBarraVisualizza(pane);
            }
            else
            {
                browseToolStrip.Visible = false;
                formatToolStrip.Visible = Settings.Default.PrincipaleBSFormato;
                formatToolStrip.Enabled = editorAttivo;

                fontToolStripMenuItem.Enabled = editorAttivo;
                paragraphToolStripMenuItem.Enabled = editorAttivo;
                bool testoSelezionato = false;
                if (editorAttivo)
                    testoSelezionato = (((Editor)ActiveMdiChild).rtEditor.SelectionLength > 0);

                highlightToolStripMenuItem.Enabled = testoSelezionato;
                highlightFormatToolStripSplitButton.Enabled = testoSelezionato;
                hypertextToolStripMenuItem.Enabled = testoSelezionato;
                hypertextToolStripSplitButton.Enabled = testoSelezionato;

                undoToolStripButton.Enabled = editorAttivo;
                redoToolStripButton.Enabled = editorAttivo;

                printToolStripMenuItem.Enabled = editorAttivo;
                printToolStripButton.Enabled = editorAttivo;

                if (editorAttivo)
                {
                    AggiornaPulsanti(((Editor)ActiveMdiChild).rtEditor);
                    zoomToolStripComboBox.Text = Convert.ToInt32((((Editor)ActiveMdiChild).rtEditor.ZoomFactor * 100.0F)).ToString(CultureInfo.InvariantCulture) + "%";
                }
                else
                    AggiornaPulsanti(null);

                pasteToolStripButton.Enabled = editorAttivo;
            }

            if (editorAttivo)
            {
                orderToolStrip.Visible = (((Editor)ActiveMdiChild).MostraOrdine && Settings.Default.PrincipaleBSOrdine);
                orderPreviousToolStripButton.Enabled = !string.IsNullOrEmpty(((Editor)ActiveMdiChild).NotaPrecedente);
                orderNextToolStripButton.Enabled = !string.IsNullOrEmpty(((Editor)ActiveMdiChild).NotaProssima);
                orderIndexToolStripButton.Enabled = !string.IsNullOrEmpty(((Editor)ActiveMdiChild).NotaIndice);
            }
            else if (visualizzaAttivo)
            {
                ImpostaBarraOrdinePerVisualizza(pane);
            }
            else
            {
                orderToolStrip.Visible = false;
            }

            saveAsToolStripMenuItem.Enabled = editorAttivo;
            saveToolStripMenuItem.Enabled = editorAttivo;
            saveToolStripButton.Enabled = editorAttivo;
            printPreviewToolStripMenuItem.Enabled = editorAttivo;
            printPreviewToolStripButton.Enabled = editorAttivo;
            printSetupToolStripMenuItem.Enabled = editorAttivo;

            pasteToolStripMenuItem.Enabled = editorAttivo;
            pasteToolStripButton.Enabled = editorAttivo;
            selectAllToolStripMenuItem.Enabled = editorAttivo;
            findToolStripMenuItem.Enabled = editorAttivo;
            findToolStripButton.Enabled = editorAttivo;
            findAgainToolStripMenuItem.Enabled = editorAttivo;
            findAgainToolStripButton.Enabled = editorAttivo;
            replaceToolStripMenuItem.Enabled = editorAttivo;
            replaceToolStripButton.Enabled = editorAttivo;
            replaceAgainToolStripMenuItem.Enabled = editorAttivo;

            if (ActiveMdiChild != null && ActiveMdiChild.Tag != null)
            {
                switch (ActiveMdiChild.Tag.ToString())
                {
                    case "Editor":
                        AggiornaPulsanti(((Editor)ActiveMdiChild).rtEditor);
                        break;
                    case "Visualizza":
                        AggiornaPulsanti(((Visualizza)ActiveMdiChild).paneAttivo.Rtf);
                        break;
                    default:
                        AggiornaPulsanti(null);
                        break;
                    //                    case "Lettura":
                    //                        AggiornaPulsanti(((Lettura)ActiveMdiChild).UltimaRtb);
                    //                        break;
                    //                    case "BraniParalleli":
                    //                        AggiornaPulsanti(((BraniParalleli)ActiveMdiChild).UltimaRtb);
                    //                        break;
                }
            }
        }

        internal void ImpostaBarraOrdinePerVisualizza(Visualizza.Pane pane)
        {
            if (pane.TipoTesto == TestoTipi.Dizionario && pane.TuttiTesti == TestoTipi.None)
            {
                Collection<string> ordine = testi.GetNoteInOrdine(pane.Versione);
                orderToolStrip.Visible = ((ordine.Count > 1 || (ordine.Count == 1 && !string.IsNullOrEmpty(ordine[0]))));
                if (orderToolStrip.Visible)
                {
                    string[] notePrecedenteSuccessiva = testi.NotePrecedenteSuccessiva(pane.Versione, pane.Voce);
                    orderPreviousToolStripButton.Enabled = !string.IsNullOrEmpty(notePrecedenteSuccessiva[0]);
                    orderNextToolStripButton.Enabled = !string.IsNullOrEmpty(notePrecedenteSuccessiva[1]);
                    orderIndexToolStripButton.Enabled = !string.IsNullOrEmpty(ordine[0]);
                }
                orderToolStrip.Visible = orderToolStrip.Visible && Settings.Default.PrincipaleBSOrdine;
            }
            else
                orderToolStrip.Visible = false;
        }

        internal void AggiornaBarraVisualizza(Visualizza.Pane pane)
        {
            bool nonDizionario = (pane.TipoTesto != TestoTipi.Dizionario);

            browseToolStripComboBox.Visible = nonDizionario;
            browseNotesToolStripComboBox.Visible = !nonDizionario;
            if (!nonDizionario)
            {
                string versione = pane.Versione;
                browseNotesToolStripComboBox.Items.Clear();
                if (!string.IsNullOrEmpty(versione))
                {
                    browseNotesToolStripComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                    browseNotesToolStripComboBox.Items.AddRange(new List<string>(Principale.testi.NotePrimaOrdinate(versione, false)).ToArray());
                }
                else
                {
                    browseNotesToolStripComboBox.DropDownStyle = ComboBoxStyle.DropDown;
                }
            }

            gotoToolStripButton.Enabled = (nonDizionario ? browseToolStripComboBox.Text.Length > 0 : browseNotesToolStripComboBox.Text.Length > 0);

            bookToolStripLabel.Visible = nonDizionario;
            bookUDToolStripButton.Visible = nonDizionario;
            chapterToolStripLabel.Visible = nonDizionario;
            chapterUDToolStripButton.Visible = nonDizionario;
            verseToolStripLabel.Visible = nonDizionario;
            verseUDToolStripButton.Visible = nonDizionario;
            toolStripSeparatorBCV.Visible = nonDizionario;

            browseBookmarkBookmarkToolStripComboBox.Visible = nonDizionario;
            browseBookmarkListToolStripComboBox.Visible = nonDizionario;
            browseBookmarkNextToolStripButton.Visible = nonDizionario;
            browseBookmarkPreviousToolStripButton.Visible = nonDizionario;
            toolStripSeparatorBookmark.Visible = nonDizionario;

            bool tutti = (pane.TuttiTesti != TestoTipi.None);
            browseSearchToolStripComboBox.Visible = !tutti;
            browseSearchGotoToolStripButton.Visible = !tutti;
            browseSearchVersesToolStripComboBox.Visible = !tutti;
            toolStripSeparatorSearch.Visible = !tutti;

            browseBackToolStripButton.Visible = nonDizionario;
            browseForwardToolStripButton.Visible = nonDizionario;
            toolStripSeparatorBrowse.Visible = nonDizionario;
        }

        private void Principale_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.Cancel)
            {
                // per una finestra dell'editor, alla domanda "Salva?", l'utente ha detto Annulla. 
                // quindi non serve salvare collezioni modificate né impostazioni
                // infatti, salvare collezioni modificate fa danni, perché non tutte le finestre erano chiusi quindi non tutte le note erano salvate
                return;
            }

            SalvaDisposizioneAttuale("disposizione all'uscita del programma");
            //            SalvaElencoAddins();

            if (testi.NoteModificate())
                SetBarraDiStatoTesto(LocRM.GetString("MainNoteChanged"));
            Cursor.Current = Cursors.WaitCursor;
            Application.DoEvents();
            string versioniNonSalvate = testi.Chiudi();
            if (!String.IsNullOrEmpty(versioniNonSalvate))
                MessageBox.Show(String.Format(CultureInfo.InvariantCulture, LocRM.GetString("MainErrorChangesNotSaved"), versioniNonSalvate), LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, messageBoxOptions);

            int nBraniDaSalvare = Settings.Default.MiscRichiesteMemorizzate;
            if (browseToolStripComboBox.Items.Count < nBraniDaSalvare)
                nBraniDaSalvare = browseToolStripComboBox.Items.Count;
            StringBuilder braniDaSalvare = new StringBuilder("");
            for (int i = 0; i < nBraniDaSalvare; ++i)
                braniDaSalvare.Append("|").Append(browseToolStripComboBox.Items[i]);
            Settings.Default.PrincipaleSfogliaBraniPrecedenti = braniDaSalvare.ToString();

            int nRicercheDaSalvare = Settings.Default.MiscRichiesteMemorizzate;
            if (browseSearchToolStripComboBox.Items.Count < nRicercheDaSalvare)
                nRicercheDaSalvare = browseSearchToolStripComboBox.Items.Count;
            StringBuilder ricercheDaSalvare = new StringBuilder("");
            for (int i = 0; i < nRicercheDaSalvare; ++i)
                ricercheDaSalvare.Append("§").Append(browseSearchToolStripComboBox.Items[i]);
            Settings.Default.PrincipaleSfogliaRicerchePrecedenti = ricercheDaSalvare.ToString();

            int nComandiDaSalvare = Settings.Default.MiscRichiesteMemorizzate;
            if (comandoToolStripComboBox.Items.Count < nComandiDaSalvare)
                nComandiDaSalvare = comandoToolStripComboBox.Items.Count;
            StringBuilder comandiDaSalvare = new StringBuilder("");
            for (int i = 0; i < nComandiDaSalvare; ++i)
                comandiDaSalvare.Append("§").Append(comandoToolStripComboBox.Items[i]);
            Settings.Default.PrincipaleComandiPrecedenti = comandiDaSalvare.ToString();

            Settings.Default.UltimaBibbia = testi.UltimaBibbia;
            Settings.Default.UltimaBibbiaCompleta = testi.UltimaBibbiaCompleta;
            Settings.Default.PrincipaleWindowState = WindowState;
            if (WindowState != FormWindowState.Maximized)
            {
                Settings.Default.PrincipaleWindowSize = Size;
                Settings.Default.PrincipaleWindowLocation = Location;
            }

            StringBuilder fontPreferitiStringa = new StringBuilder(100);
            foreach (string fontPredefinito in fontPreferiti)
                fontPreferitiStringa.Append(fontPredefinito).Append("|");
            Settings.Default.PrincipaleFontPreferiti = fontPreferitiStringa.ToString();

            string[] fileDaCancellare = Directory.GetFiles(Path.GetTempPath(), "laparola*.html");
            foreach (string fileTemp in fileDaCancellare)
                File.Delete(fileTemp);

            Settings.Default.Save();
        }

        private static void SalvaElencoAddins()
        {
            string nomeFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola" + Path.DirectorySeparatorChar + "addins.xml";
            string testoFile = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>";
            testoFile += Environment.NewLine + "<addins>";
            foreach (string s in testi.NomiVersioni())
            {
                testoFile += Environment.NewLine + "<addin>";
                testoFile += Environment.NewLine + "<name>" + s + "</name>";
                testoFile += Environment.NewLine + "<type>" + testi.Info(s).Tipo.ToString() + "</type>";
                testoFile += Environment.NewLine + "</addin>";
                // TODO (?) braniparalleli, letture, collegamenti, disposizioni, segnalibri
            }
            testoFile += Environment.NewLine + "</addins>";
            File.WriteAllText(nomeFile, testoFile);
        }

        private void formatToolStrip_ParentChanged(object sender, EventArgs e)
        {
            fontToolStripComboBox.Visible = (formatToolStrip.Parent != toolStripPanelLeft && formatToolStrip.Parent != toolStripPanelRight);
            fontSizeToolStripComboBox.Visible = (formatToolStrip.Parent != toolStripPanelLeft && formatToolStrip.Parent != toolStripPanelRight);
        }

        private void msBarreDiStrumenti_Opening(object sender, CancelEventArgs e)
        {
            bsPrincipaleToolStripMenuItem.Checked = Settings.Default.PrincipaleBSPrincipale;
            bsFormatoToolStripMenuItem.Checked = Settings.Default.PrincipaleBSFormato;
            bsOrdineToolStripMenuItem.Checked = Settings.Default.PrincipaleBSOrdine;
            bsRigaComanoToolStripMenuItem.Checked = Settings.Default.PrincipaleBSComando;
        }

        private void barraStrumentiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.Default.PrincipaleBSPrincipale = bsPrincipaleToolStripMenuItem.Checked;
            Settings.Default.PrincipaleBSFormato = bsFormatoToolStripMenuItem.Checked;
            Settings.Default.PrincipaleBSOrdine = bsOrdineToolStripMenuItem.Checked;
            Settings.Default.PrincipaleBSComando = bsRigaComanoToolStripMenuItem.Checked;
            MostraNascondiBarreStrumenti();
        }

        private void MostraNascondiBarreStrumenti()
        {
            mainToolStrip.Visible = Settings.Default.PrincipaleBSPrincipale;
            formatToolStrip.Visible = Settings.Default.PrincipaleBSFormato;
            browseToolStrip.Visible = Settings.Default.PrincipaleBSFormato;
            orderToolStrip.Visible = Settings.Default.PrincipaleBSOrdine;
            commandToolStrip.Visible = Settings.Default.PrincipaleBSComando;

            Principale_MdiChildActivate(null, null);
        }

        private void Principale_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Move;
        }

        private void Principale_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                ApriDiversiFile((string[])(e.Data.GetData(DataFormats.FileDrop)));
        }

        private void timerClipboard_Tick(object sender, EventArgs e)
        {
            string clipboard = "";
            try
            {
                clipboard = Clipboard.GetText();
            }
            catch { } // a volte la riga precedente dà un External Exception; non so perché
            if (clipboard != testoInClipboard)
            {
                testoInClipboard = clipboard;
                if (isRunningOnMono || (!isRunningOnMono && !ProgrammaAttivoNotMono()))
                {
                    if (!string.IsNullOrEmpty(clipboard) && (clipboard.Length < Settings.Default.PrincipaleClipboardLunghezzaMassima || Settings.Default.PrincipaleClipboardLunghezzaMassima == 0))
                    {
                        Riferimento riferimento = testi.ConvertiRiferimenti(clipboard);
                        if (riferimento.Count > 0)
                            MostraBranoInEditor(riferimento, testi.UltimaBibbia);
                    }
                }
            }
        }

        private bool ProgrammaAttivoNotMono()
        {
            return (SafeNativeMethods.GetForegroundWindow() == Handle);
        }

        #endregion

        #region VisualizzaControlli

        #region vai

        private void browseToolStripComboBox_Enter(object sender, EventArgs e)
        {
            browseToolStripComboBox.SelectAll();
        }

        private void browseToolStripComboBox_TextChanged(object sender, EventArgs e)
        {
            gotoToolStripButton.Enabled = browseToolStripComboBox.Text.Length > 0;
        }

        private void browseToolStripComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                gotoToolStripButton_Click(sender, null);
        }

        private void browseToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (faBrowseIndiceCambio)
            {
                faBrowseIndiceCambio = false;
                gotoToolStripButton_Click(sender, null);
            }
            faBrowseIndiceCambio = true;
        }

        private void browseNotesToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            browseNotesToolStripComboBox_TextChanged(sender, e);
            browseToolStripComboBox_SelectedIndexChanged(sender, e);
        }

        private void browseNotesToolStripComboBox_TextChanged(object sender, EventArgs e)
        {
            gotoToolStripButton.Enabled = browseNotesToolStripComboBox.Text.Length > 0;
        }

        private void gotoToolStripButton_Click(object sender, EventArgs e)
        {
            if (((Visualizza)ActiveMdiChild) != null)
            {
                if (browseToolStripComboBox.Visible)
                {
                    faBrowseIndiceCambio = false;
                    string riferimento = browseToolStripComboBox.Text;
                    if (browseToolStripComboBox.Items.IndexOf(riferimento) > -1)
                        browseToolStripComboBox.Items.RemoveAt(browseToolStripComboBox.Items.IndexOf(riferimento));
                    if (!String.IsNullOrEmpty(riferimento))
                        browseToolStripComboBox.Items.Insert(0, riferimento);
                    //                browseToolStripComboBox.Text = "";
                    browseToolStripComboBox.AutoCompleteMode = AutoCompleteMode.None;
                    // per costringere il combo box a rileggere i valori che possono essere usati per l'autocomplete
                    browseToolStripComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    browseToolStripComboBox.Text = riferimento;
                    browseToolStripComboBox.SelectAll();
                    ((Visualizza)ActiveMdiChild).SpostaTesto(testi.ConvertiRiferimento(riferimento), true);
                    faBrowseIndiceCambio = true;
                }
                else
                {
                    ((Visualizza)ActiveMdiChild).SpostaTesto(browseNotesToolStripComboBox.Text, true);
                }
            }
        }

        #endregion

        #region capitolo libro versetto

        private void UDToolStripButton_Click(object sender, EventArgs e)
        {
            if (((Visualizza)ActiveMdiChild) != null)
            {
                byte nuovoVersetto = ((Visualizza)ActiveMdiChild).paneAttivo.Versetto;
                byte nuovoCapitolo = ((Visualizza)ActiveMdiChild).paneAttivo.Capitolo;
                byte nuovoLibro = ((Visualizza)ActiveMdiChild).paneAttivo.Libro;
                switch (((ToolStripButton)sender).Name)
                {
                    case "verseUDToolStripButton":
                        nuovoVersetto = (byte)(nuovoVersetto + pulsanteUDGiu);
                        break;
                    case "chapterUDToolStripButton":
                        nuovoCapitolo = (byte)(nuovoCapitolo + pulsanteUDGiu);
                        nuovoVersetto = 1;
                        break;
                    case "bookUDToolStripButton":
                        nuovoLibro = (byte)(nuovoLibro + pulsanteUDGiu);
                        nuovoCapitolo = 1;
                        nuovoVersetto = 1;
                        break;
                }
                string versione = ((Visualizza)ActiveMdiChild).paneAttivo.Versione;
                if ((testi.Info(versione).Tipo & TestoTipi.Bibbia) != TestoTipi.Bibbia)
                    versione = testi.UltimaBibbiaCompleta;
                try
                {
                    if (nuovoVersetto > testi.VersettiInCapitolo(nuovoLibro, nuovoCapitolo, versione))
                    {
                        nuovoCapitolo += 1;
                        nuovoVersetto = 1;
                    }
                }
                catch { } // se libro +1 dall'ultimo libro oppure capitolo + 1 dall'ultimo capitolo

                // se spostiamo libro e il libro non esiste, ma altri libri prima/dopo esistono, andiamo al prossimo libro esistente
                if (nuovoLibro > 1 && testi.CapitoliFinoALibro(nuovoLibro, versione) > 0)
                {
                    try
                    {
                        if (nuovoCapitolo > testi.CapitoliInLibro(nuovoLibro, versione) && nuovoLibro >= 1)
                        {
                            do
                            {
                                nuovoLibro += (byte)pulsanteUDGiu;
                            } while (nuovoLibro >= 1 && nuovoLibro <= 73 && testi.CapitoliInLibro(nuovoLibro, versione) == 0);
                            nuovoCapitolo = 1;
                            nuovoVersetto = 1;
                        }
                    }
                    catch { } // se capitolo + 1 dall'ultimo capitolo
                }
                if (nuovoLibro > 73)
                {
                    // trovare l'ultimo libro con testo
                    nuovoLibro = 74;
                    do
                    {
                        nuovoLibro -= 1;
                    } while (testi.CapitoliInLibro(nuovoLibro, versione) == 0);
                    nuovoCapitolo = testi.CapitoliInLibro(nuovoLibro, versione);
                    nuovoVersetto = testi.VersettiInCapitolo(nuovoLibro, nuovoCapitolo, versione);
                }
                if (nuovoLibro < 1)
                {
                    nuovoLibro = 1;
                    nuovoCapitolo = 1;
                    nuovoVersetto = 1;
                }
                if (nuovoCapitolo < 1)
                {
                    UltimoCapitoloInLibroPrecedente(ref nuovoLibro, ref nuovoCapitolo, ref nuovoVersetto, versione);
                    nuovoVersetto = 1;
                }
                if (nuovoVersetto < 1)
                {
                    if (nuovoCapitolo > 1)
                    {
                        --nuovoCapitolo;
                        nuovoVersetto = testi.VersettiInCapitolo(nuovoLibro, nuovoCapitolo, versione);
                    }
                    else // l'utente è andato indietro dal primo versetto di un libro
                    {
                        if (nuovoLibro > 1)
                            UltimoCapitoloInLibroPrecedente(ref nuovoLibro, ref nuovoCapitolo, ref nuovoVersetto, versione);
                    }
                }

                ((Visualizza)ActiveMdiChild).SpostaTesto(nuovoLibro, nuovoCapitolo, nuovoVersetto, true);
            }
        }

        private static void UltimoCapitoloInLibroPrecedente(ref byte nuovoLibro, ref byte nuovoCapitolo, ref byte nuovoVersetto, string versione)
        {
            if (nuovoLibro > 1)
                --nuovoLibro;
            try
            {
                while (nuovoLibro >= 1 && testi.CapitoliInLibro(nuovoLibro, versione) == 0)
                    --nuovoLibro;
                nuovoCapitolo = testi.CapitoliInLibro(nuovoLibro, versione);
            }
            catch (ArgumentOutOfRangeException) // può succedere con una collezione di note
            {
                nuovoCapitolo = 1;
            }
            if (nuovoCapitolo < 1)
                nuovoCapitolo = 1;
            try
            {
                nuovoVersetto = testi.VersettiInCapitolo(nuovoLibro, nuovoCapitolo, versione);
            }
            catch (ArgumentOutOfRangeException) // può succedere con una collezione di note
            {
                nuovoVersetto = 1;
            }
        }

        private void UDToolStripButton_MouseDown(object sender, MouseEventArgs e)
        {
            pulsanteUDGiu = ((e.Y > ((ToolStripButton)sender).Height / 2) ? 1 : -1);
        }

        #endregion

        #region segnalibro

        private void browseBookmarkListToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (browseBookmarkListToolStripComboBox.SelectedIndex == 0)
            {
                browseBookmarkPreviousToolStripButton.Enabled = false;
                browseBookmarkBookmarkToolStripComboBox.Enabled = false;
                browseBookmarkNextToolStripButton.Enabled = false;
            }

            if (nonAggiornareBrowseBookmarkBookmark)
                return;
            browseBookmarkBookmarkToolStripComboBox.Items.Clear();
            for (int i = 4; i < bookmarksToolStripMenuItem.DropDownItems.Count - 1; ++i)
            {
                if (bookmarksToolStripMenuItem.DropDownItems[i].Text == browseBookmarkListToolStripComboBox.SelectedItem.ToString())
                    AggiungiSegnalibriAVisualizza((ToolStripMenuItem)(bookmarksToolStripMenuItem.DropDownItems[i]));
                if (browseBookmarkBookmarkToolStripComboBox.Items.Count > 0)
                {
                    browseBookmarkBookmarkToolStripComboBox.Enabled = true;
                    browseBookmarkBookmarkToolStripComboBox.SelectedIndex = 0;
                }
                else
                    browseBookmarkBookmarkToolStripComboBox.Enabled = false;
            }
        }

        private void AggiungiSegnalibriAVisualizza(ToolStripMenuItem menuVoce)
        {
            foreach (ToolStripItem voce in menuVoce.DropDownItems)
            {
                if (voce.Tag != null)
                {
                    string[] riferimento = voce.Tag.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (riferimento.Length >= 3)
                        browseBookmarkBookmarkToolStripComboBox.Items.Add(testi.NormalizzaRiferimento(riferimento[0], riferimento[1], riferimento[2]));
                }
                AggiungiSegnalibriAVisualizza((ToolStripMenuItem)voce);
            }
        }

        private void browseBookmarkPreviousToolStripButton_Click(object sender, EventArgs e)
        {
            if (browseBookmarkBookmarkToolStripComboBox.SelectedIndex > 0)
                browseBookmarkBookmarkToolStripComboBox.SelectedIndex--;
        }

        private void browseBookmarkBookmarkToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null && ActiveMdiChild.Tag != null && ActiveMdiChild.Tag.ToString() == "Visualizza")
            {
                Riferimento riferimento = testi.ConvertiRiferimento(browseBookmarkBookmarkToolStripComboBox.SelectedItem.ToString());
                riferimento.DaTradurre = true;
                ((Visualizza)ActiveMdiChild).SpostaTesto(riferimento, true);
            }
            browseBookmarkPreviousToolStripButton.Enabled = (browseBookmarkBookmarkToolStripComboBox.SelectedIndex > 0);
            browseBookmarkNextToolStripButton.Enabled = (browseBookmarkBookmarkToolStripComboBox.SelectedIndex >= 0 && browseBookmarkBookmarkToolStripComboBox.SelectedIndex < browseBookmarkBookmarkToolStripComboBox.Items.Count - 1);
        }

        private void browseBookmarkNextToolStripButton_Click(object sender, EventArgs e)
        {
            if (browseBookmarkBookmarkToolStripComboBox.SelectedIndex >= 0 && browseBookmarkBookmarkToolStripComboBox.SelectedIndex < browseBookmarkBookmarkToolStripComboBox.Items.Count - 1)
                browseBookmarkBookmarkToolStripComboBox.SelectedIndex++;
        }

        #endregion

        #region ricerca

        private void browseSearchToolStripComboBox_TextChanged(object sender, EventArgs e)
        {
            browseSearchGotoToolStripButton.Enabled = (browseSearchToolStripComboBox.Text.Length > 0);
        }

        private void browseSearchToolStripComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                browseSearchGotoToolStripButton_Click(sender, new EventArgs());
        }

        private void browseSearchGotoToolStripButton_Click(object sender, EventArgs e)
        {
            if (((Visualizza)ActiveMdiChild) != null)
            {
                string espressione = browseSearchToolStripComboBox.Text;
                RicercaInVisualizza(espressione, ((Visualizza)ActiveMdiChild).paneAttivo.Versione);

                if (browseSearchToolStripComboBox.Items.IndexOf(espressione) > -1)
                    browseSearchToolStripComboBox.Items.Remove(espressione);
                browseSearchToolStripComboBox.Items.Insert(0, espressione);
                //                browseSearchToolStripComboBox.Text = "";
                browseSearchToolStripComboBox.AutoCompleteMode = AutoCompleteMode.None;
                // per costringere il combo box a rileggere i valori che possono essere usati per l'autocomplete
                browseSearchToolStripComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                browseSearchToolStripComboBox.Text = espressione;
                browseSearchToolStripComboBox.SelectAll();
            }
        }

        public void RicercaInVisualizza(string espressioneDaRicercare, string versione)
        {
            if (string.IsNullOrEmpty(versione))
                return; // per esempio "Tutte le Bibbie"
            try
            {
                Riferimento versettiTrovati = testi.Ricerca(espressioneDaRicercare, versione);
                ((Visualizza)ActiveMdiChild).paneAttivo.ParoleRicercate = versettiTrovati;
                browseSearchVersesToolStripComboBox.Items.Clear();
                if (versettiTrovati.Count > 0)
                {
                    foreach (byte[] brano in versettiTrovati.Brani)
                        browseSearchVersesToolStripComboBox.Items.Add(testi.NormalizzaRiferimento(brano[0], brano[1], brano[2]));
                    foreach (string nota in versettiTrovati.Note)
                        browseSearchVersesToolStripComboBox.Items.Add(nota.StartsWith("#", StringComparison.Ordinal) ? testi.ConvertiTitoloNotaARiferimento(nota) : nota);
                    browseSearchVersesToolStripComboBox.Enabled = (browseSearchVersesToolStripComboBox.Items.Count > 0);
                    if (browseSearchVersesToolStripComboBox.Enabled)
                        browseSearchVersesToolStripComboBox.SelectedIndex = 0;
                }
                else
                    browseSearchVersesToolStripComboBox.Enabled = false;
            }
            catch
            {
                // se c'è qualcosa di sbagliato nell'espressione da ricercare
            }
        }

        private void browseSearchVersesToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((Visualizza)ActiveMdiChild) != null)
            {
                if (((Visualizza)ActiveMdiChild).paneAttivo.TipoTesto == TestoTipi.Dizionario)
                {
                    ((Visualizza)ActiveMdiChild).SpostaTesto(browseSearchVersesToolStripComboBox.SelectedItem.ToString(), true);
                }
                else
                {
                    Riferimento rif = testi.ConvertiRiferimento(browseSearchVersesToolStripComboBox.SelectedItem.ToString());
                    if (rif.Count > 0)
                        ((Visualizza)ActiveMdiChild).SpostaTesto(rif, true);
                }
            }
        }

        #endregion

        #region cronologia

        private void browseBackToolStripButton_Click(object sender, EventArgs e)
        {
            if (((Visualizza)ActiveMdiChild) != null && numeroInCronologia > 0)
            {
                numeroInCronologia -= 1;
                aggiornaCronologia = false;
                ((Visualizza)ActiveMdiChild).SpostaTesto(cronologia[numeroInCronologia], true);
                aggiornaCronologia = true;
            }
        }

        private void browseForwardToolStripButton_Click(object sender, EventArgs e)
        {
            if (((Visualizza)ActiveMdiChild) != null && numeroInCronologia < cronologia.Count - 1)
            {
                numeroInCronologia += 1;
                aggiornaCronologia = false;
                ((Visualizza)ActiveMdiChild).SpostaTesto(cronologia[numeroInCronologia], true);
                aggiornaCronologia = true;
            }
        }

        #endregion

        #endregion

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((Editor)ActiveMdiChild != null)
            {
                using (TrovaSostituisci trovaSostituisciForm = new TrovaSostituisci(this, ((ToolStripItem)sender).Tag.ToString() == "Find"))
                {
                    trovaSostituisciForm.TestoTrova = trovaTesto;
                    trovaSostituisciForm.TestoSostituisci = sostituisciTesto;
                    trovaSostituisciForm.Opzioni = trovaOpzioni;
                    trovaSostituisciForm.ShowDialog();
                    if (trovaSostituisciForm.DialogResult == DialogResult.OK)
                    {
                        RichTextBoxEx rtb = ((Editor)ActiveMdiChild).rtEditor;
                        trovaTesto = trovaSostituisciForm.TestoTrova;
                        sostituisciTesto = trovaSostituisciForm.TestoSostituisci;
                        trovaOpzioni = trovaSostituisciForm.Opzioni;
                        int posizioneTestoTrovato;
                        switch (trovaSostituisciForm.PulsanteCliccato)
                        {
                            case 1: // trova
                                TrovaInRichText(rtb, trovaTesto, trovaOpzioni);
                                break;
                            case 2: // sostituisci
                                SostituisciInRichText(rtb, trovaTesto, sostituisciTesto, trovaOpzioni);
                                break;
                            case 3: // sostituisci tutto
                                posizioneTestoTrovato = rtb.Find(trovaTesto, 0, trovaSostituisciForm.Opzioni);
                                while (posizioneTestoTrovato >= 0)
                                {
                                    rtb.SelectedText = sostituisciTesto;
                                    posizioneTestoTrovato = rtb.Find(trovaTesto, 0, trovaSostituisciForm.Opzioni);
                                }
                                break;
                        }
                    }
                }
            }
        }

        #region CollegamentiEsterni

        private void externalLinkStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            string linguaFinestra = "";
            bool riferimentoTrovato = false;
            bool finestraConRichTextBoxAttiva = (ActiveMdiChild != null && ActiveMdiChild.Tag != null && (ActiveMdiChild.Tag.ToString() == "Visualizza" || ActiveMdiChild.Tag.ToString() == "Editor" || ActiveMdiChild.Tag.ToString() == "BraniParalleli" || ActiveMdiChild.Tag.ToString() == "Lettura"));
            if (finestraConRichTextBoxAttiva)
            {
                RichTextBoxEx richText = null;
                if (ActiveMdiChild.Tag.ToString() == "Editor")
                    richText = ((Editor)ActiveMdiChild).rtEditor;
                else if (ActiveMdiChild.Tag.ToString() == "Visualizza")
                    richText = ((Visualizza)ActiveMdiChild).RtfAttiva;
                else if (ActiveMdiChild.Tag.ToString() == "BraniParalleli" || ActiveMdiChild.Tag.ToString() == "Lettura")
                {
                    try
                    {
                        richText = (RichTextBoxEx)(ActiveMdiChild.ActiveControl);
                    }
                    catch { }
                }
                if (richText != null)
                {
                    linguaFinestra = richText.Lingua;
                    string selezione = richText.SelectedText;
                    if (!string.IsNullOrEmpty(selezione))
                        if (char.IsDigit(selezione[selezione.Length - 1]) && (testi.ConvertiRiferimento(selezione).Count > 0))
                            riferimentoTrovato = true;
                    if (!riferimentoTrovato)
                        riferimentoTrovato = !string.IsNullOrEmpty(richText.VersettoAttuale(richText.SelectionStart));
                    if (!riferimentoTrovato && ActiveMdiChild.Tag.ToString() == "Editor")
                        riferimentoTrovato = !string.IsNullOrEmpty(((Editor)(ActiveMdiChild)).RiferimentoDaNomeNota());
                }
            }
            int numeroCollegamenti = externalLinkStripMenuItem.DropDownItems.Count - 2;
            string linguaVoce;
            bool sottovoceVisibile, visibile = true;
            ToolStripMenuItem voceMenu;
            for (int i = 0; i < numeroCollegamenti; ++i)
            {
                voceMenu = (ToolStripMenuItem)(externalLinkStripMenuItem.DropDownItems[i]);
                voceMenu.Enabled = finestraConRichTextBoxAttiva;
                if (voceMenu.DropDownItems.Count == 0)
                { // è una voce normale, non una categoria
                    if (finestraConRichTextBoxAttiva)
                    {
                        if (((InfoCollegamento)(voceMenu.Tag)).tipo == CollegamentoTipo.Parola)
                        {
                            linguaVoce = ((InfoCollegamento)(voceMenu.Tag)).lingua;
                            visibile = (string.IsNullOrEmpty(linguaVoce) || string.IsNullOrEmpty(linguaFinestra) || linguaFinestra == linguaVoce);
                        }
                        else
                            visibile = riferimentoTrovato;
                    }
                    externalLinkStripMenuItem.DropDownItems[i].Visible = visibile;
                }
                else
                {
                    sottovoceVisibile = false;
                    foreach (ToolStripItem voceInCategoria in voceMenu.DropDownItems)
                    {
                        if (finestraConRichTextBoxAttiva)
                        {
                            if (((InfoCollegamento)(voceInCategoria.Tag)).tipo == CollegamentoTipo.Parola)
                            {
                                linguaVoce = ((InfoCollegamento)(voceInCategoria.Tag)).lingua;
                                visibile = (string.IsNullOrEmpty(linguaVoce) || string.IsNullOrEmpty(linguaFinestra) || linguaFinestra == linguaVoce);
                            }
                            else
                                visibile = riferimentoTrovato;
                        }
                        voceInCategoria.Enabled = finestraConRichTextBoxAttiva;
                        voceInCategoria.Visible = visibile;
                        if (visibile)
                            sottovoceVisibile = true;
                    }
                    voceMenu.Visible = sottovoceVisibile;
                }
            }
        }

        #endregion

        public void AggiornaPulsanti(RichTextBoxEx rt)
        {
            if (rt == null)
            {
                formatToolStrip.Enabled = false;
                fontSizeToolStripComboBox.Text = "";
                aggiornaFont = false;
                fontToolStripComboBox.SelectedIndex = -1;
                aggiornaFont = true;
                copyToolStripButton.Enabled = false;

                zoomToolStripMenuItem.Enabled = false;
                zoomToolStripMenuItem.DropDown.Enabled = false;
                cutToolStripMenuItem.Enabled = false;
                copyToolStripMenuItem.Enabled = false;
                deleteToolStripMenuItem.Enabled = false;
                undoToolStripMenuItem.Enabled = false;
                redoToolStripMenuItem.Enabled = false;
                highlightToolStripMenuItem.Enabled = false;
                highlightFormatToolStripSplitButton.Enabled = false;
                hypertextToolStripMenuItem.Enabled = false;
                printToolStripMenuItem.Enabled = false;

                ImpostaBarraDiStato("");
            }
            else
            {
                formatToolStrip.Enabled = true;

                bool testoSelezionato = (!String.IsNullOrEmpty(rt.SelectedText));

                undoToolStripMenuItem.Enabled = rt.CanUndo && !rt.ReadOnly;
                redoToolStripMenuItem.Enabled = rt.CanRedo && !rt.ReadOnly;
                string azione = rt.UndoActionName;
                if (String.IsNullOrEmpty(azione) || azione == "Unknown" || azione == "Sconosciuto" || azione == "Desconocido")
                    // "it" - questo commento è solo affinché la riga sia trovata se c'è un'altra lingua aggiunta, e facciamo una ricerca per la lingua it per trovare dove cambiare il programma
                    azione = "";
                else
                    azione = " " + azione;
                undoToolStripMenuItem.Text = undoCaption + azione;
                azione = isRunningOnMono ? "" : rt.RedoActionName;
                if (String.IsNullOrEmpty(azione) || azione == "Unknown" || azione == "Sconosciuto" || azione == "Desconocido")
                    azione = "";
                else
                    azione = " " + azione;
                redoToolStripMenuItem.Text = redoCaption + azione;

                cutToolStripMenuItem.Enabled = testoSelezionato && !rt.ReadOnly;
                copyToolStripMenuItem.Enabled = testoSelezionato;
                deleteToolStripMenuItem.Enabled = testoSelezionato && !rt.ReadOnly;

                undoToolStripButton.Enabled = rt.CanUndo && !rt.ReadOnly;
                undoToolStripButton.Text = (undoToolStripButton.Enabled ? undoToolStripMenuItem.Text : undoCaption);
                redoToolStripButton.Enabled = rt.CanRedo && !rt.ReadOnly;
                redoToolStripButton.Text = (redoToolStripButton.Enabled ? redoToolStripMenuItem.Text : redoCaption);

                cutToolStripButton.Enabled = testoSelezionato && !rt.ReadOnly;
                copyToolStripButton.Enabled = testoSelezionato;

                zoomToolStripMenuItem.Enabled = true;
                zoomToolStripMenuItem.DropDown.Enabled = true;

                bool finestraVisualizza = rt.Parent.Tag != null && rt.Parent.Tag.ToString().Contains("Visualizza");
                highlightToolStripMenuItem.Enabled = testoSelezionato && finestraVisualizza;
                highlightFormatToolStripSplitButton.Enabled = testoSelezionato && finestraVisualizza;
                hypertextToolStripMenuItem.Enabled = testoSelezionato && !rt.ReadOnly;
                hypertextToolStripSplitButton.Enabled = testoSelezionato && !rt.ReadOnly;

                //                printToolStripMenuItem.Enabled = testoSelezionato;
                //                printToolStripButton.Enabled = testoSelezionato;

                fontSizeToolStripComboBox.Enabled = !rt.ReadOnly;
                fontToolStripComboBox.Enabled = !rt.ReadOnly;
                boldToolStripButton.Enabled = !rt.ReadOnly;
                italicToolStripButton.Enabled = !rt.ReadOnly;
                underlineToolStripButton.Enabled = !rt.ReadOnly;
                fontToolStripButton.Enabled = !rt.ReadOnly;
                paragraphToolStripButton.Enabled = !rt.ReadOnly;
                indentDecreaseToolStripButton.Enabled = !rt.ReadOnly;
                indentIncreaseToolStripButton.Enabled = !rt.ReadOnly;
                hypertextToolStripSplitButton.Enabled = !rt.ReadOnly;

                if (rt.SelectionFont == null)
                {
                    // se la selezione contiene due font diversi, SelectionFont è null; possiamo solo assegnare false anche se tutto il testo è grassetto
                    fontSizeToolStripComboBox.Text = "";
                    aggiornaFont = false;
                    fontToolStripComboBox.SelectedIndex = -1;
                    aggiornaFont = true;

                    boldToolStripButton.Checked = false;
                    italicToolStripButton.Checked = false;
                    underlineToolStripButton.Checked = false;
                }
                else
                {
                    fontSizeToolStripComboBox.Text = rt.SelectionFont.SizeInPoints.ToString(CultureInfo.CurrentCulture);
                    aggiornaFont = false;
                    fontToolStripComboBox.SelectedIndex = fontToolStripComboBox.Items.IndexOf(rt.SelectionFont.Name);
                    aggiornaFont = true;

                    boldToolStripButton.Checked = rt.SelectionFont.Bold;
                    italicToolStripButton.Checked = rt.SelectionFont.Italic;
                    underlineToolStripButton.Checked = rt.SelectionFont.Underline;
                }

                alignLeftToolStripButton.Enabled = !rt.ReadOnly;
                alignCenterToolStripButton.Enabled = !rt.ReadOnly;
                alignRightToolStripButton.Enabled = !rt.ReadOnly;
                alignJustifyToolStripButton.Enabled = !rt.ReadOnly;
                try
                {
                    switch (rt.SelectionAlignment)
                    {
                        case RichTextBoxEx.TextAlign.Left:
                            alignLeftToolStripButton.Checked = true;
                            alignCenterToolStripButton.Checked = false;
                            alignRightToolStripButton.Checked = false;
                            alignJustifyToolStripButton.Checked = false;
                            break;
                        case RichTextBoxEx.TextAlign.Center:
                            alignLeftToolStripButton.Checked = false;
                            alignCenterToolStripButton.Checked = true;
                            alignRightToolStripButton.Checked = false;
                            alignJustifyToolStripButton.Checked = false;
                            break;
                        case RichTextBoxEx.TextAlign.Right:
                            alignLeftToolStripButton.Checked = false;
                            alignCenterToolStripButton.Checked = false;
                            alignRightToolStripButton.Checked = true;
                            alignJustifyToolStripButton.Checked = false;
                            break;
                        case RichTextBoxEx.TextAlign.Justify:
                            alignLeftToolStripButton.Checked = false;
                            alignCenterToolStripButton.Checked = false;
                            alignRightToolStripButton.Checked = false;
                            alignJustifyToolStripButton.Checked = true;
                            break;
                    }
                }
                // Mono 1.2.5 dà exception qui quando si apre un file RTF
                catch (NullReferenceException) { }

                if (!isRunningOnMono)
                {
                    bulletsToolStripButton.Enabled = !rt.ReadOnly;
                    bulletsToolStripButton.Checked = rt.SelectionBullet;
                }

                statusEditor.Text = String.Format(CultureInfo.CurrentCulture, Principale.LocRM.GetString("EditorPosition"), rt.GetLineFromCharIndex(rt.SelectionStart) + 1, rt.SelectionStart - rt.GetFirstCharIndexOfCurrentLine() + 1);
            }
        }

        #region Zoom

        private void zoomToolStripComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ActiveMdiChild.Tag != null)
            {
                string zoom = zoomToolStripComboBox.Text;
                if (zoom.EndsWith("%", StringComparison.Ordinal))
                    zoom = zoom.Substring(0, zoom.Length - 1);
                Single zoomFactor = Convert.ToSingle(zoom, CultureInfo.InvariantCulture) / 100.0F;
                if (zoomFactor < 64.0 && zoomFactor > 1.0 / 64.0)
                {
                    try
                    {
                        switch (ActiveMdiChild.Tag.ToString())
                        {
                            case "Editor":
                                ((Editor)ActiveMdiChild).rtEditor.ZoomFactor = zoomFactor;
                                break;
                            case "Lettura":
                                ((Lettura)ActiveMdiChild).UltimaRtb.ZoomFactor = zoomFactor;
                                break;
                            case "BraniParalleli":
                                ((BraniParalleli)ActiveMdiChild).UltimaRtb.ZoomFactor = zoomFactor;
                                break;
                        }
                    }
                    catch // non numerico zoom
                    {
                    }
                }
            }
        }

        private void zoomToolStripComboBox_Leave(object sender, EventArgs e)
        {
            zoomToolStripComboBox_SelectedIndexChanged(sender, e);
        }

        private void zoomToolStripComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                zoomToolStripComboBox_SelectedIndexChanged(sender, e);
        }

        private void zoom100ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null)
            {
                string zoom = ((ToolStripMenuItem)sender).Text;
                if (zoom.IndexOf('&') > -1)
                    zoom = zoom.Remove(zoom.IndexOf('&'), 1);

                if (ActiveMdiChild.Tag != null)
                {
                    string zoom2 = zoom;
                    if (zoom.EndsWith("%", StringComparison.Ordinal))
                        zoom2 = zoom.Substring(0, zoom.Length - 1);
                    Single zoom3 = Convert.ToSingle(zoom2, CultureInfo.InvariantCulture) / 100.0F;
                    switch (ActiveMdiChild.Tag.ToString())
                    {
                        case "Editor":
                            zoomToolStripComboBox.SelectedIndex = zoomToolStripComboBox.Items.IndexOf(zoom);
                            ((Editor)ActiveMdiChild).rtEditor.ZoomFactor = zoom3;
                            break;
                        case "Visualizza":
                            browseZoomToolStripComboBox.SelectedIndex = browseZoomToolStripComboBox.Items.IndexOf(zoom);
                            ((Visualizza)ActiveMdiChild).paneAttivo.Zoom = zoom3;
                            break;
                        case "Lettura":
                            zoomToolStripComboBox.SelectedIndex = zoomToolStripComboBox.Items.IndexOf(zoom);
                            ((Lettura)ActiveMdiChild).UltimaRtb.ZoomFactor = zoom3;
                            break;
                        case "BraniParalleli":
                            zoomToolStripComboBox.SelectedIndex = zoomToolStripComboBox.Items.IndexOf(zoom);
                            ((BraniParalleli)ActiveMdiChild).UltimaRtb.ZoomFactor = zoom3;
                            break;
                    }
                }
            }
        }

        internal void VisualizzaZoom()
        {
            // il MouseWheel evento è segnalato prima che lo zoom cambi,
            // quindi dobbiamo aspettare un po' di tempo prima di aggiornare i combobox
            timerZoom.Enabled = true;
        }

        private void timerZoom_Tick(object sender, EventArgs e)
        {
            timerZoom.Enabled = false;
            if (ActiveMdiChild.Tag != null)
            {
                if (ActiveMdiChild.Tag.ToString() == "Editor")
                    zoomToolStripComboBox.Text = Convert.ToInt32(((Editor)(ActiveMdiChild)).rtEditor.ZoomFactor * 100.0F).ToString(CultureInfo.CurrentCulture) + "%";
                else if (ActiveMdiChild.Tag.ToString() == "Visualizza")
                    browseZoomToolStripComboBox.Text = Convert.ToInt32(((Visualizza)(ActiveMdiChild)).paneAttivo.Zoom * 100.0F).ToString(CultureInfo.CurrentCulture) + "%";
            }
            Application.DoEvents();
        }

        #endregion

        #region Barra di ordine

        private void orderPreviousToolStripButton_Click(object sender, EventArgs e)
        {
            string tag = "";
            if (ActiveMdiChild != null && ActiveMdiChild.Tag != null)
                tag = ActiveMdiChild.Tag.ToString();
            if (tag == "Editor")
                ApriNotaInEditor(((Editor)ActiveMdiChild).NotaPrecedente, ((Editor)ActiveMdiChild).rtEditor.Versione);
            else if (tag == "Visualizza")
            {
                string[] notePrecedenteSuccessiva = testi.NotePrecedenteSuccessiva(((Visualizza)ActiveMdiChild).paneAttivo.Versione, ((Visualizza)ActiveMdiChild).paneAttivo.Voce);
                if (!string.IsNullOrEmpty(notePrecedenteSuccessiva[0]))
                    ((Visualizza)ActiveMdiChild).paneAttivo.SpostaTesto(notePrecedenteSuccessiva[0], true);
            }
        }

        private void orderIndexToolStripButton_Click(object sender, EventArgs e)
        {
            string tag = "";
            if (ActiveMdiChild != null && ActiveMdiChild.Tag != null)
                tag = ActiveMdiChild.Tag.ToString();
            if (tag == "Editor")
                ApriNotaInEditor(((Editor)ActiveMdiChild).NotaIndice, ((Editor)ActiveMdiChild).rtEditor.Versione);
            else if (tag == "Visualizza")
            {
                Collection<string> ordine = testi.GetNoteInOrdine(((Visualizza)ActiveMdiChild).paneAttivo.Versione);
                if (ordine.Count > 0 && !string.IsNullOrEmpty(ordine[0]))
                    ((Visualizza)ActiveMdiChild).paneAttivo.SpostaTesto(ordine[0], true);
            }
        }

        private void orderNextToolStripButton_Click(object sender, EventArgs e)
        {
            string tag = "";
            if (ActiveMdiChild != null && ActiveMdiChild.Tag != null)
                tag = ActiveMdiChild.Tag.ToString();
            if (tag == "Editor")
                ApriNotaInEditor(((Editor)ActiveMdiChild).NotaProssima, ((Editor)ActiveMdiChild).rtEditor.Versione);
            else if (tag == "Visualizza")
            {
                string[] notePrecedenteSuccessiva = testi.NotePrecedenteSuccessiva(((Visualizza)ActiveMdiChild).paneAttivo.Versione, ((Visualizza)ActiveMdiChild).paneAttivo.Voce);
                if (!string.IsNullOrEmpty(notePrecedenteSuccessiva[1]))
                    ((Visualizza)ActiveMdiChild).paneAttivo.SpostaTesto(notePrecedenteSuccessiva[1], true);
            }
        }

        #endregion

        #region BarraDiStato

        public void ImpostaBarraDiStato(string riferimento)
        {
            statusEditor.Text = riferimento;
        }

        public BarraConEtichetta CreaBarraDiStato(string messaggio, int minimo, int massimo)
        {
            BarraConEtichetta barraConEtichetta = new BarraConEtichetta(messaggio, minimo, massimo, statusMessagge);
            barraConEtichetta.MettiInStatusStrip(statusStrip);
            Application.DoEvents();
            return barraConEtichetta;
        }

        public void SetBarraDiStatoTesto(string messaggio)
        {
            statusMessagge.Text = messaggio;
        }

        #endregion

        #region RigaDiComando

        private void comandoToolStripButton_Click(object sender, EventArgs e)
        {
            string comando = comandoToolStripComboBox.Text;
            EseguiComando(comando);

            if (comandoToolStripComboBox.Items.IndexOf(comando) > -1)
                comandoToolStripComboBox.Items.Remove(comando);
            comandoToolStripComboBox.Items.Insert(0, comando);
            //            comandoToolStripComboBox.Text = "";
            comandoToolStripComboBox.AutoCompleteMode = AutoCompleteMode.None;
            // per costringere il combo box a rileggere i valori che possono essere usati per l'autocomplete
            comandoToolStripComboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comandoToolStripComboBox.Text = comando;
            comandoToolStripComboBox.SelectAll();
        }

        /// <summary>
        /// Esegue un comando dalla riga di comando.
        /// </summary>
        /// <param name="comandoRiga">Il comando da eseguire.</param>
        public void EseguiComando(string comandoRiga)
        {
            EseguiComando(comandoRiga, false);
        }

        /// <summary>
        /// Esegue un comando dalla riga di comando.
        /// </summary>
        /// <param name="comandoRiga">Il comando da eseguire.</param>
        /// <param name="comandoSingolo">Falso se ci sono diversi comandi separati da un punto e virgola.</param>
        public void EseguiComando(string comandoRiga, bool comandoSingolo)
        {
            string[] comandi = new string[] { comandoRiga };
            if (!comandoSingolo)
                comandi = comandoRiga.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string comando in comandi)
            {
                List<string> comandoParole = new List<string>(comando.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                // testo fra virgolette considerato come una parola
                bool inVirgolette = false;
                for (int i = comandoParole.Count - 1; i >= 0; --i)
                {
                    if (inVirgolette)
                    {
                        comandoParole[i] += " " + comandoParole[i + 1];
                        comandoParole.RemoveAt(i + 1);
                    }
                    if (comandoParole[i].EndsWith("\"", StringComparison.Ordinal))
                    {
                        inVirgolette = true;
                        comandoParole[i] = comandoParole[i].Remove(comandoParole[i].Length - 1);
                    }
                    if (comandoParole[i].StartsWith("\"", StringComparison.Ordinal))
                    {
                        inVirgolette = false;
                        comandoParole[i] = comandoParole[i].Remove(0, 1);
                    }
                }

                bool eseguito = false;
                try
                {
                    switch (comandoParole[0].ToUpperInvariant())
                    {
                        case "SEARCH":
                        case "RICERCA":
                        case "BUSCAR":
                            string versioneDaRicercare = testi.UltimaBibbia, branoDaRicercare = "";
                            if (comandoParole.Count > 3)
                            {
                                string parola2 = comandoParole[2].ToUpperInvariant();
                                if (parola2 == "DA" || parola2 == "FROM" || parola2 == "DE")
                                    versioneDaRicercare = comandoParole[3];
                                if (parola2 == "IN" || parola2 == "EN")
                                    branoDaRicercare = comandoParole[3];
                                if (comandoParole.Count > 5)
                                {
                                    string parola4 = comandoParole[4].ToUpperInvariant();
                                    if (parola4 == "DA" || parola4 == "FROM" || parola2 == "DE")
                                        versioneDaRicercare = comandoParole[5];
                                    if (parola4 == "IN" || parola2 == "EN")
                                        branoDaRicercare = comandoParole[5];
                                }
                            }
                            string versioneDaRicercare2 = versioneDaRicercare;
                            if (!testi.VersioneEsiste(versioneDaRicercare))
                                versioneDaRicercare2 = testi.VersioneDaAbbreviazione(versioneDaRicercare);
                            if (!testi.VersioneEsiste(versioneDaRicercare2))
                                MessageBox.Show(string.Format(CultureInfo.CurrentCulture, LocRM.GetString("CommandlineUnknownText"), versioneDaRicercare) + "\n" + LocRM.GetString("CommandlineGetHelp"), LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, messageBoxOptions);
                            else
                                RicercaInEditor(comandoParole[1], branoDaRicercare, versioneDaRicercare2);
                            break;
                        case "MOSTRA":
                        case "SHOW":
                        case "MOSTRAR":
                        case "COPIA":
                        case "COPY":
                        case "COPIAR":
                            string versioneDaMostrare = testi.UltimaBibbia;
                            if (comandoParole.Count > 3)
                            {
                                string parola2 = comandoParole[2].ToUpperInvariant();
                                if (parola2 == "DA" || parola2 == "FROM" || parola2 == "DE")
                                    versioneDaMostrare = comandoParole[3];
                            }
                            if (!comandoParole[0].ToUpperInvariant().StartsWith("C", StringComparison.Ordinal))
                            {
                                if (versioneDaMostrare.ToUpperInvariant() == "BIBBIE" || versioneDaMostrare.ToUpperInvariant() == "BIBLES" || versioneDaMostrare.ToUpperInvariant() == "BIBLIAS")
                                {
                                    MostraBranoInEditor(comandoParole[1], testi.NomiVersioni(TestoTipi.Bibbia));
                                    break;
                                }
                                else if (versioneDaMostrare.ToUpperInvariant() == "COMMENTARI" || versioneDaMostrare.ToUpperInvariant() == "COMMENTARIES" || versioneDaMostrare.ToUpperInvariant() == "COMENTARIOS")
                                {
                                    MostraBranoInEditor(comandoParole[1], testi.NomiVersioni(TestoTipi.Commentario));
                                    break;
                                }
                            }
                            string versioneDaMostrare2 = versioneDaMostrare;
                            if (!testi.VersioneEsiste(versioneDaMostrare))
                                versioneDaMostrare2 = testi.VersioneDaAbbreviazione(versioneDaMostrare);
                            if (!testi.VersioneEsiste(versioneDaMostrare2))
                                MessageBox.Show(string.Format(CultureInfo.CurrentCulture, LocRM.GetString("CommandlineUnknownText"), versioneDaMostrare) + "\n" + LocRM.GetString("CommandlineGetHelp"), LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, messageBoxOptions);
                            else
                            {
                                if (!comandoParole[0].ToUpperInvariant().StartsWith("C", StringComparison.Ordinal))
                                    MostraBranoInEditor(comandoParole[1], versioneDaMostrare2);
                                else
                                    testi.PassageInClipboard(comandoParole[1], versioneDaMostrare2);
                            }
                            break;
                        case "SIMILI":
                        case "SIMILAR":
                        case "DEFINIZIONI":
                        case "DEFINITIONS":
                        case "DEFINICIONES":
                        case "CHIAVE":
                        case "CONCORDANCE":
                        case "CONCORDANCIA":
                            string versioneDaUsare = testi.UltimaBibbia;
                            if (comandoParole.Count > 3)
                            {
                                string parola2 = comandoParole[2].ToUpperInvariant();
                                if (parola2 == "DA" || parola2 == "FROM" || parola2 == "DE")
                                    versioneDaUsare = comandoParole[3];
                            }
                            string versioneDaUsare2 = versioneDaUsare;
                            if (!testi.VersioneEsiste(versioneDaUsare))
                                versioneDaUsare2 = testi.VersioneDaAbbreviazione(versioneDaUsare);
                            if (!testi.VersioneEsiste(versioneDaUsare2))
                                MessageBox.Show(string.Format(CultureInfo.CurrentCulture, LocRM.GetString("CommandlineUnknownText"), versioneDaUsare) + "\n" + LocRM.GetString("CommandlineGetHelp"), LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, messageBoxOptions);
                            else
                            {
                                if (comandoParole[0].ToUpperInvariant().StartsWith("S", StringComparison.Ordinal))
                                    TrovaBraniSimili(comandoParole[1], versioneDaUsare2);
                                else if (comandoParole[0].ToUpperInvariant().StartsWith("C", StringComparison.Ordinal))
                                    ChiaveInEditor(comandoParole[1], versioneDaUsare2,
                                        Settings.Default.ChiaveParoleRadici != 1, Settings.Default.ChiaveNonRadiciComuni, Settings.Default.ChiaveRadiciComuni.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries), Settings.Default.ChiaveOrdine, Settings.Default.ChiaveNumeroMinimo, Settings.Default.ChiaveConRiferimenti, Settings.Default.ChiaveDefinizioni ? Funzioni.DizionarioDiVersione(versioneDaUsare2) : "");
                                else
                                    MostraDefinizioniInEditor(comandoParole[1], versioneDaUsare2);
                            }
                            break;
                        case "NOTA":
                        case "NOTE":
                        case "COPIANOTA":
                        case "COPYNOTE":
                        case "COPIARNOTA":
                            string collezioneDaUsare = "";
                            if (comandoParole.Count > 3)
                            {
                                string parola2 = comandoParole[2].ToUpperInvariant();
                                if (parola2 == "DA" || parola2 == "FROM" || parola2 == "DE")
                                    collezioneDaUsare = comandoParole[3];
                            }
                            string collezioneDaUsare2 = collezioneDaUsare;
                            if (!testi.VersioneEsiste(collezioneDaUsare))
                                collezioneDaUsare2 = testi.VersioneDaAbbreviazione(collezioneDaUsare);
                            if (!testi.VersioneEsiste(collezioneDaUsare2))
                                MessageBox.Show(string.Format(CultureInfo.CurrentCulture, LocRM.GetString("CommandlineUnknownText"), collezioneDaUsare) + "\n" + LocRM.GetString("CommandlineGetHelp"), LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, messageBoxOptions);
                            else
                            {
                                if (comandoParole[0].ToUpperInvariant().StartsWith("C", StringComparison.Ordinal))
                                    testi.NoteTitleInClipboard(comandoParole[1], collezioneDaUsare2);
                                else
                                    ApriNotaInEditor(comandoParole[1], collezioneDaUsare2);
                            }
                            break;
                        case "REFERENCES":
                        case "RIFERIMENTI":
                        case "REFERENCIAS":
                            string collezioneDaCercare = "";
                            if (comandoParole.Count > 3)
                            {
                                string parola2 = comandoParole[2].ToUpperInvariant();
                                if (parola2 == "DA" || parola2 == "FROM" || parola2 == "DE")
                                    collezioneDaCercare = comandoParole[3];
                            }
                            string collezioneDaCercare2 = collezioneDaCercare;
                            if (!testi.VersioneEsiste(collezioneDaCercare))
                                collezioneDaCercare2 = testi.VersioneDaAbbreviazione(collezioneDaCercare);
                            if (!testi.VersioneEsiste(collezioneDaCercare2))
                                MessageBox.Show(string.Format(CultureInfo.CurrentCulture, LocRM.GetString("CommandlineUnknownText"), collezioneDaCercare) + "\n" + LocRM.GetString("CommandlineGetHelp"), LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, messageBoxOptions);
                            else
                                MostraBranoInEditor(testi.Citazioni(comandoParole[1], collezioneDaCercare2), collezioneDaCercare2);
                            break;
                        case "VIEW":
                        case "VISUALIZZA":
                        case "VER":
                        case "GOTO":
                        case "VAI":
                        case "IRA":
                            string versioneDaVisualizzare = testi.UltimaBibbia;
                            string riferimento = "";
                            if (comandoParole.Count > 3)
                            {
                                string parola2 = comandoParole[2].ToUpperInvariant();
                                if (parola2 == "DA" || parola2 == "FROM" || parola2 == "DE")
                                    versioneDaVisualizzare = comandoParole[3];
                                riferimento = comandoParole[1];
                            }
                            else if (comandoParole.Count > 2)
                            {
                                string parola1 = comandoParole[1].ToUpperInvariant();
                                if (parola1 == "DA" || parola1 == "FROM" || parola1 == "DE")
                                    versioneDaVisualizzare = comandoParole[2];
                            }
                            else if (comandoParole.Count == 2)
                                riferimento = comandoParole[1];

                            if (comandoParole[0].ToUpperInvariant() == "GOTO" || comandoParole[0].ToUpperInvariant() == "VAI" || comandoParole[0].ToUpperInvariant() == "IRA")
                            {
                                if (ActiveMdiChild != null && ActiveMdiChild.Tag != null && ActiveMdiChild.Tag.ToString() == "Visualizza" && !string.IsNullOrEmpty(riferimento))
                                {
                                    ((Visualizza)ActiveMdiChild).SpostaTesto(testi.ConvertiRiferimento(riferimento), true);
                                    break;
                                }
                            }

                            string versioneDaVisualizzare2 = versioneDaVisualizzare;
                            if (!testi.VersioneEsiste(versioneDaVisualizzare))
                                versioneDaVisualizzare2 = testi.VersioneDaAbbreviazione(versioneDaVisualizzare);
                            if (!testi.VersioneEsiste(versioneDaVisualizzare2))
                            {
                                ToolStripItem menuTrovato = null;
                                foreach (ToolStripItem tsi in parallelTextsStripMenuItem.DropDownItems)
                                {
                                    if (tsi.Text == versioneDaVisualizzare)
                                        menuTrovato = tsi;
                                }
                                if (menuTrovato != null)
                                {
                                    try
                                    {
                                        Visualizza fromVisualizzaParalleli = VisualizzaParalleli(new List<string>(File.ReadAllLines(menuTrovato.Tag.ToString(), System.Text.Encoding.UTF8)), true);
                                        if (!string.IsNullOrEmpty(riferimento))
                                            fromVisualizzaParalleli.SpostaTesto(testi.ConvertiDaStandard(testi.ConvertiRiferimento(riferimento), versioneDaVisualizzare2), true);
                                        return;
                                    }
                                    catch { }
                                }
                            }
                            if (!testi.VersioneEsiste(versioneDaVisualizzare2))
                                MessageBox.Show(string.Format(CultureInfo.CurrentCulture, LocRM.GetString("CommandlineUnknownText"), versioneDaVisualizzare) + "\n" + LocRM.GetString("CommandlineGetHelp"), LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, messageBoxOptions);
                            else
                            {
                                Visualizza formVisualizza = VisualizzaTesto(versioneDaVisualizzare2, TestoTipi.Bibbia);
                                if (!string.IsNullOrEmpty(riferimento))
                                    formVisualizza.SpostaTesto(testi.ConvertiDaStandard(testi.ConvertiRiferimento(riferimento), versioneDaVisualizzare2), false);
                            }
                            break;
                        case "APRI":
                        case "OPEN":
                        case "ABRIR":
                            string estensione = Path.GetExtension(comandoParole[1]).ToUpperInvariant();
                            if (estensione == ".GIF" || estensione == ".JPG" || estensione == ".JPEG" || estensione == ".BMP" || estensione == ".PNG")
                                ApriNomeImmagini(new string[] { comandoParole[1] });
                            else
                                ApriDiversiFile(new string[] { comandoParole[1] });
                            break;
                        case "INFO":
                            ApriInformazione(comandoParole[1]);
                            break;
                        case "LINK":
                        case "VÍNCOLO":
                            Funzioni.ApriBrowser(Uri.EscapeUriString(comandoParole[1]), comandoParole[2], false);
                            break;
                        case "CLOSE":
                        case "CHIUDI":
                        case "CERRAR":
                            if (comandoParole.Count > 1)
                            {
                                if (comandoParole[1].ToUpperInvariant() == "ALL" || comandoParole[1].ToUpperInvariant().Substring(0, 4) == "TUTT" || comandoParole[1].ToUpperInvariant() == "TODO")
                                    for (int i = MdiChildren.Length - 1; i >= 0; --i)
                                        MdiChildren[i].Close();
                            }
                            else
                            {
                                if (MdiChildren.Length > 0)
                                    ActiveMdiChild.Close();
                            }
                            break;
                        case "AGGIORNA":
                        case "UPDATE":
                        case "ACTUALIZAR":
                            if (comandoParole.Count > 1)
                            {
                                string secondaParola = comandoParole[1].ToUpperInvariant();
                                if (secondaParola.StartsWith("E", StringComparison.Ordinal))
                                {
                                    RichiediAggiornamento(1);
                                    eseguito = true;
                                }
                                else if (secondaParola == "ALL" || secondaParola.Substring(0, 4) == "TUTT" || secondaParola == "TODO")
                                {
                                    RichiediAggiornamento(2);
                                    eseguito = true;
                                }
                            }
                            if (!eseguito)
                                RichiediAggiornamento(0);
                            break;
                        case "IMPOSTA":
                        case "SET":
                        case "COLOCAR":
                            string opzione = "", valore = "";
                            if (comandoParole.Count >= 3)
                            {
                                opzione = comandoParole[1].ToUpperInvariant();
                                valore = comandoParole[2];
                            }

                            if (!string.IsNullOrEmpty(opzione) && !string.IsNullOrEmpty(valore))
                            {
                                switch (opzione)
                                {
                                    case "BIBLE":
                                    case "BIBBIA":
                                    case "BIBLIA":
                                        string valore2 = valore;
                                        if (!testi.VersioneEsiste(valore))
                                            valore2 = testi.VersioneDaAbbreviazione(valore);
                                        if (testi.VersioneEsiste(valore2))
                                            testi.UltimaBibbia = valore2;
                                        else
                                            MessageBox.Show(string.Format(CultureInfo.CurrentCulture, LocRM.GetString("CommandlineUnknownText"), valore) + "\n" + LocRM.GetString("CommandlineGetHelp"), LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, messageBoxOptions);
                                        eseguito = true;
                                        break;
                                    case "LINGUA":
                                    case "LANGUAGE":
                                    case "IDIOMA":
                                        valore = valore.ToUpperInvariant().Substring(0, 2);
                                        switch (valore)
                                        {
                                            case "IT":
                                                Settings.Default.InterfacciaLingua = "it-IT";
                                                eseguito = true;
                                                break;
                                            case "EN":
                                                Settings.Default.InterfacciaLingua = "en-GB";
                                                eseguito = true;
                                                break;
                                            case "ES":
                                                Settings.Default.InterfacciaLingua = "es-ES";
                                                eseguito = true;
                                                break;
                                        }
                                        if (eseguito)
                                        {
                                            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(Settings.Default.InterfacciaLingua);
                                            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Settings.Default.InterfacciaLingua);
                                        }

                                        break;
                                }
                            }
                            if (!eseguito)
                                MessageBox.Show(LocRM.GetString("CommandlineSyntaxError") + "\n" + LocRM.GetString("CommandlineGetHelp"), LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, messageBoxOptions);
                            break;
                        case "AIUTO":
                        case "HELP":
                        case "AYUDA":
                        case "?":
                            Editor formEditor = new Editor(this)
                            {
                                MdiParent = this
                            };
                            formEditor.rtEditor.Rtf = testi.RtfIntestazione() + LocRM.GetString("CommandlineHelp") + "}";
                            formEditor.rtEditor.Modified = false;
                            formEditor.Text = comandoParole[0];
                            formEditor.Show();
                            break;
                        default:
                            MessageBox.Show(string.Format(CultureInfo.CurrentCulture, LocRM.GetString("CommandlineUnknownCommand"), comandoParole[0]) + "\n" + LocRM.GetString("CommandlineGetHelp"), LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, messageBoxOptions);
                            break;
                    }
                }
                catch
                {
                    MessageBox.Show(LocRM.GetString("CommandlineSyntaxError") + "\n" + LocRM.GetString("CommandlineGetHelp"), LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, messageBoxOptions);
                }
            }
        }

        private void comandoToolStripComboBox_TextChanged(object sender, EventArgs e)
        {
            comandoToolStripButton.Enabled = (!string.IsNullOrEmpty(comandoToolStripComboBox.Text));
        }

        private void comandoToolStripComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
                comandoToolStripButton_Click(sender, e);
        }

        #endregion

        #region Note

        public void CreaNuovaNota(string testo, string nota, string versione)
        {
            // se non c'è testo, salviamo testo vuoto (anche se RTF non è vuoto), che significa che la nota sarà cancellata
            testi.SetNotaTesto(testo, nota, versione);
            foreach (Form formFiglio in MdiChildren)
            {
                if (formFiglio.Tag.ToString() == "ApriNota")
                {
                    ApriNota formApriNota = (ApriNota)formFiglio;
                    if (formApriNota.Versione == versione)
                        formApriNota.AggiornaElenchi(TestoTipi.Commentario | TestoTipi.Dizionario);
                }
            }
        }

        public void CreaNuovaNota(string testo, Riferimento riferimento, string versione)
        {
            if (riferimento == null)
                throw new ArgumentNullException("riferimento");
            else
                CreaNuovaNota(testo, riferimento.ComeNotaTuttoRiferimento(), versione);
        }

        /// <summary>
        /// Apre la nota di un titolo nell'editor.
        /// </summary>
        /// <param name="titolo">Il titolo della nota da aprire</param>
        /// <param name="nomeVersione">La nomeVersione della nota da aprire</param>
        public void ApriNotaInEditor(string titolo, string versione)
        {
            String testo;
            try
            {
                testo = testi.GetNotaTesto(titolo, versione);
            }
            catch (TextNotExistException)
            {
                return;
            }

            Cursor cursoreAttuale = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            Editor formEditor = new Editor(this, titolo, versione)
            {
                MdiParent = this,
                Text = titolo.StartsWith("#", StringComparison.Ordinal) ? testi.ConvertiTitoloNotaARiferimento(titolo) : titolo + " (" + versione + ")"
            };
            try
            {
                formEditor.rtEditor.Rtf = testo;
                formEditor.rtEditor.MostraLink();
                formEditor.rtEditor.SetSelectionLink(false); // altrimenti a volte l'ultimo link rimane evidenziato dopo l'apertura
            }
            catch
            {
                formEditor.rtEditor.Text = testo;
            }
            finally
            {
                Cursor.Current = cursoreAttuale;
                if (cursoreAttuale != null)
                    cursoreAttuale.Dispose();
            }
            /*
            if (titolo.StartsWith("#",StringComparison.InvariantCulture) && formEditor.rtEditor.Text.IndexOf(RichTextBoxEx.InizioRiferimento) < 0)
            { // inserisci i codici per indicare il riferimento del versetto (se non sono già stati inserti)
                formEditor.rtEditor.SelectionStart = 0;
                formEditor.rtEditor.SelectedRtf = @"{\rtf1\v " + RichTextBoxEx.InizioRiferimento + titolo.Substring(1, 8) + " }";
            }
            */
            formEditor.Show();
            formEditor.rtEditor.Lingua = testi.Info(versione).Lingua;
            formEditor.rtEditor.Versione = versione;
            formEditor.rtEditor.Modified = false; // string.IsNullOrEmpty(testo);
            if (testi.Info(versione).Bloccato != BloccatoTipi.Sbloccato)
                formEditor.rtEditor.ReadOnly = true;
            Collection<string> ordine = testi.GetNoteInOrdine(versione);
            for (int i = 0; i < ordine.Count; ++i)
                ordine[i] = ordine[i].TrimStart();
            if (ordine.Count == 0 || (ordine.Count == 1 && string.IsNullOrEmpty(ordine[0])))
            {
                formEditor.MostraOrdine = false;
            }
            else
            {
                formEditor.MostraOrdine = true;
                if (!string.IsNullOrEmpty(ordine[0]))
                {
                    formEditor.NotaIndice = ordine[0];
                    orderIndexToolStripButton.Enabled = true;
                }
                else
                    orderIndexToolStripButton.Enabled = false;

                string[] notePrecedenteSuccessiva = testi.NotePrecedenteSuccessiva(versione, titolo);
                formEditor.NotaPrecedente = notePrecedenteSuccessiva[0];
                orderPreviousToolStripButton.Enabled = (!string.IsNullOrEmpty(formEditor.NotaPrecedente));

                formEditor.NotaProssima = notePrecedenteSuccessiva[1];
                orderNextToolStripButton.Enabled = (!string.IsNullOrEmpty(formEditor.NotaProssima));
            }
            orderToolStrip.Visible = (formEditor.MostraOrdine && Settings.Default.PrincipaleBSOrdine);
        }

        #endregion

        #region Link

        public void LinkHover(LinkHoverEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");
            else
                LinkHover("", e);
        }

        public void LinkHover(string versionePerIpertesto, LinkHoverEventArgs e)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            if (Settings.Default.OpzioniIpertestoTooltip)
            {
                String[] testoEVersione = LinkCliccato(null, e.RichText.Versione, versionePerIpertesto, e.LinkTesto, false);
                SetBarraDiStatoTesto(testoEVersione[2]);
                if (!string.IsNullOrEmpty(testoEVersione[0]))
                    e.RichText.MostraHover(testoEVersione[0], testoEVersione[1], Cursor.Position, Settings.Default.OpzioniIpertestoTooltipInTooltip);
            }
        }

        public void LinkCliccato(Visualizza finestra, string versioneDelTesto, string linkTesto)
        {
            if (linkTesto == null)
                throw new ArgumentNullException("linkTesto");
            else
                LinkCliccato(finestra, versioneDelTesto, "", linkTesto, true);
        }

        public string[] LinkCliccato(string versioneDelTesto, string versionePerIpertesto, string linkTesto)
        {
            return LinkCliccato(null, versioneDelTesto, versionePerIpertesto, linkTesto, true);
        }

        public string[] LinkCliccato(Visualizza finestra, string versioneDelTesto, string versionePerIpertesto, string linkTesto, bool creaFinestra)
        {
            if (linkTesto == null)
                throw new ArgumentNullException("linkTesto");

            // restituisce un array con il testo da visualizzare, la versione, e il testo del link
            string[] testoEVersione = new string[3];
            int posizioneDivisione = -1;
            bool divisioneTrovata = false;
            char tipoLink = RichTextBoxEx.FineLinkFile; // predefinito: ad un file
            while (!divisioneTrovata)
            {
                posizioneDivisione = linkTesto.IndexOf(RichTextBoxEx.FineLink1, posizioneDivisione + 1);
                if (posizioneDivisione == -1 || posizioneDivisione == linkTesto.Length - 1)
                    break;
                if (linkTesto[posizioneDivisione + 1] >= RichTextBoxEx.FineLinkBrano && linkTesto[posizioneDivisione + 1] <= RichTextBoxEx.FineLinkFile)
                {
                    divisioneTrovata = true;
                    tipoLink = linkTesto[posizioneDivisione + 1];
                }
            }
            string nomeLink = divisioneTrovata ? linkTesto.Substring(posizioneDivisione + 2) : linkTesto;
            if (!string.IsNullOrEmpty(nomeLink) && nomeLink[nomeLink.Length - 1] == RichTextBoxEx.FineLink2)
                nomeLink = nomeLink.Remove(nomeLink.Length - 1);
            if (String.IsNullOrEmpty(nomeLink) && divisioneTrovata) // se non c'è un link a cui andare, supponiamo che è uguale al testo del link visualizzato
                nomeLink = linkTesto.Substring(0, posizioneDivisione);
            testoEVersione[2] = nomeLink;
            switch (tipoLink)
            {
                case RichTextBoxEx.FineLinkBrano:
                    string versionePredefinita = testi.UltimaBibbia;
                    if (nomeLink.IndexOf(@"\", StringComparison.Ordinal) > 0) // in questo modo, è possibile creare un link "Nuova Riveduta\#010010010000-01001002000"
                    {
                        versionePredefinita = nomeLink.Remove(nomeLink.IndexOf(@"\", StringComparison.Ordinal));
                        nomeLink = nomeLink.Substring(nomeLink.IndexOf(@"\", StringComparison.Ordinal) + 1);
                    }
                    testoEVersione[2] = testi.ConvertiTitoloNotaARiferimento(nomeLink);
                    Riferimento riferimento = testi.ConvertiRiferimento(testoEVersione[2]);
                    if (!string.IsNullOrEmpty(versioneDelTesto)) // versioneDelTesto="" quando una finestra di testo, non di una collezione
                    {
                        riferimento = testi.ConvertiAStandard(riferimento, versioneDelTesto);
                        string versioneDelleNote = testi.Info(versionePredefinita).VersioneDelleNote;
                        if (!string.IsNullOrEmpty(versioneDelleNote))
                            versionePredefinita = versioneDelleNote;
                    }
                    string abbVersione = testi.Info(testi.UltimaBibbia).Abbreviazione;
                    riferimento = testi.ConvertiDaStandard(riferimento, versionePredefinita);
                    testoEVersione[0] = testi.TestoBrano(riferimento, versionePredefinita);
                    testoEVersione[1] = versionePredefinita;
                    if (creaFinestra)
                    {
                        if (riferimento.Count == 0)
                        {
                            // non fare niente
                        }
                        // un + invece di un - nel riferimento indica sempre nella finestra Visualizza (usato per mostrare i risultati di una ricerca in contesto)
                        else if ((riferimento.Count == 1 && riferimento.SoloUnoVersetto(0)) || nomeLink.Contains("+"))
                        {
                            Visualizza formVisualizza = new Visualizza(this, versionePredefinita, TestoTipi.Bibbia);
                            formVisualizza.SpostaTesto(riferimento, false);
                            formVisualizza.MdiParent = this;
                            formVisualizza.Show();
                        }
                        else
                        {
                            Editor fEditor = new Editor(this)
                            {
                                MdiParent = this
                            };
                            fEditor.Show();
                            fEditor.Text = testi.NormalizzaRiferimento(riferimento) + " (" + abbVersione + ")";
                            fEditor.rtEditor.Rtf = testoEVersione[0];
                            fEditor.rtEditor.Lingua = testi.Info(versionePredefinita).Lingua;
                            fEditor.rtEditor.Modified = false;
                        }
                    }
                    break;
                case RichTextBoxEx.FineLinkNota:
                    string collezioneNuovaNota = "";
                    if (!string.IsNullOrEmpty(versioneDelTesto)) // una nota
                        collezioneNuovaNota = versioneDelTesto;
                    if (!string.IsNullOrEmpty(versionePerIpertesto))
                        collezioneNuovaNota = versionePerIpertesto;
                    if (nomeLink.IndexOf(@"\", StringComparison.Ordinal) > 0)
                    {
                        collezioneNuovaNota = nomeLink.Remove(nomeLink.IndexOf(@"\", StringComparison.Ordinal) + 1);
                        nomeLink = nomeLink.Substring(nomeLink.IndexOf(@"\", StringComparison.Ordinal) + 1);
                    }
                    if (collezioneNuovaNota.EndsWith(@"\", StringComparison.Ordinal))
                        collezioneNuovaNota = collezioneNuovaNota.Remove(collezioneNuovaNota.Length - 1);

                    if (string.IsNullOrEmpty(collezioneNuovaNota))
                    { // non si sa ancora quale collezione usare; proviamo con i dizionari
                        if (!string.IsNullOrEmpty(nomeLink))
                        {
                            if (Funzioni.IsLetteraGreca(nomeLink[0]))
                                collezioneNuovaNota = Settings.Default.DizionarioGreco;
                            else
                            {
                                string lingua = Settings.Default.InterfacciaLingua;
                                if (lingua.Length >= 2)
                                {
                                    lingua = lingua.Substring(0, 2).ToLowerInvariant();
                                    if (lingua == "it")
                                        collezioneNuovaNota = Settings.Default.DizionarioItaliano;
                                    else if (lingua == "en")
                                        collezioneNuovaNota = Settings.Default.DizionarioInglese;
                                    else if (lingua == "es")
                                        collezioneNuovaNota = Settings.Default.DizionarioSpagnolo;
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(collezioneNuovaNota))
                    {
                        testoEVersione[0] = testi.GetNotaTesto(nomeLink, collezioneNuovaNota);
                        testoEVersione[1] = collezioneNuovaNota;
                        if (creaFinestra)
                        {
                            if (finestra != null && finestra.Tag != null && finestra.Tag.ToString() == "Visualizza" && (((Visualizza)(finestra)).paneAttivo.TuttiTesti != TestoTipi.None || finestra.Text.StartsWith(collezioneNuovaNota, StringComparison.Ordinal)))
                            {
                                if (nomeLink.StartsWith("#", StringComparison.Ordinal))
                                    finestra.SpostaTesto(testi.ConvertiRiferimento(nomeLink), true);
                                else
                                    finestra.SpostaTesto(nomeLink, true);
                            }
                            else
                                ApriNotaInEditor(nomeLink, collezioneNuovaNota);
                        }
                        testoEVersione[2] = nomeLink;
                        if (testoEVersione[2].StartsWith("#", StringComparison.Ordinal))
                            testoEVersione[2] = testi.ConvertiTitoloNotaARiferimento(testoEVersione[2]);
                    }
                    break;
                default: // predefinito è RichTextBoxEx.FineLinkFile, ad un file
                    if (creaFinestra)
                    {
                        if (!File.Exists(nomeLink))
                        {
                            string nomeDelFileDellaCollezione = testi.Info(versioneDelTesto).NomeDelFile;
                            try
                            {
                                if (!string.IsNullOrEmpty(nomeDelFileDellaCollezione))
                                { // solo se la nota fa parte di una collezione
                                    string[] fileTrovati = Directory.GetFiles(Path.GetDirectoryName(nomeDelFileDellaCollezione), nomeLink + ".*");
                                    if (fileTrovati.Length > 0)
                                        nomeLink = fileTrovati[0];
                                    else
                                    { // proviamo anche nella sottocartella con lo stesso nome della collezione
                                        string[] fileTrovatiSotto = Directory.GetFiles(Path.GetDirectoryName(nomeDelFileDellaCollezione) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(nomeDelFileDellaCollezione), nomeLink + ".*");
                                        if (fileTrovatiSotto.Length > 0)
                                            nomeLink = fileTrovatiSotto[0];
                                    }
                                }
                            }
                            catch
                            {
                                // un Internet link, o altro testo che non è lecito per un percorso, dà un errore qui; basta saltare
                            }
                        }
                        if (!File.Exists(nomeLink))
                        {
                            try
                            {
                                string[] fileTrovati = Directory.GetFiles(Application.StartupPath, nomeLink + ".*");
                                if (fileTrovati.Length > 0)
                                    nomeLink = fileTrovati[0];
                            }
                            catch
                            {
                                // un Internet link, o altro testo che non è lecito per un percorso, dà un errore qui; basta saltare
                            }
                        }
                        if (!File.Exists(nomeLink))
                        {
                            try
                            {
                                string[] fileTrovati = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar + "LaParola", nomeLink + ".*");
                                if (fileTrovati.Length > 0)
                                    nomeLink = fileTrovati[0];
                            }
                            catch
                            {
                                // un Internet link, o altro testo che non è lecito per un percorso, dà un errore qui; basta saltare
                            }
                        }
                        if (!File.Exists(nomeLink))
                        {
                            string[] cartelle = Settings.Default.CartelleDaCercare.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                            try
                            {
                                foreach (string cartella in cartelle)
                                {
                                    string[] fileTrovati = Directory.GetFiles(cartella, nomeLink + ".*");
                                    if (fileTrovati.Length > 0)
                                    {
                                        nomeLink = fileTrovati[0];
                                        break;
                                    }
                                }
                            }
                            catch
                            {
                                // un Internet link, o altro testo che non è lecito per un percorso, dà un errore qui; basta saltare
                            }
                        }

                        nomeLink = nomeLink.Replace(RichTextBoxEx.ParolaRicercata.ToString(), "");
                        string estensione = Path.GetExtension(nomeLink);
                        if (File.Exists(nomeLink) && (string.Compare(estensione, ".gif", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(estensione, ".jpg", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(estensione, ".jpeg", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(estensione, ".bmp", StringComparison.OrdinalIgnoreCase) == 0 || string.Compare(estensione, ".png", StringComparison.OrdinalIgnoreCase) == 0))
                            ApriNomeImmagini(new string[] { nomeLink });
                        else
                        {
                            try
                            {
                                Funzioni.ApriBrowser(nomeLink, true);
                            }
                            catch (Exception exc)
                            {
                                MessageBox.Show(string.Format(CultureInfo.CurrentCulture, LocRM.GetString("EditorErrorCantStartFile"), nomeLink, exc.Message), LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, messageBoxOptions);
                            }
                        }
                    }
                    else
                    {
                        testoEVersione[0] = nomeLink;
                    }
                    break;
            }
            return testoEVersione;
        }

        /// <summary>
        /// Restituisce informazioni quando il mouse è usato per saltare ad un dizionario
        /// </summary>
        /// <param name="posizione">La posizione nel testo cliccata o con lo hover</param>
        /// <param name="nomeVersione">La nomeVersione del testo (in cui cercare la radice)</param>
        /// <returns>Un array di tre stringhe, con la parola sotto il mouse, il testo del link, e il dizionario usato.</returns>
        public static string[] TestoDalDizionario(RichTextBoxEx rtb, int posizione, string versione)
        {
            if (rtb == null)
                throw new ArgumentNullException("rtb");

            string parolaAttuale = rtb.ParolaAttuale(posizione);
            string[] parolaTestoDizionario = new string[3];
            string dizionario = "";
            char primaLettera = (!string.IsNullOrEmpty(parolaAttuale) ? parolaAttuale[0] : (char)0);
            bool cercaRadiceInDizionario = true;
            // se la parola è greca/ebraica in un testo in un'altra lingua, usa il dizionario appropriato
            // anche la radice della parola deve essere cercata nel dizionario greco/ebraico, non nella nomeVersione del testo
            if (Funzioni.IsLetteraGreca(primaLettera))
                dizionario = Settings.Default.DizionarioGreco;
            else if ((primaLettera >= '\u0591' && primaLettera <= '\u05ff') || (primaLettera >= '\ufb1e' && primaLettera <= '\u5b4f'))
                dizionario = Settings.Default.DizionarioEbraico;
            else
            {
                cercaRadiceInDizionario = string.IsNullOrEmpty(versione); // se non c'è nomeVersione (per es un file non in una Bibbia o collezione) controlliamo invece del dizionario
                foreach (string linguaDelTesto in testi.Info(versione).Lingua.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (string.IsNullOrEmpty(dizionario))
                    {
                        switch (linguaDelTesto)
                        {
                            case "it":
                                dizionario = Settings.Default.DizionarioItaliano;
                                break;
                            case "en":
                                dizionario = Settings.Default.DizionarioInglese;
                                break;
                            case "es":
                                dizionario = Settings.Default.DizionarioSpagnolo;
                                break;
                            case "la":
                                dizionario = Settings.Default.DizionarioLatino;
                                break;
                        }
                    }
                }
                if (string.IsNullOrEmpty(dizionario))
                {
                    switch (Settings.Default.InterfacciaLingua.ToUpperInvariant())
                    {
                        case "IT":
                            dizionario = Settings.Default.DizionarioItaliano;
                            break;
                        case "EN":
                            dizionario = Settings.Default.DizionarioInglese;
                            break;
                        case "ES":
                            dizionario = Settings.Default.DizionarioSpagnolo;
                            break;
                        default:
                            dizionario = Settings.Default.DizionarioInglese;
                            break;
                    }
                }
            }

            string testo = "";
            if (!string.IsNullOrEmpty(parolaAttuale) && !string.IsNullOrEmpty(dizionario))
            {
                testo = testi.GetNotaTesto(parolaAttuale, dizionario);
                if (string.IsNullOrEmpty(testo))
                {
                    string parolaAttualeTrovata = "";
                    try
                    {
                        parolaAttualeTrovata = testi.RadiceDiParola(parolaAttuale, cercaRadiceInDizionario ? dizionario : versione);
                        parolaAttuale = ((string.IsNullOrEmpty(parolaAttualeTrovata) && cercaRadiceInDizionario) ? testi.RadiceDiParola(parolaAttuale, versione) : parolaAttualeTrovata);
                    }
                    catch (TextNotExistException)
                    { // succede quando la versione è stata cancellata
                        parolaAttuale = "";
                    }

                    primaLettera = (!string.IsNullOrEmpty(parolaAttuale) ? parolaAttuale[0] : (char)0);
                    if (Funzioni.IsLetteraGreca(primaLettera))
                        dizionario = Settings.Default.DizionarioGreco;
                    else if ((primaLettera >= '\u0591' && primaLettera <= '\u05ff') || (primaLettera >= '\ufb1e' && primaLettera <= '\u5b4f'))
                        dizionario = Settings.Default.DizionarioEbraico;
                    else if (primaLettera == 'H' && parolaAttuale.Length >= 2 && parolaAttuale[1] == '8')
                        dizionario = Settings.Default.DizionarioEbraico; // nel dizionario Strongs Hebrew, le voci di tempo voce modo hanno radice H8xxx

                    // dizionario può essere "" se non c'è un dizionario ebraico installato
                    testo = (string.IsNullOrEmpty(dizionario) ? "" : testi.GetNotaTesto(parolaAttuale, dizionario));
                    if (string.IsNullOrEmpty(testo))
                        parolaAttuale = "";
                }
            }

            parolaTestoDizionario[0] = parolaAttuale;
            parolaTestoDizionario[1] = testo;
            parolaTestoDizionario[2] = dizionario;
            return parolaTestoDizionario;
        }

        #endregion

        #region UltimaBibbia

        private void ChangeUltimaBibbia(object sender, UltimaBibbiaEventArgs e)
        {
            try
            {
                statusTranslations.Text = e.NuovaBibbia;
            }
            catch (InvalidOperationException) // un altro thread ha cercato di cambiare l'ultima Bibbia; ignoriamo (anche se così a volte la barra di stato è sbagliata)
            {
            }
        }

        private void CambiaBibbiaUtilizzata(object sender, EventArgs e)
        {
            if (sender == null)
                throw new ArgumentNullException("sender");
            else
                testi.UltimaBibbia = ((ToolStripItem)sender).Text;
        }

        private void statusTranslations_DropDownOpening(object sender, EventArgs e)
        {
            foreach (ToolStripItem voce in statusTranslations.DropDownItems)
            {
                if (voce.Text == testi.UltimaBibbia)
                {
                    voce.Select();
                    break;
                }
            }
        }

        #endregion

        #region Strumenti in thread

        #region RicercaInEditor

        struct ThreadRicercaArgomenti
        {
            public string espressione;
            public string brano;
            public string versione;
            public bool errore;
            public Riferimento versettiConFrase;
            public string testo; // il risultato
            public BarraConEtichetta barra;
        }

        internal Riferimento RicercaInEditor(string espressione, string brano, string versione)
        {
            ThreadRicercaArgomenti argomenti = new ThreadRicercaArgomenti
            {
                barra = CreaBarraDiStato(Principale.LocRM.GetString("SearchCurrent"), 0, 10),
                brano = brano,
                versione = versione,
                espressione = espressione,

                errore = false
            };
            Riferimento versettiConFrase = new Riferimento();
            try
            {
                versettiConFrase = Principale.testi.Ricerca(argomenti.espressione, argomenti.brano, argomenti.versione);
            }
            catch (SearchExpressionEmptyException)
            {
                MessageBox.Show(Principale.LocRM.GetString("SearchExpressionEmpty"), Principale.LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions);
                argomenti.errore = true;
            }
            catch (SearchSyntaxErrorException ex)
            {
                MessageBox.Show(String.Format(CultureInfo.InvariantCulture, Principale.LocRM.GetString("SearchSyntax"), ex.Message), Principale.LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions);
                argomenti.errore = true;
            }
            catch (SearchParenthesesException)
            {
                MessageBox.Show(Principale.LocRM.GetString("SearchBrackets"), Principale.LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions);
                argomenti.errore = true;
            }
            catch (SearchBracketsException)
            {
                MessageBox.Show(Principale.LocRM.GetString("SearchSquareBrackets"), Principale.LocRM.GetString("MiscError"), MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions);
                argomenti.errore = true;
            }

            argomenti.versettiConFrase = versettiConFrase;
            argomenti.barra.Massimo = versettiConFrase.Count + 3;
            argomenti.barra.Valore = 1;

            BackgroundWorker backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(RicercaFraseProgresso);

            if (isRunningOnMono)
            {
                if (!argomenti.errore)
                    argomenti.testo = Principale.testi.TestoBrano(argomenti.versettiConFrase, argomenti.versione, backgroundWorker, new DoWorkEventArgs(argomenti));
                FraseRicercata(backgroundWorker, new RunWorkerCompletedEventArgs(argomenti, null, false));
            }
            else
            {
                backgroundWorker.DoWork += new DoWorkEventHandler(RicercaFrase);
                backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(FraseRicercata);
                backgroundWorker.RunWorkerAsync(argomenti);
            }

            return versettiConFrase;
        }

        private void RicercaFrase(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            ThreadRicercaArgomenti argomenti = (ThreadRicercaArgomenti)e.Argument;

            if (!argomenti.errore)
                argomenti.testo = Principale.testi.TestoBrano(argomenti.versettiConFrase, argomenti.versione, worker, e);
            e.Result = argomenti;

            if (worker.CancellationPending)
                e.Cancel = true;
        }

        private void RicercaFraseProgresso(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                int valore = e.ProgressPercentage;
                if (valore >= 0) // è il valore da impostare
                    ((ThreadRicercaArgomenti)(((DoWorkEventArgs)(e.UserState)).Argument)).barra.Valore = e.ProgressPercentage;
                else // è il negativo dell'aumento 
                    ((ThreadRicercaArgomenti)(((DoWorkEventArgs)(e.UserState)).Argument)).barra.Aumenta(-valore);
            }
            catch (NullReferenceException) // succede quando programma è chiuso mentre thread in esecuzione
            {
                ((BackgroundWorker)sender).CancelAsync();
            }
            ((ThreadRicercaArgomenti)(((DoWorkEventArgs)(e.UserState)).Argument)).barra.Ridisegna();
        }

        private void FraseRicercata(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                return;

            ThreadRicercaArgomenti argomenti = (ThreadRicercaArgomenti)e.Result;

            if (!argomenti.errore)
            {
                string espressione = argomenti.espressione;
                string caption = espressione;
                if (!String.IsNullOrEmpty(argomenti.brano))
                    caption += " in " + Principale.testi.NormalizzaRiferimento(argomenti.brano);
                caption += " (" + Principale.testi.Info(argomenti.versione).Abbreviazione + ") " + argomenti.versettiConFrase.Count.ToString(CultureInfo.CurrentCulture) + " ";
                caption += (argomenti.versettiConFrase.Count == 1 ? Principale.LocRM.GetString("SearchTime") : Principale.LocRM.GetString("SearchTimes"));

                Editor fEditor;
                if (Settings.Default.OpzioniStessaFinestra && finestraRisultati != null)
                {
                    fEditor = finestraRisultati;
                }
                else
                {
                    fEditor = new Editor(this)
                    {
                        MdiParent = this
                    };
                    fEditor.Show();
                }
                fEditor.Text = caption;
                fEditor.VersionePerIpertesto = argomenti.versione;
                Application.DoEvents();
                fEditor.rtEditor.BloccaRtf(true);
                fEditor.rtEditor.Rtf = argomenti.testo;
                Application.DoEvents();
                fEditor.rtEditor.MostraLink();
                Application.DoEvents();
                if (isRunningOnMono && fEditor.rtEditor.Text.IndexOf(RichTextBoxEx.ParolaRicercata) > 0)
                {
                    fEditor.rtEditor.SelectionStart = fEditor.rtEditor.Text.IndexOf(RichTextBoxEx.ParolaRicercata);
                    fEditor.rtEditor.ScrollToCaret();
                }
                fEditor.rtEditor.BloccaRtf(false);
                fEditor.rtEditor.Versione = argomenti.versione;
                fEditor.rtEditor.Lingua = Principale.testi.Info(argomenti.versione).Lingua;
                fEditor.rtEditor.Modified = false;
                if (Settings.Default.OpzioniStessaFinestra)
                {
                    finestraRisultati = fEditor;
                    fEditor.Activate();
                }
            }

            (((ThreadRicercaArgomenti)e.Result).barra).MessaggioCompleto(Principale.LocRM.GetString("SearchCompleted"));
            (((ThreadRicercaArgomenti)e.Result).barra).Chiudi();
            //            SetBarraDiStatoTesto(Principale.LocRM.GetString("SearchCompleted"));
        }

        #endregion

        #region MostraBranoInEditor

        struct ThreadMostraArgomenti
        {
            public Riferimento brano;
            public Collection<string> versioni;
            public string abbreviazioniVersioni;
            public bool alternare;
            public bool almenoUnCommentario;
            public bool almenoUnaBibbia;
            public string testo; // il risultato
            public BarraConEtichetta barra;
        }

        internal void MostraBranoInEditor(string branoDaMostrare, string versione)
        {
            Collection<string> versioni = new Collection<string>
            {
                versione
            };
            MostraBranoInEditor(branoDaMostrare, versioni);
        }

        internal void MostraBranoInEditor(string branoDaMostrare, Collection<string> versioni)
        {
            MostraBranoInEditor(Principale.testi.ConvertiRiferimento(branoDaMostrare), versioni);
        }

        internal void MostraBranoInEditor(string branoDaMostrare, Collection<string> versioni, bool alternare)
        {
            MostraBranoInEditor(Principale.testi.ConvertiRiferimento(branoDaMostrare), versioni, alternare);
        }

        internal void MostraBranoInEditor(Collection<string> noteDaMostrare, string versione)
        {
            Riferimento riferimentoDaMostrare = new Riferimento(false);
            foreach (string nota in noteDaMostrare)
                riferimentoDaMostrare.AggiungiNotaEParole(nota, new Collection<UInt16>());
            MostraBranoInEditor(riferimentoDaMostrare, versione);
        }

        internal void MostraBranoInEditor(Riferimento riferimentoDaMostrare, string versione)
        {
            Collection<string> versioni = new Collection<string>
            {
                versione
            };
            MostraBranoInEditor(riferimentoDaMostrare, versioni);
        }

        internal void MostraBranoInEditor(Riferimento riferimentoDaMostrare, Collection<string> versioni)
        {
            MostraBranoInEditor(riferimentoDaMostrare, versioni, false);
        }

        internal void MostraBranoInEditor(Riferimento riferimentoDaMostrare, Collection<string> versioni, bool alternare)
        {
            string abbVersioni = "";
            bool almenoUnCommentario = false;
            bool almenoUnaBibbia = false;

            foreach (string versione in versioni)
            {
                VersioneInformazioni info = Principale.testi.Info(versione);
                abbVersioni += info.Abbreviazione + ", ";
                if ((info.Tipo & TestoTipi.Commentario) == TestoTipi.Commentario)
                    almenoUnCommentario = true;
                if ((info.Tipo & TestoTipi.Bibbia) == TestoTipi.Bibbia)
                    almenoUnaBibbia = true;
            }
            if (!String.IsNullOrEmpty(abbVersioni))
                abbVersioni = " (" + abbVersioni.Remove(abbVersioni.Length - 2) + ")";

            ThreadMostraArgomenti argomenti = new ThreadMostraArgomenti
            {
                barra = CreaBarraDiStato(Principale.LocRM.GetString("ShowCurrent"), 0, riferimentoDaMostrare.Count * versioni.Count + 2),
                brano = riferimentoDaMostrare,
                alternare = alternare,
                almenoUnCommentario = almenoUnCommentario,
                almenoUnaBibbia = almenoUnaBibbia,
                versioni = versioni,
                abbreviazioniVersioni = abbVersioni
            };

            BackgroundWorker backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(MostraBranoProgresso);
            // if (true)
            if (isRunningOnMono)
            {
                argomenti.testo = Principale.testi.TestoBrano(argomenti.brano, argomenti.versioni, backgroundWorker, new DoWorkEventArgs(argomenti));
                BranoMostrato(backgroundWorker, new RunWorkerCompletedEventArgs(argomenti, null, false));
            }
            else
            {
                backgroundWorker.DoWork += new DoWorkEventHandler(MostraBrano);
                backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BranoMostrato);
                backgroundWorker.RunWorkerAsync(argomenti);
            }
        }

        private void MostraBrano(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            ThreadMostraArgomenti argomenti = (ThreadMostraArgomenti)e.Argument;

            if (argomenti.almenoUnaBibbia)
            {
                argomenti.testo = Principale.testi.TestoBrano(argomenti.brano, argomenti.versioni, argomenti.alternare, worker, e);
            }
            else
            {
                // in questo caso diciamo a TestoBrano che ci sono solo commentari
                // così commentari senza una nota sul brano non sono visualizzati, ma se TestoBrano è chiamato nell'altra modo sono visualizzati
                argomenti.testo = Principale.testi.TestoBrano(argomenti.brano, new Collection<string>(), argomenti.versioni, argomenti.alternare, worker, e);
            }
            e.Result = argomenti;

            if (worker.CancellationPending)
                e.Cancel = true;
        }

        private void MostraBranoProgresso(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                int valore = e.ProgressPercentage;
                if (valore >= 0) // è il valore da impostare
                    ((ThreadMostraArgomenti)(((DoWorkEventArgs)(e.UserState)).Argument)).barra.Valore = e.ProgressPercentage;
                else // è il negativo dell'aumento
                    ((ThreadMostraArgomenti)(((DoWorkEventArgs)(e.UserState)).Argument)).barra.Aumenta(-valore);
                ((ThreadMostraArgomenti)(((DoWorkEventArgs)(e.UserState)).Argument)).barra.Ridisegna();
            }
            catch (NullReferenceException) // succede quando programma è chiuso mentre thread in esecuzione
            {
                ((BackgroundWorker)sender).CancelAsync();
            }
        }

        private void BranoMostrato(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                return;

            ThreadMostraArgomenti argomenti = (ThreadMostraArgomenti)e.Result;

            Editor fEditor;
            if (Settings.Default.OpzioniStessaFinestra && finestraRisultati != null)
            {
                fEditor = finestraRisultati;
            }
            else
            {
                fEditor = new Editor(this)
                {
                    MdiParent = this
                };
                fEditor.Show();
            }
            fEditor.Text = Principale.testi.NormalizzaRiferimento(argomenti.brano) + argomenti.abbreviazioniVersioni;
            Application.DoEvents();
            fEditor.rtEditor.BloccaRtf(true);
            fEditor.rtEditor.Rtf = argomenti.testo;
            Application.DoEvents();
            if (argomenti.almenoUnCommentario || !argomenti.brano.Versetti) // cioè non quando è solo il testo biblico
                if (argomenti.testo.Length < LUNGHEZZA_MASSIMA_PER_MOSTRARE_LINK) // cioè non quando il testo è così lungo, che ci vorrà troppo tempo per creare tutti i link
                    fEditor.rtEditor.MostraLink();
            fEditor.rtEditor.BloccaRtf(false);
            Application.DoEvents();
            fEditor.rtEditor.Versione = argomenti.versioni[0];
            fEditor.rtEditor.Lingua = Principale.testi.Info(argomenti.versioni[0]).Lingua;
            fEditor.rtEditor.Modified = false;
            if (Settings.Default.OpzioniStessaFinestra)
            {
                finestraRisultati = fEditor;
                fEditor.Activate();
            }

            (((ThreadMostraArgomenti)e.Result).barra).MessaggioCompleto(Principale.LocRM.GetString("ShowCompleted"));
            (((ThreadMostraArgomenti)e.Result).barra).Chiudi();
            Application.DoEvents();
        }

        #endregion

        #region MostraDefinizioniInEditor

        struct ThreadDefinizioniArgomenti
        {
            public string brano;
            public string versione;
            public string dizionario;
            public string testo; // il risultato
            public BarraConEtichetta barra;
        }

        internal void MostraDefinizioniInEditor(string brano, string versione)
        {
            int limiteProgresso = Principale.testi.Parole(versione).Length;

            ThreadDefinizioniArgomenti argomenti = new ThreadDefinizioniArgomenti
            {
                barra = CreaBarraDiStato(Principale.LocRM.GetString("DefinitionsCurrent"), 0, limiteProgresso),
                brano = brano,
                versione = versione,
                dizionario = Funzioni.DizionarioDiVersione(versione)
            };

            BackgroundWorker backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(MostraDefinizioniProgresso);

            // if (true)
            if (isRunningOnMono)
            {
                argomenti.testo = TestoDefinizioni(backgroundWorker, argomenti);
                DefinizioniMostrate(backgroundWorker, new RunWorkerCompletedEventArgs(argomenti, null, false));
            }
            else
            {
                backgroundWorker.DoWork += new DoWorkEventHandler(MostraDefinizioni);
                backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(DefinizioniMostrate);
                backgroundWorker.RunWorkerAsync(argomenti);
            }
        }

        private void MostraDefinizioni(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            ThreadDefinizioniArgomenti argomenti = (ThreadDefinizioniArgomenti)e.Argument;

            argomenti.testo = TestoDefinizioni(worker, argomenti);
            e.Result = argomenti;

            if (worker.CancellationPending)
                e.Cancel = true;
        }

        static private string TestoDefinizioni(BackgroundWorker worker, ThreadDefinizioniArgomenti argomenti)
        {
            string versione = argomenti.versione;
            string dizionario = argomenti.dizionario;
            string brano = argomenti.brano;

            int nParole = Principale.testi.Parole(versione).Length;
            string[] parole = new string[nParole];
            Array.Copy(Principale.testi.Parole(versione), parole, nParole);

            Riferimento[] apparenze = new Riferimento[nParole];
            Riferimento riferimentoBrano = Principale.testi.ConvertiRiferimento(brano);
            for (int i = 0; i < nParole; ++i)
            {
                apparenze[i] = Principale.testi.RicercaParolaInBrano(parole[i], riferimentoBrano, versione);
                worker.ReportProgress(i, argomenti.barra);
            }
            Array.Sort(apparenze, parole, new Riferimento()); // new Riferimento() serve solo per usare Riferimento.Compare

            string testo = "";
            if (!string.IsNullOrEmpty(dizionario))
            {
                RichTextBoxEx rtb = new RichTextBoxEx();
                string testoNota;
                bool primaParola = true;
                string radice, titoloNota;
                string intestazione = Principale.testi.RtfIntestazione() + @"\par{\fs" + Convert.ToString(Convert.ToInt32(Principale.testi.Formato.FontDimensione * 2 + 2), CultureInfo.InvariantCulture) + @"\b ";
                List<string> radiciMostrate = new List<string>(nParole);
                for (int i = 0; i < nParole; ++i)
                {
                    if (apparenze[i].Count > 0)
                    {
                        testoNota = "";
                        titoloNota = "";
                        // cerchiamo nel dizionario prima la radice, se la nota della radice non è stata ancora aggiunta
                        // se non c'è una nota per la radice, proviamo anche la parola stessa
                        radice = Principale.testi.RadiceDiParola(parole[i], versione);
                        if (!string.IsNullOrEmpty(radice))
                        {
                            int nRadice = radiciMostrate.IndexOf(radice);
                            if (nRadice < 0)
                            {
                                testoNota = Principale.testi.GetNotaTesto(radice, dizionario);
                                radiciMostrate.Add(radice);
                                if (string.IsNullOrEmpty(testoNota))
                                {
                                    testoNota = Principale.testi.GetNotaTesto(parole[i], dizionario);
                                    titoloNota = parole[i];
                                }
                                else
                                {
                                    titoloNota = radice;
                                }
                            }
                        }
                        else
                        {
                            testoNota = Principale.testi.GetNotaTesto(parole[i], dizionario);
                            titoloNota = parole[i];
                        }
                        if (!string.IsNullOrEmpty(testoNota))
                        {
                            if (primaParola)
                            {
                                rtb.Rtf = Principale.testi.RtfIntestazione() + @"{\fs" + Convert.ToString(Convert.ToInt32(Principale.testi.Formato.FontDimensione * 2 + 2), CultureInfo.InvariantCulture) + @"\b " + titoloNota + @"\par}\par}";
                                primaParola = false;
                            }
                            else
                                rtb.AggiungiRtf(intestazione + titoloNota + @"}\par}");
                            try
                            {
                                rtb.AggiungiRtf(testoNota);
                            }
                            catch
                            {
                                rtb.AppendText(testoNota);
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(rtb.Text))
                    testo = rtb.Rtf;
            }
            return testo;
        }

        private void MostraDefinizioniProgresso(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                ((BarraConEtichetta)(e.UserState)).Valore = e.ProgressPercentage;
            }
            catch (NullReferenceException) // succede quando programma è chiuso mentre thread in esecuzione
            {
                ((BackgroundWorker)sender).CancelAsync();
            }
            ((BarraConEtichetta)(e.UserState)).Ridisegna();
        }

        private void DefinizioniMostrate(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                return;

            ThreadDefinizioniArgomenti argomenti = (ThreadDefinizioniArgomenti)e.Result;

            if (!string.IsNullOrEmpty(argomenti.testo))
            {
                Editor fEditor = new Editor(this)
                {
                    MdiParent = this
                };
                fEditor.Show();
                fEditor.Text = Principale.testi.NormalizzaRiferimento(argomenti.brano) + " (" + argomenti.dizionario + ")";
                Application.DoEvents();
                fEditor.rtEditor.BloccaRtf(true);
                fEditor.rtEditor.Rtf = argomenti.testo;
                Application.DoEvents();
                if (fEditor.rtEditor.Text.Length < LUNGHEZZA_MASSIMA_PER_MOSTRARE_LINK) // cioè non quando il testo è così lungo, che ci vorrà troppo tempo per creare tutti i link
                    fEditor.rtEditor.MostraLink();
                fEditor.rtEditor.BloccaRtf(false);
                Application.DoEvents();
                fEditor.rtEditor.Versione = argomenti.dizionario;
                fEditor.rtEditor.Lingua = Principale.testi.Info(argomenti.versione).Lingua;
                fEditor.rtEditor.Modified = false;
            }

            (((ThreadDefinizioniArgomenti)e.Result).barra).MessaggioCompleto(Principale.LocRM.GetString("DefinitionsCompleted"));
            (((ThreadDefinizioniArgomenti)e.Result).barra).Chiudi();
        }

        #endregion

        #region ChiaveInEditor

        struct ThreadChiaveArgomenti
        {
            public string brano;
            public string versione;
            public bool diParole;
            public bool nonRadiciComuni;
            public string[] radiciComuni;
            public int ordine;
            public int numeroMinimo;
            public bool conRiferimenti;
            public string dizionario;
            public string testo; // il risultato
            public BarraConEtichetta barra;
        }

        internal void ChiaveInEditor(string brano, string versione, bool diParole, bool nonRadiciComuni, string[] radiciComuni, int ordine, int numeroMinimo, bool conRiferimenti, string dizionario)
        {
            int limiteProgresso = (diParole ? Principale.testi.Parole(versione).Length : Principale.testi.Radici(versione).Length);
            if (ordine != 0) // ordine per apparenze e prima apparenza devono passare 2 volte attraverso l'elenco di parole o radici
                limiteProgresso *= 2;

            ThreadChiaveArgomenti argomenti = new ThreadChiaveArgomenti
            {
                barra = CreaBarraDiStato(Principale.LocRM.GetString("ConcordanceCurrent"), 0, limiteProgresso),
                brano = brano,
                versione = versione,
                diParole = diParole,
                nonRadiciComuni = nonRadiciComuni,
                radiciComuni = radiciComuni,
                ordine = ordine,
                numeroMinimo = numeroMinimo,
                conRiferimenti = conRiferimenti,
                dizionario = dizionario
            };

            BackgroundWorker backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(CreaChiaveProgresso);

            // if (true)
            if (isRunningOnMono)
            {
                argomenti.testo = CreaTestoDiChiave(backgroundWorker, argomenti);
                ChiaveCreata(backgroundWorker, new RunWorkerCompletedEventArgs(argomenti, null, false));
            }
            else
            {
                backgroundWorker.DoWork += new DoWorkEventHandler(CreaChiave);
                backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ChiaveCreata);
                backgroundWorker.RunWorkerAsync(argomenti);
            }
        }

        private void CreaChiave(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            ThreadChiaveArgomenti argomenti = (ThreadChiaveArgomenti)e.Argument;

            argomenti.testo = CreaTestoDiChiave(worker, argomenti);
            e.Result = argomenti;

            if (worker.CancellationPending)
                e.Cancel = true;
        }

        static private string CreaTestoDiChiave(BackgroundWorker worker, ThreadChiaveArgomenti argomenti)
        {
            Riferimento branoDaMostrare = Principale.testi.ConvertiRiferimento(argomenti.brano);
            string versione = argomenti.versione;
            StringBuilder testo = new StringBuilder(Principale.testi.RtfIntestazione());

            if (argomenti.diParole)
            {
                int nParole = Principale.testi.Parole(versione).Length;

                string[] parole = new string[nParole];
                Array.Copy(Principale.testi.Parole(versione), parole, nParole);
                if (argomenti.nonRadiciComuni)
                {
                    Collection<string> paroleDiRadice = new Collection<string>();
                    foreach (string radiceComune in argomenti.radiciComuni)
                    {
                        paroleDiRadice = Principale.testi.ParoleDiRadice(radiceComune, versione);
                        foreach (string parolaDiRadice in paroleDiRadice)
                        {
                            if (Array.IndexOf(parole, parolaDiRadice) >= 0)
                                parole[Array.IndexOf(parole, parolaDiRadice)] = "";
                        }
                    }
                }

                if (argomenti.ordine == 1)
                {
                    ParolaApparenze[] paroleEApparenze = new ParolaApparenze[nParole];
                    string[] numeroApparenze = new string[nParole];
                    for (int i = 0; i < nParole; ++i)
                    {
                        paroleEApparenze[i].Parola = parole[i];
                        paroleEApparenze[i].Apparenze = Principale.testi.RicercaParolaInBrano(parole[i], branoDaMostrare, versione);
                        // è necessario fare il seguente trucco, affinché i numeri siano in ordine discendente ma le parole in ordine ascendente
                        numeroApparenze[i] = "0000000" + (9999999 - paroleEApparenze[i].Apparenze.Count).ToString(CultureInfo.InvariantCulture);
                        numeroApparenze[i] = numeroApparenze[i].Remove(0, numeroApparenze[i].Length - 7) + parole[i];
                        worker.ReportProgress(i, argomenti.barra);
                    }
                    Array.Sort(numeroApparenze, paroleEApparenze);
                    for (int i = 0; i < nParole; ++i)
                    {
                        testo.Append(RigaDiChiave(paroleEApparenze[i].Parola, paroleEApparenze[i].Apparenze, argomenti.numeroMinimo, argomenti.conRiferimenti, argomenti.dizionario, versione));
                        worker.ReportProgress(nParole + i, argomenti.barra);
                    }
                }
                else if (argomenti.ordine == 2)
                {
                    Riferimento[] apparenze = new Riferimento[nParole];
                    for (int i = 0; i < nParole; ++i)
                    {
                        apparenze[i] = Principale.testi.RicercaParolaInBrano(parole[i], branoDaMostrare, versione);
                        worker.ReportProgress(i, argomenti.barra);
                    }
                    Array.Sort(apparenze, parole, new Riferimento()); // new Riferimento() serve solo per usare Riferimento.Compare
                    for (int i = 0; i < nParole; ++i)
                    {
                        testo.Append(RigaDiChiave(parole[i], apparenze[i], argomenti.numeroMinimo, argomenti.conRiferimenti, argomenti.dizionario, versione));
                        worker.ReportProgress(nParole + i, argomenti.barra);
                    }
                }
                else // alfabetico
                {
                    Riferimento apparenze;
                    for (int i = 0; i < nParole; ++i)
                    {
                        apparenze = Principale.testi.RicercaParolaInBrano(parole[i], branoDaMostrare, versione);
                        testo.Append(RigaDiChiave(parole[i], apparenze, argomenti.numeroMinimo, argomenti.conRiferimenti, argomenti.dizionario, versione));
                        worker.ReportProgress(i, argomenti.barra);
                    }
                }
                testo.Append("}");
            }
            else // radici
            {
                int nRadici = Principale.testi.Radici(versione).Length;

                string[] radici = new string[nRadici];
                Array.Copy(Principale.testi.Radici(versione), radici, nRadici);
                if (argomenti.nonRadiciComuni)
                {
                    foreach (string radiceComune in argomenti.radiciComuni)
                    {
                        if (Array.IndexOf(radici, radiceComune) >= 0)
                            radici[Array.IndexOf(radici, radiceComune)] = "";
                    }
                }

                int primaRadiceDaRicercare = (radici[0] == "*" ? 1 : 0);

                if (argomenti.ordine == 1)
                {
                    ParolaApparenze[] radiciEApparenze = new ParolaApparenze[nRadici];
                    string[] numeroApparenze = new string[nRadici];
                    if (primaRadiceDaRicercare == 1)
                    {
                        radiciEApparenze[0].Parola = "*";
                        radiciEApparenze[0].Apparenze = new Riferimento();
                        numeroApparenze[0] = "9999999*";
                    }
                    for (int i = primaRadiceDaRicercare; i < nRadici; ++i)
                    {
                        radiciEApparenze[i].Parola = radici[i];
                        radiciEApparenze[i].Apparenze = Principale.testi.RicercaRadiceInBrano(radici[i], branoDaMostrare, versione);
                        // è necessario fare il seguente trucco, affinché i numeri siano in ordine discendente ma le parole in ordine ascendente
                        numeroApparenze[i] = "0000000" + (9999999 - radiciEApparenze[i].Apparenze.Count).ToString(CultureInfo.InvariantCulture);
                        numeroApparenze[i] = numeroApparenze[i].Remove(0, numeroApparenze[i].Length - 7) + radici[i];
                        worker.ReportProgress(i, argomenti.barra);
                    }
                    Array.Sort(numeroApparenze, radiciEApparenze);
                    for (int i = 0; i < nRadici; ++i)
                    {
                        testo.Append(RigaDiChiave(radiciEApparenze[i].Parola, radiciEApparenze[i].Apparenze, argomenti.numeroMinimo, argomenti.conRiferimenti, argomenti.dizionario, ""));
                        worker.ReportProgress(nRadici + i, argomenti.barra);
                    }
                }
                else if (argomenti.ordine == 2)
                {
                    Riferimento[] apparenze = new Riferimento[nRadici];
                    if (primaRadiceDaRicercare == 1)
                        apparenze[0] = new Riferimento();
                    for (int i = primaRadiceDaRicercare; i < nRadici; ++i)
                    {
                        apparenze[i] = Principale.testi.RicercaRadiceInBrano(radici[i], branoDaMostrare, versione);
                        worker.ReportProgress(i, argomenti.barra);
                    }
                    Array.Sort(apparenze, radici, new Riferimento()); // new Riferimento() serve solo per usare Riferimento.Compare
                    for (int i = 0; i < nRadici; ++i)
                    {
                        testo.Append(RigaDiChiave(radici[i], apparenze[i], argomenti.numeroMinimo, argomenti.conRiferimenti, argomenti.dizionario, ""));
                        worker.ReportProgress(nRadici + i, argomenti.barra);
                    }
                }
                else // alfabetico
                {
                    Riferimento apparenze;
                    for (int i = primaRadiceDaRicercare; i < nRadici; ++i)
                    {
                        apparenze = Principale.testi.RicercaRadiceInBrano(radici[i], branoDaMostrare, versione);
                        testo.Append(RigaDiChiave(radici[i], apparenze, argomenti.numeroMinimo, argomenti.conRiferimenti, argomenti.dizionario, ""));
                        worker.ReportProgress(i, argomenti.barra);
                    }
                }
                testo.Append("}");
            }
            return testo.ToString();
        }

        private void CreaChiaveProgresso(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                ((BarraConEtichetta)(e.UserState)).Valore = e.ProgressPercentage;
            }
            catch (NullReferenceException) // succede quando programma è chiuso mentre thread in esecuzione
            {
                ((BackgroundWorker)sender).CancelAsync();
            }
            ((BarraConEtichetta)(e.UserState)).Ridisegna();
        }

        private static string RigaDiChiave(string parola, Riferimento apparenze, int numeroMinimo, bool conRiferimenti, string dizionario, string versione)
        {
            if (apparenze.Count < numeroMinimo)
                return "";
            StringBuilder riga = new StringBuilder(parola);
            if (conRiferimenti)
            {
                riga.Append(" (").Append(apparenze.Count.ToString(CultureInfo.CurrentCulture)).Append("): ");
                if (apparenze.Versetti)
                {
                    string[] separatori = Principale.testi.SeparatoriNeiRiferimenti();
                    foreach (byte[] brano in apparenze.Brani)
                        riga.Append(Principale.testi.GetLibroAbbreviazioneUsata(brano[0])).Append(separatori[0]).Append(brano[1].ToString(CultureInfo.InvariantCulture)).Append(separatori[1]).Append(brano[2].ToString(CultureInfo.InvariantCulture)).Append(", ");
                }
                else
                {
                    foreach (string nota in apparenze.Note)
                    {
                        if (nota[0] == '#') // nota su un brano
                            riga.Append(Principale.testi.ConvertiTitoloNotaARiferimento(nota)).Append(", ");
                        else // nota con un titolo
                            riga.Append(nota).Append(", ");
                    }
                }
            }
            else
            {
                riga.Append(": ").Append(apparenze.Count.ToString(CultureInfo.CurrentCulture));
            }
            string rigaStringa = riga.ToString();
            if (rigaStringa.Substring(rigaStringa.Length - 2) == ", ")
                rigaStringa = rigaStringa.Remove(rigaStringa.Length - 2);

            if (!string.IsNullOrEmpty(dizionario))
            {
                string testo = testi.GetNotaTesto(parola, dizionario);
                if (string.IsNullOrEmpty(testo) && !string.IsNullOrEmpty(versione))
                    testo = testi.GetNotaTesto(testi.RadiceDiParola(parola, versione), dizionario);
                if (!string.IsNullOrEmpty(testo))
                    rigaStringa += @"\par " + testo;
            }
            return rigaStringa + @"\par ";
        }

        private void ChiaveCreata(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                return;

            string testo = ((ThreadChiaveArgomenti)e.Result).testo;
            string versione = ((ThreadChiaveArgomenti)e.Result).versione;
            string brano = ((ThreadChiaveArgomenti)e.Result).brano;

            Editor fEditor;
            if (Settings.Default.OpzioniStessaFinestra && finestraRisultati != null)
            {
                fEditor = finestraRisultati;
            }
            else
            {
                fEditor = new Editor(this)
                {
                    MdiParent = this
                };
                fEditor.Show();
            }

            string titoloFinestra = Principale.LocRM.GetString("ConcordanceTitle") + " ";
            if (string.IsNullOrEmpty(brano))
                titoloFinestra += Principale.testi.Info(versione).Abbreviazione;
            else
                titoloFinestra += brano + " (" + Principale.testi.Info(versione).Abbreviazione + ") ";
            fEditor.Text = titoloFinestra;
            fEditor.VersionePerIpertesto = versione;
            Application.DoEvents();
            fEditor.rtEditor.BloccaRtf(true);
            fEditor.rtEditor.Rtf = testo;
            Application.DoEvents();
            fEditor.rtEditor.MostraLink();
            fEditor.rtEditor.BloccaRtf(false);
            Application.DoEvents();
            fEditor.rtEditor.Versione = versione;
            fEditor.rtEditor.Lingua = Principale.testi.Info(versione).Lingua;
            fEditor.rtEditor.Modified = false;
            if (Settings.Default.OpzioniStessaFinestra)
            {
                finestraRisultati = fEditor;
                fEditor.Activate();
            }

            (((ThreadChiaveArgomenti)e.Result).barra).MessaggioCompleto(Principale.LocRM.GetString("ConcordanceCompleted"));
            (((ThreadChiaveArgomenti)e.Result).barra).Chiudi();
            //            SetBarraDiStatoTesto(Principale.LocRM.GetString("ConcordanceCompleted"));
        }

        #endregion

        #region BraniSimili

        struct ThreadSimiliArgomenti
        {
            public string brano;
            public string versione;
            public bool versetti;
            public int numeroMinimo;
            public string testo; // il risultato
            public BarraConEtichetta barra;
        }

        private void TrovaBraniSimili(string brano, string versione)
        {
            TrovaBraniSimili(brano, versione, Settings.Default.SimiliVersetti, Settings.Default.SimiliNumeroMassimo);
        }

        internal void TrovaBraniSimili(string brano, string versione, bool versetti, int numeroMassimo)
        {
            int limiteProgresso = Principale.testi.Radici(versione).Length;

            ThreadSimiliArgomenti argomenti = new ThreadSimiliArgomenti
            {
                barra = CreaBarraDiStato(Principale.LocRM.GetString("SimilarCurrent"), 0, limiteProgresso),
                brano = brano,
                versione = versione,
                versetti = versetti,
                numeroMinimo = numeroMassimo
            };

            BackgroundWorker backgroundWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(MostraSimiliProgresso);

            // if (true)
            if (isRunningOnMono)
            {
                argomenti.testo = TestoSimili(backgroundWorker, argomenti);
                SimiliMostrati(backgroundWorker, new RunWorkerCompletedEventArgs(argomenti, null, false));
            }
            else
            {
                backgroundWorker.DoWork += new DoWorkEventHandler(MostraSimili);
                backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(SimiliMostrati);
                backgroundWorker.RunWorkerAsync(argomenti);
            }
        }

        private void MostraSimili(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            ThreadSimiliArgomenti argomenti = (ThreadSimiliArgomenti)e.Argument;

            argomenti.testo = TestoSimili(worker, argomenti);
            e.Result = argomenti;

            if (worker.CancellationPending)
                e.Cancel = true;
        }

        static private string TestoSimili(BackgroundWorker worker, ThreadSimiliArgomenti argomenti)
        {
            string versione = argomenti.versione;
            string brano = argomenti.brano;

            string[] radici = Principale.testi.Radici(versione);
            int nRadici = radici.Length;

            double[, ,] punteggio = new double[74, 151, 177];
            Riferimento apparenze;
            Riferimento apparenzeInBrano;
            Riferimento riferimentoBrano = Principale.testi.ConvertiRiferimento(brano);
            int nVolteInBrano;
            double fattore, logNumeroVersetti = Math.Log(Principale.testi.VersettiFinoACapitolo(73, 22, versione) + Principale.testi.VersettiInCapitolo(73, 22, versione));
            List<double> listaPunteggi = new List<double>();
            List<uint> listaNumeroVersetti = new List<uint>();

            // prima di tutto, controlliamo che il riferimento esiste in questa versione, cioè almeno uno dei libri
            bool riferimentoEsiste = false;
            foreach (byte[] branoDaControllare in riferimentoBrano.Brani)
            {
                for (byte i = branoDaControllare[0]; i <= branoDaControllare[3]; ++i)
                {
                    if (Principale.testi.CapitoliInLibro(i, versione) > 0)
                    {
                        riferimentoEsiste = true;
                        break;
                    }
                }
                if (riferimentoEsiste)
                    break;
            }

            if (riferimentoEsiste)
            {
                // usiamo il metodo tf-idf
                // http://en.wikipedia.org/wiki/Tf%E2%80%93idf
                // con qualche modifica
                // dovrebbe essere
                // somma (numero volte in brano base)/(numero totale parole in quel brano)*(numero volte in quel brano) * log(numero versetti/numero versetti con parola)
                for (int i = 0; i < nRadici; ++i)
                {
                    if (radici[i] != "*")
                    {
                        apparenzeInBrano = Principale.testi.RicercaRadiceInBrano(radici[i], riferimentoBrano, versione);
                        nVolteInBrano = apparenzeInBrano.Count;
                        if (nVolteInBrano > 0)
                        {
                            apparenze = Principale.testi.RicercaRadiceInBrano(radici[i], versione);
                            apparenze.RimuoviVersetti(apparenzeInBrano);
                            if (apparenze.Count > 0)
                            {
                                //fattore = (float)nVolteInBrano / apparenze.Count; - un modo alternativo
                                fattore = nVolteInBrano * (logNumeroVersetti - Math.Log(apparenze.Count));
                                foreach (byte[] b in apparenze.Brani)
                                {
                                    if (argomenti.versetti)
                                        punteggio[b[0], b[1], b[2]] += fattore;
                                    else
                                        punteggio[b[0], b[1], 0] += fattore;
                                }
                            }
                        }
                    }
                    worker.ReportProgress(i, argomenti.barra);
                }

                if (argomenti.versetti)
                {
                    for (byte i = 1; i <= 73; ++i)
                    {
                        for (byte j = 0; j <= 149; ++j) // necessario fare in questo modo, perché da 1 a 150, byte-1 non funziona
                        {
                            for (byte k = 1; k <= 176; ++k)
                            {
                                if (punteggio[i, j + 1, k] > 0)
                                {
                                    listaPunteggi.Add(punteggio[i, j + 1, k]); // bisogna anche dividere per numero di parole in versetto, ma il programma non sa questo numero
                                    listaNumeroVersetti.Add(Principale.testi.VersettiFinoACapitolo(i, j, versione) + k);
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (byte i = 1; i <= 73; ++i) // necessario fare in questo modo, perché da 1 a 73, byte-1 non funziona
                    {
                        for (byte j = 1; j <= 150; ++j)
                        {
                            if (punteggio[i, j, 0] > 0)
                            {
                                listaPunteggi.Add(punteggio[i, j, 0] / Principale.testi.VersettiInCapitolo(i, j, versione));
                                // dovrebbe essere "/ (parole in capitoli)", ma il programma non sa questo numero
                                listaNumeroVersetti.Add((uint)(Principale.testi.CapitoliFinoALibro((byte)(i - 1), versione) + j));
                            }
                        }
                    }
                }
            }
            else
            {
                worker.ReportProgress(nRadici - 1, argomenti.barra);
            }

            double[] listaPunteggiOrdinati = listaPunteggi.ToArray();
            uint[] listaNumeroVersettiOrdinati = listaNumeroVersetti.ToArray();
            Array.Sort(listaPunteggiOrdinati, listaNumeroVersettiOrdinati);

            Riferimento braniSimili = new Riferimento();
            int massimoBrani = argomenti.numeroMinimo;
            if (listaNumeroVersettiOrdinati.Length < massimoBrani)
                massimoBrani = listaNumeroVersettiOrdinati.Length;
            for (int i = 0; i < massimoBrani; ++i)
            {
                if (argomenti.versetti)
                    braniSimili.AggiungiBraniDaRiferimento(Principale.testi.RiferimentoDiVersetto((int)(listaNumeroVersettiOrdinati[listaNumeroVersettiOrdinati.Length - 1 - i]), versione));
                else
                    braniSimili.AggiungiBraniDaRiferimento(Principale.testi.RiferimentoDiCapitolo((int)(listaNumeroVersettiOrdinati[listaNumeroVersettiOrdinati.Length - 1 - i]), versione));
            }

            return Principale.testi.TestoBrano(braniSimili, versione);
        }

        private void MostraSimiliProgresso(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                ((BarraConEtichetta)(e.UserState)).Valore = e.ProgressPercentage;
            }
            catch (NullReferenceException) // succede quando programma è chiuso mentre thread in esecuzione
            {
                ((BackgroundWorker)sender).CancelAsync();
            }
            ((BarraConEtichetta)(e.UserState)).Ridisegna();
        }

        private void SimiliMostrati(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
                return;

            ThreadSimiliArgomenti argomenti = (ThreadSimiliArgomenti)e.Result;

            if (!string.IsNullOrEmpty(argomenti.testo))
            {
                Editor fEditor;
                if (Settings.Default.OpzioniStessaFinestra && finestraRisultati != null)
                {
                    fEditor = finestraRisultati;
                }
                else
                {
                    fEditor = new Editor(this)
                    {
                        MdiParent = this
                    };
                    fEditor.Show();
                }
                fEditor.Text = Principale.LocRM.GetString("SimilarTitle") + Principale.testi.NormalizzaRiferimento(argomenti.brano) + " (" + argomenti.versione + ")";
                Application.DoEvents();
                fEditor.rtEditor.BloccaRtf(true);
                fEditor.rtEditor.Rtf = argomenti.testo;
                Application.DoEvents();
                fEditor.rtEditor.BloccaRtf(false);
                Application.DoEvents();
                fEditor.rtEditor.Versione = argomenti.versione;
                fEditor.rtEditor.Lingua = Principale.testi.Info(argomenti.versione).Lingua;
                fEditor.rtEditor.Modified = false;
                if (Settings.Default.OpzioniStessaFinestra)
                {
                    finestraRisultati = fEditor;
                    fEditor.Activate();
                }
            }

            (((ThreadSimiliArgomenti)e.Result).barra).MessaggioCompleto(Principale.LocRM.GetString("SimilarCompleted"));
            (((ThreadSimiliArgomenti)e.Result).barra).Chiudi();
        }

        #endregion

        #endregion

        internal string NomeFileGuida()
        {
            return fileGuida.HelpNamespace;
        }

    }
}
