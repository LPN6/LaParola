package net.laparola.ui.android;

import net.laparola.R;
import android.content.Context;
import android.util.AttributeSet;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.FrameLayout;
import android.view.View.OnClickListener;

import com.google.android.material.button.MaterialButton;

public class FourColorSwitcher extends FrameLayout implements OnClickListener {
	public interface OnColorClickListener {
		void OnColorClicked(int color, int index);
	}
	
	private final MaterialButton mButtonCyan;
	private final MaterialButton mButtonGreen;
	private final MaterialButton mButtonOrange;
	private final MaterialButton mButtonPurple;
	private OnColorClickListener mListener;
	private int mIndex;
	private int mColor;

	public FourColorSwitcher(Context context, AttributeSet attrs, int defStyle) {
		super(context, attrs, defStyle);
		
		LayoutInflater inflater = LayoutInflater.from(context);
		inflater.inflate(R.layout.four_color_switcher, this);
		
		mButtonCyan = findViewById(R.id.color_cyan);
		mButtonGreen = findViewById(R.id.color_green);
		mButtonOrange = findViewById(R.id.color_orange);
		mButtonPurple = findViewById(R.id.color_purple);
		
		mButtonCyan.setOnClickListener(this);
		mButtonGreen.setOnClickListener(this);
		mButtonOrange.setOnClickListener(this);
		mButtonPurple.setOnClickListener(this);
	}

	@Override
	public void onClick(View v) {
		/*
		rosso     eb1313
		verde     8bc53f
		giallo    fffc8b
		arancione f7901e
		viola     a466aa
		azzurro   33b5e5
		*/
		
		if (v == mButtonCyan) {
			mIndex = 0;
			mColor = 0xff33b5e5;
		} else if (v == mButtonGreen) {
			mIndex = 1;
			mColor = 0xff8bc53f;
		} else if (v == mButtonOrange) {
			mIndex = 2;
			mColor = 0xfff7901e;
		} else if (v == mButtonPurple) {
			mIndex = 3;
			mColor = 0xffa466aa;
		}
			
		if (mListener != null) {
			mListener.OnColorClicked(mColor, getIndex());
		}
	}

	public void setOnColorClickListener(OnColorClickListener mListener) {
		this.mListener = mListener;
	}

	public int getIndex() {
		return mIndex;
	}

	public void setIndex(int index) {
		switch (index) {
		case 0:
			onClick(mButtonCyan);
			break;
		case 1:
			onClick(mButtonGreen);
			break;
		case 2:
			onClick(mButtonOrange);
			break;
		case 3:
			onClick(mButtonPurple);
			break;
		}
	}
}
