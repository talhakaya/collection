using UnityEngine;
using System.Collections;

namespace Games.TroubledFootball
{
	public class Detector : MonoBehaviour {

		public int stateToChangeTo;

		// Use this for initialization
		void Start () {

		}

		// Update is called once per frame
		void Update () {

		}

		void OnTriggerEnter2D(Collider2D other)
		{
			if (other.name == "Ball" && GameManager.gameState == 0)
			{
				GameManager.gameState = stateToChangeTo;
				if (GameManager.gameState == 1)
				{
					AudioManager.playGoal = true;
				}
			}
		}
	}
}
