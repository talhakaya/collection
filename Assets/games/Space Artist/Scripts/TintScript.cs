using UnityEngine;
using System.Collections;

namespace Games.SpaceArtist
{
	public class TintScript : MonoBehaviour {

		public static Color weatherColor = new Color (1f, 1f, 1f);//new Color (1f, 1f, 1f);
		public static Color seaColor = new Color (0.3f, 0.3f, 1f);
		private SpriteRenderer sprite;
	    public bool inWater;
	    public bool notAffectedByWeather;
		public Color selfColor = new Color (1f, 1f, 1f);
		public Color spriteEffectColor = new Color (1f, 1f, 1f);
		public Color changingColor = new Color (1f, 1f, 1f);
		public float blurAlpha;
	    public float shadowConst = 1f;
	    public bool shadowOn;
	    public bool shadowAlwaysVisible;

	    void OnValidate()
	    {
	        GetComponent<SpriteRenderer>().color = selfColor;
	    }

		void Start ()
		{
			sprite = GetComponent<SpriteRenderer>();
	        if (shadowOn)
	        {
	            SpriteEffect.make(Effect.Shadow, gameObject);
	        }
		}

		void Update ()
		{
			if (!inWater)
			{
				sprite.color = new Color(selfColor.r * weatherColor.r * spriteEffectColor.r * changingColor.r, 
				                         selfColor.g * weatherColor.g * spriteEffectColor.g * changingColor.g, 
				                         selfColor.b * weatherColor.b * spriteEffectColor.b * changingColor.b, 
				                         selfColor.a * weatherColor.a * spriteEffectColor.a * changingColor.a);
	        }
	        else if (notAffectedByWeather)
	        {
	            sprite.color = new Color(selfColor.r * seaColor.r * spriteEffectColor.r * changingColor.r,
	                                     selfColor.g * seaColor.g * spriteEffectColor.g * changingColor.g,
	                                     selfColor.b * seaColor.b * spriteEffectColor.b * changingColor.b,
	                                     selfColor.a * seaColor.a * spriteEffectColor.a * changingColor.a);
	        }
	        else
	        {
	            sprite.color = new Color(selfColor.r * weatherColor.r * seaColor.r * spriteEffectColor.r * changingColor.r,
	                                     selfColor.g * weatherColor.g * seaColor.g * spriteEffectColor.g * changingColor.g,
	                                     selfColor.b * weatherColor.b * seaColor.b * spriteEffectColor.b * changingColor.b,
	                                     selfColor.a * weatherColor.a * seaColor.a * spriteEffectColor.a * changingColor.a);
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
