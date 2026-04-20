package net.laparola.core;

import java.io.Serial;

public class RicercaErroreSintassiException extends Exception {
	@Serial
    private static final long serialVersionUID = 1L;

	public RicercaErroreSintassiException(String s) {
      super(s);
  }
}
