using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.CrimeFactory
{
	public class Dead : MonoBehaviour {
	    private SpriteRenderer sr;
	    private float timer;
	    public Vector2 scale;
	    public Color color;

	    void Awake() {
	        sr = GetComponent<SpriteRenderer>();
		}

	    private void OnEnable() {
	        timer = 0f;
	    }

	    void Update () {
	        if (Simulator.IsPaused) return;
	        timer += Platformer.dt;
	        if (timer < 0.3f) {
	            float animRatio = timer / 0.3f;
	            transform.localScale = new Vector3(1f + animRatio, 1f + animRatio, 1f);
	            sr.color = new Color(color.r, color.g, color.b, color.a * (1f - animRatio));
	        }
	        else {
	            gameObject.SetActive(false);
	        }
	    }
	}
}
