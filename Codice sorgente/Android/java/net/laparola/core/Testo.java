package net.laparola.core;

import java.io.*;
import java.nio.ByteBuffer;
import java.nio.CharBuffer;
import java.nio.channels.*;
import java.util.*;
import java.util.regex.Pattern;

import javax.crypto.Cipher;
import javax.crypto.spec.SecretKeySpec;

import net.laparola.core.Testi.*;

import timber.log.Timber;

public class Testo {
    private final VersioneInformazioni info = new VersioneInformazioni();

    VersioneInformazioni getInfo() {
        return info;
    }

    private final String percorso;
    public int[] capitoliInLibro = new int[74];
    public int[] indiceLibri = new int[74];
    public int[] versettiInCapitolo;
    public int[] indiceCapitoli;
    private final Testi genitore;

    private int pTesto;
    private int pTestoIndice;
    private int pParole;
    private int pParoleIndiceIndice;
    private int pParoleIndice;
    private int pRadici;
    private int pRadiciDiParole;
    private int pCitazioniRiferimenti;
    private int pOffset;

    private String[] parole = null;
    private String[] radici = null;
    private int[] radiceDiParola = null;
    private StringBuilder[] paroleDiRadice = null;

    public List<String> noteTitoli = new ArrayList<>();
    private final List<Integer> notePosizione = new ArrayList<>();
    private final List<String> noteNuoveTesto = new ArrayList<>();

    private static class CitazioneRiferimento {
        public int[] brano;
        public int numeroNota;

        public CitazioneRiferimento() {
            //
        }
    }

    private List<CitazioneRiferimento> citazioniRiferimenti = null;

    public List<String> noteInOrdine = new ArrayList<>();

    private static final char REPLACEMENT_CHAR = (char) 0xfffd;

    private boolean interrompiGetBrano = false;

    public void interrompiGetBrano() {
        interrompiGetBrano = true;
    }

    public List<String> NoteTitoli() {
        return noteTitoli;
    }

    private final List<RadiceDiversa> radiciDiverse = new ArrayList<>();
    List<int[]> riferimentiDiversi = new ArrayList<>();

    Testo(Testi t, String percorso) throws FileNonValidoException {
        this.percorso = percorso;
        info.setNomeDelFile(percorso);
        genitore = t;

        try {
            leggiInputStream();
        } catch (FileNonValidoException e) {
            chiudi();
            throw e;
        }
    }

    private void leggiInputStream() throws FileNonValidoException {
        byte[] capitoliInLibroByte = new byte[73];
        pOffset = 0;
        try (FileInputStream localInFile = new FileInputStream(percorso); FileChannel localFc = localInFile.getChannel()) {
            info.setDimensione(localFc.size());
            byte[] b3 = leggiByteAt(localFc, 3, 0);
            if (b3[0] != 'L' || b3[1] != 'P' || b3[2] != 'N') {
                byte[] b45 = leggiByteAt(localFc, 45, 3);

                byte[] b48 = new byte[48];
                b48[0] = b3[0];
                b48[1] = b3[1];
                b48[2] = b3[2];
                System.arraycopy(b45, 0, b48, 3, 45);

                b3 = leggiByteAt(localFc, 45, 48);
                if (b3[0] != 'L' || b3[1] != 'P' || b3[2] != 'N')
                    throw new FileNonValidoException("");

                byte[] decryptedMessage;
                try {
                    Cipher cipher = Cipher.getInstance("AES");
                    StringBuilder keysb = new StringBuilder("lpnj");
                    for (int i = 1; i < 8; ++i)
                        keysb.append("lpnj");

                    SecretKeySpec key = new SecretKeySpec(keysb.toString().getBytes(), "AES");
                    cipher.init(Cipher.DECRYPT_MODE, key);

                    decryptedMessage = cipher.doFinal(b48);
                } catch (Exception e) {
                    //throw new FileNonValidoException("");
                }

                //if (!Arrays.equals(decryptedMessage, genitore.deviceUuid.toString().getBytes()))
                //    throw new FileNonValidoException("");
                pOffset = 48;
            }

            int pDati;
            int pIndice;
            // TODO la versione del programma deve essere dopo quella del testo
            b3 = leggiByteAt(localFc, 3, 3 + pOffset);
            if (b3[0] > genitore.versioneMassimaFile1 || (b3[0] == genitore.versioneMassimaFile1 && b3[1] > genitore.versioneMassimaFile2))
                throw new FileNonValidoException("");
            if (b3[0] == 0 && ((b3[1] == 2 && b3[2] == 0) || b3[1] < 2))
                throw new FileNonValidoException("");

            info.setVersione1(b3[0]);
            info.setVersione2(b3[1]);
            info.setVersione3(b3[2]);

            int v = leggiIntAt(localFc, 6 + pOffset); // sposta all'inizio delle informazioni sulla versione
            long fPointer = v + pOffset;
            pDati = leggiIntAt(localFc, fPointer);
            fPointer += 4;
            info.setNome(leggiStringaDalCanale(localFc, fPointer));
            fPointer += info.getNome().getBytes(java.nio.charset.StandardCharsets.UTF_8).length + 1;
            info.setAbbreviazione(leggiStringaDalCanale(localFc, fPointer));
            fPointer += info.getAbbreviazione().getBytes(java.nio.charset.StandardCharsets.UTF_8).length + 1;
            info.setTitolo(leggiStringaDalCanale(localFc, fPointer));
            fPointer += info.getTitolo().getBytes(java.nio.charset.StandardCharsets.UTF_8).length + 1;
            info.setAutore(leggiStringaDalCanale(localFc, fPointer));
            fPointer += info.getAutore().getBytes(java.nio.charset.StandardCharsets.UTF_8).length + 1;
            info.setCasaEditrice(leggiStringaDalCanale(localFc, fPointer));
            fPointer += info.getCasaEditrice().getBytes(java.nio.charset.StandardCharsets.UTF_8).length + 1;
            info.setData(leggiStringaDalCanale(localFc, fPointer));
            fPointer += info.getData().getBytes(java.nio.charset.StandardCharsets.UTF_8).length + 1;
            info.setCopyright(leggiStringaDalCanale(localFc, fPointer));
            fPointer += info.getCopyright().getBytes(java.nio.charset.StandardCharsets.UTF_8).length + 1;
            info.setIsbn(leggiStringaDalCanale(localFc, fPointer));
            fPointer += info.getIsbn().getBytes(java.nio.charset.StandardCharsets.UTF_8).length + 1;
            info.setDescrizione(leggiStringaDalCanale(localFc, fPointer));
            fPointer += info.getDescrizione().getBytes(java.nio.charset.StandardCharsets.UTF_8).length + 1;
            info.setLingua(leggiStringaDalCanale(localFc, fPointer));
            fPointer += info.getLingua().getBytes(java.nio.charset.StandardCharsets.UTF_8).length + 1;

            if (leggiByteAt(localFc, 1, fPointer)[0] == 0) info.setTipo(TestoTipi.BIBBIA);
            // else una collezione di note, tipo commentario e/o dizionario e/o libro sarà scelto dopo

            pTesto = leggiIntAt(localFc, pDati + pOffset) + pOffset;
            pIndice = leggiIntAt(localFc, pDati + 4 + pOffset) + pOffset; // indice libri e capitoli per una Bibbia, indice note per una collezione
            pTestoIndice = leggiIntAt(localFc, pDati + 8 + pOffset) + pOffset;
            pParole = leggiIntAt(localFc, pDati + 12 + pOffset) + pOffset;
            pParoleIndiceIndice = leggiIntAt(localFc, pDati + 16 + pOffset) + pOffset;
            pParoleIndice = leggiIntAt(localFc, pDati + 20 + pOffset) + pOffset;
            pRadici = leggiIntAt(localFc, pDati + 24 + pOffset) + pOffset;
            pRadiciDiParole = leggiIntAt(localFc, pDati + 28 + pOffset) + pOffset;
            int pRadiciDiverse = leggiIntAt(localFc, pDati + 32 + pOffset) + pOffset;
            int pRiferimentiDiversi = leggiIntAt(localFc, pDati + 36 + pOffset) + pOffset;

            if (info.getTipo().contains(TestoTipi.BIBBIA)) {
                capitoliInLibroByte = leggiByteAt(localFc, 73, pIndice);
                indiceLibri[0] = 0;
                capitoliInLibro[0] = 0;
                int nCapitoli;
                for (int iLibro = 1; iLibro < 74; ++iLibro) {
                    capitoliInLibro[iLibro] = funzioni.unsignedByte(capitoliInLibroByte[iLibro - 1]);
                    indiceLibri[iLibro] = indiceLibri[iLibro - 1] + capitoliInLibro[iLibro];
                }
                nCapitoli = indiceLibri[73];
                byte[] versettiInCapitoloByte = new byte[nCapitoli];
                versettiInCapitoloByte = leggiByteAt(localFc, nCapitoli, pIndice + 73);
                indiceCapitoli = new int[nCapitoli + 1];
                indiceCapitoli[0] = 0;
                versettiInCapitolo = new int[nCapitoli + 1];
                versettiInCapitolo[0] = 0;
                for (int iCapitolo = 1; iCapitolo <= nCapitoli; ++iCapitolo) {
                    versettiInCapitolo[iCapitolo] = funzioni.unsignedByte(versettiInCapitoloByte[iCapitolo - 1]);
                    indiceCapitoli[iCapitolo] = indiceCapitoli[iCapitolo - 1] + versettiInCapitolo[iCapitolo];
                }
            } else {
                pCitazioniRiferimenti = leggiIntAt(localFc, pDati + 40 + pOffset) + pOffset;
                int pNoteInOrdine = leggiIntAt(localFc, pDati + 44 + pOffset) + pOffset;

                noteTitoli = Arrays.asList(leggiStringhe(pIndice, pTestoIndice - pIndice));
                int numeroNote = noteTitoli.size();
                boolean commentario = (numeroNote == 0); // collezione vuota automaticamente di tutto e due i tipi
                boolean dizionario = (numeroNote == 0);
                for (int i = 0; i < numeroNote; ++i) {
                    if (noteTitoli.get(i).startsWith("#"))
                        commentario = true;
                    else
                        dizionario = true;
                    notePosizione.add(i);
                }
                if (commentario) {
                    if (dizionario) {
                        info.setTipo(EnumSet.of(TestoTipi.COMMENTARIO, TestoTipi.DIZIONARIO));
                    } else {
                        info.setTipo(TestoTipi.COMMENTARIO);
                    }
                } else {
                    //if (dizionario) { // per forza dizionario è vero
                    info.setTipo(TestoTipi.DIZIONARIO);
                    //}
                }

                if (pNoteInOrdine > pOffset) // quando ==pOffset, non ci sono note in ordine
                {
                    int nNoteInOrdine = leggiIntAt(localFc, pNoteInOrdine);
                    // Start the pointer just past the integer we just read (+4 bytes)
                    long ptr = pNoteInOrdine + 4;

                    for (int i = 0; i < nNoteInOrdine; ++i) {
                        // Read the string at the current pointer position
                        String nota = leggiStringaDalCanale(localFc, ptr);
                        noteInOrdine.add(nota);

                        // Move the pointer forward by the actual BYTE length of the string,
                        // plus 1 for the 0 (null) terminator.
                        ptr += nota.getBytes(java.nio.charset.StandardCharsets.UTF_8).length + 1;
                    }

                    if (nNoteInOrdine > 0) {
                        EnumSet<TestoTipi> tt = info.getTipo();
                        tt.add(TestoTipi.LIBRO);
                        info.setTipo(tt);
                    }
                }
            }

            if (pRadiciDiverse > pOffset) {
                int nRadiciDiverse = leggiIntAt(localFc, pRadiciDiverse);
                long ptr = pRadiciDiverse + 4;
                int[] rifRD = new int[6];
                for (int i = 0; i < nRadiciDiverse; ++i) {
                    OccorrenzaParola op = new OccorrenzaParola();
                    if (info.getTipo().contains(TestoTipi.BIBBIA)) {
                        rifRD[0] = leggiIntAt(localFc, ptr);
                        ptr += 4;
                        rifRD[1] = leggiIntAt(localFc, ptr);
                        ptr += 4;
                        rifRD[2] = leggiIntAt(localFc, ptr);
                        ptr += 4;
                        rifRD[3] = rifRD[0];
                        rifRD[4] = rifRD[1];
                        rifRD[5] = rifRD[2];
                        op.setVoce(numeroVersettoDaRiferimento(rifRD)[0]);
                    } else {
                        op.setVoce(leggiIntAt(localFc, ptr));
                        ptr += 4;
                    }
                    op.setParola(leggiIntAt(localFc, ptr));
                    ptr += 4;
                    RadiceDiversa radice = new RadiceDiversa();
                    radice.OccorrenzaRadice = op;

                    String s = leggiStringaDalCanale(localFc, ptr);
                    radice.NuovaRadice = s;
                    ptr += s.getBytes(java.nio.charset.StandardCharsets.UTF_8).length + 1;
                    radiciDiverse.add(radice);
                }
            }

            if (pRiferimentiDiversi > pOffset) {
                int nRiferimentiDiversi = leggiIntAt(localFc, pRiferimentiDiversi);
                // 6 integers * 4 bytes each = 24 bytes per reference
                int bytesToRead = nRiferimentiDiversi * 24;
                // Allocate a buffer big enough for the WHOLE block of data
                ByteBuffer buffer = ByteBuffer.allocate(bytesToRead);
                // Make just ONE file read operation
                localFc.read(buffer, pRiferimentiDiversi + 4);
                buffer.flip(); // Prepare the buffer for reading
                for (int i = 0; i < nRiferimentiDiversi; ++i) {
                    int[] row = new int[6];
                    for (int j = 0; j < 6; j++) {
                        int val = buffer.getInt();
                        if (val > 16000000) {
                            val -= 16777216;
                        }
                        row[j] = val;
                    }
                    riferimentiDiversi.add(row);
                }
            }
        } catch (IOException e) {
            Timber.tag("LaParola").e(e, "Error reading info");
            throw new FileNonValidoException(e.getMessage());
        }
    }

