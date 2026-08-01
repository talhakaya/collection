using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Games.SpaceArtist
{
	public class FadeOut : MonoBehaviour
	{
	    private Image image;

		void Start ()
	    {
	        image = GetComponent<Image>();
		}
		
		void Update ()
	    {
	        image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a - Game.dt);
	        if (image.color.a <= 0f)
	        {
	            Destroy(gameObject);
	        }
		}
	}

}
