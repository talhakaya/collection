using UnityEngine;
using System.Collections;

namespace Games.SpaceArtist
{
	public class Tilt : MonoBehaviour
	{
	    public bool forceRight;
	    public bool forceLeft;
	    private bool goingRight;
	    private float timeCounter;
	    public float period = 1f;
	    public float angle = 5f;
	    public float waitFor;

		void Start ()
	    {
	        if (!forceLeft && (forceRight || Random.value < 0.5f))
	        {
	            goingRight = true;
	            transform.Rotate(-Vector3.forward * angle);
	        }
	        else
	        {
	            goingRight = false;
	            transform.Rotate(Vector3.forward * angle);
	        }
		}
		
		void Update ()
	    {
	        timeCounter += Game.dt;

	        if (timeCounter >= period + waitFor)
	        {
	            timeCounter -= period + waitFor;
	            goingRight = !goingRight;
	        }

	        if (timeCounter <= period)
	        {
	            if (goingRight)
	            {
	                transform.Rotate(Vector3.forward * 2 * angle / period * Game.dt);
	            }
	            else
	            {
	                transform.Rotate(-Vector3.forward * 2 * angle / period * Game.dt);
	            }
	        }
		}
	}

}
