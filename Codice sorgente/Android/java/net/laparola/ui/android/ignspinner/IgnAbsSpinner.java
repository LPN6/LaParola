/*
 * Copyright (C) 2007 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

package net.laparola.ui.android.ignspinner;

import net.laparola.R;

import android.content.Context;
import android.content.DialogInterface;
import android.content.DialogInterface.OnClickListener;
import android.content.res.Resources;
import android.content.res.TypedArray;
import android.graphics.Rect;
import android.graphics.drawable.Drawable;
import android.util.AttributeSet;
import android.util.TypedValue;
import android.view.Gravity;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.ListAdapter;
import android.widget.SpinnerAdapter;


/**
 * A view that displays one child at a time and lets the user pick among them.
 * The items in the Spinner come from the Adapter associated with
 * this view.
 *
 * <p>See the <a href="{@docRoot}resources/tutorials/views/hello-spinner.html">Spinner
 * tutorial</a>.</p>
 *
 * attr ref android.R.styleable#Spinner_prompt
 */
public abstract class IgnAbsSpinner extends IgnAbsAbsSpinner implements OnClickListener {
    //private static final String TAG = "Spinner";

    // Only measure this many items to get a decent max width.
    private static final int MAX_ITEMS_MEASURED = 200; //15;

    /*
     * Use a dialog window for selecting spinner options.
     */
    //public static final int MODE_DIALOG = 0;

    /**
     * Use a dropdown anchored to the Spinner for selecting spinner options.
     */
    public static final int MODE_DROPDOWN = 1;

    /**
     * Use the theme-supplied value to select the dropdown mode.
     */
    //private static final int MODE_THEME = -1;

    protected SpinnerPopup mPopup;
    private IgnDropDownAdapter mTempAdapter;
    int mDropDownWidth;

    private int mGravity;
    private final boolean mDisableChildrenWhenDisabled;

    private final Rect mTempRect = new Rect();

    public abstract IgnDropdownPopup createPopup(Context context, AttributeSet attrs, int defStyle);

    public IgnAbsSpinner(Context context, AttributeSet attrs) {
        this(context, attrs, R.attr.actionDropDownStyle);
    }

    /**
     * Construct a new spinner with the given context's theme, the supplied attribute set,
     * and default style.
     *
     * @param context The Context the view is running in, through which it can
     *        access the current theme, resources, etc.
     * @param attrs The attributes of the XML tag that is inflating the view.
     * @param defStyle The default style to apply to this view. If 0, no style
     *        will be applied (beyond what is included in the theme). This may
     *        either be an attribute resource, whose value will be retrieved
     *        from the current theme, or an explicit style resource.
     */
    public IgnAbsSpinner(Context context, AttributeSet attrs, int defStyle) {
        super(context, attrs, defStyle);

        TypedArray a = context.obtainStyledAttributes(attrs,
                new int[] {
                        android.R.attr.dropDownWidth,
                        android.R.attr.popupBackground,
                        android.R.attr.dropDownVerticalOffset,
                        android.R.attr.dropDownHorizontalOffset,
                        android.R.attr.gravity,
                        android.R.attr.prompt
                },
                defStyle, 0);

        IgnDropdownPopup popup = createPopup(context, attrs, defStyle);

        mDropDownWidth = a.getLayoutDimension(0, ViewGroup.LayoutParams.WRAP_CONTENT);

        popup.setBackgroundDrawable(a.getDrawable(1));
        final int verticalOffset = a.getDimensionPixelOffset(2, 0);
        if (verticalOffset != 0) {
            popup.setVerticalOffset(verticalOffset);
        }

        final int horizontalOffset = a.getDimensionPixelOffset(3, 0);
        if (horizontalOffset != 0) {
            popup.setHorizontalOffset(horizontalOffset);
        }

        mPopup = popup;

        mGravity = a.getInt(4, Gravity.CENTER);
        mPopup.setPromptText(a.getString(5));

        mDisableChildrenWhenDisabled = true;

        a.recycle();

        // Base constructor can call setAdapter before we initialize mPopup.
        // Finish setting things up if this happened.
        if (mTempAdapter != null) {
            mPopup.setAdapter(mTempAdapter);
            mTempAdapter = null;
        }
    }

