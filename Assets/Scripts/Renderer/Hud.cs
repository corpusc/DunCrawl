using UnityEngine;
using System.Collections;

public class Hud : MonoBehaviour {
	public HudMode Mode = HudMode.EditPalette;
	public PicData Brush = new PicData();

	// private
	ObjectType currType = ObjectType.Floor;
	Vector3 mouPos; 
	Rect screen;
	int maxTilesAcross = 20;
	int span = 32;



	void Start() {
		Brush.Pic = Pics.GetFirstWith("tab_unselected");
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

	string[] mapNames;
	void loadMapNames() {
		var dir = PlayerPrefs.GetString("Directory", ""); // we have to create our own directory of "file" names which are ksy
		mapNames = dir.Split('-');
		var data = PlayerPrefs.GetString("HighScores");
		if(!string.IsNullOrEmpty(data))	{
			var b = new BinaryFormatter();
			var m = new MemoryStream(Convert.FromBase64String(data));
			highScores = (List<ScoreEntry>)b.Deserialize(m);
		}
	}

	void loadMap(string mapName) {
		var data = PlayerPrefs.GetString(mapName);

		if (!string.IsNullOrEmpty(data)) {
			var b = new BinaryFormatter();
			var m = new MemoryStream(Convert.FromBase64String(data));
			highScores = (List<ScoreEntry>)b.Deserialize(m);
		}
	}

	void saveMap(string name) {
		var b = new BinaryFormatter();
		var m = new MemoryStream();
		b.Serialize(m, highScores);
		var dir = PlayerPrefs.GetString("Directory", ""); // we have to create our own directory of "file" names which are ksy
		// into PlayerPrefs
		dir += "-" + name;
		PlayerPrefs.SetString("Directory", dir);
		PlayerPrefs.SetString(name, Convert.ToBase64String(m.GetBuffer()));
	}

	void commonChrome() { // most modes would be always drawing these graphics
		GUILayout.BeginArea(screen);
		allowChangingMode();
		GUILayout.EndArea();
		
		// draw ERASE graphic, then BRUSH graphic.... in bottom right corner
		int ds = span*2; // double span
		var r = new Rect(Screen.width-ds-20/*past scrollbar*/, Screen.height-ds, ds, ds);
		GUI.DrawTexture(r, Pics.Black);
		GUI.Label(r, "RMB");
		r.x -= ds;
		GUI.DrawTexture(r, Brush.Pic);
		GUI.Label(r, "LMB");
	}

	void allowChangingMode() {
		GUILayout.BeginHorizontal();
		for (int i = 0; i < (int)HudMode.Count; i++) { // object type index
			if (Mode == (HudMode)i)
				GUI.color = Color.cyan;
			else
				GUI.color = Color.white;

			if (GUILayout.Button("" + (HudMode)i)) {
				Mode = (HudMode)i;
			}
		}
		GUI.color = Color.white;
		GUILayout.EndHorizontal();
	}

	Vector2 scroll = Vector2.zero;
	void drawTextureChoosingGrid() { // draw all textures to choose from
		GUI.DrawTexture(screen, Pics.Black);
		GUILayout.BeginArea(screen);

		allowChangingMode();

		// show the diff types/folders
		GUILayout.BeginHorizontal();
		for (int oIdx = 0; oIdx < (int)ObjectType.Count; oIdx++) { // object type index
			if (currType == (ObjectType)oIdx)
				GUI.color = Color.cyan;
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
			    	Brush.Pic = p;
					Brush.Type = currType;
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
