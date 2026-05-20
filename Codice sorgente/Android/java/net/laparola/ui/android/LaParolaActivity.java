package net.laparola.ui.android;

import android.Manifest;
import android.content.ActivityNotFoundException;
import android.content.Context;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.content.res.Configuration;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.text.Editable;
import android.view.KeyEvent;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.inputmethod.InputMethodManager;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import com.google.android.material.appbar.MaterialToolbar;
import com.google.android.material.bottomnavigation.BottomNavigationView;
import com.google.android.material.bottomsheet.BottomSheetBehavior;
import com.google.android.material.bottomsheet.BottomSheetDialog;
import com.google.android.material.navigation.NavigationView;
import com.google.android.material.progressindicator.CircularProgressIndicator;

import androidx.annotation.NonNull;
import androidx.appcompat.app.ActionBarDrawerToggle;
import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.view.ActionMode;
import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;
import androidx.core.view.GravityCompat;
import androidx.drawerlayout.widget.DrawerLayout;
import androidx.fragment.app.FragmentTransaction;
import timber.log.Timber;

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
import net.laparola.ui.android.actionbar.LibraryActionItemManager;
import net.laparola.ui.android.actionbar.ReferenceActionItemManager;
import net.laparola.ui.android.actionbar.SearchActionItemManager;
import net.laparola.ui.android.actionbar.TTSActionItemManager;
import net.laparola.ui.android.dialogs.AccessibilityDialog;
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
import java.util.Objects;

import static android.view.View.GONE;
import static android.view.View.VISIBLE;

public class LaParolaActivity extends AppCompatActivity implements LaParolaBrowserStaticClient {
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

    private DrawerLayout mLeftDrawerLayout;
    private ActionBarDrawerToggle mDrawerToggle;

    private LaParolaFragment activeFragment;

    private SearchActionItemManager searchActionItemManager;
    private ReferenceActionItemManager referenceActionItemManager;
    private LibraryActionItemManager libraryActionItemManager;
    private TTSActionItemManager ttsActionItemManager;

    private MenuItem referenceActionItem;
    private MenuItem searchActionItem;
    private MenuItem libraryActionItem;
    private MenuItem starActionItem;
    private MenuItem forwardActionItem;
    private MenuItem highlighterActionItem;
    private MenuItem nightModeActionItem;
    private MenuItem zoomInActionItem;
    private MenuItem zoomOutActionItem;

    private MenuItem forwardBottomItem;
    private MenuItem starBottomItem;

    //private boolean firstReferenceClicked = true;

    private boolean mGoingBack;
    private long lastBackPressedTime = 0;
    private boolean mFinishedLoading = false;

    protected boolean isPaused;
    private FourPanesLayout fourPanesLayout;

    /* package */ List<LaParolaFragment> fragments;

    private LaParolaActivityInitUtility initUtility;

    private int mLastOrientation;

    private LaParolaUrl mSelectingPanelForOpeningUrl;
    private LaParolaFragment mSwitchingPanels;
    /* package */ ActionMode actionMode;

    public boolean isTablet;

    private void checkAllPermissions() {
        // Handle Storage (Android 12 and below)
        // We only need this if we are writing to shared/external storage
        if (Build.VERSION.SDK_INT < Build.VERSION_CODES.TIRAMISU) { // 33
            if (ContextCompat.checkSelfPermission(this, Manifest.permission.WRITE_EXTERNAL_STORAGE)
                    != PackageManager.PERMISSION_GRANTED) {

                // Trigger the storage permission popup for the old tablet
                ActivityCompat.requestPermissions(this,
                        new String[]{Manifest.permission.WRITE_EXTERNAL_STORAGE},
                        LaParolaActivity.MY_PERMISSIONS_REQUEST_WRITE_EXTERNAL_STORAGE);
            }
        }
    }

