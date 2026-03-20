package net.laparola.ui.android.actionbar;

import net.laparola.R;
import net.laparola.core.Testi.TestoTipi;
import net.laparola.ui.android.LaParolaActivity;
import net.laparola.ui.android.LaParolaFragment;
import net.laparola.ui.android.actionbar.bibleversionspinner.BibleVersionSpinner;
import net.laparola.ui.android.actionbar.bibleversionspinner.VersionAdapter;
import net.laparola.ui.android.ignspinner.IgnAdapterView;
import net.laparola.ui.android.ignspinner.IgnAbsSpinner;
import net.laparola.ui.android.ignspinner.IgnAdapterView.OnItemSelectedListener;
import net.laparola.ui.android.library.LibraryActivity;
import android.content.Intent;
import android.view.MenuItem;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.ImageButton;
import android.widget.LinearLayout;

public class LibraryActionItemManager implements OnItemSelectedListener, OnClickListener, MenuItem.OnActionExpandListener {
	private LaParolaActivity parent;
	private MenuItem libraryActionItem;
	private IgnAbsSpinner versionSpinner;
	private boolean ignoreSelection;
	private VersionAdapter versionAdapter;
	//private ImageButton managementButton;
	private View panelsButton;
	//private VersionChooserPopup mVersionChooserPopup;

	public LibraryActionItemManager(LaParolaActivity laParolaActivity, MenuItem actionItem) {
		//mVersionChooserPopup = new VersionChooserPopup(laParolaActivity, laParolaActivity.bibleView.getBrowser());
		
		parent = laParolaActivity;
		libraryActionItem = actionItem;

		actionItem.setOnActionExpandListener(this);

		LinearLayout libraryActionView = (LinearLayout) actionItem.getActionView();
		versionSpinner = (BibleVersionSpinner)libraryActionView.findViewById(R.id.version_spinner);
		versionSpinner.setOnItemSelectedListener(this);
		versionAdapter = new VersionAdapter(laParolaActivity);
		versionSpinner.setAdapter(versionAdapter);
		
		/*
		managementButton = (ImageButton) libraryActionView.findViewById(R.id.version_management_button);
		managementButton.setOnClickListener(this);
		*/
		
		panelsButton = libraryActionView.findViewById(R.id.panels_button);
		panelsButton.setOnClickListener(this);
	}

	public void collapse(MenuItem exclude) {
		if (libraryActionItem != exclude) {
			libraryActionItem.collapseActionView();
		}
	}

	public void onVersionChanged() {
		refreshVersions();
	}

	private void refreshVersions() {
		versionAdapter.refresh();
		
		LaParolaFragment activeFragment = parent.getActiveFragment();
		if (activeFragment == null || !activeFragment.isCreated()) {
			return;
		}
		
		String versione = activeFragment.getVersione();
		setVersion(versione);
    }
    
    public void setVersion (String versione) {
		TestoTipi t = versionAdapter.getVersionType(versione);
		if (t != TestoTipi.NESSUNO)
			versionAdapter.setTipo(t);
		
		int selection = versionAdapter.getPosition(versione);	
		if (selection != -1) {
			versionAdapter.sendChanged();
			versionSpinner.setSelection(selection);
			ignoreSelection = true;
		}
	}

	@Override
	public void onItemSelected(IgnAdapterView<?> view, View itemview, int pos, long id) {
		if (ignoreSelection) {
			ignoreSelection = false;
		} else if (!versionSpinner.isPopupVisible()) {
			String item = (String)versionAdapter.getItem(pos);
			
			if (!item.equals(versionAdapter.NO_VERSION_INSTALLED)) {
				LaParolaFragment activeFragment = parent.getActiveFragment();
				if (activeFragment != null)
					activeFragment.setVersione(item);
			} else {			
				startLibrary();
			}
		}
	}

	@Override
	public void onNothingSelected(IgnAdapterView<?> view) {
		//
	}

	@Override
	public void onClick(View view) {
		if (view == panelsButton) {
			parent.showPanelsManagment();
		}
		/*
		else if (view == managementButton) {
			startLibrary();
		}
		*/
	}

	private void startLibrary() {
		Intent intent = new Intent(parent, LibraryActivity.class);
		parent.startActivity(intent);
	}

	@Override
	public boolean onMenuItemActionExpand(MenuItem item) {
		return parent.collapseActionViewsExcept(libraryActionItem);
	}

	@Override
	public boolean onMenuItemActionCollapse(MenuItem item) {
		return true;
	}

	public void onTestiCambiati() {
		refreshVersions();
	}

	public void expandActionView() {
		libraryActionItem.expandActionView();
	}
}
