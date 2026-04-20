package net.laparola.ui.android.actionbar;

import android.content.Context;
import android.database.DataSetObserver;
import android.view.KeyEvent;
import android.view.LayoutInflater;
import android.view.MenuItem;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.view.ViewGroup.LayoutParams;
import android.view.inputmethod.EditorInfo;
import android.view.inputmethod.InputMethodManager;
import android.widget.AbsListView;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemSelectedListener;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.LinearLayout;
import android.widget.SpinnerAdapter;
import android.widget.TextView;
import android.widget.TextView.OnEditorActionListener;

import com.google.android.material.bottomsheet.BottomSheetDialog;

import net.laparola.R;
import net.laparola.ui.LaParolaBrowser;
import net.laparola.ui.LaParolaUrl;
import net.laparola.ui.android.LaParolaActivity;
import net.laparola.ui.android.LaParolaFragment;
import net.laparola.ui.android.LaParolaPreferences;
import net.laparola.ui.android.ignspinner.GridSpinner;
import net.laparola.ui.android.ignspinner.IgnAdapterView;
import net.laparola.ui.android.ignspinner.ListSpinner;

import java.util.Objects;

import androidx.annotation.NonNull;

/* rmw1024
import androidx.recyclerview.widget.DividerItemDecoration;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import static androidx.core.content.ContentProviderCompat.requireContext;
*/

public class ReferenceActionItemManager implements OnItemSelectedListener, MenuItem.OnActionExpandListener, OnClickListener, OnEditorActionListener, net.laparola.ui.android.ignspinner.IgnAdapterView.OnItemSelectedListener {
    private final LaParolaActivity parent;
    private final MenuItem referenceActionItem;
    private LinearLayout bcvLayout;
    private ListSpinner bookSpinner;
    private GridSpinner chapterSpinner;
    private GridSpinner verseSpinner;
    private ImageButton referenceEditButton;
    private BookSpinnerAdapter bookSpinnerAdapter;
    private ChapterSpinnerAdapter chapterSpinnerAdapter;
    private VerseSpinnerAdapter verseSpinnerAdapter;
    private boolean ignoreBookSelection;
    private boolean ignoreChapterSelection;
    private boolean ignoreVerseSelection;

    private LinearLayout fullRefLayout;
    private EditText referenceEditText;
    private ImageButton referenceGoButton;
    private BottomSheetDialog bottomSheetDialog;

    private LinearLayout dizionarioLayout;
    // rmw1024 private EditText notaTitolo;
    // rmw1024 private RecyclerView dizionarioRecyclerView;

    // rmw1024 private List<String> items = new ArrayList<>();

    public ReferenceActionItemManager(LaParolaActivity parent, MenuItem referenceActionItem) {
        this.parent = parent;
        this.referenceActionItem = referenceActionItem;

        // Only initialize the ActionView automatically if we are on a tablet
        if (parent.isTablet) {
            View actionView = referenceActionItem.getActionView();
            if (actionView != null) setupViews(actionView);
        }
    }

