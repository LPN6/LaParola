package net.laparola.core;

import net.laparola.core.Testi.*;

public class FormatoTesto {
	private TestoVisualizzato testoVisualizzato;
	private RiferimentoTipo riferimentoTipo;
	private RiferimentoFormato riferimentoFormato;
	private RiferimentoPosto riferimentoPosto;
	private boolean fontRiferimentoGrassetto;
	private boolean fontRiferimentoCorsivo;
	private boolean fontRiferimentoSottolineato;
	private boolean fontRicercaGrassetto;
	private boolean fontRicercaCorsivo;
	private boolean fontRicercaSottolineato;
	private boolean riferimentoApice;
	private boolean titoliVisualizzati;

	public TestoVisualizzato getTestoVisualizzato() {
		return testoVisualizzato;
	}

	public void setTestoVisualizzato(TestoVisualizzato valore) {
		testoVisualizzato = valore;
	}

	public RiferimentoTipo getRiferimentoTipo() {
		return riferimentoTipo;
	}

	public void setRiferimentoTipo(RiferimentoTipo valore) {
		riferimentoTipo = valore;
	}

	public RiferimentoFormato getRiferimentoFormato() {
		return riferimentoFormato;
	}

	public void setRiferimentoFormato(RiferimentoFormato valore) {
		riferimentoFormato = valore;
	}

	public RiferimentoPosto getRiferimentoPosto() {
		return riferimentoPosto;
	}

	public void setRiferimentoPosto(RiferimentoPosto valore) {
		riferimentoPosto = valore;
	}

	public boolean getFontRiferimentoGrassetto() {
		return fontRiferimentoGrassetto;
	}

	public void setFontRiferimentoGrassetto(boolean valore) {
		fontRiferimentoGrassetto = valore;
	}

	public boolean getFontRiferimentoCorsivo() {
		return fontRiferimentoCorsivo;
	}

	public void setFontRiferimentoCorsivo(boolean valore) {
		fontRiferimentoCorsivo = valore;
	}

	public boolean getFontRiferimentoSottolineato() {
		return fontRiferimentoSottolineato;
	}

	public void setFontRiferimentoSottolineato(boolean valore) {
		fontRiferimentoSottolineato = valore;
	}

	public boolean getFontRicercaGrassetto() {
		return fontRicercaGrassetto;
	}

	public void setFontRicercaGrassetto(boolean valore) {
		fontRicercaGrassetto = valore;
	}

	public boolean getFontRicercaCorsivo() {
		return fontRicercaCorsivo;
	}

	public void setFontRicercaCorsivo(boolean valore) {
		fontRicercaCorsivo = valore;
	}

	public boolean getFontRicercaSottolineato() {
		return fontRicercaSottolineato;
	}

	public void setFontRicercaSottolineato(boolean valore) {
		fontRicercaSottolineato = valore;
	}

	public boolean getRiferimentoApice() {
		return riferimentoApice;
	}

	public void setRiferimentoApice(boolean valore) {
		riferimentoApice = valore;
	}

	public boolean TitoliVisualizzati() {
		return titoliVisualizzati;
	}

	public void setTitoliVisualizzati(boolean valore) {
		titoliVisualizzati = valore;
	}

	public FormatoTesto() {
		// fontNome = "Times New Roman";
		// fontDimensione = 12;
		// fontColore = Color.Black;

		// fontRiferimentoNome = fontNome;
		// fontRiferimentoDimensione = 12;
		fontRiferimentoGrassetto = true;
		fontRiferimentoCorsivo = false;
		fontRiferimentoSottolineato = false;
		riferimentoApice = true;
		titoliVisualizzati = true;
		// riferimentoContestoRicerche = false;
		// fontRiferimentoColore = Color.Black;

		// fontRicercaNome = fontNome;
		// fontRicercaDimensione = 12;
		fontRicercaGrassetto = false;
		fontRicercaCorsivo = false;
		fontRicercaSottolineato = true;
		// fontRicercaColore = Color.Black;

		riferimentoTipo = RiferimentoTipo.DUE_PUNTI;
		riferimentoFormato = RiferimentoFormato.ABBREVIAZIONE;
		riferimentoPosto = RiferimentoPosto.PRIMA_STESSA_RIGA;
		testoVisualizzato = TestoVisualizzato.PARAGRAFI;
	}