	@Override
    public void setEnabled(boolean enabled) {
        super.setEnabled(enabled);
        if (mDisableChildrenWhenDisabled) {
            final int count = getChildCount();
            for (int i = 0; i < count; i++) {
                getChildAt(i).setEnabled(enabled);
            }
        }
    }

    /**
     * Describes how the selected item view is positioned. Currently only the horizontal component
     * is used. The default is determined by the current theme.
     *
     * @param gravity See {@link android.view.Gravity}
     *
     * attr ref android.R.styleable#Spinner_gravity
     */
    public void setGravity(int gravity) {
        if (mGravity != gravity) {
            if ((gravity & Gravity.HORIZONTAL_GRAVITY_MASK) == 0) {
                gravity |= Gravity.START;
            }
            mGravity = gravity;
            requestLayout();
        }
    }

    @Override
    public void setAdapter(SpinnerAdapter adapter) {
        super.setAdapter(adapter);

        if (mPopup != null) {
            mPopup.setAdapter(new IgnDropDownAdapter(adapter));
        } else {
            mTempAdapter = new IgnDropDownAdapter(adapter);
        }
    }

    @Override
    public int getBaseline() {
        View child = null;

        if (getChildCount() > 0) {
            child = getChildAt(0);
        } else if (mAdapter != null && mAdapter.getCount() > 0) {
            child = makeAndAddView(0);
            mRecycler.put(0, child);
            removeAllViewsInLayout();
        }

        if (child != null) {
            final int childBaseline = child.getBaseline();
            return childBaseline >= 0 ? child.getTop() + childBaseline : -1;
        } else {
            return -1;
        }
    }

    @Override
    protected void onDetachedFromWindow() {
        super.onDetachedFromWindow();

        if (mPopup != null && mPopup.isShowing()) {
            mPopup.dismiss();
        }
    }

    /**
     * <p>A spinner does not support item click events. Calling this method
     * will raise an exception.</p>
     *
     * @param l this listener will be ignored
     */
    @Override
    public void setOnItemClickListener(OnItemClickListener l) {
        throw new RuntimeException("setOnItemClickListener cannot be used with a spinner.");
    }

    @Override
    protected void onMeasure(int widthMeasureSpec, int heightMeasureSpec) {
        super.onMeasure(widthMeasureSpec, heightMeasureSpec);
        if (mPopup != null && MeasureSpec.getMode(widthMeasureSpec) == MeasureSpec.AT_MOST) {
            final int measuredWidth = getMeasuredWidth();
            
            final int measuredContentWidth = measureContentWidth(getAdapter(), getBackground());
            
			setMeasuredDimension(Math.min(
					Math.max(measuredWidth, measuredContentWidth),
                    MeasureSpec.getSize(widthMeasureSpec)),
                getMeasuredHeight());
        }
    }

    /**
     * @see android.view.View#onLayout(boolean,int,int,int,int)
     *
     * Creates and positions all views
     *
     */
    @Override
    protected void onLayout(boolean changed, int l, int t, int r, int b) {
        super.onLayout(changed, l, t, r, b);
        mInLayout = true;
        layout(0, false);
        mInLayout = false;
    }

