using UnityEngine;
using System.Collections;

namespace Games.Abused
{
	public enum Effect
	{
		None,
		RGBSplit,
		Blur
	}

	public class SpriteEffect : MonoBehaviour {

		public static float blurConst = 40f;
		public static float rgbSplitConst = 10f;
		public static int blurSpriteCount = 6;
		public static float rgbSplitMainAlpha = 0.5f;
		public static float rgbSplitSideAlpha = 0.2f;

		public TintScript tint;
		public TintScript tintParent;
		public SpriteRenderer rendererSelf;
		public SpriteRenderer rendererParent;
		public Effect effect;
		public float angle;
		public float distance = 0f;
		public float localZ;
		public bool relativeToMouse;
		public bool relativeToObject;
		public Vector2 relativeVector2;
		public Transform relativeTransform;


		void Start ()
		{
		
		}

		void Update ()
		{
			if (relativeToMouse)
			{
				relativeVector2 = new Vector2(MousePosition.x, MousePosition.y);
			}
			else if (relativeToObject && relativeTransform != null)
			{
				relativeVector2 = new Vector2(relativeTransform.position.x, relativeTransform.position.y);
			}

			tint.selfColor = tintParent.selfColor;
			distance = Mathf.Max (0f, Mathf.Sqrt(Geometry.lengthOfVector2(relativeVector2 - new Vector2(transform.position.x, transform.position.y))) / 1f - 0.2f);
			float maxDistance = 0.2f;
			if (distance > 0)
			{
				if (distance < maxDistance)
				{
					distance = maxDistance - distance;
				}
				else
				{
					distance = 0f;
				}
			}
			else if (distance < 0)
			{
				if (distance > -maxDistance)
				{
					distance = -maxDistance - distance;
				}
				else
				{
					distance = 0f;
				}
			}
			else
			{
				distance = maxDistance;
			}
			if (effect == Effect.Blur)
			{
				if (blurConst > 0/* && distance > 0.05f*/)
				{
					tintParent.spriteEffectColor = new Color(tintParent.spriteEffectColor.r, tintParent.spriteEffectColor.g, tintParent.spriteEffectColor.b, tintParent.blurAlpha);
					tint.spriteEffectColor = new Color(tintParent.spriteEffectColor.r, tintParent.spriteEffectColor.g, tintParent.spriteEffectColor.b, tintParent.blurAlpha);
				}
				else
				{
					tintParent.spriteEffectColor = new Color(tintParent.spriteEffectColor.r, tintParent.spriteEffectColor.g, tintParent.spriteEffectColor.b, 1f);
					tint.spriteEffectColor = new Color(tintParent.spriteEffectColor.r, tintParent.spriteEffectColor.g, tintParent.spriteEffectColor.b, 0f);
				}
			}
			else if (effect == Effect.RGBSplit)
			{
				if (rgbSplitConst > 0)
				{
					tint.spriteEffectColor = new Color(tint.spriteEffectColor.r, tint.spriteEffectColor.g, tint.spriteEffectColor.b, rgbSplitSideAlpha);
				}
				else
				{
					tint.spriteEffectColor = new Color(tint.spriteEffectColor.r, tint.spriteEffectColor.g, tint.spriteEffectColor.b, 0f);
				}
			}

			if (effect == Effect.Blur)
			{
				distance *= blurConst;
			}
			else if (effect == Effect.RGBSplit)
			{
				distance *= rgbSplitConst;
			}

			transform.localPosition = Geometry.createVector3(angle, distance) + Vector3.forward * localZ;
			rendererSelf.sprite = rendererParent.sprite;
		}

