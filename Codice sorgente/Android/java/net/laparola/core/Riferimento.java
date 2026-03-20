package net.laparola.core;

import java.text.Collator;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

public class Riferimento {

	private List<int[]> brani;
	private List<List<Integer>> numeroParola;
	private boolean versetti = true;
	private boolean daTradurre = false;
	private List<String> note;

	public Riferimento() {
		costruttoreComune();
	}

	public Riferimento(int libro, int capitolo, int versetto) {
		costruttoreComune();
		brani.add(new int[] { libro, capitolo, versetto, libro, capitolo, versetto });
		numeroParola.add(new ArrayList<Integer>());
	}

	public Riferimento(int[] brano) {
		costruttoreComune();
		brani.add(brano);
		numeroParola.add(new ArrayList<Integer>());
	}

	public Riferimento(boolean brano) {
		costruttoreComune();
		versetti = brano;
	}

	public Riferimento(Riferimento riferimento) {
		costruttoreComune();
		versetti = riferimento.getVersetti();
		daTradurre = riferimento.getDaTradurre();

		if (versetti) {
			for (int i = 0; i < riferimento.getBrani().size(); ++i) {
				brani.add(new int[] { riferimento.getBrani().get(i)[0], riferimento.getBrani().get(i)[1], riferimento.getBrani().get(i)[2], riferimento.getBrani().get(i)[3],
						riferimento.getBrani().get(i)[4], riferimento.getBrani().get(i)[5] });
				numeroParola.add(new ArrayList<Integer>());
				for (int j = 0; j < riferimento.numeroParola.get(i).size(); ++j)
					numeroParola.get(i).add(riferimento.numeroParola.get(i).get(j));
			}
		} else {
			for (int i = 0; i < riferimento.getNote().size(); ++i)
				note.add(riferimento.getNote().get(i));
		}
	}

	private void costruttoreComune() {
		brani = new ArrayList<int[]>();
		note = new ArrayList<String>();
		numeroParola = new ArrayList<List<Integer>>();
	}

	public List<int[]> getBrani() {
		// return new ArrayList<int[]>(getBrani());
		return brani;
	}

	public boolean getVersetti() {
		return versetti;
	}

	public void setVersetti(boolean valore) {
		versetti = valore;
	}

	public boolean getDaTradurre() {
		return daTradurre;
	}

	public void setDaTradurre(boolean valore) {
		daTradurre = valore;
	}

	public List<String> getNote() {
		return note;
		// return new ArrayList<String>(note);
	}

	public List<Integer> getNumeroParola(int i) {
		return numeroParola.get(i);
	}

	public void aggiungiBrano(int[] brano) {
		aggiungiBranoNumeroParola(brano, new ArrayList<Integer>());
	}

	public void aggiungiBrano8Int(int[] brano) {
		List<Integer> listaParole = new ArrayList<Integer>();
		if (brano[3] != 0 || brano[7] != 0) {
			listaParole.add(brano[3]);
			listaParole.add(brano[7]);
		}
		aggiungiBranoNumeroParola(new int[] { brano[0], brano[1], brano[2], brano[4], brano[5], brano[6] }, listaParole);
	}

	public void aggiungiNumeroParola(int i, int j) {
		numeroParola.get(i).add(j);
	}

	public void aggiungiNumeroParola(int i, List<Integer> numeriParole) {
		numeroParola.get(i).addAll(numeriParole);
	}

	public void aggiungiBranoNumeroParola(int[] brano, List<Integer> numeriParole) {
		brani.add(brano);
		numeroParola.add(numeriParole);
	}

	public void aggiungiNotaNumeroParola(String nota, List<Integer> numeriParole) {
		note.add(nota);
		numeroParola.add(numeriParole);
	}

	public void inserisciNumeroParola(int i, int j, int k) {
		numeroParola.get(i).add(j, k);
	}

	public void rimuoviBrano(int i) {
		brani.remove(i);
		numeroParola.remove(i);
	}

	public void rimuoviNota(int i) {
		note.remove(i);
		numeroParola.remove(i);
	}

	public void ordinaParole(int i) {
		Collections.sort(numeroParola.get(i));
	}

	public int count() {
		return versetti ? brani.size() : note.size();
	}

	public int countNumeroParola() {
		return numeroParola.size();
	}

