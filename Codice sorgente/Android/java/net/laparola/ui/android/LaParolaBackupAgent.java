package net.laparola.ui.android;

import java.io.IOException;

import net.laparola.ui.LaParolaBrowser;
import net.laparola.ui.utils.Files;

import android.annotation.SuppressLint;
import android.app.backup.BackupAgentHelper;
import android.app.backup.BackupDataInput;
import android.app.backup.BackupDataOutput;
import android.app.backup.BackupManager;
import android.app.backup.FileBackupHelper;
import android.app.backup.SharedPreferencesBackupHelper;
import android.os.ParcelFileDescriptor;

@SuppressLint("NewApi")
public class LaParolaBackupAgent extends BackupAgentHelper {
	private final static String[] PREFS = new String[] {LaParolaPreferences.LAPAROLA_PREFERENCES};
	private final static String[] FILES = new String[] {
		"evidenziati", 
		"cronologia", 
		"preferiti.xml"
	};
	
	@Override
	public void onCreate() {
		SharedPreferencesBackupHelper h1 = new SharedPreferencesBackupHelper(this, PREFS);
		addHelper("prefs", h1);

        FileBackupHelper h2 = new FileBackupHelper(this, FILES);
        addHelper("files", h2);
    }

	private void copyDir(String from, String to) {
		for (String name : FILES) {
			try {
				Files.copyFileIfExists(from + "/" + name, to + "/" + name);
			} catch (IOException e) {
				e.printStackTrace();
			}
		}
	}
	
	@Override
	public void onBackup(ParcelFileDescriptor oldState, BackupDataOutput data, ParcelFileDescriptor newState) throws IOException {
		synchronized (LaParolaBrowser.DataLock) {
			// copyDir(LaParolaPreferences.writeStoragePath, getFilesDir().getAbsolutePath());
			// col nuovo sistema sono sempre sincronizzati (è LaParolaPreferences.internalStoragePath)
			
			super.onBackup(oldState, data, newState);
		}
	}
	
	@Override
	public void onRestore(BackupDataInput data, int appVersionCode, ParcelFileDescriptor newState) throws IOException {
		synchronized (LaParolaBrowser.DataLock) {
			super.onRestore(data, appVersionCode, newState);
			
			LaParolaPreferences.load(null);

			//copyDir(getFilesDir().getAbsolutePath(), LaParolaPreferences.writeStoragePath);
			
			LaParolaBrowser.resetDatiSalvati();
			LaParolaActivityInitUtility.caricaFileDati(LaParolaPreferences.readStoragePaths);
		}
	}
	
	public static void dataChanged (String packageName) {
        BackupManager.dataChanged(packageName);
	}
}
