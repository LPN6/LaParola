package net.laparola.core;

import java.io.Serial;

public class TestoNonEsisteException extends Exception {
	@Serial
    private static final long serialVersionUID = 1L;

	public TestoNonEsisteException(String s) {
		super(s, null, false, false);
	}
}