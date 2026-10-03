using UnityEngine;
using System.Collections;

namespace Games.TheParasite
{
	public class Mouth : MonoBehaviour {

	    public static bool talking;
	    private Vector3 localScale;
	    public bool realSprite;

		void Start ()
	    {
	        localScale = transform.localScale;
		}

		void Update ()
	    {
	        if (realSprite == Game.realSprites)
	        {
	            if (talking)
	            {
	                transform.localScale = new Vector3(localScale.x * Random.Range(0.8f, 1f), localScale.y * Random.Range(0.5f, 1.5f), localScale.z);
	            }
	            else
	            {
	                transform.localScale = localScale;
	            }
	        }
	        else
	        {
	            transform.localScale = Vector3.zero;
	        }
		}
	}
}
