package net.laparola.ui.android.library.downloadmanager;

import net.laparola.ui.android.library.LibraryItemInfo;
import net.laparola.ui.android.library.downloadmanager.LibraryDownloadTask.Status;
import android.app.Notification;
import android.app.NotificationManager;
import android.app.Service;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.Binder;
import android.os.IBinder;
import android.util.SparseArray;

public class LibraryDownloaderService extends Service {
	public static final String DOWNLOAD_FINISHED = "net.laparola.DOWNLOAD_FINISHED";
	public static final String NOTIFICATION_SELECTED = "net.laparola.NOTIFICATION_SELECTED";

	public class LocalBinder extends Binder {
		public LibraryDownloaderService getService() {
			return LibraryDownloaderService.this;
		}
	}

	private final IBinder mBinder = new LocalBinder();

	private NotificationManager mNotificationManager;
	private SparseArray<LibraryDownloadTask> mDownloaders;

	@Override
	public IBinder onBind(Intent intent) {
		return mBinder;
	}

	private BroadcastReceiver notificationSelectedBroadcastReceiver = new BroadcastReceiver() {
		@Override
		public void onReceive(Context context, Intent intent) {
			int id = intent.getExtras().getInt("notificationID");
			LibraryDownloadTask downloadTask = mDownloaders.get(id);
			if (downloadTask != null && 
				downloadTask.status == LibraryDownloadTask.Status.WORKING && 
				downloadTask.progress < LibraryDownloadTask.DOWNLOAD_PERCENT) {

				Intent i = new Intent(LibraryDownloaderService.this, CancelDownloadActivity.class);
				i.putExtra("clickedNotificationID", id);
				i.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
				startActivity(i);
			}
		}
	};

	@Override
	public void onCreate() {
		super.onCreate();

		mNotificationManager = (NotificationManager) getSystemService(Context.NOTIFICATION_SERVICE);
		mDownloaders = new SparseArray<LibraryDownloadTask>();

		registerReceiver(notificationSelectedBroadcastReceiver, new IntentFilter(NOTIFICATION_SELECTED), Context.RECEIVER_NOT_EXPORTED);
	}

	@Override
	public int onStartCommand(Intent intent, int flags, int startId) {
		return START_STICKY;
	}

	@Override
	public void onDestroy() {
		for (int i = 0; i < mDownloaders.size(); i++) {
			LibraryDownloadTask t = mDownloaders.valueAt(i);
			t.cancel(false);
		}
		unregisterReceiver(notificationSelectedBroadcastReceiver);
	}

	public synchronized void startDownload(LibraryItemInfo info) {
		int notificationId = 0;
		while (mDownloaders.get(notificationId) != null) {
			notificationId++;
		}

		if (isDownloading(info))
			return;

		LibraryDownloadTask downloader = new LibraryDownloadTask(this);
		downloader.notificationID = notificationId;
		downloader.libraryInfo = info;
		downloader.execute();

		mDownloaders.put(notificationId, downloader);
	}

	public synchronized void cancelDownload(int notificationID) {
		LibraryDownloadTask t = mDownloaders.get(notificationID);
		if (t != null) {
			t.cancel(false);
		}
	}

	public synchronized void onDownloadFinished(LibraryDownloadTask downloadTask, boolean success) {
		// mDownloaders.remove(downloadTask.notificationID);
		// eliminarlo potrebbe portare all'annullamento di download successivi se ne si vuole annullare uno finito
		Intent intent = new Intent(DOWNLOAD_FINISHED);
		intent.setPackage("net.laparola");
		sendBroadcast(intent);   // anche con errore
	}

	public void notify(int notificationID, Notification notification) {
		mNotificationManager.notify(notificationID, notification);
	}

	public void cancelNotification(int notificationID) {
		mNotificationManager.cancel(notificationID);
	}
	
	public boolean isDownloading (LibraryItemInfo info) {
		for (int i = 0; i < mDownloaders.size(); i++) {
			LibraryDownloadTask t = mDownloaders.valueAt(i);
			if (t.status == Status.WORKING &&  t.libraryInfo.getUrl().equals(info.getUrl())) {
				// lo sto già scaricando
				return true;
			}
		}		
		return false;
	}
}
