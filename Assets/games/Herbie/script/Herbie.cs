using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.Herbie
{
	public class Herbie : MonoBehaviour {
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
		public GameObject coke;
		public AudioSource aStep;
		private GameObject whore;
		private bool holdingWhore;
		public static Herbie instance;
		private int momLastSeen = -1;
		private float walkingLastTime = 0f;

		void Start ()
		{
			instance = this;
			tint = GetComponent<TintScript> ();
			tint.selfColor = Game.color1;
			walkLines = lines.walk;
			idleLines = lines.idle;
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
				transform.localScale = new Vector3(-1, 1, 1);
			}
			else if (!lookingLeft && lookingLeftOld)
			{
				transform.localScale = new Vector3(1, 1, 1);
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
				float walkTime = Game.time % 0.8f;
				if (walkTime >= 0.2f && walkingLastTime < 0.2f)
				{
					stepSound();
				}
				else if (walkTime >= 0.6f && walkingLastTime < 0.6f)
				{
					stepSound();
				}
				walkingLastTime = walkTime;
			}
			walkingOld = walking;
			lookingLeftOld = lookingLeft;
			if (Game.cokeTaken && !Game.cokeGiven)
			{
				if (!coke.activeSelf)
				{
					coke.SetActive(true);
				}
			}
			else
			{
				if (coke.activeSelf)
				{
					coke.SetActive(false);
				}
			}

			if (holdingWhore)
			{
				whore.transform.position = transform.position + new Vector3(-1f, 0.8f, -0.00002f);

			}
		}

		void bushSound(float pitch)
		{
			Game.instance.aBush.pitch = pitch;
			Game.instance.aBush.Play ();
		}

		void talkSound(float pitch)
		{
			Game.instance.aTalk.pitch = pitch;
			Game.instance.aTalk.Play ();
		}

		void stepSound()
		{
			Game.instance.aStep.pitch = Random.Range (0.3f, 1f);
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

		void OnTriggerEnter2D(Collider2D other)
		{
			if (other.name == "Mark")
			{
				isHappy = true;
			}
			else if (other.name == "Bush")
			{
				bushSound(Random.Range(0.7f, 1.3f));
			}
			else if (other.name == "BigBush")
			{
				bushSound(Random.Range(0.3f, 0.5f));
			}
			else if (other.name == "Mother")
			{
				isHappy = false;
				if (momLastSeen != TalhaTexting.iGet(other.gameObject))
				{
					Game.momLove++;
					momLastSeen = TalhaTexting.iGet(other.gameObject);
				}
				talkSound(1.5f);
			}
			else if (other.name == "Grave")
			{
				isHappy = false;
			}
			else if (other.name == "Whore")
			{
				isHappy = true;
				talkSound(2f);
			}
			else if (other.name == "Muscle")
			{
				isHappy = false;
				if (TalhaTexting.iGet(other.gameObject) < 2)
				{
					if (coke.activeSelf || holdingWhore)
					{
						TalhaTexting.changeI(other.gameObject, 1);
					}
					else
					{
						TalhaTexting.changeI(other.gameObject, 0);
					}
				}
				talkSound(0.75f);
			}
			else if (other.name == "Pacman")
			{
				isHappy = false;
				talkSound(0.5f);
			}
			if (other.GetComponent<TalhaTexting>() != null)
			{
				TalhaTexting.visibleOnOff(other.gameObject, true);
			}
		}

		void OnTriggerStay2D(Collider2D other)
		{
			if (other.name == "Mark")
			{
				Game.herbieMultiplier = 2f;
			}
			else if (other.name == "Casette")
			{
				if (TaloketoInputManager.GetButtonDown("Interact"))
				{
					other.GetComponent<Casette>().done = true;
					other.enabled = false;
					other.transform.position -= Vector3.forward;
					other.GetComponent<GroundObject>().enabled = false;
					isHappy = true;
					Game.instance.music.Play();
				}
			}
			else if (other.name == "Drug")
			{
				if (TaloketoInputManager.GetButtonDown("Interact"))
				{
					other.GetComponent<Casette>().done = true;
					other.enabled = false;
					other.transform.position -= Vector3.forward;
					other.GetComponent<GroundObject>().enabled = false;
					isHappy = true;
					Game.drugTimer = Game.DRUGTIMER;
				}
			}
			else if (other.name == "Mother")
			{
				if (TaloketoInputManager.GetButtonDown("Interact"))
				{
					if (TalhaTexting.iGet(other.gameObject) == 6)
					{
						Game.startMiniGame("end0");
						Game.instance.music.Play();
					}
					else
					{
						Game.startMiniGame("momKiss");
					}
				}
			}
			else if (other.name == "Grave")
			{
				if (TaloketoInputManager.GetButtonDown("Interact"))
				{
					Game.startMiniGame("end1");
					Game.instance.music.Play();
				}
			}
			else if (other.name == "Whore")
			{
				if (TaloketoInputManager.GetButtonDown("Interact"))
				{
					if (coke.activeSelf)
					{
						holdingWhore = true;
						other.transform.Rotate(-Vector3.forward * 75);
						other.GetComponent<GroundObject>().enabled = false;
						Game.cokeGiven = true;
						whore = other.gameObject;
						other.enabled = false;
						TalhaTexting.next(whore);
						TalhaTexting.visibleOnOff(whore, true);
						talkSound(2f);
					}
				}
			}
			else if (other.name == "SexPlace")
			{
				if (whore != null && TalhaTexting.iGet(whore) < 2)
				{
					TalhaTexting.next(whore);
					talkSound(2f);
				}
				if (holdingWhore && TaloketoInputManager.GetButtonDown("Interact"))
				{
					Game.startMiniGame("sex");
				}
			}
			else if (other.name == "Muscle")
			{
				if (TaloketoInputManager.GetButtonDown("Interact"))
				{
					if (!Game.muscleBeaten && TalhaTexting.iGet(other.gameObject) == 0)
					{
						Game.muscleBeaten = true;
						TalhaTexting.changeI(other.gameObject, 2);
						Game.instance.drug.SetActive(true);
						Game.startMiniGame("fight");
					}
				}
			}
			else if (other.name == "Pacman")
			{
				if (TaloketoInputManager.GetButtonDown("Interact"))
				{
					Game.startMiniGame("pacman");
				}
				if (Game.cokeTaken)
				{
					if (!Game.cokeGiven)
					{
						TalhaTexting.changeI(other.gameObject, 1);
					}
					else
					{
						TalhaTexting.changeI(other.gameObject, 2);
					}
				}
			}
		}

		void OnTriggerExit2D(Collider2D other)
		{
			if (other.GetComponent<TalhaTexting>() != null)
			{
				TalhaTexting.visibleOnOff(other.gameObject, false);
			}
		}

		public void endSex()
		{
			holdingWhore = false;
			whore.transform.position = transform.position + Vector3.right / 4f;
			whore.GetComponent<GroundObject>().enabled = true;
			whore.transform.rotation = Quaternion.identity;
			Game.instance.hammer.SetActive (true);
			TalhaTexting.next (whore);
			TalhaTexting.visibleOnOff(whore, false);
			whore.GetComponent<Collider2D>().enabled = true;
		}
	}
}
