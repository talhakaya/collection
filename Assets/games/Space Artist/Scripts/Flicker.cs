using UnityEngine;
using System.Collections;

namespace Games.SpaceArtist
{
	public class Flicker : MonoBehaviour {
	    private TintScript tint;
	    public float maxAlpha = 1f;
	    public float minAlpha = 0f;

		void Start ()
	    {
	        tint = GetComponent<TintScript>();
		}
		
		void Update ()
	    {
	        tint.selfColor = new Color(tint.selfColor.r, tint.selfColor.g, tint.selfColor.b, Random.Range(minAlpha, maxAlpha));
		}
	}

}
