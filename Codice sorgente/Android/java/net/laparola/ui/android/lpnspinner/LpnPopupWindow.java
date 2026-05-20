package net.laparola.ui.android.lpnspinner;

import android.content.Context;
import android.content.res.Resources;
import android.database.DataSetObserver;
import android.graphics.Canvas;
import android.graphics.ColorFilter;
import android.graphics.PixelFormat;
import android.graphics.Rect;
import android.graphics.drawable.Drawable;
import android.os.Handler;
import android.util.AttributeSet;
import android.view.Gravity;
import android.view.View;
import android.view.View.OnTouchListener;
import android.view.MotionEvent;
import android.view.ViewGroup;
import android.widget.AbsListView;
import android.widget.ListView;
import android.widget.PopupWindow;

import androidx.annotation.NonNull;

public abstract class LpnPopupWindow {
    public class ResizePopupRunnable implements Runnable {
        public void run() {
            show();
        }
    }

    public static class ShowPopupRunnable implements Runnable {
        public void run() {
        }
    }

    class PopupTouchInterceptor implements OnTouchListener {
        public boolean onTouch(View v, MotionEvent event) {
            final int action = event.getAction();
            final int x = (int) event.getX();
            final int y = (int) event.getY();

            if (action == MotionEvent.ACTION_DOWN &&
                    mPopup != null && mPopup.isShowing() &&
                    (x >= 0 && x < mPopup.getWidth() && y >= 0 && y < mPopup.getHeight())) {
                mHandler.postDelayed(mResizePopupRunnable, EXPAND_LIST_TIMEOUT);
            } else if (action == MotionEvent.ACTION_UP) {
                mHandler.removeCallbacks(mResizePopupRunnable);
            }
            return false;
        }
    }

    public class PopupScrollListener implements ListView.OnScrollListener {
        public void onScroll(AbsListView view, int firstVisibleItem, int visibleItemCount,
                             int totalItemCount) {

        }

        public void onScrollStateChanged(AbsListView view, int scrollState) {
            if (scrollState == SCROLL_STATE_TOUCH_SCROLL &&
                    !isInputMethodNotNeeded() && mPopup.getContentView() != null) {
                mHandler.removeCallbacks(mResizePopupRunnable);
                mResizePopupRunnable.run();
            }
        }
    }

    /**
     * This value controls the length of time that the user
     * must leave a pointer down without scrolling to expand
     * the autocomplete dropdown list to cover the IME.
     */
    protected static final int EXPAND_LIST_TIMEOUT = 250;
    public static final int POSITION_PROMPT_ABOVE = 0;
    public static final int POSITION_PROMPT_BELOW = 1;

    protected Context mContext;
    protected PopupWindow mPopup;
    protected int mDropDownHeight = ViewGroup.LayoutParams.WRAP_CONTENT;
    protected int mDropDownWidth = ViewGroup.LayoutParams.WRAP_CONTENT;
    protected int mDropDownHorizontalOffset;
    protected int mDropDownVerticalOffset;
    protected int mListItemExpandMaximum = Integer.MAX_VALUE;
    protected DataSetObserver mObserver;
    protected View mDropDownAnchorView;
    protected ResizePopupRunnable mResizePopupRunnable;
    protected ShowPopupRunnable mShowPopupRunnable;
    private final PopupTouchInterceptor mTouchInterceptor = new PopupTouchInterceptor();
    protected Handler mHandler = new Handler();
    protected Rect mTempRect = new Rect();
    protected boolean mModal;
    protected boolean mDropDownVerticalOffsetSet;

    protected boolean mCentered = false;

    public LpnPopupWindow(Context context) {
        this(context, null, 0);

        initRunnables();
    }

    public LpnPopupWindow(Context context, AttributeSet attrs, int defStyleAttr) {
        mContext = context;

        mPopup = new PopupWindow(context, attrs, defStyleAttr);
        mPopup.setInputMethodMode(PopupWindow.INPUT_METHOD_NEEDED);

        initRunnables();
    }

