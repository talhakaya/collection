using UnityEngine;
using System.Collections;

namespace Games.Herbie
{
	public class Coke : MonoBehaviour {

		// Use this for initialization
		void Start () {

		}

		// Update is called once per frame
		void Update () {

		}

		void OnTriggerEnter2D(Collider2D other)
		{
			if (other.gameObject.name == "HerbieSmall")
			{
				Game.instance.aCoke.Play ();
				Pacman.cokeTaken = true;
				Destroy(gameObject);
			}
		}
	}
}