    public void setupViews(View root) {
        bottomSheetDialog = new BottomSheetDialog(parent);

        bcvLayout = root.findViewById(R.id.bcv_linear_layout);
        fullRefLayout = root.findViewById(R.id.fullref_linear_layout);

        bookSpinner = root.findViewById(R.id.book_spinner);
        chapterSpinner = root.findViewById(R.id.chapter_spinner);
        verseSpinner = root.findViewById(R.id.verse_spinner);
        referenceEditText = root.findViewById(R.id.reference_edittext);
        referenceEditText.setOnEditorActionListener(this);
        //View closeButton = root.findViewById(R.id.reference_close);
        referenceGoButton = root.findViewById(R.id.reference_go_btn);

        bookSpinnerAdapter = new BookSpinnerAdapter(parent);
        bookSpinner.setAdapter(bookSpinnerAdapter);
        bookSpinner.setOnItemSelectedListener(this);
        chapterSpinnerAdapter = new ChapterSpinnerAdapter(parent);
        chapterSpinner.setAdapter(chapterSpinnerAdapter);
        chapterSpinner.setOnItemSelectedListener(this);
        verseSpinnerAdapter = new VerseSpinnerAdapter(parent);
        verseSpinner.setAdapter(verseSpinnerAdapter);
        verseSpinner.setOnItemSelectedListener(this);

        ignoreBookSelection = true;
        ignoreChapterSelection = true;
        bookSpinner.setSelection(0);
        chapterSpinner.setEnabled(false);
        verseSpinner.setEnabled(false);

        // In setupViews, ensure the adapters are refreshed
        updateBooks();

        if (parent.isTablet) {
            referenceEditButton = root.findViewById(R.id.reference_edit_btn);
            referenceEditButton.setOnClickListener(this);
        } else {
            LaParolaFragment fragment = parent.getActiveFragment();
            if (fragment != null) {
                LaParolaUrl url = fragment.getUrlCorrente();
                if (url != null && url.getLCV() != null) {
                    int b = url.getLCV()[0];
                    int c = url.getLCV()[1];
                    int v = url.getLCV()[2];
                    ignoreBookSelection = true;
                    ignoreChapterSelection = true;
                    ignoreVerseSelection = true;

                    //bookSpinner.setSelection(b); se non tutti i libri sono nella versione, non funziona
                    for (int pos = 0; pos < bookSpinnerAdapter.getCount(); pos++) {
                        if (bookSpinnerAdapter.getItemId(pos) == b) {
                            bookSpinner.setSelection(pos);
                            break;
                        }
                    }

                    chapterSpinnerAdapter.setBook(b);
                    if (b == 0) {
                        chapterSpinner.setEnabled(false);
                        chapterSpinner.setSelection(0);
                        verseSpinner.setEnabled(false);
                        verseSpinner.setSelection(0);
                    } else {
                        chapterSpinner.setEnabled(true);
                        chapterSpinner.setSelection(c);
                        verseSpinnerAdapter.setBookAndChapter(b, c);
                        verseSpinner.setEnabled(true);
                        verseSpinner.setSelection(v);
                    }
                }
            }
        }

        if (referenceGoButton != null) {
            referenceGoButton.setOnClickListener(v -> parent.executeAndClose(referenceEditText.getText().toString(), bottomSheetDialog));
        }
    }

    public void expandActionView() {
        if (parent.isTablet) {
            referenceActionItem.expandActionView();
        } else {
            showAsBottomSheet();
        }
    }

    private void showAsBottomSheet() {
        View sheetView = parent.getLayoutInflater().inflate(R.layout.bottom_sheet_reference, null);

        // Bind current listeners and views to the new layout
        setupViews(sheetView);

        if (referenceGoButton != null) {
            referenceGoButton.setOnClickListener(v -> {
                String ref = referenceEditText.getText().toString();
                parent.executeAndClose(ref, bottomSheetDialog);
            });
        }

        bottomSheetDialog.setContentView(sheetView);
        bottomSheetDialog.show();
    }

