package net.laparola.ui.android;

import android.content.Context;
import android.content.SharedPreferences;
import android.os.Environment;

import net.laparola.ui.LaParolaUrl;

import java.io.File;
import java.util.ArrayList;

public class LaParolaPreferences {
    public static final String LAPAROLA_PREFERENCES = "net.laparola";

    private static SharedPreferences sharedPreferences;

    public static int fragmentsNumber;
    public static int fragmentsOrientation;
    public static boolean homeAtStart;
    public static boolean paragraphOrVerses;
    public static boolean showTitles;
    public static int referencePlacement;
    public static int referenceType;
    public static boolean referenceSuperscript;
    public static boolean useVolumeKeys;
    public static boolean nightMode;
    public static boolean keepScreenOn;
    public static String lastBible;
    public static String[] readStoragePaths;
    public static String writeStoragePath;
    public static String internalStoragePath;
    public static String highlighColor;
    public static boolean useLzma;
    public static int ttsPitch;
    public static int ttsSpeed;
    public static boolean ttsStopEndChapter;
    public static boolean ttsFollowVerse;
    public static int[] syncColor;
    public static String[] lastVersion;
    public static String[] lastUrl;
    public static int[] textZoom;
    public static boolean accessibilityMode;
    public static boolean fontPredefinito;
    public static boolean autoOpenRef;
    public static boolean oneHandZoom;
    public static boolean menuZoom;
    public static boolean swipeChapters;

    public static void load(Context c) {
        if (c != null && sharedPreferences == null) {
            sharedPreferences = c.getSharedPreferences(LAPAROLA_PREFERENCES, Context.MODE_PRIVATE);
        }

        if (sharedPreferences == null)
            return;

        fragmentsNumber = sharedPreferences.getInt("fragmentsNumber", 1);
        fragmentsOrientation = sharedPreferences.getInt("fragmentsOrientation", FourPanesLayout.HORIZONTAL);
        homeAtStart = sharedPreferences.getBoolean("homeAtStart", false);

        paragraphOrVerses = sharedPreferences.getBoolean("paragraphOrVerses", true);
        showTitles = sharedPreferences.getBoolean("showTitles", true);
        referencePlacement = Integer.parseInt(sharedPreferences.getString("referencePlacement", "1"));
        referenceType = Integer.parseInt(sharedPreferences.getString("referenceType", "0"));
        referenceSuperscript = sharedPreferences.getBoolean("referenceSuperscript", true);

        useVolumeKeys = sharedPreferences.getBoolean("useVolumeKeys", true);
        swipeChapters = sharedPreferences.getBoolean("swipeChapters", true);
        keepScreenOn = sharedPreferences.getBoolean("keepScreenOn", false);
        nightMode = sharedPreferences.getBoolean("nightMode", false);
        accessibilityMode = sharedPreferences.getBoolean("accessibilityMode", false);
        fontPredefinito = sharedPreferences.getBoolean("fontPredefinito", false);
        autoOpenRef = sharedPreferences.getBoolean("autoOpenRef", false);
        oneHandZoom = sharedPreferences.getBoolean("oneHandZoom", true);
        menuZoom = sharedPreferences.getBoolean("menuZoom", false);

        lastBible = sharedPreferences.getString("lastBible", "");
        highlighColor = sharedPreferences.getString("highlighColor", "yellow");
        useLzma = sharedPreferences.getBoolean("useLzma", false);

        ttsPitch = sharedPreferences.getInt("ttsPitch", 3);
        ttsSpeed = sharedPreferences.getInt("ttsSpeed", 3);
        ttsStopEndChapter = sharedPreferences.getBoolean("ttsStopEndChapter", false);
        ttsFollowVerse = sharedPreferences.getBoolean("ttsFollowVerse", false);

        resetStoragePath(c);

        syncColor = new int[LaParolaActivity.MAX_PANELS];
        lastVersion = new String[LaParolaActivity.MAX_PANELS];
        lastUrl = new String[LaParolaActivity.MAX_PANELS];
        textZoom = new int[LaParolaActivity.MAX_PANELS];
        for (int i = 0; i < LaParolaActivity.MAX_PANELS; i++) {
            String idx = i == 0 ? "" : String.valueOf(i);

            syncColor[i] = sharedPreferences.getInt("syncColor" + idx, 0);
            lastVersion[i] = sharedPreferences.getString("lastVersion" + idx, "");
            lastUrl[i] = sharedPreferences.getString("lastUrl" + idx, null);
            textZoom[i] = sharedPreferences.getInt("textZoom" + idx, 100);
        }
    }

