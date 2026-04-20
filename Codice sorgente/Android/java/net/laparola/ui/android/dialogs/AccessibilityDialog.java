package net.laparola.ui.android.dialogs;

import android.content.Context;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.inputmethod.EditorInfo;
import android.widget.*;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.DialogFragment;
import androidx.recyclerview.widget.GridLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import com.google.android.material.button.MaterialButtonToggleGroup;

import net.laparola.R;
import net.laparola.ui.LaParolaUrl;
import net.laparola.ui.android.LaParolaActivity;
import net.laparola.ui.android.LaParolaFragment;
import net.laparola.ui.android.actionbar.ReferenceActionItemManager.*;

import java.util.Optional;

public class AccessibilityDialog extends DialogFragment {
    private LaParolaActivity parent;
    private RecyclerView recyclerView;
    private TextView header;
    private EditText referenceEditText;
    private ViewFlipper viewFlipper;

    private BookSpinnerAdapter bookAdapter;
    private ChapterSpinnerAdapter chapterAdapter;
    private VerseSpinnerAdapter verseAdapter;

    private int selectedBook = 0;
    private int selectedChapter = 0;

    // Fragments require an empty constructor
    public AccessibilityDialog() {
    }

    @Override
    public void onAttach(@NonNull Context context) {
        super.onAttach(context);
        this.parent = (LaParolaActivity) context;
    }

    @Nullable
    @Override
    public View onCreateView(@NonNull LayoutInflater inflater, @Nullable ViewGroup container, @Nullable Bundle savedInstanceState) {
        setCancelable(true);
        View view = inflater.inflate(R.layout.accessibility_dialog, container, false);

        recyclerView = view.findViewById(R.id.selectionRecyclerView);
        header = view.findViewById(R.id.selectionHeader);
        referenceEditText = view.findViewById(R.id.reference_edittext);
        viewFlipper = view.findViewById(R.id.viewFlipper);
        MaterialButtonToggleGroup toggleGroup = view.findViewById(R.id.toggleGroup);

        bookAdapter = new BookSpinnerAdapter(parent);
        bookAdapter.togliLibroZero();
        chapterAdapter = new ChapterSpinnerAdapter(parent);
        chapterAdapter.togliCapitoloZero();
        verseAdapter = new VerseSpinnerAdapter(parent);
        verseAdapter.togliVersettoZero();

        setupTabs(toggleGroup);
        setupAdvancedInput();

        // Start the sequence: Book -> Chapter -> Verse
        showBookSelection();

        return view;
    }

    @Override
    public void onStart() {
        super.onStart();
        // Make the dialog fill the screen width
        if (getDialog() != null && getDialog().getWindow() != null) {
            //if (!parent.isTablet)  if not full width, some book names wrap. the alternative is to have only two columns
//            getDialog().getWindow().setLayout(ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.WRAP_CONTENT);
            getDialog().getWindow().setLayout(ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.MATCH_PARENT);
        }
    }

    private void resetScroll() {
        if (recyclerView != null) {
            recyclerView.scrollToPosition(0);
        }
    }

    private void showBookSelection() {
        resetScroll();
        int libroAttuale = Optional.ofNullable(parent.getActiveFragment())
                .map(LaParolaFragment::getUrlCorrente)
                .map(LaParolaUrl::getLCV)
                .filter(lcv -> lcv.length > 0) // Ensure the array isn't empty
                .map(lcv -> lcv[0])           // Get the first element
                .orElse(0); // Default to 0 if anything above was null/empty

        header.setText(R.string.book);
        recyclerView.setLayoutManager(new GridLayoutManager(getContext(), parent.isTablet ? 3 : 1));

        GridLayoutManager layoutManager = new GridLayoutManager(getContext(), parent.isTablet ? 3 : 1);
        recyclerView.setLayoutManager(layoutManager);

        SelectionAdapter adapter = new SelectionAdapter(bookAdapter, true, position -> {
            selectedBook = (int) bookAdapter.getItemId(position);
            chapterAdapter.setBook(selectedBook);
            showChapterSelection();
            parent.getActiveFragment().vaiALibroCapitoloVersetto(selectedBook, 1, 1);
        });
        recyclerView.setAdapter(adapter);

        int position = -1;
        // necessario, perché non tutte le versioni hanno tutti i libri,
        // quindi cerchiamo la posizione del libro desiderato
        for (int i = 0; i < bookAdapter.getCount(); i++) {
            if (bookAdapter.getItemId(i) == libroAttuale) {
                position = i;
                break;
            }
        }

        int positionInAdapter = position;
        if (position >= 0 && position < adapter.getItemCount()) {
            recyclerView.post(() -> layoutManager.scrollToPositionWithOffset(positionInAdapter, 0));
        }
    }

