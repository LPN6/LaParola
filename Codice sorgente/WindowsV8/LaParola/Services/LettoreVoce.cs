using LaParola.DocumentViews;
using MahApps.Metro.IconPacks;
using System.Globalization;
using System.Speech.Synthesis;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

namespace LaParola.Services
{
    internal partial class LettoreVoce
    {
        private static VoiceInfo? vocePredefinita;
        private static readonly SpeechSynthesizer sintetizzatore = CreaSintetizzatore();
        private static readonly WeakReference<Button> pulsanteAttivoRef = new(null!);

        // Evidenziazione della parola in lettura (karaoke) - vedi la regione "Evidenziazione
        // della parola in lettura" più sotto per la spiegazione completa del meccanismo, oggi
        // basato sugli eventi reali SpeakProgress di System.Speech.Synthesis (non più su
        // marcatori/stime, vedi changelog 2026-07-18). Un solo insieme di campi statici basta: si
        // legge una sola voce alla volta (sintetizzatore e' un singleton condiviso), quindi non
        // serve tenerne uno per finestra come per pulsanteAttivoRef.
        private static IFlowDocumentHost? hostAttuale;
        private static DispatcherTimer? timerEvidenziazione;
        private static List<ParolaConPosizione>? paroleOriginaliAttuali;
        private static ParolaConPosizione[]? mappaturaParoleAttuale;
        private static string[]? paroleLetteAttuali;
        // Offset di inizio (in caratteri, dentro il testo inviato a SpeakAsync) di ciascuna
        // parola di paroleLetteAttuali - serve a far corrispondere l'evento SpeakProgress (che
        // riporta un CharacterPosition dentro TUTTO il testo, non un indice di parola) alla
        // parola giusta.
        private static int[]? offsetsParoleLette;
        private static int indiceParolaCorrenteEvidenziata = -1;
        // Istante (orologio reale) in cui e' iniziata la parola attualmente in evidenziazione,
        // secondo l'ultimo evento SpeakProgress ricevuto - usato solo per animare il riempimento
        // progressivo DENTRO la parola corrente (il "quale parola" e' invece deciso con certezza
        // dall'evento stesso, non da questo orologio).
        private static DateTime? istanteInizioParolaCorrente;
        // Millisecondi per carattere stimati, per prevedere quanto durera' la parola ANCORA IN
        // CORSO (non sappiamo la sua durata reale finche' non arriva il prossimo evento) - si
        // autocorregge a ogni parola in base all'ultima durata REALMENTE misurata (differenza fra
        // due eventi SpeakProgress consecutivi), quindi l'errore non si accumula mai sull'intera
        // lettura: al massimo sbaglia la stima di UNA parola, mai di piu'.
        private static double msPerCarattereStimati = 90.0;
        private static SolidColorBrush pennelloLetto = new(Color.FromRgb(44, 62, 80));
        private static string _linguaEspansione = "it";
        // Segnalato dall'utente: l'oro (#FFD700) si vedeva a malapena - sostituito col rosso.
        private static readonly SolidColorBrush pennelloParolaCorrente = new(Color.FromRgb(0xFF, 0x00, 0x00));

        // Punti di taglio (uno per ogni offset di carattere) della parola attualmente in
        // riempimento progressivo, precalcolati TUTTI INSIEME la prima volta che questa parola
        // viene raggiunta - MAI richiesti di nuovo con GetPositionAtOffset a ogni tick del timer.
        // Necessario perché ApplyPropertyValue spezza il Run sottostante in più parti: chiamare
        // TextPointer.GetPositionAtOffset di nuovo DOPO che la parola è già stata colorata anche
        // una sola volta restituisce offset progressivamente scorretti (verificato: il taglio
        // richiesto si accorcia via via che la parola viene ricolorata più volte in sequenza).
        private static TextPointer[]? puntiTaglioParolaCorrente;
        private static int indiceParolaConPuntiPrecalcolati = -1;

        private static SpeechSynthesizer CreaSintetizzatore()
        {
            SpeechSynthesizer s = new();
            s.SetOutputToDefaultAudioDevice();

            string linguaInterfaccia = MainWindow.settings.Lingua.ToLower()[..2];

            // Scegli la voce impostata nelle Opzioni
            // Difficilmente non sarà disponibile, ma in quel caso sceglie una voce nella lingua dell'interfaccia dell'app
            // Le voci installate dipendono dal sistema operativo dell'utente
            // (Impostazioni -> Ora e lingua -> Lingua e area geografica -> Voce)
            // LaParola non può installarne una da sola.
            InstalledVoice? voceImpostata = s.GetInstalledVoices()
                .FirstOrDefault(v => v.VoiceInfo.Name.Contains(MainWindow.settings.VoceSintesiVocale, StringComparison.OrdinalIgnoreCase));
            if (voceImpostata != null)
                s.SelectVoice(voceImpostata.VoiceInfo.Name);
            else
            {
                try
                {
                    bool vocePredefinitaGiaInterfaccia = s.Voice != null
                        && s.Voice.Culture.TwoLetterISOLanguageName.Equals(linguaInterfaccia, StringComparison.OrdinalIgnoreCase);
                    if (!vocePredefinitaGiaInterfaccia)
                    {
                        InstalledVoice? voceInterfaccia = s.GetInstalledVoices()
                            .FirstOrDefault(v => v.VoiceInfo.Culture.TwoLetterISOLanguageName.Equals(linguaInterfaccia, StringComparison.OrdinalIgnoreCase));
                        if (voceInterfaccia != null)
                            s.SelectVoice(voceInterfaccia.VoiceInfo.Name);
                    }
                    if (s.Voice != null)
                        MainWindow.settings.VoceSintesiVocale = s.Voice.Name;
                }
                catch (Exception /*exc*/)
                {
                    //Program.RegistraErrore(exc);
                }
            }

            // ricordare quella voce, per reimpostare ogni volta, perché può essere cambiata da ImpostaLingua()
            vocePredefinita = s.Voice;

            // Eventi reali per-parola (System.Speech.Synthesis/SAPI5) - a differenza dei
            // marcatori di Windows.Media.SpeechSynthesis (verificato: sempre vuoti con le voci
            // OneCore installate su questo PC), questi funzionano davvero con le stesse voci,
            // perché le raggiungono tramite il livello SAPI5 classico invece che quello WinRT.
            s.SpeakProgress += OnSpeakProgress;
            s.SpeakCompleted += OnSpeakCompleted;

            return s;
        }

