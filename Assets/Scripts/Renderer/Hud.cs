using UnityEngine;
using System.Collections;

public class Hud : MonoBehaviour {
	public ObjectType BrushType;
	public Texture BrushPic;
	public string MapName;

	private HudMode mode = HudMode.EditPalette;
	public HudMode Mode { 
		get { return mode; }
		set { 
			// do this when changing FROM the OLD mode
			if (mode == HudMode.Playing) {
				if (Player.O != null) {
					Player.O.SetActive(false);
					Debug.Log("making player INactive");
				}
			}

			mode = value; 

			// do this when changing TO the NEW mode
			if (mode == HudMode.Playing) {
				if (Player.O != null) {
					var v = Camera.main.transform.position;
					v.z = 0.35f;
					Player.O.transform.position = v;
					Player.O.SetActive(true);
					Debug.Log("making player ACTive");
				}
			}
		}
	}

	// private
	MapEditor mapEditor;
	ObjectType currType = ObjectType.Wall;
	string[] mapNames;
	Vector3 mouPos; 
	Rect screen;
	int maxTilesAcross = 20;
	int span = 32;
	Color highlighted = Color.magenta;
	Color editBox = Color.magenta;
	string defaultEditBox = "Type name here";
	Texture panView;
	Texture mousePic;



	void Start() {
		MapName = defaultEditBox;
		BrushType = currType;
		BrushPic = Pics.GetFirstWith("abyss0");
		panView = Pics.GetFirstWith("PanView");
		mousePic = Pics.GetFirstWith("MouseWithWheel");
		mapEditor = GetComponent<MapEditor>();
	}
	
	void Update() {
		switch (Mode) {
			case HudMode.Playing:
				var p =	Player.O.transform.position;
				p.z = Camera.main.transform.position.z;
				Camera.main.transform.position = p;
				break;
		}
	}
	
	void OnGUI() {
		screen = new Rect(0, 0, Screen.width, Screen.height);
		
		// convert mouse pos to gui coordinates (y increasing DOWNwards)
		mouPos = Input.mousePosition;
		mouPos.y = Screen.height - mouPos.y;
		
		// hud state machine
		switch (Mode) {
		case HudMode.EditPalette:
			drawTextureChoosingGrid();
			break;
		case HudMode.Playing:
			commonChrome();
			
			if (Player.Class == Class.NotSelected) {
				int w = Screen.width / (int)Class.Count-1;
				int h = Screen.height/2;
				GUILayout.BeginArea(new Rect(0, h, Screen.width, h));
				GUILayout.BeginHorizontal();
				for (int i = 1; i < (int)Class.Count; i++) {
					if (GUILayout.Button("" + (Class)i, GUILayout.MinWidth(w), GUILayout.MinHeight(h)))
						Player.Class = (Class)i;
				}
				GUILayout.EndHorizontal();
				GUILayout.EndArea();
			}else{
				GUI.Box(new Rect(0, Screen.height - 30, Screen.width, 30), 
				        "Level: " + Player.Level + "   " + Player.Class + "   HP: " + Player.Hp + "   XP: " + Player.Xp);
			}
			break;
		default:
			commonChrome();
			break;
		}
	}

	void commonChrome() { // most modes would be always drawing these graphics
		GUILayout.BeginArea(screen);
		mainMenu();
		GUILayout.EndArea();
		
		// draw ERASE graphic, then BRUSH graphic.... in bottom right corner
		int ds = span*2; // double span
		int mar = span / 4; // margin around texture graphic
		int mouW = ds*3 + mar*6; // mouse width
		var r = new Rect(Screen.width-mouW, Screen.height-ds*2, 
		                              mouW, ds*2);
		GUI.DrawTextureWithTexCoords(r, mousePic, new Rect(0f, 0.6666f, 1f, 0.3333f));

		r = new Rect(Screen.width-ds-mar, Screen.height-ds, ds, ds);
		GUI.DrawTexture(r, Pics.Black);
		r.x -= ds+mar*2;
		GUI.DrawTexture(r, panView);
		r.x -= ds+mar*2;
		GUI.DrawTexture(r, BrushPic);
	}

