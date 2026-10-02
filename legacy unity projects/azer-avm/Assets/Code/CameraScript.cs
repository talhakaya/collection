using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {

    public float cameraSpeed;
    public float jumpFixSpeed;

	void Start ()
    {
	
	}
	
	void Update ()
    {
        Vector3 camPos = Vector3.zero;
        int noOfPlayers = 0;
        float maxLength = 0;
        for (int i = 0; i < PlayerScript.list.Length; i++)
        {
            if (PlayerScript.list[i] != null)
            {
                noOfPlayers++;
                Vector3 playerPos = new Vector3(PlayerScript.list[i].transform.position.x, (PlayerScript.list[i].transform.position.y < -10f) ? -10f : PlayerScript.list[i].transform.position.y, PlayerScript.list[i].transform.position.z);
                camPos += playerPos;
                for (int j = i + 1; j < PlayerScript.list.Length; j++)
                {
                    if (PlayerScript.list[j] != null)
                    {
                        float len = Geometry.lengthOfVector3(PlayerScript.list[i].transform.position - PlayerScript.list[j].transform.position);
                        if (len > maxLength)
                        {
                            maxLength = len;
                        }
                    }
                }
            }
        }
        if (noOfPlayers != 0)
        {
            camPos = camPos / noOfPlayers;
        }
        camPos += Vector3.forward * (-10f);
        float ort = 5f;
        if (maxLength != 0)
        {
            ort = 3f + maxLength / 2f;
        }

        Camera.main.orthographicSize += (ort - Camera.main.orthographicSize) * Game.dt * cameraSpeed;
        if (Mathf.Abs(ort - Camera.main.orthographicSize) < 0.01f)
        {
            Camera.main.orthographicSize = ort;
        }
        transform.position += (camPos - transform.position) * Game.dt * cameraSpeed;
        if (Geometry.lengthOfVector3(camPos - transform.position) > 0.01f)
        {
            transform.position = camPos;
        }

        if (transform.eulerAngles.z != 0)
        {
            fixRotation();

            if (transform.eulerAngles.z < 2f || transform.eulerAngles.z > 358f)
            {
                transform.eulerAngles = Vector3.zero;
            }
        }
	}



    void fixRotation()
    {
        if (transform.eulerAngles.z < 180f)
        {
            if (transform.eulerAngles.z < jumpFixSpeed * Game.dt)
            {
                transform.eulerAngles -= Vector3.forward * transform.eulerAngles.z;
            }
            else
            {
                transform.eulerAngles -= Vector3.forward * jumpFixSpeed * Game.dt;
            }
        }
        else
        {
            if (transform.eulerAngles.z > 360 - jumpFixSpeed * Game.dt)
            {
                transform.eulerAngles += Vector3.forward * transform.eulerAngles.z;
            }
            else
            {
                transform.eulerAngles += Vector3.forward * jumpFixSpeed * Game.dt;
            }
        }
    }
}
