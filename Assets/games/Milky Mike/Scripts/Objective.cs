using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.MilkyMike
{
	public class Objective : MonoBehaviour {
	    public bool done;
	    public bool disableOnFinish;

	    public void ResetState() {
	        done = false;
	        gameObject.SetActive(true);
	        Rigidbody2D body = GetComponent<Rigidbody2D>();
	        body.linearVelocity = new Vector2(0f, 0f);
	        body.angularVelocity = 0f;
	    }
	}
}
