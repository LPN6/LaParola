package net.laparola.ui.android.library;

import java.util.Locale;

import net.laparola.R;
import android.content.Context;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;
import androidx.fragment.app.FragmentActivity;
import androidx.viewpager2.adapter.FragmentStateAdapter;

public class LibraryFragmentPager extends FragmentStateAdapter {
    protected static String[] TABNAMES = null;
    
    protected LibraryFragment[] mFragments; 
    
    public LibraryFragmentPager(FragmentActivity fm, Context context) {
        super(fm);
        
        if (TABNAMES == null) {
        	TABNAMES = new String[] {
                    context.getString(R.string.type_bible),
                    context.getString(R.string.type_commentario),
        			//context.getString(R.string.type_bible),
        			//context.getString(R.string.type_commentario),
                    //rmw1024 context.getString(R.string.type_dictionary)
        	};
        }
        
        mFragments = new LibraryFragment[2]; // //rmw1024 era 3
        mFragments[0] = new LibraryFragment();
        mFragments[1] = new LibraryFragment();
        //rmw1024 mFragments[2] = new LibraryFragment();
    }

    @NonNull
    @Override
    public Fragment createFragment(int position) {
        // return a new Fragment instance for each page
        return switch (position) {
            case 0 -> mFragments[0];
            case 1 -> mFragments[1];
            //rmw1024 case 2: return mFragments[2];
            default -> mFragments[0];
        };
    }

    public Fragment getItem(int position) {
    	return mFragments[position];
    }

    public CharSequence getPageTitle(int position) {
        return TABNAMES[position % TABNAMES.length].toUpperCase(Locale.getDefault());
    }

    public int getCount() {
        return TABNAMES.length;
    }

    @Override
    public int getItemCount() {
        return TABNAMES.length;
    }

    public void setAdapters(LibraryAdapter bibbieAdapter, LibraryAdapter commentariAdapter, LibraryAdapter dizionariAdapter) {
		mFragments[0].setAdapter(bibbieAdapter);
		mFragments[1].setAdapter(commentariAdapter);
        //rmw1024 mFragments[2].setAdapter(dizionariAdapter);
	}
}
