package net.laparola.ui.android;

import android.Manifest;
import android.content.ActivityNotFoundException;
import android.content.Context;
import android.content.DialogInterface;
import android.content.DialogInterface.OnDismissListener;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.content.res.Configuration;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.support.v4.app.ActivityCompat;
import android.support.v4.app.FragmentTransaction;
import android.support.v4.content.ContextCompat;
import android.support.v4.view.GravityCompat;
import android.support.v4.widget.DrawerLayout;
import android.util.Log;
import android.view.ContextMenu;
import android.view.Gravity;
import android.view.KeyEvent;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.ExpandableListView;
import android.widget.FrameLayout;
import android.widget.Toast;

import net.laparola.BuildConfig;
import net.laparola.R;
import net.laparola.core.Testi;
import net.laparola.core.Testi.TestoTipi;
import net.laparola.core.VersioneInformazioni;
import net.laparola.ui.LaParolaBrowser;
import net.laparola.ui.LaParolaBrowser.LaParolaBrowserStaticClient;
import net.laparola.ui.LaParolaEvidenziatore;
import net.laparola.ui.LaParolaSegnalibri.Segnalibro;
import net.laparola.ui.LaParolaUrl;
import net.laparola.ui.android.LaParolaFragment.MyRunnable;
import net.laparola.ui.android.actionbar.LibraryActionItemManager;
import net.laparola.ui.android.actionbar.ReferenceActionItemManager;
import net.laparola.ui.android.actionbar.SearchActionItemManager;
import net.laparola.ui.android.actionbar.TTSActionItemManager;
import net.laparola.ui.android.dialogs.AccessibilityDialog;
import net.laparola.ui.android.dialogs.HoloDialog;
import net.laparola.ui.android.dialogs.MessageDialog;
import net.laparola.ui.android.dialogs.PanelsDialog;
import net.laparola.ui.android.dialogs.PopupDialog;
import net.laparola.ui.android.dialogs.StarredDialog;
import net.laparola.ui.android.dialogs.WebViewDialog;
import net.laparola.ui.android.library.LibraryActivity;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.InputStream;
import java.util.ArrayList;
import java.util.EnumSet;
import java.util.List;

import androidx.appcompat.app.ActionBarDrawerToggle;
import androidx.drawerlayout.widget.DrawerLayout;
import androidx.fragment.app.FragmentActivity;

public class LaParolaActivity extends FragmentActivity implements LaParolaBrowserStaticClient, ExpandableListView.OnChildClickListener, ExpandableListView.OnGroupClickListener {
    public static final int MAX_PANELS = 4;
    public static final int MY_PERMISSIONS_REQUEST_WRITE_EXTERNAL_STORAGE = 1;

    public static boolean apriLink(Context context, String link) {
        Uri laparolauri = Uri.parse(link);
        Intent intent = new Intent(Intent.ACTION_VIEW, laparolauri);
        intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
        try {
            context.startActivity(intent);
            return true;
        } catch (ActivityNotFoundException e) {
            return false;
        }
    }

    private DrawerLayout mDrawerLayout;
    private ActionBarDrawerToggle mDrawerToggle;
    private ExpandableListView mDrawerList;
    private DrawerAdapter mDrawerAdapter;

    private LaParolaFragment activeFragment;

    private SearchActionItemManager searchActionItemManager;
    private ReferenceActionItemManager referenceActionItemManager;
    private LibraryActionItemManager libraryActionItemManager;
    private TTSActionItemManager ttsActionItemManager;

    private MenuItem forwardActionItem;
    private MenuItem nightModeActionItem;
    private MenuItem starActionItem;
    private MenuItem searchActionItem;
    private MenuItem referenceActionItem;
    private MenuItem libraryActionItem;
    private MenuItem zoomInActionItem;
    private MenuItem zoomOutActionItem;
    private MenuItem highlighterActionItem;

    //private boolean firstReferenceClicked = true;

    private boolean mGoingBack;
    private long lastBackPressedTime = 0;
    private boolean mFinishedLoading = false;
    private boolean firstLoading = true;

    protected boolean isPaused;
    private FourPanesLayout fourPanesLayout;

    /* package */ List<LaParolaFragment> fragments;

    private LaParolaActivityInitUtility initUtility;

    private int mLastOrientation;

    private LaParolaUrl mSelectingPanelForOpeningUrl;
    private LaParolaFragment mSwitchingPanels;
    /* package */ ActionMode actionMode;

    private View mPanelContextMenuView;

    private boolean checkPermissions () {
        if (ContextCompat.checkSelfPermission(this, Manifest.permission.WAKE_LOCK) != PackageManager.PERMISSION_GRANTED) {
            Log.d("LaParola","Non ho l'autorizzazione per WAKE_LOCK");
        }
        if (ContextCompat.checkSelfPermission(this, Manifest.permission.INTERNET) != PackageManager.PERMISSION_GRANTED) {
            Log.d("LaParola","Non ho l'autorizzazione per INTERNET");
        }
        if (Build.VERSION.SDK_INT < 33 && ContextCompat.checkSelfPermission(this, Manifest.permission.WRITE_EXTERNAL_STORAGE) != PackageManager.PERMISSION_GRANTED) {
            Log.d("LaParola","Non ho l'autorizzazione per WRITE_EXTERNAL_STORAGE");

            // Permission is not granted
            // Should we show an explanation?
            if (ActivityCompat.shouldShowRequestPermissionRationale(this,
                    Manifest.permission.WRITE_EXTERNAL_STORAGE)) {
                // Show an explanation to the user *asynchronously* -- don't block
                // this thread waiting for the user's response! After the user
                // sees the explanation, try again to request the permission.

                if (!this.isFinishing()) {
                    this.runOnUiThread(() -> {
                        HoloDialog d = new MessageDialog(this, R.string.error, R.string.permission_write_storage);
                        d.setOnDismissListener((dialog) -> {
                            ActivityCompat.requestPermissions(this,
                                    new String[]{Manifest.permission.WRITE_EXTERNAL_STORAGE},
                                    LaParolaActivity.MY_PERMISSIONS_REQUEST_WRITE_EXTERNAL_STORAGE);
                        });
                        d.show();
                    });
                }
            } else {
                // No explanation needed; request the permission
                ActivityCompat.requestPermissions(this,
                        new String[]{Manifest.permission.WRITE_EXTERNAL_STORAGE},
                        LaParolaActivity.MY_PERMISSIONS_REQUEST_WRITE_EXTERNAL_STORAGE);
            }

            return false;
        } else {
            //Log.d("LaParola","Autorizzazione per WRITE_EXTERNAL_STORAGE accordata");
        }
        return true;
    }