	/*
	public ReferenceActionItemManager(LaParolaActivity parent, MenuItem referenceActionItem) {
        this.parent = parent;

        this.referenceActionItem = referenceActionItem;
		LinearLayout referenceActionView = (LinearLayout)referenceActionItem.getActionView();
		
		bcvLayout = referenceActionView.findViewById(R.id.bcv_linear_layout);
		
		bookSpinner = referenceActionView.findViewById(R.id.book_spinner);
		bookSpinnerAdapter = new BookSpinnerAdapter(parent);
		bookSpinner.setAdapter(bookSpinnerAdapter);
		bookSpinner.setOnItemSelectedListener(this);

		chapterSpinner = referenceActionView.findViewById(R.id.chapter_spinner);
		chapterSpinnerAdapter = new ChapterSpinnerAdapter(parent);
		chapterSpinner.setAdapter(chapterSpinnerAdapter);
		chapterSpinner.setOnItemSelectedListener(this);
		
		verseSpinner = referenceActionView.findViewById(R.id.verse_spinner);
		verseSpinnerAdapter = new VerseSpinnerAdapter(parent);
		verseSpinner.setAdapter(verseSpinnerAdapter);
		verseSpinner.setOnItemSelectedListener(this);
		
		ignoreBookSelection = true;
		ignoreChapterSelection = true;
		bookSpinner.setSelection(0);
		chapterSpinner.setEnabled(false);
		verseSpinner.setEnabled(false);
		
		referenceEditButton = referenceActionView.findViewById(R.id.reference_edit_btn);
		referenceEditButton.setOnClickListener(this);

        if (!parent.isTablet) {
            referenceEditButton.setPadding(0, 0, 0, 0);
            referenceEditButton.setScaleX(0.7F);
            referenceEditButton.setScaleY(0.7F);
            bookSpinner.setPadding(0,0,0,0);
            chapterSpinner.setPadding(0,0,0,0);
            verseSpinner.setPadding(0,0,0,0);
        }

		fullRefLayout = referenceActionView.findViewById(R.id.fullref_linear_layout);
		
		referenceEditText = referenceActionView.findViewById(R.id.reference_edittext);
		referenceEditText.setOnEditorActionListener(this);
		
		referenceGoButton = referenceActionView.findViewById(R.id.reference_go_btn);
		referenceGoButton.setOnClickListener(this);

		items.add("Alfa");
		items.add("Beta");
		items.add("Gamma");
		items.add("Delta");
		items.add("Epsilon");

		// rmw1024 dizionarioLayout = referenceActionView.findViewById(R.id.dizionario_linear_layout);
		// rmw1024 notaTitolo = referenceActionView.findViewById(R.id.note_titolo);
		// rmw1024 dizionarioRecyclerView = referenceActionView.findViewById(R.id.recyclerview_diz);

		// TODO giusti?
		// rmw1024 dizionarioRecyclerView.setLayoutManager(new LinearLayoutManager(parent));
		// rmw1024 dizionarioRecyclerView.addItemDecoration(new DividerItemDecoration(parent, DividerItemDecoration.VERTICAL));

		// rmw1024 notaRecyclerAdapter = new DizionarioRecyclerAdapter(items);
		// rmw1024 dizionarioRecyclerView.setAdapter(notaRecyclerAdapter);
        // rmw1024 private DizionarioRecyclerAdapter notaRecyclerAdapter;
        boolean ignoreNotaSelection = true;

		/*
		// Aggiungi alcuni elementi alla lista in ordine alfabetico
		items.add("Alfa");
		items.add("Beta");
		items.add("Gamma");
		items.add("Delta");
		items.add("Epsilon");

		dizionarioRecycler = referenceActionView.findViewById(R.id.recycler_diz);

		LinearLayoutManager layoutManager = new LinearLayoutManager(parent);
		dizionarioRecycler.setLayoutManager(layoutManager);

		DizionarioRecyclerAdapter dizionarioRecyclerAdapter = new DizionarioRecyclerAdapter(items);
		dizionarioRecycler.setAdapter(dizionarioRecyclerAdapter);

	 // end inner /* here

		referenceActionItem.setOnActionExpandListener(this);

		float scaledDensity = parent.getResources().getDisplayMetrics().scaledDensity;
		float fs = parent.getResources().getConfiguration().fontScale;
		int textSize = 24;
		int columnWidth = Math.round(2.5f * textSize * scaledDensity * fs);

        if (!parent.isTablet)
            chapterSpinner.setMinimumWidth(32);

		chapterSpinner.setColumnWidth(columnWidth);
		verseSpinner.setColumnWidth(columnWidth);
    }

	*/
    // rmw1024
	/*
	public void setDizionario(boolean tipoDizionario) {
		fullRefLayout.setVisibility(View.GONE);
		if ( tipoDizionario) {
			bcvLayout.setVisibility(View.GONE);
			dizionarioLayout.setVisibility(View.VISIBLE);
			dizionarioRecyclerView.setVisibility(View.VISIBLE);
		} else {
			bcvLayout.setVisibility(View.GONE);
			dizionarioLayout.setVisibility(View.VISIBLE);
			dizionarioRecyclerView.setVisibility(View.VISIBLE);
			//bcvLayout.setVisibility(View.VISIBLE); // TODO rimettere
			//dizionarioLayout.setVisibility(View.GONE);
			//dizionarioRecyclerView.setVisibility(View.GONE);
		}
	}
	*/

