using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

    public static CameraScript instance;
    public Vector3 roomCoordinate;
    public float moveRadius;
    private float maxOrtho;

	void Awake ()
    {
        instance = this;
        maxOrtho = Camera.main.orthographicSize;
	}
	
	void Update ()
    {
	    if (Camera.main.orthographicSize < maxOrtho)
        {
            Camera.main.orthographicSize += (maxOrtho - Camera.main.orthographicSize) * 3f * Game.dt;
            Camera.main.orthographicSize += Game.dt * 0.3f;
            if (Camera.main.orthographicSize > maxOrtho)
            {
                Camera.main.orthographicSize = maxOrtho;
            }
        }
	}

    public static void zoom(float val = 0.05f)
    {
        Camera.main.orthographicSize -= val;
    }
}
