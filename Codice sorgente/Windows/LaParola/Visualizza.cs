using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using LaParola.Properties;
using TestiBiblici;

// TODO (C) guida
// TODO (C) riga di comando: view b1, b2, b3, ...; view {set of texts}; ditto confronta

namespace LaParola
{
    public partial class Visualizza : Template
    {
        #region Pane

        public class Pane : IDisposable
        {
            #region Proprietà

            private Panel panComponenti;
            private RichTextBoxHighlight rtfTesto;
            private VScrollBar sbRtf;
            private Button pulRimuovi;
            private Label etiAbbreviazione;
            private Button pulSinc;
            private Button pulNote;
            private ContextMenuStrip pmCollezioni;
            private ContextMenuStrip pmTesto;
            private ToolStripMenuItem copyToolStripMenuItem;
            private ToolStripMenuItem printToolStripMenuItem;
            private ToolStripSeparator popupToolStripSeparatorGeneralWord;
            private ToolStripMenuItem informationOnWordToolStripMenuItem;
            private ToolStripMenuItem searchToolStripMenuItem;
            private ToolStripMenuItem searchWordToolStripMenuItem;
            private ToolStripMenuItem searchRadiceToolStripMenuItem;
            private ToolStripMenuItem searchSelectionToolStripMenuItem;
            private ToolStripMenuItem noteOnWordToolStripMenuItem;
            private ToolStripSeparator popupToolStripSeparatorWordVerse;
            private ToolStripMenuItem informationOnVerseToolStripMenuItem;
            private ToolStripMenuItem bookmarkVerseToolStripMenuItem;
            private ToolStripMenuItem noteOnVerseToolStripMenuItem;
            private Font font = null;

            public RichTextBoxHighlight Rtf
            {
                get { return rtfTesto; }
            }

            //private string versione;
            public string Versione
            {
                get { return rtfTesto.Versione; }
                set { if (rtfTesto != null) rtfTesto.Versione = value; }
            }

            //private byte libro;
            public byte Libro
            {
                get { return rtfTesto.Libro; }
                set { rtfTesto.Libro = value; }
            }

            //private byte capitolo;
            public byte Capitolo
            {
                get { return rtfTesto.Capitolo; }
                set { rtfTesto.Capitolo = value; }
            }

            //private byte versetto;
            public byte Versetto
            {
                get { return rtfTesto.Versetto; }
                set { rtfTesto.Versetto = value; }
            }

            //private string voce;
            public string Voce
            {
                get { return rtfTesto.Voce; }
                set { rtfTesto.Voce = value; }
            }

            private string postoAttuale;
            public string PostoAttuale
            {
                get { return postoAttuale; }
                set { postoAttuale = value; }
            }

            public int ScrollBarValore
            {
                get { return sbRtf.Value; }
                set
                {
                    if (value < sbRtf.Minimum)
                        sbRtf.Value = sbRtf.Minimum;
                    else if (value > sbRtf.Maximum)
                        sbRtf.Value = sbRtf.Maximum;
                    else
                        sbRtf.Value = value;
                }
            }

            private Riferimento paroleRicercate = new Riferimento();
            public Riferimento ParoleRicercate
            {
                get { return paroleRicercate; }
                set { paroleRicercate = value; }
            }

            public int SincNumero
            {
                get
                {
                    string s = pulSinc.Text.Substring(1);
                    return (s == "X" ? 0 : Convert.ToInt32(s, CultureInfo.InvariantCulture));
                }
                set
                {
                    pulSinc.Text = (value == 0 ? "&X" : "&" + value.ToString(CultureInfo.InvariantCulture));
                }
            }

            public bool RimuoviVisibile
            {
                get { return pulRimuovi.Visible; }
                set { pulRimuovi.Visible = value; }
            }

            public float Zoom
            {
                get { return rtfTesto.ZoomFactor; }
                set { rtfTesto.ZoomFactor = value; }
            }

            public int SelectionLength
            {
                get { return rtfTesto.SelectionLength; }
            }

            bool tipoConfronta;
            public bool TipoConfronta
            {
                get { return tipoConfronta; }
                set { tipoConfronta = value; }
            }

            private readonly TestoTipi tipoTesto;
            public TestoTipi TipoTesto
            {
                get { return tipoTesto; }
            }

            private readonly bool tuttiTesti = false;
            public TestoTipi TuttiTesti
            {
                get { return tuttiTesti ? tipoTesto : TestoTipi.None; }
            }

            private bool spostando = false;
            private int posizioneUltimoClic = -1;
            private Rectangle dragBoxFromMouseDown;
            private bool ctrlPremuto = false;

            private Visualizza genitore;

            #endregion

            #region Costruttori

            public Pane(Visualizza sender, TestoTipi tipo, int larghezza)
            {
                CreaComponenti(sender, larghezza);

                tipoTesto = tipo;
                tuttiTesti = true;
                Versione = "";

                CostruttoreComune(sender, larghezza);

                sbRtf.Visible = false;
                rtfTesto.ScrollBars = RichTextBoxScrollBars.Vertical;
            }

            public Pane(Visualizza sender, string nomeVersione, TestoTipi tipo, int larghezza, bool confronta)
            {
                // TODO (C) confronta
                tipoTesto = tipo;
                tipoConfronta = confronta;

                CreaComponenti(sender, larghezza);

                Versione = nomeVersione;

                CostruttoreComune(sender, larghezza);

                rtfTesto.Versione = Versione;
                rtfTesto.Lingua = Principale.testi.Info(Versione).Lingua;

                if (confronta)
                {
                    pulSinc.Visible = false;
                    pulNote.Visible = false;
                }

                if (tipo == TestoTipi.Bibbia)
                {
                    rtfTesto.MouseWheel += new MouseEventHandler(RtfTesto_MouseWheel);

                    try
                    {
                        sbRtf.Maximum = Principale.testi.CapitoliFinoALibro(73, Versione) + Principale.testi.CapitoliInLibro(73, Versione);
                        for (byte i = 1; i <= 73; ++i)
                        {
                            if (Principale.testi.CapitoliInLibro(i, Versione) > 0)
                            {
                                Libro = i;
                                break;
                            }
                        }
                    }
                    catch { }
                }
                else
                {
                    sbRtf.Visible = false;
                    rtfTesto.ScrollBars = RichTextBoxScrollBars.Vertical;
                    rtfTesto.Modified = false;
                }
            }

            private void CostruttoreComune(Visualizza sender, int larghezza)
            {
                genitore = sender;

                if (Principale.isRunningOnMono)
                {
                    sbRtf.Visible = false;
                    printToolStripMenuItem.Visible = false;
                }

                FontStyle fs = FontStyle.Regular;
                if (Principale.testi.Formato.FontGrassetto)
                    fs &= FontStyle.Bold;
                if (Principale.testi.Formato.FontCorsivo)
                    fs &= FontStyle.Italic;
                if (Principale.testi.Formato.FontSottolineato)
                    fs &= FontStyle.Underline;
                try
                {
                    font = new Font(Principale.testi.Formato.FontNome, Principale.testi.Formato.FontDimensione, fs);
                }
                catch (ArgumentException)
                { // con un computer, la riga precedente ha dato un errore che Palatino Linotype non ha lo stile normale. Proviamo in altri modi...
                    try
                    {
                        font = new Font(Principale.testi.Formato.FontNome, Principale.testi.Formato.FontDimensione);
                    }
                    catch (ArgumentException)
                    {
                    }
                }
                if (font != null)
                    rtfTesto.Font = font;

                rtfTesto.AggiungiHighlightDaFile(); // deve essere prima di SpostaTesto

                Libro = 1;
                if (tipoTesto == TestoTipi.Bibbia && !tuttiTesti)
                {
                    AggiornaMenuCollezioni(); // deve essere fatto prima di mostrare il testo
                    // cerca il primo libro con testo
                    while (Libro <= 73 && Principale.testi.CapitoliInLibro(Libro, Versione) == 0)
                        ++Libro;
                }
                if (Libro == 74)
                    Libro = 1;

                sender.genitore.aggiornaCronologia = false;
                if (tipoTesto != TestoTipi.Dizionario)
                    SpostaTesto(Libro, 1, 1, false);
                else
                {
                    try
                    {
                        string notaDaAprire = "";
                        if (!tuttiTesti)
                        {
                            Collection<string> noteOrdinate = Principale.testi.GetNoteInOrdine(Versione);
                            if (noteOrdinate.Count > 0)
                                notaDaAprire = noteOrdinate[0]; // l'indice (se esiste)
                            if (string.IsNullOrEmpty(notaDaAprire) && noteOrdinate.Count > 1)
                                notaDaAprire = noteOrdinate[1]; // la prima nota in ordine
                            if (string.IsNullOrEmpty(notaDaAprire))
                                notaDaAprire = Principale.testi.NoteConTitolo(Versione)[0]; // può dare exception
                        }
                        SpostaTesto(notaDaAprire, false);
                    }
                    catch
                    {
                        // per esempio nessuna nota su un tema trovata
                    }
                }
                sender.genitore.aggiornaCronologia = true;

                rtfTesto.Modified = false;
                rtfTesto.AllowDrop = true;

                if (sender.panes.Count > 0)
                    sender.panes[0].NuovoPrimoPane();
                else
                    NuovoPrimoPane();

                // per controllare i riferimenti diversi

                if (Principale.testi.Info(Versione).Tipo == TestoTipi.Bibbia)
                {
                    int numeroCapitolo = 1;
                    string[] capInLibri = new string[74 + Principale.testi.CapitoliFinoALibro(73, Versione)];
                    for (byte i = 1; i <= 73; ++i)
                        capInLibri[i] = Principale.testi.CapitoliInLibro(i, Versione).ToString(CultureInfo.InvariantCulture);
                    for (byte i = 1; i <= 73; ++i)
                    {
                        for (byte j = 1; j <= Principale.testi.CapitoliInLibro(i, Versione); ++j)
                        {
                            capInLibri[73 + numeroCapitolo] = Principale.testi.VersettiInCapitolo(i, j, Versione).ToString(CultureInfo.InvariantCulture);
                            ++numeroCapitolo;
                        }
                    }
                    //System.IO.File.WriteAllLines(@"d:\Documenti\Visual Studio 2008\Projects\laparola\altri prog\riferimenti diversi\" + Versione + ".txt", capInLibri);
                }
            }

            private void CreaComponenti(Visualizza sender, int larghezza)
            {
                panComponenti = new Panel();
                rtfTesto = new RichTextBoxHighlight();
                sbRtf = new VScrollBar();
                pulSinc = new Button();
                pulNote = new Button();
                pulRimuovi = new Button();
                etiAbbreviazione = new Label();
                pmCollezioni = new ContextMenuStrip();
                pmTesto = new ContextMenuStrip();
                copyToolStripMenuItem = new ToolStripMenuItem(Principale.LocRM.GetString("BrowsePopupCopy"), global::LaParola.Properties.Resources.copia, CopyToolStripMenuItem_Click);
                printToolStripMenuItem = new ToolStripMenuItem(Principale.LocRM.GetString("BrowsePopupPrint"), global::LaParola.Properties.Resources.stampa, PrintToolStripMenuItem_Click);
                popupToolStripSeparatorGeneralWord = new ToolStripSeparator();
                informationOnWordToolStripMenuItem = new ToolStripMenuItem(Principale.LocRM.GetString("BrowsePopupInfoWord"), global::LaParola.Properties.Resources.info, InformationToolStripMenuItem_Click)
                {
                    Tag = ""
                };
                searchWordToolStripMenuItem = new ToolStripMenuItem("", null, SearchToolStripMenuItem_Click);
                searchRadiceToolStripMenuItem = new ToolStripMenuItem("", null, SearchToolStripMenuItem_Click);
                searchSelectionToolStripMenuItem = new ToolStripMenuItem("", null, SearchToolStripMenuItem_Click);
                searchToolStripMenuItem = new ToolStripMenuItem(Principale.LocRM.GetString("BrowsePopupSearch"), global::LaParola.Properties.Resources.ricerca);
                searchToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            searchWordToolStripMenuItem,
            searchRadiceToolStripMenuItem,
            searchSelectionToolStripMenuItem});
                noteOnWordToolStripMenuItem = new ToolStripMenuItem(Principale.LocRM.GetString("BrowsePopupNoteWord"), global::LaParola.Properties.Resources.aprinota)
                {
                    Tag = ""
                };
                popupToolStripSeparatorWordVerse = new ToolStripSeparator();
                informationOnVerseToolStripMenuItem = new ToolStripMenuItem(Principale.LocRM.GetString("BrowsePopupInfoVerse"), global::LaParola.Properties.Resources.info, InformationToolStripMenuItem_Click)
                {
                    Tag = ""
                };
                bookmarkVerseToolStripMenuItem = new ToolStripMenuItem(Principale.LocRM.GetString("BrowsePopupBookmark"), global::LaParola.Properties.Resources.segnalibri, BookmarkVerseToolStripMenuItem_Click)
                {
                    Tag = ""
                };
                noteOnVerseToolStripMenuItem = new ToolStripMenuItem(Principale.LocRM.GetString("BrowsePopupNoteVerse"), global::LaParola.Properties.Resources.aprinota)
                {
                    Tag = ""
                };

