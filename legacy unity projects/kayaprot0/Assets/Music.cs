using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour {
    public static Music instance;
    private AudioSource audioSource;
    
	void Start () {
		if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
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