    @Override
    public void onRequestPermissionsResult(int requestCode, String permissions[], int[] grantResults) {
        switch (requestCode) {
            case MY_PERMISSIONS_REQUEST_WRITE_EXTERNAL_STORAGE: {
                // If request is cancelled, the result arrays are empty.
                if (grantResults.length > 0 && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
                    // permission was granted, yay! Do the
                    // contacts-related task you need to do.
                    onFinishedLoadingActivity();
                } else {
                    if (!this.isFinishing()) {
                        this.runOnUiThread(() -> {
                            HoloDialog d = new MessageDialog(this, R.string.error, R.string.permission_write_storage_denied);
                            d.setOnDismissListener((dialog) -> this.finish());
                            d.show();
                        });
                    }
                }
                return;
            }
        }
    }

    @Override
    public InputStream apriFile(String filename) {
        try {
            if ((new File(filename)).exists())
                return new FileInputStream(filename);
            return getAssets().open(filename);
        } catch (Exception e) {
            if (!(e instanceof FileNotFoundException)) {
                e.printStackTrace();
            }
        }

        return null;
    }

    public boolean collapseActionViewsExcept(MenuItem exclude) {
        searchActionItemManager.collapse(exclude);
        referenceActionItemManager.collapse(exclude);
        libraryActionItemManager.collapse(exclude);
        ttsActionItemManager.collapse(exclude);

        if (initUtility == null) {
            return true;
        }
        return (!initUtility.isWorking());
    }

    @Override
    public boolean dispatchKeyEvent(KeyEvent event) {
        int keyCode = event.getKeyCode();

        if (event.getAction() == KeyEvent.ACTION_UP) {
            if (keyCode == KeyEvent.KEYCODE_BACK && actionMode == null) {
                long milliTime = System.nanoTime() / 1000000;
                if (!mGoingBack && milliTime > lastBackPressedTime + 500) {
                    // ignora due "indietro" troppo vicini
                    lastBackPressedTime = milliTime;
                    if (activeFragment != null && activeFragment.isCreated() && activeFragment.precedenteEsiste()) {
                        mGoingBack = true;
                        activeFragment.vaiAPrecendente();
                        forwardActionItem.setEnabled(true);
                        return true;
                    }
                    // lo lascia gestire al S.O.
                }
            } else if (keyCode == KeyEvent.KEYCODE_SEARCH) {
                if (searchActionItemManager.isExpanded()) {
                    searchActionItemManager.expandActionView();
                } else {
                    searchActionItemManager.search();
                }
                return true;
            } else if (LaParolaPreferences.useVolumeKeys && keyCode == KeyEvent.KEYCODE_VOLUME_UP && !ttsActionItemManager.isExpanded()) {
                if (activeFragment != null && !activeFragment.pageUp(false))
                    activeFragment.goToPreviousUrl();
                return true;
            } else if (LaParolaPreferences.useVolumeKeys && keyCode == KeyEvent.KEYCODE_VOLUME_DOWN && !ttsActionItemManager.isExpanded()) {
                if (activeFragment != null && !activeFragment.pageDown(false))
                    activeFragment.goToNextUrl();
                return true;
            }
        } else if (LaParolaPreferences.useVolumeKeys &&
                (keyCode == KeyEvent.KEYCODE_VOLUME_DOWN ||
                        keyCode == KeyEvent.KEYCODE_VOLUME_UP) && !ttsActionItemManager.isExpanded()) {
            return true;
        }
        return super.dispatchKeyEvent(event);
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        requestWindowFeature(Window.FEATURE_INDETERMINATE_PROGRESS);
        super.onCreate(savedInstanceState);

        setContentView(R.layout.main_activity);
        fourPanesLayout = (FourPanesLayout) findViewById(R.id.four_panes_layout);

        fragments = new ArrayList<LaParolaFragment>();

        LaParolaPreferences.load(this);

        setupDrawer();

        /*
         * TODO : usare risorse mLaParolaBrowser.Stringhe.Errore_Nessuna_versione = getContext().getString(R.string.no_version_present);
		 * mLaParolaBrowser.Stringhe.Errore_Non_presente = getContext().getString(R.string.not_present);
		 */
    }

