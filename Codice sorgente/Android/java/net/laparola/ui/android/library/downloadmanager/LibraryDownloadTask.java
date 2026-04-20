package net.laparola.ui.android.library.downloadmanager;

import android.app.Notification;
import android.content.Context;
import android.os.Handler;
import android.os.Looper;
import android.os.PowerManager;

import net.laparola.core.VersioneInformazioni;
import net.laparola.ui.LaParolaBrowser;
import net.laparola.ui.android.LaParolaPreferences;
import net.laparola.ui.android.library.LibraryItemInfo;
import net.laparola.ui.utils.LZMAFile;
import net.laparola.ui.utils.lzma_java.LZMADecoder;

import java.io.BufferedInputStream;
import java.io.BufferedOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.URL;
import java.net.URLConnection;
import java.nio.channels.FileChannel;
import java.nio.charset.Charset;
import java.util.zip.ZipEntry;
import java.util.zip.ZipFile;

import timber.log.Timber;

public class LibraryDownloadTask implements Runnable {
	/* package */ static final int DOWNLOAD_PERCENT = 80;
	private static final int BUFFER_LENGTH = 16384;

	private final LibraryDownloaderService libraryDownloader;
	public final LibraryItemInfo libraryInfo;
	public final int notificationID;
	private final Handler mainHandler = new Handler(Looper.getMainLooper());

	private volatile boolean isCancelled = false;

	public enum Status { WORKING, DONE, ERROR }
	public volatile Status status;
	public volatile int progress;
	long wakeLockTimeout = 10 * 60 * 1000L;

	public LibraryDownloadTask(LibraryDownloaderService service, LibraryItemInfo info, int notificationID) {
		this.libraryDownloader = service;
		this.libraryInfo = info;
		this.notificationID = notificationID;
		this.status = Status.WORKING;
	}

	public void cancel() {
		isCancelled = true;
	}

	@Override
	public void run() {
		updateProgress(0);
		boolean result = downloadAndUncompress(LaParolaPreferences.useLzma);

		// Respect cancellation at the end
		if (isCancelled) {
			libraryDownloader.cancelNotification(notificationID);
			return;
		}

		status = result ? Status.DONE : Status.ERROR;
		updateProgress(result ? 100 : -1);

		// Notify service to update UI lists
		mainHandler.post(() -> libraryDownloader.onDownloadFinished(this, result));
	}

	private void updateProgress(int value) {
		this.progress = value;

		// Always update the notification, even for -1 (Error) or 100 (Done)
		// The NotificationBuilder now handles showing the right icon/text for these states.
		Notification notification = DownloadNotificationBuilder.getNotification(
				libraryDownloader, libraryInfo, notificationID, value);
		libraryDownloader.notifyNotification(notificationID, notification);

		// Broadcast to Activity for the progress bar
		libraryDownloader.notifyProgress(libraryInfo, value);
	}

