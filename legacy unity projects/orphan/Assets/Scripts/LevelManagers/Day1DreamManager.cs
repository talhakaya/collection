using UnityEngine;
using System.Collections;

public class Day1DreamManager : MonoBehaviour {
	
	private bool firstFrame;
	
	void Start ()
	{
		
	}
	
	void Update ()
	{
		if (!firstFrame)
		{
			firstFrame = true;
			CameraScript.changeZoom(0.0000001f);
			CameraScript.zoomInOut(0.65f, 0.05f);
		}
	}
}