    private void setupDrawer() {
        mDrawerLayout = (DrawerLayout) findViewById(R.id.drawer_layout);
        mDrawerToggle = new ActionBarSherlockDrawerToggle(
                this,                  /* host Activity */
                mDrawerLayout,         /* DrawerLayout object */
                R.drawable.ic_drawer,  /* nav drawer icon to replace 'Up' caret */
                R.string.drawer_open,  /* "open drawer" description */
                R.string.drawer_close  /* "close drawer" description */
        ) {

            /** Called when a drawer has settled in a completely closed state. */
            @Override
            public void onDrawerClosed(View view) {
                setActivityTitle();
            }

            /** Called when a drawer has settled in a completely open state. */
            @Override
            public void onDrawerOpened(View drawerView) {
                setActivityTitle();
            }
        };

        // Set the drawer toggle as the DrawerListener
        mDrawerLayout.setDrawerListener(mDrawerToggle);
        getSupportActionBar().setDisplayHomeAsUpEnabled(true);
        getSupportActionBar().setHomeButtonEnabled(true);

        mDrawerList = mDrawerLayout.findViewById(R.id.left_drawer);
        mDrawerAdapter = new DrawerAdapter(this);
        mDrawerList.setAdapter(mDrawerAdapter);

        mDrawerList.setOnChildClickListener(this);
        mDrawerList.setOnGroupClickListener(this);
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getSupportMenuInflater().inflate(R.menu.action_bar, menu);

        searchActionItem = menu.findItem(R.id.menu_item_seach);
        referenceActionItem = menu.findItem(R.id.menu_item_reference);
        libraryActionItem = menu.findItem(R.id.menu_item_library);
        MenuItem ttsActionItem = menu.findItem(R.id.menu_item_tts);

        searchActionItem.setShowAsAction(MenuItem.SHOW_AS_ACTION_ALWAYS | MenuItem.SHOW_AS_ACTION_COLLAPSE_ACTION_VIEW);
        referenceActionItem.setShowAsAction(MenuItem.SHOW_AS_ACTION_ALWAYS | MenuItem.SHOW_AS_ACTION_COLLAPSE_ACTION_VIEW);
        libraryActionItem.setShowAsAction(MenuItem.SHOW_AS_ACTION_ALWAYS | MenuItem.SHOW_AS_ACTION_COLLAPSE_ACTION_VIEW);
        ttsActionItem.setShowAsAction(MenuItem.SHOW_AS_ACTION_NEVER | MenuItem.SHOW_AS_ACTION_COLLAPSE_ACTION_VIEW);

        searchActionItem.setActionView(R.layout.search_action_view);
        referenceActionItem.setActionView(R.layout.reference_action_view);
        libraryActionItem.setActionView(R.layout.version_action_view);
        ttsActionItem.setActionView(R.layout.tts_action_view);

        searchActionItemManager = new SearchActionItemManager(this, searchActionItem);
        referenceActionItemManager = new ReferenceActionItemManager(this, referenceActionItem);
        libraryActionItemManager = new LibraryActionItemManager(this, libraryActionItem);
        ttsActionItemManager = new TTSActionItemManager(this, ttsActionItem);

        forwardActionItem = menu.findItem(R.id.menu_item_forward);
        starActionItem = menu.findItem(R.id.menu_item_star);
        nightModeActionItem = menu.findItem(R.id.menu_item_night_mode);
        highlighterActionItem = menu.findItem(R.id.menu_item_highlighter);
        forwardActionItem.setShowAsAction(MenuItem.SHOW_AS_ACTION_IF_ROOM);
        starActionItem.setShowAsAction(MenuItem.SHOW_AS_ACTION_IF_ROOM);

        zoomInActionItem = menu.findItem(R.id.menu_item_zoom_in);
        zoomOutActionItem = menu.findItem(R.id.menu_item_zoom_out);

        nightModeActionItem.setTitle(LaParolaPreferences.nightMode ? R.string.night_mode_off : R.string.night_mode_on);

        if (Build.VERSION.SDK_INT <= 10) {
            menu.findItem(R.id.menu_item_copy).setVisible(true);
            menu.findItem(R.id.menu_item_share).setVisible(true);
        }

        // if (!getPackageManager().hasSystemFeature(PackageManager.FEATURE_TOUCHSCREEN_MULTITOUCH)) {
        menu.findItem(R.id.menu_item_zoom_in).setVisible(true);
        menu.findItem(R.id.menu_item_zoom_out).setVisible(true);
        // }

        onFinishedLoadingActivity();

        return super.onCreateOptionsMenu(menu);
    }

    @Override
    public void onDestroy() {
        initUtility = null;
        LaParolaBrowser.chiudi();
        if (ttsActionItemManager != null) {
            ttsActionItemManager.destroy();
        }
        super.onDestroy();
    }

    private void updateStar() {
        LaParolaUrl currentUrl = activeFragment.getUrlCorrente();

        starActionItem.setEnabled(true);
        starActionItem.setIcon(R.drawable.ic_action_unstarred);

        if (currentUrl == null) {
            return;
        }

        /*
        if (currentUrl.schema.equals("lppreferiti") ||
                currentUrl.schema.equals("lpcronologia") ||
                currentUrl.schema.equals("lpevidenziati")) {
            starActionItem.setEnabled(false);
        }
        */

        Segnalibro s = LaParolaBrowser.cercaUrlTraPreferiti(currentUrl);
        if (s != null) {
            starActionItem.setIcon(R.drawable.ic_action_starred);
        }
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        if (initUtility != null) {
            if (initUtility.isWorking())
                return true;
        }

        // Pass the event to ActionBarDrawerToggle, if it returns
        // true, then it has handled the app icon touch event
        if (mDrawerToggle != null) {
            if (mDrawerToggle.onOptionsItemSelected(item)) {
                return true;
            }
        }
        // Handle your other action bar items...

        int itemId = item.getItemId();

        if (itemId == R.id.menu_item_forward) {
            if (activeFragment != null) {
                activeFragment.vaiASuccessivo();
            }
            return true;
        /*
        } else if (itemId == R.id.menu_item_bookmarks) {
            activeFragment.vaiAdUrl("lppreferiti:");
            return true;
        } else if (itemId == R.id.menu_item_history) {
            activeFragment.vaiAdUrl("lpcronologia:");
            return true;
        */
        /*
        } else if (itemId == R.id.menu_item_settings) {
            startActivity(new Intent(this, LaParolaPreferencesActivity.class));
            return true;
        } else if (itemId == R.id.menu_item_help) {
            activeFragment.vaiAdUrl("lpfile:Guida.html");
            return true;
        */
        } else if (itemId == R.id.menu_item_copy) {
            copy(R.string.select_text_copy);
            return true;
        } else if (itemId == R.id.menu_item_highlighter) {
            if (!startHighlighter()) {
                Toast.makeText(this, R.string.highlighter_error, Toast.LENGTH_LONG).show();
                LaParolaFragment af = getActiveFragment();
                if (af != null) {
                    af.vaiAdUrl("lpevidenziati:");
                }
            }
            return true;
        } else if (itemId == R.id.menu_item_star) {
            showStarDialog();
            return true;
        } else if (itemId == R.id.menu_item_zoom_in) {
            if (activeFragment != null) {
                activeFragment.setTextZoom(activeFragment.getTextZoom() + 10);
            }
            return true;
        } else if (itemId == R.id.menu_item_night_mode) {
            setNightMode(!LaParolaPreferences.nightMode);
            return true;
        } else if (itemId == R.id.menu_item_zoom_out) {
            if (activeFragment != null) {
                activeFragment.setTextZoom(activeFragment.getTextZoom() - 10);
            }
            return true;
        } else if (itemId == R.id.menu_item_share) {
            share();
            return true;
        } else if (itemId == R.id.menu_item_panels) {
            showPanelsManagment();
            return true;
        } else if (itemId == R.id.menu_item_library_management) {
            startActivity(new Intent(this, LibraryActivity.class));
            return true;
		/*
		} else if (itemId == android.R.id.home) {
			activeFragment.vaiAHome();
			return true;
		*/
        } else {
            // workaround per bug di actionbarsherlock

            if (itemId == R.id.menu_item_seach) {
                searchActionItemManager.expandActionView();
                return true;
            } else if (itemId == R.id.menu_item_library) {
                libraryActionItemManager.expandActionView();
                return true;
            } else if (itemId == R.id.menu_item_reference) {
                EnumSet<Testi.TestoTipi> tipoTesto= getActiveFragment().getInformazioniVersione().getTipo();
                // rmw1024 referenceActionItemManager.setDizionario(tipoTesto.contains(Testi.TestoTipi.DIZIONARIO));

                if (LaParolaPreferences.accessibilityMode)
                    showAccessibilityDialog();

                referenceActionItemManager.expandActionView();
                return true;
            } else if (itemId == R.id.menu_item_tts) {
                ttsActionItemManager.expandActionView();
                return true;
            }
        }

        return super.onOptionsItemSelected(item);
    }

