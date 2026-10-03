using UnityEngine;
using System.Collections;

namespace Games.BloodSpace
{
	public class PlayerFireExplosion : MonoBehaviour {

		public float direction;
		private float directionOld;
		private float firstSpeed;
		public bool isDead;
		private TintScript tint;
		private float timeCounter;

		void Start ()
		{
			gameObject.name = "Player Fire Explosion";
			transform.parent = Game.instance.transform;
			tint = GetComponent<TintScript> ();
			tint.selfColor = new Color (1f, 0.5f, 0.5f);
			SpriteEffect.make (Effect.Blur, gameObject, false, true, PlayerScript.instance);
			SpriteEffect.make (Effect.RGBSplit, gameObject, false, true, PlayerScript.instance);
			GetComponent<AudioSource>().pitch = Random.Range (0.8f, 1.2f);
			GetComponent<AudioSource>().Play ();
			firstSpeed = Random.Range(900f, 1200f);
		}

		void Update ()
		{
			if (direction != -1000)
			{
				GetComponent<Rigidbody2D>().AddForce (Geometry.createVector2(direction + 90, firstSpeed));
				transform.Rotate (Vector3.forward * direction);
				directionOld = direction;
				direction = -1000;
			}
			else
			{
				timeCounter += Time.deltaTime;
				GetComponent<Rigidbody2D>().AddForce (Geometry.createVector2(directionOld - 90, 500 * Time.deltaTime));
				if (Geometry.lengthOfVector2(GetComponent<Rigidbody2D>().linearVelocity) < 0.1f && timeCounter > 0.5f)
				{
					Explode();
				}
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

		void Explode()
		{
			ExplosionScript.Create (transform.position, 0.5f);
			int noOfFires = Random.Range (7, 15);
			for (int i = 0; i < noOfFires; i++)
			{
				Game.playerFireExplosionParticle.GetComponent<PlayerFireExplosionParticle> ().direction = Random.Range(0f, 360f);
				PlayerFireExplosionParticle playerFire = (Instantiate (Game.playerFireExplosionParticle, transform.position, Quaternion.identity) as GameObject).GetComponent<PlayerFireExplosionParticle>();
				playerFire.GetComponent<Collider2D>().isTrigger = true;
			}
			Destroy (gameObject);
		}

		void OnTriggerEnter2D(Collider2D coll)
		{
			if (coll.gameObject.name == "Enemy")
			{
				Explode();
			}
		}
	}
}
