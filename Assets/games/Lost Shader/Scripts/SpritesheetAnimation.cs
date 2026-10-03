using UnityEngine;
using System.Collections;

namespace Games.LostShader
{
	public class SpritesheetAnimation : MonoBehaviour {

		public Sprite[] sprites;
		public float period = 0.2f;
		private float time = 0f;
		private int i = 0;
		private SpriteRenderer spriteRenderer;

		void OnEnable()
		{
			time = 0f;
			spriteRenderer = GetComponent<SpriteRenderer> ();
			spriteRenderer.sprite = sprites[0];
		}

		void Start ()
		{
			spriteRenderer = GetComponent<SpriteRenderer> ();
			spriteRenderer.sprite = sprites[0];
		}

		void Update ()
		{
			time += Time.deltaTime;
			i = Mathf.FloorToInt((time % (period * sprites.Length)) / period);
			// In the collection: rounding can land the division exactly on sprites.Length.
			if (i >= sprites.Length)
			{
				i = sprites.Length - 1;
			}
			spriteRenderer.sprite = sprites[i];
		}
	}
}
