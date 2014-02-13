using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {
	// private
	GameObject cursorCell; // what the selection pointer is hovering-over/pointing-at
	GameObject entireFloor; // the single black background that covers the whole possible floor space.
		// this way we don't have a bunch of objects in the scene just to represent every single blank cell.
		// instead, we only store and do calculations on objects that the player can see and interact with.

	// quad positioning and spans
	int cellsAcross = 3;
	Vector3 horiBarCenter;
	Vector3 vertBarCenter;
	Vector3 pastX;
	Vector3 pastY;
	Vector3 stretchX;
	Vector3 stretchY;



	void Start() {
		Vector3 bumpLeft = new Vector3(-1, 0, 0);
		Vector3 bumpDown = new Vector3(0, -1, 0);
		setQuadPositioningAndSpans(3);

		// make the boundaries of the floor
		// top edge
		var v = GameObject.CreatePrimitive(PrimitiveType.Quad);
		v.transform.position = horiBarCenter + pastY;
		v.transform.localScale = stretchX;
		//Debug.Log ("top edge - transform.position: " + transform.position);
		v.renderer.material.shader = Shader.Find("Unlit/Transparent");
		v.renderer.material.mainTexture = Pics.GetFirstThatContains("cursor_green");
		
		// bottom edge
		v = GameObject.CreatePrimitive(PrimitiveType.Quad);
		v.transform.position = horiBarCenter + bumpDown;
		v.transform.localScale = stretchX;
		v.renderer.material.shader = Shader.Find("Unlit/Transparent");
		v.renderer.material.mainTexture = Pics.GetFirstThatContains("cursor_green");
		//Debug.Log ("bottom edge - transform.position: " + transform.position);

		// left edge
		v = GameObject.CreatePrimitive(PrimitiveType.Quad);
		v.transform.position = vertBarCenter + bumpLeft;
		v.transform.localScale = stretchY;
		v.renderer.material.shader = Shader.Find("Unlit/Transparent");
		v.renderer.material.mainTexture = Pics.GetFirstThatContains("cursor_green");
		
		// right edge
		v = GameObject.CreatePrimitive(PrimitiveType.Quad);
		v.transform.position = vertBarCenter + pastX;
		v.transform.localScale = stretchY;
		v.renderer.material.shader = Shader.Find("Unlit/Transparent");
		v.renderer.material.mainTexture = Pics.GetFirstThatContains("cursor_green");
	

		
		// floor
		entireFloor = GameObject.CreatePrimitive(PrimitiveType.Quad);
		entireFloor.transform.position = horiBarCenter + vertBarCenter;
		entireFloor.transform.localScale = Vector3.Scale(stretchX, stretchY);
		entireFloor.renderer.material.shader = Shader.Find("Unlit/Transparent");
		entireFloor.renderer.material.mainTexture = Pics.Black;
		
		// cursor
		cursorCell = GameObject.CreatePrimitive(PrimitiveType.Quad);
		cursorCell.renderer.material.shader = Shader.Find("Unlit/Transparent");
		cursorCell.renderer.material.mainTexture = Pics.GetFirstThatContains("cursor");
	}
	
	Vector3 v;
	void Update() {
		v = Input.mousePosition;
		v.z = 10.0f;
		v = Camera.main.ScreenToWorldPoint(v);
		cursorCell.transform.position = v;
		
		if (scaledUnitSquareContains(v, entireFloor)) {
			cursorCell.renderer.material.mainTexture = Pics.GetFirstThatContains("cursor");
		}else{
			cursorCell.renderer.material.mainTexture = Pics.GetFirstThatContains("cursor_red");
		}
	}
	
	bool scaledUnitSquareContains(Vector3 v, GameObject o) {
		if (
			v.x > o.transform.position.x - o.transform.localScale.x/2 &&
			v.x < o.transform.position.x + o.transform.localScale.x/2 &&
			v.y > o.transform.position.x - o.transform.localScale.y/2 &&
			v.y < o.transform.position.x + o.transform.localScale.y/2) 
		{
			return true;
		}else{
			return false;
		}
	}

	void setQuadPositioningAndSpans(int i) {
		cellsAcross = i;
		horiBarCenter = new Vector3(cellsAcross*0.5f-0.5f, 0, 0);
		vertBarCenter = new Vector3(0, cellsAcross*0.5f-0.5f, 0);
		pastX = new Vector3(cellsAcross, 0, 0);
		pastY = new Vector3(0, cellsAcross, 0);
		stretchX = new Vector3(cellsAcross, 1, 1);
		stretchY = new Vector3(1, cellsAcross, 1);
	}
}
