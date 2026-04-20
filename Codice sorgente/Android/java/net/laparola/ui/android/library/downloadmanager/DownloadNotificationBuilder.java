package net.laparola.ui.android.library.downloadmanager;

import net.laparola.R;
import net.laparola.ui.android.library.LibraryItemInfo;

import android.app.Notification;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;

import java.util.Locale;

import androidx.core.app.NotificationCompat;

public class DownloadNotificationBuilder {
    private final Context mContext;
    public static final String ACTION_CANCEL_DOWNLOAD = "net.laparola.CANCEL_DOWNLOAD";

    public int icon;
    public String title;
    public String caption;
    public String contentInfo;
    public String channel;
    private final int mNotificationID;
    public long when;
    public int progress;
    public boolean autoCancel;
    public boolean onGoing;
    public boolean createExpandedView;
    public Intent intent;
    public PendingIntent pendingIntent;

    private DownloadNotificationBuilder(Context context, String channel, LibraryItemInfo info, int notificationID, Integer progress) {
        mContext = context;

        this.channel = channel;
        this.mNotificationID = notificationID;
        title = info.getName();
        when = System.currentTimeMillis();
        this.progress = progress;
        if (progress == 100) {
            caption = mContext.getString(R.string.notification_download_complete);
            contentInfo = null;
            onGoing = false;
            autoCancel = true;
            icon = R.drawable.ic_stat_download_ok;
            createExpandedView = false;
        } else if (progress == -1) {
            caption = mContext.getString(R.string.notification_download_failed);
            contentInfo = null;
            onGoing = false;
            autoCancel = true;
            icon = android.R.drawable.stat_sys_warning;
            createExpandedView = false;
        } else {
            if (progress > LibraryDownloadTask.DOWNLOAD_PERCENT) {
                caption = mContext.getString(R.string.decompressing);
            } else {
                caption = info.getDescription();
            }
            contentInfo = String.format(Locale.getDefault(), "%d%%", progress);
            onGoing = true;
            autoCancel = false;
            icon = android.R.drawable.stat_sys_download_done;
            createExpandedView = true;
        }

        intent = new Intent(LibraryDownloaderService.NOTIFICATION_SELECTED);
        intent.putExtra("notificationID", notificationID);
        pendingIntent = PendingIntent.getBroadcast(mContext, notificationID, intent, PendingIntent.FLAG_IMMUTABLE | PendingIntent.FLAG_UPDATE_CURRENT);
    }

    public static Notification getNotification(Context context, LibraryItemInfo info, int notificationID, Integer progress) {
        String CHANNEL_ID = "download_channel";
        DownloadNotificationBuilder builder = new DownloadNotificationBuilder(context, CHANNEL_ID, info, notificationID, progress);

        NotificationManager notificationManager = (NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);
        CharSequence name = "Download";
        String Description = "Le notifiche relative ai download";
        int importance = NotificationManager.IMPORTANCE_LOW;
        NotificationChannel mChannel = new NotificationChannel(CHANNEL_ID, name, importance);
        mChannel.setDescription(Description);
        notificationManager.createNotificationChannel(mChannel);

        return builder.build();
    }

    private Notification build() {
        NotificationCompat.Builder builder = new NotificationCompat.Builder(mContext, channel)
                .setSmallIcon(icon)
                .setWhen(when)
                .setContentTitle(title)
                .setContentText(caption)
                .setSubText(contentInfo)
                .setProgress(100, Math.max(0, progress), false)
                .setOngoing(onGoing)
                .setAutoCancel(autoCancel)
                .setContentIntent(pendingIntent)
                .setDeleteIntent(pendingIntent);

        if (onGoing && progress < LibraryDownloadTask.DOWNLOAD_PERCENT) {
            Intent cancelIntent = new Intent(ACTION_CANCEL_DOWNLOAD);
            cancelIntent.putExtra("notificationID", mNotificationID);
            cancelIntent.setPackage(mContext.getPackageName());

            PendingIntent cancelPendingIntent = PendingIntent.getBroadcast(
                    mContext,
                    mNotificationID,
                    cancelIntent,
                    PendingIntent.FLAG_IMMUTABLE | PendingIntent.FLAG_UPDATE_CURRENT
            );

            // NotificationCompat.Action.Builder still accepts the integer icon ID
            builder.addAction(new NotificationCompat.Action.Builder(
                    android.R.drawable.ic_menu_close_clear_cancel,
                    mContext.getString(android.R.string.cancel),
                    cancelPendingIntent).build());
        }

        return builder.build();
    }
}
