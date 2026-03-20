package net.laparola.ui.android.library;

import java.util.List;

import net.laparola.R;
import net.laparola.core.Testi.StatoAggiornamento;
import net.laparola.core.Testi.TestoTipi;
import android.content.Context;
import android.view.View;

/* package */ class LibraryAdapter extends ClickableListAdapter<LibraryItemInfo> {
	private LibraryActivity libraryActivity;

	public LibraryAdapter(LibraryActivity libraryActivity, Context context, List<LibraryItemInfo> components) {
		super(context, R.layout.component_list_item, components);
		this.libraryActivity = libraryActivity;
		
		//long availableMegs = freeMemoryMB();
		//android.util.Log.d("laparola", String.format("free ram: %d MB", availableMegs));
		// zip : +60% (errore su libreria 21 maggio 2012: -9%/+5%)
		// nuova riveduta: lzma 2.5 MB -> finestra 8.8 MB
	}

	/*
	private long freeMemoryMB() {
		MemoryInfo mi = new MemoryInfo();
		ActivityManager activityManager = (ActivityManager) this.libraryActivity.getSystemService(LibraryActivity.ACTIVITY_SERVICE);
		activityManager.getMemoryInfo(mi);
		long availableMegs = mi.availMem / 1048576L;
		return availableMegs;
	}
	*/

	@Override
	protected void bindHolder(ClickableListAdapter.ViewHolder h) {
		((LibraryItemViewHolder) h).bind();
	}

	@Override
	protected ClickableListAdapter.ViewHolder createHolder(View v) {
		return new LibraryItemViewHolder(this.libraryActivity, v);
	}

	public int getInstalledBibleCount() {
		int r = 0;
		for (LibraryItemInfo i : this.mDataObjects) {
			if (i.showDelete() && i.getStatoAggiornamento() != StatoAggiornamento.FILE_CORROTTO && i.getTipo().contains(TestoTipi.BIBBIA))
				r++;
		}
		return r;
	}
}