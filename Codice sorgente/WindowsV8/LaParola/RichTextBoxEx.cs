using LaParola.Dialogs;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using static LaParola.Utilities.Funzioni;

namespace LaParola
{
    /// <summary>
    /// An extension for RichTextBox
    /// </summary>
    public class RichTextBoxEx : RichTextBox
    {
        #region proprietà

        /// <summary>
        /// Le otto cifre che seguono danno il riferimento del versetto che segue.
        /// </summary>
//        public const char InizioRiferimento = (char)1;
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
        //public const char ParolaRicercata = (char)14;

        internal static bool isRunningOnMono;

        private string versione = "";
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

        /// <summary>
        /// Il testo Rtf del controllo.
        /// </summary>
        public string Rtf
        {
            get
            {
                TextRange range = new(Document.ContentStart, Document.ContentEnd);
                using MemoryStream ms = new();
                range.Save(ms, DataFormats.Rtf);            // Save in RTF (https://stackoverflow.com/questions/79407278/can-i-save-a-textrange-inline-property-to-rtf-in-wpf)
                return Encoding.UTF8.GetString(ms.ToArray()); // conversione a stringa (esempio comune) (https://stackoverflow.com/questions/79407278/can-i-save-a-textrange-inline-property-to-rtf-in-wpf)
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
                using MemoryStream ms = new(Encoding.UTF8.GetBytes(ConvertiUnicodeInRtf(value)));
                range.Load(ms, DataFormats.Rtf); // Load supporta Rtf (https://learn.microsoft.com/en-us/dotnet/api/system.windows.documents.textrange.load?view=windowsdesktop-10.0) (https://stackoverflow.com/questions/1367256/set-rtf-text-into-wpf-richtextbox-control)
            }
        }

