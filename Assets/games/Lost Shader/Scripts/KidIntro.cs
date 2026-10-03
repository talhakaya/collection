using UnityEngine;
using System.Collections;

namespace Games.LostShader
{
	public class KidIntro : MonoBehaviour
	{
	    private enum State
	    {
	        GoingRight,
	        Waiting,
	        GoingLeft
	    }
	    private State state;
	    private const float MaxX = 12f;
	    public float speed;
	    private float timer;
	    public int noOfTurnsToProgress;
	    public GameObject objectToEnable;
	    public bool destroyYourself;

		void Start ()
	    {
	        state = State.GoingRight;
	        transform.position = new Vector3(-MaxX, transform.position.y, transform.position.z);
	        timer = 0f;
	        if (objectToEnable != null)
	        {
	            objectToEnable.SetActive(false);
	        }
		}

		void Update ()
	    {
	        GetComponent<Animator>().speed = Game.timeSpeed;
	        if (state == State.GoingRight)
	        {
	            transform.position += Vector3.right * speed * Game.dt;
	            if (transform.position.x >= MaxX)
	            {
	                transform.localScale = new Vector3(-1f * transform.localScale.x, transform.localScale.y, transform.localScale.z);
	                state = State.Waiting;
	                checkNoOfTurnsToProgress();
	            }
	        }
	        else if (state == State.Waiting)
	        {
	            timer += Game.dt;
	            if (timer >= 2f)
	            {
	                if (transform.position.x > 0f)
	                {
	                    state = State.GoingLeft;
	                    transform.position = new Vector3(MaxX, transform.position.y, transform.position.z);
	                }
	                else
	                {
	                    state = State.GoingRight;
	                    transform.position = new Vector3(-MaxX, transform.position.y, transform.position.z);
	                }
	            }
	        }
	        else if (state == State.GoingLeft)
	        {
	            transform.position += Vector3.left * speed * Game.dt;
	            if (transform.position.x <= -MaxX)
	            {
	                transform.localScale = new Vector3(-1f * transform.localScale.x, transform.localScale.y, transform.localScale.z);
	                state = State.Waiting;
	                checkNoOfTurnsToProgress();
	            }
	        }
		}

	    public void checkNoOfTurnsToProgress()
	    {
	        noOfTurnsToProgress--;
	        if (noOfTurnsToProgress <= 0)
	        {
	            if (objectToEnable != null)
	            {
	                objectToEnable.SetActive(true);
	            }
	            if (destroyYourself)
	            {
	                Destroy(gameObject);
	            }
	        }
	    }
	}
}
