package net.laparola.core;

import java.io.*;
import java.text.Collator;
import java.util.*;
import java.util.concurrent.ConcurrentHashMap;

import org.w3c.dom.*;
import org.xml.sax.*;

import javax.xml.parsers.*;

//TODO
//parole ricercate non sottolineate bene se titolo dentro il versetto (1Cor 12:31)
//collegamenti cliccabili nei riferimenti (<a href="1Cor 13">1Cor 13</a> oppure <a href="Matteo 5,1-7,28">Discorso della montagna</a>)
//senza riferimento: troppi spazi
// se non c'è Market, lpn/Android non visualizzato bene in alcuni browser di Android; vedi anche http://developer.android.com/guide/webapps/targeting.html

//preferiti, cronologia, condividi guida
// lpn/javafile/index.php (Mac e Linux link e cartella)

//TODO altro
//libreria: sposta da/a sd
// dizionari, libri
// testi greci, ebraici
// testo scorrevole
// evidenziatore
// organizzare/gestire preferiti

// Nota: Quando cambi il file della versione nelle risorse, per informare il programma dovresti cambiare anche la funzione LaParolaActivityInitUtility.checkBibleInstalled.

public class Testi implements Closeable {
    private Map<String, Testo> listaVersioni = null;
    private Set<String> listaFileIllegibili = null;
    private FormatoTesto formato;
    public UUID deviceUuid;

    public FormatoTesto getFormato() {
        return formato;
    }

    public void setFormato(FormatoTesto valore) {
        valore.copiaA(formato);
    }

    public static final String URL_FILE_AGGIORNAMENTI = "https://www.laparola.net/javafile/aggiorna3.xml";

    // public static final String LIBRI_NOMI_INGLESE = "|Genesis|Exodus|Leviticus|Numbers|Deuteronomy|Joshua|Judges|Ruth|1Samuel|2Samuel|1Kings|2Kings|1Chronicles|2Chronicles|Ezra|Nehemiah|Tobit|Judith|Esther|1Maccabees|2Maccabees|Job|Psalms|Proverbs|Ecclesiastes|Song of Songs|Wisdom|Sirach|Isaiah|Jeremiah|Lamentations|Baruch|Ezekiel|Daniel|Hosea|Joel|Amos|Obadiah|Jonah|Micah|Nahum|Habakkuk|Zephaniah|Haggai|Zechariah|Malachi|Matthew|Mark|Luke|John|Acts|Romans|1Corinthians|2Corinthians|Galatians|Ephesians|Philippians|Colossians|1Thessalonians|2Thessalonians|1Timothy|2Timothy|Titus|Philemon|Hebrews|James|1Peter|2Peter|1John|2John|3John|Jude|Revelation";
    public static final String LIBRI_NOMI_ITALIANO = "|Genesi|Esodo|Levitico|Numeri|Deuteronomio|Giosuè|Giudici|Rut|1Samuele|2Samuele|1Re|2Re|1Cronache|2Cronache|Esdra|Neemia|Tobia|Giuditta|Ester|1Maccabei|2Maccabei|Giobbe|Salmi|Proverbi|Ecclesiaste|Cantico|Sapienza|Siracide|Isaia|Geremia|Lamentazioni|Baruc|Ezechiele|Daniele|Osea|Gioele|Amos|Abdia|Giona|Michea|Naum|Abacuc|Sofonia|Aggeo|Zaccaria|Malachia|Matteo|Marco|Luca|Giovanni|Atti|Romani|1Corinzi|2Corinzi|Galati|Efesini|Filippesi|Colossesi|1Tessalonicesi|2Tessalonicesi|1Timoteo|2Timoteo|Tito|Filemone|Ebrei|Giacomo|1Pietro|2Pietro|1Giovanni|2Giovanni|3Giovanni|Giuda|Apocalisse";
    // private static final String LIBRI_ABBREVIAZIONI_USATE_ENGLISH ="|Gen|Ex|Le|Nu|De|Josh|Judg|Ru|1Sam|2Sam|1K|2K|1Chr|2Chr|Ezra|Ne|Tob|Judi|Est|1M|2M|Job|Ps|Prov|Ec|SS|Wis|Sir|Is|Jer|Lam|Bar|Ezek|Dan|Hos|Joel|Am|Ob|Jon|Mi|Na|Hab|Zep|Hag|Zec|Mal|Mat|Mar|Lu|John|Ac|Ro|1Co|2Co|Ga|Eph|Phili|Col|1Th|2Th|1Ti|2Ti|Tit|Phile|Heb|Jam|1P|2P|1J|2J|3J|Jude|Rev";
    private static final String LIBRI_ABBREVIAZIONI_USATE_ITALIANO = "|Gen|Eso|Le|Nu|De|Gios|Giudic|Ru|1Sam|2Sam|1Re|2Re|1Cr|2Cr|Esd|Ne|Tob|Giudit|Est|1Macc|2Macc|Giob|Sal|Prov|Ec|CC|Sap|Sir|Is|Ger|Lam|Bar|Ez|Da|Os|Gioe|Am|Abd|Gion|Mi|Na|Abac|So|Ag|Zac|Mal|Mt|Mc|Lc|Gv|At|Rm|1Cor|2Cor|Gal|Ef|Fili|Col|1Ts|2Ts|1Tm|2Tm|Tt|Fm|Eb|Giac|1P|2P|1G|2G|3G|Giuda|Ap";
    // private static final String LibriAbbreviazioniRiconosciuteInglese = "|gen,gn|ex|le,lv|nm,nu|de,dt|jos,js|judg,jg|rt,ru|1 s,1s,isam|2 s,2s,iis|1 k,1k,ik|2 k,2k,iik|1 ch,1ch,ich|2 ch,2ch,iich|ezr|ne|tb,to|jdt,jt,judi|est,et|1 m,1m,im|2 m,2m,iim|jb,job|ps|pr,pv|ec|so,ss|w|si|is|je,jr|la|b|ez|da,dn|ho|jl,joe|am|o|jon|mi|na|hab|zep|hag|zec|mal,ml|mat,mt|mar,mk,mr|lk,lu|jn,joh|ac|rm,ro|1 co,1co,ico|2 co,2co,iico|ga|ep|phi,php,pl|cl,co|1 th,1th,1ts,ith|2 th,2th,2ts,iith|1 ti,1ti,1tm,iti|2 ti,2ti,2tm,iiti|ti,tt|phile,phlm,phm,pm|he|jam,jas,jm|1 p,1p,ip|2 p,2p,iip|1 j,1j,ij|2 j,2j,iij|3 j,3j,iiij|jd,jude|re";
    private static final String LIBRI_ABBREVIAZIONI_RICONOSCIUTE_ITALIANO = "|ge,gn|eo,es|le,lv|nm,nu|de,dt|gios,gs|gc,gdc,giudic|rt,ru|1 s,1s,isam|2 s,2s,iis|1 r,1r,ir|2 r,2r,iir|1 cr,1cr,icr|2 cr,2cr,iicr|ed,esd|ne|tb,to|giudit|est,et|1 m,1m,im|2 m,2m,iim|gb,giob|sal,sl|pr,pv|ec,q|ca,cc,ct|sap|si|is|ger,gr|la|b|ez|da,dn|o|gioe,gl|am|abd,ad|gion|mi|na|aba,ac,h|so|ag|z|mal,ml|mat,mt|mar,mc,mr|lc,lu|giov,gv|at|rm,ro|1 co,1co,ico|2 co,2co,iico|ga|ef|fili,fl|cl,co|1 te,1te,1ts,ite|2 te,2te,2ts,iite|1 ti,1ti,1tm,iti|2 ti,2ti,2tm,iiti|ti,tt|file,fm|eb|gia,gm|1 p,1p,ip|2 p,2p,iip|1 g,1g,ig|2 g,2g,iig|3 g,3g,iiig|gd,giuda|ap";
    static final String[] PAROLE_ITALIANE_CON_APOSTROFE = {"be", "co", "com", "da", "di", "die", "dov", "e", "fa", "fe", "mo", "pe", "po", "quant", "que", "rifa", "sta", "va"};
    static final String[] PAROLE_INGLESI_SENZA_APOSTROFE = {"amiss", "apostates", "commandments", "fillets", "holiness", "intercessions", "means", "prayer-fillets", "prayers",
            "prays", "righteous", "terms", "us", "was", "yes"};
    String[] libriNomi;
    String[] libriAbbreviazioniUsate;
    private LibriAbbreviazioniRiconosciuteHash libriAbbreviazioniRiconosciute;

    private volatile String ultimaBibbiaCompleta = "";
    private volatile String ultimaBibbia = "";

    int versioneMassimaFile1;
    int versioneMassimaFile2;

    public enum TestoTipi {
        NESSUNO, BIBBIA, COMMENTARIO, DIZIONARIO, LIBRO
    }

    public enum StatoAggiornamento {
        NON_DISPONIBILE, // non sul server, sull’Android
        SENZA_INTERNET, // sconosciuto se sul server (perché senza Internet), sull'Android
        NON_INSTALLATO, // sul server, non sull’Android
        DA_AGGIORNARE, // sul server, sull’Android, versione_server > versione_android
        AGGIORNATO, // sul server, sull’Android, versione_server <= versione_Android
        AGGIORNAMENTO_NON_COMPATIBILE, // sul server, sull’Android, versione_server > versione_programma
        INSTALLAZIONE_NON_COMPATIBILE, // sul server, non sull’Android, versione_server > versione_programma
        FILE_CORROTTO // sull'Android, ma non leggibile dal programma
    }

    public enum TestoVisualizzato {
        VERSETTI, PARAGRAFI, NESSUNO
    }

    public enum RiferimentoTipo {
        DUE_PUNTI, VIRGOLA, CITAZIONE
    }

    public enum RiferimentoFormato {
        INTERO, ABBREVIAZIONE, NESSUNO, NESSUN_LIBRO, ABBREVIAZIONE_RICONOSCIUTA
    }

    public enum RiferimentoPosto {
        PRIMA_STESSA_RIGA, PRIMA_RIGA_DIVERSA, DOPO
    }

    public static class RiferimentoDiverso {
        public int libroStandard;
        public int capitoloStandard;
        public int versettoStandard;
        public int libroVersione;
        public int capitoloVersione;
        public int versettoVersione;
    }

    public Collator confrontoParole;
    public boolean cacheUltimoFileAggiornamenti;

    private void costruttore(int versioneProgramma1, int versioneProgramma2) {
        listaVersioni = new ConcurrentHashMap<>();
        listaFileIllegibili = ConcurrentHashMap.newKeySet();
        libriNomi = LIBRI_NOMI_ITALIANO.split("\\|");
        libriAbbreviazioniUsate = LIBRI_ABBREVIAZIONI_USATE_ITALIANO.split("\\|");
        libriAbbreviazioniRiconosciute = new LibriAbbreviazioniRiconosciuteHash();
        String[] abbreviazioniItaliane = LIBRI_ABBREVIAZIONI_RICONOSCIUTE_ITALIANO.split("\\|");
        String[] abbreviazioni;
        for (int i = 1; i <= 73; ++i) {
            abbreviazioni = abbreviazioniItaliane[i].split(",");
            for (String s : abbreviazioni) {
                libriAbbreviazioniRiconosciute.put(s, i);
            }
        }

        confrontoParole = new ConfrontoParole();
        formato = new FormatoTesto();

        versioneMassimaFile1 = versioneProgramma1;
        versioneMassimaFile2 = versioneProgramma2;
    }

    public Testi(UUID uuid, int versioneProgramma1, int versioneProgramma2) {
        deviceUuid = uuid;
        costruttore(versioneProgramma1, versioneProgramma2);
    }

    // Aggiungi il file che contiene un testo.
    // percorso: Il percorso (inclusa la cartella) del file da aggiungere.
    // Restituisce falso se il testo esiste già (che non è aggiunto una seconda volta), altrimenti restituisce "".
    public boolean aggiungiTesto(String percorso) {
        Testo t;
        try {
            t = new Testo(this, percorso);
            String nome = t.getInfo().getNome();

            Testo existing = listaVersioni.putIfAbsent(nome, t);
            if (existing != null) {
                t.chiudi();
                return false;
            }

            if (t.capitoliInLibro[1] > 0) {
                ultimaBibbia = nome;
                if (t.capitoliInLibro[17] > 0) {
                    ultimaBibbiaCompleta = nome;
                }
            }
            listaFileIllegibili.remove(percorso);
        } catch (Exception e) {
            listaFileIllegibili.add(percorso);
        }
        return true;
    }

    public List<String> aggiungiTestiDaDirectory(String directory) {
        ArrayList<String> giaCaricati = new ArrayList<>();
        String directoryModificata = directory;
        File dir = new File(directoryModificata);

        FilenameFilter filter = (directoryDaControllare, name) -> name.endsWith(".lpj");
        String[] children = dir.list(filter);
        if (children == null) // "dir" non era una directory valida
            children = new String[0];

        if (!directoryModificata.isEmpty())
            directoryModificata += File.separator;

        for (String child : children) {
            String nomeFile = directoryModificata + child;
            if (!aggiungiTesto(nomeFile))
                giaCaricati.add(nomeFile);
        }

        return giaCaricati;
    }

