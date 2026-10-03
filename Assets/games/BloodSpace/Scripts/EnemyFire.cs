using UnityEngine;
using System.Collections;

namespace Games.BloodSpace
{
	public class EnemyFire : MonoBehaviour {

		public float direction;
		public bool isDead;
		private TintScript tint;

		void Start ()
		{
			gameObject.name = "Enemy Fire";
			transform.parent = Game.instance.transform;
			tint = GetComponent<TintScript> ();
	//		SpriteEffect.make (Effect.Blur, gameObject, false, true, PlayerScript.instance);
	//		SpriteEffect.make (Effect.RGBSplit, gameObject, false, true, PlayerScript.instance);
			GetComponent<AudioSource>().pitch = Random.Range (0.8f, 1.2f);
			GetComponent<AudioSource>().Play ();
		}

		void Update ()
		{
			if (direction != -1000)
			{
				GetComponent<Rigidbody2D>().AddForce (Geometry.createVector2(direction + 90, 100));
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

		void OnCollisionEnter2D(Collision2D coll)
		{

		}

		void OnTriggerEnter2D(Collider2D coll)
		{
			if (coll.gameObject.name == "Player" && Shield.thereIs == 0)
			{
				PlayerScript.script.Die ();
			}
			else if (coll.gameObject.name == "Player Fire Freeze")
			{
				Destroy (gameObject);
			}
		}
	}
}
