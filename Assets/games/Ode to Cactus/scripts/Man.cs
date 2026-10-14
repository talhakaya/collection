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
	    private bool onGround;
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
	        onGround = TouchingGround();

	        if (movementEnabled)
	        {
	            speedV = GetComponent<Rigidbody2D>().linearVelocity.y;
	            speedV -= Game.dt * 19.87f;

	            if (TaloketoInputManager.GetButton("Up")) // In the collection: was W or the up arrow
	            {
	                if (onGround)
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
	            if (horizontalAxis != 0 && onGround)
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
	                if (onGround && transform.position.x > -3.7f)
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

	    // In the collection: on the ground means touching something whose surface faces up. It
	    // was any collision whose contact was below half a unit over the man's middle - every
	    // point of a body 0.8 tall, so a wall touched from the air counted as ground and Up
	    // jumped off it again. Asked of the contacts every frame, where it was a count kept
	    // from entering and leaving collisions.
	    private const float GroundFacing = 0.7f;
	    private readonly ContactPoint2D[] contacts = new ContactPoint2D[8];

	    private bool TouchingGround()
	    {
	        int count = GetComponent<Rigidbody2D>().GetContacts(contacts);
	        for (int i = 0; i < count; i++)
	        {
	            if (contacts[i].normal.y > GroundFacing)
	            {
	                return true;
	            }
	        }
	        return false;
	    }

	    void OnCollisionEnter2D(Collision2D other)
	    {
	        if (!norr2 && transform.position.y < -189f && !fallPlayed)
	        {
	            for (int i = 0; i < other.contactCount; i++)
	            {
	                if (other.GetContact(i).normal.y > GroundFacing)
	                {
	                    aFall.Play();
	                    fallPlayed = true;
	                    break;
	                }
	            }
	        }
	    }
	}
}
