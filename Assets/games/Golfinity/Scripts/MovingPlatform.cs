using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games.Golfinity
{
	public class MovingPlatform : MonoBehaviour
	{
	    public enum Direction { LeftToRight, RightToLeft }
	    public Direction direction;
	    public float distance = 4f;
	    public float period = 3f;

	    private Rigidbody2D body;
	    private Vector2 initialPosition;
	    private Vector2 moveVec;

	    void Start()
	    {
	        body = GetComponent<Rigidbody2D>();
	        initialPosition = body.position;
	        moveVec = Geometry.createVector2(transform.eulerAngles.z, distance);
	    }

	    void FixedUpdate()
	    {
	        float timer = Time.realtimeSinceStartup % period;
	        float animRatio = timer / period;
	        if (direction == Direction.RightToLeft) animRatio = 1f - animRatio;
	        if (animRatio < 0.5f)
	        {
	            body.position = initialPosition - moveVec * 0.5f + moveVec * Easing.SineEaseOut(animRatio / 0.5f, 0f, 1f, 1f);
	        }
	        else
	        {
	            body.position = initialPosition + moveVec * 0.5f - moveVec * Easing.SineEaseOut((animRatio - 0.5f) / 0.5f, 0f, 1f, 1f);
	        }
	    }
	}

}
