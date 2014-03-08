//This is the parent of all scripts with Generator in their name.
//You may use this class and its methods for making your own map generation class.
//We tried to name all the methods as plainly as possible for your understanding.

using UnityEngine;
using System.Collections.Generic;

public class BaseGenerator : MonoBehaviour 
{
	public bool toggleDebugPrint = false;
	public GameObject mapGO;
	protected Transform _MapGOTr;
	
	protected bool _EnableEditorScript = false;
	public bool EnableEditorScript
    {
        get { return _EnableEditorScript; }
	}   
	
	public bool resizeCamera = true;
	public bool placePlayer = false;
	
	protected Cell[,] _MapArray;
	public Cell[,] mapArray
    {
        get { return _MapArray; }
	}
	
	protected GameObject[,] _PrefabMapArray;
	public GameObject[,] prefabMapArray
    {
        get { return _PrefabMapArray; }
	}
	
	protected string[,] _MapArrayInString;
	public string[,] mapArrayInString
    {
        get { return _MapArrayInString; }
	}
	
	protected int maxMapSize = 200;
	
	public int mapSize_X = 49;
	public int mapSize_Y = 49;
	
	public float inGameLoc_x = 0;
	public float inGameLoc_y = 0;
	
	public bool convertUnreachableWallsToAbyss = false;
		
	protected ProD_Data _ProD_Data;
	
	void Awake() 
	{ 
		_EnableEditorScript = true;
		SetReferences();
	}
	
	#region MAP CREATION METHODS
	protected void SetReferences()
	{
		if(toggleDebugPrint) Debug.Log ("Setting references!");
		GameObject _ProD_DataGO = Resources.Load("ProD_Data") as GameObject;
		_ProD_Data = _ProD_DataGO.GetComponent<ProD_Data>();
		if(mapGO == null)
		{
			Debug.LogError("You need a Map GameObject with two children: PostBuild and VisualBuild in your scene!");
			Debug.LogError("ABORTING!");
			Destroy(this.gameObject);
		} else
		{
			_MapGOTr = mapGO.transform;
		}
		if(toggleDebugPrint) Debug.Log ("References set!");
	}
	
	public virtual Cell[,] Generate()
	{
		//Override this in any generation scripts you make.
		return _MapArray;
	}
	
	protected void PlacePlayerOnMap()
	{
		if(!placePlayer) return;
		GameObject playerGO = (GameObject)Instantiate(Resources.Load("Player"));
		PlayerMovement playerMovement = playerGO.GetComponent<PlayerMovement>();
		playerMovement.SetupPlayer(_MapArray, prefabMapArray);
	}
	
	public void ResizeCameraToMap()
	{
		CameraManager c = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraManager>();
		if(c && _ProD_Data.z == ReplacementType.Prefabs) 
		{
			c.AdjustCameraForPerspectiveCoverage();
		}
	}
	
	public void AdjustMapViewCamera() {
		if(_ProD_Data.z == ReplacementType.Prefabs) {
			GameObject mapCamGO = GameObject.FindGameObjectWithTag("MapCamera");
			CameraDragAndZoom cDrag = mapCamGO.GetComponent<CameraDragAndZoom>();	
			Camera cCam = mapCamGO.GetComponent<Camera>();
			
			cCam.orthographicSize = 60;
			
			cDrag.dragSpeed = -1;
			cDrag.zoomSpeed = 5;
		}		
	}
	
	protected virtual void DeletePreviousMap()
	{
		if(toggleDebugPrint) Debug.Log("Deleting previous map.");
		_MapArray = new Cell[0,0];
		_PrefabMapArray = new GameObject[0,0];
		DestroyChildrenOf(_MapGOTr);
		if(toggleDebugPrint) Debug.Log("Deleted previous map.");
	}
	
	protected virtual void DestroyPlayer()
	{
		if(toggleDebugPrint) Debug.Log("Deleting player.");
		GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
		if(playerGO) 
		{
			Destroy(playerGO);
			if(toggleDebugPrint) Debug.Log("Deleting player.");
		}
		else
			if(toggleDebugPrint) Debug.Log("No player found on scene.");
	}
	
