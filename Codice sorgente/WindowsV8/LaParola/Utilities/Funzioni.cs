using AvalonDock.Layout;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
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
                throw new ArgumentNullException("url");
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
                throw new ArgumentNullException("url");
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
    }
}
