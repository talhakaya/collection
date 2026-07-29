using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Golfinity
{
	public class Music : MonoBehaviour
	{
	    public static Music instance;
	    public AudioSource audioSource;

	    void Awake() {
	        instance = this;
	        if (Game.musicOn) audioSource.Play();
	    }
	}

}