    // Cancella il file che contiene un testo, e rimuovere il testo dalla lista di testi installati.
    // nomeVersione: Il nome del testo da cancellare.
    // IOException se non è stato possibile cancellare il file
    // TestoNonEsisteException se nomeVersione non esiste
    public void cancellaTesto(String nomeVersione) throws TestoNonEsisteException, IOException {
        getTesto(nomeVersione).cancella();
        // se non è stato possibile cancellare, exception nella riga precedente e la versione non è rimossa dall'elenco nella prossima riga
        listaVersioni.remove(nomeVersione);
        if (ultimaBibbiaCompleta.equals(nomeVersione))
            ultimaBibbiaCompleta = "";
        if (ultimaBibbia.equals(nomeVersione)) {
            if (nomiVersioni().length == 0)
                ultimaBibbia = "";
            else
                ultimaBibbia = nomiVersioni()[0];
        }
    }

    @Override
    public void close() {
        for (Testo testo : listaVersioni.values()) {
            testo.chiudi();
        }
    }

    public Set<String> getFileIllegibili() {
        return listaFileIllegibili;
    }

    private Testo getTesto(String nomeVersione) throws TestoNonEsisteException {
        Testo t = listaVersioni.get(nomeVersione);
        if (t == null)
            throw new TestoNonEsisteException(nomeVersione);
        return t;
    }

    public String[] nomiVersioni() {
        String[] nomi = listaVersioni.keySet().toArray(new String[0]);
        Arrays.sort(nomi);
        return nomi;
    }

    // Informazioni su un testo installato
    // nomeVersione: Il nome della versione nel file dei dati.
    // Restituisce: Informazioni sulla versione.
    public VersioneInformazioni getInfo(String nomeVersione) throws TestoNonEsisteException {
        return getTesto(nomeVersione).getInfo();
    }

    // public String getLinguaTesto(String nomeVersione) throws TestoNonEsisteException {    // return getInfo(nomeVersione).getLingua();    // }

    public String getUltimaBibbia() {
        return (ultimaBibbiaCompleta.isEmpty() ? ultimaBibbia : ultimaBibbiaCompleta);
    }

    public String getLibroAbbreviazioneUsata(int numeroLibro) {
        return ((numeroLibro >= 1 && numeroLibro <= 73) ? libriAbbreviazioniUsate[numeroLibro] : "");
    }

    public void setLibroAbbreviazioneUsata(int numeroLibro, String nome) {
        if (numeroLibro >= 1 && numeroLibro <= 73) {
            libriAbbreviazioniUsate[numeroLibro] = nome;
        }
    }

    LibriAbbreviazioniRiconosciuteHash getLibriAbbreviazioniRiconosciute() {
        return libriAbbreviazioniRiconosciute;
    }

    // La radice di una certa parola in un testo.
    public String radiceDiParola(String parola, String nomeVersione) {
        if (nomeVersione == null)
            return parola;
        try {
            return getTesto(nomeVersione).radiceDiParola(parola);
        } catch (TestoNonEsisteException e) {
            return parola;
        }
    }

    // #NB
    // @Override
    public CharSequence getBrano(String riferimento, String nomeVersione) throws TestoNonEsisteException {
        return getBrano(convertiRiferimento(riferimento), nomeVersione);
    }

    public CharSequence getBrano(Riferimento riferimento, String nomeVersione) throws TestoNonEsisteException {
        return getBrano(riferimento, nomeVersione, null);
    }

    public CharSequence getBrano(String riferimento, String nomeVersione, String nomeCommentario) throws TestoNonEsisteException {
        return getBrano(convertiRiferimento(riferimento), nomeVersione, nomeCommentario);
    }

    public CharSequence getBrano(Riferimento riferimento, String nomeVersione, String nomeCommentario) throws TestoNonEsisteException {
        ultimaBibbia = nomeVersione;
        if (getTesto(nomeVersione).capitoliInLibro[17] > 0)
            ultimaBibbiaCompleta = nomeVersione;

        Riferimento noteDaVisualizzare;
        if (nomeCommentario != null && !nomeCommentario.isEmpty())
            noteDaVisualizzare = getTesto(nomeCommentario).elencaNoteInBrano(riferimento);
        else
            noteDaVisualizzare = new Riferimento();

        return getTesto(nomeVersione).getBrano(riferimento, new Riferimento(), nomeCommentario, noteDaVisualizzare);
    }

    // Interrompe la creazione del testo di un brano
    // nomeVersione: Il nome del testo di cui interrompere il processo
    public void interrompiGetBrano(String nomeVersione) {
        try {
            getTesto(nomeVersione).interrompiGetBrano();
        } catch (TestoNonEsisteException e) {
            // non fare niente, cioè non interrompiamo
        }
    }

    public String getNotaConTitolo(String titolo, String nomeVersione) throws TestoNonEsisteException {
        return getTesto(nomeVersione).getNotaConTitolo(titolo);
    }

    public String getNota(Riferimento riferimento, String nomeVersione) throws TestoNonEsisteException {
        return riferimento == null ? "" : getNotaConTitolo(riferimento.comeNotaTuttoRiferimento(), nomeVersione);
    }

    // Restituisce un elenco di tutte le note.
    public List<String> note(String nomeVersione) throws TestoNonEsisteException {
        List<String> note = new ArrayList<>(getTesto(nomeVersione).noteTitoli.size());
        note.addAll(getTesto(nomeVersione).noteTitoli);
        return note;
    }

    // / Restituisce un elenco di tutte le note con un titolo.
    public List<String> noteConTitolo(String nomeVersione) throws TestoNonEsisteException {
        List<String> noteDaControllare = getTesto(nomeVersione).noteTitoli;
        int numeroNote = noteDaControllare.size();
        List<String> note = new ArrayList<>(numeroNote);
        for (int i = 0; i < numeroNote; ++i)
            if (noteDaControllare.get(i).charAt(0) != '#')
                note.add(noteDaControllare.get(i));
        return note;
    }

    public Riferimento ricerca(String espressione, String riferimentoDaRicercare, String nomeVersione) throws RicercaEspressioneVuotaException, RicercaErroreSintassiException,
            RicercaParentesiException, RicercaParentesiQuadrateException {
        return ricerca(espressione, convertiRiferimento(riferimentoDaRicercare), nomeVersione);
    }

    public Riferimento ricerca(String espressione, Riferimento riferimentoDaRicercare, String nomeVersione) throws RicercaEspressioneVuotaException,
            RicercaErroreSintassiException, RicercaParentesiException, RicercaParentesiQuadrateException {
        Riferimento versettiTrovati = trovaOccorrenzeEspressione(controllaEspressioneDaRicercare(espressione, nomeVersione), riferimentoDaRicercare, false, 0, nomeVersione);
        return unisciVociRipetute(versettiTrovati);
    }

    private String controllaEspressioneDaRicercare(String espressioneDaControllare, String nomeVersione) throws RicercaEspressioneVuotaException, RicercaErroreSintassiException,
            RicercaParentesiException, RicercaParentesiQuadrateException {
        StringBuilder espressione = new StringBuilder(espressioneDaControllare.trim().toLowerCase().replace(' ', ' ').replace('^', '~').replace('!', '|'));
        // il primo spazio è il carattere hexA0 (spazio unificatore), il secondo hex20 (spazio normale)
        if (espressione.length() == 0)
            throw new RicercaEspressioneVuotaException();
        int nParentesiSinistra = 0, nParentesiDestra = 0, nParentesiQuadrateSinistra = 0;
        int erroreSintassi = -1;
        boolean erroreParentesi = false, erroreParentesiQuadrate = false;
        char a, b, c;

        try {
            if (Arrays.asList(getTesto(nomeVersione).getInfo().getLingua().toLowerCase().split("\\|")).indexOf("it") >= 0) {
                int p = 0;
                while (espressione.indexOf("'", p + 1) > -1) {
                    p = espressione.indexOf("'", p + 1);
                    if (p < espressione.length() - 1 && (funzioni.isLettera(espressione.charAt(p + 1)) || espressione.charAt(p + 1) == '*' || espressione.charAt(p + 1) == '?'))
                        espressione = espressione.insert(p + 1, " ");
                }
            }
        } catch (TestoNonEsisteException e) {
            // non fare niente;
        }

        String prossimaParentesiQuadrate = "[";
        while (espressione.indexOf("\"") >= 0) {
            espressione.replace(espressione.indexOf("\""), espressione.indexOf("\"") + 1, prossimaParentesiQuadrate);
            prossimaParentesiQuadrate = (prossimaParentesiQuadrate.equals("[") ? "]" : "[");
        }

        for (int i = 0; i < espressione.length() - 1; ++i) {
            c = espressione.charAt(i);
            if (c == ' ' && i > 1) {
                a = espressione.charAt(i - 1);
                b = espressione.charAt(i + 1);
                if ((a < 'a' && a != '\'' && a != '-' && a != ']' && a != ')' && !(funzioni.isLettera(a) || a == '*' || a == '?')) || a == '~' || a == '|' || a == ':' || a == '<'
                        || a == '>' || (b < 'a' && b != '\'' && b != '-' && b != '(' && b != '[' && !(funzioni.isLettera(b) || b == '*' || b == '?')) || b == '~' || b == '|'
                        || b == ':' || b == '<' || a == '>') {
                    espressione = espressione.deleteCharAt(i);
                    --i;
                }
            }
        }

        for (int i = 0; i < espressione.length(); ++i) {
            c = espressione.charAt(i);
            if (i == 0) {
                if (c == '(')
                    ++nParentesiSinistra;
                else if (c == '[')
                    ++nParentesiQuadrateSinistra;
                else if (c == '<') {
                    int nuovoI = espressione.indexOf(">", i);
                    if (nuovoI > i)
                        i = nuovoI;
                    else
                        erroreSintassi = i;
                } else if (!(funzioni.isLettera(c) || c == '\'' || c == '/' || c == '\\' || c == '*' || c == '?'))
                    erroreSintassi = i;
            } else {
                a = espressione.charAt(i - 1);
                if (c == ' ') {
                    espressione = espressione.deleteCharAt(i);
                    espressione = espressione.insert(i, "0");
                } else if (c == '-' || c == '\'') {
                    if (!(funzioni.isLettera(a) || a == '*' || a == '?'))
                        erroreSintassi = i;
                } else if (c == '/' || c == '\\') {
                    if (a == '/' || a == '\\' || a == '<')
                        erroreSintassi = i;
                    else {
                        if (a != '|' && a != ':' && a != '~' && (!Character.isDigit(a)) && a != '(' && a != '[') {
                            espressione = espressione.insert(i, "0");
                            ++i;
                        }
                    }
                } else if (c == '(') {
                    ++nParentesiSinistra;
                    if (nParentesiQuadrateSinistra > 0 && nParentesiSinistra > 1)
                        erroreParentesiQuadrate = true;
                    if (a == '/' || a == '\\' || a == ':' || a == '<')
                        erroreSintassi = i;
                    else {
                        if (a != '|' && a != '~' && a != '[' && (!Character.isDigit(a))) {
                            espressione = espressione.insert(i, "0");
                            ++i;
                        }
                    }
                } else if (c == ')') {
                    ++nParentesiDestra;
                    if (nParentesiDestra > nParentesiSinistra)
                        erroreParentesi = true;
                    if ((a >= '/' && a <= ':') || a == '|' || a == '~' || a == '\\' || a == '<')
                        erroreSintassi = i;
                } else if (c == '[') {
                    ++nParentesiQuadrateSinistra;
                    if (nParentesiQuadrateSinistra > 1)
                        erroreParentesiQuadrate = true;
                    if (a == '/' || a == '\\' || a == ':' || a == '<')
                        erroreSintassi = i;
                    else {
                        if (a != '|' && a != '~' && (!Character.isDigit(a))) {
                            espressione = espressione.insert(i, "0");
                            ++i;
                        }
                    }
                } else if (c == ']') {
                    nParentesiQuadrateSinistra--;
                    if (nParentesiQuadrateSinistra < 0)
                        erroreParentesiQuadrate = true;
                    if (nParentesiDestra - nParentesiSinistra < 0)
                        erroreParentesi = true;
                    if (Character.isDigit(a) || a == '/' || a == ':' || a == '|' || a == '~' || a == '\\' || a == '<')
                        erroreSintassi = i;
                } else if (c == '|' || Character.isDigit(c)) {
                    if (a != ')' && a != ']' && a != '<' && a != '>' && !(funzioni.isLettera(a) || a == '*' || a == '?'))
                        erroreSintassi = i;
                    if (nParentesiQuadrateSinistra == 1 && c == '|') {
                        b = 'a';
                        int j;
                        for (j = i + 1; b != ']' && (!Character.isDigit(b)) && b != ':' && j < espressione.length(); ++j)
                            b = espressione.charAt(j);
                        espressione = espressione.insert(j - 1, ")");
                        b = 'a';
                        for (j = i - 1; b != '[' && (!Character.isDigit(b)) && b != ':' && j >= 0; --j)
                            b = espressione.charAt(j);
                        espressione = espressione.insert(j + 2, "(");
                        ++i;
                        ++nParentesiSinistra;
                    }
                } else if (c == ':') {
                    if ((a != ')' && a != '>' && !(funzioni.isLettera(a) || a == '*' || a == '?')) || nParentesiQuadrateSinistra == 0)
                        erroreSintassi = i;
                } else if (c == '~') {
                    if (a == '<' || a == '(' || a == '[' || a == ':' || a == '/' || a == '|' || a == '~' || a == '\\'
                            || (nParentesiQuadrateSinistra == 1 && nParentesiSinistra > 0))
                        erroreSintassi = i;
                    else {
                        if (a == ')' || a > ']' || funzioni.isLettera(a) || a == '*' || a == '?') {
                            espressione = espressione.insert(i, "0");
                            ++i;
                        }
                    }
                    if (nParentesiQuadrateSinistra == 1
                            && (funzioni.isLettera(espressione.charAt(i + 1)) || espressione.charAt(i + 1) == '*' || espressione.charAt(i + 1) == '?'
                            || espressione.charAt(i + 1) == '/' || espressione.charAt(i + 1) == '\\')) {
                        espressione = espressione.insert(i + 1, "(");
                        int j;
                        b = 'a';
                        for (j = i + 2; b != ']' && (!Character.isDigit(b)) && b != ':' && j < espressione.length(); ++j)
                            b = espressione.charAt(j);
                        espressione = espressione.insert(j, ")");
                    }
                } else if (c == '>') {
                    // niente da controllare
                } else if (c == '<') {
                    if (a == ')' || a == ']' || a == '>' || funzioni.isLettera(a) || a == '*' || a == '?') {
                        espressione = espressione.insert(i, "0");
                        ++i;
                    }
                    int nuovoI = espressione.indexOf(">", i);
                    if (nuovoI > i)
                        i = nuovoI;
                    else
                        erroreSintassi = i;
                } else if (funzioni.isLettera(c) || c == '*' || c == '?' || c == '<') { // lettera (senza o con accento)
                    if (a == ')' || a == ']' || a == '>') {
                        espressione = espressione.insert(i, "0");
                        ++i;
                    }
                } else
                    // carattere non riconosciuto
                    erroreSintassi = i;
            } // if (i == 0) - else
        } // for (int i = 0; i < espressione.Length; ++i)

        a = espressione.charAt(espressione.length() - 1);
        if (!(a == ')' || a == ']' || a == '-' || a == '\'' || funzioni.isLettera(a) || a == '*' || a == '?' || a == '>'))
            erroreSintassi = espressione.length() - 1;

        if (nParentesiSinistra != nParentesiDestra)
            erroreParentesi = true;
        if (nParentesiQuadrateSinistra == 1)
            erroreParentesiQuadrate = true;
        if (erroreParentesiQuadrate)
            erroreParentesi = false; // indicare solo uno degli errori
        if (erroreSintassi >= 0)
            throw new RicercaErroreSintassiException(Integer.toString(erroreSintassi));
        if (erroreParentesi)
            throw new RicercaParentesiException();
        if (erroreParentesiQuadrate)
            throw new RicercaParentesiQuadrateException();
        return espressione.toString();
    }