    public void onClick(View view) {
        if (view == referenceEditButton) {
            if (bcvLayout != null)
                bcvLayout.setVisibility(View.GONE);
            if (fullRefLayout != null)
                fullRefLayout.setVisibility(View.VISIBLE);

            referenceEditText.post(() -> {
                referenceEditText.requestFocusFromTouch();
                InputMethodManager imm = (InputMethodManager) parent.getSystemService(Context.INPUT_METHOD_SERVICE);
                imm.showSoftInput(referenceEditText, 0);
            });
        } else if (view == referenceGoButton) {
            parent.getActiveFragment().vaiARiferimento(referenceEditText.getText());
        }
    }

    public boolean onEditorAction(TextView v, int actionId, KeyEvent event) {
        if ((event != null && event.getAction() == KeyEvent.ACTION_DOWN && event.getKeyCode() == KeyEvent.KEYCODE_ENTER) || (actionId == EditorInfo.IME_ACTION_GO)) {

            //parent.getActiveFragment().vaiARiferimento(referenceEditText.getText());
            parent.executeAndClose(referenceEditText.getText().toString(), bottomSheetDialog);
            return true;
        }

        return false;
    }

    public void onItemSelected(AdapterView<?> view, View itemview, int position, long id) {
        onItemSelectedGeneric(view);
    }

    protected void onItemSelectedGeneric(Object view) {
        boolean dismiss = false;
        if (false) {
            // TODO
        } else {
            int b = (int) bookSpinner.getSelectedItemId();
            int c = (int) chapterSpinner.getSelectedItemId();
            int v = (int) verseSpinner.getSelectedItemId();
            boolean load = false;

            if (view == bookSpinner) {
                if (!ignoreBookSelection) {
                    chapterSpinnerAdapter.setBook(b);
                    if (b == 0) {
                        chapterSpinner.setEnabled(false);
                        chapterSpinner.setSelection(0);
                        verseSpinner.setEnabled(false);
                        verseSpinner.setSelection(0);
                    } else {
                        ignoreChapterSelection = true;
                        chapterSpinner.setEnabled(true);
                        chapterSpinner.setSelection(1);
                        ignoreVerseSelection = true;
                        verseSpinner.setEnabled(true);
                        verseSpinner.setSelection(1);
                        load = true;
                        if (LaParolaPreferences.autoOpenRef) chapterSpinner.performClick();
                    }
                }
                ignoreBookSelection = false;
            } else if (view == chapterSpinner) {
                if (!ignoreChapterSelection) {
                    verseSpinnerAdapter.setBookAndChapter(b, c);
                    if (b == 0 || c == 0) {
                        verseSpinner.setEnabled(false);
                        verseSpinner.setSelection(0);
                    } else {
                        ignoreVerseSelection = true;
                        verseSpinner.setEnabled(true);
                        verseSpinner.setSelection(1);
                        load = true;
                        if (LaParolaPreferences.autoOpenRef) verseSpinner.performClick();
                    }
                }
                ignoreChapterSelection = false;
            } else if (view == verseSpinner) {
                if (!ignoreVerseSelection) {
                    load = true;
                    dismiss = true;
                }
                ignoreVerseSelection = false;
            }

            if (load && b != 0 && c != 0 && v != 0) {
                parent.getActiveFragment().vaiALibroCapitoloVersetto(b, c, v);

                if (dismiss) {
                    if (bottomSheetDialog != null && bottomSheetDialog.isShowing()) {
                        bottomSheetDialog.dismiss();
                        bottomSheetDialog = null; // Clean up
                    }
                }
            }
        }
    }

    public void onNothingSelected(AdapterView<?> arg0) {
    }

    public boolean onMenuItemActionExpand(@NonNull MenuItem item) {
        if (LaParolaPreferences.accessibilityMode) {
            return false;
        }

        bcvLayout.post(() -> {
            ignoreBookSelection = false;
            ignoreChapterSelection = false;
            ignoreVerseSelection = false;
        });

        return parent.collapseActionViewsExcept(item);
    }

