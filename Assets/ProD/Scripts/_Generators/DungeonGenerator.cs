using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DungeonGenerator : BaseGenerator 
{
	
	//This script currently tries to place roomFrequency amount of rooms.
	//The script will either get you that amount of rooms or less if he can't place that many.
	public int roomFrequency = 24;
	protected int _roomsCreated;
	protected int roomRetryCounter = 20;
	protected int doorsPerRoom = 1;
	
	//Reduction cycle of winding corridors.
	protected int reduceUCorridorCycles = 6;
	
	public int room_MinX = 3;
	public int room_MaxX = 11;
	public int room_MinY = 3;
	public int room_MaxY = 11;
	
	public int stairs;
	
	//For setting variables outside of this script.
	public void SetMapProperty(
		bool newResizeCamera,
		bool newPlacePlayer,
		int newMap_X,
		int newMap_Y,
		int newInGameLoc_x,
		int newInGameLoc_y,
		int newRoomFrequency,
		int newRoom_MinX,
		int newRoom_MinY,
		int newRoom_MaxX,
		int newRoom_MaxY,
		int newStairs
		)
	{
		resizeCamera = newResizeCamera;
		placePlayer = newPlacePlayer;
		mapSize_X = newMap_X;
		mapSize_Y = newMap_Y;
		inGameLoc_x = newInGameLoc_x;
		inGameLoc_y = newInGameLoc_y;
		roomFrequency = newRoomFrequency;
		room_MinX = newRoom_MinX;
		room_MinY = newRoom_MinY;
		room_MaxX = newRoom_MaxX;
		room_MaxY = newRoom_MaxY;
		stairs = newStairs;
	}
	
	public override Cell[,] Generate()
	{
		if(toggleDebugPrint) Debug.Log("Started on " + System.DateTime.UtcNow.ToString());
		DeletePreviousMap();
		DestroyPlayer();
		CreateNewMap();
		CreateCells();
		CreateRooms();
		CreateMaze();
		SetAllCellsOfTypeAToB("Abyss", "Wall");
		CloseDeadEndCells();
		ReduceUCorridors(reduceUCorridorCycles);
		PlaceStairs();
		if(convertUnreachableWallsToAbyss) ConvertUnreachableWallsTo("Abyss");
		ResizeCameraToMap();
		AdjustMapViewCamera();
		ReplaceCellsWithPrefabs();
		PlacePlayerOnMap();
		if(toggleDebugPrint) Debug.Log("Ended on " + System.DateTime.UtcNow.ToString());
		ConvertMapArrayIntoStringMapArray();
		return _MapArray;
	}
	
	protected override void DeletePreviousMap ()
	{
		_roomsCreated = 0;
		base.DeletePreviousMap ();
	} 
	
	protected bool CreateRooms()
	{
		if(toggleDebugPrint) Debug.Log("Creating rooms.");
		
		//Checking if we want any rooms in the map
		if(roomFrequency < 1) return true;
		
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
				//Assign cells
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
					}
				}
				CreateDoors(currentPlacement_X,
							currentPlacement_Y,
							currentPlacement_X + currentRoom_X+1,
							currentPlacement_Y + currentRoom_Y+1);
				
			}
			
			_roomsCreated++;

		} while (_roomsCreated < roomFrequency);
		if(toggleDebugPrint) Debug.Log("Created rooms.");
		
		return true;
	}
	
	protected void CreateDoors(int a_min, int b_min, int a_max, int b_max)
	{
		int doorCount = doorsPerRoom;
		//Base cases:
		if(doorCount < 1)
		{
			Debug.Log ("You chose to have no doors to any of your rooms. Forcing 1 door per room.");
			doorsPerRoom = 1;
			doorCount = doorsPerRoom;
		}
	    if(doorCount > (((a_max-a_min)*2)+((b_max-b_min)*2))) 
	    {
	        Debug.LogError("Too many doors allocated for this room. Forcing 1 door for this room.");
	        doorCount = 1;
	    }
	    if(a_min >= a_max || b_min >= b_max) 
	    {
	        Debug.LogError("Coordinates are too small or equal to one another.");
	        Debug.LogError("ABORTING GENERATION!");
	        return;
	    }

	    //Creating a list to randomly pick from for doors.
		List<Cell> listOfWallCellsAroundRoom = new List<Cell>();
		
		//Traversing the room coordinates.
		for(int i = a_min; i <= a_max; i++)
		{
			for(int j = b_min; j <= b_max; j++)
			{
				if((doorCount == 0) ||                                           //No doors are available.
                    (i == 0 || j == 0 || i == mapSize_X-1 || j == mapSize_Y-1) ||        //No doors on edges.
			        ((i%2 == 0 && j%2 == 0) || (i%2 == 1 && j%2 == 1)) ||        //For doors i must be odd when j is even and vice versa.
			        (!(i == a_min || i == a_max || j == b_min || j == b_max)) || //Forcing to traverse on the walls only.
			        (((i == a_min) && (j == b_min || j == b_max)) || ((i == a_max) && (j == b_min || j == b_max)))) {continue;} //Skip corners of the room.
			        listOfWallCellsAroundRoom.Add(_MapArray[i,j]); //If all conditions above are matched, then this is a good tile for placing a door on.
			}
		}
		while(doorCount > 0)
		{
			doorCount--;
			listOfWallCellsAroundRoom[Random.Range(0,listOfWallCellsAroundRoom.Count)].SetCellType("Door");
			
		}
	}
	
	protected void CloseDeadEndCells()
	{
		List<Cell> listOfDeadEndCells = GetListOfDeadEndCells();
		while(listOfDeadEndCells.Count > 0)
		{
			foreach(Cell deadEndCell in listOfDeadEndCells)
			{
				deadEndCell.SetCellType("Wall");;
			}
			listOfDeadEndCells = GetListOfDeadEndCells();
		}
	}
	
	//Gets list of cells that have 3 or more walls around them in NEWS directions.
	protected List<Cell> GetListOfDeadEndCells()
	{
		List<Cell>  listOfDeadEndCells = new List<Cell>();
		for(int i = 1; i < mapSize_X-1; i++)
		{
			for(int j = 1; j < mapSize_Y-1; j++)
			{
				int wallCountOnNEWS = 0;
				Cell cell = _MapArray[i,j];
				////cell.dirMan.Reset();
				if(_MapArray[i,j].type == "Path")
				{
					//Reset all directions according to walls.
					if(_MapArray[i-1,j].type == "Wall") wallCountOnNEWS++;
					else cell.dirMan.Visit(cell.dirMan.west);
					if(_MapArray[i+1,j].type == "Wall") wallCountOnNEWS++;
					else cell.dirMan.Visit(cell.dirMan.east);
					if(_MapArray[i,j+1].type == "Wall") wallCountOnNEWS++;
					else cell.dirMan.Visit(cell.dirMan.north);
					if(_MapArray[i,j-1].type == "Wall") wallCountOnNEWS++;
					else cell.dirMan.Visit(cell.dirMan.south);
					
					if(wallCountOnNEWS == 3) 
					{
						listOfDeadEndCells.Add(cell);
					}
				}
			}
		}
		return listOfDeadEndCells;
	}
	
	protected void ReduceUCorridors(int cycles)
	{
		if(cycles == 0) return;
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
			}
		}
		
		ReduceUCorridors(cycles-1);
		
	}
	
	//Find corridors that are U shaped! 
	//These are cells that are wall in center and have 7 paths around them.
	protected List<Cell> GetListOfUCorridorCells()
	{
		List<Cell>  listOfUCorridorCells = new List<Cell>();
		for(int i = 0; i < mapSize_X; i++)
		{
			for(int j = 0; j < mapSize_Y; j++)
			{
				if(_MapArray[i,j].type != "Wall") continue;
				int pathCount = 0;
				for(int ii = i-1; ii <= i+1; ii++)
				{
					for(int jj = j-1; jj <= j+1; jj++)
					{
					    if( ii < 0 || jj < 0 || ii > mapSize_X-1 || jj > mapSize_Y-1 ) continue;
					    if(_MapArray[ii,jj].type == "Path") pathCount++;
					}
				}
				if(pathCount == 7) 
				{	
					listOfUCorridorCells.Add(_MapArray[i,j]);
				}
			}
		}
		return listOfUCorridorCells;
	}
	
	protected void PlaceStairs()
	{
		if(stairs == 0) return;
		
		int stairsPlaced = 0;
		List<Cell> placementList = GetCellListOfType("Path");
		
		int infiniteLoopBreaker = 100;
		
		do{
			Cell c = placementList[Random.Range(0,placementList.Count-1)];
			int pathCellsAround = 0;
			for(int i = c.x-1; i <= c.x+1; i++)
			{
				for(int j = c.y-1; j <= c.y+1; j++)
				{
					if((!(i == c.x && j == c.y)) && (_MapArray[i,j].type == "Path"))	
					{
						pathCellsAround++;
					}
				}
			}
			if(pathCellsAround == 8) 
			{
				c.SetCellType("Stair");
				stairsPlaced++;
			}
			infiniteLoopBreaker--;
		} while (stairsPlaced != stairs && infiniteLoopBreaker > 0);
	}
	
}

