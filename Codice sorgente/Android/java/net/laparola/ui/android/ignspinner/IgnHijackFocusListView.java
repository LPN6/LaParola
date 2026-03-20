package net.laparola.ui.android.ignspinner;

import java.lang.reflect.Field;
import java.lang.reflect.Method;

import android.annotation.SuppressLint;
import android.annotation.TargetApi;
import android.content.Context;
import android.graphics.drawable.Drawable;
import android.os.Build;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AbsListView;
import android.widget.Adapter;
import android.widget.AdapterView;
import android.widget.FrameLayout;
import android.widget.GridView;
import android.widget.LinearLayout;
import android.widget.ListAdapter;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.AdapterView.OnItemSelectedListener;
import android.widget.ListView;

public abstract class IgnHijackFocusListView extends FrameLayout {
    /*
     * WARNING: This is a workaround for a touch mode issue.
     *
     * Touch mode is propagated lazily to windows. This causes problems in
     * the following scenario:
     * - Type something in the AutoCompleteTextView and get some results
     * - Move down with the d-pad to select an item in the list
     * - Move up with the d-pad until the selection disappears
     * - Type more text in the AutoCompleteTextView *using the soft keyboard*
     *   and get new results; you are now in touch mode
     * - The selection comes back on the first item in the list, even though
     *   the list is supposed to be in touch mode
     *
     * Using the soft keyboard triggers the touch mode change but that change
     * is propagated to our window only after the first list layout, therefore
     * after the list attempts to resurrect the selection.
     *
     * The trick to work around this issue is to pretend the list is in touch
     * mode when we know that the selection should not appear, that is when
     * we know the user moved the selection away from the list.
     *
     * This boolean is set to true whenever we explicitly hide the list's
     * selection and reset to false whenever we know the user moved the
     * selection back to the list.
     *
     * When this boolean is true, isInTouchMode() returns true, otherwise it
     * returns super.isInTouchMode().
     */
    boolean mListSelectionHidden;

    private boolean mHijackFocus;
    
    protected AbsListView mListView;

    protected AdapterView.OnItemSelectedListener mOnItemSelectedListener;
    
    public IgnHijackFocusListView(Context context, boolean hijackFocus) {
        super(context);
        
        mHijackFocus = hijackFocus;
        
        mListView = createViews(context);
        mListView.setCacheColorHint(0); // Transparent, since the background drawable could be anything.
    }
	
    protected abstract AbsListView createViews(Context context);
	protected abstract int getHeaderHeight (int widthMeasureSpec);

	@Override
    public boolean isInTouchMode() {
        // WARNING: Please read the comment where mListSelectionHidden is declared
        return (mHijackFocus && mListSelectionHidden) || super.isInTouchMode();
    }

    @Override
    public boolean hasWindowFocus() {
        return mHijackFocus || super.hasWindowFocus();
    }

    @Override
    public boolean isFocused() {
        return mHijackFocus || super.isFocused();
    }

    @Override
    public boolean hasFocus() {
        return mHijackFocus || super.hasFocus();
    }

	public void setOnScrollListener(IgnPopupWindow.PopupScrollListener listener) {
		mListView.setOnScrollListener(listener);
	}

	public void setOnItemSelectedListener(OnItemSelectedListener listener) {
		mListView.setOnItemSelectedListener(listener);
        mOnItemSelectedListener = listener;
	}
	
	@TargetApi(Build.VERSION_CODES.HONEYCOMB)
	public void setAdapter(ListAdapter mAdapter) {
		if (Build.VERSION.SDK_INT >= 11) {
			mListView.setAdapter(mAdapter);
		} else {
			if (mListView instanceof ListView) {
				mListView.setAdapter(mAdapter);
			} else if (mListView instanceof GridView) {
				mListView.setAdapter(mAdapter);
			} else {
				try {
					Method m = mListView.getClass().getDeclaredMethod("setAdapter", Adapter.class);
					m.invoke(mListView, mAdapter);
				} catch (Exception e) {
					e.printStackTrace();
				}
			}
		}
	}

	public AbsListView getListView() {
		return mListView;
	}

	public void setOnItemClickListener(OnItemClickListener listener) {
		mListView.setOnItemClickListener(listener);
	}

	public void setSelector(Drawable mDropDownListHighlight) {
		mListView.setSelector(mDropDownListHighlight);
	}

	public int getCount() {
		return mListView.getCount();
	}

    public void setSelection(int position) {
        mListSelectionHidden = false;
		mListView.setSelection(position);
	}
	
