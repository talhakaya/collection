using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.KayabrosPrototype0
{
	public class Music : MonoBehaviour {
	    public static Music instance;
	    private AudioSource audioSource;

		void Start () {
			if (instance == null) {
	            instance = this;
	            Collection.Controls.PortHelpers.KeepWithinGame(gameObject); // In the collection: was DontDestroyOnLoad
	            audioSource = GetComponent<AudioSource>();
	        }
	        else {
	            Destroy(gameObject);
	        }
		}

		void Update () {
	        audioSource.volume = Game.failed ? 0f : 1f;
	        audioSource.pitch = Game.instance.player.fireRatio + 1f;
	    }
	}
}