    public static void save(LaParolaActivity laParolaActivity) {
        if (sharedPreferences == null)
            return;

        SharedPreferences.Editor editor = sharedPreferences.edit();

        editor.putString("lastBible", lastBible);
        editor.putInt("fragmentsNumber", laParolaActivity.getPanesNumber());
        editor.putInt("fragmentsOrientation", laParolaActivity.getPanelsOrientation());

        for (int i = 0; i < laParolaActivity.fragments.size(); i++) {
            String idx = i == 0 ? "" : String.valueOf(i);
            LaParolaFragment f = laParolaActivity.fragments.get(i);

            if (f.isCreated()) {
                editor.putInt("textZoom" + idx, f.getTextZoom());
                editor.putInt("syncColor" + idx, f.getSyncColor());
                LaParolaUrl urlCorrente = f.getUrlCorrente();
                if (urlCorrente != null) {
                    editor.putString("lastVersion" + idx, f.getVersione());
                    editor.putString("lastUrl" + idx, urlCorrente.getUrl());
                }
            }
        }

        LaParolaFragment activeFragment = laParolaActivity.getActiveFragment();
        if (activeFragment != null && activeFragment.isCreated())
            editor.putString("lastVersion", activeFragment.getVersione());

        editor.putBoolean("nightMode", nightMode);
        editor.putBoolean("accessibilityMode", accessibilityMode);
        editor.putBoolean("fontPredefinito", fontPredefinito);
        editor.putBoolean("autoOpenRef", autoOpenRef);
        editor.putBoolean("oneHandZoom", oneHandZoom);
        editor.putBoolean("menuZoom", menuZoom);
        editor.putString("storagePath", writeStoragePath);
        editor.putString("highlighColor", highlighColor);
        editor.putBoolean("swipeChapters", swipeChapters);

        editor.putInt("ttsPitch", ttsPitch);
        editor.putInt("ttsSpeed", ttsSpeed);
        editor.putBoolean("ttsStopEndChapter", ttsStopEndChapter);
        editor.putBoolean("ttsFollowVerse", ttsFollowVerse);

        editor.apply();

        LaParolaBackupAgent.dataChanged(laParolaActivity.getPackageName());


    }

    public static boolean getHomeOption(String key, boolean def) {
        if (sharedPreferences == null)
            return def;

        return sharedPreferences.getBoolean(key, def);
    }

    public static void resetStoragePath(Context c) {
        Context appContext = c.getApplicationContext();
        ArrayList<String> paths = new ArrayList<>();

        File externalFilesDir = appContext.getExternalFilesDir(null);
        if (externalFilesDir != null) {
            paths.add(externalFilesDir.getAbsolutePath());
        }

        internalStoragePath = appContext.getFilesDir().getAbsolutePath();
        paths.add(internalStoragePath);

        String p = Environment.getExternalStorageDirectory().getPath() + "/laparola";
        (new File(p)).mkdirs();
        paths.add(p);

        readStoragePaths = new String[paths.size()];
        paths.toArray(readStoragePaths);

        for (String path : paths) {
            if (LaParolaActivityInitUtility.checkStoragePath(path)) {
                writeStoragePath = path;
/*
                if (false) {   // test
                    writeStoragePath = writeStoragePath + "/test";
                    (new File(writeStoragePath)).mkdirs();

                    paths.add(writeStoragePath);
                    readStoragePaths = new String[paths.size()];
                    paths.toArray(readStoragePaths);

                    return;
                }
 */

                return;
            }
        }

        // Qui non dovrebbe arrivare
    }
}
