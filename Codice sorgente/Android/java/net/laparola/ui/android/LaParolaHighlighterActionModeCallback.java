package net.laparola.ui.android;

import java.util.HashMap;

import net.laparola.R;
import net.laparola.ui.LaParolaEvidenziatore;

import android.content.res.Resources;
import android.graphics.Bitmap;
import android.graphics.Bitmap.Config;
import android.graphics.Canvas;
import android.graphics.Paint;
import android.graphics.Paint.Style;
import android.graphics.drawable.BitmapDrawable;
import android.view.ActionMode;
import android.view.ActionMode.Callback;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;

public class LaParolaHighlighterActionModeCallback implements Callback {
	private LaParolaActivity mParentActivity;
	private MenuItem mColorMenuItem;
	
	private final static HashMap<String, Integer> COLORS;
	static {
		COLORS = new HashMap<String, Integer>();   // htmlName, androidColor 
		COLORS.put("yellow",  0xffffff00);
		COLORS.put("lime",    0xff00ff00);
		COLORS.put("cyan",    0xff00ffff);
		COLORS.put("magenta", 0xffff00ff);
	}
	
	public LaParolaHighlighterActionModeCallback (LaParolaActivity activity) {
		mParentActivity = activity;
		
		setColor(LaParolaPreferences.highlighColor);
	}

	public boolean setup() {
		boolean ok = false;
		for (int i = 0; i < mParentActivity.fragments.size(); i++) {
			LaParolaFragment f = mParentActivity.fragments.get(i);
			ok = ok || f.attivaEvidenziatore(true);
		}
		return ok;
	}
	
	@Override
	public boolean onCreateActionMode(ActionMode mode, Menu menu) {
        MenuInflater inflater = mode.getMenuInflater();
        inflater.inflate(R.menu.highlighter, menu);
        return true;
	}

	private BitmapDrawable getPenBitmap (int color) {
		Resources resources = mParentActivity.getResources();
		float density = resources.getDisplayMetrics().density;
		BitmapDrawable penDrawable = (BitmapDrawable)resources.getDrawable(R.drawable.ic_action_highlight_color);
		
		Paint fillPaint = new Paint();
		fillPaint.setColor(color);
		fillPaint.setStyle(Style.FILL);
		
		Paint strokePaint = new Paint();
		strokePaint.setColor(0x99333333);
		strokePaint.setStrokeWidth(density);
		strokePaint.setStyle(Style.STROKE);
		
		int w = penDrawable.getIntrinsicWidth();
		int h = penDrawable.getIntrinsicHeight();
		
		Bitmap myBmp = Bitmap.createBitmap(w, h, Config.ARGB_8888);
		Canvas canvas = new Canvas(myBmp);
		canvas.drawBitmap(penDrawable.getBitmap(), 0, 0, null);
		canvas.drawRect(density * 20, density * 20, density * 28, density * 28, fillPaint);
		canvas.drawRect(density * 20, density * 20, density * 28, density * 28, strokePaint);
		
		return new BitmapDrawable(resources, myBmp);
	}

	private void setPenIcon() {
		if (mColorMenuItem != null)
			mColorMenuItem.setIcon(getPenBitmap(getColorInt()));
	}	
	
	@Override
	public boolean onPrepareActionMode(ActionMode mode, Menu menu) {
		mColorMenuItem = menu.findItem(R.id.change_color);
		setPenIcon();
		return false;
	}

	@Override
	public boolean onActionItemClicked(ActionMode mode, MenuItem item) {
        switch (item.getItemId()) {
        case R.id.change_color:
    		if (LaParolaPreferences.highlighColor.equals("yellow"))
    			setColor("lime");
    		else if (LaParolaPreferences.highlighColor.equals("lime"))
    			setColor("cyan");
    		else if (LaParolaPreferences.highlighColor.equals("cyan"))
    			setColor("magenta");
    		else
    			setColor("yellow");
    			
            return true;
        case R.id.highlighted_list:
            mParentActivity.getActiveFragment().vaiAdUrl("lpevidenziati:");        	
            //mode.finish();   // Action picked, so close the CAB
            return true;
        default:
            return false;
        }
    }

	private void setColor(String name) {
		LaParolaPreferences.highlighColor = name;

		setPenIcon();
		
		for (int i = 0; i < mParentActivity.fragments.size(); i++) {
			LaParolaFragment f = mParentActivity.fragments.get(i);
			f.setColoreEvidenziatore(LaParolaPreferences.highlighColor);
		}
	}

	private Integer getColorInt() {
		if (COLORS.containsKey(LaParolaPreferences.highlighColor))
			return COLORS.get(LaParolaPreferences.highlighColor);
		return 0;
	}

	@Override
	public void onDestroyActionMode(ActionMode mode) {
		for (int i = 0; i < mParentActivity.fragments.size(); i++)
			mParentActivity.fragments.get(i).attivaEvidenziatore(false);
		
		String storagePath = LaParolaPreferences.writeStoragePath;
		LaParolaEvidenziatore.salvaVersettiEvidenziatiSuFile();
		
		mParentActivity.actionMode = null;
	}
}
