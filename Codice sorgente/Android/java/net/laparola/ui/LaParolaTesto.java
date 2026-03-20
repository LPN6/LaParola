package net.laparola.ui;

import android.util.LruCache;

import net.laparola.BuildConfig;
import net.laparola.core.Riferimento;
import net.laparola.core.TestoNonEsisteException;
import net.laparola.ui.utils.TaskAsincrono;

import java.io.InputStream;
import java.io.InputStreamReader;
import java.nio.charset.Charset;

/* package */ class LaParolaTesto {
    private class TaskGetBranoAsincrono extends TaskAsincrono<CharSequence> {
    	
        public LaParolaUrl mUrl;
		public boolean mInserisciInCronologia;
    
        public TaskGetBranoAsincrono (LaParolaUrl url, boolean inserisciInCronologia) {
            mUrl = url;
            mInserisciInCronologia = inserisciInCronologia;
        }
    
        @Override
		public void annulla() {
			super.annulla();
        	synchronized (LaParolaBrowser.mTesti) {
				LaParolaBrowser.mTesti.interrompiGetBrano(mUrl.versione);
        	}
        }

        @Override
        protected CharSequence lavoraInBackground() {
            return getBrano(mUrl);
        }

        @Override
        protected void onAnnullato() {
        	// onFinito del task successivo sarà eseguito 
        }

		@Override
        protected void onFinito(CharSequence risultato) {
	        synchronized (mBrowser) {
				// non verrà eseguito in caso di annullamento (quindi risultato == null)
	            mBrowser.mUrlCorrente = mUrl;
	            mBrowser.mTestoCorrente = risultato;

	            if (mBrowser.mClient != null) {
	            	mBrowser.mClient.visualizzaTesto(risultato, mUrl);
	            }
	            if (mInserisciInCronologia) {
	                mBrowser.aggiungiUrlACronologia(mUrl);
	            }
	        }
        }
    }
    
    private static final int CACHE_SIZE = 10;

    private static final LruCache<String, CharSequence> mCache = new LruCache<String, CharSequence>(CACHE_SIZE);
    
    

	/* internal */ static String mDebugFileName = null;

	private LaParolaBrowser mBrowser;
	private TaskGetBranoAsincrono mTaskCorrente = null;
	private StringBuilder _tmpStringBuilder = new StringBuilder();
    
    public LaParolaTesto (LaParolaBrowser browser) {
    	mBrowser = browser;
    }
    
	public CharSequence getBrano (LaParolaUrl url) {
		CharSequence res;
		if (url.cacheAttiva && mCache != null) {
            String key = url.getChiaveCache();
            
            synchronized (mCache) {
				res = mCache.get(key);
			}
			if (res == null) {
				res = getBranoSenzaCache(url);
			}
			synchronized (mCache) {
				if (res != null) {
					mCache.put(key, res);
				} else {
					mCache.remove(key);
				}
			}
        } else {
        	res = getBranoSenzaCache(url);
        }
		
		if (BuildConfig.DEBUG && mDebugFileName != null) {
			try {
				if (res != null) {
					java.io.FileWriter out = new java.io.FileWriter(mDebugFileName);
					out.write(res.toString());
					out.close();
				}
				//Log.d("LaParola", res.toString());
			} catch (Exception e) {
				e.printStackTrace();
			}
		}
		
        return res;
    }

	public CharSequence getBrano (String url) {
	    return getBrano(mBrowser.nuovoUrl(url));
	}

	private CharSequence getBranoSenzaCache (final LaParolaUrl url) {
	    if (!url.gestito) {
	    	return null;
	    }
	    
		if (url.schema.equals("laparola")) {
			return getBranoLaParola(url, null, LaParolaStringhe.get(LaParolaStringhe.ERRORE_BRANO_NON_PRESENTE, url.versione));
	    } else if (url.schema.equals("lpfile")) {
	        String file = leggiFile(url.brani);
	        if (mBrowser.mClient != null && file != null)
	        	file = file.replace("<!--[LaParolaHeader]-->", mBrowser.mClient.getAggiuntaHeader(url));
	        return file;
        } else if (url.schema.equals("lpsegnalibri")) {
        	return LaParolaBrowser.mSegnalibri.getPaginaGruppi(mBrowser);
        } else if (url.schema.equals("lpsegnalibro")) {
        	return LaParolaBrowser.mSegnalibri.getPaginaGruppo(url.brani, url.versione, mBrowser);
        } else if (url.schema.equals("lppreferiti")) {
        	return LaParolaBrowser.mPreferiti.getPaginaGruppo(null, null, mBrowser);
        } else if (url.schema.equals("lpcronologia")) {
        	return LaParolaBrowser.mCronologia.getPagina(mBrowser);
	    } else if (url.schema.equals("error")) {
		    return LaParolaStringhe.get(LaParolaStringhe.ERRORE_URL, url.requesturl);
	    } else if (url.schema.equals("lpevidenziati")) {
	    	Riferimento rif = LaParolaEvidenziatore.getRiferimentoVersettiEvidenziati(url.versione);
			return getBranoLaParola(url, rif, LaParolaStringhe.get(LaParolaStringhe.ERRORE_NESSUN_VERSETTO_EVIDENZIATO));
	    } else if (url.schema.equals("null")) {
	        return "";
	    }
		
	    return null;
	}

	private CharSequence getBranoLaParola(final LaParolaUrl url, Riferimento rif, String errore) {
		if (url.versione.isEmpty()) {
			return LaParolaStringhe.get(LaParolaStringhe.ERRORE_NESSUNA_VERSIONE);
		}
		CharSequence res;
		synchronized (LaParolaBrowser.mTesti) {
			Riferimento urlrif = (rif == null) ? url.getRiferimento() : rif;
			if (urlrif != null) {
				try {
					res = LaParolaBrowser.mTesti.getBrano(urlrif, url.versione, url.versioneCommentario);
				} catch (TestoNonEsisteException e) {
					StringBuilder sb = new StringBuilder();

                    sb.append(LaParolaStringhe.get(LaParolaStringhe.ERRORE_VERSIONE_NON_PRESENTE, e.getMessage()));

					for (String versione : LaParolaBrowser.getNomiVersioni()) {
						String nuovourl = url.getUrlConAltraVersione(versione, "");   // elimina il commentario
						sb.append(LaParolaStringhe.get(LaParolaStringhe.VISUALIZZA_BRANO_IN, nuovourl, versione));
					}
				
					return sb.toString();
				}
			} else {
				if (url.brani != null && url.brani.startsWith("$")) {
					if (url.brani.equals("$$")) {
						// è la lista di tutte le note
						LaParolaNote laParolaNote = new LaParolaNote();
						res = laParolaNote.creaListaNote(url);
					} else {
						// è un riferimento ad una nota con titolo
						try {
							String nota = url.brani.substring(1);
							StringBuilder sb = new StringBuilder();
							sb.append("<h3>");
							if (nota.startsWith(";")) {
								nota = nota.replace(';', '#');
								sb.append(LaParolaBrowser.mTesti.convertiTitoloNotaARiferimento(nota));
							} else {
								sb.append(nota);
							}
							sb.append("</h3><p>");
							sb.append(LaParolaBrowser.mTesti.getNotaConTitolo(nota, url.versione));
							sb.append("</p>");
							res = sb;
						} catch (TestoNonEsisteException e) {
							res = "";
						}
					}
				} else {
					if (url.testo != null) {
						res = url.testo;
					} else {
						res = "";
					}
				}
			}
		}

        CharSequence aggiuntaHeader = "";

        if (mBrowser.mClient != null) {
            aggiuntaHeader = mBrowser.mClient.getAggiuntaHeader(url);
        }

		if (res == null || aggiuntaHeader == null) {
			// la richiesta è stata annullata, altrimenti sarebbe
			// stata restituita una stringa con l'errore o una stringa vuota
			// restituendo null non lo mette nella cache
			return null;
		}
		    
		if (res.isEmpty()) {
		    return errore;
		}
		// se length()>0
		CharSequence preres = getPreLink(url);
		CharSequence postres = getPostLink(url);

		if (res instanceof StringBuilder) {
			StringBuilder sb = (StringBuilder)res;
		    sb.insert(0, LaParolaStringhe.get(LaParolaStringhe.ANCHOR_INIZIO));
		    if (preres != null)
		        sb.insert(0, preres);
		    sb.insert(0, LaParolaStringhe.get(LaParolaStringhe.HTML_HEADER, aggiuntaHeader));
		    if (postres != null)
		        sb.append(postres);
		    sb.append(LaParolaStringhe.get(LaParolaStringhe.ANCHOR_FINE));
		    sb.append(LaParolaStringhe.get(LaParolaStringhe.HTML_FOOTER));
		    return sb.toString();
		}
		// se non instanceof StringBuilder
		synchronized (_tmpStringBuilder) {
			_tmpStringBuilder.setLength(0);
			_tmpStringBuilder.append(LaParolaStringhe.get(LaParolaStringhe.HTML_HEADER, aggiuntaHeader));
		    if (preres != null)
		        _tmpStringBuilder.append(preres);
		    _tmpStringBuilder.append(LaParolaStringhe.get(LaParolaStringhe.ANCHOR_INIZIO));
		    _tmpStringBuilder.append(res);
		    if (postres != null)
		        _tmpStringBuilder.append(postres);
		    _tmpStringBuilder.append(LaParolaStringhe.get(LaParolaStringhe.ANCHOR_FINE));
		    _tmpStringBuilder.append(LaParolaStringhe.get(LaParolaStringhe.HTML_FOOTER));
		    return _tmpStringBuilder.toString();
		}
	}

	private static CharSequence getPreLink(LaParolaUrl url) {
		String rif = url.getUrlPrecedente();
		
		if (rif == null)
			return null;
		
		Riferimento urlrif = url.getRiferimento();
		if (urlrif != null) {
	        int[] rifbrano = urlrif.getBrani().get(0);
			int v1 = rifbrano[2];
			
			if (v1 == 1) {
				return LaParolaStringhe.get(LaParolaStringhe.MOSTRA_CAPITOLO_PRECEDENTE, rif);
			}
			// se v1!=1
			return LaParolaStringhe.get(LaParolaStringhe.MOSTRA_CAPITOLO_INTERO_INIZIO, rif);
		}
		// se url.riferimento == null
		return LaParolaStringhe.get(LaParolaStringhe.MOSTRA_CAPITOLO_PRECEDENTE, rif);
	}

	private static CharSequence getPostLink(LaParolaUrl url) {
		String rif = url.getUrlSuccessivo();
		
		if (rif == null)
			return null;
		
		Riferimento urlrif = url.getRiferimento();
		if (urlrif != null) {
	        int[] rifbrano = urlrif.getBrani().get(0);
			int l2 = rifbrano[3];
			int c2 = rifbrano[4];
			int v2 = rifbrano[5];
			int versettiInUltimoCapitolo = LaParolaBrowser.mTesti.versettiInCapitolo(l2, c2, url.versione);
			
			if (v2 >= versettiInUltimoCapitolo) {
		        return LaParolaStringhe.get(LaParolaStringhe.MOSTRA_CAPITOLO_SUCCESSIVO, rif);
			}
			// se v2<versettiInUltimoCapitolo
			return LaParolaStringhe.get(LaParolaStringhe.MOSTRA_CAPITOLO_INTERO_FINE, rif);
		}
		// se url.riferimento == null
		return LaParolaStringhe.get(LaParolaStringhe.MOSTRA_CAPITOLO_SUCCESSIVO, rif);
	}

	public void mostraBranoInBrowser(final LaParolaUrl url,	boolean inserisciInCronologia) {

		if (mTaskCorrente != null && !mTaskCorrente.finito() && !mTaskCorrente.annullato()) {
		    if (mBrowser.mUrlCorrente != null && mBrowser.mUrlCorrente.stessoTestoDi(mTaskCorrente.mUrl)) {
		    	mTaskCorrente.mUrl = url;   // nuovo segnalibro
		        return;
		    }
		    mTaskCorrente.annulla();
		}
		
		mTaskCorrente = new TaskGetBranoAsincrono(url, inserisciInCronologia);
		mTaskCorrente.esegui();
	}
    
	private static String leggiFile (String filename) {
		StringBuilder str = new StringBuilder();
	
		try {
            InputStream inp = LaParolaBrowser.apriFile(filename);
            
			InputStreamReader fin = new InputStreamReader(inp, Charset.forName("ISO-8859-1"));
		    char[] buffer = new char[1024];
		    int bufferLength = 0;
		    while ((bufferLength = fin.read(buffer)) > 0 ) {
		    	str.append(buffer, 0, bufferLength);
		    }
	        
	        fin.close();
		} catch (Exception e) {
			e.printStackTrace();
			return null;
		}
		
	    return str.toString();
	}

	public static void pulisciCache() {
		synchronized (mCache) {
			mCache.evictAll();
		}
	}
}
