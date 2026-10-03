using UnityEngine;
using System.Collections;

namespace Games.ChildhoodNightmare
{
	public class LightningColor : MonoBehaviour {

		private TintScript tint;
		private float timeCounter = 0f;
		public float period = 0.1f;

		void Start ()
		{
			tint = GetComponent<TintScript> ();
		}

		void Update ()
		{
			timeCounter += Game.dt;
			if (timeCounter >= period)
			{
				timeCounter -= period;
				float rand = Random.Range(0f, 3f);
				if (rand < 1f)
				{
					tint.selfColor = new Color(1f, 0f, 0f);
				}
				else if (rand < 2f)
				{
					tint.selfColor = new Color(0f, 1f, 0f);
				}
				else
				{
					tint.selfColor = new Color(0f, 0f, 1f);
				}
			}
		}
	}
}