    public boolean onMenuItemActionCollapse(@NonNull MenuItem item) {
        if (bcvLayout != null && bcvLayout.getVisibility() == View.GONE) {
            bcvLayout.setVisibility(View.VISIBLE);
            if (fullRefLayout != null)
                fullRefLayout.setVisibility(View.GONE);

            InputMethodManager imm = (InputMethodManager) parent.getSystemService(Context.INPUT_METHOD_SERVICE);
            imm.hideSoftInputFromWindow(referenceEditText.getWindowToken(), 0);
            referenceEditText.post(referenceEditText::clearFocus);

            // preferisco chiudere del tutto il reference
            //return false;
        }
        return true;
    }

    public void resettaView() {
        if (bcvLayout != null) bcvLayout.setVisibility(View.VISIBLE);
        if (fullRefLayout != null) fullRefLayout.setVisibility(View.GONE);
    }

    public void onVersionChanged() {
        updateBooks();
    }

    public void collapse(MenuItem exclude) {
        if (exclude != referenceActionItem) {
            referenceActionItem.collapseActionView();
        }
    }

    public void select(int b, int c, int v) {
        if (chapterSpinnerAdapter == null) return;

        chapterSpinnerAdapter.setBook(b);
        verseSpinnerAdapter.setBookAndChapter(b, c);

        if (bookSpinner.getSelectedItemId() != b) {
            ignoreBookSelection = true;
            boolean ok = false;
            for (int pos = 0; pos < bookSpinnerAdapter.getCount(); pos++) {
                if (bookSpinnerAdapter.getItemId(pos) == b) {
                    bookSpinner.setSelection(pos);
                    ok = true;
                    break;
                }
            }
            if (!ok) {
                b = (int) bookSpinner.getSelectedItemId();
                c = (b != 0) ? 1 : 0;
                v = c;
            }
        }

        if (b != 0 && c != 0 && v != 0) {
            chapterSpinner.setEnabled(true);
            verseSpinner.setEnabled(true);
        }

        if (chapterSpinner.getSelectedItemId() != c) {
            ignoreChapterSelection = true;
            chapterSpinner.setSelection(c);
        }

        if (verseSpinner.getSelectedItemId() != v) {
            ignoreVerseSelection = true;
            verseSpinner.setSelection(v);
        }
    }

    public void select(CharSequence reference) {
        if (referenceEditText != null) {
            referenceEditText.setText(reference);
        }
    }

    public static class BookSpinnerAdapter extends BCSSpinnerAdapter {
        private final int[] book_ids;
        private int book_count;
        private boolean conLibroZero = true;

        public BookSpinnerAdapter(LaParolaActivity p) {
            super(p);

            book_ids = new int[74];
            updateBooks();
        }

        public void togliLibroZero() {
            book_count = 0;
            conLibroZero = false;
            aggiungiLibri();
        }

        public void updateBooks() {
            book_count = 1;   // compreso lo 0 (nessun libro)
            aggiungiLibri();
        }

        private void aggiungiLibri() {
            LaParolaFragment fragment = getFragment();
            if (fragment != null && fragment.isVisible() && !fragment.getVersione().isEmpty()) {
                for (int b = 1; b <= 73; b++) {
                    if (getFragment().getCapitoliInLibro(b) != 0) {
                        book_ids[book_count++] = b;
                    }
                }
            } else {
                // non ci sono versioni installate, metto tutti i libri
                for (int b = 1; b <= 73; b++) {
                    book_ids[book_count++] = b;
                }
            }

        }

        public int getCount() {
            return book_count;
        }

        public Object getItem(int position) {
            LaParolaFragment fragment = getFragment();
            if ((position == 0 && conLibroZero) || fragment == null) return "";
            return LaParolaBrowser.getNomeLibro(book_ids[position]);
        }

        public long getItemId(int position) {
            if (position == 0 && conLibroZero) return 0;
            return book_ids[position];
        }

