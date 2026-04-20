package net.laparola.ui.android.library.downloadmanager;

import net.laparola.ui.android.library.LibraryActivity;
import net.laparola.ui.android.library.LibraryItemInfo;

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

import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

import androidx.core.content.ContextCompat;

public class LibraryDownloaderService extends Service {
    public static final String DOWNLOAD_FINISHED = "net.laparola.DOWNLOAD_FINISHED";
    public static final String DOWNLOAD_PROGRESS = "net.laparola.DOWNLOAD_PROGRESS";
    public static final String NOTIFICATION_SELECTED = "net.laparola.NOTIFICATION_SELECTED";

    // Extras for broadcasts
    public static final String EXTRA_PROGRESS = "progress";
    public static final String EXTRA_URL = "url";
    public static final String EXTRA_SUCCESS = "success";

    public class LocalBinder extends Binder {
        public LibraryDownloaderService getService() {
            return LibraryDownloaderService.this;
        }
    }

    private final IBinder mBinder = new LocalBinder();
    private NotificationManager mNotificationManager;
    private SparseArray<LibraryDownloadTask> mDownloaders;
    private final ExecutorService mExecutor = Executors.newFixedThreadPool(2); // Allow 2 simultaneous downloads

    @Override
    public IBinder onBind(Intent intent) {
        return mBinder;
    }

    private final BroadcastReceiver mNotificationReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            int id = intent.getIntExtra("notificationID", -1);
            if (id == -1) return;

            String action = intent.getAction();

            if (DownloadNotificationBuilder.ACTION_CANCEL_DOWNLOAD.equals(action)) {
                // User clicked the "CANCEL" button
                cancelDownload(id);
            } else if (NOTIFICATION_SELECTED.equals(action)) {
                // Bring the app to the front if they tap the notification
                Intent launchIntent = new Intent(context, LibraryActivity.class);
                launchIntent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_SINGLE_TOP);
                startActivity(launchIntent);
            }
        }
    };

    @Override
    public void onCreate() {
        super.onCreate();
        mNotificationManager = (NotificationManager) getSystemService(Context.NOTIFICATION_SERVICE);
        mDownloaders = new SparseArray<>();

        // 2. Consolidated Registration
        IntentFilter filter = new IntentFilter();
        filter.addAction(NOTIFICATION_SELECTED);
        filter.addAction(DownloadNotificationBuilder.ACTION_CANCEL_DOWNLOAD);

        ContextCompat.registerReceiver(
                this,
                mNotificationReceiver, // Use the correct variable name here
                filter,
                ContextCompat.RECEIVER_NOT_EXPORTED
        );
    }

    @Override
    public int onStartCommand(Intent intent, int flags, int startId) {
        return START_STICKY;
    }

    @Override
    public void onDestroy() {
        mExecutor.shutdownNow();
        for (int i = 0; i < mDownloaders.size(); i++) {
            LibraryDownloadTask t = mDownloaders.valueAt(i);
            if (t != null) t.cancel();
        }

        // Clean up the receiver
        try {
            unregisterReceiver(mNotificationReceiver);
        } catch (IllegalArgumentException e) {
            // Already unregistered
        }
        super.onDestroy();
    }

    public synchronized void startDownload(LibraryItemInfo info) {
        if (isDownloading(info)) return;

        int notificationId = 0;
        while (mDownloaders.get(notificationId) != null) {
            notificationId++;
        }

        LibraryDownloadTask downloader = new LibraryDownloadTask(this, info, notificationId);
        mDownloaders.put(notificationId, downloader);
        mExecutor.execute(downloader); // Replaces .execute() from AsyncTask
    }

    public synchronized void cancelDownload(int notificationID) {
        LibraryDownloadTask t = mDownloaders.get(notificationID);
        if (t != null) {
            mDownloaders.remove(notificationID); // Remove it from the list
            mNotificationManager.cancel(notificationID); // Dismiss the notification
        }
    }

    // Called by the Task to notify UI via Broadcasts
    public void notifyProgress(LibraryItemInfo info, int progress) {
        Intent intent = new Intent(DOWNLOAD_PROGRESS);
        intent.putExtra(EXTRA_PROGRESS, progress);
        intent.putExtra(EXTRA_URL, info.getUrl());
        intent.setPackage(getPackageName());
        sendBroadcast(intent);
    }

    public synchronized void onDownloadFinished(LibraryDownloadTask downloadTask, boolean success) {
        // Broadcast completion
        Intent intent = new Intent(DOWNLOAD_FINISHED);
        intent.putExtra(EXTRA_SUCCESS, success);
        intent.putExtra(EXTRA_URL, downloadTask.libraryInfo.getUrl());
        intent.setPackage(getPackageName());
        sendBroadcast(intent);
    }

    public void notifyNotification(int notificationID, Notification notification) {
        mNotificationManager.notify(notificationID, notification);
    }

    public void cancelNotification(int notificationID) {
        mNotificationManager.cancel(notificationID);
    }

    public boolean isDownloading(LibraryItemInfo info) {
        for (int i = 0; i < mDownloaders.size(); i++) {
            LibraryDownloadTask t = mDownloaders.valueAt(i);
            if (t.status == LibraryDownloadTask.Status.WORKING && t.libraryInfo.getUrl().equals(info.getUrl())) {
                return true;
            }
        }
        return false;
    }
}