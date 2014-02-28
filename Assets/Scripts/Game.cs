using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {
	void Start() {
		//Player.O = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		Player.O = GameObject.CreatePrimitive(PrimitiveType.Quad);
		Player.O.renderer.material.shader = Shader.Find("Unlit/Transparent");
		Player.O.renderer.material.mainTexture = Pics.Get("Icon", "cursor_green");
		Player.O.AddComponent<Rigidbody2D>();
	}
	
	void Update() {
		Player.Update();
	}
}
