package net.laparola.ui.android.dialogs;

import net.laparola.R;
import net.laparola.ui.LaParolaBrowser;
import net.laparola.ui.LaParolaSegnalibri.Segnalibro;
import net.laparola.ui.LaParolaUrl;
import android.content.Context;
import android.os.Bundle;
import android.widget.EditText;
import androidx.core.content.ContextCompat;

import static android.view.View.INVISIBLE;

public class StarredDialog extends LaParolaDialog {
	private EditText description;
	public LaParolaUrl url;

	public StarredDialog(Context context) {
		super(context, true);
	}

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);

		setContentView(R.layout.starred_dialog);
		setTitle(R.string.star_added);
		setIcon(R.drawable.ic_icon_star);

		description = findViewById(R.id.description);
		button2.setTextColor(ContextCompat.getColor(getContext(), R.color.colorError));

		setYesNo(android.R.string.ok, R.string.remove, this::onOkClick, this::onRemoveClick);
	}

	private void onOkClick() {
		Segnalibro s = LaParolaBrowser.cercaUrlTraPreferiti(url);
		if (s == null) {
			LaParolaBrowser.aggiungiPreferito("Preferiti", description.getText().toString(), url);
		} else {
			s.setAncoraggio(url.ancoraggio);
			s.nome = description.getText().toString();
		}
		LaParolaBrowser.salvaPreferitiSuFile();
	}

	private void onRemoveClick() {
		LaParolaBrowser.rimuoviPreferito(url);
		LaParolaBrowser.salvaPreferitiSuFile();
	}

	public void setDescription(String descrizione) {
		description.setText(descrizione);
		description.setSelection(descrizione.length());
	}

	public void setSegnalibroNonEsiste() {
		button2.setVisibility(INVISIBLE);
	}
}
