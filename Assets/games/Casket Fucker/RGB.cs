using UnityEngine;
using System.Collections;

namespace Games.CasketFucker
{
	public class RGB : MonoBehaviour {



		void Start ()
	    {
	        SpriteEffect.make(Effect.RGBSplit, gameObject, false, false, null, Vector2.right * 20);
		}

		void Update ()
	    {

		}
	}
}
