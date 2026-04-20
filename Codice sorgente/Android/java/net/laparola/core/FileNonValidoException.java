package net.laparola.core;

import java.io.Serial;

public class FileNonValidoException extends Exception {
	@Serial
    private static final long serialVersionUID = 1L;

	public FileNonValidoException(String s) {
        super(s);
    }
}
