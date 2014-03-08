using UnityEngine;
using System;


//This class is used by ProD_Data and ProD_Window as a Data Structure.
//This reduces the method calls for saving variables for aforementioned scripts.
[Serializable]
public class CellTypeParameters
{
	public int index;
	public string name;
	public Color color;
	public GameObject prefab;
	public Texture2D texture;
}