    public boolean startHighlighter() {
        LaParolaHighlighterActionModeCallback acc = new LaParolaHighlighterActionModeCallback(this);
        if (acc.setup()) {
            actionMode = startActionMode(acc);
            actionMode.setTitle(R.string.highlighter_title);
            return true;
        }
        return false;
    }

    private void copy(int id) {
        Toast.makeText(this, id, Toast.LENGTH_LONG).show();
        activeFragment.selectAndCopyText();
    }

    @SuppressWarnings("deprecation")
    private void share() {
        final android.text.ClipboardManager clipboard = (android.text.ClipboardManager) getSystemService(CLIPBOARD_SERVICE);

        final String t = "--";
        clipboard.setText(t);

        copy(R.string.select_text_share);

        Runnable r = new Runnable() {
            @Override
            public void run() {
                CharSequence text = t;
                while (text.equals(t)) {
                    text = clipboard.getText();
                    try {
                        Thread.sleep(1000);
                        if (isPaused) {
                            return;
                        }
                    } catch (InterruptedException e) {
                        //
                    }
                }

                if (text != null) {
                    Intent intent = new Intent(Intent.ACTION_SEND);
                    intent.setType("text/plain");
                    intent.putExtra(Intent.EXTRA_SUBJECT, R.string.share_subject);
                    intent.putExtra(Intent.EXTRA_TEXT, text);
                    startActivity(Intent.createChooser(intent, getString(R.string.share_with)));
                }
            }
        };

        new Thread(r).start();
    }

    private void showStarDialog() {
        if (isFinishing())
            return;

        if (activeFragment == null)
            return;
        LaParolaUrl urlCorrente = activeFragment.getUrlCorrente();
        if (urlCorrente == null)
            return;

        Segnalibro s = LaParolaBrowser.cercaUrlTraPreferiti(urlCorrente);

        StarredDialog bookmark = new StarredDialog(this);
        bookmark.show();
        bookmark.setDescription(s == null ? urlCorrente.getDescrizione() : s.nome);
        bookmark.url = urlCorrente;
        bookmark.setOnDismissListener(new OnDismissListener() {
            @Override
            public void onDismiss(DialogInterface dialog) {
                updateStar();
            }
        });
    }

    private void setNightMode(boolean b) {
        LaParolaPreferences.nightMode = b;

        if (nightModeActionItem != null)
            nightModeActionItem.setTitle(b ? R.string.night_mode_off : R.string.night_mode_on);

        for (LaParolaFragment f : fragments)
            f.setNightMode(b);
    }

    @Override
    protected void onPause() {
        super.onPause();

        isPaused = true;

        LaParolaPreferences.save(this);
        if (initUtility != null && !initUtility.isWorking()) {
            LaParolaEvidenziatore.salvaVersettiEvidenziatiSuFile();
        }

        LaParolaBrowser.setLaParolaBrowserStaticClient(null);
    }

    @Override
    protected void onResume() {
        super.onResume();

        isPaused = false;

        LaParolaBrowser.setLaParolaBrowserStaticClient(this);

        if (hasFinishedLoading())
            onTestiCambiati();   // non riceve le informazioni quando l'attività è in pausa

        applyPreferences();
    }

    @Override
    public void onTestiCambiati() {
        for (LaParolaFragment f : fragments)
            f.onTestiCambiati();

        libraryActionItemManager.onTestiCambiati();

        onVersionChanged();
    }

    private void applyPreferences() {
        if (!hasFinishedLoading()) {
            fourPanesLayout.postDelayed(new Runnable() {
                @Override
                public void run() {
                    applyPreferences();
                }
            }, 100);
            return;
        }

        if (isPaused) {
            return;
        }

        LaParolaPreferences.load(this);

        if (fragments.isEmpty()) {
            if (LaParolaPreferences.homeAtStart || LaParolaPreferences.fragmentsNumber < 1) {
                LaParolaPreferences.fragmentsNumber = 1;
            }

            addLaParolaFragment(true);
            setPanes(LaParolaPreferences.fragmentsNumber, LaParolaPreferences.fragmentsOrientation, true, null);
            setActiveFragment(fragments.get(0));
        }

        LaParolaBrowser.setMostraParagrafi(LaParolaPreferences.paragraphOrVerses);
        LaParolaBrowser.setMostraTitoli(LaParolaPreferences.showTitles);
        LaParolaBrowser.setPosizioneRiferimento(LaParolaPreferences.referencePlacement);
        LaParolaBrowser.setTipoRiferimento(LaParolaPreferences.referenceType);
        LaParolaBrowser.setRiferimentoInApice(LaParolaPreferences.referenceSuperscript);

        setNightMode(LaParolaPreferences.nightMode);
        mDrawerLayout.setKeepScreenOn(LaParolaPreferences.keepScreenOn);
        zoomInActionItem.setVisible(LaParolaPreferences.menuZoom);
        zoomOutActionItem.setVisible(LaParolaPreferences.menuZoom);

        for (LaParolaFragment f : fragments) {
            f.aggiornaPreferenze();
            f.aggiornaPagina();
        }
    }

