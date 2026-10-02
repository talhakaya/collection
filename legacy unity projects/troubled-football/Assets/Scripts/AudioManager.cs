using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

	public static bool musicOnOff = true;
	public AudioSource music;
	public AudioSource goal;
	public AudioSource kick;
	public static float pitch = 1f;
	public static bool playGoal = false;
	public static bool playKick = false;
	public static bool playBall = false;


	void Start ()
	{
		music.volume = 0f;
	}

	void Update ()
	{
		music.pitch = pitch;
		goal.pitch = pitch;

		if (playGoal)
		{
			goal.Play();
			playGoal = false;
		}
		if (playKick)
		{
			kick.pitch = 1f;
			kick.volume = 1f;
			kick.Play();
			playKick = false;
		}
		if (playBall)
		{
			kick.pitch = Random.Range(0.5f, 0.7f);
			kick.volume = 0.5f;
			kick.Play();
			playBall = false;
		}

		if (Input.GetKeyDown(KeyCode.M))
		{
			musicOnOff = !musicOnOff;
		}

		if (musicOnOff)
		{
			if (music.volume < 1f)
			{
				music.volume += Time.deltaTime;
			}
			else if (music.volume > 1f)
			{
				music.volume = 1f;
			}
		}
		else
		{
			if (music.volume > 0f)
			{
				music.volume -= Time.deltaTime;
			}
			else if (music.volume < 0f)
			{
				music.volume = 0f;
			}
		}
	}
}
