package net.laparola.ui.android.library;

import java.io.File;
import java.util.EnumSet;
import java.util.Locale;
import java.util.Objects;

import android.content.Context;
import net.laparola.R;
import net.laparola.core.ComponenteInformazioni;
import net.laparola.core.Testi;
import net.laparola.core.Testi.StatoAggiornamento;
import net.laparola.core.Testi.TestoTipi;
import net.laparola.ui.android.LaParolaPreferences;

public class LibraryItemInfo implements Comparable<LibraryItemInfo> {
	private ComponenteInformazioni mInfo;
	private String mBroken;
	private final Context mContext;

	public LibraryItemInfo(Context context, ComponenteInformazioni info) {
		mContext = context;
		mInfo = info;
	}

	public LibraryItemInfo(Context context, String broken) {
		mContext = context;
		mBroken = broken;
	}

	@Override
	public int compareTo(LibraryItemInfo another) {
		int lv = this.getTipoOrder();
		int rv = another.getTipoOrder();
		if (lv < rv) {
			return -1;
		} else if (lv > rv) {
			return 1;
		}

		lv = this.getStatoAggiornamentoOrder();
		rv = another.getStatoAggiornamentoOrder();
		if (lv < rv) {
			return -1;
		} else if (lv > rv) {
			return 1;
		}

		// TODO : getDescription al posto di getName
		return this.getName().compareTo(another.getName());
	}

	public String getDescription() {
		if (mInfo != null)
			return mInfo.getDescrizione();
		return "";
	}

	public String getFileName() {
		// Potrebbe essere sbagliato, ma è sempre corretto per i file corrotti.

		return String.format(Locale.getDefault(),"%s/%s.lpj", LaParolaPreferences.writeStoragePath, getName());
	}

	public float getFileSizeMB() {
		File file = new File(getFileName());

		if (file.exists()) {
			return file.length() / 1024f / 1024;
		}

		// Il file non esiste. Può succedere se è installato manualmente, ed il nome del file è
		// diverso dal nome nel contenuto.
		// In questo caso però posso usare getDimensione, che ha la dimensione del file.
		if (mInfo != null && getStatoAggiornamento() == StatoAggiornamento.NON_DISPONIBILE) {
			return mInfo.getDimensione() / 1024f / 1024;
		}

		return 0;
	}

	public String getMessage() {
		String message;
		if (!getTipo().contains(Testi.TestoTipi.NESSUNO)) {
			String r = getUpdateReason();
			String updateReason="";
			if (r != null && !r.isEmpty())
            	updateReason =  mContext.getString(R.string.update_reason, r);

			message = mContext.getString(R.string.component_details, getStateString(), getDescription(), getTipoString(), getVersion(), updateReason);
		} else {
			message = getStateString();
		}
		return message;
	}

	private String getUpdateReason() {
		if (mInfo == null)
			return null;
		return mInfo.getMotivo();
	}

	public String getName() {
		if (mInfo != null)
			return mInfo.getComponente();
		String fn = mBroken.substring(mBroken.lastIndexOf('/') + 1);
		return fn.substring(0, fn.lastIndexOf('.'));
	}

	public String getStateString() {
        return switch (getStatoAggiornamento()) {
            case AGGIORNAMENTO_NON_COMPATIBILE, INSTALLAZIONE_NON_COMPATIBILE ->
                    mContext.getString(R.string.update_not_compatible);
            case AGGIORNATO -> mContext.getString(R.string.installed, getFileSizeMB());
            case DA_AGGIORNARE ->
                    mContext.getString(R.string.update_available, getFileSizeMB(), getDownloadSizeMB());
            case NON_INSTALLATO -> mContext.getString(R.string.not_installed, getDownloadSizeMB());
            case FILE_CORROTTO -> mContext.getString(R.string.cannot_load_file, getFileSizeMB());
            case SENZA_INTERNET ->
                    mContext.getString(R.string.cannot_download_update_information, getFileSizeMB());
            default -> mContext.getString(R.string.installed_manually, getFileSizeMB());
        };
	}

	private float getDownloadSizeMB() {
		long i = LaParolaPreferences.useLzma ? getDownload1Size() : getDownload2Size();
		return i / 1024f / 1024;
	}

