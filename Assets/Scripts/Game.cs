using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {
	// private
	Hud hud;
	Vector3 mouPos; // mouse position on screen
	Vector3 mouWorldPos; // mouse position in the world
	GameObject cursorCell; // what the selection pointer is hovering-over/pointing-at
	GameObject entireFloor; // the single black background that covers the whole possible floor space.
		// this way we don't have a bunch of objects in the scene just to represent every single blank cell.
		// instead, we only store and do calculations on objects that the player can see and interact with.

	// quad positioning and spans
	int cellsAcross = 10;
	Vector3 horiBarCenter;
	Vector3 vertBarCenter;
	Vector3 pastX;
	Vector3 pastY;
	Vector3 stretchX;
	Vector3 stretchY;

	// textures
	Texture cursOk; // for valid cursor
	Texture cursBad; // for invalid cursor



	void Start() {
		hud = this.GetComponent<Hud>();

		cursOk = Pics.GetFirstWith("cursor");
		cursBad = Pics.GetFirstWith("cursor_red");
		var bumpLeft = new Vector3(-1, 0, 0);
		var bumpDown = new Vector3(0, -1, 0);
		var farBack = new Vector3(0, 0, 30);

		setQuadPositioningAndSpans(cellsAcross);
		Camera.main.transform.position = new Vector3(cellsAcross/2, cellsAcross/2, 0); // put in the middle of floor

		// make the boundaries of the floor
		// top edge
		var v = GameObject.CreatePrimitive(PrimitiveType.Quad);
		v.transform.position = farBack + horiBarCenter + pastY;
		v.transform.localScale = stretchX;
		//Debug.Log ("top edge - transform.position: " + transform.position);
		v.renderer.material.shader = Shader.Find("Unlit/Transparent");
		v.renderer.material.mainTexture = Pics.GetFirstWith("cursor_green");
		
		// bottom edge
		v = GameObject.CreatePrimitive(PrimitiveType.Quad);
		v.transform.position = farBack + horiBarCenter + bumpDown;
		v.transform.localScale = stretchX;
		v.renderer.material.shader = Shader.Find("Unlit/Transparent");
		v.renderer.material.mainTexture = Pics.GetFirstWith("cursor_green");
		//Debug.Log ("bottom edge - transform.position: " + transform.position);

		// left edge
		v = GameObject.CreatePrimitive(PrimitiveType.Quad);
		v.transform.position = farBack + vertBarCenter + bumpLeft;
		v.transform.localScale = stretchY;
		v.renderer.material.shader = Shader.Find("Unlit/Transparent");
		v.renderer.material.mainTexture = Pics.GetFirstWith("cursor_green");
		
		// right edge
		v = GameObject.CreatePrimitive(PrimitiveType.Quad);
		v.transform.position = farBack + vertBarCenter + pastX;
		v.transform.localScale = stretchY;
		v.renderer.material.shader = Shader.Find("Unlit/Transparent");
		v.renderer.material.mainTexture = Pics.GetFirstWith("cursor_green");
	

		
		// floor
		entireFloor = GameObject.CreatePrimitive(PrimitiveType.Quad);
		entireFloor.transform.position = farBack + horiBarCenter + vertBarCenter;
		entireFloor.transform.localScale = Vector3.Scale(stretchX, stretchY);
		entireFloor.renderer.material.shader = Shader.Find("Unlit/Transparent");
		entireFloor.renderer.material.mainTexture = Pics.Black;
		
		// cursor
		cursorCell = GameObject.CreatePrimitive(PrimitiveType.Quad);
		cursorCell.renderer.material.shader = Shader.Find("Unlit/Transparent");
		cursorCell.renderer.material.mainTexture = Pics.GetFirstWith("cursor");
	}
	
	void FixedUpdate() {
		// all we do in this method is pan around the dungeon floor (ONLY whewn editing map)
		if (hud.Mode != HudMode.EditMap)
			return;

		float speed = 0.06f;
		int mar = 16; // margin from edge, where we pan camera

		if (mouPos.x > Screen.width - mar)
			Camera.main.transform.position += new Vector3(speed, 0, 0);
		if (mouPos.x < mar)
			Camera.main.transform.position -= new Vector3(speed, 0, 0);
		if (mouPos.y > Screen.height - mar)
			Camera.main.transform.position += new Vector3(0, speed, 0);
		if (mouPos.y < mar)
			Camera.main.transform.position -= new Vector3(0, speed, 0);
	}

	int prevX;
	int prevY;
	void Update() { // FIXME: hohoho boy.  first we have screen mouse pos, 
		// ...................then world position, then quantized world position....all with the same var
		mouPos = Input.mousePosition;
		mouPos.z = 10.0f;
		mouWorldPos = Camera.main.ScreenToWorldPoint(mouPos);

		if (scaledUnitSquareContains(mouWorldPos, entireFloor)) {
			int x = (int)((mouWorldPos.x + 0.5f) / 1f);
			int y = (int)((mouWorldPos.y + 0.5f) / 1f);
			mouWorldPos.x = x;
			mouWorldPos.y = y;

			// if just now pressed
			if (Input.GetMouseButtonDown(0)) {
				createQuad(mouWorldPos);
			}
			
			if (!(x == prevX && y == prevY)) {
				// we just moved to a new cell
				if (Input.GetMouseButton(0)) {
					createQuad(mouWorldPos);
				}
			}

			prevX = x;
			prevY = y;

			cursorCell.renderer.material.mainTexture = cursOk;
		}else{
			cursorCell.renderer.material.mainTexture = cursBad;
		}

		cursorCell.transform.position = mouWorldPos;
	}
	
	void createQuad(Vector3 pos) {
		Debug.Log("createQuad()");
		var o = GameObject.CreatePrimitive(PrimitiveType.Quad);
		o.transform.position = mouWorldPos;
		o.renderer.material.shader = Shader.Find("Unlit/Transparent");
		o.renderer.material.mainTexture = hud.Brush;
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
