package net.laparola.ui.android;

import net.laparola.R;
import android.os.Bundle;
import androidx.appcompat.app.ActionBar;
import androidx.appcompat.app.AppCompatActivity;

public class LaParolaPreferencesActivity extends AppCompatActivity {

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.preferences_activity_container);

		androidx.appcompat.widget.Toolbar toolbar = findViewById(R.id.preferences_toolbar);
		setSupportActionBar(toolbar);

		ActionBar ab = getSupportActionBar();
		if (ab != null) {
			ab.setDisplayHomeAsUpEnabled(true); // show back arrow
			ab.setDisplayShowHomeEnabled(true);
			ab.setLogo(R.drawable.ic_launcher);  // show app icon
			ab.setDisplayUseLogoEnabled(true);
			//ab.setTitle(R.string.menu_impostazioni);
		}

		getSupportFragmentManager()
				.beginTransaction()
				.replace(R.id.preferences_container, new LaParolaPreferencesFragment())
				.commit();
	}

	@Override
	public boolean onSupportNavigateUp() {
		finish(); // closes the activity when the arrow is pressed
		return true;
	}
}
