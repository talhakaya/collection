using UnityEngine;
using System.Collections;

namespace Games.SpaceArtist
{
	public class CameraBGColor : MonoBehaviour
	{
	    public static Color colorStatic;
	    private Camera cam;

		void Start ()
	    {
	        colorStatic = new Color(Game.color1.r / 1.5f, Game.color1.g / 2f, Game.color1.b / 2f, 1f);
	        cam = GetComponent<Camera>();
	        cam.backgroundColor = colorStatic;
	        
		}
		
		void Update ()
	    {
		
		}
	}

}
