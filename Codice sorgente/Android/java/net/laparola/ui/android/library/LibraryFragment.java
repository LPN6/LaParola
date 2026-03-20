package net.laparola.ui.android.library;

import net.laparola.R;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ListView;
import android.widget.TextView;

import androidx.fragment.app.Fragment;

public class LibraryFragment extends Fragment {
	private ListView mListView;
	private TextView mNoBookTextView;
	private LibraryAdapter mLibraryAdapter; 

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }
    
    @Override
    public void onSaveInstanceState(Bundle outState) {
        super.onSaveInstanceState(outState);
    }
    
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        View res = inflater.inflate(R.layout.components_fragment, container, false);
        mListView = res.findViewById(R.id.listView);
        mNoBookTextView = res.findViewById(R.id.no_book_available);
        
        if (mLibraryAdapter != null)
        	setAdapter(mLibraryAdapter);
        
        return res;
    }

	public void setAdapter(LibraryAdapter libraryAdapter) {
		mLibraryAdapter = libraryAdapter;
		if (mListView != null) {
			mListView.setAdapter(libraryAdapter);
			if (libraryAdapter.getCount() == 0) {
				mNoBookTextView.setVisibility(View.VISIBLE);
			} else {
				mNoBookTextView.setVisibility(View.GONE);
			}
		}
	}
}
