package net.laparola.ui.android.actionbar.bibleversionspinner;

import android.content.Context;
import android.util.AttributeSet;
import android.widget.ListAdapter;

import net.laparola.ui.android.ignspinner.IgnDropDownAdapter;
import net.laparola.ui.android.ignspinner.IgnDropdownPopup;
import net.laparola.ui.android.ignspinner.IgnHijackFocusListView;
import net.laparola.ui.android.ignspinner.IgnAbsSpinner;

public class BibleVersionSpinner extends IgnAbsSpinner {
    public boolean libraryVisible = true;

    public BibleVersionSpinner(Context context, AttributeSet attrs) {
        super(context, attrs);
    }

    public BibleVersionSpinner(Context context, AttributeSet attrs, int defStyle) {
        super(context, attrs, defStyle);
    }

    @Override
    public IgnDropdownPopup createPopup(Context context, AttributeSet attrs, int defStyle) {
        return new BibleVersionDropDownPopup(context, attrs, defStyle, this);
    }

    static class BibleVersionDropDownPopup extends IgnDropdownPopup {
        private VersionAdapter mVersionAdapter;
        private final BibleVersionSpinner parent;

        public BibleVersionDropDownPopup(Context context, AttributeSet attrs, int defStyleAttr, BibleVersionSpinner ignSpinner) {
            super(context, attrs, defStyleAttr, ignSpinner);
            this.parent = ignSpinner;
        }

        @Override
        protected IgnHijackFocusListView createListView(Context context, boolean hijackfocus) {
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
            mVersionAdapter = (VersionAdapter) ((IgnDropDownAdapter) adapter).getInternalAdapter();
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
