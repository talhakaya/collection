using UnityEngine;
using System.Collections;

namespace Games.WhereIsHe
{
	public class Enemy : MonoBehaviour
	{
	    public bool walkX;
	    public bool walkY;
	    public Transform groundCheck;
	    public Transform rightCheck;
	    private Vector3 velocity;
	    private float maxX = 13f;
	    private float maxY = 7f;

		void Start ()
	    {
	        velocity = Vector3.zero;
	        if (walkX)
	        {
	            velocity += Vector3.right * 5f;
	        }
	        if (walkY)
	        {
	            velocity += Vector3.up * 5f;
	        }
		}

		void Update ()
	    {
	        if (walkX)
	        {
	            bool righted = Physics2D.Linecast(transform.position, rightCheck.position, Game.GroundMask);
	            if (righted)
	            {
	                velocity = new Vector3(-velocity.x, velocity.y, velocity.z);
	                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
	            }
	        }
	        if (walkY)
	        {
	            bool grounded = Physics2D.Linecast(transform.position, groundCheck.position, Game.GroundMask);
	            if (grounded)
	            {
	                velocity = new Vector3(velocity.x, -velocity.y, velocity.z);
	                transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y, transform.localScale.z);
	            }
	        }

	        if (transform.localPosition.x < -maxX)
	        {
	            transform.localPosition = new Vector3(-maxX, transform.localPosition.y, transform.localPosition.z);
	            velocity = new Vector3(Mathf.Abs(velocity.x), velocity.y, velocity.z);
	            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
	        }
	        else if (transform.localPosition.x > maxX)
	        {
	            transform.localPosition = new Vector3(maxX, transform.localPosition.y, transform.localPosition.z);
	            velocity = new Vector3(-Mathf.Abs(velocity.x), velocity.y, velocity.z);
	            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
	        }
	        if (transform.localPosition.y < -maxY)
	        {
	            transform.localPosition = new Vector3(transform.localPosition.x, -maxY, transform.localPosition.z);
	            velocity = new Vector3(velocity.x, Mathf.Abs(velocity.y), velocity.z);
	            transform.localScale = new Vector3(transform.localScale.x, Mathf.Abs(transform.localScale.y), transform.localScale.z);
	        }
	        else if (transform.localPosition.y > maxY)
	        {
	            transform.localPosition = new Vector3(transform.localPosition.x, maxY, transform.localPosition.z);
	            velocity = new Vector3(velocity.x, -Mathf.Abs(velocity.y), velocity.z);
	            transform.localScale = new Vector3(transform.localScale.x, -Mathf.Abs(transform.localScale.y), transform.localScale.z);
	        }

	        transform.position += velocity * Game.dt;
		}

	    void OnCollisionEnter2D(Collision2D collision)
	    {
	        if (collision.gameObject.tag == "Player")
	        {
	            PlayerScript p = collision.gameObject.GetComponent<PlayerScript>();
	            for (int i = 0; i < p.sprites.Length; i++)
	            {
	                Explosion.create(p.sprites[i], 3f);
	            }
	            WorldWander.currentRoom.GetComponent<Room>().reset();
	        }
	    }
	}
}
