using UnityEngine;
using System.Collections;

namespace Games.BloodSpace
{
	public class PlayerFire : MonoBehaviour {

		public float direction;
		public bool isDead;
		public bool isBlood;
		private TintScript tint;
		private float timeCounter;
		public Sprite blood1;
		public Sprite blood2;
		public Sprite blood3;

		void Start ()
		{
			gameObject.name = "Player Fire";
			transform.parent = Game.instance.transform;
			tint = GetComponent<TintScript> ();
			SpriteEffect.make (Effect.Blur, gameObject, false, true, PlayerScript.instance);
			SpriteEffect.make (Effect.RGBSplit, gameObject, false, true, PlayerScript.instance);
			GetComponent<AudioSource>().pitch = Random.Range (0.8f, 1.2f);
			GetComponent<AudioSource>().Play ();
		}

		void Update ()
		{
			timeCounter += Time.deltaTime;
			if (direction != -1000)
			{
				GetComponent<Rigidbody2D>().AddForce (Geometry.createVector2(direction + 90, 1000));
				transform.Rotate (Vector3.forward * direction);
				direction = -1000;
			}
			else if (timeCounter > 0.05f)
			{
				if (!isBlood)
				{
					transform.Rotate (-Vector3.forward * (transform.eulerAngles.z + 90));
					transform.Rotate (Vector3.forward * Mathf.Atan2 (GetComponent<Rigidbody2D>().linearVelocity.y, GetComponent<Rigidbody2D>().linearVelocity.x) * 180 / Mathf.PI);
				}

				if (Mathf.Abs (transform.position.y) > Game.Y_MAX || Mathf.Abs (transform.position.x) > Game.X_MAX + 2)
				{
					Destroy(gameObject);
				}
				else
				{
					if (isDead)
					{
						if (tint.selfColor.a <= 0)
						{
							Destroy(gameObject);
						}
						else
						{
							tint.selfColor = new Color(tint.selfColor.r, tint.selfColor.g, tint.selfColor.b, tint.selfColor.a - Time.deltaTime * 2f);
						}
					}
					else if (Geometry.lengthOfVector2(GetComponent<Rigidbody2D>().linearVelocity) < 5f)
					{
						Destroy(gameObject);
					}
				}
			}
		}

		void OnCollisionEnter2D(Collision2D coll)
		{
			if (coll.gameObject.name == "Enemy")
			{
				float rand = Random.Range (0f, 3f);
				isBlood = true;
				if (rand < 1f)
				{
					GetComponent<SpriteRenderer>().sprite = blood1;
				}
				else if (rand < 2f)
				{
					GetComponent<SpriteRenderer>().sprite = blood2;
				}
				else
				{
					GetComponent<SpriteRenderer>().sprite = blood3;
				}
			}
		}

		void OnTriggerEnter2D(Collider2D coll)
		{
			if (coll.gameObject.name == "Enemy")
			{
				coll.gameObject.GetComponent<EnemyScript>().hp -= 100f;
			}
		}
	}
}
