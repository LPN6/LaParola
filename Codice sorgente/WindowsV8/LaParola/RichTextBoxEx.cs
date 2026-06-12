using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using static LaParola.Utilities.Funzioni;

namespace LaParola
{
    /// <summary>
    /// An extension for RichTextBox suitable for printing, formatting selections
    /// </summary>
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
        public string Rtf
        {
            get
            {
                TextRange range = new(Document.ContentStart, Document.ContentEnd);
                using MemoryStream ms = new();
                range.Save(ms, DataFormats.Rtf);            // Save in RTF [3](https://stackoverflow.com/questions/79407278/can-i-save-a-textrange-inline-property-to-rtf-in-wpf)
                return Encoding.UTF8.GetString(ms.ToArray()); // conversione a stringa (esempio comune) [3](https://stackoverflow.com/questions/79407278/can-i-save-a-textrange-inline-property-to-rtf-in-wpf)
            }
            set
            {
                // Se stringa nulla/vuota: pulisci il documento
                if (string.IsNullOrEmpty(value))
                {
                    Document.Blocks.Clear();
                    return;
                }

                // Carica l’RTF in un TextRange che copre tutto il documento
                TextRange range = new(Document.ContentStart, Document.ContentEnd);
                using MemoryStream ms = new(Encoding.UTF8.GetBytes(ConvertiUnicodeInRtf(value))); // puoi usare un encoding diverso se serve
                range.Load(ms, DataFormats.Rtf); // Load supporta Rtf [1](https://learn.microsoft.com/en-us/dotnet/api/system.windows.documents.textrange.load?view=windowsdesktop-10.0)[2](https://stackoverflow.com/questions/1367256/set-rtf-text-into-wpf-richtextbox-control)
            }
        }

        /* TODO2 ipertesto
        /// <summary>
        /// Il testo Rtf del testo selezionato.
        /// </summary>
        new public string SelectedRtf
        {
            get => base.SelectedRtf;
            set => base.SelectedRtf = ConvertiUnicodeInRtf(value);
        }
        */

        public string Text
        {
            get
            {
                // Testo plain dell’intero documento
                TextRange range = new(Document.ContentStart, Document.ContentEnd);
                string text = range.Text;// [1](https://github.com/MicrosoftDocs/winrt-api/blob/docs/windows.ui.xaml.documents/textelement_fontfamilyproperty.md/)

                // Opzionale: WPF spesso aggiunge CRLF finale perché ogni Paragraph termina con newline
                // Se ti dà fastidio, rimuovilo:
                return text;//.TrimEnd('\r', '\n'); [2](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/how-to-extract-the-text-content-from-a-richtextbox)[1](https://github.com/MicrosoftDocs/winrt-api/blob/docs/windows.ui.xaml.documents/textelement_fontfamilyproperty.md/)
            }
            set
            {
                // Sostituisce *tutto* il contenuto con plain text (nessuna formattazione)
                new TextRange(Document.ContentStart, Document.ContentEnd).Text = value ?? string.Empty; //[1](https://github.com/MicrosoftDocs/winrt-api/blob/docs/windows.ui.xaml.documents/textelement_fontfamilyproperty.md/)
            }
        }


        /// <summary>
        /// Seleziona tutto il testo.
        /// </summary>
        new public void SelectAll()
        {
            base.SelectAll(); // a volte non funziona, perché i caratteri con ASCII<16 inseriti per inizio riferimento eccetera fanno sì che niente sia selezionato
            if (Selection.Text.Length == 0 && Text.Length > 0)
            {
                int lunghezza = Text.Length;
                for (int i = 1; i <= 10; ++i)
                {
                    Select(i, lunghezza);
                    if (Selection.Text.Length > 0 && i < lunghezza)
                    {
                        break;
                    }
                }
            }
        }

        public void Select(int m, int n)
        {
            if (n < m) (m, n) = (n, m);

            TextPointer start = GetTextPointerAtCharOffset(m);
            TextPointer end = GetTextPointerAtCharOffset(n);

            Selection.Select(start, end); // Select(TextPointer, TextPointer) [1](https://learn.microsoft.com/en-us/dotnet/api/system.windows.documents.textselection?view=windowsdesktop-10.0)
            Focus();
        }