    private static Riferimento unisciVociRipetute(Riferimento riferimento) {
        if (riferimento.getVersetti()) {
            int nVersetti = riferimento.getBrani().size();
            for (int i = nVersetti - 1; i > 0; --i) {
                if (riferimento.primoVersettoUguale(i - 1, i)) {
                    riferimento.aggiungiNumeroParola(i - 1, riferimento.getNumeroParola(i));
                    riferimento.rimuoviBrano(i);
                } else
                    riferimento.ordinaParole(i);
            }
            if (nVersetti > 0)
                riferimento.ordinaParole(0);
        } else {
            int nNote = riferimento.getNote().size();
            for (int i = nNote - 1; i > 0; --i) {
                if (riferimento.getNote().get(i - 1).equals(riferimento.getNote().get(i))) {
                    riferimento.aggiungiNumeroParola(i - 1, riferimento.getNumeroParola(i));
                    riferimento.rimuoviNota(i);
                } else
                    riferimento.ordinaParole(i);
            }
            if (nNote > 0)
                riferimento.ordinaParole(0);
        }
        return riferimento;
    }

    private Riferimento trovaOccorrenzeEspressione(String espressione, Riferimento branoDaRicercare, boolean inFrase, int numeroParoleInFrase, String nomeVersione) {
        // se branoDaRicerca non contiene brani, tutta la Bibbia (oppure tutta la collezione di note) è ricercata
        String espressioneDaTrovare = espressione;
        int nParoleInFrase = numeroParoleInFrase;
        espressioneDaTrovare += (char) 0;
        Riferimento riferimenti = new Riferimento();
        String tipoOperazione;
        while (espressioneDaTrovare.length() > 1) { // altrimenti rimane solo char(0)
            char primoCarattere = espressioneDaTrovare.charAt(0);
            if (primoCarattere == '~') {
                primoCarattere = '0';
                espressioneDaTrovare = "0" + espressioneDaTrovare;
            }
            if (Character.isDigit(primoCarattere) || primoCarattere == ':') {
                if (Character.isDigit(espressioneDaTrovare.charAt(1)))
                    tipoOperazione = "prima";
                else {
                    tipoOperazione = espressioneDaTrovare.substring(0, 1);
                    espressioneDaTrovare = espressioneDaTrovare.substring(1);
                    if (espressioneDaTrovare.charAt(0) == '~') {
                        tipoOperazione += "n";
                        espressioneDaTrovare = espressioneDaTrovare.substring(1);
                    }
                }
            } else {
                if (primoCarattere == '|') {
                    tipoOperazione = "oppure";
                    espressioneDaTrovare = espressioneDaTrovare.substring(1);
                } else
                    tipoOperazione = "prima";
            } // if ((IsNumero(cPrimoCarattere)) || cPrimoCarattere==':') else

            int i;
            Riferimento occorrenzeProssimaParola = new Riferimento();
            primoCarattere = espressioneDaTrovare.charAt(0);
            if (primoCarattere == '(') {
                i = 0;
                int nParentesi = 1;
                do {
                    ++i;
                    if (espressioneDaTrovare.charAt(i) == ')')
                        --nParentesi;
                    if (espressioneDaTrovare.charAt(i) == '(')
                        ++nParentesi;
                } while (nParentesi != 0);
                occorrenzeProssimaParola = trovaOccorrenzeEspressione(espressioneDaTrovare.substring(1, i), branoDaRicercare, false, nParoleInFrase, nomeVersione);
                espressioneDaTrovare = espressioneDaTrovare.substring(i + 1);
            } else if (primoCarattere == '[') {
                i = espressioneDaTrovare.indexOf("]");
                nParoleInFrase = 0;
                occorrenzeProssimaParola = trovaOccorrenzeEspressione(espressioneDaTrovare.substring(1, i), branoDaRicercare, true, nParoleInFrase, nomeVersione);
                espressioneDaTrovare = espressioneDaTrovare.substring(i + 1);
            } else {
                String parola = prossimaParola(espressioneDaTrovare, 0);
                try {
                    occorrenzeProssimaParola = getTesto(nomeVersione).ricercaParolaInBrano(parola, branoDaRicercare);
                } catch (TestoNonEsisteException e) {
                    return riferimenti;
                }
                int lunghezzaExtra = (espressioneDaTrovare.charAt(0) == '<' ? 1 : 0);
                if (lunghezzaExtra == 1 && espressioneDaTrovare.contains(">"))
                    ++lunghezzaExtra;
                espressioneDaTrovare = espressioneDaTrovare.substring(parola.length() + lunghezzaExtra);
                ++nParoleInFrase;
            } // if (cPrimoCarattere=='(') else

            Riferimento occorrenzeInBrano = new Riferimento();
            if (tipoOperazione.equals("prima"))
                riferimenti = occorrenzeProssimaParola;
            else {
                primoCarattere = tipoOperazione.charAt(0);
                if (Character.isDigit(primoCarattere) || primoCarattere == ':') {
                    int primoCarattereComeNumero;
                    if (primoCarattere == ':')
                        primoCarattereComeNumero = Integer.MAX_VALUE / 2;
                    else
                        primoCarattereComeNumero = Character.getNumericValue(primoCarattere);
                    if (!inFrase || tipoOperazione.length() == 1) {
                        if (riferimenti.getVersetti()) {
                            int j = i = 1;
                            int nI = riferimenti.count();
                            int nJ = occorrenzeProssimaParola.count();
                            int nVersettoRiferimenti = (nI > 0 ? (versettiFinoACapitolo(riferimenti.getBrani().get(i - 1)[0], riferimenti.getBrani().get(i - 1)[1] - 1,
                                    nomeVersione) + riferimenti.getBrani().get(i - 1)[2]) : 0);
                            int nVersettoOccorrenze = (nJ > 0 ? (versettiFinoACapitolo(occorrenzeProssimaParola.getBrani().get(j - 1)[0],
                                    occorrenzeProssimaParola.getBrani().get(j - 1)[1] - 1, nomeVersione) + occorrenzeProssimaParola.getBrani().get(j - 1)[2]) : 0);
                            while (i <= nI && j <= nJ) {
                                if (inFrase) {
                                    if (nVersettoOccorrenze < nVersettoRiferimenti
                                            || (nVersettoOccorrenze == nVersettoRiferimenti && occorrenzeProssimaParola.getNumeroParola(j - 1).get(0) < riferimenti
                                            .getNumeroParola(i - 1).get(0))) {
                                        ++j;
                                        if (j <= nJ)
                                            nVersettoOccorrenze = versettiFinoACapitolo(occorrenzeProssimaParola.getBrani().get(j - 1)[0],
                                                    occorrenzeProssimaParola.getBrani().get(j - 1)[1] - 1, nomeVersione)
                                                    + occorrenzeProssimaParola.getBrani().get(j - 1)[2];
                                    } else {
                                        if (nVersettoOccorrenze > nVersettoRiferimenti
                                                || (nVersettoOccorrenze == nVersettoRiferimenti && occorrenzeProssimaParola.getNumeroParola(j - 1).get(0) > riferimenti
                                                .getNumeroParola(i - 1).get(0) + primoCarattereComeNumero + 1)) {
                                            if (tipoOperazione.length() > 1) {
                                                occorrenzeInBrano.aggiungiBranoNumeroParola(riferimenti.getBrani().get(i - 1), riferimenti.getNumeroParola(i - 1));
                                            }
                                        } else {
                                            if (tipoOperazione.length() == 1) {
                                                occorrenzeInBrano.aggiungiBranoNumeroParola(riferimenti.getBrani().get(i - 1), riferimenti.getNumeroParola(i - 1));
                                                occorrenzeInBrano.inserisciNumeroParola(occorrenzeInBrano.countNumeroParola() - 1, 0,
                                                        occorrenzeProssimaParola.getNumeroParola(j - 1).get(0));
                                            }
                                        }
                                        ++i;
                                        if (i <= nI)
                                            nVersettoRiferimenti = versettiFinoACapitolo(riferimenti.getBrani().get(i - 1)[0], riferimenti.getBrani().get(i - 1)[1] - 1,
                                                    nomeVersione) + riferimenti.getBrani().get(i - 1)[2];
                                    }
                                } else {
                                    if (nVersettoOccorrenze < nVersettoRiferimenti - primoCarattereComeNumero) {
                                        ++j;
                                        if (j <= nJ)
                                            nVersettoOccorrenze = versettiFinoACapitolo(occorrenzeProssimaParola.getBrani().get(j - 1)[0],
                                                    occorrenzeProssimaParola.getBrani().get(j - 1)[1] - 1, nomeVersione)
                                                    + occorrenzeProssimaParola.getBrani().get(j - 1)[2];
                                    } else {
                                        if (nVersettoOccorrenze > nVersettoRiferimenti + primoCarattereComeNumero) {
                                            if (tipoOperazione.length() > 1) {
                                                occorrenzeInBrano.aggiungiBranoNumeroParola(riferimenti.getBrani().get(i - 1), riferimenti.getNumeroParola(i - 1));
                                            }
                                        } else {
                                            if (tipoOperazione.length() == 1) {
                                                occorrenzeInBrano.aggiungiBranoNumeroParola(riferimenti.getBrani().get(i - 1), riferimenti.getNumeroParola(i - 1));
                                                if (primoCarattereComeNumero == 0) { // seconda parola nel versetto anche, quindi va sottolineata
                                                    occorrenzeInBrano.inserisciNumeroParola(occorrenzeInBrano.countNumeroParola() - 1, 0,
                                                            occorrenzeProssimaParola.getNumeroParola(j - 1).get(0));
                                                    while (j < nJ && occorrenzeProssimaParola.primoVersettoUguale(j - 1, j)) {
                                                        occorrenzeInBrano.aggiungiNumeroParola(occorrenzeInBrano.countNumeroParola() - 1,
                                                                occorrenzeProssimaParola.getNumeroParola(j).get(0));
                                                        ++j;
                                                    }
                                                }
                                            }
                                        }
                                        ++i;
                                        if (i <= nI)
                                            nVersettoRiferimenti = versettiFinoACapitolo(riferimenti.getBrani().get(i - 1)[0], riferimenti.getBrani().get(i - 1)[1] - 1,
                                                    nomeVersione) + riferimenti.getBrani().get(i - 1)[2];
                                    }
                                }
                            } // while (i <= riferimenti.Count && j <= occorrenzeProssimaParola.Count)
                            if (tipoOperazione.length() > 1) {
                                while (i <= riferimenti.count()) {
                                    occorrenzeInBrano.aggiungiBranoNumeroParola(riferimenti.getBrani().get(i - 1), riferimenti.getNumeroParola(i - 1));
                                    ++i;
                                }
                            }
                        } else // if (riferimenti.Versetti)
                        {
                            occorrenzeInBrano.setVersetti(false);
                            int j = i = 1;
                            int nI = riferimenti.count();
                            int nJ = occorrenzeProssimaParola.count();
                            String notaRiferimenti = (nI > 0 ? riferimenti.getNote().get(i - 1) : "");
                            String notaOccorrenze = (nJ > 0 ? occorrenzeProssimaParola.getNote().get(j - 1) : "");
                            int[] differenze = {-1, primoCarattereComeNumero}; // [0] = differenzaVersetti, [1] = differenzaRicercata
                            while (i <= nI && j <= nJ) {
                                if (inFrase) {
                                    if (notaOccorrenze.compareTo(notaRiferimenti) < 0
                                            || (notaOccorrenze.equals(notaRiferimenti) && occorrenzeProssimaParola.getNumeroParola(j - 1).get(0) < riferimenti.getNumeroParola(
                                            i - 1).get(0))) {
                                        ++j;
                                        if (j <= nJ)
                                            notaOccorrenze = occorrenzeProssimaParola.getNote().get(j - 1);
                                    } else {
                                        if (notaOccorrenze.compareTo(notaRiferimenti) > 0
                                                || (notaOccorrenze.equals(notaRiferimenti) && occorrenzeProssimaParola.getNumeroParola(j - 1).get(0) > riferimenti.getNumeroParola(
                                                i - 1).get(0)
                                                + primoCarattereComeNumero + 1)) {
                                            if (tipoOperazione.length() > 1) {
                                                occorrenzeInBrano.aggiungiNotaNumeroParola(riferimenti.getNote().get(i - 1), riferimenti.getNumeroParola(i - 1));
                                            }
                                        } else {
                                            if (tipoOperazione.length() == 1) {
                                                occorrenzeInBrano.aggiungiNotaNumeroParola(riferimenti.getNote().get(i - 1), riferimenti.getNumeroParola(i - 1));
                                                occorrenzeInBrano.inserisciNumeroParola(occorrenzeInBrano.countNumeroParola() - 1, 0,
                                                        occorrenzeProssimaParola.getNumeroParola(j - 1).get(0));
                                            }
                                        }
                                        ++i;
                                        if (i <= nI)
                                            notaRiferimenti = riferimenti.getNote().get(i - 1);
                                    }
                                } else {
                                    differenze = calcolaDifferenzeDelleNote(primoCarattereComeNumero, notaRiferimenti, notaOccorrenze);
                                    if (differenze[0] < -differenze[1]) // cioè string.Compare(notaOccorrenze, notaRiferimenti) < 0 per due note quando una non è ad un brano
                                    {
                                        ++j;
                                        if (j <= nJ)
                                            notaOccorrenze = occorrenzeProssimaParola.getNote().get(j - 1);
                                    } else {
                                        if (differenze[0] > differenze[1]) // cioè string.Compare(notaOccorrenze, notaRiferimenti) > 0 per due note quando una non è ad un brano
                                        {
                                            if (tipoOperazione.length() > 1) {
                                                occorrenzeInBrano.aggiungiNotaNumeroParola(notaRiferimenti, riferimenti.getNumeroParola(i - 1));
                                            }
                                        } else {
                                            if (tipoOperazione.length() == 1) {
                                                if (!occorrenzeInBrano.getNote().isEmpty()
                                                        && notaRiferimenti.equals(occorrenzeInBrano.getNote().get(occorrenzeInBrano.getNote().size() - 1))) {
                                                    occorrenzeInBrano
                                                            .inserisciNumeroParola(occorrenzeInBrano.countNumeroParola() - 1, 0, riferimenti.getNumeroParola(i - 1).get(0));
                                                } else {
                                                    occorrenzeInBrano.aggiungiNotaNumeroParola(notaRiferimenti, riferimenti.getNumeroParola(i - 1));
                                                    if (differenze[1] == 0) // seconda parola nel versetto anche, quindi va sottolineata
                                                        occorrenzeInBrano.inserisciNumeroParola(occorrenzeInBrano.countNumeroParola() - 1, 0, occorrenzeProssimaParola
                                                                .getNumeroParola(j - 1).get(0));
                                                }
                                                ++j;
                                                if (j <= nJ) {
                                                    notaOccorrenze = occorrenzeProssimaParola.getNote().get(j - 1);
                                                    differenze = calcolaDifferenzeDelleNote(primoCarattereComeNumero, notaRiferimenti, notaOccorrenze);
                                                }
                                                while (Math.abs(differenze[0]) <= differenze[1] && j <= nJ) {
                                                    occorrenzeInBrano.inserisciNumeroParola(occorrenzeInBrano.countNumeroParola() - 1, 0,
                                                            occorrenzeProssimaParola.getNumeroParola(j - 1).get(0));
                                                    ++j;
                                                    if (j <= nJ) {
                                                        notaOccorrenze = occorrenzeProssimaParola.getNote().get(j - 1);
                                                        differenze = calcolaDifferenzeDelleNote(primoCarattereComeNumero, notaRiferimenti, notaOccorrenze);
                                                    }
                                                }
                                                while (i < nI && riferimenti.getNote().get(i - 1).equals(riferimenti.getNote().get(i))) {
                                                    ++i;
                                                    occorrenzeInBrano
                                                            .inserisciNumeroParola(occorrenzeInBrano.countNumeroParola() - 1, 0, riferimenti.getNumeroParola(i - 1).get(0));
                                                }
                                            }
                                        }
                                        ++i;
                                        if (i <= nI)
                                            notaRiferimenti = riferimenti.getNote().get(i - 1);
                                    }
                                }
                            } // while (i <= nI && j <= nJ)
                            if (tipoOperazione.length() > 1) {
                                while (i <= riferimenti.count()) {
                                    occorrenzeInBrano.aggiungiNotaNumeroParola(riferimenti.getNote().get(i - 1), riferimenti.getNumeroParola(i - 1));
                                    ++i;
                                }
                            }
                        } // if (riferimenti.Versetti) else
                        riferimenti = occorrenzeInBrano;
                    } // if (!inFrase || tipoOper.Length == 1)
                } // if (Char.IsDigit(primoCarattere)) {
                else {
                    if (primoCarattere == 'o') {
                        int j = i = 1;
                        if (riferimenti.getVersetti()) {
                            if (riferimenti.count() > 0 && occorrenzeProssimaParola.count() > 0) {
                                int nVersettoRiferimenti = versettiFinoACapitolo(riferimenti.getBrani().get(i - 1)[0], riferimenti.getBrani().get(i - 1)[1] - 1, nomeVersione)
                                        + riferimenti.getBrani().get(i - 1)[2];
                                int nVersettoOccorrenze = versettiFinoACapitolo(occorrenzeProssimaParola.getBrani().get(j - 1)[0],
                                        occorrenzeProssimaParola.getBrani().get(j - 1)[1] - 1, nomeVersione) + occorrenzeProssimaParola.getBrani().get(j - 1)[2];
                                int nI = riferimenti.count();
                                int nJ = occorrenzeProssimaParola.count();
                                while (i <= nI && j <= nJ) {
                                    if (nVersettoOccorrenze < nVersettoRiferimenti
                                            || (nVersettoOccorrenze == nVersettoRiferimenti && occorrenzeProssimaParola.getNumeroParola(j - 1).get(0) < riferimenti
                                            .getNumeroParola(i - 1).get(0))) {
                                        occorrenzeInBrano
                                                .aggiungiBranoNumeroParola(occorrenzeProssimaParola.getBrani().get(j - 1), occorrenzeProssimaParola.getNumeroParola(j - 1));
                                        ++j;
                                        if (j <= nJ)
                                            nVersettoOccorrenze = versettiFinoACapitolo(occorrenzeProssimaParola.getBrani().get(j - 1)[0], (occorrenzeProssimaParola.getBrani()
                                                    .get(j - 1)[1] - 1), nomeVersione)
                                                    + occorrenzeProssimaParola.getBrani().get(j - 1)[2];
                                    } else {
                                        occorrenzeInBrano.aggiungiBranoNumeroParola(riferimenti.getBrani().get(i - 1), riferimenti.getNumeroParola(i - 1));
                                        ++i;
                                        if (i <= nI)
                                            nVersettoRiferimenti = versettiFinoACapitolo(riferimenti.getBrani().get(i - 1)[0], riferimenti.getBrani().get(i - 1)[1] - 1,
                                                    nomeVersione) + riferimenti.getBrani().get(i - 1)[2];
                                    }
                                } // while
                            }
                            while (j <= occorrenzeProssimaParola.count()) {
                                occorrenzeInBrano.aggiungiBranoNumeroParola(occorrenzeProssimaParola.getBrani().get(j - 1), occorrenzeProssimaParola.getNumeroParola(j - 1));
                                ++j;
                            }
                            while (i <= riferimenti.count()) {
                                occorrenzeInBrano.aggiungiBranoNumeroParola(riferimenti.getBrani().get(i - 1), riferimenti.getNumeroParola(i - 1));
                                ++i;
                            }
                        } else // collezioni di note
                        {
                            occorrenzeInBrano.setVersetti(false);
                            int nI = riferimenti.count();
                            int nJ = occorrenzeProssimaParola.count();
                            String notaRiferimenti = (nI > 0 ? riferimenti.getNote().get(i - 1) : "");
                            String notaOccorrenze = (nJ > 0 ? occorrenzeProssimaParola.getNote().get(j - 1) : "");
                            while (i <= nI && j <= nJ) {
                                if (notaOccorrenze.compareTo(notaRiferimenti) < 0
                                        || (notaOccorrenze.equals(notaRiferimenti) && occorrenzeProssimaParola.getNumeroParola(j - 1).get(0) < riferimenti.getNumeroParola(i - 1)
                                        .get(0))) {
                                    occorrenzeInBrano.aggiungiNotaNumeroParola(occorrenzeProssimaParola.getNote().get(j - 1), occorrenzeProssimaParola.getNumeroParola(j - 1));
                                    ++j;
                                    if (j < nJ)
                                        notaOccorrenze = occorrenzeProssimaParola.getNote().get(j - 1);
                                } else {
                                    occorrenzeInBrano.aggiungiNotaNumeroParola(riferimenti.getNote().get(i - 1), riferimenti.getNumeroParola(i - 1));
                                    ++i;
                                    if (i < nI)
                                        notaRiferimenti = riferimenti.getNote().get(i - 1);
                                }
                            } // while
                            while (j <= occorrenzeProssimaParola.count()) {
                                occorrenzeInBrano.aggiungiNotaNumeroParola(occorrenzeProssimaParola.getNote().get(j - 1), occorrenzeProssimaParola.getNumeroParola(j - 1));
                                ++j;
                            }
                            while (i <= riferimenti.count()) {
                                occorrenzeInBrano.aggiungiNotaNumeroParola(riferimenti.getNote().get(i - 1), riferimenti.getNumeroParola(i - 1));
                                ++i;
                            }
                        }
                        riferimenti = occorrenzeInBrano;
                    }
                } // if (Char.IsDigit(primoCarattere)) else
            }
        }

        return riferimenti;
    }

