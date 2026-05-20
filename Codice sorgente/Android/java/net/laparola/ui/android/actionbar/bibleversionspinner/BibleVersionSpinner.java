package net.laparola.ui.android.actionbar.bibleversionspinner;

import android.content.Context;
import android.util.AttributeSet;
import android.widget.ListAdapter;

import net.laparola.ui.android.lpnspinner.LpnDropDownAdapter;
import net.laparola.ui.android.lpnspinner.LpnDropdownPopup;
import net.laparola.ui.android.lpnspinner.LpnHijackFocusListView;
import net.laparola.ui.android.lpnspinner.LpnAbsSpinner;

public class BibleVersionSpinner extends LpnAbsSpinner {
    public boolean libraryVisible = true;

    public BibleVersionSpinner(Context context, AttributeSet attrs) {
        super(context, attrs);
    }

    public BibleVersionSpinner(Context context, AttributeSet attrs, int defStyle) {
        super(context, attrs, defStyle);
    }

    @Override
    public LpnDropdownPopup createPopup(Context context, AttributeSet attrs, int defStyle) {
        return new BibleVersionDropDownPopup(context, attrs, defStyle, this);
    }

    static class BibleVersionDropDownPopup extends LpnDropdownPopup {
        private VersionAdapter mVersionAdapter;
        private final BibleVersionSpinner parent;

        public BibleVersionDropDownPopup(Context context, AttributeSet attrs, int defStyleAttr, BibleVersionSpinner ignSpinner) {
            super(context, attrs, defStyleAttr, ignSpinner);
            this.parent = ignSpinner;
        }

        @Override
        protected LpnHijackFocusListView createListView(Context context, boolean hijackfocus) {
            BibleVersionListView.sNextLibraryVisible = parent.libraryVisible;
            BibleVersionListView bibleVersionListView = new BibleVersionListView(context, hijackfocus);
            bibleVersionListView.setOnTypeChangedListener(tipo -> {
                mVersionAdapter.setTipo(tipo);
                dismiss();
                show();
            });
            return bibleVersionListView;
        }

        @Override
        public void setAdapter(ListAdapter adapter) {
            super.setAdapter(adapter);
            mVersionAdapter = (VersionAdapter) ((LpnDropDownAdapter) adapter).getInternalAdapter();
        }

        @Override
        public void show() {
            super.show();
            if (mDropDownList instanceof BibleVersionListView) {
                ((BibleVersionListView) mDropDownList).setSelectedType(mVersionAdapter.getTipo());
            }
        }
    }
}
