package net.laparola.ui.android;

import android.content.Context;
import android.util.AttributeSet;
import android.util.DisplayMetrics;
import android.util.TypedValue;
import android.view.View;
import android.widget.FrameLayout;
import android.widget.LinearLayout;

import net.laparola.R;

import androidx.core.content.ContextCompat;

public class FourPanesLayout extends LinearLayout {
    private final LinearLayout mLayoutNear;
    private final LinearLayout mLayoutSep;
    private final LinearLayout mLayoutFar;
    private final View[] mSeparators;
    private final int mGrayColor;
    private final int mBlueColor;
    private int mSepDim;

    private int mSelection;
    private final FrameLayout[] mFrames;
    private int[][] mSelectionSeparators;

    private LayoutParams mLayoutMM;
    private LayoutParams mLayoutM0;
    private LayoutParams mLayout0M;
    private LayoutParams mLayoutSepV;
    private LayoutParams mLayoutSepH;
    private LayoutParams mLayoutSepVF;
    private LayoutParams mLayoutSepC;
    private int mNumber;
    private final Context mContext;

    public FourPanesLayout(Context context, AttributeSet attrs) {
        super(context, attrs);
        mContext = context;

        mGrayColor = getThemeColor(context, R.attr.colorOnSurface);
        mBlueColor = getThemeColor(context, R.attr.colorPrimary);

        setSeparatorWidth(2);

        mLayoutNear = new LinearLayout(context);
        mLayoutSep = new LinearLayout(context);
        mLayoutFar = new LinearLayout(context);

        mSeparators = new View[7];
        for (int i = 0; i < mSeparators.length; i++) {
            mSeparators[i] = new View(context);
            mSeparators[i].setBackgroundColor(mGrayColor);
            mSeparators[i].setVisibility(View.VISIBLE);
        }

        mFrames = new FrameLayout[4];
        int c = getThemeColor(context, R.attr.colorSurface);
        for (int i = 0; i < mFrames.length; i++) {
            mFrames[i] = new FrameLayout(context);
            mFrames[i].setBackgroundColor(c);
        }

        mFrames[0].setId(net.laparola.R.id.frame0);
        mFrames[1].setId(net.laparola.R.id.frame1);
        mFrames[2].setId(net.laparola.R.id.frame2);
        mFrames[3].setId(net.laparola.R.id.frame3);

        for (int i = 0; i < mFrames.length; i++) {
            final int j = i;
            mFrames[i].setBackgroundColor(0);
            mFrames[i].setOnClickListener(v -> setSelectedPane(j));
        }

        setPanes(1, HORIZONTAL);
        setSelectedPane(0);
    }

    public static int getThemeColor(Context context, int attr) {
        TypedValue typedValue = new TypedValue();
        if (context.getTheme().resolveAttribute(attr, typedValue, true)) {
            return typedValue.data;
        } else {
            // fallback to a default color if attribute not found
            return ContextCompat.getColor(context, android.R.color.darker_gray);
        }
    }

