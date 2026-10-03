using UnityEngine;
using System.Collections;

namespace Games.BloodSpace
{
	public class Powerup : MonoBehaviour {

		private TintScript tint;
		private float timeCounter;
		private float period = 0.1f;
		private bool playerGot;

		void Start ()
		{
			name = "Powerup";
			transform.parent = Game.instance.transform;
			SpriteEffect.make (Effect.Blur, gameObject, false, true, PlayerScript.instance);
			SpriteEffect.make (Effect.RGBSplit, gameObject, false, true, PlayerScript.instance);
			tint = GetComponent<TintScript> ();
		}

		void Update ()
		{
			timeCounter += Time.deltaTime;
			if (!playerGot)
			{
				if (timeCounter >= period)
				{
					timeCounter = 0f;
					if (tint.selfColor.r == 0f)
					{
						tint.selfColor = new Color(1f, 1f, 1f);
					}
					else
					{
						tint.selfColor = new Color(0f, 1f, 1f);
					}
				}
			}
			else
			{
				if (timeCounter > 2)
				{
					Destroy (gameObject);
				}
			}
		}

		public void PlayerGet()
		{
			GetComponent<AudioSource>().Play ();
			PlayerScript.instance.GetComponent<PlayerScript> ().firesPerShot++;
			timeCounter = 0f;
			playerGot = true;
			tint.selfColor = new Color (0f, 0f, 0f, 0f);
		}

		public static Powerup Create(Vector3 position)
		{
			return (Instantiate(Game.powerup, position, Quaternion.identity) as GameObject).GetComponent<Powerup>();
		}

		void OnTriggerEnter2D(Collider2D coll)
		{
			Trigger (coll);
		}

		void OnTriggerStay2D(Collider2D coll)
		{
			Trigger (coll);
		}

		void Trigger(Collider2D coll)
		{
			if (!playerGot && coll.gameObject.name == "Player")
			{
				PlayerGet();
			}
		}
	}
}
