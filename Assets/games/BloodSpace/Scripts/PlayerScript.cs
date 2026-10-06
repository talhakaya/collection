using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Collection.Controls;

namespace Games.BloodSpace
{
	public class PlayerScript : MonoBehaviour {

		public static Transform instance;
		public static PlayerScript script;
		public static float MinDistanceForMouseMove = 0.1f;

		public float maxSpeed;
		public float acceleration;
		public float drag;
		private float speedX;
		private float speedY;
		private float fireTimeCounter;
		public float firePeriod;
		public int firesPerShot;
		public List<Collectable> collectables;
		private float collectibleTimeCounter;
		public float collectiblePeriod;
		public Sprite normal;
		public Sprite left;
		public Sprite right;
		public Sprite up;
		public Sprite down;
		private SpriteRenderer spriteRenderer;

		private float verticalOld = 0;
		private float horizontalOld = 0;

		void Awake()
		{
			instance = transform;
			script = this;
			gameObject.name = "Player";
			transform.parent = Game.instance.transform;
		}

		void Start ()
		{
			collectables = new List<Collectable> ();
			spriteRenderer = GetComponent<SpriteRenderer> ();

		}

		void Update ()
		{
			if (fireTimeCounter < firePeriod)
			{
				fireTimeCounter += Time.deltaTime;
			}
			collectibleTimeCounter += Time.deltaTime;
			if (collectibleTimeCounter >= collectiblePeriod)
			{
				collectibleTimeCounter -= collectiblePeriod;
			}

			spriteRenderer.sprite = normal;
			if (!Game.mouseMode)
			{
				float vertical = TaloketoInputManager.GetAxis("Vertical0") - TaloketoInputManager.GetAxis("Vertical1");
				if (vertical > 1)
				{
					vertical = 1;
				}
				else if (vertical < -1)
				{
					vertical = -1;
				}

				if ((vertical > 0 && vertical < verticalOld) || (vertical < 0 && vertical > verticalOld))
				{
					verticalOld = vertical;
					vertical = 0;
				}
				else
				{
					verticalOld = vertical;
				}

				float horizontal = TaloketoInputManager.GetAxis("Horizontal0") + TaloketoInputManager.GetAxis("Horizontal1");
				if (horizontal > 1)
				{
					horizontal = 1;
				}
				else if (horizontal < -1)
				{
					horizontal = -1;
				}

				if ((horizontal > 0 && horizontal < horizontalOld) || (horizontal < 0 && horizontal > horizontalOld))
				{
					horizontalOld = horizontal;
					horizontal = 0;
				}
				else
				{
					horizontalOld = horizontal;
				}

				if (vertical > 0)
				{
					spriteRenderer.sprite = up;
				}
				if (vertical < 0)
				{
					spriteRenderer.sprite = down;
				}
				transform.position += Vector3.up * vertical * 6 * Time.deltaTime;
				if (horizontal < 0)
				{
					spriteRenderer.sprite = left;
				}
				if (horizontal > 0)
				{
					spriteRenderer.sprite = right;
				}
				transform.position += Vector3.right * horizontal * 6 * Time.deltaTime;
			}



			if (speedX > drag * Time.deltaTime)
			{
				speedX -= drag * Time.deltaTime;
			}
			else if (speedX > 0f)
			{
				speedX = 0f;
			}
			if (speedX < -drag * Time.deltaTime)
			{
				speedX += drag * Time.deltaTime;
			}
			else if (speedX < 0f)
			{
				speedX = 0f;
			}
			if (speedY > drag * Time.deltaTime)
			{
				speedY -= drag * Time.deltaTime;
			}
			else if (speedY > 0f)
			{
				speedY = 0f;
			}
			if (speedY < -drag * Time.deltaTime)
			{
				speedY += drag * Time.deltaTime;
			}
			else if (speedY < 0f)
			{
				speedY = 0f;
			}

			if (Game.mouseMode)
			{
				Vector3 deltaVector3 = MousePosition.get() - transform.position;
				if (deltaVector3.y > MinDistanceForMouseMove)
				{
					spriteRenderer.sprite = up;
				}
				if (deltaVector3.y < -MinDistanceForMouseMove)
				{
					spriteRenderer.sprite = down;
				}
				if (deltaVector3.x > MinDistanceForMouseMove)
				{
					spriteRenderer.sprite = right;
				}
				if (deltaVector3.x < -MinDistanceForMouseMove)
				{
					spriteRenderer.sprite = left;
				}
				if (Geometry.lengthOfVector3(deltaVector3) > MinDistanceForMouseMove)
				{
					Vector3 accelerationVector3 = Geometry.normalizeVector3(deltaVector3, acceleration * 1.2f);
					speedX = accelerationVector3.x;
					speedY = accelerationVector3.y;
				}
			}

			if (speedX > maxSpeed)
			{
				speedX = maxSpeed;
			}
			if (speedX < -maxSpeed)
			{
				speedX = -maxSpeed;
			}
			if (speedY > maxSpeed)
			{
				speedY = maxSpeed;
			}
			if (speedY < -maxSpeed)
			{
				speedY = -maxSpeed;
			}

			transform.position += Vector3.up * speedY * Time.deltaTime + Vector3.right * speedX * Time.deltaTime;

			if (fireTimeCounter >= firePeriod)
			{
				fireTimeCounter = 0f;
				Fire();
			}
	//		if (Input.GetKey(KeyCode.X))
	//		{
	//			if (fireTimeCounter >= firePeriod)
	//			{
	//				fireTimeCounter = 0f;
	//				Fire();
	//			}
	//		}
			if (TaloketoInputManager.GetButton("Special0") || TaloketoInputManager.GetButton("Special1"))
			{
				if (collectables.Count == 3)
				{
					SpecialAttack();
				}
			}

			for (int i = 0; i < collectables.Count; i++)
			{
				collectables[i].transform.localPosition = Geometry.createVector3(i * 120 + collectibleTimeCounter * 360 / collectiblePeriod, 0.3f);
				collectables[i].transform.rotation = Quaternion.identity;
				collectables[i].transform.Rotate(Vector3.forward * Geometry.angleOfVector3(collectables[i].transform.localPosition));
			}

			if (transform.position.x > Game.X_MAX - Game.PLAYER_OFFSET)
			{
				transform.position = new Vector3(Game.X_MAX - Game.PLAYER_OFFSET, transform.position.y, transform.position.z);
			}
			else if (transform.position.x < -Game.X_MAX + Game.PLAYER_OFFSET)
			{
				transform.position = new Vector3(-Game.X_MAX + Game.PLAYER_OFFSET, transform.position.y, transform.position.z);
			}
			if (transform.position.y > Game.Y_MAX - Game.PLAYER_OFFSET)
			{
				transform.position = new Vector3(transform.position.x, Game.Y_MAX - Game.PLAYER_OFFSET, transform.position.z);
			}
			else if (transform.position.y < -Game.Y_MAX + Game.PLAYER_OFFSET)
			{
				transform.position = new Vector3(transform.position.x, -Game.Y_MAX + Game.PLAYER_OFFSET, transform.position.z);
			}
		}