    private static String prossimaParola(String fraseRicercata, int inizio) {
        int j = 0;
        StringBuilder prossimaParola = new StringBuilder();
        String frase = fraseRicercata.substring(inizio) + " "; // con " ", la riga c = sFraseRicercata[iInizio+j] funziona anche quando passa oltre la fine di sFraseRicercata
        char c = frase.charAt(0);
        if (c == '<') {
            int p = frase.indexOf(">");
            return (p > 0 ? frase.substring(1, p) : "");
        } else if (Character.isDigit(c)) {
            while (Character.isDigit(c)) {
                prossimaParola.append(c);
                ++j;
                c = frase.charAt(j);
            }
        } else {
            while (funzioni.isLettera(c) || c == '-' || c == '\'' || c == '*' || c == '?' || c == '/' || c == '\\') {
                prossimaParola.append(c);
                ++j;
                c = frase.charAt(j);
            }
        }

        return prossimaParola.toString();
    }

    private int[] calcolaDifferenzeDelleNote(int primoCarattereComeNumero, String notaRiferimenti, String notaOccorrenze) {
        int differenzaVersetti;
        int differenzaRicercata = primoCarattereComeNumero;
        if (notaOccorrenze.startsWith("#") && notaRiferimenti.startsWith("#")) {
            differenzaVersetti = versettiFinoACapitolo(Integer.parseInt(notaOccorrenze.substring(1, 3)), Integer.parseInt(notaOccorrenze.substring(3, 6)))
                    + Integer.parseInt(notaOccorrenze.substring(6, 9))
                    - versettiFinoACapitolo(Integer.parseInt(notaRiferimenti.substring(1, 3)), Integer.parseInt(notaRiferimenti.substring(3, 6)))
                    - Integer.parseInt(notaRiferimenti.substring(6, 9));
        } else {
            differenzaVersetti = notaOccorrenze.compareTo(notaRiferimenti);
            differenzaRicercata = 0;
        }
        return new int[]{differenzaVersetti, differenzaRicercata};
    }

