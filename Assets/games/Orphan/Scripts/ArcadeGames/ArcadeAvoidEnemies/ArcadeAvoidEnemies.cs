using UnityEngine;
using System.Collections;

namespace Games.Orphan
{
	public class ArcadeAvoidEnemies : MonoBehaviour {

		public static ArcadeAvoidEnemies instance;

		private SpriteAlphaState playersRealSpriteAlphaState;
		private Vector3 playersRealPosition;
		private float playersRealSpeed;
		private float timeCount;
		private TextTalha timeText;
		private float periodCounter;
		private GameObject arcadeEnemy;

		public bool makeEnemiesStop;
		public float period;
		public float timeLimit;

		void Awake()
		{
			instance = this;
		}

		void Start ()
		{
			timeCount = 0.0f;
			timeLimit = 15.0f;
			arcadeEnemy = Resources.Load ("Orphan/arcadeEnemy") as GameObject;
			periodCounter = 0f;
			period = 0.5f;

			PlayerScript.instance.canWalk = true;
			playersRealPosition = PlayerScript.instance.gameObject.transform.position;
			playersRealSpeed = PlayerScript.instance.speed;
			playersRealSpriteAlphaState = PlayerScript.instance.spriteAlphaState;
			PlayerScript.instance.spriteType = GameArcadeKind.AvoidEnemies;
			PlayerScript.instance.spriteAlphaState = SpriteAlphaState.Normal;
			PlayerScript.instance.zOffset = 8.5f;
			PlayerScript.instance.speed = 1000f;
			GameObject textTalha = TextTalha.create(CameraScript.instance.transform.position + Vector3.forward * 0.5f + Vector3.up * 20f, 
				timeLimit.ToString(), 5, 5, Color.white, 25f, 0.05f, new Vector2(0.10f, -0.10f), true);
			textTalha.transform.parent = transform;
			timeText = textTalha.GetComponent<TextTalha>();
		}

		void Update ()
		{
			if (ArcadeGameManager.instance.arcadeGameManagerState == ArcadeGameManager.ArcadeGameManagerState.Playing)
			{
				if (timeLimit != 0.0f)
				{
					timeText.text = (timeLimit - timeCount).ToString("0.00");
					if (timeCount < timeLimit)
					{
						if (!ArcadeGameManager.instance.isPaused)
						{
							timeCount += Time.deltaTime;
						}
					}
					else
					{
						timeText.text = "0.00";
						ArcadeGameManager.instance.isPaused = true;
						makeEnemiesStop = true;
						ArcadeGameManager.instance.arcadeGameManagerState = ArcadeGameManager.ArcadeGameManagerState.Won;
					}
				}

				if (periodCounter <= period)
				{
					periodCounter += Time.deltaTime;
				}
				else
				{
					periodCounter = 0f;
					Instantiate(arcadeEnemy, new Vector3(CameraScript.instance.transform.position.x + Random.Range (-48f, 48f),
						CameraScript.instance.transform.position.y + 30f, PlayerScript.instance.transform.position.z), Quaternion.identity);
				}
			}
			else if (ArcadeGameManager.instance.arcadeGameManagerState == ArcadeGameManager.ArcadeGameManagerState.FadingOut)
			{
				if (PlayerScript.instance.spriteType != GameArcadeKind.NonArcade)
				{
					/*PlayerScript.instance.spriteType = GameArcadeState.NonArcade;
					PlayerScript.instance.gameObject.transform.position = playersRealPosition;
					CameraScript.instance.followingPlayer = true;
					PlayerScript.instance.zOffset = 0.0f;*/
					Destroy(PlayerScript.instance.gameObject);
					Instantiate(Resources.Load ("Orphan/Player"), playersRealPosition, Quaternion.identity);
					PlayerScript.instance.name = "Player";
					PlayerScript.instance.spriteAlphaState = playersRealSpriteAlphaState;
					PlayerScript.instance.speed = playersRealSpeed;
				}
				if (GetComponent<Renderer>().material.color.a > 0f)
				{
					timeText.color = new Color(timeText.color.r, timeText.color.g, timeText.color.b, GetComponent<Renderer>().material.color.a);
				}
			}
		}

		public static void touchedEnemy ()
		{
			ArcadeGameManager.instance.isPaused = true;
			ArcadeGameManager.instance.arcadeGameManagerState = ArcadeGameManager.ArcadeGameManagerState.Lost;
			instance.makeEnemiesStop = true;
		}
	}
}