    public int measureHeightOfChildren(int widthMeasureSpec, int startPosition, int endPosition,
            final int maxHeight, int disallowPartialChildPosition) {

        int returnedHeight = getHeaderHeight(widthMeasureSpec);
        returnedHeight += mListView.getListPaddingTop() + mListView.getListPaddingBottom();
        
        final ListAdapter adapter = mListView.getAdapter();
        if (adapter == null) {
			return returnedHeight;
        }

        // Include the padding of the list
        final int dividerHeight = getDividerHeight();
        // The previous height value that was less than maxHeight and contained
        // no partial children
        int prevHeightWithoutPartialChild = 0;
        int i;
        View child;

        // mItemCount - 1 since endPosition parameter is inclusive
        endPosition = (endPosition == -1/*NO_POSITION*/) ? adapter.getCount() - 1 : endPosition;

        int rowHeight = 0;
        
        int numcolumns = getNumColumns(widthMeasureSpec);
        
        returnedHeight -= dividerHeight - 1;
        
		for (i = startPosition; i <= endPosition; ++i) {
            child = adapter.getView(i, null, mListView);
            if (mListView.getCacheColorHint() != 0) {
                child.setDrawingCacheBackgroundColor(mListView.getCacheColorHint());
            }

            measureScrapChild(child, i, widthMeasureSpec);

            
        	int t = child.getMeasuredHeight() + dividerHeight;
            if (t > rowHeight) {
				rowHeight = t;
            }

            if (i % numcolumns == 0) {
            	returnedHeight += rowHeight;
            	rowHeight = 0;
            }
            
            if (returnedHeight >= maxHeight) {
                // We went over, figure out which height to return.  If returnedHeight > maxHeight,
                // then the i'th position did not fit completely.
                return (disallowPartialChildPosition >= 0) // Disallowing is enabled (> -1)
                            && (i > disallowPartialChildPosition) // We've past the min pos
                            && (prevHeightWithoutPartialChild > 0) // We have a prev height
                            && (returnedHeight != maxHeight) // i'th child did not fit completely
                        ? prevHeightWithoutPartialChild
                        : maxHeight;
            }

            if ((disallowPartialChildPosition >= 0) && (i >= disallowPartialChildPosition)) {
                prevHeightWithoutPartialChild = returnedHeight;
            }
        }

        // At this point, we went through the range of children, and they each
        // completely fit, so return the returnedHeight
        return returnedHeight;
    }

	@SuppressLint("NewApi")
	protected int getNumColumns(int widthMeasureSpec) {
        if (mListView instanceof GridView) {
        	GridView gridView = (GridView)mListView;
        	int t;
        	if (widthMeasureSpec > 0 && MeasureSpec.getMode(widthMeasureSpec) == MeasureSpec.UNSPECIFIED) {
        		t = MeasureSpec.makeMeasureSpec(widthMeasureSpec, MeasureSpec.EXACTLY);
        	} else {
        		t = widthMeasureSpec;
        	}
        	gridView.measure(t, LayoutParams.WRAP_CONTENT);
			int horizontalSpacing;
			int columnWidth;
        	if (Build.VERSION.SDK_INT >= 16) {
				horizontalSpacing = gridView.getHorizontalSpacing();
				columnWidth = gridView.getColumnWidth();
        	} else {
        		horizontalSpacing = getPrivateFieldInt(gridView, "mHorizontalSpacing", Math.round(4 * getDensity()));
				columnWidth = getPrivateFieldInt(gridView, "mColumnWidth", Math.round(48 * getDensity()));
        	}
			int vw = gridView.getMeasuredWidth() + horizontalSpacing;
			int iw = columnWidth + horizontalSpacing;
			
			return vw / iw;
        } else {
        	return 1;
        }
	}

	@SuppressLint("NewApi")
	protected int getDividerHeight() {
		if (mListView instanceof ListView) {
			ListView lw = (ListView)mListView;
			if ((lw.getDividerHeight() > 0) && lw.getDivider() != null) 
				return lw.getDividerHeight();
		} else if (mListView instanceof GridView) {
        	GridView gridView = (GridView)mListView;
        	if (Build.VERSION.SDK_INT >= 16) {
        		return gridView.getVerticalSpacing();
        	} else {
				return getPrivateFieldInt(gridView, "mVerticalSpacing", Math.round(4 * getDensity()));
        	}
		}
		
		return 0;
	}

	private int getPrivateFieldInt(Object obj, String field, int defaultValue) {
		try { 
			Field f = obj.getClass().getDeclaredField(field); 
			f.setAccessible(true);
			return f.getInt(obj);
		} catch (Exception e) {
			e.printStackTrace();
			return defaultValue;
		}
	}

	private float getDensity() {
		return mListView.getContext().getResources().getDisplayMetrics().density;
	}
    
    private void measureScrapChild(View child, int position, int widthMeasureSpec) {
    	AbsListView.LayoutParams p = (AbsListView.LayoutParams) child.getLayoutParams();
        if (p == null) {
            p = new AbsListView.LayoutParams(ViewGroup.LayoutParams.MATCH_PARENT,
                    ViewGroup.LayoutParams.WRAP_CONTENT, 0);
            child.setLayoutParams(p);
        }
        //XXX p.viewType = mAdapter.getItemViewType(position);
        //XXX p.forceAdd = true;

        int childWidthSpec = ViewGroup.getChildMeasureSpec(widthMeasureSpec,
        		mListView.getPaddingLeft() + mListView.getPaddingRight(), p.width);
        int lpHeight = p.height;
        int childHeightSpec;
        if (lpHeight > 0) {
            childHeightSpec = MeasureSpec.makeMeasureSpec(lpHeight, MeasureSpec.EXACTLY);
        } else {
            childHeightSpec = MeasureSpec.makeMeasureSpec(0, MeasureSpec.UNSPECIFIED);
        }
        child.measure(childWidthSpec, childHeightSpec);
    }
}
