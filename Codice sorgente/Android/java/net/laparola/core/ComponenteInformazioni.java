package net.laparola.core;

import java.util.EnumSet;

import net.laparola.core.Testi.*;

public class ComponenteInformazioni {
	private final String componente;

	public String getComponente() {
		return componente;
	}

	private final String descrizione;

	public String getDescrizione() {
		return descrizione;
	}

	private final int versione1;

	public int getVersione1() {
		return versione1;
	}

	private final int versione2;

	public int getVersione2() {
		return versione2;
	}

	private final int versione3;

	public int getVersione3() {
		return versione3;
	}

	private final String motivo;

	public String getMotivo() {
		return motivo;
	}

	private final String url;

	public String getUrl() {
		return url;
	}

	private final long dimensione;

	public long getDimensione() {
		return dimensione;
	}

	private final String url2;
	
	public String getUrl2() {
		return url2;
	}

	private final long dimensione2;

	public long getDimensione2() {
		return dimensione2;
	}

	private final EnumSet<TestoTipi> tipo;

	public EnumSet<TestoTipi> getTipo() {
		return tipo;
	}

	private StatoAggiornamento statoAggiornamento = StatoAggiornamento.NON_INSTALLATO;

	public StatoAggiornamento getStatoAggiornamento() {
		return statoAggiornamento;
	}

	public void setStatoAggiornamento(StatoAggiornamento stato) {
		statoAggiornamento = stato;
	}

	public ComponenteInformazioni(String comp, String desc, String vers, String mot, String u, long dim, String u2, long dim2, EnumSet<TestoTipi> t) {
		componente = comp;
		descrizione = desc;
		motivo = mot;
		url = u;
		dimensione = dim;
		url2 = u2;
		dimensione2 = dim2;
		String[] v3 = vers.split("\\.");
		versione1 = Integer.parseInt(v3[0]);
		versione2 = Integer.parseInt(v3[1]);
		versione3 = Integer.parseInt(v3[2]);
		tipo = t;
	}
}
