using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// *************** PICTURE MANAGER ***************
// i've never seen a satisfying abbreviation for texture, that is not ugly and/or easy to confuse with "text"/strings....
// so i call them pics


public static class Pics { 
	public static Texture Black;

	// private
	//const int numTypes = (int)ObjectType.Count;
	static List<Object[]> pics = new List<Object[]>();



	static Pics() {
		// load a list of textures for every ObjectType (folders with the same name)
		for (int i = 0; i < (int)ObjectType.Count; i++) {
			string s = "";
			
			Debug.Log("_____________________" + (ObjectType)i + "_____________________");
			pics.Add(Resources.LoadAll<Texture>("Pics/DCStoneSoup/" +(ObjectType)i));

			foreach (var o in pics[pics.Count-1]) {
				s += o.name + ", ";

				if (o.name == "black")
					Black = (Texture)o;
			}
			
			Debug.Log(s);
		}
	}	
	
	public static int GetArrayCount(int type) {
		return pics[type].Length;
	}
	
	static public Texture Get(int type, int arrIdx) {
		return (Texture)pics[type][arrIdx];
	}

	public static Texture GetFirstWith(string s) { // first that contains this string
		for (int i = 0; i < (int)ObjectType.Count; i++) {
			int arrIdx = 0;
			foreach (var o in pics[i]) {
				if (o.name.Contains(s))
				    return (Texture)pics[i][arrIdx];

			    arrIdx++;
			}
		}

		return null;
	}
}