    public LpnPopupWindow(Context context, AttributeSet attrs, int defStyleAttr, int defStyleRes) {
        mContext = context;
        mPopup = new PopupWindow(context, attrs, defStyleAttr, defStyleRes);
        mPopup.setInputMethodMode(PopupWindow.INPUT_METHOD_NEEDED);

        initRunnables();
    }

    protected void initRunnables() {
        mResizePopupRunnable = new ResizePopupRunnable();
        mShowPopupRunnable = new ShowPopupRunnable();
    }

    public void setModal(boolean modal) {
        mModal = true;
        mPopup.setFocusable(modal);
    }

    public void setBackgroundDrawable(Drawable d) {
        mPopup.setBackgroundDrawable(d);
    }

    public void setAnchorView(View anchor) {
        mDropDownAnchorView = anchor;
    }

    public void setHorizontalOffset(int offset) {
        mDropDownHorizontalOffset = offset;
    }

    public void setVerticalOffset(int offset) {
        mDropDownVerticalOffset = offset;
        mDropDownVerticalOffsetSet = true;
    }

    public void setContentWidth(int width) {
        Drawable popupBackground = mPopup.getBackground();
        if (popupBackground != null) {
            popupBackground.getPadding(mTempRect);
            mDropDownWidth = mTempRect.left + mTempRect.right + width;
        } else {
            mDropDownWidth = width;
        }
    }

    public void setContentHeight(int heigth) {
        Drawable popupBackground = mPopup.getBackground();
        if (popupBackground != null) {
            popupBackground.getPadding(mTempRect);
            mDropDownHeight = mTempRect.top + mTempRect.bottom + heigth;
        } else {
            mDropDownHeight = heigth;
        }
    }

    protected abstract int buildDropDown(int popupWidthSpec);

    public void show() {
        int height = buildDropDown(mDropDownWidth);

        int widthSpec = 0;
        int heightSpec = 0;

        boolean noInputMethod = isInputMethodNotNeeded();
        //XXX mPopup.setAllowScrollingAnchorParent(!noInputMethod);

        if (mPopup.isShowing()) {
            if (mDropDownWidth == ViewGroup.LayoutParams.MATCH_PARENT) {
                // The call to PopupWindow's update method below can accept -1 for any
                // value you do not want to update.
                widthSpec = -1;
            } else if (mDropDownWidth == ViewGroup.LayoutParams.WRAP_CONTENT) {
                widthSpec = mDropDownAnchorView.getWidth();
            } else {
                widthSpec = mDropDownWidth;
            }

            if (mDropDownHeight == ViewGroup.LayoutParams.MATCH_PARENT) {
                // The call to PopupWindow's update method below can accept -1 for any
                // value you do not want to update.
                heightSpec = noInputMethod ? height : ViewGroup.LayoutParams.MATCH_PARENT;
                if (noInputMethod) {
                    mPopup.setWindowLayoutMode(
                            mDropDownWidth == ViewGroup.LayoutParams.MATCH_PARENT ?
                                    ViewGroup.LayoutParams.MATCH_PARENT : 0, 0);
                } else {
                    mPopup.setWindowLayoutMode(
                            mDropDownWidth == ViewGroup.LayoutParams.MATCH_PARENT ?
                                    ViewGroup.LayoutParams.MATCH_PARENT : 0,
                            ViewGroup.LayoutParams.MATCH_PARENT);
                }
            } else if (mDropDownHeight == ViewGroup.LayoutParams.WRAP_CONTENT) {
                heightSpec = height;
            } else {
                heightSpec = mDropDownHeight;
            }

            mPopup.setOutsideTouchable(true);

            mPopup.update(mDropDownAnchorView, mDropDownHorizontalOffset,
                    mDropDownVerticalOffset, widthSpec, heightSpec);
        } else {
            //XXX mPopup.setClipToScreenEnabled(true);

            // per correggere un bug di Android: se non c'è sfondo
            // onTouchInterceptor non funziona e ci sono altri problemi
            if (mPopup.getBackground() == null) {
                mPopup.setBackgroundDrawable(new Drawable() {
                    @Override
                    public void draw(@NonNull Canvas canvas) {
                    }

                    @Override
                    public int getOpacity() {
                        return PixelFormat.UNKNOWN;
                    }

                    @Override
                    public void setAlpha(int alpha) {
                    }

                    @Override
                    public void setColorFilter(ColorFilter cf) {
                    }
                });
            }

            // use outside touchable to dismiss drop down when touching outside of it, so
            // only set this if the dropdown is not always visible
            mPopup.setOutsideTouchable(true);
            mPopup.setTouchInterceptor(mTouchInterceptor);

            if (mCentered) {
                mPopup.setWidth(ViewGroup.LayoutParams.MATCH_PARENT);
                mPopup.setHeight(ViewGroup.LayoutParams.WRAP_CONTENT);
                mPopup.showAtLocation(mDropDownAnchorView, Gravity.CENTER, 0, 0);
            } else {
                if (mDropDownWidth == ViewGroup.LayoutParams.MATCH_PARENT) {
                    widthSpec = ViewGroup.LayoutParams.MATCH_PARENT;
                } else {
                    if (mDropDownWidth == ViewGroup.LayoutParams.WRAP_CONTENT) {
                        mPopup.setWidth(mDropDownAnchorView.getWidth());
                    } else {
                        mPopup.setWidth(mDropDownWidth);
                    }
                }

                if (mDropDownHeight == ViewGroup.LayoutParams.MATCH_PARENT) {
                    heightSpec = ViewGroup.LayoutParams.MATCH_PARENT;
                } else {
                    if (mDropDownHeight == ViewGroup.LayoutParams.WRAP_CONTENT) {
                        mPopup.setHeight(height);
                    } else {
                        mPopup.setHeight(mDropDownHeight);
                    }
                }

                mPopup.setWindowLayoutMode(widthSpec, heightSpec);

                mPopup.showAsDropDown(mDropDownAnchorView,
                        mDropDownHorizontalOffset, mDropDownVerticalOffset);
            }

            mShowPopupRunnable.run();
        }
    }

