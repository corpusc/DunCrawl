using UnityEngine;
using System.Collections;

public class Controls : MonoBehaviour {
	public float force;
	private Rigidbody2D sphere;



	void Awake() {
		sphere = gameObject.GetComponent<Rigidbody2D> ();
	}
	
	void Update() {
		if (transform.position.y < 4f && Input.GetKeyDown (KeyCode.Space)) {
			sphere.isKinematic = true;
			sphere.isKinematic = false;
			
			// Nudges the sphere upward (Y-axis only) by giving it a force impulse
			sphere.AddForce(new Vector2(0f, force));
		}
	}
}
