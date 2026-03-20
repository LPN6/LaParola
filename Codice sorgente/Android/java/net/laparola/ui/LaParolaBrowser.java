package net.laparola.ui;

import android.util.Log;

import net.laparola.core.ComponenteInformazioni;
import net.laparola.core.FormatoTesto;
import net.laparola.core.RicercaErroreSintassiException;
import net.laparola.core.RicercaEspressioneVuotaException;
import net.laparola.core.RicercaParentesiException;
import net.laparola.core.RicercaParentesiQuadrateException;
import net.laparola.core.Riferimento;
import net.laparola.core.Testi;
import net.laparola.core.Testi.RiferimentoFormato;
import net.laparola.core.Testi.RiferimentoPosto;
import net.laparola.core.Testi.RiferimentoTipo;
import net.laparola.core.Testi.StatoAggiornamento;
import net.laparola.core.Testi.TestoTipi;
import net.laparola.core.Testi.TestoVisualizzato;
import net.laparola.core.TestoNonEsisteException;
import net.laparola.core.VersioneInformazioni;
import net.laparola.ui.LaParolaSegnalibri.GruppoSegnalibri;
import net.laparola.ui.LaParolaSegnalibri.Segnalibro;
import net.laparola.ui.android.LaParolaPreferences;
import net.laparola.ui.utils.Files;

import org.xml.sax.SAXException;

import java.io.BufferedInputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.FileWriter;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.MalformedURLException;
import java.net.URL;
import java.net.URLConnection;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.Locale;
import java.util.Set;
import java.util.UUID;
import java.util.regex.Pattern;

import javax.xml.parsers.ParserConfigurationException;

public class LaParolaBrowser {
    // L'Oggetto LaParolaBrowser dialoga con la finestra di visualizzazione attraverso i metodi
	// dell'interfaccia LaParolaBrowserClient
	public interface LaParolaBrowserClient {
		// Apre un link
        void apriLink(String string);

		// Ottiene un header da aggiungere alla sezione head delle pagine generate,
		// deve gestire anche url == null
        CharSequence getAggiuntaHeader(LaParolaUrl url);

		// Segnala l'inizio di una richiesta, per visualizzare la clessidra.
        void onRichiestaIniziata(LaParolaUrl url);

		// La versione è stata cambiata, il client deve reagire di conseguenza.
		// Serve per evitare di dimenticare di richiamare un metodo in ogni parte del codice che cambia la versione.
        void onVersioneCambiata();

		// Il client salta al segnalibro.
        void vaiAdAncoraggio(String ancoraggio);

		// Visualizza il sito internet
        void visualizzaSito();

		// Il client visualizza il testo dell'url nella sua finesta e salta ad url.segnalibro.
        void visualizzaTesto(CharSequence risultato, LaParolaUrl url);
		
		// Esegue del codice JavaScript. 
        void eseguiJavaScript(CharSequence codice);
		void eseguiFunzioneJavaScriptSeDefinita(String funzione, CharSequence argomenti);

        void mostraRicerca();
    }

	// L'Oggetto LaParolaBrowser dialoga con l'interfaccia utente attraverso i metodi
	// dell'interfaccia LaParolaBrowserStaticClient
	public interface LaParolaBrowserStaticClient {
		// Il client apre un file in lettura e restituisce l'InputStream corrispondente.
		// Se restituisce null, viene usata una funzione predefinita per l'apertura dei file.
		// Serve per poter usare funzioni specifiche della piattaforma, come gli asset di Android.
        InputStream apriFile(String filename);

		// Mostra l'help della ricerca
        void mostraAiutoRicerca();

		// Segnala che i testi sono cambiati
        void onTestiCambiati();

		// Mostra una finestra che chiede se pulire la cronologia, ed eventualmente la
		// pulisce chiamando LaParolaBrowser.pulisciCronologia()
        void mostraPulisciCronologia();

		// Memorizzano l'ultima versione usata
        String getUltimaBibbiaSalvata();
		void setUltimaBibbiaSalvata(String versione);

		// Mostra la conferma per eliminare i preferiti 
        void mostraEliminaPreferito(LaParolaUrl nuovoUrl);

		void installaCarattereGreco();


        // Apre il gestore delle versioni (in risposta ad un link lpcomando:versioni)
        void apriGestioneVersioni();

        // Restituisce il percorso in cui si trovano laparola.css e utils.js
        String getPercorsoAsset();

        void mostraOpzioni();

        void mostraGestorePannelli();
	}

	private static final long AGGIONA_URL_DURATA_CACHE = 1 * 1000 * 60 * 60; // 1 ora
	private static final long AGGIONA_URL_DURATA_CACHE_PER_AGGIORNAMENTI = 7 * 24 * 1000 * 60 * 60; // 1 settimana
	private static final Pattern riferimento_segnalibro_regex = Pattern.compile("^[0-9 ;]+$");

	/* package */static Testi mTesti;
	/* package */static LaParolaSegnalibri mSegnalibri;
	/* package */static LaParolaSegnalibri mPreferiti;
	/* package */static LaParolaCronologia mCronologia;

