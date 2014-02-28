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
	static Dictionary<string, Object[]> pics = new Dictionary<string, Object[]>();



	static Pics() {
		// load a list of textures for every ObjectType (folders with the same name)
		for (int i = 0; i < (int)ObjectType.Count; i++) {
			string s = "";
			var ot = (ObjectType)i;
			
			Debug.Log("_____________________" + ot + "_____________________");
			pics.Add("" + ot, Resources.LoadAll<Texture>("Pics/DCStoneSoup/" + ot));

			foreach (var o in pics["" + ot]) {
				s += o.name + ", ";

				if (o.name == "black")
					Black = (Texture)o;
			}
			
			Debug.Log(s);
		}
	}	
	
	public static int GetArrayCount(string type) {
		return pics[type].Length;
	}
	
	static public Texture Get(ObjectType objectType, int arrIdx) {
		return (Texture)pics["" + objectType][arrIdx];
	}
	static public Texture Get(string type, string name) {
		foreach (var o in pics[type]) {
			if (o.name == name)
				return (Texture)o;
		}

		return null;
	}
	
	public static Texture GetFirstWith(string s) { // first that contains this string
		for (int i = 0; i < (int)ObjectType.Count; i++) {
			var ot = (ObjectType)i;
			foreach (var o in pics["" + ot]) {
				if (o.name.Contains(s))
				    return (Texture)o;
			}
		}

		return null;
	}
}
