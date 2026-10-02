using UnityEngine;
using System.Collections;

public class RotateScript : MonoBehaviour {

	public bool rotating;
	public bool isClockwise;
	public float speed;

	void Start ()
	{
	
	}

	void Update ()
	{
		if (rotating)
		{
			float cons = 20f;
			if (!isClockwise)
			{
				cons = -cons;
			}
			if (Input.GetMouseButton(0))
			{
				cons *= 2;
			}
			transform.Rotate(0f, 0f, speed * cons * Time.deltaTime);
		}
	}
}
