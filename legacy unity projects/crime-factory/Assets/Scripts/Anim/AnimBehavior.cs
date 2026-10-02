using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimBehavior : MonoBehaviour {
    protected Sprite[] _sprites;
    protected Sprite[] sprites {
        get {
            return _sprites;
        }
        set {
            if (_sprites != value) {
                animTimer = 0f;
                _sprites = value;
            }
        }
    }
    protected float animTimer;
    protected SpriteRenderer sr;
    protected float period = 0.15f;
    protected AudioClip[] sounds;
    protected float[] soundTimings;

    protected void Start () {
        sr = GetComponent<SpriteRenderer>();
    }
    
    protected void Update () {
        if (Simulator.IsPaused || sprites.Length == 0) return;
        int index = Mathf.FloorToInt((animTimer % (period * sprites.Length)) / period);
        sr.sprite = sprites[index];
        animTimer += Time.deltaTime;
        animTimer = animTimer % (period * sprites.Length);
        if (sounds != null && soundTimings != null && sounds.Length > 0 && sounds.Length == soundTimings.Length) {
            for (int i = 0, len = sounds.Length; i < len; i++) {
                if (soundTimings[i] < animTimer && soundTimings[i] >= animTimer - Time.deltaTime) {
                    AudioPlayer.instance.Play(sounds[i]);
                }
            }
        }
    }
}
