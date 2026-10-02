using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour
{
    private Animator animatorr;
    public float maxSpeed;
    public float acceleration;
    private float speed;
    private Rigidbody2D body;
    private float enablePlayerTimer;

	void Start ()
    {
        body = GetComponent<Rigidbody2D>();
        animatorr = GetComponent<Animator>();
        animatorr.speed = 0f;

	    //set camera follow object
        CameraScript.followObject = gameObject;
        body.isKinematic = true;
        enablePlayerTimer = 0f;
	}
	
	void Update ()
    {
        if (transform.lossyScale.x > 0.1f && transform.lossyScale.y > 0.1f && !CameraScript.inMiniGame)
        {
            if (enablePlayerTimer < 1f)
            {
                enablePlayerTimer += Game.dt;
                if (enablePlayerTimer >= 1f)
                {
                    body.isKinematic = false;
                    transform.position = Vector3.zero;
                }
            }
            animatorr.speed = speed * 2f;

            Vector2 dir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (Input.GetMouseButton(0))
            {
                dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            }
            if (dir.x > 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (dir.x < 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            if (dir.x != 0f || dir.y != 0f)
            {
                speed += Game.dt * acceleration;
                if (speed > maxSpeed)
                {
                    speed = maxSpeed;
                }
            }
            else
            {
                speed = 0f;
            }
            body.velocity = Geometry.normalizeVector2(dir, speed * Game.timeSpeed);
        }
        else
        {
            body.velocity = Vector2.zero;
        }
	}
}
