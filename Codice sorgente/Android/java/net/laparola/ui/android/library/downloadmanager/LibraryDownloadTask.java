package net.laparola.ui.android.library.downloadmanager;

import java.io.BufferedInputStream;
import java.io.BufferedOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.MalformedURLException;
import java.net.URL;
import java.net.URLConnection;
import java.nio.channels.FileChannel;
import java.nio.charset.Charset;
import java.util.zip.ZipEntry;
import java.util.zip.ZipFile;

import net.laparola.core.VersioneInformazioni;
import net.laparola.ui.LaParolaBrowser;
import net.laparola.ui.android.LaParolaPreferences;
import net.laparola.ui.android.library.LibraryItemInfo;
import net.laparola.ui.utils.LZMAFile;
import net.laparola.ui.utils.lzma_java.LZMADecoder;
import android.annotation.SuppressLint;
import android.app.Notification;
import android.content.Context;
import android.os.AsyncTask;
import android.os.Build;
import android.os.PowerManager;

public class LibraryDownloadTask extends AsyncTask<Void, Integer, Boolean> {
	/* package */ static final int DOWNLOAD_PERCENT = 80;

	static final boolean USE_SD_REPOSITORY = false;
	static final int BUFFER_LENGHT = 16384;

	private LibraryDownloaderService libraryDownloader;
	public LibraryItemInfo libraryInfo;
	public int notificationID;

	public enum Status {
		WORKING, DONE, ERROR
	}

	public Status status;
	public int progress;

	public LibraryDownloadTask(LibraryDownloaderService libraryDownloader) {
		this.libraryDownloader = libraryDownloader;
		status = Status.WORKING;
	}

	protected Boolean doInBackground(Void... params) {
		publishProgress(0);

		return downloadAndUncompress(LaParolaPreferences.useLzma);
	}

	private boolean downloadAndUncompress(boolean useLzma) {
		try {
			download(useLzma);
		} catch (Exception e) {
			e.printStackTrace();
			if (useLzma == true) {
				return downloadAndUncompress(false);
			} else {
				return false;
			}
		}

		String downloadFileType = useLzma ? libraryInfo.getDownload1FileType() : libraryInfo.getDownload2FileType();
		long downloadSize = useLzma ? libraryInfo.getDownload1Size() : libraryInfo.getDownload2Size();

		File comprFile = new File(libraryInfo.getFileName() + "." + downloadFileType);
		if (comprFile.length() != downloadSize) {
			// return false;
		}

		PowerManager pm = (PowerManager)libraryDownloader.getSystemService(Context.POWER_SERVICE);
		PowerManager.WakeLock wl = pm.newWakeLock(PowerManager.PARTIAL_WAKE_LOCK, "laparola:uncompress");
		wl.acquire();

		try {
			if (downloadFileType.equals("zip")) {
				uncompresszip(comprFile);
			} else {
				uncompresslzma(comprFile);
			}
		} catch (Exception e) {
			e.printStackTrace();
			if (useLzma) {
				return downloadAndUncompress(false);
			} else {
				return false;
			}
		} finally {
			wl.release();
			comprFile.delete();
		}

		VersioneInformazioni informazioniTesto = LaParolaBrowser.getInformazioniTesto(libraryInfo.getName());
		String oldpath = null;
		if (informazioniTesto != null) {
			oldpath = informazioniTesto.getNomeDelFile();
		}

		LaParolaBrowser.cancellaTesto(libraryInfo.getName(), libraryInfo.getFileName());
		File filetmp = new File(libraryInfo.getFileName() + ".tmp");
		File file = new File(oldpath != null ? oldpath : libraryInfo.getFileName());
		boolean ok = filetmp.renameTo(file);
		if (!ok) {
			try {
				copyFile(filetmp, file);
			} catch (Exception e) {
				e.printStackTrace();
				return false;
			}
			filetmp.delete();
		}

		//try {
		LaParolaBrowser.aggiungiTesto(libraryInfo.getFileName());
		//} catch (FileNonValidoException e) {
		//	return false;
		//}

		return true;
	}

	private void copyFile(File srcFile, File destFile) throws IOException {
		final long FILE_COPY_BUFFER_SIZE = 1 << 20;

		FileInputStream fis = null;
		FileOutputStream fos = null;
		FileChannel input = null;
		FileChannel output = null;
		try {
			fis = new FileInputStream(srcFile);
			fos = new FileOutputStream(destFile);
			input  = fis.getChannel();
			output = fos.getChannel();
			final long size = input.size();
			long pos = 0;
			long count = 0;
			while (pos < size) {
				count = size - pos > FILE_COPY_BUFFER_SIZE ? FILE_COPY_BUFFER_SIZE : size - pos;
				pos += output.transferFrom(input, pos, count);
			}
		} finally {
			try {output.close();} catch (Exception e) {}
			try {fos.close();} catch (Exception e) {}
			try {input.close();} catch (Exception e) {}
			try {fis.close();} catch (Exception e) {}
		}

		if (srcFile.length() != destFile.length()) {
			throw new IOException("Failed to copy full contents from '" +
					srcFile + "' to '" + destFile + "'");
		}
	}

