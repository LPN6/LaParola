package net.laparola.ui.android.library.downloadmanager;

import net.laparola.R;
import net.laparola.ui.android.library.LibraryItemInfo;
import android.annotation.TargetApi;
import android.app.Notification;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.graphics.Color;
import android.os.Build;
import android.support.v4.app.NotificationCompat;
import android.widget.RemoteViews;

public class DownloadNotificationBuilder {
	private Context mContext;

	public int icon;
	public String title;
	public String caption;
	public String contentInfo;
	public String channel;
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
		title = info.getName();
		when = System.currentTimeMillis();
		this.progress = progress;
		if (progress == 100) {
			caption = mContext.getString(R.string.notification_download_complete);
			contentInfo = null;
			onGoing = false;
			autoCancel = true;
			if (Build.VERSION.SDK_INT >= 11) {
				icon = R.drawable.ic_stat_download_ok;
			} else {
				icon = android.R.drawable.stat_sys_download_done;
			}
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
			contentInfo = String.format("%d%%", progress);
			onGoing = true;
			autoCancel = false;
			if (Build.VERSION.SDK_INT >= 11) {
				icon = android.R.drawable.stat_sys_download_done;
			} else {
				icon = android.R.drawable.stat_sys_download;
			}
			createExpandedView = true;
		}

		intent = new Intent(LibraryDownloaderService.NOTIFICATION_SELECTED);
		intent.putExtra("notificationID", notificationID);
		pendingIntent = PendingIntent.getBroadcast(mContext, notificationID, intent,PendingIntent.FLAG_IMMUTABLE);
	}

	public static Notification getNotification(Context context, LibraryItemInfo info, int notificationID, Integer progress) {
		String CHANNEL_ID = "download_channel";
		DownloadNotificationBuilder builder = new DownloadNotificationBuilder(context, CHANNEL_ID, info, notificationID, progress);

		NotificationManager notificationManager = (NotificationManager)context.getSystemService(Context.NOTIFICATION_SERVICE);
		if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.O) {
			CharSequence name = "Download";
			String Description = "Le notifiche relative ai download";
			int importance = NotificationManager.IMPORTANCE_LOW;
			NotificationChannel mChannel = new NotificationChannel(CHANNEL_ID, name, importance);
			mChannel.setDescription(Description);
			/*
			mChannel.enableLights(true);
			mChannel.setLightColor(Color.RED);
			mChannel.enableVibration(true);
			mChannel.setVibrationPattern(new long[]{100, 200, 300, 400, 500, 400, 300, 200, 400});
			mChannel.setShowBadge(false);
			*/
			notificationManager.createNotificationChannel(mChannel);
		}

		Notification ret = builder.getNotificationCompat();

		return ret;
		/*
		if (Build.VERSION.SDK_INT >= 14) {
			return builder.getNotificationV14();
		}
		// se <= 13
		return builder.getNotificationV7();
		*/
	}

	/*
	@SuppressWarnings("deprecation")
	private Notification getNotificationV7() {
		Notification n = new Notification();

		n.icon = icon;
		n.when = when;
		if (autoCancel)
			n.flags |= Notification.FLAG_AUTO_CANCEL;
		if (onGoing)
			n.flags |= Notification.FLAG_ONGOING_EVENT;

		if (createExpandedView) {
            RemoteViews expandedView = new RemoteViews(mContext.getPackageName(),
                    R.layout.status_bar_ongoing_event_progress_bar);
			n.contentView = expandedView;
			expandedView.setTextViewText(R.id.title, title);
			expandedView.setTextViewText(R.id.description, caption);
			expandedView.setProgressBar(R.id.progress_bar, 100, progress, false);
			expandedView.setTextViewText(R.id.progress_text, contentInfo);
			expandedView.setImageViewResource(R.id.appIcon, n.icon);
			n.contentIntent = pendingIntent;
		} else {
			n.setLatestEventInfo(mContext, title, caption, pendingIntent);
		}
		n.deleteIntent = pendingIntent;

		return n;
	}

	@SuppressWarnings("deprecation")
	@TargetApi(14)
	private Notification getNotificationV14() {
		Notification.Builder builder = new Notification.Builder(mContext);

		builder.setSmallIcon(icon);
		builder.setWhen(when);
		builder.setAutoCancel(autoCancel);
		builder.setOngoing(onGoing);
		builder.setContentTitle(title);
		builder.setContentText(caption);
		builder.setProgress(100, progress, false);
		builder.setContentInfo(contentInfo);
		builder.setContentIntent(pendingIntent);
		builder.setDeleteIntent(pendingIntent);

		return builder.getNotification();
	}
	*/

	private Notification getNotificationCompat() {
		if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.O) {
			Notification.Builder builder = new Notification.Builder(mContext, channel);

			builder.setSmallIcon(icon);
			builder.setWhen(when);
			builder.setAutoCancel(autoCancel);
			builder.setOngoing(onGoing);
			builder.setContentTitle(title);
			builder.setContentText(caption);
			builder.setProgress(100, progress, false);
			builder.setSubText(contentInfo);
			builder.setContentIntent(pendingIntent);
			builder.setDeleteIntent(pendingIntent);

			return builder.build();
		} else {
			NotificationCompat.Builder compatbuilder = new NotificationCompat.Builder(mContext);

			compatbuilder.setSmallIcon(icon);
			compatbuilder.setWhen(when);
			compatbuilder.setAutoCancel(autoCancel);
			compatbuilder.setOngoing(onGoing);
			compatbuilder.setContentTitle(title);
			compatbuilder.setContentText(caption);
			compatbuilder.setProgress(100, progress, false);
			compatbuilder.setContentInfo(contentInfo);
			compatbuilder.setContentIntent(pendingIntent);
			compatbuilder.setDeleteIntent(pendingIntent);

			return compatbuilder.build();
		}
	}
}