    public void setSeparatorWidth(int dp) {
        DisplayMetrics displayMetrics = mContext.getResources().getDisplayMetrics();
        float px = TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_DIP, dp, displayMetrics);
        mSepDim = Math.round(px);
    }

    public void setPanes(int number, int orientation) {
        setOrientation(orientation == HORIZONTAL ? VERTICAL : HORIZONTAL);

        if (number >= 1 && number <= 4) {
            mNumber = number;

            switch (number) {
                case 1:
                    layout1();
                    break;
                case 2:
                    layout2();
                    break;
                case 3:
                    layout3();
                    break;
                case 4:
                default:
                    layout4();
                    break;
            }

            setSelectedPane(Math.min(mSelection, 3));
        }
    }

    public int getNumberPanes() {
        return mNumber;
    }

    private void resetContainers() {
        this.removeAllViews();
        mLayoutNear.removeAllViews();
        mLayoutSep.removeAllViews();
        mLayoutFar.removeAllViews();
    }

    private void layout1() {
        resetContainers();

        this.addView(mFrames[0], mLayoutMM);

        mSelectionSeparators = new int[][]{
                new int[]{},
                null,
                null,
                null,
        };
    }

    private void layout2() {
        resetContainers();

        mLayoutNear.addView(mSeparators[0], mLayoutSepH);
        mLayoutNear.addView(mFrames[0], mLayoutM0);
        mLayoutNear.addView(mSeparators[5], mLayoutSepH);

        mLayoutSep.addView(mSeparators[1], mLayoutSepVF);
        mLayoutSep.addView(mSeparators[2], mLayoutSepC);
        mLayoutSep.addView(mSeparators[3], mLayoutSepVF);

        mLayoutFar.addView(mSeparators[4], mLayoutSepH);
        mLayoutFar.addView(mFrames[1], mLayoutM0);
        mLayoutFar.addView(mSeparators[6], mLayoutSepH);

        this.addView(mLayoutNear, mLayout0M);
        this.addView(mLayoutSep, mLayoutSepV);
        this.addView(mLayoutFar, mLayout0M);

        mSelectionSeparators = new int[][]{
                new int[]{0, 1, 2, 3, 5},
                new int[]{1, 2, 3, 4, 6},
                null,
                null,
        };
    }

    private void layout3() {
        resetContainers();

        mLayoutNear.addView(mSeparators[0], mLayoutSepH);
        mLayoutNear.addView(mFrames[0], mLayoutM0);
        mLayoutNear.addView(mSeparators[5], mLayoutSepH);

        mLayoutSep.addView(mSeparators[1], mLayoutSepVF);
        mLayoutSep.addView(mSeparators[2], mLayoutSepC);
        mLayoutSep.addView(mSeparators[3], mLayoutSepVF);

        mLayoutFar.addView(mFrames[1], mLayoutM0);
        mLayoutFar.addView(mSeparators[4], mLayoutSepH);
        mLayoutFar.addView(mFrames[2], mLayoutM0);

        this.addView(mLayoutNear, mLayout0M);
        this.addView(mLayoutSep, mLayoutSepV);
        this.addView(mLayoutFar, mLayout0M);

        mSelectionSeparators = new int[][]{
                new int[]{0, 1, 2, 3, 5},
                new int[]{1, 2, 4},
                new int[]{2, 3, 4},
                null,
        };
    }

    private void layout4() {
        resetContainers();

        mLayoutNear.addView(mFrames[0], mLayoutM0);
        mLayoutNear.addView(mSeparators[0], mLayoutSepH);
        mLayoutNear.addView(mFrames[3], mLayoutM0);

        mLayoutSep.addView(mSeparators[1], mLayoutSepVF);
        mLayoutSep.addView(mSeparators[2], mLayoutSepC);
        mLayoutSep.addView(mSeparators[3], mLayoutSepVF);

        mLayoutFar.addView(mFrames[1], mLayoutM0);
        mLayoutFar.addView(mSeparators[4], mLayoutSepH);
        mLayoutFar.addView(mFrames[2], mLayoutM0);

        this.addView(mLayoutNear, mLayout0M);
        this.addView(mLayoutSep, mLayoutSepV);
        this.addView(mLayoutFar, mLayout0M);

        mSelectionSeparators = new int[][]{
                new int[]{0, 1, 2},
                new int[]{1, 2, 4},
                new int[]{2, 3, 4},
                new int[]{0, 2, 3},
        };
    }

    @Override
    public void setOrientation(int orientation) {
        super.setOrientation(orientation);

        //LayoutParams mLayoutSepHF;
        if (orientation == HORIZONTAL) {
            mLayoutMM = new LayoutParams(LayoutParams.MATCH_PARENT, LayoutParams.MATCH_PARENT);
            mLayoutM0 = new LayoutParams(LayoutParams.MATCH_PARENT, 0, 1);
            mLayout0M = new LayoutParams(0, LayoutParams.MATCH_PARENT, 1);
            mLayoutSepV = new LayoutParams(mSepDim, LayoutParams.MATCH_PARENT);
            mLayoutSepH = new LayoutParams(LayoutParams.MATCH_PARENT, mSepDim);
            mLayoutSepVF = new LayoutParams(mSepDim, 0, 1);
            //mLayoutSepHF = new LayoutParams(0, mSepDim, 1);
            mLayoutSepC = new LayoutParams(mSepDim, mSepDim);
        } else {
            mLayoutMM = new LayoutParams(LayoutParams.MATCH_PARENT, LayoutParams.MATCH_PARENT);
            mLayoutM0 = new LayoutParams(0, LayoutParams.MATCH_PARENT, 1);
            mLayout0M = new LayoutParams(LayoutParams.MATCH_PARENT, 0, 1);
            mLayoutSepV = new LayoutParams(LayoutParams.MATCH_PARENT, mSepDim);
            mLayoutSepH = new LayoutParams(mSepDim, LayoutParams.MATCH_PARENT);
            mLayoutSepVF = new LayoutParams(0, mSepDim, 1);
            //mLayoutSepHF = new LayoutParams(mSepDim, 0, 1);
            mLayoutSepC = new LayoutParams(mSepDim, mSepDim);
        }

        int norientation = (orientation == HORIZONTAL) ? VERTICAL : HORIZONTAL;
        mLayoutNear.setOrientation(norientation);
        mLayoutSep.setOrientation(norientation);
        mLayoutFar.setOrientation(norientation);
    }

/*    public int getSelectedPane() {        return mSelection;    }*/

    public void setSelectedPane(int selection) {
        if (selection < 0 || selection > 3)
            return;

        this.mSelection = selection;
        for (View mSeparator : mSeparators) {
            mSeparator.setBackgroundColor(mGrayColor);
        }
        if (mSelectionSeparators[selection] != null) {
            for (int j = 0; j < mSelectionSeparators[selection].length; j++) {
                mSeparators[mSelectionSeparators[selection][j]].setBackgroundColor(mBlueColor);
            }
        }
    }

    public int getFrameId(int n) {
        return mFrames[n].getId();
    }

    public FrameLayout getFrame(int n) {
        return mFrames[n];
    }
}
