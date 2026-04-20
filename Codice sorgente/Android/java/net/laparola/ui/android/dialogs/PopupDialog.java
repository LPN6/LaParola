package net.laparola.ui.android.dialogs;

import net.laparola.R;
import net.laparola.ui.LaParolaUrl;
import net.laparola.ui.android.LaParolaActivity;
import net.laparola.ui.android.LaParolaPreferences;
import net.laparola.ui.android.bibleview.BibleView;

import android.os.Bundle;
import android.view.View;
import android.view.ViewGroup.LayoutParams;
import android.widget.LinearLayout;

public class PopupDialog extends LaParolaDialog {
	private BibleView mBibleView;
	private LaParolaUrl mUrl;
	
	public PopupDialog(LaParolaActivity context) {
		super(context, true);
	}
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);

		setSoftwareRendererV11();

		mBibleView = new BibleView(getContext(), null);
		mBibleView.setNightMode(LaParolaPreferences.nightMode);

		mBibleView.setVisibility(View.GONE);   // per evitare glitch, sarà visualizzato a pagina caricata
		LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LayoutParams.MATCH_PARENT, LayoutParams.MATCH_PARENT);
		//layoutParams.weight = 1;
		setContentView(mBibleView, layoutParams);
		
		setTitle(mUrl.versione);
		//topPanel.setVisibility(View.GONE);
		
		LaParolaActivity laParolaActivity = (LaParolaActivity)mContext;
		boolean t = laParolaActivity.getPanesNumber() < LaParolaActivity.MAX_PANELS;
		
		setButtons(R.string.open_in_panel, R.string.close, t ? R.string.open_new_panel : 0);

		mBibleView.post(() -> mBibleView.getBrowser().vaiAdUrl(mUrl));

		// Force the dialog window to match the parent's height
		if (getWindow() != null) {
			getWindow().setLayout(LayoutParams.MATCH_PARENT, LayoutParams.MATCH_PARENT);
		}
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

	//protected void onCancelClick() {} non usato più

	public void setUrl(LaParolaUrl url) {
		mUrl = url;
	}
}