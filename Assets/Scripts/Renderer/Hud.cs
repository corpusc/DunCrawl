using UnityEngine;
using System.Collections;

public class Hud : MonoBehaviour {
	public HudMode Mode = HudMode.EditPalette;
	public ObjectType BrushType;
	public Texture BrushPic;
	public string MapName;

	// private
	MapEditor mapEditor;
	ObjectType currType = ObjectType.Floor;
	Vector3 mouPos; 
	Rect screen;
	int maxTilesAcross = 20;
	int span = 32;
	Color highlighted = Color.magenta;
	Color editBox = Color.magenta;
	string defaultEditBox = "Type name here";



	void Start() {
		MapName = defaultEditBox;
		BrushType = currType;
		BrushPic = Pics.GetFirstWith("tab_unselected");
		mapEditor = GetComponent<MapEditor>();
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
//			case HudMode.Playing:
//				commonChrome();
//				break;
//			case HudMode.EditMap:
//				commonChrome();
//				break;
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
		var r = new Rect(Screen.width-ds-20/*past scrollbar*/, Screen.height-ds, ds, ds);
		GUI.DrawTexture(r, Pics.Black);
		GUI.Label(r, "RMB");
		r.x -= ds;
		GUI.DrawTexture(r, BrushPic);
		GUI.Label(r, "LMB");
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
				}
			}
		}
		GUI.color = Color.white;
		GUILayout.EndHorizontal();
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
		int num = Pics.GetArrayCount((int)currType);
		int arrIdx = 0;
		while (arrIdx < num) {
			GUILayout.BeginHorizontal();
			for (int i=0; 
			     arrIdx+i < num && i < maxInRow; 
			     i++) 
			{ // Texture array index
				var p = Pics.Get((int)currType, arrIdx+i);

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
