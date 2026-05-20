package net.laparola.ui.android.dialogs;

import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import net.laparola.R;
import net.laparola.ui.android.lpnspinner.GridSpinner;
import net.laparola.ui.android.lpnspinner.ListSpinner;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;

public class BookTabFragment extends Fragment {
    private ListSpinner bookSpinner;
    private GridSpinner chapterSpinner;
    private GridSpinner verseSpinner;

    public BookTabFragment() {}

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater,
                             @Nullable ViewGroup container,
                             @Nullable Bundle savedInstanceState) {
        View view = inflater.inflate(R.layout.fragment_book_tab, container, false);

        bookSpinner = view.findViewById(R.id.book_spinner);
        chapterSpinner = view.findViewById(R.id.chapter_spinner);
        verseSpinner = view.findViewById(R.id.verse_spinner);

        return view;
    }

    public ListSpinner getBookSpinner() {
        return bookSpinner;
    }

    public GridSpinner getChapterSpinner() {
        return chapterSpinner;
    }

    public GridSpinner getVerseSpinner() {
        return verseSpinner;
    }
}