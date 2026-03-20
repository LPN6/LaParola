package net.laparola.ui.android;

import android.os.Bundle;
import android.os.Handler;
import android.text.Editable;
import android.text.Html;
import android.text.Spanned;
import android.text.style.ClickableSpan;
import android.text.style.URLSpan;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.LinearLayout.LayoutParams;

import androidx.fragment.app.Fragment;

import net.laparola.core.ComponenteInformazioni;
import net.laparola.core.Testi.TestoTipi;
import net.laparola.core.VersioneInformazioni;
import net.laparola.ui.LaParolaBrowser;
import net.laparola.ui.LaParolaUrl;
import net.laparola.ui.android.bibleview.BibleView;
import net.laparola.ui.android.bibleview.BibleView.OnBibleViewListener;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Comparator;
import java.util.List;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

public class LaParolaFragment extends Fragment implements OnBibleViewListener {
    public class AnchorAndText {
		public String anchor;
		public String link;
		public String text;
		
		public AnchorAndText(String anchor, String link, String text) {
			this.anchor = anchor;
			this.link = link;
			this.text = text;
		}
	}
	
	public interface MyRunnable {
		void run(LaParolaFragment self);
	}

	private BibleView bibleView;
	private LaParolaBrowser laParolaBrowser;
	private LaParolaActivity parent;
	private boolean mLoading;
	private int mSyncColor;
	
	public MyRunnable onCreateViewRunnable;
	public Runnable onProssimaPaginaCaricata;
	private boolean mIgnoreNextUrlForSync;
	private Handler mHandler;
	private Runnable mSyncPanelsRunnable;
	public String onCreateGoToUrl;
	private boolean mCreated;

	@Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
		parent = (LaParolaActivity)getActivity();
		
		mHandler = new Handler();
		mSyncPanelsRunnable = new Runnable() {
			@Override
			public void run() {
				parent.syncPanels(LaParolaFragment.this);
			}
		};
		
		bibleView = new BibleView(parent, null);
		bibleView.setParents((LaParolaActivity)getActivity(), this);
		
		//bibleView.setLayoutParams(new LayoutParams(LayoutParams.MATCH_PARENT, 0, 1));
		bibleView.setLayoutParams(new LayoutParams(LayoutParams.MATCH_PARENT, LayoutParams.MATCH_PARENT));
		
		bibleView.setOnBibleViewListener(this);
		laParolaBrowser = bibleView.getBrowser();
		bibleView.setNightMode(LaParolaPreferences.nightMode);
				
		if (onCreateViewRunnable != null) {
			onCreateViewRunnable.run(this);
			onCreateViewRunnable = null;
		}
		
		mCreated = true;
		
