using UnityEngine;
using System.Collections;

namespace Games.LetsNeverDoThatAgain
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
			// In the collection: the float maths can land on sprites.Length, so it is clamped.
			i = Mathf.Min(Mathf.FloorToInt((Game.time % (period * sprites.Length)) / period), sprites.Length - 1);
			spriteRenderer.sprite = sprites[i];
		}
	}
}
