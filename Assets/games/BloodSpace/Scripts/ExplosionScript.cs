using UnityEngine;
using System.Collections;

namespace Games.BloodSpace
{
	public class ExplosionScript : MonoBehaviour {

		private float timeCounter;
		private TintScript tint;
		private float deathTime = 10f;
		private float bulletPushConst = 10000f;
		public float firstScale;

		void Start ()
		{
			gameObject.name = "Explosion";
			transform.parent = Game.instance.transform;
			tint = GetComponent<TintScript> ();
			SpriteEffect.make (Effect.Blur, gameObject, false, true, PlayerScript.instance);
			SpriteEffect.make (Effect.RGBSplit, gameObject, false, true, PlayerScript.instance);
			GetComponent<AudioSource>().pitch = 1.2f - transform.localScale.x / 5f;
			Game.maxBlur += 1f;
			Game.screenShake += 0.1f;
			GetComponent<AudioSource>().Play ();
		}

		void Update ()
		{
			timeCounter += 5 * Time.deltaTime;
			if (timeCounter >= firstScale)
			{
				if (timeCounter > deathTime)
				{
					Destroy(gameObject);
				}

				tint.selfColor = new Color (tint.selfColor.r, tint.selfColor.g, tint.selfColor.b, 0f);
			}
			else
			{
				transform.localScale = Vector3.one * (timeCounter + firstScale);
				tint.selfColor = new Color (tint.selfColor.r, tint.selfColor.g, tint.selfColor.b, (firstScale * 2 - transform.localScale.x) / firstScale / 2f);
			}

			transform.Rotate (Vector3.forward * transform.localScale.x * Time.deltaTime * 60);
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
			if (tint.selfColor.a > 0 && timeCounter < firstScale && (coll.GetComponent<Rigidbody2D>() != null && !coll.isTrigger && (coll.name == "Player" || coll.name == "Player Fire")))
			{
				Vector2 delta = new Vector2(coll.transform.position.x - transform.position.x, coll.transform.position.y - transform.position.y);
				coll.GetComponent<Rigidbody2D>().AddForce(Geometry.normalizeVector2(delta, bulletPushConst * tint.selfColor.a / (Geometry.lengthOfVector2(delta) + 1f)));
			}
			if (tint.selfColor.a > 0 && timeCounter < firstScale && (coll.GetComponent<Rigidbody2D>() != null && !coll.isTrigger && (coll.tag == "Enemy")))
			{
				Vector2 delta = new Vector2(coll.transform.position.x - transform.position.x, coll.transform.position.y - transform.position.y);
				coll.GetComponent<Rigidbody2D>().AddForce(Geometry.normalizeVector2(delta, bulletPushConst / 10 * tint.selfColor.a / (Geometry.lengthOfVector2(delta) + 1f)));
			}
		}

		public static ExplosionScript Create (Vector3 position, float scale)
		{
			ExplosionScript newObject = (Instantiate (Game.explosion, position, Quaternion.identity) as GameObject).GetComponent<ExplosionScript> ();
			newObject.transform.localScale = Vector3.one * scale;
			newObject.firstScale = scale;
			Game.maxBlur += scale;

			return newObject;
		}
	}
}
