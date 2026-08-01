using UnityEngine;
using System.Collections;

namespace Games.SpaceArtist
{
	public class TintAlphaToPlanes : MonoBehaviour
	{
	    public bool invert;
	    private TintScript tint;

		void Start ()
	    {
	        tint = GetComponent<TintScript>();
		}
		
		void Update ()
	    {
	        tint.selfColor = new Color(tint.selfColor.r, tint.selfColor.g, tint.selfColor.b, invert ? (1f - PlaneManager.openColorAlpha) : PlaneManager.openColorAlpha);
		}
	}

}
