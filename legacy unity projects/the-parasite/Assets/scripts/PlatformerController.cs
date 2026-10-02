using UnityEngine;
using System.Collections;

public class PlatformerController : MonoBehaviour {

    public bool isRight;
    public float speedH;
    public float maxSpeedV;
    private float speedV;
    public float jumpForceFirst;
    public float jumpForceCont;
    // private TalhaAnimation anim;
    private int onGround = 0;
    private Rigidbody2D rigidbody2d;

	void Start ()
    {
        // anim = GetComponent<TalhaAnimation>();
        rigidbody2d = GetComponent<Rigidbody2D>();
	}
	
	void Update ()
    {
        speedV = rigidbody2d.velocity.y;
        speedV -= Game.dt * 19.87f;
        if (Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.Space))
        {
            
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
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
        float horizontalAxis = Input.GetAxisRaw("Horizontal");
        // anim.enabled = (horizontalAxis != 0);
        rigidbody2d.velocity = new Vector2(horizontalAxis * speedH, Mathf.Clamp(speedV, -maxSpeedV, maxSpeedV));

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
