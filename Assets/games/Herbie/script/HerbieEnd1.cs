using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.Herbie
{
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
			if (allowedToWalk && TaloketoInputManager.GetAxisRaw("Horizontal") > 0.5f) // was == 1: a stick rarely reads exactly 1
			{
				walking = true;
				Vector3 force = new Vector2(150, 0);
				GetComponent<Rigidbody2D>().AddForce(force * Game.dt);
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
}