        private static void ImpostaLingua(string lingua)
        {
            if (string.IsNullOrEmpty(lingua))
                lingua = MainWindow.settings.Lingua.ToLower()[..2];
            if (vocePredefinita != null)
                sintetizzatore.SelectVoice(vocePredefinita.Name);

            try
            {
                bool voceAttualeInLingua = sintetizzatore.Voice != null
                    && sintetizzatore.Voice.Culture.TwoLetterISOLanguageName.Equals(lingua, StringComparison.OrdinalIgnoreCase);
                if (!voceAttualeInLingua)
                {
                    InstalledVoice? voceInLingua = sintetizzatore.GetInstalledVoices()
                        .FirstOrDefault(v => v.VoiceInfo.Culture.TwoLetterISOLanguageName.Equals(lingua, StringComparison.OrdinalIgnoreCase));
                    voceInLingua ??= sintetizzatore.GetInstalledVoices()
                        .FirstOrDefault(v => v != null);
                    if (voceInLingua != null)
                        sintetizzatore.SelectVoice(voceInLingua.VoiceInfo.Name);
                }
            }
            catch (Exception /*exc*/)
            {
                //Program.RegistraErrore(exc);
            }
        }

        /// <summary>
        /// Da richiamare dal Click del pulsante "Leggi"/"Ferma" di ciascuna finestra. Se si sta
        /// gia' leggendo (da questa o da un'altra finestra), ferma; altrimenti ottiene il
        /// FlowDocument tramite <paramref name="ottieniDocumento"/> e avvia la lettura,
        /// evidenziando via via la parola pronunciata (vedi la regione "Evidenziazione della
        /// parola in lettura" piu' sotto per il meccanismo completo).
        /// </summary>
        /// <param name="host">Host del FlowDocument (Viewer o Editor) usato per applicare la
        /// colorazione dell'evidenziazione senza sporcare lo stato "non salvato" negli editor
        /// editabili - vedi IFlowDocumentHost.EseguiSenzaSporcareDocumento.</param>
        /// <param name="testoBiblico">Richiesto dall'utente (13/07/2026): quando il testo letto è
        /// il testo biblico vero e proprio (Visualizza/Lettura/BraniParalleli - ogni versetto
        /// preceduto dal proprio riferimento completo, es. "Marco 16:16 Chi avra' creduto...")
        /// il riferimento va tolto del tutto prima di leggere, non letto (una persona che legge
        /// la Bibbia ad alta voce non pronuncia il numero del versetto). Per tutte le altre
        /// finestre (note, quiz, info versione) il parametro resta false: li' un riferimento puo'
        /// comparire dentro una frase con un senso proprio e va comunque letto (espanso invece
        /// di eliminato, come già faceva EspandiRiferimentiVersetti).</param>
        internal static void ToggleLettura(Button pulsante, IFlowDocumentHost host, Func<FlowDocument> ottieniDocumento, string lingua = "", bool testoBiblico = false)
        {
            // Se questo pulsante è già quello "attivo" (mostra "Ferma"), il click è uno stop
            // vero e proprio - anche se la sintesi non ha ancora iniziato a riprodurre nulla
            // (evita corse fra un secondo click rapido e la sintesi ancora in corso in background).
            if (pulsanteAttivoRef.TryGetTarget(out Button? pulsanteAttivo) && pulsanteAttivo == pulsante)
            {
                Ferma();
                return;
            }

            Ferma();

            FlowDocument? documento;
            try
            {
                documento = ottieniDocumento();
            }
            catch { return; }

            if (documento == null)
                return;

            List<ParolaConPosizione> paroleOriginali = EstraiParoleConPosizione(documento);
            if (paroleOriginali.Count == 0)
                return;

            // I numeri di versetto (solo per testoBiblico) e i marcatori di struttura di
            // un'esposizione a punti (es. "I.", "(1.)", sempre) sono parole a se stanti che non
            // vanno pronunciate - escluderle qui, prima di unire le parole in una stringa, le
            // tiene comunque nella mappatura come parole originali "saltate" (vedi AllineaParole).
            IEnumerable<ParolaConPosizione> paroleDaPronunciare = paroleOriginali.Where(p =>
                !(testoBiblico && EstaNumeroDiVersetto(p)) && !EstaMarcatoreElencoAInizioParagrafo(p));
            string testo = string.Join(" ", paroleDaPronunciare.Select(p => p.Testo));

            if (!string.IsNullOrEmpty(lingua) && lingua.Length >= 2)
                _linguaEspansione = lingua[..2].ToLowerInvariant();

            /*
            try
            {
                System.IO.File.AppendAllText(@"C:\Users\amministratore\AppData\Local\Temp\tts_debug.txt",
                    $"\n=== TTS [{DateTime.Now:HH:mm:ss}] biblico={testoBiblico} ===\n" +
                    $"PRIMA({testo.Length}c): {{{testo.Substring(0, Math.Min(200, testo.Length))}}}\n");
            }
            catch { }*/

            if (testoBiblico)
            {
                testo = RimuoviRiferimentiVersettoBibbia(testo);
            }
            else
            {
                testo = EspandiAbbreviazioniLibri(testo);
                testo = EspandiNumeriLibriCompleti(testo);
                testo = EspandiRiferimentiVersetti(testo);
            }

            /*
            try
            {
                System.IO.File.AppendAllText(@"C:\Users\amministratore\AppData\Local\Temp\tts_debug.txt",
                    $"DOPO({testo.Length}c): {{{testo.Substring(0, Math.Min(200, testo.Length))}}}\n\n");
            }
            catch { }*/

            if (string.IsNullOrWhiteSpace(testo))
                return;

            // Le parole EFFETTIVAMENTE pronunciate (dopo le trasformazioni sopra) non
            // corrispondono più 1:1 alle parole originali del documento (riferimenti tolti,
            // abbreviazioni espanse in più parole, numeri convertiti in ordinali) -
            // AllineaParole fa combaciare le due sequenze con un allineamento per sottosequenza
            // comune più lunga (LCS), così l'evidenziazione resta ancorata alla parola giusta.
            string[] paroleLette = testo.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
            ParolaConPosizione[] mappatura = AllineaParole(paroleOriginali, paroleLette);

            hostAttuale = host;
            paroleOriginaliAttuali = paroleOriginali;
            mappaturaParoleAttuale = mappatura;
            paroleLetteAttuali = paroleLette;

            pulsanteAttivoRef.SetTarget(pulsante);
            if (pulsante.Content is PackIconMaterial icona)
            {
                // L'utente sta ascoltando -> Mostriamo l'icona per fermare
                icona.Kind = PackIconMaterialKind.AccountVoiceOff;

                // Aggiorna il Tooltip con una risorsa dinamica di stop
                pulsante.ToolTip = (string)(Application.Current.TryFindResource("ViewerVoceAiutoFerma") ?? "Stop the speech synthesis");
            }

            if (MainWindow.settings.VoceDelTesto)
                ImpostaLingua(lingua);
            AvviaLettura(pulsante, testo);
        }

