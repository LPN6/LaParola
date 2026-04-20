package net.laparola.ui.android.ignspinner;

import static android.view.ViewGroup.LayoutParams.MATCH_PARENT;
import static android.view.ViewGroup.LayoutParams.WRAP_CONTENT;

import net.laparola.R;
import net.laparola.ui.android.ignspinner.IgnAbsSpinner.SpinnerPopup;
import android.content.Context;
import android.database.DataSetObserver;
import android.graphics.drawable.Drawable;
import android.util.AttributeSet;
import android.view.View;
import android.view.ViewGroup;
import android.view.ViewParent;
import android.view.View.MeasureSpec;
import android.widget.AbsListView;
import android.widget.AdapterView;
import android.widget.LinearLayout;
import android.widget.ListAdapter;
import android.widget.ListView;
import android.widget.PopupWindow;
import android.widget.SpinnerAdapter;

import androidx.core.content.ContextCompat;

public abstract class IgnDropdownPopup extends IgnPopupWindow implements SpinnerPopup {
	protected class SpinnerResizePopupRunnable extends ResizePopupRunnable {
        public void run() {
        	final AbsListView listView = getListView();
			if (listView != null && listView.getCount() > listView.getChildCount() &&
					listView.getChildCount() <= mListItemExpandMaximum) {
                mPopup.setInputMethodMode(PopupWindow.INPUT_METHOD_NOT_NEEDED);
                show();
            }
        }
    }
	
	protected class SpinnerShowPopupRunnable extends ShowPopupRunnable {
        public void run() {
        	final AbsListView listView = getListView();
			listView.setSelection(ListView.INVALID_POSITION);
	        
	        if (!mModal || listView.isInTouchMode()) {
	            clearListSelection();
	        }
	        
	        if (!mModal) {
	            mHandler.post(mHideSelector);
	        }
        }
    }
	
    class ListSelectorHider implements Runnable {
        public void run() {
            clearListSelection();
        }
    }
    
    protected IgnHijackFocusListView mDropDownList;
	protected IgnAbsSpinner mIgnSpinner;
	protected ListSelectorHider mHideSelector = new ListSelectorHider();
	protected PopupScrollListener mScrollListener = new PopupScrollListener();
	protected CharSequence mHintText;
    protected AdapterView.OnItemClickListener mItemClickListener;
    protected AdapterView.OnItemSelectedListener mItemSelectedListener;
	protected int mPromptPosition = POSITION_PROMPT_ABOVE;
	protected Drawable mDropDownListHighlight;
	protected View mPromptView;
	protected ListAdapter mAdapter;

	public IgnDropdownPopup(Context context, AttributeSet attrs, int defStyleAttr, IgnAbsSpinner ignSpinner) {
		super(context, attrs, defStyleAttr);
		mIgnSpinner = ignSpinner;

        setAnchorView(mIgnSpinner);
        setModal(true);
        setPromptPosition(POSITION_PROMPT_ABOVE);
        setOnItemClickListener((parent, v, position, id) -> {
            //mAdapter.sendChanged();
            mIgnSpinner.setSelection(position);
            dismiss();
            mIgnSpinner.requestLayout();
            mIgnSpinner.invalidate();
            mIgnSpinner.selectionChanged();
        });
    }

	@Override
    public void show() {
        boolean wasShown = isShowing();

        // Force opaque background that respects day/night mode
        mPopup.setBackgroundDrawable(
                ContextCompat.getDrawable(mContext, R.drawable.spinner_popup_background)
        );

        final int spinnerPaddingLeft = mIgnSpinner.getPaddingLeft();
        final int spinnerPaddingRight = mIgnSpinner.getPaddingRight();
        final int spinnerWidth = mIgnSpinner.getWidth();
        
        if (mDropDownWidth == WRAP_CONTENT) {
            setContentWidth(Math.max(
            		mIgnSpinner.measureContentWidth((SpinnerAdapter) mAdapter, mIgnSpinner.getBackground()),
                    Math.max(spinnerWidth - spinnerPaddingLeft - spinnerPaddingRight,
                    mIgnSpinner.getMinimumPopupWidth())));
        } else if (mDropDownWidth == MATCH_PARENT) {
            setContentWidth(spinnerWidth - spinnerPaddingLeft - spinnerPaddingRight);
        } else {
            //setContentWidth(mDropDownWidth);
        }
        
        final Drawable background = mIgnSpinner.getBackground();
        int bgOffset = 0;
        if (background != null) {
            background.getPadding(mTempRect);
            bgOffset = -mTempRect.left;
        }
        setHorizontalOffset(bgOffset + spinnerPaddingLeft);
        setInputMethodMode(PopupWindow.INPUT_METHOD_NOT_NEEDED);
        super.show();
        setChoiceMode();
        if (!wasShown)
            mDropDownList.setSelection(mIgnSpinner.getSelectedItemPosition());
        //mIgnSpinner.setSelection(mIgnSpinner.getSelectedItemPosition());
    }