	/*
	 * #region Riferimento
	 * 
	 * public class Riferimento : IComparer { /// <summary> /// Cancella un brano o una nota dal riferimento. Se il paramento è meno di 0 o più del numero dei brani/note, non
	 * succede niente. /// </summary> /// <param name="numero">Il numero del brano o della nota da rimuovere.</param> public void Rimuovi(int numero) { if (numero >= 0 && numero <
	 * Count) { if (versetti) brani.RemoveAt(numero); else note.RemoveAt(numero); } }
	 * 
	 * /// <summary> /// Cancella tutti i dati del riferimento. /// </summary> public void Clear() { brani.Clear(); note.Clear(); numeroParola.Clear(); daTradurre = false; }
	 */

	// Indica se il primo versetto di due brani sono uguali.
	// primoIndice: L'indice del primo brano.
	// secondoIndice: L'indice del secondo brano.
	// Restituisce un boolean che dà se i primi versetti sono uguali.
	public boolean primoVersettoUguale(int primoIndice, int secondoIndice) {
		return (brani.get(primoIndice)[0] == brani.get(secondoIndice)[0] && brani.get(primoIndice)[1] == brani.get(secondoIndice)[1] && brani.get(primoIndice)[2] == brani
				.get(secondoIndice)[2]);
	}

	public void ordinaNote(Collator coll) {
		Collections.sort(note, coll);
	}

	/*
	 * /// <summary> /// Indica se un brano è composto da uno solo versetto. /// </summary> /// <param name="indice">L'indice del brano.</param> /// <returns>Un boolean che dà se
	 * il brano è composto da uno solo versetto.</returns> public bool SoloUnoVersetto(int indice) { return (brani[indice][0] == brani[indice][3] && brani[indice][1] ==
	 * brani[indice][4] && brani[indice][2] == brani[indice][5]); }
	 * 
	 * 
	 * /// <summary> /// Aggiunge tutti i brani di un riferimento al riferimento. /// </summary> /// <param name="riferimento">Il riferimento che contiene i brani da
	 * aggiungere.</param> public void AggiungiBraniDaRiferimento(Riferimento riferimento) { for (int i = 0; i < riferimento.Brani.Count; ++i)
	 * AggiungiBranoEParole(riferimento.Brani[i], new Collection<UInt16>(riferimento.numeroParola[i])); }
	 * 
	 * /// <summary> /// Aggiunge un brano e una collezione di parole selezionate nel primo versetto del brano al riferimento. /// </summary> /// <param name="brano">Il brano da
	 * aggiungere.</param> /// <param name="parole">I numeri delle parole nel versetto.</param> [CLSCompliant(false)] public void AggiungiBranoEParole(byte[] brano,
	 * Collection<UInt16> parole) { brani.Add(brano); numeroParola.Add(new List<UInt16>(parole)); }
	 * 
	 * /// <summary> /// Aggiunge una nota e una collezione di parole selezionate al riferimento. /// </summary> /// <param name="nota">Il titolo della nota da aggiungere.</param>
	 * /// <param name="parole">I numeri delle parole nella nota.</param> [CLSCompliant(false)] public void AggiungiNotaEParole(string nota, Collection<UInt16> parole) {
	 * note.Add(nota); numeroParola.Add(new List<UInt16>(parole)); }
	 */

	// Restituisce una string che rappresenta come tutto il riferimento è mostrato quando è il titolo di una nota.
	public String comeNotaTuttoRiferimento() {
		// vedi ConvertiTitoloNotaARiferimento per l'altra direzione
		if (brani.size() > 0) {
			StringBuilder comeNota = new StringBuilder(26 * brani.size());
			for (int i = 0; i < brani.size(); ++i)
				comeNota.append(comeNotaUnBrano(i));
			return comeNota.toString();
		}
		return "";
	}

	// Restituisce una string che rappresenta come il primo brano nel riferimento è mostrato quando è il titolo di una nota.
	public String comeNotaPrimoRiferimento() {
		// vedi ConvertiTitoloNotaARiferimento per l'altra direzione
		if (brani.size() > 0)
			return comeNotaUnBrano(0);
		return "";
	}

