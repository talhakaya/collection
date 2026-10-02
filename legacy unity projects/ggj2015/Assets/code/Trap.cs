using UnityEngine;
using System.Collections;

public class Trap : MonoBehaviour {

	void Start ()
	{
		Game.fitInTile(gameObject);
	}

	void Update ()
	{
	
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.name == "player" && !Game.transition)
		{
			Player.count --;
			Destroy(other.gameObject);
		}
	}
}
