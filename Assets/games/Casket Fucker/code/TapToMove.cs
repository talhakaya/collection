using UnityEngine;
using System.Collections;

namespace Games.CasketFucker
{
	public class TapToMove : MonoBehaviour {

	    public bool autoTapped;
	    public float dtMultiplier;
	    public Vector3 direction;
	    public float rotation;
	    public float nextState;
	    private float stateCounter;
	    private float time;


		void Start ()
	    {

		}

		void Update ()
	    {
	        if (Game.anyKeyDown || autoTapped)
	        {
	            time += Game.dt * dtMultiplier;
	        }

	        if (time > 0f)
	        {
	            time -= Game.dt;
	            transform.position += direction * Game.dt;
	            transform.eulerAngles += Vector3.forward * rotation * Game.dt;
	            stateCounter += Game.dt;
	            if (stateCounter >= nextState)
	            {
	                State.next();
	            }
	        }
		}
	}
}
