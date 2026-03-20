package net.laparola.ui.android.ignspinner;

import android.database.DataSetObserver;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ListAdapter;
import android.widget.SpinnerAdapter;

public class IgnDropDownAdapter implements ListAdapter, SpinnerAdapter {
    private SpinnerAdapter mAdapter;
    private ListAdapter mListAdapter;

    /**
     * <p>Creates a new ListAdapter wrapper for the specified adapter.</p>
     *
     * @param adapter the Adapter to transform into a ListAdapter
     */
    public IgnDropDownAdapter(SpinnerAdapter adapter) {
        this.mAdapter = adapter;
        if (adapter instanceof ListAdapter) {
            this.mListAdapter = (ListAdapter) adapter;
        }
    }

    public SpinnerAdapter getInternalAdapter () {
    	return mAdapter;
    }
    
    public int getCount() {
        return mAdapter == null ? 0 : mAdapter.getCount();
    }

    public Object getItem(int position) {
        return mAdapter == null ? null : mAdapter.getItem(position);
    }

    public long getItemId(int position) {
        return mAdapter == null ? -1 : mAdapter.getItemId(position);
    }

    public View getView(int position, View convertView, ViewGroup parent) {
        return getDropDownView(position, convertView, parent);
    }

    public View getDropDownView(int position, View convertView, ViewGroup parent) {
        return mAdapter == null ? null :
                mAdapter.getDropDownView(position, convertView, parent);
    }

    public boolean hasStableIds() {
        return mAdapter != null && mAdapter.hasStableIds();
    }

    public void registerDataSetObserver(DataSetObserver observer) {
        if (mAdapter != null) {
            mAdapter.registerDataSetObserver(observer);
        }
    }

    public void unregisterDataSetObserver(DataSetObserver observer) {
        if (mAdapter != null) {
            mAdapter.unregisterDataSetObserver(observer);
        }
    }

    /**
     * If the wrapped SpinnerAdapter is also a ListAdapter, delegate this call.
     * Otherwise, return true.
     */
    public boolean areAllItemsEnabled() {
        final ListAdapter adapter = mListAdapter;
        if (adapter != null) {
            return adapter.areAllItemsEnabled();
        } else {
            return true;
        }
    }

    /**
     * If the wrapped SpinnerAdapter is also a ListAdapter, delegate this call.
     * Otherwise, return true.
     */
    public boolean isEnabled(int position) {
        final ListAdapter adapter = mListAdapter;
        if (adapter != null) {
            return adapter.isEnabled(position);
        } else {
            return true;
        }
    }

    public int getItemViewType(int position) {
        return 0;
    }

    public int getViewTypeCount() {
        return 1;
    }

    public boolean isEmpty() {
        return getCount() == 0;
    }
}