package net.laparola.ui.android.library.downloadmanager;

import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;

import net.laparola.R;
import android.app.Activity;
import android.content.ComponentName;
import android.content.Context;
import android.content.Intent;
import android.content.ServiceConnection;
import android.os.Bundle;
import android.os.IBinder;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.Window;
import android.widget.Button;
import android.widget.FrameLayout;
import android.widget.LinearLayout;
import android.widget.TextView;

public class CancelDownloadActivity extends Activity implements OnClickListener {
	protected FrameLayout custom;
	protected LinearLayout topPanel;
	protected TextView message;
	protected LinearLayout buttonPanel;
	protected Button button1;
	protected Button button2;
	protected Button button3;
	protected TextView alertTitle;
	protected Context mContext;
	protected LinearLayout contentPanel;
	protected View divider1;
	protected View divider2;

	private ServiceConnection mConnection = new ServiceConnection() {
		public void onServiceConnected(ComponentName className, IBinder service) {
			// This is called when the connection with the service has been
			// established, giving us the service object we can use to
			// interact with the service. Because we have bound to a explicit
			// service that we know is running in our own process, we can
			// cast its IBinder to a concrete class and directly access it.
			downloader = ((LibraryDownloaderService.LocalBinder) service).getService();
		}

		public void onServiceDisconnected(ComponentName className) {
			downloader = null;
		}
	};

	private int id;
	private LibraryDownloaderService downloader;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		Bundle intentextras = getIntent().getExtras();

		if (intentextras == null || !intentextras.containsKey("clickedNotificationID")) {
			finish();
		}
		id = intentextras.getInt("clickedNotificationID");

		bindService(new Intent(this, LibraryDownloaderService.class), mConnection, Context.BIND_AUTO_CREATE);

		getWindow().requestFeature(Window.FEATURE_NO_TITLE);

		super.setContentView(R.layout.holo_alert_dialog);
		topPanel = findViewById(R.id.topPanel);
		alertTitle = findViewById(R.id.alertTitle);
		contentPanel = findViewById(R.id.contentPanel);
		message = findViewById(R.id.message);
		custom = findViewById(R.id.custom);
		buttonPanel = findViewById(R.id.buttonPanel);
		button2 = findViewById(R.id.button2);
		divider1 = findViewById(R.id.divider1);
		button3 = findViewById(R.id.button3);
		divider2 = findViewById(R.id.divider2);
		button1 = findViewById(R.id.button1);

		message.setVisibility(View.GONE);
		custom.setVisibility(View.GONE);

		buttonPanel.setVisibility(View.GONE);
		button2.setVisibility(View.GONE);
		divider1.setVisibility(View.GONE);
		button3.setVisibility(View.GONE);
		divider2.setVisibility(View.GONE);
		button1.setVisibility(View.GONE);
		setCloseOnTouchOutside(true);

		alertTitle.setText(R.string.download_cancel);

		message.setVisibility(View.VISIBLE);
		message.setText(R.string.confirm_download_cancel);

		buttonPanel.setVisibility(View.VISIBLE);
		button1.setVisibility(View.VISIBLE);
		button2.setVisibility(View.VISIBLE);
		divider1.setVisibility(View.VISIBLE);

		button1.setOnClickListener(this);
		button2.setOnClickListener(this);

		button1.setText(R.string.download_cancel);
		button2.setText(android.R.string.cancel);
	}

	@Override
	protected void onDestroy() {
		super.onDestroy();
		unbindService(mConnection);
	}

	protected void setCloseOnTouchOutside(boolean value) {
		Method m;
		try {
			m = Window.class.getMethod("setCloseOnTouchOutside", boolean.class);
			m.invoke(getWindow(), value);
		} catch (NoSuchMethodException e) {
			//
		} catch (IllegalArgumentException e) {
			//
		} catch (IllegalAccessException e) {
			//
		} catch (InvocationTargetException e) {
			//
		}
	}

	@Override
	public void onClick(View v) {
		if (v == button1) {
			if (downloader != null) {
				downloader.cancelDownload(id);
			}
		}
		finish();
	}
}
