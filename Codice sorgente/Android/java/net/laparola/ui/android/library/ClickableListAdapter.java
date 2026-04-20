package net.laparola.ui.android.library;

import java.util.ArrayList;
import java.util.List;

import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.BaseAdapter;

public abstract class ClickableListAdapter<T> extends BaseAdapter {
	private final LayoutInflater mInflater;
	protected List<T> mDataObjects; // our generic object list
	private final int mViewId;

	/**
	 * This is the holder will provide fast access to arbitrary objects and views. Use a subclass to adapt it for your personal needs.
	 */
	public static class ViewHolder {
		// back reference to our list object
		public Object data;
	}

	/**
	 * The click listener base class.
	 */
	public static abstract class OnClickListener implements View.OnClickListener {

		private final ViewHolder mViewHolder;

		/**
		 * @param holder
		 *            The holder of the clickable item
		 */
		public OnClickListener(ViewHolder holder) {
			mViewHolder = holder;
		}

		// delegates the click event
		public void onClick(View v) {
			onClick(v, mViewHolder);
		}

		/**
		 * Implement your click behavior here
		 * 
		 * @param v
		 *            The clicked view.
		 */
		public abstract void onClick(View v, ViewHolder viewHolder);
	}

	/**
	 * The long click listener base class.
	 */
	public static abstract class OnLongClickListener implements View.OnLongClickListener {
		private final ViewHolder mViewHolder;

		/**
		 * @param holder
		 *            The holder of the clickable item
		 */
		public OnLongClickListener(ViewHolder holder) {
			mViewHolder = holder;
		}

		// delegates the click event
		public boolean onLongClick(View v) {
			onLongClick(v, mViewHolder);
			return true;
		}

		/**
		 * Implement your click behavior here
		 * 
		 * @param v
		 *            The clicked view.
		 */
		public abstract void onLongClick(View v, ViewHolder viewHolder);

	}

	/**
	 * @param context
	 *            The current context
	 * @param viewid
	 *            The resource id of your list view item
	 * @param objects
	 *            The object list, or null, if you like to indicate an empty list
	 */
	public ClickableListAdapter(Context context, int viewid, List<T> objects) {
		// Cache the LayoutInflate to avoid asking for a new one each time.
		mInflater = LayoutInflater.from(context);
		mDataObjects = objects;
		mViewId = viewid;

		if (objects == null) {
			mDataObjects = new ArrayList<>();
		}
	}

	/**
	 * The number of objects in the list.
	 */
	public int getCount() {
		return mDataObjects.size();
	}

	/**
	 * @return We simply return the object at position of our object list Note, the holder object uses a back reference to its related data object. So, the user usually should use
	 *         {@link ViewHolder#data} for faster access.
	 */
	public Object getItem(int position) {
		return mDataObjects.get(position);
	}

	/**
	 * We use the array index as a unique id. That is, position equals id.
	 * 
	 * @return The id of the object
	 */
	public long getItemId(int position) {
		return position;
	}

	/**
	 * Make a view to hold each row. This method is instantiated for each list object. Using the Holder Pattern, avoids the unnecessary findViewById()-calls.
	 */
	public View getView(int position, View view, ViewGroup parent) {
		// A ViewHolder keeps references to children views to avoid uneccessary
		// calls
		// to findViewById() on each row.
		ViewHolder holder;

		// When view is not null, we can reuse it directly, there is no need
		// to reinflate it. We only inflate a new View when the view supplied
		// by ListView is null.
		if (view == null) {

			view = mInflater.inflate(mViewId, null);
			// call the user's implementation
			holder = createHolder(view);
			// we set the holder as tag
			view.setTag(holder);

		} else {
			// get holder back...much faster than inflate
			holder = (ViewHolder) view.getTag();
		}

		// we must update the object's reference
		holder.data = getItem(position);
		// call the user's implementation
		bindHolder(holder);

		return view;
	}

	/**
	 * Creates your custom holder, that carries reference for e.g. ImageView and/or TextView. If necessary connect your clickable View object with the PrivateOnClickListener, or
	 * PrivateOnLongClickListener
	 * 
	 * @param v
	 *            The view for the new holder object
	 */
	protected abstract ViewHolder createHolder(View v);

	/**
	 * Binds the data from user's object to the holder
	 * 
	 * @param h
	 *            The holder that shall represent the data object.
	 */
	protected abstract void bindHolder(ViewHolder h);
}
