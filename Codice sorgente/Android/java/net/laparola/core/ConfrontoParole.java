package net.laparola.core;

import java.text.CollationKey;
import java.text.Collator;
import java.util.Locale;

public class ConfrontoParole extends Collator {
	
	Collator ital = Collator.getInstance(Locale.ITALY);
	
	public ConfrontoParole() {
		ital.setStrength(Collator.SECONDARY);
	}
	
	@Override
	public int compare(String string1, String string2) {
		String s1 = string1.replace("-", "");
		String s2 = string2.replace("-", "");
		int r = ital.compare(s1, s2);
		if (r==0) {
			int p1 = string1.indexOf('-');
			int p2 = string2.indexOf('-');
			if (p2>=0 && p1<0) r=-1;
			if (p1>=0 && p2>0) r=1;
			if (p1>=0 && p2>=0 && p1!=p2) r=(p1<p2?1:-1);
		}
		return r;
	}

	@Override
	public CollationKey getCollationKey(String string) {
		return ital.getCollationKey(string);
	}

	@Override
	public int hashCode() {
		return ital.hashCode();
	}
}

/*
Locale[] locales = Collator.getAvailableLocales();
RuleBasedCollator currentCollator = (RuleBasedCollator)Collator.getInstance();
String r1 = currentCollator.getRules();
Locale l = java.util.Locale.getDefault();
RuleBasedCollator it_Collator = (RuleBasedCollator)Collator.getInstance(Locale.ITALY);
String r2 = it_Collator.getRules();
try {
	confrontoParole = new RuleBasedCollator("a < b");//( "-<a");
} catch (ParseException e) {
	// non dovrebbe succedere, ma bisogna mettere una catch
	confrontoParole = (RuleBasedCollator)Collator.getInstance(Locale.ITALY);
}
confrontoParole.setStrength(Collator.SECONDARY);
*/