        /// <summary>
        /// Restituisce un TextPointer corrispondente all'offset di caratteri "plain text"
        /// (simile a TextRange.Text). Ogni fine paragrafo conta come \r\n (2 char).
        /// </summary>
        public TextPointer GetTextPointerAtCharOffset(int charOffset)
        {
            if (charOffset < 0) charOffset = 0;

            TextPointer navigator = Document.ContentStart;
            int count = 0;

            while (navigator != null && navigator.CompareTo(Document.ContentEnd) < 0)
            {
                switch (navigator.GetPointerContext(LogicalDirection.Forward))
                {
                    case TextPointerContext.Text:
                        // lunghezza del run di testo
                        int runLength = navigator.GetTextRunLength(LogicalDirection.Forward);
                        if (count + runLength >= charOffset)
                        {
                            // posizione dentro il run
                            return navigator.GetPositionAtOffset(charOffset - count, LogicalDirection.Forward);
                        }
                        count += runLength;
                        navigator = navigator.GetPositionAtOffset(runLength, LogicalDirection.Forward);
                        continue;

                    case TextPointerContext.ElementEnd:
                        // Fine paragrafo => \r\n (2 caratteri) come in TextRange.Text
                        if (navigator.GetAdjacentElement(LogicalDirection.Forward) is Paragraph)
                            count += 2;
                        break;

                    case TextPointerContext.EmbeddedElement:
                        // un elemento embedded conta come 1 "simbolo"; qui lo contiamo come 1 char
                        count += 1;
                        break;

                        // ElementStart / None: non incrementiamo count
                }

                navigator = navigator.GetNextContextPosition(LogicalDirection.Forward);
            }

            return Document.ContentEnd;
        }

        private bool hoverNotificato = false;
        //private Form fIpertesto; TODO2 ipertesto
        private System.Windows.Point ultimoHover = new(-999, -999);
        internal static bool isRunningOnMono;

        private enum StatoFinestraIpertesto
        {
            Antenato, NonUtilizzato, Utilizzato
        }

        private StatoFinestraIpertesto statoFinestra = StatoFinestraIpertesto.Antenato;
        private StatoFinestraIpertesto StatoFinestra
        {
            get => statoFinestra; set => statoFinestra = value;
        }

        /* TODO2 ipertesto
        /// <summary>
        /// Se il controllo rivela e visualizza gli url automaticamente.
        /// </summary>
        [System.ComponentModel.DefaultValue(false)]
        public new bool DetectUrls
        { get => base.DetectUrls; set => base.DetectUrls = value;
        }
        */

        private string versione="";
        /// <summary>
        /// La versione della Bibbia del testo nel controllo.
        /// </summary>
        public string Versione
        {
            get => versione; set => versione = value;
        }

        private string lingua = "";
        /// <summary>
        /// La lingua del testo nel controllo (o lingue, se separate con una riga verticale |).
        /// </summary>
        public string Lingua
        {
            get => lingua; set => lingua = value;
        }

        #endregion

        #region const e struct

        #region per i link
        /* TODO2 ipertesto

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
        */
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
            // this.DetectUrls = false; TODO2 ipertesto
            isRunningOnMono = (Type.GetType("Mono.Runtime") != null);
            this.IsDocumentEnabled = true;

