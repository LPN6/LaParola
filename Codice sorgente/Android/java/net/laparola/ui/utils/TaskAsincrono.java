package net.laparola.ui.utils;

public abstract class TaskAsincrono<T> {
	protected abstract T lavoraInBackground ();
	protected abstract void onAnnullato ();
	protected abstract void onFinito (T risultato);
	
	private boolean mAnnullato = false;
	private boolean mFinito = false;
	
	public void annulla () {
		mAnnullato = true;
	}
    
	public boolean annullato () {
		return mAnnullato;
	}
    
	public boolean finito () {
		return mFinito;
	}
	
	public void esegui () {
        mAnnullato = false;
        mFinito = false;
		Thread t = new Thread(() -> {
            T res = lavoraInBackground();
			mFinito = true;
            if (!mAnnullato) {
                onFinito(res);
            } else {
                onAnnullato();
            }
        });
		t.start();
	}
}
