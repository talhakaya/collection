using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour {

	void Start ()
	{
		Game.fitInTile (gameObject);
	}

	void Update ()
	{
		collider2D.enabled = !Game.transition;
	}
}
