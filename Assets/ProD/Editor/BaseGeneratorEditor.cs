using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(BaseGenerator))]

public class BaseGeneratorEditor : Editor 
{	
	//Debug.Log("target is " + target.name);
	BaseGenerator _MyBaseGenerator;
	
	void OnEnable()
	{
		_MyBaseGenerator = target as BaseGenerator;
	}
	
	override public void  OnInspectorGUI () 
	{
		if(!_MyBaseGenerator.EnableEditorScript) 
		{
			DrawDefaultInspector();	
			return;
		}
        else
		{	
			if(GUILayout.Button("Generate " + _MyBaseGenerator.name.Replace("Generator","")))
			{
				_MyBaseGenerator.Generate();
			}
			DrawDefaultInspector();
			
			if(GUI.changed)
			{
				EditorUtility.SetDirty(target);
			}
		}
	}
}
