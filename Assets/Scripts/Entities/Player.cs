using UnityEngine;
using System.Collections;

public static class Player {
	public static GameObject O;

	// private
	static Sprite left;
	static Sprite middle;
	static Sprite right;
	static SpriteRenderer sr;



	static Player() {
		O = GameObject.Find("PlayerSprite");
		sr = O.GetComponent<SpriteRenderer>();
		left = Resources.Load<Sprite>("Pics/DCStoneSoup/Monster/boggart - left");
		right = Resources.Load<Sprite>("Pics/DCStoneSoup/Monster/boggart - right");
		middle = Resources.Load<Sprite>("Pics/DCStoneSoup/Monster/boggart");
	}

	public static Vector2 Pos2D {
		get {
			return new Vector2(Player.O.transform.position.x, 
			                   Player.O.transform.position.y);
		}
	}

	public static void Update() {
		if (O == null)
			return;
		
		float speed = 12.01f;

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

		// set correct animation frame
		sr.sprite = middle;
		if (
			O.rigidbody2D.velocity.x > 0.2f ||
			O.rigidbody2D.velocity.y > 0.2f ||
			O.rigidbody2D.velocity.x < -0.2f ||
			O.rigidbody2D.velocity.y < -0.2f
			) 
		{
			if /***/ (Time.time % 0.3f > 0.2f) {
				sr.sprite = left;
			}else if (Time.time % 0.3f > 0.1f) {
				sr.sprite = right;
			}
		}
	}
}
