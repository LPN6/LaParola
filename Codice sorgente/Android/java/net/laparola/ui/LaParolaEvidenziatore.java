package net.laparola.ui;

import android.util.Log;

import java.io.BufferedReader;
import java.io.FileNotFoundException;
import java.io.FileWriter;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collections;
import java.util.Comparator;
import java.util.HashMap;
import java.util.HashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;

import net.laparola.core.Riferimento;
import net.laparola.core.Testi.TestoTipi;
import net.laparola.ui.android.LaParolaPreferences;
import net.laparola.ui.utils.Files;

public class LaParolaEvidenziatore {
	private static class Versetto {
		private Map<String, String> mAnchorPerVersioni = new HashMap<String, String>();
		private Map<String, int[]> mBranoPerVersioni = new HashMap<String, int[]>();
		private Riferimento mStandard; 
		private static StringBuilder _tmp = new StringBuilder();
		private String mVersetto0;
		private String mVersione0;
		private String mColore;
		private String mToString = null;
				
		public Versetto (String versetto, String versione, String colore) {
			mVersetto0 = versetto;
			mVersione0 = versione;
			mColore = colore;
			mAnchorPerVersioni.put(versione, versetto);
		}
		
		public Versetto(String line) {
			mToString = line;

			if (line.contains("|")) {
				String[] s = line.split("\\|");
				mColore = s[0];
				line = s[1];
			} else {
				mColore = "yellow";
			}
			for (String versione : line.split("/")) {
				String[] s = versione.split("@");
				if (s.length == 2) {
					mAnchorPerVersioni.put(s[0], s[1]);
					if (mVersetto0 == null) {
						mVersione0 = s[0];
						mVersetto0 = s[1];
					}
				}
			}
		}

		public boolean equals(Object o) {
			if (o == null || !(o instanceof Versetto)) {
				return false;
			}
			return o.toString().equals(this.toString());
		}

		public String getAnchor (String versione) {
			if (!mAnchorPerVersioni.containsKey(versione)) {
				int[] lcv = getBranoPerVersione(versione);
				if (lcv == null)
					return "*ERRORE*";

				synchronized (_tmp) {
					_tmp.setLength(0);
					_tmp.append(LaParolaBrowser.getAbbreviazioneLibro(lcv[0]));
					_tmp.append("_");
					_tmp.append(lcv[1]);
					_tmp.append("_");
					_tmp.append(lcv[2]);
					mAnchorPerVersioni.put(versione, _tmp.toString());
					mToString = null;
				}
			}
			return mAnchorPerVersioni.get(versione);
		}

		public int[] getBranoPerVersione(String versione) {
			if (!mBranoPerVersioni.containsKey(versione)) {
				if (mStandard == null) {
					String srif = mVersetto0.replaceFirst("_", " ").replaceFirst("_", ":");
					Riferimento rif = LaParolaBrowser.creaRiferimento(srif, versione);
					mStandard = LaParolaBrowser.convertiRiferimentoAStandard(rif, mVersione0);
				}
				Riferimento rif = LaParolaBrowser.convertiRiferimentoDaStandard(mStandard, versione);
				List<int[]> brani = rif.getBrani();
				
				if (brani.size() == 0) {
					mBranoPerVersioni.put(versione, null);
				} else {
					mBranoPerVersioni.put(versione, brani.get(0));
				}
			}
			return mBranoPerVersioni.get(versione);
		}
		
		@Override
		public String toString() {
			if (mToString == null) {
				synchronized (_tmp) {
					_tmp.setLength(0);
					_tmp.append(mColore);
					_tmp.append("|");

					String[] keys = new String[mAnchorPerVersioni.size()];
					keys = mAnchorPerVersioni.keySet().toArray(keys);
					Arrays.sort(keys);

					for (String k : keys) {
						_tmp.append(k);
						_tmp.append("@");
						_tmp.append(mAnchorPerVersioni.get(k));
						_tmp.append("/");
					}

					mToString =  _tmp.toString();
				}
			}

			return mToString;
		}

		public boolean isEmpty() {
			return mAnchorPerVersioni.isEmpty();
		}

		public String getColore() {
			return mColore;
		}

		public void setColore(String colore) {
			mColore = colore;
			mToString = null;
		}
	}
	
	private static Set<Versetto> VersettiEvidenziati = new HashSet<Versetto>();
	
	private LaParolaBrowser mBrowser;
	private String mColore;
			
	/* package */ LaParolaEvidenziatore(LaParolaBrowser laParolaBrowser) {
		mBrowser = laParolaBrowser;
		mColore = "yellow";
	}

	public static void caricaVersettiEvidenziatiDaFile(String nomeFile) {
		synchronized (LaParolaBrowser.DataLock) {
			if (Files.fileIsEqualToInternalStorage(nomeFile)) {
				return;
			}

			InputStream inp = null;
			try {
				inp = LaParolaBrowser.apriFile(nomeFile);
			} catch (FileNotFoundException e) {
				//
			}

			HashSet<String> dupLines = new HashSet<String>();
	
			if (inp != null) {
				//VersettiEvidenziati.clear();
		        String line = null;
		        try {
			        BufferedReader br = new BufferedReader(new InputStreamReader(inp));
			        while ((line = br.readLine()) != null) {
			        	if (dupLines.contains(line))
			        		continue;
			        	dupLines.add(line);

		        		Versetto versetto = new Versetto(line);
		        		if (!versetto.isEmpty()) {
		        			VersettiEvidenziati.add(versetto);
		        			//Log.d("laparola", "versetto evidenziato " + versetto.toString());
						}
			        }
		        } catch (IOException e) {
		        	e.printStackTrace();
				}
		    }
		}
	}

