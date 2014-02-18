using UnityEngine;
using System.Collections;








// CURRENTLY THIS FILE IS JUST AN UNUSED SIMPLE EXAMPLE OF SIMPLE PHYSICS BEING APPLIED TO A DYNAMIC GAME OBJECT









public class Controls : MonoBehaviour {
	public float force;
	private Rigidbody2D sphere;



	void Awake() {
		sphere = gameObject.GetComponent<Rigidbody2D> ();
	}
	
	void Update() {
		if (transform.position.y < 4f && Input.GetKeyDown (KeyCode.Space)) {
			// reset dynamics of sphere
			sphere.isKinematic = true;
			sphere.isKinematic = false;
			
			// Nudges the sphere upward (Y-axis only) by giving it a force impulse
			sphere.AddForce(new Vector2(0f, force));
		}
	}
}
