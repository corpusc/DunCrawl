using UnityEngine;
using System.Collections;

public class Hud : MonoBehaviour {
	public HudMode Mode = HudMode.EditPalette;
	public Texture Brush;

	// private
	ObjectType curr = ObjectType.Floor;
	Vector3 mouPos; 
	Rect screen;
	int maxTilesAcross = 20;
	int span = 32;



	void Start() {
		Brush = Pics.GetFirstWith("tab_unselected");
	}

	void OnGUI() {
		screen = new Rect(0, 0, Screen.width, Screen.height);

		// convert mouse pos to gui coordinates (y increasing DOWNwards)
		mouPos = Input.mousePosition;
		mouPos.y = Screen.height - mouPos.y;
		
		// hud state machine
		switch (Mode) {
			case HudMode.EditPalette:
				editPalette();
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
		
		// draw ERASE graphic, then BRUSH graphic.... in bottom right corner
		int ds = span*2; // double span
		var r = new Rect(Screen.width-ds-20/*past scrollbar*/, Screen.height-ds, ds, ds);
		GUI.DrawTexture(r, Pics.Black);
		GUI.Label(r, "RMB");
		r.x -= ds;
		GUI.DrawTexture(r, Brush);
		GUI.Label(r, "LMB");
	}

	void commonChrome() { // most modes would be always drawing these graphics
		GUILayout.BeginArea(screen);
		allowChangingMode();
		GUILayout.EndArea();
	}

	void allowChangingMode() {
		GUILayout.BeginHorizontal();
		for (int i = 0; i < (int)HudMode.Count; i++) { // object type index
			if (Mode == (HudMode)i)
				GUI.color = Color.magenta;
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
	void editPalette() { // draw all textures to choose from
		GUI.DrawTexture(screen, Pics.Black);
		GUILayout.BeginArea(screen);

		allowChangingMode();

		// show the diff types/folders
		GUILayout.BeginHorizontal();
		for (int oIdx = 0; oIdx < (int)ObjectType.Count; oIdx++) { // object type index
			if (curr == (ObjectType)oIdx)
				GUI.color = Color.magenta;
			else
				GUI.color = Color.white;

			if (GUILayout.Button("" + (ObjectType)oIdx)) {
				curr = (ObjectType)oIdx;
			}
		}
		GUI.color = Color.white;
		GUILayout.EndHorizontal();

		scroll = GUILayout.BeginScrollView(scroll);

		int bSpan = span + span/2; // button span, cuz it has borders
		int maxInRow = Screen.width / (bSpan+8);
		int num = Pics.GetArrayCount((int)curr);
		int arrIdx = 0;
		while (arrIdx < num) {
			GUILayout.BeginHorizontal();
			for (int i=0; 
			     arrIdx+i < num && i < maxInRow; 
			     i++) 
			{ // Texture array index
				var p = Pics.Get((int)curr, arrIdx+i);

				if (GUILayout.Button(p, GUILayout.MinWidth(bSpan), GUILayout.MinHeight(bSpan))) {
				    	Brush = p;
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
