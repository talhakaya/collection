using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.Herbie
{
	public class Game : MonoBehaviour {

		public static float dt;
		public static float time;
		public static bool input;
		private static bool inputOld;
		public static bool inputDown;
		public static bool inputUp;
		public static Color color0 = new Color(183 / 255f, 162 / 255f, 130 / 255f);
		public static Color color1 = new Color(193 / 255f, 106 / 255f, 68 / 255f);
		public static Color color2 = new Color(170 / 255f, 68 / 255f, 68 / 255f);
		public static Color color3 = new Color(189 / 255f, 68 / 255f, 193 / 255f);
		public static Color color4 = new Color(110 / 255f, 64 / 255f, 183 / 255f);
		public static Color[] colors = new Color[5] {color0, color1, color2, color3, color4};
		public static float herbieMultiplier = 1f;
		public static Game instance;
		public GameObject main;
		public GameObject world;
		public GameObject drug;
		public GameObject mother;
		public GameObject grave;
		public GameObject hammer;
		public AudioSource music;
		public static bool isThereMiniGame = false;
		public static GameObject miniGame;
		public static bool cokeTaken;
		public static bool cokeGiven;
		public static bool muscleBeaten;
		public static float DRUGTIMER = 10f;
		public static float drugTimer = 0f;
		public static float momLove = 0f;
		public AudioSource aTalk;
		public AudioSource aBush;
		public AudioSource aKiss;
		public AudioSource aCoke;
		public AudioSource aStep;
		public AudioSource aHammer;
		public AudioSource aFist;
		public AudioSource aFistHit;

		void Awake ()
		{
			instance = this;

			// In the collection: statics outlive the game here, where they used to die with
			// the application. Left as they were, a drug trip or a mini-game that was running
			// when the player went back to the menu would still be on at the next start.
			isThereMiniGame = false;
			miniGame = null;
			herbieMultiplier = 1f;
			drugTimer = 0f;
			TintScript.changeWeatherColor(1f, 1f, 1f);
		}

		void Start ()
		{
			time = 0f;
			Physics2D.gravity = new Vector2(0f,0f);
			momLove = 0f;
			cokeTaken = false;
			cokeGiven = false;
			muscleBeaten = false;
			Pacman.cokeTaken = false;
		}

		void Update ()
		{
			float cheatMultiplier = 1f;
			if (TaloketoInputManager.GetButton("SpeedUp"))
			{
				cheatMultiplier = 2f;
			}
			if (herbieMultiplier > 1f)
			{
				herbieMultiplier -= Time.deltaTime * 5f;
			}
			else if (herbieMultiplier < 1f)
			{
				herbieMultiplier = 1f;
			}

			dt = Time.deltaTime * herbieMultiplier * cheatMultiplier;
			time += dt;

			if (drugTimer > 0)
			{
				drugTimer -= dt;
				TintScript.changeWeatherColor(0.5f + modd(time, 1f) / 2f, 0.5f + modd(time, 0.5f) / 2f, 0.5f + modd(time, 1.5f) / 2f);
			}
			else if (TintScript.weatherColor != new Color(1f, 1f, 1f))
			{
				TintScript.changeWeatherColor(1f, 1f, 1f);
			}

			if (world.transform.position != Vector3.zero)
			{
				if (TalhaTexting.iGet(mother) == 0 && time > 30f)
				{
					momAlpha();
				}
				if (TalhaTexting.iGet(mother) == 1 && time > 60f)
				{
					momAlpha();
				}
				if (TalhaTexting.iGet(mother) == 2 && time > 90f)
				{
					momAlpha();
				}
				if (TalhaTexting.iGet(mother) == 3 && time > 120f)
				{
					momAlpha();
				}
				if (TalhaTexting.iGet(mother) == 4 && time > 150f)
				{
					momAlpha();
				}
				if (TalhaTexting.iGet(mother) == 5 && time > 180f)
				{
					if (momLoveCondition())
					{
						mother.SetActive(false);
						grave.SetActive(true);
					}
					else
					{
						momAlpha();
					}
				}
			}

			MousePosition.get = Camera.main.ScreenToWorldPoint (TaloketoInputManager.mousePosition) + Vector3.forward;
			MousePosition.x = MousePosition.get.x;
			MousePosition.y = MousePosition.get.y;

			input = TaloketoInputManager.GetMouseButton (0);
			inputDown = input && !inputOld;
			inputUp = !input && inputOld;

			inputOld = input;
		}

		public static bool momLoveCondition()
		{
			return momLove < 3f;
		}

		private void momAlpha()
		{
			TalhaTexting.next(mother);
			TintScript tint = mother.GetComponent<TintScript> ();
			TalhaTexting texting = mother.GetComponent<TalhaTexting> ();
			tint.selfColor = new Color (tint.selfColor.r, tint.selfColor.g, tint.selfColor.b, 0.25f + 0.75f * (texting.texts.Length - texting.i) / texting.texts.Length);
		}

		public static float modd(float value, float max)
		{
			if (value < max / 2f)
			{
				return (value % (max / 2f)) * 2f / max;
			}
			else
			{
				return ((max - (value % max)) % (max / 2f)) * 2f / max;
			}
		}

		public static void startMiniGame(string id)
		{
			instance.main.SetActive(false);
			isThereMiniGame = true;
			GameObject minigame = Resources.Load ("Herbie/" + id) as GameObject; // in the collection: its own Resources subfolder
			miniGame = Instantiate (minigame, Vector3.zero, Quaternion.identity) as GameObject;
		}

		public static void endMiniGame()
		{
			Destroy (miniGame);
			isThereMiniGame = false;
			instance.main.SetActive(true);
		}
	}
}
