package net.laparola.ui.android.dialogs;

import net.laparola.R;
import net.laparola.ui.LaParolaUrl;
import net.laparola.ui.android.LaParolaActivity;
import net.laparola.ui.android.bibleview.BibleView;
import android.os.Build;
import android.os.Bundle;
import android.view.View;
import android.view.ViewGroup.LayoutParams;
import android.widget.LinearLayout;

public class PopupDialog extends HoloDialog {
	private BibleView mBibleView;
	private LaParolaUrl mUrl;
	
	public PopupDialog(LaParolaActivity context) {
		super(context, true);
	}
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);

		if (Build.VERSION.SDK_INT >= 11) {
			// la webview ha dei bug su ics legati all'accelerazione
			// grafica: niente sfondo trasparente e problemi con il
			// pulsante sottostante
			setSoftwareRendererV11();
		}
		
		mBibleView = new BibleView(mContext, null);
		mBibleView.setVisibility(View.GONE);   // per evitare glitch, sarà visualizzato a pagina caricata
		LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LayoutParams.MATCH_PARENT, LayoutParams.WRAP_CONTENT);
		layoutParams.weight = 1;
		setContentView(mBibleView, layoutParams);
		
		setTitle(mUrl.versione);
		//topPanel.setVisibility(View.GONE);
		
		LaParolaActivity laParolaActivity = (LaParolaActivity)mContext;
		boolean t = laParolaActivity.getPanesNumber() < LaParolaActivity.MAX_PANELS;
		
		setButtons(R.string.open_in_panel, R.string.close, t ? R.string.open_new_panel : 0);
		
		mBibleView.post(new Runnable() {
			@Override
			public void run() {
				mBibleView.getBrowser().vaiAdUrl(mUrl);
			}
		});
	}
	
	@Override
	public void onClick(View v) {
		LaParolaActivity laParolaActivity = (LaParolaActivity)mContext;
		LaParolaUrl urlCorrente = mBibleView.getBrowser().getUrlCorrente();

		if (urlCorrente == null) {
			urlCorrente = mUrl;
		}
		
		if (v == button1) {
			laParolaActivity.selectPanelForOpening(urlCorrente);
		} else if (v == button3) {
			laParolaActivity.openInNewPanel(urlCorrente);
		}
		
		dismiss();
	}

	protected void onCancelClick() {}

	public void setUrl(LaParolaUrl url) {
		mUrl = url;
	}
}