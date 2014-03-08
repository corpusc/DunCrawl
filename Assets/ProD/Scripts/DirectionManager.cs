using UnityEngine;
using System.Collections.Generic;

//Used by the MazeGenerator.cs and other Generator type scripts.
//Supports the algorithm that makes the corridors in aforementioned scripts. 
public class DirectionManager 
{
	public enum Direction{ North, East, West, South, NONE};
	
	private List<Direction> listOfUnvisitedDirections;
	public Direction north;
	public Direction east;
	public Direction west;
	public Direction south;
	private Direction none;
	
	public DirectionManager()
	{
		listOfUnvisitedDirections = new List<Direction>();
		
		north = Direction.North;	
		east = Direction.East;
		west = Direction.West;
		south = Direction.South;
		none = Direction.NONE;
		
		listOfUnvisitedDirections.Add(north);
		listOfUnvisitedDirections.Add(east);
		listOfUnvisitedDirections.Add(west);
		listOfUnvisitedDirections.Add(south);
	}
	
	//Put all directions back into list of unvisited directions
	//public void Reset() { DirectionManager(); }
	
	//Visit a direction and remove it from the list of unvisited directions
	public void Visit(Direction d)
	{
		listOfUnvisitedDirections.RemoveAll(x => x == d);
	}
	
	//Return next unvisited direction and visit it
	public Direction GetNextDirection()
	{
		//No more directions left to pick
		if(listOfUnvisitedDirections.Count == 0) return none;
		
		//Pick next random direction
		int r = Random.Range(0, listOfUnvisitedDirections.Count);
		Direction nextDirection = listOfUnvisitedDirections[r];
		Visit(nextDirection);
		return nextDirection;
	}
}
