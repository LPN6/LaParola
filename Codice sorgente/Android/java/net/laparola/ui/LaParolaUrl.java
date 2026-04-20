package net.laparola.ui;

import net.laparola.core.RicercaErroreSintassiException;
import net.laparola.core.RicercaEspressioneVuotaException;
import net.laparola.core.RicercaParentesiException;
import net.laparola.core.RicercaParentesiQuadrateException;
import net.laparola.core.Riferimento;
import net.laparola.core.Testi.TestoTipi;
import net.laparola.core.TestoNonEsisteException;
import net.laparola.core.VersioneInformazioni;

import java.io.UnsupportedEncodingException;
import java.net.URLDecoder;
import java.net.URLEncoder;
import java.util.EnumSet;
import java.util.List;
import java.util.Locale;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import timber.log.Timber;

/*
laparola:[brani][_ricerca][@testo][£commentario aggiuntivo][#segnalibro]
"brani" descrive i brani da ricercare. Può essere espresso come:
- laparola:Gen 1,1
- laparola:1 1 1 1 1 30
- laparola:$Titolo della nota
- laparola:$$ (elenco di tutte le note) 
- laparola:$;...-... (nota da convertire in #...-...) 
*/

public class LaParolaUrl {
    // da escludere : %*?|!/\^~()[]:"<>+
    private static final Pattern url_regex = Pattern.compile(
            "^" +
                    "(?:(.+?):)?/*" +   // schema
                    "(" +   // serve per far riconoscere il contenuto per schemi diversi da laparol)
                    "(.*?)" +   // brani
                    "(?:_(.*?))?" +   // ricerca
                    "(?:@(.*?))?" +   // versione
                    "(?:[&£](.*?))?" +   // versioneCommentarioAggiuntivo
                    "(?:#(.*?))?" +   // ancoraggio
                    ")$");

    public String requesturl;
    public String schema;
    public String contenuto;
    public String brani;
    public String ricerca;
    public String versione;
    public String versioneCommentario;
    public String ancoraggio;
    public boolean forzaAltraFinestra;
    public boolean cacheAttiva;
    public boolean gestito;
    public String testo;

    private Riferimento riferimento;
    private boolean creaRiferimento;


    /* package */LaParolaUrl(String schema, String parte_gerarchica, String contenuto, String ricerca, String versione, String versioneCommentario, String ancoraggio, LaParolaBrowser browser) {
        this.schema = schema;
        this.contenuto = parte_gerarchica;
        this.brani = contenuto;
        this.ricerca = ricerca;
        this.versione = versione;
        this.versioneCommentario = versioneCommentario;
        this.ancoraggio = ancoraggio;

        gestito = true;

        init(browser.getVersione(), browser.getVersioneCommentario());

        requesturl = getUrl();
    }

    /* package */LaParolaUrl(String url, String versionePredefinita, String versioneCommentarioPredefinita) {
        try {
            url = URLDecoder.decode(url, "UTF-8");
        } catch (UnsupportedEncodingException ignored) {
        }
        catch (IllegalArgumentException ign) {
            // URL is malformed
        }

        requesturl = url;

        Matcher m = url_regex.matcher(url);
        if (!m.matches()) {
            gestito = false;
        } else {
            gestito = true;

            schema = m.group(1);
            contenuto = m.group(2);
            brani = m.group(3);
            ricerca = m.group(4);
            versione = m.group(5);
            versioneCommentario = m.group(6);
            ancoraggio = m.group(7);

            init(versionePredefinita, versioneCommentarioPredefinita);
        }
    }

