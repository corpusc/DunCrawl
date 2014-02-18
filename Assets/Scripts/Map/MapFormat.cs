﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapFormat {
	// lists of strings, of only the types and textures used in this map.
	// the indexes of which are the minimal data stored in each tile.
	// this allows tiny sizes, & our ability to add to 
	// our types and texture library while preserving integrity of older maps.
	// (a simplistic indexing scheme would cause the indexes to change as the
	// types/textures library was changed)
	public List<string> Types = new List<string>();
	public List<string> Pics = new List<string>(); // textures
	
	
	
	// .......then the grid of cells 
	// 		* which are stacks of TileData, which only stores index numbers into those 2 string lists

	// looks like we should have a version of the map for realtime, and a stripped down version
	//		for map saving.  so we don't store all unneeded game object info  and such
	// (they will get generated	from the small map format
}
