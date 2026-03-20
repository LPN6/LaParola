package net.laparola.ui.android.library;

import net.laparola.R;
import net.laparola.ui.android.LaParolaActivityInitUtility;
import net.laparola.ui.android.LaParolaPreferences;
import net.laparola.ui.android.library.downloadmanager.LibraryDownloaderService;

//import com.actionbarsherlock.app.SherlockFragmentActivity;
//import com.actionbarsherlock.view.MenuItem;
//import com.actionbarsherlock.view.Window;
import com.viewpagerindicator.TabPageIndicator;

import android.content.BroadcastReceiver;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.ServiceConnection;
import android.os.Build;
import android.os.Bundle;
import android.os.IBinder;
import android.support.v4.view.ViewPager;

import androidx.fragment.app.FragmentActivity;

public class LibraryActivity extends FragmentActivity {
	private ViewPager mPager;
	private TabPageIndicator mIndicator;
	private LibraryUpdateTask mUpdateTask;
	private LibraryDownloaderService mDownloader;
	private int mInstalledBibleCount = 0;
	private LibraryFragmentPager mLibraryFragmentPager;

	private ServiceConnection mConnection = new ServiceConnection() {
		public void onServiceConnected(ComponentName className, IBinder service) {
			// This is called when the connection with the service has been
			// established, giving us the service object we can use to
			// interact with the service. Because we have bound to a explicit
			// service that we know is running in our own process, we can
			// cast its IBinder to a concrete class and directly access it.
			mDownloader = ((LibraryDownloaderService.LocalBinder) service).getService();
		}

		public void onServiceDisconnected(ComponentName className) {
			mDownloader = null;
		}
	};

	private BroadcastReceiver downloadFinishedBroadcastReceiver = new BroadcastReceiver() {
		@Override
		public void onReceive(Context context, Intent intent) {
			refreshLibrary(false);
		}
	};

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		requestWindowFeature(Window.FEATURE_INDETERMINATE_PROGRESS);
		super.onCreate(savedInstanceState);
		setContentView(R.layout.components_activity);

		getSupportActionBar().setDisplayHomeAsUpEnabled(true);

		mPager = (ViewPager)findViewById(R.id.pager);
		mLibraryFragmentPager = new LibraryFragmentPager(getSupportFragmentManager(), this);
		mPager.setAdapter(mLibraryFragmentPager);
		mIndicator = (TabPageIndicator)findViewById(R.id.indicator);
		mIndicator.setViewPager(mPager);

		LaParolaPreferences.load(this);
		bindService(new Intent(this, LibraryDownloaderService.class), mConnection, Context.BIND_AUTO_CREATE);
	}

	@Override
	protected void onDestroy() {
		super.onDestroy();
		if (mDownloader != null) {
			unbindService(mConnection);
		}
	}

	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
		if (item.getItemId() == android.R.id.home) {
			finish();
			return true;
		}

		return super.onOptionsItemSelected(item);
	}

	@Override
	protected void onPause() {
		super.onPause();
		unregisterReceiver(downloadFinishedBroadcastReceiver);
		if (mUpdateTask != null) {
			mUpdateTask.cancel(false);
		}
	}

	@Override
	protected void onResume() {
		super.onResume();
		bindService(new Intent(this, LibraryDownloaderService.class), mConnection, Context.BIND_AUTO_CREATE);

			registerReceiver(downloadFinishedBroadcastReceiver, new IntentFilter(LibraryDownloaderService.DOWNLOAD_FINISHED), Context.RECEIVER_NOT_EXPORTED);

		// TODO : non gestisco l'aggiornamento manuale nella cartella sd
		refreshLibrary(false);
	}

	public void refreshLibrary(boolean clean) {
		if (clean) {
			LaParolaActivityInitUtility.aggiungiTesti(LaParolaPreferences.writeStoragePath, this);
		}

		if (mUpdateTask != null) {
			mUpdateTask.cancel(false);
		}
		mUpdateTask = new LibraryUpdateTask(this);
		mUpdateTask.execute();
	}

	public void startDownload(final LibraryItemInfo info) {
		if (mDownloader == null) {
			mPager.postDelayed(new Runnable() {
				@Override
				public void run() {
					LibraryActivity.this.startDownload(info);
				}
			}, 100);

			return;
		}

		mDownloader.startDownload(info);
	}

	public int getInstalledBibleCount() {
		return mInstalledBibleCount;
	}

	public void setAdapters(LibraryAdapter bibbiaAdapter, LibraryAdapter commentariAdapter, LibraryAdapter dizionariAdapter) {
		mInstalledBibleCount = bibbiaAdapter.getInstalledBibleCount();
		mLibraryFragmentPager.setAdapters(bibbiaAdapter, commentariAdapter, dizionariAdapter);
	}

	public boolean isDownloading(LibraryItemInfo item) {
		while (mDownloader == null) {
			try {
				Thread.sleep(50);
			} catch (InterruptedException e) {
			}
		}

		return mDownloader.isDownloading(item);
	}
}