    final void chiudi() {
    }

    // chiude il testo e cancella il file che lo contiene
    void cancella() throws IOException {
        File f = new File(info.getNomeDelFile());
        chiudi();
        if (!f.delete()) throw new IOException();
    }

    private void creaListaRadiceDiParole() throws IOException {
        if (radiceDiParola == null) {
            int numeroParole = parole().length;
            int numeroRadici = radici().length; // serve solo per costringere la lettura delle radici
            radiceDiParola = new int[numeroParole];
            if (numeroRadici > 0 && pRadiciDiParole > 0) { // quando pRadiciDiParole==0 (valore predefinito), non ci sono radici in questa versione
                // numeroRadici>0 quindi non è necessario, ma è incluso per fare sì che la riga che definisce numeroRadici è usata
                try (FileInputStream localInFile = new FileInputStream(percorso); FileChannel localFc = localInFile.getChannel()) {
                    byte[] radiciArray = leggiByteAt(localFc, numeroParole * 4, pRadiciDiParole);

                    int i4;
                    for (int i = 0; i < numeroParole; ++i) {
                        i4 = 4 * i;
                        radiceDiParola[i] = 256 * (256 * (256 * funzioni.unsignedByte(radiciArray[i4]) + funzioni.unsignedByte(radiciArray[i4 + 1])) + funzioni.unsignedByte(radiciArray[i4 + 2])) + funzioni.unsignedByte(radiciArray[i4 + 3]);
                    }
                } catch (IOException e) {
                    Timber.tag("LaParola").e(e, "Errore in creaListaRadiceDiParole");
                }
            }
        }
    }

    private void creaListaCitazioni() throws IOException {
        if (citazioniRiferimenti == null) {
            citazioniRiferimenti = new ArrayList<>();
            if (pCitazioniRiferimenti > pOffset) // quando ==0, non ci sono collegamenti a riferimenti
            {
                CitazioneRiferimento citazione = new CitazioneRiferimento();
                int i10;

                try (FileInputStream localInFile = new FileInputStream(percorso); FileChannel localFc = localInFile.getChannel()) {
                    int nCitazioniRiferimenti = leggiIntAt(localFc, pCitazioniRiferimenti);
                    byte[] citazioniArray = leggiByteAt(localFc, 10 * nCitazioniRiferimenti, pCitazioniRiferimenti);
                    for (int i = 0; i < nCitazioniRiferimenti; ++i) {
                        i10 = 10 * i;
                        citazione.brano = new int[]{citazioniArray[i10], citazioniArray[i10 + 1], citazioniArray[i10 + 2], citazioniArray[i10 + 3], citazioniArray[i10 + 4], citazioniArray[i10 + 5]};
                        citazione.numeroNota = 256 * (256 * (256 * citazioniArray[i10 + 9] + citazioniArray[i10 + 8]) + citazioniArray[i10 + 7]) + citazioniArray[i10 + 6];
                        citazioniRiferimenti.add(citazione);
                    }
                } catch (IOException e) {
                    Timber.tag("LaParola").e(e, "Errore in creaListaCitazioni");
                }
            }
        }
    }

    private String[] leggiStringhe(int inizio, int lunghezza) throws IOException {
        ByteBuffer lista = ByteBuffer.allocateDirect(lunghezza);
        CharBuffer cbuf = lista.asCharBuffer();

        // Open a completely localized stream that auto-closes at the end of the brackets
        try (FileInputStream fis = new FileInputStream(percorso);
             FileChannel localFc = fis.getChannel()) {

            int bytesRead = localFc.read(lista, inizio);

            cbuf.rewind();

            if (bytesRead < lunghezza && bytesRead > 0) {
                cbuf.limit(bytesRead / 2);
            }

            return cbuf.toString().split("\\|");
        }
    }

    public String[] parole() throws IOException {
        if (parole == null) parole = leggiStringhe(pParole, pParoleIndiceIndice - pParole);
        return parole;
    }

    public String[] radici() throws IOException {
        if (radici == null) {
            if (pRadici > pOffset) // quando ==pOffset, non ci sono radici in questa versione
                radici = leggiStringhe(pRadici, pRadiciDiParole - pRadici);
            else radici = new String[0];
        }
        return radici;
    }

    public String getRadice(int i) throws IOException {
        if (radici == null) radici();
        return radici[i];
    }

    public CharSequence getBrano(Riferimento riferimento, Riferimento paroleRicercate) {
        return getBrano(riferimento, paroleRicercate, null, null);
    }

