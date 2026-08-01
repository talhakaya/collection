using UnityEngine;
using System.Collections;

namespace Games.Chocolate
{
	public class Bus : MonoBehaviour {

		public SpriteRenderer[] busmen;
		public Sprite busman;
		public Sprite busdead;
		public AudioClip bus;
		public AudioClip gun;
		private bool moving;
		private float timeCounter;
		private float period = 2f;
		private float period2 = 2f;
		private float animTimeCounter;
		private float animPeriod = 0.05f;
		private int busmenCounter;

		void Start ()
		{
			timeCounter = period2;
		}

		void Update ()
		{
			timeCounter += Time.deltaTime;
			if (!moving)
			{
				if (busmenCounter < busmen.Length && timeCounter >= period2)
				{
					timeCounter = 0f;
					moving = true;
					GetComponent<AudioSource>().clip = bus;
					GetComponent<AudioSource>().volume = 0.3f;
					GetComponent<AudioSource>().Play ();
				}
			}
			else
			{
				animTimeCounter += Time.deltaTime;
				if (animTimeCounter >= animPeriod)
				{
					animTimeCounter = 0f;
					transform.position = Vector3.up * Random.Range (-0.1f, 0.1f);
				}

				if (timeCounter >= period)
				{
					timeCounter = 0f;
					moving = false;
					busmen[busmenCounter].sprite = busdead;
					if (busmen[busmenCounter].transform.position.y < -0.1)
					{
						busmen[busmenCounter].transform.position += Vector3.up * 0.4f;
						busmen[busmenCounter].transform.localScale = Vector3.one * 0.5f;
					}
					busmenCounter++;
					GetComponent<AudioSource>().clip = gun;
					GetComponent<AudioSource>().volume = 1f;
					GetComponent<AudioSource>().Play ();
				}
			}
		}
	}
}
