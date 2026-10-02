using UnityEngine;
using System.Collections;

public class Jail : MonoBehaviour {

	public bool destroyed;

	void Start ()
	{
		Game.fitInTile(gameObject);
	}

	void Update ()
	{
		if (destroyed)
		{
			collider2D.enabled = false;
			transform.localScale -= Vector3.one * Game.dt;
			if (transform.localScale.x < Game.dt)
			{
				Destroy(gameObject);
			}
		}
	}
}