	static LaParolaBrowserStaticClient mStaticClient;
	private static int mVersioneProgramma1;
	private static int mVersioneProgramma2;
	private static String mUltimoFilePreferiti;
	private static String mUltimoFileCronologia;
	private static RiferimentoFormato mRiferimentoFormato;

	private static final Object cacheLock = new Object();
	public static final Object DataLock = new Object();

	public static void inizializza(UUID uuid, int versioneProgramma1, int versioneProgramma2) {
		mVersioneProgramma1 = versioneProgramma1;
		mVersioneProgramma2 = versioneProgramma2;

		if (mTesti == null) {
			mTesti = new Testi(uuid, mVersioneProgramma1, mVersioneProgramma2);
		}
		
		resetDatiSalvati ();
	}
	
	public static void resetDatiSalvati () {
		mSegnalibri = new LaParolaSegnalibri();
		mPreferiti = new LaParolaSegnalibri();
		mCronologia = new LaParolaCronologia();
	}
	
	public static void aggiungiPreferitiDaFile(String nomeFile) {
		synchronized (LaParolaBrowser.DataLock) {
			if (Files.fileIsEqualToInternalStorage(nomeFile)) {
				return;
			}

			mUltimoFilePreferiti = nomeFile;

			InputStream inp = null;
			try {
				inp = apriFile(nomeFile);
			} catch (FileNotFoundException e) {
				//
			}
	
			if (inp != null) {
				boolean ok = mPreferiti.aggiungiDaXml(inp);

                if (!ok) {
                    /*
                    mPreferiti = new LaParolaSegnalibri();
                    tryFix(nomeFile);
                    try {
                        inp = apriFile(nomeFile);
                        mPreferiti.aggiungiDaXml(inp);
                    } catch (FileNotFoundException e) {
                        //
                    }
                    */
                }
			}
		}
    }

    /*
    private static void tryFix(String fileName) {
        String line;
        StringBuilder n = new StringBuilder();
        try {
            FileReader fileReader = new FileReader(fileName);
            BufferedReader bufferedReader = new BufferedReader(fileReader);
            while((line = bufferedReader.readLine()) != null) {
                n.append(line.replace("&", "&amp;"));
                n.append('\n');
            }
            bufferedReader.close();
        } catch(FileNotFoundException ex) {
        } catch(IOException ex) {
        }

        try {
            FileWriter fileWriter = new FileWriter(fileName);
            BufferedWriter bufferedWriter = new BufferedWriter(fileWriter);
            bufferedWriter.write(n.toString());
            bufferedWriter.close();
        } catch(IOException ex) {
        }
    }
    */

	public static void aggiungiPreferito(String gruppo, String nome, LaParolaUrl url) {
		mPreferiti.aggiungiSegnalibro(gruppo, nome, url.getUrl());
	}

	public static void aggiungiSegnalibriDaFile(String filename) {
		InputStream inp = null;
		try {
			inp = apriFile(filename);
		} catch (FileNotFoundException e) {
			//
		}

		if (inp != null) {
			mSegnalibri.aggiungiDaXml(inp);
		}
	}
	
	public static List<String> aggiungiTestiDaDirectory(String path) {
		synchronized (mTesti) {
			List<String> res = mTesti.aggiungiTestiDaDirectory(path);
			if (mStaticClient != null) {
				mStaticClient.onTestiCambiati();
			}
			return res;
		}
	}

	public static void aggiungiTesto(String fileName) {
		synchronized (mTesti) {
			mTesti.aggiungiTesto(fileName);
		}
		if (mStaticClient != null) {
			mStaticClient.onTestiCambiati();
		}
	}

	/* package */static InputStream apriFile(String filename) throws FileNotFoundException {
		InputStream inp = null;
		
		if (mStaticClient != null) {
			inp = mStaticClient.apriFile(filename);
		}

		if (inp == null) {
			inp = new FileInputStream(filename);
		}

		return inp;
	}
	
	/* package */ static Riferimento convertiRiferimentoAStandard (Riferimento riferimento, String versione) {
		synchronized (mTesti) {
			Riferimento ret = mTesti.convertiAStandard(riferimento, versione);
			return ret;
		}
	}

	/* package */ static Riferimento convertiRiferimentoDaStandard (Riferimento riferimento, String versione) {
		synchronized (mTesti) {
			return mTesti.convertiDaStandard(riferimento, versione);
		}
	}
	
	public static String cambiaVersioneRiferimento(Riferimento riferimento, String vecchiaVersione, String nuovaVersione) {
		synchronized (mTesti) {
			Riferimento standard = mTesti.convertiAStandard(riferimento, vecchiaVersione);
			Riferimento nuovo = mTesti.convertiDaStandard(standard, nuovaVersione);
			return mTesti.normalizzaRiferimento(nuovo);
		}
	}

	public static void cancellaTesto(String nomeVersione, String filename) {
		synchronized (mTesti) {
			boolean ok = false;
			try {
				mTesti.cancellaTesto(nomeVersione);
				ok = true;
			} catch (TestoNonEsisteException e) {
				//
			} catch (IOException e) {
				//
			}

			if (!ok) {
				ok = (new File(filename)).delete();
				if (ok) {
					mTesti.getFileIllegibili().remove(filename);
				}
			}

			if (mStaticClient != null) {
				mStaticClient.onTestiCambiati();
			}
		}
	}

