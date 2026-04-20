package net.laparola.ui.android.actionbar.bibleversionspinner;

import java.util.ArrayList;
import java.util.Comparator;
import java.util.EnumSet;
import java.util.List;

import net.laparola.R;
import net.laparola.core.ComponenteInformazioni;
import net.laparola.core.Testi.TestoTipi;
import net.laparola.ui.LaParolaBrowser;

import android.content.Context;
import android.database.DataSetObserver;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ListAdapter;
import android.widget.SpinnerAdapter;
import android.widget.TextView;

public class VersionAdapter implements SpinnerAdapter, ListAdapter {
    public final String NO_VERSION_INSTALLED;

    private final List<ComponenteInformazioni> mTestiFiltrati;
    private TestoTipi mTipo = TestoTipi.NESSUNO;

    private LayoutInflater mInflater;
    private List<ComponenteInformazioni> mTestiInstallati;
    private final Comparator<ComponenteInformazioni> mComparator;

    private final List<DataSetObserver> mObservers = new ArrayList<>();

    public VersionAdapter(Context c) {
        NO_VERSION_INSTALLED = c.getString(R.string.no_version_installed);
        mTestiFiltrati = new ArrayList<>();

        mComparator = Comparator.comparing(ComponenteInformazioni::getComponente);
    }

    public void refresh() {
        mTestiInstallati = LaParolaBrowser.getTestiInstallati();
        if (mTestiInstallati==null)
            return;

        mTestiInstallati.sort(mComparator);
        mTestiFiltrati.clear();
        for (int i = 0; i < mTestiInstallati.size(); i++) {
            ComponenteInformazioni c = mTestiInstallati.get(i);
            if (mTipo == TestoTipi.NESSUNO || c.getTipo().contains(mTipo)) {
                mTestiFiltrati.add(c);
            }
        }
    }

    public TestoTipi getTipo() {
        return mTipo;
    }

    public void setTipo(TestoTipi value) {
        if (mTipo == value)
            return;

        mTipo = value;
        refresh();
    }

    public TestoTipi getVersionType(String versione) {
        if (mTestiInstallati != null) {
            for (int i = 0; i < mTestiInstallati.size(); i++) {
                if (versione.equals(mTestiInstallati.get(i).getComponente())) {
                    EnumSet<TestoTipi> t = mTestiInstallati.get(i).getTipo();
                    if (t.contains(TestoTipi.BIBBIA)) {
                        return TestoTipi.BIBBIA;
                    } else if (t.contains(TestoTipi.COMMENTARIO)) {
                        return TestoTipi.COMMENTARIO;
                    }

                }
            }
        }
        return TestoTipi.NESSUNO;
    }

    public int getPosition(String versione) {
        int selection = -1;

        for (int i = 0; i < mTestiFiltrati.size(); i++) {
            if (versione.equals(mTestiFiltrati.get(i).getComponente())) {
                selection = i;
                break;
            }
        }

        if (mTestiFiltrati.isEmpty()) {
            selection = 0;
        }

        return selection;
    }

    public int getCount() {
        return Math.max(1, mTestiFiltrati.size());
    }

    public Object getItem(int position) {
        if (mTestiFiltrati.isEmpty()) {
            return NO_VERSION_INSTALLED;
        }

        return mTestiFiltrati.get(position).getComponente();
    }

    public long getItemId(int position) {
        return position + mTipo.ordinal() * 1000;
    }

    public int getItemViewType(int position) {
        return 0;
    }

    public int getViewTypeCount() {
        return 1;
    }

    public boolean hasStableIds() {
        return true;
    }

    public boolean isEmpty() {
        return mTestiFiltrati.isEmpty();
    }

    public void registerDataSetObserver(DataSetObserver observer) {
        if (!mObservers.contains(observer))
            mObservers.add(observer);
    }

    public void unregisterDataSetObserver(DataSetObserver observer) {
        while (mObservers.contains(observer))
            mObservers.remove(observer);
    }

    protected View createViewFromResource(int position, View convertView, ViewGroup parent, int resource) {
        View view;
        TextView text;

        if (convertView == null) {
            if (mInflater == null) {
                mInflater = (LayoutInflater) parent.getContext().getSystemService(Context.LAYOUT_INFLATER_SERVICE);
            }
            view = mInflater.inflate(resource, parent, false);
        } else {
            view = convertView;
        }

        try {
            int mFieldId = 0;

            if (mFieldId == 0) {
                //  If no custom field is assigned, assume the whole resource is a TextView
                text = (TextView) view;
            } else {
                //  Otherwise, find the TextView field within the layout
                text = view.findViewById(mFieldId);
            }
        } catch (ClassCastException e) {
            throw new IllegalStateException("ArrayAdapter requires the resource ID to be a TextView", e);
        }

        Object item = getItem(position);
        if (item instanceof CharSequence) {
            text.setText((CharSequence) item);
        } else {
            text.setText(item.toString());
        }

        return view;
    }

    public View getView(int position, View convertView, ViewGroup parent) {
        //return createViewFromResource(position, convertView, parent, android.R.layout.simple_spinner_item);
        return getDropDownView(position, convertView, parent);
    }

    public View getDropDownView(int position, View convertView, ViewGroup parent) {
        return createViewFromResource(position, convertView, parent, android.R.layout.simple_spinner_dropdown_item
        );
    }

    @Override
    public boolean areAllItemsEnabled() {
        return true;
    }

    @Override
    public boolean isEnabled(int position) {
        return true;
    }

    public void sendChanged() {
        for (int i = 0; i < mObservers.size(); i++) {
            mObservers.get(i).onChanged();
        }
    }
}