	private String comeNotaUnBrano(int numeroBrano) {
		StringBuilder comeNota = new StringBuilder("#");
		String temp;
		temp = "0" + Integer.toString(brani.get(numeroBrano)[0]);
		temp = temp.substring(temp.length() - 2);
		comeNota.append(temp);
		temp = "00" + Integer.toString(brani.get(numeroBrano)[1]);
		temp = temp.substring(temp.length() - 3);
		comeNota.append(temp);
		temp = "00" + Integer.toString(brani.get(numeroBrano)[2]);
		temp = temp.substring(temp.length() - 3);
		comeNota.append(temp);
		if (numeroParola.get(numeroBrano).size() < 2)
			comeNota.append("0000-");
		else {
			temp = "0000" + Integer.toString(numeroParola.get(numeroBrano).get(0));
			temp = temp.substring(temp.length() - 4);
			comeNota.append(temp).append("-");
		}

		temp = "0" + Integer.toString(brani.get(numeroBrano)[3]);
		temp = temp.substring(temp.length() - 2);
		comeNota.append(temp);
		temp = "00" + Integer.toString(brani.get(numeroBrano)[4]);
		temp = temp.substring(temp.length() - 3);
		comeNota.append(temp);
		temp = "00" + Integer.toString(brani.get(numeroBrano)[5]);
		temp = temp.substring(temp.length() - 3);
		comeNota.append(temp);
		if (numeroParola.get(numeroBrano).size() < 2)
			comeNota.append("0000");
		else {
			temp = "0000" + Integer.toString(numeroParola.get(numeroBrano).get(1));
			temp = temp.substring(temp.length() - 4);
			comeNota.append(temp);
		}

		String notaStringa = comeNota.toString();
		if (notaStringa.endsWith("2552550000")) // un riferimento per tutto il libro
			notaStringa = notaStringa.substring(0, 3) + "000000" + notaStringa.substring(9, 16) + "0000000000";
		if (notaStringa.endsWith("2550000")) // un riferimento per tutto il capitolo
			notaStringa = notaStringa.substring(0, 6) + "000" + notaStringa.substring(9, 19) + "0000000";
		return notaStringa;
	}

