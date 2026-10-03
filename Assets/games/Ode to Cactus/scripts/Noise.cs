using UnityEngine;
using System.Collections;

namespace Games.OdeToCactus
{
	public class Noise : MonoBehaviour {

		void Start ()
	    {
	        transform.position = new Vector3(Random.Range(-12f, 12f), Random.Range(-7f, 7f), 0f);
		}

		void Update ()
	    {
	        //transform.position = new Vector3(Random.Range(-8f, 8f), Random.Range(-5f, 5f), 0f);
	        transform.position = new Vector3(Random.Range(-12f, 12f), Random.Range(-7f, 7f), 0f);
		}
	}
}
