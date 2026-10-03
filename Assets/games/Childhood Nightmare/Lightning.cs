using UnityEngine;
using System.Collections;

namespace Games.ChildhoodNightmare
{
	public class Lightning : MonoBehaviour {

		public Transform sprite;
		private Vector3 rotateDir;

		void Start ()
		{
			sprite.localScale = Vector3.one * Random.Range (16f, 24f);
			rotateDir = new Vector3 (Random.Range (-10f, 10f), Random.Range (50f, 100f), 0f);
			transform.Rotate(new Vector3(Random.Range (0f, 360f), Random.Range (0f, 360f), 0f));
		}

		void Update ()
		{
			transform.Rotate (rotateDir * Game.dt);
		}
	}
}
