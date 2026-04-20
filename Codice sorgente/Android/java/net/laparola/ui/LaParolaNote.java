package net.laparola.ui;

import java.util.ArrayList;
import java.util.List;

public class LaParolaNote {
    public interface NotaOGruppo {
    }

    public static class Nota implements NotaOGruppo {
        public String url;
        public String titolo;
        public boolean conTitolo;
    }

    private static class Gruppo implements NotaOGruppo {
        public String titolo;
        public List<NotaOGruppo> figli;

        public Gruppo(String titolo) {
            this.titolo = titolo;
            this.figli = new ArrayList<>();
        }
    }

    private final StringBuilder _tmpStringBuilder = new StringBuilder();
    private int lastId;

    public CharSequence creaListaNote(LaParolaUrl url) {
        List<NotaOGruppo> note = LaParolaBrowser.elencaNoteInTesto(url.versione);
        if (note == null) {
            return LaParolaStringhe.get(LaParolaStringhe.NESSUNA_NOTA, url.versione);
        }

        _tmpStringBuilder.setLength(0);
        Gruppo gruppi = creaGruppi(note);

        lastId = 0;
        aggiungiGruppo(_tmpStringBuilder, gruppi);

        return _tmpStringBuilder;
    }

    private Gruppo creaGruppi(List<NotaOGruppo> note) {
        Gruppo gruppi = new Gruppo(null);
        Gruppo senzaTitolo = new Gruppo("Senza titolo");
        Gruppo conTitolo = new Gruppo("Con titolo");

        for (NotaOGruppo ng : note) {
            if (ng instanceof Nota nota) {
                if (nota.conTitolo) {
                    conTitolo.figli.add(nota);
                } else {
                    senzaTitolo.figli.add(nota);
                }
            }
        }

        gruppi.figli.add(senzaTitolo);
        gruppi.figli.add(conTitolo);
        return gruppi;
    }

    private void aggiungiGruppo(StringBuilder res, Gruppo note) {
        if (note.figli.isEmpty())
            return;

        if (note.titolo != null) {
            res.append("<p class='gruppo_nome'>");
            res.append(note.titolo);
            res.append("</p>\n");
            res.append("<div id='gruppo");
            res.append(lastId++);
            res.append("' class='gruppo_div'>\n");
        }
        for (NotaOGruppo notaogruppo : note.figli) {
            if (notaogruppo instanceof Nota nota) {
                res.append("<a name='");
                res.append(nota.titolo);
                res.append("' href='");
                res.append(nota.url);
                res.append("'>");
                res.append(nota.titolo);
                res.append("</a><br/>\n");
            } else {
                aggiungiGruppo(res, (Gruppo) notaogruppo);
            }
        }
        if (note.titolo != null) {
            res.append("</div>\n");
        }
    }
}
