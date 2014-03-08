//PlayerMovement.cs is a simple script for creating and moving your player.
//To use it place it on the player prefab in Resources folder or any other player prefab you come up with.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour 
{
	public enum SpawnLocation{ onRandomTile, /* TIP: Other spawn locations can be placed here. */ }
	public SpawnLocation playerSpawnLocation;
	public bool allowCrossMovement = true;
	public float layer = 1f; //Player's y distance from the map.
	private Cell[,] currentMapArray;
	private GameObject[,] currentPrefabMapArray;
	private Cell currentCell;
	private CameraManager myCameraManager;
	
	void Awake()
	{
		myCameraManager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraManager>();
	}
	
	//This gets called once to place the player on the map.
	public void SetupPlayer(Cell[,] newMapArray, GameObject[,] newPrefabMapArray)
	{
		currentMapArray = newMapArray;
		currentPrefabMapArray = newPrefabMapArray;
		
		//Resize player to cell size
		float scale = newPrefabMapArray[0,0].transform.localScale.x;
		transform.localScale = new Vector3(scale,scale,scale);
		
		switch(playerSpawnLocation)
		{
			case SpawnLocation.onRandomTile:
			List<Cell> placementList = GetCellListOfType(currentMapArray, "Path");
			MoveToCell(placementList[Random.Range(0,placementList.Count-1)]);
			break;
		}
	}
	
	//Ask the necessary questions for moving to the new cell.
	//If all conditions are met move the player to that cell.
	public void MoveToCell(Cell targetCell)
	{
		bool cellIsMovableTo = false;
		
		if((targetCell.type == "Path" || targetCell.type == "Door") /* && some other conditions are met. */)
		{
			cellIsMovableTo = true;	
		}
		
		if(cellIsMovableTo)
		{
			
			//Put the player on the world location 1 units above map.
			float newPos_X = currentPrefabMapArray[targetCell.x, targetCell.y].transform.position.x;
			float newPos_Z = currentPrefabMapArray[targetCell.x, targetCell.y].transform.position.z;
			transform.position = new Vector3(newPos_X, layer, newPos_Z);
			//Set his theoretical location on the array.
			currentCell = targetCell;
			
			//Reposition camera on player.
			myCameraManager.SetCameraOnPlayer(transform.position);
		}
		
		
	}
	
	//Set movement input here
	void Update() 
	{
		//Move using CursorKeys and 2,4,6,8 on numPad
        if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Keypad8))
		{
			MoveToCell(currentMapArray[currentCell.x, currentCell.y+1]);
    	}
		else if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.Keypad2))
		{
			MoveToCell(currentMapArray[currentCell.x, currentCell.y-1]);
    	}
		if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Keypad4))
		{
			MoveToCell(currentMapArray[currentCell.x-1, currentCell.y]);
    	}
		if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Keypad6))
		{
			MoveToCell(currentMapArray[currentCell.x+1, currentCell.y]);
    	}
		
		//Move using 1,3,7,9 on numPad
		if(allowCrossMovement)
		{
			if(Input.GetKeyDown(KeyCode.Keypad9))
			{
				MoveToCell(currentMapArray[currentCell.x+1, currentCell.y+1]);
	    	}
			if(Input.GetKeyDown(KeyCode.Keypad3))
			{
				MoveToCell(currentMapArray[currentCell.x+1, currentCell.y-1]);
	    	}
			if(Input.GetKeyDown(KeyCode.Keypad7))
			{
				MoveToCell(currentMapArray[currentCell.x-1, currentCell.y+1]);
	    	}
			if(Input.GetKeyDown(KeyCode.Keypad1))
			{
				MoveToCell(currentMapArray[currentCell.x-1, currentCell.y-1]);
	    	}
		}
	}
	
	private List<Cell> GetCellListOfType(Cell[,] tempMapArray ,string ct)
	{
		int tempMapXLength = tempMapArray.GetLength(0);
		int tempMapYLength = tempMapArray.GetLength(1);
		List<Cell>  listOfCells = new List<Cell>();
		for(int i = 0; i < tempMapXLength; i++)
		{
			for(int j = 0; j < tempMapYLength; j++)
			{
				if(tempMapArray[i,j].type == ct) listOfCells.Add(tempMapArray[i,j]);	
			}
		}
		return listOfCells;
	}
		
}
