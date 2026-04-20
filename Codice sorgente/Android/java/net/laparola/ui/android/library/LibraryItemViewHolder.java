package net.laparola.ui.android.library;

import net.laparola.R;
import net.laparola.core.Testi.StatoAggiornamento;
import net.laparola.core.Testi.TestoTipi;
import net.laparola.ui.LaParolaBrowser;
import net.laparola.ui.android.LaParolaActivity;
import net.laparola.ui.android.dialogs.LaParolaDialog;
import net.laparola.ui.android.dialogs.MessageDialog;
import net.laparola.ui.android.library.ClickableListAdapter.ViewHolder;
import android.view.View;
import android.widget.FrameLayout;
import android.widget.ImageButton;
import android.widget.TextView;

class LibraryItemViewHolder extends ViewHolder implements View.OnClickListener {
	LibraryActivity mLibraryActivity;
	View parentView;
	FrameLayout component_list_item;
	TextView title;
	TextView description;
	ImageButton install;
	ImageButton update;
	ImageButton delete;
	ImageButton market;

	public LibraryItemViewHolder(LibraryActivity libraryActivity, View v) {
		parentView = v;
		component_list_item = (FrameLayout) v;
		title = v.findViewById(R.id.title);
		description = v.findViewById(R.id.description);
		install = v.findViewById(R.id.install);
		update = v.findViewById(R.id.update);
		delete = v.findViewById(R.id.delete);
		market = v.findViewById(R.id.market);

		mLibraryActivity = libraryActivity;

		component_list_item.setOnClickListener(this);
		install.setOnClickListener(this);
		update.setOnClickListener(this);
		delete.setOnClickListener(this);
		market.setOnClickListener(this);
	}

	public void bind() {
		LibraryItemInfo item = (LibraryItemInfo) data;

		boolean installing = mLibraryActivity.isDownloading(item);
		
		install.setVisibility((!installing && item.showInstall()) ? View.VISIBLE : View.GONE);
		update.setVisibility(item.showUpdate() ? View.VISIBLE : View.GONE);
		delete.setVisibility(item.showDelete() ? View.VISIBLE : View.GONE);
		market.setVisibility(item.showMarket() ? View.VISIBLE : View.GONE);
		title.setText(item.getName());
		if (!installing) {
			description.setText(item.getStateString());
		} else {
			description.setText(R.string.downloading);
		}
	}

	@Override
	public void onClick(View v) {
		final LibraryItemInfo info = (LibraryItemInfo) data;

		if (v == component_list_item) {
			LaParolaDialog d = new MessageDialog(mLibraryActivity, info.getName(), info.getMessage());
			d.show();
		} else if (v == market) {
			boolean ok = LaParolaActivity.apriLink(mLibraryActivity, mLibraryActivity.getString(R.string.market_url));
			if (!ok) {
				LaParolaDialog d = new MessageDialog(mLibraryActivity, R.string.error, R.string.no_market_installed);
				d.setOnDismissListener(dialog -> LaParolaActivity.apriLink(mLibraryActivity, mLibraryActivity.getString(R.string.laparola_url)));
				d.show();
			}
		} else if (v == install || v == update) {
			mLibraryActivity.startDownload(info);
			bind();
		} else if (v == delete) {
			if (mLibraryActivity.getInstalledBibleCount() <= 1 && info.getStatoAggiornamento() != StatoAggiornamento.FILE_CORROTTO && info.getTipo().contains(TestoTipi.BIBBIA)) {
				LaParolaDialog md = new MessageDialog(mLibraryActivity, R.string.error, R.string.error_delete_last_bible);
				md.show();
			} else {
				LaParolaDialog md = new MessageDialog(mLibraryActivity, R.string.delete, R.string.confirm_delete);
				md.setYesNo(R.string.delete, android.R.string.cancel, () -> {
                    LaParolaBrowser.cancellaTesto(info.getName(), info.getFileName());
                    mLibraryActivity.refreshLibrary(false);
                }, null);
				md.show();
			}
		}
	}
}