    private void init(String versionePredefinita, String versioneCommentarioPredefinita) {
        riferimento = null;
        creaRiferimento = false;

        if (schema == null || schema.isEmpty())
            schema = "laparola";

        if (brani != null && brani.isEmpty())
            brani = null;

        if (ricerca != null && ricerca.isEmpty())
            ricerca = null;

        if (versione == null || versione.isEmpty()) {
            versione = versionePredefinita != null ? versionePredefinita : "";
        }

        if (versione.toLowerCase(Locale.ENGLISH).equals("*bibbia")) {
            versione = LaParolaBrowser.getUltimaBibbia();
            if (versioneCommentario == null || versioneCommentario.isEmpty())
                versioneCommentario = versioneCommentarioPredefinita != null ? versioneCommentarioPredefinita : "";
        }

        if (versioneCommentario == null)
            versioneCommentario = "";

        if (ancoraggio == null || ancoraggio.isEmpty())
            ancoraggio = "inizio";

        switch (schema) {
            case "laparola" -> {
                if (brani == null && ricerca == null) {
                    if (LaParolaBrowser.getCapitoliInLibro(1, versione) != 0) {
                        brani = "Gen 1";
                    } else {
                        brani = "Mat 1";
                    }
                }

                cacheAttiva = true;
                creaRiferimento = true;
            }
            case "lpfile", "lpcomando", "lpsegnalibro", "lppreferiti", "lpcronologia",
                 "lpsegnalibri", "lpevidenziati" -> cacheAttiva = false;
            case "null" -> gestito = false;
            default -> {
                gestito = false;
                //throw new RuntimeException("Schema non gestito (" + schema + ")");
                schema = "error";
            }
        }
    }

    public Riferimento getRiferimento() {
        if (creaRiferimento && riferimento == null) {
            creaRiferimento = false;
            creaRiferimento();
        }

        return riferimento;
    }

    public int[] getLCV() {
        if (brani == null || ricerca != null)
            return null;

        int[] lcv = null;

        if (ancoraggio != null) {
            try {
                String[] s = ancoraggio.split("_");

                int b = 1;
                for (int i = 1; i <= 73; i++) {
                    if (LaParolaBrowser.getAbbreviazioneLibro(i).equals(s[0])) {
                        b = i;
                        break;
                    }
                }

                lcv = new int[]{b, Integer.parseInt(s[1]), Integer.parseInt(s[2])};
            } catch (Exception e) {
                //
            }
        }

        Riferimento rif = getRiferimento();
        if (lcv == null && rif != null) {
            List<int[]> rifbrani = rif.getBrani();
            // se è un riferimento continuo, va all'inizio, indipendentemente da eventuali libri o capitoli diversi
            if (rifbrani.size() == 1) {
                int[] rifbrano = rifbrani.get(0);
                // if (rifbrano[0] == rifbrano[3] && rifbrano[1] == rifbrano[4]) {
                lcv = new int[]{rifbrano[0], rifbrano[1], rifbrano[2]};
                // }
            }
        }

        return lcv;
    }

    private void creaRiferimento() {
        if (brani != null && ricerca == null) {
            if (!brani.startsWith("$")) {
                riferimento = LaParolaBrowser.creaRiferimento(brani, versione);
                brani = LaParolaBrowser.normalizzaRiferimento(riferimento, versione);
            } else {
                // è un riferimento ad una nota con titolo
                riferimento = null;
                testo = null;
            }
        } else if (ricerca != null) {
            try {
                brani = LaParolaBrowser.normalizzaRiferimento(brani, versione);
            } catch (Exception e) {
                //
            }

            try {
                riferimento = LaParolaBrowser.ricerca(ricerca, brani != null ? brani : "", versione);
            } catch (RicercaEspressioneVuotaException e) {
                testo = LaParolaStringhe.get(LaParolaStringhe.ERRORE_RICERCA_ESPRESSIONE_VUOTA);
                cacheAttiva = false;
            } catch (RicercaErroreSintassiException e) {
                if (brani != null || !LaParolaBrowser.isRiferimento(ricerca, versione)) {
                    testo = String.format(LaParolaStringhe.get(LaParolaStringhe.ERRORE_RICERCA_ERRORE_SINTASSI), e.getMessage());
                } else {
                    testo = String.format(LaParolaStringhe.get(LaParolaStringhe.ERRORE_RICERCA_ERRORE_SINTASSI_PROPONI_RIFERIMENTO), e.getMessage(), ricerca);
                }
                cacheAttiva = false;
            } catch (RicercaParentesiException e) {
                testo = LaParolaStringhe.get(LaParolaStringhe.ERRORE_RICERCA_PARENTESI);
                cacheAttiva = false;
            } catch (RicercaParentesiQuadrateException e) {
                testo = LaParolaStringhe.get(LaParolaStringhe.ERRORE_RICERCA_PARENTESI_QUADRATE);
                cacheAttiva = false;
            } catch (Exception e) {
                if (brani != null || !LaParolaBrowser.isRiferimento(ricerca, versione)) {
                    testo = LaParolaStringhe.get(LaParolaStringhe.ERRORE_RICERCA);
                } else {
                    testo = String.format(LaParolaStringhe.get(LaParolaStringhe.ERRORE_RICERCA_PROPONI_RIFERIMENTO), ricerca);
                }
                cacheAttiva = false;
            }
        } else {
            gestito = false;
        }
    }

