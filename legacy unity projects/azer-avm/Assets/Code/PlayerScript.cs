using UnityEngine;
using System.Collections;

public enum PlayerNo { P1, P2, P3, P4 }

public class PlayerScript : MonoBehaviour {

    public static PlayerScript[] list;
    public bool isRight;
    public PlayerNo playerNo;
    public float speedH;
    public float jumpForceFirst;
    public float jumpForceCont;
    public float jumpFixSpeed;
    private bool justPressedJump;
    private bool justReleasedJump;
    private bool pressingJump;
    private bool onGround = false;
    private float horizontalAxis;
    private float walkAnimCount;
    public Transform arm0;
    public Transform arm1;
    public Transform leg0;
    public Transform leg1;


	void Start ()
    {
        list[(int) playerNo] = this;
        SpriteEffect.make(Effect.Blur, gameObject, false, true, transform);
	}
	
	void Update ()
    {
        if (playerNo == PlayerNo.P1)
        {
            horizontalAxis = Input.GetAxisRaw("P1Horizontal");
            justPressedJump = Input.GetButtonDown("P1Jump");
            pressingJump = Input.GetButton("P1Jump");
            justReleasedJump = Input.GetButtonUp("P1Jump");
        }
        else if (playerNo == PlayerNo.P2)
        {
            horizontalAxis = Input.GetAxisRaw("P2Horizontal");
            justPressedJump = Input.GetButtonDown("P2Jump");
            pressingJump = Input.GetButton("P2Jump");
            justReleasedJump = Input.GetButtonUp("P2Jump");
        }
        else if (playerNo == PlayerNo.P3)
        {
            horizontalAxis = Input.GetAxisRaw("P3Horizontal");
            justPressedJump = Input.GetButtonDown("P3Jump");
            pressingJump = Input.GetButton("P3Jump");
            justReleasedJump = Input.GetButtonUp("P3Jump");
        }
        else if (playerNo == PlayerNo.P4)
        {
            horizontalAxis = Input.GetAxisRaw("P4Horizontal");
            justPressedJump = Input.GetButtonDown("P4Jump");
            pressingJump = Input.GetButton("P4Jump");
            justReleasedJump = Input.GetButtonUp("P4Jump");
        }

        if (rigidbody2D.velocity.x < -0.1f)
        {
            isRight = true;
        }
        else if (rigidbody2D.velocity.x > 0.1f)
        {
            isRight = false;
        }

	    if (isRight)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        if (pressingJump)
        {
            fixRotation();
        }
        if (onGround)
        {
            rigidbody2D.AddForce(Vector2.up * jumpForceFirst);
            
        }
        else
        {
            rigidbody2D.AddForce(Vector2.up * jumpForceCont * Game.dt);
        }
        rigidbody2D.AddForce(Vector2.right * horizontalAxis * speedH * Game.dt);
        if (horizontalAxis == 0)
        {
            leg0.localEulerAngles = Vector3.zero;
            leg1.localEulerAngles = Vector3.zero;
        }
        else
        {
            walkAnimCount += Game.dt;
            if (walkAnimCount > 0.1f)
            {
                walkAnimCount = 0f;
                leg0.localEulerAngles = Vector3.forward * Random.Range(-60f, 60f);
                leg1.localEulerAngles = Vector3.forward * Random.Range(-60f, 60f);
                arm0.localEulerAngles = Vector3.forward * Random.Range(-60f, 60f);
                arm1.localEulerAngles = Vector3.forward * Random.Range(-60f, 60f);
            }
        }
	}

    void fixRotation()
    { 
        if (transform.eulerAngles.z != 0)
        {
            if (transform.eulerAngles.z < 180f)
            {
                if (transform.eulerAngles.z < jumpFixSpeed * Game.dt)
                {
                    transform.eulerAngles -= Vector3.forward * transform.eulerAngles.z;
                }
                else
                {
                    transform.eulerAngles -= Vector3.forward * jumpFixSpeed * Game.dt;
                }
            }
            else
            {
                if (transform.eulerAngles.z > 360 - jumpFixSpeed * Game.dt)
                {
                    transform.eulerAngles += Vector3.forward * transform.eulerAngles.z;
                }
                else
                {
                    transform.eulerAngles += Vector3.forward * jumpFixSpeed * Game.dt;
                }
            }

            if (transform.eulerAngles.z < 2f || transform.eulerAngles.z > 358f)
            {
                transform.eulerAngles = Vector3.zero;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.name == "Platform" && other.contacts[0].point.y < transform.position.y)
        {
            onGround = true;
            SpriteEffect.blurConst = 1f;
            Game.instance.tasak.pitch = Random.Range(0.9f, 1.1f);
            Game.instance.tasak.Play();
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.name == "Platform" && other.contacts[0].point.y < transform.position.y)
        {
            onGround = false;
        }
    }
}
