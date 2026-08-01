using UnityEngine;
using System.Collections;

namespace Games.Nykrig
{
	public enum Effect
	{
		None,
		RGBSplit,
		Blur,
	    Shadow,
	    MotionBlur
	}

	public class SpriteEffect : MonoBehaviour {

		public static float blurConst = 0.4f;
	    public static float rgbSplitConst = 0.05f;
	    public static int blurSpriteCount = 6;
	    public static int motionBlurSpriteCount = 10;
	    public static float rgbSplitMainAlpha = 0.8f;
	    public static float motionBlurAlpha = 0.6f;
		public static float rgbSplitSideAlpha = 0.2f;
	    public static Color shadowColor = new Color(0f, 0f, 0f, 0.5f);

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
	    public float parentShadowConst = 1f;
	    //public float parentShadowDistanceConst = 1f;
	    public static bool skipChildren;
	    private Vector3 nondynamicShadowVector;
	    private bool dontUpdateSprite;
	    private GameObject motionBlurParent;
	    private float motionBlurPeriod;
	    private float motionBlurOffset;
	    private bool useRealTime;
	    private float motionBlurTimer;
	    private Vector3 motionBlurPosition;
	    private Vector3 motionBlurRotation;
	    private bool motionBlurScaleDown;

		void Start ()
		{
		    if (effect == Effect.Shadow && tintParent.lightSource != null && !tintParent.dynamicShadowFromLightSource)
	        {
	            nondynamicShadowVector = Geometry.createVector3(Geometry.angleOfVector3(tintParent.transform.position - tintParent.lightSource.position), Game.DefaultShadowDistance) + Vector3.forward * 0.1f;
	        }
	        if (effect == Effect.MotionBlur)
	        {
	            setMotionBlurData();
	            if (!useRealTime)
	            {
	                motionBlurTimer = motionBlurOffset;
	            }
	        }
		}

		void Update ()
	    {
	        tint.selfColor = tintParent.selfColor;
	        if (effect == Effect.RGBSplit || effect == Effect.Blur)
	        {
	            if (relativeToMouse)
	            {
	                relativeVector2 = new Vector2(MousePosition.x - transform.position.x, MousePosition.y - transform.position.y);
	            }
	            else if (relativeToObject && relativeTransform != null)
	            {
	                relativeVector2 = new Vector2(relativeTransform.position.x - transform.position.x, relativeTransform.position.y - transform.position.y);
	            }

	            distance = Mathf.Max(0f, Mathf.Sqrt(Geometry.lengthOfVector2(relativeVector2)) / 5f - 0.2f);
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
	        }
	        else if (effect == Effect.Shadow)
	        {
	            Vector3 shadowVector = Game.shadowVector * (tintParent.reverseShadow ? -1f : 1f);
	            shadowVector.z = 1f;
	            if (tintParent.lightSource != null)
	            {
	                if (tintParent.dynamicShadowFromLightSource)
	                {
	                    shadowVector = Geometry.createVector3(Geometry.angleOfVector3(tintParent.transform.position - tintParent.lightSource.position), Game.DefaultShadowDistance) + Vector3.forward * 0.1f;
	                }
	                else
	                {
	                    shadowVector = nondynamicShadowVector;
	                }
	            }
	            transform.position = transform.parent.position + shadowVector * tint.shadowConst * parentShadowConst;

	            if (tintParent.shadowScaleContinuously)
	            {
	                setShadowScale(tint, tintParent, tintParent.shadowZScale);
	            }
	            //if (tintParent.shadowParent != null)
	            //{
	                //transform.localPosition += (tint.shadowConst * parentShadowConst) * tintParent.shadowParent.InverseTransformPoint(tintParent.transform.position);
	            //}
	        }
	        else if (effect == Effect.MotionBlur)
	        {
	            if (motionBlurAlpha == 0f)
	            {
	                tint.spriteEffectColor = new Color(1f, 1f, 1f, 0f);
	            }
	            else
	            {
	                if (useRealTime)
	                {
	                    bool aboveHalfPeriod = (motionBlurTimer > (motionBlurPeriod * 0.5f));
	                    motionBlurTimer = (Time.realtimeSinceStartup + motionBlurOffset) % motionBlurPeriod;
	                    if (aboveHalfPeriod && (motionBlurTimer < (motionBlurPeriod * 0.5f)))
	                    {
	                        setMotionBlurData();
	                    }
	                }
	                else
	                {
	                    motionBlurTimer += Game.dt;
	                    if (motionBlurTimer >= motionBlurPeriod)
	                    {
	                        motionBlurTimer -= motionBlurPeriod;
	                        setMotionBlurData();
	                    }
	                }
	                transform.position = motionBlurPosition;
	                transform.eulerAngles = motionBlurRotation;
	                if (motionBlurScaleDown)
	                {
	                    transform.localScale = Vector3.one * ((motionBlurPeriod - motionBlurTimer) / motionBlurPeriod);
	                }
	                tint.spriteEffectColor = new Color(1f, 1f, 1f, (useRealTime ? 1f : motionBlurAlpha) * ((motionBlurPeriod - motionBlurTimer) / motionBlurPeriod));
	            }
	        }

	        if (!dontUpdateSprite)
	        {
	            rendererSelf.sprite = rendererParent.sprite;
	        }
		}

