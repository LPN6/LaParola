package net.laparola.ui.android.dialogs;

import net.laparola.R;
import net.laparola.ui.android.LaParolaActivity;
import net.laparola.ui.android.actionbar.ReferenceActionItemManager.BookSpinnerAdapter;
import net.laparola.ui.android.actionbar.ReferenceActionItemManager.ChapterSpinnerAdapter;
import net.laparola.ui.android.actionbar.ReferenceActionItemManager.VerseSpinnerAdapter;
import net.laparola.ui.android.ignspinner.GridSpinner;
import net.laparola.ui.android.ignspinner.IgnAdapterView;
import net.laparola.ui.android.ignspinner.IgnAdapterView.OnItemSelectedListener;
import net.laparola.ui.android.ignspinner.ListSpinner;
import android.annotation.SuppressLint;
import android.content.DialogInterface;
import android.os.Bundle;
import android.view.KeyEvent;
import android.view.View;
import android.view.WindowManager;
import android.view.inputmethod.EditorInfo;
import android.widget.EditText;
import android.widget.TabHost;
import android.widget.TextView;
import android.widget.TabHost.TabSpec;
import android.widget.TextView.OnEditorActionListener;

public class AccessibilityDialog extends HoloDialog implements android.content.DialogInterface.OnClickListener, OnItemSelectedListener, OnEditorActionListener {
	private ListSpinner bookSpinner;
	private BookSpinnerAdapter bookSpinnerAdapter;
	private GridSpinner chapterSpinner;
	private ChapterSpinnerAdapter chapterSpinnerAdapter;
	private GridSpinner verseSpinner;
	private VerseSpinnerAdapter verseSpinnerAdapter;
	private LaParolaActivity parent;
	private boolean ignoreBookSelection;
	private boolean ignoreChapterSelection;
	private boolean ignoreVerseSelection;
	private EditText referenceEditText;

	public AccessibilityDialog(LaParolaActivity context) {
		super(context, true);
		parent = context;
	}

	@SuppressLint("InlinedApi")
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		
		float scaledDensity = parent.getResources().getDisplayMetrics().scaledDensity;
		float fs = parent.getResources().getConfiguration().fontScale;
		int textSize = 48;
		int columnWidth = Math.round(2.0f * textSize * scaledDensity * fs);
		
		setContentView(R.layout.accessibility_dialog);
		setTitle(R.string.reference);
		setYesNo(R.string.close, 0, null, null);
		button1.setTextSize(textSize);

		bookSpinnerAdapter = new BookSpinnerAdapter(parent);
		chapterSpinnerAdapter = new ChapterSpinnerAdapter(parent);
		verseSpinnerAdapter = new VerseSpinnerAdapter(parent);
		
		bookSpinnerAdapter.setTextSize(textSize);
		chapterSpinnerAdapter.setTextSize(textSize);
		verseSpinnerAdapter.setTextSize(textSize);
		
		bookSpinner = findViewById(R.id.book_spinner);
		bookSpinner.setAdapter(bookSpinnerAdapter);
		bookSpinner.setOnItemSelectedListener(this);

		chapterSpinner = findViewById(R.id.chapter_spinner);
		chapterSpinner.setAdapter(chapterSpinnerAdapter);
		chapterSpinner.setOnItemSelectedListener(this);
		chapterSpinner.setColumnWidth(columnWidth);
		
		verseSpinner = findViewById(R.id.verse_spinner);
		verseSpinner.setAdapter(verseSpinnerAdapter);
		verseSpinner.setOnItemSelectedListener(this);
		verseSpinner.setColumnWidth(columnWidth);
		
		bookSpinner.setSelection(0);
		chapterSpinner.setEnabled(false);
		verseSpinner.setEnabled(false);

		bookSpinner.setPopupCentered(true);
		chapterSpinner.setPopupCentered(true);
		verseSpinner.setPopupCentered(true);

		referenceEditText = findViewById(R.id.reference_edittext);
		referenceEditText.setOnEditorActionListener(this);
		referenceEditText.setTextSize(textSize);
		
		
		getWindow().setLayout(WindowManager.LayoutParams.MATCH_PARENT,
                WindowManager.LayoutParams.MATCH_PARENT);
		

		int[] lcv = null;
        try {
        	lcv = parent.getActiveFragment().getUrlCorrente().getLCV();
        } catch (NullPointerException e) {}
		if (lcv != null) {
			int b = lcv[0];
			int c = lcv[1];
			int v = lcv[2];

			select(b, c, v);
		} else {
			select(0, 0, 0);
		}


		TabHost tabHost = findViewById(android.R.id.tabhost);
		tabHost.setup();
		
		String t1name = parent.getString(R.string.basic);
        TabSpec t1spec = tabHost.newTabSpec(t1name);
        t1spec.setIndicator(t1name);
        t1spec.setContent(R.id.tab1);
        tabHost.addTab(t1spec);
        
		String t2name = parent.getString(R.string.advanced);
        TabSpec t2spec = tabHost.newTabSpec(t2name);
        t2spec.setIndicator(t2name);
        t2spec.setContent(R.id.tab2);
        tabHost.addTab(t2spec);
	}
	
	@Override
    public boolean onEditorAction(TextView v, int actionId, KeyEvent event) {
        if ((event != null &&
             event.getAction() == KeyEvent.ACTION_DOWN && 
             event.getKeyCode() == KeyEvent.KEYCODE_ENTER) ||
            (actionId == EditorInfo.IME_ACTION_GO)) {
            
            parent.getActiveFragment().vaiARiferimento(referenceEditText.getText());
            dismiss();
            return true;
        }

        return false;
    }
    
	@Override
	public void onClick(DialogInterface arg0, int arg1) {}
    
    protected void onItemSelectedGeneric(Object view, View itemview, int position, long id) {
    	int b = (int)bookSpinner.getSelectedItemId();
    	int c = (int)chapterSpinner.getSelectedItemId();
    	int v = (int)verseSpinner.getSelectedItemId();
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
	        		chapterSpinner.setEnabled(true);
	        		chapterSpinner.setSelection(1);
	        		verseSpinner.setEnabled(true);
	        		verseSpinner.setSelection(1);
                    load = true;
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
	        		verseSpinner.setEnabled(true);
	        		verseSpinner.setSelection(1);
                    load = true;
	        	}
        	}
        	ignoreChapterSelection = false;
        } else if (view == verseSpinner) {
        	if (!ignoreVerseSelection) {
                load = true;
	    	}
        	ignoreVerseSelection = false;
        }
        
        if (load && b != 0 && c != 0 && v != 0) {
            parent.getActiveFragment().vaiALibroCapitoloVersetto(b, c, v);
        }
    }

	@Override
	public void onItemSelected(IgnAdapterView<?> parent, View view,	int position, long id) {
    	onItemSelectedGeneric(parent, view, position, id);
	}

	@Override
	public void onNothingSelected(IgnAdapterView<?> parent) {}

    public void select (int b, int c, int v) {
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
	        if (ok == false) {
	        	b = (int)bookSpinner.getSelectedItemId();
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
}
