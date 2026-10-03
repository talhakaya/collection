using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.CasketFucker
{
	public class MoveToFind : MonoBehaviour
	{
	    public float moveSpeed = 1f;
	    public float moveTime;
	    public GameObject goal;
	    private float audioTime;
	    private AudioSource audioSource;

		void Start ()
	    {
	        audioSource = GetComponent<AudioSource>();

	        // In the collection: the goal's body falls asleep, and a sleeping body sends no
	        // OnTriggerStay2D, so holding still on the goal stopped counting. It is kept awake.
	        if (goal != null && goal.GetComponent<Rigidbody2D>() != null)
	        {
	            goal.GetComponent<Rigidbody2D>().sleepMode = RigidbodySleepMode2D.NeverSleep;
	        }
		}

		void Update ()
	    {
	        transform.position += new Vector3(TaloketoInputManager.GetAxis("Horizontal"), TaloketoInputManager.GetAxis("Vertical"), 0f) * moveSpeed * Game.dt;
		    if (audioSource != null)
	        {
	            if (audioTime > 0)
	            {
	                audioSource.enabled = true;
	                audioTime -= Game.dt;
	            }
	            else
	            {
	                audioSource.enabled = false;
	            }
	        }
		}

	    void OnTriggerStay2D(Collider2D other)
	    {
	        if (other.gameObject == goal)
	        {
	            audioTime = Game.dt * 10f;
	            moveTime -= Game.dt;
	            if (moveTime < 0)
	            {
	                State.next();
	            }
	        }
	    }
	}
}
