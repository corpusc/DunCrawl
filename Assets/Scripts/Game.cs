using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {
	void Start() {
		Player.O = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		Player.O.AddComponent<Rigidbody2D>();
		Player.O.renderer.material.mainTexture = Pics.Get("Icon", "cursor_green");
	}
	
	void Update() {
		Player.Update();
	}
}
