using UnityEngine;
using System.Collections;

namespace Games.MilkyMike
{
	public class Tilt : MonoBehaviour {
	    public bool active = true;
	    private bool goingRight;
	    private float timeCounter;
	    public float period = 1f;
	    public float angle = 5f;
	    public bool forceRight;
	    public bool forceLeft;
	    private float first;
	    public Transform aimAt;

	    void Start ()
	    {
	        first = transform.localEulerAngles.z;
	        if (!forceLeft && (Random.value < 0.5f || forceRight))
	        {
	            goingRight = true;
	        }
	        else
	        {
	            goingRight = false;
	        }
	    }

		void Update ()
	    {
	        if (aimAt != null) {
	            transform.eulerAngles = new Vector3(0f, 0f, Geometry.angleOfVector3(aimAt.position - transform.position) + 90f);
	        }
	        else if (active) {
	            timeCounter += Game.dt;

	            if (timeCounter >= period) {
	                timeCounter = 0f;
	                goingRight = !goingRight;
	            }

	            float current = first + (goingRight ? -1f : 1f) * angle + (goingRight ? 2f : -2f) * (angle * timeCounter / period);

	            transform.localEulerAngles = new Vector3(0f, 0f, current);
	        }
	        else {
	            timeCounter -= timeCounter * Game.dt;
	            transform.localEulerAngles = new Vector3(0f, 0f, transform.localEulerAngles.z + Geometry.differenceOfAnglesNegative(first, transform.localEulerAngles.z) * Game.dt * 4f);
	        }
		}
	}
}