        @Override
        public View getDropDownView(int position, View convertView, ViewGroup genitore) {
            TextView text = (TextView) super.getDropDownView(position, convertView, genitore);
            boolean empty = text.getText().length() == 0;
            text.setEnabled(!empty);
            text.setClickable(empty);   // controintuitivo... perché? forse ha a che fare con
            // il fatto che si clicca sul contenitore
            if (empty) text.setText(R.string.book);
            return text;
        }

        @Override
        public View getView(int position, View convertView, ViewGroup genitore) {
            TextView text = (TextView) super.getView(position, convertView, genitore);
            boolean empty = text.getText().length() == 0;
            if (empty) text.setText(R.string.book);
            return text;
        }

        @Override
        public boolean hasStableIds() {
            return false;
        }
    }

    public static class ChapterSpinnerAdapter extends BCSSpinnerAdapter {
        private int book;
        private boolean conCapitoloZero = true;

        public ChapterSpinnerAdapter(LaParolaActivity p) {
            super(p);
            book = 0;
        }

        public void togliCapitoloZero() {
            conCapitoloZero = false;
        }

        public void setBook(int b) {
            book = b;
        }

        public int getCount() {
            LaParolaFragment fragment = getFragment();
            if (book == 0 || fragment == null) return 1;
            return Math.max(1, fragment.getCapitoliInLibro(book) + (conCapitoloZero ? 1 : 0));
        }

        public Object getItem(int position) {
            if (position == 0 && conCapitoloZero) return "";
            return String.valueOf(position + (conCapitoloZero ? 0 : 1));
        }

        public long getItemId(int position) {
            return position + (conCapitoloZero ? 0 : 1);
        }
    }

    public static class VerseSpinnerAdapter extends BCSSpinnerAdapter {
        private int book;
        private int chapter;
        private boolean conVersettoZero = true;

        public VerseSpinnerAdapter(LaParolaActivity p) {
            super(p);
            book = 0;
            chapter = 0;
        }

        public void togliVersettoZero() {
            conVersettoZero = false;
        }

        public void setBookAndChapter(int b, int c) {
            book = b;
            chapter = c;
        }

        public int getCount() {
            LaParolaFragment fragment = getFragment();
            if (book == 0 || chapter == 0 || fragment == null) return 1;
            return Math.max(1, fragment.getVersettiInCapitolo(book, chapter) + (conVersettoZero ? 1 : 0));
        }

        public Object getItem(int position) {
            if (position == 0 && conVersettoZero) return "";
            return String.valueOf(position + (conVersettoZero ? 0 : 1));
        }

        public long getItemId(int position) {
            return position + (conVersettoZero ? 0 : 1);
        }
    }

    private static abstract class BCSSpinnerAdapter implements SpinnerAdapter {
        private LayoutInflater mInflater;
        protected LaParolaActivity parent;
        private float mTextSize;

        public BCSSpinnerAdapter(LaParolaActivity p) {
            parent = p;
        }

        public LaParolaFragment getFragment() {
            return parent.getActiveFragment();
        }

        public int getItemViewType(int position) {
            return 0;
        }

        public int getViewTypeCount() {
            return 1;
        }

        public boolean hasStableIds() {
            return true;
        }

        public boolean isEmpty() {
            return false;
        }

        public void registerDataSetObserver(DataSetObserver observer) {
        }

        public void unregisterDataSetObserver(DataSetObserver observer) {
        }

        protected View createViewFromResource(int position, View convertView, ViewGroup parent, int resource) {
            View view;
            TextView text;

            if (convertView == null) {
                if (mInflater == null) {
                    mInflater = (LayoutInflater) parent.getContext().getSystemService(Context.LAYOUT_INFLATER_SERVICE);
                }
                view = mInflater.inflate(resource, parent, false);
            } else {
                view = convertView;
            }

            try {
                int mFieldId = 0;

                if (mFieldId == 0) {
                    //  If no custom field is assigned, assume the whole resource is a TextView
                    text = (TextView) view;
                } else {
                    //  Otherwise, find the TextView field within the layout
                    text = view.findViewById(mFieldId);
                }
            } catch (ClassCastException e) {
                throw new IllegalStateException("ArrayAdapter requires the resource ID to be a TextView", e);
            }

            Object item = getItem(position);
            if (item instanceof CharSequence) {
                text.setText((CharSequence) item);
            } else {
                text.setText(item.toString());
            }

            if (mTextSize > 0) {
                text.setTextSize(mTextSize);
                int w = LayoutParams.MATCH_PARENT;
                int h = LayoutParams.WRAP_CONTENT;
                text.setLayoutParams(new AbsListView.LayoutParams(w, h));
            }

            return view;
        }

