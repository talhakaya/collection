using UnityEngine;
using System.Collections;
[RequireComponent (typeof(AudioSource))]

public class VideoPlay : MonoBehaviour {

	public MovieTexture movTexture;
	public bool skipScene;
	public bool playSound;
	void Start()
	{
		GetComponent<Renderer>().material.mainTexture = movTexture;
		movTexture.loop = !skipScene;
		if (playSound)
		{
			GetComponent<AudioSource> ().clip = movTexture.audioClip;
			GetComponent<AudioSource> ().Play ();
		}
		movTexture.Play();
	}

	void Update()
	{
		if (skipScene && !movTexture.isPlaying)
		{
			if (Application.loadedLevel == 9)
			{
				Application.Quit();
			}
			else
			{
				Application.LoadLevel(Application.loadedLevel + 1);
			}
		}
	}
}
