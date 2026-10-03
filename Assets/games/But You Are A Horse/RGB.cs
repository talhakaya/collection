using UnityEngine;
using System.Collections;

namespace Games.ButYouAreAHorse
{
	public class RGB : MonoBehaviour {

		void Start ()
	    {
	        SpriteEffect.make(Effect.RGBSplit, gameObject, false, true, Camera.main.transform);
		}
	}
}
