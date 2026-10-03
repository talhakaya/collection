using UnityEngine;
using System.Collections;

namespace Games.GGJ2015
{
	public class Goal : MonoBehaviour {

		public static int count = 0;
		public GameObject playerOutline;
		private bool playerHere;
		private float counter = 0f;

		void Start ()
		{
			Game.fitInTile(gameObject);
		}

		void Update ()
		{
			GetComponent<Collider2D>().enabled = !Game.transition;

			if (playerHere)
			{
				counter += Game.dt;
				if (counter >= 0.1f)
				{
					counter -= 0.1f;
					Instantiate(playerOutline, transform.position - Vector3.forward * 5, Quaternion.identity);
				}
			}
		}

		void OnTriggerEnter2D(Collider2D other)
		{
			if (other.gameObject.name == "player")
			{
				Debug.Log ("+ " + other.transform.position.x + " " + other.transform.position.y);
				playerHere = true;
				Player.onGoal++;
			}
		}

		void OnTriggerExit2D(Collider2D other)
		{
			if (other.gameObject.name == "player")
			{
				Debug.Log ("- " + other.transform.position.x + " " + other.transform.position.y);
				playerHere = false;
				Player.onGoal--;
			}
		}

		public void reset()
		{
			playerHere = false;
		}
	}
}
