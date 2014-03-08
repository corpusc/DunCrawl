using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour 
{
	public bool followPlayer = true;
	
	#region 2-D
	public float screenResolution = 1024;
	public float screenSizeMultiplier = 1;
	#endregion
	
	#region 3-D
	public float camera_Y = 16;
	public float camera_farClipPane = 30;
	public float camera_fieldOfView = 70;
	#endregion
	
	void Awake()
	{
		SetCameraToPixelPerfect(screenSizeMultiplier);	
	}
	
	//This is used for 3D prefabs.
	public void AdjustCameraForPerspectiveCoverage()
	{
		camera.isOrthoGraphic = false;
		camera.transform.position = new Vector3(camera.transform.position.x, camera_Y, camera.transform.position.z);
		camera.farClipPlane = camera_farClipPane;
		camera.fieldOfView = camera_fieldOfView;
	}
	
	public void SetCameraToPixelPerfect()
	{
		SetCameraToPixelPerfect(screenSizeMultiplier);
	}
	public void SetCameraToPixelPerfect(float cameraSize)
	{
		camera.orthographicSize = (screenResolution/2) * cameraSize;	
	}
	
	//Place camera on the player without changing height of camera from map.
	public void SetCameraOnPlayer(Vector3 newPosition)
	{
		if(followPlayer)
		{	
			newPosition = new Vector3(newPosition.x, transform.position.y, newPosition.z);
			SetCameraOn(newPosition);
		}
	}
	private void SetCameraOn(Vector3 newPosition)
	{
		transform.position = newPosition;	
	}
}
