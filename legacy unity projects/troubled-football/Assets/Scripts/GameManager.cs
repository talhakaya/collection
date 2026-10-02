using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static int effect;
	public static float mouseX = 0f;
	public static int gameState = 0;
	public static bool eyesore = true;
	private float timeCounter = 0f;
	private float period = 0.2f;
	private float deltaColor = 0.3f;
	public TextMesh goalText;

	void Start ()
	{
		effect = 1;
	}

	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.N))
		{
			eyesore = !eyesore;
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}


		mouseX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
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
