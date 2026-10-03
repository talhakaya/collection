using UnityEngine;
using System.Collections;

namespace Games.GGJ2015
{
	public class Arrow : MonoBehaviour {

		public bool isVertical;

		void Start ()
		{

		}

		void Update ()
		{
			GetComponent<Collider2D>().enabled = !Game.transition;
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
}
