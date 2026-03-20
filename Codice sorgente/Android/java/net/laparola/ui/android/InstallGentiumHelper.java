package net.laparola.ui.android;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.util.zip.ZipEntry;
import java.util.zip.ZipInputStream;

import android.annotation.TargetApi;
import android.app.Activity;
import android.app.DownloadManager;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.database.Cursor;
import android.net.Uri;
import android.os.Build;
import android.os.ParcelFileDescriptor;
import android.util.Log;
import android.widget.Toast;
import net.laparola.R;

@TargetApi(Build.VERSION_CODES.GINGERBREAD)
public class InstallGentiumHelper {
	static final int BUFFER_LENGHT = 16384;
	public static boolean isInstalling = false;

	public static boolean isInstalled (LaParolaActivity mParentActivity) {
		if (Build.VERSION.SDK_INT >= 30) {
			// non c'è bisogno di Gentium da Android 11
			return true;
		}
		String fontPath = getFontPath(mParentActivity);
    	return (new File(fontPath)).exists();
	}
	
	public synchronized static void install(LaParolaActivity mContext) {
		Toast.makeText(mContext, R.string.greek_installing, Toast.LENGTH_LONG).show();
		if (isInstalling)
			return;
		isInstalling = true;
		
		final LaParolaActivity activity = mContext;
		final DownloadManager downloadManager = (DownloadManager)activity.getSystemService(Activity.DOWNLOAD_SERVICE);

		Uri Download_Uri = Uri.parse(mContext.getString(R.string.greek_font_url));
		DownloadManager.Request request = new DownloadManager.Request(Download_Uri);
		final long download_id = downloadManager.enqueue(request);

		BroadcastReceiver downloadReceiver = new BroadcastReceiver() {
			@Override
			public void onReceive(Context arg0, Intent arg1) {
				DownloadManager.Query query = new DownloadManager.Query();
				query.setFilterById(download_id);
				Cursor cursor = downloadManager.query(query);

				if(cursor.moveToFirst()){
					int columnIndex = cursor.getColumnIndex(DownloadManager.COLUMN_STATUS);
					int status = cursor.getInt(columnIndex);
					int columnReason = cursor.getColumnIndex(DownloadManager.COLUMN_REASON);
					int reason = cursor.getInt(columnReason);

					if (status == DownloadManager.STATUS_SUCCESSFUL) {
						//Retrieve the saved download id
						ParcelFileDescriptor file;
						try {
							String fontPath = LaParolaPreferences.writeStoragePath + "/Gentium/";
							
							file = downloadManager.openDownloadedFile(download_id);
							unzip(file, fontPath);
							activity.refreshAll();
						} catch (FileNotFoundException e) {
							e.printStackTrace();
						}
						isInstalling = false;
					} else if (status == DownloadManager.STATUS_FAILED) {
						Log.d("laparola", "Download failed: " + reason);
						isInstalling = false;
					} else if (status == DownloadManager.STATUS_PAUSED) {
					} else if (status == DownloadManager.STATUS_PENDING) {
					} else if (status == DownloadManager.STATUS_RUNNING) {
					}
				}
			}
		};
		
		IntentFilter intentFilter = new IntentFilter(DownloadManager.ACTION_DOWNLOAD_COMPLETE);
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.TIRAMISU) {
            activity.registerReceiver(downloadReceiver, intentFilter, Context.RECEIVER_NOT_EXPORTED);
        }
		else {
			activity.registerReceiver(downloadReceiver, intentFilter);
		}
    }

	public static void unzip(ParcelFileDescriptor zip, String loc) { 
		dirChecker(loc); 
		try  { 
			FileInputStream fin = new ParcelFileDescriptor.AutoCloseInputStream(zip);
			ZipInputStream zin = new ZipInputStream(fin); 
			ZipEntry ze = null; 
			while ((ze = zin.getNextEntry()) != null) { 
				if(ze.isDirectory()) { 
					//dirChecker(loc + ze.getName()); 
				} else { 
					String fname = new File(ze.getName()).getName();
					// String fname = ze.getName()   // comprende la directory 
					FileOutputStream fout = new FileOutputStream(loc + fname); 
					byte buf[] = new byte[BUFFER_LENGHT];
					int len;
					while ((len = zin.read(buf)) > 0) {
						fout.write(buf, 0, len);
					}

					zin.closeEntry(); 
					fout.close(); 
				} 
			} 
			zin.close(); 
		} catch(Exception e) { 
			e.printStackTrace(); 
		} 

	} 

	private static void dirChecker(String dir) { 
		File f = new File(dir); 

		if(!f.isDirectory()) { 
			f.mkdirs(); 
		} 
	}

    public static String getFontPath(LaParolaActivity mParentActivity) {
        String fontPath = LaParolaPreferences.writeStoragePath + "/Gentium/GentiumPlus-R.ttf";
        return fontPath;
    }
    public static String getFontPathItalics(LaParolaActivity mParentActivity) {
        String fontPath = LaParolaPreferences.writeStoragePath + "/Gentium/GentiumPlus-I.ttf";
        return fontPath;
    }
}
