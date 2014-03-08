using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Game : MonoBehaviour {
	// realtime version of map
	public List<TileDataRealtime>[,] cellsRealtime = new List<TileDataRealtime>[S.CellsAcross, S.CellsAcross];

	// mouse
	public Vector3 prevMouPos;
	public Vector3 mouPos; // mouse position on screen
	public Vector3 mouWorldPos; // mouse position in the world

	// map sizing/positioning
	public Vector3 farBack = new Vector3(0, 0, 1.5f);
	public Vector3 stretchX = new Vector3(S.CellsAcross, 1, 1);
	public Vector3 stretchY = new Vector3(1, S.CellsAcross, 1);
	public Vector3 horiBarCenter;
	public Vector3 vertBarCenter;
	public GameObject entireFloor; // a single black background that covers the whole possible floor space.
	// for the EditMap mode, i wanted the out of bounds area to be a different color.
	// (which is why i didn't just make the default background black)
	// this way we don't have a bunch of objects in the scene just to represent every single blank cell.
	// instead, we only store and do calculations on objects that the player can see and interact with.
	
	// private
	Hud hud;
	Cell[,] cells;
	DungeonGenerator dunGen; 



	void Start() {
		// various setup
		Physics2D.gravity = new Vector3(0, 0, 0);
		hud = GetComponent<Hud>();

		// make floor of the whole map 
		float f = (S.CellsAcross * 0.5f) - 0.5f;
		horiBarCenter = new Vector3(f, 0, 0);
		vertBarCenter = new Vector3(0, f, 0);
		entireFloor = GameObject.CreatePrimitive(PrimitiveType.Quad);
		entireFloor.name = "Map Chunk Backdrop";
		entireFloor.transform.position = farBack + horiBarCenter + vertBarCenter;
		entireFloor.transform.localScale = Vector3.Scale(stretchX, stretchY);
		entireFloor.renderer.material.shader = Shader.Find("Unlit/Transparent");
		entireFloor.renderer.material.mainTexture = Pics.Black;
		
		// generate dungeon
		dunGen = GetComponent<DungeonGenerator>();
		dunGen.mapSize_X = S.CellsAcross;
		dunGen.mapSize_Y = S.CellsAcross;
		cells = dunGen.Generate();

		// make dungeon visuals & physics 
		hud.Mode = HudMode.EditMap;
		for (int y = 0; y < S.CellsAcross; y++) {
			for (int x = 0; x < S.CellsAcross; x++) {
				Debug.Log(cells[y, x].type + "");

				switch (cells[y, x].type) {
				case "Path":
					makeRealtimeQuad(new Vector3(x, y, 0.4f), ObjectType.Floor, Pics.Get("Floor", "limestone0"));
					break;
				case "Wall":
					makeRealtimeQuad(new Vector3(x, y, 0.4f), hud.BrushType, hud.BrushPic);
					break;
				}
			}
		}
		hud.Mode = HudMode.Playing;
	}
	
	void Update() {
		Player.Update();

		// setup mouse vars
		if (Input.GetMouseButtonDown(2)) {
			prevMouPos = mouPos = Input.mousePosition;
		}else{
			prevMouPos = mouPos;
			mouPos = Input.mousePosition;
		}
		
		mouPos.z = 0.4f; //10.0f;
		mouWorldPos = Camera.main.ScreenToWorldPoint(mouPos);
	}
				
	public void makeRealtimeQuad(Vector3 pos, ObjectType type, Texture pic) {
		if (hud.Mode != HudMode.EditMap)
			return;
		
		pos.z = 1f;
		
		// don't make, if user is clicking part of the button strip at top of screen
		if (mouPos.y >= Screen.height - hud.BVS)
			return;
		
		bool addingNewQuad = false;
		var cl = cellsRealtime[(int)pos.y, (int)pos.x]; // cell list
		//Debug.Log("pos.x: " + (int)pos.x + "  pos.y: " + (int)pos.y);
		
		if (cl == null) {
			cl = cellsRealtime[(int)pos.y, (int)pos.x] = new List<TileDataRealtime>();
			//Debug.Log("     cell was null, made a new list     ");
		}
		
		if (cl.Count < 1) {
			addingNewQuad = true;
			//Debug.Log("     cell had empty list     ");
		}
		
		// replace if brush is the same type as something else in the list
		for (int i = cl.Count-1; i >= 0; i--) {
			//Debug.Log("     at least one PicData here     ");
			
			if (cl[i].Type == hud.BrushType) {
				Destroy(cl[i].O);
				cl.RemoveAt(i);
				addingNewQuad = true;
			}
		}
		
		if (addingNewQuad) {
			var td = new TileDataRealtime();
			var o = GameObject.CreatePrimitive(PrimitiveType.Quad);
			o.transform.parent = entireFloor.transform;
			o.transform.position = pos;
			//o.renderer.material.shader = Shader.Find("Unlit/Transparent");
			o.renderer.material.mainTexture = pic;
			if (type == ObjectType.Wall) {
				var po = new GameObject();
				po.transform.parent = entireFloor.transform;
				po.transform.position = pos;
				po.AddComponent<BoxCollider2D>();
			}
			td.Type = type;
			td.O = o;
			cellsRealtime[(int)pos.y, (int)pos.x].Add(td);
		}
	}
}