	/*
	 * /// <summary> /// Valuta se due riferimenti sono uguali. /// </summary> /// <param name="riferimentoDaConfrontare">Il riferimento con cui confrontare quello attuale.</param>
	 * /// <returns>Vero se i due riferimenti sono identici.</returns> public bool Uguale(Riferimento riferimentoDaConfrontare) { if (brani.Count !=
	 * riferimentoDaConfrontare.brani.Count || note.Count != riferimentoDaConfrontare.note.Count || daTradurre != riferimentoDaConfrontare.DaTradurre || numeroParola.Count !=
	 * riferimentoDaConfrontare.numeroParola.Count || versetti != riferimentoDaConfrontare.Versetti) return false; bool uguale = true; for (int i = 0; i < brani.Count; ++i) { if
	 * (brani[i][0] != riferimentoDaConfrontare.brani[i][0] || brani[i][1] != riferimentoDaConfrontare.brani[i][1] || brani[i][2] != riferimentoDaConfrontare.brani[i][2] ||
	 * brani[i][3] != riferimentoDaConfrontare.brani[i][3] || brani[i][4] != riferimentoDaConfrontare.brani[i][4] || brani[i][5] != riferimentoDaConfrontare.brani[i][5] ) uguale =
	 * false; } for (int i = 0; i < note.Count; ++i) { if (note[i] != riferimentoDaConfrontare.note[i]) uguale = false; } for (int i = 0; i < numeroParola.Count; ++i) { if
	 * (numeroParola[i].Count != riferimentoDaConfrontare.numeroParola[i].Count) uguale = false; else { for (int j = 0; j < numeroParola[i].Count; ++j) if (numeroParola[i][j] !=
	 * riferimentoDaConfrontare.numeroParola[i][j]) uguale = false; } } return uguale; }
	 * 
	 * /// <summary> /// Decide se il riferimento contiene un certo versetto. /// </summary> /// <param name="versettoDaRicercare">Un riferimento che contiene il versetto da
	 * controllare (solo l'inizio del primo brano è controllato).</param> /// <returns>Vero se il riferimento contiene il versetto. Falso se il riferimento da controllare era
	 * vuoto.</returns> public bool ContieneVersetto(Riferimento versettoDaRicercare) { if (versettoDaRicercare.brani.Count == 0) return false; byte[] versettoDaRicercaComeByte =
	 * versettoDaRicercare.brani[0]; foreach (byte[] brano in brani) { if ((brano[0] < versettoDaRicercaComeByte[0] || (brano[0] == versettoDaRicercaComeByte[0] && brano[1] <
	 * versettoDaRicercaComeByte[1]) || (brano[0] == versettoDaRicercaComeByte[0] && brano[1] == versettoDaRicercaComeByte[1] && brano[2] <= versettoDaRicercaComeByte[2])) &&
	 * (brano[3] > versettoDaRicercaComeByte[0] || (brano[3] == versettoDaRicercaComeByte[0] && brano[4] > versettoDaRicercaComeByte[1]) || (brano[3] ==
	 * versettoDaRicercaComeByte[0] && brano[4] == versettoDaRicercaComeByte[1] && brano[5] >= versettoDaRicercaComeByte[2]))) return true; } return false; }
	 * 
	 * /// <summary> /// Decide se il riferimento contiene almeno una parte di un certo brano. /// </summary> /// <param name="branoDaRicercare">Un riferimento che contiene il
	 * brano da controllare.</param> /// <returns>Vero se il riferimento contiene una parte del brano. Falso se il brano da controllare era vuoto.</returns> public bool
	 * ContieneBrano(Riferimento branoDaRicercare) { foreach (byte[] parteBranoDaRicercare in branoDaRicercare.brani) { foreach (byte[] parteRiferimento in brani) { if
	 * (Sovrapposizione(parteRiferimento, parteBranoDaRicercare)) return true; } } return false; }
	 * 
	 * private static bool Sovrapposizione(byte[] b1, byte[] b2) { return ((b1[0] < b2[3] || (b1[0] == b2[3] && b1[1] < b2[4]) || (b1[0] == b2[3] && b1[1] == b2[4] && b1[2] <=
	 * b2[5])) && (b1[3] > b2[0] || (b1[3] == b2[0] && b1[4] > b2[1]) || (b1[3] == b2[0] && b1[4] == b2[1] && b1[5] >= b2[2]))); }
	 * 
	 * #region IComparer Members
	 * 
	 * /// <summary> /// Confonta due riferimenti per determinare quale è primo. /// </summary> /// <param name="x">Il primo riferimento.</param> /// <param name="y">Il secondo
	 * riferimento.</param> /// <returns>-1, 0 o 1 se il primo riferimento è prima, uguale a o dopo il secondo.</returns> public int Compare(object x, object y) { try { Riferimento
	 * riferimento1 = (Riferimento)x; Riferimento riferimento2 = (Riferimento)y; if (riferimento1.Count == 0) if (riferimento2.Count == 0) return 0; else return -1; if
	 * (riferimento2.Count == 0) return 1; if (riferimento1.Versetti) { if (riferimento2.Versetti) { byte[] brano1 = riferimento1.brani[0]; byte[] brano2 = riferimento2.brani[0];
	 * if (brano1[0] < brano2[0]) return -1; if (brano1[0] > brano2[0]) return 1; // libro uguale if (brano1[1] < brano2[1]) return -1; if (brano1[1] > brano2[1]) return 1; //
	 * capitolo uguale if (brano1[2] < brano2[2]) return -1; if (brano1[2] > brano2[2]) return 1; // versetto uguale List<UInt16> parole1 = riferimento1.numeroParola[0];
	 * List<UInt16> parole2 = riferimento2.numeroParola[0]; if (parole1.Count == 0) if (parole2.Count == 0) return 0; else return -1; if (parole2.Count == 0) return 1; if
	 * (parole1[0] < parole2[0]) return -1; if (parole1[0] > parole2[0]) return 1; return 0; } else return -1; // versetti prima di note } else { if (riferimento2.Versetti) {
	 * return 1; // versetti dopo note } else if (riferimento1.note[0] == riferimento2.note[0]) { // nota uguale List<UInt16> parole1 = riferimento1.numeroParola[0]; List<UInt16>
	 * parole2 = riferimento2.numeroParola[0]; if (parole1.Count == 0) if (parole2.Count == 0) return 0; else return -1; if (parole2.Count == 0) return 1; if (parole1[0] <
	 * parole2[0]) return -1; if (parole1[0] > parole2[0]) return 1; return 0; } else return String.Compare(riferimento1.note[0], riferimento2.note[0], StringComparison.Ordinal); }
	 * } catch { return 0; } }
	 * 
	 * #endregion }
	 * 
	 * #endregion
	 */

}