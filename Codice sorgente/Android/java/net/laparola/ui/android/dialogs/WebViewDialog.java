package net.laparola.ui.android.dialogs;

import net.laparola.R;
import android.content.Context;
import android.os.Build;
import android.os.Bundle;
import android.view.View;
import android.webkit.WebView;
import android.widget.FrameLayout;

public class WebViewDialog extends HoloDialog {
	private Context mContext;
	private String mUrl;

	public WebViewDialog(Context context, String url) {
		super(context, true);
		mContext = context;
		mUrl = url;
	}

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);

		FrameLayout fl = findViewById(R.id.custom);
		WebView wv = new WebView(mContext);
		fl.addView(wv);
		wv.loadUrl(mUrl);
		wv.setBackgroundColor(0);

		if (Build.VERSION.SDK_INT >= 11) {
			// la webview ha dei bug su ics legati all'accelerazione
			// grafica: niente sfondo trasparente e problemi con il
			// pulsante sottostante
			setSoftwareRendererV11();
		}

		custom.setVisibility(View.VISIBLE);

		buttonPanel.setVisibility(View.VISIBLE);
		button1.setVisibility(View.VISIBLE);
		button1.setText("OK");
		button1.setOnClickListener(this);
	}

	@Override
	public void onClick(View view) {
		dismiss();
	}
}
