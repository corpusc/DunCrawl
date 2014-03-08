/*
 * This script is a converted version of DungeonGenerator.cs
 * It uses most methods in DungeonGenerator.cs as coroutines
 * You will get a yield for every important step the code takes.
 * Find the commentary for this script in DungeonGenerator.cs
 * This script doesn't contain all reference DungeonGenerator.cs has. Use this script only for learning.
 * Enjoy!
 */ 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VisualGenerator : DungeonGenerator 
{
	//Use these settings so that the real map and the visual version don't overlap.
	public float visualInGameLoc_x = 1000;
	public float visualInGameLoc_y = 1000;
	
	private GameObject[,] _VisualMapArray;
	
	public override Cell[,] Generate()
	{
		StartCoroutine(StartVisualGeneration());
		return null;
	}
	
	IEnumerator StartVisualGeneration()
	{
		ResizeCameraToMap(); //Nothing to visualize here
		AdjustMapViewCamera(); //Nothing to visualize here
		
		if(toggleDebugPrint) Debug.Log("Started on " + System.DateTime.UtcNow.ToString());
		
		DeletePreviousMap(); //Nothing to visualize here
		DestroyPlayer(); //Nothing to visualize here
		CreateNewMap(); //Nothing to visualize here
		CreateCells(); //Nothing to visualize here
		
		_VisualMapArray = new GameObject[mapSize_X, mapSize_Y];
		
		//if(resizeCamera) ResizeCameraForMap(); //Nothing to visualize here
		
		yield return StartCoroutine(CreateRoomsRoutine());
		yield return StartCoroutine(CreateMazeRoutine());
		
		yield return StartCoroutine(SetAllCellsOfTypeAToBRoutine("Abyss", "Wall")); //Nothing to visualize here
		
		yield return StartCoroutine(CloseDeadEndCellsRoutine());
		yield return StartCoroutine(ReduceUCorridorsRoutine(reduceUCorridorCycles));
		
		RedrawAllCells();
		
		PlaceStairs(); //Nothing to visualize here
		if(convertUnreachableWallsToAbyss) ConvertUnreachableWallsTo("Abyss"); //Nothing to visualize here
		PlacePlayerOnMap(); //Nothing to visualize here
		if(toggleDebugPrint) Debug.Log("Ended on " + System.DateTime.UtcNow.ToString());
		ConvertMapArrayIntoStringMapArray(); //Nothing to visualize here
		
		RedrawAllCells();
		
		yield return null;
	}
	
	protected override void DeletePreviousMap ()
	{
		if(toggleDebugPrint) Debug.Log("Deleting previous map.");
		_roomsCreated = 0;
		_MapArray = new Cell[0,0];
		_PrefabMapArray = new GameObject[0,0];
		_VisualMapArray = new GameObject[0,0];
		DestroyChildrenOf(_MapGOTr);
		if(toggleDebugPrint) Debug.Log("Deleted previous map.");
	}
	
	IEnumerator CreateRoomsRoutine()
	{
		if(toggleDebugPrint) Debug.Log("Creating rooms.");
		
		//Checking if we want any rooms in the map
		if(roomFrequency < 1) yield return null; //Previously: return true;
		
		//Checking if room size is too small or too big to calculate
	    if(room_MinX < 3 || room_MaxX < 3 ||room_MaxX >= mapSize_X)
	    {
	        Debug.LogError("Your values for room size X are too small or too big for map size! Setting room sizes of X to 3");
	        room_MinX = 3; room_MaxX = 3;
	    }
		if(room_MinY < 3 || room_MaxY < 3 || room_MaxY >= mapSize_Y)
	    {
	        Debug.LogError("Your values for room size Y are too small or too big for map size! Setting room sizes of Y to 3");
	        room_MinY = 3; room_MaxY = 3;
	    }
		if(room_MinX > room_MaxX)
		{
	        Debug.LogError("Your values for room size X are swapped! Reswapping them.");
			int tempX = room_MinX;
			room_MinX = room_MaxX;
			room_MaxX = tempX;
		}
		if(room_MinY > room_MaxY)
		{
	        Debug.LogError("Your values for room size Y are swapped! Reswapping them.");
			int tempY = room_MinY;
			room_MinY = room_MaxY;
			room_MaxY = tempY;
		}
		
		do
		{
			//Make a random size room
			int currentRoom_X = Random.Range(room_MinX,room_MaxX);
			int currentRoom_Y = Random.Range(room_MinY,room_MaxY);

			//Room size must be odd, force even numbers down
			if(currentRoom_X%2 != 1) currentRoom_X-=1;
			if(currentRoom_Y%2 != 1) currentRoom_Y-=1;
			
			//Pick a random cell for placement:
			//1 - Rooms must not overflow outside of map
			//2 - Rooms must be on even numbers due to maze generation algorithm
			int currentPlacement_X = Random.Range(0, mapSize_X - (currentRoom_X+2));
			int currentPlacement_Y = Random.Range(0, mapSize_Y - (currentRoom_Y+2));
			if(currentPlacement_X%2 != 0)currentPlacement_X--;
			if(currentPlacement_Y%2 != 0)currentPlacement_Y--;
			
			//Code will look for suitable location for the room this many times
			int retry = roomRetryCounter;
			
			//Check every tile the room will occupy
			//Do not place a room if it's overlapping with Walls
			for(int i = currentPlacement_X; i <= currentPlacement_X+currentRoom_X+2; i++)
			{
				for(int j = currentPlacement_Y; j <= currentPlacement_Y+currentRoom_Y+2; j++)
				{
					switch (_MapArray[i,j].type)
					{
						case "Path":
					    case "Wall":
					        if(retry > 0)
					        {
					            retry--;
					            currentPlacement_X = Random.Range(1, mapSize_X - (currentRoom_X+2));
					            currentPlacement_Y = Random.Range(1, mapSize_Y - (currentRoom_Y+2));
					            if(currentPlacement_X%2 != 0)currentPlacement_X--;
					            if(currentPlacement_Y%2 != 0)currentPlacement_Y--;
					            i = currentPlacement_X;
					            j = currentPlacement_Y;
					        }
					            //Ran out of retries for placing a room. Giving up on this room!
					        else
					        {
					            //Breaking the for loops.
					            i = currentPlacement_X+currentRoom_X+2;
					            j = currentPlacement_Y+currentRoom_Y+2;	
					        }
					        break;
					}
				}
			}
			
			//We didn't run out of retries. Place the room!
			if(retry != 0)
			{
				//Assign Cells
				for(int i = currentPlacement_X; i < currentPlacement_X+currentRoom_X+2; i++)
				{
					for(int j = currentPlacement_Y; j < currentPlacement_Y+currentRoom_Y+2; j++)
					{
						if(i == currentPlacement_X 
							|| i == currentPlacement_X+currentRoom_X+1
							|| j == currentPlacement_Y
							|| j == currentPlacement_Y+currentRoom_Y+1)
						{
							_MapArray[i,j].SetCellType("Wall"); //Perimeter
						}
						else 
						{
							_MapArray[i,j].SetCellType("Path"); //Inside
						}
						
						DrawCellOnVisualMap(i,j);
						
					}
				}
				CreateDoors(currentPlacement_X,
							currentPlacement_Y,
							currentPlacement_X + currentRoom_X+1,
							currentPlacement_Y + currentRoom_Y+1);
			}
			
			_roomsCreated++;
			yield return null; //Shows a single room after it's placed.

		} while (_roomsCreated < roomFrequency);
		if(toggleDebugPrint) Debug.Log("Created rooms.");
		
		yield return null; //Previously: return true;
	}
	
	IEnumerator CreateMazeRoutine()
	{
		if(toggleDebugPrint) Debug.Log("Creating maze.");
		
		//Create lists for tracking maze progress.
		List<Cell> listOfMazeACells = new List<Cell>();
		List<Cell> listOfMazeBCells = new List<Cell>();
		
		//Set every cell on odd numbered x and y coordinates as a B type cell.
		for(int i = 1; i < mapSize_X; i+=2)
		{
			for(int j = 1; j < mapSize_Y; j+=2)
			{
				Cell tempCell = _MapArray[i,j];
				if(tempCell.type == "Abyss") 
				{
					tempCell.SetCellType("Maze_B");
					listOfMazeBCells.Add(tempCell);
				}
			}
		}
		
		//Get a B type cell and turn it into A. This is your first cell.
		int r = Random.Range(0, listOfMazeBCells.Count);
		Cell firstCell = listOfMazeBCells[r];
		listOfMazeBCells.RemoveAt(r);
		firstCell.SetCellType("Maze_A");
		listOfMazeACells.Add(firstCell);
		
		int currentCell_X = firstCell.x;
		int currentCell_Y = firstCell.y;
		
		//Traverse until all B cells are marked as A.
		while( GetCellCountOfType("Maze_B") > 0)
		{
			//routineClock++;
			//Pick a random direction from your current cell.
			DirectionManager.Direction currentDirection = _MapArray[currentCell_X,currentCell_Y].dirMan.GetNextDirection();
			
			if(currentDirection == DirectionManager.Direction.NONE)
			{
				yield return null; //Shows whenever one arm of a maze is completed.
			}
			
			//Try to find a random cell you turned into A already and see if that one has any directions available.
			while(currentDirection == DirectionManager.Direction.NONE)
			{			
				r = Random.Range(0, listOfMazeACells.Count);
				Cell randomCell = listOfMazeACells[r];			
				currentDirection = listOfMazeACells[r].dirMan.GetNextDirection();
				if(currentDirection == DirectionManager.Direction.NONE)
				{
					listOfMazeACells.RemoveAt(r);
				}
				currentCell_X = randomCell.x;
				currentCell_Y = randomCell.y;
			}
			
			int targetCell_X = currentCell_X;
			int targetCell_Y = currentCell_Y;
			
			switch(currentDirection)
			{
			case DirectionManager.Direction.North:
				targetCell_Y = currentCell_Y + 2;
				break;
			case DirectionManager.Direction.East:
				targetCell_X = currentCell_X + 2;
				break;
			case DirectionManager.Direction.West:
				targetCell_X = currentCell_X - 2;
				break;
			case DirectionManager.Direction.South:
				targetCell_Y = currentCell_Y - 2;
				break;
			}
			
			//	1 - if the cell on target destination is NOT in bounds of map, pick next destination.
			//  2 - if the cell on that destination is a violated cell, pick next destination.
            //  3 - 1 and 2 are false, which means there exists an available NONViolated cell to lay a path towards.
		    if (targetCell_X < 1 || targetCell_Y < 1 ||
		        targetCell_X >= mapSize_X - 1 || targetCell_Y >= mapSize_Y - 1
		        || _MapArray[targetCell_X, targetCell_Y].type != "Maze_B") continue;
			
		    int cellInMid_X = currentCell_X;
		    int cellInMid_Y = currentCell_Y;
		    if(targetCell_X > currentCell_X) cellInMid_X++;
		    if(targetCell_X < currentCell_X) cellInMid_X--;
		    if(targetCell_Y > currentCell_Y) cellInMid_Y++;
		    if(targetCell_Y < currentCell_Y) cellInMid_Y--; 
		    _MapArray[cellInMid_X, cellInMid_Y].SetCellType("Path");
				
		    //Make target the current cell.
		    Cell targetCell = _MapArray[targetCell_X,targetCell_Y];
		    targetCell.SetCellType("Maze_A");
				
		    listOfMazeBCells.RemoveAll(temp => temp.type == "Maze_A");
				
		    listOfMazeACells.Add(targetCell);
				
		    currentCell_X = targetCell_X;
		    currentCell_Y = targetCell_Y;
			
			DrawCellOnVisualMap(currentCell_X, currentCell_Y);
			
		}
		
		yield return StartCoroutine(SetAllCellsOfTypeAToBRoutine("Maze_A", "Path"));
		if(toggleDebugPrint) Debug.Log("Created maze.");
	}
	
	IEnumerator SetAllCellsOfTypeAToBRoutine(string A, string B)
	{
		for(int i = 0; i < mapSize_X; i++)
		{
			for(int j = 0; j < mapSize_Y; j++)
			{
				if(_MapArray[i,j].type == A) 
				{
					_MapArray[i,j].SetCellType(B);
					DrawCellOnVisualMap(i,j);
				}
			}
		}
		yield return null; //Shows whenever there's a mapwide replacement.
	}
	
	IEnumerator CloseDeadEndCellsRoutine()
	{
		List<Cell> listOfDeadEndCells = GetListOfDeadEndCells();
		while(listOfDeadEndCells.Count > 0)
		{
			foreach(Cell deadEndCell in listOfDeadEndCells)
			{
				deadEndCell.SetCellType("Wall");
				DrawCellOnVisualMap(deadEndCell.x, deadEndCell.y);
			}
			listOfDeadEndCells = GetListOfDeadEndCells();
			yield return new WaitForSeconds(0.1f); //Shows when a collection of deadEndCells is taken care of.
		}
		yield return null;
	}
	
	IEnumerator ReduceUCorridorsRoutine(int cycles)
	{
		if(cycles == 0) yield break; //return;
		List<Cell> listOfUCorridorCells = GetListOfUCorridorCells();
		for(int i = 0; i < listOfUCorridorCells.Count; i++)
		{
			Cell c = listOfUCorridorCells[i];

			//Does it have 7 paths around it?
			bool fixable = true;
			int pathCount = 0;
			int wallCellX = 0;
			int wallCellY = 0;
			for(int ii = c.x-1; ii <= c.x+1; ii++)
			{
				for(int jj = c.y-1; jj <= c.y+1; jj++)
				{
					if( ii < 0 || jj < 0 || ii > mapSize_X-1 || jj > mapSize_Y-1 || (ii == c.x && jj == c.y)) continue;
					if(_MapArray[ii,jj].type == "Path") pathCount++;
					else if(_MapArray[ii,jj].type == "Wall")
					{
						wallCellX = ii;
						wallCellY = jj;
					}
				}
			}
			
			//Do the corner paths of U shape have paths connected to them? We don't want to mess a multi connected corridor.
			//If there are multiple connections to the corner pieces of U, the fixable = false;
			if(pathCount == 7)
			{
				//According to its orientation, check the corner of U.
				//Case for wallCell under center cell.
				if(
					(wallCellX == c.x && wallCellY < c.y) &&
				    (!_MapArray[c.x-2, c.y+1].type.Equals("Wall") ||
				     !_MapArray[c.x-1, c.y+2].type.Equals("Wall") ||
				     !_MapArray[c.x+1, c.y+2].type.Equals("Wall") ||
				     !_MapArray[c.x+2, c.y+1].type.Equals("Wall"))
				   )
				{
					fixable = false;	
				}
				//Case for wallCell to the left of center cell.
				else if(
					(wallCellX < c.x && wallCellY == c.y) &&
				    (!_MapArray[c.x+2, c.y-1].type.Equals("Wall") ||
				     !_MapArray[c.x+1, c.y-2].type.Equals("Wall") ||
				     !_MapArray[c.x+1, c.y+2].type.Equals("Wall") ||
				     !_MapArray[c.x+2, c.y+1].type.Equals("Wall"))
				  )
				{
					fixable = false;	
				}
				//Case for wallCell above center cell.
				else if(
					(wallCellX == c.x && wallCellY > c.y) &&
				    (!_MapArray[c.x-2, c.y-1].type.Equals("Wall") ||
				     !_MapArray[c.x-1, c.y-2].type.Equals("Wall") ||
				     !_MapArray[c.x+1, c.y-2].type.Equals("Wall") ||
				     !_MapArray[c.x+2, c.y-1].type.Equals("Wall"))
				  )
				{
					fixable = false;	
				}
				//Case for wallCell to the right of center cell.
				else if(
					(wallCellX > c.x && wallCellY == c.y) &&
				    (!_MapArray[c.x-2, c.y-1].type.Equals("Wall") ||
				     !_MapArray[c.x-1, c.y-2].type.Equals("Wall") ||
				     !_MapArray[c.x-1, c.y+2].type.Equals("Wall") ||
				     !_MapArray[c.x-2, c.y+1].type.Equals("Wall"))
				  )
				{
					fixable = false;	
				}
			}
			else fixable = false;
			
			if(fixable)
			{
				if(wallCellX == c.x && wallCellY < c.y)
				{
					//Turn part of U into wall.
					_MapArray[c.x-1, c.y].SetCellType(    "Wall" ); //West
					_MapArray[c.x-1, c.y+1].SetCellType(  "Wall" ); //Northwest
					_MapArray[c.x, c.y+1].SetCellType(    "Wall" ); //North
					_MapArray[c.x+1, c.y+1].SetCellType(  "Wall" ); //Northeast
					_MapArray[c.x+1, c.y].SetCellType(    "Wall" ); //East
					//mapArray[c.x+1, c.y-1].SetCell(  "Wall" ); //Southeast
					_MapArray[c.x, c.y-1].SetCellType(    "Path" ); //South
					//mapArray[c.x-1, c.y-1].SetCell(  "Wall" ); //Southwest
				}
				else if(wallCellX < c.x && wallCellY == c.y)
				{
					//Turn part of U into wall.
					_MapArray[c.x-1, c.y].SetCellType(    "Path" ); //West
					//mapArray[c.x-1, c.y+1].SetCell(  "Wall" ); //Northwest
					_MapArray[c.x, c.y+1].SetCellType(    "Wall" ); //North
					_MapArray[c.x+1, c.y+1].SetCellType(  "Wall" ); //Northeast
					_MapArray[c.x+1, c.y].SetCellType(    "Wall" ); //East
					_MapArray[c.x+1, c.y-1].SetCellType(  "Wall" ); //Southeast
					_MapArray[c.x, c.y-1].SetCellType(    "Wall" ); //South
					//mapArray[c.x-1, c.y-1].SetCell(  "Wall" ); //Southwest
				}
				else if(wallCellX == c.x && wallCellY > c.y)
				{
					//Turn part of U into wall.
					_MapArray[c.x-1, c.y].SetCellType(    "Wall" ); //West
					//mapArray[c.x-1, c.y+1].SetCell(  "Wall" ); //Northwest
					_MapArray[c.x, c.y+1].SetCellType(    "Path" ); //North
					//mapArray[c.x+1, c.y+1].SetCell(  "Wall" ); //Northeast
					_MapArray[c.x+1, c.y].SetCellType(    "Wall" ); //East
					_MapArray[c.x+1, c.y-1].SetCellType(  "Wall" ); //Southeast
					_MapArray[c.x, c.y-1].SetCellType(    "Wall" ); //South
					_MapArray[c.x-1, c.y-1].SetCellType(  "Wall" ); //Southwest
				}
				else if(wallCellX > c.x && wallCellY == c.y)
				{
					//Turn part of U into wall.
					_MapArray[c.x-1, c.y].SetCellType(    "Wall" ); //West
					_MapArray[c.x-1, c.y+1].SetCellType(  "Wall" ); //Northwest
					_MapArray[c.x, c.y+1].SetCellType(    "Wall" ); //North
					//mapArray[c.x+1, c.y+1].SetCell(  "Wall" ); //Northeast
					_MapArray[c.x+1, c.y].SetCellType(    "Path" ); //East
					//mapArray[c.x+1, c.y-1].SetCell(  "Wall" ); //Southeast
					_MapArray[c.x, c.y-1].SetCellType(    "Wall" ); //South
					_MapArray[c.x-1, c.y-1].SetCellType(  "Wall" ); //Southwest
				}
				DrawCellOnVisualMap(c.x+1, c.y+1);
				DrawCellOnVisualMap(c.x+1, c.y);
				DrawCellOnVisualMap(c.x+1, c.y-1);
				DrawCellOnVisualMap(c.x, c.y+1);
				DrawCellOnVisualMap(c.x, c.y);
				DrawCellOnVisualMap(c.x, c.y-1);
				DrawCellOnVisualMap(c.x-1, c.y+1);
				DrawCellOnVisualMap(c.x-1, c.y);
				DrawCellOnVisualMap(c.x-1, c.y-1);
				yield return new WaitForSeconds(0.2f); //Shows a single fixed U Corridor.
			}
		}
		
		yield return StartCoroutine(ReduceUCorridorsRoutine(cycles-1));	
	}
	
	protected void DrawCellOnVisualMap(int i, int j)
	{
		Destroy(_VisualMapArray[i,j]);
		
		//Create cell object and script
		GameObject cellGO =  PlaceCellOn(i,j, _ProD_Data.GetPrefabOf(_MapArray[i,j].type));
		if(_ProD_Data.z == ReplacementType.Textures) 
		{
			cellGO.renderer.material.mainTexture = _ProD_Data.GetTextureOf(_MapArray[i,j].type);
		}
		if(cellGO == null)
		{
			Debug.LogError("Could not instantiate prefab for type " + _MapArray[i,j].type + ".");
			return;	
		}
		
		//Get reference to CellManager on prefab
		CellManager tempCellMan = cellGO.GetComponent<CellManager>();
		if(!tempCellMan) tempCellMan = cellGO.AddComponent<CellManager>();
		
		//Get temporary reference to cell for setting values
		Cell tempCell = mapArray[i,j];
		if(tempCell == null) Debug.LogError("No Cell reference in mapArray @ " +i+ ", " +j + ".");
		
		//Connect GameObject's CellManager with Cell in mapArray
		tempCellMan.cell = tempCell;
		
		//Save references of cellGO
		tempCell.myGameObject = cellGO;
		_PrefabMapArray[i,j] = cellGO;
		
		//Cell's object's name
		cellGO.name = "Cell_" + i + "X_" + j + "Y_" + tempCell.type;
		//Cell's object's parent
		cellGO.transform.parent = _MapGOTr;
		//Cell's location in game space
		cellGO.transform.position = new Vector3(inGameLoc_x + (cellGO.transform.localScale.x*i), cellGO.transform.position.y, inGameLoc_y + (cellGO.transform.localScale.z*j));	
		
		//Arrange the orientation of the 3D prefab
		//Hi! This is a paid version only feature. Please buy the paid version of ProD in the Unity Asset Store to access it.
		
		_VisualMapArray[i,j] = cellGO;
	}
	
	void RedrawAllCells()
	{
		for(int i = 0; i < mapSize_X; i++)
		{
			for(int j = 0; j < mapSize_Y; j++)
			{
				if(_MapArray[i,j] != null && !_MapArray[i,j].type.Equals("")) DrawCellOnVisualMap(i,j);		
			}
		}
	}
}

