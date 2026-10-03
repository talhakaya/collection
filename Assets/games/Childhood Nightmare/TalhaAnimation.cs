using UnityEngine;
using System.Collections;

namespace Games.ChildhoodNightmare
{
	public class TalhaAnimation : MonoBehaviour {

		public Sprite[] sprites;
		private float timeCounter = 0f;
		public float period = 0.1f;
		private int i = 0;
		private SpriteRenderer spriteRenderer;

		void Start ()
		{
			spriteRenderer = GetComponent<SpriteRenderer> ();
			i = Random.Range (0, sprites.Length);
			spriteRenderer.sprite = sprites[i];
		}

		void Update ()
		{
			timeCounter += Game.dt;
			if (timeCounter >= period)
			{
				timeCounter -= period;
				i++;
				if (i >= sprites.Length)
				{
					i = 0;
				}
				spriteRenderer.sprite = sprites[i];
			}
		}
	}
}