	public static void chiudi() {
		if (mTesti == null) {
			return;
		}
		synchronized (mTesti) {
			mTesti.close();
		}
	}

	public static String convertiRiferimentoAStandardVirgola (String riferimento, String versione) {
		synchronized (mTesti) {
			FormatoTesto f = mTesti.getFormato();

			RiferimentoTipo oldTipo = f.getRiferimentoTipo();
			f.setRiferimentoTipo(RiferimentoTipo.VIRGOLA);
			String ret = convertiRiferimentoAStandard(riferimento, versione);
			f.setRiferimentoTipo(oldTipo);

			return ret;
		}
	}

	public static String convertiRiferimentoAStandard(String riferimento, String versione) {
		synchronized (mTesti) {
			Riferimento r = mTesti.convertiAStandard(mTesti.convertiRiferimento(riferimento), versione);
			StringBuilder res = new StringBuilder();
			for (int[] b : r.getBrani()) {
				for (int i : b) {
					res.append(i);
					res.append(" ");
				}
				res.setLength(res.length() - 1);
				res.append(";");
			}
			res.setLength(res.length() - 1);
			return res.toString();
		}
	}

	public static Riferimento creaRiferimento(String riferimento, String versione) {
		synchronized (mTesti) {
			Riferimento rif;

			if (riferimento_segnalibro_regex.matcher(riferimento).matches()) {
				// è un riferimento da segnalibro, del tipo "1 1 1 1 2 200; 1 3 1 1 3 10; 2 1 1"
				rif = new Riferimento();

				for (String l : riferimento.split(";")) {
					String[] n = l.trim().split(" ");
					if (n.length == 6) {
						int a = Integer.parseInt(n[0]);
						int b = Integer.parseInt(n[1]);
						int c = Integer.parseInt(n[2]);
						int d = Integer.parseInt(n[3]);
						int e = Integer.parseInt(n[4]);
						int f = Integer.parseInt(n[5]);
						rif.aggiungiBrano(new int[] { a, b, c, d, e, f });
					} else if (n.length == 3) {
						int a = Integer.parseInt(n[0]);
						int b = Integer.parseInt(n[1]);
						int c = Integer.parseInt(n[2]);
						rif.aggiungiBrano(new int[] { a, b, c, a, b, c });
					}
				}
				rif = mTesti.convertiDaStandard(rif, versione);
			} else {
				rif = mTesti.convertiRiferimento(riferimento);
			}

			return rif;
		}
	}

	public static boolean getAggionamentiDisponibiliDebole(final String cacheFileName) {
		// è consigliabile usare una cache diversa per evitare di cancellarla se nel file
		// cache è false.

		String url;
		try {
			url = getFileInCacheUrl(
					cacheFileName, 
					AGGIONA_URL_DURATA_CACHE_PER_AGGIORNAMENTI, 
					Testi.URL_FILE_AGGIORNAMENTI, 
					true);
		} catch (Exception e) {
			return false;
		}

		if (url == null) {
			// il download è stato avviato, la prossima volta funzionerà
			return false;
		}

		List<ComponenteInformazioni> componenti = null;

		try {
			componenti = getTestiDisponibili(cacheFileName, -1, true);
		} catch (Exception e) {
			return false;
		}

		if (componenti != null) {
			for (ComponenteInformazioni c : componenti) {
				if (c.getStatoAggiornamento() == StatoAggiornamento.DA_AGGIORNARE) {
					return true;
				}
			}
		}

		return false;
	}

	public static Set<String> getFileIllegibili() {
		synchronized (mTesti) {
			return mTesti.getFileIllegibili();
		}
	}

	private static String getFileInCacheUrl(final String cacheFileName, long durataCache, final String urlFile, boolean downloadAsync) {
		if (cacheFileName != null) {
			synchronized (cacheLock) {
				File cacheFile = new File(cacheFileName);
				boolean exists = cacheFile.exists();
				long lastModified = cacheFile.lastModified();
				long now = System.currentTimeMillis();

				if (exists && (durataCache <= 0 || now <= lastModified + durataCache)) {
					return "file://" + cacheFileName;
				}

				// non c'è cache, lo scarico
				Runnable r = new Runnable() {
					@Override
					public void run() {
						try {
							// se eseguito in un altro thread è necessario
							// il secondo lock
							synchronized (cacheLock) {
								scaricaUrl(urlFile, cacheFileName);
							}
						} catch (MalformedURLException e) {
							//
						} catch (IOException e) {
							//
						}
					}
				};

				if (downloadAsync) {
					// ritorno subito, ma scarico il file per la prossima volta
					// la prossima volta sarà ok
					new Thread(r).start();
					return null;
				}
				// else
				r.run();
				return "file://" + cacheFileName;
			}
		}

		return urlFile;
	}

	public static List<GruppoSegnalibri> getGruppiSegnalibri() {
		return mSegnalibri.gruppi;
	}

