using UnityEngine;
using System.Collections;

namespace Games.Fuck
{
	public class TalhaRandomSprite : MonoBehaviour {

		public Sprite[] sprites;
		private SpriteRenderer spriteRenderer;

		void Start ()
		{
			spriteRenderer = GetComponent<SpriteRenderer> ();
			spriteRenderer.sprite = sprites[Random.Range (0, sprites.Length)];
		}

		void Update ()
		{

		}
	}
}
