package net.laparola.ui.android;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.Intent;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager.NameNotFoundException;
import android.os.Environment;
import android.view.View;
import android.webkit.WebSettings;
import android.webkit.WebView;
import android.widget.FrameLayout;
import android.widget.LinearLayout;

import net.laparola.R;
import net.laparola.core.VersioneInformazioni;
import net.laparola.ui.LaParolaBrowser;
import net.laparola.ui.LaParolaEvidenziatore;
import net.laparola.ui.android.dialogs.LaParolaDialog;
import net.laparola.ui.android.dialogs.MessageDialog;
import net.laparola.ui.android.library.LibraryActivity;
import net.laparola.ui.utils.Files;
import net.laparola.ui.utils.LZMAFile;
import net.laparola.ui.utils.lzma_java.LZMADecoder;

import java.io.BufferedInputStream;
import java.io.BufferedOutputStream;
import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;
import java.util.Locale;
import java.util.Scanner;

import timber.log.Timber;

public class LaParolaActivityInitUtility {
    private final LaParolaActivity parent;
    private WebView messageView;
    private boolean mIsWorking;

    public LaParolaActivityInitUtility(LaParolaActivity parent) {
        this.parent = parent;
        mIsWorking = true;
    }

    @SuppressLint("SetJavaScriptEnabled")
    private boolean checkBibleInstalled(final String path) {
        final String nomeVersioneInRisorse = "Nuova Riveduta";
        int v1 = 1;
        int v2 = 1;
        int v3 = 11;

        VersioneInformazioni ivi = LaParolaBrowser.getInformazioniTesto(nomeVersioneInRisorse);
        boolean installataAggiornabile = (ivi != null && (
                (v1 > ivi.getVersione1()) ||
                        (v1 == ivi.getVersione1() && v2 > ivi.getVersione2()) ||
                        (v1 == ivi.getVersione1() && v2 == ivi.getVersione2() && v3 > ivi.getVersione3())));

        if (installataAggiornabile) {
            LaParolaBrowser.cancellaTesto(nomeVersioneInRisorse, ivi.getNomeDelFile());
        }

        // installataAggiornabile = true;

        if (installataAggiornabile || LaParolaBrowser.getNomiVersioni().length == 0) {
            parent.runOnUiThread(new Runnable() {
                @SuppressLint("InlinedApi")
                @Override
                public void run() {
                    messageView = new WebView(parent);
                    parent.findViewById(R.id.loading).setVisibility(View.GONE);
                    FrameLayout activityLayout = parent.findViewById(R.id.main_activity_container);
                    activityLayout.addView(messageView, 0, new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MATCH_PARENT, LinearLayout.LayoutParams.MATCH_PARENT));
                    WebSettings mSettings = messageView.getSettings();
                    mSettings.setJavaScriptEnabled(true);
                    mSettings.setBuiltInZoomControls(false);
                    messageView.loadUrl("file:///android_asset/preparazione.html");
                }
            });

            Thread resourceCopyingThread = new Thread(() -> {
                String nomeRisorsa = nomeVersioneInRisorse.toLowerCase(Locale.ENGLISH).replace(' ', '_');
                String nomeFile = path + "/" + nomeVersioneInRisorse + ".lpj";

                copyBibleFromResource(nomeRisorsa, nomeFile);

                parent.runOnUiThread(() -> checkBibleFromResourceCopied(path));
            });

            resourceCopyingThread.start();

            return false;
        } else {
            return true;
        }
    }

    private void checkBibleFromResourceCopied(final String path) {
        LaParolaBrowser.aggiungiTestiDaDirectory(path);
        FrameLayout activityLayout = parent.findViewById(R.id.main_activity_container);
        activityLayout.removeView(messageView);
        parent.findViewById(R.id.loading).setVisibility(View.VISIBLE);
        messageView = null;

        mIsWorking = false;

        if (LaParolaBrowser.getNomiVersioni().length == 0) {
            parent.runOnUiThread(() -> {
                if (!parent.isFinishing()) {
                    LaParolaDialog d = new MessageDialog(parent, R.string.error, R.string.no_bible_installed);
                    d.setOnDismissListener(dialog -> parent.startActivity(new Intent(parent, LibraryActivity.class)));
                    d.show();
                }
            });
        }
    }

    public static boolean checkStoragePath(String storagePath) {
        File f = new File(storagePath + "/tmp");
        f.delete();
        (new File(storagePath)).mkdirs();
        try {
            f.createNewFile();
        } catch (IOException e) {
            return false;
        }
        f.delete();
        return true;
    }

    private void copyBibleFromResource(String resourceName, String fileName) {
        // È necessario usare questo metodo perchè gli asset non permettono l'accesso casuale e
        // prima di android 2.3 non possono superare quando decompressi dall'apk la dimensione di 1 MB.
        // Però l'eccezione è per alcuni tipi di risorse (con certi estensioni) che non vengono compressi dall'apk.
        // Quindi noi dobbiamo comprimere la risorsa con zip (altrimenti il file apk è troppo grande),
        // e poi dare un'estensione come mp3 affinchè non venga compresa dentro l'apk.
        // Vedi per esempio http://ponystyle.com/blog/2010/03/26/dealing-with-asset-compression-in-android-apps/

        // così può essere compilato anche togliendo il file da raw
        //int id = parent.getResources().getIdentifier(resourceName, "raw", parent.getApplicationInfo().packageName);
        //if (id == 0) {
        //	return;
        //}
        int id = R.raw.nuova_riveduta; // safer, faster, checked by compiler

        File file = new File(fileName);
        file.mkdirs();
        file.delete(); // makedirs crea una cartella chiamata nomefile

        final int BUFFER = 8192;
        BufferedInputStream src = new BufferedInputStream(parent.getResources().openRawResource(id), BUFFER);
        BufferedOutputStream dest;
        try {
            FileOutputStream openFileOutput = new FileOutputStream(file);
            dest = new BufferedOutputStream(openFileOutput, BUFFER);
        } catch (Exception e) {
            return;
        }

        // ZipInputStream zis = null;
        try {
            /*
             * zis = new ZipInputStream(src); zis.getNextEntry(); int count; byte data[] = new byte[BUFFER]; while ((count = zis.read(data, 0, BUFFER)) != -1) dest.write(data, 0,
             * count);
             */
            LZMAFile.decomprimi(src, dest, new LZMADecoder.ProgressRunnable() {
                int lastPercent = 0;
                long lastPercentTime = 0;

                @Override
                public void publish(long progress, long size) {
                    final int percent = (int) Math.round((double) progress / size * 100);
                    long milliTime = System.nanoTime() / 1000000;
                    if (lastPercent != percent && milliTime > lastPercentTime + 1000) {
                        parent.runOnUiThread(() -> {
                            String command = "publishProgress(" + percent + ");";
                            messageView.loadUrl("javascript:(function() {" + command + "})()");
                        });
                        lastPercent = percent;
                        lastPercentTime = milliTime;
                    }
                }
            });
        } catch (Exception e) {
            file.delete();

            if (!parent.isFinishing()) {
                parent.runOnUiThread(() -> {
                    MessageDialog d = new MessageDialog(parent, R.string.error, R.string.error_decompressing_resource);
                    d.show();
                });
            }
        } finally {
            try {
                // if (dest != null) {
                dest.flush();
                dest.close();
                // }
            } catch (Exception e) {
                //
            }
            /*
             * try { if (zis != null) zis.close(); } catch (Exception e) {}
             */
        }
    }

    public boolean isWorking() {
        return mIsWorking;
    }

    public void init() {
        new Thread(this::initThread).start();
    }

    private void initThread() {
        int v1, v2;
        try {
            PackageInfo manager = parent.getPackageManager().getPackageInfo(parent.getPackageName(), 0);
            String[] p = manager.versionName.split("\\.");
            v1 = Integer.parseInt(p[0]);
            v2 = Integer.parseInt(p[1]);
        } catch (NameNotFoundException e) {
            v1 = 1;
            v2 = 0;
        }
        DeviceUuidFactory duf = new DeviceUuidFactory(parent.getApplicationContext());
        LaParolaBrowser.inizializza(duf.getDeviceUuid(), v1, v2);
        LaParolaBrowser.setLaParolaBrowserStaticClient(parent);

        if (!checkStoragePath(LaParolaPreferences.writeStoragePath)) {
            if (!parent.isFinishing()) {
                parent.runOnUiThread(() -> {
                    LaParolaDialog d = new MessageDialog(parent, R.string.error, R.string.storage_error);
                    d.setOnDismissListener(dialog -> parent.finish());
                    d.show();
                });
            }
            return;
        }

        if (LaParolaBrowser.getGruppiSegnalibri().isEmpty()) {
            caricaFileDati(LaParolaPreferences.readStoragePaths);
        }

        LaParolaBrowser.setNomeFileDebug(LaParolaPreferences.writeStoragePath + "/debug.html");

        // se presenti in altre cartelle, sposta i file in quella corretta
        for (String sp : LaParolaPreferences.readStoragePaths) {
            if (sp.equals(LaParolaPreferences.writeStoragePath)) {
                continue;
            }

            File folder = new File(sp);

            File[] files = folder.listFiles();
            if (files == null) {
                continue;
            }

            for (File f : files) {
                if (!(f.isFile() && f.getPath().toLowerCase().endsWith(".lpj"))) {
                    continue;
                }

                try {
                    f.renameTo(new File(LaParolaPreferences.writeStoragePath + "/" + f.getName()));
                } catch (Exception e) {
                    Timber.e(e, "Unexpected error occurred while renaming Preferences.");
                }
            }
        }
        aggiungiTesti(LaParolaPreferences.writeStoragePath, parent);

        if (checkBibleInstalled(LaParolaPreferences.writeStoragePath)) {
            mIsWorking = false;
        }
    }

    public static void caricaFileDati(String[] storagePaths) {
        LaParolaBrowser.aggiungiSegnalibriDaFile("segnalibri.xml");   // dagli asset

        for (String storagePath : storagePaths) {
            LaParolaBrowser.aggiungiPreferitiDaFile(storagePath + "/preferiti.xml");
            LaParolaBrowser.caricaCronologia(storagePath + "/cronologia");
            LaParolaEvidenziatore.caricaVersettiEvidenziatiDaFile(storagePath + "/evidenziati");
        }

        // riscrive il risultato ed elimina i file in più
        LaParolaBrowser.salvaPreferitiSuFile();
        LaParolaBrowser.salvaCronologia();
        LaParolaEvidenziatore.salvaVersettiEvidenziatiSuFile();

        //Timber.tag("LaParola").d(LaParolaPreferences.writeStoragePath);Timber.tag("LaParola").d(LaParolaPreferences.internalStoragePath);

        for (String storagePath : storagePaths) {
            if (Files.fileIsSame(storagePath, LaParolaPreferences.writeStoragePath)) {
                continue;
            }
            if (Files.fileIsSame(storagePath, LaParolaPreferences.internalStoragePath)) {
                continue;
            }

            Files.delete(storagePath + "/preferiti.xml");
            Files.delete(storagePath + "/cronologia");
            Files.delete(storagePath + "/evidenziati");
        }
    }


    public static void aggiungiTesti(String storagePath, final Activity parent) {
        LaParolaBrowser.pulisciTesti();

        LaParolaBrowser.aggiungiTestiDaDirectory(storagePath);
        ArrayList<String> falliti = new ArrayList<>();
        ArrayList<String> cartelleAggiunte = new ArrayList<>();
        cartelleAggiunte.add(storagePath);

        try {
            File mountFile = new File("/proc/mounts");
            if (mountFile.exists()) {
                Scanner scanner = new Scanner(mountFile);
                while (scanner.hasNext()) {
                    String line = scanner.nextLine();
                    String[] lineElements = line.split(" ");
                    String element = lineElements[1];

                    String path = element + "/laparola";
                    if (new File(path).exists() && !path.equals(storagePath)) {
                        boolean dup = false;
                        for (String c : cartelleAggiunte) {
                            if (stessaCartella(c, path)) {
                                dup = true;
                                break;
                            }
                        }

                        if (dup) {
                            Timber.tag("laparola").d("Duplicato trovato: %s", path);
                        } else {
                            List<String> f = LaParolaBrowser.aggiungiTestiDaDirectory(path);
                            falliti.addAll(f);

                            cartelleAggiunte.add(path);
                            Timber.tag("laparola").d("Aggiunti testi da cartella: %s", path);
                        }
                    }
                }
            }

            String e = Environment.getExternalStorageDirectory().getPath() + "/laparola";
            if (!storagePath.equals(e)) {
                LaParolaBrowser.aggiungiTestiDaDirectory(e);
            }

        } catch (Exception e) {
            Timber.e(e, "Unexpected error occurred while adding text.");
        }

        if (!falliti.isEmpty()) {
            final StringBuilder sb = new StringBuilder();
            for (String f : falliti) {
                sb.append(f);
                sb.append(", ");
            }
            sb.deleteCharAt(sb.length() - 2);
            sb.deleteCharAt(sb.length() - 2);

            Timber.tag("Laparola").d(parent.getResources().getString(R.string.error_book_already_loaded, sb));
			/*
			parent.runOnUiThread(new Runnable() {
				@Override
				public void run() {
					Toast.makeText(
							parent, 
							parent.getResources().getString(R.string.error_book_already_loaded, sb), 
							Toast.LENGTH_LONG).show();
				}
			});
			*/
        }
    }

    private static boolean stessaCartella(String path1, String path2) {
        /* potrebbero essere uguali, ma non sullo stesso mount point */
        /* mi devo inventare qualcosa */

        File file1, file2;
        String tmp;
        int i = 0;
        while (true) {
            tmp = "laparola" + i + ".tmp";
            file1 = new File(path1 + "/" + tmp);
            file2 = new File(path2 + "/" + tmp);
            if (file1.exists() || file2.exists()) {
                i++;
            } else {
                break;
            }
        }

        try {
            file1.createNewFile();
            boolean exists = file2.exists();
            file1.delete();

            if (exists) {
                return true;
            }
        } catch (IOException e) {
            Timber.e(e, "Unexpected IO error occurred while comparing folders.");
        }

        return false;
    }
}