	protected void uncompresslzma(File comprFile) throws Exception {
		InputStream in;
		OutputStream out;
		try {
			in = new FileInputStream(comprFile);
			out = new FileOutputStream(libraryInfo.getFileName() + ".tmp");
		} catch (Exception e) {
			e.printStackTrace();
			throw e;
		}

		BufferedInputStream src = new BufferedInputStream(in, BUFFER_LENGHT);
		BufferedOutputStream dest = new BufferedOutputStream(out, BUFFER_LENGHT);

		try {
			LZMAFile.decomprimi(src, dest, new LZMADecoder.ProgressRunnable() {
				int lastPercent = 0;
				long lastPercentTime = 0;

				@Override
				public void publish(long progresso, long size) {
					int percent = (int) Math.round(DOWNLOAD_PERCENT + (double) progresso / size * (100 - DOWNLOAD_PERCENT));
					long milliTime = System.nanoTime() / 1000000;
					if (lastPercent != percent && milliTime > lastPercentTime + 1000) {
						// android.util.Log.d("laparola", String.format("%d %d", progress, size));
						publishProgress(percent);
						lastPercent = percent;
						lastPercentTime = milliTime;
					}
				}
			});
		} catch (Exception e) {
			throw e;
		} finally {
			in.close();
			out.close();
		}
	}

	protected void uncompresszip(File zipFile) throws IOException {
		ZipFile zip = new ZipFile(zipFile);
		ZipEntry zipEntry = null;
		try {
			zipEntry = zip.entries().nextElement();
		} catch (IllegalArgumentException e) {
			if (Build.VERSION.SDK_INT >= 24) {
				for (Charset cs : Charset.availableCharsets().values()) {
					boolean ok = true;
					try {
						zip = new ZipFile(zipFile, cs);
						zipEntry = zip.entries().nextElement();
					} catch (IllegalArgumentException e2) {
						ok = false;
					}
					if (ok) {
						break;
					}
				}
			}
		}
		if (zipEntry == null) {
			throw new IllegalArgumentException("could not determine charset");
		}
		InputStream in = zip.getInputStream(zipEntry);
		OutputStream out = new FileOutputStream(libraryInfo.getFileName() + ".tmp");

		long uncompressed = 0;
		long length = zipEntry.getSize();

		int lastProgress = -1;
		long lastProgressTime = -1;

		try {
			byte buf[] = new byte[BUFFER_LENGHT];
			int len;
			while (!isCancelled() && (len = in.read(buf)) > 0) {
				out.write(buf, 0, len);

				uncompressed += len;
				int progresso = (int) Math.round(DOWNLOAD_PERCENT + (double) uncompressed / length * (100 - DOWNLOAD_PERCENT));
				long milliTime = System.nanoTime() / 1000000;
				if (lastProgress != progresso && milliTime > lastProgressTime + 1000) {
					publishProgress(progresso);
					lastProgress = progresso;
					lastProgressTime = milliTime;
				}
			}
		} finally {
			in.close();
			out.close();
		}
	}

	protected void download(boolean useLzma) throws IOException {
		String url = useLzma ? libraryInfo.getUrl() : libraryInfo.getUrl2();
		String destination = libraryInfo.getFileName() + "." + (useLzma ? libraryInfo.getDownload1FileType() : libraryInfo.getDownload2FileType());

		downloadFile(url, destination);
	}

	@SuppressLint("SdCardPath")
	protected void downloadFile(String url, String destination)	throws IOException {
		/*
		for (int progresso = 0; progresso < DOWNLOAD_PERCENT && !isCancelled(); progresso++) {
			publishProgress(progresso);
			try {
				Thread.sleep(1000);
			} catch (InterruptedException e) {
			}
		}
		*/

		PowerManager pm = (PowerManager)libraryDownloader.getSystemService(Context.POWER_SERVICE);
		PowerManager.WakeLock wl = pm.newWakeLock(PowerManager.PARTIAL_WAKE_LOCK, "laparola:download");
		wl.acquire();

		InputStream fis = null;
		OutputStream fos = null;

		if (Build.VERSION.SDK_INT < Build.VERSION_CODES.P) {
			url = url.replace("https://", "http://");
		}
		if (USE_SD_REPOSITORY) {
			url = url.replace("http://", "file:///sdcard/laparola/");
		}

		try {
			URL u = new URL(url);
			URLConnection con = u.openConnection();
			con.setConnectTimeout(10000);
			con.setReadTimeout(10000);
			int contentLength = con.getContentLength();
			fis = con.getInputStream();

			File file = new File(destination);

			file.delete();   // TODO : implementare ripresa download interrotti!

			fis.skip(file.length());
			long downloaded = file.length();
			fos = new FileOutputStream(file, file.exists());

			int lastProgress = -1;
			long lastProgressTime = -1;

			byte buf[] = new byte[BUFFER_LENGHT];
			int len;
			while (!isCancelled() && (len = fis.read(buf)) > 0) {
				fos.write(buf, 0, len);

				downloaded += len;
				int progresso = (int) Math.round((double) downloaded / contentLength * DOWNLOAD_PERCENT);
				long milliTime = System.nanoTime() / 1000000;
				if (lastProgress != progresso && milliTime > lastProgressTime + 1000) {
					publishProgress(progresso);
					lastProgress = progresso;
					lastProgressTime = milliTime;
				}

				if (USE_SD_REPOSITORY) {
					try {
						Thread.sleep(50);
					} catch (Exception e) {
						//
					}
				}
			}
		} finally {
			wl.release();
			if (fis != null)
				fis.close();
			if (fos != null)
				fos.close();
		}
	}

	@Override
	protected void onProgressUpdate(Integer... values) {
		progress = values[0];
		notificate(progress);
	}

	protected void notificate(Integer progresso) {
		Notification notification = DownloadNotificationBuilder.getNotification(libraryDownloader, libraryInfo, notificationID, progresso);
		libraryDownloader.notify(notificationID, notification);
	}

	protected void onPostExecute(Boolean result) {
		if (result == true) {
			status = Status.DONE;
			notificate(100);
		} else {
			status = Status.ERROR;
			notificate(-1);
		}
		libraryDownloader.onDownloadFinished(this, result);
	}

	protected void onCancelled() {
		libraryDownloader.cancelNotification(notificationID);
	}

}