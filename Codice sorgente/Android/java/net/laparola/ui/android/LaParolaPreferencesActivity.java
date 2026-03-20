package net.laparola.ui.android;

import net.laparola.R;
import android.os.Bundle;
import android.preference.ListPreference;
import android.preference.Preference;
import android.preference.Preference.OnPreferenceChangeListener;
import android.preference.PreferenceActivity;

@SuppressWarnings("deprecation")
public class LaParolaPreferencesActivity extends PreferenceActivity implements OnPreferenceChangeListener {
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		
        getPreferenceManager().setSharedPreferencesName(LaParolaPreferences.LAPAROLA_PREFERENCES);
        
		addPreferencesFromResource(R.xml.preferences);
		
		for (String k : new String[] {"referenceType", "referencePlacement"}) {
			ListPreference lp = (ListPreference)findPreference(k);
			lp.setSummary(lp.getEntry());
			lp.setOnPreferenceChangeListener(this);
		}
	}

	@Override
	public boolean onPreferenceChange(Preference preference, Object newValue) {
		if (preference instanceof ListPreference) {
			ListPreference lp = (ListPreference)preference;
			
			CharSequence[] values = lp.getEntryValues();
			for (int i = 0; i < values.length; i++) {
				if (values[i].equals(newValue)) {
					lp.setSummary(lp.getEntries()[i]);
					break;
				}
			}
		}
		
		return true;
	}
}