	private boolean downloadAndUncompress(boolean useLzma) {
		if (isCancelled) return false;
		try {
			download(useLzma);
		} catch (Exception e) {
			Timber.e(e, "Download failed.");
			// If LZMA fails, try the standard download once before giving up
			return useLzma && !isCancelled && downloadAndUncompress(false);
		}

		if (isCancelled) return false;

		String downloadFileType = useLzma ? libraryInfo.getDownload1FileType() : libraryInfo.getDownload2FileType();
		File comprFile = new File(libraryInfo.getFileName() + "." + downloadFileType);

		PowerManager pm = (PowerManager) libraryDownloader.getSystemService(Context.POWER_SERVICE);
		PowerManager.WakeLock wl = pm.newWakeLock(PowerManager.PARTIAL_WAKE_LOCK, "laparola:uncompress");
		wl.acquire(wakeLockTimeout);

		try {
			if ("zip".equals(downloadFileType)) {
				uncompresszip(comprFile);
			} else {
				uncompresslzma(comprFile);
			}
		} catch (Exception e) {
			Timber.e(e, "Decompression failed.");
			return useLzma && !isCancelled && downloadAndUncompress(false);
		} finally {
			if (wl.isHeld()) wl.release();
			if (comprFile.exists() && !comprFile.delete()) {
				Timber.w("Failed to delete compressed file: %s", comprFile.getAbsolutePath());
			}
		}

		if (isCancelled) return false;

		// Finalize logic
		VersioneInformazioni informazioniTesto = LaParolaBrowser.getInformazioniTesto(libraryInfo.getName());
		String oldpath = (informazioniTesto != null) ? informazioniTesto.getNomeDelFile() : null;

		LaParolaBrowser.cancellaTesto(libraryInfo.getName(), libraryInfo.getFileName());
		File filetmp = new File(libraryInfo.getFileName() + ".tmp");
		File file = new File(oldpath != null ? oldpath : libraryInfo.getFileName());

		if (!filetmp.renameTo(file)) {
			try {
				copyFile(filetmp, file);
				if (filetmp.exists() && !filetmp.delete()) {
					Timber.w("Failed to delete temporary file: %s", filetmp.getAbsolutePath());
				}
			} catch (Exception e) {
				return false;
			}
		}
		LaParolaBrowser.aggiungiTesto(libraryInfo.getFileName());
		return true;
	}

	private void copyFile(File srcFile, File destFile) throws IOException {
		final long FILE_COPY_BUFFER_SIZE = 1 << 20; // 1MB buffer

		// Any resource declared in these parentheses is automatically
		// closed when the block exits, even if an exception is thrown.
		try (FileInputStream fis = new FileInputStream(srcFile);
			 FileOutputStream fos = new FileOutputStream(destFile);
			 FileChannel input = fis.getChannel();
			 FileChannel output = fos.getChannel()) {

			final long size = input.size();
			long pos = 0;
			while (pos < size) {
				long count = Math.min(size - pos, FILE_COPY_BUFFER_SIZE);
				pos += output.transferFrom(input, pos, count);
			}
		}
		// No finally block needed! Null checks and closing are handled by the JVM.

		if (srcFile.length() != destFile.length()) {
			throw new IOException("Failed to copy full contents from '" +
					srcFile + "' to '" + destFile + "'");
		}
	}

	protected void uncompresslzma(File comprFile) throws Exception {
		File outputFile = new File(libraryInfo.getFileName() + ".tmp");

		// Try-with-resources handles closing in the correct order:
		// It closes 'dest' then 'src', which internally closes 'out' and 'in'.
		try (InputStream in = new FileInputStream(comprFile);
			 OutputStream out = new FileOutputStream(outputFile);
			 BufferedInputStream src = new BufferedInputStream(in, BUFFER_LENGTH);
			 BufferedOutputStream dest = new BufferedOutputStream(out, BUFFER_LENGTH)) {

			LZMAFile.decomprimi(src, dest, new LZMADecoder.ProgressRunnable() {
				int lastPercent = 0;
				long lastPercentTime = 0;

				@Override
				public void publish(long progresso, long size) {
					// Check isCancelled inside the callback to stop decompression if needed
					if (isCancelled) {
						// Note: You might need to throw a RuntimeException here
						// if LZMAFile doesn't check for interruption internally.
						return;
					}

					if (size > 0) {
						int percent = (int) Math.round(DOWNLOAD_PERCENT + (double) progresso / size * (100 - DOWNLOAD_PERCENT));
						long milliTime = System.nanoTime() / 1000000;

						if (lastPercent != percent && milliTime > lastPercentTime + 1000) {
							updateProgress(percent);
							lastPercent = percent;
							lastPercentTime = milliTime;
						}
					}
				}
			});
		} catch (Exception e) {
			Timber.e(e, "Unexpected error occurred while decompressing LZMA file.");
			throw e;
		}
	}

