using UnityEngine;
using System.Collections;
using Collection.Controls;

namespace Games.Chocolate
{
	public class Band : MonoBehaviour {

		public Transform[] musicians;
		private float timeCounter = 1f;
		private float period = 1f;
		private float time;

		void Start ()
		{
			GetComponent<AudioSource>().Play ();
		}

		void Update ()
		{
			timeCounter += Time.deltaTime;
			if (TaloketoInputManager.GetMouseButtonDown(0))
			{
				timeCounter = 0f;
				for (int i = 0; i < musicians.Length; i++)
				{
					musicians[i].rotation = Quaternion.identity;
					musicians[i].Rotate (Vector3.forward * Random.Range (-20f, 20f));
				}

				if (!GetComponent<AudioSource>().isPlaying)
				{
					GetComponent<AudioSource>().time = time;
					GetComponent<AudioSource>().Play();
				}
			}

			if (timeCounter > period && GetComponent<AudioSource>().isPlaying)
			{
				time = GetComponent<AudioSource>().time;
				GetComponent<AudioSource>().Pause();
			}

			if (GetComponent<AudioSource>().time > 15f)
			{
				Game.done = true;
			}
		}
	}
}