		void Fire()
		{
			for (int i = 0; i < firesPerShot; i++)
			{
				Game.playerFire.GetComponent<PlayerFire> ().direction = (i + 0.5f - firesPerShot / 2f) * 15f;
				Instantiate (Game.playerFire, transform.position + Vector3.forward + Vector3.up * 0.2f, Quaternion.identity);
			}
		}

		public void SpecialAttack()
		{
			float rgbValue = 3f;
			if (typeInCollectables(Collectables.Beam))
			{
				if (Game.mode != GameMode.Hardcore || numberInCollectables(Collectables.Beam) == 3)
				{
					Beam beam = (Instantiate(Game.beam, transform.position + Vector3.up * 0.3f, Quaternion.identity) as GameObject).GetComponent<Beam>();
					if (numberInCollectables(Collectables.Beam) == 1)
					{
						beam.lifeTime = 0.5f;
						beam.firstScale = 0.5f;
						Game.score += 100;
						Game.maxRgbSplit += rgbValue;
						AudioSource.PlayClipAtPoint(Game.instance.audioLaser.clip, Vector3.zero, 0.2f);
					}
					else if (numberInCollectables(Collectables.Beam) == 2)
					{
						beam.lifeTime = 1f;
						beam.firstScale = 0.75f;
						Game.score += 300;
						Game.maxRgbSplit += 3 * rgbValue;
						AudioSource.PlayClipAtPoint(Game.instance.audioLaser.clip, Vector3.zero, 0.5f);

					}
					else if (numberInCollectables(Collectables.Beam) == 3)
					{
						beam.lifeTime = 2f;
						beam.firstScale = 1f;
						Game.score += 1000;
						Game.maxRgbSplit += 6 * rgbValue;
						AudioSource.PlayClipAtPoint(Game.instance.audioLaser.clip, Vector3.zero, 1f);
					}
				}
			}
			if (typeInCollectables(Collectables.Explosive))
			{
				if (Game.mode != GameMode.Hardcore || numberInCollectables(Collectables.Explosive) == 3)
				{
					int noOfFires = 0;
					if (numberInCollectables(Collectables.Explosive) == 1)
					{
						noOfFires = 2;
						Game.score += 100;
						Game.maxRgbSplit += rgbValue;
					}
					else if (numberInCollectables(Collectables.Explosive) == 2)
					{
						noOfFires = 4;
						Game.score += 300;
						Game.maxRgbSplit += 3 * rgbValue;
					}
					else if (numberInCollectables(Collectables.Explosive) == 3)
					{
						noOfFires = 8;
						Game.score += 1000;
						Game.maxRgbSplit += 6 * rgbValue;
					}
					for (int i = 0; i < noOfFires; i++)
					{
						Game.playerFireExplosion.GetComponent<PlayerFireExplosion> ().direction = Random.Range(-10f * noOfFires, 10f * noOfFires);
						PlayerFireExplosion playerFire = (Instantiate (Game.playerFireExplosion, transform.position + Vector3.forward, Quaternion.identity) as GameObject).GetComponent<PlayerFireExplosion>();
						playerFire.GetComponent<Collider2D>().isTrigger = true;
						playerFire.GetComponent<Rigidbody2D>().mass = 4f;
					}
				}
			}
			if (typeInCollectables(Collectables.PiercingFire))
			{
				if (Game.mode != GameMode.Hardcore || numberInCollectables(Collectables.PiercingFire) == 3)
				{
					int noOfFires = 0;
					float deltaAngle = 15;
					float scale = 6f;
					if (numberInCollectables(Collectables.PiercingFire) == 1)
					{
						noOfFires = 2;
						Game.score += 100;
						Game.maxRgbSplit += rgbValue;
					}
					else if (numberInCollectables(Collectables.PiercingFire) == 2)
					{
						noOfFires = 4;
						Game.score += 300;
						Game.maxRgbSplit += 3 * rgbValue;
					}
					else if (numberInCollectables(Collectables.PiercingFire) == 3)
					{
						noOfFires = 9;
						Game.score += 1000;
						Game.maxRgbSplit += 6 * rgbValue;
					}
					for (int i = 0; i < noOfFires; i++)
					{
						Game.playerFire.GetComponent<PlayerFire> ().direction = (i + 0.5f - noOfFires / 2f) * deltaAngle;
						PlayerFire playerFire = (Instantiate (Game.playerFire, transform.position + Vector3.forward, Quaternion.identity) as GameObject).GetComponent<PlayerFire>();
						playerFire.transform.localScale = Vector3.one * scale;
						playerFire.GetComponent<Collider2D>().isTrigger = true;
						playerFire.GetComponent<Rigidbody2D>().mass = 3f;
					}
				}
			}
			if (typeInCollectables(Collectables.Freeze))
			{
				if (Game.mode != GameMode.Hardcore || numberInCollectables(Collectables.Freeze) == 3)
				{
					int noOfFires = 0;
					float deltaAngle = 10;
					float freezePeriod = 0f;
					if (numberInCollectables(Collectables.Freeze) == 1)
					{
						freezePeriod = 3f;
						noOfFires = 3;
						Game.score += 100;
						Game.maxRgbSplit += rgbValue;
					}
					else if (numberInCollectables(Collectables.Freeze) == 2)
					{
						noOfFires = 5;
						freezePeriod = 3f;
						Game.score += 300;
						Game.maxRgbSplit += 3 * rgbValue;
					}
					else if (numberInCollectables(Collectables.Freeze) == 3)
					{
						noOfFires = 9;
						freezePeriod = 5f;
						Game.score += 1000;
						Game.maxRgbSplit += 6 * rgbValue;
					}
					for (int i = 0; i < noOfFires; i++)
					{
						Game.playerFireFreeze.GetComponent<PlayerFireFreeze> ().direction = (i + 0.5f - noOfFires / 2f) * deltaAngle;
						PlayerFireFreeze playerFire = (Instantiate (Game.playerFireFreeze, transform.position + Vector3.forward, Quaternion.identity) as GameObject).GetComponent<PlayerFireFreeze>();
						playerFire.GetComponent<Collider2D>().isTrigger = true;
						playerFire.GetComponent<Rigidbody2D>().mass = 4f;
						playerFire.freezePeriod = freezePeriod;
					}
				}
			}
			if (typeInCollectables(Collectables.Shield))
			{
				if (Game.mode != GameMode.Hardcore || numberInCollectables(Collectables.Shield) == 3)
				{
					Shield shield = (Instantiate(Game.shield) as GameObject).GetComponent<Shield>();
					if (numberInCollectables(Collectables.Shield) == 1)
					{
						shield.transform.localScale = Vector3.one / 3;
						shield.lifeTime = 2f;
						Game.score += 100;
						Game.maxRgbSplit += rgbValue;
						AudioSource.PlayClipAtPoint(Game.instance.audioShield.clip, Vector3.zero, 0.2f);
					}
					else if (numberInCollectables(Collectables.Shield) == 2)
					{
						shield.transform.localScale = Vector3.one / 2;
						shield.lifeTime = 4f;
						Game.score += 300;
						Game.maxRgbSplit += 3 * rgbValue;
						AudioSource.PlayClipAtPoint(Game.instance.audioShield.clip, Vector3.zero, 0.5f);
					}
					else if (numberInCollectables(Collectables.Shield) == 3)
					{
						shield.transform.localScale = Vector3.one;
						shield.lifeTime = 6f;
						Game.score += 1000;
						Game.maxRgbSplit += 6 * rgbValue;
						AudioSource.PlayClipAtPoint(Game.instance.audioShield.clip, Vector3.zero, 1f);
					}
				}
			}

			for (int i = 0; i < collectables.Count; i++)
			{
				Destroy (collectables [i].gameObject);
			}

			collectables = new List<Collectable> ();
		}

		public void Die()
		{
			// In the collection: see Game.StoryArtifactDeaths.
			Game.storyDeaths++;
			if (Game.storyDeaths >= Game.StoryArtifactDeaths) Collection.Story.StoryGames.Finish(1.5f);
			Game.instance.menu.SetActive (true);
			Game.instance.audioPlayerDeath.Play ();
			ExplosionScript.Create(transform.position, 5f);
			Destroy(gameObject);
		}

	//	void OnTriggerEnter2D(Collider2D coll)
	//	{
	//		Trigger (coll);
	//	}
	//	
	//	void OnTriggerStay2D(Collider2D coll)
	//	{
	//		Trigger (coll);
	//	}
	//	
	//	void Trigger(Collider2D coll)
	//	{
	//
	//	}

		public bool typeInCollectables(Collectables type)
		{
			for (int i = 0; i < collectables.Count; i++)
			{
				if (collectables[i].type == type)
				{
					return true;
				}
			}
			return false;
		}

		public int numberInCollectables(Collectables type)
		{
			int toReturn = 0;
			for (int i = 0; i < collectables.Count; i++)
			{
				if (collectables[i].type == type)
				{
					toReturn++;
				}
			}
			return toReturn;
		}
	}
}
