package net.laparola.ui.android;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseExpandableListAdapter;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.TextView;

import net.laparola.R;

class DrawerAdapter extends BaseExpandableListAdapter implements View.OnClickListener {
    private LaParolaActivity laParolaActivity;
    private String[] mLabels;
    private String[] mLinks;
    private int[] mIcons;
    private LayoutInflater mInflater;

    public DrawerAdapter(LaParolaActivity laParolaActivity) {
        this.laParolaActivity = laParolaActivity;
        String[] items = laParolaActivity.getResources().getStringArray(R.array.drawer_items);
        mLabels = new String[items.length];
        mLinks = new String[items.length];
        mIcons = new int[items.length];
        for (int i = 0; i < items.length; i++) {
            String[] tmp = items[i].split("/", 3);
            mLabels[i] = tmp[0];
            mLinks[i] = tmp[2];

            if (tmp[1].length() == 0) {
                mIcons[i] = 0;
            } else {
                mIcons[i] = laParolaActivity.getResources().getIdentifier(tmp[1], "drawable", laParolaActivity.getPackageName());
            }
        }
    }

    public String getLink(long id) {
        return mLinks[(int)id];
    }


    private View getGenericView(int groupPosition, int childPosition, boolean isExpanded, View convertView, ViewGroup parent) {
        final View view;

        int position = (int)getChildId(groupPosition, childPosition);

        if (convertView == null) {
            if (mInflater == null) {
                mInflater = (LayoutInflater)parent.getContext().getSystemService(Context.LAYOUT_INFLATER_SERVICE);
            }
            view = mInflater.inflate(R.layout.menu_list_item, parent, false);
        } else {
            view = convertView;
        }

        TextView text = view.findViewById(R.id.textView);
        View spacer = view.findViewById(R.id.spacerView);
        ImageView image = view.findViewById(R.id.iconView);
        ImageButton expand = view.findViewById(R.id.expandView);

        if (mLabels[position].startsWith("-")) {
            text.setText(mLabels[position].substring(1));
            spacer.setVisibility(View.VISIBLE);
            view.setBackgroundResource(R.drawable.item_background_holo_light_dim);
        } else {
            text.setText(mLabels[position]);
            spacer.setVisibility(View.GONE);
            view.setBackgroundResource(R.drawable.item_background_holo_light);
        }

        if (mIcons[position] != 0) {
            image.setImageResource(mIcons[position]);
        } else {
            image.setImageDrawable(null);
        }

        if (getChildrenCount(groupPosition) > 0 && childPosition == -1) {
            expand.setVisibility(View.VISIBLE);

            if (isExpanded) {
                expand.setImageResource(R.drawable.expander_close_holo_light);
            } else {
                expand.setImageResource(R.drawable.expander_open_holo_light);
            }
        } else {
            expand.setVisibility(View.GONE);
        }

        expand.setTag(groupPosition);
        expand.setOnClickListener(this);

        expand.setFocusable(false);
        spacer.setFocusable(false);

        return view;
    }

    public void onClick (View view) {
        laParolaActivity.toggleDrawerGroupExpansion((Integer)view.getTag());
    }

    @Override public boolean hasStableIds() { return true; }
    @Override public boolean isChildSelectable(int group, int child) { return true; }

    @Override
    public long getChildId(int group, int child) {
        for (int i = 0; i < mLabels.length; i++) {
            if (!mLabels[i].startsWith("-")) {
                if (group == 0)
                    return i + child + 1;
                group--;
            }
        }
        return -1;
    }

    @Override public long getGroupId(int group) {
        return getChildId(group, -1);
    }

    @Override
    public int getGroupCount() {
        int res = 0;
        for (int i = 0; i < mLabels.length; i++) {
            if (!mLabels[i].startsWith("-")) {
                res++;
            }
        }
        return res;
    }

    @Override
    public int getChildrenCount(int group) {
        int start = 0;
        for (int i = 0; i < mLabels.length; i++) {
            if (!mLabels[i].startsWith("-")) {
                if (group == 0)
                    start = i;
                if (group == -1)
                    return i - start - 1;
                group--;
            }
        }
        return mLabels.length - start - 1;
    }

    @Override
    public Object getGroup(int group) {
        return mLabels[(int)getGroupId(group)];
    }

    @Override
    public Object getChild(int group, int child) {
        return mLabels[(int)getChildId(group, child)];
    }

    @Override
    public View getGroupView(int groupPosition, boolean isExpanded, View convertView,
                             ViewGroup parent) {
        return getGenericView(groupPosition, -1, isExpanded, convertView, parent);
    }

    @Override
    public View getChildView(int groupPosition, int childPosition, boolean isLastChild,
                             View convertView, ViewGroup parent) {
        return getGenericView(groupPosition, childPosition, false, convertView, parent);
    }
}
