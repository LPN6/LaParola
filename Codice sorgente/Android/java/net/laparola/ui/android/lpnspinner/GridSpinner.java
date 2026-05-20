package net.laparola.ui.android.lpnspinner;

import android.content.Context;
import android.util.AttributeSet;
import android.widget.AbsListView;
import android.widget.GridView;

import net.laparola.R;

public class GridSpinner extends LpnAbsSpinner {
    private int mColumnWidth = -1;
    private int mNumColumns = -1;

    public GridSpinner(Context context) {
        super(context, null);
    }

    public GridSpinner(Context context, AttributeSet attrs) {
        super(context, attrs);
    }

    public GridSpinner(Context context, AttributeSet attrs, int defStyle) {
        super(context, attrs, defStyle);
    }

    @Override
    public LpnDropdownPopup createPopup(Context context, AttributeSet attrs, int defStyle) {
        return new GridDropDownPopup(context, attrs, defStyle, this);
    }

    class LpnGridView extends LpnHijackFocusListView {
        GridView mGridView = null;

        public LpnGridView(Context context, boolean hijackfocus) {
            super(context, hijackfocus);
        }

        @Override
        protected AbsListView createViews(Context context) {
            inflate(context, R.layout.grid_spinner, this);
            mGridView = findViewById(R.id.gridview);
            if (mColumnWidth > 0)
                mGridView.setColumnWidth(mColumnWidth);
            if (mNumColumns > 0)
                mGridView.setNumColumns(mNumColumns);
            return mGridView;
        }

        @Override
        protected int getHeaderHeight(int widthMeasureSpec) {
            return 0;
        }
    }

    class GridDropDownPopup extends LpnDropdownPopup {
        public GridDropDownPopup(Context context, AttributeSet attrs, int defStyleAttr, LpnAbsSpinner lpnSpinner) {
            super(context, attrs, defStyleAttr, lpnSpinner);
        }

        @Override
        protected LpnHijackFocusListView createListView(Context context, boolean hijackfocus) {
            return new LpnGridView(context, hijackfocus);
        }
    }

    public void setNumColumns(int numColumns) {
        mNumColumns = numColumns;

        // If the popup already exists, update the internal GridView immediately
        GridDropDownPopup gridDropDownPopup = (GridDropDownPopup) mPopup;
        if (gridDropDownPopup != null) {
            LpnGridView lpnGridView = (LpnGridView) gridDropDownPopup.mDropDownList;
            if (lpnGridView != null && lpnGridView.mGridView != null) {
                lpnGridView.mGridView.setNumColumns(numColumns);
            }
        }
    }

    public void setColumnWidth(int columnWidth) {
        mColumnWidth = columnWidth;

        GridDropDownPopup gridDropDownPopup = (GridDropDownPopup) mPopup;
        if (gridDropDownPopup != null) {
            LpnGridView lpnGridView = (LpnGridView) gridDropDownPopup.mDropDownList;
            if (lpnGridView != null && lpnGridView.mGridView != null) {
                lpnGridView.mGridView.setColumnWidth(columnWidth);
            }
        }
    }
}