                //                panComponenti.Location = new Point(sinistra, 0);
                panComponenti.Size = new Size(larghezza, sender.ClientSize.Height);
                panComponenti.Tag = this;
                panComponenti.Resize += new EventHandler(PanComponenti_Resize);

                rtfTesto.BackColor = Color.FromName("Window");
                rtfTesto.Location = new Point(0, 0);
                rtfTesto.ReadOnly = true;
                rtfTesto.ScrollBars = RichTextBoxScrollBars.None;
                rtfTesto.TabIndex = 0;
                rtfTesto.KeyDown += new KeyEventHandler(RtfTesto_KeyDown);
                rtfTesto.KeyUp += new KeyEventHandler(RtfTesto_KeyUp);
                rtfTesto.LinkClicked += new LinkClickedEventHandler(RtfTesto_LinkClicked);
                rtfTesto.MouseDoubleClick += new MouseEventHandler(RtfTesto_MouseDoubleClick);
                rtfTesto.MouseDown += new MouseEventHandler(RtfTesto_MouseDown);
                rtfTesto.MouseHover += new EventHandler(RtfTesto_MouseHover);
                rtfTesto.MouseMove += new MouseEventHandler(RtfTesto_MouseMove);
                rtfTesto.MouseUp += new MouseEventHandler(RtfTesto_MouseUp);
                rtfTesto.MouseWheel += new MouseEventHandler(RtfTestoZoom_MouseWheel);
                rtfTesto.LinkHoverEvento += new EventHandler<LinkHoverEventArgs>(RtfTesto_LinkHover);
                rtfTesto.HighlightChangedEvent += new EventHandler<RichTextBoxHighlight.HighlightChangedEventArgs>(RtfTesto_HighlightChangedEvent);
                rtfTesto.SelectionChanged += new EventHandler(RtfTesto_SelectionChanged);
                rtfTesto.Enter += new EventHandler(Controllo_Enter);

                sbRtf.Visible = (tipoTesto == TestoTipi.Bibbia);
                sbRtf.LargeChange = 1;
                sbRtf.Maximum = 1;
                sbRtf.Minimum = 1;
                sbRtf.TabIndex = 1;
                sbRtf.Value = 1;
                sbRtf.Scroll += new ScrollEventHandler(ScrollBar_Scroll);
                sbRtf.Enter += new EventHandler(Controllo_Enter);

                pulSinc.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
                pulSinc.Image = global::LaParola.Properties.Resources.collegamento;
                pulSinc.ImageAlign = ContentAlignment.MiddleLeft;
                pulSinc.Location = new Point(2, panComponenti.Height - 35);
                pulSinc.Size = new Size(37, 23);
                pulSinc.TabIndex = 2;
                pulSinc.Text = "&X";
                pulSinc.TextAlign = ContentAlignment.MiddleRight;
                pulSinc.Click += new EventHandler(PulSinc_Click);
                pulSinc.Enter += new EventHandler(Controllo_Enter);

                pulRimuovi.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
                pulRimuovi.Image = global::LaParola.Properties.Resources.rimuovi;
                pulRimuovi.ImageAlign = ContentAlignment.MiddleLeft;
                pulRimuovi.Location = new Point(42, panComponenti.Height - 35);
                pulRimuovi.Size = new Size(37, 23);
                pulRimuovi.TabIndex = 3;
                pulRimuovi.Text = "&R";
                pulRimuovi.TextAlign = ContentAlignment.MiddleRight;
                pulRimuovi.Visible = false;
                pulRimuovi.Click += new EventHandler(sender.PulRimuovi_Click);
                pulRimuovi.Enter += new EventHandler(Controllo_Enter);

                pulNote.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
                pulNote.Image = global::LaParola.Properties.Resources.visnota;
                pulNote.ImageAlign = ContentAlignment.MiddleLeft;
                pulNote.Location = new Point(82, panComponenti.Height - 35);
                pulNote.Size = new Size(37, 23);
                pulNote.TabIndex = 4;
                pulNote.Text = "&N";
                pulNote.TextAlign = ContentAlignment.MiddleRight;
                pulNote.Visible = false;
                pulNote.Click += new EventHandler(PulNote_Click);
                pulNote.Enter += new EventHandler(Controllo_Enter);

                etiAbbreviazione.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left);
                etiAbbreviazione.Location = new Point(118, panComponenti.Height - 35);
                etiAbbreviazione.Height = pulRimuovi.Height;
                etiAbbreviazione.TabIndex = 5;
                etiAbbreviazione.Text = Principale.testi.Info(Versione).Abbreviazione;
                etiAbbreviazione.TextAlign = ContentAlignment.MiddleLeft;
                etiAbbreviazione.Enter += new EventHandler(Controllo_Enter);

                pmCollezioni.ShowCheckMargin = true;
                pmCollezioni.ShowImageMargin = true;

                pmTesto.Items.AddRange(new ToolStripItem[] {copyToolStripMenuItem,
            printToolStripMenuItem,
            popupToolStripSeparatorGeneralWord,
            informationOnWordToolStripMenuItem,
            searchToolStripMenuItem,
            noteOnWordToolStripMenuItem,
            popupToolStripSeparatorWordVerse,
            informationOnVerseToolStripMenuItem,
            bookmarkVerseToolStripMenuItem,
            noteOnVerseToolStripMenuItem});

                pmTesto.Opening += PmTesto_Opening;
                rtfTesto.ContextMenuStrip = pmTesto;

                panComponenti.Controls.Add(rtfTesto);
                panComponenti.Controls.Add(sbRtf);
                panComponenti.Controls.Add(pulSinc);
                panComponenti.Controls.Add(pulNote);
                panComponenti.Controls.Add(pulRimuovi);
                panComponenti.Controls.Add(etiAbbreviazione);

                RidimensionaComponenti();

