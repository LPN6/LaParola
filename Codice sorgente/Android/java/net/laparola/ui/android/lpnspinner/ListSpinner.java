package net.laparola.ui.android.lpnspinner;

import android.annotation.SuppressLint;
import android.content.Context;
import android.util.AttributeSet;
import android.widget.AbsListView;
import android.widget.LinearLayout;
import android.widget.ListView;

public class ListSpinner extends LpnAbsSpinner {
	public ListSpinner(Context context, AttributeSet attrs) {
		super(context, attrs);
	}	
	
	public ListSpinner(Context context, AttributeSet attrs, int defStyle) {
		super(context, attrs, defStyle);
	}

	@Override
	public LpnDropdownPopup createPopup(Context context, AttributeSet attrs, int defStyle) {
		return new ListDropDownPopup(context, attrs, defStyle, this);
	}
	
	static class LpnListView extends LpnHijackFocusListView {
		public LpnListView(Context context, boolean hijackfocus) {
			super(context, hijackfocus);
		}

		@SuppressLint("InlinedApi")
		@Override
		protected AbsListView createViews(Context context) {
			ListView listView = new ListView(context);
			this.addView(listView, new LinearLayout.LayoutParams(
					LinearLayout.LayoutParams.MATCH_PARENT, 
					LinearLayout.LayoutParams.MATCH_PARENT));
			return listView;
		}

		@Override
		protected int getHeaderHeight(int widthMeasureSpec) {
			return 0;
		}
	}
	
	static class ListDropDownPopup extends LpnDropdownPopup {
	    public ListDropDownPopup(Context context, AttributeSet attrs, int defStyleAttr, LpnAbsSpinner lpnSpinner) {
	    	super(context, attrs, defStyleAttr, lpnSpinner);
	    }
	    
	    @Override
		protected LpnHijackFocusListView createListView(Context context, boolean hijackfocus) {
			return new LpnListView(context, hijackfocus);
		}
	}
}