    public Riferimento convertiAStandard(Riferimento riferimento, String nomeVersione) {
        // Converte un riferimento in una versione allo schema standard di riferimenti del programma.
        // riferimento: Il riferimento da convertire.
        // nomeVersione: Il nome della versione.
        // Restituisce: Il riferimento nello schema standard.
        Riferimento rif = new Riferimento(riferimento);
        for (int[] branoDaConvertire : rif.getBrani()) {
            boolean inizioConvertito = false;
            boolean fineConvertita = false;
            try {
                if (branoDaConvertire[5] == 255) {
                    int bdc = versettiInCapitolo(branoDaConvertire[3], branoDaConvertire[4], nomeVersione);
                    if (bdc == 0) {
                        return rif; // se nomeVersione non esiste, restituisce 0 invece di Exception
                        // succede con la liturgia
                    } else {
                        branoDaConvertire[5] = bdc;
                    }
                }
                for (int[] rifDiversi : getTesto(nomeVersione).riferimentiDiversi) {
                    if (!inizioConvertito && branoDaConvertire[0] == rifDiversi[3] && branoDaConvertire[1] == rifDiversi[4]
                            && (branoDaConvertire[2] == rifDiversi[5] || rifDiversi[5] <= 0)) {
                        branoDaConvertire[0] = rifDiversi[0];
                        branoDaConvertire[1] = rifDiversi[1];
                        inizioConvertito = true;
                        if (rifDiversi[5] > 0)
                            branoDaConvertire[2] = rifDiversi[2];
                        else if (rifDiversi[5] == 0) { // fare la stessa cosa a tutti i versetti nel capitolo: cambiare il capitolo e/o sottrarre un numero da ogni versetto
                            if (rifDiversi[2] < 0)
                                branoDaConvertire[2] = branoDaConvertire[2] + rifDiversi[2];
                        } else
                            // <0 ==> bisogna aggiungere il numero di versetti
                            branoDaConvertire[2] = branoDaConvertire[2] - rifDiversi[5];
                    }
                    if (!fineConvertita && branoDaConvertire[3] == rifDiversi[3] && branoDaConvertire[4] == rifDiversi[4]
                            && (branoDaConvertire[5] == rifDiversi[5] || rifDiversi[5] <= 0 || branoDaConvertire[5] == 255)) {
                        branoDaConvertire[3] = rifDiversi[0];
                        branoDaConvertire[4] = rifDiversi[1];
                        //if (branoDaConvertire[5]!=255)
                        fineConvertita = true;
                        if (rifDiversi[5] > 0) {
                            if (branoDaConvertire[5] != 255) {
                                branoDaConvertire[5] = rifDiversi[2];
                            }
                        } else {
                            if (branoDaConvertire[5] != 255) {
                                if (rifDiversi[5] == 0) {
                                    if (rifDiversi[2] < 0)
                                        branoDaConvertire[5] = branoDaConvertire[5] + rifDiversi[5];
                                } else {
                                    branoDaConvertire[5] = branoDaConvertire[5] - rifDiversi[5];
                                }
                            }
                        }
                    }
                }
            } catch (TestoNonEsisteException e) {
                return rif; // "nomeVersione" non esiste; non cambiamo il riferimento
            }
        }
        rif.setDaTradurre(true);
        return rif;
    }

    public Riferimento convertiDaStandard(Riferimento riferimento, String nomeVersione) {
        // Converte un riferimento nello schema standard di riferimenti del programma al riferimento in una versione della Bibbia.
        // riferimento: Il riferimento da convertire.
        // nomeVersione: Il nome della versione.
        // Restituisce: Il riferimento nello schema della versione.
        Riferimento rif = new Riferimento(riferimento);
        for (int[] branoDaConvertire : rif.getBrani()) {
            boolean inizioConvertito = false;
            boolean fineConvertita = false;
            try {
                for (int[] rifDiversi : getTesto(nomeVersione).riferimentiDiversi) {
                    if (!inizioConvertito && branoDaConvertire[0] == rifDiversi[0] && branoDaConvertire[1] == rifDiversi[1]
                            && (branoDaConvertire[2] == rifDiversi[2] || rifDiversi[2] <= 0)) {
                        branoDaConvertire[0] = rifDiversi[3];
                        branoDaConvertire[1] = rifDiversi[4];
                        inizioConvertito = true;
                        if (rifDiversi[2] > 0)
                            branoDaConvertire[2] = rifDiversi[5];
                        else if (rifDiversi[2] == 0) // fare la stessa cosa a tutti i versetti nel capitolo: cambiare il capitolo e/o sottrarre un numero da ogni versetto
                        {
                            if (rifDiversi[5] < 0)
                                branoDaConvertire[2] = branoDaConvertire[2] + rifDiversi[5];
                        } else
                            // <0 ==> bisogna aggiungere il numero di versetti
                            branoDaConvertire[2] = branoDaConvertire[2] - rifDiversi[2];
                    }
                    if (!fineConvertita && branoDaConvertire[3] == rifDiversi[0] && branoDaConvertire[4] == rifDiversi[1]
                            && (branoDaConvertire[5] == rifDiversi[2] || rifDiversi[2] <= 0)) {
                        branoDaConvertire[3] = rifDiversi[3];
                        branoDaConvertire[4] = rifDiversi[4];
                        fineConvertita = true;
                        if (rifDiversi[2] > 0)
                            branoDaConvertire[5] = rifDiversi[5];
                        else {
                            if (branoDaConvertire[5] != 255) {
                                if (rifDiversi[2] == 0) {
                                    if (rifDiversi[5] < 0)
                                        branoDaConvertire[5] = branoDaConvertire[5] + rifDiversi[5];
                                } else {
                                    branoDaConvertire[5] = branoDaConvertire[5] - rifDiversi[2];
                                }
                            }
                        }
                    }
                }
            } catch (
                    TestoNonEsisteException e) { // "nomeVersione" non esiste; non cambiamo il riferimento
                return rif;
            }
        }
        rif.setDaTradurre(false);
        return rif;
    }

    public String convertiRiferimentoDa3Numeri(String riferimentoDaConvertire) {
        // Converte un riferimento nel formato "1 28:14; 4 24:17" a "Genesi 28:14; Numeri 24:17".
        // riferimentoDaConvertire: Il riferimento da convertire
        // restituisce: Il riferimento convertito
        StringBuilder riferimentoConvertito = new StringBuilder();
        if (!riferimentoDaConvertire.isEmpty()) {
            String rifDaConvertire = riferimentoDaConvertire;
            rifDaConvertire = ";" + rifDaConvertire + ";";
            rifDaConvertire = rifDaConvertire.replace("; ", ";");
            rifDaConvertire = rifDaConvertire.substring(1);
            while (!rifDaConvertire.isEmpty()) {
                int posizioneSpazio = rifDaConvertire.indexOf(" ");
                int posizionePuntoVirgola = rifDaConvertire.indexOf(";");
                if (posizionePuntoVirgola == -1)
                    rifDaConvertire = "";
                else {
                    if (posizioneSpazio >= 0)
                        riferimentoConvertito.append(libriNomi[Integer.parseInt(rifDaConvertire.substring(0, posizioneSpazio))])
                                .append(rifDaConvertire.substring(posizioneSpazio, posizionePuntoVirgola)).append("; ");
                    rifDaConvertire = rifDaConvertire.substring(posizionePuntoVirgola + 1);
                }
            }
        }
        String riferimentoStringa = riferimentoConvertito.toString().trim();
        if (riferimentoStringa.endsWith(";"))
            riferimentoStringa = riferimentoStringa.substring(0, riferimentoStringa.length() - 1);
        return riferimentoStringa;
    }

    public Riferimento convertiRiferimento(String riferimento) {
        Riferimento nuovoRiferimento = new Riferimento();
        String riferimentoModificato = riferimento.trim().toLowerCase();
        if (riferimentoModificato.isEmpty()) {
            return nuovoRiferimento;
        }

        // cancellare eventuali spazi dopo punteggiatura o un numero (per esempio 2 re)
        for (int i = riferimentoModificato.length() - 1; i >= 1; --i) {
            if (riferimentoModificato.charAt(i) == ' '
                    && (riferimentoModificato.charAt(i - 1) == ':' || riferimentoModificato.charAt(i - 1) == ',' || riferimentoModificato.charAt(i - 1) == '.'
                    || riferimentoModificato.charAt(i - 1) == ';' || riferimentoModificato.charAt(i - 1) == '-' || Character.isDigit(riferimentoModificato.charAt(i - 1)))) {
                riferimentoModificato = funzioni.rimuovi(riferimentoModificato, i, 1);
            }
        }
        // cancellare eventuali punti o virgole dopo il nome di un libro
        // (virgole succede con RIFTIPO_CITAZIONE)
        for (int i = riferimentoModificato.length() - 1; i >= 1; --i) {
            if ((riferimentoModificato.charAt(i) == '.') && (Character.isLetter(riferimentoModificato.charAt(i - 1)))) {
                riferimentoModificato = funzioni.rimuovi(riferimentoModificato, i, 1);
            } else {
                if ((riferimentoModificato.charAt(i) == ',') && (Character.isLetter(riferimentoModificato.charAt(i - 1)))) {
                    if (i == riferimentoModificato.length() - 1 || (Character.isDigit(riferimentoModificato.charAt(i + 1)))
                            && (i == riferimentoModificato.length() - 2 || !Character.isLetter(riferimentoModificato.charAt(i + 2)))) // non caso di mr,gv o mr,3g ma sì nel caso di mr,3,4
                    {
                        riferimentoModificato = funzioni.rimuovi(riferimentoModificato, i, 1);
                    }
                }
            }
        }
        // cancellare eventuali due punti alla fine o prima di punteggiatura (possibile con RIFTIPO_CITAZIONE)
        for (int i = riferimentoModificato.length() - 1; i >= 1; --i) {
            if (riferimentoModificato.charAt(i) == ':'
                    && (i == riferimentoModificato.length() - 1 || (riferimentoModificato.charAt(i + 1) == ';' || riferimentoModificato.charAt(i + 1) == ',' || riferimentoModificato
                    .charAt(i + 1) == '.'))) {
                riferimentoModificato = funzioni.rimuovi(riferimentoModificato, i, 1);
            }
        }

        if ((formato.getRiferimentoTipo() == RiferimentoTipo.VIRGOLA || formato.getRiferimentoTipo() == RiferimentoTipo.CITAZIONE)
                && (riferimentoModificato.indexOf(':') < 0 || riferimentoModificato.indexOf(':') >= riferimentoModificato.length() - 2)) {
            riferimentoModificato = riferimentoModificato.replace(',', ':');
            riferimentoModificato = riferimentoModificato.replace('.', ',');
            while (riferimentoModificato.indexOf(';') >= 0) {
                int dopoDivisore = riferimentoModificato.indexOf(';') + 1;
                // controlla situazioni come Is 7,1-10;12 che viene tradotto in modo diverso
                while (dopoDivisore <= riferimentoModificato.length() - 1
                        && ((Character.isDigit(riferimentoModificato.charAt(dopoDivisore))) || riferimentoModificato.charAt(dopoDivisore) == ' ')) {
                    ++dopoDivisore;
                }
                if (dopoDivisore > riferimentoModificato.length() - 1
                        || (riferimentoModificato.charAt(dopoDivisore) != ':' && riferimentoModificato.charAt(dopoDivisore) != '.' && (!Character.isLetter(riferimentoModificato
                        .charAt(dopoDivisore))))) {
                    riferimentoModificato = riferimentoModificato.substring(1, dopoDivisore) + ":1-200" + riferimentoModificato.substring(dopoDivisore);
                }
                riferimentoModificato = riferimentoModificato.replace(';', ',');
            }
        }

        int punteggiature, capitolo = 0;
        boolean trattinoVecchio = true, trattino = false, versettoMancante = false;
        String riferimentoDaAnalizzare, libroNome = "";
        int[] riferimentoBrano = {0, 0, 0, 0, 0, 0, 0, 0};
        int[] riferimentoBranoPrecedente = {0, 0, 0, 0, 0, 0, 0, 0};
        int[] riferimentoBrano4Byte = {0, 0, 0, 0, 0, 0, 0, 0};
        do {
            // troviamo il riferimento del primo brano, cioè fino alla prima
            // punteggiatura
            punteggiature = riferimentoModificato.indexOf(',');
            if (punteggiature < 0 || (riferimentoModificato.indexOf(';') < punteggiature && riferimentoModificato.indexOf(';') >= 0)) {
                punteggiature = riferimentoModificato.indexOf(';');
            }
            if (punteggiature < 0 || (riferimentoModificato.indexOf('-') < punteggiature && riferimentoModificato.indexOf('-') >= 0)) {
                punteggiature = riferimentoModificato.indexOf('-');
                if (punteggiature >= 0) {
                    trattino = true;
                }
            }
            if (punteggiature >= 0) {
                riferimentoDaAnalizzare = riferimentoModificato.substring(0, punteggiature);
                // il riferimento del primo brano
                riferimentoModificato = funzioni.rimuovi(riferimentoModificato, 0, punteggiature + 1).trim();
                // il resto del riferimento, che analizzeremo più tardi
            } else {
                riferimentoDaAnalizzare = riferimentoModificato;
                riferimentoModificato = "";
            }
            riferimentoBrano = convertiRiferimentoDaTestoA4Byte(riferimentoDaAnalizzare, trattinoVecchio);
            // il primo brano, in formatto a 4 byte
            if (riferimentoBrano[0] == 0 && !riferimentoDaAnalizzare.isEmpty() && (!Character.isLetter(riferimentoDaAnalizzare.charAt(0)))) {
                if (riferimentoDaAnalizzare.indexOf(':') == -1 && riferimentoDaAnalizzare.indexOf('.') == -1 && !versettoMancante) {
                    riferimentoDaAnalizzare = capitolo + ":" + riferimentoDaAnalizzare;
                }
                riferimentoDaAnalizzare = libroNome + riferimentoDaAnalizzare;
                riferimentoBrano = convertiRiferimentoDaTestoA4Byte(riferimentoDaAnalizzare, trattinoVecchio);
            }
            versettoMancante = false;
            if (riferimentoBrano[0] > 0) {
                riferimentoBrano4Byte = riferimentoBrano;
                if (riferimentoDaAnalizzare.indexOf(':') == -1 && riferimentoDaAnalizzare.indexOf('.') == -1) {
                    versettoMancante = true;
                    if (trattino) {
                        if (!riferimentoModificato.isEmpty() && (!Character.isLetter(riferimentoModificato.charAt(0)))
                                && (riferimentoModificato.length() == 1 || (!Character.isLetter(riferimentoModificato.charAt(1))))) {
                            riferimentoModificato = libriAbbreviazioniUsate[riferimentoBrano4Byte[0]] + riferimentoModificato;
                        }
                    } else {
                        if (trattinoVecchio) {
                            trattino = true;
                            riferimentoModificato = riferimentoDaAnalizzare + ";" + riferimentoModificato;
                        }
                    }
                }
                libroNome = libriAbbreviazioniUsate[riferimentoBrano4Byte[0]];
                capitolo = riferimentoBrano4Byte[1];
            }
            if (!trattinoVecchio) {
                riferimentoBrano[4] = riferimentoBrano[0];
                riferimentoBrano[5] = riferimentoBrano[1];
                riferimentoBrano[6] = riferimentoBrano[2];
                riferimentoBrano[7] = riferimentoBrano[3];
                riferimentoBrano[0] = riferimentoBranoPrecedente[0];
                riferimentoBrano[1] = riferimentoBranoPrecedente[1];
                riferimentoBrano[2] = riferimentoBranoPrecedente[2];
                riferimentoBrano[3] = riferimentoBranoPrecedente[3];
                trattinoVecchio = true;
            } else {
                if (trattino) {
                    trattinoVecchio = false;
                    trattino = false;
                    riferimentoBranoPrecedente[0] = riferimentoBrano[0];
                    riferimentoBranoPrecedente[1] = riferimentoBrano[1];
                    riferimentoBranoPrecedente[2] = riferimentoBrano[2];
                    riferimentoBranoPrecedente[3] = riferimentoBrano[3];
                } else {
                    riferimentoBrano[4] = riferimentoBrano[0];
                    riferimentoBrano[5] = riferimentoBrano[1];
                    riferimentoBrano[6] = riferimentoBrano[2];
                    riferimentoBrano[7] = riferimentoBrano[3];
                }
            }
            if (riferimentoBrano[0] > 0 && riferimentoBrano[4] > 0) {
                nuovoRiferimento.aggiungiBrano8Int(riferimentoBrano);
            }
        } while (!riferimentoModificato.isEmpty());
        return nuovoRiferimento;
    }

