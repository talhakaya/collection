using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.Herbie
{
	public class HerbieSmall : MonoBehaviour {
		public TalhaAnimation walk;
		public TalhaAnimation idle;
		private TalhaAnimation current;
		private TalhaAnimation walkLines;
		private TalhaAnimation idleLines;
		private TalhaAnimation currentLines;
		public Lines lines;
		private bool walkingOld = true;
		private bool lookingLeft = false;
		private bool lookingLeftOld = false;
		private TintScript tint;
		public bool isHappy;
		public float speed;
		private float firstScaleX;
		private float walkingLastTime = 0f;

		void Start ()
		{
			tint = GetComponent<TintScript> ();
			tint.selfColor = Game.color1;
			walkLines = lines.walk;
			idleLines = lines.idle;
			firstScaleX = transform.localScale.x;
		}

		void Update ()
		{
			bool walking = false;
			if (TaloketoInputManager.GetAxisRaw("Horizontal") != 0f)
			{
				lookingLeft = (TaloketoInputManager.GetAxisRaw("Horizontal") < 0);
			}
			if (TaloketoInputManager.GetAxisRaw("Horizontal") != 0 || TaloketoInputManager.GetAxisRaw("Vertical") != 0)
			{
				walking = true;
				Vector3 force = Geometry.normalizeVector2(new Vector2(TaloketoInputManager.GetAxisRaw("Horizontal"), TaloketoInputManager.GetAxisRaw("Vertical")), speed);
				GetComponent<Rigidbody2D>().AddForce(force * Game.dt);
			}
			if (lookingLeft && !lookingLeftOld)
			{
				transform.localScale = new Vector3(-firstScaleX, transform.localScale.y, transform.localScale.z);
			}
			else if (!lookingLeft && lookingLeftOld)
			{
				transform.localScale = new Vector3(firstScaleX, transform.localScale.y, transform.localScale.z);
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
			if (walking)
			{
				float walkTime = Game.time % 0.2f;
				if (walkTime >= 0.05f && walkingLastTime < 0.05f)
				{
					stepSound();
				}
				else if (walkTime >= 0.15f && walkingLastTime < 0.15f)
				{
					stepSound();
				}
				walkingLastTime = walkTime;
			}
			walkingOld = walking;
			lookingLeftOld = lookingLeft;
		}

		void stepSound()
		{
			Game.instance.aStep.pitch = Random.Range (1.3f, 2f);
			Game.instance.aStep.Play ();
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

		void OnCollisionEnter2D(Collision2D other)
		{
			if (other.gameObject.name == "Pacman")
			{
				Game.endMiniGame();
				Game.startMiniGame("pacman");
			}
		}
	}
}