    /**
     * Creates and positions all views for this Spinner.
     *
     * @param delta Change in the selected position. +1 moves selection is moving to the right,
     * so views are scrolling to the left. -1 means selection is moving to the left.
     */
    @Override
    void layout(int delta, boolean animate) {
        int childrenLeft = mSpinnerPadding.left;
        int childrenWidth = getRight() - getLeft() - mSpinnerPadding.left - mSpinnerPadding.right;

        if (mDataChanged) {
            handleDataChanged();
        }

        // Handle the empty set by removing all views
        if (mItemCount == 0) {
            resetList();
            return;
        }

        if (mNextSelectedPosition >= 0) {
            setSelectedPositionInt(mNextSelectedPosition);
        }

        recycleAllViews();

        // Clear out old views
        removeAllViewsInLayout();

        // Make selected view and position it
        mFirstPosition = mSelectedPosition;
        View sel = makeAndAddView(mSelectedPosition);
        int width = sel.getMeasuredWidth();
        int selectedOffset = childrenLeft;
        selectedOffset = switch (mGravity & Gravity.HORIZONTAL_GRAVITY_MASK) {
            case Gravity.CENTER_HORIZONTAL -> childrenLeft + (childrenWidth / 2) - (width / 2);
            case Gravity.END -> childrenLeft + childrenWidth - width;
            default -> selectedOffset;
        };
        sel.offsetLeftAndRight(selectedOffset);

        // Flush any cached views that did not get reused above
        mRecycler.clear();

        invalidate();

        checkSelectionChanged();

        mDataChanged = false;
        mNeedSync = false;
        setNextSelectedPositionInt(mSelectedPosition);
    }

    /**
     * Obtain a view, either by pulling an existing view from the recycler or
     * by getting a new one from the adapter. If we are animating, make sure
     * there is enough information in the view's layout parameters to animate
     * from the old to new positions.
     *
     * @param position Position in the spinner for the view to obtain
     * @return A view that has been added to the spinner
     */
    private View makeAndAddView(int position) {

        View child;

        if (!mDataChanged) {
            child = mRecycler.get(position);
            if (child != null) {
                // Position the view
                setUpChild(child);

                return child;
            }
        }

        // Nothing found in the recycler -- ask the adapter for a view
        child = mAdapter.getView(position, null, this);

        // Position the view
        setUpChild(child);

        return child;
    }

    /**
     * Helper for makeAndAddView to set the position of a view
     * and fill out its layout paramters.
     *
     * @param child The view to position
     */
    private void setUpChild(View child) {

        // Respect layout params that are already in the view. Otherwise,
        // make some up...
        ViewGroup.LayoutParams lp = child.getLayoutParams();
        if (lp == null) {
            lp = generateDefaultLayoutParams();
        }

        addViewInLayout(child, 0, lp);

        child.setSelected(hasFocus());
        if (mDisableChildrenWhenDisabled) {
            child.setEnabled(isEnabled());
        }

        // Get measure specs
        int childHeightSpec = ViewGroup.getChildMeasureSpec(mHeightMeasureSpec,
                mSpinnerPadding.top + mSpinnerPadding.bottom, lp.height);
        int childWidthSpec = ViewGroup.getChildMeasureSpec(mWidthMeasureSpec,
                mSpinnerPadding.left + mSpinnerPadding.right, lp.width);

        // Measure child
        child.measure(childWidthSpec, childHeightSpec);

        int childLeft;
        int childRight;

        // Position vertically based on gravity setting
        int childTop = mSpinnerPadding.top
                + ((getMeasuredHeight() - mSpinnerPadding.bottom -
                        mSpinnerPadding.top - child.getMeasuredHeight()) / 2);
        int childBottom = childTop + child.getMeasuredHeight();

        int width = child.getMeasuredWidth();
        childLeft = 0;
        childRight = childLeft + width;

        child.layout(childLeft, childTop, childRight, childBottom);
    }

    @Override
    public boolean performClick() {
        boolean handled = super.performClick();

        if (!handled) {
            handled = true;

            if (!mPopup.isShowing()) {
                mPopup.show();
            }
        }

        return handled;
    }

    public void onClick(DialogInterface dialog, int which) {
        setSelection(which);
        dialog.dismiss();
    }

    /**
     * Sets the prompt to display when the dialog is shown.
     * @param prompt the prompt to set
     */
    public void setPrompt(CharSequence prompt) {
        mPopup.setPromptText(prompt);
    }

    /**
     * Sets the prompt to display when the dialog is shown.
     * @param promptId the resource ID of the prompt to display when the dialog is shown
     */
    public void setPromptId(int promptId) {
        setPrompt(getContext().getText(promptId));
    }

