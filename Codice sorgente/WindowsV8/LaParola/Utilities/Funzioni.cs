using System.Globalization;
using System.Text;

namespace LaParola.Utilities
{
    class Funzioni
    {
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

        /// <summary>
        /// Cancella il testo nascosto da una stringa in formato RTF.
        /// </summary>
        /// <param name="testoRtf">La stringa da cui cancellare il testo nascosto.</param>
        /// <returns>Una stringa senza il testo nascosto.</returns>
        public static string RimuoviTestoNascosto(string testoRtf)
        {
            // qualcosa di simile in RichTextBoxEx.cs::CopiaSenzaTestoNascosto
            while (testoRtf.IndexOf(@"\v\'01", StringComparison.Ordinal) > 0)
            {
                testoRtf = testoRtf.Remove(testoRtf.IndexOf(@"\v\'01", StringComparison.Ordinal), 14); // InizioRiferimento
            }
            while (testoRtf.IndexOf(@"\'01", StringComparison.Ordinal) > 0)
            {
                testoRtf = testoRtf.Remove(testoRtf.IndexOf(@"\'01", StringComparison.Ordinal), 12); // InizioRiferimento
            }
            while (testoRtf.IndexOf(@"\v\'02\v0 ", StringComparison.Ordinal) > 0) // InizioLink
            {
                testoRtf = testoRtf.Remove(testoRtf.IndexOf(@"\v\'02\v0 ", StringComparison.Ordinal), 10);
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
            }
            while (testoRtf.IndexOf(@"\v\'0e", StringComparison.Ordinal) > 0) // testo ricercato
            {
                int p = testoRtf.IndexOf(@"\v0", testoRtf.IndexOf(@"\v\'0e", StringComparison.Ordinal), StringComparison.Ordinal);
                if (p > 0)
                {
                    testoRtf = testoRtf.Remove(p, 3).Remove(testoRtf.IndexOf(@"\v\'0e", StringComparison.Ordinal), 6);
                }
            }
            return testoRtf.Replace(@"\v\", @"\").Replace(@"\'0e", "").Replace(@"\'02", "").Replace(@"\v0", "");
        }

        // TODO cancel
        /*
        public static string ConvertiUnicodeInRtf(string rtf)
        {
            // StripRtf in testi.cs fa parzialmente il contrario
            if (string.IsNullOrEmpty(rtf))
            {
                return "";
            }

            int numeroCaratteri = rtf.Length;
            StringBuilder rtfSB = new(numeroCaratteri * 7);
            for (int i = 0; i < numeroCaratteri; ++i)
            {
                if (rtf[i] >= 128)
                {
                    rtfSB.Append(@"\u").Append(((int)rtf[i]).ToString(CultureInfo.InvariantCulture)).Append('?');
                }
                else
                {
                    rtfSB.Append(rtf[i]);
                }
            }
            return rtfSB.ToString();
        }
        */
        /*
        public static string ConvertiUnicodeInRtf(string rtf)
        {
            if (string.IsNullOrEmpty(rtf))
                return string.Empty;

            ReadOnlySpan<char> span = rtf.AsSpan();

            StringBuilder sb = new(rtf.Length + (rtf.Length / 4));

            foreach (char c in span)
            {
                if (c >= 128)
                {
                    sb.Append(@"\u");
                    sb.Append((int)c);
                    sb.Append('?');
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }
        */
        public static string ConvertiUnicodeInRtf(string rtf)
        {
            if (string.IsNullOrEmpty(rtf))
                return string.Empty;

            StringBuilder sb = new(rtf.Length + 256);

            foreach (char c in rtf)
            {
                if (c < 128)
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append(@"\u");

                    Span<char> buffer = stackalloc char[6];
                    ((int)c).TryFormat(buffer, out int written);

                    sb.Append(buffer[..written]);

                    sb.Append('?');
                }
            }

            return sb.ToString();
        }

    }
}
