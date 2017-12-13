using UnityEngine;

public class Utils {
	public static Vector3 invertOrZero(Vector3 v) {
		var result = new Vector3();

		result.x = (v.x == 0) ? 0 : 1 / v.x;
		result.y = (v.y == 0) ? 0 : 1 / v.y;
		result.z = (v.z == 0) ? 0 : 1 / v.z;

		return result;
	}

	public static Vector3 ScreenToWorld(Vector3 screenPos) {
		var result = ScreenToWorld(screenPos, Camera.main.transform.position.magnitude);
		return result;
	}

	public static Vector3 ScreenToWorld(Vector3 screenPos, float distance) {
		screenPos.z = Camera.main.transform.position.magnitude;
		
		var result = Camera.main.ScreenToWorldPoint(screenPos);
		return result;
	}
}
