using UnityEngine;
using System.Collections;

namespace Games.BloodSpace
{
	public class EnemyHead : MonoBehaviour {

		private EnemyScript enemyScript;
		public float speed;
		private static int noOfArms = 4;
		private static int lengthOfArms = 3;
		private static int noOfFiringEnemiesPerArm = 0;
		public static int octopusCounter = 4;
		private float timeCounter;
		public float firePeriod;
		public int firesPerShot;

		void Start ()
		{
			if (Game.mode == GameMode.Hardcore)
			{
				if (octopusCounter < 6)
				{
					firePeriod = firePeriod / 2;
				}
				else
				{
					firePeriod = firePeriod / 3;
				}
			}

			enemyScript = GetComponent<EnemyScript> ();
			Game.instance.audioOctopusComing.Play ();
			enemyScript.hp = 500f;
			if (octopusCounter == 0)
			{
				noOfArms = 3;
				lengthOfArms = 4;
				noOfFiringEnemiesPerArm = 0;
			}
			else if (octopusCounter == 1)
			{
				noOfArms = 4;
				lengthOfArms = 5;
				noOfFiringEnemiesPerArm = 0;
			}
			else if (octopusCounter == 2)
			{
				noOfArms = 3;
				lengthOfArms = 3;
				noOfFiringEnemiesPerArm = 1;
			}
			else if (octopusCounter == 3)
			{
				noOfArms = 5;
				lengthOfArms = 4;
				noOfFiringEnemiesPerArm = 1;
			}
			else if (octopusCounter == 4)
			{
				noOfArms = 4;
				lengthOfArms = 3;
				noOfFiringEnemiesPerArm = 2;
			}
			else if (octopusCounter == 5)
			{
				noOfArms = 4;
				lengthOfArms = 5;
				noOfFiringEnemiesPerArm = 3;
			}
			else if (octopusCounter == 6)
			{
				noOfArms = 3;
				lengthOfArms = 5;
				noOfFiringEnemiesPerArm = 5;
			}
			else if (octopusCounter == 7)
			{
				noOfArms = 5;
				lengthOfArms = 4;
				noOfFiringEnemiesPerArm = 1;
			}
			else if (octopusCounter == 8)
			{
				noOfArms = 4;
				lengthOfArms = 3;
				noOfFiringEnemiesPerArm = 2;
			}
			else if (octopusCounter == 9)
			{
				noOfArms = 4;
				lengthOfArms = 5;
				noOfFiringEnemiesPerArm = 3;
			}
			else if (octopusCounter == 10)
			{
				noOfArms = 3;
				lengthOfArms = 5;
				noOfFiringEnemiesPerArm = 5;
			}
			else if (octopusCounter == 11)
			{
				noOfArms = 4;
				lengthOfArms = 5;
				noOfFiringEnemiesPerArm = 5;
			}
			else if (octopusCounter == 12)
			{
				noOfArms = 5;
				lengthOfArms = 5;
				noOfFiringEnemiesPerArm = 5;
			}

			octopusCounter++;
			for (int j = 0; j < noOfArms; j++)
			{
				EnemyScript enemyMain = enemyScript;
				for (int i = 0; i < lengthOfArms; i++)
				{
					if (i < noOfFiringEnemiesPerArm)
					{
						if (i < 1)
						{
							EnemyScript enemy = EnemyScript.Create (transform.position + Geometry.createVector3(i * 60, 0.5f) + Vector3.forward * 0.1f, 1);
							enemyMain.JointEnemy (enemy, -j * 360f / noOfArms, 0.9f);
							enemyMain = enemy;
						}
						else
						{
							EnemyScript enemy = EnemyScript.Create (transform.position + Geometry.createVector3(i * 60, 0.5f) + Vector3.forward * 0.1f, 1);
							enemyMain.JointEnemy (enemy, -j * 360f / noOfArms, 0.6f);
							enemyMain = enemy;
						}
					}
					else
					{
						if (noOfFiringEnemiesPerArm > 0 && i == noOfFiringEnemiesPerArm)
						{
							EnemyScript enemy = EnemyScript.Create (transform.position + Geometry.createVector3(i * 60, 0.5f) + Vector3.forward * 0.1f, 0);
							enemyMain.JointEnemy (enemy, -j * 360f / noOfArms, 0.9f);
							enemyMain = enemy;
						}
						else
						{
							EnemyScript enemy = EnemyScript.Create (transform.position + Geometry.createVector3(i * 60, 0.5f) + Vector3.forward * 0.1f, 0);
							enemyMain.JointEnemy (enemy, -j * 360f / noOfArms, 1f);
							enemyMain = enemy;
						}
					}
				}
			}
			if (speed == 0f)
			{
				speed = Random.Range (2000f, 3000f);
			}
		}

		void Update ()
		{
			GetComponent<Rigidbody2D>().AddForce(Geometry.normalizeVector2(new Vector2(-transform.position.x, -transform.position.y + Game.Y_MAX / 4), speed) * forcePerFrame());

			if (PlayerScript.instance != null && !enemyScript.frozen)
			{
				timeCounter += Time.deltaTime;
				if (timeCounter > firePeriod)
				{
					timeCounter = 0f;
					Fire();
				}
			}
		}

		// In the collection: the force above was added once per frame whatever the frame rate, so
		// the octopus pulled harder the faster the game ran. This is 1 at 60 frames a second.
		float forcePerFrame()
		{
			return Time.deltaTime * 60f;
		}

		void Fire()
		{
			for (int i = 0; i < firesPerShot; i++)
			{
				Game.enemyFire.GetComponent<EnemyFire> ().direction = Geometry.angleOfVector3(transform.position - PlayerScript.instance.position) + 90 + (i + 0.5f - firesPerShot / 2f) * 30f;
				Instantiate (Game.enemyFire, transform.position + Vector3.forward, Quaternion.identity);
			}
		}
	}
}