	private void setChoiceMode() {
		AbsListView listView = getListView();
        listView.setChoiceMode(ListView.CHOICE_MODE_SINGLE);
    }

	@Override
	protected void initRunnables() {
    	mResizePopupRunnable = new SpinnerResizePopupRunnable();
    	mShowPopupRunnable = new SpinnerShowPopupRunnable();
	}

    private class PopupDataSetObserver extends DataSetObserver {
        @Override
        public void onChanged() {
            if (isShowing()) {
                // Resize the popup to fit new content
                show();
            }
        }

        @Override
        public void onInvalidated() {
            dismiss();
        }
    }

    public void setAdapter(ListAdapter adapter) {
        if (mObserver == null) {
            mObserver = new PopupDataSetObserver();
        } else if (mAdapter != null) {
            mAdapter.unregisterDataSetObserver(mObserver);
        }
        mAdapter = adapter;
        if (mAdapter != null) {
            adapter.registerDataSetObserver(mObserver);
        }

        if (mDropDownList != null) {
            mDropDownList.setAdapter(mAdapter);
        }
    }

	public void setPromptPosition(int position) {
	    mPromptPosition = position;
	}

    public void setOnItemClickListener(AdapterView.OnItemClickListener clickListener) {
        mItemClickListener = clickListener;
    }

    public CharSequence getHintText() {
        return mHintText;
    }

    public void setPromptText(CharSequence hintText) {
        // Hint text is ignored for dropdowns, but maintain it here.
        mHintText = hintText;
    }
    
    @Override
    public void dismiss() {
	    super.dismiss();
	    if (mPromptView != null) {
	        final ViewParent parent = mPromptView.getParent();
	        if (parent instanceof ViewGroup group) {
                group.removeView(mPromptView);
	        }
	    }
	    if (mDropDownList != null) {
	        final ViewParent parent = mDropDownList.getParent();
        if (parent instanceof ViewGroup group) {
                group.removeView(mDropDownList);
	        }
    	    mDropDownList = null;
	    }
    }    

    public void clearListSelection() {
        final IgnHijackFocusListView list = mDropDownList;
        if (list != null) {
            // WARNING: Please read the comment where mListSelectionHidden is declared
            list.mListSelectionHidden = true;
            //XXX list.hideSelector();
            list.requestLayout();
        }
    }

    public AbsListView getListView() {
        return mDropDownList.getListView();
    }

