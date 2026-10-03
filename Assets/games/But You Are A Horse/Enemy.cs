using UnityEngine;
using System.Collections;

namespace Games.ButYouAreAHorse
{
	public class Enemy : MonoBehaviour
	{
	    public bool isRight;
	    public float speedH;
	    public float maxSpeedV;
	    private float speedV;
	    public float jumpForceFirst;
	    public float jumpForceCont;
	    private int onGround = 0;
	    private Rigidbody2D rigidbody2D;
	    public Animator animator;
	    public bool isDead;
	    public bool activeEnemy;
	    public bool jumper;
	    private int onGroundTimer;
	    private int onGroundPerFrame;
	    private int health;
	    private float gettingHitTimer;
	    private float hitSpeedH;

		void Start ()
	    {
	        health = 3;
	        rigidbody2D = GetComponent<Rigidbody2D>();
	        SpriteEffect.make(Effect.RGBSplit, animator.gameObject, false, true, Camera.main.transform);
	        foreach (Transform child in animator.transform)
	        {
	            SpriteEffect.make(Effect.RGBSplit, child.gameObject, false, true, Camera.main.transform);
	        }
		}

	    public void getHit()
	    {
	        if (gettingHitTimer <= 0)
	        {
	            if (transform.position.x > PlatformerController.instance.position.x)
	            {
	                hitSpeedH = 15f / Geometry.lengthOfVector3(transform.position - PlatformerController.instance.position);
	            }
	            else
	            {
	                hitSpeedH = -15f / Geometry.lengthOfVector3(transform.position - PlatformerController.instance.position);
	            }
	            gettingHitTimer = 1f;
	            health--;
	            if (health <= 0)
	            {
	                rigidbody2D.freezeRotation = false; // In the collection: was fixedAngle
	                rigidbody2D.AddTorque(hitSpeedH);
	            }
	        }
	    }

		void Update ()
	    {
	        if (transform.position.y < -6)
	        {
	            Destroy(gameObject);
	        }

	        if (gettingHitTimer > 0)
	        {
	            gettingHitTimer -= Game.dt;
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

	        speedV = rigidbody2D.linearVelocity.y;
	        speedV -= Game.dt * 19.87f;
	        float horizontalAxis = 0;
	        if (activeEnemy && PlatformerController.instance != null && health > 0)
	        {
	            if (Geometry.lengthOfVector3(PlatformerController.instance.transform.position - transform.position) < 4f)
	            {
	                if (jumper)
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
	                horizontalAxis = (PlatformerController.instance.transform.position.x > transform.position.x) ? 1f : -1f;
	            }

	        }

	        // anim.enabled = (horizontalAxis != 0);
	        if (gettingHitTimer > 0)
	        {
	            rigidbody2D.linearVelocity = new Vector2(gettingHitTimer * hitSpeedH, Mathf.Clamp(speedV, -maxSpeedV, maxSpeedV));
	        }
	        else
	        {
	            rigidbody2D.linearVelocity = new Vector2(horizontalAxis * speedH, Mathf.Clamp(speedV, -maxSpeedV, maxSpeedV));
	        }


	        if (horizontalAxis < 0f)
	        {
	            isRight = false;
	        }
	        else if (horizontalAxis > 0f)
	        {
	            isRight = true;
	        }

	        if (health <= 0)
	        {
	            PlayAnim("enemyDead");
	        }
	        else if (onGround == 0)
	        {
	            PlayAnim("enemyJump");
	        }
	        else if (horizontalAxis != 0f)
	        {
	            PlayAnim("enemyWalk");
	        }
	        else
	        {
	            PlayAnim("enemyIdle");
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
	        return p.y < transform.position.y - 1.65f && Mathf.Abs(p.x - transform.position.x) < 0.25f;
	    }
	}
}