    private void aggiungiOpzioneHome(StringBuilder sb, String id, String key, boolean def) {
        String display;
        if (LaParolaPreferences.getHomeOption(key, def)) {
            if (id.startsWith("span_")) {
                display = "inline";
            } else {
                display = "block";
            }
            // display = "initial";   // CSS3, non so se funziona con tutte le versioni android
        } else {
            display = "none";
        }
        aggiungiMostraId(sb, id, display);
    }

    private void aggiungiMostraId(StringBuilder sb, String id, String display) {
        sb.append("e = document.getElementById('");
        sb.append(id);
        sb.append("'); if (e != null) {e.style.display = '");
        sb.append(display);
        sb.append("';}");
    }

    public String javaScriptPerOpzioniHomePage() {
        StringBuilder sb = new StringBuilder();
        aggiungiMostraId(sb, "div_loading", "none");
        aggiungiMostraId(sb, "div_debug", BuildConfig.DEBUG ? "block" : "none");
        aggiungiOpzioneHome(sb, "div_inizia_subito", "home_show_start", true);
        aggiungiOpzioneHome(sb, "div_versetto_casuale", "home_show_random", true);
        aggiungiOpzioneHome(sb, "span_versetti_casuali_at_nt", "home_show_random_at_nt", true);
        aggiungiOpzioneHome(sb, "div_libreria", "home_show_library", true);
        aggiungiOpzioneHome(sb, "div_parola_del_giorno", "home_show_pdg", true);
        aggiungiOpzioneHome(sb, "div_liturgia_del_giorno", "home_show_liturgy", false);
        aggiungiOpzioneHome(sb, "div_segnalibri", "home_show_bookmarks", true);
        aggiungiOpzioneHome(sb, "div_aiuto", "home_show_about", true);
        return sb.toString();
    }

    @Override
    public void mostraAiutoRicerca() {
        WebViewDialog webViewDialog = new WebViewDialog(this, "file:///android_asset/Guida_ricerca.html");
        webViewDialog.setTitle(R.string.help);
        webViewDialog.show();
    }

    @Override
    public void mostraPulisciCronologia() {
        MessageDialog m = new MessageDialog(this, R.string.history, R.string.clear_history);
        m.setYesNo(R.string.clear, android.R.string.cancel, new Runnable() {
            @Override
            public void run() {
                LaParolaBrowser.pulisciCronologia();
                activeFragment.aggiornaPagina();   // la pagina della cronologia
            }
        }, null);
        m.show();
    }

    public boolean hasFinishedLoading() {
        return mFinishedLoading;
    }

    public void onLoadingChanged() {
        boolean show = false;
        for (int i = 0; i < getPanesNumber(); i++) {
            LaParolaFragment f = fragments.get(i);
            if (f.isLoading()) {
                show = true;
                break;
            }
        }
        setSupportProgressBarIndeterminateVisibility(show);

        if (!show) {
            setActivityTitle();
            if (!firstLoading)
                mDrawerLayout.closeDrawer(GravityCompat.START);
            firstLoading = false;
        }
    }

    public void updateActionBar() {
        referenceActionItemManager.updateBooks();

        mGoingBack = false;
        forwardActionItem.setEnabled(activeFragment.successivoEsiste());

        LaParolaUrl currentUrl = activeFragment.getUrlCorrente();

        if (currentUrl != null && currentUrl.gestito && currentUrl.schema.equals("laparola")) {
            String versione = activeFragment.getVersione();
            libraryActionItemManager.setVersion(versione);

            int[] lcv = currentUrl.getLCV();
            if (lcv != null) {
                int b = lcv[0];
                int c = lcv[1];
                int v = lcv[2];

                referenceActionItemManager.select(b, c, v);
				/*
				if (firstReferenceClicked) {
					referenceActionItemManager.expand();
					firstReferenceClicked = false;
				}
				*/
            } else {
                referenceActionItemManager.select(0, 0, 0);
            }

            if (currentUrl.ricerca != null) {
                searchActionItemManager.select(currentUrl.ricerca);
            }

            if (currentUrl.brani != null) {
                referenceActionItemManager.select(currentUrl.brani);
            }
        } else {
            referenceActionItemManager.select(0, 0, 0);
        }

        updateStar();

        if (currentUrl != null && currentUrl.gestito) {
            libraryActionItem.setVisible(true);
            referenceActionItem.setVisible(true);
            starActionItem.setVisible(true);
            highlighterActionItem.setVisible(true);
            if (currentUrl.schema.equals("lpsegnalibri")) {
                referenceActionItem.setVisible(false);
                starActionItem.setVisible(false);
                highlighterActionItem.setVisible(false);
            }
            if (currentUrl.schema.equals("lpsegnalibro")) {
                referenceActionItem.setVisible(false);
                starActionItem.setVisible(false);
                highlighterActionItem.setVisible(false);
            }
            if (currentUrl.schema.equals("lppreferiti")) {
                referenceActionItem.setVisible(false);
                starActionItem.setVisible(false);
                highlighterActionItem.setVisible(false);
            }
            if (currentUrl.schema.equals("lpevidenziati")) {
                referenceActionItem.setVisible(false);
                starActionItem.setVisible(false);
            }
            if (currentUrl.schema.equals("lpcronologia")) {
                libraryActionItem.setVisible(false);
                referenceActionItem.setVisible(false);
                starActionItem.setVisible(false);
                highlighterActionItem.setVisible(false);
            }
            if (currentUrl.schema.equals("lpfile")) {
                libraryActionItem.setVisible(false);
                referenceActionItem.setVisible(false);
                starActionItem.setVisible(false);
                highlighterActionItem.setVisible(false);
            }
        }
	
		/*
		if (initUtility.isWorking() || activeFragment.inHome()) {
			getSupportActionBar().setDisplayHomeAsUpEnabled(false);
		} else {
			getSupportActionBar().setDisplayHomeAsUpEnabled(true);
		}
		*/
    }

