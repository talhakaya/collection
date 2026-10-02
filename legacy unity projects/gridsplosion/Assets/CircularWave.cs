using UnityEngine;
using System.Collections;

public class CircularWave : MonoBehaviour {

    float rotSpeed;

	void Start ()
    {
        GetComponent<Scale>().period = Random.Range(1f, 4f);
        GetComponent<Scale>().deltaScale = Random.Range(0.05f, 0.2f);
        transform.Rotate(0f, 0f, Random.value * 360f);
        rotSpeed = Random.Range(200f, 500f);
        if (Random.value < 0.5f)
        {
            rotSpeed = -rotSpeed;
        }
	}
	
	void Update ()
    {
        transform.Rotate(0f, 0f, rotSpeed * Game.dt);
	}
}
