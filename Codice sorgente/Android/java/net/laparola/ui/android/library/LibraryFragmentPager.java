package net.laparola.ui.android.library;

import java.util.Locale;

import net.laparola.R;
import android.content.Context;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentPagerAdapter;

public class LibraryFragmentPager extends FragmentPagerAdapter {
    protected static String[] TABNAMES = null;
    
    protected LibraryFragment[] mFragments; 
    
    public LibraryFragmentPager(FragmentManager fm, Context context) {
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

    @Override
    public Fragment getItem(int position) {
    	return mFragments[position];
    }

    @Override
    public CharSequence getPageTitle(int position) {
        return TABNAMES[position % TABNAMES.length].toUpperCase(Locale.getDefault());
    }
    
    @Override
    public int getCount() {
        return TABNAMES.length;
    }


	public void setAdapters(LibraryAdapter bibbieAdapter, LibraryAdapter commentariAdapter, LibraryAdapter dizionariAdapter) {
		mFragments[0].setAdapter(bibbieAdapter);
		mFragments[1].setAdapter(commentariAdapter);
        //rmw1024 mFragments[2].setAdapter(dizionariAdapter);
	}
}
