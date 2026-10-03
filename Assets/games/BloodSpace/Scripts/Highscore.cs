using UnityEngine;
using System.Collections;

namespace Games.BloodSpace
{
	public class Highscore : MonoBehaviour {

		private TextBlood textBlood;

		void Start ()
		{
			textBlood = GetComponent<TextBlood> ();
			textBlood.text = ": " + PlayerPrefs.GetInt ("BloodSpace.highscore", 0);
		}

		void Update ()
		{
			if (Game.score > PlayerPrefs.GetInt ("BloodSpace.highscore", 0))
			{
				PlayerPrefs.SetInt("BloodSpace.highscore", Game.score);
				textBlood.text = ": " + Game.score;
			}
		}
	}
}
