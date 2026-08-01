using UnityEngine;
using System.Collections;

namespace Games.Chocolate
{
	public class Girl : MonoBehaviour {

		public Sprite idle;
		public Sprite walk1;
		public Sprite walk2;

		public bool walking;
		public bool isRight;
		private float animPeriod = 0.25f;
		private float timeCounter;
		private SpriteRenderer spriteRenderer;

		public static Transform instance;
		public static Girl script;
		public static float speed = 2f;

		void Start ()
		{
			spriteRenderer = GetComponent<SpriteRenderer> ();
			instance = transform;
			script = this;
		}

		void Update ()
		{
			transform.rotation = Quaternion.identity;
			if (!isRight)
			{
				transform.Rotate(Vector3.up * 180);
			}

			if (!walking)
			{
				spriteRenderer.sprite = idle;
				timeCounter = 0f;
			}
			else
			{
				timeCounter += Time.deltaTime;
				if (timeCounter < animPeriod)
				{
					spriteRenderer.sprite = walk1;
				}
				else if (timeCounter < 2 * animPeriod)
				{
					spriteRenderer.sprite = idle;
				}
				else if (timeCounter < 3 * animPeriod)
				{
					spriteRenderer.sprite = walk2;
				}
				else if (timeCounter < 4 * animPeriod)
				{
					spriteRenderer.sprite = idle;
				}
				else
				{
					timeCounter = 0f;
				}
			}
		}
	}
}
