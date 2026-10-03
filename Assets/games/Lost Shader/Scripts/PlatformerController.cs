using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.LostShader
{
	public class PlatformerController : MonoBehaviour {

	    public bool isRight;
	    public float speedH;
	    public float maxSpeedV;
	    private float speedV;
	    public float jumpForceFirst;
	    public float jumpForceCont;
	    // private TalhaAnimation anim;
	    private int onGround = 0;
	    private Rigidbody2D rigidbody2D;

		void Start ()
	    {
	        // anim = GetComponent<TalhaAnimation>();
	        rigidbody2D = GetComponent<Rigidbody2D>();
		}

		void Update ()
	    {
	        speedV = GetComponent<Rigidbody2D>().linearVelocity.y;
	        speedV -= Game.dt * 19.87f;
	        if (false) // In the collection: was Z or Space, an empty branch
	        {

	        }
	        if (TaloketoInputManager.GetAxisRaw("Vertical") > 0.5f) // In the collection: was W or the up arrow
	        {
	            if (onGround > 0)
	            {
	                speedV = jumpForceFirst;

	            }
	            else
	            {
	               speedV += jumpForceCont * Game.dt;
	            }
	        }
	        float horizontalAxis = TaloketoInputManager.GetAxisRaw("Horizontal");
	        // anim.enabled = (horizontalAxis != 0);
	        GetComponent<Rigidbody2D>().linearVelocity = new Vector2(horizontalAxis * speedH, Mathf.Clamp(speedV, -maxSpeedV, maxSpeedV));

	        if (horizontalAxis < 0f)
	        {
	            isRight = false;
	        }
	        else if (horizontalAxis > 0f)
	        {
	            isRight = true;
	        }
	        if (isRight)
	        {
	            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
	        }
	        else
	        {
	            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
	        }
	    }

	    void OnCollisionEnter2D(Collision2D other)
	    {
	        if (other.contacts[0].point.y < transform.position.y + 0.5f)
	        {
	            onGround++;
	        }
	    }

	    void OnCollisionExit2D(Collision2D other)
	    {
	        if (other.contacts[0].point.y < transform.position.y + 0.5f)
	        {
	            onGround--;
	            if (onGround < 0)
	            {
	                onGround = 0;
	            }
	        }
	    }
	}
}
