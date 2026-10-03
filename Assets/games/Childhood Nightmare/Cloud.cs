using UnityEngine;
using System.Collections;

namespace Games.ChildhoodNightmare
{
	public class Cloud : MonoBehaviour {

		public Transform sprite;
		private Vector3 rotateDir;

		void Start ()
		{
			sprite.localScale = Vector3.one * Random.Range (48f, 64f);
			rotateDir = new Vector3 (Random.Range (-1f, 1f), Random.Range (5f, 10f), 0f);
			transform.Rotate(new Vector3(Random.Range (0f, 360f), Random.Range (0f, 360f), 0f));
	//		SpriteEffect.make (Effect.RGBSplit, gameObject, false, true, Player.trans);
		}

		void Update ()
		{
			transform.Rotate (rotateDir * Game.dt);
		}
	}
}
