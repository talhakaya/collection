using UnityEngine;
using System.Collections;

namespace Games.BloodSpace
{
	public enum Collectables
	{
		Beam,
		Shield,
		Explosive,
		Freeze,
		PiercingFire//piercing
	}

	public class Collectable : MonoBehaviour {

		private static float speed = 2f;

		public Collectables type;
		private bool playerGot;
		private Vector3 direction;
		private float distanceFromPlayer = 1;

		void Start ()
		{
			name = "Collectable";
			transform.parent = Game.instance.transform;
			SpriteEffect.make (Effect.Blur, gameObject, false, true, PlayerScript.instance);
			SpriteEffect.make (Effect.RGBSplit, gameObject, false, true, PlayerScript.instance);

			float sum = 0f;
			float[] poss = {0.2f,0.2f,0.2f,0.2f,0.2f};

			if (PlayerScript.script.collectables.Count < 3)
			{
				for (int i = 0; i < PlayerScript.script.collectables.Count; i++)
				{
					poss[(int) PlayerScript.script.collectables[i].type] += 0.25f;
				}
			}

			float total = 0f;
			for (int i = 0; i < poss.Length; i++)
			{
				total += poss[i];
			}

			float random = Random.Range (0f, total);

			for (int i = 0; i < poss.Length; i++)
			{
				sum += poss[i];
				if (random < sum)
				{
					Init ((Collectables) i);
					break;
				}
			}
		}

		void Init(Collectables c)
		{
			type = c;
			if (c == Collectables.Shield)
			{
				GetComponent<TintScript>().selfColor = new Color(0f, 1f, 0f);
			}
			else if (c == Collectables.Beam)
			{
				GetComponent<TintScript>().selfColor = new Color(1f, 1f, 0f);
			}
			else if (c == Collectables.Freeze)
			{
				GetComponent<TintScript>().selfColor = new Color(0f, 0.7f, 1f);
			}
			else if (c == Collectables.Explosive)
			{
				GetComponent<TintScript>().selfColor = new Color(1f, 0.2f, 0.2f);
			}
			else
			{
				GetComponent<TintScript>().selfColor = new Color(1f, 1f, 1f);
			}
		}

		void Update ()
		{
			if (!playerGot)
			{
				transform.position -= Vector3.up * Time.deltaTime * speed;
				if (Mathf.Abs (transform.position.y) > Game.Y_MAX || Mathf.Abs (transform.position.x) > Game.X_MAX + 2)
				{
					Destroy(gameObject);
				}
			}
		}

		public void PlayerGet(int dir)
		{
			GetComponent<AudioSource>().Play ();
			playerGot = true;
			if (dir == 0)
			{
				direction = -Vector3.right;
			}
			else if (dir == 1)
			{
				direction = -Vector3.up;
			}
			else if (dir == 2)
			{
				direction = Vector3.right;
			}
			transform.position = PlayerScript.instance.transform.position + Geometry.normalizeVector3(direction, distanceFromPlayer);
			transform.parent = PlayerScript.instance.transform;
		}

		public static Collectable Create(Collectables type, Vector3 position)
		{
			Collectable newObject = (Instantiate(Game.collectable, position, Quaternion.identity) as GameObject).GetComponent<Collectable>();
			newObject.type = type;

			return newObject;
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
			if (coll.name == "Player" && PlayerScript.script.collectables.Count < 3 && !playerGot)
			{
				if (Game.mode == GameMode.Hardcore)
				{
					if (PlayerScript.script.numberInCollectables(type) != PlayerScript.script.collectables.Count)
					{
						PlayerScript.script.SpecialAttack();
					}
				}
				PlayerGet(PlayerScript.script.collectables.Count);
				PlayerScript.script.collectables.Add (this);
			}
		}
	}
}
