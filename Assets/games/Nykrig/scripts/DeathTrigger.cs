using UnityEngine;
using System.Collections;

namespace Games.Nykrig
{
	public class DeathTrigger : MonoBehaviour {
	    public Enemy enemy;

	    void OnTriggerEnter2D(Collider2D other) {
	        if (enemy != null && enemy.isDead) return;
	        if (other.tag == "Player") {
	            other.GetComponent<Player>().Die();
	        }
	    }

	    void OnCollisionEnter2D(Collision2D other) {
	        if (enemy != null && enemy.isDead) return;
	        if (other.gameObject.tag == "Player") {
	            other.gameObject.GetComponent<Player>().Die();
	        }
	    }
	}

}
