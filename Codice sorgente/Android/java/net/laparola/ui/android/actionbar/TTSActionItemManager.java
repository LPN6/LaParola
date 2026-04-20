package net.laparola.ui.android.actionbar;

import java.util.HashMap;
import java.util.List;
import java.util.Locale;

import net.laparola.R;
import net.laparola.ui.LaParolaUrl;
import net.laparola.ui.android.LaParolaActivity;
import net.laparola.ui.android.LaParolaFragment;
import net.laparola.ui.android.LaParolaFragment.AnchorAndText;
import net.laparola.ui.android.LaParolaPreferences;

import android.content.Context;
import android.content.Intent;
import android.media.AudioManager;
import android.speech.tts.TextToSpeech;
import android.view.*;
import android.view.View.OnClickListener;
import android.widget.*;

import androidx.annotation.NonNull;
import androidx.core.content.ContextCompat;

@SuppressWarnings("deprecation")
public class TTSActionItemManager implements MenuItem.OnActionExpandListener, OnClickListener, TextToSpeech.OnInitListener, TextToSpeech.OnUtteranceCompletedListener {
    private static final int HUGE = 30000;

    LaParolaActivity parent;
    private final MenuItem ttsMenuItem;
    private final View readButton;
    private final View stopButton;
    private final View rewindButton;
    private final View ffButton;
    View settingsButton;
    private TextToSpeech mTts;
    private List<AnchorAndText> mPlayList;
    private int mPlayListIndex;
    private final TTSSettingsPopup mPopup;
    private LaParolaFragment mParentFragment;

    public TTSActionItemManager(Context context, LaParolaActivity parent, MenuItem item) {
        this.parent = parent;
        this.ttsMenuItem = item;
        LinearLayout searchActionView = (LinearLayout) ttsMenuItem.getActionView();

        readButton = searchActionView.findViewById(R.id.tts_read_button);
        readButton.setOnClickListener(this);

        stopButton = searchActionView.findViewById(R.id.tts_stop_button);
        stopButton.setOnClickListener(this);

        rewindButton = searchActionView.findViewById(R.id.tts_rewind_button);
        rewindButton.setOnClickListener(this);

        ffButton = searchActionView.findViewById(R.id.tts_ff_button);
        ffButton.setOnClickListener(this);

        settingsButton = searchActionView.findViewById(R.id.tts_settings_button);
        settingsButton.setOnClickListener(this);

        setButtonsVisibility();

        mPopup = new TTSSettingsPopup(this, parent);
        mPopup.setBackgroundDrawable(ContextCompat.getDrawable(context, R.drawable.abc_popup_background_mtrl_mult));

        ttsMenuItem.setOnActionExpandListener(this);
    }

    private void readTTS() {
        if (mTts == null) {
            mTts = new TextToSpeech(parent, this);
        } else {
            mParentFragment = parent.getActiveFragment();
            if (mParentFragment == null)
                return;

            String lang = mParentFragment.getInformazioniVersione().getLingua();
            Locale locale = new Locale(lang);
            int result = mTts.setLanguage(locale);

            if (result == TextToSpeech.LANG_MISSING_DATA || result == TextToSpeech.LANG_NOT_SUPPORTED) {
                String s = parent.getString(R.string.tts_unsupported_language, "\"" + lang + "\"");
                Toast.makeText(parent, s, Toast.LENGTH_LONG).show();
                startInstallLanguage();
                return;
            }

            initTextToRead();
            readCurrent();
        }
    }

    private void readCurrent() {
        synchronized (this) {
            if (mPlayList == null)
                return;

            setButtonsVisibility();

            if (mPlayListIndex < 0) {
                // sono tornato troppo indietro
                mPlayListIndex = 0;
            } else if (mPlayListIndex >= HUGE) {
                // ho finito
                return;
            } else if (mPlayListIndex >= mPlayList.size()) {
                // o sono andato troppo avanti

                if (LaParolaPreferences.ttsStopEndChapter)
                    return;

                if (mParentFragment != null && mParentFragment.isVisible()) {
                    LaParolaUrl urlCorrente = mParentFragment.getUrlCorrente();
                    if (urlCorrente != null) {
                        String urlSuccessivo = urlCorrente.getUrlSuccessivo();
                        if (urlSuccessivo != null) {
                            mParentFragment.vaiAdUrl(urlSuccessivo);
                            mParentFragment.onProssimaPaginaCaricata = this::readTTS;
                        }
                    }
                }
                return;
            }

            String s = mPlayList.get(mPlayListIndex).text;

            // queste correzioni servono ad evitare errori del sintetizzatore
            s = s.replace('«', '"');
            s = s.replace('»', '"');
            s = s.replace(" - ", ", ");

            HashMap<String, String> params = new HashMap<>();
            params.put(TextToSpeech.Engine.KEY_PARAM_UTTERANCE_ID, "1");   // altrimenti non viene chiamato onUtteranceCompleted

            if (LaParolaPreferences.ttsFollowVerse && mParentFragment != null && mParentFragment.isVisible()) {
                String link = mPlayList.get(mPlayListIndex).link;
                if (link != null && link.contains("#")) {
                    String[] tmp = link.split("#");
                    mParentFragment.vaiASegnalibro(tmp[tmp.length - 1]);
                }
            }

            mTts.speak(s, TextToSpeech.QUEUE_FLUSH, params);
        }
    }

