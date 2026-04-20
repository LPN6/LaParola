package net.laparola.ui.android.dialogs;

import android.content.Context;
import android.view.View;

public class MessageDialog extends LaParolaDialog {
	private MessageDialog(Context context) {
		super(context, true);

		message.setVisibility(View.VISIBLE);
		setYesNo(android.R.string.ok, 0, null, null);
	}
	
	public MessageDialog(Context context, int titleId, int messageId) {
		this(context);
		if (titleId == 0) {
			topPanel.setVisibility(View.GONE);
		} else {
			alertTitle.setText(titleId);
		}
		message.setText(messageId);
	}
	
	public MessageDialog(Context context, String titleStr, String messageStr) {
		this(context);
		alertTitle.setText(titleStr);
		message.setText(messageStr);
	}
}
