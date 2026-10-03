using UnityEngine;
using System.Collections;

namespace Games.LostShader
{
	public class ZSorterContinuous : MonoBehaviour
	{
	    public GameObject refObject;

		void Start ()
	    {
	        if (refObject == null)
	        {
	            refObject = gameObject;
	        }
		}

		void Update ()
	    {
	        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.y * 0.1f);
		}
	}
}
