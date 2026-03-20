package net.laparola.ui;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.FileOutputStream;
import java.io.FileWriter;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.io.Writer;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Date;
import java.util.List;
import java.text.DateFormat;

public class LaParolaCronologia {
	public class ElementoCronologia implements Comparable<ElementoCronologia> {
		public LaParolaUrl url;
		public Date data;

		public ElementoCronologia(LaParolaUrl url, Date data) {
			super();
			this.url = url;
			this.data = data;
		}

		@Override
		public int compareTo(ElementoCronologia another) {
			return -data.compareTo(another.data);
		}

        @Override
        public boolean equals(Object obj) {
            if (obj.getClass() != this.getClass())
                return false;
            ElementoCronologia e = (ElementoCronologia)obj;
            return (e.url.getUrl() == this.url.getUrl()) && e.data == this.data;
        }
    }

	private static int CONSERVA_PER_GIORNI = 30;
	private static int SPAZIO_TRA_MINUTI = 10;

	private static final long CONSERVA_PER = CONSERVA_PER_GIORNI * 24l * 60 * 60 * 1000;
	private static final long SPAZIO_TRA = SPAZIO_TRA_MINUTI * 60 * 1000;

	private List<ElementoCronologia> mLista = new ArrayList<ElementoCronologia>();

	public void aggiungi(LaParolaUrl url, Date data) {
        synchronized (mLista) {
            ElementoCronologia e =  new ElementoCronologia(url, data);
            // potrebbe essere troppo lento controllare per ciascuno
            // li aggiungo tutti, poi dopo l'ordinamento elimino i duplicati

            //if (!mLista.contains(e)) {
                mLista.add(0, e);
            //}
        }
	}

	public void ordina () {
        synchronized (mLista) {
            Collections.sort(mLista);
            for (int i = 0; i < mLista.size() - 1; i++) {
                ElementoCronologia e1 = mLista.get(i);
                ElementoCronologia e2 = mLista.get(i+1);
                if (e1.compareTo(e2) == 0) {
                    mLista.remove(i+1);
                    i--;
                }
            }
        }
    }

	public void pulisci() {
        synchronized (mLista) {
            mLista.clear();
        }
	}

	public CharSequence getPagina(LaParolaBrowser browser) {
        synchronized (mLista) {
            StringBuilder res = new StringBuilder();

            res.append("<html><head>\n");
            res.append(LaParolaStringhe.get(LaParolaStringhe.CRONOLOGIA_HEADER, "file:///android_asset/laparola.css"));
            if (browser.mClient != null)
                res.append(browser.mClient.getAggiuntaHeader(null));
            res.append("</head><body>\n");

            res.append("<table width='100%' cellspacing='1' cellpadding='1' border='0'><tr>");
            res.append("<td valign='baseline' align='left'>");
            res.append("<h2 style='margin-top:0px;margin-bottom:0px;'>");
            res.append(LaParolaStringhe.get(LaParolaStringhe.CRONOLOGIA));
            res.append("</h2>\n");
            res.append("</td>");
            res.append("<td valign='baseline' align='right'>");
            if (mLista.size() > 0) {
                res.append(LaParolaStringhe.get(LaParolaStringhe.PULISCI_CRONOLOGIA));
            }
            res.append("</td>");
            res.append("</tr></table>");

            Collections.sort(mLista);

            String ultimaData = null;
            long ultimoTime = 0;
            DateFormat df = DateFormat.getDateInstance(DateFormat.LONG);
            DateFormat tf = DateFormat.getTimeInstance(DateFormat.SHORT);

            for (ElementoCronologia e : mLista) {
                String data = df.format(e.data);
                if (!data.equals(ultimaData)) {
                    ultimaData = data;
                    res.append("<h4>");
                    res.append(data);
                    res.append("</h4>");
                }

                if (ultimoTime != 0 && ultimoTime - e.data.getTime() > SPAZIO_TRA) {
                    res.append("<p style='margin-top:1.75em;margin-bottom:0px;'>");
                } else {
                    res.append("<p style='margin-top:0px;margin-bottom:0px;'>");
                }
                ultimoTime = e.data.getTime();

                res.append("<table width='100%' cellspacing='1' cellpadding='1' border='0'><tr>");

                res.append("<td valign='top' align='left' class='oracella'>");
                res.append("<span class='ora'>");
                res.append(tf.format(e.data));
                res.append("</span>");
                res.append("</td>");

                res.append("<td valign='top' align='left'>");
                res.append("<a href='");
                res.append(e.url.getUrl());
                res.append("'>");
                res.append(e.url.getDescrizione());
                res.append("</a>");
                res.append("</td>");

                res.append("</tr></table>");

                res.append("</p>\n");
            }

            res.append("</body></html>\n");

            return res.toString();
        }
	}

	public void caricaDaFile(String nomeFile) {
        synchronized (mLista) {
            synchronized (LaParolaBrowser.DataLock) {
                BufferedReader reader;
                try {
                    reader = new BufferedReader(new InputStreamReader(LaParolaBrowser.apriFile(nomeFile), "UTF-8"));
                    String strLine;
                    while ((strLine = reader.readLine()) != null) {
                        int i = strLine.indexOf(',');
                        if (i > 0) {
                            try {
                                long date = Long.parseLong(strLine.substring(0, i));
                                LaParolaUrl url = LaParolaBrowser.nuovoUrl(strLine.substring(i + 1), null, null);
                                aggiungi(url, new Date(date));
                            } catch (Exception e) {
                                //
                            }
                        }
                    }
                } catch (Exception e) {
                    return;
                }
            }
            }
	}

	public void salvaSuFile(String nomeFile) {
        synchronized (mLista) {
            synchronized (LaParolaBrowser.DataLock) {
                Writer writer;
                try {
                    writer = new BufferedWriter(new OutputStreamWriter(new FileOutputStream(nomeFile), "UTF-8"));
                } catch (Exception e1) {
                    return;
                }

                long now = new Date().getTime();

                try {
                    writer = new FileWriter(nomeFile);
                    for (ElementoCronologia e : mLista) {
                        if ((now - e.data.getTime()) < CONSERVA_PER) {
                            writer.append(String.valueOf(e.data.getTime()));
                            writer.append(',');
                            writer.append(e.url.getUrl());
                            writer.append('\n');
                        }
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
            }
        }
	}
}
