using UnityEngine;
using System.Collections;

namespace Games.ChildhoodNightmare
{
	public class Screenn : MonoBehaviour {

		public TextMesh[] texts;
		private float timeCounter = 0f;
		public float period = 0.1f;
		public Font nes;
		public Font amon;

		void Start ()
		{

		}

		void Update ()
		{
	//		for (int i = 0; i < texts.Length; i++)
	//		{
	//			texts[i].color = TintScript.weatherColor;
	//		}

			timeCounter += Game.dt;
			if (timeCounter >= period)
			{
				timeCounter -= period;
				for (int i = 0; i < texts.Length; i++)
				{
					texts[i].text = "" + Random.Range(0, 10);
					bool glitchyFont = (Random.Range(0f, 100f) < 20f);
					if (glitchyFont)
					{
						texts[i].font = amon;
					}
					else
					{
						texts[i].font = nes;
					}
				}

			}
		}
	}
}