	public static void salvaVersettiEvidenziatiSuFile() {
		String filename = LaParolaPreferences.internalStoragePath + "/evidenziati";

		synchronized (LaParolaBrowser.DataLock) {
			FileWriter writer = null;
			try {
				writer = new FileWriter(filename);
				for (Versetto v : VersettiEvidenziati) {
					writer.append(v.toString());
					writer.append('\n');
				}
				writer.flush();
			} catch (Exception e) {
				e.printStackTrace();
			} finally {
				if (writer != null) {
					try {
						writer.close();
					} catch (IOException e) {
						//
					}
				}
			}

			try {
				Files.copyFileIfExists(
						filename,
						LaParolaPreferences.writeStoragePath + "/evidenziati");
			} catch (Exception e) {
				e.printStackTrace();
			}
		}
	}
	
	/*package*/ static Riferimento getRiferimentoVersettiEvidenziati(String versione) {
		ArrayList<int[]> ris = new ArrayList<int[]>();

		synchronized (LaParolaBrowser.DataLock) {
			for (Versetto v : VersettiEvidenziati) {
				int[] branoPerVersione = v.getBranoPerVersione(versione);
				if (branoPerVersione != null)
					ris.add(branoPerVersione);
			}
		}
		
		Collections.sort(ris, new Comparator<int[]>() {
			@Override
			public int compare(int[] arg0, int[] arg1) {
				for (int i = 0; i < 3; i++) {
					if (arg0[i] < arg1[i]) {
						return -1;
					} else if (arg0[i] > arg1[i]) {
						return 1;
					}
				}
				
				return 0;
			}
		});
		
		Riferimento rif = new Riferimento();
		int[] l = null;
		for (int[] b : ris) {
			if (l != null && l[3] == b[0] && l[4] == b[1] && l[5] == b[2] - 1) {
				l[5]++;
			} else {
				l = b.clone();
				rif.aggiungiBrano(l);
			}
		}
		return rif;
	}
	
	public boolean cambiaEvidenziazioneVersetto (String anchor) {
		String versione = mBrowser.getVersione();
		for (Versetto v : VersettiEvidenziati) {
			if (v.getAnchor(versione).equals(anchor)) {
				if (v.getColore().equals(mColore)) {
					VersettiEvidenziati.remove(v);
					return false;
				} else {
					v.setColore(mColore);
					return true;
				}
			}
		}

		VersettiEvidenziati.add(new Versetto(anchor, versione, mColore));
		return true;
	}

	public boolean attivaEvidenziatore(boolean attivo) {
		if (mBrowser == null) {
			return false;
		}
		if (mBrowser.mClient != null) {
			LaParolaUrl url = mBrowser.getUrlCorrente();
			if (url == null) {
				return false;
			}
			if (!url.gestito) {
				return false;
			}
			boolean schemaOk = url.schema.equals("laparola") || url.schema.equals("lpevidenziati");
			boolean testoOk = url.getTipoTesto().contains(TestoTipi.BIBBIA);
			
			if (schemaOk && testoOk) {
				mBrowser.mClient.eseguiFunzioneJavaScriptSeDefinita("attivaEvidenziatore", "attivaEvidenziatore(" + String.valueOf(attivo) + ")");
				return true;
			}
		}
		return false;
	}
	
	public void evidenziaVersetto (String anchor, boolean evidenzia, boolean coloraTesto) {
		if (mBrowser.mClient != null) {
			mBrowser.mClient.eseguiFunzioneJavaScriptSeDefinita("evidenziaVersetto", String.format(
					"evidenziaVersetto('%s', '%s', %s)", 
					anchor,
					evidenzia ? mColore : "",
					coloraTesto ? "true" : "false"));
		}
	}
	
	/* package */ void evidenziaVersetti(boolean coloraTesto) {
		String versione = mBrowser.getVersione();
		if (mBrowser.mClient != null) {
			StringBuilder sb = new StringBuilder();
			int i = 0;
			for (Versetto v : VersettiEvidenziati) {
				sb.append("evidenziaVersetto('");
				sb.append(v.getAnchor(versione));
				sb.append("', '");
				sb.append(v.getColore());
				sb.append("', ");
				sb.append(coloraTesto ? "true" : "false");
				sb.append("); ");
				i++;
				if (i >= 100) {
					mBrowser.mClient.eseguiFunzioneJavaScriptSeDefinita("evidenziaVersetto", sb);
					sb.setLength(0);
					i = 0;
				}
			}
			mBrowser.mClient.eseguiFunzioneJavaScriptSeDefinita("evidenziaVersetto", sb);
		}
	}

	public String getColore() {
		return mColore;
	}

	public void setColore(String colore) {
		this.mColore = colore;
	}
}