    /* no longer necessary, actually blocks the program on old Android versions
    private boolean checkPermissions() {
        if (ContextCompat.checkSelfPermission(this, Manifest.permission.WAKE_LOCK) != PackageManager.PERMISSION_GRANTED) {
            Timber.tag("LaParola").d("Non ho l'autorizzazione per WAKE_LOCK");
        }
        if (ContextCompat.checkSelfPermission(this, Manifest.permission.INTERNET) != PackageManager.PERMISSION_GRANTED) {
            Timber.tag("LaParola").d("Non ho l'autorizzazione per INTERNET");
        }
        if (Build.VERSION.SDK_INT < 33 && ContextCompat.checkSelfPermission(this, Manifest.permission.WRITE_EXTERNAL_STORAGE) != PackageManager.PERMISSION_GRANTED) {
            Timber.tag("LaParola").d("Non ho l'autorizzazione per WRITE_EXTERNAL_STORAGE");

            // Permission is not granted
            // Should we show an explanation?
            if (ActivityCompat.shouldShowRequestPermissionRationale(this, Manifest.permission.WRITE_EXTERNAL_STORAGE)) {
                // Show an explanation to the user *asynchronously* -- don't block
                // this thread waiting for the user's response! After the user
                // sees the explanation, try again to request the permission.

                if (!this.isFinishing()) {
                    this.runOnUiThread(() -> {
                        LaParolaDialog d = new MessageDialog(this, R.string.error, R.string.permission_write_storage);
                        d.setOnDismissListener((dialog) -> ActivityCompat.requestPermissions(this, new String[]{Manifest.permission.WRITE_EXTERNAL_STORAGE}, LaParolaActivity.MY_PERMISSIONS_REQUEST_WRITE_EXTERNAL_STORAGE));
                        d.show();
                    });
                }
            } else {
                // No explanation needed; request the permission
                ActivityCompat.requestPermissions(this, new String[]{Manifest.permission.WRITE_EXTERNAL_STORAGE}, LaParolaActivity.MY_PERMISSIONS_REQUEST_WRITE_EXTERNAL_STORAGE);
            }

            return false;
        } else {
            Timber.tag("LaParola").d("Autorizzazione per WRITE_EXTERNAL_STORAGE accordata");
        }
        return true;
    }

    @Override
    public void onRequestPermissionsResult(int requestCode, @NonNull String[] permissions, @NonNull int[] grantResults) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults);
        if (requestCode == MY_PERMISSIONS_REQUEST_WRITE_EXTERNAL_STORAGE) {// If request is canceled, the result arrays are empty.
            if (grantResults.length > 0 && grantResults[0] == PackageManager.PERMISSION_GRANTED) {
                onFinishedLoadingActivity();
            } else {
                if (!this.isFinishing()) {
                    this.runOnUiThread(() -> {
                        LaParolaDialog d = new MessageDialog(this, R.string.error, R.string.permission_write_storage_denied);
                        d.setOnDismissListener((dialog) -> this.finish());
                        d.show();
                    });
                }
            }
        }
    }
*/
    @Override
    public InputStream apriFile(String filename) {
        try {
            if ((new File(filename)).exists()) return new FileInputStream(filename);
            return getAssets().open(filename);
        } catch (Exception e) {
            if (!(e instanceof FileNotFoundException)) {
                Timber.e(e, "Unexpected IO error occurred while opening file.");
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
        if (isFinishing() || isDestroyed()) {
            return super.dispatchKeyEvent(event);
        }

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
                        setEnabledForward(true);
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
        } else {
            if (LaParolaPreferences.useVolumeKeys && (keyCode == KeyEvent.KEYCODE_VOLUME_DOWN || keyCode == KeyEvent.KEYCODE_VOLUME_UP) && ttsActionItemManager!=null && !ttsActionItemManager.isExpanded()) {
                return true;
            }
        }

        return super.dispatchKeyEvent(event);
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        if (BuildConfig.DEBUG) {
            Timber.plant(new Timber.DebugTree());
        }
        isTablet = getResources().getBoolean(R.bool.isTablet);

        setContentView(R.layout.main_activity);

        MaterialToolbar toolbar = findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);
        toolbar.setLogo(R.drawable.ic_launcher);
        toolbar.setTitle(getTitle());

        CircularProgressIndicator progressIndicator = findViewById(R.id.progressIndicator);
        progressIndicator.setVisibility(VISIBLE);
        progressIndicator.show();

        fourPanesLayout = findViewById(R.id.four_panes_layout);

        fragments = new ArrayList<>();

        LaParolaPreferences.load(this);

        setupDrawer();

        /*
         * TODO : usare risorse mLaParolaBrowser.Stringhe.Errore_Nessuna_versione = getContext().getString(R.string.no_version_present);
         * mLaParolaBrowser.Stringhe.Errore_Non_presente = getContext().getString(R.string.not_present);
         */

        progressIndicator.hide();
        progressIndicator.setVisibility(GONE);
    }

