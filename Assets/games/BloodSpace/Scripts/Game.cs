using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.BloodSpace
{
	public enum GameMode
	{
		Normal,
		Hardcore
	}

	public class Game : MonoBehaviour {

		public static Game instance;
		public static float X_MAX = 9.6f;
		public static float Y_MAX = 5.4f;
		public static float PLAYER_OFFSET = 0.5f;
		public static float BackgroundSpeed = 1f;

		public static float maxBlur;
		public static float maxRgbSplit;

		public static GameObject player;
		public static GameObject playerFire;
		public static GameObject playerFireFreeze;
		public static GameObject playerFireExplosion;
		public static GameObject playerFireExplosionParticle;
		public static GameObject enemy;
		public static GameObject enemyFire;
		public static GameObject enemyFiring;
		public static GameObject enemyHead;
		public static GameObject explosion;
		public static GameObject collectable;
		public static GameObject shield;
		public static GameObject beam;
		public static GameObject powerup;

		public static bool mouseMode;
		public static GameMode mode;

		public static float screenShake;
		public static bool textReadable;
		public static int deadOctopus;

		public GameObject menu;
		public GameObject credits;
		public TextMesh scoreText;
		public AudioSource audioOctopusComing;
		public AudioSource audioOctopusDeath;
		public AudioSource audioPlayerDeath;
		public AudioSource audioLaser;
		public AudioSource audioShield;
		public AudioSource audioIce;

		private float enemyTimeCounter;
		private float enemyPeriod = 0.6f;
		private float octopusTimeCounter;
		private float octopusPeriod = 30f;
		private float firingTimeCounter;
		private float firingPeriod = 2f;

		private float textTimeCounter;
		private float textPeriod;
		public static int score; 

		void Awake ()
		{
			instance = this;

			// In the collection: the process outlives the game, so what a run left behind is cleared.
			score = 0;
			deadOctopus = 0;
			screenShake = 0f;
			textReadable = false;
			Shield.thereIs = 0;
			TintScript.changeWeatherColor(1f, 1f, 1f);
			maxBlur = 0f;
			maxRgbSplit = 0f;

			enemyTimeCounter = enemyPeriod;
			EnemyHead.octopusCounter = 0;
		}

		void Start ()
		{
			Music.volume = 0.5f;
			Music.audioSource.Play ();
		}

		public static void PrefabInit()
		{
			player = Resources.Load ("BloodSpace/Player") as GameObject;
			playerFire = Resources.Load ("BloodSpace/Player Fire") as GameObject;
			playerFireFreeze = Resources.Load ("BloodSpace/Player Fire Freeze") as GameObject;
			playerFireExplosion = Resources.Load ("BloodSpace/Player Fire Explosion") as GameObject;
			playerFireExplosionParticle = Resources.Load ("BloodSpace/Player Fire Explosion Particle") as GameObject;
			enemy = Resources.Load ("BloodSpace/Enemy") as GameObject;
			enemyFire = Resources.Load ("BloodSpace/Enemy Fire") as GameObject;
			enemyFiring = Resources.Load ("BloodSpace/Enemy Firing") as GameObject;
			enemyHead = Resources.Load ("BloodSpace/Enemy Head") as GameObject;
			explosion = Resources.Load ("BloodSpace/Explosion") as GameObject;
			collectable = Resources.Load ("BloodSpace/Collectable") as GameObject;
			shield = Resources.Load ("BloodSpace/Shield") as GameObject;
			beam = Resources.Load ("BloodSpace/Beam") as GameObject;
			powerup = Resources.Load ("BloodSpace/Powerup") as GameObject;
		}

		void Update ()
		{
			if (PlayerScript.instance != null)
			{
				Music.volume = Music.GameVolume;
				if (deadOctopus < 13 && (deadOctopus < 6 || mode != GameMode.Normal))
				{
					enemyTimeCounter += Time.deltaTime;
					if (enemyTimeCounter >= enemyPeriod)
					{
						enemyTimeCounter = 0f;
						float rand = Random.Range(0f, 4f);
						if (EnemyHead.octopusCounter < 1 || rand < 2f)
						{
							EnemyScript.Create(Vector3.up * (Game.Y_MAX + 2f) + Vector3.right * Random.Range (-Game.X_MAX + 4f, Game.X_MAX - 4f), 0);
						}
						else if (rand < 2.9f)
						{
							EnemyBasic enemyBasic = EnemyScript.Create(-Vector3.right * (Game.X_MAX + 2f) + Vector3.up * Random.Range (-Game.Y_MAX + 2f, Game.Y_MAX - 2f), 0).GetComponent<EnemyBasic>();
							enemyBasic.ChangeDirection(Vector2.right);
						}
						else if (rand < 3.8f)
						{
							EnemyBasic enemyBasic = EnemyScript.Create(Vector3.right * (Game.X_MAX + 2f) + Vector3.up * Random.Range (-Game.Y_MAX + 2f, Game.Y_MAX - 2f), 0).GetComponent<EnemyBasic>();
							enemyBasic.ChangeDirection(-Vector2.right);
						}
						else
						{
							EnemyBasic enemyBasic = EnemyScript.Create(-Vector3.up * (Game.Y_MAX + 2f) + Vector3.right * Random.Range (-Game.X_MAX + 4f, Game.X_MAX - 4f), 0).GetComponent<EnemyBasic>();
							enemyBasic.ChangeDirection(Vector2.up);
						}

					}

					firingTimeCounter += Time.deltaTime;
					if (firingTimeCounter >= firingPeriod)
					{
						firingTimeCounter = 0f;

						int firesPerShot = 1;
						if (Game.mode == GameMode.Hardcore)
						{
							if (EnemyHead.octopusCounter >= 6)
							{
								firesPerShot = 3;
							}
							else
							{
								firesPerShot = 2;
							}
						}

						float rand = Random.Range(0f, 4f);
						if (EnemyHead.octopusCounter < 1 || rand < 2f)
						{
							EnemyFiring enemy = EnemyScript.Create(Vector3.up * (Game.Y_MAX + 2f) + Vector3.right * Random.Range (-Game.X_MAX + 4f, Game.X_MAX - 4f), 1).GetComponent<EnemyFiring>();
							enemy.firesPerShot = firesPerShot;
						}
						else if (rand < 2.9f)
						{
							EnemyFiring enemy = EnemyScript.Create(-Vector3.right * (Game.X_MAX + 2f) + Vector3.up * Random.Range (-Game.Y_MAX + 2f, Game.Y_MAX - 2f), 1).GetComponent<EnemyFiring>();
							enemy.firesPerShot = firesPerShot;
							EnemyBasic enemyBasic = enemy.GetComponent<EnemyBasic>();
							enemyBasic.ChangeDirection(Vector2.right);
						}
						else if (rand < 3.8f)
						{
							EnemyFiring enemy = EnemyScript.Create(Vector3.right * (Game.X_MAX + 2f) + Vector3.up * Random.Range (-Game.Y_MAX + 2f, Game.Y_MAX - 2f), 1).GetComponent<EnemyFiring>();
							enemy.firesPerShot = firesPerShot;
							EnemyBasic enemyBasic = enemy.GetComponent<EnemyBasic>();
							enemyBasic.ChangeDirection(-Vector2.right);
						}
						else
						{
							EnemyFiring enemy = EnemyScript.Create(-Vector3.up * (Game.Y_MAX + 2f) + Vector3.right * Random.Range (-Game.X_MAX + 4f, Game.X_MAX - 4f), 1).GetComponent<EnemyFiring>();
							enemy.firesPerShot = firesPerShot;
							EnemyBasic enemyBasic = enemy.GetComponent<EnemyBasic>();
							enemyBasic.ChangeDirection(Vector2.up);
						}

					}

					octopusTimeCounter += Time.deltaTime;
					if (octopusTimeCounter >= octopusPeriod)
					{
						octopusTimeCounter = 0f;
						EnemyScript.Create(Vector3.up * (Game.Y_MAX + 5f) + Vector3.right * Random.Range (-Game.X_MAX + 4f, Game.X_MAX - 4f), 2);
						if (EnemyHead.octopusCounter == 2)
						{
							enemyPeriod = 0.6f;
							firingPeriod = 2f;
						}
						if (EnemyHead.octopusCounter == 4)
						{
							enemyPeriod = 0.5f;
							firingPeriod = 1.5f;
						}
						if (EnemyHead.octopusCounter== 9)
						{
							enemyPeriod = 1f;
							firingPeriod = 1f;
						}
					}
				}
				else if (!credits.activeSelf)
				{
					bool isThereEnemy = false;
					foreach (Transform child in transform)
					{
						if (child.name == "Enemy" || child.name == "Enemy Fire")
						{
							isThereEnemy = true;
							break;
						}
					}
					if (!isThereEnemy)
					{
						credits.SetActive(true);
						PlayerPrefs.SetInt("BloodSpace.Won", PlayerPrefs.GetInt("BloodSpace.Won",0) + 1);
					}
				}
				else
				{
					if (TaloketoInputManager.GetButtonDown("Select0") || TaloketoInputManager.GetButtonDown("Select1"))
					{
						GetComponent<AudioSource>().Play ();
						PlayerScript.script.Die();
						credits.SetActive(false);
					}
				}
			}
			else
			{
				Music.volume = Music.MenuVolume;
				if (textReadable)
				{
					TintScript.changeWeatherColor(1f, 0f ,0f);
				}
				else
				{
					TintScript.changeWeatherColor(1f, 1f ,1f);
				}

				if (TaloketoInputManager.GetButtonDown("Select0") || TaloketoInputManager.GetButtonDown("Select1"))
				{
	//				if (TaloketoInputManager.GetMouseButtonDown(0))
	//				{
	//					mouseMode = true;
	//				}
	//				else
	//				{
	//					mouseMode = false;
	//				}
					Reset();
					menu.SetActive(false);
					credits.SetActive(false);
				}
			}

			if (screenShake > 0)
			{
				if (screenShake > 2)
				{
					screenShake -= 25 * Time.deltaTime;
				}
				else if (screenShake > 1)
				{
					screenShake -= 10 * Time.deltaTime;
				}
				else if (screenShake > 0.2)
				{
					screenShake -= 2 * Time.deltaTime;
				}
				else
				{
					screenShake -= Time.deltaTime;
				}
				transform.position = Geometry.createVector3(Random.Range (0f, 360f), screenShake);
			}
			else
			{
				transform.position = Vector3.zero;
			}

			if (maxBlur > 0)
			{
				if (maxBlur > 2)
				{
					maxBlur -= 25 * Time.deltaTime;
				}
				else if (maxBlur > 1)
				{
					maxBlur -= 10 * Time.deltaTime;
				}
				else if (maxBlur > 0.2)
				{
					maxBlur -= 2 * Time.deltaTime;
				}
				else
				{
					maxBlur -= Time.deltaTime;
				}
			}
			else
			{
				maxBlur = 0f;
			}
			if (maxRgbSplit > 0)
			{
				if (maxRgbSplit > 2)
				{
					maxRgbSplit -= 25 * Time.deltaTime;
				}
				else if (maxRgbSplit > 1)
				{
					maxRgbSplit -= 10 * Time.deltaTime;
				}
				else if (maxRgbSplit > 0.2)
				{
					maxRgbSplit -= 2 * Time.deltaTime;
				}
				else
				{
					maxRgbSplit -= Time.deltaTime;
				}
			}
			else
			{
				maxRgbSplit = 0f;
			}
			SpriteEffect.blurConst = maxBlur;
			SpriteEffect.rgbSplitConst = maxRgbSplit;

			textTimeCounter += Time.deltaTime;
			if (textTimeCounter >= textPeriod)
			{
				textTimeCounter = 0f;
				if (textPeriod != 0.2f)
				{
					textPeriod = 0.2f;
				}
				else
				{
					while (textPeriod == 0.2f)
					{
						textPeriod = Random.Range (5f, 10f);
					}
				}
				textReadable = !textReadable;
			}

			scoreText.text = "SCORE: " + score;
		}

		void Reset()
		{
			GetComponent<AudioSource>().Play ();
			foreach (Transform child in transform)
			{
				Destroy (child.gameObject);
			}
			EnemyHead.octopusCounter = 0;
			enemyTimeCounter = 0f;
			firingTimeCounter = 0f;
			octopusTimeCounter = 0f;
			enemyPeriod = 1f;
			octopusPeriod = 30f;
			firingPeriod = 3f;
			TintScript.changeWeatherColor(1f, 1f ,1f);
			Instantiate(Game.player);
			deadOctopus = 0;
			score = 0;
		}
	}
}
