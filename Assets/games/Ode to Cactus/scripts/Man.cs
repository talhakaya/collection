using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.OdeToCactus
{
	public class Man : MonoBehaviour {

	    public bool isRight;
	    public float speedH;
	    public float maxSpeedV;
	    private float speedV;
	    public float jumpForceFirst;
	    public float jumpForceCont;
	    private TalhaAnimation anim;
	    private int onGround = 0;
	    public Transform fallPlace;
	    public AudioSource aWalk;
	    public AudioSource aFall;
	    private float walkCounter;
	    private bool fallPlayed;
	    public bool norr2;
	    public bool movementEnabled;
	    private float man2Counter;
	    private bool manShot;
	    public GameObject man2;
	    public GameObject whiteScreen;

		void Start ()
	    {
	        anim = GetComponent<TalhaAnimation>();
	        aWalk = GetComponent<AudioSource>();
	        movementEnabled = true;
		}

		void Update ()
	    {
	        if (movementEnabled)
	        {
	            speedV = GetComponent<Rigidbody2D>().linearVelocity.y;
	            speedV -= Game.dt * 19.87f;

	            if (TaloketoInputManager.GetButton("Up")) // In the collection: was W or the up arrow
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
	            anim.enabled = (horizontalAxis != 0);
	            if (horizontalAxis != 0 && onGround > 0)
	            {
	                if (walkCounter <= 0)
	                {
	                    aWalk.Play();
	                    walkCounter += 0.2f;
	                }
	                else
	                {
	                    walkCounter -= Game.dt;
	                }
	            }
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

	        if (!norr2 && transform.position.y > -189f)
	        {
	            fallPlace.transform.position = new Vector3(transform.position.x, fallPlace.transform.position.y, fallPlace.transform.position.z);
	        }

	        if (norr2 && man2 != null)
	        {
	            if (movementEnabled)
	            {
	                if (onGround > 0 && transform.position.x > -3.7f)
	                {
	                    movementEnabled = false;
	                    GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
	                    anim.enabled = false;
	                    GetComponent<Collider2D>().isTrigger = true;
	                }
	            }
	            else
	            {
	                man2Counter += Game.dt;
	                if (!manShot && man2Counter >= 3f)
	                {
	                    man2Counter = 0f;
	                    manShot = true;
	                    transform.Rotate(Vector3.forward * 90f);
	                    transform.position += Vector3.down * 1.05f;
	                    aFall.Play();
	                    whiteScreen.SetActive(true);
	                }
	                else if (man2Counter >= 0.1f && man2Counter < 3f)
	                {
	                    whiteScreen.SetActive(false);
	                }
	                else if (man2Counter >= 3f && man2 != null)
	                {
	                    man2.GetComponent<Man>().enabled = true;
	                    man2 = null;
	                    this.enabled = false;
	                }
	            }
	        }

	        Camera.main.transform.position = new Vector3(16 * Mathf.Ceil((transform.position.x - 8f) / 16f), 9 * Mathf.Ceil((transform.position.y - 4.5f) / 9f), -10f);
	    }

	    void OnCollisionEnter2D(Collision2D other)
	    {
	        if (other.contacts[0].point.y < transform.position.y + 0.5f)
	        {
	            onGround++;
	            grounds.Add(other.collider);

	            if (!norr2 && transform.position.y < -189f && !fallPlayed)
	            {
	                aFall.Play();
	                fallPlayed = true;
	            }
	        }
	    }

	    // In the collection: a collision that has ended has no contact points left in Unity 6
	    // (contacts[0] threw), so which colliders counted as ground is remembered on entering.
	    private System.Collections.Generic.HashSet<Collider2D> grounds = new System.Collections.Generic.HashSet<Collider2D>();

	    void OnCollisionExit2D(Collision2D other)
	    {
	        if (grounds.Remove(other.collider))
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
