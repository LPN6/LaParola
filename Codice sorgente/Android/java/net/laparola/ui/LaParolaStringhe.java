package net.laparola.ui;

import java.lang.reflect.Field;
import java.lang.reflect.Modifier;

/* internal */ class LaParolaStringhe {
	public static final int ERRORE_NESSUNA_VERSIONE = 0;
	public static final int ERRORE_BRANO_NON_PRESENTE = 1;
	public static final int ERRORE_VERSIONE_NON_PRESENTE = 2;
	public static final int VISUALIZZA_BRANO_IN = 17;
	public static final int ERRORE_NESSUN_VERSETTO_EVIDENZIATO = 30;
	
	public static final int ERRORE_RICERCA_ESPRESSIONE_VUOTA = 3;
	public static final int ERRORE_RICERCA_ERRORE_SINTASSI = 4;
	public static final int ERRORE_RICERCA_ERRORE_SINTASSI_PROPONI_RIFERIMENTO = 26;
	public static final int ERRORE_RICERCA_PARENTESI = 5;
	public static final int ERRORE_RICERCA_PARENTESI_QUADRATE = 6;
	public static final int ERRORE_RICERCA = 7;
	public static final int ERRORE_RICERCA_PROPONI_RIFERIMENTO = 25;
	
	public static final int HTML_HEADER = 8;
	public static final int HTML_FOOTER = 9;
	
	public static final int DESCRIZIONE_URL_RICERCA = 10;
	public static final int DESCRIZIONE_URL_RICERCA_IN = 11;
	public static final int VERSIONE_CON_COMMENTARIO = 32;
	
	public static final int MOSTRA_CAPITOLO_INTERO_INIZIO = 12;
	public static final int MOSTRA_CAPITOLO_INTERO_FINE = 13;
	public static final int MOSTRA_CAPITOLO_PRECEDENTE = 14;
	public static final int MOSTRA_CAPITOLO_SUCCESSIVO = 15;
	public static final int ANCHOR_INIZIO = 16;
	public static final int ANCHOR_FINE = 28;
	
	public static final int DESCRIZIONE_URL_BRANI = 18;
	public static final int DESCRIZIONE_URL_GRUPPI_SEGNALIBRI = 19;
	public static final int DESCRIZIONE_URL_GRUPPO_SEGNALIBRI = 20;
	public static final int DESCRIZIONE_URL_FILE = 21;
	public static final int DESCRIZIONE_URL_EVIDENZIATI = 29;
	public static final int DESCRIZIONE_URL_CRONOLOGIA = 34;
    public static final int DESCRIZIONE_URL_PREFERITI =37 ;

	public static final int CRONOLOGIA = 22;
	public static final int PULISCI_CRONOLOGIA = 23;
	public static final int CRONOLOGIA_HEADER = 24;
    public static final int ERRORE_URL = 27;

    public static final int PREFERITI_VUOTO = 35;
    public static final int ELIMINA = 36;

	public static final int NESSUNA_NOTA = 31;
	public static final int ELENCO_NOTE = 33;

    // riprendi da 38
		
	private static String[] stringhe;

	private LaParolaStringhe () {}
	
	static {
		int numero_stringhe = 0;
		Class<? extends LaParolaStringhe> clazz = LaParolaStringhe.class;
		for (Field f : clazz.getFields()) {
			int modifiers = f.getModifiers();
			boolean isStatic = Modifier.isStatic(modifiers);
			boolean isPublic = Modifier.isPublic(modifiers);
			boolean isFinal = Modifier.isFinal(modifiers);
			boolean isInt = (f.getType() == int.class);
			if (isPublic && isStatic && isFinal && isInt) {
				numero_stringhe++;
			}
		}
		
		stringhe = new String[numero_stringhe];
			
		stringhe[ERRORE_NESSUNA_VERSIONE]           = "Nessun testo della Bibbia è stata installato. <a href='lpcomando:versioni'>Scaricane uno.</a>";
		stringhe[ERRORE_BRANO_NON_PRESENTE]         = "Il brano richiesto non è presente nel testo in uso (%s).";
		stringhe[ERRORE_VERSIONE_NON_PRESENTE]      = "Il testo richiesto (%s) non è presente.<br/><a href='lpcomando:versioni'>Installa testo.</a>";
		stringhe[VISUALIZZA_BRANO_IN]               = "<br/><a href='%1$s'>Visualizza brano da %2$s.</a>";
		
		stringhe[ERRORE_RICERCA_ESPRESSIONE_VUOTA]   = "L'espressione da ricerca è vuota; digita quello che vuoi cercare.<br/><a href='lpcomando:aiuto_ricerca'>Mostra guida.</a>";
		stringhe[ERRORE_RICERCA_ERRORE_SINTASSI]     = "Errore di sintassi nell'espressione da ricercare al carattere numero %s.<br/><a href='lpcomando:aiuto_ricerca'>Mostra guida.</a>";
		stringhe[ERRORE_RICERCA_ERRORE_SINTASSI_PROPONI_RIFERIMENTO] = "Errore di sintassi nell'espressione da ricercare al carattere numero %1$s.<br/>Intendevi forse andare al <a href='laparola:%2$s'>riferimento %2$s</a>?<br/><br/><a href='lpcomando:aiuto_ricerca'>Mostra guida.</a>";
		stringhe[ERRORE_RICERCA_PARENTESI]           = "Errore nell'espressione da ricercare: le parentesi non corrispondono.<br/><a href='lpcomando:aiuto_ricerca'>Mostra guida.</a>";
		stringhe[ERRORE_RICERCA_PARENTESI_QUADRATE]  = "Errore nell'espressione da ricercare: le parentesi quadrate non corrispondono.<br/><a href='lpcomando:aiuto_ricerca'>Mostra guida.</a>";
        stringhe[ERRORE_RICERCA]                     = "Errore nella ricerca.<br/><a href='lpcomando:aiuto_ricerca'>Mostra guida.</a>";
		stringhe[ERRORE_RICERCA_PROPONI_RIFERIMENTO] = "Errore nella ricerca.<br/>Intendevi forse andare al <a href='laparola:%1$s'>riferimento %1$s</a>?<br/><br/><a href='lpcomando:aiuto_ricerca'>Mostra guida.</a>";
                
        stringhe[HTML_HEADER] =
            "<html>\n" +
            "  <head>\n" + 
            "    %s" +
            "  </head>\n"+
            "  <body>\n" +
            "    <span id='bodystart'></span>\n" +
            "    <p>\n";
        stringhe[HTML_FOOTER] = 
        	"    </p>\n" + 
    		"  </body>\n" + 
        	"</html>\n";
        
        stringhe[DESCRIZIONE_URL_RICERCA]           = "Ricerca di \"%1$s\" (%2$s)";
        stringhe[DESCRIZIONE_URL_RICERCA_IN]        = "Ricerca di \"%1$s\" in %2$s (%3$s)";
        stringhe[DESCRIZIONE_URL_BRANI]             = "%1$s (%2$s)";
        stringhe[DESCRIZIONE_URL_GRUPPI_SEGNALIBRI] = "Segnalibri";
        stringhe[DESCRIZIONE_URL_GRUPPO_SEGNALIBRI] = "%1$s (%2$s)";
        stringhe[DESCRIZIONE_URL_FILE]              = "%1$s (%2$s)";
        stringhe[DESCRIZIONE_URL_EVIDENZIATI]       = "Lista dei versetti evidenziati";
        stringhe[DESCRIZIONE_URL_CRONOLOGIA]        = "Cronologia";
        stringhe[DESCRIZIONE_URL_PREFERITI]         = "Preferiti";

        stringhe[MOSTRA_CAPITOLO_INTERO_INIZIO] = "</p><p align='center'><a href='%s'>Mostra dall'inizio del capitolo</a></p>\n<p>";
        stringhe[MOSTRA_CAPITOLO_INTERO_FINE]   = "</p><p align='center'><a href='%s'>Mostra fino alla fine del capitolo</a></p>\n<p>";
        stringhe[MOSTRA_CAPITOLO_PRECEDENTE]    = "</p><p align='center'><a href='%s'>Vai al capitolo precedente</a></p>\n<p>";
        stringhe[MOSTRA_CAPITOLO_SUCCESSIVO]    = "</p><p align='center'><a href='%s'>Vai al capitolo successivo</a></p>\n<p>";
        
        stringhe[CRONOLOGIA] = "Cronologia";
        stringhe[PULISCI_CRONOLOGIA] = " <a href='lpcomando:pulisci_cronologia'>cancella tutto</a>";
        stringhe[CRONOLOGIA_HEADER] = "<link rel='stylesheet' type='text/css' href='%s'>\n";

        stringhe[ANCHOR_INIZIO] = "<a class=\"posizione_versetto\" name='inizio'>&nbsp;</a>";
        stringhe[ANCHOR_FINE] = "<a class=\"posizione_versetto\" name='fine'>&nbsp;</a>";
        
        stringhe[ERRORE_URL] = "<html><body>Impossibile visualizzare la pagina %1$s.</body></html>";
        
        stringhe[ERRORE_NESSUN_VERSETTO_EVIDENZIATO] = "Non è stato evidenziato nessun versetto.<br/>Per evidenziare i versetti, selezionare di nuovo \"Evidenziatore\" quando sono visualizzati.";
        
        stringhe[NESSUNA_NOTA] = "Non è presente nessuna nota nel testo %s.";
        stringhe[ELENCO_NOTE] = "Elenco note";

        stringhe[VERSIONE_CON_COMMENTARIO] = "%s con commentario %s";

        stringhe[PREFERITI_VUOTO] = "Non ci sono preferiti. Per aggiungerne, toccare la stella durante la visualizzazione del brano.";
        stringhe[ELIMINA] = "elimina";
    }
	
	public static String get (int stringa) {
		return stringhe[stringa];
	}
	
	public static String get (int stringa, Object... args) {
		return String.format(stringhe[stringa], args);
	}
	
	public static void set (int stringa, String valore) {
		stringhe[stringa] = valore;
	}
}

/*
 * CSS per font in asset
 * non funziona su android 2.1
 * 
 
@font-face {
    font-family: MyFont;
    src: url("file:///android_asset/fonts/MyFont.otf")
}
body {
    font-family: MyFont;
    font-size: medium;
    text-align: justify;
}

*/
