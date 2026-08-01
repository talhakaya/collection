using UnityEngine;
using System.Collections;

namespace Games.Nykrig
{
	public class NightPill : MonoBehaviour {
	    public bool isDead;
	    public bool nextLevel;

	    void OnEnable() {
	        transform.localScale = new Vector3(0.5f, 0.5f, 1f);
	        transform.eulerAngles = new Vector3(0f, 0f, 0f);
	    }
		
		void Update () {
	        if (isDead) {
	            transform.localScale -= new Vector3(1f, 1f, 0f) * Game.dt * 4f;
	            if (transform.localScale.x <= 0f) {
	                gameObject.SetActive(false);
	            }
	        }
	        else {
	            transform.eulerAngles += new Vector3(0f, 0f, Game.dt * 120f);
	        }
		}

	    void OnTriggerEnter2D(Collider2D other) {
	        if (!isDead && other.tag == "Player") {
	            if (nextLevel) {
	                Game.enemyCount = 0;
	            }
	            else {
	                Player.nightVisionTimer = Player.nightVisionPeriod;
	            }
	            isDead = true;
	        }
	    }
	}

}