    private int[] convertiRiferimentoDaTestoA4Byte(String riferimentoTestuale, boolean primaDelTrattino) {
        // convertire a 4 interi un riferimento di un versetto+parola
        // se primaDelTrattino = false, il riferimento va dopo il trattino
        int[] riferimentoRestituito = {0, 0, 0, 0, 0, 0, 0, 0};
        int primaNonLettera = -1;
        String riferimento = riferimentoTestuale.toLowerCase().trim();
        if (riferimento.isEmpty()) {
            return riferimentoRestituito;
        }

        String nomeLibro = "";
        if (riferimento.charAt(0) >= '1' && riferimento.charAt(0) <= '3') {
            nomeLibro = riferimento.substring(0, 1);
            riferimento = funzioni.rimuovi(riferimento, 0, 1).trim();
        }

        do {
            ++primaNonLettera;
        } while (primaNonLettera < riferimento.length() - 1 && Character.isLetter(riferimento.charAt(primaNonLettera)));

        String riferimentoRimanente = "";
        int capitolo = 0, versetto = 0, parola = 0;
        if (primaNonLettera == riferimento.length() - 1 && Character.isLetter(riferimento.charAt(riferimento.length() - 1))) {
            nomeLibro += riferimento;
        } else {
            nomeLibro += riferimento.substring(0, primaNonLettera);
            riferimentoRimanente = riferimento.substring(primaNonLettera).trim();
            StringBuilder capitoloNumerico = new StringBuilder();
            for (int j = 0; j < riferimentoRimanente.length() && Character.isDigit(riferimentoRimanente.charAt(j)); ++j) {
                capitoloNumerico.append(riferimentoRimanente.charAt(j));
            }
            try {
                capitolo = Integer.parseInt(capitoloNumerico.toString());
            } catch (NumberFormatException e) {
                capitolo = 0;
            }
        }

        if (!riferimentoRimanente.isEmpty()) {
            int posDivisoreCapitoloVersetto = getPosDivisoreCapitoloVersetto(riferimentoRimanente);
            if (posDivisoreCapitoloVersetto >= 0) {
                riferimentoRimanente = funzioni.rimuovi(riferimentoRimanente, 0, posDivisoreCapitoloVersetto + 1).trim();
            } else {
                riferimentoRimanente = "";
            }
            StringBuilder versettoNumerico = new StringBuilder();
            for (int j = 0; j < riferimentoRimanente.length() && Character.isDigit(riferimentoRimanente.charAt(j)); ++j) {
                versettoNumerico.append(riferimentoRimanente.charAt(j));
            }
            try {
                versetto = Integer.parseInt(versettoNumerico.toString());
            } catch (NumberFormatException e) {
                versetto = 0;
            }
        }

        // trovare eventuale parola dopo /
        if (!riferimentoRimanente.isEmpty()) {
            int posDivisoreVersettoParola = riferimentoRimanente.indexOf("/");
            if (posDivisoreVersettoParola >= 0) {
                riferimentoRimanente = funzioni.rimuovi(riferimentoRimanente, 0, posDivisoreVersettoParola + 1).trim();
            } else {
                riferimentoRimanente = "";
            }
            StringBuilder parolaNumerico = new StringBuilder();
            for (int j = 0; j < riferimentoRimanente.length() && Character.isDigit(riferimentoRimanente.charAt(j)); ++j) {
                parolaNumerico.append(riferimentoRimanente.charAt(j));
            }
            try {
                parola = Integer.parseInt(parolaNumerico.toString());
            } catch (NumberFormatException e) {
                parola = 0;
            }
        }

        int libro = getLibroNumeroDaAbbreviazione(nomeLibro);

        if (libro > 0) {
            riferimentoRestituito[0] = libro;
            if ((libro == 38 || libro == 64 || libro == 70 || libro == 71 || libro == 72) && versetto == 0) {
                versetto = capitolo;
                capitolo = 1;
            }
            if (capitolo == 0) {
                if (primaDelTrattino) {
                    riferimentoRestituito[1] = 1;
                    riferimentoRestituito[2] = 1;
                } else {
                    riferimentoRestituito[1] = 255;
                    riferimentoRestituito[2] = 255;
                }
            } // if (iCapitolo==0)
            else {
                riferimentoRestituito[1] = capitolo;
                if (versetto == 0) {
                    if (primaDelTrattino) {
                        versetto = 1;
                    } else {
                        versetto = 255;
                    }
                }
                riferimentoRestituito[2] = versetto;
            }
        } // if (!string.IsNullOrEmpty(rifOut))

        riferimentoRestituito[3] = parola;

        return riferimentoRestituito;
    }

    private int getPosDivisoreCapitoloVersetto(String riferimentoRimanente) {
        int posDivisoreCapitoloVersetto = riferimentoRimanente.indexOf(":");
        if (posDivisoreCapitoloVersetto == -1 || (riferimentoRimanente.indexOf(".") < posDivisoreCapitoloVersetto && riferimentoRimanente.contains("."))) {
            posDivisoreCapitoloVersetto = riferimentoRimanente.indexOf(".");
        }
        if ((formato.getRiferimentoTipo() == RiferimentoTipo.VIRGOLA || formato.getRiferimentoTipo() == RiferimentoTipo.CITAZIONE)
                && (posDivisoreCapitoloVersetto == -1 || (riferimentoRimanente.indexOf(",") < posDivisoreCapitoloVersetto && riferimentoRimanente.contains(",")))) {
            posDivisoreCapitoloVersetto = riferimentoRimanente.indexOf(",");
        }
        return posDivisoreCapitoloVersetto;
    }

    // Converte un segnalibro ad un formato testuale più bello.
    // segnalibro: Il riferimento del segnalibro.
    // Restituisce: Il riferimento convertito.
    public String normalizzaRiferimentoSegnalibro(String segnalibro) {
        if (segnalibro.isEmpty())
            return "";
        StringBuilder riferimento = new StringBuilder();
        String[] brani = segnalibro.split(";");
        for (String brano : brani) {
            String[] numeri = brano.split(" ");
            if (numeri.length >= 6)
                riferimento.append(normalizzaRiferimento(numeri[0], numeri[1], numeri[2], numeri[3], numeri[4], numeri[5])).append(";");
            else if (numeri.length >= 3)
                riferimento.append(normalizzaRiferimento(numeri[0], numeri[1], numeri[2])).append(";");
        }
        String riferimentoNormalizzato = riferimento.toString();
        if (riferimentoNormalizzato.endsWith(";"))
            riferimentoNormalizzato = riferimentoNormalizzato.substring(0, riferimentoNormalizzato.length() - 1);
        return riferimentoNormalizzato;
    }

    // Converte un oggetto Riferimento al formato dei riferimenti usati dai segnalibri
    // riferimento: il riferimento da convertire
    // Restituisce: il testo del riferimento del segnalibro
    public static String creaRiferimentoSegnalibro(Riferimento riferimento) {
        StringBuilder riferimentoSegnalibro = new StringBuilder();
        for (int[] brano : riferimento.getBrani())
            riferimentoSegnalibro.append(brano[0]).append(" ").append(brano[1]).append(brano[2]).append(brano[3]).append(brano[4]).append(brano[5]).append(";");
        String riferimentoStringa = riferimentoSegnalibro.toString();
        if (riferimentoStringa.endsWith(";"))
            riferimentoStringa = riferimentoStringa.substring(0, riferimentoSegnalibro.length());
        return riferimentoStringa;
    }

    // TODO non ancora necessario
    // Trova tutti i riferimenti in una stringa.
    // stringaDaAnalizzare: La stringa in cui cercare i riferimenti.
    // Restituisce: I riferimenti trovati, nel formato usato dal programma.
    /*
     * public Riferimento convertiRiferimenti(String stringaDaAnalizzare) { String riferimentoTrovato = ""; if (!stringaDaAnalizzare.equals("")) { char[] numeri = new char[] { '0',
     * '1', '2', '3', '4', '5', '6', '7', '8', '9' }; int indice = stringaDaAnalizzare.indexOfAny(numeri, 1); int primaLetteraDopo, primaLetteraPrima; while (indice > 0) {
     * primaLetteraDopo = indice + 1; while (primaLetteraDopo < stringaDaAnalizzare.length() && !Char.IsLetter(stringaDaAnalizzare[primaLetteraDopo])) ++primaLetteraDopo;
     * primaLetteraPrima = indice - 1; // while (primaLetteraPrima > 0 && !Char.IsLetter(s[primaLetteraPrima])) if (Char.IsWhiteSpace(stringaDaAnalizzare[primaLetteraPrima])) {
     * while (primaLetteraPrima > 0 && Char.IsWhiteSpace(stringaDaAnalizzare[primaLetteraPrima])) --primaLetteraPrima; // adesso andiamo all'inizio di questa parola while
     * (primaLetteraPrima > 0 && Char.IsLetter(stringaDaAnalizzare[primaLetteraPrima - 1])) --primaLetteraPrima; // aggiustiamo per 1Giovanni eccetera if (primaLetteraPrima > 0 &&
     * (stringaDaAnalizzare[primaLetteraPrima - 1] >= '1' && stringaDaAnalizzare[primaLetteraPrima - 1] <= '3')) --primaLetteraPrima; if (primaLetteraPrima > 1 &&
     * char.IsWhiteSpace(stringaDaAnalizzare[primaLetteraPrima - 1]) && (stringaDaAnalizzare[primaLetteraPrima - 2] >= '1' && stringaDaAnalizzare[primaLetteraPrima - 2] <= '3'))
     * primaLetteraPrima -= 2; riferimentoTrovato += stringaDaAnalizzare.Substring(primaLetteraPrima, primaLetteraDopo - primaLetteraPrima) + ";"; } indice = (primaLetteraDopo ==
     * stringaDaAnalizzare.Length ? -1 : stringaDaAnalizzare.IndexOfAny(numeri, primaLetteraDopo)); } } Riferimento riferimento = ConvertiRiferimento(riferimentoTrovato); for (int
     * i = riferimento.Count - 1; i >= 0; --i) { if (riferimento.Brani[i][4] == 255 && riferimento.Brani[i][5] == 255) riferimento.Rimuovi(i); } return riferimento; }
     */

    // Converte un riferimento in formato testuale ad uno più bello. Usa le abbreviazioni dei libri.
    // riferimento: Il riferimento da convertire.
    // restituisce: Il riferimento convertito.
    public String normalizzaRiferimento(String riferimento) {
        return normalizzaRiferimento(riferimento, RiferimentoFormato.ABBREVIAZIONE);
    }