    private void showExtraMenu(View anchor) {
        // 1. Create the PopupMenu anchored to the "More" icon
        androidx.appcompat.widget.PopupMenu popup = new androidx.appcompat.widget.PopupMenu(this, anchor);

        // 2. Inflate your menu resource
        popup.getMenuInflater().inflate(R.menu.bottom_extra_menu, popup.getMenu());

        MenuItem nightmodeItem = popup.getMenu().findItem(R.id.menu_item_night_mode_bottom);
        if (nightmodeItem != null) {
            nightmodeItem.setTitle(LaParolaPreferences.nightMode ? R.string.night_mode_off : R.string.night_mode_on);
        }
        MenuItem zoomInItem = popup.getMenu().findItem(R.id.menu_item_zoom_in);
        if (zoomInItem != null) {
            zoomInItem.setVisible(LaParolaPreferences.menuZoom);
        }
        MenuItem zoomOutItem = popup.getMenu().findItem(R.id.menu_item_zoom_out);
        if (zoomOutItem != null) {
            zoomOutItem.setVisible(LaParolaPreferences.menuZoom);
        }

        // 3. Handle clicks by routing them to your existing onOptionsItemSelected
        // This keeps your logic centralized!
        popup.setOnMenuItemClickListener(this::onOptionsItemSelected);

        // 4. Show the menu
        popup.show();
    }

    private void setupDrawer() {
        mLeftDrawerLayout = findViewById(R.id.left_drawer_layout);

        NavigationView mNavigationView = findViewById(R.id.navigation_view);

        mDrawerToggle = new ActionBarDrawerToggle(this, mLeftDrawerLayout, R.string.drawer_open, R.string.drawer_close) {
            @Override
            public void onDrawerClosed(View view) {
                super.onDrawerClosed(view);
                setActivityTitle();
            }

            @Override
            public void onDrawerOpened(View drawerView) {
                super.onDrawerOpened(drawerView);
                setActivityTitle();
            }
        };

        mLeftDrawerLayout.addDrawerListener(mDrawerToggle);
        mLeftDrawerLayout.post(() -> mDrawerToggle.syncState());

        // Enable home button and "hamburger" icon
        Objects.requireNonNull(getSupportActionBar()).setDisplayHomeAsUpEnabled(true);
        getSupportActionBar().setHomeButtonEnabled(true);
        mNavigationView.setNavigationItemSelectedListener(item -> {
            handleNavigationItem(item.getItemId());
            DrawerLayout drawer = mLeftDrawerLayout;
            drawer.closeDrawers();
            return true;
        });
    }

    private void handleNavigationItem(int itemId) {
        String link = getLink(itemId);
        executeNavigationLink(link);
    }

    private void executeNavigationLink(String link) {
        LaParolaFragment activeFragment = getActiveFragment();
        if (activeFragment != null && !link.isEmpty()) {
            activeFragment.vaiAdUrl(link);
        }
    }

    private static String getLink(int itemId) {
        String link = "";
        if (itemId == R.id.nav_bible) {
            link = "laparola:@*bibbia";
        } else if (itemId == R.id.nav_parola_giorno) {
            link = "lpfile:Parola del giorno.html";
        } else if (itemId == R.id.nav_liturgia) {
            link = "lpfile:Liturgia del giorno.html";
        } else if (itemId == R.id.nav_casuale) {
            link = "lpcomando:casuale";
        } else if (itemId == R.id.nav_casuale_at) {
            link = "lpcomando:casualeat";
        } else if (itemId == R.id.nav_casuale_nt) {
            link = "lpcomando:casualent";
        } else if (itemId == R.id.nav_bookmarks) {
            link = "lpsegnalibri:";
        } else if (itemId == R.id.nav_starred) {
            link = "lppreferiti:";
        } else if (itemId == R.id.nav_highlight_color) {
            link = "lpevidenziati:";
        } else if (itemId == R.id.nav_history) {
            link = "lpcronologia:";
        } else if (itemId == R.id.nav_settings) {
            link = "lpcomando:impostazioni";
        } else if (itemId == R.id.nav_help) {
            link = "lpfile:Guida.html";
        }
        return link;
    }

    /**
     * Navigates to a specific reference and hides the selection UI.
     *
     * @param reference The text reference (e.g., "John 3:16")
     * @param dialog    The BottomSheetDialog to dismiss (can be null if in tablet mode)
     */
    public void executeAndClose(String reference, BottomSheetDialog dialog) {
        if (reference != null && !reference.isEmpty()) {
            // 1. Navigate using your existing fragment method
            if (getActiveFragment() != null) {
                getActiveFragment().vaiARiferimento(Editable.Factory.getInstance().newEditable(reference));
            }

            // 2. Hide keyboard
            InputMethodManager imm = (InputMethodManager) getSystemService(Context.INPUT_METHOD_SERVICE);
            View view = getCurrentFocus();
            if (view != null) imm.hideSoftInputFromWindow(view.getWindowToken(), 0);

            // 3. Close the UI
            if (isTablet) {
                // Your collapse method requires a MenuItem parameter
                referenceActionItemManager.collapse(null);
            } else if (dialog != null) {
                dialog.dismiss();
            }
        }
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.action_bar, menu);
        searchActionItem = menu.findItem(R.id.menu_item_search);
        referenceActionItem = menu.findItem(R.id.menu_item_reference);
        libraryActionItem = menu.findItem(R.id.menu_item_library);
        starActionItem = menu.findItem(R.id.menu_item_star);
        forwardActionItem = menu.findItem(R.id.menu_item_forward);
        highlighterActionItem = menu.findItem(R.id.menu_item_highlighter);
        nightModeActionItem = menu.findItem(R.id.menu_item_night_mode);
        MenuItem ttsActionItem = menu.findItem(R.id.menu_item_tts);
        zoomInActionItem = menu.findItem(R.id.menu_item_zoom_in);
        zoomOutActionItem = menu.findItem(R.id.menu_item_zoom_out);

