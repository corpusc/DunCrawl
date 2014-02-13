using UnityEngine;
//using System;
using System.Collections;



public enum HudMode {
	Playing,
	//Dead, this shouldn't be choosable, so might as well keep disabled until we have player death
	EditMap,
	EditPalette,
	
	Count // used for iterating thru these types
};

public enum ObjectType { // atm, we're not loading everything cuz it doesn't look like we can load lots of 
	// single textures without making a huge pause before you can get in and interact with the game.
	// atlases aren't that much of an extra step.  and unity is supposed to have a feature where it
	// breaks them up into single units of some type so they can effectively appear as one item.
	// altho then the names would have to be pulled from the Dungeon Crawl Stone Soup source code since
	// we'd lose the individual filenames

	Deco, // decoration.  can be on top of walls/floors, UNDER items and players (maybe over, if its a web)
	Floor,
	Icon,
	Item,
	Monster,
	Wall,

	Count // used for iterating thru these types
};