	protected void CreateNewMap()
	{
		if(toggleDebugPrint) Debug.Log("Creating a new map.");
		
		if(mapSize_X > maxMapSize || mapSize_Y > maxMapSize)
		{
			Debug.LogError("You are exceeding the maximum size of map. We put a cap on this for you not to accidentally make giant maps that take 1 hour to generate. " +
				"You may change this value by going to BaseGenerator.cs and altering the variable maxMapSize");	
		}
		
		//Checking if map size is too small or too big to calculate.
		//If you put in numbers bigger than 100 that will take a long time to create.
		if(mapSize_X < 4 || mapSize_Y < 4 || mapSize_X > maxMapSize || mapSize_Y > maxMapSize)
		{
			Debug.LogError("Values for map_X and map_Y are too small or too big! Setting map size to 31 by 31.");
			mapSize_X = 31; mapSize_Y = 31;
		}
		
	    //Since we are using a maze algorithm for creation of corridors,
		//we require all map size elements to be odd numbers.
		if(mapSize_X%2 != 1)
		{
			Debug.LogWarning("Value for map_X is even. It needs to be an odd number! Bumping up +1");
			mapSize_X++;
		}
		if(mapSize_Y%2 != 1)
		{
			Debug.LogWarning("Value for map_Y is even. It needs to be an odd number! Bumping up +1");
			mapSize_Y++;
		}
		
	    _MapArray = new Cell[mapSize_X,mapSize_Y];
		_PrefabMapArray = new GameObject[mapSize_X,mapSize_Y];
		if(toggleDebugPrint) Debug.Log("Created a new map.");
	}
	
	protected void CreateCells()
	{
		if(toggleDebugPrint) Debug.Log("Creating cells!");
		for(int i = 0; i < mapSize_X; i++)
		{
			for(int j = 0; j < mapSize_Y; j++)
			{
				//Create cell script and store it in mapArray.
				Cell cell = new Cell();
				_MapArray[i,j] = cell;
				//Set Cell's type and location
				cell.SetCellType("Abyss");
				cell.SetCellAddress(i,j);
			}
		}
		if(toggleDebugPrint) Debug.Log("Created cells!");	
	}
	public void ConvertUnreachableWallsTo(string cellType)
	{
		//Make note of all unreachable cells in a boolean array
		//If you try to set these cells as you go, you would be changing the map as you were scanning
		//This results in faulty checker pattern removal
		bool[,] tempBoolArray = new bool[mapSize_X,mapSize_Y];
		for(int i = 1; i < mapSize_X-1; i++)
		{
			for(int j = 1; j < mapSize_Y-1; j++)
			{
				//Check if cell has walls around it: For four directions:
				/*
				if(_MapArray[i,j].type.Equals("Wall")
					&& _MapArray[i+1,j].type.Equals("Wall")
					&& _MapArray[i-1,j].type.Equals("Wall")
					&& _MapArray[i,j+1].type.Equals("Wall")
					&& _MapArray[i,j-1].type.Equals("Wall")
					)
				{
					tempBoolArray[i,j] = true;
				}
				*/
				
				//Check if cell has walls around it: For all directions:
				//Remove cell if it's surrounded by walls.
				
				if(_MapArray[i,j].type.Equals("Wall")
					&& _MapArray[i+1,j+1].type.Equals("Wall")
					&& _MapArray[i+1,j].type.Equals("Wall")
					&& _MapArray[i+1,j-1].type.Equals("Wall")
					&& _MapArray[i,j+1].type.Equals("Wall")
					//&& _MapArray[i,j].type.Equals("Wall")
					&& _MapArray[i,j-1].type.Equals("Wall")
					&& _MapArray[i-1,j+1].type.Equals("Wall")
					&& _MapArray[i-1,j].type.Equals("Wall")
					&& _MapArray[i-1,j-1].type.Equals("Wall")
					)
				{
					tempBoolArray[i,j] = true;
				}
				
				//Check if cells on borders have no path around them: For all directions except out of bounds:
				//Remove cell if it can't block a path.
				//TODO: To be implemented.
			}
		}
		
		//Remove the true addresses in the tempBoolArray from _MapArray
		for(int i = 1; i < mapSize_X-1; i++)
		{
			for(int j = 1; j < mapSize_Y-1; j++)
			{
				if(tempBoolArray[i,j]) _MapArray[i,j].SetCellType(cellType);
			}	
		}		
	}
	#endregion
	
	#region SUPPORTIVE METHODS
	protected void DestroyChildrenOf(Transform parentTr)
	{
		if(parentTr.childCount > 0)
		{
			foreach (Transform childTr in parentTr)
			{
				if(childTr != parentTr) Destroy(childTr.gameObject);
			} 
		}
	}
	
	protected int GetCellCountOfType(string ct)
	{
		int cellCount = 0;
		for(int i = 0; i < mapSize_X; i++)
		{
			for(int j = 0; j < mapSize_Y; j++)
			{
				if(_MapArray[i,j].type == ct) cellCount++;	
			}
		}
		return cellCount;
	}
	
	protected List<Cell> GetCellListOfType(string ct)
	{
		List<Cell>  listOfCells = new List<Cell>();
		for(int i = 0; i < mapSize_X; i++)
		{
			for(int j = 0; j < mapSize_Y; j++)
			{
				if(_MapArray[i,j].type == ct) listOfCells.Add(_MapArray[i,j]);	
			}
		}
		return listOfCells;
	}
	
