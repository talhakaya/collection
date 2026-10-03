using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.KayabrosPrototype0
{
	public class Wall : MonoBehaviour {
	    private float scaleZ;
	    private bool dead;

		void Start () {
	        scaleZ = transform.localScale.z;
	    }

		void Update () {
	        if (dead) {
	            if (transform.localScale.z <= 0f) {
	                Destroy(gameObject);
	            }
	            else {
	                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z - Game.dt * 5f);
	            }
	        }
	        else {
	            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, scaleZ * Game.rhythm);
	        }
		}

	    public void Destroy() {
	        GetComponent<Collider2D>().enabled = false;
	        dead = true;
	    }

	    public bool Destroyable() {
	        return transform.localScale.x + transform.localScale.y < Mathf.Max(Game.roomWidth, Game.roomHeight) + 2f;
	    }
	}
}
