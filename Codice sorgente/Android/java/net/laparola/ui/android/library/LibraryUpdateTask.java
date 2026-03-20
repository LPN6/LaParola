package net.laparola.ui.android.library;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.Set;

import net.laparola.R;
import net.laparola.core.ComponenteInformazioni;
import net.laparola.core.Testi.TestoTipi;
import net.laparola.ui.LaParolaBrowser;
import net.laparola.ui.android.LaParolaPreferences;
import net.laparola.ui.android.dialogs.HoloDialog;
import net.laparola.ui.android.dialogs.MessageDialog;

import org.xml.sax.SAXParseException;

import android.content.DialogInterface;
import android.content.DialogInterface.OnDismissListener;
import android.os.AsyncTask;

/* package */ class LibraryUpdateTask extends AsyncTask<Void, Void, List<ComponenteInformazioni>> {
	private LibraryActivity libraryActivity;

	LibraryUpdateTask(LibraryActivity libraryActivity) {
		this.libraryActivity = libraryActivity;
	}

	private int message = 0;

	@Override
	protected List<ComponenteInformazioni> doInBackground(Void... params) {
		List<ComponenteInformazioni> components = null;

		String storagePath = LaParolaPreferences.writeStoragePath;

		try {
			message = 0;
			components = LaParolaBrowser.getTestiDisponibili(storagePath + "/aggiorna.xml.cache");
		} catch (IOException e) {
			e.printStackTrace();
			message = R.string.cannot_download_updates;
		} catch (SAXParseException e) {
			e.printStackTrace();
			message = R.string.cannot_download_updates;
		} catch (Exception e) {
			e.printStackTrace();
			message = R.string.error_parsing_updates;
		}

		if (components == null) {
			components = LaParolaBrowser.getTestiInstallati();
		}

		return components;
	}

	@Override
	protected void onPostExecute(List<ComponenteInformazioni> result) {
		this.libraryActivity.setSupportProgressBarIndeterminateVisibility(false);

		if (result != null) {
			List<LibraryItemInfo> bibbie = new ArrayList<LibraryItemInfo>();
			List<LibraryItemInfo> commentari = new ArrayList<LibraryItemInfo>();
			List<LibraryItemInfo> dizionari = new ArrayList<LibraryItemInfo>();
			
			for (int i = 0; i < result.size(); i++) {
				LibraryItemInfo li = new LibraryItemInfo(this.libraryActivity, result.get(i));

				if (li.getTipo().contains(TestoTipi.COMMENTARIO)) {
					commentari.add(li);
				}
				else {
					if (li.getTipo().contains(TestoTipi.DIZIONARIO)) {
						dizionari.add(li);
					} else {
						if (li.getTipo().contains(TestoTipi.DIZIONARIO)) {
							dizionari.add(li);
						} else {
							bibbie.add(li);
						}
					}
				}
			}
			
			Set<String> broken = LaParolaBrowser.getFileIllegibili();
			for (String b : broken) {
				bibbie.add(new LibraryItemInfo(this.libraryActivity, b));
			}

			Collections.sort(bibbie);
			Collections.sort(commentari);
			Collections.sort(dizionari);

			LibraryAdapter bibbieAdapter = new LibraryAdapter(this.libraryActivity, this.libraryActivity, bibbie);
			LibraryAdapter commentariAdapter = new LibraryAdapter(this.libraryActivity, this.libraryActivity, commentari);
			LibraryAdapter dizionariAdapter = new LibraryAdapter(this.libraryActivity, this.libraryActivity, dizionari);
			this.libraryActivity.setAdapters(bibbieAdapter, commentariAdapter, dizionariAdapter);
		}

		if (message != 0) {
			HoloDialog messageDialog = new MessageDialog(this.libraryActivity, R.string.error, message);
			messageDialog.show();

			if (result == null || result.isEmpty()) {
				messageDialog.setOnDismissListener(new OnDismissListener() {
					@Override
					public void onDismiss(DialogInterface dialog) {
						LibraryUpdateTask.this.libraryActivity.finish();
					}
				});
			}
		}
	}

	@Override
	protected void onCancelled() {
		this.libraryActivity.setSupportProgressBarIndeterminateVisibility(false);
	}

	@Override
	protected void onPreExecute() {
		this.libraryActivity.setSupportProgressBarIndeterminateVisibility(true);
	}
}