using UnityEngine;
using System.Collections;

namespace Games.Orphan
{
	public class FlickeringObject : MonoBehaviour {

		private bool isTk2d;
		private bool isRendered;
		private float originalAlpha;
		private tk2dSprite sprite;

		public float minAlpha;
		public float maxAlpha;
		public bool isFlickering;

		void Start ()
		{
			if (maxAlpha <= minAlpha)
			{
				maxAlpha = 1;
			}
			if (gameObject.GetComponent<tk2dSprite>() != null)
			{
				isTk2d = true;
				sprite = gameObject.GetComponent<tk2dSprite>();
				originalAlpha = sprite.color.a;
			}
			else if (GetComponent<Renderer>() != null && GetComponent<Renderer>().material != null)
			{
				isRendered = true;
			}
		}

		void Update ()
		{
			if (isTk2d)
			{
				if (isFlickering)
				{
					sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, Random.Range(minAlpha, maxAlpha));
				}
				else if (sprite.color.a != originalAlpha)
				{
					sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, originalAlpha);
				}
			}
			else if (isRendered)
			{
				if (isFlickering)
				{
					GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b, Random.Range(minAlpha, maxAlpha));
				}
				else if (GetComponent<Renderer>().material.color.a != originalAlpha)
				{
					GetComponent<Renderer>().material.color = new Color(GetComponent<Renderer>().material.color.r, GetComponent<Renderer>().material.color.g, GetComponent<Renderer>().material.color.b, originalAlpha);
				}
			}
		}
	}
}