    public CharSequence getBrano(Riferimento riferimento, Riferimento paroleRicercate, String nomeCommentario, Riferimento noteDaVisualizzare) {
        interrompiGetBrano = false;

        int[] riferimentoDaMostrare;
        int nRiferimenti = riferimento.count();
        StringBuilder testoDaVisualizzare = new StringBuilder(8192);
        if (nRiferimenti == 0) {
            return testoDaVisualizzare;
        }

        /*
         * String formatoRiferimento = "", fineFormatoRiferimento = ""; if (genitore.getFormato().getFontRiferimentoGrassetto()) { formatoRiferimento += "<b>";
         * fineFormatoRiferimento = "</b>" + fineFormatoRiferimento; } if (genitore.getFormato().getFontRiferimentoCorsivo()) { formatoRiferimento += "<i>"; fineFormatoRiferimento
         * = "</i>" + fineFormatoRiferimento; } if (genitore.getFormato().getFontRiferimentoSottolineato()) { formatoRiferimento += "<u>"; fineFormatoRiferimento = "</u>" +
         * fineFormatoRiferimento; } if (genitore.getFormato().getRiferimentoApice()) { formatoRiferimento += "<sup>"; fineFormatoRiferimento = "</sup>" + fineFormatoRiferimento; }
         * fineFormatoRiferimento = "</a>" + fineFormatoRiferimento + "&nbsp;";
         */
        final String formatoRiferimento = ""; // ha la classe 'versetto'
        final String fineFormatoRiferimento = "</a>&nbsp;";

        /*
         * String inizioFormatoRicercaNote = "", fineFormatoRicercaNote = ""; // TODO forse anche <(/)lpnparolaricercata> String formatoRicerca = "", fineFormatoRicerca = ""; if
         * (genitore.getFormato().getFontRicercaGrassetto()) { formatoRicerca += "<b>"; fineFormatoRicerca = "</b>" + fineFormatoRicerca; inizioFormatoRicercaNote += "<b>";
         * fineFormatoRicercaNote = "</b>" + fineFormatoRicercaNote; } if (genitore.getFormato().getFontRicercaCorsivo()) { formatoRicerca += "<i>"; fineFormatoRicerca = "</i>" +
         * fineFormatoRicerca; inizioFormatoRicercaNote += "<i>"; fineFormatoRicercaNote = "</i>" + fineFormatoRicercaNote; } if
         * (genitore.getFormato().getFontRicercaSottolineato()) { formatoRicerca += "<u>"; fineFormatoRicerca = "</u>" + fineFormatoRicerca; inizioFormatoRicercaNote += "<u>";
         * fineFormatoRicercaNote = "</u>" + fineFormatoRicercaNote; }
         */
        final String formatoRicerca = "<span class='ricerca'>";
        final String fineFormatoRicerca = "</span>";
        final String inizioFormatoRicercaNote = "<span class='ricerca_note'>";
        final String fineFormatoRicercaNote = "</span>";

        int cap0, cap1, vers0, vers1;
        StringBuilder testoVersetto = new StringBuilder();
        StringBuilder testoVersettoTitolo = new StringBuilder();
        StringBuilder testoVersettoTestoBiblico = new StringBuilder();
        StringBuilder riferimentoVersetto = new StringBuilder();
        StringBuilder posizioneVersetto = new StringBuilder();

        String riferimentoLibro = "";
        String libroPunt, capitoloPunt, libroCapitoloPunt;
        String libroStringa, capitoloStringa, versettoStringa, versettoStringaInTestoNascosto = "";
        String punteggiaturaFraLibroECapitolo = genitore.separatoriNeiRiferimenti()[0];
        String punteggiaturaFraCapitoloEVersetto = genitore.separatoriNeiRiferimenti()[1];
        RiferimentoPosto riferimentoPosto = genitore.getFormato().getRiferimentoPosto();
        TestoVisualizzato testoVisualizzato = genitore.getFormato().getTestoVisualizzato();

        int ultimaParolaRicercata = -1;
        int numeroParoleRicercate = paroleRicercate.count();
        int p, p1, pInizio;
        boolean inizioTitolo;
        int libroDaCercare, capitoloDaCercare;

        if (info.getTipo().contains(TestoTipi.BIBBIA)) {
            try (FileInputStream localInFile = new FileInputStream(percorso);
                 FileChannel localFc = localInFile.getChannel()) {
                for (int iRiferimento = 0; iRiferimento < nRiferimenti; ++iRiferimento) {
                    if (iRiferimento > 0) { // riga vuota fra i brani
                        // a quanto pare Chrome non riesce a gestire un paragrafo lunghissimo,
                        // come ad esempio quello risultante dalla ricerca di "Gesù"
                        /*
                        if (funzioni.endsWith(testoDaVisualizzare, "<br />")) {
                            testoDaVisualizzare.append("<br />");
                        } else {
                            if (testoDaVisualizzare.length() > 0)
                                testoDaVisualizzare.append("<br /><br />");
                        }
                        */
                        if (funzioni.endsWith(testoDaVisualizzare, "<br /></i>")) {
                            testoDaVisualizzare.delete(testoDaVisualizzare.length() - 10, testoDaVisualizzare.length() - 4);
                        }
                        if (funzioni.endsWith(testoDaVisualizzare, "<br />")) {
                            testoDaVisualizzare.delete(testoDaVisualizzare.length() - 6, testoDaVisualizzare.length());
                        }
                        testoDaVisualizzare.append("</p><br/><p>");
                    }
                    riferimentoDaMostrare = riferimento.getBrani().get(iRiferimento);
                    libroDaCercare = riferimentoDaMostrare[0] - 1;
                    if (libroDaCercare >= indiceLibri.length)
                        libroDaCercare = indiceLibri.length - 1;
                    capitoloDaCercare = indiceLibri[libroDaCercare] + riferimentoDaMostrare[1] - 1;
                    if (capitoloDaCercare >= indiceCapitoli.length)
                        capitoloDaCercare = indiceCapitoli.length - 1;
                    pInizio = leggiIntAt(localFc, pTestoIndice + 4L * (indiceCapitoli[capitoloDaCercare] + riferimentoDaMostrare[2] - 1)) + pOffset;

                    // formatoRifPerVersetto = "";
                    testoVersetto.setLength(0);
                    // soloUnVersetto = (riferimentoDaMostrare[0] == riferimentoDaMostrare[3] && riferimentoDaMostrare[1] == riferimentoDaMostrare[4] && riferimentoDaMostrare[2] ==
                    // riferimentoDaMostrare[5]);

                    for (int lib = riferimentoDaMostrare[0]; lib <= riferimentoDaMostrare[3]; ++lib) {
                        if (lib == riferimentoDaMostrare[0]) {
                            cap0 = riferimentoDaMostrare[1];
                        } else {
                            cap0 = 1;
                        }
                        if (lib == riferimentoDaMostrare[3]) {
                            cap1 = riferimentoDaMostrare[4];
                        } else {
                            cap1 = capitoliInLibro[lib];
                        }
                        if (cap1 > capitoliInLibro[lib]) {
                            cap1 = capitoliInLibro[lib];
                        }
                        switch (genitore.getFormato().getRiferimentoFormato()) {
                            case INTERO:
                                riferimentoLibro = genitore.libriNomi[lib];
                                break;
                            case ABBREVIAZIONE:
                                riferimentoLibro = genitore.libriAbbreviazioniUsate[lib];
                                break;
                            case NESSUNO, NESSUN_LIBRO:
                                break;
                            case ABBREVIAZIONE_RICONOSCIUTA:
                                riferimentoLibro = genitore.getLibriAbbreviazioniRiconosciute().Abbreviazione(lib);
                                break;
                        }

                        libroStringa = (lib <= 9 ? "0" + lib : Integer.toString(lib));
                        libroPunt = riferimentoLibro + punteggiaturaFraLibroECapitolo;

                        for (int cap = cap0; cap <= cap1; ++cap) {
                            if (lib > riferimentoDaMostrare[0] && cap == cap0) {
                                // messo qui invece di prima del loop per evitare righe addizionali quando ci sono libri mancanti per es. l'Apocrifa
                                if (!funzioni.endsWith(testoVersetto, "<br />")) {
                                    testoDaVisualizzare.append("<br />");
                                }
                                testoDaVisualizzare.append("<br />");
                                // riga vuota fra i libri
                            }
                            if (lib == riferimentoDaMostrare[0] && cap == riferimentoDaMostrare[1]) {
                                vers0 = riferimentoDaMostrare[2];
                            } else {
                                vers0 = 1;
                            }
                            if (lib == riferimentoDaMostrare[3] && cap == riferimentoDaMostrare[4]) {
                                vers1 = riferimentoDaMostrare[5];
                            } else {
                                vers1 = versettiInCapitolo[indiceLibri[lib - 1] + cap];
                            }
                            if (vers1 > versettiInCapitolo[indiceLibri[lib - 1] + cap]) {
                                vers1 = versettiInCapitolo[indiceLibri[lib - 1] + cap];
                            }
                            capitoloStringa = "00" + cap;
                            capitoloStringa = libroStringa + capitoloStringa.substring(capitoloStringa.length() - 3);
                            if (cap > cap0) {
                                if (!funzioni.endsWith(testoVersetto, "<br />")) {
                                    testoDaVisualizzare.append("<br />");
                                }
                                testoDaVisualizzare.append("<br />");
                                // riga vuota fra capitoli
                            }

                            capitoloPunt = cap + punteggiaturaFraCapitoloEVersetto;
                            libroCapitoloPunt = libroPunt;
                            if (capitoliInLibro[lib] > 1) {
                                libroCapitoloPunt += capitoloPunt;
                            }
                            for (int vers = vers0; vers <= vers1; ++vers) {
                                riferimentoVersetto.setLength(0);
                                riferimentoVersetto.append(formatoRiferimento);

                                if (capitoliInLibro[lib] > 1)
                                    riferimentoVersetto.append(String.format(Locale.getDefault(), "<a class=\"versetto\" href=\"laparola:%s %d#%s_%d_%d\">", genitore.getLibroAbbreviazioneUsata(lib), cap, genitore.getLibroAbbreviazioneUsata(lib), cap, vers));
                                else
                                    riferimentoVersetto.append(String.format(Locale.getDefault(), "<a class=\"versetto\" href=\"laparola:%s#%s_%d_%d\">", genitore.getLibroAbbreviazioneUsata(lib), genitore.getLibroAbbreviazioneUsata(lib), cap, vers));

                                switch (genitore.getFormato().getRiferimentoFormato()) {
                                    case INTERO, ABBREVIAZIONE_RICONOSCIUTA:
                                        riferimentoVersetto.append(libroCapitoloPunt).append(vers);
                                        break;
                                    case ABBREVIAZIONE:
                                        if (vers == vers0) {
                                            riferimentoVersetto.append(libroCapitoloPunt).append(vers);
                                        } else {
                                            riferimentoVersetto.append(vers);
                                        }
                                        break;
                                    case NESSUNO:
                                        break;
                                    case NESSUN_LIBRO:
                                        if (capitoliInLibro[lib] > 1) {
                                            riferimentoVersetto.append(capitoloPunt);
                                        }
                                        riferimentoVersetto.append(vers);
                                        break;
                                }
                                if (genitore.getFormato().getRiferimentoTipo() == RiferimentoTipo.CITAZIONE) {
                                    riferimentoVersetto.append(":");
                                }
                                riferimentoVersetto.append(fineFormatoRiferimento);

                                if (testoDaVisualizzare.length() > 0 && !funzioni.trimEndsWith(testoDaVisualizzare, "<br />") && !funzioni.endsWith(testoDaVisualizzare, " ")) {
                                    testoDaVisualizzare.append(" ");
                                }
                                testoDaVisualizzare.append(versettoStringaInTestoNascosto);

                                versettoStringa = "00" + vers;
                                versettoStringa = capitoloStringa + versettoStringa.substring(versettoStringa.length() - 3);

                                switch (testoVisualizzato) {
                                    case VERSETTI:
                                        testoVersetto = new StringBuilder(leggiStringaDalCanale(localFc, pInizio));
                                        pInizio += testoVersetto.toString().getBytes(java.nio.charset.StandardCharsets.UTF_8).length + 1;
                                        if (!funzioni.trimEndsWith(testoVersetto, "<br />")) {
                                            testoVersetto.append("<br />");
                                        }
                                        break;
                                    case PARAGRAFI:
                                        testoVersetto = new StringBuilder(leggiStringaDalCanale(localFc, pInizio));
                                        pInizio += testoVersetto.toString().getBytes(java.nio.charset.StandardCharsets.UTF_8).length + 1;
                                        break;
                                    case NESSUNO:
                                        testoVersetto.setLength(0);
                                        break;
                                }

                                testoVersettoTitolo.setLength(0);
                                inizioTitolo = false;

                                testoVersettoTestoBiblico.setLength(0);

                                if (testoVersetto.indexOf("<lpt>") == 0) {
                                    p = testoVersetto.indexOf("</lpt>");
                                    if (p > -1) {
                                        if (genitore.getFormato().TitoliVisualizzati()) {
                                            testoVersettoTitolo.append(testoVersetto.substring(0, p + 6));
                                            inizioTitolo = true;
                                        }
                                        testoVersettoTestoBiblico.append(testoVersetto.substring(p + 6));
                                    }
                                } else {
                                    testoVersettoTestoBiblico.append(testoVersetto);
                                }

                                if (!genitore.getFormato().TitoliVisualizzati()) {
                                    while (testoVersettoTestoBiblico.indexOf("<lpt>") >= 0) {
                                        // quando ci sono due titoli in un versetto, come Sal 24 nella CEI
                                        p1 = testoVersettoTestoBiblico.indexOf("<lpt>");
                                        p = testoVersettoTestoBiblico.indexOf("</lpt>");
                                        if (p > -1) testoVersettoTestoBiblico.delete(p1, p + 6);
                                        else
                                            // in questo caso, c'è un errore nel testo
                                            testoVersettoTestoBiblico.delete(p1, p1 + 6);
                                    }
                                }

                                // inserire le note nel posto giusto nel testo
                                if (nomeCommentario != null && !nomeCommentario.isEmpty()) {
                                    String notaStringa;
                                    int numeroNote = noteDaVisualizzare.count();
                                    for (int iNota = numeroNote-1; iNota >=0; --iNota) {
                                        notaStringa = noteDaVisualizzare.getNote().get(iNota);
                                        if (notaStringa.substring(1, 9).equals(versettoStringa) || (notaStringa.startsWith("000", 6) && (notaStringa.substring(1, 6) + "001").equals(versettoStringa)) // nota per tutto il capitolo mostrato all'inizio del primo versetto
                                                || (notaStringa.startsWith("000000", 3) && (notaStringa.substring(1, 3) + "001001").equals(versettoStringa))) // nota per tutto il libro mostrato all'inizio del primo versetto
                                        {
                                            int numeroDellaParola = Integer.parseInt(notaStringa.substring(9, 13));
                                            String link = String.format("<a class='rimando_nota' href='laparola:$%s@%s'>*</a>", notaStringa.replace('#', ';'), nomeCommentario);
                                            modificaFormatoParole(testoVersettoTestoBiblico, numeroDellaParola, "", link, info.getLingua());
                                            // <a class='rimando_nota' href='laparola:Luca 1,1@Note della Nuova Riveduta'>b</a>.
                                        }
                                    }
                                }

                                // indichiamo (di solito con sottolineatura) le parole ricercate
                                if (lib == riferimentoDaMostrare[0] && cap == cap0 && vers == vers0) {
                                    modificaFormatoParole(testoVersettoTestoBiblico, riferimento.getNumeroParola(iRiferimento), formatoRicerca, fineFormatoRicerca, info.getLingua());
                                }
                                for (int numeroParolaRicercata = ultimaParolaRicercata + 1; numeroParolaRicercata < numeroParoleRicercate; ++numeroParolaRicercata) {
                                    if (lib > paroleRicercate.getBrani().get(numeroParolaRicercata)[0]) {
                                        ultimaParolaRicercata = numeroParolaRicercata;
                                    } else if (lib < paroleRicercate.getBrani().get(numeroParolaRicercata)[0]) {
                                        break;
                                    } else if (cap == paroleRicercate.getBrani().get(numeroParolaRicercata)[1] && vers == paroleRicercate.getBrani().get(numeroParolaRicercata)[2]) {
                                        modificaFormatoParole(testoVersettoTestoBiblico, paroleRicercate.getNumeroParola(numeroParolaRicercata), formatoRicerca, fineFormatoRicerca, info.getLingua());
                                    }
                                }

                                // se c'è un titolo, non all'inizio del testo e senza una riga vuota prima, mettiamo la riga vuota
                                if (inizioTitolo && testoDaVisualizzare.length() > 0 && !testoDaVisualizzare.substring(testoDaVisualizzare.length() - 12).equals("<br /><br />"))
                                    testoDaVisualizzare.append("<br />");

                                // A causa di un bug di WebKit non funziona il meccanismo di rilevazione della posizione
                                // verticale dell'elemento se questo è sia inline che vuoto.
                                // Per questo, se non ci fosse il &nbsp; non funzionerebbe nemmeno il meccanismo di
                                // sincronizzazione dei versetti.
                                // Il foglio di stile rende il &nbsp; piccolissimo.
                                posizioneVersetto.setLength(0);
                                posizioneVersetto.append(String.format(Locale.getDefault(), "<a class=\"posizione_versetto\" name=\"%s_%d_%d\">&nbsp;</a>", genitore.getLibroAbbreviazioneUsata(lib), cap, vers));

                                testoDaVisualizzare.append(String.format(Locale.getDefault(), "<span data-versetto=\"%s_%d_%d\">", genitore.getLibroAbbreviazioneUsata(lib), cap, vers));

                                switch (riferimentoPosto) {
                                    case PRIMA_STESSA_RIGA:
                                        testoDaVisualizzare.append(testoVersettoTitolo).append(riferimentoVersetto).append(posizioneVersetto).append(testoVersettoTestoBiblico);
                                        break;
                                    case PRIMA_RIGA_DIVERSA:
                                        testoDaVisualizzare.append(testoVersettoTitolo).append(riferimentoVersetto).append(posizioneVersetto).append("<br />").append(testoVersettoTestoBiblico);
                                        break;
                                    case DOPO:
                                        if (funzioni.endsWith(testoVersettoTestoBiblico, "<br />")) {
                                            testoVersettoTestoBiblico.delete(testoVersettoTestoBiblico.length() - 6, testoVersettoTestoBiblico.length() - 1);
                                            riferimentoVersetto.append("<br />");
                                        }
                                        if (funzioni.endsWith(testoVersettoTestoBiblico, "<br />")) {
                                            testoVersettoTestoBiblico.delete(testoVersettoTestoBiblico.length() - 6, testoVersettoTestoBiblico.length() - 1);
                                            riferimentoVersetto.append("<br />");
                                            if (funzioni.endsWith(testoVersettoTestoBiblico, "<br />")) { // nuovo paragrafo, ma il testo è visualizzato a versetti
                                                testoVersettoTestoBiblico.delete(testoVersettoTestoBiblico.length() - 6, testoVersettoTestoBiblico.length() - 1);
                                                riferimentoVersetto.append("<br />");
                                            }
                                        }
                                        testoDaVisualizzare.append(testoVersettoTitolo).append(posizioneVersetto).append(testoVersettoTestoBiblico).append(" - ").append(riferimentoVersetto);
                                        break;
                                }

                                testoDaVisualizzare.append("</span>");

                                if (interrompiGetBrano) {
                                    return null;
                                }
                            }
                        }
                    }
                }
            } catch (IOException ex) {
                return "";
            }
        } else {
            Riferimento noteDaMostrare = (riferimento.getVersetti() ? elencaNoteInBrano(riferimento) : riferimento);
            String titoloNota, titoloNotaDaLeggere;
            boolean conNomiDelleNote = true;

            String inizioFormatoRiferimento = ""; // "{" + formatoRiferimento + " ";
            // String inizioInizioRiferimento = ""; // TODO
            boolean notaSuBrano;
            int numeroNote = noteDaMostrare.getNote().size();
            for (int i = 0; i < numeroNote; ++i) {
                titoloNota = noteDaMostrare.getNote().get(i);
                notaSuBrano = titoloNota.startsWith("#");
                if (notaSuBrano) {
                    titoloNotaDaLeggere = genitore.convertiTitoloNotaARiferimento(titoloNota);
                    String anchor = titoloNotaDaLeggere;
                    for (String separatore : genitore.separatoriNeiRiferimenti()) {
                        anchor = anchor.replace(separatore, "_");
                    }
                    testoDaVisualizzare.append(String.format("<a class=\"posizione_versetto\" name=\"%s\">&nbsp;</a>", anchor));
                } else {
                    titoloNotaDaLeggere = titoloNota;
                }
                if (conNomiDelleNote) {
                    testoDaVisualizzare.append("<a class=\"titolo_nota\" href=\"laparola:$");
                    testoDaVisualizzare.append(titoloNotaDaLeggere);
                    testoDaVisualizzare.append("\">");
                    testoDaVisualizzare.append(titoloNotaDaLeggere).append("</a>");
                    testoDaVisualizzare.append(inizioFormatoRiferimento);
                }
                String testoModificato = modificaFormatoParole(getNotaConTitolo(titoloNota), noteDaMostrare.getNumeroParola(i), inizioFormatoRicercaNote, fineFormatoRicercaNote, info.getLingua());
                for (int numeroParolaRicercata = ultimaParolaRicercata + 1; numeroParolaRicercata < numeroParoleRicercate; ++numeroParolaRicercata) {
                    switch (noteDaMostrare.getNote().get(i).compareTo(paroleRicercate.getNote().get(numeroParolaRicercata))) {
                        case 1:
                            ultimaParolaRicercata = numeroParolaRicercata;
                            break;
                        case -1:
                            numeroParolaRicercata = numeroParoleRicercate; // finire il loop, non ci sono più note uguali
                            break;
                        case 0:
                            testoModificato = modificaFormatoParole(testoModificato, paroleRicercate.getNumeroParola(numeroParolaRicercata), inizioFormatoRicercaNote, fineFormatoRicercaNote, info.getLingua());
                            break;
                    }
                }
                testoDaVisualizzare.append("<p>").append(testoModificato).append("</p>");

                if (interrompiGetBrano) {
                    return null;
                }
            }
        }

        return testoDaVisualizzare;
    }

