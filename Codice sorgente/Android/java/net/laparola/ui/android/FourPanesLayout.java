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
	private LinearLayout mLayoutNear;
	private LinearLayout mLayoutSep;
	private LinearLayout mLayoutFar;
	private View[] mSeparators;
	private int mGrayColor;
	private int mBlueColor;
	private int mSepDim;
	
	private int mSelection;
	private FrameLayout[] mFrames;
	private int[][] mSelectionSeparators; 
	
	private LayoutParams mLayoutMM;
	private LayoutParams mLayoutM0;
	private LayoutParams mLayout0M;
	private LayoutParams mLayoutSepV;
	private LayoutParams mLayoutSepH;
	private LayoutParams mLayoutSepVF;
	@SuppressWarnings("unused")
	private LayoutParams mLayoutSepHF;
	private LayoutParams mLayoutSepC;
	private int mNumber;
	private Context mContext;
	
	public FourPanesLayout(Context context, AttributeSet attrs) {
		super(context, attrs);
		mContext = context;
		
		//Resources res = context.getResources();
		//mGrayColor = res.getColor(R.color.bright_foreground_disabled_holo_light);
		mGrayColor = ContextCompat.getColor(context, R.color.bright_foreground_disabled_holo_light);
		//mBlueColor = res.getColor(R.color.holo_blue_light);
		mBlueColor = ContextCompat.getColor(context, R.color.holo_blue_light);

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
		//int c = res.getColor(R.color.background_holo_light);
		int c = ContextCompat.getColor(context, R.color.background_holo_light);
		for (int i = 0; i < mFrames.length; i++) {
			mFrames[i] = new FrameLayout(context);
			mFrames[i].setBackgroundColor(c);
		}
		
		mFrames[0].setId(R.id.frame0);
		mFrames[1].setId(R.id.frame1);
		mFrames[2].setId(R.id.frame2);
		mFrames[3].setId(R.id.frame3);
		
		for (int i = 0; i < mFrames.length; i++) {
			final int j = i;
			mFrames[i].setBackgroundColor(0);
			mFrames[i].setOnClickListener(new OnClickListener() {
				@Override
				public void onClick(View v) {
					setSelectedPane(j);
				}
			});
		}

		/*
		mFrames[0].setBackgroundColor(0xffff0000);
		mFrames[1].setBackgroundColor(0xff00ff00);
		mFrames[2].setBackgroundColor(0xff0000ff);
		mFrames[3].setBackgroundColor(0xffffff00);
		*/

		setPanes(1, HORIZONTAL);
		setSelectedPane(0);
	}

	public void setSeparatorWidth(int dp) {
		DisplayMetrics displayMetrics = mContext.getResources().getDisplayMetrics();
		float px = TypedValue.applyDimension(TypedValue.COMPLEX_UNIT_DIP, dp, displayMetrics);
		mSepDim = Math.round(px);
	}
	
	public void setPanes (int number, int orientation) {
		setOrientation(orientation == HORIZONTAL ? VERTICAL : HORIZONTAL);
		
		if (number >= 1 && number <= 4) {
			mNumber = number;
			
			switch (number){
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
	
	public int getNumberPanes () {
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
		
		mSelectionSeparators = new int[][] { 
				new int[] {},
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
		
		mSelectionSeparators = new int[][] {
				new int[] {0, 1, 2, 3, 5},
				new int[] {1, 2, 3, 4, 6},
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
		
		mSelectionSeparators = new int[][] {
				new int[] {0, 1, 2, 3, 5},
				new int[] {1, 2, 4},
				new int[] {2, 3, 4},
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
		
		mSelectionSeparators = new int[][] {
				new int[] {0, 1, 2},
				new int[] {1, 2, 4},
				new int[] {2, 3, 4},
				new int[] {0, 2, 3},
		};
	}

	@Override
	public void setOrientation(int orientation) {
		super.setOrientation(orientation);
		
		if (orientation == HORIZONTAL) {
			mLayoutMM = new LayoutParams(LayoutParams.MATCH_PARENT, LayoutParams.MATCH_PARENT);
			mLayoutM0 = new LayoutParams(LayoutParams.MATCH_PARENT, 0, 1);
			mLayout0M = new LayoutParams(0, LayoutParams.MATCH_PARENT, 1);
			mLayoutSepV = new LayoutParams(mSepDim, LayoutParams.MATCH_PARENT);
			mLayoutSepH = new LayoutParams(LayoutParams.MATCH_PARENT, mSepDim);
			mLayoutSepVF = new LayoutParams(mSepDim, 0, 1);
			mLayoutSepHF = new LayoutParams(0, mSepDim, 1);
			mLayoutSepC = new LayoutParams(mSepDim, mSepDim);
		} else {
			mLayoutMM = new LayoutParams(LayoutParams.MATCH_PARENT, LayoutParams.MATCH_PARENT);
			mLayoutM0 = new LayoutParams(0, LayoutParams.MATCH_PARENT, 1);
			mLayout0M = new LayoutParams(LayoutParams.MATCH_PARENT, 0, 1);
			mLayoutSepV = new LayoutParams(LayoutParams.MATCH_PARENT, mSepDim);
			mLayoutSepH = new LayoutParams(mSepDim, LayoutParams.MATCH_PARENT);
			mLayoutSepVF = new LayoutParams(0, mSepDim, 1);
			mLayoutSepHF = new LayoutParams(mSepDim, 0, 1);
			mLayoutSepC = new LayoutParams(mSepDim, mSepDim);
		}
		
		int norientation = (orientation == HORIZONTAL) ? VERTICAL : HORIZONTAL;
		mLayoutNear.setOrientation(norientation);
		mLayoutSep.setOrientation(norientation);
		mLayoutFar.setOrientation(norientation);
	}

	public int getSelectedPane() {
		return mSelection;
	}

	public void setSelectedPane(int selection) {
        if (selection < 0 || selection > 3)
            return;

		this.mSelection = selection;
		for (int i = 0; i < mSeparators.length; i++) {
			mSeparators[i].setBackgroundColor(mGrayColor);
		}
		if (mSelectionSeparators[selection] != null) {
			for (int j = 0; j < mSelectionSeparators[selection].length; j++) {
				mSeparators[mSelectionSeparators[selection][j]].setBackgroundColor(mBlueColor);
			}
		}
	}

	public int getFrameId (int n) {
		return mFrames[n].getId();
	}

	public FrameLayout getFrame(int n) {
		return mFrames[n];
	}
}
