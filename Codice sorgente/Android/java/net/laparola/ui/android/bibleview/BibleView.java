package net.laparola.ui.android.bibleview;

// ricerca nel brano:
// findAll("parola");
// Method m = WebView.class.getMethod("setFindIsUp", Boolean.TYPE);
// m.invoke(webView, true);
// boolean ok = findNext();

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.Context;
import android.os.Build;
import android.os.Parcelable;
import android.util.AttributeSet;
import android.view.ContextMenu;
import android.view.MenuItem;
import android.view.MotionEvent;
import android.view.View;
import android.webkit.ConsoleMessage;
import android.webkit.WebChromeClient;
import android.webkit.WebSettings;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.Toast;

import net.laparola.BuildConfig;
import net.laparola.R;
import net.laparola.core.VersioneInformazioni;
import net.laparola.ui.LaParolaBrowser;
import net.laparola.ui.LaParolaBrowser.LaParolaBrowserClient;
import net.laparola.ui.LaParolaUrl;
import net.laparola.ui.android.InstallGentiumHelper;
import net.laparola.ui.android.LaParolaActivity;
import net.laparola.ui.android.LaParolaFragment;
import net.laparola.ui.android.LaParolaJavascriptInterfaceAndroid;
import net.laparola.ui.android.LaParolaPreferences;
import net.laparola.ui.android.dialogs.MessageDialog;

import java.net.URLDecoder;
import java.net.URLEncoder;
import java.util.Locale;

import timber.log.Timber;

@SuppressLint("SetJavaScriptEnabled")
public class BibleView extends WebView implements LaParolaBrowserClient {
    class BibleWebViewClient extends WebViewClient {
        @Override
        public boolean shouldOverrideUrlLoading(WebView view, String url) {
            try {
                url = URLDecoder.decode(url, "UTF-8");
            } catch (Exception e) {
                //
            }
            LaParolaUrl purl = mLaParolaBrowser.nuovoUrl(url);
            if (purl.gestito) {
                if (purl.richiedeNuovaFinestra(mLaParolaBrowser.getUrlCorrente()) && mFragment != null) {
                    mParentActivity.openInPopupWindow(purl);
                } else {
                    mLaParolaBrowser.vaiAdUrl(purl);
                }
                return true;
            }
            return false;
        }

        @Override
        public void onPageFinished(WebView view, String url) {
            final BibleView bibleView = (BibleView) view;
            if (mGoToAnchor != null) {
                bibleView.vaiAdAncoraggio(mGoToAnchor);
                mGoToAnchor = null;
            }

            StringBuilder css = appendCSS(null);
            applyCSS(css);

            finishedLoading();
            bibleView.setupOnScroll();
        }
    }

    class BibleWebChromeClient extends WebChromeClient {
        @Override
        public boolean onConsoleMessage(ConsoleMessage cm) {
            String errore = cm.message() + " -- From line "
                    + cm.lineNumber() + " of "
                    + cm.sourceId();
            Timber.tag("laparola").d(errore);

            if (BuildConfig.DEBUG) {
                MessageDialog d = new MessageDialog(mContext, "Errore WebView", errore);
                d.show();
            }

            return true;
        }
    }

    public interface OnBibleViewListener {
        void onStartLoading(BibleView view);

        void onFinishedLoading(BibleView view);

        void onVersionChanged(BibleView view);

        void onZoomChanged(BibleView bibleView, int mTextZoom);

        void onAnchorChanged(BibleView bibleView, String newAnchor);
    }

    private final LaParolaBrowser mLaParolaBrowser;
    private OnBibleViewListener mOnBibleViewListener = null;
    private String mGoToAnchor;
    private final Context mContext;
    //private boolean mIsFrozen;
    private int mTextZoom = 100;
    private final BibleViewTouchHandler mTouchHandler;
    private final Toast mZoomToast;
    private boolean mNightMode;
    private boolean mLoading;
    private boolean mIgnoreNextScroll;
    private LaParolaActivity mParentActivity;
    private LaParolaFragment mFragment;
    protected boolean mSkipNextContextMenu;
    private boolean longPressCanceled;

