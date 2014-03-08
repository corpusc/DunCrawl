using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// for saving/loading via BinaryFormatter
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public class MapEditor : MonoBehaviour {
	// private
	Hud hud;
	Game game;
	char splitSeperator = '-';
	GameObject cursorCell; // what the selection pointer is hovering-over/pointing-at

	// in-world chrome 
	// quad positioning and spans
	Vector3 pastMaxX;
	Vector3 pastMaxY;
	Vector3 stretchX;
	Vector3 stretchY;

	// textures
	Texture cursOk; // for valid cursor
	Texture cursBad; // for invalid cursor



	void Start() {
		hud = this.GetComponent<Hud>();
		game = this.GetComponent<Game>();


		cursOk = Pics.GetFirstWith("cursor");
		cursBad = Pics.GetFirstWith("cursor_red");
		var bumpLeft = new Vector3(-1, 0, 0);
		var bumpDown = new Vector3(0, -1, 0);
		var near = new Vector3(0, 0, 0.1f);

		setQuadPositioningAndSpans();
		Camera.main.transform.position = new Vector3(S.CellsAcross/2, S.CellsAcross/2, 0); // put in the middle of floor

		// make the boundaries of the floor
		var v = GameObject.CreatePrimitive(PrimitiveType.Quad);
		v.name = "Boundary Top";
		v.transform.position = game.farBack + game.horiBarCenter + pastMaxY;
		v.transform.localScale = stretchX;
		picSetting(v, "cursor_green");
		
		v = GameObject.CreatePrimitive(PrimitiveType.Quad);
		v.name = "Boundary Bottom";
		v.transform.position = game.farBack + game.horiBarCenter + bumpDown;
		v.transform.localScale = stretchX;
		picSetting(v, "cursor_green");

		v = GameObject.CreatePrimitive(PrimitiveType.Quad);
		v.name = "Boundary Left";
		v.transform.position = game.farBack + game.vertBarCenter + bumpLeft;
		v.transform.localScale = stretchY;
		picSetting(v, "cursor_green");

		v = GameObject.CreatePrimitive(PrimitiveType.Quad);
		v.name = "Boundary Right";
		v.transform.position = game.farBack + game.vertBarCenter + pastMaxX;
		v.transform.localScale = stretchY;
		picSetting(v, "cursor_green");


		
		cursorCell = GameObject.CreatePrimitive(PrimitiveType.Quad);
		cursorCell.name = "Cursor";
		cursorCell.transform.position = near;
		picSetting(cursorCell, "cursor");
	}
	
	void FixedUpdate() {
		// all we do in this method is pan around the dungeon floor (ONLY when editing map)
		if (hud.Mode != HudMode.EditMap)
			return;

//		float speed = 0.06f;
//		int mar = 16; // margin from edge, where we pan camera
//
//		if (mouPos.x > Screen.width - mar &&
//		    Camera.main.transform.position.x < S.CellsAcross)
//			Camera.main.transform.position += new Vector3(speed, 0, 0);
//		if (mouPos.x < mar &&
//		    Camera.main.transform.position.x > 0)
//			Camera.main.transform.position -= new Vector3(speed, 0, 0);
//		if (mouPos.y > Screen.height - mar &&
//		    Camera.main.transform.position.y < S.CellsAcross)
//			Camera.main.transform.position += new Vector3(0, speed, 0);
//		if (mouPos.y < mar &&
//		    Camera.main.transform.position.y > 0)
//			Camera.main.transform.position -= new Vector3(0, speed, 0);
	}

	int prevX;
	int prevY;
	void Update() { // ................then quantized world position (to match the cell grid)
		if (Input.GetMouseButton(2))
			Camera.main.transform.position -= (game.mouPos - game.prevMouPos) / 50f;

		if (scaledUnitSquareContains(game.mouWorldPos, game.entireFloor)) {
			int x = (int)((game.mouWorldPos.x + 0.5f) / 1f);
			int y = (int)((game.mouWorldPos.y + 0.5f) / 1f);
			game.mouWorldPos.x = x;
			game.mouWorldPos.y = y;

			// if just now pressed
			if (Input.GetMouseButtonDown(0)) {
				game.makeRealtimeQuad(game.mouWorldPos, hud.BrushType, hud.BrushPic);
			}
			if (Input.GetMouseButtonDown(1)) {
				destroyOneQuad(game.mouWorldPos);
			}

			if (!(x == prevX && y == prevY)) {
				// we just moved to a new cell
				if (Input.GetMouseButton(0))
					game.makeRealtimeQuad(game.mouWorldPos, hud.BrushType, hud.BrushPic);
				if (Input.GetMouseButton(1))
					destroyOneQuad(game.mouWorldPos);
			}

			prevX = x;
			prevY = y;

			cursorCell.renderer.material.mainTexture = cursOk;
		}else{
			cursorCell.renderer.material.mainTexture = cursBad;
		}

		cursorCell.transform.position = game.mouWorldPos;
	}
	
	void destroyOneQuad(int x, int y) {
		destroyOneQuad(new Vector3(x, y, 0));
	}
	void destroyOneQuad(Vector3 pos) {
		var cl = game.cellsRealtime[(int)pos.y, (int)pos.x]; // cell list
	
		if (cl == null || cl.Count < 1)
			return;

		Destroy(cl[cl.Count-1].O);
		cl.RemoveAt(cl.Count-1);
		Debug.Log("destroyQuad()");
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
		pastMaxX = new Vector3(S.CellsAcross, 0, 0);
		pastMaxY = new Vector3(0, S.CellsAcross, 0);
		stretchX = new Vector3(S.CellsAcross, 1, 1);
		stretchY = new Vector3(1, S.CellsAcross, 1);
	}
	
	public string[] LoadDirectory() {
		var dir = PlayerPrefs.GetString("Directory", ""); // we have to create our own directory of "file" names which are keys into...
		var mapNames = dir.Split(splitSeperator);

		if (mapNames.Length < 1)
			mapNames[0] = "No files exist right now!";

		return mapNames;
	}
	
	public void LoadMap(string name) {
		Debug.Log("LoadMap(" + name + ")");

		var d = PlayerPrefs.GetString(name);

		if (string.IsNullOrEmpty(d)) {
			Debug.LogError("LoadMap(" + name + ") was not able to get any map data!");
		}else{
			var bf = new BinaryFormatter();
			var ms = new MemoryStream(Convert.FromBase64String(d));
			var mf = (MapFormat)bf.Deserialize(ms);

			for (int y = 0; y < S.CellsAcross; y++) {
				for (int x = 0; x < S.CellsAcross; x++) {
					if (game.cellsRealtime[y,x] != null)	{
						while (game.cellsRealtime[y,x].Count > 0)
							destroyOneQuad(x, y);
					}

					if (mf.Cells[y,x] != null && 
					    mf.Cells[y,x].Count > 0)
					{
						foreach (var c in mf.Cells[y,x]) {
							var t = (ObjectType)Enum.Parse(typeof(ObjectType), mf.Types[(int)c.Type]);
							Debug.Log("t: " + t);
							var pic = Pics.Get("" + t, mf.Pics[c.Pic]);
							Debug.Log("mf.Pics[c.Pic]: " + mf.Pics[c.Pic]);
							game.makeRealtimeQuad(new Vector3(x, y, 0), t, pic);
						}
					}
				}
			}
		}
	}
	
	public void SaveMap(string name) { 
		// FIXME: need to check that there isn't already a key/map with that name!
		// (to warn about losing existing map if proceeding)
		Debug.Log("SaveMap(" + name + ")");

		// add entry to our directory key (
		var dir = PlayerPrefs.GetString("Directory", ""); // we have to create our own directory of "file" names which are keys	into PlayerPrefs
		if (string.IsNullOrEmpty(dir))
			dir = name;
		else 
			dir += splitSeperator + name;
		PlayerPrefs.SetString("Directory", dir);

		// make tiny version of map for saving
		var mf = new MapFormat();
		for (int y = 0; y < S.CellsAcross; y++) {
			for (int x = 0; x < S.CellsAcross; x++) {
				if (game.cellsRealtime[y,x] != null && 
				    game.cellsRealtime[y,x].Count > 0)
				{
					mf.Cells[y,x] = new List<TileData>();

					foreach (var rt in game.cellsRealtime[y,x]) {
						var picName = rt.O.renderer.material.mainTexture.name;
						int ti = mf.Types.IndexOf("" + rt.Type); // type index
						int pi = mf.Pics.IndexOf(picName);   // pic index

						if (ti < 0) {
							ti = mf.Types.Count;
							mf.Types.Add("" + rt.Type);
						}
						if (pi < 0) {
							pi = mf.Pics.Count;
							mf.Pics.Add(picName);
						}

						var td = new TileData();
						td.Type = (ObjectType)ti; 
						td.Pic = pi;
						mf.Cells[y,x].Add(td);
					}
				}			
			}
		}
		var bf = new BinaryFormatter();
		var ms = new MemoryStream();
		bf.Serialize(ms, mf);
		PlayerPrefs.SetString(name, Convert.ToBase64String(ms.GetBuffer()));
	}

	public string ProblemWithName(string name) {
		// check that map name is not empty, and that it doesn't contain the .Split seperator char
		if (name.Contains("" + splitSeperator)) {
			return "CANNOT USE '" + splitSeperator + "' !!!";
		}else if (string.IsNullOrEmpty(name)) {
			return "TYPE IN A NAME!!!";
		}else{
			return null;
		}
	}
	
	void picSetting(GameObject o, string s, string shader = "Unlit/Transparent") {
		o.renderer.material.shader = Shader.Find(shader);
		o.renderer.material.mainTexture = Pics.GetFirstWith(s);
	}
}
