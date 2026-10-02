using UnityEngine;
using System.Collections;

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
        anim.speed = Mathf.Abs(rigidbody2D.velocity.x * 0.25f);
        bool groundedOld = grounded || lefted || righted;
        grounded = Physics2D.Linecast(transform.position, groundCheck.position, 1 << LayerMask.NameToLayer("Ground"));
        lefted = Physics2D.Linecast(transform.position, leftCheck.position, 1 << LayerMask.NameToLayer("Ground"));
        righted = Physics2D.Linecast(transform.position, rightCheck.position, 1 << LayerMask.NameToLayer("Ground"));
        if (!groundedOld && (grounded || lefted || righted))
        {
            AudioSource.PlayClipAtPoint(hitGroundAudio, Camera.main.transform.position);
        }
        speedV = rigidbody2D.velocity.y;
        speedV -= Game.dt * 19.87f;
        bool jumpPressing = Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        bool jumpPressed = Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
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
                    rigidbody2D.velocity = new Vector2(speedH * wallJumpSpeedMultiplier, rigidbody2D.velocity.y);
                    AudioSource.PlayClipAtPoint(jumpAudio, Camera.main.transform.position);
                }
                else if (righted)
                {
                    speedV = jumpForceFirst;
                    rigidbody2D.velocity = new Vector2(-speedH * wallJumpSpeedMultiplier, rigidbody2D.velocity.y);
                    AudioSource.PlayClipAtPoint(jumpAudio, Camera.main.transform.position);
                }
            }
            else
            {
               speedV += jumpForceCont * Game.dt;
            }
        }

        if (rigidbody2D.velocity.x > 0)
        {
            if (rigidbody2D.velocity.x > frictionH * Game.dt)
            {
                rigidbody2D.velocity -= Vector2.right * frictionH * Game.dt;
            }
            else
            {
                rigidbody2D.velocity = new Vector2(0f, rigidbody2D.velocity.y);
            }
        }
        else if (rigidbody2D.velocity.x < 0)
        {
            if (rigidbody2D.velocity.x < -frictionH * Game.dt)
            {
                rigidbody2D.velocity += Vector2.right * frictionH * Game.dt;
            }
            else
            {
                rigidbody2D.velocity = new Vector2(0f, rigidbody2D.velocity.y);
            }
        }

        float horizontalAxis = Input.GetAxisRaw("Horizontal");

        if (horizontalAxis > 0)
        {
            if (grounded)
            {
                if (rigidbody2D.velocity.x < speedH - accelerationH * Game.dt)
                {
                    rigidbody2D.velocity += Vector2.right * accelerationH * Game.dt;
                }
            }
            else
            {
                if (rigidbody2D.velocity.x < speedH - accelerationOnAirH * Game.dt)
                {
                    rigidbody2D.velocity += Vector2.right * accelerationOnAirH * Game.dt;
                }
            }
        }
        else if (horizontalAxis < 0)
        {
            if (grounded)
            {
                if (rigidbody2D.velocity.x > -speedH + accelerationH * Game.dt)
                {
                    rigidbody2D.velocity -= Vector2.right * accelerationH * Game.dt;
                }
            }
            else
            {
                if (rigidbody2D.velocity.x > -speedH + accelerationOnAirH * Game.dt)
                {
                    rigidbody2D.velocity -= Vector2.right * accelerationOnAirH * Game.dt;
                }
            }
        }

        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, Mathf.Clamp(speedV, -maxSpeedV, maxSpeedV));
        if (horizontalAxis < 0f)
        {
            isRight = false;
        }
        else if (horizontalAxis > 0f)
        {
            isRight = true;
        }

        noRotation.eulerAngles = Vector3.zero;
        scaleX = 1f - 0.1f * Mathf.Abs(rigidbody2D.velocity.y / maxSpeedV) + 0.1f * Mathf.Abs(rigidbody2D.velocity.x / speedH);
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
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.contacts[0].point.y < transform.position.y - 0.25f)
        {
            onGround--;
            if (onGround < 0)
            {
                onGround = 0;
            }
        }
    }
}
