using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Games.Nykrig
{
	public class TintScript : MonoBehaviour {
	    public static float blackAndWhiteRatio;
	    public static bool blackAndWhite;
	    //public static bool notAffectedByWeatherStatic;
		public static Color weatherColor = new Color (1f, 1f, 1f);//new Color (1f, 1f, 1f);
		public static Color seaColor = new Color (0.3f, 0.3f, 1f);
		private SpriteRenderer sprite;
		private Image image;
	    public bool inWater;
	    public bool notAffectedByWeather;
		public Color selfColor = new Color (1f, 1f, 1f);
		public Color spriteEffectColor = new Color (1f, 1f, 1f);
		public Color changingColor = new Color (1f, 1f, 1f);
		public float blurAlpha;
	    public float shadowConst = 1f;
	    //public float shadowDistanceConst = 1f;
	    public bool shadowOn;
	    public bool shadowButNotOnChildren;
	    public Transform lightSource;
	    public bool dynamicShadowFromLightSource;
	    public Transform shadowParent;
	    public Sprite shadowSprite;
	    public bool shadowScaleContinuously;
	    public float shadowZScale = 0.1f;
	    public float blackAndWhiteRatioMax = 1f;
	    public bool reverseShadow;

	    void OnValidate() {
	        if (GetComponent<SpriteRenderer>() != null) {
	            GetComponent<SpriteRenderer>().color = selfColor;
	        }
	        else if (GetComponent<Image>() != null) {
	            GetComponent<Image>().color = selfColor;
	        }
	    }

		void Start ()
		{
			sprite = GetComponent<SpriteRenderer>();
	        image = GetComponent<Image>();
	        if (shadowOn || shadowButNotOnChildren)
	        {
	            SpriteEffect.skipChildren = shadowButNotOnChildren;
	            SpriteEffect.make(Effect.Shadow, gameObject);
	            SpriteEffect.skipChildren = false;
	        }
		}

		void Update ()
		{
	        Color c = Color.white;
	        if (blackAndWhite || notAffectedByWeather)
	        {
	            c = new Color(selfColor.r * spriteEffectColor.r * changingColor.r,
	                                     selfColor.g * spriteEffectColor.g * changingColor.g,
	                                     selfColor.b * spriteEffectColor.b * changingColor.b,
	                                     selfColor.a * spriteEffectColor.a * changingColor.a);
	        }
	        else if (!inWater)
	        {
	            c = new Color(selfColor.r * weatherColor.r * spriteEffectColor.r * changingColor.r,
	                                     selfColor.g * weatherColor.g * spriteEffectColor.g * changingColor.g,
	                                     selfColor.b * weatherColor.b * spriteEffectColor.b * changingColor.b,
	                                     selfColor.a * weatherColor.a * spriteEffectColor.a * changingColor.a);
	        }
	        else
	        {
	            c = new Color(selfColor.r * weatherColor.r * seaColor.r * spriteEffectColor.r * changingColor.r,
	                                     selfColor.g * weatherColor.g * seaColor.g * spriteEffectColor.g * changingColor.g,
	                                     selfColor.b * weatherColor.b * seaColor.b * spriteEffectColor.b * changingColor.b,
	                                     selfColor.a * weatherColor.a * seaColor.a * spriteEffectColor.a * changingColor.a);
	        }

	        if (blackAndWhite)
	        {
	            float grey = (c.r + c.g + c.b) / 3f;
	            if (blackAndWhiteRatio == 0f) {
	                c = new Color(grey, grey, grey, c.a);
	            }
	            else {
	                float bawr = Mathf.Min(blackAndWhiteRatioMax, blackAndWhiteRatio);
	                float r = bawr * c.r + (1f - bawr) * grey;
	                float g = bawr * c.g + (1f - bawr) * grey;
	                float b = bawr * c.b + (1f - bawr) * grey;
	                c = new Color(r, g, b, c.a);
	            }
	        }

	        if (sprite != null) {
	            sprite.color = c;
	        }
	        else if (image != null) {
	            image.color = c;
	        }
	    }

		public static Color deltaWeatherColor(float deltaR, float deltaG, float deltaB)
		{
			return weatherColor = new Color (weatherColor.r + deltaR, weatherColor.g + deltaG, weatherColor.b + deltaB);
		}
		
		public static Color deltaRWeatherColor(float deltaR)
		{
			return weatherColor = new Color (weatherColor.r + deltaR, weatherColor.g, weatherColor.b);
		}
		
		public static Color deltaGWeatherColor(float deltaG)
		{
			return weatherColor = new Color (weatherColor.r, weatherColor.g + deltaG, weatherColor.b);
		}
		
		public static Color deltaBWeatherColor(float deltaB)
		{
			return weatherColor = new Color (weatherColor.r, weatherColor.g, weatherColor.b + deltaB);
		}
		
		public static Color changeWeatherColor(float r, float g, float b)
		{
			return weatherColor = new Color (r, g, b);
		}
		
		public static Color changeRWeatherColor(float r)
		{
			return weatherColor = new Color (r, weatherColor.g, weatherColor.b);
		}
		
		public static Color changeGWeatherColor(float g)
		{
			return weatherColor = new Color (weatherColor.r, g, weatherColor.b);
		}
		
		public static Color changeBWeatherColor(float b)
		{
			return weatherColor = new Color (weatherColor.r, weatherColor.g, b);
		}
	}

}
