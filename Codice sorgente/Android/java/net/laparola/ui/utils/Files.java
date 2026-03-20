package net.laparola.ui.utils;

import android.util.Log;

import net.laparola.ui.android.LaParolaPreferences;

import java.io.BufferedInputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.nio.channels.FileChannel;

public class Files {
    private Files() {}

    public static void delete (String fname) {
        File file = new File(fname);
        if (!file.exists()) {
            return;
        }

        try {
            boolean res = file.delete();
            if (!res) {
                //Log.d("laparola", "impossibile eliminare " + fname);
            }
        } catch (SecurityException e) {
            //Log.d("laparola", "security exception eliminando " + fname);
        }
    }

    public static boolean fileIsSame (String f1, String f2) {
        String cf1, cf2;
        try {
            cf1 = new File(f1).getCanonicalPath();   // dovrebbe gestire soft e hard link
            cf2 = new File(f2).getCanonicalPath();
        } catch (Exception e) {
            return false;
        }
        return cf1.equals(cf2);
    }

    public static boolean fileIsEqualToInternalStorage(String filename) {
        //Log.d("LaParola", "is same as internal? " + filename);
        File f = new File(filename);

        if (!f.exists()) {
            // Log.d("LaParola", "no, does not exits");
            return false;
        }

        if (f.getAbsolutePath().equals(LaParolaPreferences.internalStoragePath + "/" + f.getName())) {
            // lo è, quindi va caricato
            return false;
        }

        String absPath;
        try {
            absPath = f.getCanonicalPath();   // dovrebbe gestire soft e hard link
        } catch (Exception e) {
            // Log.d("LaParola", "no, error getting canonical path");
            return false;
        }

        String fname = f.getName();
        String internalPath = LaParolaPreferences.internalStoragePath + "/" + fname;

        String internalAbsPath;
        try {
            internalAbsPath = new File(internalPath).getCanonicalPath();   // dovrebbe gestire soft e hard link
        } catch (Exception e) {
            //Log.d("LaParola", "no, error getting internal canonical path");
            return false;
        }

        if (absPath.equals(internalAbsPath)) {
            //Log.d("LaParola", "no, it IS internal");
            return true;
        }

        File ifile = new File(internalPath);

        if (!ifile.exists()) {
            // Log.d("LaParola", "no, no internal file");
            return false;
        }

        if (f.length() != ifile.length()) {
            // Log.d("LaParola", String.format("no, different length %d, %d", f.length(), ifile.length()));
            return false;
        }

        boolean ret = compareContents(filename, internalPath);
        return ret;
    }

    public static boolean compareContents(String p1, String p2) {
        FileInputStream fis1 = null, fis2 = null;

        try {
            fis1 = new FileInputStream(new File(p1));
            fis2 = new FileInputStream(new File(p2));
            byte[] buf1 = new byte[1024];
            byte[] buf2 = new byte[1024];

            while (true) {
                int n1 = fis1.read(buf1);
                int n2 = fis2.read(buf2);

                if (n1 != n2) {return false;}
                if (n1 == -1) {
                    return true;
                }

                for (int i = 0; i < n1; i++) {
                    if (buf1[i] != buf2[i]) {
                        return false;
                    }
                }
            }
        } catch (Exception e) {
            if (! (e instanceof FileNotFoundException)) {
                e.printStackTrace();
            }
            return false;
        } finally {
            if (fis1 != null) {
                try {
                    fis1.close();
                } catch (Exception e) {}
            }
            if (fis2 != null) {
                try {
                    fis2.close();
                } catch (Exception e) {}
            }
        }
    }

    public static String readAllFile(String path) {
        try {
            File f = new File(path);
            BufferedInputStream bis = new BufferedInputStream(new FileInputStream(f));
            byte[] buf = new byte[(int)f.length()];
            bis.read(buf);
            bis.close();
            return new String(buf);
        } catch (Exception e) {
            if (! (e instanceof FileNotFoundException)) {
                e.printStackTrace();
            }
            return null;
        }
    }

    //@SuppressWarnings("resource")
    public static void copyFileIfExists(String sourceFileName, String destFileName) throws IOException {
        if (sourceFileName == destFileName) {
            return;
        }

        File sourceFile = new File(sourceFileName);
        File destFile = new File(destFileName);

        if (!sourceFile.exists()) {
            return;
        }

        if(!destFile.exists()) {
            destFile.createNewFile();
        }

        FileChannel source = null;
        FileChannel destination = null;
        try {
            source = new FileInputStream(sourceFile).getChannel();
            destination = new FileOutputStream(destFile).getChannel();

            // previous code: destination.transferFrom(source, 0, source.size());
            // to avoid infinite loops, should be:
            long count = 0;
            long size = source.size();
            while((count += destination.transferFrom(source, count, size-count))<size);
        }
        finally {
            if(source != null) {
                source.close();
            }
            if(destination != null) {
                destination.close();
            }
        }
    }
}