    public Riferimento ricercaParolaInBrano(String parola, Riferimento branoDaRicercare) {
        // se branoDaRicerca non contiene brani, tutta la Bibbia (o collezione di note) è ricercata
        if (branoDaRicercare.getBrani().isEmpty()) return ricercaParolaInBrano(parola);
        List<OccorrenzaParola> occorrenze = ricercaParola(parola);
        return restringiRiferimentoABrano(occorrenze, branoDaRicercare);
    }

    public Riferimento ricercaParolaInBrano(String parola) {
        return convertiOccorrenzeARiferimento(ricercaParola(parola));
    }

    private List<OccorrenzaParola> ricercaParola(String parola) {
        String parolaDaRicercare = parola;
        List<OccorrenzaParola> occorrenze = new ArrayList<>();
        try {
            creaListaRadiceDiParole();

            boolean cercaRadice = false, cercaRadiceDiParola = false;

            if (parolaDaRicercare.startsWith("\\")) // tutte le parole con la stessa radice della parola
            {
                cercaRadiceDiParola = true;
                cercaRadice = true; // perché la ricerca sarà convertita in /(radice della parola)
                parolaDaRicercare = parolaDaRicercare.substring(1);
            }
            if (parolaDaRicercare.startsWith("/")) // tutte le parole della radice
            {
                cercaRadice = true;
                parolaDaRicercare = parolaDaRicercare.substring(1);
            }
            if (parolaDaRicercare.contains("*") || parolaDaRicercare.contains("?")) {
                Pattern regExpParola = Pattern.compile("^" + parolaDaRicercare.replace("?", ".").replace("*", ".*") + "$");
                int numeroDiParole = parole().length;
                for (int i = 0; i < numeroDiParole; ++i) {
                    if (regExpParola.matcher(parole()[i]).matches()) {
                        String radiceDaRicercare = parole()[i];
                        if (cercaRadiceDiParola) radiceDaRicercare = getRadice(radiceDiParola[i]);
                        if (cercaRadice) {
                            String[] paroleDaRicercare = paroleNumeriDiRadice(radiceDaRicercare).split("\\|");
                            for (String s : paroleDaRicercare)
                                occorrenze.addAll(occorrenzeParola(Integer.parseInt(s), true));
                            occorrenze.addAll(occorrenzeRadiceDiversa(radiceDaRicercare));
                        } else occorrenze.addAll(occorrenzeParola(i));
                    }
                }
            } else if (!parolaDaRicercare.isEmpty()) {
                if (cercaRadiceDiParola) {
                    if (radici().length > 0) {
                        int numeroParola = numeroDiParola(parolaDaRicercare);
                        if (numeroParola >= 0)
                            parolaDaRicercare = getRadice(radiceDiParola[numeroParola]);
                        else parolaDaRicercare = ""; // parola non esiste in questo testo
                    } else {
                        // cerchiamo "parola" anche quando la ricerca è per \parola
                        cercaRadice = false;
                    }
                }
                if (cercaRadice) {
                    String paroleNumeri = paroleNumeriDiRadice(parolaDaRicercare);
                    if (!paroleNumeri.isEmpty()) {
                        String[] paroleDaRicercare = paroleNumeri.split("\\|");
                        for (String s : paroleDaRicercare)
                            occorrenze.addAll(occorrenzeParola(Integer.parseInt(s), true));
                    }
                    occorrenze.addAll(occorrenzeRadiceDiversa(parolaDaRicercare));
                } else {
                    occorrenze.addAll(occorrenzeParola(numeroDiParola(parolaDaRicercare))); // anche se negativo, funziona perché OccorrenzeParola resitutisce niente
                }
            }
        } catch (IOException e) {
            // se errore quando si legge il file, restituiamo una lista vuota
        }

        Collections.sort(occorrenze);
        return occorrenze;
    }

