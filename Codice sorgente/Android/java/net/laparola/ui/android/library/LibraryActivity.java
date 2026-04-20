package net.laparola.ui.android.library;

import net.laparola.R;
import net.laparola.core.ComponenteInformazioni;
import net.laparola.core.Testi;
import net.laparola.ui.LaParolaBrowser;
import net.laparola.ui.android.LaParolaActivityInitUtility;
import net.laparola.ui.android.LaParolaPreferences;
import net.laparola.ui.android.dialogs.LaParolaDialog;
import net.laparola.ui.android.dialogs.MessageDialog;
import net.laparola.ui.android.library.downloadmanager.LibraryDownloaderService;

import android.Manifest;
import android.content.BroadcastReceiver;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.ServiceConnection;
import android.content.SharedPreferences;
import android.content.pm.PackageManager;
import android.os.Build;
import android.os.Bundle;
import android.os.IBinder;
import android.view.MenuItem;
import android.view.View;

import com.google.android.material.appbar.MaterialToolbar;
import com.google.android.material.progressindicator.LinearProgressIndicator;
import com.google.android.material.tabs.TabLayout;
import com.google.android.material.tabs.TabLayoutMediator;

import org.xml.sax.SAXParseException;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Collections;
import java.util.EnumSet;
import java.util.List;
import java.util.Set;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.Future;

import androidx.activity.result.ActivityResultLauncher;
import androidx.activity.result.contract.ActivityResultContracts;
import androidx.appcompat.app.ActionBar;
import androidx.appcompat.app.AlertDialog;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.app.ActivityCompat;
import androidx.core.content.ContextCompat;
import androidx.viewpager2.widget.ViewPager2;
import timber.log.Timber;

public class LibraryActivity extends AppCompatActivity {
    private ViewPager2 mPager;
    private final ExecutorService mExecutor = Executors.newSingleThreadExecutor();
    private Future<?> mUpdateFuture;

    private LibraryDownloaderService mDownloader;
    private int mInstalledBibleCount = 0;
    private LibraryFragmentPager mLibraryFragmentPager;
    private LinearProgressIndicator mProgressBar;

    private final ServiceConnection mConnection = new ServiceConnection() {
        public void onServiceConnected(ComponentName className, IBinder service) {
            // This is called when the connection with the service has been
            // established, giving us the service object we can use to
            // interact with the service. Because we have bound to an explicit
            // service that we know is running in our own process, we can
            // cast its IBinder to a concrete class and directly access it.
            mDownloader = ((LibraryDownloaderService.LocalBinder) service).getService();

            // REFRESH the UI now that the service is actually ready
            // This makes sure the "isDownloading" checks run again with the valid service
            refreshLibrary(false);
        }

        public void onServiceDisconnected(ComponentName className) {
            mDownloader = null;
        }
    };

