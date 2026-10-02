using UnityEngine;
using System.Collections;

public class HerbieFight : MonoBehaviour {

	void Start ()
	{

	}

	void Update ()
	{
	
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.name == "fist")
		{
			Game.instance.aFistHit.Play ();
			Game.endMiniGame();
		}
	}
}