	void mainMenu() {
		GUILayout.BeginHorizontal();
		for (int i = 0; i < (int)HudMode.Count; i++) { // object type index
			var cmi = (HudMode)i; // current mode iteration for this loop (not the "Mode" we're in)
			
			if (Mode == cmi) {
				GUI.color = editBox;
				if (Mode == HudMode.SaveMap)
					MapName = GUILayout.TextField(MapName, 25);
				GUI.color = highlighted;
			}else{
				GUI.color = Color.white;
			}

			if (GUILayout.Button("" + cmi)) {
				if (Mode == HudMode.SaveMap) {
					var prob = mapEditor.ProblemWithName(MapName);

					if (prob == null) {
						mapEditor.SaveMap(MapName);
						Mode = HudMode.EditMap;

						MapName = defaultEditBox;
						editBox = highlighted;
					}else{
						MapName = prob;
						editBox = Color.red;
					}
				}else{ // for all other modes, we just switch to that mode
					Mode = cmi;

					if (Mode == HudMode.LoadMap) {
						mapNames = mapEditor.LoadDirectory();
					}
				}
			}
		}
		GUI.color = Color.white;
		GUILayout.EndHorizontal();

		if (Mode == HudMode.LoadMap) {
			// show list of available maps
			scroll = GUILayout.BeginScrollView(scroll);
			//GUILayout.BeginVertical();
			foreach (var s in mapNames) {
				if (GUILayout.Button(s)) {
					mapEditor.LoadMap(s);
					Mode = HudMode.EditMap;
				}
			}
			//GUILayout.EndVertical();
			GUILayout.EndScrollView();
		}
	}

	Vector2 scroll = Vector2.zero;
	void drawTextureChoosingGrid() { // draw all textures to choose from
		GUI.DrawTexture(screen, Pics.Black);
		GUILayout.BeginArea(screen);

		mainMenu();

		// show the diff types/folders
		GUILayout.BeginHorizontal();
		for (int oIdx = 0; oIdx < (int)ObjectType.Count; oIdx++) { // object type index
			if (currType == (ObjectType)oIdx)
				GUI.color = highlighted;
			else
				GUI.color = Color.white;

			if (GUILayout.Button("" + (ObjectType)oIdx)) {
				currType = (ObjectType)oIdx;
			}
		}
		GUI.color = Color.white;
		GUILayout.EndHorizontal();

		scroll = GUILayout.BeginScrollView(scroll);

		int bSpan = span + span/2; // button span, cuz it has borders
		int maxInRow = Screen.width / (bSpan+8);
		int num = Pics.GetArrayCount("" + currType);
		int arrIdx = 0;
		while (arrIdx < num) {
			GUILayout.BeginHorizontal();
			for (int i=0; 
			     arrIdx+i < num && i < maxInRow; 
			     i++) 
			{ // Texture array index
				var p = Pics.Get(currType, arrIdx+i);

				if (GUILayout.Button(p, GUILayout.MinWidth(bSpan), GUILayout.MinHeight(bSpan))) {
			    	BrushPic = p;
					BrushType = currType;
					Mode = HudMode.EditMap;
			    }
			}
			
			arrIdx += maxInRow;
			GUILayout.EndHorizontal();
		}

		DoneIteratingThruPics:
		GUILayout.EndScrollView();
		GUILayout.EndArea();



//		// draw selection outline
//		GUI.DrawTexture(new Rect((int)mouPos.x/(int)span*span, 
//		                         (int)mouPos.y/(int)span*span, span, span), Pics.GetFirstThatContains("curs"));
	}

	void OnEnable() {
	}
	void OnDisable() {
	}
}
