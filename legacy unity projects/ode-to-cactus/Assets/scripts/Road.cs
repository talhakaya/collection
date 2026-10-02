using UnityEngine;
using System.Collections;

public class Road : MonoBehaviour {

	void Start ()
    {
	
	}
	
	void Update ()
    {
        transform.position -= Vector3.forward * Game.roadSpeed * Game.dt;
        if (transform.position.z < -36f)
        {
            transform.position += Vector3.forward * 108f;
        }
	}
}
