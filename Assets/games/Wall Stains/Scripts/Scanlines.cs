using UnityEngine;
using System.Collections;

namespace Games.WallStains
{
	public class Scanlines : MonoBehaviour {

		void Update ()
	    {
	        //transform.position = new Vector3(transform.position.x, transform.parent.position.y + Random.Range(-540f, 540f), transform.position.z);
	        transform.localScale = Vector3.one + Vector3.up * Random.value;
		}
	}
}
