using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Score : MonoBehaviour {

	public Text scoreText;
	public int scoreToNextLevel;
	private float score;
	private Vector3 pos;

	void Start ()
	{
		pos = transform.position;
	}

	void Update ()
	{
		float x = Mathf.Abs(transform.position.x - pos.x);
		float y = Mathf.Abs(transform.position.z - pos.z);
		score += Mathf.Sqrt(x * x + y * y) * ((Input.GetKey(KeyCode.I) && Input.GetKey(KeyCode.O) && Input.GetKey(KeyCode.P))? 100f : 1f);
		scoreText.text = "SCORE: " + Mathf.Round (score) + " / " + scoreToNextLevel;
		pos = transform.position;
		if (score > scoreToNextLevel)
		{
			if (Application.loadedLevel == 9)
			{
				Application.Quit();
				Debug.Log ("QUIT!");
			}
			else
			{
				Application.LoadLevel(Application.loadedLevel + 1);
			}
		}
	}
}