        public View getView(int position, View convertView, ViewGroup parent) {
            return createViewFromResource(position, convertView, parent, android.R.layout.simple_spinner_item);
        }

        public View getDropDownView(int position, View convertView, ViewGroup parent) {
            return createViewFromResource(position, convertView, parent, android.R.layout.simple_spinner_dropdown_item);
        }

        public float getTextSize() {
            return mTextSize;
        }

        public void setTextSize(float mBigText) {
            this.mTextSize = mBigText;
        }
    }

    /* rmw1024
    public class DizionarioRecyclerAdapter extends RecyclerView.Adapter<DizionarioRecyclerAdapter.MyViewHolder> {
        // Usa una lista di stringhe come sorgente dei dati
        private List<String> items; // TODO

        // Crea un costruttore che accetta una lista di stringhe come parametro
        public DizionarioRecyclerAdapter(List<String> items) {
            this.items = items;
        }

        // Crea una classe interna che estende RecyclerView.ViewHolder e contiene una TextView
        public class MyViewHolder extends RecyclerView.ViewHolder {
            // Usa una TextView per mostrare il testo di ogni elemento
            public TextView textView;

            // Crea un costruttore che accetta una vista come parametro
            public MyViewHolder(View view) {
                super(view);
                // Trova la TextView nella vista
                textView = view.findViewById(R.id.dizionario_item_titolo);
            }
        }

        // Sovrascrivi il metodo onCreateViewHolder per creare le viste degli elementi
        @Override
        public MyViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
            // Crea una vista da un layout XML che contiene una TextView con un id
            View view = LayoutInflater.from(parent.getContext())
                    .inflate(R.layout.dizionario_list_item, parent, false);
            // Crea un MyViewHolder dalla vista e restituiscilo
            MyViewHolder viewHolder = new MyViewHolder(view);
            return viewHolder;
        }

        // Sovrascrivi il metodo onBindViewHolder per collegare i dati alle viste degli elementi
        @Override
        public void onBindViewHolder(MyViewHolder holder, int position) {
            // Ottieni la stringa corrispondente alla posizione
            String item = items.get(position);
            // Imposta il testo della TextView con la stringa
            holder.textView.setText(item);
        }

        // Sovrascrivi il metodo getItemCount per restituire il numero di elementi nella lista
        @Override
        public int getItemCount() {
            return items.size();
        }
    }
*/
    public void expand() {
        referenceActionItem.expandActionView();
    }

    public void updateBooks() {
        if (bookSpinner == null) return;

        String book = (String) bookSpinner.getSelectedItem();
        int nbooks = bookSpinnerAdapter.getCount();
        bookSpinnerAdapter.updateBooks();
        if (bookSpinnerAdapter.getCount() != nbooks) {
            bookSpinner.setAdapter(null);
            bookSpinner.setAdapter(bookSpinnerAdapter);

            // è cambiato il numero di libri, il numero non è più significativo
            for (int i = 0; i < bookSpinner.getCount(); i++) {
                if (book.equals(bookSpinner.getItemAtPosition(i))) {
                    bookSpinner.setSelection(i);
                    break;
                }
            }
        }
    }

	/* da cancellare
	public void expandActionView() {
		referenceActionItem.expandActionView();
	}
	*/


    @Override
    public void onItemSelected(IgnAdapterView<?> view, View itemview, int position, long id) {
        onItemSelectedGeneric(view);
    }

    @Override
    public void onNothingSelected(IgnAdapterView<?> parent) {
    }
}
