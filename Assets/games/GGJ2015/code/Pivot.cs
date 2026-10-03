using UnityEngine;
using System.Collections;

namespace Games.GGJ2015
{
	public class Pivot : MonoBehaviour {

		public static Pivot instance;

		void Start ()
		{
			instance = this;
			Game.fitInTileHalf (gameObject);
		}

		void Update ()
		{

		}
	}
}