    @Override
	protected int buildDropDown(int popupWidthSpec) {
	    ViewGroup dropDownView;
	    int otherHeights = 0;
	
	    if (mDropDownList == null) {
	        Context context = mContext;
	
	        mDropDownList = createListView(context, !mModal);
	        if (mDropDownListHighlight != null) {
	            mDropDownList.setSelector(mDropDownListHighlight);
	        }
	        mDropDownList.setAdapter(mAdapter);
	        mDropDownList.setOnItemClickListener(mItemClickListener);
	        mDropDownList.setFocusable(true);
	        mDropDownList.setFocusableInTouchMode(true);
	        mDropDownList.setOnItemSelectedListener(new AdapterView.OnItemSelectedListener() {
	            public void onItemSelected(AdapterView<?> parent, View view,
	                    int position, long id) {
	
	                if (position != -1) {
	                	IgnHijackFocusListView dropDownList = mDropDownList;
	
	                    if (dropDownList != null) {
	                        dropDownList.mListSelectionHidden = false;
	                    }
	                }
	            }
	
	            public void onNothingSelected(AdapterView<?> parent) {
			    	dismiss();
	            }
	        });
	        mDropDownList.setOnScrollListener(mScrollListener);
	        
	        if (mItemSelectedListener != null) {
	            mDropDownList.setOnItemSelectedListener(mItemSelectedListener);
	        }
	
	        dropDownView = mDropDownList;
	
	        View hintView = mPromptView;
	        if (hintView != null) {
	            // if a hint has been specified, we accomodate more space for it and
	            // add a text view in the drop-down menu, at the bottom of the list
	            LinearLayout hintContainer = new LinearLayout(context);
	            hintContainer.setOrientation(LinearLayout.VERTICAL);
	
	            LinearLayout.LayoutParams hintParams = new LinearLayout.LayoutParams(
	                    ViewGroup.LayoutParams.MATCH_PARENT, 0, 1.0f
	            );
	
	            switch (mPromptPosition) {
	            case POSITION_PROMPT_BELOW:
	                hintContainer.addView(dropDownView, hintParams);
	                hintContainer.addView(hintView);
	                break;
	
	            case POSITION_PROMPT_ABOVE:
	                hintContainer.addView(hintView);
	                hintContainer.addView(dropDownView, hintParams);
	                break;
	
	            default:
	                break;
	            }
	
	            // measure the hint's height to find how much more vertical space
	            // we need to add to the drop-down's height
	            int widthSpec = MeasureSpec.makeMeasureSpec(mDropDownWidth, MeasureSpec.AT_MOST);
	            int heightSpec = MeasureSpec.UNSPECIFIED;
	            hintView.measure(widthSpec, heightSpec);
	
	            hintParams = (LinearLayout.LayoutParams) hintView.getLayoutParams();
	            otherHeights = hintView.getMeasuredHeight() + hintParams.topMargin + hintParams.bottomMargin;
	
	            dropDownView = hintContainer;
	        }
	
	        mPopup.setContentView(dropDownView);
	    } else {
	        dropDownView = (ViewGroup) mPopup.getContentView();
	        final View view = mPromptView;
	        if (view != null) {
	            LinearLayout.LayoutParams hintParams =
	                    (LinearLayout.LayoutParams) view.getLayoutParams();
	            otherHeights = view.getMeasuredHeight() + hintParams.topMargin
	                    + hintParams.bottomMargin;
	        }
	    }
	
	    // getMaxAvailableHeight() subtracts the padding, so we put it back
	    // to get the available height for the whole window
	    int padding = 0;
	    Drawable background = mPopup.getBackground();
	    if (background != null) {
	        background.getPadding(mTempRect);
	        padding = mTempRect.top + mTempRect.bottom;
	
	        // If we don't have an explicit vertical offset, determine one from the window
	        // background so that content will line up.
	        if (!mDropDownVerticalOffsetSet) {
	            mDropDownVerticalOffset = -mTempRect.top;
	        }
	    }
	
	    // Max height available on the screen for a popup.
	    boolean ignoreBottomDecorations =
	            mPopup.getInputMethodMode() == PopupWindow.INPUT_METHOD_NOT_NEEDED;
	    final int maxHeight = /*mPopup.*/getMaxAvailableHeight(
	            mDropDownAnchorView, mDropDownVerticalOffset, ignoreBottomDecorations);
	
	    if (mDropDownHeight == ViewGroup.LayoutParams.MATCH_PARENT) {
	        return maxHeight + padding;
	    }
	
	    final int listContent = mDropDownList.measureHeightOfChildren(popupWidthSpec,
	            0, -1/*ListView.NO_POSITION*/, maxHeight - otherHeights, -1);
	    // add padding only if the list has items in it, that way we don't show
	    // the popup if it is not needed
	    if (listContent > 0) otherHeights += padding;
		    
	    return listContent + otherHeights;
	}

	protected abstract IgnHijackFocusListView createListView(Context context, boolean hijackfocus);
}