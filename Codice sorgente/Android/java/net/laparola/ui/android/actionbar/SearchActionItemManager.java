package net.laparola.ui.android.actionbar;

import net.laparola.R;
import net.laparola.ui.android.LaParolaActivity;
import net.laparola.ui.android.dialogs.SearchDialog;

import android.content.Context;
import android.text.Editable;
import android.view.*;
import android.view.View.*;
import android.view.inputmethod.EditorInfo;
import android.view.inputmethod.InputMethodManager;
import android.widget.*;
import android.widget.TextView.OnEditorActionListener;

import com.google.android.material.bottomsheet.BottomSheetDialog;

import androidx.annotation.NonNull;

public class SearchActionItemManager implements OnEditorActionListener, MenuItem.OnActionExpandListener, OnClickListener {
    private final LaParolaActivity parent;
    private final MenuItem searchActionItem;
    private EditText searchText;
    private ImageButton searchButton;
    private View advancedButton;
    private BottomSheetDialog currentDialog;

    private String lastReference = null;
    private String lastQuery = "";

    public SearchActionItemManager(LaParolaActivity parent, MenuItem item) {
        this.parent = parent;
        this.searchActionItem = item;
        if (parent.isTablet) {
            if (item.getActionView() != null)
                setupViews(item.getActionView());
        }
    }

    public void setupViews(View root) {
        this.searchText = root.findViewById(R.id.search_edittext);
        this.searchButton = root.findViewById(R.id.search_go_btn);
        this.advancedButton = root.findViewById(R.id.search_advanced_btn);

        if (searchText != null) {
            searchText.setText(lastQuery);
            searchText.setSelection(searchText.getText().length());
            searchText.setOnEditorActionListener(this);
        }
        if (searchButton != null) {
            searchButton.setOnClickListener(this);
        }
        if (advancedButton != null) {
            advancedButton.setOnClickListener(this);
        }
    }

    private void showAsBottomSheet() {
        currentDialog = new BottomSheetDialog(parent);
        View sheetView = parent.getLayoutInflater().inflate(R.layout.bottom_sheet_search, null,false);

        setupViews(sheetView);

        currentDialog.setContentView(sheetView);
        currentDialog.show();
    }

    public void onClick(View view) {
        if (view == searchButton) {
            search();
        } else if (view == advancedButton) {
            if (currentDialog != null && currentDialog.isShowing()) {
                currentDialog.dismiss();
            }

            final SearchDialog d = new SearchDialog(parent);
            d.show();
            if (searchText != null) {
                d.expressionText.setText(searchText.getText());
                d.expressionText.setSelection(searchText.getText().length());
            }

            if (lastReference != null) {
                d.referenceText.setText(lastReference);
            }

            d.setOnDismissListener(dialog -> {
                if (d.searchOk) {
                    Editable expression = d.expressionText.getText();
                    Editable reference = d.referenceText.getText();

                    lastReference = reference.toString();
                    lastQuery = expression.toString();

                    if (searchText != null) searchText.setText(expression);

                    // EXECUTE THE SEARCH
                    parent.getActiveFragment().vaiARicerca(expression, reference);

                    // Ensure the Action Bar collapses on tablet after search
                    searchActionItem.collapseActionView();
                }
            });
        }
    }

    public void search() {
        if (searchText != null) {
            lastQuery = searchText.getText().toString();

            if (parent.getActiveFragment() != null)
                parent.getActiveFragment().vaiARicerca(searchText.getText());

            // Hide keyboard and close UI
            InputMethodManager imm = (InputMethodManager) parent.getSystemService(Context.INPUT_METHOD_SERVICE);
            imm.hideSoftInputFromWindow(searchText.getWindowToken(), 0);

            if (currentDialog != null && currentDialog.isShowing()) {
                currentDialog.dismiss();
            } else {
                searchActionItem.collapseActionView();
            }
        }
    }

    public boolean onEditorAction(TextView v, int actionId, KeyEvent event) {
        if ((event != null &&
                event.getAction() == KeyEvent.ACTION_DOWN &&
                event.getKeyCode() == KeyEvent.KEYCODE_ENTER) ||
                (actionId == EditorInfo.IME_ACTION_SEARCH)) {

            search();
            return true;
        }

        return false;
    }

    public boolean isExpanded() {
        return searchActionItem.isActionViewExpanded();
    }

    public boolean onMenuItemActionExpand(@NonNull MenuItem item) {
        boolean res = parent.collapseActionViewsExcept(item);
        if (res) {
            searchText.post(() -> {
                searchText.requestFocusFromTouch();
                InputMethodManager imm = (InputMethodManager) parent.getSystemService(Context.INPUT_METHOD_SERVICE);
                imm.showSoftInput(searchText, 0);
            });
        }
        return res;
    }

    public boolean onMenuItemActionCollapse(@NonNull MenuItem item) {
        InputMethodManager imm = (InputMethodManager) parent.getSystemService(Context.INPUT_METHOD_SERVICE);
        imm.hideSoftInputFromWindow(searchText.getWindowToken(), 0);
        searchText.post(searchText::clearFocus);
        return true;
    }

    public void collapse(MenuItem exclude) {
        if (exclude != searchActionItem) {
            searchActionItem.collapseActionView();
        }
    }

    public void expandActionView() {
        if (parent.isTablet) {
            searchActionItem.expandActionView();
        } else {
            showAsBottomSheet();
        }
    }

    public void select(String ricerca) {
        if (ricerca != null && searchText != null)
            searchText.setText(ricerca);
    }
}
