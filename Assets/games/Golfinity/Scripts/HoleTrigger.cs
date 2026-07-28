using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Games.Golfinity
{
	public class HoleTrigger : MonoBehaviour
	{
	    public static Vector3 pos;
	    private bool triggered;
	    private float time;

		void Start ()
	    {
	        pos = transform.position;
		}
		
		// Update is called once per frame
		void Update ()
	    {
		    if (triggered)
	        {
	            float timeOld = time;
	            time += Game.dt;
	            if (timeOld < 1f && time >= 1f)
	            {
	                UIReferences.levelScorePopup.Show();
	            }
	        }
		}

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
	            triggered = true;
	            if (Game.circleHoleEffectOn)
	            {
	                CircleParticle.create(5, transform.position - Vector3.forward * 2f);
	            }
	            Game.instance.SoundHole();
	        }
	    }
	}

}
