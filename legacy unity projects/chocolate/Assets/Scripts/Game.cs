using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {
	
	public GameObject referenceObject;
	public AudioClip chocolate;
	public AudioClip rhythm;
	public GameObject[] thingsToActivate;
	public static GameObject reference;
	private int slideCounter;

	public static bool done;

	void Awake ()
	{
		reference = referenceObject;
	}

	void Start ()
	{
		slideCounter = 0;
		done = true;
		Screen.showCursor = false;
		audio.clip = chocolate;
		audio.volume = 1f;
		audio.Play ();
	}

	void Update ()
	{
		if (done)
		{
			if (slideCounter > 0)
			{
				Destroy(thingsToActivate[slideCounter - 1]);
			}
			if (thingsToActivate.Length > slideCounter)
			{
				thingsToActivate[slideCounter].SetActive(true);
			}
			else
			{
				Debug.Log ("Game is finished");
			}
			slideCounter++;
			done = false;

			if (slideCounter == 8)
			{
				audio.clip = rhythm;
				audio.volume = 0.3f;
				audio.Play ();
			}
			else if (slideCounter == 17)
			{
				audio.clip = chocolate;
				audio.volume = 1f;
//				audio.time = 72f;
				audio.Play ();
			}
		}

		if (Input.GetKey(KeyCode.Escape))
		{
			Application.Quit ();
		}
	}
}
