using UnityEngine;
using System.Collections;

namespace Games.Orphan
{
	public class SoundManager : MonoBehaviour
	{
		private float time;
		private bool soundPutDone;

		public GameObject soundObject;
		public float timeToPut;

		void Start ()
		{
			time = 0f;
			soundPutDone = false;
		}

		void Update ()
		{
			if (!soundPutDone)
			{
				time += Time.deltaTime;
				if (time > timeToPut)
				{
					soundPutDone = true;
					Instantiate(soundObject, transform.position, transform.rotation);
				}
			}
		}
	}
}
