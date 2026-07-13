using AvalonDock.Layout;
using LaParola.Utilities;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Security.Principal;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace LaParola.DocumentViews;

// TODO2 indice in Sommario Dottrina cristiana ha 3 sezioni in un unico link invece di 3 link diversi

// TODO2 toolbar: lista versetti, bookmark, zoom, highlights
// TODO2 in 7, paralleli, aggiungi, noteNonAggiunte: servono?

public partial class ViewerDocumentView : UserControl, IFlowDocumentHost
{
    private string _versione;
    public string Versione
    {
        get => _versione;
        set
        {
            if (_versione == value)
                return;

            _versione = value;
        }
    }

    private Riferimento paroleRicercate = new();
    public Riferimento ParoleRicercate
    {
        get { return paroleRicercate; }
        set { paroleRicercate = value; }
    }

    public double ScrollBarValore
    {
        get { return SBViewer.Value; }
        set
        {
            if (value < SBViewer.Minimum)
                SBViewer.Value = SBViewer.Minimum;
            else if (value > SBViewer.Maximum)
                SBViewer.Value = SBViewer.Maximum;
            else
                SBViewer.Value = value;
        }
    }

    private bool isSpostando = false;
    private readonly List<Riferimento> cronologia = [];
    private int nCronologia = -1;
    private bool spostando = false;
    private bool isDraggingThumb = false;
    private bool ctrlPremuto;
    private readonly bool tipoBibbia;
    private readonly bool tipoVersetti;
    private readonly bool tipoNote;

    public byte Libro = 0, Capitolo = 1, Versetto = 1;
    internal bool VersettoMostrato = true;
    internal string Titolo = "";

    public bool IsTocVisible
    {
        get => LeftPanel.Visibility == Visibility.Visible;
        set => LeftPanel.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
    }

    private int _sincGruppo = 0;
    public int SincGruppo
    {
        get => _sincGruppo;
        set
        {
            if (value < 0 || value > 9) value = 0;
            _sincGruppo = value;
            AggiornaGraficaSincronizzazione();
        }
    }

    public ViewerDocumentView(string versione)
    {
        InitializeComponent();
        _versione = versione;
        Viewer.Lingua = MainWindow.Testi.Info(versione).Lingua;
        Viewer.Versione = versione;
        DataContext = this;

        tipoBibbia = ((MainWindow.Testi.Info(versione).Tipo & TestoTipi.Bibbia) == TestoTipi.Bibbia);
        tipoVersetti = tipoBibbia | ((MainWindow.Testi.Info(versione).Tipo & TestoTipi.Commentario) == TestoTipi.Commentario);
        tipoNote = ((MainWindow.Testi.Info(versione).Tipo & TestoTipi.Dizionario) == TestoTipi.Dizionario); // tutti il tipo Libri sono così

        if (tipoBibbia)
        {
            SBViewer.Maximum = MainWindow.Testi.CapitoliFinoALibro(73, versione) + MainWindow.Testi.CapitoliInLibro(73, versione);
        }
        else
        {
            SBViewer.Visibility = Visibility.Collapsed;
            Viewer.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            int numeroNoteTitolo = MainWindow.Testi.NumeroNoteConTitolo(versione);
            if (numeroNoteTitolo > 0)
            {
                CmbTitoloNota.Visibility = Visibility.Visible;
                CmbTitoloNota.ItemsSource = MainWindow.Testi.NoteConTitolo(versione);
                CmbTitoloNota.SelectedIndex = -1;
            }
            if (numeroNoteTitolo == MainWindow.Testi.NumeroNote(versione))
            {
                TxtReference.Visibility = Visibility.Collapsed;
                BtnGoReference.Visibility = Visibility.Collapsed;
                BorderReference.Visibility = Visibility.Collapsed;
                SPLibro.Visibility = Visibility.Collapsed;
                SPCapitolo.Visibility = Visibility.Collapsed;
                SPVersetto.Visibility = Visibility.Collapsed;
                BorderReference2.Visibility = Visibility.Collapsed;
            }
        }

        Viewer.Document = new FlowDocument
        {
            FontFamily = new System.Windows.Media.FontFamily("Georgia"),
            FontSize = 16,// 12 *4 / 3
            PageWidth = double.NaN,
            ColumnWidth = double.PositiveInfinity,
            PagePadding = new Thickness(20),
            Tag = versione
        };

        PopolaIndice();
    }

    private void HelpFlyout_OnHelpClicked(object sender, RoutedEventArgs e)
    {
        // TODO2: Open correct help section
        MessageBox.Show("Open Help Centre");
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        // Limita la larghezza massima del pannello alla metà esatta della larghezza corrente della finestra
        LeftColumn.MaxWidth = e.NewSize.Width / 2;
    }

