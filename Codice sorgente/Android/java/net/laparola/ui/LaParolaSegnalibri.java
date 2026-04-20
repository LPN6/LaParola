package net.laparola.ui;

import org.xml.sax.Attributes;
import org.xml.sax.InputSource;
import org.xml.sax.SAXException;
import org.xml.sax.XMLReader;
import org.xml.sax.helpers.DefaultHandler;

import java.io.IOException;
import java.io.InputStream;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

import javax.xml.parsers.ParserConfigurationException;
import javax.xml.parsers.SAXParser;
import javax.xml.parsers.SAXParserFactory;

import timber.log.Timber;

public class LaParolaSegnalibri {
    public static class GruppoSegnalibri implements Comparable<GruppoSegnalibri> {
        public String descrizione;
        public String nome;
        public List<LaParolaSegnalibri.Segnalibro> segnalibri;

        public GruppoSegnalibri() {
            segnalibri = new ArrayList<>();
        }

        public GruppoSegnalibri(String nome) {
            this();
            this.nome = nome;
            this.descrizione = "";
        }

        @Override
        public int compareTo(GruppoSegnalibri another) {
            return nome.compareTo(another.nome);
        }
    }

    public static class Segnalibro implements Comparable<Segnalibro> {
        public String nome;
        public List<String> riferimenti;
        public String inizioSottogruppo;

        public Segnalibro() {
            riferimenti = new ArrayList<>();
        }

        public Segnalibro(String nome, String url) {
            this();
            this.nome = nome;
            this.riferimenti.add(url);
        }

        @Override
        public int compareTo(Segnalibro another) {
            return nome.compareTo(another.nome);
        }

        public void setAncoraggio(String ancoraggio) {
            if (riferimenti.size() != 1)
                return;

            String[] t = riferimenti.get(0).split("#");
            t[t.length - 1] = ancoraggio;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < t.length; i++) {
                if (i != 0)
                    sb.append("#");
                sb.append(t[i]);
            }

