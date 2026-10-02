using UnityEngine;
using System.Collections;

public class Highscore : MonoBehaviour {

	private TextBlood textBlood;

	void Start ()
	{
		textBlood = GetComponent<TextBlood> ();
		textBlood.text = ": " + PlayerPrefs.GetInt ("highscore", 0);
	}

	void Update ()
	{
		if (Game.score > PlayerPrefs.GetInt ("highscore", 0))
		{
			PlayerPrefs.SetInt("highscore", Game.score);
			textBlood.text = ": " + Game.score;
		}
	}
}