    private float lastTouchX, lastTouchY;
    final static int ID_APRI = 0;
    final static int ID_APRI_NUOVO_PANNELLO = 1;
    final static int ID_APRI_PANNELLO_ESISTENTE = 2;
    final static int ID_APRI_POPUP = 3;

    @SuppressLint("ShowToast")
    public BibleView(Context context, AttributeSet attrs) {
        super(context, attrs);

        mContext = context;
        if (mContext instanceof Activity)
            ((Activity) mContext).registerForContextMenu(this);

        setWebChromeClient(new BibleWebChromeClient());
        setWebViewClient(new BibleWebViewClient());
        WebSettings mSettings = getSettings();
        mSettings.setJavaScriptEnabled(true);
        mSettings.setBuiltInZoomControls(false);

        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.R) { // 30
            mSettings.setAllowFileAccess(true);
        }

        mLaParolaBrowser = new LaParolaBrowser();
        mLaParolaBrowser.setLaParolaBrowserClient(this);

        mTouchHandler = new BibleViewTouchHandler(this);

        addJavascriptInterface(new LaParolaJavascriptInterfaceAndroid(this), "LaParola");

        mZoomToast = Toast.makeText(mContext, "", Toast.LENGTH_SHORT);
    }


    public void aggiornaPreferenze() {
        mTouchHandler.updatePreferences();
    }

    public void setupOnScroll() {
        eseguiFunzioneJavaScriptSeDefinita("setupOnScroll", "setupOnScroll()");
    }

    @Override
    public void eseguiFunzioneJavaScriptSeDefinita(String function, CharSequence codice) {
        executeJavascript(String.format("if (typeof(%s) === 'function') {%s;}", function, codice));
    }

    @Override
    protected void onSizeChanged(int w, int h, int ow, int oh) {
        super.onSizeChanged(w, h, ow, oh);
        setupOnScroll();
    }

    @Override
    public boolean performLongClick() {
        if (mTouchHandler.isZooming() || longPressCanceled) {
            return false;
        }
        return super.performLongClick();
    }

    @Override
    public boolean onTouchEvent(MotionEvent ev) {
        int action = ev.getAction() & MotionEvent.ACTION_MASK;
        if (action == MotionEvent.ACTION_DOWN && ev.getPointerCount() == 1) {
            longPressCanceled = false;
            if (mFragment != null) {
                mParentActivity.onFragmentTouch(mFragment);
            }
        }

        if (action == MotionEvent.ACTION_DOWN || action == MotionEvent.ACTION_MOVE) {
            lastTouchX = ev.getX();
            lastTouchY = ev.getY();
        }

        if (mTouchHandler.onTouchEvent(ev))
            return true;

        return super.onTouchEvent(ev);
    }

    public void cancelTouch() {
        //this.cancelLongPress(); era necessario in Android 4, probabilmente non più
    }

    @Override
    protected void onCreateContextMenu(ContextMenu menu) {
// il context menu è nero su biano anche in Modo notte (invece del contrario)
        // sembra che non si sia un modo per invertire i colori
        super.onCreateContextMenu(menu);

        if (mFragment == null)
            return;

        if (mSkipNextContextMenu) {
            mSkipNextContextMenu = false;
            return;
        }

        HitTestResult result = getHitTestResult();
        if (result.getType() == HitTestResult.SRC_ANCHOR_TYPE) {
            final String url = result.getExtra();
            if (url==null) return;
            if (url.startsWith("lpcomando:")) {
                mLaParolaBrowser.vaiAdUrl(url);
                return;
            }

            MenuItem.OnMenuItemClickListener handler = item -> {
                if (item.getItemId() == ID_APRI) {
                    mLaParolaBrowser.vaiAdUrl(url);
                } else if (item.getItemId() == ID_APRI_NUOVO_PANNELLO) {
                    mParentActivity.openInNewPanel(url);
                } else if (item.getItemId() == ID_APRI_PANNELLO_ESISTENTE) {
                    mParentActivity.selectPanelForOpening(url);
                } else if (item.getItemId() == ID_APRI_POPUP) {
                    mParentActivity.openInPopupWindow(url);
                }
                return true;
            };

            menu.add(0, ID_APRI, 0, R.string.open_here).setOnMenuItemClickListener(handler);
            if (mParentActivity.getPanesNumber() < LaParolaActivity.MAX_PANELS) {
                menu.add(0, ID_APRI_NUOVO_PANNELLO, 0, R.string.open_new_panel).setOnMenuItemClickListener(handler);
            }
            if (mParentActivity.getPanesNumber() > 1) {
                menu.add(0, ID_APRI_PANNELLO_ESISTENTE, 0, R.string.open_in_panel).setOnMenuItemClickListener(handler);
            }
            menu.add(0, ID_APRI_POPUP, 0, R.string.open_popup).setOnMenuItemClickListener(handler);
        } else {
            // visualizza il menu se sul vuoto, se sul testo selezionalo.
            // Devo fare così perché da KitKat in poi non c'è la possibilità
            // di avviare la selezione del testo.
            String sx = String.valueOf(lastTouchX / getWidth()).replace(',', '.');   // coordinate normalizzate
            String sy = String.valueOf(lastTouchY / getHeight()).replace(',', '.');
            executeJavascript(String.format(Locale.getDefault(), "checkLongTouchOnBackground(%s, %s)", sx, sy));
        }
    }

    public void selectAndCopyText() {
        mSkipNextContextMenu = true;
        BibleView.super.performLongClick();
    }

    public void setParents(LaParolaActivity parentActivity, LaParolaFragment fragment) {
        mParentActivity = parentActivity;
        mFragment = fragment;
    }

    @Override
    protected void onRestoreInstanceState(Parcelable state) {
        super.onRestoreInstanceState(state);
        mLaParolaBrowser.setLaParolaBrowserClient(this);
    }

    @Override
    protected Parcelable onSaveInstanceState() {
        mLaParolaBrowser.setLaParolaBrowserClient(null);
        return super.onSaveInstanceState();
    }

    public void setOnBibleViewListener(OnBibleViewListener onBibleViewListener) {
        this.mOnBibleViewListener = onBibleViewListener;
    }

    public LaParolaBrowser getBrowser() {
        return mLaParolaBrowser;
    }

    @Override
    public void visualizzaTesto(final CharSequence testo, final LaParolaUrl url) {
        post(() -> {
            String urlText = url.getUrl();
            urlText = "http://localhost/?laparolaurl=" + URLEncoder.encode(urlText);
            if (testo != null) {
                loadDataWithBaseURL(urlText, testo.toString(), "text/html", "UTF-8", urlText);
            } else {
                loadDataWithBaseURL(urlText, "", "text/html", "UTF-8", urlText);
            }
            mGoToAnchor = url.ancoraggio;

            if (mFragment == null) {
                // E questo cos'è???
                setBackgroundColor(0);
                //Timber.tag("laparola").d("*");
            }
        });
    }

    @Override
    public void vaiAdAncoraggio(final String ancoraggio) {
        post(() -> {
            mIgnoreNextScroll = true;
            // sembra che il precedente non funzioni su alcune versioni della WebView Chrome
            executeJavascript("{" +
                    //"location.hash = '#" + ancoraggio + "';\n" +
                    "let element = document.querySelector('a[name=\"" + ancoraggio + "\"]');\n" +
                    "if (element != null) {\n" +
                    "    let scrollTop = window.scrollY;\n" +
                    "    scrollTop += element.getBoundingClientRect().top;\n" +
                    "    window.scrollTo({top: scrollTop});\n" +
                    "}\n" +
                    "}");
            finishedLoading();
        });
    }

    @Override
    public void onRichiestaIniziata(final LaParolaUrl url) {
        post(() -> {
            //Timber.tag("laparola").d("Iniziato caricamento di %s", url.getUrl());
            mLoading = true;
            if (mOnBibleViewListener != null) {
                mOnBibleViewListener.onStartLoading(BibleView.this);
            }
        });
    }

    @Override
    public void onVersioneCambiata() {
        post(() -> {
            if (mOnBibleViewListener != null) {
                mOnBibleViewListener.onVersionChanged(BibleView.this);
            }
        });
    }

    @Override
    public void visualizzaSito() {
        apriLink(mContext.getString(R.string.laparola_url));
    }

    @Override
    public void apriLink(String link) {
        LaParolaActivity.apriLink(mContext, link);
    }

    public void setTextZoom(int zoom) {
        setTextZoom(zoom, true);
    }

    public void setTextZoom(int zoom, boolean showToast) {
        if (zoom < 10)
            zoom = 10;
        if (zoom > 500)
            zoom = 500;

        if (zoom != mTextZoom) {
            mTextZoom = zoom;
            executeJavascript("if (document.body != null) {document.body.style.fontSize = '" + mTextZoom + "%';}");

            if (showToast) {
                mZoomToast.setText(mContext.getString(R.string.zoom_percent, mTextZoom));
                mZoomToast.show();
            }

            if (mOnBibleViewListener != null)
                mOnBibleViewListener.onZoomChanged(this, mTextZoom);
        }
    }

    public void executeJavascript(String command) {
        loadUrl("javascript:(function() {" + command + "})()");
    }

    public int getTextZoom() {
        return mTextZoom;
    }

    @Override
    public CharSequence getAggiuntaHeader(LaParolaUrl url) {
        StringBuilder sb = new StringBuilder();

        sb.append("<style type='text/css'><!--\n");
        appendCSS(sb);
        sb.append("--></style>");

        String assets_path = LaParolaBrowser.getPercorsoAsset();
        sb.append("<link rel='stylesheet' type='text/css' href='");
        sb.append(assets_path);
        sb.append("laparola.css'>\n");
        sb.append("<script type='text/javascript' src='");
        sb.append(assets_path);
        sb.append("utils.js'></script>\n");
        if (!LaParolaPreferences.referenceSuperscript) {
            sb.append("<style type='text/css'><!--\n");
            sb.append("  .versetto {\n");
            sb.append("        vertical-align:  baseline;").append(";\n");
            sb.append("        font-size:       100%%;").append(";\n");
            sb.append("  }\n");
            sb.append("--></style>");
        }
        /*

         */
        return sb;
    }

    private StringBuilder appendCSS(StringBuilder sb) {
        if (sb == null)
            sb = new StringBuilder();

        String background, bodycolor, acolor, versettocolor;

        if (!mNightMode) {
            if (mFragment != null) {
                background = "white";
            } else {
                background = "transparent";
            }
            bodycolor = "black";
            acolor = "blue";
            versettocolor = "black";
        } else {
            background = "black";
            bodycolor = "white";
            acolor = "yellow";
            versettocolor = "white";
        }

        sb.append("  body {\n");
        sb.append("    font-size:  ").append(mTextZoom).append("%;\n");
        sb.append("    background: ").append(background).append(";\n");
        sb.append("    color:      ").append(bodycolor).append(";\n");
        sb.append("  }\n");
        sb.append("  a {\n");
        sb.append("    color:      ").append(acolor).append(";\n");
        sb.append("  }\n");
        sb.append("  .versetto {\n");
        sb.append("    color:      ").append(versettocolor).append(";\n");
        sb.append("  }\n");
        sb.append("  .titolo_nota {\n");
        sb.append("    color:      ").append(versettocolor).append(";\n");
        sb.append("  }\n");

        return sb;
    }

    public void goToNextUrl() {
        if (mLaParolaBrowser.getUrlCorrente() != null) {
            String url = mLaParolaBrowser.getUrlCorrente().getUrlSuccessivo();
            if (url != null)
                mLaParolaBrowser.vaiAdUrl(url);
        }
    }

    public void goToPreviousUrl() {
        if (mLaParolaBrowser.getUrlCorrente() != null) {
            String url = mLaParolaBrowser.getUrlCorrente().getUrlPrecedente();
            if (url != null)
                mLaParolaBrowser.vaiAdUrl(url);
        }
    }

    public void setNightMode(boolean value) {
        mNightMode = value;

        StringBuilder css = appendCSS(null);
        applyCSS(css);
        mLaParolaBrowser.onCaricamentoFinito(value);
    }

    public void applyCSS(CharSequence css) {
        StringBuilder sb = new StringBuilder();
        sb.append("var styleElement = document.createElement(\"style\");");
        sb.append("styleElement.type = \"text/css\";");
        sb.append("styleElement.appendChild(document.createTextNode('");
        sb.append(css);
        sb.append("'));");
        sb.append("var h = document.getElementsByTagName(\"head\")[0];");
        sb.append("if (h) h.appendChild(styleElement);");

        int i = sb.indexOf("\n");
        while (i != -1) {
            sb.replace(i, i + 1, " ");
            i = sb.indexOf("\n");
        }

        executeJavascript(sb.toString());
    }

    public boolean getNightMode() {
        return mNightMode;
    }

    public void onScrolledToAnchor(String s) {
        if (mIgnoreNextScroll) {
            mIgnoreNextScroll = false;
        } else if (!mLoading) {
            getBrowser().getUrlCorrente().ancoraggio = s;
            if (mOnBibleViewListener != null)
                mOnBibleViewListener.onAnchorChanged(this, s);
        }
    }

    private void finishedLoading() {
        if (mLoading) {
            mLoading = false;

            StringBuilder css = appendCSS(null);
            applyCSS(css);

            LaParolaUrl urlCorrente = mLaParolaBrowser.getUrlCorrente();
            if (urlCorrente != null) {
                VersioneInformazioni informazioniTesto = LaParolaBrowser.getInformazioniTesto(urlCorrente.versione);
                boolean greek = (informazioniTesto != null) && (
                        (informazioniTesto.getLingua().contains("el") && !informazioniTesto.getLingua().contains("transliterated")) ||
                                (informazioniTesto.getLingua().isEmpty() && informazioniTesto.getTitolo().contains("nterlin") && informazioniTesto.getTitolo().contains("reco"))
                );

                if (!mLaParolaBrowser.inHome() && greek) {
                    if (!InstallGentiumHelper.isInstalled()) {
                        Toast.makeText(mContext, R.string.greek_not_installed, Toast.LENGTH_LONG).show();
                        executeJavascript(
                                "var bs = document.getElementById('bodystart');" +
                                        "if (bs != null) {" +
                                        "bs.innerHTML = '<p align=\\'center\\'><a href=\\'lpcomando:installa_font_greco\\'>" + mContext.getString(R.string.greek_install) + "</a></p>';" +
                                        "}");
                    }

                    applyCSS(
                            "@font-face {" +
                                    "	font-family: \"FontGreco\";" +
                                    "	src: url(\"file://" + InstallGentiumHelper.getFontPath() + "\");" +
                                    "   font-style: normal;" +
                                    "}" +
                                    "@font-face {" +
                                    "	font-family: \"FontGreco\";" +
                                    "	src: url(\"file://" + InstallGentiumHelper.getFontPathItalics() + "\");" +
                                    "   font-style: italic;" +
                                    "}" +
                                    "body {font-family: \"FontGreco\", sans-serif;}"
                    );
                }
            }

            if (mFragment == null)
                setVisibility(View.VISIBLE);   // per evitare glitch nella finestra popup

            if (mOnBibleViewListener != null) {
                mOnBibleViewListener.onFinishedLoading(BibleView.this);
            }
        }
    }

    @Override
    public void eseguiJavaScript(CharSequence codice) {
        executeJavascript(codice.toString());
    }

    @Override
    public void mostraRicerca() {
        mParentActivity.showSearch();
    }
}

