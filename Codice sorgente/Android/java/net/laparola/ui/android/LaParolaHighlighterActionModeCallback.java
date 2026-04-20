package net.laparola.ui.android;

import java.util.HashMap;

import net.laparola.R;
import net.laparola.ui.LaParolaEvidenziatore;

import android.content.res.Resources;
import android.graphics.Bitmap;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.Paint;
import android.graphics.Paint.Style;
import android.graphics.drawable.BitmapDrawable;
import android.graphics.drawable.Drawable;
import androidx.appcompat.view.ActionMode;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;

import com.google.android.material.color.MaterialColors;

import androidx.core.content.res.ResourcesCompat;
import androidx.core.graphics.drawable.DrawableCompat;

public class LaParolaHighlighterActionModeCallback implements ActionMode.Callback {
	private final LaParolaActivity mParentActivity;
	private MenuItem mColorMenuItem;
	
	private final static HashMap<String, Integer> COLORS;
	static {
		COLORS = new HashMap<>();   // htmlName, androidColor
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
		// Hide the ActionBar when the contextual mode starts
		if (mParentActivity.getSupportActionBar() != null) {
			mParentActivity.getSupportActionBar().hide();
		}

		MenuInflater inflater = mode.getMenuInflater();
        inflater.inflate(R.menu.highlighter, menu);
        return true;
	}

	private BitmapDrawable getPenBitmap (int color) {
		Resources resources = mParentActivity.getResources();
		float density = resources.getDisplayMetrics().density;

		Drawable drawable = ResourcesCompat.getDrawable(
				resources, R.drawable.ic_action_highlight_color, mParentActivity.getTheme() );

		if (drawable == null) {
			throw new IllegalArgumentException("Drawable non trovato in LaParolaHighlighterActionModeCallback");
		}

		Drawable wrapped = DrawableCompat.wrap(drawable).mutate();
		DrawableCompat.setTint(wrapped, MaterialColors.getColor(mParentActivity, R.attr.colorOnSurface, Color.WHITE));
		drawable = wrapped;

		Bitmap bitmap;
		if (drawable instanceof BitmapDrawable) {
			bitmap = ((BitmapDrawable) drawable).getBitmap();
		} else {
			int w = drawable.getIntrinsicWidth() > 0 ? drawable.getIntrinsicWidth() : (int)(48 * density);
			int h = drawable.getIntrinsicHeight() > 0 ? drawable.getIntrinsicHeight() : (int)(48 * density);

			bitmap = Bitmap.createBitmap(w, h, Bitmap.Config.ARGB_8888);
			Canvas canvas = new Canvas(bitmap);
			drawable.setBounds(0, 0, canvas.getWidth(), canvas.getHeight());
			drawable.draw(canvas);
		}

		Paint fillPaint = new Paint();
		fillPaint.setColor(color);
		fillPaint.setStyle(Style.FILL);
		
		Paint strokePaint = new Paint();
		strokePaint.setColor(0x99333333);
		strokePaint.setStrokeWidth(density);
		strokePaint.setStyle(Style.STROKE);

		int squareSize = (int)(16 * density);
		int margin = (int)(4 * density);

		int left = bitmap.getWidth() - squareSize -  margin;  // margin from right
		int top  = bitmap.getHeight() - squareSize -  margin;  // margin from bottom
		int right = bitmap.getWidth() -  margin;
		int bottom = bitmap.getHeight() -  margin;
		
		Bitmap myBmp = bitmap.copy(Bitmap.Config.ARGB_8888, true);
		Canvas canvas = new Canvas(myBmp);
		canvas.drawRect(left, top, right, bottom, fillPaint);
		canvas.drawRect(left, top, right, bottom, strokePaint);
		
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
		int id = item.getItemId();
		if (id == R.id.change_color) {
			switch (LaParolaPreferences.highlighColor) {
				case "yellow" -> setColor("lime");
				case "lime" -> setColor("cyan");
				case "cyan" -> setColor("magenta");
				default -> setColor("yellow");
			}
			return true;
		}
		if (id == R.id.highlighted_list) {
			mParentActivity.getActiveFragment().vaiAdUrl("lpevidenziati:");
			//mode.finish();   // Action picked, so close the CAB
			return true;
		}
		return false;
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
		// Show the ActionBar again when the contextual mode ends
		if (mParentActivity.getSupportActionBar() != null) {
			mParentActivity.getSupportActionBar().show();
		}

		for (int i = 0; i < mParentActivity.fragments.size(); i++)
			mParentActivity.fragments.get(i).attivaEvidenziatore(false);
		
		//String storagePath = LaParolaPreferences.writeStoragePath;
		LaParolaEvidenziatore.salvaVersettiEvidenziatiSuFile();
		
		mParentActivity.actionMode = null;
	}
}
