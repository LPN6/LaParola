package net.laparola.core;

import java.util.EnumSet;

import net.laparola.core.Testi.TestoTipi;

public class VersioneInformazioni {
	private int versione1;

	public int getVersione1() {
		return versione1;
	}

	public void setVersione1(int valore) {
		versione1 = valore;
	}

	private int versione2;

	public int getVersione2() {
		return versione2;
	}

	public void setVersione2(int valore) {
		versione2 = valore;
	}

	private int versione3;

	public int getVersione3() {
		return versione3;
	}

	public void setVersione3(int valore) {
		versione3 = valore;
	}

	private String nomeDelFile; // nome e percorso

	public String getNomeDelFile() {
		return nomeDelFile;
	}

	public void setNomeDelFile(String valore) {
		nomeDelFile = valore;
	}

	private String nome;

	public String getNome() {
		return nome;
	}

	public void setNome(String valore) {
		nome = valore;
	}

	private String abbreviazione;

	public String getAbbreviazione() {
		return abbreviazione;
	}

	public void setAbbreviazione(String valore) {
		abbreviazione = valore;
	}

	private String titolo;

	public String getTitolo() {
		return titolo;
	}

	public void setTitolo(String valore) {
		titolo = valore;
	}

	private String autore;

	public String getAutore() {
		return autore;
	}

	public void setAutore(String valore) {
		autore = valore;
	}

	private String casaEditrice;

	public String getCasaEditrice() {
		return casaEditrice;
	}

	public void setCasaEditrice(String valore) {
		casaEditrice = valore;
	}

	private String data;

	public String getData() {
		return data;
	}

	public void setData(String valore) {
		data = valore;
	}

	private String copyright;

	public String getCopyright() {
		return copyright;
	}

	public void setCopyright(String valore) {
		copyright = valore;
	}

	private String isbn;

	public String getIsbn() {
		return isbn;
	}

	public void setIsbn(String valore) {
		isbn = valore;
	}

	private String descrizione;

	public String getDescrizione() {
		return descrizione;
	}

	public void setDescrizione(String valore) {
		descrizione = valore;
	}

	private String lingua;

	// un codice ISO 639-1 (2 lettere) oppure ISO 639-2 (3 lettere)
	public String getLingua() {
		return lingua;
	}

	public void setLingua(String valore) {
		lingua = valore;
	}

	private long dimensione;

	public long getDimensione() {
		return dimensione;
	}

	public void setDimensione(long valore) {
		dimensione = valore;
	}

	public VersioneInformazioni() {
		versione1 = 0;
		versione2 = 0;
		versione3 = 0;
		nomeDelFile = "";
		nome = "";
		abbreviazione = "";
		titolo = "";
		autore = "";
		casaEditrice = "";
		data = "";
		copyright = "";
		isbn = "";
		descrizione = "";
		lingua = "";
		dimensione = 0;
	}

	private EnumSet<TestoTipi> tipo = EnumSet.of(TestoTipi.NESSUNO);

	public EnumSet<TestoTipi> getTipo() {
		return tipo;
	}

	public void setTipo(EnumSet<TestoTipi> valore) {
		tipo = valore;
	}

	public void setTipo(TestoTipi valore) {
		tipo = EnumSet.of(valore);
	}

	public String getVersione() {
		return getVersione1() + "." + getVersione2() + "." + getVersione3();
	}
}
