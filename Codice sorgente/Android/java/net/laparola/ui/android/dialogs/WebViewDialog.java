package net.laparola.ui.android.dialogs;

//import net.laparola.R;
import android.annotation.SuppressLint;
import android.content.Context;
import android.content.res.Configuration;
import android.os.Bundle;
import android.util.TypedValue;
import android.view.View;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.FrameLayout;

import net.laparola.R;

public class WebViewDialog extends LaParolaDialog {
	private final Context mContext;
	private final String mUrl;

	public WebViewDialog(Context context, String url) {
		super(context, true);
		mContext = context;
		mUrl = url;
	}

	@SuppressLint("SetJavaScriptEnabled")
    @Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);

		FrameLayout fl = findViewById(net.laparola.R.id.custom);
		WebView wv = new WebView(mContext);

		wv.getSettings().setJavaScriptEnabled(true);
		// Make WebView background match the theme
		int bgColor = getBackgroundColorForMode();

		fl.addView(wv);
		wv.loadUrl(mUrl);
		wv.setBackgroundColor(bgColor);

		// Inject CSS to fix text color
		wv.setWebViewClient(new WebViewClient() {
			@Override
			public void onPageFinished(WebView view, String url) {
				super.onPageFinished(view, url);

				int nightModeFlags = mContext.getResources().getConfiguration().uiMode
						& Configuration.UI_MODE_NIGHT_MASK;

				// Decide text color based on night/day mode
				String textColor = (nightModeFlags == Configuration.UI_MODE_NIGHT_YES) ? "#FEFEFE" : "#010101";
				// Inject JS to update text color
				String js = "document.body.style.color='" + textColor + "';";
				wv.evaluateJavascript(js, null);
			}
		});

			setSoftwareRendererV11();

		custom.setVisibility(View.VISIBLE);

		buttonPanel.setVisibility(View.VISIBLE);

		// Resolve theme-aware text color
		TypedValue typedValue = new TypedValue();
		mContext.getTheme().resolveAttribute(net.laparola.R.attr.colorOnSurface, typedValue, true);
		int textColor = typedValue.data;
		button1.setTextColor(textColor);
		button1.setText(R.string.ok);
		button1.setVisibility(View.VISIBLE);
		button1.setOnClickListener(this);
	}

	private int getBackgroundColorForMode() {
		int nightModeFlags = mContext.getResources().getConfiguration().uiMode
				& android.content.res.Configuration.UI_MODE_NIGHT_MASK;

		return (nightModeFlags==Configuration.UI_MODE_NIGHT_YES ? 0xFF121212 : 0xF3F3F3);
	}

	@Override
	public void onClick(View view) {
		dismiss();
	}
}