    public String getChiaveCache() {
        return String.format("%s:%s_%s@%s£%s",
                schema,
                brani != null ? brani : "",
                ricerca != null ? ricerca : "",
                versione,
                versioneCommentario);
    }

    public boolean stessoTestoDi(LaParolaUrl altro) {
        return altro != null && getChiaveCache().equals(altro.getChiaveCache());
    }

    public String getUrl() {
        return getUrl(null, null, true);
    }

    public String getUrlConAltraVersione(String nuovaVersione, String nuovaVersioneCommentario) {
        return getUrl(nuovaVersione, nuovaVersioneCommentario, true);
    }

    public String getUrlConAltraVersione(String nuovaVersione) {
        return getUrl(nuovaVersione, null, true);
    }

    public String getUrlConAltraVersioneCommentario(String nuovaVersioneCommentario) {
        return getUrl(null, nuovaVersioneCommentario, true);
    }

    private String getUrl(String nuovaVersione, String nuovaVersioneCommentario, boolean conAncoraggio) {
        if (schema.equals("lpcomando")) {
            return String.format("lpcomando:%s", contenuto);
        }

        String nuoviBrani = brani;
        String nuovoAncoraggio = ancoraggio;
        if (nuovaVersione != null && brani != null && versione != null) {
            Riferimento rif = getRiferimento();
            if (schema.equals("laparola") && rif != null) {
                nuoviBrani = LaParolaBrowser.cambiaVersioneRiferimento(rif, versione, nuovaVersione);
            }
            if (conAncoraggio && ancoraggio != null) {
                String[] t = ancoraggio.split("_");
                if (t.length == 3) {
                    Riferimento rifAncoraggio = LaParolaBrowser.creaRiferimento(t[0] + " " + t[1] + ":" + t[2], versione);
                    nuovoAncoraggio = LaParolaBrowser.cambiaVersioneRiferimento(rifAncoraggio, versione, nuovaVersione);
                    nuovoAncoraggio = nuovoAncoraggio.replace(' ', '_').replace(':', '_');
                }
            }
        }

        return String.format("%s:%s_%s@%s£%s#%s",
                schema,
                nuoviBrani != null ? nuoviBrani : "",
                ricerca != null ? ricerca : "",
                nuovaVersione == null ? (versione != null ? versione : "") : nuovaVersione,
                nuovaVersioneCommentario == null ? (versioneCommentario != null ? versioneCommentario : "") : nuovaVersioneCommentario,
                (ancoraggio != null && conAncoraggio) ? nuovoAncoraggio : "");
    }

