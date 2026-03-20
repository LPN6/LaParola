package net.laparola.ui;

public interface LaParolaJavascriptInterface {
	String getSegnalibriCasuali(int n);
	String getVersettoCasuale();
	String getVersettoCasuale(int minlibro, int maxlibro);
	String getVersioneProgramma();
	String normalizzaRiferimento(String riferimento, String versione);
	String normalizzaRiferimento(String riferimento);
	String convertiRiferimentoAStandardVirgola(String riferimento, String versione);
	String getVersione();
	boolean getAggionamentiDisponibiliDebole();
	void notificaPrimoSegnalibroVisible(String s);
	void cambiaEvidenziatore(final String versetto);
	long getUltimaDataLiturgia();
	void sceltaDataLiturgia();
	boolean isRiferimento(String rif);
    void scriviFile(String nome, String contenuto);
    String leggiFile(String nome);
    void toccoLungoSuSfondo();
    void logd(String s);
}
