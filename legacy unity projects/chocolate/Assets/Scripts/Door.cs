using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {
	
	public bool ready;
	
	void Start ()
	{
		
	}
	
	void Update ()
	{
		
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.name == "girl")
		{
			ready = true;
		}
	}
	
	void OnTriggerExit2D(Collider2D other)
	{
		if (other.name == "girl")
		{
			ready = false;
		}
	}
}
