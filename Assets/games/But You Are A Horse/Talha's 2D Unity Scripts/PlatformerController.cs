using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.ButYouAreAHorse
{
	public class PlatformerController : MonoBehaviour {

	    public static Transform instance;
	    public bool isRight;
	    public float speedH;
	    public float maxSpeedV;
	    private float speedV;
	    public float jumpForceFirst;
	    public float jumpForceCont;
	    // private TalhaAnimation anim;
	    private int onGround = 0;
	    private Rigidbody2D rigidbody2D;
	    public Animator animator;
	    public bool happy;
	    private int onGroundTimer;
	    private int onGroundPerFrame;
	    private float jumpTimeCounter;
	    public Collider2D hitCollider;
	    public float hitTimeCounter;

	    void Awake ()
	    {
	        instance = transform;
	    }

		void Start ()
	    {
	        // anim = GetComponent<TalhaAnimation>();
	        rigidbody2D = GetComponent<Rigidbody2D>();
	        SpriteEffect.make(Effect.RGBSplit, animator.gameObject, false, true, Camera.main.transform);
	        foreach (Transform child in animator.transform)
	        {
	            SpriteEffect.make(Effect.RGBSplit, child.gameObject, false, true, Camera.main.transform);
	        }
		}

		void Update ()
	    {
	        if (transform.position.y < -6)
	        {
	            transform.position = new Vector3(12.8f * Mathf.FloorToInt((transform.position.x + 6.4f) / 12.8f) - 12.8f, 6f, transform.position.z);
	        }


	        if (onGroundPerFrame != onGround)
	        {
	            onGroundTimer++;
	            if (onGroundTimer > 5)
	            {
	                onGround = onGroundPerFrame;
	            }
	        }
	        else
	        {
	            onGroundTimer = 0;
	        }
	        onGroundPerFrame = 0;

	        if (jumpTimeCounter > 0f)
	        {
	            jumpTimeCounter -= Game.dt;
	        }

	        speedV = rigidbody2D.linearVelocity.y;
	        speedV -= Game.dt * 19.87f;

	        if (TaloketoInputManager.GetButton("Up"))
	        {
	            //PlayAnim("horseJump");
	            if (onGround > 0 && jumpTimeCounter <= 0f)
	            {
	                speedV = jumpForceFirst;
	                jumpTimeCounter = 0.5f;
	            }
	            else
	            {
	                speedV += jumpForceCont * Game.dt;
	            }
	        }
	        float horizontalAxis = TaloketoInputManager.GetAxisRaw("Horizontal");

	        rigidbody2D.linearVelocity = new Vector2(horizontalAxis * speedH, Mathf.Clamp(speedV, -maxSpeedV, maxSpeedV));

	        if (horizontalAxis < 0f)
	        {
	            isRight = false;
	        }
	        else if (horizontalAxis > 0f)
	        {
	            isRight = true;
	        }
	        hitCollider.enabled = false;
	        if (TaloketoInputManager.GetButton("Jump"))
	        {
	            PlayAnim("horseHit");
	            hitTimeCounter += Game.dt;
	            if (hitTimeCounter >= 0.05f && hitTimeCounter < 0.15f)
	            {
	                hitCollider.enabled = true;
	            }
	        }
	        else if (onGround == 0)
	        {
	            PlayAnim("horseJump");
	        }
	        else if (horizontalAxis != 0f)
	        {
	            PlayAnim("horseWalk");
	        }
	        else
	        {
	            PlayAnim("horseIdle");
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

	    private System.Collections.Generic.HashSet<Collider2D> groundContacts = new System.Collections.Generic.HashSet<Collider2D>();

	    void OnCollisionEnter2D(Collision2D other)
	    {
	        if (collisionPlatform(other.contacts[0].point))
	        {
	                groundContacts.Add(other.collider);
	            onGround++;
	        }
	    }

	    void OnCollisionStay2D(Collision2D other)
	    {
	        if (collisionPlatform(other.contacts[0].point))
	        {
	            onGroundPerFrame++;
	        }
	    }

	    void OnCollisionExit2D(Collision2D other)
	    {
	        // In the collection: Unity 6 gives no contact points when a collision ends, so the
	        // colliders that counted as ground on entering are remembered instead.
	        if (groundContacts.Remove(other.collider))
	        {
	            onGround--;
	            if (onGround < 0)
	            {
	                onGround = 0;
	            }
	        }
	    }

	    string currentAnim;
	    void PlayAnim(string animName)
	    {
	        if (currentAnim != animName)
	        {
	            currentAnim = animName;
	            animator.Play(animName);
	            //animator.CrossFade(animName, 0.2f);
	        }
	    }

	    bool collisionPlatform(Vector2 p)
	    {
	        return p.y < transform.position.y - 1.65f;// && Mathf.Abs(p.x - transform.position.x) < 0.25f;
	    }

	    void resetHitAnim()
	    {
	        hitTimeCounter = 0f;
	    }

	    void OnTriggerEnter2D(Collider2D other)
	    {
	        if (other.name.Contains("enemy"))
	        {
	            other.GetComponent<Enemy>().getHit();
	        }
	    }
	}
}