        private static Regex? regexAbbreviazioniLibri;
        private static bool regexAbbreviazioniLibriTentataCreazione;

        /// <summary>
        /// Richiesto dall'utente: le abbreviazioni dei libri biblici (es. "Gv", "Sal", "1Cor")
        /// venivano lette lettera per lettera o come parole senza senso invece che come nome
        /// completo ("i riferimenti biblici vengono letti male dalla voce"). Sostituisce ogni
        /// abbreviazione riconosciuta - purché seguita (dopo eventuale punto e spazi) da una
        /// cifra, il segnale che si tratti davvero di un riferimento a capitolo/versetto e non di
        /// una parola qualunque che assomiglia a un'abbreviazione (alcune abbreviazioni sono di
        /// una o due lettere, es. "o" per Osea, "la"/"mi"/"ti" per altri libri, che altrimenti
        /// sarebbero normalissime parole italiane) - con il nome completo del libro
        /// (Principale.testi.GetLibroNumeroDaAbbreviazione + GetLibroNome, gia' usati altrove nel
        /// programma per lo stesso scopo, solo in direzione inversa).
        /// </summary>
        private static string EspandiAbbreviazioniLibri(string testo)
        {
            Regex? regex = OttieniRegexAbbreviazioniLibri();
            if (regex == null)
                return testo;
            try
            {
                return regex.Replace(testo, m =>
                {
                    byte numeroLibro = MainWindow.Testi.GetLibroNumeroDaAbbreviazione(m.Groups[1].Value);
                    if (numeroLibro < 1 || numeroLibro > 73)
                        return m.Value;
                    string nomeCompleto = MainWindow.Testi.GetLibroNome(numeroLibro);
                    if (string.IsNullOrEmpty(nomeCompleto))
                        return m.Value;
                    return ConvertiNumeroLibroInOrdinale(nomeCompleto) + " ";
                });
            }
            catch { return testo; }
        }

        private static string ConvertiNumeroLibroInOrdinale(string nomeLibro)
        {
            Match m = RegexNumeroLibroInOrdinale().Match(nomeLibro);
            if (!m.Success) return nomeLibro;

            string num = m.Groups[1].Value;
            string resto = m.Groups[2].Value;
            string ordinale;

            if (_linguaEspansione == "en")
                ordinale = num switch { "1" => "First", "2" => "Second", "3" => "Third", _ => "" };
            else if (_linguaEspansione == "es")
                ordinale = num switch { "1" => "Primera", "2" => "Segunda", "3" => "Tercera", _ => "" };
            else
                ordinale = num switch { "1" => "Prima", "2" => "Seconda", "3" => "Terza", _ => "" };

            return string.IsNullOrEmpty(ordinale) ? nomeLibro : ordinale + " " + resto;
        }

        private static string EspandiRiferimentiVersetti(string testo)
        {
            try
            {
                string cap = "capitolo", vers = "versetto", dal = "dal versetto", al = "al versetto", conn = " e ";
                if (_linguaEspansione == "en")
                { cap = "chapter"; vers = "verse"; dal = "from verse"; al = "to verse"; conn = " and "; }
                else if (_linguaEspansione == "es")
                { cap = "capítulo"; vers = "versículo"; dal = "del versículo"; al = "al versículo"; conn = " y "; }

                testo = RegexEspandiRiferimenti1().Replace(testo, m =>
                    $"{cap} {m.Groups[1].Value} {dal} {m.Groups[2].Value} {al} {m.Groups[3].Value}");

                testo = RegexEspandiRiferimenti2().Replace(testo, m =>
                {
                    string[] versetti = RegexEspandiRiferimenti3().Split(m.Groups[2].Value);
                    string elenco = versetti.Length == 1
                        ? versetti[0]
                        : string.Join(", ", versetti, 0, versetti.Length - 1) + conn + versetti[^1];
                    return $"{cap} {m.Groups[1].Value} {vers} {elenco}";
                });

                testo = RegexEspandiRiferimenti4().Replace(testo, m =>
                    $"{cap} {m.Groups[1].Value} {vers} {m.Groups[2].Value}");
                return testo;
            }
            catch
            {
                return testo;
            }
        }

        private static Regex? regexRiferimentoVersettoBibbia;
        private static bool regexRiferimentoVersettoBibbiaTentataCreazione;

