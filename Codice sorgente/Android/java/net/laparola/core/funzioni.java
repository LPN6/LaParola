package net.laparola.core;

class funzioni {
	public static String rimuovi(String s, int i, int n) {
		return s.substring(0, i) + s.substring(i + n);
	}
	
	public static int unsignedByte(byte b) {
		return (b >= 0 ? b : b + 256);
	}

	public static boolean trimEndsWith(StringBuilder str, String end) {
		return endsWith(str, end, true);
	}

	public static boolean endsWith(CharSequence str, CharSequence end) {
		return endsWith(str, end, false);
	}

	public static boolean endsWith(CharSequence str, CharSequence end, boolean trim) {
		int sl = str.length();
		if (trim) {
			while (sl > 0 && str.charAt(sl - 1) <= ' ')
				sl--;
		}
		int el = end.length();
		if (sl < el)
			return false;
		return str.subSequence(sl - el, sl).toString().equals(end.toString());
	}
	
    public static boolean isLettera(char c)
    {
        return (Character.isLetter(c) || Character.getType(c) == java.lang.Character.NON_SPACING_MARK || (c >= '\u0485' && c <= '\u0486')); // gli ultimi caratteri sono usati nella traslitterazione dell'ebraico
    }
}
