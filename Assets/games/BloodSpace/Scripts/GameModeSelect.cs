using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.BloodSpace
{
	public class GameModeSelect : MonoBehaviour {

		public TextBlood text;
		private float timeCounter;
		private float period = 0.1f;
		void Start ()
		{
			if (PlayerPrefs.GetInt("BloodSpace.GameMode", 0) == 0)
			{
				Game.mode = GameMode.Normal;
			}
			else if (PlayerPrefs.GetInt("BloodSpace.GameMode", 0) == 1)
			{
				Game.mode = GameMode.Hardcore;
			}
		}

		void Update ()
		{
			timeCounter += Time.deltaTime;

			if (timeCounter > 2 * period)
			{
				timeCounter -= 2 * period;
			}

			// In the collection: was KeyCode.C; "GameMode" is C and the gamepad's Y.
			if (TaloketoInputManager.GetButtonDown("GameMode"))
			{
				if (Game.mode == GameMode.Normal)
				{
					Game.mode = GameMode.Hardcore;
					PlayerPrefs.SetInt("BloodSpace.GameMode", 1);
				}
				else if (Game.mode == GameMode.Hardcore)
				{
					Game.mode = GameMode.Normal;
					PlayerPrefs.SetInt("BloodSpace.GameMode", 0);
				}
			}

			if (Game.mode == GameMode.Normal)
			{
				text.text = "GAME MODE: NORMAL";
				text.color = Color.red;
			}
			else if (Game.mode == GameMode.Hardcore)
			{
				text.text = "GAME MODE: HARDCORE";
				if (timeCounter < period)
				{
					text.color = Color.red;
				}
				else
				{
					text.color = Color.white;
				}
			}
		}
	}
}
