using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.CrimeFactory
{
	public class TutorialSign : MonoBehaviour {
	    public Sprite controller;
	    public Sprite keyboard;
	    private SpriteRenderer sr;

	    void Start () {
	        sr = GetComponent<SpriteRenderer>();
		}

		void Update () {
	        sr.sprite = Platformer.isUsingController ? controller : keyboard;
	    }
	}
}
