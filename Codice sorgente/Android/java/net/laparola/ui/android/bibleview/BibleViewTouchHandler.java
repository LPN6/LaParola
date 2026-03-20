package net.laparola.ui.android.bibleview;

import android.content.Context;
import android.view.GestureDetector;
import android.view.GestureDetector.SimpleOnGestureListener;
import android.view.MotionEvent;

import net.laparola.ui.android.LaParolaPreferences;
import net.laparola.ui.android.bibleview.ScaleGestureDetector.OnScaleGestureListener;

/* package */class BibleViewTouchHandler implements OnScaleGestureListener {
	private static final int SWIPE_MIN_DISTANCE = 120;
	private static final int SWIPE_MAX_OFF_PATH = 250;
	private static final int SWIPE_THRESHOLD_VELOCITY = 200;

	private BibleView mBibleView;
	private Context mContext;
	private boolean mZooming;
	private MotionEvent mLastMotionEventWithOnePointer;

	private ScaleGestureDetector mScaleGestureDetector;
	private GestureDetector mSwipeGestureDetector;

	public BibleViewTouchHandler(BibleView bibleView) {
		mBibleView = bibleView;
		mContext = mBibleView.getContext();

		mScaleGestureDetector = new ScaleGestureDetector(mContext, this);
        updatePreferences();

        mSwipeGestureDetector = new GestureDetector(mContext, new SwipeGestureDetector());
	}

    public void updatePreferences() {
        mScaleGestureDetector.setQuickScaleEnabled(LaParolaPreferences.oneHandZoom);
    }

    public boolean onTouchEvent(MotionEvent ev) {
		boolean wasZooming = mZooming;

		if (ev.getPointerCount() == 1) {
			mLastMotionEventWithOnePointer = ev;
		}

		mScaleGestureDetector.onTouchEvent(ev);
		if (!wasZooming && mZooming && mLastMotionEventWithOnePointer != null) {
			mBibleView.cancelTouch();
		}

		if (!mZooming) {
			mSwipeGestureDetector.onTouchEvent(ev);
		}

		return wasZooming || mZooming;
	}

	@Override
	public boolean onScale(ScaleGestureDetector detector) {
		float scaleFactor = detector.getScaleFactor();
		int prevTextZoom = mBibleView.getTextZoom();
		int nextTextZoom = Math.round(prevTextZoom * scaleFactor);

		nextTextZoom = Math.round(nextTextZoom / 10f) * 10;

		if (prevTextZoom != nextTextZoom) {
			mBibleView.setTextZoom(nextTextZoom);

			return true;
		}

		return false;
	}

	@Override
	public boolean onScaleBegin(ScaleGestureDetector detector) {
		mZooming = true;
		return true;
	}

	@Override
	public void onScaleEnd(ScaleGestureDetector detector) {
		mZooming = false;
	}

	public void onRightToLeftSwipe() {
		if (LaParolaPreferences.swipeChapters) {
			mBibleView.goToNextUrl();
		}
	}

	private void onLeftToRightSwipe() {
		if (LaParolaPreferences.swipeChapters) {
			mBibleView.goToPreviousUrl();
		}
	}

	private class SwipeGestureDetector extends SimpleOnGestureListener {
		@Override
		public boolean onFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY) {
			try {
				if (Math.abs(e1.getY() - e2.getY()) > SWIPE_MAX_OFF_PATH)
					return false;

				if (e1.getX() - e2.getX() > SWIPE_MIN_DISTANCE && Math.abs(velocityX) > SWIPE_THRESHOLD_VELOCITY) {
					onRightToLeftSwipe();
				} else if (e2.getX() - e1.getX() > SWIPE_MIN_DISTANCE && Math.abs(velocityX) > SWIPE_THRESHOLD_VELOCITY) {
					onLeftToRightSwipe();
				}
			} catch (Exception e) {
				//
			}
			return false;
		}
	}

	public boolean isZooming() {
		return mZooming;
	}
}
