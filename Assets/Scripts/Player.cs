using UnityEngine;
using System.Collections;

public static class Player {
	public static GameObject O;

	public static void Update() {
		if (O == null)
			return;

		float speed = 0.01f;

		if (Input.GetKey(KeyCode.A)) {
			O.rigidbody2D.AddForce(new Vector2(-speed, 0f));
		}
		if (Input.GetKey(KeyCode.D)) {
			O.rigidbody2D.AddForce(new Vector2(speed, 0f));
		}
		if (Input.GetKey(KeyCode.W)) {
			O.rigidbody2D.AddForce(new Vector2(0f, speed));
		}
		if (Input.GetKey(KeyCode.S)) {
			O.rigidbody2D.AddForce(new Vector2(0f, -speed));
		}
	}
}
