using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Golfinity
{
	public class Flag : MonoBehaviour
	{
	    private Animator anim;
	    private float timer;
	    private float period = 3f;

	    void Awake()
	    {
	        anim = GetComponent<Animator>();
	        timer = period;
	    }

	    private void Update()
	    {
	        timer += Time.deltaTime;
	    }

	    private void OnTriggerEnter2D(Collider2D collision)
	    {
	        OnTrigger(collision);
	    }

	    private void OnTriggerExit2D(Collider2D collision)
	    {
	        OnTrigger(collision);
	    }

	    private void OnTrigger(Collider2D collision)
	    {
	        if (timer < period) return;
	        if (collision.gameObject.name == "ball")
	        {
	            timer = 0f;
	            anim.Play("Shake");
	        }
	    }
	}

}
