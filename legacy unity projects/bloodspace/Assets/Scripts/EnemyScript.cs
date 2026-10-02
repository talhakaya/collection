using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour {

	public float hp = 100f;
	public bool frozen;
	private float maxHp;
	private Vector3 freezePosition;
	private float freezeTimeCounter;
	private float freezePeriod;
	private TintScript tint;
	public EnemyScript jointEnemy;

	void Start ()
	{
		gameObject.name = "Enemy";
		if (transform.parent == null)
		{
			transform.parent = Game.instance.transform;
		}

		tint = GetComponent<TintScript> ();
		SpriteEffect.make (Effect.Blur, gameObject, false, true, PlayerScript.instance);
		SpriteEffect.make (Effect.RGBSplit, gameObject, false, true, PlayerScript.instance);

		maxHp = hp;
	}

	void Update ()
	{
		if (frozen)
		{
			freezeTimeCounter += Time.deltaTime;
			transform.position = freezePosition;

			if (freezeTimeCounter >= freezePeriod)
			{
				rigidbody2D.velocity = Vector2.zero;
				rigidbody2D.angularVelocity = 0;
				freezeTimeCounter = 0f;
				tint.selfColor = new Color(tint.selfColor.r * 2f, tint.selfColor.g * 2f, tint.selfColor.b, tint.selfColor.a);
				frozen = false;
			}
		}

		if (jointEnemy != null)
		{
			transform.rotation = Quaternion.identity;
			transform.Rotate (Vector3.forward * (Geometry.angleOfVector3(transform.position - jointEnemy.transform.position) + 90));
		}

		if (hp <= 0)
		{
			ExplosionScript.Create(transform.position - Vector3.forward, Random.Range (0.7f, 1.3f) * maxHp * transform.localScale.x / 100f);
			if (GetComponent<EnemyHead>() == null)
			{
				Collectable.Create(Collectables.Shield, transform.position);
			}
			else if (EnemyHead.octopusCounter < 6)
			{
				Powerup.Create(transform.position);
			}
			Destroy(gameObject);

			if (GetComponent<EnemyHead>() != null)
			{
				Game.score += 2500;
				Game.deadOctopus++;
				Game.instance.audioOctopusDeath.Play ();
			}
			else if (GetComponent<EnemyFire>() != null)
			{
				Game.score += 200;
			}
			else
			{
				Game.score += 100;
			}
		}
		else if ( Mathf.Abs (transform.position.y) > Game.Y_MAX + 10 || Mathf.Abs (transform.position.x) > Game.X_MAX + 2)
		{
			Destroy(gameObject);
		}
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		if (coll.gameObject.name == "Player Fire")
		{
			if (!coll.gameObject.GetComponent<PlayerFire>().isDead)
			{
				coll.gameObject.GetComponent<PlayerFire>().isDead = true;
				audio.pitch = Random.Range (0.9f, 1.1f);
				audio.Play ();
				hp -= 20f;
				Game.score ++;
			}
		}
	}

	public static EnemyScript Create(Vector3 position, int type)
	{
		EnemyScript newObject = null;
		if (type == 0)
		{
			newObject = (Instantiate (Game.enemy, position, Quaternion.identity) as GameObject).GetComponent<EnemyScript> ();
		}
		else if (type == 1)
		{
			newObject = (Instantiate (Game.enemyFiring, position, Quaternion.identity) as GameObject).GetComponent<EnemyScript> ();
		}
		else if (type == 2)
		{
			newObject = (Instantiate (Game.enemyHead, position, Quaternion.identity) as GameObject).GetComponent<EnemyScript> ();
		}
		return newObject;
	}

	public void Freeze(float period)
	{
		if (!frozen)
		{
			AudioSource.PlayClipAtPoint(Game.instance.audioIce.clip, Vector3.zero, 0.7f);
			frozen = true;
			freezePosition = transform.position;
			tint.selfColor = new Color(tint.selfColor.r / 2f, tint.selfColor.g / 2f, tint.selfColor.b, tint.selfColor.a);
			freezeTimeCounter = 0f;
			hp = hp / 2f;
			freezePeriod = period;
		}
	}

	public void JointEnemy(EnemyScript enemy, float direction, float distance)
	{
		enemy.transform.position = transform.position + Geometry.createVector3 (direction + transform.eulerAngles.z, distance);
		DistanceJoint2D joint = enemy.gameObject.AddComponent<DistanceJoint2D> ();
		joint.connectedBody = rigidbody2D;
		joint.distance = distance;
		enemy.jointEnemy = this;
	}
	
	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.gameObject.name == "Player" && Shield.thereIs == 0)
		{
			PlayerScript.script.Die ();
		}
	}
}
