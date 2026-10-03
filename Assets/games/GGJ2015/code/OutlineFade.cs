using UnityEngine;
using System.Collections;

namespace Games.GGJ2015
{
	public class OutlineFade : MonoBehaviour {

		private SpriteRenderer sr;
		// Use this for initialization
		void Start () {
			sr = GetComponent<SpriteRenderer> ();
		}

		// Update is called once per frame
		void Update () {
			transform.localScale += Vector3.one * Game.dt * 2f;
			sr.color = new Color (1f, 1f, 1f, sr.color.a - Game.dt);
			if (sr.color.a <= 0f)
			{
				Destroy(gameObject);
			}
		}
	}
}
