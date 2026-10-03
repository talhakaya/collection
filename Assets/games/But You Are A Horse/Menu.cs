using UnityEngine;
using System.Collections;

namespace Games.ButYouAreAHorse
{
	public class Menu : MonoBehaviour {

	    private static float minY = -21;
	    public GameObject rgbAffected;

		void Start ()
	    {
	        SpriteEffect.make(Effect.RGBSplit, rgbAffected, false, true, Camera.main.transform);
		}

		void Update ()
	    {
	        if (transform.position.y > minY)
	        {
	            transform.position -= Vector3.up * Game.dt;
	        }
	        else
	        {
	            transform.position = new Vector3(transform.position.x, minY, transform.position.z);
	        }
		}
	}
}