    private void PopolaIndice()
    {
        if (TocTreeView == null) return;

        TocTreeView.Items.Clear();

        if (tipoBibbia)
        {
            for (byte numeroLibro = 1; numeroLibro <= 73; numeroLibro++)
            {
                int totaleCapitoli = MainWindow.Testi.CapitoliInLibro(numeroLibro, _versione);
                if (totaleCapitoli == 0) continue;

                string nomeLibro = MainWindow.Testi.libriNomi[numeroLibro];

                TreeViewItem libroNode = new()
                {
                    Header = nomeLibro,
                    Tag = numeroLibro
                };

                for (byte c = 1; c <= totaleCapitoli; c++)
                {
                    TreeViewItem capitoloNode = new()
                    {
                        Header = c.ToString(),
                        Tag = new Tuple<byte, byte>(numeroLibro, c)
                    };
                    libroNode.Items.Add(capitoloNode);
                }

                TocTreeView.Items.Add(libroNode);
            }
        }
        else
        {
            // sezione note in ordine
            Collection<string> noteNonAggiunte = MainWindow.Testi.NotePrimaOrdinate(_versione, true);
            Collection<string> noteInOrdine = MainWindow.Testi.GetNoteInOrdine(_versione);

            if (noteInOrdine.Count > 0)
            {
                List<TreeViewItem> ultimoALivello = [];

                for (int i = 0; i < noteInOrdine.Count; ++i)
                {
                    string stringaOriginale = noteInOrdine[i];

                    // Cleanly strip tabs and measure depth in one go
                    string titolo = stringaOriginale.TrimStart('\t');
                    int livello = stringaOriginale.Length - titolo.Length;

                    // Create the WPF native tree node element
                    TreeViewItem nuovoItem = new() { Header = titolo, Tag = titolo };
                    noteNonAggiunte.Remove(titolo);

                    // Ensure our tracking cache list has enough capacity for this level depth
                    while (livello >= ultimoALivello.Count)
                    {
                        ultimoALivello.Add(null!);
                    }

                    if (livello == 0)
                    {
                        // Add directly to the root of the TreeView
                        TocTreeView.Items.Add(nuovoItem);
                    }
                    else
                    {
                        // Grab the active parent from the preceding level depth
                        TreeViewItem padre = ultimoALivello[livello - 1];
                        padre?.Items.Add(nuovoItem);
                    }

                    // Cache this element as the latest node at this specific hierarchy level
                    ultimoALivello[livello] = nuovoItem;
                }
            }

            // sezione noteNonAggiunte su versetti

            int libroPrecedente = -1, capitoloPrecedente = -1, libro, capitolo;
            string libroNome = "";
            char letteraPrecedente = '\0';

            // Dichiarazione delle variabili dei nodi senza pre-istanziarle
            TreeViewItem ultimoLibroNodo = null!;
            TreeViewItem ultimoCapitoloNodo = null!;
            TreeViewItem ultimoLetteraNodo = null!;

            // 1. Pre-calcolo del numero di note che non iniziano con '#' per decidere la strategia di layout
            int conteggioNonHash = 0;
            foreach (string titolo in noteNonAggiunte)
            {
                if (!string.IsNullOrEmpty(titolo) && !titolo.StartsWith('#'))
                {
                    conteggioNonHash++;
                }
            }
            bool raggruppaPerLettera = conteggioNonHash >= 10;

            // 2. Ciclo principale di popolamento del TreeView
            foreach (string titolo in noteNonAggiunte)
            {
                // Clausola di salvaguardia per evitare stringhe vuote accidentali
                if (string.IsNullOrEmpty(titolo)) continue;

                if (titolo.StartsWith('#'))
                {
                    libro = Convert.ToInt32(titolo.Substring(1, 2), CultureInfo.InvariantCulture);
                    if (libro != libroPrecedente)
                    {
                        libroNome = MainWindow.Testi.GetLibroNome(libro);

                        // Crea esplicitamente il nodo del Libro
                        ultimoLibroNodo = new TreeViewItem
                        {
                            Header = libroNome,
                            Tag = libro // Memorizza il numero del libro
                        };
                        TocTreeView.Items.Add(ultimoLibroNodo);

                        libroPrecedente = libro;
                        capitoloPrecedente = -1; // Reset del tracciamento capitoli per il nuovo libro
                    }

                    capitolo = Convert.ToInt32(titolo.Substring(3, 3), CultureInfo.InvariantCulture);
                    if (capitolo != capitoloPrecedente)
                    {
                        // Se capitolo == 0, la nota è su tutto il libro
                        string capitoloHeader = libroNome + (capitolo > 0 ? (" " + capitolo.ToString(CultureInfo.InvariantCulture)) : "");

                        // Crea esplicitamente il nodo del Capitolo
                        ultimoCapitoloNodo = new TreeViewItem
                        {
                            Header = capitoloHeader,
                            Tag = new Tuple<int, int>(libro, capitolo)
                        };
                        ultimoLibroNodo.Items.Add(ultimoCapitoloNodo);

                        capitoloPrecedente = capitolo;
                    }

                    // Crea esplicitamente il nodo della singola Nota standard (#)
                    TreeViewItem ultimoNotaNodo = new()
                    {
                        Header = MainWindow.Testi.ConvertiTitoloNotaARiferimento(titolo),
                        Tag = titolo // Il riferimento come titolo di una nota, usato con i pulsanti OK e Cancella
                    };

                    ultimoCapitoloNodo.Items.Add(ultimoNotaNodo);
                }
                else
                {
                    // LOGICA PER LE NOTE SENZA '#'
                    if (raggruppaPerLettera)
                    {
                        // Estrae la prima lettera e la rende maiuscola (unificando così 'a' e 'A')
                        char letteraCorrente = char.ToUpper(titolo[0], CultureInfo.InvariantCulture);

                        if (letteraCorrente != letteraPrecedente)
                        {
                            // Crea il nodo di primo livello per la Lettera (solo se esiste una nota corrispondente)
                            ultimoLetteraNodo = new TreeViewItem
                            {
                                Header = letteraCorrente.ToString(),
                                Tag = letteraCorrente
                            };
                            TocTreeView.Items.Add(ultimoLetteraNodo);

                            letteraPrecedente = letteraCorrente;
                        }

                        // Crea il nodo della nota di secondo livello inserendolo sotto la lettera corrispondente
                        TreeViewItem notaNodo = new()
                        {
                            Header = titolo,
                            Tag = titolo // Il tag deve essere il titolo originale della nota
                        };
                        ultimoLetteraNodo.Items.Add(notaNodo);
                    }
                    else
                    {
                        // Meno di 10 note totali: le inseriamo direttamente nel livello radice dell'albero
                        TreeViewItem notaNodo = new()
                        {
                            Header = titolo,
                            Tag = titolo // Il tag deve essere il titolo originale della nota
                        };
                        TocTreeView.Items.Add(notaNodo);
                    }
                }
            }
        }
    }

    internal void MostraIndice(bool mostra)
    {
        IsTocVisible = mostra;
    }

