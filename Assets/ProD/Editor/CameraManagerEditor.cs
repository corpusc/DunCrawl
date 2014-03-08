using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CameraManager))]

class CameraManagerEditor : Editor 
{	
	CameraManager _MyCameraManager;
	
	void OnEnable()
	{
		_MyCameraManager = target as CameraManager;
	}
	
	public override void OnInspectorGUI() 
	{
		if(GUILayout.Button("Adjust Camera Size"))
		{
			_MyCameraManager.SetCameraToPixelPerfect();	
		}
		DrawDefaultInspector();
	}
}