            /* TODO2 ipertesto
            MouseLeave += new MouseEventHandler(RichTextBoxEx_MouseLeave);
            MouseMove += new MouseEventHandler(RichTextBoxEx_MouseMove);
            */
        }

        #region AggiungiRtf

        /// <summary>
        /// Aggiunge del testo RTF (cioè {\rtf...}) alla fine del controllo.
        /// </summary>
        /// <param name="testoRtfDaAggiungere">Il testo da aggiungere.</param>
        public void AggiungiRtf(string testoRtfDaAggiungere)
        {
            if (string.IsNullOrWhiteSpace(testoRtfDaAggiungere))
                return;

            // Assicura che esista almeno un Paragraph (così l'inserimento in coda è più stabile)
            if (Document.Blocks.Count == 0)
                Document.Blocks.Add(new Paragraph());

            // Posizione di inserimento: subito prima di ContentEnd (insertion position)
            TextPointer insertPos =
                Document.ContentEnd.GetInsertionPosition(LogicalDirection.Backward)
                ?? Document.ContentEnd;

            // Crea un range vuoto in quel punto: Load sostituisce la selezione con il contenuto caricato [1](https://umaranis.com/2010/11/29/save-and-load-richtextbox-content-in-wpf/)
            TextRange range = new(insertPos, insertPos);

            // converti la stringa in bytes in modo coerente con il tuo RTF.
            using MemoryStream ms = new(Encoding.ASCII.GetBytes(ConvertiUnicodeInRtf(testoRtfDaAggiungere)));
            ms.Position = 0;

            // Carica l'RTF nel range (inserisce in quel punto) [1](https://umaranis.com/2010/11/29/save-and-load-richtextbox-content-in-wpf/)[2](https://www.vbforums.com/showthread.php?719411-RESOLVED-TextRange-does-not-get-RTF-of-WPF-RichTextbox)
            range.Load(ms, DataFormats.Rtf);

            // opzionale: porta il caret alla fine dopo l'inserimento
            CaretPosition = Document.ContentEnd;
        }

        #endregion

        #region stampa
        /* TODO2 stampa
        
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
            {
                throw new ArgumentNullException(nameof(e));
            }

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
            IntPtr lParam = new(0);
            SafeNativeMethods.SendMessage(Handle, EM_FORMATRANGE, (IntPtr)0, lParam);
        }
        */
        #endregion

        #region font

        /// <summary>
        /// Imposta il font della selezione.
        /// </summary>
        /// <param name="nomeFont">Il nome del font.</param>
        public void SetFont(string nomeFont)
        {
            if (nomeFont == null || nomeFont.Length == 0)
            {
                return;
            }

            if (!Selection.IsEmpty)
            {
                Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, new FontFamily(nomeFont)); ;
                return;
            }

            if (Selection.Start.Paragraph == null)
            {
                Paragraph p = new() { FontFamily = new FontFamily(nomeFont) };
                Document.Blocks.Add(p);
                CaretPosition = p.ContentStart;
                return;
            }

            TextPointer? caret = CaretPosition;
            Block? block = Document.Blocks
                .FirstOrDefault(b => b.ContentStart.CompareTo(caret) < 0 && b.ContentEnd.CompareTo(caret) > 0);

            if (block is Paragraph para)
            {
                Run run = new() { FontFamily = new FontFamily(nomeFont) };
                para.Inlines.Add(run);
                CaretPosition = run.ElementStart; // così il prossimo testo usa quella size [3](https://github.com/dotnet/docs-desktop/blob/master/dotnet-desktop-guide/framework/wpf/advanced/how-to-enumerate-system-fonts.md)
            }
        }

        /// <summary>
        /// Imposta la dimensione del font della selezione.
        /// </summary>
        /// <param name="size">La dimensione da impostare.</param>
        public void SetSize(Single size)
        {
            if (!Selection.IsEmpty)
            {
                Selection.ApplyPropertyValue(TextElement.FontSizeProperty, size);
                return;
            }

            if (Selection.Start.Paragraph == null)
            {
                Paragraph p = new() { FontSize = size };
                Document.Blocks.Add(p);
                CaretPosition = p.ContentStart;
                return;
            }

            TextPointer caret = CaretPosition;
            Block? block = Document.Blocks
                .FirstOrDefault(b => b.ContentStart.CompareTo(caret) < 0 && b.ContentEnd.CompareTo(caret) > 0);

            if (block is Paragraph para)
            {
                Run run = new() { FontSize = size };
                para.Inlines.Add(run);
                CaretPosition = run.ElementStart; // così il prossimo testo usa quella size [3](https://github.com/dotnet/docs-desktop/blob/master/dotnet-desktop-guide/framework/wpf/advanced/how-to-enumerate-system-fonts.md)
            }
        }

        /// <summary>
        /// Cambia se la selezione è in grassetto.
        /// </summary>
        /// <param name="set">True lo imposta, false lo toglie.</param>
        public void SetSelectionBold(bool set)
        {
            if (!Selection.IsEmpty)
            {
                Selection.ApplyPropertyValue(TextElement.FontWeightProperty, set ? FontWeights.Bold : FontWeights.Normal);
                return;
            }

            if (Selection.Start.Paragraph == null)
            {
                Paragraph p = new() { FontWeight = set ? FontWeights.Bold : FontWeights.Normal };
                Document.Blocks.Add(p);
                CaretPosition = p.ContentStart;
                return;
            }

            TextPointer caret = CaretPosition;
            Block? block = Document.Blocks
                .FirstOrDefault(b => b.ContentStart.CompareTo(caret) < 0 && b.ContentEnd.CompareTo(caret) > 0);

            if (block is Paragraph para)
            {
                Run run = new() { FontWeight = set ? FontWeights.Bold : FontWeights.Normal };
                para.Inlines.Add(run);
                CaretPosition = run.ElementStart; // così il prossimo testo usa quella size [3](https://github.com/dotnet/docs-desktop/blob/master/dotnet-desktop-guide/framework/wpf/advanced/how-to-enumerate-system-fonts.md)
            }
        }

        /// <summary>
        /// Cambia se la selezione è in corsivo.
        /// </summary>
        /// <param name="set">True lo imposta, false lo toglie.</param>
        public void SetSelectionItalic(bool set)
        {
            if (!Selection.IsEmpty)
            {
                Selection.ApplyPropertyValue(TextElement.FontStyleProperty, set ? FontStyles.Italic : FontStyles.Normal);
                return;
            }

            if (Selection.Start.Paragraph == null)
            {
                Paragraph p = new() { FontStyle = set ? FontStyles.Italic : FontStyles.Normal };
                Document.Blocks.Add(p);
                CaretPosition = p.ContentStart;
                return;
            }

            TextPointer caret = CaretPosition;
            Block? block = Document.Blocks
                .FirstOrDefault(b => b.ContentStart.CompareTo(caret) < 0 && b.ContentEnd.CompareTo(caret) > 0);

            if (block is Paragraph para)
            {
                Run run = new() { FontStyle = set ? FontStyles.Italic : FontStyles.Normal };
                para.Inlines.Add(run);
                CaretPosition = run.ElementStart; // così il prossimo testo usa quella size [3](https://github.com/dotnet/docs-desktop/blob/master/dotnet-desktop-guide/framework/wpf/advanced/how-to-enumerate-system-fonts.md)
            }
        }

        /// <summary>
        /// Cambia se la selezione è sottolineata.
        /// </summary>
        /// <param name="set">True lo imposta, false lo toglie.</param>
        public void SetSelectionUnderline(bool set)
        {
            if (!Selection.IsEmpty)
            {
                Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, set ? TextDecorations.Underline : null);
                return;
            }

            if (!Document.Blocks.OfType<Paragraph>().Any())
            {
                Paragraph p = new();
                Document.Blocks.Add(p);
                CaretPosition = p.ContentStart;
            }

            TextPointer insertPos = CaretPosition.GetInsertionPosition(LogicalDirection.Forward);

            Run run = new("", insertPos)
            {
                TextDecorations = set ? TextDecorations.Underline : null // TextDecorations su Inline/Run [1](https://learn.microsoft.com/en-us/dotnet/api/system.windows.documents.inline.textdecorations?view=windowsdesktop-10.0)
            };

            CaretPosition = run.ElementStart;
            Focus();
        }

        /* TODO2 sottolineatura tipo
        /// <summary>
        /// Cambia il tipo di sottolineatura.
        /// </summary>
        /// <param name="underlineType">Il tipo di sottolineatura.</param>
        public void SetSelectionUnderlineTypeNotMono(byte underlineType)
        {
            ApplyStyleUnderlineNotMono(CFM_UNDERLINETYPE, underlineType);
        }
        */

        /* TODO2 ipertesto
        /// <summary>
        /// Set the current selection's link style
        /// </summary>
        /// <param name="link">true: set link style, false: clear link style</param>
        public void SetSelectionLink(bool link)
        {
            ApplyStyle(CFM_LINK, link);
        }

        /// <summary>
        /// Change the selected text to a link. The link text is followed by a @ symbol,
        /// a letter indicating the type of link, and the given hyperlink text, all of them invisible.
        /// When clicked on, the whole link text and hyperlink string are given in the LinkClickedEventArgs.
        /// </summary>
        /// <param name="link">Invisible hyperlink string to be inserted</param>
        /// <param name="tipo">The type of link: FineLinkBrano->Bible passage, FineLinkNota->nota, FineLinkFile->file</param>
        public void InserisciLink(string link, char tipo)
        {
            ArgumentNullException.ThrowIfNull(link);

            int selezioneInizio = this.SelectionStart;
            int selezioneLunghezza = this.Selection.Text.Length;
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
            int selezioneInizio = new TextRange(rtb.Document.ContentStart, rtb.Selection.Start).Text.Length;
            int selezioneLunghezza = Selection.Text.Length;
            bool modificato = Modified;
            int posizioneInizio = this.Text.IndexOf(InizioLink, posizione);
            BloccaRtf(true);
            try
            {
                //                while (this.Text().IndexOf((InizioLink.ToString(), posizione, StringComparison.Ordinal) >= posizione)
                while (posizioneInizio >= posizione)
                {
                    //                    posizioneInizio = this.Text().IndexOf(InizioLink.ToString(), posizione, StringComparison.Ordinal);
                    //posizioneInizio = this.Text().IndexOf(InizioLink, posizione);
                    //                    linkFine = this.Text().IndexOf(FineLink2.ToString(), posizioneInizio, StringComparison.Ordinal);
                    posizione = this.Text.IndexOf(FineLink2, posizioneInizio);

                    if (posizione >= posizioneInizio)
                    {
                        ImpostaFormatoDelLink(posizioneInizio, posizione - posizioneInizio + 1);
                    }

                    posizioneInizio = this.Text.IndexOf(InizioLink, posizione);
                }
            }
            finally
            {
                if (selezioneLunghezza >= 0)
                {
                    Select(selezioneInizio, selezioneLunghezza);
                }

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
        */
        /*
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
        */
        #endregion

        #region allineamento

        /// <summary>
        /// Gets or sets the alignment to apply to the current
        /// selection or insertion point.
        /// </summary>
        /// <remarks>
        /// Replaces the SelectionAlignment from <see cref="RichTextBox"/>.
        /// </remarks>
        public TextAlignment? SelectionAlignment
        {
            get
            {
                object v = Selection.GetPropertyValue(Paragraph.TextAlignmentProperty);
                if (v == DependencyProperty.UnsetValue)
                    return null; // mixed alignment
                return (TextAlignment)v;
            }
            set
            {
                Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, value);
            }
        }

        #endregion

        #region link hover
        /* TODO2 ipertesto
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
                                string link = Text()[enLink.chrg.cpMin..enLink.chrg.cpMax];
                                if (!string.IsNullOrEmpty(link) && link[0] == InizioLink && link[^1] == FineLink2)
                                {
                                    link = link[..^1][1..];
                                    LinkHoverEventArgs e = new(this, link);
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
        public void MostraHover(string testo, string versioneDelTesto, System.Windows.Point posizione, bool mostraInTooltip)
        {
            // se mostraInTooltip è falso, non permettiamo l'ipertesto con hover da finestre dell'ipertesto (ma è comunque possibile fare il doppio clic)
            if (string.IsNullOrEmpty(testo) || (StatoFinestra != StatoFinestraIpertesto.Antenato && !mostraInTooltip))
            {
                return;
            }

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
                RichTextBoxEx rtControllo = new()
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

                fIpertesto.Height = Math.Min(Screen.PrimaryScreen.Bounds.Height, rtIpertesto.GetPositionFromCharIndex(rtIpertesto.Text().Length).Y + 30);
                fIpertesto.Location = new System.Windows.Point(posizione.X - 4, posizione.Y - 4);
                ultimoHover.X = posizione.X;
                ultimoHover.Y = posizione.Y;
                try
                {
                    if (fIpertesto.Right + fIpertesto.Owner.MdiParent.Left > Screen.PrimaryScreen.Bounds.Width)
                    {
                        fIpertesto.Width = Screen.PrimaryScreen.Bounds.Width - fIpertesto.Owner.MdiParent.Left - fIpertesto.Left - 30;
                    }

                    if (fIpertesto.Bottom + fIpertesto.Owner.MdiParent.Top > Screen.PrimaryScreen.Bounds.Height)
                    {
                        fIpertesto.Height = Screen.PrimaryScreen.Bounds.Height - fIpertesto.Owner.MdiParent.Top - fIpertesto.Top - 30;
                    }
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
            {
                ((RichTextBoxEx)(formDelRT.OwnedForms[0].Controls[0])).AnnullaFinestraEFigli();
            }
        }

        private void LinkCliccatoNonAntenato(object sender, LinkClickedEventArgs e)
        {
            RichTextBoxEx richTextAttuale = (RichTextBoxEx)(sender);
            if (richTextAttuale != null)
            {
                if (richTextAttuale.StatoFinestra != StatoFinestraIpertesto.Antenato)
                {
                    RichTextBoxEx richTextGenitore = RichTextDelGenitore(richTextAttuale);
                    richTextGenitore?.OnLinkClicked(e);
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
                // Invokes the delegates. 
                LinkHoverEvento?.Invoke(e.RichText, e);
            }
            else // chiama l'evento nel genitore
            {
                RichTextBoxEx richTextGenitore = RichTextDelGenitore((RichTextBoxEx)(this));
                richTextGenitore?.OnLinkHover(e);
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
                        {
                            if (formGenitore.Controls[i].Controls[j].GetType().ToString() == "LaParola.RichTextBoxEx")
                            {
                                richTextGenitore = (RichTextBoxEx)(formGenitore.Controls[i].Controls[j]);
                            }
                        }
                    }
                    // per Editor
                    if (formGenitore.Controls[i].GetType().ToString() == "LaParola.RichTextBoxEx")
                    {
                        richTextGenitore = (RichTextBoxEx)(formGenitore.Controls[i]);
                    }
                }
            }
            catch
            {
            }
            return richTextGenitore;
        }
        */
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
            {
                return "";
            }

            int selezioneInizio = posizione;
            int parolaFine = selezioneInizio;
            int parolaInizio = selezioneInizio;
            if (parolaInizio >= Text.Length) // possibile se la selezione è alla fine del testo nel RTF
            {
                parolaInizio = Text.Length - 1;
            }

            while (parolaFine < Text.Length)
            {
                if (char.IsLetterOrDigit(Text[parolaFine]) || (Text[parolaFine] == '\'' && lingua == "el") || Char.GetUnicodeCategory(Text[parolaFine]) == UnicodeCategory.NonSpacingMark)
                {
                    ++parolaFine;
                }
                else
                {
                    break;
                }
            }
            --parolaFine;
            while (parolaInizio >= 0)
            {
                if (char.IsLetterOrDigit(Text[parolaInizio]) || Char.GetUnicodeCategory(Text[parolaInizio]) == UnicodeCategory.NonSpacingMark)
                {
                    --parolaInizio;
                }
                else
                {
                    break;
                }
            }
            ++parolaInizio;
            string parolaAttuale = "";
            if (parolaFine - parolaInizio >= -1)
            {
                parolaAttuale = Text.Substring(parolaInizio, parolaFine - parolaInizio + 1);
                if (!string.IsNullOrEmpty(parolaAttuale))
                {
                    if (parolaAttuale[0] == InizioLink)
                    {
                        parolaAttuale = parolaAttuale[1..];
                    }

                    if (parolaAttuale[^1] == FineLink2)
                    {
                        parolaAttuale = parolaAttuale[..^1];
                    }
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
            {
                return "";
            }

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

        /* TODO2 ipertesto
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
                {
                    testoRtf = testoRtf.Remove(testoRtf.IndexOf(@"\v\'03", StringComparison.Ordinal), p - testoRtf.IndexOf(@"\v\'03", StringComparison.Ordinal) + 7);
                }
                else
                {
                    p = testoRtf.IndexOf(@"\'04", testoRtf.IndexOf(@"\v\'03", StringComparison.Ordinal), StringComparison.Ordinal);
                    if (p > 0)
                    {
                        testoRtf = testoRtf.Remove(testoRtf.IndexOf(@"\v\'03", StringComparison.Ordinal), p - testoRtf.IndexOf(@"\v\'03", StringComparison.Ordinal) + 4);
                    }
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
                RichTextBoxEx clipboardRtf = new()
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
        */
        #endregion

    }

    #region LinkHover classi
    /* TODO2 ipertesto
    /// <summary>
    /// Gli argomenti dell'evento quando il mouse si ferma sopra un link.
    /// </summary>
    [ComVisible(false)]
    public class LinkHoverEventArgs : EventArgs
    {
        private readonly string linkTesto;
        /// <summary>
        /// Il testo del link.
        /// </summary>
        public string LinkTesto
        {
            get { return linkTesto; }
            //            set { linkTesto = value; }
        }

        private readonly RichTextBoxEx richText;
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

    */

    #endregion
}
