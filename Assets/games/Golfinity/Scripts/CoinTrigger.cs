using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Golfinity
{
	public class CoinTrigger : MonoBehaviour
	{
	    private bool triggered;

	    void OnTriggerEnter2D(Collider2D other)
	    {
	        trigger(other);
	    }

	    void OnTriggerStay2D(Collider2D other)
	    {
	        trigger(other);
	    }

	    void trigger(Collider2D other)
	    {
	        if (other.gameObject == GolfBall.instance.gameObject && !triggered)
	        {
	            LevelGenerator.NumCoinsCollected++;
	            triggered = true;
	            Vector3 screenPos = Game.cam.WorldToScreenPoint(transform.position);
	            CoinIcon.Create(1, screenPos);
	            Destroy(gameObject);
	        }
	    }
	}

}