	public StatoAggiornamento getStatoAggiornamento() {
		if (mInfo == null)
			return StatoAggiornamento.FILE_CORROTTO;
		return mInfo.getStatoAggiornamento();
	}

	private int getStatoAggiornamentoOrder() {
        return switch (getStatoAggiornamento()) {
            case DA_AGGIORNARE -> 0;
            case AGGIORNAMENTO_NON_COMPATIBILE -> 1;
            case INSTALLAZIONE_NON_COMPATIBILE -> 2;
            case AGGIORNATO -> 3;
            case NON_INSTALLATO -> 4;
            case NON_DISPONIBILE -> 5;
            case SENZA_INTERNET -> 6;
            default -> 7;
        };
	}

	public EnumSet<TestoTipi> getTipo() {
		if (mInfo == null)
			return EnumSet.of(TestoTipi.NESSUNO);
		return mInfo.getTipo();
	}

	private int getTipoOrder() {
		EnumSet<TestoTipi> s = getTipo();
		if (s.contains(TestoTipi.BIBBIA))
			return 0;
		if (s.contains(TestoTipi.COMMENTARIO))
			return 1;
		if (s.contains(TestoTipi.DIZIONARIO))
			return 2;
		if (s.contains(TestoTipi.LIBRO))
			return 3;
		return 4;
	}

	public String getTipoString() {
		EnumSet<TestoTipi> s = getTipo();
		if (s.contains(TestoTipi.BIBBIA))
			return mContext.getString(R.string.type_bible);
		if (s.contains(TestoTipi.COMMENTARIO))
			return mContext.getString(R.string.type_commentario);
		if (s.contains(TestoTipi.DIZIONARIO))
			return mContext.getString(R.string.type_dictionary);
		if (s.contains(TestoTipi.LIBRO))
			return mContext.getString(R.string.type_book);
		return mContext.getString(R.string.type_unknown);
	}

	public String getUrl() {
		if (mInfo == null)
			return null;
		return mInfo.getUrl();
	}

	public String getUrl2() {
		if (mInfo == null)
			return null;
		return mInfo.getUrl2();
	}
	
	public String getVersion() {
		if (mInfo == null)
			return "N/A";
		return String.format(Locale.ENGLISH, "%d.%d.%d", mInfo.getVersione1(), mInfo.getVersione2(), mInfo.getVersione3());
	}

	@SuppressWarnings("incomplete-switch")
	public boolean showDelete() {
        return switch (getStatoAggiornamento()) {
            case AGGIORNAMENTO_NON_COMPATIBILE, AGGIORNATO, DA_AGGIORNARE, NON_DISPONIBILE,
                 FILE_CORROTTO, SENZA_INTERNET -> true;
            default -> false;
        };
    }

	@SuppressWarnings("incomplete-switch")
	public boolean showInstall() {
        return Objects.requireNonNull(getStatoAggiornamento()) == StatoAggiornamento.NON_INSTALLATO;
    }

	@SuppressWarnings("incomplete-switch")
	public boolean showUpdate() {
        return Objects.requireNonNull(getStatoAggiornamento()) == StatoAggiornamento.DA_AGGIORNARE;
    }
	
	@SuppressWarnings("incomplete-switch")
	public boolean showMarket() {
        return switch (getStatoAggiornamento()) {
            case AGGIORNAMENTO_NON_COMPATIBILE, INSTALLAZIONE_NON_COMPATIBILE -> true;
            default -> false;
        };
    }

	public long getDownload1Size() {
		if (mInfo == null)
			return 0;
		return mInfo.getDimensione();
	}

	public long getDownload2Size() {
		if (mInfo == null)
			return 0;
		return mInfo.getDimensione2();
	}
	
	public String getDownload1FileType() {
		String url = getUrl();
		if (url == null)
			return null;
		if (url.endsWith(".zip"))
			return "zip";
		if (url.endsWith(".lzma"))
			return "lzma";
		return null;
	}

	public String getDownload2FileType() {
		String url = getUrl2();
		if (url == null)
			return null;
		if (url.endsWith(".zip"))
			return "zip";
		if (url.endsWith(".lzma"))
			return "lzma";
		return null;
	}
}