	public static VersioneInformazioni getInformazioniTesto(String nomeVersione) {
		synchronized (mTesti) {
			try {
				return mTesti.getInfo(nomeVersione);
			} catch (TestoNonEsisteException e) {
				return null;
			}
		}
	}

	public static String[] getNomiVersioni() {
		synchronized (mTesti) {
			return mTesti.nomiVersioni();
		}
	}

    public static boolean esisteTesto(String versione) {
        for (String v : getNomiVersioni())
            if (v.equals(versione))
                return true;
        return false;
    }

    public static String getPercorsoAsset() {
        if (mStaticClient == null)
            return null;
        return mStaticClient.getPercorsoAsset();
    }

    public static List<ComponenteInformazioni> getTestiDisponibili(String cacheFileName) throws ParserConfigurationException, IOException, SAXException {
		return getTestiDisponibili(cacheFileName, AGGIONA_URL_DURATA_CACHE, false);
	}

	private static List<ComponenteInformazioni> getTestiDisponibili(String cacheFileName, long durataCache, boolean overrideCancellaCache) throws
            ParserConfigurationException, IOException, SAXException {
		// per test :
		// final String urlFileAggiornamenti = Testi.URL_FILE_AGGIORNAMENTI.replace("http://", "file:///sdcard/laparola/");
		final String urlFileAggiornamenti = Testi.URL_FILE_AGGIORNAMENTI;

		if (mTesti == null)
			return null;
		
		String url = getFileInCacheUrl(cacheFileName, durataCache, urlFileAggiornamenti, false);
		
		List<ComponenteInformazioni> risultato = null;
		boolean attivaCacheAggiornamenti = false;
		try {
			synchronized (mTesti) {
				risultato = mTesti.getTestiDisponibili(url);
				attivaCacheAggiornamenti = mTesti.cacheUltimoFileAggiornamenti;
			}
		} catch (IOException e) {
			e.printStackTrace();
			throw e;
		} catch (SAXException e) {
			e.printStackTrace();
			throw e;
		} finally {
			if (!overrideCancellaCache && cacheFileName != null && !attivaCacheAggiornamenti) {
				new File(cacheFileName).delete();
			}
		}

		return risultato;
	}

	public static List<ComponenteInformazioni> getTestiInstallati() {
	    if (mTesti == null) {
	        return null;
	    }
		synchronized (mTesti) {
			try {
				return mTesti.getTestiInstallati();
			} catch (Exception e) {
				e.printStackTrace();
				return null;
			}
		}
	}

	public static String normalizzaRiferimento(Riferimento rif, String versione) {
		synchronized (mTesti) {
			// workaround per versetti oltre la fine capitolo
			List<int[]> brani = rif.getBrani();
			for (int b = 0; b < brani.size(); b++) {
				int[] brano = brani.get(b);

				// int l1 = brano[0];
				// int c1 = brano[1];
				int v1 = brano[2];
				int l2 = brano[3];
				int c2 = brano[4];
				int v2 = brano[5];

				int versettiInCapitolo = mTesti.versettiInCapitolo(l2, c2, versione);
				if (v1 == 1 && v2 >= versettiInCapitolo) {
					brano[5] = 255; // versettiInCapitolo;
				}
			}
			return mTesti.normalizzaRiferimento(rif);
		}
	}

	public static String normalizzaRiferimento(String riferimento, String versione) {
		synchronized (mTesti) {
			Riferimento rif = LaParolaBrowser.creaRiferimento(riferimento, versione);
			return normalizzaRiferimento(rif, versione);
		}
	}

	public static void pulisciCronologia() {
		mCronologia.pulisci();
		mCronologia.salvaSuFile(mUltimoFileCronologia);
	}

	public static void pulisciTesti() {
		synchronized (mTesti) {
			// TODO : modo migliore?
			mTesti.close();
            UUID uuid = mTesti.deviceUuid;
			mTesti = new Testi(uuid, mVersioneProgramma1, mVersioneProgramma2);
			if (mStaticClient != null) {
				mStaticClient.onTestiCambiati();
			}
		}
	}

    public static Riferimento ricerca(String ricerca, String riferimento, String versione) 
		throws RicercaEspressioneVuotaException, RicercaErroreSintassiException, 
			RicercaParentesiException, RicercaParentesiQuadrateException {

		synchronized (mTesti) {
			return mTesti.ricerca(ricerca, riferimento, versione);
		}
	}

