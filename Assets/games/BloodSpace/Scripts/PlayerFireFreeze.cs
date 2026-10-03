using UnityEngine;
using System.Collections;

namespace Games.BloodSpace
{
	public class PlayerFireFreeze : MonoBehaviour {

		public float direction;
		public bool isDead;
		public float freezePeriod;
		private TintScript tint;

		void Start ()
		{
			gameObject.name = "Player Fire Freeze";
			transform.parent = Game.instance.transform;
			tint = GetComponent<TintScript> ();
			tint.selfColor = new Color (0f, 1f, 1f);
			SpriteEffect.make (Effect.Blur, gameObject, false, true, PlayerScript.instance);
			SpriteEffect.make (Effect.RGBSplit, gameObject, false, true, PlayerScript.instance);
			GetComponent<AudioSource>().pitch = Random.Range (0.8f, 1.2f);
			GetComponent<AudioSource>().Play ();
		}

		void Update ()
		{
			if (direction != -1000)
			{
				GetComponent<Rigidbody2D>().AddForce (Geometry.createVector2(direction + 90, 1000));
				transform.Rotate (Vector3.forward * direction);
				direction = -1000;
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
						tint.selfColor = new Color(tint.selfColor.r, tint.selfColor.g, tint.selfColor.b, tint.selfColor.a - Time.deltaTime * 0.5f);
					}
				}
			}
		}

		void OnTriggerEnter2D(Collider2D coll)
		{
			if (coll.gameObject.name == "Enemy")
			{
				EnemyScript enemy = coll.gameObject.GetComponent<EnemyScript>();
				enemy.Freeze(freezePeriod);
			}
		}
	}
}
