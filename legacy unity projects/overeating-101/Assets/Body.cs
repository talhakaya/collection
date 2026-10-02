using UnityEngine;
using System.Collections;

public class Body : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetAxisRaw("Horizontal") != 0)
		{
			transform.position += Vector3.right * Input.GetAxisRaw("Horizontal") * 5 * Game.dt;
		}
	}
}