    public String getDescrizione() {
        String v = versione != null ? versione : "";
        if (versioneCommentario != null && !versioneCommentario.isEmpty()) {
            if (!getTipoTesto().contains(TestoTipi.COMMENTARIO)) {
                v = LaParolaStringhe.get(LaParolaStringhe.VERSIONE_CON_COMMENTARIO, v, versioneCommentario);
            }
        }

        switch (schema) {
            case "laparola" -> {
                if (ricerca != null) {
                    if (brani == null) {
                        return LaParolaStringhe.get(LaParolaStringhe.DESCRIZIONE_URL_RICERCA, ricerca, v);
                    }
                    return LaParolaStringhe.get(LaParolaStringhe.DESCRIZIONE_URL_RICERCA_IN, ricerca, brani, v);
                } else if (brani != null && brani.startsWith("$")) {
                    // Nota con titolo
                    if (brani.equals("$$")) {
                        return LaParolaStringhe.get(LaParolaStringhe.ELENCO_NOTE);
                    } else if (brani.startsWith("$;")) {
                        String tn = brani.substring(1).replace(';', '#');
                        return LaParolaStringhe.get(LaParolaStringhe.DESCRIZIONE_URL_BRANI, LaParolaBrowser.mTesti.convertiTitoloNotaARiferimento(tn), v);
                    } else {
                        return LaParolaStringhe.get(LaParolaStringhe.DESCRIZIONE_URL_BRANI, brani.substring(1), v);
                    }
                } else if (brani != null) {
                    Riferimento rif = getRiferimento();
                    if (rif == null)
                        return "";
                    String normBrani = LaParolaBrowser.normalizzaRiferimento(rif, versione);
                    return LaParolaStringhe.get(LaParolaStringhe.DESCRIZIONE_URL_BRANI, normBrani, v);
                }
            }
            case "lpfile" -> {
                int ipos = brani.lastIndexOf('/');
                int fpos = brani.lastIndexOf('.');

                if (ipos == -1)
                    ipos = -1;
                if (fpos == -1)
                    fpos = brani.length() + 1;

                return LaParolaStringhe.get(LaParolaStringhe.DESCRIZIONE_URL_FILE, brani.substring(ipos + 1, fpos), v);
            }
            case "lpsegnalibri" -> {
                return LaParolaStringhe.get(LaParolaStringhe.DESCRIZIONE_URL_GRUPPI_SEGNALIBRI);
            }
            case "lpsegnalibro" -> {
                return LaParolaStringhe.get(LaParolaStringhe.DESCRIZIONE_URL_GRUPPO_SEGNALIBRI, brani, v);
            }
            case "lppreferiti" -> {
                return LaParolaStringhe.get(LaParolaStringhe.DESCRIZIONE_URL_PREFERITI);
            }
            case "lpevidenziati" -> {
                return LaParolaStringhe.get(LaParolaStringhe.DESCRIZIONE_URL_EVIDENZIATI);
            }
            case "lpcronologia" -> {
                return LaParolaStringhe.get(LaParolaStringhe.DESCRIZIONE_URL_CRONOLOGIA);
            }
        }

        return "";
    }

    public String getUrlSuccessivo() {
        if (!schema.equals("laparola") || getRiferimento() == null) {
            return null;
        }

        List<int[]> rifbrani = getRiferimento().getBrani();
        if (rifbrani.size() != 1) {
            return null;
        }
        String versioneDaControllare = versione;


        try {
            if (!LaParolaBrowser.mTesti.getInfo(versione).getTipo().contains(TestoTipi.BIBBIA)) {
                versioneDaControllare = LaParolaBrowser.getUltimaBibbia();
                if (versioneDaControllare.isEmpty())
                    return null;
            }
        } catch (TestoNonEsisteException e) {
            return null;
        }

        String rif = null;

        int[] rifbrano = rifbrani.get(0);
        int l1 = rifbrano[0];
        int c1 = rifbrano[1];
        int l2 = rifbrano[3];
        int c2 = rifbrano[4];
        int v2 = rifbrano[5];
        int versettiInUltimoCapitolo = LaParolaBrowser.mTesti.versettiInCapitolo(l2, c2, versioneDaControllare);

        if (v2 >= versettiInUltimoCapitolo) {
            // alla fine del capitolo : vado al capitolo successivo
            if (c2 >= LaParolaBrowser.mTesti.capitoliInLibro(l2, versioneDaControllare)) {
                // ultimo capitolo di un libro : vado al libro successivo
                int l = l2 + 1;
                while (l < 74 && LaParolaBrowser.mTesti.capitoliInLibro(l, versioneDaControllare) == 0) {
                    // considero i libri mancanti nelle varie versioni
                    l++;
                }
                if (l < 74) { // altrimenti siamo all'ultimo libro del testo: Apoc, o Malachia se solo AT, ecc
                    int c = 1;

                    rif = getUrlLCV(l, c, 1, l, c, 255, "inizio");
                }
            } else {
                // altro capitolo
                rif = getUrlLCV(l1, c1 + 1, 1, l1, c1 + 1, 255, "inizio");
            }

        } else {
            // in mezzo al capitolo : mostra l'intero capitolo (eventualmente, gli interi capitoli)
            rif = getUrlLCV(l1, c1, 1, l2, c2, 255,
                    String.format(Locale.getDefault(), "%s_%d_%d", LaParolaBrowser.mTesti.getLibroAbbreviazioneUsata(l2), c2, v2 + 1));
        }

        return rif;
    }

