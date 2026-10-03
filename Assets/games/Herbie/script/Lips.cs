using UnityEngine;
using System.Collections;

namespace Games.Herbie
{
	public class Lips : MonoBehaviour {

		bool done = false;
		// Use this for initialization
		void Start () {

		}

		// Update is called once per frame
		void Update () {

		}

		void OnTriggerEnter2D(Collider2D other)
		{
			if (!done)
			{
				done = true;
				Game.instance.aKiss.Play ();
				if (Game.miniGame.name.Contains("momKiss"))
				{
					MomKiss.instance.kiss ();
				}
				else if (Game.miniGame.name.Contains("end0"))
				{
					End0.instance.kiss ();
				}
			}
		}
	}
}
