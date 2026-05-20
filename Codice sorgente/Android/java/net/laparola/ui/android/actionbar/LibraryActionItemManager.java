package net.laparola.ui.android.actionbar;

import net.laparola.R;
import net.laparola.core.Testi.TestoTipi;
import net.laparola.ui.android.LaParolaActivity;
import net.laparola.ui.android.LaParolaFragment;
import net.laparola.ui.android.actionbar.bibleversionspinner.BibleVersionSpinner;
import net.laparola.ui.android.actionbar.bibleversionspinner.VersionAdapter;
import net.laparola.ui.android.lpnspinner.LpnAdapterView;
import net.laparola.ui.android.lpnspinner.LpnAdapterView.OnItemSelectedListener;
import net.laparola.ui.android.library.LibraryActivity;

import android.content.Intent;
import android.view.MenuItem;
import android.view.View;
import android.view.View.OnClickListener;

import com.google.android.material.bottomsheet.BottomSheetDialog;

import androidx.annotation.NonNull;

public class LibraryActionItemManager implements OnItemSelectedListener, OnClickListener, MenuItem.OnActionExpandListener {
    private final LaParolaActivity parent;
    private final MenuItem libraryActionItem;
    private BibleVersionSpinner versionSpinner;
    private boolean ignoreSelection;
    private final VersionAdapter versionAdapter;
    private View managementButton;
    private View panelsButton;
    private BottomSheetDialog bottomSheetDialog;
    private String currentVersion;

    public LibraryActionItemManager(LaParolaActivity laParolaActivity, MenuItem actionItem) {
        parent = laParolaActivity;
        libraryActionItem = actionItem;
        versionAdapter = new VersionAdapter(laParolaActivity);

        if (parent.isTablet) {
            if (actionItem.getActionView() != null)
                setupViews(actionItem.getActionView());
        }
    }

    public void setupViews(View root) {
        versionSpinner = root.findViewById(R.id.version_spinner);
        panelsButton = root.findViewById(R.id.panels_button);
        managementButton = root.findViewById(R.id.version_management_button);

        if (versionSpinner != null) {
            versionSpinner.libraryVisible = parent.isTablet;
            versionSpinner.setAdapter(versionAdapter);
            ignoreSelection = true;
            versionSpinner.setOnItemSelectedListener(this);
            refreshVersions();
            if (currentVersion != null) {
                setVersion(currentVersion);
            }
        }

        if (panelsButton != null) {
            panelsButton.setOnClickListener(this);
        }
        if (managementButton != null) {
            managementButton.setOnClickListener(this);
        }
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

    public void setVersion(String versione) {
        this.currentVersion = versione;
        if (versionAdapter == null) return;

        TestoTipi t = versionAdapter.getVersionType(versione);
        if (t != TestoTipi.NESSUNO)
            versionAdapter.setTipo(t);

        int selection = versionAdapter.getPosition(versione);
        if (selection != -1) {
            versionAdapter.sendChanged();
            if (versionSpinner != null) {
                ignoreSelection = true;
                versionSpinner.setSelection(selection);
            }
        }
    }

    @Override
    public void onItemSelected(LpnAdapterView<?> view, View itemview, int pos, long id) {
        if (ignoreSelection) {
            ignoreSelection = false;
        } else {
            String item = (String) versionAdapter.getItem(pos);
            if (!item.equals(versionAdapter.NO_VERSION_INSTALLED)) {
                LaParolaFragment activeFragment = parent.getActiveFragment();
                if (activeFragment != null) {
                    if (!item.equals(activeFragment.getVersione())) {
                        activeFragment.setVersione(item);

                        if (bottomSheetDialog != null && bottomSheetDialog.isShowing()) {
                            bottomSheetDialog.dismiss();
                        }
                    }
                }
            } else {
                startLibrary();
            }
        }
    }

    @Override
    public void onNothingSelected(LpnAdapterView<?> view) {
        //
    }

    @Override
    public void onClick(View view) {
        if (view == panelsButton) {
            parent.showPanelsManagment();
        } else if (view == managementButton) {
            startLibrary();
        }
    }

    private void startLibrary() {
        Intent intent = new Intent(parent, LibraryActivity.class);
        parent.startActivity(intent);
    }

    @Override
    public boolean onMenuItemActionExpand(@NonNull MenuItem item) {
        return parent.collapseActionViewsExcept(libraryActionItem);
    }

    @Override
    public boolean onMenuItemActionCollapse(@NonNull MenuItem item) {
        return true;
    }

    public void onTestiCambiati() {
        refreshVersions();
    }

    public void expandActionView() {
        if (parent.isTablet) {
            libraryActionItem.expandActionView();
        } else {
            showAsBottomSheet();
        }
    }

    private void showAsBottomSheet() {
        bottomSheetDialog = new BottomSheetDialog(parent);
        View sheetView = parent.getLayoutInflater().inflate(R.layout.bottom_sheet_library, null);
        refreshVersions();
        setupViews(sheetView);

        bottomSheetDialog.setContentView(sheetView);
        bottomSheetDialog.show();
    }
}
