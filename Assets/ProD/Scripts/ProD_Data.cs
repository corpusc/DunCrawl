using UnityEngine;
using System.Collections;
//using UnityEditor;
using System;

public enum ReplacementType {Textures, Prefabs, None}

[Serializable]
public class ProD_Data : MonoBehaviour  
{
	public int x = 0;
	public CellTypeParameters[] y = new CellTypeParameters[0];
	public ReplacementType z = ReplacementType.Textures;
	public GameObject a;
	
	public void Save(int new_x, CellTypeParameters[] new_y, ReplacementType new_z, GameObject new_a)
	{
		x = new_x;
		y = new_y;
		z = new_z;
		a = new_a;
		//WARNING! Next line gives you a warning on build since UnityEditor can only be used in scripts that reside in Editor folder.
		//I apologize for this problem. Until I find a solution, please gracefully uncomment the next line and build like that.
		//Thank you for your kind understanding.
		UnityEditor.EditorUtility.SetDirty(this.gameObject);
	}
	
	public int Load_x()
	{
		return x;
	}
	
	public CellTypeParameters[] Load_y()
	{
		return y;
	}
	
	public ReplacementType Load_z()
	{
		return z;
	}
	
	public GameObject Load_a()
	{
		return a;
	}
	
	public Color GetColorOf(string cellTypeName)
	{
		foreach(CellTypeParameters ctp in y)
		{
			if(ctp.name.Equals(cellTypeName)) return ctp.color;	
		}
		//Debug.LogWarning("Couldn't find the " + cellTypeName + " cell type you were looking for in " + this);
		return Color.black;
	}
	
	public GameObject GetPrefabOf(string cellTypeName)
	{
		if(z == ReplacementType.Prefabs)
		{
			//Hi! This is a paid version only feature. Please buy the paid version of ProD in the Unity Asset Store to access it.
		}
		else if(z == ReplacementType.Textures)
		{
			return a;
		}
		Debug.LogError("Did you set a Replacement Type in ProD window? You may open the ProD Window in Windows tab.");
		return null;
	}
	
	public Texture2D GetTextureOf(string cellTypeName)
	{
		if(z == ReplacementType.Textures)
		{
			foreach(CellTypeParameters ctp in y)
			{
				if(ctp.name.Equals(cellTypeName)) return ctp.texture;	
			}
			Debug.LogError("Cell Type " + cellTypeName + " does not have a texture set for it. You may open the ProD Window in Windows tab to set Textures.");
			return null;
		}
		Debug.LogError("Something went wrong. Did you set replacementType in ProD window?");
		return null;
	}
	
}
