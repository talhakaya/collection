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
		}

		void Update ()
	    {
	        Vector3 step = new Vector3(TaloketoInputManager.GetAxis("Horizontal"), TaloketoInputManager.GetAxis("Vertical"), 0f) * moveSpeed * Game.dt;
	        transform.position += step;
	        if (step != Vector3.zero && touchingGoal())
	        {
	            found();
	        }
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

	    // In the collection: this was OnTriggerStay2D. Unity 6 sends it only now and then
	    // while this object is being moved by its transform, and not at all once the goal's
	    // body has fallen asleep, so cutting the hole took half a minute instead of
	    // moveTime seconds. The overlap is tested directly, on the frames the player moves.
	    bool touchingGoal()
	    {
	        Collider2D mine = GetComponent<Collider2D>();
	        if (goal == null || mine == null)
	        {
	            return false;
	        }
	        Physics2D.SyncTransforms();
	        foreach (Collider2D other in goal.GetComponents<Collider2D>())
	        {
	            if (mine.Distance(other).isOverlapped)
	            {
	                return true;
	            }
	        }
	        return false;
	    }

	    void found()
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