	public static void salvaPreferitiSuFile() {
        String filename = LaParolaPreferences.internalStoragePath + "/preferiti.xml";

        synchronized (LaParolaBrowser.DataLock) {
			mUltimoFilePreferiti = filename;
	
			FileWriter writer = null;
			try {
				writer = new FileWriter(filename);
				writer.append(mPreferiti.salvaInXml());
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
                        LaParolaPreferences.writeStoragePath + "/preferiti.xml");
            } catch (Exception e) {
                e.printStackTrace();
            }
		}
	}

	public static void scaricaUrl(String urlString, String filename) throws IOException {
		File tempFile = File.createTempFile("laparoladownload", null);
		byte data[] = new byte[1024];

		InputStream in = null;
		OutputStream fout = null;
		try {
			URL u = new URL(urlString);
			URLConnection con = u.openConnection();
			con.setConnectTimeout(5000);
			con.setReadTimeout(5000);
			in = new BufferedInputStream(con.getInputStream());
			fout = new FileOutputStream(tempFile);

			int count = 0;
			while ((count = in.read(data, 0, data.length)) != -1) {
				fout.write(data, 0, count);
			}

			in.close();
			fout.close();

			in = new FileInputStream(tempFile);
			fout = new FileOutputStream(filename);

			count = 0;
			while ((count = in.read(data, 0, data.length)) != -1) {
				fout.write(data, 0, count);
			}
		} catch (Exception e) {
			Log.d("LaParola", e.toString());
		} finally {
			if (in != null)
				in.close();
			if (fout != null)
				fout.close();
			tempFile.delete();
		}
	}

	public static void setLaParolaBrowserStaticClient(LaParolaBrowserStaticClient client) {
		mStaticClient = client;
	}
	
	public static Segnalibro cercaUrlTraPreferiti(LaParolaUrl url) {
		return mPreferiti.cercaPerUrl(url);
	}

	public static void rimuoviPreferito(LaParolaUrl url) {
		Segnalibro s = cercaUrlTraPreferiti(url);
		if (s != null) {
			mPreferiti.rimuoviSegnalibro(s);
		}
	}

	public static void setMostraParagrafi(boolean valore) {
		synchronized (mTesti) {
			FormatoTesto f = mTesti.getFormato();
			if (valore) {
				f.setTestoVisualizzato(TestoVisualizzato.PARAGRAFI);
			} else {
				f.setTestoVisualizzato(TestoVisualizzato.VERSETTI);
			}
			LaParolaTesto.pulisciCache();
		}
	}

	public static void setMostraTitoli(boolean valore) {
		synchronized (mTesti) {
			FormatoTesto f = mTesti.getFormato();
			f.setTitoliVisualizzati(valore);
			LaParolaTesto.pulisciCache();
		}
	}

	public static void setPosizioneRiferimento(int valore) {
		synchronized (mTesti) {
			FormatoTesto f = mTesti.getFormato();
			switch (valore) {
			case 0:
				RiferimentoFormato riferimentoFormato = f.getRiferimentoFormato();
				if (riferimentoFormato != RiferimentoFormato.NESSUNO) {
					mRiferimentoFormato = riferimentoFormato;
					f.setRiferimentoFormato(RiferimentoFormato.NESSUNO);
				}
				break;
			case 1:
			default:
				f.setRiferimentoFormato(mRiferimentoFormato);
				f.setRiferimentoPosto(RiferimentoPosto.PRIMA_STESSA_RIGA);
				break;
			case 2:
				f.setRiferimentoFormato(mRiferimentoFormato);
				f.setRiferimentoPosto(RiferimentoPosto.PRIMA_RIGA_DIVERSA);
				break;
			case 3:
				f.setRiferimentoFormato(mRiferimentoFormato);
				f.setRiferimentoPosto(RiferimentoPosto.DOPO);
				break;
			}
			LaParolaTesto.pulisciCache();
		}
	}

	public static void setRiferimentoInApice(boolean valore) {
		synchronized (mTesti) {
			FormatoTesto f = mTesti.getFormato();
			f.setRiferimentoApice(valore);
			LaParolaTesto.pulisciCache();
		}
	}

	public static void setTipoRiferimento(int valore) {
		synchronized (mTesti) {
			FormatoTesto f = mTesti.getFormato();
			switch (valore) {
			case 0:
			default:
				f.setRiferimentoTipo(RiferimentoTipo.DUE_PUNTI);
				mRiferimentoFormato = RiferimentoFormato.ABBREVIAZIONE;
				break;
			case 1:
				f.setRiferimentoTipo(RiferimentoTipo.VIRGOLA);
				mRiferimentoFormato = RiferimentoFormato.ABBREVIAZIONE;
				break;
			case 2:
				f.setRiferimentoTipo(RiferimentoTipo.CITAZIONE);
				mRiferimentoFormato = RiferimentoFormato.ABBREVIAZIONE;
				break;
			case 3:
				f.setRiferimentoTipo(RiferimentoTipo.DUE_PUNTI);
				mRiferimentoFormato = RiferimentoFormato.INTERO;
				break;
			case 4:
				f.setRiferimentoTipo(RiferimentoTipo.VIRGOLA);
				mRiferimentoFormato = RiferimentoFormato.INTERO;
				break;
			}

			if (f.getRiferimentoFormato() != RiferimentoFormato.NESSUNO) {
				f.setRiferimentoFormato(mRiferimentoFormato);
			}

			LaParolaTesto.pulisciCache();
		}
	}

	/**********************************************************************/
	
	public LaParolaEvidenziatore Evidenziatore = new LaParolaEvidenziatore(this);
	
	private LaParolaTesto mLaParolaTesti;
	private ArrayList<LaParolaUrl> mAvantiIndietroUrl;
	private int mAvantiIndietroIndice;
	private String mVersione;
	private String mVersioneCommentario;
	
	/* package */LaParolaUrl mUrlCorrente;
	/* package */LaParolaBrowserClient mClient;
	/* package */CharSequence mTestoCorrente;

	public LaParolaBrowser() {
		mAvantiIndietroIndice = -1;
		mAvantiIndietroUrl = new ArrayList<LaParolaUrl>();
		mVersione = getUltimaBibbia();
		mVersioneCommentario = ""; //"Note della Nuova Riveduta";   // TODO : correggere
		
		mLaParolaTesti = new LaParolaTesto(this);
	}
	
	public LaParolaBrowser(UUID uuid, int versioneProgramma1, int versioneProgramma2) {
		this();

		if (mTesti == null) {
			inizializza(uuid, versioneProgramma1, versioneProgramma2);
			if (mVersione.equals(""))
				mVersione = getUltimaBibbia();
		}		
	}

	public void aggiornaPagina() {
		LaParolaUrl urlCorrente = mUrlCorrente;
		mUrlCorrente = null;
		if (urlCorrente != null)
			vaiAdUrl(urlCorrente, false);
	}

	/* package */void aggiungiUrlACronologia(LaParolaUrl url) {
		if (url == null || !url.gestito || url.schema.equals("null") || url.schema.equals("lpcommand")) {
			return;
		}

		if (!inHome() && mUltimoFileCronologia != null && url.getDescrizione().length() > 0) {
			mCronologia.aggiungi(url, new Date());
			mCronologia.salvaSuFile(mUltimoFileCronologia);
		}

		if (precedenteEsiste()) {
			if (url.equals(mAvantiIndietroUrl.get(mAvantiIndietroIndice - 1))) {
				return;
			}
		}

		mAvantiIndietroIndice++;
		final int size = mAvantiIndietroUrl.size();
		final int newPos = mAvantiIndietroIndice;
		if (newPos != size) {
			for (int i = size - 1; i >= newPos; i--) {
				mAvantiIndietroUrl.remove(i);
			}
		}
		mAvantiIndietroUrl.add(url);
	}

	public static void caricaCronologia(String nomeFile) {
		if (Files.fileIsEqualToInternalStorage(nomeFile)) {
			return;
		}

		mUltimoFileCronologia = nomeFile;
		mCronologia.caricaDaFile(nomeFile);
		mCronologia.ordina();
	}

	public static void salvaCronologia() {
		mUltimoFileCronologia = LaParolaPreferences.internalStoragePath + "/cronologia";
		mCronologia.salvaSuFile(mUltimoFileCronologia);

        try {
            Files.copyFileIfExists(
                mUltimoFileCronologia,
                LaParolaPreferences.writeStoragePath + "/cronologia");
        } catch (Exception e) {
            e.printStackTrace();
        }
	}

	public static String getAbbreviazioneLibro(int libro) {
		synchronized (mTesti) {
			return mTesti.getLibroAbbreviazioneUsata(libro);
		}
	}

	public LaParolaUrl getVersettoCasuale (int minlibro, int maxlibro) {
		int numVersetti = 0;
		
		for (int libro = minlibro; libro <= maxlibro; libro++) {
			for (int capitolo = 1; capitolo < getCapitoliInLibro(libro); capitolo++) {
				numVersetti += getVersettiInCapitolo(libro, capitolo);
			}
		}
		
		if (numVersetti == 0)
			return null;
		
		while (true) {
			int v = (int)Math.floor(Math.random() * numVersetti) + 1;
			for (int libro = minlibro; libro <= maxlibro; libro++) {
				for (int capitolo = 1; capitolo < getCapitoliInLibro(libro); capitolo++) {
					int n = getVersettiInCapitolo(libro, capitolo);
					if (v <= n) {
						return nuovoUrl(libro, capitolo, v);
					}
					v -= n;
				}
			}
		}
	}

    public static int getCapitoliInLibro(int libro, String versione) {
        synchronized (mTesti) {
            try {
                if (!mTesti.getInfo(versione).getTipo().contains(TestoTipi.BIBBIA))
                    versione = getUltimaBibbia();
            } catch (TestoNonEsisteException e) {
                versione = getUltimaBibbia();
            }
            if (versione.equals(""))
                return 0;
            return mTesti.capitoliInLibro(libro, versione);
        }
    }


    public int getCapitoliInLibro(int libro) {
        return getCapitoliInLibro(libro, getVersione());
    }

    public static String getNomeLibro(int libro) {
		synchronized (mTesti) {
			return mTesti.getLibroNome(libro);
		}
	}

	public LaParolaUrl getUrlCorrente() {
		return mUrlCorrente;
	}

	public int getVersettiInCapitolo(int libro, int capitolo) {
		synchronized (mTesti) {
			String versione = getVersione();
			try {
				if (!mTesti.getInfo(versione).getTipo().contains(TestoTipi.BIBBIA))
					versione = getUltimaBibbia();
			} catch (TestoNonEsisteException e) {
				return 0;
			}
			if (versione.length() == 0)
				return 0;
			return mTesti.versettiInCapitolo(libro, capitolo, versione);
		}
	}

	public String getVersione() {
		return mVersione;
	}

	public boolean inHome() {
		LaParolaUrl url = getUrlCorrente();
		return (url != null && url.gestito && url.schema.equals("lpfile") && url.brani.equals("Home.html"));
	}

	public void mostraHtml(String string) {
		String html = new StringBuilder("<html><body><p>").append(string).append("</p></body></html>").toString();
		vaiAdUrl(new LaParolaUrl("html", html, null, null, getVersione(), null, null, this));
	}

	public static LaParolaUrl nuovoUrl(String url, String versione, String versioneCommentario) {
        return new LaParolaUrl(url, versione, versioneCommentario);
	}

	public LaParolaUrl nuovoUrl(String url) {
        return new LaParolaUrl(url, getVersione(), getVersioneCommentario());
	}

	public boolean precedenteEsiste() {
		return mAvantiIndietroIndice > 0;
	}

	public void setLaParolaBrowserClient(LaParolaBrowserClient client) {
		mClient = client;
	}

	public boolean setVersione(String nomeVersione) {
		if (nomeVersione == null || nomeVersione.length() == 0)
			return false;

		if (nomeVersione.equals(getVersione()))
			return true;

		String[] versioni;
		synchronized (mTesti) {
			versioni = mTesti.nomiVersioni();

			for (int i = 0; i < versioni.length; i++) {
				if (versioni[i].equals(nomeVersione)) {
					// mVersione sarà impostata da vaiAdUrl
	
					if (mUrlCorrente != null) {
						String urlConAltraVersione = mUrlCorrente.getUrlConAltraVersione(nomeVersione);
						vaiAdUrl(urlConAltraVersione);
					} else {
						vaiAdUrl(new LaParolaUrl("null", null, null, null, nomeVersione, getVersioneCommentario(), null, this));
					}
	
					return true;
				}
			}
		}

		return false;
	}

	public boolean successivoEsiste() {
		return mAvantiIndietroIndice + 1 < mAvantiIndietroUrl.size();
	}

	public void vaiAdUrl(LaParolaUrl url) {
		vaiAdUrl(url, true);
	}

	public void vaiAdUrl(final LaParolaUrl url, final boolean inserisciInCronologia) {
		if (mClient == null || url == null || url.gestito == false)
			return;
		
		if (!getVersione().equals(url.versione) || !getVersioneCommentario().equals(url.versioneCommentario)) {
			mVersione = url.versione;
			mVersioneCommentario = url.versioneCommentario;
			mClient.onVersioneCambiata();

			synchronized (mTesti) {
				if (mStaticClient != null && mTesti != null) {
					VersioneInformazioni v;
					try {
						v = mTesti.getInfo(mVersione);
						if (v.getTipo().contains(TestoTipi.BIBBIA)) {
							mStaticClient.setUltimaBibbiaSalvata(mVersione);
						}
					} catch (TestoNonEsisteException e) {}
				}
			}
		}

		if (url.schema.equals("lpcomando")) {
			if (url.contenuto.equals("versioni")) {
				mStaticClient.apriGestioneVersioni();
			} else if (url.contenuto.equals("sito")) {
				mClient.visualizzaSito();
			} else if (url.contenuto.startsWith("link:")) {
				mClient.apriLink(url.contenuto.substring(5));
			} else if (url.contenuto.startsWith("aiuto_ricerca")) {
				mStaticClient.mostraAiutoRicerca();
			} else if (url.contenuto.startsWith("installa_font_greco")) {
				mStaticClient.installaCarattereGreco();
			} else if (url.contenuto.startsWith("pulisci_cronologia")) {
				mStaticClient.mostraPulisciCronologia();
			} else if (url.contenuto.startsWith("cancellapreferito:")) {
				mStaticClient.mostraEliminaPreferito(nuovoUrl(url.contenuto.substring(18), null, null));
            } else if (url.contenuto.equals("casuale")) {
                vaiAdUrl(getVersettoCasuale(1, 73));
            } else if (url.contenuto.equals("casualeat")) {
                vaiAdUrl(getVersettoCasuale(1, 46));
            } else if (url.contenuto.equals("casualent")) {
                vaiAdUrl(getVersettoCasuale(47, 73));
            } else if (url.contenuto.equals("ricerca")) {
                mClient.mostraRicerca();
            } else if (url.contenuto.equals("pannelli")) {
                mStaticClient.mostraGestorePannelli();
            } else if (url.contenuto.equals("impostazioni")) {
                mStaticClient.mostraOpzioni();
            }
		} else {
			mClient.onRichiestaIniziata(url);

			// uso un thread, altrimenti non aggiorna subito l'interfaccia
			new Thread(new Runnable() {
				@Override
				public void run() {
					synchronized (this) {
						if (mUrlCorrente != null && mUrlCorrente.stessoTestoDi(url)) {
							vaiASegnalibro(url.ancoraggio);
							mUrlCorrente = url;
							if (inserisciInCronologia) {
								aggiungiUrlACronologia(url);
							}
						} else {
							mLaParolaTesti.mostraBranoInBrowser(url, inserisciInCronologia);
						}
					}
				}
			}).start();
		}
	}

    public void vaiAdUrl(String url) {
		vaiAdUrl(url, true);
	}

	public void vaiAdUrl(String url, boolean inserisciInCronologia) {
		final LaParolaUrl pUrl = nuovoUrl(url, getVersione(), getVersioneCommentario());
		vaiAdUrl(pUrl, inserisciInCronologia);
	}

	public void vaiAHome() {
		vaiAdUrl("lpfile:Home.html");
	}

	public void vaiALibroCapitoloVersetto(int l, int c, int v) {
		LaParolaUrl url = nuovoUrl(l, c, v);
		vaiAdUrl(url);
	}

	public LaParolaUrl nuovoUrl(int l, int c, int v) {
		String abbreviazioneLibro = getAbbreviazioneLibro(l);
		String segnalibro = String.format(Locale.ENGLISH, "%s_%d_%d", abbreviazioneLibro, c, v);
		String riferimento;

		synchronized (mTesti) {
			if (getCapitoliInLibro(l) > 1) {
				// if (mTesti.capitoliInLibro(l, versione) > 1) { modificato da RMW
				riferimento = String.format(Locale.ENGLISH, "%s %d", abbreviazioneLibro, c);
			} else {
				riferimento = abbreviazioneLibro;
			}
		}

		LaParolaUrl url = new LaParolaUrl("laparola", null, riferimento, null, getVersione(), getVersioneCommentario(), segnalibro, this);
		return url;
	}

	public void vaiAPrecendente() {
		if (!precedenteEsiste())
			return;

		mAvantiIndietroIndice--;
		vaiAdUrl(mAvantiIndietroUrl.get(mAvantiIndietroIndice), false);
	}

	public void vaiARicerca(CharSequence text) {
		if (text.length() > 0) {
			vaiAdUrl(new LaParolaUrl("laparola", null, null, text.toString(), getVersione(), getVersioneCommentario(), null, this));
		}
	}

	public void vaiARicerca(CharSequence text, CharSequence brano) {
		if (text.length() > 0) {
			vaiAdUrl(new LaParolaUrl("laparola", null, brano.toString(), text.toString(), getVersione(), getVersioneCommentario(), null, this));
		}
	}

	public void vaiARiferimento(CharSequence ref) {
		if (ref.length() > 0) {
			vaiAdUrl(new LaParolaUrl("laparola", null, ref.toString(), null, getVersione(), getVersioneCommentario(), null, this));
		}
	}

	public void vaiASegnalibro(String segnalibro) {
		if (mClient == null)
			return;
		mClient.vaiAdAncoraggio(segnalibro);
		mUrlCorrente.ancoraggio = segnalibro;
	}

	public void vaiASuccessivo() {
		if (!successivoEsiste())
			return;

		mAvantiIndietroIndice++;
		vaiAdUrl(mAvantiIndietroUrl.get(mAvantiIndietroIndice), false);
	}

	public static boolean isRiferimento(String brani, String versione) {
		Riferimento riferimento = LaParolaBrowser.creaRiferimento(brani, versione);
		return riferimento.getBrani().size() > 0;
	}

	public static String getUltimaBibbia() {
		String res = "";
		if (mStaticClient != null) {
			res = mStaticClient.getUltimaBibbiaSalvata();
		}
		
		if (res.equals("") && mTesti != null) {
			for (ComponenteInformazioni v : getTestiInstallati()) {
				if (v.getTipo().contains(TestoTipi.BIBBIA)) {
					res = v.getComponente();
					break;
				}
			}
		}
		
		return res;
	}

	public CharSequence getBrano(LaParolaUrl url) {
		return mLaParolaTesti.getBrano(url);
	}

	public CharSequence getTestoCorrente() {
		return mTestoCorrente;
	}

	public void onCaricamentoFinito(boolean coloraTesto) {
		Evidenziatore.evidenziaVersetti(coloraTesto);
	}
	
	public static void setNomeFileDebug (String fn) {
		LaParolaTesto.mDebugFileName = fn;
	}
	
	public static List<LaParolaNote.NotaOGruppo> elencaNoteInTesto(String versione) {
		StringBuilder url = new StringBuilder();
		List<LaParolaNote.NotaOGruppo> res = new ArrayList<LaParolaNote.NotaOGruppo>();
		
		synchronized (mTesti) {
			List<String> note;
			try {
				note = mTesti.note(versione);
			} catch (TestoNonEsisteException e) {
				return null;
			}
			
			for (String nota : note) {
				LaParolaNote.Nota lpnota = new LaParolaNote.Nota();
				
				if (nota.startsWith("#")) {
					lpnota.titolo = mTesti.convertiTitoloNotaARiferimento(nota);
					lpnota.conTitolo = false;
				} else {
					lpnota.titolo = nota;
					lpnota.conTitolo = true;
				}
				
				url.setLength(0);
				url.append("laparola:"); 
				if (!nota.startsWith("#")) {
					url.append('$');
				}
				url.append(lpnota.titolo);
				url.append('@'); 
				url.append(versione); 
				
				lpnota.url = url.toString();
				
				res.add(lpnota);
			}
		}
		
		return res;
	}

	public String getVersioneCommentario() {
		return mVersioneCommentario;
	}
}
