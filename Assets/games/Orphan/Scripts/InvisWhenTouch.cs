using UnityEngine;
using System.Collections;

namespace Games.Orphan
{
	public class InvisWhenTouch : MonoBehaviour {
		public tk2dSprite sprite;
		public float minAlpha;
		public bool touching;
		public Color color;

		void Start () {
			sprite = gameObject.GetComponent<tk2dSprite>();
			color = Color.white;
		}

		void Update () {
			if (touching)
			{
				if (color.a > minAlpha)
				{
					color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a - Time.deltaTime);
					sprite.color = color;
				}
			}
			else
			{
				if (color.a < 1f)
				{
					color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a + Time.deltaTime);
					sprite.color = color;
				}
			}
		}

		void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.name == "Player")
			{
				touching = true;
			}
		}

		void OnTriggerExit(Collider other)
		{
			if (other.gameObject.name == "Player")
			{
				touching = false;
			}
		}
	}
}