    private void TocTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        if (TocTreeView.SelectedItem is not TreeViewItem selectedItem) return;

        // --- CASE 1: BIBLE (Chapter Node) ---
        if (selectedItem.Tag is Tuple<byte, byte> destinazioneBibbia)
        {
            byte libroTarget = destinazioneBibbia.Item1;
            byte capitoloTarget = destinazioneBibbia.Item2;

            _ = SpostaTesto(libroTarget, capitoloTarget, 1, true, true);
        }

        // --- CASE 2: COMMENTARY (Individual Note Leaf Node) ---
        else if (selectedItem.Tag is string rifNota && rifNota.StartsWith('#'))
        {
            // Extract indices matching your string padding layout (# + 2-digit book + 3-digit chapter + 3-digit verse)
            byte libro = Convert.ToByte(rifNota.Substring(1, 2), CultureInfo.InvariantCulture);
            byte capitolo = Convert.ToByte(rifNota.Substring(3, 3), CultureInfo.InvariantCulture);
            byte versetto = Convert.ToByte(rifNota.Substring(6, 3), CultureInfo.InvariantCulture);

            // Safety Guard: If a comment is written for a whole book (capitolo == 0) 
            // or a whole chapter (versetto == 0), normalize to 1 so SpostaTesto doesn't crash.
            byte targetCapitolo = (byte)(capitolo == 0 ? 1 : capitolo);
            byte targetVersetto = (byte)(versetto == 0 ? 1 : versetto);

            _ = SpostaTesto(libro, targetCapitolo, targetVersetto, true, true);
        }

        // --- CASE az: COMMENTARY (Chapter Node, optional navigation) ---
        /*
        else if (selectedItem.Tag is Tuple<int, int> destinazioneCommentario)
        {
            int libro = destinazioneCommentario.Item1;
            int capitolo = destinazioneCommentario.Item2;
            int targetCapitolo = capitolo == 0 ? 1 : capitolo;

            // Navigates to the start of that chapter when the folder item itself is selected
            _ = SpostaTesto(libro, targetCapitolo, 1, true, true);
        }*/
        // --- CASE 3: DICTIONARY AND BOOK (Chapter Node, optional navigation) ---