    private Riferimento restringiRiferimentoABrano(List<OccorrenzaParola> occorrenze, Riferimento branoDaRicercare) {
        Riferimento occorrenzeInBrano = new Riferimento(info.getTipo().contains(TestoTipi.BIBBIA));
        int numeroBrani = branoDaRicercare.getBrani().size();
        for (OccorrenzaParola op : occorrenze) {
            if (occorrenzeInBrano.getVersetti()) {
                List<Integer> inizioBrani = new ArrayList<>();
                List<Integer> fineBrani = new ArrayList<>();
                int[] numeroVersetto;
                for (int[] b : branoDaRicercare.getBrani()) {
                    numeroVersetto = numeroVersettoDaRiferimento(b);
                    inizioBrani.add(numeroVersetto[0]);
                    fineBrani.add(numeroVersetto[1]);
                }
                for (int i = 0; i < numeroBrani; ++i) {
                    if (inizioBrani.get(i) <= op.getVoce() && fineBrani.get(i) >= op.getVoce()) {
                        List<Integer> lista = new ArrayList<>(1);
                        lista.add(op.getParola());
                        occorrenzeInBrano.aggiungiBranoNumeroParola(riferimentoDaNumeroVersetto(op.getVoce()), lista);
                        break;
                    }
                }
            } else {
                String nomeNota;
                int libro, capitolo, versetto;
                for (int i = 0; i < numeroBrani; ++i) {
                    nomeNota = noteTitoli.get(op.getVoce());
                    if (nomeNota.startsWith("#")) // altrimenti fa parte di un dizionario
                    {
                        libro = Integer.parseInt(nomeNota.substring(1, 2));
                        capitolo = Integer.parseInt(nomeNota.substring(3, 3));
                        versetto = Integer.parseInt(nomeNota.substring(6, 3));
                        if ((branoDaRicercare.getBrani().get(i)[0] < libro || (branoDaRicercare.getBrani().get(i)[0] == libro && branoDaRicercare.getBrani().get(i)[1] < capitolo) || (branoDaRicercare.getBrani().get(i)[0] == libro && branoDaRicercare.getBrani().get(i)[1] == capitolo && branoDaRicercare.getBrani().get(i)[2] <= versetto)) && (branoDaRicercare.getBrani().get(i)[3] > libro || (branoDaRicercare.getBrani().get(i)[3] == libro && branoDaRicercare.getBrani().get(i)[4] > capitolo) || (branoDaRicercare.getBrani().get(i)[3] == libro && branoDaRicercare.getBrani().get(i)[4] == capitolo && branoDaRicercare.getBrani().get(i)[5] >= versetto)))
                            try {
                                List<Integer> lista = new ArrayList<>();
                                lista.add(op.getParola());
                                occorrenzeInBrano.aggiungiNotaNumeroParola(noteTitoli.get(op.getVoce()), lista);
                            } catch (Exception e) {
                                // la prima riga sopra può dare un errore se una nota è stata cancellata, e quindi op.Voce>noteTitoli.Count
                            }
                    }
                }
            }
        }
        return occorrenzeInBrano;
    }

    private Riferimento convertiOccorrenzeARiferimento(List<OccorrenzaParola> occorrenze) {
        Riferimento occorrenzeInBibbia = new Riferimento(info.getTipo().contains(TestoTipi.BIBBIA));
        for (OccorrenzaParola op : occorrenze) {
            if (occorrenzeInBibbia.getVersetti()) {
                List<Integer> lista = new ArrayList<>(1);
                lista.add(op.getParola());
                occorrenzeInBibbia.aggiungiBranoNumeroParola(riferimentoDaNumeroVersetto(op.getVoce()), lista);
            } else {
                try {
                    List<Integer> lista = new ArrayList<>(1);
                    lista.add(op.getParola());
                    occorrenzeInBibbia.aggiungiNotaNumeroParola(noteTitoli.get(op.getVoce()), lista);
                } catch (Exception e) {
                    // la prima riga sopra può dare un errore se una nota è stata cancellata, e quindi op.Voce>noteTitoli.Count
                }
            }
        }
        return occorrenzeInBibbia;
    }

    private List<OccorrenzaParola> occorrenzeRadiceDiversa(String radice) {
        // restituisce una lista con tutte le occorrenze di una radice quando non è la radice normale della parola
        List<OccorrenzaParola> occorrenze = new ArrayList<>();
        for (int i = 0; i < radiciDiverse.size(); ++i) {
            if (radiciDiverse.get(i).NuovaRadice.toLowerCase().equals(radice))
                occorrenze.add(radiciDiverse.get(i).OccorrenzaRadice);
        }
        return occorrenze;
    }

    private List<OccorrenzaParola> occorrenzeParola(int nParola, boolean solaRadiceNormale) {
        // restituisce una lista con tutte le occorrenze di una parola; con la radice normale oppure solo quando non c'è una radice diversa
        List<OccorrenzaParola> occorrenze = new ArrayList<>();

        try {
            creaListaRadiceDiParole();

            if (nParola >= 0) {
                try (FileInputStream localInFile = new FileInputStream(percorso); FileChannel localFc = localInFile.getChannel()) {
                    int inizioVersetti = leggiIntAt(localFc, pParoleIndiceIndice + 4L * nParola);
                    int fineVersetti = leggiIntAt(localFc, pParoleIndiceIndice + 4L * nParola + 4);

                    int nByte = fineVersetti - inizioVersetti;
                    byte[] occArray = leggiByteAt(localFc, nByte, pParoleIndice + inizioVersetti);

                    int nOccorrenze = nByte / 6; // 6 perché ogni occorrenza prende 6 byte (UInt32 + UInt16)
                    String radice = "";
                    if (solaRadiceNormale) radice = radiceDiParola(parole[nParola]);
                    for (int i = 0; i < nOccorrenze; ++i) {
                        OccorrenzaParola op = new OccorrenzaParola();
                        op.setVoce((16777216 * funzioni.unsignedByte(occArray[6 * i + 3]) + 65536 * funzioni.unsignedByte(occArray[6 * i + 2]) + 256 * funzioni.unsignedByte(occArray[6 * i + 1]) + funzioni.unsignedByte(occArray[6 * i])));
                        op.setParola((256 * funzioni.unsignedByte(occArray[6 * i + 5]) + funzioni.unsignedByte(occArray[6 * i + 4])));
                        if (!solaRadiceNormale) occorrenze.add(op);
                        else {
                            boolean radiceEDiversa = false;
                            for (int j = 0; j < radiciDiverse.size(); ++j) {
                                if (radiciDiverse.get(j).OccorrenzaRadice.compareTo(op) == 0) {
                                    radiceEDiversa = (!radiciDiverse.get(j).NuovaRadice.equals(radice));
                                    if (radiceEDiversa) break;
                                }
                            }
                            if (!radiceEDiversa) occorrenze.add(op);
                        }
                    }
                }
            }
        } catch (IOException e) {
            Timber.tag("LaParola").e(e, "Error reading occorrenzeParola");
            return occorrenze;
        }

        return occorrenze;
    }

