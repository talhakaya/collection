using UnityEngine;
using System.Collections;

namespace Games.Orphan
{
	public class WaveScriptPrologue : MonoBehaviour {

		private float speed;
		private Vector3 initialPos;
		private const float yDistance = 1f;
		private tk2dSprite sprite;
		private bool movingDown;
		private bool visible;

		public bool randomPos;
		public bool moving;

		void Start ()
		{
			initialPos = transform.position;
			sprite = gameObject.GetComponent<tk2dSprite>();
			Reset ();
			movingDown = true;
			if (Random.Range(-1f, 1f) > 0f)
			{
				transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
			}
		}

		void Update ()
		{
			if (moving)
			{
				transform.position -= Vector3.up * speed;
				if (visible)
				{
					sprite.color = new Color(1f, 1f, 1f, (transform.position.y - (initialPos.y - yDistance)) / yDistance);
				}
				else
				{
					sprite.color = new Color(1f, 1f, 1f, sprite.color.a + 0.02f);
					if (sprite.color.a > 0.8f)
					{
						visible = true;
					}
				}
				if (transform.position.y < initialPos.y - yDistance)
				{
					Reset ();
				}
			}
			else
			{
				if (movingDown)
				{
					transform.position -= Vector3.up * speed;
					if (transform.position.y < initialPos.y - yDistance)
					{
						movingDown = false;
					}
				}
				else
				{
					transform.position += Vector3.up * speed;
					if (transform.position.y > initialPos.y)
					{
						movingDown = true;
					}
				}
			}
		}

		void Reset()
		{
			speed = Random.Range(0.003f, 0.005f);
			transform.position = initialPos;
			visible = false;
			if (moving)
			{
				sprite.color = new Color(1f, 1f, 1f, 0f);
			}
			if (randomPos)
			{
				transform.position += Vector3.right * Random.Range(-8f, 8f) + Vector3.up * Random.Range(-5f, 5f);
				transform.localScale = Vector3.one * Random.Range(0.5f, 1f);
				speed = Random.Range(0.005f, 0.01f);
			}
		}
	}
}
