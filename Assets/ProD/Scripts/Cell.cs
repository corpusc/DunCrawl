using UnityEngine;

//You should note that this script doesn't inherit from Monobehaviour.
//We do this so we can generate cells on the go without having to create GameObjects for them first.
public class Cell
{
	//Setting cell's x and y coordinates.
	private int _x;
	public int x { get{ return _x; } }
	private int _y;
	public int y { get{ return _y; } }
	public void SetCellAddress(int x_location, int y_location)
	{
		_x = x_location;
		_y = y_location;
	}
	
	//Setting cell's type.
	private string _type = null;
	public string type{ get { return _type; } }
	public void SetCellType(string typeName)
	{
		_type = typeName;
		//Whenever you set a new cell type, the cell will lose its connection to its GameObject.
		//This acts as a flag that will be questioned when refreshing the GameObjects in the scene.
		_MyGameObject = null; 
		
	}
	
	//This is set if we are replacing the map with prefabs.
	private GameObject _MyGameObject;
	public GameObject myGameObject
	{ 
		get { return _MyGameObject; }
		set { _MyGameObject = value; }
	}
	
	private Mesh _mesh;
	private Color[] _arrOfColorVertices;
	public Material material;
	public DirectionManager dirMan;
	
	public Cell()
	{
		dirMan = new DirectionManager();
		//Set cell's material.
		if(!material) material = Resources.Load("MAT_TopDownTile") as Material;
		if(!material) Debug.LogError("Couldn't find MAT_Cell in Resources!");
	}
}