        public string Text
        {
            get
            {
                // Testo plain dell’intero documento
                TextRange range = new(Document.ContentStart, Document.ContentEnd);
                string text = range.Text;// (https://github.com/MicrosoftDocs/winrt-api/blob/docs/windows.ui.xaml.documents/textelement_fontfamilyproperty.md/)

                // Opzionale: WPF spesso aggiunge CRLF finale perché ogni Paragraph termina con newline
                // Se ti dà fastidio, rimuovilo:
                return text;//.TrimEnd('\r', '\n'); (https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/how-to-extract-the-text-content-from-a-richtextbox)[1](https://github.com/MicrosoftDocs/winrt-api/blob/docs/windows.ui.xaml.documents/textelement_fontfamilyproperty.md/)
            }
            set
            {
                // Sostituisce *tutto* il contenuto con plain text (nessuna formattazione)
                new TextRange(Document.ContentStart, Document.ContentEnd).Text = value ?? string.Empty; //(https://github.com/MicrosoftDocs/winrt-api/blob/docs/windows.ui.xaml.documents/textelement_fontfamilyproperty.md/)
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

            Selection.Select(start, end); // Select(TextPointer, TextPointer) (https://learn.microsoft.com/en-us/dotnet/api/system.windows.documents.textselection?view=windowsdesktop-10.0)
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

        #endregion

        #region const e struct

        #region per la stampa

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
            isRunningOnMono = (Type.GetType("Mono.Runtime") != null);

            // 1. Create the base style for Hyperlink
            Style linkStyle = new(typeof(Hyperlink));
            linkStyle.Setters.Add(new Setter(TextElement.ForegroundProperty, Brushes.Blue));
            linkStyle.Setters.Add(new Setter(Inline.TextDecorationsProperty, TextDecorations.Underline));

            // 2. Create the hover trigger (turns red when mouse is over)
            Trigger hoverTrigger = new() { Property = ContentElement.IsMouseOverProperty, Value = true };
            hoverTrigger.Setters.Add(new Setter(TextElement.ForegroundProperty, Brushes.Red));
            linkStyle.Triggers.Add(hoverTrigger);

            // 3. Apply the style to the Editor's resources
            Resources.Add(typeof(Hyperlink), linkStyle);

            // 4. CRITICAL: Ensure the document is enabled to receive UI events
            IsDocumentEnabled = true;

            // 5. ATTACH GLOBAL HYPERLINK ROUTED EVENTS DIRECTLY TO THE EDITOR
            // This catches the click/navigation for ALL links, regardless of document resources
            AddHandler(Hyperlink.RequestNavigateEvent, new System.Windows.Navigation.RequestNavigateEventHandler(Editor_RequestNavigate));

            linkStyle.Setters.Add(new EventSetter(ContentElement.MouseEnterEvent,
                new MouseEventHandler(HoverPopup.OnHyperlinkHover)));
        }

        protected override void OnMouseDoubleClick(System.Windows.Input.MouseButtonEventArgs e)
        {
            // Lasciamo che il RichTextBox base esegua prima la sua selezione nativa della parola ("all'uomo")
            base.OnMouseDoubleClick(e);

            if (MainWindow.settings.IpertestoDizionario == false)
            {
                return;
            }

            // INTERCEZIONE E CORREZIONE DELL'APOSTROFO
            try
            {
                // Recuperiamo la posizione esatta del puntatore del mouse al momento del doppio clic
                System.Windows.Point clickPoint = e.GetPosition(this);
                TextPointer clickPos = this.GetPositionFromPoint(clickPoint, true);

                if (clickPos != null)
                {
                    // Espandiamo verso SINISTRA per trovare l'inizio della parola
                    TextPointer wordStart = clickPos;
                    while (wordStart.CompareTo(this.Document.ContentStart) > 0)
                    {
                        TextPointer prev = wordStart.GetNextInsertionPosition(LogicalDirection.Backward);
                        if (prev == null) break;

                        string chText = new TextRange(prev, wordStart).Text;
                        if (string.IsNullOrEmpty(chText)) break;

                        char c = chText[0];
                        // Se andiamo a sinistra e colpiamo un apostrofo o punteggiatura, ci fermiamo.
                        // Questo fa sì che "uomo" NON includa l'apostrofo alla sua sinistra.
                        if (IsCarattereDaScartare(c) || c == '\'' || c == '’')
                        {
                            break;
                        }
                        wordStart = prev;
                    }

                    // Espandiamo verso DESTRA per trovare la fine della parola
                    TextPointer wordEnd = clickPos;
                    while (wordEnd.CompareTo(this.Document.ContentEnd) < 0)
                    {
                        TextPointer next = wordEnd.GetNextInsertionPosition(LogicalDirection.Forward);
                        if (next == null) break;

                        string chText = new TextRange(wordEnd, next).Text;
                        if (string.IsNullOrEmpty(chText)) break;

                        char c = chText[0];
                        // Se andiamo a destra e colpiamo un apostrofo, lo INCLUDIAMO nella parola di sinistra ("all'") e poi ci fermiamo.
                        if (c == '\'' || c == '’')
                        {
                            wordEnd = next;
                            break;
                        }
                        if (IsCarattereDaScartare(c))
                        {
                            break;
                        }
                        wordEnd = next;
                    }

                    // Sovrascriviamo la selezione nativa di WPF con i nostri confini personalizzati
                    this.Selection.Select(wordStart, wordEnd);
                }
            }
            catch /*(Exception ex)*/
            {
                // Silenzioso: se il parser di WPF fallisce il calcolo geometrico dei punti per motivi di rendering,
                // l'app non crasha e mantiene la selezione nativa di base.
                //System.Diagnostics.Debug.WriteLine($"Errore calcolo shortcut apostrofo: {ex.Message}");
            }

            // Recuperiamo il testo che ora è stato correttamente ricalcolato (all' OPPURE uomo)
            string testoSelezionato = this.Selection.Text;

            if (!string.IsNullOrWhiteSpace(testoSelezionato))
            {
                // 3. Puliamo la parola
                string parolaPulita = PulisciParolaPerDizionario(testoSelezionato);

                if (!string.IsNullOrEmpty(parolaPulita))
                {
                    // Inviamo la parola isolata al dizionario
                    MainWindow.ApriDefinizioneDizionario(parolaPulita, versione);
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Determina se un carattere è da scartare ai bordi della parola, 
        /// proteggendo le lettere, i numeri, i diacritici greci e gli apostrofi utili.
        /// </summary>
        private static bool IsCarattereDaScartare(char c)
        {
            // AGGIORNAMENTO: Proteggiamo l'apostrofo (sia dritto ' che tipografico ’) 
            // per evitare che PulisciParola tranci via la coda di "all'" dopo che l'abbiamo isolata.
            if (char.IsLetterOrDigit(c) || c == '\'' || c == '’') return false;

            var categoria = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
            if (categoria == System.Globalization.UnicodeCategory.NonSpacingMark) return false;

            return true;
        }

        /// <summary>
        /// Pulisce la parola isolando i caratteri alfanumerici e i diacritici combinati (Greco Form D).
        /// </summary>
        private static string PulisciParolaPerDizionario(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            text = text.Trim();

            // Trova l'inizio reale della parola saltando la punteggiatura iniziale
            int start = 0;
            while (start < text.Length && IsCarattereDaScartare(text[start]))
            {
                start++;
            }

            // Trova la fine reale della parola saltando la punteggiatura finale
            int end = text.Length - 1;
            while (end >= start && IsCarattereDaScartare(text[end]))
            {
                end--;
            }

            if (start > end) return string.Empty;

            return text.Substring(start, end - start + 1);
        }

        private void Editor_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            e.Handled = true;
            string uri = e.Uri.OriginalString;

            // CASE 1: Bible Reference
            if (uri.StartsWith("bibbia:"))
            {
                string code = uri.Replace("bibbia:", "");
                MainWindow.LinkCliccato(1, code);
            }

            // CASE 2: Cross-Reference to another note
            else if (uri.StartsWith("nota:"))
            {
                string noteName = uri.Replace("nota:", "");
                MainWindow.LinkCliccato(2, noteName);
            }

            // CASE 3: External File Target
            else if (uri.StartsWith("filenome:"))
            {
                string targetFile = uri.Replace("filenome:", "");
                MainWindow.LinkCliccato(3, targetFile);
            }
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
        /* TODO2 cancellare?
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
        */

        #endregion
    }
}