    @Override
    protected void onPostCreate(Bundle savedInstanceState) {
        super.onPostCreate(savedInstanceState);
        // Sync the toggle state after onRestoreInstanceState has occurred.
        if (mDrawerToggle != null)
            mDrawerToggle.syncState();
    }

    @Override
    public void onConfigurationChanged(Configuration newConfig) {
        super.onConfigurationChanged(newConfig);
        if (mDrawerToggle != null)
            mDrawerToggle.onConfigurationChanged(newConfig);
    }

    public void onVersionChanged() {
        if (referenceActionItemManager != null) {
            referenceActionItemManager.onVersionChanged();
            libraryActionItemManager.onVersionChanged();
        }
    }

    private void onFinishedLoadingActivity() {
        boolean ok = checkPermissions();
        if (!ok) {
            return;   // verrà richiamato da checkPermissions
        }

        if (hasFinishedLoading())
            return;

        if (initUtility == null) {
            initUtility = new LaParolaActivityInitUtility(this);
            fourPanesLayout.post(new Runnable() {
                @Override
                public void run() {
                    // non so esattamente quando possa accadere, ma dai log accade
                    // immagino quando si chiude l'applicazione prima che sia terminato il
                    // caricamento
                    if (initUtility == null) {
                        initUtility = new LaParolaActivityInitUtility(LaParolaActivity.this);
                    }
                    initUtility.init();
                }
            });
        }

        if (initUtility.isWorking()) {
            fourPanesLayout.postDelayed(new Runnable() {
                @Override
                public void run() {
                    onFinishedLoadingActivity();
                }
            }, 100);
            return;
        }

        if (LaParolaPreferences.homeAtStart) {
            // TODO : se è la prima volta, mostrare help
            mDrawerLayout.openDrawer(GravityCompat.START);
        }

        setFinishedLoading(true);
    }

    public void setActiveFragment(LaParolaFragment value) {
        if (activeFragment == value || value == null || !fragments.contains(value))
            return;

        activeFragment = value;
        fourPanesLayout.setSelectedPane(fragments.indexOf(value));

        setActivityTitle();

        if (activeFragment.isCreated()) {
            updateActionBar();

            LaParolaUrl url = activeFragment.getUrlCorrente();
            if (!getUltimaBibbiaSalvata().equals(url.versione)) {
                VersioneInformazioni informazioniTesto = LaParolaBrowser.getInformazioniTesto(url.versione);
                if (informazioniTesto != null && informazioniTesto.getTipo().contains(TestoTipi.BIBBIA)) {
                    setUltimaBibbiaSalvata(url.versione);
                }
            }
        }
    }

    public LaParolaFragment getActiveFragment() {
        if (activeFragment == null && initUtility != null && !initUtility.isWorking()) {
            applyPreferences();   // lo crea
        }
        return activeFragment;
    }

    public void addLaParolaFragment(boolean restoring) {
        final int position = fragments.size();

        if (position == MAX_PANELS) {
            return;
        }

        LaParolaFragment fragment = new LaParolaFragment();

        final String version = LaParolaPreferences.lastVersion[position];

        final String goToUrl;
        final int zoom;

        if (restoring) {
            fragment.setSyncColor(LaParolaPreferences.syncColor[position]);

            zoom = LaParolaPreferences.textZoom[position];
            if (position == 0) {
                if (LaParolaPreferences.homeAtStart) {
                    goToUrl = null;
                } else {
                    goToUrl = LaParolaPreferences.lastUrl[position];
                }
            } else {
                // String defUrl = getActiveFragment().inHome() ? null : getActiveFragment().getUrlCorrente().getUrl();
                //String defUrl = null; //getActiveFragment().getUrlCorrente().getUrl();
                goToUrl = LaParolaPreferences.lastUrl[position];
                //if (goToUrl == null)
                //	goToUrl = defUrl;
            }
        } else {
            // lastUrl = getActiveFragment().inHome() ? null : getActiveFragment().getUrlCorrente().getUrl();
            goToUrl = getActiveFragment().getUrlCorrente().getUrl();
            zoom = getActiveFragment().getTextZoom();
        }

        final String ultimaBibbia = LaParolaBrowser.getUltimaBibbia();

        fragment.onCreateGoToUrl = goToUrl;
        fragment.onCreateViewRunnable = new MyRunnable() {
            @Override
            public void run(LaParolaFragment self) {
                self.setTextZoom(zoom, false);
                self.setVersione(version);
                if (self.onCreateGoToUrl != null) {
                    self.vaiAdUrl(self.onCreateGoToUrl);
                } else if (position == 0) {
                    self.setVersione(ultimaBibbia);
                    self.vaiAHome();
                }
				
				/*
				if (myRunnable != null)
					myRunnable.run(self);
				}
				*/
            }
        };

        fragments.add(fragment);
        FragmentTransaction transaction = getSupportFragmentManager().beginTransaction();
        transaction.add(fourPanesLayout.getFrameId(position), fragment, String.valueOf(position));
        transaction.commit();
    }

    public void onZoomChanged(LaParolaFragment fragment, int zoom) {
        for (LaParolaFragment f : fragments) {
            if (f.isCreated()) {
                f.setTextZoom(zoom, false);
            }
        }
    }

    public int getPanesNumber() {
        return fourPanesLayout.getNumberPanes();
    }

    public int getPanesOrientation() {
        return fourPanesLayout.getOrientation();
    }

