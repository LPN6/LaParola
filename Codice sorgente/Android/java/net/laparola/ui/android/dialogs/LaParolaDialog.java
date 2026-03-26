package net.laparola.ui.android.dialogs;

import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;

import net.laparola.R;
import android.app.Dialog;
import android.content.Context;
import android.graphics.drawable.Drawable;
import android.view.View;
import android.view.ViewGroup.LayoutParams;
import android.view.Window;
import android.view.*;
import android.widget.Button;
import android.widget.FrameLayout;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.TextView;

public class LaParolaDialog extends Dialog implements android.view.View.OnClickListener {
	protected FrameLayout custom;
	protected LinearLayout topPanel;
	protected TextView message;
	protected ImageView icon;
	protected LinearLayout buttonPanel;
	protected Button button1;
	protected Button button2;
	protected Button button3;
	protected TextView alertTitle;
	protected Context mContext;
	protected LinearLayout contentPanel;
	private Runnable mOnYes;
	private Runnable mOnNo;

	public LaParolaDialog(Context context, boolean isAlert) {
		super(new ContextThemeWrapper(context, R.style.Theme_LaParola_Dialog));
		mContext = context;

			if (isAlert) {
			getWindow().requestFeature(Window.FEATURE_NO_TITLE);

			super.setContentView(R.layout.laparola_alert_dialog);
			topPanel = findViewById(R.id.topPanel);
			icon = findViewById(R.id.icon);
			alertTitle = findViewById(R.id.alertTitle);
			contentPanel = findViewById(R.id.contentPanel);
			message = findViewById(R.id.message);
			custom = findViewById(R.id.custom);
			buttonPanel = findViewById(R.id.buttonPanel);
			button2 = findViewById(R.id.button2);
			//divider1 = findViewById(R.id.divider1);
			button3 = findViewById(R.id.button3);
			//divider2 = findViewById(R.id.divider2);
			button1 = findViewById(R.id.button1);

			message.setVisibility(View.GONE);
			custom.setVisibility(View.GONE);

			buttonPanel.setVisibility(View.GONE);
			button2.setVisibility(View.GONE);
			//divider1.setVisibility(View.GONE);
			button3.setVisibility(View.GONE);
			//divider2.setVisibility(View.GONE);
			button1.setVisibility(View.GONE);

			setCloseOnTouchOutside(true);
		}
	}

	protected void setCloseOnTouchOutside(boolean value) {
		Method m;
		try {
			m = Window.class.getMethod("setCloseOnTouchOutside", boolean.class);
			m.invoke(getWindow(), value);
		} catch (NoSuchMethodException | IllegalArgumentException | IllegalAccessException |
                 InvocationTargetException e) {
			//
		}
    }

	@Override
	public void setContentView(View view) {
		LinearLayout.LayoutParams layoutParams = new LinearLayout.LayoutParams(LayoutParams.MATCH_PARENT, LayoutParams.WRAP_CONTENT);
		super.setContentView(view, layoutParams);
	}

	@Override
	public void setContentView(int layoutResID) {
		if (custom == null) {
			super.setContentView(layoutResID);
		} else {
			custom.setVisibility(View.VISIBLE);
			custom.removeAllViews();
			getLayoutInflater().inflate(layoutResID, custom);
		}
	}

	@Override
	public void setContentView(View view, LayoutParams params) {
		if (custom == null) {
			super.setContentView(view, params);
		} else {
			custom.removeAllViews();
			if (view != null) {
				custom.setVisibility(View.VISIBLE);
				custom.addView(view, params);
			} else {
				custom.setVisibility(View.GONE);
			}
		}
	}

	@Override
	public void setTitle(int titleId) {
		if (alertTitle == null) {
			super.setTitle(titleId);
		} else {
			alertTitle.setText(titleId);
		}
	}

	@Override
	public void setTitle(CharSequence title) {
		if (alertTitle == null) {
			super.setTitle(title);
		} else {
			alertTitle.setText(title);
		}
	}

	public void setIcon(Drawable d) {
		icon.setImageDrawable(d);
	}

	public void setIcon(int i) {
		icon.setImageResource(i);
	}

	public void setYesNo(int yesStringId, int noStringId, Runnable onYes, Runnable onNo) {
		buttonPanel.setVisibility(View.VISIBLE);
		button1.setOnClickListener(this);
		button2.setOnClickListener(this);

		button1.setText(yesStringId);
		button1.setVisibility(View.VISIBLE);

		if (noStringId != 0) {
			button2.setText(noStringId);

			button2.setVisibility(View.VISIBLE);
			//divider1.setVisibility(View.VISIBLE);
		} else {
			button2.setVisibility(View.GONE);
			//divider1.setVisibility(View.GONE);
		}

		mOnYes = onYes;
		mOnNo = onNo;
	}

	public void setButtons(int string1Id, int string2Id, int string3Id) {
		buttonPanel.setVisibility(View.VISIBLE);
		button1.setOnClickListener(this);
		button2.setOnClickListener(this);
		button3.setOnClickListener(this);

		button1.setText(string1Id);
		button1.setVisibility(View.VISIBLE);

		button2.setText(string2Id);
		button2.setVisibility(View.VISIBLE);
		//divider1.setVisibility(View.VISIBLE);

		if (string3Id != 0) {
			button3.setText(string3Id);
			button3.setVisibility(View.VISIBLE);
			//divider2.setVisibility(View.VISIBLE);
		}
	}

	@Override
	public void onClick(View v) {
		if (v == button2 && mOnNo != null) {
			mOnNo.run();
		} else if (v == button1 && mOnYes != null) {
			mOnYes.run();
		}

		dismiss();
	}

	// solo per API>=11, che è sempre vero
	protected void setSoftwareRendererV11() {
		contentPanel.setLayerType(View.LAYER_TYPE_SOFTWARE, null);
	}
}
