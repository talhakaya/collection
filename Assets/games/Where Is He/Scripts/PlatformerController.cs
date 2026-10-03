using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.WhereIsHe
{
	public class PlatformerController : MonoBehaviour {

	    public bool isRight;
	    public float speedH;
	    public float maxSpeedV;
	    private float speedV;
	    public float jumpForceFirst;
	    public float jumpForceCont;
	    private Animator anim;
	    private int onGround = 0;
	    private Rigidbody2D rigidbody2D;
	    public Transform noRotation;
	    public TintScript sprite;
	    public bool grounded;
	    public bool lefted;
	    public bool righted;
	    public Transform groundCheck;
	    public Transform leftCheck;
	    public Transform rightCheck;
	    public float accelerationH;
	    public float accelerationOnAirH;
	    public float frictionH;
	    public float wallJumpSpeedMultiplier;
	    public AudioClip jumpAudio;
	    public AudioClip hitGroundAudio;

		void Start ()
	    {
	        anim = sprite.GetComponent<Animator>();
	        anim.speed = 0f;
	        rigidbody2D = GetComponent<Rigidbody2D>();
	        SpriteEffect.make(Effect.RGBSplit, sprite.gameObject, false, true, Camera.main.transform);
		}

		void Update ()
	    {
	        float scaleX = 1f;
	        anim.speed = Mathf.Abs(GetComponent<Rigidbody2D>().linearVelocity.x * 0.25f);
	        bool groundedOld = grounded || lefted || righted;
	        grounded = Physics2D.Linecast(transform.position, groundCheck.position, Game.GroundMask);
	        lefted = Physics2D.Linecast(transform.position, leftCheck.position, Game.GroundMask);
	        righted = Physics2D.Linecast(transform.position, rightCheck.position, Game.GroundMask);
	        if (!groundedOld && (grounded || lefted || righted))
	        {
	            AudioSource.PlayClipAtPoint(hitGroundAudio, Camera.main.transform.position);
	        }
	        speedV = GetComponent<Rigidbody2D>().linearVelocity.y;
	        speedV -= Game.dt * 19.87f;
	        bool jumpPressing = TaloketoInputManager.GetButton("Jump");
	        bool jumpPressed = TaloketoInputManager.GetButtonDown("Jump");
	        if (jumpPressing)
	        {
	            if (jumpPressed)
	            {
	                if (grounded)
	                {
	                    speedV = jumpForceFirst;
	                    AudioSource.PlayClipAtPoint(jumpAudio, Camera.main.transform.position);
	                }
	                else if (lefted)
	                {
	                    speedV = jumpForceFirst;
	                    GetComponent<Rigidbody2D>().linearVelocity = new Vector2(speedH * wallJumpSpeedMultiplier, GetComponent<Rigidbody2D>().linearVelocity.y);
	                    AudioSource.PlayClipAtPoint(jumpAudio, Camera.main.transform.position);
	                }
	                else if (righted)
	                {
	                    speedV = jumpForceFirst;
	                    GetComponent<Rigidbody2D>().linearVelocity = new Vector2(-speedH * wallJumpSpeedMultiplier, GetComponent<Rigidbody2D>().linearVelocity.y);
	                    AudioSource.PlayClipAtPoint(jumpAudio, Camera.main.transform.position);
	                }
	            }
	            else
	            {
	               speedV += jumpForceCont * Game.dt;
	            }
	        }

	        if (GetComponent<Rigidbody2D>().linearVelocity.x > 0)
	        {
	            if (GetComponent<Rigidbody2D>().linearVelocity.x > frictionH * Game.dt)
	            {
	                GetComponent<Rigidbody2D>().linearVelocity -= Vector2.right * frictionH * Game.dt;
	            }
	            else
	            {
	                GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0f, GetComponent<Rigidbody2D>().linearVelocity.y);
	            }
	        }
	        else if (GetComponent<Rigidbody2D>().linearVelocity.x < 0)
	        {
	            if (GetComponent<Rigidbody2D>().linearVelocity.x < -frictionH * Game.dt)
	            {
	                GetComponent<Rigidbody2D>().linearVelocity += Vector2.right * frictionH * Game.dt;
	            }
	            else
	            {
	                GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0f, GetComponent<Rigidbody2D>().linearVelocity.y);
	            }
	        }

	        float horizontalAxis = TaloketoInputManager.GetAxisRaw("Horizontal");

	        if (horizontalAxis > 0)
	        {
	            if (grounded)
	            {
	                if (GetComponent<Rigidbody2D>().linearVelocity.x < speedH - accelerationH * Game.dt)
	                {
	                    GetComponent<Rigidbody2D>().linearVelocity += Vector2.right * accelerationH * Game.dt;
	                }
	            }
	            else
	            {
	                if (GetComponent<Rigidbody2D>().linearVelocity.x < speedH - accelerationOnAirH * Game.dt)
	                {
	                    GetComponent<Rigidbody2D>().linearVelocity += Vector2.right * accelerationOnAirH * Game.dt;
	                }
	            }
	        }
	        else if (horizontalAxis < 0)
	        {
	            if (grounded)
	            {
	                if (GetComponent<Rigidbody2D>().linearVelocity.x > -speedH + accelerationH * Game.dt)
	                {
	                    GetComponent<Rigidbody2D>().linearVelocity -= Vector2.right * accelerationH * Game.dt;
	                }
	            }
	            else
	            {
	                if (GetComponent<Rigidbody2D>().linearVelocity.x > -speedH + accelerationOnAirH * Game.dt)
	                {
	                    GetComponent<Rigidbody2D>().linearVelocity -= Vector2.right * accelerationOnAirH * Game.dt;
	                }
	            }
	        }

	        GetComponent<Rigidbody2D>().linearVelocity = new Vector2(GetComponent<Rigidbody2D>().linearVelocity.x, Mathf.Clamp(speedV, -maxSpeedV, maxSpeedV));
	        if (horizontalAxis < 0f)
	        {
	            isRight = false;
	        }
	        else if (horizontalAxis > 0f)
	        {
	            isRight = true;
	        }

	        noRotation.eulerAngles = Vector3.zero;
	        scaleX = 1f - 0.1f * Mathf.Abs(GetComponent<Rigidbody2D>().linearVelocity.y / maxSpeedV) + 0.1f * Mathf.Abs(GetComponent<Rigidbody2D>().linearVelocity.x / speedH);
	        sprite.transform.localScale = new Vector3(Mathf.Abs(sprite.transform.localScale.x) / sprite.transform.localScale.x * scaleX, 2f - scaleX, sprite.transform.localScale.z);
	        if (grounded)
	        {
	            if (isRight)
	            {
	                sprite.transform.localScale = new Vector3(Mathf.Abs(sprite.transform.localScale.x), sprite.transform.localScale.y, sprite.transform.localScale.z);
	            }
	            else
	            {
	                sprite.transform.localScale = new Vector3(-Mathf.Abs(sprite.transform.localScale.x), sprite.transform.localScale.y, sprite.transform.localScale.z);
	            }
	            sprite.transform.eulerAngles = Vector3.zero;
	        }
	        else if (lefted)
	        {
	            sprite.transform.eulerAngles = -Vector3.forward * 90f;
	        }
	        else if (righted)
	        {
	            sprite.transform.eulerAngles = Vector3.forward * 90f;
	        }
	        else
	        {
	            sprite.transform.eulerAngles += 150f * Vector3.forward * Game.dt * horizontalAxis;
	        }

	        GetComponent<PlayerScript>().ForceUpdate();
	    }

	    void OnCollisionEnter2D(Collision2D other)
	    {
	        LineManager lm = other.gameObject.GetComponent<LineManager>();
	        if (lm != null)
	        {
	            lm.rgbSplitOffset += 0.2f;
	        }
	        if (other.contacts[0].point.y < transform.position.y - 0.25f)
	        {
	            onGround++;
	            grounds.Add(other.collider);
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