            riferimenti.set(0, sb.toString());
        }
    }

    private class SegnalibriHandler extends DefaultHandler {
        private boolean inDescrizione;
        private boolean inGruppo;
        private boolean inSottoGruppo;
        private boolean inNome;
        private boolean inRiferimento;
        private boolean inSegnalibro;

        private final StringBuilder caratteri = new StringBuilder();

        //private String ultimoSottogruppo;
        private ArrayList<String> ultimiRiferimenti;
        private String ultimoNome;
        private String ultimoNomeGruppo;
        private String ultimoNomeSottoGruppo;

        @Override
        public void characters(char[] ch, int start, int length) {
            /*
             * if (caratteri.length() > 0) { caratteri.append(" "); }
             */

            caratteri.append(ch, start, length);
        }

        @Override
        public void endDocument() {
            //
        }

        @Override
        public void endElement(String namespaceURI, String localName, String qName) {
            String valore = caratteri.toString().replace('\t', ' ').replace('\r', ' ').replace('\n', ' ').trim();

            if (inNome) {
                if (inSegnalibro) {
                    ultimoNome = valore;
                } else if (inSottoGruppo) {
                    ultimoNomeSottoGruppo = valore;
                } else if (inGruppo) {
                    ultimoNomeGruppo = valore;
                }
            } else if (inDescrizione) {
                if (inGruppo) {
                }
            } else if (inRiferimento) {
                if (inSegnalibro) {
                    if (valore.contains("&#")) {
                        valore = valore.replace("&", "£");   // workaround per bug introdotto con commentari
                    }
                    if (!ultimiRiferimenti.contains(valore)) {
                        ultimiRiferimenti.add(valore);
                    }
                }
            }
            caratteri.setLength(0);

            switch (localName) {
                case "gruppo" -> inGruppo = false;
                case "sottogruppo" -> {
                    inSottoGruppo = false;
                    ultimoNomeSottoGruppo = null;
                }
                case "segnalibro" -> {
                    inSegnalibro = false;
                    String[] urls = ultimiRiferimenti.toArray(new String[]{});
                    aggiungiSegnalibro(ultimoNomeGruppo, ultimoNome, ultimoNomeSottoGruppo, urls);
                    ultimoNomeSottoGruppo = null;
                }
                case "nome" -> inNome = false;
                case "descrizione" -> inDescrizione = false;
                case "riferimento" -> inRiferimento = false;
            }
        }

        @Override
        public void startDocument() {
            //
        }

        @Override
        public void startElement(String namespaceURI, String localName, String qName, Attributes atts) {
            switch (localName) {
                case "gruppo" -> inGruppo = true;
                case "sottogruppo" -> inSottoGruppo = true;
                case "segnalibro" -> inSegnalibro = true;
                case "nome" -> inNome = true;
                case "descrizione" -> inDescrizione = true;
                case "riferimento" -> {
                    inRiferimento = true;
                    ultimiRiferimenti = new ArrayList<>();
                }
            }
        }
    }

    public static void appendLinkGruppo(StringBuilder res, LaParolaSegnalibri.GruppoSegnalibri gruppo) {
        res.append("<p><a href='lpsegnalibro:");
        res.append(gruppo.nome);
        res.append("'>");
        res.append(gruppo.nome);
        res.append("</a>");
        if (gruppo.descrizione != null && !gruppo.descrizione.isEmpty()) {
            res.append("\n<br />");
            res.append(gruppo.descrizione);
        }
        res.append("</p>\n");
    }

    public static void appendLinkSegnalibro(StringBuilder res, LaParolaSegnalibri.Segnalibro segnalibro, String versionePredefinita, boolean mostraCancella) {
        int n = segnalibro.riferimenti.size();
        if (n == 0)
            return;

        res.append("<p>");
        if (n == 1 && segnalibro.riferimenti.get(0).contains(":")) {
            res.append("<a href='");
            res.append(segnalibro.riferimenti.get(0));
            res.append("'>");
            res.append(segnalibro.nome);
            res.append("</a>");
        } else {
            res.append(segnalibro.nome);
            res.append(" (");

            for (int i = 0; i < n; i++) {
                String riferimento = segnalibro.riferimenti.get(i);

                String normalizzato = LaParolaBrowser.normalizzaRiferimento(riferimento, versionePredefinita);
                res.append("<a href='laparola:");
                res.append(normalizzato);
                res.append("'>");
                res.append(normalizzato);
                res.append("</a>");

                if (i != n - 1) {
                    res.append(", ");
                }
            }

            res.append(")");
        }

        if (mostraCancella) {
            res.append("<a style='color:#9a9a9a;float:right;' href='lpcomando:cancellapreferito:");
            res.append(segnalibro.riferimenti.get(0));
            res.append("'>");
            res.append(LaParolaStringhe.get(LaParolaStringhe.ELIMINA));
            res.append("</a>");
        }

        res.append("</p>\n");
    }

    public List<LaParolaSegnalibri.GruppoSegnalibri> gruppi;

    /* package */LaParolaSegnalibri() {
        gruppi = new ArrayList<>();
    }

    /* package */boolean aggiungiDaXml(InputStream xml) {
        SegnalibriHandler handler = new SegnalibriHandler();
        SAXParserFactory spf = SAXParserFactory.newInstance();
        SAXParser sp;
        XMLReader xr;
        try {
            sp = spf.newSAXParser();
            xr = sp.getXMLReader();
            xr.setContentHandler(handler);
            xr.parse(new InputSource(xml));
            return true;
        } catch (IOException e) {
            Timber.e(e, "Unexpected IO error occurred while adding bookmarks.");
            return true;
        } catch (ParserConfigurationException | SAXException e) {
            Timber.e(e, "Unexpected error occurred while adding bookmarks.");
        }
        return false;
    }

    public void aggiungiSegnalibro(String gruppo, String nome, LaParolaUrl url) {
        aggiungiSegnalibro(gruppo, nome, url.getUrl());
    }

    public void aggiungiSegnalibro(String gruppo, String nome, String url) {
        aggiungiSegnalibro(gruppo, nome, null, new String[]{url});
    }

    public void aggiungiSegnalibro(String gruppo, String nome, String inizioSottogruppo, String[] urls) {
        GruppoSegnalibri g = null;

        for (GruppoSegnalibri gg : gruppi) {
            if (gg.nome.equals(gruppo)) {
                g = gg;
                break;
            }
        }

        if (g == null) {
            g = new GruppoSegnalibri(gruppo);
            gruppi.add(g);

            Collections.sort(gruppi);
        }

        for (String url : urls) {
            Segnalibro newSegnalibro = new Segnalibro(nome, url);
            newSegnalibro.inizioSottogruppo = inizioSottogruppo;

            boolean presente = false;
            for (Segnalibro s : g.segnalibri) {
                if (s.compareTo(newSegnalibro) == 0) {
                    presente = true;
                    break;
                }
            }

            if (presente) {
                continue;
            }

            g.segnalibri.add(newSegnalibro);
        }

        boolean haSottogruppi = false;
        for (Segnalibro s : g.segnalibri) {
            if (s.inizioSottogruppo != null) {
                haSottogruppi = true;
                break;
            }
        }
        if (!haSottogruppi) {
            Collections.sort(g.segnalibri);
        }
    }

    /* package */CharSequence salvaInXml() {
        StringBuilder xml = new StringBuilder();

        boolean insottogruppo = false;

        xml.append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n");
        xml.append("<segnalibri>\n");

        for (GruppoSegnalibri g : gruppi) {
            xml.append("  <gruppo>\n");
            xml.append("    <nome><![CDATA[");
            xml.append(g.nome);
            xml.append("]]></nome>\n");
            xml.append("    <descrizione><![CDATA[");
            xml.append(g.descrizione);
            xml.append("]]></descrizione>\n");
            for (Segnalibro s : g.segnalibri) {
                if (s.inizioSottogruppo != null) {
                    if (insottogruppo) {
                        insottogruppo = false;
                        xml.append("    </sottogruppo>\n");
                    }
                    xml.append("    <sottogruppo>\n");
                    xml.append("      <nome><![CDATA[");
                    xml.append(s.inizioSottogruppo);
                    xml.append("]]></nome>\n");
                }
                xml.append("      <segnalibro>\n");
                xml.append("        <nome><![CDATA[");
                xml.append(s.nome);
                xml.append("]]></nome>\n");
                for (String r : s.riferimenti) {
                    xml.append("        <riferimento><![CDATA[");
                    xml.append(r);
                    xml.append("]]></riferimento>\n");
                }
                xml.append("      </segnalibro>\n");
            }

            if (insottogruppo) {
                insottogruppo = false;
                xml.append("    </sottogruppo>\n");
            }
            xml.append("  </gruppo>\n");
        }

        xml.append("</segnalibri>\n");

        return xml;
    }

    public CharSequence getPaginaGruppi(LaParolaBrowser browser) {
        StringBuilder res = new StringBuilder();

        res.append("<html><head>\n");
        if (browser.mClient != null)
            res.append(browser.mClient.getAggiuntaHeader(null));
        res.append("</head><body>\n");

        res.append("<h3>Segnalibri</h3>\n");

        for (LaParolaSegnalibri.GruppoSegnalibri gruppo : gruppi) {
            appendLinkGruppo(res, gruppo);
        }

        res.append("</body></html>\n");

        return res;
    }

    public CharSequence getPaginaGruppo(String gruppo, String versionePredefinita, LaParolaBrowser browser) {
        boolean preferiti = (gruppo == null);

        StringBuilder res = new StringBuilder();

        res.append("<html><head>\n");
        if (browser.mClient != null)
            res.append(browser.mClient.getAggiuntaHeader(null));
        res.append("</head><body>\n");

        if (gruppi.isEmpty() || (gruppi.size() == 1 && gruppi.get(0).segnalibri.isEmpty())) {
            res.append(LaParolaStringhe.get(LaParolaStringhe.PREFERITI_VUOTO));
        } else {
            for (LaParolaSegnalibri.GruppoSegnalibri g : gruppi) {
                if (gruppo == null || g.nome.equals(gruppo)) {
                    res.append("<h3>");
                    res.append(g.nome);
                    res.append("</h3>\n");

                    if (g.descrizione != null && !g.descrizione.isEmpty()) {
                        res.append("<p>");
                        res.append(g.descrizione);
                        res.append("</p>\n");
                    }

                    for (LaParolaSegnalibri.Segnalibro segnalibro : g.segnalibri) {
                        if (segnalibro.inizioSottogruppo != null) {
                            res.append("<p><b>");
                            res.append(segnalibro.inizioSottogruppo);
                            res.append("</b></p>\n");
                        }
                        appendLinkSegnalibro(res, segnalibro, versionePredefinita, preferiti);
                    }
                }
            }
        }

        res.append("</body></html>\n");

        return res;
    }

    public Segnalibro cercaPerUrl(LaParolaUrl url) {
        // TODO : ha senso fare un Map per la ricerca più rapida?

        if (url == null)
            return null;

        String stringurl = url.getUrlSenzaAncoraggio();

        for (int i = 0; i < gruppi.size(); i++) {
            GruppoSegnalibri g = gruppi.get(i);

            for (int j = 0; j < g.segnalibri.size(); j++) {
                Segnalibro s = g.segnalibri.get(j);

                for (int k = 0; k < s.riferimenti.size(); k++) {
                    String r = s.riferimenti.get(k);

                    if (r.startsWith(stringurl)) {   // non confronta l'ancoraggio
                        return s;
                    }
                }
            }
        }

        return null;
    }

    public void rimuoviSegnalibro(Segnalibro s) {
        for (int i = 0; i < gruppi.size(); i++) {
            GruppoSegnalibri g = gruppi.get(i);
            g.segnalibri.remove(s);
        }
    }
}