using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.BloodSpace
{
	public class Music : MonoBehaviour {

		public static float volume = 0.5f;
		public static AudioSource audioSource;
		public static float MenuVolume = 0.3f;
		public static float GameVolume = 0.6f;

		void Start ()
		{
			audioSource = GetComponent<AudioSource>();
	//		AudioListener.volume = 0.2f;
			if (PlayerPrefs.GetInt ("BloodSpace.musicOn", 1) == 1)
			{
				GetComponent<AudioSource>().volume = volume;
			}
			else
			{
				GetComponent<AudioSource>().volume = 0;
			}
		}

		void Update () 
		{
			if (TaloketoInputManager.GetButtonDown("MusicOnOff"))
			{
				if (GetComponent<AudioSource>().volume > 0)
				{
					GetComponent<AudioSource>().volume = 0;
					PlayerPrefs.SetInt ("BloodSpace.musicOn", 0);
				}
				else
				{
					GetComponent<AudioSource>().volume = volume;
					PlayerPrefs.SetInt ("BloodSpace.musicOn", 1);
				}
			}

			if (GetComponent<AudioSource>().volume != 0)
			{
				GetComponent<AudioSource>().volume = volume;
			}

			// In the collection: the Escape quit and the F4 fullscreen switch that stood here are gone;
			// the collection has its own exit and its own display settings.
		}
	}
}