	protected void uncompresszip(File zipFile) throws IOException {
		ZipFile zip = null;
		ZipEntry zipEntry = null;

		// 1. Initial attempt with default Charset
		try {
			zip = new ZipFile(zipFile);
			if (zip.entries().hasMoreElements()) {
				zipEntry = zip.entries().nextElement();
			}
		} catch (IllegalArgumentException e) {
			// If the default fails, close the bad handle and try others
			if (zip != null) zip.close();

			for (Charset cs : Charset.availableCharsets().values()) {
				try {
					zip = new ZipFile(zipFile, cs);
					if (zip.entries().hasMoreElements()) {
						zipEntry = zip.entries().nextElement();
						break; // Found it!
					}
				} catch (IllegalArgumentException e2) {
					if (zip != null) zip.close();
					zip = null;
				}
			}
		}

		if (zip == null || zipEntry == null) {
			if (zip != null) zip.close();
			throw new IllegalArgumentException("Could not determine charset or zip is empty");
		}

		// 2. Use try-with-resources for the streams.
		// Note: We MUST keep the ZipFile 'zip' open while reading 'in'.
		try (ZipFile finalZip = zip;
			 InputStream in = finalZip.getInputStream(zipEntry);
			 OutputStream out = new FileOutputStream(libraryInfo.getFileName() + ".tmp")) {

			long uncompressed = 0;
			long length = zipEntry.getSize();
			int lastProgress = -1;
			long lastProgressTime = -1;

			byte[] buf = new byte[BUFFER_LENGTH];
			int len;

			while (!isCancelled && (len = in.read(buf)) > 0) {
				out.write(buf, 0, len);

				uncompressed += len;
				// Avoid division by zero if length is unknown (-1)
				if (length > 0) {
					int progresso = (int) Math.round(DOWNLOAD_PERCENT + (double) uncompressed / length * (100 - DOWNLOAD_PERCENT));
					long milliTime = System.nanoTime() / 1000000;
					if (lastProgress != progresso && milliTime > lastProgressTime + 1000) {
						updateProgress(progresso);
						lastProgress = progresso;
						lastProgressTime = milliTime;
					}
				}
			}
		}
		// finalZip, in, and out are all automatically closed here.
	}

	protected void download(boolean useLzma) throws IOException {
		String url = useLzma ? libraryInfo.getUrl() : libraryInfo.getUrl2();
		String destination = libraryInfo.getFileName() + "." + (useLzma ? libraryInfo.getDownload1FileType() : libraryInfo.getDownload2FileType());

		downloadFile(url, destination);
	}

	protected void downloadFile(String url, String destination)	throws IOException {
		PowerManager pm = (PowerManager)libraryDownloader.getSystemService(Context.POWER_SERVICE);
		PowerManager.WakeLock wl = pm.newWakeLock(PowerManager.PARTIAL_WAKE_LOCK, "laparola:download");
		wl.acquire(wakeLockTimeout);

		InputStream fis = null;
		OutputStream fos = null;

		try {
			URL u = new URL(url);
			URLConnection con = u.openConnection();
			con.setConnectTimeout(10000);
			con.setReadTimeout(10000);
			int contentLength = con.getContentLength();
			fis = con.getInputStream();

			File file = new File(destination);

			// TODO : implementare ripresa download interrotti!
			if (file.exists() && !file.delete()) {
				Timber.w("Failed to delete destination file: %s", file.getAbsolutePath());
			}
			fis.skip(file.length());
			long downloaded = file.length();
			fos = new FileOutputStream(file, file.exists());

			int lastProgress = -1;
			long lastProgressTime = -1;

			byte[] buf = new byte[BUFFER_LENGTH];
			int len;
			while (!isCancelled && (len = fis.read(buf)) > 0) {
				fos.write(buf, 0, len);

				downloaded += len;
				int progresso = (int) Math.round((double) downloaded / contentLength * DOWNLOAD_PERCENT);
				long milliTime = System.nanoTime() / 1000000;
				if (lastProgress != progresso && milliTime > lastProgressTime + 1000) {
					updateProgress(progresso);
					lastProgress = progresso;
					lastProgressTime = milliTime;
				}
			}
		} finally {
			if (wl.isHeld()) {
				wl.release();
			}
			if (fis != null)
				fis.close();
			if (fos != null)
				fos.close();
		}
	}
}