using UnityEngine;
using System.Collections;

public class Band : MonoBehaviour {

	public Transform[] musicians;
	private float timeCounter = 1f;
	private float period = 1f;
	private float time;

	void Start ()
	{
		audio.Play ();
	}

	void Update ()
	{
		timeCounter += Time.deltaTime;
		if (Input.GetMouseButtonDown(0))
		{
			timeCounter = 0f;
			for (int i = 0; i < musicians.Length; i++)
			{
				musicians[i].rotation = Quaternion.identity;
				musicians[i].Rotate (Vector3.forward * Random.Range (-20f, 20f));
			}

			if (!audio.isPlaying)
			{
				audio.time = time;
				audio.Play();
			}
		}

		if (timeCounter > period && audio.isPlaying)
		{
			time = audio.time;
			audio.Pause();
		}

		if (audio.time > 15f)
		{
			Game.done = true;
		}
	}
}
