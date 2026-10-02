using UnityEngine;
using System.Collections;

public class Scanlines : MonoBehaviour {
	
	void Update ()
    {
        transform.position = new Vector3(transform.position.x, transform.parent.position.y + Random.Range(-3.6f, 3.6f), transform.position.z);
	}
}
