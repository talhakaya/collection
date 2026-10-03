using UnityEngine;
using System.Collections;

namespace Games.Herbie
{
	public class SexPlace : MonoBehaviour {

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
				Pacman.cokeTaken = true;
				Destroy(gameObject);
			}
		}
	}
}