    private void setButtonsVisibility() {
        final int rv, sv;
        final boolean keepOn;

        if (mPlayList == null || mPlayListIndex < 0 || mPlayListIndex >= mPlayList.size()) {
            rv = View.VISIBLE;
            sv = View.GONE;
            keepOn = false;
        } else {
            rv = View.GONE;
            sv = View.VISIBLE;
            keepOn = true;
        }

        readButton.post(() -> {
            if (mParentFragment != null) {
                View view = mParentFragment.getView();
                if (view != null)
                    view.setKeepScreenOn(keepOn);
            }
            readButton.setVisibility(rv);
            rewindButton.setVisibility(sv);
            stopButton.setVisibility(sv);
            ffButton.setVisibility(sv);
        });
    }

    private void initTextToRead() {
        synchronized (this) {
            LaParolaFragment activeFragment = parent.getActiveFragment();
            if (activeFragment == null)
                return;

            mPlayList = activeFragment.getPlainTextWithAnchors();
            if (mPlayList == null)
                return;

            LaParolaUrl urlCorrente = activeFragment.getUrlCorrente();
            if (urlCorrente == null)
                return;


            if (urlCorrente.ancoraggio.equals("inizio")) {
                mPlayListIndex = 0;
            } else {
                String currentAnchor = "#" + urlCorrente.ancoraggio;

                mPlayListIndex = -1;

                for (int i = 0; i < mPlayList.size(); i++) {
                    AnchorAndText v = mPlayList.get(i);
                    if (v.link != null && v.link.endsWith(currentAnchor)) {
                        mPlayListIndex = i;
                        break;
                    }
                }

                if (mPlayListIndex == -1) {
                    mPlayListIndex = 0;
                }
            }
        }
    }

    private void showTTSSettings() {
        if (!mPopup.isShowing()) {
            mPopup.show();
        }
    }

    public void onClick(View view) {
        synchronized (this) {
            if (view == readButton) {
                readTTS();
            } else if (view == stopButton) {
                stop();
            } else if (view == rewindButton) {
                mPlayListIndex--;
                readCurrent();
            } else if (view == ffButton) {
                if (mTts != null) {
                    mTts.stop();
                }
                // va avanti in automatico perché viene
                // chiamato onUtteranceCompleted
            } else if (view == settingsButton) {
                showTTSSettings();
            }
        }
    }

    public void stop() {
        synchronized (this) {
            if (mTts != null) {
                mTts.stop();
            }
            mPlayListIndex = HUGE;
            setButtonsVisibility();
        }
    }

    public void destroy() {
        if (mTts != null)
            mTts.shutdown();
        mTts = null;
    }

    public boolean isExpanded() {
        return ttsMenuItem.isActionViewExpanded();
    }

    public boolean onMenuItemActionExpand(@NonNull MenuItem item) {
        parent.setVolumeControlStream(AudioManager.STREAM_MUSIC);
        return parent.collapseActionViewsExcept(item);
    }

    public boolean onMenuItemActionCollapse(@NonNull MenuItem item) {
        parent.setVolumeControlStream(AudioManager.USE_DEFAULT_STREAM_TYPE);
        /*if (mPopup.isShowing())
    		mPopup.dismiss();*/
        return true;
    }

    public void collapse(MenuItem exclude) {
        if (exclude != ttsMenuItem) {
            ttsMenuItem.collapseActionView();
        }
    }

    public void expandActionView() {
        ttsMenuItem.expandActionView();
    }

    @Override
    public void onUtteranceCompleted(String utteranceId) {
        synchronized (this) {
            mPlayListIndex++;
            readCurrent();
        }
    }

    @Override
    public void onInit(int status) {
        if (status == TextToSpeech.SUCCESS) {
            mTts.setOnUtteranceCompletedListener(this);
            loadPreferences(false);
            readTTS();
        } else {
            Toast.makeText(parent, R.string.tts_init_failed, Toast.LENGTH_LONG).show();
            mTts = null;
        }
    }

    private float f(int x, float m, float M) {
        float xnorm = (x - 3f) / 3;
        if (xnorm < 0)
            return xnorm * (1 - m) + 1;
        else if (xnorm > 0)
            return xnorm * (M - 1) + 1;
        else
            return 1;
    }

    public void loadPreferences(boolean allowrestart) {
        if (mTts == null)
            return;

        mTts.setPitch(f(LaParolaPreferences.ttsPitch, 0.5f, 2.0f));
        mTts.setSpeechRate(f(LaParolaPreferences.ttsSpeed, 0.5f, 2.0f));

        if (allowrestart) {
            synchronized (this) {
                mPlayListIndex--;
                readCurrent();
            }
        }
    }

    public void startTtsSettings() {
        Intent intent = new Intent();
        intent.setAction("com.android.settings.TTS_SETTINGS");
        //intent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK);
        parent.startActivity(intent);
    }

    private void startInstallLanguage() {
        Intent installIntent = new Intent();
        installIntent.setAction(TextToSpeech.Engine.ACTION_INSTALL_TTS_DATA);
        parent.startActivity(installIntent);
    }
}
