using UnityEngine;
using System.Collections;

namespace Games.OdeToCactus
{
	public class MondoFace : MonoBehaviour {

	    private Vector3 firstPos;

		void Start ()
	    {
	        firstPos = transform.localPosition;
	        transform.parent.GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
		}

		void Update ()
	    {
	        transform.localPosition = firstPos + new Vector3(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f), 0f);
		}
	}
}