		public static void make(Effect newEffect, GameObject obj, bool relativeToMouse = true, bool relativeToObject = false, Transform relativeTransform = null, Vector2 relativeVector2 = default(Vector2))
		{
			if (newEffect == Effect.None)
			{
				return;
			}

			destroy (newEffect, obj);

			foreach (Transform child in obj.transform)
			{
				make (newEffect, child.gameObject);
			}

			if (obj.GetComponent<SpriteEffect> () == null && obj.GetComponent<TintScript> () != null && obj.GetComponent<SpriteRenderer> () != null)
			{
				TintScript tint = obj.GetComponent<TintScript> ();
				if (newEffect == Effect.RGBSplit)
				{
					SpriteEffect tempSprite = createNewObject(obj, newEffect);
					tempSprite.tint.spriteEffectColor = new Color(1f, 0f, 0f, 0.2f);
					tempSprite.angle = 210f;
					tempSprite.localZ = 0.01f;
					tempSprite.relativeToMouse = relativeToMouse;
					tempSprite.relativeToObject = relativeToObject;
					tempSprite.relativeTransform = relativeTransform;
					tempSprite.relativeVector2 = relativeVector2;

					tempSprite = createNewObject(obj, newEffect);
					tempSprite.tint.spriteEffectColor = new Color(0f, 1f, 0f, 0.2f);
					tempSprite.angle = 90;
					tempSprite.localZ = 0.01f;
					tempSprite.relativeToMouse = relativeToMouse;
					tempSprite.relativeToObject = relativeToObject;
					tempSprite.relativeTransform = relativeTransform;
					tempSprite.relativeVector2 = relativeVector2;

					tempSprite = createNewObject(obj, newEffect);
					tempSprite.tint.spriteEffectColor = new Color(0f, 0f, 1f, 0.2f);
					tempSprite.angle = -30;
					tempSprite.localZ = 0.01f;
					tempSprite.relativeToMouse = relativeToMouse;
					tempSprite.relativeToObject = relativeToObject;
					tempSprite.relativeTransform = relativeTransform;
					tempSprite.relativeVector2 = relativeVector2;

					tint.spriteEffectColor = new Color(tint.spriteEffectColor.r, tint.spriteEffectColor.g, tint.spriteEffectColor.b, 0.5f);
				}
				else if (newEffect == Effect.Blur)
				{
					tint.blurAlpha = 1f / blurSpriteCount;
					for (int i = 0; i < blurSpriteCount - 1; i++)
					{
						SpriteEffect tempSprite = createNewObject(obj, newEffect);
						tempSprite.tint.spriteEffectColor = new Color(1f, 1f, 1f, tint.blurAlpha);
						tempSprite.angle = Random.Range (0f, 360f);
						tempSprite.localZ = -0.01f;
						tempSprite.relativeToMouse = relativeToMouse;
						tempSprite.relativeToObject = relativeToObject;
						tempSprite.relativeTransform = relativeTransform;
						tempSprite.relativeVector2 = relativeVector2;

					}
					tint.spriteEffectColor = new Color(tint.spriteEffectColor.r, tint.spriteEffectColor.g, tint.spriteEffectColor.b, tint.blurAlpha);
				}
			}
		}

		private static SpriteEffect createNewObject(GameObject obj, Effect newEffect)
		{
			GameObject newObj = new GameObject ("TemporarySprite");

			SpriteEffect spriteEffect = newObj.AddComponent<SpriteEffect> ();
			spriteEffect.tint = newObj.AddComponent<TintScript> ();
			spriteEffect.tintParent = obj.GetComponent<TintScript> ();
			spriteEffect.rendererSelf = newObj.AddComponent<SpriteRenderer> ();
			spriteEffect.rendererParent = obj.GetComponent<SpriteRenderer> ();
			spriteEffect.effect = newEffect;
			newObj.transform.parent = obj.transform;
			newObj.transform.localScale = Vector3.one;

			return newObj.GetComponent<SpriteEffect>();
		}

		public static void destroy(Effect effect, GameObject obj)
		{
			if (effect == Effect.None)
			{
				return;
			}

			foreach (Transform child in obj.transform)
			{
				destroy (effect, child.gameObject);
			}
			
			if (obj.GetComponent<SpriteEffect> () != null)
			{
				if (obj.GetComponent<SpriteEffect> ().effect == effect)
				{
					Destroy(obj);
				}
			}
			else if (obj.GetComponent<TintScript> () != null && obj.GetComponent<SpriteRenderer> () != null)
			{
				TintScript tint = obj.GetComponent<TintScript> ();
				tint.spriteEffectColor = new Color(tint.spriteEffectColor.r, tint.spriteEffectColor.g, tint.spriteEffectColor.b, 1f);
			}
		}
	}

}