        zoomInActionItem.setVisible(true);
        zoomOutActionItem.setVisible(true);

        BottomNavigationView bottomNavigationView = findViewById(R.id.bottom_nav);
        bottomNavigationView.setVisibility(isTablet ? GONE : VISIBLE);
        Menu bottomNavigationViewMenu = bottomNavigationView.getMenu();
        forwardBottomItem = bottomNavigationViewMenu.findItem(R.id.bottom_item_forward);
        starBottomItem = bottomNavigationViewMenu.findItem(R.id.bottom_item_star);

        if (!isTablet) {
            bottomNavigationView.setOnItemSelectedListener(item -> {
                int itemId = item.getItemId();

                if (itemId == R.id.bottom_item_reference) {
                    if (LaParolaPreferences.accessibilityMode) {
                        showAccessibilityDialog();
                    } else {
                        referenceActionItemManager.expandActionView();
                    }
                    return true;
                } else if (itemId == R.id.bottom_item_search) {
                    searchActionItemManager.expandActionView();
                    return true;
                } else if (itemId == R.id.bottom_item_library) {
                    libraryActionItemManager.expandActionView();
                    return true;
                } else if (itemId == R.id.bottom_item_star) {
                    showStarredBottomSheet();
                    return true;
                } else if (itemId == R.id.bottom_item_forward) {
                    eseguiForward();
                    return true;
                } else if (itemId == R.id.bottom_item_extra) {
                    View anchor = findViewById(R.id.bottom_item_extra);
                    showExtraMenu(anchor);
                    return true;
                }
                return false;
            });
        }

        if (isTablet) {
            searchActionItem.setShowAsAction(MenuItem.SHOW_AS_ACTION_IF_ROOM | MenuItem.SHOW_AS_ACTION_COLLAPSE_ACTION_VIEW);
            referenceActionItem.setShowAsAction(MenuItem.SHOW_AS_ACTION_IF_ROOM | MenuItem.SHOW_AS_ACTION_COLLAPSE_ACTION_VIEW);
            libraryActionItem.setShowAsAction(MenuItem.SHOW_AS_ACTION_IF_ROOM | MenuItem.SHOW_AS_ACTION_COLLAPSE_ACTION_VIEW);
            forwardActionItem.setShowAsAction(MenuItem.SHOW_AS_ACTION_IF_ROOM);
            starActionItem.setShowAsAction(MenuItem.SHOW_AS_ACTION_IF_ROOM);
        } else {
            nascondiAction();
            forwardActionItem.setShowAsAction(MenuItem.SHOW_AS_ACTION_NEVER);
            referenceActionItem.setVisible(false);
            searchActionItem.setVisible(false);
            libraryActionItem.setVisible(false);
            starActionItem.setVisible(false);
            forwardActionItem.setVisible(false);
            highlighterActionItem.setVisible(false);
            ttsActionItem.setVisible(false);
            nightModeActionItem.setVisible(false);
            zoomInActionItem.setVisible(false);
            zoomOutActionItem.setVisible(false);
            MenuItem panelsItem = menu.findItem(R.id.menu_item_panels);
            if (panelsItem != null) panelsItem.setVisible(false);
            MenuItem libraryItem = menu.findItem(R.id.menu_item_library_management);
            if (libraryItem != null) libraryItem.setVisible(false);

        }

        searchActionItem.setActionView(R.layout.search_action_view);
        referenceActionItem.setActionView(R.layout.reference_action_view);
        libraryActionItem.setActionView(R.layout.version_action_view);

        searchActionItemManager = new SearchActionItemManager(this, searchActionItem);
        referenceActionItemManager = new ReferenceActionItemManager(this, referenceActionItem);
        libraryActionItemManager = new LibraryActionItemManager(this, libraryActionItem);

        if (referenceActionItemManager != null) {
            // We override the default behavior to check for Accessibility Mode
            View actionView = referenceActionItem.getActionView();

            if (actionView != null) {
                actionView.setOnClickListener(v -> {
                    if (LaParolaPreferences.accessibilityMode) {
                        showAccessibilityDialog();
                    } else {
                        referenceActionItemManager.expandActionView();
                    }
                });
            }
        }