    private List<OccorrenzaParola> occorrenzeParola(int nParola) {
        // restituisce una lista con tutte le occorrenze di una parola
        return occorrenzeParola(nParola, false);
    }

    /*
    public boolean esistonoRadici() {
        try {
            return (radici().length > 0);
        } catch (IOException e) {
            return false;
        }
    }
     */

    private int numeroDiParola(String parola) {
        return parola.isEmpty() ? -1 : Arrays.binarySearch(parole, parola, genitore.confrontoParole);
    }

    /*
    public int numeroVolteParola(String parola) throws IOException {
        int numeroVolte;
        int numeroParola = numeroDiParola(parola);
        if (numeroParola >= 0) {
            try (FileInputStream localInFile = new FileInputStream(percorso); FileChannel localFc = localInFile.getChannel()) {
                int inizioVersetti = leggiIntAt(localFc, pParoleIndiceIndice + 4L * numeroParola);
                numeroVolte = (leggiIntAt(localFc, pParoleIndiceIndice + 4L * numeroParola + 4) - inizioVersetti) / 6;
            }
        } else
            numeroVolte = 0;
        return numeroVolte;
    }
     */

    public String radiceDiParola(String parola) {
        // la radice normale, non un'eventuale radice diversa

        if (radici.length == 0) return "";

        try {
            creaListaRadiceDiParole();
        } catch (IOException e) {
            return "";
        }

        int numeroParola = numeroDiParola(parola);
        return ((numeroParola >= 0) ? radici[radiceDiParola[numeroParola]] : "");
    }

    private String paroleNumeriDiRadice(String radice) {
        // restituisce tutte le parole di una certa radice - restituisce una stringa con i numeri delle parole separati da |
        int numeroRadice = Arrays.binarySearch(radici, radice, genitore.confrontoParole);
        if (numeroRadice < 0) return "";
        if (paroleDiRadice == null) { // siccome la creazione di paroleDiRadice richiede un po' di tempo, lo facciamo solo la prima volta che è necessario
            try {
                creaListaRadiceDiParole();
            } catch (IOException e) {
                return "";
            }

            int numeroRadici = radici.length;
            paroleDiRadice = new StringBuilder[numeroRadici];
            for (int i = 0; i < numeroRadici; ++i)
                paroleDiRadice[i] = new StringBuilder();
            int numeroParole = parole.length;
            for (int i = 0; i < numeroParole; ++i)
                paroleDiRadice[radiceDiParola[i]].append(i).append("|");
        }
        return paroleDiRadice[numeroRadice].toString();
    }

    private static String modificaFormatoParole(StringBuilder testoDaModificare, int numeroParoleDaModificare, String formatoPrimaDellaParola, String formatoDopoLaParola, String lingua) {
        List<Integer> numeriParoleDaModificare = new ArrayList<>(1);
        numeriParoleDaModificare.add(numeroParoleDaModificare);
        return modificaFormatoParole(testoDaModificare, numeriParoleDaModificare, formatoPrimaDellaParola, formatoDopoLaParola, lingua);
    }

    private static String modificaFormatoParole(String testoDaModificare, List<Integer> numeriParoleDaModificare, String formatoPrimaDellaParola, String formatoDopoLaParola, String lingua) {
        return modificaFormatoParole(new StringBuilder(testoDaModificare), numeriParoleDaModificare, formatoPrimaDellaParola, formatoDopoLaParola, lingua);
    }

    private static String modificaFormatoParole(StringBuilder testoDaModificare, List<Integer> numeriParoleDaModificare, String formatoPrimaDellaParola, String formatoDopoLaParola, String lingua) {

        if ((formatoPrimaDellaParola.equals("{") && formatoDopoLaParola.equals("}")) || (formatoPrimaDellaParola.isEmpty() && formatoDopoLaParola.isEmpty()) || (numeriParoleDaModificare.isEmpty())) {
            return testoDaModificare.toString(); // non ci sono modifiche da fare, quindi rimane uguale
        }

        String[] lingue = lingua.toLowerCase().split("\\|");
        String linguaDaUsare, linguaPrincipale = (lingue.length >= 1 ? lingue[0] : "");
        // TODO greco
        // boolean dizionarioGreco = (linguaPrincipale.equals("el") && lingue.length >= 2);
        boolean dizionarioEbraico = (linguaPrincipale.startsWith("he") && lingue.length >= 2);

        int nParoleDaCambiare = numeriParoleDaModificare.size();
        // a volte si chiede che la stessa parola sia modificata 2 volte;
        // non è possibile quindi togliamo i doppioni
        for (int i = nParoleDaCambiare - 1; i >= 1; --i) {
            if (numeriParoleDaModificare.get(i).equals(numeriParoleDaModificare.get(i - 1))) {
                numeriParoleDaModificare.remove(i);
                --nParoleDaCambiare;
            }
        }
        int iParolaDaCambiare = 0;
        int nProssimaParolaDaCambiare = numeriParoleDaModificare.get(0);
        int paroleTrovate = 0;
        StringBuilder parola = new StringBuilder();
        int statoCambiamento = 0; // 0=niente da cambiare, 1=cambiare la
        // prossima, 2=chiudere il cambiamento
        // alla fine di questa parola
        if (nProssimaParolaDaCambiare == 1) {
            statoCambiamento = 1;
            ++iParolaDaCambiare;
            if (iParolaDaCambiare < nParoleDaCambiare) {
                nProssimaParolaDaCambiare = numeriParoleDaModificare.get(iParolaDaCambiare);
            }
        }
        char c;
        boolean analizzaParola;
        int carattereIniziale = 0, carattereDaInserire = 0;
        if (nProssimaParolaDaCambiare == 0) {
            testoDaModificare.insert(carattereDaInserire, formatoPrimaDellaParola + formatoDopoLaParola);
            carattereIniziale += (formatoPrimaDellaParola + formatoDopoLaParola).length();
            ++iParolaDaCambiare;
            if (iParolaDaCambiare < nParoleDaCambiare) {
                nProssimaParolaDaCambiare = numeriParoleDaModificare.get(iParolaDaCambiare);
            }
        }

        linguaDaUsare = linguaPrincipale;
        if (linguaDaUsare.length() > 2) {
            linguaDaUsare = linguaDaUsare.substring(0, 2);
        }
        for (int i = carattereIniziale; i < testoDaModificare.length(); ++i) {
            c = testoDaModificare.charAt(i);
            if (Character.isLetterOrDigit(c)) {
                parola.append(c);
                if (statoCambiamento == 1) {
                    testoDaModificare.insert(i, formatoPrimaDellaParola);
                    i += formatoPrimaDellaParola.length();
                    statoCambiamento = 2;
                }
            } else if (!Character.isLetterOrDigit(c)) {
                analizzaParola = true;
                carattereDaInserire = i;
                if (c == '\'') {
                    switch (linguaDaUsare) {
                        case "en" -> {
                            if ((i == 1 || !Character.isLetterOrDigit(testoDaModificare.charAt(i - 1))) && ((i < testoDaModificare.length() - 1 && (testoDaModificare.charAt(i + 1) == 't' || testoDaModificare.charAt(i + 1) == 'T') && (i == testoDaModificare.length() - 2 || !Character.isLetterOrDigit(testoDaModificare.charAt(i + 2)))) || (i < testoDaModificare.length() - 3 && testoDaModificare.substring(i + 1, i + 4).equalsIgnoreCase("tis") && (i == testoDaModificare.length() - 4 || !Character.isLetterOrDigit(testoDaModificare.charAt(i + 4)))) || (i < testoDaModificare.length() - 4 && testoDaModificare.substring(i + 1, i + 5).equalsIgnoreCase("twas") && (i == testoDaModificare.length() - 5 || !Character.isLetterOrDigit(testoDaModificare.charAt(i + 5)))))) {
                                parola.append(c);
                                analizzaParola = false;
                            } else if (i >= 2) {
                                if (i < testoDaModificare.length() - 1 && (Character.isLetterOrDigit(testoDaModificare.charAt(i - 1)) && Character.isLetter(testoDaModificare.charAt(i + 1)) && (i == testoDaModificare.length() - 2 || !Character.isLetterOrDigit(testoDaModificare.charAt(i + 2))))) {
                                    parola.append(c);
                                    analizzaParola = false;
                                } else if (dizionarioEbraico && i < testoDaModificare.length() - 1 && (Character.isLetter(testoDaModificare.charAt(i - 1)) && testoDaModificare.charAt(i + 1) == '-')) {
                                    // per il dizionario Strong's Hebrew, che ha pronunce come eh'-sheth
                                    parola.append(c);
                                    analizzaParola = false;
                                } else if ((testoDaModificare.charAt(i - 1) == 's' || testoDaModificare.charAt(i - 1) == 'S') && (i == testoDaModificare.length() - 1 || Character.isLetterOrDigit(testoDaModificare.charAt(i + 1))) && Arrays.binarySearch(Testi.PAROLE_INGLESI_SENZA_APOSTROFE, parola.toString()) < 0) {
                                    parola.append(c);
                                    analizzaParola = false;
                                } else if (i < testoDaModificare.length() - 2 && Character.isLetterOrDigit(testoDaModificare.charAt(i - 1)) && (i == testoDaModificare.length() - 3 || !Character.isLetterOrDigit(testoDaModificare.charAt(i + 3))) && (testoDaModificare.substring(i + 1, i + 3).equals("en") || testoDaModificare.substring(i + 1, i + 3).equals("er") || testoDaModificare.substring(i + 1, i + 3).equals("ll") || testoDaModificare.substring(i + 1, i + 3).equals("lt") || testoDaModificare.substring(i + 1, i + 3).equals("ry") || testoDaModificare.substring(i + 1, i + 3).equals("st"))) {
                                    parola.append(c);
                                    analizzaParola = false;
                                } else if (i < testoDaModificare.length() - 4 && Character.isLetterOrDigit(testoDaModificare.charAt(i - 1)) && (i == testoDaModificare.length() - 3 || !Character.isLetterOrDigit(testoDaModificare.charAt(i + 5))) && (testoDaModificare.substring(i + 1, i + 5).equals("ring"))) {
                                    parola.append(c);
                                    analizzaParola = false;
                                }
                            }
                        }
                        case "it" -> {
                            if (i > 0 && i < testoDaModificare.length() - 1) {
                                if ((Character.isLetterOrDigit(testoDaModificare.charAt(i - 1)) && (Character.isLetterOrDigit(testoDaModificare.charAt(i + 1)) || testoDaModificare.charAt(i + 1) == '\'' || testoDaModificare.charAt(i + 1) == '«')) || (Arrays.binarySearch(Testi.PAROLE_ITALIANE_CON_APOSTROFE, parola.toString()) >= 0)) {
                                    // per esempio l'uomo
                                    parola.append(c);
                                }
                            }
                        }
                        case "el" ->
                            /*
                             * TODO greco non funziona con HTML, che usa &#...; per il greco if (i > 0) { if (IsLetteraGreca(testoDaModificare.charAt(i - 1))) parola.append(c); else if
                             * (i < testoDaModificare.length() - 1 && Character.isLetter(testoDaModificare.charAt(i - 1)) && Character.isLetter(testoDaModificare.charAt(i + 1))) {
                             * parola.append(c); analizzaParola = false; } }
                             */
                                parola.append(c);
                        default -> parola.append(c);
                    }
                } else if (c == '[' || c == ']') {
                    if (i > 0 && i < testoDaModificare.length() - 1) {
                        if (Character.isLetter(testoDaModificare.charAt(i - 1)) && Character.isLetter(testoDaModificare.charAt(i + 1))) {
                            // parentesi quadrate in mezzo ad una parola
                            analizzaParola = false;
                        }
                    }
                } else if (c == '-') {
                    if (i > 0 && i < testoDaModificare.length() - 1) {
                        if (((Character.isLetter(testoDaModificare.charAt(i - 1)) || (testoDaModificare.charAt(i - 1) == '?' && i > 1 && Character.isDigit(testoDaModificare.charAt(i - 2)))) && (Character.isLetter(testoDaModificare.charAt(i + 1)))) // per esempio Eben-Ezer ma non 1-2
                                || (dizionarioEbraico && testoDaModificare.charAt(i - 1) == '\'' && Character.isLetter(testoDaModificare.charAt(i + 1))))
                        // per esempio eh'-sheth in Strong's Hebrew
                        {
                            parola.append(c);
                            analizzaParola = false;
                        }
                    }
                } else if (c == '}') {
                    if (i > 0 && i < testoDaModificare.length() - 1) {
                        if (Character.isLetter(testoDaModificare.charAt(i - 1)) && Character.isLetter(testoDaModificare.charAt(i + 1))) {
                            // per esempio una parola parzialmente in italico come {\\i1 del}la
                            analizzaParola = false;
                        }
                    }
                } else if (c == '&') {
                    if (i < testoDaModificare.length() - 1) {
                        if (testoDaModificare.charAt(i + 1) == '#') {
                            int p2Punti = testoDaModificare.indexOf(";", i);
                            if (p2Punti > 0) {
                                parola.append(testoDaModificare.subSequence(i, p2Punti + 1));
                                analizzaParola = false;
                                i = p2Punti;
                            }
                        }
                    }
                } else if (c == '<') {
                    int pAngolo = testoDaModificare.indexOf(">", i);
                    if (pAngolo > 0) i = pAngolo;
                }

                if (parola.length() > 0 && analizzaParola) {
                    if (statoCambiamento == 2) {
                        testoDaModificare.insert(carattereDaInserire, formatoDopoLaParola);
                        i += formatoDopoLaParola.length();
                        statoCambiamento = 0;
                    }
                    ++paroleTrovate;
                    if (paroleTrovate == nProssimaParolaDaCambiare - 1) {
                        statoCambiamento = 1;
                        ++iParolaDaCambiare;
                        if (iParolaDaCambiare < nParoleDaCambiare) {
                            nProssimaParolaDaCambiare = numeriParoleDaModificare.get(iParolaDaCambiare);
                        }
                    }
                    parola.delete(0, parola.length());
                }
            }
        } // for (int iCarattere = 0; iCarattere < testoVersetto.Length; ++iCarattere)
        if (statoCambiamento == 2) {
            testoDaModificare.append(formatoDopoLaParola);
        }
        return testoDaModificare.toString();
    }

