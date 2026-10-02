using UnityEngine;
using System.Collections;

public class GroundObject : MonoBehaviour {
	public float yOffset;

	void Start ()
	{
	
	}

	void Update ()
	{
		transform.position = new Vector3 (transform.position.x, transform.position.y, (transform.position.y + yOffset) * 0.01f);
	}
}