                sender.panPanes.Controls.Add(panComponenti);
                panComponenti.Dock = DockStyle.Right;
            }

            #endregion

            #region Chiusura

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                font?.Dispose();
                noteOnWordToolStripMenuItem?.Dispose();
                noteOnVerseToolStripMenuItem?.Dispose();
                etiAbbreviazione?.Dispose();
                popupToolStripSeparatorGeneralWord?.Dispose();
                popupToolStripSeparatorWordVerse?.Dispose();
                informationOnWordToolStripMenuItem?.Dispose();
                informationOnVerseToolStripMenuItem?.Dispose();
                searchWordToolStripMenuItem?.Dispose();
                searchSelectionToolStripMenuItem?.Dispose();
                searchToolStripMenuItem?.Dispose();
                searchRadiceToolStripMenuItem?.Dispose();
                copyToolStripMenuItem?.Dispose();
                bookmarkVerseToolStripMenuItem?.Dispose();
                printToolStripMenuItem?.Dispose();
                sbRtf?.Dispose();
                rtfTesto?.Dispose();
                pulRimuovi?.Dispose();
                pulNote?.Dispose();
                pulSinc?.Dispose();
                pmTesto?.Dispose();
                pmCollezioni?.Dispose();
                panComponenti?.Dispose();
            }

            #endregion

            #region Metodi

            internal void Rimuovi()
            {
                genitore.panPanes.Controls.Remove(panComponenti);
                genitore.panes[0].NuovoPrimoPane();
            }

            internal Size Size
            {
                get { return panComponenti.Size; }
                set { panComponenti.Size = value; }
            }

            internal Point Location
            {
                get { return panComponenti.Location; }
                set { panComponenti.Location = value; }
            }

            internal void ImpostaPulsantiVisibili(bool visibili)
            {
                pulNote.Visible = visibili;
                pulRimuovi.Visible = visibili;
                pulSinc.Visible = visibili;
            }

            internal void SetTab(int tab)
            {
                panComponenti.TabIndex = tab;
            }

            private void NuovoPrimoPane()
            {
                panComponenti.Dock = DockStyle.Fill;
            }

            internal void RidimensionaComponenti()
            {
                PanComponenti_Resize(null, null);
            }

            internal void StampaSelezione()
            {
                int len = rtfTesto.SelectionStart + rtfTesto.SelectionLength - 1;
                // in Light è necessario usare una variable intermedia len; usiamo anche qui per essere sicuri
                genitore.genitore.StampaRichText(rtfTesto, rtfTesto.SelectionStart, len);
            }

            internal void CopiaSelezione()
            {
                rtfTesto.CopiaSenzaTestoNascosto();
            }

            internal void AggiornaMenuCollezioni()
            {
                if (tipoTesto == TestoTipi.Bibbia && !tuttiTesti)
                {
                    pmCollezioni.Items.Clear();
                    noteOnVerseToolStripMenuItem.DropDownItems.Clear();

                    Collection<string> commentari = Principale.testi.NomiVersioni(TestoTipi.Commentario);
                    string[] commentariPredefiniti = Settings.Default.Commentari.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                    // collezioni con una versione preferita vanno mostrate solo in quella versione
                    foreach (string commentario in commentari)
                    {
                        string versioneDelleNote = Principale.testi.Info(commentario).VersioneDelleNote;
                        if (String.IsNullOrEmpty(versioneDelleNote) || versioneDelleNote == Versione)
                        {
                            pmCollezioni.Items.Add(commentario, null, CollezioneClick);
                            foreach (string commentarioPredefinito in commentariPredefiniti)
                                if (commentarioPredefinito == commentario)
                                    ((ToolStripMenuItem)(pmCollezioni.Items[pmCollezioni.Items.Count - 1])).Checked = true;
                            noteOnVerseToolStripMenuItem.DropDownItems.Add(commentario, null, NoteOnVerseClick);
                        }
                    }
                    pulNote.Visible = (pmCollezioni.Items.Count > 0);
                }
                else
                    pulNote.Visible = false;
            }

            internal void AggiornaHighlight()
            {
                Rtf.AggiungiHighlightDaFile();
                CambiaFormato();
            }

            private void CollezioneClick(object sender, EventArgs e)
            {
                ToolStripMenuItem voceDelMenu = ((ToolStripMenuItem)sender);
                voceDelMenu.Checked = !(voceDelMenu.Checked);
                CambiaFormato();
            }

            private void NoteOnVerseClick(object sender, EventArgs e)
            { // questo codice è anche in Editor::NoteOnVerseClick
                string riferimento = rtfTesto.VersettoAttuale(posizioneUltimoClic) + "0000";
                string riferimentoComeNota = "#" + riferimento + "-" + riferimento;
                string commentario = ((ToolStripMenuItem)sender).Text;
                if (!string.IsNullOrEmpty(Principale.testi.GetNotaTesto(riferimento, commentario)))
                    genitore.genitore.ApriNotaInEditor(riferimentoComeNota, commentario);
                else
                {
                    Riferimento riferimentoComeRiferimento = Principale.testi.ConvertiRiferimento(Principale.testi.ConvertiTitoloNotaARiferimento(riferimentoComeNota));
                    Riferimento noteCheContengonoVersetto = Principale.testi.ElencaNoteInBrano(riferimentoComeRiferimento, commentario);
                    if (noteCheContengonoVersetto.Count > 0)
                    {
                       genitore.genitore.ApriNotaInEditor(noteCheContengonoVersetto.Note[0], commentario);
                    }
                    else
                    { // non c'è una nota che contiene questo versetto, quindi aprire una nota vuota
                        genitore.genitore.ApriNotaInEditor(riferimentoComeNota, commentario);
                    }
                }
            }

            private void NoteOnWordClick(object sender, EventArgs e)
            {
                string parola = GetParolaAttuale();
                string dizionario = ((ToolStripMenuItem)sender).Text;

                if (!string.IsNullOrEmpty(Principale.testi.GetNotaTesto(parola, dizionario)))
                    genitore.genitore.ApriNotaInEditor(parola, dizionario);
                else
                {
                    string radice = (string.IsNullOrEmpty(Versione) ? "" : Principale.testi.RadiceDiParola(parola, Versione));
                    if (radice != "*" && !string.IsNullOrEmpty(radice))
                    {
                        if (!string.IsNullOrEmpty(Principale.testi.GetNotaTesto(radice, dizionario)))
                            genitore.genitore.ApriNotaInEditor(radice, dizionario);
                    }
                }
            }

            #region SpostaTesto

            internal void SpostaTesto(byte nuovoLibro, byte nuovoCapitolo, byte nuovoVersetto, bool sincronizza)
            {
                rtfTesto.BloccaRtf(true);
                try
                {
                    if (nuovoLibro < 1)
                    {
                        nuovoLibro = 1;
                        nuovoCapitolo = 1;
                        nuovoVersetto = 1;
                    }
                    if (nuovoCapitolo < 1)
                    {
                        nuovoCapitolo = 1;
                        nuovoVersetto = 1;
                    }
                    if (nuovoVersetto < 1)
                        nuovoVersetto = 1;
                    if (tipoTesto == TestoTipi.Bibbia && !tuttiTesti)
                    {
                        if (nuovoLibro > 73)
                        {
                            nuovoLibro = 73;
                            nuovoCapitolo = Math.Max((byte)1, Principale.testi.CapitoliInLibro(73, Versione));
                            nuovoVersetto = Math.Max((byte)1, Principale.testi.VersettiInCapitolo(73, nuovoCapitolo, Versione));
                        }
                        if (nuovoCapitolo > Principale.testi.CapitoliInLibro(nuovoLibro, Versione))
                        {
                            nuovoCapitolo = Math.Max((byte)1, Principale.testi.CapitoliInLibro(nuovoLibro, Versione));
                            nuovoVersetto = Math.Max((byte)1, Principale.testi.VersettiInCapitolo(nuovoLibro, nuovoCapitolo, Versione));
                        }
                        if (nuovoVersetto > Principale.testi.VersettiInCapitolo(nuovoLibro, nuovoCapitolo, Versione))
                        {
                            nuovoVersetto = Math.Max((byte)1, Principale.testi.VersettiInCapitolo(nuovoLibro, nuovoCapitolo, Versione));
                        }
                    }
                    else
                        if (nuovoLibro > 73)
                        nuovoLibro = 73;

                    Libro = nuovoLibro;
                    Capitolo = nuovoCapitolo;
                    Versetto = nuovoVersetto;

                    if (tuttiTesti)
                    {
                        Collection<string> versioni;
                        string testoNota;
                        switch (tipoTesto)
                        {
                            case TestoTipi.Bibbia:
                                versioni = Principale.testi.NomiVersioni(TestoTipi.Bibbia);
                                testoNota = Principale.testi.TestoBrano(new Riferimento(Libro, Capitolo, Versetto), versioni);
                                break;
                            case TestoTipi.Commentario:
                                versioni = Principale.testi.NomiVersioni(TestoTipi.Commentario);
                                testoNota = Principale.testi.TestoBrano(new Riferimento(Libro, Capitolo, Versetto), new Collection<string>(), versioni);
                                break;
                            default:
                                testoNota = "";
                                break;
                        }
                        try
                        {
                            rtfTesto.Rtf = testoNota;
                            rtfTesto.MostraLink();
                        }
                        catch
                        {
                            rtfTesto.Text = testoNota;
                        }
                        rtfTesto.Modified = false;
                    }
                    else
                    {
                        if (tipoTesto == TestoTipi.Bibbia)
                        {
                            StringBuilder riferimento = new StringBuilder();
                            riferimento.Append(Principale.testi.LibriAbbreviazioniRiconosciute.Abbreviazione(nuovoLibro)).Append(nuovoCapitolo).Append("-");
                            byte libroFine = (byte)(nuovoLibro - 1);
                            UInt16 capitoloFine = (UInt16)(Principale.testi.CapitoliFinoALibro(libroFine, Versione) + nuovoCapitolo + 5);
                            do
                            {
                                ++libroFine;
                            } while (libroFine < 73 && Principale.testi.CapitoliFinoALibro(libroFine, Versione) < capitoloFine);
                            riferimento.Append(Principale.testi.LibriAbbreviazioniRiconosciute.Abbreviazione(libroFine)).Append(capitoloFine - Principale.testi.CapitoliFinoALibro((byte)(libroFine - 1), Versione));

                            Collection<string> collezioniDaVisualizzare = new Collection<string>();
                            for (int i = 0; i < pmCollezioni.Items.Count; ++i)
                                if (((ToolStripMenuItem)(pmCollezioni.Items[i])).Checked)
                                    collezioniDaVisualizzare.Add(pmCollezioni.Items[i].Text);
                            string testoBrano = Principale.testi.TestoBrano(riferimento.ToString(), Versione, collezioniDaVisualizzare, ParoleRicercate);
                            rtfTesto.Rtf = testoBrano;

                            rtfTesto.SelectionStart = Math.Max(0, rtfTesto.Text.IndexOf(RichTextBoxEx.InizioRiferimento + Funzioni.AggiungiZero(nuovoLibro, 2) + Funzioni.AggiungiZero(nuovoCapitolo, 3) + Funzioni.AggiungiZero(nuovoVersetto, 3), StringComparison.Ordinal));
                            rtfTesto.SelectionLength = 0;
                            rtfTesto.ScrollToCaret();
                            ScrollBarValore = Principale.testi.CapitoliFinoALibro((byte)(Libro - 1), Versione) + Capitolo;
                            int ss = rtfTesto.SelectionStart;
                            int sl = rtfTesto.SelectionLength;
                            int posizioneCapitolo;

                            foreach (Highlight highlight in rtfTesto.highlightAttuale)
                            {
                                if (nuovoLibro <= highlight.libro && libroFine >= highlight.libro)
                                { // non necessario, ma è più veloce se facciamo IndexOf solo quando il libro del testo da selezionare è fra i libri del testo visualizzato
                                    posizioneCapitolo = rtfTesto.Text.IndexOf(RichTextBoxEx.InizioRiferimento + Funzioni.AggiungiZero(highlight.libro, 2) + Funzioni.AggiungiZero(highlight.capitolo, 3) + "001", StringComparison.Ordinal);
                                    if (posizioneCapitolo >= 0)
                                    {
                                        // se l'inizio non è stato trovato, non mostriamo l'evidenziatore
                                        // può essere un errore, ma più probabilmente il testo da evidenziare non è nei 5 capitoli attualmente visualizzati
                                        rtfTesto.MettiHighlight(highlight, posizioneCapitolo);
                                    }
                                }
                            }

                            rtfTesto.SelectionStart = ss;
                            rtfTesto.SelectionLength = sl;
                        }
                        else // commentario
                        {
                            string testoNota = Principale.testi.TestoBrano(new Riferimento(Libro, Capitolo, Versetto), Versione, ParoleRicercate);
                            try
                            {
                                rtfTesto.Rtf = testoNota;
                                // rtfTesto.MostraLink(); fatto più avanti
                            }
                            catch
                            {
                                rtfTesto.Text = testoNota;
                            }
                            bool highlightUsato = false;
                            foreach (Highlight highlight in rtfTesto.highlightAttuale)
                            {
                                if (highlight.libro == Libro && highlight.capitolo == Capitolo && highlight.versetto == Versetto)
                                {
                                    rtfTesto.MettiHighlight(highlight, 0);
                                    highlightUsato = true;
                                }
                            }
                            if (highlightUsato)
                            {
                                rtfTesto.SelectionLength = 0;
                                rtfTesto.SelectionStart = 0;
                                rtfTesto.ScrollToCaret();
                            }
                            rtfTesto.Modified = false;
                        }
                    }

                    if (sincronizza)
                    {
                        SpostaAltreVisualizza(new Riferimento(nuovoLibro, nuovoCapitolo, nuovoVersetto));
                        AggiornaCronologia(); // altrimenti la cronologia è già stata aggiornata dalla finestra che ha spostato le altre sincronizzate
                    }

                    string riferimentoCaption = Principale.testi.NormalizzaRiferimento(nuovoLibro, nuovoCapitolo, nuovoVersetto);
                    postoAttuale = riferimentoCaption;
                    genitore.Text = Versione + " (" + riferimentoCaption + ")";
                    genitore.genitore.ImpostaBarraDiStato(riferimentoCaption);
                }
                catch (TextNotExistException)
                {
                    // la versione è stata cancellata; basta non spostare il testo
                }
                finally
                {
                    rtfTesto.MostraLink();
                    rtfTesto.BloccaRtf(false);
                    rtfTesto.Invalidate();
                    Application.DoEvents();
                }
            }

            internal void SpostaTesto(string titolo, bool sincronizza)
            {
                SpostaTesto(titolo, "", sincronizza);
            }

            private void SpostaTesto(string titolo, string radice, bool sincronizza)
            {
                rtfTesto.BloccaRtf(true);
                try
                {
                    string testoNota;
                    if (tuttiTesti)
                    {
                        testoNota = Principale.testi.GetTutteLeNote(titolo, radice);
                    }
                    else
                    {
                        testoNota = Principale.testi.GetNotaTesto(titolo, Versione);
                        if (string.IsNullOrEmpty(testoNota) && !string.IsNullOrEmpty(radice))
                            testoNota = Principale.testi.GetNotaTesto(radice, Versione);
                    }
                    try
                    {
                        rtfTesto.Rtf = testoNota;
                    }
                    catch
                    {
                        rtfTesto.Text = testoNota;
                    }

                    if (!tuttiTesti)
                    {
                        bool highlightUsato = false;
                        foreach (Highlight highlight in rtfTesto.highlightAttuale)
                        {
                            if (highlight.voce == Voce)
                            {
                                rtfTesto.MettiHighlight(highlight, 0);
                                highlightUsato = true;
                            }
                        }
                        if (highlightUsato)
                        {
                            rtfTesto.SelectionLength = 0;
                            rtfTesto.SelectionStart = 0;
                            rtfTesto.ScrollToCaret();
                        }
                    }
                    rtfTesto.Modified = false;

                    if (sincronizza)
                    {
                        SpostaAltreVisualizza(titolo);
                        AggiornaCronologia(); // altrimenti la cronologia è già stata aggiornata dalla finestra che ha spostato le altre sincronizzate
                    }

                    postoAttuale = titolo;
                    Voce = titolo;
                    genitore.Text = Versione + " (" + titolo + ")";
                    genitore.genitore.ImpostaBarraDiStato(titolo);

                }
                catch (TextNotExistException)
                {
                    // la versione è stata cancellata; basta non spostare il testo
                }
                finally
                {
                    rtfTesto.MostraLink();
                    rtfTesto.BloccaRtf(false);
                    rtfTesto.Invalidate();
                    Application.DoEvents();
                    genitore.genitore.ImpostaBarraOrdinePerVisualizza(this);
                }
            }

            internal void SpostaTesto(Riferimento riferimento, bool sincronizza)
            {
                Riferimento riferimentoTradotto = new Riferimento(riferimento); // necessario, perché riferimento è chiamato con variable e non valore, quindi un cambiamento qui cambierebbe anche il valore nella routine chiamante
                if (riferimentoTradotto.DaTradurre)
                    riferimentoTradotto = Principale.testi.ConvertiDaStandard(riferimentoTradotto, Versione);
                if (riferimentoTradotto.Count > 0)
                    SpostaTesto(riferimentoTradotto.Brani[0][0], riferimentoTradotto.Brani[0][1], riferimentoTradotto.Brani[0][2], sincronizza);
            }

            private void SpostaTestoNuovaSezioneNotMono(int nRighe)
            {
                // sposta il testo in rtfTesto nRighe (in giù se nRighe>0, in su se nRighe<0), quando il testo desiderato non è più dentro rtfTesto
                if (nRighe == 0)
                    return;

                // il metodo è di ricordare il primo testo (100 caratteri), caricare il nuovo testo, cercare il testo, e poi spostare il testo
                int caratteriDaRicordare = 100;

                int charPos = rtfTesto.GetCharIndexFromPosition(new Point(0, 0));

                string primoTestoVisibile;
                if (charPos + caratteriDaRicordare <= rtfTesto.Text.Length)
                    primoTestoVisibile = rtfTesto.Text.Substring(charPos, caratteriDaRicordare);
                else
                    primoTestoVisibile = rtfTesto.Text.Substring(charPos);

                // metteremo il testo che comincia due capitoli dopo o prima del testo attuale (dipende se nRighe>0 o <0; se =0, siamo già usciti da questa routine
                int capitoliDaSpostare = Math.Sign(nRighe);
                capitoliDaSpostare *= 2;
                int nuovoCapitolo = Principale.testi.CapitoliFinoALibro((byte)(Libro - 1), Versione) + Capitolo + capitoliDaSpostare;
                byte libroDiCapitolo = Principale.testi.LibroDiCapitolo(nuovoCapitolo, Versione);
                nuovoCapitolo -= Principale.testi.CapitoliFinoALibro((byte)(libroDiCapitolo - 1), Versione);
                if (nuovoCapitolo < 1)
                    nuovoCapitolo = 1;

                SpostaTesto(libroDiCapitolo, (byte)(nuovoCapitolo), 1, false);
                rtfTesto.BloccaRtf(true); // bloccare l'aggiornamento del RichEdit
                try
                {
                    SpostaTestoAPrimoTestoVisibileNotMono(primoTestoVisibile, nRighe);
                }
                finally
                {
                    rtfTesto.BloccaRtf(false);
                }
            }

            private void SpostaTestoAPrimoTestoVisibileNotMono(String primoTesto, int nRighe)
            {
                // dato il primo testo, questa funziona sposta il testo in reTesto nRighe dopo (o prima se nRighe<0) questo testo
                int charPos = rtfTesto.Text.IndexOf(primoTesto, StringComparison.Ordinal);
                int nRigheDaSpostare = (charPos >= 0) ? rtfTesto.GetLineFromCharIndex(charPos) : -nRighe;

                rtfTesto.BloccaRtf(true); // bloccare l'aggiornamento del RichEdit
                try
                {
                    nRigheDaSpostare += nRighe - (int)(SafeNativeMethods.SendMessage(rtfTesto.Handle, EM_GETFIRSTVISIBLELINE, (IntPtr)0, (IntPtr)0));
                    SafeNativeMethods.SendMessage(rtfTesto.Handle, EM_LINESCROLL, (IntPtr)0, (IntPtr)nRigheDaSpostare);
                }
                finally
                {
                    rtfTesto.BloccaRtf(false);
                }
            }

            public void CambiaFormato()
            {
                if (tipoTesto == TestoTipi.Dizionario)
                    SpostaTesto(Voce, false);
                else
                    SpostaTesto(Libro, Capitolo, Versetto, false);
            }

            private void SpostaAltreVisualizza()
            {
                SpostaAltreVisualizza(Principale.testi.ConvertiRiferimento(postoAttuale));
            }

            private void SpostaAltreVisualizza(string titolo)
            {
                // c'è codice simile in pmSincSottomenu_Click, un'altra SpostaAltreVisualizza
                string radice = (string.IsNullOrEmpty(Versione) ? "" : Principale.testi.RadiceDiParola(titolo, Versione));
                if (pulSinc.Text != "&X" && !string.IsNullOrEmpty(titolo))
                {
                    int sincNumero = SincNumero;
                    foreach (Form formFiglio in genitore.genitore.MdiChildren)
                    {
                        if (formFiglio.Tag != null && formFiglio.Tag.ToString() == "Visualizza")
                        {
                            foreach (Pane paneFiglio in ((Visualizza)formFiglio).panes)
                            {
                                if (paneFiglio.SincNumero == sincNumero && paneFiglio != this && paneFiglio.tipoTesto == TestoTipi.Dizionario)
                                    paneFiglio.SpostaTesto(titolo, radice, false);
                            }
                        }
                    }
                    if (genitore.genitore.formProiettato != null && genitore.genitore.formProiettato.Tag != null && genitore.genitore.formProiettato.Tag.ToString() == "Visualizza")
                    {
                        foreach (Pane paneFiglio in ((Visualizza)genitore.genitore.formProiettato).panes)
                        {
                            if (paneFiglio.SincNumero == sincNumero && paneFiglio != this && paneFiglio.tipoTesto == TestoTipi.Dizionario)
                                paneFiglio.SpostaTesto(titolo, radice, false);
                        }
                    }
                    ImpostaTitolo(); // perché spostare il testo nelle altre finestre cambia i titoli mostrati
                }
            }

            private void SpostaAltreVisualizza(Riferimento riferimento)
            {
                // c'è codice simile in pmSincSottomenu_Click, un'altra SpostaAltreVisualizza
                if (pulSinc.Text != "&X")
                {
                    int sincNumero = SincNumero;
                    Riferimento riferimentoACuiSpostare = Principale.testi.ConvertiAStandard(riferimento, Versione);
                    riferimentoACuiSpostare.DaTradurre = true;
                    foreach (Form formFiglio in genitore.genitore.MdiChildren)
                    {
                        if (formFiglio.Tag != null && formFiglio.Tag.ToString() == "Visualizza")
                        {
                            foreach (Pane paneFiglio in ((Visualizza)formFiglio).panes)
                            {
                                if (paneFiglio.SincNumero == sincNumero && paneFiglio != this && paneFiglio.tipoTesto != TestoTipi.Dizionario)
                                    paneFiglio.SpostaTesto(riferimentoACuiSpostare, false);
                            }
                        }
                    }
                    if (genitore.genitore.formProiettato != null && genitore.genitore.formProiettato.Tag != null && genitore.genitore.formProiettato.Tag.ToString() == "Visualizza")
                    {
                        foreach (Pane paneFiglio in ((Visualizza)genitore.genitore.formProiettato).panes)
                        {
                            if (paneFiglio.SincNumero == sincNumero && paneFiglio != this && paneFiglio.tipoTesto != TestoTipi.Dizionario)
                                paneFiglio.SpostaTesto(riferimentoACuiSpostare, false);
                        }
                    }
                    ImpostaTitolo(); // perché spostare il testo nelle altre finestre cambia i titoli mostrati
                    Principale.testi.UltimaBibbia = Versione; // perché le versioni sincronizzate non dovrebbero essere contate come UltimaBibbia
                }
            }

            #endregion

            #endregion

            #region RichTextBox eventi

            private void RtfTesto_KeyDown(object sender, KeyEventArgs e)
            {
                if (e.Control)
                    ctrlPremuto = true;
                if ((tipoTesto == TestoTipi.Bibbia) && !tuttiTesti)
                {
                    if (!e.Shift)
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.Down:
                                ScrollBar_Scroll(sender, new ScrollEventArgs(ScrollEventType.SmallIncrement, 0));
                                e.Handled = true;
                                break;
                            case Keys.End:
                                if (e.Control)
                                {
                                    ScrollBar_Scroll(sender, new ScrollEventArgs(ScrollEventType.Last, 0));
                                    e.Handled = true;
                                }
                                break;
                            case Keys.Home:
                                if (e.Control)
                                {
                                    ScrollBar_Scroll(sender, new ScrollEventArgs(ScrollEventType.First, 0));
                                    e.Handled = true;
                                }
                                break;
                            case Keys.PageDown:
                                ScrollBar_Scroll(sender, new ScrollEventArgs(ScrollEventType.LargeIncrement, 0));
                                e.Handled = true;
                                break;
                            case Keys.PageUp:
                                ScrollBar_Scroll(sender, new ScrollEventArgs(ScrollEventType.LargeDecrement, 0));
                                e.Handled = true;
                                break;
                            case Keys.Up:
                                ScrollBar_Scroll(sender, new ScrollEventArgs(ScrollEventType.SmallDecrement, 0));
                                e.Handled = true;
                                break;
                        }
                    }
                }
            }

            void RtfTesto_KeyUp(object sender, KeyEventArgs e)
            {
                ctrlPremuto = false;
            }

            private void RtfTesto_SelectionChanged(object sender, EventArgs e)
            {
                try
                {
                    bool testoSelezionato = (rtfTesto.SelectionLength > 0);
                    genitore.genitore.copyToolStripMenuItem.Enabled = testoSelezionato;
                    genitore.genitore.copyToolStripButton.Enabled = testoSelezionato;
                    genitore.genitore.printToolStripMenuItem.Enabled = testoSelezionato;
                    genitore.genitore.printToolStripButton.Enabled = testoSelezionato;
                    genitore.genitore.highlightToolStripMenuItem.Enabled = testoSelezionato;
                    genitore.genitore.highlightBrowseToolStripSplitButton.Enabled = testoSelezionato;

                    SpostaAltreVisualizza(rtfTesto.ParolaAttuale(rtfTesto.SelectionStart));
                }
                // è chiamato durante la creazione della finestra, quando MdiParent non è anche impostato
                catch { }
            }

            private void RtfTesto_LinkHover(object sender, LinkHoverEventArgs e)
            {
                genitore.genitore.LinkHover(e);
            }

            private void RtfTesto_LinkClicked(object sender, LinkClickedEventArgs e)
            {
                genitore.genitore.LinkCliccato(genitore, Versione, e.LinkText);
            }

            void RtfTesto_HighlightChangedEvent(object sender, RichTextBoxHighlight.HighlightChangedEventArgs e)
            {
                genitore.genitore.HighlightChangedEvent(e);
            }

            internal void SaltoIpertestuale()
            {
                bool provaConDoppioClic = true;
                int p1 = rtfTesto.Text.LastIndexOf(RichTextBoxEx.InizioLink, rtfTesto.SelectionStart);
                int p2 = rtfTesto.Text.IndexOf(RichTextBoxEx.FineLink2, rtfTesto.SelectionStart);
                if (p1 >= 0 && p2 >= 0)
                {
                    string ipertesto = rtfTesto.Text.Substring(p1 + 1, p2 - p1 - 1);
                    if (ipertesto.IndexOf(RichTextBoxEx.InizioLink) < 0) // altrimenti c'è SelectionStart è fra 2 link, ma non ne fa parte di uno
                    {
                        string[] link = genitore.genitore.LinkCliccato(genitore, rtfTesto.Versione, null, ipertesto, true);
                        if (!string.IsNullOrEmpty(link[0]))
                            provaConDoppioClic = false;
                    }
                }
                if (provaConDoppioClic)
                    RtfTesto_MouseDoubleClick(this, null);
            }

            #region mouse eventi

            private void RtfTesto_MouseDoubleClick(object sender, MouseEventArgs e)
            {
                // bisogna usare posizioneUltimoClic perché con un doppio click su una parola come kat' nel greco,
                // .NET automaticamente seleziona la parola con un doppio clic,
                // però in questo caso seleziona solo l'apostrofe, quindi quando entra in questa routine
                // non c'è più una parola selezionata.
                string[] parolaTestoDizionario = Principale.TestoDalDizionario(rtfTesto, posizioneUltimoClic /* rtfTesto.SelectionStart*/, Versione);
                if (!string.IsNullOrEmpty(parolaTestoDizionario[0]))
                    genitore.genitore.ApriNotaInEditor(parolaTestoDizionario[0], parolaTestoDizionario[2]);
            }

            private void RtfTesto_MouseHover(object sender, EventArgs e)
            {
                if (genitore.genitore.ActiveMdiChild == genitore && Settings.Default.DizionarioTooltip)
                {
                    // non so perché, ma sembra funzionare meglio fare così, invece di fare rtTesto.GetCharIndexFromPosition(rtTesto.PointToClient(Cursor.Position))
                    Point pointScreen = System.Windows.Forms.Cursor.Position;
                    Point pointClient = rtfTesto.PointToClient(pointScreen);
                    int charPos = rtfTesto.GetCharIndexFromPosition(pointClient);
                    string[] parolaTestoDizionario = Principale.TestoDalDizionario(rtfTesto, charPos, Versione);
                    if (!string.IsNullOrEmpty(parolaTestoDizionario[0]))
                        rtfTesto.MostraHover(parolaTestoDizionario[1], parolaTestoDizionario[2], pointScreen, Settings.Default.OpzioniIpertestoTooltipInTooltip);
                }
            }

            private void RtfTesto_MouseDown(object sender, MouseEventArgs e)
            {
                posizioneUltimoClic = rtfTesto.GetCharIndexFromPosition(e.Location);

                if (rtfTesto.SelectionLength > 0)
                {
                    // Create a rectangle using the DragSize, with the mouse position being at the center of the rectangle.
                    Size dragSize = SystemInformation.DragSize;
                    dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
                }
                else
                {
                    dragBoxFromMouseDown = Rectangle.Empty;
                }
            }

            private void RtfTesto_MouseUp(object sender, MouseEventArgs e)
            {
                dragBoxFromMouseDown = Rectangle.Empty;
            }

            private void RtfTesto_MouseMove(object sender, MouseEventArgs e)
            {
                if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
                {
                    // If the mouse moves outside the rectangle, start the drag.
                    if (dragBoxFromMouseDown != Rectangle.Empty && !dragBoxFromMouseDown.Contains(e.X, e.Y))
                    {
                        DataObject dati = new DataObject();
                        dati.SetData(DataFormats.Text, rtfTesto.SelectedText);
                        dati.SetData(DataFormats.Rtf, rtfTesto.SelectedRtf);
                        rtfTesto.DoDragDrop(dati, DragDropEffects.Copy | DragDropEffects.Move);
                    }
                }
            }

            private void RtfTestoZoom_MouseWheel(object sender, MouseEventArgs e)
            {
                genitore.genitore.VisualizzaZoom();
            }

            private void RtfTesto_MouseWheel(object sender, MouseEventArgs e)
            {
                if (!ctrlPremuto) // con il tasto CTRL, la rotellina fa solo lo zoom, non scorre
                {
                    if (SystemInformation.MouseWheelScrollLines != -1) // un'opzione nel panello di controllo di Windows, di scorrere una schermata alla volta
                    {
                        int righe = e.Delta * SystemInformation.MouseWheelScrollLines / 120;
                        if (righe > 0)
                            for (int i = 1; i <= righe; ++i)
                                ScrollBar_Scroll(sender, new ScrollEventArgs(ScrollEventType.SmallDecrement, 0));
                        if (righe < 0)
                            for (int i = 1; i <= -righe; ++i)
                                ScrollBar_Scroll(sender, new ScrollEventArgs(ScrollEventType.SmallIncrement, 0));
                    }
                }
            }

            #endregion

            #endregion

            #region altri eventi

            internal bool Pane_Closing()
            {
                bool daCancellare = false;
                if (tipoTesto != TestoTipi.Bibbia && !tuttiTesti && rtfTesto.Modified)
                {
                    DialogResult dialogoRisultato = MessageBox.Show(string.Format(CultureInfo.CurrentCulture, Principale.LocRM.GetString("EditorSaveChanges"), genitore.Text), Principale.LocRM.GetString("MiscConfirm"), MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, Principale.messageBoxOptions);
                    if (dialogoRisultato == DialogResult.Yes)
                    {
                        if (tipoTesto == TestoTipi.Commentario)
                            Principale.testi.SetNotaTesto(rtfTesto.Rtf, new Riferimento(Libro, Capitolo, Versetto), Versione);
                        else if (tipoTesto == TestoTipi.Dizionario)
                            Principale.testi.SetNotaTesto(rtfTesto.Rtf, Voce, Versione);
                    }
                    if (dialogoRisultato == DialogResult.Cancel)
                        daCancellare = true;
                }
                if (!daCancellare)
                {
                    pmTesto.Dispose();
                    pmCollezioni.Dispose();
                }
                return daCancellare;
            }

            private void PanComponenti_Resize(object sender, EventArgs e)
            {
                int larghezzaScrollBar = ((tipoTesto == TestoTipi.Bibbia) ? 18 : 0);
                rtfTesto.Size = new Size(panComponenti.Width - larghezzaScrollBar, panComponenti.Height - 41);
                sbRtf.Location = new Point(panComponenti.Width - larghezzaScrollBar, 0);
                sbRtf.Size = new Size(18, panComponenti.Height - 41);
            }

            private void Controllo_Enter(object sender, EventArgs e)
            {
                genitore.paneAttivo = this;

                ImpostaTitolo();
                genitore.genitore.Principale_MdiChildActivate(null, null);

                ctrlPremuto = false;
            }

            internal void ImpostaTitolo()
            {
                string riferimentoCaption = (tipoTesto == TestoTipi.Dizionario ? Voce : Principale.testi.NormalizzaRiferimento(Libro, Capitolo, Versetto));
                genitore.Text = Versione + " (" + riferimentoCaption + ")";
                genitore.genitore.ImpostaBarraDiStato(riferimentoCaption);
            }

            private void ScrollBar_Scroll(object sender, ScrollEventArgs e)
            {
                if (!spostando)
                {
                    spostando = true;
                    if (!Principale.isRunningOnMono)
                        ScrollScrollBarNotMono(e);
                }
                spostando = false;
            }

            private void ScrollScrollBarNotMono(ScrollEventArgs e)
            {
                int rigaAttuale, righe;
                switch (e.Type)
                {
                    case ScrollEventType.EndScroll: // utente ha finito di trascinare lo scrollbar
                        break; // uno degli altri casi già chiamato
                    case ScrollEventType.First: // utente ha trascinato lo scrollbar fino in su
                        SpostaTesto(1, 1, 1, true);
                        break;
                    case ScrollEventType.LargeDecrement:
                        rigaAttuale = (int)(SafeNativeMethods.SendMessage(rtfTesto.Handle, EM_GETFIRSTVISIBLELINE, (IntPtr)0, (IntPtr)0));
                        righe = NRigheVisibili();
                        if (rigaAttuale < righe)
                            SpostaTestoNuovaSezioneNotMono(-righe + 1);
                        else
                            SafeNativeMethods.SendMessage(rtfTesto.Handle, EM_SCROLL, (IntPtr)SB_PAGEUP, (IntPtr)0);
                        MettiVersettoInTitoloEBarraDiStato();
                        SpostaAltreVisualizza();
                        break;
                    case ScrollEventType.LargeIncrement:
                        rigaAttuale = (int)(SafeNativeMethods.SendMessage(rtfTesto.Handle, EM_GETFIRSTVISIBLELINE, (IntPtr)0, (IntPtr)0));
                        righe = NRigheVisibili();
                        if (rigaAttuale + 2 * righe < (int)(SafeNativeMethods.SendMessage(rtfTesto.Handle, EM_GETLINECOUNT, (IntPtr)0, (IntPtr)0)))
                            SafeNativeMethods.SendMessage(rtfTesto.Handle, EM_SCROLL, (IntPtr)SB_PAGEDOWN, (IntPtr)0);
                        else
                            SpostaTestoNuovaSezioneNotMono(righe - 1);
                        MettiVersettoInTitoloEBarraDiStato();
                        SpostaAltreVisualizza();
                        break;
                    case ScrollEventType.Last: // utente ha trascinato lo scrollbar fino in giù
                        byte cap = Principale.testi.CapitoliInLibro(73, Versione);
                        SpostaTesto(73, cap, Principale.testi.VersettiInCapitolo(73, cap, Versione), true);
                        break;
                    case ScrollEventType.SmallDecrement:
                        if ((int)(SafeNativeMethods.SendMessage(rtfTesto.Handle, EM_GETFIRSTVISIBLELINE, (IntPtr)0, (IntPtr)0)) == 0)
                            SpostaTestoNuovaSezioneNotMono(-1);
                        else
                            SafeNativeMethods.SendMessage(rtfTesto.Handle, EM_LINESCROLL, (IntPtr)0, (IntPtr)(-1));
                        MettiVersettoInTitoloEBarraDiStato();
                        SpostaAltreVisualizza();
                        break;
                    case ScrollEventType.SmallIncrement:
                        rigaAttuale = (int)(SafeNativeMethods.SendMessage(rtfTesto.Handle, EM_GETFIRSTVISIBLELINE, (IntPtr)0, (IntPtr)0));
                        SafeNativeMethods.SendMessage(rtfTesto.Handle, EM_LINESCROLL, (IntPtr)0, (IntPtr)1);
                        if (rigaAttuale == (int)(SafeNativeMethods.SendMessage(rtfTesto.Handle, EM_GETFIRSTVISIBLELINE, (IntPtr)0, (IntPtr)0))) // testo non spostato, perché siamo alla fine del testo nella finestra
                            SpostaTestoNuovaSezioneNotMono(1);
                        MettiVersettoInTitoloEBarraDiStato();
                        SpostaAltreVisualizza();
                        break;
                    case ScrollEventType.ThumbPosition: // utente ha trascinato lo scrollbar in una nuova posizione
                        byte lib = 0;
                        int sbValue = ScrollBarValore;
                        do
                        {
                            lib++;
                        } while (lib <= 73 && Principale.testi.CapitoliFinoALibro(lib, Versione) < sbValue);
                        SpostaTesto((byte)(lib - 1), (byte)(sbValue - Principale.testi.CapitoliFinoALibro((byte)(lib - 1), Versione)), 1, true);
                        break;
                    case ScrollEventType.ThumbTrack: // utente sta spostando lo scrollbar
                        break; // non bisogna fare niente
                }
            }

            private void PulSinc_Click(object sender, EventArgs e)
            {
                genitore.pmSincronizzato.Show(pulSinc, 0, pulSinc.Height - 1);
            }

            void PulNote_Click(object sender, EventArgs e)
            {
                pmCollezioni.Show(pulNote, 0, pulNote.Height - 1);
            }

            #endregion

            private int NRigheVisibili()
            {
                int charPos1 = rtfTesto.GetCharIndexFromPosition(new Point(0, 0));
                int riga1 = rtfTesto.GetLineFromCharIndex(charPos1);
                int charPos2 = rtfTesto.GetCharIndexFromPosition(new Point(0, rtfTesto.Height + 1));
                int riga2 = rtfTesto.GetLineFromCharIndex(charPos2);
                return riga2 - riga1;
            }

            private void MettiVersettoInTitoloEBarraDiStato()
            {
                // chiamata solo da ScrollScrollBarNotMono, quindi non serve considerare il caso di TestoTipi.Dizionario
                string versettoAttuale8Byte = rtfTesto.VersettoAttuale(rtfTesto.GetCharIndexFromPosition(new Point(5, 5)));
                string riferimento = (versettoAttuale8Byte.Length >= 8 ? Principale.testi.NormalizzaRiferimento(Convert.ToInt32(versettoAttuale8Byte.Substring(0, 2), CultureInfo.InvariantCulture), Convert.ToInt32(versettoAttuale8Byte.Substring(2, 3), CultureInfo.InvariantCulture), Convert.ToInt32(versettoAttuale8Byte.Substring(5, 3), CultureInfo.InvariantCulture)) : "");

                if (string.IsNullOrEmpty(riferimento))
                    genitore.Text = Versione;
                else
                    genitore.Text = Versione + " (" + riferimento + ")";

                genitore.genitore.ImpostaBarraDiStato(riferimento);
                postoAttuale = riferimento;
            }

            private void AggiornaCronologia()
            {
                if (tipoTesto == TestoTipi.Dizionario)
                    return;
                if (genitore.genitore.aggiornaCronologia) // quando i pulsante Avanti e Indietro sono cliccati, è falso
                {
                    for (int i = genitore.genitore.cronologia.Count - 1; i > genitore.genitore.numeroInCronologia; --i)
                        genitore.genitore.cronologia.RemoveAt(i);

                    Riferimento riferimentoAttuale = new Riferimento(Libro, Capitolo, Versetto);
                    if (!string.IsNullOrEmpty(Versione)) // non per "Tutte le versioni" o "Tutti i commentari"
                        riferimentoAttuale = Principale.testi.ConvertiAStandard(riferimentoAttuale, Versione);
                    genitore.genitore.cronologia.Add(riferimentoAttuale);

                    genitore.genitore.numeroInCronologia += 1;
                }

                try
                {
                    genitore.genitore.browseBackToolStripButton.Enabled = (genitore.genitore.numeroInCronologia > 0);
                    if (genitore.genitore.browseBackToolStripButton.Enabled)
                        genitore.genitore.browseBackToolStripButton.Text = Principale.LocRM.GetString("BrowseBack") + " " + Principale.LocRM.GetString("BrowseTo") + " " + Principale.testi.NormalizzaRiferimento(genitore.genitore.cronologia[genitore.genitore.numeroInCronologia - 1]);
                    else
                        genitore.genitore.browseBackToolStripButton.Text = Principale.LocRM.GetString("BrowseBack");
                    genitore.genitore.browseForwardToolStripButton.Enabled = (genitore.genitore.numeroInCronologia < genitore.genitore.cronologia.Count - 1);
                    if (genitore.genitore.browseForwardToolStripButton.Enabled)
                        genitore.genitore.browseForwardToolStripButton.Text = Principale.LocRM.GetString("BrowseForward") + " " + Principale.LocRM.GetString("BrowseTo") + " " + Principale.testi.NormalizzaRiferimento(genitore.genitore.cronologia[genitore.genitore.numeroInCronologia + 1]);
                    else
                        genitore.genitore.browseForwardToolStripButton.Text = Principale.LocRM.GetString("BrowseForward");
                }
                // è chiamato durante la creazione della finestra, quando MdiParent non è anche impostato
                catch { }
            }

            #region Popup menu

            private void PmTesto_Opening(object sender, CancelEventArgs e)
            {
                copyToolStripMenuItem.Enabled = (rtfTesto.SelectionLength > 0);
                printToolStripMenuItem.Enabled = (rtfTesto.SelectionLength > 0);

                // the rest of the code is also in Editor for its popup
                string riferimento = rtfTesto.VersettoAttuale(posizioneUltimoClic);
                bool riferimentoScelto = !string.IsNullOrEmpty(riferimento);
                popupToolStripSeparatorWordVerse.Visible = riferimentoScelto;
                informationOnVerseToolStripMenuItem.Visible = riferimentoScelto;
                bookmarkVerseToolStripMenuItem.Visible = riferimentoScelto;

                if (riferimentoScelto)
                {
                    bool menuVisibile = false, submenuVisibile;
                    string commentario, riferimentoComeNota = "#" + riferimento + "0000-" + riferimento + "0000";
                    Riferimento riferimentoComeRiferimento = Principale.testi.ConvertiRiferimento(Principale.testi.ConvertiTitoloNotaARiferimento(riferimentoComeNota));
                    for (int i = 0; i < noteOnVerseToolStripMenuItem.DropDownItems.Count; ++i)
                    {
                        commentario = noteOnVerseToolStripMenuItem.DropDownItems[i].Text;
                        submenuVisibile = (Principale.testi.Info(commentario).Bloccato == BloccatoTipi.Sbloccato || Principale.testi.ElencaNoteInBrano(riferimentoComeRiferimento, commentario).Count > 0);
                        noteOnVerseToolStripMenuItem.DropDownItems[i].Visible = submenuVisibile;
                        // bisogna usare submenuVisibile, perché anche se diciamo menu.Visibile=true, menu.Visibile rimane falso fino a quando il menu è aperto
                        if (submenuVisibile)
                            menuVisibile = true;
                    }
                    noteOnVerseToolStripMenuItem.Visible = menuVisibile;

                    string riferimentoTestuale = Principale.testi.ConvertiTitoloNotaARiferimento(riferimentoComeNota);
                    if (string.IsNullOrEmpty(informationOnVerseToolStripMenuItem.Tag.ToString()))
                        informationOnVerseToolStripMenuItem.Tag = informationOnVerseToolStripMenuItem.Text;
                    informationOnVerseToolStripMenuItem.Text = informationOnVerseToolStripMenuItem.Tag.ToString() + riferimentoTestuale;
                    if (string.IsNullOrEmpty(bookmarkVerseToolStripMenuItem.Tag.ToString()))
                        bookmarkVerseToolStripMenuItem.Tag = bookmarkVerseToolStripMenuItem.Text;
                    bookmarkVerseToolStripMenuItem.Text = bookmarkVerseToolStripMenuItem.Tag.ToString() + riferimentoTestuale;
                    if (string.IsNullOrEmpty(noteOnVerseToolStripMenuItem.Tag.ToString()))
                        noteOnVerseToolStripMenuItem.Tag = noteOnVerseToolStripMenuItem.Text;
                    noteOnVerseToolStripMenuItem.Text = noteOnVerseToolStripMenuItem.Tag.ToString() + riferimentoTestuale;
                }

                string parola = GetParolaAttuale();
                bool parolaScelta = !string.IsNullOrEmpty(parola);
                popupToolStripSeparatorGeneralWord.Visible = parolaScelta;
                informationOnWordToolStripMenuItem.Visible = parolaScelta;
                searchToolStripMenuItem.Visible = (parolaScelta && !string.IsNullOrEmpty(Versione));
                noteOnWordToolStripMenuItem.Visible = parolaScelta;
                if (parolaScelta)
                {
                    searchWordToolStripMenuItem.Text = parola;
                    string radice = (string.IsNullOrEmpty(Versione) ? "" : Principale.testi.RadiceDiParola(parola, Versione));
                    if (radice == "*")
                        radice = "";
                    searchRadiceToolStripMenuItem.Visible = !string.IsNullOrEmpty(radice);
                    searchRadiceToolStripMenuItem.Text = radice;
                    if (string.IsNullOrEmpty(informationOnWordToolStripMenuItem.Tag.ToString()))
                        informationOnWordToolStripMenuItem.Tag = informationOnWordToolStripMenuItem.Text;
                    informationOnWordToolStripMenuItem.Text = informationOnWordToolStripMenuItem.Tag.ToString() + parola;
                    string searchSelection = rtfTesto.SelectedText;
                    for (int i = searchSelection.Length - 1; i >= 0; --i)
                        if (!Funzioni.IsLettera(searchSelection[i]) && !char.IsWhiteSpace(searchSelection[i]))
                            searchSelection = searchSelection.Remove(i, 1);
                    searchSelection = searchSelection.Trim();
                    searchSelectionToolStripMenuItem.Text = searchSelection;
                    searchSelectionToolStripMenuItem.Visible = !string.IsNullOrEmpty(searchSelection);

                    noteOnWordToolStripMenuItem.DropDownItems.Clear();
                    Collection<string> dizionari = Principale.testi.NomiVersioni(TestoTipi.Dizionario);
                    foreach (string dizionario in dizionari)
                    {
                        bool collezioneDaAggiungere = false;
                        if (!string.IsNullOrEmpty(Principale.testi.GetNotaTesto(parola, dizionario)))
                            collezioneDaAggiungere = true;
                        else
                        {
                            if (!string.IsNullOrEmpty(Principale.testi.GetNotaTesto(radice, dizionario)))
                                collezioneDaAggiungere = true;
                        }
                        if (collezioneDaAggiungere)
                            noteOnWordToolStripMenuItem.DropDownItems.Add(dizionario, null, NoteOnWordClick);
                    }
                    if (noteOnWordToolStripMenuItem.DropDownItems.Count == 0)
                        noteOnWordToolStripMenuItem.Visible = false;
                    else
                    {
                        if (string.IsNullOrEmpty(noteOnWordToolStripMenuItem.Tag.ToString()))
                            noteOnWordToolStripMenuItem.Tag = noteOnWordToolStripMenuItem.Text;
                        noteOnWordToolStripMenuItem.Text = noteOnWordToolStripMenuItem.Tag.ToString() + parola;
                    }
                }
            }

            private string GetParolaAttuale()
            {
                string parola = rtfTesto.ParolaAttuale(posizioneUltimoClic);
                // a click before a verse reference picks up the hidden text
                if (parola.Length >= 8)
                    if (char.IsDigit(parola[0]) && char.IsDigit(parola[1]) && char.IsDigit(parola[2]) && char.IsDigit(parola[3]) && char.IsDigit(parola[4]) && char.IsDigit(parola[5]) && char.IsDigit(parola[6]) && char.IsDigit(parola[7]))
                        parola = "";
                return parola;
            }

            private static string Rovescia(string parola)
            {
                StringBuilder parolaRovesciata = new StringBuilder("");
                for (int i = parola.Length - 1; i >= 0; --i)
                    parolaRovesciata.Append(parola[i]);
                return parolaRovesciata.ToString();
            }

            private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
            {
                CopiaSelezione();
            }

            private void PrintToolStripMenuItem_Click(object sender, EventArgs e)
            {
                StampaSelezione();
            }

            private void InformationToolStripMenuItem_Click(object sender, EventArgs e)
            {
                ToolStripItem tsi = (ToolStripItem)(sender);
                string testo = tsi.Text.Substring(tsi.Tag.ToString().Length);
                if (sender == informationOnWordToolStripMenuItem)
                {
                    if (testo.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }) >= 0 && testo.IndexOfAny(new char[] { '<', '>' }) == 0)
                        testo = "<" + testo + ">";
                }
                genitore.genitore.ApriInformazione(testo);
            }

            private void SearchToolStripMenuItem_Click(object sender, EventArgs e)
            {
                string frase = ((ToolStripItem)(sender)).Text;
                if (sender == searchRadiceToolStripMenuItem)
                    frase = "/" + frase;
                else if (sender == searchSelectionToolStripMenuItem)
                    frase = "[" + frase + "]";
                else
                    frase = "<" + frase + ">";
                genitore.genitore.RicercaInVisualizza(frase, Versione);
            }

            private void BookmarkVerseToolStripMenuItem_Click(object sender, EventArgs e)
            {
                bool trovato = false;
                Segnalibri formSegnalibri = null;
                foreach (Form formFiglio in genitore.MdiChildren)
                {
                    if (formFiglio.Tag != null && formFiglio.Tag.ToString() == "Segnalibri")
                    {
                        formFiglio.Activate();
                        trovato = true;
                        formSegnalibri = (Segnalibri)formFiglio;
                    }
                }
                if (!trovato)
                {
                    formSegnalibri = new Segnalibri(genitore.genitore)
                    {
                        MdiParent = genitore.genitore
                    };
                    formSegnalibri.Show();
                }
                string versettoCliccato = rtfTesto.VersettoAttuale(posizioneUltimoClic);
                if (!string.IsNullOrEmpty(versettoCliccato))
                {
                    Riferimento riferimento = Principale.testi.ConvertiAStandard(new Riferimento(Convert.ToByte(versettoCliccato.Substring(0, 2), CultureInfo.InvariantCulture), Convert.ToByte(versettoCliccato.Substring(2, 3), CultureInfo.InvariantCulture), Convert.ToByte(versettoCliccato.Substring(5, 3), CultureInfo.InvariantCulture)), Versione);
                    if (riferimento.Count > 0)
                        formSegnalibri.AggiungiSegnalibroPersonale(riferimento.Brani[0][0], riferimento.Brani[0][1], riferimento.Brani[0][2]);
                }
            }

            #endregion

        }

        #endregion

        private const Int32 EM_SCROLL = 181;
        private const Int32 EM_LINESCROLL = 182;
        private const Int32 EM_GETLINECOUNT = 186;
        private const Int32 EM_GETFIRSTVISIBLELINE = 206;
        private const Int32 SB_PAGEUP = 2;
        private const Int32 SB_PAGEDOWN = 3;

        #region proprietà

        public const int LARGHEZZA_PULSANTI = 56;
        private const int LARGHEZZA_MINIMA_PANE = 135;
        private const int LARGHEZZA_SPLITTER = 1;

        private readonly Principale genitore;

        public List<Pane> panes = new List<Pane>();
        public Pane paneAttivo = null;

        private readonly List<Splitter> splitters = new List<Splitter>();

        private int ultimoTab = -1;
        private readonly int massimoNumeroPane = (SystemInformation.MaxWindowTrackSize.Width - LARGHEZZA_PULSANTI) / LARGHEZZA_MINIMA_PANE;
        private bool noResize = false;
        private int larghezzaVecchia;

        public int SelectionLength
        {
            get { return paneAttivo.SelectionLength; }
        }

        public RichTextBoxEx RtfAttiva
        {
            get { return paneAttivo.Rtf; }
        }

        #endregion

        #region Costruttori

        public Visualizza(Principale formGenitore, TestoTipi tipo)
        {
            InitializeComponent();
            genitore = formGenitore;
            panes.Add(new Pane(this, tipo, 320));
            CostruttoreComune();

            if (tipo != TestoTipi.Dizionario) // per mettere i link ipertestuali
                CambiaFormato();
        }

        public Visualizza(Principale formGenitore, string nomeVersione, TestoTipi tipoTesto)
        {
            InitializeComponent();
            genitore = formGenitore;
            panes.Add(new Pane(this, nomeVersione, tipoTesto, 320, false));
            CostruttoreComune();

            // per qualche motivo sconosciuto, quando si apre un commentario, eventuale evidenziazione di sottolineatura non è mostrata
            // (mentre lo è se si sposta il testo, e se la finestra è aperta all'avvio del programma)
            // la seguente riga costringe a finestra di ridisegnarsi, e la sottolineatura è mostrata
            // è anche in AggiungiPane, mentre non funziona se messa nel costruttore di Pane
            if (((Principale.testi.Info(nomeVersione).Tipo & TestoTipi.Commentario) == TestoTipi.Commentario) || ((Principale.testi.Info(nomeVersione).Tipo & TestoTipi.Dizionario) == TestoTipi.Dizionario))
                CambiaFormato();
        }

        private void CostruttoreComune()
        {
            paneAttivo = panes[0];
            larghezzaVecchia = Width;
            splitters.Add(null);

            AggiornaMenu();
        }

        #endregion

        #region metodi

        internal void AggiornaHighlight(string versione)
        {
            foreach (Pane p in panes)
            {
                if (p.Versione == versione)
                    p.AggiornaHighlight();
            }
        }

        internal void AggiornaMenu()
        {
            pmAggiungiBibbia.DropDownItems.Clear();
            Collection<string> bibbie = Principale.testi.NomiVersioni(TestoTipi.Bibbia);
            foreach (string bibbia in bibbie)
                pmAggiungiBibbia.DropDownItems.Add(bibbia, null, AggiungiTestoClick);
            pmAggiungiBibbia.Visible = (pmAggiungiBibbia.DropDownItems.Count > 0);

            pmAggiungiCommentario.DropDownItems.Clear();
            Collection<string> commentari = Principale.testi.NomiVersioni(TestoTipi.Commentario);
            foreach (string commentario in commentari)
                pmAggiungiCommentario.DropDownItems.Add(commentario, null, AggiungiTestoClick);
            pmAggiungiCommentario.Visible = (pmAggiungiCommentario.DropDownItems.Count > 0);

            pmAggiungiDizionario.DropDownItems.Clear();
            Collection<string> dizionari = Principale.testi.NomiVersioni(TestoTipi.Dizionario);
            foreach (string dizionario in dizionari)
                pmAggiungiDizionario.DropDownItems.Add(dizionario, null, AggiungiTestoClick);
            pmAggiungiDizionario.Visible = (pmAggiungiDizionario.DropDownItems.Count > 0);

            AggiornaMenuConfronta(bibbie);

            //ImpostaAltriTestiDaAggiungere(pmAggiungiBibbia.Visible | pmAggiungiCommentario.Visible| pmAggiungiDizionario.Visible); questo non funziona nel costruttore
            ImpostaAltriTestiDaAggiungere((pmAggiungiCommentario.DropDownItems.Count > 0) | (pmAggiungiBibbia.DropDownItems.Count > 0) | (pmAggiungiDizionario.DropDownItems.Count > 0));

            foreach (Pane paneFiglio in panes)
                paneFiglio.AggiornaMenuCollezioni();
        }

        private void AggiornaMenuConfronta()
        {
            AggiornaMenuConfronta(Principale.testi.NomiVersioni(TestoTipi.Bibbia));
        }

        private void AggiornaMenuConfronta(Collection<string> bibbie)
        {
            pmConfrontaBibbia.DropDownItems.Clear();
            string lingua = Principale.testi.Info(panes[0].Versione).Lingua;
            bool versioneTraslitterata = panes[0].Versione.EndsWith("transliterated", StringComparison.InvariantCultureIgnoreCase);
            if (!string.IsNullOrEmpty(lingua))
            {
                foreach (string bibbia in bibbie)
                {
                    if (Principale.testi.Info(bibbia).Lingua == lingua && panes[0].Versione != bibbia)
                        if ((versioneTraslitterata && bibbia.EndsWith("transliterated", StringComparison.InvariantCultureIgnoreCase)) || (!versioneTraslitterata && !bibbia.EndsWith("transliterated", StringComparison.InvariantCultureIgnoreCase)))
                            pmConfrontaBibbia.DropDownItems.Add(bibbia, null, ConfrontaBibbiaClick);
                }
            }
            pmConfrontaBibbia.Visible = (pmConfrontaBibbia.DropDownItems.Count > 0);

            // TODO (C) cancella questa riga
            pmConfrontaBibbia.Visible = false;
        }

        private void AggiungiTestoClick(object sender, EventArgs e)
        {
            TestoTipi tipo = TestoTipi.None;
            ToolStripItem tsTipo = (((ToolStripMenuItem)sender).OwnerItem);
            if (tsTipo == pmAggiungiBibbia)
                tipo = TestoTipi.Bibbia;
            else if (tsTipo == pmAggiungiCommentario)
                tipo = TestoTipi.Commentario;
            else if (tsTipo == pmAggiungiDizionario)
                tipo = TestoTipi.Dizionario;
            AggiungiPane(((ToolStripMenuItem)sender).Text, tipo);
        }

        private void ConfrontaBibbiaClick(object sender, EventArgs e)
        {
            AggiungiPaneConfronta(((ToolStripMenuItem)sender).Text);
        }

        internal void StampaSelezione()
        {
            paneAttivo.StampaSelezione();
        }

        internal void CopiaSelezione()
        {
            paneAttivo.CopiaSelezione();
        }

        internal Pane AggiungiPane(string versione, TestoTipi tipoTesto)
        {
            return AggiungiPane(versione, tipoTesto, false);
        }

        private Pane AggiungiPane(string versione, TestoTipi tipoTesto, bool confronta)
        {
            if (panes.Count == massimoNumeroPane)
                return null;

            // proviamo ad aprire un nuovo pane con la larghezza media degli altri pane
            int larghezzaNuovoPane = (panes[panes.Count - 1].Location.X + panes[panes.Count - 1].Size.Width) / panes.Count;
            int nuovaSinistraVisualizza = Left;
            int larghezzaMassimaProgramma = genitore.ClientSize.Width - 4;
            if (Left + Width + larghezzaNuovoPane > larghezzaMassimaProgramma)
            {
                nuovaSinistraVisualizza = larghezzaMassimaProgramma - Width - larghezzaNuovoPane;
                if (nuovaSinistraVisualizza < 0)
                    nuovaSinistraVisualizza = 0;
                Left = nuovaSinistraVisualizza;
            }
            if (nuovaSinistraVisualizza + Width + larghezzaNuovoPane > larghezzaMassimaProgramma)
                larghezzaNuovoPane = larghezzaMassimaProgramma - nuovaSinistraVisualizza - Width;

            // stringiamo ogni pane che non è già al minimo per cercare di inserire quello nuovo
            // ma se il pane è appena sopra il minimo, non viene stretto abbastanza e il nuovo pane non ci sta ancora
            // quindi proviamo di nuovo con i pane che adesso non sono al minimo
            // ripetiamo il ciclo, al massimo una volta per ogni pane esistente
            // c'è codice simile in Visualizza_Resize
            int numeroTentativi = 0;
            while (larghezzaNuovoPane < LARGHEZZA_MINIMA_PANE && numeroTentativi < panes.Count)
            {
                ++numeroTentativi;
                int numeroPaneDaRidurre = panes.Count;
                for (int i = 0; i < panes.Count; ++i)
                {
                    if (panes[i].Size.Width == LARGHEZZA_MINIMA_PANE)
                        --numeroPaneDaRidurre;
                }
                int daRidurre = LARGHEZZA_MINIMA_PANE - larghezzaNuovoPane;
                int daRidurreOgniPane;
                if (numeroPaneDaRidurre == 0)
                    daRidurreOgniPane = 0;
                else
                {
                    daRidurreOgniPane = daRidurre / numeroPaneDaRidurre;
                    if (daRidurre % panes.Count != 0)
                        daRidurreOgniPane += 1;
                }
                int larghezzaRidotta = 0;
                for (int i = 0; i < panes.Count; ++i)
                {
                    int larghezzaNuovaP = panes[i].Size.Width - daRidurreOgniPane;
                    if (larghezzaNuovaP < LARGHEZZA_MINIMA_PANE)
                        larghezzaNuovaP = LARGHEZZA_MINIMA_PANE;
                    larghezzaRidotta += panes[i].Size.Width - larghezzaNuovaP;
                    panes[i].Size = new Size(larghezzaNuovaP, panes[i].Size.Height);
                    panes[i].Location = new Point((i == 0 ? 0 : panes[i - 1].Location.X + panes[i - 1].Size.Width), 0);
                }
                larghezzaNuovoPane += larghezzaRidotta;
            }
            if (larghezzaNuovoPane < LARGHEZZA_MINIMA_PANE)
                larghezzaNuovoPane = LARGHEZZA_MINIMA_PANE;

            noResize = true;
            Width += larghezzaNuovoPane;
            noResize = false;
            return AggiungiPane(versione, tipoTesto, larghezzaNuovoPane, confronta);
        }

        internal Pane AggiungiPane(string versione, TestoTipi tipoTesto, int larghezza)
        {
            return AggiungiPane(versione, tipoTesto, larghezza, false);
        }

        private Pane AggiungiPane(string versione, TestoTipi tipoTesto, int larghezza, bool confronta)
        {
            Splitter splitter = new Splitter
            {
                MinExtra = LARGHEZZA_MINIMA_PANE,
                MinSize = LARGHEZZA_MINIMA_PANE,
                Width = LARGHEZZA_SPLITTER
            };
            splitter.SplitterMoved += new SplitterEventHandler(Splitter_SplitterMoved);
            splitters.Add(splitter);
            panPanes.Controls.Add(splitter);
            splitter.Dock = DockStyle.Right;

            Pane nuovoPane = new Pane(this, versione, tipoTesto, larghezza, confronta);
            panes[0].RidimensionaComponenti(); // la routine non viene chiamata quando c'è una ridimensionata causata da Dock

            // TODO (C) confronta = true
            // TODO (C) salva il fatto con il pane è tipo confronta

            nuovoPane.RimuoviVisibile = true; // se è aggiunto attraverso il menu, per forza c'era già un altro pane, e questa non può essere il primo e quindi ha il pulsante Rimuovi
            panes[0].RimuoviVisibile = true; // se c'era solo un pane, adesso ce ne sono due e tutte e due sono rimovibili
            panes.Add(nuovoPane);
            // se c'è almeno un pane con il confronto, non possiamo più rimuovere il primo (fino a quando tutti quelli a confronto sono rimossi)
            bool almenoUnoConfronto = false;
            for (int i = 1; i < panes.Count; ++i)
            {
                if (panes[i].TipoConfronta)
                {
                    almenoUnoConfronto = true;
                    break;
                }
            }
            panes[0].RimuoviVisibile = !almenoUnoConfronto;

            paneAttivo = nuovoPane;
            ++ultimoTab;
            nuovoPane.SetTab(ultimoTab);
            if (panes.Count == massimoNumeroPane)
                pulAggiungi.Visible = false;
            ImpostaLarghezzaMinima();

            // per qualche motivo sconosciuto, quando si apre un commentario, eventuale evidenziazione di sottolineatura non è mostrata
            // (mentre lo è se si sposta il testo, e se la finestra è aperta all'avvio del programma)
            // la seguente riga costringe a finestra di ridisegnarsi, e la sottolineatura è mostrata
            // è anche nel costruttore di Visualizza, mentre non funziona se messa nel costruttore di Pane
            if (((Principale.testi.Info(versione).Tipo & TestoTipi.Commentario) == TestoTipi.Commentario) || ((Principale.testi.Info(versione).Tipo & TestoTipi.Dizionario) == TestoTipi.Dizionario))
                CambiaFormato();

            return nuovoPane;
        }

        private Pane AggiungiPaneConfronta(string p)
        {
            return AggiungiPane(p, TestoTipi.Bibbia, true);
        }

        private void ImpostaLarghezzaMinima()
        {
            noResize = true;
            MinimumSize = new Size(panes.Count * (LARGHEZZA_MINIMA_PANE + LARGHEZZA_SPLITTER) + LARGHEZZA_PULSANTI, MinimumSize.Height);
            noResize = false;
        }

        internal void ImpostaAltriTestiDaAggiungere(bool altriTesti)
        {
            pulAggiungi.Visible = altriTesti;
        }

        internal void SaltoIpertestuale()
        {
            paneAttivo.SaltoIpertestuale();
        }

        internal void ImpostaPulsantiVisibili(bool visibili)
        {
            panPulsanti.Visible = visibili;
            btnCanc.Visible = visibili;
            foreach (Pane p in panes)
                p.ImpostaPulsantiVisibili(visibili);
        }

        #endregion

        #region eventi

        private void Visualizza_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (Pane paneFiglio in panes)
            {
                bool daCancellare = paneFiglio.Pane_Closing();
                if (daCancellare)
                    e.Cancel = true;
            }

            // se != UserClosing, il programma è stato chiuso, e non serve cancellare tutti i panelli
            // inoltre, se i panelli sono chiusi, il programma non riesce a salvare l'ultima disposizione all'uscita,
            // perché FormClosing di una form figlia è chiamata prima di FormClosing della finestra principale
            if (e.CloseReason == CloseReason.UserClosing && !e.Cancel)
            {
                paneAttivo = null;
                for (int i = panes.Count - 1; i >= 0; --i)
                {
                    panes[i].Dispose();
                    panes[i] = null;
                    panes.RemoveAt(i);
                }
            }
        }

        private void Visualizza_Resize(object sender, EventArgs e)
        {
            if (!noResize)
            {
                panPanes.Width = Width - LARGHEZZA_PULSANTI;
                if (Width > larghezzaVecchia)
                {
                    int daAumentare = Width - larghezzaVecchia;
                    int aumentato = 0;
                    int panesRimasti = panes.Count;
                    foreach (Pane paneFiglio in panes)
                    {
                        paneFiglio.Size = new Size(paneFiglio.Size.Width + daAumentare / panesRimasti, ClientSize.Height);
                        paneFiglio.Location = new Point(paneFiglio.Location.X + aumentato, paneFiglio.Location.Y);
                        aumentato += daAumentare / panesRimasti;
                        daAumentare -= daAumentare / panesRimasti;
                        --panesRimasti;
                    }
                }
                else if (Width == larghezzaVecchia)
                {
                    foreach (Pane paneFiglio in panes)
                    {
                        paneFiglio.Size = new Size(paneFiglio.Size.Width, ClientSize.Height);
                    }
                }
                else // Width < larghezzaVecchia
                { // c'è codice simile in AggiungiTestoClick
                    int daRidurre = larghezzaVecchia - Width;
                    int numeroTentativi = 0;
                    while (daRidurre > 0 && numeroTentativi < panes.Count)
                    {
                        ++numeroTentativi;
                        int numeroPaneDaRidurre = panes.Count;
                        for (int i = 0; i < panes.Count; ++i)
                        {
                            if (panes[i].Size.Width == LARGHEZZA_MINIMA_PANE)
                                --numeroPaneDaRidurre;
                        }
                        for (int i = 0; i < panes.Count; ++i)
                        {
                            int larghezzaNuovaP;
                            if (numeroPaneDaRidurre == 0)
                                larghezzaNuovaP = panes[i].Size.Width;
                            else
                                larghezzaNuovaP = panes[i].Size.Width - (daRidurre / numeroPaneDaRidurre + ((daRidurre % numeroPaneDaRidurre) > 0 ? 1 : 0));
                            if (larghezzaNuovaP < LARGHEZZA_MINIMA_PANE)
                                larghezzaNuovaP = LARGHEZZA_MINIMA_PANE;
                            daRidurre -= panes[i].Size.Width - larghezzaNuovaP;
                            if (panes[i].Size.Width != larghezzaNuovaP)
                                --numeroPaneDaRidurre;
                            panes[i].Size = new Size(larghezzaNuovaP, ClientSize.Height);
                            panes[i].Location = new Point((i == 0 ? 0 : panes[i - 1].Location.X + panes[i - 1].Size.Width), 0);
                        }
                    }
                }
            }
            larghezzaVecchia = Width;
        }

        void Splitter_SplitterMoved(object sender, SplitterEventArgs e)
        {
            foreach (Pane paneFiglio in panes)
                paneFiglio.RidimensionaComponenti();
            // in realtà, basterebbe fare Resize solo dei due panes accanto allo splitter, ma più facile così
        }

        private void PmSincronizzato_Opening(object sender, CancelEventArgs e)
        {
            foreach (ToolStripItem tsi in pmSincronizzato.Items)
            {
                string sincNumero = "&" + paneAttivo.SincNumero.ToString(CultureInfo.InvariantCulture);
                if (sincNumero == "&0")
                    sincNumero = "&X";
                if (tsi.GetType().Name == "ToolStripMenuItem")
                {
                    ((ToolStripMenuItem)tsi).Checked = (tsi.Tag.ToString() == sincNumero);
                }
            }
        }

        private void PmSincSottomenu_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsi = (ToolStripMenuItem)sender;
            string sincString = tsi.Tag.ToString().Substring(1);
            int sincNumero = (sincString == "X" ? 0 : Convert.ToInt32(sincString, CultureInfo.InvariantCulture));
            paneAttivo.SincNumero = sincNumero;

            // cambiare il testo di questa finestra al versetto delle altre finestre sincronizzate con questa
            // c'è codice simile in SpostaAltreVisualizza (2 volte)
            if (sincString != "X")
            {
                foreach (Form formFiglio in MdiParent.MdiChildren)
                {
                    if (formFiglio.Tag != null && formFiglio.Tag.ToString() == "Visualizza")
                    {
                        foreach (Pane paneFiglio in ((Visualizza)formFiglio).panes)
                        {
                            if (paneFiglio.SincNumero == sincNumero && paneFiglio != paneAttivo)
                            {
                                if (paneFiglio.TipoTesto != TestoTipi.Dizionario && paneAttivo.TipoTesto != TestoTipi.Dizionario)
                                {
                                    paneAttivo.SpostaTesto(Principale.testi.ConvertiDaStandard(Principale.testi.ConvertiAStandard(new Riferimento(paneFiglio.Libro, paneFiglio.Capitolo, paneFiglio.Versetto), paneFiglio.Versione), paneAttivo.Versione), false);
                                    break;
                                }
                                if (paneFiglio.TipoTesto == TestoTipi.Dizionario && paneAttivo.TipoTesto == TestoTipi.Dizionario)
                                {
                                    paneAttivo.SpostaTesto(paneFiglio.Voce, false);
                                    break;
                                }
                            }
                        }
                    }
                }
                if (genitore.formProiettato != null && genitore.formProiettato.Tag != null && genitore.formProiettato.Tag.ToString() == "Visualizza")
                {
                    foreach (Pane paneFiglio in ((Visualizza)genitore.formProiettato).panes)
                    {
                        if (paneFiglio.SincNumero == sincNumero && paneFiglio != paneAttivo)
                        {
                            if (paneFiglio.TipoTesto != TestoTipi.Dizionario && paneAttivo.TipoTesto != TestoTipi.Dizionario)
                            {
                                paneAttivo.SpostaTesto(Principale.testi.ConvertiDaStandard(Principale.testi.ConvertiAStandard(new Riferimento(paneFiglio.Libro, paneFiglio.Capitolo, paneFiglio.Versetto), paneFiglio.Versione), paneAttivo.Versione), false);
                                break;
                            }
                            if (paneFiglio.TipoTesto == TestoTipi.Dizionario && paneAttivo.TipoTesto == TestoTipi.Dizionario)
                            {
                                paneAttivo.SpostaTesto(paneFiglio.Voce, false);
                                break;
                            }
                        }
                    }
                }
                paneAttivo.ImpostaTitolo(); // perché spostare il testo nelle altre finestre cambia i titoli mostrati
            }
        }

        private void PulRimuovi_Click(object sender, EventArgs e)
        {
            RimuoviPane((Pane)((((Button)sender).Parent).Tag));
        }

        private void RimuoviPane(Pane p)
        {
            if (!p.Pane_Closing())
            {
                int indice = panes.IndexOf(p);
                int larghezza = p.Size.Width;
                p.Rimuovi();
                //            for (int i = indice + 1; i < panes.Count; ++i)
                //                panes[i].Location = new Point(panes[i].Location.X - larghezza, panes[i].Location.Y);
                panes.Remove(p);
                p.Dispose();
                p = null;
                panPanes.Controls.Remove(splitters[indice > 0 ? indice : 1]);
                splitters.RemoveAt(indice);
                ImpostaLarghezzaMinima();
                noResize = true;
                this.Width = this.Width - larghezza;
                noResize = false;
                paneAttivo = (indice != panes.Count ? panes[indice] : panes[panes.Count - 1]);
                if (panes.Count == 1)
                    panes[0].RimuoviVisibile = false;
                else
                { // se c'è ancora un pane con un confronto con il primo pane, non possiamo rimuovere il primo
                    bool almenoUnoConfronto = false;
                    for (int i = 1; i < panes.Count; ++i)
                    {
                        if (panes[i].TipoConfronta)
                        {
                            almenoUnoConfronto = true;
                            break;
                        }
                    }
                    panes[0].RimuoviVisibile = !almenoUnoConfronto;
                }
                pulAggiungi.Visible = true;

                if (indice == 0) // il primo pane potrà avere una nuova lingua
                    AggiornaMenuConfronta();
            }
        }

        private void PulAggiungi_Click(object sender, EventArgs e)
        {
            pmAggiungi.Show(pulAggiungi, 0, pulAggiungi.Height - 1);
        }

        private void PulTestiParalleli_Click(object sender, EventArgs e)
        {
            List<string> versioni = new List<string>(panes.Count);
            foreach (Pane paneFiglio in panes)
                versioni.Add(paneFiglio.Versione + "#" + paneFiglio.TipoTesto.ToString().ToLowerInvariant()[0]);
            using (TestiParalleli formTestiParalleli = new TestiParalleli(genitore, versioni))
            {
                formTestiParalleli.ShowDialog();
                List<string> testi = formTestiParalleli.testi;
                List<TestoTipi> tipi = formTestiParalleli.tipiTestiSelezionati;
                if (testi.Count > 0) // altrimenti Annulla è stata cliccata
                {
                    int numeroPaneDaRimuovere = panes.Count;
                    // per non superare il numero massimo di panes e non rimuoverne il primo, bisogna rimuovere, aggiungere e rimuovere in questo modo
                    for (int i = numeroPaneDaRimuovere - 1; i > 0; --i)
                        RimuoviPane(panes[i]);
                    for (int i = 0; i < testi.Count; ++i)
                    {
                        AggiungiPane(testi[i], tipi[i]);
                        if (i == 0)
                            RimuoviPane(panes[0]);
                    }
                }
            }
        }

        private void BtnGuida_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, genitore.NomeFileGuida(), HelpNavigator.Topic, "viewbible.html");
            //Help.ShowHelp(this, genitore.NomeFileGuida(), HelpNavigator.Topic, tipoBibbia ? "viewbible.html" : "viewcommentary.html");
        }

        private void BtnCanc_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Sposta testo

        internal void SpostaTesto(byte nuovoLibro, byte nuovoCapitolo, byte nuovoVersetto, bool sincronizza)
        {
            paneAttivo.SpostaTesto(nuovoLibro, nuovoCapitolo, nuovoVersetto, sincronizza);
        }

        internal void SpostaTesto(string titolo, bool sincronizza)
        {
            paneAttivo.SpostaTesto(titolo, sincronizza);
        }

        internal void SpostaTesto(Riferimento riferimento, bool sincronizza)
        {
            paneAttivo.SpostaTesto(riferimento, sincronizza);
        }

        internal void CambiaFormato()
        {
            foreach (Pane paneFiglio in panes)
                paneFiglio.CambiaFormato();
        }

        #endregion

        #region highlight

        internal void HighlighterClick(Color colore, TipoHighlight tipo)
        {
            paneAttivo.Rtf.HighlighterClick(colore, tipo);
        }

        internal void HighlighterClick(byte tipoSottolineatura)
        {
            paneAttivo.Rtf.HighlighterClick(tipoSottolineatura);
        }

        internal void HighlighterNoneClick()
        {
            paneAttivo.Rtf.HighlighterNoneClick();
        }

        internal void HighlighterNoneNotMonoClick()
        {
            paneAttivo.Rtf.HighlighterNoneNotMonoClick();
        }

        /*
        internal void ImpostaHighlight(TipoHighlight tipoHighlight, Color colore)
        {
            paneAttivo.Rtf.ImpostaHighlight(tipoHighlight, colore);
        }

        internal void ImpostaHighlightSottolineatura(byte tipoSottolineatura)
        {
            paneAttivo.Rtf.ImpostaHighlightSottolineatura(tipoSottolineatura);
        }

        internal void ImpostaHighlightNone()
        {
            paneAttivo.Rtf.ImpostaHighlightNone();
        }
         */

        #endregion

        internal void ImpostaSincronizzato(bool daSincronizzare)
        {
            if (daSincronizzare)
            {
                bool[] numeriPresi = new bool[10];
                for (int i = 1; i <= 9; ++i)
                    numeriPresi[i] = false;
                foreach (Form formFiglio in MdiParent.MdiChildren)
                {
                    if (formFiglio.Tag != null && formFiglio != this && formFiglio.Tag.ToString() == "Visualizza")
                    {
                        foreach (Pane paneFiglio in ((Visualizza)formFiglio).panes)
                            numeriPresi[paneFiglio.SincNumero] = true;
                    }
                }

                int numeroMinimo = 1;
                for (int i = 1; i <= 9; ++i)
                {
                    if (!numeriPresi[i])
                    {
                        numeroMinimo = i;
                        break;
                    }
                }
                foreach (Pane p in panes)
                    p.SincNumero = numeroMinimo;
            }
            else
            {
                foreach (Pane p in panes)
                    p.SincNumero = 0;
            }
        }

        internal void ZoomPanes(float z)
        {
            foreach (Pane p in panes)
                p.Rtf.ZoomFactor *= z;
        }
    }
}