    private int[] riferimentoDaNumeroVersetto(int numeroVersetto) {
        int libro = -1;
        int capitolo = -1;
        do ++capitolo; while (indiceCapitoli[capitolo] < numeroVersetto);
        do ++libro; while (indiceLibri[libro] < capitolo);
        int b1 = capitolo - indiceLibri[libro - 1];
        int b2 = numeroVersetto - indiceCapitoli[capitolo - 1];
        return new int[]{libro, b1, b2, libro, b1, b2};
    }

    private int[] numeroVersettoDaRiferimento(int[] riferimento) {
        int inizio, fine;
        int cap1 = riferimento[1];
        if (cap1 > capitoliInLibro[riferimento[0]]) cap1 = capitoliInLibro[riferimento[0]];
        int vers1 = riferimento[2];
        if (vers1 > versettiInCapitolo[indiceLibri[riferimento[0] - 1] + cap1])
            vers1 = versettiInCapitolo[indiceLibri[riferimento[0] - 1] + cap1];
        int cap2 = riferimento[4];
        if (cap2 > capitoliInLibro[riferimento[3]]) cap2 = capitoliInLibro[riferimento[3]];
        int vers2 = riferimento[5];
        if (vers2 > versettiInCapitolo[indiceLibri[riferimento[3] - 1] + cap2])
            vers2 = versettiInCapitolo[indiceLibri[riferimento[3] - 1] + cap2];
        inizio = indiceCapitoli[indiceLibri[riferimento[0] - 1] + cap1 - 1] + vers1;
        fine = indiceCapitoli[indiceLibri[riferimento[3] - 1] + cap2 - 1] + vers2;
        return new int[]{inizio, fine};
    }

    public String getNotaConTitolo(String titolo) {
        if (titolo.isEmpty()) return "";

        // prima cerchiamo la nota con esattamente lo stesso titolo, poi con lettere minuscole
        int numeroNota = Collections.binarySearch(noteTitoli, titolo, genitore.confrontoParole);
        if (numeroNota < 0)
            numeroNota = Collections.binarySearch(noteTitoli, titolo, String.CASE_INSENSITIVE_ORDER);
        if (numeroNota < 0 && !titolo.startsWith("#") && Character.isDigit(titolo.charAt(titolo.length() - 1)))
        // possibilmente una nota ad un versetto, ma nel formato Mt 2:1
        {
            Riferimento noteInBrano = elencaNoteInBrano(genitore.convertiRiferimento(titolo));
            if (noteInBrano.count() > 1) // diverse note nel brano, restituiamo il testo di tutte insieme
                return getBrano(noteInBrano, new Riferimento()).toString();
            if (noteInBrano.count() > 0)
                numeroNota = Collections.binarySearch(noteTitoli, noteInBrano.getNote().get(0), genitore.confrontoParole);
        }

        if (numeroNota < 0) {
            return "";
        }
        if (notePosizione.get(numeroNota) >= 0) {
            String testo;
            try (FileInputStream localInFile = new FileInputStream(percorso); FileChannel localFc = localInFile.getChannel()) {
                long indexOffset = pTestoIndice + 4L * notePosizione.get(numeroNota);
                int textPointer = leggiIntAt(localFc, indexOffset);
                long textOffset = pTesto + textPointer;
                testo = leggiStringaDalCanale(localFc, textOffset);
            } catch (IOException e) {
                testo = "";
            }
            return testo;
        }
        return noteNuoveTesto.get(-notePosizione.get(numeroNota) - 1);
    }

    public Riferimento elencaNoteInBrano(Riferimento riferimento) {
        Riferimento noteInBrano = new Riferimento(false);
        int libroInizio, capitoloInizio, versettoInizio, libroFine, capitoloFine, versettoFine;
        for (String titolo : noteTitoli) {
            if (titolo.startsWith("#")) {
                String[] titoliNote = titolo.split("#");
                for (String titoloNota : titoliNote) {
                    try {
                        libroInizio = Integer.parseInt(titoloNota.substring(0, 2));
                        capitoloInizio = Integer.parseInt(titoloNota.substring(2, 5));
                        versettoInizio = Integer.parseInt(titoloNota.substring(5, 8));
                        libroFine = Integer.parseInt(titoloNota.substring(13, 15));
                        capitoloFine = Integer.parseInt(titoloNota.substring(15, 18));
                        if (capitoloFine == 0) // tutto il libro, quindi dobbiamo garantire che il capitolo cercato sia sempre trovato
                            capitoloFine = Integer.MAX_VALUE;
                        versettoFine = Integer.parseInt(titoloNota.substring(18, 21));
                        if (versettoFine == 0) // tutto il capitolo, quindi dobbiamo garantire che il capitolo cercato sia sempre trovato
                            versettoFine = Integer.MAX_VALUE;
                        for (int[] brano : riferimento.getBrani()) {
                            if ((brano[0] < libroFine || (brano[0] == libroFine && brano[1] < capitoloFine) || (brano[0] == libroFine && brano[1] == capitoloFine && brano[2] <= versettoFine)) && (brano[3] > libroInizio || (brano[3] == libroInizio && brano[4] > capitoloInizio) || (brano[3] == libroInizio && brano[4] == capitoloInizio && brano[5] >= versettoInizio))) {
                                noteInBrano.aggiungiNotaNumeroParola(titolo, new ArrayList<>());
                                break;
                            }
                        }
                    } catch (Exception e) {
                        // se titolo non è nel formato giusto, titolo.Substring può dare errore
                    }
                }
            }
        }
        return noteInBrano;
    }

