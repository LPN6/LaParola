package net.laparola.ui.android.actionbar;

import net.laparola.R;
import net.laparola.ui.android.LaParolaActivity;
import net.laparola.ui.android.dialogs.SearchDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.DialogInterface.OnDismissListener;
import android.text.Editable;
import android.view.*;
import android.view.View.*;
import android.view.inputmethod.EditorInfo;
import android.view.inputmethod.InputMethodManager;
import android.widget.*;
import android.widget.TextView.OnEditorActionListener;

public class SearchActionItemManager implements OnEditorActionListener, MenuItem.OnActionExpandListener, OnClickListener {
    private LaParolaActivity parent;
    private MenuItem searchActionItem;
    private EditText searchText;
    private ImageButton searchButton;
	private ImageButton advancedButton;
    
    private String lastReference = null;

	public SearchActionItemManager(LaParolaActivity parent, MenuItem item) {
        this.parent = parent;
        this.searchActionItem = item;
		LinearLayout searchActionView = (LinearLayout)searchActionItem.getActionView();

		searchText = searchActionView.findViewById(R.id.search_edittext);
		searchText.setOnEditorActionListener(this);
        
        searchButton = searchActionView.findViewById(R.id.search_go_btn);
        searchButton.setOnClickListener(this);

        advancedButton = searchActionView.findViewById(R.id.search_advanced_btn);
        advancedButton.setOnClickListener(this);

		searchActionItem.setOnActionExpandListener(this);
	}

    public void onClick(View view) {
    	if (view == searchButton) {
    		search();
    	} else if (view == advancedButton) {
    		final SearchDialog d = new SearchDialog(parent);
    		d.show();
    		d.expressionText.setText(searchText.getText());
    		d.expressionText.setSelection(searchText.getText().length());
    		//d.expressionText.selectAll();
            if (lastReference != null) {
                d.referenceText.setText(lastReference);
            }
    		d.setOnDismissListener(new OnDismissListener() {
				@Override
				public void onDismiss(DialogInterface dialog) {
					if (d.searchOk) {
						Editable expression = d.expressionText.getText();
						Editable reference = d.referenceText.getText();
                        
                        lastReference = reference.toString();
						
						searchText.setText(expression);
			            parent.getActiveFragment().vaiARicerca(expression, reference);
					}
				}
			});
    	}
    }

	public void search() {
		parent.getActiveFragment().vaiARicerca(searchText.getText());
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
    
    public boolean onMenuItemActionExpand(MenuItem item) {
        boolean res = parent.collapseActionViewsExcept(item);
        if (res) {
	        searchText.post(new Runnable() {
	            public void run() {
	                searchText.requestFocusFromTouch();
	                InputMethodManager imm = (InputMethodManager)parent.getSystemService(Context.INPUT_METHOD_SERVICE);
	                imm.showSoftInput(searchText, 0);
	            }
	        });
        }
        return res;
    }
    
    public boolean onMenuItemActionCollapse(MenuItem item) {
        InputMethodManager imm = (InputMethodManager)parent.getSystemService(Context.INPUT_METHOD_SERVICE);
        imm.hideSoftInputFromWindow(searchText.getWindowToken(), 0);
        searchText.post(new Runnable() {
            public void run() {
                searchText.clearFocus();
            }
        });
        return true;
    }
    
    public void collapse (MenuItem exclude) {
        if (exclude != searchActionItem) {
            searchActionItem.collapseActionView();
        }
    }

	public void expandActionView() {
		searchActionItem.expandActionView();
	}

	public void select(String ricerca) {
		searchText.setText(ricerca);
	}
}
