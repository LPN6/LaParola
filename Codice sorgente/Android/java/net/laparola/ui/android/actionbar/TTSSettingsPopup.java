package net.laparola.ui.android.actionbar;

import net.laparola.R;
import net.laparola.ui.android.LaParolaPreferences;
import net.laparola.ui.android.lpnspinner.LpnPopupWindow;
import android.content.Context;
import android.graphics.drawable.Drawable;
import android.view.View;
import android.view.ViewGroup;
import android.view.View.MeasureSpec;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.CompoundButton;
import android.widget.LinearLayout;
import android.widget.PopupWindow;
import android.widget.CompoundButton.OnCheckedChangeListener;

import com.google.android.material.slider.Slider;

public class TTSSettingsPopup extends LpnPopupWindow implements OnCheckedChangeListener, OnClickListener {
    /**
	 * 
	 */
	private final TTSActionItemManager ttsActionItemManager;
	private View mDropDownView;
    private CheckBox mStopEndChapter;
	private CheckBox mFollowVerse;
	
    public TTSSettingsPopup(TTSActionItemManager ttsActionItemManager, Context context) {
		super(context);
		this.ttsActionItemManager = ttsActionItemManager;
        
        setAnchorView(this.ttsActionItemManager.settingsButton);
        setModal(true);
    }

    @Override
    public void show() {
        final int spinnerPaddingLeft = this.ttsActionItemManager.settingsButton.getPaddingLeft();
        setHorizontalOffset(spinnerPaddingLeft);
        setInputMethodMode(PopupWindow.INPUT_METHOD_NOT_NEEDED);
        super.show();
    }

	@Override
	protected int buildDropDown(int popupWidthSpec) {
		if (mDropDownView == null) {
	        mDropDownView = new LinearLayout(mContext);
	        View.inflate(mContext, R.layout.tts_settings, (LinearLayout)mDropDownView);
		    
	        mDropDownView.setFocusable(true);
	        mDropDownView.setFocusableInTouchMode(true);
	        mPopup.setContentView(mDropDownView);
	        
	        setEventHandlers();
	        
	        mDropDownView.measure(MeasureSpec.UNSPECIFIED, MeasureSpec.UNSPECIFIED);
	        setContentWidth(mDropDownView.getMeasuredWidth());
	        setContentHeight(mDropDownView.getMeasuredHeight());
	    } else {
	    	mDropDownView = mPopup.getContentView();
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
	
		return mDropDownHeight + padding;
	}

	private void setEventHandlers() {
        Slider mPitchSlider = mDropDownView.findViewById(R.id.tts_pitch_slider);
        Slider mSpeedSlider = mDropDownView.findViewById(R.id.tts_speed_slider);
		mPitchSlider.addOnChangeListener((slider, value, fromUser) -> {

			LaParolaPreferences.ttsPitch = (int)value;
			this.ttsActionItemManager.loadPreferences(true);
		});

		mSpeedSlider.addOnChangeListener((slider, value, fromUser) -> {
			LaParolaPreferences.ttsSpeed = (int)value;
			this.ttsActionItemManager.loadPreferences(true);
		});

		mStopEndChapter = mDropDownView.findViewById(R.id.stop_end_chapter_checkbox);
		mFollowVerse = mDropDownView.findViewById(R.id.follow_verse_checkbox);
        Button mSettingsButton = mDropDownView.findViewById(R.id.tts_settings_button);
		
		mPitchSlider.setValue(LaParolaPreferences.ttsPitch);
		mSpeedSlider.setValue(LaParolaPreferences.ttsSpeed);
		mStopEndChapter.setChecked(LaParolaPreferences.ttsStopEndChapter);
		mFollowVerse.setChecked(LaParolaPreferences.ttsFollowVerse);
		
		mStopEndChapter.setOnCheckedChangeListener(this);
		mFollowVerse.setOnCheckedChangeListener(this);
		
		mSettingsButton.setOnClickListener(this);
	}

	@Override
	public void onClick(View v) {
		this.ttsActionItemManager.startTtsSettings();				
	}

	@Override
	public void onCheckedChanged(CompoundButton view, boolean value) {
		if (view == mStopEndChapter) {
			LaParolaPreferences.ttsStopEndChapter = value;
		} else if (view == mFollowVerse) {
			LaParolaPreferences.ttsFollowVerse = value;
		}
		
		this.ttsActionItemManager.loadPreferences(false);
	}
}