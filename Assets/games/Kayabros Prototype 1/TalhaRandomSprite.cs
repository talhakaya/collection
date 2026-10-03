using UnityEngine;
using System.Collections;

namespace Games.KayabrosPrototype1
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