	    public static void make(Effect newEffect, GameObject obj, bool relativeToMouse = true, bool relativeToObject = false, Transform relativeTransform = null, Vector2 relativeVector2 = default(Vector2), float motionBlurPeriod = 0.5f, bool useRealTime = false, bool motionBlurScaleDown = false)
		{
			if (newEffect == Effect.None)
			{
				return;
			}


	        if (!skipChildren)
	        {
	            destroy(newEffect, obj);
			    foreach (Transform child in obj.transform)
			    {
				    make (newEffect, child.gameObject, relativeToMouse, relativeToObject, relativeTransform, relativeVector2);
			    }
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

					tint.spriteEffectColor = new Color(tint.spriteEffectColor.r, tint.spriteEffectColor.g, tint.spriteEffectColor.b, 1f);
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
	            else if (newEffect == Effect.Shadow)
	            {
	                SpriteEffect tempSprite = createNewObject(obj, newEffect);
	                TintScript tintParent = obj.GetComponent<TintScript>();
	                tempSprite.parentShadowConst = tintParent.shadowConst;
	                //tempSprite.tint.shadowConst = (0.9f + (1f - obj.transform.position.z) * 0.1f);// *tempSprite.parentShadowConst;
	                //tempSprite.transform.localScale *= Mathf.Max(1f, tempSprite.tint.shadowConst);
	                setShadowScale(tempSprite.tint, tintParent);
	                tempSprite.tint.spriteEffectColor = new Color(shadowColor.r, shadowColor.g, shadowColor.g, shadowColor.a + Mathf.Min(0f, -tempSprite.tint.shadowConst + 1f) * 0.6f);
	                Sprite shadowSprite = obj.GetComponent<TintScript>().shadowSprite;
	                if (shadowSprite != null)
	                {
	                    tempSprite.GetComponent<SpriteRenderer>().sprite = shadowSprite;
	                    tempSprite.dontUpdateSprite = true;
	                }
	            }
	            else if (newEffect == Effect.MotionBlur)
	            {
	                for (int i = 0; i < motionBlurSpriteCount; i++)
	                {
	                    SpriteEffect tempSprite = createNewObject(obj, newEffect);
	                    tempSprite.tint.spriteEffectColor = new Color(1f, 1f, 1f, tint.blurAlpha);
	                    tempSprite.angle = Random.Range(0f, 360f);
	                    tempSprite.localZ = 0.01f;
	                    tempSprite.motionBlurPeriod = motionBlurPeriod;
	                    tempSprite.motionBlurOffset = motionBlurPeriod * i / motionBlurSpriteCount;
	                    tempSprite.useRealTime = useRealTime;
	                    tempSprite.motionBlurScaleDown = motionBlurScaleDown;
	                }
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
	        newObj.transform.localEulerAngles = Vector3.zero;

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
	            destroy(effect, child.gameObject);
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

	    void setMotionBlurData()
	    {
	        motionBlurPosition = transform.parent.position + Vector3.forward * localZ;
	        motionBlurRotation = transform.parent.eulerAngles;
	    }

	    static void setShadowScale(TintScript shadow, TintScript parent, float zScale = 0.1f)
	    {
	        shadow.shadowConst = ((1f - zScale) + (1f - parent.transform.position.z) * zScale);// *tempSprite.parentShadowConst;
	        shadow.transform.localScale = Vector3.one * Mathf.Max(1f, shadow.shadowConst);
	    }
	}

}
