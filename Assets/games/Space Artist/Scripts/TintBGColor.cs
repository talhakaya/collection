using UnityEngine;
using System.Collections;

namespace Games.SpaceArtist
{
	public class TintBGColor : MonoBehaviour
	{
	    private TintScript tint;

		void Start ()
	    {
	        tint = GetComponent<TintScript>();
		}
		
		void Update ()
	    {
	        tint.selfColor = new Color(CameraBGColor.colorStatic.r, CameraBGColor.colorStatic.g, CameraBGColor.colorStatic.b, tint.selfColor.a);
		}
	}

}