    private final BroadcastReceiver progressReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            if (LibraryDownloaderService.DOWNLOAD_PROGRESS.equals(intent.getAction())) {
                int progress = intent.getIntExtra(LibraryDownloaderService.EXTRA_PROGRESS, 0);

                if (progress >= 0) {
                    // Switch progress bar to DETERMINATE mode
                    mProgressBar.setIndeterminate(false);
                    mProgressBar.setProgress(progress);
                    mProgressBar.setVisibility(View.VISIBLE);
                }
            } else if (LibraryDownloaderService.DOWNLOAD_FINISHED.equals(intent.getAction())) {
                // Hide progress bar when done
                mProgressBar.setVisibility(View.GONE);
                refreshLibrary(false);
            }
        }
    };

    // Register the permission callback
    private final ActivityResultLauncher<String> requestPermissionLauncher =
            registerForActivityResult(new ActivityResultContracts.RequestPermission(), isGranted -> {
                if (isGranted) {
                    // Permission granted! Notifications will now show up.
                    Timber.d("Notification permission granted.");
                } else {
                    // Permission denied. Maybe show a toast explaining why
                    // they won't see download progress?
                    Timber.w("Notification permission denied.");
                }
            });

    private void checkNotificationPermission() {
        if (Build.VERSION.SDK_INT < Build.VERSION_CODES.TIRAMISU) return; // 33

        // If already granted, stop.
        if (ContextCompat.checkSelfPermission(this, Manifest.permission.POST_NOTIFICATIONS)
                == PackageManager.PERMISSION_GRANTED) {
            return;
        }

        SharedPreferences prefs = getSharedPreferences("app_prefs", MODE_PRIVATE);
        boolean hasShownRationale = prefs.getBoolean("pref_notif_rationale_shown", false);
        boolean hasAskedOnce = prefs.getBoolean("pref_notif_asked_once", false);

        if (ActivityCompat.shouldShowRequestPermissionRationale(this, Manifest.permission.POST_NOTIFICATIONS)) {
            // User denied once. Only show our rationale dialog IF we haven't shown it yet.
            if (!hasShownRationale) {
                new AlertDialog.Builder(this, R.style.Theme_LaParola_Dialog)
                        .setTitle(R.string.notifiche)
                        .setMessage(R.string.notifiche_richiesta)
                        .setPositiveButton(R.string.ok, (dialog, which) -> {
                            // Mark rationale as shown so we don't nag again
                            prefs.edit().putBoolean("pref_notif_rationale_shown", true).apply();
                            requestPermissionLauncher.launch(Manifest.permission.POST_NOTIFICATIONS);
                        })
                        .setNegativeButton(R.string.no, (dialog, which) -> {
                            // User said "No" to the rationale. Mark it shown so we stop asking.
                            prefs.edit().putBoolean("pref_notif_rationale_shown", true).apply();
                            dialog.dismiss();
                        })
                        .create()
                        .show();
            }
        } else {
            // First time asking (or user blocked it).
            // Only launch the system popup if we've never asked at all.
            if (!hasAskedOnce) {
                prefs.edit().putBoolean("pref_notif_asked_once", true).apply();
                requestPermissionLauncher.launch(Manifest.permission.POST_NOTIFICATIONS);
            }
        }
    }

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.components_activity);

        mProgressBar = findViewById(R.id.library_progress_bar);

        MaterialToolbar toolbar = findViewById(R.id.library_toolbar);
        setSupportActionBar(toolbar);

        ActionBar ab = getSupportActionBar();
        if (ab != null) {
            ab.setDisplayHomeAsUpEnabled(true);
            ab.setDisplayShowHomeEnabled(true);
            ab.setLogo(R.drawable.ic_launcher);  // show app icon
            ab.setDisplayUseLogoEnabled(true);
            ab.setTitle(R.string.version_management);
        }

        toolbar.setNavigationOnClickListener(v -> finish());

        mPager = findViewById(R.id.pager);
        //mLibraryFragmentPager = new LibraryFragmentPager(getSupportFragmentManager(), this);
        mLibraryFragmentPager = new LibraryFragmentPager(this, this);
        mPager.setAdapter(mLibraryFragmentPager);
        TabLayout mTabLayout = findViewById(R.id.indicator);

        new TabLayoutMediator(mTabLayout, mPager,
                (tab, position) -> {
                    switch (position) {
                        case 0:
                            tab.setText(this.getString(R.string.type_bible));
                            break;
                        case 1:
                            tab.setText(this.getString(R.string.type_commentario));
                            break;
                        //rmw1024 case 2: tab.setText(this.getString(R.string.type_dictionary)); break;
                        default:
                            tab.setText(this.getString(R.string.type_bible));
                            break;
                    }
                }
        ).attach();

        checkNotificationPermission();

        LaParolaPreferences.load(this);
        bindService(new Intent(this, LibraryDownloaderService.class), mConnection, Context.BIND_AUTO_CREATE);
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        // 3. Essential: Shutdown the executor to prevent memory leaks
        mExecutor.shutdownNow();

        if (mDownloader != null) {
            unbindService(mConnection);
            mDownloader = null;
        }
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        if (item.getItemId() == android.R.id.home) {
            finish();
            return true;
        }

        return super.onOptionsItemSelected(item);
    }

    @Override
    protected void onPause() {
        super.onPause();
        // Unregister the progress receiver
        try {
            unregisterReceiver(progressReceiver);
        } catch (IllegalArgumentException e) {
            // Already unregistered
        }

        if (mUpdateFuture != null && !mUpdateFuture.isDone()) {
            mUpdateFuture.cancel(true);
        }
    }

    @Override
    protected void onResume() {
        super.onResume();

        // Register progressReceiver for both intents
        IntentFilter filter = new IntentFilter();
        filter.addAction(LibraryDownloaderService.DOWNLOAD_FINISHED);
        filter.addAction(LibraryDownloaderService.DOWNLOAD_PROGRESS);

        ContextCompat.registerReceiver(
                this,
                progressReceiver,
                filter,
                ContextCompat.RECEIVER_NOT_EXPORTED
        );

        if (mDownloader != null) {
            refreshLibrary(false);
        }
    }

    public void refreshLibrary(boolean clean) {
        if (mUpdateFuture != null && !mUpdateFuture.isDone()) {
            mUpdateFuture.cancel(true);
        }
        performLibraryUpdate(clean);
    }

    public void startDownload(final LibraryItemInfo info) {
        if (mDownloader == null) {
            mPager.postDelayed(() -> LibraryActivity.this.startDownload(info), 100);
            return;
        }
        mDownloader.startDownload(info);
    }

    public int getInstalledBibleCount() {
        return mInstalledBibleCount;
    }

    public void setAdapters(LibraryAdapter bibbiaAdapter, LibraryAdapter commentariAdapter, LibraryAdapter dizionariAdapter) {
        mInstalledBibleCount = bibbiaAdapter.getInstalledBibleCount();
        mLibraryFragmentPager.setAdapters(bibbiaAdapter, commentariAdapter, dizionariAdapter);
    }

    public boolean isDownloading(LibraryItemInfo item) {
        if (mDownloader == null) {
            return false;
        }
        return mDownloader.isDownloading(item);
    }

    private void performLibraryUpdate(boolean clean) {
        // 1. Pre-Execute: Show progress (Run on UI Thread)
        setProgressVisible(true);

        mUpdateFuture = mExecutor.submit(() -> {
            int errorMessage = 0;
            List<ComponenteInformazioni> components = null;
            String storagePath = LaParolaPreferences.writeStoragePath;

            // 2. Background Work: Fetch Data
            try {
                if (clean) {
                    LaParolaActivityInitUtility.aggiungiTesti(storagePath, LibraryActivity.this);
                }
                components = LaParolaBrowser.getTestiDisponibili(storagePath + "/aggiorna.xml.cache");
            } catch (IOException | SAXParseException e) {
                Timber.e(e, "IO error getting available texts.");
                errorMessage = R.string.cannot_download_updates;
            } catch (Exception e) {
                Timber.e(e, "Error parsing updates.");
                errorMessage = R.string.error_parsing_updates;
            }

            if (components == null) {
                components = LaParolaBrowser.getTestiInstallati();
            }

            // 3. Background Work: Categorize and Sort (Keep UI thread free!)
            final List<LibraryItemInfo> bibbie = new ArrayList<>();
            final List<LibraryItemInfo> commentari = new ArrayList<>();
            final List<LibraryItemInfo> dizionari = new ArrayList<>();

            if (components != null) {
                for (ComponenteInformazioni info : components) {
                    LibraryItemInfo li = new LibraryItemInfo(LibraryActivity.this, info);
                    EnumSet<Testi.TestoTipi> tipo = li.getTipo();

                    if (tipo.contains(Testi.TestoTipi.COMMENTARIO)) {
                        commentari.add(li);
                    } else if (tipo.contains(Testi.TestoTipi.DIZIONARIO)) {
                        dizionari.add(li);
                    } else {
                        bibbie.add(li);
                    }
                }

                // Add broken files
                Set<String> broken = LaParolaBrowser.getFileIllegibili();
                for (String b : broken) {
                    bibbie.add(new LibraryItemInfo(LibraryActivity.this, b));
                }

                Collections.sort(bibbie);
                Collections.sort(commentari);
                Collections.sort(dizionari);
            }

            // 4. Post-Execute: Update UI
            final int finalError = errorMessage;
            final List<ComponenteInformazioni> finalResult = components;

            runOnUiThread(() -> {
                if (isFinishing()) return;

                setProgressVisible(false);

                // Create adapters and update UI
                LibraryAdapter bibbieAdapter = new LibraryAdapter(LibraryActivity.this, LibraryActivity.this, bibbie);
                LibraryAdapter commentariAdapter = new LibraryAdapter(LibraryActivity.this, LibraryActivity.this, commentari);
                LibraryAdapter dizionariAdapter = new LibraryAdapter(LibraryActivity.this, LibraryActivity.this, dizionari);

                setAdapters(bibbieAdapter, commentariAdapter, dizionariAdapter);

                // Handle Errors
                if (finalError != 0) {
                    showErrorDialog(finalError, finalResult == null || finalResult.isEmpty());
                }
            });
        });
    }

    private void setProgressVisible(boolean visible) {
        if (mProgressBar != null) {
            mProgressBar.setVisibility(visible ? View.VISIBLE : View.GONE);
        }
    }

    private void showErrorDialog(int messageRes, boolean finishOnDismiss) {
        LaParolaDialog messageDialog = new MessageDialog(this, R.string.error, messageRes);
        if (finishOnDismiss) {
            messageDialog.setOnDismissListener(dialog -> finish());
        }
        messageDialog.show();
    }
}