	// Copia tutte le caratteristiche di un formato ad un altro.
	// formato: Il formato a cui copiare le caratteristiche.
	public void copiaA(FormatoTesto formato) {
		// formato.fontNome = fontNome;
		// formato.fontDimensione = fontDimensione;
		// formato.fontGrassetto = fontGrassetto;
		// formato.fontCorsivo = fontCorsivo;
		// formato.fontSottolineato = fontSottolineato;
		// formato.fontColore = fontColore;

		// formato.fontGrecoNome = fontGrecoNome; formato.fontGrecoDimensione = fontGrecoDimensione; formato.fontGrecoColore = fontGrecoColore;

		// formato.fontEbraicoNome = fontEbraicoNome; formato.fontEbraicoDimensione = fontEbraicoDimensione; formato.fontEbraicoColore = fontEbraicoColore;

		// formato.fontRiferimentoNome = fontRiferimentoNome;
		// formato.fontRiferimentoDimensione = fontRiferimentoDimensione;
		formato.setFontRiferimentoGrassetto(fontRiferimentoGrassetto);
		formato.setFontRiferimentoCorsivo(fontRiferimentoCorsivo);
		formato.setFontRiferimentoSottolineato(fontRiferimentoSottolineato);
		// formato.fontRiferimentoColore =fontRiferimentoColore;
		formato.setRiferimentoApice(riferimentoApice);
		// formato.riferimentoContestoRicerche = riferimentoContestoRicerche;

		// formato.fontRicercaNome = fontRicercaNome;
		// formato.fontRicercaDimensione = fontRicercaDimensione;
		formato.setFontRicercaGrassetto(fontRicercaGrassetto);
		formato.setFontRicercaCorsivo(fontRicercaCorsivo);
		formato.setFontRicercaSottolineato(fontRicercaSottolineato);
		// formato.fontRicercaColore = fontRicercaColore;

		formato.setTitoliVisualizzati(titoliVisualizzati);
		formato.setRiferimentoTipo(riferimentoTipo);
		formato.setRiferimentoFormato(riferimentoFormato);
		formato.setRiferimentoPosto(riferimentoPosto);
		formato.setTestoVisualizzato(testoVisualizzato);
	}
}

/*
 * 
 * 
 * #region FontPredef
 * 
 * private string fontNome; /// <summary> /// Il nome del font predefinito. /// </summary> public string FontNome { get { return fontNome; } set { fontNome = value; } }
 * 
 * private float fontDimensione; /// <summary> /// La dimensione del font predefinito. /// </summary> public float FontDimensione { get { return fontDimensione; } set {
 * fontDimensione = value; } }
 * 
 * private bool fontGrassetto; /// <summary> /// Se il font predefinito è in grassetto. /// </summary> public bool FontGrassetto { get { return fontGrassetto; } set { fontGrassetto
 * = value; } } private bool fontCorsivo; /// <summary> /// Se il font predefinito è in corsivo. /// </summary> public bool FontCorsivo { get { return fontCorsivo; } set {
 * fontCorsivo = value; } } private bool fontSottolineato; /// <summary> /// Se il font predefinito è sottolineato. /// </summary> public bool FontSottolineato { get { return
 * fontSottolineato; } set { fontSottolineato = value; } }
 * 
 * private Color fontColore; /// <summary> /// Il colore del font. /// </summary> public Color FontColore { get { return fontColore; } set { fontColore = value; } }
 * 
 * #endregion
 * 
 * #region FontRiferimento
 * 
 * private string fontRiferimentoNome; /// <summary> /// Il nome del font usato per i riferimenti. /// </summary> public string FontRiferimentoNome { get { return
 * fontRiferimentoNome; } set { fontRiferimentoNome = value; } }
 * 
 * private float fontRiferimentoDimensione; /// <summary> /// La dimensione del font usato per i riferimenti. /// </summary> public float FontRiferimentoDimensione { get { return
 * fontRiferimentoDimensione; } set { fontRiferimentoDimensione = value; } }
 * 
 * private Color fontRiferimentoColore; /// <summary> /// Il colore del font usato per i riferimenti. /// </summary> public Color FontRiferimentoColore { get { return
 * fontRiferimentoColore; } set { fontRiferimentoColore = value; } }
 * 
 * private bool riferimentoContestoRicerche; /// <summary> /// Se un collegamento ipertestuale è creato per i riferimenti in una ricerca. /// </summary> public bool
 * RiferimentoContestoRicerche { get { return riferimentoContestoRicerche; } set { riferimentoContestoRicerche = value; } }
 * 
 * #endregion
 * 
 * #region FontRicerca
 * 
 * private string fontRicercaNome; /// <summary> /// Il nome del font usato per le parole ricercate. /// </summary> public string FontRicercaNome { get { return fontRicercaNome; }
 * set { fontRicercaNome = value; } }
 * 
 * private float fontRicercaDimensione; /// <summary> /// La dimensione del font usato per le parole ricercate. /// </summary> public float FontRicercaDimensione { get { return
 * fontRicercaDimensione; } set { fontRicercaDimensione = value; } }
 * 
 * private Color fontRicercaColore; /// <summary> /// Il colore del font usato per le parole ricercate. /// </summary> public Color FontRicercaColore { get { return
 * fontRicercaColore; } set { fontRicercaColore = value; } }
 * 
 * #endregion
 */