    // Converte un riferimento in formato testuale ad uno più bello. Usa le abbreviazioni dei libri.
    // riferimento: Il riferimento da convertire.
    // formatoDelRiferimento: Il formato del riferimento da visualizzare.
    // restituisce: Il riferimento convertito.
    public String normalizzaRiferimento(String riferimento, RiferimentoFormato formatoDelRiferimento) {
        return normalizzaRiferimento(convertiRiferimento(riferimento), formatoDelRiferimento);
    }

    // Converte un riferimento in formato testuale ad uno più bello. Usa le abbreviazioni dei libri.
    // riferimento: Il riferimento da convertire.
    // restituisce: Il riferimento convertito.
    public String normalizzaRiferimento(Riferimento riferimento) {
        return normalizzaRiferimento(riferimento, RiferimentoFormato.ABBREVIAZIONE);
    }

    // Converte un riferimento in formato testuale ad uno più bello. Usa le abbreviazioni dei libri.
    // riferimento: Il riferimento da convertire.
    // formatoDelRiferimento: Il formato del riferimento da visualizzare.
    // restituisce: Il riferimento convertito.
    public String normalizzaRiferimento(Riferimento riferimento, RiferimentoFormato formatoDelRiferimento) {
        StringBuilder riferimentoNormalizzato = new StringBuilder();
        String[] separatori = separatoriNeiRiferimenti();

        if (formatoDelRiferimento != RiferimentoFormato.NESSUNO && riferimento.getVersetti()) { // se è un riferimento con note, restituisce niente
            String riferimentoTestuale;
            int sLibroVecchio = 0;
            int sCapitoloVecchio = 0;
            int nRiferimenti = riferimento.count();
            for (int i = 0; i < nRiferimenti; ++i) {
                riferimentoTestuale = convertiRiferimentoDa3IntATesto(riferimento.getBrani().get(i), formatoDelRiferimento);
                if (riferimentoTestuale.endsWith(":")) // se RifTipo==RIFTIPO_CITAZIONE
                    riferimentoTestuale = riferimentoTestuale.substring(0, riferimentoTestuale.length() - 1);
                if (!riferimentoNormalizzato.toString().isEmpty()) {
                    if (riferimento.getBrani().get(i)[0] == sLibroVecchio && riferimento.getBrani().get(i)[1] == sCapitoloVecchio
                            && riferimento.getBrani().get(i)[0] == riferimento.getBrani().get(i)[3] && riferimento.getBrani().get(i)[1] == riferimento.getBrani().get(i)[4]) {
                        riferimentoTestuale = riferimentoTestuale.substring(riferimentoTestuale.indexOf(" ") + 1);
                        riferimentoTestuale = riferimentoTestuale.substring(riferimentoTestuale.indexOf(separatori[1]) + 1);
                        riferimentoNormalizzato.append(separatori[2]);
                    } else {
                        riferimentoNormalizzato.append("; ");
                        if (riferimento.getBrani().get(i)[0] == sLibroVecchio && riferimento.getBrani().get(i)[0] == riferimento.getBrani().get(i)[3])
                            riferimentoTestuale = riferimentoTestuale.substring(riferimentoTestuale.indexOf(" ") + 1);
                    }
                }
                riferimentoNormalizzato.append(riferimentoTestuale);
                sLibroVecchio = 0;
                if (riferimento.getBrani().get(i)[0] == riferimento.getBrani().get(i)[3])
                    sLibroVecchio = riferimento.getBrani().get(i)[3];
                sCapitoloVecchio = 0;
                if (riferimento.getBrani().get(i)[0] == riferimento.getBrani().get(i)[3] && riferimento.getBrani().get(i)[1] == riferimento.getBrani().get(i)[4])
                    sCapitoloVecchio = riferimento.getBrani().get(i)[4];
            }
        }

        if (formato.getRiferimentoTipo() == RiferimentoTipo.CITAZIONE && !riferimentoNormalizzato.toString().isEmpty())
            riferimentoNormalizzato.append(":");

        return riferimentoNormalizzato.toString();
    }

    // Converte un riferimento di un brano (libro, capitolo, versetto) ad un formato più bello.
    // libroInizio: Il numero del libro dell'inizio del brano.
    // capitoloInizio: Il capitolo dell'inizio del brano.
    // versettoInizio: Il versetto dell'inizio del brano.
    // libroFine: Il numero del libro della fine del brano.
    // capitoloFine: Il capitolo della fine del brano.
    // versettoFine: Il versetto della fine del brano.
    // Restituisce: Il riferimento convertito.
    public String normalizzaRiferimento(int libroInizio, int capitoloInizio, int versettoInizio, int libroFine, int capitoloFine, int versettoFine) {
        return normalizzaRiferimento(new Riferimento(new int[]{libroInizio, capitoloInizio, versettoInizio, libroFine, capitoloFine, versettoFine}));
    }

    // Converte un riferimento (libro, capitolo, versetto) ad un formato più bello.
    // libro: Il numero del libro.
    // capitolo: Il capitolo.
    // versetto: Il versetto.
    // Restituisce: Il riferimento convertito.
    public String normalizzaRiferimento(int libro, int capitolo, int versetto) {
        return normalizzaRiferimento(new Riferimento(libro, capitolo, versetto));
    }

    // Converte un riferimento (libro, capitolo, versetto) ad un formato più bello.
    // libro: Il numero del libro, come stringa.
    // capitolo: Il capitolo, come stringa.
    // versetto: Il versetto, come stringa.
    // Restituisce: Il riferimento convertito.
    public String normalizzaRiferimento(String libro, String capitolo, String versetto) {
        return normalizzaRiferimento(Integer.parseInt(libro), Integer.parseInt(capitolo), Integer.parseInt(versetto));
    }

    // Converte un riferimento di un brano (libro, capitolo, versetto) ad un formato più bello.
    // libroInizio: Il numero del libro dell'inizio del brano, come stringa.
    // capitoloInizio: Il capitolo dell'inizio del brano, come stringa.
    // versettoInizio: Il versetto dell'inizio del brano, come stringa.
    // libroFine: Il numero del libro della fine del brano, come stringa.
    // capitoloFine: Il capitolo della fine del brano, come stringa.
    // versettoFine: Il versetto della fine del brano, come stringa.
    // Restituisce: Il riferimento convertito.
    public String normalizzaRiferimento(String libroInizio, String capitoloInizio, String versettoInizio, String libroFine, String capitoloFine, String versettoFine) {
        return normalizzaRiferimento(Integer.parseInt(libroInizio), Integer.parseInt(capitoloInizio), Integer.parseInt(versettoInizio), Integer.parseInt(libroFine),
                Integer.parseInt(capitoloFine), Integer.parseInt(versettoFine));
    }

    private int getLibroNumeroDaAbbreviazione(String abbreviazione) {
        if (!abbreviazione.isEmpty()) {
            String abbreviazioneLC = abbreviazione.toLowerCase();
            for (int numeroLettere = abbreviazioneLC.length(); numeroLettere > 0; --numeroLettere) {
                if (libriAbbreviazioniRiconosciute.ContainsKey(abbreviazioneLC.substring(0, numeroLettere))) {
                    return libriAbbreviazioniRiconosciute.get(abbreviazioneLC.substring(0, numeroLettere));
                }
            }
        }
        return 0;
    }

    private String convertiRiferimentoDa3IntATesto(int[] rif, RiferimentoFormato rf) {
        if (rf == RiferimentoFormato.NESSUNO)
            return "";

        String riferimentoTestuale = "";
        int libro1 = rif[0];
        int capitolo1 = rif[1];
        int versetto1 = rif[2];
        int libro2 = rif[3];
        int capitolo2 = rif[4];
        int versetto2 = rif[5];

        String dopoLibro = (formato.getRiferimentoTipo() == RiferimentoTipo.CITAZIONE ? "., " : " ");
        if (rf == RiferimentoFormato.INTERO)
            riferimentoTestuale = libriNomi[libro1] + dopoLibro;
        else if (rf == RiferimentoFormato.ABBREVIAZIONE)
            riferimentoTestuale = libriAbbreviazioniUsate[libro1] + dopoLibro;
        else if (rf == RiferimentoFormato.ABBREVIAZIONE_RICONOSCIUTA) {
            riferimentoTestuale = libriAbbreviazioniRiconosciute.Abbreviazione(libro1);
            if (!riferimentoTestuale.contains(","))
                riferimentoTestuale = riferimentoTestuale + dopoLibro;
            else
                riferimentoTestuale = riferimentoTestuale.substring(0, riferimentoTestuale.indexOf(",")) + dopoLibro;
        }

        String[] separatori = separatoriNeiRiferimenti();
        StringBuilder rifSB = new StringBuilder(riferimentoTestuale);

        if (capitolo1 == 1 && capitolo2 > 240) {
            if (libro1 == libro2) { // Gv
                // rifSB += "";
            } else { // Gv-At
                rifSB.append("-");
                if (rf == RiferimentoFormato.INTERO)
                    rifSB.append(libriNomi[libro2]);
                else if (rf == RiferimentoFormato.ABBREVIAZIONE)
                    rifSB.append(libriAbbreviazioniUsate[libro2]);
                else if (rf == RiferimentoFormato.ABBREVIAZIONE_RICONOSCIUTA) {
                    String s = libriAbbreviazioniRiconosciute.Abbreviazione(libro2);
                    rifSB.append(s.substring(0, s.indexOf(",")));
                }
            }
        } else {
            if (versetto1 == 1 && versetto2 > 240) {
                if (libro1 == 38 || libro1 == 64 || libro1 == 70 || libro1 == 71 || libro1 == 72) {
                    // rifSB += "";
                } else
                    rifSB.append(capitolo1);

                if (libro1 == libro2) {
                    if (capitolo1 == capitolo2) { // Gv 4
                        // rifSB += "";
                    } else
                        // Gv 4-5
                        rifSB.append("-").append(capitolo2);
                } else { // Gv 4-At 3
                    rifSB.append("-");
                    if (rf == RiferimentoFormato.INTERO)
                        rifSB.append(libriNomi[libro2]).append(dopoLibro);
                    else if (rf == RiferimentoFormato.ABBREVIAZIONE)
                        rifSB.append(libriAbbreviazioniUsate[libro2]).append(dopoLibro);
                    else if (rf == RiferimentoFormato.ABBREVIAZIONE_RICONOSCIUTA) {
                        String s = libriAbbreviazioniRiconosciute.Abbreviazione(libro2);
                        rifSB.append(s.substring(0, s.indexOf(","))).append(dopoLibro);
                    }
                    if (libro2 == 38 || libro2 == 64 || libro2 == 70 || libro2 == 71 || libro2 == 72) {
                        // rifSB += "";
                    } else
                        rifSB.append(capitolo2);
                }
            } else {
                if (libro1 == 38 || libro1 == 64 || libro1 == 70 || libro1 == 71 || libro1 == 72)
                    rifSB.append(versetto1);
                else
                    rifSB.append(capitolo1).append(separatori[1]).append(versetto1);

                if (libro1 == libro2) {
                    if (capitolo1 == capitolo2) {
                        if (versetto1 != versetto2)
                            rifSB.append("-").append(versetto2);
                    } else
                        rifSB.append("-").append(capitolo2).append(separatori[1]).append(versetto2);
                } else {
                    riferimentoTestuale += " - ";
                    if (rf == RiferimentoFormato.INTERO)
                        rifSB.append(libriNomi[libro2]).append(dopoLibro);
                    else if (rf == RiferimentoFormato.ABBREVIAZIONE)
                        rifSB.append(libriAbbreviazioniUsate[libro2]).append(dopoLibro);
                    else if (rf == RiferimentoFormato.ABBREVIAZIONE_RICONOSCIUTA) {
                        String s = libriAbbreviazioniRiconosciute.Abbreviazione(libro2);
                        rifSB.append(s.substring(0, s.indexOf(","))).append(dopoLibro);
                    }
                    if (libro2 == 38 || libro2 == 64 || libro2 == 70 || libro2 == 71 || libro2 == 72)
                        rifSB.append(versetto2);
                    else
                        rifSB.append(capitolo2).append(separatori[1]).append(versetto2);
                }
            }
        }

        if (formato.getRiferimentoTipo() == RiferimentoTipo.CITAZIONE)
            rifSB.append(":");

        return rifSB.toString().trim().replaceFirst(" -", "-");
    }

    public String getLibroNome(int numeroLibro) {
        return ((numeroLibro >= 1 && numeroLibro <= 73) ? libriNomi[numeroLibro] : "");
    }

    public void setLibroNome(int numeroLibro, String nome) {
        if (numeroLibro >= 1 && numeroLibro <= 73) {
            libriNomi[numeroLibro] = nome;
        }
    }

    public int capitoliInLibro(int libro, String nomeVersione) {
        try {
            return getTesto(nomeVersione).capitoliInLibro[libro];
        } catch (TestoNonEsisteException e) {
            return 0;
        }
    }

    public int capitoliFinoALibro(int libro, String nomeVersione) {
        try {
            return getTesto(nomeVersione).indiceLibri[libro - 1];
        } catch (TestoNonEsisteException e) {
            return 0;
        }
    }

    public int versettiInCapitolo(int libro, int capitolo, String nomeVersione) {
        try {
            return getTesto(nomeVersione).capitoliInLibro[libro] == 0 ? 0 : getTesto(nomeVersione).versettiInCapitolo[getTesto(nomeVersione).indiceLibri[libro - 1] + capitolo];
        } catch (TestoNonEsisteException | ArrayIndexOutOfBoundsException e) {
            return 0;
        }
    }