    public void dismiss() {
        mPopup.dismiss();
        mPopup.setContentView(null);
        mHandler.removeCallbacks(mResizePopupRunnable);
    }

    public void setOnDismissListener(PopupWindow.OnDismissListener listener) {
        mPopup.setOnDismissListener(listener);
    }

    public void setInputMethodMode(int mode) {
        mPopup.setInputMethodMode(mode);
    }

    public boolean isShowing() {
        return mPopup.isShowing();
    }

    protected boolean isInputMethodNotNeeded() {
        return mPopup.getInputMethodMode() == PopupWindow.INPUT_METHOD_NOT_NEEDED;
    }

    protected int getMaxAvailableHeight(View anchor, int yOffset, boolean ignoreBottomDecorations) {
        final Rect displayFrame = new Rect();
        anchor.getWindowVisibleDisplayFrame(displayFrame);

        final int[] anchorPos = new int[2];
        anchor.getLocationOnScreen(anchorPos);

        int bottomEdge = displayFrame.bottom;
        if (ignoreBottomDecorations) {
            Resources res = anchor.getContext().getResources();
            bottomEdge = res.getDisplayMetrics().heightPixels;
        }
        final int distanceToBottom = bottomEdge - (anchorPos[1] + anchor.getHeight()) - yOffset;
        final int distanceToTop = anchorPos[1] - displayFrame.top + yOffset;

        // anchorPos[1] is distance from anchor to top of screen
        int returnedHeight = Math.max(distanceToBottom, distanceToTop);
        if (mPopup.getBackground() != null) {
            mPopup.getBackground().getPadding(mTempRect);
            returnedHeight -= mTempRect.top + mTempRect.bottom;
        }
        return returnedHeight;
    }

    public boolean isCentered() {
        return mCentered;
    }

    public void setCentered(boolean mCentered) {
        this.mCentered = mCentered;
    }
}