using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.MilkyMike
{
	public class TiledBoxCollider : MonoBehaviour {
	    public bool callOnValidate = true;

		void OnValidate () {
	        if (callOnValidate) {
	            callOnValidate = false;
	            GetComponent<BoxCollider2D>().size = GetComponent<SpriteRenderer>().size;
	        }
		}
	}
}
