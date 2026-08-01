using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace LaParola.Services
{
    internal sealed class ParametriEstrazionePdf
    {
        public string PercorsoPdf = "";
        public string Lingua = "";
        public string CartellaNote = "";
        public string CartellaLibro = "";
        public bool SaltaNote;
        public bool SaltaLibro;
        public int? PaginaFine;
        public int OgniQuantePagineOrdine = 200;
    }

    internal sealed class RisultatoEstrazionePdf
    {
        public int PagineTotali;
        public int IntestazioniTrovate;
        public int NoteScritte;
    }

    internal static class EstrazionePdf
    {
        private const char INIZIO_LINK = (char)2;
        private const char FINE_LINK1 = (char)3;
        private const char FINE_LINK2 = (char)4;
        private const char FINE_LINK_BRANO = (char)5;

        private static readonly Encoding Cp1252Severo = Encoding.GetEncoding(1252, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
        internal static readonly Encoding Cp1252Tollerante = Encoding.GetEncoding(1252, new EncoderReplacementFallback("?"), new DecoderReplacementFallback("?"));
        private static readonly Encoding Utf8SenzaBom = new UTF8Encoding(false);

        private static readonly (int numero, string nome, string[] abbreviazioni)[] Libri =
        [ // TODO2 usare Testi
            (1, "Genesi", new[] { "gen", "genesi", "ge", "gn" }),
            (2, "Esodo", new[] { "eso", "esodo", "eo", "es" }),
            (3, "Levitico", new[] { "lev", "le", "levitico", "lv" }),
            (4, "Numeri", new[] { "num", "numeri", "nu", "nm" }),
            (5, "Deuteronomio", new[] { "deut", "deuteronomio", "de", "dt" }),
            (6, "Giosuè", new[] { "gios", "giosue", "giosuè", "gs" }),
            (7, "Giudici", new[] { "giud", "giudici", "giudic", "gc" }),
            (8, "Rut", new[] { "rut", "ru", "rt" }),
            (9, "1 Samuele", new[] { "1sam", "1samuele", "1s" }),
            (10, "2 Samuele", new[] { "2sam", "2samuele", "2s" }),
            (11, "1 Re", new[] { "1re", "1r" }),
            (12, "2 Re", new[] { "2re", "2r" }),
            (13, "1 Cronache", new[] { "1cron", "1cronache", "1cr" }),
            (14, "2 Cronache", new[] { "2cron", "2cronache", "2cr" }),
            (15, "Esdra", new[] { "esd", "esdra", "ed" }),
            (16, "Neemia", new[] { "nee", "neemia", "ne" }),
            (19, "Ester", new[] { "est", "ester", "et" }),
            (22, "Giobbe", new[] { "giob", "giobbe", "gb" }),
            (23, "Salmi", new[] { "sal", "salmi", "salmo", "sl" }),
            (24, "Proverbi", new[] { "prov", "proverbi", "p" }),
            (25, "Ecclesiaste", new[] { "eccl", "ecclesiaste", "ec", "q" }),
            (26, "Cantico dei Cantici", new[] { "cant", "cantico", "cc", "ca", "ct" }),
            (29, "Isaia", new[] { "isa", "isaia", "is" }),
            (30, "Geremia", new[] { "ger", "geremia", "gr" }),
            (31, "Lamentazioni", new[] { "lam", "lamentazioni", "la" }),
            (33, "Ezechiele", new[] { "ezec", "ezechiele", "ez" }),
            (34, "Daniele", new[] { "dan", "daniele", "da", "dn" }),
            (35, "Osea", new[] { "os", "osea", "o" }),
            (36, "Gioele", new[] { "gioe", "gioele", "gl" }),
            (37, "Amos", new[] { "amo", "amos", "am" }),
            (38, "Abdia", new[] { "abd", "abdia", "ad" }),
            (39, "Giona", new[] { "gion", "giona" }),
            (40, "Michea", new[] { "mic", "michea", "mi" }),
            (41, "Naum", new[] { "nah", "naum", "na" }),
            (42, "Abacuc", new[] { "abac", "abacuc", "aba", "ac", "h" }),
            (43, "Sofonia", new[] { "sof", "sofonia", "so" }),
            (44, "Aggeo", new[] { "agg", "aggeo", "ag" }),
            (45, "Zaccaria", new[] { "zac", "zaccaria", "z" }),
            (46, "Malachia", new[] { "mal", "malachia", "ml" }),
            (47, "Matteo", new[] { "mat", "matteo", "mt" }),
            (48, "Marco", new[] { "mar", "marco", "mc", "mr" }),
            (49, "Luca", new[] { "luc", "luca", "lu", "lc" }),
            (50, "Giovanni", new[] { "giov", "giovanni", "gv" }),
            (51, "Atti", new[] { "att", "atti", "at" }),
            (52, "Romani", new[] { "rom", "romani", "ro", "rm" }),
            (53, "1 Corinzi", new[] { "1cor", "1corinzi", "1co" }),
            (54, "2 Corinzi", new[] { "2cor", "2corinzi", "2co" }),
            (55, "Galati", new[] { "gal", "galati", "ga" }),
            (56, "Efesini", new[] { "ef", "efesini" }),
            (57, "Filippesi", new[] { "fil", "filippesi", "fili", "fl" }),
            (58, "Colossesi", new[] { "col", "colossesi", "cl", "co" }),
            (59, "1 Tessalonicesi", new[] { "1tes", "1tessalonicesi", "1te", "1ts" }),
            (60, "2 Tessalonicesi", new[] { "2tes", "2tessalonicesi", "2te", "2ts" }),
            (61, "1 Timoteo", new[] { "1tim", "1timoteo", "1ti", "1tm" }),
            (62, "2 Timoteo", new[] { "2tim", "2timoteo", "2ti", "2tm" }),
            (63, "Tito", new[] { "tit", "tito", "ti", "tt" }),
            (64, "Filemone", new[] { "filem", "filemone", "file", "fm" }),
            (65, "Ebrei", new[] { "ebr", "ebrei", "eb" }),
            (66, "Giacomo", new[] { "giac", "giacomo", "gia", "gm" }),
            (67, "1 Pietro", new[] { "1piet", "1pietro", "1p" }),
            (68, "2 Pietro", new[] { "2piet", "2pietro", "2p" }),
            (69, "1 Giovanni", new[] { "1gv", "1giovanni", "1g" }),
            (70, "2 Giovanni", new[] { "2gv", "2giovanni", "2g" }),
            (71, "3 Giovanni", new[] { "3gv", "3giovanni", "3g" }),
            (72, "Giuda", new[] { "giuda", "gd" }),
            (73, "Apocalisse", new[] { "apoc", "apocalisse", "ap" }),
        ];

        internal static readonly Dictionary<string, int> AbbrevALibro = CostruisciAbbrevALibro();
        private static readonly HashSet<int> LibriUnCapitolo = [38, 64, 70, 71, 72];

        private static readonly Regex RigaHeader = new(@"^([1-3]?\s?[A-Za-zàèéìòùÀÈÉÌÒÙ]+)\.?\s+(\d{1,3}):(\d{1,3})(?:[-,](\d{1,3}))?$", RegexOptions.Compiled);
        private static readonly Regex RigaHeaderUnCapitolo = new(@"^([1-3]?\s?[A-Za-zàèéìòùÀÈÉÌÒÙ]+)\.?\s+(\d{1,3})(?:[-,](\d{1,3}))?$", RegexOptions.Compiled);
        internal static readonly Regex CitazioneInline = new(@"\b([1-3]?\s?[A-ZÀ-Ù][a-zàèéìòù]+)\.?\s+(\d{1,3}):(\d{1,3})(?:[-,](\d{1,3}))?", RegexOptions.Compiled);

        private static Dictionary<string, int> CostruisciAbbrevALibro()
        {
            Dictionary<string, int> dict = [];
            foreach (var (numero, _nome, abbreviazioni) in Libri)
                foreach (string a in abbreviazioni)
                    dict[a] = numero;
            return dict;
        }

        internal static string NormalizzaToken(string tok) => tok.ToLowerInvariant().Replace(" ", "").Replace(".", "");

        internal static (int numero, int capitolo, int v1, int v2)? RilevaIntestazione(string rigaPulita)
        {
            Match m = RigaHeader.Match(rigaPulita);
            if (m.Success)
            {
                string token = NormalizzaToken(m.Groups[1].Value);
                if (AbbrevALibro.TryGetValue(token, out int numero))
                {
                    int capitolo = int.Parse(m.Groups[2].Value, CultureInfo.InvariantCulture);
                    int v1 = int.Parse(m.Groups[3].Value, CultureInfo.InvariantCulture);
                    int v2 = m.Groups[4].Success ? int.Parse(m.Groups[4].Value, CultureInfo.InvariantCulture) : v1;
                    return (numero, capitolo, v1, v2);
                }
                return null;
            }
            Match m2 = RigaHeaderUnCapitolo.Match(rigaPulita);
            if (m2.Success)
            {
                string token = NormalizzaToken(m2.Groups[1].Value);
                if (AbbrevALibro.TryGetValue(token, out int numero) && LibriUnCapitolo.Contains(numero))
                {
                    int v1 = int.Parse(m2.Groups[2].Value, CultureInfo.InvariantCulture);
                    int v2 = m2.Groups[3].Success ? int.Parse(m2.Groups[3].Value, CultureInfo.InvariantCulture) : v1;
                    return (numero, 1, v1, v2);
                }
            }
            return null;
        }

        // TODO2 da cancellare sostituito con Riferimento.ComeNotaTuttoRiferimento()
        /*
        internal static string NomeFileNota(int numeroLibro, int capitolo, int v1, int v2)
        {
            return $"#{numeroLibro:D2}{capitolo:D3}{v1:D3}0000-{numeroLibro:D2}{capitolo:D3}{v2:D3}0000.rtf";
        }

        internal static string RiferimentoAStringa(int numeroLibro, int capitolo, int v1, int v2)
        {
            return $"#{numeroLibro:D2}{capitolo:D3}{v1:D3}0000-{numeroLibro:D2}{capitolo:D3}{v2:D3}0000";
        }
        */

        private static string EscapaRtf(string testo)
        {
            StringBuilder sb = new(testo.Length + 16);
            foreach (char ch in testo)
            {
                if (ch == '\\')
                    sb.Append("\\\\");
                else if (ch == '{')
                    sb.Append("\\{");
                else if (ch == '}')
                    sb.Append("\\}");
                else if (ch < 128)
                    sb.Append(ch);
                else
                {
                    try
                    {
                        byte[] b = Cp1252Severo.GetBytes([ch]);
                        sb.Append("\\'").Append(b[0].ToString("x2", CultureInfo.InvariantCulture));
                    }
                    catch (EncoderFallbackException)
                    {
                        int valore = ch < 32768 ? ch : ch - 65536;
                        sb.Append("\\u").Append(valore.ToString(CultureInfo.InvariantCulture)).Append('?');
                    }
                }
            }
            return sb.ToString();
        }

        internal static string CollegaCitazioniEEscape(string riga)
        {
            StringBuilder sb = new();
            int ultimaFine = 0;
            foreach (Match m in CitazioneInline.Matches(riga))
            {
                string token = NormalizzaToken(m.Groups[1].Value);
                if (!AbbrevALibro.TryGetValue(token, out int numeroLibro))
                    continue;
                int capitolo = int.Parse(m.Groups[2].Value, CultureInfo.InvariantCulture);
                int v1 = int.Parse(m.Groups[3].Value, CultureInfo.InvariantCulture);
                int v2 = m.Groups[4].Success ? int.Parse(m.Groups[4].Value, CultureInfo.InvariantCulture) : v1;
                // TODO2 da cancellare string riferimento = RiferimentoAStringa(numeroLibro, capitolo, v1, v2);
                string riferimento = new Riferimento([(byte)numeroLibro, (byte)capitolo, (byte)v1, (byte)numeroLibro, (byte)capitolo, (byte)v2]).ComeNotaTuttoRiferimento();
                sb.Append(EscapaRtf(riga[ultimaFine..m.Index]));
                sb.Append("\\v \\'0").Append((int)INIZIO_LINK).Append("\\v0 ");
                sb.Append(EscapaRtf(m.Value));
                sb.Append("\\v \\'0").Append((int)FINE_LINK1).Append(" \\'0").Append((int)FINE_LINK_BRANO).Append(riferimento).Append(" \\'0").Append((int)FINE_LINK2).Append("\\v0 ");
                ultimaFine = m.Index + m.Length;
            }
            sb.Append(EscapaRtf(riga[ultimaFine..]));
            return sb.ToString();
        }

        private static List<string> RaggruppaInParagrafi(IEnumerable<string> righe)
        {
            List<string> paragrafi = [];
            List<string> corrente = [];
            foreach (string riga in righe)
            {
                if (riga.Length == 0)
                {
                    if (corrente.Count > 0)
                    {
                        paragrafi.Add(string.Join(" ", corrente));
                        corrente.Clear();
                    }
                }
                else
                {
                    corrente.Add(riga);
                }
            }
            if (corrente.Count > 0)
                paragrafi.Add(string.Join(" ", corrente));
            return paragrafi;
        }

        private static string DocumentoRtf(string corpo) => "{\\rtf1\\ansi\\ansicpg1252\\deff0\n" + corpo + "\n}";

        private static string PaginaARtf(string testoPagina)
        {
            string[] righe = testoPagina.Trim().Split(["\r\n", "\r", "\n"], StringSplitOptions.None);
            List<string> paragrafi = RaggruppaInParagrafi(righe);
            string corpo = string.Join("\\par\n", paragrafi.Select(CollegaCitazioniEEscape));
            return DocumentoRtf(corpo);
        }

        private static void ScriviNotaRtf(string cartellaNote, string nomeFile, string corpoRtf, int numeroPagina, Action<string> log)
        {
            string percorso = Path.Combine(cartellaNote, nomeFile);
            string testoCompleto = DocumentoRtf(corpoRtf);
            try
            {
                Cp1252Severo.GetBytes(testoCompleto);
            }
            catch (EncoderFallbackException)
            {
                log($"    ATTENZIONE [pagina {numeroPagina}] '{nomeFile}': carattere non codificabile in cp1252 dopo escapa_rtf.");
            }
            try
            {
                if (File.Exists(percorso))
                {
                    string precedente = File.ReadAllText(percorso, Cp1252Tollerante).TrimEnd();
                    if (precedente.EndsWith('}'))
                        precedente = precedente[..^1];
                    else
                        log($"    ATTENZIONE [pagina {numeroPagina}] '{nomeFile}': il file esistente non terminava con '}}'.");
                    File.WriteAllText(percorso, precedente + "\\par\\par\n" + corpoRtf + "\n}", Cp1252Tollerante);
                }
                else
                {
                    File.WriteAllText(percorso, testoCompleto, Cp1252Tollerante);
                }
            }
            catch (Exception errore)
            {
                log($"    ATTENZIONE [pagina {numeroPagina}] '{nomeFile}': errore scrivendo il file ({errore.Message}).");
            }
        }

        internal static void AggiornaOrdineLibri(string cartellaLibri)
        {
            List<string> titoliInOrdine = [.. Directory.GetFiles(cartellaLibri, "*.rtf")
                .Select(static f => Path.GetFileNameWithoutExtension(f))
                .Where(static n => !n.StartsWith('#'))
                .OrderBy(static s => s, StringComparer.Ordinal)];
            if (titoliInOrdine.Count == 0)
                return;
            string nomeCartella = new DirectoryInfo(cartellaLibri).Name;
            if (nomeCartella.StartsWith("LaParola", StringComparison.InvariantCultureIgnoreCase))
                nomeCartella = nomeCartella[8..];
            string percorsoOrdine = Path.Combine(cartellaLibri, nomeCartella + ".ordine");
            File.WriteAllText(percorsoOrdine, "\n" + string.Join("\n", titoliInOrdine), Utf8SenzaBom);
        }

        private static string TestoPagina(Page pagina)
        {
            List<Word> parole = [.. pagina.GetWords()];
            if (parole.Count == 0)
                return "";

            List<Word> paroleOrdinate = [.. parole.OrderByDescending(w => w.BoundingBox.Bottom)];
            List<List<Word>> righe = [];
            const double sogliaVerticale = 3.0;

            foreach (Word parola in paroleOrdinate)
            {
                List<Word>? rigaCorrente = righe.Count > 0 ? righe[^1] : null;
                if (rigaCorrente != null && Math.Abs(rigaCorrente[0].BoundingBox.Bottom - parola.BoundingBox.Bottom) <= sogliaVerticale)
                    rigaCorrente.Add(parola);
                else
                    righe.Add([parola]);
            }

            double altezzaMediana = righe.Count > 0 ? righe.Average(r => r[0].BoundingBox.Height) : 10.0;
            List<string> righeTesto = [];
            double? bottomPrecedente = null;
            foreach (List<Word> riga in righe)
            {
                double bottomAttuale = riga[0].BoundingBox.Bottom;
                if (bottomPrecedente.HasValue)
                {
                    double scarto = bottomPrecedente.Value - bottomAttuale;
                    if (scarto > altezzaMediana * 1.8)
                        righeTesto.Add("");
                }
                string testoRiga = string.Join(" ", riga.OrderBy(w => w.BoundingBox.Left).Select(w => w.Text));
                righeTesto.Add(testoRiga);
                bottomPrecedente = bottomAttuale;
            }
            return string.Join("\n", righeTesto);
        }

        // TODO2 da cancellare non usato da nessuno codice
        /*
        private sealed class ParolaStile
        {
            public string Testo = "";
            public bool Grassetto;
            public bool Corsivo;
        }

        private static (bool grassetto, bool corsivo) StileDaFont(string nomeFont)
        {
            if (string.IsNullOrEmpty(nomeFont))
                return (false, false);
            bool grassetto = nomeFont.Contains("bold", StringComparison.OrdinalIgnoreCase)
                || nomeFont.Contains("black", StringComparison.OrdinalIgnoreCase)
                || nomeFont.Contains("heavy", StringComparison.OrdinalIgnoreCase);
            bool corsivo = nomeFont.Contains("italic", StringComparison.OrdinalIgnoreCase)
                || nomeFont.Contains("oblique", StringComparison.OrdinalIgnoreCase);
            return (grassetto, corsivo);
        }

        private static string ParagrafoStileARtf(List<ParolaStile> paragrafo)
        {
            if (paragrafo.Count == 0) return "";
            var runs = new List<(bool g, bool c, string t)>();
            StringBuilder cur = new();
            bool gCorr = paragrafo[0].Grassetto, cCorr = paragrafo[0].Corsivo;
            for (int i = 0; i < paragrafo.Count; i++)
            {
                ParolaStile p = paragrafo[i];
                if (p.Grassetto != gCorr || p.Corsivo != cCorr)
                {
                    runs.Add((gCorr, cCorr, cur.ToString()));
                    cur.Clear(); gCorr = p.Grassetto; cCorr = p.Corsivo;
                }
                if (cur.Length > 0) cur.Append(' ');
                cur.Append(p.Testo);
            }
            runs.Add((gCorr, cCorr, cur.ToString()));
            StringBuilder sb = new();
            for (int i = 0; i < runs.Count; i++)
            {
                var (g, c, t) = runs[i];
                string corpo = CollegaCitazioniEEscape(i < runs.Count - 1 ? t + " " : t);
                if (!g && !c) { sb.Append(corpo); continue; }
                sb.Append('{');
                if (g) sb.Append(@"\b ");
                if (c) sb.Append(@"\i ");
                sb.Append(corpo);
                sb.Append('}');
            }
            return sb.ToString();
        }

        private static List<List<ParolaStile>> RaggruppaInParagrafiStile(IEnumerable<List<ParolaStile>> righe)
        {
            List<List<ParolaStile>> paragrafi = [];
            List<ParolaStile> corrente = [];
            foreach (List<ParolaStile> riga in righe)
            {
                if (riga.Count == 0) { if (corrente.Count > 0) { paragrafi.Add(corrente); corrente = []; } }
                else { corrente.AddRange(riga); }
            }
            if (corrente.Count > 0) paragrafi.Add(corrente);
            return paragrafi;
        }

        private static List<List<ParolaStile>> RigheStilizzate(Page pagina)
        {
            List<Word> parole = [.. pagina.GetWords()];
            if (parole.Count == 0) return [];
            List<Word> ordinate = [.. parole.OrderByDescending(w => w.BoundingBox.Bottom)];
            List<List<Word>> righe = [];
            const double soglia = 3.0;
            foreach (Word w in ordinate)
            {
                List<Word>? ultima = righe.Count > 0 ? righe[^1] : null;
                if (ultima != null && Math.Abs(ultima[0].BoundingBox.Bottom - w.BoundingBox.Bottom) <= soglia)
                    ultima.Add(w);
                else
                    righe.Add([w]);
            }
            double altezza = righe.Count > 0 ? righe.Average(r => r[0].BoundingBox.Height) : 10.0;
            List<List<ParolaStile>> risultato = [];
            double? prec = null;
            foreach (List<Word> riga in righe)
            {
                double cur = riga[0].BoundingBox.Bottom;
                if (prec.HasValue && prec.Value - cur > altezza * 1.8)
                    risultato.Add([]);
                List<ParolaStile> rs = [];
                foreach (Word w in riga.OrderBy(w => w.BoundingBox.Left))
                {
                    string? fn = w.Letters.Count > 0 ? w.Letters[0].FontName : null;
                    if (fn != null)
                    {
                        var (g, c) = StileDaFont(fn);
                        rs.Add(new ParolaStile { Testo = w.Text, Grassetto = g, Corsivo = c });
                    }
                }
                risultato.Add(rs);
                prec = cur;
            }
            return risultato;
        }
        */

        internal static RisultatoEstrazionePdf Estrai(ParametriEstrazionePdf p, Action<string> log, CancellationToken token, Action<int, int>? progresso = null)
        {
            if (p.SaltaNote && p.SaltaLibro)
                throw new InvalidOperationException("Non puoi saltare sia Note sia Libro.");

            if (!File.Exists(p.PercorsoPdf))
                throw new FileNotFoundException("PDF non trovato: " + p.PercorsoPdf, p.PercorsoPdf);

            if (!p.SaltaNote)
                Directory.CreateDirectory(p.CartellaNote);
            if (!p.SaltaLibro)
                Directory.CreateDirectory(p.CartellaLibro);

            if (!p.SaltaNote)
            {
                string[] noteTxtVecchie = Directory.GetFiles(p.CartellaNote, "#*.txt");
                if (noteTxtVecchie.Length > 0)
                {
                    log($"Trovate {noteTxtVecchie.Length} note .txt precedenti in 'Note': le cancello.");
                    foreach (string f in noteTxtVecchie)
                        File.Delete(f);
                }
            }

            log($"Apro il PDF: {Path.GetFileName(p.PercorsoPdf)}");
            using PdfDocument documento = PdfDocument.Open(p.PercorsoPdf);
            int numeroPagine = documento.NumberOfPages;
            int paginaFinale = p.PaginaFine ?? numeroPagine;
            log($"Pagine totali: {numeroPagine} (elaboro fino alla pagina {paginaFinale})");
            string parolaPagina = "Pagina";
            if (p.Lingua.Length > 1 && p.Lingua.ToLower()[..2] == "en")
                parolaPagina = "Page";
            else if (p.Lingua.Length <= 1 || (p.Lingua.Length > 1 && p.Lingua.ToLower()[..2] != "it"))
            {
                if (MainWindow.settings.Lingua == "en")
                    parolaPagina = "Page";
            }

            int paginaIniziale = 1;
            int cifre = paginaFinale > 0 ? paginaFinale.ToString().Length : 1;
            string formato = $"D{cifre}";
            if (!p.SaltaLibro)
            {
                while (paginaIniziale <= paginaFinale && File.Exists(Path.Combine(p.CartellaLibro, $"{parolaPagina}{paginaIniziale.ToString(formato)}.rtf")))
                    paginaIniziale++;
                if (paginaIniziale > 1)
                {
                    log($"Trovate già elaborate le pagine 1-{paginaIniziale - 1} in 'Libri': riparto dalla pagina {paginaIniziale}.");
                    if (paginaIniziale > paginaFinale)
                    {
                        log("Tutte le pagine richieste sono già state elaborate.");
                        AggiornaOrdineLibri(p.CartellaLibro);
                        return new RisultatoEstrazionePdf { PagineTotali = numeroPagine, IntestazioniTrovate = 0, NoteScritte = 0 };
                    }
                }
            }

            string? nomeFileAttuale = null;
            List<string> bufferTesto = [];
            int noteScritte = 0;
            int intestazioniTrovate = 0;
            int paginaDelBloccoCorrente = paginaIniziale;
            int ultimoPercentoSegnalato = -1;

            if (progresso != null && paginaFinale > 0)
            {
                ultimoPercentoSegnalato = (paginaIniziale - 1) * 100 / paginaFinale;
                progresso(paginaIniziale - 1, paginaFinale);
            }

            void SalvaBloccoCorrente()
            {
                if (nomeFileAttuale != null)
                {
                    try
                    {
                        List<string> paragrafi = RaggruppaInParagrafi(bufferTesto);
                        string corpoRtf = string.Join("\\par\n", paragrafi.Where(pRiga => pRiga.Trim().Length > 0).Select(CollegaCitazioniEEscape));
                        if (corpoRtf.Trim().Length > 0)
                        {
                            ScriviNotaRtf(p.CartellaNote, nomeFileAttuale, corpoRtf, paginaDelBloccoCorrente, log);
                            noteScritte++;
                        }
                    }
                    catch (Exception errore)
                    {
                        log($"    ATTENZIONE [pagina {paginaDelBloccoCorrente}] '{nomeFileAttuale}': {errore.Message}");
                    }
                }
                bufferTesto.Clear();
            }

            for (int i = paginaIniziale - 1; i < paginaFinale; i++)
            {
                token.ThrowIfCancellationRequested();
                int numeroPagina = i + 1;
                Page pagina = documento.GetPage(numeroPagina);
                string testoPagina = TestoPagina(pagina) ?? "";

                if (testoPagina.Trim().Length < 5)
                    log($"    ATTENZIONE [pagina {numeroPagina}]: pagina vuota o quasi.");

                if (!p.SaltaLibro)
                {
                    string rtfPagina = PaginaARtf(testoPagina);
                    try
                    {
                        File.WriteAllText(Path.Combine(p.CartellaLibro, $"{parolaPagina}{numeroPagina.ToString(formato)}.rtf"), rtfPagina, Cp1252Tollerante);
                    }
                    catch (Exception errore)
                    {
                        log($"    ATTENZIONE [pagina {numeroPagina}]: errore scrivendo la pagina ({errore.Message}).");
                    }
                }

                if (!p.SaltaNote)
                {
                    foreach (string riga in testoPagina.Split(["\r\n", "\r", "\n"], StringSplitOptions.None))
                    {
                        string rigaPulita = riga.Trim();
                        if (rigaPulita.Length == 0)
                        {
                            bufferTesto.Add("");
                            continue;
                        }
                        var riferimento = RilevaIntestazione(rigaPulita);
                        if (riferimento.HasValue)
                        {
                            SalvaBloccoCorrente();
                            var (numeroLibro, capitolo, v1, v2) = riferimento.Value;
                            // TODO2 da cancellare nomeFileAttuale = NomeFileNota(numeroLibro, capitolo, v1, v2);
                            nomeFileAttuale = new Riferimento([(byte)numeroLibro, (byte)capitolo, (byte)v1, (byte)numeroLibro, (byte)capitolo, (byte)v2]).ComeNotaTuttoRiferimento() + ".rtf";
                            paginaDelBloccoCorrente = numeroPagina;
                            intestazioniTrovate++;
                            continue;
                        }
                        bufferTesto.Add(rigaPulita);
                    }
                }

                if (!p.SaltaLibro && numeroPagina % p.OgniQuantePagineOrdine == 0)
                    AggiornaOrdineLibri(p.CartellaLibro);

                if (numeroPagina % 500 == 0)
                    log($"  ...elaborate {numeroPagina}/{paginaFinale} pagine ({intestazioniTrovate} intestazioni, {noteScritte} note)");

                if (progresso != null)
                {
                    int percentoAttuale = paginaFinale > 0 ? numeroPagina * 100 / paginaFinale : 100;
                    if (percentoAttuale != ultimoPercentoSegnalato)
                    {
                        ultimoPercentoSegnalato = percentoAttuale;
                        progresso(numeroPagina, paginaFinale);
                    }
                }
            }

            if (!p.SaltaNote)
                SalvaBloccoCorrente();
            if (!p.SaltaLibro)
                AggiornaOrdineLibri(p.CartellaLibro);

            return new RisultatoEstrazionePdf
            {
                PagineTotali = numeroPagine,
                IntestazioniTrovate = intestazioniTrovate,
                NoteScritte = noteScritte
            };
        }
    }
}
