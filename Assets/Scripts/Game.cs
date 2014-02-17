using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// for saving/loading via BinaryFormatter
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public class Game : MonoBehaviour {
	// private
	Hud hud;
	Vector3 mouPos; // mouse position on screen
	Vector3 mouWorldPos; // mouse position in the world
	GameObject cursorCell; // what the selection pointer is hovering-over/pointing-at
	GameObject entireFloor; // the single black background that covers the whole possible floor space.
		// this way we don't have a bunch of objects in the scene just to represent every single blank cell.
		// instead, we only store and do calculations on objects that the player can see and interact with.

	// in-world ui 
	// quad positioning and spans
	const int cellsAcross = 16;
	Vector3 horiBarCenter;
	Vector3 vertBarCenter;
	Vector3 pastMaxX;
	Vector3 pastMaxY;
	Vector3 stretchX;
	Vector3 stretchY;

	// textures
	Texture cursOk; // for valid cursor
	Texture cursBad; // for invalid cursor

	List<PicData>[,] cells = new List<PicData>[cellsAcross, cellsAcross];



	void Start() {
		hud = this.GetComponent<Hud>();


		cursOk = Pics.GetFirstWith("cursor");
		cursBad = Pics.GetFirstWith("cursor_red");
		var bumpLeft = new Vector3(-1, 0, 0);
		var bumpDown = new Vector3(0, -1, 0);
		var farBack = new Vector3(0, 0, 30);

		setQuadPositioningAndSpans();
		Camera.main.transform.position = new Vector3(cellsAcross/2, cellsAcross/2, 0); // put in the middle of floor

		// make the boundaries of the floor
		// top edge
		var v = GameObject.CreatePrimitive(PrimitiveType.Quad);
		v.transform.position = farBack + horiBarCenter + pastMaxY;
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
		v.transform.position = farBack + vertBarCenter + pastMaxX;
		v.transform.localScale = stretchY;
		v.renderer.material.shader = Shader.Find("Unlit/Transparent");
		v.renderer.material.mainTexture = Pics.GetFirstWith("cursor_green");
	

		
		// the backdrop of valid floor space
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

		if (mouPos.x > Screen.width - mar &&
		    Camera.main.transform.position.x < cellsAcross)
			Camera.main.transform.position += new Vector3(speed, 0, 0);
		if (mouPos.x < mar &&
		    Camera.main.transform.position.x > 0)
			Camera.main.transform.position -= new Vector3(speed, 0, 0);
		if (mouPos.y > Screen.height - mar &&
		    Camera.main.transform.position.y < cellsAcross)
			Camera.main.transform.position += new Vector3(0, speed, 0);
		if (mouPos.y < mar &&
		    Camera.main.transform.position.y > 0)
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
			if (Input.GetMouseButtonDown(1)) {
				destroyQuad(mouWorldPos);
			}

			if (!(x == prevX && y == prevY)) {
				// we just moved to a new cell
				if (Input.GetMouseButton(0))
					createQuad(mouWorldPos);
				if (Input.GetMouseButton(1))
					destroyQuad(mouWorldPos);
			}

			prevX = x;
			prevY = y;

			cursorCell.renderer.material.mainTexture = cursOk;
		}else{
			cursorCell.renderer.material.mainTexture = cursBad;
		}

		cursorCell.transform.position = mouWorldPos;
	}
	
	void destroyQuad(Vector3 pos) {
		var cl = cells[(int)pos.y, (int)pos.x]; // cell list
		
		if (cl == null || cl.Count < 1)
			return;

		Destroy(cl[cl.Count-1].GameObject);
		cl.RemoveAt(cl.Count-1);
		Debug.Log("destroyQuad()");
	}
		
	void createQuad(Vector3 pos) {
		if (hud.Mode != HudMode.EditMap)
			return;

		bool adding = false;
		var cl = cells[(int)pos.y, (int)pos.x]; // cell list
		//Debug.Log("pos.x: " + (int)pos.x + "  pos.y: " + (int)pos.y);

		if (cl == null) {
			cl = cells[(int)pos.y, (int)pos.x] = new List<PicData>();
			Debug.Log("cell was null, made a new list");
		}
		
		if (cl.Count < 1) {
			adding = true;
			Debug.Log("cell had empty list");
		}

		// replace if brush is the same type as something else in the list
		for (int i = cl.Count-1; i >= 0; i--) {
			Debug.Log("at least one PicData here");

			if (cl[i].Type == hud.Brush.Type) {
				Destroy(cl[i].GameObject);
				cl.RemoveAt(i);
				adding = true;
			}
		}

		if (adding) {
			var pd = new PicData();
			var o = GameObject.CreatePrimitive(PrimitiveType.Quad);
			o.transform.position = pos;
			o.renderer.material.shader = Shader.Find("Unlit/Transparent");
			o.renderer.material.mainTexture = 
				pd.Pic = hud.Brush.Pic;
			pd.Type = hud.Brush.Type;
			pd.GameObject = o;
			cells[(int)pos.y, (int)pos.x].Add(pd);
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

	void setQuadPositioningAndSpans() {
		horiBarCenter = new Vector3(cellsAcross*0.5f-0.5f, 0, 0);
		vertBarCenter = new Vector3(0, cellsAcross*0.5f-0.5f, 0);
		pastMaxX = new Vector3(cellsAcross, 0, 0);
		pastMaxY = new Vector3(0, cellsAcross, 0);
		stretchX = new Vector3(cellsAcross, 1, 1);
		stretchY = new Vector3(1, cellsAcross, 1);
	}
}
