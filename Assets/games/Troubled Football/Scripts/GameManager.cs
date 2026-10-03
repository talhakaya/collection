using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.TroubledFootball
{
	public class GameManager : MonoBehaviour {

		public static int effect;
		public static float mouseX = 0f;
		public static int gameState = 0;
		public static bool eyesore = true;
		private float timeCounter = 0f;
		private float period = 0.2f;
		private float deltaColor = 0.3f;
		public TextMesh goalText;

		// In the collection: the process outlives the game, so the statics start over here.
		void Awake ()
		{
			gameState = 0;
			eyesore = true;
			mouseX = 0f;
			TintScript.weatherColor = new Color (1f, 1f, 1f);
			AudioManager.musicOnOff = true;
			AudioManager.pitch = 1f;
			AudioManager.playGoal = false;
			AudioManager.playKick = false;
			AudioManager.playBall = false;
		}

		// In the collection: Unity only sends OnMouseUp with the old Input Manager, so the
		// four buttons ask this in their Update and call their OnMouseUp themselves.
		public static bool clickedOn(GameObject go)
		{
			if (!TaloketoInputManager.GetMouseButtonUp(0))
			{
				return false;
			}
			Vector3 screen = TaloketoInputManager.mousePosition;
			Collider2D c2 = go.GetComponent<Collider2D>();
			if (c2 != null)
			{
				return c2.OverlapPoint(Camera.main.ScreenToWorldPoint(screen));
			}
			Collider c3 = go.GetComponent<Collider>();
			if (c3 != null)
			{
				RaycastHit hit;
				return c3.Raycast(Camera.main.ScreenPointToRay(screen), out hit, 1000f);
			}
			return false;
		}

		void Start ()
		{
			effect = 1;
		}

		void Update ()
		{
			if (TaloketoInputManager.GetButtonDown("Eyesore"))
			{
				eyesore = !eyesore;
			}

			// In the collection: the Escape-quit is gone (the collection has its own exit).


			mouseX = Camera.main.ScreenToWorldPoint(TaloketoInputManager.mousePosition).x;
			float rand = Random.Range (0f, 1f);

			timeCounter += Time.deltaTime;
			if (timeCounter > period)
			{
				timeCounter = 0f;
				if (rand < 0.95f)
				{
					effect = 1;
					AudioManager.pitch = 1f;
				}
				else if (rand < 0.97f)
				{
					effect = 2;
					AudioManager.pitch = 1.5f;
				}
				else
				{
					effect = 3;
					AudioManager.pitch = 0.5f;
				}
			}



			if (!eyesore)
			{
				effect = 1;
				AudioManager.pitch = 1f;
				TintScript.changeWeatherColor (1f, 1f, 1f);
			}
			else
			{
				float rand1 = Random.Range(0f, 4f);
				float rand2 = Random.Range(0f, deltaColor);
				if (rand1 <= 1f)
				{
					TintScript.changeWeatherColor (Random.Range (1f - deltaColor / 3, 1f), Random.Range (1f - deltaColor / 3, 1f), Random.Range (1f - deltaColor / 3, 1f));
				}
				else if (rand1 <= 2f)
				{
					TintScript.changeWeatherColor (1f, 1f - rand2, 1f - 0.45f + rand2);
				}
				else if (rand1 <= 3f)
				{
					TintScript.changeWeatherColor (1f - rand2, 1f, 1f - 0.45f + rand2);
				}
				else
				{
					TintScript.changeWeatherColor (1f - 0.45f + rand2, 1f - rand2, 1f);
				}
			}

			if (gameState == 0)
			{
				goalText.text = "";
			}
			else if (gameState == 1)
			{
				goalText.text = "GOAL!!!!!";
			}
			else if (gameState == 2)
			{
				goalText.text = "OUT :(";
			}

		}
	}
}
