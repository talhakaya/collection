using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public static int count = 0;
	public static int onGoal = 0;

	private Vector2 dir = Vector2.zero;
	private bool moving = false;
	private bool movingOld = false;
	private float moveTime = 0f;

	void Start ()
	{
		Game.fitInTile(gameObject);
	}

	void Update ()
	{
		if (moving)
		{
			rigidbody2D.AddForce(dir * 2000 * Game.dt * Mathf.Max(Game.timeMultiplier / 2f, 1f));
			moveTime -= Game.dt;
			
			if (moveTime <= 0)
			{
				moving = false;
				moveTime = 0.15f;
			}
		}
		else
		{
			if (moveTime > 0f)
			{
				moveTime -= Game.dt;
				if (moveTime <= 0f)
				{
					rigidbody2D.velocity = Vector2.zero;
					Game.fitInTile(gameObject);
				}
			}
			else if (!Game.transition)
			{
				if (Input.GetAxisRaw ("Horizontal") != 0)
				{
					moving = true;
					dir = new Vector2(Input.GetAxisRaw ("Horizontal"), 0f);
				}
				else if (Input.GetAxisRaw ("Vertical") != 0)
				{
					moving = true;
					dir = new Vector2(0f, Input.GetAxisRaw ("Vertical"));
				}
			}
		}
		
		if (moving && !movingOld)
		{
			moveTime = 0.17f;
		}
		movingOld = moving;
	}
	
	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.name == "jail" && !other.gameObject.GetComponent<Jail>().destroyed)
		{
			count++;
			other.gameObject.GetComponent<Jail>().destroyed = true;
			GameObject player = Instantiate(gameObject, transform.position, Quaternion.identity) as GameObject;
			player.name = "player";
			player.transform.position -= 2 * (transform.position - other.gameObject.transform.position);
			player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
		}
	}
}