        ttsActionItem.setShowAsAction(MenuItem.SHOW_AS_ACTION_NEVER | MenuItem.SHOW_AS_ACTION_COLLAPSE_ACTION_VIEW);
        ttsActionItem.setActionView(R.layout.tts_action_view);
        ttsActionItemManager = new TTSActionItemManager(this, this, ttsActionItem);

        nightModeActionItem.setTitle(LaParolaPreferences.nightMode ? R.string.night_mode_off : R.string.night_mode_on);

        onFinishedLoadingActivity();

        return super.onCreateOptionsMenu(menu);
    }

    private void nascondiAction() {
        if (mLeftDrawerLayout != null && mLeftDrawerLayout.isDrawerOpen(GravityCompat.START)) {
            // Skip changes while drawer is open
            return;
        }

        if (referenceActionItemManager != null) referenceActionItemManager.resettaView();
        searchActionItem.setShowAsAction(MenuItem.SHOW_AS_ACTION_NEVER);
        referenceActionItem.setShowAsAction(MenuItem.SHOW_AS_ACTION_NEVER);
        libraryActionItem.setShowAsAction(MenuItem.SHOW_AS_ACTION_NEVER);
        starActionItem.setShowAsAction(MenuItem.SHOW_AS_ACTION_NEVER);
    }

    @Override
    public boolean onPrepareOptionsMenu(Menu menu) {
        super.onPrepareOptionsMenu(menu);
        if (!isTablet) {
            menu.findItem(R.id.menu_item_search).setVisible(false);
            menu.findItem(R.id.menu_item_reference).setVisible(false);
            menu.findItem(R.id.menu_item_library).setVisible(false);
            menu.findItem(R.id.menu_item_star).setVisible(false);
            menu.findItem(R.id.menu_item_forward).setVisible(false);
        }
        return true;
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
        starActionItem.setEnabled(true);
        starActionItem.setIcon(R.drawable.ic_action_unstarred);
        starBottomItem.setIcon(R.drawable.ic_action_unstarred);

        LaParolaUrl currentUrl = activeFragment.getUrlCorrente();
        if (currentUrl == null) {
            return;
        }

        Segnalibro s = LaParolaBrowser.cercaUrlTraPreferiti(currentUrl);
        if (s != null) {
            starActionItem.setIcon(R.drawable.ic_action_starred);
            starBottomItem.setIcon(R.drawable.ic_action_starred);
        }
    }

    @Override
    public boolean onOptionsItemSelected(@NonNull MenuItem item) {
        if (initUtility != null) {
            if (initUtility.isWorking()) return true;
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
            eseguiForward();
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
        } else if (itemId == R.id.menu_item_night_mode || itemId == R.id.menu_item_night_mode_bottom) {
            setNightMode(!LaParolaPreferences.nightMode);
            return true;
        } else if (itemId == R.id.menu_item_zoom_out) {
            if (activeFragment != null) {
                activeFragment.setTextZoom(activeFragment.getTextZoom() - 10);
            }
            return true;
        } else if (itemId == R.id.menu_item_panels) {
            showPanelsManagment();
            return true;
        } else if (itemId == R.id.menu_item_library_management) {
            startActivity(new Intent(this, LibraryActivity.class));
            return true;
        } else {
            // workaround per bug di actionbarsherlock
// todo probabilmente da cancellare
            if (itemId == R.id.menu_item_search) {
                collapseActionViewsExcept(searchActionItem);
                searchActionItemManager.expandActionView();
                return true;
            } else if (itemId == R.id.menu_item_library) {
                collapseActionViewsExcept(libraryActionItem);
                libraryActionItemManager.expandActionView();
                return true;
            } else if (itemId == R.id.menu_item_reference) {
                if (getActiveFragment() != null) {
                    EnumSet<Testi.TestoTipi> tipoTesto = getActiveFragment().getInformazioniVersione().getTipo();
                    // rmw1024 referenceActionItemManager.setDizionario(tipoTesto.contains(Testi.TestoTipi.DIZIONARIO));
                }

                if (LaParolaPreferences.accessibilityMode) showAccessibilityDialog();

                collapseActionViewsExcept(referenceActionItem);
                referenceActionItemManager.expandActionView();
                return true;
            } else if (itemId == R.id.menu_item_tts) {
                ttsActionItemManager.expandActionView();
                return true;
            }
        }

        return super.onOptionsItemSelected(item);
    }

    private void eseguiForward() {
        if (activeFragment != null) {
            activeFragment.vaiASuccessivo();
        }
    }

    private void setEnabledForward(boolean value) {
        forwardActionItem.setEnabled(value);
        if (forwardBottomItem != null) {
            forwardBottomItem.setEnabled(value);
            if (forwardBottomItem.isEnabled()) {
                forwardBottomItem.setIcon(R.drawable.ic_action_forward_enabled);
            } else {
                forwardBottomItem.setIcon(R.drawable.ic_action_forward_disabled);
            }
        }
    }

    public boolean startHighlighter() {
        LaParolaHighlighterActionModeCallback acc = new LaParolaHighlighterActionModeCallback(this);
        if (acc.setup()) {
            actionMode = startSupportActionMode(acc);
            if (actionMode != null)
                actionMode.setTitle(R.string.highlighter_title);
            return true;
        }
        return false;
    }

    private void showStarredBottomSheet() {
        LaParolaFragment fragment = getActiveFragment();
        if (fragment == null) return;

        final LaParolaUrl url = fragment.getUrlCorrente();
        final BottomSheetDialog starDialog = new BottomSheetDialog(this);
        View sheetView = getLayoutInflater().inflate(R.layout.bottom_sheet_starred, null, false);

        EditText descriptionInput = sheetView.findViewById(R.id.starred_description);
        Button saveBtn = sheetView.findViewById(R.id.starred_save_btn);
        Button removeBtn = sheetView.findViewById(R.id.starred_remove_btn);

        final net.laparola.ui.LaParolaSegnalibri.Segnalibro s = net.laparola.ui.LaParolaBrowser.cercaUrlTraPreferiti(url);

        if (s != null) {
            descriptionInput.setText(s.nome);
            removeBtn.setVisibility(View.VISIBLE);
        } else {
            if (url != null && url.getDescrizione() != null)
                descriptionInput.setText(url.getDescrizione());
            else
                descriptionInput.setText("");
        }

        saveBtn.setOnClickListener(v -> {
            if (url!=null) {
                String desc = descriptionInput.getText().toString();
                if (s == null) {
                    LaParolaBrowser.aggiungiPreferito("Preferiti", desc, url);
                } else {
                    s.setAncoraggio(url.ancoraggio);
                    s.nome = desc;
                }
                LaParolaBrowser.salvaPreferitiSuFile();
            }
            starDialog.dismiss();
        });

        removeBtn.setOnClickListener(v -> {
            if (s != null) {
                LaParolaBrowser.rimuoviPreferito(url);
                LaParolaBrowser.salvaPreferitiSuFile();
            }
            starDialog.dismiss();
        });

        starDialog.setContentView(sheetView);
        starDialog.show();
        starDialog.setOnDismissListener(dialog -> updateStar());
    }

    private void showStarDialog() {
        if (isFinishing()) return;

        if (activeFragment == null) return;
        LaParolaUrl urlCorrente = activeFragment.getUrlCorrente();
        if (urlCorrente == null) return;

        Segnalibro s = LaParolaBrowser.cercaUrlTraPreferiti(urlCorrente);

        StarredDialog bookmark = new StarredDialog(this);
        bookmark.show();
        if (s == null) {
            bookmark.setDescription(urlCorrente.getDescrizione());
            bookmark.setSegnalibroNonEsiste();
        } else {
            bookmark.setDescription(s.nome);
        }
        bookmark.url = urlCorrente;
        bookmark.setOnDismissListener(dialog -> updateStar());
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
            fourPanesLayout.postDelayed(this::applyPreferences, 100);
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
        mLeftDrawerLayout.setKeepScreenOn(LaParolaPreferences.keepScreenOn);
        if (isTablet) {
            zoomInActionItem.setVisible(LaParolaPreferences.menuZoom);
            zoomOutActionItem.setVisible(LaParolaPreferences.menuZoom);
        }

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
        m.setYesNo(R.string.clear, android.R.string.cancel, () -> {
            LaParolaBrowser.pulisciCronologia();
            activeFragment.aggiornaPagina();   // la pagina della cronologia
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
        }
    }

    public void updateActionBar() {
        referenceActionItemManager.updateBooks();

        mGoingBack = false;
        setEnabledForward(activeFragment.successivoEsiste());

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

        if (isTablet && currentUrl != null && currentUrl.gestito) {
            libraryActionItem.setVisible(true);
            referenceActionItem.setVisible(true);
            searchActionItem.setVisible(true);
            starActionItem.setVisible(true);
            highlighterActionItem.setVisible(true);
            if (currentUrl.schema.equals("lpsegnalibri")) {
                referenceActionItem.setVisible(false);
                libraryActionItem.setVisible(false);
                searchActionItem.setVisible(false);
                starActionItem.setVisible(false);
                highlighterActionItem.setVisible(false);
            }
            if (currentUrl.schema.equals("lpsegnalibro")) {
                referenceActionItem.setVisible(false);
                libraryActionItem.setVisible(false);
                searchActionItem.setVisible(false);
                starActionItem.setVisible(false);
                highlighterActionItem.setVisible(false);
            }
            if (currentUrl.schema.equals("lppreferiti")) {
                referenceActionItem.setVisible(false);
                libraryActionItem.setVisible(false);
                searchActionItem.setVisible(false);
                starActionItem.setVisible(false);
                highlighterActionItem.setVisible(false);
            }
            if (currentUrl.schema.equals("lpevidenziati")) {
                referenceActionItem.setVisible(false);
                libraryActionItem.setVisible(false);
                searchActionItem.setVisible(false);
                starActionItem.setVisible(false);
            }
            if (currentUrl.schema.equals("lpcronologia")) {
                libraryActionItem.setVisible(false);
                searchActionItem.setVisible(false);
                referenceActionItem.setVisible(false);
                starActionItem.setVisible(false);
                highlighterActionItem.setVisible(false);
            }
            if (currentUrl.schema.equals("lpfile")) {
                libraryActionItem.setVisible(false);
                searchActionItem.setVisible(false);
                referenceActionItem.setVisible(false);
                starActionItem.setVisible(false);
                highlighterActionItem.setVisible(false);
            }
        }
    }

    @Override
    protected void onPostCreate(Bundle savedInstanceState) {
        super.onPostCreate(savedInstanceState);
        // Sync the toggle state after onRestoreInstanceState has occurred.
        if (mDrawerToggle != null) mDrawerToggle.syncState();
    }

    @Override
    public void onConfigurationChanged(@NonNull Configuration newConfig) {
        super.onConfigurationChanged(newConfig);
        if (mDrawerToggle != null) mDrawerToggle.onConfigurationChanged(newConfig);
    }

    public void onVersionChanged() {
        if (referenceActionItemManager != null) {
            referenceActionItemManager.onVersionChanged();
            libraryActionItemManager.onVersionChanged();
        }
    }

    private void onFinishedLoadingActivity() {
        //checkAllPermissions();

        if (hasFinishedLoading()) return;

        if (initUtility == null) {
            initUtility = new LaParolaActivityInitUtility(this);
            fourPanesLayout.post(() -> {
                // non so esattamente quando possa accadere, ma dai log accade
                // immagino quando si chiude l'applicazione prima che sia terminato il
                // caricamento
                if (initUtility == null) {
                    initUtility = new LaParolaActivityInitUtility(LaParolaActivity.this);
                }
                initUtility.init();
            });
        }

        if (initUtility.isWorking()) {
            fourPanesLayout.postDelayed(this::onFinishedLoadingActivity, 100);
            return;
        }

        if (LaParolaPreferences.homeAtStart) {
            // TODO : se è la prima volta, mostrare help
            mLeftDrawerLayout.openDrawer(GravityCompat.START);
        }

        setFinishedLoading(true);
    }

    public void setActiveFragment(LaParolaFragment value) {
        if (activeFragment == value || value == null || !fragments.contains(value)) return;

        activeFragment = value;
        fourPanesLayout.setSelectedPane(fragments.indexOf(value));

        setActivityTitle();

        if (activeFragment.isCreated()) {
            updateActionBar();

            LaParolaUrl url = activeFragment.getUrlCorrente();
            if (url != null) {
                if (!getUltimaBibbiaSalvata().equals(url.versione)) {
                    VersioneInformazioni informazioniTesto = LaParolaBrowser.getInformazioniTesto(url.versione);
                    if (informazioniTesto != null && informazioniTesto.getTipo().contains(TestoTipi.BIBBIA)) {
                        setUltimaBibbiaSalvata(url.versione);
                    }
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
                goToUrl = LaParolaPreferences.lastUrl[position];
            }
        } else {
            goToUrl = getActiveFragment().getUrlCorrente().getUrl();
            zoom = getActiveFragment().getTextZoom();
        }

        final String ultimaBibbia = LaParolaBrowser.getUltimaBibbia();

        fragment.onCreateGoToUrl = goToUrl;
        fragment.onCreateViewRunnable = self -> {
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
        };

        fragments.add(fragment);
        FragmentTransaction transaction = getSupportFragmentManager().beginTransaction();
        transaction.add(fourPanesLayout.getFrameId(position), fragment, String.valueOf(position));
        transaction.commit();
    }

    //    public void onZoomChanged(LaParolaFragment fragment, int zoom) {
    public void onZoomChanged(int zoom) {
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

        if (synccolors != null) for (int i = 0; i < fragments.size(); i++)
            fragments.get(i).setSyncColor(synccolors[i]);
        //setActiveFragment(fragment);
    }

    public void setFinishedLoading(boolean value) {
        mFinishedLoading = value;

        if (value) {
            setSupportProgressBarIndeterminateVisibility(false);
            findViewById(R.id.loading).setVisibility(GONE);
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
        if (getPanesNumber() == 1) return;

        (Toast.makeText(this, R.string.select_panel, Toast.LENGTH_LONG)).show();
        mSwitchingPanels = getActiveFragment();
    }

    public void selectPanelForOpening(String url) {
        selectPanelForOpening(getActiveFragment().nuovoUrl(url));
    }

    public void selectPanelForOpening(LaParolaUrl url) {
        if (getPanesNumber() == 1) {
            LaParolaFragment f = getActiveFragment();
            f.vaiAdUrl(url);
        } else {
            (Toast.makeText(this, R.string.select_panel, Toast.LENGTH_LONG)).show();
            mSelectingPanelForOpeningUrl = url;
        }
    }

    public void onFragmentTouch(LaParolaFragment mFragment) {
        setActiveFragment(mFragment);

        if (mSelectingPanelForOpeningUrl != null) {
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
        if (isFinishing()) return;

        PanelsDialog diag = new PanelsDialog(this);
        diag.show();
    }

    @Override
    public void mostraEliminaPreferito(final LaParolaUrl nuovoUrl) {
        MessageDialog m = new MessageDialog(this, 0, R.string.delete_starred);
        m.setYesNo(R.string.delete, android.R.string.cancel, () -> {
            LaParolaBrowser.rimuoviPreferito(nuovoUrl);
            LaParolaBrowser.salvaPreferitiSuFile();
            LaParolaFragment fragment = getActiveFragment();
            if (fragment != null) {
                fragment.aggiornaPagina();
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
        if (!mLeftDrawerLayout.isDrawerOpen(GravityCompat.START)) {
            LaParolaFragment af = getActiveFragment();
            if (af != null) {
                LaParolaUrl url = af.getUrlCorrente();
                if (url != null && !af.inHome()) {
                    setTitle(url.getDescrizione());
                    return;
                }
            }
        }
        setTitle(R.string.app_name);
    }

    public int getPanelsOrientation() {
        return fourPanesLayout.getOrientation() == FourPanesLayout.HORIZONTAL ? FourPanesLayout.VERTICAL : FourPanesLayout.HORIZONTAL;
    }

    public void showAccessibilityDialog() {
        if (isFinishing()) return;

        AccessibilityDialog dialog = new AccessibilityDialog();
        dialog.show(getSupportFragmentManager(), "accessibility_dialog");
    }

    public void showSearch() {
        searchActionItemManager.expandActionView();
    }

    public void showPanelBottomSheet() {
        BottomSheetDialog bottomSheetDialog = new BottomSheetDialog(this);

        LayoutInflater inflater = LayoutInflater.from(bottomSheetDialog.getContext());
        View sheetView = inflater.inflate(R.layout.bottom_sheet_panel_menu, null, false);
        bottomSheetDialog.setContentView(sheetView);

        // Hide unused options based on panel count
        int paneCount = getPanesNumber();

        TextView closePanel = sheetView.findViewById(R.id.menu_close_panel);
        TextView duplicatePanel = sheetView.findViewById(R.id.menu_duplicate_panel);
        TextView switchPanel = sheetView.findViewById(R.id.menu_switch_panel);
        TextView managePanels = sheetView.findViewById(R.id.menu_manage_panels);

        if (paneCount <= 1) closePanel.setVisibility(GONE);
        if (paneCount >= LaParolaActivity.MAX_PANELS) duplicatePanel.setVisibility(GONE);
        if (paneCount <= 1) switchPanel.setVisibility(GONE);

        closePanel.setOnClickListener(v -> {
            closeActivePanel();
            bottomSheetDialog.dismiss();
        });

        duplicatePanel.setOnClickListener(v -> {
            openInNewPanel(getActiveFragment().getUrlCorrente());
            bottomSheetDialog.dismiss();
        });

        switchPanel.setOnClickListener(v -> {
            switchPanels();
            bottomSheetDialog.dismiss();
        });

        managePanels.setOnClickListener(v -> {
            showPanelsManagment();
            bottomSheetDialog.dismiss();
        });

// Force expand
        sheetView.post(() -> {
            View bottomSheetInternal = bottomSheetDialog.findViewById(com.google.android.material.R.id.design_bottom_sheet);
            if (bottomSheetInternal != null) {
                BottomSheetBehavior<View> behavior = BottomSheetBehavior.from(bottomSheetInternal);
                behavior.setState(BottomSheetBehavior.STATE_EXPANDED);
            }
        });

        bottomSheetDialog.show();
    }
}
