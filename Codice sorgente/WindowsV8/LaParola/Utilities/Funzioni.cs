using AvalonDock.Layout;
using LaParola.ToolViews;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace LaParola.Utilities
{
    class Funzioni
    {
        internal static string AggiungiZero(string stringa, int lunghezza)
        {
            string s1 = new String('0', lunghezza) + stringa;
            return s1[^lunghezza..];
        }

        internal static string AggiungiZero(int numero, int lunghezza)
        {
            return AggiungiZero(numero.ToString(CultureInfo.InvariantCulture), lunghezza);
        }

        public static string[] SplitString(string stringa, char divisore)
        {
            return SplitString(stringa, [divisore]);
        }

        public static string[] SplitString(string stringa, char[] divisore)
        {
            return stringa.Split(divisore, StringSplitOptions.RemoveEmptyEntries);
        }

        public static bool IsLettera(char c)
        { // anche in funzioni.cs
            return (Char.IsLetter(c) || Char.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark || (c >= '\u02be' && c <= '\u02bf')); // gli ultimi caratteri sono usati nella traslitterazione dell'ebraico
        }

        public static bool IsLetteraONumero(char c)
        {
            return (Char.IsLetterOrDigit(c) || Char.GetUnicodeCategory(c) == UnicodeCategory.OtherNumber || Char.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark || (c >= '\u02be' && c <= '\u02bf'));
        }

        public static bool IsLetteraEbraica(char c)
        {
            return ((c >= '\u0591' && c <= '\u05f4') || (c >= '\ufb1e' && c <= '\ufb4f'));
        }

        public static bool IsLetteraGreca(char c)
        { // anche in funzioni.cs
            return ((c >= '\u0370' && c <= '\u03ff') || (c >= '\u1f00' && c <= '\u1fff'));
        }

        /// <summary>
        /// Data una stringa con diverse lingue separate da una riga verticale |, restituisce la prima
        /// </summary>
        /// <param name="lingua">Un elenco di lingue separate da una riga verticale.</param>
        /// <returns>La lingua principale.</returns>
        public static string LinguaPrincipale(string lingua)
        { // anche in funzioni.cs, Light
            if (!string.IsNullOrEmpty(lingua))
            {
                return SplitString(lingua, '|')[0].ToLower(CultureInfo.InvariantCulture);
            }
            else
            {
                return "";
            }
        }

        public static bool RightToLeft(string lingua)
        { // anche in funzioni.cs
            string linguaPrincipale = LinguaPrincipale(lingua);
            return (linguaPrincipale == "he" || linguaPrincipale == "ar");
        }

        public static string ConvertiUnicodeInRtf(string rtf)
        {
            if (string.IsNullOrEmpty(rtf))
                return string.Empty;

            StringBuilder sb = new(rtf.Length + 256);

            Span<char> buffer = stackalloc char[6];
            foreach (char c in rtf)
            {
                if (c < 128)
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append(@"\u");
                    if (((int)c).TryFormat(buffer, out int written))
                    {
                        sb.Append(buffer[..written]);
                    }
                    sb.Append('?');
                }
            }

            return sb.ToString();
        }

        public static List<LayoutDocument>? ListViewerDocuments()
        {
            if (Application.Current.MainWindow is MainWindow mw)
            {
                if (mw.FindName("Dock") is AvalonDock.DockingManager dock)
                {
                    LayoutRoot? root = dock.Layout;
                    if (root != null)
                    {
                        List<LayoutDocument> viewers = [.. root.Descendents()
                          .OfType<LayoutDocument>()
                          .Where(d => (d.ContentId ?? "").StartsWith("doc.viewer."))];
                        return viewers;
                    }
                }
            }
            return null;
        }

        #region ApriBrowser

        /// <summary>
        /// Apre un indirizzo Internet nel browser predefinito.
        /// </summary>
        /// <param name="url">L'indirizzo da aprire.</param>
        /// <param name="throwException">Se un eventuale errore è passato a chi ha chiamato questa routine, oppure è ignorato.</param>
        internal static void AprilFileOUrl(Uri url, bool throwException)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));
            else
                AprilFileOUrl(url.ToString(), "", throwException);
        }

        /// <summary>
        /// Apre un indirizzo Internet nel browser predefinito.
        /// </summary>
        /// <param name="url">L'indirizzo da aprire.</param>
        /// <param name="throwException">Se un eventuale errore è passato a chi ha chiamato questa routine, oppure è ignorato.</param>
        internal static void AprilFileOUrl(string url, bool throwException)
        {
            AprilFileOUrl(url, "", throwException);
        }

        /// <summary>
        /// Apre un indirizzo Internet nel browser predefinito.
        /// </summary>
        /// <param name="url">L'indirizzo da aprire.</param>
        /// <param name="parametri">Gli eventuali parametri dell'indirizzo.</param>
        /// <param name="throwException">Se un eventuale errore è passato a chi ha chiamato questa routine, oppure è ignorato.</param>
        internal static void AprilFileOUrl(Uri url, string parametri, bool throwException)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));
            else
                AprilFileOUrl(url.ToString(), parametri, throwException);
        }

        /// <summary>
        /// Apre un indirizzo Internet nel browser predefinito.
        /// </summary>
        /// <param name="url">L'indirizzo da aprire.</param>
        /// <param name="parametri">Gli eventuali parametri dell'indirizzo.</param>
        /// <param name="throwException">Se un eventuale errore è passato a chi ha chiamato questa routine, oppure è ignorato.</param>
        internal static void AprilFileOUrl(string url, string parametri, bool throwException)
        {
            if (string.IsNullOrWhiteSpace(url)) return;

            // 1. Salva l'eventuale cursore override attualmente attivo in WPF
            Cursor cursoreAttuale = Mouse.OverrideCursor;

            try
            {
                // 2. Imposta il cursore di attesa nativo di WPF a livello di applicazione
                Mouse.OverrideCursor = Cursors.AppStarting;

                // 3. Configura ProcessStartInfo (Obbligatorio per i link web in .NET moderno)
                ProcessStartInfo psi = new()
                {
                    FileName = url,
                    Arguments = parametri ?? string.Empty,
                    UseShellExecute = true // Permette a Windows di capire che è un URL e aprire il browser
                };

                Process.Start(psi);
            }
            catch (Exception)
            {
                if (throwException)
                    throw;
            }
            finally
            {
                // 4. Ripristina il cursore precedente (senza chiamare Dispose(), ci pensa WPF)
                Mouse.OverrideCursor = cursoreAttuale;
            }
        }
        #endregion

        /// <summary>
        /// Aggiungi le radici delle parole ad un testo.
        /// </summary>
        /// <param name="cartellaDeiFile">La cartella in cui si trova i file *.parole_radici con contengono le radici.</param>
        /// <param name="linguePreferite">La lingua normale del testo (radici in questa lingua hanno preferenza sulle altre).</param>
        /// <param name="elencoParoleInVersione">Tutte le parole nel testo.</param>
        /// <param name="listaRadici">Una lista di tutte le radici usate.</param>
        /// <returns>La radice di ogni parola nel testo.</returns>
        public static string[] AggiungiRadiciDaFile(string? cartellaDeiFile, string linguePreferite, string[] elencoParoleInVersione, List<string> listaRadici)
        {
            if (string.IsNullOrEmpty(cartellaDeiFile))
                return [];

            ConfrontoCI confrontoParole = new();
            string[] fileParoleRadici = Directory.GetFiles(cartellaDeiFile, "*.parole_radici");
            // i file devono essere UTF-8
            foreach (string linguaPreferita in linguePreferite.ToUpperInvariant().Split(['|'], StringSplitOptions.RemoveEmptyEntries))
            {
                if (linguaPreferita.Length >= 2)
                {
                    // far sì che la lingua del testo abbia precedenza sulle altre lingue presenti nel testo
                    bool trovato = false;
                    for (int i = 0; i < fileParoleRadici.Length; ++i)
                    {
                        if (fileParoleRadici[i].EndsWith(Path.DirectorySeparatorChar + linguaPreferita + ".parole_radici", StringComparison.OrdinalIgnoreCase))
                        {
                            (fileParoleRadici[0], fileParoleRadici[i]) = (fileParoleRadici[i], fileParoleRadici[0]);
                            trovato = true;
                            break;
                        }
                    }
                    if (trovato)
                    {
                        trovato = false;
                        for (int i = 1; i < fileParoleRadici.Length; ++i)
                        {
                            if (fileParoleRadici[i].EndsWith(Path.DirectorySeparatorChar + linguaPreferita + "1.parole_radici", StringComparison.OrdinalIgnoreCase))
                            {
                                (fileParoleRadici[1], fileParoleRadici[i]) = (fileParoleRadici[i], fileParoleRadici[1]);
                                trovato = true;
                                break;
                            }
                        }
                    }
                    if (trovato)
                    {
                        for (int i = 2; i < fileParoleRadici.Length; ++i)
                        {
                            if (fileParoleRadici[i].EndsWith(Path.DirectorySeparatorChar + linguaPreferita + "2.parole_radici", StringComparison.OrdinalIgnoreCase))
                            {
                                (fileParoleRadici[2], fileParoleRadici[i]) = (fileParoleRadici[i], fileParoleRadici[2]);
                                break;
                            }
                        }
                    }
                }
            }
            string[] paroleRadici = [];
            foreach (string fileConParoleRadici in fileParoleRadici)
            {
                string[] paroleRadiciInFile = File.ReadAllLines(fileConParoleRadici);
                Array.Resize(ref paroleRadici, paroleRadici.Length + paroleRadiciInFile.Length);
                Array.Copy(paroleRadiciInFile, 0, paroleRadici, paroleRadici.Length - paroleRadiciInFile.Length, paroleRadiciInFile.Length);
            }

            string[] radiceDiParola = new string[elencoParoleInVersione.Length];

            int indiceRadice, numeroParola;
            string[] parolaRadice;
            foreach (string s in paroleRadici)
            {
                try
                {
                    parolaRadice = s.Split('=');
                    numeroParola = Array.BinarySearch(elencoParoleInVersione, parolaRadice[0], confrontoParole);
                    // se parola è usata nel testo, e non già data una radice (perché multipli file possono dare radici diverse alla stessa parola)
                    //      dà la radice alla parola
                    if (numeroParola >= 0 && string.IsNullOrEmpty(radiceDiParola[numeroParola]))
                        radiceDiParola[numeroParola] = parolaRadice[1];
                }
                catch
                {
                    // problema con una riga in formato sbagliato (cioè senza =) - basta saltare la riga
                }
            }

            // creare la lista di tutte le radici utilizzate
            listaRadici.Add("*"); // quando una parola non ha radice
            bool esisteUnaParolaSenzaRadice = false;
            int numeroParole = radiceDiParola.Length;
            for (int i = 0; i < numeroParole; ++i)
            {
                if (String.IsNullOrEmpty(radiceDiParola[i]))
                    radiceDiParola[i] = "*";
                if (radiceDiParola[i] == "*")
                    esisteUnaParolaSenzaRadice = true;

                indiceRadice = listaRadici.BinarySearch(radiceDiParola[i], confrontoParole);
                if (indiceRadice < 0)
                    listaRadici.Insert(~indiceRadice, radiceDiParola[i]);
            }
            if (!esisteUnaParolaSenzaRadice)
                listaRadici.RemoveAt(0);

            return radiceDiParola;
        }

        internal static void AggiornaTestiNellInterfaccia()
        {
            // Find the TextGenerator and Search and Options and Concordance tools window via its ContentId and refresh its layout
            if (Application.Current.MainWindow is MainWindow mw)
            {
                if (mw.FindName("Dock") is AvalonDock.DockingManager dock)
                {
                    LayoutAnchorable? searchAnchorable = dock.Layout.Descendents()
                        .OfType<LayoutAnchorable>()
                        .FirstOrDefault(a => a.ContentId == "tool.search");

                    if (searchAnchorable?.Content is SearchToolView searchView)
                    {
                        searchView.AggiornaVersioniDisponibili();
                    }

                    LayoutAnchorable? textGenAnchorable = dock.Layout.Descendents()
                        .OfType<LayoutAnchorable>()
                        .FirstOrDefault(a => a.ContentId == "tool.textgen");

                    if (textGenAnchorable?.Content is TextGeneratorToolView textGenView)
                    {
                        textGenView.AggiornaVersioniDisponibili();
                    }

                    LayoutAnchorable? creaChiaveAnchorable = dock.Layout.Descendents()
                        .OfType<LayoutAnchorable>()
                        .FirstOrDefault(a => a.ContentId == "tool.creachiave");

                    if (creaChiaveAnchorable?.Content is CreaChiaveToolView creaChiaveView)
                    {
                        creaChiaveView.AggiornaVersioniDisponibili();
                    }

                    LayoutDocument? opzioniDocument = dock.Layout.Descendents()
                        .OfType<LayoutDocument>()
                        .FirstOrDefault(a => a.ContentId == "tool.options");

                    if (opzioniDocument?.Content is OptionsToolView opzioniView)
                    {
                        opzioniView.InitializeTextsPreferences();
                    }

                    LayoutDocument? bibliotecaDocument = dock.Layout.Descendents()
                        .OfType<LayoutDocument>()
                        .FirstOrDefault(a => a.ContentId == "tool.library");

                    if (bibliotecaDocument?.Content is LibraryToolView bibliotecaView)
                    {
                        bibliotecaView.LoadLibraryData();
                    }


                    LayoutDocument? aggiungiTestiDocument = dock.Layout.Descendents()
                        .OfType<LayoutDocument>()
                        .FirstOrDefault(a => a.ContentId == "tool.aggiungitesti");

                    if (aggiungiTestiDocument?.Content is AggiungiTesti aggiungiTestiView)
                    {
                        aggiungiTestiView.PopolaGrigia();
                        aggiungiTestiView.AggiornaConteggioStato();
                    }

                }

                mw.AggiornaMenuVisualizza();
            }

        }
    }
}
