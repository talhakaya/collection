using UnityEngine;
using System.Collections;

namespace Games.Golfinity
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
			spriteRenderer.sprite = sprites[i];
		}
	}

}
