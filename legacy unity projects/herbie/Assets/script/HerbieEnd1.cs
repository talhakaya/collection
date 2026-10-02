using UnityEngine;
using System.Collections;

public class HerbieEnd1 : MonoBehaviour {
	public TalhaAnimation walk;
	public TalhaAnimation idle;
	private TalhaAnimation current;
	private TalhaAnimation walkLines;
	private TalhaAnimation idleLines;
	private TalhaAnimation currentLines;
	public Lines lines;
	public static bool allowedToWalk = true;
	private bool walkingOld = true;

	void Start ()
	{
		walkLines = lines.walk;
		idleLines = lines.idle;
		allowedToWalk = true;
	}

	void Update ()
	{
		bool walking = false;
		if (allowedToWalk && Input.GetAxisRaw("Horizontal") == 1)
		{
			walking = true;
			Vector3 force = new Vector2(150, 0);
			rigidbody2D.AddForce(force * Game.dt);
		}
		if (walking && !walkingOld)
		{
			changeAnimation(walk);
			changeAnimationLines(walkLines);
		}
		else if (!walking && walkingOld)
		{
			changeAnimation(idle);
			changeAnimationLines(idleLines);
		}
		walkingOld = walking;
	}
	
	void changeAnimation(TalhaAnimation newAnim)
	{
		if (current != null)
		{
			current.enabled = false;
		}
		newAnim.enabled = true;
		current = newAnim;
	}
	
	void changeAnimationLines(TalhaAnimation newAnim)
	{
		if (currentLines != null)
		{
			currentLines.enabled = false;
		}
		newAnim.enabled = true;
		currentLines = newAnim;
	}
}
