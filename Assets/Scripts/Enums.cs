using UnityEngine;
//using System;
using System.Collections;



public enum HudMode {
	Playing,
	//Dead,        this shouldn't be choosable, so might as well keep disabled until we have player death
	EditMap,
	EditPalette,
	SaveMap,
	LoadMap,
	
	Count // used for iterating thru these types
};

public enum ObjectType {
	Deco, // decoration.  can be on top of walls/floors, UNDER items and players (maybe over, if its a web)
	Floor,
	Icon,
	Item,
	Monster,
	Wall,

	Count // used for iterating thru these types
};