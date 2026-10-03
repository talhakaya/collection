using UnityEngine;
using System.Collections;

namespace Games.Herbie
{
	public class Casette : MonoBehaviour {

		public bool done;
		private TintScript tint;

		void Start ()
		{
			tint = GetComponent<TintScript> ();
		}

		void Update ()
		{
			if (done)
			{
				transform.localScale += Vector3.one * Game.dt * 5;
				tint.selfColor = new Color(tint.selfColor.r, tint.selfColor.g, tint.selfColor.b, tint.selfColor.a - Game.dt / 5);
				transform.Rotate(Vector3.forward * Game.dt * 360);
				if (tint.selfColor.a <= 0f)
				{
					Destroy(gameObject);
				}
			}
		}
	}
}
