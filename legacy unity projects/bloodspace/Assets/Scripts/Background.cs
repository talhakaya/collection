using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour {

	public float speed;
	public float amountOfJump = 2f;

	void Start ()
	{
	
	}

	void Update ()
	{
		transform.position += Vector3.down * speed * Time.deltaTime;
		if (transform.position.y > 2 * Game.Y_MAX)
		{
			transform.position -= Game.BackgroundSpeed * amountOfJump * 2 * Vector3.up * Game.Y_MAX;
		}
		else if (transform.position.y < -2 * Game.Y_MAX)
		{
			transform.position += Game.BackgroundSpeed * amountOfJump * 2 * Vector3.up * Game.Y_MAX;
		}
	}
}
