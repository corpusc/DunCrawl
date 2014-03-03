using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {
	void Start() {
		//Player.O = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		//Player.O.name = "Player";
		//Player.O.renderer.material.shader = Shader.Find("Unlit/Transparent");
		//Player.O.renderer.material.mainTexture = Pics.Get("Monster", "deep_elf_mage");
		//Player.O.collider.enabled = false;
		//Destroy(Player.O.collider);
		//Player.O.AddComponent<Rigidbody2D>();
		Physics2D.gravity = new Vector3(0, 0, 0);
	}
	
	void Update() {
		Player.Update();
	}
}
