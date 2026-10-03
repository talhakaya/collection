using UnityEngine;
using System.Collections;

namespace Games.GGJ2015
{
	public class Wall : MonoBehaviour {

		void Start ()
		{
			Game.fitInTile (gameObject);
		}

		void Update ()
		{
			GetComponent<Collider2D>().enabled = !Game.transition;
		}
	}
}
