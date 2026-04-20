package net.laparola.core;

import java.util.HashMap;
import java.util.Iterator;
import java.util.Map.Entry;

class LibriAbbreviazioniRiconosciuteHash {

	private final HashMap<String, Integer> libriAbbreviazioniRiconosciute;

	LibriAbbreviazioniRiconosciuteHash() {
		libriAbbreviazioniRiconosciute = new HashMap<String, Integer>(256);
	}

	public int get(String abbreviazione) {
		return libriAbbreviazioniRiconosciute.get(abbreviazione);
	}

	public void put(String abbreviazione, int value) {
		libriAbbreviazioniRiconosciute.put(abbreviazione, value);
	}

	public String Abbreviazione(int libro) {
		Iterator<Entry<String, Integer>> it = libriAbbreviazioniRiconosciute.entrySet().iterator();
		Entry<String, Integer> e;
		while (it.hasNext()) {
			e = it.next();
			if (e.getValue() == libro)
				return e.getKey();
		}
		return "";
	}

	public boolean ContainsKey(String abbreviazione) {
		return libriAbbreviazioniRiconosciute.containsKey(abbreviazione);
	}

	public void Clear() {
		libriAbbreviazioniRiconosciute.clear();
	}

	// / <summary>
	// / Restituisce tutte le abbreviazioni riconosciute, ordinate per libro.
	// / </summary>
	// / <returns>Un array con 73 elementi (da 0 a 72), ogni elemento ha tutte
	// le abbreviazioni separate da una virgola per un libro.</returns>
	public String[] AbbreviazioniPerLibro() {
		String[] abbreviazioniRiconoconosciute = new String[73];
		Iterator<Entry<String, Integer>> it = libriAbbreviazioniRiconosciute.entrySet().iterator();
		Entry<String, Integer> e;
		while (it.hasNext()) {
			e = it.next();
			abbreviazioniRiconoconosciute[e.getValue() - 1] += e.getKey() + ",";
		}
		return abbreviazioniRiconoconosciute;
	}
}
