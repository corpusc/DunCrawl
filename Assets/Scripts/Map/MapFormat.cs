using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// for saving/loading via BinaryFormatter
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;



[Serializable]
public class MapFormat {
	// we have a version of the map for realtime, and a stripped down version
	// for map saving.  so we don't store all the unneeded (& large) game object
	// info and such (they will get generated from the tiny map format upon loading)

	// lists of strings.  of only the types and textures used in this map.
	// the indexes of which are the minimal data stored in each tile.
	// this allows tiny sizes, & our ability to add to 
	// our types and texture library while preserving integrity of older maps.
	// (a simplistic indexing scheme would cause the indexes to change as the
	// types/textures library was changed)
	public List<string> Types = new List<string>();
	public List<string> Pics = new List<string>(); // textures
	
	// array of tile lists
	public List<TileData>[,] Cells = new List<TileData>[S.CellsAcross, S.CellsAcross];
}
