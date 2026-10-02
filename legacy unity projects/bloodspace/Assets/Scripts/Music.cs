using UnityEngine;
using System.Collections;

public class Music : MonoBehaviour {

	public static float volume = 0.5f;
	public static AudioSource audioSource;
	public static float MenuVolume = 0.3f;
	public static float GameVolume = 0.6f;

	void Start ()
	{
		audioSource = audio;
//		AudioListener.volume = 0.2f;
		if (PlayerPrefs.GetInt ("musicOn", 1) == 1)
		{
			audio.volume = volume;
		}
		else
		{
			audio.volume = 0;
		}
	}

	void Update () 
	{
		if (Input.GetButtonDown("MusicOnOff"))
		{
			if (audio.volume > 0)
			{
				audio.volume = 0;
				PlayerPrefs.SetInt ("musicOn", 0);
			}
			else
			{
				audio.volume = volume;
				PlayerPrefs.SetInt ("musicOn", 1);
			}
		}

		if (audio.volume != 0)
		{
			audio.volume = volume;
		}

		if (Input.GetButtonDown("Quit"))
		{
			Application.Quit ();
		}

		if (Input.GetButtonDown("Fullscreen"))
		{
			Screen.fullScreen = !Screen.fullScreen;
		}
	}
}