		return bibleView;
	}

    public void aggiornaPreferenze() {
        if (bibleView != null)
            bibleView.aggiornaPreferenze();
    }

    public void onStartLoading(BibleView view) {
		mLoading = true;
		parent.onLoadingChanged();
	}

	public void onFinishedLoading(final BibleView view) {
		if (!parent.hasFinishedLoading()) {
			// potrebbe finire il caricamento prima di preparare
			// l'action bar
			view.postDelayed(new Runnable() {
				@Override
				public void run() {
					onFinishedLoading(view);
				}
			}, 100);
			return;
		}

		mLoading = false;
		
		parent.onLoadingChanged();
		parent.updateActionBar();
				
		if (!mIgnoreNextUrlForSync) {
			parent.syncPanels(this);
		} else {
			mIgnoreNextUrlForSync = false;
		}
		
		if (onProssimaPaginaCaricata != null) {
			onProssimaPaginaCaricata.run();
			onProssimaPaginaCaricata = null;
		}
		
		if (parent.actionMode != null)
			attivaEvidenziatore(true);
		
		if (laParolaBrowser.inHome()) {
			bibleView.executeJavascript(parent.javaScriptPerOpzioniHomePage());
		}

        LaParolaUrl urlCorrente = laParolaBrowser.getUrlCorrente();
        if (urlCorrente != null && urlCorrente.gestito && urlCorrente.schema.equals("lpevidenziati")) {
            parent.startHighlighter();
        }
		
		laParolaBrowser.onCaricamentoFinito(getNightMode());
	}

	@Override
	public void onAnchorChanged(BibleView bibleView, String newAnchor) {
		mHandler.removeCallbacks(mSyncPanelsRunnable);
		mHandler.postDelayed(mSyncPanelsRunnable, 500);
	}

	public void setIgnoreNextUrlForSync(boolean value) {
		this.mIgnoreNextUrlForSync = value;
	}
	
	public void onTestiCambiati () {
		if (laParolaBrowser != null) {
			boolean ok = setVersione(laParolaBrowser.getVersione());
			if (!ok) {
				for (ComponenteInformazioni c : LaParolaBrowser.getTestiInstallati()) {
					if (c.getTipo().contains(TestoTipi.BIBBIA)) {
						setVersione(c.getComponente());
    					break;
                    }
				}
				//onVersionChanged(bibleView);
			}
		}
	}
	
	public void onVersionChanged(BibleView view) {
		parent.onVersionChanged();
	}

	@Override
	public void onZoomChanged(BibleView bibleView, int zoom) {
		parent.onZoomChanged(this, zoom);
	}
	
	public boolean isLoading() {
		return mLoading;
	}

	public int getSyncColor() {
		return mSyncColor;
	}

	public void setSyncColor(int mSyncColor) {
		this.mSyncColor = mSyncColor;
	}

	public List<AnchorAndText> getPlainTextWithAnchors() {
		Pattern rbody = Pattern.compile("<body.*?>(.*?)</body>", Pattern.CASE_INSENSITIVE | Pattern.DOTALL);
		Pattern rcomment = Pattern.compile("<!--.*?-->", Pattern.CASE_INSENSITIVE | Pattern.DOTALL);
		
		CharSequence testoCorrente = laParolaBrowser.getTestoCorrente();
		Matcher m = rbody.matcher(testoCorrente);
		if (m.find()) {
			testoCorrente = m.group(1);
		}
		
		m = rcomment.matcher(testoCorrente);
		testoCorrente = m.replaceAll("");
		
		final Spanned html = Html.fromHtml(testoCorrente.toString());
		
		List<AnchorAndText> res = new ArrayList<AnchorAndText>();
		
		int i = 0;
		res.add(new AnchorAndText(null, null, null));

        // gli span non sono ordinati su android 7+
        URLSpan[] spans = html.getSpans(0, html.length(), URLSpan.class);
        Arrays.sort(spans, new Comparator<ClickableSpan>() {
            @Override
            public int compare(ClickableSpan o1, ClickableSpan o2) {
                return html.getSpanStart(o1)-html.getSpanStart(o2);
            }
        });

        for (URLSpan s : spans) {
			int start = html.getSpanStart(s);
			int end = html.getSpanEnd(s);

            Log.d("laparola", "---");
			String t =  html.subSequence(i, start).toString();
			Log.d("laparola", t);
			String a = html.subSequence(start, end).toString();
			Log.d("laparola", a);
			res.get(res.size() - 1).text = t;
			res.add(new AnchorAndText(a, s.getURL(), null));
			i = end;
		}
		if (!res.isEmpty()) {
			res.get(res.size() - 1).text = html.subSequence(i, html.length()).toString();
		}
		
		return res;
	}
	
	public boolean isCreated() {
		return mCreated;
	}
	
	// === Inizio proxy === 
	// TODO : ma ne vale la pena?
	// Il tutto è nato per essere sicuro che tutto fosse ok nel refactoring
	// della Activity in diversi Fragment, ma ora potrei rendere pubblici 
	// laParolaBrowser e bibleView.
	// D'altro canto, però, forse questo sistema è più "pulito".
	// Se Java avesse l'ereditarietà multipla, basterebbe far ereditare questa
	// classe da LaParolaBrowser.

	public boolean pageDown(boolean bottom) {
		return bibleView.pageDown(bottom);
	}

	public void goToNextUrl() {
		bibleView.goToNextUrl();
	}

	public boolean pageUp(boolean top) {
		return bibleView.pageUp(top);
	}

	public void goToPreviousUrl() {
		bibleView.goToPreviousUrl();
	}

	public void setNightMode(boolean value) {
		if (bibleView != null) {
			// altrimenti lo erediterà da parent
			bibleView.setNightMode(value);
		}
	}
	
	public boolean getNightMode() {
		if (bibleView != null) {
			return bibleView.getNightMode();
		}
		return false;
	}

	public int getTextZoom() {
		return bibleView.getTextZoom();
	}

	public void setTextZoom(int zoom, boolean showToast) {
		bibleView.setTextZoom(zoom, showToast);
	}
	
	public void setTextZoom(int zoom) {
		bibleView.setTextZoom(zoom);
	}
	
	public boolean precedenteEsiste() {
		return laParolaBrowser.precedenteEsiste();
	}

	public void vaiAPrecendente() {
		laParolaBrowser.vaiAPrecendente();
	}

	public String getVersione() {
		if (laParolaBrowser == null) {
			return null;
		}
		return laParolaBrowser.getVersione();
	}

	public LaParolaUrl getUrlCorrente() {
		if (laParolaBrowser == null)
			return null;
		return laParolaBrowser.getUrlCorrente();
	}

	public void vaiAHome() {
		laParolaBrowser.vaiAHome();
	}

	public void vaiAdUrl(String url) {
		laParolaBrowser.vaiAdUrl(url);
	}

	public void vaiAdUrl(LaParolaUrl url) {
		laParolaBrowser.vaiAdUrl(url);
	}

	public void vaiASuccessivo() {
		laParolaBrowser.vaiASuccessivo();
	}

	public void selectAndCopyText() {
		bibleView.selectAndCopyText();
	}

	public void aggiornaPagina() {
		if (laParolaBrowser == null)
			return;
					
		final LaParolaUrl urlCorrente = laParolaBrowser.getUrlCorrente();
		if (urlCorrente == null || !urlCorrente.gestito || urlCorrente.schema.equals("null")) {
			// workaround
			if (onCreateViewRunnable != null) {
				onCreateViewRunnable.run(this);
				onCreateViewRunnable = null;
			} else {
				laParolaBrowser.vaiAHome();
			}
		} else {
			laParolaBrowser.aggiornaPagina();
		}
	}

	public boolean setVersione(String nomeVersione) {
		if (laParolaBrowser == null) {
			return false;
		}
		return laParolaBrowser.setVersione(nomeVersione);
	}

	public boolean successivoEsiste() {
		return laParolaBrowser.successivoEsiste();
	}

	public boolean inHome() {
		if (laParolaBrowser == null)
			return true;   // workaround per impostare il titolo
		return laParolaBrowser.inHome();
	}

	public void vaiARicerca(Editable text, Editable reference) {
		laParolaBrowser.vaiARicerca(text, reference);
	}

	public void vaiARicerca(Editable text) {
		laParolaBrowser.vaiARicerca(text);
	}

	public void vaiARiferimento(Editable ref) {
		laParolaBrowser.vaiARiferimento(ref);
	}

	public void vaiALibroCapitoloVersetto(int b, int c, int v) {
		laParolaBrowser.vaiALibroCapitoloVersetto(b, c, v);
	}

	public int getCapitoliInLibro(int b) {
		return laParolaBrowser.getCapitoliInLibro(b);
	}

	public int getVersettiInCapitolo(int book, int chapter) {
		return laParolaBrowser.getVersettiInCapitolo(book, chapter);
	}

	public VersioneInformazioni getInformazioniVersione() {
		return LaParolaBrowser.getInformazioniTesto(getVersione());
	}

	public void vaiASegnalibro(String segnalibro) {
		laParolaBrowser.vaiASegnalibro(segnalibro);
	}

	public boolean attivaEvidenziatore(boolean attivo) {
		return laParolaBrowser.Evidenziatore.attivaEvidenziatore(attivo);
	}

	public void evidenziaVersetto(String versetto, boolean evidenzia, boolean coloraTesto) {
		laParolaBrowser.Evidenziatore.evidenziaVersetto(versetto, evidenzia, coloraTesto);
	}

	public void setColoreEvidenziatore(String colore) {
		laParolaBrowser.Evidenziatore.setColore(colore);
	}

    public LaParolaUrl nuovoUrl(String url) {
        return laParolaBrowser.nuovoUrl(url);
    }
}
