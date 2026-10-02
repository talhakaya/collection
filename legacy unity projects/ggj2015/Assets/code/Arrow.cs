using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {

	public bool isVertical;

	void Start ()
	{
		
	}
	
	void Update ()
	{
		collider2D.enabled = !Game.transition;
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.name == "player")
		{
			if (!Game.transition)
			{
				Game.instance.rotateWorld(isVertical);
			}
		}
	}
}