        private static Regex? OttieniRegexRiferimentoVersettoBibbia()
        {
            if (regexRiferimentoVersettoBibbia != null || regexRiferimentoVersettoBibbiaTentataCreazione)
                return regexRiferimentoVersettoBibbia;
            regexRiferimentoVersettoBibbiaTentataCreazione = true;
            try
            {
                // TODO2 Berea regex
                regexRiferimentoVersettoBibbia = new Regex(
                    @"\b(?:[1-3]\s*)?[A-Za-zÀ-ÿ]+\.?\s+\d+\s*:\s*\d+\s*(?:[,\/\-]\s*\d+\s*)*",
                    RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }
            catch { regexRiferimentoVersettoBibbia = null; }
            return regexRiferimentoVersettoBibbia;
        }

        private static string RimuoviRiferimentiVersettoBibbia(string testo)
        {
            Regex? regex = OttieniRegexRiferimentoVersettoBibbia();
            if (regex == null) return testo;
            try
            {
                string risultato = regex.Replace(testo, "");
                /*
                if (risultato != testo)
                {
                    try { System.IO.File.AppendAllText(@"C:\Users\amministratore\AppData\Local\Temp\tts_debug.txt",
                        $"RIMOSSO: {{{testo.Substring(0, Math.Min(100, testo.Length))}}}\nRESTO: {{{risultato.Substring(0, Math.Min(100, risultato.Length))}}}\n\n"); }
                    catch { }
                }*/
                return risultato;
            }
            catch { return testo; }
        }

        private static string CostruisciPatternNomeLibro(string nome)
        {
            Match m = RegExCostruisciNomeLibro().Match(nome);
            if (m.Success)
                return Regex.Escape(m.Groups[1].Value) + @"\s*" + Regex.Escape(m.Groups[2].Value);
            return Regex.Escape(nome);
        }

        private static Regex? regexNomiLibriNumerati;
        private static bool regexNomiLibriNumeratiTentataCreazione;

        private static Regex? OttieniRegexNomiLibriNumerati()
        {
            if (regexNomiLibriNumerati != null || regexNomiLibriNumeratiTentataCreazione)
                return regexNomiLibriNumerati;
            regexNomiLibriNumeratiTentataCreazione = true;
            try
            {
                List<string> nomi = [];
                for (byte n = 1; n <= 73; n++)
                {
                    string nome = MainWindow.Testi.GetLibroNome(n);
                    if (!string.IsNullOrEmpty(nome) && RegexLibriNumerati().IsMatch(nome))
                        nomi.Add(CostruisciPatternNomeLibro(nome));
                }
                if (nomi.Count == 0) return null;
                nomi.Sort((a, b) => b.Length.CompareTo(a.Length));
                string pattern = @"\b(" + string.Join("|", nomi) + @")\b";
                regexNomiLibriNumerati = new Regex(pattern, RegexOptions.Compiled);
            }
            catch { regexNomiLibriNumerati = null; }
            return regexNomiLibriNumerati;
        }

        private static string EspandiNumeriLibriCompleti(string testo)
        {
            Regex? regex = OttieniRegexNomiLibriNumerati();
            if (regex == null) return testo;
            try { return regex.Replace(testo, m => ConvertiNumeroLibroInOrdinale(m.Value)); }
            catch { return testo; }
        }

        private static Regex? OttieniRegexAbbreviazioniLibri()
        {
            if (regexAbbreviazioniLibri != null || regexAbbreviazioniLibriTentataCreazione)
                return regexAbbreviazioniLibri;
            regexAbbreviazioniLibriTentataCreazione = true;
            try
            {
                HashSet<string> viste = [];
                string[] perLibro = MainWindow.Testi.LibriAbbreviazioniRiconosciute.AbbreviazioniPerLibro();
                foreach (string variantiLibro in perLibro)
                {
                    if (string.IsNullOrEmpty(variantiLibro)) continue;
                    foreach (string variante in variantiLibro.Split(','))
                    {
                        string v = variante.Trim();
                        if (v.Length > 0) viste.Add(v);
                    }
                }
                // Aggiunge abbreviazioni italiane mancanti (non presenti nell'elenco inglese caricato da Testi)
                string[] itExtra = ["ge","gn","eo","es","le","lv","nm","nu","de","dt","gios","gs","gdc","giudic","rt","ru",
                    "1s","2s","1r","2r","1cr","2cr","ed","esd","ne","tb","to","giudit","est","et","1m","2m","gb","giob",
                    "sal","sl","pr","pv","ec","q","ca","cc","ct","sap","si","is","ger","gr","la","b","ez","da","dn","o",
                    "gioe","gl","am","abd","ad","gion","mi","na","aba","ac","h","so","ag","z","mal","ml","mat","mt",
                    "mar","mc","mr","lc","lu","giov","gv","at","rm","ro","1co","ico","2co","iico","ga","ef","fili","fl",
                    "cl","co","1te","ite","2te","iite","1ti","iti","2ti","iiti","ti","tt","file","fm","eb","gc","gia","gm",
                    "1p","ip","2p","iip","1g","ig","2g","iig","3g","iiig","gd","giuda","ap","1s","2s","1r","1cr","2cr",
                    "1m","2m","1co","2co","1te","2te","1ti","2ti","1p","2p","1g","2g","3g"];
                foreach (string a in itExtra)
                    if (a.Length > 0) viste.Add(a);

                List<String> abbreviazioni = [.. viste.OrderByDescending(x => x.Length).Select(Regex.Escape)];
                if (abbreviazioni.Count == 0) return null;
                abbreviazioni.Sort((a, b) => b.Length.CompareTo(a.Length));
                string pattern = @"\b(" + string.Join("|", abbreviazioni) + @")\.?\s*(?=\d)";
                regexAbbreviazioniLibri = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }
            catch { regexAbbreviazioniLibri = null; }
            return regexAbbreviazioniLibri;
        }

        private static void ApplicaImpostazioniVoce()
        {
            try { sintetizzatore.Rate = ConvertiVelocitaInRateSapi(MainWindow.settings.VelocitaVoce); }
            catch { }
            try { sintetizzatore.Volume = (int)MainWindow.settings.VolumeVoce; }
            catch { }
        }

        /// <summary>VelocitaVoce (0.0-2.0, 1.0 = normale) era la scala di
        /// Windows.Media.SpeechSynthesis (SpeakingRate). System.Speech.Synthesis usa invece una
        /// scala intera -10 (più lenta) / +10 (più veloce), 0 = normale: Log2(velocità) mappa
        /// 0.5x -> -10, 1x -> 0, 2x -> +10, coerente con come i motori TTS scalano tipicamente la
        /// velocità (raddoppiare/dimezzare la velocità agli estremi della scala SAPI5).</summary>
        private static int ConvertiVelocitaInRateSapi(double velocitaVoce)
        {
            if (velocitaVoce <= 0) return 0;
            int rate = (int)Math.Round(10.0 * Math.Log2(velocitaVoce));
            return Math.Clamp(rate, -10, 10);
        }

        /// <summary>Offset di inizio (in caratteri, dentro <paramref name="testo"/>) di ciascuna
        /// parola di <paramref name="parole"/>, nell'ordine in cui compaiono - usato per far
        /// corrispondere l'evento SpeakProgress (CharacterPosition dentro tutto il testo) alla
        /// parola giusta.</summary>
        private static int[] CalcolaOffsetParole(string testo, string[] parole)
        {
            int[] offsets = new int[parole.Length];
            int pos = 0;
            for (int i = 0; i < parole.Length; i++)
            {
                int trovato = testo.IndexOf(parole[i], pos, StringComparison.Ordinal);
                offsets[i] = trovato >= 0 ? trovato : pos;
                pos = offsets[i] + parole[i].Length;
            }
            return offsets;
        }

        private static void AvviaLettura(Button pulsante, string testo)
        {
            try
            {
                ApplicaImpostazioniVoce();

                offsetsParoleLette = CalcolaOffsetParole(testo, paroleLetteAttuali ?? []);
                indiceParolaCorrenteEvidenziata = -1;
                istanteInizioParolaCorrente = null;
                msPerCarattereStimati = 90.0; // valore di partenza per una nuova sessione, si autocorregge dal primo evento reale

                AvviaTimerRiempimento(pulsante);
                sintetizzatore.SpeakAsync(testo);
            }
            catch (Exception exc)
            {
                //System.Diagnostics.Debug.WriteLine($"Errore TTS: {exc.Message}");

                if (pulsante != null && !pulsante.Dispatcher.HasShutdownStarted)
                {
                    void mostraErrore()
                    {
                        Window finestraPadre = Window.GetWindow(pulsante);
                        MessageBoxLPN.Show(finestraPadre ?? Application.Current.MainWindow, String.Format(CultureInfo.InvariantCulture, (string)(Application.Current.TryFindResource("SintesiAvvioErrore") ?? "The speech synthesis could not be started: {0}."), exc.Message), (string)(Application.Current.TryFindResource("Errore") ?? "Error"));
                    }

                    if (!pulsante.Dispatcher.CheckAccess())
                        pulsante.Dispatcher.Invoke(mostraErrore);
                    else
                        mostraErrore();
                }

                if (pulsanteAttivoRef.TryGetTarget(out Button? globale) && globale == pulsante)
                {
                    pulsanteAttivoRef.SetTarget(null!);
                }
                FermaEvidenziazione();

                if (pulsante != null)
                    RipristinaSpecificoPulsante(pulsante);
            }
        }

        internal static void Ferma()
        {
            Button? daRipristinare = null;
            if (pulsanteAttivoRef.TryGetTarget(out Button? pulsanteAttivo))
            {
                daRipristinare = pulsanteAttivo;
            }

            try
            {
                sintetizzatore.SpeakAsyncCancelAll();
            }
            catch (Exception /*exc*/)
            {
                // Program.RegistraErrore(exc);
            }

            FermaEvidenziazione();

            // Se c'era un pulsante attivo, ripuliamo il riferimento statico e lo ripristiniamo subito
            if (daRipristinare != null)
            {
                pulsanteAttivoRef.SetTarget(null!); // Svuota correttamente il WeakReference statico
                RipristinaSpecificoPulsante(daRipristinare);
            }
        }

        /// <summary>Riceve, dalla stessa istanza condivisa di SpeechSynthesizer, un evento reale
        /// per ogni parola effettivamente pronunciata - arriva su un thread interno del
        /// sintetizzatore, non sul thread della UI (marshalling tramite Dispatcher qui sotto).</summary>
        private static void OnSpeakProgress(object? sender, SpeakProgressEventArgs e)
        {
            if (!MainWindow.settings.VoceEvidenzia)
                return;
            if (!pulsanteAttivoRef.TryGetTarget(out Button? pulsante))
                return;

            try
            {
                pennelloLetto = (SolidColorBrush)Application.Current.FindResource("AppForegroundBrush");
            }
            catch
            {
                pennelloLetto = Brushes.Black;
            }

            void GestisciSuUI()
            {
                try
                {
                    if (!pulsanteAttivoRef.TryGetTarget(out Button? attivo) || attivo != pulsante)
                        return;

                    int[]? offsets = offsetsParoleLette;
                    ParolaConPosizione[]? mappatura = mappaturaParoleAttuale;
                    IFlowDocumentHost? host = hostAttuale;
                    if (offsets == null || offsets.Length == 0 || mappatura == null || mappatura.Length == 0 || host == null)
                        return;

                    int nuovoIndice = -1;
                    for (int i = 0; i < offsets.Length; i++)
                    {
                        if (offsets[i] <= e.CharacterPosition) nuovoIndice = i;
                        else break;
                    }
                    if (nuovoIndice < 0 || nuovoIndice == indiceParolaCorrenteEvidenziata)
                        return;

                    if (indiceParolaCorrenteEvidenziata >= 0)
                    {
                        ParolaConPosizione precedente = mappatura[Math.Min(indiceParolaCorrenteEvidenziata, mappatura.Length - 1)];
                        host.EseguiSenzaSporcareDocumento(() =>
                        {
                            try
                            {
                                TextRange intervallo = new(precedente.Inizio, precedente.Fine);
                                intervallo.ApplyPropertyValue(TextElement.ForegroundProperty, pennelloLetto);
                            }
                            catch { }
                        });

                        if (istanteInizioParolaCorrente != null && precedente.Testo.Length > 0)
                        {
                            double msReali = (DateTime.Now - istanteInizioParolaCorrente.Value).TotalMilliseconds;
                            if (msReali > 0)
                                msPerCarattereStimati = (msPerCarattereStimati + msReali / precedente.Testo.Length) / 2.0;
                        }
                    }

                    indiceParolaCorrenteEvidenziata = nuovoIndice;
                    istanteInizioParolaCorrente = DateTime.Now;

                    ParolaConPosizione corrente = mappatura[Math.Min(nuovoIndice, mappatura.Length - 1)];
                    puntiTaglioParolaCorrente = PrecalcolaPuntiTaglio(corrente);
                    indiceParolaConPuntiPrecalcolati = nuovoIndice;

                    corrente.Inizio.Paragraph?.BringIntoView();
                }
                catch { }
            }

            try
            {
                if (pulsante.Dispatcher.CheckAccess())
                    GestisciSuUI();
                else
                    pulsante.Dispatcher.BeginInvoke(GestisciSuUI);
            }
            catch { }
        }

        /// <summary>Fine naturale (o fallita, o annullata da Ferma()) della lettura in corso.</summary>
        private static void OnSpeakCompleted(object? sender, SpeakCompletedEventArgs e)
        {
            if (!pulsanteAttivoRef.TryGetTarget(out Button? pulsante))
                return;

            void GestisciSuUI()
            {
                try
                {
                    if (!pulsanteAttivoRef.TryGetTarget(out Button? attivo) || attivo != pulsante)
                        return;

                    pulsanteAttivoRef.SetTarget(null!);
                    FermaEvidenziazione();
                    RipristinaSpecificoPulsante(pulsante);

                    if (e.Error != null && !e.Cancelled && !pulsante.Dispatcher.HasShutdownStarted)
                    {
                        Window finestraPadre = Window.GetWindow(pulsante);
                        MessageBoxLPN.Show(finestraPadre ?? Application.Current.MainWindow,
                            String.Format(CultureInfo.InvariantCulture, (string)(Application.Current.TryFindResource("SintesiAvvioErrore") ?? "The speech synthesis could not be started: {0}."), e.Error.Message),
                            (string)(Application.Current.TryFindResource("Errore") ?? "Error"));
                    }
                }
                catch { }
            }

            try
            {
                if (pulsante.Dispatcher.CheckAccess())
                    GestisciSuUI();
                else
                    pulsante.Dispatcher.BeginInvoke(GestisciSuUI);
            }
            catch { }
        }

        private static void RipristinaSpecificoPulsante(Button pulsante)
        {
            if (pulsante == null || pulsante.Dispatcher.HasShutdownStarted)
                return;

            if (!pulsante.Dispatcher.CheckAccess())
            {
                pulsante.Dispatcher.Invoke(() => EseguiRipristinoSuUI(pulsante));
            }
            else
            {
                EseguiRipristinoSuUI(pulsante);
            }
        }

        internal static void FermaSeAttivo(Button pulsante)
        {
            if (pulsanteAttivoRef.TryGetTarget(out Button? pulsanteAttivo) && pulsanteAttivo == pulsante)
            {
                Ferma();
            }
        }

        internal static void Chiudi()
        {
            FermaEvidenziazione();
            try
            {
                sintetizzatore.SpeakAsyncCancelAll();
            }
            catch (Exception /*exc*/)
            {
                // Program.RegistraErrore(exc);
            }
            try
            {
                sintetizzatore.Dispose();
            }
            catch (Exception /*exc*/)
            {
                //   Program.RegistraErrore(exc);
            }
        }

        private static void EseguiRipristinoSuUI(Button pulsante)
        {
            // Verifichiamo che il controllo sia ancora attivo nell'interfaccia (equivalente a !IsDisposed)
            if (pulsante == null)
                return;

            // Gestiamo l'aggiornamento grafico (Icona o Testo)
            if (pulsante.Content is PackIconMaterial icona)
            {
                icona.Kind = PackIconMaterialKind.AccountVoice;
                pulsante.ToolTip = (string)(Application.Current.TryFindResource("ViewerVoceAiuto") ?? "Start the speech synthesis to read this text");
            }
        }

        // ------------------------------------------------------------------------------------
        // Evidenziazione della parola in lettura (karaoke)
        // ------------------------------------------------------------------------------------
        // Meccanismo (aggiornato 2026-07-18 per usare eventi reali invece di stime - vedi
        // changelog nella guida):
        //  1. EstraiParoleConPosizione cammina il FlowDocument con TextPointer ed elenca ogni
        //     parola visibile con l'esatto TextPointer di inizio/fine nel documento mostrato a
        //     schermo. Registra anche se la parola e' in grassetto (numeri di versetto) e se e'
        //     la prima del suo paragrafo (marcatori di elenco tipo "I.", "(1.)").
        //  2. Le parole COSI' OTTENUTE vengono rifuse in una stringa e passate alle funzioni di
        //     trasformazione gia' esistenti (RimuoviRiferimentiVersettoBibbia, ecc.) per ottenere
        //     il testo davvero pronunciato - che percio' NON corrisponde piu' parola-per-parola
        //     all'originale (riferimenti tolti, abbreviazioni espanse in piu' parole).
        //  3. AllineaParole fa combaciare le due sequenze di parole (originale vs pronunciata) con
        //     un allineamento a sottosequenza comune piu' lunga (LCS).
        //  4. System.Speech.Synthesis.SpeechSynthesizer.SpeakProgress riporta un evento REALE per
        //     ogni parola effettivamente pronunciata, con la posizione esatta nel testo - non piu'
        //     un marcatore precalcolato ne' una stima: quando arriva, sappiamo con certezza che la
        //     parola precedente e' finita e quella nuova e' iniziata (CalcolaOffsetParole fa
        //     corrispondere la posizione nel testo alla parola giusta).
        //  5. La parola corrente si riempie PROGRESSIVAMENTE in rosso da sinistra a destra tramite
        //     un timer leggero (solo per l'animazione, non per decidere "quale" parola è corrente
        //     - quello lo decide sempre l'evento reale): stima quanti millisecondi per carattere
        //     ci vogliono in base all'ultima parola REALMENTE misurata (msPerCarattereStimati, si
        //     autocorregge parola per parola, l'errore non si accumula mai sull'intera lettura) -
        //     due TextRange colorate separatamente (non un LinearGradientBrush: WPF applicherebbe
        //     un gradiente per singolo glifo invece che uno continuo sull'intera parola) - le
        //     parole già lette diventano blu scuro (#2C3E50); quelle non ancora raggiunte
        //     restano al colore normale del tema (non toccate).

        private readonly struct ParolaConPosizione
        {
            internal string Testo { get; }
            internal TextPointer Inizio { get; }
            internal TextPointer Fine { get; }
            internal bool Grassetto { get; }
            internal bool InizioParagrafo { get; }

            internal ParolaConPosizione(string testo, TextPointer inizio, TextPointer fine, bool grassetto, bool inizioParagrafo)
            {
                Testo = testo;
                Inizio = inizio;
                Fine = fine;
                Grassetto = grassetto;
                InizioParagrafo = inizioParagrafo;
            }
        }

        private static List<ParolaConPosizione> EstraiParoleConPosizione(FlowDocument documento)
        {
            List<ParolaConPosizione> parole = new List<ParolaConPosizione>();
            TextPointer? posizione = documento.ContentStart;
            Paragraph? paragrafoPrecedente = null;
            while (posizione != null)
            {
                if (posizione.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    string testoBlocco = posizione.GetTextInRun(LogicalDirection.Forward);
                    bool grassetto = posizione.Parent is Run run && run.FontWeight == FontWeights.Bold;
                    foreach (Match m in RegexParola().Matches(testoBlocco))
                    {
                        TextPointer inizioParola = posizione.GetPositionAtOffset(m.Index)!;
                        TextPointer fineParola = inizioParola.GetPositionAtOffset(m.Length)!;
                        Paragraph? paragrafo = inizioParola.Paragraph;
                        bool inizioParagrafo = paragrafo != paragrafoPrecedente;
                        paragrafoPrecedente = paragrafo;
                        parole.Add(new ParolaConPosizione(m.Value, inizioParola, fineParola, grassetto, inizioParagrafo));
                    }
                    posizione = posizione.GetPositionAtOffset(testoBlocco.Length);
                }
                else
                {
                    posizione = posizione.GetNextContextPosition(LogicalDirection.Forward);
                }
            }
            return parole;
        }

        private static bool EstaNumeroDiVersetto(ParolaConPosizione parola)
        {
            return parola.Grassetto && RegexNumeroVersetto().IsMatch(parola.Testo);
        }

        private static bool EstaMarcatoreElencoAInizioParagrafo(ParolaConPosizione parola)
        {
            return parola.InizioParagrafo && RegexMarcatoreElenco().IsMatch(parola.Testo);
        }

        // Limite di sicurezza sulla tabella di allineamento (O(n*m) in tempo e spazio) - oltre
        // questa dimensione l'evidenziazione si disattiva da sola (la lettura audio prosegue
        // comunque) invece di rischiare di rallentare l'avvio della lettura su testi enormi.
        private const int LimiteParolePerAllineamento = 4000;

        private static ParolaConPosizione[] AllineaParole(List<ParolaConPosizione> originali, string[] lette)
        {
            int n = originali.Count, m = lette.Length;
            ParolaConPosizione[] risultato = new ParolaConPosizione[m];
            if (n == 0 || m == 0 || (long)n * m > (long)LimiteParolePerAllineamento * LimiteParolePerAllineamento)
                return risultato;

            string[] normOriginali = [.. originali.Select(p => NormalizzaParolaPerConfronto(p.Testo))];
            string[] normLette = [.. lette.Select(NormalizzaParolaPerConfronto)];

            // dp[i,j] = lunghezza della LCS fra originali[i..] e lette[j..].
            int[,] dp = new int[n + 1, m + 1];
            for (int i = n - 1; i >= 0; i--)
                for (int j = m - 1; j >= 0; j--)
                    dp[i, j] = normOriginali[i] == normLette[j]
                        ? dp[i + 1, j + 1] + 1
                        : Math.Max(dp[i + 1, j], dp[i, j + 1]);

            int oi = 0, li = 0;
            ParolaConPosizione ultimaConosciuta = originali[0];
            while (li < m)
            {
                if (oi < n && normOriginali[oi] == normLette[li] && dp[oi, li] == dp[oi + 1, li + 1] + 1)
                {
                    ultimaConosciuta = originali[oi];
                    risultato[li] = ultimaConosciuta;
                    oi++; li++;
                }
                else if (oi < n && dp[oi + 1, li] >= dp[oi, li + 1])
                {
                    oi++; // parola originale "saltata" (es. il riferimento di versetto rimosso)
                }
                else
                {
                    // parola pronunciata "inserita" dalla trasformazione (es. un ordinale, o
                    // "capitolo"/"versetto") - resta ancorata all'ultima parola originale nota.
                    risultato[li] = ultimaConosciuta;
                    li++;
                }
            }
            return risultato;
        }

        private static string NormalizzaParolaPerConfronto(string parola)
        {
            return parola.Trim(',', '.', ';', ':', '!', '?', '"', '\'', '(', ')', '«', '»', '-').ToLowerInvariant();
        }

        /// <summary>Avvia (o riavvia, se gia' presente) il timer leggero che anima SOLO il
        /// riempimento progressivo dentro la parola corrente - NON decide quale parola è corrente
        /// (quello lo fa OnSpeakProgress con un evento reale), si limita a interpolare fra
        /// l'istante di inizio della parola corrente e la sua durata stimata.</summary>
        private static void AvviaTimerRiempimento(Button pulsante)
        {
            timerEvidenziazione?.Stop();
            timerEvidenziazione = new DispatcherTimer(DispatcherPriority.Render, pulsante.Dispatcher)
            {
                Interval = TimeSpan.FromMilliseconds(50),
            };
            timerEvidenziazione.Tick += (s, e) =>
            {
                try
                {
                    if (!pulsanteAttivoRef.TryGetTarget(out Button? attivo) || attivo != pulsante)
                    {
                        timerEvidenziazione?.Stop();
                        return;
                    }
                    ParolaConPosizione[]? mappatura = mappaturaParoleAttuale;
                    IFlowDocumentHost? host = hostAttuale;
                    TextPointer[]? puntiTaglio = puntiTaglioParolaCorrente;
                    if (mappatura == null || mappatura.Length == 0 || host == null || puntiTaglio == null
                        || indiceParolaCorrenteEvidenziata < 0 || istanteInizioParolaCorrente == null)
                        return;

                    ParolaConPosizione corrente = mappatura[Math.Min(indiceParolaCorrenteEvidenziata, mappatura.Length - 1)];
                    int lunghezza = corrente.Testo.Length;
                    double durataStimataMs = Math.Max(1.0, lunghezza * msPerCarattereStimati);
                    double trascorsoMs = (DateTime.Now - istanteInizioParolaCorrente.Value).TotalMilliseconds;
                    double progresso = Math.Clamp(trascorsoMs / durataStimataMs, 0.0, 1.0);

                    host.EseguiSenzaSporcareDocumento(() =>
                    {
                        try
                        {
                            int offsetTaglio = Math.Clamp((int)Math.Round(progresso * lunghezza), 0, lunghezza);
                            if (offsetTaglio > 0 && offsetTaglio < puntiTaglio.Length)
                            {
                                TextPointer puntoTaglio = puntiTaglio[offsetTaglio];
                                TextRange intervalloEvidenziato = new(corrente.Inizio, puntoTaglio);
                                intervalloEvidenziato.ApplyPropertyValue(TextElement.ForegroundProperty, pennelloParolaCorrente);

                                if (offsetTaglio < lunghezza)
                                {
                                    TextRange intervalloRestante = new(puntoTaglio, corrente.Fine);
                                    intervalloRestante.ApplyPropertyValue(TextElement.ForegroundProperty, pennelloLetto);
                                }
                            }
                        }
                        catch { }
                    });
                }
                catch { }
            };
            timerEvidenziazione.Start();
        }

        /// <summary>Calcola in un colpo solo, dalla parola ancora "pulita" (nessuna colorazione
        /// applicata), il TextPointer corrispondente a ogni possibile offset di carattere al suo
        /// interno - da richiamare una sola volta quando si raggiunge questa parola, non a ogni
        /// tick del timer (vedi il commento sul campo puntiTaglioParolaCorrente).</summary>
        private static TextPointer[] PrecalcolaPuntiTaglio(ParolaConPosizione parola)
        {
            int lunghezza = parola.Testo.Length;
            TextPointer[] punti = new TextPointer[lunghezza + 1];
            punti[0] = parola.Inizio;
            for (int i = 1; i <= lunghezza; i++)
                punti[i] = parola.Inizio.GetPositionAtOffset(i) ?? parola.Fine;
            return punti;
        }

        /// <summary>Colore di foreground normale del tema attivo (Chiaro/Scuro) - usato per
        /// resettare le parole a fine lettura e per la porzione non ancora raggiunta della parola
        /// corrente, così che l'evidenziazione resti leggibile in entrambi i temi (a differenza
        /// di un nero fisso, illeggibile su sfondo scuro).</summary>
        private static Brush ColoreTemaCorrente()
        {
            try
            {
                return (Brush)Application.Current.FindResource("AppForegroundBrush");
            }
            catch
            {
                return Brushes.Black;
            }
        }

        private static void FermaEvidenziazione()
        {
            timerEvidenziazione?.Stop();
            timerEvidenziazione = null;
            if (paroleOriginaliAttuali != null && hostAttuale != null)
            {
                IFlowDocumentHost host = hostAttuale;
                List<ParolaConPosizione> parole = paroleOriginaliAttuali;
                host.EseguiSenzaSporcareDocumento(() =>
                {
                    Brush fg = pennelloLetto;
                    foreach (ParolaConPosizione parola in parole)
                    {
                        try
                        {
                            TextRange intervallo = new(parola.Inizio, parola.Fine);
                            intervallo.ApplyPropertyValue(TextElement.ForegroundProperty, fg);
                        }
                        catch { }
                    }
                });
            }
            hostAttuale = null;
            paroleOriginaliAttuali = null;
            mappaturaParoleAttuale = null;
            paroleLetteAttuali = null;
            offsetsParoleLette = null;
            indiceParolaCorrenteEvidenziata = -1;
            istanteInizioParolaCorrente = null;
            puntiTaglioParolaCorrente = null;
            indiceParolaConPuntiPrecalcolati = -1;
        }

        [GeneratedRegex(@"\S+")]
        private static partial Regex RegexParola();
        [GeneratedRegex(@"^\d{1,3}$")]
        private static partial Regex RegexNumeroVersetto();
        [GeneratedRegex(@"^(?:[IVXLCDM]{1,4}\.|\(\d{1,3}\.?\)|\[\d{1,3}\.?\]|\d{1,3}\.)$")]
        private static partial Regex RegexMarcatoreElenco();
        [GeneratedRegex(@"^([123])\s*(.+)$")]
        private static partial Regex RegexNumeroLibroInOrdinale();
        [GeneratedRegex(@"(\d+)\s*:\s*(\d+)\s*-\s*(\d+)\b")]
        private static partial Regex RegexEspandiRiferimenti1();
        [GeneratedRegex(@"(\d+)\s*:\s*(\d+(?:\s*,\s*\d+)+)\b")]
        private static partial Regex RegexEspandiRiferimenti2();
        [GeneratedRegex(@"\s*,\s*")]
        private static partial Regex RegexEspandiRiferimenti3();
        [GeneratedRegex(@"(\d+)\s*:\s*(\d+)\b")]
        private static partial Regex RegexEspandiRiferimenti4();
        [GeneratedRegex(@"^([123])(.+)$")]
        private static partial Regex RegExCostruisciNomeLibro();
        [GeneratedRegex(@"^[123]")]
        private static partial Regex RegexLibriNumerati();
    }
}
