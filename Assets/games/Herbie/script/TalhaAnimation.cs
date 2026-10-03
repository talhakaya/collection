using UnityEngine;
using System.Collections;

namespace Games.Herbie
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
			// In the collection: rounding can land the division exactly on sprites.Length.
			if (i >= sprites.Length)
			{
				i = sprites.Length - 1;
			}
			spriteRenderer.sprite = sprites[i];
		}
	}
}
