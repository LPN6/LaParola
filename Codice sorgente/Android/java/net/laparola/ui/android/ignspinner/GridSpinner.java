package net.laparola.ui.android.ignspinner;

import android.content.Context;
import android.util.AttributeSet;
import android.widget.AbsListView;
import android.widget.GridView;
import net.laparola.R;
import net.laparola.ui.android.ignspinner.IgnDropdownPopup;
import net.laparola.ui.android.ignspinner.IgnHijackFocusListView;
import net.laparola.ui.android.ignspinner.IgnAbsSpinner;

public class GridSpinner extends IgnAbsSpinner {
	private int mColumnWidth = -1;
	
	public GridSpinner(Context context, AttributeSet attrs) {
		super(context, attrs);
	}	
	
	public GridSpinner(Context context, AttributeSet attrs, int defStyle) {
		super(context, attrs, defStyle);
	}

	@Override
	public IgnDropdownPopup createPopup(Context context, AttributeSet attrs, int defStyle) {
		return new GridDropDownPopup(context, attrs, defStyle, this);
	}
	
	class IgnGridView extends IgnHijackFocusListView {
		GridView mGridView = null;

		public IgnGridView(Context context, boolean hijackfocus) {
			super(context, hijackfocus);
		}

		@Override
		protected AbsListView createViews(Context context) {
			inflate(context, R.layout.grid_spinner, this);
			mGridView = findViewById(R.id.gridview);
			if (mColumnWidth > 0)
				mGridView.setColumnWidth(mColumnWidth); 
			return mGridView;
		}

		@Override
		protected int getHeaderHeight(int widthMeasureSpec) {
			return 0;
		}
	}
	
	class GridDropDownPopup extends IgnDropdownPopup {
	    public GridDropDownPopup(Context context, AttributeSet attrs, int defStyleAttr, IgnAbsSpinner ignSpinner) {
	    	super(context, attrs, defStyleAttr, ignSpinner);
	    }
	    
	    @Override
		protected IgnHijackFocusListView createListView(Context context, boolean hijackfocus) {
			return new IgnGridView(context, hijackfocus);
		}
	}

	public void setColumnWidth(int columnWidth) {
		mColumnWidth = columnWidth;
		
		GridDropDownPopup gridDropDownPopup = (GridDropDownPopup)mPopup;
		if (gridDropDownPopup != null) {
			IgnGridView ignGridView = (IgnGridView)gridDropDownPopup.mDropDownList;
			if (ignGridView != null && ignGridView.mGridView != null) {
				ignGridView.mGridView.setColumnWidth(columnWidth);
			}
		}
	}
}
