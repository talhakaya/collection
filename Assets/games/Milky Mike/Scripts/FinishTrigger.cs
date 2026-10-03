using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.MilkyMike
{
	public class FinishTrigger : MonoBehaviour {
	    [HideInInspector] public Level level;
	    public bool ed;
	    private float timer;
	    private bool ing;
	    private SpriteRenderer spriteRenderer;

	    void Start() {
	        spriteRenderer = GetComponent<SpriteRenderer>();
	    }

	    public void ResetState() {
	        ed = false;
	        ing = false;
	        timer = 0f;
	    }

	    void Update() {
	        if (ing) {
	            timer += Game.dt;
	            if (timer >= 0.5f) {
	                ed = true;
	            }
	        }
	        spriteRenderer.color = (ed || level.CanFinish()) ? new Color(1f, 1f, 1f, 0.5f) : new Color(0.5f, 0.5f, 0.5f, 0.5f);
	        if (ing) {
	            transform.localScale += new Vector3(0f, Game.dt, 0f);
	        }
	        else {
	            transform.localScale = new Vector3(1f - Game.Rhythm(1f) * 0.2f, 1f + Game.Rhythm(1f) * 0.2f, 1f);
	        }
	    }

	    void OnTriggerEnter2D(Collider2D other) {
	        if (!ing && level != null && level.CanFinish() && other.CompareTag("Player")) {
	            ing = true;
	        }
	    }

	    void OnTriggerStay2D(Collider2D other) {
	        OnTriggerEnter2D(other);
	    }
	}
}
