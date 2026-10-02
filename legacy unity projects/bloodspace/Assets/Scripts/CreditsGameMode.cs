using UnityEngine;
using System.Collections;

public class CreditsGameMode : MonoBehaviour {

	public TextBlood text;

	void Start ()
	{
		if (Game.mode == GameMode.Normal)
		{
			text.text = "YOU BEAT NORMAL";
		}
		else if (Game.mode == GameMode.Hardcore)
		{
			text.text = "YOU BEAT HARDCORE";
		}
	}

	void Update ()
	{

	}
}
