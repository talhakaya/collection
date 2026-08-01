using UnityEngine;
using System.Collections;

namespace Games.Nykrig
{
	public class Scale : MonoBehaviour {

	    private bool goingUp;
	    private float timeCounter;
	    public float period = 0.5f;
	    public float deltaScale = 0.25f;
	    public bool controlStart;
	    public bool startByGoingUp;

	    void Start()
	    {
	        if (controlStart) {
	            if (startByGoingUp) {
	                goingUp = true;
	                transform.localScale -= Vector3.one * deltaScale;
	            }
	            else {
	                goingUp = false;
	                transform.localScale += Vector3.one * deltaScale;
	            }
	        }
	        else {
	            if (Random.value < 0.5f) {
	                goingUp = true;
	                transform.localScale -= Vector3.one * deltaScale;
	            }
	            else {
	                goingUp = false;
	                transform.localScale += Vector3.one * deltaScale;
	            }
	        }
	    }

	    void Update()
	    {
	        timeCounter += Game.dt;

	        if (timeCounter >= period)
	        {
	            timeCounter -= period;
	            goingUp = !goingUp;
	        }

	        if (goingUp)
	        {
	            transform.localScale += Vector3.one * 2 * deltaScale / period * Game.dt;
	        }
	        else
	        {
	            transform.localScale -= Vector3.one * 2 * deltaScale / period * Game.dt;
	        }
	    }
	}

}