        else if (selectedItem.Tag is string titolo && !titolo.StartsWith('#'))
        {
            _ = SpostaTesto(titolo, true, true);
        }
    }

    private void BtnToggleToc_Click(object sender, RoutedEventArgs e)
    {
        IsTocVisible = !IsTocVisible;
    }

    public void SetDocument(FlowDocument doc)
    {
        Viewer.Document = doc;
    }

    public void CambiaFormato()
    {
        _ = SpostaTesto(Libro, Capitolo, Versetto, false, false);
        VersettoMostrato = true;
    }

    public async void SpostaTesto(Riferimento riferimento, bool aggiungiACronologia = true, bool sincronizza = true)
    {
        if (riferimento.Count < 1)
            return;
        if (!riferimento.Versetti)
        {
            _ = SpostaTesto(riferimento.Note[0], aggiungiACronologia, sincronizza);
        }
        else
        {
            _ = SpostaTesto(riferimento.Brani[0][0], riferimento.Brani[0][1], riferimento.Brani[0][2], aggiungiACronologia, sincronizza);
        }
    }

    // Overload supporting precise vertical line offsets during scrolling sync
    public async Task SpostaTesto(byte libro, byte capitolo, byte versetto, bool aggiungiACronologia = true, bool sincronizza = true, double verseTopOffset = double.NaN)
    {
        if (libro < 1) libro = 1;
        if (capitolo < 1) capitolo = 1;
        if (versetto < 1) versetto = 1;
        if (libro > 73) libro = 73;
        if (tipoBibbia)
        {
            if (capitolo > MainWindow.Testi.CapitoliInLibro(libro, _versione)) capitolo = (byte)MainWindow.Testi.CapitoliInLibro(libro, _versione);
            if (versetto > MainWindow.Testi.VersettiInCapitolo(libro, capitolo, _versione)) versetto = (byte)MainWindow.Testi.VersettiInCapitolo(libro, capitolo, _versione);
        }

        // If chapter matches, bypass database refresh entirely
        if (tipoBibbia && Libro == libro && Capitolo == capitolo && Viewer.Document != null)
        {
            ScrollToVerse(libro, capitolo, versetto, verseTopOffset);
            ScrollBarValore = MainWindow.Testi.CapitoliFinoALibro((byte)(libro - 1), Versione) + capitolo;
            Versetto = versetto;

            if (sincronizza)
                SpostaAltreViewer(new Riferimento(libro, capitolo, versetto), aggiungiACronologia, verseTopOffset);

            if (aggiungiACronologia)
            {
                for (int i = cronologia.Count - 1; i > nCronologia; i--) cronologia.RemoveAt(i);
                cronologia.Add(new Riferimento(libro, capitolo, versetto));
                nCronologia = cronologia.Count - 1;
                AggiornaCronologia();
            }
            return;
        }

        try
        {
            if (tipoBibbia)
            {
                StringBuilder riferimento = new();
                riferimento.Append(MainWindow.Testi.LibriAbbreviazioniRiconosciute.Abbreviazione(libro)).Append(capitolo).Append('-');
                byte libroFine = (byte)(libro - 1);
                UInt16 capitoloFine = (UInt16)(MainWindow.Testi.CapitoliFinoALibro(libroFine, _versione) + capitolo + 5);
                do
                {
                    ++libroFine;
                } while (libroFine < 73 && MainWindow.Testi.CapitoliFinoALibro(libroFine, _versione) < capitoloFine);
                riferimento.Append(MainWindow.Testi.LibriAbbreviazioniRiconosciute.Abbreviazione(libroFine)).Append(capitoloFine - MainWindow.Testi.CapitoliFinoALibro((byte)(libroFine - 1), _versione));
                Viewer.Document = await MainWindow.Testi.FlowDocumentBranoAsync(riferimento.ToString(), _versione, paroleRicercate: ParoleRicercate);
            }
            else
            {
                Viewer.Document = await MainWindow.Testi.FlowDocumentBranoAsync(new Riferimento(libro, capitolo, versetto), _versione, paroleRicercate: ParoleRicercate);
            }

            Brush fg = (Brush)Application.Current.FindResource("AppForegroundBrush");
            RtfColorTransformer.ApplyThemeToDocument(Viewer.Document, true, fg, true);
        }
        catch
        {
            return;
        }

        if (tipoBibbia)
        {
            await Dispatcher.InvokeAsync(() =>
                {
                    ScrollToVerse(libro, capitolo, versetto, verseTopOffset);
                }, System.Windows.Threading.DispatcherPriority.Background);

            ScrollBarValore = MainWindow.Testi.CapitoliFinoALibro((byte)(libro - 1), Versione) + capitolo;
        }

        Libro = libro;
        Capitolo = capitolo;
        Versetto = versetto;

        if (sincronizza)
            SpostaAltreViewer(new Riferimento(libro, capitolo, versetto), aggiungiACronologia, verseTopOffset);

        if (aggiungiACronologia)
        {
            for (int i = cronologia.Count - 1; i > nCronologia; i--)
            {
                cronologia.RemoveAt(i);
            }
            cronologia.Add(new Riferimento(libro, capitolo, versetto));
            nCronologia = cronologia.Count - 1;
            AggiornaCronologia();
        }

        VersettoMostrato = true;
    }

    public async Task SpostaTesto(string notaTitolo, bool aggiungiACronologia = true, bool sincronizza = true)
    {
        Riferimento rif = new(false);
        rif.AggiungiNotaEParole(notaTitolo, []);
        Viewer.Document = await MainWindow.Testi.FlowDocumentBranoAsync(rif, _versione, paroleRicercate: ParoleRicercate);

        Brush fg = (Brush)Application.Current.FindResource("AppForegroundBrush");
        RtfColorTransformer.ApplyThemeToDocument(Viewer.Document, true, fg, true);

        isSpostando = true;
        CmbTitoloNota.SelectedItem = notaTitolo;
        isSpostando = false;

        if (sincronizza)
            SpostaAltreViewer(rif, aggiungiACronologia);

        if (aggiungiACronologia)
        {
            for (int i = cronologia.Count - 1; i > nCronologia; i--)
            {
                cronologia.RemoveAt(i);
            }
            cronologia.Add(rif);
            nCronologia = cronologia.Count - 1;
            AggiornaCronologia();
        }

        Titolo = notaTitolo;
        VersettoMostrato = false;
    }

    private void SpostaAltreViewer(bool cronologia = true)
    {
        if (tipoBibbia)
        {
            var state = GetCurrentTopVerseState();
            if (!string.IsNullOrEmpty(state.Tag))
            {
                string verseTag = state.Tag;
                byte currentBook = byte.Parse(verseTag.Substring(6, 2));
                byte currentChapter = byte.Parse(verseTag.Substring(8, 3));
                byte currentVerse = byte.Parse(verseTag.Substring(11, 3));
                SpostaAltreViewer(new Riferimento(currentBook, currentChapter, currentVerse), cronologia, state.VerseTopOffset);
            }
        }
        else
        {
            SpostaAltreViewer(new Riferimento(Libro, Capitolo, Versetto), cronologia);
        }
    }

    private void SpostaAltreViewer(Riferimento riferimento, bool cronologia = true, double verseTopOffset = double.NaN)
    {
        if (riferimento.Count < 1)
            return;

        if (SincGruppo != 0)
        {
            List<LayoutDocument>? viewers = Funzioni.ListViewerDocuments();
            if (viewers != null)
            {
                foreach (LayoutDocument d in viewers)
                {
                    ViewerDocumentView? vd = (d.Content as ViewerDocumentView);
                    if (vd != null && vd != this && vd.SincGruppo == SincGruppo)
                        if (riferimento.Versetti)
                        {
                            if (vd.tipoVersetti)
                            {
                                Riferimento rif = MainWindow.Testi.ConvertiDaStandard(MainWindow.Testi.ConvertiAStandard(riferimento, _versione), vd.Versione);
                                // Explicitly call the byte overload to maintain micro scroll precision offsets
                                _ = vd.SpostaTesto(rif.Brani[0][0], rif.Brani[0][1], rif.Brani[0][2], cronologia, false, verseTopOffset);
                            }
                        }
                        else
                        {
                            if (vd.tipoNote)
                            {
                                _ = vd.SpostaTesto(riferimento.Note[0], cronologia, false);
                            }
                        }
                }
            }
            if (tipoBibbia)
                MainWindow.Testi.UltimaBibbia = Versione;
        }
    }

    public void ScrollToVerse(int libro, int capitolo, int versetto, double verseTopOffset = double.NaN)
    {
        if (!tipoBibbia)
            return;

        // Enforce immediate rendering loop check before measuring coordinates
        Viewer.UpdateLayout();

        string targetVerseId = $"VERSE_{libro:D2}{capitolo:D3}{versetto:D3}";
        TextPointer navigator = Viewer.Document.ContentStart;

        while (navigator != null && navigator.CompareTo(Viewer.Document.ContentEnd) < 0)
        {
            if (navigator.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementStart)
            {
                DependencyObject obj = navigator.GetAdjacentElement(LogicalDirection.Forward);

                if (obj is FrameworkContentElement fce && fce.Tag?.ToString() == targetVerseId)
                {
                    Viewer.CaretPosition = navigator;
                    Viewer.UpdateLayout();

                    Rect characterRect = navigator.GetCharacterRect(LogicalDirection.Forward);

                    // If hidden layout calculations yield empty space, fallback gracefully
                    if (characterRect == Rect.Empty || double.IsInfinity(characterRect.Top))
                    {
                        fce.BringIntoView();
                        return;
                    }

                    double targetOffset;
                    if (!double.IsNaN(verseTopOffset))
                    {
                        // Match line alignment pixel-for-pixel across frames
                        targetOffset = Viewer.VerticalOffset + characterRect.Top - verseTopOffset;
                    }
                    else
                    {
                        targetOffset = Viewer.VerticalOffset + characterRect.Top;
                    }

                    if (targetOffset < 0) targetOffset = 0;
                    if (targetOffset > Viewer.ExtentHeight) targetOffset = Viewer.ExtentHeight;

                    Viewer.ScrollToVerticalOffset(targetOffset);
                    return;
                }
            }
            navigator = navigator.GetNextContextPosition(LogicalDirection.Forward);
        }
    }

    private void TxtReference_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == System.Windows.Input.Key.Enter || e.Key == System.Windows.Input.Key.Return)
        {
            EseguiSpostamentoRiferimento(TxtReference.Text);
            e.Handled = true;
        }
    }

    private void BtnGoReference_Click(object sender, RoutedEventArgs e)
    {
        EseguiSpostamentoRiferimento(TxtReference.Text);
    }

    private void EseguiSpostamentoRiferimento(string input)
    {
        if (!string.IsNullOrWhiteSpace(input))
        {
            Riferimento r = MainWindow.Testi.ConvertiRiferimento(input);
            if (r.Count >= 1)
            {
                SpostaTesto(r);
            }
        }
    }

    private void EseguiSpostamentoNota(string titoloNota)
    {
        if (!string.IsNullOrWhiteSpace(titoloNota))
        {
            Riferimento rif = new()
            {
                Versetti = false
            };
            rif.AggiungiNotaEParole(titoloNota, []);
            SpostaTesto(rif);
        }
    }

    private void BtnPrevBook_Click(object sender, RoutedEventArgs e) { SaltaTesto(-1, 0, 0); }
    private void BtnNextBook_Click(object sender, RoutedEventArgs e) { SaltaTesto(1, 0, 0); }
    private void BtnPrevChapter_Click(object sender, RoutedEventArgs e) { SaltaTesto(0, -1, 0); }
    private void BtnNextChapter_Click(object sender, RoutedEventArgs e) { SaltaTesto(0, 1, 0); }
    private void BtnPrevVerse_Click(object sender, RoutedEventArgs e) { SaltaTesto(0, 0, -1); }
    private void BtnNextVerse_Click(object sender, RoutedEventArgs e) { SaltaTesto(0, 0, 1); }

    private void SaltaTesto(int deltaLibro, int deltaCapitolo, int deltaVersetto)
    {
        byte nuovoVersetto = (byte)(Versetto + deltaVersetto);
        byte nuovoCapitolo = (byte)(Capitolo + deltaCapitolo);
        byte nuovoLibro = (byte)(Libro + deltaLibro);
        if (deltaCapitolo != 0) nuovoVersetto = 1;
        if (deltaLibro != 0) nuovoCapitolo = 1;

        string versione = _versione;
        if (!MainWindow.Testi.VersioneEsiste(versione))
            return;
        if (!tipoBibbia)
            versione = MainWindow.Testi.UltimaBibbiaCompleta;

        try
        {
            if (nuovoVersetto > MainWindow.Testi.VersettiInCapitolo(nuovoLibro, nuovoCapitolo, versione))
            {
                nuovoCapitolo += 1;
                nuovoVersetto = 1;
            }
        }
        catch { }

        if (nuovoLibro > 1 && MainWindow.Testi.CapitoliFinoALibro(nuovoLibro, versione) > 0)
        {
            try
            {
                if (nuovoCapitolo > MainWindow.Testi.CapitoliInLibro(nuovoLibro, versione) && nuovoLibro >= 1)
                {
                    do
                    {
                        nuovoLibro = (byte)(nuovoLibro + deltaLibro);
                    } while (nuovoLibro >= 1 && nuovoLibro <= 73 && MainWindow.Testi.CapitoliInLibro(nuovoLibro, versione) == 0);
                    nuovoCapitolo = 1;
                    nuovoVersetto = 1;
                }
            }
            catch { }
        }
        if (nuovoLibro > 73)
        {
            nuovoLibro = 74;
            do
            {
                nuovoLibro -= 1;
            } while (MainWindow.Testi.CapitoliInLibro(nuovoLibro, versione) == 0);
            nuovoCapitolo = MainWindow.Testi.CapitoliInLibro(nuovoLibro, versione);
            nuovoVersetto = MainWindow.Testi.VersettiInCapitolo(nuovoLibro, nuovoCapitolo, versione);
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
                nuovoVersetto = MainWindow.Testi.VersettiInCapitolo(nuovoLibro, nuovoCapitolo, versione);
            }
            else
            {
                if (nuovoLibro > 1)
                    UltimoCapitoloInLibroPrecedente(ref nuovoLibro, ref nuovoCapitolo, ref nuovoVersetto, versione);
                else
                    nuovoVersetto = 1;
            }
        }

        _ = SpostaTesto(nuovoLibro, nuovoCapitolo, nuovoVersetto);
    }

    private static void UltimoCapitoloInLibroPrecedente(ref byte nuovoLibro, ref byte nuovoCapitolo, ref byte nuovoVersetto, string versione)
    {
        if (nuovoLibro > 1)
            --nuovoLibro;
        try
        {
            while (nuovoLibro >= 1 && MainWindow.Testi.CapitoliInLibro(nuovoLibro, versione) == 0)
                --nuovoLibro;
            nuovoCapitolo = MainWindow.Testi.CapitoliInLibro(nuovoLibro, versione);
        }
        catch (ArgumentOutOfRangeException)
        {
            nuovoCapitolo = 1;
        }
        if (nuovoCapitolo < 1)
            nuovoCapitolo = 1;
        try
        {
            nuovoVersetto = MainWindow.Testi.VersettiInCapitolo(nuovoLibro, nuovoCapitolo, versione);
        }
        catch (ArgumentOutOfRangeException)
        {
            nuovoVersetto = 1;
        }
    }

    private void TxtSearchWord_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == System.Windows.Input.Key.Enter || e.Key == System.Windows.Input.Key.Return)
        {
            EseguiRicercaParola();
            e.Handled = true;
        }
    }

    private void BtnExecuteSearch_Click(object sender, RoutedEventArgs e)
    {
        EseguiRicercaParola();
    }

    private void EseguiRicercaParola()
    {
        string espressione = TxtSearchWord.Text;
        if (string.IsNullOrWhiteSpace(espressione)) return;

        Riferimento? versettiConFrase = null;
        try
        {
            versettiConFrase = MainWindow.Testi.Ricerca(espressione, "", _versione);
        }
        catch (SearchParenthesesException)
        {
            MessageBoxLPN.Show(Window.GetWindow(this), (string)(Application.Current.TryFindResource("RicercaErroreParentesi") ?? "The parentheses in the search expression are not balanced."), (string)(Application.Current.TryFindResource("Errore") ?? "Error"));
        }
        catch (SearchBracketsException)
        {
            MessageBoxLPN.Show(Window.GetWindow(this), (string)(Application.Current.TryFindResource("RicercaErroreParentesiQuadrate") ?? "The square brackets in the search expression are not balanced."), (string)(Application.Current.TryFindResource("Errore") ?? "Error"));
        }
        catch (SearchSyntaxErrorException ex)
        {
            MessageBoxLPN.Show(Window.GetWindow(this), String.Format(CultureInfo.InvariantCulture, (string)(Application.Current.TryFindResource("RicercaErroreSintasi") ?? "The syntax of the search expression is incorrect at about character number {0}."), ex.Message), (string)(Application.Current.TryFindResource("Errore") ?? "Error"));
        }
        catch
        {
            return;
        }

        if (versettiConFrase != null)
            ParoleRicercate = versettiConFrase;

        if (versettiConFrase != null && versettiConFrase.Count > 0)
        {
            List<NotaSearchResult> risultati = [];
            if (tipoBibbia)
            {
                foreach (byte[] brano in versettiConFrase.Brani)
                {
                    risultati.Add(new NotaSearchResult
                    {
                        DisplayText = MainWindow.Testi.NormalizzaRiferimento(brano[0], brano[1], brano[2]),
                        RawNota = "", // non è utilizzato
                        IsReference = true
                    });
                }
            }
            else
            {
                foreach (string nota in versettiConFrase.Note)
                {
                    if (nota.StartsWith('#'))
                    {
                        risultati.Add(new NotaSearchResult
                        {
                            DisplayText = MainWindow.Testi.ConvertiTitoloNotaARiferimento(nota),
                            RawNota = nota,
                            IsReference = true
                        });
                    }
                    else
                        risultati.Add(new NotaSearchResult
                        {
                            DisplayText = nota,
                            RawNota = nota,
                            IsReference = false
                        });
                }
            }
            CmbSearchResults.DisplayMemberPath = nameof(NotaSearchResult.DisplayText);
            CmbSearchResults.ItemsSource = risultati;
            CmbSearchResults.IsEnabled = true;
            CmbSearchResults.SelectedIndex = 0;
        }
        else
        {
            CmbSearchResults.ItemsSource = null;
            CmbSearchResults.IsEnabled = false;
        }
    }

    private void CmbTitoloNota_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (isSpostando)
            return;
        if (CmbTitoloNota.SelectedItem is string selectedTitle)
        {
            _ = SpostaTesto(selectedTitle);
        }
    }

    private void CmbSearchResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CmbSearchResults.IsEnabled && CmbSearchResults.SelectedItem is NotaSearchResult selectedItem)
        {
            if (selectedItem.IsReference)
            {
                EseguiSpostamentoRiferimento(selectedItem.DisplayText);
            }
            else
            {
                // Call your alternative function here for non-# text string titles
                EseguiSpostamentoNota(selectedItem.RawNota);
            }
        }
    }

    private void Indietro_Click(object sender, RoutedEventArgs e)
    {
        if (nCronologia > 0)
        {
            nCronologia--;
            SpostaTesto(cronologia[nCronologia], false, true);
            AggiornaCronologia();
        }
    }

    private void Avanti_Click(object sender, RoutedEventArgs e)
    {
        if (nCronologia < cronologia.Count - 1)
        {
            nCronologia++;
            SpostaTesto(cronologia[nCronologia], false, true);
            AggiornaCronologia();
        }
    }

    private void AggiornaCronologia()
    {
        BtnAvanti.IsEnabled = (nCronologia < cronologia.Count - 1);
        BtnIndietro.IsEnabled = (nCronologia > 0);
    }

    private void BtnSync_Click(object sender, RoutedEventArgs e)
    {
        if (BtnSync.ContextMenu != null)
        {
            if (Application.Current.TryFindResource("ControlBackgroundBrush") is Brush currentThemeBrush)
            {
                BtnSync.ContextMenu.Resources[SystemColors.MenuBrushKey] = currentThemeBrush;
                BtnSync.ContextMenu.Resources[SystemColors.MenuBarBrushKey] = currentThemeBrush;
            }

            BtnSync.ContextMenu.PlacementTarget = BtnSync;
            BtnSync.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            BtnSync.ContextMenu.IsOpen = true;
        }
    }

    private void MenuSyncItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem item && item.Tag is string tagStr && int.TryParse(tagStr, out int gruppoScelto))
        {
            SincGruppo = gruppoScelto;

            List<LayoutDocument>? viewers = Funzioni.ListViewerDocuments();

            if (viewers != null)
            {
                foreach (LayoutDocument d in viewers)
                {
                    ViewerDocumentView? vd = (d.Content as ViewerDocumentView);
                    if (vd != null && vd != this && vd.SincGruppo == SincGruppo)
                    {
                        Riferimento rif = MainWindow.Testi.ConvertiDaStandard(MainWindow.Testi.ConvertiAStandard(new Riferimento(vd.Libro, vd.Capitolo, vd.Versetto), vd.Versione), _versione);
                        SpostaTesto(rif, true, false);
                    }
                }
            }
        }
    }

    private void AggiornaGraficaSincronizzazione()
    {
        if (IconSyncBrano == null || BadgeSync == null || TxtSyncGroup == null) return;

        if (_sincGruppo == 0)
        {
            IconSyncBrano.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.LinkOff;
            BadgeSync.Visibility = Visibility.Collapsed;
            BtnSync.ToolTip = (string)(Application.Current.TryFindResource("ViewerSincAiuto0") ?? "Window not synchronized. To synchronize the text, click the button and choose the same number in this and in another window.");
        }
        else
        {
            IconSyncBrano.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.Link;
            TxtSyncGroup.Text = _sincGruppo.ToString();
            BadgeSync.Visibility = Visibility.Visible;
            string messageTemplate = (string)(Application.Current.TryFindResource("ViewerSincAiutoN") ?? $"Window synchronized with the other windows in group {{0}}.");
            BtnSync.ToolTip = string.Format(messageTemplate, _sincGruppo);
        }

        if (BtnSync.ContextMenu != null)
        {
            foreach (var item in BtnSync.ContextMenu.Items)
            {
                if (item is MenuItem menuItem && menuItem.Tag is string t)
                {
                    menuItem.IsChecked = (t == _sincGruppo.ToString());
                }
            }
        }
    }

    private void RtfTesto_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        ctrlPremuto = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
        bool isShiftPressed = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;

        if (tipoBibbia)
        {
            if (!isShiftPressed)
            {
                switch (e.Key)
                {
                    case Key.Down:
                        SBViewer_Scroll(sender, new ScrollEventArgs(ScrollEventType.SmallIncrement, 0));
                        e.Handled = true;
                        break;

                    case Key.End:
                        if (ctrlPremuto)
                        {
                            SBViewer_Scroll(sender, new ScrollEventArgs(ScrollEventType.Last, 0));
                            e.Handled = true;
                        }
                        break;

                    case Key.Home:
                        if (ctrlPremuto)
                        {
                            SBViewer_Scroll(sender, new ScrollEventArgs(ScrollEventType.First, 0));
                            e.Handled = true;
                        }
                        break;

                    case Key.PageDown:
                        SBViewer_Scroll(sender, new ScrollEventArgs(ScrollEventType.LargeIncrement, 0));
                        e.Handled = true;
                        break;

                    case Key.PageUp:
                        SBViewer_Scroll(sender, new ScrollEventArgs(ScrollEventType.LargeDecrement, 0));
                        e.Handled = true;
                        break;

                    case Key.Up:
                        SBViewer_Scroll(sender, new ScrollEventArgs(ScrollEventType.SmallDecrement, 0));
                        e.Handled = true;
                        break;
                }
            }
        }
        else
        {
            // Force direct viewport scrolling layout commands because 
            // EditingCommands are disabled when IsReadOnly="True"
            if (sender is RichTextBox rtb)
            {
                switch (e.Key)
                {
                    case Key.Down:
                        rtb.LineDown();
                        e.Handled = true;
                        break;

                    case Key.Up:
                        rtb.LineUp();
                        e.Handled = true;
                        break;

                    case Key.PageDown:
                        rtb.PageDown();
                        e.Handled = true;
                        break;

                    case Key.PageUp:
                        rtb.PageUp();
                        e.Handled = true;
                        break;

                    case Key.Home:
                        if (ctrlPremuto)
                        {
                            rtb.ScrollToHome();
                            e.Handled = true;
                        }
                        break;

                    case Key.End:
                        if (ctrlPremuto)
                        {
                            rtb.ScrollToEnd();
                            e.Handled = true;
                        }
                        break;

                    default:
                        e.Handled = false;
                        break;
                }
            }
        }
    }

    private void RtfTesto_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
        {
            ctrlPremuto = false;
        }
    }

    private void Viewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (!tipoBibbia)
            return;

        if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
        {
            return;
        }

        e.Handled = true;
        int wheelScrollLines = SystemParameters.WheelScrollLines;
        int righe = Math.Abs(e.Delta * wheelScrollLines / 120);

        if (e.Delta > 0)
        {
            for (int i = 0; i < righe; ++i)
            {
                SBViewer_Scroll(sender, new ScrollEventArgs(ScrollEventType.SmallDecrement, 0));
            }
        }
        else if (e.Delta < 0)
        {
            for (int i = 0; i < righe; ++i)
            {
                SBViewer_Scroll(sender, new ScrollEventArgs(ScrollEventType.SmallIncrement, 0));
            }
        }
    }

    private async void SBViewer_Scroll(object sender, ScrollEventArgs e)
    {
        if (!tipoBibbia)
        {
            return; // SBViewer non esiste se non Bibbia, ma controlliamo comunque
        }

        // Re-entry lock safeguard structured inside thread safety scopes
        if (spostando) return;

        try
        {
            spostando = true;

            switch (e.ScrollEventType)
            {
                case ScrollEventType.First:
                    byte libroPrimo = 1;
                    for (byte i = 1; i <= 73; ++i)
                    {
                        if (MainWindow.Testi.CapitoliInLibro(i, _versione) > 0)
                        {
                            libroPrimo = i;
                            break;
                        }
                    }
                    await SpostaTesto(libroPrimo, 1, 1, true, true);
                    break;

                case ScrollEventType.Last:
                    byte libroUltimo = 73;
                    for (byte i = 73; i >= 1; --i)
                    {
                        if (MainWindow.Testi.CapitoliInLibro(i, _versione) > 0)
                        {
                            libroUltimo = i;
                            break;
                        }
                    }
                    byte cap = MainWindow.Testi.CapitoliInLibro(libroUltimo, _versione);
                    await SpostaTesto(libroUltimo, cap, MainWindow.Testi.VersettiInCapitolo(libroUltimo, cap, _versione), true, true);
                    break;

                case ScrollEventType.SmallDecrement:
                    if (Viewer.VerticalOffset < 2)
                    {
                        await LoadNewSlidingWindow(() => Viewer.LineUp());
                    }
                    else
                    {
                        Viewer.LineUp();
                    }
                    SpostaAltreViewer(false);
                    break;

                case ScrollEventType.SmallIncrement:
                    if (Viewer.VerticalOffset >= Viewer.ExtentHeight - Viewer.ViewportHeight - 1)
                    {
                        await LoadNewSlidingWindow(() => Viewer.LineDown());
                    }
                    else
                    {
                        Viewer.LineDown();
                    }
                    SpostaAltreViewer(false);
                    break;

                case ScrollEventType.LargeDecrement:
                    if (Viewer.VerticalOffset < Viewer.ViewportHeight)
                    {
                        await LoadNewSlidingWindow(() => Viewer.PageUp());
                    }
                    else
                    {
                        Viewer.PageUp();
                    }
                    SpostaAltreViewer(false);
                    break;

                case ScrollEventType.LargeIncrement:
                    if (Viewer.VerticalOffset > Viewer.ExtentHeight - 2 * Viewer.ViewportHeight - 1)
                    {
                        await LoadNewSlidingWindow(() => Viewer.PageDown());
                    }
                    else
                    {
                        Viewer.PageDown();
                    }
                    SpostaAltreViewer(false);
                    break;

                case ScrollEventType.EndScroll:
                    if (isDraggingThumb)
                    {
                        isDraggingThumb = false;
                        byte lib = 0;
                        double sbValue = e.NewValue;
                        do
                        {
                            lib++;
                        } while (lib <= 73 && MainWindow.Testi.CapitoliFinoALibro(lib, _versione) < sbValue);
                        await SpostaTesto((byte)(lib - 1), (byte)(sbValue - MainWindow.Testi.CapitoliFinoALibro((byte)(lib - 1), _versione)), 1, true, true);
                    }
                    break;

                case ScrollEventType.ThumbTrack:
                    isDraggingThumb = true;
                    break;
            }
        }
        finally
        {
            spostando = false;
        }
    }

    private (string? Tag, double VerseTopOffset) GetCurrentTopVerseState()
    {
        Viewer.UpdateLayout();
        // Locate the document position exactly at the top-left coordinate of the viewport
        TextPointer topPointer = Viewer.GetPositionFromPoint(new Point(0, 0), true);
        if (topPointer != null)
        {
            TextPointer scanner = topPointer;
            while (scanner != null)
            {
                // Match against TextElement to expose ContentStart safely
                if (scanner.Parent is TextElement te && te.Tag != null)
                {
                    string s = te.Tag.ToString() ?? "";
                    if (s.StartsWith("VERSE_"))
                    {
                        Viewer.UpdateLayout();
                        // Measure layout space relative to the absolute start boundary of the element
                        Rect verseStartRect = te.ContentStart.GetCharacterRect(LogicalDirection.Forward);
                        return (s, verseStartRect.Top);
                    }
                }
                scanner = scanner.GetNextContextPosition(LogicalDirection.Backward);
            }
        }
        return (null, 0);
    }

    private static TextPointer? FindPointerForTag(FlowDocument doc, string tag)
    {
        TextPointer navigator = doc.ContentStart;
        while (navigator != null && navigator.CompareTo(doc.ContentEnd) < 0)
        {
            if (navigator.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementStart)
            {
                if (navigator.GetAdjacentElement(LogicalDirection.Forward) is FrameworkContentElement fce)
                {
                    if (fce.Tag?.ToString() == tag)
                    {
                        return navigator;
                    }
                }
            }
            navigator = navigator.GetNextContextPosition(LogicalDirection.Forward);
        }
        return null;
    }

    private async Task LoadNewSlidingWindow(Action postScrollAction)
    {
        var state = GetCurrentTopVerseState();

        if (!string.IsNullOrEmpty(state.Tag))
        {
            string verseTag = state.Tag;
            byte currentBook = byte.Parse(verseTag.Substring(6, 2));
            byte currentChapter = byte.Parse(verseTag.Substring(8, 3));

            int nuovoCapitolo = MainWindow.Testi.CapitoliFinoALibro((byte)(currentBook - 1), Versione) + currentChapter - 2;
            byte libroDiCapitolo = MainWindow.Testi.LibroDiCapitolo(nuovoCapitolo, Versione);
            nuovoCapitolo -= MainWindow.Testi.CapitoliFinoALibro((byte)(libroDiCapitolo - 1), Versione);
            if (nuovoCapitolo < 1)
                nuovoCapitolo = 1;
            await SpostaTesto(libroDiCapitolo, (byte)(nuovoCapitolo), 1, false, false);
        }

        Viewer.Opacity = 0;
        Viewer.UpdateLayout();

        if (!string.IsNullOrEmpty(state.Tag))
        {
            TextPointer? newPointer = FindPointerForTag(Viewer.Document, state.Tag);

            if (newPointer != null)
            {
                Rect rectStart = Viewer.Document.ContentStart.GetCharacterRect(LogicalDirection.Forward);
                Rect rectTarget = newPointer.GetCharacterRect(LogicalDirection.Forward);

                double absoluteTopInNewDoc = rectTarget.Top - rectStart.Top;
                double targetScrollOffset = absoluteTopInNewDoc - state.VerseTopOffset;

                Viewer.ScrollToVerticalOffset(targetScrollOffset);
                Viewer.UpdateLayout();
            }
        }

        postScrollAction?.Invoke();
        Viewer.Opacity = 1;
    }

    public class NotaSearchResult
    {
        public string DisplayText { get; set; } = string.Empty;
        public string RawNota { get; set; } = string.Empty;
        public bool IsReference { get; set; }
    }
}