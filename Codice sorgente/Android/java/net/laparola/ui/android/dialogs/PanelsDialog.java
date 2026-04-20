package net.laparola.ui.android.dialogs;

import net.laparola.R;
import net.laparola.ui.android.FourColorSwitcher;
import net.laparola.ui.android.FourPanesLayout;
import net.laparola.ui.android.LaParolaActivity;
import android.graphics.Color;
import android.os.Bundle;
import android.view.View;

import com.google.android.material.button.MaterialButton;

public class PanelsDialog extends LaParolaDialog {
	private MaterialButton mButton1;
	private MaterialButton mButton2h;
	private MaterialButton mButton2v;
	private MaterialButton mButton3h;
	private MaterialButton mButton3v;
	private MaterialButton mButton4;
	private FourPanesLayout mPanels;
	private final LaParolaActivity mParent;
	private FourColorSwitcher[] mColorSwitchers;

	public PanelsDialog(LaParolaActivity context) {
		super(context, true);
		mParent = context;
	}
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);

		setContentView(R.layout.fragments_dialog);

		setTitle(R.string.fragments_management);
		
		mButton1 = findViewById(R.id.fragments_1);
		mButton2h = findViewById(R.id.fragments_2h);
		mButton2v = findViewById(R.id.fragments_2v);
		mButton3h = findViewById(R.id.fragments_3h);
		mButton3v = findViewById(R.id.fragments_3v);
		mButton4 = findViewById(R.id.fragments_4);
		mPanels = findViewById(R.id.four_panes_layout);
		
		mButton1.setOnClickListener(this);
		mButton2h.setOnClickListener(this);
		mButton2v.setOnClickListener(this);
		mButton3h.setOnClickListener(this);
		mButton3v.setOnClickListener(this);
		mButton4.setOnClickListener(this);
	
		mPanels.setSeparatorWidth(4);
		
		mColorSwitchers = new FourColorSwitcher[4];
		for (int i = 0; i < mColorSwitchers.length; i++) {
			final int ii = i;
			
			mColorSwitchers[i] = new FourColorSwitcher(mParent, null, 0);
			mPanels.getFrame(i).addView(mColorSwitchers[i]);
			mColorSwitchers[i].setOnColorClickListener((color, index) -> {
                int r = Color.red(color);
                int g = Color.green(color);
                int b = Color.blue(color);

                r = (r + 3 * 0xff) / 4;
                g = (g + 3 * 0xff) / 4;
                b = (b + 3 * 0xff) / 4;

                int dimColor = Color.rgb(r, g, b);

                mPanels.getFrame(ii).setBackgroundColor(dimColor);
            });
			
			mColorSwitchers[i].setIndex(mParent.getSyncColor(i));
		}
		
		int panesNumber = mParent.getPanesNumber();
		int panesOrientation = mParent.getPanesOrientation();
		if (panesNumber == 1) {
			onClick(mButton1);
		} else if (panesNumber == 2) {
			if (panesOrientation == FourPanesLayout.HORIZONTAL) {
				onClick(mButton2v);
			} else {
				onClick(mButton2h);
			}
		} else if (panesNumber == 3) {
			if (panesOrientation == FourPanesLayout.HORIZONTAL) {
				onClick(mButton3v);
			} else {
				onClick(mButton3h);
			}
		} else if (panesNumber == 4) {
			onClick(mButton4);
		}
		
		setYesNo(android.R.string.ok, android.R.string.cancel, this::onOkClick, this::onCancelClick);
	}
	
	protected void onOkClick() {
		mParent.setPanes(
				mPanels.getNumberPanes(), 
				mPanels.getOrientation() == FourPanesLayout.HORIZONTAL ? FourPanesLayout.VERTICAL : FourPanesLayout.HORIZONTAL, 
				false,
				new int[] {
					mColorSwitchers[0].getIndex(),
					mColorSwitchers[1].getIndex(),
					mColorSwitchers[2].getIndex(),
					mColorSwitchers[3].getIndex()
				});
	}

	protected void onCancelClick() {}

	@Override
	public void onClick(View v) {
		mButton1.setSelected(v == mButton1);
		mButton2h.setSelected(v == mButton2h);
		mButton2v.setSelected(v == mButton2v);
		mButton3h.setSelected(v == mButton3h);
		mButton3v.setSelected(v == mButton3v);
		mButton4.setSelected(v == mButton4);
		
		if (v == mButton1) {
			 mPanels.setPanes(1, FourPanesLayout.HORIZONTAL);
		} else if (v == mButton2h) {
			 mPanels.setPanes(2, FourPanesLayout.HORIZONTAL);
		} else if (v == mButton2v) {
			 mPanels.setPanes(2, FourPanesLayout.VERTICAL);
		} else if (v == mButton3h) {
			 mPanels.setPanes(3, FourPanesLayout.HORIZONTAL);
		} else if (v == mButton3v) {
			 mPanels.setPanes(3, FourPanesLayout.VERTICAL);
		} else if (v == mButton4) {
			 mPanels.setPanes(4, FourPanesLayout.HORIZONTAL);
		} else {
			super.onClick(v);
		}
	}
}