    public Boolean esisteBrano(Riferimento riferimento) {
        boolean branoEsiste = false;
        int[] branoDaControllare = new int[]{0, 0, 0, 0, 0, 0};

        if (riferimento.getVersetti()) {
            if (info.getTipo().contains(TestoTipi.BIBBIA)) {
                for (int[] brano : riferimento.getBrani()) {
                    System.arraycopy(brano, 0, branoDaControllare, 0, 6);
                    // altrimenti quando brano[] è cambiato, il valore originale nell'argomento viene modificato anche
                    if (indiceLibri[branoDaControllare[0] - 1] != indiceLibri[branoDaControllare[3]]) {
                        if (branoDaControllare[1] == 255) branoDaControllare[1] = 1;
                        if (branoDaControllare[4] == 255) branoDaControllare[4] = 1;
                        if (capitoliInLibro[branoDaControllare[0]] >= branoDaControllare[1] || capitoliInLibro[branoDaControllare[3]] >= branoDaControllare[4]) {
                            // c'è testo nella parte richiesta del primo o dell'ultimo libro
                            branoEsiste = true;
                            break;
                        }
                        if (branoDaControllare[3] > branoDaControllare[0] + 1 && indiceLibri[branoDaControllare[0]] != indiceLibri[branoDaControllare[3] - 1]) {
                            // c'è testo nei libri fra il primo e l'ultimo
                            branoEsiste = true;
                            break;
                        }
                    }
                }
            } else {
                if (elencaNoteInBrano(riferimento).count() > 0) {
                    branoEsiste = true;
                }
            }
        } else // if (riferimento.Versetti)
        {
            for (String nota : riferimento.getNote()) {
                if (!(getNotaConTitolo(nota).isEmpty())) {
                    branoEsiste = true;
                    break;
                }
            }
        }
        return branoEsiste;
    }

    /*
    public Boolean esistonoCitazioni() {
        try {
            creaListaCitazioni();
        } catch (IOException e) {
            // non è stato possibile leggere le citazioni, quindi come se non ci fossero
            return false;
        }
        return (!citazioniRiferimenti.isEmpty());
    }
     */

    public Riferimento citazioni(Riferimento riferimento) {
        Riferimento citazioniTrovate = new Riferimento(false);
        List<Integer> note = new ArrayList<>();
        int numeroBrani = riferimento.count();
        try {
            creaListaCitazioni();
        } catch (IOException e) {
            // non è stato possibile leggere le citazioni, quindi come se non ci fossero
            return citazioniTrovate;
        }
        int numeroCitazioniInCollezione = citazioniRiferimenti.size();
        for (int i = 0; i < numeroBrani; ++i) {
            for (int j = 0; j < numeroCitazioniInCollezione; ++j) {
                if (confrontaBrani(riferimento.getBrani().get(i), citazioniRiferimenti.get(j).brano) == 0) {
                    if (!note.contains(citazioniRiferimenti.get(j).numeroNota))
                        note.add(citazioniRiferimenti.get(j).numeroNota);
                }
            }
        }
        for (int numeroNota : note)
            citazioniTrovate.aggiungiNotaNumeroParola(noteTitoli.get(numeroNota), new ArrayList<>());
        citazioniTrovate.ordinaNote(genitore.confrontoParole);
        return citazioniTrovate;
    }

    // -1 se tutto brano1 è prima di brano2
// 0 se si sovrappongono
// 1 se tutto brano1 è dopo brano2
// brano1/2 sono di 6 byte
    private static int confrontaBrani(int[] brano1, int[] brano2) {
        if (confrontaVersetti(brano1[3], brano1[4], brano1[5], brano2[0], brano2[1], brano2[2]) < 0)
            return -1;
        if (confrontaVersetti(brano1[0], brano1[1], brano1[2], brano2[3], brano2[4], brano2[5]) > 0)
            return 1;
        return 0;
    }

    // -1 se tutto brano1 è prima di brano2
// 0 se si sovrappongono
// 1 se tutto brano1 è dopo brano2
    private static int confrontaVersetti(int libro1, int capitolo1, int versetto1, int libro2, int capitolo2, int versetto2) {
        int confronto = 0;
        if (libro1 < libro2) confronto = -1;
        if (libro1 > libro2) confronto = 1;
        if (confronto == 0) {
            if (capitolo1 < capitolo2) confronto = -1;
            if (capitolo1 > capitolo2) confronto = 1;
        }
        if (confronto == 0) {
            if (versetto1 < versetto2) confronto = -1;
            if (versetto1 > versetto2) confronto = 1;
        }
        return confronto;
    }

    private int leggiIntAt(FileChannel fc, long position) throws IOException {
        // Local buffer: thread-safe and no race conditions
        ByteBuffer buf = ByteBuffer.allocate(4);

        // Positional read: does NOT move the global file pointer
        int bytesRead = fc.read(buf, position);

        if (bytesRead < 4) {
            throw new IOException("Could not read 4 bytes for Integer at position " + position);
        }

        buf.flip(); // Prepare buffer for reading

        // We can use ByteBuffer's built-in math to get the int,
        // or keep your manual math if the byte order is specific.
        // Standard Java/Network order (Big Endian):
        return buf.getInt();
    }

    private byte[] leggiByteAt(FileChannel fc, int numero, long position) throws IOException {
        // Create a local buffer (Thread-Safe)
        ByteBuffer buf = ByteBuffer.allocate(numero);

        // Read exactly numero bytes from the specific position (Thread-Safe)
        int bytesRead = fc.read(buf, position);

        if (bytesRead < numero) {
            throw new IOException("Non è stato possibile leggere " + numero + " byte alla posizione " + position);
        }

        return buf.array(); // Returns a fresh 3-byte array
    }

    private static void accoda_ByteUTF8_A_StringBuffer(byte[] data, int offset, int byteCount, StringBuilder buffer) {
        // preso dal codice sorgente di android-15

        int idx = offset;
        int last = offset + byteCount;

        outer:
        while (idx < last) {
            byte b0 = data[idx++];
            if ((b0 & 0x80) == 0) {
                // 0xxxxxxx
                // Range: U-00000000 - U-0000007F
                int val = b0 & 0xff;
                buffer.append((char) val);
            } else if (((b0 & 0xe0) == 0xc0) || ((b0 & 0xf0) == 0xe0) || ((b0 & 0xf8) == 0xf0) || ((b0 & 0xfc) == 0xf8) || ((b0 & 0xfe) == 0xfc)) {
                int utfCount = 1;
                if ((b0 & 0xf0) == 0xe0) utfCount = 2;
                else if ((b0 & 0xf8) == 0xf0) utfCount = 3;
                else if ((b0 & 0xfc) == 0xf8) utfCount = 4;
                else if ((b0 & 0xfe) == 0xfc) utfCount = 5;

                if (idx + utfCount > last) {
                    buffer.append(REPLACEMENT_CHAR);
                    break;
                }

                // Extract usable bits from b0
                int val = b0 & (0x1f >> (utfCount - 1));
                for (int i = 0; i < utfCount; i++) {
                    byte b = data[idx++];
                    if ((b & 0xC0) != 0x80) {
                        buffer.append(REPLACEMENT_CHAR);
                        idx--; // Put the input char back
                        continue outer;
                    }
                    // Push new bits in from the right side
                    val <<= 6;
                    val |= b & 0x3f;
                }

                // Allow surrogate values (0xD800 - 0xDFFF) to
                // be specified using 3-byte UTF values only
                if ((utfCount != 2) && (val >= 0xD800) && (val <= 0xDFFF)) {
                    buffer.append(REPLACEMENT_CHAR);
                    continue;
                }

                // Reject chars greater than the Unicode maximum of U+10FFFF.
                if (val > 0x10FFFF) {
                    buffer.append(REPLACEMENT_CHAR);
                    continue;
                }

                // Encode chars from U+10000 up as surrogate pairs
                if (val < 0x10000) {
                    buffer.append((char) val);
                } else {
                    int x = val & 0xffff;
                    int u = (val >> 16) & 0x1f;
                    int w = (u - 1) & 0xffff;
                    int hi = 0xd800 | (w << 6) | (x >> 10);
                    int lo = 0xdc00 | (x & 0x3ff);
                    buffer.append((char) hi);
                    buffer.append((char) lo);
                }
            } else {
                // Illegal values 0x8*, 0x9*, 0xa*, 0xb*, 0xfd-0xff
                buffer.append(REPLACEMENT_CHAR);
            }
        }
    }

    /**
     * Reads a null-terminated (or 0-terminated) UTF-8 string from a specific
     * position in the FileChannel without affecting the channel's global position.
     */
    private String leggiStringaDalCanale(FileChannel fc, long offset) throws IOException {
        StringBuilder buffer = new StringBuilder();
        int LEGGISTRINGA_BUFFERLEN = 1024;
        ByteBuffer byteBuf = ByteBuffer.allocate(LEGGISTRINGA_BUFFERLEN);
        byte[] localArray = new byte[LEGGISTRINGA_BUFFERLEN];

        long currentOffset = offset;
        boolean endReached = false;

        while (!endReached) {
            byteBuf.clear();
            // positional read: does NOT move fc.position()
            int bytesRead = fc.read(byteBuf, currentOffset);

            if (bytesRead <= 0) break; // End of file or error

            byteBuf.flip();
            int p = 0;
            for (int i = 0; i < bytesRead; i++) {
                byte b = byteBuf.get();
                if (b == 0) { // Found our string terminator
                    endReached = true;
                    break;
                }
                localArray[p++] = b;

                // If our local array is full, flush it to the StringBuilder
                if (p == LEGGISTRINGA_BUFFERLEN) {
                    // Handle UTF-8 multibyte character splitting at buffer boundaries
                    int m = p - 1;
                    while (m >= 0 && (localArray[m] & 0x80) != 0 && (localArray[m] & 0x40) == 0) {
                        m--; // Back up if we are in the middle of a UTF-8 sequence
                    }
                    // If it's a start byte, we also need to check it
                    if (m >= 0 && (localArray[m] & 0x80) != 0) m--;

                    int lenToAppend = m + 1;
                    accoda_ByteUTF8_A_StringBuffer(localArray, 0, lenToAppend, buffer);

                    // Move leftovers to the start for next iteration
                    int leftovers = p - lenToAppend;
                    System.arraycopy(localArray, lenToAppend, localArray, 0, leftovers);
                    p = leftovers;
                }
            }

            accoda_ByteUTF8_A_StringBuffer(localArray, 0, p, buffer);
            currentOffset += bytesRead;
            if (endReached) break;
        }

        return buffer.toString();
    }
}

