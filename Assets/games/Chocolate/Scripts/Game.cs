using UnityEngine;
using System.Collections;

namespace Games.Chocolate
{
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
			//Cursor.visible = false;
			GetComponent<AudioSource>().clip = chocolate;
			GetComponent<AudioSource>().volume = 1f;
			GetComponent<AudioSource>().Play ();
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
					GetComponent<AudioSource>().clip = rhythm;
					GetComponent<AudioSource>().volume = 0.3f;
					GetComponent<AudioSource>().Play ();
				}
				else if (slideCounter == 17)
				{
					GetComponent<AudioSource>().clip = chocolate;
					GetComponent<AudioSource>().volume = 1f;
	//				audio.time = 72f;
					GetComponent<AudioSource>().Play ();
				}
			}

			// Escape used to quit the application here. The collection binds its own exit
			// combination (Select+Start, or Shift+Escape), so a bare Escape quitting is both
			// redundant and a way to lose the whole collection by accident.
		}
	}
}
