package net.laparola.ui.android.dialogs;

import net.laparola.R;
import net.laparola.ui.android.LaParolaActivity;
import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.res.Resources;
import android.os.Bundle;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageButton;
import android.view.View;

public class SearchDialog extends HoloDialog implements android.content.DialogInterface.OnClickListener {
	public EditText expressionText;
	public EditText referenceText;
	public ImageButton helpButton;
	public ImageButton moreButton;
	public Button searchButton;
	public boolean searchOk;

	private AlertDialog mPopup;

	public SearchDialog(Context context) {
		super(context, false);
	}

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);

		setContentView(R.layout.advanced_search_dialog);

		setTitle(R.string.advanced_search);

		expressionText = findViewById(R.id.expression_text);
		referenceText = findViewById(R.id.reference_text);
		helpButton = findViewById(R.id.help_button);
		moreButton = findViewById(R.id.more_button);
		searchButton = findViewById(R.id.search_button);

		helpButton.setOnClickListener(this);
		moreButton.setOnClickListener(this);
		searchButton.setOnClickListener(this);

		searchOk = false;
	}

	@Override
	public void onDetachedFromWindow() {
		super.onDetachedFromWindow();

		if (mPopup != null && mPopup.isShowing()) {
			mPopup.dismiss();
		}
	}

	private void showDropDown() {
		if (mPopup == null) {
			ArrayAdapter<String> mListAdapter = new ArrayAdapter<String>(mContext, android.R.layout.simple_spinner_dropdown_item);

			Resources res = mContext.getResources();
			String[] stringArray = res.getStringArray(R.array.advanced_search_ref_names);

            for (String s : stringArray) {
                mListAdapter.add(s);
            }
			// mListAdapter.addAll(stringArray); // solo API >= 11

            mPopup = new AlertDialog.Builder(mContext)
            	.setSingleChoiceItems(mListAdapter, 0, this)
            	.create();
		}

		if (!mPopup.isShowing()) {
			mPopup.show();
		}
	}

	@Override
	public void onClick(View view) {
		if (view == moreButton) {
			showDropDown();
		} else if (view == helpButton) {
			((LaParolaActivity) mContext).mostraAiutoRicerca();
		} else if (view == searchButton) {
			searchOk = true;
			dismiss();
		}
	}

	@Override
	public void onClick(DialogInterface dialogInterface, int which) {
		Resources res = mContext.getResources();
		referenceText.setText(res.getStringArray(R.array.advanced_search_ref_values)[which]);
		dialogInterface.dismiss();
	}
}