	protected virtual void SetAllCellsOfTypeAToB(string A, string B)
	{
		for(int i = 0; i < mapSize_X; i++)
		{
			for(int j = 0; j < mapSize_Y; j++)
			{
				if(_MapArray[i,j].type == A) 
					_MapArray[i,j].SetCellType(B);
			}
		}
	}
	
	//Stores the types of the cells in _MapArray as string in a string array. Useful for getting a string representation.
	protected string[,] ConvertMapArrayIntoStringMapArray()
	{
		_MapArrayInString = new string[mapSize_X,mapSize_Y];
		for(int i = 0; i < mapSize_X; i++)
		{
			for(int j = 0; j < mapSize_Y; j++)
			{
				if(_MapArray[i,j] != null)
					_MapArrayInString[i,j] = _MapArray[i,j].type.ToString();
			}
		}
		return _MapArrayInString;
	}
	protected void CreateMaze()
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
			//Pick a random direction from your current cell.
			DirectionManager.Direction currentDirection = _MapArray[currentCell_X,currentCell_Y].dirMan.GetNextDirection();
			
			//Try to find a random cell you tunred into A already and see if that one has any directions available.
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
		}
		
		SetAllCellsOfTypeAToB("Maze_A", "Path");
		if(toggleDebugPrint) Debug.Log("Created maze.");
	}
	#endregion
	
	#region CELL REPLACEMENT METHODS
	protected void ReplaceCellsWithPrefabs()
	{
		for(int i = 0; i < mapSize_X; i++)
		{
			for(int j = 0; j < mapSize_Y; j++)
			{
				ReplaceCellWithPrefabs(i,j);				
			}
		}
	}
	
	protected void ReplaceCellWithPrefabs(int i, int j)
	{
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
	}
	
	//Go through the existing game objects and compare them to the information in _MapArray.
	//If there's a non-matching game object and cell info then delete the game object and replace according to what the cell type is.
	public void DeleteAndReplaceOldPrefabs()
	{
		for(int i = 0; i < mapSize_X; i++)
		{
			for(int j = 0; j < mapSize_Y; j++)
			{
				Cell c = prefabMapArray[i,j].GetComponent<CellManager>().cell;
				if(c != null)
				{	
					if(c.myGameObject == null)
					{
						Destroy(prefabMapArray[i,j]);
						ReplaceCellWithPrefabs(i,j);
					}
				}
			}
			
		}
	}
	
	protected GameObject PlaceCellOn(int x, int y, GameObject prefab)
	{
		if(prefab == null)
		{
			Debug.LogWarning("PlaceCellOn(" + x.ToString() + ", " + y.ToString() + ", null) is being called. Null is not a valid prefab for placement.");
			return null;
		}
		Vector3 temp;
		//if(applyCellScale) temp = new Vector3(inGameLoc_x + (cellScale_X*x), 0f, inGameLoc_y + (cellScale_Y*y));
		//else 
		temp = new Vector3(inGameLoc_x + (prefab.transform.localScale.x*x), prefab.transform.position.y, inGameLoc_y + (prefab.transform.localScale.z*y));
		GameObject cellGO = (GameObject)Instantiate(prefab, temp, new Quaternion(0f,0f,0f,0f));
		//if(applyCellScale) cellGO.transform.localScale = new Vector3(cellScale_X,1f,cellScale_Y);
		SetPrefabColor(mapArray[x,y], cellGO);
		return cellGO;
	}
	
	protected void SetPrefabColor(Cell cell, GameObject cellGO)
	{
		//Set references:
		MeshFilter cellMF = cellGO.GetComponent<MeshFilter>();
		if(cellMF != null) {
			Mesh cellGO_Mesh = cellGO.GetComponent<MeshFilter>().mesh;
			Vector3[] cellGO_ArrOfVertices = cellGO_Mesh.vertices;
			Color[] cellGO_ArrOfColorVertices = new Color[cellGO_ArrOfVertices.Length];
		
			//Set cell s color. Find color information via ProD_Data in Resources.
			if(_ProD_Data)
			{
				int i = 0;
				Color c = _ProD_Data.GetColorOf(cell.type);
				while (i < cellGO_ArrOfVertices.Length) 
				{
	            	cellGO_ArrOfColorVertices[i] = c;
	           		i++;
				}
	        }
			
			//WARNING! You may only set the vertex color of a material that displays it!
			//Recommendation is that you use one of the bulilt-in particle shaders.
			//In doing so PAY ATTENTION to the color and transparency of shader.
			//If you leave everything on white, it may override the vertex colors.
			//Recommended Shader: Particles-Additive(Soft)
			if(!cellGO_Mesh) Debug.LogError("You are missing the mesh on your cell object!");
        	cellGO_Mesh.colors = cellGO_ArrOfColorVertices;
		}
	}
	#endregion
}