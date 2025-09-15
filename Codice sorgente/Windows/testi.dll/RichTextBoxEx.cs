using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace TestiBiblici
{
    /// <summary>
    /// An extension for RichTextBox suitable for printing, formatting selections, justifying text
    /// </summary>
    [ComVisible(false)]
    public class RichTextBoxEx : RichTextBox
    {
        #region proprietà

        /// <summary>
        /// Le otto cifre che seguono danno il riferimento del versetto che segue.
        /// </summary>
        public const char InizioRiferimento = (char)1;
        /// <summary>
        /// Il carattere inserito per indicare l'inizio di un link ipertestuale.
        /// </summary>
        public const char InizioLink = (char)2;
        /// <summary>
        /// Il carattere inserito per indicare l'inizio della parte finale un link ipertestuale.
        /// </summary>
        public const char FineLink1 = (char)3;
        /// <summary>
        /// Il carattere inserito per indicare la fine della parte finale un link ipertestuale.
        /// </summary>
        public const char FineLink2 = (char)4;
        /// <summary>
        /// Il carattere inserito per indicare la fine di un link ipertestuale ad un brano.
        /// </summary>
        public const char FineLinkBrano = (char)5;
        /// <summary>
        /// Il carattere inserito per indicare la fine di un link ipertestuale ad una nota.
        /// </summary>
        public const char FineLinkNota = (char)6;
        /// <summary>
        /// Il carattere inserito per indicare la fine di un link ipertestuale ad un file.
        /// </summary>
        public const char FineLinkFile = (char)7;
        /// <summary>
        /// Il carattere inserito per indicare l'inizio di una parola ricercata.
        /// </summary>
        public const char ParolaRicercata = (char)14;

        /// <summary>
        /// Il testo Rtf del controllo.
        /// </summary>
        new public string Rtf
        {
            get { return base.Rtf; }
            set
            {
                // necessario fare così perché c'è un bug in .NET
                // altrimenti lo zoom è impostato su 1.0
                float vecchioZoom = ZoomFactor;
                ZoomFactor = 1.0F;
                base.Rtf = ConvertiUnicodeInRtf(value);
                ZoomFactor = vecchioZoom;
            }
        }

        /// <summary>
        /// Il testo Rtf del testo selezionato.
        /// </summary>
        new public string SelectedRtf
        {
            get { return base.SelectedRtf; }
            set
            {
                base.SelectedRtf = ConvertiUnicodeInRtf(value);
            }
        }

        /// <summary>
        /// Seleziona tutto il testo.
        /// </summary>
        new public void SelectAll()
        {
            base.SelectAll(); // a volte non funziona, perché i caratteri con ASCII<16 inseriti per inizio riferimento eccetera fanno sì che niente sia selezionato
            if (SelectionLength == 0 && Text.Length > 0)
            {
                int lunghezza = Text.Length;
                for (int i = 1; i <= 10; ++i)
                {
                    Select(i, lunghezza);
                    if (SelectionLength > 0 && i < lunghezza)
                        break;
                }
            }
        }

        private bool hoverNotificato = false;
        private Form fIpertesto;
        private Point ultimoHover = new Point(-999, -999);
        internal static bool isRunningOnMono;
        private CHARFORMAT charFormato = new CHARFORMAT();

        private enum StatoFinestraIpertesto
        {
            Antenato, NonUtilizzato, Utilizzato
        }

        private StatoFinestraIpertesto statoFinestra = StatoFinestraIpertesto.Antenato;
        private StatoFinestraIpertesto StatoFinestra
        {
            get { return statoFinestra; }
            set { statoFinestra = value; }
        }

        /// <summary>
        /// Se il controllo rivela e visualizza gli url automaticamente.
        /// </summary>
        [System.ComponentModel.DefaultValue(false)]
        public new bool DetectUrls
        {
            get { return base.DetectUrls; }
            set { base.DetectUrls = value; }
        }

        private string versione;
        /// <summary>
        /// La versione della Bibbia del testo nel controllo.
        /// </summary>
        public string Versione
        {
            get { return versione; }
            set { versione = value; }
        }

        private string lingua;
        /// <summary>
        /// La lingua del testo nel controllo (o lingue, se separate con una riga verticale |).
        /// </summary>
        public string Lingua
        {
            get { return lingua; }
            set { lingua = value; }
        }

        #endregion

        #region const e struct

        #region per AggiungiRtf

        internal delegate IntPtr EditStreamCallback(IntPtr dwCookie, IntPtr pbBuff, IntPtr cb, out IntPtr pcb);

        [StructLayout(LayoutKind.Sequential)]
        internal struct EDITSTREAM
        {
            public IntPtr dwCookie;
            public IntPtr dwError;
            public EditStreamCallback pfnCallback;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MEMORYSTREAM
        {
            public MemoryStream memoryStream;
        }

        private const int SFF_SELECTION = 0x8000;
        private const int SF_TEXT = 2;
        private const int SF_RTF = 2;
        private const int SF_UNICODE = 2;
        private const int EM_STREAMIN = 1097; // WM_USER + 73;

        #endregion

        #region per i link

        private const int distanzaNonRipetereHover = 10;

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSG
        {
            public IntPtr hwnd;
            public int message;
            public int wParam;
            public IntPtr lParam;
            public uint time;
            public POINT pt;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NMHDR
        {
            public IntPtr hwndFrom;
            public int idFrom;
            public int code;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CHARRANGE
        {
            public int cpMin;
            public int cpMax;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ENLINK
        {
            public NMHDR nmhdr;
            public int msg;
            public int wParam;
            public IntPtr lParam;
            public CHARRANGE chrg;
        }

        #endregion

        #region per la giustificazione

        /// <summary>
        /// Specifies how text in a RichText control is horizontally aligned.
        /// </summary>
        [ComVisible(false)]
        public enum TextAlign
        {
            /// <summary>
            /// The text has no alignment assigned.
            /// </summary>
            None = 0,
            /// <summary>
            /// The text is aligned to the left.
            /// </summary>
            Left = 1,
            /// <summary>
            /// The text is aligned to the right.
            /// </summary>
            Right = 2,
            /// <summary>
            /// The text is aligned in the center.
            /// </summary>
            Center = 3,
            /// <summary>
            /// The text is justified.
            /// </summary>
            Justify = 4
        }

        #endregion

        #region per il font

        private const int MAX_LUNGHEZZA_FONT_NOME = 32;

        //        private const int WM_USER = 0x0400;

        private const int WM_NOTIFY = 0x004E;
        private const int WM_SETCURSOR = 0x0020;
        private const int WM_MOUSEMOVE = 0x0200;

        private const int EM_GETCHARFORMAT = 1082; // WM_USER + 58;
        private const int EM_SETCHARFORMAT = 1092;

        private const int EM_GETEVENTMASK = 1083; // WM_USER + 59;
        private const int EM_SETEVENTMASK = 1093;

        private const UInt32 CFM_BOLD = 0x00000001;
        private const UInt32 CFM_ITALIC = 0x00000002;
        private const UInt32 CFM_UNDERLINE = 0x00000004;
        private const UInt32 CFM_STRIKEOUT = 0x00000008;
        private const UInt32 CFM_PROTECTED = 0x00000010;
        private const UInt32 CFM_LINK = 0x00000020;
        private const UInt32 CFM_UNDERLINETYPE = 0x00800000;
        private const UInt32 CFM_CHARSET = 0x08000000;
        private const UInt32 CFM_OFFSET = 0x10000000;
        private const UInt32 CFM_FACE = 0x20000000;
        private const UInt32 CFM_COLOR = 0x40000000;
        private const UInt32 CFM_SIZE = 0x80000000;

        private const UInt32 ENM_LINK = 0x04000000;
        private const UInt32 EN_LINK = 0x070b;

        private const int SCF_SELECTION = 1;

        [StructLayout(LayoutKind.Sequential)]
        private struct CHARFORMAT
        {
            public int cbSize;
            public uint dwMask;
            public uint dwEffects;
            public int yHeight;
            public int yOffset;
            public int crTextColor;
            public byte bCharSet;
            public byte bPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_LUNGHEZZA_FONT_NOME)]
            public char[] szFaceName;

            // CHARFORMAT2 from here onwards.
            public short wWeight;
            public short sSpacing;
            public int crBackColor; // Color.ToArgb() -> int
            public int LCID;
            public uint dwReserved;
            public short sStyle;
            public short wKerning;
            public byte bUnderlineType;
            public byte bAnimation;
            public byte bRevAuthor;
        }

        #endregion

        #region per la stampa
        //        private const int WM_USER = 0x400;
        private const int EM_FORMATRANGE = 1081; // WM_USER + 57;

        [StructLayout(LayoutKind.Sequential)]
        private struct STRUCT_RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct STRUCT_CHARRANGE
        {
            public int cpMin;
            public int cpMax;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct STRUCT_FORMATRANGE
        {
            public IntPtr hdc;
            public IntPtr hdcTarget;
            public STRUCT_RECT rc;
            public STRUCT_RECT rcPage;
            public STRUCT_CHARRANGE chrg;
        }

        #endregion

        #endregion

        /// <summary>
        /// Il costruttore.
        /// </summary>
        public RichTextBoxEx()
        {
            // Otherwise, non-standard links get lost when user starts typing next to a non-standard link
            this.DetectUrls = false;
            isRunningOnMono = (Type.GetType("Mono.Runtime") != null);
            if (!isRunningOnMono)
                CostruttoreNotMono();
            MouseLeave += new EventHandler(RichTextBoxEx_MouseLeave);
            MouseMove += new MouseEventHandler(RichTextBoxEx_MouseMove);
        }

        private void CostruttoreNotMono()
        {
            int eventMask = (int)(SafeNativeMethods.SendMessage(Handle, EM_GETEVENTMASK, (IntPtr)0, (IntPtr)0));
            SafeNativeMethods.SendMessage(Handle, EM_SETEVENTMASK, (IntPtr)0, (IntPtr)(eventMask | ENM_LINK));
        }

        private static string ConvertiUnicodeInRtf(string rtf)
        {
            // StripRtf in testi.cs fa parzialmente il contrario
            if (string.IsNullOrEmpty(rtf))
                return "";

            int numeroCaratteri = rtf.Length;
            StringBuilder rtfSB = new StringBuilder(numeroCaratteri * 7);
            for (int i = 0; i < numeroCaratteri; ++i)
            {
                if (rtf[i] >= 256)
                    rtfSB.Append(@"\u" + Convert.ToUInt32(rtf[i]).ToString(CultureInfo.InvariantCulture) + "?");
                else if (rtf[i] >= 128)
                    rtfSB.Append(@"\'" + Uri.HexEscape(rtf[i]).Substring(1));
                else
                    rtfSB.Append(rtf[i]);
            }
            return rtfSB.ToString();

            /* questo metodo è un po' più lento (50% più tempo)
                        for (int i = 0; i < rtf.Length; ++i)
                        {
                            if (rtf[i] >= 256)
                                rtf = rtf.Replace(rtf[i].ToString(CultureInfo.InvariantCulture), @"\u" + Convert.ToUInt32(rtf[i]).ToString(CultureInfo.InvariantCulture) + "?");
                            // else if (rtf[i] > 128)
                            //   rtf = rtf.Replace(rtf[i].ToString(), @"\'"+Uri.HexEscape(rtf[i]).Substring(1));
                        }
                        return rtf;*/
        }

        #region AggiungiRtf

        /// <summary>
        /// Aggiunge del testo RTF (cioè {\rtf...}) alla fine del controllo.
        /// </summary>
        /// <param name="testoRtfDaAggiungere">Il testo da aggiungere.</param>
        public void AggiungiRtf(string testoRtfDaAggiungere)
        {
            if (string.IsNullOrEmpty(testoRtfDaAggiungere))
                return;

            SelectionStart = Text.Length;
            if (isRunningOnMono)
                SelectedRtf = testoRtfDaAggiungere;
            else
                AggiungiRtfNotMono(testoRtfDaAggiungere);
        }

        private void AggiungiRtfNotMono(string testoRtfDaAggiungere)
        {
            MEMORYSTREAM msStruttura = new MEMORYSTREAM
            {
                memoryStream = new MemoryStream(new ASCIIEncoding().GetBytes(ConvertiUnicodeInRtf(testoRtfDaAggiungere)))
                {
                    Position = 0
                }
            };
            IntPtr cookie = Marshal.AllocCoTaskMem(Marshal.SizeOf(msStruttura));
            Marshal.StructureToPtr(msStruttura, cookie, false);

            int formato = SFF_SELECTION | SF_RTF;
            EDITSTREAM es = new EDITSTREAM
            {
                dwCookie = cookie,
                dwError = IntPtr.Zero,
                pfnCallback = new EditStreamCallback(StreamIn)
            };

            IntPtr lParam = Marshal.AllocCoTaskMem(Marshal.SizeOf(es));
            Marshal.StructureToPtr(es, lParam, false);

            //            System.Windows.Forms.MessageBox.Show("a");
            SafeNativeMethods.SendMessage(new HandleRef(this, this.Handle), (Int32)EM_STREAMIN, (IntPtr)formato, ref es);
            //SafeNativeMethods.SendMessage(Handle, EM_STREAMIN, (IntPtr)formato, lParam);

            Marshal.FreeCoTaskMem(cookie);
            Marshal.FreeCoTaskMem(lParam);

            if (es.dwError.ToInt32() != 0)
            {
                //                System.Windows.Forms.MessageBox.Show("Errore 2: " + es.dwError.ToString());
                throw new FormatException("Errore aggiungendo testo al controllo RTF. Error adding text to the RTF control.");
            }
        }

        static IntPtr StreamIn(IntPtr dwCookie, IntPtr pbBuff, IntPtr cb, out IntPtr pcb)
        {
            //            System.Windows.Forms.MessageBox.Show("b");
            MEMORYSTREAM dati = new MEMORYSTREAM();
            dati = (MEMORYSTREAM)(Marshal.PtrToStructure(dwCookie, typeof(MEMORYSTREAM)));
            //            System.Windows.Forms.MessageBox.Show(dati.memoryStream.Length.ToString());
            byte[] byteArray = new byte[cb.ToInt32()];
            //            IntPtr unmanagedArray = Marshal.AllocHGlobal(cb);

            uint risultato = 0;
            pcb = (IntPtr)0;
            try
            {
                //int numeroByte = dati.memoryStream.Read(byteArray, 0, cb.ToInt32());
                //                System.Windows.Forms.MessageBox.Show(numeroByte.ToString());
                pcb = (IntPtr)(dati.memoryStream.Read(byteArray, 0, cb.ToInt32()));
                Marshal.Copy(byteArray, 0, pbBuff, pcb.ToInt32());
                //                System.Windows.Forms.MessageBox.Show(numeroByte.ToString());
            }
            catch
            {
                //                System.Windows.Forms.MessageBox.Show("Errore 1: " + exc.Message);
                pcb = IntPtr.Zero;
                risultato = 1;
            }
            return (IntPtr)risultato;
        }

        #endregion

        #region stampa

        private void SetCharFormatMessageNotMono(ref CHARFORMAT formato)
        {
            IntPtr lParam = Marshal.AllocCoTaskMem(Marshal.SizeOf(formato));
            Marshal.StructureToPtr(formato, lParam, false);
            SafeNativeMethods.SendMessage(Handle, EM_SETCHARFORMAT, (IntPtr)SCF_SELECTION, (IntPtr)lParam);
            Marshal.FreeCoTaskMem(lParam);
        }

        /// <summary>
        /// Calculate or render the contents of our RichTextBox for printing
        /// </summary>
        /// <param name="measureOnly">If true, only the calculation is performed, otherwise the text is rendered as well</param>
        /// <param name="e">The PrintPageEventArgs object from the PrintPage event</param>
        /// <param name="primoCarattere">Index of first character to be printed</param>
        /// <param name="ultimoCarattere">Index of last character to be printed</param>
        /// <returns>(Index of last character that fitted on the page) + 1</returns>
        public int FormatRangeNotMono(bool measureOnly, PrintPageEventArgs e, int primoCarattere, int ultimoCarattere)
        {
            if (e == null)
                throw new ArgumentNullException("e");

            // Specify which characters to print
            STRUCT_CHARRANGE cr;
            cr.cpMin = primoCarattere;
            cr.cpMax = ultimoCarattere;

            // Specify the area inside page margins
            STRUCT_RECT rettangoloZonaStampabile;
            rettangoloZonaStampabile.top = HundredthInchToTwips(e.MarginBounds.Top);
            rettangoloZonaStampabile.bottom = HundredthInchToTwips(e.MarginBounds.Bottom);
            rettangoloZonaStampabile.left = HundredthInchToTwips(e.MarginBounds.Left);
            rettangoloZonaStampabile.right = HundredthInchToTwips(e.MarginBounds.Right);

            // Specify the page area
            STRUCT_RECT rettangoloPagina;
            rettangoloPagina.top = HundredthInchToTwips(e.PageBounds.Top);
            rettangoloPagina.bottom = HundredthInchToTwips(e.PageBounds.Bottom);
            rettangoloPagina.left = HundredthInchToTwips(e.PageBounds.Left);
            rettangoloPagina.right = HundredthInchToTwips(e.PageBounds.Right);

            // Get device context of output device
            IntPtr hdc = e.Graphics.GetHdc();

            // Fill in the FORMATRANGE struct
            STRUCT_FORMATRANGE fr;
            fr.chrg = cr;
            fr.hdc = hdc;
            fr.hdcTarget = hdc;
            fr.rc = rettangoloZonaStampabile;
            fr.rcPage = rettangoloPagina;

            // Non-Zero lParam means render, Zero means measure
            int wParam = (measureOnly ? 0 : 1);

            // Allocate memory for the FORMATRANGE struct and
            // copy the contents of our struct to this memory
            IntPtr lParam = Marshal.AllocCoTaskMem(Marshal.SizeOf(fr));
            Marshal.StructureToPtr(fr, lParam, false);

            // Send the actual Win32 message
            int res = (int)(SafeNativeMethods.SendMessage(Handle, EM_FORMATRANGE, (IntPtr)wParam, lParam));

            // Free allocated memory
            Marshal.FreeCoTaskMem(lParam);

            // and release the device context
            e.Graphics.ReleaseHdc(hdc);

            return res;
        }

        /// <summary>
        /// Convert between 1/100 inch (unit used by the .NET framework)
        /// and twips (1/1440 inch, used by Win32 API calls)
        /// </summary>
        /// <param name="n">Value in 1/100 inch</param>
        /// <returns>Value in twips</returns>
        private static int HundredthInchToTwips(int n)
        {
            return (int)(n * 14.4);
        }

        /// <summary>
        /// Free cached data from rich edit control after printing
        /// </summary>
        public void FormatRangeDoneNotMono()
        {
            IntPtr lParam = new IntPtr(0);
            SafeNativeMethods.SendMessage(Handle, EM_FORMATRANGE, (IntPtr)0, lParam);
        }

        #endregion

        #region font

        /// <summary>
        /// Imposta il font della selezione.
        /// </summary>
        /// <param name="nomeFont">Il nome del font.</param>
        public void SetFont(string nomeFont)
        {
            if (nomeFont == null)
                throw new ArgumentNullException("nomeFont");

            //CHARFORMAT charFormato = new CHARFORMAT();
            charFormato.cbSize = Marshal.SizeOf(charFormato);
            charFormato.dwMask = CFM_FACE;
            charFormato.szFaceName = new char[MAX_LUNGHEZZA_FONT_NOME];
            for (int i = 0; i < nomeFont.Length; ++i)
                charFormato.szFaceName[i] = nomeFont[i];
            SetCharFormatMessageNotMono(ref charFormato);
        }

        /// <summary>
        /// Imposta la dimensione del font della selezione.
        /// </summary>
        /// <param name="size">La dimensione da impostare.</param>
        public void SetSize(Single size)
        {
            //CHARFORMAT charFormato = new CHARFORMAT();
            charFormato.cbSize = Marshal.SizeOf(charFormato);
            charFormato.dwMask = CFM_SIZE;
            charFormato.yHeight = (int)(size * 20);
            SetCharFormatMessageNotMono(ref charFormato);
        }

        /// <summary>
        /// Cambia se la selezione è in grassetto.
        /// </summary>
        /// <param name="set">True lo imposta, false lo toglie.</param>
        public void SetSelectionBold(bool set)
        {
            ApplyStyle(CFM_BOLD, set);
        }

        /// <summary>
        /// Cambia se la selezione è in corsivo.
        /// </summary>
        /// <param name="set">True lo imposta, false lo toglie.</param>
        public void SetSelectionItalic(bool set)
        {
            ApplyStyle(CFM_ITALIC, set);
        }

        /// <summary>
        /// Cambia se la selezione è sottolineata.
        /// </summary>
        /// <param name="set">True lo imposta, false lo toglie.</param>
        public void SetSelectionUnderline(bool set)
        {
            ApplyStyle(CFM_UNDERLINE, set);
        }

        /// <summary>
        /// Cambia il tipo di sottolineatura.
        /// </summary>
        /// <param name="underlineType">Il tipo di sottolineatura.</param>
        public void SetSelectionUnderlineTypeNotMono(byte underlineType)
        {
            ApplyStyleUnderlineNotMono(CFM_UNDERLINETYPE, underlineType);
        }

        /// <summary>
        /// Set the current selection's link style
        /// </summary>
        /// <param name="link">true: set link style, false: clear link style</param>
        public void SetSelectionLink(bool link)
        {
            ApplyStyle(CFM_LINK, link);
        }

        /*
        /// <summary>
        /// Get the link style for the current selection
        /// </summary>
        /// <returns>0: link style not set, 1: link style set, -1: mixed</returns>
        public int GetSelectionLink()
        {
            return (isRunningOnMono ? 0 : GetSelectionStyleNotMono(CFM_LINK, CFM_LINK));
        }
         */

        /// <summary>
        /// Change the selected text to a link. The link text is followed by a @ symbol,
        /// a letter indicating the type of link, and the given hyperlink text, all of them invisible.
        /// When clicked on, the whole link text and hyperlink string are given in the LinkClickedEventArgs.
        /// </summary>
        /// <param name="link">Invisible hyperlink string to be inserted</param>
        /// <param name="tipo">The type of link: FineLinkBrano->Bible passage, FineLinkNota->nota, FineLinkFile->file</param>
        public void InserisciLink(string link, char tipo)
        {
            if (link == null)
                throw new ArgumentNullException("link");

            int selezioneInizio = this.SelectionStart;
            int selezioneLunghezza = this.SelectionLength;
            this.Select(selezioneInizio + selezioneLunghezza, 0);
            link = link.Replace(@"\", @"\\");
            this.SelectedRtf = @"{\rtf1\ansi{\v " + FineLink1 + tipo + link + FineLink2 + @"}}";
            this.Select(selezioneInizio, 0);
            this.SelectedRtf = @"{\rtf1\ansi{\v " + InizioLink + @"}}";
            ImpostaFormatoDelLink(selezioneInizio, 1 + selezioneLunghezza + link.Length + 3);
        }

        /// <summary>
        /// Cambia il formato del testo per visualizzare tutti i link.
        /// </summary>
        public void MostraLink()
        {
            int posizione = 0;
            int selezioneInizio = SelectionStart;
            int selezioneLunghezza = SelectionLength;
            bool modificato = Modified;
            int posizioneInizio = this.Text.IndexOf(InizioLink, posizione);
            BloccaRtf(true);
            try
            {
                //                while (this.Text.IndexOf((InizioLink.ToString(), posizione, StringComparison.Ordinal) >= posizione)
                while (posizioneInizio >= posizione)
                {
                    //                    posizioneInizio = this.Text.IndexOf(InizioLink.ToString(), posizione, StringComparison.Ordinal);
                    //posizioneInizio = this.Text.IndexOf(InizioLink, posizione);
                    //                    linkFine = this.Text.IndexOf(FineLink2.ToString(), posizioneInizio, StringComparison.Ordinal);
                    posizione = this.Text.IndexOf(FineLink2, posizioneInizio);

                    if (posizione >= posizioneInizio)
                        ImpostaFormatoDelLink(posizioneInizio, posizione - posizioneInizio + 1);
                    posizioneInizio = this.Text.IndexOf(InizioLink, posizione);
                }
            }
            finally
            {
                if (selezioneLunghezza >= 0)
                    Select(selezioneInizio, selezioneLunghezza);
                Modified = modificato;
                BloccaRtf(false);
            }
        }

        private void ImpostaFormatoDelLink(int selezioneInizio, int selezioneLunghezza)
        {
            if (selezioneInizio == 0)
            { // necessario fare in questo modo, perché Select(0,...) non funziona se il testo inizia con testo nascosto
                // nota che quando una nota è aperta e questa routine inserisce i link, il {BS} non funziona. Così uno spazio è sempre inserito all'inizio se c'è un link all'inizio.
                this.SelectionStart = 0;
                this.SelectedRtf = @"{\rtf1\ansi  }";
                this.Select(1, selezioneLunghezza);
                this.SetSelectionLink(true);
                this.Select(1, 0);
                SendKeys.Send("{BS}");
            }
            else
            {
                this.Select(selezioneInizio, selezioneLunghezza);
                this.SetSelectionLink(true);
            }
        }

        /*
        private int GetSelectionStyleNotMono(UInt32 mask, UInt32 effect)
        {
            //CHARFORMAT charFormato = new CHARFORMAT();
            charFormato.cbSize = Marshal.SizeOf(charFormato);
            charFormato.szFaceName = new char[32];

            IntPtr lParam = Marshal.AllocCoTaskMem(Marshal.SizeOf(charFormato));
            Marshal.StructureToPtr(charFormato, lParam, false);

            //            int res = (int)(SafeNativeMethods.SendMessage(Handle, EM_GETCHARFORMAT, (IntPtr)SCF_SELECTION, lParam));

            charFormato = (CHARFORMAT)Marshal.PtrToStructure(lParam, typeof(CHARFORMAT));

            int state;
            // dwMask holds the information which properties are consistent throughout the selection:
            if ((charFormato.dwMask & mask) == mask)
            {
                if ((charFormato.dwEffects & effect) == effect)
                    state = 1;
                else
                    state = 0;
            }
            else
            {
                state = -1;
            }

            Marshal.FreeCoTaskMem(lParam);
            return state;
        }
         */

        private void ApplyStyle(UInt32 style, bool on)
        {
            ApplyStyle(style, on ? style : 0);
        }

        private void ApplyStyle(UInt32 style, UInt32 effect)
        {
            if (!isRunningOnMono)
            {
                //CHARFORMAT charFormato = new CHARFORMAT();
                charFormato.cbSize = Marshal.SizeOf(charFormato);
                charFormato.dwMask = style;
                charFormato.dwEffects = effect;

                SetCharFormatMessageNotMono(ref charFormato);
            }
        }

        private void ApplyStyleUnderlineNotMono(UInt32 style, byte underlineType)
        {
            if (!isRunningOnMono)
            {
                //CHARFORMAT charFormato = new CHARFORMAT();
                charFormato.cbSize = Marshal.SizeOf(charFormato);
                charFormato.dwMask = style;
                charFormato.bUnderlineType = underlineType;

                SetCharFormatMessageNotMono(ref charFormato);
            }
        }

        #endregion

        #region giustificazione

        /// <summary>
        /// Gets or sets the alignment to apply to the current
        /// selection or insertion point.
        /// </summary>
        /// <remarks>
        /// Replaces the SelectionAlignment from <see cref="RichTextBox"/>.
        /// </remarks>
        public new TextAlign SelectionAlignment
        {
            get
            {
                if (!isRunningOnMono)
                    return GetSelectionAlignmentNotMono();
                else
                {
                    switch (base.SelectionAlignment)
                    {
                        case HorizontalAlignment.Center:
                            return TextAlign.Center;
                        case HorizontalAlignment.Right:
                            return TextAlign.Right;
                        default:
                            return TextAlign.Left; // for Left and Justified;
                    }
                }
            }
            set
            {
                if (!isRunningOnMono)
                    SetSelectionAlignmentNotMono(value);
                else
                {
                    switch (value)
                    {
                        case TextAlign.Right:
                            base.SelectionAlignment = HorizontalAlignment.Right;
                            break;
                        case TextAlign.Center:
                            base.SelectionAlignment = HorizontalAlignment.Center;
                            break;
                        default: // Left and Justified
                            base.SelectionAlignment = HorizontalAlignment.Left;
                            break;
                    }
                }
            }
        }

        private void SetSelectionAlignmentNotMono(TextAlign value)
        {
            PARAFORMAT fmt = new PARAFORMAT();
            fmt.cbSize = Marshal.SizeOf(fmt);
            fmt.dwMask = PFM_ALIGNMENT;
            fmt.wAlignment = (short)value;

            // Set the alignment.
            SafeNativeMethods.SendMessage(Handle, EM_SETPARAFORMAT, (IntPtr)SCF_SELECTION, ref fmt);
        }

        private TextAlign GetSelectionAlignmentNotMono()
        {
            PARAFORMAT fmt = new PARAFORMAT();
            fmt.cbSize = Marshal.SizeOf(fmt);

            // Get the alignment.
            SafeNativeMethods.SendMessage(Handle, EM_GETPARAFORMAT, (IntPtr)SCF_SELECTION, ref fmt);

            // Default to Left align.
            if ((fmt.dwMask & PFM_ALIGNMENT) == 0)
                return TextAlign.Left;

            return (TextAlign)fmt.wAlignment;
        }

        /// <summary>
        /// This member overrides
        /// <see cref="Control"/>.OnHandleCreated.
        /// </summary>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!isRunningOnMono)
                AbilitaGiustificazioneNotMono();
        }

        private void AbilitaGiustificazioneNotMono()
        {
            // Enable support for justification.
            SafeNativeMethods.SendMessage(Handle, EM_SETTYPOGRAPHYOPTIONS, (IntPtr)TO_ADVANCEDTYPOGRAPHY, (IntPtr)TO_ADVANCEDTYPOGRAPHY);
        }

        // Constants from the Platform SDK.
        private const int EM_GETPARAFORMAT = 1085;
        private const int EM_SETPARAFORMAT = 1095;
        private const int EM_SETTYPOGRAPHYOPTIONS = 1226;
        private const int WM_SETREDRAW = 11;
        private const int TO_ADVANCEDTYPOGRAPHY = 1;
        private const int PFM_ALIGNMENT = 8;
        //    private const int SCF_SELECTION = 1; // già definito nella sezione sugli stili

        // It makes no difference if we use PARAFORMAT or
        // PARAFORMAT2 here, so I have opted for PARAFORMAT2.
        [StructLayout(LayoutKind.Sequential)]
        internal struct PARAFORMAT
        {
            public int cbSize;
            public uint dwMask;
            public short wNumbering;
            public short wReserved;
            public int dxStartIndent;
            public int dxRightIndent;
            public int dxOffset;
            public short wAlignment;
            public short cTabCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public int[] rgxTabs;

            // PARAFORMAT2 from here onwards.
            public int dySpaceBefore;
            public int dySpaceAfter;
            public int dyLineSpacing;
            public short sStyle;
            public byte bLineSpacingRule;
            public byte bOutlineLevel;
            public short wShadingWeight;
            public short wShadingStyle;
            public short wNumberingStart;
            public short wNumberingStyle;
            public short wNumberingTab;
            public short wBorderSpace;
            public short wBorderWidth;
            public short wBorders;
        }

        #endregion

        #region bloccare

        private int nBlocchi = 0;
        private IntPtr oldEventMask = (IntPtr)0;

        /// <summary>
        /// Impedisce che il controllo sia aggiornato.
        /// Siccome diversi aggiornamenti, e quindi blocchi, possono essersi innestati, la funzione conta il numero di blocchi
        ///    e sblocca il controllo solo quando l'ultimo sblocco è rimosso.
        /// Per questo motivo, è importante sempre chiamare questa funzione due volte, per bloccare e poi sbloccare.
        /// </summary>
        /// <param name="blocca">True per bloccare, false per sbloccare.</param>
        public void BloccaRtf(bool blocca)
        {
            // blocca (se bBlocca è vero) o sblocca (se bBlocca è falso) il controlla rtTesto
            if (!isRunningOnMono)
                BloccaNotMono(blocca);
        }

        private void BloccaNotMono(bool blocca)
        {
            if (blocca)
            {
                nBlocchi++;
                if (nBlocchi == 1)
                {
                    // Prevent the control from raising any events.
                    oldEventMask = SafeNativeMethods.SendMessage(Handle, EM_GETEVENTMASK, (IntPtr)0, (IntPtr)0);
                    SafeNativeMethods.SendMessage(Handle, EM_SETEVENTMASK, (IntPtr)0, (IntPtr)0);
                    // Prevent the control from redrawing itself.
                    SafeNativeMethods.SendMessage(Handle, WM_SETREDRAW, (IntPtr)0, (IntPtr)0);
                }
            }
            else
            {
                nBlocchi--;
                if (nBlocchi <= 0)
                {
                    // Allow the control to redraw itself.
                    SafeNativeMethods.SendMessage(Handle, WM_SETREDRAW, (IntPtr)1, (IntPtr)0);
                    // Allow the control to raise event messages.
                    SafeNativeMethods.SendMessage(Handle, EM_SETEVENTMASK, (IntPtr)0, oldEventMask);
                    nBlocchi = 0;
                    Invalidate();
                }
            }
        }

        #endregion

        #region link hover

        /// <summary>
        /// Analizza un messaggio di Windows.
        /// </summary>
        /// <param name="m">Il messaggio</param>
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x2000 + WM_NOTIFY:
                    try
                    {
                        if (((NMHDR)Marshal.PtrToStructure(m.LParam, typeof(NMHDR))).code == EN_LINK)
                        {
                            ENLINK enLink = (ENLINK)Marshal.PtrToStructure(m.LParam, typeof(ENLINK));
                            if (enLink.msg == WM_MOUSEMOVE || enLink.msg == WM_SETCURSOR && !hoverNotificato)
                            { // hoverNotificato impedisce due messaggi consecutivi, per esempio SETCURSOR e MOUSEMOVE uno dopo l'altro
                                string link = Text.Substring(enLink.chrg.cpMin, enLink.chrg.cpMax - enLink.chrg.cpMin);
                                if (!string.IsNullOrEmpty(link) && link[0] == InizioLink && link[link.Length - 1] == FineLink2)
                                {
                                    link = link.Remove(link.Length - 1).Remove(0, 1);
                                    LinkHoverEventArgs e = new LinkHoverEventArgs(this, link);
                                    OnLinkHover(e);
                                    hoverNotificato = true;
                                }
                            }
                        }
                    }
                    catch { } // non fare hover se c'è stato qualche problema
                    break;
            }
            hoverNotificato = false;
            try
            {
                base.WndProc(ref m);
            }
            catch (AccessViolationException)
            {
                // su un computer (con Vista Ultimate 64 bit) c'era un errore, che forse possiamo ignorare
            }
        }

        /// <summary>
        /// Mostra il testo ipertestuale quando il mouse è sopra un link.
        /// </summary>
        /// <param name="testo">Il testo da visualizzare.</param>
        /// <param name="versioneDelTesto">La nomeVersione della Bibbia del testo.</param>
        /// <param name="posizione">La posizione del mouse sul controllo.</param>
        /// <param name="mostraInTooltip">Se il testo dovrà essere visualizzato anche nelle finestre dell'ipertesto.</param>
        public void MostraHover(string testo, string versioneDelTesto, Point posizione, bool mostraInTooltip)
        {
            // se mostraInTooltip è falso, non permettiamo l'ipertesto con hover da finestre dell'ipertesto (ma è comunque possibile fare il doppio clic)
            if (string.IsNullOrEmpty(testo) || (StatoFinestra != StatoFinestraIpertesto.Antenato && !mostraInTooltip))
                return;

            // se l'ultimo hover è stato molto vicino, non ripetere la visualizzazione del testo
            if (Math.Abs(ultimoHover.X - posizione.X) < distanzaNonRipetereHover && Math.Abs(ultimoHover.Y - posizione.Y) < distanzaNonRipetereHover)
            {
                this.FindForm().Activate();
                return;
            }

            if (fIpertesto == null)
            {
                fIpertesto = new Form
                {
                    Owner = this.FindForm(),
                    FormBorderStyle = FormBorderStyle.None,
                    ShowInTaskbar = false
                };
            }
            if (fIpertesto.Controls.Count == 0)
            {
                RichTextBoxEx rtControllo = new RichTextBoxEx
                {
                    Parent = fIpertesto,
                    Dock = DockStyle.Fill,
                    BackColor = Color.FromKnownColor(KnownColor.Info),
                    ForeColor = Color.FromKnownColor(KnownColor.InfoText),
                    StatoFinestra = StatoFinestraIpertesto.NonUtilizzato,
                    ReadOnly = true
                };
                rtControllo.LinkClicked += new LinkClickedEventHandler(LinkCliccatoNonAntenato);
            }
            RichTextBoxEx rtIpertesto = (RichTextBoxEx)fIpertesto.Controls[0];

            if (rtIpertesto.StatoFinestra == StatoFinestraIpertesto.NonUtilizzato)
            {
                rtIpertesto.StatoFinestra = StatoFinestraIpertesto.Utilizzato;
                rtIpertesto.Versione = versioneDelTesto;
                try
                {
                    rtIpertesto.Rtf = testo;
                    rtIpertesto.MostraLink();
                }
                catch
                {
                    rtIpertesto.Text = testo;
                }
                rtIpertesto.Modified = false;
                fIpertesto.Show();
                fIpertesto.Width = 300;

                fIpertesto.Height = Math.Min(Screen.PrimaryScreen.Bounds.Height, rtIpertesto.GetPositionFromCharIndex(rtIpertesto.Text.Length).Y + 30);
                fIpertesto.Location = new Point(posizione.X - 4, posizione.Y - 4);
                ultimoHover.X = posizione.X;
                ultimoHover.Y = posizione.Y;
                try
                {
                    if (fIpertesto.Right + fIpertesto.Owner.MdiParent.Left > Screen.PrimaryScreen.Bounds.Width)
                        fIpertesto.Width = Screen.PrimaryScreen.Bounds.Width - fIpertesto.Owner.MdiParent.Left - fIpertesto.Left - 30;
                    if (fIpertesto.Bottom + fIpertesto.Owner.MdiParent.Top > Screen.PrimaryScreen.Bounds.Height)
                        fIpertesto.Height = Screen.PrimaryScreen.Bounds.Height - fIpertesto.Owner.MdiParent.Top - fIpertesto.Top - 30;
                }
                catch { }
            }
        }

        private void RichTextBoxEx_MouseMove(object sender, MouseEventArgs e)
        {
            if (Math.Abs(Cursor.Position.X - ultimoHover.X) >= distanzaNonRipetereHover || Math.Abs(Cursor.Position.Y - ultimoHover.Y) >= distanzaNonRipetereHover)
            { // siamo andati lontano con il mouse, quindi tutti i hover sono ora validi
                ultimoHover.X = -999;
                ultimoHover.Y = -999;
            }
        }

        private void RichTextBoxEx_MouseLeave(object sender, EventArgs e)
        {
            if (StatoFinestra == StatoFinestraIpertesto.Utilizzato)
            {
                Form formDaProvare = (Form)(this.Parent); // darebbe errore se RTB fosse il primo, ma non è possibile perché StatoFinestra sarebbe Antenato
                bool formAttualeEDiscendente = false;
                if (formDaProvare.OwnedForms.Length > 0)
                {
                    do
                    {
                        formDaProvare = formDaProvare.OwnedForms[0];
                        if (formDaProvare == Form.ActiveForm)
                        {
                            formAttualeEDiscendente = true;
                        }
                    } while (formDaProvare.OwnedForms.Length > 0);
                }
                if (!formAttualeEDiscendente)
                {
                    AnnullaFinestraEFigli();
                    formDaProvare.Owner.Activate();
                }
            }
        }

        private void AnnullaFinestraEFigli()
        {
            StatoFinestra = StatoFinestraIpertesto.NonUtilizzato;
            Form formDelRT = (Form)(this.Parent);
            formDelRT.Visible = false;
            if (formDelRT.OwnedForms.Length > 0)
                ((RichTextBoxEx)(formDelRT.OwnedForms[0].Controls[0])).AnnullaFinestraEFigli();
        }

        private void LinkCliccatoNonAntenato(object sender, LinkClickedEventArgs e)
        {
            RichTextBoxEx richTextAttuale = (RichTextBoxEx)(sender);
            if (richTextAttuale != null)
            {
                if (richTextAttuale.StatoFinestra != StatoFinestraIpertesto.Antenato)
                {
                    RichTextBoxEx richTextGenitore = RichTextDelGenitore(richTextAttuale);
                    if (richTextGenitore != null)
                        richTextGenitore.OnLinkClicked(e);
                }
            }
        }

        /// <summary>
        /// L'evento quando il mouse si ferma sopra un link.
        /// </summary>
        public event EventHandler<LinkHoverEventArgs> LinkHoverEvento;

        /// <summary>
        /// L'evento quando il mouse si ferma sopra un link.
        /// </summary>
        /// <param name="e">Gli argomenti dell'evento.</param>
        protected virtual void OnLinkHover(LinkHoverEventArgs e)
        {
            if (StatoFinestra == StatoFinestraIpertesto.Antenato)
            {
                if (LinkHoverEvento != null)
                {
                    // Invokes the delegates. 
                    LinkHoverEvento(e.RichText, e);
                }
            }
            else // chiama l'evento nel genitore
            {
                RichTextBoxEx richTextGenitore = RichTextDelGenitore((RichTextBoxEx)(this));
                if (richTextGenitore != null)
                    richTextGenitore.OnLinkHover(e);
            }
        }

        private static RichTextBoxEx RichTextDelGenitore(RichTextBoxEx richTextAttuale)
        {
            RichTextBoxEx richTextGenitore = null;
            try
            {
                Form formGenitore = ((Form)(richTextAttuale.Parent)).Owner;
                for (int i = 0; i < formGenitore.Controls.Count; ++i)
                {
                    // per Sfogliare
                    if (formGenitore.Controls[i].GetType().ToString() == "System.Windows.Forms.Panel")
                    {
                        for (int j = 0; j < formGenitore.Controls[i].Controls.Count; ++j)
                            if (formGenitore.Controls[i].Controls[j].GetType().ToString() == "LaParola.RichTextBoxEx")
                                richTextGenitore = (RichTextBoxEx)(formGenitore.Controls[i].Controls[j]);
                    }
                    // per Editor
                    if (formGenitore.Controls[i].GetType().ToString() == "LaParola.RichTextBoxEx")
                        richTextGenitore = (RichTextBoxEx)(formGenitore.Controls[i]);
                }
            }
            catch
            {
            }
            return richTextGenitore;
        }

        #endregion

        #region metodi generali

        /// <summary>
        /// La parola di una certa posizione nel testo della finestra.
        /// </summary>
        /// <param name="posizione">La posizione della parola.</param>
        /// <returns>La parola a quella posizione.</returns>
        public string ParolaAttuale(int posizione)
        {
            if (posizione < 0)
                return "";

            int selezioneInizio = posizione;
            int parolaFine = selezioneInizio;
            int parolaInizio = selezioneInizio;
            if (parolaInizio >= Text.Length) // possibile se la selezione è alla fine del testo nel RTF
                parolaInizio = Text.Length - 1;
            while (parolaFine < Text.Length)
            {
                if (char.IsLetterOrDigit(Text[parolaFine]) || (Text[parolaFine] == '\'' && lingua == "el") || Char.GetUnicodeCategory(Text[parolaFine]) == UnicodeCategory.NonSpacingMark)
                    ++parolaFine;
                else
                    break;
            }
            --parolaFine;
            while (parolaInizio >= 0)
            {
                if (char.IsLetterOrDigit(Text[parolaInizio]) || Char.GetUnicodeCategory(Text[parolaInizio]) == UnicodeCategory.NonSpacingMark)
                    --parolaInizio;
                else
                    break;
            }
            ++parolaInizio;
            string parolaAttuale = "";
            if (parolaFine - parolaInizio >= -1)
            {
                parolaAttuale = Text.Substring(parolaInizio, parolaFine - parolaInizio + 1);
                if (!string.IsNullOrEmpty(parolaAttuale))
                {
                    if (parolaAttuale[0] == InizioLink)
                        parolaAttuale = parolaAttuale.Remove(0, 1);
                    if (parolaAttuale[parolaAttuale.Length - 1] == FineLink2)
                        parolaAttuale = parolaAttuale.Remove(parolaAttuale.Length - 1);
                }
            }

            return parolaAttuale;
        }

        /// <summary>
        /// Il versetto di una certa posizione nel testo della finestra, nel formato 01002003=Gen 2:3.
        /// </summary>
        /// <param name="posizione">La posizione del versetto.</param>
        /// <returns>Il riferimento del versetto attuale.</returns>
        public string VersettoAttuale(int posizione)
        {
            if (posizione < 0)
                return "";
            int p = IndexUltimoOf(InizioRiferimento, posizione + 1);
            return (p > -1 ? Text.Substring(p + 1, 8) : "");
        }

        /// <summary>
        /// Restituisce l'indice dell'ultima volta che una stringa occorre.
        /// </summary>
        /// <param name="stringaDaRicerca">La stringa da ricercare.</param>
        /// <param name="posizioneFinale">L'ultima posizione nella stringa in cui cercare la stringa da ricercare.</param>
        /// <returns></returns>
        public int IndexUltimoOf(string stringaDaRicerca, int posizioneFinale)
        {
            int p = -1, pPrecedente;
            do
            {
                pPrecedente = p;
                p = Text.IndexOf(stringaDaRicerca, p + 1, StringComparison.Ordinal);
            } while (p >= 0 && p < posizioneFinale);
            return pPrecedente;
        }

        /// <summary>
        /// Restituisce l'indice dell'ultima volta che un carattere occorre.
        /// </summary>
        /// <param name="carattereDaRicerca">Il carattere da ricercare.</param>
        /// <param name="posizioneFinale">L'ultima posizione nella stringa in cui cercare il carattere da ricercare.</param>
        /// <returns></returns>
        public int IndexUltimoOf(char carattereDaRicerca, int posizioneFinale)
        {
            int p = -1, pPrecedente;
            do
            {
                pPrecedente = p;
                p = Text.IndexOf(carattereDaRicerca.ToString(), p + 1, StringComparison.Ordinal);
            } while (p >= 0 && p < posizioneFinale);
            return pPrecedente;
        }

        /// <summary>
        /// Copia il testo selezionato al clipboard; però il testo semplice (non RTF) non contiene il testo nascosto.
        /// </summary>
        public void CopiaSenzaTestoNascosto()
        {
            string testoRtf = SelectedRtf;
            bool testoCambiato = false;
            // qualcosa di simile in testi.cs::RimuoviTestoNascosto
            while (testoRtf.IndexOf(@"\v\'01", StringComparison.Ordinal) > 0)
            {
                testoRtf = testoRtf.Remove(testoRtf.IndexOf(@"\v\'01", StringComparison.Ordinal), 14); // InizioRiferimento
                testoCambiato = true;
            }
            while (testoRtf.IndexOf(@"\'01", StringComparison.Ordinal) > 0)
            {
                testoRtf = testoRtf.Remove(testoRtf.IndexOf(@"\'01", StringComparison.Ordinal), 12); // InizioRiferimento
                testoCambiato = true;
            }
            while (testoRtf.IndexOf(@"\v\'02\v0 ", StringComparison.Ordinal) > 0) // InizioLink
            {
                testoRtf = testoRtf.Remove(testoRtf.IndexOf(@"\v\'02\v0 ", StringComparison.Ordinal), 10);
                testoCambiato = true;
            }
            while (testoRtf.IndexOf(@"\'02", StringComparison.Ordinal) > 0) // InizioLink
            {
                testoRtf = testoRtf.Remove(testoRtf.IndexOf(@"\'02", StringComparison.Ordinal), 4);
                testoCambiato = true;
            }
            while (testoRtf.IndexOf(@"\v\'03", StringComparison.Ordinal) > 0) // FineLink1
            {
                int p = testoRtf.IndexOf(@"\'04", testoRtf.IndexOf(@"\v\'03", StringComparison.Ordinal), StringComparison.Ordinal); // FineLink2
                if (p > 0 && p + 6 < testoRtf.Length && testoRtf.Substring(p, 7) == @"\'04\v0")
                    testoRtf = testoRtf.Remove(testoRtf.IndexOf(@"\v\'03", StringComparison.Ordinal), p - testoRtf.IndexOf(@"\v\'03", StringComparison.Ordinal) + 7);
                else
                {
                    p = testoRtf.IndexOf(@"\'04", testoRtf.IndexOf(@"\v\'03", StringComparison.Ordinal), StringComparison.Ordinal);
                    if (p > 0)
                        testoRtf = testoRtf.Remove(testoRtf.IndexOf(@"\v\'03", StringComparison.Ordinal), p - testoRtf.IndexOf(@"\v\'03", StringComparison.Ordinal) + 4);
                }
                testoCambiato = true;
            }
            while (testoRtf.IndexOf(@"\v\'0e", StringComparison.Ordinal) > 0)
            {
                int p = testoRtf.IndexOf(@"\v0", testoRtf.IndexOf(@"\v\'0e", StringComparison.Ordinal), StringComparison.Ordinal); // ParolaRicercata
                testoRtf = testoRtf.Remove(p, 3).Remove(testoRtf.IndexOf(@"\v\'0e", StringComparison.Ordinal), 6);
                testoCambiato = true;
            }
            while (testoRtf.IndexOf(@"\'0e", StringComparison.Ordinal) > 0)
            {
                int p = testoRtf.IndexOf(@"\v0", testoRtf.IndexOf(@"\'0e", StringComparison.Ordinal), StringComparison.Ordinal); // ParolaRicercata
                testoRtf = testoRtf.Remove(p, 3).Remove(testoRtf.IndexOf(@"\'0e", StringComparison.Ordinal), 4);
                testoCambiato = true;
            }
            testoRtf = testoRtf.Replace(@"\v\", @"\");

            if (testoCambiato)
            {
                RichTextBoxEx clipboardRtf = new RichTextBoxEx
                {
                    Rtf = testoRtf
                };
                clipboardRtf.SelectAll();
                clipboardRtf.Copy();
            }
            else
            {
                Copy();
            }
        }

        #endregion

    }

    #region LinkHover classi

    /// <summary>
    /// Gli argomenti dell'evento quando il mouse si ferma sopra un link.
    /// </summary>
    [ComVisible(false)]
    public class LinkHoverEventArgs : EventArgs
    {
        private string linkTesto;
        /// <summary>
        /// Il testo del link.
        /// </summary>
        public string LinkTesto
        {
            get { return linkTesto; }
            //            set { linkTesto = value; }
        }

        private RichTextBoxEx richText;
        /// <summary>
        /// Il controllo in cui il link esiste.
        /// </summary>
        public RichTextBoxEx RichText
        {
            get { return richText; }
            //            set { richText = value; }
        }

        /// <summary>
        /// Il costruttore della classe.
        /// </summary>
        /// <param name="controllo">Il controllo in cui il link esiste.</param>
        /// <param name="testo">Il testo del link.</param>
        public LinkHoverEventArgs(RichTextBoxEx controllo, string testo)
        {
            linkTesto = testo;
            richText = controllo;
        }
    }

    /*
    /// <summary>
    /// Il delegate che inizia l'evento quando il mouse si ferma sopra un link.
    /// </summary>
    /// <param name="sender">La classe che ha generato l'evento.</param>
    /// <param name="e">Gli argomenti dell'evento.</param>
    public delegate void LinkHoverEventHandler(object sender, LinkHoverEventArgs e);
    */

    #endregion

    internal static class SafeNativeMethods
    {
        [DllImport("user32", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref RichTextBoxEx.PARAFORMAT lp);

        [DllImport("user32", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(HandleRef hWnd, Int32 msg, IntPtr wParam, ref RichTextBoxEx.EDITSTREAM editStream);
    }
}