    /**
     * @return The prompt to display when the dialog is shown
     */
    public CharSequence getPrompt() {
        return mPopup.getHintText();
    }

    public int measureContentWidth(SpinnerAdapter adapter, Drawable background) {
        if (adapter == null) {
            return 0;
        }

        int width = 0;
        View itemView = null;
        int itemType = 0;
        final int widthMeasureSpec =
            MeasureSpec.makeMeasureSpec(0, MeasureSpec.UNSPECIFIED);
        final int heightMeasureSpec =
            MeasureSpec.makeMeasureSpec(0, MeasureSpec.UNSPECIFIED);

        // Make sure the number of items we'll measure is capped. If it's a huge data set
        // with wildly varying sizes, oh well.
        int start = Math.max(0, getSelectedItemPosition());
        final int end = Math.min(adapter.getCount(), start + MAX_ITEMS_MEASURED);
        final int count = end - start;
        start = Math.max(0, start - (MAX_ITEMS_MEASURED - count));
        for (int i = start; i < end; i++) {
            final int positionType = adapter.getItemViewType(i);
            if (positionType != itemType) {
                itemType = positionType;
                itemView = null;
            }
            itemView = adapter.getView(i, itemView, this);
            if (itemView.getLayoutParams() == null) {
                itemView.setLayoutParams(new ViewGroup.LayoutParams(
                        ViewGroup.LayoutParams.WRAP_CONTENT,
                        ViewGroup.LayoutParams.WRAP_CONTENT));
            }
            itemView.measure(widthMeasureSpec, heightMeasureSpec);
            width = Math.max(width, itemView.getMeasuredWidth());
        }

        // Add background padding to measured width
        if (background != null) {
            background.getPadding(mTempRect);
            width += mTempRect.left + mTempRect.right;
        }

        return width;
    }

    /**
     * Implements some sort of popup selection interface for selecting a spinner option.
     * Allows for different spinner modes.
     */
    public interface SpinnerPopup {
        void setAdapter(ListAdapter adapter);

        /**
         * Show the popup
         */
        void show();

        /**
         * Dismiss the popup
         */
        void dismiss();

        /**
         * @return true if the popup is showing, false otherwise.
         */
        boolean isShowing();

        /**
         * Set hint text to be displayed to the user. This should provide
         * a description of the choice being made.
         * @param hintText Hint text to set.
         */
        void setPromptText(CharSequence hintText);
        CharSequence getHintText();
    }

    /*
    private class DialogPopup implements SpinnerPopup, DialogInterface.OnClickListener {
        private AlertDialog mPopup;
        private ListAdapter mListAdapter;
        private CharSequence mPrompt;

        public void dismiss() {
            mPopup.dismiss();
            mPopup = null;
        }

        public boolean isShowing() {
            return mPopup != null ? mPopup.isShowing() : false;
        }

        public void setAdapter(ListAdapter adapter) {
            mListAdapter = adapter;
        }

        public void setPromptText(CharSequence hintText) {
            mPrompt = hintText;
        }

        public CharSequence getHintText() {
            return mPrompt;
        }

        public void show() {
            AlertDialog.Builder builder = new AlertDialog.Builder(getContext());
            if (mPrompt != null) {
                builder.setTitle(mPrompt);
            }
            mPopup = builder.setSingleChoiceItems(mListAdapter,
                    getSelectedItemPosition(), this).show();
        }

        public void onClick(DialogInterface dialog, int which) {
            setSelection(which);
            dismiss();
        }
    }
    */

    public int getMinimumPopupWidth() {
		Resources r = getResources();
		return Math.round(TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_DIP, 260, r.getDisplayMetrics()));
	}
	
	public boolean isPopupVisible() {
		return mPopup != null && mPopup.isShowing();
	}

	public boolean isPopupCentered() {
		if (!(mPopup instanceof IgnPopupWindow))
			return false;
		return ((IgnPopupWindow)mPopup).isCentered();
	}

	public void setPopupCentered(boolean value) {
		if (mPopup instanceof IgnPopupWindow)
			((IgnPopupWindow)mPopup).setCentered(value);
	}
}
