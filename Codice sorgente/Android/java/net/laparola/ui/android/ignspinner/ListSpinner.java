package net.laparola.ui.android.ignspinner;

import android.annotation.SuppressLint;
import android.content.Context;
import android.util.AttributeSet;
import android.widget.AbsListView;
import android.widget.LinearLayout;
import android.widget.ListView;

public class ListSpinner extends IgnAbsSpinner {
	public ListSpinner(Context context, AttributeSet attrs) {
		super(context, attrs);
	}	
	
	public ListSpinner(Context context, AttributeSet attrs, int defStyle) {
		super(context, attrs, defStyle);
	}

	@Override
	public IgnDropdownPopup createPopup(Context context, AttributeSet attrs, int defStyle) {
		return new ListDropDownPopup(context, attrs, defStyle, this);
	}
	
	static class IgnListView extends IgnHijackFocusListView {
		public IgnListView(Context context, boolean hijackfocus) {
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
	
	static class ListDropDownPopup extends IgnDropdownPopup {
	    public ListDropDownPopup(Context context, AttributeSet attrs, int defStyleAttr, IgnAbsSpinner ignSpinner) {
	    	super(context, attrs, defStyleAttr, ignSpinner);
	    }
	    
	    @Override
		protected IgnHijackFocusListView createListView(Context context, boolean hijackfocus) {
			return new IgnListView(context, hijackfocus);
		}
	}
}
