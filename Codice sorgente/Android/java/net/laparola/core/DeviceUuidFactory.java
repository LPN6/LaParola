package net.laparola.core;

import java.util.UUID;

import android.content.Context;
import android.provider.Settings.Secure;

public class DeviceUuidFactory {
	protected volatile static UUID uuid;

	public DeviceUuidFactory(Context context) {
		// http://stackoverflow.com/questions/2785485/is-there-a-unique-android-device-id
		// Use the Android ID unless it's broken, in which case
		// fallback on serial number,
		// unless it's not available, then fallback on a fixed string

		String id = Secure.getString(context.getContentResolver(), Secure.ANDROID_ID);

		if (id == null || id.length() < 15 || "9774d56d682e549c".equals(id)) {
			if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.GINGERBREAD) {
				id = android.os.Build.SERIAL;
			}
		}

		if (id == null || id.equals("") || "9774d56d682e549c".equals(id))
			id = "lpnj";

		uuid = UUID.nameUUIDFromBytes(id.getBytes());
	}

	public UUID getDeviceUuid() {
		return uuid;
	}
}
