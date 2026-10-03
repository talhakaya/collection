using UnityEngine;
using System.Collections;

namespace Games.Orphan
{
	public enum GameArcadeKind
	{
		NonArcade,
		AvoidEnemies,
		SwallowPill,
		Crying
	};

	public class ArcadeGameManager : MonoBehaviour
	{
		public enum ArcadeGameManagerState
		{
			None,
			FadingIn,
			Playing,
			Won,
			Lost,
			Tie,
			FadingOut
		}

		public static ArcadeGameManager instance;
		public static bool isWon;

		private float winLostTimeToFadeOut = 2.5f;
		private const float textWinLostPeriod = 0.4f;
		private float textWinLostCounter = 0;
		private TextTalha textWinLost;
		private float oldZoomFactor;

		public GameArcadeKind gameArcadeKind;
		public ArcadeGameManagerState arcadeGameManagerState;
		public bool isPaused;

		void Awake()
		{
			instance = this;
		}

		void Start ()
		{
			GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0f);
			//transform.localScale = transform.localScale / CameraScript.instance.cameraTk2d.ZoomFactor;
			arcadeGameManagerState = ArcadeGameManagerState.FadingIn;
			isWon = false;
		}

		void Update ()
		{
			if (arcadeGameManagerState == ArcadeGameManagerState.FadingIn)
			{
				CameraScript.instance.followingPlayer = false;
				if (GetComponent<Renderer>().material.color.a < 1f)
				{
					PlayerScript.instance.canWalk = false;
					GetComponent<Renderer>().material.color = new Color(0f, 0f, 0f, GetComponent<Renderer>().material.color.a + 0.5f * Time.deltaTime);
					transform.position = CameraScript.instance.transform.position + 2 * Vector3.forward;
				}
				else
				{
					arcadeGameManagerState = ArcadeGameManagerState.Playing;
					oldZoomFactor = CameraScript.instance.cameraTk2d.ZoomFactor;
					CameraScript.changeZoom(1f);
					CameraScript.instance.zoomFactor = 1f;

					if (gameArcadeKind == GameArcadeKind.AvoidEnemies)
					{
						gameObject.AddComponent<ArcadeAvoidEnemies>();
					}
					else if (gameArcadeKind == GameArcadeKind.SwallowPill)
					{
						gameObject.AddComponent<ArcadeSwallowPill>();
					}
					else if (gameArcadeKind == GameArcadeKind.Crying)
					{
						gameObject.AddComponent<ArcadeCrying>();
					}
				}
			}
			else if (arcadeGameManagerState == ArcadeGameManagerState.Playing)
			{


				if (PlayerScript.instance.transform.position.x > CameraScript.instance.transform.position.x + 48)
				{
					PlayerScript.instance.transform.position = new Vector3(CameraScript.instance.transform.position.x + 48, 
						PlayerScript.instance.transform.position.y, PlayerScript.instance.transform.position.z);
				}
				else if (PlayerScript.instance.transform.position.x < CameraScript.instance.transform.position.x - 48)
				{
					PlayerScript.instance.transform.position = new Vector3(CameraScript.instance.transform.position.x - 48, 
						PlayerScript.instance.transform.position.y, PlayerScript.instance.transform.position.z);
				}

				if (PlayerScript.instance.transform.position.y > CameraScript.instance.transform.position.y + 27)
				{
					PlayerScript.instance.transform.position = new Vector3(PlayerScript.instance.transform.position.x, 
						CameraScript.instance.transform.position.y + 27, PlayerScript.instance.transform.position.z);
				}
				else if (PlayerScript.instance.transform.position.y < CameraScript.instance.transform.position.y - 27)
				{
					PlayerScript.instance.transform.position = new Vector3(PlayerScript.instance.transform.position.x,
						CameraScript.instance.transform.position.y - 27, PlayerScript.instance.transform.position.z);
				}
			}
			else if (arcadeGameManagerState == ArcadeGameManagerState.Won || arcadeGameManagerState == ArcadeGameManagerState.Lost || arcadeGameManagerState == ArcadeGameManagerState.Tie)
			{
				if (textWinLost == null)
				{
					GameObject objectTextWinLost = TextTalha.create(CameraScript.instance.transform.position + Vector3.forward * 0.5f, 
						"", 5, 5, Color.white, 10f, 0.05f, new Vector2(0.10f, -0.10f), true);
					textWinLost = objectTextWinLost.GetComponent<TextTalha>();
					objectTextWinLost.transform.parent = transform;
					if (arcadeGameManagerState == ArcadeGameManagerState.Won)
					{
						isWon = true;
						textWinLost.color = new Color(0.133f, 0.667f, 0.133f, 0f);
						if (GameManagerScript.lang == Language.Eng)
						{
							textWinLost.text = "You Win";
						}
						else if (GameManagerScript.lang == Language.Tur)
						{
							textWinLost.text = "Kazandın";
						}
					}
					else if (arcadeGameManagerState == ArcadeGameManagerState.Lost)
					{
						isWon = false;
						textWinLost.color = new Color(0.667f, 0.133f, 0.133f, 0f);
						if (GameManagerScript.lang == Language.Eng)
						{
							textWinLost.text = "You Lost";
						}
						else if (GameManagerScript.lang == Language.Tur)
						{
							textWinLost.text = "Kaybettin";
						}
					}
					else if (arcadeGameManagerState == ArcadeGameManagerState.Tie)
					{
						isWon = false;
						textWinLost.color = new Color(0.133f, 0.133f, 0.667f, 0f);
						if (GameManagerScript.lang == Language.Eng)
						{
							textWinLost.text = "Done";
						}
						else if (GameManagerScript.lang == Language.Tur)
						{
							textWinLost.text = "Bitti";
						}
					}
				}
				textWinLostCounter += Time.deltaTime;
				winLostTimeToFadeOut -= Time.deltaTime;
				if (textWinLostCounter > textWinLostPeriod)
				{
					textWinLostCounter = 0f;
					if (textWinLost.color.a > 0)
					{
						textWinLost.color = new Color(textWinLost.color.r, textWinLost.color.g, textWinLost.color.b, 0);
					}
					else
					{
						textWinLost.color = new Color(textWinLost.color.r, textWinLost.color.g, textWinLost.color.b, 1);
					}
				}

				if (winLostTimeToFadeOut <= 0)
				{
					arcadeGameManagerState = ArcadeGameManagerState.FadingOut;
				}
			}
			else if (arcadeGameManagerState == ArcadeGameManagerState.FadingOut)
			{
				transform.position = CameraScript.instance.transform.position + 2 * Vector3.forward;
				CameraScript.zoomInOut(oldZoomFactor, 0.2f);

				if (GetComponent<Renderer>().material.color.a > 0f)
				{
					GetComponent<Renderer>().material.color = new Color(0f, 0f, 0f, GetComponent<Renderer>().material.color.a - 0.5f * Time.deltaTime);
					if (textWinLost != null)
					{
						textWinLost.color = new Color(textWinLost.color.r, textWinLost.color.g, textWinLost.color.b, GetComponent<Renderer>().material.color.a);
						PlayerScript.instance.transform.position -= 2 * Vector3.forward;
					}
				}
				else
				{
					isPaused = false;
					PlayerScript.instance.canWalk = true;
					CameraScript.instance.followingPlayer = true;
					Destroy(gameObject);
					PlayerScript.instance.transform.position += 2 * Vector3.forward;
				}
			}
		}

		public static ArcadeGameManager createNewArcadeGame(GameArcadeKind id)
		{
			if (instance == null)
			{
				GameObject box = Resources.Load ("Orphan/ArcadeGame") as GameObject;
				box = Instantiate(box) as GameObject;
				ArcadeGameManager game = box.GetComponent<ArcadeGameManager>();
				game.gameArcadeKind = id;
				return game;
			}
			return null;
		}
	}
}
