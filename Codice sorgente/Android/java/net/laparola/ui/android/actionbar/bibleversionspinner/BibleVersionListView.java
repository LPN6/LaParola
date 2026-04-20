package net.laparola.ui.android.actionbar.bibleversionspinner;

import net.laparola.R;
import net.laparola.core.Testi.TestoTipi;
import net.laparola.ui.android.ignspinner.IgnHijackFocusListView;
import net.laparola.ui.android.library.LibraryActivity;

import android.content.Context;
import android.content.Intent;
import android.graphics.Typeface;
import android.view.Gravity;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.AbsListView;
import android.widget.Button;
import android.widget.LinearLayout;

class BibleVersionListView extends IgnHijackFocusListView implements OnClickListener {
    public interface OnTypeChangedListener {
        void OnTypeChanged(TestoTipi tipo);
    }

    private OnTypeChangedListener mOnTypeChangedListener;

    private LinearLayout mButtons;
    private Button mBibleButton;
    private Button mCommentarioButton;
    //rmw1024 private Button mDizionarioButton;
    private View mSeparator;
    private Button mLibraryButton;

    public static boolean sNextLibraryVisible = true;
    public boolean libraryVisible;

    public BibleVersionListView(Context context, boolean hijackFocus) {
        super(context, hijackFocus);
    }

    public BibleVersionListView(Context context) {
        this(context, false);
    }

    @Override
    public void onClick(View v) {
        if (v == mLibraryButton) {
            Context context = getContext();
            Intent intent = new Intent(context, LibraryActivity.class);
            context.startActivity(intent);
            mOnItemSelectedListener.onNothingSelected(null);
            return;
        }

        // workaround per un bug
        mBibleButton.setGravity(Gravity.NO_GRAVITY);
        mBibleButton.setGravity(Gravity.CENTER);

        if (v.isSelected()) {
            return;
        }

        TestoTipi r = TestoTipi.BIBBIA;
        //if (v == mBibleButton) {
        //	r = TestoTipi.BIBBIA;
        //} else
        if (v == mCommentarioButton) {
            r = TestoTipi.COMMENTARIO;
        }
        // rmw1024 else if (v == mDizionarioButton) {
        // rmw1024			 r = TestoTipi.DIZIONARIO;
        // rmw1024}

        setSelectedType(r);
        if (mOnTypeChangedListener != null) {
            mOnTypeChangedListener.OnTypeChanged(r);
        }
    }

    //public OnTypeChangedListener getOnTypeChangedListener() {return mOnTypeChangedListener;}

    public void setOnTypeChangedListener(OnTypeChangedListener listener) {
        mOnTypeChangedListener = listener;
    }

    public void setSelectedType(final TestoTipi tipo) {
        mButtons.post(() -> {
            switch (tipo) {
                case BIBBIA:
                case LIBRO:
                case NESSUNO:
                    mBibleButton.setSelected(true);
                    mBibleButton.setTypeface(null, Typeface.BOLD);
                    mCommentarioButton.setSelected(false);
                    mCommentarioButton.setTypeface(null, Typeface.NORMAL);
                    //rmw1024 mDizionarioButton.setSelected(false);
                    //rmw1024 mDizionarioButton.setTypeface(null, Typeface.NORMAL);
                    break;
                case COMMENTARIO:
                    mBibleButton.setSelected(false);
                    mBibleButton.setTypeface(null, Typeface.NORMAL);
                    mCommentarioButton.setSelected(true);
                    mCommentarioButton.setTypeface(null, Typeface.BOLD);
                    //rmw1024 mDizionarioButton.setSelected(false);
                    //rmw1024 mDizionarioButton.setTypeface(null, Typeface.NORMAL);
                    break;
                case DIZIONARIO:
                    mBibleButton.setSelected(false);
                    mBibleButton.setTypeface(null, Typeface.NORMAL);
                    mCommentarioButton.setSelected(false);
                    mCommentarioButton.setTypeface(null, Typeface.NORMAL);
                    //rmw1024 mDizionarioButton.setSelected(true);
                    //rmw1024 mDizionarioButton.setTypeface(null, Typeface.BOLD);
                    break;
            }
        });
    }

    @Override
    protected int getHeaderHeight(int widthMeasureSpec) {
        int buttonsHeight = 0;
        if (mButtons != null && mButtons.getVisibility() == View.VISIBLE) {
            mBibleButton.measure(widthMeasureSpec, LayoutParams.WRAP_CONTENT);
            buttonsHeight = mBibleButton.getMeasuredHeight();
            if (libraryVisible) {
                mSeparator.measure(widthMeasureSpec, 0);
                mLibraryButton.measure(widthMeasureSpec, 0);
                buttonsHeight += mSeparator.getMeasuredHeight();
                buttonsHeight += mLibraryButton.getMeasuredHeight();
            }
        }
        return buttonsHeight;
    }

    @Override
    protected AbsListView createViews(Context context) {
        this.libraryVisible = sNextLibraryVisible;

        inflate(context, R.layout.version_chooser, this);

        mButtons = findViewById(R.id.buttons);
        mBibleButton = mButtons.findViewById(R.id.button_bible);
        mCommentarioButton = mButtons.findViewById(R.id.button_commentario);
        //rmw1024 mDizionarioButton = mButtons.findViewById(R.id.button_dizionario);
        mBibleButton.setOnClickListener(this);
        mCommentarioButton.setOnClickListener(this);
        //rmw1024 mDizionarioButton.setOnClickListener(this);
        mListView = findViewById(R.id.list_view);
        mSeparator = findViewById(R.id.separator);
        mLibraryButton = findViewById(R.id.library_button);

        if (libraryVisible) {
            mLibraryButton.setOnClickListener(this);
        } else {
            mSeparator.setVisibility(GONE);
            mLibraryButton.setVisibility(GONE);
        }

        return mListView;
    }
}