    public String getUrlPrecedente() {
        if (!schema.equals("laparola") || getRiferimento() == null) {
            return null;
        }

        List<int[]> rifbrani = getRiferimento().getBrani();
        if (rifbrani.size() != 1) {
            return null;
        }
        String versioneDaControllare = versione;

        try {
            if (!LaParolaBrowser.mTesti.getInfo(versione).getTipo().contains(TestoTipi.BIBBIA)) {
                versioneDaControllare = LaParolaBrowser.getUltimaBibbia();
                if (versioneDaControllare.isEmpty())
                    return null;
            }
        } catch (TestoNonEsisteException e) {
            return null;
        }

        String rif = null;

        int[] rifbrano = rifbrani.get(0);
        int l1 = rifbrano[0];
        int c1 = rifbrano[1];
        int v1 = rifbrano[2];
        int l2 = rifbrano[3];
        int c2 = rifbrano[4];
        // int v2 = rifbrano[5];
        //int versettiInUltimoCapitolo = LaParolaBrowser.mTesti.versettiInCapitolo(l2, c2, versioneDaControllare);

        if (v1 == 1) {
            // inizio capitolo : vai a capitolo precedente
            if (c1 == 1) {
                // primo capitolo di un libro : torna al libro precedente
                int l = l1 - 1;
                while (l > 0 && LaParolaBrowser.mTesti.capitoliInLibro(l, versioneDaControllare) == 0) {
                    // considero i libri mancanti nelle varie versioni
                    l--;
                }
                if (l > 0) { // altrimenti siamo al primo libro del testo: Genesi, o Matteo se solo NT, ecc
                    int c = LaParolaBrowser.mTesti.capitoliInLibro(l, versioneDaControllare);

                    rif = getUrlLCV(l, c, 1, l, c, 255, "inizio");
                }
            } else {
                // altro capitolo
                rif = getUrlLCV(l1, c1 - 1, 1, l1, c1 - 1, 255, "inizio");
            }
        } else {
            // in mezzo al capitolo : mostra l'intero capitolo (eventualmente, gli interi capitoli)
            rif = getUrlLCV(l1, c1, 1, l2, c2, 255, "inizio");
        }

        return rif;
    }

    private String getUrlLCV(int l1, int c1, int v1, int l2, int c2, int v2, String anchor) {
        // questo formato dell'output (stringa 6 numeri) restituisce un riferimento standard
        Riferimento rif = new Riferimento();
        rif.aggiungiBrano(new int[]{l1, c1, v1, l2, c2, v2});
        rif = LaParolaBrowser.convertiRiferimentoAStandard(rif, versione);
        int[] b = rif.getBrani().get(0);

        String ver = versione;
        String verComm = versioneCommentario;
        try {
            ver = URLEncoder.encode(versione, "UTF-8");
            verComm = URLEncoder.encode(versioneCommentario, "UTF-8");
        } catch (Exception e) {
        }

        return String.format(Locale.ENGLISH, "laparola:%d %d %d %d %d %d@%s£%s#%s",
                b[0], b[1], b[2], b[3], b[4], b[5],
                ver,
                verComm,
                anchor);
    }

    @Override
    public boolean equals(Object o) {
        if (!(o instanceof LaParolaUrl other)) {
            return false;
        }

        return getUrl().equals(other.getUrl());
    }

    public String getUrlSenzaAncoraggio() {
        return getUrl(null, null, false);
    }

    public boolean richiedeNuovaFinestra(LaParolaUrl urlCorrente) {
        if (urlCorrente == null) return forzaAltraFinestra;
        return forzaAltraFinestra ||
                (urlCorrente.getTipoTesto().contains(TestoTipi.COMMENTARIO) && this.getTipoTesto().contains(TestoTipi.BIBBIA)) ||
                (urlCorrente.getTipoTesto().contains(TestoTipi.BIBBIA) && this.getTipoTesto().contains(TestoTipi.COMMENTARIO));
    }

    public EnumSet<TestoTipi> getTipoTesto() {
        try {
            VersioneInformazioni info = LaParolaBrowser.getInformazioniTesto(versione);
            if (info != null && info.getTipo() != null) {
                return info.getTipo();
            }
        } catch (Exception e) {
            // Log it so you know something went wrong!
            Timber.e(e, "Error getting text type");
        }
        return EnumSet.of(TestoTipi.NESSUNO);
    }
}