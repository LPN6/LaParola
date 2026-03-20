package net.laparola.core;

// Per la concordanza, dà il numero del versetto o della nota e il numero della parola nella voce
class OccorrenzaParola implements Comparable<OccorrenzaParola> {

	// Il numero della voce (versetto o nota) nel testo.
	private int voce;

	public int getVoce() {
		return voce;
	}

	public void setVoce(int voce) {
		this.voce = voce;
	}

	// Il numero della parola nel testo.
	private int parola;

	public int getParola() {
		return parola;
	}

	public void setParola(int parola) {
		this.parola = parola;
	}

	// Confronta un altro oggetto di tipo OccorrenzaParola con quello attuale.
	// op: L'altro oggetto di tipo OccorrenzaParola da confrontare.
	// Restituisce -1 se questa parola è prima dell'altro, 0 se è uguale, 1 se è dopo.
	@Override
	public int compareTo(OccorrenzaParola op) {
		// OccorrenzaParola op= (OccorrenzaParola)oggetto;
		if (op == null)
			throw new NullPointerException("OccorrenzaParola è null");
		if (this.voce < op.voce)
			return -1;
		else if (this.voce > op.voce)
			return 1;
		else {
			if (this.parola < op.parola)
				return -1;
			else if (this.parola > op.parola)
				return 1;
			else
				return 0;
		}
	}

	public Boolean Equals(OccorrenzaParola op) {
		return (this.compareTo(op) == 0);
	}

	// public override int GetHashCode()
	// {
	// return (int)(voce / 2) ^ parola;
	// }
}
