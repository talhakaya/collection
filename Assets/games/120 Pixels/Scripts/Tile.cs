using UnityEngine;
using System.Collections;

namespace Games.Pixels120
{
	public class Tile : MonoBehaviour {

		private float timeCounter;
		public float blackTime;
		private float shineTime;
		private TintScript tint;
		private Color color;
		private const float normalColorShake = 0.025f;
		private const float extraColorShake = 0.5f;
		private bool steppedOn;

		void Start ()
		{
			name = "Tile";

			tint = GetComponent<TintScript> ();
			tint.selfColor = new Color (0f, 0f, 0f, 0f);
			shineTime = 1f;
			color = new Color (Random.Range (0.3f, 1f), Random.Range (0.3f, 1f), Random.Range (0.3f, 1f));
		}

		void Update ()
		{
			transform.rotation = Quaternion.identity;
			transform.Rotate (Vector3.forward * Random.Range (-5f, 5f));
			if (timeCounter < blackTime)
			{
				timeCounter += Game.dt;
				tint.selfColor = new Color (0f, 0f, 0f, timeCounter / 1f);
			}
			else if (timeCounter < blackTime + shineTime)
			{
				timeCounter += Game.dt;
				tint.selfColor = new Color((color.r + normalColorShake * Random.Range (-1f, 1f)) * (timeCounter - blackTime) / shineTime, (color.g + normalColorShake * Random.Range (-1f, 1f)) * (timeCounter - blackTime) / shineTime, (color.b + normalColorShake * Random.Range (-1f, 1f)) * (timeCounter - blackTime) / shineTime);
			}
			else
			{
				if (steppedOn)
				{
					tint.selfColor = new Color(color.r + extraColorShake * Random.Range (-1f, 1f), color.g + extraColorShake * Random.Range (-1f, 1f), color.b + extraColorShake * Random.Range (-1f, 1f));
				}
				else
				{
					tint.selfColor = new Color(color.r + normalColorShake * Random.Range (-1f, 1f), color.g + normalColorShake * Random.Range (-1f, 1f), color.b + normalColorShake * Random.Range (-1f, 1f));
				}
			}
		}

		void OnTriggerEnter2D(Collider2D other)
		{
			if (other.name == "Talha")
			{
				steppedOn = true;
			}
		}

		void OnTriggerExit2D(Collider2D other)
		{
			if (other.name == "Talha")
			{
				steppedOn = false;
			}
		}
	}
}
