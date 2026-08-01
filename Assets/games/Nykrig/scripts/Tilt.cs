using UnityEngine;
using System.Collections;

namespace Games.Nykrig
{
	public class Tilt : MonoBehaviour {

	    private bool goingRight;
	    private float timeCounter;
	    public float period = 1f;
	    public float angle = 5f;

		void Start ()
	    {
		    if (Random.value < 0.5f)
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

	        if (timeCounter >= period)
	        {
	            timeCounter -= period;
	            goingRight = !goingRight;
	        }

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