    public void setPanes(int numberPanes, int orientation, boolean restoring, int[] synccolors) {
        while (fragments.size() < numberPanes) {
            addLaParolaFragment(restoring);
        }

        mLastOrientation = orientation;

        fourPanesLayout.setPanes(numberPanes, orientation);

        if (synccolors != null)
            for (int i = 0; i < fragments.size(); i++)
                fragments.get(i).setSyncColor(synccolors[i]);
        //setActiveFragment(fragment);
    }

    public void setFinishedLoading(boolean value) {
        mFinishedLoading = value;

        if (value) {
            setSupportProgressBarIndeterminateVisibility(false);
            findViewById(R.id.loading).setVisibility(View.GONE);
        }
    }

    public int getSyncColor(int i) {
        if (i < fragments.size()) {
            return fragments.get(i).getSyncColor();
        } else {
            return LaParolaPreferences.syncColor[i];
        }
    }

    public void syncPanels(LaParolaFragment master) {
        LaParolaUrl urlCorrente = master.getUrlCorrente();
        for (int i = 0; i < fragments.size(); i++) {
            LaParolaFragment slave = fragments.get(i);
            if (slave != master && slave.getSyncColor() == master.getSyncColor()) {
                slave.setIgnoreNextUrlForSync(true);
                slave.vaiAdUrl(urlCorrente.getUrlConAltraVersione(slave.getVersione()));
            }
        }
    }

    public void openInPopupWindow(LaParolaUrl url) {
        PopupDialog pd = new PopupDialog(this);
        pd.setUrl(url);
        pd.show();
    }

    public void openInNewPanel(String url) {
        final int numberPanes = fourPanesLayout.getNumberPanes();
        if (numberPanes < MAX_PANELS) {
            setPanes(numberPanes + 1, mLastOrientation, false, null);

            LaParolaFragment f = fragments.get(numberPanes);
            f.setSyncColor(getFreeSyncColor());
            LaParolaUrl lpUrl = getActiveFragment().nuovoUrl(url);
            if (!f.isCreated()) {
                f.onCreateGoToUrl = lpUrl.getUrl();
            } else {
                f.vaiAdUrl(lpUrl);
            }
        } else {
            Toast.makeText(this, R.string.error_open_new_panel, Toast.LENGTH_LONG).show();
        }
    }

    private int getFreeSyncColor() {
        for (int r = 0; r < MAX_PANELS; r++) {
            boolean ok = true;
            for (int i = 0; i < fragments.size(); i++) {
                if (fragments.get(i).getSyncColor() == r) {
                    ok = false;
                    break;
                }
            }
            if (ok) return r;
        }
        return -1;
    }

    public void closeActivePanel() {
        int panesNumber = getPanesNumber();
        LaParolaFragment f = getActiveFragment();

        setPanes(panesNumber - 1, mLastOrientation, false, null);

        for (int i = 0; i < panesNumber; i++) {
            if (fragments.get(i) == f) {
                for (int j = i; j < panesNumber - 1; j++) {
                    switchPanels(fragments.get(j), fragments.get(j + 1));
                }
                return;
            }
        }
    }

    public void switchPanels() {
        if (getPanesNumber() == 1)
            return;

        (Toast.makeText(this, R.string.select_panel, Toast.LENGTH_LONG)).show();
        mSwitchingPanels = getActiveFragment();
    }

    public void selectPanelForOpening(String url) {
        selectPanelForOpening(getActiveFragment().nuovoUrl(url));
    }

    public void selectPanelForOpening(LaParolaUrl url) {
        if (getPanesNumber() == 1) {
            LaParolaFragment f = getActiveFragment();
            //String versione = f.getUrlCorrente().versione;
            //f.vaiAdUrl(url.getUrlConAltraVersione(versione));

            f.vaiAdUrl(url);
        } else {
            (Toast.makeText(this, R.string.select_panel, Toast.LENGTH_LONG)).show();
            mSelectingPanelForOpeningUrl = url;
        }
    }

    public void onFragmentTouch(LaParolaFragment mFragment) {
        setActiveFragment(mFragment);

        if (mSelectingPanelForOpeningUrl != null) {
            //String versione = mFragment.getUrlCorrente().versione;
            //mFragment.vaiAdUrl(mSelectingPanelForOpening.getUrlConAltraVersione(versione));
            mFragment.vaiAdUrl(mSelectingPanelForOpeningUrl);

            for (int i = 0; i < fragments.size(); i++) {
                LaParolaFragment f = fragments.get(i);
                if (f != mFragment && f.getSyncColor() == mFragment.getSyncColor()) {
                    (Toast.makeText(this, R.string.link_broken, Toast.LENGTH_LONG)).show();
                    mFragment.setSyncColor(getFreeSyncColor());
                    break;
                }
            }

            mSelectingPanelForOpeningUrl = null;
        } else if (mSwitchingPanels != null) {
            switchPanels(mFragment, mSwitchingPanels);

            mSwitchingPanels = null;
        }
    }

    private void switchPanels(LaParolaFragment f1, LaParolaFragment f2) {
        int c1 = f1.getSyncColor();
        int c2 = f2.getSyncColor();
        LaParolaUrl u1 = f1.getUrlCorrente();
        LaParolaUrl u2 = f2.getUrlCorrente();

        f1.setSyncColor(c2);
        f2.setSyncColor(c1);

        f1.vaiAdUrl(u2);
        f2.vaiAdUrl(u1);
    }

    public void openInNewPanel(LaParolaUrl url) {
        openInNewPanel(url.getUrl());
    }

    public void openInPopupWindow(String url) {
        openInPopupWindow(getActiveFragment().nuovoUrl(url));
    }

    @Override
    public String getUltimaBibbiaSalvata() {
        return LaParolaPreferences.lastBible;
    }

    @Override
    public void setUltimaBibbiaSalvata(String versione) {
        LaParolaPreferences.lastBible = versione;
    }

    public void showPanelsManagment() {
        if (isFinishing())
            return;

        PanelsDialog diag = new PanelsDialog(this);
        diag.show();
    }

