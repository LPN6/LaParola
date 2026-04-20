package net.laparola.ui.android;

import android.app.DatePickerDialog;
import android.content.Context;
import android.content.pm.PackageInfo;
import android.content.pm.PackageManager.NameNotFoundException;
import android.os.Environment;
import android.webkit.JavascriptInterface;

import net.laparola.R;
import net.laparola.ui.LaParolaBrowser;
import net.laparola.ui.LaParolaJavascriptInterface;
import net.laparola.ui.LaParolaSegnalibri;
import net.laparola.ui.LaParolaUrl;
import net.laparola.ui.android.bibleview.BibleView;

import java.io.BufferedWriter;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStreamWriter;
import java.nio.charset.StandardCharsets;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.List;
import java.util.Scanner;

import timber.log.Timber;

public class LaParolaJavascriptInterfaceAndroid implements LaParolaJavascriptInterface {
	private final BibleView mBibleView;
	private long mUltimaDataLiturgia = 0;
	
	public LaParolaJavascriptInterfaceAndroid(BibleView bibleView) {
		mBibleView = bibleView;
	}
	
	@Override
	@JavascriptInterface
	public String getSegnalibriCasuali(int n) {
		StringBuilder sb = new StringBuilder();
		List<LaParolaSegnalibri.GruppoSegnalibri> g = LaParolaBrowser.getGruppiSegnalibri();

		if (g.size() < n) return "";
		
		List<Integer> fatti = new ArrayList<>();
		int s;
		for (int i = 0; i < n; i++) {
			do {
				s = (int) Math.floor(Math.random() * g.size());
			} while (fatti.contains(s));
			fatti.add(s);
			LaParolaSegnalibri.appendLinkGruppo(sb, g.get(s));
		}

		return sb.toString();
	}

	@Override
	@JavascriptInterface
	public String getVersettoCasuale () {
		return getVersettoCasuale(1, 73);
	}
	
	@Override
	@JavascriptInterface
	public String getVersettoCasuale (int minlibro, int maxlibro) {
		LaParolaUrl ret = mBibleView.getBrowser().getVersettoCasuale(minlibro, maxlibro);
		if (ret == null)
			return "";
		return ret.getUrl();
	}
	
	@Override
	@JavascriptInterface
	public String getVersioneProgramma() {
		PackageInfo packageInfo;
		try {
            Context context = mBibleView.getContext();
			packageInfo = context.getPackageManager().getPackageInfo(context.getPackageName(), 0);
			return packageInfo.versionName;
		} catch (NameNotFoundException e) {
			return "";
		}
	}
	
	@Override
	@JavascriptInterface
	public String normalizzaRiferimento (String riferimento, String versione) {
		return LaParolaBrowser.normalizzaRiferimento(riferimento, versione);
	}
	
	@Override
	@JavascriptInterface
	public String normalizzaRiferimento (String riferimento) {
		return LaParolaBrowser.normalizzaRiferimento(riferimento, getVersione());
	}
	
	@Override
	@JavascriptInterface
	public String convertiRiferimentoAStandardVirgola (String riferimento, String versione) {
		return LaParolaBrowser.convertiRiferimentoAStandardVirgola(riferimento, versione);
	}
	
	@Override
	@JavascriptInterface
	public String getVersione () {
		return mBibleView.getBrowser().getVersione();
	}
	
	@Override
	@JavascriptInterface
	public boolean getAggionamentiDisponibiliDebole () {
		// Restituisce se esistono aggiornamenti o meno, ma non blocca l'esecuzione:
		// se il file aggiorna.xml è in cache restituisce il risultato, altrimenti 
		// restituisce no e scarica il file in un thread separato. 
		
		String cacheFileName = LaParolaPreferences.writeStoragePath + "/aggiorna.xml.cachedebole";
		
		return LaParolaBrowser.getAggionamentiDisponibiliDebole(cacheFileName);
	}
	
	@Override
	@JavascriptInterface
	public void notificaPrimoSegnalibroVisible (String s) {
		mBibleView.onScrolledToAnchor(s);
	}
	
	@Override
	@JavascriptInterface
	public void cambiaEvidenziatore (final String versetto) {
		mBibleView.post(() -> {
            boolean e = mBibleView.getBrowser().Evidenziatore.cambiaEvidenziazioneVersetto(versetto);

            LaParolaActivity laParolaActivity = (LaParolaActivity)mBibleView.getContext();
            for (int i = 0; i < laParolaActivity.fragments.size(); i++)
                laParolaActivity.fragments.get(i).evidenziaVersetto(versetto, e, mBibleView.getNightMode());
        });
	}

	@Override
	@JavascriptInterface
	public boolean isRiferimento(String rif) {
		return LaParolaBrowser.isRiferimento(rif, LaParolaBrowser.getUltimaBibbia());
	}

	@Override
	@JavascriptInterface
	public long getUltimaDataLiturgia() {
		if (mUltimaDataLiturgia == 0)
			return (new Date()).getTime();
		return mUltimaDataLiturgia;
	}

	@Override
	@JavascriptInterface
	public void sceltaDataLiturgia() {
		Context context = mBibleView.getContext();
		
        final Calendar c = Calendar.getInstance();
        int year = c.get(Calendar.YEAR);
        int month = c.get(Calendar.MONTH);
        int day = c.get(Calendar.DAY_OF_MONTH);		
        
        DatePickerDialog.OnDateSetListener l = (view, year1, monthOfYear, dayOfMonth) -> {
            Calendar c1 = Calendar.getInstance();
            c1.set(year1, monthOfYear, dayOfMonth);
            mUltimaDataLiturgia = c1.getTimeInMillis();

            mBibleView.post(() -> mBibleView.executeJavascript("liturgia(" + mUltimaDataLiturgia + ");"));
        };
        
        DatePickerDialog d = new DatePickerDialog(context, l, year, month, day);
		int alertColor = androidx.core.content.ContextCompat.getColor(context, R.color.colorOnSurface);
		d.show();
		d.getButton(DatePickerDialog.BUTTON_POSITIVE).setTextColor(alertColor);
		d.getButton(DatePickerDialog.BUTTON_NEGATIVE).setTextColor(alertColor);
	}
	
	@Override
    @JavascriptInterface
	public void scriviFile(String nome, String contenuto) {
		nome = Environment.getExternalStorageDirectory() + "/" + nome;

        try (BufferedWriter writer = new BufferedWriter(new OutputStreamWriter(new FileOutputStream(nome), StandardCharsets.UTF_8))) {
            try {
                writer.write(contenuto);
            } catch (IOException ex) {
				Timber.e(ex, "Unexpected IO error occurred while writing file.");
            }
        } catch (Exception e) {
			Timber.e(e, "Unexpected  error occurred while creating write buffer.");
        }
	}

    @Override
    @JavascriptInterface
    public String leggiFile(String nome) {
        LaParolaActivity context = (LaParolaActivity)mBibleView.getContext();
        InputStream is = context.apriFile(nome);
        Scanner s = new Scanner(is, "UTF-8").useDelimiter("\\A");
        return s.hasNext() ? s.next() : "";
    }

    @Override
    @JavascriptInterface
    public void toccoLungoSuSfondo() {
        LaParolaActivity activity = (LaParolaActivity)mBibleView.getContext();
		activity.runOnUiThread(activity::showPanelBottomSheet);
    }

    @Override
    @JavascriptInterface
    public void logd(String s) {
        Timber.tag("laparola").d(s);
    }
}