    private void showChapterSelection() {
        resetScroll();
        header.setText(R.string.chapter);
        recyclerView.setLayoutManager(new GridLayoutManager(getContext(), parent.isTablet ? 5 : 3));

        SelectionAdapter adapter = new SelectionAdapter(chapterAdapter, position -> {
            selectedChapter = (int) chapterAdapter.getItemId(position);
            verseAdapter.setBookAndChapter(selectedBook, selectedChapter);
            showVerseSelection();
            parent.getActiveFragment().vaiALibroCapitoloVersetto(selectedBook, selectedChapter, 1);
        });
        recyclerView.setAdapter(adapter);
    }

    private void showVerseSelection() {
        resetScroll();
        header.setText(R.string.verse);
        recyclerView.setLayoutManager(new GridLayoutManager(getContext(), parent.isTablet ? 5 : 3));

        SelectionAdapter adapter = new SelectionAdapter(verseAdapter, position -> {
            int verse = (int) verseAdapter.getItemId(position);
            // Navigate using the method from your uploaded file
            parent.getActiveFragment().vaiALibroCapitoloVersetto(selectedBook, selectedChapter, verse);
            dismiss();
        });
        recyclerView.setAdapter(adapter);
    }

    private void setupTabs(MaterialButtonToggleGroup group) {
        group.addOnButtonCheckedListener((tg, checkedId, isChecked) -> {
            if (isChecked) {
                resetScroll();
                viewFlipper.setDisplayedChild(checkedId == R.id.btnTabBasic ? 0 : 1);
            }
        });
    }

    private void setupAdvancedInput() {
        referenceEditText.setOnEditorActionListener((v, actionId, event) -> {
            if (actionId == EditorInfo.IME_ACTION_GO ||
                    (event != null && event.getKeyCode() == android.view.KeyEvent.KEYCODE_ENTER)) {
                parent.getActiveFragment().vaiARiferimento(referenceEditText.getText());
                dismiss();
                return true;
            }
            return false;
        });
    }

    // Bridge for BaseAdapters
    private static class SelectionAdapter extends RecyclerView.Adapter<SelectionAdapter.ViewHolder> {
        private final SpinnerAdapter adapter;
        private final OnClickListener listener;
        private final boolean piuSpazio;

        interface OnClickListener {
            void onClick(int position);
        }

        SelectionAdapter(SpinnerAdapter adapter, boolean piuSpazio, OnClickListener listener) {
            this.adapter = adapter;
            this.listener = listener;
            this.piuSpazio = piuSpazio;
        }

        SelectionAdapter(SpinnerAdapter adapter, OnClickListener listener) {
            this(adapter, false, listener);
        }

        @NonNull
        @Override
        public ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
            View v = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_accessibility_button, parent, false);
            if (piuSpazio) { // mettiamo più spazio fra i libri che fra i numeri
                v.setMinimumHeight(v.getMinimumHeight() + 12);
            }
            return new ViewHolder(v);
        }

        @Override
        public void onBindViewHolder(@NonNull ViewHolder holder, int position) {
            Button btn = (Button) holder.itemView;
            btn.setText(adapter.getItem(position).toString());
            btn.setOnClickListener(v -> listener.onClick(position));
        }

        @Override
        public int getItemCount() {
            return adapter.getCount();
        }

        static class ViewHolder extends RecyclerView.ViewHolder {
            ViewHolder(View v) {
                super(v);
            }
        }
    }
}