    @Override
    public void mostraEliminaPreferito(final LaParolaUrl nuovoUrl) {
        MessageDialog m = new MessageDialog(this, 0, R.string.delete_starred);
        m.setYesNo(R.string.delete, android.R.string.cancel, new Runnable() {
            @Override
            public void run() {
                LaParolaBrowser.rimuoviPreferito(nuovoUrl);
                LaParolaBrowser.salvaPreferitiSuFile();
                LaParolaFragment fragment = getActiveFragment();
                if (fragment != null) {
                    fragment.aggiornaPagina();
                }
            }
        }, null);
        m.show();
    }

    public void refreshAll() {
        for (LaParolaFragment f : fragments)
            f.aggiornaPagina();
    }

    @Override
    public void installaCarattereGreco() {
        InstallGentiumHelper.install(this);
    }

    @Override
    public void apriGestioneVersioni() {
        this.startActivity(new Intent(this, LibraryActivity.class));
    }

    @Override
    public String getPercorsoAsset() {
        return "file:///android_asset/";
    }

    @Override
    public void mostraOpzioni() {
        startActivity(new Intent(this, LaParolaPreferencesActivity.class));
    }

    @Override
    public void mostraGestorePannelli() {
        showPanelsManagment();
    }

    private void setActivityTitle() {
        setTitle(R.string.app_name);
        if (!mDrawerLayout.isDrawerOpen(GravityCompat.START)) {
            LaParolaFragment af = getActiveFragment();
            if (af != null) {
                LaParolaUrl url = af.getUrlCorrente();
                if (url != null && !af.inHome()) {
                    setTitle(url.getDescrizione());
                }
            }
        }
    }

    public int getPanelsOrientation() {
        return fourPanesLayout.getOrientation() == FourPanesLayout.HORIZONTAL ? FourPanesLayout.VERTICAL : FourPanesLayout.HORIZONTAL;
    }

    private void showAccessibilityDialog() {
        if (isFinishing())
            return;

        AccessibilityDialog d = new AccessibilityDialog(this);
        d.show();
    }

    public void showSearch() {
        searchActionItemManager.expandActionView();
    }

    public void toggleDrawerGroupExpansion(int id) {
        if (mDrawerList.isGroupExpanded(id)) {
            mDrawerList.collapseGroup(id);
        } else {
            mDrawerList.expandGroup(id);
        }
    }

    @Override
    public boolean onChildClick(ExpandableListView expandableListView, View view, int g, int i, long l) {
        int id = (int)mDrawerAdapter.getChildId(g, i);
        String link = mDrawerAdapter.getLink(id);
        LaParolaFragment activeFragment = getActiveFragment();
        if (activeFragment != null) {
            activeFragment.vaiAdUrl(link);
            mDrawerLayout.closeDrawer(GravityCompat.START);
        }

        /*
        if (link.startsWith("laparola:")) {
            referenceActionItemManager.expandActionView();
        }
        */

        return true;
    }

    @Override
    public boolean onGroupClick(ExpandableListView expandableListView, View view, int g, long l) {
        return onChildClick(expandableListView, view, g, -1, l);
    }

    public void showPanelContextMenu() {
        if (mPanelContextMenuView == null) {
            mPanelContextMenuView = new View(this) {
                @Override
                protected void onCreateContextMenu(ContextMenu menu) {
                    super.onCreateContextMenu(menu);
                    createBibleViewContextMenu(menu);
                }
            };

            fourPanesLayout.post(new Runnable() {
                @Override
                public void run() {
                    mPanelContextMenuView.setVisibility(View.GONE);
                    ((FrameLayout)findViewById(R.id.main_activity_container)).addView(mPanelContextMenuView);
                    registerForContextMenu(mPanelContextMenuView);
                }
            });
        }

        fourPanesLayout.post(new Runnable() {
            @Override
            public void run() {
                mPanelContextMenuView.showContextMenu();
            }
        });
    }

    public void createBibleViewContextMenu(ContextMenu menu) {
        final int ID_CHIUDI = 10;
        final int ID_APRI_NUOVO_PANNELLO = 11;
        final int ID_APRI_PANNELLO_ESISTENTE = 12;
        final int ID_GESTIONE_PANNELLI = 13;
        //final int ID_SELEZIONE = 14;

        final android.view.MenuItem.OnMenuItemClickListener handler = new android.view.MenuItem.OnMenuItemClickListener() {
            public boolean onMenuItemClick(android.view.MenuItem item) {
                if (item.getItemId() == ID_CHIUDI) {
                    closeActivePanel();
                } else if (item.getItemId() == ID_APRI_NUOVO_PANNELLO) {
                    openInNewPanel(getActiveFragment().getUrlCorrente());
                } else if (item.getItemId() == ID_APRI_PANNELLO_ESISTENTE) {
                    switchPanels();
                } else if (item.getItemId() == ID_GESTIONE_PANNELLI) {
                    showPanelsManagment();
                } /*else if (item.getItemId() == ID_SELEZIONE) {
                        copy(R.string.select_text_copy);
                    }*/
                return true;
            }
        };

        if (getPanesNumber() > 1) {
            menu.add(0, ID_CHIUDI, 0, R.string.close_panel).setOnMenuItemClickListener(handler);
        }
        if (getPanesNumber() < LaParolaActivity.MAX_PANELS) {
            menu.add(0, ID_APRI_NUOVO_PANNELLO, 0, R.string.duplicate_panel).setOnMenuItemClickListener(handler);
        }
        if (getPanesNumber() > 1) {
            menu.add(0, ID_APRI_PANNELLO_ESISTENTE, 0, R.string.switch_panel).setOnMenuItemClickListener(handler);
        }
        menu.add(0, ID_GESTIONE_PANNELLI, 0, R.string.fragments_management).setOnMenuItemClickListener(handler);

        /* Prima di Honeycomb bisogna usare "copia" e "condividi" dal menù, se si usa il
         * context menu è come se la webview non ricevesse il "action_up"
         *
        if (Build.VERSION.SDK_INT < Build.VERSION_CODES.HONEYCOMB) {
            menu.add(0, ID_SELEZIONE, 0, R.string.select_text).setOnMenuItemClickListener(handler);
        }
        */
    }
}