    public int versettiFinoACapitolo(int libro, int capitolo, String nomeVersione) {
        try {
            return getTesto(nomeVersione).indiceCapitoli[getTesto(nomeVersione).indiceLibri[libro - 1] + capitolo];
        } catch (TestoNonEsisteException e) {
            return 0;
        }
    }

    private int versettiFinoACapitolo(int libro, int capitolo) {
        String versione = getUltimaBibbia();
        if (versione.isEmpty()) {
            return 0;
        }

        Testo testo = listaVersioni.get(versione);

        // Check if the version actually exists in the map
        if (testo == null) {
            return 0;
        }

        return testo.indiceCapitoli[testo.indiceLibri[libro - 1] + capitolo];
    }

    // Il numero di un libro in cui è un certo capitolo della Bibbia (contando da 1 a circa 1300).
    // capitolo: Il capitolo da cercare (1-50 in Genesi, 51-90 in Esodo, ecc.).
    // Restituisce il numero del libro (1=Genesi, 47=Matteo, 73=Apocalisse)
    public int libroDiCapitolo(int capitolo, String nomeVersione) {
        int capitoloPos = (Math.max(capitolo, 1));
        int libro = 0;
        do {
            libro++;
        } while (libro <= 73 && capitoliFinoALibro(libro, nomeVersione) < capitoloPos);
        return libro;
    }

    // Il riferimento in cui è un certo versetto della Bibbia (contando da 1 a circa 31000).
    // Versetto: Il numero del versetto da cercare (1-31 in Genesi 1, 32-56 in Genesi 2, ecc.).
    // Restituisce il riferimento del versetto.
    public Riferimento riferimentoDiVersetto(int versetto, String nomeVersione) {
        int versettoPos = (Math.max(versetto, 1));

        int libro = 0;
        do {
            libro++;
        } while (libro <= 73 && versettiFinoACapitolo(libro, capitoliInLibro(libro, nomeVersione), nomeVersione) < versettoPos);

        int capitolo = 0;
        do {
            capitolo++;
        } while (versettiFinoACapitolo(libro, capitolo, nomeVersione) < versettoPos);

        return new Riferimento(libro, capitolo, versettoPos - versettiFinoACapitolo(libro - 1, capitolo - 1, nomeVersione)
                + versettiInCapitolo(libro - 1, capitolo - 1, nomeVersione));
    }

    public String[] separatoriNeiRiferimenti() {
        String[] separatori = new String[3];
        switch (formato.getRiferimentoTipo()) {
            case VIRGOLA:
                separatori[0] = " ";
                separatori[1] = ",";
                separatori[2] = ".";
                break;
            case CITAZIONE:
                separatori[0] = ((formato.getRiferimentoFormato() == RiferimentoFormato.ABBREVIAZIONE) ? "., " : ", ");
                separatori[1] = ", ";
                separatori[2] = ".";
                break;
            default: // DuePunti o valori illegali
                separatori[0] = " ";
                separatori[1] = ":";
                separatori[2] = ",";
                break;
        }
        return separatori;
    }

    public Riferimento elencaNoteInBrano(Riferimento brano, String nomeVersione) {
        try {
            return getTesto(nomeVersione).elencaNoteInBrano(brano);
        } catch (TestoNonEsisteException e) {
            return new Riferimento();
        }
    }

    // / Se un brano o delle note esistono in una certa versione.
    // / Restituisce vero se il brano o nota esiste.
    // / riferimento: Il brano o elenco di note da controllare.
    // / nomeVersione: La versione in cui cercare il brano o note.
    public Boolean esisteBrano(Riferimento riferimento, String nomeVersione) throws TestoNonEsisteException {
        try {
            return getTesto(nomeVersione).esisteBrano(riferimento);
        } catch (TestoNonEsisteException e) {
            throw new TestoNonEsisteException(nomeVersione);
        }
    }

    public String convertiTitoloNotaARiferimento(String notaDaConvertire) {
        // vedi anche Riferimento.ComeNota per l'altra direzione
        if (notaDaConvertire.isEmpty())
            return "";

        String[] separatori = separatoriNeiRiferimenti();
        StringBuilder riferimento = new StringBuilder();

        String[] note = notaDaConvertire.split("#");
        for (String nota : note) {
            try {
                if (!riferimento.toString().isEmpty())
                    riferimento.append(";");
                // nota non ha # all'inizio qui
                int libro1 = Integer.parseInt(nota.substring(0, 2));
                riferimento.append(libriAbbreviazioniUsate[libro1]);
                int capitolo1 = Integer.parseInt(nota.substring(2, 5));
                int versetto1 = Integer.parseInt(nota.substring(5, 8));
                int numeroParola1 = Integer.parseInt(nota.substring(8, 12));
                int capitoliInLibro1 = capitoliInLibro(libro1, ultimaBibbiaCompleta);
                if (capitolo1 > 0) {
                    riferimento.append(separatori[0]);
                    if (capitoliInLibro1 != 1)
                        riferimento.append(capitolo1);
                    if (versetto1 > 0) {
                        if (capitoliInLibro1 != 1)
                            riferimento.append(separatori[1]);
                        riferimento.append(versetto1);
                        if (numeroParola1 > 0)
                            riferimento.append("/").append(numeroParola1);
                    }
                }

                if (!nota.substring(0, 12).equals(nota.substring(13, 25))) {
                    riferimento.append("-");
                    int libro2 = Integer.parseInt(nota.substring(13, 15));
                    int capitolo2 = Integer.parseInt(nota.substring(15, 18));
                    int versetto2 = Integer.parseInt(nota.substring(18, 21));
                    int numeroParola2 = Integer.parseInt(nota.substring(21, 25));
                    int capitoliInLibro2 = capitoliInLibro(libro2, ultimaBibbiaCompleta);
                    if (libro2 != libro1) {
                        riferimento.append(libriAbbreviazioniUsate[libro2]);
                        if (capitolo2 > 0) {
                            riferimento.append(separatori[0]);
                            if (capitoliInLibro2 != 1)
                                riferimento.append(capitolo2);
                            if (versetto2 > 0) {
                                if (capitoliInLibro1 != 1)
                                    riferimento.append(separatori[1]);
                                riferimento.append(versetto2);
                                if (numeroParola2 > 0)
                                    riferimento.append("/").append(numeroParola2);
                            }
                        }
                    } else {
                        if (capitolo2 != capitolo1) {
                            if (capitolo2 > 0) {
                                riferimento.append(capitolo2);
                                if (versetto2 > 0) {
                                    riferimento.append(separatori[1]).append(versetto2);
                                    if (numeroParola2 > 0)
                                        riferimento.append("/").append(numeroParola2);
                                }
                            }
                        } else {
                            if (versetto2 != versetto1 || numeroParola2 > 0) { // aggiungi il numero del versetto se c'è la parola, altrimenti c'è un riferimento ambiguo come Gen
                                // 1:2/3-4 invece di Gen 1:2/3-2/4.
                                if (versetto2 > 0) {
                                    riferimento.append(versetto2);
                                    if (numeroParola2 > 0)
                                        riferimento.append("/").append(numeroParola2);
                                }
                            }
                        }
                    }
                }
            } catch (Exception e) {
                // se c'è un errore nel formato, saltiamo
            }
        }
        return riferimento.toString();
    }

    public List<ComponenteInformazioni> getTestiInstallati() throws ParserConfigurationException, IOException, SAXException {
        return getTestiDisponibili(null);
    }

    /*
    public List<ComponenteInformazioni> getTestiDisponibili() throws ParserConfigurationException, IOException, SAXException {
        return getTestiDisponibili(URL_FILE_AGGIORNAMENTI);
    }
    */

    // NB
    // @Override
    public List<ComponenteInformazioni> getTestiDisponibili(String url) throws ParserConfigurationException, IOException, SAXException {
        // ParserConfigurationException: problema nella configurazione di Java Run Time
        // IOException, SAXParseException: errore nello scaricamento del file con le informazioni sulle versioni (nessun collegamento Internet, sito non funziona, ecc)
        // SAXException: errore nel file scaricato

        List<ComponenteInformazioni> listaComponenti = new ArrayList<>();
        Map<String, Boolean> versioniAggiornabili = new HashMap<>();
        cacheUltimoFileAggiornamenti = false;

        for (String s : listaVersioni.keySet()) versioniAggiornabili.put(s, false);

        if (url != null) {
            DocumentBuilderFactory dbFactory = DocumentBuilderFactory.newInstance();
            DocumentBuilder dBuilder = dbFactory.newDocumentBuilder();
            Document doc;
            if (url.startsWith("file://")) {
                doc = dBuilder.parse(new File(url.substring(7)));
            } else {
                doc = dBuilder.parse(url);
            }
            doc.getDocumentElement().normalize();

            try {
                cacheUltimoFileAggiornamenti = doc.getElementsByTagName("cache").item(0).getChildNodes().item(0).getNodeValue().equals("vero");
            } catch (Exception e) {
                cacheUltimoFileAggiornamenti = true;
            }

            NodeList nList = doc.getElementsByTagName("file");
            String nomeVersioneDisponibile, tipoStringa, nomeVersioneInstallata;
            int v1, v2, v3;
            TestoTipi tipo;
            StatoAggiornamento stato;
            VersioneInformazioni vi;

            for (int i = 0; i < nList.getLength(); ++i) {
                Node nNode = nList.item(i);
                if (nNode.getNodeType() == Node.ELEMENT_NODE) {
                    Element eElement = (Element) nNode;

                    nomeVersioneDisponibile = getTagValue("componente", eElement);
                    tipoStringa = getTagValue("tipo", eElement).toLowerCase();
                    tipo = switch (tipoStringa) {
                        case "bibbia" -> TestoTipi.BIBBIA;
                        case "commentario" -> TestoTipi.COMMENTARIO;
                        case "dizionario" -> TestoTipi.DIZIONARIO;
                        case "libro" -> TestoTipi.LIBRO;
                        default -> TestoTipi.NESSUNO;
                    };

                    ComponenteInformazioni ci = new ComponenteInformazioni(nomeVersioneDisponibile, getTagValue("descrizione", eElement), getTagValue("versione", eElement),
                            getTagValue("motivo", eElement), getTagValue("url", eElement), Integer.parseInt(getTagValue("dimensione", eElement)), getTagValue("url2", eElement),
                            Integer.parseInt(getTagValue("dimensione2", eElement)), EnumSet.of(tipo));

                    stato = StatoAggiornamento.NON_INSTALLATO;
                    for (String s : listaVersioni.keySet()) {
                        nomeVersioneInstallata = s;
                        if (nomeVersioneInstallata.equals(nomeVersioneDisponibile)) {
                            try {
                                vi = getTesto(nomeVersioneInstallata).getInfo();
                                v1 = vi.getVersione1();
                                v2 = vi.getVersione2();
                                v3 = vi.getVersione3();
                                if (v1 < ci.getVersione1() || (v1 == ci.getVersione1() && v2 < ci.getVersione2())
                                        || (v1 == ci.getVersione1() && v2 == ci.getVersione2() && v3 < ci.getVersione3()))
                                    stato = StatoAggiornamento.DA_AGGIORNARE;
                                else
                                    stato = StatoAggiornamento.AGGIORNATO;
                                versioniAggiornabili.put(nomeVersioneInstallata, true);
                            } catch (TestoNonEsisteException e) {
                                // non fare niente, sarà come se non fosse installato
                            }
                        }
                    }
                    if (ci.getVersione1() > versioneMassimaFile1 || (ci.getVersione1() == versioneMassimaFile1 & ci.getVersione2() > versioneMassimaFile2)) {
                        if (stato == StatoAggiornamento.NON_INSTALLATO)
                            stato = StatoAggiornamento.INSTALLAZIONE_NON_COMPATIBILE;
                        else
                            stato = StatoAggiornamento.AGGIORNAMENTO_NON_COMPATIBILE;
                    }
                    ci.setStatoAggiornamento(stato);

                    listaComponenti.add(ci);
                }
            }
        }

        Iterator<String> it2 = versioniAggiornabili.keySet().iterator();
        String versioneNonAggiornabile;
        VersioneInformazioni vi;
        while (it2.hasNext()) {
            versioneNonAggiornabile = it2.next();
            if (Boolean.FALSE.equals(versioniAggiornabili.get(versioneNonAggiornabile))) {
                ComponenteInformazioni ci;
                try {
                    vi = getTesto(versioneNonAggiornabile).getInfo();
                    ci = new ComponenteInformazioni(versioneNonAggiornabile, vi.getDescrizione(), vi.getVersione(), "", "", vi.getDimensione(), "", vi.getDimensione(),
                            vi.getTipo());
                } catch (TestoNonEsisteException e) {
                    ci = new ComponenteInformazioni(versioneNonAggiornabile, "", "0.0.0", "", "", 0, "", 0, EnumSet.of(TestoTipi.NESSUNO));
                }
                ci.setStatoAggiornamento(url != null ? StatoAggiornamento.NON_DISPONIBILE : StatoAggiornamento.SENZA_INTERNET);
                listaComponenti.add(ci);
            }
        }

        return listaComponenti;
    }

    private static String getTagValue(String sTag, Element eElement) {
        try {
            return eElement.getElementsByTagName(sTag).item(0).getChildNodes().item(0).getNodeValue();
        } catch (NullPointerException e) {
            // se sTag non esiste o è vuoto (=> getChildNodes è vuoto), restituiamo una stringa vuota
            return "";
        }
    }
}
