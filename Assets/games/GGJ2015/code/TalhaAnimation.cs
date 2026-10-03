using UnityEngine;
using System.Collections;

namespace Games.GGJ2015
{
	public class TalhaAnimation : MonoBehaviour {

		public Sprite[] sprites;
		public float period = 0.2f;
		private int i = 0;
		private SpriteRenderer spriteRenderer;

		void Start ()
		{
			spriteRenderer = GetComponent<SpriteRenderer> ();
			spriteRenderer.sprite = sprites[0];
		}

		void Update ()
		{
			i = Mathf.FloorToInt((Game.time % (period * sprites.Length)) / period);
			// In the collection: the float maths can land on sprites.Length, so it is clamped.
			i = Mathf.Clamp(i, 0, sprites.Length - 1);
			spriteRenderer.sprite = sprites[i];
		}
	}
}
