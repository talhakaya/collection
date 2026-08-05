using UnityEngine;
using System.Collections;

namespace Games.Abused
{
	public class Tree : MonoBehaviour {

		void Start ()
	    {
	        transform.Rotate(Vector3.forward * Random.Range(0f, 360f));
	        transform.localScale = Vector3.one * Random.Range(0.8f, 1.2f);
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	}

}
