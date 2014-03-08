using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;
using System;

[Serializable]
public class ProD_Window : EditorWindow 
{
	
	public int numOfCellTypes;
	private int storedNumOfCellTypes;
	public CellTypeParameters[] arrOfCellTypeParameters;
	static Texture2D windowIcon;
	public ReplacementType replacementType;
	public GameObject texturePrefab;
	
	Vector2 scrollPos = new Vector2(0f,0f);
	PropertyInfo cachedTitleContent;
	
	void OnEnable()    
	{
		LoadData();
		windowIcon = Resources.Load("PNG_ProDIcon") as Texture2D;
        if (cachedTitleContent == null)
        {
            cachedTitleContent = typeof(EditorWindow).GetProperty
				("cachedTitleContent", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
        }
        if (cachedTitleContent != null)
        {
            GUIContent titleContent = cachedTitleContent.GetValue(this, null) as GUIContent;
            if (titleContent != null)
            {
				//Icon next to name of your window;
                titleContent.image = windowIcon;
				//Name of your window:
                titleContent.text = "ProD";
            }
        }
	}
	
	//This is how you declare your window.
	[MenuItem("Window/ProD")]
	static void ShowWindow()
	{
		EditorWindow.GetWindow(typeof(ProD_Window));
	}
	
	void OnDisable()
	{
		//SaveData();	
	}
	void UpdateArrayValues()
	{
		//Setting the first values in case the array is null at start
		if(arrOfCellTypeParameters == null)
		{
			arrOfCellTypeParameters = new CellTypeParameters[1];
			for (int i = 0; i < arrOfCellTypeParameters.Length; i++) 
			{
				arrOfCellTypeParameters[i] = new CellTypeParameters();
				arrOfCellTypeParameters[i].index = i;
				arrOfCellTypeParameters[i].color = Color.white;
				arrOfCellTypeParameters[i].prefab = null;
				arrOfCellTypeParameters[i].texture = null;
			}
			numOfCellTypes = arrOfCellTypeParameters.Length;
			storedNumOfCellTypes = numOfCellTypes;
		}
		//Resetting the values again in case array size changes
		else if(numOfCellTypes != storedNumOfCellTypes)
		{
			CellTypeParameters[] surrogate = new CellTypeParameters[numOfCellTypes]; 
			if(numOfCellTypes < storedNumOfCellTypes)
			{
				for (int i = 0; i < surrogate.Length; i++) 
				{
					surrogate[i] = arrOfCellTypeParameters[i];
					surrogate[i].index = i;
				}
			}
			else if(numOfCellTypes > storedNumOfCellTypes)
			{
				for (int i = 0; i < surrogate.Length; i++) 
				{
					if(i > storedNumOfCellTypes-1)
					{
						surrogate[i] = new CellTypeParameters();
						surrogate[i].color = Color.white;
						surrogate[i].prefab = null;
						surrogate[i].texture = null;
					}
					else surrogate[i] = arrOfCellTypeParameters[i];
					surrogate[i].index = i;
				}
			}
			arrOfCellTypeParameters = surrogate;
			numOfCellTypes = arrOfCellTypeParameters.Length;
			storedNumOfCellTypes = numOfCellTypes;
		}	
	}
	
	void OnGUI ()
	{
		//Putting the icon next to window name in tab:
		if (cachedTitleContent != null)
        {
            GUIContent titleContent = cachedTitleContent.GetValue(this, null) as GUIContent;
            if (titleContent != null) titleContent.image = windowIcon;
        }
		
		EditorGUILayout.BeginHorizontal();
		GUI.color = Color.green;
		if(GUILayout.Button("SAVE",GUILayout.Width(76*3/2))) SaveData();
		GUI.color = Color.red;
		if(GUILayout.Button("LOAD",GUILayout.Width(76f*3/2f))) LoadData();
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		GUI.color = Color.white;
		GUILayout.TextArea("Warning: There's no autosave! Always use these buttons to save/load your data!", GUILayout.Width(76*3f));
		EditorGUILayout.EndHorizontal();
		
		//This value sets the size of the array
		EditorGUILayout.BeginHorizontal();
		GUI.color = Color.white;
		if(replacementType != ReplacementType.Prefabs)
		{
			GUILayout.Label("Size", GUILayout.Width(96f));
        	numOfCellTypes = EditorGUILayout.IntField(numOfCellTypes, GUILayout.Width(20f));
        	numOfCellTypes = Mathf.Clamp(numOfCellTypes, 1, int.MaxValue);
		}
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Replace using", GUILayout.Width(96f));
		replacementType = (ReplacementType) EditorGUILayout.EnumPopup(replacementType, GUILayout.Width(76f));
		EditorGUILayout.EndHorizontal();
		
		if(replacementType == ReplacementType.Textures)
		{
			EditorGUILayout.BeginHorizontal();
			//GUILayout.Space(18);
			GUILayout.Label("Texture Prefab", GUILayout.Width(96f));
			texturePrefab = EditorGUILayout.ObjectField(texturePrefab,typeof(GameObject), false, GUILayout.Width(76*2f)) as GameObject;
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			GUI.color = Color.white;
			GUILayout.TextArea("Reminder: Don't forget to assign a generic prefab for your textures to be displayed on!", GUILayout.Width(76*3f));
			EditorGUILayout.EndHorizontal();
		}
		//Checking and modifying array values in case values changed or not initialized
		UpdateArrayValues();
		
		//Display the array of CellTypeParameters
		GUI.color = Color.white;
		GUILayout.Space(9f);
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width (292f), GUILayout.Height (220f));
		foreach( CellTypeParameters ctp in arrOfCellTypeParameters )
		{
			if(replacementType == ReplacementType.Prefabs)
			{
				//Hi! This is a paid version only feature. Please buy the paid version of ProD in the Unity Asset Store to access it.
				GUILayout.TextArea("Hi! This is a paid version only feature. Please buy the paid version of ProD in the Unity Asset Store to access it.", GUILayout.Width(76*3f));
				break;
			}
			
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(18);
			GUILayout.Label("Cell Name " + ctp.index.ToString(), GUILayout.Width(76f));
	        ctp.name = EditorGUILayout.TextField(ctp.name, GUILayout.Width(96f));
			ctp.color = EditorGUILayout.ColorField(ctp.color, GUILayout.Width(56f));
			EditorGUILayout.EndHorizontal();
			
			if(replacementType == ReplacementType.Textures)
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Space(18);
				GUILayout.Label("Cell Texture", GUILayout.Width(76f));
				ctp.texture = EditorGUILayout.ObjectField("", ctp.texture,typeof(Texture2D), false) as Texture2D;
				EditorGUILayout.EndHorizontal();
			}
			GUILayout.Space(9f);
		}
		EditorGUILayout.EndScrollView();
		
	}
	
	public void SaveData()
	{
		//Debug.Log ("Saving!");
		GameObject saveGameObject = Resources.Load("ProD_Data") as GameObject;
		if(saveGameObject == null) 
		{
			//Debug.LogError("Can't find ProD_Data in resources!");
			return;
		}
		ProD_Data saveProD_Data = saveGameObject.GetComponent<ProD_Data>();
		if(saveProD_Data == null) 
		{
			//Debug.LogError("Can't find ProD_Data.cs on ProD_Data prefab in resources!");
			return;
		}
		saveProD_Data.Save(numOfCellTypes, arrOfCellTypeParameters, replacementType, texturePrefab);
		//Debug.Log ("SAVED!");
	}
	
	public void LoadData()
	{
		//Debug.Log ("Loading!");
		GameObject loadGameObject = Resources.Load("ProD_Data") as GameObject;
		if(loadGameObject == null) 
		{
			//Debug.LogError("Can't find ProD_Data in resources!");
			return;
		}
		ProD_Data loadProD_Data = loadGameObject.GetComponent<ProD_Data>();
		if(loadProD_Data == null) 
		{
			//Debug.LogError("Can't find ProD_Data.cs on ProD_Data prefab in resources!");
			return;	
		}
		numOfCellTypes = loadProD_Data.Load_x();
		arrOfCellTypeParameters = loadProD_Data.Load_y();
		replacementType = loadProD_Data.Load_z();
		texturePrefab = loadProD_Data.Load_a();
		storedNumOfCellTypes = numOfCellTypes;
		//Debug.Log ("LOADED!");
	}
}
