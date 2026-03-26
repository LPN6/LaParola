package net.laparola.ui.android;

import android.os.Bundle;

import net.laparola.R;

import androidx.preference.PreferenceFragmentCompat;

public class LaParolaPreferencesFragment extends PreferenceFragmentCompat {
    //public class LaParolaPreferencesFragment extends PreferenceFragmentCompat implements Preference.OnPreferenceChangeListener {
    @Override
    public void onCreatePreferences(Bundle savedInstanceState, String rootKey) {
        getPreferenceManager().setSharedPreferencesName(LaParolaPreferences.LAPAROLA_PREFERENCES);
        setPreferencesFromResource(R.xml.preferences, rootKey);
/* no longer necessary, SummaryProvider set in prefences.xml
        for (String key : new String[]{"referenceType", "referencePlacement"}) {
            ListPreference lp = findPreference(key);
            if (lp != null) {
                lp.setSummaryProvider(ListPreference.SimpleSummaryProvider.getInstance());
            }
        }
        */
    }
}
