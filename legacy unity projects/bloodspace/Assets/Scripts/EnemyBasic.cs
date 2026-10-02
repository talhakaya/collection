using UnityEngine;
using System.Collections;

public class EnemyBasic : MonoBehaviour {

	private Vector2 direction;
	public float speed;

	void Start ()
	{
		if (direction == Vector2.zero)
		{
			direction = -Vector2.up;
		}
		if (speed == 0f)
		{
			speed = 2000f;
		}
	}

	void Update ()
	{
		rigidbody2D.AddForce (direction * speed * Time.deltaTime);
	}

	public void ChangeDirection(Vector2 dir)
	{
		direction = Geometry.normalizeVector2 (dir, 1);
	}
}
